Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Imprese_Contratti_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal PIVA As String,
                          ByVal Contratto_Cod As Integer,
                          ByVal Contratto_Numero As String,
                          ByVal Cau_Contratto As String,
                          ByVal Cod_Contatto As String,
                          ByVal Cod_Risum As Integer,
                          ByVal Veg_Cod As Integer,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Imprese_Contratti_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile
                'AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi()
                Case Else
                    StrSQL.Length = 0

                    StrSQL.AppendLine(" SELECT Distinct Imprese_Contratti.*, Isnull(Contatti.Rag_Soc, '') as Rag_Soc, Year(Imprese_Contratti.Validita_inizio) as Anno ")
                    StrSQL.AppendLine(" FROM   Imprese_Contratti  ")
                    StrSQL.AppendLine(" LEFT OUTER JOIN Risorse_Umane ON ((Risorse_Umane.Piva = '" & Agro_SQL_SaveText(PIVA) & "' Or Risorse_Umane.Sa_Cod = -1) And Imprese_Contratti.Cod_RisUm = Risorse_Umane.Cod_RisUm)")
                    StrSQL.AppendLine(" LEFT OUTER JOIN Contatti ON ((Contatti.Piva = '" & Agro_SQL_SaveText(PIVA) & "' Or Contatti.Sa_Cod = -1) And Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto)")
                    StrSQL.AppendLine(" LEFT OUTER JOIN Imprese_Contratto_Fasi ON (Imprese_Contratto_Fasi.Piva = Imprese_Contratti.Piva And Imprese_Contratto_Fasi.Contratto_Cod = Imprese_Contratti.Contratto_Cod)")

                    If Veg_Cod <> 0 Then
                        StrSQL.AppendLine(" And Imprese_Contratto_Fasi.Mat_Cod in (Select Mat_Cod From Materie_Prime Where Veg_Cod = " & Veg_Cod & ")")
                    End If


                    StrSQL.AppendLine(" WHERE Imprese_Contratti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   Imprese_Contratti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.AppendLine(" AND   Imprese_Contratti.Piva = '" & Agro_SQL_SaveText(PIVA) & "'   ")


                    If Contratto_Cod <> -1 Then
                        StrSQL.AppendLine(" AND Imprese_Contratti.Contratto_Cod = " & Agro_SQL_SaveNum(Contratto_Cod) & "   ")
                    End If

                    If Trim(Contratto_Numero) <> "" Then
                        StrSQL.AppendLine(" AND Imprese_Contratti.Contratto_Numero = '" & Agro_SQL_SaveText(Contratto_Numero) & "'   ")
                    End If

                    If Trim(Cau_Contratto) <> "" Then
                        StrSQL.AppendLine(" AND Imprese_Contratti.Cau_Contratto = '" & Agro_SQL_SaveText(Cau_Contratto) & "'   ")
                    End If

                    If Cod_Risum <> 0 Then
                        StrSQL.AppendLine(" AND Imprese_Contratti.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_Risum) & "   ")
                    End If

                    If Cod_Contatto <> "" Then
                        StrSQL.AppendLine(" AND Contatti.Cod_Contatto  = '" & Agro_SQL_SaveNum(Cod_Contatto) & "'   ")
                    End If


                    'If Elem_Cod <> 0 Then
                    '    StrSQL.Append(" AND Imprese_Contratto_Fasi.Elem_Cod  = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
                    '    'sSql = sSql & " AND Imprese_Contratto_Fasi.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   "
                    'End If

                    'If Pro_Cod <> 0 Then
                    '    StrSQL.Append(" AND Imprese_Contratto_Fasi.Pro_Cod  = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
                    '    'sSql = sSql & " AND Imprese_Contratto_Fasi.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   "
                    'End If

                    'If Mat_Cod <> 0 Then
                    '    StrSQL.Append(" AND  Imprese_Contratto_Fasi.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
                    '    'sSql = sSql & " AND Imprese_Contratto_Fasi.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   "
                    'End If

                    'If Udm_Cod <> 0 Then
                    '    StrSQL.Append(" AND Imprese_Contratto_Fasi.Udm_Cod  = " & Agro_SQL_SaveNum(Udm_Cod) & "   ")
                    '    'sSql = sSql & " AND Imprese_Contratto_Fasi.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   "
                    'End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   Imprese_Contratti.Inviato >=0 ")

                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   Imprese_Contratti.Inviato =-1 ")

                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine("ORDER BY Imprese_Contratti.Piva ASC, Imprese_Contratti.Contratto_Numero ASC, Rag_Soc ASC")
                    End If

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

    'Lettura Anni Distinti Contratti
    Public Function Leggi_Anno(ByVal PIVA As String,
                               ByVal Contratto_Cod As Integer,
                               ByVal Contratto_Numero As String,
                               ByVal Cau_Contratto As String,
                               ByVal Cod_Contatto As String,
                               ByVal Cod_Risum As Integer,
                               ByVal Veg_Cod As Integer,
                               ByVal xSelezioneVariabile As enumSelezioneVariabile,
                               ByVal xFiltroAggiuntivo As String,
                               ByVal xOrderBy As String,
                               ByRef objParametri As AgronicaCoreParametri
                               ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Imprese_Contratti_R.Leggi_Anno()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile
                'AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi()
                Case Else
                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT Distinct Year(Imprese_Contratti.Validita_inizio) as Anno, Year(Imprese_Contratti.Validita_inizio) as Anno_Des  ")
                    StrSQL.Append(" FROM   Imprese_Contratti  ")


                    If Veg_Cod <> 0 Then
                        StrSQL.Append(" Inner JOIN Imprese_Contratto_Fasi ON (Imprese_Contratto_Fasi.Piva = Imprese_Contratti.Piva And Imprese_Contratto_Fasi.Contratto_Cod = Imprese_Contratti.Contratto_Cod) ")
                        StrSQL.Append(" And Imprese_Contratto_Fasi.Mat_Cod in (Select Mat_Cod From Materie_Prime Where Veg_Cod = " & Veg_Cod & ")")
                    End If

                    StrSQL.Append(" LEFT OUTER JOIN Risorse_Umane ON ((Risorse_Umane.Piva = '" & Agro_SQL_SaveText(PIVA) & "' Or Risorse_Umane.Sa_Cod = -1) And Imprese_Contratti.Cod_RisUm = Risorse_Umane.Cod_RisUm)")
                    StrSQL.Append(" LEFT OUTER JOIN Contatti ON ((Contatti.Piva = '" & Agro_SQL_SaveText(PIVA) & "' Or Contatti.Sa_Cod = -1) And Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto)")


                    StrSQL.Append(" WHERE Imprese_Contratti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Imprese_Contratti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   Imprese_Contratti.Piva = '" & Agro_SQL_SaveText(PIVA) & "'   ")
                    

                    If Contratto_Cod <> 0 Then
                        StrSQL.Append(" AND Imprese_Contratti.Contratto_Cod = " & Agro_SQL_SaveNum(Contratto_Cod) & "   ")
                    End If

                    If Trim(Contratto_Numero) <> "" Then
                        StrSQL.Append(" AND Imprese_Contratti.Contratto_Numero = '" & Agro_SQL_SaveText(Contratto_Numero) & "'   ")
                    End If

                    If Trim(Cau_Contratto) <> "" Then
                        StrSQL.Append(" AND Imprese_Contratti.Cau_Contratto = '" & Agro_SQL_SaveText(Cau_Contratto) & "'   ")
                    End If

                    If Cod_Risum <> 0 Then
                        StrSQL.Append(" AND Imprese_Contratti.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_Risum) & "   ")
                    End If

                    If Cod_Contatto <> "" Then
                        StrSQL.Append(" AND Contatti.Cod_Contatto  = '" & Agro_SQL_SaveNum(Cod_Contatto) & "'   ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Imprese_Contratti.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Imprese_Contratti.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append("ORDER BY Anno ASC ")
                    End If

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

    'Lettura Prodotti Distinti Contratti
    Public Function Leggi_Prodotti(ByVal PIVA As String,
                                   ByVal Cod_Contatto As String,
                                   ByVal Cod_Risum As Integer,
                                   ByVal Veg_Cod As Integer,
                                   ByVal Elem_Cod As Integer,
                                   ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByVal xOrderBy As String,
                                   ByRef objParametri As AgronicaCoreParametri
                                   ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Imprese_Contratti_R.Leggi_Prodotti()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile
                'AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi()
                Case Else
                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT Distinct Materie_Prime.Mat_Cod, Materie_Prime.Cod_Articolo, Materie_Prime.Mat_Des  ")
                    StrSQL.Append(" FROM   Imprese_Contratto_Fasi, Materie_Prime, Imprese_Contratti  ")
                    StrSQL.Append(" LEFT OUTER JOIN Risorse_Umane ON ((Risorse_Umane.Piva = '" & Agro_SQL_SaveText(PIVA) & "' Or Risorse_Umane.Sa_Cod = -1) And Imprese_Contratti.Cod_RisUm = Risorse_Umane.Cod_RisUm)")
                    StrSQL.Append(" LEFT OUTER JOIN Contatti ON ((Contatti.Piva = '" & Agro_SQL_SaveText(PIVA) & "' Or Contatti.Sa_Cod = -1) And Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto)")
                    StrSQL.Append(" WHERE Imprese_Contratti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Imprese_Contratti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   Imprese_Contratti.Piva = '" & Agro_SQL_SaveText(PIVA) & "'   ")
                    StrSQL.Append(" AND   Imprese_Contratti.Piva = Imprese_Contratto_Fasi.Piva  ")
                    StrSQL.Append(" AND   Imprese_Contratti.Contratto_Cod = Imprese_Contratto_Fasi.Contratto_Cod  ")
                    StrSQL.Append(" AND   Materie_Prime.Elem_Cod = Imprese_Contratto_Fasi.Elem_Cod  ")
                    StrSQL.Append(" AND   Materie_Prime.Mat_Cod = Imprese_Contratto_Fasi.Mat_Cod  ")

                    If Elem_Cod <> 0 Then
                        StrSQL.Append(" AND Imprese_Contratto_Fasi.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
                    End If

                    If Veg_Cod <> 0 Then
                        StrSQL.Append(" AND Materie_Prime.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "   ")
                    End If

                    If Cod_Risum <> 0 Then
                        StrSQL.Append(" AND Imprese_Contratti.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_Risum) & "   ")
                    End If

                    If Cod_Contatto <> "" Then
                        StrSQL.Append(" AND Contatti.Cod_Contatto = '" & Agro_SQL_SaveNum(Cod_Contatto) & "'   ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Imprese_Contratti.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Imprese_Contratti.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append("ORDER BY Mat_Des ASC ")
                    End If

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



End Class

'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################

Public Class Imprese_Contratti_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal Piva As String,
                       ByVal Contratto_Cod As Integer,
                       ByVal Riferimento As String,
                       ByVal Contratto_Nome As String,
                       ByVal Contratto_Des As String,
                       ByVal Contratto_Numero As String,
                       ByVal Superficie_Prevista As Double,
                       ByVal Resa_Prevista As Double,
                       ByVal Ricavi_Previsti As Double,
                       ByVal Cau_Contratto As String,
                       ByVal Cod_Conto As Integer,
                       ByVal Giudizio As String,
                       ByVal Data_Inizio_Prevista As Date,
                       ByVal Data_Fine_Prevista As Date,
                       ByVal Descrizione_1 As String,
                       ByVal Descrizione_2 As String,
                       ByVal Cod_RisUm As Integer,
                       ByVal Stato As Integer,
                       ByVal ChkStato_Automatico As Integer,
                       ByVal Cau_Pagamento As Integer,
                       ByVal Data_Stipulazione As Date,
                       ByVal Validita_Inizio As Date,
                       ByVal Validita_Fine As Date,
                       ByVal sa_cod As Integer,
                       ByVal Fabbricato_Cod As Integer,
                       ByRef objParametri As AgronicaCoreParametri
                       ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Imprese_Contratti_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" INSERT INTO Imprese_Contratti ")
            StrSQL.AppendLine("         ( ")
            StrSQL.AppendLine("            Piva,                 Contratto_Cod,         Contratto_Des, ")
            StrSQL.AppendLine("            Contratto_Nome,       Cau_Contratto,         Contratto_Numero,  ")
            StrSQL.AppendLine("            Cod_Conto,            Giudizio,              Riferimento,            ")
            StrSQL.AppendLine("            Descrizione_1,        Descrizione_2,             ")
            StrSQL.AppendLine("            Data_Inizio_Prevista, Data_Fine_Prevista,            ")
            StrSQL.AppendLine("            Superficie_Prevista,  Ricavi_Previsti,       Resa_Prevista,            ")
            StrSQL.AppendLine("            Cod_RisUm,            Stato,                 ChkStato_Automatico,            ")
            StrSQL.AppendLine("            Cau_Pagamento,        Data_Stipulazione,            ")

            StrSQL.AppendLine("          Inviato,            DataInvio, ")
            StrSQL.AppendLine("          Data_Creazione,     Data_Modifica, ")
            StrSQL.AppendLine("          UserName_Creazione, UserName_Modifica, ")
            StrSQL.AppendLine("          Validita_Inizio,    Validita_Fine,")
            StrSQL.AppendLine("          sa_cod,    Fabbricato_Cod ")
            StrSQL.AppendLine("         ) ")

            StrSQL.AppendLine(" VALUES ( ")
            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Contratto_Cod) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Contratto_Des) & "'  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Contratto_Nome) & "'  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(UCase(Cau_Contratto)) & "'  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Contratto_Numero) & "'  ")

            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Cod_Conto) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Giudizio) & "'   ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Riferimento) & "'  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Descrizione_1) & "'  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Descrizione_2) & "'  ")

            StrSQL.AppendLine("        , " & Agro_SQL_SaveDate(Data_Inizio_Prevista) & "  ")
            StrSQL.AppendLine("        , " & Agro_SQL_SaveDate(Data_Fine_Prevista) & "  ")



            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Superficie_Prevista) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Ricavi_Previsti) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Resa_Prevista) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Cod_RisUm) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Stato) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(ChkStato_Automatico) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Cau_Pagamento) & "  ")
            StrSQL.AppendLine("        , " & Agro_SQL_SaveDate(Data_Stipulazione) & "  ")


            StrSQL.AppendLine("         , 0  ")
            StrSQL.AppendLine("         , Null  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Now) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Now) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(sa_cod) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Fabbricato_Cod) & "  ")

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

    Public Function Modifica(ByVal Piva As String,
                       ByVal Contratto_Cod As Integer,
                       ByVal Riferimento As String,
                       ByVal Contratto_Nome As String,
                       ByVal Contratto_Des As String,
                       ByVal Contratto_Numero As String,
                       ByVal Superficie_Prevista As Double,
                       ByVal Resa_Prevista As Double,
                       ByVal Ricavi_Previsti As Double,
                       ByVal Cau_Contratto As String,
                       ByVal Cod_Conto As Integer,
                       ByVal Giudizio As String,
                       ByVal Data_Inizio_Prevista As Date,
                       ByVal Data_Fine_Prevista As Date,
                       ByVal Descrizione_1 As String,
                       ByVal Descrizione_2 As String,
                       ByVal Cod_RisUm As Integer,
                       ByVal Stato As Integer,
                       ByVal ChkStato_Automatico As Integer,
                       ByVal Cau_Pagamento As Integer,
                       ByVal Data_Stipulazione As Date,
                       ByVal Validita_Inizio As Date,
                       ByVal Validita_Fine As Date,
                       ByVal sa_cod As Integer,
                       ByVal Fabbricato_Cod As Integer,
                       ByRef objParametri As AgronicaCoreParametri
                       ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Imprese_Contratti_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" Update Imprese_Contratti ")
            StrSQL.AppendLine(" Set ")
            StrSQL.AppendLine("    Contratto_Des = '" & Agro_SQL_SaveText(Contratto_Des) & "' ")
            StrSQL.AppendLine("    ,Contratto_Nome = '" & Agro_SQL_SaveText(Contratto_Nome) & "' ")
            StrSQL.AppendLine("    ,Cau_Contratto =  '" & Agro_SQL_SaveText(UCase(Cau_Contratto)) & "'  ")
            StrSQL.AppendLine("    ,Contratto_Numero = '" & Agro_SQL_SaveText(Contratto_Numero) & "'  ")
            StrSQL.AppendLine("    ,Cod_Conto = " & Agro_SQL_SaveNum(Cod_Conto) & "  ")
            StrSQL.AppendLine("    ,Giudizio ='" & Agro_SQL_SaveText(Giudizio) & "'   ")
            StrSQL.AppendLine("    ,Riferimento = '" & Agro_SQL_SaveText(Riferimento) & "'  ")
            StrSQL.AppendLine("    ,Descrizione_1 = '" & Agro_SQL_SaveText(Descrizione_1) & "'  ")
            StrSQL.AppendLine("    ,Descrizione_2 = '" & Agro_SQL_SaveText(Descrizione_2) & "'  ")

            StrSQL.AppendLine("    ,Data_Inizio_Prevista = " & Agro_SQL_SaveDate(Data_Inizio_Prevista) & "  ")
            StrSQL.AppendLine("    ,Data_Fine_Prevista = " & Agro_SQL_SaveDate(Data_Fine_Prevista) & "  ")


            StrSQL.AppendLine("    ,Superficie_Prevista = " & Agro_SQL_SaveNum(Superficie_Prevista) & "  ")
            StrSQL.AppendLine("    ,Ricavi_Previsti = " & Agro_SQL_SaveNum(Ricavi_Previsti) & "  ")
            StrSQL.AppendLine("    ,Resa_Prevista = " & Agro_SQL_SaveNum(Resa_Prevista) & "  ")
            StrSQL.AppendLine("    ,Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "  ")
            StrSQL.AppendLine("    ,Stato = " & Agro_SQL_SaveNum(Stato) & "  ")
            StrSQL.AppendLine("    ,ChkStato_Automatico = " & Agro_SQL_SaveNum(ChkStato_Automatico) & "  ")
            StrSQL.AppendLine("    ,Cau_Pagamento = " & Agro_SQL_SaveNum(Cau_Pagamento) & "  ")
            StrSQL.AppendLine("    ,Data_Stipulazione = " & Agro_SQL_SaveDate(Data_Stipulazione) & "  ")


            StrSQL.AppendLine("    ,Inviato           =  0 ")
            StrSQL.AppendLine("    ,DataInvio         =  Null ")
            StrSQL.AppendLine("    ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now))
            StrSQL.AppendLine("    ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.AppendLine("    ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.AppendLine("    ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.AppendLine("    ,sa_cod = " & Agro_SQL_SaveNum(sa_cod) & "  ")
            StrSQL.AppendLine("    ,Fabbricato_Cod = " & Agro_SQL_SaveNum(Fabbricato_Cod) & "  ")

            StrSQL.AppendLine(" WHERE 1 = 1 ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Contratto_Cod <> 0 Then
                StrSQL.AppendLine(" AND Contratto_Cod = " & Agro_SQL_SaveNum(Contratto_Cod) & "   ")
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

    Public Function Cancella(ByVal Piva As String,
                             ByVal Contratto_Cod As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Imprese_Contratti_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Imprese_Contratti ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")

            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM Imprese_Contratti ")
                StrSQL.Append(" WHERE  1=1 ")

            End If


            If Piva <> "" Then
                StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            'If Contratto_Cod <> 0 Then
            StrSQL.Append(" AND Contratto_Cod = " & Agro_SQL_SaveNum(Contratto_Cod) & "   ")
            'End If


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

    Public Function writeContractFromRequisitiStabilimento(
                                                          ByVal piva As String,
                                                          ByVal sup As Decimal,
                                                          ByVal qta As Decimal,
                                                          ByVal trDes As String,
                                                          ByVal trCod As Integer,
                                                          ByVal contrattoCodFiglio As Integer,
                                                          ByVal contrattoCodPadre As Integer,
                                                          ByVal faseCodPadre As Integer,
                                                          ByVal objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                          )

        Const nomeRoutine = "AgronicaCoreContabDAL.Imprese_Contratti_W.writeContractFromRequisitiStabilimento()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine("SELECT TOP(1) * FROM Imprese_Contratti WHERE Contratto_Cod = " & contrattoCodPadre)
            '-------------------------------------------------------------------------------------
            Dim dtFatherContract = EseguiQuery_Lettura(objParametriServer, StrSQL.ToString, nomeRoutine)
            '-------------------------------------------------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine("SELECT TOP(1) imp.rag_soc, ru.Cod_RisUm")
            StrSQL.AppendLine("FROM Imprese imp")
            StrSQL.AppendLine("    LEFT JOIN Risorse_Umane ru ON ru.Cod_Contatto = imp.Piva AND Cod_Rapporto = -18")
            StrSQL.AppendLine("WHERE imp.PIVA = '" & Agro_SQL_SaveText(piva) & "' ")
            '-------------------------------------------------------------------------------------
            Dim dtSonData = EseguiQuery_Lettura(objParametriServer, StrSQL.ToString, nomeRoutine)
            '-------------------------------------------------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine("INSERT INTO Imprese_Contratti (")
            StrSQL.AppendLine("    Piva,                 Contratto_Cod,         Contratto_Des,")
            StrSQL.AppendLine("    Contratto_Nome,       Cau_Contratto,         Contratto_Numero,")
            StrSQL.AppendLine("    Cod_Conto,            Giudizio,              Riferimento,")
            StrSQL.AppendLine("    Descrizione_1,        Descrizione_2,")
            StrSQL.AppendLine("    Data_Inizio_Prevista, Data_Fine_Prevista,")
            StrSQL.AppendLine("    Superficie_Prevista,  Ricavi_Previsti,       Resa_Prevista,")
            StrSQL.AppendLine("    Cod_RisUm,            Stato,                 ChkStato_Automatico,")
            StrSQL.AppendLine("    Cau_Pagamento,        Data_Stipulazione,            ")
            StrSQL.AppendLine("    Inviato,              DataInvio,")
            StrSQL.AppendLine("    Data_Creazione,       Data_Modifica,")
            StrSQL.AppendLine("    UserName_Creazione,   UserName_Modifica,")
            StrSQL.AppendLine("    Validita_Inizio,      Validita_Fine,")
            StrSQL.AppendLine("    sa_cod,               Fabbricato_Cod,        Contratto_Cod_Padre,")
            StrSQL.AppendLine("    ID_Budget,            Fase_Cod_Padre")
            StrSQL.AppendLine(") ")

            StrSQL.AppendLine("VALUES ( ")
            StrSQL.AppendLine("    '" & Agro_SQL_SaveText(piva) & "',")
            StrSQL.AppendLine("    " & Agro_SQL_SaveNum(contrattoCodFiglio) & ",")
            StrSQL.AppendLine("    '" & Agro_SQL_SaveText(dtFatherContract.Rows(0)("Contratto_Des")) & "',")
            StrSQL.AppendLine("    '" & Agro_SQL_SaveText(dtSonData.Rows(0)("rag_soc")) & "',")
            StrSQL.AppendLine("    '" & Agro_SQL_SaveText(UCase("9301")) & "',")
            StrSQL.AppendLine("    '" & Agro_SQL_SaveText(dtFatherContract.Rows(0)("Contratto_Numero")) & "',")
            StrSQL.AppendLine("    " & Agro_SQL_SaveNum(dtFatherContract.Rows(0)("Cod_Conto")) & ",")
            StrSQL.AppendLine("    '" & Agro_SQL_SaveText(dtFatherContract.Rows(0)("Giudizio")) & "',")
            StrSQL.AppendLine("    '" & Agro_SQL_SaveText(dtFatherContract.Rows(0)("Riferimento")) & "',")
            StrSQL.AppendLine("    '" & Agro_SQL_SaveText(dtFatherContract.Rows(0)("Descrizione_1")) & "',")
            StrSQL.AppendLine("    '" & Agro_SQL_SaveText(dtFatherContract.Rows(0)("Descrizione_2")) & "',")
            StrSQL.AppendLine("    " & Agro_SQL_SaveDate(dtFatherContract.Rows(0)("Data_Inizio_Prevista")) & ",")
            StrSQL.AppendLine("    " & Agro_SQL_SaveDate(dtFatherContract.Rows(0)("Data_Fine_Prevista")) & ",")
            StrSQL.AppendLine("    " & Agro_SQL_SaveNum(dtFatherContract.Rows(0)("Superficie_Prevista")) & ",")
            StrSQL.AppendLine("    " & Agro_SQL_SaveNum(dtFatherContract.Rows(0)("Ricavi_Previsti")) & ",")
            StrSQL.AppendLine("    " & Agro_SQL_SaveNum(dtFatherContract.Rows(0)("Resa_Prevista")) & ",")
            StrSQL.AppendLine("    " & Agro_SQL_SaveNum(If(IsDBNull(dtSonData.Rows(0)("Cod_RisUm")), 0, dtSonData.Rows(0)("Cod_RisUm"))) & ",")
            StrSQL.AppendLine("    " & Agro_SQL_SaveNum(dtFatherContract.Rows(0)("Stato")) & ",")
            StrSQL.AppendLine("    " & Agro_SQL_SaveNum(dtFatherContract.Rows(0)("ChkStato_Automatico")) & ",")
            StrSQL.AppendLine("    " & Agro_SQL_SaveNum(dtFatherContract.Rows(0)("Cau_Pagamento")) & ",")
            StrSQL.AppendLine("    " & Agro_SQL_SaveDate(dtFatherContract.Rows(0)("Data_Stipulazione")) & ",")
            StrSQL.AppendLine("    0,")
            StrSQL.AppendLine("    Null,")
            StrSQL.AppendLine("    " & Agro_SQL_SaveDateTime(Now) & ",")
            StrSQL.AppendLine("    " & Agro_SQL_SaveDateTime(Now) & ",")
            StrSQL.AppendLine("    '" & Agro_SQL_SaveText(objParametriServer.UsernameOperazione) & "',")
            StrSQL.AppendLine("    '" & Agro_SQL_SaveText(objParametriServer.UsernameOperazione) & "',")
            StrSQL.AppendLine("    " & Agro_SQL_SaveDate(dtFatherContract.Rows(0)("Validita_Inizio")) & ",")
            StrSQL.AppendLine("    " & Agro_SQL_SaveDate(dtFatherContract.Rows(0)("Validita_Fine")) & ",")
            StrSQL.AppendLine("    " & Agro_SQL_SaveNum(dtFatherContract.Rows(0)("sa_cod")) & ",")
            StrSQL.AppendLine("    " & Agro_SQL_SaveNum(dtFatherContract.Rows(0)("Fabbricato_Cod")) & ",")
            'StrSQL.AppendLine("    " & Agro_SQL_SaveNum(If(IsDBNull(dtFatherContract.Rows(0)("Contratto_Cod_Padre")), 0, dtFatherContract.Rows(0)("Contratto_Cod_Padre"))) & ",")
            'StrSQL.AppendLine("    " & Agro_SQL_SaveNum(If(IsDBNull(dtFatherContract.Rows(0)("ID_Budget")), 0, dtFatherContract.Rows(0)("ID_Budget"))) & ",")
            'StrSQL.AppendLine("    " & Agro_SQL_SaveNum(If(IsDBNull(dtFatherContract.Rows(0)("Fase_Cod_Padre")), 0, dtFatherContract.Rows(0)("Fase_Cod_Padre"))))
            StrSQL.AppendLine("    " & Agro_SQL_SaveNum(contrattoCodPadre) & ",")
            StrSQL.AppendLine("    " & Agro_SQL_SaveNum(0) & ",")
            StrSQL.AppendLine("    " & Agro_SQL_SaveNum(faseCodPadre))
            StrSQL.Append(")")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametriServer, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function deleteContractFromRequisitiStabilimento(
                                                           ByVal piva As String,
                                                           ByVal contrattoCodFiglio As Integer,
                                                           ByVal faseCodFiglio As Integer,
                                                           ByVal contrattoCodPadre As Integer,
                                                           ByVal faseCodPadre As Integer,
                                                           ByVal objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                           )

        Const nomeRoutine = "AgronicaCoreContabDAL.Imprese_Contratti_W.deleteContractFromRequisitiStabilimento()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            StrSQL.AppendLine("SELECT * FROM Imprese_Contratto_Fasi WHERE Contratto_Cod = " & Agro_SQL_SaveNum(contrattoCodFiglio))
            Dim dt = EseguiQuery_Lettura(objParametriServer, StrSQL.ToString, nomeRoutine)

            If dt.Rows.Count = 0 Then
                If objParametriServer.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                    StrSQL.Length = 0
                    StrSQL.AppendLine("UPDATE Imprese_Contratti")
                    StrSQL.AppendLine("SET")
                    StrSQL.AppendLine("    Username_Modifica = '" & Agro_SQL_SaveText(objParametriServer.UsernameOperazione) & "',")
                    StrSQL.AppendLine("    Inviato = -1")
                    StrSQL.AppendLine("WHERE Inviato >= 0")
                Else
                    StrSQL.Length = 0
                    StrSQL.AppendLine("DELETE")
                    StrSQL.AppendLine("FROM Imprese_Contratti")
                    StrSQL.AppendLine("WHERE 1=1")
                End If

                If piva <> "" Then
                    StrSQL.AppendLine("    AND Piva = '" & Agro_SQL_SaveText(piva) & "'")
                End If

                StrSQL.AppendLine("    AND Contratto_Cod = " & Agro_SQL_SaveNum(contrattoCodFiglio))

                '--------------------------------------------------------------------------
                xRisp = EseguiQuery_Scrittura(objParametriServer, StrSQL.ToString, nomeRoutine)
                '--------------------------------------------------------------------------
            Else
                xRisp = True
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

End Class
