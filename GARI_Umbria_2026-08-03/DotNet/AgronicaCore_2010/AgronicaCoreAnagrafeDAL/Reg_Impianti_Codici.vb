Imports System.Data.Entity
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider

Public Class Reg_Impianti_Codici_R
    Inherits AgronicaCoreDataProvider.DataProvider




    Public Function Leggi_TabellaCompleta(
                               ByVal PIVA As String,
                               ByVal Sa_Cod As Int32,
                               ByVal Appezza As Int32,
                               ByVal Id_Reg As Int32,
                               ByVal Gruppo As String,
                               ByVal Id_Cod As Int32,
                               ByVal Val_Cod As String,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R.Leggi_TabellaCompleta()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = "" 
        '   Sa_Cod = 0 
        '   Appezza = 0 
        '   Id_Reg = 0
        '   Id_Cod = 0 
        '   Val_Cod = "" 
        '   Gruppo = "" 
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim StrSQL_Codici_Anagrafe As New System.Text.StringBuilder
        Dim StrSQL_SpecieVegetali As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------

            StrSQL.Length = 0
            StrSQL_SpecieVegetali.Append(" SELECT Distinct RIP.*,CA.*,  RIP.Validita_Inizio as xValidita_Inizio, RIP.Validita_Fine as xValidita_Fine, Veg_Cod as CodiceSV,isnull(Veg_Des,rip.val_cod) as DescriSV ")
            StrSQL_Codici_Anagrafe.Append(" SELECT Distinct RIP.*,CA.*, RIP.Validita_Inizio as xValidita_Inizio, RIP.Validita_Fine as xValidita_Fine, SPVE.codice  as CodiceSV, isnull(SPVE.descrizione,rip.val_cod)  as DescriSV   ")
            StrSQL.Append(" FROM  Reg_Impianti_Codici as RIP inner join Codici_Anagrafe CA")
            StrSQL.Append(" on   RIP.Id_Cod = CA.Codice ")
            StrSQL.Append(" AND RIP.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND RIP.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND RIP.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.Append(" AND CA.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND CA.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.Append(" AND RIP.Progetto_Cod = 0 ")

            If PIVA <> "" Then
                StrSQL.Append(" AND RIP.Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND RIP.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Appezza <> 0 Then
                StrSQL.Append(" AND RIP.Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If

            If Id_Reg <> 0 Then
                StrSQL.Append(" AND RIP.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & " ")
            End If

            If Id_Cod <> 0 Then
                StrSQL.Append(" AND RIP.Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")
            End If

            If Trim(Val_Cod) <> "" Then
                StrSQL.Append(" AND RIP.Val_Cod Like '%" & Agro_SQL_SaveText(Val_Cod) & "%' ")
            End If

            If Gruppo <> "" Then
                StrSQL.Append(" AND CA.Gruppo = '" & Agro_SQL_SaveText(Trim(Gruppo)) & "'")
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND RIP.Inviato >= 0 ")
                    StrSQL.Append(" AND CA.Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND RIP.Inviato = -1 ")
                    StrSQL.Append(" AND CA.Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            StrSQL_SpecieVegetali.Append(StrSQL)
            StrSQL_Codici_Anagrafe.Append(StrSQL)

            StrSQL_SpecieVegetali.Append(" Left join SpecieVegetali as SPVE ")
            StrSQL_Codici_Anagrafe.Append(" Left join Codici_Anagrafe as SPVE ")
            StrSQL_SpecieVegetali.Append(" on convert(varchar(20),SPVE.Veg_Cod) = RIP.val_cod or SPVE.Veg_Des = RIP.val_cod ")
            StrSQL_Codici_Anagrafe.Append(" on (convert(varchar(20),SPVE.codice) = RIP.val_cod or SPVE.descrizione = RIP.val_cod) ")
            StrSQL_Codici_Anagrafe.Append(" and SPVE.codice>=3000 and SPVE.gruppo='TERRENO' ")
            StrSQL.Clear()
            StrSQL.Append(StrSQL_SpecieVegetali)
            StrSQL.Append(" UNION ")
            StrSQL.Append(StrSQL_Codici_Anagrafe)
            StrSQL.Append(" where not exists( ")
            StrSQL.Append(StrSQL_SpecieVegetali)
            StrSQL.Append(" ) ")


            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY RIP.Piva, RIP.Sa_Cod, RIP.Appezza, RIP.Id_reg, RIP.Id_Cod ASC ")
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

    '################################################
    'Attenzione! Fa il like sul val_cod
    Public Function Leggi(
                               ByVal PIVA As String,
                               ByVal Sa_Cod As Int32,
                               ByVal Appezza As Int32,
                               ByVal Id_Reg As Int32,
                               ByVal Gruppo As String,
                               ByVal Id_Cod As Int32,
                               ByVal Val_Cod As String,
                                    ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = "" 
        '   Sa_Cod = 0 
        '   Appezza = 0 
        '   Id_Reg = 0
        '   Id_Cod = 0 
        '   Val_Cod = "" 
        '   Gruppo = "" 
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT PIVA, sa_cod, appezza, Id_Reg, id_cod, val_cod, Validita_Inizio, Validita_Fine ")
                    StrSQL.AppendLine(" FROM  Reg_Impianti_Codici ")
                    StrSQL.AppendLine(" WHERE Reg_Impianti_Codici.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   Reg_Impianti_Codici.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.AppendLine(" AND   Reg_Impianti_Codici.Progetto_Cod = 0 ")

                    If PIVA <> "" Then
                        StrSQL.AppendLine(" AND Reg_Impianti_Codici.Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Reg_Impianti_Codici.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If

                    If Appezza <> 0 Then
                        StrSQL.AppendLine(" AND Reg_Impianti_Codici.Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
                    End If

                    If Id_Reg <> 0 Then
                        StrSQL.AppendLine(" AND Reg_Impianti_Codici.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & " ")
                    End If

                    If Id_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Reg_Impianti_Codici.Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")
                    End If

                    If Trim(Val_Cod) <> "" Then
                        StrSQL.AppendLine(" AND Reg_Impianti_Codici.Val_Cod Like '%" & Agro_SQL_SaveText(Val_Cod) & "%' ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND     Reg_Impianti_Codici.Inviato >= 0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND     Reg_Impianti_Codici.Inviato = -1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY Reg_Impianti_Codici.Piva, Reg_Impianti_Codici.Sa_Cod, Reg_Impianti_Codici.Appezza, Reg_Impianti_Codici.Id_reg, Reg_Impianti_Codici.Id_Cod ASC ")
                    End If



                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT Distinct *,  Reg_Impianti_Codici.Validita_Inizio as xValidita_Inizio, Reg_Impianti_Codici.Validita_Fine as xValidita_Fine ")
                    StrSQL.AppendLine(" FROM  Reg_Impianti_Codici, Codici_Anagrafe ")
                    StrSQL.AppendLine(" WHERE Reg_Impianti_Codici.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   Reg_Impianti_Codici.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.AppendLine(" AND   Codici_Anagrafe.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   Codici_Anagrafe.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.AppendLine(" AND   Reg_Impianti_Codici.Id_Cod = Codici_Anagrafe.Codice ")
                    StrSQL.AppendLine(" AND   Reg_Impianti_Codici.Progetto_Cod = 0 ")

                    If PIVA <> "" Then
                        StrSQL.AppendLine(" AND Reg_Impianti_Codici.Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Reg_Impianti_Codici.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If

                    If Appezza <> 0 Then
                        StrSQL.AppendLine(" AND Reg_Impianti_Codici.Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
                    End If

                    If Id_Reg <> 0 Then
                        StrSQL.AppendLine(" AND Reg_Impianti_Codici.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & " ")
                    End If

                    If Id_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Reg_Impianti_Codici.Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")
                    End If

                    If Trim(Val_Cod) <> "" Then
                        StrSQL.AppendLine(" AND Reg_Impianti_Codici.Val_Cod Like '%" & Agro_SQL_SaveText(Val_Cod) & "%' ")
                    End If

                    If Gruppo <> "" Then
                        StrSQL.AppendLine(" AND Codici_Anagrafe.Gruppo = '" & Agro_SQL_SaveText(Trim(Gruppo)) & "'")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND     Reg_Impianti_Codici.Inviato >= 0 ")
                            StrSQL.AppendLine(" AND     Codici_Anagrafe.Inviato >= 0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND     Reg_Impianti_Codici.Inviato = -1 ")
                            StrSQL.AppendLine(" AND     Codici_Anagrafe.Inviato = -1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY Reg_Impianti_Codici.Piva, Reg_Impianti_Codici.Sa_Cod, Reg_Impianti_Codici.Appezza, Reg_Impianti_Codici.Id_reg, Reg_Impianti_Codici.Id_Cod ASC ")
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni
                    '
                    '
                    '
                    '



                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
                    '
                    '
                    '
                    ' Estrapolo tutti i record Codici dell'impianto (non quelli del'azienda, Reg_Impianti_Codici.Progetto_Cod = 0)

                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT Distinct *,  Reg_Impianti_Codici.Validita_Inizio as xValidita_Inizio, Reg_Impianti_Codici.Validita_Fine as xValidita_Fine ")
                    StrSQL.AppendLine(" FROM  Reg_Impianti_Codici, Codici_Anagrafe ")
                    StrSQL.AppendLine(" WHERE Reg_Impianti_Codici.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   Reg_Impianti_Codici.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.AppendLine(" AND   Codici_Anagrafe.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   Codici_Anagrafe.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.AppendLine(" AND   Reg_Impianti_Codici.Id_Cod = Codici_Anagrafe.Codice ")
                    StrSQL.AppendLine(" AND   Reg_Impianti_Codici.Progetto_Cod <> 0 ")

                    If PIVA <> "" Then
                        StrSQL.AppendLine(" AND Reg_Impianti_Codici.Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Reg_Impianti_Codici.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If

                    If Appezza <> 0 Then
                        StrSQL.AppendLine(" AND Reg_Impianti_Codici.Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
                    End If

                    If Id_Reg <> 0 Then
                        StrSQL.AppendLine(" AND Reg_Impianti_Codici.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & " ")
                    End If

                    If Id_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Reg_Impianti_Codici.Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")
                    End If

                    If Trim(Val_Cod) <> "" Then
                        StrSQL.AppendLine(" AND Reg_Impianti_Codici.Val_Cod Like '%" & Agro_SQL_SaveText(Val_Cod) & "%' ")
                    End If

                    If Gruppo <> "" Then
                        StrSQL.AppendLine(" AND Codici_Anagrafe.Gruppo = '" & Agro_SQL_SaveText(Trim(Gruppo)) & "'")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND     Reg_Impianti_Codici.Inviato >= 0 ")
                            StrSQL.AppendLine(" AND     Codici_Anagrafe.Inviato >= 0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND     Reg_Impianti_Codici.Inviato = -1 ")
                            StrSQL.AppendLine(" AND     Codici_Anagrafe.Inviato = -1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY Reg_Impianti_Codici.Piva, Reg_Impianti_Codici.Sa_Cod, Reg_Impianti_Codici.Appezza, Reg_Impianti_Codici.Id_reg, Reg_Impianti_Codici.Id_Cod ASC ")
                    End If

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

    '################################################################
    'usato da ABOCA
    'NumOPCollegamento: è l'eventuale OP dell’azienda SAM collegato a quello di Aboca, creato quando si effettua il ribaltamento impianti da SAM ad Aboca.
    'Si trova leggendo reg_impianti_codici senza PIVA, con id_cod 1311 e con val_cod = piva + “|” + sa_cod + “|” + appezza + “|” + id_reg + “|” + progetto_cod
    'Leggendo il record trovato e prendendo piva, Sa_Cod, Appezza, Id_Reg, Progetto_Cod si cerca Imprese_progetti e si prende PIVA + “_” + Progetto_nome
    '
    'Ad es.ho letto un esercizio che ha come chiave piva = 01704430519, Sa_Cod = 130023437, Appezza = 130023425, Id_Reg = 130023425, Progetto_Cod = 2326...
    'cerco su reg_impianti_codici con id_cod 1311 e val_cod = 01704430519|130023437|130023425|130023425|2326
    'Trovo record con Piva= '02969160544’, sa_cod = 130023425, appezza = 130023425, id_reg = 130023425, progetto_cod = 1935
    'Vado a leggere imprese_progetti con questa chiave e trovo progetto_nome = 17019698
    'Imposto NumOPCollegamento = 02969160544_17019698
    Function NumOPCollegamento_From_Progetto(ByVal Piva As String,
                                            ByVal Sa_Cod As Integer,
                                            ByVal Appezza As Integer,
                                            ByVal Id_Reg As Integer,
                                            ByVal Progetto_Cod As Integer,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As String

        Dim MessaggioErrore As String = ""
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R.NumOPCollegamento_From_Progetto()"
        Dim NumOPCollegamento As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Sa_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            End If

            If Appezza = 0 Then
                Throw New Exception("Parametro non corretto nella query (Appezza obbligatorio)")
            End If

            If Id_Reg = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Reg obbligatorio)")
            End If

            If Progetto_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Progetto_Cod obbligatorio)")
            End If

            Dim VAL_COD As String = Piva & "|" & CStr(Sa_Cod) & "|" & (Appezza) & "|" & CStr(Id_Reg) & "|" & CStr(Progetto_Cod)

            StrSQL.Length = 0
            StrSQL.Append(" SELECT PIVA, sa_cod, appezza, Id_Reg, progetto_cod, id_cod, val_cod ")
            StrSQL.Append(" FROM  Reg_Impianti_Codici ")
            StrSQL.Append(" WHERE Reg_Impianti_Codici.Id_Cod = " & Agro_SQL_SaveNum(enum_CodiciAnagrafe.Codice_Impianto_Ribaltato) & " ")
            StrSQL.Append(" AND Reg_Impianti_Codici.Val_Cod = '" & Agro_SQL_SaveText(VAL_COD) & "' ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not IsNothing(dt) Then
                If dt.Rows.Count <> 0 Then

                    Dim objImpreseProg As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
                    Dim Dt_Prog As DataTable

                    Dt_Prog = objImpreseProg.LeggiMinimal(dt.Rows(0).Item("piva"),
                                                            dt.Rows(0).Item("sa_cod"),
                                                            dt.Rows(0).Item("appezza"),
                                                            dt.Rows(0).Item("id_reg"),
                                                            dt.Rows(0).Item("progetto_cod"),
                                                            AGRODATAINIZIO,
                                                            AGRODATAFINE,
                                                          "",
                                                          "",
                                                           objParametri)

                    If Not IsNothing(Dt_Prog) AndAlso Dt_Prog.Rows.Count > 0 Then
                        NumOPCollegamento = Dt_Prog.Rows(0).Item("piva") & "_" & Dt_Prog.Rows(0).Item("Progetto_Nome")
                    End If

                End If
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return NumOPCollegamento

    End Function

    '################################################################
    'usato da ABOCA
    'Data_Chiusura_Esercizio: esercizio è chiuso se id_cod = 1301 di reg_impianti_codici vale 1; 
    'come Data prendere Data_Creazione (di reg_impianti_codici) perché tanto una volta chiuso non si apre più
    Function DataChiusuraEsercizio_From_Progetto(ByVal Piva As String,
                                                    ByVal Sa_Cod As Integer,
                                                    ByVal Appezza As Integer,
                                                    ByVal Id_Reg As Integer,
                                                    ByVal Progetto_Cod As Integer,
                                                      ByVal xFiltroAggiuntivo As String,
                                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                      ) As String

        Dim DataChiusuraEsercizio As Date = AGRODATAFINE
        Dim dt As DataTable
        Dim MessaggioErrore As String = ""
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R.DataChiusuraEsercizio_From_Progetto()"

        Try

            dt = Leggi_2(Piva, Sa_Cod, Appezza, Id_Reg, Progetto_Cod, enum_CodiciAnagrafe.Distinta_Chiusa, "", "", "", objParametri)

            If Not IsNothing(dt) Then
                If dt.Rows.Count <> 0 Then
                    DataChiusuraEsercizio = dt.Rows(0).Item("data_creazione")
                End If
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DataChiusuraEsercizio

    End Function


    '################################################
    'Attenzione! Fa il like sul val_cod
    Public Function Leggi_Veg_Cod_Gestiti_da_Gias_Normale(
                               ByVal PIVA As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = "" 
        '   Sa_Cod = 0 
        '   Appezza = 0 
        '   Id_Reg = 0
        '   Id_Cod = 0 
        '   Val_Cod = "" 
        '   Gruppo = "" 
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------


            StrSQL.Length = 0
            StrSQL.Append(" SELECT     distinct Cultivar.Veg_Cod ")
            StrSQL.Append(" FROM         Reg_Impianti INNER JOIN ")
            StrSQL.Append("       Cultivar ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod ")
            StrSQL.Append(" WHERE Reg_Impianti.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Reg_Impianti.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")


            If PIVA <> "" Then
                StrSQL.Append(" AND Reg_Impianti.Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
            End If

            StrSQL.Append(" AND (NOT EXISTS  ( ")
            StrSQL.Append("                 SELECT     PIVA, sa_cod, appezza, Id_Reg ")
            StrSQL.Append("                     FROM Reg_Impianti_Codici ")
            StrSQL.Append("                     WHERE      (id_cod = 1131) AND (val_cod <> 1) ")
            If PIVA <> "" Then
                StrSQL.Append("                 AND Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
            End If
            StrSQL.Append("                  ) ")
            StrSQL.Append("    ) ")

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




    '################################################
    Public Function Leggi_Codice_Anagrafe(
                               ByVal PIVA As String,
                               ByVal Sa_Cod As Int32,
                               ByVal Appezza As Int32,
                               ByVal Id_Reg As Int32,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R.Leggi_Codice_Anagrafe()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0


            StrSQL.Append(" SELECT DISTINCT Reg_Impianti_Codici.id_cod, Codici_Anagrafe.descrizione ")
            StrSQL.Append("         FROM         Reg_Impianti_Codici INNER JOIN ")
            StrSQL.Append("                 Codici_Anagrafe ON Reg_Impianti_Codici.id_cod = Codici_Anagrafe.codice ")

            StrSQL.Append(" WHERE Reg_Impianti_Codici.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Reg_Impianti_Codici.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.Append(" AND   Reg_Impianti_Codici.Progetto_Cod = 0 ")
            StrSQL.Append(" AND   ID_Cod > 2999 and id_Cod < 4000 ")

            If PIVA <> "" Then
                StrSQL.Append(" AND Reg_Impianti_Codici.Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
            End If
            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Reg_Impianti_Codici.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If
            If Appezza <> 0 Then
                StrSQL.Append(" AND Reg_Impianti_Codici.Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If
            If Id_Reg <> 0 Then
                StrSQL.Append(" AND Reg_Impianti_Codici.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND     Reg_Impianti_Codici.Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND     Reg_Impianti_Codici.Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
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

    '#####################################################################
    'A differenza della leggi (dati minimi) non fa il like sul val_cod
    'ma lo cerca tale e quale
    'inoltre c'è la possibilità di filtrare cod_progetto:
    'il default è -1, perchè 0 è significativo (ovvero codice legato all'impianto)
    Public Function Leggi_2(
                              ByVal PIVA As String,
                              ByVal Sa_Cod As Int32,
                              ByVal Appezza As Int32,
                              ByVal Id_Reg As Int32,
                              ByVal Progetto_Cod As Int32,
                              ByVal Id_Cod As Int32,
                              ByVal Val_Cod As String,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByVal xOrderBy As String,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                   ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R.Leggi_2()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = "" 
        '   Sa_Cod = 0 
        '   Appezza = 0 
        '   Id_Reg = 0
        '   Id_Cod = 0 
        '   Val_Cod = "" 
        '   Gruppo = "" 
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT PIVA, sa_cod, appezza, Id_Reg, id_cod, val_cod, Validita_Inizio, Validita_Fine, data_creazione ")
            StrSQL.Append(" FROM  Reg_Impianti_Codici ")
            StrSQL.Append(" WHERE Reg_Impianti_Codici.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Reg_Impianti_Codici.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If PIVA <> "" Then
                StrSQL.Append(" AND Reg_Impianti_Codici.Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Reg_Impianti_Codici.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Appezza <> 0 Then
                StrSQL.Append(" AND Reg_Impianti_Codici.Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If

            If Id_Reg <> 0 Then
                StrSQL.Append(" AND Reg_Impianti_Codici.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & " ")
            End If

            If Progetto_Cod <> -1 Then
                StrSQL.Append(" AND Reg_Impianti_Codici.Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod) & " ")
            End If

            If Id_Cod <> 0 Then
                StrSQL.Append(" AND Reg_Impianti_Codici.Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")
            End If

            If Val_Cod <> "" Then
                StrSQL.Append(" AND Reg_Impianti_Codici.Val_Cod = '" & Agro_SQL_SaveText(Val_Cod) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND     Reg_Impianti_Codici.Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND     Reg_Impianti_Codici.Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Reg_Impianti_Codici.Piva, Reg_Impianti_Codici.Sa_Cod, Reg_Impianti_Codici.Appezza, Reg_Impianti_Codici.Id_reg, Reg_Impianti_Codici.Id_Cod ASC ")
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

    '#####################################################################
    'A differenza della leggi (dati minimi) non fa il like sul val_cod
    'ma lo cerca tale e quale
    'inoltre c'è la possibilità di filtrare cod_progetto:
    'il default è -1, perchè 0 è significativo (ovvero codice legato all'impianto)
    Public Function Leggi_Con_CodiciACA(
                              ByVal PIVA As String,
                              ByVal Sa_Cod As Int32,
                              ByVal Appezza As Int32,
                              ByVal Id_Reg As Int32,
                              ByVal Progetto_Cod As Int32,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByVal xOrderBy As String,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                   ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R.Leggi_Con_CodiciACA()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = "" 
        '   Sa_Cod = 0 
        '   Appezza = 0 
        '   Id_Reg = 0
        '   Id_Cod = 0 
        '   Val_Cod = "" 
        '   Gruppo = "" 
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT Imprese_Progetti.PIVA, Imprese_Progetti.sa_cod, Imprese_Progetti.appezza, ")
            StrSQL.AppendLine(" Imprese_Progetti.Id_Reg, Imprese_Progetti.Progetto_Cod, CodiciACA.Nr_Domanda_ACA ")
            StrSQL.AppendLine(" FROM  Imprese_Progetti ")
            StrSQL.AppendLine(" LEFT JOIN ( ")
            StrSQL.AppendLine(" SELECT ipc.Piva, ipc.ProgettoCod, ipc.ContributoTipo,")
            StrSQL.AppendLine("        aca.ContributoDes Nr_domanda_ACA")
            StrSQL.AppendLine(" FROM Imprese_ProgettiXContributi ipc")
            StrSQL.AppendLine(" JOIN Contributi aca")
            StrSQL.AppendLine($" ON ipc.ContributoTipo = {CInt(ContributeType.ACA)}")
            StrSQL.AppendLine(" AND ipc.ContributoTipo = aca.Tipo")
            StrSQL.AppendLine(" AND ipc.ContributoCod = aca.ContributoCod")
            StrSQL.AppendLine(" ) CodiciACA ")
            StrSQL.AppendLine(" ON Imprese_Progetti.Piva = CodiciACA.Piva ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Progetto_Cod = CodiciACA.ProgettoCod ")
            StrSQL.AppendLine(" WHERE Imprese_Progetti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If PIVA <> "" Then
                StrSQL.AppendLine(" AND Imprese_Progetti.Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Imprese_Progetti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Appezza <> 0 Then
                StrSQL.AppendLine(" AND Imprese_Progetti.Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If

            If Id_Reg <> 0 Then
                StrSQL.AppendLine(" AND Imprese_Progetti.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & " ")
            End If

            If Progetto_Cod <> -1 Then
                StrSQL.AppendLine(" AND Imprese_Progetti.Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod) & " ")
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND     Imprese_Progetti.Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND     Imprese_Progetti.Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Imprese_Progetti.Piva, Imprese_Progetti.Sa_Cod, Imprese_Progetti.Appezza, Imprese_Progetti.Id_reg")
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


    Public Function LeggiDistinctSpecieImpiantiConCodice(ByVal id_cod As Integer,
                                                         ByVal listaVal_Cod As List(Of String),
                                                         ByRef objParametri As AgronicaCoreParametri) As DataTable

        If (id_cod = 0) Then
            Throw New ArgumentException("Id_cod deve essere diverso da 0")
        End If

        If listaVal_Cod Is Nothing Then
            Throw New ArgumentNullException("listaVal_Cod")
        End If

        If Not listaVal_Cod.Any() Then
            Throw New ArgumentException("La lista val cod non contiene nessun elemento ")
        End If

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R.LeggiDistinctSpecieImpiantiConCodice()"

        Dim messaggioErrore As String = ""
        Dim sb As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            sb.Length = 0

            sb.AppendLine(" SELECT ")
            sb.AppendLine("     DISTINCT ISNULL(c.Veg_Cod, 0) as Veg_Cod")
            sb.AppendLine(" FROM ")
            sb.AppendLine("     Reg_Impianti_Codici rc")
            sb.AppendLine(" JOIN ")
            sb.AppendLine("     Reg_Impianti r")
            sb.AppendLine("     ON r.PIVA = rc.PIVA")
            sb.AppendLine("     AND r.Sa_Cod = rc.Sa_Cod")
            sb.AppendLine("     AND r.Appezza = rc.Appezza")
            sb.AppendLine("     AND r.Id_Reg = rc.Id_Reg")
            sb.AppendLine(" LEFT JOIN ")
            sb.AppendLine("     Cultivar c")
            sb.AppendLine("     ON c.Cul_Cod = r.Cul_Cod")
            sb.AppendLine(" WHERE ")
            sb.AppendLine("     rc.Id_Cod = " & Agro_SQL_SaveNum(id_cod) & " ")
            sb.AppendLine("     AND rc.Val_Cod IN (" & Agro_SQL_Save_Clausola_IN(String.Join(",", listaVal_Cod), True) & ")")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, sb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    '#####################################################################
    'A differenza della leggi (dati minimi) non fa il like sul val_cod
    'ma lo cerca tale e quale
    'inoltre c'è la possibilità di filtrare cod_progetto:
    'il default è -1, perchè 0 è significativo (ovvero codice legato all'impianto)
    'Questo in più prende i dati relativi
    '
    Public Function Leggi_ConDatiImpianto(
                              ByVal PIVA As String,
                              ByVal Sa_Cod As Int32,
                              ByVal Appezza As Int32,
                              ByVal Id_Reg As Int32,
                              ByVal Cul_Cod As Int32,
                              ByVal Progetto_Cod_meno1tutti As Int32,
                              ByVal Id_Cod As Int32,
                              ByVal Val_Cod As String,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByVal xOrderBy As String,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                   ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R.Leggi_2()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = "" 
        '   Sa_Cod = 0 
        '   Appezza = 0 
        '   Id_Reg = 0
        '   Veg_Cod = -1
        '   Cul_Cod = 0
        '   Id_Cod = 0 
        '   Val_Cod = "" 
        '   Gruppo = "" 
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT Reg_Impianti_Codici.PIVA, Reg_Impianti_Codici.sa_cod, Reg_Impianti_Codici.appezza, Reg_Impianti_Codici.Id_Reg, Reg_Impianti.Sup_Imp, Reg_Impianti.cul_cod, Reg_Impianti_Codici.id_cod, Reg_Impianti_Codici.val_cod, Reg_Impianti.Validita_Inizio, Reg_Impianti.Validita_Fine, Reg_Impianti.Sup_Imp ")
            StrSQL.Append(" FROM  Reg_Impianti_Codici ")
            StrSQL.Append("  inner join Reg_impianti on Reg_Impianti_Codici.PIVA = Reg_impianti.PIVA and Reg_Impianti_Codici.sa_cod = Reg_Impianti.sa_cod and Reg_Impianti_Codici.appezza =  Reg_Impianti.appezza and Reg_Impianti_Codici.Id_Reg = Reg_Impianti.Id_Reg")
            StrSQL.Append(" WHERE Reg_Impianti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If PIVA <> "" Then
                StrSQL.Append(" AND Reg_Impianti_Codici.Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Reg_Impianti_Codici.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Appezza <> 0 Then
                StrSQL.Append(" AND Reg_Impianti_Codici.Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If

            If Id_Reg <> 0 Then
                StrSQL.Append(" AND Reg_Impianti_Codici.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & " ")
            End If

            'If Veg_Cod_meno1tutti <> -1 Then
            '    StrSQL.Append(" AND Reg_Impianti.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod_meno1tutti) & " ")
            'End If

            If Cul_Cod <> 0 Then
                StrSQL.Append(" AND Reg_Impianti.cul_cod = " & Agro_SQL_SaveNum(Cul_Cod) & " ")
            End If

            If Progetto_Cod_meno1tutti <> -1 Then
                StrSQL.Append(" AND Reg_Impianti_Codici.Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod_meno1tutti) & " ")
            End If

            If Id_Cod <> 0 Then
                StrSQL.Append(" AND Reg_Impianti_Codici.Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")
            End If

            If Val_Cod <> "" Then
                StrSQL.Append(" AND Reg_Impianti_Codici.Val_Cod = '" & Agro_SQL_SaveText(Val_Cod) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND     Reg_Impianti_Codici.Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND     Reg_Impianti_Codici.Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Reg_Impianti_Codici.Piva, Reg_Impianti_Codici.Sa_Cod, Reg_Impianti_Codici.Appezza, Reg_Impianti_Codici.Id_reg, Reg_Impianti_Codici.Id_Cod ASC ")
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

    Public Function LeggiValCod_2(
                            ByVal PIVA As String,
                            ByVal Sa_Cod As Int32,
                            ByVal Appezza As Int32,
                            ByVal Id_Reg As Int32,
                            ByVal Progetto_Cod As Int32,
                            ByVal Id_Cod As Int32,
                            ByVal PermettiPiuRighe As Boolean,
                                 ByVal xFiltroAggiuntivo As String,
                                 ByVal xOrderBy As String,
                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                 ) As String


        Dim dt As DataTable = Leggi_2(PIVA, Sa_Cod, Appezza, Id_Reg, Progetto_Cod, Id_Cod, "", xFiltroAggiuntivo, xOrderBy, objParametri)

        If Not PermettiPiuRighe AndAlso dt.Rows.Count > 1 Then
            Throw New Exception("LeggiValCod_2, dt.Rows.Count > 1 ")
        End If

        If dt.Rows.Count > 0 Then
            Return dt.Rows(0).Item("Val_Cod")
        End If


        Return ""

    End Function



    Public Function LeggixProgetto(ByVal PIVA As String,
                                   ByVal Sa_Cod As Int32,
                                   ByVal Appezza As Int32,
                                   ByVal Id_Reg As Int32,
                                   ByVal Gruppo As String,
                                   ByVal Progetto_Cod As Int32,
                                   ByVal Id_Cod As Int32,
                                   ByVal Val_Cod As String,
                                   ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByVal xOrderBy As String,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                   ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R.LeggixProgetto()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = "" 
        '   Sa_Cod = 0 
        '   Appezza = 0 
        '   Id_Reg = 0
        '   Id_Cod = 0 
        '   Progetto_Cod = 0 
        '   Val_Cod = "" 
        '   Gruppo = ""         '
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    '---------------------------------------------
                    StrSQL.Length = 0
                    StrSQL.AppendLine("SELECT id_cod, val_cod, progetto_cod")
                    StrSQL.AppendLine("FROM Reg_Impianti_Codici")
                    StrSQL.AppendLine("WHERE Reg_Impianti_Codici.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "")
                    StrSQL.AppendLine("    AND Reg_Impianti_Codici.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "")
                    StrSQL.AppendLine("    AND Reg_Impianti_Codici.Progetto_Cod <> 0")

                    If PIVA <> "" Then
                        StrSQL.AppendLine("    AND Reg_Impianti_Codici.Piva = '" & Agro_SQL_SaveText(PIVA) & "'")
                    End If

                    If Progetto_Cod <> 0 Then
                        StrSQL.AppendLine("    AND Reg_Impianti_Codici.Progetto_Cod = " & Progetto_Cod & "")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.AppendLine("    AND Reg_Impianti_Codici.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "")
                    End If

                    If Appezza <> 0 Then
                        StrSQL.AppendLine("    AND Reg_Impianti_Codici.Appezza = " & Agro_SQL_SaveNum(Appezza) & "")
                    End If

                    If Id_Reg <> 0 Then
                        StrSQL.AppendLine("    AND Reg_Impianti_Codici.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & "")
                    End If

                    If Id_Cod <> 0 Then
                        StrSQL.AppendLine("    AND Reg_Impianti_Codici.Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & "")
                    End If

                    If Trim(Val_Cod) <> "" Then
                        StrSQL.AppendLine("    AND Reg_Impianti_Codici.Val_Cod Like '%" & Agro_SQL_SaveText(Val_Cod) & "%'")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine("    AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine("    AND Reg_Impianti_Codici.Inviato >= 0")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine("    AND Reg_Impianti_Codici.Inviato = -1")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        'StrSQL.Append(" ORDER BY Reg_Impianti_Codici.Piva, Reg_Impianti_Codici.Sa_Cod, Reg_Impianti_Codici.Appezza, Reg_Impianti_Codici.Id_reg, Reg_Impianti_Codici.Id_Cod ASC ")
                    End If


                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    '---------------------------------------------
                    StrSQL.Length = 0
                    StrSQL.AppendLine("SELECT Distinct *, Imprese_Progetti.Validita_Inizio as xValidita_Inizio, Imprese_Progetti.Validita_Fine as xValidita_Fine")
                    StrSQL.AppendLine("FROM  Imprese_Progetti, Reg_Impianti_Codici, Codici_Anagrafe ")
                    StrSQL.AppendLine("WHERE Imprese_Progetti.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "")
                    StrSQL.AppendLine("    AND Imprese_Progetti.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "")
                    StrSQL.AppendLine("    AND Codici_Anagrafe.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "")
                    StrSQL.AppendLine("    AND Codici_Anagrafe.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "")
                    StrSQL.AppendLine("    AND Reg_Impianti_Codici.Piva = Imprese_Progetti.Piva")
                    StrSQL.AppendLine("    AND Reg_Impianti_Codici.Sa_Cod = Imprese_Progetti.Sa_Cod")
                    StrSQL.AppendLine("    AND Reg_Impianti_Codici.Appezza = Imprese_Progetti.Appezza")
                    StrSQL.AppendLine("    AND Reg_Impianti_Codici.Id_Reg = Imprese_Progetti.Id_Reg")
                    StrSQL.AppendLine("    AND Reg_Impianti_Codici.Progetto_Cod = Imprese_Progetti.Progetto_Cod")
                    StrSQL.AppendLine("    AND Reg_Impianti_Codici.Id_Cod = Codici_Anagrafe.Codice")
                    StrSQL.AppendLine("    AND Reg_Impianti_Codici.Progetto_Cod <> 0")

                    If PIVA <> "" Then
                        StrSQL.AppendLine("    AND Reg_Impianti_Codici.Piva = '" & Agro_SQL_SaveText(PIVA) & "'")
                    End If

                    If Progetto_Cod <> 0 Then
                        StrSQL.AppendLine("    AND Reg_Impianti_Codici.Progetto_Cod = " & Progetto_Cod & "")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.AppendLine("    AND Reg_Impianti_Codici.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "")
                    End If

                    If Appezza <> 0 Then
                        StrSQL.AppendLine("    AND Reg_Impianti_Codici.Appezza = " & Agro_SQL_SaveNum(Appezza) & "")
                    End If

                    If Id_Reg <> 0 Then
                        StrSQL.AppendLine("    AND Reg_Impianti_Codici.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & "")
                    End If

                    If Id_Cod <> 0 Then
                        StrSQL.AppendLine("    AND Reg_Impianti_Codici.Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & "")
                    End If

                    If Trim(Val_Cod) <> "" Then
                        StrSQL.AppendLine("    AND Reg_Impianti_Codici.Val_Cod Like '%" & Agro_SQL_SaveText(Val_Cod) & "%'")
                    End If

                    If Gruppo <> "" Then
                        StrSQL.AppendLine("    AND Codici_Anagrafe.Gruppo = '" & Agro_SQL_SaveText(Trim(Gruppo)) & "'")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine("    AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine("    AND Reg_Impianti_Codici.Inviato >= 0")
                            StrSQL.AppendLine("    AND Codici_Anagrafe.Inviato >= 0")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine("    AND Reg_Impianti_Codici.Inviato = -1")
                            StrSQL.AppendLine("    AND Codici_Anagrafe.Inviato = -1")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine("ORDER BY Reg_Impianti_Codici.Piva, Reg_Impianti_Codici.Sa_Cod, Reg_Impianti_Codici.Appezza, Reg_Impianti_Codici.Id_reg, Reg_Impianti_Codici.Id_Cod ASC ")

                    End If


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni



                Case enumSelezioneVariabile.Selezione_JoinCompleta


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


    '##############################################################################################
    Public Function LeggixGruppoAnagrafe(
                                    ByVal Piva As String,
                                    ByVal Sa_Cod As Int32,
                                    ByVal Appezza As Int32,
                                    ByVal Id_Reg As Int32,
                                    ByVal Id_Cod As Int32,
                                    ByVal GruppoAnagrafe As String,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As DataTable

        Dim NomeRoutine As String = "AnagrafeDAL.Reg_Impianti_Codici_R.LeggixGruppoAnagrafe()"

        '====================================================================================
        'Parametri opzionali :
        '   Sa_Cod = 0           =>  si leggono tutti gli impianti dell'impresa
        '   Appezza = 0          =>  si leggono tutti gli impianti del centro aziendale
        '   Id_reg = 0           =>  si leggono tutti gli impianti dell'appezzamento
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

            StrSQL.Append(" SELECT DISTINCT Reg_Impianti_Codici.Piva, Reg_Impianti_Codici.Sa_Cod, Reg_Impianti_Codici.Appezza, Reg_Impianti_Codici.Id_Reg, Reg_Impianti_Codici.id_cod, Codici_Anagrafe.descrizione ")
            StrSQL.Append(" FROM Reg_Impianti_Codici, Codici_Anagrafe ")
            StrSQL.Append(" WHERE Reg_Impianti_Codici.id_cod = Codici_Anagrafe.codice ")

            StrSQL.Append(" AND   (Reg_Impianti_Codici.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
            StrSQL.Append(" AND     (Reg_Impianti_Codici.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")


            If (Piva <> "") Then
                StrSQL.Append(" AND Reg_Impianti_Codici.PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Reg_Impianti_Codici.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            End If


            If Appezza <> 0 Then
                StrSQL.Append(" AND Reg_Impianti_Codici.Appezza = " & Agro_SQL_SaveNum(Appezza) & "  ")
            End If

            If Id_Reg <> 0 Then
                StrSQL.Append(" AND Reg_Impianti_Codici.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & "  ")
            End If

            If Id_Cod <> 0 Then
                StrSQL.Append(" AND Reg_Impianti_Codici.id_cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")
            End If

            If GruppoAnagrafe <> "" Then
                StrSQL.Append(" AND Codici_Anagrafe.gruppo = '" & Agro_SQL_SaveText(GruppoAnagrafe) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Reg_Impianti_Codici.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Reg_Impianti_Codici.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
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


    '###################################################################################
    Public Function Esiste_ImpiantoCodice(ByVal Piva As String,
                                                ByVal Sa_Cod As Integer,
                                                ByVal Appezza As String,
                                                ByVal Id_Reg As Integer,
                                                ByVal Id_Cod As Integer,
                                                ByVal Val_Cod As String,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici.Esiste_ImpiantoCodice()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Flag_Esiste As Boolean = False

        Try

            DT = Leggi(Piva,
                            Sa_Cod,
                            Appezza,
                            Id_Reg,
                             "",
                             Id_Cod,
                            Val_Cod,
                            enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                              "", "",
                              objParametri)

            If DT IsNot Nothing AndAlso DT.Rows.Count > 0 Then
                Flag_Esiste = True
            End If

            DT = Nothing

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Flag_Esiste

    End Function


    '###################################################################################
    Public Function EsisteCodice_RecuperaDatiImpianto(ByVal Piva As String,
                                                        ByVal Id_Cod As Integer,
                                                        ByVal Val_Cod As String,
                                                        ByRef Sa_Cod As Integer,
                                                        ByRef Appezza As String,
                                                        ByRef Id_Reg As Integer,
                                                        ByVal xFiltroAggiuntivo As String,
                                                        ByVal xOrderBy As String,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici.EsisteCodice_RecuperaDatiimpianto()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Flag_Esiste As Boolean = False

        Try

            DT = Leggi_2(Piva,
                            0,
                            0,
                            0,
                            -1,
                            Id_Cod,
                            Val_Cod,
                           "", "",
                              objParametri)

            If DT IsNot Nothing AndAlso DT.Rows.Count > 0 Then
                Flag_Esiste = True
                Sa_Cod = DT.Rows(0).Item("Sa_Cod")
                Appezza = DT.Rows(0).Item("Appezza")
                Id_Reg = DT.Rows(0).Item("Id_Reg")
            End If

            DT = Nothing

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Flag_Esiste

    End Function

    Public Function EsisteCodice_RecuperaDatiImpianto_2(ByVal Piva As String,
                                ByVal val_cod As String,
                                ByVal id_cod As Integer,
                                ByRef Data_Modifica As Date,
                                ByRef App_Nome As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                Optional ByRef Sup_Imp As Decimal = 0.0,
                                Optional ByRef Cul_Cod As Integer = -1) As String


        Dim NomeRoutine As String = "AnagrafeDAL.Reg_Impianti_Codici_R.EsisteCodice_RecuperaDatiImpianto_2()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.Append(" SELECT Reg_Impianti.*, Reg_Impianti_Codici.id_cod, Reg_Impianti_Codici.val_cod, Appezzamento.APP_NOME, Appezzamento.Campo_Cod ")
            StrSQL.Append(" FROM Reg_Impianti, Reg_Impianti_Codici, Appezzamento ")
            StrSQL.Append(" WHERE Reg_Impianti_Codici.PIVA = Reg_Impianti.PIVA AND Reg_Impianti_Codici.sa_cod = Reg_Impianti.SA_COD ")
            StrSQL.Append(" AND     Reg_Impianti_Codici.appezza = Reg_Impianti.APPEZZA And Reg_Impianti_Codici.Id_Reg = Reg_Impianti.ID_REG ")
            StrSQL.Append(" AND     Appezzamento.PIVA = Reg_Impianti.PIVA AND Appezzamento.sa_cod = Reg_Impianti.SA_COD AND Appezzamento.appezza = Reg_Impianti.appezza ")

            StrSQL.Append(" AND Reg_Impianti_Codici.PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            StrSQL.Append(" AND Reg_Impianti_Codici.id_cod = " & Agro_SQL_SaveNum(id_cod) & " ")
            StrSQL.Append(" AND Reg_Impianti_Codici.val_cod = '" & Agro_SQL_SaveText(val_cod) & "' ")


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Reg_Impianti_Codici.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Reg_Impianti_Codici.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT IsNot Nothing AndAlso DT.Rows.Count > 0 Then

                Data_Modifica = CDate(DT.Rows(0).Item("data_modifica"))
                App_Nome = DT.Rows(0).Item("app_nome")
                Sup_Imp = DT.Rows(0).Item("Sup_Imp")
                Cul_Cod = DT.Rows(0).Item("Cul_Cod")

                Return CStr(DT.Rows(0).Item("appezza")) & "/" & CStr(DT.Rows(0).Item("id_reg")) & "/" & CStr(DT.Rows(0).Item("campo_cod")) & "/" & Format(Sup_Imp, "0.0000") & "/" & CStr(Cul_Cod)

            Else

                Data_Modifica = Nothing
                App_Nome = ""

                Return "0"

            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
            Data_Modifica = Nothing
            App_Nome = ""
            Return "0"
        End Try

    End Function

    Public Function Esiste_DestinazioneUso(ByVal Piva As String,
                                           ByVal Sa_Cod As Integer,
                                           ByVal Appezza As Integer,
                                           ByVal IdImp As Integer,
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                           ) As Boolean

        Dim dt As DataTable
        Dim i As Integer
        Dim strFiltro As String = ""

        For i = 3000 To 3999
            If strFiltro = "" Then
                strFiltro = " id_cod=" & i & " "
            Else
                strFiltro = strFiltro & " OR id_cod = " & i & " "
            End If
        Next

        dt = Leggi(CStr(Piva),
                   CInt(Sa_Cod),
                   CInt(Appezza),
                   CInt(IdImp),
                   "", 0, "",
                   enumSelezioneVariabile.Selezione_TabellaCompleta,
                   " (" & strFiltro & ") ",
                   "",
                   objParametri)

        If dt IsNot Nothing Then

            If dt.Rows.Count > 0 Then

                Return True
            Else
                Return False
            End If
        Else
            Return False
        End If

    End Function

    'ritorna la descrizione
    Public Function Esiste_DestinazioneUso_2(ByVal Piva As String,
                                        ByVal Sa_Cod As Integer,
                                        ByVal Appezza As Integer,
                                        ByVal IdImp As Integer,
                                        ByRef Descrizione As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) _
                                        As Boolean

        Dim strFiltro As String = "id_cod >= 3000 AND id_cod <=3999"

        'leggo i codici..
        Dim Dt As DataTable = Leggi(CStr(Piva),
                            CInt(Sa_Cod),
                            CInt(Appezza),
                            CInt(IdImp),
                            "", 0, "",
                            AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                            " (" & strFiltro & ") ",
                            "",
                            objParametri)

        If Dt IsNot Nothing Then

            If Dt.Rows.Count > 0 Then
                Descrizione = Dt.Rows(0).Item("descrizione")
                Return True
            Else
                Return False
            End If
        Else
            Return False
        End If

    End Function

    '################################################################################
    Public Function Leggi_Codici_ACA_from_Contributi(ByVal Piva As String,
                                                          ByVal SaCod As Integer,
                                                          ByVal Appezza As Integer,
                                                          ByVal IdReg As Integer,
                                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String()
        Dim result As New List(Of String)
        Dim Dt As DataTable

        'Recupero le informazioni		
        Dt = Leggi_Con_CodiciACA(CStr(Piva),
                          CInt(SaCod),
                          CInt(Appezza),
                          CInt(IdReg),
                          -1,
                          "", "",
                          objParametri)

        If Dt IsNot Nothing AndAlso Dt.Rows.Count > 0 Then
            result = Dt.AsEnumerable() _
                .Where(Function(x) Not IsDBNull(x("Nr_Domanda_ACA")) AndAlso
                    Not String.IsNullOrEmpty(x("Nr_Domanda_ACA"))) _
                .Select(Function(x) x("Nr_Domanda_ACA").ToString()) _
                .Distinct() _
                .ToList()
        End If

        Return result.ToArray()
    End Function

    '################################################################################
    Public Function Leggi_Codice_from_Reg_Impianti_Codici(ByRef Piva As String,
                                                          ByRef SaCod As Integer,
                                                          ByRef Appezza As Integer,
                                                          ByRef IdReg As Integer,
                                                          ByRef IdCod As Integer,
                                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim Dt As DataTable

        'Recupero le informazioni		
        Dt = Leggi_2(CStr(Piva),
                          CInt(SaCod),
                          CInt(Appezza),
                          CInt(IdReg),
                          0,
                          CInt(IdCod), "", "", "",
                          objParametri)


        If Dt IsNot Nothing AndAlso Dt.Rows.Count > 0 Then
            Return Dt.Rows(0).Item("val_cod")
        Else
            Return ""
        End If

    End Function

    '################################################################################
    Public Function Leggi_Codice_from_Reg_Impianti_Codici_Distinta(ByRef Piva As String,
                                                          ByRef SaCod As Integer,
                                                          ByRef Appezza As Integer,
                                                          ByRef IdReg As Integer,
                                                          ByRef IdCod As Integer,
                                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                        Optional ByVal progetto_cod As Integer = -1) As String

        Dim Dt As DataTable

        'Recupero le informazioni		
        Dt = Leggi_2(CStr(Piva),
                          CInt(SaCod),
                          CInt(Appezza),
                          CInt(IdReg),
                          CInt(progetto_cod),
                          CInt(IdCod), "", "", "",
                          objParametri)


        If Dt IsNot Nothing AndAlso Dt.Rows.Count > 0 Then

            Return Dt.Rows(0).Item("val_cod")

        Else

            Return ""

        End If


    End Function
    '#############################################################################################################
    Public Sub Leggi_ApportiMassimiMacroelementi(ByVal Piva As String,
                                                 ByVal SaCod As Integer,
                                                 ByVal Appezza As Integer,
                                                 ByVal IdReg As Integer,
                                                 ByVal Progetto_Cod As Integer,
                                                 ByRef N As String,
                                                 ByRef P2O5 As String,
                                                 ByRef K2O As String,
                                                 ByRef MgO As String,
                                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim Dt As DataTable

        Dt = LeggixProgetto(CStr(Piva),
                                    CInt(SaCod),
                                    CInt(Appezza),
                                    CInt(IdReg),
                                    "",
                                    CInt(Progetto_Cod),
                                    0, "", enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri)


        If Not IsNothing(Dt) AndAlso Dt.Rows.Count <> 0 Then
            Dim Dr() As DataRow

            'Azoto
            Dr = Dt.Select("id_cod = 1050")
            'Rs.Filter = "id_cod = 1050"
            If Not IsNothing(Dr) AndAlso Dr.Length > 0 Then
                If IsNumeric(Dr(0).Item("val_cod")) Then
                    N = Dr(0).Item("val_cod")
                End If
            End If

            'Potassio
            'Rs.Filter = "id_cod = 1051"
            'If Not Rs.EOF Then
            '    If IsNumeric(Rs.Fields("val_cod").Value) Then
            '        P2O5 = Rs.Fields("val_cod").Value
            '    End If
            'End If
            Dr = Dt.Select("id_cod = 1051")
            If Not IsNothing(Dr) AndAlso Dr.Length > 0 Then
                If IsNumeric(Dr(0).Item("val_cod")) Then
                    P2O5 = Dr(0).Item("val_cod")
                End If
            End If

            'Fosforo
            'Rs.Filter = "id_cod = 1052"
            'If Not Rs.EOF Then
            '    If IsNumeric(Rs.Fields("val_cod").Value) Then
            '        K2O = Rs.Fields("val_cod").Value
            '    End If
            'End If
            Dr = Dt.Select("id_cod = 1052")
            If Not IsNothing(Dr) AndAlso Dr.Length > 0 Then
                If IsNumeric(Dr(0).Item("val_cod")) Then
                    K2O = Dr(0).Item("val_cod")
                End If
            End If

            'Magnesio
            'Rs.Filter = "id_cod = 1053"
            'If Not Rs.EOF Then
            '    If IsNumeric(Rs.Fields("val_cod").Value) Then
            '        MgO = Rs.Fields("val_cod").Value
            '    End If
            'End If
            Dr = Dt.Select("id_cod = 1053")
            If Not IsNothing(Dr) AndAlso Dr.Length > 0 Then
                If IsNumeric(Dr(0).Item("val_cod")) Then
                    MgO = Dr(0).Item("val_cod")
                End If
            End If


        End If

    End Sub



    '#####################################################################
    Public Function Esiste_Impianto(
                                      ByVal Piva As String,
                                      ByVal Id_Cod As Int32,
                                      ByVal Val_Cod As String,
                                      ByRef appezza As Int32,
                                      ByRef id_reg As Int32,
                                      ByRef campo_cod As Int32,
                                      ByRef Sup_Imp As Decimal,
                                      ByRef Cul_Cod As Int32,
                                      ByRef Data_Modifica As Date,
                                      ByRef App_Nome As String,
                                      ByVal xFiltroAggiuntivo As String,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                   ) As Integer

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R.Esiste_Impianto()"

        '====================================================================================


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT  Reg_Impianti.*, Reg_Impianti_Codici.id_cod, Reg_Impianti_Codici.val_cod, Appezzamento.APP_NOME, Appezzamento.Campo_Cod ")
            StrSQL.Append(" FROM    Reg_Impianti, Reg_Impianti_Codici, Appezzamento ")
            StrSQL.Append(" WHERE   Reg_Impianti_Codici.PIVA = Reg_Impianti.PIVA AND Reg_Impianti_Codici.sa_cod = Reg_Impianti.SA_COD ")
            StrSQL.Append(" AND     Reg_Impianti_Codici.appezza = Reg_Impianti.APPEZZA And Reg_Impianti_Codici.Id_Reg = Reg_Impianti.ID_REG ")
            StrSQL.Append(" AND     Appezzamento.PIVA = Reg_Impianti.PIVA AND Appezzamento.sa_cod = Reg_Impianti.SA_COD AND Appezzamento.appezza = Reg_Impianti.appezza ")
            StrSQL.Append(" AND     Reg_Impianti_Codici.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            If Id_Cod <> 0 Then
                StrSQL.Append(" AND Reg_Impianti_Codici.Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")
            End If

            If Val_Cod <> "" Then
                StrSQL.Append(" AND Reg_Impianti_Codici.Val_Cod = '" & Agro_SQL_SaveText(Val_Cod) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND     Reg_Impianti_Codici.Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND     Reg_Impianti_Codici.Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            StrSQL.Append(" ORDER BY  Reg_Impianti_Codici.Data_Creazione desc ")
            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


            If DT.Rows.Count = 0 Then

                appezza = 0
                id_reg = 0
                campo_cod = 0
                Sup_Imp = 0
                Cul_Cod = 0
                Data_Modifica = Nothing
                App_Nome = ""

            ElseIf DT.Rows.Count > 0 Then

                appezza = DT.Rows(0).Item("appezza")
                id_reg = DT.Rows(0).Item("id_reg")
                campo_cod = DT.Rows(0).Item("campo_cod")
                Sup_Imp = DT.Rows(0).Item("Sup_Imp")
                Cul_Cod = DT.Rows(0).Item("Cul_Cod")
                Data_Modifica = CDate(DT.Rows(0).Item("data_modifica"))
                App_Nome = DT.Rows(0).Item("app_nome")


            End If

            Return DT.Rows.Count

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return False

    End Function



    '#####################################################################
    'in realtà è sulla distinta, visto che si passa Progetto_Cod
    Public Function Leggi_OrgReferente_SuImpianto(
                              ByVal PIVA As String,
                              ByVal Sa_Cod As Int32,
                              ByVal Appezza As Int32,
                              ByVal Id_Reg As Int32,
                              ByVal Progetto_Cod As Int32,
                              ByVal Val_cod As String,
                              ByVal Data_Inizio As Date,
                              ByVal Data_Fine As Date,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByVal xOrderBy As String,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                   ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R.Leggi_OrgReferente_SuImpianto()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT Reg_Impianti_Codici.PIVA, Reg_Impianti_Codici.sa_cod, Reg_Impianti_Codici.appezza, Reg_Impianti_Codici.Id_Reg, id_cod, val_cod, rag_soc")
            StrSQL.Append(" FROM  Reg_Impianti_Codici ")
            StrSQL.Append(" INNER JOIN Contatti ON Reg_Impianti_Codici.VAL_COD = Contatti.cod_Contatto ")
            StrSQL.Append(" INNER JOIN Imprese_Progetti ON Reg_Impianti_Codici.piva = Imprese_Progetti.piva ")
            StrSQL.Append(" AND Reg_Impianti_Codici.sa_cod = Imprese_Progetti.sa_cod ")
            StrSQL.Append(" AND Reg_Impianti_Codici.appezza = Imprese_Progetti.appezza ")
            StrSQL.Append(" AND Reg_Impianti_Codici.id_reg = Imprese_Progetti.id_reg ")
            StrSQL.Append(" AND Reg_Impianti_Codici.Progetto_Cod = Imprese_Progetti.Progetto_Cod ")

            StrSQL.Append(" WHERE Reg_Impianti_Codici.Id_Cod = " & Agro_SQL_SaveNum(enum_CodiciAnagrafe.Organismo_Referente) & " ")
            StrSQL.Append(" AND Imprese_Progetti.Validita_inizio <= " & Agro_SQL_SaveDate(Data_Fine) & " ")
            StrSQL.Append(" AND   Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")

            If PIVA <> "" Then
                StrSQL.Append(" AND Reg_Impianti_Codici.Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Reg_Impianti_Codici.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Appezza <> 0 Then
                StrSQL.Append(" AND Reg_Impianti_Codici.Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If

            If Id_Reg <> 0 Then
                StrSQL.Append(" AND Reg_Impianti_Codici.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & " ")
            End If

            If Progetto_Cod <> 0 Then
                StrSQL.Append(" AND Reg_Impianti_Codici.Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod) & " ")
            End If

            If Val_cod <> "" Then
                StrSQL.Append(" AND Reg_Impianti_Codici.Val_Cod = '" & Agro_SQL_SaveText(Val_cod) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND     Reg_Impianti_Codici.Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND     Reg_Impianti_Codici.Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY rag_soc ")
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

    '#####################################################################
    Public Function Leggi_Contatti_SuImpianto_SenzaDistinta(
        ByVal idCod As enum_CodiciAnagrafe,
        ByVal PIVA As String,
        ByVal Sa_Cod As Int32,
        ByVal Appezza As Int32,
        ByVal Id_Reg As Int32,
        ByVal Progetto_Cod As Int32,
        ByVal Val_cod As String,
        ByVal Data_Inizio As Date,
        ByVal Data_Fine As Date,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R.Leggi_Contatti_SuImpianto_SenzaDistinta()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT Reg_Impianti_Codici.PIVA, Reg_Impianti_Codici.sa_cod, Reg_Impianti_Codici.appezza, Reg_Impianti_Codici.Id_Reg, id_cod, val_cod, rag_soc")
            StrSQL.Append(" FROM  Reg_Impianti_Codici ")
            StrSQL.Append(" INNER JOIN Contatti ON Reg_Impianti_Codici.VAL_COD = Contatti.cod_Contatto ")
            StrSQL.Append(" INNER JOIN Reg_Impianti ON Reg_Impianti_Codici.piva = Reg_Impianti.piva ")
            StrSQL.Append(" AND Reg_Impianti_Codici.sa_cod = Reg_Impianti.sa_cod ")
            StrSQL.Append(" AND Reg_Impianti_Codici.appezza = Reg_Impianti.appezza ")
            StrSQL.Append(" AND Reg_Impianti_Codici.id_reg = Reg_Impianti.id_reg ")


            StrSQL.Append(" WHERE Reg_Impianti_Codici.Id_Cod = " & Agro_SQL_SaveNum(idCod) & " ")
            StrSQL.Append(" AND Reg_Impianti.Validita_inizio <= " & Agro_SQL_SaveDate(Data_Fine) & " ")
            StrSQL.Append(" AND   Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")

            If PIVA <> "" Then
                StrSQL.Append(" AND Reg_Impianti_Codici.Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Reg_Impianti_Codici.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Appezza <> 0 Then
                StrSQL.Append(" AND Reg_Impianti_Codici.Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If

            If Id_Reg <> 0 Then
                StrSQL.Append(" AND Reg_Impianti_Codici.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & " ")
            End If

            If Progetto_Cod <> 0 Then
                StrSQL.Append(" AND Reg_Impianti_Codici.Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod) & " ")
            End If

            If Val_cod <> "" Then
                StrSQL.Append(" AND Reg_Impianti_Codici.Val_Cod = '" & Agro_SQL_SaveText(Val_cod) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND     Reg_Impianti_Codici.Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND     Reg_Impianti_Codici.Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY rag_soc ")
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

    '#####################################################################
    Public Function Leggi_OrgReferente_SuImpianto_SenzaDistinta(
                              ByVal PIVA As String,
                              ByVal Sa_Cod As Int32,
                              ByVal Appezza As Int32,
                              ByVal Id_Reg As Int32,
                              ByVal Progetto_Cod As Int32,
                              ByVal Val_cod As String,
                              ByVal Data_Inizio As Date,
                              ByVal Data_Fine As Date,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByVal xOrderBy As String,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                   ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R.Leggi_OrgReferente_SuImpianto()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT Reg_Impianti_Codici.PIVA, Reg_Impianti_Codici.sa_cod, Reg_Impianti_Codici.appezza, Reg_Impianti_Codici.Id_Reg, id_cod, val_cod, rag_soc")
            StrSQL.Append(" FROM  Reg_Impianti_Codici ")
            StrSQL.Append(" INNER JOIN Contatti ON Reg_Impianti_Codici.VAL_COD = Contatti.cod_Contatto ")
            StrSQL.Append(" INNER JOIN Reg_Impianti ON Reg_Impianti_Codici.piva = Reg_Impianti.piva ")
            StrSQL.Append(" AND Reg_Impianti_Codici.sa_cod = Reg_Impianti.sa_cod ")
            StrSQL.Append(" AND Reg_Impianti_Codici.appezza = Reg_Impianti.appezza ")
            StrSQL.Append(" AND Reg_Impianti_Codici.id_reg = Reg_Impianti.id_reg ")

            StrSQL.Append(" WHERE Reg_Impianti_Codici.Id_Cod = " & Agro_SQL_SaveNum(enum_CodiciAnagrafe.Organismo_Referente) & " ")
            StrSQL.Append(" AND Reg_Impianti.Validita_inizio <= " & Agro_SQL_SaveDate(Data_Fine) & " ")
            StrSQL.Append(" AND   Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")

            If PIVA <> "" Then
                StrSQL.Append(" AND Reg_Impianti_Codici.Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Reg_Impianti_Codici.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Appezza <> 0 Then
                StrSQL.Append(" AND Reg_Impianti_Codici.Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If

            If Id_Reg <> 0 Then
                StrSQL.Append(" AND Reg_Impianti_Codici.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & " ")
            End If

            If Progetto_Cod <> 0 Then
                StrSQL.Append(" AND Reg_Impianti_Codici.Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod) & " ")
            End If

            If Val_cod <> "" Then
                StrSQL.Append(" AND Reg_Impianti_Codici.Val_Cod = '" & Agro_SQL_SaveText(Val_cod) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND     Reg_Impianti_Codici.Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND     Reg_Impianti_Codici.Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY rag_soc ")
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

    '###################################################################################
    Public Sub OrgReferentePivaRagSoc_from_Impianto(ByVal PIVA As String,
                                              ByVal Sa_Cod As Int32,
                                              ByVal Appezza As Int32,
                                              ByVal Id_Reg As Int32,
                                              ByVal Progetto_Cod As Int32,
                                              ByVal Data_Inizio As Date,
                                              ByVal Data_Fine As Date,
                                                   ByVal xFiltroAggiuntivo As String,
                                                   ByVal xOrderBy As String,
                                                   ByRef Piva_OrgRef As String,
                                                   ByRef RagSoc_OrgRef As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                )

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici.OrgReferente_from_impianto()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Piva_OrgRef = ""
        RagSoc_OrgRef = ""

        Try

            DT = Leggi_OrgReferente_SuImpianto(PIVA,
                            Sa_Cod,
                            Appezza,
                            Id_Reg,
                             Progetto_Cod,
                             "",
                             Data_Inizio,
                             Data_Fine,
                            xFiltroAggiuntivo,
                             xOrderBy,
                              objParametri)

            If DT IsNot Nothing AndAlso DT.Rows.Count > 0 Then
                Piva_OrgRef = DT.Rows(0).Item("val_cod")
                RagSoc_OrgRef = DT.Rows(0).Item("rag_soc")
            End If

            DT = Nothing

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


    End Sub

    '###################################################################################

    Public Function CaricaList_PianiSemina(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici.CaricaList_PianiSemina()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Try
            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT DISTINCT val_cod ")
            StrSQL.AppendLine(" FROM Reg_Impianti_Codici ")
            StrSQL.AppendLine(" WHERE (id_cod = 1071) ")
            StrSQL.AppendLine(" ORDER BY val_cod ")

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

    Public Function ChiaveImpianto_From_ChiaveAGEA_Massivo(ByVal ChiaviAGEA As List(Of String),
                                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                           Optional estraiEsercizio As Boolean = False
                                                           ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Codici_Read.CUAA_from_Piva_Massivo()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            ConnessioniTransazioni.ApriConnessione(True, objParametri)

            TempChiaviMassivo.CreaTabellaTemp_FiltroChiaveStringa(ChiaviAGEA, NomeRoutine, objParametri, varcharSize:=255)

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SELECT  ")
            StrSQL.Append("   Reg_Impianti_Codici.PIVA ")
            StrSQL.Append(" , Reg_Impianti_Codici.Sa_Cod ")
            StrSQL.Append(" , Reg_Impianti_Codici.Appezza ")
            StrSQL.Append(" , Reg_Impianti_Codici.Id_Reg ")
            If estraiEsercizio Then
                StrSQL.Append(" , Imprese_Progetti.Progetto_Cod ")
            Else
                StrSQL.Append(" , 0 AS Progetto_Cod ")
            End If
            StrSQL.Append(" , Reg_Impianti_Codici.Val_Cod AS Chiave_AGEA ")
            StrSQL.Append(" FROM Reg_Impianti_Codici ")
            If ChiaviAGEA IsNot Nothing AndAlso ChiaviAGEA.Count > 0 Then
                StrSQL.AppendLine(" INNER JOIN #TempChiave temp (NOLOCK) ON  ")
                StrSQL.AppendLine("     temp.chiave = Reg_Impianti_Codici.val_cod ")
                StrSQL.AppendLine($" AND Reg_Impianti_Codici.Id_Cod = {CInt(enum_CodiceAnagrafe_Clienti.Demetra)} ")
            End If

            'AF 09/25 join con reg_impianti per filtrare su flagCessata = 0
            StrSQL.AppendLine(" INNER JOIN Reg_Impianti (NOLOCK) ON  ")
            StrSQL.AppendLine("     Reg_Impianti.Piva = Reg_Impianti_Codici.Piva ")
            StrSQL.AppendLine(" AND Reg_Impianti.Sa_Cod = Reg_Impianti_Codici.Sa_Cod ")
            StrSQL.AppendLine(" AND Reg_Impianti.Appezza = Reg_Impianti_Codici.Appezza ")
            StrSQL.AppendLine(" AND Reg_Impianti.Id_Reg = Reg_Impianti_Codici.Id_Reg ")
            StrSQL.AppendLine(" AND ISNULL(Reg_Impianti.flagCessata, 0) = 0 ")

            If estraiEsercizio Then
                'Per convenzione gli impianti Demetra-NewAgri hanno 1 appezzammento, con 1 impianto, con 1 esercizio
                'Quindi non ho bisogno di fare filtri sulle date per andare in join con Imprese_Progetti
                StrSQL.AppendLine(" INNER JOIN Imprese_Progetti (NOLOCK) ON  ")
                StrSQL.AppendLine("     Imprese_Progetti.Piva = Reg_Impianti_Codici.Piva ")
                StrSQL.AppendLine(" AND Imprese_Progetti.Sa_Cod = Reg_Impianti_Codici.Sa_Cod ")
                StrSQL.AppendLine(" AND Imprese_Progetti.Appezza = Reg_Impianti_Codici.Appezza ")
                StrSQL.AppendLine(" AND Imprese_Progetti.Id_Reg = Reg_Impianti_Codici.Id_Reg ")
            End If

            StrSQL.Append(" WHERE 1 = 1 ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            ' Eliminazione tabella temporanea
            TempChiaviMassivo.EliminaTabellaTemp_FiltroChiaveStringa(NomeRoutine, objParametri)

            'commit transazione
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(1, objParametri)

        Catch ex As Exception
            ' Rollback
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri)
        End Try

        Return DT

    End Function

End Class



'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################


Public Class Reg_Impianti_Codici_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Scrivi(ByVal Piva As String,
                                ByVal Sa_Cod As Int32,
                                ByVal Appezza As Int32,
                                ByVal Id_Reg As Int32,
                                ByVal Id_Cod As Int32,
                                ByVal Val_Cod As String,
                                ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                    , Optional ByVal Data_creazione As DateTime = #2/1/1900# _
                    , Optional ByVal Data_modifica As DateTime = #2/1/1900# _
                    , Optional ByVal username_creazione As String = "" _
                    , Optional ByVal username_modifica As String = ""
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_W.Scrivi()"

        Dim MessaggioErrore As String = ""
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
            StrSQL.Append("INSERT INTO Reg_Impianti_Codici( ")
            StrSQL.Append("                    Piva,        ")
            StrSQL.Append("                    Sa_Cod,      ")
            StrSQL.Append("                    Appezza,      ")
            StrSQL.Append("                    Id_Reg,      ")
            StrSQL.Append("                    Id_Cod,      ")
            StrSQL.Append("                    Val_Cod,     ")
            StrSQL.Append("                    Inviato, DataInvio, ")
            StrSQL.Append("                    Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                    UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("                    Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("                    ) ")
            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Appezza) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Reg) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Cod) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Val_Cod) & "' ")
            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append(")")
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
    Public Function ScrivixProgetto(ByVal Piva As String,
                                        ByVal Sa_Cod As Int32,
                                        ByVal Appezza As Int32,
                                        ByVal Id_Reg As Int32,
                                        ByVal Progetto_Cod As Int32,
                                        ByVal Id_Cod As Int32,
                                        ByVal Val_Cod As String,
                                        ByVal Validita_Inizio As Date,
                                        ByVal Validita_Fine As Date,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            , Optional ByVal Data_creazione As DateTime = #2/1/1900# _
                            , Optional ByVal Data_modifica As DateTime = #2/1/1900# _
                            , Optional ByVal username_creazione As String = "" _
                            , Optional ByVal username_modifica As String = ""
                                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_W.ScrivixProgetto()"

        Dim MessaggioErrore As String = ""
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
            StrSQL.Append("INSERT INTO Reg_Impianti_Codici( ")
            StrSQL.Append("                    Piva,        ")
            StrSQL.Append("                    Sa_Cod,      ")
            StrSQL.Append("                    Appezza,      ")
            StrSQL.Append("                    Id_Reg,      ")
            StrSQL.Append("                    Progetto_Cod,")
            StrSQL.Append("                    Id_Cod,      ")
            StrSQL.Append("                    Val_Cod,     ")
            StrSQL.Append("                    Inviato, DataInvio, ")
            StrSQL.Append("                    Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                    UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("                    Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("                    ) ")
            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Appezza) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Reg) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Progetto_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Cod) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Val_Cod) & "' ")
            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append(")")
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
    Public Function Modifica(ByVal Piva As String,
                                ByVal Sa_Cod As Int32,
                                ByVal Appezza As Int32,
                                ByVal Id_Reg As Int32,
                                ByVal Id_Cod As Int32,
                                ByVal Val_Cod As String,
                                    ByVal Validita_Inizio As Date,
                                    ByVal Validita_Fine As Date,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_W.Modifica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE Reg_Impianti_Codici SET ")
            StrSQL.Append("    Val_Cod           = '" & Agro_SQL_SaveText(Val_Cod) & "'")
            StrSQL.Append("   ,Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "'")
            StrSQL.Append(" AND   Sa_Cod = " & Sa_Cod & " ")
            StrSQL.Append(" AND   Appezza = " & Appezza & " ")
            StrSQL.Append(" AND   Id_Reg = " & Id_Reg & " ")
            StrSQL.Append(" AND   Id_Cod = " & Id_Cod & " ")
            StrSQL.Append(" AND   Progetto_Cod  = 0 ")
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

    Public Function Modifica_Parametrizzata(ByVal Piva As String,
                                           ByVal Sa_Cod As Int32,
                                           ByVal Appezza As Int32,
                                           ByVal Id_Reg As Int32,
                                           ByVal Campo As String,
                                           ByVal Valore As Object,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_W.Modifica_Parametrizzata()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False
        Dim strAssegnamento As String = String.Empty

        ' a seconda del tipo del valore che devo aggiornare, formatto la query
        Dim Stringa As Type = GetType(System.String)
        Dim Data As Type = GetType(System.DateTime)
        Dim Intero32 As Type = GetType(System.Int32)

        Try
            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Sa_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            End If

            If Appezza = 0 Then
                Throw New Exception("Parametro non corretto nella query (Appezza obbligatorio)")
            End If

            If Id_Reg = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Reg obbligatorio)")
            End If

            '---------------------------------------------

            Dim TypeVal As Type = Valore.GetType()

            If TypeVal.Equals(Stringa) Then

                strAssegnamento = Campo & "= '" & Agro_SQL_SaveText(Valore.ToString) & "' "

            ElseIf TypeVal.Equals(Data) Then

                strAssegnamento = Campo & "= " & Agro_SQL_SaveDate(Valore.ToString) & " "

            Else

                strAssegnamento = Campo & "= " & Agro_SQL_SaveNum(Valore.ToString) & " "

            End If


            '---------------------------------------------

            StrSQL.Length = 0
            StrSQL.Append("UPDATE Reg_Impianti_Codici SET ")

            StrSQL.Append(strAssegnamento)

            StrSQL.Append("         ,Data_Modifica        =  " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.Append("         ,UserName_Modifica    = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")


            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "'")
            StrSQL.Append(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            StrSQL.Append(" AND   Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            StrSQL.Append(" AND   Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & " ")

            '---------------------------------------------
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

    Public Function Modifica_Chiave(ByVal Piva As String,
                                           ByVal Sa_Cod As Int32,
                                           ByVal Appezza As Int32,
                                           ByVal Id_Reg As Int32,
                                           ByVal Piva_OLD As String,
                                           ByVal Sa_Cod_OLD As Int32,
                                           ByVal Appezza_OLD As Int32,
                                           ByVal Id_Reg_OLD As Int32,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Reg_Impianti_Write.Modifica_Parametrizzata()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False
        Dim strAssegnamento As String = String.Empty

        ' a seconda del tipo del valore che devo aggiornare, formatto la query
        Dim Stringa As Type = GetType(System.String)
        Dim Data As Type = GetType(System.DateTime)
        Dim Intero32 As Type = GetType(System.Int32)

        Try
            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Sa_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            End If

            If Appezza = 0 Then
                Throw New Exception("Parametro non corretto nella query (Appezza obbligatorio)")
            End If

            If Id_Reg = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Reg obbligatorio)")
            End If

            StrSQL.Length = 0
            StrSQL.Append("UPDATE Reg_Impianti_Codici SET ")

            StrSQL.Append(" Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "'")
            StrSQL.Append(" ,Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            StrSQL.Append(" ,Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            StrSQL.Append(" ,Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & " ")

            StrSQL.Append("         ,Data_Modifica        =  " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.Append("         ,UserName_Modifica    = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")


            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Trim(Piva_OLD)) & "'")
            StrSQL.Append(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod_OLD) & " ")
            StrSQL.Append(" AND   Appezza = " & Agro_SQL_SaveNum(Appezza_OLD) & " ")
            StrSQL.Append(" AND   Id_Reg = " & Agro_SQL_SaveNum(Id_Reg_OLD) & " ")

            '---------------------------------------------
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
    Public Function Modifica2(ByVal PIVA As String,
                                ByVal Sa_Cod As Long,
                                ByVal Appezza As Long,
                                ByVal Id_Reg As Long,
                                ByVal id_cod As Long,
                                ByVal Val_Cod As String,
                                ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_W.Modifica2()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False
        Dim Dummmy As Boolean


        If Trim(PIVA) <> "" AndAlso Sa_Cod <> 0 AndAlso Appezza <> 0 AndAlso Id_Reg <> 0 AndAlso id_cod <> 0 Then

            Cancella(PIVA, Sa_Cod, Appezza, Id_Reg, id_cod, "", objParametri)
            Dummmy = Scrivi(PIVA, Sa_Cod, Appezza, Id_Reg, id_cod, Val_Cod, Validita_Inizio, Validita_Fine, objParametri)

        End If

        Return Dummmy

    End Function

    '##############################################################################################
    Public Function ModificaxProgetto(ByVal Piva As String,
                                        ByVal Sa_Cod As Int32,
                                        ByVal Appezza As Int32,
                                        ByVal Id_Reg As Int32,
                                        ByVal Progetto_Cod As Int32,
                                        ByVal Id_Cod As Int32,
                                        ByVal Val_Cod As String,
                                            ByVal Validita_Inizio As Date,
                                            ByVal Validita_Fine As Date,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_W.ModificaxProgetto()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE Reg_Impianti_Codici SET ")
            StrSQL.Append("    Val_Cod           = '" & Agro_SQL_SaveText(Val_Cod) & "'")
            StrSQL.Append("   ,Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "'")
            StrSQL.Append(" AND   Sa_Cod = " & Sa_Cod & " ")
            StrSQL.Append(" AND   Appezza = " & Appezza & " ")
            StrSQL.Append(" AND   Id_Reg = " & Id_Reg & " ")
            StrSQL.Append(" AND   Progetto_Cod = " & Progetto_Cod & " ")
            StrSQL.Append(" AND   Id_Cod = " & Id_Cod & " ")
            '---------------------------------------------

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
    Public Function ModificaxProgetto2(ByVal PIVA As String,
                                       ByVal Sa_Cod As Long,
                                       ByVal Appezza As Long,
                                       ByVal Id_Reg As Long,
                                       ByVal Progetto_Cod As Long,
                                       ByVal id_cod As Long,
                                       ByVal Val_Cod As String,
                                       ByVal Validita_Inizio As Date,
                                       ByVal Validita_Fine As Date,
                                       ByVal xFiltroAggiuntivo As String,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                       ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_W.ModificaxProgetto2()"
        Dim messaggioErrore As String = ""
        Dim xRisp As Boolean = False

        Try

            If Trim(PIVA) <> "" AndAlso Sa_Cod <> 0 AndAlso Appezza <> 0 AndAlso Id_Reg <> 0 AndAlso Progetto_Cod <> 0 AndAlso id_cod <> 0 Then

                CancellaxProgetto(PIVA, Sa_Cod, Appezza, Id_Reg, Progetto_Cod, id_cod, "", objParametri)
                xRisp = ScrivixProgetto(PIVA, Sa_Cod, Appezza, Id_Reg, Progetto_Cod, id_cod, Val_Cod, Validita_Inizio, Validita_Fine, objParametri)

            End If

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
                             ByVal Sa_Cod As Integer,
                             ByVal Appezza As Integer,
                             ByVal Id_Reg As Integer,
                             ByVal Id_Cod As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try


            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.AppendLine(" UPDATE Reg_Impianti_Codici ")
                StrSQL.AppendLine(" SET ")
                StrSQL.AppendLine("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.AppendLine("      ,Inviato = -1 ")
                StrSQL.AppendLine(" WHERE  Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                StrSQL.AppendLine(" AND   Progetto_Cod  = 0 ")
                StrSQL.AppendLine(" AND Inviato > 0")

            Else

                StrSQL.Length = 0
                StrSQL.AppendLine(" DELETE ")
                StrSQL.AppendLine(" FROM     Reg_Impianti_Codici ")
                StrSQL.AppendLine(" WHERE    Piva= '" & Agro_SQL_SaveText(Piva) & "' ")
                StrSQL.AppendLine(" AND    Inviato = 0 ")

            End If


            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND   Sa_Cod = " & Sa_Cod & " ")
            End If

            If Appezza <> 0 Then
                StrSQL.AppendLine(" AND   Appezza = " & Appezza & " ")
            End If

            If Id_Reg <> 0 Then
                StrSQL.AppendLine(" AND   Id_Reg = " & Id_Reg & " ")
            End If

            If Id_Cod <> 0 Then
                StrSQL.AppendLine(" AND Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")
            End If

            '---------------------------------------------
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



    '##############################################################################################
    Public Function CancellaxProgetto(ByVal Piva As String,
                           ByVal Sa_Cod As Int32,
                           ByVal Appezza As Int32,
                           ByVal Id_Reg As Int32,
                           ByVal Progetto_Cod As Int32,
                           ByVal Id_Cod As Int32,
                                ByVal xFiltroAggiuntivo As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_W.CancellaxProgetto()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Reg_Impianti_Codici ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                StrSQL.Append(" AND Inviato > 0")
            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     Reg_Impianti_Codici ")
                StrSQL.Append(" WHERE    Piva= '" & Agro_SQL_SaveText(Piva) & "' ")
                StrSQL.Append(" AND    Inviato = 0 ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND   Sa_Cod = " & Sa_Cod & " ")
            End If

            If Appezza <> 0 Then
                StrSQL.Append(" AND   Appezza = " & Appezza & " ")
            End If

            If Id_Reg <> 0 Then
                StrSQL.Append(" AND   Id_Reg = " & Id_Reg & " ")
            End If

            If Progetto_Cod <> 0 Then
                StrSQL.Append(" AND   Progetto_Cod = " & Progetto_Cod & " ")
            End If

            If Id_Cod <> 0 Then
                StrSQL.Append(" AND Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")
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

    Public Function aggiornaxProgetto(PIVA As String, sa_cod As Integer, appezza As Integer, id_reg As Integer, Progetto_Cod As Integer, id_cod As AgronicaCoreDataProvider.TipiEnumerativi.enum_CodiciAnagrafe, val_cod As String, datainizio As Date, datafine As Date, objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri)
        If PIVA = "" Then
            Throw New Exception("PIVA parametro obbligatorio")
        End If
        If id_cod = 0 Then
            Throw New Exception("id_cod parametro obbligatorio")
        End If
        If sa_cod = 0 Then
            Throw New Exception("sa_cod parametro obbligatorio")
        End If
        If appezza = 0 Then
            Throw New Exception("appezza parametro obbligatorio")
        End If
        CancellaxProgetto(PIVA, sa_cod, appezza, id_reg, Progetto_Cod, id_cod, "", objParametriServer)
        Return ScrivixProgetto(PIVA, sa_cod, appezza, id_reg, Progetto_Cod, id_cod, val_cod, datainizio, datafine, objParametriServer)

    End Function

    Public Function InsertUpdateDeleteCodiciEsercizio_Massivo(listChiavi As List(Of (String, Integer, Integer, Integer, Integer)),
                                                              tipoOperazione As enum_TipoOperazioneDB,
                                                              id_cod As Integer,
                                                              val_cod As String,
                                                              timeStamp As Date,
                                                              ByVal objParametri As AgronicaCoreParametri
                                                              ) As Boolean

        Const nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_W.InsertUpdateDeleteCodiciEsercizio_Massivo()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Dim flagConnessione, flagTransazione As Boolean

        Try

            If listChiavi.Count = 0 Then
                Throw New Exception("Parametro non corretto nella query (listChiavi obbligatorio)")
            End If

            Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

            TempChiaviMassivo.CreaTabellaTemp_FiltroEsercizi(listChiavi, nomeRoutine, objParametri)

            StrSQL.Length = 0
            Select Case tipoOperazione
                Case enum_TipoOperazioneDB.Scrittura, enum_TipoOperazioneDB.Modifica
                    StrSQL.AppendLine(" MERGE INTO Reg_Impianti_Codici AS target ")
                    StrSQL.AppendLine(" USING #TempEsercizio AS source ON  ")
                    StrSQL.AppendLine("     target.Piva = source.Piva COLLATE DATABASE_DEFAULT")
                    StrSQL.AppendLine(" AND target.Sa_Cod = source.Sa_Cod ")
                    StrSQL.AppendLine(" AND target.Appezza = source.Appezza ")
                    StrSQL.AppendLine(" AND target.Id_Reg = source.Id_Reg ")
                    StrSQL.AppendLine(" AND target.Progetto_Cod = source.Progetto_Cod ")
                    StrSQL.AppendLine($" AND target.Id_Cod = {Agro_SQL_SaveNum(id_cod)}")
                    StrSQL.AppendLine("")
                    StrSQL.AppendLine(" WHEN MATCHED THEN")
                    StrSQL.AppendLine(" UPDATE SET ")
                    StrSQL.AppendLine($"     target.Val_Cod = '{Agro_SQL_SaveText(val_cod)}' ")
                    StrSQL.AppendLine($"   , target.Data_Modifica = {Agro_SQL_SaveDateTime(Date.Now())} ")
                    StrSQL.AppendLine($"   , target.UserName_Modifica = '{Agro_SQL_SaveText(objParametri.UsernameOperazione)}' ")
                    StrSQL.AppendLine("")
                    StrSQL.AppendLine(" WHEN NOT MATCHED THEN")
                    StrSQL.AppendLine("")
                    StrSQL.AppendLine(" INSERT ( ")
                    StrSQL.AppendLine("           Piva ")
                    StrSQL.AppendLine("         , Sa_Cod ")
                    StrSQL.AppendLine("         , Appezza   ")
                    StrSQL.AppendLine("         , Id_Reg   ")
                    StrSQL.AppendLine("         , Progetto_Cod   ")
                    StrSQL.AppendLine("         , Id_Cod ")
                    StrSQL.AppendLine("         , Val_Cod ")
                    StrSQL.AppendLine("         , Inviato ")
                    StrSQL.AppendLine("         , DataInvio ")
                    StrSQL.AppendLine("         , Data_Creazione ")
                    StrSQL.AppendLine("         , Data_Modifica ")
                    StrSQL.AppendLine("         , UserName_Creazione ")
                    StrSQL.AppendLine("         , UserName_Modifica ")
                    StrSQL.AppendLine("         , Validita_Inizio ")
                    StrSQL.AppendLine("         , Validita_Fine ")
                    StrSQL.AppendLine(" ) ")
                    StrSQL.AppendLine("VALUES (")
                    StrSQL.AppendLine("           source.Piva ")
                    StrSQL.AppendLine("         , source.Sa_Cod ")
                    StrSQL.AppendLine("         , source.Appezza ")
                    StrSQL.AppendLine("         , source.Id_Reg ")
                    StrSQL.AppendLine("         , source.Progetto_Cod ")
                    StrSQL.AppendLine($"         , {Agro_SQL_SaveNum(id_cod)} ")
                    StrSQL.AppendLine($"         ,'{Agro_SQL_SaveText(val_cod)}' ")
                    StrSQL.AppendLine("         , 0  ")
                    StrSQL.AppendLine("         , NULL  ")
                    StrSQL.AppendLine($"         , {Agro_SQL_SaveDateTime(Date.Now())}")
                    StrSQL.AppendLine($"         , {Agro_SQL_SaveDateTime(Date.Now())}")
                    StrSQL.AppendLine($"         , '{Agro_SQL_SaveText(objParametri.UsernameOperazione)}' ")
                    StrSQL.AppendLine($"         , '{Agro_SQL_SaveText(objParametri.UsernameOperazione)}' ")
                    StrSQL.AppendLine($"         , {Agro_SQL_SaveDate(AGRODATAINIZIO)}")
                    StrSQL.AppendLine($"         , {Agro_SQL_SaveDate(AGRODATAFINE)}")
                    StrSQL.AppendLine(" );")

                Case enum_TipoOperazioneDB.Cancellazione
                    StrSQL.AppendLine(" DELETE Reg_Impianti_Codici ")
                    StrSQL.AppendLine(" FROM Reg_Impianti_Codici ")
                    StrSQL.AppendLine(" JOIN #TempEsercizio temp ON  ")
                    StrSQL.AppendLine("     Reg_Impianti_Codici.Piva = temp.Piva COLLATE DATABASE_DEFAULT ")
                    StrSQL.AppendLine(" AND Reg_Impianti_Codici.Sa_Cod = temp.Sa_Cod ")
                    StrSQL.AppendLine(" AND Reg_Impianti_Codici.Appezza = temp.Appezza ")
                    StrSQL.AppendLine(" AND Reg_Impianti_Codici.ID_Reg = temp.ID_Reg ")
                    StrSQL.AppendLine(" AND Reg_Impianti_Codici.Progetto_Cod = temp.Progetto_Cod ")
                    StrSQL.AppendLine($" WHERE Id_Cod = {Agro_SQL_SaveNum(id_cod)} ")
            End Select
            '---------------------------------------------

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            ' Eliminazione tabella temporanea
            TempChiaviMassivo.EliminaTabellaTemp_FiltroEsercizi(nomeRoutine, objParametri)

            'commit transazione
            Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

        Catch ex As Exception
            ' Rollback
            Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        Finally
            Utility.VerificaChiudiConnessione(objParametri, flagConnessione)
        End Try

        Return xRisp

    End Function

    Public Function InsertUpdateDeleteCodiciImpianto_Massivo(listChiavi As List(Of (String, Integer, Integer, Integer)),
                                                             tipoOperazione As enum_TipoOperazioneDB,
                                                             id_cod As Integer,
                                                             val_cod As String,
                                                             timeStamp As Date,
                                                             ByVal objParametri As AgronicaCoreParametri
                                                             ) As Boolean

        Const nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_W.InsertUpdateDeleteCodiciImpianto_Massivo()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Dim flagConnessione, flagTransazione As Boolean

        Try

            If listChiavi.Count = 0 Then
                Throw New Exception("Parametro non corretto nella query (listChiavi obbligatorio)")
            End If

            Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

            TempChiaviMassivo.CreaTabellaTemp_FiltroImpianti(listChiavi, nomeRoutine, objParametri)

            StrSQL.Length = 0
            Select Case tipoOperazione
                Case enum_TipoOperazioneDB.Scrittura, enum_TipoOperazioneDB.Modifica
                    StrSQL.AppendLine(" MERGE INTO Reg_Impianti_Codici AS target ")
                    StrSQL.AppendLine(" USING #TempImpianto AS source ON  ")
                    StrSQL.AppendLine("     target.Piva = source.Piva COLLATE DATABASE_DEFAULT")
                    StrSQL.AppendLine(" AND target.Sa_Cod = source.Sa_Cod ")
                    StrSQL.AppendLine(" AND target.Appezza = source.Appezza ")
                    StrSQL.AppendLine(" AND target.Id_Reg = source.Id_Reg ")
                    StrSQL.AppendLine($" AND target.Id_Cod = {Agro_SQL_SaveNum(id_cod)}")
                    StrSQL.AppendLine("")
                    StrSQL.AppendLine(" WHEN MATCHED THEN")
                    StrSQL.AppendLine(" UPDATE SET ")
                    StrSQL.AppendLine($"     target.Val_Cod = '{Agro_SQL_SaveText(val_cod)}' ")
                    StrSQL.AppendLine($"   , target.Data_Modifica = {Agro_SQL_SaveDateTime(Date.Now())} ")
                    StrSQL.AppendLine($"   , target.UserName_Modifica = '{Agro_SQL_SaveText(objParametri.UsernameOperazione)}' ")
                    StrSQL.AppendLine("")
                    StrSQL.AppendLine(" WHEN NOT MATCHED THEN")
                    StrSQL.AppendLine("")
                    StrSQL.AppendLine(" INSERT ( ")
                    StrSQL.AppendLine("           Piva ")
                    StrSQL.AppendLine("         , Sa_Cod ")
                    StrSQL.AppendLine("         , Appezza   ")
                    StrSQL.AppendLine("         , Id_Reg   ")
                    StrSQL.AppendLine("         , Id_Cod ")
                    StrSQL.AppendLine("         , Val_Cod ")
                    StrSQL.AppendLine("         , Inviato ")
                    StrSQL.AppendLine("         , DataInvio ")
                    StrSQL.AppendLine("         , Data_Creazione ")
                    StrSQL.AppendLine("         , Data_Modifica ")
                    StrSQL.AppendLine("         , UserName_Creazione ")
                    StrSQL.AppendLine("         , UserName_Modifica ")
                    StrSQL.AppendLine("         , Validita_Inizio ")
                    StrSQL.AppendLine("         , Validita_Fine ")
                    StrSQL.AppendLine(" ) ")
                    StrSQL.AppendLine("VALUES (")
                    StrSQL.AppendLine("           source.Piva ")
                    StrSQL.AppendLine("         , source.Sa_Cod ")
                    StrSQL.AppendLine("         , source.Appezza ")
                    StrSQL.AppendLine("         , source.Id_Reg ")
                    StrSQL.AppendLine($"         , {Agro_SQL_SaveNum(id_cod)} ")
                    StrSQL.AppendLine($"         ,'{Agro_SQL_SaveText(val_cod)}' ")
                    StrSQL.AppendLine("         , 0  ")
                    StrSQL.AppendLine("         , NULL  ")
                    StrSQL.AppendLine($"         , {Agro_SQL_SaveDateTime(Date.Now())}")
                    StrSQL.AppendLine($"         , {Agro_SQL_SaveDateTime(Date.Now())}")
                    StrSQL.AppendLine($"         , '{Agro_SQL_SaveText(objParametri.UsernameOperazione)}' ")
                    StrSQL.AppendLine($"         , '{Agro_SQL_SaveText(objParametri.UsernameOperazione)}' ")
                    StrSQL.AppendLine($"         , {Agro_SQL_SaveDate(AGRODATAINIZIO)}")
                    StrSQL.AppendLine($"         , {Agro_SQL_SaveDate(AGRODATAFINE)}")
                    StrSQL.AppendLine(" );")

                Case enum_TipoOperazioneDB.Cancellazione
                    StrSQL.AppendLine(" DELETE Reg_Impianti_Codici ")
                    StrSQL.AppendLine(" FROM Reg_Impianti_Codici ")
                    StrSQL.AppendLine(" JOIN #TempImpianto temp ON  ")
                    StrSQL.AppendLine("     Reg_Impianti_Codici.Piva = temp.Piva COLLATE DATABASE_DEFAULT ")
                    StrSQL.AppendLine(" AND Reg_Impianti_Codici.Sa_Cod = temp.Sa_Cod ")
                    StrSQL.AppendLine(" AND Reg_Impianti_Codici.Appezza = temp.Appezza ")
                    StrSQL.AppendLine(" AND Reg_Impianti_Codici.ID_Reg = temp.ID_Reg ")
                    StrSQL.AppendLine($" WHERE Id_Cod = {Agro_SQL_SaveNum(id_cod)} ")
            End Select
            '---------------------------------------------

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            ' Eliminazione tabella temporanea
            TempChiaviMassivo.EliminaTabellaTemp_FiltroImpianti(nomeRoutine, objParametri)

            'commit transazione
            Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

        Catch ex As Exception
            ' Rollback
            Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        Finally
            Utility.VerificaChiudiConnessione(objParametri, flagConnessione)
        End Try

        Return xRisp

    End Function

#Region "Entity Framework"

    Public Sub ScriviModificaEliminaxImpianto(ByVal piva As String,
                                              ByVal saCod As Integer,
                                              ByVal appezza As Integer,
                                              ByVal idReg As Integer,
                                              ByVal idCod As Integer,
                                              ByVal valCod As String,
                                              ByVal delete As Boolean,
                                              ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                              ByRef GiasContext As Gias_DeveloperServer_Entities)

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_W.ScriviModificaEliminaxImpianto()"
        Dim messaggioErrore As String = ""

        Try

            If idCod <> 0 Then

                Dim imp_codl = From ic In GiasContext.Reg_Impianti_Codici
                               Where ic.PIVA = piva AndAlso
                                     ic.sa_cod = saCod AndAlso
                                     ic.appezza = appezza AndAlso
                                     ic.Id_Reg = idReg AndAlso
                                     ic.Progetto_Cod = 0 AndAlso
                                     ic.id_cod = idCod
                               Select ic

                Dim operazione As enum_TipoOperazioneDB

                If imp_codl.Count > 0 AndAlso (valCod <> "" AndAlso valCod IsNot Nothing) Then
                    operazione = enum_TipoOperazioneDB.Modifica
                ElseIf imp_codl.Count > 0 AndAlso (valCod = "" OrElse valCod Is Nothing) Then
                    operazione = enum_TipoOperazioneDB.Cancellazione
                ElseIf imp_codl.Count = 0 AndAlso (valCod = "" OrElse valCod Is Nothing) Then
                    operazione = enum_TipoOperazioneDB.Lettura
                ElseIf imp_codl.Count = 0 AndAlso (valCod <> "" AndAlso valCod IsNot Nothing) Then
                    operazione = enum_TipoOperazioneDB.Scrittura
                End If

                Select Case operazione
                    Case enum_TipoOperazioneDB.Scrittura

                        Dim imp_cod As New AgronicaCoreEntityFramework_POCO.Reg_Impianti_Codici With {
                            .PIVA = piva,
                            .sa_cod = saCod,
                            .appezza = appezza,
                            .Id_Reg = idReg,
                            .Progetto_Cod = 0,
                            .id_cod = idCod,
                            .val_cod = valCod,
                            .inviato = 0,
                            .Data_Creazione = DateTime.Now,
                            .Data_Modifica = DateTime.Now,
                            .Validita_Inizio = AGRODATAINIZIO,
                            .Validita_Fine = AGRODATAFINE,
                            .Username_Creazione = objParametriServer.UsernameOperazione,
                            .Username_Modifica = objParametriServer.UsernameOperazione
                        }

                        GiasContext.Reg_Impianti_Codici.Add(imp_cod)

                    Case enum_TipoOperazioneDB.Modifica
                        Dim imp_cod = imp_codl.FirstOrDefault
                        imp_cod.val_cod = valCod
                        imp_cod.Data_Modifica = DateTime.Now
                        imp_cod.Username_Modifica = objParametriServer.UsernameOperazione

                        GiasContext.Reg_Impianti_Codici.Attach(imp_cod)
                        GiasContext.Entry(imp_cod).State = EntityState.Modified

                    Case enum_TipoOperazioneDB.Cancellazione

                        Dim imp_cod = imp_codl.FirstOrDefault
                        GiasContext.Reg_Impianti_Codici.Attach(imp_cod)
                        GiasContext.Reg_Impianti_Codici.Remove(imp_cod)

                End Select

                GiasContext.SaveChanges()

            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

    Public Sub ScriviModificaEliminaxProgetto(ByVal piva As String,
                                              ByVal saCod As Integer,
                                              ByVal appezza As Integer,
                                              ByVal idReg As Integer,
                                              ByVal progettoCod As Integer,
                                              ByVal idCod As Integer,
                                              ByVal valCod As String,
                                              ByVal delete As Boolean,
                                              ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                              ByRef GiasContext As Gias_DeveloperServer_Entities,
                                              Optional ByVal validitaInizio As Date = AGRODATAINIZIO,
                                              Optional ByVal validitaFine As Date = AGRODATAFINE)

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_W.ScriviModificaEliminaxProgetto()"
        Dim messaggioErrore As String = ""

        Try

            Dim imp_codl = From ic In GiasContext.Reg_Impianti_Codici
                           Where ic.PIVA = piva AndAlso
                                 ic.sa_cod = saCod AndAlso
                                 ic.appezza = appezza AndAlso
                                 ic.Id_Reg = idReg AndAlso
                                 ic.Progetto_Cod = progettoCod AndAlso
                                 ic.id_cod = idCod
                           Select ic

            Dim operazione As enum_TipoOperazioneDB

            If imp_codl.Count > 0 AndAlso (valCod <> "" AndAlso valCod IsNot Nothing) Then
                operazione = enum_TipoOperazioneDB.Modifica
            ElseIf imp_codl.Count > 0 AndAlso (valCod = "" OrElse valCod Is Nothing) Then
                operazione = enum_TipoOperazioneDB.Cancellazione
            ElseIf imp_codl.Count = 0 AndAlso (valCod = "" OrElse valCod Is Nothing) Then
                operazione = enum_TipoOperazioneDB.Lettura
            ElseIf imp_codl.Count = 0 AndAlso (valCod <> "" AndAlso valCod IsNot Nothing) Then
                operazione = enum_TipoOperazioneDB.Scrittura
            End If

            Select Case operazione
                Case enum_TipoOperazioneDB.Scrittura

                    Dim imp_cod As New AgronicaCoreEntityFramework_POCO.Reg_Impianti_Codici With {
                        .PIVA = piva,
                        .sa_cod = saCod,
                        .appezza = appezza,
                        .Id_Reg = idReg,
                        .Progetto_Cod = progettoCod,
                        .id_cod = idCod,
                        .val_cod = valCod,
                        .inviato = 0,
                        .Data_Creazione = DateTime.Now,
                        .Data_Modifica = DateTime.Now,
                        .Validita_Inizio = validitaInizio,
                        .Validita_Fine = validitaFine,
                        .Username_Creazione = objParametriServer.UsernameOperazione,
                        .Username_Modifica = objParametriServer.UsernameOperazione
                    }

                    GiasContext.Reg_Impianti_Codici.Add(imp_cod)

                Case enum_TipoOperazioneDB.Modifica
                    Dim imp_cod = imp_codl.FirstOrDefault
                    imp_cod.val_cod = valCod
                    imp_cod.Data_Modifica = DateTime.Now
                    imp_cod.Validita_Inizio = validitaInizio
                    imp_cod.Validita_Fine = validitaFine
                    imp_cod.Username_Modifica = objParametriServer.UsernameOperazione

                    GiasContext.Reg_Impianti_Codici.Attach(imp_cod)
                    GiasContext.Entry(imp_cod).State = EntityState.Modified

                Case enum_TipoOperazioneDB.Cancellazione

                    Dim imp_cod = imp_codl.FirstOrDefault
                    GiasContext.Reg_Impianti_Codici.Attach(imp_cod)
                    GiasContext.Reg_Impianti_Codici.Remove(imp_cod)

            End Select

            GiasContext.SaveChanges()

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

#End Region

End Class

