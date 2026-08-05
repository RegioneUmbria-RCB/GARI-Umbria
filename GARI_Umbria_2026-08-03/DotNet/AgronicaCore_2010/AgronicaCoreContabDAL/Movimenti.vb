Imports System.Data.Entity
Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class Movimenti_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '#########################################################
    Public Function ChkLayOutPrezzo_from_id_Agenda(ByVal piva As String,
                                                   ByVal idAgenda As Integer,
                                                   ByVal lavCod As Integer,
                                                   ByVal xFiltroAggiuntivo As String,
                                                   ByRef objParametri As AgronicaCoreParametri
                                                   ) As Integer

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_R.ChkLayOutPrezzo_from_id_Agenda()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim chkLayOutPrezzo As Integer = 0

        Try

            dt = MovimentiContabili(lavCod,
                                    piva,
                                    0,
                                    idAgenda,
                                    0, 0, CStr(CAU_REGISTRAZIONI), 0,
                                    AGRODATAINIZIO,
                                    AGRODATAFINE,
                                    "XYZ",
                                    0,
                                    "XYZ",
                                    0, 0,
                                    AGRODATAINIZIO,
                                    xFiltroAggiuntivo, "",
                                    objParametri)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                chkLayOutPrezzo = dt.Rows(0).Item("ChkLayOut_Prezzo")
            End If


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return chkLayOutPrezzo

    End Function

    '#########################################################
    Public Function Leggi_Cod_RisUm_Cessionario_From_idAgenda(ByVal piva As String,
                                                              ByVal idAgenda As Integer,
                                                              ByVal xFiltroAggiuntivo As String,
                                                              ByRef objParametri As AgronicaCoreParametri
                                                              ) As Integer

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_R.Leggi_Cod_RisUm_Cessionario_From_idAgenda"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim codRisUm As Integer = 0

        Try

            dt = MovimentiContabili(0,
                                    piva,
                                    0,
                                    idAgenda,
                                    0, 0, CStr(CAU_REGISTRAZIONI), 0,
                                    AGRODATAINIZIO,
                                    AGRODATAFINE,
                                    "XYZ",
                                    0,
                                    "XYZ",
                                    0, 0,
                                    AGRODATAINIZIO,
                                    xFiltroAggiuntivo, "",
                                    objParametri)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                codRisUm = dt.Rows(0).Item("Cod_RisUm")
            End If


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return codRisUm

    End Function

    '#########################################################################
    Public Function Leggi_CodRisUm_CodIndirizzoRisUm_Cessionario_From_idAgenda(ByVal piva As String,
                                                                                ByVal idAgenda As Integer,
                                                                                ByVal xFiltroAggiuntivo As String,
                                                                                ByRef Cod_IndirizzoRisUm As Integer,
                                                                                ByRef objParametri As AgronicaCoreParametri
                                                                                ) As Integer

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_R.Leggi_CodRisUm_CodIndirizzoRisUm_Cessionario_From_idAgenda"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim codRisUm As Integer = 0
        Cod_IndirizzoRisUm = 0

        Try

            dt = MovimentiContabili(0,
                                    piva,
                                    0,
                                    idAgenda,
                                    0, 0, CStr(CAU_REGISTRAZIONI), 0,
                                    AGRODATAINIZIO,
                                    AGRODATAFINE,
                                    "XYZ",
                                    0,
                                    "XYZ",
                                    0, 0,
                                    AGRODATAINIZIO,
                                    xFiltroAggiuntivo, "",
                                    objParametri)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                codRisUm = dt.Rows(0).Item("Cod_RisUm")
                Cod_IndirizzoRisUm = dt.Rows(0).Item("Cod_IndirizzoRisUm")
            End If


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return codRisUm

    End Function

    '#########################################################################
    Public Function Leggi_CodRisUm_CodIndirizzoRisUm_CessionarioDiverso_From_idAgenda(ByVal piva As String,
                                                                                      ByVal idAgenda As Integer,
                                                                                      ByVal xFiltroAggiuntivo As String,
                                                                                      ByRef Cod_IndirizzoRisUm As Integer,
                                                                                      ByRef objParametri As AgronicaCoreParametri
                                                                                      ) As Integer

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_R.Leggi_CodRisUm_CodIndirizzoRisUm_CessionarioDiverso_From_idAgenda"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim codRisUm As Integer = 0
        Cod_IndirizzoRisUm = 0

        Try

            dt = MovimentiContabili(0,
                                    piva,
                                    0,
                                    idAgenda,
                                    0, 0, CStr(CAU_REGISTRAZIONI), 0,
                                    AGRODATAINIZIO,
                                    AGRODATAFINE,
                                    "XYZ",
                                    0,
                                    "XYZ",
                                    0, 0,
                                    AGRODATAINIZIO,
                                    xFiltroAggiuntivo, "",
                                    objParametri)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                codRisUm = dt.Rows(0).Item("Cod_Destinazione")
                Cod_IndirizzoRisUm = dt.Rows(0).Item("Cod_IndirizzoDestinazione")
            End If


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return codRisUm

    End Function


    '#########################################################
    Public Function Cau_Mov_From_Piva_Sa_Cod_Id_Agenda_Id_Mov(ByVal piva As String,
                                                              ByVal saCod As Integer,
                                                              ByVal idAgenda As Integer,
                                                              ByVal idMov As Integer,
                                                              ByRef objParametri As AgronicaCoreParametri
                                                              ) As String

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_R.Lav_Cod_From_Piva_Sa_Cod_Id_Agenda()"


        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT Cau_Mov ")
            strSql.AppendLine(" FROM  Movimenti ")
            strSql.AppendLine(" WHERE 1=1 ")


            strSql.AppendLine(" AND Movimenti.Piva = '" & Agro_SQL_SaveText(piva) & "' ")

            strSql.AppendLine(" AND Movimenti.Sa_Cod = " & Agro_SQL_SaveNum(saCod) & " ")


            strSql.AppendLine(" AND Movimenti.Id_Agenda = " & Agro_SQL_SaveNum(idAgenda) & " ")


            strSql.AppendLine(" AND Movimenti.Id_Mov = " & Agro_SQL_SaveNum(idMov) & "   ")


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If dt.Rows.Count = 0 Then
                Throw New Exception(" Nessun movimento trovato ")
            End If
            If dt.Rows.Count > 1 Then
                Throw New Exception(" Troppi movimenti trovati ")
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try


        Return CStr(dt.Rows(0).Item("Cau_Mov"))

    End Function


    '#######################################################
    'legge solo la tabella movimenti
    Public Function Leggi2(ByVal Piva As String,
                           ByVal Sa_Cod As Integer,
                           ByVal Id_Agenda As Integer,
                           ByVal Id_Mov As Integer,
                           ByVal Cod_RisUm As Integer,
                           ByVal Cau_Mov As String,
                           ByVal xFiltroAggiuntivo As String,
                           ByVal xOrderBy As String,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_R.Leggi2()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""            =>  
        '   Sa_Cod = 0           =>  
        '   Id_Mov = 0           =>  
        '   Cod_RisUm = 0        =>
        '   Cau_Mov = ""         =>
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT * ")
            strSql.AppendLine(" FROM  Movimenti ")

            strSql.AppendLine(" WHERE Movimenti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            strSql.AppendLine(" AND   Movimenti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Piva <> "" Then
                strSql.AppendLine(" AND Movimenti.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND Movimenti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Id_Agenda <> 0 Then
                strSql.AppendLine(" AND Movimenti.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
            End If

            If Id_Mov <> 0 Then
                strSql.AppendLine(" AND Movimenti.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
            End If

            If Cod_RisUm <> 0 Then
                strSql.AppendLine(" AND Movimenti.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   ")
            End If

            If Cau_Mov <> "" Then
                strSql.AppendLine(" AND Movimenti.Cau_Mov = '" & UCase(Agro_SQL_SaveText(Cau_Mov)) & "'   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Movimenti.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Movimenti.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
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

    '###############################################
    Public Function Leggi(ByVal Piva As String,
                          ByVal Sa_Cod As Integer,
                          ByVal Id_Agenda As Integer,
                          ByVal Id_Mov As Integer,
                          ByVal Cod_RisUm As Integer,
                          ByVal Cau_Mov As String,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri,
                          Optional ByVal topNRighe As Integer? = Nothing
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Movimenti_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""            =>  
        '   Sa_Cod = 0           =>  
        '   Id_Mov = 0           =>  
        '   Cod_RisUm = 0        =>
        '   Cau_Mov = ""         =>
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            '------------------------------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    strSql.Length = 0
                    strSql.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
                    If Not IsNothing(topNRighe) AndAlso topNRighe.HasValue Then
                        strSql.AppendLine(" SELECT DISTINCT TOP " + topNRighe.Value.ToString + " ")
                        strSql.AppendLine(" Agenda.Id_Agenda, Agenda.Des_Lib, Movimenti.Data_MOvimento, Agenda.Data_Modifica ")
                    Else
                        strSql.AppendLine(" Select *, Movimenti.Sa_Cod ")
                    End If

                    strSql.AppendLine(" FROM  Movimenti WITH(NOLOCK)")
                    strSql.AppendLine(" INNER JOIN Agenda WITH(NOLOCK)")
                    strSql.AppendLine(" ON Movimenti.Piva = Agenda.Piva And Movimenti.Id_Agenda = Agenda.Id_Agenda ")
                    'Nota: il magazzino per lo scarico potrebbe appartenere ad un altro centro aziendale
                    '          'Join sul Sa_Cod
                    '           strSql.AppendLine(" And   Agenda.Sa_Cod = Movimenti.Sa_Cod ")

                    strSql.AppendLine(" WHERE Movimenti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" And   Movimenti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then
                        strSql.AppendLine(" And Movimenti.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If

                    If Sa_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Id_Agenda <> 0 Then
                        strSql.AppendLine(" AND Movimenti.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
                    End If

                    If Id_Mov <> 0 Then
                        strSql.AppendLine(" AND Movimenti.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
                    End If

                    If Cod_RisUm <> 0 Then
                        strSql.AppendLine(" AND Movimenti.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   ")
                    End If

                    If Cau_Mov <> "" Then
                        strSql.AppendLine(" AND Movimenti.Cau_Mov = '" & UCase(Agro_SQL_SaveText(Cau_Mov)) & "'   ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Agenda.Inviato >=0 ")
                            strSql.AppendLine(" AND   Movimenti.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Agenda.Inviato =-1 ")
                            strSql.AppendLine(" AND   Movimenti.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Movimenti.Piva, Movimenti.sa_cod, Movimenti.id_agenda, Movimenti.id_mov  Asc ")
                    End If


                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    strSql.Length = 0
                    strSql.AppendLine(" SELECT *, Movimenti.Sa_Cod ")
                    strSql.AppendLine(" FROM  Movimenti WITH(NOLOCK)")
                    strSql.AppendLine(" INNER JOIN Agenda WITH(NOLOCK)")
                    strSql.AppendLine(" ON Movimenti.Piva = Agenda.Piva And Movimenti.Id_Agenda = Agenda.Id_Agenda ")
                    'Nota: il magazzino per lo scarico potrebbe appartenere ad un altro centro aziendale
                    '          'Join sul Sa_Cod
                    '           strSql.AppendLine(" And   Agenda.Sa_Cod = Movimenti.Sa_Cod ")

                    strSql.AppendLine(" WHERE Movimenti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" And   Movimenti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")


                    If Piva <> "" Then
                        strSql.AppendLine(" And Movimenti.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If

                    If Sa_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Id_Agenda <> 0 Then
                        strSql.AppendLine(" AND Movimenti.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
                    End If

                    If Id_Mov <> 0 Then
                        strSql.AppendLine(" AND Movimenti.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
                    End If

                    If Cod_RisUm <> 0 Then
                        strSql.AppendLine(" AND Movimenti.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   ")
                    End If

                    If Cau_Mov <> "" Then
                        strSql.AppendLine(" AND Movimenti.Cau_Mov = '" & UCase(Agro_SQL_SaveText(Cau_Mov)) & "'   ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Agenda.Inviato >=0 ")
                            strSql.AppendLine(" AND   Movimenti.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Agenda.Inviato =-1 ")
                            strSql.AppendLine(" AND   Movimenti.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Movimenti.Piva, Movimenti.sa_cod, Movimenti.id_agenda, Movimenti.id_mov  Asc ")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                    strSql.Length = 0
                    strSql.AppendLine(" SELECT Movimenti.*, Imprese.rag_soc, Agenda.des_lib  ")
                    strSql.AppendLine(" FROM  Movimenti WITH(NOLOCK)")
                    strSql.AppendLine(" INNER JOIN Imprese WITH(NOLOCK) ")
                    strSql.AppendLine(" ON Movimenti.Piva = Imprese.Piva ")
                    strSql.AppendLine(" INNER JOIN Agenda WITH(NOLOCK)")
                    strSql.AppendLine(" ON Movimenti.Piva = Agenda.Piva And Movimenti.Id_Agenda = Agenda.Id_Agenda ")
                    strSql.AppendLine(" WHERE Movimenti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" And   Movimenti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then
                        strSql.AppendLine(" And Movimenti.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If

                    If Sa_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Id_Agenda <> 0 Then
                        strSql.AppendLine(" AND Movimenti.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
                    End If

                    If Id_Mov <> 0 Then
                        strSql.AppendLine(" AND Movimenti.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
                    End If

                    If Cod_RisUm <> 0 Then
                        strSql.AppendLine(" AND Movimenti.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   ")
                    End If

                    If Cau_Mov <> "" Then
                        strSql.AppendLine(" AND Movimenti.Cau_Mov = '" & UCase(Agro_SQL_SaveText(Cau_Mov)) & "'   ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Movimenti.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Movimenti.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Movimenti.Piva, Movimenti.sa_cod, Movimenti.id_agenda, Movimenti.id_mov  Asc ")
                    End If


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



    '###############################################
    Public Function LeggixScheduling_Documenti_Contabili(ByVal Piva As String,
                                                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                                          ByVal xFiltroAggiuntivo As String,
                                                          ByVal xOrderBy As String,
                                                          ByRef objParametri As AgronicaCoreParametri
                                                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Movimenti_R.LeggixScheduling_Documenti_Contabili()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""            =>  
        '   Sa_Cod = 0           =>  
        '   Id_Mov = 0           =>  
        '   Cod_RisUm = 0        =>
        '   Cau_Mov = ""         =>
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            '------------------------------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    strSql.Length = 0
                    strSql.AppendLine(" SELECT Distinct Movimenti.Cod_Risum ")
                    strSql.AppendLine(" FROM  Movimenti ")
                    strSql.AppendLine(" INNER JOIN Agenda ON Movimenti.Piva = Agenda.Piva AND Movimenti.Id_Agenda = Agenda.Id_Agenda ")
                    strSql.AppendLine(" INNER JOIN Movimenti_Dettagli ON Movimenti.Piva = Movimenti_Dettagli.Piva AND Movimenti.Id_Agenda = Movimenti_Dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov ")
                    'Nota: il magazzino per lo scarico potrebbe appartenere ad un altro centro aziendale
                    '          'Join sul Sa_Cod
                    '           strSql.AppendLine(" AND   Agenda.Sa_Cod = Movimenti.Sa_Cod ")

                    strSql.AppendLine(" WHERE Movimenti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND   Movimenti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then
                        strSql.AppendLine(" AND Movimenti.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Agenda.Inviato >=0 ")
                            strSql.AppendLine(" AND   Movimenti.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Agenda.Inviato =-1 ")
                            strSql.AppendLine(" AND   Movimenti.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Movimenti.Cod_Risum  Asc ")
                    End If
                    '



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


    '#########################################################
    '====================================================================================
    'Parametri opzionali :
    '   Lav_Cod = 0
    '   Piva = ""            
    '   Sa_Cod = 0           
    '   Id_Mov = 0           
    '   Cod_RisUm = 0       
    '   Cau_Mov = ""   
    ' Anno =0
    ' Data_Movimento = agrodatainizio
    ' Scadenza= agrodatafine
    ' Doc_Numero_Sin = XYZ (perchè stringa vuota è significativa)
    ' Doc_Numero = 0
    ' Doc_Numero_Des = XYZ (perchè stringa vuota è significativa)
    ' Progr_Protocollo = 0
    ' Progr_Registrazione =0
    ' Data_Registrazione = agrodatainizio
    '====================================================================================
    Public Function MovimentiContabili(ByVal Lav_Cod As Integer,
                                       ByVal Piva As String,
                                       ByVal Sa_Cod As Integer,
                                       ByVal Id_Agenda As Integer,
                                       ByVal Id_Mov As Integer,
                                       ByVal Cod_RisUm As Integer,
                                       ByVal Cau_Mov As String,
                                       ByVal Anno As Integer,
                                       ByVal Data_Movimento As Date,
                                       ByVal Scadenza As Date,
                                       ByVal Doc_Numero_Sin As String,
                                       ByVal Doc_Numero As Decimal,
                                       ByVal Doc_Numero_Des As String,
                                       ByVal Progr_Protocollo As Integer,
                                       ByVal Progr_Registrazione As Integer,
                                       ByVal Data_Registrazione As Date,
                                       ByVal xFiltroAggiuntivo As String,
                                       ByVal xOrderBy As String,
                                       ByRef objParametri As AgronicaCoreParametri
                                       ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_R.MovimentiContabili()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT * ")

            strSql.AppendLine(" FROM    Agenda  ")
            strSql.AppendLine(" INNER JOIN Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda")

            strSql.AppendLine(" WHERE Movimenti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            strSql.AppendLine(" AND   Movimenti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Lav_Cod <> 0 Then
                strSql.AppendLine(" AND Agenda.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & "   ")
            End If

            If Piva <> "" Then
                strSql.AppendLine(" AND Movimenti.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND Movimenti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Id_Agenda <> 0 Then
                strSql.AppendLine(" AND Movimenti.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
            End If

            If Id_Mov <> 0 Then
                strSql.AppendLine(" AND Movimenti.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
            End If

            If Cod_RisUm <> 0 Then
                strSql.AppendLine(" AND Movimenti.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   ")
            End If

            If Cau_Mov <> "" Then
                strSql.AppendLine(" AND Movimenti.Cau_Mov = '" & UCase(Agro_SQL_SaveText(Cau_Mov)) & "'   ")
            End If

            If Anno <> 0 Then
                strSql.AppendLine(" AND Year(Movimenti.Data_Movimento) = " & Agro_SQL_SaveNum(Anno) & "  ")
            End If

            If Data_Movimento <> AGRODATAINIZIO Then
                strSql.AppendLine(" AND Movimenti.Data_Movimento = " & UCase(Agro_SQL_SaveDate(Data_Movimento)) & "   ")
            End If

            If Scadenza <> AGRODATAFINE AndAlso Scadenza <> AGRODATAINIZIO Then
                strSql.AppendLine(" AND Movimenti.Scadenza = " & UCase(Agro_SQL_SaveDate(Scadenza)) & "   ")
            End If

            If Doc_Numero_Sin.ToUpper <> "XYZ" Then
                strSql.AppendLine(" AND Movimenti.Doc_Numero_Sin = '" & UCase(Agro_SQL_SaveText(Doc_Numero_Sin)) & "'   ")
            End If

            If Doc_Numero <> 0 Then
                strSql.AppendLine(" AND Movimenti.Doc_Numero = " & Agro_SQL_SaveNum(Doc_Numero) & "   ")
            End If

            If Doc_Numero_Des.ToUpper <> "XYZ" Then
                strSql.AppendLine(" AND Movimenti.Doc_Numero_Des = '" & UCase(Agro_SQL_SaveText(Doc_Numero_Des)) & "'   ")
            End If

            If Progr_Protocollo <> 0 Then
                strSql.AppendLine(" AND Movimenti.Progr_Protocollo = " & Agro_SQL_SaveNum(Progr_Protocollo) & "   ")
            End If

            If Progr_Registrazione <> 0 Then
                strSql.AppendLine(" AND Movimenti.Progr_Registrazione = " & Agro_SQL_SaveNum(Progr_Registrazione) & "   ")
            End If

            If Data_Registrazione <> AGRODATAINIZIO Then
                strSql.AppendLine(" AND Movimenti.Data_Registrazione = " & UCase(Agro_SQL_SaveDate(Data_Registrazione)) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Movimenti.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Movimenti.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Movimenti.Piva, Data_Movimento, Doc_Numero_Sin, Doc_Numero, Doc_Numero_Des  Asc ")
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

    Public Function LeggiMovimentixPUA(ByVal Piva As String,
                                       ByVal lav_cod As Integer(),
                                       ByVal elem_cod As Integer,
                                       ByVal pro_cod As Integer,
                                       ByVal Cau_Mov As String,
                                       ByVal Data_Inizio As Date,
                                       ByVal Data_fine As Date,
                                       ByVal Udm_Cod As Integer,
                                       ByVal xFiltroAggiuntivo As String,
                                       ByVal xOrderBy As String,
                                       ByRef objParametri As AgronicaCoreParametri
                                       ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_R.LeggiMovimentixPUA()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT Agenda.PIVA, Agenda.id_agenda, Agenda.lav_cod, Movimenti.id_mov, Movimenti_dettagli.elem_cod, Movimenti.Cau_Mov, Movimenti_dettagli.udm_cod, Movimenti_dettagli.qta, ISNULL(Mov_Dettaglio_Tecnico.N, 0) AS N ")
            strSql.AppendLine(" ,ISNULL(Mov_Dettaglio_Tecnico.P, 0) AS P, ISNULL(Mov_Dettaglio_Tecnico.K, 0) AS K,ISNULL(Mov_Dettaglio_Tecnico.Mg, 0) AS Mg, ISNULL(Mov_Dettaglio_Tecnico.Cu, 0) AS Cu, Movimenti_dettagli.lotto ")
            strSql.AppendLine(" ,Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.Id_Destinazione")

            strSql.AppendLine(" FROM Agenda ")
            strSql.AppendLine(" INNER JOIN Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda")
            strSql.AppendLine(" INNER JOIN Movimenti_dettagli ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_dettagli.Id_mov = Movimenti.Id_mov")
            strSql.AppendLine(" INNER JOIN Mov_Destinazioni ON Movimenti_dettagli.PIVA = Mov_Destinazioni.PIVA AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Movimenti_dettagli.Id_mov = Mov_Destinazioni.Id_mov AND Movimenti_dettagli.Id_mov_det = Mov_Destinazioni.Id_mov_det ")

            strSql.AppendLine(" LEFT JOIN Mov_dettaglio_tecnico ON Movimenti_dettagli.PIVA = Mov_dettaglio_tecnico.PIVA AND Movimenti_dettagli.Id_Agenda = Mov_dettaglio_tecnico.Id_Agenda AND Movimenti_dettagli.Id_mov = Mov_dettaglio_tecnico.Id_mov AND Movimenti_dettagli.Id_mov_det = Mov_dettaglio_tecnico.Id_mov_det")

            strSql.AppendLine(" WHERE Movimenti.data_movimento >= " & Agro_SQL_SaveDate(Data_Inizio))
            strSql.AppendLine(" AND   Movimenti.data_movimento <= " & Agro_SQL_SaveDate(Data_fine))

            strSql.AppendLine(" AND Movimenti.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            strSql.AppendLine(" AND Movimenti.Cau_Mov = '" & Agro_SQL_SaveText(Cau_Mov) & "' ")
            strSql.AppendLine(" AND Agenda.lav_cod IN (" & Agro_SQL_Save_Clausola_IN(String.Join(",", lav_cod), False) & ") ")
            strSql.AppendLine(" AND Movimenti_dettagli.elem_cod = " & Agro_SQL_SaveNum(elem_cod))
            strSql.AppendLine(" AND Movimenti_dettagli.pro_cod = " & Agro_SQL_SaveNum(pro_cod))
            strSql.AppendLine(" AND Movimenti_dettagli.udm_cod = " & Agro_SQL_SaveNum(Udm_Cod))

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Movimenti.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Movimenti.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Movimenti.Piva, Data_Movimento ")
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

    Public Function LeggiMovimentixTracciabilità(ByVal Piva As String,
                                                 ByVal Sa_cod As String,
                                               ByVal lav_cod As Integer(),
                                               ByVal elem_cod As Integer,
                                               ByVal pro_cod As Integer,
                                               ByVal mat_cod As Integer,
                                               ByVal Cau_Mov As String,
                                               ByVal Data_Inizio As Date,
                                               ByVal Data_fine As Date,
                                               ByVal xFiltroAggiuntivo As String,
                                               ByVal xOrderBy As String,
                                               ByRef objParametri As AgronicaCoreParametri
                                               ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_R.LeggiMovimentixTracciabilità()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT  Agenda.PIVA,Movimenti_dettagli.Sa_Cod, Agenda.id_agenda,Movimenti_dettagli.Id_Mov,Movimenti_dettagli.Id_Mov_Det, Movimenti.Data_Movimento ")
            strSql.AppendLine(" ,Agenda.lav_cod,Movimenti.Cau_Mov, Movimenti_dettagli.elem_cod, Movimenti_dettagli.Pro_Cod, UnitaMisura.UDM_SIM, Movimenti_dettagli.qta, Agenda.des_lib ")
            strSql.AppendLine(" ,ISNULL(Mov_Dettaglio_Tecnico.N, 0) AS N ,ISNULL(Mov_Dettaglio_Tecnico.P, 0) AS P, ISNULL(Mov_Dettaglio_Tecnico.K, 0) AS K,ISNULL(Mov_Dettaglio_Tecnico.Mg, 0) AS Mg, ISNULL(Mov_Dettaglio_Tecnico.Cu, 0) AS Cu ")
            strSql.AppendLine(" FROM Agenda ")
            strSql.AppendLine(" INNER JOIN Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda")
            strSql.AppendLine(" INNER JOIN Movimenti_dettagli ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_dettagli.Id_mov = Movimenti.Id_mov")
            strSql.AppendLine(" INNER JOIN Mov_Destinazioni ON Movimenti_dettagli.PIVA = Mov_Destinazioni.PIVA AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Movimenti_dettagli.Id_mov = Mov_Destinazioni.Id_mov AND Movimenti_dettagli.Id_mov_det = Mov_Destinazioni.Id_mov_det ")
            strSql.AppendLine(" INNER JOIN UnitaMisura on UnitaMisura.UDM_COD =Movimenti_dettagli.Udm_Cod ")
            strSql.AppendLine(" LEFT JOIN Mov_dettaglio_tecnico ON Movimenti_dettagli.PIVA = Mov_dettaglio_tecnico.PIVA AND Movimenti_dettagli.Id_Agenda = Mov_dettaglio_tecnico.Id_Agenda AND Movimenti_dettagli.Id_mov = Mov_dettaglio_tecnico.Id_mov AND Movimenti_dettagli.Id_mov_det = Mov_dettaglio_tecnico.Id_mov_det")

            strSql.AppendLine(" WHERE Movimenti.data_movimento >= " & Agro_SQL_SaveDate(Data_Inizio))
            strSql.AppendLine(" AND   Movimenti.data_movimento <= " & Agro_SQL_SaveDate(Data_fine))

            strSql.AppendLine(" AND Movimenti.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            strSql.AppendLine(" AND Movimenti_dettagli.Sa_Cod = " & Agro_SQL_SaveNum(Sa_cod) & " ")
            strSql.AppendLine(" AND Movimenti.Cau_Mov = '" & Agro_SQL_SaveText(Cau_Mov) & "' ")
            strSql.AppendLine(" AND Agenda.lav_cod IN (" & Agro_SQL_Save_Clausola_IN(String.Join(",", lav_cod), False) & ") ")
            strSql.AppendLine(" AND Movimenti_dettagli.elem_cod = " & Agro_SQL_SaveNum(elem_cod))
            strSql.AppendLine(" AND Movimenti_dettagli.pro_cod = " & Agro_SQL_SaveNum(pro_cod))
            strSql.AppendLine(" AND Movimenti_dettagli.mat_cod = " & Agro_SQL_SaveNum(mat_cod))

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Movimenti.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Movimenti.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Movimenti.Piva, Data_Movimento ")
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

    'a differenza della precedente carica anche i dati del contatto
    Public Function MovimentiContabili_Contatto(ByVal Lav_Cod As Integer,
                                                ByVal Piva As String,
                                                ByVal Sa_Cod As Integer,
                                                ByVal Id_Agenda As Integer,
                                                ByVal Id_Mov As Integer,
                                                ByVal Cod_RisUm As Integer,
                                                ByVal Cau_Mov As String,
                                                ByVal Anno As Integer,
                                                ByVal Data_Movimento As Date,
                                                ByVal Scadenza As Date,
                                                ByVal Doc_Numero_Sin As String,
                                                ByVal Doc_Numero As Decimal,
                                                ByVal Doc_Numero_Des As String,
                                                ByVal Progr_Protocollo As Integer,
                                                ByVal Progr_Registrazione As Integer,
                                                ByVal Data_Registrazione As Date,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreParametri,
                                                    Optional ByVal DescrizioneOperazione_xDocumentale As Boolean = False,
                                                    Optional ByVal IdTipologia As Integer = 0,
                                                    Optional joinContatti As Boolean = True
                                                ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_R.MovimentiContabili_Contatto()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT * ")

            If DescrizioneOperazione_xDocumentale = True Then
                strSql.AppendLine(" , Operazioni.LAV_DES AS Operazione")
            End If

            'se si tratta di un contratto d'affitto, ne estrae la scadenza di testata
            If IdTipologia = enum_ID_Area_Tipologia.Contratti_Affitto Then
                strSql.AppendLine(" , m2.Scadenza AS Scadenza_Contratto")
            End If

            strSql.AppendLine(" FROM    Agenda  ")
            strSql.AppendLine(" INNER JOIN Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda")

            'mette in JOIN Movimenti su Cau_Mov = CAU_REGISTRAZIONE_SECONDARIA per poter settare la data scadenza di testata
            'dei contratti d'affitto
            If IdTipologia = enum_ID_Area_Tipologia.Contratti_Affitto Then
                strSql.AppendLine(" LEFT JOIN Movimenti m2 ON")
                strSql.AppendLine("           m2.PIVA = agenda.PIVA")
                strSql.AppendLine("           AND m2.Id_Agenda = agenda.id_agenda")
                strSql.AppendLine("           AND m2.Cau_Mov = '" & CAU_REGISTRAZIONE_SECONDARIA & "' ")
            End If

            If joinContatti Then
                strSql.AppendLine(" LEFT OUTER JOIN  Risorse_Umane ON Movimenti.Cod_RisUm = Risorse_Umane.Cod_RisUm ")
                strSql.AppendLine(" RIGHT OUTER JOIN Contatti ON Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto AND Risorse_Umane.Piva = Contatti.Piva ")
            End If

            If DescrizioneOperazione_xDocumentale = True Then
                strSql.AppendLine(" LEFT JOIN Operazioni ON Operazioni.LAV_COD = Agenda.Lav_Cod")
            End If

            strSql.AppendLine(" WHERE Movimenti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            strSql.AppendLine(" AND   Movimenti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Lav_Cod <> 0 Then
                strSql.AppendLine(" AND Agenda.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & "   ")
            End If

            If Piva <> "" Then
                strSql.AppendLine(" AND Movimenti.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND Movimenti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Id_Agenda <> 0 Then
                strSql.AppendLine(" AND Movimenti.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
            End If

            If Id_Mov <> 0 Then
                strSql.AppendLine(" AND Movimenti.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
            End If

            If Cod_RisUm <> 0 Then
                strSql.AppendLine(" AND Movimenti.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   ")
            End If

            If Cau_Mov <> "" Then
                strSql.AppendLine(" AND Movimenti.Cau_Mov = '" & UCase(Agro_SQL_SaveText(Cau_Mov)) & "'   ")
            End If

            If Anno <> 0 Then
                strSql.AppendLine(" AND Year(Movimenti.Data_Movimento) = " & Agro_SQL_SaveNum(Anno) & "  ")
            End If

            If Data_Movimento <> AGRODATAINIZIO Then
                strSql.AppendLine(" AND Movimenti.Data_Movimento = " & UCase(Agro_SQL_SaveDate(Data_Movimento)) & "   ")
            End If

            If Scadenza <> AGRODATAFINE AndAlso Scadenza <> AGRODATAINIZIO Then
                strSql.AppendLine(" AND Movimenti.Scadenza = " & UCase(Agro_SQL_SaveDate(Scadenza)) & "   ")
            End If

            If Doc_Numero_Sin.ToUpper <> "XYZ" Then
                strSql.AppendLine(" AND Movimenti.Doc_Numero_Sin = '" & UCase(Agro_SQL_SaveText(Doc_Numero_Sin)) & "'   ")
            End If

            If Doc_Numero <> 0 Then
                strSql.AppendLine(" AND Movimenti.Doc_Numero = " & Agro_SQL_SaveNum(Doc_Numero) & "   ")
            End If

            If Doc_Numero_Des.ToUpper <> "XYZ" Then
                strSql.AppendLine(" AND Movimenti.Doc_Numero_Des = '" & UCase(Agro_SQL_SaveText(Doc_Numero_Des)) & "'   ")
            End If

            If Progr_Protocollo <> 0 Then
                strSql.AppendLine(" AND Movimenti.Progr_Protocollo = " & Agro_SQL_SaveNum(Progr_Protocollo) & "   ")
            End If

            If Progr_Registrazione <> 0 Then
                strSql.AppendLine(" AND Movimenti.Progr_Registrazione = " & Agro_SQL_SaveNum(Progr_Registrazione) & "   ")
            End If

            If Data_Registrazione <> AGRODATAINIZIO Then
                strSql.AppendLine(" AND Movimenti.Data_Registrazione = " & UCase(Agro_SQL_SaveDate(Data_Registrazione)) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Movimenti.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Movimenti.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Movimenti.Piva, Movimenti.Data_Movimento, Movimenti.Doc_Numero_Sin, Movimenti.Doc_Numero, Movimenti.Doc_Numero_Des ASC")
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


    '#########################################################
    Public Function DocNumero_from_IdAgenda(ByVal Piva As String,
                                            ByVal Id_Agenda As Integer,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByRef objParametri As AgronicaCoreParametri
                                            ) As Decimal

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_R.DocNumero_from_IdAgenda()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim docNumero As Decimal = -1

        Try

            dt = MovimentiContabili(0,
                                    Piva,
                                    0,
                                    Id_Agenda,
                                    0, 0, CStr(CAU_REGISTRAZIONI), 0,
                                    AGRODATAINIZIO,
                                    AGRODATAFINE,
                                    "XYZ",
                                    0,
                                    "XYZ",
                                    0, 0,
                                    AGRODATAINIZIO,
                                    xFiltroAggiuntivo, "",
                                    objParametri)

            If Not IsNothing(dt) AndAlso dt.Rows.Count = 1 Then
                docNumero = dt.Rows(0).Item("Doc_Numero")
            End If

            If IsNothing(dt) OrElse dt.Rows.Count = 0 Then
                Throw New Exception("Nessun documento trovato")
            End If

            If dt.Rows.Count > 1 Then
                Throw New Exception("Sono stati trovati più documenti")
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return docNumero

    End Function

    '#########################################################
    Public Function IdMov_from_IdAgendaCauMov(ByVal Piva As String,
                                              ByVal Id_Agenda As Integer,
                                              ByVal Cau_Mov As String,
                                              ByVal xFiltroAggiuntivo As String,
                                              ByRef objParametri As AgronicaCoreParametri
                                              ) As Integer

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_R.IdMov_from_IdAgendaCauMov()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim idMov As Integer = -1

        Try

            dt = Leggi2(Piva,
                        0,
                        Id_Agenda,
                        0,
                        0,
                        Cau_Mov,
                        xFiltroAggiuntivo, "",
                        objParametri)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                idMov = dt.Rows(0).Item("Id_Mov")
            End If


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return idMov

    End Function

    Public Function Leggi_Extra_Str(ByVal Piva As String,
                                    ByVal Id_Agenda As Integer,
                                    ByVal Cau_Mov As String,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreParametri
                                    ) As String

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_R.IdMov_from_IdAgendaCauMov()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim extraStr As String = ""

        Try

            dt = Leggi2(Piva,
                        0,
                        Id_Agenda,
                        0,
                        0,
                        Cau_Mov,
                        xFiltroAggiuntivo, "",
                        objParametri)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                extraStr = dt.Rows(0).Item("Extra_Str")
            End If


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return extraStr

    End Function


    '##############################################################################################
    '##############################################################################################
    '##############################################################################################
    '##############################################################################################
    '##############################################################################################

    ' -----------------------------------------------------------------------------
    ' <summary>
    ' chiama la funzione del core AgronicaStampeDAL che legge i movimenti di 
    ' magazzino
    ' </summary>
    ' -----------------------------------------------------------------------------
    'default lotto=LOTTO_NONDEFINITO
    Public Function SchedaMovimentiMagazzino(ByVal Data_Inizio As Date,
                                             ByVal Data_Fine As Date,
                                             ByVal Piva As String,
                                             ByVal Sa_Cod As Integer,
                                             ByVal Id_Destinazione As Integer,
                                             ByVal Elem_Cod As Integer,
                                             ByVal Pro_Cod As Integer,
                                             ByVal Mat_Cod As Integer,
                                             ByVal Cal_Cod As Integer,
                                             ByVal Cod_Progetto As Integer,
                                             ByVal Fase_Cod As Integer,
                                             ByVal Udm_Cod As Integer,
                                             ByVal Lotto As String,
                                             ByVal xFiltroAggiuntivo As String,
                                             ByVal xFiltroAggiuntivo_1 As String,
                                             ByVal xFiltroAggiuntivo_2 As String,
                                             ByVal xFiltroAggiuntivo_3 As String,
                                             ByVal xFiltroAggiuntivo_4 As String,
                                             ByVal xFiltroAggiuntivo_5 As String,
                                             ByVal xFiltroAggiuntivo_6 As String,
                                             ByVal xFiltroAggiuntivo_7 As String,
                                             ByVal xFiltroAggiuntivo_8 As String,
                                             ByVal xFiltroAggiuntivo_9 As String,
                                             ByVal xFiltroAggiuntivo_10 As String,
                                             ByVal xOrderBy As String,
                                             ByRef objParametriServer As AgronicaCoreParametri,
                                             ByRef objParametriUtenti As AgronicaCoreParametri,
                                             Optional ByVal xFiltroAggiuntivo_13 As String = "",
                                             Optional ByVal xFiltroAggiuntivo_14 As String = "",
                                             Optional ByVal flagRecuperaCodArticolo As Boolean = False,
                                             Optional ByVal codArticolo As String = "",
                                             Optional ByVal cercaCodArticoloPerLike As Boolean = False,
                                             Optional ByVal isFreshAndFood As Boolean = False,
                                             Optional ByVal cercaLottoPerLike As Boolean = False,
                                             Optional ByVal xFiltroAggiuntivo_15 As String = "",
                                             Optional ByVal gruppiMerceDefaultPerCategoria As List(Of ImpostazioneDefault_GruppiMerce) = Nothing
                                             ) As DataTable

        Dim objStampe As New AgronicaCoreStampeDAL.Magazzino

        'aggiungere variabile xFiltroAggiuntivo_12 se occorre mandare anche il filtro su confezioni prodotti
        Return objStampe.SchedaMovimentiMagazzino(Data_Inizio, Data_Fine, Piva, Sa_Cod, Id_Destinazione, Elem_Cod,
                                                  Pro_Cod, Mat_Cod, Cal_Cod, Cod_Progetto, Fase_Cod, Udm_Cod,
                                                  Lotto, xFiltroAggiuntivo, xFiltroAggiuntivo_1, xFiltroAggiuntivo_2,
                                                  xFiltroAggiuntivo_3, xFiltroAggiuntivo_4, xFiltroAggiuntivo_5,
                                                  xFiltroAggiuntivo_6, xFiltroAggiuntivo_7, xFiltroAggiuntivo_8,
                                                  xFiltroAggiuntivo_9, xFiltroAggiuntivo_10, "",
                                                  xOrderBy, objParametriServer, objParametriUtenti,
                                                  xFiltroAggiuntivo_13, xFiltroAggiuntivo_14,
                                                  flagRecuperaCodArticolo, codArticolo, cercaCodArticoloPerLike,
                                                  isFreshAndFood, cercaLottoPerLike, xFiltroAggiuntivo_15, gruppiMerceDefaultPerCategoria:=gruppiMerceDefaultPerCategoria)
    End Function



    Function Esiste_Almeno_Un_Doc_Contabile_Riferito(ByVal piva As String,
                                                     ByVal idAgenda As Integer,
                                                     ByVal xFiltroAggiuntivo As String,
                                                     ByRef docNumero As Decimal,
                                                     ByRef objParametri As AgronicaCoreParametri
                                                     ) As Decimal

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_R.Esiste_Un_Doc_Contabile_Riferito()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        'Dim resdefault As Decimal = -1

        Try

            dt = MovimentiContabili(0,
                                    piva,
                                    0,
                                    idAgenda,
                                    0, 0, CStr(CAU_REGISTRAZIONI), 0,
                                    AGRODATAINIZIO,
                                    AGRODATAFINE,
                                    "XYZ",
                                    0,
                                    "XYZ",
                                    0, 0,
                                    AGRODATAINIZIO,
                                    xFiltroAggiuntivo, "",
                                    objParametri)

            If Not IsNothing(dt) AndAlso dt.Rows.Count = 1 Then
                docNumero = dt.Rows(0).Item("Doc_Numero")
                Return True
            End If

            If IsNothing(dt) OrElse dt.Rows.Count = 0 Then
                docNumero = -1
                Return False
            End If

            If dt.Rows.Count > 1 Then
                docNumero = -1
                'Dim i As Integer
                For i As Integer = 0 To dt.Rows.Count - 1
                    'do' priorità a i ddt reali se ce se sono di piu
                    If IsNumeric(dt.Rows(0).Item("Doc_Numero")) AndAlso dt.Rows(0).Item("Doc_Numero") > 0 Then
                        docNumero = dt.Rows(0).Item("Doc_Numero")
                        Return True
                    End If
                Next
                Return True
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try
        Return False

    End Function

    ''' <summary>
    ''' Restituisce l'elenco dei prefissi o dei suffissi di doc_numero relativi alle causali ed ai codici lavorazioni passati
    ''' </summary>
    ''' <param name="piva"></param>
    ''' <param name="sinDes">Se minore di zero restituisce il campo prefisso (doc_numero_sin), altrimenti il suffisso (doc_numero_des)</param>
    ''' <param name="arrCauMov"></param>
    ''' <param name="arrLavCod"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    Public Function LeggiDistinctDocNumeroSinDes(ByVal piva As String, ByVal sinDes As Integer, ByVal arrCauMov As String(), ByVal arrLavCod As Integer(), ByRef objParametri As AgronicaCoreParametri) As DataTable
        Dim methodInfo = Reflection.MethodBase.GetCurrentMethod()
        Dim NomeRoutine As String = methodInfo.ReflectedType.FullName & "." & methodInfo.Name

        Dim dt As DataTable

        Dim strSql As New StringBuilder()

        Try
            Dim campoSinDes As String
            If sinDes < 0 Then
                campoSinDes = "Movimenti.Doc_Numero_Sin"
            Else
                campoSinDes = "Movimenti.Doc_Numero_Des"
            End If

            strSql.AppendLine("SELECT DISTINCT " & campoSinDes)
            strSql.AppendLine("FROM Movimenti")
            strSql.AppendLine("INNER JOIN Agenda")
            strSql.AppendLine("ON Movimenti.Piva = Agenda.Piva AND Movimenti.Id_Agenda = Agenda.Id_Agenda") 'Non è necessario l'on sul sa_cod

            If DataProviderFactory.Instance.TipoProvider = TipiEnumerativi.enum_DataProvidersType.OleDbProvider Then
                strSql.AppendLine(String.Format(
                    "WHERE Movimenti.Piva = '{0}' AND Movimenti.Cau_Mov IN ({1}) AND Agenda.Lav_Cod IN ({2})",
                    piva,
                    "'" & Agro_SQL_Save_Clausola_IN(String.Join("','", arrCauMov), True) & "'",
                    Agro_SQL_Save_Clausola_IN(String.Join(",", arrLavCod))
                ))
            Else
                If DataProviderFactory.Instance.ParametrizzaQuery Then
                    strSql.AppendLine(String.Format(
                   "WHERE Movimenti.Piva = '{0}' AND Movimenti.Cau_Mov IN ({1}) AND Agenda.Lav_Cod IN ({2})",
                   piva,
                   Agro_SQL_Save_Clausola_IN(String.Join(",", arrCauMov), True),
                   Agro_SQL_Save_Clausola_IN(String.Join(",", arrLavCod))
                ))
                Else
                    strSql.AppendLine(String.Format(
                        "WHERE Movimenti.Piva = '{0}' AND Movimenti.Cau_Mov IN ({1}) AND Agenda.Lav_Cod IN ({2})",
                    piva,
                    "'" & Agro_SQL_Save_Clausola_IN(String.Join("','", arrCauMov), True) & "'",
                    Agro_SQL_Save_Clausola_IN(String.Join(",", arrLavCod))
                ))
                End If
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Dim messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, messaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt
    End Function


    Public Function Leggi_NumProtocollo(ByVal Piva As String,
                                        ByVal Sa_Cod As Integer,
                                        ByVal Id_Agenda As Integer,
                                        ByRef objParametri As AgronicaCoreParametri
                                        ) As Integer

        Const nomeRoutine = "AgronicaCoreContabDAL.Movimenti_R.Leggi_NumProtocollo()"

        Dim regolamento_cod As Integer = 0

        Try

            Dim dt = Leggi(Piva, Sa_Cod, Id_Agenda, 0, 0, "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)

            If dt.Rows.Count > 0 Then
                regolamento_cod = dt.Rows(0).Item("Num_Protocollo")
            End If

        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return regolamento_cod

    End Function

    Public Function Ottieni_Qta_per_Impianti(ByVal Piva As String,
                                            ByVal Id_Agenda As Integer,
                                            ByVal Validita_Inizio As Date,
                                            ByVal Validita_Fine As Date,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_R.Ottieni_Qta_per_Impianti()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT Agenda.Lav_Cod, Mov_Destinazioni.Id_Agenda, Movimenti_dettagli.Pro_Cod, Mov_Destinazioni.Piva,Mov_Destinazioni.Sa_Cod,Mov_Destinazioni.Appezza, ")
            strSql.AppendLine(" Mov_Destinazioni.Id_Destinazione, Mov_Destinazioni.Qta, Mov_Destinazioni.Qta2, ISNULL(Mov_Dettaglio_Tecnico.Qta_Ril,0) AS Qta_Ril")
            strSql.AppendLine(" FROM    Movimenti  ")
            strSql.AppendLine(" INNER JOIN Agenda")
            strSql.AppendLine(" ON Agenda.Piva = Movimenti.Piva and")
            strSql.AppendLine(" Agenda.Id_Agenda = Movimenti.Id_Agenda")
            strSql.AppendLine(" INNER JOIN Movimenti_dettagli")
            strSql.AppendLine(" ON Movimenti_dettagli.Piva = Movimenti.Piva and")
            strSql.AppendLine(" Movimenti_dettagli.Sa_Cod = Movimenti.Sa_Cod and ")
            strSql.AppendLine(" Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov  ")
            strSql.AppendLine(" INNER JOIN Mov_Destinazioni  ")
            strSql.AppendLine(" ON Mov_Destinazioni.Piva = Movimenti_dettagli.Piva and  ")
            strSql.AppendLine(" Mov_Destinazioni.Sa_Cod = Movimenti_dettagli.Sa_Cod and   ")
            strSql.AppendLine(" Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov and ")
            strSql.AppendLine(" Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det ")
            strSql.AppendLine(" LEFT JOIN Mov_Dettaglio_Tecnico  ")
            strSql.AppendLine(" ON Mov_Dettaglio_Tecnico.Piva = Movimenti_dettagli.Piva and   ")
            strSql.AppendLine(" Mov_Dettaglio_Tecnico.Sa_Cod = Movimenti_dettagli.Sa_Cod and")
            strSql.AppendLine(" Mov_Dettaglio_Tecnico.Id_Mov = Movimenti_dettagli.Id_Mov and")
            strSql.AppendLine(" Mov_Dettaglio_Tecnico.Id_Mov_Det = 0")
            strSql.AppendLine(" where Movimenti.Cau_Mov = '" & CAU_TRATTAMENTO & "' or Movimenti.Cau_Mov = '" & CAU_LAVORAZIONE & "'")
            strSql.AppendLine(" and Mov_Destinazioni.Tipo_Destinazione = 0 and Movimenti_dettagli.Pro_Cod > 0 and Movimenti_dettagli.Pro_Cod IS NOT NULL")

            If Piva <> "" Then
                strSql.AppendLine(" AND Movimenti.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Id_Agenda <> 0 Then
                strSql.AppendLine(" AND Movimenti.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
            End If

            If Validita_Fine <> AGRODATAFINE Then
                strSql.AppendLine(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            End If

            If Validita_Inizio <> AGRODATAINIZIO Then
                strSql.AppendLine(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

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

    Public Function Ottieni_Validita_Rilievi_Fasi_Fenologiche_per_Impianti(ByVal Piva As String,
                                                                            ByVal Id_Agenda As Integer,
                                                                            ByVal Validita_Inizio As Date,
                                                                            ByVal Validita_Fine As Date,
                                                                            ByVal xFiltroAggiuntivo As String,
                                                                            ByVal xOrderBy As String,
                                                                            ByRef objParametri As AgronicaCoreParametri,
                                                                            Optional ByVal IDTestataTemp As Integer = 0) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_R.Ottieni_Validita_Rilievi_Fasi_Fenologiche_per_Impianti()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT Agenda.Id_Agenda, Mov_Destinazioni.Piva, Mov_Destinazioni.Sa_Cod, ")
            strSql.AppendLine(" Mov_Destinazioni.Appezza, Mov_Destinazioni.Id_Destinazione, Stadi_Crescita_BBCH.Stadio_Principale,")
            strSql.AppendLine(" SpecieVegetaliXStadiCrescita.Descrizione As Descrizione_StadiCrescita, Mov_Destinazioni.Validita_Inizio, Agenda.Des_Lib")
            strSql.AppendLine(" FROM    Movimenti WITH(NOLOCK) ")
            strSql.AppendLine(" INNER JOIN Agenda WITH(NOLOCK)")
            strSql.AppendLine(" ON Movimenti.PIVA = Agenda.PIVA AND Movimenti.Sa_Cod = Agenda.Sa_Cod AND Movimenti.Id_Agenda = Agenda.Id_Agenda")
            strSql.AppendLine(" INNER JOIN Movimenti_dettagli WITH(NOLOCK)")
            strSql.AppendLine(" ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod AND")
            strSql.AppendLine(" Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov")
            strSql.AppendLine(" INNER JOIN Mov_Destinazioni WITH(NOLOCK) ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod AND ")
            strSql.AppendLine(" Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND Movimenti_dettagli.Id_Mov_Det = dbo.Mov_Destinazioni.Id_Mov_Det")
            strSql.AppendLine(" INNER JOIN Mov_Dettaglio_Tecnico WITH(NOLOCK) ON Movimenti_dettagli.PIVA = Mov_Dettaglio_Tecnico.Piva AND Movimenti_dettagli.Sa_Cod = Mov_Dettaglio_Tecnico.Sa_Cod AND Movimenti_dettagli.Id_Agenda = Mov_Dettaglio_Tecnico.Id_Agenda AND")
            strSql.AppendLine(" Movimenti_dettagli.Id_Mov = Mov_Dettaglio_Tecnico.Id_Mov AND Movimenti_dettagli.Id_Mov_Det = Mov_Dettaglio_Tecnico.Id_Mov_Det")
            strSql.AppendLine(" INNER JOIN SpecieVegetaliXStadiCrescita WITH(NOLOCK) ON Mov_Dettaglio_Tecnico.FF_Classe = SpecieVegetaliXStadiCrescita.Cod_SS")
            strSql.AppendLine(" INNER JOIN Stadi_Crescita_BBCH WITH(NOLOCK) ON Stadi_Crescita_BBCH.ID_BBCH = SpecieVegetaliXStadiCrescita.ID_BBCH ")

            If IDTestataTemp > 0 Then
                strSql.AppendLine("INNER JOIN __tmp_Agenda agnFiltro WITH(NOLOCK)")
                strSql.AppendLine("ON Agenda.piva = agnFiltro.Piva  ")
                strSql.AppendLine("AND Agenda.id_agenda = agnFiltro.id_agenda  ")
                strSql.AppendLine("AND agnFiltro.IDTestataTemp = " & IDTestataTemp)
            End If

            strSql.AppendLine(" WHERE (Agenda.Lav_Cod = " & LAVCOD_FASI_FENOLOGICHE & ") ")
            strSql.AppendLine(" AND (Movimenti.Cau_Mov = '" & CAU_RILIEVO_CAMPO & "')")
            strSql.AppendLine(" AND (Mov_Dettaglio_Tecnico.FF_Classe <> 0)")

            If Piva <> "" Then
                strSql.AppendLine(" AND Movimenti.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Id_Agenda <> 0 Then
                strSql.AppendLine(" AND Movimenti.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
            End If

            If Validita_Fine <> AGRODATAFINE Then
                strSql.AppendLine(" AND (Mov_Destinazioni.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & ")  ")
            End If

            If Validita_Inizio <> AGRODATAINIZIO Then
                strSql.AppendLine(" AND (Mov_Destinazioni.Validita_Inizio >= " & Agro_SQL_SaveDate(Validita_Inizio) & ")  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            strSql.AppendLine(" GROUP BY Agenda.Id_Agenda, Mov_Destinazioni.Piva, Mov_Destinazioni.Sa_Cod,Mov_Destinazioni.Appezza,")
            strSql.AppendLine("  Mov_Destinazioni.Id_Destinazione,Stadi_Crescita_BBCH.Stadio_Principale,")
            strSql.AppendLine("  SpecieVegetaliXStadiCrescita.Descrizione,Mov_Destinazioni.Validita_Inizio, Agenda.Des_Lib")
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



'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§



Public Class Movimenti_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal Piva As String,
                           ByVal Sa_Cod As Long,
                           ByVal Id_Agenda As Integer,
                           ByVal Id_Mov As Integer,
                           ByVal Cod_RisUm As Integer,
                           ByVal Cau_Mov As String,
                           ByVal Mov_Desc As String,
                           ByVal Data_Movimento As Date,
                           ByVal Scadenza As Date,
                           ByVal Scadenza_Extra As Date,
                           ByVal Doc_Numero As Decimal,
                           ByVal Num_Protocollo As Decimal,
                           ByVal Cod_IndirizzoRisUm As Integer,
                           ByVal Cod_Destinazione As Integer,
                           ByVal Cod_IndirizzoDestinazione As Integer,
                           ByVal Mezzo As Integer,
                           ByVal Cod_Vettore As Integer,
                           ByVal Cod_IndirizzoVettore As Integer,
                           ByVal Causale_Trasporto As String,
                           ByVal Aspetto As String,
                           ByVal Peso As Decimal,
                           ByVal Ora As Date,
                           ByVal Colli As Integer,
                           ByVal Tipo_Sconto As Integer,
                           ByVal Extra_Str As String,
                           ByVal Extra_Int As Integer,
                           ByVal Extra_Date As Date,
                           ByVal Doc_Numero_Sin As String,
                           ByVal Doc_Numero_Des As String,
                           ByVal Natura_Beni As String,
                           ByVal Tara_Veicolo As Decimal,
                           ByVal Tara_Imballi As Decimal,
                           ByVal Tipo_Peso As Integer,
                           ByVal Modalita As Integer,
                           ByVal Username_Note As String,
                           ByVal Progr_Protocollo As Integer,
                           ByVal Progr_Registrazione As Integer,
                           ByVal Data_Registrazione As Date,
                           ByVal ChkLayOut_Bypass_Fatturato As Integer,
                           ByVal ChkLayOut_Join_Prodotti As Integer,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByVal Disciplinare_PubblicoPrivato As Integer,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = AgroDataInizializzata,
                           Optional ByVal Data_modifica As DateTime = AgroDataInizializzata,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = "",
                           Optional ByVal Sezionale_Cod As Integer = 0,
                           Optional ByVal Causale_Trasporto_Cod As Integer = 0,
                           Optional ByVal Cod_RisUm_Altro As Integer = 0,
                           Optional ByVal ChkLayOut_Peso As Integer = 0,
                           Optional ByVal ChkLayOut_Prezzo As Integer = 0,
                           Optional ByVal ChkFiltro_Varietale As Integer = 0,
                           Optional ByVal ChkLayOut_Litri As Integer = 0,
                           Optional ByVal Flag_Usa_Ora_Reale As Boolean = False,
                           Optional ByVal Cod_RisUm_Aggiuntivo As Integer = 0,
                           Optional ByVal Cod_IndirizzoAggiuntivo As Integer = 0,
                           Optional ByVal ChkLayOut_Riscontrato As Integer = 0,
                           Optional ByVal Doc_Numero_Visualizzato As String = "",
                           Optional ByVal Cod_Macchina_Lav As String = "",
                           Optional ByVal TipoDocumento As Integer = 0,
                           Optional ByVal OraFine As DateTime = Nothing,
                           Optional ByVal Modalita_Applicazione As Integer = 0
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Movimenti_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False
        Dim strOra As String

        Dim scadenzaXOra As Date = Scadenza.ToShortDateString

        Try

            strOra = Ora.ToLongTimeString


            If Data_creazione = AgroDataInizializzata Then
                Data_creazione = Date.Now
            End If

            If Data_modifica = AgroDataInizializzata Then
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
            strSql.AppendLine(" INSERT INTO Movimenti ")
            strSql.AppendLine("             (Piva,         Sa_Cod,         Id_Agenda,     Id_Mov,           ")
            strSql.AppendLine("              Cod_RisUm,    Cau_Mov,        Mov_Desc,      Data_Movimento,   ")
            strSql.AppendLine("              Scadenza,     Scadenza_Extra, Doc_Numero,    Num_Protocollo,   ")
            strSql.AppendLine("              Cod_IndirizzoRisUm, Cod_Destinazione,  Cod_IndirizzoDestinazione,  ")
            strSql.AppendLine("              Mezzo,              Cod_Vettore,       Cod_IndirizzoVettore,       ")
            strSql.AppendLine("              Causale_Trasporto,  Aspetto,           Peso,                       ")
            strSql.AppendLine("              Ora,                Colli,             Tipo_Sconto,                ")
            strSql.AppendLine("              Extra_Str,          Extra_Int,         Extra_Date,                 ")
            strSql.AppendLine("              Doc_Numero_Sin,     Doc_Numero_Des,    Natura_Beni,                Tara_Veicolo,       ")
            strSql.AppendLine("              Tara_Imballi,       Tipo_Peso,         Modalita,                   ")
            strSql.AppendLine("              Username_Note,      Progr_Protocollo,  Progr_Registrazione,        Data_Registrazione, ")
            strSql.AppendLine("              ChkLayOut_Bypass_Fatturato,       ChkLayOut_Join_Prodotti,         ")
            strSql.AppendLine("              Sezionale_Cod,       Causale_Trasporto_Cod,         ")
            strSql.AppendLine("              Cod_RisUm_Altro,       ChkFiltro_Varietale,         ")
            strSql.AppendLine("              ChkLayOut_Peso,        ChkLayOut_Prezzo,         ChkLayOut_Litri, ")
            strSql.AppendLine("              Cod_RisUm_Aggiuntivo,  Cod_Indirizzo_Aggiuntivo, ChkLayOut_Riscontrato, ")
            strSql.AppendLine("              Doc_Numero_Visualizzato,  Cod_Macchina_Lav, TipoDocumento, OraFine, Modalita_Applicazione, ")

            strSql.AppendLine("              Inviato,            DataInvio, ")
            strSql.AppendLine("              Data_Creazione,     Data_Modifica, ")
            strSql.AppendLine("              UserName_Creazione, UserName_Modifica, ")
            strSql.AppendLine("              Validita_Inizio,    Validita_Fine, Disciplinare_PubblicoPrivato ")
            strSql.AppendLine("              ) ")

            strSql.AppendLine(" VALUES (")
            strSql.AppendLine("          '" & Agro_SQL_SaveText(Piva) & "'  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Id_Agenda) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Id_Mov) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Cod_RisUm) & "  ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(Cau_Mov) & "'  ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(Mov_Desc) & "'  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Data_Movimento) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Scadenza) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Scadenza_Extra) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Doc_Numero) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Num_Protocollo) & "  ")

            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Cod_IndirizzoRisUm) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Cod_Destinazione) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Cod_IndirizzoDestinazione) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Mezzo) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Cod_Vettore) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Cod_IndirizzoVettore) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Causale_Trasporto) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Aspetto) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Peso) & "  ")

            '  Giulia, 15/02/2017 11:19:05: per i movimenti contabili (nati dal LAN) nel campo "Ora" c'è la data di spedizione mentre "Scadenza" è AGRODATAFINE
            '           quindi non posso fare questo replace che invece va fatto nel mondo Online-Campagna
            If Flag_Usa_Ora_Reale = True Then
                strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Ora) & "  ")
            Else
                strSql.AppendLine("         , " & Replace(Agro_SQL_SaveDateTime(CDate(scadenzaXOra) & " " & strOra), ".", ":") & "  ")
            End If


            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Colli) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Tipo_Sconto) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Extra_Str) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Extra_Int) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Extra_Date) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Doc_Numero_Sin) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Doc_Numero_Des) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Natura_Beni) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Tara_Veicolo) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Tara_Imballi) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Tipo_Peso) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Modalita) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Username_Note) & "' ")

            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Progr_Protocollo) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Progr_Registrazione) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Data_Registrazione) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(ChkLayOut_Bypass_Fatturato) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(ChkLayOut_Join_Prodotti) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Sezionale_Cod) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Causale_Trasporto_Cod) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Cod_RisUm_Altro) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(ChkFiltro_Varietale) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(ChkLayOut_Peso) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(ChkLayOut_Prezzo) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(ChkLayOut_Litri) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Cod_RisUm_Aggiuntivo) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Cod_IndirizzoAggiuntivo) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(ChkLayOut_Riscontrato) & "  ")
            strSql.AppendLine("			,'" & Agro_SQL_SaveText(Doc_Numero_Visualizzato) & "' ")
            strSql.AppendLine("			,'" & Agro_SQL_SaveText(Cod_Macchina_Lav) & "' ")
            strSql.AppendLine("			,'" & Agro_SQL_SaveText(TipoDocumento) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime_NULL(OraFine) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Modalita_Applicazione) & "  ")

            strSql.AppendLine("         , 0  ")
            strSql.AppendLine("         , Null  ")

            strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            strSql.AppendLine("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            strSql.AppendLine("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")

            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Disciplinare_PubblicoPrivato) & "  ")

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

    <Obsolete("Utilizzare Movimenti_W.ModificaPuntuale, che contiene tutti i campi")>
    Public Function Modifica(ByVal Piva As String,
                             ByVal Sa_Cod As Integer,
                             ByVal Id_Agenda As Integer,
                             ByVal Id_Mov As Integer,
                             ByVal Cod_RisUm As Integer,
                             ByVal Cau_Mov As String,
                             ByVal Mov_Desc As String,
                             ByVal Data_Movimento As Date,
                             ByVal Scadenza As Date,
                             ByVal Scadenza_Extra As Date,
                             ByVal Doc_Numero As Decimal,
                             ByVal Num_Protocollo As Decimal,
                             ByVal Cod_IndirizzoRisUm As Integer,
                             ByVal Cod_Destinazione As Integer,
                             ByVal Cod_IndirizzoDestinazione As Integer,
                             ByVal Mezzo As Integer,
                             ByVal Cod_Vettore As Integer,
                             ByVal Cod_IndirizzoVettore As Integer,
                             ByVal Causale_Trasporto As String,
                             ByVal Aspetto As String,
                             ByVal Peso As Decimal,
                             ByVal Ora As Date,
                             ByVal Colli As Integer,
                             ByVal Tipo_Sconto As Integer,
                             ByVal Extra_Str As String,
                             ByVal Extra_Int As Integer,
                             ByVal Extra_Date As Date,
                             ByVal Doc_Numero_Sin As String,
                             ByVal Doc_Numero_Des As String,
                             ByVal Natura_Beni As String,
                             ByVal Tara_Veicolo As Decimal,
                             ByVal Tara_Imballi As Decimal,
                             ByVal Tipo_Peso As Integer,
                             ByVal Modalita As Integer,
                             ByVal Username_Note As String,
                             ByVal Progr_Protocollo As Integer,
                             ByVal Progr_Registrazione As Integer,
                             ByVal Data_Registrazione As Date,
                             ByVal ChkLayOut_Bypass_Fatturato As Integer,
                             ByVal ChkLayOut_Join_Prodotti As Integer,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal Disciplinare_PubblicoPrivato As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Movimenti_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False
        Dim strOra As String

        Try

            strOra = Ora.ToLongTimeString

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            'If Sa_Cod = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            'End If

            If Id_Agenda = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Agenda obbligatorio)")
            End If

            If Id_Mov = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Mov obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0
            strSql.AppendLine(" UPDATE Movimenti SET ")
            strSql.AppendLine("    Cod_RisUm                   =  " & Agro_SQL_SaveNum(Cod_RisUm) & "  ")
            strSql.AppendLine("   ,Cau_Mov                     = '" & Agro_SQL_SaveText(Cau_Mov) & "'  ")
            strSql.AppendLine("   ,Mov_Desc                    = '" & Agro_SQL_SaveText(Mov_Desc) & "'  ")
            strSql.AppendLine("   ,Data_Movimento              =  " & Agro_SQL_SaveDate(Data_Movimento) & "  ")
            strSql.AppendLine("   ,Scadenza                    =  " & Agro_SQL_SaveDate(Scadenza) & "  ")
            strSql.AppendLine("   ,Scadenza_Extra              =  " & Agro_SQL_SaveDate(Scadenza_Extra) & "  ")
            strSql.AppendLine("   ,Doc_Numero                  =  " & Agro_SQL_SaveText(Doc_Numero) & "  ")
            strSql.AppendLine("   ,Num_Protocollo              =  " & Agro_SQL_SaveText(Num_Protocollo) & "  ")
            strSql.AppendLine("   ,Cod_IndirizzoRisUm          =  " & Agro_SQL_SaveNum(Cod_IndirizzoRisUm) & "  ")
            strSql.AppendLine("   ,Cod_Destinazione            =  " & Agro_SQL_SaveNum(Cod_Destinazione) & "  ")
            strSql.AppendLine("   ,Cod_IndirizzoDestinazione   =  " & Agro_SQL_SaveNum(Cod_IndirizzoDestinazione) & "  ")
            strSql.AppendLine("   ,Mezzo                       =  " & Agro_SQL_SaveNum(Mezzo) & "  ")
            strSql.AppendLine("   ,Cod_Vettore                 =  " & Agro_SQL_SaveNum(Cod_Vettore) & "  ")
            strSql.AppendLine("   ,Cod_IndirizzoVettore        =  " & Agro_SQL_SaveNum(Cod_IndirizzoVettore) & "  ")
            strSql.AppendLine("   ,Causale_Trasporto           = '" & Agro_SQL_SaveText(Causale_Trasporto) & "'  ")
            strSql.AppendLine("   ,Aspetto                     = '" & Agro_SQL_SaveText(Aspetto) & "'  ")
            strSql.AppendLine("   ,Peso                        =  " & Agro_SQL_SaveNum(Peso) & "  ")
            strSql.AppendLine("   ,Ora                         =  " & Replace(Agro_SQL_SaveDateTime(Scadenza & " " & strOra), ".", ":") & "  ")
            'StrSQL.AppendLine("   ,Ora                         =  " & Agro_SQL_SaveDateTime(Data_Movimento, CStr(Ora)) & "  ")
            strSql.AppendLine("   ,Colli                       =  " & Agro_SQL_SaveNum(Colli) & "  ")
            strSql.AppendLine("   ,Tipo_Sconto                 =  " & Agro_SQL_SaveNum(Tipo_Sconto) & "  ")
            strSql.AppendLine("   ,Extra_Str                   = '" & Agro_SQL_SaveText(Extra_Str) & "'  ")
            strSql.AppendLine("   ,Extra_Int                   =  " & Agro_SQL_SaveNum(Extra_Int) & "  ")
            strSql.AppendLine("   ,Extra_Date                  =  " & Agro_SQL_SaveDate(Extra_Date))
            strSql.AppendLine("   ,Doc_Numero_Sin              = '" & Agro_SQL_SaveText(Doc_Numero_Sin) & "'  ")
            strSql.AppendLine("   ,Doc_Numero_Des              = '" & Agro_SQL_SaveText(Doc_Numero_Des) & "'  ")
            strSql.AppendLine("   ,Natura_Beni                 = '" & Agro_SQL_SaveText(Natura_Beni) & "'  ")
            strSql.AppendLine("   ,Tara_Veicolo                =  " & Agro_SQL_SaveNum(Tara_Veicolo) & "  ")
            strSql.AppendLine("   ,Tara_Imballi                =  " & Agro_SQL_SaveNum(Tara_Imballi) & "  ")
            strSql.AppendLine("   ,Tipo_Peso                   =  " & Agro_SQL_SaveNum(Tipo_Peso) & "  ")
            strSql.AppendLine("   ,Modalita                    =  " & Agro_SQL_SaveNum(Modalita) & "  ")
            strSql.AppendLine("   ,UserName_Note               = '" & Agro_SQL_SaveText(Username_Note) & "'  ")
            strSql.AppendLine("   ,Progr_Protocollo            =  " & Agro_SQL_SaveNum(Progr_Protocollo) & "  ")
            strSql.AppendLine("   ,Progr_Registrazione         =  " & Agro_SQL_SaveNum(Progr_Registrazione) & "  ")
            strSql.AppendLine("   ,Data_Registrazione          =  " & Agro_SQL_SaveDateTime(Data_Registrazione, CStr(Ora)) & "  ")
            strSql.AppendLine("   ,ChkLayOut_Bypass_Fatturato  =  " & Agro_SQL_SaveNum(ChkLayOut_Bypass_Fatturato) & "  ")
            strSql.AppendLine("   ,ChkLayOut_Join_Prodotti     =  " & Agro_SQL_SaveNum(ChkLayOut_Join_Prodotti) & "  ")

            strSql.AppendLine("   ,Inviato           =  0 ")
            strSql.AppendLine("   ,DataInvio         =  Null ")
            strSql.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))
            strSql.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            strSql.AppendLine("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.AppendLine("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            strSql.AppendLine("   ,Disciplinare_PubblicoPrivato     =  " & Agro_SQL_SaveNum(Disciplinare_PubblicoPrivato) & "  ")

            strSql.AppendLine(" WHERE Piva      = '" & Agro_SQL_SaveText(Piva) & "'  ")
            strSql.AppendLine(" AND   Sa_Cod    =  " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            strSql.AppendLine(" AND   Id_Agenda =  " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
            strSql.AppendLine(" AND   Id_Mov =  " & Agro_SQL_SaveNum(Id_Mov) & "   ")


            '----------------------------------------------------------------------
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

    Public Function ModificaPuntuale(ByVal Piva As String,
                                     ByVal Sa_Cod As Integer,
                                     ByVal Id_Agenda As Integer,
                                     ByVal Id_Mov As Integer,
                                     ByRef objParametri As AgronicaCoreParametri,
                                     Optional ByVal Cod_RisUm As Integer? = Nothing,
                                     Optional ByVal Cau_Mov As String = Nothing,
                                     Optional ByVal Mov_Desc As String = Nothing,
                                     Optional ByVal Data_Movimento As Date? = Nothing,
                                     Optional ByVal Scadenza As Date? = Nothing,
                                     Optional ByVal Scadenza_Extra As Date? = Nothing,
                                     Optional ByVal Doc_Numero As Decimal? = Nothing,
                                     Optional ByVal Num_Protocollo As Decimal? = Nothing,
                                     Optional ByVal Cod_IndirizzoRisUm As Integer? = Nothing,
                                     Optional ByVal Cod_Destinazione As Integer? = Nothing,
                                     Optional ByVal Cod_IndirizzoDestinazione As Integer? = Nothing,
                                     Optional ByVal Mezzo As Integer? = Nothing,
                                     Optional ByVal Cod_Vettore As Integer? = Nothing,
                                     Optional ByVal Cod_IndirizzoVettore As Integer? = Nothing,
                                     Optional ByVal Causale_Trasporto As String = Nothing,
                                     Optional ByVal Aspetto As String = Nothing,
                                     Optional ByVal Peso As Decimal? = Nothing,
                                     Optional ByVal Ora As DateTime? = Nothing,
                                     Optional ByVal Colli As Integer? = Nothing,
                                     Optional ByVal Tipo_Sconto As Integer? = Nothing,
                                     Optional ByVal Extra_Str As String = Nothing,
                                     Optional ByVal Extra_Int As Integer? = Nothing,
                                     Optional ByVal Extra_Date As Date? = Nothing,
                                     Optional ByVal Doc_Numero_Sin As String = Nothing,
                                     Optional ByVal Doc_Numero_Des As String = Nothing,
                                     Optional ByVal Natura_Beni As String = Nothing,
                                     Optional ByVal Tara_Veicolo As Decimal? = Nothing,
                                     Optional ByVal Tara_Imballi As Decimal? = Nothing,
                                     Optional ByVal Tipo_Peso As Integer? = Nothing,
                                     Optional ByVal Modalita As Integer? = Nothing,
                                     Optional ByVal Username_Note As String = Nothing,
                                     Optional ByVal Progr_Protocollo As Integer? = Nothing,
                                     Optional ByVal Progr_Registrazione As Integer? = Nothing,
                                     Optional ByVal Data_Registrazione As DateTime? = Nothing,
                                     Optional ByVal ChkLayOut_Bypass_Fatturato As Integer? = Nothing,
                                     Optional ByVal ChkLayOut_Join_Prodotti As Integer? = Nothing,
                                     Optional ByVal Validita_Inizio As Date? = Nothing,
                                     Optional ByVal Validita_Fine As Date? = Nothing,
                                     Optional ByVal Disciplinare_PubblicoPrivato As Integer? = Nothing,
                                     Optional ByVal Cod_RisUm_Altro As Integer? = Nothing,
                                     Optional ByVal ChkLayOut_Peso As Integer? = Nothing,
                                     Optional ByVal ChkLayOut_Prezzo As Integer? = Nothing,
                                     Optional ByVal ChkFiltro_Varietale As Integer? = Nothing,
                                     Optional ByVal Sezionale_Cod As Integer? = Nothing,
                                     Optional ByVal Causale_Trasporto_Cod As Integer? = Nothing,
                                     Optional ByVal ChkLayOut_Litri As Integer? = Nothing,
                                     Optional ByVal Cod_RisUm_Aggiuntivo As Integer? = Nothing,
                                     Optional ByVal Cod_Indirizzo_Aggiuntivo As Integer? = Nothing,
                                     Optional ByVal ChkLayOut_Riscontrato As Integer? = Nothing,
                                     Optional ByVal xFiltroAggiuntivo As String = "",
                                     Optional ByVal Flag_Usa_Ora_Reale As Boolean = False,
                                     Optional ByVal Data_Modifica As DateTime = AgroDataInizializzata,
                                     Optional ByVal Username_Modifica As String = "",
                                     Optional ByVal Doc_Numero_Visualizzato As String = Nothing,
                                     Optional ByVal Cod_Macchina_Lav As String = Nothing,
                                     Optional ByVal TipoDocumento As Integer? = Nothing,
                                     Optional ByVal OraFine As DateTime? = Nothing
                                     ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Movimenti_W.ModificaPuntuale()"

        '====================================================================================
        'Parametri opzionali :
        '   Tutti i valori non chiave (se impostati a nothing o non passati 
        '   non ne verrà fatto l'aggiornamento e rimarranno i valori precedenti)
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Dim strOra As String = ""
        Dim oraNew As Date
        Dim scadenzaXOra As Date
        Dim scadenzaNew As Date

        Try

            If Not IsNothing(Ora) Then
                'vista che Ora è Nullable, devo fare così sennò il metodo sotto non esiste
                oraNew = Ora
                strOra = oraNew.ToLongTimeString
            End If

            If Not IsNothing(Scadenza) Then
                'vista che Scadenza è Nullable, devo fare così sennò il metodo sotto non esiste
                scadenzaNew = Scadenza
                scadenzaXOra = scadenzaNew.ToShortDateString
            End If

            If Data_Modifica = AgroDataInizializzata Then
                Data_Modifica = Date.Now
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

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE Movimenti ")
            strSql.AppendLine(" SET Data_Modifica = " & Agro_SQL_SaveDateTime(Data_Modifica) & " ")
            strSql.AppendLine("   , Username_Modifica = '" & Agro_SQL_SaveText(Username_Modifica) & "' ")

            If Not IsNothing(Cod_RisUm) Then
                strSql.AppendLine("   , Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & " ")
            End If

            If Not IsNothing(Cau_Mov) Then
                strSql.AppendLine("   , Cau_Mov = '" & Agro_SQL_SaveText(Cau_Mov) & "' ")
            End If

            If Not IsNothing(Mov_Desc) Then
                strSql.AppendLine("   , Mov_Desc = '" & Agro_SQL_SaveText(Mov_Desc) & "' ")
            End If

            If Not IsNothing(Data_Movimento) Then
                strSql.AppendLine("   , Data_Movimento = " & Agro_SQL_SaveDate(Data_Movimento) & " ")
            End If

            If Not IsNothing(Scadenza) Then
                strSql.AppendLine("   , Scadenza = " & Agro_SQL_SaveDate(Scadenza) & " ")
            End If

            If Not IsNothing(Scadenza_Extra) Then
                strSql.AppendLine("   , Scadenza_Extra = " & Agro_SQL_SaveDate(Scadenza_Extra) & " ")
            End If

            If Not IsNothing(Doc_Numero_Sin) Then
                strSql.AppendLine("   , Doc_Numero_Sin = '" & Agro_SQL_SaveText(Doc_Numero_Sin) & "' ")
            End If

            If Not IsNothing(Doc_Numero) Then
                strSql.AppendLine("   , Doc_Numero = " & Agro_SQL_SaveNum(Doc_Numero) & " ")
            End If

            If Not IsNothing(Doc_Numero_Des) Then
                strSql.AppendLine("   , Doc_Numero_Des = '" & Agro_SQL_SaveText(Doc_Numero_Des) & "' ")
            End If

            If Not IsNothing(Doc_Numero_Visualizzato) Then
                strSql.AppendLine("   , Doc_Numero_Visualizzato = '" & Agro_SQL_SaveText(Doc_Numero_Visualizzato) & "' ")
            End If

            If Not IsNothing(Cod_Macchina_Lav) Then
                strSql.AppendLine("   , Cod_Macchina_Lav = '" & Agro_SQL_SaveText(Cod_Macchina_Lav) & "' ")
            End If

            If Not IsNothing(Num_Protocollo) Then
                strSql.AppendLine("   , Num_Protocollo = " & Agro_SQL_SaveNum(Num_Protocollo) & " ")
            End If

            If Not IsNothing(Validita_Inizio) Then
                strSql.AppendLine("   , Validita_Inizio = " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            End If

            If Not IsNothing(Validita_Fine) Then
                strSql.AppendLine("   , Validita_Fine = " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            End If

            If Not IsNothing(Cod_IndirizzoRisUm) Then
                strSql.AppendLine("   , Cod_IndirizzoRisUm = " & Agro_SQL_SaveNum(Cod_IndirizzoRisUm) & " ")
            End If

            If Not IsNothing(Cod_Destinazione) Then
                strSql.AppendLine("   , Cod_Destinazione = " & Agro_SQL_SaveNum(Cod_Destinazione) & " ")
            End If

            If Not IsNothing(Cod_IndirizzoDestinazione) Then
                strSql.AppendLine("   , Cod_IndirizzoDestinazione = " & Agro_SQL_SaveNum(Cod_IndirizzoDestinazione) & " ")
            End If

            If Not IsNothing(Mezzo) Then
                strSql.AppendLine("   , Mezzo = " & Agro_SQL_SaveNum(Mezzo) & " ")
            End If

            If Not IsNothing(Cod_Vettore) Then
                strSql.AppendLine("   , Cod_Vettore = " & Agro_SQL_SaveNum(Cod_Vettore) & " ")
            End If

            If Not IsNothing(Cod_IndirizzoVettore) Then
                strSql.AppendLine("   , Cod_IndirizzoVettore = " & Agro_SQL_SaveNum(Cod_IndirizzoVettore) & " ")
            End If

            If Not IsNothing(Causale_Trasporto) Then
                strSql.AppendLine("   , Causale_Trasporto = '" & Agro_SQL_SaveText(Causale_Trasporto) & "' ")
            End If

            If Not IsNothing(Aspetto) Then
                strSql.AppendLine("   , Aspetto = '" & Agro_SQL_SaveText(Aspetto) & "' ")
            End If

            If Not IsNothing(Peso) Then
                strSql.AppendLine("   , Peso = " & Agro_SQL_SaveNum(Peso) & " ")
            End If

            If Not IsNothing(TipoDocumento) Then
                strSql.AppendLine("   , TipoDocumento = " & Agro_SQL_SaveNum(TipoDocumento) & " ")
            End If

            If Not IsNothing(Ora) Then

                '  Giulia, 15/02/2017 11:19:05: per i movimenti contabili (nati dal LAN) nel campo "Ora" c'è la data di spedizione mentre "Scadenza" è AGRODATAFINE
                '           quindi non posso fare questo replace che invece va fatto nel mondo Online-Campagna
                If Flag_Usa_Ora_Reale = True Then
                    strSql.AppendLine("   , Ora = " & Agro_SQL_SaveDateTime(Ora) & " ")
                ElseIf Not IsNothing(Scadenza) Then
                    strSql.AppendLine("   , Ora = " & Replace(Agro_SQL_SaveDateTime(CDate(scadenzaXOra) & " " & strOra), ".", ":") & "  ")
                End If

            End If

            If Not IsNothing(OraFine) Then
                strSql.AppendLine("   , OraFine = " & Agro_SQL_SaveDateTime(OraFine) & " ")
            End If


            If Not IsNothing(Colli) Then
                strSql.AppendLine("   , Colli = " & Agro_SQL_SaveNum(Colli) & " ")
            End If

            If Not IsNothing(Extra_Str) Then
                strSql.AppendLine("   , Extra_Str = '" & Agro_SQL_SaveText(Extra_Str) & "' ")
            End If

            If Not IsNothing(Extra_Int) Then
                strSql.AppendLine("   , Extra_Int = " & Agro_SQL_SaveNum(Extra_Int) & " ")
            End If

            If Not IsNothing(Extra_Date) Then
                strSql.AppendLine("   , Extra_Date = " & Agro_SQL_SaveDate(Extra_Date) & " ")
            End If

            If Not IsNothing(Tipo_Sconto) Then
                strSql.AppendLine("   , Tipo_Sconto = " & Agro_SQL_SaveNum(Tipo_Sconto) & " ")
            End If

            If Not IsNothing(Natura_Beni) Then
                strSql.AppendLine("   , Natura_Beni = '" & Agro_SQL_SaveText(Natura_Beni) & "' ")
            End If

            If Not IsNothing(Tara_Veicolo) Then
                strSql.AppendLine("   , Tara_Veicolo = " & Agro_SQL_SaveNum(Tara_Veicolo) & " ")
            End If

            If Not IsNothing(Tara_Imballi) Then
                strSql.AppendLine("   , Tara_Imballi = " & Agro_SQL_SaveNum(Tara_Imballi) & " ")
            End If

            If Not IsNothing(Tipo_Peso) Then
                strSql.AppendLine("   , Tipo_Peso = " & Agro_SQL_SaveNum(Tipo_Peso) & " ")
            End If

            If Not IsNothing(Modalita) Then
                strSql.AppendLine("   , Modalita = " & Agro_SQL_SaveNum(Modalita) & " ")
            End If

            If Not IsNothing(Username_Note) Then
                strSql.AppendLine("   , Username_Note = '" & Agro_SQL_SaveText(Username_Note) & "' ")
            End If

            If Not IsNothing(Progr_Protocollo) Then
                strSql.AppendLine("   , Progr_Protocollo = " & Agro_SQL_SaveNum(Progr_Protocollo) & " ")
            End If

            If Not IsNothing(Progr_Registrazione) Then
                strSql.AppendLine("   , Progr_Registrazione = " & Agro_SQL_SaveNum(Progr_Registrazione) & " ")
            End If

            If Not IsNothing(Data_Registrazione) Then
                strSql.AppendLine("   , Data_Registrazione = " & Agro_SQL_SaveDateTime(Data_Registrazione) & " ")
            End If

            If Not IsNothing(ChkLayOut_Bypass_Fatturato) Then
                strSql.AppendLine("   , ChkLayOut_Bypass_Fatturato = " & Agro_SQL_SaveNum(ChkLayOut_Bypass_Fatturato) & " ")
            End If

            If Not IsNothing(ChkLayOut_Join_Prodotti) Then
                strSql.AppendLine("   , ChkLayOut_Join_Prodotti = " & Agro_SQL_SaveNum(ChkLayOut_Join_Prodotti) & " ")
            End If

            If Not IsNothing(Cod_RisUm_Altro) Then
                strSql.AppendLine("   , Cod_RisUm_Altro = " & Agro_SQL_SaveNum(Cod_RisUm_Altro) & " ")
            End If

            If Not IsNothing(ChkLayOut_Peso) Then
                strSql.AppendLine("   , ChkLayOut_Peso = " & Agro_SQL_SaveNum(ChkLayOut_Peso) & " ")
            End If

            If Not IsNothing(ChkLayOut_Prezzo) Then
                strSql.AppendLine("   , ChkLayOut_Prezzo = " & Agro_SQL_SaveNum(ChkLayOut_Prezzo) & " ")
            End If

            If Not IsNothing(ChkFiltro_Varietale) Then
                strSql.AppendLine("   , ChkFiltro_Varietale = " & Agro_SQL_SaveNum(ChkFiltro_Varietale) & " ")
            End If

            If Not IsNothing(Disciplinare_PubblicoPrivato) Then
                strSql.AppendLine("   , Disciplinare_PubblicoPrivato = " & Agro_SQL_SaveNum(Disciplinare_PubblicoPrivato) & " ")
            End If

            If Not IsNothing(Sezionale_Cod) Then
                strSql.AppendLine("   , Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & " ")
            End If

            If Not IsNothing(Causale_Trasporto_Cod) Then
                strSql.AppendLine("   , Causale_Trasporto_Cod = " & Agro_SQL_SaveNum(Causale_Trasporto_Cod) & " ")
            End If

            If Not IsNothing(ChkLayOut_Litri) Then
                strSql.AppendLine("   , ChkLayOut_Litri = " & Agro_SQL_SaveNum(ChkLayOut_Litri) & " ")
            End If

            If Not IsNothing(Cod_RisUm_Aggiuntivo) Then
                strSql.AppendLine("   , Cod_RisUm_Aggiuntivo = " & Agro_SQL_SaveNum(Cod_RisUm_Aggiuntivo) & " ")
            End If

            If Not IsNothing(Cod_Indirizzo_Aggiuntivo) Then
                strSql.AppendLine("   , Cod_Indirizzo_Aggiuntivo = " & Agro_SQL_SaveNum(Cod_Indirizzo_Aggiuntivo) & " ")
            End If

            If Not IsNothing(ChkLayOut_Riscontrato) Then
                strSql.AppendLine("   , ChkLayOut_Riscontrato = " & Agro_SQL_SaveNum(ChkLayOut_Riscontrato) & " ")
            End If

            '----------------------------------------------------------------------

            strSql.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            strSql.AppendLine(" AND Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")

            If Id_Mov <> 0 Then
                strSql.AppendLine(" AND Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & " ")
            End If

            '----------------------------------------------------------------------
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

    Public Function Modifica_x_Trasferimento(ByVal saCod As Integer,
                                             ByVal strIdAgendaIdMov As String,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Movimenti_W.Modifica_x_Trasferimento()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If strIdAgendaIdMov = "" Then
                Throw New Exception("Parametro non corretto nella query (Str_id_agenda_id_mov obbligatorio)")
            End If


            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE movimenti  SET ")
            strSql.AppendLine("    sa_cod    = " & Agro_SQL_SaveNum(saCod) & "  ")


            strSql.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))
            strSql.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            strSql.AppendLine(" WHERE    (" & strIdAgendaIdMov & ")")
            'esclusi record con sa_cod=0 (DDT e Fatture)
            strSql.AppendLine(" AND Sa_Cod <> 0 ")


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

    Public Function Cancella(ByVal piva As String,
                             ByVal saCod As Integer,
                             ByVal idAgenda As Integer,
                             ByVal idMov As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Movimenti_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""
        '   Sa_Cod = 0
        '   Id_Agenda = 0
        '   Id_Mov = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If idAgenda = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Agenda obbligatorio)")
            End If

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.Length = 0
                strSql.AppendLine(" UPDATE Movimenti ")
                strSql.AppendLine(" SET ")
                strSql.AppendLine("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.AppendLine("      ,Inviato = -1 ")
                strSql.AppendLine(" WHERE  Inviato >= 0 ")

            Else
                strSql.Length = 0
                strSql.AppendLine(" DELETE ")
                strSql.AppendLine(" FROM Movimenti ")
                strSql.AppendLine(" WHERE  1=1 ")
            End If

            If piva <> String.Empty Then
                strSql.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            If saCod <> 0 Then
                strSql.AppendLine(" AND Sa_Cod = " & Agro_SQL_SaveNum(saCod) & "   ")
            End If


            strSql.AppendLine(" AND Id_Agenda = " & Agro_SQL_SaveNum(idAgenda) & "   ")


            If idMov <> 0 Then
                strSql.AppendLine(" AND Id_Mov = " & Agro_SQL_SaveNum(idMov) & "   ")
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

    Public Function IPNO_Delete(ByVal piva As String,
                                ByVal idAgenda As Integer,
                                ByVal idMov As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByRef objParametri As AgronicaCoreParametri
                                ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Movimenti_W.IPNO_Delete()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Dim strFiltro As New StringBuilder
        Dim strFiltroRiferimenti As New StringBuilder
        Dim strPreFiltro As New StringBuilder

        Try

            '---------------------------------------------

            '====================================================================================================================
            'Impostazione Filtro
            '--------------------------------------------------------------------------------------------------------------------
            'Nota1: La coerenza dei parametri  fatto dalla routine chiamante (Agro_Contab.Movimenti_W) ed è comunque inutile.
            '       Non potranno essere infatti fatte cancellazioni di record non volute.
            'Nota2: Ll Sa_Cod non Compare pe evitare guai (inoltre è inutile)

            strFiltro.Length = 0

            strFiltro.AppendLine(" Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            strFiltro.AppendLine(" And Id_Agenda = " & Agro_SQL_SaveNum(idAgenda) & " ")
            strFiltro.AppendLine(" And Id_Mov    = " & Agro_SQL_SaveNum(idMov))
            '====================================================================================================================



            '====================================================================================================================
            'Impostazione Filtro Tabella 'Mov_Dettagli_Riferimenti'
            '--------------------------------------------------------------------------------------------------------------------
            strFiltroRiferimenti.Length = 0

            strFiltroRiferimenti.AppendLine(" ((Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            strFiltroRiferimenti.AppendLine(" And Id_Agenda = " & Agro_SQL_SaveNum(idAgenda) & " ")
            strFiltroRiferimenti.AppendLine(" And Id_Mov    = " & Agro_SQL_SaveNum(idMov) & " ) OR ")
            strFiltroRiferimenti.AppendLine(" (Piva_Rif = '" & Agro_SQL_SaveText(piva) & "' ")
            strFiltroRiferimenti.AppendLine(" And Id_Agenda_Rif = " & Agro_SQL_SaveNum(idAgenda) & " ")
            strFiltroRiferimenti.AppendLine(" And Id_Mov_Rif    = " & Agro_SQL_SaveNum(idMov) & " ))")
            '====================================================================================================================



            strSql.Length = 0
            strPreFiltro.Length = 0


            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                'Impostazione del PreFiltro

                'StrPreFiltro.AppendLine(" WHERE Inviato > 0 ")
                strPreFiltro.AppendLine(" WHERE Inviato >= 0 ")

                strSql.AppendLine(" UPDATE Mov_Destinazioni             SET Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ,Inviato = -1 " & strPreFiltro.ToString & " And " & strFiltro.ToString)
                strSql.AppendLine(" UPDATE Mov_Dettaglio_Tecnico        SET Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ,Inviato = -1 " & strPreFiltro.ToString & " And " & strFiltro.ToString)
                strSql.AppendLine(" UPDATE Mov_Dettaglio_Tecnico_Extra  SET Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ,Inviato = -1 " & strPreFiltro.ToString & " And " & strFiltro.ToString)
                strSql.AppendLine(" UPDATE MovimentixReport             SET Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ,Inviato = -1 " & strPreFiltro.ToString & " And " & strFiltro.ToString)
                strSql.AppendLine(" UPDATE Movimenti_Dettagli           SET Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ,Inviato = -1 " & strPreFiltro.ToString & " And " & strFiltro.ToString)
                strSql.AppendLine(" UPDATE Mov_Dettagli_Riferimenti     SET Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ,Inviato = -1 " & strPreFiltro.ToString & " And " & strFiltroRiferimenti.ToString)
                strSql.AppendLine(" UPDATE Movimenti                    SET Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ,Inviato = -1 " & strPreFiltro.ToString & " And " & strFiltro.ToString)

            Else

                'Impostazione del PreFiltro

                'StrPreFiltro.AppendLine(" WHERE Inviato = 0 ")
                strPreFiltro.AppendLine(" WHERE 1=1 ")

                strSql.AppendLine(" DELETE FROM  Mov_Destinazioni            " & strPreFiltro.ToString & " And " & strFiltro.ToString)
                strSql.AppendLine(" DELETE FROM  Mov_Dettaglio_Tecnico       " & strPreFiltro.ToString & " And " & strFiltro.ToString)
                strSql.AppendLine(" DELETE FROM  Mov_Dettaglio_Tecnico_Extra " & strPreFiltro.ToString & " And " & strFiltro.ToString)
                strSql.AppendLine(" DELETE FROM  MovimentixReport            " & strPreFiltro.ToString & " And " & strFiltro.ToString)
                strSql.AppendLine(" DELETE FROM  Movimenti_Dettagli          " & strPreFiltro.ToString & " And " & strFiltro.ToString)
                strSql.AppendLine(" DELETE FROM  Mov_Dettagli_Riferimenti    " & strPreFiltro.ToString & " And " & strFiltroRiferimenti.ToString)
                strSql.AppendLine(" DELETE FROM  Movimenti                   " & strPreFiltro.ToString & " And " & strFiltro.ToString)

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

    Public Sub Scrivi(ByRef Movimenti As AgronicaCoreEntityFramework_POCO.Movimenti,
                      ByRef GiasContext As Gias_DeveloperServer_Entities,
                      ByRef objParametriServer As AgronicaCoreParametri)

        Const nomeRoutine = "ContabDAL.Movimenti_W.Scrivi()"
        Dim messaggioErrore As String = ""

        Try
            Valorizza(Movimenti, objParametriServer)

            Movimenti.Data_Creazione = DateTime.Now
            Movimenti.Data_Modifica = DateTime.Now
            Movimenti.Username_Creazione = objParametriServer.UsernameOperazione
            Movimenti.Username_Modifica = objParametriServer.UsernameOperazione

            GiasContext.Movimenti.Add(Movimenti)

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

    Public Sub Modifica(ByRef Movimenti As AgronicaCoreEntityFramework_POCO.Movimenti,
                        ByRef GiasContext As Gias_DeveloperServer_Entities,
                        ByRef objParametriServer As AgronicaCoreParametri)

        Const nomeRoutine = "ContabDAL.Movimenti_W.Modifica()"
        Dim messaggioErrore As String = ""

        Try

            Valorizza(Movimenti, objParametriServer)

            Movimenti.Data_Modifica = DateTime.Now
            Movimenti.Username_Modifica = objParametriServer.UsernameOperazione

            GiasContext.Entry(Movimenti).State = EntityState.Modified

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

    Public Sub Valorizza(ByRef Movimenti As AgronicaCoreEntityFramework_POCO.Movimenti,
                         ByRef objParametriServer As AgronicaCoreParametri)

        Const nomeRoutine = "ContabDAL.Movimenti_W.Valorizza()"
        Dim messaggioErrore As String = ""

        Try
            If Movimenti.Mov_Desc Is Nothing Then
                Movimenti.Mov_Desc = ""
            End If

            If Movimenti.Data_Movimento Is Nothing Then
                Movimenti.Data_Movimento = DateTime.Now
            End If

            If Movimenti.Scadenza Is Nothing Then
                Movimenti.Scadenza = DateTime.Now
            End If

            If Movimenti.Doc_Numero Is Nothing Then
                Movimenti.Doc_Numero = 0
            End If

            If Movimenti.Num_Protocollo Is Nothing Then
                Movimenti.Num_Protocollo = 0
            End If

            If Movimenti.inviato Is Nothing Then
                Movimenti.inviato = 0
            End If

            If Movimenti.Data_Creazione Is Nothing Then
                Movimenti.Data_Creazione = AGRODATAINIZIO
            End If

            If Movimenti.Username_Creazione Is Nothing Then
                Movimenti.Username_Creazione = objParametriServer.UsernameOperazione
            End If

            If Movimenti.Validita_Inizio Is Nothing Then
                Movimenti.Validita_Inizio = AGRODATAINIZIO
            End If

            If Movimenti.Validita_Fine Is Nothing Then
                Movimenti.Validita_Fine = AGRODATAFINE
            End If

            If Movimenti.Cod_IndirizzoRisUm Is Nothing Then
                Movimenti.Cod_IndirizzoRisUm = 0
            End If

            If Movimenti.Cod_Destinazione Is Nothing Then
                Movimenti.Cod_Destinazione = 0
            End If

            If Movimenti.Cod_IndirizzoDestinazione Is Nothing Then
                Movimenti.Cod_IndirizzoDestinazione = 0
            End If

            If Movimenti.Mezzo Is Nothing Then
                Movimenti.Mezzo = 0
            End If

            If Movimenti.Cod_Vettore Is Nothing Then
                Movimenti.Cod_Vettore = 0
            End If

            If Movimenti.Cod_IndirizzoVettore Is Nothing Then
                Movimenti.Cod_IndirizzoVettore = 0
            End If

            If Movimenti.Causale_Trasporto Is Nothing Then
                Movimenti.Causale_Trasporto = "0"
            End If

            If Movimenti.Aspetto Is Nothing Then
                Movimenti.Aspetto = ""
            End If

            If Movimenti.Peso Is Nothing Then
                Movimenti.Peso = 0
            End If

            If Movimenti.Ora Is Nothing Then
                Movimenti.Ora = AGRODATAINIZIO
            End If

            If Movimenti.Colli Is Nothing Then
                Movimenti.Colli = 0
            End If

            If Movimenti.Extra_Str Is Nothing Then
                Movimenti.Extra_Str = ""
            End If

            If Movimenti.Extra_Int Is Nothing Then
                Movimenti.Extra_Int = 0
            End If

            If Movimenti.Extra_Date Is Nothing Then
                Movimenti.Extra_Date = AGRODATAINIZIO
            End If

            If Movimenti.Tipo_Sconto Is Nothing Then
                Movimenti.Tipo_Sconto = 0
            End If

            If Movimenti.Doc_Numero_Des Is Nothing Then
                Movimenti.Doc_Numero_Des = ""
            End If

            If Movimenti.Natura_Beni Is Nothing Then
                Movimenti.Natura_Beni = ""
            End If

            If Movimenti.Username_Note Is Nothing Then
                Movimenti.Username_Note = ""
            End If

            If IsNothing(Movimenti.Scadenza_Extra) OrElse Movimenti.Scadenza_Extra < AGRODATAINIZIO Then
                Movimenti.Scadenza_Extra = AGRODATAINIZIO
            End If

            If Movimenti.Doc_Numero_Sin Is Nothing Then
                Movimenti.Doc_Numero_Sin = ""
            End If

            If Movimenti.Data_Registrazione < AGRODATAINIZIO Then
                Movimenti.Data_Registrazione = AGRODATAINIZIO
            End If

            If Movimenti.Disciplinare_PubblicoPrivato Is Nothing Then
                Movimenti.Disciplinare_PubblicoPrivato = 0
            End If

            If Movimenti.Sezionale_Cod Is Nothing Then
                Movimenti.Sezionale_Cod = 0
            End If

            If Movimenti.Causale_Trasporto_Cod Is Nothing Then
                Movimenti.Causale_Trasporto_Cod = 0
            End If

            If Movimenti.ChkLayOut_Litri Is Nothing Then
                Movimenti.ChkLayOut_Litri = 0
            End If

            If Movimenti.Cod_RisUm_Aggiuntivo Is Nothing Then
                Movimenti.Cod_RisUm_Aggiuntivo = 0
            End If

            If Movimenti.Cod_Indirizzo_Aggiuntivo Is Nothing Then
                Movimenti.Cod_Indirizzo_Aggiuntivo = 0
            End If

            If Movimenti.ChkLayOut_Riscontrato Is Nothing Then
                Movimenti.ChkLayOut_Riscontrato = 0
            End If

            If Movimenti.Doc_Numero_Visualizzato Is Nothing Then
                Movimenti.Doc_Numero_Visualizzato = ""
            End If

            If Movimenti.Cod_Macchina_Lav Is Nothing Then
                Movimenti.Cod_Macchina_Lav = ""
            End If

            If Movimenti.TipoDocumento Is Nothing Then
                Movimenti.TipoDocumento = 0
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

    Public Sub Elimina(ByRef Movimenti As AgronicaCoreEntityFramework_POCO.Movimenti,
                       ByRef GiasContext As Gias_DeveloperServer_Entities,
                       ByRef objParametriServer As AgronicaCoreParametri)

        Const nomeRoutine = "ContabDAL.Movimenti_W.Elimina()"
        Dim messaggioErrore As String = ""

        Try

            GiasContext.Movimenti.Remove(Movimenti)

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try
    End Sub

End Class
