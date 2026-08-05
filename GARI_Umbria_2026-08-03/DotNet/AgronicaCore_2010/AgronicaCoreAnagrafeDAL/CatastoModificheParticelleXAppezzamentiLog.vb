Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate


Public Class CatastoModificheParticelleXAppezzamentiLog_R
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Leggi(
                  ByVal CatastoModifiche_COD As Integer _
                , ByVal PIVA As String _
                , ByVal SA_COD As Integer _
                , ByVal Programmazione_Entita_Cod As Integer _
                , ByVal APPEZZA As Integer _
                , ByVal PROV As String _
                , ByVal COM As String _
                , ByVal SEZIONE As String _
                , ByVal FOGLIO As Integer _
                , ByVal NUMERO As Integer _
                , ByVal SUBALTERNO As String _
                , ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile _
                , ByVal xFiltroAggiuntivo As String _
                , ByVal xOrderBy As String _
                , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CatastoModificheParticelleXAppezzamentiLog_W.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Dim DT As DataTable


        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    stb.Length = 0

                    stb.Append(" select mm.* " & vbCrLf)
                    stb.Append("     ,ist.COMUNI_PROV + ' (' + mm.prov + ') - ' +   " & vbCrLf)
                    stb.Append("      ist.LOCALITA + ' (' + mm.COM + ') - '  + " & vbCrLf)
                    stb.Append("      mm.sezione + ' - '  +  " & vbCrLf)
                    stb.Append("      cast(mm.FOGLIO as varchar(100))   + ' - '  +  " & vbCrLf)
                    stb.Append("      cast(mm.NUMERO as varchar(100))  + ' - '  +  " & vbCrLf)
                    stb.Append("      cast(mm.subalterno as varchar(100)) as catasto " & vbCrLf)
                    stb.Append("     , case when mm.operazione_DB = 1 then 'Verde' else  " & vbCrLf)
                    stb.Append("      case when mm.operazione_DB = 2 and abs(mm.percentualeScarto)>5 then 'Rosso' else  " & vbCrLf)
                    stb.Append("      case when mm.operazione_DB = 2 and abs(percentualeScarto)<=5 then 'Nero' else  " & vbCrLf)
                    stb.Append("                             'Blu' end end end as TipoOperazione_DB_descrizione " & vbCrLf)
                    stb.Append("  , case when mm.programmazione_entita_cod is null or mm.programmazione_entita_cod = 0 then 'Reale' else 'Pianificato' end as TipoDiImpianto" & vbCrLf)
                    stb.Append(" from CatastoModificheParticelleXAppezzamentiLog mm " & vbCrLf)
                    stb.Append("     inner join istat ist   " & vbCrLf)
                    stb.Append("         on ist.prov = mm.prov   " & vbCrLf)
                    stb.Append("         and ist.COM = mm.COM " & vbCrLf)


                    stb.Append("    where 1=1  " & vbCrLf)

                    '----- Condizioni
                    If (PIVA <> "") Then
                        stb.Append(" AND mm.PIVA = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
                    End If

                    If SA_COD <> 0 Then
                        stb.Append(" AND mm.Sa_Cod = " & Agro_SQL_SaveNum(SA_COD) & "  ")
                    End If

                    If APPEZZA <> 0 Then
                        stb.Append(" AND mm.Appezza = " & Agro_SQL_SaveNum(APPEZZA) & "  ")
                    End If

                    If PROV <> "" Then
                        stb.Append(" AND mm.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
                    End If

                    If COM <> "" Then
                        stb.Append(" AND mm.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
                    End If

                    If SEZIONE <> "0" Then
                        stb.Append(" AND mm.SEZIONE = '" & Agro_SQL_SaveText(Trim(LCase(SEZIONE))) & "' ")
                    End If

                    If FOGLIO <> 0 Then
                        stb.Append(" AND mm.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
                    End If

                    If NUMERO <> 0 Then
                        stb.Append(" AND mm.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
                    End If

                    If SUBALTERNO <> "0" Then
                        stb.Append(" AND mm.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
                    End If

                    If Programmazione_Entita_Cod <> "0" Then
                        stb.Append(" AND mm.Programmazione_Entita_Cod = " & Agro_vb_SaveNum(Programmazione_Entita_Cod) & " ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            stb.Append(" AND   mm.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            stb.Append(" AND   mm.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
                    '
                    '
                    '
                    '


            End Select



            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
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


Public Class CatastoModificheParticelleXAppezzamentiLog_W
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Scrivi(
                  ByVal CatastoModifiche_COD As Integer _
                , ByVal PIVA As String _
                , ByVal SA_COD As Integer _
                , ByVal APPEZZA As Integer _
                , ByVal Programmazione_Entita_cod As Integer _
                , ByVal PROV As String _
                , ByVal COM As String _
                , ByVal SEZIONE As String _
                , ByVal FOGLIO As Integer _
                , ByVal NUMERO As Integer _
                , ByVal SUBALTERNO As String _
                , ByVal rag_soc As String _
                , ByVal sa_nome As String _
                , ByVal Campo_Des As String _
                , ByVal p_Ettari_Are_Centiare As String _
                , ByVal inter_Ettari_Are_Centiare As String _
                , ByVal percentualeScarto As Decimal _
                , ByVal vecchioInter_Ettari_Are_Centiarevarchar As String _
                , ByVal App_nome As String _
                , ByVal impianto_Ettari_Are_Centiare As String _
                , ByVal operazione_DB As Integer _
                , ByVal CfgSalvataggio As String _
                , ByVal Validita_inizio As DateTime _
                , ByVal Validita_fine As DateTime _
                , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                , Optional ByVal Data_creazione As DateTime = #2/1/1900# _
                , Optional ByVal Data_modifica As DateTime = #2/1/1900# _
                , Optional ByVal username_creazione As String = "" _
                , Optional ByVal username_modifica As String = ""
            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CatastoModificheParticelleXAppezzamentiLog_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False



        If Data_creazione = #2/1/1900# Then
            Data_creazione = Date.Now
        End If

        If Data_modifica = #2/1/1900# Then
            Data_modifica = Date.Now
        End If

        If username_creazione = "" Then
            username_creazione = objParametri.UsernameOperazione
        End If

        If username_modifica = "" Then
            username_modifica = objParametri.UsernameOperazione
        End If



        Try

            stb.Append("Insert CatastoModificheParticelleXAppezzamentiLog (" & vbCrLf)
            stb.Append("   [CatastoModifiche_COD] " & vbCrLf)
            stb.Append("  ,[PIVA] " & vbCrLf)
            stb.Append("  ,[SA_COD] " & vbCrLf)
            stb.Append("  ,[APPEZZA] " & vbCrLf)
            stb.Append("  ,[PROV] " & vbCrLf)
            stb.Append("  ,[COM] " & vbCrLf)
            stb.Append("  ,[SEZIONE] " & vbCrLf)
            stb.Append("  ,[FOGLIO] " & vbCrLf)
            stb.Append("  ,[NUMERO] " & vbCrLf)
            stb.Append("  ,[SUBALTERNO] " & vbCrLf)
            stb.Append("  ,[rag_soc] " & vbCrLf)
            stb.Append("  ,[sa_nome] " & vbCrLf)
            stb.Append("  ,[Campo_Des] " & vbCrLf)
            stb.Append("  ,[p_Ettari_Are_Centiare] " & vbCrLf)
            stb.Append("  ,[inter_Ettari_Are_Centiare] " & vbCrLf)
            stb.Append("  ,[percentualeScarto] " & vbCrLf)
            stb.Append("  ,[vecchioInter_Ettari_Are_Centiare] " & vbCrLf)
            stb.Append("  ,[App_nome] " & vbCrLf)
            stb.Append("  ,[impianto_Ettari_Are_Centiare] " & vbCrLf)
            stb.Append("  ,[operazione_DB] " & vbCrLf)
            stb.Append("  ,[CfgSalvataggio] " & vbCrLf)
            stb.Append("  ,[inviato] " & vbCrLf)
            stb.Append("  ,[datainvio] " & vbCrLf)
            stb.Append("  ,[Data_Creazione] " & vbCrLf)
            stb.Append("  ,[Data_Modifica] " & vbCrLf)
            stb.Append("  ,[Username_Creazione] " & vbCrLf)
            stb.Append("  ,[Username_Modifica] " & vbCrLf)
            stb.Append("  ,[Validita_Inizio] " & vbCrLf)
            stb.Append("  ,[Validita_Fine] " & vbCrLf)
            stb.Append("  ,[Programmazione_Entita_cod] " & vbCrLf)
            stb.Append("  ) values (" & vbCrLf)

            stb.Append("  " & Agro_SQL_SaveNum(CatastoModifiche_COD) & " " & vbCrLf)
            stb.Append(",'" & Agro_SQL_SaveText(PIVA) & "'" & vbCrLf)
            stb.Append(", " & Agro_SQL_SaveNum(SA_COD) & " " & vbCrLf)
            stb.Append(", " & Agro_SQL_SaveNum(APPEZZA) & " " & vbCrLf)
            stb.Append(",'" & Agro_SQL_SaveText(PROV) & "'" & vbCrLf)
            stb.Append(",'" & Agro_SQL_SaveText(COM) & "'" & vbCrLf)
            stb.Append(",'" & Agro_SQL_SaveText(SEZIONE) & "'" & vbCrLf)
            stb.Append(", " & Agro_SQL_SaveNum(FOGLIO) & " " & vbCrLf)
            stb.Append(", " & Agro_SQL_SaveNum(NUMERO) & " " & vbCrLf)
            stb.Append(",'" & Agro_SQL_SaveText(SUBALTERNO) & "'" & vbCrLf)
            stb.Append(",'" & Agro_SQL_SaveText(rag_soc) & "'" & vbCrLf)
            stb.Append(",'" & Agro_SQL_SaveText(sa_nome) & "'" & vbCrLf)
            stb.Append(",'" & Agro_SQL_SaveText(Campo_Des) & "'" & vbCrLf)
            stb.Append(",'" & Agro_SQL_SaveText(p_Ettari_Are_Centiare) & "'" & vbCrLf)
            stb.Append(",'" & Agro_SQL_SaveText(inter_Ettari_Are_Centiare) & "'" & vbCrLf)
            stb.Append(", " & Agro_SQL_SaveNum(percentualeScarto) & " " & vbCrLf)
            stb.Append(",'" & Agro_SQL_SaveText(vecchioInter_Ettari_Are_Centiarevarchar) & "'" & vbCrLf)
            stb.Append(",'" & Agro_SQL_SaveText(App_nome) & "'" & vbCrLf)
            stb.Append(",'" & Agro_SQL_SaveText(impianto_Ettari_Are_Centiare) & "'" & vbCrLf)
            stb.Append(", " & Agro_SQL_SaveNum(operazione_DB) & " " & vbCrLf)
            stb.Append(",'" & Agro_SQL_SaveText(CfgSalvataggio) & "'" & vbCrLf)
            stb.Append(", 0" & vbCrLf)
            stb.Append(", NULL " & vbCrLf)
            stb.Append("," & Agro_SQL_SaveDate(Data_creazione) & " " & vbCrLf)
            stb.Append("," & Agro_SQL_SaveDate(Data_modifica) & " " & vbCrLf)
            stb.Append(",'" & Agro_SQL_SaveText(username_creazione) & "'" & vbCrLf)
            stb.Append(",'" & Agro_SQL_SaveText(username_modifica) & "'" & vbCrLf)
            stb.Append("," & Agro_SQL_SaveDate(Validita_inizio) & " " & vbCrLf)
            stb.Append("," & Agro_SQL_SaveDate(Validita_fine) & " " & vbCrLf)
            stb.Append("," & Agro_SQL_SaveNum(Programmazione_Entita_cod) & " " & vbCrLf)
            stb.Append("  )" & vbCrLf)


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function Modifica(
                            ByVal CatastoModifiche_COD As Integer _
                , ByVal PIVA As String _
                , ByVal SA_COD As Integer _
                , ByVal APPEZZA As Integer _
                , ByVal Programmazione_entita_cod As Integer _
                , ByVal PROV As String _
                , ByVal COM As String _
                , ByVal SEZIONE As String _
                , ByVal FOGLIO As Integer _
                , ByVal NUMERO As Integer _
                , ByVal SUBALTERNO As String _
                , ByVal rag_soc As String _
                , ByVal sa_nome As String _
                , ByVal Campo_Des As String _
                , ByVal p_Ettari_Are_Centiare As String _
                , ByVal inter_Ettari_Are_Centiare As String _
                , ByVal percentualeScarto As Decimal _
                , ByVal vecchioInter_Ettari_Are_Centiarevarchar As String _
                , ByVal App_nome As String _
                , ByVal impianto_Ettari_Are_Centiare As String _
                , ByVal operazione_DB As Integer _
                , ByVal CfgSalvataggio As String _
                , ByVal Validita_inizio As DateTime _
                , ByVal Validita_fine As DateTime _
                , ByVal xFiltroAggiuntivo As String _
                , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.AppezzaxParticelle_W.Modifica()"

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
            StrSQL.Append("UPDATE CatastoModificheParticelleXAppezzamentiLog SET ")

            StrSQL.Append("  rag_soc = '" & Agro_SQL_SaveText(Trim(rag_soc)) & "' ")
            StrSQL.Append(" ,sa_nome = '" & Agro_SQL_SaveText(Trim(sa_nome)) & "' ")
            StrSQL.Append(" ,campo_des = '" & Agro_SQL_SaveText(Trim(Campo_Des)) & "' ")
            StrSQL.Append(" ,App_nome = '" & Agro_SQL_SaveText(Trim(App_nome)) & "' ")
            StrSQL.Append(" ,impianto_Ettari_Are_Centiare = '" & Agro_SQL_SaveText(Trim(impianto_Ettari_Are_Centiare)) & "' ")
            StrSQL.Append(" ,p_Ettari_Are_Centiare = '" & Agro_SQL_SaveText(Trim(p_Ettari_Are_Centiare)) & "' ")
            StrSQL.Append(" ,inter_Ettari_Are_Centiare = '" & Agro_SQL_SaveText(Trim(inter_Ettari_Are_Centiare)) & "' ")
            StrSQL.Append(" ,percentualeScarto = " & Agro_SQL_SaveNum((percentualeScarto)) & " ")
            StrSQL.Append(" ,operazione_DB = " & Agro_SQL_SaveNum((operazione_DB)) & " ")
            StrSQL.Append(" ,CfgSalvataggio = '" & Agro_SQL_SaveText(Trim(CfgSalvataggio)) & "' ")
            StrSQL.Append(" ,username_modifica = '" & Agro_SQL_SaveText((objParametri.UsernameOperazione)) & "' ")
            StrSQL.Append(" ,data_modifica = " & Agro_SQL_SaveDate((Now)) & " ")
            StrSQL.Append(" ,Validita_Inizio      =  " & Agro_SQL_SaveDate(Validita_inizio) & " ")
            StrSQL.Append(" ,Validita_Fine        =  " & Agro_SQL_SaveDate(Validita_fine) & " ")

            StrSQL.Append(" where 1=1 ")

            If APPEZZA <> 0 Then
                StrSQL.Append(" AND      PIVA        = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
                StrSQL.Append(" AND      Sa_Cod      =  " & Agro_SQL_SaveNum(SA_COD) & "  ")
                StrSQL.Append(" AND      Appezza     =  " & Agro_SQL_SaveNum(APPEZZA) & "  ")
            Else
                StrSQL.Append(" AND      Programmazione_Entita_cod = " & Agro_SQL_SaveNum(Programmazione_entita_cod) & " ")
            End If

            StrSQL.Append(" AND      PROV        = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            StrSQL.Append(" AND      COM         = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            StrSQL.Append(" AND      Sezione     = '" & IIf(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "'  ")
            StrSQL.Append(" AND      FOGLIO      = " & IIf(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
            StrSQL.Append(" AND      Numero      = " & IIf(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
            StrSQL.Append(" AND      SUBALTERNO =  '" & IIf(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), 0) & "' ")


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

    Public Function Cancella(
                ByVal PIVA As String _
                , ByVal SA_COD As Integer _
                , ByVal APPEZZA As Integer _
                , ByVal Programmazione_entita_cod As Integer _
                , ByVal PROV As String _
                , ByVal COM As String _
                , ByVal SEZIONE As String _
                , ByVal FOGLIO As Integer _
                , ByVal NUMERO As Integer _
                , ByVal SUBALTERNO As String _
                , ByVal xFiltroAggiuntivo As String _
                , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.AppezzaxParticelle_W.Cancella()"

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
            StrSQL.Append("DELETE FROM CatastoModificheParticelleXAppezzamentiLog ")

            StrSQL.Append(" where 1=1 ")

            If Programmazione_entita_cod <> 0 Then
                StrSQL.Append(" AND      Programmazione_Entita_cod = " & Agro_SQL_SaveNum(Programmazione_entita_cod) & " ")
            Else
                StrSQL.Append(" AND      PIVA        = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
                StrSQL.Append(" AND      Sa_Cod      =  " & Agro_SQL_SaveNum(SA_COD) & "  ")
                If APPEZZA <> 0 Then
                    StrSQL.Append(" AND      Appezza     =  " & Agro_SQL_SaveNum(APPEZZA) & "  ")
                End If
            End If

            StrSQL.Append(" AND      PROV        = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            StrSQL.Append(" AND      COM         = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            StrSQL.Append(" AND      Sezione     = '" & IIf(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "'  ")
            StrSQL.Append(" AND      FOGLIO      = " & IIf(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
            StrSQL.Append(" AND      Numero      = " & IIf(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
            StrSQL.Append(" AND      SUBALTERNO =  '" & IIf(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), 0) & "' ")

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
