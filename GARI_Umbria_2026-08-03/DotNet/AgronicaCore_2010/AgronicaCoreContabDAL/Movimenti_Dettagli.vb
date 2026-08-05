Imports System.Data.Entity
Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreDataProvider.My.Resources

Public Class Movimenti_Dettagli_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiProdottiMovimentatiinMagazzino_daIdAgenda(ByVal Piva As String,
                                                                   ByVal Id_Agenda As Integer,
                                                                   ByVal xFiltroAggiuntivo As String,
                                                                   ByVal xOrderBy As String,
                                                                   ByRef objParametri As AgronicaCoreParametri
                                                                   ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Dettagli_R.LeggiProdottiMovimentatiinMagazzino_daIdAgenda()"


        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try


            strSql.Length = 0
            strSql.AppendLine(" SELECT  dest.piva,dest.sa_cod,dest.Id_Destinazione, dett.elem_cod,dett.pro_cod,dett.mat_cod,dett.cal_cod,dett.cod_progetto,dett.lotto,dett.qta,dett.udm_cod,f.fabbricato_des, u.udm_sim ")

            strSql.AppendLine(" FROM    Mov_Destinazioni dest ")
            strSql.AppendLine(" inner Join Movimenti_dettagli dett ON dest.piva=dett.piva AND dest.sa_cod=dett.sa_cod AND dest.id_mov_det=dett.id_mov_det ")
            strSql.AppendLine(" inner join fabbricati f on f.piva=dest.piva AND f.sa_cod=dest.sa_cod AND f.Fabbricato_Cod=dest.id_destinazione ")
            strSql.AppendLine(" inner join unitamisura u on u.udm_cod=dett.Udm_Cod ")

            strSql.AppendLine(" WHERE   tipo_destinazione = 20  ")
            strSql.AppendLine(" AND   dett.piva ='" & Agro_SQL_SaveText(Piva) & "' ")
            strSql.AppendLine(" AND   dett.id_agenda =" & Agro_SQL_SaveNum(Id_Agenda) & " ")

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   dett.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   dett.Inviato =-1 ")
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

    '##############################################################################################
    'utilizzata da importatore di magazzino
    '-> chiedere a maga prima di modificare!
    Public Sub IdMov_IdMovDet_by_IdAgendaChiaveProdotto(ByRef Id_Mov As Integer,
                                                        ByRef Id_Mov_Det As Integer,
                                                        ByVal Piva As String,
                                                        ByVal Id_agenda As Integer,
                                                        ByVal Elem_Cod As Integer,
                                                        ByVal Pro_Cod As Integer,
                                                        ByVal Mat_Cod As Integer,
                                                        ByVal Cod_Progetto As Integer,
                                                        ByVal Fase_Cod As Integer,
                                                        ByVal Lotto As String,
                                                        ByVal Cal_Cod As Integer,
                                                        ByVal Udm_Cod As Integer,
                                                        ByVal Qta As Decimal,
                                                        ByRef indice_riga_dett_stessaqta As Integer,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Id_Mov = 0
        Id_Mov_Det = 0

        Dim dtDett As DataTable
        Dim filtro As String = " Movimenti_dettagli.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod)

        If Lotto <> LOTTO_NONDEFINITO AndAlso Lotto <> "" Then
            filtro &= " AND Movimenti_dettagli.Lotto = " & Agro_SQL_SaveText(Lotto)
        End If

        dtDett = Leggi(Piva,
                        0,
                        Id_agenda,
                        0, 0,
                        Elem_Cod,
                        Pro_Cod,
                        Mat_Cod,
                        "",
                        Cal_Cod,
                        Cod_Progetto,
                        Fase_Cod,
                        0,
                        0,
                        0,
                        enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                        filtro, "",
                        objParametri)

        If Not IsNothing(dtDett) AndAlso dtDett.Rows.Count > 0 Then

            If dtDett.Rows.Count = 1 Then
                'c'è un solo record, non mi sbaglio
                Id_Mov = dtDett.Rows(0).Item("Id_Mov")
                Id_Mov_Det = dtDett.Rows(0).Item("Id_Mov_Det")
            Else
                'se sullo stesso id_agenda ci sono più record relativi allo stesso prodotto
                'provo a cercare l'id_mov_det giusto considerando la qta
                filtro &= " AND Movimenti_dettagli.Qta = " & Agro_SQL_SaveNum(Qta)

                dtDett = Leggi(Piva,
                               0,
                               Id_agenda,
                               0, 0,
                               Elem_Cod,
                               Pro_Cod,
                               Mat_Cod,
                               "",
                               Cal_Cod,
                               Cod_Progetto,
                               Fase_Cod,
                               0,
                               0,
                               0,
                               enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                               filtro, "",
                               objParametri)

                If Not IsNothing(dtDett) AndAlso dtDett.Rows.Count > 0 Then
                    If dtDett.Rows.Count = 1 Then
                        'c'è un solo record, non mi sbaglio
                        Id_Mov = dtDett.Rows(0).Item("Id_Mov")
                        Id_Mov_Det = dtDett.Rows(0).Item("Id_Mov_Det")
                    Else
                        'ci sono più dettagli con lo stesso prodotto e stessa qta (che dati del cavolo che arrivano dal cap!)
                        Id_Mov = dtDett.Rows(indice_riga_dett_stessaqta).Item("Id_Mov")
                        Id_Mov_Det = dtDett.Rows(indice_riga_dett_stessaqta).Item("Id_Mov_Det")
                        indice_riga_dett_stessaqta += 1
                    End If
                Else
                    'questo non dovrebbe capitare
                    'non c'è alcun record di quel prodotto con quella qta
                    Dim debug As Boolean = True
                    Throw New Exception("Sono stati trovati più dettagli dello stesso prodotto, ma nessuno con la quantità di " & CStr(Qta) & " e udm_cod=" & CStr(Udm_Cod))
                End If
            End If 'un solo record
        End If 'DT_Dett vuoto

    End Sub


    '##############################################################################################
    'Conta il numero di trattamenti effettuati nella distinta o nell'anno solare per singolo impianto
    Public Function NumeroTrattamentiperImpianti(ByVal frCod As Integer,
                                                 ByVal xFiltroAggiuntivo As String,
                                                 ByVal xOrderBy As String,
                                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                 ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Dettagli_R.NumeroTrattamentiperImpianti()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            '------------------------------------------------------------------
            strSql.Length = 0

            'Query per il prelievo dei dati                 ' #### CLASSE ####


            strSql.AppendLine(" SELECT      Mov_Destinazioni.id_agenda, Mov_Destinazioni.Piva, Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.Appezza,  ")
            strSql.AppendLine("             Mov_Destinazioni.Id_Destinazione, Movimenti.Data_Movimento, Imprese_Progetti.Validita_Inizio, Imprese_Progetti.Validita_Fine ")
            strSql.AppendLine("             , Movimenti_dettagli.doseetichetta_value ")

            strSql.AppendLine(" FROM         Movimenti_dettagli INNER JOIN ")
            strSql.AppendLine("         Mov_Destinazioni ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod AND ")
            strSql.AppendLine("         Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND ")
            strSql.AppendLine("         Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det INNER JOIN ")
            strSql.AppendLine("         Movimenti ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND Movimenti_dettagli.Sa_Cod = Movimenti.Sa_Cod AND ")
            strSql.AppendLine("         Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov INNER JOIN ")
            strSql.AppendLine("         Imprese_Progetti ON Mov_Destinazioni.Piva = Imprese_Progetti.Piva AND Mov_Destinazioni.Sa_Cod = Imprese_Progetti.Sa_Cod AND ")
            strSql.AppendLine("         Mov_Destinazioni.Appezza = Imprese_Progetti.Appezza AND Mov_Destinazioni.Id_Destinazione = Imprese_Progetti.Id_Reg ")
            strSql.AppendLine(" WHERE(Movimenti_dettagli.Elem_Cod = 191) ")
            strSql.AppendLine("         AND (Movimenti_dettagli.Mat_Cod = 0) ")

            strSql.AppendLine(" AND   Movimenti_dettagli.Pro_Cod =" & frCod)


            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Movimenti_dettagli.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Movimenti_dettagli.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Movimenti_dettagli.id_agenda Asc ")
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


    Public Function UltimoTrattamentoImpianti(ByVal frCod As Integer,
                                              ByVal dataOperazione As Date,
                                              ByVal xFiltroAggiuntivo As String,
                                              ByVal xOrderBy As String,
                                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                              ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Dettagli_R.UltimoTrattamentoImpianti()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            '------------------------------------------------------------------
            strSql.Length = 0

            'Query per il prelievo dei dati                 ' #### CLASSE ####


            strSql.AppendLine(" SELECT      Mov_Destinazioni.id_agenda, Mov_Destinazioni.Piva, Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.Appezza,  ")
            strSql.AppendLine("             Mov_Destinazioni.Id_Destinazione, Movimenti.Data_Movimento, Imprese_Progetti.Validita_Inizio, Imprese_Progetti.Validita_Fine ")
            strSql.AppendLine(" FROM         Movimenti_dettagli INNER JOIN ")
            strSql.AppendLine("         Mov_Destinazioni ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod AND ")
            strSql.AppendLine("         Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND ")
            strSql.AppendLine("         Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det INNER JOIN ")
            strSql.AppendLine("         Movimenti ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND Movimenti_dettagli.Sa_Cod = Movimenti.Sa_Cod AND ")
            strSql.AppendLine("         Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov INNER JOIN ")
            strSql.AppendLine("         Imprese_Progetti ON Mov_Destinazioni.Piva = Imprese_Progetti.Piva AND Mov_Destinazioni.Sa_Cod = Imprese_Progetti.Sa_Cod AND ")
            strSql.AppendLine("         Mov_Destinazioni.Appezza = Imprese_Progetti.Appezza AND Mov_Destinazioni.Id_Destinazione = Imprese_Progetti.Id_Reg ")
            strSql.AppendLine(" WHERE(Movimenti_dettagli.Elem_Cod = 191) ")
            strSql.AppendLine("         AND (Movimenti_dettagli.Mat_Cod = 0) ")

            strSql.AppendLine(" AND   Movimenti_dettagli.Pro_Cod = " & frCod)

            strSql.AppendLine(" AND   Movimenti.Data_Movimento <=" & Agro_SQL_SaveDate(dataOperazione) & " ")


            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Movimenti_dettagli.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Movimenti_dettagli.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Movimenti_dettagli.id_agenda Asc ")
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

    Public Function UltimoTrattamentoImpianti_conFrCod(ByVal Piva As String,
                                                                  ByVal Id_Agenda As Integer, ByVal IDTestataTemp As Integer,
                                                                  ByVal frCod As Integer, ByVal dataOperazione As Date,
                                                                      ByVal xFiltroAggiuntivo As String, ByVal xOrderBy As String,
                                                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                      ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Dettagli_R.UltimoTrattamentoImpianti_conFrCod()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            '------------------------------------------------------------------
            strSql.Length = 0

            strSql.Append(" SELECT Appezzamento.app_nome, imprese_progetti.Piva, imprese_progetti.sa_cod,imprese_progetti.appezza,imprese_progetti.id_reg, imprese_progetti.progetto_cod, ")
            strSql.Append("         max (Movimenti.Data_Movimento) as Data_Movimento ")

            strSql.Append(" FROM Mov_Destinazioni  " & vbCrLf)

            strSql.Append(" INNER JOIN Appezzamento  " & vbCrLf)
            strSql.Append(" ON Appezzamento.Piva = Mov_Destinazioni.Piva AND Appezzamento.Sa_Cod = Mov_Destinazioni.Sa_Cod " & vbCrLf)
            strSql.Append(" AND Appezzamento.Appezza = Mov_Destinazioni.Appezza   " & vbCrLf)

            strSql.Append(" INNER JOIN Imprese_Progetti  " & vbCrLf)
            strSql.Append(" ON Imprese_Progetti.Piva = Mov_Destinazioni.Piva AND Imprese_Progetti.Sa_Cod = Mov_Destinazioni.Sa_Cod " & vbCrLf)
            strSql.Append(" AND Imprese_Progetti.Appezza = Mov_Destinazioni.Appezza AND Imprese_Progetti.Id_Reg = Mov_Destinazioni.Id_destinazione  " & vbCrLf)

            strSql.Append(" INNER JOIN Movimenti_dettagli " & vbCrLf)
            strSql.Append(" ON Mov_Destinazioni.Piva = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Sa_Cod = Movimenti_dettagli.Sa_Cod  " & vbCrLf)
            strSql.Append(" AND Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det " & vbCrLf)

            strSql.Append(" INNER JOIN Movimenti " & vbCrLf)
            strSql.Append(" ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod   " & vbCrLf)
            strSql.Append(" AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov " & vbCrLf)

            If Id_Agenda <> 0 Then

                strSql.Append("         inner Join ( " & vbCrLf)
                strSql.Append("             select distinct ip.Piva, ip.Sa_Cod, ip.Appezza, ip.Id_reg , ip.Progetto_Cod,  ip.validita_inizio, ip.validita_fine  " & vbCrLf)
                strSql.Append("             from mov_destinazioni d " & vbCrLf)
                strSql.Append("             inner join movimenti m on m.piva=d.piva AND m.id_agenda=d.id_agenda AND m.Id_Mov=d.id_mov " & vbCrLf)
                strSql.Append("             inner join imprese_progetti ip on d.piva=ip.Piva AND ip.Sa_Cod=d.Sa_Cod AND ip.Appezza=d.Appezza AND ip.Id_Reg=d.Id_Destinazione " & vbCrLf)
                strSql.Append("             where d.id_Agenda=" & Agro_SQL_SaveNum(Id_Agenda) & " " & vbCrLf)
                strSql.Append("             AND   d.Piva ='" & Agro_SQL_SaveText(Piva) & "' ")
                strSql.Append("             AND Tipo_Destinazione=0 " & vbCrLf)
                strSql.Append("             AND m.Data_Movimento >= ip.Validita_Inizio " & vbCrLf)
                strSql.Append("             AND m.Data_Movimento <= ip.Validita_fine " & vbCrLf)
                strSql.Append("         ) destinazioni_operazione   " & vbCrLf)
                strSql.Append("         On Imprese_Progetti.piva= destinazioni_operazione.piva  " & vbCrLf)
                strSql.Append("         AND Imprese_Progetti.Sa_Cod=destinazioni_operazione.Sa_Cod  " & vbCrLf)
                strSql.Append("         AND Imprese_Progetti.Appezza=destinazioni_operazione.Appezza  " & vbCrLf)
                strSql.Append("         AND Imprese_Progetti.Id_Reg=destinazioni_operazione.id_reg " & vbCrLf)
                strSql.Append("         AND Imprese_Progetti.progetto_cod=destinazioni_operazione.Progetto_Cod " & vbCrLf)
                strSql.Append("         AND Movimenti.Data_Movimento >= destinazioni_operazione.Validita_Inizio " & vbCrLf)
                strSql.Append("         AND Movimenti.Data_Movimento <= destinazioni_operazione.Validita_fine " & vbCrLf)

            Else

                strSql.Append("         inner Join __Tmp_Movimenti_Destinazioni_DateDistinta ")
                strSql.Append("                 On Imprese_Progetti.piva=__Tmp_Movimenti_Destinazioni_DateDistinta.piva   ")
                strSql.Append("                 AND Imprese_Progetti.Sa_Cod=__Tmp_Movimenti_Destinazioni_DateDistinta.Sa_Cod   ")
                strSql.Append("                 AND Imprese_Progetti.Appezza=__Tmp_Movimenti_Destinazioni_DateDistinta.Appezza   ")
                strSql.Append("                 AND Imprese_Progetti.Id_Reg=__Tmp_Movimenti_Destinazioni_DateDistinta.id_reg  ")
                strSql.Append("                 AND Imprese_Progetti.progetto_cod=__Tmp_Movimenti_Destinazioni_DateDistinta.Progetto_Cod  ")
                strSql.Append("                 AND Movimenti.Data_Movimento >= __Tmp_Movimenti_Destinazioni_DateDistinta.Validita_Inizio  ")
                strSql.Append("                 AND Movimenti.Data_Movimento <= __Tmp_Movimenti_Destinazioni_DateDistinta.Validita_fine  ")
                strSql.Append("         	    AND __Tmp_Movimenti_Destinazioni_DateDistinta.idTestataTemp = " & Agro_SQL_SaveNum(IDTestataTemp) & " ")

            End If

            strSql.Append(" WHERE (Movimenti_dettagli.Elem_Cod = 191) ")
            strSql.Append(" AND (Movimenti_dettagli.Mat_Cod = 0) ")
            strSql.Append(" AND   Movimenti_dettagli.Pro_Cod = " & frCod)

            strSql.Append(" AND   Movimenti.Data_Movimento <=" & Agro_SQL_SaveDate(dataOperazione) & " ")
            strSql.Append(" AND   Movimenti.Piva ='" & Agro_SQL_SaveText(Piva) & "' ")


            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.Append(" AND   Movimenti_dettagli.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.Append(" AND   Movimenti_dettagli.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            strSql.Append(" GROUP BY Appezzamento.app_nome, imprese_progetti.Piva, imprese_progetti.Sa_Cod, imprese_progetti.Appezza, imprese_progetti.Id_reg, imprese_progetti.progetto_cod ")


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

    Public Function NumeroTrattamentiImpianti_conFrCod(ByVal Piva As String,
                                                                  ByVal Id_Agenda As Integer, ByVal IDTestataTemp As Integer,
                                                                  ByVal frCod As Integer,
                                                                  ByVal xFiltroAggiuntivo As String, ByVal xOrderBy As String,
                                                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                  ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Dettagli_R.NumeroTrattamentiImpianti_conFrCod()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Append(" Select Appezzamento.app_nome, imprese_progetti.Piva, imprese_progetti.sa_cod,imprese_progetti.appezza,imprese_progetti.id_reg, imprese_progetti.progetto_cod,  " & vbCrLf)
            strSql.Append(" count(Mov_Destinazioni.id_agenda) as N_Operazioni ")

            strSql.Append(" FROM Mov_Destinazioni  " & vbCrLf)

            strSql.Append(" INNER JOIN Appezzamento  " & vbCrLf)
            strSql.Append(" ON Appezzamento.Piva = Mov_Destinazioni.Piva AND Appezzamento.Sa_Cod = Mov_Destinazioni.Sa_Cod " & vbCrLf)
            strSql.Append(" AND Appezzamento.Appezza = Mov_Destinazioni.Appezza   " & vbCrLf)

            strSql.Append(" INNER JOIN Imprese_Progetti  " & vbCrLf)
            strSql.Append(" ON Imprese_Progetti.Piva = Mov_Destinazioni.Piva AND Imprese_Progetti.Sa_Cod = Mov_Destinazioni.Sa_Cod " & vbCrLf)
            strSql.Append(" AND Imprese_Progetti.Appezza = Mov_Destinazioni.Appezza AND Imprese_Progetti.Id_Reg = Mov_Destinazioni.Id_destinazione  " & vbCrLf)

            strSql.Append(" INNER JOIN Movimenti_dettagli " & vbCrLf)
            strSql.Append(" ON Mov_Destinazioni.Piva = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Sa_Cod = Movimenti_dettagli.Sa_Cod  " & vbCrLf)
            strSql.Append(" AND Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det " & vbCrLf)

            strSql.Append(" INNER JOIN Movimenti " & vbCrLf)
            strSql.Append(" ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod   " & vbCrLf)
            strSql.Append(" AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov " & vbCrLf)

            If Id_Agenda <> 0 Then

                strSql.Append("         inner Join ( " & vbCrLf)
                strSql.Append("             select distinct ip.Piva, ip.Sa_Cod, ip.Appezza, ip.Id_reg , ip.Progetto_Cod,  ip.validita_inizio, ip.validita_fine  " & vbCrLf)
                strSql.Append("             from mov_destinazioni d " & vbCrLf)
                strSql.Append("             inner join movimenti m on m.piva=d.piva AND m.id_agenda=d.id_agenda AND m.Id_Mov=d.id_mov " & vbCrLf)
                strSql.Append("             inner join imprese_progetti ip on d.piva=ip.Piva AND ip.Sa_Cod=d.Sa_Cod AND ip.Appezza=d.Appezza AND ip.Id_Reg=d.Id_Destinazione " & vbCrLf)
                strSql.Append("             where d.id_Agenda=" & Agro_SQL_SaveNum(Id_Agenda) & " " & vbCrLf)
                strSql.Append("             AND   d.Piva ='" & Agro_SQL_SaveText(Piva) & "' ")
                strSql.Append("             AND Tipo_Destinazione=0 " & vbCrLf)
                strSql.Append("             AND m.Data_Movimento >= ip.Validita_Inizio " & vbCrLf)
                strSql.Append("             AND m.Data_Movimento <= ip.Validita_fine " & vbCrLf)
                strSql.Append("         ) destinazioni_operazione   " & vbCrLf)
                strSql.Append("         On Imprese_Progetti.piva= destinazioni_operazione.piva  " & vbCrLf)
                strSql.Append("         AND Imprese_Progetti.Sa_Cod=destinazioni_operazione.Sa_Cod  " & vbCrLf)
                strSql.Append("         AND Imprese_Progetti.Appezza=destinazioni_operazione.Appezza  " & vbCrLf)
                strSql.Append("         AND Imprese_Progetti.Id_Reg=destinazioni_operazione.id_reg " & vbCrLf)
                strSql.Append("         AND Imprese_Progetti.progetto_cod=destinazioni_operazione.Progetto_Cod " & vbCrLf)
                strSql.Append("         AND Movimenti.Data_Movimento >= destinazioni_operazione.Validita_Inizio " & vbCrLf)
                strSql.Append("         AND Movimenti.Data_Movimento <= destinazioni_operazione.Validita_fine " & vbCrLf)

            Else

                strSql.Append("         inner Join __Tmp_Movimenti_Destinazioni_DateDistinta ")
                strSql.Append("                 On Imprese_Progetti.piva=__Tmp_Movimenti_Destinazioni_DateDistinta.piva   ")
                strSql.Append("                 AND Imprese_Progetti.Sa_Cod=__Tmp_Movimenti_Destinazioni_DateDistinta.Sa_Cod   ")
                strSql.Append("                 AND Imprese_Progetti.Appezza=__Tmp_Movimenti_Destinazioni_DateDistinta.Appezza   ")
                strSql.Append("                 AND Imprese_Progetti.Id_Reg=__Tmp_Movimenti_Destinazioni_DateDistinta.id_reg  ")
                strSql.Append("                 AND Imprese_Progetti.progetto_cod=__Tmp_Movimenti_Destinazioni_DateDistinta.Progetto_Cod  ")
                strSql.Append("                 AND Movimenti.Data_Movimento >= __Tmp_Movimenti_Destinazioni_DateDistinta.Validita_Inizio  ")
                strSql.Append("                 AND Movimenti.Data_Movimento <= __Tmp_Movimenti_Destinazioni_DateDistinta.Validita_fine  ")
                strSql.Append("         	    AND __Tmp_Movimenti_Destinazioni_DateDistinta.idTestataTemp = " & Agro_SQL_SaveNum(IDTestataTemp) & " ")

            End If

            strSql.Append(" WHERE (Mov_Destinazioni.Tipo_Destinazione = 0) ")
            strSql.Append(" AND (Movimenti_dettagli.Elem_Cod = 191) ")
            strSql.Append(" AND (Movimenti.Cau_Mov = '2050') ")
            strSql.Append(" AND   Movimenti_dettagli.Pro_Cod = " & frCod)
            strSql.Append(" AND   Movimenti.Piva ='" & Agro_SQL_SaveText(Piva) & "' ")


            'filtro aggiuntivo
            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            strSql.Append(" GROUP BY Appezzamento.app_nome, imprese_progetti.Piva, imprese_progetti.Sa_Cod, imprese_progetti.Appezza, imprese_progetti.Id_reg, imprese_progetti.progetto_cod ")

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

    Public Function NumeroProdottiImpianti_suAvversita(ByVal Piva As String,
                                                        ByVal Id_Agenda As Integer, ByVal IDTestataTemp As Integer,
                                                        ByVal strAvversita As String, ByVal strGruppiAvversita As String,
                                                        ByVal xFiltroAggiuntivo As String, ByVal xOrderBy As String,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                        ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Dettagli_R.NumeroProdottiImpianti_suAvversita()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Append(" Select Appezzamento.app_nome, imprese_progetti.Piva, imprese_progetti.sa_cod,imprese_progetti.appezza,imprese_progetti.id_reg, imprese_progetti.progetto_cod,  " & vbCrLf)
            strSql.Append(" count(movimenti_dettagli.Id_Mov_Det) as N_Prodotti ")

            strSql.Append(" FROM Mov_Destinazioni  " & vbCrLf)

            strSql.Append(" INNER JOIN Appezzamento  " & vbCrLf)
            strSql.Append(" ON Appezzamento.Piva = Mov_Destinazioni.Piva AND Appezzamento.Sa_Cod = Mov_Destinazioni.Sa_Cod " & vbCrLf)
            strSql.Append(" AND Appezzamento.Appezza = Mov_Destinazioni.Appezza   " & vbCrLf)

            strSql.Append(" INNER JOIN Imprese_Progetti  " & vbCrLf)
            strSql.Append(" ON Imprese_Progetti.Piva = Mov_Destinazioni.Piva AND Imprese_Progetti.Sa_Cod = Mov_Destinazioni.Sa_Cod " & vbCrLf)
            strSql.Append(" AND Imprese_Progetti.Appezza = Mov_Destinazioni.Appezza AND Imprese_Progetti.Id_Reg = Mov_Destinazioni.Id_destinazione  " & vbCrLf)

            strSql.Append(" INNER JOIN Movimenti_dettagli " & vbCrLf)
            strSql.Append(" ON Mov_Destinazioni.Piva = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Sa_Cod = Movimenti_dettagli.Sa_Cod  " & vbCrLf)
            strSql.Append(" AND Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det " & vbCrLf)

            strSql.Append(" INNER JOIN Movimenti " & vbCrLf)
            strSql.Append(" ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod   " & vbCrLf)
            strSql.Append(" AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov " & vbCrLf)

            strSql.Append(" INNER JOIN Mov_Dettaglio_Tecnico  " & vbCrLf)
            strSql.Append(" ON Mov_Dettaglio_Tecnico.Piva = Movimenti_dettagli.PIVA AND Mov_Dettaglio_Tecnico.Sa_Cod = Movimenti_dettagli.Sa_Cod AND Mov_Dettaglio_Tecnico.Id_Agenda = Movimenti_dettagli.Id_Agenda AND " & vbCrLf)
            strSql.Append(" Mov_Dettaglio_Tecnico.Id_Mov = Movimenti_dettagli.Id_Mov AND Mov_Dettaglio_Tecnico.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det " & vbCrLf)

            If Id_Agenda <> 0 Then

                strSql.Append("         inner Join ( " & vbCrLf)
                strSql.Append("             select distinct ip.Piva, ip.Sa_Cod, ip.Appezza, ip.Id_reg , ip.Progetto_Cod,  ip.validita_inizio, ip.validita_fine  " & vbCrLf)
                strSql.Append("             from mov_destinazioni d " & vbCrLf)
                strSql.Append("             inner join movimenti m on m.piva=d.piva AND m.id_agenda=d.id_agenda AND m.Id_Mov=d.id_mov " & vbCrLf)
                strSql.Append("             inner join imprese_progetti ip on d.piva=ip.Piva AND ip.Sa_Cod=d.Sa_Cod AND ip.Appezza=d.Appezza AND ip.Id_Reg=d.Id_Destinazione " & vbCrLf)
                strSql.Append("             where d.id_Agenda=" & Agro_SQL_SaveNum(Id_Agenda) & " " & vbCrLf)
                strSql.Append("             AND   d.Piva ='" & Agro_SQL_SaveText(Piva) & "' ")
                strSql.Append("             AND Tipo_Destinazione=0 " & vbCrLf)
                strSql.Append("             AND m.Data_Movimento >= ip.Validita_Inizio " & vbCrLf)
                strSql.Append("             AND m.Data_Movimento <= ip.Validita_fine " & vbCrLf)
                strSql.Append("         ) destinazioni_operazione   " & vbCrLf)
                strSql.Append("         On Imprese_Progetti.piva= destinazioni_operazione.piva  " & vbCrLf)
                strSql.Append("         AND Imprese_Progetti.Sa_Cod=destinazioni_operazione.Sa_Cod  " & vbCrLf)
                strSql.Append("         AND Imprese_Progetti.Appezza=destinazioni_operazione.Appezza  " & vbCrLf)
                strSql.Append("         AND Imprese_Progetti.Id_Reg=destinazioni_operazione.id_reg " & vbCrLf)
                strSql.Append("         AND Imprese_Progetti.progetto_cod=destinazioni_operazione.Progetto_Cod " & vbCrLf)
                strSql.Append("         AND Movimenti.Data_Movimento >= destinazioni_operazione.Validita_Inizio " & vbCrLf)
                strSql.Append("         AND Movimenti.Data_Movimento <= destinazioni_operazione.Validita_fine " & vbCrLf)

            Else

                strSql.Append("         inner Join __Tmp_Movimenti_Destinazioni_DateDistinta ")
                strSql.Append("                 On Imprese_Progetti.piva=__Tmp_Movimenti_Destinazioni_DateDistinta.piva   ")
                strSql.Append("                 AND Imprese_Progetti.Sa_Cod=__Tmp_Movimenti_Destinazioni_DateDistinta.Sa_Cod   ")
                strSql.Append("                 AND Imprese_Progetti.Appezza=__Tmp_Movimenti_Destinazioni_DateDistinta.Appezza   ")
                strSql.Append("                 AND Imprese_Progetti.Id_Reg=__Tmp_Movimenti_Destinazioni_DateDistinta.id_reg  ")
                strSql.Append("                 AND Imprese_Progetti.progetto_cod=__Tmp_Movimenti_Destinazioni_DateDistinta.Progetto_Cod  ")
                strSql.Append("                 AND Movimenti.Data_Movimento >= __Tmp_Movimenti_Destinazioni_DateDistinta.Validita_Inizio  ")
                strSql.Append("                 AND Movimenti.Data_Movimento <= __Tmp_Movimenti_Destinazioni_DateDistinta.Validita_fine  ")
                strSql.Append("         	    AND __Tmp_Movimenti_Destinazioni_DateDistinta.idTestataTemp = " & Agro_SQL_SaveNum(IDTestataTemp) & " ")

            End If

            strSql.Append(" WHERE (Mov_Destinazioni.Tipo_Destinazione = 0) ")
            strSql.Append(" AND (Movimenti_dettagli.Elem_Cod = 191) ")
            strSql.Append(" AND (Movimenti.Cau_Mov = '2050') ")
            strSql.Append(" AND   Movimenti.Piva ='" & Agro_SQL_SaveText(Piva) & "' ")

            If strAvversita <> "" Then
                strSql.Append(" AND Mov_Dettaglio_Tecnico.Av_Cod IN " & Agro_SQL_Save_Clausola_IN(strAvversita, False, False) & "  ")
            End If
            If strGruppiAvversita <> "" Then
                strSql.Append(" AND Mov_Dettaglio_Tecnico.Av_Gru IN " & Agro_SQL_Save_Clausola_IN(strGruppiAvversita, False, False) & "  ")
            End If

            'filtro aggiuntivo
            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            strSql.Append(" GROUP BY Appezzamento.app_nome, imprese_progetti.Piva, imprese_progetti.Sa_Cod, imprese_progetti.Appezza, imprese_progetti.Id_reg, imprese_progetti.progetto_cod ")


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

    Public Function EsistonoOperazioniPianificate(ByVal Piva As String,
                                                  ByVal Sa_Cod As Integer,
                                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean


        Dim objContab As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
        Dim dtContab As DataTable

        'Leggo le lavorazioni eseguite nella data corrente
        dtContab = objContab.LeggiPianificazioni(CStr(Piva),
                                                 CInt(Sa_Cod),
                                                 0, 0, 0, -1,
                                                 "", "", objParametri)

        objContab = Nothing

        If dtContab IsNot Nothing AndAlso dtContab.Rows.Count > 0 Then

            dtContab = Nothing
            Return True

        Else

            dtContab = Nothing
            Return False

        End If

    End Function



    '##############################################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="PIVA"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="Id_Agenda"></param>
    ''' <param name="Id_Mov"></param>
    ''' <param name="Id_Mov_Det"></param>
    ''' <param name="Contabilizzato"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	15/03/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function LeggiPianificazioni(ByVal PIVA As String,
                                        ByVal Sa_Cod As Integer,
                                        ByVal Id_Agenda As Integer,
                                        ByVal Id_Mov As Integer,
                                        ByVal Id_Mov_Det As Integer,
                                        ByVal Contabilizzato As Integer,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Dettagli_R.LeggiPianificazioni()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable
        Dim strGroup As String

        Try

            '------------------------------------------------------------------
            strGroup = "Agenda.Piva, Agenda.Sa_Cod, Agenda.Id_Agenda, Agenda.Lav_Cod, Agenda.Des_Lib, Agenda.Username_Creazione, Movimenti.Data_Movimento"

            strSql.Length = 0

            strSql.AppendLine(" SELECT Distinct " & strGroup)
            strSql.AppendLine(" FROM  Agenda , Movimenti , Movimenti_Dettagli , Mov_Destinazioni ")
            strSql.AppendLine(" WHERE Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            strSql.AppendLine(" AND   Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))

            'Join sulla Piva
            strSql.AppendLine(" AND   Agenda.Piva = Movimenti.Piva ")
            strSql.AppendLine(" AND   Agenda.Piva = Movimenti_Dettagli.Piva ")
            strSql.AppendLine(" AND   Agenda.Piva = Mov_Destinazioni.Piva ")
            strSql.AppendLine(" AND   Movimenti.Piva = Movimenti_Dettagli.Piva ")
            strSql.AppendLine(" AND   Movimenti.Piva = Mov_Destinazioni.Piva ")
            strSql.AppendLine(" AND   Movimenti_Dettagli.Piva = Mov_Destinazioni.Piva ")

            'Join sul Sa_Cod
            strSql.AppendLine(" AND   Agenda.Sa_Cod = Movimenti.Sa_Cod ")
            strSql.AppendLine(" AND   Movimenti_Dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod ")

            'Join sul Id_Agenda
            strSql.AppendLine(" AND   Agenda.Id_Agenda = Movimenti.Id_Agenda ")
            strSql.AppendLine(" AND   Agenda.Id_Agenda = Movimenti_Dettagli.Id_Agenda ")
            strSql.AppendLine(" AND   Agenda.Id_Agenda = Mov_Destinazioni.Id_Agenda ")
            strSql.AppendLine(" AND   Movimenti.Id_Agenda = Movimenti_Dettagli.Id_Agenda ")
            strSql.AppendLine(" AND   Movimenti.Id_Agenda = Mov_Destinazioni.Id_Agenda ")
            strSql.AppendLine(" AND   Movimenti_Dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda ")


            'Join su Id_Mov
            strSql.AppendLine(" AND   Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov ")
            strSql.AppendLine(" AND   Movimenti.Id_Mov = Mov_Destinazioni.Id_Mov ")
            strSql.AppendLine(" AND   Movimenti_Dettagli.Id_Mov = Mov_Destinazioni.Id_Mov ")

            'Join si Id_Mov_Det
            strSql.AppendLine(" AND   Movimenti_Dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")

            If PIVA <> "" Then
                strSql.AppendLine(" AND Movimenti_Dettagli.Piva = '" & Agro_SQL_SaveText(PIVA) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND Movimenti_Dettagli.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Id_Agenda <> 0 Then
                strSql.AppendLine(" AND Movimenti_Dettagli.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
            End If

            If Id_Mov <> 0 Then
                strSql.AppendLine(" AND Movimenti_Dettagli.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
            End If

            If Id_Mov_Det <> 0 Then
                strSql.AppendLine(" AND Movimenti_Dettagli.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")
            End If

            If Contabilizzato <> 0 Then
                strSql.AppendLine(" AND Movimenti_Dettagli.Contabilizzato = " & Agro_SQL_SaveNum(Contabilizzato) & "   ")
            Else
                'Tutte le Pianificazioni
                strSql.AppendLine(" AND Movimenti_Dettagli.Contabilizzato < 0 ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Agenda.Inviato >=0 ")
                    strSql.AppendLine(" AND   Movimenti.Inviato >=0 ")
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Inviato >=0 ")
                    strSql.AppendLine(" AND   Mov_Destinazioni.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Agenda.Inviato =-1 ")
                    strSql.AppendLine(" AND   Movimenti.Inviato =-1 ")
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Inviato =-1 ")
                    strSql.AppendLine(" AND   Mov_Destinazioni.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            strSql.AppendLine(" Group BY " & strGroup)


            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Movimenti.Data_Movimento Asc ")
            End If


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function




    '##############################################################################################
    Public Function Verifica_Giacenze(ByVal Piva As String,
                                      ByVal Sa_Cod As Integer,
                                      ByVal Id_Destinazione As Integer,
                                      ByVal Elem_Cod As Integer,
                                      ByVal Pro_Cod As Integer,
                                      ByVal Mat_Cod As Integer,
                                      ByVal Cod_Progetto As Integer,
                                      ByVal Fase_Cod As Integer,
                                      ByVal Lotto As String,
                                      ByVal Cal_Cod As Integer,
                                      ByVal Udm_Cod As Integer,
                                      ByVal Dal_Data_Verifica As Date,
                                      ByVal Al_Data_Verifica As Date,
                                      ByVal Id_Agenda_daNon_considerare As Integer,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                      ) As Decimal

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Dettagli_R.Verifica_Giacenze()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            '------------------------------------------------------------------
            strSql.Length = 0

            'Query per il prelievo dei dati                 ' #### CLASSE ####

            strSql.AppendLine(" SELECT ( ")

            strSql.AppendLine("           SELECT ISNULL(SUM( round( Int_Mov_Dest.Qta , +4)),0) as Qta_Totale_Plus  ")

            strSql.AppendLine("           FROM    Agenda Int_Ag ")
            strSql.AppendLine("           INNER JOIN Movimenti Int_Mov ON Int_Ag.PIVA = Int_Mov.PIVA AND Int_Ag.Id_Agenda = Int_Mov.Id_Agenda ")
            strSql.AppendLine("           INNER JOIN Movimenti_dettagli Int_Mov_Dett ON Int_Mov.PIVA = Int_Mov_Dett.PIVA AND Int_Mov.Id_Agenda = Int_Mov_Dett.Id_Agenda AND Int_Mov.Id_Mov = Int_Mov_Dett.Id_Mov  ")
            strSql.AppendLine("           INNER JOIN Mov_Destinazioni Int_Mov_Dest ON Int_Mov_Dett.PIVA = Int_Mov_Dest.Piva AND Int_Mov_Dett.Id_Agenda = Int_Mov_Dest.Id_Agenda AND Int_Mov_Dett.Id_Mov = Int_Mov_Dest.Id_Mov AND Int_Mov_Dett.Id_Mov_Det = Int_Mov_Dest.Id_Mov_Det ")

            strSql.AppendLine("           WHERE   Int_Mov_Dett.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "   ")
            strSql.AppendLine("           AND     Int_Mov_Dett.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "   ")
            strSql.AppendLine("           AND     Int_Mov_Dett.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   ")
            strSql.AppendLine("           AND     Int_Mov_Dett.Contabilizzato >= 0  ")
            strSql.AppendLine("           AND     Int_Mov_Dest.Tipo_Destinazione = " & CStr(MAGAZZINO) & "    ")
            strSql.AppendLine("           AND     Int_Mov.Cau_Mov IN ( '" & CAU_CARICO & "', '" & CAU_CONFERIMENTO & "' )  ")
            strSql.AppendLine("           AND     Int_Mov.Data_Movimento >= " & Agro_SQL_SaveDate(Dal_Data_Verifica) & "   ")
            strSql.AppendLine("           AND     Int_Mov.Data_Movimento <= " & Agro_SQL_SaveDate(Al_Data_Verifica) & "   ")

            If Piva <> "" Then
                strSql.AppendLine(" AND Int_Mov_Dest.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND Int_Mov_Dest.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Id_Destinazione <> 0 Then
                strSql.AppendLine(" AND Int_Mov_Dest.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "   ")
            End If

            If Elem_Cod <> 0 Then
                strSql.AppendLine(" AND Int_Mov_Dett.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
            End If

            If Pro_Cod <> 0 Then
                strSql.AppendLine(" AND Int_Mov_Dett.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
            End If

            If Mat_Cod <> 0 Then
                strSql.AppendLine(" AND Int_Mov_Dett.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
            End If

            If Cod_Progetto <> 0 Then
                strSql.AppendLine(" AND Int_Mov_Dett.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & "   ")
            End If

            If Fase_Cod <> 0 Then
                strSql.AppendLine(" AND Int_Mov_Dett.Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   ")
            End If

            If Lotto <> LOTTO_NONDEFINITO Then
                strSql.AppendLine(" AND Int_Mov_Dett.Lotto = '" & Agro_SQL_SaveText(Lotto) & "'   ")
            End If

            If Cal_Cod <> 0 Then
                strSql.AppendLine(" AND Int_Mov_Dett.Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "   ")
            End If

            If Udm_Cod <> 0 Then
                strSql.AppendLine(" AND Int_Mov_Dett.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   ")
            End If

            If Id_Agenda_daNon_considerare <> 0 Then
                strSql.AppendLine(" AND Int_Mov_Dett.Id_Agenda <> " & Agro_SQL_SaveNum(Id_Agenda_daNon_considerare) & "   ")
            End If


            strSql.AppendLine(" )")
            strSql.AppendLine(" + ")
            strSql.AppendLine(" ( ")

            strSql.AppendLine(" SELECT ISNULL(-SUM(round( Int_Mov_Dest.Qta , +4)),0) as Qta_Totale_Minus  ")

            strSql.AppendLine("           FROM    Agenda Int_Ag ")
            strSql.AppendLine("           INNER JOIN Movimenti Int_Mov ON Int_Ag.PIVA = Int_Mov.PIVA AND Int_Ag.Id_Agenda = Int_Mov.Id_Agenda ")
            strSql.AppendLine("           INNER JOIN Movimenti_dettagli Int_Mov_Dett ON Int_Mov.PIVA = Int_Mov_Dett.PIVA AND Int_Mov.Id_Agenda = Int_Mov_Dett.Id_Agenda AND Int_Mov.Id_Mov = Int_Mov_Dett.Id_Mov  ")
            strSql.AppendLine("           INNER JOIN Mov_Destinazioni Int_Mov_Dest ON Int_Mov_Dett.PIVA = Int_Mov_Dest.Piva AND Int_Mov_Dett.Id_Agenda = Int_Mov_Dest.Id_Agenda AND Int_Mov_Dett.Id_Mov = Int_Mov_Dest.Id_Mov AND Int_Mov_Dett.Id_Mov_Det = Int_Mov_Dest.Id_Mov_Det ")

            strSql.AppendLine("           WHERE   Int_Mov_Dett.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "   ")
            strSql.AppendLine("           AND     Int_Mov_Dett.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "   ")
            strSql.AppendLine("           AND     Int_Mov_Dett.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   ")
            strSql.AppendLine("           AND     Int_Mov_Dett.Contabilizzato >= 0  ")
            strSql.AppendLine("           AND     Int_Mov_Dest.Tipo_Destinazione = " & CStr(MAGAZZINO) & "    ")
            strSql.AppendLine("           AND     Int_Mov.Cau_Mov IN ( '" & CAU_SCARICO & "', '" & CAU_CONFERIMENTO_DIVERSI & "' )  ")
            strSql.AppendLine("           AND     Int_Mov.Data_Movimento >= " & Agro_SQL_SaveDate(Dal_Data_Verifica) & "   ")
            strSql.AppendLine("           AND     Int_Mov.Data_Movimento <= " & Agro_SQL_SaveDate(Al_Data_Verifica) & "   ")

            If Piva <> "" Then
                strSql.AppendLine(" AND Int_Mov_Dest.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND Int_Mov_Dest.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Id_Destinazione <> 0 Then
                strSql.AppendLine(" AND Int_Mov_Dest.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "   ")
            End If

            If Elem_Cod <> 0 Then
                strSql.AppendLine(" AND Int_Mov_Dett.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
            End If

            If Pro_Cod <> 0 Then
                strSql.AppendLine(" AND Int_Mov_Dett.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
            End If

            If Mat_Cod <> 0 Then
                strSql.AppendLine(" AND Int_Mov_Dett.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
            End If

            If Cod_Progetto <> 0 Then
                strSql.AppendLine(" AND Int_Mov_Dett.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & "   ")
            End If

            If Fase_Cod <> 0 Then
                strSql.AppendLine(" AND Int_Mov_Dett.Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   ")
            End If

            If Lotto <> LOTTO_NONDEFINITO Then
                strSql.AppendLine(" AND Int_Mov_Dett.Lotto = '" & Agro_SQL_SaveText(Lotto) & "'   ")
            End If

            If Cal_Cod <> 0 Then
                strSql.AppendLine(" AND Int_Mov_Dett.Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "   ")
            End If

            If Udm_Cod <> 0 Then
                strSql.AppendLine(" AND Int_Mov_Dett.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   ")
            End If

            If Id_Agenda_daNon_considerare <> 0 Then
                strSql.AppendLine(" AND Int_Mov_Dett.Id_Agenda <> " & Agro_SQL_SaveNum(Id_Agenda_daNon_considerare) & "   ")
            End If

            strSql.AppendLine(" ) AS Verifica_Giacenze")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If Not IsNothing(dt) Then
            If dt.Rows.Count <> 0 Then
                Return dt.Rows(0).Item("Verifica_Giacenze")
            Else
                Return 0
            End If
        Else
            Return 0
        End If

    End Function




    '##############################################################################################
    'utilizza l'id_agenda passato per ricavare gli id_agenda rifeririti nel magazzino esterno ed escluderli dalle giacenze,
    'in questo modo in modifica quando si salva non vengono contati quegli scarichi
    Public Function Verifica_Giacenze_Con_Magazzino_Esterno(ByVal Piva_Magazzino As String,
                                                            ByVal Sa_Cod_Magazzino As Integer,
                                                            ByVal Id_Destinazione_Magazzino As Integer,
                                                            ByVal Elem_Cod As Integer,
                                                            ByVal Pro_Cod As Integer,
                                                            ByVal Mat_Cod As Integer,
                                                            ByVal Cod_Progetto As Integer,
                                                            ByVal Fase_Cod As Integer,
                                                            ByVal Lotto As String,
                                                            ByVal Cal_Cod As Integer,
                                                            ByVal Udm_Cod As Integer,
                                                            ByVal Dal_Data_Verifica As Date,
                                                            ByVal Al_Data_Verifica As Date,
                                                            ByVal Piva_Id_Agenda As String,
                                                            ByVal Sa_Cod_Id_Agenda As Integer,
                                                            ByVal Id_Agenda_daNon_considerare As Integer,
                                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                            Optional ByVal ID_Agenda_Scarico As Integer = 0,
                                                            Optional ByVal conRaccoglitore As Boolean = False
                                                            ) As Decimal

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Dettagli_R.Verifica_Giacenze_Con_Magazzino()"

        Dim filtro As String = ""

        'gestisco caso magazzino esterno o no
        If Piva_Magazzino <> Piva_Id_Agenda Then
            Try
                Dim MatriceIdAgendaOld As String(,)
                Dim objRif As New AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R
                MatriceIdAgendaOld = objRif.MatriceChiaviAgenda_MovRiferiti(Piva_Id_Agenda,
                                                                            Sa_Cod_Id_Agenda,
                                                                            Id_Agenda_daNon_considerare,
                                                                            "",
                                                                            objParametri,
                                                                            conRaccoglitore)
                Dim Id_Agende As String = ""
                Dim i As Integer
                For i = 0 To UBound(MatriceIdAgendaOld, 2)
                    If i = 0 Then
                        Id_Agende = UtilityProvider.Agro_SQL_SaveNum(MatriceIdAgendaOld(2, i))
                    Else
                        Id_Agende = Id_Agende & "," & UtilityProvider.Agro_SQL_SaveNum(MatriceIdAgendaOld(2, i))
                    End If

                Next
                If Id_Agende <> "" Then
                    filtro = " AND Int_Mov_Dett.Id_Agenda not in (" & Agro_SQL_Save_Clausola_IN(Id_Agende) & ") "
                ElseIf ID_Agenda_Scarico <> 0 Then
                    filtro = " AND Int_Mov_Dest.Id_Agenda not in (" & Agro_SQL_Save_Clausola_IN(ID_Agenda_Scarico) & ") "
                End If

            Catch ex As Exception

            End Try

        Else
            'magazzino interno, uso id_agenda
            If Id_Agenda_daNon_considerare <> 0 Then
                filtro = " AND Int_Mov_Dett.Id_Agenda <>  " & Agro_SQL_SaveNum(Id_Agenda_daNon_considerare)
                filtro &= " AND ISNULL(Int_Ag.Raccoglitore_Cod, 0) <> ISNULL((select case when ISNULL(Raccoglitore_Cod, 0) > 0 Then Raccoglitore_Cod Else -999 End from agenda where Piva = '" & Agro_SQL_SaveText(Piva_Id_Agenda) & "' AND Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda_daNon_considerare) & "),-999)"
            End If
        End If


        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            '------------------------------------------------------------------
            strSql.Length = 0

            'Query per il prelievo dei dati                 ' #### CLASSE ####
            strSql.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")
            strSql.AppendLine(" SELECT ( ")

            strSql.AppendLine("           SELECT ISNULL(SUM( round( Int_Mov_Dest.Qta , +5)),0) as Qta_Totale_Plus  ")

            strSql.AppendLine("           FROM    Agenda Int_Ag ")
            strSql.AppendLine("           INNER JOIN Movimenti Int_Mov ON Int_Ag.PIVA = Int_Mov.PIVA AND Int_Ag.Id_Agenda = Int_Mov.Id_Agenda ")
            strSql.AppendLine("           INNER JOIN Movimenti_dettagli Int_Mov_Dett ON Int_Mov.PIVA = Int_Mov_Dett.PIVA AND Int_Mov.Id_Agenda = Int_Mov_Dett.Id_Agenda AND Int_Mov.Id_Mov = Int_Mov_Dett.Id_Mov  ")
            strSql.AppendLine("           INNER JOIN Mov_Destinazioni Int_Mov_Dest ON Int_Mov_Dett.PIVA = Int_Mov_Dest.Piva AND Int_Mov_Dett.Id_Agenda = Int_Mov_Dest.Id_Agenda AND Int_Mov_Dett.Id_Mov = Int_Mov_Dest.Id_Mov AND Int_Mov_Dett.Id_Mov_Det = Int_Mov_Dest.Id_Mov_Det ")

            strSql.AppendLine("           WHERE   Int_Mov_Dett.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "   ")
            strSql.AppendLine("           AND     Int_Mov_Dett.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "   ")
            strSql.AppendLine("           AND     Int_Mov_Dett.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   ")
            strSql.AppendLine("           AND     Int_Mov_Dett.Contabilizzato >= 0  ")
            strSql.AppendLine("           AND     Int_Mov_Dest.Tipo_Destinazione = " & CStr(MAGAZZINO) & "    ")
            strSql.AppendLine("           AND     Int_Mov.Cau_Mov IN ( '" & CAU_CARICO & "', '" & CAU_CONFERIMENTO & "' )  ")
            strSql.AppendLine("           AND     Int_Mov.Data_Movimento >= " & Agro_SQL_SaveDate(Dal_Data_Verifica) & "   ")
            strSql.AppendLine("           AND     Int_Mov.Data_Movimento <= " & Agro_SQL_SaveDate(Al_Data_Verifica) & "   ")

            If Piva_Magazzino <> "" Then
                strSql.AppendLine(" AND Int_Mov_Dest.Piva = '" & Agro_SQL_SaveText(Piva_Magazzino) & "' ")
            End If

            If Sa_Cod_Magazzino <> 0 Then
                strSql.AppendLine(" AND Int_Mov_Dest.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod_Magazzino) & "   ")
            End If

            If Id_Destinazione_Magazzino <> 0 Then
                strSql.AppendLine(" AND Int_Mov_Dest.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione_Magazzino) & "   ")
            End If

            If Elem_Cod <> 0 Then
                strSql.AppendLine(" AND Int_Mov_Dett.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
            End If

            If Pro_Cod <> 0 Then
                strSql.AppendLine(" AND Int_Mov_Dett.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
            End If

            If Mat_Cod <> 0 Then
                strSql.AppendLine(" AND Int_Mov_Dett.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
            End If

            If Cod_Progetto <> 0 Then
                strSql.AppendLine(" AND Int_Mov_Dett.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & "   ")
            End If

            If Fase_Cod <> 0 Then
                strSql.AppendLine(" AND Int_Mov_Dett.Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   ")
            End If

            If Lotto <> LOTTO_NONDEFINITO Then
                strSql.AppendLine(" AND Int_Mov_Dett.Lotto = '" & Agro_SQL_SaveText(Lotto) & "'   ")
            End If

            If Cal_Cod <> 0 Then
                strSql.AppendLine(" AND Int_Mov_Dett.Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "   ")
            End If

            If Udm_Cod <> 0 Then
                strSql.AppendLine(" AND Int_Mov_Dett.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   ")
            End If

            If filtro <> "" Then
                strSql.AppendLine(" " & filtro & " ")
            End If


            strSql.AppendLine(" )")
            strSql.AppendLine(" + ")
            strSql.AppendLine(" ( ")

            strSql.AppendLine(" SELECT ISNULL(-SUM(round( Int_Mov_Dest.Qta , +5)),0) as Qta_Totale_Minus  ")

            strSql.AppendLine("           FROM    Agenda Int_Ag ")
            strSql.AppendLine("           INNER JOIN Movimenti Int_Mov ON Int_Ag.PIVA = Int_Mov.PIVA AND Int_Ag.Id_Agenda = Int_Mov.Id_Agenda ")
            strSql.AppendLine("           INNER JOIN Movimenti_dettagli Int_Mov_Dett ON Int_Mov.PIVA = Int_Mov_Dett.PIVA AND Int_Mov.Id_Agenda = Int_Mov_Dett.Id_Agenda AND Int_Mov.Id_Mov = Int_Mov_Dett.Id_Mov  ")
            strSql.AppendLine("           INNER JOIN Mov_Destinazioni Int_Mov_Dest ON Int_Mov_Dett.PIVA = Int_Mov_Dest.Piva AND Int_Mov_Dett.Id_Agenda = Int_Mov_Dest.Id_Agenda AND Int_Mov_Dett.Id_Mov = Int_Mov_Dest.Id_Mov AND Int_Mov_Dett.Id_Mov_Det = Int_Mov_Dest.Id_Mov_Det ")

            strSql.AppendLine("           WHERE   Int_Mov_Dett.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "   ")
            strSql.AppendLine("           AND     Int_Mov_Dett.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "   ")
            strSql.AppendLine("           AND     Int_Mov_Dett.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   ")
            strSql.AppendLine("           AND     Int_Mov_Dett.Contabilizzato >= 0  ")
            strSql.AppendLine("           AND     Int_Mov_Dest.Tipo_Destinazione = " & CStr(MAGAZZINO) & "    ")
            strSql.AppendLine("           AND     Int_Mov.Cau_Mov IN ( '" & CAU_SCARICO & "', '" & CAU_CONFERIMENTO_DIVERSI & "' )  ")
            strSql.AppendLine("           AND     Int_Mov.Data_Movimento >= " & Agro_SQL_SaveDate(Dal_Data_Verifica) & "   ")
            strSql.AppendLine("           AND     Int_Mov.Data_Movimento <= " & Agro_SQL_SaveDate(Al_Data_Verifica) & "   ")

            If Piva_Magazzino <> "" Then
                strSql.AppendLine(" AND Int_Mov_Dest.Piva = '" & Agro_SQL_SaveText(Piva_Magazzino) & "' ")
            End If

            If Sa_Cod_Magazzino <> 0 Then
                strSql.AppendLine(" AND Int_Mov_Dest.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod_Magazzino) & "   ")
            End If

            If Id_Destinazione_Magazzino <> 0 Then
                strSql.AppendLine(" AND Int_Mov_Dest.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione_Magazzino) & "   ")
            End If

            If Elem_Cod <> 0 Then
                strSql.AppendLine(" AND Int_Mov_Dett.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
            End If

            If Pro_Cod <> 0 Then
                strSql.AppendLine(" AND Int_Mov_Dett.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
            End If

            If Mat_Cod <> 0 Then
                strSql.AppendLine(" AND Int_Mov_Dett.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
            End If

            If Cod_Progetto <> 0 Then
                strSql.AppendLine(" AND Int_Mov_Dett.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & "   ")
            End If

            If Fase_Cod <> 0 Then
                strSql.AppendLine(" AND Int_Mov_Dett.Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   ")
            End If

            If Lotto <> LOTTO_NONDEFINITO Then
                strSql.AppendLine(" AND Int_Mov_Dett.Lotto = '" & Agro_SQL_SaveText(Lotto) & "'   ")
            End If

            If Cal_Cod <> 0 Then
                strSql.AppendLine(" AND Int_Mov_Dett.Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "   ")
            End If

            If Udm_Cod <> 0 Then
                strSql.AppendLine(" AND Int_Mov_Dett.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   ")
            End If

            If filtro <> "" Then
                strSql.AppendLine(" " & filtro & " ")
            End If

            strSql.AppendLine(" ) AS Verifica_Giacenze")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & messaggioErrore)
        End Try

        If Not IsNothing(dt) Then
            If dt.Rows.Count <> 0 Then
                Return dt.Rows(0).Item("Verifica_Giacenze")
            Else
                Return 0
            End If
        Else
            Return 0
        End If

    End Function


    Public Function ValorizzazioneProdotto_New(ByVal tipoValorizzazione As Integer,
                                               ByVal Piva As String,
                                               ByVal Elem_Cod As Integer,
                                               ByVal Pro_Cod As Integer,
                                               ByVal Mat_Cod As Integer,
                                               ByVal Udm_Cod As Integer,
                                               ByVal Cal_Cod As Integer,
                                               ByVal Cod_Progetto As Integer,
                                               ByVal Fase_Cod As Integer,
                                               ByVal Lotto As String,
                                               ByVal Dal_Data_Verifica As Date,
                                               ByVal Al_Data_Verifica As Date,
                                               ByVal objParametri As AgronicaCoreParametri
                                               ) As Decimal

        Dim sSql As String

        Dim strJoin As String

        '------------------------------

        'Nome del metodo
        Const nomeRoutine = "ValorizzazioneProdotto_New:"
        Dim messaggioErrore As String


        Select Case tipoValorizzazione

            Case 1, 2

                '##############################################################################################################
                '##########################################  MEDIA PONDERATA  #################################################
                '##############################################################################################################

                'Condizioni Di Join

                'Join sulla Piva
                strJoin = "" &
                       " AND   Agenda.Piva = Movimenti.Piva " &
                       " AND   Movimenti.Piva = Movimenti_Dettagli.Piva " &
                       " AND   Movimenti_Dettagli.Piva = Mov_Destinazioni.Piva "

                'Join su Id_Agenda
                strJoin = strJoin &
                       " AND   Agenda.Id_Agenda = Movimenti.Id_Agenda " &
                       " AND   Movimenti.Id_Agenda = Movimenti_Dettagli.Id_Agenda " &
                       " AND   Movimenti_Dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda "

                'Join su Id_Mov
                strJoin = strJoin &
                       " AND   Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov " &
                       " AND   Movimenti_Dettagli.Id_Mov = Mov_Destinazioni.Id_Mov "

                'Join su Id_Mov_Det
                strJoin = strJoin &
                       " AND   Movimenti_Dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det "

                'Lettura mirata del prodotto

                strJoin = strJoin & " AND Mov_Destinazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "'   "

                strJoin = strJoin & " AND Movimenti_Dettagli.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   "

                strJoin = strJoin & " AND Movimenti_Dettagli.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   "

                strJoin = strJoin & " AND Movimenti_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   "

                strJoin = strJoin & " AND Movimenti_Dettagli.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   "

                strJoin = strJoin & " AND Movimenti_Dettagli.Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "   "

                'Soppresso perché errato
                'StrJoin = StrJoin & " AND Movimenti_Dettagli.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & "   "


                strJoin = strJoin & " AND Movimenti_Dettagli.Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   "

                strJoin = strJoin & " AND Upper(Movimenti_Dettagli.Lotto) = '" & UCase(Agro_SQL_SaveText(Lotto)) & "'   "

                strJoin = strJoin & " AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Dal_Data_Verifica) & "   "

                strJoin = strJoin & " AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Al_Data_Verifica) & "   "

                'La condizione necessaria è che il dettaglio abbia una imputazione economica e una quantità valida
                strJoin = strJoin & " AND Movimenti_Dettagli.Prezzo_Unitario_Netto > 0 AND Movimenti_Dettagli.Qta > 0 "


                '======================================================================================================================================================================
                'COSTO PONDERATO --> Bolle Ricevute, Fatture Ricevute, Acquisto, Carico, Ricevimento Fatture Liquidazione, Emissione AutoFatture Liquidazione, Note di Accredito Emesse,
                '                    Aumento Consistenze Zoo
                '----------------------------------------------------------------------------------------------------------------------------------------------------------------------
                '======================================================================================================================================================================
                'RICAVO PONDERATO --> Bolle Emesse, Fatture Emesse, Vendita, Scarico, Emissione Ricevute Fiscali, Emissione Fatture Liquidazione, Ricevimento AutoFatture Liquidazione,
                '                     Note di Accredito Ricevute, Autoconsumo
                '----------------------------------------------------------------------------------------------------------------------------------------------------------------------
                strJoin = strJoin & " AND Agenda.Lav_Cod IN ( 1025, 1000, 1021, 1022, 1056, 1057, 1003, 3001, " &
                                                            " 1031, 1001, 1020, 1023, 1053, 1052, 1055, 1058, 1002, 1028, 1069 ) "

                sSql = " " 'Nota: escludo le bolle allegate a fatture

                sSql = sSql & " Select SUM(IsNull(Movimenti_Dettagli.Prezzo_Unitario_Netto,0) * IsNull(Movimenti_Dettagli.Qta ,0)) as Delta, " &
                              "        SUM(IsNull(Movimenti_Dettagli.Qta ,0)) as Qta_Complessiva " &
                              " FROM   Agenda, Movimenti, Movimenti_Dettagli, Mov_Destinazioni " &
                              " WHERE  Movimenti_Dettagli.Id_Mov_Det NOT IN " &
                              "           (Select Distinct Id_Mov_Det_Rif FROM Mov_Dettagli_Riferimenti " &
                              "            Where Piva = '" & Agro_SQL_SaveText(Piva) & "' AND Lav_Cod In (1000, 1001) )" & strJoin


                Dim dt As DataTable

                Try

                    '--------------------------------------------------------------------------
                    dt = EseguiQuery_Lettura(objParametri, sSql, nomeRoutine)
                    '--------------------------------------------------------------------------

                Catch ex As Exception
                    messaggioErrore = ex.Message
                    Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
                    dt = Nothing
                    Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
                End Try


                Dim rs As DataRow = dt.Rows(0)


                '------------------------------
                If IsNumeric(rs("Delta")) AndAlso IsNumeric(rs("Qta_Complessiva")) Then

                    Select Case CDbl(rs("Qta_Complessiva"))

                        Case 0 'Non esistono valorizzazioni del bene nel periodo competenza

                            Return Format(0, "###,###,##0.0#")

                        Case Else 'Esistono valorizzazioni del bene nel periodo di competenza

                            Return Format(rs("Delta") / rs("Qta_Complessiva"), "###,###,##0.0#")

                    End Select

                Else

                    'Gestione Eccezione
                    Return Format(0, "###,###,##0.0#")

                End If


            Case Else

                Return Format(0, "###,###,##0.0#")

        End Select

    End Function


    Public Function Leggi(ByVal Piva As String,
                          ByVal Sa_Cod As Integer,
                          ByVal Id_Agenda As Integer,
                          ByVal Id_Mov As Integer,
                          ByVal Id_Mov_Det As Integer,
                          ByVal Elem_Cod As Integer,
                          ByVal Pro_Cod As Integer,
                          ByVal Mat_Cod As Integer,
                          ByVal Cau_Mov As String,
                          ByVal Cal_Cod As Integer,
                          ByVal Cod_Progetto As Integer,
                          ByVal Fase_Cod As Integer,
                          ByVal Contabilizzato As Integer,
                          ByVal Pendente As Integer,
                          ByVal Cod_Conto As Integer,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          Optional ByVal topNrecords As Integer? = Nothing,
                          Optional ByVal ignoraValidita As Boolean = False
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Dettagli_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    strSql.Length = 0
                    strSql.AppendLine(" SELECT PIVA,Sa_Cod,Id_Agenda,Id_Mov,Id_Mov_Det,Elem_Cod,Pro_Cod,Mat_Cod,Mov_Det_Des,Udm_Cod,Qta,Prezzo_Unitario,Cod_Progetto,Fase_Cod,Cal_Cod,Lotto,Jolly_Int  ")
                    strSql.AppendLine(" FROM  Movimenti_Dettagli WITH(NOLOCK)")

                    If ignoraValidita Then
                        strSql.AppendLine(" WHERE 1=1 ")
                    Else
                        strSql.AppendLine(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                        strSql.AppendLine(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    End If

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

                    If Elem_Cod <> 0 Then
                        strSql.AppendLine(" AND Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
                    End If

                    If Pro_Cod <> 0 Then
                        strSql.AppendLine(" AND Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
                    End If

                    If Mat_Cod <> 0 Then
                        strSql.AppendLine(" AND Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
                    End If

                    If Cal_Cod <> 0 Then
                        strSql.AppendLine(" AND Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "   ")
                    End If

                    If Cod_Progetto <> 0 Then
                        strSql.AppendLine(" AND Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & "   ")
                    End If

                    If Fase_Cod <> 0 Then
                        strSql.AppendLine(" AND Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   ")
                    End If

                    If Contabilizzato <> 0 Then
                        strSql.AppendLine(" AND Contabilizzato = " & Agro_SQL_SaveNum(Contabilizzato) & "   ")
                    End If

                    If Pendente <> 0 Then
                        strSql.AppendLine(" AND Pendente = " & Agro_SQL_SaveNum(Pendente) & "   ")
                    End If

                    If Cod_Conto <> 0 Then
                        strSql.AppendLine(" AND Cod_Conto = " & Agro_SQL_SaveNum(Cod_Conto) & "   ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

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

                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                '--------------------------------------------------------------------------

                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    strSql.Length = 0
                    strSql.AppendLine(" SELECT * ")
                    strSql.AppendLine(" FROM  Movimenti_Dettagli WITH(NOLOCK)")

                    If ignoraValidita Then
                        strSql.AppendLine(" WHERE 1=1 ")
                    Else
                        strSql.AppendLine(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                        strSql.AppendLine(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    End If

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

                    If Elem_Cod <> 0 Then
                        strSql.AppendLine(" AND Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
                    End If

                    If Pro_Cod <> 0 Then
                        strSql.AppendLine(" AND Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
                    End If

                    If Mat_Cod <> 0 Then
                        strSql.AppendLine(" AND Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
                    End If

                    If Cal_Cod <> 0 Then
                        strSql.AppendLine(" AND Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "   ")
                    End If

                    If Cod_Progetto <> 0 Then
                        strSql.AppendLine(" AND Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & "   ")
                    End If

                    If Fase_Cod <> 0 Then
                        strSql.AppendLine(" AND Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   ")
                    End If

                    If Contabilizzato <> 0 Then
                        strSql.AppendLine(" AND Contabilizzato = " & Agro_SQL_SaveNum(Contabilizzato) & "   ")
                    End If

                    If Pendente <> 0 Then
                        strSql.AppendLine(" AND Pendente = " & Agro_SQL_SaveNum(Pendente) & "   ")
                    End If

                    If Cod_Conto <> 0 Then
                        strSql.AppendLine(" AND Cod_Conto = " & Agro_SQL_SaveNum(Cod_Conto) & "   ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

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

                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Id_Mov Asc ")
                    End If

                '--------------------------------------------------------------------------

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                    strSql.Length = 0
                    strSql.AppendLine(" SELECT Movimenti_Dettagli.*, Movimenti.Data_Movimento, Movimenti.Cau_Mov, Imprese.rag_soc, Agenda.des_lib ")
                    strSql.AppendLine(" FROM  Movimenti_Dettagli WITH(NOLOCK)")
                    strSql.AppendLine(" INNER JOIN Imprese WITH(NOLOCK) ON Movimenti_Dettagli.Piva = Imprese.Piva ")
                    strSql.AppendLine(" INNER JOIN Agenda WITH(NOLOCK) ON Movimenti_Dettagli.Piva = Agenda.Piva AND Movimenti_Dettagli.Id_Agenda = Agenda.Id_Agenda ")
                    strSql.AppendLine(" INNER JOIN Movimenti WITH(NOLOCK) ON Movimenti_Dettagli.Piva = Movimenti.Piva AND Movimenti_Dettagli.Id_Agenda = Movimenti.Id_Agenda  ")
                    strSql.AppendLine(" AND Movimenti_Dettagli.Id_Mov = Movimenti.Id_Mov  ")

                    If ignoraValidita Then
                        strSql.AppendLine(" WHERE 1=1 ")
                    Else
                        strSql.AppendLine(" WHERE Movimenti_Dettagli.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                        strSql.AppendLine(" AND   Movimenti_Dettagli.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    End If

                    If Piva <> "" Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If

                    If Sa_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Id_Agenda <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
                    End If

                    If Id_Mov <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
                    End If

                    If Id_Mov_Det <> 0 Then
                        strSql.AppendLine(" AND Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")
                    End If

                    If Elem_Cod <> 0 Then
                        strSql.AppendLine(" AND Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
                    End If

                    If Pro_Cod <> 0 Then
                        strSql.AppendLine(" AND Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
                    End If

                    If Mat_Cod <> 0 Then
                        strSql.AppendLine(" AND Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
                    End If

                    If Cal_Cod <> 0 Then
                        strSql.AppendLine(" AND Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "   ")
                    End If

                    If Cod_Progetto <> 0 Then
                        strSql.AppendLine(" AND Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & "   ")
                    End If

                    If Fase_Cod <> 0 Then
                        strSql.AppendLine(" AND Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   ")
                    End If

                    If Contabilizzato <> 0 Then
                        strSql.AppendLine(" AND Contabilizzato = " & Agro_SQL_SaveNum(Contabilizzato) & "   ")
                    End If

                    If Pendente <> 0 Then
                        strSql.AppendLine(" AND Pendente = " & Agro_SQL_SaveNum(Pendente) & "   ")
                    End If

                    If Cod_Conto <> 0 Then
                        strSql.AppendLine(" AND Cod_Conto = " & Agro_SQL_SaveNum(Cod_Conto) & "   ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Movimenti_Dettagli.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Movimenti_Dettagli.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select

                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                '--------------------------------------------------------------------------

                Case enumSelezioneVariabile.Selezione_JoinCompleta

                    strSql.Length = 0

                    strSql.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")

                    If Not IsNothing(topNrecords) Then
                        strSql.AppendLine(" SELECT TOP " & topNrecords.ToString & " ")
                        strSql.AppendLine(" Agenda.Id_Agenda, Agenda.Sa_Cod, Agenda.Lav_Cod, ")
                        strSql.AppendLine(" Movimenti_Dettagli.Id_Mov, Movimenti_Dettagli.Id_Mov_Det, ")
                        strSql.AppendLine(" Movimenti_Dettagli.Elem_Cod, Movimenti_Dettagli.Pro_Cod, Movimenti_Dettagli.Mat_Cod,")
                        strSql.AppendLine(" Movimenti_Dettagli.Lotto, Movimenti.Data_Movimento, Movimenti_Dettagli.Udm_Cod ")
                    Else
                        strSql.AppendLine(" SELECT Movimenti_Dettagli.* , Agenda.* , Movimenti. * ,  Imprese.Rag_Soc ")
                    End If

                    strSql.AppendLine(" FROM  Agenda WITH(NOLOCK), Movimenti WITH(NOLOCK), Movimenti_Dettagli WITH(NOLOCK), Imprese WITH(NOLOCK)")

                    If ignoraValidita Then
                        strSql.AppendLine(" WHERE 1=1 ")
                    Else
                        strSql.AppendLine(" WHERE Movimenti_Dettagli.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                        strSql.AppendLine(" AND   Movimenti_Dettagli.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                        strSql.AppendLine(" AND   Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                        strSql.AppendLine(" AND   Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    End If

                    'Join sulla Piva
                    strSql.AppendLine(" AND   Agenda.Piva = Movimenti.Piva ")
                    strSql.AppendLine(" AND   Agenda.Piva = Movimenti_Dettagli.Piva ")
                    strSql.AppendLine(" AND   Movimenti.Piva = Movimenti_Dettagli.Piva ")
                    strSql.AppendLine(" AND   Agenda.Piva = Imprese.Piva ")

                    'Join sul Id_Agenda
                    strSql.AppendLine(" AND   Agenda.Id_Agenda = Movimenti.Id_Agenda ")
                    strSql.AppendLine(" AND   Agenda.Id_Agenda = Movimenti_Dettagli.Id_Agenda ")
                    strSql.AppendLine(" AND   Movimenti.Id_Agenda = Movimenti_Dettagli.Id_Agenda ")

                    'Join su Id_Mov
                    strSql.AppendLine(" AND   Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov ")

                    If Piva <> "" Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If

                    If Sa_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Id_Agenda <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
                    End If

                    If Id_Mov <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
                    End If

                    If Id_Mov_Det <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")
                    End If

                    If Elem_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
                    End If

                    If Pro_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
                    End If

                    If Mat_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
                    End If

                    If Cau_Mov <> "" Then
                        strSql.AppendLine(" AND Movimenti.Cau_Mov = '" & Agro_SQL_SaveText(Cau_Mov) & "'   ")
                    End If

                    If Cal_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "   ")
                    End If

                    If Cod_Progetto <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & "   ")
                    End If

                    If Fase_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   ")
                    End If

                    If Contabilizzato <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Contabilizzato = " & Agro_SQL_SaveNum(Contabilizzato) & "   ")
                    End If

                    If Pendente <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Pendente = " & Agro_SQL_SaveNum(Pendente) & "   ")
                    End If

                    If Cod_Conto <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Cod_Conto = " & Agro_SQL_SaveNum(Cod_Conto) & "   ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Agenda.Inviato >=0 ")
                            strSql.AppendLine(" AND   Movimenti.Inviato >=0 ")
                            strSql.AppendLine(" AND   Movimenti_Dettagli.Inviato >=0 ")
                            strSql.AppendLine(" AND   Imprese.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Agenda.Inviato =-1 ")
                            strSql.AppendLine(" AND   Movimenti.Inviato =-1 ")
                            strSql.AppendLine(" AND   Movimenti_Dettagli.Inviato =-1 ")
                            strSql.AppendLine(" AND   Imprese.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select

                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Movimenti_Dettagli.Id_Mov Asc ")
                    End If

            End Select

            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, NomeRoutine)

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function


    Public Function LeggiGiacenze_Chiavi(ByVal Piva As String,
                                         ByVal Sa_Cod As Integer,
                                         ByVal Elem_Cod As Integer,
                                         ByVal Pro_Cod As Integer,
                                         ByVal Mat_Cod As Integer,
                                         ByVal Udm_Cod As Integer,
                                         ByVal Cal_Cod As Integer,
                                         ByVal Cod_Progetto As Integer,
                                         ByVal Fase_Cod As Integer,
                                         ByVal Lotto As String,
                                         ByVal Id_Destinazione As Integer,
                                         ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByVal xOrderBy As String,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                         ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Dettagli_R.LeggiGiacenze_Chiavi()"

        '====================================================================================
        '  LETTURA DELLE CHIAVI (Id_Agenda, Id_Mov, Id_Mov_Det) delle GIACENZE DI MAGAZZINO
        '  Questa Query è Utilizzata da Agro_Contab.Giacenze_W

        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            '------------------------------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    strSql.Length = 0
                    strSql.AppendLine(" SELECT  ISNULL(Agenda.Piva, '') as Piva_Chiave, ISNULL(Agenda.Id_Agenda,0) As Id_Agenda_Chiave, ")
                    strSql.AppendLine("         ISNULL(Movimenti.Id_Mov,0) as Id_Mov_Chiave,    ")
                    strSql.AppendLine("         Movimenti_Dettagli.*, ISNULL(Movimenti_Dettagli.Id_Mov_Det,0) As Id_Mov_Det_Chiave, Movimenti_Dettagli.Qta as Dett_Qta, ")
                    strSql.AppendLine("         Mov_Destinazioni.*, ISNULL(Mov_Destinazioni.Id_Mov_Det,0) As Id_Mov_Det_Des_Chiave, Mov_Destinazioni.Qta as Dest_Qta  ")

                    strSql.AppendLine(" FROM    Agenda (NOLOCK) ")
                    strSql.AppendLine("         Left Outer Join Movimenti (NOLOCK) ON (Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' AND Agenda.Piva = Movimenti.Piva AND Agenda.Id_Agenda = Movimenti.Id_Agenda)  ")
                    strSql.AppendLine("         Left Outer Join Movimenti_Dettagli (NOLOCK) ON (Movimenti.Piva = Movimenti_Dettagli.Piva AND ")
                    strSql.AppendLine("                             Movimenti.Id_Agenda = Movimenti_Dettagli.Id_Agenda AND ")
                    strSql.AppendLine("                             Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov AND ")
                    strSql.AppendLine("                             Movimenti_Dettagli.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " AND ")
                    strSql.AppendLine("                             Movimenti_Dettagli.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & " AND ")
                    strSql.AppendLine("                             Movimenti_Dettagli.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & " AND ")
                    strSql.AppendLine("                             Movimenti_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & " AND ")
                    strSql.AppendLine("                             Movimenti_Dettagli.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & " AND ")
                    strSql.AppendLine("                             Movimenti_Dettagli.Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & " AND ")
                    strSql.AppendLine("                             Movimenti_Dettagli.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & " AND ")
                    strSql.AppendLine("                             Movimenti_Dettagli.Lotto = '" & Agro_SQL_SaveText(Lotto) & "') ")
                    strSql.AppendLine("         Left Outer Join Mov_Destinazioni (NOLOCK) ON (Movimenti_Dettagli.Piva = Mov_Destinazioni.Piva AND ")
                    strSql.AppendLine("                             Movimenti_Dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND ")
                    strSql.AppendLine("                             Movimenti_Dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND ")
                    strSql.AppendLine("                             Movimenti_Dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det AND ")
                    strSql.AppendLine("                             Mov_Destinazioni.Tipo_Destinazione = 20 AND ")
                    strSql.AppendLine("                             Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & ")")

                    strSql.AppendLine(" WHERE   Agenda.Lav_Cod = -1            ")
                    strSql.AppendLine(" AND     Movimenti.Cau_Mov = 'GIACENZE' ")


                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Agenda.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Agenda.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If


                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    strSql.Length = 0

                    strSql.AppendLine(" SELECT  ISNULL(Agenda.Piva, '') as Piva_Chiave, ISNULL(Agenda.Id_Agenda,0) As Id_Agenda_Chiave, ")
                    strSql.AppendLine("         ISNULL(Movimenti.Id_Mov,0) as Id_Mov_Chiave,    ")
                    strSql.AppendLine("         Movimenti_Dettagli.*, ISNULL(Movimenti_Dettagli.Id_Mov_Det,0) As Id_Mov_Det_Chiave, Movimenti_Dettagli.Qta as Dett_Qta, ")
                    strSql.AppendLine("         Mov_Destinazioni.*, ISNULL(Mov_Destinazioni.Id_Mov_Det,0) As Id_Mov_Det_Des_Chiave, Mov_Destinazioni.Qta as Dest_Qta  ")

                    strSql.AppendLine(" FROM    Agenda (NOLOCK) ")
                    strSql.AppendLine("         Left Outer Join Movimenti (NOLOCK) ON (Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' AND Agenda.Piva = Movimenti.Piva AND Agenda.Id_Agenda = Movimenti.Id_Agenda)  ")
                    strSql.AppendLine("         Left Outer Join Movimenti_Dettagli (NOLOCK) ON (Movimenti.Piva = Movimenti_Dettagli.Piva AND ")
                    strSql.AppendLine("                             Movimenti.Id_Agenda = Movimenti_Dettagli.Id_Agenda AND ")
                    strSql.AppendLine("                             Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov AND ")
                    strSql.AppendLine("                             Movimenti_Dettagli.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " AND ")
                    strSql.AppendLine("                             Movimenti_Dettagli.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & " AND ")
                    strSql.AppendLine("                             Movimenti_Dettagli.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & " AND ")
                    strSql.AppendLine("                             Movimenti_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & " AND ")
                    strSql.AppendLine("                             Movimenti_Dettagli.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & " AND ")
                    strSql.AppendLine("                             Movimenti_Dettagli.Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & " AND ")
                    strSql.AppendLine("                             Movimenti_Dettagli.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & " AND ")
                    strSql.AppendLine("                             Movimenti_Dettagli.Lotto = '" & Agro_SQL_SaveText(Lotto) & "') ")
                    strSql.AppendLine("         Left Outer Join Mov_Destinazioni (NOLOCK) ON (Movimenti_Dettagli.Piva = Mov_Destinazioni.Piva AND ")
                    strSql.AppendLine("                             Movimenti_Dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND ")
                    strSql.AppendLine("                             Movimenti_Dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND ")
                    strSql.AppendLine("                             Movimenti_Dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det AND ")
                    strSql.AppendLine("                             Mov_Destinazioni.Tipo_Destinazione = 20 AND ")
                    strSql.AppendLine("                             Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & ")")

                    strSql.AppendLine(" WHERE   Agenda.Lav_Cod = -1            ")
                    strSql.AppendLine(" AND     Movimenti.Cau_Mov = 'GIACENZE' ")


                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Agenda.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Agenda.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni



                Case enumSelezioneVariabile.Selezione_JoinCompleta



            End Select



            'ATTENZIONE!!!
            'Modifica del 23/04/2010:
            'non va bene mettere queste clausole nel where
            'perché le tabelle sono in LEFT JOIN e se non ci sono record in Movimenti_Dettagli
            'la query non restituisce alcun record ---> MEGA CASINO NELLE GIACENZE!!!!!

            ''Select Case FlagVisibilita
            ''    Case 1  'Solo i NON CANCELLATI
            ''        StrSQL.AppendLine(" AND   Agenda.Inviato >=0 ")
            ''        StrSQL.AppendLine(" AND   Movimenti.Inviato >=0 ")
            ''        StrSQL.AppendLine(" AND   Movimenti_Dettagli.Inviato >=0 ")
            ''        StrSQL.AppendLine(" AND   Mov_Destinazioni.Inviato >=0 ")
            ''    Case 2  'Solo i CANCELLATI
            ''        StrSQL.AppendLine(" AND   Agenda.Inviato =-1 ")
            ''        StrSQL.AppendLine(" AND   Movimenti.Inviato =-1 ")
            ''        StrSQL.AppendLine(" AND   Movimenti_Dettagli.Inviato =-1 ")
            ''        StrSQL.AppendLine(" AND   Mov_Destinazioni.Inviato =-1 ")
            ''    Case 3  'TUTTI
            ''        '
            ''    Case Else
            ''        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            ''End Select

            '------------------------------------------------------------------


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


    Public Function Leggi_Dettagli_Formulati_SpecieVegetali(ByVal Piva As String,
                                                            ByVal Sa_Cod As Integer,
                                                            ByVal Id_Agenda As Integer,
                                                            ByVal Id_Mov As Integer,
                                                            ByVal Id_Mov_Det As Integer,
                                                            ByVal Elem_Cod As Integer,
                                                            ByVal Pro_Cod As Integer,
                                                            ByVal Mat_Cod As Integer,
                                                            ByVal Cau_Mov As String,
                                                            ByVal xFiltroAggiuntivo As String,
                                                            ByVal xOrderBy As String,
                                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                            ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Dettagli_R.Leggi_Dettagli_Formulati_SpecieVegetali()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""            =>  si leggono tutti i Movimenti_dettagli
        '   Sa_Cod = 0           =>  si leggono tutti i Movimenti_dettagli dell'impresa
        '   Appezza = 0          =>  si leggono tutti i Movimenti_dettagli del centro aziendale
        '   Id_Destinazione = 0  =>  si leggono tutti i Movimenti_dettagli dell'appezzamento
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT DISTINCT Movimenti_dettagli.Pro_Cod, Cultivar.Veg_Cod, Movimenti_dettagli.PIVA, Movimenti_dettagli.Sa_Cod, Movimenti_dettagli.Id_Agenda, Movimenti_dettagli.Id_Mov, Movimenti_dettagli.Id_Mov_Det ")
            strSql.AppendLine(" FROM  Cultivar INNER JOIN ")
            strSql.AppendLine(" Reg_Impianti ON Cultivar.Cul_Cod = Reg_Impianti.CUL_COD INNER JOIN ")
            strSql.AppendLine(" Movimenti_dettagli INNER JOIN ")
            strSql.AppendLine(" Mov_Destinazioni ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod AND  ")
            strSql.AppendLine(" Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND  ")
            strSql.AppendLine(" Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ON Reg_Impianti.PIVA = Mov_Destinazioni.Piva AND  ")
            strSql.AppendLine(" Reg_Impianti.SA_COD = Mov_Destinazioni.Sa_Cod AND Reg_Impianti.APPEZZA = Mov_Destinazioni.Appezza AND  ")
            strSql.AppendLine(" Reg_Impianti.ID_REG = Mov_Destinazioni.Id_Destinazione INNER JOIN ")
            strSql.AppendLine(" Movimenti ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov ")
            strSql.AppendLine(" WHERE Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            strSql.AppendLine(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Piva <> "" Then
                strSql.AppendLine(" AND Movimenti_dettagli.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND Movimenti_dettagli.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Id_Agenda <> 0 Then
                strSql.AppendLine(" AND Movimenti_dettagli.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
            End If

            If Id_Mov <> 0 Then
                strSql.AppendLine(" AND Movimenti_dettagli.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
            End If

            If Id_Mov_Det <> 0 Then
                strSql.AppendLine(" AND Movimenti_dettagli.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")
            End If

            If Elem_Cod <> 0 Then
                strSql.AppendLine(" AND Movimenti_dettagli.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
            End If

            If Pro_Cod <> 0 Then
                strSql.AppendLine(" AND Movimenti_dettagli.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
            End If

            If Mat_Cod <> 0 Then
                strSql.AppendLine(" AND Movimenti_dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
            End If


            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Movimenti_dettagli.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Movimenti_dettagli.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Movimenti_Dettagli.Id_Mov Asc ")
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

    Public Function LeggiConsistenzeEnologiche(ByVal Piva As String,
                                               ByVal Sa_Cod As Integer,
                                               ByVal Elem_Cod As Integer,
                                               ByVal Pro_Cod As Integer,
                                               ByVal Mat_Cod As Integer,
                                               ByVal Udm_Cod As Integer,
                                               ByVal Id_Destinazione As Integer,
                                               ByVal Cal_Cod As Integer,
                                               ByVal Cod_Progetto As Integer,
                                               ByVal Fase_Cod As Integer,
                                               ByVal Lotto As String,
                                               ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                               ByVal xFiltroAggiuntivo As String,
                                               ByVal xOrderBy As String,
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                               ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Dettagli_R.LeggiConsistenzeEnologiche()"

        '====================================================================================
        '  LETTURA DELLE CONSISTENZE ENOLOGICHE ALL'INTERNO DELLE VASCHE
        '
        'Parametri opzionali :
        '   Piva = ""
        '   Sa_Cod = 0
        '   Elem_Cod = 0            
        '   Pro_Cod = 0
        '   Mat_Cod = 0
        '   Udm_Cod = 0
        '   Id_Destinazione = 0
        '   Cal_Cod = 0
        '   Cod_Progetto = 0
        '   Fase_Cod = 0
        '   Lotto = ""
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            '------------------------------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    strSql.Length = 0

                    strSql.AppendLine(" SELECT Agenda.* , Movimenti.* , Movimenti_Dettagli.* , Mov_Destinazioni.* , Mov_Destinazioni.Qta as Dest_Qta , Movimenti_Dettagli.Qta as Dett_Qta  ")
                    strSql.AppendLine(" FROM  Agenda (NOLOCK) , Movimenti (NOLOCK) , Movimenti_Dettagli (NOLOCK) , Mov_Destinazioni (NOLOCK) ")
                    strSql.AppendLine(" WHERE Movimenti_Dettagli.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    strSql.AppendLine(" AND   Agenda.Lav_Cod = -3    ")
                    strSql.AppendLine(" AND   Movimenti.Cau_Mov = 'MOSTO'    ")
                    strSql.AppendLine(" AND   Mov_Destinazioni.Tipo_Destinazione = 13   ")

                    'Join sulla Piva
                    strSql.AppendLine(" AND   Agenda.Piva = Movimenti.Piva ")
                    strSql.AppendLine(" AND   Agenda.Piva = Movimenti_Dettagli.Piva ")
                    strSql.AppendLine(" AND   Agenda.Piva = Mov_Destinazioni.Piva ")
                    strSql.AppendLine(" AND   Movimenti.Piva = Movimenti_Dettagli.Piva ")
                    strSql.AppendLine(" AND   Movimenti.Piva = Mov_Destinazioni.Piva ")
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Piva = Mov_Destinazioni.Piva ")

                    'Join sul Sa_Cod
                    strSql.AppendLine(" AND   Agenda.Sa_Cod = Movimenti.Sa_Cod ")
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod ")

                    'Join su Id_Agenda
                    strSql.AppendLine(" AND   Agenda.Id_Agenda = Movimenti.Id_Agenda ")
                    strSql.AppendLine(" AND   Movimenti.Id_Agenda = Movimenti_Dettagli.Id_Agenda ")
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda ")

                    'Join su Id_Mov
                    strSql.AppendLine(" AND   Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov ")
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Id_Mov = Mov_Destinazioni.Id_Mov ")

                    'Join su Id_Mov_Det
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")

                    If Piva <> "" Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If

                    If Sa_Cod <> 0 Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Elem_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
                    End If

                    If Pro_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
                    End If

                    If Mat_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
                    End If

                    If Udm_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   ")
                    End If

                    If Id_Destinazione <> 0 Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "   ")
                    End If

                    If Cal_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "   ")
                    End If

                    If Cod_Progetto <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & "   ")
                    End If

                    If Fase_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   ")
                    End If

                    If Lotto <> "" Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Lotto = '" & Agro_SQL_SaveText(Lotto) & "'   ")
                    End If



                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Agenda.Inviato >=0 ")
                            strSql.AppendLine(" AND   Movimenti.Inviato >=0 ")
                            strSql.AppendLine(" AND   Movimenti_Dettagli.Inviato >=0 ")
                            strSql.AppendLine(" AND   Mov_Destinazioni.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Agenda.Inviato =-1 ")
                            strSql.AppendLine(" AND   Movimenti.Inviato =-1 ")
                            strSql.AppendLine(" AND   Movimenti_Dettagli.Inviato =-1 ")
                            strSql.AppendLine(" AND   Mov_Destinazioni.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Movimenti_Dettagli.Elem_Cod Asc ")
                    End If


                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    strSql.Length = 0

                    strSql.AppendLine(" SELECT Agenda.* , Movimenti.* , Movimenti_Dettagli.* , Mov_Destinazioni.* , Mov_Destinazioni.Qta as Dest_Qta , Movimenti_Dettagli.Qta as Dett_Qta  ")
                    strSql.AppendLine(" FROM  Agenda (NOLOCK) , Movimenti (NOLOCK) , Movimenti_Dettagli (NOLOCK) , Mov_Destinazioni (NOLOCK) ")
                    strSql.AppendLine(" WHERE Movimenti_Dettagli.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    strSql.AppendLine(" AND   Agenda.Lav_Cod = -3    ")
                    strSql.AppendLine(" AND   Movimenti.Cau_Mov = 'MOSTO'    ")
                    strSql.AppendLine(" AND   Mov_Destinazioni.Tipo_Destinazione = 13   ")

                    'Join sulla Piva
                    strSql.AppendLine(" AND   Agenda.Piva = Movimenti.Piva ")
                    strSql.AppendLine(" AND   Agenda.Piva = Movimenti_Dettagli.Piva ")
                    strSql.AppendLine(" AND   Agenda.Piva = Mov_Destinazioni.Piva ")
                    strSql.AppendLine(" AND   Movimenti.Piva = Movimenti_Dettagli.Piva ")
                    strSql.AppendLine(" AND   Movimenti.Piva = Mov_Destinazioni.Piva ")
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Piva = Mov_Destinazioni.Piva ")

                    'Join sul Sa_Cod
                    strSql.AppendLine(" AND   Agenda.Sa_Cod = Movimenti.Sa_Cod ")
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod ")

                    'Join su Id_Agenda
                    strSql.AppendLine(" AND   Agenda.Id_Agenda = Movimenti.Id_Agenda ")
                    strSql.AppendLine(" AND   Movimenti.Id_Agenda = Movimenti_Dettagli.Id_Agenda ")
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda ")

                    'Join su Id_Mov
                    strSql.AppendLine(" AND   Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov ")
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Id_Mov = Mov_Destinazioni.Id_Mov ")

                    'Join su Id_Mov_Det
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")

                    If Piva <> "" Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If

                    If Sa_Cod <> 0 Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Elem_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
                    End If

                    If Pro_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
                    End If

                    If Mat_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
                    End If

                    If Udm_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   ")
                    End If

                    If Id_Destinazione <> 0 Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "   ")
                    End If

                    If Cal_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "   ")
                    End If

                    If Cod_Progetto <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & "   ")
                    End If

                    If Fase_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   ")
                    End If

                    If Lotto <> "" Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Lotto = '" & Agro_SQL_SaveText(Lotto) & "'   ")
                    End If



                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Agenda.Inviato >=0 ")
                            strSql.AppendLine(" AND   Movimenti.Inviato >=0 ")
                            strSql.AppendLine(" AND   Movimenti_Dettagli.Inviato >=0 ")
                            strSql.AppendLine(" AND   Mov_Destinazioni.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Agenda.Inviato =-1 ")
                            strSql.AppendLine(" AND   Movimenti.Inviato =-1 ")
                            strSql.AppendLine(" AND   Movimenti_Dettagli.Inviato =-1 ")
                            strSql.AppendLine(" AND   Mov_Destinazioni.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Movimenti_Dettagli.Elem_Cod Asc ")
                    End If


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case enumSelezioneVariabile.Selezione_JoinCompleta


            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & messaggioErrore)
        End Try


        Return dt

    End Function

    Public Function LeggiCaricoScaricoConsistenze(ByVal Piva As String,
                                                  ByVal Sa_Cod As Integer,
                                                  ByVal Id_Agenda As Integer,
                                                  ByVal Id_Mov As Integer,
                                                  ByVal Id_Mov_Det As Integer,
                                                  ByVal Elem_Cod As Integer,
                                                  ByVal Pro_Cod As Integer,
                                                  ByVal Mat_Cod As Integer,
                                                  ByVal Udm_Cod As Integer,
                                                  ByVal Lav_Cod As Integer,
                                                  ByVal Id_Destinazione As Integer,
                                                  ByVal Tipo_Destinazione As Integer,
                                                  ByVal Cau_Mov As String,
                                                  ByVal Cod_Progetto As Integer,
                                                  ByVal Fase_Cod As Integer,
                                                  ByVal Contabilizzato As Integer,
                                                  ByVal Pendente As Integer,
                                                  ByVal Lotto As String,
                                                  ByVal Cal_Cod As Integer,
                                                  ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                                  ByVal xFiltroAggiuntivo As String,
                                                  ByVal xOrderBy As String,
                                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                  ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Dettagli_R.LeggiCaricoScaricoConsistenze()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    '------------------------------------------------------------------

                    strSql.Length = 0

                    strSql.AppendLine(" SELECT Agenda.* , Movimenti.* , Movimenti_Dettagli.* , Mov_Destinazioni.* , Mov_Destinazioni.Qta as Dest_Qta , Movimenti_Dettagli.Qta as Dett_Qta, Zoo_Animali.*  ")
                    strSql.AppendLine(" FROM  Agenda , Movimenti , Movimenti_Dettagli , Mov_Destinazioni, Zoo_Animali ")
                    strSql.AppendLine(" WHERE Movimenti_Dettagli.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    strSql.AppendLine(" AND   Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND   Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))

                    'Join sulla Piva
                    strSql.AppendLine(" AND   Agenda.Piva = Movimenti.Piva ")
                    strSql.AppendLine(" AND   Agenda.Piva = Movimenti_Dettagli.Piva ")
                    strSql.AppendLine(" AND   Agenda.Piva = Mov_Destinazioni.Piva ")
                    strSql.AppendLine(" AND   Movimenti.Piva = Movimenti_Dettagli.Piva ")
                    strSql.AppendLine(" AND   Movimenti.Piva = Mov_Destinazioni.Piva ")
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Piva = Mov_Destinazioni.Piva ")

                    'Join sul Sa_Cod
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod ")

                    'Join sul Id_Agenda
                    strSql.AppendLine(" AND   Agenda.Id_Agenda = Movimenti.Id_Agenda ")
                    strSql.AppendLine(" AND   Agenda.Id_Agenda = Movimenti_Dettagli.Id_Agenda ")
                    strSql.AppendLine(" AND   Agenda.Id_Agenda = Mov_Destinazioni.Id_Agenda ")
                    strSql.AppendLine(" AND   Movimenti.Id_Agenda = Movimenti_Dettagli.Id_Agenda ")
                    strSql.AppendLine(" AND   Movimenti.Id_Agenda = Mov_Destinazioni.Id_Agenda ")
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda ")


                    'Join su Id_Mov
                    strSql.AppendLine(" AND   Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov ")
                    strSql.AppendLine(" AND   Movimenti.Id_Mov = Mov_Destinazioni.Id_Mov ")
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Id_Mov = Mov_Destinazioni.Id_Mov ")

                    'Join si Id_Mov_Det
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")

                    'Join si Cod_Progetto
                    strSql.AppendLine(" AND   Movimenti_dettagli.Cod_Progetto = Zoo_Animali.Cod_Progetto ")

                    If Piva <> "" Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If

                    If Sa_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Id_Agenda <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
                    End If

                    If Id_Mov <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
                    End If

                    If Id_Mov_Det <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")
                    End If

                    If Elem_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
                    End If

                    If Pro_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
                    End If

                    If Mat_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
                    End If

                    If Udm_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   ")
                    End If

                    If Lav_Cod <> 0 Then
                        strSql.AppendLine(" AND Agenda.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & "   ")
                    End If

                    If Id_Destinazione <> 0 Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "   ")
                    End If

                    If Tipo_Destinazione <> 0 Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Tipo_Destinazione = " & Agro_SQL_SaveNum(Tipo_Destinazione) & "   ")
                    End If

                    If Cau_Mov <> "" Then
                        strSql.AppendLine(" AND  Movimenti.Cau_Mov = '" & Agro_SQL_SaveText(Cau_Mov) & "'   ")
                    Else

                        'Sono state aggiunte le causali dei conferimenti
                        strSql.AppendLine(" AND Movimenti.Cau_Mov IN ('7300', '7350', '4100', '4200')   ")
                    End If

                    If Cod_Progetto <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & "   ")
                    End If

                    If Fase_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   ")
                    End If

                    If Contabilizzato <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Contabilizzato = " & Agro_SQL_SaveNum(Contabilizzato) & "   ")
                    End If

                    If Pendente <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Pendente = " & Agro_SQL_SaveNum(Pendente) & "   ")
                    End If

                    If Cal_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "   ")
                    End If

                    If Lotto <> "" Then
                        strSql.AppendLine(" AND Upper(Movimenti_Dettagli.Lotto) = '" & UCase(Agro_SQL_SaveText(Lotto)) & "'   ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Agenda.Inviato >=0 ")
                            strSql.AppendLine(" AND   Movimenti.Inviato >=0 ")
                            strSql.AppendLine(" AND   Movimenti_Dettagli.Inviato >=0 ")
                            strSql.AppendLine(" AND   Mov_Destinazioni.Inviato >=0 ")
                            strSql.AppendLine(" AND   Zoo_Animali.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Agenda.Inviato =-1 ")
                            strSql.AppendLine(" AND   Movimenti.Inviato =-1 ")
                            strSql.AppendLine(" AND   Movimenti_Dettagli.Inviato =-1 ")
                            strSql.AppendLine(" AND   Mov_Destinazioni.Inviato =-1 ")
                            strSql.AppendLine(" AND   Zoo_Animali.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Movimenti_Dettagli.Elem_Cod Asc ")
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

    Public Function LeggiConsistenze(ByVal Piva As String,
                                     ByVal Sa_Cod As Integer,
                                     ByVal Elem_Cod As Integer,
                                     ByVal Pro_Cod As Integer,
                                     ByVal Mat_Cod As Integer,
                                     ByVal Udm_Cod As Integer,
                                     ByVal Id_Destinazione As Integer,
                                     ByVal Cal_Cod As Integer,
                                     ByVal Cod_Progetto As Integer,
                                     ByVal Fase_Cod As Integer,
                                     ByVal Lotto As String,
                                     ByVal Gen_Cod As Integer,
                                     ByVal Spe_Cod As Integer,
                                     ByVal IPro_Cod As Integer,
                                     ByVal Raz_Cod As Integer,
                                     ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                     ByVal xFiltroAggiuntivo As String,
                                     ByVal xOrderBy As String,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                     ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Dettagli_R.LeggiConsistenze()"

        '====================================================================================
        '  LETTURA DELLE CONSISTENZE DI STALLA
        '
        'Parametri opzionali :
        '   Piva = ""
        '   Sa_Cod = 0
        '   Elem_Cod = 0            
        '   Pro_Cod = 0
        '   Mat_Cod = 0
        '   Udm_Cod = 0
        '   Id_Destinazione = 0
        '   Cal_Cod = 0
        '   Cod_Progetto = 0
        '   Fase_Cod = 0
        '   Lotto = ""
        '
        '   Gen_Cod = -1
        '   Spe_Cod = -1
        '   IPro_Cod = -1
        '   Raz_Cod = -1
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    strSql.Length = 0

                    strSql.AppendLine(" SELECT Agenda.* , Movimenti.* , Movimenti_Dettagli.* , Mov_Destinazioni.* , Mov_Destinazioni.Qta as Dest_Qta , Movimenti_Dettagli.Qta as Dett_Qta, Zoo_Animali.*  ")
                    strSql.AppendLine(" FROM  Agenda (NOLOCK) , Movimenti (NOLOCK) , Movimenti_Dettagli (NOLOCK) , Mov_Destinazioni (NOLOCK) ,  Zoo_Animali (NOLOCK) ")
                    strSql.AppendLine(" WHERE Movimenti_Dettagli.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    strSql.AppendLine(" AND   Agenda.Lav_Cod = -2    ")
                    strSql.AppendLine(" AND   Movimenti.Cau_Mov = 'ANIMALI'    ")
                    strSql.AppendLine(" AND   Mov_Destinazioni.Tipo_Destinazione = 15   ")
                    strSql.AppendLine(" AND   Zoo_Animali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND   Zoo_Animali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    'Join sulla Piva
                    strSql.AppendLine(" AND   Agenda.Piva = Movimenti.Piva ")
                    strSql.AppendLine(" AND   Agenda.Piva = Movimenti_Dettagli.Piva ")
                    strSql.AppendLine(" AND   Agenda.Piva = Mov_Destinazioni.Piva ")
                    strSql.AppendLine(" AND   Movimenti.Piva = Movimenti_Dettagli.Piva ")
                    strSql.AppendLine(" AND   Movimenti.Piva = Mov_Destinazioni.Piva ")
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Piva = Mov_Destinazioni.Piva ")

                    'Join sul Sa_Cod
                    strSql.AppendLine(" AND   Agenda.Sa_Cod = Movimenti.Sa_Cod ")
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod ")

                    'Join su Id_Agenda
                    strSql.AppendLine(" AND   Agenda.Id_Agenda = Movimenti.Id_Agenda ")
                    strSql.AppendLine(" AND   Movimenti.Id_Agenda = Movimenti_Dettagli.Id_Agenda ")
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda ")

                    'Join su Id_Mov
                    strSql.AppendLine(" AND   Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov ")
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Id_Mov = Mov_Destinazioni.Id_Mov ")

                    'Join su Id_Mov_Det
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")


                    'Join su Zoo_Animali
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Cod_Progetto = Zoo_Animali.Cod_Progetto ")
                    '" AND   Movimenti_Dettagli.Piva = Zoo_Animali.Piva "


                    If Piva <> "" Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If

                    If Sa_Cod <> 0 Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Elem_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
                    End If

                    If Pro_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
                    End If

                    If Mat_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
                    End If

                    If Udm_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   ")
                    End If

                    If Id_Destinazione <> 0 Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "   ")
                    End If

                    If Cal_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "   ")
                    End If

                    If Cod_Progetto <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & "   ")
                    End If

                    If Fase_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   ")
                    End If

                    If Lotto <> "" Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Lotto = '" & Agro_SQL_SaveText(Lotto) & "'   ")
                    End If

                    If Gen_Cod <> -1 Then
                        strSql.AppendLine(" AND Zoo_Animali.Gen_Cod = " & Agro_SQL_SaveNum(Gen_Cod) & "   ")
                    End If

                    If Spe_Cod <> -1 Then
                        strSql.AppendLine(" AND Zoo_Animali.Spe_Cod = " & Agro_SQL_SaveNum(Spe_Cod) & "   ")
                    End If

                    If IPro_Cod <> -1 Then
                        strSql.AppendLine(" AND Zoo_Animali.IPro_Cod = " & Agro_SQL_SaveNum(IPro_Cod) & "   ")
                    End If

                    If Raz_Cod <> -1 Then
                        strSql.AppendLine(" AND Zoo_Animali.Raz_Cod = " & Agro_SQL_SaveNum(Raz_Cod) & "   ")
                    End If



                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Agenda.Inviato >=0 ")
                            strSql.AppendLine(" AND   Movimenti.Inviato >=0 ")
                            strSql.AppendLine(" AND   Movimenti_Dettagli.Inviato >=0 ")
                            strSql.AppendLine(" AND   Mov_Destinazioni.Inviato >=0 ")
                            strSql.AppendLine(" AND   Zoo_Animali.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Agenda.Inviato =-1 ")
                            strSql.AppendLine(" AND   Movimenti.Inviato =-1 ")
                            strSql.AppendLine(" AND   Movimenti_Dettagli.Inviato =-1 ")
                            strSql.AppendLine(" AND   Mov_Destinazioni.Inviato =-1 ")
                            strSql.AppendLine(" AND   Zoo_Animali.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Movimenti_Dettagli.Piva, Movimenti_Dettagli.Sa_Cod, Movimenti_Dettagli.Cod_Progetto Asc ")
                    End If




                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    '------------------------------------------------------------------

                    strSql.Length = 0

                    strSql.AppendLine(" SELECT Agenda.* , Movimenti.* , Movimenti_Dettagli.* , Mov_Destinazioni.* , Mov_Destinazioni.Qta as Dest_Qta , Movimenti_Dettagli.Qta as Dett_Qta, Zoo_Animali.*  ")
                    strSql.AppendLine(" FROM  Agenda (NOLOCK) , Movimenti (NOLOCK) , Movimenti_Dettagli (NOLOCK) , Mov_Destinazioni (NOLOCK) ,  Zoo_Animali (NOLOCK) ")
                    strSql.AppendLine(" WHERE Movimenti_Dettagli.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    strSql.AppendLine(" AND   Agenda.Lav_Cod = -2    ")
                    strSql.AppendLine(" AND   Movimenti.Cau_Mov = 'ANIMALI'    ")
                    strSql.AppendLine(" AND   Mov_Destinazioni.Tipo_Destinazione = 15   ")
                    strSql.AppendLine(" AND   Zoo_Animali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND   Zoo_Animali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    'Join sulla Piva
                    strSql.AppendLine(" AND   Agenda.Piva = Movimenti.Piva ")
                    strSql.AppendLine(" AND   Agenda.Piva = Movimenti_Dettagli.Piva ")
                    strSql.AppendLine(" AND   Agenda.Piva = Mov_Destinazioni.Piva ")
                    strSql.AppendLine(" AND   Movimenti.Piva = Movimenti_Dettagli.Piva ")
                    strSql.AppendLine(" AND   Movimenti.Piva = Mov_Destinazioni.Piva ")
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Piva = Mov_Destinazioni.Piva ")

                    'Join sul Sa_Cod
                    strSql.AppendLine(" AND   Agenda.Sa_Cod = Movimenti.Sa_Cod ")
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod ")

                    'Join su Id_Agenda
                    strSql.AppendLine(" AND   Agenda.Id_Agenda = Movimenti.Id_Agenda ")
                    strSql.AppendLine(" AND   Movimenti.Id_Agenda = Movimenti_Dettagli.Id_Agenda ")
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda ")

                    'Join su Id_Mov
                    strSql.AppendLine(" AND   Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov ")
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Id_Mov = Mov_Destinazioni.Id_Mov ")

                    'Join su Id_Mov_Det
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")


                    'Join su Zoo_Animali
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Cod_Progetto = Zoo_Animali.Cod_Progetto ")
                    '" AND   Movimenti_Dettagli.Piva = Zoo_Animali.Piva "


                    If Piva <> "" Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If

                    If Sa_Cod <> 0 Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Elem_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
                    End If

                    If Pro_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
                    End If

                    If Mat_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
                    End If

                    If Udm_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   ")
                    End If

                    If Id_Destinazione <> 0 Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "   ")
                    End If

                    If Cal_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "   ")
                    End If

                    If Cod_Progetto <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & "   ")
                    End If

                    If Fase_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   ")
                    End If

                    If Lotto <> "" Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Lotto = '" & Agro_SQL_SaveText(Lotto) & "'   ")
                    End If

                    If Gen_Cod <> -1 Then
                        strSql.AppendLine(" AND Zoo_Animali.Gen_Cod = " & Agro_SQL_SaveNum(Gen_Cod) & "   ")
                    End If

                    If Spe_Cod <> -1 Then
                        strSql.AppendLine(" AND Zoo_Animali.Spe_Cod = " & Agro_SQL_SaveNum(Spe_Cod) & "   ")
                    End If

                    If IPro_Cod <> -1 Then
                        strSql.AppendLine(" AND Zoo_Animali.IPro_Cod = " & Agro_SQL_SaveNum(IPro_Cod) & "   ")
                    End If

                    If Raz_Cod <> -1 Then
                        strSql.AppendLine(" AND Zoo_Animali.Raz_Cod = " & Agro_SQL_SaveNum(Raz_Cod) & "   ")
                    End If



                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Agenda.Inviato >=0 ")
                            strSql.AppendLine(" AND   Movimenti.Inviato >=0 ")
                            strSql.AppendLine(" AND   Movimenti_Dettagli.Inviato >=0 ")
                            strSql.AppendLine(" AND   Mov_Destinazioni.Inviato >=0 ")
                            strSql.AppendLine(" AND   Zoo_Animali.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Agenda.Inviato =-1 ")
                            strSql.AppendLine(" AND   Movimenti.Inviato =-1 ")
                            strSql.AppendLine(" AND   Movimenti_Dettagli.Inviato =-1 ")
                            strSql.AppendLine(" AND   Mov_Destinazioni.Inviato =-1 ")
                            strSql.AppendLine(" AND   Zoo_Animali.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Movimenti_Dettagli.Piva, Movimenti_Dettagli.Sa_Cod, Movimenti_Dettagli.Cod_Progetto Asc ")
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


    '###################################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Da usare con Selezione_JoinDescrizioni
    ''' </summary>
    ''' <param name="PIVA"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="Elem_Cod"></param>
    ''' <param name="Pro_Cod"></param>
    ''' <param name="Mat_Cod"></param>
    ''' <param name="Udm_Cod"></param>
    ''' <param name="Id_Destinazione"></param>
    ''' <param name="Cal_Cod"></param>
    ''' <param name="Cod_Progetto"></param>
    ''' <param name="Fase_Cod"></param>
    ''' <param name="Lotto"></param>
    ''' <param name="Gen_Cod"></param>
    ''' <param name="Spe_Cod"></param>
    ''' <param name="IPro_Cod"></param>
    ''' <param name="Raz_Cod"></param>
    ''' <param name="xSelezioneVariabile"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	20/01/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function LeggiConsistenzeRaggruppamenti(ByVal PIVA As String,
                                                   ByVal Sa_Cod As Integer,
                                                   ByVal Elem_Cod As Integer,
                                                   ByVal Pro_Cod As Integer,
                                                   ByVal Mat_Cod As Integer,
                                                   ByVal Udm_Cod As Integer,
                                                   ByVal Id_Destinazione As Integer,
                                                   ByVal Cal_Cod As Integer,
                                                   ByVal Cod_Progetto As Integer,
                                                   ByVal Fase_Cod As Integer,
                                                   ByVal Lotto As String,
                                                   ByVal Gen_Cod As Integer,
                                                   ByVal Spe_Cod As Integer,
                                                   ByVal IPro_Cod As Integer,
                                                   ByVal Raz_Cod As Integer,
                                                   ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                                   ByVal xFiltroAggiuntivo As String,
                                                   ByVal xOrderBy As String,
                                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                   ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Dettagli_R.LeggiConsistenzeRaggruppamenti()"


        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi




                Case enumSelezioneVariabile.Selezione_TabellaCompleta





                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                    strSql.Length = 0

                    strSql.AppendLine(" SELECT Distinct Zoo_Animali.Gen_Cod, Zoo_Animali.Spe_Cod, Zoo_Animali.IPro_Cod, Zoo_Animali.Raz_Cod   ")
                    strSql.AppendLine(" FROM  Agenda (NOLOCK) , Movimenti (NOLOCK) , Movimenti_Dettagli (NOLOCK) , Mov_Destinazioni (NOLOCK) ,  Zoo_Animali (NOLOCK) ")
                    strSql.AppendLine(" WHERE Movimenti_Dettagli.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    strSql.AppendLine(" AND   Agenda.Lav_Cod = -2    ")
                    strSql.AppendLine(" AND   Movimenti.Cau_Mov = 'ANIMALI'    ")
                    strSql.AppendLine(" AND   Mov_Destinazioni.Tipo_Destinazione = 15   ")
                    strSql.AppendLine(" AND   Zoo_Animali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND   Zoo_Animali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")


                    'Join sulla Piva
                    strSql.AppendLine(" AND   Agenda.Piva = Movimenti.Piva " &
                           " AND   Agenda.Piva = Movimenti_Dettagli.Piva " &
                           " AND   Agenda.Piva = Mov_Destinazioni.Piva " &
                           " AND   Movimenti.Piva = Movimenti_Dettagli.Piva " &
                           " AND   Movimenti.Piva = Mov_Destinazioni.Piva " &
                           " AND   Movimenti_Dettagli.Piva = Mov_Destinazioni.Piva ")

                    'Join sul Sa_Cod
                    strSql.AppendLine(" AND   Agenda.Sa_Cod = Movimenti.Sa_Cod " &
                           " AND   Movimenti_Dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod ")

                    'Join su Id_Agenda
                    strSql.AppendLine(" AND   Agenda.Id_Agenda = Movimenti.Id_Agenda " &
                           " AND   Movimenti.Id_Agenda = Movimenti_Dettagli.Id_Agenda " &
                           " AND   Movimenti_Dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda ")

                    'Join su Id_Mov
                    strSql.AppendLine(" AND   Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov " &
                           " AND   Movimenti_Dettagli.Id_Mov = Mov_Destinazioni.Id_Mov ")

                    'Join su Id_Mov_Det
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")


                    'Join su Zoo_Animali
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Cod_Progetto = Zoo_Animali.Cod_Progetto ")
                    '" AND   Movimenti_Dettagli.Piva = Zoo_Animali.Piva " & _


                    If PIVA <> "" Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Piva = '" & Agro_SQL_SaveText(PIVA) & "'   ")
                    End If

                    If Sa_Cod <> 0 Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Elem_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
                    End If

                    If Pro_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
                    End If

                    If Mat_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
                    End If

                    If Udm_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   ")
                    End If

                    If Id_Destinazione <> 0 Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "   ")
                    End If

                    If Cal_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "   ")
                    End If

                    If Cod_Progetto <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & "   ")
                    End If

                    If Fase_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   ")
                    End If

                    If Lotto <> "" Then
                        strSql.AppendLine(" AND Upper(Movimenti_Dettagli.Lotto) = '" & UCase(Agro_SQL_SaveText(Lotto)) & "'   ")
                    End If

                    If Gen_Cod <> -1 Then
                        strSql.AppendLine(" AND Zoo_Animali.Gen_Cod = " & Agro_SQL_SaveNum(Gen_Cod) & "   ")
                    End If

                    If Spe_Cod <> -1 Then
                        strSql.AppendLine(" AND Zoo_Animali.Spe_Cod = " & Agro_SQL_SaveNum(Spe_Cod) & "   ")
                    End If

                    If IPro_Cod <> -1 Then
                        strSql.AppendLine(" AND Zoo_Animali.IPro_Cod = " & Agro_SQL_SaveNum(IPro_Cod) & "   ")
                    End If

                    If Raz_Cod <> -1 Then
                        strSql.AppendLine(" AND Zoo_Animali.Raz_Cod = " & Agro_SQL_SaveNum(Raz_Cod) & "   ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Agenda.Inviato >=0 ")
                            strSql.AppendLine(" AND   Movimenti.Inviato >=0 ")
                            strSql.AppendLine(" AND   Movimenti_Dettagli.Inviato >=0 ")
                            strSql.AppendLine(" AND   Mov_Destinazioni.Inviato >=0 ")
                            strSql.AppendLine(" AND   Zoo_Animali.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Agenda.Inviato =-1 ")
                            strSql.AppendLine(" AND   Movimenti.Inviato =-1 ")
                            strSql.AppendLine(" AND   Movimenti_Dettagli.Inviato =-1 ")
                            strSql.AppendLine(" AND   Mov_Destinazioni.Inviato =-1 ")
                            strSql.AppendLine(" AND   Zoo_Animali.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Zoo_Animali.Gen_Cod, Zoo_Animali.Spe_Cod, Zoo_Animali.IPro_Cod, Zoo_Animali.Raz_Cod Asc ")
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


    Public Function Leggi_Singola(ByVal Piva As String,
                                  ByVal Sa_Cod As Integer,
                                  ByVal Id_Agenda As Integer,
                                  ByVal Id_Mov As Integer,
                                  ByVal Id_Mov_Det As Integer,
                                  ByVal Elem_Cod As Integer,
                                  ByVal Pro_Cod As Integer,
                                  ByVal Mat_Cod As Integer,
                                  ByVal Cod_Progetto As Integer,
                                  ByVal Fase_Cod As Integer,
                                  ByVal Lotto As String,
                                  ByVal Cal_Cod As Integer,
                                  ByVal Udm_Cod As Integer,
                                  ByVal FiltroAggiuntivo As String,
                                  ByVal Ordinamento As String,
                                  ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                  ByVal xFiltroAggiuntivo As String,
                                  ByVal xOrderBy As String,
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                  ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Dettagli_R.Leggi_Singola()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""            =>  si leggono tutti i Mov_Destinazioni
        '   Sa_Cod = 0           =>  si leggono tutti i Mov_Destinazioni dell'impresa
        '   Appezza = 0          =>  si leggono tutti i Mov_Destinazioni del centro aziendale
        '   Id_Destinazione = 0  =>  si leggono tutti i Mov_Destinazioni dell'appezzamento
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            '------------------------------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    strSql.Length = 0

                    strSql.AppendLine(" SELECT Movimenti_dettagli.* ")

                    strSql.AppendLine(" FROM Movimenti_dettagli ")

                    strSql.AppendLine(" WHERE   Movimenti_Dettagli.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND     Movimenti_Dettagli.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If

                    If Sa_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Id_Agenda <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
                    End If

                    If Id_Mov <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
                    End If

                    If Id_Mov_Det <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")
                    End If

                    If Elem_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
                    End If

                    If Pro_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
                    End If

                    If Mat_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
                    End If

                    If Cod_Progetto <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & "   ")
                    End If

                    If Fase_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   ")
                    End If

                    If Lotto <> "-999" Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Lotto = '" & Agro_SQL_SaveText(Lotto) & "'   ")
                    End If

                    If Cal_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "   ")
                    End If

                    If Udm_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Movimenti_Dettagli.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Movimenti_Dettagli.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))

                    End If



                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    strSql.Length = 0

                    strSql.AppendLine(" SELECT Movimenti_dettagli.* ")

                    strSql.AppendLine(" FROM Movimenti_dettagli ")

                    strSql.AppendLine(" WHERE   Movimenti_Dettagli.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND     Movimenti_Dettagli.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If

                    If Sa_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Id_Agenda <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
                    End If

                    If Id_Mov <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
                    End If

                    If Id_Mov_Det <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")
                    End If

                    If Elem_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
                    End If

                    If Pro_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
                    End If

                    If Mat_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
                    End If

                    If Cod_Progetto <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & "   ")
                    End If

                    If Fase_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   ")
                    End If

                    If Lotto <> "-999" Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Lotto = '" & Agro_SQL_SaveText(Lotto) & "'   ")
                    End If

                    If Cal_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "   ")
                    End If

                    If Udm_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Movimenti_Dettagli.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Movimenti_Dettagli.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))

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


    'Se Cau_Mov = "" -> leggo sia i carichi che gli scarichi
    Public Function Leggi_UDM_Utilizzate(ByVal PIVA As String,
                                         ByVal Sa_Cod As Integer,
                                         ByVal Id_Dest As Integer,
                                         ByVal Id_Agenda As Integer,
                                         ByVal Id_Mov As Integer,
                                         ByVal Id_Mov_Det As Integer,
                                         ByVal Elem_Cod As Integer,
                                         ByVal Pro_Cod As Integer,
                                         ByVal Mat_Cod As Integer,
                                         ByVal Udm_Cod As Integer,
                                         ByVal Cau_Mov As String,
                                         ByVal Cod_Progetto As Integer,
                                         ByVal Fase_Cod As Integer,
                                         ByVal Contabilizzato As Integer,
                                         ByVal Pendente As Integer,
                                         ByVal Lotto As String,
                                         ByVal Cal_Cod As Integer,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByVal xOrderBy As String,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                         ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Dettagli_R.Leggi_UDM_Utilizzate()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            '------------------------------------------------------------------
            strSql.Length = 0

            strSql.AppendLine("  SELECT DISTINCT Movimenti_Dettagli.Udm_Cod ")
            strSql.AppendLine("  FROM Agenda ")
            strSql.AppendLine("  Inner join Movimenti on Agenda.Id_Agenda=Movimenti.Id_Agenda ")
            strSql.AppendLine("  Inner join Movimenti_Dettagli on Movimenti.Id_Agenda=Movimenti_Dettagli.Id_Agenda AND Movimenti.Id_Mov=Movimenti_Dettagli.Id_Mov ")

            If Id_Dest <> 0 Then
                strSql.AppendLine("  LEFT JOIN [Mov_Destinazioni] ")
                strSql.AppendLine("   on Mov_Destinazioni.Id_Agenda=Movimenti_Dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov=Movimenti_Dettagli.Id_Mov AND  Mov_Destinazioni.Id_Mov_Det=Movimenti_Dettagli.Id_Mov_Det ")
            End If

            strSql.AppendLine("  Where 1=1 ")

            If Id_Dest <> 0 Then
                strSql.AppendLine("  AND Mov_Destinazioni.ID_DESTINAZIONE= " & Agro_SQL_SaveNum(Id_Dest) & "  ")
            End If

            If PIVA <> "" Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Piva = '" & Agro_SQL_SaveText(PIVA) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Id_Agenda <> 0 Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
            End If

            If Id_Mov <> 0 Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
            End If

            If Id_Mov_Det <> 0 Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")
            End If

            If Elem_Cod <> 0 Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
            End If

            If Pro_Cod <> 0 Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
            End If

            If Mat_Cod <> 0 Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
            End If

            If Udm_Cod <> 0 Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   ")
            End If

            If Cau_Mov <> "" Then
                strSql.AppendLine("  AND  Movimenti.Cau_Mov = '" & Agro_SQL_SaveText(Cau_Mov) & "'   ")
            Else

                'Sono state aggiunte le causali dei conferimenti
                strSql.AppendLine("  AND Movimenti.Cau_Mov IN ('7300', '7350', '4100', '4200', '7900', '7920')     ")
            End If

            If Cod_Progetto <> 0 Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & "   ")
            End If

            If Fase_Cod <> 0 Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   ")
            End If

            If Contabilizzato <> 0 Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Contabilizzato = " & Agro_SQL_SaveNum(Contabilizzato) & "   ")
            End If

            If Pendente <> 0 Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Pendente = " & Agro_SQL_SaveNum(Pendente) & "   ")
            End If

            If Cal_Cod <> 0 Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "   ")
            End If

            If Lotto <> "" Then
                strSql.AppendLine("  AND Upper(Movimenti_Dettagli.Lotto) = '" & UCase(Agro_SQL_SaveText(Lotto)) & "'   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine("  AND (" & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & ")")
            End If

            strSql.AppendLine("  ORDER BY Movimenti_Dettagli.Udm_Cod ")

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

    Public Function LeggiTracciabilita(ByVal Piva As String,
                                        ByVal Sa_Cod As Integer,
                                        ByVal Id_Agenda As Integer,
                                        ByVal Id_Mov As Integer,
                                        ByVal Id_Mov_Det As Integer,
                                        ByVal Elem_Cod As Integer,
                                        ByVal Pro_Cod As Integer,
                                        ByVal Mat_Cod As Integer,
                                        ByVal Udm_Cod As Integer,
                                        ByVal Appezza As Integer,
                                        ByVal Id_Destinazione As Integer,
                                        ByVal Tipo_Destinazione As Integer,
                                        ByVal Cau_Mov As String,
                                        ByVal Cod_Progetto As Integer,
                                        ByVal Fase_Cod As Integer,
                                        ByVal Contabilizzato As Integer,
                                        ByVal Pendente As Integer,
                                        ByVal Cal_Cod As Integer,
                                        ByVal Lotto As String,
                                        ByVal Validita_Inizio As Date,
                                        ByVal Validita_Fine As Date,
                                        ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Dettagli_R.LeggiTracciabilita()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    strSql.Length = 0

                    strSql.AppendLine(" SELECT Agenda.* , Movimenti.* , Movimenti_Dettagli.* , Mov_Destinazioni.* , Mov_Destinazioni.Qta as Dest_Qta , Movimenti_Dettagli.Qta as Dett_Qta, Movimenti_Dettagli.Extra_str as Dett_Extra_str  " & vbCrLf &
                                    " FROM  Agenda , Movimenti , Movimenti_Dettagli , Mov_Destinazioni " & vbCrLf &
                                    " WHERE Movimenti_Dettagli.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " " & vbCrLf &
                                    " AND   Movimenti_Dettagli.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " " & vbCrLf &
                                    " AND   Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine) & " " & vbCrLf &
                                    " AND   Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio) & vbCrLf &
                                    " AND   Agenda.Piva = Movimenti.Piva " & vbCrLf &
                                    " AND   Agenda.Piva = Movimenti_Dettagli.Piva " & vbCrLf &
                                    " AND   Agenda.Piva = Mov_Destinazioni.Piva " & vbCrLf &
                                    " AND   Movimenti.Piva = Movimenti_Dettagli.Piva " & vbCrLf &
                                    " AND   Movimenti.Piva = Mov_Destinazioni.Piva " & vbCrLf &
                                    " AND   Movimenti_Dettagli.Piva = Mov_Destinazioni.Piva " & vbCrLf &
                                    " AND   Agenda.Sa_Cod = Movimenti.Sa_Cod " & vbCrLf &
                                    " AND   Movimenti_Dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod " & vbCrLf &
                                    " AND   Agenda.Id_Agenda = Movimenti.Id_Agenda " & vbCrLf &
                                    " AND   Agenda.Id_Agenda = Movimenti_Dettagli.Id_Agenda " & vbCrLf &
                                    " AND   Agenda.Id_Agenda = Mov_Destinazioni.Id_Agenda " & vbCrLf &
                                    " AND   Movimenti.Id_Agenda = Movimenti_Dettagli.Id_Agenda " & vbCrLf & vbCrLf &
                                    " AND   Movimenti.Id_Agenda = Mov_Destinazioni.Id_Agenda " & vbCrLf &
                                    " AND   Movimenti_Dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda " & vbCrLf &
                                    " AND   Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov " & vbCrLf &
                                    " AND   Movimenti.Id_Mov = Mov_Destinazioni.Id_Mov " & vbCrLf &
                                    " AND   Movimenti_Dettagli.Id_Mov = Mov_Destinazioni.Id_Mov " & vbCrLf &
                                    " AND   Movimenti_Dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")


                    If Piva <> "" Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If

                    If Sa_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Id_Agenda <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
                    End If

                    If Id_Mov <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
                    End If

                    If Id_Mov_Det <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")
                    End If

                    If Elem_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
                    End If

                    If Pro_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
                    End If

                    If Mat_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
                    End If

                    If Udm_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   ")
                    End If

                    If Appezza <> 0 Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
                    End If

                    If Id_Destinazione <> 0 Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "   ")
                    End If

                    If Tipo_Destinazione <> 0 Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Tipo_Destinazione = " & Agro_SQL_SaveNum(Tipo_Destinazione) & "   ")
                    End If

                    If Cau_Mov <> "" Then
                        strSql.AppendLine(" AND  Movimenti.Cau_Mov = '" & Agro_SQL_SaveText(Cau_Mov) & "'   ")
                    End If

                    If Cod_Progetto <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & "   ")
                    End If

                    If Fase_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   ")
                    End If

                    If Contabilizzato <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Contabilizzato = " & Agro_SQL_SaveNum(Contabilizzato) & "   ")
                    End If

                    If Pendente <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Pendente = " & Agro_SQL_SaveNum(Pendente) & "   ")
                    End If

                    If Cal_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "   ")
                    End If

                    If Lotto <> "" Then
                        strSql.AppendLine(" AND Upper(Movimenti_Dettagli.Lotto) = '" & UCase(Agro_SQL_SaveText(Lotto)) & "'   ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Movimenti_Dettagli.Elem_Cod Asc ")
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


    Public Function Leggi_ProCod_Utilizzati(ByVal PIVA As String,
                                         ByVal Sa_Cod As Integer,
                                         ByVal Id_Dest As Integer,
                                         ByVal Id_Agenda As Integer,
                                         ByVal Id_Mov As Integer,
                                         ByVal Id_Mov_Det As Integer,
                                         ByVal Elem_Cod As Integer,
                                         ByVal Pro_Cod As Integer,
                                         ByVal Mat_Cod As Integer,
                                         ByVal Udm_Cod As Integer,
                                         ByVal Cau_Mov As String,
                                         ByVal Lotto As String,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByVal xOrderBy As String,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                         ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Dettagli_R.Leggi_ProCod_Utilizzati()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            '------------------------------------------------------------------
            strSql.Length = 0

            strSql.AppendLine("  SELECT DISTINCT Movimenti_Dettagli.Pro_Cod ")
            strSql.AppendLine("  FROM Movimenti Inner join Movimenti_Dettagli on Movimenti.Id_Agenda=Movimenti_Dettagli.Id_Agenda AND Movimenti.Id_Mov=Movimenti_Dettagli.Id_Mov ")

            If Id_Dest <> 0 Then
                strSql.AppendLine("  LEFT JOIN [Mov_Destinazioni] ")
                strSql.AppendLine("   on Mov_Destinazioni.Id_Agenda=Movimenti_Dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov=Movimenti_Dettagli.Id_Mov AND  Mov_Destinazioni.Id_Mov_Det=Movimenti_Dettagli.Id_Mov_Det ")
            End If

            strSql.AppendLine("  Where 1=1 ")

            If Id_Dest <> 0 Then
                strSql.AppendLine("  AND Mov_Destinazioni.ID_DESTINAZIONE= " & Agro_SQL_SaveNum(Id_Dest) & "  ")
            End If

            If PIVA <> "" Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Piva = '" & Agro_SQL_SaveText(PIVA) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Id_Agenda <> 0 Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
            End If

            If Id_Mov <> 0 Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
            End If

            If Id_Mov_Det <> 0 Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")
            End If

            If Elem_Cod <> 0 Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
            End If

            If Pro_Cod <> 0 Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
            End If

            If Mat_Cod <> 0 Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
            End If

            If Udm_Cod <> 0 Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   ")
            End If

            If Cau_Mov <> "" Then
                strSql.AppendLine("  AND  Movimenti.Cau_Mov = '" & Agro_SQL_SaveText(Cau_Mov) & "'   ")
            Else
                'Sono state aggiunte le causali dei conferimenti
                strSql.AppendLine("  AND Movimenti.Cau_Mov IN ('7300', '7350', '4100', '4200', '7900', '7920')     ")
            End If

            If Lotto <> "" Then
                strSql.AppendLine("  AND Upper(Movimenti_Dettagli.Lotto) = '" & UCase(Agro_SQL_SaveText(Lotto)) & "'   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine("  AND (" & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & ")")
            End If

            strSql.AppendLine("  ORDER BY Movimenti_Dettagli.Pro_Cod ")

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



    Public Function Leggi_Macroelementi_Distribuiti(ByRef N_Distribuito_Ha As Decimal,
                                                    ByRef P_Distribuito_Ha As Decimal,
                                                    ByRef K_Distribuito_Ha As Decimal,
                                                    ByRef Mg_Distribuito_Ha As Decimal,
                                                    ByRef Cu_Distribuito_Ha As Decimal,
                                                    ByRef N_Distribuito_Imp As Decimal,
                                                    ByRef P_Distribuito_Imp As Decimal,
                                                    ByRef K_Distribuito_Imp As Decimal,
                                                    ByRef Mg_Distribuito_Imp As Decimal,
                                                    ByRef Cu_Distribuito_Imp As Decimal,
                                                    ByVal Piva As String,
                                                    ByVal Sa_Cod As Integer,
                                                    ByVal Appezza As Integer,
                                                    ByVal Id_Reg As Integer,
                                                    ByVal Progetto_Cod As Integer,
                                                    ByVal Validita_Inizio As Date,
                                                    ByVal Validita_Fine As Date,
                                                    ByVal Id_Agenda_Escluso As Integer,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                        Optional Raccoglitore_Cod_Escluso As Integer = 0
                                                    ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Dettagli_R.Leggi_Macroelementi_Distribuiti()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable
        Dim supTratt As Decimal
        Dim supImp As Decimal

        Try
            '---------------------------------------------


            strSql.Length = 0

            strSql.AppendLine(" SELECT DISTINCT ")
            strSql.AppendLine(" fertilizzanti.fer_cod, fertilizzanti.FER_DES, ")
            strSql.AppendLine(" ISNULL(fertilizzanti.N,0) AS N, ISNULL(fertilizzanti.P2O5,0) AS P2O5, ISNULL(fertilizzanti.K2O,0) AS K2O, ISNULL(fertilizzanti.MgO,0) AS MgO, ISNULL(fertilizzanti.Cu,0) AS Cu,")
            strSql.AppendLine(" Agenda.des_lib, Imprese_Progetti.Progetto_Nome as Lotto, ")
            strSql.AppendLine(" Movimenti.Data_Movimento as Data_Creazione, dbo.Movimenti.Mov_Desc, ")
            strSql.AppendLine(" Movimenti.Mezzo, Mov_Dettaglio_Tecnico.Qta_Ril, unitamisura.UDM_DES, ")
            strSql.AppendLine(" Cultivar.CUL_DES, Reg_Impianti.sup_imp, Mov_Destinazioni.Qta, ")
            strSql.AppendLine(" Mov_Destinazioni.Qta2, ")
            strSql.AppendLine(" Operazioni.lav_cod, Operazioni.lav_des, Movimenti_dettagli.UDM_COD, Movimenti_dettagli.Extra_Int, ")
            strSql.AppendLine(" Reg_Impianti.ID_REG, Agenda.Id_Agenda, ISNULL(Raccoglitore_Cod, 0) AS Raccoglitore_Cod,  ")
            strSql.AppendLine(" ISNULL(Mov_Dettaglio_Tecnico.N,0) AS N_Tecnico, ")
            strSql.AppendLine(" ISNULL(Mov_Dettaglio_Tecnico.K,0) AS K2O_Tecnico, ")
            strSql.AppendLine(" ISNULL(Mov_Dettaglio_Tecnico.P,0) AS P2O5_Tecnico, ")
            strSql.AppendLine(" ISNULL(Mov_Dettaglio_Tecnico.Mg,0) AS MgO_Tecnico, ")
            strSql.AppendLine(" ISNULL(Mov_Dettaglio_Tecnico.Efficienza,1) AS Efficienza, ")
            strSql.AppendLine(" ISNULL(Mov_Dettaglio_Tecnico.Cu,0) AS Cu_Tecnico ")

            strSql.AppendLine(" FROM Mov_Dettaglio_Tecnico RIGHT OUTER JOIN ")
            strSql.AppendLine(" Reg_Impianti INNER JOIN ")
            strSql.AppendLine(" Mov_Destinazioni ON Reg_Impianti.PIVA = Mov_Destinazioni.Piva AND Reg_Impianti.SA_COD = Mov_Destinazioni.Sa_Cod AND ")
            strSql.AppendLine(" Reg_Impianti.ID_REG = Mov_Destinazioni.Id_Destinazione AND Reg_Impianti.APPEZZA = Mov_Destinazioni.Appezza ")
            strSql.AppendLine(" LEFT OUTER JOIN Imprese_Progetti On Imprese_Progetti.Piva = Reg_Impianti.Piva AND Imprese_Progetti.Sa_Cod = Reg_Impianti.Sa_Cod AND Imprese_Progetti.Appezza = Reg_Impianti.Appezza AND Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg ")
            strSql.AppendLine(" INNER JOIN Operazioni INNER JOIN ")
            strSql.AppendLine(" Agenda INNER JOIN ")
            strSql.AppendLine(" Movimenti ON Agenda.Id_Agenda = Movimenti.Id_Agenda AND Agenda.PIVA = Movimenti.PIVA AND ")
            strSql.AppendLine(" Agenda.Sa_Cod = Movimenti.Sa_Cod INNER JOIN ")
            strSql.AppendLine(" Movimenti_dettagli ON Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov AND Movimenti.PIVA = Movimenti_dettagli.PIVA AND ")
            strSql.AppendLine(" Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda INNER JOIN ")
            strSql.AppendLine(" Fertilizzanti fertilizzanti ON fertilizzanti.Fer_Cod = Movimenti_dettagli.Pro_Cod INNER JOIN ")
            strSql.AppendLine(" UnitaMisura unitamisura ON Movimenti_dettagli.Udm_Cod = unitamisura.UDM_COD ON Operazioni.LAV_COD = Agenda.Lav_Cod ON ")
            strSql.AppendLine(" Mov_Destinazioni.Piva = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Sa_Cod = Movimenti_dettagli.Sa_Cod AND ")
            strSql.AppendLine(" Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND ")
            strSql.AppendLine(" Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det ON Mov_Dettaglio_Tecnico.Piva = Movimenti_dettagli.PIVA AND ")
            strSql.AppendLine(" Mov_Dettaglio_Tecnico.Sa_Cod = Movimenti_dettagli.Sa_Cod AND Mov_Dettaglio_Tecnico.Id_Agenda = Movimenti_dettagli.Id_Agenda AND ")
            strSql.AppendLine(" Mov_Dettaglio_Tecnico.Id_Mov = Movimenti_dettagli.Id_Mov AND Mov_Dettaglio_Tecnico.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det ")
            strSql.AppendLine(" LEFT OUTER JOIN SpecieVegetali INNER JOIN Cultivar ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod  ")
            strSql.AppendLine(" ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod ")

            '14 = concimazione in pieno campo
            '26 = fertirrigazione
            '106 = trattamento antibutteratura 
            '123 = concimazione fogliare
            '124 = distribuzione ammendanti organici o liquami
            '156 = sarchiatura con concimazione
            strSql.AppendLine("WHERE Agenda.Lav_Cod IN (14, 26, 106, 123, 124, 156) ")

            If Piva <> "" Then
                strSql.AppendLine(" AND Mov_Destinazioni.PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            End If

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            End If

            If Appezza <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Appezza = " & Agro_SQL_SaveNum(Appezza) & "  ")
            End If

            If Id_Reg <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Id_Destinazione =  " & Agro_SQL_SaveNum(Id_Reg) & " ")
            End If

            If Progetto_Cod <> 0 Then
                strSql.AppendLine(" AND Imprese_Progetti.Progetto_Cod =" & Agro_SQL_SaveNum(Progetto_Cod))
            End If

            strSql.AppendLine(" AND Movimenti.Data_Movimento >=" & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.AppendLine(" AND Movimenti.Data_Movimento <=" & Agro_SQL_SaveDate(Validita_Fine))
            strSql.AppendLine(" AND Imprese_Progetti.validita_fine >=" & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.AppendLine(" AND Imprese_Progetti.validita_inizio <=" & Agro_SQL_SaveDate(Validita_Fine))



            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Mov_Destinazioni.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Mov_Destinazioni.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            strSql.AppendLine(" ORDER BY Data_Movimento DESC ")


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If Not IsNothing(dt) Then

                Dim N_Prodotto As Decimal
                Dim P_Prodotto As Decimal
                Dim K_Prodotto As Decimal
                Dim Mg_Prodotto As Decimal
                Dim Cu_Prodotto As Decimal
                Dim i As Integer

                For i = 0 To dt.Rows.Count - 1

                    'Controllo che l'Id_Agenda non sia già stato verificato
                    If CInt(dt.Rows(i).Item("Id_Agenda")) <> Id_Agenda_Escluso AndAlso
                        CInt(dt.Rows(i).Item("Raccoglitore_Cod")) <> If(Raccoglitore_Cod_Escluso = 0, -999, Raccoglitore_Cod_Escluso) Then

                        'Mi baso solo sugli apporti salvati nel dettaglio tecnico come viene fatto nel Report Sostenibilità

                        N_Prodotto = CDec(dt.Rows(i).Item("N_Tecnico"))

                        P_Prodotto = CDec(dt.Rows(i).Item("P2O5_Tecnico"))

                        K_Prodotto = CDec(dt.Rows(i).Item("K2O_Tecnico"))

                        Mg_Prodotto = CDec(dt.Rows(i).Item("MgO_Tecnico"))

                        Cu_Prodotto = CDec(dt.Rows(i).Item("Cu_Tecnico"))

                        'Aggiornamenti Macroelementi Distribuiti

                        supImp = CDec(dt.Rows(i).Item("Sup_Imp"))

                        supTratt = 0
                        supTratt = CDec(dt.Rows(i).Item("Qta2"))
                        If supTratt = 0 Then
                            supTratt = supImp
                        End If

                        If CDec(dt.Rows(i).Item("efficienza")) < 1 Then
                            N_Distribuito_Ha += CDec(dt.Rows(i).Item("Qta")) / supTratt * N_Prodotto / 100 * CDec(dt.Rows(i).Item("efficienza"))
                            N_Distribuito_Imp += CDec(dt.Rows(i).Item("Qta")) * N_Prodotto / 100 * CDec(dt.Rows(i).Item("efficienza"))
                        Else
                            N_Distribuito_Ha += CDec(dt.Rows(i).Item("Qta")) * N_Prodotto / 100 / supTratt
                            N_Distribuito_Imp += CDec(dt.Rows(i).Item("Qta")) * N_Prodotto / 100
                        End If

                        P_Distribuito_Ha += CDec(dt.Rows(i).Item("Qta")) * P_Prodotto / 100 / supTratt
                        K_Distribuito_Ha += CDec(dt.Rows(i).Item("Qta")) * K_Prodotto / 100 / supTratt
                        Mg_Distribuito_Ha += CDec(dt.Rows(i).Item("Qta")) * Mg_Prodotto / 100 / supTratt
                        Cu_Distribuito_Ha += CDec(dt.Rows(i).Item("Qta")) * Cu_Prodotto / 100 / supTratt

                        P_Distribuito_Imp += CDec(dt.Rows(i).Item("Qta")) * P_Prodotto / 100
                        K_Distribuito_Imp += CDec(dt.Rows(i).Item("Qta")) * K_Prodotto / 100
                        Mg_Distribuito_Imp += CDec(dt.Rows(i).Item("Qta")) * Mg_Prodotto / 100
                        Cu_Distribuito_Imp += CDec(dt.Rows(i).Item("Qta")) * Cu_Prodotto / 100

                    End If

                Next

            End If

            Return dt

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

            Return Nothing

        End Try

    End Function


    'Public Function Leggi_Macroelementi_Distribuiti_suDistinte_DaIdAgenda(ByVal Piva As String, ByVal ID_Agenda As Integer,
    '                                                                      ByVal IDTestataTemp__tmp_FormulatiXPrincipiAttivi As Integer,
    '                                                            ByVal xFiltroAggiuntivo As String,
    '                                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    '                                                                ) As DataTable

    '    Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Dettagli_R.Leggi_Macroelementi_Distribuiti_suDistinte_DaIdAgenda()"

    '    Dim messaggioErrore As String = ""
    '    Dim strSql As New StringBuilder
    '    Dim dt As DataTable

    '    Try

    '        '----------------------------------------

    '        strSql.Append(" Select " & vbCrLf)

    '        strSql.Append(" fertilizzazioni.*, Cu_Distribuito_Trattamenti_Ha  " & vbCrLf)

    '        strSql.Append(" from  " & vbCrLf)
    '        strSql.Append(" (  " & vbCrLf)


    '        '----------------------------------------
    '        strSql.Append(" Select " & vbCrLf)

    '        strSql.Append(" tutte.Piva, tutte.Sa_Cod, tutte.Appezza, tutte.Id_Reg, tutte.Progetto_Cod, APP_NOME, GRFI_COD, Regolamento_Concimazioni_Cod, " & vbCrLf)
    '        strSql.Append(" N_Massimo, P_Massimo, K_Massimo, Mg_massimo,  " & vbCrLf)
    '        strSql.Append(" N_Distribuito_Ha, P_Distribuito_Ha, K_Distribuito_Ha, Mg_Distribuito_Ha, Cu_Distribuito_Ha,  " & vbCrLf)
    '        strSql.Append(" N_Operazione_Ha, P_Operazione_Ha, K_Operazione_Ha, Mg_Operazione_Ha, Cu_Operazione_Ha  " & vbCrLf)

    '        strSql.Append(" from  " & vbCrLf)
    '        strSql.Append(" (  " & vbCrLf)

    '        strSql.Append(" Select imprese_progetti.Piva, imprese_progetti.sa_cod,imprese_progetti.appezza,imprese_progetti.id_reg, imprese_progetti.progetto_cod, Appezzamento.App_Nome, Reg_Impianti.Grfi_cod, ISNULL(Imprese_Progetti.Regolamento_Concimazioni_Cod,0) AS Regolamento_Concimazioni_Cod " & vbCrLf)

    '        '--- DISTRIBUITI ---

    '        strSql.Append(" , sum(Mov_Destinazioni.Qta / mov_destinazioni.qta2 * ISNULL(Mov_Dettaglio_Tecnico.N, 0) / 100 * ISNULL(Mov_Dettaglio_Tecnico.efficienza, 1)) As N_Distribuito_Ha " & vbCrLf)
    '        strSql.Append(" , sum(Mov_Destinazioni.Qta / mov_destinazioni.qta2 * ISNULL(Mov_Dettaglio_Tecnico.P, 0) / 100) As P_Distribuito_Ha  " & vbCrLf)
    '        strSql.Append(" , sum(Mov_Destinazioni.Qta / mov_destinazioni.qta2 * ISNULL(Mov_Dettaglio_Tecnico.K, 0) / 100) As K_Distribuito_Ha  " & vbCrLf)
    '        strSql.Append(" , sum(Mov_Destinazioni.Qta / mov_destinazioni.qta2 * ISNULL(Mov_Dettaglio_Tecnico.Mg, 0) / 100) As Mg_Distribuito_Ha  " & vbCrLf)
    '        strSql.Append(" , sum(Mov_Destinazioni.Qta / mov_destinazioni.qta2 * ISNULL(Mov_Dettaglio_Tecnico.Cu, 0) / 100) As Cu_Distribuito_Ha  " & vbCrLf)

    '        '--- LIMITI APPORTI ---
    '        strSql.Append("   ,  ISNULL ((Select Reg_Impianti_Codici.val_cod " & vbCrLf)
    '        strSql.Append("         FROM   Reg_Impianti_Codici " & vbCrLf)
    '        strSql.Append("         WHERE  Piva = Imprese_Progetti.PIVA AND sa_cod = Imprese_Progetti.sa_cod " & vbCrLf)
    '        strSql.Append("         AND appezza = Imprese_Progetti.appezza AND Id_Reg = Imprese_Progetti.id_reg " & vbCrLf)
    '        strSql.Append("         AND Progetto_Cod = Imprese_Progetti.Progetto_Cod  " & vbCrLf)
    '        strSql.Append("         AND Reg_Impianti_Codici.id_cod=1050 " & vbCrLf)
    '        strSql.Append("         ), '') AS N_Massimo " & vbCrLf)

    '        strSql.Append("   ,  ISNULL ((Select Reg_Impianti_Codici.val_cod " & vbCrLf)
    '        strSql.Append("         FROM   Reg_Impianti_Codici " & vbCrLf)
    '        strSql.Append("         WHERE  Piva = Imprese_Progetti.PIVA AND sa_cod = Imprese_Progetti.sa_cod " & vbCrLf)
    '        strSql.Append("         AND appezza = Imprese_Progetti.appezza AND Id_Reg = Imprese_Progetti.id_reg " & vbCrLf)
    '        strSql.Append("         AND Progetto_Cod = Imprese_Progetti.Progetto_Cod  " & vbCrLf)
    '        strSql.Append("         AND Reg_Impianti_Codici.id_cod=1051 " & vbCrLf)
    '        strSql.Append("         ), '') AS P_Massimo " & vbCrLf)

    '        strSql.Append("   ,  ISNULL ((Select Reg_Impianti_Codici.val_cod " & vbCrLf)
    '        strSql.Append("         FROM   Reg_Impianti_Codici " & vbCrLf)
    '        strSql.Append("         WHERE  Piva = Imprese_Progetti.PIVA AND sa_cod = Imprese_Progetti.sa_cod " & vbCrLf)
    '        strSql.Append("         AND appezza = Imprese_Progetti.appezza AND Id_Reg = Imprese_Progetti.id_reg " & vbCrLf)
    '        strSql.Append("         AND Progetto_Cod = Imprese_Progetti.Progetto_Cod  " & vbCrLf)
    '        strSql.Append("         AND Reg_Impianti_Codici.id_cod=1052 " & vbCrLf)
    '        strSql.Append("         ), '') AS K_Massimo " & vbCrLf)

    '        strSql.Append("   ,  ISNULL ((Select Reg_Impianti_Codici.val_cod " & vbCrLf)
    '        strSql.Append("         FROM   Reg_Impianti_Codici " & vbCrLf)
    '        strSql.Append("         WHERE  Piva = Imprese_Progetti.PIVA AND sa_cod = Imprese_Progetti.sa_cod " & vbCrLf)
    '        strSql.Append("         AND appezza = Imprese_Progetti.appezza AND Id_Reg = Imprese_Progetti.id_reg " & vbCrLf)
    '        strSql.Append("         AND Progetto_Cod = Imprese_Progetti.Progetto_Cod  " & vbCrLf)
    '        strSql.Append("         AND Reg_Impianti_Codici.id_cod=1053 " & vbCrLf)
    '        strSql.Append("         ), '') AS Mg_Massimo " & vbCrLf)


    '        strSql.Append(" FROM Mov_Destinazioni  " & vbCrLf)

    '        strSql.Append(" INNER JOIN Reg_Impianti " & vbCrLf)
    '        strSql.Append(" ON Reg_Impianti.PIVA = Mov_Destinazioni.Piva AND Reg_Impianti.SA_COD = Mov_Destinazioni.Sa_Cod  " & vbCrLf)
    '        strSql.Append(" AND Reg_Impianti.APPEZZA = Mov_Destinazioni.Appezza AND Reg_Impianti.ID_REG = Mov_Destinazioni.Id_Destinazione " & vbCrLf)

    '        strSql.Append(" LEFT OUTER JOIN Imprese_Progetti  " & vbCrLf)
    '        strSql.Append(" ON Imprese_Progetti.Piva = Reg_Impianti.Piva AND Imprese_Progetti.Sa_Cod = Reg_Impianti.Sa_Cod " & vbCrLf)
    '        strSql.Append(" AND Imprese_Progetti.Appezza = Reg_Impianti.Appezza AND Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg  " & vbCrLf)

    '        strSql.Append(" INNER JOIN Appezzamento " & vbCrLf)
    '        strSql.Append(" ON Reg_Impianti.PIVA = Appezzamento.Piva AND Reg_Impianti.SA_COD = Appezzamento.Sa_Cod  " & vbCrLf)
    '        strSql.Append(" AND Reg_Impianti.APPEZZA = Appezzamento.Appezza " & vbCrLf)

    '        strSql.Append(" INNER JOIN Movimenti_dettagli " & vbCrLf)
    '        strSql.Append(" ON Mov_Destinazioni.Piva = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Sa_Cod = Movimenti_dettagli.Sa_Cod  " & vbCrLf)
    '        strSql.Append(" AND Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det " & vbCrLf)

    '        strSql.Append(" INNER JOIN Movimenti " & vbCrLf)
    '        strSql.Append(" ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod   " & vbCrLf)
    '        strSql.Append(" AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov " & vbCrLf)

    '        strSql.Append(" INNER JOIN Agenda " & vbCrLf)
    '        strSql.Append(" ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda AND  Agenda.Sa_Cod = Movimenti.Sa_Cod  " & vbCrLf)

    '        strSql.Append(" LEFT OUTER JOIN Mov_Dettaglio_Tecnico  " & vbCrLf)
    '        strSql.Append(" ON Mov_Dettaglio_Tecnico.Piva = Movimenti_dettagli.PIVA AND Mov_Dettaglio_Tecnico.Sa_Cod = Movimenti_dettagli.Sa_Cod AND Mov_Dettaglio_Tecnico.Id_Agenda = Movimenti_dettagli.Id_Agenda AND " & vbCrLf)
    '        strSql.Append(" Mov_Dettaglio_Tecnico.Id_Mov = Movimenti_dettagli.Id_Mov AND Mov_Dettaglio_Tecnico.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det " & vbCrLf)

    '        strSql.Append("         inner Join ( " & vbCrLf)
    '        strSql.Append("             select distinct ip.Piva, ip.Sa_Cod, ip.Appezza, ip.Id_reg , ip.Progetto_Cod,  ip.validita_inizio, ip.validita_fine  " & vbCrLf)
    '        strSql.Append("             from mov_destinazioni d " & vbCrLf)
    '        strSql.Append("             inner join movimenti m on m.piva=d.piva AND m.id_agenda=d.id_agenda AND m.Id_Mov=d.id_mov " & vbCrLf)
    '        strSql.Append("             inner join imprese_progetti ip on d.piva=ip.Piva AND ip.Sa_Cod=d.Sa_Cod AND ip.Appezza=d.Appezza AND ip.Id_Reg=d.Id_Destinazione " & vbCrLf)
    '        strSql.Append("             where d.id_Agenda=" & Agro_SQL_SaveNum(ID_Agenda) & " " & vbCrLf)
    '        strSql.Append("             AND   d.Piva ='" & Agro_SQL_SaveText(Piva) & "' ")
    '        strSql.Append("             AND Tipo_Destinazione=0 " & vbCrLf)
    '        strSql.Append("             AND m.Data_Movimento >= ip.Validita_Inizio " & vbCrLf)
    '        strSql.Append("             AND m.Data_Movimento <= ip.Validita_fine " & vbCrLf)
    '        strSql.Append("         ) destinazioni_operazione   " & vbCrLf)
    '        strSql.Append("         On Imprese_Progetti.piva= destinazioni_operazione.piva  " & vbCrLf)
    '        strSql.Append("         AND Imprese_Progetti.Sa_Cod=destinazioni_operazione.Sa_Cod  " & vbCrLf)
    '        strSql.Append("         AND Imprese_Progetti.Appezza=destinazioni_operazione.Appezza  " & vbCrLf)
    '        strSql.Append("         AND Imprese_Progetti.Id_Reg=destinazioni_operazione.id_reg " & vbCrLf)
    '        strSql.Append("         AND Imprese_Progetti.progetto_cod=destinazioni_operazione.Progetto_Cod " & vbCrLf)
    '        strSql.Append("         AND Movimenti.Data_Movimento >= destinazioni_operazione.Validita_Inizio " & vbCrLf)
    '        strSql.Append("         AND Movimenti.Data_Movimento <= destinazioni_operazione.Validita_fine " & vbCrLf)

    '        strSql.Append(" WHERE Movimenti_dettagli.Elem_Cod = 3 " & vbCrLf)
    '        strSql.Append(" AND Movimenti.Cau_Mov = '2300' " & vbCrLf)
    '        strSql.Append(" AND   Movimenti.Piva ='" & Agro_SQL_SaveText(Piva) & "' ")

    '        If xFiltroAggiuntivo <> "" Then
    '            StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
    '        End If

    '        strSql.Append(" GROUP BY imprese_progetti.Piva, imprese_progetti.Sa_Cod, imprese_progetti.Appezza, imprese_progetti.Id_reg, imprese_progetti.progetto_cod, App_Nome, Reg_Impianti.Grfi_cod, Imprese_Progetti.Regolamento_Concimazioni_Cod ")

    '        strSql.Append("  ) tutte " & vbCrLf)

    '        strSql.Append(" inner Join " & vbCrLf)

    '        strSql.Append(" ( " & vbCrLf)

    '        strSql.Append(" Select imprese_progetti.Piva, imprese_progetti.sa_cod,imprese_progetti.appezza,imprese_progetti.id_reg, imprese_progetti.progetto_cod  " & vbCrLf)

    '        '--- DISTRIBUITI ---

    '        strSql.Append(" , sum(Mov_Destinazioni.Qta / mov_destinazioni.qta2 * ISNULL(Mov_Dettaglio_Tecnico.N, 0) / 100 * ISNULL(Mov_Dettaglio_Tecnico.efficienza, 1)) As N_Operazione_Ha " & vbCrLf)
    '        strSql.Append(" , sum(Mov_Destinazioni.Qta / mov_destinazioni.qta2 * ISNULL(Mov_Dettaglio_Tecnico.P, 0) / 100) As P_Operazione_Ha  " & vbCrLf)
    '        strSql.Append(" , sum(Mov_Destinazioni.Qta / mov_destinazioni.qta2 * ISNULL(Mov_Dettaglio_Tecnico.K, 0) / 100) As K_Operazione_Ha  " & vbCrLf)
    '        strSql.Append(" , sum(Mov_Destinazioni.Qta / mov_destinazioni.qta2 * ISNULL(Mov_Dettaglio_Tecnico.Mg, 0) / 100) As Mg_Operazione_Ha  " & vbCrLf)
    '        strSql.Append(" , sum(Mov_Destinazioni.Qta / mov_destinazioni.qta2 * ISNULL(Mov_Dettaglio_Tecnico.Cu, 0) / 100) As Cu_Operazione_Ha  " & vbCrLf)

    '        strSql.Append(" FROM Mov_Destinazioni  " & vbCrLf)

    '        strSql.Append(" INNER JOIN Reg_Impianti " & vbCrLf)
    '        strSql.Append(" ON Reg_Impianti.PIVA = Mov_Destinazioni.Piva AND Reg_Impianti.SA_COD = Mov_Destinazioni.Sa_Cod  " & vbCrLf)
    '        strSql.Append(" AND Reg_Impianti.APPEZZA = Mov_Destinazioni.Appezza AND Reg_Impianti.ID_REG = Mov_Destinazioni.Id_Destinazione " & vbCrLf)

    '        strSql.Append(" LEFT OUTER JOIN Imprese_Progetti  " & vbCrLf)
    '        strSql.Append(" ON Imprese_Progetti.Piva = Reg_Impianti.Piva AND Imprese_Progetti.Sa_Cod = Reg_Impianti.Sa_Cod " & vbCrLf)
    '        strSql.Append(" AND Imprese_Progetti.Appezza = Reg_Impianti.Appezza AND Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg  " & vbCrLf)

    '        strSql.Append(" INNER JOIN Movimenti_dettagli " & vbCrLf)
    '        strSql.Append(" ON Mov_Destinazioni.Piva = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Sa_Cod = Movimenti_dettagli.Sa_Cod  " & vbCrLf)
    '        strSql.Append(" AND Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det " & vbCrLf)

    '        strSql.Append(" INNER JOIN Movimenti " & vbCrLf)
    '        strSql.Append(" ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod   " & vbCrLf)
    '        strSql.Append(" AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov " & vbCrLf)

    '        strSql.Append(" INNER JOIN Agenda " & vbCrLf)
    '        strSql.Append(" ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda AND  Agenda.Sa_Cod = Movimenti.Sa_Cod  " & vbCrLf)

    '        strSql.Append(" LEFT OUTER JOIN Mov_Dettaglio_Tecnico  " & vbCrLf)
    '        strSql.Append(" ON Mov_Dettaglio_Tecnico.Piva = Movimenti_dettagli.PIVA AND Mov_Dettaglio_Tecnico.Sa_Cod = Movimenti_dettagli.Sa_Cod AND Mov_Dettaglio_Tecnico.Id_Agenda = Movimenti_dettagli.Id_Agenda AND " & vbCrLf)
    '        strSql.Append(" Mov_Dettaglio_Tecnico.Id_Mov = Movimenti_dettagli.Id_Mov AND Mov_Dettaglio_Tecnico.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det " & vbCrLf)

    '        strSql.Append("         inner Join ( " & vbCrLf)
    '        strSql.Append("             select distinct ip.Piva, ip.Sa_Cod, ip.Appezza, ip.Id_reg , ip.Progetto_Cod,  ip.validita_inizio, ip.validita_fine  " & vbCrLf)
    '        strSql.Append("             from mov_destinazioni d " & vbCrLf)
    '        strSql.Append("             inner join movimenti m on m.piva=d.piva AND m.id_agenda=d.id_agenda AND m.Id_Mov=d.id_mov " & vbCrLf)
    '        strSql.Append("             inner join imprese_progetti ip on d.piva=ip.Piva AND ip.Sa_Cod=d.Sa_Cod AND ip.Appezza=d.Appezza AND ip.Id_Reg=d.Id_Destinazione " & vbCrLf)
    '        strSql.Append("             where d.id_Agenda=" & Agro_SQL_SaveNum(ID_Agenda) & " " & vbCrLf)
    '        strSql.Append("            AND   d.Piva ='" & Agro_SQL_SaveText(Piva) & "' ")
    '        strSql.Append("             AND Tipo_Destinazione=0 " & vbCrLf)
    '        strSql.Append("             AND m.Data_Movimento >= ip.Validita_Inizio " & vbCrLf)
    '        strSql.Append("             AND m.Data_Movimento <= ip.Validita_fine " & vbCrLf)
    '        strSql.Append("         ) destinazioni_operazione   " & vbCrLf)
    '        strSql.Append("         On Imprese_Progetti.piva= destinazioni_operazione.piva  " & vbCrLf)
    '        strSql.Append("         AND Imprese_Progetti.Sa_Cod=destinazioni_operazione.Sa_Cod  " & vbCrLf)
    '        strSql.Append("         AND Imprese_Progetti.Appezza=destinazioni_operazione.Appezza  " & vbCrLf)
    '        strSql.Append("         AND Imprese_Progetti.Id_Reg=destinazioni_operazione.id_reg " & vbCrLf)
    '        strSql.Append("         AND Imprese_Progetti.progetto_cod=destinazioni_operazione.Progetto_Cod " & vbCrLf)
    '        strSql.Append("         AND Movimenti.Data_Movimento >= destinazioni_operazione.Validita_Inizio " & vbCrLf)
    '        strSql.Append("         AND Movimenti.Data_Movimento <= destinazioni_operazione.Validita_fine " & vbCrLf)

    '        strSql.Append(" WHERE Movimenti_dettagli.Elem_Cod = 3 " & vbCrLf)
    '        strSql.Append(" AND Movimenti.Cau_Mov = '2300' " & vbCrLf)
    '        strSql.Append(" AND   Movimenti.Piva ='" & Agro_SQL_SaveText(Piva) & "' ")
    '        strSql.Append(" AND Mov_Destinazioni.id_Agenda=" & Agro_SQL_SaveNum(ID_Agenda) & " " & vbCrLf)

    '        If xFiltroAggiuntivo <> "" Then
    '            StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
    '        End If

    '        strSql.Append(" GROUP BY imprese_progetti.Piva, imprese_progetti.Sa_Cod, imprese_progetti.Appezza, imprese_progetti.Id_reg, imprese_progetti.progetto_cod ")


    '        strSql.Append(" ) questa" & vbCrLf)


    '        strSql.Append(" On tutte.piva=questa.piva " & vbCrLf)
    '        strSql.Append(" AND tutte.sa_cod=questa.sa_cod " & vbCrLf)
    '        strSql.Append(" AND tutte.appezza=questa.appezza " & vbCrLf)
    '        strSql.Append(" AND tutte.id_reg=questa.id_reg " & vbCrLf)
    '        strSql.Append(" AND tutte.progetto_cod=questa.progetto_cod " & vbCrLf)


    '        strSql.Append("  ) fertilizzazioni " & vbCrLf)

    '        strSql.Append(" LEFT JOIN " & vbCrLf)

    '        strSql.Append(" ( " & vbCrLf)


    '        strSql.Append(" Select imprese_progetti.Piva, imprese_progetti.sa_cod,imprese_progetti.appezza,imprese_progetti.id_reg, imprese_progetti.progetto_cod,  " & vbCrLf)

    '        strSql.Append("  SUM (case   " & vbCrLf)
    '        strSql.Append(" 	    when __tmp_FormulatiXPrincipiAttivi.peso > 0   " & vbCrLf)
    '        strSql.Append(" 		then Mov_Destinazioni.Qta / mov_destinazioni.qta2 * __tmp_FormulatiXPrincipiAttivi.Peso / 1000  " & vbCrLf)
    '        strSql.Append(" 	    Else Mov_Destinazioni.Qta / mov_destinazioni.qta2 * __tmp_FormulatiXPrincipiAttivi.Titolo / 100  " & vbCrLf)
    '        strSql.Append("  End ) As Cu_Distribuito_Trattamenti_Ha  " & vbCrLf)

    '        strSql.Append(" FROM Mov_Destinazioni  " & vbCrLf)

    '        strSql.Append(" INNER JOIN Reg_Impianti " & vbCrLf)
    '        strSql.Append(" ON Reg_Impianti.PIVA = Mov_Destinazioni.Piva AND Reg_Impianti.SA_COD = Mov_Destinazioni.Sa_Cod  " & vbCrLf)
    '        strSql.Append(" AND Reg_Impianti.APPEZZA = Mov_Destinazioni.Appezza AND Reg_Impianti.ID_REG = Mov_Destinazioni.Id_Destinazione " & vbCrLf)

    '        strSql.Append(" LEFT OUTER JOIN Imprese_Progetti  " & vbCrLf)
    '        strSql.Append(" ON Imprese_Progetti.Piva = Reg_Impianti.Piva AND Imprese_Progetti.Sa_Cod = Reg_Impianti.Sa_Cod " & vbCrLf)
    '        strSql.Append(" AND Imprese_Progetti.Appezza = Reg_Impianti.Appezza AND Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg  " & vbCrLf)

    '        strSql.Append(" INNER JOIN Movimenti_dettagli " & vbCrLf)
    '        strSql.Append(" ON Mov_Destinazioni.Piva = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Sa_Cod = Movimenti_dettagli.Sa_Cod  " & vbCrLf)
    '        strSql.Append(" AND Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det " & vbCrLf)

    '        strSql.Append(" INNER JOIN Movimenti " & vbCrLf)
    '        strSql.Append(" ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod   " & vbCrLf)
    '        strSql.Append(" AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov " & vbCrLf)

    '        strSql.Append(" INNER JOIN Agenda " & vbCrLf)
    '        strSql.Append(" ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda AND  Agenda.Sa_Cod = Movimenti.Sa_Cod  " & vbCrLf)

    '        strSql.Append(" INNER JOIN __tmp_FormulatiXPrincipiAttivi " & vbCrLf)
    '        strSql.Append(" On __tmp_FormulatiXPrincipiAttivi.fr_cod=Movimenti_dettagli.Pro_Cod  " & vbCrLf)

    '        strSql.Append("         inner Join ( " & vbCrLf)
    '        strSql.Append("             select distinct ip.Piva, ip.Sa_Cod, ip.Appezza, ip.Id_reg , ip.Progetto_Cod,  ip.validita_inizio, ip.validita_fine  " & vbCrLf)
    '        strSql.Append("             from mov_destinazioni d " & vbCrLf)
    '        strSql.Append("             inner join movimenti m on m.piva=d.piva AND m.id_agenda=d.id_agenda AND m.Id_Mov=d.id_mov " & vbCrLf)
    '        strSql.Append("             inner join imprese_progetti ip on d.piva=ip.Piva AND ip.Sa_Cod=d.Sa_Cod AND ip.Appezza=d.Appezza AND ip.Id_Reg=d.Id_Destinazione " & vbCrLf)
    '        strSql.Append("             where d.id_Agenda=" & Agro_SQL_SaveNum(ID_Agenda) & " " & vbCrLf)
    '        strSql.Append("             AND   d.Piva ='" & Agro_SQL_SaveText(Piva) & "' ")
    '        strSql.Append("             AND Tipo_Destinazione=0 " & vbCrLf)
    '        strSql.Append("             AND m.Data_Movimento >= ip.Validita_Inizio " & vbCrLf)
    '        strSql.Append("             AND m.Data_Movimento <= ip.Validita_fine " & vbCrLf)
    '        strSql.Append("         ) destinazioni_operazione   " & vbCrLf)
    '        strSql.Append("         On Imprese_Progetti.piva= destinazioni_operazione.piva  " & vbCrLf)
    '        strSql.Append("         AND Imprese_Progetti.Sa_Cod=destinazioni_operazione.Sa_Cod  " & vbCrLf)
    '        strSql.Append("         AND Imprese_Progetti.Appezza=destinazioni_operazione.Appezza  " & vbCrLf)
    '        strSql.Append("         AND Imprese_Progetti.Id_Reg=destinazioni_operazione.id_reg " & vbCrLf)
    '        strSql.Append("         AND Imprese_Progetti.progetto_cod=destinazioni_operazione.Progetto_Cod " & vbCrLf)
    '        strSql.Append("         AND Movimenti.Data_Movimento >= destinazioni_operazione.Validita_Inizio " & vbCrLf)
    '        strSql.Append("         AND Movimenti.Data_Movimento <= destinazioni_operazione.Validita_fine " & vbCrLf)

    '        strSql.Append(" WHERE Movimenti_dettagli.Elem_Cod = 191 " & vbCrLf)
    '        strSql.Append(" AND Movimenti.Cau_Mov = '2050' " & vbCrLf)
    '        strSql.Append(" AND   Movimenti.Piva ='" & Agro_SQL_SaveText(Piva) & "' ")
    '        strSql.Append(" AND __tmp_FormulatiXPrincipiAttivi.IDTestataTemp=" & Agro_SQL_SaveNum(IDTestataTemp__tmp_FormulatiXPrincipiAttivi) & " " & vbCrLf)
    '        strSql.Append(" AND pa_cod IN " & Agro_SQL_SaveText(PaRameici_str) & " " & vbCrLf)

    '        If xFiltroAggiuntivo <> "" Then
    '            StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
    '        End If
    '        strSql.Append(" GROUP BY imprese_progetti.Piva, imprese_progetti.Sa_Cod, imprese_progetti.Appezza, imprese_progetti.Id_reg, imprese_progetti.progetto_cod ")

    '        strSql.Append(" ) trattamenti" & vbCrLf)

    '        strSql.Append(" On fertilizzazioni.piva=trattamenti.piva " & vbCrLf)
    '        strSql.Append(" AND fertilizzazioni.sa_cod=trattamenti.sa_cod " & vbCrLf)
    '        strSql.Append(" AND fertilizzazioni.appezza=trattamenti.appezza " & vbCrLf)
    '        strSql.Append(" AND fertilizzazioni.id_reg=trattamenti.id_reg " & vbCrLf)
    '        strSql.Append(" AND fertilizzazioni.progetto_cod=trattamenti.progetto_cod " & vbCrLf)


    '        '--------------------------------------------------------------------------
    '        dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
    '        '--------------------------------------------------------------------------

    '    Catch ex As Exception
    '        messaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
    '        dt = Nothing
    '        Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
    '    End Try

    '    Return dt

    'End Function

    Public Function Leggi_Rame_Distribuito_suDistinte(ByVal Piva As String,
                                                        ByVal ID_Agenda As Integer, ByVal IDTestataTemp As Integer,
                                                        ByVal IDTestataTemp__tmp_FormulatiXPrincipiAttivi As Integer,
                                                        ByVal xFiltroAggiuntivo As String,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                        ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Dettagli_R.Leggi_Rame_Distribuito_suDistinte()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            '----------------------------------------

            'strSql.Append(" Select * " & vbCrLf)

            strSql.Append(" Select Imprese_Progetti.piva, Imprese_Progetti.sa_cod, Imprese_Progetti.appezza, Imprese_Progetti.id_reg, Imprese_Progetti.progetto_cod " & vbCrLf)
            strSql.Append(" , Imprese_Progetti.validita_inizio, Imprese_Progetti.validita_fine " & vbCrLf)
            strSql.Append(" , fertilizzazioni.Cu_Distribuito_Fertilizzazioni_Ha, trattamenti.Cu_Distribuito_Trattamenti_Ha " & vbCrLf)
            strSql.Append(" , Appezzamento.app_nome " & vbCrLf)

            strSql.Append(" from Imprese_Progetti " & vbCrLf)


            If ID_Agenda <> 0 Then

                strSql.Append("         inner Join ( " & vbCrLf)
                strSql.Append("             select distinct ip.Piva, ip.Sa_Cod, ip.Appezza, ip.Id_reg , ip.Progetto_Cod,  ip.validita_inizio, ip.validita_fine  " & vbCrLf)
                strSql.Append("             from mov_destinazioni d " & vbCrLf)
                strSql.Append("             inner join movimenti m on m.piva=d.piva AND m.id_agenda=d.id_agenda AND m.Id_Mov=d.id_mov " & vbCrLf)
                strSql.Append("             inner join imprese_progetti ip on d.piva=ip.Piva AND ip.Sa_Cod=d.Sa_Cod AND ip.Appezza=d.Appezza AND ip.Id_Reg=d.Id_Destinazione " & vbCrLf)
                strSql.Append("             where d.id_Agenda=" & Agro_SQL_SaveNum(ID_Agenda) & " " & vbCrLf)
                strSql.Append("             AND   d.Piva ='" & Agro_SQL_SaveText(Piva) & "' ")
                strSql.Append("             AND Tipo_Destinazione=0 " & vbCrLf)
                strSql.Append("             AND m.Data_Movimento >= ip.Validita_Inizio " & vbCrLf)
                strSql.Append("             AND m.Data_Movimento <= ip.Validita_fine " & vbCrLf)
                strSql.Append("         ) destinazioni_operazione   " & vbCrLf)
                strSql.Append("         On Imprese_Progetti.piva= destinazioni_operazione.piva  " & vbCrLf)
                strSql.Append("         AND Imprese_Progetti.Sa_Cod=destinazioni_operazione.Sa_Cod  " & vbCrLf)
                strSql.Append("         AND Imprese_Progetti.Appezza=destinazioni_operazione.Appezza  " & vbCrLf)
                strSql.Append("         AND Imprese_Progetti.Id_Reg=destinazioni_operazione.id_reg " & vbCrLf)
                strSql.Append("         AND Imprese_Progetti.progetto_cod=destinazioni_operazione.Progetto_Cod " & vbCrLf)


            Else

                strSql.Append("         inner Join __Tmp_Movimenti_Destinazioni_DateDistinta " & vbCrLf)
                strSql.Append("                 On Imprese_Progetti.piva=__Tmp_Movimenti_Destinazioni_DateDistinta.piva   " & vbCrLf)
                strSql.Append("                 AND Imprese_Progetti.Sa_Cod=__Tmp_Movimenti_Destinazioni_DateDistinta.Sa_Cod   " & vbCrLf)
                strSql.Append("                 AND Imprese_Progetti.Appezza=__Tmp_Movimenti_Destinazioni_DateDistinta.Appezza   " & vbCrLf)
                strSql.Append("                 AND Imprese_Progetti.Id_Reg=__Tmp_Movimenti_Destinazioni_DateDistinta.id_reg  " & vbCrLf)
                strSql.Append("                 AND Imprese_Progetti.progetto_cod=__Tmp_Movimenti_Destinazioni_DateDistinta.Progetto_Cod  " & vbCrLf)
                strSql.Append("         	    AND __Tmp_Movimenti_Destinazioni_DateDistinta.idTestataTemp = " & Agro_SQL_SaveNum(IDTestataTemp) & vbCrLf)

            End If


            strSql.Append(" LEFT JOIN  " & vbCrLf)
            strSql.Append(" (  " & vbCrLf)

            strSql.Append(" Select imprese_progetti.Piva, imprese_progetti.sa_cod,imprese_progetti.appezza,imprese_progetti.id_reg, imprese_progetti.progetto_cod " & vbCrLf)
            strSql.Append(" , ISNULL(sum((Mov_Destinazioni.Qta / mov_destinazioni.qta2) * ISNULL(Mov_Dettaglio_Tecnico.Cu, 0) / 100),0) As Cu_Distribuito_Fertilizzazioni_Ha  " & vbCrLf)

            strSql.Append(" FROM Mov_Destinazioni  " & vbCrLf)

            strSql.Append(" INNER JOIN Imprese_Progetti " & vbCrLf)
            strSql.Append(" ON Imprese_Progetti.PIVA = Mov_Destinazioni.Piva AND Imprese_Progetti.SA_COD = Mov_Destinazioni.Sa_Cod  " & vbCrLf)
            strSql.Append(" AND Imprese_Progetti.APPEZZA = Mov_Destinazioni.Appezza AND Imprese_Progetti.ID_REG = Mov_Destinazioni.Id_Destinazione " & vbCrLf)

            strSql.Append(" INNER JOIN Movimenti_dettagli " & vbCrLf)
            strSql.Append(" ON Mov_Destinazioni.Piva = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Sa_Cod = Movimenti_dettagli.Sa_Cod  " & vbCrLf)
            strSql.Append(" AND Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det " & vbCrLf)

            strSql.Append(" INNER JOIN Movimenti " & vbCrLf)
            strSql.Append(" ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod   " & vbCrLf)
            strSql.Append(" AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov " & vbCrLf)

            strSql.Append(" INNER JOIN Agenda " & vbCrLf)
            strSql.Append(" ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda AND  Agenda.Sa_Cod = Movimenti.Sa_Cod  " & vbCrLf)

            strSql.Append(" LEFT OUTER JOIN Mov_Dettaglio_Tecnico  " & vbCrLf)
            strSql.Append(" ON Mov_Dettaglio_Tecnico.Piva = Movimenti_dettagli.PIVA AND Mov_Dettaglio_Tecnico.Sa_Cod = Movimenti_dettagli.Sa_Cod AND Mov_Dettaglio_Tecnico.Id_Agenda = Movimenti_dettagli.Id_Agenda AND " & vbCrLf)
            strSql.Append(" Mov_Dettaglio_Tecnico.Id_Mov = Movimenti_dettagli.Id_Mov AND Mov_Dettaglio_Tecnico.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det " & vbCrLf)

            If ID_Agenda <> 0 Then

                strSql.Append("         inner Join ( " & vbCrLf)
                strSql.Append("             select distinct ip.Piva, ip.Sa_Cod, ip.Appezza, ip.Id_reg , ip.Progetto_Cod,  ip.validita_inizio, ip.validita_fine  " & vbCrLf)
                strSql.Append("             from mov_destinazioni d " & vbCrLf)
                strSql.Append("             inner join movimenti m on m.piva=d.piva AND m.id_agenda=d.id_agenda AND m.Id_Mov=d.id_mov " & vbCrLf)
                strSql.Append("             inner join imprese_progetti ip on d.piva=ip.Piva AND ip.Sa_Cod=d.Sa_Cod AND ip.Appezza=d.Appezza AND ip.Id_Reg=d.Id_Destinazione " & vbCrLf)
                strSql.Append("             where d.id_Agenda=" & Agro_SQL_SaveNum(ID_Agenda) & " " & vbCrLf)
                strSql.Append("             AND   d.Piva ='" & Agro_SQL_SaveText(Piva) & "' ")
                strSql.Append("             AND Tipo_Destinazione=0 " & vbCrLf)
                strSql.Append("             AND m.Data_Movimento >= ip.Validita_Inizio " & vbCrLf)
                strSql.Append("             AND m.Data_Movimento <= ip.Validita_fine " & vbCrLf)
                strSql.Append("         ) destinazioni_operazione   " & vbCrLf)
                strSql.Append("         On Imprese_Progetti.piva= destinazioni_operazione.piva  " & vbCrLf)
                strSql.Append("         AND Imprese_Progetti.Sa_Cod=destinazioni_operazione.Sa_Cod  " & vbCrLf)
                strSql.Append("         AND Imprese_Progetti.Appezza=destinazioni_operazione.Appezza  " & vbCrLf)
                strSql.Append("         AND Imprese_Progetti.Id_Reg=destinazioni_operazione.id_reg " & vbCrLf)
                strSql.Append("         AND Imprese_Progetti.progetto_cod=destinazioni_operazione.Progetto_Cod " & vbCrLf)
                strSql.Append("         AND Movimenti.Data_Movimento >= destinazioni_operazione.Validita_Inizio " & vbCrLf)
                strSql.Append("         AND Movimenti.Data_Movimento <= destinazioni_operazione.Validita_fine " & vbCrLf)

            Else

                strSql.Append("         inner Join __Tmp_Movimenti_Destinazioni_DateDistinta " & vbCrLf)
                strSql.Append("                 On Imprese_Progetti.piva=__Tmp_Movimenti_Destinazioni_DateDistinta.piva   " & vbCrLf)
                strSql.Append("                 AND Imprese_Progetti.Sa_Cod=__Tmp_Movimenti_Destinazioni_DateDistinta.Sa_Cod   " & vbCrLf)
                strSql.Append("                 AND Imprese_Progetti.Appezza=__Tmp_Movimenti_Destinazioni_DateDistinta.Appezza   " & vbCrLf)
                strSql.Append("                 AND Imprese_Progetti.Id_Reg=__Tmp_Movimenti_Destinazioni_DateDistinta.id_reg  " & vbCrLf)
                strSql.Append("                 AND Imprese_Progetti.progetto_cod=__Tmp_Movimenti_Destinazioni_DateDistinta.Progetto_Cod  " & vbCrLf)
                strSql.Append("                 AND Movimenti.Data_Movimento >= __Tmp_Movimenti_Destinazioni_DateDistinta.Validita_Inizio  " & vbCrLf)
                strSql.Append("                 AND Movimenti.Data_Movimento <= __Tmp_Movimenti_Destinazioni_DateDistinta.Validita_fine  " & vbCrLf)
                strSql.Append("         	    AND __Tmp_Movimenti_Destinazioni_DateDistinta.idTestataTemp = " & Agro_SQL_SaveNum(IDTestataTemp) & " ")

            End If


            strSql.Append(" WHERE Movimenti_dettagli.Elem_Cod = 3 " & vbCrLf)
            strSql.Append(" AND Movimenti.Cau_Mov = '2300' " & vbCrLf)
            strSql.Append(" AND   Movimenti.Piva ='" & Agro_SQL_SaveText(Piva) & "' ")
            strSql.Append(" AND Mov_Dettaglio_Tecnico.Cu <> 0 " & vbCrLf)
            strSql.Append(" AND Mov_Destinazioni.qta2 > 0 " & vbCrLf)

            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            strSql.AppendLine(" GROUP BY imprese_progetti.Piva, imprese_progetti.Sa_Cod, imprese_progetti.Appezza, imprese_progetti.Id_reg, imprese_progetti.progetto_cod ")

            strSql.AppendLine("  ) fertilizzazioni " & vbCrLf)

            strSql.AppendLine(" On imprese_progetti.piva=fertilizzazioni.piva " & vbCrLf)
            strSql.AppendLine(" AND imprese_progetti.sa_cod=fertilizzazioni.sa_cod " & vbCrLf)
            strSql.AppendLine(" AND imprese_progetti.appezza=fertilizzazioni.appezza " & vbCrLf)
            strSql.AppendLine(" AND imprese_progetti.id_reg=fertilizzazioni.id_reg " & vbCrLf)
            strSql.AppendLine(" AND imprese_progetti.progetto_cod=fertilizzazioni.progetto_cod " & vbCrLf)

            strSql.AppendLine(" LEFT JOIN " & vbCrLf)

            strSql.AppendLine(" ( " & vbCrLf)

            strSql.Append(" Select imprese_progetti.Piva, imprese_progetti.sa_cod,imprese_progetti.appezza,imprese_progetti.id_reg, imprese_progetti.progetto_cod,  " & vbCrLf)

            strSql.Append("  ISNULL (SUM (case   " & vbCrLf)
            strSql.Append(" 	    when __tmp_FormulatiXPrincipiAttivi.peso > 0   " & vbCrLf)
            strSql.Append(" 		then (Mov_Destinazioni.Qta / mov_destinazioni.qta2) * __tmp_FormulatiXPrincipiAttivi.Peso / 1000  " & vbCrLf)
            strSql.Append(" 	    Else (Mov_Destinazioni.Qta / mov_destinazioni.qta2) * __tmp_FormulatiXPrincipiAttivi.Titolo / 100  " & vbCrLf)
            strSql.Append("  End ) ,0) As Cu_Distribuito_Trattamenti_Ha  " & vbCrLf)

            strSql.Append(" FROM Mov_Destinazioni  " & vbCrLf)

            strSql.Append(" INNER JOIN Imprese_Progetti " & vbCrLf)
            strSql.Append(" ON Imprese_Progetti.PIVA = Mov_Destinazioni.Piva AND Imprese_Progetti.SA_COD = Mov_Destinazioni.Sa_Cod  " & vbCrLf)
            strSql.Append(" AND Imprese_Progetti.APPEZZA = Mov_Destinazioni.Appezza AND Imprese_Progetti.ID_REG = Mov_Destinazioni.Id_Destinazione " & vbCrLf)

            strSql.Append(" INNER JOIN Movimenti_dettagli " & vbCrLf)
            strSql.Append(" ON Mov_Destinazioni.Piva = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Sa_Cod = Movimenti_dettagli.Sa_Cod  " & vbCrLf)
            strSql.Append(" AND Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det " & vbCrLf)

            strSql.Append(" INNER JOIN Movimenti " & vbCrLf)
            strSql.Append(" ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod   " & vbCrLf)
            strSql.Append(" AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov " & vbCrLf)

            strSql.Append(" INNER JOIN Agenda " & vbCrLf)
            strSql.Append(" ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda AND  Agenda.Sa_Cod = Movimenti.Sa_Cod  " & vbCrLf)

            strSql.Append(" INNER JOIN __tmp_FormulatiXPrincipiAttivi " & vbCrLf)
            strSql.Append(" On __tmp_FormulatiXPrincipiAttivi.fr_cod=Movimenti_dettagli.Pro_Cod  " & vbCrLf)

            If ID_Agenda <> 0 Then

                strSql.Append("         inner Join ( " & vbCrLf)
                strSql.Append("             select distinct ip.Piva, ip.Sa_Cod, ip.Appezza, ip.Id_reg , ip.Progetto_Cod,  ip.validita_inizio, ip.validita_fine  " & vbCrLf)
                strSql.Append("             from mov_destinazioni d " & vbCrLf)
                strSql.Append("             inner join movimenti m on m.piva=d.piva AND m.id_agenda=d.id_agenda AND m.Id_Mov=d.id_mov " & vbCrLf)
                strSql.Append("             inner join imprese_progetti ip on d.piva=ip.Piva AND ip.Sa_Cod=d.Sa_Cod AND ip.Appezza=d.Appezza AND ip.Id_Reg=d.Id_Destinazione " & vbCrLf)
                strSql.Append("             where d.id_Agenda=" & Agro_SQL_SaveNum(ID_Agenda) & " " & vbCrLf)
                strSql.Append("             AND   d.Piva ='" & Agro_SQL_SaveText(Piva) & "' ")
                strSql.Append("             AND Tipo_Destinazione=0 " & vbCrLf)
                strSql.Append("             AND m.Data_Movimento >= ip.Validita_Inizio " & vbCrLf)
                strSql.Append("             AND m.Data_Movimento <= ip.Validita_fine " & vbCrLf)
                strSql.Append("         ) destinazioni_operazione   " & vbCrLf)
                strSql.Append("         On Imprese_Progetti.piva= destinazioni_operazione.piva  " & vbCrLf)
                strSql.Append("         AND Imprese_Progetti.Sa_Cod=destinazioni_operazione.Sa_Cod  " & vbCrLf)
                strSql.Append("         AND Imprese_Progetti.Appezza=destinazioni_operazione.Appezza  " & vbCrLf)
                strSql.Append("         AND Imprese_Progetti.Id_Reg=destinazioni_operazione.id_reg " & vbCrLf)
                strSql.Append("         AND Imprese_Progetti.progetto_cod=destinazioni_operazione.Progetto_Cod " & vbCrLf)
                strSql.Append("         AND Movimenti.Data_Movimento >= destinazioni_operazione.Validita_Inizio " & vbCrLf)
                strSql.Append("         AND Movimenti.Data_Movimento <= destinazioni_operazione.Validita_fine " & vbCrLf)

            Else

                strSql.Append("         inner Join __Tmp_Movimenti_Destinazioni_DateDistinta " & vbCrLf)
                strSql.Append("                 On Imprese_Progetti.piva=__Tmp_Movimenti_Destinazioni_DateDistinta.piva   " & vbCrLf)
                strSql.Append("                 AND Imprese_Progetti.Sa_Cod=__Tmp_Movimenti_Destinazioni_DateDistinta.Sa_Cod   " & vbCrLf)
                strSql.Append("                 AND Imprese_Progetti.Appezza=__Tmp_Movimenti_Destinazioni_DateDistinta.Appezza   " & vbCrLf)
                strSql.Append("                 AND Imprese_Progetti.Id_Reg=__Tmp_Movimenti_Destinazioni_DateDistinta.id_reg  " & vbCrLf)
                strSql.Append("                 AND Imprese_Progetti.progetto_cod=__Tmp_Movimenti_Destinazioni_DateDistinta.Progetto_Cod  " & vbCrLf)
                strSql.Append("                 AND Movimenti.Data_Movimento >= __Tmp_Movimenti_Destinazioni_DateDistinta.Validita_Inizio  " & vbCrLf)
                strSql.Append("                 AND Movimenti.Data_Movimento <= __Tmp_Movimenti_Destinazioni_DateDistinta.Validita_fine  " & vbCrLf)
                strSql.Append("         	    AND __Tmp_Movimenti_Destinazioni_DateDistinta.idTestataTemp = " & Agro_SQL_SaveNum(IDTestataTemp) & " ")

            End If


            strSql.Append(" WHERE Movimenti_dettagli.Elem_Cod = 191 " & vbCrLf)
            strSql.Append(" AND Movimenti.Cau_Mov = '2050' " & vbCrLf)
            strSql.Append(" AND   Movimenti.Piva ='" & Agro_SQL_SaveText(Piva) & "' ")

            strSql.Append(" AND __tmp_FormulatiXPrincipiAttivi.IDTestataTemp=" & Agro_SQL_SaveNum(IDTestataTemp__tmp_FormulatiXPrincipiAttivi) & " " & vbCrLf)
            strSql.Append(" AND pa_cod IN " & Agro_SQL_Save_Clausola_IN(PaRameici_str, False) & " " & vbCrLf)

            strSql.Append(" AND Mov_Destinazioni.qta2 > 0 " & vbCrLf)

            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            strSql.AppendLine(" GROUP BY imprese_progetti.Piva, imprese_progetti.Sa_Cod, imprese_progetti.Appezza, imprese_progetti.Id_reg, imprese_progetti.progetto_cod ")

            strSql.AppendLine(" ) trattamenti" & vbCrLf)


            strSql.AppendLine(" On imprese_progetti.piva=trattamenti.piva " & vbCrLf)
            strSql.AppendLine(" AND imprese_progetti.sa_cod=trattamenti.sa_cod " & vbCrLf)
            strSql.AppendLine(" AND imprese_progetti.appezza=trattamenti.appezza " & vbCrLf)
            strSql.AppendLine(" AND imprese_progetti.id_reg=trattamenti.id_reg " & vbCrLf)
            strSql.AppendLine(" AND imprese_progetti.progetto_cod=trattamenti.progetto_cod " & vbCrLf)


            strSql.AppendLine("   INNER JOIN Appezzamento  " & vbCrLf)
            strSql.AppendLine(" ON Appezzamento.PIVA = Imprese_Progetti.PIVA AND Appezzamento.Sa_Cod = Imprese_Progetti.Sa_Cod and Appezzamento.APPEZZA = Imprese_Progetti.APPEZZA " & vbCrLf)

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

    Public Function Leggi_Macroelementi_Distribuiti_suDistinte(ByVal Piva As String,
                                                               ByVal ID_Agenda As Integer, ByVal IDTestataTemp As Integer,
                                                               ByVal IDTestataTemp__tmp_FormulatiXPrincipiAttivi As Integer,
                                                               ByVal xFiltroAggiuntivo As String,
                                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                                    Optional isVerificaPosteriori As Boolean = False
                                                               ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Dettagli_R.Leggi_Macroelementi_Distribuiti_suDistinte()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            '----------------------------------------

            strSql.AppendLine(" SELECT Imprese_Progetti.piva, Imprese_Progetti.sa_cod, Imprese_Progetti.appezza, Imprese_Progetti.id_reg, Imprese_Progetti.progetto_cod ")
            strSql.AppendLine("        , Imprese_Progetti.validita_inizio, Imprese_Progetti.validita_fine, Imprese_Progetti.Regolamento_Concimazioni_Cod ")
            strSql.AppendLine("        , APP_NOME, Reg_Impianti.GRFI_COD ")

            strSql.AppendLine("")
            '--- LIMITI APPORTI ---
            strSql.AppendLine("        , ISNULL((SELECT Reg_Impianti_Codici.val_cod ")
            strSql.AppendLine("                     FROM Reg_Impianti_Codici ")
            strSql.AppendLine("                     WHERE Piva = Imprese_Progetti.PIVA AND sa_cod = Imprese_Progetti.sa_cod ")
            strSql.AppendLine("                     AND appezza = Imprese_Progetti.appezza AND Id_Reg = Imprese_Progetti.id_reg ")
            strSql.AppendLine("                     AND Progetto_Cod = Imprese_Progetti.Progetto_Cod  ")
            strSql.AppendLine("                     AND Reg_Impianti_Codici.id_cod = 1050 ")
            strSql.AppendLine("        ), '') AS N_Massimo ")

            strSql.AppendLine("")

            strSql.AppendLine("        , ISNULL((SELECT Reg_Impianti_Codici.val_cod ")
            strSql.AppendLine("                     FROM Reg_Impianti_Codici ")
            strSql.AppendLine("                     WHERE Piva = Imprese_Progetti.PIVA AND sa_cod = Imprese_Progetti.sa_cod ")
            strSql.AppendLine("                     AND appezza = Imprese_Progetti.appezza AND Id_Reg = Imprese_Progetti.id_reg ")
            strSql.AppendLine("                     AND Progetto_Cod = Imprese_Progetti.Progetto_Cod  ")
            strSql.AppendLine("                     AND Reg_Impianti_Codici.id_cod = 1051 ")
            strSql.AppendLine("                 ), '') AS P_Massimo ")

            strSql.AppendLine("")

            strSql.AppendLine("        , ISNULL((SELECT Reg_Impianti_Codici.val_cod ")
            strSql.AppendLine("                     FROM Reg_Impianti_Codici ")
            strSql.AppendLine("                     WHERE Piva = Imprese_Progetti.PIVA AND sa_cod = Imprese_Progetti.sa_cod ")
            strSql.AppendLine("                     AND appezza = Imprese_Progetti.appezza AND Id_Reg = Imprese_Progetti.id_reg ")
            strSql.AppendLine("                     AND Progetto_Cod = Imprese_Progetti.Progetto_Cod  ")
            strSql.AppendLine("                     AND Reg_Impianti_Codici.id_cod = 1052 ")
            strSql.AppendLine("        ), '') AS K_Massimo ")

            strSql.AppendLine("")

            strSql.AppendLine("        , ISNULL((SELECT Reg_Impianti_Codici.val_cod ")
            strSql.AppendLine("                     FROM Reg_Impianti_Codici ")
            strSql.AppendLine("                     WHERE Piva = Imprese_Progetti.PIVA AND sa_cod = Imprese_Progetti.sa_cod ")
            strSql.AppendLine("                     AND appezza = Imprese_Progetti.appezza AND Id_Reg = Imprese_Progetti.id_reg ")
            strSql.AppendLine("                     AND Progetto_Cod = Imprese_Progetti.Progetto_Cod  ")
            strSql.AppendLine("                     AND Reg_Impianti_Codici.id_cod = 1053 ")
            strSql.AppendLine("        ), '') AS Mg_Massimo ")

            strSql.AppendLine("")

            strSql.AppendLine("        , fertilizzazioni.N_Distribuito_Ha ")
            strSql.AppendLine("        , fertilizzazioni.P_Distribuito_Ha ")
            strSql.AppendLine("        , fertilizzazioni.K_Distribuito_Ha ")
            strSql.AppendLine("        , fertilizzazioni.Mg_Distribuito_Ha ")
            strSql.AppendLine("        , fertilizzazioni.Cu_Distribuito_Ha ")
            strSql.AppendLine("        , trattamenti.Cu_Distribuito_Trattamenti_Ha ")

            If ID_Agenda <> 0 Then
                strSql.AppendLine("        , questa.N_Operazione_Ha, questa.P_Operazione_Ha, questa.K_Operazione_Ha, questa.Mg_Operazione_Ha, questa.Cu_Operazione_Ha ")
            End If

            If isVerificaPosteriori Then
                strSql.AppendLine("        , questa.Sup_Coinvolta ")
            End If

            strSql.AppendLine("")

            strSql.AppendLine(" FROM Imprese_Progetti ")

            strSql.AppendLine(" INNER JOIN Reg_Impianti ")
            strSql.AppendLine("     ON Imprese_Progetti.PIVA = Reg_Impianti.Piva AND Imprese_Progetti.SA_COD = Reg_Impianti.Sa_Cod  ")
            strSql.AppendLine("     AND Imprese_Progetti.APPEZZA = Reg_Impianti.Appezza AND Imprese_Progetti.ID_REG = Reg_Impianti.Id_Reg ")

            strSql.AppendLine(" INNER JOIN Appezzamento ")
            strSql.AppendLine("     ON Appezzamento.PIVA = Reg_Impianti.Piva AND Appezzamento.SA_COD = Reg_Impianti.Sa_Cod  ")
            strSql.AppendLine("     AND Appezzamento.APPEZZA = Reg_Impianti.Appezza ")

            If ID_Agenda <> 0 Then

                strSql.AppendLine(" INNER JOIN ( ")
                strSql.AppendLine("             SELECT distinct ip.Piva, ip.Sa_Cod, ip.Appezza, ip.Id_reg, ip.Progetto_Cod, ip.validita_inizio, ip.validita_fine  ")
                strSql.AppendLine("             FROM mov_destinazioni d ")
                strSql.AppendLine("             INNER JOIN movimenti m ON m.piva = d.piva AND m.id_agenda = d.id_agenda AND m.Id_Mov = d.id_mov ")
                strSql.AppendLine("             INNER JOIN imprese_progetti ip ON d.piva = ip.Piva AND ip.Sa_Cod = d.Sa_Cod AND ip.Appezza = d.Appezza AND ip.Id_Reg = d.Id_Destinazione ")
                strSql.AppendLine("             WHERE d.id_Agenda = " & Agro_SQL_SaveNum(ID_Agenda) & " ")
                strSql.AppendLine("             AND   d.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                strSql.AppendLine("             AND Tipo_Destinazione = 0 ")
                strSql.AppendLine("             AND m.Data_Movimento >= ip.Validita_Inizio ")
                strSql.AppendLine("             AND m.Data_Movimento <= ip.Validita_fine ")
                strSql.AppendLine("         ) destinazioni_operazione   ")
                strSql.AppendLine("     ON Imprese_Progetti.piva = destinazioni_operazione.piva  ")
                strSql.AppendLine("     AND Imprese_Progetti.Sa_Cod = destinazioni_operazione.Sa_Cod  ")
                strSql.AppendLine("     AND Imprese_Progetti.Appezza = destinazioni_operazione.Appezza  ")
                strSql.AppendLine("     AND Imprese_Progetti.Id_Reg = destinazioni_operazione.id_reg ")
                strSql.AppendLine("     AND Imprese_Progetti.progetto_cod = destinazioni_operazione.Progetto_Cod ")

            Else

                strSql.AppendLine(" INNER JOIN __Tmp_Movimenti_Destinazioni_DateDistinta ")
                strSql.AppendLine("     ON Imprese_Progetti.piva = __Tmp_Movimenti_Destinazioni_DateDistinta.piva   ")
                strSql.AppendLine("     AND Imprese_Progetti.Sa_Cod = __Tmp_Movimenti_Destinazioni_DateDistinta.Sa_Cod   ")
                strSql.AppendLine("     AND Imprese_Progetti.Appezza = __Tmp_Movimenti_Destinazioni_DateDistinta.Appezza   ")
                strSql.AppendLine("     AND Imprese_Progetti.Id_Reg = __Tmp_Movimenti_Destinazioni_DateDistinta.id_reg  ")
                strSql.AppendLine("     AND Imprese_Progetti.progetto_cod = __Tmp_Movimenti_Destinazioni_DateDistinta.Progetto_Cod  ")
                strSql.AppendLine("     AND __Tmp_Movimenti_Destinazioni_DateDistinta.idTestataTemp = " & Agro_SQL_SaveNum(IDTestataTemp))

            End If


            strSql.AppendLine(" LEFT JOIN ( ")
            strSql.AppendLine("         SELECT imprese_progetti.Piva, imprese_progetti.sa_cod,imprese_progetti.appezza,imprese_progetti.id_reg, imprese_progetti.progetto_cod ")
            strSql.AppendLine("            , ISNULL(SUM((Mov_Destinazioni.Qta / mov_destinazioni.qta2) * ISNULL(Mov_Dettaglio_Tecnico.N, 0) / 100 * ISNULL(Mov_Dettaglio_Tecnico.efficienza, 1)), 0) AS N_Distribuito_Ha  ")
            strSql.AppendLine("            , ISNULL(SUM((Mov_Destinazioni.Qta / mov_destinazioni.qta2) * ISNULL(Mov_Dettaglio_Tecnico.P, 0) / 100), 0) AS P_Distribuito_Ha  ")
            strSql.AppendLine("            , ISNULL(SUM((Mov_Destinazioni.Qta / mov_destinazioni.qta2) * ISNULL(Mov_Dettaglio_Tecnico.K, 0) / 100), 0) AS K_Distribuito_Ha  ")
            strSql.AppendLine("            , ISNULL(SUM((Mov_Destinazioni.Qta / mov_destinazioni.qta2) * ISNULL(Mov_Dettaglio_Tecnico.Mg, 0) / 100), 0) AS Mg_Distribuito_Ha  ")
            strSql.AppendLine("            , ISNULL(SUM((Mov_Destinazioni.Qta / mov_destinazioni.qta2) * ISNULL(Mov_Dettaglio_Tecnico.Cu, 0) / 100), 0) AS Cu_Distribuito_Ha  ")

            strSql.AppendLine("         FROM Mov_Destinazioni  ")

            strSql.AppendLine("         INNER JOIN Imprese_Progetti ")
            strSql.AppendLine("             ON Imprese_Progetti.PIVA = Mov_Destinazioni.Piva AND Imprese_Progetti.SA_COD = Mov_Destinazioni.Sa_Cod  ")
            strSql.AppendLine("             AND Imprese_Progetti.APPEZZA = Mov_Destinazioni.Appezza AND Imprese_Progetti.ID_REG = Mov_Destinazioni.Id_Destinazione ")

            strSql.AppendLine("         INNER JOIN Movimenti_dettagli ")
            strSql.AppendLine("             ON Mov_Destinazioni.Piva = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Sa_Cod = Movimenti_dettagli.Sa_Cod  ")
            strSql.AppendLine("             AND Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det ")

            strSql.AppendLine("         INNER JOIN Movimenti ")
            strSql.AppendLine("             ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod   ")
            strSql.AppendLine("             AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov ")

            strSql.AppendLine("         INNER JOIN Agenda ")
            strSql.AppendLine("             ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda AND Agenda.Sa_Cod = Movimenti.Sa_Cod  ")

            strSql.AppendLine("         LEFT OUTER JOIN Mov_Dettaglio_Tecnico  ")
            strSql.AppendLine("             ON Mov_Dettaglio_Tecnico.Piva = Movimenti_dettagli.PIVA AND Mov_Dettaglio_Tecnico.Sa_Cod = Movimenti_dettagli.Sa_Cod AND Mov_Dettaglio_Tecnico.Id_Agenda = Movimenti_dettagli.Id_Agenda  ")
            strSql.AppendLine("             AND Mov_Dettaglio_Tecnico.Id_Mov = Movimenti_dettagli.Id_Mov AND Mov_Dettaglio_Tecnico.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det ")

            If ID_Agenda <> 0 Then

                strSql.AppendLine("         INNER JOIN ( ")
                strSql.AppendLine("                     SELECT distinct ip.Piva, ip.Sa_Cod, ip.Appezza, ip.Id_reg, ip.Progetto_Cod, ip.validita_inizio, ip.validita_fine  ")
                strSql.AppendLine("                     FROM mov_destinazioni d ")
                strSql.AppendLine("                     INNER JOIN movimenti m ON m.piva = d.piva AND m.id_agenda = d.id_agenda AND m.Id_Mov = d.id_mov ")
                strSql.AppendLine("                     INNER JOIN imprese_progetti ip ON d.piva = ip.Piva AND ip.Sa_Cod = d.Sa_Cod AND ip.Appezza = d.Appezza AND ip.Id_Reg = d.Id_Destinazione ")
                strSql.AppendLine("                     WHERE d.id_Agenda = " & Agro_SQL_SaveNum(ID_Agenda) & " ")
                strSql.AppendLine("                     AND d.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                strSql.AppendLine("                     AND Tipo_Destinazione = 0 ")
                strSql.AppendLine("                     AND m.Data_Movimento >= ip.Validita_Inizio ")
                strSql.AppendLine("                     AND m.Data_Movimento <= ip.Validita_fine ")
                strSql.AppendLine("         ) destinazioni_operazione   ")
                strSql.AppendLine("             ON Imprese_Progetti.piva= destinazioni_operazione.piva  ")
                strSql.AppendLine("             AND Imprese_Progetti.Sa_Cod = destinazioni_operazione.Sa_Cod  ")
                strSql.AppendLine("             AND Imprese_Progetti.Appezza = destinazioni_operazione.Appezza  ")
                strSql.AppendLine("             AND Imprese_Progetti.Id_Reg = destinazioni_operazione.id_reg ")
                strSql.AppendLine("             AND Imprese_Progetti.progetto_cod = destinazioni_operazione.Progetto_Cod ")
                strSql.AppendLine("             AND Movimenti.Data_Movimento >= destinazioni_operazione.Validita_Inizio ")
                strSql.AppendLine("             AND Movimenti.Data_Movimento <= destinazioni_operazione.Validita_fine ")

            Else

                strSql.AppendLine("         INNER JOIN __Tmp_Movimenti_Destinazioni_DateDistinta ")
                strSql.AppendLine("             ON Imprese_Progetti.piva = __Tmp_Movimenti_Destinazioni_DateDistinta.piva   ")
                strSql.AppendLine("             AND Imprese_Progetti.Sa_Cod = __Tmp_Movimenti_Destinazioni_DateDistinta.Sa_Cod   ")
                strSql.AppendLine("             AND Imprese_Progetti.Appezza= __Tmp_Movimenti_Destinazioni_DateDistinta.Appezza   ")
                strSql.AppendLine("             AND Imprese_Progetti.Id_Reg = __Tmp_Movimenti_Destinazioni_DateDistinta.id_reg  ")
                strSql.AppendLine("             AND Imprese_Progetti.progetto_cod = __Tmp_Movimenti_Destinazioni_DateDistinta.Progetto_Cod  ")
                strSql.AppendLine("             AND Movimenti.Data_Movimento >= __Tmp_Movimenti_Destinazioni_DateDistinta.Validita_Inizio  ")
                strSql.AppendLine("             AND Movimenti.Data_Movimento <= __Tmp_Movimenti_Destinazioni_DateDistinta.Validita_fine  ")
                strSql.AppendLine("             AND __Tmp_Movimenti_Destinazioni_DateDistinta.idTestataTemp = " & Agro_SQL_SaveNum(IDTestataTemp) & " ")

            End If


            strSql.AppendLine("         WHERE Movimenti_dettagli.Elem_Cod = 3 ")
            strSql.AppendLine("         AND Movimenti.Cau_Mov = '2300' ")
            strSql.AppendLine("         AND Movimenti.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            strSql.AppendLine("         AND Mov_Destinazioni.qta2 > 0 ")

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine("         AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            strSql.AppendLine("         GROUP BY imprese_progetti.Piva, imprese_progetti.Sa_Cod, imprese_progetti.Appezza, imprese_progetti.Id_reg, imprese_progetti.progetto_cod ")

            strSql.AppendLine(" ) fertilizzazioni ")

            strSql.AppendLine("     ON imprese_progetti.piva = fertilizzazioni.piva ")
            strSql.AppendLine("     AND imprese_progetti.sa_cod = fertilizzazioni.sa_cod ")
            strSql.AppendLine("     AND imprese_progetti.appezza = fertilizzazioni.appezza ")
            strSql.AppendLine("     AND imprese_progetti.id_reg = fertilizzazioni.id_reg ")
            strSql.AppendLine("     AND imprese_progetti.progetto_cod = fertilizzazioni.progetto_cod ")


            strSql.AppendLine(" LEFT JOIN (")
            strSql.AppendLine("         SELECT imprese_progetti.Piva, imprese_progetti.sa_cod, imprese_progetti.appezza, imprese_progetti.id_reg, imprese_progetti.progetto_cod,  ")
            strSql.AppendLine("         ISNULL (SUM(CASE   ")
            strSql.AppendLine("                 WHEN __tmp_FormulatiXPrincipiAttivi.peso > 0   ")
            strSql.AppendLine("                 THEN (Mov_Destinazioni.Qta / mov_destinazioni.qta2) * __tmp_FormulatiXPrincipiAttivi.Peso / 1000  ")
            strSql.AppendLine("                 ELSE (Mov_Destinazioni.Qta / mov_destinazioni.qta2) * __tmp_FormulatiXPrincipiAttivi.Titolo / 100  ")
            strSql.AppendLine("         END ), 0) AS Cu_Distribuito_Trattamenti_Ha  ")

            strSql.AppendLine("         FROM Mov_Destinazioni  ")

            strSql.AppendLine("         INNER JOIN Imprese_Progetti ")
            strSql.AppendLine("             ON Imprese_Progetti.PIVA = Mov_Destinazioni.Piva AND Imprese_Progetti.SA_COD = Mov_Destinazioni.Sa_Cod  ")
            strSql.AppendLine("             AND Imprese_Progetti.APPEZZA = Mov_Destinazioni.Appezza AND Imprese_Progetti.ID_REG = Mov_Destinazioni.Id_Destinazione ")

            strSql.AppendLine("         INNER JOIN Movimenti_dettagli ")
            strSql.AppendLine("             ON Mov_Destinazioni.Piva = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Sa_Cod = Movimenti_dettagli.Sa_Cod  ")
            strSql.AppendLine("             AND Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det ")

            strSql.AppendLine("         INNER JOIN Movimenti ")
            strSql.AppendLine("             ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod   ")
            strSql.AppendLine("             AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov ")

            strSql.AppendLine("         INNER JOIN Agenda ")
            strSql.AppendLine("             ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda AND  Agenda.Sa_Cod = Movimenti.Sa_Cod  ")

            strSql.AppendLine("         INNER JOIN __tmp_FormulatiXPrincipiAttivi ")
            strSql.AppendLine("             ON __tmp_FormulatiXPrincipiAttivi.fr_cod = Movimenti_dettagli.Pro_Cod  ")

            If ID_Agenda <> 0 Then

                strSql.AppendLine("         INNER JOIN ( ")
                strSql.AppendLine("             SELECT distinct ip.Piva, ip.Sa_Cod, ip.Appezza, ip.Id_reg , ip.Progetto_Cod,  ip.validita_inizio, ip.validita_fine  ")
                strSql.AppendLine("             FROM mov_destinazioni d ")
                strSql.AppendLine("             INNER JOIN movimenti m ON m.piva=d.piva AND m.id_agenda=d.id_agenda AND m.Id_Mov=d.id_mov ")
                strSql.AppendLine("             INNER JOIN imprese_progetti ip ON d.piva=ip.Piva AND ip.Sa_Cod=d.Sa_Cod AND ip.Appezza=d.Appezza AND ip.Id_Reg=d.Id_Destinazione ")
                strSql.AppendLine("             WHERE d.id_Agenda=" & Agro_SQL_SaveNum(ID_Agenda) & " ")
                strSql.AppendLine("             AND   d.Piva ='" & Agro_SQL_SaveText(Piva) & "' ")
                strSql.AppendLine("             AND Tipo_Destinazione=0 ")
                strSql.AppendLine("             AND m.Data_Movimento >= ip.Validita_Inizio ")
                strSql.AppendLine("             AND m.Data_Movimento <= ip.Validita_fine ")
                strSql.AppendLine("         ) destinazioni_operazione   ")
                strSql.AppendLine("             ON Imprese_Progetti.piva= destinazioni_operazione.piva  ")
                strSql.AppendLine("             AND Imprese_Progetti.Sa_Cod = destinazioni_operazione.Sa_Cod  ")
                strSql.AppendLine("             AND Imprese_Progetti.Appezza = destinazioni_operazione.Appezza  ")
                strSql.AppendLine("             AND Imprese_Progetti.Id_Reg = destinazioni_operazione.id_reg ")
                strSql.AppendLine("             AND Imprese_Progetti.progetto_cod = destinazioni_operazione.Progetto_Cod ")
                strSql.AppendLine("             AND Movimenti.Data_Movimento >= destinazioni_operazione.Validita_Inizio ")
                strSql.AppendLine("             AND Movimenti.Data_Movimento <= destinazioni_operazione.Validita_fine ")

            Else

                strSql.AppendLine("         INNER JOIN __Tmp_Movimenti_Destinazioni_DateDistinta ")
                strSql.AppendLine("             ON Imprese_Progetti.piva = __Tmp_Movimenti_Destinazioni_DateDistinta.piva   ")
                strSql.AppendLine("             AND Imprese_Progetti.Sa_Cod = __Tmp_Movimenti_Destinazioni_DateDistinta.Sa_Cod   ")
                strSql.AppendLine("             AND Imprese_Progetti.Appezza = __Tmp_Movimenti_Destinazioni_DateDistinta.Appezza   ")
                strSql.AppendLine("             AND Imprese_Progetti.Id_Reg = __Tmp_Movimenti_Destinazioni_DateDistinta.id_reg  ")
                strSql.AppendLine("             AND Imprese_Progetti.progetto_cod = __Tmp_Movimenti_Destinazioni_DateDistinta.Progetto_Cod  ")
                strSql.AppendLine("             AND Movimenti.Data_Movimento >= __Tmp_Movimenti_Destinazioni_DateDistinta.Validita_Inizio  ")
                strSql.AppendLine("             AND Movimenti.Data_Movimento <= __Tmp_Movimenti_Destinazioni_DateDistinta.Validita_fine  ")
                strSql.AppendLine("             AND __Tmp_Movimenti_Destinazioni_DateDistinta.idTestataTemp = " & Agro_SQL_SaveNum(IDTestataTemp) & " ")

            End If


            strSql.AppendLine("         WHERE Movimenti_dettagli.Elem_Cod = 191 ")
            strSql.AppendLine("         AND Movimenti.Cau_Mov = '2050' ")
            strSql.AppendLine("         AND Movimenti.Piva ='" & Agro_SQL_SaveText(Piva) & "' ")
            strSql.AppendLine("         AND Mov_Destinazioni.qta2 > 0 ")

            strSql.AppendLine("         AND __tmp_FormulatiXPrincipiAttivi.IDTestataTemp = " & Agro_SQL_SaveNum(IDTestataTemp__tmp_FormulatiXPrincipiAttivi) & " ")
            strSql.AppendLine("         AND pa_cod IN " & Agro_SQL_Save_Clausola_IN(PaRameici_str) & " ")

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine("         AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            strSql.AppendLine("         GROUP BY imprese_progetti.Piva, imprese_progetti.Sa_Cod, imprese_progetti.Appezza, imprese_progetti.Id_reg, imprese_progetti.progetto_cod ")

            strSql.AppendLine(" ) trattamenti")

            strSql.AppendLine("     ON imprese_progetti.piva = trattamenti.piva ")
            strSql.AppendLine("     AND imprese_progetti.sa_cod = trattamenti.sa_cod ")
            strSql.AppendLine("     AND imprese_progetti.appezza = trattamenti.appezza ")
            strSql.AppendLine("     AND imprese_progetti.id_reg = trattamenti.id_reg ")
            strSql.AppendLine("     AND imprese_progetti.progetto_cod = trattamenti.progetto_cod ")


            If ID_Agenda <> 0 Then

                strSql.AppendLine(" INNER JOIN (")
                strSql.AppendLine("         SELECT imprese_progetti.Piva, imprese_progetti.sa_cod, imprese_progetti.appezza, imprese_progetti.id_reg, imprese_progetti.progetto_cod  ")

                '--- DISTRIBUITI ---

                strSql.AppendLine("             , SUM((Mov_Destinazioni.Qta / mov_destinazioni.qta2) * ISNULL(Mov_Dettaglio_Tecnico.N, 0) / 100 * ISNULL(Mov_Dettaglio_Tecnico.efficienza, 1)) AS N_Operazione_Ha ")
                strSql.AppendLine("             , SUM((Mov_Destinazioni.Qta / mov_destinazioni.qta2) * ISNULL(Mov_Dettaglio_Tecnico.P, 0) / 100) AS P_Operazione_Ha  ")
                strSql.AppendLine("             , SUM((Mov_Destinazioni.Qta / mov_destinazioni.qta2) * ISNULL(Mov_Dettaglio_Tecnico.K, 0) / 100) AS K_Operazione_Ha  ")
                strSql.AppendLine("             , SUM((Mov_Destinazioni.Qta / mov_destinazioni.qta2) * ISNULL(Mov_Dettaglio_Tecnico.Mg, 0) / 100) AS Mg_Operazione_Ha  ")
                strSql.AppendLine("             , SUM((Mov_Destinazioni.Qta / mov_destinazioni.qta2) * ISNULL(Mov_Dettaglio_Tecnico.Cu, 0) / 100) AS Cu_Operazione_Ha  ")
                strSql.AppendLine("             , MAX(Mov_Destinazioni.qta2) AS Sup_Coinvolta ")

                strSql.AppendLine("         FROM Mov_Destinazioni  ")

                strSql.AppendLine("         INNER JOIN Imprese_Progetti ")
                strSql.AppendLine("             ON Imprese_Progetti.PIVA = Mov_Destinazioni.Piva AND Imprese_Progetti.SA_COD = Mov_Destinazioni.Sa_Cod  ")
                strSql.AppendLine("             AND Imprese_Progetti.APPEZZA = Mov_Destinazioni.Appezza AND Imprese_Progetti.ID_REG = Mov_Destinazioni.Id_Destinazione ")

                strSql.AppendLine("         INNER JOIN Movimenti_dettagli ")
                strSql.AppendLine("             ON Mov_Destinazioni.Piva = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Sa_Cod = Movimenti_dettagli.Sa_Cod  ")
                strSql.AppendLine("             AND Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det ")

                strSql.AppendLine("         INNER JOIN Movimenti ")
                strSql.AppendLine("             ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod   ")
                strSql.AppendLine("             AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov ")

                strSql.AppendLine("         INNER JOIN Agenda ")
                strSql.AppendLine("             ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda AND  Agenda.Sa_Cod = Movimenti.Sa_Cod  ")

                strSql.AppendLine("         LEFT OUTER JOIN Mov_Dettaglio_Tecnico  ")
                strSql.AppendLine("             ON Mov_Dettaglio_Tecnico.Piva = Movimenti_dettagli.PIVA AND Mov_Dettaglio_Tecnico.Sa_Cod = Movimenti_dettagli.Sa_Cod AND Mov_Dettaglio_Tecnico.Id_Agenda = Movimenti_dettagli.Id_Agenda AND ")
                strSql.AppendLine("             Mov_Dettaglio_Tecnico.Id_Mov = Movimenti_dettagli.Id_Mov AND Mov_Dettaglio_Tecnico.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det ")

                strSql.AppendLine("         INNER JOIN ( ")
                strSql.AppendLine("                 SELECT distinct ip.Piva, ip.Sa_Cod, ip.Appezza, ip.Id_reg , ip.Progetto_Cod,  ip.validita_inizio, ip.validita_fine  ")
                strSql.AppendLine("                 FROM mov_destinazioni d ")
                strSql.AppendLine("                 INNER JOIN movimenti m ON m.piva = d.piva AND m.id_agenda = d.id_agenda AND m.Id_Mov = d.id_mov ")
                strSql.AppendLine("                 INNER JOIN imprese_progetti ip ON d.piva = ip.Piva AND ip.Sa_Cod = d.Sa_Cod AND ip.Appezza = d.Appezza AND ip.Id_Reg = d.Id_Destinazione ")
                strSql.AppendLine("                 WHERE d.id_Agenda = " & Agro_SQL_SaveNum(ID_Agenda) & " ")
                strSql.AppendLine("                 AND   d.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                strSql.AppendLine("                 AND Tipo_Destinazione = 0 ")
                strSql.AppendLine("                 AND m.Data_Movimento >= ip.Validita_Inizio ")
                strSql.AppendLine("                 AND m.Data_Movimento <= ip.Validita_fine ")
                strSql.AppendLine("         ) destinazioni_operazione   ")
                strSql.AppendLine("             ON Imprese_Progetti.piva = destinazioni_operazione.piva  ")
                strSql.AppendLine("             AND Imprese_Progetti.Sa_Cod = destinazioni_operazione.Sa_Cod  ")
                strSql.AppendLine("             AND Imprese_Progetti.Appezza = destinazioni_operazione.Appezza  ")
                strSql.AppendLine("             AND Imprese_Progetti.Id_Reg = destinazioni_operazione.id_reg ")
                strSql.AppendLine("             AND Imprese_Progetti.progetto_cod = destinazioni_operazione.Progetto_Cod ")
                strSql.AppendLine("             AND Movimenti.Data_Movimento >= destinazioni_operazione.Validita_Inizio ")
                strSql.AppendLine("             AND Movimenti.Data_Movimento <= destinazioni_operazione.Validita_fine ")

                strSql.AppendLine("         WHERE Movimenti_dettagli.Elem_Cod = 3 ")
                strSql.AppendLine("         AND Movimenti.Cau_Mov = '2300' ")
                strSql.AppendLine("         AND Movimenti.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                strSql.AppendLine("         AND Mov_Destinazioni.id_Agenda = " & Agro_SQL_SaveNum(ID_Agenda) & " ")

                If xFiltroAggiuntivo <> "" Then
                    strSql.AppendLine("         AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                End If

                strSql.AppendLine("         GROUP BY imprese_progetti.Piva, imprese_progetti.Sa_Cod, imprese_progetti.Appezza, imprese_progetti.Id_reg, imprese_progetti.progetto_cod ")


                strSql.AppendLine(" ) questa")


                strSql.AppendLine("     ON imprese_progetti.piva = questa.piva ")
                strSql.AppendLine("     AND imprese_progetti.sa_cod = questa.sa_cod ")
                strSql.AppendLine("     AND imprese_progetti.appezza = questa.appezza ")
                strSql.AppendLine("     AND imprese_progetti.id_reg = questa.id_reg ")
                strSql.AppendLine("     AND imprese_progetti.progetto_cod = questa.progetto_cod ")

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


    Public Function LeggiUsatoSiNo(ByVal Elem_Cod As Integer,
                                   ByVal Mat_Cod As Integer,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                   ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Dettagli_R.LeggiUsatoSiNo()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""            =>  si leggono tutti i Mov_Destinazioni
        '   Sa_Cod = 0           =>  si leggono tutti i Mov_Destinazioni dell'impresa
        '   Appezza = 0          =>  si leggono tutti i Mov_Destinazioni del centro aziendale
        '   Id_Destinazione = 0  =>  si leggono tutti i Mov_Destinazioni dell'appezzamento
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try


            strSql.Length = 0
            strSql.AppendLine(" SELECT Agenda.Lav_Cod ")
            strSql.AppendLine(" FROM  Movimenti_dettagli ")
            strSql.AppendLine("         INNER JOIN Agenda ON Movimenti_dettagli.PIVA = Agenda.PIVA AND Movimenti_dettagli.Sa_Cod = Agenda.Sa_Cod AND Movimenti_dettagli.Id_Agenda = Agenda.Id_Agenda ")
            strSql.AppendLine(" WHERE 1 = 1")

            If Elem_Cod <> 0 Then
                strSql.AppendLine(" AND Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
            End If

            If Mat_Cod <> 0 Then
                strSql.AppendLine(" AND Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
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

    '========================================================================================================================
    'Leggo la lavorazioni da stringa dei principi attivi
    '------------------------------------------------------------------------------------------------------------------------
    Public Function LeggiLavorazioni_Da_Principi_Attivi(ByVal Piva As String,
                                                        ByVal Sa_Cod As Integer,
                                                        ByVal strPA As String,
                                                        ByVal Av_Cod As Integer,
                                                        ByVal Av_Gru As Integer,
                                                        ByVal Appezza As Integer,
                                                        ByVal Id_Destinazione As Integer,
                                                        ByVal Validita_Inizio As Date,
                                                        ByVal Validita_Fine As Date,
                                                        ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                                        ByVal xFiltroAggiuntivo As String,
                                                        ByVal xOrderBy As String,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                        ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Dettagli.LeggiLavorazioni_Da_Principi_Attivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    strSql.Length = 0

                    strSql.AppendLine(" SELECT Distinct Agenda.*,  Mov_Destinazioni.Qta as Dest_Qta , Movimenti_Dettagli.*, Movimenti_Dettagli.Qta as Dett_Qta   " &
                                " FROM  Agenda , Movimenti , Movimenti_Dettagli , Mov_Destinazioni , Formulati , FormulatixPrincipiAttivi, Mov_Dettaglio_Tecnico " &
                                " WHERE Movimenti_Dettagli.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " " &
                                " AND   Movimenti_Dettagli.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " " &
                                " AND   Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine) & " " &
                                " AND   Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio) &
                                " AND   Agenda.Piva = Movimenti.Piva " &
                                " AND   Agenda.Piva = Movimenti_Dettagli.Piva " &
                                " AND   Agenda.Piva = Mov_Destinazioni.Piva " &
                                " AND   Movimenti.Piva = Movimenti_Dettagli.Piva " &
                                " AND   Movimenti.Piva = Mov_Destinazioni.Piva " &
                                " AND   Movimenti.Piva = Mov_Destinazioni.Piva " &
                                " AND   Movimenti_Dettagli.Piva = Mov_Dettaglio_Tecnico.Piva " &
                                " AND   Agenda.Sa_Cod = Movimenti.Sa_Cod " &
                                " AND   Movimenti_Dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod " &
                                " AND   Movimenti.Sa_Cod = Mov_Dettaglio_Tecnico.Sa_Cod " &
                                " AND   Agenda.Id_Agenda = Movimenti.Id_Agenda " &
                                " AND   Agenda.Id_Agenda = Movimenti_Dettagli.Id_Agenda " &
                                " AND   Agenda.Id_Agenda = Mov_Destinazioni.Id_Agenda " &
                                " AND   Movimenti.Id_Agenda = Movimenti_Dettagli.Id_Agenda " &
                                " AND   Movimenti.Id_Agenda = Mov_Destinazioni.Id_Agenda " &
                                " AND   Movimenti_Dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda " &
                                " AND   Movimenti.Id_Agenda = Mov_Dettaglio_Tecnico.Id_Agenda " &
                                " AND   Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov " &
                                " AND   Movimenti.Id_Mov = Mov_Destinazioni.Id_Mov " &
                                " AND   Movimenti.Id_Mov = Mov_Dettaglio_Tecnico.Id_Mov " &
                                " AND   Movimenti_Dettagli.Id_Mov = Mov_Destinazioni.Id_Mov " &
                                " AND   Movimenti_Dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")

                    If Av_Gru <> 0 OrElse Av_Cod <> 0 Then
                        'Avversità specifica
                        strSql.AppendLine(" AND Mov_Dettaglio_Tecnico.Id_Mov_Det = Movimenti_Dettagli.Id_Mov_Det ")
                    End If

                    strSql.AppendLine(" AND Mov_Destinazioni.Tipo_Destinazione = 0 " &
                                    " AND Formulati.Fr_Cod = FormulatixPrincipiAttivi.Fr_Cod " &
                                    " AND Movimenti_Dettagli.Elem_Cod = 191 " &
                                    " AND Formulati.Fr_Cod = Movimenti_Dettagli.Pro_Cod " &
                                    " AND Movimenti_Dettagli.Mat_Cod = 0 ")

                    If Piva <> "" Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If

                    If Sa_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Trim(strPA) <> "" Then
                        strSql.AppendLine(" AND FormulatixPrincipiAttivi.Pa_Cod IN " & Agro_SQL_Save_Clausola_IN(strPA, False) & "   ")
                    End If

                    If Appezza <> 0 Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
                    End If

                    If Av_Cod <> 0 Then
                        strSql.AppendLine(" AND Mov_Dettaglio_Tecnico.Av_Cod = " & Agro_SQL_SaveNum(Av_Cod) & "   ")
                    End If

                    If Av_Gru <> 0 Then
                        strSql.AppendLine(" AND Mov_Dettaglio_Tecnico.Av_Gru = " & Agro_SQL_SaveNum(Av_Gru) & "   ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Agenda.Validita_Inizio Asc ")
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



    '#############################################################################################################
    '#############################################################################################################
    '#############################################################################################################

    'Se Cau_Mov = "" -> leggo sia i carichi che gli scarichi

    'Nuova Lettura Necessaria Dopo Implementazione Note Accredito/Addebito con gestione RESI
    'Lettura puntuale del prodotto (utilizzata dal pulitore delle giacenze)
    Public Function LeggiCaricoScarico_New_Puntuale(ByVal Piva As String,
                                                    ByVal Sa_Cod As Integer,
                                                    ByVal Id_Agenda As Integer,
                                                    ByVal Id_Mov As Integer,
                                                    ByVal Id_Mov_Det As Integer,
                                                    ByVal Elem_Cod As Integer,
                                                    ByVal Pro_Cod As Integer,
                                                    ByVal Mat_Cod As Integer,
                                                    ByVal Udm_Cod As Integer,
                                                    ByVal Appezza As Integer,
                                                    ByVal Id_Destinazione As Integer,
                                                    ByVal Tipo_Destinazione As Integer,
                                                    ByVal Cau_Mov As String,
                                                    ByVal Cod_Progetto As Integer,
                                                    ByVal Fase_Cod As Integer,
                                                    ByVal Contabilizzato As Integer,
                                                    ByVal Pendente As Integer,
                                                    ByVal Lotto As String,
                                                    ByVal Cal_Cod As Integer,
                                                    ByVal strFiltro_Dettaglio As String,
                                                    ByVal Validita_Inizio As Date,
                                                    ByVal Validita_Fine As Date,
                                                    ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                    ) As DataTable


        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Dettagli_R.LeggiCaricoScarico_New_Puntuale()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    strSql.Length = 0

                    strSql.AppendLine(" SELECT Agenda.* , Movimenti.* , Mov_Destinazioni.* , Movimenti_Dettagli.* , " &
                                " ISNULL(Mov_Destinazioni.Qta,0) as Dest_Qta , ISNULL(Movimenti_Dettagli.Qta,0) as Dett_Qta, " &
                                " Mov_Destinazioni.Tipo_Destinazione, Mov_Destinazioni.Sa_Cod as Sa_Cod_Destinazione, Mov_Destinazioni.Id_Destinazione " &
                                " FROM Agenda, Movimenti, Movimenti_Dettagli " &
                                " Left Outer Join Mov_Destinazioni ON " &
                                "                    (Movimenti_Dettagli.Piva = Mov_Destinazioni.Piva AND  " &
                                "                     Movimenti_Dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod  AND " &
                                "                     Movimenti_Dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND " &
                                "                     Movimenti_Dettagli.Id_Mov = Mov_Destinazioni.Id_Mov  AND " &
                                "                     Movimenti_Dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")


                    If Piva <> "" Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If

                    If Tipo_Destinazione <> 0 Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Tipo_Destinazione = " & Agro_SQL_SaveNum(Tipo_Destinazione) & "   ")
                    End If

                    If Appezza <> 0 Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
                    End If

                    If Id_Destinazione <> 0 Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "   ")
                    End If

                    strSql.AppendLine(")")

                    strSql.AppendLine(" WHERE Movimenti_Dettagli.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " " &
                 " AND   Movimenti_Dettagli.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " " &
                 " AND   Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine) & " " &
                 " AND   Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio))

                    'Join sulla Piva
                    strSql.AppendLine(" AND   Agenda.Piva = Movimenti.Piva " &
                           " AND   Agenda.Piva = Movimenti_Dettagli.Piva " &
                           " AND   Movimenti.Piva = Movimenti_Dettagli.Piva ")


                    'Join sul Id_Agenda
                    strSql.AppendLine(" AND   Agenda.Id_Agenda = Movimenti.Id_Agenda " &
                           " AND   Agenda.Id_Agenda = Movimenti_Dettagli.Id_Agenda " &
                           " AND   Movimenti.Id_Agenda = Movimenti_Dettagli.Id_Agenda ")


                    'Join su Id_Mov
                    strSql.AppendLine(" AND   Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov ")

                    '=========================================================================================================
                    'Lettura Puntuale del Prodotto
                    '---------------------------------------------------------------------------------------------------------
                    strSql.AppendLine(" AND Movimenti_Dettagli.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")

                    strSql.AppendLine(" AND Movimenti_Dettagli.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")

                    strSql.AppendLine(" AND Movimenti_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")

                    strSql.AppendLine(" AND Movimenti_Dettagli.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & "   ")

                    strSql.AppendLine(" AND Movimenti_Dettagli.Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   ")

                    strSql.AppendLine(" AND Movimenti_Dettagli.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   ")

                    strSql.AppendLine(" AND Movimenti_Dettagli.Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "   ")

                    strSql.AppendLine(" AND Upper(Movimenti_Dettagli.Lotto) = '" & UCase(Agro_SQL_SaveText(Lotto)) & "'   ")
                    '=========================================================================================================


                    If Piva <> "" Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If

                    If Sa_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Id_Agenda <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
                    End If

                    If Id_Mov <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
                    End If

                    If Id_Mov_Det <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")
                    End If


                    If Cau_Mov <> "" Then
                        strSql.AppendLine(" AND  Movimenti.Cau_Mov = '" & Agro_SQL_SaveText(Cau_Mov) & "'   ")
                    Else

                        'Sono state aggiunte le causali dei conferimenti
                        strSql.AppendLine(" AND Movimenti.Cau_Mov IN ('7300', '7350', '4100', '4200', '7900', '7920')     ")
                    End If


                    If Contabilizzato <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Contabilizzato = " & Agro_SQL_SaveNum(Contabilizzato) & "   ")
                    End If

                    If Pendente <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Pendente = " & Agro_SQL_SaveNum(Pendente) & "   ")
                    End If


                    If strFiltro_Dettaglio <> "" Then
                        strSql.AppendLine(" AND (" & strFiltro_Dettaglio & ")")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Movimenti_Dettagli.Elem_Cod Asc ")
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


    'Se Cau_Mov = "" -> leggo sia i carichi che gli scarichi
    'Nuova Lettura Necessaria Dopo Implementazione Note Accredito/Addebito con gestione RESI
    '##############################################################################################
    Public Function LeggiCaricoScarico_New(ByVal PIVA As String,
                                           ByVal Sa_Cod As Integer,
                                           ByVal Id_Agenda As Integer,
                                           ByVal Id_Mov As Integer,
                                           ByVal Id_Mov_Det As Integer,
                                           ByVal Elem_Cod As Integer,
                                           ByVal Pro_Cod As Integer,
                                           ByVal Mat_Cod As Integer,
                                           ByVal Udm_Cod As Integer,
                                           ByVal Appezza As Integer,
                                           ByVal Id_Destinazione As Integer,
                                           ByVal Tipo_Destinazione As Integer,
                                           ByVal Cau_Mov As String,
                                           ByVal Cod_Progetto As Integer,
                                           ByVal Fase_Cod As Integer,
                                           ByVal Contabilizzato As Integer,
                                           ByVal Pendente As Integer,
                                           ByVal Lotto As String,
                                           ByVal Cal_Cod As Integer,
                                           ByVal strFiltro_Dettaglio As String,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByVal xOrderBy As String,
                                           ByRef objParametri As AgronicaCoreParametri,
                                           Optional ByVal leggiProdotto As Boolean = False,
                                           Optional ByVal leggiMateriePrimeCampionature As Boolean = False,
                                           Optional ByVal leggiUbicazione As Boolean = False,
                                           Optional ByVal leggiProvvigioni As Boolean = False,
                                           Optional ByVal leggiPesiRiscontrati As Boolean = False,
                                           Optional ByVal leggiContiEconPatr As Boolean = False,
                                           Optional ByVal leggiOrdiniDaEvadere As Boolean = False,
                                           Optional ByVal leggiTrasferimenti As Boolean = False,
                                           Optional ByVal leggiLavorazioni As Boolean = False,
                                           Optional ByVal leggiConferimenti As Boolean = False,
                                           Optional ByVal leggiOrdini As Boolean = False,
                                           Optional ByVal leggiRifOrdineCliente As Boolean = False,
                                           Optional ByVal leggiDDTDaEvadere As Boolean = False,
                                           Optional ByVal leggiCdC As Boolean = False,
                                           Optional ByVal leggiOrdiniLavoroDaEvadere As Boolean = False,
                                           Optional ByVal mostraOrdiniEvasi As Boolean = False,
                                           Optional ByVal leggiGruppiMerce As Boolean = False,
                                           Optional ByVal leggiPratica As Boolean = False
                                           ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Movimenti_Dettagli_R.LeggiCaricoScarico_New()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            Dim objConfigDettagli As New AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli_R
            Dim DTParamQual As New DataTable

            If leggiMateriePrimeCampionature Then
                DTParamQual = objConfigDettagli.Leggi(PIVA,
                                                      0,
                                                      False,
                                                      "Tipo = 1",
                                                      "",
                                                      objParametri)
            End If

            Dim arrLavCodAccettazioniConf As Integer() = New Integer() {
                LAVCOD_ACCETTAZIONE_DIVERSI,
                LAVCOD_DISTINTA_CARICO_ACCETTAZIONE,
                LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE
            } 'LAVCOD_DISTINTA_CARICO, LAVCOD_AUTO_DDT_EMESSO : non usate per i conferimenti

            Dim objGruppiMerce As New AgronicaCoreAnagrafeDAL.Gruppi_Merce_R

            '------------------------------------------------------------------

            strSql.Length = 0

            strSql.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")

            If leggiGruppiMerce Then
                'Creo tabella temporanea dei default dei gruppi
                Dim sqlCreaTempDefault = objGruppiMerce.ComponiSql_CreaTempDefaultGruppiMerce(New List(Of String) From {PIVA}, objParametri, Nothing)
                strSql.AppendLine(sqlCreaTempDefault.ToString())
            End If

            strSql.AppendLine("  SELECT Agenda.* , Movimenti.* , Mov_Destinazioni.* , Movimenti_Dettagli.* , ")

            If leggiOrdiniDaEvadere OrElse leggiDDTDaEvadere Then
                ' Dati Documento
                strSql.AppendLine(" ISNULL(Movimenti_Registrazione.Data_Movimento, cast('1900-01-01' as date)) AS Mov_Registrazione_Data, ")
                strSql.AppendLine(" CASE WHEN Movimenti_Registrazione.Id_Mov IS NOT NULL THEN ")
                strSql.AppendLine("   Movimenti_Registrazione.Doc_Numero_Sin + ")
                strSql.AppendLine("     CASE WHEN LEN(LTRIM(STR(Movimenti_Registrazione.doc_numero,10))) > 5 THEN LTRIM(STR(Movimenti_Registrazione.doc_numero,10)) ")
                strSql.AppendLine("     ELSE REPLICATE('0', 5 - LEN(LTRIM(STR(Movimenti_Registrazione.doc_numero,10)))) + LTRIM(STR(Movimenti_Registrazione.doc_numero,10)) END ")
                strSql.AppendLine("   + Movimenti_Registrazione.Doc_Numero_Des ELSE ")
                strSql.AppendLine(" '' END AS Mov_Registrazione_Numero, ")
            End If

            If leggiProvvigioni Then
                strSql.AppendLine("  ISNULL(Mov_Dettaglio_Tecnico_Extra.Provvigione,0) as Provvigione , ")
            End If

            If leggiOrdini OrElse leggiRifOrdineCliente Then
                strSql.AppendLine("  ISNULL(Mov_Dettaglio_Tecnico_Extra.N_Doc_Cliente,'') as N_Doc_Cliente , ")
                strSql.AppendLine("  ISNULL(Mov_Dettaglio_Tecnico_Extra.Data_Doc_Cliente, cast('1900-01-01' as date)) as Data_Doc_Cliente , ")

                strSql.AppendLine("  ISNULL(Mov_Dettaglio_Tecnico_Extra.N_Nota_DDT,'') as N_Nota_DDT, ")
                strSql.AppendLine("  ISNULL(Mov_Dettaglio_Tecnico_Extra.N_Nota_Riga_DDT,'') as N_Nota_Riga_DDT, ")
                strSql.AppendLine("  ISNULL(Mov_Dettaglio_Tecnico_Extra.Data_Nota_DDT, cast('1900-01-01' as date)) as Data_Nota_DDT, ")
            End If

            If leggiContiEconPatr Then
                strSql.AppendLine("  LTRIM(RTRIM(ISNULL(RicXConti.Id_Riclassificazione,'')             + ' '  +  ISNULL(Conto_Descr,''))) as Descr_Conto_Economico , ")
                strSql.AppendLine("  LTRIM(RTRIM(ISNULL(RicxConti_Patrimonio.Id_Riclassificazione,'')  + ' '  +  ISNULL(Conto_Pat_Descr, ''))) as Descr_Conto_Patrimoniale , ")
            End If

            If leggiPesiRiscontrati Then
                strSql.AppendLine("  CASE WHEN Mov_Dettaglio_Tecnico_extra.Num_Conf_Riscontrate = -1 THEN NULL")
                strSql.AppendLine("  ELSE Mov_Dettaglio_Tecnico_extra.Num_Conf_Riscontrate END As Num_Conf_Riscontrate, ")

                strSql.AppendLine("  CASE WHEN Mov_Dettaglio_Tecnico_extra.Num_Colli_Riscontrati = -1 THEN NULL ")
                strSql.AppendLine("  ELSE Mov_Dettaglio_Tecnico_extra.Num_Colli_Riscontrati END As Num_Colli_Riscontrati, ")

                strSql.AppendLine("  CASE WHEN Mov_Dettaglio_Tecnico_extra.Num_Imballi_Riscontrati = -1 THEN NULL")
                strSql.AppendLine("  ELSE Mov_Dettaglio_Tecnico_extra.Num_Imballi_Riscontrati END As Num_Imballi_Riscontrati, ")

                strSql.AppendLine("  ISNULL(Mov_Dettaglio_Tecnico_extra.Peso_Netto_Riscontrato,0) As Peso_Netto_Riscontrato, ")
                strSql.AppendLine("  ISNULL(Mov_Dettaglio_Tecnico_extra.Peso_Lordo_Riscontrato,0) As Peso_Lordo_Riscontrato, ")

                strSql.AppendLine("  CASE WHEN Mov_Dettaglio_Tecnico_extra.Tara_Unit_Conf_Riscontrata = -1 THEN NULL ")
                strSql.AppendLine("  ELSE Mov_Dettaglio_Tecnico_extra.Tara_Unit_Conf_Riscontrata END As Tara_Unit_Conf_Riscontrata, ")

                strSql.AppendLine("  CASE WHEN Mov_Dettaglio_Tecnico_extra.Tara_Unit_Collo_Riscontrata = -1 THEN NULL ")
                strSql.AppendLine("  ELSE Mov_Dettaglio_Tecnico_extra.Tara_Unit_Collo_Riscontrata END As Tara_Unit_Collo_Riscontrata, ")

                strSql.AppendLine("  CASE WHEN Mov_Dettaglio_Tecnico_extra.Tara_Unit_Imballo_Riscontrata = -1 THEN NULL ")
                strSql.AppendLine("  ELSE Mov_Dettaglio_Tecnico_extra.Tara_Unit_Imballo_Riscontrata END As Tara_Unit_Imballo_Riscontrata, ")
            End If

            strSql.AppendLine("  ISNULL(UnitaMisura.UDM_DES, '') AS Udm_Des, ISNULL(UnitaMisura.UDM_SIM, '') AS Udm_Sim, ")
            strSql.AppendLine("  CASE Movimenti_Dettagli.Elem_Cod ")
            strSql.AppendLine("  WHEN " & ALTRI_BENI & " THEN '" & Agro_SQL_SaveText(Gias.AltriBeniStrumentali) & "'")
            strSql.AppendLine("  WHEN " & RIGA_DESCRIZIONE_LIBERA & " THEN '" & Agro_SQL_SaveText(Gias.RigaDescrizioneLibera) & "'")
            strSql.AppendLine("  WHEN " & SERVIZI & " THEN '" & Agro_SQL_SaveText(Gias.Servizi) & "'")
            strSql.AppendLine("  ELSE ISNULL(CategorieMagazzino.NomeComune, '') END as Elem_Des, ")
            strSql.AppendLine("  ISNULL(Mov_Destinazioni.Qta,0) As Dest_Qta , ISNULL(Movimenti_Dettagli.Qta,0) As Dett_Qta, ")
            strSql.AppendLine("  IVA_Aliquote.Descrizione AS Aliquota_Iva_Des, ")

            'Necessario perché ora legge tutte le righe e quindi Id_Mov_Det è quella di Mov_Destinazioni che potrebbe non esserci
            strSql.AppendLine("  Movimenti_Dettagli.Id_Mov_Det As Dett_Id_Mov_Det, ")
            strSql.AppendLine("  Movimenti_Dettagli.Sa_Cod As Sa_Cod_Mov_Det, ")
            strSql.AppendLine("  ISNULL(Mov_Destinazioni.Tipo_Destinazione, 0) AS Tipo_Destinazione, ISNULL(Mov_Destinazioni.Sa_Cod, 0) As Sa_Cod_Destinazione, ISNULL(Mov_Destinazioni.Id_Destinazione, 0) AS Id_Destinazione, ")
            strSql.AppendLine("  ISNULL(Mov_Destinazioni.qta_dest1, 0) As Dest_Qta_dest1 , ISNULL(Mov_Destinazioni.qta_dest2, 0) As Dest_Qta_dest2, ")
            strSql.AppendLine("  ISNULL(Mov_Dettaglio_Tecnico.N, 0) As N, ")
            strSql.AppendLine("  ISNULL(Mov_Dettaglio_Tecnico.P, 0) As P205, ")
            strSql.AppendLine("  ISNULL(Mov_Dettaglio_Tecnico.K, 0) As K20, ")
            strSql.AppendLine("  ISNULL(Mov_Dettaglio_Tecnico.Cu, 0) As Cu, ")
            strSql.AppendLine("  ISNULL(Mov_Dettaglio_Tecnico.Extra_Int, 0) As cod_regolamento, ")

            ' Impostati a parte per essere sicuro di utilizzare poi queste e non quelle di anagrafica
            strSql.AppendLine("  ISNULL(Movimenti_Dettagli.Qta_Extra, 0) As MovDett_Qta_Extra, ")
            strSql.AppendLine("  ISNULL(Movimenti_Dettagli.Udm_Cod_Extra, 0) As MovDett_Udm_Cod_Extra, ")
            strSql.AppendLine("  ISNULL(Movimenti_Dettagli.Extra_Str, '') As MovDett_Extra_Str, ")
            strSql.AppendLine("  ISNULL(Movimenti_Dettagli.Extra_Int, 0) As MovDett_Extra_Int ")

            If leggiMateriePrimeCampionature Then

                AggiungiColonneParamQualLeggiCaricoScaricoNew(strSql, DTParamQual)

            End If

            If leggiProdotto Then
                strSql.AppendLine(" , COALESCE(Materie_Prime.Mat_Des, ")
                strSql.AppendLine("            Fertilizzanti.Fer_Des, ")
                strSql.AppendLine("            Formulati.Fr_Des, ")
                strSql.AppendLine("            InsettiUtili.Ins_Des, ")
                strSql.AppendLine("            Trappole.Trap_Des, ")
                strSql.AppendLine("            Avversita.Av_Des_Vol, ")
                strSql.AppendLine("            '') AS Descrizione_Prodotto ")
                strSql.AppendLine(" , ISNULL(Materie_Prime.Cod_Articolo, '') AS Cod_Articolo ")
                strSql.AppendLine(" , ISNULL(Materie_Prime.Veg_Cod, 0) AS Veg_Cod ")
                strSql.AppendLine(" , ISNULL(Materie_Prime.Cul_Cod, 0) AS Cul_Cod ")
                strSql.AppendLine(" , ISNULL(Materie_Prime.Regolamento, 0) AS Reg_Cod ")
                strSql.AppendLine(" , ISNULL(Materie_Prime.Linea_Cod, 0) AS MP_Linea_Cod ")
                strSql.AppendLine(" , CASE WHEN Materie_Prime.Elem_Cod != " & CStr(TRASFORMATI_VEGETALI) & " THEN 0 ")
                strSql.AppendLine("        ELSE ")
                strSql.AppendLine("            CASE WHEN Materie_Prime.Mat_Cod_Referenza != 0 THEN Materie_Prime.Mat_Cod_Referenza ")
                strSql.AppendLine("                 ELSE Movimenti_dettagli.Mat_Cod ")
                strSql.AppendLine("            END")
                strSql.AppendLine("   END AS Mat_Cod_OMNI")
            End If

            If leggiUbicazione Then
                strSql.AppendLine(" , Fabbricati.Fabbricato_Des, Fabbricati_Tipi.Tipo_Fabbricato_Cod  ")
                strSql.AppendLine(" , Fabbricati_Tipi.Tipo_Fabbricato_Des, Fabbricati_Tipi.Tipo_Fabbricato_Cod ")
                strSql.AppendLine(" , Cantina_Vasche.Tipo_Destinazione , Cantina_Vasche.Identificativo, Cantina_Vasche.Insieme_Cod ")
                strSql.AppendLine(" , Cantina_Insiemi.insieme_des ")
            End If

            If leggiOrdini OrElse leggiOrdiniDaEvadere OrElse leggiDDTDaEvadere OrElse leggiOrdiniLavoroDaEvadere Then
                strSql.AppendLine(" , Qta_Richiesta, Qta_Evasa, Qta_Residua ")
            End If

            If leggiTrasferimenti Then
                strSql.AppendLine(" , Trasf_Rif.Cau_Mov AS Trasf_Rif_Cau_Mov, Trasf_Rif.Id_Mov AS Trasf_Rif_Id_Mov, Trasf_Rif.Id_Mov_Det AS Trasf_Rif_Id_Mov_Det ")
                strSql.AppendLine(" , Trasf_Rif.Cau_Mov_Rif AS Trasf_Rif_Cau_Mov_Rif, Trasf_Rif.Id_Mov_Rif AS Trasf_Rif_Id_Mov_Rif, Trasf_Rif.Id_Mov_Det_Rif AS Trasf_Rif_Id_Mov_Det_Rif ")
                strSql.AppendLine(" , Trasf_Rif.Qta AS Trasf_Rif_Qta ")

                strSql.AppendLine(" , Trasf_Dest_2.Sa_Cod AS Trasf_Sa_Cod_2, Trasf_Dest_2.Id_Destinazione AS Trasf_Id_Destinazione_2, Trasf_Dest_2.Tipo_Destinazione AS Trasf_Tipo_Destinazione_2 ")

                If leggiUbicazione Then
                    strSql.AppendLine(" , Trasf_Fabbricati_2.Fabbricato_Des AS Trasf_Fabbricato_Des_2, Trasf_Fabbricati_Tipi_2.Tipo_Fabbricato_Cod AS Trasf_Tipo_Fabbricato_Cod_2 ")
                    strSql.AppendLine(" , Trasf_Fabbricati_Tipi_2.Tipo_Fabbricato_Des AS Trasf_Tipo_Fabbricato_Des_2 ")
                    strSql.AppendLine(" , Trasf_Cantina_Vasche_2.Identificativo AS Trasf_Identificativo_2, Trasf_Cantina_Vasche_2.Insieme_Cod AS Trasf_Insieme_Cod_2 ")
                    strSql.AppendLine(" , Trasf_Cantina_Insiemi_2.insieme_des AS Trasf_insieme_des_2 ")
                End If
            End If


            If leggiLavorazioni Then
                strSql.AppendLine(" , Movimenti_10001.Extra_Int Extra_Int_10001 ")
            End If

            If leggiConferimenti Then
                strSql.AppendLine(" , ISNULL(Mov_Dettaglio_Conferimento.Id_Reg_Dettaglio, 0) AS Id_Reg_Dettaglio_Conferimento ")
                strSql.AppendLine(" , ISNULL(Mov_Dettaglio_Conferimento.Tagliando_Pesa, '') AS Tagliando_Pesa ")
                strSql.AppendLine(" , ISNULL(Mov_Dettaglio_Conferimento.Premio_Complessivo, 0) AS Premio_Complessivo ")
                strSql.AppendLine(" , ISNULL(Mov_Dettaglio_Conferimento.Cod_Varieta, 0) AS Cod_Varieta_Conferimento ")
                strSql.AppendLine(" , ISNULL(Mov_Dettaglio_Conferimento.Desc_Appezzamenti, '') AS Desc_Appezzamenti_Conferimento ")
            End If

            If leggiCdC Then
                strSql.AppendLine(" , CASE WHEN ISNULL(CdC_Imputazioni.imputazione_Cod_Des,'') = '' THEN '' ")
                strSql.AppendLine("  ELSE CdC_Imputazioni.imputazione_Cod_Des + ' - ' + CdC_Imputazioni.Imputazione_Nome + ' - ' + ISNULL(CdC_Imputazioni_Classi.imputazione_classe_des, '') END As CdC_Des_Imputazione ")
            End If

            If leggiGruppiMerce Then
                strSql.AppendLine(" ,COALESCE(grpMerce.Id_Gruppo_Merce, #DefaultGruppiMerce.Id_Gruppo_Merce, 0) AS Id_Gruppo_Merce ")
                strSql.AppendLine(" ,CASE ")
                strSql.AppendLine(" WHEN grpMerce.Codice IS NOT NULL THEN grpMerce.Codice + ' ' + grpMerce.Descrizione ")
                strSql.AppendLine(" WHEN #DefaultGruppiMerce.Elem_Cod IS NOT NULL THEN #DefaultGruppiMerce.Codice + ' ' + #DefaultGruppiMerce.Descrizione COLLATE DATABASE_DEFAULT ")
                strSql.AppendLine(" ELSE '' END AS Gruppo_Merce_Desc ")
            End If

            If leggiPratica Then
                strSql.AppendLine(" ,ISNULL(Agenda.Pratica_Cod, 0) AS Pratica_Cod ")
                strSql.AppendLine(" ,ISNULL(Pratiche.Servizio_Cod, 0) AS Pratica_Servizio_Cod")
                strSql.AppendLine(" ,ISNULL(praticheStatiAtt.Stato_Cod, 0) AS Pratica_Stato_Cod")
                strSql.AppendLine(" ,ISNULL(wAnagrStati.WAnagraficaStati_Des, '') AS Pratica_Stato_Des")
                strSql.AppendLine(" ,ISNULL(praticheStatiAtt.Note, '') AS Pratica_Stato_Note")
            End If

            strSql.AppendLine("  FROM Movimenti_Dettagli ")

            'Join Movimenti
            strSql.AppendLine("  INNER JOIN Movimenti ")
            strSql.AppendLine("     ON Movimenti.Piva = Movimenti_Dettagli.Piva ")
            strSql.AppendLine("     AND Movimenti.Id_Agenda = Movimenti_Dettagli.Id_Agenda ")
            strSql.AppendLine("     AND Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov ")

            If leggiOrdiniDaEvadere OrElse leggiDDTDaEvadere Then
                strSql.AppendLine("  INNER JOIN Movimenti AS Movimenti_Registrazione ")
                strSql.AppendLine("     ON Movimenti_Registrazione.Piva = Movimenti_Dettagli.Piva ")
                strSql.AppendLine("     AND Movimenti_Registrazione.Id_Agenda = Movimenti_Dettagli.Id_Agenda ")
                strSql.AppendLine("     AND   Movimenti_Registrazione.Cau_Mov = '" & CAU_REGISTRAZIONI & "'")
            End If

            'Join Agenda
            strSql.AppendLine("  INNER JOIN Agenda ")
            strSql.AppendLine("     ON Agenda.Piva = Movimenti.Piva AND Agenda.Id_Agenda = Movimenti.Id_Agenda ")

            If leggiLavorazioni Then
                'Join Movimenti
                strSql.AppendLine("  INNER JOIN Movimenti AS Movimenti_10001 ")
                strSql.AppendLine("     ON Agenda.Piva = Movimenti_10001.Piva AND Agenda.Id_Agenda = Movimenti_10001.Id_Agenda ")
                strSql.AppendLine("     AND Movimenti_10001.Cau_mov = '" & CAU_LINEA_PRODUZIONE & "' ")
            End If

            'Join UnitaMisura
            strSql.AppendLine("  LEFT JOIN UnitaMisura ")
            strSql.AppendLine("     ON Movimenti_Dettagli.Udm_Cod = UnitaMisura.Udm_Cod ")

            'Join CategorieMagazzino
            strSql.AppendLine("  LEFT JOIN CategorieMagazzino ")
            strSql.AppendLine("     ON Movimenti_Dettagli.Elem_Cod = CategorieMagazzino.Elem_Cod ")

            'Join Aliquota Iva
            strSql.AppendLine("  LEFT JOIN IVA_Aliquote ")
            strSql.AppendLine("     ON IVA_Aliquote.Codice = Movimenti_Dettagli.Cod_Iva ")

            If leggiOrdini OrElse leggiProvvigioni OrElse leggiPesiRiscontrati OrElse leggiRifOrdineCliente Then
                strSql.AppendLine("  LEFT OUTER JOIN Mov_Dettaglio_Tecnico_extra ON ")
                strSql.AppendLine("                      Movimenti_Dettagli.Piva = Mov_Dettaglio_Tecnico_extra.Piva AND  ")
                strSql.AppendLine("                      Movimenti_Dettagli.Sa_Cod = Mov_Dettaglio_Tecnico_extra.Sa_Cod  AND ")
                strSql.AppendLine("                      Movimenti_Dettagli.Id_Agenda = Mov_Dettaglio_Tecnico_extra.Id_Agenda AND ")
                strSql.AppendLine("                      Movimenti_Dettagli.Id_Mov = Mov_Dettaglio_Tecnico_extra.Id_Mov  AND ")
                strSql.AppendLine("                      Movimenti_Dettagli.Id_Mov_Det = Mov_Dettaglio_Tecnico_extra.Id_Mov_Det ")
            End If

            strSql.AppendLine("  LEFT OUTER JOIN Mov_Dettaglio_Tecnico ON ")
            strSql.AppendLine("                      Movimenti_Dettagli.Piva = Mov_Dettaglio_Tecnico.Piva AND  ")
            strSql.AppendLine("                      Movimenti_Dettagli.Sa_Cod = Mov_Dettaglio_Tecnico.Sa_Cod  AND ")
            strSql.AppendLine("                      Movimenti_Dettagli.Id_Agenda = Mov_Dettaglio_Tecnico.Id_Agenda AND ")
            strSql.AppendLine("                      Movimenti_Dettagli.Id_Mov = Mov_Dettaglio_Tecnico.Id_Mov  AND ")
            strSql.AppendLine("                      Movimenti_Dettagli.Id_Mov_Det = Mov_Dettaglio_Tecnico.Id_Mov_Det ")

            If leggiConferimenti Then
                strSql.AppendLine("  LEFT OUTER JOIN Mov_Dettaglio_Conferimento ON ")
                strSql.AppendLine("                      Movimenti_Dettagli.Piva = Mov_Dettaglio_Conferimento.Piva AND  ")
                strSql.AppendLine("                      Movimenti_Dettagli.Sa_Cod = Mov_Dettaglio_Conferimento.Sa_Cod  AND ")
                strSql.AppendLine("                      Movimenti_Dettagli.Id_Agenda = Mov_Dettaglio_Conferimento.Id_Agenda AND ")
                strSql.AppendLine("                      Movimenti_Dettagli.Id_Mov = Mov_Dettaglio_Conferimento.Id_Mov  AND ")
                strSql.AppendLine("                      Movimenti_Dettagli.Id_Mov_Det = Mov_Dettaglio_Conferimento.Id_Mov_Det ")
            End If

            strSql.AppendLine("  LEFT OUTER JOIN Mov_Destinazioni ON ")
            strSql.AppendLine("                    ( Movimenti_Dettagli.Piva = Mov_Destinazioni.Piva AND  ")
            strSql.AppendLine("                      Movimenti_Dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod  AND ")
            strSql.AppendLine("                      Movimenti_Dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND ")
            strSql.AppendLine("                      Movimenti_Dettagli.Id_Mov = Mov_Destinazioni.Id_Mov  AND ")
            strSql.AppendLine("                      Movimenti_Dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")

            If PIVA <> "" Then
                strSql.AppendLine("  AND Mov_Destinazioni.Piva = '" & Agro_SQL_SaveText(PIVA) & "'   ")
            End If

            If Tipo_Destinazione <> 0 Then
                strSql.AppendLine("  AND Mov_Destinazioni.Tipo_Destinazione = " & Agro_SQL_SaveNum(Tipo_Destinazione) & "   ")
            End If

            If Appezza <> 0 Then
                strSql.AppendLine("  AND Mov_Destinazioni.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
            End If

            If Id_Destinazione <> 0 Then
                strSql.AppendLine("  AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "   ")
            End If

            strSql.AppendLine(" )")

            If leggiMateriePrimeCampionature Then
                'JOIN MATERIE_PRIME_CAMPIONATURE
                For Each paramQual In DTParamQual.Rows
                    strSql.AppendLine(" LEFT JOIN Materie_Prime_Campionature AS Materie_Prime_Campionature_" & paramQual("Tabella_Key"))
                    strSql.AppendLine("     ON Movimenti_dettagli.Cal_Cod = Materie_Prime_Campionature_" & paramQual("Tabella_Key") & ".Progressivo ")
                    strSql.AppendLine("     AND Materie_Prime_Campionature_" & paramQual("Tabella_Key") & ".Tipo = 'o" & paramQual("Tabella_Key") & "'")
                    If paramQual("Tipo") = enum_TipoParamQual.CodiceNumerico AndAlso Not ({"cliente", "fornitore"}).Contains(paramQual("Tabella_Key")) Then
                        strSql.AppendLine(" LEFT JOIN OTabelle_Parametri AS OTabelle_Parametri_" & paramQual("Tabella_Key"))
                        strSql.AppendLine("     ON Materie_Prime_Campionature_" & paramQual("Tabella_Key") & ".Tipo_Cod =  OTabelle_Parametri_" & paramQual("Tabella_Key") & ".Tabella_Par_Cod ")
                        strSql.AppendLine("     AND OTabelle_Parametri_" & paramQual("Tabella_Key") & ".Tabella_Cod = '" & paramQual("Tabella_ID") & "'")
                    End If
                Next
                'NOTE
                strSql.AppendLine(" LEFT JOIN Materie_Prime_Campionature AS Materie_Prime_Campionature_ONote")
                strSql.AppendLine("     ON Movimenti_dettagli.Cal_Cod = Materie_Prime_Campionature_ONote.Progressivo ")
                strSql.AppendLine("     AND Materie_Prime_Campionature_ONote.Tipo = 'onote'")
            End If

            If leggiContiEconPatr Then
                'Conto Economico
                strSql.AppendLine(" LEFT OUTER JOIN RicXConti ")
                strSql.AppendLine("     ON    Movimenti_dettagli.PIVA = RicXConti.Piva AND Movimenti_dettagli.Anno = RicXConti.Anno AND Movimenti_dettagli.Ric_Cod = RicXConti.Ric_Cod ")
                strSql.AppendLine("     AND   Movimenti_dettagli.Cod_Conto = RicXConti.Cod_Conto ")
                strSql.AppendLine("     AND   RicXConti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
                strSql.AppendLine("     AND   RicXConti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
                strSql.AppendLine(" LEFT OUTER JOIN Conti ")
                strSql.AppendLine("     ON   Conti.Cod_Conto = Movimenti_dettagli.Cod_Conto ")
                strSql.AppendLine("     AND Conti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
                strSql.AppendLine("     AND   Conti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
                strSql.AppendLine(" LEFT OUTER JOIN Riclassificazioni ")
                strSql.AppendLine("     ON   Riclassificazioni.Ric_Cod = RicXConti.Ric_Cod  AND   Riclassificazioni.Piva = RicXConti.Piva ")
                strSql.AppendLine("     AND   Riclassificazioni.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
                strSql.AppendLine("     AND   Riclassificazioni.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")

                'Conto Patrimoniale
                strSql.AppendLine(" LEFT OUTER JOIN  RicxConti_Patrimonio ")
                strSql.AppendLine("     ON    Movimenti_dettagli.PIVA = RicxConti_Patrimonio.Piva AND Movimenti_dettagli.Anno = RicxConti_Patrimonio.Anno AND Movimenti_dettagli.Ric_Cod_Pat = RicxConti_Patrimonio.Ric_Cod_Pat ")
                strSql.AppendLine("     AND   Movimenti_dettagli.Cod_Conto_Pat = RicxConti_Patrimonio.Cod_Conto_Pat ")
                strSql.AppendLine("     AND   RicxConti_Patrimonio.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
                strSql.AppendLine("     AND   RicxConti_Patrimonio.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
                strSql.AppendLine(" LEFT OUTER JOIN Conti_Patrimonio ")
                strSql.AppendLine("     ON   Conti_Patrimonio.Cod_Conto_Pat = Movimenti_dettagli.Cod_Conto_Pat ")
                strSql.AppendLine("     AND Conti_Patrimonio.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
                strSql.AppendLine("     AND   Conti_Patrimonio.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
                strSql.AppendLine(" LEFT OUTER JOIN Riclassificazioni_Patrimonio ")
                strSql.AppendLine("     ON   Riclassificazioni_Patrimonio.Ric_Cod_Pat = RicxConti_Patrimonio.Ric_Cod_Pat  AND   Riclassificazioni_Patrimonio.Piva = RicxConti_Patrimonio.Piva ")
                strSql.AppendLine("     AND   Riclassificazioni_Patrimonio.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
                strSql.AppendLine("     AND   Riclassificazioni_Patrimonio.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
            End If

            If leggiProdotto Then

                strSql.AppendLine(" LEFT JOIN Materie_Prime ")
                strSql.AppendLine("     ON Movimenti_Dettagli.Elem_Cod = Materie_Prime.Elem_Cod AND Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod ")
                strSql.AppendLine("     AND Movimenti_Dettagli.Elem_Cod in (2,10,200,201,204,205,210,301,304,305,306,307,310,401,700) ")

                'I carburanti non sono più gestiti da banca dati, ma sono prodotti aziendali
                'strSql.AppendLine("  LEFT JOIN Carburanti ON Carburanti.car_cod = Movimenti_Dettagli.Pro_Cod  ")
                'strSql.AppendLine("     AND Movimenti_Dettagli.Elem_Cod = 2 ")

                strSql.AppendLine("  LEFT JOIN Fertilizzanti ON Fertilizzanti.Fer_Cod = Movimenti_Dettagli.Pro_Cod  ")
                strSql.AppendLine("     AND Movimenti_Dettagli.Elem_Cod = 3 ")

                'TODO: da verificare!!!
                strSql.AppendLine("  LEFT JOIN TipologieSementi ON TipologieSementi.Sem_Cod = Materie_Prime.Sem_Cod  ")
                strSql.AppendLine("     AND Movimenti_Dettagli.Elem_Cod = 10 ")

                strSql.AppendLine("  LEFT JOIN Formulati ON Formulati.Fr_Cod = Movimenti_Dettagli.Pro_Cod  ")
                strSql.AppendLine("     AND Movimenti_Dettagli.Elem_Cod = 191 ")

                'Non più usata categoria a parte (integrata in Formulati)
                'strSql.AppendLine("  LEFT JOIN Coadiuvante ON Coadiuvante.Coad_Cod = Movimenti_Dettagli.Pro_Cod  ")
                'strSql.AppendLine("     AND Movimenti_Dettagli.Elem_Cod = 195 ")

                strSql.AppendLine("  LEFT JOIN InsettiUtili ON InsettiUtili.Ins_Cod = Movimenti_Dettagli.Pro_Cod  ")
                strSql.AppendLine("     AND Movimenti_Dettagli.Elem_Cod = 196 ")

                strSql.AppendLine("  LEFT JOIN Trappole ON Trappole.Trap_Cod = Movimenti_Dettagli.Pro_Cod  ")
                strSql.AppendLine("     AND Movimenti_Dettagli.Elem_Cod = 197   ")

                strSql.AppendLine("  LEFT JOIN Avversita ON Avversita.Av_Cod = Movimenti_Dettagli.Pro_Cod  ")
                strSql.AppendLine("     AND Movimenti_Dettagli.Elem_Cod = 198   ")

                strSql.AppendLine("  LEFT JOIN Zoo_Animali ON Zoo_Animali.Piva = Movimenti_Dettagli.Piva  ")
                strSql.AppendLine("     AND Zoo_Animali.Cod_Progetto = Movimenti_Dettagli.Pro_Cod  ")
                strSql.AppendLine("     AND Movimenti_Dettagli.Elem_Cod = 300   ")

            End If

            If leggiUbicazione Then
                'JOIN FABBRICATO
                strSql.AppendLine(" LEFT JOIN Fabbricati ON Mov_Destinazioni.Piva = Fabbricati.PIVA AND Mov_Destinazioni.Sa_Cod = Fabbricati.SA_COD ")
                strSql.AppendLine("     AND Mov_Destinazioni.Id_Destinazione = Fabbricati.Fabbricato_Cod ")

                'JOIN TIPO FABBRICATO
                strSql.AppendLine(" LEFT JOIN Fabbricati_Tipi ON Fabbricati.Tipo_Fabbricato_Cod = Fabbricati_Tipi.Tipo_Fabbricato_Cod ")

                'JOIN Cantina Insiemi per trovare decodifica cella
                strSql.AppendLine(" LEFT JOIN Cantina_Vasche ON Mov_Destinazioni.Piva = Cantina_Vasche.PIVA AND Mov_Destinazioni.Sa_Cod = Cantina_Vasche.SA_COD ")
                strSql.AppendLine("     AND Mov_Destinazioni.tipo_destinazione = Cantina_Vasche.tipo_destinazione AND Mov_Destinazioni.Id_Destinazione = Cantina_Vasche.vas_cod ")

                strSql.AppendLine(" LEFT JOIN Cantina_Insiemi ON Cantina_Vasche.Insieme_Cod = Cantina_Insiemi.Insieme_Cod ")
            End If

            If leggiOrdini OrElse leggiOrdiniDaEvadere OrElse leggiDDTDaEvadere OrElse leggiOrdiniLavoroDaEvadere Then
                Dim listTupleOrdiniMovDetRif As List(Of Tuple(Of Integer, Integer)) = Nothing

                Select Case True

                    Case leggiOrdini, leggiOrdiniDaEvadere
                        listTupleOrdiniMovDetRif = New List(Of Tuple(Of Integer, Integer))() From {
                        New Tuple(Of Integer, Integer)(LAVCOD_ORDINE_VENDITA, LAVCOD_FATTURA_EMESSA),
                        New Tuple(Of Integer, Integer)(LAVCOD_ORDINE_VENDITA, LAVCOD_BOLLA_EMESSA),
                        New Tuple(Of Integer, Integer)(LAVCOD_ORDINE_VENDITA, LAVCOD_MVV_EMESSO),
                        New Tuple(Of Integer, Integer)(LAVCOD_ORDINE_ACQUISTO, LAVCOD_FATTURA_RICEVUTA),
                        New Tuple(Of Integer, Integer)(LAVCOD_ORDINE_ACQUISTO, LAVCOD_BOLLA_RICEVUTA),
                        New Tuple(Of Integer, Integer)(LAVCOD_ORDINE_ACQUISTO, LAVCOD_MVV_RICEVUTO),
                        New Tuple(Of Integer, Integer)(LAVCOD_ORDINE_ACQUISTO, LAVCOD_ACCETTAZIONE_DIVERSI),
                        New Tuple(Of Integer, Integer)(LAVCOD_ORDINE_ACQUISTO, LAVCOD_DISTINTA_CARICO_ACCETTAZIONE),
                        New Tuple(Of Integer, Integer)(LAVCOD_ORDINE_ACQUISTO, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE)
                        }

                    Case leggiDDTDaEvadere
                        listTupleOrdiniMovDetRif = New List(Of Tuple(Of Integer, Integer))() From {
                        New Tuple(Of Integer, Integer)(LAVCOD_BOLLA_EMESSA, LAVCOD_FATTURA_EMESSA),
                        New Tuple(Of Integer, Integer)(LAVCOD_BOLLA_RICEVUTA, LAVCOD_FATTURA_RICEVUTA)
                        }

                    Case leggiOrdiniLavoroDaEvadere
                        listTupleOrdiniMovDetRif = New List(Of Tuple(Of Integer, Integer))() From {
                        New Tuple(Of Integer, Integer)(LAVCOD_TESTATE_ORDINE_LAVORAZIONE, LAVCOD_TRASFORMAZIONI)
                        }

                End Select

                Dim listClausoleOrdiniMovDetRif As New List(Of String)
                For Each tupleOrdiniMovDetRif In listTupleOrdiniMovDetRif
                    listClausoleOrdiniMovDetRif.Add(String.Format("Lav_Cod_Rif = {0} AND Lav_Cod = {1}", tupleOrdiniMovDetRif.Item1, tupleOrdiniMovDetRif.Item2))
                Next
                Dim strClausoleOrdiniMovDetRif = "(" & String.Join(") OR (", listClausoleOrdiniMovDetRif) & ")"

                strSql.AppendLine(" LEFT JOIN (SELECT a.id_mov_det_rif, avg(b.qta) as qta_richiesta, sum(c.qta) as qta_evasa, avg(b.qta)-sum(c.qta) as qta_residua ")
                strSql.AppendLine("             FROM mov_dettagli_riferimenti a ")
                strSql.AppendLine("             INNER JOIN movimenti_dettagli b ON a.id_mov_det_rif = b.id_mov_det ")
                strSql.AppendLine("             INNER JOIN movimenti_dettagli c ON a.id_mov_det = c.id_mov_det ")
                strSql.AppendLine("             WHERE " & strClausoleOrdiniMovDetRif)
                strSql.AppendLine("             GROUP BY a.id_mov_det_rif) Movimenti_Riferimenti ON Movimenti_Dettagli.Id_Mov_Det=Movimenti_Riferimenti.id_mov_det_rif ")
            End If

            If leggiTrasferimenti Then
                strSql.AppendLine(" LEFT JOIN Mov_Dettagli_Riferimenti Trasf_Rif ")
                strSql.AppendLine("     ON Movimenti_Dettagli.Id_Mov_Det=(CASE Movimenti.Cau_Mov WHEN '7350' THEN Trasf_Rif.Id_Mov_Det WHEN '7300' THEN Trasf_Rif.Id_Mov_Det_Rif ELSE -1 END) ")
                strSql.AppendLine("     AND Trasf_Rif.Lav_Cod = 1033 AND Trasf_Rif.Lav_Cod_Rif = 1033 ")
                strSql.AppendLine("     AND Trasf_Rif.Cau_Mov = '7350' AND Trasf_Rif.Cau_Mov_Rif = '7300' ")

                strSql.AppendLine(" -- Contiene la Cella di Carico (se il mio dettaglio principale è lo Scarico) oppure la cella di Scarico (se il mio dettaglio principale è lo scarico) ")
                strSql.AppendLine(" LEFT OUTER JOIN Mov_Destinazioni Trasf_Dest_2 ON ")
                strSql.AppendLine("                   ( Trasf_Rif.Piva = Trasf_Dest_2.Piva AND ")
                strSql.AppendLine("                     Trasf_Rif.Id_Agenda = Trasf_Dest_2.Id_Agenda AND ")
                strSql.AppendLine("   				    (CASE Movimenti.Cau_Mov WHEN '7350' THEN Trasf_Rif.Id_Mov_Rif WHEN '7300' THEN Trasf_Rif.Id_Mov ELSE -1 END) = Trasf_Dest_2.Id_Mov AND ")
                strSql.AppendLine("                     (CASE Movimenti.Cau_Mov WHEN '7350' THEN Trasf_Rif.Id_Mov_Det_Rif WHEN '7300' THEN Trasf_Rif.Id_Mov_Det ELSE -1 END) = Trasf_Dest_2.Id_Mov_Det ")
                strSql.AppendLine("                     ) ")

                If leggiUbicazione Then
                    strSql.AppendLine(" LEFT JOIN Fabbricati Trasf_Fabbricati_2 ON Trasf_Dest_2.Piva = Trasf_Fabbricati_2.PIVA AND Trasf_Dest_2.Sa_Cod = Trasf_Fabbricati_2.SA_COD ")
                    strSql.AppendLine("     AND Trasf_Dest_2.Id_Destinazione = Trasf_Fabbricati_2.Fabbricato_Cod ")
                    strSql.AppendLine(" LEFT JOIN Fabbricati_Tipi Trasf_Fabbricati_Tipi_2 ON Trasf_Fabbricati_2.Tipo_Fabbricato_Cod = Trasf_Fabbricati_Tipi_2.Tipo_Fabbricato_Cod ")
                    strSql.AppendLine(" LEFT JOIN Cantina_Vasche Trasf_Cantina_Vasche_2 ON Trasf_Dest_2.Piva = Trasf_Cantina_Vasche_2.PIVA AND Trasf_Dest_2.Sa_Cod = Trasf_Cantina_Vasche_2.SA_COD ")
                    strSql.AppendLine("     AND Trasf_Dest_2.tipo_destinazione = Trasf_Cantina_Vasche_2.tipo_destinazione AND Trasf_Dest_2.Id_Destinazione = Trasf_Cantina_Vasche_2.vas_cod ")
                    strSql.AppendLine(" LEFT JOIN Cantina_Insiemi Trasf_Cantina_Insiemi_2 ON Trasf_Cantina_Vasche_2.Insieme_Cod = Trasf_Cantina_Insiemi_2.Insieme_Cod ")
                End If

            End If

            If leggiCdC Then
                strSql.AppendLine(" LEFT JOIN Mov_Dettagli_Riferimenti CdC_Rif ")
                strSql.AppendLine("     ON Movimenti_Dettagli.Id_Mov_Det = CdC_Rif.Id_Mov_Det ")
                strSql.AppendLine("     AND CdC_Rif.Lav_Cod_Rif = 4500 ")
                strSql.AppendLine("     AND CdC_Rif.Piva = '" & Agro_SQL_SaveText(PIVA) & "'   ")
                strSql.AppendLine("     AND CdC_Rif.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")

                strSql.AppendLine(" LEFT JOIN Cdg_Testata CdC_Cdg_Testata ")
                strSql.AppendLine("     ON CdC_Rif.piva_rif = CdC_Cdg_Testata.piva ")
                strSql.AppendLine("     AND CdC_Rif.Id_Agenda_Rif = CdC_Cdg_Testata.Id_Agenda AND CdC_Cdg_Testata.Budget = 0 ")

                strSql.AppendLine(" LEFT JOIN Cdg_Dettagli CdC_Cdg_Dettagli ")
                strSql.AppendLine("     ON CdC_Cdg_Testata.piva = CdC_Cdg_Dettagli.piva ")
                strSql.AppendLine("     AND CdC_Cdg_Testata.Id_Cdg = CdC_Cdg_Dettagli.Id_Cdg ")

                strSql.AppendLine(" LEFT JOIN Imputazioni CdC_Imputazioni ")
                strSql.AppendLine("     ON CdC_Cdg_Dettagli.Id_Imputazione = CdC_Imputazioni.Imputazione_Cod ")

                strSql.AppendLine(" LEFT JOIN Imputazioni_Classi CdC_Imputazioni_Classi ")
                strSql.AppendLine("     ON CdC_Imputazioni.Imputazione_Classe_Cod = CdC_Imputazioni_Classi.Imputazione_Classe_Cod ")
            End If

            If leggiGruppiMerce Then
                strSql.AppendLine(" LEFT JOIN Prodotti_Extra_Privata as prodExtraPriv WITH (nolock)")
                strSql.AppendLine("              ON prodExtraPriv.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
                strSql.AppendLine("       AND prodExtraPriv.Elem_Cod = Movimenti_Dettagli.Elem_Cod")
                strSql.AppendLine("       AND prodExtraPriv.Mat_Cod = Movimenti_Dettagli.Mat_Cod")
                strSql.AppendLine("       AND prodExtraPriv.Pro_Cod = Movimenti_Dettagli.Pro_Cod")
                strSql.AppendLine("             AND (prodExtraPriv.Piva = Movimenti_Dettagli.Piva OR Movimenti_Dettagli.Pro_Cod = 0)")
                strSql.AppendLine(" LEFT JOIN Gruppi_Merce as grpMerce WITH (nolock)")
                strSql.AppendLine("              ON grpMerce.Id_Gruppo_Merce = prodExtraPriv.Id_Gruppo_Merce")
                strSql.AppendLine(" LEFT JOIN #DefaultGruppiMerce")
                strSql.AppendLine("              ON #DefaultGruppiMerce.Elem_Cod = Movimenti_Dettagli.Elem_Cod")
            End If

            If leggiPratica Then
                strSql.AppendLine(" LEFT JOIN Pratiche WITH (nolock) ON Pratiche.Pratica_Cod = Agenda.Pratica_Cod AND Pratiche.Piva = Agenda.Piva")
                strSql.AppendLine(" LEFT JOIN Pratiche_Stati_Attuali as praticheStatiAtt WITH (nolock) ON praticheStatiAtt.Pratica_Cod = Agenda.Pratica_Cod AND praticheStatiAtt.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                strSql.AppendLine(" LEFT JOIN WAnagraficaStati as wAnagrStati WITH (nolock) ON wAnagrStati.WAnagraficaStati_Cod = praticheStatiAtt.Stato_Cod ")
            End If

            'Where Principale
            strSql.AppendLine("  WHERE Movimenti_Dettagli.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            strSql.AppendLine("  AND   Movimenti_Dettagli.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            strSql.AppendLine("  AND   Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            strSql.AppendLine("  AND   Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))

            If PIVA <> "" Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Piva = '" & Agro_SQL_SaveText(PIVA) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Id_Agenda <> 0 Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
            End If

            If Id_Mov <> 0 Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
            End If

            If Id_Mov_Det <> 0 Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")
            End If

            If Elem_Cod <> 0 Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
            End If

            If Pro_Cod <> 0 Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
            End If

            If Mat_Cod <> 0 Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
            End If

            If Udm_Cod <> 0 Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   ")
            End If

            If Cau_Mov <> "" Then
                strSql.AppendLine("  AND  Movimenti.Cau_Mov = '" & Agro_SQL_SaveText(Cau_Mov) & "'   ")
            Else
                'Sono state aggiunte le causali dei conferimenti
                strSql.AppendLine("  AND Movimenti.Cau_Mov IN ('7300', '7350', '4100', '4200', '7900', '7920')     ")
            End If

            If Cod_Progetto <> 0 Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & "   ")
            End If

            If Fase_Cod <> 0 Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   ")
            End If

            If Contabilizzato <> 0 Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Contabilizzato = " & Agro_SQL_SaveNum(Contabilizzato) & "   ")
            End If

            If Pendente <> 0 Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Pendente = " & Agro_SQL_SaveNum(Pendente) & "   ")
            End If

            If Cal_Cod <> 0 Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "   ")
            End If

            If Lotto <> "" Then
                strSql.AppendLine("  AND Upper(Movimenti_Dettagli.Lotto) = '" & UCase(Agro_SQL_SaveText(Lotto)) & "'   ")
            End If

            If (leggiOrdiniDaEvadere OrElse leggiOrdiniLavoroDaEvadere) AndAlso Not mostraOrdiniEvasi Then
                strSql.AppendLine("  AND (Qta_Residua IS NULL Or Qta_Residua > 0)  ")
            End If

            If strFiltro_Dettaglio <> "" Then
                strSql.AppendLine("  AND (" & Agro_SQL_Save_xFiltroAggiuntivo(strFiltro_Dettaglio, True, objParametri) & ")")
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Movimenti_Dettagli.Elem_Cod Asc ")
            End If

            If leggiGruppiMerce Then
                Dim sqlCancTempDefault = objGruppiMerce.ComponiSql_CancellaTempDefaultGruppiMerce()
                strSql.AppendLine("")
                strSql.AppendLine(sqlCancTempDefault.ToString())
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

    Private Sub AggiungiColonneParamQualLeggiCaricoScaricoNew(
        ByRef strSql As StringBuilder,
        ByVal DTParamQual As DataTable)

        Dim parametriPomodoro As String() = {"gradobrix", "inerti", "verde", "marcio", "fruttischiacciati", "fruttiimmaturi", "fruttiscottati", "fruttilesionati"}

        Const MatPriCam_ = "Materie_Prime_Campionature_"
        Const OTabParam_ = "OTabelle_Parametri_"
        Const FF_ = "FF_"
        Const Val_Cod = "Val_Cod"
        Const Tipo_Cod = "Tipo_Cod"
        Const TaraCamp = "Tara_Campionatura"
        Const Sigla = "Sigla"
        Const Descr = "Descrizione"
        Const CodGenLink = "Codice_Generazione_Link"
        Const MatCodGenLink = "Mat_Cod_Generazione_Link"

        For Each paramQual In DTParamQual.Rows

            Dim paramQualTipo = paramQual("Tipo")
            Dim paramQualTabKey = paramQual("Tabella_Key")

            If paramQualTipo <> enum_TipoParamQual.CodiceNumerico Then

                Dim nomeColonnaMpc_ValCod = MatPriCam_ & paramQualTabKey & "." & Val_Cod
                Dim nomeColonnaAliasFF_ValCod = FF_ & paramQualTabKey & "_" & Val_Cod

                Select Case paramQualTipo

                    Case enum_TipoParamQual.Numero
                        ' per gestire i decimali nei parametri pomodoro
                        If parametriPomodoro.Contains(paramQualTabKey) Then
                            strSql.AppendLine(" , CONVERT(decimal(12,3),ISNULL(NULLIF(" & nomeColonnaMpc_ValCod & ", ''), 0)) As " & nomeColonnaAliasFF_ValCod)
                        Else
                            strSql.AppendLine(" , COALESCE(convert(float," & nomeColonnaMpc_ValCod & "), 0) As " & nomeColonnaAliasFF_ValCod)
                        End If

                    Case enum_TipoParamQual.Stringa, enum_TipoParamQual.Data
                        strSql.AppendLine(" , COALESCE(" & nomeColonnaMpc_ValCod & ", '') As " & nomeColonnaAliasFF_ValCod)

                End Select

            Else

                Dim nomeColonnaMpc_TipoCod = MatPriCam_ & paramQualTabKey & "." & Tipo_Cod
                Dim nomeColonnaAliasFF_TipoCod = FF_ & paramQualTabKey & "_" & Tipo_Cod

                strSql.AppendLine(" , COALESCE(" & nomeColonnaMpc_TipoCod & ", 0) As " & nomeColonnaAliasFF_TipoCod)

                If Not ({"cliente", "fornitore"}).Contains(paramQualTabKey) Then
                    strSql.AppendLine(" , COALESCE(" & MatPriCam_ & paramQualTabKey & "." & TaraCamp & ", 0) As " & FF_ & paramQualTabKey & "_" & TaraCamp)
                    strSql.AppendLine(" , COALESCE(" & OTabParam_ & paramQualTabKey & "." & Sigla & ", '') As " & FF_ & paramQualTabKey & "_" & Sigla)
                    strSql.AppendLine(" , COALESCE(" & OTabParam_ & paramQualTabKey & "." & Descr & ", '') As " & FF_ & paramQualTabKey & "_" & Descr)
                    strSql.AppendLine(" , COALESCE(" & OTabParam_ & paramQualTabKey & "." & CodGenLink & ", 0) As " & FF_ & paramQualTabKey & "_" & CodGenLink)
                    strSql.AppendLine(" , COALESCE(" & OTabParam_ & paramQualTabKey & "." & MatCodGenLink & ", 0) As " & FF_ & paramQualTabKey & "_" & MatCodGenLink)
                End If

            End If

        Next

        'Note
        strSql.AppendLine(" , COALESCE(Materie_Prime_Campionature_ONote.Descrizione, '') AS FF_ONote_Descrizione ")

    End Sub

    'Se Cau_Mov = "" -> leggo sia i carichi che gli scarichi
    'Qui non vado a leggere la movimenti destinazione perchè ci sono delle casistiche in cui non c'è
    '##############################################################################################
    Public Function LeggiCaricoScarico_NoDest(ByVal PIVA As String,
                                              ByVal Sa_Cod As Integer,
                                              ByVal Id_Agenda As Integer,
                                              ByVal Id_Mov As Integer,
                                              ByVal Id_Mov_Det As Integer,
                                              ByVal Elem_Cod As Integer,
                                              ByVal Pro_Cod As Integer,
                                              ByVal Mat_Cod As Integer,
                                              ByVal Udm_Cod As Integer,
                                              ByVal Jolly_Int As Integer,
                                              ByVal Cau_Mov As String,
                                              ByVal Cod_Progetto As Integer,
                                              ByVal Fase_Cod As Integer,
                                              ByVal Contabilizzato As Integer,
                                              ByVal Pendente As Integer,
                                              ByVal Lotto As String,
                                              ByVal Cal_Cod As Integer,
                                              ByVal xFiltroAggiuntivo As String,
                                              ByVal xOrderBy As String,
                                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                              Optional ByVal leggiProdotto As Boolean = False,
                                              Optional ByVal leggiMateriePrimeCampionature As Boolean = False,
                                              Optional ByVal leggiTareCampionDaAnagrafica As Boolean = False
                                              ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Dettagli_R.LeggiCaricoScarico_NoDest()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            Dim objConfigDettagli As New AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli_R
            Dim DTParamQual As New DataTable
            If leggiMateriePrimeCampionature Then
                DTParamQual = objConfigDettagli.Leggi(PIVA, 0, False, "Tipo = 1", "", objParametri)
            End If

            '------------------------------------------------------------------
            strSql.Length = 0

            strSql.AppendLine("  SELECT Agenda.* , Movimenti.* , Movimenti_Dettagli.* ")
            strSql.AppendLine("  , ISNULL(Movimenti_Dettagli.Qta,0) as Dett_Qta ")

            If leggiMateriePrimeCampionature Then

                For Each paramQual In DTParamQual.Rows

                    If paramQual("Tipo") <> enum_TipoParamQual.CodiceNumerico Then

                        If paramQual("Tipo") = enum_TipoParamQual.Numero Then
                            strSql.AppendLine(" , COALESCE(convert(float,Materie_Prime_Campionature_" & paramQual("Tabella_Key") & ".Val_Cod), 0) As FF_" & paramQual("Tabella_Key") & "_Val_Cod  ")
                        End If

                        If paramQual("Tipo") = enum_TipoParamQual.Stringa Then
                            strSql.AppendLine(" , COALESCE(Materie_Prime_Campionature_" & paramQual("Tabella_Key") & ".Val_Cod, '') As FF_" & paramQual("Tabella_Key") & "_Val_Cod  ")
                        End If

                        If paramQual("Tipo") = enum_TipoParamQual.Data Then
                            strSql.AppendLine(" , COALESCE(Materie_Prime_Campionature_" & paramQual("Tabella_Key") & ".Val_Cod, '') As FF_" & paramQual("Tabella_Key") & "_Val_Cod  ")
                        End If

                    Else

                        strSql.AppendLine(" , COALESCE(Materie_Prime_Campionature_" & paramQual("Tabella_Key") & ".Tipo_Cod, 0) AS FF_" & paramQual("Tabella_Key") & "_Tipo_Cod  ")

                        If Not ({"cliente", "fornitore"}).Contains(paramQual("Tabella_Key")) Then
                            strSql.AppendLine(" , COALESCE(Materie_Prime_Campionature_" & paramQual("Tabella_Key") & ".ChkTara_Campionatura, 0) AS FF_" & paramQual("Tabella_Key") & "_ChkTara_Campionatura  ")
                            strSql.AppendLine(" , COALESCE(Materie_Prime_Campionature_" & paramQual("Tabella_Key") & ".Tara_Campionatura, 0) AS FF_" & paramQual("Tabella_Key") & "_Tara_Campionatura  ")
                            strSql.AppendLine(" , COALESCE(OTabelle_Parametri_" & paramQual("Tabella_Key") & ".Sigla, '') AS FF_" & paramQual("Tabella_Key") & "_Sigla  ")
                            strSql.AppendLine(" , COALESCE(OTabelle_Parametri_" & paramQual("Tabella_Key") & ".Descrizione, '') AS FF_" & paramQual("Tabella_Key") & "_Descrizione ")
                        End If

                    End If

                    If leggiTareCampionDaAnagrafica AndAlso ({"imballaggio", "contenitore", "confezione"}).Contains(paramQual("Tabella_Key")) Then
                        strSql.AppendLine(" , COALESCE(OTabelle_Parametri_" & paramQual("Tabella_Key") & ".Mat_Cod_Generazione_Link, 0) AS FF_" & paramQual("Tabella_Key") & "_Mat_Cod_Generazione_Link  ")
                        strSql.AppendLine(" , COALESCE(Materie_Prime_" & paramQual("Tabella_Key") & ".Tara, 0) AS FF_" & paramQual("Tabella_Key") & "_Tara_Anagrafica  ")
                    End If

                Next

            End If

            If leggiProdotto Then
                strSql.AppendLine(" , Materie_Prime.Mat_Des AS Descrizione_Prodotto, Materie_Prime.Cod_Articolo ")
            End If

            strSql.AppendLine("  FROM Agenda, Movimenti, Movimenti_Dettagli ")


            If leggiMateriePrimeCampionature Then
                'JOIN MATERIE_PRIME_CAMPIONATURE
                For Each paramQual In DTParamQual.Rows
                    strSql.AppendLine(" LEFT JOIN Materie_Prime_Campionature AS Materie_Prime_Campionature_" & paramQual("Tabella_Key"))
                    strSql.AppendLine(" ON Movimenti_dettagli.Cal_Cod = Materie_Prime_Campionature_" & paramQual("Tabella_Key") & ".Progressivo " &
                                      " AND Materie_Prime_Campionature_" & paramQual("Tabella_Key") & ".Tipo = 'o" & paramQual("Tabella_Key") & "'")
                    If Not ({"cliente", "fornitore"}).Contains(paramQual("Tabella_Key")) Then
                        strSql.AppendLine(" LEFT JOIN OTabelle_Parametri AS OTabelle_Parametri_" & paramQual("Tabella_Key"))
                        strSql.AppendLine(" ON Materie_Prime_Campionature_" & paramQual("Tabella_Key") & ".Tipo_Cod = OTabelle_Parametri_" & paramQual("Tabella_Key") & ".Tabella_Par_Cod " &
                                          " AND OTabelle_Parametri_" & paramQual("Tabella_Key") & ".Tabella_Cod = '" & paramQual("Tabella_ID") & "'")
                    End If

                    If leggiTareCampionDaAnagrafica AndAlso ({"imballaggio", "contenitore", "confezione"}).Contains(paramQual("Tabella_Key")) Then
                        strSql.AppendLine(" LEFT JOIN Materie_Prime AS Materie_Prime_" & paramQual("Tabella_Key"))
                        strSql.AppendLine(" ON Materie_Prime_" & paramQual("Tabella_Key") & ".Mat_Cod = OTabelle_Parametri_" & paramQual("Tabella_Key") & ".Mat_Cod_Generazione_Link " &
                                          " AND OTabelle_Parametri_" & paramQual("Tabella_Key") & ".Tabella_Cod = '" & paramQual("Tabella_ID") & "'")
                    End If
                Next

            End If

            If leggiProdotto Then
                'JOIN MOVIMENTI DETTAGLI - MATERIE PRIME
                strSql.AppendLine(" INNER JOIN Materie_Prime ")
                strSql.AppendLine(" ON Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod AND Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod ")
            End If


            strSql.AppendLine("  WHERE Movimenti_Dettagli.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            strSql.AppendLine("  AND   Movimenti_Dettagli.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            strSql.AppendLine("  AND   Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            strSql.AppendLine("  AND   Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))

            'Join sulla Piva
            strSql.AppendLine("  AND   Agenda.Piva = Movimenti.Piva ")
            strSql.AppendLine("  AND   Agenda.Piva = Movimenti_Dettagli.Piva ")
            strSql.AppendLine("  AND   Movimenti.Piva = Movimenti_Dettagli.Piva ")


            'Join sul Id_Agenda
            strSql.AppendLine("  AND   Agenda.Id_Agenda = Movimenti.Id_Agenda ")
            strSql.AppendLine("  AND   Agenda.Id_Agenda = Movimenti_Dettagli.Id_Agenda ")
            strSql.AppendLine("  AND   Movimenti.Id_Agenda = Movimenti_Dettagli.Id_Agenda ")


            'Join su Id_Mov
            strSql.AppendLine("  AND   Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov ")


            If PIVA <> "" Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Piva = '" & Agro_SQL_SaveText(PIVA) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Id_Agenda <> 0 Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
            End If

            If Id_Mov <> 0 Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
            End If

            If Id_Mov_Det <> 0 Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")
            End If

            If Elem_Cod <> 0 Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
            End If

            If Pro_Cod <> 0 Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
            End If

            If Mat_Cod <> 0 Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
            End If

            If Udm_Cod <> 0 Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   ")
            End If

            If Jolly_Int <> -1 Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Jolly_int = " & Agro_SQL_SaveNum(Jolly_Int) & "   ")
            End If

            If Cau_Mov <> "" Then
                strSql.AppendLine("  AND  Movimenti.Cau_Mov = '" & Agro_SQL_SaveText(Cau_Mov) & "'   ")
            Else

                'Sono state aggiunte le causali dei conferimenti
                strSql.AppendLine("  AND Movimenti.Cau_Mov IN ('7300', '7350', '4100', '4200', '7900', '7920')     ")
            End If

            If Cod_Progetto <> 0 Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & "   ")
            End If

            If Fase_Cod <> 0 Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   ")
            End If

            If Contabilizzato <> 0 Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Contabilizzato = " & Agro_SQL_SaveNum(Contabilizzato) & "   ")
            End If

            If Pendente <> 0 Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Pendente = " & Agro_SQL_SaveNum(Pendente) & "   ")
            End If

            If Cal_Cod <> 0 Then
                strSql.AppendLine("  AND Movimenti_Dettagli.Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "   ")
            End If

            If Lotto <> "" Then
                strSql.AppendLine("  AND Upper(Movimenti_Dettagli.Lotto) = '" & UCase(Agro_SQL_SaveText(Lotto)) & "'   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine("  AND (" & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & ")")
            End If


            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Movimenti_Dettagli.Elem_Cod Asc ")
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


    Public Function LeggiLavorazioni_Da_Principi_Attivi2(ByVal Piva As String,
                                                         ByVal Sa_Cod As Integer,
                                                         ByVal strPA As String,
                                                         ByVal strAvversita As String,
                                                         ByVal Appezza As Integer,
                                                         ByVal Id_Destinazione As Integer,
                                                         ByVal Validita_Inizio As Date,
                                                         ByVal Validita_Fine As Date,
                                                         ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                                         ByVal xFiltroAggiuntivo As String,
                                                         ByVal xOrderBy As String,
                                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                         ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Dettagli.LeggiLavorazioni_Da_Principi_Attivi2()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    strSql.Length = 0

                    strSql.AppendLine(" SELECT Distinct Agenda.*,  Mov_Destinazioni.Qta as Dest_Qta , Movimenti_Dettagli.*, Movimenti_Dettagli.Qta as Dett_Qta   " &
                                " FROM  Agenda , Movimenti , Movimenti_Dettagli , Mov_Destinazioni , Formulati , FormulatixPrincipiAttivi, Mov_Dettaglio_Tecnico " &
                                " WHERE Movimenti_Dettagli.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " " &
                                " AND   Movimenti_Dettagli.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " " &
                                " AND   Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine) & " " &
                                " AND   Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio) &
                                " AND   Agenda.Piva = Movimenti.Piva " &
                                " AND   Agenda.Piva = Movimenti_Dettagli.Piva " &
                                " AND   Agenda.Piva = Mov_Destinazioni.Piva " &
                                " AND   Movimenti.Piva = Movimenti_Dettagli.Piva " &
                                " AND   Movimenti.Piva = Mov_Destinazioni.Piva " &
                                " AND   Movimenti.Piva = Mov_Destinazioni.Piva " &
                                " AND   Movimenti_Dettagli.Piva = Mov_Dettaglio_Tecnico.Piva " &
                                " AND   Agenda.Sa_Cod = Movimenti.Sa_Cod " &
                                " AND   Movimenti_Dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod " &
                                " AND   Movimenti.Sa_Cod = Mov_Dettaglio_Tecnico.Sa_Cod " &
                                " AND   Agenda.Id_Agenda = Movimenti.Id_Agenda " &
                                " AND   Agenda.Id_Agenda = Movimenti_Dettagli.Id_Agenda " &
                                " AND   Agenda.Id_Agenda = Mov_Destinazioni.Id_Agenda " &
                                " AND   Movimenti.Id_Agenda = Movimenti_Dettagli.Id_Agenda " &
                                " AND   Movimenti.Id_Agenda = Mov_Destinazioni.Id_Agenda " &
                                " AND   Movimenti_Dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda " &
                                " AND   Movimenti.Id_Agenda = Mov_Dettaglio_Tecnico.Id_Agenda " &
                                " AND   Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov " &
                                " AND   Movimenti.Id_Mov = Mov_Destinazioni.Id_Mov " &
                                " AND   Movimenti.Id_Mov = Mov_Dettaglio_Tecnico.Id_Mov " &
                                " AND   Movimenti_Dettagli.Id_Mov = Mov_Destinazioni.Id_Mov " &
                                " AND   Movimenti_Dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")

                    If Trim(strAvversita) <> "" Then
                        'Avversità specifica
                        strSql.AppendLine(" AND Mov_Dettaglio_Tecnico.Id_Mov_Det = Movimenti_Dettagli.Id_Mov_Det ")
                    End If

                    strSql.AppendLine(" AND Mov_Destinazioni.Tipo_Destinazione = 0 " &
                                    " AND Formulati.Fr_Cod = FormulatixPrincipiAttivi.Fr_Cod " &
                                    " AND Movimenti_Dettagli.Elem_Cod = 191 " &
                                    " AND Formulati.Fr_Cod = Movimenti_Dettagli.Pro_Cod " &
                                    " AND Movimenti_Dettagli.Mat_Cod = 0 ")

                    If Piva <> "" Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If

                    If Sa_Cod <> 0 Then
                        strSql.AppendLine(" AND Movimenti_Dettagli.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Trim(strPA) <> "" Then
                        strSql.AppendLine(" AND FormulatixPrincipiAttivi.Pa_Cod IN " & Agro_SQL_Save_Clausola_IN(strPA, False) & "   ")
                    End If

                    If Appezza <> 0 Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
                    End If

                    If Trim(strAvversita) <> "" Then
                        strSql.AppendLine(" AND (( Mov_Dettaglio_Tecnico.Av_Cod IN " & Agro_SQL_Save_Clausola_IN(strAvversita, False) & " ) OR  " &
                                      "      ( Mov_Dettaglio_Tecnico.Av_Gru <> 0 AND Mov_Dettaglio_Tecnico.Av_Gru In (select AvversitaxGruppoAvversita.Av_Gru FROM AvversitaxGruppoAvversita Where Av_Cod IN " & Agro_SQL_Save_Clausola_IN(strAvversita, False) & " ) )) ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Agenda.Validita_Inizio Asc")
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

    '############################################################################################
    'non deve essere letta mov_destinazioni!!!! copiatyo da NewCom_MovimentiDettagli_Leggi
    Public Function MovimentiDettagli_Leggi(ByVal Piva As String,
                                            ByVal Sa_Cod As Integer,
                                            ByVal Id_Agenda As Integer,
                                            ByVal Id_Mov As Integer,
                                            ByVal Id_Mov_Det As Integer,
                                            ByVal Elem_Cod As Integer,
                                            ByVal Pro_Cod As Integer,
                                            ByVal Mat_Cod As Integer,
                                            ByVal Cod_Progetto As Integer,
                                            ByVal Fase_Cod As Integer,
                                            ByVal Lotto As String,
                                            ByVal Cal_Cod As Integer,
                                            ByVal Udm_Cod As Integer,
                                            ByVal Lav_Cod As Integer,
                                            ByVal Cau_Mov As String,
                                            ByVal Flag_NoGiacenze As Boolean,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Dettagli.MovimentiDettagli_Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As String = ""
        Dim dt As DataTable

        '----------------------------------------------------
        '--- Preparo la Query SQL ---------------------------
        '----------------------------------------------------

        Try
            strSql &= " SELECT Imprese.Rag_Soc, ISNULL(Centri_Aziendali.Sa_Nome,'') AS Sa_Nome, "
            strSql &= " Agenda.*, Movimenti. *, Movimenti_dettagli.*, "
            strSql &= " CategorieMagazzino.*, UnitaMisura.Udm_Des, UnitaMisura.Udm_Sim "



            'JOIN AGENDA - MOVIMENTI
            'NO Join sul Sa_Cod
            strSql &= " FROM    Agenda INNER JOIN Movimenti "
            strSql &= " ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda "

            'JOIN IMPRESE - AGENDA
            strSql &= " INNER JOIN Imprese ON Imprese.Piva = Agenda.Piva "

            'JOIN MOVIMENTI - MOVIMENTI DETTAGLI
            strSql &= " INNER JOIN Movimenti_dettagli "
            strSql &= " ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov "

            'JOIN MOVIMENTI DETTAGLI - CENTRI AZIENDALI
            strSql &= " INNER JOIN Centri_Aziendali ON Movimenti_dettagli.Piva = Centri_Aziendali.Piva AND Movimenti_dettagli.Sa_Cod = Centri_Aziendali.Sa_Cod "


            'JOIN CATEGORIE MAGAZZINO
            strSql &= " INNER JOIN CategorieMagazzino ON Movimenti_dettagli.Elem_Cod = CategorieMagazzino.Elem_Cod "

            'JOIN UNITA DI MISURA
            strSql &= " LEFT OUTER JOIN UnitaMisura ON Movimenti_dettagli.Udm_Cod = UnitaMisura.Udm_Cod "



            strSql &= " WHERE 1=1 "

            ''CONDIZIONI
            'StrSQL &= " WHERE     Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " "
            'StrSQL &= " AND     Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " "

            If Flag_NoGiacenze Then
                strSql &= " AND Agenda.Lav_Cod > 0 "
            End If

            If Piva <> "" Then
                strSql &= " AND Movimenti_Dettagli.Piva = '" & Agro_SQL_SaveText(Piva) & "'   "
            End If

            If Sa_Cod <> 0 Then
                strSql &= " AND Movimenti_Dettagli.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   "
            End If

            If Id_Agenda <> 0 Then
                strSql &= " AND Movimenti_Dettagli.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   "
            End If

            If Id_Mov <> 0 Then
                strSql &= " AND Movimenti_Dettagli.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   "
            End If

            If Id_Mov_Det <> 0 Then
                strSql &= " AND Movimenti_Dettagli.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   "
            End If

            If Cau_Mov <> "" Then
                strSql &= " AND Movimenti.Cau_Mov = '" & Agro_SQL_SaveText(Cau_Mov) & "'   "
            End If

            If Lav_Cod <> 0 Then
                strSql &= " AND Agenda.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & "   "
            End If

            If Elem_Cod <> 0 Then
                strSql &= " AND Movimenti_Dettagli.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   "
            End If

            If Pro_Cod <> 0 Then
                strSql &= " AND Movimenti_Dettagli.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   "
            End If

            If Mat_Cod <> 0 Then
                strSql &= " AND Movimenti_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   "
            End If

            If Cod_Progetto <> 0 Then
                strSql &= " AND Movimenti_Dettagli.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & "   "
            End If

            If Fase_Cod <> 0 Then
                strSql &= " AND Movimenti_Dettagli.Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   "
            End If

            If Lotto <> LOTTO_NONDEFINITO Then
                strSql &= " AND Movimenti_Dettagli.Lotto = '" & Agro_SQL_SaveText(Lotto) & "'   "
            End If

            If Cal_Cod <> 0 Then
                strSql &= " AND Movimenti_Dettagli.Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "   "
            End If

            If Udm_Cod <> 0 Then
                strSql &= " AND Movimenti_Dettagli.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   "
            End If


            If xFiltroAggiuntivo <> "" Then
                strSql &= Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri)
            End If

            If xOrderBy <> "" Then
                strSql &= Agro_SQL_Save_xOrderBy(xOrderBy)
            Else
                strSql &= " ORDER BY Agenda.PIVA, Movimenti.Data_Movimento DESC "
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






    '############################################################################################ 
    'Lettura degli id_mov_det da inserire nel gruppo per scheduling documenti contabili
    Public Function Elenco_Dettagli_Scheduling_Documenti_Contabili(ByVal Piva As String,
                                                                   ByVal Cau_Mov As String,
                                                                   ByVal xFiltroAggiuntivo As String,
                                                                   ByVal xOrderBy As String,
                                                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                   ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Dettagli.Elenco_Dettagli_Scheduling_Documenti_Contabili()"

        Dim messaggioErrore As String = ""
        Dim strSql As String = ""
        Dim dt As DataTable

        '----------------------------------------------------
        '--- Preparo la Query SQL ---------------------------
        '----------------------------------------------------

        Try
            strSql &= " SELECT Distinct Agenda.Piva, Agenda.ID_Agenda, Agenda.Lav_Cod, Movimenti_Dettagli.ID_Mov_Det, "
            strSql &= " IsNUll(Pagamenti.Cau_Pagamento, '9999') as Cau_Pagamento, Movimenti.Cod_IndirizzoRisUm, Movimenti.Cod_Destinazione, Movimenti.Sezionale_Cod "

            'Nota: Cau_Pagamento = 9999 --> da impostare da anagrafe

            'JOIN AGENDA - MOVIMENTI
            'NO Join sul Sa_Cod
            strSql &= " FROM    Agenda INNER JOIN Movimenti "
            strSql &= " ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda "

            'JOIN IMPRESE - AGENDA
            strSql &= " Left Outer JOIN Pagamenti ON (Movimenti.Piva = Pagamenti.Piva AND Movimenti.Id_Agenda = Pagamenti.Id_Agenda)  "

            'JOIN IMPRESE - AGENDA
            strSql &= " INNER JOIN Imprese ON Imprese.Piva = Agenda.Piva "

            'JOIN MOVIMENTI - MOVIMENTI DETTAGLI
            strSql &= " INNER JOIN Movimenti_dettagli "
            strSql &= " ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov "

            'JOIN CATEGORIE MAGAZZINO
            strSql &= " Left Outer JOIN CategorieMagazzino ON Movimenti_dettagli.Elem_Cod = CategorieMagazzino.Elem_Cod "

            'JOIN UNITA DI MISURA
            strSql &= " LEFT OUTER JOIN UnitaMisura ON Movimenti_dettagli.Udm_Cod = UnitaMisura.Udm_Cod "



            strSql &= " WHERE 1=1 "



            If Piva <> "" Then
                strSql &= " AND Movimenti_Dettagli.Piva = '" & Agro_SQL_SaveText(Piva) & "'   "
            End If

            If Cau_Mov <> "" Then
                strSql &= " AND Movimenti.Cau_Mov = '" & Agro_SQL_SaveText(Cau_Mov) & "'   "
            End If


            If xFiltroAggiuntivo <> "" Then
                strSql &= " AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo)
            End If

            If xOrderBy <> "" Then
                strSql &= Agro_SQL_Save_xOrderBy(xOrderBy)
            Else
                strSql &= " ORDER BY Agenda.Piva, Agenda.Id_Agenda, Movimenti_Dettagli.Id_Mov_Det "
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


    '############################################
    'per non filtrare il sa_cod, passare il valore SACOD_NOFILTRO
    'perchè 0 è sigificativo
    Public Function LeggiOperazioneMacchine(ByVal Piva As String,
                                            ByVal Sa_Cod As Integer,
                                            ByVal Id_Agenda As Integer,
                                            ByVal Id_Mov As Integer,
                                            ByVal Id_Mov_Det As Integer,
                                            ByVal Mac_Cod As Integer,
                                            ByVal Sa_Cod_Macchine As Integer,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Dettagli_R.LeggiOperazioneMacchine()"

        '====================================================================================
        'Parametri opzionali :
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT lav_cod, des_lib, Movimenti_Dettagli.* ")
            strSql.AppendLine(" FROM  Movimenti_Dettagli ")
            strSql.AppendLine(" INNER JOIN Agenda ON Agenda.Piva = Movimenti_Dettagli.Piva AND Agenda.Id_Agenda = Movimenti_Dettagli.Id_Agenda")
            strSql.AppendLine(" INNER JOIN Parco_Macchine ")
            strSql.AppendLine(" ON Movimenti_Dettagli.Mat_Cod = Parco_Macchine.Mac_Cod ")

            strSql.AppendLine(" WHERE Movimenti_Dettagli.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            strSql.AppendLine(" AND   Movimenti_Dettagli.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            strSql.AppendLine(" AND Elem_Cod = " & Agro_SQL_SaveNum(MACCHINE) & "   ")
            strSql.AppendLine(" AND Lav_Cod = " & Agro_SQL_SaveNum(LAVCOD_ACQUISTO_BENI) & "   ")
            strSql.AppendLine(" AND Pro_Cod = 0   ")

            If Piva <> "" Then
                strSql.AppendLine(" AND Movimenti_Dettagli.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND Movimenti_Dettagli.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Id_Agenda <> 0 Then
                strSql.AppendLine(" AND Movimenti_Dettagli.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
            End If

            If Id_Mov <> 0 Then
                strSql.AppendLine(" AND Movimenti_Dettagli.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
            End If

            If Id_Mov_Det <> 0 Then
                strSql.AppendLine(" AND Movimenti_Dettagli.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")
            End If

            If Mac_Cod <> 0 Then
                strSql.AppendLine(" AND Movimenti_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mac_Cod) & "   ")
            End If

            If Sa_Cod <> SACOD_NOFILTRO Then
                strSql.AppendLine(" AND    Parco_Macchine.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Inviato =-1 ")
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

    '############################################
    Public Function LeggiRilievoAttivita(ByVal Piva As String,
                                         ByVal Sa_Cod As Integer,
                                         ByVal Cod_Risum As Integer,
                                         ByVal Validita_Inizio As Date,
                                         ByVal Validita_Fine As Date,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByVal xOrderBy As String,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                         ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Dettagli_R.LeggiOperazioneMacchine()"

        '====================================================================================
        'Parametri opzionali :
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT Movimenti_dettagli.Id_Agenda, Movimenti_dettagli.Id_Mov, Movimenti_dettagli.Id_Mov_Det, ")
            strSql.AppendLine(" Agenda.Piva, Imprese.rag_soc, Agenda.Sa_Cod, Centri_Aziendali.Sa_Nome, Movimenti_dettagli.Mat_Cod, Movimenti.Data_Movimento, ")
            strSql.AppendLine(" Movimenti_dettagli.Turno_Cod, Turni.Turno_Des, Movimenti_dettagli.ID_Attivita, Attivita.[Desc], Movimenti_dettagli.Dettaglio_VegCod, SpecieVegetali.Veg_Des, Movimenti_dettagli.Qta ")
            strSql.AppendLine(" FROM Agenda ")
            strSql.AppendLine(" INNER JOIN Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda ")
            strSql.AppendLine(" INNER JOIN Movimenti_dettagli ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod AND ")
            strSql.AppendLine(" Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov ")
            strSql.AppendLine(" INNER JOIN Imprese ON Agenda.PIVA = Imprese.PIVA AND Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod ")
            strSql.AppendLine(" INNER JOIN Centri_Aziendali ON Agenda.PIVA = Centri_Aziendali.PIVA AND Agenda.Sa_Cod = Centri_Aziendali.Sa_Cod ")
            strSql.AppendLine(" INNER JOIN SpecieVegetali ON Movimenti_dettagli.Dettaglio_VegCod = SpecieVegetali.Veg_Cod ")
            strSql.AppendLine(" INNER JOIN Turni ON Movimenti_dettagli.Turno_Cod = Turni.Turno_Cod ")
            strSql.AppendLine(" INNER JOIN Attivita ON Movimenti_dettagli.ID_Attivita = Attivita.ID_Attivita ")

            strSql.AppendLine(" WHERE Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            strSql.AppendLine(" AND   Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            strSql.AppendLine(" AND Lav_Cod = " & Agro_SQL_SaveNum(LAVCOD_ALTRE_OPERAZIONI) & "   ")
            strSql.AppendLine(" AND Elem_Cod = 0 ")
            strSql.AppendLine(" AND Pro_Cod = 0   ")
            strSql.AppendLine(" AND Movimenti.cau_Mov='" & Agro_SQL_SaveText(CAU_IMPUTAZIONE_MANODOPERA) & "'   ")

            strSql.AppendLine(" AND Attivita.Piva='" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            strSql.AppendLine(" AND Turni.Piva='" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")

            If Piva <> "" Then
                strSql.AppendLine(" AND Movimenti_Dettagli.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND Movimenti_Dettagli.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Cod_Risum <> 0 Then
                strSql.AppendLine(" AND Movimenti_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Cod_Risum) & "   ")
            End If


            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Agenda.Inviato >=0 ")
                    strSql.AppendLine(" AND   Movimenti.Inviato >=0 ")
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Agenda.Inviato =-1 ")
                    strSql.AppendLine(" AND   Movimenti.Inviato =-1 ")
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Inviato =-1 ")
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


    '############################################################################################
    Public Function MovimentiDettagli_Leggi_FF(ByVal Piva As String,
                                               ByVal Sa_Cod As Integer,
                                               ByVal Cod_Contatto As String,
                                               ByVal Id_Agenda As Integer,
                                               ByVal Id_Mov As Integer,
                                               ByVal Id_Mov_Det As Integer,
                                               ByVal Elem_Cod As Integer,
                                               ByVal Pro_Cod As Integer,
                                               ByVal Mat_Cod As Integer,
                                               ByVal Cod_Progetto As Integer,
                                               ByVal Fase_Cod As Integer,
                                               ByVal Lotto As String,
                                               ByVal Cal_Cod As Integer,
                                               ByVal Udm_Cod As Integer,
                                               ByVal Lav_Cod As Integer(),
                                               ByVal Cau_Mov As String(),
                                               ByVal Cau_Agg As Integer,
                                               ByVal Flag_Jolly_Int As Short,
                                               ByVal xSelectAggiuntiva As String,
                                               ByVal xJoinAggiuntiva As String,
                                               ByVal xFiltroAggiuntivo As String,
                                               ByVal xOrderBy As String,
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                               Optional ByVal Id_Destinazione As Integer = 0,
                                               Optional Tipo_Destinazione As Integer = 0) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Dettagli.MovimentiDettagli_Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As String = ""
        Dim dt As DataTable

        '----------------------------------------------------
        '--- Preparo la Query SQL ---------------------------
        '----------------------------------------------------

        Try

            Dim objConfigDettagli As New AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli_R
            Dim DTParamQual As New DataTable
            DTParamQual = objConfigDettagli.Leggi(Piva, 0, False, "Tipo = 1", "", objParametri)

            strSql &= " SELECT  ISNULL(Contatti.Cod_Contatto, 0) As Cod_Contatto, ISNULL(Contatti.Rag_Soc, '') As Rag_Soc, "
            strSql &= " Agenda.*, Movimenti. *, Movimenti_dettagli.*, Materie_Prime.Elem_Cod, Materie_Prime.Veg_Cod, Materie_Prime.Cul_Cod, Materie_Prime.Mat_Cod, Materie_Prime.Mat_Des,  Materie_Prime.Cod_Articolo, Materie_Prime.Linea_Cod As MP_Linea_Cod, "
            strSql &= " CategorieMagazzino.*, UnitaMisura.Udm_Des, UnitaMisura.Udm_Sim, "

            For Each paramQual In DTParamQual.Rows

                If paramQual("Tipo") <> enum_TipoParamQual.CodiceNumerico Then

                    If paramQual("Tipo") = enum_TipoParamQual.Numero Then
                        strSql &= " COALESCE(convert(float,Materie_Prime_Campionature_" & paramQual("Tabella_Key") & ".Val_Cod), 0) AS " & paramQual("Tabella_Key") & "_Cod ,  "
                    End If

                    If paramQual("Tipo") = enum_TipoParamQual.Stringa Then
                        strSql &= " COALESCE(Materie_Prime_Campionature_" & paramQual("Tabella_Key") & ".Val_Cod, '') AS " & paramQual("Tabella_Key") & "_Cod ,  "
                    End If

                    If paramQual("Tipo") = enum_TipoParamQual.Data Then
                        strSql &= " COALESCE(Materie_Prime_Campionature_" & paramQual("Tabella_Key") & ".Val_Cod, '') AS " & paramQual("Tabella_Key") & "_Cod ,  "
                    End If

                Else

                    strSql &= " COALESCE(Materie_Prime_Campionature_" & paramQual("Tabella_Key") & ".Tipo_Cod, 0) AS " & paramQual("Tabella_Key") & "_Cod ,  "

                    If ({"imballaggio", "contenitore", "confezione"}).Contains(paramQual("Tabella_Key")) Then
                        strSql &= " COALESCE(Materie_Prime_Campionature_" & paramQual("Tabella_Key") & ".Tara_Campionatura, 0) AS " & paramQual("Tabella_Key") & "_Tara_Campionatura, "
                    End If

                    If Not ({"cliente", "fornitore"}).Contains(paramQual("Tabella_Key")) Then
                        strSql &= " COALESCE(OTabelle_Parametri_" & paramQual("Tabella_Key") & ".Sigla, '') AS " & paramQual("Tabella_Key") & "_Sigla  , "
                        strSql &= " COALESCE(OTabelle_Parametri_" & paramQual("Tabella_Key") & ".Descrizione, '') AS " & paramQual("Tabella_Key") & "_Descrizione ,  "
                    End If

                End If

            Next

            strSql &= " ISNULL(lp.codice_generazione, 0) as codice_generazione, "
            strSql &= " ISNULL(Mov_Destinazioni.Qta,0) as Dest_Qta, ISNULL(Movimenti_Dettagli.Qta,0) as Dett_Qta, "
            strSql &= " ISNULL(Mov_Destinazioni.Tipo_Destinazione,0) as Tipo_Destinazione, ISNULL(Mov_Destinazioni.Sa_Cod,0) as Sa_Cod_Destinazione, ISNULL(Mov_Destinazioni.Id_Destinazione,0) as Id_Destinazione "
            strSql &= xSelectAggiuntiva

            'JOIN AGENDA - MOVIMENTI
            'NO Join sul Sa_Cod
            strSql &= " FROM    Agenda INNER JOIN Movimenti "
            strSql &= " ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda "

            'JOIN MOVIMENTI - MOVIMENTI DETTAGLI
            strSql &= " INNER JOIN Movimenti_dettagli "
            strSql &= " ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov "

            'LEFT MOVIMENTI DESTINAZIONI
            strSql &= "  Left Outer Join Mov_Destinazioni ON "
            strSql &= "                     (Movimenti_Dettagli.Piva = Mov_Destinazioni.Piva AND  "
            strSql &= "                      Movimenti_Dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod  AND "
            strSql &= "                      Movimenti_Dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND "
            strSql &= "                      Movimenti_Dettagli.Id_Mov = Mov_Destinazioni.Id_Mov  AND "
            strSql &= "                      Movimenti_Dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det "
            If Piva <> "" Then
                strSql &= "  AND Mov_Destinazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "'   "
            End If
            If Tipo_Destinazione <> 0 Then
                strSql &= "  AND Mov_Destinazioni.Tipo_Destinazione = " & Agro_SQL_SaveNum(Tipo_Destinazione) & "   "
            End If
            If Id_Destinazione <> 0 Then
                strSql &= "  AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "   "
            End If
            strSql &= " )"


            'JOIN RISORSE UMANI
            strSql &= " LEFT JOIN risorse_umane ON movimenti.PIVA = risorse_umane.PIVA AND movimenti.Cod_RisUm = risorse_umane.Cod_RisUm "

            'JOIN CONTATTI
            strSql &= " LEFT JOIN contatti ON contatti.PIVA = risorse_umane.PIVA AND contatti.Cod_Contatto = risorse_umane.Cod_Contatto "

            'JOIN CATEGORIE MAGAZZINO
            strSql &= " INNER JOIN CategorieMagazzino ON Movimenti_dettagli.Elem_Cod = CategorieMagazzino.Elem_Cod "

            strSql &= " LEFT OUTER JOIN UnitaMisura ON Movimenti_dettagli.Udm_Cod = UnitaMisura.Udm_Cod "

            'JOIN MATERIE PRIME
            strSql &= " INNER JOIN Materie_Prime On Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod AND  Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod"

            'JOIN MATERIE_PRIME_CAMPIONATURE
            For Each paramQual In DTParamQual.Rows
                strSql &= " LEFT JOIN Materie_Prime_Campionature AS Materie_Prime_Campionature_" & paramQual("Tabella_Key") &
                                            " ON Movimenti_dettagli.Cal_Cod = Materie_Prime_Campionature_" & paramQual("Tabella_Key") & ".Progressivo " &
                                             " AND Materie_Prime_Campionature_" & paramQual("Tabella_Key") & ".Tipo = 'o" & paramQual("Tabella_Key") & "'"
                If Not ({"cliente", "fornitore"}).Contains(paramQual("Tabella_Key")) Then
                    strSql &= " LEFT JOIN OTabelle_Parametri AS OTabelle_Parametri_" & paramQual("Tabella_Key") &
                                      " ON Materie_Prime_Campionature_" & paramQual("Tabella_Key") & ".Tipo_Cod =  OTabelle_Parametri_" & paramQual("Tabella_Key") & ".Tabella_Par_Cod " &
                                      " AND OTabelle_Parametri_" & paramQual("Tabella_Key") & ".Tabella_Cod = '" & paramQual("Tabella_ID") & "'"
                End If
            Next

            'JOIN LINEE PREPARAZIONE
            strSql &= " LEFT JOIN Linee_Preparazioni lp on Agenda.PREPARAZIONE_COD = lp.Preparazione_Cod "

            'JOIN Movimenti causale aggiuntiva
            If Cau_Agg <> 0 Then
                strSql &= " INNER JOIN Movimenti MovimentiCausaleAgg On Agenda.PIVA = MovimentiCausaleAgg.PIVA AND Agenda.Id_Agenda = MovimentiCausaleAgg.Id_Agenda AND MovimentiCausaleAgg.Cau_Mov = '" & Cau_Agg & "' "
            End If

            'JOIN aggiuntiva
            strSql &= xJoinAggiuntiva

            strSql &= " WHERE 1=1 "

            If Piva <> "" Then
                strSql &= " AND Movimenti_Dettagli.Piva = '" & Agro_SQL_SaveText(Piva) & "'   "
            End If

            If Sa_Cod <> 0 Then
                strSql &= " AND Movimenti_Dettagli.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   "
            End If

            If Id_Agenda <> 0 Then
                strSql &= " AND Movimenti_Dettagli.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   "
            End If

            If Id_Mov <> 0 Then
                strSql &= " AND Movimenti_Dettagli.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   "
            End If

            If Id_Mov_Det <> 0 Then
                strSql &= " AND Movimenti_Dettagli.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   "
            End If

            strSql &= " AND movimenti_dettagli.Jolly_Int = '" & Agro_SQL_SaveText(Flag_Jolly_Int.ToString) & "' "

            If Lav_Cod IsNot Nothing AndAlso Lav_Cod.Length > 0 Then
                strSql &= " AND Agenda.Lav_Cod IN ( "
                For i As Integer = 0 To Lav_Cod.Count - 1
                    If i > 0 Then
                        strSql &= ", "
                    End If
                    strSql &= Agro_SQL_SaveNum(Lav_Cod(i))
                Next
                strSql &= ") "
            End If

            If Cau_Mov IsNot Nothing AndAlso Cau_Mov.Length > 0 Then
                strSql &= " AND Movimenti.Cau_Mov IN ('"
                For i As Integer = 0 To Cau_Mov.Count - 1
                    If i > 0 Then
                        strSql &= "', '"
                    End If
                    strSql &= Agro_SQL_SaveText(Cau_Mov(i))
                Next
                strSql &= "') "
            End If

            If Elem_Cod <> 0 Then
                strSql &= " AND Movimenti_Dettagli.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   "
            End If

            If Pro_Cod <> 0 Then
                strSql &= " AND Movimenti_Dettagli.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   "
            End If

            If Mat_Cod <> 0 Then
                strSql &= " AND Movimenti_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   "
            End If

            If Cod_Progetto <> 0 Then
                strSql &= " AND Movimenti_Dettagli.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & "   "
            End If

            If Fase_Cod <> 0 Then
                strSql &= " AND Movimenti_Dettagli.Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   "
            End If

            If Lotto <> LOTTO_NONDEFINITO Then
                strSql &= " AND Movimenti_Dettagli.Lotto = '" & Agro_SQL_SaveText(Lotto) & "'   "
            End If

            If Cal_Cod <> 0 Then
                strSql &= " AND Movimenti_Dettagli.Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "   "
            End If

            If Udm_Cod <> 0 Then
                strSql &= " AND Movimenti_Dettagli.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   "
            End If

            If Not String.IsNullOrEmpty(Cod_Contatto) Then
                strSql &= " AND Contatti.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "'   "
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql &= Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo)
            End If

            If xOrderBy <> "" Then
                strSql &= Agro_SQL_Save_xOrderBy(xOrderBy)
            Else
                strSql &= " ORDER BY Agenda.PIVA, Movimenti.Data_Movimento DESC "
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


    Public Function Leggi_Cal_Cod(ByVal Piva As String,
                                  ByVal Sa_Cod As Integer,
                                  ByVal Id_Agenda As Integer,
                                  ByVal Id_Mov As Integer,
                                  ByVal Id_Mov_Det As Integer,
                                  ByVal xFiltroAggiuntivo As String,
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                  ) As Integer

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Dettagli.Leggi_Cal_Cod()"
        Dim messaggioErrore As String = ""

        Dim dt As DataTable
        Dim calCod As Integer = 0

        Try

            dt = Leggi(Piva,
                       Sa_Cod,
                       Id_Agenda,
                       Id_Mov,
                       Id_Mov_Det,
                       0, 0, 0, "", 0, 0, 0, 0, 0, 0,
                       enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                       xFiltroAggiuntivo,
                       "",
                       objParametri)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                calCod = dt.Rows(0).Item("Cal_Cod")
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return calCod

    End Function

    Public Function Leggi_UltimaModifica(ByVal Piva As String,
                                         ByVal Sa_Cod As Integer,
                                         ByVal Id_Agenda As Integer,
                                         ByVal Id_Mov As Integer,
                                         ByVal Id_Mov_Det As Integer,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByRef Username_Modifica As String,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                         ) As DateTime

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Dettagli.Leggi_UltimaModifica()"
        Dim messaggioErrore As String = ""

        Dim dt As DataTable
        Dim dataModifica As DateTime = AGRODATAINIZIO

        Try

            dt = Leggi(Piva,
                       Sa_Cod,
                       Id_Agenda,
                       Id_Mov,
                       Id_Mov_Det,
                       0, 0, 0, "", 0, 0, 0, 0, 0, 0,
                       enumSelezioneVariabile.Selezione_TabellaCompleta,
                       xFiltroAggiuntivo,
                       "",
                       objParametri)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                dataModifica = dt.Rows(0).Item("Data_Modifica")
                Username_Modifica = dt.Rows(0).Item("Username_Modifica")
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dataModifica

    End Function

    Public Function ProdottoPresenteDettagli(ByVal piva As String,
                                             ByVal elemCod As Integer,
                                             ByVal proCod As Integer,
                                             ByVal matCod As Integer,
                                             ByVal lotto As String,
                                             ByVal calCod As Integer,
                                             ByVal cauMov As String,
                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                             Optional ByVal matCodAlias As Integer? = 0
                                             ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Dettagli.ProdottoPresenteDettagli()"
        Dim messaggioErrore As String = ""
        Dim presente As Boolean = False
        Try

            Dim mov_det As List(Of AgronicaCoreEntityFramework_POCO.Movimenti_dettagli) = Leggi_Dettagli_Prodotto(piva, elemCod, proCod,
                                                                                                                      matCod, lotto, calCod,
                                                                                                                      cauMov, objParametri, matCodAlias)
            If Not IsNothing(mov_det) AndAlso mov_det.Count > 0 Then
                presente = True
            End If


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return presente

    End Function

    Public Function Leggi_Dettagli_Prodotto(ByVal piva As String,
                                             ByVal elemCod As Integer,
                                             ByVal proCod As Integer,
                                             ByVal matCod As Integer,
                                             ByVal lotto As String,
                                             ByVal calCod As Integer,
                                             ByVal cauMov As String,
                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                             Optional ByVal matCodAlias As Integer? = 0
                                             ) As List(Of AgronicaCoreEntityFramework_POCO.Movimenti_dettagli)

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Dettagli.Leggi_Dettagli_Prodotto()"
        Dim messaggioErrore As String = ""
        Dim mov_det As List(Of AgronicaCoreEntityFramework_POCO.Movimenti_dettagli)

        Try
            Dim gefutils As New Gias_EF_Utility

            Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
            Using giasContext As New Gias_DeveloperServer_Entities(efConnString)

                Dim comandoSqlSetTransaction = UtilityDatabase.SetTransactionIsolationLevel(IsolationLevel.ReadUncommitted)
                giasContext.Database.ExecuteSqlCommand(comandoSqlSetTransaction)

                mov_det = (From md In giasContext.Movimenti_dettagli
                           Join m In giasContext.Movimenti
                           On md.PIVA Equals m.PIVA And md.Id_Agenda Equals m.Id_Agenda And md.Id_Mov Equals m.Id_Mov
                           Where (String.IsNullOrEmpty(piva) OrElse md.PIVA.Equals(piva)) And
                        (elemCod = 0 OrElse md.Elem_Cod = elemCod) AndAlso
                        (proCod = 0 OrElse md.Pro_Cod = proCod) AndAlso
                        (matCod = 0 OrElse md.Mat_Cod = matCod) AndAlso
                        (lotto = "" OrElse md.Lotto.Equals(lotto)) AndAlso
                        (calCod = 0 OrElse md.Cal_Cod = calCod) AndAlso
                        (cauMov = "" OrElse m.Cau_Mov.Equals(cauMov)) AndAlso
                        (matCodAlias = 0 OrElse md.Mat_Cod_Alias = matCodAlias)
                           Select md).ToList()


            End Using

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return mov_det

    End Function


    Public Function LeggiLottoStripMovimentato(ByVal piva As String,
                                               ByVal elemCod As Integer,
                                               ByVal matCod As Integer,
                                               ByVal lotto As String,
                                               ByVal xOrderBy As String,
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                               ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Dettagli.LeggiLottoStripMovimentato()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try
            strSql.Length = 0

            strSql.AppendLine("SELECT DISTINCT Piva, Elem_Cod, Mat_Cod, Lotto, SUBSTRING(Lotto, 3, LEN(Lotto)-2) AS Lotto_Strip ")
            strSql.AppendLine("FROM Movimenti_dettagli ")

            strSql.AppendLine("WHERE Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            'mi serve perchè sennò si impianta la query se il lotto è vuoto
            strSql.AppendLine("AND LEN(Lotto) >= 2 ")

            If elemCod <> 0 Then
                strSql.AppendLine("AND Elem_Cod = " & Agro_SQL_SaveNum(elemCod) & " ")
            End If

            If matCod <> 0 Then
                strSql.AppendLine("AND Mat_Cod = " & Agro_SQL_SaveNum(matCod) & " ")
            End If

            If lotto <> "" Then
                strSql.AppendLine("AND SUBSTRING(Lotto, 3, LEN(Lotto)-2) = '" & Agro_SQL_SaveText(lotto) & "' ")
            End If

            If xOrderBy <> "" Then
                strSql.AppendLine(Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Lotto DESC ")
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

    Public Function ProdottoMovimentatoInAltreOp(ByVal piva As String,
                                                 ByVal idAgenda As Integer,
                                                 ByVal idMovDet As Integer,
                                                 ByVal cauMov As String,
                                                 ByRef objParametri As AgronicaCoreParametri,
                                                 Optional ByVal dataVerifica As Date? = Nothing,
                                                 Optional ByVal escludiDdtResoConferimento As Boolean = False,
                                                 Optional ByVal listaElemCod As List(Of Integer) = Nothing,
                                                 Optional ByVal movimentiPrecedenti As Boolean = False,
                                                 Optional ByVal soloMagazzinoMovimentato As Boolean = False
                                                 ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Movimenti_Dettagli.ProdottoMovimentatoInAltreOp()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.AppendLine(" SELECT a.Piva, a.Id_Agenda, a.Lav_Cod, a.Des_Lib, a.Validita_Inizio ")
            strSql.AppendLine("        , m.Cau_Mov, m.Data_Movimento ")
            strSql.AppendLine("        , md.Elem_Cod, md.Pro_Cod, md.Mat_Cod, md.Mov_Det_Des, md.Udm_Cod, md.Fase_Cod, md.Cal_Cod, md.Lotto ")
            strSql.AppendLine("        , dest.Tipo_Destinazione, dest.Sa_Cod, dest.Id_Destinazione ")
            strSql.AppendLine(" FROM Movimenti_dettagli md ")
            strSql.AppendLine(" INNER JOIN Agenda a ON md.PIVA = a.PIVA AND md.Id_Agenda = a.Id_Agenda ")
            strSql.AppendLine(" INNER JOIN Movimenti m ON md.PIVA = m.PIVA AND md.Id_Agenda = m.Id_Agenda AND md.Id_Mov = m.Id_Mov ")
            strSql.AppendLine(" INNER JOIN Mov_Destinazioni dest ON md.PIVA = dest.Piva AND md.Id_Agenda = dest.Id_Agenda ")
            strSql.AppendLine("                                 AND md.Id_Mov = dest.Id_Mov AND md.Id_Mov_Det = dest.Id_Mov_Det ")

            '-- Inizio Join Alias Doc

            strSql.AppendLine(" INNER JOIN ( ")

            strSql.AppendLine("     SELECT DISTINCT md2.Validita_Inizio, Elem_Cod, Pro_Cod, Mat_Cod, Mov_Det_Des, Udm_Cod, Fase_Cod, Cal_Cod, Lotto ")
            strSql.AppendLine("                     , dest2.Tipo_Destinazione, dest2.Sa_Cod, dest2.Id_Destinazione ")
            strSql.AppendLine("     FROM Movimenti_dettagli md2 ")
            strSql.AppendLine("     INNER JOIN Mov_Destinazioni dest2 ON md2.PIVA = dest2.Piva AND md2.Id_Agenda = dest2.Id_Agenda ")
            strSql.AppendLine("                                      AND md2.Id_Mov = dest2.Id_Mov AND md2.Id_Mov_Det = dest2.Id_Mov_Det ")
            strSql.AppendLine("     WHERE md2.Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            strSql.AppendLine("     AND md2.Id_Agenda = " & Agro_SQL_SaveNum(idAgenda))
            If soloMagazzinoMovimentato Then
                strSql.AppendLine("     AND md2.Jolly_Int = " & Agro_SQL_SaveNum(MagazzinoMovimentato))
            End If
            If idMovDet <> 0 Then
                strSql.AppendLine("     AND Id_Mov_Det = " & Agro_SQL_SaveNum(idMovDet))
            End If

            strSql.AppendLine(" ) AS Doc ")

            strSql.AppendLine("      ON md.Elem_Cod = Doc.Elem_Cod AND md.Pro_Cod = Doc.Pro_Cod AND md.Mat_Cod = Doc.Mat_Cod ")
            strSql.AppendLine("      AND md.Udm_Cod = Doc.Udm_Cod AND md.Fase_Cod = Doc.Fase_Cod AND md.Cal_Cod = Doc.Cal_Cod  ")
            strSql.AppendLine("      AND md.Lotto = Doc.Lotto AND dest.Tipo_Destinazione = Doc.Tipo_Destinazione ")
            strSql.AppendLine("      AND dest.Sa_Cod = Doc.Sa_Cod AND dest.Id_Destinazione = Doc.Id_Destinazione ")

            '-- Fine Join Alias  Doc

            strSql.AppendLine(" WHERE md.Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            strSql.AppendLine(" AND md.Id_Agenda <> " & Agro_SQL_SaveNum(idAgenda))

            If cauMov <> "" Then
                strSql.AppendLine(" AND m.Cau_Mov = '" & Agro_SQL_SaveText(cauMov) & "' ")
            End If

            If soloMagazzinoMovimentato Then
                strSql.AppendLine(" AND md.Jolly_Int = " & Agro_SQL_SaveNum(MagazzinoMovimentato))
            End If

            If Not IsNothing(dataVerifica) Then
                Dim operatoreConfronto As String
                If movimentiPrecedenti Then
                    operatoreConfronto = " <= "
                Else
                    operatoreConfronto = " >= "
                End If
                strSql.AppendLine(" AND m.Validita_Inizio " & operatoreConfronto & Agro_SQL_SaveDate(dataVerifica) & " ")
            End If

            If escludiDdtResoConferimento Then
                strSql.AppendLine(" AND a.Id_Agenda NOT IN ( ")
                strSql.AppendLine("     SELECT Id_Agenda FROM Mov_Dettagli_Riferimenti ")
                strSql.AppendLine("     WHERE Piva = '" & Agro_SQL_SaveText(piva) & "' ")
                strSql.AppendLine("     AND Id_Agenda_Rif = " & Agro_SQL_SaveNum(idAgenda))
                strSql.AppendLine("     AND Lav_Cod_Rif IN ( " & LAVCOD_ACCETTAZIONE_DIVERSI & "," &
                                                                  LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE & "," &
                                                                  LAVCOD_DISTINTA_CARICO_ACCETTAZIONE & "," &
                                                                  LAVCOD_DISTINTA_CARICO & "," &
                                                                  LAVCOD_AUTO_DDT_EMESSO & ") ")
                strSql.AppendLine("     AND Lav_Cod = " & LAVCOD_BOLLA_EMESSA)
                strSql.AppendLine(" ) ")
            End If

            If Not IsNothing(listaElemCod) AndAlso listaElemCod.Count > 0 Then
                strSql.Append(" AND md.Elem_Cod in (")
                For i = 0 To listaElemCod.Count - 1
                    If i > 0 Then
                        strSql.Append(",")
                    End If
                    strSql.Append(Agro_SQL_SaveNum(listaElemCod(i).ToString))
                Next
                strSql.AppendLine(")")
            End If

            strSql.AppendLine(" ORDER BY a.Validita_Inizio, a.Id_Agenda ")

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

    Public Function RaccolteCollegate(ByVal piva As String,
                                      ByVal idAgenda As Integer,
                                      ByVal dataVerifica As Date,
                                      ByRef objParametri As AgronicaCoreParametri
                                      ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Movimenti_Dettagli.RaccolteCollegate()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.AppendLine(" SELECT dest.Piva, dest.Sa_Cod, dest.Id_Agenda, dest.Id_Mov, dest.Id_Mov_Det, dest.Appezza, dest.Id_Destinazione, dest.Tipo_Destinazione ")
            strSql.AppendLine("        , ipp.Progetto_Cod, ipp.Validita_Inizio, ipp.Validita_Fine ")
            strSql.AppendLine(" FROM Mov_Destinazioni dest ")
            strSql.AppendLine(" INNER JOIN Movimenti m ON dest.Piva = m.PIVA AND dest.Id_Agenda = m.Id_Agenda AND dest.Id_Mov = m.Id_Mov ")
            strSql.AppendLine(" INNER JOIN Mov_Dettagli_Riferimenti rif ON m.Piva = rif.Piva_Rif AND m.Id_Agenda = rif.Id_Agenda_Rif ")
            strSql.AppendLine("                                        AND m.Cau_Mov = rif.Cau_Mov_Rif AND rif.Lav_Cod_Rif = " & LAVCOD_RACCOLTA)
            strSql.AppendLine(" LEFT JOIN Imprese_Progetti ipp ON dest.Piva = ipp.Piva AND dest.Sa_Cod = ipp.Sa_Cod ")
            strSql.AppendLine("                               AND dest.Appezza = ipp.Appezza AND dest.Id_Destinazione = ipp.Id_Reg ")
            strSql.AppendLine("                               AND ipp.Validita_Inizio <= " & Agro_SQL_SaveDate(dataVerifica))
            strSql.AppendLine("                               AND ipp.Validita_Fine    > " & Agro_SQL_SaveDate(dataVerifica))

            strSql.AppendLine(" WHERE rif.Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            strSql.AppendLine(" AND rif.Id_Agenda = " & Agro_SQL_SaveNum(idAgenda))

            strSql.AppendLine(" ORDER BY dest.Piva, dest.Id_Agenda")

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

    ''' <summary>
    ''' Restituisce l'elenco delle operazioni di raccolta effettuate da una Azienda di Campagna che non sono ancora state collegate ad un conferimento
    ''' </summary>
    ''' <param name="piva">Impresa di Campagna a cui è intestata la raccolta</param>
    ''' <param name="inizioValImpianto">Visualizza le raccolte appartenenti ad impianti attivi a partire da questa data</param>
    ''' <param name="fineValImpianto">Visualizza le raccolte appartenenti ad impianti attivi fini a questa data</param>
    ''' <param name="idMovDetConf">Se specificato, estrae anche le raccolte collegate alla specifica riga di conferimento</param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    Public Function RaccolteCollegabiliConferimento(ByVal piva As String, ByVal inizioValImpianto As Date, ByVal fineValImpianto As Date, ByVal idMovDetConf As Integer, ByRef objParametri As AgronicaCoreParametri) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Movimenti_Dettagli.RaccolteCollegabiliConferimento()"

        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            Dim arrLavCodAccettazioniConf As Integer() = New Integer() {LAVCOD_ACCETTAZIONE_DIVERSI, LAVCOD_DISTINTA_CARICO_ACCETTAZIONE, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE} 'LAVCOD_DISTINTA_CARICO, LAVCOD_AUTO_DDT_EMESSO Non usate per i conferimenti

            'Versione della query con CTE sugli impianti.
            'La difficoltà nel reperire questi dati è costituita dal fatto che se ho multipli dettagli di prodotto, più impianti dai quali ho raccolto e multipli magazzini di destinazione,
            'i prodotti non sono collegati direttamente ad un unico impianto invece
            'sono collegati ognuno ad un singolo magazzino;
            'Dovendo quindi in ogni riga del datatable mostrare il prodotto, il suo magazzino di destinazione ed i multipli impianti di provenienza, per non avere una moltiplicazione
            'cartesiana fra i record, devo recuperare i dati degli impianti separatamente rispetto alla query principale e per farlo utilizzo la CTE_Campagna
            'nella quale ogni record equivale ad una agenda di raccolta e le cui colonne sono piva, id_agenda ed una colonna di raggruppamento con tutti i dati descrittivi utili degli impianti.
            'Sono necessarie queste tre colonne perché le prime due servono a collegare la CTE alla query principale, mentre la terza contiene i dati richiesti;
            'Dovendo supportare fino a sql2008 posso raggruppare i dati testo solo attraverso l'istruzione FOR XML PATH, la quale però per essere usata con questo scopo necessita
            'che venga specificata in select della query dove la si utilizza un'unica colonna.
            'Con queste premesse, allora il calcolo di questa colonna è specificato in una subquery nella CTE_Campagna.
            'EDIT: Siccome ho necessità di avere altri dati dagli impianti su colonne singole e non composti nella colonna descrittiva, scrivo prima
            'la CTE_Dati_Campagna per prelevare tutti i dati necessari poi la uso come base per la CTE_Campagna e le altre CTE
            strSql.AppendLine("WITH CTE_Dati_Campagna(CTE_Piva, CTE_Lav_Cod, CTE_Id_Agenda, CTE_Data_Movimento, CTE_Cau_Mov, CTE_Id_Mov, CTE_Id_Mov_Det, CTE_Elem_Cod, CTE_Mat_Cod, CTE_Lotto, CTE_Cod_Progetto, CTE_Udm_Cod, CTE_Qta, CTE_Qta_Dest, CTE_Sup_Impiegata, CTE_QuotaDistribuzione, CTE_Appezza, CTE_Id_Destinazione, CTE_Sa_Cod, CTE_Sa_Nome, CTE_Cul_Cod, CTE_Cul_Des, CTE_Veg_Cod, CTE_Veg_Des, CTE_Appez_Nome, CTE_Appez_Codici_Rif, CTE_Imp_Codici_Cod, CTE_Progetto_Cod, CTE_Progetto_Nome, CTE_Regolamento_Cod, CTE_Cod_Articolo, CTE_Mat_Des, CTE_Udm_Sim) AS (")
            strSql.AppendLine(" SELECT DISTINCT Agenda.Piva")
            strSql.AppendLine(" ,Agenda.Lav_Cod")
            strSql.AppendLine(" ,Agenda.Id_Agenda")
            strSql.AppendLine(" ,Movimenti.Data_Movimento")
            strSql.AppendLine(" ,Movimenti.Cau_Mov")
            strSql.AppendLine(" ,Movimenti.Id_Mov")
            strSql.AppendLine(" ,Movimenti_dettagli.Id_Mov_Det")
            strSql.AppendLine(" ,Movimenti_dettagli.Elem_Cod")
            strSql.AppendLine(" ,Movimenti_dettagli.Mat_Cod")
            strSql.AppendLine(" ,Movimenti_dettagli.Lotto")
            strSql.AppendLine(" ,Movimenti_dettagli.Cod_Progetto")
            strSql.AppendLine(" ,Movimenti_dettagli.Udm_Cod")
            strSql.AppendLine(" ,Movimenti_dettagli.Qta")
            strSql.AppendLine(" ,Mov_Destinazioni.Qta AS Qta_Dest")
            strSql.AppendLine(" ,Mov_Destinazioni.Qta2 AS Sup_Impiegata")
            strSql.AppendLine(" ,Mov_Destinazioni.QuotaDistribuzione AS QuotaDistribuzione")
            strSql.AppendLine(" ,Mov_Destinazioni.Appezza")
            strSql.AppendLine(" ,Mov_Destinazioni.Id_Destinazione")
            strSql.AppendLine(" ,Centri_Aziendali.sa_cod")
            strSql.AppendLine(" ,Centri_Aziendali.sa_nome")
            strSql.AppendLine(" ,Cultivar.Cul_Cod")
            strSql.AppendLine(" ,Cultivar.Cul_Des")
            strSql.AppendLine(" ,SpecieVegetali.Veg_Cod")
            strSql.AppendLine(" ,SpecieVegetali.Veg_Des")
            strSql.AppendLine(" ,Appezzamento.APP_NOME")
            strSql.AppendLine(" ,Appezzamento_Codici.Val_Cod")
            strSql.AppendLine(" ,Reg_Impianti_Codici.Val_Cod")
            strSql.AppendLine(" ,Imprese_Progetti.Progetto_Cod")
            strSql.AppendLine(" ,Imprese_Progetti.Progetto_Nome")
            strSql.AppendLine(" ,Imprese_Progetti.Regolamento_Cod")
            strSql.AppendLine(" ,Materie_Prime.Cod_Articolo")
            strSql.AppendLine(" ,Materie_Prime.Mat_Des")
            strSql.AppendLine(" ,UnitaMisura.UDM_SIM")
            strSql.AppendLine(" FROM Agenda ")
            strSql.AppendLine("")
            strSql.AppendLine(" INNER JOIN Movimenti ")
            strSql.AppendLine(" ON Agenda.Piva = Movimenti.Piva")
            strSql.AppendLine(" AND Agenda.Id_Agenda = Movimenti.Id_Agenda")
            strSql.AppendLine("")
            strSql.AppendLine(" INNER JOIN Movimenti_dettagli ")
            strSql.AppendLine(" ON Movimenti.Piva = Movimenti_dettagli.Piva")
            strSql.AppendLine(" AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda")
            strSql.AppendLine(" AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov")
            strSql.AppendLine("")
            strSql.AppendLine(" INNER JOIN Mov_Destinazioni ")
            strSql.AppendLine(" ON Movimenti_dettagli.Piva = Mov_Destinazioni.Piva  ")
            strSql.AppendLine(" AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda  ")
            strSql.AppendLine(" AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov  ")
            strSql.AppendLine(" AND Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det  ")
            strSql.AppendLine("")
            strSql.AppendLine(" INNER JOIN Centri_Aziendali ")
            strSql.AppendLine(" ON Centri_Aziendali.PIVA = Agenda.PIVA")
            strSql.AppendLine(" AND Centri_Aziendali.sa_cod = Mov_Destinazioni.Sa_Cod")
            strSql.AppendLine("")
            strSql.AppendLine(" INNER JOIN Reg_Impianti ")
            strSql.AppendLine(" ON Reg_Impianti.PIVA = Mov_Destinazioni.Piva  ")
            strSql.AppendLine(" AND Reg_Impianti.SA_COD = Mov_Destinazioni.Sa_Cod  ")
            strSql.AppendLine(" AND Reg_Impianti.APPEZZA = Mov_Destinazioni.Appezza  ")
            strSql.AppendLine(" AND Reg_Impianti.ID_REG = Mov_Destinazioni.Id_Destinazione ")
            strSql.AppendLine("")
            strSql.AppendLine(" LEFT JOIN Reg_Impianti_Codici ")
            strSql.AppendLine(" ON Reg_Impianti_Codici.PIVA = Reg_Impianti.PIVA  ")
            strSql.AppendLine(" AND Reg_Impianti_Codici.SA_COD = Reg_Impianti.SA_COD  ")
            strSql.AppendLine(" AND Reg_Impianti_Codici.APPEZZA = Reg_Impianti.APPEZZA  ")
            strSql.AppendLine(" AND Reg_Impianti_Codici.ID_REG = Reg_Impianti.ID_REG ")
            strSql.AppendLine(" AND Reg_Impianti_Codici.Id_Cod = " & enum_CodiciAnagrafe.Codice_Impianto)
            strSql.AppendLine("")
            strSql.AppendLine(" INNER JOIN Cultivar ")
            strSql.AppendLine(" ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod ")
            strSql.AppendLine("")
            strSql.AppendLine(" INNER JOIN SpecieVegetali ")
            strSql.AppendLine(" ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ")
            strSql.AppendLine("")
            strSql.AppendLine(" INNER JOIN Appezzamento ")
            strSql.AppendLine(" ON Appezzamento.PIVA = Reg_Impianti.PIVA  ")
            strSql.AppendLine(" AND Appezzamento.SA_COD = Reg_Impianti.SA_COD  ")
            strSql.AppendLine(" AND Appezzamento.APPEZZA = Reg_Impianti.APPEZZA  ")
            strSql.AppendLine("")
            strSql.AppendLine(" LEFT JOIN Appezzamento_Codici ")
            strSql.AppendLine(" ON Appezzamento_Codici.PIVA = Appezzamento.PIVA  ")
            strSql.AppendLine(" AND Appezzamento_Codici.SA_COD = Appezzamento.SA_COD  ")
            strSql.AppendLine(" AND Appezzamento_Codici.APPEZZA = Appezzamento.APPEZZA  ")
            strSql.AppendLine(" AND Appezzamento_Codici.Id_Cod = " & TipiEnumerativi.enum_CodiciAnagrafe.Riferimento_Alfanumerico_Appezzamento)
            strSql.AppendLine("")
            strSql.AppendLine(" INNER JOIN Imprese_Progetti ")
            strSql.AppendLine(" ON Imprese_Progetti.Piva = Reg_Impianti.PIVA ")
            strSql.AppendLine(" AND Imprese_Progetti.Sa_Cod = Reg_Impianti.SA_COD ")
            strSql.AppendLine(" AND Imprese_Progetti.Appezza = Reg_Impianti.APPEZZA ")
            strSql.AppendLine(" AND Imprese_Progetti.Id_Reg = Reg_Impianti.ID_REG ")
            strSql.AppendLine("")
            strSql.AppendLine(" LEFT JOIN Regolamenti ")
            strSql.AppendLine(" ON Regolamenti.Reg_Cod = Imprese_Progetti.Regolamento_Cod")
            strSql.AppendLine("")
            strSql.AppendLine(" LEFT JOIN Materie_Prime ")
            strSql.AppendLine(" ON Materie_Prime.Mat_Cod = Movimenti_dettagli.Mat_Cod")
            strSql.AppendLine("")
            strSql.AppendLine(" LEFT JOIN UnitaMisura ")
            strSql.AppendLine(" ON UnitaMisura.Udm_Cod = Movimenti_dettagli.Udm_Cod")
            strSql.AppendLine("")
            strSql.AppendLine(" WHERE Agenda.Lav_Cod = " & LAVCOD_RACCOLTA & " AND Movimenti.Cau_Mov = '" & CAU_RILIEVO_RACCOLTA & "'")
            strSql.AppendLine(" AND Agenda.Piva = '" & Agro_SQL_SaveText(piva) & "'")
            strSql.AppendLine(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(fineValImpianto))
            strSql.AppendLine(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(inizioValImpianto))
            strSql.AppendLine(")")
            strSql.AppendLine(",CTE_Campagna(CTE_Piva, CTE_Id_Agenda, CTE_Data_Movimento, CTE_Cau_Mov, CTE_Id_Mov, CTE_Id_Mov_Det, CTE_Elem_Cod, CTE_Mat_Cod, CTE_Lotto, CTE_Cod_Progetto, CTE_Udm_Cod, CTE_Qta, CTE_Cod_Articolo, CTE_Mat_Des, CTE_Udm_Sim, CTE_Sa_Cod, CTE_Sa_Nome, CTE_Campagna_Des) AS (")
            strSql.AppendLine(" SELECT DISTINCT CTE_Dati_Campagna.CTE_Piva")
            strSql.AppendLine(" ,CTE_Dati_Campagna.CTE_Id_Agenda")
            strSql.AppendLine(" ,CTE_Dati_Campagna.CTE_Data_Movimento")
            strSql.AppendLine(" ,CTE_Dati_Campagna.CTE_Cau_Mov")
            strSql.AppendLine(" ,CTE_Dati_Campagna.CTE_Id_Mov")
            strSql.AppendLine(" ,CTE_Dati_Campagna.CTE_Id_Mov_Det")
            strSql.AppendLine(" ,CTE_Dati_Campagna.CTE_Elem_Cod")
            strSql.AppendLine(" ,CTE_Dati_Campagna.CTE_Mat_Cod")
            strSql.AppendLine(" ,CTE_Dati_Campagna.CTE_Lotto")
            strSql.AppendLine(" ,CTE_Dati_Campagna.CTE_Cod_Progetto")
            strSql.AppendLine(" ,CTE_Dati_Campagna.CTE_Udm_Cod")
            strSql.AppendLine(" ,CTE_Dati_Campagna.CTE_Qta")
            strSql.AppendLine(" ,CTE_Dati_Campagna.CTE_Cod_Articolo")
            strSql.AppendLine(" ,CTE_Dati_Campagna.CTE_Mat_Des")
            strSql.AppendLine(" ,CTE_Dati_Campagna.CTE_Udm_Sim")
            strSql.AppendLine(" ,CTE_Dati_Campagna.CTE_Sa_Cod")
            strSql.AppendLine(" ,CTE_Dati_Campagna.CTE_Sa_Nome")
            strSql.AppendLine(" ,REPLACE(REPLACE(ISNULL(STUFF(")
            strSql.AppendLine("  (SELECT DISTINCT '<br/>' ")
            strSql.AppendLine("   + '" & Agro_SQL_SaveText("<strong>" & Gias.Centro & "</strong> ") & "' + CTE_Sa_Nome + '; '")
            strSql.AppendLine("   + '" & Agro_SQL_SaveText("<strong>" & Gias.Specie & "</strong> ") & "' + CTE_Veg_Des + ' ' + CTE_Cul_des + '; '")
            strSql.AppendLine("   + CASE ISNULL(CTE_Appez_Nome, '') WHEN ''")
            strSql.AppendLine("     THEN '' ")
            strSql.AppendLine("     ELSE '" & Agro_SQL_SaveText("<strong>" & Gias.AppezzamentoAbbr & "</strong> ") & "' + CTE_Appez_Nome + ' ' ")
            strSql.AppendLine("       + CASE ISNULL(CTE_Appez_Codici_Rif, '') WHEN '' ")
            strSql.AppendLine("       THEN '' ")
            strSql.AppendLine("       ELSE ' (' + CTE_Appez_Codici_Rif + ') '")
            strSql.AppendLine("       END")
            strSql.AppendLine("     END")
            strSql.AppendLine("   + CASE ISNULL(CTE_Imp_Codici_Cod, '') WHEN ''")
            strSql.AppendLine("     THEN ''")
            strSql.AppendLine("     ELSE '" & Agro_SQL_SaveText("<strong>" & Gias.ImpiantoAbbr & "</strong> ") & "' + CTE_Imp_Codici_Cod + '; '")
            strSql.AppendLine("     END")
            strSql.AppendLine("   + '" & Agro_SQL_SaveText("<strong>" & Gias.Lotto & "</strong> ") & "'")
            strSql.AppendLine("   + CASE ISNULL(CTE_Progetto_Nome, '') WHEN ''")
            strSql.AppendLine("     THEN CONVERT(varchar, CTE_Progetto_Cod)")
            strSql.AppendLine("     ELSE CTE_Progetto_Nome + ' - ' + CONVERT(varchar, CTE_Progetto_Cod)")
            strSql.AppendLine("     END")
            strSql.AppendLine("   + '; ' + CASE CTE_Regolamento_Cod WHEN 4 THEN '" & Agro_SQL_SaveText(Gias.Biologico) & "' ELSE '" & Agro_SQL_SaveText(Gias.NonBiologico) & "' END")
            strSql.AppendLine("")
            strSql.AppendLine("   FROM CTE_Dati_Campagna CTE_Dati_Campagna_For_XML ")
            strSql.AppendLine("   WHERE CTE_Dati_Campagna_For_XML.CTE_Piva = CTE_Dati_Campagna.CTE_PIVA")
            strSql.AppendLine("   AND CTE_Dati_Campagna_For_XML.CTE_Id_Agenda = CTE_Dati_Campagna.CTE_Id_Agenda")
            strSql.AppendLine("   AND CTE_Dati_Campagna_For_XML.CTE_Id_Mov = CTE_Dati_Campagna.CTE_Id_Mov")
            strSql.AppendLine("   AND CTE_Dati_Campagna_For_XML.CTE_Id_Mov_Det = CTE_Dati_Campagna.CTE_Id_Mov_Det")
            strSql.AppendLine("   FOR XML PATH('')")
            strSql.AppendLine("")
            strSql.AppendLine("  )")
            strSql.AppendLine(" , 1, 11, ''), ''), '&lt;', '<'), '&gt;', '>') AS Col_Campagna_Des")
            strSql.AppendLine(" FROM CTE_Dati_Campagna")
            strSql.AppendLine(")")
            strSql.AppendLine(",CTE_Varieta(CTE_Piva, CTE_Id_Agenda, CTE_Mat_Cod, CTE_Lotto, CTE_Cul_Cod) AS (")
            strSql.AppendLine(" SELECT DISTINCT CTE_Dati_Campagna.CTE_Piva")
            strSql.AppendLine(" ,CTE_Dati_Campagna.CTE_Id_Agenda")
            strSql.AppendLine(" ,CTE_Dati_Campagna.CTE_Mat_Cod")
            strSql.AppendLine(" ,CTE_Dati_Campagna.CTE_Lotto")
            strSql.AppendLine(" ,ISNULL(STUFF(")
            strSql.AppendLine("  (SELECT DISTINCT '|' + CONVERT(varchar, CTE_Cul_Cod)")
            strSql.AppendLine("  FROM CTE_Dati_Campagna CTE_Dati_Campagna_For_XML ")
            strSql.AppendLine("  WHERE CTE_Dati_Campagna_For_XML.CTE_Piva = CTE_Dati_Campagna.CTE_PIVA")
            strSql.AppendLine("  AND CTE_Dati_Campagna_For_XML.CTE_Id_Agenda = CTE_Dati_Campagna.CTE_Id_Agenda")
            strSql.AppendLine("  AND CTE_Dati_Campagna_For_XML.CTE_Id_Mov = CTE_Dati_Campagna.CTE_Id_Mov")
            strSql.AppendLine("  AND CTE_Dati_Campagna_For_XML.CTE_Id_Mov_Det = CTE_Dati_Campagna.CTE_Id_Mov_Det")
            strSql.AppendLine("  FOR XML PATH('')")
            strSql.AppendLine("  )")
            strSql.AppendLine(" , 1, 1, ''), '')")
            strSql.AppendLine(" FROM CTE_Dati_Campagna")
            strSql.AppendLine(")")
            strSql.AppendLine(",CTE_Specie(CTE_Piva, CTE_Id_Agenda, CTE_Mat_Cod, CTE_Lotto, CTE_Veg_Cod) AS (")
            strSql.AppendLine(" SELECT DISTINCT CTE_Dati_Campagna.CTE_Piva")
            strSql.AppendLine(" ,CTE_Dati_Campagna.CTE_Id_Agenda")
            strSql.AppendLine(" ,CTE_Dati_Campagna.CTE_Mat_Cod")
            strSql.AppendLine(" ,CTE_Dati_Campagna.CTE_Lotto")
            strSql.AppendLine(" ,ISNULL(STUFF(")
            strSql.AppendLine("  (SELECT DISTINCT '|' + CONVERT(varchar, CTE_Veg_Cod)")
            strSql.AppendLine("  FROM CTE_Dati_Campagna CTE_Dati_Campagna_For_XML ")
            strSql.AppendLine("  WHERE CTE_Dati_Campagna_For_XML.CTE_Piva = CTE_Dati_Campagna.CTE_PIVA")
            strSql.AppendLine("  AND CTE_Dati_Campagna_For_XML.CTE_Id_Agenda = CTE_Dati_Campagna.CTE_Id_Agenda")
            strSql.AppendLine("  AND CTE_Dati_Campagna_For_XML.CTE_Id_Mov = CTE_Dati_Campagna.CTE_Id_Mov")
            strSql.AppendLine("  AND CTE_Dati_Campagna_For_XML.CTE_Id_Mov_Det = CTE_Dati_Campagna.CTE_Id_Mov_Det")
            strSql.AppendLine("  FOR XML PATH('')")
            strSql.AppendLine("  )")
            strSql.AppendLine(" , 1, 1, ''), '')")
            strSql.AppendLine(" FROM CTE_Dati_Campagna")
            strSql.AppendLine(")")
            strSql.AppendLine(",CTE_Specie_Des(CTE_Piva, CTE_Id_Agenda, CTE_Mat_Cod, CTE_Lotto, CTE_Veg_Des) AS (")
            strSql.AppendLine(" SELECT DISTINCT CTE_Dati_Campagna.CTE_Piva")
            strSql.AppendLine(" ,CTE_Dati_Campagna.CTE_Id_Agenda")
            strSql.AppendLine(" ,CTE_Dati_Campagna.CTE_Mat_Cod")
            strSql.AppendLine(" ,CTE_Dati_Campagna.CTE_Lotto")
            strSql.AppendLine(" ,ISNULL(STUFF(")
            strSql.AppendLine("  (SELECT DISTINCT '|' + CONVERT(varchar, CTE_Veg_Des)")
            strSql.AppendLine("  FROM CTE_Dati_Campagna CTE_Dati_Campagna_For_XML ")
            strSql.AppendLine("  WHERE CTE_Dati_Campagna_For_XML.CTE_Piva = CTE_Dati_Campagna.CTE_PIVA")
            strSql.AppendLine("  AND CTE_Dati_Campagna_For_XML.CTE_Id_Agenda = CTE_Dati_Campagna.CTE_Id_Agenda")
            strSql.AppendLine("  AND CTE_Dati_Campagna_For_XML.CTE_Id_Mov = CTE_Dati_Campagna.CTE_Id_Mov")
            strSql.AppendLine("  AND CTE_Dati_Campagna_For_XML.CTE_Id_Mov_Det = CTE_Dati_Campagna.CTE_Id_Mov_Det")
            strSql.AppendLine("  FOR XML PATH('')")
            strSql.AppendLine("  )")
            strSql.AppendLine(" , 1, 1, ''), '')")
            strSql.AppendLine(" FROM CTE_Dati_Campagna")
            strSql.AppendLine(")")
            strSql.AppendLine("SELECT")
            strSql.AppendLine("CAST(ROW_NUMBER() OVER(ORDER BY Agenda.Id_Agenda Desc) AS nvarchar) AS Chiave")
            strSql.AppendLine(",Agenda.PIVA")
            strSql.AppendLine(",CTE_Campagna.CTE_Data_Movimento AS Data_Movimento")
            strSql.AppendLine(",CTE_Campagna.CTE_Sa_Cod AS Sa_Cod_Campagna")
            strSql.AppendLine(",CTE_Campagna.CTE_Sa_Nome AS Sa_Nome_Campagna")
            strSql.AppendLine(",ISNULL(Centri_Aziendali_Carico.Sa_Cod, 0) AS Sa_Cod_Carico")
            strSql.AppendLine(",ISNULL(Centri_Aziendali_Carico.Sa_Nome, '') AS Sa_Nome_Carico")
            strSql.AppendLine(",CASE Centri_Aziendali_Carico.Sa_Cod WHEN NULL")
            strSql.AppendLine(" THEN ''")
            strSql.AppendLine(" ELSE '" & Agro_SQL_SaveText("<strong>" & Gias.Centro & "</strong> ") & "' + Centri_Aziendali_Carico.Sa_Nome + ")
            strSql.AppendLine("      '" & Agro_SQL_SaveText("; <strong>" & Gias.Magazzino & "</strong> ") & "' + Magazzino_Carico.Fabbricato_Des")
            strSql.AppendLine(" END AS Destinazione_Des")
            strSql.AppendLine(",ISNULL(Magazzino_Carico.Fabbricato_Cod, 0) AS Fabbricato_Cod_Carico")
            strSql.AppendLine(",ISNULL(Magazzino_Carico.Fabbricato_Des, '') AS Fabbricato_Des_Carico")
            strSql.AppendLine(",CTE_Varieta.CTE_Cul_Cod AS Impianto_Cul_Cod")
            strSql.AppendLine(",CTE_Specie.CTE_Veg_Cod AS Impianto_Veg_Cod")
            strSql.AppendLine(",CTE_Specie_Des.CTE_Veg_Des AS Impianto_Veg_Des")
            strSql.AppendLine(",CTE_Campagna.CTE_Campagna_Des AS Campagna_Des")
            strSql.AppendLine(",Agenda.Des_Lib")
            strSql.AppendLine(",Agenda.Id_Agenda")
            strSql.AppendLine(",CTE_Campagna.CTE_Cau_Mov AS Cau_Mov_Campagna")
            strSql.AppendLine(",CTE_Campagna.CTE_Id_Mov AS Id_Mov_Campagna")
            strSql.AppendLine(",CTE_Campagna.CTE_Id_Mov_Det AS Id_Mov_Det_Campagna")
            strSql.AppendLine(",CTE_Campagna.CTE_Elem_Cod AS Elem_Cod_Campagna")
            strSql.AppendLine(",CTE_Campagna.CTE_Mat_Cod AS Mat_Cod_Campagna")
            strSql.AppendLine(",CTE_Campagna.CTE_Lotto AS Lotto_Campagna")
            strSql.AppendLine(",CTE_Campagna.CTE_Udm_Cod AS Udm_Cod_Campagna")
            strSql.AppendLine(",CTE_Campagna.CTE_Qta AS Qta_Campagna")
            strSql.AppendLine("--,CTE_Campagna.CTE_Qta_Dest AS Qta_Dest_Campagna")
            strSql.AppendLine("--,CTE_Campagna.CTE_Sup_Impiegata AS Sup_Impiegata_Campagna")
            strSql.AppendLine("--,CTE_Campagna.CTE_QuotaDistribuzione AS QuotaDistribuzione_Campagna")
            strSql.AppendLine(",CASE ISNULL(Movimenti_Dettagli_Carico.Cod_Progetto, 0) WHEN 0 THEN CTE_Campagna.CTE_Qta ELSE ")
            strSql.AppendLine(" ISNULL(Movimenti_Dettagli_Carico.Qta, 0) END AS Qta_Dettaglio")
            strSql.AppendLine(",ISNULL(Movimenti_Carico.Cau_Mov, '') AS Cau_Mov_Carico")
            strSql.AppendLine(",ISNULL(Movimenti_Carico.Id_Mov, 0) AS Id_Mov_Carico")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Id_Mov_Det, 0) AS Id_Mov_Det_Carico")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Elem_Cod, 0) AS Elem_Cod_Carico")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Pro_Cod, 0) AS Pro_Cod_Carico")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Mat_Cod, 0) AS Mat_Cod_Carico")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Mov_Det_Des, 0) AS Mov_Det_Des_Carico")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Udm_Cod, 0) AS Udm_Cod_Carico")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Qta, 0) AS Qta_Carico")
            strSql.AppendLine(",ISNULL(Mov_Destinazioni_Carico.Qta, 0) AS Qta_Dest_Carico")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Cod_Iva, 0) AS Cod_Iva_Carico")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Sconto, 0) AS Sconto_Carico")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Prezzo_Unitario, 0) AS Prezzo_Unitario_Carico")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Cod_Conto, 0) AS Cod_Conto_Carico")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Cod_Progetto, 0) AS Cod_Progetto_Carico")
            strSql.AppendLine(",ISNULL(Imprese_Progetti_Carico.Progetto_Nome, '') AS Progetto_Nome_Carico")
            strSql.AppendLine(",ISNULL(Imprese_Progetti_Carico.Progetto_Des, '') AS Progetto_Des_Carico")
            strSql.AppendLine(",CASE ISNULL(Movimenti_Dettagli_Carico.Cod_Progetto, 0) WHEN 0")
            strSql.AppendLine(" THEN ''")
            strSql.AppendLine(" ELSE CASE ISNULL(Imprese_Progetti_Carico.Progetto_Nome, '') WHEN ''")
            strSql.AppendLine("      THEN CONVERT(varchar, Movimenti_Dettagli_Carico.Cod_Progetto)")
            strSql.AppendLine("      ELSE Imprese_Progetti_Carico.Progetto_Nome + ' - ' + CONVERT(varchar, Movimenti_Dettagli_Carico.Cod_Progetto)")
            strSql.AppendLine("      END")
            strSql.AppendLine(" END AS Progetto_Lotto_Carico")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Fase_Cod, 0) AS Fase_Cod_Carico")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Contabilizzato, 0) AS Contabilizzato_Carico")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Pendente, 0) AS Pendente_Carico")
            strSql.AppendLine("--,Movimenti_Dettagli_Carico.inviato")
            strSql.AppendLine("--,Movimenti_Dettagli_Carico.datainvio")
            strSql.AppendLine("--,Movimenti_Dettagli_Carico.Data_Creazione")
            strSql.AppendLine("--,Movimenti_Dettagli_Carico.Data_Modifica")
            strSql.AppendLine("--,Movimenti_Dettagli_Carico.Username_Creazione")
            strSql.AppendLine("--,Movimenti_Dettagli_Carico.Username_Modifica")
            strSql.AppendLine("--,Movimenti_Dettagli_Carico.Validita_Inizio")
            strSql.AppendLine("--,Movimenti_Dettagli_Carico.Validita_Fine")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Cal_Cod, 0) AS Cal_Cod")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Extra_Str, '') AS Extra_Str")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Extra_Int, 0) AS Extra_Int")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Extra_Date, 0) AS Extra_Date")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Anno, 0) AS Anno")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Ric_Cod, 0) AS Ric_Cod")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Imponibile, 0) AS Imponibile")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Iva, 0) AS Iva")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Lotto, '') AS Lotto")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Jolly_Int, 0) AS Jolly_Int")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Imponibile_Netto, 0) AS Imponibile_Netto")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Prezzo_Unitario_Netto, 0) AS Prezzo_Unitario_Netto")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.UDM_COD_EXTRA, 0) AS UDM_COD_EXTRA")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.QTA_EXTRA, 0) AS QTA_EXTRA")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Prezzo_Effettivo, 0) AS Prezzo_Effettivo")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.ChkIva_Manuale, 0) AS ChkIva_Manuale")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Cod_IvaIndetraibile, 0) AS Cod_IvaIndetraibile")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Qta_Extra_Totale, 0) AS Qta_Extra_Totale")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Tara, 0) AS Tara")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.ChkLayOut_Hide, 0) AS ChkLayOut_Hide")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Variazione, 0) AS Variazione")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Listino_Cod, 0) AS Listino_Cod")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Sconto_Listino, 0) AS Sconto_Listino")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Sconto_Modalita, 0) AS Sconto_Modalita")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Mat_Cod_Alias, 0) AS Mat_Cod_Alias")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Mezzo_Det, 0) AS Mezzo_Det")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Sconto_Testo, '') AS Sconto_Testo")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Ric_Cod_Pat, 0) AS Ric_Cod_Pat")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Cod_Conto_Pat, 0) AS Cod_Conto_Pat")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.TempoCarenza, 0) AS TempoCarenza")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.DoseEtichetta, 0) AS DoseEtichetta")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Turno_Cod, 0) AS Turno_Cod")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.ID_Attivita, 0) AS ID_Attivita")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Dettaglio_VegCod, 0) AS Dettaglio_VegCod")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Iva_Indetraibile, 0) AS Iva_Indetraibile")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Iva_Indetraibile_Perc, 0) AS Iva_Indetraibile_Perc")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.PrincipiAttivi, 0) AS PrincipiAttivi")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.ClassiTossicologiche, 0) AS ClassiTossicologiche")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.DoseEtichetta_Value, 0) AS DoseEtichetta_Value")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Iva_Deto_Cod, 0) AS Iva_Deto_Cod")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Qta_Dettaglio1, 0) AS Qta_Dettaglio1")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Qta_Dettaglio2, 0) AS Qta_Dettaglio2")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Dettagli_Blocco_Flag, 0) AS Dettagli_Blocco_Flag")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Dettagli_Blocco_Username, 0) AS Dettagli_Blocco_Username")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Dettagli_Blocco_Data, 0) AS Dettagli_Blocco_Data")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Qualifica_Cod, 0) AS Qualifica_Cod")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Tariffa_Cod, 0) AS Tariffa_Cod")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Prezzo_Livello, 0) AS Prezzo_Livello")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Ordine_Det, 0) AS Ordine_Det")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Deroga_Cod, 0) AS Deroga_Cod")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.PrincipiAttiviPesi, 0) AS PrincipiAttiviPesi")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Buffer, 0) AS Buffer")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Rif_Esterno, 0) AS Rif_Esterno")
            strSql.AppendLine(",ISNULL(Movimenti_Dettagli_Carico.Rif_Esterno_2, 0) AS Rif_Esterno_2")
            strSql.AppendLine(",ISNULL(Mov_Dettagli_Riferimenti.Id_Mov_Det, 0) AS Conf_Id_Mov_Det")
            strSql.AppendLine(",Mov_Dettagli_Riferimenti.Tipo_Associazione")
            strSql.AppendLine("--,Movimenti.Cau_Mov")
            strSql.AppendLine("--,Movimenti.Mov_Desc")
            strSql.AppendLine("--,Movimenti.Peso")
            strSql.AppendLine("--,Movimenti.Tipo_Peso")
            strSql.AppendLine(",CTE_Campagna.CTE_Cod_Articolo")
            strSql.AppendLine(",CTE_Campagna.CTE_Mat_Des")
            strSql.AppendLine(",ISNULL(CTE_Campagna.CTE_Udm_Sim, '') AS Udm_Sim_Campagna")
            strSql.AppendLine("FROM Agenda ")
            strSql.AppendLine("")
            strSql.AppendLine("--INNER JOIN Movimenti ")
            strSql.AppendLine("--ON Agenda.Piva = Movimenti.Piva")
            strSql.AppendLine("--AND Agenda.Id_Agenda = Movimenti.Id_Agenda")
            strSql.AppendLine("")
            strSql.AppendLine("LEFT JOIN Movimenti Movimenti_Carico")
            strSql.AppendLine("ON Movimenti_Carico.Piva = Agenda.Piva")
            strSql.AppendLine("AND Movimenti_Carico.Id_Agenda = Agenda.Id_Agenda")
            strSql.AppendLine("AND Movimenti_Carico.Cau_Mov = '" & CAU_CARICO & "'")
            strSql.AppendLine("")
            strSql.AppendLine("LEFT JOIN Movimenti_Dettagli Movimenti_Dettagli_Carico")
            strSql.AppendLine("ON Movimenti_Dettagli_Carico.PIVA = Agenda.PIVA")
            strSql.AppendLine("AND Movimenti_Dettagli_Carico.Id_Agenda = Agenda.Id_Agenda")
            strSql.AppendLine("AND Movimenti_Dettagli_Carico.Id_Mov = Movimenti_Carico.Id_Mov")
            strSql.AppendLine("")
            strSql.AppendLine("LEFT JOIN Mov_Dettagli_Riferimenti ")
            strSql.AppendLine("ON Mov_Dettagli_Riferimenti.Piva_Rif = Agenda.Piva")
            strSql.AppendLine("AND Mov_Dettagli_Riferimenti.Id_Agenda_Rif = Agenda.Id_Agenda")
            strSql.AppendLine("AND (Mov_Dettagli_Riferimenti.Id_Mov_Rif = Movimenti_Carico.Id_Mov OR Mov_Dettagli_Riferimenti.Id_Mov_Rif = -1)")
            strSql.AppendLine("AND (Mov_Dettagli_Riferimenti.Id_Mov_Det_Rif = Movimenti_Dettagli_Carico.Id_Mov_Det OR Mov_Dettagli_Riferimenti.Id_Mov_Det_Rif = -1)")
            strSql.AppendLine("AND Mov_Dettagli_Riferimenti.Lav_Cod IN (" & Agro_SQL_Save_Clausola_IN(String.Join(", ", arrLavCodAccettazioniConf)) & ")")
            strSql.AppendLine("")
            strSql.AppendLine("LEFT JOIN Mov_Destinazioni Mov_Destinazioni_Carico")
            strSql.AppendLine("ON Mov_Destinazioni_Carico.Piva = Agenda.Piva  ")
            strSql.AppendLine("AND Mov_Destinazioni_Carico.Sa_Cod = Movimenti_Dettagli_Carico.Sa_Cod  ")
            strSql.AppendLine("AND Mov_Destinazioni_Carico.Id_Agenda = Agenda.Id_Agenda  ")
            strSql.AppendLine("AND Mov_Destinazioni_Carico.Id_Mov = Movimenti_Carico.Id_Mov  ")
            strSql.AppendLine("AND Mov_Destinazioni_Carico.Id_Mov_Det = Movimenti_Dettagli_Carico.Id_Mov_Det  ")
            strSql.AppendLine("")
            strSql.AppendLine("LEFT JOIN Centri_Aziendali Centri_Aziendali_Carico")
            strSql.AppendLine("ON Centri_Aziendali_Carico.PIVA = Agenda.Piva")
            strSql.AppendLine("AND Centri_Aziendali_Carico.sa_cod = Movimenti_Dettagli_Carico.Sa_Cod")
            strSql.AppendLine("")
            strSql.AppendLine("LEFT JOIN Fabbricati Magazzino_Carico")
            strSql.AppendLine("ON Magazzino_Carico.PIVA = Agenda.PIVA")
            strSql.AppendLine("AND Magazzino_Carico.SA_COD = Movimenti_Dettagli_Carico.Sa_Cod")
            strSql.AppendLine("AND Magazzino_Carico.Fabbricato_Cod = Mov_Destinazioni_Carico.Id_Destinazione")
            strSql.AppendLine("")
            strSql.AppendLine("INNER JOIN CTE_Campagna")
            strSql.AppendLine("ON CTE_Campagna.CTE_Piva = Agenda.PIVA")
            strSql.AppendLine("AND CTE_campagna.CTE_Id_Agenda = Agenda.Id_Agenda")
            strSql.AppendLine("AND (CTE_campagna.CTE_Mat_Cod = Movimenti_Dettagli_Carico.Mat_Cod OR Movimenti_Dettagli_Carico.Id_Mov_Det IS NULL)")
            strSql.AppendLine("AND (CTE_Campagna.CTE_Lotto = Movimenti_Dettagli_Carico.Lotto OR Movimenti_Dettagli_Carico.Id_Mov_Det IS NULL)")
            strSql.AppendLine("")
            strSql.AppendLine("INNER JOIN CTE_Varieta")
            strSql.AppendLine("ON CTE_Varieta.CTE_Piva = Agenda.PIVA")
            strSql.AppendLine("AND CTE_Varieta.CTE_Id_Agenda = Agenda.Id_Agenda")
            strSql.AppendLine("AND (CTE_Varieta.CTE_Mat_Cod = Movimenti_Dettagli_Carico.Mat_Cod OR Movimenti_Dettagli_Carico.Id_Mov_Det IS NULL)")
            strSql.AppendLine("AND (CTE_Varieta.CTE_Lotto = Movimenti_Dettagli_Carico.Lotto OR Movimenti_Dettagli_Carico.Id_Mov_Det IS NULL)")
            strSql.AppendLine("AND CTE_Varieta.CTE_Lotto = CTE_Campagna.CTE_Lotto -- Per evitare duplicazioni dei record a causa delle multiple join")
            strSql.AppendLine("AND CTE_Varieta.CTE_Mat_Cod = CTE_Campagna.CTE_Mat_Cod")
            strSql.AppendLine("")
            strSql.AppendLine("INNER JOIN CTE_Specie")
            strSql.AppendLine("ON CTE_Specie.CTE_Piva = Agenda.PIVA")
            strSql.AppendLine("AND CTE_Specie.CTE_Id_Agenda = Agenda.Id_Agenda")
            strSql.AppendLine("AND (CTE_Specie.CTE_Mat_Cod = Movimenti_Dettagli_Carico.Mat_Cod OR Movimenti_Dettagli_Carico.Id_Mov_Det IS NULL)")
            strSql.AppendLine("AND (CTE_Specie.CTE_Lotto = Movimenti_Dettagli_Carico.Lotto OR Movimenti_Dettagli_Carico.Id_Mov_Det IS NULL)")
            strSql.AppendLine("AND CTE_Specie.CTE_Lotto = CTE_Campagna.CTE_Lotto")
            strSql.AppendLine("AND CTE_Specie.CTE_Mat_Cod = CTE_Campagna.CTE_Mat_Cod")
            strSql.AppendLine("")
            strSql.AppendLine("INNER JOIN CTE_Specie_Des")
            strSql.AppendLine("ON CTE_Specie_Des.CTE_Piva = Agenda.PIVA")
            strSql.AppendLine("AND CTE_Specie_Des.CTE_Id_Agenda = Agenda.Id_Agenda")
            strSql.AppendLine("AND (CTE_Specie_Des.CTE_Mat_Cod = Movimenti_Dettagli_Carico.Mat_Cod OR Movimenti_Dettagli_Carico.Id_Mov_Det IS NULL)")
            strSql.AppendLine("AND (CTE_Specie_Des.CTE_Lotto = Movimenti_Dettagli_Carico.Lotto OR Movimenti_Dettagli_Carico.Id_Mov_Det IS NULL)")
            strSql.AppendLine("AND CTE_Specie_Des.CTE_Lotto = CTE_Campagna.CTE_Lotto")
            strSql.AppendLine("AND CTE_Specie_Des.CTE_Mat_Cod = CTE_Campagna.CTE_Mat_Cod")
            strSql.AppendLine("")
            strSql.AppendLine("LEFT JOIN Imprese_Progetti Imprese_Progetti_Carico")
            strSql.AppendLine("ON Imprese_Progetti_Carico.Piva = Agenda.PIVA")
            strSql.AppendLine("AND Imprese_Progetti_Carico.Progetto_Cod = Movimenti_Dettagli_Carico.Cod_Progetto")
            strSql.AppendLine("")
            strSql.AppendLine("WHERE Agenda.Lav_Cod = " & LAVCOD_RACCOLTA)
            strSql.AppendLine("AND Agenda.Piva = '" & Agro_SQL_SaveText(piva) & "'")
            strSql.AppendLine("AND (")
            strSql.AppendLine(" Mov_Dettagli_Riferimenti.Lav_Cod is null")
            If idMovDetConf <> 0 Then
                strSql.AppendLine(" OR Mov_Dettagli_Riferimenti.Id_Mov_Det = " & idMovDetConf)
            End If
            strSql.AppendLine(")")
            strSql.AppendLine("ORDER BY CTE_Campagna.CTE_Data_Movimento Desc, Mov_Dettagli_Riferimenti.Id_Mov_Det Desc")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Dim messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    ''' <summary>
    ''' Query usata da AgronicaSincronizzatore_2010\INDICODE_EDI_BMI_Import_Export\ImportatoreCAI_Giacenze.vb
    ''' </summary>
    ''' <param name="piva"></param>
    ''' <param name="inizioVal"></param>
    ''' <param name="fineVal"></param>
    ''' <param name="arrLavCod"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    Public Function DDTInterscambio_FattureAccompagnatorie_Ricevuti(ByVal piva As String, ByVal inizioVal As Date, ByVal fineVal As Date, ByVal arrLavCod As Integer(), ByRef objParametri As AgronicaCoreParametri) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Movimenti_Dettagli.DDTInterscambio_FattureAccompagnatorie_Ricevuti()"

        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.AppendLine("SELECT")
            strSql.AppendLine(" CAST(ROW_NUMBER() OVER(ORDER BY Agenda.Id_Agenda Desc) AS nvarchar) AS Chiave")
            strSql.AppendLine(" ,Agenda.Piva")
            strSql.AppendLine(" ,Imprese.rag_soc")
            strSql.AppendLine(" ,Agenda.Id_Agenda")
            strSql.AppendLine(" ,Movimenti.Id_Mov")
            strSql.AppendLine(" ,Movimenti_Dettagli_Mag.Id_Mov_Det")
            strSql.AppendLine(" ,Mov_Destinazioni_Mag.Id_Destinazione")
            strSql.AppendLine(" ,Mov_Destinazioni_Mag.Tipo_Destinazione")
            strSql.AppendLine(" ,Agenda.Lav_Cod")
            strSql.AppendLine(" ,Movimenti.Cau_Mov")
            strSql.AppendLine(" ,Movimenti.Data_Movimento")
            strSql.AppendLine(" ,Movimenti.Data_Creazione")
            strSql.AppendLine(" ,Movimenti.Data_Modifica")
            strSql.AppendLine(" ,Movimenti.Cod_RisUm")
            strSql.AppendLine(" ,Movimenti.Doc_Numero_Sin")
            strSql.AppendLine(" ,Movimenti.Doc_Numero")
            strSql.AppendLine(" ,Movimenti.Doc_Numero_Des")
            strSql.AppendLine(" ,Movimenti.Doc_Numero_Visualizzato")
            strSql.AppendLine(" ,Impresa_Fornitore.Piva AS Fornitore_Piva")
            strSql.AppendLine(" ,Imprese_Codici_Fornitore.Val_Cod AS Fornitore_Codice_Stabilimento")
            strSql.AppendLine(" ,Movimenti_Dettagli_Mag.Mov_Det_Des")
            strSql.AppendLine(" ,Movimenti_Dettagli_Mag.Elem_Cod")
            strSql.AppendLine(" ,Movimenti_Dettagli_Mag.Pro_Cod")
            strSql.AppendLine(" ,Movimenti_Dettagli_Mag.Mat_Cod")
            strSql.AppendLine(" ,Movimenti_Dettagli_Mag.Lotto")
            strSql.AppendLine(" ,Movimenti_Dettagli_Mag.Qta")
            strSql.AppendLine(" ,Mov_Destinazioni_Mag.Qta Dest_Qta")
            strSql.AppendLine(" ,Movimenti_Dettagli_Mag.Udm_Cod")
            strSql.AppendLine(" ,Mov_Destinazioni_Mag.Qta2 as Dest_Qta2")
            strSql.AppendLine(" ,Mov_Dettagli_Riferimenti.Piva_Rif")
            strSql.AppendLine(" ,Mov_Dettagli_Riferimenti.Id_Agenda_Rif")
            strSql.AppendLine(" ,Mov_Dettagli_Riferimenti.Id_Mov_Rif")
            strSql.AppendLine(" ,Mov_Dettagli_Riferimenti.Id_Mov_Det_Rif")
            strSql.AppendLine(" ,Mov_Dettaglio_Tecnico_Extra.N_Doc_Cliente")
            strSql.AppendLine(" ,Mov_Dettaglio_Tecnico_Extra.Data_Doc_Cliente")
            strSql.AppendLine("FROM Agenda")
            strSql.AppendLine("")
            strSql.AppendLine("INNER JOIN Imprese")
            strSql.AppendLine("ON Imprese.PIVA = Agenda.Piva")
            strSql.AppendLine("")
            strSql.AppendLine("INNER JOIN Pratiche")
            strSql.AppendLine("ON Agenda.Piva = Pratiche.Piva")
            strSql.AppendLine("")
            strSql.AppendLine("INNER JOIN Pratiche_Stati_Attuali")
            strSql.AppendLine("ON Pratiche_Stati_Attuali.Piva_SuperUser = Pratiche.Piva_SuperUser")
            strSql.AppendLine("AND Pratiche_Stati_Attuali.Pratica_Cod = Pratiche.Pratica_Cod")
            strSql.AppendLine("")
            strSql.AppendLine("INNER JOIN Movimenti")
            strSql.AppendLine("ON Agenda.Piva = Movimenti.Piva")
            strSql.AppendLine("AND Agenda.Id_Agenda = Movimenti.Id_Agenda")
            strSql.AppendLine("")
            strSql.AppendLine("INNER JOIN Risorse_Umane AS Risorsa_Fornitore")
            strSql.AppendLine("ON Risorsa_Fornitore.Cod_RisUm = Movimenti.Cod_RisUm")
            strSql.AppendLine("")
            strSql.AppendLine("INNER JOIN Imprese AS Impresa_Fornitore")
            strSql.AppendLine("ON Impresa_Fornitore.PIVA = Risorsa_Fornitore.Cod_Contatto")
            strSql.AppendLine("")
            strSql.AppendLine("INNER JOIN Imprese_Codici AS Imprese_Codici_Fornitore")
            strSql.AppendLine("ON Imprese_Codici_Fornitore.PIVA = Impresa_Fornitore.Piva")
            strSql.AppendLine("AND Imprese_Codici_Fornitore.Id_Cod = " & enum_CodiciAnagrafe.CodiceStabilimento)
            strSql.AppendLine("")
            strSql.AppendLine("LEFT JOIN Movimenti Movimenti_Mag")
            strSql.AppendLine("ON Movimenti_Mag.Piva = Agenda.Piva")
            strSql.AppendLine("AND Movimenti_Mag.Id_Agenda = Agenda.Id_Agenda")
            strSql.AppendLine("")
            strSql.AppendLine("LEFT JOIN Movimenti_Dettagli Movimenti_Dettagli_Mag")
            strSql.AppendLine("ON Movimenti_Dettagli_Mag.PIVA = Agenda.PIVA")
            strSql.AppendLine("AND Movimenti_Dettagli_Mag.Id_Agenda = Agenda.Id_Agenda")
            strSql.AppendLine("AND Movimenti_Dettagli_Mag.Id_Mov = Movimenti_Mag.Id_Mov")
            strSql.AppendLine("")
            strSql.AppendLine("LEFT JOIN Mov_Dettagli_Riferimenti")
            strSql.AppendLine("ON Mov_Dettagli_Riferimenti.Piva_Rif = Agenda.Piva")
            strSql.AppendLine("AND Mov_Dettagli_Riferimenti.Id_Agenda_Rif = Agenda.Id_Agenda")
            strSql.AppendLine("AND (Mov_Dettagli_Riferimenti.Id_Mov_Rif = Movimenti_Mag.Id_Mov OR Mov_Dettagli_Riferimenti.Id_Mov_Rif = -1)")
            strSql.AppendLine("AND (Mov_Dettagli_Riferimenti.Id_Mov_Det_Rif = Movimenti_Dettagli_Mag.Id_Mov_Det OR Mov_Dettagli_Riferimenti.Id_Mov_Det_Rif = -1)")
            strSql.AppendLine("AND Mov_Dettagli_Riferimenti.Lav_Cod IN (" & Agro_SQL_Save_Clausola_IN(String.Join(", ", arrLavCod)) & ")")
            strSql.AppendLine("")
            strSql.AppendLine("LEFT JOIN Mov_Destinazioni Mov_Destinazioni_Mag")
            strSql.AppendLine("ON Mov_Destinazioni_Mag.Piva = Agenda.Piva")
            strSql.AppendLine("AND Mov_Destinazioni_Mag.Sa_Cod = Movimenti_Dettagli_Mag.Sa_Cod")
            strSql.AppendLine("AND Mov_Destinazioni_Mag.Id_Agenda = Agenda.Id_Agenda")
            strSql.AppendLine("AND Mov_Destinazioni_Mag.Id_Mov = Movimenti_Mag.Id_Mov")
            strSql.AppendLine("AND Mov_Destinazioni_Mag.Id_Mov_Det = Movimenti_Dettagli_Mag.Id_Mov_Det")
            strSql.AppendLine("")
            strSql.AppendLine("LEFT JOIN Mov_Dettaglio_Tecnico_Extra")
            strSql.AppendLine("ON Mov_Dettaglio_Tecnico_Extra.Piva = Agenda.Piva")
            strSql.AppendLine("AND Mov_Dettaglio_Tecnico_Extra.Sa_Cod = Movimenti_Dettagli_Mag.Sa_Cod")
            strSql.AppendLine("AND Mov_Dettaglio_Tecnico_Extra.Id_Agenda = Agenda.Id_Agenda")
            strSql.AppendLine("AND Mov_Dettaglio_Tecnico_Extra.Id_Mov = Movimenti_Mag.Id_Mov")
            strSql.AppendLine("AND Mov_Dettaglio_Tecnico_Extra.Id_Mov_Det = Movimenti_Dettagli_Mag.Id_Mov_Det")
            strSql.AppendLine("")
            strSql.AppendLine("WHERE Agenda.Lav_Cod IN (" & Agro_SQL_Save_Clausola_IN(String.Join(", ", arrLavCod)) & ")")
            strSql.AppendLine(" AND Pratiche_Stati_Attuali.Stato_Cod = " & enum_WWorflow_WAnagraficaStati.QdCAttivo)
            strSql.AppendLine(" AND Movimenti.Cau_Mov = '" & CAU_REGISTRAZIONI & "'")
            strSql.AppendLine(" AND Movimenti_Mag.Cau_Mov IN ('" & CAU_CARICO & "', '" & CAU_SCARICO & "')")
            If piva <> "" Then
                strSql.AppendLine("AND Agenda.Piva = '" & Agro_SQL_SaveText(piva) & "'")
            End If
            strSql.AppendLine("AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(fineVal))
            strSql.AppendLine("AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(inizioVal))
            strSql.AppendLine("ORDER BY Movimenti.Data_Movimento Desc, Mov_Dettagli_Riferimenti.Id_Mov_Det Desc")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Dim messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    ''' <summary>
    ''' Stessa funzione di Leggi_Macroelementi_Distribuiti ma in input prende una Lista di Agenda e non uno singolo
    ''' </summary>
    ''' <returns></returns>
    Public Function Leggi_Macroelementi_Distribuiti_List_Id_Agenda_Esclusi(ByRef N_Distribuito_Ha As Decimal,
                                                                            ByRef P_Distribuito_Ha As Decimal,
                                                                            ByRef K_Distribuito_Ha As Decimal,
                                                                            ByRef Mg_Distribuito_Ha As Decimal,
                                                                            ByRef Cu_Distribuito_Ha As Decimal,
                                                                            ByRef N_Distribuito_Imp As Decimal,
                                                                            ByRef P_Distribuito_Imp As Decimal,
                                                                            ByRef K_Distribuito_Imp As Decimal,
                                                                            ByRef Mg_Distribuito_Imp As Decimal,
                                                                            ByRef Cu_Distribuito_Imp As Decimal,
                                                                            ByVal Piva As String,
                                                                            ByVal Sa_Cod As Integer,
                                                                            ByVal Appezza As Integer,
                                                                            ByVal Id_Reg As Integer,
                                                                            ByVal Progetto_Cod As Integer,
                                                                            ByVal Validita_Inizio As Date,
                                                                            ByVal Validita_Fine As Date,
                                                                            ByVal List_Id_Agenda_Esclusi As List(Of Integer),
                                                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                            ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Dettagli_R.Leggi_Macroelementi_Distribuiti_Id_Agenda_Esclusi_List()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable
        Dim supTratt As Decimal
        Dim supImp As Decimal

        Try
            '---------------------------------------------


            strSql.Length = 0
            strSql.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
            strSql.AppendLine(" SELECT DISTINCT ")
            strSql.AppendLine(" fertilizzanti.fer_cod, fertilizzanti.FER_DES, ")
            strSql.AppendLine(" ISNULL(fertilizzanti.N,0) AS N, ISNULL(fertilizzanti.P2O5,0) AS P2O5, ISNULL(fertilizzanti.K2O,0) AS K2O, ISNULL(fertilizzanti.MgO,0) AS MgO, ISNULL(fertilizzanti.Cu,0) AS Cu,")
            strSql.AppendLine(" Agenda.des_lib, Imprese_Progetti.Progetto_Nome as Lotto, ")
            strSql.AppendLine(" Movimenti.Data_Movimento as Data_Creazione, dbo.Movimenti.Mov_Desc, ")
            strSql.AppendLine(" Movimenti.Mezzo, Mov_Dettaglio_Tecnico.Qta_Ril, unitamisura.UDM_DES, ")
            strSql.AppendLine(" Cultivar.CUL_DES, Reg_Impianti.sup_imp, Mov_Destinazioni.Qta, ")
            strSql.AppendLine(" Mov_Destinazioni.Qta2, ")
            strSql.AppendLine(" Operazioni.lav_cod, Operazioni.lav_des, Movimenti_dettagli.UDM_COD, Movimenti_dettagli.Extra_Int, ")
            strSql.AppendLine(" Reg_Impianti.ID_REG, Agenda.Id_Agenda,  ")
            strSql.AppendLine(" ISNULL(Mov_Dettaglio_Tecnico.N,0) AS N_Tecnico, ")
            strSql.AppendLine(" ISNULL(Mov_Dettaglio_Tecnico.K,0) AS K2O_Tecnico, ")
            strSql.AppendLine(" ISNULL(Mov_Dettaglio_Tecnico.P,0) AS P2O5_Tecnico, ")
            strSql.AppendLine(" ISNULL(Mov_Dettaglio_Tecnico.Mg,0) AS MgO_Tecnico, ")
            strSql.AppendLine(" ISNULL(Mov_Dettaglio_Tecnico.Efficienza,1) AS Efficienza, ")
            strSql.AppendLine(" ISNULL(Mov_Dettaglio_Tecnico.Cu,0) AS Cu_Tecnico ")

            strSql.AppendLine(" FROM Mov_Dettaglio_Tecnico RIGHT OUTER JOIN ")
            strSql.AppendLine(" Reg_Impianti INNER JOIN ")
            strSql.AppendLine(" Mov_Destinazioni ON Reg_Impianti.PIVA = Mov_Destinazioni.Piva AND Reg_Impianti.SA_COD = Mov_Destinazioni.Sa_Cod AND ")
            strSql.AppendLine(" Reg_Impianti.ID_REG = Mov_Destinazioni.Id_Destinazione AND Reg_Impianti.APPEZZA = Mov_Destinazioni.Appezza ")
            strSql.AppendLine(" LEFT OUTER JOIN Imprese_Progetti On Imprese_Progetti.Piva = Reg_Impianti.Piva AND Imprese_Progetti.Sa_Cod = Reg_Impianti.Sa_Cod AND Imprese_Progetti.Appezza = Reg_Impianti.Appezza AND Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg ")
            strSql.AppendLine(" INNER JOIN Operazioni INNER JOIN ")
            strSql.AppendLine(" Agenda INNER JOIN ")
            strSql.AppendLine(" Movimenti ON Agenda.Id_Agenda = Movimenti.Id_Agenda AND Agenda.PIVA = Movimenti.PIVA AND ")
            strSql.AppendLine(" Agenda.Sa_Cod = Movimenti.Sa_Cod INNER JOIN ")
            strSql.AppendLine(" Movimenti_dettagli ON Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov AND Movimenti.PIVA = Movimenti_dettagli.PIVA AND ")
            strSql.AppendLine(" Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda INNER JOIN ")
            strSql.AppendLine(" Fertilizzanti fertilizzanti ON fertilizzanti.Fer_Cod = Movimenti_dettagli.Pro_Cod INNER JOIN ")
            strSql.AppendLine(" UnitaMisura unitamisura ON Movimenti_dettagli.Udm_Cod = unitamisura.UDM_COD ON Operazioni.LAV_COD = Agenda.Lav_Cod ON ")
            strSql.AppendLine(" Mov_Destinazioni.Piva = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Sa_Cod = Movimenti_dettagli.Sa_Cod AND ")
            strSql.AppendLine(" Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND ")
            strSql.AppendLine(" Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det ON Mov_Dettaglio_Tecnico.Piva = Movimenti_dettagli.PIVA AND ")
            strSql.AppendLine(" Mov_Dettaglio_Tecnico.Sa_Cod = Movimenti_dettagli.Sa_Cod AND Mov_Dettaglio_Tecnico.Id_Agenda = Movimenti_dettagli.Id_Agenda AND ")
            strSql.AppendLine(" Mov_Dettaglio_Tecnico.Id_Mov = Movimenti_dettagli.Id_Mov AND Mov_Dettaglio_Tecnico.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det ")
            strSql.AppendLine(" LEFT OUTER JOIN SpecieVegetali INNER JOIN Cultivar ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod  ")
            strSql.AppendLine(" ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod ")

            '14 = concimazione in pieno campo
            '26 = fertirrigazione
            '106 = trattamento antibutteratura 
            '123 = concimazione fogliare
            '124 = distribuzione ammendanti organici o liquami
            '156 = sarchiatura con concimazione
            strSql.AppendLine("WHERE Agenda.Lav_Cod IN (14, 26, 106, 123, 124, 156) ")

            If Piva <> "" Then
                strSql.AppendLine(" AND Mov_Destinazioni.PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            End If

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            End If

            If Appezza <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Appezza = " & Agro_SQL_SaveNum(Appezza) & "  ")
            End If

            If Id_Reg <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Id_Destinazione =  " & Agro_SQL_SaveNum(Id_Reg) & " ")
            End If

            If Progetto_Cod <> 0 Then
                strSql.AppendLine(" AND Imprese_Progetti.Progetto_Cod =" & Agro_SQL_SaveNum(Progetto_Cod))
            End If

            strSql.AppendLine(" AND Movimenti.Data_Movimento >=" & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.AppendLine(" AND Movimenti.Data_Movimento <=" & Agro_SQL_SaveDate(Validita_Fine))
            strSql.AppendLine(" AND Imprese_Progetti.validita_fine >=" & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.AppendLine(" AND Imprese_Progetti.validita_inizio <=" & Agro_SQL_SaveDate(Validita_Fine))



            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Mov_Destinazioni.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Mov_Destinazioni.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            strSql.AppendLine(" ORDER BY Data_Movimento DESC ")


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If Not IsNothing(dt) Then

                Dim N_Prodotto As Decimal
                Dim P_Prodotto As Decimal
                Dim K_Prodotto As Decimal
                Dim Mg_Prodotto As Decimal
                Dim Cu_Prodotto As Decimal
                Dim i As Integer

                For i = 0 To dt.Rows.Count - 1

                    'Controllo che l'Id_Agenda non sia già stato verificato
                    If Not List_Id_Agenda_Esclusi.Contains(CInt(dt.Rows(i).Item("Id_Agenda"))) Then

                        'Mi baso solo sugli apporti salvati nel dettaglio tecnico come viene fatto nel Report Sostenibilità

                        N_Prodotto = CDec(dt.Rows(i).Item("N_Tecnico"))

                        P_Prodotto = CDec(dt.Rows(i).Item("P2O5_Tecnico"))

                        K_Prodotto = CDec(dt.Rows(i).Item("K2O_Tecnico"))

                        Mg_Prodotto = CDec(dt.Rows(i).Item("MgO_Tecnico"))

                        Cu_Prodotto = CDec(dt.Rows(i).Item("Cu_Tecnico"))

                        'Aggiornamenti Macroelementi Distribuiti

                        supImp = CDec(dt.Rows(i).Item("Sup_Imp"))

                        supTratt = 0
                        supTratt = CDec(dt.Rows(i).Item("Qta2"))
                        If supTratt = 0 Then
                            supTratt = supImp
                        End If

                        If CDec(dt.Rows(i).Item("efficienza")) < 1 Then
                            N_Distribuito_Ha += CDec(dt.Rows(i).Item("Qta")) / supTratt * N_Prodotto / 100 * CDec(dt.Rows(i).Item("efficienza"))
                            N_Distribuito_Imp += CDec(dt.Rows(i).Item("Qta")) * N_Prodotto / 100 * CDec(dt.Rows(i).Item("efficienza"))
                        Else
                            N_Distribuito_Ha += CDec(dt.Rows(i).Item("Qta")) * N_Prodotto / 100 / supTratt
                            N_Distribuito_Imp += CDec(dt.Rows(i).Item("Qta")) * N_Prodotto / 100
                        End If

                        P_Distribuito_Ha += CDec(dt.Rows(i).Item("Qta")) * P_Prodotto / 100 / supTratt
                        K_Distribuito_Ha += CDec(dt.Rows(i).Item("Qta")) * K_Prodotto / 100 / supTratt
                        Mg_Distribuito_Ha += CDec(dt.Rows(i).Item("Qta")) * Mg_Prodotto / 100 / supTratt
                        Cu_Distribuito_Ha += CDec(dt.Rows(i).Item("Qta")) * Cu_Prodotto / 100 / supTratt

                        P_Distribuito_Imp += CDec(dt.Rows(i).Item("Qta")) * P_Prodotto / 100
                        K_Distribuito_Imp += CDec(dt.Rows(i).Item("Qta")) * K_Prodotto / 100
                        Mg_Distribuito_Imp += CDec(dt.Rows(i).Item("Qta")) * Mg_Prodotto / 100
                        Cu_Distribuito_Imp += CDec(dt.Rows(i).Item("Qta")) * Cu_Prodotto / 100

                    End If

                Next

            End If

            Return dt

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

            Return Nothing

        End Try

    End Function

    Public Function RicavaDettagliFiltroAgendeElemCod(ByRef listaIdAgenda As List(Of Integer), ByRef listaElemCod As List(Of Integer), ByRef objParametriServer As AgronicaCoreParametri) As List(Of Integer)

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Dettagli_R.RicavaDettagliFiltroAgendeElemCod()"

        Dim listaIdAgendaFiltrata As New List(Of Integer)()
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try
            strSql.Length = 0
            strSql.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
            strSql.AppendLine(" SELECT DISTINCT Id_Agenda ")
            strSql.AppendLine(" FROM Movimenti_Dettagli ")
            strSql.AppendLine(" WHERE 1 = 1 ")

            If listaIdAgenda.Count > 0 Then
                strSql.AppendLine(" AND Id_Agenda IN (" & String.Join(",", listaIdAgenda) & ")")
            End If

            If listaElemCod.Count > 0 Then
                strSql.AppendLine(" AND Elem_Cod IN (" & String.Join(",", listaElemCod) & ")")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametriServer, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            For Each dr As DataRow In dt.Rows
                listaIdAgendaFiltrata.Add(dr.Field(Of Integer)("Id_Agenda"))
            Next

        Catch ex As Exception
            Scrivi_LOG(objParametriServer, nomeRoutine, Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False))
        End Try

        Return listaIdAgendaFiltrata

    End Function

    Public Function OttieniStoricoInstallazioneReinneschi(ByVal Piva As String,
                                                          ByVal Sa_Cod As Integer,
                                                          ByVal Id_Agenda As Integer,
                                                          ByVal Id_Mov As Integer,
                                                          ByVal Id_Mov_Det As Integer,
                                                          ByVal xFiltroAggiuntivo As String,
                                                          ByVal xOrderBy As String,
                                                          ByVal objParametriServer As AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Dettagli_R.OttieniStoricoInstallazioneReinneschi()"

        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try
            strSql.Length = 0
            strSql.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
            strSql.AppendLine(" SELECT DISTINCT Movimenti_Dettagli.Piva,Movimenti_Dettagli.Sa_Cod, Movimenti_Dettagli.Id_Agenda, Movimenti_Dettagli.Id_Mov, ")
            strSql.AppendLine(" Movimenti_Dettagli.Id_Mov_Det, Movimenti.Data_Movimento AS Data_Installazione,")
            strSql.AppendLine("   (")
            strSql.AppendLine("        CASE")
            strSql.AppendLine("        WHEN ( ISNULL(Formulati.DurataFeromone,0) > 0)")
            strSql.AppendLine("            THEN  DATEADD(day,ISNULL(Formulati.DurataFeromone,0),Movimenti.Data_Movimento) ")
            strSql.AppendLine("            ELSE CONVERT(DateTime,'1900/01/01',120) ")
            strSql.AppendLine("        END ")
            strSql.AppendLine("     ) AS Data_Scadenza_Feromone_Installazione,")
            strSql.AppendLine(" ISNULL(Movimenti_Dettagli_Reinneschi.Piva,'') AS Piva_Reinnesco,ISNULL(Movimenti_Dettagli_Reinneschi.Sa_Cod,0) AS Sa_Cod_Reinnesco, ISNULL(Movimenti_Dettagli_Reinneschi.Id_Agenda,0) AS Id_Agenda_Reinnesco,")
            strSql.AppendLine(" ISNULL(Movimenti_Dettagli_Reinneschi.Id_Mov,0) AS Id_Mov_Reinnesco,ISNULL(Movimenti_Dettagli_Reinneschi.Id_Mov_Det,0) AS Id_Mov_Det_Reinnesco,")
            strSql.AppendLine(" ISNULL(Movimenti_Reinneschi.Data_Movimento,CONVERT(DateTime,'1900/01/01',120)) AS Data_Reinnesco,")
            strSql.AppendLine("    (")
            strSql.AppendLine("        CASE")
            strSql.AppendLine("        WHEN ( ISNULL(Formulati.DurataFeromone,0) > 0 AND Movimenti_Reinneschi.Data_Movimento IS NOT NULL)")
            strSql.AppendLine("            THEN  DATEADD(day,ISNULL(Formulati.DurataFeromone,0),Movimenti_Reinneschi.Data_Movimento) ")
            strSql.AppendLine("            ELSE CONVERT(DateTime,'1900/01/01',120) ")
            strSql.AppendLine("        END ")
            strSql.AppendLine("     ) AS Data_Scadenza_Feromone_Reinnesco")
            strSql.AppendLine(" FROM Movimenti_Dettagli ")
            strSql.AppendLine("    INNER JOIN Movimenti ")
            strSql.AppendLine("    ON  Movimenti.Piva = Movimenti_Dettagli.Piva ")
            strSql.AppendLine("    AND Movimenti.Sa_Cod = Movimenti_Dettagli.Sa_Cod ")
            strSql.AppendLine("    AND Movimenti.Id_Agenda = Movimenti_Dettagli.Id_Agenda ")
            strSql.AppendLine("    AND Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov ")
            strSql.AppendLine("          AND Movimenti.Cau_Mov = '" & CAU_TRATTAMENTO & "'")
            strSql.AppendLine("    INNER JOIN Agenda ")
            strSql.AppendLine("    ON  Movimenti.Piva = Agenda.Piva")
            strSql.AppendLine("    AND Movimenti.Sa_Cod = Agenda.Sa_Cod")
            strSql.AppendLine("    AND Movimenti.Id_Agenda = Agenda.Id_Agenda")
            strSql.AppendLine(" INNER JOIN Formulati ")
            strSql.AppendLine(" ON Movimenti_Dettagli.Pro_Cod = Formulati.Fr_Cod ")
            strSql.AppendLine(" LEFT JOIN Mov_Dettagli_Riferimenti ")
            strSql.AppendLine(" ON  Mov_Dettagli_Riferimenti.Piva = Movimenti_Dettagli.Piva ")
            strSql.AppendLine(" AND Mov_Dettagli_Riferimenti.Sa_Cod = Movimenti_Dettagli.Sa_Cod ")
            strSql.AppendLine(" AND Mov_Dettagli_Riferimenti.Id_Agenda = Movimenti_Dettagli.Id_Agenda ")
            strSql.AppendLine(" AND Mov_Dettagli_Riferimenti.Id_Mov = Movimenti_Dettagli.Id_Mov ")
            strSql.AppendLine(" AND Mov_Dettagli_Riferimenti.Id_Mov_Det = Movimenti_Dettagli.Id_Mov_Det ")
            strSql.AppendLine("       AND Mov_Dettagli_Riferimenti.Lav_Cod_Rif = " & LAVCOD_REINNESCO_TRAPPOLE)
            strSql.AppendLine(" LEFT JOIN Movimenti_Dettagli AS Movimenti_Dettagli_Reinneschi ")
            strSql.AppendLine(" ON  Movimenti_Dettagli_Reinneschi.Piva = Mov_Dettagli_Riferimenti.Piva_Rif ")
            strSql.AppendLine(" AND Movimenti_Dettagli_Reinneschi.Sa_Cod = Mov_Dettagli_Riferimenti.Sa_Cod_Rif ")
            strSql.AppendLine(" AND Movimenti_Dettagli_Reinneschi.Id_Agenda = Mov_Dettagli_Riferimenti.Id_Agenda_Rif ")
            strSql.AppendLine(" AND Movimenti_Dettagli_Reinneschi.Id_Mov = Mov_Dettagli_Riferimenti.Id_Mov_Rif ")
            strSql.AppendLine(" AND Movimenti_Dettagli_Reinneschi.Id_Mov_Det = Mov_Dettagli_Riferimenti.Id_Mov_Det_Rif ")
            strSql.AppendLine(" LEFT JOIN Movimenti AS Movimenti_Reinneschi ")
            strSql.AppendLine(" ON  Movimenti_Reinneschi.Piva = Movimenti_Dettagli_Reinneschi.Piva ")
            strSql.AppendLine(" AND Movimenti_Reinneschi.Sa_Cod = Movimenti_Dettagli_Reinneschi.Sa_Cod ")
            strSql.AppendLine(" AND Movimenti_Reinneschi.Id_Agenda = Movimenti_Dettagli_Reinneschi.Id_Agenda ")
            strSql.AppendLine(" AND Movimenti_Reinneschi.Id_Mov = Movimenti_Dettagli_Reinneschi.Id_Mov ")
            strSql.AppendLine(" WHERE Agenda.Lav_Cod = " & LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA)
            strSql.AppendLine(" AND (Movimenti_Dettagli_Reinneschi.Pro_Cod IS NULL OR Movimenti_Dettagli.Pro_Cod = Movimenti_Dettagli_Reinneschi.Pro_Cod)")

            If Piva <> "" Then
                strSql.AppendLine(" AND Movimenti_Dettagli.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND Movimenti_Dettagli.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "")
            End If

            If Id_Agenda <> 0 Then
                strSql.AppendLine(" AND Movimenti_Dettagli.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "")
            End If

            If Id_Mov <> 0 Then
                strSql.AppendLine(" AND Movimenti_Dettagli.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "")
            End If

            If Id_Mov_Det <> 0 Then
                strSql.AppendLine(" AND Movimenti_Dettagli.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & xFiltroAggiuntivo)
            End If

            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & xOrderBy)
            Else
                strSql.AppendLine(" ORDER BY Data_Reinnesco ASC ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametriServer, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Scrivi_LOG(objParametriServer, nomeRoutine, Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False))
        End Try

        Return dt

    End Function

End Class



'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§



Public Class Movimenti_Dettagli_W
    Inherits AgronicaCoreDataProvider.DataProvider

#Region "Scrittura"

    Public Function Scrivi(ByVal Piva As String,
                           ByVal Sa_Cod As Integer,
                           ByVal Id_Agenda As Integer,
                           ByVal Id_Mov As Integer,
                           ByVal Id_Mov_Det As Integer,
                           ByVal Elem_Cod As Integer,
                           ByVal Pro_Cod As Integer,
                           ByVal Mat_Cod As Integer,
                           ByVal Mov_Det_Des As String,
                           ByVal Qta As Decimal,
                           ByVal Udm_Cod As Integer,
                           ByVal Cod_Iva As Integer,
                           ByVal Jolly_Int As Integer,
                           ByVal Sconto As Decimal,
                           ByVal Prezzo_Unitario As Decimal,
                           ByVal Prezzo_Unitario_Netto As Decimal,
                           ByVal Cod_Conto As Integer,
                           ByVal Cal_Cod As Integer,
                           ByVal Cod_Progetto As Integer,
                           ByVal Fase_Cod As Integer,
                           ByVal Extra_Str As String,
                           ByVal Extra_Int As Integer,
                           ByVal Extra_Date As Date,
                           ByVal Ric_Cod As Integer,
                           ByVal Anno As Integer,
                           ByVal Imponibile As Decimal,
                           ByVal Imponibile_Netto As Decimal,
                           ByVal Iva As Decimal,
                           ByVal Listino_Cod As Integer,
                           ByVal Contabilizzato As Integer,
                           ByVal Pendente As Integer,
                           ByVal Lotto As String,
                           ByVal Udm_Cod_Extra As Integer,
                           ByVal Qta_Extra As Decimal,
                           ByVal Qta_Extra_Totale As Decimal,
                           ByVal Prezzo_Effettivo As Decimal,
                           ByVal Variazione As Decimal,
                           ByVal Tara As Decimal,
                           ByVal ChkLayOut_Hide As Integer,
                           ByVal ChkIva_Manuale As Integer,
                           ByVal Cod_IvaIndetraibile As Integer,
                           ByVal TempoCarenza As Integer,
                           ByVal DoseEtichetta As String,
                           ByVal Turno_Cod As Integer,
                           ByVal ID_Attivita As Integer,
                           ByVal Dettaglio_VegCod As Integer,
                           ByVal PrincipiAttivi As String,
                           ByVal ClassiTossicologiche As String,
                           ByVal DoseEtichetta_Value As String,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = "",
                           Optional ByVal Mat_Cod_alias As Integer = 0,
                           Optional ByVal Qta_Dettaglio1 As Decimal = 0,
                           Optional ByVal Qta_Dettaglio2 As Decimal = 0,
                           Optional ByVal Sconto_Listino As Decimal = 0,
                           Optional ByVal Sconto_Modalita As Integer = 0,
                           Optional ByVal Sconto_Testo As String = "",
                           Optional ByVal Qualifica_Cod As Integer = 0,
                           Optional ByVal Tariffa_Cod As Integer = 0,
                           Optional ByVal Mezzo_Det As Integer = -1,
                           Optional ByVal Ric_cod_Pat As Integer = 0,
                           Optional ByVal Cod_Conto_Pat As Integer = 0,
                           Optional ByVal Iva_Indetraibile As Decimal = 0,
                           Optional ByVal Iva_Indetraibile_Perc As Decimal = 0,
                           Optional ByVal Iva_Deto_Cod As Integer = 0,
                           Optional ByVal Dettagli_Blocco_Flag As Integer = 0,
                           Optional ByVal Dettagli_Blocco_Username As String = "0",
                           Optional ByVal Dettagli_Blocco_Data As DateTime = AGRODATAINIZIO,
                           Optional ByVal Ordine_Det As Integer = 0,
                           Optional ByVal Deroga_Cod As Integer = 0,
                           Optional ByVal Prezzo_Livello As Integer = 0,
                           Optional ByVal PrincipiAttiviPesi As String = "",
                           Optional ByVal Buffer As String = "",
                           Optional ByVal Rif_Esterno As String = "",
                           Optional ByVal Rif_Esterno_2 As String = "",
                           Optional ByVal PrincipiAttiviPercAbb As String = "",
                           Optional ByVal Polverulento As Integer = 0,
                           Optional ByVal Dettaglio_IdCod As Integer = 0,
                           Optional ByVal Dettaglio_GenCod As Integer = 0,
                           Optional ByVal Dettaglio_SpeCod As Integer = 0,
                           Optional ByVal Dettaglio_IProCod As Integer = 0
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Movimenti_Dettagli_W.Scrivi()"

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

            strSql.AppendLine(" INSERT INTO Movimenti_Dettagli ")
            strSql.AppendLine("        ( Piva,      Sa_Cod,      Id_Agenda,           Id_Mov,           Id_Mov_Det,                                             ")
            strSql.AppendLine("          Elem_Cod,  Pro_Cod,     Mat_Cod, Mat_Cod_alias,             Mov_Det_Des,      Qta,                     Udm_Cod,        ")
            strSql.AppendLine("          Cod_Iva,   Jolly_Int,   Sconto,              Prezzo_Unitario,  Prezzo_Unitario_Netto,   Cod_Conto,       Cod_Progetto, ")
            strSql.AppendLine("          Fase_Cod,  Cal_Cod,     Extra_Str,           Extra_Int,        Extra_Date,                                             ")
            strSql.AppendLine("          Ric_Cod,   Anno,        Imponibile,          Imponibile_Netto, Iva,                     Listino_Cod,                   ")
            strSql.AppendLine("          Contabilizzato,         Pendente,            Lotto,            Udm_Cod_Extra,           Qta_Extra,                     ")
            strSql.AppendLine("          Qta_Extra_Totale,       Prezzo_Effettivo,    Variazione,       Tara,                    ChkLayOut_Hide,                ")
            strSql.AppendLine("          ChkIva_Manuale,         Cod_IvaIndetraibile, TempoCarenza ,    DoseEtichetta ,                                         ")
            strSql.AppendLine("          Turno_Cod,              ID_Attivita,         Dettaglio_VegCod,                                                         ")
            strSql.AppendLine("          PrincipiAttivi,         PrincipiAttiviPesi,  Buffer, ClassiTossicologiche, DoseEtichetta_Value, Qta_Dettaglio1, Qta_Dettaglio2, ")
            strSql.AppendLine("          Qualifica_Cod,          Tariffa_Cod, ")

            strSql.AppendLine("          Sconto_Listino,         Sconto_Modalita,     Sconto_Testo, ")

            strSql.AppendLine("          Mezzo_Det,              Ric_Cod_Pat,                   Cod_Conto_Pat, ")
            strSql.AppendLine("          Iva_Indetraibile,       Iva_Indetraibile_Perc,         Iva_Deto_Cod, ")
            strSql.AppendLine("          Dettagli_Blocco_Flag,   Dettagli_Blocco_Username,      Dettagli_Blocco_Data, ")
            strSql.AppendLine("          Ordine_Det,             Deroga_Cod,                    Prezzo_Livello, ")
            strSql.AppendLine("          Rif_Esterno,            Rif_Esterno_2,                 PrincipiAttiviPercAbb, ")
            strSql.AppendLine("          Polverulento, Dettaglio_IdCod, Dettaglio_GenCod, Dettaglio_SpeCod, Dettaglio_IProCod, ")


            strSql.AppendLine("          Inviato,            DataInvio, ")
            strSql.AppendLine("          Data_Creazione,     Data_Modifica, ")
            strSql.AppendLine("          UserName_Creazione, UserName_Modifica, ")
            strSql.AppendLine("          Validita_Inizio,    Validita_Fine ")
            strSql.AppendLine("          ) ")

            strSql.AppendLine(" VALUES (")
            strSql.AppendLine("          '" & Agro_SQL_SaveText(Piva) & "'  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Id_Agenda) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Id_Mov) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Id_Mov_Det) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Elem_Cod) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Pro_Cod) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Mat_Cod) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Mat_Cod_alias) & "  ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(Mov_Det_Des) & "'  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Qta) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Udm_Cod) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Cod_Iva) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Jolly_Int) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Sconto) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Prezzo_Unitario) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Prezzo_Unitario_Netto) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Cod_Conto) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Cod_Progetto) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Fase_Cod) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Cal_Cod) & "  ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(Extra_Str) & "'  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Extra_Int) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Extra_Date) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Ric_Cod) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Anno) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Imponibile) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Imponibile_Netto) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Iva) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Listino_Cod) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Contabilizzato) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Pendente) & "  ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(Lotto) & "'  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Udm_Cod_Extra) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Qta_Extra) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Qta_Extra_Totale) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Prezzo_Effettivo) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Variazione) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Tara) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(ChkLayOut_Hide) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(ChkIva_Manuale) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Cod_IvaIndetraibile) & "  ")

            strSql.AppendLine("         , " & Agro_SQL_SaveNum(TempoCarenza) & "  ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(DoseEtichetta) & "' ")

            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Turno_Cod) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(ID_Attivita) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Dettaglio_VegCod) & "  ")

            strSql.AppendLine("         , '" & Agro_SQL_SaveText(PrincipiAttivi) & "' ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(PrincipiAttiviPesi) & "' ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(Buffer) & "' ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(ClassiTossicologiche) & "' ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(DoseEtichetta_Value) & "' ")

            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Qta_Dettaglio1) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Qta_Dettaglio2) & " ")

            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Qualifica_Cod) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Tariffa_Cod) & " ")

            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Sconto_Listino) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Sconto_Modalita) & " ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(Sconto_Testo) & "' ")


            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Mezzo_Det) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Ric_cod_Pat) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Cod_Conto_Pat) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Iva_Indetraibile) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Iva_Indetraibile_Perc) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Iva_Deto_Cod) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Dettagli_Blocco_Flag) & " ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(Dettagli_Blocco_Username) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Dettagli_Blocco_Data) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Ordine_Det) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Deroga_Cod) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Prezzo_Livello) & " ")

            strSql.AppendLine("         , '" & Agro_SQL_SaveText(Rif_Esterno) & "' ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(Rif_Esterno_2) & "' ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(PrincipiAttiviPercAbb) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Polverulento) & " ")

            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Dettaglio_IdCod) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Dettaglio_GenCod) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Dettaglio_SpeCod) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Dettaglio_IProCod) & " ")

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

#End Region

#Region "Modifica"

    Public Function ModificaPuntuale(ByVal Piva As String,
                                     ByVal Sa_Cod As Integer,
                                     ByVal Id_Agenda As Integer,
                                     ByVal Id_Mov As Integer,
                                     ByVal Id_Mov_Det As Integer,
                                     ByRef objParametri As AgronicaCoreParametri,
                                     Optional ByVal Elem_Cod As Integer? = Nothing,
                                     Optional ByVal Pro_Cod As Integer? = Nothing,
                                     Optional ByVal Mat_Cod As Integer? = Nothing,
                                     Optional ByVal Mov_Det_Des As String = Nothing,
                                     Optional ByVal Udm_Cod As Integer? = Nothing,
                                     Optional ByVal Qta As Decimal? = Nothing,
                                     Optional ByVal Cod_Iva As Integer? = Nothing,
                                     Optional ByVal Sconto As Decimal? = Nothing,
                                     Optional ByVal Prezzo_Unitario As Decimal? = Nothing,
                                     Optional ByVal Cod_Conto As Integer? = Nothing,
                                     Optional ByVal Cod_Progetto As Integer? = Nothing,
                                     Optional ByVal Fase_Cod As Integer? = Nothing,
                                     Optional ByVal Contabilizzato As Integer? = Nothing,
                                     Optional ByVal Pendente As Integer? = Nothing,
                                     Optional ByVal Validita_Inizio As Date? = Nothing,
                                     Optional ByVal Validita_Fine As Date? = Nothing,
                                     Optional ByVal Cal_Cod As Integer? = Nothing,
                                     Optional ByVal Extra_Str As String = Nothing,
                                     Optional ByVal Extra_Int As Integer? = Nothing,
                                     Optional ByVal Extra_Date As DateTime? = Nothing,
                                     Optional ByVal Anno As Integer? = Nothing,
                                     Optional ByVal Ric_Cod As Integer? = Nothing,
                                     Optional ByVal Imponibile As Decimal? = Nothing,
                                     Optional ByVal Iva As Decimal? = Nothing,
                                     Optional ByVal Lotto As String = Nothing,
                                     Optional ByVal Jolly_Int As Integer? = Nothing,
                                     Optional ByVal Imponibile_Netto As Decimal? = Nothing,
                                     Optional ByVal Prezzo_Unitario_Netto As Decimal? = Nothing,
                                     Optional ByVal Udm_Cod_Extra As Integer? = Nothing,
                                     Optional ByVal Qta_Extra As Decimal? = Nothing,
                                     Optional ByVal Prezzo_Effettivo As Decimal? = Nothing,
                                     Optional ByVal ChkIva_Manuale As Integer? = Nothing,
                                     Optional ByVal Cod_IvaIndetraibile As Integer? = Nothing,
                                     Optional ByVal Qta_Extra_Totale As Decimal? = Nothing,
                                     Optional ByVal Tara As Decimal? = Nothing,
                                     Optional ByVal ChkLayOut_Hide As Integer? = Nothing,
                                     Optional ByVal Variazione As Decimal? = Nothing,
                                     Optional ByVal Listino_Cod As Integer? = Nothing,
                                     Optional ByVal Sconto_Listino As Decimal? = Nothing,
                                     Optional ByVal Sconto_Modalita As Integer? = Nothing,
                                     Optional ByVal Mat_Cod_Alias As Integer? = Nothing,
                                     Optional ByVal Mezzo_Det As Integer? = Nothing,
                                     Optional ByVal Sconto_Testo As String = Nothing,
                                     Optional ByVal Ric_Cod_Pat As Integer? = Nothing,
                                     Optional ByVal Cod_Conto_Pat As Integer? = Nothing,
                                     Optional ByVal TempoCarenza As Integer? = Nothing,
                                     Optional ByVal DoseEtichetta As String = Nothing,
                                     Optional ByVal Turno_Cod As Integer? = Nothing,
                                     Optional ByVal Id_Attivita As Integer? = Nothing,
                                     Optional ByVal Dettaglio_VegCod As Integer? = Nothing,
                                     Optional ByVal Iva_Indetraibile As Decimal? = Nothing,
                                     Optional ByVal Iva_Indetraibile_Perc As Decimal? = Nothing,
                                     Optional ByVal PrincipiAttivi As String = Nothing,
                                     Optional ByVal ClassiTossicologiche As String = Nothing,
                                     Optional ByVal DoseEtichetta_Value As String = Nothing,
                                     Optional ByVal Iva_Deto_Cod As Integer? = Nothing,
                                     Optional ByVal Qta_Dettaglio1 As Decimal? = Nothing,
                                     Optional ByVal Qta_Dettaglio2 As Decimal? = Nothing,
                                     Optional ByVal Dettagli_Blocco_Flag As Integer? = Nothing,
                                     Optional ByVal Dettagli_Blocco_Username As String = Nothing,
                                     Optional ByVal Dettagli_Blocco_Data As DateTime? = Nothing,
                                     Optional ByVal Qualifica_Cod As Integer? = Nothing,
                                     Optional ByVal Tariffa_Cod As Integer? = Nothing,
                                     Optional ByVal Ordine_Det As Integer? = Nothing,
                                     Optional ByVal Deroga_Cod As Integer? = Nothing,
                                     Optional ByVal Prezzo_Livello As Integer? = Nothing,
                                     Optional ByVal Data_Modifica As DateTime = #2/1/1900#,
                                     Optional ByVal Username_Modifica As String = "",
                                     Optional ByVal PrincipiAttiviPesi As String = Nothing,
                                     Optional ByVal Buffer As String = Nothing,
                                     Optional ByVal Rif_Esterno As String = Nothing,
                                     Optional ByVal Rif_Esterno_2 As String = Nothing,
                                     Optional ByVal xFiltroAggiuntivo As String = "",
                                     Optional ByVal New_Sa_Cod As Integer? = Nothing,
                                     Optional ByVal PrincipiAttiviPercAbb As String = Nothing,
                                     Optional ByVal Polverulento As Integer? = Nothing,
                                     Optional ByVal Dettaglio_IdCod As Integer? = Nothing,
                                     Optional ByVal Dettaglio_GenCod As Integer? = Nothing,
                                     Optional ByVal Dettaglio_SpeCod As Integer? = Nothing,
                                     Optional ByVal Dettaglio_IProCod As Integer? = Nothing
                                     ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Movimenti_Dettagli_W.ModificaPuntuale()"

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


            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE Movimenti_Dettagli ")
            strSql.AppendLine(" SET Data_Modifica = " & Agro_SQL_SaveDateTime(Data_Modifica) & " ")
            strSql.AppendLine("   , Username_Modifica = '" & Agro_SQL_SaveText(Username_Modifica) & "' ")


            If Not IsNothing(Elem_Cod) Then
                strSql.AppendLine("   , Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & " ")
            End If

            If Not IsNothing(Pro_Cod) Then
                strSql.AppendLine("   , Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & " ")
            End If

            If Not IsNothing(Mat_Cod) Then
                strSql.AppendLine("   , Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & " ")
            End If

            If Not IsNothing(Mov_Det_Des) Then
                strSql.AppendLine("   , Mov_Det_Des = '" & Agro_SQL_SaveText(Mov_Det_Des) & "' ")
            End If

            If Not IsNothing(Udm_Cod) Then
                strSql.AppendLine("   , Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & " ")
            End If

            If Not IsNothing(Qta) Then
                strSql.AppendLine("   , Qta = " & Agro_SQL_SaveNum(Qta) & " ")
            End If

            If Not IsNothing(Cod_Iva) Then
                strSql.AppendLine("   , Cod_Iva = " & Agro_SQL_SaveNum(Cod_Iva) & " ")
            End If

            If Not IsNothing(Sconto) Then
                strSql.AppendLine("   , Sconto = " & Agro_SQL_SaveNum(Sconto) & " ")
            End If

            If Not IsNothing(Prezzo_Unitario) Then
                strSql.AppendLine("   , Prezzo_Unitario = " & Agro_SQL_SaveNum(Prezzo_Unitario) & " ")
            End If

            If Not IsNothing(Cod_Conto) Then
                strSql.AppendLine("   , Cod_Conto = " & Agro_SQL_SaveNum(Cod_Conto) & " ")
            End If

            If Not IsNothing(Cod_Progetto) Then
                strSql.AppendLine("   , Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & " ")
            End If

            If Not IsNothing(Fase_Cod) Then
                strSql.AppendLine("   , Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & " ")
            End If

            If Not IsNothing(Contabilizzato) Then
                strSql.AppendLine("   , Contabilizzato = " & Agro_SQL_SaveNum(Contabilizzato) & " ")
            End If

            If Not IsNothing(Pendente) Then
                strSql.AppendLine("   , Pendente = " & Agro_SQL_SaveNum(Pendente) & " ")
            End If

            If Not IsNothing(Validita_Inizio) Then
                strSql.AppendLine("   , Validita_Inizio = " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            End If

            If Not IsNothing(Validita_Fine) Then
                strSql.AppendLine("   , Validita_Fine = " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            End If

            If Not IsNothing(Cal_Cod) Then
                strSql.AppendLine("   , Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & " ")
            End If

            If Not IsNothing(Extra_Str) Then
                strSql.AppendLine("   , Extra_Str = '" & Agro_SQL_SaveText(Extra_Str) & "' ")
            End If

            If Not IsNothing(Extra_Int) Then
                strSql.AppendLine("   , Extra_Int = " & Agro_SQL_SaveNum(Extra_Int) & " ")
            End If

            If Not IsNothing(Extra_Date) Then
                strSql.AppendLine("   , Extra_Date = " & Agro_SQL_SaveDateTime(Extra_Date) & " ")
            End If

            If Not IsNothing(Anno) Then
                strSql.AppendLine("   , Anno = " & Agro_SQL_SaveNum(Anno) & " ")
            End If

            If Not IsNothing(Ric_Cod) Then
                strSql.AppendLine("   , Ric_Cod = " & Agro_SQL_SaveNum(Ric_Cod) & " ")
            End If

            If Not IsNothing(Imponibile) Then
                strSql.AppendLine("   , Imponibile = " & Agro_SQL_SaveNum(Imponibile) & " ")
            End If

            If Not IsNothing(Iva) Then
                strSql.AppendLine("   , Iva = " & Agro_SQL_SaveNum(Iva) & " ")
            End If

            If Not IsNothing(Lotto) Then
                strSql.AppendLine("   , Lotto = '" & Agro_SQL_SaveText(Lotto) & "' ")
            End If

            If Not IsNothing(Jolly_Int) Then
                strSql.AppendLine("   , Jolly_Int = " & Agro_SQL_SaveNum(Jolly_Int) & " ")
            End If

            If Not IsNothing(Imponibile_Netto) Then
                strSql.AppendLine("   , Imponibile_Netto = " & Agro_SQL_SaveNum(Imponibile_Netto) & " ")
            End If

            If Not IsNothing(Prezzo_Unitario_Netto) Then
                strSql.AppendLine("   , Prezzo_Unitario_Netto = " & Agro_SQL_SaveNum(Prezzo_Unitario_Netto) & " ")
            End If

            If Not IsNothing(Udm_Cod_Extra) Then
                strSql.AppendLine("   , Udm_Cod_Extra = " & Agro_SQL_SaveNum(Udm_Cod_Extra) & " ")
            End If

            If Not IsNothing(Qta_Extra) Then
                strSql.AppendLine("   , Qta_Extra = " & Agro_SQL_SaveNum(Qta_Extra) & " ")
            End If

            If Not IsNothing(Prezzo_Effettivo) Then
                strSql.AppendLine("   , Prezzo_Effettivo = " & Agro_SQL_SaveNum(Prezzo_Effettivo) & " ")
            End If

            If Not IsNothing(ChkIva_Manuale) Then
                strSql.AppendLine("   , ChkIva_Manuale = " & Agro_SQL_SaveNum(ChkIva_Manuale) & " ")
            End If

            If Not IsNothing(Cod_IvaIndetraibile) Then
                strSql.AppendLine("   , Cod_IvaIndetraibile = " & Agro_SQL_SaveNum(Cod_IvaIndetraibile) & " ")
            End If

            If Not IsNothing(Qta_Extra_Totale) Then
                strSql.AppendLine("   , Qta_Extra_Totale = " & Agro_SQL_SaveNum(Qta_Extra_Totale) & " ")
            End If

            If Not IsNothing(Tara) Then
                strSql.AppendLine("   , Tara = " & Agro_SQL_SaveNum(Tara) & " ")
            End If

            If Not IsNothing(ChkLayOut_Hide) Then
                strSql.AppendLine("   , ChkLayOut_Hide = " & Agro_SQL_SaveNum(ChkLayOut_Hide) & " ")
            End If

            If Not IsNothing(Variazione) Then
                strSql.AppendLine("   , Variazione = " & Agro_SQL_SaveNum(Variazione) & " ")
            End If

            If Not IsNothing(Listino_Cod) Then
                strSql.AppendLine("   , Listino_Cod = " & Agro_SQL_SaveNum(Listino_Cod) & " ")
            End If

            If Not IsNothing(Sconto_Listino) Then
                strSql.AppendLine("   , Sconto_Listino = " & Agro_SQL_SaveNum(Sconto_Listino) & " ")
            End If

            If Not IsNothing(Sconto_Modalita) Then
                strSql.AppendLine("   , Sconto_Modalita = " & Agro_SQL_SaveNum(Sconto_Modalita) & " ")
            End If

            If Not IsNothing(Mat_Cod_Alias) Then
                strSql.AppendLine("   , Mat_Cod_Alias = " & Agro_SQL_SaveNum(Mat_Cod_Alias) & " ")
            End If

            If Not IsNothing(Mezzo_Det) Then
                strSql.AppendLine("   , Mezzo_Det = " & Agro_SQL_SaveNum(Mezzo_Det) & " ")
            End If

            If Not IsNothing(Sconto_Testo) Then
                strSql.AppendLine("   , Sconto_Testo = '" & Agro_SQL_SaveText(Sconto_Testo) & "' ")
            End If

            If Not IsNothing(Ric_Cod_Pat) Then
                strSql.AppendLine("   , Ric_Cod_Pat = " & Agro_SQL_SaveNum(Ric_Cod_Pat) & " ")
            End If

            If Not IsNothing(Cod_Conto_Pat) Then
                strSql.AppendLine("   , Cod_Conto_Pat = " & Agro_SQL_SaveNum(Cod_Conto_Pat) & " ")
            End If

            If Not IsNothing(TempoCarenza) Then
                strSql.AppendLine("   , TempoCarenza = " & Agro_SQL_SaveNum(TempoCarenza) & " ")
            End If

            If Not IsNothing(DoseEtichetta) Then
                strSql.AppendLine("   , DoseEtichetta = '" & Agro_SQL_SaveText(DoseEtichetta) & "' ")
            End If

            If Not IsNothing(Turno_Cod) Then
                strSql.AppendLine("   , Turno_Cod = " & Agro_SQL_SaveNum(Turno_Cod) & " ")
            End If

            If Not IsNothing(Id_Attivita) Then
                strSql.AppendLine("   , Id_Attivita = " & Agro_SQL_SaveNum(Id_Attivita) & " ")
            End If

            If Not IsNothing(Dettaglio_VegCod) Then
                strSql.AppendLine("   , Dettaglio_VegCod = " & Agro_SQL_SaveNum(Dettaglio_VegCod) & " ")
            End If

            If Not IsNothing(Iva_Indetraibile) Then
                strSql.AppendLine("   , Iva_Indetraibile = " & Agro_SQL_SaveNum(Iva_Indetraibile) & " ")
            End If

            If Not IsNothing(Iva_Indetraibile_Perc) Then
                strSql.AppendLine("   , Iva_Indetraibile_Perc = " & Agro_SQL_SaveNum(Iva_Indetraibile_Perc) & " ")
            End If

            If Not IsNothing(PrincipiAttivi) Then
                strSql.AppendLine("   , PrincipiAttivi = '" & Agro_SQL_SaveText(PrincipiAttivi) & "' ")
            End If

            If Not IsNothing(ClassiTossicologiche) Then
                strSql.AppendLine("   , ClassiTossicologiche = '" & Agro_SQL_SaveText(ClassiTossicologiche) & "' ")
            End If

            If Not IsNothing(DoseEtichetta_Value) Then
                strSql.AppendLine("   , DoseEtichetta_Value = '" & Agro_SQL_SaveText(DoseEtichetta_Value) & "' ")
            End If

            If Not IsNothing(Iva_Deto_Cod) Then
                strSql.AppendLine("   , Iva_Deto_Cod = " & Agro_SQL_SaveNum(Iva_Deto_Cod) & " ")
            End If

            If Not IsNothing(Qta_Dettaglio1) Then
                strSql.AppendLine("   , Qta_Dettaglio1 = " & Agro_SQL_SaveNum(Qta_Dettaglio1) & " ")
            End If

            If Not IsNothing(Qta_Dettaglio2) Then
                strSql.AppendLine("   , Qta_Dettaglio2 = " & Agro_SQL_SaveNum(Qta_Dettaglio2) & " ")
            End If

            If Not IsNothing(Dettagli_Blocco_Flag) Then
                strSql.AppendLine("   , Dettagli_Blocco_Flag = " & Agro_SQL_SaveNum(Dettagli_Blocco_Flag) & " ")
            End If

            If Not IsNothing(Dettagli_Blocco_Username) Then
                strSql.AppendLine("   , Dettagli_Blocco_Username = '" & Agro_SQL_SaveText(Dettagli_Blocco_Username) & "' ")
            End If

            If Not IsNothing(Dettagli_Blocco_Data) Then
                strSql.AppendLine("   , Dettagli_Blocco_Data = " & Agro_SQL_SaveDateTime(Dettagli_Blocco_Data) & " ")
            End If

            If Not IsNothing(Qualifica_Cod) Then
                strSql.AppendLine("   , Qualifica_Cod = " & Agro_SQL_SaveNum(Qualifica_Cod) & " ")
            End If

            If Not IsNothing(Tariffa_Cod) Then
                strSql.AppendLine("   , Tariffa_Cod = " & Agro_SQL_SaveNum(Tariffa_Cod) & " ")
            End If

            If Not IsNothing(Ordine_Det) Then
                strSql.AppendLine("   , Ordine_Det = " & Agro_SQL_SaveNum(Ordine_Det) & " ")
            End If

            If Not IsNothing(Deroga_Cod) Then
                strSql.AppendLine("   , Deroga_Cod = " & Agro_SQL_SaveNum(Deroga_Cod) & " ")
            End If

            If Not IsNothing(Prezzo_Livello) Then
                strSql.AppendLine("   , Prezzo_Livello = " & Agro_SQL_SaveNum(Prezzo_Livello) & " ")
            End If

            If Not IsNothing(PrincipiAttiviPesi) Then
                strSql.AppendLine("   , PrincipiAttiviPesi = '" & Agro_SQL_SaveText(PrincipiAttiviPesi) & "' ")
            End If

            If Not IsNothing(Buffer) Then
                strSql.AppendLine("   , Buffer = '" & Agro_SQL_SaveText(Buffer) & "' ")
            End If

            If Not IsNothing(Rif_Esterno) Then
                strSql.AppendLine("   , Rif_Esterno = '" & Agro_SQL_SaveText(Rif_Esterno) & "' ")
            End If

            If Not IsNothing(Rif_Esterno_2) Then
                strSql.AppendLine("   , Rif_Esterno_2 = '" & Agro_SQL_SaveText(Rif_Esterno_2) & "' ")
            End If

            If Not IsNothing(New_Sa_Cod) Then
                strSql.AppendLine("   , Sa_Cod = " & Agro_SQL_SaveNum(New_Sa_Cod) & " ")
            End If

            If Not IsNothing(PrincipiAttiviPercAbb) Then
                strSql.AppendLine("   , PrincipiAttiviPercAbb = '" & Agro_SQL_SaveText(PrincipiAttiviPercAbb) & "' ")
            End If

            If Not IsNothing(Polverulento) Then
                strSql.AppendLine("   , Polverulento = " & Agro_SQL_SaveNum(Polverulento) & " ")
            End If

            If Not IsNothing(Dettaglio_IdCod) Then
                strSql.AppendLine("   , Dettaglio_IdCod = " & Agro_SQL_SaveNum(Dettaglio_IdCod) & " ")
            End If

            If Not IsNothing(Dettaglio_GenCod) Then
                strSql.AppendLine("   , Dettaglio_GenCod = " & Agro_SQL_SaveNum(Dettaglio_GenCod) & " ")
            End If

            If Not IsNothing(Dettaglio_SpeCod) Then
                strSql.AppendLine("   , Dettaglio_SpeCod = " & Agro_SQL_SaveNum(Dettaglio_SpeCod) & " ")
            End If

            If Not IsNothing(Dettaglio_IProCod) Then
                strSql.AppendLine("   , Dettaglio_IProCod = " & Agro_SQL_SaveNum(Dettaglio_IProCod) & " ")
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

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
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

    <Obsolete("Usare la funzione ModificaPuntuale")>
    Public Function Modifica(ByVal Piva As String,
                             ByVal Sa_Cod As Integer,
                             ByVal Id_Agenda As Integer,
                             ByVal Id_Mov As Integer,
                             ByVal Id_Mov_Det As Integer,
                             ByVal Elem_Cod As Integer,
                             ByVal Pro_Cod As Integer,
                             ByVal Mat_Cod As Integer,
                             ByVal Mov_Det_Des As String,
                             ByVal Qta As Decimal,
                             ByVal Udm_Cod As Integer,
                             ByVal Cod_Iva As Integer,
                             ByVal Jolly_Int As Integer,
                             ByVal Sconto As Decimal,
                             ByVal Prezzo_Unitario As Decimal,
                             ByVal Prezzo_Unitario_Netto As Decimal,
                             ByVal Cal_Cod As Integer,
                             ByVal Cod_Conto As Integer,
                             ByVal Cod_Progetto As Integer,
                             ByVal Fase_Cod As Integer,
                             ByVal Extra_Str As String,
                             ByVal Extra_Int As Integer,
                             ByVal Extra_Date As Date,
                             ByVal Ric_Cod As Integer,
                             ByVal Anno As Integer,
                             ByVal Imponibile As Decimal,
                             ByVal Imponibile_Netto As Decimal,
                             ByVal Iva As Decimal,
                             ByVal Contabilizzato As Integer,
                             ByVal Pendente As Integer,
                             ByVal Lotto As String,
                             ByVal Udm_Cod_Extra As Integer,
                             ByVal Qta_Extra As Decimal,
                             ByVal Qta_Extra_Totale As Decimal,
                             ByVal Prezzo_Effettivo As Decimal,
                             ByVal Tara As Decimal,
                             ByVal ChkLayOut_Hide As Integer,
                             ByVal ChkIva_Manuale As Integer,
                             ByVal Cod_IvaIndetraibile As Integer,
                             ByVal TempoCarenza As Integer,
                             ByVal DoseEtichetta As String,
                             ByVal Turno_Cod As Integer,
                             ByVal ID_Attivita As Integer,
                             ByVal Dettaglio_VegCod As Integer,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Movimenti_Dettagli_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Sa_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            End If

            If Id_Agenda = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Agenda obbligatorio)")
            End If

            If Id_Mov = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Mov obbligatorio)")
            End If

            If Id_Mov_Det = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Mov_Det obbligatorio)")
            End If


            '---------------------------------------------
            strSql.Length = 0
            strSql.AppendLine(" UPDATE Movimenti_Dettagli SET ")
            strSql.AppendLine("    Elem_Cod            = " & Agro_SQL_SaveNum(Elem_Cod) & "  ")
            strSql.AppendLine("   ,Pro_Cod             = " & Agro_SQL_SaveNum(Pro_Cod) & "  ")
            strSql.AppendLine("   ,Mat_Cod             = " & Agro_SQL_SaveNum(Mat_Cod) & "  ")
            strSql.AppendLine("   ,Mov_Det_Des         = '" & Agro_SQL_SaveText(Mov_Det_Des) & "'  ")
            strSql.AppendLine("   ,Qta                 = " & Agro_SQL_SaveNum(Qta) & "  ")
            strSql.AppendLine("   ,Udm_Cod             = " & Agro_SQL_SaveNum(Udm_Cod) & "  ")
            strSql.AppendLine("   ,Cod_Iva             = " & Agro_SQL_SaveNum(Cod_Iva) & "  ")
            strSql.AppendLine("   ,Jolly_Int           = " & Agro_SQL_SaveNum(Jolly_Int) & "  ")
            strSql.AppendLine("   ,Sconto              = " & Agro_SQL_SaveNum(Sconto) & "  ")
            strSql.AppendLine("   ,Prezzo_Unitario     = " & Agro_SQL_SaveNum(Prezzo_Unitario) & "  ")
            strSql.AppendLine("   ,Cod_Conto           = " & Agro_SQL_SaveNum(Cod_Conto) & "  ")
            strSql.AppendLine("   ,Cod_Progetto        = " & Agro_SQL_SaveNum(Cod_Progetto) & "  ")
            strSql.AppendLine("   ,Fase_Cod            = " & Agro_SQL_SaveNum(Fase_Cod) & "  ")
            strSql.AppendLine("   ,Cal_Cod             = " & Agro_SQL_SaveNum(Cal_Cod) & "  ")
            strSql.AppendLine("   ,Extra_Str           = '" & Agro_SQL_SaveText(Extra_Str) & "'  ")
            strSql.AppendLine("   ,Extra_Int           = " & Agro_SQL_SaveNum(Extra_Int) & "  ")
            strSql.AppendLine("   ,Extra_Date          = " & Agro_SQL_SaveDate(Extra_Date) & "  ")
            strSql.AppendLine("   ,Ric_Cod             = " & Agro_SQL_SaveNum(Ric_Cod) & "  ")
            strSql.AppendLine("   ,Anno                = " & Agro_SQL_SaveNum(Anno) & "  ")
            strSql.AppendLine("   ,Imponibile          = " & Agro_SQL_SaveNum(Imponibile) & "  ")
            strSql.AppendLine("   ,Iva                 = " & Agro_SQL_SaveNum(Iva) & "  ")
            strSql.AppendLine("   ,Contabilizzato      = " & Agro_SQL_SaveNum(Contabilizzato) & "  ")
            strSql.AppendLine("   ,Pendente            = " & Agro_SQL_SaveNum(Pendente) & "  ")
            strSql.AppendLine("   ,Lotto               = '" & Agro_SQL_SaveText(Lotto) & "'  ")
            strSql.AppendLine("   ,Prezzo_Unitario_Netto  = " & Agro_SQL_SaveNum(Prezzo_Unitario_Netto) & "  ")
            strSql.AppendLine("   ,Prezzo_Effettivo       = " & Agro_SQL_SaveNum(Prezzo_Effettivo) & "  ")
            strSql.AppendLine("   ,Imponibile_Netto       = " & Agro_SQL_SaveNum(Imponibile_Netto) & "  ")
            strSql.AppendLine("   ,Tara                   = " & Agro_SQL_SaveNum(Tara) & "  ")
            strSql.AppendLine("   ,ChkLayOut_Hide         = " & Agro_SQL_SaveNum(ChkLayOut_Hide) & "  ")
            strSql.AppendLine("   ,ChkIva_Manuale         = " & Agro_SQL_SaveNum(ChkIva_Manuale) & "  ")
            strSql.AppendLine("   ,Cod_IvaIndetraibile    = " & Agro_SQL_SaveNum(Cod_IvaIndetraibile) & "  ")
            strSql.AppendLine("   ,Udm_Cod_Extra          = " & Agro_SQL_SaveNum(Udm_Cod_Extra) & "  ")
            strSql.AppendLine("   ,Qta_Extra              = " & Agro_SQL_SaveNum(Qta_Extra) & "  ")
            strSql.AppendLine("   ,Qta_Extra_Totale       = " & Agro_SQL_SaveNum(Qta_Extra_Totale) & "  ")

            strSql.AppendLine("   ,TempoCarenza         = " & Agro_SQL_SaveNum(TempoCarenza) & "  ")
            strSql.AppendLine("   ,DoseEtichetta        = '" & Agro_SQL_SaveText(DoseEtichetta) & "'  ")

            strSql.AppendLine("   ,Turno_Cod            = " & Agro_SQL_SaveNum(Turno_Cod) & "  ")
            strSql.AppendLine("   ,ID_Attivita          = " & Agro_SQL_SaveNum(ID_Attivita) & "  ")
            strSql.AppendLine("   ,Dettaglio_VegCod     = " & Agro_SQL_SaveNum(Dettaglio_VegCod) & "  ")

            strSql.AppendLine("   ,Inviato           =  0 ")
            strSql.AppendLine("   ,DataInvio         =  Null ")
            strSql.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now))
            strSql.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            strSql.AppendLine("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.AppendLine("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            strSql.AppendLine(" WHERE Piva      = '" & Agro_SQL_SaveText(Piva) & "'  ")
            strSql.AppendLine(" AND   Sa_Cod    =  " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            strSql.AppendLine(" AND   Id_Agenda =  " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
            strSql.AppendLine(" AND   Id_Mov =  " & Agro_SQL_SaveNum(Id_Mov) & "   ")
            strSql.AppendLine(" AND   Id_Mov_Det =  " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")


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

    Public Function Modifica_x_Trasferimento(ByVal sa_cod As Integer,
                                             ByVal Str_id_agenda_id_mov_id_mov_det As String,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Movimenti_Dettagli_W.Modifica_x_Trasferimento()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Str_id_agenda_id_mov_id_mov_det = "" Then
                Throw New Exception("Parametro non corretto nella query (Str_id_agenda_id_mov_id_mov_det obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE movimenti_dettagli SET ")
            strSql.AppendLine("    sa_cod    = " & Agro_SQL_SaveNum(sa_cod) & "  ")


            strSql.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now))
            strSql.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            strSql.AppendLine(" WHERE    (" & Str_id_agenda_id_mov_id_mov_det & ")")

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

    Public Function Movimenti_Dettagli_Blocca(ByVal Piva As String,
                                              ByVal Sa_Cod As Integer,
                                              ByVal Id_Agenda As Integer,
                                              ByVal id_mov As Integer,
                                              ByVal id_mov_det As Integer,
                                              ByVal Blocco_Username As String,
                                              ByVal Blocco_Data As Date,
                                              ByVal xFiltroAggiuntivo As String,
                                              ByRef objParametri As AgronicaCoreParametri
                                              ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Agenda_W.Movimenti_Dettagli_Blocca()"

        '====================================================================================
        'Parametri opzionali :
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            'Non va bene, perché nelle operazioni contabili il sa_cod deve essere =0
            'If Sa_Cod = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            'End If

            If Id_Agenda = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Agenda obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE Movimenti_Dettagli SET ")
            strSql.AppendLine("     Dettagli_Blocco_Flag         =  1 ")
            strSql.AppendLine("    ,Dettagli_Blocco_Username     = '" & Agro_SQL_SaveText(Blocco_Username) & "'  ")
            strSql.AppendLine("    ,Dettagli_Blocco_Data         =  " & Agro_SQL_SaveDate(Blocco_Data))

            strSql.AppendLine(" WHERE   PIVA        = '" & Agro_SQL_SaveText(Trim(Piva)) & "'  ")
            'StrSQL.AppendLine(" AND     Sa_Cod      = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            strSql.AppendLine(" AND     Id_Agenda   = " & Agro_SQL_SaveNum(Id_Agenda) & "  ")
            strSql.AppendLine(" AND     Id_mov   = " & Agro_SQL_SaveNum(id_mov) & "  ")
            strSql.AppendLine(" AND     Id_mov_det   = " & Agro_SQL_SaveNum(id_mov_det) & "  ")


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

    Public Function Modifica_Prezzo_Unitario(ByVal Id_Agenda As Long,
                                             ByVal Id_Mov As Long,
                                             ByVal Id_Mov_Det As Long,
                                             ByVal Elem_Cod As Long,
                                             ByVal Pro_Cod As Long,
                                             ByVal Mat_Cod As Long,
                                             ByVal Udm_Cod As Long,
                                             ByVal Prezzo_Unitario As Decimal,
                                             ByVal Cal_Cod As Long,
                                             ByVal Cod_Progetto As Long,
                                             ByVal Fase_Cod As Long,
                                             ByVal Lotto As String,
                                             ByVal xFiltroAggiuntivo As String,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Movimenti_Dettagli_W.Modifica_Prezzo_Unitario()"

        '====================================================================================
        'Parametri opzionali :
        '   Id_Mov_Det = 0 
        '   Elem_Cod = 0 
        '   Pro_Cod = 0 
        '   Mat_Cod = 0 
        '   Udm_Cod = 0 
        '   Cod_Progetto = 0 
        '   Fase_Cod = 0 
        '   Cal_Cod = 0 
        '   Lotto = "" 
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            strSql.Length = 0
            strSql.AppendLine(" UPDATE Movimenti_Dettagli SET ")
            strSql.AppendLine("    Prezzo_Unitario     = " & Agro_SQL_SaveNum(Prezzo_Unitario) & "  ")

            strSql.AppendLine(" WHERE Id_Agenda       =  " & Agro_SQL_SaveNum(Id_Agenda) & "  ")
            strSql.AppendLine("    AND   Id_Mov       =  " & Agro_SQL_SaveNum(Id_Mov) & "  ")


            If Id_Mov_Det <> 0 Then
                strSql.AppendLine(" AND Movimenti_Dettagli.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")
            End If

            If Elem_Cod <> 0 Then
                strSql.AppendLine(" AND Movimenti_Dettagli.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
            End If

            If Pro_Cod <> 0 Then
                strSql.AppendLine(" AND Movimenti_Dettagli.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
            End If

            If Mat_Cod <> 0 Then
                strSql.AppendLine(" AND Movimenti_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
            End If

            If Udm_Cod <> 0 Then
                strSql.AppendLine(" AND Movimenti_Dettagli.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   ")
            End If

            If Cod_Progetto <> 0 Then
                strSql.AppendLine(" AND Movimenti_Dettagli.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & "   ")
            End If

            If Fase_Cod <> 0 Then
                strSql.AppendLine(" AND Movimenti_Dettagli.Fase_Cod = " & Agro_SQL_SaveNum(Cod_Progetto) & "   ")
            End If

            If Cal_Cod <> 0 Then
                strSql.AppendLine(" AND Movimenti_Dettagli.Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "   ")
            End If

            If Lotto <> "" Then
                strSql.AppendLine(" AND Movimenti_Dettagli.Lotto = '" & Agro_SQL_SaveText(Lotto) & "'   ")
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

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Spostato dai COM perché usato dalla pagina di verifica delle giacenze.
    ''' </summary>
    ''' <history>
    ''' 	[pierantoni]	20/04/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function Modifica_Quantita(ByVal Piva As String,
                                      ByVal Id_Agenda As Integer,
                                      ByVal Id_Mov As Integer,
                                      ByVal Id_Mov_Det As Integer,
                                      ByVal Elem_Cod As Integer,
                                      ByVal Pro_Cod As Integer,
                                      ByVal Mat_Cod As Integer,
                                      ByVal Udm_Cod As Integer,
                                      ByVal Qta As Decimal,
                                      ByVal Qta_Extra As Decimal,
                                      ByVal Qta_Extra_Totale As Decimal,
                                      ByVal Cal_Cod As Integer,
                                      ByVal Cod_Progetto As Integer,
                                      ByVal Fase_Cod As Integer,
                                      ByVal Lotto As String,
                                      ByVal xFiltroAggiuntivo As String,
                                      ByRef objParametri As AgronicaCoreParametri
                                      ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Movimenti_Dettagli_W.Modifica_Quantita()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            strSql.Length = 0
            strSql.AppendLine(" UPDATE Movimenti_Dettagli SET ")
            strSql.AppendLine("    Qta              = " & Agro_SQL_SaveNum(Qta) & "  ")
            strSql.AppendLine("   ,Qta_Extra        = " & Agro_SQL_SaveNum(Qta_Extra) & "  ")
            strSql.AppendLine("   ,Qta_Extra_Totale = " & Agro_SQL_SaveNum(Qta_Extra_Totale) & "  ")


            strSql.AppendLine("    WHERE Piva       =  '" & Agro_SQL_SaveText(Piva) & "'  ")
            strSql.AppendLine("    AND   Id_Agenda  =  " & Agro_SQL_SaveNum(Id_Agenda) & "  ")
            strSql.AppendLine("    AND   Id_Mov     =  " & Agro_SQL_SaveNum(Id_Mov) & "  ")


            If Id_Mov_Det <> 0 Then
                strSql.AppendLine(" AND Movimenti_Dettagli.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")
            End If

            If Elem_Cod <> 0 Then
                strSql.AppendLine(" AND Movimenti_Dettagli.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
            End If

            If Pro_Cod <> 0 Then
                strSql.AppendLine(" AND Movimenti_Dettagli.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
            End If

            If Mat_Cod <> 0 Then
                strSql.AppendLine(" AND Movimenti_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
            End If

            If Udm_Cod <> 0 Then
                strSql.AppendLine(" AND Movimenti_Dettagli.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   ")
            End If

            If Cod_Progetto <> 0 Then
                strSql.AppendLine(" AND Movimenti_Dettagli.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & "   ")
            End If

            If Fase_Cod <> 0 Then
                strSql.AppendLine(" AND Movimenti_Dettagli.Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   ")
            End If

            If Cal_Cod <> 0 Then
                strSql.AppendLine(" AND Movimenti_Dettagli.Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "   ")
            End If

            If Lotto <> "" Then
                strSql.AppendLine(" AND Upper(Movimenti_Dettagli.Lotto) = '" & UCase(Agro_SQL_SaveText(Lotto)) & "'   ")
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

    Public Function Modifica_Carenza(ByVal Piva As String,
                                     ByVal Sa_Cod As Integer,
                                     ByVal Id_Agenda As Integer,
                                     ByVal Id_Mov As Integer,
                                     ByVal Id_Mov_Det As Integer,
                                     ByVal Elem_Cod As Integer,
                                     ByVal Pro_Cod As Integer,
                                     ByVal Mat_Cod As Integer,
                                     ByVal TempoCarenza As Integer,
                                     ByVal xFiltroAggiuntivo As String,
                                     ByRef objParametri As AgronicaCoreParametri
                                     ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Movimenti_Dettagli_W.Modifica_Quantita()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            strSql.Length = 0
            strSql.AppendLine(" UPDATE Movimenti_Dettagli SET ")
            strSql.AppendLine("    TempoCarenza              = " & Agro_SQL_SaveNum(TempoCarenza) & "  ")

            strSql.AppendLine("    WHERE Piva       =  '" & Agro_SQL_SaveText(Piva) & "'  ")
            strSql.AppendLine("    AND   Sa_Cod  =  " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            strSql.AppendLine("    AND   Id_Agenda  =  " & Agro_SQL_SaveNum(Id_Agenda) & "  ")
            strSql.AppendLine("    AND   Id_Mov     =  " & Agro_SQL_SaveNum(Id_Mov) & "  ")

            If Id_Mov_Det <> 0 Then
                strSql.AppendLine(" AND Movimenti_Dettagli.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")
            End If

            If Elem_Cod <> 0 Then
                strSql.AppendLine(" AND Movimenti_Dettagli.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
            End If

            If Pro_Cod <> 0 Then
                strSql.AppendLine(" AND Movimenti_Dettagli.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
            End If

            If Mat_Cod <> 0 Then
                strSql.AppendLine(" AND Movimenti_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
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

    Public Function Modifica_DatiAttivita(ByVal Piva As String,
                                          ByVal Sa_Cod As Integer,
                                          ByVal Id_Agenda As Integer,
                                          ByVal Id_Mov As Integer,
                                          ByVal Id_Mov_Det As Integer,
                                          ByVal Qta As Decimal,
                                          ByVal Turno_Cod As Integer,
                                          ByVal Id_Attivita As Integer,
                                          ByVal Dettaglio_VegCod As Integer,
                                          ByRef objParametri As AgronicaCoreParametri
                                          ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Movimenti_Dettagli_W.Modifica_DatiAttivita()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            strSql.Length = 0
            strSql.AppendLine(" UPDATE Movimenti_Dettagli SET ")
            strSql.AppendLine("    Qta              = " & Agro_SQL_SaveNum(Qta) & "  ")
            strSql.AppendLine("   ,Turno_Cod        = " & Agro_SQL_SaveNum(Turno_Cod) & "  ")
            strSql.AppendLine("   ,Id_Attivita      = " & Agro_SQL_SaveNum(Id_Attivita) & "  ")
            strSql.AppendLine("   ,Dettaglio_VegCod = " & Agro_SQL_SaveNum(Dettaglio_VegCod) & "  ")

            strSql.AppendLine("    WHERE Piva       =  '" & Agro_SQL_SaveText(Piva) & "'  ")
            strSql.AppendLine("    AND   Sa_Cod     =  " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            strSql.AppendLine("    AND   Id_Agenda  =  " & Agro_SQL_SaveNum(Id_Agenda) & "  ")
            strSql.AppendLine("    AND   Id_Mov     =  " & Agro_SQL_SaveNum(Id_Mov) & "  ")
            strSql.AppendLine("    AND   Id_Mov_Det =  " & Agro_SQL_SaveNum(Id_Mov_Det) & "  ")


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

    Public Function Modifica_Quantita_DaPercentuale(ByVal Piva As String,
                                                    ByVal Id_Agenda As Integer,
                                                    ByVal Id_Mov As Integer,
                                                    ByVal Id_Mov_Det As Integer,
                                                    ByVal Percentuale As Double,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByRef objParametri As AgronicaCoreParametri
                                                    ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Movimenti_Dettagli_W.Modifica_Quantita_DaPercentuale()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            strSql.Length = 0
            strSql.AppendLine(" UPDATE Movimenti_Dettagli SET ")
            strSql.AppendLine("    Qta                  = Qta * " & Agro_SQL_SaveNum(Percentuale) & "/100  ")
            strSql.AppendLine("   ,Qta_Extra            = Qta_Extra * " & Agro_SQL_SaveNum(Percentuale) & "/100  ")
            strSql.AppendLine("   ,Qta_Extra_Totale     = Qta_Extra_Totale * " & Agro_SQL_SaveNum(Percentuale) & "/100  ")


            strSql.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now))
            strSql.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            strSql.AppendLine(" WHERE Piva              = '" & Agro_SQL_SaveText(Piva) & "'  ")
            strSql.AppendLine(" AND   Id_Agenda         =  " & Agro_SQL_SaveNum(Id_Agenda) & "  ")

            If Id_Mov <> 0 Then
                strSql.AppendLine(" AND Movimenti_Dettagli.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
            End If
            If Id_Mov_Det <> 0 Then
                strSql.AppendLine(" AND Movimenti_Dettagli.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")
            End If

            'escludo i dettagli dei dosaggi perché sempre salvati /ha
            strSql.AppendLine(" AND Movimenti_Dettagli.Id_Mov IN (  ")
            strSql.AppendLine("     select id_mov from movimenti where cau_mov not in ('2050','2300','2100','2200')   ")
            strSql.AppendLine("                                  AND Piva = '" & Agro_SQL_SaveText(Piva) & "'  ")
            strSql.AppendLine("                                  AND Id_Agenda =  " & Agro_SQL_SaveNum(Id_Agenda) & "  ")
            strSql.AppendLine(" )   ")


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

    Public Function Modifica_PrincipiAttiviPesi_Da_DtFrCod(ByVal Dt As DataTable,
                                                           ByVal xFiltroAggiuntivo As String,
                                                           ByRef objParametri As AgronicaCoreParametri
                                                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Movimenti_Dettagli_W.Modifica_PrincipiAttiviPesi_Da_DtFrCod()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            strSql.Length = 0

            'strSql.AppendLine(" UPDATE Movimenti_Dettagli SET PrincipiAttiviPesi = CASE Pro_Cod ")

            For j = 0 To Dt.Rows.Count - 1

                'strSql.Length = 0
                strSql.AppendLine(" UPDATE Movimenti_Dettagli SET ")
                strSql.AppendLine("      PrincipiAttiviPesi = '" & Agro_SQL_SaveText(Dt.Rows(j).Item("Elenco_PrincipiAttiviPesi")) & "'  ")
                strSql.AppendLine("    , Data_Modifica = " & Agro_SQL_SaveDateTime(Now) & " ")
                strSql.AppendLine("    , Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")

                strSql.AppendLine("    WHERE Elem_Cod       =  " & Agro_SQL_SaveNum(FORMULATI) & " ")
                strSql.AppendLine("    AND   Pro_Cod  =  " & Agro_SQL_SaveNum(Dt.Rows(j).Item("Fr_Cod")) & "  ")
                strSql.AppendLine("    AND   PrincipiAttivi  <> '' ")


                'strSql.AppendLine(" WHEN " & Dt.Rows(j).Item("Fr_Cod") & " THEN '" & Dt.Rows(j).Item("Elenco_PrincipiAttiviPesi") & "'")
            Next

            'strSql.AppendLine(" End  WHERE Elem_Cod =  " & Agro_SQL_SaveNum(FORMULATI) & " ")
            'strSql.AppendLine("       AND   PrincipiAttivi  <> ''")


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

    Public Function Modifica_PrincipiAttiviPesi_Da_FrCod_OLD(ByVal Pro_Cod As Integer,
                                                             ByVal PrincipiAttiviPesi As String,
                                                             ByVal xFiltroAggiuntivo As String,
                                                             ByRef objParametri As AgronicaCoreParametri
                                                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Movimenti_Dettagli_W.Modifica_PrincipiAttiviPesi_Da_FrCod_OLD()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Pro_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Pro_Cod obbligatorio)")
            End If

            strSql.Length = 0
            strSql.AppendLine(" UPDATE Movimenti_Dettagli SET ")
            strSql.AppendLine("      PrincipiAttiviPesi              = '" & Agro_SQL_SaveText(PrincipiAttiviPesi) & "'  ")
            strSql.AppendLine("    , Data_Modifica = " & Agro_SQL_SaveDateTime(Now) & " ")
            strSql.AppendLine("    , Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")

            strSql.AppendLine("    WHERE Elem_Cod       =  " & Agro_SQL_SaveNum(FORMULATI) & " ")
            strSql.AppendLine("    AND   Pro_Cod  =  " & Agro_SQL_SaveNum(Pro_Cod) & "  ")
            strSql.AppendLine("    AND   PrincipiAttivi  <> '' ")

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

#End Region

#Region "Cancellazione"

    Public Function Cancella(ByVal Piva As String,
                             ByVal Sa_Cod As Integer,
                             ByVal Id_Agenda As Integer,
                             ByVal Id_Mov As Integer,
                             ByVal Id_Mov_Det As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Movimenti_Dettagli_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""
        '   Sa_Cod = 0
        '   Id_Agenda = 0
        '   Id_Mov = 0
        '   Id_Mov_Det = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Id_Agenda = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Agenda obbligatorio)")
            End If
            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.Length = 0
                strSql.AppendLine(" UPDATE Movimenti_Dettagli ")
                strSql.AppendLine(" SET ")
                strSql.AppendLine("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.AppendLine("      ,Inviato = -1 ")
                strSql.AppendLine(" WHERE  Inviato >= 0 ")

            Else
                strSql.Length = 0
                strSql.AppendLine(" DELETE ")
                strSql.AppendLine(" FROM Movimenti_Dettagli ")
                strSql.AppendLine(" WHERE  1=1 ")
            End If

            If Piva <> String.Empty Then
                strSql.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If


            strSql.AppendLine(" AND Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")


            If Id_Mov <> 0 Then
                strSql.AppendLine(" AND Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
            End If

            If Id_Mov_Det <> 0 Then
                strSql.AppendLine(" AND Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")
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

    Public Function Cancella_x_giacenze(ByVal Elem_Cod As Integer,
                                        ByVal Mat_Cod As String,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByRef objParametri As AgronicaCoreParametri
                                        ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Movimenti_Dettagli_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""
        '   Sa_Cod = 0
        '   Id_Agenda = 0
        '   Id_Mov = 0
        '   Id_Mov_Det = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Elem_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Agenda obbligatorio)")
            End If

            If Mat_Cod = "" Then
                Throw New Exception("Parametro non corretto nella query (Id_Agenda obbligatorio)")
            End If

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                strSql.Length = 0
                strSql.AppendLine(" UPDATE Movimenti_Dettagli ")
                strSql.AppendLine(" SET ")
                strSql.AppendLine("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.AppendLine("      ,Inviato = -1 ")
                strSql.AppendLine(" WHERE  Inviato >= 0 ")
            Else
                strSql.Length = 0
                strSql.AppendLine(" DELETE ")
                strSql.AppendLine(" FROM Movimenti_Dettagli ")
                strSql.AppendLine(" WHERE  1=1 ")
            End If


            strSql.AppendLine(" AND Sa_Cod = -1   ")
            strSql.AppendLine(" AND Elem_Cod = " & Elem_Cod)
            strSql.AppendLine(" AND Mat_Cod in (" & Agro_SQL_Save_Clausola_IN(Mat_Cod) & ")")


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

#End Region

#Region "Entity Framework"

    Public Sub Scrivi(ByRef Movimenti_dettagli As AgronicaCoreEntityFramework_POCO.Movimenti_dettagli,
                      ByRef GiasContext As Gias_DeveloperServer_Entities,
                      ByRef objParametriServer As AgronicaCoreParametri)

        Const nomeRoutine = "ContabDAL.Movimenti_Dettagli_W.Scrivi()"
        Dim messaggioErrore As String = ""

        Try
            Valorizza(Movimenti_dettagli, objParametriServer)

            Movimenti_dettagli.Data_Creazione = DateTime.Now
            Movimenti_dettagli.Data_Modifica = DateTime.Now
            Movimenti_dettagli.Username_Creazione = objParametriServer.UsernameOperazione
            Movimenti_dettagli.Username_Modifica = objParametriServer.UsernameOperazione

            GiasContext.Movimenti_dettagli.Add(Movimenti_dettagli)
        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try
    End Sub

    Public Sub Modifica(ByRef Movimenti_dettagli As AgronicaCoreEntityFramework_POCO.Movimenti_dettagli,
                        ByRef GiasContext As Gias_DeveloperServer_Entities,
                        ByRef objParametriServer As AgronicaCoreParametri)

        Const nomeRoutine = "ContabDAL.Movimenti_Dettagli_W.Modifica()"
        Dim messaggioErrore As String = ""

        Try

            Valorizza(Movimenti_dettagli, objParametriServer)

            Movimenti_dettagli.Data_Modifica = DateTime.Now
            Movimenti_dettagli.Username_Modifica = objParametriServer.UsernameOperazione

            GiasContext.Entry(Movimenti_dettagli).State = EntityState.Modified

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try
    End Sub

    Public Sub Valorizza(ByRef Movimenti_dettagli As AgronicaCoreEntityFramework_POCO.Movimenti_dettagli,
                         ByRef objParametriServer As AgronicaCoreParametri)

        Const nomeRoutine = "ContabDAL.Movimenti_Dettagli_W.Valorizza()"
        Dim messaggioErrore As String = ""

        Try

            If Movimenti_dettagli.Elem_Cod Is Nothing Then
                Movimenti_dettagli.Elem_Cod = 0
            End If

            If Movimenti_dettagli.Pro_Cod Is Nothing Then
                Movimenti_dettagli.Pro_Cod = 0
            End If

            If Movimenti_dettagli.Mat_Cod Is Nothing Then
                Movimenti_dettagli.Mat_Cod = 0
            End If

            If Movimenti_dettagli.Mov_Det_Des Is Nothing Then
                Movimenti_dettagli.Mov_Det_Des = ""
            End If

            If Movimenti_dettagli.Udm_Cod Is Nothing Then
                Movimenti_dettagli.Udm_Cod = 0
            End If

            If Movimenti_dettagli.Qta Is Nothing Then
                Movimenti_dettagli.Qta = 0
            End If

            If Movimenti_dettagli.Cod_Iva Is Nothing Then
                Movimenti_dettagli.Cod_Iva = 0
            End If

            If Movimenti_dettagli.Sconto Is Nothing Then
                Movimenti_dettagli.Sconto = 0
            End If

            If Movimenti_dettagli.Prezzo_Unitario Is Nothing Then
                Movimenti_dettagli.Prezzo_Unitario = 0
            End If

            If Movimenti_dettagli.Cod_Conto Is Nothing Then
                Movimenti_dettagli.Cod_Conto = 0
            End If

            If Movimenti_dettagli.Cod_Progetto Is Nothing Then
                Movimenti_dettagli.Cod_Progetto = 0
            End If

            If Movimenti_dettagli.Fase_Cod Is Nothing Then
                Movimenti_dettagli.Fase_Cod = 0
            End If

            If Movimenti_dettagli.Contabilizzato Is Nothing Then
                Movimenti_dettagli.Contabilizzato = 0
            End If

            If Movimenti_dettagli.Pendente Is Nothing Then
                Movimenti_dettagli.Pendente = 0
            End If

            If Movimenti_dettagli.inviato Is Nothing Then
                Movimenti_dettagli.inviato = 0
            End If

            If Movimenti_dettagli.Data_Creazione Is Nothing Then
                Movimenti_dettagli.Data_Creazione = DateTime.Now
            End If

            If Movimenti_dettagli.Username_Creazione Is Nothing Then
                Movimenti_dettagli.Username_Creazione = objParametriServer.UsernameOperazione
            End If

            If Movimenti_dettagli.Validita_Inizio Is Nothing Then
                Movimenti_dettagli.Validita_Inizio = AGRODATAINIZIO
            End If

            If Movimenti_dettagli.Validita_Fine Is Nothing Then
                Movimenti_dettagli.Validita_Fine = AGRODATAFINE
            End If

            If Movimenti_dettagli.Validita_Inizio < AGRODATAINIZIO Then
                Movimenti_dettagli.Validita_Inizio = AGRODATAINIZIO
            End If

            If Movimenti_dettagli.Validita_Fine < AGRODATAINIZIO Then
                Movimenti_dettagli.Validita_Fine = AGRODATAFINE
            End If

            If Movimenti_dettagli.Cal_Cod Is Nothing Then
                Movimenti_dettagli.Cal_Cod = 0
            End If

            If Movimenti_dettagli.Extra_Str Is Nothing Then
                Movimenti_dettagli.Extra_Str = ""
            End If

            If Movimenti_dettagli.Extra_Int Is Nothing Then
                Movimenti_dettagli.Extra_Int = 0
            End If

            If Movimenti_dettagli.Extra_Date Is Nothing Then
                Movimenti_dettagli.Extra_Date = AGRODATAINIZIO
            End If

            If Movimenti_dettagli.Anno Is Nothing Then
                Movimenti_dettagli.Anno = 0
            End If

            If Movimenti_dettagli.Ric_Cod Is Nothing Then
                Movimenti_dettagli.Ric_Cod = 0
            End If

            If Movimenti_dettagli.Imponibile Is Nothing Then
                Movimenti_dettagli.Imponibile = 0
            End If

            If Movimenti_dettagli.Iva Is Nothing Then
                Movimenti_dettagli.Iva = 0
            End If

            If Movimenti_dettagli.Lotto Is Nothing Then
                Movimenti_dettagli.Lotto = ""
            End If

            If Movimenti_dettagli.Jolly_Int Is Nothing Then
                Movimenti_dettagli.Jolly_Int = 0
            End If

            If Movimenti_dettagli.Imponibile_Netto Is Nothing Then
                Movimenti_dettagli.Imponibile_Netto = 0
            End If

            If Movimenti_dettagli.Prezzo_Unitario_Netto Is Nothing Then
                Movimenti_dettagli.Prezzo_Unitario_Netto = 0
            End If

            If Movimenti_dettagli.Mezzo_Det Is Nothing Then
                Movimenti_dettagli.Mezzo_Det = 0
            End If

            If Movimenti_dettagli.Sconto_Testo Is Nothing Then
                Movimenti_dettagli.Sconto_Testo = ""
            End If

            If Movimenti_dettagli.Ric_Cod_Pat Is Nothing Then
                Movimenti_dettagli.Ric_Cod_Pat = 0
            End If

            If Movimenti_dettagli.Cod_Conto_Pat Is Nothing Then
                Movimenti_dettagli.Cod_Conto_Pat = 0
            End If

            If Movimenti_dettagli.TempoCarenza Is Nothing Then
                Movimenti_dettagli.TempoCarenza = 0
            End If

            If Movimenti_dettagli.DoseEtichetta Is Nothing Then
                Movimenti_dettagli.DoseEtichetta = ""
            End If

            If Movimenti_dettagli.Turno_Cod Is Nothing Then
                Movimenti_dettagli.Turno_Cod = 0
            End If

            If Movimenti_dettagli.ID_Attivita Is Nothing Then
                Movimenti_dettagli.ID_Attivita = 0
            End If

            If Movimenti_dettagli.Dettaglio_VegCod Is Nothing Then
                Movimenti_dettagli.Dettaglio_VegCod = 0
            End If

            If Movimenti_dettagli.Iva_Indetraibile Is Nothing Then
                Movimenti_dettagli.Iva_Indetraibile = 0
            End If

            If Movimenti_dettagli.Iva_Indetraibile_Perc Is Nothing Then
                Movimenti_dettagli.Iva_Indetraibile_Perc = 0
            End If

            If Movimenti_dettagli.PrincipiAttivi Is Nothing Then
                Movimenti_dettagli.PrincipiAttivi = ""
            End If

            If Movimenti_dettagli.ClassiTossicologiche Is Nothing Then
                Movimenti_dettagli.ClassiTossicologiche = ""
            End If

            If Movimenti_dettagli.DoseEtichetta_Value Is Nothing Then
                Movimenti_dettagli.DoseEtichetta_Value = ""
            End If

            If Movimenti_dettagli.Iva_Deto_Cod Is Nothing Then
                Movimenti_dettagli.Iva_Deto_Cod = 0
            End If

            If Movimenti_dettagli.Qta_Dettaglio1 Is Nothing Then
                Movimenti_dettagli.Qta_Dettaglio1 = 0
            End If

            If Movimenti_dettagli.Qta_Dettaglio2 Is Nothing Then
                Movimenti_dettagli.Qta_Dettaglio2 = 0
            End If

            If Movimenti_dettagli.Dettagli_Blocco_Flag Is Nothing Then
                Movimenti_dettagli.Dettagli_Blocco_Flag = 0
            End If

            If Movimenti_dettagli.Dettagli_Blocco_Username Is Nothing Then
                Movimenti_dettagli.Dettagli_Blocco_Username = ""
            End If

            If Movimenti_dettagli.Dettagli_Blocco_Data Is Nothing Then
                Movimenti_dettagli.Dettagli_Blocco_Data = AGRODATAINIZIO
            End If

            If Movimenti_dettagli.Ordine_Det Is Nothing Then
                Movimenti_dettagli.Ordine_Det = 0
            End If

            If Movimenti_dettagli.Deroga_Cod Is Nothing Then
                Movimenti_dettagli.Deroga_Cod = 0
            End If

            If Movimenti_dettagli.Prezzo_Livello Is Nothing Then
                Movimenti_dettagli.Prezzo_Livello = 0
            End If

            If Movimenti_dettagli.Qualifica_Cod Is Nothing Then
                Movimenti_dettagli.Qualifica_Cod = 0
            End If

            If Movimenti_dettagli.Tariffa_Cod Is Nothing Then
                Movimenti_dettagli.Tariffa_Cod = 0
            End If

            If Movimenti_dettagli.PrincipiAttiviPesi Is Nothing Then
                Movimenti_dettagli.PrincipiAttiviPesi = ""
            End If

            If Movimenti_dettagli.Buffer Is Nothing Then
                Movimenti_dettagli.Buffer = ""
            End If

            If Movimenti_dettagli.Rif_Esterno Is Nothing Then
                Movimenti_dettagli.Rif_Esterno = ""
            End If

            If Movimenti_dettagli.Rif_Esterno_2 Is Nothing Then
                Movimenti_dettagli.Rif_Esterno_2 = ""
            End If

            If Movimenti_dettagli.Polverulento Is Nothing Then
                Movimenti_dettagli.Polverulento = 0
            End If

            If Movimenti_dettagli.Dettaglio_IdCod Is Nothing Then
                Movimenti_dettagli.Dettaglio_IdCod = 0
            End If

            If Movimenti_dettagli.Dettaglio_GenCod Is Nothing Then
                Movimenti_dettagli.Dettaglio_GenCod = 0
            End If

            If Movimenti_dettagli.Dettaglio_SpeCod Is Nothing Then
                Movimenti_dettagli.Dettaglio_SpeCod = 0
            End If

            If Movimenti_dettagli.Dettaglio_IProCod Is Nothing Then
                Movimenti_dettagli.Dettaglio_IProCod = 0
            End If

            If Movimenti_dettagli.Id_Mov_Esterno Is Nothing Then
                Movimenti_dettagli.Id_Mov_Esterno = 0
            End If

            If Movimenti_dettagli.Mat_Cod_Alias Is Nothing Then
                Movimenti_dettagli.Mat_Cod_Alias = 0
            End If

            If Movimenti_dettagli.PrincipiAttiviPercAbb Is Nothing Then
                Movimenti_dettagli.PrincipiAttiviPercAbb = ""
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

    Public Sub Elimina(ByRef Movimenti_dettagli As AgronicaCoreEntityFramework_POCO.Movimenti_dettagli,
                       ByRef GiasContext As Gias_DeveloperServer_Entities,
                       ByRef objParametriServer As AgronicaCoreParametri)

        Const nomeRoutine = "ContabDAL.Movimenti_Dettagli_W.Elimina()"
        Dim messaggioErrore As String = ""

        Try

            GiasContext.Movimenti_dettagli.Attach(Movimenti_dettagli)
            GiasContext.Movimenti_dettagli.Remove(Movimenti_dettagli)

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

#End Region

End Class
