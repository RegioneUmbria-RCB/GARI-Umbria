Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################


Public Class Campi_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################
    Public Function LeggiColtureDatoCampo( _
                                ByVal Piva As String, _
                                ByVal Sa_Cod As Int32, _
                                ByVal Campo_Cod As Int32, _
                                ByVal lista_cul_cod As String, _
                                ByVal DataRiferimento As String, _
                                    ByVal xSelezioneVariabile As enumSelezioneVariabile, _
                                    ByVal xFiltroAggiuntivo As String, _
                                    ByVal xOrderBy As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Campi_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    stb.Length = 0

                    If xFiltroAggiuntivo = "" Then
                        stb.Append("select distinct  cc.cul_cod, veg.veg_des, cc.cul_des, c.Campo_Cod, ii.validita_inizio, ii.validita_fine   " & vbCrLf)
                    Else
                        stb.Append(" select c.campo_cod, ii.*, veg.gru_cod " & vbCrLf)
                    End If


                    stb.Append(" from campi c " & vbCrLf)
                    stb.Append("    inner join Appezzamento a " & vbCrLf)
                    stb.Append(" on c.Piva = a.PIVA " & vbCrLf)
                    stb.Append("        and c.Sa_Cod = a.SA_COD  " & vbCrLf)
                    stb.Append("        and c.Campo_Cod = a.Campo_Cod " & vbCrLf)

                    stb.Append("    inner join Reg_Impianti ii " & vbCrLf)
                    stb.Append("        on ii.PIVA = a.PIVA  " & vbCrLf)
                    stb.Append("        and ii.SA_COD = a.SA_COD  " & vbCrLf)
                    stb.Append("        and ii.APPEZZA = a.APPEZZA  " & vbCrLf)
                    stb.Append("    inner join Cultivar cc " & vbCrLf)
                    stb.Append("        on cc.cul_cod = ii.cul_cod " & vbCrLf)
                    stb.Append("    inner join specievegetali veg " & vbCrLf)
                    stb.Append("        on Veg.veg_cod = cc.Veg_Cod  " & vbCrLf)
                    stb.Append(" where c.Campo_Cod = " & Campo_Cod & vbCrLf)

                   

                    stb.Append(" and a.piva = '" & Agro_SQL_SaveText(Piva) & "'" & vbCrLf)
                    stb.Append(" and a.sa_cod =  " & Sa_Cod & vbCrLf)
                    stb.Append(" and ii.validita_inizio <= " & Agro_SQL_SaveDate(DataRiferimento) & " " & vbCrLf)
                    stb.Append(" and ii.validita_fine >= " & Agro_SQL_SaveDate(DataRiferimento) & " " & vbCrLf)

                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            stb.Append(" AND   c.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            stb.Append(" AND   c.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------


                    If xFiltroAggiuntivo <> "" Then
                        stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    If xOrderBy = "" Then
                        stb.Append(" order by veg_des " & vbCrLf)
                    End If

                    
                Case enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case enumSelezioneVariabile.Selezione_JoinCompleta
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



    '##############################################################
    Public Function Leggi_soloAggregatori(ByVal Piva As String, _
                                ByVal Sa_Cod As Int32, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                Optional ByVal Campo_Cod As Int32 = 0 _
                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Campi_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim i As Integer

        Try
            StrSQL.Length = 0
            StrSQL.Append("select Campi.* " & vbCrLf)
            StrSQL.Append(" from Campi" & vbCrLf)
            StrSQL.Append(" left join CampiXParticelle cp on Campi.piva = cp.piva and  " & vbCrLf)
            StrSQL.Append(" Campi.sa_cod = cp.sa_cod and Campi.campo_cod = cp.campo_cod " & vbCrLf)
            StrSQL.Append(" where cp.CAMPO_COD is null")
            StrSQL.Append(" AND Campi.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Campi.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")


            If Piva <> "" Then
                StrSQL.Append(" AND    Campi.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND     Campi.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            Else
                'leggo se ci sono filtri sui centri
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim FiltroCentri As String = ""
                Dim DtCentriVisibili As DataTable
                DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                If Not DtCentriVisibili Is Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    For i = 0 To DtCentriVisibili.Rows.Count - 1
                        FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                    Next
                    If FiltroCentri <> "" Then
                        StrSQL.Append(" AND Campi.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ")
                    End If
                End If
            End If

            If Campo_Cod <> 0 Then
                StrSQL.Append(" AND Campi.Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & " ")
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Campi.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Campi.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Campo_Des ASC")
            End If

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




    '##############################################################
    Public Function Leggi( _
                                ByVal Piva As String, _
                                ByVal Sa_Cod As Int32, _
                                ByVal Campo_Cod As Int32, _
                                    ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                    ByVal xFiltroAggiuntivo As String, _
                                    ByVal xOrderBy As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Campi_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim i As Integer

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT Campi.* ")
                    StrSQL.Append(" FROM  Campi ")
                    StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then
                        StrSQL.Append(" AND    Campi.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND     Campi.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    Else
                        'leggo se ci sono filtri sui centri
                        Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                        Dim FiltroCentri As String = ""
                        Dim DtCentriVisibili As DataTable
                        DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                        If Not DtCentriVisibili Is Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                            For i = 0 To DtCentriVisibili.Rows.Count - 1
                                FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                            Next
                            If FiltroCentri <> "" Then
                                StrSQL.Append(" AND Campi.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ")
                            End If
                        End If
                    End If

                    If Campo_Cod <> 0 Then
                        StrSQL.Append(" AND Campi.Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & " ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Campi.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Campi.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Campo_Des ASC")
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni

                    StrSQL.Append(" SELECT  Imprese.Rag_Soc, Centri_Aziendali.Sa_Nome, Campi.* ")
                    StrSQL.Append(" FROM  Imprese INNER JOIN ")
                    StrSQL.Append(" Centri_Aziendali ON Imprese.Piva = Centri_Aziendali.Piva  ")
                    StrSQL.Append(" INNER JOIN Campi ON Campi.PIVA = Centri_Aziendali.PIVA ")
                    StrSQL.Append(" AND Campi.sa_cod = Centri_Aziendali.sa_cod ")

                    StrSQL.Append(" WHERE Campi.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Campi.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then
                        StrSQL.Append(" AND    Campi.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND     Campi.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    Else
                        'leggo se ci sono filtri sui centri
                        Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                        Dim FiltroCentri As String = ""
                        Dim DtCentriVisibili As DataTable
                        DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                        If Not DtCentriVisibili Is Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                            For i = 0 To DtCentriVisibili.Rows.Count - 1
                                FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                            Next
                            If FiltroCentri <> "" Then
                                StrSQL.Append(" AND Campi.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ")
                            End If
                        End If
                    End If

                    If Campo_Cod <> 0 Then
                        StrSQL.Append(" AND     Campi.Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & "   ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Campi.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Campi.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Rag_Soc, Sa_Nome, Campo_Des ")
                    End If


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



    '##############################################################
    Public Function Leggi_x_anagrafica(
                                ByVal Piva As String,
                                ByVal Sa_Cod As Long,
                                ByVal Campo_Cod As Long,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Campi_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim i As Integer

        Try


            StrSQL.Length = 0
            StrSQL.Append(" SELECT        c.Piva + '_' + cast (c.Sa_Cod as nvarchar(20)) + '_' + cast(c.Campo_Cod as nvarchar(20)) as 'chiave', ")
            StrSQL.Append("    c.Campo_Des as 'Campo', c.Campo_Des, c.Campo_Cod, c.Validita_Inizio, c.Validita_Fine, c.Gru_Cod, c.Veg_Cod, ")
            StrSQL.Append("   (select [User] from utenti where CODICE_FISCALE = c.Username_Modifica ) as utente_modifica , c.Data_Modifica, ")
            StrSQL.Append("   (select [User] from utenti where CODICE_FISCALE = c.Username_Creazione ) as utente_creazione , c.Data_Creazione, ")
            StrSQL.Append("    gv.Gru_Des, s.Veg_Des, ")
            StrSQL.Append("    ca.sa_cod, ca.sa_nome, ")
            StrSQL.Append("    ca.PIVA, ")
            StrSQL.Append("    CASE WHEN c.Validita_Inizio < GETDATE() AND c.validita_fine > GETDATE() THEN 1 ELSE 0 END as Attivo ")

            StrSQL.Append(" FROM            Campi AS c  ")
            StrSQL.Append("     LEFT OUTER JOIN SpecieVegetali AS s ON c.Veg_Cod = s.Veg_Cod  ")
            StrSQL.Append("     LEFT OUTER JOIN GruppoVegetale AS gv ON c.Gru_Cod = gv.Gru_Cod ")
            StrSQL.Append("     LEFT OUTER JOIN Centri_Aziendali AS ca ON c.sa_cod = ca.sa_Cod AND c.piva = ca.piva ")

            StrSQL.Append(" WHERE c.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   c.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Piva <> "" Then
                StrSQL.Append(" AND    c.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND     c.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            Else
                'leggo se ci sono filtri sui centri
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim FiltroCentri As String = ""
                Dim DtCentriVisibili As DataTable = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                If Not DtCentriVisibili Is Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    For i = 0 To DtCentriVisibili.Rows.Count - 1
                        FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                    Next
                    If FiltroCentri <> "" Then
                        StrSQL.Append(" AND c.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ")
                    End If
                End If
            End If

            If Campo_Cod <> 0 Then
                StrSQL.Append(" AND     c.Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   c.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   c.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Campo_Des ASC")
            End If




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

    Public Function Leggi_x_anagrafica_NG(
                                ByVal Piva As String,
                                ByVal Sa_Cod As Long,
                                ByVal Campo_Cod As Long,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Campi_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim i As Integer

        Try


            StrSQL.Length = 0
            StrSQL.AppendLine(" WITH sup_catastali as ( ")
            StrSQL.AppendLine(" 	SELECT  Piva, Sa_Cod, Campo_Cod, SUM(AREA) as Sup_Catastale ")
            StrSQL.AppendLine(" 	FROM    CampiXParticelle (NOLOCK) ")
            StrSQL.AppendLine("     WHERE CampiXParticelle.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine("     AND   CampiXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            If Piva <> "" Then
                StrSQL.AppendLine("     AND   CampiXParticelle.Piva = '" & Agro_SQL_SaveText(Piva) & "'  ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine("     AND   CampiXParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Campo_Cod <> 0 Then
                StrSQL.AppendLine("     AND   CampiXParticelle.Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & "  ")
            End If

            StrSQL.AppendLine(" 	GROUP BY Piva, Sa_Cod, CAMPO_COD ")
            StrSQL.AppendLine(" ), sup_appezzamenti as ( ")
            StrSQL.AppendLine(" 	SELECT  Appezzamento.PIVA, ")
            StrSQL.AppendLine(" 			Appezzamento.SA_COD,  ")
            StrSQL.AppendLine(" 			Appezzamento.Campo_Cod, ")
            StrSQL.AppendLine(" 			SUM (CASE WHEN Appezzamento_Codici.val_cod = '1' OR Appezzamento_Codici.val_cod IS NULL THEN Appezzamento.sup_app ELSE 0 END) as SAU_Convenzionale, ")
            StrSQL.AppendLine(" 			SUM (CASE WHEN Appezzamento_Codici.val_cod = '2' THEN Appezzamento.sup_app ELSE 0 END) as SAU_Conversione, ")
            StrSQL.AppendLine(" 			SUM (CASE WHEN Appezzamento_Codici.val_cod = '3' THEN Appezzamento.sup_app ELSE 0 END) as SAU_Biologico, ")
            StrSQL.AppendLine(" 			SUM (Appezzamento.sup_app) AS SAU_Totale ")
            StrSQL.AppendLine(" 	FROM    Appezzamento (NOLOCK) ")
            StrSQL.AppendLine(" 	LEFT OUTER JOIN Appezzamento_Codici (NOLOCK) ON Appezzamento.PIVA = Appezzamento_Codici.PIVA   ")
            StrSQL.AppendLine(" 										AND Appezzamento.SA_COD = Appezzamento_Codici.sa_cod ")
            StrSQL.AppendLine(" 										AND Appezzamento.APPEZZA = Appezzamento_Codici.appezza ")
            StrSQL.AppendLine(" 										AND Appezzamento_Codici.id_cod = 1018 ")
            StrSQL.AppendLine("     WHERE Appezzamento.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine("     AND   Appezzamento.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            If Piva <> "" Then
                StrSQL.AppendLine("     AND   Appezzamento.Piva = '" & Agro_SQL_SaveText(Piva) & "'  ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine("     AND   Appezzamento.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Campo_Cod <> 0 Then
                StrSQL.AppendLine("     AND   Appezzamento.Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & "  ")
            End If
            StrSQL.AppendLine(" 	GROUP BY Appezzamento.PIVA, Appezzamento.SA_COD, Appezzamento.Campo_Cod ")
            StrSQL.AppendLine(" )  ")

            StrSQL.AppendLine(" SELECT        c.Piva + '_' + cast (c.Sa_Cod as nvarchar(20)) + '_' + cast(c.Campo_Cod as nvarchar(20)) as 'chiave', ")
            StrSQL.AppendLine("    c.Campo_Des as 'Campo', c.Campo_Des, c.Campo_Cod, c.Validita_Inizio, c.Validita_Fine, c.Gru_Cod, c.Veg_Cod, ")
            StrSQL.AppendLine("   (select [User] from utenti where CODICE_FISCALE = c.Username_Modifica ) as utente_modifica , c.Data_Modifica, ")
            StrSQL.AppendLine("   (select [User] from utenti where CODICE_FISCALE = c.Username_Creazione ) as utente_creazione , c.Data_Creazione, ")
            StrSQL.AppendLine("    gv.Gru_Des, s.Veg_Des, ")
            StrSQL.AppendLine("    ca.sa_cod, ca.sa_nome, ")
            StrSQL.AppendLine("    ca.PIVA, ")
            StrSQL.AppendLine("    COALESCE(rif_alfanumerico.val_cod, '') as rif_alfanumerico, ")
            StrSQL.AppendLine("    COALESCE(sup_contratto.val_cod, '') as sup_contratto, ")
            StrSQL.AppendLine("    COALESCE(filiera.val_cod, '') as filiera, ")
            StrSQL.AppendLine("    CAST(COALESCE(sup_catastali.Sup_Catastale, 0) as Decimal(10,4)) AS Superficie_Catastale, ")
            StrSQL.AppendLine("    CAST(COALESCE(sup_appezzamenti.SAU_Convenzionale, 0) as Decimal(10,4)) AS Superficie_Convenzionale, ")
            StrSQL.AppendLine("    CAST(COALESCE(sup_appezzamenti.SAU_Conversione, 0) as Decimal(10,4)) AS Superficie_Conversione, ")
            StrSQL.AppendLine("    CAST(COALESCE(sup_appezzamenti.SAU_Biologico, 0) as Decimal(10,4)) AS Superficie_Biologico, ")
            StrSQL.AppendLine("    CAST(COALESCE(sup_appezzamenti.SAU_Totale, 0) as Decimal(10,4)) AS Superficie_Totale, ")
            StrSQL.AppendLine("    CASE WHEN c.Validita_Inizio < GETDATE() AND c.validita_fine > GETDATE() THEN 1 ELSE 0 END as Attivo ")

            StrSQL.AppendLine(" FROM            Campi (NOLOCK) AS c  ")
            StrSQL.AppendLine("     JOIN SpecieVegetali (NOLOCK) AS s ON c.Veg_Cod = s.Veg_Cod  ")
            StrSQL.AppendLine("     LEFT OUTER JOIN GruppoVegetale (NOLOCK) AS gv ON c.Gru_Cod = gv.Gru_Cod ")
            StrSQL.AppendLine("     JOIN Centri_Aziendali (NOLOCK) AS ca ON c.sa_cod = ca.sa_Cod AND c.piva = ca.piva ")
            StrSQL.AppendLine("     LEFT JOIN Campi_Codici (NOLOCK) rif_alfanumerico ON c.Piva = rif_alfanumerico.Piva AND c.Sa_Cod = rif_alfanumerico.Sa_Cod AND c.Campo_Cod = rif_alfanumerico.Campo_Cod AND rif_alfanumerico.id_cod = 1279 ")
            StrSQL.AppendLine("     LEFT JOIN Campi_Codici (NOLOCK) sup_contratto ON c.Piva = sup_contratto.Piva  AND c.Sa_Cod = sup_contratto.Sa_Cod AND c.Campo_Cod = sup_contratto.Campo_Cod AND sup_contratto.id_cod = 1325 ")
            StrSQL.AppendLine("     LEFT JOIN Campi_Codici (NOLOCK) filiera ON c.Piva = filiera.Piva AND c.Sa_Cod = filiera.Sa_Cod AND c.Campo_Cod = filiera.Campo_Cod AND filiera.id_cod = 1326 ")
            StrSQL.AppendLine(" 	LEFT JOIN sup_catastali ON c.Piva = sup_catastali.Piva AND c.Sa_Cod = sup_catastali.Sa_Cod AND c.Campo_Cod = sup_catastali.Campo_Cod  ")
            StrSQL.AppendLine("     LEFT JOIN sup_appezzamenti ON c.Piva = sup_appezzamenti.PIVA AND c.Sa_Cod = sup_appezzamenti.SA_COD AND c.Campo_Cod = sup_appezzamenti.Campo_Cod ")
            StrSQL.AppendLine(" WHERE c.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   c.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND    c.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND     c.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            Else
                'leggo se ci sono filtri sui centri
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim FiltroCentri As String = ""
                Dim DtCentriVisibili As DataTable = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                If Not DtCentriVisibili Is Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    For i = 0 To DtCentriVisibili.Rows.Count - 1
                        FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                    Next
                    If FiltroCentri <> "" Then
                        StrSQL.AppendLine(" AND c.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ")
                    End If
                End If
            End If

            If Campo_Cod <> 0 Then
                StrSQL.AppendLine(" AND     c.Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   c.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   c.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Campo_Des ASC")
            End If




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


    '##############################################################################################
    Public Function LeggiDatiImpianto_from_CampoCodData(ByVal Piva As String, _
                                                        ByVal Sa_Cod As Integer, _
                                                        ByVal Campo_Cod As Integer, _
                                                        ByVal Data As Date, _
                                                        ByVal xFiltroAggiuntivo As String, _
                                                        ByVal xOrderBy As String, _
                                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Campi_R.LeggiDatiImpianto_from_CampoCod()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim i As Integer

        Try
            StrSQL.Length = 0

            StrSQL.Append(" SELECT campo_des, app_nome, Cultivar.cul_cod, cul_des, SpecieVegetali.veg_cod, veg_Des, SpecieVegetali.gru_cod, Campi.Campo_Cod ")
            StrSQL.Append(" FROM  Campi ")
            StrSQL.Append(" INNER JOIN  Appezzamento ON Campi.Piva = Appezzamento.Piva AND Campi.sa_cod = Appezzamento.sa_cod AND Campi.Campo_Cod = Appezzamento.Campo_Cod   ")
            StrSQL.Append(" INNER JOIN  Reg_Impianti ON Reg_Impianti.Piva = Appezzamento.Piva AND Reg_Impianti.sa_cod = Appezzamento.sa_cod AND Reg_Impianti.Appezza = Appezzamento.Appezza   ")

            StrSQL.Append(" INNER JOIN Cultivar ON Cultivar.cul_cod = Reg_Impianti.cul_cod ")
            StrSQL.Append(" INNER JOIN SpecieVegetali ON Cultivar.veg_cod = SpecieVegetali.veg_cod ")

            StrSQL.Append(" WHERE   Campi.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND     Campi.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.Append(" AND     Appezzamento.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND     Appezzamento.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.Append(" AND     Reg_Impianti.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND     Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            StrSQL.Append(" AND     Campi.Validita_Inizio <= " & Agro_SQL_SaveDate(Data) & " ")
            StrSQL.Append(" AND     Campi.Validita_Fine >= " & Agro_SQL_SaveDate(Data) & " ")
            StrSQL.Append(" AND     Appezzamento.Validita_Inizio <= " & Agro_SQL_SaveDate(Data) & " ")
            StrSQL.Append(" AND     Appezzamento.Validita_Fine >= " & Agro_SQL_SaveDate(Data) & " ")
            StrSQL.Append(" AND     Reg_Impianti.Validita_Inizio <= " & Agro_SQL_SaveDate(Data) & " ")
            StrSQL.Append(" AND     Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(Data) & " ")

            If Piva <> "" Then
                StrSQL.Append(" AND    Campi.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND     Campi.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            Else
                'leggo se ci sono filtri sui centri
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim FiltroCentri As String = ""
                Dim DtCentriVisibili As DataTable
                DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                If Not DtCentriVisibili Is Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    For i = 0 To DtCentriVisibili.Rows.Count - 1
                        FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                    Next
                    If FiltroCentri <> "" Then
                        StrSQL.Append(" AND Campi.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ")
                    End If
                End If
            End If

            If Campo_Cod <> 0 Then
                StrSQL.Append(" AND     Campi.Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Campi.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Campi.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                'Else
                '    StrSQL.Append(" ORDER BY Rag_Soc, Sa_Nome, Campo_Des ")
            End If


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


    '#############################################################################################
    Public Function CampoDes_from_CampoCod(
                                        ByVal Piva As String,
                                        ByVal Sa_Cod As Integer,
                                        ByVal Campo_Cod As Integer,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As String

        Dim DT As DataTable

        DT = Leggi(Piva, Sa_Cod, Campo_Cod,
                   enumSelezioneVariabile.Selezione_JoinDescrizioni, "", "",
                   objParametri)

        If Not IsNothing(DT) Then
            If DT.Rows.Count <> 0 Then
                Return DT.Rows(0).Item("Campo_Des")
            Else
                Return ""
            End If
        Else
            Return ""
        End If

    End Function

    Public Function LeggiCampixParticella(ByVal Piva As String,
                                          ByVal Sa_Cod As Long,
                                          ByVal Campo_Cod As Long,
                                          ByVal PROV As String,
                                          ByVal COM As String,
                                          ByVal SEZIONE As String,
                                          ByVal FOGLIO As Int32,
                                          ByVal NUMERO As Int32,
                                          ByVal SUBALTERNO As String,
                                          ByVal xFiltroAggiuntivo As String,
                                          ByVal xOrderBy As String,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R.LeggiAppezzamenti_Da_Particella()"

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
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT  ")
            StrSQL.AppendLine(" Imprese.rag_soc, ")
            StrSQL.AppendLine(" Centri_Aziendali.sa_nome, ")
            StrSQL.AppendLine(" CampixParticelle.Piva, ")
            StrSQL.AppendLine(" Centri_Aziendali.sa_cod, ")
            StrSQL.AppendLine(" CampixParticelle.PROV, ")
            StrSQL.AppendLine(" ISTAT.COMUNI_PROV, ")
            StrSQL.AppendLine(" CampixParticelle.COM, ")
            StrSQL.AppendLine(" ISTAT.LOCALITA, ")
            StrSQL.AppendLine(" CampixParticelle.SEZIONE, ")
            StrSQL.AppendLine(" CampixParticelle.FOGLIO, ")
            StrSQL.AppendLine(" CampixParticelle.NUMERO, ")
            StrSQL.AppendLine(" CampixParticelle.SUBALTERNO, ")
            StrSQL.AppendLine(" Campi.Campo_Des, ")
            StrSQL.AppendLine(" Campi.Validita_Inizio, ")
            StrSQL.AppendLine(" Campi.Validita_Fine, ")
            StrSQL.AppendLine(" CampiXParticelle.AREA, ")
            StrSQL.AppendLine(" CASE WHEN Campi.Validita_Inizio < GETDATE() AND Campi.validita_fine > GETDATE() THEN 1 ELSE 0 END as Attivo ")
            StrSQL.AppendLine(" FROM CampixParticelle ")
            StrSQL.AppendLine(" JOIN Campi ON CampixParticelle.Piva = Campi.Piva AND CampixParticelle.SA_COD = Campi.Sa_Cod AND CampixParticelle.CAMPO_COD = Campi.Campo_Cod ")
            StrSQL.AppendLine(" JOIN ISTAT ON CampixParticelle.PROV = ISTAT.PROV AND CampixParticelle.Com = ISTAT.Com ")
            StrSQL.AppendLine(" JOIN Imprese ON CampixParticelle.Piva = imprese.PIVA ")
            StrSQL.AppendLine(" JOIN Centri_Aziendali ON CampixParticelle.Piva = Centri_Aziendali.PIVA ")
            StrSQL.AppendLine(" 						AND	CampixParticelle.sa_cod = Centri_Aziendali.sa_cod ")


            If Piva <> "" Then
                StrSQL.AppendLine(" AND Imprese.Piva = '" & Agro_SQL_SaveText(Piva) & "'  ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Centri_Aziendali.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            End If

            If Campo_Cod <> 0 Then
                StrSQL.AppendLine(" AND Campi.Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & "  ")
            End If

            If PROV <> "" Then
                StrSQL.AppendLine(" AND CampixParticelle.Prov = '" & Agro_SQL_SaveText(PROV) & "'  ")
            End If

            If COM <> "" Then
                StrSQL.AppendLine(" AND CampixParticelle.Com = '" & Agro_SQL_SaveText(COM) & "'  ")
            End If

            If SEZIONE <> "" Then
                StrSQL.AppendLine(" AND CampixParticelle.Sezione = '" & Agro_SQL_SaveText(SEZIONE) & "'  ")
            End If

            If FOGLIO <> 0 Then
                StrSQL.AppendLine(" AND CampixParticelle.Foglio = " & Agro_SQL_SaveNum(FOGLIO) & "  ")
            End If

            If NUMERO <> 0 Then
                StrSQL.AppendLine(" AND CampixParticelle.Numero = " & Agro_SQL_SaveNum(NUMERO) & "  ")
            End If

            If SUBALTERNO <> "" Then
                StrSQL.AppendLine(" AND CampixParticelle.Subalterno = '" & Agro_SQL_SaveText(SUBALTERNO) & "'  ")
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   CampixParticelle.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   CampixParticelle.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY CampixParticelle.Campo_Cod ASC")
            End If


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
Public Class Campi_W
    Inherits AgronicaCoreDataProvider.DataProvider


    '#########################################################st
    Public Function Scrivi(ByVal Piva As String,
                           ByVal Sa_Cod As Integer,
                           ByVal Campo_Cod As Integer,
                           ByVal Campo_Des As String,
                           ByVal Gru_Cod As Integer,
                           ByVal Veg_Cod As Integer,
                           ByVal SAU_Totale As Decimal,
                           ByVal SAU_Biologico As Decimal,
                           ByVal SAU_Conversione As Decimal,
                           ByVal SAU_Convenzionale As Decimal,
                           ByVal Conversione_Inizio As Date,
                           ByVal Conversione_Fine As Date,
                           ByVal ConfiniRischio As String,
                           ByVal Campo_Tipo As Integer,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Campi_W.Scrivi()"

        Dim MessaggioErrore As String = ""
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
            StrSQL.Append("INSERT INTO Campi(  Piva, Sa_Cod, ")
            StrSQL.Append("                    Campo_Cod,   Campo_Des, ")
            StrSQL.Append("                    Gru_Cod,  Veg_Cod, ")
            StrSQL.Append("                    SAU_Totale,  SAU_Biologico, ")
            StrSQL.Append("                    SAU_Conversione,   SAU_Convenzionale, ")
            StrSQL.Append("                    Conversione_Inizio,   Conversione_Fine, ")
            StrSQL.Append("                    ConfiniRischio, Campo_Tipo, ")
            StrSQL.Append("                    Inviato, DataInvio, ")
            StrSQL.Append("                    Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                    UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("                    Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("                    ) ")
            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(Piva) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Campo_Cod) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Campo_Des) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Gru_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Veg_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(SAU_Totale) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(SAU_Biologico) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(SAU_Conversione) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(SAU_Convenzionale) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Conversione_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Conversione_Fine) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(ConfiniRischio) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Campo_Tipo) & "  ")
            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append(")")

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
    Public Function Campo_MarcaComeInviato(ByVal Piva As String,
                                           ByVal Sa_Cod As Integer,
                                           ByVal Campo_Cod As Integer,
                                           ByVal Data_invio As DateTime,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                           ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Appezzamento_Write.Campi_Campo_MarcaComeInviato()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Sa_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            End If

            If Campo_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Campo_Cod obbligatorio)")
            End If

            '  Marco Grilli, 23/06/2014 17:50:28: Marco come inviato la tabella Campi
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" UPDATE Campi SET ")
            StrSQL.Append("     inviato         =  -2 ")
            StrSQL.Append("    ,datainvio         =  " & Agro_SQL_SaveDateTime(Data_invio))

            StrSQL.Append(" WHERE   PIVA        = '" & Agro_SQL_SaveText(Trim(Piva)) & "'  ")
            StrSQL.Append(" AND     Sa_Cod   = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append(" AND     Campo_Cod   = " & Agro_SQL_SaveNum(Campo_Cod) & "  ")


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


            '  Marco Grilli, 23/06/2014 17:50:28: Marco come inviato la tabella UtentixCampi
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" UPDATE UtentixCampi SET ")
            StrSQL.Append("     inviato         =  -2 ")
            StrSQL.Append("    ,datainvio         =  " & Agro_SQL_SaveDateTime(Data_invio))

            StrSQL.Append(" WHERE   PIVA        = '" & Agro_SQL_SaveText(Trim(Piva)) & "'  ")
            StrSQL.Append(" AND     Sa_Cod   = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append(" AND     Campo_Cod   = " & Agro_SQL_SaveNum(Campo_Cod) & "  ")
            StrSQL.Append(" AND     [User]   = '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "'  ")


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


    '############################################################################################
    Public Function Aggrega(ByVal Piva As String,
                            ByVal Sa_Cod As Integer,
                            ByVal Campo_Cod As Integer,
                            ByVal Appezza As Integer,
                            ByVal Validita_Inizio_Storico As String,
                            ByVal Validita_Fine_Storico As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Campi_W.Aggrega()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            '---------------------------------------------------------------------------------------
            '----- 1.  Modifica del record nella tabella appezzamento (inserimento campo_cod)
            '---------------------------------------------------------------------------------------


            'Genero la query SQL
            StrSQL.Append(" UPDATE Appezzamento ")
            StrSQL.Append(" SET ")
            StrSQL.Append(" Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & ", ")
            StrSQL.Append(" Data_Modifica = " & Agro_SQL_SaveDateTime(Now) & ", ")
            StrSQL.Append(" Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")

            StrSQL.Append(" WHERE Piva= '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            StrSQL.Append(" AND Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


            '---------------------------------------------------------------------------------------
            '----- 2.  Inserimento di un record in Campo_Storico 
            '---------------------------------------------------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" INSERT INTO Campi_Storico ")
            StrSQL.Append(" (Piva, Sa_Cod, Campo_Cod, Appezza, Data_Creazione, Data_Modifica,  ")
            StrSQL.Append(" Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine) ")
            StrSQL.Append(" VALUES ( ")
            StrSQL.Append("     '" & Agro_SQL_SaveText(Piva) & "', ")
            StrSQL.Append("     " & Agro_SQL_SaveNum(Sa_Cod) & ", ")
            StrSQL.Append("     " & Agro_SQL_SaveNum(Campo_Cod) & ", ")
            StrSQL.Append("     " & Agro_SQL_SaveNum(Appezza) & ", ")
            StrSQL.Append("     " & Agro_SQL_SaveDateTime(Now) & ", ")
            StrSQL.Append("     " & Agro_SQL_SaveDateTime(Now) & ", ")
            StrSQL.Append("     '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "',  ")
            StrSQL.Append("     '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "',  ")
            StrSQL.Append("     " & Agro_SQL_SaveDate(Validita_Inizio_Storico) & ",  ")
            StrSQL.Append("     " & Agro_SQL_SaveDate(Validita_Fine_Storico) & ") ")

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



    '############################################################################################
    Public Function Disaggrega(ByVal Piva As String,
                               ByVal Sa_Cod As Integer,
                               ByVal Campo_Cod As Integer,
                               ByVal Appezza As Integer,
                               ByVal Validita_Inizio_Storico As String,
                               ByVal Validita_Fine_Storico As String,
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                               ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Campi_W.Disaggrega()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            '---------------------------------------------------------------------------------------
            '----- 1.  Modifica del record nella tabella appezzamento (eliminazione campo_cod)
            '---------------------------------------------------------------------------------------
            StrSQL.Append(" UPDATE Appezzamento ")
            StrSQL.Append(" SET ")
            StrSQL.Append(" Campo_Cod = 0, ")
            StrSQL.Append(" Data_Modifica = " & Agro_SQL_SaveDateTime(Now) & ", ")
            StrSQL.Append(" Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")

            StrSQL.Append(" WHERE Piva= '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            StrSQL.Append(" AND Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


            '---------------------------------------------------------------------------------------
            '----- 2.  Modifica del record in Campo_Storico (se non esiste lo creo...)
            '---------------------------------------------------------------------------------------

            StrSQL.Length = 0




            Dim objCampStor As New AgronicaCoreAnagrafeDAL.Campi_Storico_R
            If objCampStor.Esiste_Record_Campi_Storico(Piva, Sa_Cod, Campo_Cod, Appezza, objParametri) = True Then

                Validita_Inizio_Storico = CStr(DateAdd(DateInterval.Day, -1, CDate(Validita_Fine_Storico)))
                StrSQL.Append(" UPDATE Campi_Storico ")
                StrSQL.Append(" SET ")
                StrSQL.Append(" Data_Modifica = " & Agro_SQL_SaveDateTime(Now) & ", ")
                StrSQL.Append(" Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "', ")
                StrSQL.Append(" Validita_Fine = " & Agro_SQL_SaveDate(Validita_Fine_Storico) & " ")

                StrSQL.Append(" WHERE Piva= '" & Agro_SQL_SaveText(Piva) & "' ")
                StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                StrSQL.Append(" AND Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & " ")
                StrSQL.Append(" AND Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")

            Else
                StrSQL.Append(" INSERT INTO Campi_Storico ")
                StrSQL.Append(" (Piva, Sa_Cod, Campo_Cod, Appezza, Data_Creazione, Data_Modifica,  ")
                StrSQL.Append(" Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine) ")
                StrSQL.Append(" VALUES ( ")
                StrSQL.Append("     '" & Agro_SQL_SaveText(Piva) & "', ")
                StrSQL.Append("     " & Agro_SQL_SaveNum(Sa_Cod) & ", ")
                StrSQL.Append("     " & Agro_SQL_SaveNum(Campo_Cod) & ", ")
                StrSQL.Append("     " & Agro_SQL_SaveNum(Appezza) & ", ")
                StrSQL.Append("     " & Agro_SQL_SaveDateTime(Now) & ", ")
                StrSQL.Append("     " & Agro_SQL_SaveDateTime(Now) & ", ")
                StrSQL.Append("     '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "',  ")
                StrSQL.Append("     '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "',  ")
                StrSQL.Append("     " & Agro_SQL_SaveDate(Validita_Inizio_Storico) & ",  ")
                StrSQL.Append("     " & Agro_SQL_SaveDate(Validita_Fine_Storico) & ") ")


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


    '##################################################################
    Public Function Modifica(ByVal Piva As String,
                             ByVal Sa_Cod As Integer,
                             ByVal Campo_Cod As Integer,
                             ByVal Campo_Des As String,
                             ByVal Gru_Cod As Integer,
                             ByVal Veg_Cod As Integer,
                             ByVal SAU_Totale As Decimal,
                             ByVal SAU_Biologico As Decimal,
                             ByVal SAU_Conversione As Decimal,
                             ByVal SAU_Convenzionale As Decimal,
                             ByVal Conversione_Inizio As Date,
                             ByVal Conversione_Fine As Date,
                             ByVal ConfiniRischio As String,
                             ByVal Campo_Tipo As Integer,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                             Optional Byval Data_modifica As DateTime = #2/1/1900#,
                             Optional Byval username_modifica As String = ""
                             ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Campi_W.Modifica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        If Data_modifica = #2/1/1900# Then
            Data_modifica = Now
        End If

        If username_modifica = "" Then
            username_modifica = objParametri.UsernameOperazione
        End If

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE Campi SET ")
            StrSQL.Append("    Campo_Des          = '" & Agro_SQL_SaveText(Campo_Des) & "' ")
            StrSQL.Append("   ,Gru_Cod            =  " & Agro_SQL_SaveNum(Gru_Cod) & "  ")
            StrSQL.Append("   ,Veg_Cod            =  " & Agro_SQL_SaveNum(Veg_Cod) & "  ")
            StrSQL.Append("   ,Sau_Totale         =  " & Agro_SQL_SaveNum(SAU_Totale) & "  ")
            StrSQL.Append("   ,Sau_Biologico      =  " & Agro_SQL_SaveNum(SAU_Biologico) & "  ")
            StrSQL.Append("   ,Sau_Conversione    =  " & Agro_SQL_SaveNum(SAU_Conversione) & "  ")
            StrSQL.Append("   ,Sau_Convenzionale  =  " & Agro_SQL_SaveNum(SAU_Convenzionale) & "  ")
            StrSQL.Append("   ,Conversione_Inizio =  " & Agro_SQL_SaveDate(Conversione_Inizio))
            StrSQL.Append("   ,Conversione_Fine   =  " & Agro_SQL_SaveDate(Conversione_Fine))
            StrSQL.Append("   ,ConfiniRischio     = '" & Agro_SQL_SaveText(ConfiniRischio) & "'")
            StrSQL.Append("   ,Campo_Tipo         =  " & Agro_SQL_SaveNum(Campo_Tipo) & "  ")
            StrSQL.Append("   ,Inviato            =  0 ")
            StrSQL.Append("   ,DataInvio          =  Null ")
            StrSQL.Append("   ,Data_Modifica      =  " & Agro_SQL_SaveDateTime(Data_modifica))
            StrSQL.Append("   ,UserName_Modifica  = '" & Agro_SQL_SaveText(username_modifica) & "'")
            StrSQL.Append("   ,Validita_Inizio    =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("   ,Validita_Fine      =  " & Agro_SQL_SaveDate(Validita_Fine))

            StrSQL.Append(" WHERE Piva      =  '" & Agro_SQL_SaveText(Piva) & "'  ")
            StrSQL.Append(" AND Sa_Cod      =  " & Agro_SQL_SaveNum(Sa_Cod))
            StrSQL.Append(" AND Campo_Cod      =  " & Agro_SQL_SaveNum(Campo_Cod) & " ")
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

   
    '##########################################################################
    Public Function Cancella(ByVal Piva As String,
                             ByVal Sa_Cod As Integer,
                             ByVal Campo_Cod As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Campi_W.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try


            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Campi ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                StrSQL.Append(" AND Inviato >= 0")

                If Sa_Cod <> 0 Then
                    StrSQL.Append(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                End If

                If Campo_Cod <> 0 Then
                    StrSQL.Append(" AND Campo_Cod  =  " & Agro_SQL_SaveNum(Campo_Cod) & " ")
                End If

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     Campi ")
                StrSQL.Append(" WHERE    Piva= '" & Agro_SQL_SaveText(Piva) & "' ")

                If Sa_Cod <> 0 Then
                    StrSQL.Append(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                End If

                If Campo_Cod <> 0 Then
                    StrSQL.Append(" AND Campo_Cod  =  " & Agro_SQL_SaveNum(Campo_Cod) & " ")
                End If

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

    '#########################################################################
    Public Function AggiornaValiditaInizio(ByVal Piva As String,
                                           ByVal Sa_Cod As Integer,
                                           ByVal Campo_Cod As Integer,
                                           ByVal Validita_Inizio As Date,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                           ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.Campi_W.AggiornaValiditaInizio()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE Campi SET ")
            StrSQL.Append("   UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Data_Modifica   =  " & Agro_SQL_SaveDateTime(Now))
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))

            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND   Validita_Inizio < " & Agro_SQL_SaveDate(Validita_Inizio))

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Campo_Cod <> 0 Then
                StrSQL.Append(" AND Campo_Cod  =  " & Agro_SQL_SaveNum(Campo_Cod) & " ")
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

    '#########################################################################
    Public Function AggiornaValiditaInizioForzata(ByVal Piva As String,
                                                  ByVal Sa_Cod As Integer,
                                                  ByVal Campo_Cod As Integer,
                                                  ByVal Validita_Inizio As Date,
                                                  ByVal xFiltroAggiuntivo As String,
                                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                  ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.Campi_W.AggiornaValiditaInizioForzata()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE Campi SET ")
            StrSQL.Append("   UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Data_Modifica   =  " & Agro_SQL_SaveDateTime(Now))
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Campo_Cod <> 0 Then
                StrSQL.Append(" AND Campo_Cod  =  " & Agro_SQL_SaveNum(Campo_Cod) & " ")
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


    '#########################################################
    Public Function AggiornaValiditaFine(ByVal Piva As String,
                                         ByVal Sa_Cod As Integer,
                                         ByVal Campo_Cod As Integer,
                                         ByVal Validita_Fine As Date,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                         ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.Campi_W.AggiornaValiditaFine()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE Campi SET ")
            StrSQL.Append("   UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Data_Modifica   =  " & Agro_SQL_SaveDateTime(Now))
            StrSQL.Append("   ,Validita_Fine   =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND   Validita_Fine > " & Agro_SQL_SaveDate(Validita_Fine))

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Campo_Cod <> 0 Then
                StrSQL.Append(" AND Campo_Cod  =  " & Agro_SQL_SaveNum(Campo_Cod) & " ")
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

    '#########################################################
    Public Function AggiornaValiditaFineForzata(ByVal Piva As String,
                                                ByVal Sa_Cod As Integer,
                                                ByVal Campo_Cod As Integer,
                                                ByVal Validita_Fine As Date,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.Campi_W.AggiornaValiditaFineForzata()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE Campi SET ")
            StrSQL.Append("   UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Data_Modifica   =  " & Agro_SQL_SaveDateTime(Now))
            StrSQL.Append("   ,Validita_Fine   =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Campo_Cod <> 0 Then
                StrSQL.Append(" AND Campo_Cod  =  " & Agro_SQL_SaveNum(Campo_Cod) & " ")
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
