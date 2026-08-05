Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi


Public Class Centri_Codici_Read
    Inherits AgronicaCoreDataProvider.DataProvider

    '###############################################################################
    Public Function ValCod_from_SaCodIdCod(ByVal Piva As String,
                                           ByVal Sa_Cod As Integer,
                                           ByVal Id_Cod As Integer,
                                           ByRef objParametri As AgronicaCoreParametri
                                           ) As String

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Centri_Codici.ValCod_from_SaCodIdCod()"
        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim valCod As String

        Try

            valCod = ""

            dt = Leggi(Piva, Sa_Cod, Id_Cod,
                       "", "",
                       enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                       "", "", objParametri)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                valCod = dt.Rows(0).Item("Val_Cod")
            End If

            dt = Nothing

        Catch ex As Exception
            valCod = ""
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return valCod

    End Function

    '###############################################################################
    Public Function CodOperatoreBIO_from_PivaSaCod(ByVal Piva As String,
                                           ByVal Sa_Cod As Integer,
                                           ByRef objParametri As AgronicaCoreParametri
                                           ) As String

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Centri_Codici.ValCod_from_SaCodIdCod()"
        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim valCod As String

        Try

            valCod = ""

            Dim filtroAgg = "val_cod <> '0'"

            dt = Leggi(Piva, Sa_Cod, enum_CodiciAnagrafe.CodiceCentro_Attuale,
                       "", "",
                       enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                       filtroAgg, "", objParametri)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                Dim distinctOperatori = dt.AsEnumerable().Select(Function(row) row("Val_Cod")).Distinct()

                valCod = String.Join(", ", distinctOperatori)
            End If

        Catch ex As Exception
            valCod = ""
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return valCod

    End Function


    '#######################################
    Public Function Leggi(ByVal Piva As String,
                          ByVal Sa_Cod As Int32,
                          ByVal Id_Cod As Int32,
                          ByVal Gruppo As String,
                          ByVal Val_Cod As String,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Centri_Codici_Read.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT PIVA, sa_cod, id_cod, val_cod, Validita_Inizio, Validita_Fine ")
                    StrSQL.Append(" FROM  Centri_Aziendali_Codici ")
                    StrSQL.Append(" WHERE Centri_Aziendali_Codici.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Centri_Aziendali_Codici.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then
                        StrSQL.Append(" AND   Centri_Aziendali_Codici.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Centri_Aziendali_Codici.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If

                    If Id_Cod <> 0 Then
                        StrSQL.Append(" AND Centri_Aziendali_Codici.Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")
                    End If

                    If Trim(Val_Cod) <> "" Then
                        StrSQL.Append(" AND Centri_Aziendali_Codici.Val_Cod = '" & Agro_SQL_SaveText(Val_Cod) & "'")
                    End If

                    '---------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Centri_Aziendali_Codici.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Centri_Aziendali_Codici.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        ' StrSQL.Append(" ")
                    End If

                    '======================================================

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT *,  Centri_Aziendali_Codici.Validita_Inizio as xValidita_Inizio, Centri_Aziendali_Codici.Validita_Fine as xValidita_Fine ")
                    StrSQL.Append(", Centri_Aziendali_Codici.Validita_Inizio as Centri_Aziendali_Codici_validita_inizio ")
                    StrSQL.Append(", Centri_Aziendali_Codici.Validita_fine as Centri_Aziendali_Codici_validita_fine ")
                    StrSQL.Append(", Centri_Aziendali_Codici.data_creazione as Centri_Aziendali_Codici_data_creazione ")
                    StrSQL.Append(", Centri_Aziendali_Codici.Data_modifica as Centri_Aziendali_Codici_data_modifica ")
                    StrSQL.Append(", Centri_Aziendali_Codici.username_creazione as Centri_Aziendali_Codici_username_creazione ")
                    StrSQL.Append(", Centri_Aziendali_Codici.username_modifica as Centri_Aziendali_Codici_username_modifica ")

                    StrSQL.Append(" FROM  Centri_Aziendali_Codici, Codici_Anagrafe ")
                    StrSQL.Append(" WHERE Centri_Aziendali_Codici.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Centri_Aziendali_Codici.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   Codici_Anagrafe.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Codici_Anagrafe.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   Centri_Aziendali_Codici.Id_Cod = Codici_Anagrafe.Codice ")
                    StrSQL.Append(" AND   Centri_Aziendali_Codici.Piva = '" & Agro_SQL_SaveText(Piva) & "'")

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Centri_Aziendali_Codici.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If

                    If Id_Cod <> 0 Then
                        StrSQL.Append(" AND Centri_Aziendali_Codici.Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")
                    End If

                    If Gruppo <> "" Then
                        StrSQL.Append(" AND Codici_Anagrafe.Gruppo = '" & Agro_SQL_SaveText(Gruppo) & "'")
                    End If

                    If Trim(Val_Cod) <> "" Then
                        StrSQL.Append(" AND Centri_Aziendali_Codici.Val_Cod = '" & Agro_SQL_SaveText(Val_Cod) & "'")
                    End If

                    '---------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Centri_Aziendali_Codici.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Centri_Aziendali_Codici.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Codici_Anagrafe.Gruppo ASC")
                    End If

                    '======================================================

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT Centri_Aziendali_Codici.*, Sa_Nome, Codici_Anagrafe.descrizione ")
                    StrSQL.Append(" FROM  Centri_Aziendali_Codici ")
                    StrSQL.Append(" INNER JOIN Codici_Anagrafe ON Centri_Aziendali_Codici.Id_Cod = Codici_Anagrafe.Codice ")
                    StrSQL.Append(" INNER JOIN Centri_Aziendali ON Centri_Aziendali.Piva = Centri_Aziendali_Codici.Piva AND Centri_Aziendali.Sa_Cod = Centri_Aziendali_Codici.Sa_Cod ")

                    StrSQL.Append(" WHERE Centri_Aziendali_Codici.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Centri_Aziendali_Codici.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then
                        StrSQL.Append(" AND   Centri_Aziendali_Codici.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Centri_Aziendali_Codici.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If

                    If Id_Cod <> 0 Then
                        StrSQL.Append(" AND Centri_Aziendali_Codici.Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")
                    End If

                    If Trim(Val_Cod) <> "" Then
                        StrSQL.Append(" AND Centri_Aziendali_Codici.Val_Cod = '" & Agro_SQL_SaveText(Val_Cod) & "'")
                    End If

                    '---------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Centri_Aziendali_Codici.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Centri_Aziendali_Codici.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                    '======================================================

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

    '#######################################
    Public Function LeggiOrganismoDiControlloBIO(ByVal Piva As String,
                                                 ByVal Sa_Cod As Int32,
                                                 ByVal Id_Cod As Int32,
                                                 ByVal Val_Cod As String,
                                                 ByVal xFiltroAggiuntivo1 As String,
                                                 ByVal xFiltroAggiuntivo2 As String,
                                                 ByVal xOrderBy As String,
                                                 ByRef objParametri As AgronicaCoreParametri
                                                 ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Centri_Codici_Read.LeggiOrganismoDiControlloBIO()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" (  ")
            StrSQL.AppendLine(" SELECT Centri_Aziendali_Codici.PIVA, sa_cod, id_cod, val_cod, ")
            StrSQL.AppendLine(" ISNULL(Organismo_Sigla, '') AS Organismo_Sigla, ISNULL(Organismo_Des, '') AS Organismo_Des, ISNULL(Codice, '') AS Codice ")
            StrSQL.AppendLine(" FROM  Centri_Aziendali_Codici ")
            StrSQL.AppendLine(" INNER JOIN BIO_Dati_OrganismiControllo ON Centri_Aziendali_Codici.val_cod = BIO_Dati_OrganismiControllo.Organismo_Cod ")
            StrSQL.AppendLine(" WHERE Centri_Aziendali_Codici.Id_Cod = " & Agro_SQL_SaveNum(enum_CodiciAnagrafe.ORGANISMO_DI_CONTROLLO_BIO) & " ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND   Centri_Aziendali_Codici.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Centri_Aziendali_Codici.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Trim(Val_Cod) <> "" Then
                StrSQL.AppendLine(" AND Centri_Aziendali_Codici.Val_Cod = '" & Agro_SQL_SaveText(Val_Cod) & "'")
            End If

            '---------------------------------------------------
            If xFiltroAggiuntivo1 <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo1, , objParametri))
            End If

            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   Centri_Aziendali_Codici.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   Centri_Aziendali_Codici.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            StrSQL.AppendLine(" ) ")
            StrSQL.AppendLine(" UNION ALL ")
            StrSQL.AppendLine(" ( ")

            StrSQL.AppendLine(" SELECT PIVA, sa_cod, id_cod, val_cod,  ")
            StrSQL.AppendLine(" '' AS Organismo_Sigla, Descrizione AS Organismo_Des, '' as Codice ")
            StrSQL.AppendLine(" FROM  Centri_Aziendali_Codici ")
            StrSQL.AppendLine(" INNER JOIN Codici_Anagrafe ON Centri_Aziendali_Codici.id_cod = Codici_Anagrafe.Codice ")
            StrSQL.AppendLine(" WHERE Codici_Anagrafe.gruppo = 'ORG_OLD' ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND   Centri_Aziendali_Codici.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Centri_Aziendali_Codici.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Trim(Val_Cod) <> "" Then
                StrSQL.AppendLine(" AND Centri_Aziendali_Codici.Val_Cod = '" & Agro_SQL_SaveText(Val_Cod) & "'")
            End If

            '---------------------------------------------------
            If xFiltroAggiuntivo2 <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo2, , objParametri))
            End If

            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   Centri_Aziendali_Codici.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   Centri_Aziendali_Codici.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            StrSQL.AppendLine(" ) ")

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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


    '###############################################################################
    Public Function SaCod_from_IdCodValCod(ByVal Piva As String,
                                           ByVal Id_Cod As Integer,
                                           ByVal Val_Cod As String,
                                           ByRef objParametri As AgronicaCoreParametri
                                           ) As Integer

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Centri_Codici.SaCod_from_IdCodValCod()"
        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim saCod As Integer

        Try

            dt = Leggi(Piva, 0, Id_Cod,
                       "", Val_Cod,
                       enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                       "", "", objParametri)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                saCod = dt.Rows(0).Item("Sa_Cod")
            Else
                saCod = 0
            End If

            dt = Nothing

        Catch ex As Exception
            saCod = -1
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return saCod

    End Function


    '###############################################################################
    Public Function SaCodSaNome_from_IdCodValCod(ByRef Sa_Nome As String,
                                                 ByVal Piva As String,
                                                 ByVal Id_Cod As Integer,
                                                 ByVal Val_Cod As String,
                                                 ByRef objParametri As AgronicaCoreParametri
                                                 ) As Integer

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Centri_Codici.SaCodSaNome_from_IdCodValCod()"
        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim saCod As Integer

        Try

            Sa_Nome = ""

            dt = Leggi(Piva, 0, Id_Cod,
                       "", Val_Cod,
                       enumSelezioneVariabile.Selezione_JoinDescrizioni,
                       "", "", objParametri)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                saCod = dt.Rows(0).Item("Sa_Cod")
                Sa_Nome = dt.Rows(0).Item("Sa_Nome")
            Else
                saCod = 0
            End If

            dt = Nothing

        Catch ex As Exception
            saCod = -1
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return saCod

    End Function


    '#######################################
    Public Function Esiste_Centro(ByVal Piva As String,
                                  ByVal Id_Cod As Int32,
                                  ByVal Val_Cod As String,
                                  ByRef Sa_Cod As Int32,
                                  ByRef Sa_Nome As String,
                                  ByRef Data_Modifica As Date,
                                  ByVal xFiltroAggiuntivo As String,
                                  ByRef objParametri As AgronicaCoreParametri
                                  ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Centri_Codici_Read.Esiste_Centro()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT  Centri_Aziendali.*, Centri_Aziendali_Codici.id_cod, Centri_Aziendali_Codici.val_cod ")
            StrSQL.Append(" FROM    Centri_Aziendali, Centri_Aziendali_Codici ")

            StrSQL.Append(" WHERE   Centri_Aziendali.Piva = Centri_Aziendali_Codici.PIVA ")
            StrSQL.Append(" AND     Centri_Aziendali.sa_cod = Centri_Aziendali_Codici.sa_cod ")

            If Piva <> "" Then
                StrSQL.Append(" AND   Centri_Aziendali_Codici.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If Id_Cod <> 0 Then
                StrSQL.Append(" AND Centri_Aziendali_Codici.Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")
            End If

            If Trim(Val_Cod) <> "" Then
                StrSQL.Append(" AND Centri_Aziendali_Codici.Val_Cod = '" & Agro_SQL_SaveText(Val_Cod) & "'")
            End If

            '---------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Centri_Aziendali_Codici.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Centri_Aziendali_Codici.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If dt.Rows.Count = 0 Then

                Sa_Cod = 0
                Sa_Nome = ""
                Data_Modifica = Nothing
                Return False

            ElseIf dt.Rows.Count = 1 Then

                Data_Modifica = CDate(dt.Rows(0).Item("data_modifica"))
                Sa_Nome = CStr(dt.Rows(0).Item("sa_nome"))
                Sa_Cod = dt.Rows(0).Item("sa_cod")
                Return True

            Else
                Throw New Exception("Sono presenti più centri con quel codice, verificare sul db i record su Centri_Aziendali_Codici ")
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Function


    '##############################################################################################
    'viene usata dall'importazione da Agrea
    'l'import da anagrafe ha una sua gestione dei centri aziendali
    'modificata il 23/10/2012: aggiunto sanome byref
    Public Function RecuperaSaCodImpresaByIdAziendaFascicolo(ByVal CodiceChiaveCliente As Integer,
                                                             ByVal Piva As String,
                                                             ByVal ValCod As String,
                                                             ByRef SaNome As String,
                                                             ByRef objParametri_Server As AgronicaCoreParametri
                                                             ) As Integer

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Centri_Codici_Read.RecuperaSaCodImpresaByIdAziendaFascicolo()"
        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim saCod As Integer = 0
        Dim i As Integer

        Try

            objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(Now.Date, Now.Date)

            dt = Leggi(Piva, 0, CodiceChiaveCliente,
                       "", ValCod,
                       enumSelezioneVariabile.Selezione_JoinDescrizioni,
                       "", "", objParametri_Server)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                saCod = dt.Rows(0).Item("Sa_Cod")
                SaNome = dt.Rows(0).Item("Sa_Nome")
                For i = 1 To dt.Rows.Count - 1
                    If dt.Rows(i).Item("Sa_Cod") < saCod Then
                        saCod = dt.Rows(i).Item("Sa_Cod")
                        SaNome = dt.Rows(i).Item("Sa_Nome")
                    End If
                Next
            End If

            If saCod = 0 Then
                'non è stato trovato il sa_cod
                'relativo all'id_azienda dell'archivio di origine
                '(probabilmente è la prima importazione da agrea e ancora in archivio non è salvato)
                'nel caso di agrea, che importa il piano colturale pianificato,
                'non posso salvare 0 nelle entità, altrimenti il ribaltamento da errore
                'il sa_cod deve essere obbligatoriamente salvato!

                'recupero allora il primo sa_cod dell'impresa

                Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read

                dt = objCentri.Leggi(Piva, 0,
                                     enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                     "", " sa_cod ASC",
                                     objParametri_Server)

                If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                    saCod = dt.Rows(0).Item("Sa_Cod")
                    SaNome = dt.Rows(0).Item("Sa_Nome")
                End If

            End If

            If saCod = 0 Then
                'se il sa_cod è ancora 0, l'impresa non è presente in archivio
                'quindi nella pianificazione metterà il sa_cod del centro che verrà creato con l'import
            End If

            dt = Nothing

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        Finally
            objParametri_Server.ResettaFinestra()
        End Try

        Return saCod

    End Function


    '###############################################################################
    Public Sub OrganismodiControllo_from_PivaSaCod(ByVal Piva As String,
                                                   ByVal Sa_Cod As Integer,
                                                   ByRef ODC_sigla As String,
                                                   ByRef ODC_des As String,
                                                   ByRef ODC_codice As String,
                                                   ByRef objParametri As AgronicaCoreParametri)

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Centri_Codici.OrganismodiControllo_from_PivaSaCod()"
        Dim messaggioErrore As String = ""
        Dim dt As DataTable

        ODC_sigla = ""
        ODC_des = ""
        ODC_codice = ""

        'nella scheda colturale bio non c'è sa_cod selezionato
        'If Sa_Cod = 0 Then
        '    Throw New Exception("Parametro non corretto nella query (sa_cod obbligatorio)")
        'End If

        If Piva = "" Then
            Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
        End If

        'If xFiltroAggiuntivo <> "" Then
        '    xFiltroAggiuntivo &= "AND"
        'End If

        'xFiltroAggiuntivo &= ""

        Try

            dt = LeggiOrganismoDiControlloBIO(Piva, Sa_Cod,
                                              0, "",
                                              "", "", "",
                                              objParametri)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                Dim ODC_sigla_lista = dt.AsEnumerable().Select(Function(row) row("Organismo_Sigla")).Distinct()
                Dim ODC_des_lista = dt.AsEnumerable().Select(Function(row) row("Organismo_Des")).Distinct()
                Dim ODC_codice_lista = dt.AsEnumerable().Select(Function(row) row("Codice")).Distinct()

                ODC_sigla = String.Join(", ", ODC_sigla_lista)
                ODC_des = String.Join(", ", ODC_des_lista)
                ODC_codice = String.Join(", ", ODC_codice_lista)
            End If

            dt = Nothing

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

End Class

'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################


Public Class Centri_Codici_Write
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Scrivi(ByVal Piva As String,
                           ByVal Sa_Cod As Integer,
                           ByVal Id_Cod As Long,
                           ByVal Val_Cod As String,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Centri_Codici_Write.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
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
            StrSQL.Append("INSERT INTO Centri_Aziendali_Codici( ")
            StrSQL.Append("                    Piva,      ")
            StrSQL.Append("                    Sa_Cod,    ")
            StrSQL.Append("                    Id_Cod,    ")
            StrSQL.Append("                    Val_Cod,    ")
            StrSQL.Append("                    Inviato, DataInvio, ")
            StrSQL.Append("                    Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                    UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("                    Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("                    ) ")
            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Cod) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Val_Cod) & "' ")
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
    Public Function Modifica(ByVal Piva As String,
                             ByVal Sa_Cod As Integer,
                             ByVal Id_Cod As Long,
                             ByVal Val_Cod As String,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri,
                             Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                             Optional ByVal username_modifica As String = ""
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Centri_Codici_Write.Modifica()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        If Data_modifica = #2/1/1900# Then
            Data_modifica = Now
        End If

        If username_modifica = "" Then
            username_modifica = objParametri.UsernameOperazione
        End If

        Try

            StrSQL.Length = 0
            StrSQL.Append("UPDATE Centri_Aziendali_Codici SET ")
            StrSQL.Append("    Val_Cod           = '" & Agro_SQL_SaveText(Val_Cod) & "'")
            StrSQL.Append("   ,Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Data_modifica))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            StrSQL.Append(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            StrSQL.Append(" AND   Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")

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

    '##############################################################################################
    Public Function Cancella(ByVal Piva As String,
                             ByVal Sa_Cod As Integer,
                             ByVal Id_Cod As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Centri_Codici_Write.Cancella()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Centri_Aziendali_Codici ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                StrSQL.Append(" AND Inviato >= 0")

                If Sa_Cod <> 0 Then
                    StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                End If

                If Id_Cod <> 0 Then
                    StrSQL.Append(" AND Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")
                End If

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     Centri_Aziendali_Codici ")
                StrSQL.Append(" WHERE    Piva= '" & Agro_SQL_SaveText(Piva) & "' ")

                If Sa_Cod <> 0 Then
                    StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                End If

                If Id_Cod <> 0 Then
                    StrSQL.Append(" AND Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")
                End If

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

    '##############################################################################################
    Public Function AggiornaValiditaInizio(ByVal Piva As String,
                                           ByVal Sa_Cod As Integer,
                                           ByVal Id_Cod As Integer,
                                           ByVal Validita_Inizio As Date,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByRef objParametri As AgronicaCoreParametri
                                           ) As Boolean

        Const nomeRoutine = "AnagrafeCoreAnagrafeDAL.Centri_Codici_Write.AggiornaValiditaInizio()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE Centri_Aziendali_Codici SET ")
            StrSQL.Append("   UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND   Validita_Inizio < " & Agro_SQL_SaveDate(Validita_Inizio))

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Id_Cod <> 0 Then
                StrSQL.Append(" AND Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")
            End If
            '---------------------------------------------

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

    '##############################################################################################
    Public Function AggiornaValiditaFine(ByVal PIVA As String,
                                         ByVal Sa_Cod As Integer,
                                         ByVal Id_Cod As Integer,
                                         ByVal Validita_Fine As Date,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As Boolean

        Const nomeRoutine As String = "AnagrafeCoreAnagrafeDAL.Centri_Codici_Write.AggiornaValiditaFine()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE Centri_Aziendali_Codici SET ")
            StrSQL.Append("   UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Fine   =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
            StrSQL.Append(" AND   Validita_Fine > " & Agro_SQL_SaveDate(Validita_Fine))

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Id_Cod <> 0 Then
                StrSQL.Append(" AND Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")
            End If
            '---------------------------------------------

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

    '##############################################################################################
    Public Function CancellaId_Cod101_102_103(ByVal Piva As String,
                                              ByVal Sa_Cod As Long,
                                              ByVal xFiltroAggiuntivo As String,
                                              ByRef objParametri As AgronicaCoreParametri
                                              ) As Boolean

        Const nomeRoutine = "AnagrafeCoreAnagrafeDAL.Centri_Codici_Write.CancellaId_Cod101_102_103()"

        '====================================================================================
        'Parametri opzionali :
        '   Sa_Cod
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0

            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                StrSQL.Append(" UPDATE  Centri_Aziendali_Codici ")
                StrSQL.Append(" SET ")
                StrSQL.Append("          Username_Modifica = '" & objParametri.UsernameOperazione & "' ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "'")
                StrSQL.Append(" AND    Inviato >= 0 ")
            Else
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM  Centri_Aziendali_Codici ")
                StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "'")
                StrSQL.Append(" AND    Inviato = 0 ")
            End If

            ' clausola in comune alle 2 query
            StrSQL.Append(" AND   ( Id_Cod = 101  OR  Id_Cod = 102  OR  Id_Cod = 103 ) ")

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND   Sa_Cod = " & Sa_Cod & " ")
            End If

            '---------------------------------------------

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

    Public Function aggiorna(PIVA As String, sa_cod As Integer,
                             id_cod As enum_CodiciAnagrafe, val_cod As String,
                             datainizio As Date, datafine As Date,
                             objParametriServer As AgronicaCoreParametri)

        If PIVA = "" Then
            Throw New Exception("PIVA parametro obbligatorio")
        End If
        If id_cod = 0 Then
            Throw New Exception("id_cod parametro obbligatorio")
        End If
        If sa_cod = 0 Then
            Throw New Exception("sa_cod parametro obbligatorio")
        End If

        Cancella(PIVA, sa_cod, id_cod, "", objParametriServer)
        
        Return Scrivi(PIVA, sa_cod, id_cod, val_cod, datainizio, datafine, objParametriServer)

    End Function

End Class
