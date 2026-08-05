Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Mov_Dettagli_Riferimenti_R
    Inherits DataProvider

    '---------------------------------------------------------------------------
    '--------------------- Esempi di riferimenti: ------------------------------
    '---------------------------------------------------------------------------
    '1) NOTA ACCREDITO CON FATTURA
    '
    '
    '2) DDT CON FATTURA:
    '   PRIMA PARTE: FATTURA (LAV_COD FATTURA E CAU_MOV DEL MAGAZZINO, ID_MOV E ID_MOV_DET VALORIZZATI)
    '   SECONDA PARTE: DDT  (LAV_COD DDT E CAU_MOV 4000, ID_MOV E ID_MOV_DET VALORIZZATI)
    '---------------------------------------------------------------------------
    '---------------------------------------------------------------------------
    '---------------------------------------------------------------------------
    '---------------------------------------------------------------------------


    Public Function Qta_Usate_Di_DDT_Agganciati_A_Lavorazioni( _
                                                             ByVal Piva As String, _
                                                             ByVal Id_Agenda As Int32, _
                                                            ByVal Id_Mov As Int32, _
                                                            ByVal Id_Mov_Det As Int32, _
                                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                         ) As Decimal

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R.Qta_Usate_Di_DDT_Agganciati_A_Lavorazioni()"


        Dim dt As DataTable = Recupera_DT_Rif_Unificato(Piva, 0, Id_Agenda, Id_Mov, Id_Mov_Det, 0, CAU_REGISTRAZIONI, objParametri)
        If dt.Rows.Count = 0 Then
            Return 0
        End If
        dt.Select("Cau_Mov_Risultato = '2300' ")

        Dim tot As Decimal = 0
        Dim i As Integer
        For i = 0 To dt.Rows.Count - 1
            tot += dt.Rows(i).Item("Qta_Risultato")
        Next

        Return tot


    End Function


    '###############################################################
    'UNION di due parti
    'nota: i campi passati vengono filtrati anche nei rispettivi rif
    Public Function Leggi(
                        ByVal Piva As String,
                        ByVal Sa_Cod As Int32,
                        ByVal Id_Agenda As Int32,
                        ByVal Id_Mov As Int32,
                        ByVal Id_Mov_Det As Int32,
                        ByVal Lav_Cod As Int32,
                        ByVal Cau_Mov As String,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            Optional ByVal leggiRiferimentiInversi As Boolean = True
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""  
        '   Id_Agenda = 0    
        '   Id_Mov = 0
        '   Id_Mov_Det = 0
        '   Lav_Cod = 0
        '   Cau_Mov = ""
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '------------------------------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("  ")

            StrSQL.Append(" (SELECT Agenda.Des_Lib, Mov_Dettagli_Riferimenti.Piva, Mov_Dettagli_Riferimenti.Sa_Cod, ")
            StrSQL.Append("  Mov_Dettagli_Riferimenti.Id_Agenda, Mov_Dettagli_Riferimenti.Id_Mov, Mov_Dettagli_Riferimenti.Id_Mov_Det, ")
            StrSQL.Append("  Mov_Dettagli_Riferimenti.Lav_Cod, Mov_Dettagli_Riferimenti.Cau_Mov, ")
            StrSQL.Append("  Mov_Dettagli_Riferimenti.Piva_Rif, Mov_Dettagli_Riferimenti.Sa_Cod_Rif, ")
            StrSQL.Append("  Mov_Dettagli_Riferimenti.Id_Agenda_Rif, Mov_Dettagli_Riferimenti.Id_Mov_Rif, Mov_Dettagli_Riferimenti.Id_Mov_Det_Rif, ")
            StrSQL.Append("  Mov_Dettagli_Riferimenti.Lav_Cod_Rif, Mov_Dettagli_Riferimenti.Cau_Mov_Rif, Mov_Dettagli_Riferimenti.Qta, ")
            StrSQL.Append("  Mov_Dettagli_Riferimenti.Data_Creazione, Mov_Dettagli_Riferimenti.Data_Modifica, ")
            StrSQL.Append("  Mov_Dettagli_Riferimenti.Username_Creazione, Mov_Dettagli_Riferimenti.Username_Modifica, ")
            StrSQL.Append("  Mov_Dettagli_Riferimenti.Validita_Inizio, Mov_Dettagli_Riferimenti.Validita_Fine, Mov_Dettagli_Riferimenti.Preserva_Legame, ")
            StrSQL.Append("  ISNULL(Mov_Dettagli_Riferimenti.Tipo_Associazione, 0) AS Tipo_Associazione ")
            StrSQL.Append(" FROM  Mov_Dettagli_Riferimenti WITH(NOLOCK), Agenda WITH(NOLOCK)")
            StrSQL.Append(" WHERE Mov_Dettagli_Riferimenti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Mov_Dettagli_Riferimenti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.Append(" AND   Mov_Dettagli_Riferimenti.Piva = Agenda.Piva ")
            StrSQL.Append(" AND   Mov_Dettagli_Riferimenti.Id_Agenda = Agenda.Id_Agenda" & "  ")

            If Piva <> "" Then
                StrSQL.Append(" AND Mov_Dettagli_Riferimenti.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Id_Agenda <> 0 Then
                StrSQL.Append(" AND Mov_Dettagli_Riferimenti.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
            End If

            If Id_Mov <> 0 Then
                StrSQL.Append(" AND Mov_Dettagli_Riferimenti.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
            End If

            If Id_Mov_Det <> 0 Then
                StrSQL.Append(" AND Mov_Dettagli_Riferimenti.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")
            End If

            If Lav_Cod <> 0 Then
                StrSQL.Append(" AND Mov_Dettagli_Riferimenti.lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & "   ")
            End If

            If Cau_Mov <> "" Then
                StrSQL.Append(" AND Mov_Dettagli_Riferimenti.Cau_Mov = '" & Agro_SQL_SaveText(Cau_Mov) & "'   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
            End If

            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Agenda.Inviato >=0 ")
                    StrSQL.Append(" AND   Mov_Dettagli_Riferimenti.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Agenda.Inviato =-1 ")
                    StrSQL.Append(" AND   Mov_Dettagli_Riferimenti.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select




            StrSQL.Append(" ) ")

            '  Vanni, 09/06/2014 11:21:29: leggo il riferimento inverso in maniera condizionale..
            If leggiRiferimentiInversi Then



                StrSQL.Append(" UNION ")
                StrSQL.Append(" ( ")

                StrSQL.Append(" SELECT Agenda.Des_Lib, Mov_Dettagli_Riferimenti.Piva, Mov_Dettagli_Riferimenti.Sa_Cod, ")
                StrSQL.Append("  Mov_Dettagli_Riferimenti.Id_Agenda, Mov_Dettagli_Riferimenti.Id_Mov, Mov_Dettagli_Riferimenti.Id_Mov_Det, ")
                StrSQL.Append("  Mov_Dettagli_Riferimenti.Lav_Cod, Mov_Dettagli_Riferimenti.Cau_Mov, ")
                StrSQL.Append("  Mov_Dettagli_Riferimenti.Piva_Rif, Mov_Dettagli_Riferimenti.Sa_Cod_Rif, ")
                StrSQL.Append("  Mov_Dettagli_Riferimenti.Id_Agenda_Rif, Mov_Dettagli_Riferimenti.Id_Mov_Rif, Mov_Dettagli_Riferimenti.Id_Mov_Det_Rif, ")
                StrSQL.Append("  Mov_Dettagli_Riferimenti.Lav_Cod_Rif, Mov_Dettagli_Riferimenti.Cau_Mov_Rif, Mov_Dettagli_Riferimenti.Qta, ")
                StrSQL.Append("  Mov_Dettagli_Riferimenti.Data_Creazione, Mov_Dettagli_Riferimenti.Data_Modifica, ")
                StrSQL.Append("  Mov_Dettagli_Riferimenti.Username_Creazione, Mov_Dettagli_Riferimenti.Username_Modifica, ")
                StrSQL.Append("  Mov_Dettagli_Riferimenti.Validita_Inizio, Mov_Dettagli_Riferimenti.Validita_Fine, Mov_Dettagli_Riferimenti.Preserva_Legame, ")
                StrSQL.Append("  ISNULL(Mov_Dettagli_Riferimenti.Tipo_Associazione, 0) AS Tipo_Associazione ")
                StrSQL.Append(" FROM  Mov_Dettagli_Riferimenti WITH(NOLOCK), Agenda WITH(NOLOCK)")
                StrSQL.Append(" WHERE Mov_Dettagli_Riferimenti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                StrSQL.Append(" AND   Mov_Dettagli_Riferimenti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                StrSQL.Append(" AND   Mov_Dettagli_Riferimenti.Piva_Rif = Agenda.Piva ")
                StrSQL.Append(" AND   Mov_Dettagli_Riferimenti.Id_Agenda_Rif = Agenda.Id_Agenda" & "  ")

                If Piva <> "" Then
                    StrSQL.Append(" AND Mov_Dettagli_Riferimenti.Piva_Rif = '" & Agro_SQL_SaveText(Piva) & "'   ")
                End If

                If Id_Agenda <> 0 Then
                    StrSQL.Append(" AND Mov_Dettagli_Riferimenti.Id_Agenda_Rif = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
                End If

                If Id_Mov <> 0 Then
                    StrSQL.Append(" AND Mov_Dettagli_Riferimenti.Id_Mov_Rif = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
                End If

                If Id_Mov_Det <> 0 Then
                    StrSQL.Append(" AND Mov_Dettagli_Riferimenti.Id_Mov_Det_Rif = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")
                End If

                If Lav_Cod <> 0 Then
                    StrSQL.Append(" AND Mov_Dettagli_Riferimenti.Lav_Cod_Rif = " & Agro_SQL_SaveNum(Lav_Cod) & "   ")
                End If

                If Cau_Mov <> "" Then
                    StrSQL.Append(" AND Mov_Dettagli_Riferimenti.Cau_Mov_Rif = '" & Agro_SQL_SaveText(Cau_Mov) & "'   ")
                End If


                If xFiltroAggiuntivo <> "" Then
                    StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
                End If

                Select Case objParametri.FlagVisibilita
                    Case enumVisibilita.Visibilita_SoloNonCancellati
                        StrSQL.Append(" AND   Agenda.Inviato >=0 ")
                        StrSQL.Append(" AND   Mov_Dettagli_Riferimenti.Inviato >=0 ")
                    Case enumVisibilita.Visibilita_SoloCancellati
                        StrSQL.Append(" AND   Agenda.Inviato =-1 ")
                        StrSQL.Append(" AND   Mov_Dettagli_Riferimenti.Inviato =-1 ")
                    Case enumVisibilita.Visibilita_Tutti
                        '...................................
                    Case Else
                        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                End Select



                StrSQL.Append(" ) ")

            End If '  fine Vanni, 09/06/2014 11:21:29: leggo il riferimento inverso in maniera condizionale..



            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))

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

    '###############################################################
    'UNION di due parti
    'nota: i campi passati vengono filtrati anche nei rispettivi rif
    Public Function LeggiPerAgenda(
                        ByVal Piva As String,
                        ByVal Sa_Cod As Int32,
                        ByVal Id_Agenda As Int32,
                        ByVal Lav_Cod As Int32,
                        ByVal Cau_Mov As String,
                        ByVal xFiltroAggiuntivo As String,
                        ByVal xOrderBy As String,
                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                        Optional ByVal leggiRiferimentiInversi As Boolean = True,
                        Optional ByVal Lav_Cod_Rif As Int32 = 0,
                        Optional ByVal Cau_Mov_Rif As String = ""
                        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R.LeggiPerAgenda()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""  
        '   Id_Agenda = 0    
        '   Id_Mov = 0
        '   Id_Mov_Det = 0
        '   Lav_Cod = 0
        '   Cau_Mov = ""
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '------------------------------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" (SELECT Agenda.Des_Lib, Mov_Dettagli_Riferimenti.Piva, Mov_Dettagli_Riferimenti.Sa_Cod, ")
            StrSQL.AppendLine("  Mov_Dettagli_Riferimenti.Id_Agenda, Mov_Dettagli_Riferimenti.Id_Mov, Mov_Dettagli_Riferimenti.Id_Mov_Det, ")
            StrSQL.AppendLine("  Mov_Dettagli_Riferimenti.Lav_Cod, Mov_Dettagli_Riferimenti.Cau_Mov, ")
            StrSQL.AppendLine("  Mov_Dettagli_Riferimenti.Piva_Rif, Mov_Dettagli_Riferimenti.Sa_Cod_Rif, ")
            StrSQL.AppendLine("  Mov_Dettagli_Riferimenti.Id_Agenda_Rif, Mov_Dettagli_Riferimenti.Id_Mov_Rif, Mov_Dettagli_Riferimenti.Id_Mov_Det_Rif, ")
            StrSQL.AppendLine("  Mov_Dettagli_Riferimenti.Lav_Cod_Rif, Mov_Dettagli_Riferimenti.Cau_Mov_Rif, Mov_Dettagli_Riferimenti.Qta, ")
            StrSQL.AppendLine("  Mov_Dettagli_Riferimenti.Data_Creazione, Mov_Dettagli_Riferimenti.Data_Modifica, ")
            StrSQL.AppendLine("  Mov_Dettagli_Riferimenti.Username_Creazione, Mov_Dettagli_Riferimenti.Username_Modifica, ")
            StrSQL.AppendLine("  Mov_Dettagli_Riferimenti.Validita_Inizio, Mov_Dettagli_Riferimenti.Validita_Fine, ")
            StrSQL.AppendLine("  Mov_Dettagli_Riferimenti.Preserva_Legame, ISNULL(Mov_Dettagli_Riferimenti.Tipo_Associazione, 0) AS Tipo_Associazione ")
            StrSQL.AppendLine(" FROM  Mov_Dettagli_Riferimenti WITH(NOLOCK), Agenda WITH(NOLOCK)")
            StrSQL.AppendLine(" WHERE Mov_Dettagli_Riferimenti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   Mov_Dettagli_Riferimenti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.AppendLine(" AND   Mov_Dettagli_Riferimenti.Piva = Agenda.Piva ")
            StrSQL.AppendLine(" AND   Mov_Dettagli_Riferimenti.Id_Agenda = Agenda.Id_Agenda" & "  ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND Mov_Dettagli_Riferimenti.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Id_Agenda <> 0 Then
                StrSQL.AppendLine(" AND Mov_Dettagli_Riferimenti.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
            End If

            StrSQL.AppendLine(" AND (Mov_Dettagli_Riferimenti.Id_Mov = 0 OR Mov_Dettagli_Riferimenti.Id_Mov = -1) ")

            StrSQL.AppendLine(" AND (Mov_Dettagli_Riferimenti.Id_Mov_Det = 0 OR Mov_Dettagli_Riferimenti.Id_Mov_Det = -1) ")

            If Lav_Cod <> 0 Then
                StrSQL.AppendLine(" AND Mov_Dettagli_Riferimenti.lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & "   ")
            End If

            If Cau_Mov <> "" Then
                StrSQL.AppendLine(" AND Mov_Dettagli_Riferimenti.Cau_Mov = '" & Agro_SQL_SaveText(Cau_Mov) & "'   ")
            End If

            If Lav_Cod_Rif <> 0 Then
                StrSQL.AppendLine(" AND Mov_Dettagli_Riferimenti.Lav_Cod_Rif = " & Agro_SQL_SaveNum(Lav_Cod_Rif) & "   ")
            End If

            If Cau_Mov_Rif <> "" Then
                StrSQL.AppendLine(" AND Mov_Dettagli_Riferimenti.Cau_Mov_Rif = '" & Agro_SQL_SaveText(Cau_Mov_Rif) & "'   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
            End If

            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   Agenda.Inviato >=0 ")
                    StrSQL.AppendLine(" AND   Mov_Dettagli_Riferimenti.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   Agenda.Inviato =-1 ")
                    StrSQL.AppendLine(" AND   Mov_Dettagli_Riferimenti.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select




            StrSQL.AppendLine(" ) ")

            '  Vanni, 09/06/2014 11:21:29: leggo il riferimento inverso in maniera condizionale..
            If leggiRiferimentiInversi Then

                StrSQL.AppendLine(" UNION ")
                StrSQL.AppendLine(" ( ")

                StrSQL.AppendLine(" SELECT Agenda.Des_Lib, Mov_Dettagli_Riferimenti.Piva, Mov_Dettagli_Riferimenti.Sa_Cod, ")
                StrSQL.AppendLine("  Mov_Dettagli_Riferimenti.Id_Agenda, Mov_Dettagli_Riferimenti.Id_Mov, Mov_Dettagli_Riferimenti.Id_Mov_Det, ")
                StrSQL.AppendLine("  Mov_Dettagli_Riferimenti.Lav_Cod, Mov_Dettagli_Riferimenti.Cau_Mov, ")
                StrSQL.AppendLine("  Mov_Dettagli_Riferimenti.Piva_Rif, Mov_Dettagli_Riferimenti.Sa_Cod_Rif, ")
                StrSQL.AppendLine("  Mov_Dettagli_Riferimenti.Id_Agenda_Rif, Mov_Dettagli_Riferimenti.Id_Mov_Rif, Mov_Dettagli_Riferimenti.Id_Mov_Det_Rif, ")
                StrSQL.AppendLine("  Mov_Dettagli_Riferimenti.Lav_Cod_Rif, Mov_Dettagli_Riferimenti.Cau_Mov_Rif, Mov_Dettagli_Riferimenti.Qta, ")
                StrSQL.AppendLine("  Mov_Dettagli_Riferimenti.Data_Creazione, Mov_Dettagli_Riferimenti.Data_Modifica, ")
                StrSQL.AppendLine("  Mov_Dettagli_Riferimenti.Username_Creazione, Mov_Dettagli_Riferimenti.Username_Modifica, ")
                StrSQL.AppendLine("  Mov_Dettagli_Riferimenti.Validita_Inizio, Mov_Dettagli_Riferimenti.Validita_Fine, ")
                StrSQL.AppendLine("  Mov_Dettagli_Riferimenti.Preserva_Legame, ISNULL(Mov_Dettagli_Riferimenti.Tipo_Associazione, 0) AS Tipo_Associazione ")
                StrSQL.AppendLine(" FROM  Mov_Dettagli_Riferimenti WITH(NOLOCK), Agenda WITH(NOLOCK)")
                StrSQL.AppendLine(" WHERE Mov_Dettagli_Riferimenti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                StrSQL.AppendLine(" AND   Mov_Dettagli_Riferimenti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                StrSQL.AppendLine(" AND   Mov_Dettagli_Riferimenti.Piva_Rif = Agenda.Piva ")
                StrSQL.AppendLine(" AND   Mov_Dettagli_Riferimenti.Id_Agenda_Rif = Agenda.Id_Agenda" & "  ")

                If Piva <> "" Then
                    StrSQL.AppendLine(" AND Mov_Dettagli_Riferimenti.Piva_Rif = '" & Agro_SQL_SaveText(Piva) & "'   ")
                End If

                If Id_Agenda <> 0 Then
                    StrSQL.AppendLine(" AND Mov_Dettagli_Riferimenti.Id_Agenda_Rif = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
                End If

                StrSQL.AppendLine(" AND (Mov_Dettagli_Riferimenti.Id_Mov_Rif = 0 OR Mov_Dettagli_Riferimenti.Id_Mov_Rif = -1) ")

                StrSQL.AppendLine(" AND (Mov_Dettagli_Riferimenti.Id_Mov_Det_Rif = 0 OR Mov_Dettagli_Riferimenti.Id_Mov_Det_Rif = -1) ")

                If Lav_Cod <> 0 Then
                    StrSQL.AppendLine(" AND Mov_Dettagli_Riferimenti.Lav_Cod_Rif = " & Agro_SQL_SaveNum(Lav_Cod) & "   ")
                End If

                If Cau_Mov <> "" Then
                    StrSQL.AppendLine(" AND Mov_Dettagli_Riferimenti.Cau_Mov_Rif = '" & Agro_SQL_SaveText(Cau_Mov) & "'   ")
                End If


                If xFiltroAggiuntivo <> "" Then
                    StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
                End If

                Select Case objParametri.FlagVisibilita
                    Case enumVisibilita.Visibilita_SoloNonCancellati
                        StrSQL.AppendLine(" AND   Agenda.Inviato >=0 ")
                        StrSQL.AppendLine(" AND   Mov_Dettagli_Riferimenti.Inviato >=0 ")
                    Case enumVisibilita.Visibilita_SoloCancellati
                        StrSQL.AppendLine(" AND   Agenda.Inviato =-1 ")
                        StrSQL.AppendLine(" AND   Mov_Dettagli_Riferimenti.Inviato =-1 ")
                    Case enumVisibilita.Visibilita_Tutti
                        '...................................
                    Case Else
                        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                End Select



                StrSQL.AppendLine(" ) ")

            End If '  fine Vanni, 09/06/2014 11:21:29: leggo il riferimento inverso in maniera condizionale..



            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))

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

    '###############################################################
    Public Function LeggiPerAgendaRif(ByVal Piva_Rif As String,
                                      ByVal Sa_Cod_Rif As Int32,
                                      ByVal Id_Agenda_Rif As Int32,
                                      ByVal Lav_Cod_Rif As Int32,
                                      ByVal Cau_Mov_Rif As String,
                                      ByVal xFiltroAggiuntivo As String,
                                      ByVal xOrderBy As String,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                      Optional ByVal Lav_Cod As Int32 = 0,
                                      Optional ByVal Cau_Mov As String = ""
                                      ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R.LeggiPerAgendaRif()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT Agenda.Des_Lib, Mov_Dettagli_Riferimenti.Piva, Mov_Dettagli_Riferimenti.Sa_Cod, ")
            StrSQL.AppendLine(" Mov_Dettagli_Riferimenti.Id_Agenda, ")
            StrSQL.AppendLine(" Mov_Dettagli_Riferimenti.Lav_Cod, Mov_Dettagli_Riferimenti.Cau_Mov, ")
            StrSQL.AppendLine(" Mov_Dettagli_Riferimenti.Piva_Rif, Mov_Dettagli_Riferimenti.Sa_Cod_Rif, ")
            StrSQL.AppendLine(" Mov_Dettagli_Riferimenti.Id_Agenda_Rif, ")
            StrSQL.AppendLine(" Mov_Dettagli_Riferimenti.Lav_Cod_Rif, Mov_Dettagli_Riferimenti.Cau_Mov_Rif, Mov_Dettagli_Riferimenti.Qta, ")
            StrSQL.AppendLine(" Mov_Dettagli_Riferimenti.Data_Creazione, Mov_Dettagli_Riferimenti.Data_Modifica, ")
            StrSQL.AppendLine(" Mov_Dettagli_Riferimenti.Username_Creazione, Mov_Dettagli_Riferimenti.Username_Modifica, ")
            StrSQL.AppendLine(" Mov_Dettagli_Riferimenti.Validita_Inizio, Mov_Dettagli_Riferimenti.Validita_Fine, ")
            StrSQL.AppendLine(" Mov_Dettagli_Riferimenti.Preserva_Legame, ISNULL(Mov_Dettagli_Riferimenti.Tipo_Associazione, 0) AS Tipo_Associazione ")
            StrSQL.AppendLine(" FROM Mov_Dettagli_Riferimenti ")
            StrSQL.AppendLine(" INNER JOIN Agenda Agenda ON Agenda.Piva = Mov_Dettagli_Riferimenti.Piva_Rif ")
            StrSQL.AppendLine(" AND Agenda.Id_Agenda = Mov_Dettagli_Riferimenti.Id_Agenda_Rif ")
            StrSQL.AppendLine(" WHERE Mov_Dettagli_Riferimenti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   Mov_Dettagli_Riferimenti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Piva_Rif <> "" Then
                StrSQL.AppendLine(" AND Mov_Dettagli_Riferimenti.Piva_Rif = '" & Agro_SQL_SaveText(Piva_Rif) & "'   ")
            End If

            If Id_Agenda_Rif <> 0 Then
                StrSQL.AppendLine(" AND Mov_Dettagli_Riferimenti.Id_Agenda_Rif = " & Agro_SQL_SaveNum(Id_Agenda_Rif) & "   ")
            End If

            StrSQL.AppendLine(" AND Mov_Dettagli_Riferimenti.Id_Mov_Rif IN (0,-1) ")

            StrSQL.AppendLine(" AND Mov_Dettagli_Riferimenti.Id_Mov_Det_Rif IN (0,-1) ")

            StrSQL.AppendLine(" AND Mov_Dettagli_Riferimenti.Id_Mov IN (0,-1) ")

            StrSQL.AppendLine(" AND Mov_Dettagli_Riferimenti.Id_Mov_Det IN (0,-1) ")

            If Lav_Cod_Rif <> 0 Then
                StrSQL.AppendLine(" AND Mov_Dettagli_Riferimenti.Lav_Cod_Rif = " & Agro_SQL_SaveNum(Lav_Cod_Rif) & "   ")
            End If

            If Cau_Mov_Rif <> "" Then
                StrSQL.AppendLine(" AND Mov_Dettagli_Riferimenti.Cau_Mov_Rif = '" & Agro_SQL_SaveText(Cau_Mov_Rif) & "'   ")
            End If

            If Lav_Cod <> 0 Then
                StrSQL.AppendLine(" AND Mov_Dettagli_Riferimenti.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & "   ")
            End If

            If Cau_Mov <> "" Then
                StrSQL.AppendLine(" AND Mov_Dettagli_Riferimenti.Cau_Mov = '" & Agro_SQL_SaveText(Cau_Mov) & "'   ")
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
            End If

            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   Agenda.Inviato >=0 ")
                    StrSQL.AppendLine(" AND   Mov_Dettagli_Riferimenti.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   Agenda.Inviato =-1 ")
                    StrSQL.AppendLine(" AND   Mov_Dettagli_Riferimenti.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    '###############################################################
    Public Function LeggixChiave(
                            ByVal Piva As String,
                            ByVal Sa_Cod As Int32,
                            ByVal Id_Agenda As Int32,
                            ByVal Id_Mov As Int32,
                            ByVal Id_Mov_Det As Int32,
                            ByVal Id_Agenda_Rif As Int32,
                            ByVal Id_Mov_Rif As Int32,
                            ByVal Id_Mov_Det_Rif As Int32,
                            ByVal Lav_Cod As Int32,
                            ByVal Cau_Mov As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            Optional ByVal xFiltroAggiuntivo As String = ""
                            ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R.LeggixChiave()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""
        '   Sa_Cod = 0                  ???          
        '   Id_Agenda = 0 
        '   Id_Mov = 0           
        '   Id_Mov_Det = 0
        '   Id_Agenda_Rif = 0  =
        '   Id_Mov_Rif = 0  
        '   Id_Mov_Det_Rif = 0  
        '   Lav_Cod = 0
        '   Cau_Mov = ""
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '------------------------------------------------------------------

            StrSQL.Length = 0

            StrSQL.Append(" SELECT Agenda.Des_Lib, Agenda.Validita_Inizio AS Validita_Inizio_Agenda, Mov_Dettagli_Riferimenti.Piva, Mov_Dettagli_Riferimenti.Sa_Cod, ")
            StrSQL.Append("         Mov_Dettagli_Riferimenti.Id_Agenda, Mov_Dettagli_Riferimenti.Id_Mov, Mov_Dettagli_Riferimenti.Id_Mov_Det, ")
            StrSQL.Append("         Mov_Dettagli_Riferimenti.Lav_Cod, Mov_Dettagli_Riferimenti.Cau_Mov, ")
            StrSQL.Append("         Mov_Dettagli_Riferimenti.Piva_Rif, Mov_Dettagli_Riferimenti.Sa_Cod_Rif, ")
            StrSQL.Append("         Mov_Dettagli_Riferimenti.Id_Agenda_Rif, Mov_Dettagli_Riferimenti.Id_Mov_Rif, Mov_Dettagli_Riferimenti.Id_Mov_Det_Rif, ")
            StrSQL.Append("         Mov_Dettagli_Riferimenti.Lav_Cod_Rif, Mov_Dettagli_Riferimenti.Cau_Mov_Rif, Mov_Dettagli_Riferimenti.Qta, ")
            StrSQL.Append("         Mov_Dettagli_Riferimenti.Validita_Inizio, Mov_Dettagli_Riferimenti.Validita_Fine, ")
            StrSQL.Append("         Mov_Dettagli_Riferimenti.Preserva_Legame, ")
            StrSQL.Append("         ISNULL(Mov_Dettagli_Riferimenti.Tipo_Associazione, 0) AS Tipo_Associazione ")
            StrSQL.Append(" FROM    Mov_Dettagli_Riferimenti WITH(NOLOCK), Agenda WITH(NOLOCK)")
            StrSQL.Append(" WHERE   Mov_Dettagli_Riferimenti.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND     Mov_Dettagli_Riferimenti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.Append(" AND     Mov_Dettagli_Riferimenti.Piva = Agenda.Piva ")
            StrSQL.Append(" AND     Mov_Dettagli_Riferimenti.Id_Agenda = Agenda.Id_Agenda" & "  ")

            If Piva <> "" Then
                StrSQL.Append(" AND Mov_Dettagli_Riferimenti.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            '   If Sa_Cod <> 0 Then
            '       StrSQL.Append(" AND Mov_Dettagli_Riferimenti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            '   End If

            If Id_Agenda <> 0 Then
                StrSQL.Append(" AND Mov_Dettagli_Riferimenti.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
            End If

            If Id_Mov <> 0 Then
                StrSQL.Append(" AND Mov_Dettagli_Riferimenti.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
            End If

            If Id_Mov_Det <> 0 Then
                StrSQL.Append(" AND Mov_Dettagli_Riferimenti.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")
            End If

            If Id_Agenda_Rif <> 0 Then
                StrSQL.Append(" AND Mov_Dettagli_Riferimenti.Id_Agenda_Rif = " & Agro_SQL_SaveNum(Id_Agenda_Rif) & "   ")
            End If

            If Id_Mov_Rif <> 0 Then
                StrSQL.Append(" AND Mov_Dettagli_Riferimenti.Id_Mov_Rif = " & Agro_SQL_SaveNum(Id_Mov_Rif) & "   ")
            End If

            If Id_Mov_Det_Rif <> 0 Then
                StrSQL.Append(" AND Mov_Dettagli_Riferimenti.Id_Mov_Det_Rif = " & Agro_SQL_SaveNum(Id_Mov_Det_Rif) & "   ")
            End If

            If Lav_Cod <> 0 Then
                StrSQL.Append(" AND Mov_Dettagli_Riferimenti.lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & "   ")
            End If

            If Cau_Mov <> "" Then
                StrSQL.Append(" AND Mov_Dettagli_Riferimenti.Cau_Mov = '" & Agro_SQL_SaveText(Cau_Mov) & "'   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
            End If

            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Agenda.Inviato >=0 ")
                    StrSQL.Append(" AND   Mov_Dettagli_Riferimenti.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Agenda.Inviato =-1 ")
                    StrSQL.Append(" AND   Mov_Dettagli_Riferimenti.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Mov_Dettagli_Riferimenti.Piva Asc ")
            End If


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            DT = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try


        Return DT

    End Function




    '###############################################################
    'ingresso: chiave della seconda parte (rif)
    'uscita: movimenti_dettagli in join con la prima parte
    '(per risalire al prezzo unitario)
    Public Function MovDettagliRiferimenti_Leggi_MovDettagli(
                            ByVal Piva_Rif As String,
                            ByVal Sa_Cod_Rif As Integer,
                            ByVal Id_Agenda_Rif As Integer,
                            ByVal Id_Mov_Rif As Integer,
                            ByVal Id_Mov_Det_Rif As Integer,
                            ByVal Lav_Cod_Rif As Integer,
                            ByVal Cau_Mov_Rif As String,
                            ByVal Lav_Cod As Integer,
                            ByVal Cau_Mov As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R.MovDettagliRiferimenti_Leggi_MovDettagli()"

        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '------------------------------------------------------------------

            StrSQL.Length = 0

            StrSQL.Append(" SELECT  Agenda.Des_Lib, Mov_Dettagli_Riferimenti.Piva, Mov_Dettagli_Riferimenti.Sa_Cod, Mov_Dettagli_Riferimenti.Id_Agenda,   " + vbCrLf)
            StrSQL.Append("  Mov_Dettagli_Riferimenti.Id_Mov, Mov_Dettagli_Riferimenti.Id_Mov_Det, Mov_Dettagli_Riferimenti.Lav_Cod, Mov_Dettagli_Riferimenti.Cau_Mov, " + vbCrLf)
            StrSQL.Append("  Mov_Dettagli_Riferimenti.Piva_Rif, Mov_Dettagli_Riferimenti.Sa_Cod_Rif, Mov_Dettagli_Riferimenti.Id_Agenda_Rif, " + vbCrLf)
            StrSQL.Append("  Mov_Dettagli_Riferimenti.Id_Mov_Rif, Mov_Dettagli_Riferimenti.Id_Mov_Det_Rif, Mov_Dettagli_Riferimenti.Lav_Cod_Rif,  " + vbCrLf)
            StrSQL.Append("  Mov_Dettagli_Riferimenti.Cau_Mov_Rif, Mov_Dettagli_Riferimenti.Qta, Mov_Dettagli_Riferimenti.Validita_Inizio, Mov_Dettagli_Riferimenti.Validita_Fine, " + vbCrLf)
            StrSQL.Append("  Movimenti_dettagli.Prezzo_Unitario, Movimenti_dettagli.Prezzo_Unitario_Netto, " + vbCrLf)
            StrSQL.Append("  Mov_Dettagli_Riferimenti.Preserva_Legame, " + vbCrLf)
            StrSQL.Append("  ISNULL(Mov_Dettagli_Riferimenti.Tipo_Associazione, 0) AS Tipo_Associazione " + vbCrLf)

            StrSQL.Append("  FROM         Mov_Dettagli_Riferimenti " + vbCrLf)

            StrSQL.Append(" INNER JOIN Movimenti_dettagli " + vbCrLf)
            StrSQL.Append(" ON   Mov_Dettagli_Riferimenti.Piva = Movimenti_dettagli.Piva " + vbCrLf)
            StrSQL.Append(" AND   Mov_Dettagli_Riferimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda " + vbCrLf)
            StrSQL.Append(" AND   Mov_Dettagli_Riferimenti.Id_Mov = Movimenti_dettagli.Id_Mov " + vbCrLf)
            StrSQL.Append(" AND   Mov_Dettagli_Riferimenti.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det " + vbCrLf)

            StrSQL.Append(" INNER JOIN Movimenti " + vbCrLf)
            StrSQL.Append(" ON   Movimenti.Piva = Movimenti_dettagli.Piva " + vbCrLf)
            StrSQL.Append(" AND   Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda " + vbCrLf)
            StrSQL.Append(" AND   Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov " + vbCrLf)

            StrSQL.Append(" INNER JOIN Agenda " + vbCrLf)
            StrSQL.Append(" ON   Movimenti.Piva = Agenda.Piva " + vbCrLf)
            StrSQL.Append(" AND   Movimenti.Id_Agenda = Agenda.Id_Agenda " + vbCrLf)

            'CONDIZIONI
            StrSQL.Append(" WHERE Mov_Dettagli_Riferimenti.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " + vbCrLf)
            StrSQL.Append(" AND   Mov_Dettagli_Riferimenti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " + vbCrLf)

            If Piva_Rif <> "" Then
                StrSQL.Append(" AND Mov_Dettagli_Riferimenti.Piva_Rif = '" & Agro_SQL_SaveText(Piva_Rif) & "'   " + vbCrLf)
            End If

            If Sa_Cod_Rif <> 0 Then
                StrSQL.Append(" AND Mov_Dettagli_Riferimenti.Sa_Cod_Rif = " & Agro_SQL_SaveNum(Sa_Cod_Rif) & "   " + vbCrLf)
            End If

            If Id_Agenda_Rif <> 0 Then
                StrSQL.Append(" AND Mov_Dettagli_Riferimenti.Id_Agenda_Rif = " & Agro_SQL_SaveNum(Id_Agenda_Rif) & "   " + vbCrLf)
            End If

            If Id_Mov_Rif <> 0 Then
                StrSQL.Append(" AND Mov_Dettagli_Riferimenti.Id_Mov_Rif = " & Agro_SQL_SaveNum(Id_Mov_Rif) & "   " + vbCrLf)
            End If

            If Id_Mov_Det_Rif <> 0 Then
                StrSQL.Append(" AND Mov_Dettagli_Riferimenti.Id_Mov_Det_Rif = " & Agro_SQL_SaveNum(Id_Mov_Det_Rif) & "   " + vbCrLf)
            End If

            If Lav_Cod_Rif <> 0 Then
                StrSQL.Append(" AND Mov_Dettagli_Riferimenti.Lav_Cod_Rif = " & Agro_SQL_SaveNum(Lav_Cod_Rif) & "   " + vbCrLf)
            End If

            If Cau_Mov_Rif <> "" Then
                StrSQL.Append(" AND Mov_Dettagli_Riferimenti.Cau_Mov_Rif = '" & Agro_SQL_SaveText(Cau_Mov_Rif) & "'   " + vbCrLf)
            End If

            If Lav_Cod <> 0 Then
                StrSQL.Append(" AND Mov_Dettagli_Riferimenti.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & "   " + vbCrLf)
            End If

            If Cau_Mov <> "" Then
                StrSQL.Append(" AND Mov_Dettagli_Riferimenti.Cau_Mov = '" & Agro_SQL_SaveText(Cau_Mov) & "'   " + vbCrLf)
            End If

            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Mov_Dettagli_Riferimenti.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Mov_Dettagli_Riferimenti.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Mov_Dettagli_Riferimenti.Piva Asc ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            DT = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try


        Return DT

    End Function


    '###############################################################
    'va testata
    'query secca, non fa union e filtra esattamente i campi passati
    Public Function Leggi_Specifica(
                            ByVal Piva As String,
                            ByVal Sa_Cod As Int32,
                            ByVal Id_Agenda As Int32,
                            ByVal Id_Mov As Int32,
                            ByVal Id_Mov_Det As Int32,
                            ByVal Lav_Cod As Int32,
                            ByVal Cau_Mov As String,
                            ByVal Piva_Rif As String,
                            ByVal Sa_Cod_Rif As Int32,
                            ByVal Id_Agenda_Rif As Int32,
                            ByVal Id_Mov_Rif As Int32,
                            ByVal Id_Mov_Det_Rif As Int32,
                            ByVal Lav_Cod_Rif As Int32,
                            ByVal Cau_Mov_Rif As String,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R.Leggi_Specifica()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""
        '   Sa_Cod = 0                           
        '   Id_Agenda = 0 
        '   Id_Mov = 0           
        '   Id_Mov_Det = 0
        '   Lav_Cod = 0
        '   Cau_Mov = ""
        '   Id_Agenda_Rif = 0 
        '   Id_Mov_Rif = 0  
        '   Id_Mov_Det_Rif = 0  
        '   Lav_Cod_Rif = 0
        '   Cau_Mov_Rif = ""
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '------------------------------------------------------------------

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT Mov_Dettagli_Riferimenti.Piva, Mov_Dettagli_Riferimenti.Sa_Cod, ")
            StrSQL.AppendLine("         Mov_Dettagli_Riferimenti.Id_Agenda, Mov_Dettagli_Riferimenti.Id_Mov, Mov_Dettagli_Riferimenti.Id_Mov_Det, ")
            StrSQL.AppendLine("         Mov_Dettagli_Riferimenti.Lav_Cod, Mov_Dettagli_Riferimenti.Cau_Mov, ")
            StrSQL.AppendLine("         Mov_Dettagli_Riferimenti.Piva_Rif, Mov_Dettagli_Riferimenti.Sa_Cod_Rif, ")
            StrSQL.AppendLine("         Mov_Dettagli_Riferimenti.Id_Agenda_Rif, Mov_Dettagli_Riferimenti.Id_Mov_Rif, Mov_Dettagli_Riferimenti.Id_Mov_Det_Rif, ")
            StrSQL.AppendLine("         Mov_Dettagli_Riferimenti.Lav_Cod_Rif, Mov_Dettagli_Riferimenti.Cau_Mov_Rif, Mov_Dettagli_Riferimenti.Qta, ")
            StrSQL.AppendLine("         Mov_Dettagli_Riferimenti.Validita_Inizio, Mov_Dettagli_Riferimenti.Validita_Fine, ")
            StrSQL.AppendLine("         Mov_Dettagli_Riferimenti.Preserva_Legame, ")
            StrSQL.AppendLine("         ISNULL(Mov_Dettagli_Riferimenti.Tipo_Associazione, 0) AS Tipo_Associazione ")
            StrSQL.AppendLine(" FROM    Mov_Dettagli_Riferimenti ")
            StrSQL.AppendLine(" WHERE   Mov_Dettagli_Riferimenti.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND     Mov_Dettagli_Riferimenti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND Mov_Dettagli_Riferimenti.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Mov_Dettagli_Riferimenti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Id_Agenda <> 0 Then
                StrSQL.AppendLine(" AND Mov_Dettagli_Riferimenti.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
            End If

            If Id_Mov <> 0 Then
                StrSQL.AppendLine(" AND Mov_Dettagli_Riferimenti.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
            End If

            If Id_Mov_Det <> 0 Then
                StrSQL.AppendLine(" AND Mov_Dettagli_Riferimenti.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")
            End If

            If Lav_Cod <> 0 Then
                StrSQL.AppendLine(" AND Mov_Dettagli_Riferimenti.lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & "   ")
            End If

            If Cau_Mov <> "" Then
                StrSQL.AppendLine(" AND Mov_Dettagli_Riferimenti.Cau_Mov = '" & Agro_SQL_SaveText(Cau_Mov) & "'   ")
            End If

            If Piva_Rif <> "" Then
                StrSQL.AppendLine(" AND Mov_Dettagli_Riferimenti.Piva_Rif = '" & Agro_SQL_SaveText(Piva_Rif) & "'   ")
            End If

            If Sa_Cod_Rif <> 0 Then
                StrSQL.AppendLine(" AND Mov_Dettagli_Riferimenti.Sa_Cod_Rif = " & Agro_SQL_SaveNum(Sa_Cod_Rif) & "   ")
            End If

            If Id_Agenda_Rif <> 0 Then
                StrSQL.AppendLine(" AND Mov_Dettagli_Riferimenti.Id_Agenda_Rif = " & Agro_SQL_SaveNum(Id_Agenda_Rif) & "   ")
            End If

            If Id_Mov_Rif <> 0 Then
                StrSQL.AppendLine(" AND Mov_Dettagli_Riferimenti.Id_Mov_Rif = " & Agro_SQL_SaveNum(Id_Mov_Rif) & "   ")
            End If

            If Id_Mov_Det_Rif <> 0 Then
                StrSQL.AppendLine(" AND Mov_Dettagli_Riferimenti.Id_Mov_Det_Rif = " & Agro_SQL_SaveNum(Id_Mov_Det_Rif) & "   ")
            End If

            If Lav_Cod_Rif <> 0 Then
                StrSQL.AppendLine(" AND Mov_Dettagli_Riferimenti.Lav_Cod_Rif = " & Agro_SQL_SaveNum(Lav_Cod_Rif) & "   ")
            End If

            If Cau_Mov_Rif <> "" Then
                StrSQL.AppendLine(" AND Mov_Dettagli_Riferimenti.Cau_Mov_Rif = '" & Agro_SQL_SaveText(Cau_Mov_Rif) & "'   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
            End If

            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   Mov_Dettagli_Riferimenti.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   Mov_Dettagli_Riferimenti.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select


            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            DT = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try


        Return DT

    End Function


    'per leggere in un colpo nel caso di 3 operazioni collegate,
    'ad esempio lo scarico, il carico e la raccolta/cura
    'che hanno due righe, carico-cura e scarico-cura etc
    'l'altro movimento collegato è nei campi piva_2 etc..
    Public Function Leggi_Join(
                               ByVal Piva As String,
                               ByVal Sa_Cod As Int32,
                               ByVal Id_Agenda As Int32,
                               ByVal Id_Mov As Int32,
                               ByVal Id_Mov_Det As Int32,
                               ByVal Lav_Cod As Int32,
                               ByVal Cau_Mov As String,
                               ByVal Piva_Rif As String,
                               ByVal Sa_Cod_Rif As Int32,
                               ByVal Id_Agenda_Rif As Int32,
                               ByVal Id_Mov_Rif As Int32,
                               ByVal Id_Mov_Det_Rif As Int32,
                               ByVal Lav_Cod_Rif As Int32,
                               ByVal Cau_Mov_Rif As String,
                                       ByVal xFiltroAggiuntivo As String,
                                       ByVal xOrderBy As String,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                       ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R.Leggi_Join()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""
        '   Sa_Cod = 0                           
        '   Id_Agenda = 0 
        '   Id_Mov = 0           
        '   Id_Mov_Det = 0
        '   Lav_Cod = 0
        '   Cau_Mov = ""
        '   Id_Agenda_Rif = 0 
        '   Id_Mov_Rif = 0  
        '   Id_Mov_Det_Rif = 0  
        '   Lav_Cod_Rif = 0
        '   Cau_Mov_Rif = ""
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '------------------------------------------------------------------

            StrSQL.Length = 0

            StrSQL.Append("SELECT        r1.Piva, r1.Sa_Cod, r1.Id_Agenda, r1.Id_Mov, r1.Id_Mov_Det, r1.Lav_Cod, r1.Cau_Mov, r1.Piva_Rif, r1.Sa_Cod_Rif, r1.Id_Agenda_Rif, r1.Id_Mov_Rif,  ")
            StrSQL.Append("                         r1.Id_Mov_Det_Rif, r1.Lav_Cod_Rif, r1.Cau_Mov_Rif, r1.Qta, r2.Piva AS Piva_2, r2.Sa_Cod AS Sa_Cod_2, r2.Id_Agenda AS Id_Agenda_2, r2.Id_Mov AS Id_Mov_2,  ")
            StrSQL.Append("                         r2.Id_Mov_Det AS Id_Mov_Det_2, r2.Lav_Cod AS Lav_Cod_2, r2.Cau_Mov AS Cau_Mov_2 ")
            StrSQL.Append(" FROM            Mov_Dettagli_Riferimenti AS r1 INNER JOIN ")
            StrSQL.Append("                 Mov_Dettagli_Riferimenti AS r2 ON r1.Piva_Rif = r2.Piva_Rif AND r1.Sa_Cod_Rif = r2.Sa_Cod_Rif AND r1.Id_Agenda_Rif = r2.Id_Agenda_Rif AND  ")
            StrSQL.Append("                          r1.Id_Mov_Rif = r2.Id_Mov_Rif AND r1.Id_Mov_Det_Rif = r2.Id_Mov_Det_Rif AND r1.Lav_Cod_Rif = r2.Lav_Cod_Rif ")

            'per non fare join sulla stessa operazione
            StrSQL.Append(" WHERE   r1.Id_Agenda <> r2.Id_Agenda  ")

            If Piva <> "" Then
                StrSQL.Append(" AND r1.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND r1.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Id_Agenda <> 0 Then
                StrSQL.Append(" AND r1.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
            End If

            If Id_Mov <> 0 Then
                StrSQL.Append(" AND r1.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
            End If

            If Id_Mov_Det <> 0 Then
                StrSQL.Append(" AND r1.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")
            End If

            If Lav_Cod <> 0 Then
                StrSQL.Append(" AND r1.lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & "   ")
            End If

            If Cau_Mov <> "" Then
                StrSQL.Append(" AND r1.Cau_Mov = '" & Agro_SQL_SaveText(Cau_Mov) & "'   ")
            End If

            If Piva_Rif <> "" Then
                StrSQL.Append(" AND r1.Piva_Rif = '" & Agro_SQL_SaveText(Piva_Rif) & "'   ")
            End If

            If Sa_Cod_Rif <> 0 Then
                StrSQL.Append(" AND r1.Sa_Cod_Rif = " & Agro_SQL_SaveNum(Sa_Cod_Rif) & "   ")
            End If

            If Id_Agenda_Rif <> 0 Then
                StrSQL.Append(" AND r1.Id_Agenda_Rif = " & Agro_SQL_SaveNum(Id_Agenda_Rif) & "   ")
            End If

            If Id_Mov_Rif <> 0 Then
                StrSQL.Append(" AND r1.Id_Mov_Rif = " & Agro_SQL_SaveNum(Id_Mov_Rif) & "   ")
            End If

            If Id_Mov_Det_Rif <> 0 Then
                StrSQL.Append(" AND r1.Id_Mov_Det_Rif = " & Agro_SQL_SaveNum(Id_Mov_Det_Rif) & "   ")
            End If

            If Lav_Cod_Rif <> 0 Then
                StrSQL.Append(" AND r1.Lav_Cod_Rif = " & Agro_SQL_SaveNum(Lav_Cod_Rif) & "   ")
            End If

            If Cau_Mov_Rif <> "" Then
                StrSQL.Append(" AND r1.Cau_Mov_Rif = '" & Agro_SQL_SaveText(Cau_Mov_Rif) & "'   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
            End If

            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   r1.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   r1.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select


            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            DT = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try


        Return DT

    End Function


    '###############################################################
    'base Leggi_Specifica
    'con aggiunta di agenda e movimenti in join con la parte rif
    'va bene per leggere i ddt agganciati a fattura
    'oppure le note agganciate a fattura
    '(che hanno cau_mov = 4000 E id_mov e id_mov_det = -1)
    'PRIMA PARTE: FATTURA
    'SECONDA PARTE: DDT O NOTA
    'Bolla agganciata a fattura: il riferimento è legato al movimento dettaglio
    Public Function Leggi_DDT_Nota_aggancio_Fattura(ByVal Piva As String,
                                                    ByVal Sa_Cod As Int32,
                                                    ByVal Id_Agenda As Int32,
                                                    ByVal Id_Mov As Int32,
                                                    ByVal Id_Mov_Det As Int32,
                                                    ByVal Lav_Cod As Int32,
                                                    ByVal Cau_Mov As String,
                                                    ByVal Piva_Rif As String,
                                                    ByVal Sa_Cod_Rif As Int32,
                                                    ByVal Id_Agenda_Rif As Int32,
                                                    ByVal Id_Mov_Rif As Int32,
                                                    ByVal Id_Mov_Det_Rif As Int32,
                                                    ByVal Lav_Cod_Rif As Int32,
                                                    ByVal Cau_Mov_Rif As String,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                    Optional ByVal Flag_Qta_Dettagli As Boolean = False
                                                    ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R.Leggi_DDT_aggancio_Fattura()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""
        '   Sa_Cod = 0                           
        '   Id_Agenda = 0 
        '   Id_Mov = 0           
        '   Id_Mov_Det = 0
        '   Lav_Cod = 0
        '   Cau_Mov = ""
        '   Id_Agenda_Rif = 0 
        '   Id_Mov_Rif = 0  
        '   Id_Mov_Det_Rif = 0  
        '   Lav_Cod_Rif = 0
        '   Cau_Mov_Rif = ""
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '------------------------------------------------------------------

            StrSQL.Length = 0

            StrSQL.Append(" SELECT Mov_Dettagli_Riferimenti.Piva, Mov_Dettagli_Riferimenti.Sa_Cod, " + vbCrLf)
            StrSQL.Append("         Mov_Dettagli_Riferimenti.Id_Agenda, Mov_Dettagli_Riferimenti.Id_Mov, Mov_Dettagli_Riferimenti.Id_Mov_Det, " + vbCrLf)
            StrSQL.Append("         Mov_Dettagli_Riferimenti.Lav_Cod, Mov_Dettagli_Riferimenti.Cau_Mov, " + vbCrLf)
            StrSQL.Append("         Mov_Dettagli_Riferimenti.Piva_Rif, Mov_Dettagli_Riferimenti.Sa_Cod_Rif, " + vbCrLf)
            StrSQL.Append("         Mov_Dettagli_Riferimenti.Id_Agenda_Rif, Mov_Dettagli_Riferimenti.Id_Mov_Rif, Mov_Dettagli_Riferimenti.Id_Mov_Det_Rif, " + vbCrLf)
            StrSQL.Append("         Mov_Dettagli_Riferimenti.Lav_Cod_Rif, Mov_Dettagli_Riferimenti.Cau_Mov_Rif, Mov_Dettagli_Riferimenti.Qta, " + vbCrLf)
            StrSQL.Append("         Mov_Dettagli_Riferimenti.Validita_Inizio, Mov_Dettagli_Riferimenti.Validita_Fine, " + vbCrLf)
            StrSQL.Append("         Mov_Dettagli_Riferimenti.Preserva_Legame, " + vbCrLf)
            StrSQL.Append("         ISNULL(Mov_Dettagli_Riferimenti.Tipo_Associazione, 0) AS Tipo_Associazione " + vbCrLf)
            StrSQL.Append("         , Mov_Contabile.Data_movimento, Mov_Contabile.Doc_Numero_Sin, Mov_Contabile.Doc_Numero , Mov_Contabile.Doc_Numero_Des   " + vbCrLf)
            StrSQL.Append("         , Mov_Cont_FATT.Data_movimento AS Data_movimento_FATT, Mov_Cont_FATT.Doc_Numero_Sin AS Doc_Numero_Sin_FATT, Mov_Cont_FATT.Doc_Numero AS Doc_Numero_FATT, Mov_Cont_FATT.Doc_Numero_Des AS Doc_Numero_Des_FATT " + vbCrLf)

            '  Giulia, 02/11/2016 17.33.27: Aggiunto per estrapolare il totale di quel dettaglio sul'ordine/doc collegato
            If Flag_Qta_Dettagli = True Then
                StrSQL.Append("         , md.qta as Qta_Tot_Doc_Rif" + vbCrLf)
            Else
                StrSQL.Append("         , 0 as Qta_Tot_Doc_Rif" + vbCrLf)
            End If

            StrSQL.Append(" FROM    Mov_Dettagli_Riferimenti " + vbCrLf)

            StrSQL.Append(" INNER JOIN Agenda " + vbCrLf)
            StrSQL.Append(" ON   Agenda.Piva = Mov_Dettagli_Riferimenti.Piva_Rif " + vbCrLf)
            StrSQL.Append(" AND   Agenda.Id_Agenda = Mov_Dettagli_Riferimenti.Id_Agenda_Rif " + vbCrLf)

            StrSQL.Append(" INNER JOIN Movimenti Mov_Contabile " + vbCrLf)
            StrSQL.Append(" ON   Mov_Contabile.Piva = Agenda.Piva " + vbCrLf)
            StrSQL.Append(" AND   Mov_Contabile.Id_Agenda = Agenda.Id_Agenda " + vbCrLf)

            StrSQL.Append(" INNER JOIN Agenda A_FATT " + vbCrLf)
            StrSQL.Append(" ON   A_FATT.Piva = Mov_Dettagli_Riferimenti.Piva " + vbCrLf)
            StrSQL.Append(" AND   A_FATT.Id_Agenda = Mov_Dettagli_Riferimenti.Id_Agenda " + vbCrLf)

            StrSQL.Append(" INNER JOIN Movimenti Mov_Cont_FATT " + vbCrLf)
            StrSQL.Append(" ON   Mov_Cont_FATT.Piva = A_FATT.Piva " + vbCrLf)
            StrSQL.Append(" AND   Mov_Cont_FATT.Id_Agenda = A_FATT.Id_Agenda " + vbCrLf)

            '  Giulia, 02/11/2016 17.33.27: Aggiunto per estrapolare il totale di quel dettaglio sul'ordine/doc collegato
            If Flag_Qta_Dettagli = True Then
                StrSQL.Append(" INNER JOIN Movimenti_dettagli md " & vbCrLf)
                StrSQL.Append(" ON md.id_agenda = Mov_Dettagli_Riferimenti.Id_Agenda_rif " & vbCrLf)
                StrSQL.Append(" AND md.Id_Mov = Mov_Dettagli_Riferimenti.Id_Mov_rif " & vbCrLf)
                StrSQL.Append(" AND md.Id_Mov_Det = Mov_Dettagli_Riferimenti.Id_Mov_det_rif ")
            End If

            StrSQL.Append(" WHERE   Mov_Dettagli_Riferimenti.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND     Mov_Dettagli_Riferimenti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            StrSQL.Append(" AND     Mov_Contabile.Cau_Mov = '" & CAU_REGISTRAZIONI & "' ")
            StrSQL.Append(" AND     Mov_Cont_FATT.Cau_Mov = '" & CAU_REGISTRAZIONI & "' ")

            If Piva <> "" Then
                StrSQL.Append(" AND Mov_Dettagli_Riferimenti.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Mov_Dettagli_Riferimenti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Id_Agenda <> 0 Then
                StrSQL.Append(" AND Mov_Dettagli_Riferimenti.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
            End If

            If Id_Mov <> 0 Then
                StrSQL.Append(" AND Mov_Dettagli_Riferimenti.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
            End If

            If Id_Mov_Det <> 0 Then
                StrSQL.Append(" AND Mov_Dettagli_Riferimenti.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")
            End If

            If Lav_Cod <> 0 Then
                StrSQL.Append(" AND Mov_Dettagli_Riferimenti.lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & "   ")
            End If

            If Cau_Mov <> "" Then
                StrSQL.Append(" AND Mov_Dettagli_Riferimenti.Cau_Mov = '" & Agro_SQL_SaveText(Cau_Mov) & "'   ")
            End If

            If Piva_Rif <> "" Then
                StrSQL.Append(" AND Mov_Dettagli_Riferimenti.Piva_Rif = '" & Agro_SQL_SaveText(Piva_Rif) & "'   ")
            End If

            If Sa_Cod_Rif <> 0 Then
                StrSQL.Append(" AND Mov_Dettagli_Riferimenti.Sa_Cod_Rif = " & Agro_SQL_SaveNum(Sa_Cod_Rif) & "   ")
            End If

            If Id_Agenda_Rif <> 0 Then
                StrSQL.Append(" AND Mov_Dettagli_Riferimenti.Id_Agenda_Rif = " & Agro_SQL_SaveNum(Id_Agenda_Rif) & "   ")
            End If

            If Id_Mov_Rif <> 0 Then
                StrSQL.Append(" AND Mov_Dettagli_Riferimenti.Id_Mov_Rif = " & Agro_SQL_SaveNum(Id_Mov_Rif) & "   ")
            End If

            If Id_Mov_Det_Rif <> 0 Then
                StrSQL.Append(" AND Mov_Dettagli_Riferimenti.Id_Mov_Det_Rif = " & Agro_SQL_SaveNum(Id_Mov_Det_Rif) & "   ")
            End If

            If Lav_Cod_Rif <> 0 Then
                StrSQL.Append(" AND Mov_Dettagli_Riferimenti.Lav_Cod_Rif = " & Agro_SQL_SaveNum(Lav_Cod_Rif) & "   ")
            End If

            If Cau_Mov_Rif <> "" Then
                StrSQL.Append(" AND Mov_Dettagli_Riferimenti.Cau_Mov_Rif = '" & Agro_SQL_SaveText(Cau_Mov_Rif) & "'   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
            End If

            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Mov_Dettagli_Riferimenti.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Mov_Dettagli_Riferimenti.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select


            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            DT = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try


        Return DT

    End Function

    '#############################################################################################
    'Corrisponde a: Agro_Contab_AD.Mov_Det_Riferimenti_R.LeggiRiferimenti
    '(con una modifica riguardo alla lettura dei des_lib e delle validita_inizio)
    'Legge il dettaglio sia che si trovi nella prima parte (piva, sa_cod, ecc..), sia che si trovi nella seconda (piva_rif, sa_cod_rif, ecc...)
    'Optional ByVal Piva As String = "", _
    '                           Optional ByVal Sa_Cod As Integer = 0, _
    '                           Optional ByVal Id_Agenda As Integer = 0, _
    '                           Optional ByVal Id_Mov As Integer = 0, _
    '                           Optional ByVal Id_Mov_Det As Integer = 0, _
    '                           Optional ByVal Lav_Cod As Integer = 0, _
    '                           Optional ByVal Cau_Mov As String = "", 
    'attenzione a order by!
    Public Function MovimentiRiferimenti_LeggiBilaterale(ByVal Piva As String,
                                                         ByVal Sa_Cod As Integer,
                                                         ByVal Id_Agenda As Integer,
                                                         ByVal Id_Mov As Integer,
                                                         ByVal Id_Mov_Det As Integer,
                                                         ByVal Lav_Cod As Integer,
                                                         ByVal Cau_Mov As String,
                                                         ByVal xOrderBy As String,
                                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                         Optional ByVal conRaccoglitore As Boolean = False
                                                         ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R.MovimentiRiferimenti_LeggiBilaterale()"

        Dim messaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '----------------------------------------------------
            '--- Preparo la Query SQL ---------------------------
            '----------------------------------------------------
            stb.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")
            stb.Append(" ( ")
            stb.Append(" SELECT  Ag1.Des_Lib, Ag1.Validita_Inizio, Ag2.Des_Lib AS Des_Lib_Rif, Ag2.Validita_Inizio AS Validita_Inizio_Rif, Mov_Dettagli_Riferimenti.Piva, Mov_Dettagli_Riferimenti.Sa_Cod, Mov_Dettagli_Riferimenti.Id_Agenda,   " & vbCrLf)
            stb.Append("         Mov_Dettagli_Riferimenti.Id_Mov, Mov_Dettagli_Riferimenti.Id_Mov_Det, Mov_Dettagli_Riferimenti.Lav_Cod, Mov_Dettagli_Riferimenti.Cau_Mov, " & vbCrLf)
            stb.Append("         Mov_Dettagli_Riferimenti.Piva_Rif, Mov_Dettagli_Riferimenti.Sa_Cod_Rif, Mov_Dettagli_Riferimenti.Id_Agenda_Rif, " & vbCrLf)
            stb.Append("         Mov_Dettagli_Riferimenti.Id_Mov_Rif, Mov_Dettagli_Riferimenti.Id_Mov_Det_Rif, Mov_Dettagli_Riferimenti.Lav_Cod_Rif,  " & vbCrLf)
            stb.Append("         Mov_Dettagli_Riferimenti.Cau_Mov_Rif, Mov_Dettagli_Riferimenti.Qta, Isnull(Mov_Dettagli_Riferimenti.Preserva_Legame,0) as Preserva_Legame " & vbCrLf)
            stb.Append("         ,Mov_Dettagli_Riferimenti.Validita_Inizio AS Validita_Inizio_Tabella, Mov_Dettagli_Riferimenti.Validita_Fine AS Validita_Fine_Tabella " & vbCrLf)
            stb.Append("         ,ISNULL(Mov_Dettagli_Riferimenti.Tipo_Associazione, 0) AS Tipo_Associazione " & vbCrLf)
            stb.Append("         ,Imprese.rag_soc, Imprese_1.rag_soc AS rag_soc_rif " & vbCrLf)

            If conRaccoglitore Then
                stb.Append("         ,ag1.Raccoglitore_Cod, ag2.Raccoglitore_Cod as Raccoglitore_Cod_Rif " & vbCrLf)
            End If

            stb.Append("  FROM   Mov_Dettagli_Riferimenti " & vbCrLf)

            'stb.Append( " INNER JOIN Agenda " & vbCrLf)
            'stb.Append( " ON   Mov_Dettagli_Riferimenti.Piva_Rif = Agenda.Piva " & vbCrLf)
            'stb.Append( " AND   Mov_Dettagli_Riferimenti.Id_Agenda_Rif = Agenda.Id_Agenda " & vbCrLf)
            '------->
            'l'operazione riferita è nella parte rif
            stb.Append(" INNER JOIN Agenda Ag2 " & vbCrLf)
            stb.Append(" ON      Mov_Dettagli_Riferimenti.Piva_Rif = Ag2.Piva " & vbCrLf)
            stb.Append(" AND     Mov_Dettagli_Riferimenti.Id_Agenda_Rif = Ag2.Id_Agenda " & vbCrLf)
            stb.Append(" INNER JOIN Agenda Ag1 " & vbCrLf)
            stb.Append(" ON      Mov_Dettagli_Riferimenti.Piva = Ag1.Piva " & vbCrLf)
            stb.Append(" AND     Mov_Dettagli_Riferimenti.Id_Agenda = Ag1.Id_Agenda " & vbCrLf)

            stb.Append(" INNER JOIN Imprese ON Mov_Dettagli_Riferimenti.Piva = Imprese.PIVA INNER JOIN " & vbCrLf)
            stb.Append(" Imprese AS Imprese_1 ON Mov_Dettagli_Riferimenti.Piva_Rif = Imprese_1.PIVA " & vbCrLf)

            stb.Append(" WHERE   Mov_Dettagli_Riferimenti.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " & vbCrLf)
            stb.Append(" AND     Mov_Dettagli_Riferimenti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " & vbCrLf)

            If Piva <> "" Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Piva = '" & Agro_SQL_SaveText(Piva) & "'   " & vbCrLf)
            End If

            'If Sa_Cod <> 0 Then
            '    stb.Append( " AND Mov_Dettagli_Riferimenti.Sa_Cod = " & Agro_SQL_savenum(Sa_Cod) & "   " & vbCrLf)
            'End If

            If Id_Agenda <> 0 Then
                stb.Append(" AND (Mov_Dettagli_Riferimenti.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   " & vbCrLf)
                If conRaccoglitore Then
                    stb.Append(" OR Mov_Dettagli_Riferimenti.Id_Agenda IN (SELECT DISTINCT Id_Agenda FROM Agenda WHERE ISNULL(Raccoglitore_Cod, 0) = ISNULL((SELECT CASE WHEN ISNULL(Raccoglitore_Cod, 0) > 0 THEN Raccoglitore_Cod ELSE -999 END FROM Agenda WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' AND Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "),-999))")
                End If
                stb.Append(" ) " & "   " & vbCrLf)
            End If

            If Id_Mov <> 0 Then
                stb.Append(" And Mov_Dettagli_Riferimenti.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   " & vbCrLf)
            End If

            If Id_Mov_Det <> 0 Then
                stb.Append(" And Mov_Dettagli_Riferimenti.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   " & vbCrLf)
            End If

            If Lav_Cod <> 0 Then
                stb.Append(" And Mov_Dettagli_Riferimenti.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & "   " & vbCrLf)
            End If

            If Cau_Mov <> "" Then
                stb.Append(" And Mov_Dettagli_Riferimenti.Cau_Mov = '" & Agro_SQL_SaveText(Cau_Mov) & "'   " & vbCrLf)
            End If

            stb.Append(" )  ")
            stb.Append(" UNION " & vbCrLf)
            stb.Append(" ( ")

            stb.Append(" SELECT  Ag1.Des_Lib, Ag1.Validita_Inizio, Ag2.Des_Lib AS Des_Lib_Rif, Ag2.Validita_Inizio AS Validita_Inizio_Rif,  Mov_Dettagli_Riferimenti.Piva, Mov_Dettagli_Riferimenti.Sa_Cod, Mov_Dettagli_Riferimenti.Id_Agenda,   " & vbCrLf)
            stb.Append("         Mov_Dettagli_Riferimenti.Id_Mov, Mov_Dettagli_Riferimenti.Id_Mov_Det, Mov_Dettagli_Riferimenti.Lav_Cod, Mov_Dettagli_Riferimenti.Cau_Mov, " & vbCrLf)
            stb.Append("         Mov_Dettagli_Riferimenti.Piva_Rif, Mov_Dettagli_Riferimenti.Sa_Cod_Rif, Mov_Dettagli_Riferimenti.Id_Agenda_Rif, " & vbCrLf)
            stb.Append("         Mov_Dettagli_Riferimenti.Id_Mov_Rif, Mov_Dettagli_Riferimenti.Id_Mov_Det_Rif, Mov_Dettagli_Riferimenti.Lav_Cod_Rif,  " & vbCrLf)
            stb.Append("         Mov_Dettagli_Riferimenti.Cau_Mov_Rif, Mov_Dettagli_Riferimenti.Qta,  Isnull(Mov_Dettagli_Riferimenti.Preserva_Legame,0) as Preserva_Legame " & vbCrLf)
            stb.Append("         ,Mov_Dettagli_Riferimenti.Validita_Inizio AS Validita_Inizio_Tabella, Mov_Dettagli_Riferimenti.Validita_Fine AS Validita_Fine_Tabella " & vbCrLf)
            stb.Append("         ,ISNULL(Mov_Dettagli_Riferimenti.Tipo_Associazione, 0) AS Tipo_Associazione " & vbCrLf)
            stb.Append("         ,Imprese.rag_soc, Imprese_1.rag_soc AS rag_soc_rif " & vbCrLf)

            If conRaccoglitore Then
                stb.Append("         ,ag1.Raccoglitore_Cod, ag2.Raccoglitore_Cod as Raccoglitore_Cod_Rif " & vbCrLf)
            End If

            stb.Append("  FROM   Mov_Dettagli_Riferimenti " & vbCrLf)

            'stb.Append( " INNER JOIN Agenda "
            'stb.Append( " ON      Mov_Dettagli_Riferimenti.Piva = Agenda.Piva " & vbCrLf)
            'stb.Append( " AND     Mov_Dettagli_Riferimenti.Id_Agenda = Agenda.Id_Agenda" & vbCrLf)
            '------->
            stb.Append(" INNER JOIN Agenda Ag2 " & vbCrLf)
            stb.Append(" ON      Mov_Dettagli_Riferimenti.Piva_Rif = Ag2.Piva " & vbCrLf)
            stb.Append(" AND     Mov_Dettagli_Riferimenti.Id_Agenda_Rif = Ag2.Id_Agenda " & vbCrLf)
            'l'operazione riferita è nella parte normale
            stb.Append(" INNER JOIN Agenda Ag1 " & vbCrLf)
            stb.Append(" ON      Mov_Dettagli_Riferimenti.Piva = Ag1.Piva " & vbCrLf)
            stb.Append(" AND     Mov_Dettagli_Riferimenti.Id_Agenda = Ag1.Id_Agenda " & vbCrLf)

            stb.Append(" INNER JOIN Imprese ON Mov_Dettagli_Riferimenti.Piva = Imprese.PIVA INNER JOIN " & vbCrLf)
            stb.Append(" Imprese AS Imprese_1 ON Mov_Dettagli_Riferimenti.Piva_Rif = Imprese_1.PIVA " & vbCrLf)

            'CONDIZIONI
            stb.Append(" WHERE   Mov_Dettagli_Riferimenti.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " & vbCrLf)
            stb.Append(" AND     Mov_Dettagli_Riferimenti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " & vbCrLf)

            If Piva <> "" Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Piva_Rif = '" & Agro_SQL_SaveText(Piva) & "'   " & vbCrLf)
            End If

            'If Sa_Cod <> 0 Then
            '    stb.Append( " AND Mov_Dettagli_Riferimenti.Sa_Cod_Rif = " & Agro_SQL_savenum(Sa_Cod) & "   " & vbCrLf)
            'End If

            If Id_Agenda <> 0 Then
                stb.Append(" AND (Mov_Dettagli_Riferimenti.Id_Agenda_Rif = " & Agro_SQL_SaveNum(Id_Agenda) & "   " & vbCrLf)
                If conRaccoglitore Then
                    stb.Append(" OR Mov_Dettagli_Riferimenti.Id_Agenda_Rif IN (SELECT DISTINCT Id_Agenda FROM Agenda WHERE ISNULL(Raccoglitore_Cod, 0) = ISNULL((SELECT CASE WHEN ISNULL(Raccoglitore_Cod, 0) > 0 THEN Raccoglitore_Cod ELSE -999 END FROM Agenda WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' AND Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "),-999))")
                End If
                stb.Append(" ) " & "   " & vbCrLf)
            End If


            If Id_Mov <> 0 Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Id_Mov_Rif = " & Agro_SQL_SaveNum(Id_Mov) & "   " & vbCrLf)
            End If

            If Id_Mov_Det <> 0 Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Id_Mov_Det_Rif = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   " & vbCrLf)
            End If

            If Lav_Cod <> 0 Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Lav_Cod_Rif = " & Agro_SQL_SaveNum(Lav_Cod) & "   " & vbCrLf)
            End If

            If Cau_Mov <> "" Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Cau_Mov_Rif = '" & Agro_SQL_SaveText(Cau_Mov) & "'   " & vbCrLf)
            End If

            stb.Append(" )  ")

            If xOrderBy <> "" Then
                stb.Append(Agro_SQL_Save_xOrderBy(xOrderBy, objParametri) & vbCrLf)
            Else
                stb.Append(" ORDER BY Mov_Dettagli_Riferimenti.Piva ASC  " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            DT = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try


        Return DT


    End Function

    '#############################################################################################
    'Cerca un'operazione e la sua riferita, in tutti e due i versi
    'Optional ByVal Piva_Op1 As String = "", _
    '                                                                    Optional ByVal Sa_Cod_Op1 As Integer = 0, _
    '                                                                    Optional ByVal Id_Agenda_Op1 As Integer = 0, _
    '                                                                    Optional ByVal Id_Mov_Op1 As Integer = 0, _
    '                                                                    Optional ByVal Id_Mov_Det_Op1 As Integer = 0, _
    '                                                                    Optional ByVal Lav_Cod_Op1 As Integer = 0, _
    '                                                                    Optional ByVal Cau_Mov_Op1 As String = "", _
    '                                                                    Optional ByVal FiltroAggiuntivo_PrimaParte As String = "", _
    '                                                                    Optional ByVal Piva_Op2 As String = "", _
    '                                                                    Optional ByVal Sa_Cod_Op2 As Integer = 0, _
    '                                                                    Optional ByVal Id_Agenda_Op2 As Integer = 0, _
    '                                                                    Optional ByVal Id_Mov_Op2 As Integer = 0, _
    '                                                                    Optional ByVal Id_Mov_Det_Op2 As Integer = 0, _
    '                                                                    Optional ByVal Lav_Cod_Op2 As Integer = 0, _
    '                                                                    Optional ByVal Cau_Mov_Op2 As String = "", _
    '                                                                    Optional ByVal FiltroAggiuntivo_SecondaParte As String = "", _
    '                                                                    Optional ByVal Ordinamento As String = "" 
    Public Function MovimentiRiferimenti_LeggiBilaterale_Completa(ByVal Piva_Op1 As String,
                                                                         ByVal Sa_Cod_Op1 As Integer,
                                                                         ByVal Id_Agenda_Op1 As Integer,
                                                                         ByVal Id_Mov_Op1 As Integer,
                                                                         ByVal Id_Mov_Det_Op1 As Integer,
                                                                         ByVal Lav_Cod_Op1 As Integer,
                                                                         ByVal Cau_Mov_Op1 As String,
                                                                         ByVal FiltroAggiuntivo_PrimaParte As String,
                                                                         ByVal Piva_Op2 As String,
                                                                         ByVal Sa_Cod_Op2 As Integer,
                                                                         ByVal Id_Agenda_Op2 As Integer,
                                                                         ByVal Id_Mov_Op2 As Integer,
                                                                         ByVal Id_Mov_Det_Op2 As Integer,
                                                                         ByVal Lav_Cod_Op2 As Integer,
                                                                         ByVal Cau_Mov_Op2 As String,
                                                                         ByVal FiltroAggiuntivo_SecondaParte As String,
                                                                        ByVal xOrderBy As String,
                                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                        ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R.MovimentiRiferimenti_LeggiBilaterale_Completa()"

        Dim messaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '----------------------------------------------------
            '--- Preparo la Query SQL ---------------------------
            '----------------------------------------------------

            stb.Length = 0
            stb.Append(" ( " & vbCrLf)
            stb.Append(" SELECT  Mov_Dettagli_Riferimenti.Piva, Mov_Dettagli_Riferimenti.Sa_Cod, Mov_Dettagli_Riferimenti.Id_Agenda,   " & vbCrLf)
            stb.Append("         Mov_Dettagli_Riferimenti.Id_Mov, Mov_Dettagli_Riferimenti.Id_Mov_Det, Mov_Dettagli_Riferimenti.Lav_Cod, Mov_Dettagli_Riferimenti.Cau_Mov, " & vbCrLf)
            stb.Append("         Mov_Dettagli_Riferimenti.Piva_Rif, Mov_Dettagli_Riferimenti.Sa_Cod_Rif, Mov_Dettagli_Riferimenti.Id_Agenda_Rif, " & vbCrLf)
            stb.Append("         Mov_Dettagli_Riferimenti.Id_Mov_Rif, Mov_Dettagli_Riferimenti.Id_Mov_Det_Rif, Mov_Dettagli_Riferimenti.Lav_Cod_Rif,  Mov_Dettagli_Riferimenti.Cau_Mov_Rif,   " & vbCrLf)
            stb.Append("         Mov_Dettagli_Riferimenti.Qta, Mov_Dettagli_Riferimenti.Validita_Inizio, Mov_Dettagli_Riferimenti.Validita_Fine, " & vbCrLf)
            stb.Append("         ISNULL(Mov_Dettagli_Riferimenti.Preserva_Legame, 0) AS Preserva_Legame, " & vbCrLf)
            stb.Append("         ISNULL(Mov_Dettagli_Riferimenti.Tipo_Associazione, 0) AS Tipo_Associazione " & vbCrLf)

            stb.Append("  FROM   Mov_Dettagli_Riferimenti " & vbCrLf)

            stb.Append(" WHERE   Mov_Dettagli_Riferimenti.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " & vbCrLf)
            stb.Append(" AND     Mov_Dettagli_Riferimenti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " & vbCrLf)

            If Piva_Op1 <> "" Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Piva = '" & Agro_SQL_SaveText(Piva_Op1) & "'   " & vbCrLf)
            End If

            'pericoloso, fare attenzione
            If Sa_Cod_Op1 <> 0 Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod_Op1) & "   " & vbCrLf)
            End If

            If Id_Agenda_Op1 <> 0 Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda_Op1) & "   " & vbCrLf)
            End If

            If Id_Mov_Op1 <> 0 Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov_Op1) & "   " & vbCrLf)
            End If

            If Id_Mov_Det_Op1 <> 0 Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det_Op1) & "   " & vbCrLf)
            End If

            If Lav_Cod_Op1 <> 0 Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod_Op1) & "   " & vbCrLf)
            End If

            If Cau_Mov_Op1 <> "" Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Cau_Mov = '" & Agro_SQL_SaveText(Cau_Mov_Op1) & "'   " & vbCrLf)
            End If

            If Piva_Op2 <> "" Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Piva_Rif = '" & Agro_SQL_SaveText(Piva_Op2) & "'   " & vbCrLf)
            End If

            'pericoloso, fare attenzione
            If Sa_Cod_Op2 <> 0 Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Sa_Cod_Rif = " & Agro_SQL_SaveNum(Sa_Cod_Op2) & "   " & vbCrLf)
            End If

            If Id_Agenda_Op2 <> 0 Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Id_Agenda_Rif = " & Agro_SQL_SaveNum(Id_Agenda_Op2) & "   " & vbCrLf)
            End If

            If Id_Mov_Op2 <> 0 Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Id_Mov_Rif = " & Agro_SQL_SaveNum(Id_Mov_Op2) & "   " & vbCrLf)
            End If

            If Id_Mov_Det_Op2 <> 0 Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Id_Mov_Det_Rif = " & Agro_SQL_SaveNum(Id_Mov_Det_Op2) & "   " & vbCrLf)
            End If

            If Lav_Cod_Op2 <> 0 Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Lav_Cod_Rif = " & Agro_SQL_SaveNum(Lav_Cod_Op2) & "   " & vbCrLf)
            End If

            If Cau_Mov_Op2 <> "" Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Cau_Mov_Rif = '" & Agro_SQL_SaveText(Cau_Mov_Op2) & "'   " & vbCrLf)
            End If

            If FiltroAggiuntivo_PrimaParte <> "" Then
                stb.Append(FiltroAggiuntivo_PrimaParte & vbCrLf)
            End If


            stb.Append(" )  " & vbCrLf)
            stb.Append(" UNION " & vbCrLf)
            stb.Append(" ( " & vbCrLf)

            stb.Append(" SELECT  Mov_Dettagli_Riferimenti.Piva, Mov_Dettagli_Riferimenti.Sa_Cod, Mov_Dettagli_Riferimenti.Id_Agenda,   " & vbCrLf)
            stb.Append("         Mov_Dettagli_Riferimenti.Id_Mov, Mov_Dettagli_Riferimenti.Id_Mov_Det, Mov_Dettagli_Riferimenti.Lav_Cod, Mov_Dettagli_Riferimenti.Cau_Mov, " & vbCrLf)
            stb.Append("         Mov_Dettagli_Riferimenti.Piva_Rif, Mov_Dettagli_Riferimenti.Sa_Cod_Rif, Mov_Dettagli_Riferimenti.Id_Agenda_Rif, " & vbCrLf)
            stb.Append("         Mov_Dettagli_Riferimenti.Id_Mov_Rif, Mov_Dettagli_Riferimenti.Id_Mov_Det_Rif, Mov_Dettagli_Riferimenti.Lav_Cod_Rif,  Mov_Dettagli_Riferimenti.Cau_Mov_Rif,   " & vbCrLf)
            stb.Append("         Mov_Dettagli_Riferimenti.Qta, Mov_Dettagli_Riferimenti.Validita_Inizio, Mov_Dettagli_Riferimenti.Validita_Fine, " & vbCrLf)
            stb.Append("         ISNULL(Mov_Dettagli_Riferimenti.Preserva_Legame, 0) AS Preserva_Legame, " & vbCrLf)
            stb.Append("         ISNULL(Mov_Dettagli_Riferimenti.Tipo_Associazione, 0) AS Tipo_Associazione " & vbCrLf)

            stb.Append("  FROM   Mov_Dettagli_Riferimenti " & vbCrLf)

            'CONDIZIONI
            stb.Append(" WHERE   Mov_Dettagli_Riferimenti.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " & vbCrLf)
            stb.Append(" AND     Mov_Dettagli_Riferimenti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " & vbCrLf)

            If Piva_Op2 <> "" Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Piva = '" & Agro_SQL_SaveText(Piva_Op2) & "'   " & vbCrLf)
            End If

            'pericoloso, fare attenzione
            If Sa_Cod_Op2 <> 0 Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod_Op2) & "   " & vbCrLf)
            End If

            If Id_Agenda_Op2 <> 0 Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda_Op2) & "   " & vbCrLf)
            End If

            If Id_Mov_Op2 <> 0 Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov_Op2) & "   " & vbCrLf)
            End If

            If Id_Mov_Det_Op2 <> 0 Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det_Op2) & "   " & vbCrLf)
            End If

            If Lav_Cod_Op2 <> 0 Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod_Op2) & "   " & vbCrLf)
            End If

            If Cau_Mov_Op2 <> "" Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Cau_Mov = '" & Agro_SQL_SaveText(Cau_Mov_Op2) & "'   " & vbCrLf)
            End If

            If Piva_Op1 <> "" Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Piva_Rif = '" & Agro_SQL_SaveText(Piva_Op1) & "'   " & vbCrLf)
            End If

            'pericoloso, fare attenzione
            If Sa_Cod_Op1 <> 0 Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Sa_Cod_Rif = " & Agro_SQL_SaveNum(Sa_Cod_Op1) & "   " & vbCrLf)
            End If

            If Id_Agenda_Op1 <> 0 Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Id_Agenda_Rif = " & Agro_SQL_SaveNum(Id_Agenda_Op1) & "   " & vbCrLf)
            End If

            If Id_Mov_Op1 <> 0 Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Id_Mov_Rif = " & Agro_SQL_SaveNum(Id_Mov_Op1) & "   " & vbCrLf)
            End If

            If Id_Mov_Det_Op1 <> 0 Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Id_Mov_Det_Rif = " & Agro_SQL_SaveNum(Id_Mov_Det_Op1) & "   " & vbCrLf)
            End If

            If Lav_Cod_Op1 <> 0 Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Lav_Cod_Rif = " & Agro_SQL_SaveNum(Lav_Cod_Op1) & "   " & vbCrLf)
            End If

            If Cau_Mov_Op1 <> "" Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Cau_Mov_Rif = '" & Agro_SQL_SaveText(Cau_Mov_Op1) & "'   " & vbCrLf)
            End If

            If FiltroAggiuntivo_SecondaParte <> "" Then
                stb.Append(FiltroAggiuntivo_SecondaParte & vbCrLf)
            End If

            stb.Append(" )  " & vbCrLf)

            If xOrderBy <> "" Then
                stb.Append(Agro_SQL_Save_xOrderBy(xOrderBy, objParametri) & vbCrLf)
            Else
                stb.Append(" ORDER BY Mov_Dettagli_Riferimenti.Piva, Mov_Dettagli_Riferimenti.Piva_Rif ASC  " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            DT = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return DT

    End Function


    '################################################################################
    'Chiama NewCom_MovimentiRiferimenti_LeggiBilaterale che
    'corrisponde a Agro_Contab_AD.Mov_Det_Riferimenti_R.LeggiRiferimenti.
    'Serve per sapere se l'operazione passata, è riferita, in generale, a un'altra operazione
    'Optional ByVal Id_Mov As Integer = 0, _
    '                                      Optional ByVal Id_Mov_Det As Integer = 0, _
    '                                      Optional ByVal Lav_Cod As Integer = 0, _
    '                                      Optional ByVal Cau_Mov As String = "")
    Public Function Recupera_DT_Rif_Unificato(ByVal Piva As String,
                                              ByVal Sa_Cod As Integer,
                                              ByVal Id_Agenda As Integer,
                                              ByVal Id_Mov As Integer,
                                              ByVal Id_Mov_Det As Integer,
                                              ByVal Lav_Cod As Integer,
                                              ByVal Cau_Mov As String,
                                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                              Optional ByVal conRaccoglitore As Boolean = False
                                              ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R.Recupera_DT_Rif_Unificato()"
        Dim DT_Rif_Unificato As New DataTable

        Try

            Dim DR_Rif_Unificato As DataRow

            Dim DT_Rif As DataTable = MovimentiRiferimenti_LeggiBilaterale(Piva, Sa_Cod, Id_Agenda, Id_Mov, Id_Mov_Det,
                                                                           Lav_Cod, Cau_Mov, "", objParametri, conRaccoglitore)

            If DT_Rif.Rows.Count <> 0 Then

                '----- Definisco la struttura del DataTable

                DT_Rif_Unificato.Columns.Add(New DataColumn("Piva_Risultato", GetType(String)))
                DT_Rif_Unificato.Columns.Add(New DataColumn("Sa_Cod_Risultato", GetType(Integer)))
                DT_Rif_Unificato.Columns.Add(New DataColumn("Id_Agenda_Risultato", GetType(Integer)))
                DT_Rif_Unificato.Columns.Add(New DataColumn("Id_Mov_Risultato", GetType(Integer)))
                DT_Rif_Unificato.Columns.Add(New DataColumn("Id_Mov_Det_Risultato", GetType(Integer)))
                DT_Rif_Unificato.Columns.Add(New DataColumn("Lav_Cod_Risultato", GetType(Integer)))
                DT_Rif_Unificato.Columns.Add(New DataColumn("Cau_Mov_Risultato", GetType(String)))
                DT_Rif_Unificato.Columns.Add(New DataColumn("Des_Lib_Risultato", GetType(String)))
                DT_Rif_Unificato.Columns.Add(New DataColumn("Validita_Inizio_Risultato", GetType(String)))

                DT_Rif_Unificato.Columns.Add(New DataColumn("Qta_Risultato", GetType(Integer)))

                DT_Rif_Unificato.Columns.Add(New DataColumn("Piva", GetType(String)))
                DT_Rif_Unificato.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
                DT_Rif_Unificato.Columns.Add(New DataColumn("Id_Agenda", GetType(Integer)))
                DT_Rif_Unificato.Columns.Add(New DataColumn("Id_Mov", GetType(Integer)))
                DT_Rif_Unificato.Columns.Add(New DataColumn("Id_Mov_Det", GetType(Integer)))
                DT_Rif_Unificato.Columns.Add(New DataColumn("Lav_Cod", GetType(Integer)))
                DT_Rif_Unificato.Columns.Add(New DataColumn("Cau_Mov", GetType(String)))
                DT_Rif_Unificato.Columns.Add(New DataColumn("Des_Lib", GetType(String)))
                DT_Rif_Unificato.Columns.Add(New DataColumn("Validita_Inizio", GetType(String)))

                DT_Rif_Unificato.Columns.Add(New DataColumn("Rag_Soc_Risultato", GetType(String)))
                DT_Rif_Unificato.Columns.Add(New DataColumn("Rag_Soc", GetType(String)))
                DT_Rif_Unificato.Columns.Add(New DataColumn("Preserva_Legame", GetType(Integer)))
                DT_Rif_Unificato.Columns.Add(New DataColumn("Tipo_Associazione", GetType(Integer)))

                Dim raccoglitoreList As New List(Of Integer)

                For i As Integer = 0 To DT_Rif.Rows.Count - 1

                    If Piva = DT_Rif.Rows(i).Item("Piva") And Id_Agenda = DT_Rif.Rows(i).Item("Id_Agenda") Then

                        If conRaccoglitore Then
                            If DT_Rif.Rows(0).Item("Raccoglitore_Cod") IsNot DBNull.Value Then
                                raccoglitoreList.Add(DT_Rif.Rows(0).Item("Raccoglitore_Cod"))
                            End If
                        End If

                        DR_Rif_Unificato = DT_Rif_Unificato.NewRow

                        DR_Rif_Unificato.Item("Rag_Soc_Risultato") = DT_Rif.Rows(i).Item("Rag_Soc_Rif")
                        DR_Rif_Unificato.Item("Piva_Risultato") = DT_Rif.Rows(i).Item("Piva_Rif")
                        DR_Rif_Unificato.Item("Sa_Cod_Risultato") = DT_Rif.Rows(i).Item("Sa_Cod_Rif")
                        DR_Rif_Unificato.Item("Id_Agenda_Risultato") = DT_Rif.Rows(i).Item("Id_Agenda_Rif")
                        DR_Rif_Unificato.Item("Id_Mov_Risultato") = DT_Rif.Rows(i).Item("Id_Mov_Rif")
                        DR_Rif_Unificato.Item("Id_Mov_Det_Risultato") = DT_Rif.Rows(i).Item("Id_Mov_Det_Rif")
                        DR_Rif_Unificato.Item("Lav_Cod_Risultato") = DT_Rif.Rows(i).Item("Lav_Cod_Rif")
                        DR_Rif_Unificato.Item("Cau_Mov_Risultato") = DT_Rif.Rows(i).Item("Cau_Mov_Rif")
                        DR_Rif_Unificato.Item("Des_Lib_Risultato") = DT_Rif.Rows(i).Item("Des_Lib_Rif")
                        DR_Rif_Unificato.Item("Validita_Inizio_Risultato") = DT_Rif.Rows(i).Item("Validita_Inizio_Rif")

                        DR_Rif_Unificato.Item("Qta_Risultato") = DT_Rif.Rows(i).Item("Qta")
                        DR_Rif_Unificato.Item("Preserva_Legame") = DT_Rif.Rows(i).Item("Preserva_Legame")
                        DR_Rif_Unificato.Item("Tipo_Associazione") = DT_Rif.Rows(i).Item("Tipo_Associazione")

                        DR_Rif_Unificato.Item("Rag_Soc") = DT_Rif.Rows(i).Item("Rag_Soc")
                        DR_Rif_Unificato.Item("Piva") = DT_Rif.Rows(i).Item("Piva")
                        DR_Rif_Unificato.Item("Sa_Cod") = DT_Rif.Rows(i).Item("Sa_Cod")
                        DR_Rif_Unificato.Item("Id_Agenda") = DT_Rif.Rows(i).Item("Id_Agenda")
                        DR_Rif_Unificato.Item("Id_Mov") = DT_Rif.Rows(i).Item("Id_Mov")
                        DR_Rif_Unificato.Item("Id_Mov_Det") = DT_Rif.Rows(i).Item("Id_Mov_Det")
                        DR_Rif_Unificato.Item("Lav_Cod") = DT_Rif.Rows(i).Item("Lav_Cod")
                        DR_Rif_Unificato.Item("Cau_Mov") = DT_Rif.Rows(i).Item("Cau_Mov")
                        DR_Rif_Unificato.Item("Des_Lib") = DT_Rif.Rows(i).Item("Des_Lib")
                        DR_Rif_Unificato.Item("Validita_Inizio") = DT_Rif.Rows(i).Item("Validita_Inizio")

                        DT_Rif_Unificato.Rows.Add(DR_Rif_Unificato)

                    ElseIf Piva = DT_Rif.Rows(i).Item("Piva_Rif") And Id_Agenda = DT_Rif.Rows(i).Item("Id_Agenda_Rif") Then

                        If conRaccoglitore Then
                            If DT_Rif.Rows(0).Item("Raccoglitore_Cod_Rif") IsNot DBNull.Value Then
                                raccoglitoreList.Add(DT_Rif.Rows(0).Item("Raccoglitore_Cod_Rif"))
                            End If
                        End If

                        DR_Rif_Unificato = DT_Rif_Unificato.NewRow

                        DR_Rif_Unificato.Item("Rag_Soc_Risultato") = DT_Rif.Rows(i).Item("Rag_Soc")
                        DR_Rif_Unificato.Item("Piva_Risultato") = DT_Rif.Rows(i).Item("Piva")
                        DR_Rif_Unificato.Item("Sa_Cod_Risultato") = DT_Rif.Rows(i).Item("Sa_Cod")
                        DR_Rif_Unificato.Item("Id_Agenda_Risultato") = DT_Rif.Rows(i).Item("Id_Agenda")
                        DR_Rif_Unificato.Item("Id_Mov_Risultato") = DT_Rif.Rows(i).Item("Id_Mov")
                        DR_Rif_Unificato.Item("Id_Mov_Det_Risultato") = DT_Rif.Rows(i).Item("Id_Mov_Det")
                        DR_Rif_Unificato.Item("Lav_Cod_Risultato") = DT_Rif.Rows(i).Item("Lav_Cod")
                        DR_Rif_Unificato.Item("Cau_Mov_Risultato") = DT_Rif.Rows(i).Item("Cau_Mov")
                        DR_Rif_Unificato.Item("Des_Lib_Risultato") = DT_Rif.Rows(i).Item("Des_Lib")
                        DR_Rif_Unificato.Item("Validita_Inizio_Risultato") = DT_Rif.Rows(i).Item("Validita_Inizio")

                        DR_Rif_Unificato.Item("Qta_Risultato") = DT_Rif.Rows(i).Item("Qta")
                        DR_Rif_Unificato.Item("Preserva_Legame") = DT_Rif.Rows(i).Item("Preserva_Legame")
                        DR_Rif_Unificato.Item("Tipo_Associazione") = DT_Rif.Rows(i).Item("Tipo_Associazione")

                        DR_Rif_Unificato.Item("Rag_Soc") = DT_Rif.Rows(i).Item("Rag_Soc_Rif")
                        DR_Rif_Unificato.Item("Piva") = DT_Rif.Rows(i).Item("Piva_Rif")
                        DR_Rif_Unificato.Item("Sa_Cod") = DT_Rif.Rows(i).Item("Sa_Cod_Rif")
                        DR_Rif_Unificato.Item("Id_Agenda") = DT_Rif.Rows(i).Item("Id_Agenda_Rif")
                        DR_Rif_Unificato.Item("Id_Mov") = DT_Rif.Rows(i).Item("Id_Mov_Rif")
                        DR_Rif_Unificato.Item("Id_Mov_Det") = DT_Rif.Rows(i).Item("Id_Mov_Det_Rif")
                        DR_Rif_Unificato.Item("Lav_Cod") = DT_Rif.Rows(i).Item("Lav_Cod_Rif")
                        DR_Rif_Unificato.Item("Cau_Mov") = DT_Rif.Rows(i).Item("Cau_Mov_Rif")
                        DR_Rif_Unificato.Item("Des_Lib") = DT_Rif.Rows(i).Item("Des_Lib_Rif")
                        DR_Rif_Unificato.Item("Validita_Inizio") = DT_Rif.Rows(i).Item("Validita_Inizio_Rif")


                        DT_Rif_Unificato.Rows.Add(DR_Rif_Unificato)

                    End If

                Next

                If conRaccoglitore Then
                    For i As Integer = 0 To DT_Rif.Rows.Count - 1

                        If raccoglitoreList.Contains(DT_Rif.Rows(i).Item("Raccoglitore_Cod")) Then

                            DR_Rif_Unificato = DT_Rif_Unificato.NewRow

                            DR_Rif_Unificato.Item("Rag_Soc_Risultato") = DT_Rif.Rows(i).Item("Rag_Soc_Rif")
                            DR_Rif_Unificato.Item("Piva_Risultato") = DT_Rif.Rows(i).Item("Piva_Rif")
                            DR_Rif_Unificato.Item("Sa_Cod_Risultato") = DT_Rif.Rows(i).Item("Sa_Cod_Rif")
                            DR_Rif_Unificato.Item("Id_Agenda_Risultato") = DT_Rif.Rows(i).Item("Id_Agenda_Rif")
                            DR_Rif_Unificato.Item("Id_Mov_Risultato") = DT_Rif.Rows(i).Item("Id_Mov_Rif")
                            DR_Rif_Unificato.Item("Id_Mov_Det_Risultato") = DT_Rif.Rows(i).Item("Id_Mov_Det_Rif")
                            DR_Rif_Unificato.Item("Lav_Cod_Risultato") = DT_Rif.Rows(i).Item("Lav_Cod_Rif")
                            DR_Rif_Unificato.Item("Cau_Mov_Risultato") = DT_Rif.Rows(i).Item("Cau_Mov_Rif")
                            DR_Rif_Unificato.Item("Des_Lib_Risultato") = DT_Rif.Rows(i).Item("Des_Lib_Rif")
                            DR_Rif_Unificato.Item("Validita_Inizio_Risultato") = DT_Rif.Rows(i).Item("Validita_Inizio_Rif")

                            DR_Rif_Unificato.Item("Qta_Risultato") = DT_Rif.Rows(i).Item("Qta")
                            DR_Rif_Unificato.Item("Preserva_Legame") = DT_Rif.Rows(i).Item("Preserva_Legame")
                            DR_Rif_Unificato.Item("Tipo_Associazione") = DT_Rif.Rows(i).Item("Tipo_Associazione")

                            DR_Rif_Unificato.Item("Rag_Soc") = DT_Rif.Rows(i).Item("Rag_Soc")
                            DR_Rif_Unificato.Item("Piva") = DT_Rif.Rows(i).Item("Piva")
                            DR_Rif_Unificato.Item("Sa_Cod") = DT_Rif.Rows(i).Item("Sa_Cod")
                            DR_Rif_Unificato.Item("Id_Agenda") = DT_Rif.Rows(i).Item("Id_Agenda")
                            DR_Rif_Unificato.Item("Id_Mov") = DT_Rif.Rows(i).Item("Id_Mov")
                            DR_Rif_Unificato.Item("Id_Mov_Det") = DT_Rif.Rows(i).Item("Id_Mov_Det")
                            DR_Rif_Unificato.Item("Lav_Cod") = DT_Rif.Rows(i).Item("Lav_Cod")
                            DR_Rif_Unificato.Item("Cau_Mov") = DT_Rif.Rows(i).Item("Cau_Mov")
                            DR_Rif_Unificato.Item("Des_Lib") = DT_Rif.Rows(i).Item("Des_Lib")
                            DR_Rif_Unificato.Item("Validita_Inizio") = DT_Rif.Rows(i).Item("Validita_Inizio")

                            DT_Rif_Unificato.Rows.Add(DR_Rif_Unificato)
                        End If

                        If raccoglitoreList.Contains(DT_Rif.Rows(i).Item("Raccoglitore_Cod_Rif")) Then

                            DR_Rif_Unificato = DT_Rif_Unificato.NewRow

                            DR_Rif_Unificato.Item("Rag_Soc_Risultato") = DT_Rif.Rows(i).Item("Rag_Soc")
                            DR_Rif_Unificato.Item("Piva_Risultato") = DT_Rif.Rows(i).Item("Piva")
                            DR_Rif_Unificato.Item("Sa_Cod_Risultato") = DT_Rif.Rows(i).Item("Sa_Cod")
                            DR_Rif_Unificato.Item("Id_Agenda_Risultato") = DT_Rif.Rows(i).Item("Id_Agenda")
                            DR_Rif_Unificato.Item("Id_Mov_Risultato") = DT_Rif.Rows(i).Item("Id_Mov")
                            DR_Rif_Unificato.Item("Id_Mov_Det_Risultato") = DT_Rif.Rows(i).Item("Id_Mov_Det")
                            DR_Rif_Unificato.Item("Lav_Cod_Risultato") = DT_Rif.Rows(i).Item("Lav_Cod")
                            DR_Rif_Unificato.Item("Cau_Mov_Risultato") = DT_Rif.Rows(i).Item("Cau_Mov")
                            DR_Rif_Unificato.Item("Des_Lib_Risultato") = DT_Rif.Rows(i).Item("Des_Lib")
                            DR_Rif_Unificato.Item("Validita_Inizio_Risultato") = DT_Rif.Rows(i).Item("Validita_Inizio")

                            DR_Rif_Unificato.Item("Qta_Risultato") = DT_Rif.Rows(i).Item("Qta")
                            DR_Rif_Unificato.Item("Preserva_Legame") = DT_Rif.Rows(i).Item("Preserva_Legame")
                            DR_Rif_Unificato.Item("Tipo_Associazione") = DT_Rif.Rows(i).Item("Tipo_Associazione")

                            DR_Rif_Unificato.Item("Rag_Soc") = DT_Rif.Rows(i).Item("Rag_Soc_Rif")
                            DR_Rif_Unificato.Item("Piva") = DT_Rif.Rows(i).Item("Piva_Rif")
                            DR_Rif_Unificato.Item("Sa_Cod") = DT_Rif.Rows(i).Item("Sa_Cod_Rif")
                            DR_Rif_Unificato.Item("Id_Agenda") = DT_Rif.Rows(i).Item("Id_Agenda_Rif")
                            DR_Rif_Unificato.Item("Id_Mov") = DT_Rif.Rows(i).Item("Id_Mov_Rif")
                            DR_Rif_Unificato.Item("Id_Mov_Det") = DT_Rif.Rows(i).Item("Id_Mov_Det_Rif")
                            DR_Rif_Unificato.Item("Lav_Cod") = DT_Rif.Rows(i).Item("Lav_Cod_Rif")
                            DR_Rif_Unificato.Item("Cau_Mov") = DT_Rif.Rows(i).Item("Cau_Mov_Rif")
                            DR_Rif_Unificato.Item("Des_Lib") = DT_Rif.Rows(i).Item("Des_Lib_Rif")
                            DR_Rif_Unificato.Item("Validita_Inizio") = DT_Rif.Rows(i).Item("Validita_Inizio_Rif")

                        End If
                    Next
                End If
            End If
        Catch ex As Exception
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        End Try

        Return DT_Rif_Unificato

    End Function

    '################################################################################
    'Chiama NewCom_MovimentiRiferimenti_LeggiBilaterale_Completa 
    'Optional ByVal Piva_Op1 As String = "", _
    '                                            Optional ByVal Sa_Cod_Op1 As Integer = 0, _
    '                                            Optional ByVal Id_Agenda_Op1 As Integer = 0, _
    '                                            Optional ByVal Id_Mov_Op1 As Integer = 0, _
    '                                            Optional ByVal Id_Mov_Det_Op1 As Integer = 0, _
    '                                            Optional ByVal Lav_Cod_Op1 As Integer = 0, _
    '                                            Optional ByVal Cau_Mov_Op1 As String = "", _
    '                                            Optional ByVal FiltroAggiuntivo_PrimaParte As String = "", _
    '                                            Optional ByVal Piva_Op2 As String = "", _
    '                                            Optional ByVal Sa_Cod_Op2 As Integer = 0, _
    '                                            Optional ByVal Id_Agenda_Op2 As Integer = 0, _
    '                                            Optional ByVal Id_Mov_Op2 As Integer = 0, _
    '                                            Optional ByVal Id_Mov_Det_Op2 As Integer = 0, _
    '                                            Optional ByVal Lav_Cod_Op2 As Integer = 0, _
    '                                            Optional ByVal Cau_Mov_Op2 As String = "", _
    '                                            Optional ByVal FiltroAggiuntivo_SecondaParte As String = "", _
    '                                            Optional ByVal Ordinamento As String = "", _
    Public Function Recupera_DT_Rif_Unificato_2(ByVal Piva_Op1 As String,
                                                 ByVal Sa_Cod_Op1 As Integer,
                                                 ByVal Id_Agenda_Op1 As Integer,
                                                 ByVal Id_Mov_Op1 As Integer,
                                                 ByVal Id_Mov_Det_Op1 As Integer,
                                                 ByVal Lav_Cod_Op1 As Integer,
                                                 ByVal Cau_Mov_Op1 As String,
                                                 ByVal FiltroAggiuntivo_PrimaParte As String,
                                                 ByVal Piva_Op2 As String,
                                                 ByVal Sa_Cod_Op2 As Integer,
                                                 ByVal Id_Agenda_Op2 As Integer,
                                                 ByVal Id_Mov_Op2 As Integer,
                                                 ByVal Id_Mov_Det_Op2 As Integer,
                                                 ByVal Lav_Cod_Op2 As Integer,
                                                 ByVal Cau_Mov_Op2 As String,
                                                 ByVal FiltroAggiuntivo_SecondaParte As String,
                                                 ByVal Ordinamento As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                              ) As DataTable


        Dim DT_Rif As DataTable
        Dim DT_Rif_Unificato As New DataTable
        Dim DR_Rif_Unificato As DataRow
        Dim i As Integer

        DT_Rif = MovimentiRiferimenti_LeggiBilaterale_Completa(Piva_Op1,
                                                                Sa_Cod_Op1,
                                                                Id_Agenda_Op1,
                                                                Id_Mov_Op1,
                                                                Id_Mov_Det_Op1,
                                                                Lav_Cod_Op1,
                                                                Cau_Mov_Op1,
                                                                FiltroAggiuntivo_PrimaParte,
                                                                Piva_Op2,
                                                                Sa_Cod_Op2,
                                                                Id_Agenda_Op2,
                                                                Id_Mov_Op2,
                                                                Id_Mov_Det_Op2,
                                                                Lav_Cod_Op2,
                                                                Cau_Mov_Op2,
                                                                FiltroAggiuntivo_SecondaParte,
                                                                Ordinamento,
                                                                  objParametri)


        If DT_Rif.Rows.Count <> 0 Then


            '----- Definisco la struttura del DataTable

            DT_Rif_Unificato.Columns.Add(New DataColumn("Piva_Risultato", GetType(String)))
            DT_Rif_Unificato.Columns.Add(New DataColumn("Sa_Cod_Risultato", GetType(Integer)))
            DT_Rif_Unificato.Columns.Add(New DataColumn("Id_Agenda_Risultato", GetType(Integer)))
            DT_Rif_Unificato.Columns.Add(New DataColumn("Id_Mov_Risultato", GetType(Integer)))
            DT_Rif_Unificato.Columns.Add(New DataColumn("Id_Mov_Det_Risultato", GetType(Integer)))
            DT_Rif_Unificato.Columns.Add(New DataColumn("Lav_Cod_Risultato", GetType(Integer)))
            DT_Rif_Unificato.Columns.Add(New DataColumn("Cau_Mov_Risultato", GetType(String)))
            DT_Rif_Unificato.Columns.Add(New DataColumn("Qta_Risultato", GetType(Integer)))
            DT_Rif_Unificato.Columns.Add(New DataColumn("Des_Lib_Risultato", GetType(String)))
            DT_Rif_Unificato.Columns.Add(New DataColumn("Validita_Inizio_Risultato", GetType(String)))

            DT_Rif_Unificato.Columns.Add(New DataColumn("Piva", GetType(String)))
            DT_Rif_Unificato.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
            DT_Rif_Unificato.Columns.Add(New DataColumn("Id_Agenda", GetType(Integer)))
            DT_Rif_Unificato.Columns.Add(New DataColumn("Id_Mov", GetType(Integer)))
            DT_Rif_Unificato.Columns.Add(New DataColumn("Id_Mov_Det", GetType(Integer)))
            DT_Rif_Unificato.Columns.Add(New DataColumn("Lav_Cod", GetType(Integer)))
            DT_Rif_Unificato.Columns.Add(New DataColumn("Cau_Mov", GetType(String)))
            DT_Rif_Unificato.Columns.Add(New DataColumn("Preserva_Legame", GetType(Integer)))
            DT_Rif_Unificato.Columns.Add(New DataColumn("Tipo_Associazione", GetType(Integer)))


            For i = 0 To DT_Rif.Rows.Count - 1


                If Piva_Op1 = DT_Rif.Rows(i).Item("Piva") And Id_Agenda_Op1 = DT_Rif.Rows(i).Item("Id_Agenda") Then

                    DR_Rif_Unificato = DT_Rif_Unificato.NewRow

                    DR_Rif_Unificato.Item("Piva_Risultato") = DT_Rif.Rows(i).Item("Piva_Rif")
                    DR_Rif_Unificato.Item("Sa_Cod_Risultato") = DT_Rif.Rows(i).Item("Sa_Cod_Rif")
                    DR_Rif_Unificato.Item("Id_Agenda_Risultato") = DT_Rif.Rows(i).Item("Id_Agenda_Rif")
                    DR_Rif_Unificato.Item("Id_Mov_Risultato") = DT_Rif.Rows(i).Item("Id_Mov_Rif")
                    DR_Rif_Unificato.Item("Id_Mov_Det_Risultato") = DT_Rif.Rows(i).Item("Id_Mov_Det_Rif")
                    DR_Rif_Unificato.Item("Lav_Cod_Risultato") = DT_Rif.Rows(i).Item("Lav_Cod_Rif")
                    DR_Rif_Unificato.Item("Cau_Mov_Risultato") = DT_Rif.Rows(i).Item("Cau_Mov_Rif")
                    DR_Rif_Unificato.Item("Qta_Risultato") = DT_Rif.Rows(i).Item("Qta")
                    DR_Rif_Unificato.Item("Des_Lib_Risultato") = DT_Rif.Rows(i).Item("Des_Lib")
                    DR_Rif_Unificato.Item("Validita_Inizio_Risultato") = DT_Rif.Rows(i).Item("Validita_Inizio")

                    DR_Rif_Unificato.Item("Piva") = DT_Rif.Rows(i).Item("Piva")
                    DR_Rif_Unificato.Item("Sa_Cod") = DT_Rif.Rows(i).Item("Sa_Cod")
                    DR_Rif_Unificato.Item("Id_Agenda") = DT_Rif.Rows(i).Item("Id_Agenda")
                    DR_Rif_Unificato.Item("Id_Mov") = DT_Rif.Rows(i).Item("Id_Mov")
                    DR_Rif_Unificato.Item("Id_Mov_Det") = DT_Rif.Rows(i).Item("Id_Mov_Det")
                    DR_Rif_Unificato.Item("Lav_Cod") = DT_Rif.Rows(i).Item("Lav_Cod")
                    DR_Rif_Unificato.Item("Cau_Mov") = DT_Rif.Rows(i).Item("Cau_Mov")
                    DR_Rif_Unificato.Item("Preserva_Legame") = DT_Rif.Rows(i).Item("Preserva_Legame")
                    DR_Rif_Unificato.Item("Tipo_Associazione") = DT_Rif.Rows(i).Item("Tipo_Associazione")

                    DT_Rif_Unificato.Rows.Add(DR_Rif_Unificato)


                ElseIf Piva_Op1 = DT_Rif.Rows(i).Item("Piva_Rif") And Id_Agenda_Op1 = DT_Rif.Rows(i).Item("Id_Agenda_Rif") Then

                    DR_Rif_Unificato = DT_Rif_Unificato.NewRow

                    DR_Rif_Unificato.Item("Piva_Risultato") = DT_Rif.Rows(i).Item("Piva")
                    DR_Rif_Unificato.Item("Sa_Cod_Risultato") = DT_Rif.Rows(i).Item("Sa_Cod")
                    DR_Rif_Unificato.Item("Id_Agenda_Risultato") = DT_Rif.Rows(i).Item("Id_Agenda")
                    DR_Rif_Unificato.Item("Id_Mov_Risultato") = DT_Rif.Rows(i).Item("Id_Mov")
                    DR_Rif_Unificato.Item("Id_Mov_Det_Risultato") = DT_Rif.Rows(i).Item("Id_Mov_Det")
                    DR_Rif_Unificato.Item("Lav_Cod_Risultato") = DT_Rif.Rows(i).Item("Lav_Cod")
                    DR_Rif_Unificato.Item("Cau_Mov_Risultato") = DT_Rif.Rows(i).Item("Cau_Mov")
                    DR_Rif_Unificato.Item("Qta_Risultato") = DT_Rif.Rows(i).Item("Qta")
                    DR_Rif_Unificato.Item("Des_Lib_Risultato") = DT_Rif.Rows(i).Item("Des_Lib")
                    DR_Rif_Unificato.Item("Validita_Inizio_Risultato") = DT_Rif.Rows(i).Item("Validita_Inizio")

                    DR_Rif_Unificato.Item("Piva") = DT_Rif.Rows(i).Item("Piva_Rif")
                    DR_Rif_Unificato.Item("Sa_Cod") = DT_Rif.Rows(i).Item("Sa_Cod_Rif")
                    DR_Rif_Unificato.Item("Id_Agenda") = DT_Rif.Rows(i).Item("Id_Agenda_Rif")
                    DR_Rif_Unificato.Item("Id_Mov") = DT_Rif.Rows(i).Item("Id_Mov_Rif")
                    DR_Rif_Unificato.Item("Id_Mov_Det") = DT_Rif.Rows(i).Item("Id_Mov_Det_Rif")
                    DR_Rif_Unificato.Item("Lav_Cod") = DT_Rif.Rows(i).Item("Lav_Cod_Rif")
                    DR_Rif_Unificato.Item("Cau_Mov") = DT_Rif.Rows(i).Item("Cau_Mov_Rif")
                    DR_Rif_Unificato.Item("Preserva_Legame") = DT_Rif.Rows(i).Item("Preserva_Legame")
                    DR_Rif_Unificato.Item("Tipo_Associazione") = DT_Rif.Rows(i).Item("Tipo_Associazione")

                    DT_Rif_Unificato.Rows.Add(DR_Rif_Unificato)

                End If

            Next

        End If


        Return DT_Rif_Unificato


    End Function


    '#########################################################
    Public Function MatriceChiaviAgenda_MovRiferiti(ByVal Piva As String,
                                                    ByVal Sa_Cod As Integer,
                                                    ByVal Id_Agenda As Integer,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                    Optional ByVal conRaccoglitore As Boolean = False
                                                    ) As String(,)

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R.MatriceChiaviAgenda_MovRiferiti()"

        Dim messaggioErrore As String = ""
        Dim DT_Rif As DataTable
        Dim matrix(,) As String
        Dim i As Integer

        Try

            DT_Rif = Recupera_DT_Rif_Unificato(Piva,
                                               Sa_Cod,
                                               Id_Agenda,
                                               0, 0, 0, "",
                                               objParametri,
                                               conRaccoglitore)

            ReDim Preserve matrix(2, DT_Rif.Rows.Count - 1)

            If Not IsNothing(DT_Rif) AndAlso DT_Rif.Rows.Count > 0 Then

                For i = 0 To DT_Rif.Rows.Count - 1

                    matrix(0, i) = DT_Rif.Rows(i).Item("Piva_Risultato")
                    matrix(1, i) = DT_Rif.Rows(i).Item("Sa_Cod_Risultato")
                    matrix(2, i) = DT_Rif.Rows(i).Item("Id_Agenda_Risultato")

                Next

            End If


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return matrix

    End Function

    '################################################################################
    Public Function Verifica_Operazione_ConMagazzinoAltraimpresa(ByVal Piva As String,
                                                                 ByVal Sa_Cod As Integer,
                                                                 ByVal Id_Agenda As Integer,
                                                                 ByVal Lav_Cod As Integer,
                                                                 ByVal xFiltroAggiuntivo As String,
                                                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                 ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Movimenti_R.Verifica_SOperazione_ConMagazzinoAltraimpresa()"

        Dim messaggioErrore As String = ""
        Dim DT_Rif As DataTable
        Dim i As Integer
        Dim Flag As Boolean = False

        Try

            DT_Rif = Recupera_DT_Rif_Unificato(Piva,
                                               Sa_Cod,
                                               Id_Agenda,
                                               0, 0,
                                               Lav_Cod,
                                               "",
                                               objParametri)

            If Not IsNothing(DT_Rif) AndAlso DT_Rif.Rows.Count > 0 Then

                Dim piva_collegata As String
                Dim lav_cod_collegato As Integer

                For i = 0 To DT_Rif.Rows.Count - 1

                    piva_collegata = DT_Rif.Rows(i).Item("Piva_Risultato")
                    lav_cod_collegato = DT_Rif.Rows(i).Item("Lav_Cod_Risultato")

                    '' se l'operazione è collegata a un movimento di scarico di magazzino di un'altra impresa
                    If (lav_cod_collegato = LAVCOD_SCARICO Or lav_cod_collegato = LAVCOD_BOLLA_EMESSA) And piva_collegata <> Piva Then
                        Flag = True
                    End If

                Next

            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return Flag

    End Function


    '################################################################################
    Public Function Recupera_ChiaveMagazzinoAltraImpresa_Operazione(ByVal Piva As String,
                                                                    ByVal Sa_Cod As Integer,
                                                                    ByVal Id_Agenda As Integer,
                                                                    ByVal Lav_Cod As Integer,
                                                                    ByVal xFiltroAggiuntivo As String,
                                                                    ByRef Piva_Rif As String,
                                                                    ByRef Id_Agenda_Rif As Integer,
                                                                    ByRef Sa_Cod_Fabbricato As Integer,
                                                                    ByRef Cod_Fabbricato As Integer,
                                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                    ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Movimenti_R.Recupera_ChiaveMagazzinoAltraImpresa_Operazione()"

        Dim MessaggioErrore As String = ""
        Dim DT_Rif As DataTable
        Dim i As Integer
        Dim Flag As Boolean = False

        Piva_Rif = ""
        Id_Agenda_Rif = 0
        Sa_Cod_Fabbricato = 0
        Cod_Fabbricato = 0

        Try

            ''leggo la semina con agganciata uno scarico di magazzino
            'DT_Rif = Leggi_MovDestinazioniRif(Piva,
            '                                  Sa_Cod,
            '                                  Id_Agenda,
            '                                  0, 0,
            '                                  Lav_Cod,
            '                                  "",
            '                                  "", 0, 0, 0, 0,
            '                                  LAVCOD_SCARICO,
            '                                  "",
            '                                  "", "",
            '                                  objParametri)
            'leggo la semina con agganciata uno scarico di magazzino
            DT_Rif = Leggi_MovDestinazioniRif2(Piva,
                                               Sa_Cod,
                                               Id_Agenda,
                                               0, 0,
                                               Lav_Cod,
                                               "",
                                               "", 0, 0, 0, 0,
                                               0,
                                               "",
                                               "", "",
                                               objParametri)

            If Not IsNothing(DT_Rif) AndAlso DT_Rif.Rows.Count > 0 Then

                'Dim piva_collegata As String
                'Dim lav_cod_collegato As Integer

                For i = 0 To DT_Rif.Rows.Count - 1

                    ' se lo scarico di magazzino è di un'altra impresa
                    If DT_Rif.Rows(i).Item("Piva_Rif") <> Piva Then
                        Flag = True
                        Piva_Rif = DT_Rif.Rows(i).Item("Piva_Rif")
                        Sa_Cod_Fabbricato = DT_Rif.Rows(i).Item("Sa_Cod_Fabbricato")
                        Cod_Fabbricato = DT_Rif.Rows(i).Item("Fabbricato_Cod")
                    End If

                Next

            End If


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Flag

    End Function



    '#############################################################################################
    'si presuppone che id_mov_rif sia valorizzato e quindi viene messo in join con movimenti
    Public Function Leggi_MovDestinazioniRif(ByVal Piva As String,
                                             ByVal Sa_Cod As Integer,
                                             ByVal Id_Agenda As Integer,
                                             ByVal Id_Mov As Integer,
                                             ByVal Id_Mov_Det As Integer,
                                             ByVal Lav_Cod As Integer,
                                             ByVal Cau_Mov As String,
                                             ByVal Piva_Rif As String,
                                             ByVal Sa_Cod_Rif As Integer,
                                             ByVal Id_Agenda_Rif As Integer,
                                             ByVal Id_Mov_Rif As Integer,
                                             ByVal Id_Mov_Det_Rif As Integer,
                                             ByVal Lav_Cod_Rif As Integer,
                                             ByVal Cau_Mov_Rif As String,
                                             ByVal xFiltroAggiuntivo As String,
                                             ByVal xOrderBy As String,
                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                             ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R.LeggiRif_ConMovDestinazioni()"

        Dim messaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '----------------------------------------------------
            '--- Preparo la Query SQL ---------------------------
            '----------------------------------------------------

            stb.Append(" SELECT  Agenda.Des_Lib, Agenda.Validita_Inizio,  " & vbCrLf)
            stb.Append("         Mov_Destinazioni.Sa_Cod AS Sa_Cod_Fabbricato, Mov_Destinazioni.Id_Destinazione AS Fabbricato_Cod, " & vbCrLf)
            stb.Append("          Mov_Dettagli_Riferimenti.Piva, Mov_Dettagli_Riferimenti.Sa_Cod, Mov_Dettagli_Riferimenti.Id_Agenda,   " & vbCrLf)
            stb.Append("         Mov_Dettagli_Riferimenti.Id_Mov, Mov_Dettagli_Riferimenti.Id_Mov_Det, Mov_Dettagli_Riferimenti.Lav_Cod, Mov_Dettagli_Riferimenti.Cau_Mov, " & vbCrLf)
            stb.Append("         Mov_Dettagli_Riferimenti.Piva_Rif, Mov_Dettagli_Riferimenti.Sa_Cod_Rif, Mov_Dettagli_Riferimenti.Id_Agenda_Rif, " & vbCrLf)
            stb.Append("         Mov_Dettagli_Riferimenti.Id_Mov_Rif, Mov_Dettagli_Riferimenti.Id_Mov_Det_Rif, Mov_Dettagli_Riferimenti.Lav_Cod_Rif,  " & vbCrLf)
            stb.Append("         Mov_Dettagli_Riferimenti.Cau_Mov_Rif, Mov_Dettagli_Riferimenti.Qta, Mov_Dettagli_Riferimenti.Validita_Inizio AS Validita_Inizio_Tabella, Mov_Dettagli_Riferimenti.Validita_Fine AS Validita_Fine_Tabella, " & vbCrLf)
            stb.Append("         Mov_Dettagli_Riferimenti.Preserva_Legame, " + vbCrLf)
            stb.Append("         ISNULL(Mov_Dettagli_Riferimenti.Tipo_Associazione, 0) AS Tipo_Associazione " + vbCrLf)

            stb.Append("  FROM   Mov_Dettagli_Riferimenti " & vbCrLf)

            stb.Append(" INNER JOIN Movimenti " & vbCrLf)
            stb.Append(" ON   Mov_Dettagli_Riferimenti.Piva_Rif = Movimenti.Piva " & vbCrLf)
            stb.Append(" AND   Mov_Dettagli_Riferimenti.Id_Agenda_Rif = Movimenti.Id_Agenda " & vbCrLf)
            stb.Append(" AND   Mov_Dettagli_Riferimenti.Id_Mov_Rif = Movimenti.Id_Mov " & vbCrLf)

            stb.Append(" INNER JOIN Agenda " & vbCrLf)
            stb.Append(" ON   Movimenti.Piva = Agenda.Piva " & vbCrLf)
            stb.Append(" AND   Movimenti.Id_Agenda = Agenda.Id_Agenda " & vbCrLf)

            stb.Append(" INNER JOIN Movimenti_Dettagli " & vbCrLf)
            stb.Append(" ON   Movimenti.Piva = Movimenti_Dettagli.Piva " & vbCrLf)
            stb.Append(" AND   Movimenti.Id_Agenda = Movimenti_Dettagli.Id_Agenda " & vbCrLf)
            stb.Append(" AND   Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov " & vbCrLf)

            stb.Append(" INNER JOIN Mov_Destinazioni " & vbCrLf)
            stb.Append(" ON   Mov_Destinazioni.Piva = Movimenti_Dettagli.Piva " & vbCrLf)
            stb.Append(" AND   Mov_Destinazioni.Id_Agenda = Movimenti_Dettagli.Id_Agenda " & vbCrLf)
            stb.Append(" AND   Mov_Destinazioni.Id_Mov = Movimenti_Dettagli.Id_Mov " & vbCrLf)
            stb.Append(" AND   Mov_Destinazioni.Id_Mov_Det = Movimenti_Dettagli.Id_Mov_Det " & vbCrLf)

            stb.Append(" WHERE   Mov_Dettagli_Riferimenti.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " & vbCrLf)
            stb.Append(" AND     Mov_Dettagli_Riferimenti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " & vbCrLf)

            If Piva <> "" Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Piva = '" & Agro_SQL_SaveText(Piva) & "'   " & vbCrLf)
            End If

            If Sa_Cod <> 0 Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   " & vbCrLf)
            End If

            If Id_Agenda <> 0 Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   " & vbCrLf)
            End If

            If Id_Mov <> 0 Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   " & vbCrLf)
            End If

            If Id_Mov_Det <> 0 Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   " & vbCrLf)
            End If

            If Lav_Cod <> 0 Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & "   " & vbCrLf)
            End If

            If Cau_Mov <> "" Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Cau_Mov = '" & Agro_SQL_SaveText(Cau_Mov) & "'   " & vbCrLf)
            End If


            If Piva_Rif <> "" Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Piva_Rif = '" & Agro_SQL_SaveText(Piva_Rif) & "'   " & vbCrLf)
            End If

            If Sa_Cod_Rif <> 0 Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Sa_Cod_Rif = " & Agro_SQL_SaveNum(Sa_Cod_Rif) & "   " & vbCrLf)
            End If

            If Id_Agenda_Rif <> 0 Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Id_Agenda_Rif = " & Agro_SQL_SaveNum(Id_Agenda_Rif) & "   " & vbCrLf)
            End If

            If Id_Mov_Rif <> 0 Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Id_Mov_Rif = " & Agro_SQL_SaveNum(Id_Mov_Rif) & "   " & vbCrLf)
            End If

            If Id_Mov_Det_Rif <> 0 Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Id_Mov_Det_Rif = " & Agro_SQL_SaveNum(Id_Mov_Det_Rif) & "   " & vbCrLf)
            End If

            If Lav_Cod_Rif <> 0 Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Lav_Cod_Rif = " & Agro_SQL_SaveNum(Lav_Cod_Rif) & "   " & vbCrLf)
            End If

            If Cau_Mov_Rif <> "" Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Cau_Mov_Rif = '" & Agro_SQL_SaveText(Cau_Mov_Rif) & "'   " & vbCrLf)
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
            End If

            If xOrderBy <> "" Then
                stb.Append(Agro_SQL_Save_xOrderBy(xOrderBy, objParametri) & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            DT = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try


        Return DT


    End Function

    '#############################################################################################
    'a differenza della precedente non  si presuppone che id_mov_rif sia valorizzato e quindi non viene messo in join con movimenti
    Public Function Leggi_MovDestinazioniRif2(ByVal Piva As String,
                                                         ByVal Sa_Cod As Integer,
                                                         ByVal Id_Agenda As Integer,
                                                         ByVal Id_Mov As Integer,
                                                         ByVal Id_Mov_Det As Integer,
                                                         ByVal Lav_Cod As Integer,
                                                         ByVal Cau_Mov As String,
                                                          ByVal Piva_Rif As String,
                                                        ByVal Sa_Cod_Rif As Int32,
                                                        ByVal Id_Agenda_Rif As Int32,
                                                        ByVal Id_Mov_Rif As Int32,
                                                        ByVal Id_Mov_Det_Rif As Int32,
                                                        ByVal Lav_Cod_Rif As Int32,
                                                        ByVal Cau_Mov_Rif As String,
                                                        ByVal xFiltroAggiuntivo As String,
                                                        ByVal xOrderBy As String,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                        ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R.LeggiRif_ConMovDestinazioni()"

        Dim messaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '----------------------------------------------------
            '--- Preparo la Query SQL ---------------------------
            '----------------------------------------------------

            stb.Append(" SELECT  Agenda.Des_Lib, Agenda.Validita_Inizio,  " & vbCrLf)
            stb.Append("         Mov_Destinazioni.Sa_Cod AS Sa_Cod_Fabbricato, Mov_Destinazioni.Id_Destinazione AS Fabbricato_Cod, " & vbCrLf)
            stb.Append("          Mov_Dettagli_Riferimenti.Piva, Mov_Dettagli_Riferimenti.Sa_Cod, Mov_Dettagli_Riferimenti.Id_Agenda,   " & vbCrLf)
            stb.Append("         Mov_Dettagli_Riferimenti.Id_Mov, Mov_Dettagli_Riferimenti.Id_Mov_Det, Mov_Dettagli_Riferimenti.Lav_Cod, Mov_Dettagli_Riferimenti.Cau_Mov, " & vbCrLf)
            stb.Append("         Mov_Dettagli_Riferimenti.Piva_Rif, Mov_Dettagli_Riferimenti.Sa_Cod_Rif, Mov_Dettagli_Riferimenti.Id_Agenda_Rif, " & vbCrLf)
            stb.Append("         Mov_Dettagli_Riferimenti.Id_Mov_Rif, Mov_Dettagli_Riferimenti.Id_Mov_Det_Rif, Mov_Dettagli_Riferimenti.Lav_Cod_Rif,  " & vbCrLf)
            stb.Append("         Mov_Dettagli_Riferimenti.Cau_Mov_Rif, Mov_Dettagli_Riferimenti.Qta, Mov_Dettagli_Riferimenti.Validita_Inizio AS Validita_Inizio_Tabella, Mov_Dettagli_Riferimenti.Validita_Fine AS Validita_Fine_Tabella, " & vbCrLf)
            stb.Append("         Mov_Dettagli_Riferimenti.Preserva_Legame, " + vbCrLf)
            stb.Append("         ISNULL(Mov_Dettagli_Riferimenti.Tipo_Associazione, 0) AS Tipo_Associazione " + vbCrLf)

            stb.Append("  FROM   Mov_Dettagli_Riferimenti " & vbCrLf)

            stb.Append(" INNER JOIN Movimenti " & vbCrLf)
            stb.Append(" ON   Mov_Dettagli_Riferimenti.Piva_Rif = Movimenti.Piva " & vbCrLf)
            stb.Append(" AND   Mov_Dettagli_Riferimenti.Id_Agenda_Rif = Movimenti.Id_Agenda " & vbCrLf)
            'stb.Append(" AND   Mov_Dettagli_Riferimenti.Id_Mov_Rif = Movimenti.Id_Mov " & vbCrLf)

            stb.Append(" INNER JOIN Agenda " & vbCrLf)
            stb.Append(" ON   Movimenti.Piva = Agenda.Piva " & vbCrLf)
            stb.Append(" AND   Movimenti.Id_Agenda = Agenda.Id_Agenda " & vbCrLf)

            stb.Append(" INNER JOIN Movimenti_Dettagli " & vbCrLf)
            stb.Append(" ON   Movimenti.Piva = Movimenti_Dettagli.Piva " & vbCrLf)
            stb.Append(" AND   Movimenti.Id_Agenda = Movimenti_Dettagli.Id_Agenda " & vbCrLf)
            stb.Append(" AND   Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov " & vbCrLf)

            stb.Append(" INNER JOIN Mov_Destinazioni " & vbCrLf)
            stb.Append(" ON   Mov_Destinazioni.Piva = Movimenti_Dettagli.Piva " & vbCrLf)
            stb.Append(" AND   Mov_Destinazioni.Id_Agenda = Movimenti_Dettagli.Id_Agenda " & vbCrLf)
            stb.Append(" AND   Mov_Destinazioni.Id_Mov = Movimenti_Dettagli.Id_Mov " & vbCrLf)
            stb.Append(" AND   Mov_Destinazioni.Id_Mov_Det = Movimenti_Dettagli.Id_Mov_Det " & vbCrLf)

            stb.Append(" WHERE   Mov_Dettagli_Riferimenti.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " & vbCrLf)
            stb.Append(" AND     Mov_Dettagli_Riferimenti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " & vbCrLf)

            If Piva <> "" Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Piva = '" & Agro_SQL_SaveText(Piva) & "'   " & vbCrLf)
            End If

            If Sa_Cod <> 0 Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   " & vbCrLf)
            End If

            If Id_Agenda <> 0 Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   " & vbCrLf)
            End If

            If Id_Mov <> 0 Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   " & vbCrLf)
            End If

            If Id_Mov_Det <> 0 Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   " & vbCrLf)
            End If

            If Lav_Cod <> 0 Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & "   " & vbCrLf)
            End If

            If Cau_Mov <> "" Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Cau_Mov = '" & Agro_SQL_SaveText(Cau_Mov) & "'   " & vbCrLf)
            End If


            If Piva_Rif <> "" Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Piva_Rif = '" & Agro_SQL_SaveText(Piva_Rif) & "'   " & vbCrLf)
            End If

            If Sa_Cod_Rif <> 0 Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Sa_Cod_Rif = " & Agro_SQL_SaveNum(Sa_Cod_Rif) & "   " & vbCrLf)
            End If

            If Id_Agenda_Rif <> 0 Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Id_Agenda_Rif = " & Agro_SQL_SaveNum(Id_Agenda_Rif) & "   " & vbCrLf)
            End If

            If Id_Mov_Rif <> 0 Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Id_Mov_Rif = " & Agro_SQL_SaveNum(Id_Mov_Rif) & "   " & vbCrLf)
            End If

            If Id_Mov_Det_Rif <> 0 Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Id_Mov_Det_Rif = " & Agro_SQL_SaveNum(Id_Mov_Det_Rif) & "   " & vbCrLf)
            End If

            If Lav_Cod_Rif <> 0 Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Lav_Cod_Rif = " & Agro_SQL_SaveNum(Lav_Cod_Rif) & "   " & vbCrLf)
            End If

            If Cau_Mov_Rif <> "" Then
                stb.Append(" AND Mov_Dettagli_Riferimenti.Cau_Mov_Rif = '" & Agro_SQL_SaveText(Cau_Mov_Rif) & "'   " & vbCrLf)
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
            End If

            If xOrderBy <> "" Then
                stb.Append(Agro_SQL_Save_xOrderBy(xOrderBy, objParametri) & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            DT = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try


        Return DT


    End Function

    '################################################################################
    Public Sub Recupera_PrezzoUnitario_FatturaAllegata(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                            ByRef Prezzo_Unitario As Decimal,
                                                            ByRef Prezzo_Unitario_Netto As Decimal,
                                                            ByVal Piva_Rif As String,
                                                            ByVal Sa_Cod_Rif As Integer,
                                                            ByVal Id_Agenda_Rif As Integer,
                                                            Optional ByVal Id_Mov_Rif As Integer = 0,
                                                            Optional ByVal Id_Mov_Det_Rif As Integer = 0,
                                                            Optional ByVal Lav_Cod_Rif As Integer = 0,
                                                            Optional ByVal Cau_Mov_Rif As String = "",
                                                            Optional ByVal Lav_Cod As Integer = 0,
                                                            Optional ByVal Cau_Mov As String = ""
                                                            )

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R.Recupera_PrezzoUnitario_FatturaAllegata()"

        Dim messaggioErrore As String = ""

        Try


            Dim DT_Rif As DataTable

            Dim objRif As New AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R

            DT_Rif = objRif.MovDettagliRiferimenti_Leggi_MovDettagli(Piva_Rif,
                                                                        Sa_Cod_Rif,
                                                                        Id_Agenda_Rif,
                                                                        Id_Mov_Rif,
                                                                        Id_Mov_Det_Rif,
                                                                        Lav_Cod_Rif,
                                                                        Cau_Mov_Rif,
                                                                        Lav_Cod,
                                                                        Cau_Mov,
                                                                        "", objParametri)

            If Not IsNothing(DT_Rif) Then

                If DT_Rif.Rows.Count <> 0 Then

                    Prezzo_Unitario = DT_Rif.Rows(0).Item("Prezzo_Unitario")
                    Prezzo_Unitario_Netto = DT_Rif.Rows(0).Item("Prezzo_Unitario_Netto")

                End If

            End If


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try


    End Sub



    '################################################################################
    Public Function Verifica_Operazione_SeminaTrapianto_Con_Bolle_Collegate(ByVal Piva As String,
                                                                            ByVal Sa_Cod As Integer,
                                                                            ByVal Id_Agenda As Integer,
                                                                            ByVal Lav_Cod As Integer,
                                                                            ByVal xFiltroAggiuntivo As String,
                                                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                            ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Movimenti_R.Verifica_Operazione_SeminaTrapianto_Con_Bolle_Collegate()"

        Dim messaggioErrore As String = ""
        Dim DT_Rif As DataTable
        Dim i As Integer

        Dim EsisteDDTCollegatoConDocNumero_0 As Boolean = False
        Dim EsisteDDTCollegatoConDocNumeroNON_0 As Boolean = False


        Try
            'ci vuole piva sacod idagenda lavcod e caumov della semina per join corretto
            DT_Rif = Recupera_DT_Rif_Unificato(Piva,
                                                Sa_Cod,
                                                Id_Agenda,
                                                0, 0,
                                                Lav_Cod,
                                                CStr(enum_Agenda_Causali.LAVORAZIONE),
                                                objParametri)

            If Not IsNothing(DT_Rif) AndAlso DT_Rif.Rows.Count > 0 Then


                Dim piva_collegata As String
                Dim sa_cod_collegato As Integer
                Dim lav_cod_collegato As Integer
                Dim cau_mov_collegato As String

                For i = 0 To DT_Rif.Rows.Count - 1

                    piva_collegata = DT_Rif.Rows(i).Item("Piva_Risultato")
                    sa_cod_collegato = DT_Rif.Rows(i).Item("Sa_Cod_Risultato")
                    lav_cod_collegato = DT_Rif.Rows(i).Item("Lav_Cod_Risultato")
                    cau_mov_collegato = DT_Rif.Rows(i).Item("Cau_Mov_Risultato")

                    ' se la semina è collegata a un movimento di scarico di magazzino di un'altra impresa
                    If (lav_cod_collegato = LAVCOD_BOLLA_RICEVUTA Or lav_cod_collegato = LAVCOD_FATTURA_RICEVUTA) And
                          piva_collegata = Piva And
                          sa_cod_collegato = 0 And
                          cau_mov_collegato = CAU_REGISTRAZIONI Then

                        'dato che nella semina posso avere delle bolle collegare quando utilizzo 
                        'il magazzino di un'altra impresa, che mi crea uno scarico dall'altro magazzino e un ddt
                        'fittizio per ricevere lo scarico, devo controllare che il ddt non sia quello fittizio, in questo 
                        'caso non sono in una operazione di semina con bolle collegate 
                        'ma in una operazione di semina con magazzino altra impresa.
                        'dovrebbe essere impossibile fare una operazione con entrambe queste caratteristiche

                        'Se non esiste il collegamento ad uno ed uno solo movimento contabile mi genera una eccezione voluta,
                        'se si vuole solo controllare bisogna usare Esiste_Almeno_Un_Doc_Contabile_Riferito come nella cancellazione
                        Dim Doc_Numero As Integer = New AgronicaCoreContabDAL.Movimenti_R().DocNumero_from_IdAgenda(DT_Rif.Rows(i).Item("Piva_Risultato"),
                                                                                             DT_Rif.Rows(i).Item("Id_Agenda_Risultato"),
                                                                                             "",
                                                                                             objParametri)

                        If Doc_Numero = 0 Then
                            'bisogna verificare se il ddt è fittizio oppure no
                            EsisteDDTCollegatoConDocNumero_0 = True
                            If EsisteDDTCollegatoConDocNumeroNON_0 Then
                                'se ho trovato un doc_numero=0 ma in precedenza ne avevo trovato uno <>0, quindi 
                                'flag è true allora ho dei ddt collegati sia fittizi che veri, quindi vuol dire che ho sia delle bolle 
                                'collegate alla semina che degli scarichi da magazzini di altra impresa stile conserve, e questo non è consentito
                                Throw New Exception("Attenzione, sono collegate all'operazione sia ddt fittizi che reali, e questo non è consentito")
                            End If
                        Else
                            EsisteDDTCollegatoConDocNumeroNON_0 = True
                            'se doc_numero <>0 il ddt non è fittizio
                            'quindi ho bolle collegate
                            If EsisteDDTCollegatoConDocNumero_0 Then
                                'se ho trovato un doc_numero<>0 ma in precedenza ne avevo trovato uno =0, quindi 
                                'flag è true allora ho dei ddt collegati sia fittizi che veri, quindi vuol dire che ho sia delle bolle 
                                'collegate alla semina che degli scarichi da magazzini di altra impresa stile conserve, e questo non è consentito
                                Throw New Exception("Attenzione, sono collegate all'operazione sia ddt fittizi che reali, e questo non è consentito")
                            End If

                        End If

                        If EsisteDDTCollegatoConDocNumero_0 AndAlso EsisteDDTCollegatoConDocNumeroNON_0 Then
                            'flag è true allora ho dei ddt collegati sia fittizi che veri, quindi vuol dire che ho sia delle bolle 
                            'collegate alla semina che degli scarichi da magazzini di altra impresa stile conserve, e questo non è consentito
                            Throw New Exception("Attenzione, sono collegate all'operazione sia ddt fittizi che reali, e questo non è consentito")
                        End If


                    End If

                Next

            End If

            If EsisteDDTCollegatoConDocNumero_0 AndAlso EsisteDDTCollegatoConDocNumeroNON_0 Then
                'flag è true allora ho dei ddt collegati sia fittizi che veri, quindi vuol dire che ho sia delle bolle 
                'collegate alla semina che degli scarichi da magazzini di altra impresa stile conserve, e questo non è consentito
                Throw New Exception("Attenzione, sono collegate all'operazione sia ddt fittizi che reali, e questo non è consentito")
            End If

            Return EsisteDDTCollegatoConDocNumeroNON_0

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return EsisteDDTCollegatoConDocNumeroNON_0

    End Function

    '################################################################################
    Public Function Recupera_Dettagli_Bolle_Collegate_Al_Dettaglio_SeminaTrapianto(
                                                            ByVal Piva As String,
                                                            ByVal Sa_Cod As Integer,
                                                            ByVal Id_Agenda As Integer,
                                                            ByVal Id_Mov As Integer,
                                                            ByVal Id_Mov_Det As Integer,
                                                            ByVal LAv_Cod_Semina_Trap As Integer,
                                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                             ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Movimenti_R.Verifica_Operazione_SeminaTrapianto_Con_Bolle_Collegate()"

        Dim messaggioErrore As String = ""
        Dim DT_Rif As DataTable
        Dim i As Integer
        Dim dtret As New DataTable

        Try
            'ci vuole piva sacod idagenda lavcod e caumov della semina per join corretto
            DT_Rif = Recupera_DT_Rif_Unificato(Piva,
                                                Sa_Cod,
                                                Id_Agenda,
                                                Id_Mov, Id_Mov_Det,
                                                LAv_Cod_Semina_Trap,
                                                CStr(enum_Agenda_Causali.LAVORAZIONE),
                                                objParametri)

            If Not IsNothing(DT_Rif) AndAlso DT_Rif.Rows.Count > 0 Then


                Dim piva_collegata As String
                Dim sa_cod_collegato As Integer
                Dim lav_cod_collegato As Integer
                Dim cau_mov_collegato As String
                Dim id_agenda_collegato As Integer
                Dim id_mov_collegato As Integer
                Dim Id_Mov_det_collegato As Integer
                'per memorizzare la qta presente nel riferimento senza fare un'altra query
                Dim Qta_del_riferimento As Decimal




                For i = 0 To DT_Rif.Rows.Count - 1

                    piva_collegata = DT_Rif.Rows(i).Item("Piva_Risultato")
                    sa_cod_collegato = DT_Rif.Rows(i).Item("Sa_Cod_Risultato")
                    lav_cod_collegato = DT_Rif.Rows(i).Item("Lav_Cod_Risultato")
                    cau_mov_collegato = DT_Rif.Rows(i).Item("Cau_Mov_Risultato")
                    id_agenda_collegato = DT_Rif.Rows(i).Item("Id_Agenda_Risultato")
                    id_mov_collegato = DT_Rif.Rows(i).Item("Id_Mov_Risultato")
                    Id_Mov_det_collegato = DT_Rif.Rows(i).Item("Id_Mov_Det_Risultato")
                    Qta_del_riferimento = DT_Rif.Rows(i).Item("Qta_Risultato")


                    If (lav_cod_collegato = LAVCOD_BOLLA_RICEVUTA Or lav_cod_collegato = LAVCOD_FATTURA_RICEVUTA) And
                          piva_collegata = Piva And
                          sa_cod_collegato = 0 And
                          cau_mov_collegato = CAU_REGISTRAZIONI Then

                        'DEVO METTERE PER IL LOTTO LOTTO_NONDEFINITO=-999
                        'PER FARE IN MODO CHE NON LO FILTRI, "" FILTRA
                        Dim obj As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
                        Dim dtmd As DataTable = obj.Leggi_Singola(piva_collegata,
                                                                  sa_cod_collegato,
                                                                  id_agenda_collegato,
                                                                  id_mov_collegato,
                                                                  Id_Mov_det_collegato,
                                                                  SEMENTI,
                                                                  0,
                                                                  0, 0, 0, LOTTO_NONDEFINITO, 0, 0, "", "", enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri)



                        dtmd.Columns.Add(New DataColumn("Qta_Risultato", GetType(Decimal)))
                        Dim j = 0
                        For j = 0 To dtmd.Rows.Count - 1
                            dtmd.Rows(j).Item("Qta_Risultato") = Qta_del_riferimento
                        Next


                        If i = 0 Then
                            dtret = dtmd
                        Else
                            'dtret.Merge(dtmd)
                            Dim k As Integer
                            For k = 0 To dtmd.Rows.Count - 1
                                dtret.ImportRow(dtmd.Rows(k))
                            Next
                        End If


                    End If

                Next

            End If

            Return dtret

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return New DataTable

    End Function


End Class



'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################






Public Class Mov_Det_Riferimenti_W
    Inherits AgronicaCoreDataProvider.DataProvider


    '============================================================================
    Public Function G2G_ModificaCodici(
                            ByVal Piva As String,
                            ByVal Sa_Cod As Int32,
                            ByVal Id_Agenda As Int32,
                            ByVal Id_Mov As Int32,
                            ByVal Id_Mov_Det As Int32,
                            ByVal Piva_Ric As String,
                            ByVal Sa_Cod_Ric As Int32,
                            ByVal Id_Agenda_Ric As Int32,
                            ByVal Id_Mov_Ric As Int32,
                            ByVal Id_Mov_Det_Ric As Int32,
                            ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            ByRef righeAggiornate As Int32
                        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Mov_Det_Riferimenti_W.G2G_ModificaCodici()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try



            '---------------------------------------------
            stb.Length = 0


            stb.Append(" UPDATE Mov_Dettagli_Riferimenti SET ")
            stb.Append(" Piva_Rif = '" & Agro_SQL_SaveText(Piva_Ric) & "' " & vbCrLf)
            stb.Append(" , Sa_Cod_Rif = " & Agro_SQL_SaveNum(Sa_Cod_Ric) & vbCrLf)
            stb.Append(" , Id_Agenda_Rif = " & Agro_SQL_SaveNum(Id_Agenda_Ric) & vbCrLf)
            stb.Append(" , Id_Mov_Rif = " & Agro_SQL_SaveNum(Id_Mov_Ric) & vbCrLf)
            stb.Append(" , Id_Mov_Det_Rif = " & Agro_SQL_SaveNum(Id_Mov_Det_Ric) & vbCrLf)
            stb.Append(" where " & vbCrLf)
            stb.Append(" Piva_Rif = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            stb.Append(" and Sa_Cod_Rif = " & Sa_Cod & vbCrLf)
            stb.Append(" and Id_Agenda_Rif =  " & Id_Agenda & vbCrLf)
            stb.Append(" and Id_Mov_Rif = " & Id_Mov & vbCrLf)
            stb.Append(" and Id_Mov_Det_Rif = " & Id_Mov_Det & vbCrLf)
            stb.Append(" ")



            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_ScritturaNum(objParametri, stb.ToString, NomeRoutine, righeAggiornate)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function ScriviPerAgenda(ByVal Piva As String,
                                    ByVal Id_Agenda As Integer,
                                    ByVal Lav_Cod As Integer,
                                    ByVal Piva_Rif As String,
                                    ByVal Id_Agenda_Rif As Integer,
                                    ByVal Lav_Cod_Rif As Integer,
                                    ByVal Validita_Inizio As Date,
                                    ByRef objParametri As AgronicaCoreParametri,
                                    Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                                    Optional ByVal username_creazione As String = ""
                                    ) As Boolean

        Dim xRisp = Scrivi(Piva, 0, Id_Agenda, -1, -1, Lav_Cod, -1,
                           Piva_Rif, 0, Id_Agenda_Rif, -1, -1, Lav_Cod_Rif, -1,
                           0, Validita_Inizio, AGRODATAFINE, objParametri,
                           Data_creazione:=Data_creazione, username_creazione:=username_creazione)

        Return xRisp

    End Function

    Public Function Scrivi(ByVal Piva As String,
                           ByVal Sa_Cod As Integer,
                           ByVal Id_Agenda As Integer,
                           ByVal Id_Mov As Integer,
                           ByVal Id_Mov_Det As Integer,
                           ByVal Lav_Cod As Integer,
                           ByVal Cau_Mov As String,
                           ByVal Piva_Rif As String,
                           ByVal Sa_Cod_Rif As Integer,
                           ByVal Id_Agenda_Rif As Integer,
                           ByVal Id_Mov_Rif As Integer,
                           ByVal Id_Mov_Det_Rif As Integer,
                           ByVal Lav_Cod_Rif As Integer,
                           ByVal Cau_Mov_Rif As String,
                           ByVal Qta As Decimal,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = "",
                           Optional ByVal Preserva_Legame As Integer = 0,
                           Optional ByVal Tipo_Associazione As Integer = 0
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Det_Riferimenti_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
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
            StrSQL.Length = 0

            StrSQL.Append(" INSERT INTO Mov_Dettagli_Riferimenti ")
            StrSQL.Append("         ( ")
            StrSQL.Append("          Piva,     Sa_Cod,     Id_Agenda,      Id_Mov,     Id_Mov_Det,     Lav_Cod,     Cau_Mov,  ")
            StrSQL.Append("          Piva_Rif, Sa_Cod_Rif, Id_Agenda_Rif , Id_Mov_Rif, Id_Mov_Det_Rif, Lav_Cod_Rif, Cau_Mov_Rif, ")
            StrSQL.Append("          Qta, Preserva_Legame, Tipo_Associazione,")

            StrSQL.Append("          Inviato,            DataInvio, ")
            StrSQL.Append("          Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("          UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("          Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("         ) ")

            StrSQL.Append(" VALUES ( ")
            StrSQL.Append("          '" & Agro_SQL_SaveText(Piva) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Agenda) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Mov) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Mov_Det) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Lav_Cod) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(UCase(Cau_Mov)) & "'  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Piva_Rif) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod_Rif) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Agenda_Rif) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Mov_Rif) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Mov_Det_Rif) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Lav_Cod_Rif) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(UCase(Cau_Mov_Rif)) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Qta) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Preserva_Legame) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Tipo_Associazione) & "  ")

            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")

            StrSQL.Append("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")



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
    Public Function Cancella(ByVal Piva As String,
                             ByVal Sa_Cod As Integer,
                             ByVal Id_Agenda As Integer,
                             ByVal Id_Mov As Integer,
                             ByVal Id_Mov_Det As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                             Optional ByVal Id_Agenda_Rif As Integer = 0,
                             Optional ByVal Id_Mov_Rif As Integer = 0,
                             Optional ByVal Id_Mov_Det_Rif As Integer = 0
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Det_Riferimenti_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""
        '   Sa_Cod = 0
        '   Id_Agenda = 0
        '   Id_Mov = 0
        '   Id_Mov_Det = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Mov_Dettagli_Riferimenti ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")
            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM Mov_Dettagli_Riferimenti ")
                StrSQL.Append(" WHERE  1=1 ")
            End If

            If Piva <> String.Empty Then
                StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            'If Sa_Cod <> 0 Then
            '    StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            'End If

            If Id_Agenda <> 0 Then
                StrSQL.Append(" AND Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
            End If

            If Id_Mov <> 0 Then
                StrSQL.Append(" AND Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
            End If

            If Id_Mov_Det <> 0 Then
                StrSQL.Append(" AND Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")
            End If

            If Id_Agenda_Rif <> 0 Then
                StrSQL.Append(" AND Id_Agenda_Rif = " & Agro_SQL_SaveNum(Id_Agenda_Rif) & "   ")
            End If

            If Id_Mov_Rif <> 0 Then
                StrSQL.Append(" AND Id_Mov_Rif = " & Agro_SQL_SaveNum(Id_Mov_Rif) & "   ")
            End If

            If Id_Mov_Det_Rif <> 0 Then
                StrSQL.Append(" AND Id_Mov_Det_Rif = " & Agro_SQL_SaveNum(Id_Mov_Det_Rif) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
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

    Public Function Cancella_byChiaveRif(ByVal Piva_Rif As String,
                                         ByVal Sa_Cod_Rif As Integer,
                                         ByVal Id_Agenda_Rif As Integer,
                                         ByVal Id_Mov_Rif As Integer,
                                         ByVal Id_Mov_Det_Rif As Integer,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                         ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Det_Riferimenti_W.Cancella_byChiaveRif()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva_Rif = ""
        '   Sa_Cod_Rif = 0
        '   Id_Agenda_Rif = 0
        '   Id_Mov_Rif = 0
        '   Id_Mov_Det_Rif = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Mov_Dettagli_Riferimenti ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")

            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM Mov_Dettagli_Riferimenti ")
                StrSQL.Append(" WHERE  1=1 ")
            End If

            If Piva_Rif <> String.Empty Then
                StrSQL.Append(" AND Piva_Rif = '" & Agro_SQL_SaveText(Piva_Rif) & "' ")
            End If

            If Sa_Cod_Rif <> 0 Then
                StrSQL.Append(" AND Sa_Cod_Rif = " & Agro_SQL_SaveNum(Sa_Cod_Rif) & "   ")
            End If

            If Id_Agenda_Rif <> 0 Then
                StrSQL.Append(" AND Id_Agenda_Rif = " & Agro_SQL_SaveNum(Id_Agenda_Rif) & "   ")
            End If

            If Id_Mov_Rif <> 0 Then
                StrSQL.Append(" AND Id_Mov_Rif = " & Agro_SQL_SaveNum(Id_Mov_Rif) & "   ")
            End If

            If Id_Mov_Det_Rif <> 0 Then
                StrSQL.Append(" AND Id_Mov_Det_Rif = " & Agro_SQL_SaveNum(Id_Mov_Det_Rif) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
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

    Public Function ModificaPuntuale(ByVal Piva As String,
                                     ByVal Sa_Cod As Integer,
                                     ByVal Id_Agenda As Integer,
                                     ByVal Id_Mov As Integer,
                                     ByVal Id_Mov_Det As Integer,
                                     ByVal Id_Agenda_Rif As Integer,
                                     ByVal Id_Mov_Rif As Integer,
                                     ByVal id_Mov_Det_Rif As Integer,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                     Optional ByVal Lav_Cod As Integer? = Nothing,
                                     Optional ByVal Cau_Mov As String = Nothing,
                                     Optional ByVal Piva_Rif As String = Nothing,
                                     Optional ByVal Sa_Cod_Rif As Integer? = Nothing,
                                     Optional ByVal Lav_Cod_Rif As Integer? = Nothing,
                                     Optional ByVal Cau_Mov_Rif As String = Nothing,
                                     Optional ByVal Qta As Decimal? = Nothing,
                                     Optional ByVal Validita_Inizio As Date? = Nothing,
                                     Optional ByVal Validita_Fine As Date? = Nothing,
                                     Optional ByVal Preserva_Legame As Integer? = Nothing,
                                     Optional ByVal Data_Modifica As DateTime = #2/1/1900#,
                                     Optional ByVal Username_Modifica As String = "",
                                     Optional ByVal Tipo_Associazione As Integer? = Nothing
                                     ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Det_Riferimenti_W.ModificaPuntuale()"

        '====================================================================================
        'Parametri opzionali :
        '   Tutti i valori non chiave (se impostati a nothing o non passati 
        '   non ne verrà fatto l'aggiornamento e rimarranno i valori precedenti)
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            If Data_Modifica = #2/1/1900# Then
                Data_Modifica = Now
            End If

            If Username_Modifica = "" Then
                Username_Modifica = objParametri.UsernameOperazione
            End If

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

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

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE Mov_Dettagli_Riferimenti ")
            strSql.AppendLine(" SET Data_Modifica = " & Agro_SQL_SaveDateTime(Data_Modifica) & " ")
            strSql.AppendLine("   , Username_Modifica = '" & Agro_SQL_SaveText(Username_Modifica) & "' ")


            If Not IsNothing(Lav_Cod) Then
                strSql.AppendLine("   , Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & " ")
            End If

            If Not IsNothing(Cau_Mov) Then
                strSql.AppendLine("   , Cau_Mov = '" & Agro_SQL_SaveText(Cau_Mov) & "' ")
            End If

            If Not IsNothing(Piva_Rif) Then
                strSql.AppendLine("   , Piva_Rif = '" & Agro_SQL_SaveText(Piva_Rif) & "' ")
            End If

            If Not IsNothing(Sa_Cod_Rif) Then
                strSql.AppendLine("   , Sa_Cod_Rif = " & Agro_SQL_SaveNum(Sa_Cod_Rif) & " ")
            End If

            If Not IsNothing(Lav_Cod_Rif) Then
                strSql.AppendLine("   , Lav_Cod_Rif = " & Agro_SQL_SaveNum(Lav_Cod_Rif) & " ")
            End If

            If Not IsNothing(Cau_Mov_Rif) Then
                strSql.AppendLine("   , Cau_Mov_Rif = '" & Agro_SQL_SaveText(Cau_Mov_Rif) & "' ")
            End If

            If Not IsNothing(Qta) Then
                strSql.AppendLine("   , Qta = " & Agro_SQL_SaveNum(Qta) & " ")
            End If

            If Not IsNothing(Validita_Inizio) Then
                strSql.AppendLine("   , Validita_Inizio = " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            End If

            If Not IsNothing(Validita_Fine) Then
                strSql.AppendLine("   , Validita_Fine = " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            End If

            If Not IsNothing(Preserva_Legame) Then
                strSql.AppendLine("   , Preserva_Legame = " & Agro_SQL_SaveNum(Preserva_Legame) & " ")
            End If

            If Not IsNothing(Tipo_Associazione) Then
                strSql.AppendLine("   , Tipo_Associazione = " & Agro_SQL_SaveNum(Tipo_Associazione) & " ")
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
                strSql.AppendLine(" AND Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & " ")
            End If

            If Id_Agenda_Rif <> 0 Then
                strSql.AppendLine(" AND Id_Agenda_Rif = " & Agro_SQL_SaveNum(Id_Agenda_Rif) & " ")
            End If

            If Id_Mov_Rif <> 0 Then
                strSql.AppendLine(" AND Id_Mov_Rif = " & Agro_SQL_SaveNum(Id_Mov_Rif) & " ")
            End If

            If id_Mov_Det_Rif <> 0 Then
                strSql.AppendLine(" AND Id_Mov_Det_Rif = " & Agro_SQL_SaveNum(id_Mov_Det_Rif) & " ")
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
