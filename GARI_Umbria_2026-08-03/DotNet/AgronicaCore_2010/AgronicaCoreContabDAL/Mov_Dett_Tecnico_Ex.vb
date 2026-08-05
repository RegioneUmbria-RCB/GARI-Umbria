Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate


Public Class Mov_Dett_Tecnico_Ex_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Piva As String,
                          ByVal Sa_Cod As Integer,
                          ByVal Id_Agenda As Integer,
                          ByVal Id_Mov As Integer,
                          ByVal Id_Mov_Det As Integer,
                          ByVal Id_Reg_Dettaglio As Integer,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Dett_Tecnico_Ex_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""  
        '   Sa_Cod = 0 
        '   Id_Agenda = 0    
        '   Id_Mov = 0
        '   Id_Reg_Dettaglio = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    strSql.Length = 0

                    strSql.AppendLine(" SELECT * ")
                    strSql.AppendLine(" FROM  Mov_Dettaglio_Tecnico_Extra WITH(NOLOCK)")
                    strSql.AppendLine(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    strSql.AppendLine(" AND   Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")

                    If Piva <> "" Then
                        strSql.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If

                    If Sa_Cod <> 0 Then
                        strSql.AppendLine(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Id_Agenda <> 0 Then
                        strSql.AppendLine(" AND Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
                    End If

                    If Id_Mov <> 0 Then
                        strSql.AppendLine(" AND Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
                    End If

                    If Id_Reg_Dettaglio <> 0 Then
                        strSql.AppendLine(" AND Id_Reg_Dettaglio = " & Agro_SQL_SaveNum(Id_Reg_Dettaglio) & "   ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Piva Asc ")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case enumSelezioneVariabile.Selezione_JoinCompleta


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


    'Rispetto a Leggi Bypassa il vincolo su id_mov_det
    Public Function Leggi2(ByVal Piva As String,
                           ByVal Sa_Cod As Integer,
                           ByVal Id_Agenda As Integer,
                           ByVal Id_Mov As Integer,
                           ByVal Id_Mov_Det As Integer,
                           ByVal Id_Reg_Dettaglio As Integer,
                           ByVal xSelezioneVariabile As enumSelezioneVariabile,
                           ByVal xFiltroAggiuntivo As String,
                           ByVal xOrderBy As String,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Dett_Tecnico_Ex_R.Leggi2()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""  
        '   Sa_Cod = 0 
        '   Id_Agenda = 0    
        '   Id_Mov = 0
        '   Id_Reg_Dettaglio = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    strSql.Length = 0

                    strSql.AppendLine(" SELECT * ")
                    strSql.AppendLine(" FROM  Mov_Dettaglio_Tecnico_Extra ")
                    strSql.AppendLine(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then
                        strSql.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If

                    If Sa_Cod <> 0 Then
                        strSql.AppendLine(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Id_Agenda <> 0 Then
                        strSql.AppendLine(" AND Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
                    End If

                    If Id_Mov <> 0 Then
                        strSql.AppendLine(" AND Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
                    End If

                    If Id_Mov_Det <> 0 Then
                        strSql.AppendLine(" AND Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")
                    End If

                    If Id_Reg_Dettaglio <> 0 Then
                        strSql.AppendLine(" AND Id_Reg_Dettaglio = " & Agro_SQL_SaveNum(Id_Reg_Dettaglio) & "   ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Piva Asc ")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case enumSelezioneVariabile.Selezione_JoinCompleta


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

    Public Function ConfezionamentiAgenda(ByVal Piva As String,
                                          ByVal Id_Agenda As Integer,
                                          ByVal Id_Mov As Integer,
                                          ByVal escludiNonSpecificati As Boolean,
                                          ByVal xFiltroAggiuntivo As String,
                                          ByVal xOrderBy As String,
                                          ByRef objParametri As AgronicaCoreParametri
                                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Dett_Tecnico_Ex_R.ConfezionamentiAgenda()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.AppendLine(" ( ") 
            strSql.AppendLine("    SELECT e.Piva, e.Id_Agenda, e.Id_Mov, e.Sa_Cod, ") 
            strSql.AppendLine("           SUM(e.Num_Colli) AS Quantita, ") 
            strSql.AppendLine("           ISNULL(mp.Elem_Cod, 0) AS Elem_Cod, e.Contenitore_Cod AS Prodotto_Cod, ") 
            strSql.AppendLine("           e.Des_Contenitori AS Descrizione, ISNULL(mpc.Tara_Campionatura, 0) AS Tara_Unitaria, ") 
            strSql.AppendLine("           8 AS Tipo, 'Contenitore' AS Tipo_Des ") 

            strSql.AppendLine("    FROM Mov_Dettaglio_Tecnico_Extra e ") 
            strSql.AppendLine("    INNER JOIN Movimenti_dettagli md ON e.Piva = md.Piva AND e.Id_Agenda = md.Id_Agenda AND e.Id_Mov = md.Id_Mov AND e.Id_Mov_Det = md.Id_Mov_Det ") 
            strSql.AppendLine("    LEFT JOIN Materie_Prime mp ON e.Contenitore_Cod = mp.Mat_Cod ") 
            strSql.AppendLine("    LEFT JOIN Materie_Prime_Campionature mpc ON md.Cal_Cod = mpc.Progressivo AND mpc.Tipo = 'ocontenitore' ")
            strSql.AppendLine("    WHERE e.Id_Mov_Det <> 0 ") 
            strSql.AppendLine("    AND md.Ordine_Det NOT IN (1000, 30000) ") 
            strSql.AppendLine("    AND md.Jolly_Int = " & MagazzinoMovimentato) 
            strSql.AppendLine("    AND e.Piva = '" & Agro_SQL_SaveText(Piva) & "' ") 
            strSql.AppendLine("    AND e.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ") 
            strSql.AppendLine("    AND e.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & " ") 

            If escludiNonSpecificati Then
                strSql.AppendLine("    AND e.Contenitore_Cod <> 0 ") 
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine("    AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            strSql.AppendLine("    GROUP BY e.Piva, e.Id_Agenda, e.Id_Mov, e.Sa_Cod, e.Contenitore_Cod, mp.Elem_Cod, e.Des_Contenitori, mpc.Tara_Campionatura ")


            strSql.AppendLine(" ) ") 

            strSql.AppendLine(" UNION ") 

            strSql.AppendLine(" ( ") 
            strSql.AppendLine("    SELECT e.Piva, e.Id_Agenda, e.Id_Mov, e.Sa_Cod, ") 
            strSql.AppendLine("           SUM(e.Num_Contenitori) AS Quantita, ") 
            strSql.AppendLine("           ISNULL(mp.Elem_Cod, 0) AS Elem_Cod, e.Imballaggio_Cod AS Prodotto_Cod, ") 
            strSql.AppendLine("           e.Marche_Contenitori AS Descrizione, ISNULL(mpc.Tara_Campionatura, 0) AS Tara_Unitaria, ") 
            strSql.AppendLine("           4 AS Tipo, 'Imballaggio' AS Tipo_Des ") 

            strSql.AppendLine("    FROM Mov_Dettaglio_Tecnico_Extra e ") 
            strSql.AppendLine("    INNER JOIN Movimenti_dettagli md ON e.Piva = md.Piva AND e.Id_Agenda = md.Id_Agenda AND e.Id_Mov = md.Id_Mov AND e.Id_Mov_Det = md.Id_Mov_Det ") 
            strSql.AppendLine("    LEFT JOIN Materie_Prime mp ON e.Imballaggio_Cod = mp.Mat_Cod ") 
            strSql.AppendLine("    LEFT JOIN Materie_Prime_Campionature mpc ON md.Cal_Cod = mpc.Progressivo AND mpc.Tipo = 'oimballaggio' ")
            strSql.AppendLine("    WHERE e.Id_Mov_Det <> 0 ") 
            strSql.AppendLine("    AND md.Ordine_Det NOT IN (1000, 30000)") 
            strSql.AppendLine("    AND md.Jolly_Int = " & MagazzinoMovimentato) 
            strSql.AppendLine("    AND e.Piva = '" & Agro_SQL_SaveText(Piva) & "' ") 
            strSql.AppendLine("    AND e.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ") 
            strSql.AppendLine("    AND e.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & " ") 

            If escludiNonSpecificati Then
                strSql.AppendLine("    AND e.Imballaggio_Cod <> 0 ") 
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine("    AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            strSql.AppendLine("    GROUP BY e.Piva, e.Id_Agenda, e.Id_Mov, e.Sa_Cod, e.Imballaggio_Cod, mp.Elem_Cod, e.Marche_Contenitori, mpc.Tara_Campionatura ")


            strSql.AppendLine(" ) ") 

            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

End Class


'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################


Public Class Mov_Dett_Tecnico_Ex_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function ScriviFull(ByVal Piva As String,
                               ByVal Sa_Cod As Integer,
                               ByVal Id_Agenda As Integer,
                               ByVal Id_Mov As Integer,
                               ByVal Id_Mov_Det As Integer,
                               ByVal Id_Reg_Dettaglio As Integer,
                               ByVal Regione As String,
                               ByVal ASL As String,
                               ByVal Serie As String,
                               ByVal Numero As String,
                               ByVal Mac_Cod As Integer,
                               ByVal Cod_RisUm As Integer,
                               ByVal Trasportatore As String,
                               ByVal Mezzo_Trasporto As String,
                               ByVal Targa As String,
                               ByVal N_Immatricolazione As String,
                               ByVal N_Immatricolazione_Rimorchio As String,
                               ByVal N_Autorizzazione_Trasporto As String,
                               ByVal Data_Rilascio_Autorizzazione As DateTime,
                               ByVal Peso As Decimal,
                               ByVal Codice_Prodotto As Integer,
                               ByVal Colore As Integer,
                               ByVal Zona_Viticola As String,
                               ByVal Manipolazioni As Integer,
                               ByVal Precisazioni As String,
                               ByVal Annotazioni As String,
                               ByVal Num_Contenitori As Integer,
                               ByVal Marche_Contenitori As String,
                               ByVal Des_Contenitori As String,
                               ByVal Tipo_Documento As String,
                               ByVal Id_Cod_Autorita As Integer,
                               ByVal Luogo_Partenza As String,
                               ByVal Luogo_Consegna As String,
                               ByVal Data_Spedizione As DateTime,
                               ByVal Indicazioni_Complementari As String,
                               ByVal Titolo_Alcol As Decimal,
                               ByVal Codice_NC As String,
                               ByVal Num_Riferimento As String,
                               ByVal Data_Dichiarazione As DateTime,
                               ByVal Garanzia As String,
                               ByVal Certificati As String,
                               ByVal Durata_Viaggio As String,
                               ByVal Peso_Lordo As Decimal,
                               ByVal Num_Colli As Integer,
                               ByVal Contenitore_Cod As Integer,
                               ByVal Imballaggio_Cod As Integer,
                               ByVal Agente_Cod As Integer,
                               ByVal Provvigione As Decimal,
                               ByVal Tipo_Trasporto As Integer,
                               ByVal Unita_Trasporto As Integer,
                               ByVal Codice_Alternativo As String,
                               ByVal Id_Gestione_Vettore As Integer,
                               ByVal Ritenuta_Acconto_Cod As Integer,
                               ByVal Ritenuta_Acconto As Decimal,
                               ByVal Enasarco_Cod As Integer,
                               ByVal Enasarco As Decimal,
                               ByVal ACCDAA_Cod_Risum_Destinatario As Integer,
                               ByVal ACCDAA_Cod_Risum_Destinazione As Integer,
                               ByVal ACCDAA_Cod_IndirizzoRisum_Destinatario As Integer,
                               ByVal ACCDAA_Cod_IndirizzoRisum_Destinazione As Integer,
                               ByVal CapoArea_Cod As Integer,
                               ByVal Provvigione_CapoArea As Decimal,
                               ByVal Provvigione_Pagata_Agente As Decimal,
                               ByVal Provvigione_Pagata_CapoArea As Decimal,
                               ByVal N_Doc_Cliente As String,
                               ByVal Data_Doc_Cliente As Date,
                               ByVal N_Doc_Ente As String,
                               ByVal Anno_Doc_Ente As Integer,
                               ByVal Num_Conf_Riscontrate As Integer,
                               ByVal Num_Colli_Riscontrati As Integer,
                               ByVal Num_Imballi_Riscontrati As Integer,
                               ByVal Peso_Netto_Riscontrato As Decimal,
                               ByVal Peso_Lordo_Riscontrato As Decimal,
                               ByVal Validita_Inizio As Date,
                               ByVal Validita_Fine As Date,
                               ByRef objParametri As AgronicaCoreParametri,
                               Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                               Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                               Optional ByVal username_creazione As String = "",
                               Optional ByVal username_modifica As String = "",
                               Optional ByVal Tara_Unit_Conf_Riscontrata As Decimal = -1,
                               Optional ByVal Tara_Unit_Collo_Riscontrata As Decimal = -1,
                               Optional ByVal Tara_Unit_Imballo_Riscontrata As Decimal = -1,
                               Optional ByVal N_Nota_Fattura As String = "",
                               Optional ByVal Data_Nota_Fattura As Date = AGRODATAINIZIO,
                               Optional ByVal N_Nota_DDT As String = "",
                               Optional ByVal N_Nota_Riga_DDT As String = "",
                               Optional ByVal Data_Nota_DDT As Date = AGRODATAINIZIO,
                               Optional ByVal Causale_Fattura As Integer = 0,
                               Optional ByVal DistanzaTrasportoUdm As Integer = 0,
                               Optional ByVal DistanzaTrasporto As Decimal = 0
                               ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Dett_Tecnico_Ex_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

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

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" INSERT INTO Mov_Dettaglio_Tecnico_Extra ")
            strSql.AppendLine("         ( ")
            strSql.AppendLine("   [Piva] ")
            strSql.AppendLine("  ,[Sa_Cod] ")
            strSql.AppendLine("  ,[Id_Agenda] ")
            strSql.AppendLine("  ,[Id_Mov] ")
            strSql.AppendLine("  ,[Id_Mov_Det] ")
            strSql.AppendLine("  ,[Id_Reg_Dettaglio] ")
            strSql.AppendLine("  ,[Regione] ")
            strSql.AppendLine("  ,[ASL] ")
            strSql.AppendLine("  ,[Serie] ")
            strSql.AppendLine("  ,[Numero] ")
            strSql.AppendLine("  ,[Mac_Cod] ")
            strSql.AppendLine("  ,[Cod_RisUm] ")
            strSql.AppendLine("  ,[Trasportatore] ")
            strSql.AppendLine("  ,[Mezzo_Trasporto] ")
            strSql.AppendLine("  ,[Targa] ")
            strSql.AppendLine("  ,[N_Immatricolazione] ")
            strSql.AppendLine("  ,[N_Immatricolazione_Rimorchio] ")
            strSql.AppendLine("  ,[N_Autorizzazione_Trasporto] ")
            strSql.AppendLine("  ,[Data_Rilascio_Autorizzazione] ")
            strSql.AppendLine("  ,[Peso] ")
            strSql.AppendLine("  ,[Codice_Prodotto] ")
            strSql.AppendLine("  ,[Colore] ")
            strSql.AppendLine("  ,[Zona_Viticola] ")
            strSql.AppendLine("  ,[Manipolazioni] ")
            strSql.AppendLine("  ,[Precisazioni] ")
            strSql.AppendLine("  ,[Annotazioni] ")
            strSql.AppendLine("  ,[Num_Contenitori] ")
            strSql.AppendLine("  ,[Marche_Contenitori] ")
            strSql.AppendLine("  ,[Des_Contenitori] ")
            strSql.AppendLine("  ,[Tipo_Documento] ")
            strSql.AppendLine("  ,[Id_Cod_Autorita] ")
            strSql.AppendLine("  ,[Luogo_Partenza] ")
            strSql.AppendLine("  ,[Luogo_Consegna] ")
            strSql.AppendLine("  ,[Data_Spedizione] ")
            strSql.AppendLine("  ,[Indicazioni_Complementari] ")
            strSql.AppendLine("  ,[Titolo_Alcol] ")
            strSql.AppendLine("  ,[Codice_NC] ")
            strSql.AppendLine("  ,[Num_Riferimento] ")
            strSql.AppendLine("  ,[Data_Dichiarazione] ")
            strSql.AppendLine("  ,[Garanzia] ")
            strSql.AppendLine("  ,[Certificati] ")
            strSql.AppendLine("  ,[Durata_Viaggio] ")
            strSql.AppendLine("  ,[Peso_Lordo] ")
            strSql.AppendLine("  ,[Num_Colli] ")
            strSql.AppendLine("  ,[Contenitore_Cod] ")
            strSql.AppendLine("  ,[Imballaggio_Cod] ")
            strSql.AppendLine("  ,[Agente_Cod] ")
            strSql.AppendLine("  ,[Provvigione] ")
            strSql.AppendLine("  ,[Tipo_Trasporto] ")
            strSql.AppendLine("  ,[Unita_Trasporto] ")
            strSql.AppendLine("  ,[Codice_Alternativo] ")
            strSql.AppendLine("  ,[Id_Gestione_Vettore] ")
            strSql.AppendLine("  ,[Ritenuta_Acconto_Cod] ")
            strSql.AppendLine("  ,[Ritenuta_Acconto] ")
            strSql.AppendLine("  ,[Enasarco_Cod] ")
            strSql.AppendLine("  ,[Enasarco] ")
            strSql.AppendLine("  ,[ACCDAA_Cod_Risum_Destinatario] ")
            strSql.AppendLine("  ,[ACCDAA_Cod_Risum_Destinazione] ")
            strSql.AppendLine("  ,[ACCDAA_Cod_IndirizzoRisum_Destinatario] ")
            strSql.AppendLine("  ,[ACCDAA_Cod_IndirizzoRisum_Destinazione] ")

            strSql.AppendLine("  ,[CapoArea_Cod] ")
            strSql.AppendLine("  ,[Provvigione_CapoArea] ")
            strSql.AppendLine("  ,[Provvigione_Pagata_Agente] ")
            strSql.AppendLine("  ,[Provvigione_Pagata_CapoArea] ")
            strSql.AppendLine("  ,[N_Doc_Cliente] ")
            strSql.AppendLine("  ,[Data_Doc_Cliente] ")
            strSql.AppendLine("  ,[N_Doc_Ente] ")
            strSql.AppendLine("  ,[Anno_Doc_Ente] ")
            strSql.AppendLine("  ,[Num_Conf_Riscontrate] ")
            strSql.AppendLine("  ,[Num_Colli_Riscontrati] ")
            strSql.AppendLine("  ,[Num_Imballi_Riscontrati] ")
            strSql.AppendLine("  ,[Peso_Netto_Riscontrato] ")
            strSql.AppendLine("  ,[Peso_Lordo_Riscontrato] ")

            strSql.AppendLine("  ,[Tara_Unit_Conf_Riscontrata] ")
            strSql.AppendLine("  ,[Tara_Unit_Collo_Riscontrata] ")
            strSql.AppendLine("  ,[Tara_Unit_Imballo_Riscontrata] ")
            strSql.AppendLine("  ,[N_Nota_Fattura] ")
            strSql.AppendLine("  ,[Data_Nota_Fattura] ")
            strSql.AppendLine("  ,[N_Nota_DDT] ")
            strSql.AppendLine("  ,[N_Nota_Riga_DDT] ")
            strSql.AppendLine("  ,[Data_Nota_DDT] ")
            strSql.AppendLine("  ,[Causale_Fattura] ")
            strSql.AppendLine("  ,[Distanza_Trasporto_Udm] ")
            strSql.AppendLine("  ,[Distanza_Trasporto] ")

            strSql.AppendLine("          , Inviato,            DataInvio, ")
            strSql.AppendLine("          Data_Creazione,     Data_Modifica, ")
            strSql.AppendLine("          UserName_Creazione, UserName_Modifica, ")
            strSql.AppendLine("          Validita_Inizio,    Validita_Fine ")
            strSql.AppendLine("         ) ")


            strSql.AppendLine(" VALUES ( ")

            strSql.AppendLine(" '" & Agro_SQL_SaveText(Piva) & "'")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Id_Agenda) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Id_Mov) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Id_Mov_Det) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Id_Reg_Dettaglio) & " ")
            strSql.AppendLine(",'" & Agro_SQL_SaveText(Regione) & "'")
            strSql.AppendLine(",'" & Agro_SQL_SaveText(ASL) & "'")
            strSql.AppendLine(",'" & Agro_SQL_SaveText(Serie) & "'")
            strSql.AppendLine(",'" & Agro_SQL_SaveText(Numero) & "'")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Mac_Cod) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Cod_RisUm) & " ")
            strSql.AppendLine(",'" & Agro_SQL_SaveText(Trasportatore) & "'")
            strSql.AppendLine(",'" & Agro_SQL_SaveText(Mezzo_Trasporto) & "'")
            strSql.AppendLine(",'" & Agro_SQL_SaveText(Targa) & "'")
            strSql.AppendLine(",'" & Agro_SQL_SaveText(N_Immatricolazione) & "'")
            strSql.AppendLine(",'" & Agro_SQL_SaveText(N_Immatricolazione_Rimorchio) & "'")
            strSql.AppendLine(",'" & Agro_SQL_SaveText(N_Autorizzazione_Trasporto) & "'")
            strSql.AppendLine(",'" & Agro_SQL_SaveText(Data_Rilascio_Autorizzazione) & "'")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Peso) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Codice_Prodotto) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Colore) & " ")
            strSql.AppendLine(",'" & Agro_SQL_SaveText(Zona_Viticola) & "'")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Manipolazioni) & " ")
            strSql.AppendLine(",'" & Agro_SQL_SaveText(Precisazioni) & "'")
            strSql.AppendLine(",'" & Agro_SQL_SaveText(Annotazioni) & "'")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Num_Contenitori) & " ")
            strSql.AppendLine(",'" & Agro_SQL_SaveText(Marche_Contenitori) & "'")
            strSql.AppendLine(",'" & Agro_SQL_SaveText(Des_Contenitori) & "'")
            strSql.AppendLine(",'" & Agro_SQL_SaveText(Tipo_Documento) & "'")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Id_Cod_Autorita) & " ")
            strSql.AppendLine(",'" & Agro_SQL_SaveText(Luogo_Partenza) & "'")
            strSql.AppendLine(",'" & Agro_SQL_SaveText(Luogo_Consegna) & "'")
            strSql.AppendLine(",'" & Agro_SQL_SaveText(Data_Spedizione) & "'")
            strSql.AppendLine(",'" & Agro_SQL_SaveText(Indicazioni_Complementari) & "'")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Titolo_Alcol) & " ")
            strSql.AppendLine(",'" & Agro_SQL_SaveText(Codice_NC) & "'")
            strSql.AppendLine(",'" & Agro_SQL_SaveText(Num_Riferimento) & "'")
            strSql.AppendLine(",'" & Agro_SQL_SaveText(Data_Dichiarazione) & "'")
            strSql.AppendLine(",'" & Agro_SQL_SaveText(Garanzia) & "'")
            strSql.AppendLine(",'" & Agro_SQL_SaveText(Certificati) & "'")
            strSql.AppendLine(",'" & Agro_SQL_SaveText(Durata_Viaggio) & "'")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Peso_Lordo) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Num_Colli) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Contenitore_Cod) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Imballaggio_Cod) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Agente_Cod) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Provvigione) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Tipo_Trasporto) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Unita_Trasporto) & " ")
            strSql.AppendLine(",'" & Agro_SQL_SaveText(Codice_Alternativo) & "'")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Id_Gestione_Vettore) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Ritenuta_Acconto_Cod) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Ritenuta_Acconto) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Enasarco_Cod) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Enasarco) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(ACCDAA_Cod_Risum_Destinatario) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(ACCDAA_Cod_Risum_Destinazione) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(ACCDAA_Cod_IndirizzoRisum_Destinatario) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(ACCDAA_Cod_IndirizzoRisum_Destinazione) & " ")

            strSql.AppendLine(", " & Agro_SQL_SaveNum(CapoArea_Cod) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Provvigione_CapoArea) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Provvigione_Pagata_Agente) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Provvigione_Pagata_CapoArea) & " ")
            strSql.AppendLine(", '" & Agro_SQL_SaveText(N_Doc_Cliente) & "' ")
            strSql.AppendLine(", " & Agro_SQL_SaveDate(Data_Doc_Cliente) & " ")
            strSql.AppendLine(", '" & Agro_SQL_SaveText(N_Doc_Ente) & "' ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Anno_Doc_Ente) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Num_Conf_Riscontrate) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Num_Colli_Riscontrati) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Num_Imballi_Riscontrati) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Peso_Netto_Riscontrato) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Peso_Lordo_Riscontrato) & " ")

            strSql.AppendLine(", " & Agro_SQL_SaveNum(Tara_Unit_Conf_Riscontrata) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Tara_Unit_Collo_Riscontrata) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Tara_Unit_Imballo_Riscontrata) & " ")
            strSql.AppendLine(",'" & Agro_SQL_SaveText(N_Nota_Fattura) & "'")
            strSql.AppendLine(", " & Agro_SQL_SaveDate(Data_Nota_Fattura) & " ")
            strSql.AppendLine(",'" & Agro_SQL_SaveText(N_Nota_DDT) & "'")
            strSql.AppendLine(",'" & Agro_SQL_SaveText(N_Nota_Riga_DDT) & "'")
            strSql.AppendLine(", " & Agro_SQL_SaveDate(Data_Nota_DDT) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Causale_Fattura) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(DistanzaTrasportoUdm) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(DistanzaTrasporto) & " ")

            strSql.AppendLine("         , 0  ")
            strSql.AppendLine("         , Null  ")

            strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            strSql.AppendLine("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            strSql.AppendLine("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            strSql.AppendLine(") ")

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

    <Obsolete("Usare la funzione ScriviFull")>
    Public Function Scrivi(ByVal Piva As String,
                           ByVal Sa_Cod As Integer,
                           ByVal Id_Agenda As Integer,
                           ByVal Id_Mov As Integer,
                           ByVal Id_Mov_Det As Integer,
                           ByVal Id_Reg_Dettaglio As Integer,
                           ByVal Regione As String,
                           ByVal ASL As String,
                           ByVal Serie As String,
                           ByVal Numero As String,
                           ByVal Mac_Cod As Integer,
                           ByVal Cod_RisUm As Integer,
                           ByVal Trasportatore As String,
                           ByVal Mezzo_Trasporto As String,
                           ByVal Targa As String,
                           ByVal N_Immatricolazione As String,
                           ByVal N_Immatricolazione_Rimorchio As String,
                           ByVal N_Autorizzazione_Trasporto As String,
                           ByVal Data_Rilascio_Autorizzazione As Date,
                           ByVal Peso As Decimal,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Dett_Tecnico_Ex_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

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

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" INSERT INTO Mov_Dettaglio_Tecnico_Extra ")
            strSql.AppendLine("         ( ")
            strSql.AppendLine("          Piva, Sa_Cod, Id_Agenda, Id_Mov, ")
            strSql.AppendLine("          Id_Mov_Det, Id_Reg_Dettaglio, ")
            strSql.AppendLine("          Regione, ASL, Serie, Numero, Mac_Cod, Cod_RisUm, ")
            strSql.AppendLine("          Trasportatore, Mezzo_Trasporto, Targa, N_Immatricolazione,  ")
            strSql.AppendLine("          N_Immatricolazione_Rimorchio, N_Autorizzazione_Trasporto,   ")
            strSql.AppendLine("          Data_Rilascio_Autorizzazione, Peso, ")

            strSql.AppendLine("          Inviato,            DataInvio, ")
            strSql.AppendLine("          Data_Creazione,     Data_Modifica, ")
            strSql.AppendLine("          UserName_Creazione, UserName_Modifica, ")
            strSql.AppendLine("          Validita_Inizio,    Validita_Fine ")
            strSql.AppendLine("         ) ")

            strSql.AppendLine(" VALUES ( ")
            strSql.AppendLine("          '" & Agro_SQL_SaveText(Piva) & "'  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Id_Agenda) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Id_Mov) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Id_Mov_Det) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Id_Reg_Dettaglio) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Regione) & "'  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(ASL) & "'  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Serie) & "'  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Numero) & "'  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Mac_Cod) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Cod_RisUm) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Trasportatore) & "'  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Mezzo_Trasporto) & "'  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Targa) & "'  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(N_Immatricolazione) & "'  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(N_Immatricolazione_Rimorchio) & "'  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(N_Autorizzazione_Trasporto) & "'  ")
            strSql.AppendLine("         , " & If(Data_Rilascio_Autorizzazione = New Date, "Null", Agro_SQL_SaveDate(Data_Rilascio_Autorizzazione)) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Peso) & "  ")

            strSql.AppendLine("         , 0  ")
            strSql.AppendLine("         , Null  ")

            strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            strSql.AppendLine("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            strSql.AppendLine("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")

            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            strSql.AppendLine(") ")

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


    Public Function ModificaPuntuale(ByVal Piva As String,
                                     ByVal Sa_Cod As Integer,
                                     ByVal Id_Agenda As Integer,
                                     ByVal Id_Mov As Integer,
                                     ByVal Id_Mov_Det As Integer,
                                     ByVal Id_Reg_Dettaglio As Integer,
                                     ByRef objParametri As AgronicaCoreParametri,
                                     Optional ByVal Regione As String = Nothing,
                                     Optional ByVal ASL As String = Nothing,
                                     Optional ByVal Serie As String = Nothing,
                                     Optional ByVal Numero As String = Nothing,
                                     Optional ByVal Mac_Cod As Integer? = Nothing,
                                     Optional ByVal Cod_RisUm As Integer? = Nothing,
                                     Optional ByVal Trasportatore As String = Nothing,
                                     Optional ByVal Mezzo_Trasporto As String = Nothing,
                                     Optional ByVal Targa As String = Nothing,
                                     Optional ByVal N_Immatricolazione As String = Nothing,
                                     Optional ByVal N_Immatricolazione_Rimorchio As String = Nothing,
                                     Optional ByVal N_Autorizzazione_Trasporto As String = Nothing,
                                     Optional ByVal Data_Rilascio_Autorizzazione As DateTime? = Nothing,
                                     Optional ByVal Peso As Decimal? = Nothing,
                                     Optional ByVal Validita_Inizio As Date? = Nothing,
                                     Optional ByVal Validita_Fine As Date? = Nothing,
                                     Optional ByVal Codice_Prodotto As Integer? = Nothing,
                                     Optional ByVal Colore As Integer? = Nothing,
                                     Optional ByVal Zona_Viticola As String = Nothing,
                                     Optional ByVal Manipolazioni As Integer? = Nothing,
                                     Optional ByVal Precisazioni As String = Nothing,
                                     Optional ByVal Annotazioni As String = Nothing,
                                     Optional ByVal Num_Contenitori As Integer? = Nothing,
                                     Optional ByVal Marche_Contenitori As String = Nothing,
                                     Optional ByVal Des_Contenitori As String = Nothing,
                                     Optional ByVal Tipo_Documento As String = Nothing,
                                     Optional ByVal Id_Cod_Autorita As Integer? = Nothing,
                                     Optional ByVal Luogo_Partenza As String = Nothing,
                                     Optional ByVal Luogo_Consegna As String = Nothing,
                                     Optional ByVal Data_Spedizione As DateTime? = Nothing,
                                     Optional ByVal Indicazioni_Complementari As String = Nothing,
                                     Optional ByVal Titolo_Alcol As Decimal? = Nothing,
                                     Optional ByVal Codice_NC As String = Nothing,
                                     Optional ByVal Num_Riferimento As String = Nothing,
                                     Optional ByVal Data_Dichiarazione As DateTime? = Nothing,
                                     Optional ByVal Garanzia As String = Nothing,
                                     Optional ByVal Certificati As String = Nothing,
                                     Optional ByVal Durata_Viaggio As String = Nothing,
                                     Optional ByVal Peso_Lordo As Decimal? = Nothing,
                                     Optional ByVal Num_Colli As Integer? = Nothing,
                                     Optional ByVal Contenitore_Cod As Integer? = Nothing,
                                     Optional ByVal Imballaggio_Cod As Integer? = Nothing,
                                     Optional ByVal Agente_Cod As Integer? = Nothing,
                                     Optional ByVal Provvigione As Decimal? = Nothing,
                                     Optional ByVal Tipo_Trasporto As Integer? = Nothing,
                                     Optional ByVal Unita_Trasporto As Integer? = Nothing,
                                     Optional ByVal Codice_Alternativo As String = Nothing,
                                     Optional ByVal Id_Gestione_Vettore As Integer? = Nothing,
                                     Optional ByVal Ritenuta_Acconto_Cod As Integer? = Nothing,
                                     Optional ByVal Ritenuta_Acconto As Decimal? = Nothing,
                                     Optional ByVal Enasarco_Cod As Integer? = Nothing,
                                     Optional ByVal Enasarco As Decimal? = Nothing,
                                     Optional ByVal ACCDAA_Cod_Risum_Destinatario As Integer? = Nothing,
                                     Optional ByVal ACCDAA_Cod_Risum_Destinazione As Integer? = Nothing,
                                     Optional ByVal ACCDAA_Cod_IndirizzoRisum_Destinatario As Integer? = Nothing,
                                     Optional ByVal ACCDAA_Cod_IndirizzoRisum_Destinazione As Integer? = Nothing,
                                     Optional ByVal CapoArea_Cod As Integer? = Nothing,
                                     Optional ByVal Provvigione_CapoArea As Decimal? = Nothing,
                                     Optional ByVal Provvigione_Pagata_Agente As Decimal? = Nothing,
                                     Optional ByVal Provvigione_Pagata_CapoArea As Decimal? = Nothing,
                                     Optional ByVal N_Doc_Cliente As String = Nothing,
                                     Optional ByVal Data_Doc_Cliente As Date? = Nothing,
                                     Optional ByVal N_Doc_Ente As String = Nothing,
                                     Optional ByVal Anno_Doc_Ente As Integer? = Nothing,
                                     Optional ByVal Num_Conf_Riscontrate As Integer? = Nothing,
                                     Optional ByVal Num_Colli_Riscontrati As Integer? = Nothing,
                                     Optional ByVal Num_Imballi_Riscontrati As Integer? = Nothing,
                                     Optional ByVal Peso_Netto_Riscontrato As Decimal? = Nothing,
                                     Optional ByVal Peso_Lordo_Riscontrato As Decimal? = Nothing,
                                     Optional ByVal Data_Modifica As DateTime = #2/1/1900#,
                                     Optional ByVal Username_Modifica As String = "",
                                     Optional ByVal Tara_Unit_Conf_Riscontrata As Decimal? = Nothing,
                                     Optional ByVal Tara_Unit_Collo_Riscontrata As Decimal? = Nothing,
                                     Optional ByVal Tara_Unit_Imballo_Riscontrata As Decimal? = Nothing,
                                     Optional ByVal N_Nota_Fattura As String = Nothing,
                                     Optional ByVal Data_Nota_Fattura As Date? = Nothing,
                                     Optional ByVal N_Nota_DDT As String = Nothing,
                                     Optional ByVal N_Nota_Riga_DDT As String = Nothing,
                                     Optional ByVal Data_Nota_DDT As Date? = Nothing,
                                     Optional ByVal Causale_Fattura As Integer? = Nothing,
                                     Optional ByVal DistanzaTrasportoUdm As Integer = 0,
                                     Optional ByVal DistanzaTrasporto As Decimal = 0
                                     ) As Boolean


        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Dett_Tecnico_Ex_W.ModificaPuntuale()"

        '====================================================================================
        'Parametri opzionali :
        '   Tutti i valori non chiave (se impostati a nothing o non passati 
        '   non ne verrà fatto l'aggiornamento e rimarranno i valori precedenti)
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            If Data_Modifica = #2/1/1900# Then
                Data_Modifica = Date.Now
            End If

            If Username_Modifica = "" Then
                Username_Modifica = objParametri.UsernameOperazione
            End If

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            'per alcuni tipi di movimento dettaglio il sa_cod è valorizzato a 0 (non valorizzato)
            'If Sa_Cod = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            'End If

            If Id_Agenda = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Agenda obbligatorio)")
            End If

            'If Id_Mov = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Id_Mov obbligatorio)")
            'End If

            'If Id_Mov_Det = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Id_Mov_Det obbligatorio)")
            'End If

            'If Id_Reg_Dettaglio = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Id_Reg_Dettaglio obbligatorio)")
            'End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE Mov_Dettaglio_Tecnico_Extra ")
            strSql.AppendLine(" SET Data_Modifica = " & Agro_SQL_SaveDateTime(Data_Modifica) & " ")
            strSql.AppendLine("   , Username_Modifica = '" & Agro_SQL_SaveText(Username_Modifica) & "' ")


            If Not IsNothing(Regione) Then
                strSql.AppendLine("   , Regione = '" & Agro_SQL_SaveText(Regione) & "' ")
            End If

            If Not IsNothing(ASL) Then
                strSql.AppendLine("   , ASL = '" & Agro_SQL_SaveText(ASL) & "' ")
            End If

            If Not IsNothing(Serie) Then
                strSql.AppendLine("   , Serie = '" & Agro_SQL_SaveText(Serie) & "' ")
            End If

            If Not IsNothing(Numero) Then
                strSql.AppendLine("   , Numero = '" & Agro_SQL_SaveText(Numero) & "' ")
            End If

            If Not IsNothing(Mac_Cod) Then
                strSql.AppendLine("   , Mac_Cod = " & Agro_SQL_SaveNum(Mac_Cod) & " ")
            End If

            If Not IsNothing(Cod_RisUm) Then
                strSql.AppendLine("   , Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & " ")
            End If

            If Not IsNothing(Trasportatore) Then
                strSql.AppendLine("   , Trasportatore = '" & Agro_SQL_SaveText(Trasportatore) & "' ")
            End If

            If Not IsNothing(Mezzo_Trasporto) Then
                strSql.AppendLine("   , Mezzo_Trasporto = '" & Agro_SQL_SaveText(Mezzo_Trasporto) & "' ")
            End If

            If Not IsNothing(Targa) Then
                strSql.AppendLine("   , Targa = '" & Agro_SQL_SaveText(Targa) & "' ")
            End If

            If Not IsNothing(N_Immatricolazione) Then
                strSql.AppendLine("   , N_Immatricolazione = '" & Agro_SQL_SaveText(N_Immatricolazione) & "' ")
            End If

            If Not IsNothing(N_Immatricolazione_Rimorchio) Then
                strSql.AppendLine("   , N_Immatricolazione_Rimorchio = '" & Agro_SQL_SaveText(N_Immatricolazione_Rimorchio) & "' ")
            End If

            If Not IsNothing(N_Autorizzazione_Trasporto) Then
                strSql.AppendLine("   , N_Autorizzazione_Trasporto = '" & Agro_SQL_SaveText(N_Autorizzazione_Trasporto) & "' ")
            End If

            If Not IsNothing(Data_Rilascio_Autorizzazione) Then
                strSql.AppendLine("   , Data_Rilascio_Autorizzazione = " & Agro_SQL_SaveDateTime(Data_Rilascio_Autorizzazione) & " ")
            End If

            If Not IsNothing(Peso) Then
                strSql.AppendLine("   , Peso = " & Agro_SQL_SaveNum(Peso) & " ")
            End If

            If Not IsNothing(Validita_Inizio) Then
                strSql.AppendLine("   , Validita_Inizio = " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            End If

            If Not IsNothing(Validita_Fine) Then
                strSql.AppendLine("   , Validita_Fine = " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            End If

            If Not IsNothing(Codice_Prodotto) Then
                strSql.AppendLine("   , Codice_Prodotto = " & Agro_SQL_SaveNum(Codice_Prodotto) & " ")
            End If

            If Not IsNothing(Colore) Then
                strSql.AppendLine("   , Colore = " & Agro_SQL_SaveNum(Colore) & " ")
            End If

            If Not IsNothing(Zona_Viticola) Then
                strSql.AppendLine("   , Zona_Viticola = '" & Agro_SQL_SaveText(Zona_Viticola) & "' ")
            End If

            If Not IsNothing(Manipolazioni) Then
                strSql.AppendLine("   , Manipolazioni = " & Agro_SQL_SaveNum(Manipolazioni) & " ")
            End If

            If Not IsNothing(Precisazioni) Then
                strSql.AppendLine("   , Precisazioni = '" & Agro_SQL_SaveText(Precisazioni) & "' ")
            End If

            If Not IsNothing(Annotazioni) Then
                strSql.AppendLine("   , Annotazioni = '" & Agro_SQL_SaveText(Annotazioni) & "' ")
            End If

            If Not IsNothing(Num_Contenitori) Then
                strSql.AppendLine("   , Num_Contenitori = " & Agro_SQL_SaveNum(Num_Contenitori) & " ")
            End If

            If Not IsNothing(Marche_Contenitori) Then
                strSql.AppendLine("   , Marche_Contenitori = '" & Agro_SQL_SaveText(Marche_Contenitori) & "' ")
            End If

            If Not IsNothing(Des_Contenitori) Then
                strSql.AppendLine("   , Des_Contenitori = '" & Agro_SQL_SaveText(Des_Contenitori) & "' ")
            End If

            If Not IsNothing(Tipo_Documento) Then
                strSql.AppendLine("   , Tipo_Documento = '" & Agro_SQL_SaveText(Tipo_Documento) & "' ")
            End If

            If Not IsNothing(Id_Cod_Autorita) Then
                strSql.AppendLine("   , Id_Cod_Autorita = " & Agro_SQL_SaveNum(Id_Cod_Autorita) & " ")
            End If

            If Not IsNothing(Luogo_Partenza) Then
                strSql.AppendLine("   , Luogo_Partenza = '" & Agro_SQL_SaveText(Luogo_Partenza) & "' ")
            End If

            If Not IsNothing(Luogo_Consegna) Then
                strSql.AppendLine("   , Luogo_Consegna = '" & Agro_SQL_SaveText(Luogo_Consegna) & "' ")
            End If

            If Not IsNothing(Data_Spedizione) Then
                strSql.AppendLine("   , Data_Spedizione = " & Agro_SQL_SaveDateTime(Data_Spedizione) & " ")
            End If

            If Not IsNothing(Indicazioni_Complementari) Then
                strSql.AppendLine("   , Indicazioni_Complementari = '" & Agro_SQL_SaveText(Indicazioni_Complementari) & "' ")
            End If

            If Not IsNothing(Titolo_Alcol) Then
                strSql.AppendLine("   , Titolo_Alcol = " & Agro_SQL_SaveNum(Titolo_Alcol) & " ")
            End If

            If Not IsNothing(Codice_NC) Then
                strSql.AppendLine("   , Codice_NC = '" & Agro_SQL_SaveText(Codice_NC) & "' ")
            End If

            If Not IsNothing(Num_Riferimento) Then
                strSql.AppendLine("   , Num_Riferimento = '" & Agro_SQL_SaveText(Num_Riferimento) & "' ")
            End If

            If Not IsNothing(Data_Dichiarazione) Then
                strSql.AppendLine("   , Data_Dichiarazione = " & Agro_SQL_SaveDateTime(Data_Dichiarazione) & " ")
            End If

            If Not IsNothing(Garanzia) Then
                strSql.AppendLine("   , Garanzia = '" & Agro_SQL_SaveText(Garanzia) & "' ")
            End If

            If Not IsNothing(Certificati) Then
                strSql.AppendLine("   , Certificati = '" & Agro_SQL_SaveText(Certificati) & "' ")
            End If

            If Not IsNothing(Durata_Viaggio) Then
                strSql.AppendLine("   , Durata_Viaggio = '" & Agro_SQL_SaveText(Durata_Viaggio) & "' ")
            End If

            If Not IsNothing(Peso_Lordo) Then
                strSql.AppendLine("   , Peso_Lordo = " & Agro_SQL_SaveNum(Peso_Lordo) & " ")
            End If

            If Not IsNothing(Num_Colli) Then
                strSql.AppendLine("   , Num_Colli = " & Agro_SQL_SaveNum(Num_Colli) & " ")
            End If

            If Not IsNothing(Contenitore_Cod) Then
                strSql.AppendLine("   , Contenitore_Cod = " & Agro_SQL_SaveNum(Contenitore_Cod) & " ")
            End If

            If Not IsNothing(Imballaggio_Cod) Then
                strSql.AppendLine("   , Imballaggio_Cod = " & Agro_SQL_SaveNum(Imballaggio_Cod) & " ")
            End If

            If Not IsNothing(Agente_Cod) Then
                strSql.AppendLine("   , Agente_Cod = " & Agro_SQL_SaveNum(Agente_Cod) & " ")
            End If

            If Not IsNothing(Provvigione) Then
                strSql.AppendLine("   , Provvigione = " & Agro_SQL_SaveNum(Provvigione) & " ")
            End If

            If Not IsNothing(Tipo_Trasporto) Then
                strSql.AppendLine("   , Tipo_Trasporto = " & Agro_SQL_SaveNum(Tipo_Trasporto) & " ")
            End If

            If Not IsNothing(Unita_Trasporto) Then
                strSql.AppendLine("   , Unita_Trasporto = " & Agro_SQL_SaveNum(Unita_Trasporto) & " ")
            End If

            If Not IsNothing(Codice_Alternativo) Then
                strSql.AppendLine("   , Codice_Alternativo = '" & Agro_SQL_SaveText(Codice_Alternativo) & "' ")
            End If

            If Not IsNothing(Id_Gestione_Vettore) Then
                strSql.AppendLine("   , Id_Gestione_Vettore = " & Agro_SQL_SaveNum(Id_Gestione_Vettore) & " ")
            End If

            If Not IsNothing(Ritenuta_Acconto_Cod) Then
                strSql.AppendLine("   , Ritenuta_Acconto_Cod = " & Agro_SQL_SaveNum(Ritenuta_Acconto_Cod) & " ")
            End If

            If Not IsNothing(Ritenuta_Acconto) Then
                strSql.AppendLine("   , Ritenuta_Acconto = " & Agro_SQL_SaveNum(Ritenuta_Acconto) & " ")
            End If

            If Not IsNothing(Enasarco_Cod) Then
                strSql.AppendLine("   , Enasarco_Cod = " & Agro_SQL_SaveNum(Enasarco_Cod) & " ")
            End If

            If Not IsNothing(Enasarco) Then
                strSql.AppendLine("   , Enasarco = " & Agro_SQL_SaveNum(Enasarco) & " ")
            End If

            If Not IsNothing(ACCDAA_Cod_Risum_Destinatario) Then
                strSql.AppendLine("   , ACCDAA_Cod_Risum_Destinatario = " & Agro_SQL_SaveNum(ACCDAA_Cod_Risum_Destinatario) & " ")
            End If

            If Not IsNothing(ACCDAA_Cod_Risum_Destinazione) Then
                strSql.AppendLine("   , ACCDAA_Cod_Risum_Destinazione = " & Agro_SQL_SaveNum(ACCDAA_Cod_Risum_Destinazione) & " ")
            End If

            If Not IsNothing(ACCDAA_Cod_IndirizzoRisum_Destinatario) Then
                strSql.AppendLine("   , ACCDAA_Cod_IndirizzoRisum_Destinatario = " & Agro_SQL_SaveNum(ACCDAA_Cod_IndirizzoRisum_Destinatario) & " ")
            End If

            If Not IsNothing(ACCDAA_Cod_IndirizzoRisum_Destinazione) Then
                strSql.AppendLine("   , ACCDAA_Cod_IndirizzoRisum_Destinazione = " & Agro_SQL_SaveNum(ACCDAA_Cod_IndirizzoRisum_Destinazione) & " ")
            End If

            If Not IsNothing(CapoArea_Cod) Then
                strSql.AppendLine("   , CapoArea_Cod = " & Agro_SQL_SaveNum(CapoArea_Cod) & " ")
            End If

            If Not IsNothing(Provvigione_CapoArea) Then
                strSql.AppendLine("   , Provvigione_CapoArea = " & Agro_SQL_SaveNum(Provvigione_CapoArea) & " ")
            End If

            If Not IsNothing(Provvigione_Pagata_Agente) Then
                strSql.AppendLine("   , Provvigione_Pagata_Agente = " & Agro_SQL_SaveNum(Provvigione_Pagata_Agente) & " ")
            End If

            If Not IsNothing(Provvigione_Pagata_CapoArea) Then
                strSql.AppendLine("   , Provvigione_Pagata_CapoArea = " & Agro_SQL_SaveNum(Provvigione_Pagata_CapoArea) & " ")
            End If

            If Not IsNothing(N_Doc_Cliente) Then
                strSql.AppendLine("   , N_Doc_Cliente = '" & Agro_SQL_SaveText(N_Doc_Cliente) & "' ")
            End If

            If Not IsNothing(Data_Doc_Cliente) Then
                strSql.AppendLine("   , Data_Doc_Cliente = " & Agro_SQL_SaveDate(Data_Doc_Cliente) & " ")
            End If

            If Not IsNothing(N_Doc_Ente) Then
                strSql.AppendLine("   , N_Doc_Ente = '" & Agro_SQL_SaveText(N_Doc_Ente) & "' ")
            End If

            If Not IsNothing(Anno_Doc_Ente) Then
                strSql.AppendLine("   , Anno_Doc_Ente = " & Agro_SQL_SaveNum(Anno_Doc_Ente) & " ")
            End If

            If Not IsNothing(Num_Conf_Riscontrate) Then
                strSql.AppendLine("   , Num_Conf_Riscontrate = " & Agro_SQL_SaveNum(Num_Conf_Riscontrate) & " ")
            End If

            If Not IsNothing(Num_Colli_Riscontrati) Then
                strSql.AppendLine("   , Num_Colli_Riscontrati = " & Agro_SQL_SaveNum(Num_Colli_Riscontrati) & " ")
            End If

            If Not IsNothing(Num_Imballi_Riscontrati) Then
                strSql.AppendLine("   , Num_Imballi_Riscontrati = " & Agro_SQL_SaveNum(Num_Imballi_Riscontrati) & " ")
            End If

            If Not IsNothing(Peso_Netto_Riscontrato) Then
                strSql.AppendLine("   , Peso_Netto_Riscontrato = " & Agro_SQL_SaveNum(Peso_Netto_Riscontrato) & " ")
            End If

            If Not IsNothing(Peso_Lordo_Riscontrato) Then
                strSql.AppendLine("   , Peso_Lordo_Riscontrato = " & Agro_SQL_SaveNum(Peso_Lordo_Riscontrato) & " ")
            End If

            If Not IsNothing(Tara_Unit_Conf_Riscontrata) Then
                strSql.AppendLine("   , Tara_Unit_Conf_Riscontrata = " & Agro_SQL_SaveNum(Tara_Unit_Conf_Riscontrata) & " ")
            End If

            If Not IsNothing(Tara_Unit_Collo_Riscontrata) Then
                strSql.AppendLine("   , Tara_Unit_Collo_Riscontrata = " & Agro_SQL_SaveNum(Tara_Unit_Collo_Riscontrata) & " ")
            End If

            If Not IsNothing(Tara_Unit_Imballo_Riscontrata) Then
                strSql.AppendLine("   , Tara_Unit_Imballo_Riscontrata = " & Agro_SQL_SaveNum(Tara_Unit_Imballo_Riscontrata) & " ")
            End If

            If Not IsNothing(N_Nota_Fattura) Then
                strSql.AppendLine("   , N_Nota_Fattura = '" & Agro_SQL_SaveText(N_Nota_Fattura) & "' ")
            End If

            If Not IsNothing(Data_Nota_Fattura) Then
                strSql.AppendLine("   , Data_Nota_Fattura = " & Agro_SQL_SaveDate(Data_Nota_Fattura) & " ")
            End If

            If Not IsNothing(N_Nota_DDT) Then
                strSql.AppendLine("   , N_Nota_DDT = '" & Agro_SQL_SaveText(N_Nota_DDT) & "' ")
            End If

            If Not IsNothing(N_Nota_Riga_DDT) Then
                strSql.AppendLine("   , N_Nota_Riga_DDT = '" & Agro_SQL_SaveText(N_Nota_Riga_DDT) & "' ")
            End If

            If Not IsNothing(Data_Nota_DDT) Then
                strSql.AppendLine("   , Data_Nota_DDT = " & Agro_SQL_SaveDate(Data_Nota_DDT) & " ")
            End If

            If Not IsNothing(Causale_Fattura) Then
                strSql.AppendLine("   , Causale_Fattura = " & Agro_SQL_SaveNum(Causale_Fattura) & " ")
            End If
            
            If Not IsNothing(DistanzaTrasportoUdm) Then
                strSql.AppendLine("   , Distanza_Trasporto_Udm = " & Agro_SQL_SaveNum(DistanzaTrasportoUdm) & " ")
            End If
            
            If Not IsNothing(DistanzaTrasporto) Then
                strSql.AppendLine("   , Distanza_Trasporto = " & Agro_SQL_SaveNum(DistanzaTrasporto) & " ")
            End If

            strSql.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            
            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            strSql.AppendLine(" AND Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")
            
            If Id_Mov <> 0 Then
                strSql.AppendLine(" AND Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & " ")
            End If

            If Id_Mov_Det <> 0 Then
                strSql.AppendLine(" AND Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_det) & " ")
            End If

            If Id_Reg_Dettaglio <> 0 Then
                strSql.AppendLine(" AND Id_Reg_Dettaglio = " & Agro_SQL_SaveNum(Id_Reg_Dettaglio) & " ")
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


    Public Function Cancella(ByVal Piva As String,
                             ByVal Sa_Cod As Integer,
                             ByVal Id_Agenda As Integer,
                             ByVal Id_Mov As Integer,
                             ByVal Id_Mov_Det As Integer,
                             ByVal Id_Reg_Dettaglio As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Dett_Tecnico_Ex_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""
        '   Sa_Cod = 0
        '   Id_Agenda = 0
        '   Id_Mov = 0
        '   Id_Mov_Det = 0
        '   Id_Reg_Dettaglio = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.Length = 0
                strSql.AppendLine(" UPDATE Mov_Dettaglio_Tecnico_Extra ")
                strSql.AppendLine(" SET ")
                strSql.AppendLine("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.AppendLine("      ,Inviato = -1 ")
                strSql.AppendLine(" WHERE  Inviato >= 0 ")

            Else
                strSql.Length = 0
                strSql.AppendLine(" DELETE ")
                strSql.AppendLine(" FROM Mov_Dettaglio_Tecnico_Extra ")
                strSql.AppendLine(" WHERE  1=1 ")
            End If

            If Piva <> String.Empty Then
                strSql.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Id_Agenda <> 0 Then
                strSql.AppendLine(" AND Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
            End If

            If Id_Mov <> 0 Then
                strSql.AppendLine(" AND Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
            End If

            If Id_Mov_Det <> 0 Then
                strSql.AppendLine(" AND Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")
            End If

            If Id_Reg_Dettaglio <> 0 Then
                strSql.AppendLine(" AND Id_Reg_Dettaglio = " & Agro_SQL_SaveNum(Id_Reg_Dettaglio) & "   ")
            End If


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

End Class
