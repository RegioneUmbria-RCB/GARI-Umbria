
Imports System.Data.Common
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Stalla_Consistenze_R
    Inherits AgronicaCoreDataProvider.DataProvider




    '##############################################################################################
    'Public Function Leggi(
    '                        ByVal ID_Consistenze As Int32,
    '                        ByVal Piva As String,
    '                        ByVal Sa_Cod As Int32,
    '                        ByVal STA_NUM As Int32,
    '                        ByVal Gen_Cod As Int32,
    '                        ByVal Spe_Cod As Int32,
    '                        ByVal Raz_Cod As Int32,
    '                        ByVal Ipro_Cod As Int32,
    '                        ByVal Cat_Cod As Int32,
    '                            ByVal FinestraTemp_Inizio As Date,
    '                            ByVal FinestraTemp_Fine As Date,
    '                            ByRef objConnessione As DbConnection,
    '                            ByVal StringaConnessione As String,
    '                            ByVal FlagVisibilita As Int32,
    '                            ByVal DirectoryLOG As String,
    '                            ByVal FileLOG As String,
    '                            ByVal IdentificatoreUtente As String
    '                            ) As DataTable

    '    Dim NomeRoutine As String = "AgronicaCoreZooDAL.Stalla_Consistenze_R.Leggi()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   ID_Consistenze = 0 
    '    '   Piva = ""
    '    '   Sa_Cod = 0
    '    '   STA_NUM = 0
    '    '   Gen_Cod = 0    
    '    '   Spe_Cod = 0
    '    '   Raz_Cod = 0
    '    '   Ipro_Cod = 0
    '    '   Cat_Cod = 0
    '    '
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim DT As DataTable

    '    Try

    '        '------------------------------------------------------------------
    '        StrSQL.Length = 0

    '        StrSQL.Append(" SELECT * ")
    '        StrSQL.Append(" FROM  Stalla_Consistenze ")
    '        StrSQL.Append(" WHERE Validita_inizio < " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " ")
    '        StrSQL.Append(" AND   Validita_Fine > " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")

    '        If ID_Consistenze <> 0 Then
    '            StrSQL.Append(" AND ID_CONSISTENZE =  " & Agro_SQL_SaveNum(ID_Consistenze) & " ")
    '        End If

    '        If Piva <> "" Then
    '            StrSQL.Append(" AND PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "'")
    '        End If

    '        If Sa_Cod <> 0 Then
    '            StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
    '        End If

    '        If STA_NUM <> 0 Then
    '            StrSQL.Append(" AND STA_NUM = " & Agro_SQL_SaveNum(STA_NUM) & "  ")
    '        End If

    '        If Gen_Cod <> 0 Then
    '            StrSQL.Append(" AND GEN_COD = " & Agro_SQL_SaveNum(Gen_Cod) & "  ")
    '        End If

    '        If Spe_Cod <> 0 Then
    '            StrSQL.Append(" AND SPE_COD = " & Agro_SQL_SaveNum(Spe_Cod) & "  ")
    '        End If

    '        If Raz_Cod <> 0 Then
    '            StrSQL.Append(" AND RAZ_COD = " & Agro_SQL_SaveNum(Raz_Cod) & "  ")
    '        End If

    '        If Ipro_Cod <> 0 Then
    '            StrSQL.Append(" AND IPRO_COD = " & Agro_SQL_SaveNum(Ipro_Cod) & "  ")
    '        End If

    '        If Cat_Cod <> 0 Then
    '            StrSQL.Append(" AND CAT_COD = " & Agro_SQL_SaveNum(Cat_Cod) & "  ")
    '        End If

    '        Select Case FlagVisibilita
    '            Case 1  'Solo i NON CANCELLATI
    '                StrSQL.Append(" AND   Stalla_Consistenze.Inviato >=0 ")
    '            Case 2  'Solo i CANCELLATI
    '                StrSQL.Append(" AND   Stalla_Consistenze.Inviato =-1 ")
    '            Case 3  'TUTTI
    '                '
    '            Case Else
    '                Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
    '        End Select

    '        StrSQL.Append(" ORDER BY CAT_COD ASC ")

    '        '------------------------------------------------------------------

    '        DT = EseguiQuery_Lettura(objConnessione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        DT = Nothing
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return DT

    'End Function





    Public Function Leggi( _
                            ByVal ID_Consistenze As Int32, _
                            ByVal Piva As String, _
                            ByVal Sa_Cod As Int32, _
                            ByVal STA_NUM As Int32, _
                            ByVal Gen_Cod As Int32, _
                            ByVal Spe_Cod As Int32, _
                            ByVal Raz_Cod As Int32, _
                            ByVal Ipro_Cod As Int32, _
                            ByVal Cat_Cod As Int32, _
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreZooDAL.Stalla_Consistenze_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   ID_Consistenze = 0 
        '   Piva = ""
        '   Sa_Cod = 0
        '   STA_NUM = 0
        '   Gen_Cod = 0    
        '   Spe_Cod = 0
        '   Raz_Cod = 0
        '   Ipro_Cod = 0
        '   Cat_Cod = 0
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    'TODO
                    'modificare la query di select
                    '------------------------------------------------------------------
                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  Stalla_Consistenze ")
                    StrSQL.Append(" WHERE Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If ID_Consistenze <> 0 Then
                        StrSQL.Append(" AND ID_CONSISTENZE =  " & Agro_SQL_SaveNum(ID_Consistenze) & " ")
                    End If

                    If Piva <> "" Then
                        StrSQL.Append(" AND PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "'")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
                    End If

                    If STA_NUM <> 0 Then
                        StrSQL.Append(" AND STA_NUM = " & Agro_SQL_SaveNum(STA_NUM) & "  ")
                    End If

                    If Gen_Cod <> 0 Then
                        StrSQL.Append(" AND GEN_COD = " & Agro_SQL_SaveNum(Gen_Cod) & "  ")
                    End If

                    If Spe_Cod <> 0 Then
                        StrSQL.Append(" AND SPE_COD = " & Agro_SQL_SaveNum(Spe_Cod) & "  ")
                    End If

                    If Raz_Cod <> 0 Then
                        StrSQL.Append(" AND RAZ_COD = " & Agro_SQL_SaveNum(Raz_Cod) & "  ")
                    End If

                    If Ipro_Cod <> 0 Then
                        StrSQL.Append(" AND IPRO_COD = " & Agro_SQL_SaveNum(Ipro_Cod) & "  ")
                    End If

                    If Cat_Cod <> 0 Then
                        StrSQL.Append(" AND CAT_COD = " & Agro_SQL_SaveNum(Cat_Cod) & "  ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Stalla_Consistenze.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Stalla_Consistenze.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY CAT_COD ASC")
                    End If
                    '------------------------------------------------------------------


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
                    'TODO
                    'modificare la query di select
                    '------------------------------------------------------------------
                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  Stalla_Consistenze ")
                    StrSQL.Append(" WHERE Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If ID_Consistenze <> 0 Then
                        StrSQL.Append(" AND ID_CONSISTENZE =  " & Agro_SQL_SaveNum(ID_Consistenze) & " ")
                    End If

                    If Piva <> "" Then
                        StrSQL.Append(" AND PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "'")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
                    End If

                    If STA_NUM <> 0 Then
                        StrSQL.Append(" AND STA_NUM = " & Agro_SQL_SaveNum(STA_NUM) & "  ")
                    End If

                    If Gen_Cod <> 0 Then
                        StrSQL.Append(" AND GEN_COD = " & Agro_SQL_SaveNum(Gen_Cod) & "  ")
                    End If

                    If Spe_Cod <> 0 Then
                        StrSQL.Append(" AND SPE_COD = " & Agro_SQL_SaveNum(Spe_Cod) & "  ")
                    End If

                    If Raz_Cod <> 0 Then
                        StrSQL.Append(" AND RAZ_COD = " & Agro_SQL_SaveNum(Raz_Cod) & "  ")
                    End If

                    If Ipro_Cod <> 0 Then
                        StrSQL.Append(" AND IPRO_COD = " & Agro_SQL_SaveNum(Ipro_Cod) & "  ")
                    End If

                    If Cat_Cod <> 0 Then
                        StrSQL.Append(" AND CAT_COD = " & Agro_SQL_SaveNum(Cat_Cod) & "  ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Stalla_Consistenze.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Stalla_Consistenze.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY CAT_COD ASC")
                    End If
                    '------------------------------------------------------------------
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni
                    '
                    '
                    '
                    '


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




End Class





'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################





Public Class Stalla_Consistenze_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '<<<<< WORK IN PROGRESS >>>>>


    '============================================================================
    'Public Function Scrivi(
    '                        ByVal ID_Consistenze As Int32,
    '                        ByVal Piva As String,
    '                        ByVal Sa_Cod As Int32,
    '                        ByVal STA_NUM As Int32,
    '                        ByVal Gen_Cod As Int32,
    '                        ByVal Spe_Cod As Int32,
    '                        ByVal Raz_Cod As Int32,
    '                        ByVal Ipro_Cod As Int32,
    '                        ByVal Cat_Cod As Int32,
    '                        ByVal Data_Consistenza As Date,
    '                        ByVal Flag_Biologico As Integer,
    '                        ByVal Data_Conv_Inizio As Date,
    '                        ByVal Data_Conv_Fine As Date,
    '                        ByVal Reg_Cod As Int32,
    '                        ByVal Sesso As String,
    '                        ByVal Numero_Capi As Int32,
    '                        ByVal Numero_Famiglie As Int32,
    '                        ByVal UDM_Numero As Integer,
    '                        ByVal Peso_Vivo_Stimato As Decimal,
    '                        ByVal Prod_Descr As String,
    '                        ByVal Prod_UDM As Int32,
    '                        ByVal Prod_Qta As Decimal,
    '                        ByVal Prod_Lotto As Int32,
    '                        ByVal Prod_Categoria As Int32,
    '                        ByVal Dat_Ult_Agg As Date,
    '                            ByVal UserName_Creazione As String,
    '                            ByVal FinestraTemp_Inizio As Date,
    '                            ByVal FinestraTemp_Fine As Date,
    '                            ByRef objConnessione As DbConnection,
    '                            ByRef objTransazione As DbTransaction,
    '                            ByVal StringaConnessione As String,
    '                            ByVal DirectoryLOG As String,
    '                            ByVal FileLOG As String,
    '                            ByVal IdentificatoreUtente As String
    '                            ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreZooDAL.Stalla_Consistenze_W.Scrivi()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    Try

    '        '---------------------------------------------
    '        StrSQL.Length = 0

    '        StrSQL.Append(" INSERT INTO Stalla_Consistenze ")
    '        StrSQL.Append("          (")
    '        StrSQL.Append("          ID_CONSISTENZE, PIVA,   Sa_Cod,    STA_NUM, ")
    '        StrSQL.Append("          GEN_COD, SPE_COD, RAZ_COD, IPRO_COD, CAT_COD, ")
    '        StrSQL.Append("          DAT_DAL, FLAG_BIOLOGICO, DATA_CONV_INIZIO, DATA_CONV_FINE, REG_COD, SESSO, NUM_CAPI, NUM_Famiglie, UDM_NUMERO, ")
    '        StrSQL.Append("          PESO_VIVO_STIM, PROD_DESCR, PROD_UDM, PROD_QTA, PROD_LOTTO, Prod_Cat, ")
    '        StrSQL.Append("          DAT_ULT_AGG,  ")

    '        StrSQL.Append("          Inviato, DataInvio, ")
    '        StrSQL.Append("          Data_Creazione,     Data_Modifica, ")
    '        StrSQL.Append("          UserName_Creazione, UserName_Modifica, ")
    '        StrSQL.Append("          Validita_Inizio,    Validita_Fine ")
    '        StrSQL.Append("          ) ")

    '        StrSQL.Append(" VALUES ( ")
    '        StrSQL.Append("           " & Agro_SQL_SaveNum(ID_Consistenze) & "  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(STA_NUM) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Gen_Cod) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Spe_Cod) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Raz_Cod) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Ipro_Cod) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Cat_Cod) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_Consistenza) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Flag_Biologico) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_Conv_Inizio) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_Conv_Fine) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Reg_Cod) & "  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(Sesso)) & "'  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Numero_Capi) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Numero_Famiglie) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(UDM_Numero) & "  ")

    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Peso_Vivo_Stimato) & "  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(Prod_Descr) & "' ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Prod_UDM) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Prod_Qta) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Prod_Lotto) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Prod_Categoria) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(Dat_Ult_Agg) & "  ")

    '        StrSQL.Append("         , 0  ")
    '        StrSQL.Append("         , Null  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(UserName_Creazione) & "' ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(UserName_Creazione) & "' ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(FinestraTemp_Fine) & "  ")

    '        StrSQL.Append(") ")

    '        '---------------------------------------------
    '        xRisp = EseguiQuery_Scrittura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return xRisp

    'End Function


    Public Function Scrivi( _
                                ByVal ID_Consistenze As Int32, _
                                ByVal Piva As String, _
                                ByVal Sa_Cod As Int32, _
                                ByVal STA_NUM As Int32, _
                                ByVal Gen_Cod As Int32, _
                                ByVal Spe_Cod As Int32, _
                                ByVal Raz_Cod As Int32, _
                                ByVal Ipro_Cod As Int32, _
                                ByVal Cat_Cod As Int32, _
                                ByVal Data_Consistenza As Date, _
                                ByVal Flag_Biologico As Integer, _
                                ByVal Data_Conv_Inizio As Date, _
                                ByVal Data_Conv_Fine As Date, _
                                ByVal Reg_Cod As Int32, _
                                ByVal Sesso As String, _
                                ByVal Numero_Capi As Int32, _
                                ByVal Numero_Famiglie As Int32, _
                                ByVal UDM_Numero As Integer, _
                                ByVal Peso_Vivo_Stimato As Decimal, _
                                ByVal Prod_Descr As String, _
                                ByVal Prod_UDM As Int32, _
                                ByVal Prod_Qta As Decimal, _
                                ByVal Prod_Lotto As Int32, _
                                ByVal Prod_Categoria As Int32, _
                                ByVal Dat_Ult_Agg As Date, _
                                    ByVal Validita_Inizio As Date, _
                                    ByVal Validita_Fine As Date, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreZooDAL.Stalla_Consistenze_W.Scrivi()"

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

            StrSQL.Append(" INSERT INTO Stalla_Consistenze ")
            StrSQL.Append("          (")
            StrSQL.Append("          ID_CONSISTENZE, PIVA,   Sa_Cod,    STA_NUM, ")
            StrSQL.Append("          GEN_COD, SPE_COD, RAZ_COD, IPRO_COD, CAT_COD, ")
            StrSQL.Append("          DAT_DAL, FLAG_BIOLOGICO, DATA_CONV_INIZIO, DATA_CONV_FINE, REG_COD, SESSO, NUM_CAPI, NUM_Famiglie, UDM_NUMERO, ")
            StrSQL.Append("          PESO_VIVO_STIM, PROD_DESCR, PROD_UDM, PROD_QTA, PROD_LOTTO, Prod_Cat, ")
            StrSQL.Append("          DAT_ULT_AGG,  ")

            StrSQL.Append("          Inviato, DataInvio, ")
            StrSQL.Append("          Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("          UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("          Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("          ) ")

            StrSQL.Append(" VALUES ( ")
            StrSQL.Append("           " & Agro_SQL_SaveNum(ID_Consistenze) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(STA_NUM) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Gen_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Spe_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Raz_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Ipro_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Cat_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_Consistenza) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Flag_Biologico) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_Conv_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_Conv_Fine) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Reg_Cod) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(Sesso)) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Numero_Capi) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Numero_Famiglie) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(UDM_Numero) & "  ")

            StrSQL.Append("         , " & Agro_SQL_SaveNum(Peso_Vivo_Stimato) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Prod_Descr) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Prod_UDM) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Prod_Qta) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Prod_Lotto) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Prod_Categoria) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Dat_Ult_Agg) & "  ")

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





    '============================================================================
    'Public Function Modifica(
    '                        ByVal ID_Consistenze As Int32,
    '                        ByVal Piva As String,
    '                        ByVal Sa_Cod As Int32,
    '                        ByVal STA_NUM As Int32,
    '                        ByVal Gen_Cod As Int32,
    '                        ByVal Spe_Cod As Int32,
    '                        ByVal Raz_Cod As Int32,
    '                        ByVal Ipro_Cod As Int32,
    '                        ByVal Cat_Cod As Int32,
    '                        ByVal Data_Consistenza As Date,
    '                        ByVal Flag_Biologico As Integer,
    '                        ByVal Data_Conv_Inizio As Date,
    '                        ByVal Data_Conv_Fine As Date,
    '                        ByVal Reg_Cod As Int32,
    '                        ByVal Sesso As String,
    '                        ByVal Num_Capi As Int32,
    '                        ByVal Num_Famiglie As Int32,
    '                        ByVal UDM_Numero As Integer,
    '                        ByVal Peso_Vivo_Stimato As Decimal,
    '                        ByVal Prod_Descr As String,
    '                        ByVal Prod_UDM As Int32,
    '                        ByVal Prod_Qta As Decimal,
    '                        ByVal Prod_Lotto As Int32,
    '                        ByVal Prod_Categoria As Int32,
    '                        ByVal Dat_Ult_Agg As Date,
    '                            ByVal UserName_Modifica As String,
    '                            ByVal FinestraTemp_Inizio As Date,
    '                            ByVal FinestraTemp_Fine As Date,
    '                            ByRef objConnessione As DbConnection,
    '                            ByRef objTransazione As DbTransaction,
    '                            ByVal StringaConnessione As String,
    '                            ByVal DirectoryLOG As String,
    '                            ByVal FileLOG As String,
    '                            ByVal IdentificatoreUtente As String
    '                            ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreZooDAL.Stalla_Consistenze_W.Modifica()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    '------------------------------

    '    Try

    '        If ID_Consistenze = 0 Then
    '            Throw New Exception("Parametro non corretto nella query (ID_Consistenze obbligatorio)")
    '        End If

    '        '---------------------------------------------

    '        StrSQL.Length = 0

    '        StrSQL.Append(" UPDATE Stalla_Consistenze SET ")
    '        StrSQL.Append("    PIVA           ='" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
    '        StrSQL.Append("   ,sa_cod     = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
    '        StrSQL.Append("   ,STA_NUM    = " & Agro_SQL_SaveNum(STA_NUM) & "  ")
    '        StrSQL.Append("   ,GEN_COD    = " & Agro_SQL_SaveNum(Gen_Cod) & "  ")
    '        StrSQL.Append("   ,SPE_COD    = " & Agro_SQL_SaveNum(Spe_Cod) & "  ")
    '        StrSQL.Append("   ,RAZ_COD    = " & Agro_SQL_SaveNum(Raz_Cod) & "  ")
    '        StrSQL.Append("   ,IPRO_COD   = " & Agro_SQL_SaveNum(Ipro_Cod) & "  ")
    '        StrSQL.Append("   ,CAT_COD    = " & Agro_SQL_SaveNum(Cat_Cod) & "  ")
    '        StrSQL.Append("   ,DAT_DAL    = " & Agro_SQL_SaveDate(Data_Consistenza) & "  ")
    '        StrSQL.Append("   ,FLAG_BIOLOGICO    = " & Agro_SQL_SaveNum(Flag_Biologico) & "  ")

    '        StrSQL.Append("   ,DATA_CONV_INIZIO  =  " & Agro_SQL_SaveDate(Data_Conv_Inizio))
    '        StrSQL.Append("   ,DATA_CONV_FINE    =  " & Agro_SQL_SaveDate(Data_Conv_Fine))
    '        StrSQL.Append("   ,REG_COD           =  " & Agro_SQL_SaveNum(Reg_Cod) & "  ")
    '        StrSQL.Append("   ,SESSO             = '" & Agro_SQL_SaveText(Trim(Sesso)) & "'  ")
    '        StrSQL.Append("   ,NUM_CAPI          =  " & Agro_SQL_SaveNum(Num_Capi) & "  ")
    '        StrSQL.Append("   ,NUM_Famiglie      =  " & Agro_SQL_SaveNum(Num_Famiglie) & "  ")
    '        StrSQL.Append("   ,UDM_Numero        =  " & Agro_SQL_SaveNum(UDM_Numero) & "  ")
    '        StrSQL.Append("   ,PESO_VIVO_STIM =  " & Agro_SQL_SaveNum(Peso_Vivo_Stimato) & "  ")
    '        StrSQL.Append("   ,PROD_DESCR        = '" & Agro_SQL_SaveText(Prod_Descr) & "' ")
    '        StrSQL.Append("   ,PROD_UDM          =  " & Agro_SQL_SaveNum(Prod_UDM) & "  ")
    '        StrSQL.Append("   ,PROD_LOTTO        =  " & Agro_SQL_SaveNum(Prod_Lotto) & "  ")
    '        StrSQL.Append("   ,PROD_CAT          =  " & Agro_SQL_SaveNum(Prod_Categoria) & "  ")
    '        StrSQL.Append("   ,DAT_ULT_AGG       =  " & Agro_SQL_SaveDate(Dat_Ult_Agg) & " ")

    '        StrSQL.Append("   ,Inviato           =  0 ")
    '        StrSQL.Append("   ,DataInvio         =  Null ")
    '        StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
    '        StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(UserName_Modifica) & "'")
    '        StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(FinestraTemp_Inizio))
    '        StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(FinestraTemp_Fine))

    '        StrSQL.Append(" WHERE ID_CONSISTENZE = " & Agro_SQL_SaveNum(ID_Consistenze) & "  ")

    '        '---------------------------------------------
    '        xRisp = EseguiQuery_Scrittura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return xRisp


    'End Function



    Public Function Modifica( _
                            ByVal ID_Consistenze As Int32, _
                            ByVal Piva As String, _
                            ByVal Sa_Cod As Int32, _
                            ByVal STA_NUM As Int32, _
                            ByVal Gen_Cod As Int32, _
                            ByVal Spe_Cod As Int32, _
                            ByVal Raz_Cod As Int32, _
                            ByVal Ipro_Cod As Int32, _
                            ByVal Cat_Cod As Int32, _
                            ByVal Data_Consistenza As Date, _
                            ByVal Flag_Biologico As Integer, _
                            ByVal Data_Conv_Inizio As Date, _
                            ByVal Data_Conv_Fine As Date, _
                            ByVal Reg_Cod As Int32, _
                            ByVal Sesso As String, _
                            ByVal Num_Capi As Int32, _
                            ByVal Num_Famiglie As Int32, _
                            ByVal UDM_Numero As Integer, _
                            ByVal Peso_Vivo_Stimato As Decimal, _
                            ByVal Prod_Descr As String, _
                            ByVal Prod_UDM As Int32, _
                            ByVal Prod_Qta As Decimal, _
                            ByVal Prod_Lotto As Int32, _
                            ByVal Prod_Categoria As Int32, _
                            ByVal Dat_Ult_Agg As Date, _
                                ByVal Validita_Inizio As Date, _
                                ByVal Validita_Fine As Date, _
                                    ByVal xFiltroAggiuntivo As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreZooDAL.Stalla_Consistenze_W.Modifica()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            If ID_Consistenze = 0 Then
                Throw New Exception("Parametro non corretto nella query (ID_Consistenze obbligatorio)")
            End If

            '---------------------------------------------

            StrSQL.Length = 0

            StrSQL.Append(" UPDATE Stalla_Consistenze SET ")
            StrSQL.Append("    PIVA           ='" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            StrSQL.Append("   ,sa_cod     = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append("   ,STA_NUM    = " & Agro_SQL_SaveNum(STA_NUM) & "  ")
            StrSQL.Append("   ,GEN_COD    = " & Agro_SQL_SaveNum(Gen_Cod) & "  ")
            StrSQL.Append("   ,SPE_COD    = " & Agro_SQL_SaveNum(Spe_Cod) & "  ")
            StrSQL.Append("   ,RAZ_COD    = " & Agro_SQL_SaveNum(Raz_Cod) & "  ")
            StrSQL.Append("   ,IPRO_COD   = " & Agro_SQL_SaveNum(Ipro_Cod) & "  ")
            StrSQL.Append("   ,CAT_COD    = " & Agro_SQL_SaveNum(Cat_Cod) & "  ")
            StrSQL.Append("   ,DAT_DAL    = " & Agro_SQL_SaveDate(Data_Consistenza) & "  ")
            StrSQL.Append("   ,FLAG_BIOLOGICO    = " & Agro_SQL_SaveNum(Flag_Biologico) & "  ")

            StrSQL.Append("   ,DATA_CONV_INIZIO  =  " & Agro_SQL_SaveDate(Data_Conv_Inizio))
            StrSQL.Append("   ,DATA_CONV_FINE    =  " & Agro_SQL_SaveDate(Data_Conv_Fine))
            StrSQL.Append("   ,REG_COD           =  " & Agro_SQL_SaveNum(Reg_Cod) & "  ")
            StrSQL.Append("   ,SESSO             = '" & Agro_SQL_SaveText(Trim(Sesso)) & "'  ")
            StrSQL.Append("   ,NUM_CAPI          =  " & Agro_SQL_SaveNum(Num_Capi) & "  ")
            StrSQL.Append("   ,NUM_Famiglie      =  " & Agro_SQL_SaveNum(Num_Famiglie) & "  ")
            StrSQL.Append("   ,UDM_Numero        =  " & Agro_SQL_SaveNum(UDM_Numero) & "  ")
            StrSQL.Append("   ,PESO_VIVO_STIM =  " & Agro_SQL_SaveNum(Peso_Vivo_Stimato) & "  ")
            StrSQL.Append("   ,PROD_DESCR        = '" & Agro_SQL_SaveText(Prod_Descr) & "' ")
            StrSQL.Append("   ,PROD_UDM          =  " & Agro_SQL_SaveNum(Prod_UDM) & "  ")
            StrSQL.Append("   ,PROD_LOTTO        =  " & Agro_SQL_SaveNum(Prod_Lotto) & "  ")
            StrSQL.Append("   ,PROD_CAT          =  " & Agro_SQL_SaveNum(Prod_Categoria) & "  ")
            StrSQL.Append("   ,DAT_ULT_AGG       =  " & Agro_SQL_SaveDate(Dat_Ult_Agg) & " ")

            StrSQL.Append("   ,Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            StrSQL.Append(" WHERE ID_CONSISTENZE = " & Agro_SQL_SaveNum(ID_Consistenze) & "  ")



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

        Return xRisp


    End Function





    '============================================================================
    'Public Function Cancella(
    '                        ByVal ID_Consistenze As Int32,
    '                            ByVal UserName_Modifica As String,
    '                            ByVal FlagCancellazioneLogica As Int32,
    '                            ByRef objConnessione As DbConnection,
    '                            ByRef objTransazione As DbTransaction,
    '                            ByVal StringaConnessione As String,
    '                            ByVal DirectoryLOG As String,
    '                            ByVal FileLOG As String,
    '                            ByVal IdentificatoreUtente As String
    '                            ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreZooDAL.Stalla_Consistenze_W.Cancella()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    Try

    '        If ID_Consistenze = 0 Then
    '            Throw New Exception("Parametro non corretto nella query (ID_Consistenze obbligatorio)")
    '        End If

    '        '---------------------------------------------
    '        If FlagCancellazioneLogica Then

    '            StrSQL.Length = 0
    '            StrSQL.Append(" UPDATE Stalla_Consistenze ")
    '            StrSQL.Append(" SET ")
    '            StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(UserName_Modifica) & "' ")
    '            StrSQL.Append("      ,Inviato = -1 ")
    '            StrSQL.Append(" WHERE  Inviato >= 0 ")

    '        Else
    '            StrSQL.Length = 0
    '            StrSQL.Append(" DELETE ")
    '            StrSQL.Append(" FROM Stalla_Consistenze ")
    '            StrSQL.Append(" WHERE  1=1 ")

    '        End If

    '        StrSQL.Append(" AND ID_Consistenze = " & Agro_SQL_SaveNum(ID_Consistenze) & "   ")

    '        '---------------------------------------------
    '        xRisp = EseguiQuery_Scrittura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return xRisp

    'End Function



    Public Function Cancella( _
                            ByVal ID_Consistenze As Int32, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreZooDAL.Stalla_Consistenze_W.Cancella()"

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

            If ID_Consistenze = 0 Then
                Throw New Exception("Parametro non corretto nella query (ID_Consistenze obbligatorio)")
            End If

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then
                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Stalla_Consistenze ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")
            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM Stalla_Consistenze ")
                StrSQL.Append(" WHERE  1=1 ")
            End If

            StrSQL.Append(" AND ID_Consistenze = " & Agro_SQL_SaveNum(ID_Consistenze) & "   ")

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


    Public Function Aggiorna_Validazione( _
                                ByVal Validazione As Integer, _
                                ByVal UserName_Validazione As String, _
                                ByVal ID_Consistenze As Integer, _
                                ByVal Data_Validazione As Date, _
                                    ByVal xFiltroAggiuntivo As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreZooDAL.Stalla_Consistenze_W.Aggiorna_Validazione()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            'Controllo il valore del Campo Data_Validazione
            If (Data_Validazione = AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO) Then
                Data_Validazione = Date.Now
            End If

            '---------------------------------------------

            StrSQL.Length = 0

            '------------------------------

            'Query per la modifica dei dati                 ' #### CLASSE ####
            StrSQL.Append("UPDATE Stalla_Consistenze SET ")
            StrSQL.Append("    Validazione             =  " & Agro_SQL_SaveNum(Validazione) & "  ")
            StrSQL.Append("   ,UserName_Validazione    = '" & Agro_SQL_SaveText(UserName_Validazione) & "'  ")
            StrSQL.Append("   ,Data_Validazione        =  " & Agro_SQL_SaveDate(Data_Validazione) & "  ")
            StrSQL.Append(" WHERE    ID_CONSISTENZE     = " & Agro_SQL_SaveNum(ID_Consistenze) & "  ")

            '------------------------------



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

        Return xRisp


    End Function


    Public Function AggiornaValiditaFine( _
                                    ByVal ID_Consistenze As Integer, _
                                    ByVal Validita_Fine As Date, _
                                    ByVal xFiltroAggiuntivo As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreZooDAL.Stalla_Consistenze_W.AggiornaValiditaFine()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            '---------------------------------------------

            StrSQL.Length = 0

            '------------------------------

            'Query per la modifica dei dati                 ' #### CLASSE ####
            StrSQL.Append("UPDATE Stalla_consistenze SET ")
            StrSQL.Append("   UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Fine    =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append(" WHERE ID_CONSISTENZE    = " & Agro_SQL_SaveNum(ID_Consistenze) & "  ")
            StrSQL.Append(" AND   Validita_Fine     > " & Agro_SQL_SaveDate(Validita_Fine) & " ")

            '------------------------------



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

        Return xRisp


    End Function

End Class
