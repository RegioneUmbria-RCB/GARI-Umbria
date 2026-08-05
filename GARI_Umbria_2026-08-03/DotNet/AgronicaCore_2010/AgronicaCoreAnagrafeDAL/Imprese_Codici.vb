Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDTOStd.Identity
Imports AgronicaCoreEntityFramework

Public Class Imprese_Codici_Read
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function LeggiJoinCompleto(ByVal Piva As String,
                                      ByVal Id_Cod As Integer,
                                      ByVal xFiltroAggiuntivo As String,
                                      ByVal xOrderBy As String,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                      ) As DataTable

        Dim nomeRoutine = "AgronicaCoreAnagrafeDAL.Imprese_Codici_Read.LeggiJoinCompleto()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT Imprese.*, ")
            strSql.AppendLine(" Imprese_Codici.id_cod, Imprese_Codici.val_cod, Imprese_Codici.Validita_Inizio AS Codici_Validita_Inizio, Imprese_Codici.Validita_Fine AS Codici_Validita_Fine,  ")
            strSql.AppendLine(" Codici_Anagrafe.descrizione, Codici_Anagrafe.gruppo, ")
            strSql.AppendLine(" ImpresexIndirizzi.Tipo_Indirizzo, Indirizzi.Cod_Indirizzo, Indirizzi.ind_des, Indirizzi.frz_des, Indirizzi.CAP, Indirizzi.com_cod_istat, Indirizzi.pro_cod_istat, Indirizzi.stato, Indirizzi.note, ")
            strSql.AppendLine(" ISNULL(ISTAT.LOCALITA, '') AS com_des, ISNULL(ISTAT.COMUNI_PROV, '') AS pro_cod  ")

            strSql.AppendLine(" FROM    Imprese ")
            strSql.AppendLine(" INNER JOIN  Imprese_Codici ON Imprese_Codici.piva = Imprese.piva ")
            strSql.AppendLine(" INNER JOIN  Codici_Anagrafe ON Imprese_Codici.id_cod = Codici_Anagrafe.codice ")

            strSql.AppendLine("  INNER JOIN  ImpresexIndirizzi ON Imprese.Piva = ImpresexIndirizzi.Piva ")
            strSql.AppendLine("  INNER JOIN Indirizzi ON ImpresexIndirizzi.Cod_Indirizzo = Indirizzi.cod_indirizzo ")
            strSql.AppendLine("  INNER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM ")

            strSql.AppendLine(" WHERE   Imprese_Codici.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
            strSql.AppendLine(" AND     Imprese_Codici.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")

            If Piva <> "" Then
                strSql.AppendLine(" AND Imprese.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Id_Cod <> 0 Then
                strSql.AppendLine(" AND     Imprese_Codici.id_cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Imprese_Codici.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Imprese_Codici.Inviato =-1 ")
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

    Public Function Esiste_Impresa(ByVal Piva As String,
                                   ByVal ID_Cod As Integer,
                                   ByRef Data_Modifica As Date,
                                   ByRef Socio As String,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                   ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Imprese_Codici_Read.Esiste_Impresa()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Dim Impresa_Presente As Boolean = False

        Try

            strSql.Length = 0
            strSql.AppendLine("  SELECT  Imprese.*, Imprese_Codici.id_cod, Imprese_Codici.val_cod   ")

            strSql.AppendLine(" FROM    Imprese_Codici inner JOIN  Imprese ON Imprese_Codici.PIVA = Imprese.PIVA ")

            If Piva <> "" Then
                strSql.AppendLine(" AND     (Imprese_Codici.PIVA = '" & Agro_SQL_SaveText(Piva).Trim & "')  ")
            End If

            If ID_Cod <> 0 Then
                strSql.AppendLine(" AND     (Imprese_Codici.id_cod = " & Agro_SQL_SaveNum(ID_Cod) & ") ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   (Imprese_Codici.inviato >= 0) ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   (Imprese_Codici.inviato = -1) ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If dt.Rows.Count = 1 Then

                Impresa_Presente = True
                Data_Modifica = CDate(dt.Rows(0).Item("data_modifica"))

                Dim drs As DataRow()
                drs = dt.Select("ID_Cod = " & Agro_SQL_SaveNum(ID_Cod, False))
                If drs.Length > 0 Then
                    Socio = drs(0).Item("val_cod")
                Else
                    Socio = ""
                End If

            ElseIf dt.Rows.Count = 0 Then

                Impresa_Presente = False
                Data_Modifica = Nothing
                Socio = ""

            Else
                Throw New Exception("Sono presenti più imprese con quel codice, verificare sul db i record su Imprese_Codici ")
            End If

            Return Impresa_Presente

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return Impresa_Presente

    End Function

    Public Function Distinct_Val_cod(ByVal ID_Cod As Integer,
                                     ByVal xOrderBy As String,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                     ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Codici_Read.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" select distinct val_cod , descrizione_stabilimento from imprese_codici  ")
            strSql.AppendLine(" inner join (select distinct Cod_stabilimento , descrizione_stabilimento from [PDC_Stabilimenti_Apofruit]) as [PDC_Stabilimenti_Apofruit] on Cod_stabilimento = val_cod ")

            strSql.AppendLine(" WHERE   (Imprese_Codici.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ")  ")
            strSql.AppendLine(" AND     (Imprese_Codici.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ")  ")

            strSql.AppendLine(" AND     (Imprese_Codici.id_cod = " & Agro_SQL_SaveNum(ID_Cod) & ") ")

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Imprese_Codici.val_cod ")
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

    '###############################################################################################
    Public Sub LeggiOpzioni_InizioGestioneContabile(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                    ByVal Piva As String,
                                                    ByRef Flag_ConsideraSaldiIniziali As Boolean,
                                                    ByRef DataInizioGestCont As Date,
                                                    ByRef StrErr As String)

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Imprese_Codici_Read.LeggiOpzioni_InizioGestioneContabile()"

        Flag_ConsideraSaldiIniziali = True
        DataInizioGestCont = AGRODATAINIZIO

        Dim dt As DataTable
        Dim filtro As String
        Dim i As Integer

        filtro = " ( Id_Cod IN ( " &
                        CStr(enum_CodiciAnagrafe.GestioneContabile_ConsideraSaldiIniziali) & ", " &
                        CStr(enum_CodiciAnagrafe.GestioneContabile_DataInizio) & " " &
                        " ) )"

        dt = Leggi(Piva,
                   0,
                   enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                   filtro,
                   "", objParametri)

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            For i = 0 To dt.Rows.Count - 1

                Select Case dt.Rows(i).Item("Id_Cod")

                    Case enum_CodiciAnagrafe.GestioneContabile_ConsideraSaldiIniziali
                        If dt.Rows(i).Item("val_cod") = 0 Then
                            Flag_ConsideraSaldiIniziali = False
                        End If

                    Case enum_CodiciAnagrafe.GestioneContabile_DataInizio
                        If IsDate(dt.Rows(i).Item("val_cod")) Then
                            DataInizioGestCont = CDate(dt.Rows(i).Item("val_cod"))
                        End If

                End Select

            Next
        Else
            StrErr = "Non è stata impostata la data di inizio gestione contabile, per il corretto funzionamento della contabilità occorre impostarla (in modifica dell'impresa, gestione dei codici)."
        End If

    End Sub

    Public Function Leggi(ByVal Piva As String,
                          ByVal ID_Cod As Integer,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Imprese_Codici_Read.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   ID_Cod = 0                  =>  si leggono tutti i codici
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    strSql.Length = 0
                    strSql.AppendLine(" SELECT Piva, Imprese_Codici.id_cod, Imprese_Codici.val_cod,  ")
                    strSql.AppendLine("         Imprese_Codici.Validita_Inizio, Imprese_Codici.Validita_Fine ")

                    strSql.AppendLine(" FROM    Imprese_Codici WITH(NOLOCK)")

                    strSql.AppendLine(" WHERE   (Imprese_Codici.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ")  ")
                    strSql.AppendLine(" AND     (Imprese_Codici.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ")  ")

                    If Piva <> "" Then
                        strSql.AppendLine(" AND     (Imprese_Codici.PIVA = '" & Agro_SQL_SaveText(Piva).Trim & "')  ")
                    End If

                    If ID_Cod <> 0 Then
                        strSql.AppendLine(" AND     (Imprese_Codici.id_cod = " & Agro_SQL_SaveNum(ID_Cod) & ") ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   (Imprese_Codici.inviato >= 0) ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   (Imprese_Codici.inviato = -1) ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Imprese_Codici.Validita_Inizio ASC")
                    End If


                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    strSql.Length = 0
                    strSql.AppendLine(" SELECT * ,  ")
                    strSql.AppendLine("   Imprese_Codici.Validita_Inizio as xValidita_Inizio, Imprese_Codici.Validita_Fine as xValidita_Fine, ")
                    strSql.AppendLine("   Imprese_Codici.data_creazione as Imprese_Codici_Data_creazione, Imprese_Codici.data_modifica as Imprese_Codici_data_modifica, ")
                    strSql.AppendLine("   Imprese_Codici.username_creazione as Imprese_Codici_username_creazione, Imprese_Codici.username_modifica as Imprese_Codici_username_modifica, ")
                    strSql.AppendLine("   Imprese_Codici.validita_inizio as Imprese_Codici_validita_inizio, Imprese_Codici.validita_fine as Imprese_Codici_validita_fine, ")
                    strSql.AppendLine("   Imprese_Codici.Validazione as Imprese_Codici_Validazione, Imprese_Codici.Data_Validazione as Imprese_Codici_Data_Validazione, Imprese_Codici.UserName_Validazione as Imprese_Codici_UserName_Validazione ")
                    strSql.AppendLine(" FROM Imprese_Codici WITH(NOLOCK)  ")
                    strSql.AppendLine(" INNER JOIN Codici_Anagrafe WITH(NOLOCK) ")
                    strSql.AppendLine(" ON Imprese_Codici.id_cod = Codici_Anagrafe.codice ")

                    strSql.AppendLine(" WHERE   (Imprese_Codici.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ")  ")
                    strSql.AppendLine(" And     (Imprese_Codici.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ")  ")

                    strSql.AppendLine(" And     (Imprese_Codici.PIVA = '" & Agro_SQL_SaveText(Piva).Trim & "')  ")

                    If ID_Cod <> 0 Then
                        strSql.AppendLine(" AND     (Imprese_Codici.id_cod = " & Agro_SQL_SaveNum(ID_Cod) & ") ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   (Imprese_Codici.inviato >= 0) ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   (Imprese_Codici.inviato = -1) ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Imprese_Codici.Validita_Inizio ASC")
                    End If



                Case enumSelezioneVariabile.Selezione_JoinDescrizioni
                    '
                    '
                    '
                    '


                Case enumSelezioneVariabile.Selezione_JoinCompleta
                    strSql.Length = 0
                    strSql.AppendLine(" SELECT Imprese_Codici.PIVA, Imprese_Codici.id_cod, Imprese_Codici.val_cod, Imprese_Codici.inviato, Imprese_Codici.datainvio, Imprese_Codici.Data_Creazione, Imprese_Codici.Data_Modifica, ")
                    strSql.AppendLine(" Imprese_Codici.Username_Creazione, Imprese_Codici.Username_Modifica, Imprese_Codici.Validita_Inizio, Imprese_Codici.Validita_Fine, Imprese_Codici.Validazione, ")
                    strSql.AppendLine(" Imprese_Codici.Data_Validazione, Imprese_Codici.UserName_Validazione, ")
                    strSql.AppendLine(" Codici_Anagrafe.codice, Codici_Anagrafe.descrizione, Codici_Anagrafe.lunghezza, ISNULL(Codici_Anagrafe.picture, '') AS picture, ISNULL(Codici_Anagrafe.tipo, '') AS tipo, ISNULL(Codici_Anagrafe.gruppo, '') AS gruppo, Codici_Anagrafe.genitore, Codici_Anagrafe.creatore, Codici_Anagrafe.inviato, ")
                    strSql.AppendLine(" Codici_Anagrafe.datainvio, Codici_Anagrafe.Data_Creazione, Codici_Anagrafe.Data_Modifica, Codici_Anagrafe.Username_Creazione, Codici_Anagrafe.Username_Modifica, ")
                    strSql.AppendLine(" Codici_Anagrafe.Validita_Inizio, Codici_Anagrafe.Validita_Fine ")
                    strSql.AppendLine(" FROM Imprese_Codici WITH(NOLOCK) ")
                    strSql.AppendLine(" INNER JOIN Codici_Anagrafe WITH(NOLOCK)")
                    strSql.AppendLine(" ON Imprese_Codici.id_cod = Codici_Anagrafe.codice ")

                    strSql.AppendLine(" WHERE   (Imprese_Codici.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ")  ")
                    strSql.AppendLine(" And     (Imprese_Codici.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ")  ")

                    strSql.AppendLine(" And     (Imprese_Codici.PIVA = '" & Agro_SQL_SaveText(Piva).Trim & "')  ")

                    If ID_Cod <> 0 Then
                        strSql.AppendLine(" AND     (Imprese_Codici.id_cod = " & Agro_SQL_SaveNum(ID_Cod) & ") ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   (Imprese_Codici.inviato >= 0) ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   (Imprese_Codici.inviato = -1) ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Imprese_Codici.Validita_Inizio ASC")
                    End If

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

    '###############################################################
    Public Function Piva_from_CodiceSocio(ByVal CodiceSocio As String,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                          ) As String

        Dim dt As DataTable
        dt = Leggi("",
                   CA_COD_SOCIO,
                   enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                   " Imprese_Codici.val_cod = '" + Agro_SQL_SaveText(CodiceSocio) + "'",
                   "",
                   objParametri)

        If Not IsNothing(dt) Then
            If dt.Rows.Count > 0 Then
                Return dt.Rows(0).Item("Piva")
            Else
                Return ""
            End If
        Else
            Return ""
        End If

    End Function

    '###############################################################
    Public Function Piva_from_CUAA(ByVal CUAA As String,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                   ) As String

        Dim dt As DataTable
        dt = Leggi("",
                   CA_CUAA,
                   enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                   " Imprese_Codici.val_cod = '" + Agro_SQL_SaveText(CUAA) + "'",
                   "",
                   objParametri)

        If Not IsNothing(dt) Then
            If dt.Rows.Count > 0 Then
                Return dt.Rows(0).Item("Piva")
            Else
                Return ""
            End If
        Else
            Return ""
        End If

    End Function

     Public Function Piva_from_CUAA(CUAA As Ienumerable(of String), objParametri As AgronicaCoreParametri) As DataTable
        Dim xFiltroAggiuntivo = ""
        If CUAA IsNot Nothing AndAlso CUAA.Any() Then
            xFiltroAggiuntivo = " Imprese_Codici.val_cod IN (" & String.Join(",", CUAA.Select(Function(c) "'" & Agro_SQL_SaveText(c) & "'")) & ")"
        End If
        Return Leggi(
            Piva:="",
            CA_CUAA,
            enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
            xFiltroAggiuntivo,
            xOrderBy:="",
            objParametri
        )
    End Function

    Public Function EF_Piva_from_CUAA(ByVal CUAA As String, ByRef dal As Gias_DeveloperServer_Entities) As String

        Dim dummy As String = (From x In dal.Imprese_Codici
                               Where x.id_cod = CA_CUAA AndAlso x.val_cod = CUAA
                               Select x.PIVA).FirstOrDefault()

        If Not IsNothing(dummy) Then
            Return dummy
        Else
            Return ""
        End If

    End Function

    Public Function EF_CUAA_from_PIVA(ByVal PIVA As String, ByRef dal As Gias_DeveloperServer_Entities) As String

        Dim dummy As String = (From x In dal.Imprese_Codici
                               Where x.id_cod = CA_CUAA AndAlso x.PIVA = PIVA
                               Select x.val_cod).FirstOrDefault()

        If Not IsNothing(dummy) Then
            Return dummy
        Else
            Return ""
        End If

    End Function

    Public Function Leggi_Imprese_From_CUAA(ByVal CUAAList As List(Of String),
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Leggi_Imprese_From_CUAA.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, objParametri)

            EseguiQuery_Scrittura(objParametri, CreaTabellaTemp_FiltroCUAA(), nomeRoutine)

            If Not IsNothing(CUAAList) AndAlso CUAAList.Any Then
                Dim chunks = ChunkBy(Of String)(CUAAList, 1000)
                For Each chunk In chunks
                    strSql.AppendLine("INSERT INTO #TempCUAA (CUAA) VALUES ")
                    For Each p As String In chunk
                        strSql.AppendLine(String.Format("('{0}'),", p))
                    Next
                    Dim strSqlInsert As String = strSql.ToString
                    strSqlInsert = strSqlInsert.Remove(strSqlInsert.LastIndexOf(","))
                    strSql.Clear()
                    EseguiQuery_Scrittura(objParametri, strSqlInsert, nomeRoutine)
                Next
            End If

            strSql.Length = 0
            strSql.AppendLine(" SELECT Imprese.Piva, Imprese_Codici.val_cod AS CUAA, Imprese.Rag_Soc ")
            strSql.AppendLine(" FROM Imprese_Codici ")
            If CUAAList IsNot Nothing AndAlso CUAAList.Count > 0 Then
                strSql.AppendLine("	INNER JOIN #TempCUAA temp (NOLOCK) ON Imprese_Codici.Val_Cod COLLATE SQL_Latin1_General_CP850_CI_AS = temp.CUAA ")
            End If
            strSql.AppendLine(" LEFT JOIN Imprese ON Imprese.Piva = Imprese_Codici.Piva ")

            strSql.AppendLine(" WHERE (Imprese_Codici.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ")  ")
            strSql.AppendLine(" AND (Imprese_Codici.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ")  ")

            strSql.AppendLine(" AND (Imprese_Codici.id_cod = " & Agro_SQL_SaveNum(CA_CUAA) & ") ")

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" And   (Imprese_Codici.inviato >= 0) ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" And   (Imprese_Codici.inviato = -1) ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            ' Eliminazione tabella temporanea
            EseguiQuery_Scrittura(objParametri, EliminaTabellaTemp_FiltroCUAA, nomeRoutine)

            'commit transazione
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(1, objParametri)

        Catch ex As Exception
            ' Rollback
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        Finally

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri)

        End Try

        Return dt

    End Function


    '################################################################################
    Public Function CUAA_From_Piva(ByVal Piva As String,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Leggi_Codici_read.CUAA_From_Piva()"

        Dim StrSQL As String = ""
        Dim DT As New DataTable
        Dim strRet As String = String.Empty

        StrSQL = StrSQL & " SELECT Imprese.*, "
        StrSQL = StrSQL & " ISNULL "
        StrSQL = StrSQL & "         ((SELECT    val_cod  "
        StrSQL = StrSQL & "         FROM        Imprese_Codici "
        StrSQL = StrSQL & "         WHERE       Imprese.PIVA = Imprese_Codici.PIVA AND id_cod = '1010'), ' ') AS CUAA "
        StrSQL = StrSQL & " FROM    Imprese "
        StrSQL = StrSQL & " WHERE   Piva = '" & Agro_SQL_SaveText(Piva) & "' "

        'Recupero il datatable
        '--------------------------------------------------------------------------
        DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
        '--------------------------------------------------------------------------

        If DT.Rows.Count <> 0 Then
            strRet = DT.Rows(0).Item("CUAA")
        End If

        Return strRet

    End Function


    '################################################################################
    Public Function Indirizzo_From_Piva(ByVal Piva As String,
                                        ByRef ind_des As String,
                                        ByRef frz_des As String,
                                        ByRef CAP As String,
                                        ByRef com_des As String,
                                        ByRef pro_cod As String,
                                        ByRef pro_cod_istat As String,
                                        ByRef com_cod_istat As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Leggi_Codici_read.CUAA_From_Piva()"
        Dim StrSQL As String
        Dim DT As New DataTable
        Dim strRet As String = String.Empty


        'Preparo la query
        StrSQL = " SELECT DISTINCT Imprese.rag_soc, Indirizzi.ind_des, Indirizzi.frz_des, ISNULL(ISTAT.LOCALITA, '') AS com_des, ISNULL(ISTAT.COMUNI_PROV, '') AS pro_cod, "
        StrSQL += " Indirizzi.pro_cod_istat, Indirizzi.com_cod_istat, ISNULL(ISTAT.CAP, '') AS CAP "
        StrSQL += " FROM Indirizzi INNER JOIN "
        StrSQL += " ImpresexIndirizzi ON Indirizzi.cod_indirizzo = ImpresexIndirizzi.cod_indirizzo RIGHT OUTER JOIN "
        StrSQL += " Imprese ON ImpresexIndirizzi.PIVA = Imprese.PIVA "
        StrSQL += " LEFT OUTER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM "
        StrSQL += " WHERE Imprese.PIVA = '" & Agro_SQL_SaveText(Piva) & "'"


        'Recupero il datatable
        '--------------------------------------------------------------------------
        DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
        '--------------------------------------------------------------------------

        If DT.Rows.Count <> 0 Then
            ind_des = DT.Rows(0).Item("ind_des")
            frz_des = DT.Rows(0).Item("frz_des")
            CAP = DT.Rows(0).Item("CAP")
            com_des = DT.Rows(0).Item("com_des")
            pro_cod = DT.Rows(0).Item("pro_cod")
            pro_cod_istat = DT.Rows(0).Item("pro_cod_istat")
            com_cod_istat = DT.Rows(0).Item("com_cod_istat")
        End If

        strRet = ind_des & vbCrLf & frz_des & IIf(frz_des <> String.Empty, " ", "") & CAP & IIf(com_des <> String.Empty, "-", "") & com_des & IIf(pro_cod <> String.Empty, " (" & pro_cod & ")", "")
        Return strRet

    End Function

    Private Function CreaTabellaTemp_FiltroCUAA() As String
        Dim stb As New StringBuilder

        stb.AppendLine(" IF OBJECT_ID('tempdb.dbo.#TempCUAA') IS NULL BEGIN ")
        stb.AppendLine("    CREATE TABLE #TempCUAA ( ")
        stb.AppendLine("        CUAA varchar(255) NULL")
        stb.AppendLine("    )")
        stb.AppendLine(" END ")

        Return stb.ToString()
    End Function
    Private Function EliminaTabellaTemp_FiltroCUAA() As String
        Dim stb As New StringBuilder

        stb.AppendLine(" IF NOT OBJECT_ID('tempdb.dbo.#TempCUAA') IS NULL BEGIN ")
        stb.AppendLine("    DROP TABLE #TempCUAA ")
        stb.AppendLine(" END ")

        Return stb.ToString()
    End Function

    '###############################################################
    Public Function Piva_from_Codice_Like(ByVal CodiceSocio As String,
                                          ByVal enum_CodiciAnagrafe As enum_CodiciAnagrafe,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                          ) As String

        Dim dt As DataTable
        dt = Leggi("",
                   enum_CodiciAnagrafe,
                   enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                   " Imprese_Codici.val_cod LIKE '%" + Agro_SQL_SaveText(CodiceSocio) + "%'",
                   "",
                   objParametri)

        If Not IsNothing(dt) Then
            If dt.Rows.Count > 1 Then
                Throw New Exception("più righe selezionate")
            End If
            If dt.Rows.Count = 1 Then
                Return dt.Rows(0).Item("Piva")
            Else
                Return ""
            End If
        Else
            Return ""
        End If

    End Function

    '###############################################################
    Public Function Piva_from_IdCodValCod(ByVal Id_Cod As Integer,
                                          ByVal Val_Cod As String,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                          ) As String

        Dim dt As DataTable
        dt = Leggi("",
                   Id_Cod,
                   enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                   " Imprese_Codici.val_cod = '" + Agro_SQL_SaveText(Val_Cod) + "'",
                   "",
                   objParametri)

        If Not IsNothing(dt) Then
            If dt.Rows.Count > 0 Then
                Return dt.Rows(0).Item("Piva")
            Else
                Return ""
            End If
        Else
            Return ""
        End If

    End Function

    Public Function LeggixCodice(ByVal Piva As String,
                                 ByVal ID_Cod As Integer,
                                 ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                 ByVal xFiltroAggiuntivo As String,
                                 ByVal xOrderBy As String,
                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                 ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Imprese_Codici_Read.LeggixCodice()"

        '====================================================================================
        'Parametri opzionali :
        '   ID_Cod = 0                  =>  si leggono tutti i codici
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    strSql.Length = 0
                    strSql.AppendLine(" SELECT ")
                    strSql.AppendLine("         Imprese_Codici.id_cod,  ")
                    strSql.AppendLine("         Imprese_Codici.val_cod,  ")
                    strSql.AppendLine("         Imprese_Codici.Validita_Inizio, Imprese_Codici.Validita_Fine ")

                    strSql.AppendLine(" FROM    Imprese_Codici ")

                    strSql.AppendLine(" WHERE   (Imprese_Codici.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ")  ")
                    strSql.AppendLine(" AND     (Imprese_Codici.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ")  ")

                    strSql.AppendLine(" AND     (Imprese_Codici.PIVA = '" & Agro_SQL_SaveText(Piva).Trim & "')  ")

                    If ID_Cod <> 0 Then
                        strSql.AppendLine(" AND     (Imprese_Codici.id_cod = " & Agro_SQL_SaveNum(ID_Cod) & ") ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   (Imprese_Codici.inviato >= 0)")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   (Imprese_Codici.inviato = -1) ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Imprese_Codici.Validita_Inizio ASC")
                    End If


                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    strSql.Length = 0
                    strSql.AppendLine(" SELECT ")
                    strSql.AppendLine("         Imprese_Codici.id_cod,  ")
                    strSql.AppendLine("         Imprese_Codici.val_cod,  ")
                    strSql.AppendLine("         Imprese_Codici.Validita_Inizio, Imprese_Codici.Validita_Fine ")

                    strSql.AppendLine(" FROM    Imprese_Codici ")

                    strSql.AppendLine(" WHERE   (Imprese_Codici.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ")  ")
                    strSql.AppendLine(" AND     (Imprese_Codici.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ")  ")

                    strSql.AppendLine(" AND     (Imprese_Codici.PIVA = '" & Agro_SQL_SaveText(Piva).Trim & "')  ")

                    If ID_Cod <> 0 Then
                        strSql.AppendLine(" AND     (Imprese_Codici.id_cod = " & Agro_SQL_SaveNum(ID_Cod) & ") ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   (Imprese_Codici.inviato >= 0)")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   (Imprese_Codici.inviato = -1) ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Imprese_Codici.Validita_Inizio ASC")
                    End If



                Case enumSelezioneVariabile.Selezione_JoinDescrizioni
                    '
                    '
                    '
                    '


                Case enumSelezioneVariabile.Selezione_JoinCompleta
                    '
                    '
                    '
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

    '################################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="IdCod"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[magnani]	28/04/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function Leggi_Codice_from_Imprese_Codici(ByVal Piva As String,
                                                     ByVal IdCod As Integer,
                                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                     ) As String

        Dim dt As DataTable
        dt = Leggi(CStr(Piva), CInt(IdCod), enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri)

        If Not IsNothing(dt) AndAlso dt.Rows.Count <> 0 Then
            Return dt.Rows(0).Item("val_cod")
        Else
            Return ""
        End If

    End Function

    Public Function Leggi_Codice_Socio(ByVal Piva As String,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                       ) As String

        Return Leggi_Codice_from_Imprese_Codici(CStr(Piva), enum_CodiciAnagrafe.Codice_Socio, objParametri)

    End Function

    Public Function Leggi_CUAA(ByVal Piva As String,
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                               ) As String

        'Se la piva non è valorizzata, non posso andare a acercare il cuaa
        If Piva = "" Then
            Return ""
        End If

        Return Leggi_Codice_from_Imprese_Codici(CStr(Piva), enum_CodiciAnagrafe.CodiceCUAA, objParametri)

    End Function

    Public Sub Leggi_CUAA_GGN_Produttore_Socio(ByVal Piva As String,
                                               ByRef CUAA As String,
                                               ByRef GGN As String,
                                               ByRef Produttore As String,
                                               ByRef Socio As String,
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim dt As DataTable
        Dim filtro As String = " Imprese_Codici.id_cod IN (" & CStr(enum_CodiciAnagrafe.CodiceCUAA) & ", " & CStr(enum_CodiciAnagrafe.Codice_GlobalGap) & ", " & CStr(enum_CodiciAnagrafe.CodiceProduttore) & ", " & CStr(enum_CodiciAnagrafe.Codice_Socio) & " ) "

        CUAA = ""
        GGN = ""
        Produttore = ""
        Socio = ""

        dt = Leggi(CStr(Piva), 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, filtro, "", objParametri)

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            For i = 0 To dt.Rows.Count - 1
                Select Case dt.Rows(i).Item("id_cod")
                    Case enum_CodiciAnagrafe.CodiceCUAA
                        CUAA = dt.Rows(i).Item("val_cod")
                    Case enum_CodiciAnagrafe.Codice_GlobalGap
                        GGN = dt.Rows(i).Item("val_cod")
                    Case enum_CodiciAnagrafe.CodiceProduttore
                        Produttore = dt.Rows(i).Item("val_cod")
                    Case enum_CodiciAnagrafe.Codice_Socio
                        Socio = dt.Rows(i).Item("val_cod")
                End Select
            Next
        End If

    End Sub

    Public Function CUAA_GGN_Produttore_Socio_from_Piva_Massivo(ByVal PivaList As List(Of String),
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                           ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Codici_Read.CUAA_from_Piva_Massivo()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            ConnessioniTransazioni.ApriConnessione(True, objParametri)

            TempChiaviMassivo.CreaTabellaTemp_FiltroPiva(PivaList, NomeRoutine, objParametri)

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SELECT Imprese.PIVA, Codici.val_cod, Codici.id_cod ")
            StrSQL.Append(" FROM Imprese ")
            If PivaList IsNot Nothing AndAlso PivaList.Count > 0 Then
                StrSQL.AppendLine("	INNER JOIN #TempPiva temp (NOLOCK) ON Imprese.Piva = temp.Piva COLLATE DATABASE_DEFAULT ")
            End If
            StrSQL.AppendLine("	INNER JOIN Imprese_Codici Codici (NOLOCK) ON Imprese.Piva = Codici.Piva ")
            StrSQL.AppendLine("	AND Codici.id_cod IN (" & CStr(enum_CodiciAnagrafe.CodiceCUAA) & ", " & CStr(enum_CodiciAnagrafe.Codice_GlobalGap) & ", " & CStr(enum_CodiciAnagrafe.CodiceProduttore) & ", " & CStr(enum_CodiciAnagrafe.Codice_Socio) & ") ")

            StrSQL.Append(" WHERE 1 = 1 ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            ' Eliminazione tabella temporanea
            TempChiaviMassivo.EliminaTabellaTemp_FiltroPiva(NomeRoutine, objParametri)

            'commit transazione
            ConnessioniTransazioni.ChiudiTransazione(1, objParametri)

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            'rollback transazione
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        Finally
            ConnessioniTransazioni.ChiudiConnessione(objParametri)
        End Try

        Return DT

    End Function

    '############################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[magnani]	28/04/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function TecnicoRiferimento_from_PIVA(ByVal Piva As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String
        Dim Contatti_R As New Contatti_R
        Dim dt As DataTable

        Dim CF_Tecnico As String = ""
        Dim Tecnico As String = ""
        Dim Dt_Contatti As New DataTable

        dt = Leggi(CStr(Piva),
                   CInt(enum_CodiciAnagrafe.Tecnico),
                   enumSelezioneVariabile.Selezione_TabellaCompleta,
                   "",
                   "",
                   objParametri)

        If Not IsNothing(dt) AndAlso dt.Rows.Count <> 0 Then

            CF_Tecnico = dt.Rows(0).Item("val_cod")

            'tecnico
            If CF_Tecnico <> "" Then

                Dt_Contatti = Contatti_R.Contatti_Contatto_Leggi(objParametri.PivaSuperUser,
                                                                 CStr(CF_Tecnico),
                                                                 0, 0, True, False, 0, 0, False, 0, 0, 0, "", True, 0, 0, 0, 0, 0,
                                                                 enumSelezioneVariabile.Selezione_JoinDescrizioni, "", "", objParametri)
                If Dt_Contatti.Rows.Count > 0 Then

                    Tecnico = CStr(Dt_Contatti.Rows(0).Item("Rag_Soc")) & " " & CStr(Dt_Contatti.Rows(0).Item("Cognome")) & " " & CStr(Dt_Contatti.Rows(0).Item("Nome"))

                    '  Galassi, 08/08/2016 11:24:14: Si è scelto di prendere tutto reg_soc,cognome,nome per poter avere piena compatibilità con il LAN
                    'Tecnico = CStr(Dt_Contatti.Rows(0).Item("Rag_Soc"))
                    'If Tecnico.Trim = "" Then
                    '    Tecnico = CStr(Dt_Contatti.Rows(0).Item("Cognome")) & " " & CStr(Dt_Contatti.Rows(0).Item("Nome"))
                    'End If

                End If

            End If

        End If

        Return Tecnico

    End Function

    '###############################################################################
    Public Function Imprese_CodiciIntestazioneFattura_Leggi(ByVal Piva As String,
                                                            ByVal xFiltroAggiuntivo As String,
                                                            ByVal xOrderBy As String,
                                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Imprese_Codici_Read.Imprese_CodiciIntestazioneFattura_Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   ID_Cod = 0                  =>  si leggono tutti i codici
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT * ")

            strSql.AppendLine(" FROM    Imprese ")

            strSql.AppendLine(" INNER JOIN Imprese_Codici ON Imprese.PIVA = Imprese_Codici.PIVA ")
            strSql.AppendLine(" INNER JOIN UtentiXImprese ON Imprese.PIVA = UtentiXImprese.PIVA ")

            strSql.AppendLine(" WHERE UtentiXImprese.[USER] = '" + Agro_SQL_SaveText(objParametri.PivaSuperUser) + "' ")

            If Piva <> "" Then
                strSql.AppendLine(" AND     (Imprese.Piva = '" & Agro_SQL_SaveText(Piva) & "')   ")
            End If

            If Piva <> "" Then
                strSql.AppendLine(" AND     (Imprese_Codici.Id_Cod IN (1111,1112,1113,1119) )   ")
            End If


            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   (Imprese_Codici.inviato >= 0)")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   (Imprese_Codici.inviato = -1) ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Imprese_Codici.Val_Cod ASC")
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

    '###############################################################################
    Public Function EsistePivaSuperUser_OrigineDato(ByVal Piva As String,
                                                    ByVal SuperUser_CodFiscale As String,
                                                    ByRef PivaSuperUser_Origine As String,
                                                    ByRef SuperUser_Corrente As Boolean,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                    ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Imprese_Codici_Read.EsistePivaSuperUser_OrigineDato()"

        Dim Esiste_Piva As Boolean = False

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT Imprese_Codici.* ")
            strSql.AppendLine(" FROM Imprese_Codici   ")
            strSql.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "'  ")
            strSql.AppendLine(" AND Id_Cod = " & Agro_SQL_SaveNum(enum_CodiciAnagrafe.PivaSuperUser_Origine_Dato))


            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   (Imprese_Codici.inviato >= 0)")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   (Imprese_Codici.inviato = -1) ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If dt.Rows.Count <> 0 Then

                PivaSuperUser_Origine = CStr(dt.Rows(0).Item("val_cod"))

                If PivaSuperUser_Origine = SuperUser_CodFiscale Then
                    SuperUser_Corrente = True
                Else
                    SuperUser_Corrente = False
                End If

                Esiste_Piva = True

            Else

                PivaSuperUser_Origine = ""
                SuperUser_Corrente = False

                Esiste_Piva = False

            End If

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

            Esiste_Piva = False

        End Try

        Return Esiste_Piva

    End Function

    Public Function LeggiNumeroIscrizioneCameraDiCommercio(ByVal piva As String,
                                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Codici_Read.LeggiNumeroIscrizioneCameraDiCommercio()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim numeroIscrizione As Integer = 0

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT Val_Cod ")
            StrSQL.AppendLine(" FROM Imprese_Codici   ")
            StrSQL.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(piva) & "'  ")
            StrSQL.AppendLine(" AND (id_cod IN (" & enum_CodiciAnagrafe.UfficioRea & "," & enum_CodiciAnagrafe.NumeroRea & ") AND val_cod IS NOT NULL) ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            numeroIscrizione = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT
    End Function

    Public Function CUAA_from_Piva_Massivo(ByVal PivaList As List(Of String),
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                           ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Codici_Read.CUAA_from_Piva_Massivo()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, objParametri)

            EseguiQuery_Scrittura(objParametri, CreaTabellaTemp_FiltroPiva(), NomeRoutine)

            If Not IsNothing(PivaList) AndAlso PivaList.Any Then
                Dim chunks = ChunkBy(Of String)(PivaList, 1000)
                For Each chunk In chunks
                    StrSQL.AppendLine("INSERT INTO #TempPiva (Piva) VALUES ")
                    For Each p As String In chunk
                        StrSQL.AppendLine(String.Format("('{0}'),", p))
                    Next
                    Dim strSqlInsert As String = StrSQL.ToString
                    strSqlInsert = strSqlInsert.Remove(strSqlInsert.LastIndexOf(","))
                    StrSQL.Clear()
                    EseguiQuery_Scrittura(objParametri, strSqlInsert, NomeRoutine)
                Next
            End If

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SELECT Imprese.PIVA, CUAA.Val_Cod AS CUAA ")
            StrSQL.Append(" FROM Imprese ")
            If PivaList IsNot Nothing AndAlso PivaList.Count > 0 Then
                StrSQL.AppendLine("	INNER JOIN #TempPiva temp (NOLOCK) ON Imprese.Piva COLLATE SQL_Latin1_General_CP850_CI_AS = temp.Piva ")
            End If
            StrSQL.AppendLine("	INNER JOIN Imprese_Codici CUAA (NOLOCK) ON Imprese.Piva = CUAA.Piva AND Id_Cod = " & enum_CodiciAnagrafe.CodiceCUAA)

            StrSQL.Append(" WHERE 1 = 1 ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            ' Eliminazione tabella temporanea
            EseguiQuery_Scrittura(objParametri, EliminaTabellaTemp_FiltroPiva, NomeRoutine)

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
    Private Function CreaTabellaTemp_FiltroPiva() As String
        Dim stb As New StringBuilder

        stb.AppendLine(" IF OBJECT_ID('tempdb.dbo.#TempPiva') IS NULL BEGIN ")
        stb.AppendLine("    CREATE TABLE #TempPiva ( ")
        stb.AppendLine("        Piva varchar(50) NULL")
        stb.AppendLine("    )")
        stb.AppendLine(" END ")

        Return stb.ToString()
    End Function
    Private Function EliminaTabellaTemp_FiltroPiva() As String
        Dim stb As New StringBuilder

        stb.AppendLine(" IF NOT OBJECT_ID('tempdb.dbo.#TempPiva') IS NULL BEGIN ")
        stb.AppendLine("    DROP TABLE #TempPiva ")
        stb.AppendLine(" END ")

        Return stb.ToString()
    End Function

End Class



'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################



Public Class Imprese_Codici_Write
    Inherits AgronicaCoreDataProvider.DataProvider

    '########################################################################################
    Public Function Scrivi(ByVal Piva As String,
                           ByVal Id_Cod As Integer,
                           ByVal Val_Cod As String,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = "",
                           Optional ByVal Validazione As Integer = 0,
                           Optional ByVal Data_Validazione As DateTime = #2/1/1900#,
                           Optional ByVal UserName_Validazione As String = ""
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Imprese_Codici_Write.Scrivi()"

        '====================================================================================
        'Parametri opzionali :
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
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

        If Data_Validazione = #2/1/1900# Then
            Data_Validazione = Now
        End If

        Try

            strSql.Length = 0
            strSql.AppendLine("INSERT INTO Imprese_Codici( ")
            strSql.AppendLine("                    Piva,      ")
            strSql.AppendLine("                    Id_Cod,    ")
            strSql.AppendLine("                    Val_Cod,    ")
            strSql.AppendLine("                    Validazione,   Data_Validazione,  UserName_Validazione, ")

            strSql.AppendLine("                    Inviato, DataInvio, ")
            strSql.AppendLine("                    Data_Creazione,     Data_Modifica, ")
            strSql.AppendLine("                    UserName_Creazione, UserName_Modifica, ")
            strSql.AppendLine("                    Validita_Inizio,    Validita_Fine ")
            strSql.AppendLine("                    ) ")
            strSql.AppendLine("VALUES (")
            strSql.AppendLine("          '" & Agro_SQL_SaveText(Piva) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Id_Cod) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Val_Cod) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Validazione) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_Validazione) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(UserName_Validazione) & "' ")

            strSql.AppendLine("         , 0  ")
            strSql.AppendLine("         , Null  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            strSql.AppendLine(")")

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

    '####################################################################################
    Public Function Modifica(ByVal Piva As String,
                             ByVal Id_Cod As Integer,
                             ByVal Val_Cod As String,
                             ByVal Validita_Inizio As Date?,
                             ByVal Validita_Fine As Date?,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                             Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                             Optional ByVal username_modifica As String = "",
                             Optional ByVal Validazione As Integer? = Nothing,
                             Optional ByVal Data_Validazione As DateTime? = Nothing,
                             Optional ByVal UserName_Validazione As String = Nothing
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Imprese_Codici_Write.Modifica()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        If Data_modifica = #2/1/1900# Then
            Data_modifica = Now
        End If

        If username_modifica = "" Then
            username_modifica = objParametri.UsernameOperazione
        End If

        Try

            strSql.Length = 0
            strSql.AppendLine("UPDATE Imprese_Codici SET ")
            strSql.AppendLine("    Val_Cod           = '" & Agro_SQL_SaveText(Val_Cod) & "'")
            strSql.AppendLine("   ,Inviato           =  0 ")
            strSql.AppendLine("   ,DataInvio         =  Null ")
            strSql.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Data_modifica))
            strSql.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "'")

            If Not IsNothing(Validita_Inizio) Then
                strSql.AppendLine("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            End If

            If Not IsNothing(Validita_Fine) Then
                strSql.AppendLine("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            End If

            If Not IsNothing(Validazione) Then
                strSql.AppendLine("   ,Validazione =  " & Agro_SQL_SaveNum(Validazione) & " ")
            End If

            If Not IsNothing(Data_Validazione) Then
                strSql.AppendLine("   ,Data_Validazione =  " & Agro_SQL_SaveDateTime(Data_Validazione) & " ")
            End If

            If Not IsNothing(UserName_Validazione) Then
                strSql.AppendLine("   ,UserName_Validazione =  '" & Agro_SQL_SaveText(UserName_Validazione) & "' ")
            End If

            strSql.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            strSql.AppendLine(" AND   Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")
            '---------------------------------------------

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

    '#######################################################################################
    Public Function Cancella(ByVal Piva As String,
                             ByVal Id_Cod As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Imprese_Codici_Write.Cancella()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.Length = 0
                strSql.AppendLine(" UPDATE Imprese_Codici ")
                strSql.AppendLine(" SET ")
                strSql.AppendLine("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.AppendLine("      ,Inviato = -1 ")
                strSql.AppendLine(" WHERE  Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                strSql.AppendLine(" AND Inviato >= 0")

                If Id_Cod <> 0 Then
                    strSql.AppendLine(" AND Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")
                End If

            Else

                strSql.Length = 0
                strSql.AppendLine(" DELETE ")
                strSql.AppendLine(" FROM     Imprese_Codici ")
                strSql.AppendLine(" WHERE    Piva= '" & Agro_SQL_SaveText(Piva) & "' ")

                If Id_Cod <> 0 Then
                    strSql.AppendLine(" AND Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")
                End If

            End If
            '---------------------------------------------

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

    Public Function aggiorna(ByVal PIVA As String,
                             ByVal id_cod As enum_CodiciAnagrafe,
                             ByVal val_cod As String,
                             ByVal datainizio As Date,
                             ByVal datafine As Date,
                             ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        If PIVA = "" Then
            Throw New Exception("PIVA parametro obbligatorio")
        End If

        If id_cod = 0 Then
            Throw New Exception("enum_CodiciAnagrafe parametro obbligatorio")
        End If

        Cancella(PIVA, id_cod, "", objParametriServer)
        Return Scrivi(PIVA, id_cod, val_cod, datainizio, datafine, objParametriServer)

    End Function

End Class
