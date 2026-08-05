Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class Imputazioni_Fasi_R
    Inherits AgronicaCoreDataProvider.DataProvider
    
    Public Function Leggi(ByVal piva As String,
                          ByVal ID_Attivita As Integer,
                          ByVal Imputazione_Cod As Integer,
                          ByVal Tipo_Fase As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          Optional ByVal xSelezioneVariabile As enumSelezioneVariabile? = enumSelezioneVariabile.Selezione_JoinCompleta
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Imputazioni_Fasi_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT *")
                    StrSQL.AppendLine(" FROM Imputazioni_Fasi ")
                    StrSQL.AppendLine(" WHERE Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
                    StrSQL.AppendLine(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
                    StrSQL.AppendLine(" AND   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

                    If piva <> "" Then
                        StrSQL.AppendLine(" AND   Piva = '" & Agro_SQL_SaveText(piva) & "' ")
                    End If

                    If ID_Attivita <> 0 Then
                        StrSQL.AppendLine(" AND   Imputazione_Fase_Cod = " & Agro_SQL_SaveNum(ID_Attivita))
                    End If

                    If Imputazione_Cod <> 0 Then
                        StrSQL.AppendLine(" AND   Imputazione_Cod = " & Agro_SQL_SaveNum(Imputazione_Cod))
                    End If

                    If Tipo_Fase <> 0 Then
                        StrSQL.AppendLine(" AND   Abs(Tipo_Fase) = " & Agro_SQL_SaveNum(Tipo_Fase))
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" And   Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" And   Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query ")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case enumSelezioneVariabile.Selezione_JoinCompleta

                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT Distinct i.Imputazione_Cod, i_f.imputazione_Fase_Cod As Id_Attivita, ")
                    StrSQL.AppendLine(" i.Imputazione_Nome + ' (' + i.Imputazione_Cod_Des + ')' as Imputazione_Nome, 100 as Valore ")
                    StrSQL.AppendLine(" From [Imputazioni_Fasi] i_f  ")
                    StrSQL.AppendLine(" Join imputazioni i on i.Imputazione_Cod =  i_f.Imputazione_Cod  ")
                    StrSQL.AppendLine(" Join attivita A on A.ID_Attivita =  i_f.imputazione_Fase_Cod  ")
                    StrSQL.AppendLine(" WHERE A.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
                    StrSQL.AppendLine(" AND   A.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
                    StrSQL.AppendLine(" AND   A.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                    StrSQL.AppendLine(" AND   i.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                    StrSQL.AppendLine(" AND   i.Piva = '" & Agro_SQL_SaveText(piva) & "' ")
                    
                    If ID_Attivita <> 0 Then
                        StrSQL.AppendLine(" AND   A.ID_Attivita = " & Agro_SQL_SaveNum(ID_Attivita))
                    End If

                    If Imputazione_Cod <> 0 Then
                        StrSQL.AppendLine(" AND   i.Imputazione_Cod = " & Agro_SQL_SaveNum(Imputazione_Cod))
                    End If

                    If tipo_fase <> 0 Then
                        StrSQL.AppendLine(" AND   abs(i_f.Tipo_Fase) = " & Agro_SQL_SaveNum(Tipo_Fase))
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" And   A.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" And   A.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query ")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni
                    'Utilizzata in Anagrafiche CdG-> TAb CdC Progetti-> Tab Associazione Progetti/Attività
                    'Per ora prendo solo i consuntivi (-1 e -2).
                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT DISTINCT i.Imputazione_Cod, i_f.imputazione_Fase_Cod AS ID_Attivita, ")
                    StrSQL.AppendLine(" A.[Desc] AS Attivita_Des,")
                    StrSQL.AppendLine(" i.Imputazione_Nome AS Imputazione_Nome,")
                    StrSQL.AppendLine(" i_f.Tipo_Fase,")
                    StrSQL.AppendLine(" CASE WHEN i_f.Tipo_Fase= -" & enum_TipoEntitaImputazione.COSTI & " THEN 'Costi' ")
                    StrSQL.AppendLine(" WHEN i_f.Tipo_Fase= -" & enum_TipoEntitaImputazione.RICAVI & " THEN 'Ricavi' END AS Tipo_Fase_Des, ")
                    StrSQL.AppendLine(" it.Tipo_Imputazione, ")
                    StrSQL.AppendLine(" it.Tipo_Imputazione_Des, ")
                    StrSQL.AppendLine(" ic.Imputazione_Classe_Cod, ")
                    StrSQL.AppendLine(" ic.Imputazione_Classe_Des, ")
                    StrSQL.AppendLine(" i_f.Data_Creazione AS Data_Creazione, ")
                    StrSQL.AppendLine(" i_f.Validita_Inizio ,i_f.Validita_Fine,i_f.Data_Creazione ")
                    StrSQL.AppendLine(" FROM Imputazioni_Fasi i_f ")
                    StrSQL.AppendLine(" LEFT JOIN imputazioni i ON i.Imputazione_Cod =  i_f.Imputazione_Cod  ")
                    StrSQL.AppendLine(" LEFT JOIN Imputazioni_Tipi it on it.Piva_SuperUser =  i_f.Piva_SuperUser And it.Piva = i_f.Piva And i.Tipo_Imputazione = it.Tipo_Imputazione ")
                    StrSQL.AppendLine(" LEFT JOIN Imputazioni_Classi ic on ic.Piva_SuperUser =  i_f.Piva_SuperUser And ic.Piva = i_f.Piva And i.Imputazione_Classe_Cod = ic.Imputazione_Classe_Cod  ")
                    StrSQL.AppendLine(" LEFT JOIN attivita A ON A.ID_Attivita =  i_f.imputazione_Fase_Cod  ")
                    StrSQL.AppendLine(" WHERE A.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
                    StrSQL.AppendLine(" AND   A.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
                    StrSQL.AppendLine(" AND   A.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                    StrSQL.AppendLine(" AND   i.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                    StrSQL.AppendLine(" AND   i.Piva = '" & Agro_SQL_SaveText(piva) & "' ")

                    If ID_Attivita <> 0 Then
                        StrSQL.AppendLine(" AND   A.ID_Attivita = " & ID_Attivita)
                    End If
                    If Imputazione_Cod <> 0 Then
                        StrSQL.AppendLine(" AND   i.Imputazione_Cod = " & Imputazione_Cod)
                    End If
                    If tipo_fase <> 0 Then
                        StrSQL.AppendLine(" AND   i_f.Tipo_Fase = " & tipo_fase)
                    End If

                    If xFiltroAggiuntivo <> "" Then

                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))

                    ElseIf String.IsNullOrEmpty(xFiltroAggiuntivo) AndAlso tipo_fase = 0 Then

                        StrSQL.AppendLine(" AND (i_f.Tipo_Fase = -" & enum_TipoEntitaImputazione.COSTI & " OR i_f.Tipo_Fase = -" & enum_TipoEntitaImputazione.RICAVI & ")")

                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   A.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   A.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query ")
                    End Select
                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY i_f.Data_Creazione Desc")
                    End If
            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] :   " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Leggi_APP(ByVal piva As String,
                              ByVal xFiltroAggiuntivo As String,
                              ByVal xOrderBy As String,
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                              ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Imputazioni_Fasi_R.Leggi_APP()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '------------------------------------------------------------------
            ' Vengono estratti solo i progetti legati ad attività extra-campagna da passare all'APP 
            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT i.Imputazione_Cod, i_f.imputazione_Fase_Cod As Id_Attivita, ")
            StrSQL.AppendLine(" i.Imputazione_Nome + ' (' + i.Imputazione_Cod_Des + ')' as Imputazione_Nome ")
            StrSQL.AppendLine(" From [Imputazioni_Fasi] i_f  ")
            StrSQL.AppendLine(" Join imputazioni i on i.Imputazione_Cod =  i_f.Imputazione_Cod  ")
            StrSQL.AppendLine(" Join attivita A on A.ID_Attivita =  i_f.imputazione_Fase_Cod  ")
            StrSQL.AppendLine(" WHERE A.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
            StrSQL.AppendLine(" AND   A.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
            StrSQL.AppendLine(" AND   A.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine(" AND   i.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine(" AND   i.Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            StrSQL.AppendLine(" AND   A.Utilizzo_GiasAPP = 1 ")
            'StrSQL.AppendLine(" AND   A.Attivita_Extra_Campagna = 1 ")
            StrSQL.AppendLine(" AND   i_f.tipo_fase = -1 ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   A.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   A.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query ")
            End Select
            '--------------------------------------------------------------------------
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

End Class


Public Class Imputazioni_Fasi_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal Piva As String,
                           ByVal Imputazione_Cod As Integer,
                           ByVal Imputazione_Fase_Cod As Integer,
                           ByVal Imputazione_Fase_Nome As String,
                           ByVal Imputazione_Fase_Cod_Des As String,
                           ByVal Imputazione_Fase_Des As String,
                           ByVal Tipo_Fase As Integer,
                           ByVal ChkRicette As Integer,
                           ByVal Filtro_Operazioni As String,
                           ByVal Filtro_Operazioni_Dettagli As String,
                           ByVal Filtro_Preparazioni As String,
                           ByVal Filtro_Preparazioni_Dettagli As String,
                           ByVal Fase_Budget As Decimal,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Imputazioni_Fasi_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False
        
        Try

            StrSQL.Length = 0
            StrSQL.AppendLine("INSERT INTO Imputazioni_Fasi( ")
            StrSQL.AppendLine("                     Piva,   Piva_SuperUser,   Imputazione_Cod,     Imputazione_Fase_Cod, ")
            StrSQL.AppendLine("                     Imputazione_Fase_Nome,   Imputazione_Fase_Cod_Des,   Imputazione_Fase_Des, Ordine,")
            StrSQL.AppendLine("                     Tipo_Fase,  Filtro_Operazioni,   Fase_Budget, Filtro_Operazioni_Dettagli,")
            StrSQL.AppendLine("                     Filtro_Preparazioni,  Filtro_Preparazioni_Dettagli,  ChkRicette,")
            StrSQL.AppendLine("                     Inviato,            DataInvio, ")
            StrSQL.AppendLine("                     Data_Creazione,     Data_Modifica, ")
            StrSQL.AppendLine("                     UserName_Creazione, UserName_Modifica, ")
            StrSQL.AppendLine("                     Validita_Inizio,    Validita_Fine ")
            StrSQL.AppendLine("                     ) ")


            StrSQL.AppendLine("VALUES ( ")
            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Imputazione_Cod) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Imputazione_Fase_Cod) & " ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Imputazione_Fase_Nome) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Imputazione_Fase_Cod_Des) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Imputazione_Fase_Des) & "' ")
            StrSQL.AppendLine("         ,(SELECT ISNULL( MAX(Ordine), 0) + 1 AS Ultimo_Ordine FROM Imputazioni_Fasi ")
            StrSQL.AppendLine("             WHERE Piva_SuperUser='" & Trim(objParametri.PivaSuperUser) & "'")
            StrSQL.AppendLine("             AND Piva='" & Agro_SQL_SaveText(Piva) & "'")
            StrSQL.AppendLine("             AND Imputazione_Cod=" & Agro_SQL_SaveNum(Imputazione_Cod) & " ")
            StrSQL.AppendLine("             AND   Tipo_Fase = " & Agro_SQL_SaveNum(Tipo_Fase) & ") ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Tipo_Fase) & " ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Filtro_Operazioni) & "' ")
            StrSQL.AppendLine("         ," & Agro_SQL_SaveNum(Fase_Budget) & " ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Filtro_Operazioni_Dettagli) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Filtro_Preparazioni) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Filtro_Preparazioni_Dettagli) & "' ")
            StrSQL.AppendLine("         ," & Agro_SQL_SaveNum(ChkRicette) & " ")


            StrSQL.AppendLine("         , 0  ")
            StrSQL.AppendLine("         , Null  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Now) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Now) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.AppendLine(" )")
            '---------------------------------------------

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
    
    Public Function Modifica(ByVal Piva As String,
                             ByVal Imputazione_Cod As Integer,
                             ByVal Imputazione_Fase_Cod As Integer,
                             ByVal Imputazione_Fase_Nome As String,
                             ByVal Imputazione_Fase_Cod_Des As String,
                             ByVal Imputazione_Fase_Des As String,
                             ByVal Tipo_Fase As Integer,
                             ByVal ChkRicette As Integer,
                             ByVal Filtro_Operazioni As String,
                             ByVal Filtro_Operazioni_Dettagli As String,
                             ByVal Filtro_Preparazioni As String,
                             ByVal Filtro_Preparazioni_Dettagli As String,
                             ByVal Fase_Budget As Decimal,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Imputazioni_Fasi_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine("UPDATE Imputazioni_Fasi SET ")
            StrSQL.AppendLine("     Imputazione_Fase_Nome = '" & Agro_SQL_SaveText(Imputazione_Fase_Nome) & "'")
            StrSQL.AppendLine("    ,Imputazione_Fase_Cod_Des    =  '" & Agro_SQL_SaveText(Imputazione_Fase_Cod_Des) & "' ")
            StrSQL.AppendLine("    ,Imputazione_Fase_Des    =  '" & Agro_SQL_SaveText(Imputazione_Fase_Des) & "' ")
            StrSQL.AppendLine("    ,Imputazione_Fase_Cod    =  " & Agro_SQL_SaveNum(Imputazione_Fase_Cod) & " ")
            StrSQL.AppendLine("    ,Tipo_Fase    =  " & Agro_SQL_SaveNum(Tipo_Fase) & " ")
            StrSQL.AppendLine("    ,ChkRicette    =  " & Agro_SQL_SaveNum(ChkRicette) & " ")
            StrSQL.AppendLine("    ,Filtro_Operazioni    =  '" & Agro_SQL_SaveText(Filtro_Operazioni) & "' ")
            StrSQL.AppendLine("    ,Filtro_Operazioni_Dettagli    =  '" & Agro_SQL_SaveText(Filtro_Operazioni_Dettagli) & "' ")
            StrSQL.AppendLine("    ,Filtro_Preparazioni    =  '" & Agro_SQL_SaveText(Filtro_Preparazioni) & "' ")
            StrSQL.AppendLine("    ,Filtro_Preparazioni_Dettagli    =  '" & Agro_SQL_SaveText(Filtro_Preparazioni_Dettagli) & "' ")
            StrSQL.AppendLine("    ,Fase_Budget    =  " & Agro_SQL_SaveNum(Fase_Budget) & " ")


            StrSQL.AppendLine("   ,Inviato           =  0 ")
            StrSQL.AppendLine("   ,DataInvio         =  Null ")
            StrSQL.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now))
            StrSQL.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.AppendLine("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.AppendLine("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            StrSQL.AppendLine(" WHERE Piva_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")
            StrSQL.AppendLine(" AND   Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.AppendLine(" AND   Imputazione_Cod = " & Agro_SQL_SaveNum(Imputazione_Cod) & " ")
            StrSQL.AppendLine(" AND   Imputazione_Fase_Cod = " & Agro_SQL_SaveNum(Imputazione_Fase_Cod) & " ")
            StrSQL.AppendLine(" AND   Tipo_Fase = " & Agro_SQL_SaveNum(Tipo_Fase) & " ")
            '---------------------------------------------

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

    Public Function Cancella(ByVal Piva As String,
                             ByVal Imputazione_Cod As Integer,
                             ByVal Imputazione_Fase_Cod As Integer,
                             ByVal Tipo_Fase As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Imputazioni_Fasi_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------

            StrSQL.Length = 0
            StrSQL.AppendLine(" DELETE ")
            StrSQL.AppendLine(" FROM  Imputazioni_Fasi ")
            StrSQL.AppendLine(" WHERE  Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "'  ")
            StrSQL.AppendLine(" AND Imputazione_Cod = " & Agro_SQL_SaveNum(Imputazione_Cod) & "  ")
            StrSQL.AppendLine(" AND Imputazione_Fase_Cod = " & Agro_SQL_SaveNum(Imputazione_Fase_Cod) & "  ")
            StrSQL.AppendLine(" AND Tipo_Fase = " & Agro_SQL_SaveNum(Tipo_Fase) & "  ")

            '----------------------------------------------------------------------
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

End Class
