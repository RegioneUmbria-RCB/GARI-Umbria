Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreEntityFramework
Imports Newtonsoft.Json
Imports AgronicaCoreModelsSTD.attivita.dettagli

Public Class Operazioni_R
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Lav_Des_From_Lav_Cod(ByVal Lav_Cod As Long, ByRef objParametri As AgronicaCoreParametri) As String

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Operazioni_R.Lav_Des_From_Lav_Cod()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try


            StrSQL.Length = 0

            StrSQL.Append(" SELECT  Lav_Cod, Lav_Des ")
            StrSQL.Append(" FROM  Operazioni ")
            StrSQL.Append(" WHERE Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & " ")


            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt.Rows(0).Item("Lav_Des")

    End Function


    Public Function GruppoOperazioni_From_Lav_Cod(ByVal Lav_Cod As Long, ByRef objParametri As AgronicaCoreParametri) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Operazioni_R.Lav_Des_From_Lav_Cod()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try


            StrSQL.Length = 0

            StrSQL.Append(" SELECT  grOP.* ")
            StrSQL.Append(" FROM  Operazioni OP WITH(NOLOCK)")
            StrSQL.Append(" INNER JOIN  GruppoOperazioni grOP WITH(NOLOCK)")
            StrSQL.Append(" ON    OP.GRU_OP = grOP.GRU_COD ")
            StrSQL.Append(" WHERE Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & " ")


            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   OP.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   OP.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------


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
    '##############################################################################################

    Public Function Leggi(ByVal Lav_Cod As Long,
                          ByVal Gru_Op As Long,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Operazioni_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            'If Cod_Indirizzo = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Cod_Indirizzo obbligatorio)")
            'End If
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT  LAV_DES, LAV_COD ")
                    StrSQL.Append(" FROM  Operazioni ")
                    StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")


                    If Lav_Cod <> 0 Then
                        StrSQL.Append(" AND Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & "   ")
                    End If

                    If Gru_Op <> 0 Then
                        StrSQL.Append(" AND Gru_Op = " & Agro_SQL_SaveNum(Gru_Op) & "   ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Gru_Op ASC")
                    End If

                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT  * ")
                    StrSQL.Append(" FROM  Operazioni ")
                    StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")


                    If Lav_Cod <> 0 Then
                        StrSQL.Append(" AND Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & "   ")
                    End If

                    If Gru_Op <> 0 Then
                        StrSQL.Append(" AND Gru_Op = " & Agro_SQL_SaveNum(Gru_Op) & "   ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Gru_Op ASC")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni
                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT  OP.LAV_COD, OP.LAV_DES, ISNULL(grOP.GRU_COD,0) AS GRU_COD ,ISNULL(grOP.GRU_DES,'') AS GRU_DES ")
                    StrSQL.Append(" FROM  Operazioni OP")
                    StrSQL.Append(" LEFT JOIN  GruppoOperazioni grOP")
                    StrSQL.Append(" ON    OP.GRU_OP = grOP.GRU_COD ")
                    StrSQL.Append(" WHERE OP.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   OP.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Lav_Cod <> 0 Then
                        StrSQL.Append(" AND OP.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & "   ")
                    End If

                    If Gru_Op <> 0 Then
                        StrSQL.Append(" AND OP.GRU_OP = " & Agro_SQL_SaveNum(Gru_Op) & "   ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    Else
                        StrSQL.Append(" AND ((grOP.Tipo IN ('C', 'Z') AND (grOP.ATT_COD < 10000 AND grOP.ATT_COD <> 2500)) OR OP.Lav_Cod = 5007)")
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   OP.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   OP.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY grOP.Tipo, grOP.GRU_DES, OP.LAV_DES ASC")
                    End If


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


    '##############################################################################################

    <CacheableAttribute(False)>
    Public Function Leggi_x_Grid_Agenda(ByVal Piva As String,
                                        ByVal Sa_Cod As Integer,
                                        ByVal Validita_Inizio As Date,
                                        ByVal Validita_Fine As Date,
                                        ByVal Veg_Cod As Integer,
                                        ByVal Cul_Cod As Integer,
                                        ByVal Tipo As String,
                                        ByVal Gru_Cod As Integer,
                                        ByVal Lav_Cod As Integer,
                                        ByVal Flag_TerrenoNudo As Boolean,
                                        ByVal Flag_Anche_Veg_Cod_Cul_Cod_0 As Boolean,
                                        ByVal FiltroOperazioni As String,
                                        ByVal FiltroImpianti As String,
                                        ByVal Filtro_GruppoOperazioni As String,
                                        ByRef objParametri_Utenti As AgronicaCoreParametri,
                                        ByRef objParametri_Server As AgronicaCoreParametri
                                        ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Operazioni_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StbQuery As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            Dim NomeDB_Utenti As String
            NomeDB_Utenti = objParametri_Utenti.StringaConnessione.Split(";")(2)
            NomeDB_Utenti = NomeDB_Utenti.Split("=")(1)
            StbQuery.Length = 0

            Dim FiltroCentri As String = ""
            If Sa_Cod = 0 Then
                'leggo se ci sono filtri sui centri
                FiltroCentri = EstraiFiltroCentri(Piva, objParametri_Server)
            End If

            StbQuery.Append(" ( ")

            '/////////////////////////////////////////////////////////////////////////////////////////////
            '///////////////// LA PRIMA RECUPERA LE OP ZOOTECNICHE E /////////////////////////////////////
            '///////////////// LE OP COLTURALI CON I RELATIVI IMPIANTI E PRODOTTI IMPIEGATI //////////////
            '/////////////////////////////////////////////////////////////////////////////////////////////

            StbQuery.Append(" SELECT DISTINCT Agenda.Piva , Agenda.Sa_Cod, Agenda.Id_Agenda, " & vbCrLf)
            StbQuery.Append(" Agenda.Lav_Cod , Agenda.Des_Lib, Movimenti.Data_Movimento, Movimenti.Mov_Desc, Agenda.Username_Creazione, Movimenti.Cau_Mov, " & vbCrLf)
            StbQuery.Append(" ISNULL(Reg_Impianti.Cul_Cod, 0) AS Cul_Cod, ISNULL(Cultivar.Veg_Cod, 0) AS Veg_Cod, ISNULL(SpecieVegetali.Veg_Des, '') AS Veg_Des, " & vbCrLf)
            StbQuery.Append(" Agenda.Blocco_Flag, ISNULL((Dettagli.Cognome + ' ' + Dettagli.Nome), 'N.D.') AS Tecnico,  " & vbCrLf)

            StbQuery.Append(" ISNULL(Mov_Destinazioni.Tipo_Destinazione, -1) AS Tipo_Destinazione, ISNULL(Appezzamento.APPEZZA, 0) AS Appezza, ISNULL(Appezzamento.APP_NOME, '') AS App_Nome, " & vbCrLf)
            StbQuery.Append(" ISNULL(Movimenti_dettagli.Elem_Cod, 0) AS Elem_Cod, ISNULL(Movimenti_dettagli.Mat_Cod, 0) AS Mat_Cod, ISNULL(Movimenti_dettagli.Pro_Cod, 0) AS Pro_Cod, " & vbCrLf)

            StbQuery.Append(" ISNULL(FORMULATI.Fr_Des, '') AS Fr_Des, ISNULL(FERTILIZZANTI.Fer_Des, '') AS Fer_Des, ISNULL(TRAPPOLE.TRAP_DES, '') AS Trap_Des, " & vbCrLf)
            StbQuery.Append(" ISNULL(Materie_Prime.Mat_Des, '') AS Mat_Des, ISNULL(Materie_Prime.Cod_Articolo, '') AS Cod_Articolo, " & vbCrLf)

            StbQuery.Append(" ISNULL(( SELECT TOP 1 RicettexAgenda.Ricetta_Cod FROM RicettexAgenda " & vbCrLf)
            StbQuery.Append(" WHERE RicettexAgenda.id_agenda = Agenda.id_agenda ")
            StbQuery.Append(" AND RicettexAgenda.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri_Server.PivaSuperUser) & "') ")
            StbQuery.Append(" , 0) AS Ricetta_Cod   ,  " & vbCrLf)

            StbQuery.Append(" ISNULL(( SELECT TOP 1 Centri_Aziendali.sa_nome FROM  Centri_Aziendali " & vbCrLf)
            StbQuery.Append(" WHERE Centri_Aziendali.sa_cod = Agenda.Sa_Cod and  Centri_Aziendali.piva = Agenda.piva  ) ")
            StbQuery.Append(" , '') AS sa_nome,  " & vbCrLf)

            'modifica per avversita
            StbQuery.Append(" ISNULL(avversita.av_Des_Vol, '') AS av_Des_Vol, " & vbCrLf)
            StbQuery.Append(" ISNULL(GruppoAvversita.av_gru_Des, '') AS av_gru_Des " & vbCrLf)


            StbQuery.Append(" FROM Movimenti_dettagli " & vbCrLf)
            StbQuery.Append(" LEFT OUTER JOIN Formulati ON Movimenti_dettagli.Pro_Cod = Formulati.Fr_Cod " & vbCrLf)
            StbQuery.Append(" LEFT OUTER JOIN Fertilizzanti ON Movimenti_dettagli.Pro_Cod = Fertilizzanti.Fer_Cod " & vbCrLf)
            StbQuery.Append(" LEFT OUTER JOIN Trappole ON Movimenti_dettagli.Pro_Cod = Trappole.TRAP_COD " & vbCrLf)
            StbQuery.Append(" LEFT OUTER JOIN Materie_Prime ON Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod AND " & vbCrLf)
            StbQuery.Append(" Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod RIGHT OUTER JOIN " & vbCrLf)
            StbQuery.Append(NomeDB_Utenti & ".dbo.Utenti_Dettagli Dettagli RIGHT OUTER JOIN " & vbCrLf)
            StbQuery.Append(" Agenda INNER JOIN " & vbCrLf)
            StbQuery.Append(" Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda ON " & vbCrLf)
            StbQuery.Append(" Dettagli.CodFisc = Agenda.Username_Creazione ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND " & vbCrLf)
            StbQuery.Append(" Movimenti_dettagli.Sa_Cod = Movimenti.Sa_Cod AND Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND " & vbCrLf)
            StbQuery.Append(" Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov LEFT OUTER JOIN " & vbCrLf)
            StbQuery.Append(" Appezzamento INNER JOIN " & vbCrLf)
            StbQuery.Append(" Reg_Impianti INNER JOIN " & vbCrLf)
            StbQuery.Append(" Mov_Destinazioni ON Reg_Impianti.PIVA = Mov_Destinazioni.Piva AND Reg_Impianti.SA_COD = Mov_Destinazioni.Sa_Cod AND " & vbCrLf)
            StbQuery.Append(" Reg_Impianti.APPEZZA = Mov_Destinazioni.Appezza AND Reg_Impianti.ID_REG = Mov_Destinazioni.Id_Destinazione ON " & vbCrLf)
            StbQuery.Append(" Appezzamento.PIVA = Reg_Impianti.PIVA AND Appezzamento.SA_COD = Reg_Impianti.SA_COD AND " & vbCrLf)
            StbQuery.Append(" Appezzamento.APPEZZA = Reg_Impianti.APPEZZA ON Movimenti.PIVA = Mov_Destinazioni.Piva AND " & vbCrLf)
            StbQuery.Append(" Movimenti.Sa_Cod = Mov_Destinazioni.Sa_Cod AND Movimenti.Id_Agenda = Mov_Destinazioni.Id_Agenda AND " & vbCrLf)
            StbQuery.Append(" Movimenti.Id_Mov = Mov_Destinazioni.Id_Mov LEFT OUTER JOIN " & vbCrLf)
            StbQuery.Append(" Cultivar ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod " & vbCrLf)
            StbQuery.Append(" LEFT OUTER JOIN SpecieVegetali ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod " & vbCrLf)

            'modifica per avversita
            StbQuery.Append(" LEFT OUTER JOIN dbo.Mov_Dettaglio_Tecnico on Mov_Dettaglio_Tecnico.PIVA = Movimenti.PIVA AND Mov_Dettaglio_Tecnico.Sa_Cod = Movimenti.Sa_Cod AND Mov_Dettaglio_Tecnico.Id_Agenda = Movimenti.Id_Agenda and Mov_Dettaglio_Tecnico.id_mov=Movimenti.id_mov and ( Mov_Dettaglio_Tecnico.av_cod<>0 or Mov_Dettaglio_Tecnico.av_gru<>0) " & vbCrLf)
            StbQuery.Append(" LEFT OUTER JOIN Avversita on Avversita.av_cod=Mov_Dettaglio_Tecnico.av_cod and Mov_Dettaglio_Tecnico.av_cod<>0  " & vbCrLf)
            StbQuery.Append(" LEFT OUTER JOIN GruppoAvversita on GruppoAvversita.av_gru=Mov_Dettaglio_Tecnico.av_gru and Mov_Dettaglio_Tecnico.av_gru<>0  " & vbCrLf)

            'Modifica per gruppi operazioni e tipo
            StbQuery.Append(" LEFT OUTER JOIN dbo.Operazioni on Agenda.Lav_Cod = Operazioni.Lav_Cod " & vbCrLf)
            StbQuery.Append(" LEFT OUTER JOIN dbo.GruppoOperazioni on Operazioni.GRU_OP = GruppoOperazioni.GRU_COD " & vbCrLf)

            StbQuery.Append(" WHERE Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "'   " & vbCrLf)

            '--------------------------
            'modifica per magazzino e contabilita
            If Sa_Cod <> 0 Then
                StbQuery.Append(" AND Agenda.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "    " & vbCrLf)
            Else
                If FiltroCentri <> "" Then
                    StbQuery.Append(" AND Agenda.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ")
                End If
            End If
            'If Sa_Cod <> 0 Then
            '    StbQuery.Append(" AND ( Agenda.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   or   Agenda.Sa_Cod =0 )   " & vbCrLf)
            'End If
            '--------------------------

            If Filtro_GruppoOperazioni <> "" Then
                StbQuery.Append(" AND ( " & Filtro_GruppoOperazioni & " )  " & vbCrLf)
            End If

            If FiltroOperazioni <> "" Then
                StbQuery.Append(" AND " & Agro_SQL_SaveText(FiltroOperazioni) & "   " & vbCrLf)
            End If

            If FiltroImpianti <> "" Then
                StbQuery.Append(" AND " & Agro_SQL_SaveText(FiltroImpianti) & "   " & vbCrLf)
            End If

            If Tipo <> "" Then
                StbQuery.Append(" AND GruppoOperazioni.Tipo = '" & Agro_SQL_SaveText(Tipo) & "'   " & vbCrLf)
            End If

            If Gru_Cod <> 0 Then
                StbQuery.Append(" AND GruppoOperazioni.GRU_COD = " & Agro_SQL_SaveNum(Gru_Cod) & "   " & vbCrLf)
            End If


            '--------------------------
            'modifica per magazzino e contabilita (1000,1001,1020,1021,1022,1023,1025,1031,1033
            'venivano escluse le operazioni contabili e di magazzino
            StbQuery.Append(" AND ( (Agenda.Lav_Cod < " & CStr(LAVCOD_FATTURA_RICEVUTA) & ") OR (Agenda.Lav_Cod >= " & CStr(LAVCOD_NASCITA_ANIMALI) & " AND Agenda.Lav_Cod < " & CStr(LAVCOD_REVISIONE_MACCHINE) & ") ) " & vbCrLf)
            '--------------------------


            '--------------------------
            'modifica per magazzino e contabilita
            StbQuery.Append(" AND Movimenti.Cau_Mov IN ('" & CAU_TRATTAMENTO & "','" & CAU_RILIEVO_CAMPO & "','" & CAU_RILIEVO_RACCOLTA & "','" & CAU_LAVORAZIONE & "')  " & vbCrLf)
            '--------------------------

            StbQuery.Append(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio) & "   " & vbCrLf)
            StbQuery.Append(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine) & "   " & vbCrLf)

            StbQuery.Append(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(CDate(CDate(objParametri_Server.FinestraTemporaleInizio).ToShortDateString)) & "   " & vbCrLf)
            StbQuery.Append(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(CDate(CDate(objParametri_Server.FinestraTemporaleFine).ToShortDateString)) & "   " & vbCrLf)

            If Not Flag_TerrenoNudo Then
                If Flag_Anche_Veg_Cod_Cul_Cod_0 Then
                    If Veg_Cod <> 0 AndAlso Veg_Cod <> -1 Then
                        StbQuery.Append(" AND ( Cultivar.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " )    " & vbCrLf)
                    End If
                    If Cul_Cod <> 0 Then
                        StbQuery.Append(" AND ( Reg_Impianti.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & "  )   " & vbCrLf)
                    End If
                Else
                    If Veg_Cod <> 0 AndAlso Veg_Cod <> -1 Then
                        StbQuery.Append(" AND Cultivar.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "   " & vbCrLf)
                    End If
                    If Cul_Cod <> 0 Then
                        StbQuery.Append(" AND Reg_Impianti.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & "   " & vbCrLf)
                    End If
                End If
            Else
                If Flag_Anche_Veg_Cod_Cul_Cod_0 Then
                    StbQuery.Append(" AND ( Reg_Impianti.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & " )   " & vbCrLf)
                Else
                    StbQuery.Append(" AND Reg_Impianti.Cul_Cod = 0   " & vbCrLf)
                End If
            End If


            StbQuery.Append(" ) ")
            StbQuery.Append(" UNION ")
            StbQuery.Append(" ( ")


            '/////////////////////////////////////////////////////////////////////////////////////////////
            '///////////////// LE OP COLTURALI POST RACCOLTA //////////////
            '/////////////////////////////////////////////////////////////////////////////////////////////

            StbQuery.Append(" SELECT DISTINCT Agenda.Piva , Agenda.Sa_Cod, Agenda.Id_Agenda, " & vbCrLf)
            StbQuery.Append(" Agenda.Lav_Cod , Agenda.Des_Lib, Movimenti.Data_Movimento, Movimenti.Mov_Desc, Agenda.Username_Creazione, Movimenti.Cau_Mov, " & vbCrLf)
            StbQuery.Append(" ISNULL(Cultivar.Cul_Cod, 0) AS Cul_Cod, ISNULL(Cultivar.Veg_Cod, 0) AS Veg_Cod, ISNULL(SpecieVegetali.Veg_Des, '') AS Veg_Des, " & vbCrLf)
            StbQuery.Append(" Agenda.Blocco_Flag, ISNULL((Dettagli.Cognome + ' ' + Dettagli.Nome), 'N.D.') AS Tecnico,  " & vbCrLf)

            StbQuery.Append(" ISNULL(Mov_Destinazioni.Tipo_Destinazione, -1) AS Tipo_Destinazione, 0 AS Appezza, '' AS App_Nome, " & vbCrLf)
            StbQuery.Append(" ISNULL(Movimenti_dettagli.Elem_Cod, 0) AS Elem_Cod, ISNULL(Movimenti_dettagli.Mat_Cod, 0) AS Mat_Cod, ISNULL(Movimenti_dettagli.Pro_Cod, 0) AS Pro_Cod, " & vbCrLf)

            StbQuery.Append(" ISNULL(FORMULATI.Fr_Des, '') AS Fr_Des, '' AS Fer_Des, '' AS Trap_Des, " & vbCrLf)
            StbQuery.Append(" ISNULL(Materie_Prime.Mat_Des, '') AS Mat_Des, ISNULL(Materie_Prime.Cod_Articolo, '') AS Cod_Articolo, " & vbCrLf)

            StbQuery.Append(" ISNULL(( SELECT TOP 1 RicettexAgenda.Ricetta_Cod FROM RicettexAgenda " & vbCrLf)
            StbQuery.Append(" WHERE RicettexAgenda.id_agenda = Agenda.id_agenda ")
            StbQuery.Append(" AND RicettexAgenda.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri_Server.PivaSuperUser) & "') ")
            StbQuery.Append(" , 0) AS Ricetta_Cod   ,  " & vbCrLf)

            StbQuery.Append(" ISNULL(( SELECT TOP 1 Centri_Aziendali.sa_nome FROM  Centri_Aziendali " & vbCrLf)
            StbQuery.Append(" WHERE Centri_Aziendali.sa_cod = Agenda.Sa_Cod and  Centri_Aziendali.piva = Agenda.piva  ) ")
            StbQuery.Append(" , '') AS sa_nome,  " & vbCrLf)

            'modifica per avversita
            StbQuery.Append(" ISNULL(avversita.av_Des_Vol, '') AS av_Des_Vol, " & vbCrLf)
            StbQuery.Append(" ISNULL(GruppoAvversita.av_gru_Des, '') AS av_gru_Des " & vbCrLf)


            StbQuery.Append(" FROM     GruppoOperazioni RIGHT OUTER JOIN " & vbCrLf)
            StbQuery.Append("           Mov_Destinazioni RIGHT OUTER JOIN " & vbCrLf)
            StbQuery.Append("           SpecieVegetali INNER JOIN " & vbCrLf)
            StbQuery.Append("           Materie_Prime ON SpecieVegetali.Veg_Cod = Materie_Prime.Veg_Cod RIGHT OUTER JOIN " & vbCrLf)
            StbQuery.Append("           Cultivar ON Materie_Prime.Cul_Cod = Cultivar.Cul_Cod AND SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod RIGHT OUTER JOIN " & vbCrLf)
            StbQuery.Append("           Movimenti_dettagli LEFT OUTER JOIN " & vbCrLf)
            StbQuery.Append("           Formulati ON Movimenti_dettagli.Pro_Cod = Formulati.Fr_Cod ON Materie_Prime.Elem_Cod = Movimenti_dettagli.Elem_Cod AND  " & vbCrLf)
            StbQuery.Append("           Materie_Prime.Mat_Cod = Movimenti_dettagli.Mat_Cod RIGHT OUTER JOIN " & vbCrLf)
            StbQuery.Append(NomeDB_Utenti & ".dbo.Utenti_Dettagli AS Dettagli RIGHT OUTER JOIN " & vbCrLf)
            StbQuery.Append("           Agenda INNER JOIN " & vbCrLf)
            StbQuery.Append("           Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda ON  " & vbCrLf)
            StbQuery.Append("           Dettagli.CodFisc = Agenda.Username_Creazione ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND Movimenti_dettagli.Sa_Cod = Movimenti.Sa_Cod AND  " & vbCrLf)
            StbQuery.Append("           Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov ON Mov_Destinazioni.Piva = Movimenti.PIVA AND  " & vbCrLf)
            StbQuery.Append("           Mov_Destinazioni.Sa_Cod = Movimenti.Sa_Cod AND Mov_Destinazioni.Id_Agenda = Movimenti.Id_Agenda AND  " & vbCrLf)
            StbQuery.Append("           Mov_Destinazioni.Id_Mov = Movimenti.Id_Mov LEFT OUTER JOIN " & vbCrLf)
            StbQuery.Append("           Mov_Dettaglio_Tecnico ON Movimenti.PIVA = Mov_Dettaglio_Tecnico.Piva AND Movimenti.Sa_Cod = Mov_Dettaglio_Tecnico.Sa_Cod AND  " & vbCrLf)
            StbQuery.Append("           Movimenti.Id_Agenda = Mov_Dettaglio_Tecnico.Id_Agenda AND Movimenti.Id_Mov = Mov_Dettaglio_Tecnico.Id_Mov AND (Mov_Dettaglio_Tecnico.Av_Cod <> 0 OR " & vbCrLf)
            StbQuery.Append("           Mov_Dettaglio_Tecnico.Av_Gru <> 0) LEFT OUTER JOIN " & vbCrLf)
            StbQuery.Append("           Avversita ON Avversita.Av_Cod = Mov_Dettaglio_Tecnico.Av_Cod AND Mov_Dettaglio_Tecnico.Av_Cod <> 0 LEFT OUTER JOIN " & vbCrLf)
            StbQuery.Append("           GruppoAvversita ON GruppoAvversita.Av_Gru = Mov_Dettaglio_Tecnico.Av_Gru AND Mov_Dettaglio_Tecnico.Av_Gru <> 0 LEFT OUTER JOIN " & vbCrLf)
            StbQuery.Append("           Operazioni ON Agenda.Lav_Cod = Operazioni.LAV_COD ON GruppoOperazioni.GRU_COD = Operazioni.GRU_OP   " & vbCrLf)

            StbQuery.Append(" WHERE Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "'   " & vbCrLf)

            '--------------------------
            'modifica per magazzino e contabilita
            If Sa_Cod <> 0 Then
                StbQuery.Append(" AND Agenda.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "    " & vbCrLf)
            Else
                If FiltroCentri <> "" Then
                    StbQuery.Append(" AND Agenda.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ")
                End If
            End If
            '--------------------------

            If Filtro_GruppoOperazioni <> "" Then
                StbQuery.Append(" AND ( " & Filtro_GruppoOperazioni & " )  " & vbCrLf)
            End If

            If FiltroOperazioni <> "" Then
                StbQuery.Append(" AND " & Agro_SQL_SaveText(FiltroOperazioni) & "   " & vbCrLf)
            End If

            'ATTENZIONE, post raccolta non hanno impianti con specie collegato, quindi tolgo il filtro 
            If FiltroImpianti <> "" Then
                Dim filtroImpianti2 As String = FiltroImpianti.Replace("Reg_Impianti", "Mov_Destinazioni")
                'devo togliere  and Reg_Impianti.Id_Reg in ( 74842113 )
                If filtroImpianti2.IndexOf("Mov_Destinazioni.Id_Reg") > 0 Then
                    filtroImpianti2 = filtroImpianti2.Replace("Mov_Destinazioni.Id_Reg", " (1=1)                                ")
                    filtroImpianti2 = filtroImpianti2.Remove(filtroImpianti2.Length - 30, 30)
                End If

                StbQuery.Append(" AND " & Agro_SQL_SaveText(filtroImpianti2) & "   " & vbCrLf)
            End If

            If Tipo <> "" Then
                StbQuery.Append(" AND GruppoOperazioni.Tipo = '" & Agro_SQL_SaveText(Tipo) & "'   " & vbCrLf)
            End If

            If Gru_Cod <> 0 Then
                StbQuery.Append(" AND GruppoOperazioni.GRU_COD = " & Agro_SQL_SaveNum(Gru_Cod) & "   " & vbCrLf)
            End If


            '--------------------------
            'venivano escluse le operazioni contabili e di magazzino
            'Modifica per operazione di cura
            StbQuery.Append(" AND  (Agenda.Lav_Cod = " & CStr(LAVCOD_TRATTAMENTO_POST_RACCOLTA) & " or Agenda.Lav_Cod = " & CStr(LAVCOD_CURA) & ") " & vbCrLf)
            '--------------------------


            '--------------------------
            'modifica per magazzino e contabilita
            'Modifica per operazione di cura
            StbQuery.Append(" AND Movimenti.Cau_Mov IN ('" & CAU_TRATTAMENTO & "','" & CAU_RILIEVO_CAMPO & "','" & CAU_RILIEVO_RACCOLTA & "','" & CAU_LAVORAZIONE & "','" & CAU_LINEA_PRODUZIONE & "')  " & vbCrLf)
            '--------------------------

            StbQuery.Append(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio) & "   " & vbCrLf)
            StbQuery.Append(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine) & "   " & vbCrLf)

            StbQuery.Append(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(CDate(CDate(objParametri_Server.FinestraTemporaleInizio).ToShortDateString)) & "   " & vbCrLf)
            StbQuery.Append(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(CDate(CDate(objParametri_Server.FinestraTemporaleFine).ToShortDateString)) & "   " & vbCrLf)

            If Veg_Cod <> 0 AndAlso Veg_Cod <> -1 Then
                StbQuery.Append(" AND ( Cultivar.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " )    " & vbCrLf)
            End If
            If Cul_Cod <> 0 Then
                StbQuery.Append(" AND ( Cultivar.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & "  )   " & vbCrLf)
            End If



            StbQuery.Append(" ) ")
            StbQuery.Append(" UNION ")
            StbQuery.Append(" ( ")

            '/////////////////////////////////////////////////////////////////////////////////////////////
            '///////////////// OP CONTABILI E PRODOTTI IMPIEGATI /////////////////////////////////////////
            '/////////////////////////////////////////////////////////////////////////////////////////////

            StbQuery.Append(" SELECT DISTINCT Agenda.Piva , Agenda.Sa_Cod, Agenda.Id_Agenda, " & vbCrLf)
            'StbQuery.Append(" Agenda.Lav_Cod, Agenda.Des_Lib, Movimenti.Data_Movimento, Movimenti.Mov_Desc, Agenda.Username_Creazione, Movimenti.Cau_Mov, " & vbCrLf)
            StbQuery.Append(" Agenda.Lav_Cod , " & vbCrLf)

            StbQuery.Append(" CASE Movimenti_dettagli.pendente  " & vbCrLf)
            StbQuery.Append("       WHEN 6 THEN 'Rilievo Giacenze Iniziali - ' + Agenda.Des_Lib  " & vbCrLf)
            StbQuery.Append("       WHEN 8 THEN 'Beni Autoprodotti - ' + Agenda.Des_Lib  " & vbCrLf)
            StbQuery.Append("       WHEN 9 THEN 'Autoconsumo - ' + Agenda.Des_Lib  " & vbCrLf)
            StbQuery.Append("       WHEN 10 THEN 'Smaltimento - ' + Agenda.Des_Lib  " & vbCrLf)
            StbQuery.Append("       WHEN 16 THEN 'Furto - ' + Agenda.Des_Lib  " & vbCrLf)
            StbQuery.Append("       WHEN 12 THEN 'Trasferimento Merci - ' + Agenda.Des_Lib  " & vbCrLf)
            StbQuery.Append("       ELSE Agenda.Des_Lib  " & vbCrLf)
            StbQuery.Append(" END AS Des_Lib,  " & vbCrLf)

            StbQuery.Append(" Movimenti.Data_Movimento, Movimenti.Mov_Desc, Agenda.Username_Creazione, Movimenti.Cau_Mov, " & vbCrLf)

            StbQuery.Append(" 0 AS Cul_Cod, 0 AS Veg_Cod, '' AS Veg_Des, " & vbCrLf)
            StbQuery.Append(" Agenda.Blocco_Flag, ISNULL((Dettagli.Cognome + ' ' + Dettagli.Nome), 'N.D.') AS Tecnico,  " & vbCrLf)

            StbQuery.Append(" ISNULL(Mov_Destinazioni.Tipo_Destinazione, -1) AS Tipo_Destinazione, 0 AS Appezza,  '' AS App_Nome, " & vbCrLf)
            StbQuery.Append(" ISNULL(Movimenti_dettagli.Elem_Cod, 0) AS Elem_Cod, ISNULL(Movimenti_dettagli.Mat_Cod, 0) AS Mat_Cod, ISNULL(Movimenti_dettagli.Pro_Cod, 0) AS Pro_Cod, " & vbCrLf)

            StbQuery.Append(" ISNULL(FORMULATI.Fr_Des, '') AS Fr_Des, ISNULL(FERTILIZZANTI.Fer_Des, '') AS Fer_Des, ISNULL(TRAPPOLE.TRAP_DES, '') AS Trap_Des, " & vbCrLf)
            StbQuery.Append(" ISNULL(Materie_Prime.Mat_Des, '') AS Mat_Des, ISNULL(Materie_Prime.Cod_Articolo, '') AS Cod_Articolo, " & vbCrLf)

            StbQuery.Append(" 0 AS Ricetta_Cod   ,  " & vbCrLf)

            StbQuery.Append(" ISNULL(( SELECT TOP 1 Centri_Aziendali.sa_nome FROM  Centri_Aziendali " & vbCrLf)
            StbQuery.Append(" WHERE Centri_Aziendali.sa_cod = Agenda.Sa_Cod and  Centri_Aziendali.piva = Agenda.piva  ) ")
            StbQuery.Append(" , '') AS sa_nome,  " & vbCrLf)

            'modifica per avversita
            StbQuery.Append(" '' AS av_Des_Vol, " & vbCrLf)
            StbQuery.Append(" '' AS av_gru_Des " & vbCrLf)



            StbQuery.Append(" FROM Movimenti_dettagli " & vbCrLf)
            StbQuery.Append(" LEFT OUTER JOIN Formulati ON Movimenti_dettagli.Pro_Cod = Formulati.Fr_Cod " & vbCrLf)
            StbQuery.Append(" LEFT OUTER JOIN Fertilizzanti ON Movimenti_dettagli.Pro_Cod = Fertilizzanti.Fer_Cod " & vbCrLf)
            StbQuery.Append(" LEFT OUTER JOIN Trappole ON Movimenti_dettagli.Pro_Cod = Trappole.TRAP_COD " & vbCrLf)
            StbQuery.Append(" LEFT OUTER JOIN Materie_Prime ON Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod AND " & vbCrLf)
            StbQuery.Append(" Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod RIGHT OUTER JOIN " & vbCrLf)
            StbQuery.Append(NomeDB_Utenti & ".dbo.Utenti_Dettagli Dettagli RIGHT OUTER JOIN " & vbCrLf)
            StbQuery.Append(" Agenda INNER JOIN " & vbCrLf)
            StbQuery.Append(" Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda ON " & vbCrLf)
            StbQuery.Append(" Dettagli.CodFisc = Agenda.Username_Creazione ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND " & vbCrLf)
            StbQuery.Append(" Movimenti_dettagli.Sa_Cod = Movimenti.Sa_Cod AND Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND " & vbCrLf)
            StbQuery.Append(" Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov LEFT OUTER JOIN " & vbCrLf)
            StbQuery.Append("  Mov_Destinazioni ON Movimenti.Sa_Cod = Mov_Destinazioni.Sa_Cod AND Movimenti.Id_Agenda = Mov_Destinazioni.Id_Agenda AND " & vbCrLf)
            StbQuery.Append(" Movimenti.Id_Mov = Mov_Destinazioni.Id_Mov  " & vbCrLf)

            'Modifica per gruppi operazioni e tipo
            StbQuery.Append(" LEFT OUTER JOIN dbo.Operazioni on Agenda.Lav_Cod = Operazioni.Lav_Cod " & vbCrLf)
            StbQuery.Append(" LEFT OUTER JOIN dbo.GruppoOperazioni on Operazioni.GRU_OP = GruppoOperazioni.GRU_COD " & vbCrLf)

            StbQuery.Append(" WHERE Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "'   " & vbCrLf)

            '--------------------------
            'modifica per magazzino e contabilita
            If Sa_Cod <> 0 Then
                StbQuery.Append(" AND (  Agenda.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " OR Agenda.Sa_Cod = 0 )   " & vbCrLf)
            Else
                If FiltroCentri <> "" Then
                    StbQuery.Append(" AND  (Agenda.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") OR Agenda.Sa_Cod = 0 ) ")
                End If
            End If
            '--------------------------

            If Filtro_GruppoOperazioni <> "" Then
                StbQuery.Append(" AND ( " & Filtro_GruppoOperazioni & " )  " & vbCrLf)
            End If

            If FiltroOperazioni <> "" Then
                StbQuery.Append(" AND " & Agro_SQL_SaveText(FiltroOperazioni) & "   " & vbCrLf)
            End If

            If Tipo <> "" Then
                StbQuery.Append(" AND GruppoOperazioni.Tipo = '" & Agro_SQL_SaveText(Tipo) & "'   " & vbCrLf)
            End If

            If Gru_Cod <> 0 Then
                StbQuery.Append(" AND GruppoOperazioni.GRU_COD = " & Agro_SQL_SaveNum(Gru_Cod) & "   " & vbCrLf)
            End If

            '--------------------------
            'modifica per magazzino e contabilita (1000,1001,1020,1021,1022,1023,1025,1031,1033
            'venivano escluse le operazioni contabili e di magazzino
            StbQuery.Append(" AND ( Agenda.Lav_Cod in (" & CStr(LAVCOD_FATTURA_RICEVUTA) & "," & CStr(LAVCOD_FATTURA_EMESSA) & "," &
                            CStr(LAVCOD_VENDITA) & "," & CStr(LAVCOD_CORRISPETTIVO_VENDITA_SFUSO) & "," & CStr(LAVCOD_ACQUISTO) & "," &
                            CStr(LAVCOD_CARICO) & "," & CStr(LAVCOD_SCARICO) & "," &
                            CStr(LAVCOD_BOLLA_RICEVUTA) & "," & CStr(LAVCOD_BOLLA_EMESSA) & "," & CStr(LAVCOD_TRASFERIMENTO) & "," &
                            CStr(LAVCOD_PROCEDURA_LIQUIDAZIONE_SOCI) & "," & CStr(LAVCOD_NOTA_ACCREDITO_EMESSA) & "," & CStr(LAVCOD_NOTA_ACCREDITO_RICEVUTA) & ") ) " & vbCrLf)
            '--------------------------

            '--------------------------
            'modifica per magazzino e contabilita
            StbQuery.Append(" AND Movimenti.Cau_Mov IN ('" & CAU_REGISTRAZIONI & "','" & CAU_CARICO & "','" & CAU_SCARICO & "','" & CAU_TRASFERIMENTO & "')  " & vbCrLf)
            '--------------------------

            StbQuery.Append(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio) & "   " & vbCrLf)
            StbQuery.Append(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine) & "   " & vbCrLf)

            StbQuery.Append(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(CDate(CDate(objParametri_Server.FinestraTemporaleInizio).ToShortDateString)) & "   " & vbCrLf)
            StbQuery.Append(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(CDate(CDate(objParametri_Server.FinestraTemporaleFine).ToShortDateString)) & "   " & vbCrLf)

            StbQuery.Append(" ) ")
            StbQuery.Append(" UNION ")
            StbQuery.Append(" ( ")


            '/////////////////////////////////////////////////////////////////////////////////////////////
            '//////////// RECUPERA LA MANUTENZIONE E LA REVISIONE MACCHINE ///////////////////////////////
            '/////////////////////////////////////////////////////////////////////////////////////////////

            StbQuery.Append(" SELECT DISTINCT Agenda.Piva , Agenda.Sa_Cod, Agenda.Id_Agenda, " & vbCrLf)
            StbQuery.Append(" Agenda.Lav_Cod , Agenda.Des_Lib, Movimenti.Data_Movimento, Movimenti.Mov_Desc, Agenda.Username_Creazione, Movimenti.Cau_Mov, " & vbCrLf)
            StbQuery.Append(" 0 AS Cul_Cod, 0 AS Veg_Cod, " & vbCrLf)
            StbQuery.Append("  '' AS Veg_Des, " & vbCrLf)
            StbQuery.Append(" Agenda.Blocco_Flag, (Dettagli.Cognome + ' ' + Dettagli.Nome) AS Tecnico,  " & vbCrLf)
            StbQuery.Append("  -1 AS Tipo_Destinazione, 0 AS Appezza, '' AS App_Nome, " & vbCrLf)
            StbQuery.Append(" 0 AS Elem_Cod, 0 AS Mat_Cod, 0 AS Pro_Cod, " & vbCrLf)
            StbQuery.Append(" '' AS Fr_Des, '' AS Fer_Des, '' AS Trap_Des," & vbCrLf)
            StbQuery.Append(" '' AS Mat_Des, '' AS Cod_Articolo, " & vbCrLf)
            StbQuery.Append(" 0 AS Ricetta_Cod , " & vbCrLf)

            StbQuery.Append(" ISNULL(( SELECT TOP 1 Centri_Aziendali.sa_nome FROM  Centri_Aziendali " & vbCrLf)
            StbQuery.Append(" WHERE Centri_Aziendali.sa_cod = Agenda.Sa_Cod and  Centri_Aziendali.piva = Agenda.piva  ) ")
            StbQuery.Append(" , '') AS sa_nome , " & vbCrLf)

            'modifica per avversita
            StbQuery.Append(" null AS av_Des_Vol,  " & vbCrLf)
            StbQuery.Append(" null AS av_gru_Des  " & vbCrLf)

            StbQuery.Append(" FROM " & vbCrLf)
            StbQuery.Append(NomeDB_Utenti & ".dbo.Utenti_Dettagli Dettagli INNER JOIN " & vbCrLf)
            StbQuery.Append(" Agenda INNER JOIN " & vbCrLf)
            StbQuery.Append(" Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda ON  " & vbCrLf)
            StbQuery.Append(" Dettagli.CodFisc = Agenda.Username_Creazione " & vbCrLf)

            'Modifica per gruppi operazioni e tipo
            StbQuery.Append(" LEFT OUTER JOIN dbo.Operazioni on Agenda.Lav_Cod = Operazioni.Lav_Cod " & vbCrLf)
            StbQuery.Append(" LEFT OUTER JOIN dbo.GruppoOperazioni on Operazioni.GRU_OP = GruppoOperazioni.GRU_COD " & vbCrLf)

            StbQuery.Append(" WHERE Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "'   " & vbCrLf)

            '--------------------------

            If Filtro_GruppoOperazioni <> "" Then
                StbQuery.Append(" AND ( " & Filtro_GruppoOperazioni & " )  " & vbCrLf)
            End If

            If FiltroOperazioni <> "" Then
                StbQuery.Append(" AND " & Agro_SQL_SaveText(FiltroOperazioni) & "   " & vbCrLf)
            End If

            If Tipo <> "" Then
                StbQuery.Append(" AND GruppoOperazioni.Tipo = '" & Agro_SQL_SaveText(Tipo) & "'   " & vbCrLf)
            End If

            If Gru_Cod <> 0 Then
                StbQuery.Append(" AND GruppoOperazioni.GRU_COD = " & Agro_SQL_SaveNum(Gru_Cod) & "   " & vbCrLf)
            End If

            '--------------------------

            StbQuery.Append(" AND (  " & vbCrLf)
            StbQuery.Append(" Agenda.Lav_Cod = " & CStr(LAVCOD_MANUTENZIONE_MACCHINE) & " " & vbCrLf)
            StbQuery.Append(" OR Agenda.Lav_Cod = " & CStr(LAVCOD_REVISIONE_MACCHINE) & " " & vbCrLf)
            'le visite non devono comparire
            StbQuery.Append("  ) " & vbCrLf)

            StbQuery.Append(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio) & "   " & vbCrLf)
            StbQuery.Append(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine) & "   " & vbCrLf)

            StbQuery.Append(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(CDate(CDate(objParametri_Server.FinestraTemporaleInizio).ToShortDateString)) & "   " & vbCrLf)
            StbQuery.Append(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(CDate(CDate(objParametri_Server.FinestraTemporaleFine).ToShortDateString)) & "   " & vbCrLf)

            StbQuery.Append(" ) ")

            StbQuery.Append(" ORDER BY Movimenti.Data_Movimento DESC, Agenda.Id_Agenda DESC, App_Nome ASC")


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Server, StbQuery.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    '##############################################################################################
    <CacheableAttribute(False)>
    Public Function Leggi_x_Grid_Agenda_Fast_Senza_Avversita(
                            ByVal Piva As String,
                            ByVal Sa_Cod As Integer,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                            ByVal Veg_Cod As Integer,
                            ByVal Cul_Cod As Integer,
                            ByVal Id_Cod As Integer,
                            ByVal Tipo As String,
                            ByVal Gru_Cod As Integer,
                            ByVal Lav_Cod As Integer,
                            ByVal Flag_TerrenoNudo As Boolean,
                            ByVal Flag_Anche_Veg_Cod_Cul_Cod_0 As Boolean,
                            ByVal FiltroOperazioni As String,
                            ByVal FiltroImpianti As String,
                            ByVal Filtro_GruppoOperazioni As String,
                            ByRef objParametri_Utenti As AgronicaCoreParametri,
                            ByRef objParametri_Server As AgronicaCoreParametri
                            ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Operazioni_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StbQuery As New Text.StringBuilder
        Dim dt As DataTable

        Try
            Dim NomeDB_Utenti As String
            NomeDB_Utenti = objParametri_Utenti.StringaConnessione.Split(";")(2)
            NomeDB_Utenti = NomeDB_Utenti.Split("=")(1)
            StbQuery.Length = 0

            Dim FiltroCentri As String = ""
            If Sa_Cod = 0 Then
                'leggo se ci sono filtri sui centri
                FiltroCentri = EstraiFiltroCentri(Piva, objParametri_Server)
            End If

            StbQuery.Append(" ( ")

            '/////////////////////////////////////////////////////////////////////////////////////////////
            '///////////////// LA PRIMA RECUPERA LE OP ZOOTECNICHE E /////////////////////////////////////
            '///////////////// LE OP COLTURALI CON I RELATIVI IMPIANTI E PRODOTTI IMPIEGATI //////////////
            '/////////////////////////////////////////////////////////////////////////////////////////////

            StbQuery.Append(" SELECT DISTINCT Agenda.Piva , Agenda.Sa_Cod, Agenda.Id_Agenda, " & vbCrLf)
            StbQuery.Append(" Agenda.Lav_Cod , Agenda.Des_Lib, Movimenti.Data_Movimento, Movimenti.Mov_Desc, Agenda.Username_Creazione, Movimenti.Cau_Mov, " & vbCrLf)
            StbQuery.Append(" ISNULL(Reg_Impianti.Cul_Cod, 0) AS Cul_Cod, ISNULL(Cultivar.Veg_Cod, 0) AS Veg_Cod, ISNULL(SpecieVegetali.Veg_Des, '') AS Veg_Des, " & vbCrLf)
            StbQuery.Append(" Agenda.Blocco_Flag, ISNULL((Dettagli.Cognome + ' ' + Dettagli.Nome), 'N.D.') AS Tecnico,  " & vbCrLf)

            StbQuery.Append(" ISNULL(Mov_Destinazioni.Tipo_Destinazione, -1) AS Tipo_Destinazione, ISNULL(Appezzamento.APPEZZA, 0) AS Appezza, ISNULL(Appezzamento.APP_NOME, '') AS App_Nome, " & vbCrLf)
            StbQuery.Append(" ISNULL(Movimenti_dettagli.Elem_Cod, 0) AS Elem_Cod, ISNULL(Movimenti_dettagli.Mat_Cod, 0) AS Mat_Cod, ISNULL(Movimenti_dettagli.Pro_Cod, 0) AS Pro_Cod, " & vbCrLf)

            StbQuery.Append(" ISNULL(FORMULATI.Fr_Des, '') AS Fr_Des, ISNULL(FERTILIZZANTI.Fer_Des, '') AS Fer_Des, ISNULL(TRAPPOLE.TRAP_DES, '') AS Trap_Des, " & vbCrLf)
            StbQuery.Append(" ISNULL(Materie_Prime.Mat_Des, '') AS Mat_Des, ISNULL(Materie_Prime.Cod_Articolo, '') AS Cod_Articolo, " & vbCrLf)

            StbQuery.Append(" ISNULL(( SELECT TOP 1 RicettexAgenda.Ricetta_Cod FROM RicettexAgenda " & vbCrLf)
            StbQuery.Append(" WHERE RicettexAgenda.id_agenda = Agenda.id_agenda ")
            StbQuery.Append(" AND RicettexAgenda.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri_Server.PivaSuperUser) & "') ")
            StbQuery.Append(" , 0) AS Ricetta_Cod   ,  " & vbCrLf)

            StbQuery.Append(" ISNULL(( SELECT TOP 1 Centri_Aziendali.sa_nome FROM  Centri_Aziendali " & vbCrLf)
            StbQuery.Append(" WHERE Centri_Aziendali.sa_cod = Agenda.Sa_Cod and  Centri_Aziendali.piva = Agenda.piva  ) ")
            StbQuery.Append(" , '') AS sa_nome,  " & vbCrLf)

            'modifica per avversita
            'StbQuery.Append(" ISNULL(avversita.av_Des_Vol, '') AS av_Des_Vol, " & vbCrLf)
            'StbQuery.Append(" ISNULL(GruppoAvversita.av_gru_Des, '') AS av_gru_Des " & vbCrLf)
            StbQuery.Append(" '' AS av_Des_Vol, " & vbCrLf)
            StbQuery.Append(" '' AS av_gru_Des " & vbCrLf)
            StbQuery.Append(" , Movimenti.Id_Mov  " & vbCrLf)

            StbQuery.Append(" FROM Movimenti_dettagli " & vbCrLf)
            StbQuery.Append(" LEFT OUTER JOIN Formulati ON Movimenti_dettagli.Pro_Cod = Formulati.Fr_Cod " & vbCrLf)
            StbQuery.Append(" LEFT OUTER JOIN Fertilizzanti ON Movimenti_dettagli.Pro_Cod = Fertilizzanti.Fer_Cod " & vbCrLf)
            StbQuery.Append(" LEFT OUTER JOIN Trappole ON Movimenti_dettagli.Pro_Cod = Trappole.TRAP_COD " & vbCrLf)
            StbQuery.Append(" LEFT OUTER JOIN Materie_Prime ON Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod AND " & vbCrLf)
            StbQuery.Append(" Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod RIGHT OUTER JOIN " & vbCrLf)
            StbQuery.Append(NomeDB_Utenti & ".dbo.Utenti_Dettagli Dettagli RIGHT OUTER JOIN " & vbCrLf)
            StbQuery.Append(" Agenda INNER JOIN " & vbCrLf)
            StbQuery.Append(" Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda ON " & vbCrLf)
            StbQuery.Append(" Dettagli.CodFisc = Agenda.Username_Creazione ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND " & vbCrLf)
            StbQuery.Append(" Movimenti_dettagli.Sa_Cod = Movimenti.Sa_Cod AND Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND " & vbCrLf)
            StbQuery.Append(" Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov LEFT OUTER JOIN " & vbCrLf)
            StbQuery.Append(" Appezzamento INNER JOIN " & vbCrLf)
            StbQuery.Append(" Reg_Impianti INNER JOIN " & vbCrLf)
            StbQuery.Append(" Mov_Destinazioni ON Reg_Impianti.PIVA = Mov_Destinazioni.Piva AND Reg_Impianti.SA_COD = Mov_Destinazioni.Sa_Cod AND " & vbCrLf)
            StbQuery.Append(" Reg_Impianti.APPEZZA = Mov_Destinazioni.Appezza AND Reg_Impianti.ID_REG = Mov_Destinazioni.Id_Destinazione ON " & vbCrLf)
            StbQuery.Append(" Appezzamento.PIVA = Reg_Impianti.PIVA AND Appezzamento.SA_COD = Reg_Impianti.SA_COD AND " & vbCrLf)
            StbQuery.Append(" Appezzamento.APPEZZA = Reg_Impianti.APPEZZA ON Movimenti.PIVA = Mov_Destinazioni.Piva AND " & vbCrLf)
            StbQuery.Append(" Movimenti.Sa_Cod = Mov_Destinazioni.Sa_Cod AND Movimenti.Id_Agenda = Mov_Destinazioni.Id_Agenda AND " & vbCrLf)
            StbQuery.Append(" Movimenti.Id_Mov = Mov_Destinazioni.Id_Mov LEFT OUTER JOIN " & vbCrLf)
            StbQuery.Append(" Cultivar ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod " & vbCrLf)
            StbQuery.Append(" LEFT OUTER JOIN SpecieVegetali ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod " & vbCrLf)

            'modifica per avversita
            'StbQuery.Append(" LEFT OUTER JOIN dbo.Mov_Dettaglio_Tecnico on Mov_Dettaglio_Tecnico.PIVA = Movimenti.PIVA AND Mov_Dettaglio_Tecnico.Sa_Cod = Movimenti.Sa_Cod AND Mov_Dettaglio_Tecnico.Id_Agenda = Movimenti.Id_Agenda and Mov_Dettaglio_Tecnico.id_mov=Movimenti.id_mov and ( Mov_Dettaglio_Tecnico.av_cod<>0 or Mov_Dettaglio_Tecnico.av_gru<>0) " & vbCrLf)
            'StbQuery.Append(" LEFT OUTER JOIN Avversita on Avversita.av_cod=Mov_Dettaglio_Tecnico.av_cod and Mov_Dettaglio_Tecnico.av_cod<>0  " & vbCrLf)
            'StbQuery.Append(" LEFT OUTER JOIN GruppoAvversita on GruppoAvversita.av_gru=Mov_Dettaglio_Tecnico.av_gru and Mov_Dettaglio_Tecnico.av_gru<>0  " & vbCrLf)

            'Modifica per gruppi operazioni e tipo
            StbQuery.Append(" LEFT OUTER JOIN dbo.Operazioni on Agenda.Lav_Cod = Operazioni.Lav_Cod " & vbCrLf)
            StbQuery.Append(" LEFT OUTER JOIN dbo.GruppoOperazioni on Operazioni.GRU_OP = GruppoOperazioni.GRU_COD " & vbCrLf)

            StbQuery.Append(" WHERE Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "'   " & vbCrLf)

            '--------------------------
            'modifica per magazzino e contabilita
            If Sa_Cod <> 0 Then
                StbQuery.Append(" AND Agenda.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "    " & vbCrLf)
            Else
                If FiltroCentri <> "" Then
                    StbQuery.Append(" AND Agenda.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ")
                End If
            End If
            'If Sa_Cod <> 0 Then
            '    StbQuery.Append(" AND ( Agenda.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   or   Agenda.Sa_Cod =0 )   " & vbCrLf)
            'End If
            '--------------------------

            If Filtro_GruppoOperazioni <> "" Then
                StbQuery.Append(" AND ( " & Filtro_GruppoOperazioni & " )  " & vbCrLf)
            End If

            If FiltroOperazioni <> "" Then
                StbQuery.Append(" AND " & Agro_SQL_SaveText(FiltroOperazioni) & "   " & vbCrLf)
            End If

            If FiltroImpianti <> "" Then
                StbQuery.Append(" AND " & Agro_SQL_SaveText(FiltroImpianti) & "   " & vbCrLf)
            End If

            If Tipo <> "" Then
                StbQuery.Append(" AND GruppoOperazioni.Tipo = '" & Agro_SQL_SaveText(Tipo) & "'   " & vbCrLf)
            End If

            If Gru_Cod <> 0 Then
                StbQuery.Append(" AND GruppoOperazioni.GRU_COD = " & Agro_SQL_SaveNum(Gru_Cod) & "   " & vbCrLf)
            End If


            '--------------------------
            'modifica per magazzino e contabilita (1000,1001,1020,1021,1022,1023,1025,1031,1033
            'venivano escluse le operazioni contabili e di magazzino
            StbQuery.Append(" AND ( (Agenda.Lav_Cod < " & CStr(LAVCOD_FATTURA_RICEVUTA) & ") OR (Agenda.Lav_Cod >= " & CStr(LAVCOD_NASCITA_ANIMALI) & " AND Agenda.Lav_Cod < " & CStr(LAVCOD_REVISIONE_MACCHINE) & ") ) " & vbCrLf)
            '--------------------------


            '--------------------------
            'modifica per magazzino e contabilita
            StbQuery.Append(" AND Movimenti.Cau_Mov IN ('" & CAU_TRATTAMENTO & "','" & CAU_RILIEVO_CAMPO & "','" & CAU_RILIEVO_RACCOLTA & "','" & CAU_LAVORAZIONE & "')  " & vbCrLf)
            '--------------------------

            StbQuery.Append(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio) & "   " & vbCrLf)
            StbQuery.Append(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine) & "   " & vbCrLf)

            StbQuery.Append(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(CDate(CDate(objParametri_Server.FinestraTemporaleInizio).ToShortDateString)) & "   " & vbCrLf)
            StbQuery.Append(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(CDate(CDate(objParametri_Server.FinestraTemporaleFine).ToShortDateString)) & "   " & vbCrLf)

            If Not Flag_TerrenoNudo Then
                If Flag_Anche_Veg_Cod_Cul_Cod_0 Then
                    If Veg_Cod <> 0 AndAlso Veg_Cod <> -1 Then
                        StbQuery.Append(" AND ( Cultivar.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " )    " & vbCrLf)
                    End If
                    If Cul_Cod <> 0 Then
                        StbQuery.Append(" AND ( Reg_Impianti.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & "  )   " & vbCrLf)
                    End If
                Else
                    If Veg_Cod <> 0 AndAlso Veg_Cod <> -1 Then
                        StbQuery.Append(" AND Cultivar.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "   " & vbCrLf)
                    End If
                    If Cul_Cod <> 0 Then
                        StbQuery.Append(" AND Reg_Impianti.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & "   " & vbCrLf)
                    End If
                End If
            Else
                If Flag_Anche_Veg_Cod_Cul_Cod_0 Then
                    StbQuery.Append(" AND ( Reg_Impianti.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & " )   " & vbCrLf)
                    If Id_Cod <> 0 Then
                        StbQuery.Append(" AND EXISTS (SELECT * FROM Reg_Impianti_Codici RIC WHERE Reg_Impianti.PIVA = RIC.PIVA  AND Reg_Impianti.SA_COD = RIC.sa_cod AND   Reg_Impianti.APPEZZA = RIC.appezza AND Reg_Impianti.ID_REG = RIC.Id_Reg  AND RIC.id_cod = " & Agro_SQL_SaveNum(Id_Cod) & " ) " & vbCrLf)
                    Else
                        StbQuery.Append(" AND NOT EXISTS (SELECT * FROM Reg_Impianti_Codici RIC WHERE Reg_Impianti.PIVA = RIC.PIVA  AND Reg_Impianti.SA_COD = RIC.sa_cod AND   Reg_Impianti.APPEZZA = RIC.appezza AND Reg_Impianti.ID_REG = RIC.Id_Reg  AND RIC.id_cod >= 3000 AND RIC.id_cod < 4000 ) " & vbCrLf)
                    End If
                Else
                    StbQuery.Append(" AND Reg_Impianti.Cul_Cod = 0   " & vbCrLf)
                    If Id_Cod <> 0 Then
                        StbQuery.Append(" AND EXISTS (SELECT * FROM Reg_Impianti_Codici RIC WHERE Reg_Impianti.PIVA = RIC.PIVA  AND Reg_Impianti.SA_COD = RIC.sa_cod AND   Reg_Impianti.APPEZZA = RIC.appezza AND Reg_Impianti.ID_REG = RIC.Id_Reg  AND RIC.id_cod = " & Agro_SQL_SaveNum(Id_Cod) & " ) " & vbCrLf)
                    Else
                        StbQuery.Append(" AND NOT EXISTS (SELECT * FROM Reg_Impianti_Codici RIC WHERE Reg_Impianti.PIVA = RIC.PIVA  AND Reg_Impianti.SA_COD = RIC.sa_cod AND   Reg_Impianti.APPEZZA = RIC.appezza AND Reg_Impianti.ID_REG = RIC.Id_Reg  AND RIC.id_cod >= 3000 AND RIC.id_cod < 4000 ) " & vbCrLf)
                    End If
                End If
            End If


            StbQuery.Append(" ) ")
            StbQuery.Append(" UNION ")
            StbQuery.Append(" ( ")


            '/////////////////////////////////////////////////////////////////////////////////////////////
            '///////////////// LE OP COLTURALI POST RACCOLTA //////////////
            '/////////////////////////////////////////////////////////////////////////////////////////////

            StbQuery.Append(" SELECT DISTINCT Agenda.Piva , Agenda.Sa_Cod, Agenda.Id_Agenda, " & vbCrLf)
            StbQuery.Append(" Agenda.Lav_Cod , Agenda.Des_Lib, Movimenti.Data_Movimento, Movimenti.Mov_Desc, Agenda.Username_Creazione, Movimenti.Cau_Mov, " & vbCrLf)
            StbQuery.Append(" ISNULL(Cultivar.Cul_Cod, 0) AS Cul_Cod, ISNULL(Cultivar.Veg_Cod, 0) AS Veg_Cod, ISNULL(SpecieVegetali.Veg_Des, '') AS Veg_Des, " & vbCrLf)
            StbQuery.Append(" Agenda.Blocco_Flag, ISNULL((Dettagli.Cognome + ' ' + Dettagli.Nome), 'N.D.') AS Tecnico,  " & vbCrLf)

            StbQuery.Append(" ISNULL(Mov_Destinazioni.Tipo_Destinazione, -1) AS Tipo_Destinazione, 0 AS Appezza, '' AS App_Nome, " & vbCrLf)
            StbQuery.Append(" ISNULL(Movimenti_dettagli.Elem_Cod, 0) AS Elem_Cod, ISNULL(Movimenti_dettagli.Mat_Cod, 0) AS Mat_Cod, ISNULL(Movimenti_dettagli.Pro_Cod, 0) AS Pro_Cod, " & vbCrLf)

            StbQuery.Append(" ISNULL(FORMULATI.Fr_Des, '') AS Fr_Des, '' AS Fer_Des, '' AS Trap_Des, " & vbCrLf)
            StbQuery.Append(" ISNULL(Materie_Prime.Mat_Des, '') AS Mat_Des, ISNULL(Materie_Prime.Cod_Articolo, '') AS Cod_Articolo, " & vbCrLf)

            StbQuery.Append(" ISNULL(( SELECT TOP 1 RicettexAgenda.Ricetta_Cod FROM RicettexAgenda " & vbCrLf)
            StbQuery.Append(" WHERE RicettexAgenda.id_agenda = Agenda.id_agenda ")
            StbQuery.Append(" AND RicettexAgenda.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri_Server.PivaSuperUser) & "') ")
            StbQuery.Append(" , 0) AS Ricetta_Cod   ,  " & vbCrLf)

            StbQuery.Append(" ISNULL(( SELECT TOP 1 Centri_Aziendali.sa_nome FROM  Centri_Aziendali " & vbCrLf)
            StbQuery.Append(" WHERE Centri_Aziendali.sa_cod = Agenda.Sa_Cod and  Centri_Aziendali.piva = Agenda.piva  ) ")
            StbQuery.Append(" , '') AS sa_nome,  " & vbCrLf)

            'modifica per avversita
            'StbQuery.Append(" ISNULL(avversita.av_Des_Vol, '') AS av_Des_Vol, " & vbCrLf)
            'StbQuery.Append(" ISNULL(GruppoAvversita.av_gru_Des, '') AS av_gru_Des " & vbCrLf)
            StbQuery.Append(" '' AS av_Des_Vol, " & vbCrLf)
            StbQuery.Append(" '' AS av_gru_Des " & vbCrLf)
            StbQuery.Append(" , Movimenti.Id_Mov  " & vbCrLf)

            StbQuery.Append(" FROM     GruppoOperazioni RIGHT OUTER JOIN " & vbCrLf)
            StbQuery.Append("           Mov_Destinazioni RIGHT OUTER JOIN " & vbCrLf)
            StbQuery.Append("           SpecieVegetali INNER JOIN " & vbCrLf)
            StbQuery.Append("           Materie_Prime ON SpecieVegetali.Veg_Cod = Materie_Prime.Veg_Cod RIGHT OUTER JOIN " & vbCrLf)
            StbQuery.Append("           Cultivar ON Materie_Prime.Cul_Cod = Cultivar.Cul_Cod AND SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod RIGHT OUTER JOIN " & vbCrLf)
            StbQuery.Append("           Movimenti_dettagli LEFT OUTER JOIN " & vbCrLf)
            StbQuery.Append("           Formulati ON Movimenti_dettagli.Pro_Cod = Formulati.Fr_Cod ON Materie_Prime.Elem_Cod = Movimenti_dettagli.Elem_Cod AND  " & vbCrLf)
            StbQuery.Append("           Materie_Prime.Mat_Cod = Movimenti_dettagli.Mat_Cod RIGHT OUTER JOIN " & vbCrLf)
            StbQuery.Append(NomeDB_Utenti & ".dbo.Utenti_Dettagli AS Dettagli RIGHT OUTER JOIN " & vbCrLf)
            StbQuery.Append("           Agenda INNER JOIN " & vbCrLf)
            StbQuery.Append("           Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda ON  " & vbCrLf)
            StbQuery.Append("           Dettagli.CodFisc = Agenda.Username_Creazione ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND Movimenti_dettagli.Sa_Cod = Movimenti.Sa_Cod AND  " & vbCrLf)
            StbQuery.Append("           Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov ON Mov_Destinazioni.Piva = Movimenti.PIVA AND  " & vbCrLf)
            StbQuery.Append("           Mov_Destinazioni.Sa_Cod = Movimenti.Sa_Cod AND Mov_Destinazioni.Id_Agenda = Movimenti.Id_Agenda AND  " & vbCrLf)
            StbQuery.Append("           Mov_Destinazioni.Id_Mov = Movimenti.Id_Mov " & vbCrLf)
            'StbQuery.Append("            LEFT OUTER JOIN Mov_Dettaglio_Tecnico ON Movimenti.PIVA = Mov_Dettaglio_Tecnico.Piva AND Movimenti.Sa_Cod = Mov_Dettaglio_Tecnico.Sa_Cod AND  " & vbCrLf)
            'StbQuery.Append("           Movimenti.Id_Agenda = Mov_Dettaglio_Tecnico.Id_Agenda AND Movimenti.Id_Mov = Mov_Dettaglio_Tecnico.Id_Mov AND (Mov_Dettaglio_Tecnico.Av_Cod <> 0 OR " & vbCrLf)
            'StbQuery.Append("           Mov_Dettaglio_Tecnico.Av_Gru <> 0)  " & vbCrLf)
            'StbQuery.Append("           LEFT OUTER JOIN Avversita ON Avversita.Av_Cod = Mov_Dettaglio_Tecnico.Av_Cod AND Mov_Dettaglio_Tecnico.Av_Cod <> 0 " & vbCrLf)
            'StbQuery.Append("           LEFT OUTER JOIN GruppoAvversita ON GruppoAvversita.Av_Gru = Mov_Dettaglio_Tecnico.Av_Gru AND Mov_Dettaglio_Tecnico.Av_Gru <> 0  " & vbCrLf)
            StbQuery.Append("           LEFT OUTER JOIN Operazioni ON Agenda.Lav_Cod = Operazioni.LAV_COD ON GruppoOperazioni.GRU_COD = Operazioni.GRU_OP   " & vbCrLf)

            StbQuery.Append(" WHERE Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "'   " & vbCrLf)

            '--------------------------
            'modifica per magazzino e contabilita
            If Sa_Cod <> 0 Then
                StbQuery.Append(" AND Agenda.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "    " & vbCrLf)
            Else
                If FiltroCentri <> "" Then
                    StbQuery.Append(" AND Agenda.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ")
                End If
            End If
            '--------------------------

            If Filtro_GruppoOperazioni <> "" Then
                StbQuery.Append(" AND ( " & Filtro_GruppoOperazioni & " )  " & vbCrLf)
            End If

            If FiltroOperazioni <> "" Then
                StbQuery.Append(" AND " & Agro_SQL_SaveText(FiltroOperazioni) & "   " & vbCrLf)
            End If

            'ATTENZIONE, post raccolta non hanno impianti con specie collegato, quindi tolgo il filtro 
            If FiltroImpianti <> "" Then
                Dim FiltroImpianti2 As String = FiltroImpianti.Replace("Reg_Impianti", "Mov_Destinazioni")
                'devo togliere  and Reg_Impianti.Id_Reg in ( 74842113 )
                If FiltroImpianti2.IndexOf("Mov_Destinazioni.Id_Reg") > 0 Then
                    FiltroImpianti2 = FiltroImpianti2.Replace("Mov_Destinazioni.Id_Reg", " (1=1)                                ")
                    FiltroImpianti2 = FiltroImpianti2.Remove(FiltroImpianti2.Length - 30, 30)
                End If

                StbQuery.Append(" AND " & Agro_SQL_SaveText(FiltroImpianti2) & "   " & vbCrLf)
            End If

            If Tipo <> "" Then
                StbQuery.Append(" AND GruppoOperazioni.Tipo = '" & Agro_SQL_SaveText(Tipo) & "'   " & vbCrLf)
            End If

            If Gru_Cod <> 0 Then
                StbQuery.Append(" AND GruppoOperazioni.GRU_COD = " & Agro_SQL_SaveNum(Gru_Cod) & "   " & vbCrLf)
            End If


            '--------------------------
            'venivano escluse le operazioni contabili e di magazzino
            'Modifica per operazione di cura
            StbQuery.Append(" AND  (Agenda.Lav_Cod = " & CStr(LAVCOD_TRATTAMENTO_POST_RACCOLTA) & " or Agenda.Lav_Cod = " & CStr(LAVCOD_CURA) & ") " & vbCrLf)
            '--------------------------


            '--------------------------
            'modifica per magazzino e contabilita
            'Modifica per operazione di cura
            StbQuery.Append(" AND Movimenti.Cau_Mov IN ('" & CAU_TRATTAMENTO & "','" & CAU_RILIEVO_CAMPO & "','" & CAU_RILIEVO_RACCOLTA & "','" & CAU_LAVORAZIONE & "','" & CAU_LINEA_PRODUZIONE & "')  " & vbCrLf)
            '--------------------------

            StbQuery.Append(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio) & "   " & vbCrLf)
            StbQuery.Append(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine) & "   " & vbCrLf)

            StbQuery.Append(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(CDate(CDate(objParametri_Server.FinestraTemporaleInizio).ToShortDateString)) & "   " & vbCrLf)
            StbQuery.Append(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(CDate(CDate(objParametri_Server.FinestraTemporaleFine).ToShortDateString)) & "   " & vbCrLf)

            If Veg_Cod <> 0 AndAlso Veg_Cod <> -1 Then
                StbQuery.Append(" AND ( Cultivar.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " )    " & vbCrLf)
            End If
            If Cul_Cod <> 0 Then
                StbQuery.Append(" AND ( Cultivar.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & "  )   " & vbCrLf)
            End If



            StbQuery.Append(" ) ")
            StbQuery.Append(" UNION ")
            StbQuery.Append(" ( ")

            '/////////////////////////////////////////////////////////////////////////////////////////////
            '///////////////// OP CONTABILI E PRODOTTI IMPIEGATI /////////////////////////////////////////
            '/////////////////////////////////////////////////////////////////////////////////////////////

            StbQuery.Append(" SELECT DISTINCT Agenda.Piva , Agenda.Sa_Cod, Agenda.Id_Agenda, " & vbCrLf)
            'StbQuery.Append(" Agenda.Lav_Cod, Agenda.Des_Lib, Movimenti.Data_Movimento, Movimenti.Mov_Desc, Agenda.Username_Creazione, Movimenti.Cau_Mov, " & vbCrLf)
            StbQuery.Append(" Agenda.Lav_Cod , " & vbCrLf)

            StbQuery.Append(" CASE Movimenti_dettagli.pendente  " & vbCrLf)
            StbQuery.Append("       WHEN 6 THEN 'Rilievo Giacenze Iniziali - ' + Agenda.Des_Lib  " & vbCrLf)
            StbQuery.Append("       WHEN 8 THEN 'Beni Autoprodotti - ' + Agenda.Des_Lib  " & vbCrLf)
            StbQuery.Append("       WHEN 9 THEN 'Autoconsumo - ' + Agenda.Des_Lib  " & vbCrLf)
            StbQuery.Append("       WHEN 10 THEN 'Smaltimento - ' + Agenda.Des_Lib  " & vbCrLf)
            StbQuery.Append("       WHEN 16 THEN 'Furto - ' + Agenda.Des_Lib  " & vbCrLf)
            StbQuery.Append("       WHEN 12 THEN 'Trasferimento Merci - ' + Agenda.Des_Lib  " & vbCrLf)
            StbQuery.Append("       ELSE Agenda.Des_Lib  " & vbCrLf)
            StbQuery.Append(" END AS Des_Lib,  " & vbCrLf)

            StbQuery.Append(" Movimenti.Data_Movimento, Movimenti.Mov_Desc, Agenda.Username_Creazione, Movimenti.Cau_Mov, " & vbCrLf)

            StbQuery.Append(" 0 AS Cul_Cod, 0 AS Veg_Cod, '' AS Veg_Des, " & vbCrLf)
            StbQuery.Append(" Agenda.Blocco_Flag, ISNULL((Dettagli.Cognome + ' ' + Dettagli.Nome), 'N.D.') AS Tecnico,  " & vbCrLf)

            StbQuery.Append(" ISNULL(Mov_Destinazioni.Tipo_Destinazione, -1) AS Tipo_Destinazione, 0 AS Appezza,  '' AS App_Nome, " & vbCrLf)
            StbQuery.Append(" ISNULL(Movimenti_dettagli.Elem_Cod, 0) AS Elem_Cod, ISNULL(Movimenti_dettagli.Mat_Cod, 0) AS Mat_Cod, ISNULL(Movimenti_dettagli.Pro_Cod, 0) AS Pro_Cod, " & vbCrLf)

            StbQuery.Append(" ISNULL(FORMULATI.Fr_Des, '') AS Fr_Des, ISNULL(FERTILIZZANTI.Fer_Des, '') AS Fer_Des, ISNULL(TRAPPOLE.TRAP_DES, '') AS Trap_Des, " & vbCrLf)
            StbQuery.Append(" ISNULL(Materie_Prime.Mat_Des, '') AS Mat_Des, ISNULL(Materie_Prime.Cod_Articolo, '') AS Cod_Articolo, " & vbCrLf)

            StbQuery.Append(" 0 AS Ricetta_Cod   ,  " & vbCrLf)

            StbQuery.Append(" ISNULL(( SELECT TOP 1 Centri_Aziendali.sa_nome FROM  Centri_Aziendali " & vbCrLf)
            StbQuery.Append(" WHERE Centri_Aziendali.sa_cod = Agenda.Sa_Cod and  Centri_Aziendali.piva = Agenda.piva  ) ")
            StbQuery.Append(" , '') AS sa_nome,  " & vbCrLf)

            'modifica per avversita
            StbQuery.Append(" '' AS av_Des_Vol, " & vbCrLf)
            StbQuery.Append(" '' AS av_gru_Des " & vbCrLf)
            StbQuery.Append(" , Movimenti.Id_Mov  " & vbCrLf)


            StbQuery.Append(" FROM Movimenti_dettagli " & vbCrLf)
            StbQuery.Append(" LEFT OUTER JOIN Formulati ON Movimenti_dettagli.Pro_Cod = Formulati.Fr_Cod " & vbCrLf)
            StbQuery.Append(" LEFT OUTER JOIN Fertilizzanti ON Movimenti_dettagli.Pro_Cod = Fertilizzanti.Fer_Cod " & vbCrLf)
            StbQuery.Append(" LEFT OUTER JOIN Trappole ON Movimenti_dettagli.Pro_Cod = Trappole.TRAP_COD " & vbCrLf)
            StbQuery.Append(" LEFT OUTER JOIN Materie_Prime ON Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod AND " & vbCrLf)
            StbQuery.Append(" Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod RIGHT OUTER JOIN " & vbCrLf)
            StbQuery.Append(NomeDB_Utenti & ".dbo.Utenti_Dettagli Dettagli RIGHT OUTER JOIN " & vbCrLf)
            StbQuery.Append(" Agenda INNER JOIN " & vbCrLf)
            StbQuery.Append(" Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda ON " & vbCrLf)
            StbQuery.Append(" Dettagli.CodFisc = Agenda.Username_Creazione ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND " & vbCrLf)
            StbQuery.Append(" Movimenti_dettagli.Sa_Cod = Movimenti.Sa_Cod AND Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND " & vbCrLf)
            StbQuery.Append(" Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov LEFT OUTER JOIN " & vbCrLf)
            StbQuery.Append("  Mov_Destinazioni ON Movimenti.Sa_Cod = Mov_Destinazioni.Sa_Cod AND Movimenti.Id_Agenda = Mov_Destinazioni.Id_Agenda AND " & vbCrLf)
            StbQuery.Append(" Movimenti.Id_Mov = Mov_Destinazioni.Id_Mov  " & vbCrLf)

            'Modifica per gruppi operazioni e tipo
            StbQuery.Append(" LEFT OUTER JOIN dbo.Operazioni on Agenda.Lav_Cod = Operazioni.Lav_Cod " & vbCrLf)
            StbQuery.Append(" LEFT OUTER JOIN dbo.GruppoOperazioni on Operazioni.GRU_OP = GruppoOperazioni.GRU_COD " & vbCrLf)

            StbQuery.Append(" WHERE Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "'   " & vbCrLf)

            '--------------------------
            'modifica per magazzino e contabilita
            If Sa_Cod <> 0 Then
                StbQuery.Append(" AND (  Agenda.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " OR Agenda.Sa_Cod = 0 )   " & vbCrLf)
            Else
                If FiltroCentri <> "" Then
                    StbQuery.Append(" AND  (Agenda.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") OR Agenda.Sa_Cod = 0 ) ")
                End If
            End If
            '--------------------------

            If Filtro_GruppoOperazioni <> "" Then
                StbQuery.Append(" AND ( " & Filtro_GruppoOperazioni & " )  " & vbCrLf)
            End If

            If FiltroOperazioni <> "" Then
                StbQuery.Append(" AND " & Agro_SQL_SaveText(FiltroOperazioni) & "   " & vbCrLf)
            End If

            If Tipo <> "" Then
                StbQuery.Append(" AND GruppoOperazioni.Tipo = '" & Agro_SQL_SaveText(Tipo) & "'   " & vbCrLf)
            End If

            If Gru_Cod <> 0 Then
                StbQuery.Append(" AND GruppoOperazioni.GRU_COD = " & Agro_SQL_SaveNum(Gru_Cod) & "   " & vbCrLf)
            End If

            '--------------------------
            'modifica per magazzino e contabilita (1000,1001,1020,1021,1022,1023,1025,1031,1033
            'venivano escluse le operazioni contabili e di magazzino
            StbQuery.Append(" AND ( Agenda.Lav_Cod in (" & CStr(LAVCOD_FATTURA_RICEVUTA) & "," & CStr(LAVCOD_FATTURA_EMESSA) & "," &
                            CStr(LAVCOD_VENDITA) & "," & CStr(LAVCOD_CORRISPETTIVO_VENDITA_SFUSO) & "," & CStr(LAVCOD_ACQUISTO) & "," &
                            CStr(LAVCOD_CARICO) & "," & CStr(LAVCOD_SCARICO) & "," &
                            CStr(LAVCOD_BOLLA_RICEVUTA) & "," & CStr(LAVCOD_BOLLA_EMESSA) & "," & CStr(LAVCOD_TRASFERIMENTO) & "," &
                            CStr(LAVCOD_PROCEDURA_LIQUIDAZIONE_SOCI) & "," & CStr(LAVCOD_NOTA_ACCREDITO_EMESSA) & "," & CStr(LAVCOD_NOTA_ACCREDITO_RICEVUTA) & ") ) " & vbCrLf)
            '--------------------------

            '--------------------------
            'modifica per magazzino e contabilita
            StbQuery.Append(" AND Movimenti.Cau_Mov IN ('" & CAU_REGISTRAZIONI & "','" & CAU_CARICO & "','" & CAU_SCARICO & "','" & CAU_TRASFERIMENTO & "')  " & vbCrLf)
            '--------------------------

            StbQuery.Append(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio) & "   " & vbCrLf)
            StbQuery.Append(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine) & "   " & vbCrLf)

            StbQuery.Append(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(CDate(CDate(objParametri_Server.FinestraTemporaleInizio).ToShortDateString)) & "   " & vbCrLf)
            StbQuery.Append(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(CDate(CDate(objParametri_Server.FinestraTemporaleFine).ToShortDateString)) & "   " & vbCrLf)

            StbQuery.Append(" ) ")
            StbQuery.Append(" UNION ")
            StbQuery.Append(" ( ")


            '/////////////////////////////////////////////////////////////////////////////////////////////
            '//////////// RECUPERA LA MANUTENZIONE E LA REVISIONE MACCHINE ///////////////////////////////
            '/////////////////////////////////////////////////////////////////////////////////////////////

            StbQuery.Append(" SELECT DISTINCT Agenda.Piva , Agenda.Sa_Cod, Agenda.Id_Agenda, " & vbCrLf)
            StbQuery.Append(" Agenda.Lav_Cod , Agenda.Des_Lib, Movimenti.Data_Movimento, Movimenti.Mov_Desc, Agenda.Username_Creazione, Movimenti.Cau_Mov, " & vbCrLf)
            StbQuery.Append(" 0 AS Cul_Cod, 0 AS Veg_Cod, " & vbCrLf)
            StbQuery.Append("  '' AS Veg_Des, " & vbCrLf)
            StbQuery.Append(" Agenda.Blocco_Flag, (Dettagli.Cognome + ' ' + Dettagli.Nome) AS Tecnico,  " & vbCrLf)
            StbQuery.Append("  -1 AS Tipo_Destinazione, 0 AS Appezza, '' AS App_Nome, " & vbCrLf)
            StbQuery.Append(" 0 AS Elem_Cod, 0 AS Mat_Cod, 0 AS Pro_Cod, " & vbCrLf)
            StbQuery.Append(" '' AS Fr_Des, '' AS Fer_Des, '' AS Trap_Des," & vbCrLf)
            StbQuery.Append(" '' AS Mat_Des, '' AS Cod_Articolo, " & vbCrLf)
            StbQuery.Append(" 0 AS Ricetta_Cod , " & vbCrLf)

            StbQuery.Append(" ISNULL(( SELECT TOP 1 Centri_Aziendali.sa_nome FROM  Centri_Aziendali " & vbCrLf)
            StbQuery.Append(" WHERE Centri_Aziendali.sa_cod = Agenda.Sa_Cod and  Centri_Aziendali.piva = Agenda.piva  ) ")
            StbQuery.Append(" , '') AS sa_nome , " & vbCrLf)

            'modifica per avversita
            StbQuery.Append(" null AS av_Des_Vol,  " & vbCrLf)
            StbQuery.Append(" null AS av_gru_Des  " & vbCrLf)
            StbQuery.Append(" , Movimenti.Id_Mov  " & vbCrLf)


            StbQuery.Append(" FROM " & vbCrLf)
            StbQuery.Append(NomeDB_Utenti & ".dbo.Utenti_Dettagli Dettagli INNER JOIN " & vbCrLf)
            StbQuery.Append(" Agenda INNER JOIN " & vbCrLf)
            StbQuery.Append(" Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda ON  " & vbCrLf)
            StbQuery.Append(" Dettagli.CodFisc = Agenda.Username_Creazione " & vbCrLf)




            'Modifica per gruppi operazioni e tipo
            StbQuery.Append(" LEFT OUTER JOIN dbo.Operazioni on Agenda.Lav_Cod = Operazioni.Lav_Cod " & vbCrLf)
            StbQuery.Append(" LEFT OUTER JOIN dbo.GruppoOperazioni on Operazioni.GRU_OP = GruppoOperazioni.GRU_COD " & vbCrLf)

            StbQuery.Append(" WHERE Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "'   " & vbCrLf)

            '--------------------------

            If Filtro_GruppoOperazioni <> "" Then
                StbQuery.Append(" AND ( " & Filtro_GruppoOperazioni & " )  " & vbCrLf)
            End If

            If FiltroOperazioni <> "" Then
                StbQuery.Append(" AND " & Agro_SQL_SaveText(FiltroOperazioni) & "   " & vbCrLf)
            End If

            If Tipo <> "" Then
                StbQuery.Append(" AND GruppoOperazioni.Tipo = '" & Agro_SQL_SaveText(Tipo) & "'   " & vbCrLf)
            End If

            If Gru_Cod <> 0 Then
                StbQuery.Append(" AND GruppoOperazioni.GRU_COD = " & Agro_SQL_SaveNum(Gru_Cod) & "   " & vbCrLf)
            End If

            '--------------------------

            StbQuery.Append(" AND (  " & vbCrLf)
            StbQuery.Append(" Agenda.Lav_Cod = " & CStr(LAVCOD_MANUTENZIONE_MACCHINE) & " " & vbCrLf)
            StbQuery.Append(" OR Agenda.Lav_Cod = " & CStr(LAVCOD_REVISIONE_MACCHINE) & " " & vbCrLf)
            'le visite non devono comparire
            StbQuery.Append("  ) " & vbCrLf)

            StbQuery.Append(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio) & "   " & vbCrLf)
            StbQuery.Append(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine) & "   " & vbCrLf)

            StbQuery.Append(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(CDate(CDate(objParametri_Server.FinestraTemporaleInizio).ToShortDateString)) & "   " & vbCrLf)
            StbQuery.Append(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(CDate(CDate(objParametri_Server.FinestraTemporaleFine).ToShortDateString)) & "   " & vbCrLf)


            StbQuery.Append(" ) ")
            StbQuery.Append(" UNION ")
            StbQuery.Append(" ( ")

            '/////////////////////////////////////////////////////////////////////////////////////////////
            '//////////// RECUPERA AUDIT MONITORAGGI                       ///////////////////////////////
            '/////////////////////////////////////////////////////////////////////////////////////////////

            StbQuery.Append(" SELECT DISTINCT Agenda.Piva  " & vbCrLf)
            StbQuery.Append(" , Agenda.Sa_Cod " & vbCrLf)
            StbQuery.Append(" , Agenda.Id_Agenda " & vbCrLf)
            StbQuery.Append(" , Agenda.Lav_Cod  " & vbCrLf)
            StbQuery.Append(" , Agenda.Des_Lib " & vbCrLf)
            StbQuery.Append(" , Agenda.validita_inizio as Data_Movimento " & vbCrLf)
            StbQuery.Append(" , '' as Mov_Desc " & vbCrLf)
            StbQuery.Append(" , Agenda.Username_Creazione " & vbCrLf)
            StbQuery.Append(" , '' as Cau_Mov " & vbCrLf)
            StbQuery.Append(" , 0 AS Cul_Cod " & vbCrLf)
            StbQuery.Append(" , 0 AS Veg_Cod " & vbCrLf)
            StbQuery.Append(" , '' AS Veg_Des " & vbCrLf)
            StbQuery.Append(" , Agenda.Blocco_Flag " & vbCrLf)
            StbQuery.Append(" , (Dettagli.Cognome + ' ' + Dettagli.Nome) AS Tecnico " & vbCrLf)
            StbQuery.Append(" , -1 AS Tipo_Destinazione " & vbCrLf)
            StbQuery.Append(" , 0 AS Appezza " & vbCrLf)
            StbQuery.Append(" , '' AS App_Nome " & vbCrLf)
            StbQuery.Append(" , 0 AS Elem_Cod " & vbCrLf)
            StbQuery.Append(" , 0 AS Mat_Cod " & vbCrLf)
            StbQuery.Append(" , 0 AS Pro_Cod " & vbCrLf)
            StbQuery.Append(" , '' AS Fr_Des " & vbCrLf)
            StbQuery.Append(" , '' AS Fer_Des " & vbCrLf)
            StbQuery.Append(" , '' AS Trap_Des " & vbCrLf)
            StbQuery.Append(" , '' AS Mat_Des " & vbCrLf)
            StbQuery.Append(" , '' AS Cod_Articolo " & vbCrLf)
            StbQuery.Append(" , 0 AS Ricetta_Cod  " & vbCrLf)
            StbQuery.Append(" , ISNULL(( SELECT TOP 1 Centri_Aziendali.sa_nome FROM  Centri_Aziendali WHERE Centri_Aziendali.sa_cod = Agenda.Sa_Cod and  Centri_Aziendali.piva = Agenda.piva  )  , '') AS sa_nome  " & vbCrLf)

            'modifica per avversita
            StbQuery.Append(" , null AS av_Des_Vol " & vbCrLf)
            StbQuery.Append(" , null AS av_gru_Des  " & vbCrLf)
            StbQuery.Append(" , 0 AS Id_Mov  " & vbCrLf)


            StbQuery.Append(" FROM " & NomeDB_Utenti & ".dbo.Utenti_Dettagli Dettagli  " & vbCrLf)
            StbQuery.Append("    INNER JOIN Agenda  " & vbCrLf)
            StbQuery.Append("        ON  Dettagli.CodFisc = Agenda.Username_Creazione  " & vbCrLf)

            'Modifica per gruppi operazioni e tipo
            StbQuery.Append("    LEFT OUTER JOIN dbo.Operazioni  " & vbCrLf)
            StbQuery.Append("        on Agenda.Lav_Cod = Operazioni.Lav_Cod  " & vbCrLf)
            StbQuery.Append("    LEFT OUTER JOIN dbo.GruppoOperazioni  " & vbCrLf)
            StbQuery.Append("        on Operazioni.GRU_OP = GruppoOperazioni.GRU_COD  " & vbCrLf)

            StbQuery.Append("    inner join imprese i " & vbCrLf)
            StbQuery.Append("        on i.piva = Agenda.PIVA " & vbCrLf)

            StbQuery.Append(" WHERE Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "'   " & vbCrLf)

            '--------------------------

            If Filtro_GruppoOperazioni <> "" Then
                StbQuery.Append(" AND ( " & Filtro_GruppoOperazioni & " )  " & vbCrLf)
            End If

            If FiltroOperazioni <> "" Then
                StbQuery.Append(" AND " & Agro_SQL_SaveText(FiltroOperazioni) & "   " & vbCrLf)
            End If

            If Tipo <> "" Then
                StbQuery.Append(" AND GruppoOperazioni.Tipo = '" & Agro_SQL_SaveText(Tipo) & "'   " & vbCrLf)
            End If

            If Gru_Cod <> 0 Then
                StbQuery.Append(" AND GruppoOperazioni.GRU_COD = " & Agro_SQL_SaveNum(Gru_Cod) & "   " & vbCrLf)
            End If

            '--------------------------

            StbQuery.Append(" AND (  " & vbCrLf)
            StbQuery.Append(" Agenda.Lav_Cod IN ( " & CStr(LAVCOD_PRATICA_ECOLOGICA) & "," & CStr(LAVCOD_FORMAZIONE) & "," & CStr(LAVCOD_GESTIONE_RIFIUTI) & ") " & vbCrLf)
            StbQuery.Append("  ) " & vbCrLf)

            StbQuery.Append(" AND Agenda.validita_inizio >= " & Agro_SQL_SaveDate(Validita_Inizio) & "   " & vbCrLf)
            StbQuery.Append(" AND Agenda.validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & "   " & vbCrLf)

            StbQuery.Append(" AND Agenda.validita_inizio >= " & Agro_SQL_SaveDate(CDate(CDate(objParametri_Server.FinestraTemporaleInizio).ToShortDateString)) & "   " & vbCrLf)
            StbQuery.Append(" AND Agenda.validita_inizio <= " & Agro_SQL_SaveDate(CDate(CDate(objParametri_Server.FinestraTemporaleFine).ToShortDateString)) & "   " & vbCrLf)

            StbQuery.Append(" ) ")



            StbQuery.Append(" ORDER BY Movimenti.Data_Movimento DESC, Agenda.Id_Agenda DESC, App_Nome ASC")


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Server, StbQuery.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function



    '##############################################################################################
    <CacheableAttribute(False)>
    Public Function Leggi_x_Grid_Agenda_BS_Fast_Senza_Avversita(
                            ByVal Piva As String,
                            ByVal Sa_Cod As Integer,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                            ByVal Veg_Cod As Integer,
                            ByVal Cul_Cod As Integer,
                            ByVal Tipo As String,
                            ByVal Gru_Cod As Integer,
                            ByVal Lav_Cod As Integer,
                            ByVal Flag_TerrenoNudo As Boolean,
                            ByVal Flag_Anche_Veg_Cod_Cul_Cod_0 As Boolean,
                            ByVal FiltroOperazioni As String,
                            ByVal FiltroImpianti As String,
                            ByVal Filtro_GruppoOperazioni As String,
                            ByVal xFiltroAggiuntivo_colturali As String,
                            ByVal xFiltroAggiuntivo_postRaccolta As String,
                            ByVal xFiltroAggiuntivo_contabili As String,
                            ByVal xFiltroAggiuntivo_contabili_Macchine As String,
                            ByVal xFiltroAggiuntivo_contabili_Audit As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri_Utenti As AgronicaCoreParametri,
                            ByRef objParametri_Server As AgronicaCoreParametri,
                            Optional ByVal FF_TrackedData_Cod As Integer = -1,
                            Optional ByVal FF_FromOutToIn As Boolean = True,
                            Optional ByVal Visualizza_Codici_AppezzaImpianti As Boolean = False,
                            Optional ByVal Visualizza_KPIN_BlockName As Boolean = False,
                            Optional ByVal id_cod As Integer = 0,
                            Optional ByVal filtroAgende As List(Of Integer) = Nothing,
                            Optional ByVal numeroDiRigheDaEstrarre As Integer? = Nothing,
                            Optional ByVal Filtro_ElemCod As String = "",
                            Optional ByVal xFiltroAggiuntivo_Visite As String = ""
                            ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Operazioni_R.Leggi_x_Grid_Agenda_BS_Fast_Senza_Avversita()"

        Dim messaggioErrore As String = ""
        Dim Stb As New Text.StringBuilder
        Dim dt As DataTable

        Dim flagConnLocale As Boolean
        Dim flagTransLocale As Boolean

        Try
            Dim NomeDB_Utenti As String = objParametri_Utenti.StringaConnessione.Split(";")(2).Split("=")(1)
            Dim FiltroCentri As String = ""

            If Sa_Cod = 0 Then
                'leggo se ci sono filtri sui centri
                FiltroCentri = EstraiFiltroCentri(Piva, objParametri_Server)
            End If

            Stb.Length = 0

#Region "CTE che filtra le Agende per Piva"
            Stb.AppendLine(" ;with #cte_Agenda_Per_Piva AS ( ")
            If FF_TrackedData_Cod > 0 Then
                Stb.AppendLine("            select * from Agenda where Piva in (select distinct piva from FF_TrackedData_Agenda where FF_TrackedData_Cod=" & Agro_SQL_SaveNum(FF_TrackedData_Cod) & " ) ")
            Else
                Stb.AppendLine("            select * from Agenda where Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If
            Stb.AppendLine(" ), ")
#End Region

#Region "CTE che filtra gli Utenti_Dettagli"
            Stb.AppendLine(" #cte_Utenti_Dettagli AS ( ")
            Stb.AppendLine("             select distinct CodFisc, Nome,Cognome from " & NomeDB_Utenti & ".dbo.Utenti_Dettagli AS Dettagli ")
            Stb.AppendLine("             INNER JOIN #cte_Agenda_Per_Piva on ")
            Stb.AppendLine("             Dettagli.CodFisc = #cte_Agenda_Per_Piva.Username_Creazione")
            Stb.AppendLine(" ), ")
#End Region

#Region "CTE che estrae la specie delle operazioni su Prodotti Magazzino"
            Stb.AppendLine(" #cte_Specie_ProdottiDaTrattare AS ( ")
            Stb.AppendLine("        SELECT DISTINCT Agenda.Id_Agenda, Materie_Prime.Mat_Cod, Materie_Prime.Cul_Cod, SpecieVegetali.Veg_Cod, SpecieVegetali.Veg_Des ")
            Stb.AppendLine("        FROM Movimenti_Dettagli ")
            Stb.AppendLine("        INNER JOIN #cte_Agenda_Per_Piva Agenda ON Agenda.Id_Agenda = Movimenti_dettagli.Id_Agenda ")
            Stb.AppendLine("        INNER JOIN Materie_Prime ON Movimenti_Dettagli.Mat_Cod = Materie_Prime.Mat_Cod ")
            Stb.AppendLine("        INNER JOIN Cultivar ON Materie_Prime.Veg_Cod = Cultivar.Veg_Cod AND Materie_Prime.Cul_Cod = Cultivar.Cul_Cod ")
            Stb.AppendLine("        INNER JOIN SpecieVegetali ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ")
            Stb.AppendLine("        WHERE Agenda.Lav_Cod IN (" & LAVCOD_TRATTAMENTO_POST_RACCOLTA & ", " & LAVCOD_CONCIA_SEME & ") ")
            Stb.AppendLine(" ), ")
            Stb.AppendLine(" #cte_Cultivar_ProdottiDaTrattare AS ( ")
            Stb.AppendLine("        SELECT DISTINCT Id_Agenda, Materie_Prime.Mat_Cod, Cultivar.Veg_Cod, Cultivar.Cul_Cod, Cultivar.Cul_Des ")
            Stb.AppendLine("        FROM #cte_Specie_ProdottiDaTrattare ")
            Stb.AppendLine("        INNER JOIN Materie_Prime ON #cte_Specie_ProdottiDaTrattare.Mat_Cod = Materie_Prime.Mat_Cod ")
            Stb.AppendLine("        INNER JOIN Cultivar ON Materie_Prime.Veg_Cod = Cultivar.Veg_Cod AND Materie_Prime.Cul_Cod = Cultivar.Cul_Cod ")
            Stb.AppendLine(" ), ")
#End Region

#Region "CTE con query Menu Agenda"
            Stb.AppendLine(" #cte AS (( ")

#Region "LA PRIMA RECUPERA LE OP COLTURALI CON I RELATIVI IMPIANTI E PRODOTTI IMPIEGATI"
            '/////////////////////////////////////////////////////////////////////////////////////////////
            '///// LA PRIMA RECUPERA LE OP COLTURALI CON I RELATIVI IMPIANTI E PRODOTTI IMPIEGATI ////////
            '/////////////////////////////////////////////////////////////////////////////////////////////

            Stb.AppendLine("--LA PRIMA RECUPERA LE OP COLTURALI CON I RELATIVI IMPIANTI E PRODOTTI IMPIEGATI")

            Stb.AppendLine(" SELECT Agenda.Piva ")
            Stb.AppendLine("      , Agenda.Sa_Cod ")
            Stb.AppendLine("      , Agenda.Id_Agenda ")
            Stb.AppendLine("      , Movimenti_dettagli.id_Mov ")
            Stb.AppendLine("      , Movimenti_dettagli.Id_Mov_Det ")
            Stb.AppendLine("      , Agenda.Lav_Cod ")
            Stb.AppendLine("      , Agenda.Des_Lib ")
            Stb.AppendLine("      , Movimenti.Data_Movimento ")
            Stb.AppendLine("      , Movimenti.Ora ")
            Stb.AppendLine("      , Movimenti.Mov_Desc ")
            Stb.AppendLine("      , Agenda.Username_Creazione ")
            Stb.AppendLine("      , Movimenti.Cau_Mov ")
            Stb.AppendLine("      , ISNULL(Reg_Impianti.Cul_Cod, 0) AS Cul_Cod ")
            Stb.AppendLine("      , ISNULL(Cultivar.Veg_Cod, 0) AS Veg_Cod ")
            Stb.AppendLine("      , ISNULL(SpecieVegetali.Veg_Des, '') AS Veg_Des ")
            Stb.AppendLine("      , Agenda.Blocco_Flag ")
            Stb.AppendLine("      , Agenda.Raccoglitore_Cod ")
            Stb.AppendLine("      , ISNULL((Dettagli.Cognome + ' ' + Dettagli.Nome), 'N.D.') AS Tecnico ")

            '(26/11/2024 AF) aggiunto Id_Destinazione
            Stb.AppendLine("      , ISNULL(Mov_Destinazioni.Id_Destinazione, -1) AS Id_Destinazione ")

            Stb.AppendLine("      , ISNULL(Mov_Destinazioni.Tipo_Destinazione, -1) AS Tipo_Destinazione ")
            If FF_TrackedData_Cod > 0 Then
                Stb.AppendLine("      , ISNULL(Fabbricati.Fabbricato_Des, ISNULL(Cantina_Vasche.Identificativo, '')) AS Fabbricato_Des ")
            End If
            Stb.AppendLine("      , ISNULL(Appezzamento.APPEZZA, 0) AS Appezza ")
            Stb.AppendLine("      , ISNULL(Appezzamento.APP_NOME, '') AS App_Nome ")
            Stb.AppendLine("      , ISNULL(Movimenti_dettagli.Elem_Cod, 0) AS Elem_Cod ")
            Stb.AppendLine("      , ISNULL(Movimenti_dettagli.Mat_Cod, 0) AS Mat_Cod ")
            Stb.AppendLine("      , ISNULL(Movimenti_dettagli.Pro_Cod, 0) AS Pro_Cod ")
            Stb.AppendLine("      , ISNULL(FORMULATI.Fr_Des, '') AS Fr_Des ")
            Stb.AppendLine("      , ISNULL(FERTILIZZANTI.Fer_Des, '') AS Fer_Des ")
            Stb.AppendLine("      , ISNULL(TRAPPOLE.TRAP_DES, '') AS Trap_Des ")
            Stb.AppendLine("      , ISNULL(InsettiUtili.Ins_Des, '') AS Ins_Des ")

            ' INIZIO modificato perché la JOIN che fa sopra su materie prime non aggancia correttamente la descrizione (query tutta da rivedere)
            If FF_TrackedData_Cod > 0 Then
                Stb.AppendLine("      , ISNULL(Materie_Prime.Mat_Des, ISNULL(Materie_Prime2.Mat_Des, '')) AS Mat_Des ")
                Stb.AppendLine("      , ISNULL(Materie_Prime.Cod_Articolo, ISNULL(Materie_Prime2.Cod_Articolo, '')) AS Cod_Articolo ")
            Else
                Stb.AppendLine("      , ISNULL(Materie_Prime.Mat_Des, '') AS Mat_Des ")
                Stb.AppendLine("      , ISNULL(Materie_Prime.Cod_Articolo, '') AS Cod_Articolo ")
            End If
            ' FINE modificato perché la JOIN che fa sopra su materie prime non aggancia correttamente la descrizione (query tutta da rivedere)

            Stb.AppendLine("      , ISNULL(Centri_Aziendali.Sa_Nome, '') AS sa_nome ")

            Stb.AppendLine("      , i.rag_soc ")
            Stb.AppendLine("      , Operazioni.lav_des ")
            Stb.AppendLine("      , GruppoOperazioni.gru_Des ")
            Stb.AppendLine("      , ISNULL(GruppoOperazioni.tipo, 'C') AS tipo ")

            '(20/12/2016 fede) aggiunte varieta
            Stb.AppendLine("      , ISNULL(Cultivar.Cul_des, '') AS cul_des ")

            '(03/05/2017 MarcoG) aggiunti principi attivi
            Stb.AppendLine("      , ISNULL(Movimenti_dettagli.PrincipiAttivi, '') AS PrincipiAttivi ")

            '(04/05/2017 MarcoG) aggiunti campi
            Stb.AppendLine("      , ISNULL(Campi.Campo_des, '') AS campo_des ")

            '(09/10/2017 MarcoG) aggiunto il campo contabilizzato per la pianificazione
            Stb.AppendLine("      , ISNULL(Movimenti_dettagli.Contabilizzato, 0) AS contabilizzato ")

            '(26/11/2024 AF) aggiunto il cod_progetto
            Stb.AppendLine("      , ISNULL(Movimenti_dettagli.Cod_Progetto, 0) AS Progetto_Cod ")

            '(12/10/2017 MarcoG) aggiunto il lotto
            Stb.AppendLine("      , REPLACE(ISNULL(Movimenti_dettagli.lotto,''),'''', '') AS LottoProduzione ")

            '(12/10/2017 MarcoG) aggiunto le note a checkbox
            Stb.AppendLine("      , ISNULL(Note_Intervento.Nota_Des,'') AS Nota_Des ")

            '(13/10/2017 MarcoG) aggiunto la sup app e la sup trattata
            Stb.AppendLine("      , ISNULL(Reg_Impianti.ID_REG, 0) AS ID_REG ")
            Stb.AppendLine("      , ISNULL(Reg_Impianti.sup_imp, 0) AS SUP_APP ")
            Stb.AppendLine("      , ISNULL(Mov_Destinazioni.Qta2, 0) AS Sup_Trattata ")

            '(27/10/2017 MarcoG) aggiunto il lotto impianto (nome distinta, esercizio)
            Stb.AppendLine("      , REPLACE(ISNULL(Imprese_Progetti.Progetto_Nome,''),'''', '') AS LottoImpianto ")

            '(23/11/2017 MarcoG) aggiunte attività per altre operazioni
            Stb.AppendLine("      , ISNULL(Attivita.Sigla, '') AS AttivitaSigla ")
            Stb.AppendLine("      , ISNULL(Attivita.[Desc], '') AS AttivitaDesc ")

            '(09/02/2018 MarcoG)  aggiunte le diciture per le destinazioni dei terreni nudi
            Stb.AppendLine("      , ISNULL(Reg_Impianti_Codici.id_cod, 0) AS DestinazioneTerreniNudi_Cod ")
            Stb.AppendLine("      , ISNULL(Codici_Anagrafe.descrizione, '') AS DestinazioneTerreniNudi_Des ")

            '(09/05/2018 MarcoG)  aggiunta la dicitura per il rif di DDT e Fatture 
            Stb.AppendLine("      , '' AS RifDdtFatture ")

            '(22/05/2018 MarcoG)  aggiunta la dicitura per le Ricette
            Stb.AppendLine("      , ISNULL(RicettexAgenda.Ricetta_Cod, 0) AS Ricetta_Cod ")
            Stb.AppendLine("      , ISNULL(Ricette.Ricetta_Numero, '') AS Ricetta_Numero ")

            '(23/08/2018 MarcoG)  aggiunta data di ultima modifica
            Stb.AppendLine("      , Agenda.Data_Modifica AS Data_Ultima_Modifica_Intervento ")

            '(05/12/2018 Fede)  aggiunta validita_inizio di Mov_Destinazioni per avere la Data Rilevata
            Stb.AppendLine("      , ISNULL(Mov_Destinazioni.validita_inizio, CONVERT(DateTime,'1900/01/01',120)) AS validita_inizio_destinazione ")

            '(11/01/2023 Anny)  aggiunte quantità e unità di misura
            Stb.AppendLine("      , ROUND(coalesce(Movimenti_dettagli.Qta_Extra_Totale, 0), +4) AS Qta_Extra_Totale")
            Stb.AppendLine("      , coalesce(Movimenti_dettagli.Qta, 0) AS Qta")
            Stb.AppendLine("      , Movimenti_dettagli.QTA_EXTRA")
            Stb.AppendLine("      , Movimenti_dettagli.Udm_Cod")
            Stb.AppendLine("      , Movimenti_dettagli.Extra_Int")
            Stb.AppendLine("      , coalesce(um1.UDM_SIM, '') AS UDM_SIM_ExtraInt")
            Stb.AppendLine("      , coalesce(um2.UDM_SIM, '') AS UDM_SIM_UDM_COD")
            Stb.AppendLine("      , Agenda.Data_Creazione AS Data_Creazione")
            '(12/02/2024 Anny)  aggiunta origine attività
            Stb.AppendLine("      , Agenda.Origine AS Origine")

            If FF_TrackedData_Cod > 0 Then
                Stb.AppendLine("      , ISNULL(ta.Lotto_Padre, '') AS FF_Track_Lotto_Padre ")
                Stb.AppendLine("      , ISNULL(ta.Lotto, '') AS FF_Track_Lotto ")
                Stb.AppendLine("      , ISNULL(ta.Cal_Cod_Padre, '') AS FF_Track_Cal_Cod_Padre ")
                Stb.AppendLine("      , ISNULL(ta.Cal_Cod, '') AS FF_Track_Cal_Cod ")
                Stb.AppendLine("      , ta.Qta_Extra_Totale AS FF_Track_Qta_Extra_Totale ")
                Stb.AppendLine("      , movimenti_dettagli.Qta_Dettaglio1 AS FF_Track_Qta_Contenitori ")
                Stb.AppendLine("      , movimenti_dettagli.Qta_Dettaglio2 AS FF_Track_Qta_Imballi ")
                Stb.AppendLine("      , ISNULL(Materie_Prime.Linea_Cod, 0) AS FF_Linea_Cod ")
                Stb.AppendLine("      , ISNULL(lp.codice_generazione, 0) AS FF_codice_generazione ")
            End If

            If Visualizza_Codici_AppezzaImpianti Then
                Stb.AppendLine("      , ISNULL(Riferimento_Alfanumerico_Appezzamento.Val_Cod, '') AS Riferimento_Alfanumerico_Appezzamento ")
                Stb.AppendLine("      , ISNULL(Codice_Impianto.Val_Cod, '') AS Codice_Impianto ")
            End If

            If Visualizza_KPIN_BlockName Then
                Stb.AppendLine("      , ISNULL(Zespri_Codice_kPIN.Val_Cod, '') AS Zespri_Codice_kPIN ")
                Stb.AppendLine("      , ISNULL(Zespri_Block_Name.Val_Cod, '') AS Zespri_Block_Name ")
            End If

            Stb.AppendLine(" FROM Movimenti_dettagli ")
            Stb.AppendLine("")
            Stb.AppendLine(" LEFT JOIN Formulati ")
            Stb.AppendLine(" ON Movimenti_dettagli.Pro_Cod = Formulati.Fr_Cod ")
            Stb.AppendLine("")
            Stb.AppendLine(" LEFT JOIN UnitaMisura um1 on Movimenti_dettagli.Extra_Int = um1.UDM_COD")
            Stb.AppendLine(" LEFT JOIN UnitaMisura um2 on Movimenti_dettagli.Udm_Cod = um2.UDM_COD")
            Stb.AppendLine(" LEFT JOIN Fertilizzanti ")
            Stb.AppendLine(" ON Movimenti_dettagli.Pro_Cod = Fertilizzanti.Fer_Cod ")
            Stb.AppendLine("")
            Stb.AppendLine(" LEFT JOIN Trappole ")
            Stb.AppendLine(" ON Movimenti_dettagli.Pro_Cod = Trappole.TRAP_COD ")
            Stb.AppendLine("")
            Stb.AppendLine(" LEFT JOIN InsettiUtili ")
            Stb.AppendLine(" ON Movimenti_dettagli.Pro_Cod = InsettiUtili.ins_cod ")
            Stb.AppendLine("")
            Stb.AppendLine(" LEFT JOIN Materie_Prime ")
            Stb.AppendLine(" ON Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod ")
            Stb.AppendLine(" AND  Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod ")
            Stb.AppendLine("")
            Stb.AppendLine(" RIGHT JOIN #cte_Utenti_Dettagli AS Dettagli ")
            Stb.AppendLine("")
            Stb.AppendLine(" RIGHT JOIN #cte_Agenda_Per_Piva AS Agenda ")
            Stb.AppendLine("")
            Stb.AppendLine(" INNER JOIN Movimenti ")
            Stb.AppendLine(" ON Agenda.Id_Agenda = Movimenti.Id_Agenda ")
            Stb.AppendLine(" ON Dettagli.CodFisc = Agenda.Username_Creazione ")

            Stb.AppendLine(" ON Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda ")
            Stb.AppendLine(" AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov ")

            Stb.AppendLine(" LEFT JOIN Centri_Aziendali ")
            Stb.AppendLine(" ON Centri_Aziendali.Piva = Agenda.PIVA ")
            Stb.AppendLine(" AND Centri_Aziendali.Sa_Cod = Agenda.Sa_Cod")


            Stb.AppendLine("LEFT JOIN Appezzamento ")
            Stb.AppendLine("")
            Stb.AppendLine(" INNER JOIN Reg_Impianti ")
            Stb.AppendLine("")
            Stb.AppendLine(" INNER JOIN Mov_Destinazioni ")
            Stb.AppendLine("")
            Stb.AppendLine(" ON Reg_Impianti.PIVA = Mov_Destinazioni.Piva ")
            Stb.AppendLine(" AND Reg_Impianti.SA_COD = Mov_Destinazioni.Sa_Cod ")
            Stb.AppendLine(" AND Reg_Impianti.APPEZZA = Mov_Destinazioni.Appezza ")
            Stb.AppendLine(" AND Reg_Impianti.ID_REG = Mov_Destinazioni.Id_Destinazione ")
            Stb.AppendLine("")

            '(12/10/2017 MarcoG) aggiunto per il lotto
            Stb.AppendLine(" LEFT JOIN Imprese_Progetti ")
            Stb.AppendLine(" ON Imprese_Progetti.Piva = Mov_Destinazioni.PIVA ")
            Stb.AppendLine(" AND Imprese_Progetti.Sa_Cod = Mov_Destinazioni.SA_COD ")
            Stb.AppendLine(" AND Imprese_Progetti.Appezza = Mov_Destinazioni.APPEZZA ")
            Stb.AppendLine(" AND Imprese_Progetti.Id_Reg = Mov_Destinazioni.id_destinazione ")
            Stb.AppendLine("")

            Stb.AppendLine(" ON Appezzamento.PIVA = Reg_Impianti.PIVA ")
            Stb.AppendLine(" AND Appezzamento.SA_COD = Reg_Impianti.SA_COD ")
            Stb.AppendLine(" AND Appezzamento.APPEZZA = Reg_Impianti.APPEZZA ")
            Stb.AppendLine("")

            Stb.AppendLine(" ON Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod ")
            Stb.AppendLine(" AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda ")
            Stb.AppendLine(" AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov ")
            Stb.AppendLine(" AND Movimenti_dettagli.Id_Mov_det = Mov_Destinazioni.Id_Mov_det ")

            Stb.AppendLine("")
            Stb.AppendLine(" LEFT JOIN Cultivar ")
            Stb.AppendLine(" ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod ")
            Stb.AppendLine("")
            Stb.AppendLine(" LEFT JOIN SpecieVegetali ")
            Stb.AppendLine(" ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ")

            'Modifica per gruppi operazioni e tipo
            Stb.AppendLine(" LEFT JOIN dbo.Operazioni ")
            Stb.AppendLine(" ON Agenda.Lav_Cod = Operazioni.Lav_Cod ")

            Stb.AppendLine(" LEFT JOIN dbo.GruppoOperazioni ")
            Stb.AppendLine(" ON Operazioni.GRU_OP = GruppoOperazioni.GRU_COD ")

            Stb.AppendLine(" INNER JOIN imprese i ")
            Stb.AppendLine(" ON i.piva = Agenda.PIVA ")

            '(04/05/2017 MarcoG) aggiunti campi
            Stb.AppendLine(" LEFT JOIN Campi ")
            Stb.AppendLine(" ON Campi.Piva = Appezzamento.Piva ")
            Stb.AppendLine(" AND Campi.Sa_Cod = Appezzamento.Sa_Cod ")
            Stb.AppendLine(" AND Campi.Campo_Cod = Appezzamento.Campo_Cod ")

            '(12/10/2017 MarcoG)  aggiunte note testuali e a checkbox
            Stb.AppendLine(" LEFT JOIN AgendaxNote ")
            Stb.AppendLine(" ON Agenda.Id_Agenda = AgendaxNote.Id_Agenda ")
            Stb.AppendLine(" LEFT JOIN Note_Intervento ")
            Stb.AppendLine(" ON AgendaxNote.Nota_Cod = Note_Intervento.Nota_Cod ")

            '(23/11/2017 MarcoG)  aggiunte attività per altre operazioni
            Stb.AppendLine(" LEFT JOIN Attivita ")
            Stb.AppendLine(" ON Agenda.Id_Attivita = Attivita.Id_Attivita ")

            '(09/02/2018 MarcoG)  aggiunte le diciture per le destinazioni dei terreni nudi
            Stb.AppendLine(" LEFT JOIN Reg_Impianti_Codici ")
            Stb.AppendLine(" ON Reg_Impianti.piva = Reg_Impianti_Codici.piva ")
            Stb.AppendLine(" AND Reg_Impianti.SA_COD = Reg_Impianti_Codici.sa_cod ")
            Stb.AppendLine(" AND Reg_Impianti.APPEZZA = Reg_Impianti_Codici.appezza ")
            Stb.AppendLine(" AND Reg_Impianti.ID_REG = Reg_Impianti_Codici.Id_Reg ")
            Stb.AppendLine(" AND Reg_Impianti.Cul_Cod=0 ")
            Stb.AppendLine(" AND Reg_Impianti_Codici.id_cod BETWEEN 3000 AND 3999 ")
            Stb.AppendLine(" LEFT JOIN Codici_Anagrafe ")
            Stb.AppendLine(" ON Codici_Anagrafe.codice = Reg_Impianti_Codici.id_cod ")

            '(22/05/2018 MarcoG)  aggiunte le diciture per le ricette
            Stb.AppendLine(" LEFT JOIN RicettexAgenda ")
            Stb.AppendLine(" ON RicettexAgenda.id_agenda = Agenda.id_agenda AND RicettexAgenda.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri_Server.PivaSuperUser) & "'")
            Stb.AppendLine(" LEFT JOIN Ricette ")
            Stb.AppendLine(" ON RicettexAgenda.Ricetta_SuperUser = Ricette.Ricetta_SuperUser AND RicettexAgenda.Ricetta_Cod = Ricette.Ricetta_Cod ")

            If Visualizza_Codici_AppezzaImpianti Then
                Stb.AppendLine(" LEFT JOIN Appezzamento_Codici Riferimento_Alfanumerico_Appezzamento ")
                Stb.AppendLine(" ON Riferimento_Alfanumerico_Appezzamento.Piva = Appezzamento.Piva AND Riferimento_Alfanumerico_Appezzamento.Sa_Cod = Appezzamento.Sa_Cod AND Riferimento_Alfanumerico_Appezzamento.Appezza = Appezzamento.Appezza AND Riferimento_Alfanumerico_Appezzamento.ID_COD = " & enum_CodiciAnagrafe.Riferimento_Alfanumerico_Appezzamento & " ")

                Stb.AppendLine(" LEFT JOIN Reg_Impianti_Codici Codice_Impianto ")
                Stb.AppendLine(" ON Reg_Impianti.piva = Codice_Impianto.piva ")
                Stb.AppendLine(" AND Reg_Impianti.SA_COD = Codice_Impianto.sa_cod ")
                Stb.AppendLine(" AND Reg_Impianti.APPEZZA = Codice_Impianto.appezza ")
                Stb.AppendLine(" AND Reg_Impianti.ID_REG = Codice_Impianto.Id_Reg ")
                Stb.AppendLine(" AND Codice_Impianto.Progetto_Cod = 0 ")
                Stb.AppendLine(" AND Codice_Impianto.ID_Cod = " & enum_CodiciAnagrafe.Codice_Impianto & " ")

            End If

            If Visualizza_KPIN_BlockName Then

                Stb.AppendLine(" LEFT JOIN Reg_Impianti_Codici Zespri_Codice_kPIN ")
                Stb.AppendLine(" ON Reg_Impianti.piva = Zespri_Codice_kPIN.piva ")
                Stb.AppendLine(" AND Reg_Impianti.SA_COD = Zespri_Codice_kPIN.sa_cod ")
                Stb.AppendLine(" AND Reg_Impianti.APPEZZA = Zespri_Codice_kPIN.appezza ")
                Stb.AppendLine(" AND Reg_Impianti.ID_REG = Zespri_Codice_kPIN.Id_Reg ")
                Stb.AppendLine(" AND Zespri_Codice_kPIN.Progetto_Cod = Imprese_progetti.Progetto_Cod ")
                Stb.AppendLine(" AND Zespri_Codice_kPIN.ID_Cod = " & enum_CodiciAnagrafe.Zespri_Codice_kPIN & " ")

                Stb.AppendLine(" LEFT JOIN Reg_Impianti_Codici Zespri_Block_Name ")
                Stb.AppendLine(" ON Reg_Impianti.piva = Zespri_Block_Name.piva ")
                Stb.AppendLine(" AND Reg_Impianti.SA_COD = Zespri_Block_Name.sa_cod ")
                Stb.AppendLine(" AND Reg_Impianti.APPEZZA = Zespri_Block_Name.appezza ")
                Stb.AppendLine(" AND Reg_Impianti.ID_REG = Zespri_Block_Name.Id_Reg ")
                Stb.AppendLine(" AND Zespri_Block_Name.Progetto_Cod = Imprese_progetti.Progetto_Cod ")
                Stb.AppendLine(" AND Zespri_Block_Name.ID_Cod = " & enum_CodiciAnagrafe.Zespri_Block_Name & " ")

            End If

            If FF_TrackedData_Cod > 0 Then
                ' INIZIO aggiunta perché la JOIN che fa sopra su materie prime non aggancia correttamente la descrizione (query tutta da rivedere)
                Stb.AppendLine(" LEFT JOIN Materie_Prime  AS Materie_Prime2 ")
                Stb.AppendLine(" ON (movimenti_dettagli.piva = Materie_Prime2.piva Or Materie_Prime2.sa_cod = -1) ")
                Stb.AppendLine(" AND movimenti_dettagli.Elem_COD = Materie_Prime2.Elem_COD ")
                Stb.AppendLine(" AND movimenti_dettagli.mat_COD = Materie_Prime2.mat_COD ")
                ' FINE aggiunta perché la JOIN che fa sopra su materie prime non aggancia correttamente la descrizione (query tutta da rivedere)

                'INIZIO Aggiunto per avere la descrizione del magazzino / vasca
                Stb.AppendLine(" LEFT JOIN Mov_Destinazioni AS Mov_Destinazioni2 ")
                Stb.AppendLine(" ON Mov_Destinazioni2.Piva = Movimenti_Dettagli.PIVA")
                Stb.AppendLine(" AND Mov_Destinazioni2.Sa_Cod = Movimenti_Dettagli.Sa_Cod ")
                Stb.AppendLine(" AND Mov_Destinazioni2.Id_Agenda = Movimenti_Dettagli.Id_Agenda ")
                Stb.AppendLine(" AND Mov_Destinazioni2.Id_Mov = Movimenti_Dettagli.Id_Mov ")
                Stb.AppendLine(" AND Mov_Destinazioni2.Id_Mov_Det = Movimenti_Dettagli.Id_Mov_Det ")

                Stb.AppendLine(" LEFT JOIN fabbricati ")
                Stb.AppendLine(" ON fabbricati.PIVA = Mov_Destinazioni2.piva")
                Stb.AppendLine(" AND fabbricati.Sa_Cod = Mov_Destinazioni2.Sa_Cod")
                Stb.AppendLine(" AND fabbricati.Fabbricato_Cod = Mov_Destinazioni2.Id_destinazione")
                Stb.AppendLine(" AND Mov_Destinazioni2.Tipo_Destinazione = " & MAGAZZINO & " ")

                Stb.AppendLine(" LEFT JOIN Cantina_Vasche ")
                Stb.AppendLine(" ON Cantina_Vasche.PIVA = Mov_Destinazioni2.piva")
                Stb.AppendLine(" AND Cantina_Vasche.Sa_Cod = Mov_Destinazioni2.Sa_Cod")
                Stb.AppendLine(" AND Cantina_Vasche.vas_Cod = Mov_Destinazioni2.Id_destinazione")
                Stb.AppendLine(" AND Mov_Destinazioni2.Tipo_Destinazione = " & VASCA_ENOLOGICA & " ")
                'FINE Aggiunto per avere la descrizione del magazzino / vasca

                Stb.AppendLine(" INNER JOIN FF_TrackedData_Agenda ta ")
                Stb.AppendLine(" ON Movimenti_dettagli.PIVA = ta.piva")
                Stb.AppendLine(" AND Movimenti_dettagli.Sa_Cod = ta.sa_cod")
                Stb.AppendLine(" AND Movimenti_dettagli.Id_Agenda = ta.id_Agenda ")
                Stb.AppendLine(" AND Movimenti_dettagli.Id_Mov = ta.id_Mov ")
                Stb.AppendLine(" AND Movimenti_dettagli.Id_Mov_Det = ta.id_Mov_det ")

                Stb.AppendLine(" LEFT JOIN Linee_Preparazioni lp ")
                Stb.AppendLine(" ON agenda.PREPARAZIONE_COD = lp.Preparazione_Cod ")

                Stb.AppendLine(" WHERE ta.FF_TrackedData_Cod = " & Agro_SQL_SaveNum(FF_TrackedData_Cod))
                Stb.AppendLine(" AND ta.Visibile = 1")  'per compatibilità con visualizzazione a grafo

            Else

                Stb.AppendLine(" WHERE 1 = 1")

                '--------------------------
                'modifica per magazzino e contabilita
                If Sa_Cod <> 0 Then
                    Stb.AppendLine(" AND Agenda.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
                Else
                    If FiltroCentri <> "" Then
                        Stb.AppendLine(" AND Agenda.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ")
                    End If
                End If
                'If Sa_Cod <> 0 Then
                '    StbQuery.Append(" AND ( Agenda.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   or   Agenda.Sa_Cod =0 )   " )
                'End If
                '--------------------------
            End If


            If Filtro_GruppoOperazioni <> "" Then
                Stb.AppendLine(" AND ( " & Filtro_GruppoOperazioni & " ) ")
            End If

            If Filtro_ElemCod <> "" Then
                Stb.AppendLine(" AND ( " & Filtro_ElemCod & " ) ")
            End If

            If FiltroOperazioni <> "" Then
                Stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(FiltroOperazioni))
            End If

            If FiltroImpianti <> "" Then
                Stb.AppendLine(" AND " & Agro_SQL_SaveText(FiltroImpianti, False))
            End If

            If Tipo <> "" Then
                Stb.AppendLine(" AND GruppoOperazioni.Tipo = '" & Agro_SQL_SaveText(Tipo) & "'")
            End If

            If Gru_Cod <> 0 Then
                Stb.AppendLine(" AND GruppoOperazioni.GRU_COD = " & Agro_SQL_SaveNum(Gru_Cod))
            End If


            '--------------------------
            'modifica per magazzino e contabilita (1000,1001,1020,1021,1022,1023,1025,1031,1033
            'venivano escluse le operazioni contabili e di magazzino
            Stb.AppendLine(" AND (Agenda.Lav_Cod < " & LAVCOD_FATTURA_RICEVUTA & ") ")

            'AF Escludo i trattamenti post raccolta da questa select, perchè ne hanno una dedicata
            Stb.AppendLine(" AND Agenda.Lav_Cod NOT IN (" & LAVCOD_TRATTAMENTO_POST_RACCOLTA & ") ")

            'escludo le righe relative all'acqua delle fertirrigazioni
            Stb.AppendLine($" AND NOT(Agenda.Lav_Cod = {LAVCOD_FERTIRRIGAZIONE} AND Movimenti_dettagli.Elem_Cod = {ALTRE_MATERIE} AND Movimenti_dettagli.Mat_Cod = {MAT_COD_ACQUA_IRRIGAZIONE})")
            '--------------------------
            '--------------------------
            'modifica per magazzino e contabilita
            Stb.AppendLine(" AND Movimenti.Cau_Mov IN ('" & CAU_TRATTAMENTO & "','" & CAU_RILIEVO_CAMPO & "','" & CAU_RILIEVO_RACCOLTA & "','" & CAU_LAVORAZIONE & "') ")
            '--------------------------

            Stb.AppendLine(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio))
            Stb.AppendLine(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine))

            Stb.AppendLine(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleInizio))
            Stb.AppendLine(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine))

            Stb.AppendLine(" AND (Movimenti.Data_Movimento >= Imprese_progetti.validita_inizio OR Imprese_progetti.validita_inizio IS NULL)")
            Stb.AppendLine(" AND (Movimenti.Data_Movimento <= Imprese_progetti.validita_fine OR Imprese_progetti.validita_fine IS NULL) ")

            If Not Flag_TerrenoNudo Then
                If Flag_Anche_Veg_Cod_Cul_Cod_0 Then
                    If Veg_Cod <> 0 AndAlso Veg_Cod <> -1 Then
                        Stb.AppendLine(" AND ( Cultivar.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " )")
                    End If
                    If Cul_Cod <> 0 Then
                        Stb.AppendLine(" AND ( Reg_Impianti.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & " )")
                    End If
                Else
                    If Veg_Cod <> 0 AndAlso Veg_Cod <> -1 Then
                        Stb.AppendLine(" AND Cultivar.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod))
                    End If
                    If Cul_Cod <> 0 Then
                        Stb.AppendLine(" AND Reg_Impianti.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod))
                    End If
                End If
            Else
                If Flag_Anche_Veg_Cod_Cul_Cod_0 Then
                    Stb.AppendLine(" AND ( Reg_Impianti.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & " ) ")
                Else
                    Stb.AppendLine(" AND Reg_Impianti.Cul_Cod = 0 ")
                End If
            End If

            If id_cod <> 0 AndAlso id_cod <> -1 Then
                Stb.AppendLine(" AND Reg_Impianti_Codici.id_cod = " & Agro_SQL_SaveNum(id_cod) & " ")
            End If

            If xFiltroAggiuntivo_colturali <> "" Then
                Stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_colturali))
            End If
#End Region
            Stb.AppendLine(" ) ")

            If FF_TrackedData_Cod > 0 Then
                Stb.AppendLine(" UNION ")
            Else
                Stb.AppendLine(" UNION ALL ")
            End If

            Stb.AppendLine(" ( ")
#Region "LE OP COLTURALI POST RACCOLTA"
            '/////////////////////////////////////////////////////////////////////////////////////////////
            '///////////////// LE OP COLTURALI POST RACCOLTA //////////////
            '/////////////////////////////////////////////////////////////////////////////////////////////
            Stb.AppendLine("--LE OP COLTURALI POST RACCOLTA")

            Stb.AppendLine(" SELECT Agenda.Piva ")
            Stb.AppendLine("        , Agenda.Sa_Cod ")
            Stb.AppendLine("        , Agenda.Id_Agenda ")
            Stb.AppendLine("        , Movimenti_dettagli.Id_Mov ")
            Stb.AppendLine("        , Movimenti_dettagli.Id_Mov_Det ")
            Stb.AppendLine("        , Agenda.Lav_Cod ")
            Stb.AppendLine("        , Agenda.Des_Lib ")
            Stb.AppendLine("        , Movimenti.Data_Movimento ")
            Stb.AppendLine("        , Movimenti.Ora ")
            Stb.AppendLine("        , Movimenti.Mov_Desc ")
            Stb.AppendLine("        , Agenda.Username_Creazione ")
            Stb.AppendLine("        , Movimenti.Cau_Mov ")
            Stb.AppendLine("        , ISNULL(Cultivar.Cul_Cod, 0) AS Cul_Cod ")
            Stb.AppendLine("        , ISNULL(Cultivar.Veg_Cod, 0) AS Veg_Cod ")
            Stb.AppendLine("        , ISNULL(SpecieVegetali.Veg_Des, '') AS Veg_Des ")
            Stb.AppendLine("        , Agenda.Blocco_Flag ")
            Stb.AppendLine("        , Agenda.Raccoglitore_Cod ")
            Stb.AppendLine("        , ISNULL((Dettagli.Cognome + ' ' + Dettagli.Nome), 'N.D.') AS Tecnico ")

            '(26/11/2024 AF) aggiunto Id_Destinazione
            Stb.AppendLine(" , ISNULL(Mov_Destinazioni.Id_Destinazione, -1) AS Id_Destinazione ")

            Stb.AppendLine("        , ISNULL(Mov_Destinazioni.Tipo_Destinazione, -1) AS Tipo_Destinazione ")
            If FF_TrackedData_Cod > 0 Then
                Stb.AppendLine("        , ISNULL(Fabbricati.Fabbricato_Des, ISNULL(Cantina_Vasche.Identificativo, '')) AS Fabbricato_Des ")
            End If
            Stb.AppendLine("        , 0 AS Appezza ")
            Stb.AppendLine("        , '' AS App_Nome ")
            Stb.AppendLine("        , ISNULL(Movimenti_dettagli.Elem_Cod, 0) AS Elem_Cod ")
            Stb.AppendLine("        , ISNULL(Movimenti_dettagli.Mat_Cod, 0) AS Mat_Cod ")
            Stb.AppendLine("        , ISNULL(Movimenti_dettagli.Pro_Cod, 0) AS Pro_Cod ")
            Stb.AppendLine("        , ISNULL(FORMULATI.Fr_Des, '') AS Fr_Des ")
            Stb.AppendLine("        , '' AS Fer_Des ")
            Stb.AppendLine("        , '' AS Trap_Des ")
            Stb.AppendLine("        , '' AS Ins_Des ")
            ' INIZIO modificato perché la JOIN che fa sopra su materie prime non aggancia correttamente la descrizione (query tutta da rivedere)
            If FF_TrackedData_Cod > 0 Then
                Stb.AppendLine("        , ISNULL(Materie_Prime.Mat_Des, ISNULL(Materie_Prime2.Mat_Des, '')) AS Mat_Des ")
                Stb.AppendLine("        , ISNULL(Materie_Prime.Cod_Articolo, ISNULL(Materie_Prime2.Cod_Articolo, '')) AS Cod_Articolo ")
            Else
                Stb.AppendLine("        , ISNULL(Materie_Prime.Mat_Des, '') AS Mat_Des ")
                Stb.AppendLine("        , ISNULL(Materie_Prime.Cod_Articolo, '') AS Cod_Articolo ")
            End If
            ' FINE modificato perché la JOIN che fa sopra su materie prime non aggancia correttamente la descrizione (query tutta da rivedere)

            Stb.AppendLine("        , ISNULL(Centri_Aziendali.Sa_Nome")
            Stb.AppendLine("        , '') AS sa_nome ")

            'modifica per avversita
            'Stb.AppendLine(" , ISNULL(avversita.av_Des_Vol, '') AS av_Des_Vol ")
            'Stb.AppendLine(" , ISNULL(GruppoAvversita.av_gru_Des, '') AS av_gru_Des ")

            Stb.AppendLine("        , i.rag_soc ")
            Stb.AppendLine("        , Operazioni.lav_des ")
            Stb.AppendLine("        , GruppoOperazioni.gru_Des ")
            Stb.AppendLine("        , isnull( GruppoOperazioni.tipo, 'C' ) AS tipo ")

            '(20/12/2016 fede) aggiunte varieta
            Stb.AppendLine("        , ISNULL(Cultivar.Cul_des, '') AS cul_des ")

            '(03/05/2017 MarcoG) aggiunti principi attivi
            Stb.AppendLine("        , ISNULL(Movimenti_dettagli.PrincipiAttivi, '') AS PrincipiAttivi ")

            '(04/05/2017 MarcoG) aggiunti campi
            Stb.AppendLine("        , '' AS campo_des ")

            '(09/10/2017 MarcoG) aggiunto il campo contabilizzato per la pianificazione
            Stb.AppendLine("        , ISNULL(Movimenti_dettagli.Contabilizzato, 0) AS contabilizzato ")

            '(26/11/2024 AF) aggiunto il cod_progetto
            Stb.AppendLine(" , ISNULL(Movimenti_dettagli.Cod_Progetto, 0) AS Progetto_Cod ")

            '(12/10/2017 MarcoG) aggiunto il lotto
            Stb.AppendLine("        , REPLACE(ISNULL(Movimenti_dettagli.lotto,''),'''', '') AS LottoProduzione ")


            '(12/10/2017 MarcoG) aggiunto le note a checkbox
            Stb.AppendLine("        , ISNULL(Note_Intervento.Nota_Des,'') AS Nota_Des ")

            '(13/10/2017 MarcoG) aggiunto la sup app e la sup trattata
            Stb.AppendLine("        , ISNULL(Reg_Impianti.ID_REG, 0) AS ID_REG ")
            Stb.AppendLine("        , ISNULL(Reg_Impianti.sup_imp, 0) AS SUP_APP ")
            Stb.AppendLine("        , ISNULL(Mov_Destinazioni.Qta2, 0) AS Sup_Trattata ")

            '(27/10/2017 MarcoG) aggiunto il lotto impianto (nome distinta, esercizio)
            Stb.AppendLine("        ,REPLACE(ISNULL(Imprese_Progetti.Progetto_Nome, ''),'''', '')   AS LottoImpianto ")

            '(23/11/2017 MarcoG) aggiunte attività per altre operazioni
            Stb.AppendLine("        , '' AS AttivitaSigla ")
            Stb.AppendLine("        , '' AS AttivitaDesc ")

            '(09/02/2018 MarcoG)  aggiunte le diciture per le destinazioni dei terreni nudi
            Stb.AppendLine("        , ISNULL(Reg_Impianti_Codici.id_cod, 0) AS DestinazioneTerreniNudi_Cod ")
            Stb.AppendLine("        , ISNULL(Codici_Anagrafe.descrizione, '') AS DestinazioneTerreniNudi_Des ")

            '(09/05/2018 MarcoG)  aggiunta la dicitura per il rif di DDT e Fatture 
            Stb.AppendLine("        , '' AS RifDdtFatture ")

            '(22/05/2018 MarcoG)  aggiunta la dicitura per le Ricette
            Stb.AppendLine("        , ISNULL(RicettexAgenda.Ricetta_Cod, 0) AS Ricetta_Cod ")
            Stb.AppendLine("        , ISNULL(Ricette.Ricetta_Numero, '') AS Ricetta_Numero ")

            '(23/08/2018 MarcoG)  aggiunta data di ultima modifica
            Stb.AppendLine("        , Agenda.Data_Modifica AS Data_Ultima_Modifica_Intervento ")

            '(05/12/2018 Fede)  aggiunta validita_inizio di Mov_Destinazioni per avere la Data Rilevata
            Stb.AppendLine("        , ISNULL(Mov_Destinazioni.validita_inizio, CONVERT(DateTime,'1900/01/01',120)) AS validita_inizio_destinazione ")

            '(11/01/2023 Anny)  aggiunte quantità e unità di misura
            Stb.AppendLine("        , ROUND(coalesce(Movimenti_dettagli.Qta_Extra_Totale, 0), +4) AS Qta_Extra_Totale")
            Stb.AppendLine("        , coalesce(Movimenti_dettagli.Qta, 0) AS Qta")
            Stb.AppendLine("        , Movimenti_dettagli.QTA_EXTRA")
            Stb.AppendLine("        , Movimenti_dettagli.Udm_Cod")
            Stb.AppendLine("        , Movimenti_dettagli.Extra_Int")
            Stb.AppendLine("        , coalesce(um1.UDM_SIM, '') AS UDM_SIM_ExtraInt")
            Stb.AppendLine("        , coalesce(um2.UDM_SIM, '') AS UDM_SIM_UDM_COD")
            Stb.AppendLine("        , Agenda.Data_Creazione AS Data_Creazione")
            '(12/02/2024 Anny)  aggiunta origine attività
            Stb.AppendLine("        , Agenda.Origine AS Origine")

            If FF_TrackedData_Cod > 0 Then
                Stb.AppendLine("        , ISNULL(ta.Lotto_Padre, '') AS FF_Track_Lotto_Padre ")
                Stb.AppendLine("        , ISNULL(ta.Lotto, '') AS FF_Track_Lotto ")
                Stb.AppendLine("        , ISNULL(ta.Cal_Cod_Padre, '') AS FF_Track_Cal_Cod_Padre ")
                Stb.AppendLine("        , ISNULL(ta.Cal_Cod, '') AS FF_Track_Cal_Cod ")
                Stb.AppendLine("        , ta.Qta_Extra_Totale AS FF_Track_Qta_Extra_Totale ")
                Stb.AppendLine("        , movimenti_dettagli.Qta_Dettaglio1 AS FF_Track_Qta_Contenitori ")
                Stb.AppendLine("        , movimenti_dettagli.Qta_Dettaglio2 AS FF_Track_Qta_Imballi ")
                Stb.AppendLine("        , ISNULL(Materie_Prime.Linea_Cod, 0) AS FF_Linea_Cod ")
                Stb.AppendLine("        , ISNULL(lp.codice_generazione, 0) AS FF_codice_generazione ")
            End If

            If Visualizza_Codici_AppezzaImpianti Then
                Stb.AppendLine("        , '' AS Riferimento_Alfanumerico_Appezzamento ")
                Stb.AppendLine("        , '' AS Codice_Impianto ")
            End If

            If Visualizza_KPIN_BlockName Then
                Stb.AppendLine("        , '' AS Zespri_Codice_kPIN ")
                Stb.AppendLine("        , '' AS Zespri_Block_Name ")
            End If


            Stb.AppendLine(" FROM ")
            Stb.AppendLine("     GruppoOperazioni ")
            Stb.AppendLine("        RIGHT JOIN Mov_Destinazioni ")
            Stb.AppendLine("        RIGHT JOIN SpecieVegetali ")
            Stb.AppendLine("        INNER JOIN Materie_Prime ")
            Stb.AppendLine("            ON SpecieVegetali.Veg_Cod = Materie_Prime.Veg_Cod ")
            Stb.AppendLine("")
            Stb.AppendLine("        RIGHT JOIN Cultivar ")
            Stb.AppendLine("            ON Materie_Prime.Cul_Cod = Cultivar.Cul_Cod ")
            Stb.AppendLine("            AND SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ")
            Stb.AppendLine("")
            Stb.AppendLine("        RIGHT JOIN Movimenti_dettagli ")
            Stb.AppendLine("        LEFT JOIN UnitaMisura um1 on Movimenti_dettagli.Extra_Int = um1.UDM_COD")
            Stb.AppendLine("        LEFT JOIN UnitaMisura um2 on Movimenti_dettagli.Udm_Cod = um2.UDM_COD")
            Stb.AppendLine("        LEFT JOIN Formulati ")
            Stb.AppendLine("            ON Movimenti_dettagli.Pro_Cod = Formulati.Fr_Cod ")
            Stb.AppendLine("            ON Materie_Prime.Elem_Cod = Movimenti_dettagli.Elem_Cod ")
            Stb.AppendLine("            AND Materie_Prime.Mat_Cod = Movimenti_dettagli.Mat_Cod ")
            Stb.AppendLine("")
            Stb.AppendLine("        RIGHT JOIN #cte_Utenti_Dettagli AS Dettagli ")
            Stb.AppendLine("")
            Stb.AppendLine("        RIGHT JOIN #cte_Agenda_Per_Piva AS Agenda ")
            Stb.AppendLine("")
            Stb.AppendLine("        INNER JOIN Movimenti ")

            'vanni, verifica sa_cod
            'StbQuery.Append("           AND Agenda.Sa_Cod = Movimenti.Sa_Cod" )

            Stb.AppendLine("            ON Agenda.Id_Agenda = Movimenti.Id_Agenda ")
            Stb.AppendLine("            ON  Dettagli.CodFisc = Agenda.Username_Creazione ")

            'vanni, verifica sa_cod
            'StbQuery.Append("          AND Movimenti_dettagli.Sa_Cod = Movimenti.Sa_Cod " )

            Stb.AppendLine("            ON Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda ")
            Stb.AppendLine("            AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov ")
            Stb.AppendLine("            ON Mov_Destinazioni.Sa_Cod = Movimenti.Sa_Cod ")
            Stb.AppendLine("            AND Mov_Destinazioni.Id_Agenda = Movimenti.Id_Agenda ")
            Stb.AppendLine("            AND Mov_Destinazioni.Id_Mov = Movimenti.Id_Mov ")
            Stb.AppendLine("")

            Stb.AppendLine("        LEFT JOIN Centri_Aziendali ")
            Stb.AppendLine("        ON Centri_Aziendali.Piva = Agenda.PIVA ")
            Stb.AppendLine("        AND Centri_Aziendali.Sa_Cod = Agenda.Sa_Cod")

            '(13/10/2017 MarcoG) aggiunto per la superficie trattata
            Stb.AppendLine("            LEFT JOIN Reg_Impianti ")
            Stb.AppendLine("            ON Reg_Impianti.PIVA = Mov_Destinazioni.Piva ")
            Stb.AppendLine("            AND Reg_Impianti.SA_COD = Mov_Destinazioni.Sa_Cod ")
            Stb.AppendLine("            AND Reg_Impianti.APPEZZA = Mov_Destinazioni.Appezza ")
            Stb.AppendLine("            AND Reg_Impianti.ID_REG = Mov_Destinazioni.Id_Destinazione ")
            Stb.AppendLine("")

            '(12/10/2017 MarcoG) aggiunto per il lotto
            Stb.AppendLine("    LEFT JOIN Imprese_Progetti ")
            Stb.AppendLine("        ON Imprese_Progetti.Piva = Mov_Destinazioni.PIVA ")
            Stb.AppendLine("        AND Imprese_Progetti.Sa_Cod = Mov_Destinazioni.SA_COD ")
            Stb.AppendLine("        AND Imprese_Progetti.Appezza = Mov_Destinazioni.APPEZZA ")
            Stb.AppendLine("        AND Imprese_Progetti.Id_Reg = Mov_Destinazioni.id_destinazione ")
            Stb.AppendLine("")

            'Stb.AppendLine("        LEFT JOIN Mov_Dettaglio_Tecnico ")
            'Stb.AppendLine("            ON Movimenti.PIVA = Mov_Dettaglio_Tecnico.Piva ")
            'Stb.AppendLine("            AND Movimenti.Sa_Cod = Mov_Dettaglio_Tecnico.Sa_Cod ")
            'Stb.AppendLine("            AND Movimenti.Id_Agenda = Mov_Dettaglio_Tecnico.Id_Agenda ")
            'Stb.AppendLine("            AND Movimenti.Id_Mov = Mov_Dettaglio_Tecnico.Id_Mov ")
            'Stb.AppendLine("            AND ( ")
            'Stb.AppendLine("                Mov_Dettaglio_Tecnico.Av_Cod <> 0 ")
            'Stb.AppendLine("                OR Mov_Dettaglio_Tecnico.Av_Gru <> 0 ")
            'Stb.AppendLine("            ) ")
            'Stb.AppendLine("        LEFT JOIN Avversita ")
            'Stb.AppendLine("            ON Avversita.Av_Cod = Mov_Dettaglio_Tecnico.Av_Cod          ")
            'Stb.AppendLine("            AND Mov_Dettaglio_Tecnico.Av_Cod <> 0 ")
            'Stb.AppendLine("")
            'Stb.AppendLine("        LEFT JOIN GruppoAvversita ")
            'Stb.AppendLine("            ON GruppoAvversita.Av_Gru = Mov_Dettaglio_Tecnico.Av_Gru            ")
            'Stb.AppendLine("            AND Mov_Dettaglio_Tecnico.Av_Gru <> 0 ")
            Stb.AppendLine("")
            Stb.AppendLine("        LEFT JOIN Operazioni ")
            Stb.AppendLine("            ON Agenda.Lav_Cod = Operazioni.LAV_COD ")
            Stb.AppendLine("            ON GruppoOperazioni.GRU_COD = Operazioni.GRU_OP  ")

            Stb.AppendLine("    INNER JOIN imprese i ")
            Stb.AppendLine("        ON i.piva = Agenda.PIVA ")

            '(12/10/2017 MarcoG)  aggiunte note testuali e a checkbox
            Stb.AppendLine("   LEFT JOIN AgendaxNote ")
            Stb.AppendLine("   ON Agenda.Id_Agenda = AgendaxNote.Id_Agenda ")
            Stb.AppendLine("   LEFT JOIN Note_Intervento ")
            Stb.AppendLine("   ON AgendaxNote.Nota_Cod = Note_Intervento.Nota_Cod ")

            '(09/02/2018 MarcoG)  aggiunte le diciture per le destinazioni dei terreni nudi
            Stb.AppendLine("   LEFT JOIN Reg_Impianti_Codici ")
            Stb.AppendLine("   ON Reg_Impianti.piva = Reg_Impianti_Codici.piva ")
            Stb.AppendLine("   AND Reg_Impianti.SA_COD = Reg_Impianti_Codici.sa_cod ")
            Stb.AppendLine("   AND Reg_Impianti.APPEZZA = Reg_Impianti_Codici.appezza ")
            Stb.AppendLine("   AND Reg_Impianti.ID_REG = Reg_Impianti_Codici.Id_Reg ")
            Stb.AppendLine("   AND Reg_Impianti.Cul_Cod=0 ")
            Stb.AppendLine("   AND Reg_Impianti_Codici.id_cod BETWEEN 3000 AND 3999 ")
            Stb.AppendLine("   LEFT JOIN Codici_Anagrafe ")
            Stb.AppendLine("   ON Codici_Anagrafe.codice = Reg_Impianti_Codici.id_cod ")

            '(22/05/2018 MarcoG)  aggiunte le diciture per le ricette
            Stb.AppendLine("   LEFT JOIN RicettexAgenda ")
            Stb.AppendLine("   ON RicettexAgenda.id_agenda = Agenda.id_agenda AND RicettexAgenda.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri_Server.PivaSuperUser) & "'")
            Stb.AppendLine("   LEFT JOIN Ricette ")
            Stb.AppendLine("   ON RicettexAgenda.Ricetta_SuperUser = Ricette.Ricetta_SuperUser AND RicettexAgenda.Ricetta_Cod = Ricette.Ricetta_Cod ")



            If FF_TrackedData_Cod > 0 Then

                ' INIZIO aggiunta perché la JOIN che fa sopra su materie prime non aggancia correttamente la descrizione (query tutta da rivedere)
                Stb.AppendLine("  LEFT JOIN Materie_Prime  AS Materie_Prime2 ")
                Stb.AppendLine("  ON (movimenti_dettagli.piva = Materie_Prime2.piva Or Materie_Prime2.sa_cod = -1) ")
                Stb.AppendLine("  AND   movimenti_dettagli.Elem_COD = Materie_Prime2.Elem_COD ")
                Stb.AppendLine("  AND  movimenti_dettagli.mat_COD = Materie_Prime2.mat_COD ")
                ' FINE aggiunta perché la JOIN che fa sopra su materie prime non aggancia correttamente la descrizione (query tutta da rivedere)

                'INIZIO Aggiunto per avere la descrizione del magazzino / vasca
                Stb.AppendLine("   LEFT JOIN Mov_Destinazioni AS Mov_Destinazioni2 ")
                Stb.AppendLine("       ON Mov_Destinazioni2.Piva = Movimenti_Dettagli.PIVA")
                Stb.AppendLine("    AND Mov_Destinazioni2.Sa_Cod = Movimenti_Dettagli.Sa_Cod ")
                Stb.AppendLine("    AND Mov_Destinazioni2.Id_Agenda = Movimenti_Dettagli.Id_Agenda ")
                Stb.AppendLine("    AND Mov_Destinazioni2.Id_Mov = Movimenti_Dettagli.Id_Mov ")
                Stb.AppendLine("    AND Mov_Destinazioni2.Id_Mov_Det = Movimenti_Dettagli.Id_Mov_Det ")

                Stb.AppendLine("   LEFT JOIN fabbricati ")
                Stb.AppendLine("       ON fabbricati.PIVA = Mov_Destinazioni2.piva")
                Stb.AppendLine("       AND fabbricati.Sa_Cod = Mov_Destinazioni2.Sa_Cod")
                Stb.AppendLine("       AND fabbricati.Fabbricato_Cod = Mov_Destinazioni2.Id_destinazione")
                Stb.AppendLine("       AND Mov_Destinazioni2.Tipo_Destinazione = " & MAGAZZINO & " ")

                Stb.AppendLine("   LEFT JOIN Cantina_Vasche ")
                Stb.AppendLine("       ON Cantina_Vasche.PIVA = Mov_Destinazioni2.piva")
                Stb.AppendLine("       AND Cantina_Vasche.Sa_Cod = Mov_Destinazioni2.Sa_Cod")
                Stb.AppendLine("       AND Cantina_Vasche.vas_Cod = Mov_Destinazioni2.Id_destinazione")
                Stb.AppendLine("       AND Mov_Destinazioni2.Tipo_Destinazione = " & VASCA_ENOLOGICA & " ")
                'FINE Aggiunto per avere la descrizione del magazzino / vasca

                Stb.AppendLine("   INNER JOIN FF_TrackedData_Agenda ta ")
                Stb.AppendLine("       ON Movimenti_dettagli.PIVA = ta.piva ")
                Stb.AppendLine("       AND Movimenti_dettagli.Sa_Cod = ta.sa_cod ")
                Stb.AppendLine("       AND Movimenti_dettagli.Id_Agenda = ta.id_Agenda ")
                Stb.AppendLine("       AND Movimenti_dettagli.Id_Mov = ta.id_Mov ")
                Stb.AppendLine("       AND Movimenti_dettagli.Id_Mov_Det = ta.id_Mov_det ")

                Stb.AppendLine(" LEFT JOIN Linee_Preparazioni lp ")
                Stb.AppendLine("    ON agenda.PREPARAZIONE_COD = lp.Preparazione_Cod ")

                Stb.AppendLine(" WHERE ta.FF_TrackedData_Cod = " & Agro_SQL_SaveNum(FF_TrackedData_Cod))
                Stb.AppendLine(" AND ta.Visibile = 1")  'per compatibilità con visualizzazione a grafo

            Else
                Stb.AppendLine(" WHERE 1 = 1")

                '--------------------------
                'modifica per magazzino e contabilita
                If Sa_Cod <> 0 Then
                    Stb.AppendLine(" AND Agenda.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
                Else
                    If FiltroCentri <> "" Then
                        Stb.AppendLine(" AND Agenda.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ")
                    End If
                End If
                '--------------------------
            End If


            If Filtro_GruppoOperazioni <> "" Then
                Stb.AppendLine(" AND ( " & Filtro_GruppoOperazioni & " )")
            End If

            If Filtro_ElemCod <> "" Then
                Stb.AppendLine(" AND ( " & Filtro_ElemCod & " ) ")
            End If

            If FiltroOperazioni <> "" Then
                Stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(FiltroOperazioni))
            End If

            'ATTENZIONE, post raccolta non hanno impianti con specie collegato, quindi tolgo il filtro 
            If FiltroImpianti <> "" Then
                Dim filtroImpianti2 As String = FiltroImpianti.Replace("Reg_Impianti", "Mov_Destinazioni")
                'devo togliere  AND Reg_Impianti.Id_Reg in ( 74842113 )
                If filtroImpianti2.IndexOf("Mov_Destinazioni.Id_Reg") > 0 Then
                    filtroImpianti2 = filtroImpianti2.Replace("Mov_Destinazioni.Id_Reg", " (1=1) ")
                    filtroImpianti2 = filtroImpianti2.Remove(filtroImpianti2.Length - 30, 30)
                End If

                Stb.AppendLine(" AND " & Agro_SQL_SaveText(filtroImpianti2, False))
            End If

            If Tipo <> "" Then
                Stb.AppendLine(" AND GruppoOperazioni.Tipo = '" & Agro_SQL_SaveText(Tipo) & "'")
            End If

            If Gru_Cod <> 0 Then
                Stb.AppendLine(" AND GruppoOperazioni.GRU_COD = " & Agro_SQL_SaveNum(Gru_Cod))
            End If


            '--------------------------
            'venivano escluse le operazioni contabili e di magazzino
            'Modifica per operazione di cura
            Stb.AppendLine(" AND (Agenda.Lav_Cod = " & CStr(LAVCOD_CURA))
            Stb.AppendLine(" OR   Agenda.Lav_Cod = " & CStr(LAVCOD_TRASFORMAZIONI))
            Stb.AppendLine(" ) ")
            '--------------------------


            '--------------------------
            'modifica per magazzino e contabilita
            'Modifica per operazione di cura
            If FF_FromOutToIn Then
                Stb.AppendLine(" AND Movimenti.Cau_Mov IN ('" & CAU_TRATTAMENTO & "','" & CAU_RILIEVO_CAMPO & "','" & CAU_RILIEVO_RACCOLTA & "','" & CAU_LAVORAZIONE & "','" & CAU_LINEA_PRODUZIONE & "', '7350') ")
            Else
                Stb.AppendLine(" AND Movimenti.Cau_Mov IN ('" & CAU_TRATTAMENTO & "','" & CAU_RILIEVO_CAMPO & "','" & CAU_RILIEVO_RACCOLTA & "','" & CAU_LAVORAZIONE & "','" & CAU_LINEA_PRODUZIONE & "', '7300') ")
            End If
            '--------------------------

            Stb.AppendLine(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio))
            Stb.AppendLine(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine))

            Stb.AppendLine(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleInizio))
            Stb.AppendLine(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine))

            Stb.AppendLine(" AND (Movimenti.Data_Movimento >= Imprese_progetti.validita_inizio OR Imprese_progetti.validita_inizio IS NULL)")
            Stb.AppendLine(" AND (Movimenti.Data_Movimento <= Imprese_progetti.validita_fine OR Imprese_progetti.validita_fine IS NULL) ")

            If Veg_Cod <> 0 AndAlso Veg_Cod <> -1 Then
                Stb.AppendLine(" AND ( Cultivar.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ) ")
            End If
            If Cul_Cod <> 0 Then
                Stb.AppendLine(" AND ( Cultivar.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & " ) ")
            End If

            If id_cod <> 0 AndAlso id_cod <> -1 Then
                Stb.AppendLine(" AND Reg_Impianti_Codici.id_cod = " & Agro_SQL_SaveNum(id_cod) & " ")
            End If

            If xFiltroAggiuntivo_postRaccolta <> "" Then
                Stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_postRaccolta))
            End If
#End Region
            Stb.AppendLine(" ) ")

            If FF_TrackedData_Cod > 0 Then
                Stb.AppendLine(" UNION ")
            Else
                Stb.AppendLine(" UNION ALL ")
            End If

            Stb.AppendLine(" ( ")
#Region "LE OP COLTURALI SU PRODOTTI MAGAZZINO"
            '/////////////////////////////////////////////////////////////////////////////////////////////
            '///////////////// LE OP COLTURALI SU PRODOTTI MAGAZZINO //////////////
            '/////////////////////////////////////////////////////////////////////////////////////////////
            Stb.AppendLine("--LE OP COLTURALI SU PRODOTTI MAGAZZINO")

            Stb.AppendLine(" SELECT Agenda.Piva ")
            Stb.AppendLine("      , Agenda.Sa_Cod ")
            Stb.AppendLine("      , Agenda.Id_Agenda ")
            Stb.AppendLine("      , Movimenti_dettagli.Id_Mov ")
            Stb.AppendLine("      , Movimenti_dettagli.Id_Mov_Det ")
            Stb.AppendLine("      , Agenda.Lav_Cod ")
            Stb.AppendLine("      , Agenda.Des_Lib ")
            Stb.AppendLine("      , Movimenti.Data_Movimento ")
            Stb.AppendLine("      , Movimenti.Ora ")
            Stb.AppendLine("      , Movimenti.Mov_Desc ")
            Stb.AppendLine("      , Agenda.Username_Creazione ")
            Stb.AppendLine("      , Movimenti.Cau_Mov ")
            Stb.AppendLine("      , ISNULL(#cte_Specie_ProdottiDaTrattare.Cul_Cod, 0) AS Cul_Cod ")
            Stb.AppendLine("      , ISNULL(#cte_Specie_ProdottiDaTrattare.Veg_Cod, 0) AS Veg_Cod ")
            Stb.AppendLine("      , ISNULL(#cte_Specie_ProdottiDaTrattare.Veg_Des, '') AS Veg_Des ")
            Stb.AppendLine("      , Agenda.Blocco_Flag ")
            Stb.AppendLine("      , Agenda.Raccoglitore_Cod ")
            Stb.AppendLine("      , ISNULL((Dettagli.Cognome + ' ' + Dettagli.Nome), 'N.D.') AS Tecnico ")

            '(26/11/2024 AF) aggiunto Id_Destinazione
            Stb.AppendLine("      , ISNULL(Mov_Destinazioni.Id_Destinazione, -1) AS Id_Destinazione ")

            Stb.AppendLine("      , ISNULL(Mov_Destinazioni.Tipo_Destinazione, -1) AS Tipo_Destinazione ")
            If FF_TrackedData_Cod > 0 Then
                Stb.AppendLine("      , ISNULL(Fabbricati.Fabbricato_Des, ISNULL(Cantina_Vasche.Identificativo, '')) AS Fabbricato_Des ")
            End If
            Stb.AppendLine("      , 0 AS Appezza ")
            Stb.AppendLine("      , '' AS App_Nome ")
            Stb.AppendLine("      , ISNULL(Movimenti_dettagli.Elem_Cod, 0) AS Elem_Cod ")
            Stb.AppendLine("      , ISNULL(Movimenti_dettagli.Mat_Cod, 0) AS Mat_Cod ")
            Stb.AppendLine("      , ISNULL(Movimenti_dettagli.Pro_Cod, 0) AS Pro_Cod ")
            Stb.AppendLine("      , ISNULL(FORMULATI.Fr_Des, '') AS Fr_Des ")
            Stb.AppendLine("      , '' AS Fer_Des ")
            Stb.AppendLine("      , '' AS Trap_Des ")
            Stb.AppendLine("      , '' AS Ins_Des ")
            ' INIZIO modificato perché la JOIN che fa sopra su materie prime non aggancia correttamente la descrizione (query tutta da rivedere)
            If FF_TrackedData_Cod > 0 Then
                Stb.AppendLine("      , ISNULL(Materie_Prime.Mat_Des, ISNULL(Materie_Prime2.Mat_Des, '')) AS Mat_Des ")
                Stb.AppendLine("      , ISNULL(Materie_Prime.Cod_Articolo, ISNULL(Materie_Prime2.Cod_Articolo, '')) AS Cod_Articolo ")
            Else
                Stb.AppendLine("      , ISNULL(Materie_Prime.Mat_Des, '') AS Mat_Des ")
                Stb.AppendLine("      , ISNULL(Materie_Prime.Cod_Articolo, '') AS Cod_Articolo ")
            End If
            ' FINE modificato perché la JOIN che fa sopra su materie prime non aggancia correttamente la descrizione (query tutta da rivedere)

            Stb.AppendLine("      , ISNULL(Centri_Aziendali.Sa_Nome, '') AS sa_nome ")


            Stb.AppendLine("      , i.rag_soc ")
            Stb.AppendLine("      , Operazioni.lav_des ")
            Stb.AppendLine("      , GruppoOperazioni.gru_Des ")
            Stb.AppendLine("      , isnull( GruppoOperazioni.tipo, 'C' ) AS tipo ")

            '(20/12/2016 fede) aggiunte varieta
            Stb.AppendLine("      , ISNULL(#cte_Cultivar_ProdottiDaTrattare.Cul_des, '') AS cul_des ")

            '(03/05/2017 MarcoG) aggiunti principi attivi
            Stb.AppendLine("      , ISNULL(Movimenti_dettagli.PrincipiAttivi, '') AS PrincipiAttivi ")

            '(04/05/2017 MarcoG) aggiunti campi
            Stb.AppendLine("      , '' AS campo_des ")

            '(09/10/2017 MarcoG) aggiunto il campo contabilizzato per la pianificazione
            Stb.AppendLine("      , ISNULL(Movimenti_dettagli.Contabilizzato, 0) AS contabilizzato ")

            '(26/11/2024 AF) aggiunto il cod_progetto
            Stb.AppendLine("      , ISNULL(Movimenti_dettagli.Cod_Progetto, 0) AS Progetto_Cod ")

            '(12/10/2017 MarcoG) aggiunto il lotto
            Stb.AppendLine("      , REPLACE(ISNULL(Movimenti_dettagli.lotto, ''),'''', '') AS LottoProduzione ")


            '(12/10/2017 MarcoG) aggiunto le note a checkbox
            Stb.AppendLine("      , ISNULL(Note_Intervento.Nota_Des,'') AS Nota_Des ")

            '(13/10/2017 MarcoG) aggiunto la sup app e la sup trattata
            Stb.AppendLine("      , 0 AS ID_REG ")
            Stb.AppendLine("      , 0 AS SUP_APP ")
            Stb.AppendLine("      , ISNULL(Mov_Destinazioni.Qta2, 0) AS Sup_Trattata ")

            '(27/10/2017 MarcoG) aggiunto il lotto impianto (nome distinta, esercizio)
            Stb.AppendLine("      , '' AS LottoImpianto ")

            '(23/11/2017 MarcoG) aggiunte attività per altre operazioni
            Stb.AppendLine("      , '' AS AttivitaSigla ")
            Stb.AppendLine("      , '' AS AttivitaDesc ")

            '(09/02/2018 MarcoG)  aggiunte le diciture per le destinazioni dei terreni nudi
            Stb.AppendLine("      , 0 AS DestinazioneTerreniNudi_Cod ")
            Stb.AppendLine("      , '' AS DestinazioneTerreniNudi_Des ")

            '(09/05/2018 MarcoG)  aggiunta la dicitura per il rif di DDT e Fatture 
            Stb.AppendLine("      , '' AS RifDdtFatture ")

            '(22/05/2018 MarcoG)  aggiunta la dicitura per le Ricette
            Stb.AppendLine("      , ISNULL(RicettexAgenda.Ricetta_Cod, 0) AS Ricetta_Cod ")
            Stb.AppendLine("      , ISNULL(Ricette.Ricetta_Numero, '') AS Ricetta_Numero ")

            '(23/08/2018 MarcoG)  aggiunta data di ultima modifica
            Stb.AppendLine("      , Agenda.Data_Modifica AS Data_Ultima_Modifica_Intervento ")

            '(05/12/2018 Fede)  aggiunta validita_inizio di Mov_Destinazioni per avere la Data Rilevata
            Stb.AppendLine("      , ISNULL(Mov_Destinazioni.validita_inizio, CONVERT(DateTime,'1900/01/01',120)) AS validita_inizio_destinazione ")

            '(11/01/2023 Anny)  aggiunte quantità e unità di misura
            Stb.AppendLine("      , ROUND(coalesce(Movimenti_dettagli.Qta_Extra_Totale, 0), +4) AS Qta_Extra_Totale")
            Stb.AppendLine("      , coalesce(Movimenti_dettagli.Qta, 0) AS Qta")
            Stb.AppendLine("      , Movimenti_dettagli.QTA_EXTRA")
            Stb.AppendLine("      , Movimenti_dettagli.Udm_Cod")
            Stb.AppendLine("      , Movimenti_dettagli.Extra_Int")
            Stb.AppendLine("      , coalesce(um1.UDM_SIM, '') AS UDM_SIM_ExtraInt")
            Stb.AppendLine("      , coalesce(um2.UDM_SIM, '') AS UDM_SIM_UDM_COD")
            Stb.AppendLine("      , Agenda.Data_Creazione AS Data_Creazione")
            '(12/02/2024 Anny)  aggiunta origine attività
            Stb.AppendLine("      , Agenda.Origine AS Origine")

            If FF_TrackedData_Cod > 0 Then
                Stb.AppendLine("      , ISNULL(ta.Lotto_Padre, '') AS FF_Track_Lotto_Padre ")
                Stb.AppendLine("      , ISNULL(ta.Lotto, '') AS FF_Track_Lotto ")
                Stb.AppendLine("      , ISNULL(ta.Cal_Cod_Padre, '') AS FF_Track_Cal_Cod_Padre ")
                Stb.AppendLine("      , ISNULL(ta.Cal_Cod, '') AS FF_Track_Cal_Cod ")
                Stb.AppendLine("      , ta.Qta_Extra_Totale AS FF_Track_Qta_Extra_Totale ")
                Stb.AppendLine("      , movimenti_dettagli.Qta_Dettaglio1 AS FF_Track_Qta_Contenitori ")
                Stb.AppendLine("      , movimenti_dettagli.Qta_Dettaglio2 AS FF_Track_Qta_Imballi ")
                Stb.AppendLine("      , ISNULL(Materie_Prime.Linea_Cod, 0) AS FF_Linea_Cod ")
                Stb.AppendLine("      , ISNULL(lp.codice_generazione, 0) AS FF_codice_generazione ")
            End If

            If Visualizza_Codici_AppezzaImpianti Then
                Stb.AppendLine("      , '' AS Riferimento_Alfanumerico_Appezzamento ")
                Stb.AppendLine("      , '' AS Codice_Impianto ")
            End If

            If Visualizza_KPIN_BlockName Then
                Stb.AppendLine("      , '' AS Zespri_Codice_kPIN ")
                Stb.AppendLine("      , '' AS Zespri_Block_Name ")
            End If


            Stb.AppendLine(" FROM GruppoOperazioni ")
            Stb.AppendLine(" RIGHT JOIN Mov_Destinazioni ")
            Stb.AppendLine(" RIGHT JOIN Movimenti_dettagli ")
            Stb.AppendLine(" LEFT JOIN UnitaMisura um1 on Movimenti_dettagli.Extra_Int = um1.UDM_COD")
            Stb.AppendLine(" LEFT JOIN UnitaMisura um2 on Movimenti_dettagli.Udm_Cod = um2.UDM_COD")
            Stb.AppendLine(" LEFT JOIN Formulati ")
            Stb.AppendLine(" ON Movimenti_dettagli.Pro_Cod = Formulati.Fr_Cod ")
            Stb.AppendLine(" LEFT JOIN Materie_Prime ")
            Stb.AppendLine(" ON Materie_Prime.Elem_Cod = Movimenti_dettagli.Elem_Cod ")
            Stb.AppendLine(" AND Materie_Prime.Mat_Cod = Movimenti_dettagli.Mat_Cod ")

            Stb.AppendLine("")

            Stb.AppendLine(" RIGHT JOIN #cte_Utenti_Dettagli AS Dettagli ")
            Stb.AppendLine(" RIGHT JOIN #cte_Agenda_Per_Piva AS Agenda ")

            Stb.AppendLine("")

            Stb.AppendLine(" INNER JOIN Movimenti ")

            Stb.AppendLine(" ON Agenda.Id_Agenda = Movimenti.Id_Agenda ")
            Stb.AppendLine(" ON  Dettagli.CodFisc = Agenda.Username_Creazione ")
            Stb.AppendLine(" ON Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda ")
            Stb.AppendLine(" AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov ")
            Stb.AppendLine(" ON Mov_Destinazioni.Sa_Cod = Movimenti.Sa_Cod ")
            Stb.AppendLine(" AND Mov_Destinazioni.Id_Agenda = Movimenti.Id_Agenda ")
            Stb.AppendLine(" AND Mov_Destinazioni.Id_Mov = Movimenti.Id_Mov ")

            Stb.AppendLine("")

            Stb.AppendLine(" LEFT JOIN Centri_Aziendali ")
            Stb.AppendLine(" ON Centri_Aziendali.Piva = Agenda.PIVA ")
            Stb.AppendLine(" AND Centri_Aziendali.Sa_Cod = Agenda.Sa_Cod")


            Stb.AppendLine("")

            Stb.AppendLine(" LEFT JOIN Operazioni ")
            Stb.AppendLine(" ON Agenda.Lav_Cod = Operazioni.LAV_COD ")
            Stb.AppendLine(" ON GruppoOperazioni.GRU_COD = Operazioni.GRU_OP  ")

            Stb.AppendLine(" INNER JOIN imprese i ")
            Stb.AppendLine(" ON i.piva = Agenda.PIVA ")

            '(12/10/2017 MarcoG)  aggiunte note testuali e a checkbox
            Stb.AppendLine(" LEFT JOIN AgendaxNote ")
            Stb.AppendLine(" ON Agenda.Id_Agenda = AgendaxNote.Id_Agenda ")
            Stb.AppendLine(" LEFT JOIN Note_Intervento ")
            Stb.AppendLine(" ON AgendaxNote.Nota_Cod = Note_Intervento.Nota_Cod ")

            '(22/05/2018 MarcoG)  aggiunte le diciture per le ricette
            Stb.AppendLine(" LEFT JOIN RicettexAgenda ")
            Stb.AppendLine(" ON RicettexAgenda.id_agenda = Agenda.id_agenda AND RicettexAgenda.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri_Server.PivaSuperUser) & "'")
            Stb.AppendLine(" LEFT JOIN Ricette ")
            Stb.AppendLine(" ON RicettexAgenda.Ricetta_SuperUser = Ricette.Ricetta_SuperUser AND RicettexAgenda.Ricetta_Cod = Ricette.Ricetta_Cod ")

            Stb.AppendLine(" INNER JOIN #cte_Specie_ProdottiDaTrattare ON #cte_Specie_ProdottiDaTrattare.Id_Agenda = Agenda.Id_Agenda ")
            Stb.AppendLine(" LEFT JOIN #cte_Cultivar_ProdottiDaTrattare ON  ")
            Stb.AppendLine("     #cte_Cultivar_ProdottiDaTrattare.Mat_Cod = Materie_Prime.Mat_Cod ")
            Stb.AppendLine(" AND #cte_Cultivar_ProdottiDaTrattare.Cul_Cod = Materie_Prime.Cul_Cod ")
            Stb.AppendLine(" AND #cte_Cultivar_ProdottiDaTrattare.Veg_Cod = Materie_Prime.Veg_Cod ")
            Stb.AppendLine(" AND #cte_Cultivar_ProdottiDaTrattare.Id_Agenda = Agenda.Id_Agenda ")

            If FF_TrackedData_Cod > 0 Then

                ' INIZIO aggiunta perché la JOIN che fa sopra su materie prime non aggancia correttamente la descrizione (query tutta da rivedere)
                Stb.AppendLine(" LEFT JOIN Materie_Prime  AS Materie_Prime2 ")
                Stb.AppendLine(" ON (movimenti_dettagli.piva = Materie_Prime2.piva Or Materie_Prime2.sa_cod = -1) ")
                Stb.AppendLine(" AND movimenti_dettagli.Elem_COD = Materie_Prime2.Elem_COD ")
                Stb.AppendLine(" AND  movimenti_dettagli.mat_COD = Materie_Prime2.mat_COD ")
                ' FINE aggiunta perché la JOIN che fa sopra su materie prime non aggancia correttamente la descrizione (query tutta da rivedere)

                'INIZIO Aggiunto per avere la descrizione del magazzino / vasca
                Stb.AppendLine(" LEFT JOIN Mov_Destinazioni AS Mov_Destinazioni2 ")
                Stb.AppendLine(" ON Mov_Destinazioni2.Piva = Movimenti_Dettagli.PIVA")
                Stb.AppendLine(" AND Mov_Destinazioni2.Sa_Cod = Movimenti_Dettagli.Sa_Cod ")
                Stb.AppendLine(" AND Mov_Destinazioni2.Id_Agenda = Movimenti_Dettagli.Id_Agenda ")
                Stb.AppendLine(" AND Mov_Destinazioni2.Id_Mov = Movimenti_Dettagli.Id_Mov ")
                Stb.AppendLine(" AND Mov_Destinazioni2.Id_Mov_Det = Movimenti_Dettagli.Id_Mov_Det ")

                Stb.AppendLine(" LEFT JOIN fabbricati ")
                Stb.AppendLine(" ON fabbricati.PIVA = Mov_Destinazioni2.piva")
                Stb.AppendLine(" AND fabbricati.Sa_Cod = Mov_Destinazioni2.Sa_Cod")
                Stb.AppendLine(" AND fabbricati.Fabbricato_Cod = Mov_Destinazioni2.Id_destinazione")
                Stb.AppendLine(" AND Mov_Destinazioni2.Tipo_Destinazione = " & MAGAZZINO & " ")

                Stb.AppendLine(" LEFT JOIN Cantina_Vasche ")
                Stb.AppendLine(" ON Cantina_Vasche.PIVA = Mov_Destinazioni2.piva")
                Stb.AppendLine(" AND Cantina_Vasche.Sa_Cod = Mov_Destinazioni2.Sa_Cod")
                Stb.AppendLine(" AND Cantina_Vasche.vas_Cod = Mov_Destinazioni2.Id_destinazione")
                Stb.AppendLine(" AND Mov_Destinazioni2.Tipo_Destinazione = " & VASCA_ENOLOGICA & " ")
                'FINE Aggiunto per avere la descrizione del magazzino / vasca

                Stb.AppendLine(" INNER JOIN FF_TrackedData_Agenda ta ")
                Stb.AppendLine(" ON Movimenti_dettagli.PIVA = ta.piva ")
                Stb.AppendLine(" AND Movimenti_dettagli.Sa_Cod = ta.sa_cod ")
                Stb.AppendLine(" AND Movimenti_dettagli.Id_Agenda = ta.id_Agenda ")
                Stb.AppendLine(" AND Movimenti_dettagli.Id_Mov = ta.id_Mov ")
                Stb.AppendLine(" AND Movimenti_dettagli.Id_Mov_Det = ta.id_Mov_det ")

                Stb.AppendLine(" LEFT JOIN Linee_Preparazioni lp ")
                Stb.AppendLine(" ON agenda.PREPARAZIONE_COD = lp.Preparazione_Cod ")

                Stb.AppendLine(" WHERE ta.FF_TrackedData_Cod = " & Agro_SQL_SaveNum(FF_TrackedData_Cod))
                Stb.AppendLine(" AND ta.Visibile = 1")  'per compatibilità con visualizzazione a grafo

            Else
                Stb.AppendLine(" WHERE 1 = 1")

                '--------------------------
                'modifica per magazzino e contabilita
                If Sa_Cod <> 0 Then
                    Stb.AppendLine(" AND Agenda.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
                Else
                    If FiltroCentri <> "" Then
                        Stb.AppendLine(" AND Agenda.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ")
                    End If
                End If
                '--------------------------
            End If

            If Filtro_GruppoOperazioni <> "" Then
                Stb.AppendLine(" AND ( " & Filtro_GruppoOperazioni & " )")
            End If

            If Filtro_ElemCod <> "" Then
                Stb.AppendLine(" AND ( " & Filtro_ElemCod & " ) ")
            End If

            If FiltroOperazioni <> "" Then
                Stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(FiltroOperazioni))
            End If

            If Tipo <> "" Then
                Stb.AppendLine(" AND GruppoOperazioni.Tipo = '" & Agro_SQL_SaveText(Tipo) & "'")
            End If

            If Gru_Cod <> 0 Then
                Stb.AppendLine(" AND GruppoOperazioni.GRU_COD = " & Agro_SQL_SaveNum(Gru_Cod))
            End If

            Stb.AppendLine(" AND (Agenda.Lav_Cod IN (" & CStr(LAVCOD_TRATTAMENTO_POST_RACCOLTA) & ", " & CStr(LAVCOD_CONCIA_SEME) & ")) ")

            '--------------------------
            'modifica per magazzino e contabilita
            'Modifica per operazione di cura
            Stb.AppendLine(" AND Movimenti.Cau_Mov IN ('" & CAU_TRATTAMENTO & "','" & CAU_RILIEVO_CAMPO & "','" & CAU_RILIEVO_RACCOLTA & "','" & CAU_LAVORAZIONE & "') ")
            '--------------------------

            Stb.AppendLine(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio))
            Stb.AppendLine(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine))

            Stb.AppendLine(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleInizio))
            Stb.AppendLine(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine))

            If Veg_Cod <> 0 AndAlso Veg_Cod <> -1 Then
                Stb.AppendLine(" AND ( #cte_Specie_ProdottiDaTrattare.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ) ")
            End If
            If Cul_Cod <> 0 Then
                Stb.AppendLine(" AND ( #cte_Specie_ProdottiDaTrattare.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & " ) ")
            End If

            If xFiltroAggiuntivo_postRaccolta <> "" Then
                Stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_postRaccolta))
            End If
#End Region
            Stb.AppendLine(" ) ")

            If FF_TrackedData_Cod > 0 Then
                Stb.AppendLine(" UNION ")
            Else
                Stb.AppendLine(" UNION ALL ")
            End If

            Stb.AppendLine(" ( ")
#Region "OP CONTABILI E PRODOTTI IMPIEGATI"
            '/////////////////////////////////////////////////////////////////////////////////////////////
            '///////////////// OP CONTABILI E PRODOTTI IMPIEGATI /////////////////////////////////////////
            '/////////////////////////////////////////////////////////////////////////////////////////////
            Stb.AppendLine("--OP CONTABILI E PRODOTTI IMPIEGATI")

            Stb.AppendLine(" Select Agenda.Piva ")
            Stb.AppendLine("        , Agenda.Sa_Cod ")
            Stb.AppendLine("        , Agenda.Id_Agenda ")
            Stb.AppendLine("        , Movimenti_dettagli.Id_Mov ")
            Stb.AppendLine("        , Movimenti_dettagli.Id_Mov_Det ")
            Stb.AppendLine("        , Agenda.Lav_Cod ")
            Stb.AppendLine("        , Agenda.Des_Lib ")
            Stb.AppendLine("        , Movimenti.Data_Movimento ")
            Stb.AppendLine("        , Movimenti.Ora ")
            Stb.AppendLine("        , Movimenti.Mov_Desc ")
            Stb.AppendLine("        , Agenda.Username_Creazione ")
            Stb.AppendLine("        , Movimenti.Cau_Mov ")
            Stb.AppendLine("        , 0 AS Cul_Cod ")
            Stb.AppendLine("        , 0 AS Veg_Cod ")
            Stb.AppendLine("        , '' AS Veg_Des ")
            Stb.AppendLine("        , Agenda.Blocco_Flag ")
            Stb.AppendLine("        , Agenda.Raccoglitore_Cod ")
            Stb.AppendLine("        , ISNULL((Dettagli.Cognome + ' ' + Dettagli.Nome), 'N.D.') AS Tecnico ")

            '(26/11/2024 AF) aggiunto Id_Destinazione
            Stb.AppendLine(" , ISNULL(Mov_Destinazioni.Id_Destinazione, -1) AS Id_Destinazione ")

            Stb.AppendLine("        , ISNULL(Mov_Destinazioni.Tipo_Destinazione, -1) AS Tipo_Destinazione ")
            If FF_TrackedData_Cod > 0 Then
                Stb.AppendLine("        , ISNULL(Fabbricati.Fabbricato_Des, ISNULL(Cantina_Vasche.Identificativo, '')) AS Fabbricato_Des ")
            End If
            Stb.AppendLine("        , 0 AS Appezza ")
            Stb.AppendLine("        , '' AS App_Nome ")
            Stb.AppendLine("        , ISNULL(Movimenti_dettagli.Elem_Cod, 0) AS Elem_Cod ")
            Stb.AppendLine("        , ISNULL(Movimenti_dettagli.Mat_Cod, 0) AS Mat_Cod ")
            Stb.AppendLine("        , ISNULL(Movimenti_dettagli.Pro_Cod, 0) AS Pro_Cod ")
            Stb.AppendLine("        , ISNULL(FORMULATI.Fr_Des, '') AS Fr_Des ")
            Stb.AppendLine("        , ISNULL(FERTILIZZANTI.Fer_Des, '') AS Fer_Des ")
            Stb.AppendLine("        , ISNULL(TRAPPOLE.TRAP_DES, '') AS Trap_Des ")
            Stb.AppendLine("        , ISNULL(InsettiUtili.Ins_Des, '') AS Ins_Des ")
            ' INIZIO modificato perché la JOIN che fa sopra su materie prime non aggancia correttamente la descrizione (query tutta da rivedere)
            If FF_TrackedData_Cod > 0 Then
                Stb.AppendLine("        , ISNULL(Materie_Prime.Mat_Des, ISNULL(Materie_Prime2.Mat_Des, '')) AS Mat_Des ")
                Stb.AppendLine("        , ISNULL(Materie_Prime.Cod_Articolo, ISNULL(Materie_Prime2.Cod_Articolo, '')) AS Cod_Articolo ")
            Else
                Stb.AppendLine("        , ISNULL(Materie_Prime.Mat_Des, '') AS Mat_Des ")
                Stb.AppendLine("        , ISNULL(Materie_Prime.Cod_Articolo, '') AS Cod_Articolo ")
            End If
            ' FINE modificato perché la JOIN che fa sopra su materie prime non aggancia correttamente la descrizione (query tutta da rivedere)

            Stb.AppendLine("        , ISNULL(Centri_Aziendali.Sa_Nome, '') AS sa_nome ")

            'modifica per avversita
            'Stb.AppendLine(" , '' AS av_Des_Vol ")
            'Stb.AppendLine(" , '' AS av_gru_Des ")

            Stb.AppendLine("        , i.rag_soc ")
            Stb.AppendLine("        , Operazioni.lav_des ")
            Stb.AppendLine("        , GruppoOperazioni.gru_Des ")
            Stb.AppendLine("        , GruppoOperazioni.tipo ")

            '(20/12/2016 fede) aggiunte varieta
            Stb.AppendLine("        , '' AS cul_des ")

            '(03/05/2017 MarcoG) aggiunti principi attivi
            Stb.AppendLine("        , '' AS PrincipiAttivi ")

            '(04/05/2017 MarcoG) aggiunti campi
            Stb.AppendLine("        , '' AS campo_des ")

            '(09/10/2017 MarcoG) aggiunto il campo contabilizzato per la pianificazione
            Stb.AppendLine("        , ISNULL(Movimenti_dettagli.Contabilizzato, 0) AS contabilizzato ")

            '(26/11/2024 AF) aggiunto il cod_progetto
            Stb.AppendLine(" , ISNULL(Movimenti_dettagli.Cod_Progetto, 0) AS Progetto_Cod ")

            '(12/10/2017 MarcoG) aggiunto il lotto
            Stb.AppendLine("        , REPLACE(ISNULL(Movimenti_dettagli.lotto,''),'''', '') AS LottoProduzione ")

            '(12/10/2017 MarcoG) aggiunto le note a checkbox
            Stb.AppendLine("        , ISNULL(Note_Intervento.Nota_Des,'') AS Nota_Des ")

            '(13/10/2017 MarcoG) aggiunto la sup app e la sup trattata
            Stb.AppendLine("        , ISNULL(Reg_Impianti.ID_REG, 0) AS ID_REG ")
            Stb.AppendLine("        , ISNULL(Reg_Impianti.sup_imp, 0) AS SUP_APP ")
            Stb.AppendLine("        , ISNULL(Mov_Destinazioni.Qta2, 0) AS Sup_Trattata ")

            '(27/10/2017 MarcoG) aggiunto il lotto impianto (nome distinta, esercizio)
            Stb.AppendLine("        , '' AS LottoImpianto ")

            '(23/11/2017 MarcoG) aggiunte attività per altre operazioni
            Stb.AppendLine("        , '' AS AttivitaSigla ")
            Stb.AppendLine("        , '' AS AttivitaDesc ")

            '(09/02/2018 MarcoG)  aggiunte le diciture per le destinazioni dei terreni nudi
            Stb.AppendLine("        , 0 AS DestinazioneTerreniNudi_Cod ")
            Stb.AppendLine("        , '' AS DestinazioneTerreniNudi_Des ")

            '(09/05/2018 MarcoG)  aggiunta la dicitura per il rif di DDT e Fatture 
            Stb.AppendLine("        , ISNULL(LTRIM(RTRIM(ContattiDdtFatture.rag_soc + ContattiDdtFatture.Cognome + ' ' + ContattiDdtFatture.Nome)) , '') AS RifDdtFatture ")

            '(22/05/2018 MarcoG)  aggiunta la dicitura per le Ricette
            Stb.AppendLine("        , 0 AS Ricetta_Cod ")
            Stb.AppendLine("        , '' AS Ricetta_Numero ")

            '(23/08/2018 MarcoG)  aggiunta data di ultima modifica
            Stb.AppendLine("        , Agenda.Data_Modifica AS Data_Ultima_Modifica_Intervento ")

            '(05/12/2018 Fede)  aggiunta validita_inizio di Mov_Destinazioni per avere la Data Rilevata
            Stb.AppendLine("        , ISNULL(Mov_Destinazioni.validita_inizio, CONVERT(DateTime,'1900/01/01',120)) AS validita_inizio_destinazione ")

            '(11/01/2023 Anny)  aggiunte quantità e unità di misura
            Stb.AppendLine("        , ROUND(coalesce(Movimenti_dettagli.Qta_Extra_Totale, 0), +4) AS Qta_Extra_Totale")
            Stb.AppendLine("        , coalesce(Movimenti_dettagli.Qta, 0) AS Qta")
            Stb.AppendLine("        , Movimenti_dettagli.QTA_EXTRA")
            Stb.AppendLine("        , Movimenti_dettagli.Udm_Cod")
            Stb.AppendLine("        , Movimenti_dettagli.Extra_Int")
            Stb.AppendLine("        , coalesce(um1.UDM_SIM, '') AS UDM_SIM_ExtraInt")
            Stb.AppendLine("        , coalesce(um2.UDM_SIM, '') AS UDM_SIM_UDM_COD")
            Stb.AppendLine("        , Agenda.Data_Creazione AS Data_Creazione")
            '(12/02/2024 Anny)  aggiunta origine attività
            Stb.AppendLine("        , Agenda.Origine AS Origine")
            If Visualizza_Codici_AppezzaImpianti Then
                Stb.AppendLine("        , '' AS Riferimento_Alfanumerico_Appezzamento ")
                Stb.AppendLine("        , '' AS Codice_Impianto ")
            End If

            If Visualizza_KPIN_BlockName Then
                Stb.AppendLine("        , '' AS Zespri_Codice_kPIN ")
                Stb.AppendLine("        , '' AS Zespri_Block_Name ")
            End If


            If FF_TrackedData_Cod > 0 Then
                Stb.AppendLine("        , ISNULL(ta.Lotto_Padre, '') AS FF_Track_Lotto_Padre ")
                Stb.AppendLine("        , ISNULL(ta.Lotto, '') AS FF_Track_Lotto ")
                Stb.AppendLine("        , ISNULL(ta.Cal_Cod_Padre, '') AS FF_Track_Cal_Cod_Padre ")
                Stb.AppendLine("        , ISNULL(ta.Cal_Cod, '') AS FF_Track_Cal_Cod ")
                Stb.AppendLine("        , ta.Qta_Extra_Totale AS FF_Track_Qta_Extra_Totale ")
                Stb.AppendLine("        , movimenti_dettagli.Qta_Dettaglio1 AS FF_Track_Qta_Contenitori ")
                Stb.AppendLine("        , movimenti_dettagli.Qta_Dettaglio2 AS FF_Track_Qta_Imballi ")
                Stb.AppendLine("        , ISNULL(Materie_Prime.Linea_Cod, 0) AS FF_Linea_Cod ")
                Stb.AppendLine("        , ISNULL(lp.codice_generazione, 0) AS FF_codice_generazione ")

            End If

            Stb.AppendLine("            FROM Movimenti_dettagli ")
            Stb.AppendLine("")
            Stb.AppendLine("    LEFT JOIN Formulati ")
            Stb.AppendLine("        ON Movimenti_dettagli.Pro_Cod = Formulati.Fr_Cod ")
            Stb.AppendLine("    LEFT JOIN UnitaMisura um1 on Movimenti_dettagli.Extra_Int = um1.UDM_COD")
            Stb.AppendLine("    LEFT JOIN UnitaMisura um2 on Movimenti_dettagli.Udm_Cod = um2.UDM_COD")
            Stb.AppendLine("")
            Stb.AppendLine("    LEFT JOIN Fertilizzanti ")
            Stb.AppendLine("        ON Movimenti_dettagli.Pro_Cod = Fertilizzanti.Fer_Cod ")
            Stb.AppendLine("")
            Stb.AppendLine("    LEFT JOIN Trappole ")
            Stb.AppendLine("        ON Movimenti_dettagli.Pro_Cod = Trappole.TRAP_COD ")
            Stb.AppendLine("")
            Stb.AppendLine("    LEFT JOIN InsettiUtili ")
            Stb.AppendLine("        ON Movimenti_dettagli.Pro_Cod = InsettiUtili.ins_cod ")
            Stb.AppendLine("")
            Stb.AppendLine("    LEFT JOIN Materie_Prime ")
            Stb.AppendLine("        ON Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod ")
            Stb.AppendLine("        AND Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod ")
            Stb.AppendLine("")
            Stb.AppendLine("    RIGHT JOIN #cte_Utenti_Dettagli AS Dettagli ")
            Stb.AppendLine("")
            Stb.AppendLine("    RIGHT JOIN #cte_Agenda_Per_Piva AS Agenda ")
            Stb.AppendLine("")
            Stb.AppendLine("    INNER JOIN Movimenti ")

            'vanni, verifica sa_cod
            'StbQuery.Append("           AND Agenda.Sa_Cod = Movimenti.Sa_Cod" )

            Stb.AppendLine("        ON Agenda.Id_Agenda = Movimenti.Id_Agenda ")
            Stb.AppendLine("        ON Dettagli.CodFisc = Agenda.Username_Creazione ")

            'vanni, verifica sa_cod
            'StbQuery.Append("          AND Movimenti_dettagli.Sa_Cod = Movimenti.Sa_Cod " )

            Stb.AppendLine("        ON Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda ")
            Stb.AppendLine("        AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov ")
            Stb.AppendLine("")

            Stb.AppendLine("        LEFT JOIN Centri_Aziendali ")
            Stb.AppendLine("        ON Centri_Aziendali.Piva = Agenda.PIVA ")
            Stb.AppendLine("        AND Centri_Aziendali.Sa_Cod = Agenda.Sa_Cod")

            Stb.AppendLine("    LEFT JOIN Mov_Destinazioni ")
            Stb.AppendLine("        ON Movimenti.Sa_Cod = Mov_Destinazioni.Sa_Cod ")
            Stb.AppendLine("        AND Movimenti.Id_Agenda = Mov_Destinazioni.Id_Agenda ")
            Stb.AppendLine("        AND Movimenti.Id_Mov = Mov_Destinazioni.Id_Mov  ")
            Stb.AppendLine("")

            '(13/10/2017 MarcoG) aggiunto per la superficie trattata
            Stb.AppendLine("            LEFT JOIN Reg_Impianti ")
            Stb.AppendLine("            ON Reg_Impianti.PIVA = Mov_Destinazioni.Piva ")
            Stb.AppendLine("            AND Reg_Impianti.SA_COD = Mov_Destinazioni.Sa_Cod ")
            Stb.AppendLine("            AND Reg_Impianti.APPEZZA = Mov_Destinazioni.Appezza ")
            Stb.AppendLine("            AND Reg_Impianti.ID_REG = Mov_Destinazioni.Id_Destinazione ")
            Stb.AppendLine("")

            'Modifica per gruppi operazioni e tipo
            Stb.AppendLine("    LEFT JOIN dbo.Operazioni ")
            Stb.AppendLine("        on Agenda.Lav_Cod = Operazioni.Lav_Cod ")
            Stb.AppendLine("")
            Stb.AppendLine("    LEFT JOIN dbo.GruppoOperazioni ")
            Stb.AppendLine("        on Operazioni.GRU_OP = GruppoOperazioni.GRU_COD ")

            Stb.AppendLine("    INNER JOIN imprese i ")
            Stb.AppendLine("        on i.piva = Agenda.PIVA ")

            '(12/10/2017 MarcoG)  aggiunte note testuali e a checkbox
            Stb.AppendLine("   LEFT JOIN AgendaxNote ")
            Stb.AppendLine("   ON Agenda.Id_Agenda = AgendaxNote.Id_Agenda ")
            Stb.AppendLine("   LEFT JOIN Note_Intervento ")
            Stb.AppendLine("   ON AgendaxNote.Nota_Cod = Note_Intervento.Nota_Cod ")

            '(09/05/2018 MarcoG)  aggiunta la dicitura per il rif di DDT e Fatture 
            Stb.AppendLine("   LEFT JOIN Risorse_Umane AS Risorse_UmaneDdtFatture ")
            Stb.AppendLine("   ON Risorse_UmaneDdtFatture.Cod_RisUm = Movimenti.Cod_RisUm ")
            Stb.AppendLine("   LEFT JOIN Contatti AS ContattiDdtFatture ")
            Stb.AppendLine("   ON Risorse_UmaneDdtFatture.Cod_Contatto = ContattiDdtFatture.Cod_Contatto AND Risorse_UmaneDdtFatture.Piva = ContattiDdtFatture.Piva ")



            If FF_TrackedData_Cod > 0 Then

                ' INIZIO aggiunta perché la JOIN che fa sopra su materie prime non aggancia correttamente la descrizione (query tutta da rivedere)
                Stb.AppendLine("  LEFT JOIN Materie_Prime  AS Materie_Prime2 ")
                Stb.AppendLine("  ON (movimenti_dettagli.piva = Materie_Prime2.piva Or Materie_Prime2.sa_cod = -1) ")
                Stb.AppendLine("  AND   movimenti_dettagli.Elem_COD = Materie_Prime2.Elem_COD ")
                Stb.AppendLine("  AND  movimenti_dettagli.mat_COD = Materie_Prime2.mat_COD ")
                ' FINE aggiunta perché la JOIN che fa sopra su materie prime non aggancia correttamente la descrizione (query tutta da rivedere)

                'INIZIO Aggiunto per avere la descrizione del magazzino / vasca
                Stb.AppendLine("   LEFT JOIN Mov_Destinazioni AS Mov_Destinazioni2 ")
                Stb.AppendLine("       ON Mov_Destinazioni2.Piva = Movimenti_Dettagli.PIVA")
                Stb.AppendLine("    AND Mov_Destinazioni2.Sa_Cod = Movimenti_Dettagli.Sa_Cod ")
                Stb.AppendLine("    AND Mov_Destinazioni2.Id_Agenda = Movimenti_Dettagli.Id_Agenda ")
                Stb.AppendLine("    AND Mov_Destinazioni2.Id_Mov = Movimenti_Dettagli.Id_Mov ")
                Stb.AppendLine("    AND Mov_Destinazioni2.Id_Mov_Det = Movimenti_Dettagli.Id_Mov_Det ")

                Stb.AppendLine("   LEFT JOIN fabbricati ")
                Stb.AppendLine("       ON fabbricati.PIVA = Mov_Destinazioni2.piva")
                Stb.AppendLine("       AND fabbricati.Sa_Cod = Mov_Destinazioni2.Sa_Cod")
                Stb.AppendLine("       AND fabbricati.Fabbricato_Cod = Mov_Destinazioni2.Id_destinazione")
                Stb.AppendLine("       AND Mov_Destinazioni2.Tipo_Destinazione = " & MAGAZZINO & " ")

                Stb.AppendLine("   LEFT JOIN Cantina_Vasche ")
                Stb.AppendLine("       ON Cantina_Vasche.PIVA = Mov_Destinazioni2.piva")
                Stb.AppendLine("       AND Cantina_Vasche.Sa_Cod = Mov_Destinazioni2.Sa_Cod")
                Stb.AppendLine("       AND Cantina_Vasche.vas_Cod = Mov_Destinazioni2.Id_destinazione")
                Stb.AppendLine("       AND Mov_Destinazioni2.Tipo_Destinazione = " & VASCA_ENOLOGICA & " ")
                'FINE Aggiunto per avere la descrizione del magazzino / vasca

                Stb.AppendLine("   INNER JOIN FF_TrackedData_Agenda ta ")
                Stb.AppendLine("       on Movimenti_dettagli.PIVA = ta.piva ")
                Stb.AppendLine("       AND Movimenti_dettagli.Sa_Cod = ta.sa_cod ")
                Stb.AppendLine("       AND Movimenti_dettagli.Id_Agenda = ta.id_Agenda ")
                Stb.AppendLine("       AND Movimenti_dettagli.Id_Mov = ta.id_Mov ")
                Stb.AppendLine("       AND Movimenti_dettagli.Id_Mov_Det = ta.id_Mov_det ")

                Stb.AppendLine(" LEFT JOIN Linee_Preparazioni lp ")
                Stb.AppendLine("    on agenda.PREPARAZIONE_COD = lp.Preparazione_Cod ")

                Stb.AppendLine(" WHERE ta.FF_TrackedData_Cod = " & Agro_SQL_SaveNum(FF_TrackedData_Cod))
                Stb.AppendLine(" AND ta.Visibile = 1")  'per compatibilità con visualizzazione a grafo

            Else
                Stb.AppendLine(" WHERE 1 = 1  ")

                '--------------------------
                'modifica per magazzino e contabilita
                If Sa_Cod <> 0 Then
                    Stb.AppendLine(" AND (  Agenda.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " OR Agenda.Sa_Cod = 0 )  ")
                Else
                    If FiltroCentri <> "" Then
                        Stb.AppendLine(" AND  (Agenda.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") OR Agenda.Sa_Cod = 0 ) ")
                    End If
                End If
                '--------------------------
            End If

            If Filtro_GruppoOperazioni <> "" Then
                Stb.AppendLine(" AND ( " & Filtro_GruppoOperazioni & " ) ")
            End If

            If Filtro_ElemCod <> "" Then
                Stb.AppendLine(" AND ( " & Filtro_ElemCod & " ) ")
            End If

            If FiltroOperazioni <> "" Then
                Stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(FiltroOperazioni))
            End If

            If Tipo <> "" Then
                Stb.AppendLine(" AND GruppoOperazioni.Tipo = '" & Agro_SQL_SaveText(Tipo) & "'")
            End If

            If Gru_Cod <> 0 Then
                Stb.AppendLine(" AND GruppoOperazioni.GRU_COD = " & Agro_SQL_SaveNum(Gru_Cod))
            End If

            '--------------------------
            'modifica per magazzino e contabilita (1000,1001,1020,1021,1022,1023,1025,1031,1033
            'venivano escluse le operazioni contabili e di magazzino
            Stb.AppendLine(" AND Agenda.Lav_Cod in (" & LAVCOD_FATTURA_RICEVUTA & "," & LAVCOD_FATTURA_EMESSA & "," &
                            LAVCOD_VENDITA & "," & LAVCOD_CORRISPETTIVO_VENDITA_SFUSO & "," & LAVCOD_ACQUISTO & "," &
                            LAVCOD_CARICO & "," & LAVCOD_SCARICO & "," &
                            LAVCOD_BOLLA_RICEVUTA & "," & LAVCOD_BOLLA_EMESSA & "," & LAVCOD_TRASFERIMENTO & "," &
                            LAVCOD_PROCEDURA_LIQUIDAZIONE_SOCI & "," & LAVCOD_NOTA_ACCREDITO_EMESSA & "," & LAVCOD_NOTA_ACCREDITO_RICEVUTA & "," & LAVCOD_MVV_EMESSO & "," & LAVCOD_MVV_RICEVUTO & ")")
            '--------------------------

            '--------------------------
            'modifica per magazzino e contabilita
            Stb.AppendLine(" AND Movimenti.Cau_Mov IN ('" & CAU_REGISTRAZIONI & "','" & CAU_CARICO & "','" & CAU_SCARICO & "','" & CAU_TRASFERIMENTO & "') ")
            '--------------------------

            Stb.AppendLine(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio))
            Stb.AppendLine(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine))

            Stb.AppendLine(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleInizio))
            Stb.AppendLine(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine))

            If xFiltroAggiuntivo_contabili <> "" Then
                Stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_contabili))
            End If
#End Region
            Stb.AppendLine(" ) ")

            If FF_TrackedData_Cod > 0 Then

                Stb.AppendLine(" UNION ")

                Stb.AppendLine(" ( ")
#Region "FRESH AND FOOD, CONFERIMENTI"
                '/////////////////////////////////////////////////////////////////////////////////////////////
                '///////////////// FRESH AND FOOD, CONFERIMENTI /////////////////////////////////////////
                '/////////////////////////////////////////////////////////////////////////////////////////////
                Stb.AppendLine("--FRESH AND FOOD, CONFERIMENTI")

                Stb.AppendLine(" Select DISTINCT ")
                Stb.AppendLine("        Agenda.Piva ")
                Stb.AppendLine("        , Agenda.Sa_Cod ")
                Stb.AppendLine("        , Agenda.Id_Agenda ")
                Stb.AppendLine("        , Movimenti_dettagli.Id_Mov ")
                Stb.AppendLine("        , Movimenti_dettagli.Id_Mov_Det ")
                Stb.AppendLine("        , Agenda.Lav_Cod ")
                Stb.AppendLine("        , Agenda.Des_Lib ")
                Stb.AppendLine("        , Movimenti.Data_Movimento ")
                Stb.AppendLine("        , Movimenti.Ora ")
                Stb.AppendLine("        , Movimenti.Mov_Desc ")
                Stb.AppendLine("        , Agenda.Username_Creazione ")
                Stb.AppendLine("        , Movimenti.Cau_Mov ")
                Stb.AppendLine("        , 0 AS Cul_Cod ")
                Stb.AppendLine("        , 0 AS Veg_Cod ")
                Stb.AppendLine("        , '' AS Veg_Des ")
                Stb.AppendLine("        , Agenda.Blocco_Flag ")
                Stb.AppendLine("        , Agenda.Raccoglitore_Cod ")
                Stb.AppendLine("        , ISNULL((Dettagli.Cognome + ' ' + Dettagli.Nome), 'N.D.') AS Tecnico ")

                '(26/11/2024 AF) aggiunto Id_Destinazione
                Stb.AppendLine(" , ISNULL(Mov_Destinazioni.Id_Destinazione, -1) AS Id_Destinazione ")

                Stb.AppendLine("        , ISNULL(Mov_Destinazioni.Tipo_Destinazione, -1) AS Tipo_Destinazione ")
                If FF_TrackedData_Cod > 0 Then
                    Stb.AppendLine("        , ISNULL(Fabbricati.Fabbricato_Des, ISNULL(Cantina_Vasche.Identificativo, '')) AS Fabbricato_Des ")
                End If
                Stb.AppendLine("        , 0 AS Appezza ")
                Stb.AppendLine("        , '' AS App_Nome ")
                Stb.AppendLine("        , ISNULL(Movimenti_dettagli.Elem_Cod, 0) AS Elem_Cod ")
                Stb.AppendLine("        , ISNULL(Movimenti_dettagli.Mat_Cod, 0) AS Mat_Cod ")
                Stb.AppendLine("        , ISNULL(Movimenti_dettagli.Pro_Cod, 0) AS Pro_Cod ")
                Stb.AppendLine("        , ISNULL(FORMULATI.Fr_Des, '') AS Fr_Des ")
                Stb.AppendLine("        , ISNULL(FERTILIZZANTI.Fer_Des, '') AS Fer_Des ")
                Stb.AppendLine("        , ISNULL(TRAPPOLE.TRAP_DES, '') AS Trap_Des ")
                Stb.AppendLine("        , ISNULL(InsettiUtili.Ins_Des, '') AS Ins_Des ")
                If FF_TrackedData_Cod > 0 Then
                    Stb.AppendLine("        , ISNULL(Materie_Prime.Mat_Des, ISNULL(Materie_Prime2.Mat_Des, '')) AS Mat_Des ")
                    Stb.AppendLine("        , ISNULL(Materie_Prime.Cod_Articolo, ISNULL(Materie_Prime2.Cod_Articolo, '')) AS Cod_Articolo ")
                Else

                    Stb.AppendLine("        , ISNULL(Materie_Prime.Mat_Des, '') AS Mat_Des ")
                    Stb.AppendLine("        , ISNULL(Materie_Prime.Cod_Articolo, '') AS Cod_Articolo ")
                End If

                Stb.AppendLine("        , ISNULL(Centri_Aziendali.Sa_Nome  , '') AS sa_nome ")

                'modifica per avversita
                'Stb.AppendLine(" , '' AS av_Des_Vol ")
                'Stb.AppendLine(" , '' AS av_gru_Des ")

                Stb.AppendLine("        , i.rag_soc ")
                Stb.AppendLine("        , Operazioni.lav_des ")
                Stb.AppendLine("        , GruppoOperazioni.gru_Des ")
                Stb.AppendLine("        , GruppoOperazioni.tipo ")

                '(20/12/2016 fede) aggiunte varieta
                Stb.AppendLine("        ,  '' AS cul_des ")

                '(03/05/2017 MarcoG) aggiunti principi attivi
                Stb.AppendLine("        , '' AS PrincipiAttivi ")

                '(04/05/2017 MarcoG) aggiunti campi
                Stb.AppendLine("        , '' AS campo_des ")

                '(09/10/2017 MarcoG) aggiunto il campo contabilizzato per la pianificazione
                Stb.AppendLine("        , ISNULL(Movimenti_dettagli.Contabilizzato, 0) AS contabilizzato ")

                '(26/11/2024 AF) aggiunto il cod_progetto
                Stb.AppendLine(" , ISNULL(Movimenti_dettagli.Cod_Progetto, 0) AS Progetto_Cod ")

                '(12/10/2017 MarcoG) aggiunto il lotto
                Stb.AppendLine("        , REPLACE(ISNULL(Movimenti_dettagli.lotto,''),'''', '') AS LottoProduzione ")

                '(12/10/2017 MarcoG) aggiunto le note a checkbox
                Stb.AppendLine("        , ISNULL(Note_Intervento.Nota_Des,'') AS Nota_Des ")

                '(13/10/2017 MarcoG) aggiunto la sup app e la sup trattata
                Stb.AppendLine("        , ISNULL(Reg_Impianti.ID_REG, 0) AS ID_REG ")
                Stb.AppendLine("        , ISNULL(Reg_Impianti.sup_imp, 0) AS SUP_APP ")
                Stb.AppendLine("        , ISNULL(Mov_Destinazioni.Qta2, 0) AS Sup_Trattata ")

                '(27/10/2017 MarcoG) aggiunto il lotto impianto (nome distinta, esercizio)
                Stb.AppendLine("        , REPLACE(ISNULL(Imprese_Progetti.Progetto_Nome, ''),'''', '') AS LottoImpianto ")

                '(23/11/2017 MarcoG)  aggiunte attività per altre operazioni
                Stb.AppendLine("        , '' AS AttivitaSigla ")
                Stb.AppendLine("        , '' AS AttivitaDesc ")

                '(09/02/2018 MarcoG)  aggiunte le diciture per le destinazioni dei terreni nudi
                Stb.AppendLine("        , 0 AS DestinazioneTerreniNudi_Cod ")
                Stb.AppendLine("        , '' AS DestinazioneTerreniNudi_Des ")

                '(09/05/2018 MarcoG)  aggiunta la dicitura per il rif di DDT e Fatture 
                Stb.AppendLine(" , '' AS RifDdtFatture ")

                '(22/05/2018 MarcoG)  aggiunta la dicitura per le Ricette
                Stb.AppendLine("        , 0 AS Ricetta_Cod ")
                Stb.AppendLine("        , '' AS Ricetta_Numero ")

                '(23/08/2018 MarcoG)  aggiunta data di ultima modifica
                Stb.AppendLine("        , Agenda.Data_Modifica AS Data_Ultima_Modifica_Intervento ")

                '(05/12/2018 Fede)  aggiunta validita_inizio di Mov_Destinazioni per avere la Data Rilevata
                Stb.AppendLine("        , CONVERT(DateTime,'1900/01/01',120) AS validita_inizio_destinazione ")

                '(11/01/2023 Anny)  aggiunte quantità e unità di misura
                Stb.AppendLine("        , ROUND(coalesce(Movimenti_dettagli.Qta_Extra_Totale, 0), +4) AS Qta_Extra_Totale")
                Stb.AppendLine("        , coalesce(Movimenti_dettagli.Qta, 0) AS Qta")
                Stb.AppendLine("        , Movimenti_dettagli.QTA_EXTRA")
                Stb.AppendLine("        , Movimenti_dettagli.Udm_Cod")
                Stb.AppendLine("        , Movimenti_dettagli.Extra_Int")
                Stb.AppendLine("        , coalesce(um1.UDM_SIM, '') AS UDM_SIM_ExtraInt")
                Stb.AppendLine("        , coalesce(um2.UDM_SIM, '') AS UDM_SIM_UDM_COD")
                Stb.AppendLine("        , Agenda.Data_Creazione AS Data_Creazione")
                '(12/02/2024 Anny)  aggiunta origine attività
                Stb.AppendLine("        , Agenda.Origine AS Origine")

                If Visualizza_Codici_AppezzaImpianti Then
                    Stb.AppendLine("        , '' AS Riferimento_Alfanumerico_Appezzamento ")
                    Stb.AppendLine("        , '' AS Codice_Impianto ")
                End If

                If Visualizza_KPIN_BlockName Then
                    Stb.AppendLine("        , '' AS Zespri_Codice_kPIN ")
                    Stb.AppendLine("        , '' AS Zespri_Block_Name ")
                End If

                If FF_TrackedData_Cod > 0 Then
                    Stb.AppendLine("        , ISNULL(ta.Lotto_Padre, '') AS FF_Track_Lotto_Padre ")
                    Stb.AppendLine("        , ISNULL(ta.Lotto, '') AS FF_Track_Lotto ")
                    Stb.AppendLine("        , ISNULL(ta.Cal_Cod_Padre, '') AS FF_Track_Cal_Cod_Padre ")
                    Stb.AppendLine("        , ISNULL(ta.Cal_Cod, '') AS FF_Track_Cal_Cod ")
                    Stb.AppendLine("        , ta.Qta_Extra_Totale AS FF_Track_Qta_Extra_Totale ")
                    Stb.AppendLine("        , movimenti_dettagli.Qta_Dettaglio1 AS FF_Track_Qta_Contenitori ")
                    Stb.AppendLine("        , movimenti_dettagli.Qta_Dettaglio2 AS FF_Track_Qta_Imballi ")
                    Stb.AppendLine("        , ISNULL(Materie_Prime.Linea_Cod, 0) AS FF_Linea_Cod ")
                    Stb.AppendLine("        , ISNULL(lp.codice_generazione, 0) AS FF_codice_generazione ")

                End If

                Stb.AppendLine("            FROM Movimenti_dettagli ")
                Stb.AppendLine("")
                Stb.AppendLine("    LEFT JOIN Formulati ")
                Stb.AppendLine("        ON Movimenti_dettagli.Pro_Cod = Formulati.Fr_Cod ")
                Stb.AppendLine("    LEFT JOIN UnitaMisura um1 on Movimenti_dettagli.Extra_Int = um1.UDM_COD")
                Stb.AppendLine("    LEFT JOIN UnitaMisura um2 on Movimenti_dettagli.Udm_Cod = um2.UDM_COD")
                Stb.AppendLine("")
                Stb.AppendLine("    LEFT JOIN Fertilizzanti ")
                Stb.AppendLine("        ON Movimenti_dettagli.Pro_Cod = Fertilizzanti.Fer_Cod ")
                Stb.AppendLine("")
                Stb.AppendLine("    LEFT JOIN Trappole ")
                Stb.AppendLine("        ON Movimenti_dettagli.Pro_Cod = Trappole.TRAP_COD ")
                Stb.AppendLine("")
                Stb.AppendLine("    LEFT JOIN InsettiUtili ")
                Stb.AppendLine("        ON Movimenti_dettagli.Pro_Cod = InsettiUtili.ins_cod ")
                Stb.AppendLine("")
                Stb.AppendLine("    LEFT JOIN Materie_Prime ")
                Stb.AppendLine("        ON Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod ")
                Stb.AppendLine("        AND Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod ")
                Stb.AppendLine("")
                Stb.AppendLine("    RIGHT JOIN #cte_Utenti_Dettagli AS Dettagli ")
                Stb.AppendLine("")
                Stb.AppendLine("    RIGHT JOIN #cte_Agenda_Per_Piva AS Agenda ")
                Stb.AppendLine("")
                Stb.AppendLine("    INNER JOIN Movimenti ")
                Stb.AppendLine("        ON Agenda.PIVA = Movimenti.PIVA ")

                'vanni, verifica sa_cod
                'StbQuery.Append("           AND Agenda.Sa_Cod = Movimenti.Sa_Cod" )

                Stb.AppendLine("        AND Agenda.Id_Agenda = Movimenti.Id_Agenda ")
                Stb.AppendLine("        ON Dettagli.CodFisc = Agenda.Username_Creazione ")
                Stb.AppendLine("        ON Movimenti_dettagli.PIVA = Movimenti.PIVA ")

                'vanni, verifica sa_cod
                'StbQuery.Append("          AND Movimenti_dettagli.Sa_Cod = Movimenti.Sa_Cod " )

                Stb.AppendLine("        AND Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda ")
                Stb.AppendLine("        AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov ")
                Stb.AppendLine("")

                Stb.AppendLine("        LEFT JOIN Centri_Aziendali ")
                Stb.AppendLine("        ON Centri_Aziendali.Piva = Agenda.PIVA ")
                Stb.AppendLine("        AND Centri_Aziendali.Sa_Cod = Agenda.Sa_Cod")

                Stb.AppendLine("    LEFT JOIN Mov_Destinazioni ")
                Stb.AppendLine("        ON Movimenti.Piva = Mov_Destinazioni.Piva ")
                Stb.AppendLine("        AND Movimenti.Sa_Cod = Mov_Destinazioni.Sa_Cod ")
                Stb.AppendLine("        AND Movimenti.Id_Agenda = Mov_Destinazioni.Id_Agenda ")
                Stb.AppendLine("        AND Movimenti.Id_Mov = Mov_Destinazioni.Id_Mov  ")
                Stb.AppendLine("")

                '(13/10/2017 MarcoG) aggiunto per la superficie trattata
                Stb.AppendLine("            LEFT JOIN Reg_Impianti ")
                Stb.AppendLine("            ON Reg_Impianti.PIVA = Mov_Destinazioni.Piva ")
                Stb.AppendLine("            AND Reg_Impianti.SA_COD = Mov_Destinazioni.Sa_Cod ")
                Stb.AppendLine("            AND Reg_Impianti.APPEZZA = Mov_Destinazioni.Appezza ")
                Stb.AppendLine("            AND Reg_Impianti.ID_REG = Mov_Destinazioni.Id_Destinazione ")
                Stb.AppendLine("")

                '(12/10/2017 MarcoG) aggiunto per il lotto
                Stb.AppendLine("    LEFT JOIN Imprese_Progetti ")
                Stb.AppendLine("        ON Imprese_Progetti.Piva = Mov_Destinazioni.PIVA ")
                Stb.AppendLine("        AND Imprese_Progetti.Sa_Cod = Mov_Destinazioni.SA_COD ")
                Stb.AppendLine("        AND Imprese_Progetti.Appezza = Mov_Destinazioni.APPEZZA ")
                Stb.AppendLine("        AND Imprese_Progetti.Id_Reg = Mov_Destinazioni.id_destinazione ")
                Stb.AppendLine("")

                'Modifica per gruppi operazioni e tipo
                Stb.AppendLine("    LEFT JOIN dbo.Operazioni ")
                Stb.AppendLine("        on Agenda.Lav_Cod = Operazioni.Lav_Cod ")
                Stb.AppendLine("")
                Stb.AppendLine("    LEFT JOIN dbo.GruppoOperazioni ")
                Stb.AppendLine("        on Operazioni.GRU_OP = GruppoOperazioni.GRU_COD ")

                Stb.AppendLine("    INNER JOIN imprese i ")
                Stb.AppendLine("        on i.piva = Agenda.PIVA ")

                '(12/10/2017 MarcoG)  aggiunte note testuali e a checkbox
                Stb.AppendLine("   LEFT JOIN AgendaxNote ")
                Stb.AppendLine("   ON Agenda.Id_Agenda = AgendaxNote.Id_Agenda ")
                Stb.AppendLine("   LEFT JOIN Note_Intervento ")
                Stb.AppendLine("   ON AgendaxNote.Nota_Cod = Note_Intervento.Nota_Cod ")

                If FF_TrackedData_Cod > 0 Then

                    ' INIZIO aggiunta perché la JOIN che fa sopra su materie prime non aggancia correttamente la descrizione (query tutta da rivedere)
                    Stb.AppendLine("  LEFT JOIN Materie_Prime  AS Materie_Prime2 ")
                    Stb.AppendLine("  ON (movimenti_dettagli.piva = Materie_Prime2.piva Or Materie_Prime2.sa_cod = -1) ")
                    Stb.AppendLine("  AND   movimenti_dettagli.Elem_COD = Materie_Prime2.Elem_COD ")
                    Stb.AppendLine("  AND  movimenti_dettagli.mat_COD = Materie_Prime2.mat_COD ")
                    ' FINE aggiunta perché la JOIN che fa sopra su materie prime non aggancia correttamente la descrizione (query tutta da rivedere)

                    'INIZIO Aggiunto per avere la descrizione del magazzino / vasca
                    Stb.AppendLine("   LEFT JOIN Mov_Destinazioni AS Mov_Destinazioni2 ")
                    Stb.AppendLine("       ON Mov_Destinazioni2.Piva = Movimenti_Dettagli.PIVA")
                    Stb.AppendLine("    AND Mov_Destinazioni2.Sa_Cod = Movimenti_Dettagli.Sa_Cod ")
                    Stb.AppendLine("    AND Mov_Destinazioni2.Id_Agenda = Movimenti_Dettagli.Id_Agenda ")
                    Stb.AppendLine("    AND Mov_Destinazioni2.Id_Mov = Movimenti_Dettagli.Id_Mov ")
                    Stb.AppendLine("    AND Mov_Destinazioni2.Id_Mov_Det = Movimenti_Dettagli.Id_Mov_Det ")

                    Stb.AppendLine("   LEFT JOIN fabbricati ")
                    Stb.AppendLine("       ON fabbricati.PIVA = Mov_Destinazioni2.piva")
                    Stb.AppendLine("       AND fabbricati.Sa_Cod = Mov_Destinazioni2.Sa_Cod")
                    Stb.AppendLine("       AND fabbricati.Fabbricato_Cod = Mov_Destinazioni2.Id_destinazione")
                    Stb.AppendLine("       AND Mov_Destinazioni2.Tipo_Destinazione = " & MAGAZZINO & " ")

                    Stb.AppendLine("   LEFT JOIN Cantina_Vasche ")
                    Stb.AppendLine("       ON Cantina_Vasche.PIVA = Mov_Destinazioni2.piva")
                    Stb.AppendLine("       AND Cantina_Vasche.Sa_Cod = Mov_Destinazioni2.Sa_Cod")
                    Stb.AppendLine("       AND Cantina_Vasche.vas_Cod = Mov_Destinazioni2.Id_destinazione")
                    Stb.AppendLine("       AND Mov_Destinazioni2.Tipo_Destinazione = " & VASCA_ENOLOGICA & " ")
                    'FINE Aggiunto per avere la descrizione del magazzino / vasca

                    Stb.AppendLine("   INNER JOIN FF_TrackedData_Agenda ta ")
                    Stb.AppendLine("       on Movimenti_dettagli.PIVA = ta.piva ")
                    Stb.AppendLine("       AND Movimenti_dettagli.Sa_Cod = ta.sa_cod ")
                    Stb.AppendLine("       AND Movimenti_dettagli.Id_Agenda = ta.id_Agenda ")
                    Stb.AppendLine("       AND Movimenti_dettagli.Id_Mov = ta.id_Mov ")
                    Stb.AppendLine("       AND Movimenti_dettagli.Id_Mov_Det = ta.id_Mov_det ")

                    Stb.AppendLine(" LEFT JOIN Linee_Preparazioni lp ")
                    Stb.AppendLine("    on agenda.PREPARAZIONE_COD = lp.Preparazione_Cod ")

                    Stb.AppendLine(" WHERE ta.FF_TrackedData_Cod = " & Agro_SQL_SaveNum(FF_TrackedData_Cod))
                    Stb.AppendLine(" AND ta.Visibile = 1")  'per compatibilità con visualizzazione a grafo

                Else
                    Stb.AppendLine(" WHERE 1 = 1  ")

                    '--------------------------
                    'modifica per magazzino e contabilita
                    If Sa_Cod <> 0 Then
                        Stb.AppendLine(" AND (  Agenda.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " OR Agenda.Sa_Cod = 0 )  ")
                    Else
                        If FiltroCentri <> "" Then
                            Stb.AppendLine(" AND  (Agenda.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") OR Agenda.Sa_Cod = 0 ) ")
                        End If
                    End If
                    '--------------------------
                End If

                If Filtro_GruppoOperazioni <> "" Then
                    Stb.AppendLine(" AND ( " & Filtro_GruppoOperazioni & " ) ")
                End If

                If FiltroOperazioni <> "" Then
                    Stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(FiltroOperazioni))
                End If

                If Tipo <> "" Then
                    Stb.AppendLine(" AND GruppoOperazioni.Tipo = '" & Agro_SQL_SaveText(Tipo) & "'")
                End If

                If Gru_Cod <> 0 Then
                    Stb.AppendLine(" AND GruppoOperazioni.GRU_COD = " & Agro_SQL_SaveNum(Gru_Cod))
                End If

                '--------------------------
                'modifica per magazzino e contabilita (1000,1001,1020,1021,1022,1023,1025,1031,1033
                'venivano escluse le operazioni contabili e di magazzino
                Stb.AppendLine(" AND ( Agenda.Lav_Cod in (" &
                                      CStr(LAVCOD_BOLLA_RICEVUTA) &
                                "," & CStr(LAVCOD_BOLLA_EMESSA) &
                                "," & CStr(LAVCOD_DISTINTA_CARICO) &
                                "," & CStr(LAVCOD_DISTINTA_CARICO_ACCETTAZIONE) &
                                "," & CStr(LAVCOD_AUTO_DDT_EMESSO) &
                                "," & CStr(LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE) &
                                "," & CStr(LAVCOD_ACCETTAZIONE_DIVERSI) &
                                "," & CStr(LAVCOD_MVV_EMESSO) &
                                "," & CStr(LAVCOD_MVV_RICEVUTO) &
                               ") ) ")
                '--------------------------

                '--------------------------
                'modifica per magazzino e contabilita
                Stb.AppendLine(" AND Movimenti.Cau_Mov IN ('" & CAU_REGISTRAZIONI & "','" & CAU_CARICO & "','" & CAU_SCARICO & "','" & CAU_TRASFERIMENTO & "', '" & CAU_ACCETTAZIONE_BENI_DA_DIVERSI & "') ")
                '--------------------------

                Stb.AppendLine(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio))
                Stb.AppendLine(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine))

                Stb.AppendLine(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleInizio))
                Stb.AppendLine(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine))

                Stb.AppendLine(" AND (Movimenti.Data_Movimento >= Imprese_progetti.validita_inizio OR Imprese_progetti.validita_inizio IS NULL)")
                Stb.AppendLine(" AND (Movimenti.Data_Movimento <= Imprese_progetti.validita_fine OR Imprese_progetti.validita_fine IS NULL) ")

                If xFiltroAggiuntivo_contabili <> "" Then
                    Stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_contabili) & "  ")
                End If
#End Region
                Stb.AppendLine(" ) ")
            End If

            If FF_TrackedData_Cod > 0 Then
                Stb.AppendLine(" UNION ")
            Else
                Stb.AppendLine(" UNION ALL ")
            End If

            Stb.AppendLine(" ( ")
#Region "RECUPERA LA MANUTENZIONE E LA REVISIONE MACCHINE"
            '/////////////////////////////////////////////////////////////////////////////////////////////
            '//////////// RECUPERA LA MANUTENZIONE E LA REVISIONE MACCHINE ///////////////////////////////
            '/////////////////////////////////////////////////////////////////////////////////////////////
            Stb.AppendLine("--RECUPERA LA MANUTENZIONE E LA REVISIONE MACCHINE")

            Stb.AppendLine(" SELECT Agenda.Piva ")
            Stb.AppendLine("        , Agenda.Sa_Cod ")
            Stb.AppendLine("        , Agenda.Id_Agenda ")
            Stb.AppendLine("        , 0 AS Id_Mov ")
            Stb.AppendLine("        , 0 As Id_Mov_Det ")
            Stb.AppendLine("        , Agenda.Lav_Cod ")
            Stb.AppendLine("        , Agenda.Des_Lib ")
            Stb.AppendLine("        , Movimenti.Data_Movimento ")
            Stb.AppendLine("        , Movimenti.Ora ")
            Stb.AppendLine("        , Movimenti.Mov_Desc ")
            Stb.AppendLine("        , Agenda.Username_Creazione ")
            Stb.AppendLine("        , Movimenti.Cau_Mov ")
            Stb.AppendLine("        , 0 AS Cul_Cod ")
            Stb.AppendLine("        , 0 AS Veg_Cod ")
            Stb.AppendLine("        , '' AS Veg_Des ")
            Stb.AppendLine("        , Agenda.Blocco_Flag ")
            Stb.AppendLine("        , Agenda.Raccoglitore_Cod ")
            Stb.AppendLine("        , (Dettagli.Cognome + ' ' + Dettagli.Nome) AS Tecnico ")

            '(26/11/2024 AF) aggiunto Id_Destinazione
            Stb.AppendLine(" , -1 AS Id_Destinazione ")

            Stb.AppendLine("        , -1 AS Tipo_Destinazione ")
            If FF_TrackedData_Cod > 0 Then
                Stb.AppendLine("        , '' AS Fabbricato_Des ")
            End If
            Stb.AppendLine("        , 0 AS Appezza ")
            Stb.AppendLine("        , '' AS App_Nome ")
            Stb.AppendLine("        , 0 AS Elem_Cod ")
            Stb.AppendLine("        , 0 AS Mat_Cod ")
            Stb.AppendLine("        , 0 AS Pro_Cod ")
            Stb.AppendLine("        , '' AS Fr_Des ")
            Stb.AppendLine("        , '' AS Fer_Des ")
            Stb.AppendLine("        , '' AS Trap_Des ")
            Stb.AppendLine("        , '' AS Ins_Des ")
            Stb.AppendLine("        , '' AS Mat_Des ")
            Stb.AppendLine("        , '' AS Cod_Articolo ")
            Stb.AppendLine("        , ISNULL(Centri_Aziendali.Sa_Nome, '') AS sa_nome ")

            'modifica per avversita
            'Stb.AppendLine(" , null AS av_Des_Vol ")
            'Stb.AppendLine(" , null AS av_gru_Des ")

            Stb.AppendLine("        , i.rag_soc ")
            Stb.AppendLine("        , Operazioni.lav_des ")
            Stb.AppendLine("        , GruppoOperazioni.gru_Des ")
            Stb.AppendLine("        , GruppoOperazioni.tipo ")

            '(20/12/2016 fede) aggiunte varieta
            Stb.AppendLine("        , '' AS cul_des ")

            '(03/05/2017 MarcoG) aggiunti principi attivi
            Stb.AppendLine("        , '' AS PrincipiAttivi ")

            '(04/05/2017 MarcoG) aggiunti campi
            Stb.AppendLine("        , '' AS campo_des ")

            '(09/10/2017 MarcoG) aggiunto il campo contabilizzato per la pianificazione
            Stb.AppendLine("        , 0 AS contabilizzato ")

            '(26/11/2024 AF) aggiunto il cod_progetto
            Stb.AppendLine(" , 0 AS Progetto_Cod ")

            '(12/10/2017 MarcoG) aggiunto il lotto
            Stb.AppendLine("        , '' AS LottoProduzione ")

            '(12/10/2017 MarcoG) aggiunto le note a checkbox
            Stb.AppendLine("        , ISNULL(Note_Intervento.Nota_Des,'') AS Nota_Des ")

            '(13/10/2017 MarcoG) aggiunto la sup app e la sup trattata
            Stb.AppendLine("        , 0 AS ID_REG ")
            Stb.AppendLine("        , 0 AS SUP_APP ")
            Stb.AppendLine("        , 0 AS Sup_Trattata ")

            '(27/10/2017 MarcoG) aggiunto il lotto impianto (nome distinta, esercizio)
            Stb.AppendLine("        , '' AS LottoImpianto ")

            '(23/11/2017 MarcoG) aggiunte attività per altre operazioni
            Stb.AppendLine("        , '' AS AttivitaSigla ")
            Stb.AppendLine("        , '' AS AttivitaDesc ")

            '(09/02/2018 MarcoG)  aggiunte le diciture per le destinazioni dei terreni nudi
            Stb.AppendLine("        , 0 AS DestinazioneTerreniNudi_Cod ")
            Stb.AppendLine("        , '' AS DestinazioneTerreniNudi_Des ")

            '(09/05/2018 MarcoG)  aggiunta la dicitura per il rif di DDT e Fatture
            Stb.AppendLine("        , '' AS RifDdtFatture ")

            '(22/05/2018 MarcoG)  aggiunta la dicitura per le Ricette
            Stb.AppendLine("        , 0 AS Ricetta_Cod ")
            Stb.AppendLine("        , '' AS Ricetta_Numero ")

            '(23/08/2018 MarcoG)  aggiunta data di ultima modifica
            Stb.AppendLine("        , Agenda.Data_Modifica AS Data_Ultima_Modifica_Intervento ")

            '(05/12/2018 Fede)  aggiunta validita_inizio di Mov_Destinazioni per avere la Data Rilevata
            Stb.AppendLine("        , CONVERT(DateTime,'1900/01/01',120) AS validita_inizio_destinazione ")

            '(11/01/2023 Anny)  aggiunta quantità e unità di misura
            Stb.AppendLine("        , ROUND(coalesce(Movimenti_dettagli.Qta_Extra_Totale, 0), +4) AS Qta_Extra_Totale")
            Stb.AppendLine("        , coalesce(Movimenti_dettagli.Qta, 0) AS Qta")
            Stb.AppendLine("        , Movimenti_dettagli.QTA_EXTRA ")
            Stb.AppendLine("        , Movimenti_dettagli.Udm_Cod ")
            Stb.AppendLine("        , Movimenti_dettagli.Extra_Int ")
            Stb.AppendLine("        , coalesce(um1.UDM_SIM, '') AS UDM_SIM_ExtraInt")
            Stb.AppendLine("        , coalesce(um2.UDM_SIM, '') AS UDM_SIM_UDM_COD")
            Stb.AppendLine("        , Agenda.Data_Creazione AS Data_Creazione")
            '(12/02/2024 Anny)  aggiunta origine attività
            Stb.AppendLine("        , Agenda.Origine AS Origine")

            If Visualizza_Codici_AppezzaImpianti Then
                Stb.AppendLine("        , '' AS Riferimento_Alfanumerico_Appezzamento ")
                Stb.AppendLine("        , '' AS Codice_Impianto ")
            End If

            If Visualizza_KPIN_BlockName Then
                Stb.AppendLine("        , '' AS Zespri_Codice_kPIN ")
                Stb.AppendLine("        , '' AS Zespri_Block_Name ")
            End If

            If FF_TrackedData_Cod > 0 Then
                Stb.AppendLine("        , ISNULL(ta.Lotto_Padre, '') AS FF_Track_Lotto_Padre ")
                Stb.AppendLine("        , ISNULL(ta.Lotto, '') AS FF_Track_Lotto ")
                Stb.AppendLine("        , ISNULL(ta.Cal_Cod_Padre, '') AS FF_Track_Cal_Cod_Padre ")
                Stb.AppendLine("        , ISNULL(ta.Cal_Cod, '') AS FF_Track_Cal_Cod ")
                Stb.AppendLine("        , ta.Qta_Extra_Totale AS FF_Track_Qta_Extra_Totale ")
                Stb.AppendLine("        , 0 AS FF_Track_Qta_Contenitori ")
                Stb.AppendLine("        , 0 AS FF_Track_Qta_Imballi ")
                Stb.AppendLine("        , 0 AS FF_Linea_Cod ")
                Stb.AppendLine("        , 0 AS FF_codice_generazione ")

            End If

            Stb.AppendLine(" FROM #cte_Utenti_Dettagli AS Dettagli ")
            Stb.AppendLine("    INNER JOIN #cte_Agenda_Per_Piva AS Agenda ")
            Stb.AppendLine("    INNER JOIN Movimenti ")
            Stb.AppendLine("        ON Agenda.Sa_Cod = Movimenti.Sa_Cod ")
            Stb.AppendLine("        AND Agenda.Id_Agenda = Movimenti.Id_Agenda ")
            Stb.AppendLine("        ON  Dettagli.CodFisc = Agenda.Username_Creazione ")

            Stb.AppendLine("        LEFT JOIN Centri_Aziendali ")
            Stb.AppendLine("        ON Centri_Aziendali.Piva = Agenda.PIVA ")
            Stb.AppendLine("        AND Centri_Aziendali.Sa_Cod = Agenda.Sa_Cod")

            '(11/01/2023 Anny)  aggiunta quantità e unità di misura
            Stb.AppendLine("    INNER Join Movimenti_dettagli ")
            Stb.AppendLine("        ON Agenda.Sa_Cod = Movimenti_dettagli.Sa_Cod ")
            Stb.AppendLine("        AND  Agenda.Id_Agenda = Movimenti_dettagli.Id_Agenda")
            Stb.AppendLine("    LEFT JOIN UnitaMisura um1 on Movimenti_dettagli.Extra_Int = um1.UDM_COD")
            Stb.AppendLine("    LEFT JOIN UnitaMisura um2 on Movimenti_dettagli.Udm_Cod = um2.UDM_COD")

            'Modifica per gruppi operazioni e tipo
            Stb.AppendLine("    LEFT JOIN dbo.Operazioni ")
            Stb.AppendLine("        on Agenda.Lav_Cod = Operazioni.Lav_Cod ")
            Stb.AppendLine("    LEFT JOIN dbo.GruppoOperazioni ")
            Stb.AppendLine("        on Operazioni.GRU_OP = GruppoOperazioni.GRU_COD ")

            Stb.AppendLine("    INNER JOIN imprese i ")
            Stb.AppendLine("        on i.piva = Agenda.PIVA ")

            '(12/10/2017 MarcoG)  aggiunte note testuali e a checkbox
            Stb.AppendLine("   LEFT JOIN AgendaxNote ")
            Stb.AppendLine("   ON Agenda.Id_Agenda = AgendaxNote.Id_Agenda ")
            Stb.AppendLine("   LEFT JOIN Note_Intervento ")
            Stb.AppendLine("   ON AgendaxNote.Nota_Cod = Note_Intervento.Nota_Cod ")

            If FF_TrackedData_Cod > 0 Then

                Stb.AppendLine("   INNER JOIN FF_TrackedData_Agenda ta ")
                Stb.AppendLine("       on Movimenti.PIVA = ta.piva ")
                Stb.AppendLine("       AND Movimenti.Sa_Cod = ta.sa_cod ")
                Stb.AppendLine("       AND Movimenti.Id_Agenda = ta.id_Agenda ")
                Stb.AppendLine("       AND Movimenti.Id_Mov = ta.id_Mov ")

                'Stb.AppendLine(" LEFT JOIN Linee_Preparazioni lp " )
                'Stb.AppendLine("    on agenda.PREPARAZIONE_COD = lp.Preparazione_Cod " )

                Stb.AppendLine(" WHERE ta.FF_TrackedData_Cod = " & Agro_SQL_SaveNum(FF_TrackedData_Cod))
                Stb.AppendLine(" AND ta.Visibile = 1")  'per compatibilità con visualizzazione a grafo

            Else
                Stb.AppendLine(" WHERE 1 = 1")
            End If

            '--------------------------

            If Filtro_GruppoOperazioni <> "" Then
                Stb.AppendLine(" AND ( " & Filtro_GruppoOperazioni & " )")
            End If

            If FiltroOperazioni <> "" Then
                Stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(FiltroOperazioni))
            End If

            If Tipo <> "" Then
                Stb.AppendLine(" AND GruppoOperazioni.Tipo = '" & Agro_SQL_SaveText(Tipo) & "'")
            End If

            If Gru_Cod <> 0 Then
                Stb.AppendLine(" AND GruppoOperazioni.GRU_COD = " & Agro_SQL_SaveNum(Gru_Cod))
            End If

            '--------------------------

            'le visite non devono comparire
            Stb.AppendLine(" AND ( Agenda.Lav_Cod = " & LAVCOD_MANUTENZIONE_MACCHINE & " OR Agenda.Lav_Cod = " & LAVCOD_REVISIONE_MACCHINE & " )")

            Stb.AppendLine(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio))
            Stb.AppendLine(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine))

            Stb.AppendLine(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleInizio))
            Stb.AppendLine(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine))

            If xFiltroAggiuntivo_contabili_Macchine <> "" Then
                Stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_contabili_Macchine))
            End If
#End Region
            Stb.AppendLine(" ) ")

            If FF_TrackedData_Cod > 0 Then
                Stb.AppendLine(" UNION ")
            Else
                Stb.AppendLine(" UNION ALL ")
            End If

            Stb.AppendLine(" ( ")
#Region "RECUPERA AUDIT MONITORAGGI"
            '/////////////////////////////////////////////////////////////////////////////////////////////
            '//////////// RECUPERA AUDIT MONITORAGGI                       ///////////////////////////////
            '/////////////////////////////////////////////////////////////////////////////////////////////
            Stb.AppendLine("--RECUPERA AUDIT MONITORAGGI")

            Stb.AppendLine(" SELECT Agenda.Piva ")
            Stb.AppendLine("        , Agenda.Sa_Cod ")
            Stb.AppendLine("        , Agenda.Id_Agenda ")
            Stb.AppendLine("        , 0 AS Id_Mov ")
            Stb.AppendLine("        , 0 As Id_Mov_Det ")
            Stb.AppendLine("        , Agenda.Lav_Cod ")
            Stb.AppendLine("        , Agenda.Des_Lib ")
            Stb.AppendLine("        , Agenda.validita_inizio AS Data_Movimento ")
            Stb.AppendLine("        , '' AS Ora ")
            Stb.AppendLine("        , '' AS Mov_Desc ")
            Stb.AppendLine("        , Agenda.Username_Creazione ")
            Stb.AppendLine("        , '' AS Cau_Mov ")
            Stb.AppendLine("        , 0 AS Cul_Cod ")
            Stb.AppendLine("        , 0 AS Veg_Cod ")
            Stb.AppendLine("        , '' AS Veg_Des ")
            Stb.AppendLine("        , Agenda.Blocco_Flag ")
            Stb.AppendLine("        , Agenda.Raccoglitore_Cod ")
            Stb.AppendLine("        , (Dettagli.Cognome + ' ' + Dettagli.Nome) AS Tecnico ")

            '(26/11/2024 AF) aggiunto Id_Destinazione
            Stb.AppendLine(" , -1 AS Id_Destinazione ")

            Stb.AppendLine("        , -1 AS Tipo_Destinazione ")
            If FF_TrackedData_Cod > 0 Then
                Stb.AppendLine("        , '' AS Fabbricato_Des ")
            End If
            Stb.AppendLine("        , 0 AS Appezza ")
            Stb.AppendLine("        , '' AS App_Nome ")
            Stb.AppendLine("        , 0 AS Elem_Cod ")
            Stb.AppendLine("        , 0 AS Mat_Cod ")
            Stb.AppendLine("        , 0 AS Pro_Cod ")
            Stb.AppendLine("        , '' AS Fr_Des ")
            Stb.AppendLine("        , '' AS Fer_Des ")
            Stb.AppendLine("        , '' AS Trap_Des ")
            Stb.AppendLine("        , '' AS Ins_Des ")
            Stb.AppendLine("        , '' AS Mat_Des ")
            Stb.AppendLine("        , '' AS Cod_Articolo ")
            Stb.AppendLine("        , ISNULL(Centri_Aziendali.Sa_Nome, '') AS sa_nome ")

            'modifica per avversita
            'Stb.AppendLine(" , null AS av_Des_Vol ")
            'Stb.AppendLine(" , null AS av_gru_Des ")

            Stb.AppendLine("        , i.rag_soc ")
            Stb.AppendLine("        , Operazioni.lav_des ")
            Stb.AppendLine("        , GruppoOperazioni.gru_Des ")
            Stb.AppendLine("        , GruppoOperazioni.tipo ")

            '(20/12/2016 fede) aggiunte varieta
            Stb.AppendLine("        , '' AS cul_des ")

            '(03/05/2017 MarcoG) aggiunti principi attivi
            Stb.AppendLine("        , '' AS PrincipiAttivi ")

            '(04/05/2017 MarcoG) aggiunti campi
            Stb.AppendLine("        , '' AS campo_des ")

            '(09/10/2017 MarcoG) aggiunto il campo contabilizzato per la pianificazione
            Stb.AppendLine("        , 0 AS contabilizzato ")

            '(26/11/2024 AF) aggiunto il cod_progetto
            Stb.AppendLine(" , 0 AS Progetto_Cod ")

            '(12/10/2017 MarcoG) aggiunto il lotto
            Stb.AppendLine("        , '' AS LottoProduzione ")

            '(12/10/2017 MarcoG) aggiunto le note a checkbox
            Stb.AppendLine("        , ISNULL(Note_Intervento.Nota_Des,'') AS Nota_Des ")

            '(13/10/2017 MarcoG) aggiunto la sup app e la sup trattata
            Stb.AppendLine("        , 0 AS ID_REG ")
            Stb.AppendLine("        , 0 AS SUP_APP ")
            Stb.AppendLine("        , 0 AS Sup_Trattata ")

            '(27/10/2017 MarcoG) aggiunto il lotto impianto (nome distinta, esercizio)
            Stb.AppendLine("        , '' AS LottoImpianto ")

            '(23/11/2017 MarcoG) aggiunte attività per altre operazioni
            Stb.AppendLine("        , '' AS AttivitaSigla ")
            Stb.AppendLine("        , '' AS AttivitaDesc ")

            '(09/02/2018 MarcoG)  aggiunte le diciture per le destinazioni dei terreni nudi
            Stb.AppendLine("        , 0 AS DestinazioneTerreniNudi_Cod ")
            Stb.AppendLine("        , '' AS DestinazioneTerreniNudi_Des ")

            '(09/05/2018 MarcoG)  aggiunta la dicitura per il rif di DDT e Fatture
            Stb.AppendLine("        , '' AS RifDdtFatture ")

            '(22/05/2018 MarcoG)  aggiunta la dicitura per le Ricette
            Stb.AppendLine("        , 0 AS Ricetta_Cod ")
            Stb.AppendLine("        , '' AS Ricetta_Numero ")

            '(23/08/2018 MarcoG)  aggiunta data di ultima modifica
            Stb.AppendLine("        , Agenda.Data_Modifica AS Data_Ultima_Modifica_Intervento ")

            '(05/12/2018 Fede)  aggiunta validita_inizio di Mov_Destinazioni per avere la Data Rilevata
            Stb.AppendLine("        , CONVERT(DateTime,'1900/01/01',120) AS validita_inizio_destinazione ")

            '(11/01/2023 Anny)  aggiunta quantità e unità di misura
            Stb.AppendLine("        , ROUND(coalesce(Movimenti_dettagli.Qta_Extra_Totale, 0), +4) AS Qta_Extra_Totale")
            Stb.AppendLine("        , coalesce(Movimenti_dettagli.Qta, 0) AS Qta")
            Stb.AppendLine("        , Movimenti_dettagli.QTA_EXTRA ")
            Stb.AppendLine("        , Movimenti_dettagli.Udm_Cod ")
            Stb.AppendLine("        , Movimenti_dettagli.Extra_Int ")
            Stb.AppendLine("        , coalesce(um1.UDM_SIM, '') AS UDM_SIM_ExtraInt")
            Stb.AppendLine("        , coalesce(um2.UDM_SIM, '') AS UDM_SIM_UDM_COD")
            Stb.AppendLine("        , Agenda.Data_Creazione AS Data_Creazione")
            '(12/02/2024 Anny)  aggiunta origine attività
            Stb.AppendLine("        , Agenda.Origine AS Origine")

            If Visualizza_Codici_AppezzaImpianti Then
                Stb.AppendLine("        , '' AS Riferimento_Alfanumerico_Appezzamento ")
                Stb.AppendLine("        , '' AS Codice_Impianto ")
            End If

            If Visualizza_KPIN_BlockName Then
                Stb.AppendLine("        , '' AS Zespri_Codice_kPIN ")
                Stb.AppendLine("        , '' AS Zespri_Block_Name ")
            End If

            If FF_TrackedData_Cod > 0 Then
                Stb.AppendLine("        , '' AS FF_Track_Lotto_Padre ")
                Stb.AppendLine("        , '' AS FF_Track_Lotto ")
                Stb.AppendLine("        , 0 AS FF_Track_Cal_Cod_Padre ")
                Stb.AppendLine("        , 0 AS FF_Track_Cal_Cod ")
                Stb.AppendLine("        , 0 AS FF_Track_Qta_Extra_Totale ")
                Stb.AppendLine("        , 0 AS FF_Track_Qta_Contenitori ")
                Stb.AppendLine("        , 0 AS FF_Track_Qta_Imballi ")
                Stb.AppendLine("        , 0 AS FF_Linea_Cod ")
                Stb.AppendLine("        , 0 AS FF_codice_generazione ")

            End If

            Stb.AppendLine(" FROM #cte_Utenti_Dettagli AS Dettagli ")
            Stb.AppendLine("    INNER JOIN #cte_Agenda_Per_Piva AS Agenda ")
            Stb.AppendLine("    ON  Dettagli.CodFisc = Agenda.Username_Creazione ")

            Stb.AppendLine("        LEFT JOIN Centri_Aziendali ")
            Stb.AppendLine("        ON Centri_Aziendali.Piva = Agenda.PIVA ")
            Stb.AppendLine("        AND Centri_Aziendali.Sa_Cod = Agenda.Sa_Cod")

            '(11/01/2023 Anny)  aggiunta quantità e unità di misura
            Stb.AppendLine("    INNER Join Movimenti_dettagli ")
            Stb.AppendLine("        ON Agenda.Sa_Cod = Movimenti_dettagli.Sa_Cod ")
            Stb.AppendLine("        AND  Agenda.Id_Agenda = Movimenti_dettagli.Id_Agenda")
            Stb.AppendLine("    LEFT JOIN UnitaMisura um1 on Movimenti_dettagli.Extra_Int = um1.UDM_COD")
            Stb.AppendLine("    LEFT JOIN UnitaMisura um2 on Movimenti_dettagli.Udm_Cod = um2.UDM_COD")

            'Modifica per gruppi operazioni e tipo
            Stb.AppendLine("    LEFT JOIN dbo.Operazioni ")
            Stb.AppendLine("        on Agenda.Lav_Cod = Operazioni.Lav_Cod ")
            Stb.AppendLine("    LEFT JOIN dbo.GruppoOperazioni ")
            Stb.AppendLine("        on Operazioni.GRU_OP = GruppoOperazioni.GRU_COD ")

            Stb.AppendLine("    INNER JOIN imprese i ")
            Stb.AppendLine("        on i.piva = Agenda.PIVA ")

            '(12/10/2017 MarcoG)  aggiunte note testuali e a checkbox
            Stb.AppendLine("   LEFT JOIN AgendaxNote ")
            Stb.AppendLine("   ON Agenda.Id_Agenda = AgendaxNote.Id_Agenda ")
            Stb.AppendLine("   LEFT JOIN Note_Intervento ")
            Stb.AppendLine("   ON AgendaxNote.Nota_Cod = Note_Intervento.Nota_Cod ")

            'If FF_TrackedData_Cod > 0 Then

            '    Stb.AppendLine("   INNER JOIN FF_TrackedData_Agenda ta ")
            '    Stb.AppendLine("       on Movimenti.PIVA = ta.piva ")
            '    Stb.AppendLine("       AND Movimenti.Sa_Cod = ta.sa_cod ")
            '    Stb.AppendLine("       AND Movimenti.Id_Agenda = ta.id_Agenda ")
            '    Stb.AppendLine("       AND Movimenti.Id_Mov = ta.id_Mov ")

            '    Stb.AppendLine(" WHERE ta.FF_TrackedData_Cod = " & Agro_SQL_SaveNum(FF_TrackedData_Cod))

            'Else
            Stb.AppendLine(" WHERE 1 = 1")
            'End If

            '--------------------------

            If Filtro_GruppoOperazioni <> "" Then
                Stb.AppendLine(" AND ( " & Filtro_GruppoOperazioni & " )")
            End If

            If FiltroOperazioni <> "" Then
                Stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(FiltroOperazioni))
            End If

            If Tipo <> "" Then
                Stb.AppendLine(" AND GruppoOperazioni.Tipo = '" & Agro_SQL_SaveText(Tipo) & "'")
            End If

            If Gru_Cod <> 0 Then
                Stb.AppendLine(" AND GruppoOperazioni.GRU_COD = " & Agro_SQL_SaveNum(Gru_Cod))
            End If

            '--------------------------

            Stb.AppendLine(" AND Agenda.Lav_Cod IN ( " & LAVCOD_PRATICA_ECOLOGICA & "," & LAVCOD_FORMAZIONE & "," & LAVCOD_GESTIONE_RIFIUTI & ")")

            Stb.AppendLine(" AND Agenda.validita_inizio >= " & Agro_SQL_SaveDate(Validita_Inizio))
            Stb.AppendLine(" AND Agenda.validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine))

            Stb.AppendLine(" AND Agenda.validita_inizio >= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleInizio))
            Stb.AppendLine(" AND Agenda.validita_inizio <= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine))

            If xFiltroAggiuntivo_contabili_Audit <> "" Then
                Stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_contabili_Audit))
            End If
#End Region
            Stb.AppendLine(" ) ")

            If FF_TrackedData_Cod > 0 Then
                Stb.AppendLine(" UNION ")
            Else
                Stb.AppendLine(" UNION ALL ")
            End If

            Stb.AppendLine(" ( ")
#Region "VISITE"
            '/////////////////////////////////////////////////////////////////////////////////////////////
            '//////////// VISITE                       ///////////////////////////////
            '/////////////////////////////////////////////////////////////////////////////////////////////
            Stb.AppendLine("--VISITE")

            Stb.AppendLine(" SELECT Agenda.Piva ")
            Stb.AppendLine("        , Agenda.Sa_Cod ")
            Stb.AppendLine("        , Agenda.Id_Agenda ")
            Stb.AppendLine("        , 0 AS Id_Mov ")
            Stb.AppendLine("        , 0 As Id_Mov_Det ")
            Stb.AppendLine("        , Agenda.Lav_Cod ")
            Stb.AppendLine("        , Agenda.Des_Lib ")
            Stb.AppendLine("        , Movimenti.Data_Movimento AS Data_Movimento ")
            Stb.AppendLine("        , '' AS Ora ")
            Stb.AppendLine("        , '' AS Mov_Desc ")
            Stb.AppendLine("        , Agenda.Username_Creazione ")
            Stb.AppendLine("        , '' AS Cau_Mov ")
            Stb.AppendLine("        , 0 AS Cul_Cod ")
            Stb.AppendLine("        , 0 AS Veg_Cod ")
            Stb.AppendLine("        , '' AS Veg_Des ")
            Stb.AppendLine("        , Agenda.Blocco_Flag ")
            Stb.AppendLine("        , Agenda.Raccoglitore_Cod ")
            Stb.AppendLine("        , (Dettagli.Cognome + ' ' + Dettagli.Nome) AS Tecnico ")

            '(26/11/2024 AF) aggiunto Id_Destinazione
            Stb.AppendLine(" , -1 AS Id_Destinazione ")

            Stb.AppendLine("        , -1 AS Tipo_Destinazione ")
            If FF_TrackedData_Cod > 0 Then
                Stb.AppendLine("        , '' AS Fabbricato_Des ")
            End If
            Stb.AppendLine("        , 0 AS Appezza ")
            Stb.AppendLine("        , '' AS App_Nome ")
            Stb.AppendLine("        , 0 AS Elem_Cod ")
            Stb.AppendLine("        , 0 AS Mat_Cod ")
            Stb.AppendLine("        , 0 AS Pro_Cod ")
            Stb.AppendLine("        , '' AS Fr_Des ")
            Stb.AppendLine("        , '' AS Fer_Des ")
            Stb.AppendLine("        , '' AS Trap_Des ")
            Stb.AppendLine("        , '' AS Ins_Des ")
            Stb.AppendLine("        , '' AS Mat_Des ")
            Stb.AppendLine("        , '' AS Cod_Articolo ")
            Stb.AppendLine("        , ISNULL(Centri_Aziendali.Sa_Nome, '') AS sa_nome ")

            'modifica per avversita
            'Stb.AppendLine(" , null AS av_Des_Vol ")
            'Stb.AppendLine(" , null AS av_gru_Des ")

            Stb.AppendLine("        , i.rag_soc ")
            Stb.AppendLine("        , Operazioni.lav_des ")
            Stb.AppendLine("        , GruppoOperazioni.gru_Des ")
            Stb.AppendLine("        , GruppoOperazioni.tipo ")

            '(20/12/2016 fede) aggiunte varieta
            Stb.AppendLine("        , '' AS cul_des ")

            '(03/05/2017 MarcoG) aggiunti principi attivi
            Stb.AppendLine("        , '' AS PrincipiAttivi ")

            '(04/05/2017 MarcoG) aggiunti campi
            Stb.AppendLine("        , '' AS campo_des ")

            '(09/10/2017 MarcoG) aggiunto il campo contabilizzato per la pianificazione
            Stb.AppendLine("        , 0 AS contabilizzato ")

            '(26/11/2024 AF) aggiunto il cod_progetto
            Stb.AppendLine(" , 0 AS Progetto_Cod ")

            '(12/10/2017 MarcoG) aggiunto il lotto
            Stb.AppendLine("        , '' AS LottoProduzione ")

            '(12/10/2017 MarcoG) aggiunto le note a checkbox
            Stb.AppendLine("        , ISNULL(Note_Intervento.Nota_Des,'') AS Nota_Des ")

            '(13/10/2017 MarcoG) aggiunto la sup app e la sup trattata
            Stb.AppendLine("        , 0 AS ID_REG ")
            Stb.AppendLine("        , 0 AS SUP_APP ")
            Stb.AppendLine("        , 0 AS Sup_Trattata ")

            '(27/10/2017 MarcoG) aggiunto il lotto impianto (nome distinta, esercizio)
            Stb.AppendLine("        , '' AS LottoImpianto ")

            '(23/11/2017 MarcoG) aggiunte attività per altre operazioni
            Stb.AppendLine("        , '' AS AttivitaSigla ")
            Stb.AppendLine("        , '' AS AttivitaDesc ")

            '(09/02/2018 MarcoG)  aggiunte le diciture per le destinazioni dei terreni nudi
            Stb.AppendLine("        , 0 AS DestinazioneTerreniNudi_Cod ")
            Stb.AppendLine("        , '' AS DestinazioneTerreniNudi_Des ")

            '(09/05/2018 MarcoG)  aggiunta la dicitura per il rif di DDT e Fatture 
            Stb.AppendLine("        , '' AS RifDdtFatture ")

            '(22/05/2018 MarcoG)  aggiunta la dicitura per le Ricette
            Stb.AppendLine("        , 0 AS Ricetta_Cod ")
            Stb.AppendLine("        , '' AS Ricetta_Numero ")

            '(23/08/2018 MarcoG)  aggiunta data di ultima modifica
            Stb.AppendLine("        , Agenda.Data_Modifica AS Data_Ultima_Modifica_Intervento ")

            '(05/12/2018 Fede)  aggiunta validita_inizio di Mov_Destinazioni per avere la Data Rilevata
            Stb.AppendLine("        , CONVERT(DateTime,'1900/01/01',120) AS validita_inizio_destinazione ")

            '(11/01/2023 Anny)  aggiunta quantità e unità di misura
            Stb.AppendLine("        , ROUND(coalesce(Movimenti_dettagli.Qta_Extra_Totale, 0), +4) AS Qta_Extra_Totale")
            Stb.AppendLine("        , coalesce(Movimenti_dettagli.Qta, 0) AS Qta")
            Stb.AppendLine("        , Movimenti_dettagli.QTA_EXTRA ")
            Stb.AppendLine("        , Movimenti_dettagli.Udm_Cod ")
            Stb.AppendLine("        , Movimenti_dettagli.Extra_Int ")
            Stb.AppendLine("        , coalesce(um1.UDM_SIM, '') AS UDM_SIM_ExtraInt")
            Stb.AppendLine("        , coalesce(um2.UDM_SIM, '') AS UDM_SIM_UDM_COD")
            Stb.AppendLine("        , Agenda.Data_Creazione AS Data_Creazione")
            '(12/02/2024 Anny)  aggiunta origine attività
            Stb.AppendLine("        , Agenda.Origine AS Origine")

            If Visualizza_Codici_AppezzaImpianti Then
                Stb.AppendLine("        , '' AS Riferimento_Alfanumerico_Appezzamento ")
                Stb.AppendLine("        , '' AS Codice_Impianto ")
            End If

            If Visualizza_KPIN_BlockName Then
                Stb.AppendLine("        , '' AS Zespri_Codice_kPIN ")
                Stb.AppendLine("        , '' AS Zespri_Block_Name ")
            End If

            If FF_TrackedData_Cod > 0 Then
                Stb.AppendLine("        , '' AS FF_Track_Lotto_Padre ")
                Stb.AppendLine("        , '' AS FF_Track_Lotto ")
                Stb.AppendLine("        , 0 AS FF_Track_Cal_Cod_Padre ")
                Stb.AppendLine("        , 0 AS FF_Track_Cal_Cod ")
                Stb.AppendLine("        , 0 AS FF_Track_Qta_Extra_Totale ")
                Stb.AppendLine("        , 0 AS FF_Track_Qta_Contenitori ")
                Stb.AppendLine("        , 0 AS FF_Track_Qta_Imballi ")
                Stb.AppendLine("        , 0 AS FF_Linea_Cod ")
                Stb.AppendLine("        , 0 AS FF_codice_generazione ")

            End If

            Stb.AppendLine(" FROM #cte_Utenti_Dettagli AS Dettagli ")
            Stb.AppendLine("    INNER JOIN #cte_Agenda_Per_Piva AS Agenda ")
            Stb.AppendLine("    INNER JOIN Movimenti ")
            Stb.AppendLine("        ON Agenda.Sa_Cod = Movimenti.Sa_Cod ")
            Stb.AppendLine("        AND Agenda.Id_Agenda = Movimenti.Id_Agenda ")
            Stb.AppendLine("    ON  Dettagli.CodFisc = Agenda.Username_Creazione ")

            Stb.AppendLine("        LEFT JOIN Centri_Aziendali ")
            Stb.AppendLine("        ON Centri_Aziendali.Piva = Agenda.PIVA ")
            Stb.AppendLine("        AND Centri_Aziendali.Sa_Cod = Agenda.Sa_Cod")

            '(11/01/2023 Anny)  aggiunta quantità e unità di misura
            Stb.AppendLine("    INNER Join Movimenti_dettagli ")
            Stb.AppendLine("        ON Agenda.Sa_Cod = Movimenti_dettagli.Sa_Cod ")
            Stb.AppendLine("        AND  Agenda.Id_Agenda = Movimenti_dettagli.Id_Agenda")
            Stb.AppendLine("    LEFT JOIN UnitaMisura um1 on Movimenti_dettagli.Extra_Int = um1.UDM_COD")
            Stb.AppendLine("    LEFT JOIN UnitaMisura um2 on Movimenti_dettagli.Udm_Cod = um2.UDM_COD")

            'Modifica per gruppi operazioni e tipo
            Stb.AppendLine("    LEFT JOIN dbo.Operazioni ")
            Stb.AppendLine("        on Agenda.Lav_Cod = Operazioni.Lav_Cod ")
            Stb.AppendLine("    LEFT JOIN dbo.GruppoOperazioni ")
            Stb.AppendLine("        on Operazioni.GRU_OP = GruppoOperazioni.GRU_COD ")

            Stb.AppendLine("    INNER JOIN imprese i ")
            Stb.AppendLine("        on i.piva = Agenda.PIVA ")

            '(12/10/2017 MarcoG)  aggiunte note testuali e a checkbox
            Stb.AppendLine("   LEFT JOIN AgendaxNote ")
            Stb.AppendLine("   ON Agenda.Id_Agenda = AgendaxNote.Id_Agenda ")
            Stb.AppendLine("   LEFT JOIN Note_Intervento ")
            Stb.AppendLine("   ON AgendaxNote.Nota_Cod = Note_Intervento.Nota_Cod ")

            If FF_TrackedData_Cod > 0 Then

                Stb.AppendLine("   INNER JOIN FF_TrackedData_Agenda ta ")
                Stb.AppendLine("       on Movimenti.PIVA = ta.piva ")
                Stb.AppendLine("       AND Movimenti.Sa_Cod = ta.sa_cod ")
                Stb.AppendLine("       AND Movimenti.Id_Agenda = ta.id_Agenda ")
                Stb.AppendLine("       AND Movimenti.Id_Mov = ta.id_Mov ")

                Stb.AppendLine(" WHERE ta.FF_TrackedData_Cod = " & Agro_SQL_SaveNum(FF_TrackedData_Cod))
                Stb.AppendLine(" AND ta.Visibile = 1")  'per compatibilità con visualizzazione a grafo

            Else
                Stb.AppendLine(" WHERE 1 = 1")
            End If

            '--------------------------

            If Filtro_GruppoOperazioni <> "" Then
                Stb.AppendLine(" AND ( " & Filtro_GruppoOperazioni & " )")
            End If

            If FiltroOperazioni <> "" Then
                Stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(FiltroOperazioni))
            End If

            If Tipo <> "" Then
                Stb.AppendLine(" AND GruppoOperazioni.Tipo = '" & Agro_SQL_SaveText(Tipo) & "'")
            End If

            If Gru_Cod <> 0 Then
                Stb.AppendLine(" AND GruppoOperazioni.GRU_COD = " & Agro_SQL_SaveNum(Gru_Cod))
            End If

            If xFiltroAggiuntivo_Visite <> "" Then
                Stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_Visite))
            End If
            '--------------------------

            Stb.AppendLine(" AND Agenda.Lav_Cod = " & LAVCOD_VISITA)

            Stb.AppendLine(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio))
            Stb.AppendLine(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine))

            Stb.AppendLine(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleInizio))
            Stb.AppendLine(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine))

            'If xFiltroAggiuntivo_contabili_Audit <> "" Then
            '    Stb.AppendLine(" AND " & Agro_SQL_SaveText(xFiltroAggiuntivo_contabili_Audit))
            'End If
#End Region
            Stb.AppendLine(" ) ),")
#End Region

#Region "VISITE COLLEGATE A DEI RILIEVI"
            Stb.AppendLine(" ")
            Stb.AppendLine("-- VISITE COLLEGATE A DEI RILIEVI")

            Stb.AppendLine("#Visite_con_Rilievi AS (")

            Stb.AppendLine("select distinct Agenda.Id_Agenda AS Id_Agenda_Visita, Agenda.Piva AS Piva_Visita, Mov_Dettagli_Riferimenti.Id_Agenda_Rif AS Id_Agenda_Rilievo_Collegato_A_Visita")
            Stb.AppendLine("from Agenda ")
            Stb.AppendLine("INNER JOIN Movimenti on Movimenti.Piva = Agenda.Piva AND Movimenti.Id_Agenda = Agenda.Id_Agenda")
            Stb.AppendLine("INNER JOIN Movimenti_dettagli on Movimenti_dettagli.Piva = Agenda.Piva AND Movimenti_dettagli.Id_Agenda = Agenda.Id_Agenda")
            Stb.AppendLine("AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov")
            Stb.AppendLine("INNER JOIN Mov_Dettagli_Riferimenti on  Agenda.PIVA = Mov_Dettagli_Riferimenti.PIVA AND Agenda.Sa_Cod = Mov_Dettagli_Riferimenti.Sa_Cod AND")
            Stb.AppendLine("Agenda.Id_Agenda = Mov_Dettagli_Riferimenti.Id_Agenda AND Movimenti_dettagli.Id_Mov = Mov_Dettagli_Riferimenti.Id_Mov AND ")
            Stb.AppendLine("Movimenti_dettagli.Id_Mov_Det = Mov_Dettagli_Riferimenti.Id_Mov_Det AND Mov_Dettagli_Riferimenti.Lav_Cod_Rif IN (" & STR_OP_COLLEGABILI_A_VISITE_NG & ")")
            Stb.AppendLine("AND Mov_Dettagli_Riferimenti.Cau_Mov = '" & CAU_VISITE_ISPETTIVE & "'")
            Stb.AppendLine("where Agenda.Lav_Cod = " & LAVCOD_VISITA & " ")

            Stb.AppendLine("),")
#End Region

#Region "INSTALLAZIONI TRAPPOLE REINNESCATE"
            Stb.AppendLine(" ")
            Stb.AppendLine("-- INSTALLAZIONI TRAPPOLE REINNESCATE")

            Stb.AppendLine("#Installazioni_Trappole_Reinnescate AS (")

            Stb.AppendLine("select distinct #cte.Id_Agenda AS Id_Agenda_Installazione, #cte.PIVA AS Piva_Installazione, #cte.Sa_Cod AS Sa_Cod_Installazione")
            Stb.AppendLine("from #cte ")
            Stb.AppendLine("INNER JOIN Mov_Dettagli_Riferimenti on  #cte.PIVA = Mov_Dettagli_Riferimenti.PIVA AND #cte.Sa_Cod = Mov_Dettagli_Riferimenti.Sa_Cod AND")
            Stb.AppendLine("#cte.Id_Agenda = Mov_Dettagli_Riferimenti.Id_Agenda AND #cte.Id_Mov = Mov_Dettagli_Riferimenti.Id_Mov AND ")
            Stb.AppendLine("#cte.Id_Mov_Det = Mov_Dettagli_Riferimenti.Id_Mov_Det AND Mov_Dettagli_Riferimenti.Lav_Cod_Rif = " & LAVCOD_REINNESCO_TRAPPOLE)
            Stb.AppendLine("AND Mov_Dettagli_Riferimenti.Cau_Mov = '" & CAU_TRATTAMENTO & "'")
            Stb.AppendLine("where #cte.Lav_Cod = " & LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA & " ")

            Stb.AppendLine(")")
#End Region

            Stb.AppendLine("select #cte.*, ISNULL(#Visite_con_Rilievi.Id_Agenda_Visita, 0) AS Id_Agenda_Visita, ")
            Stb.AppendLine("    (")
            Stb.AppendLine("        CASE")
            Stb.AppendLine("        WHEN ( ISNULL(#Installazioni_Trappole_Reinnescate.Id_Agenda_Installazione,0) > 0)")
            Stb.AppendLine("            THEN 1 ")
            Stb.AppendLine("            ELSE 0 ")
            Stb.AppendLine("        END ")
            Stb.AppendLine("     ) AS Installazione_Trappola_Reinnescata from #cte")
            Stb.AppendLine("LEFT JOIN #Visite_con_Rilievi on #Visite_con_Rilievi.Id_Agenda_Rilievo_Collegato_A_Visita = #cte.Id_Agenda AND #Visite_con_Rilievi.Piva_Visita = #cte.Piva")
            Stb.AppendLine("LEFT JOIN #Installazioni_Trappole_Reinnescate on #Installazioni_Trappole_Reinnescate.Id_Agenda_Installazione = #cte.Id_Agenda AND ")
            Stb.AppendLine("#Installazioni_Trappole_Reinnescate.Piva_Installazione = #cte.Piva AND #Installazioni_Trappole_Reinnescate.Sa_Cod_Installazione = #cte.Sa_Cod")

            If filtroAgende IsNot Nothing AndAlso filtroAgende.Count > 0 Then
                Stb.AppendLine("WHERE #cte.Id_Agenda IN (")
                Stb.AppendLine(Agro_SQL_Save_Clausola_IN(String.Join(", ", filtroAgende)))
                Stb.AppendLine(")")
            End If


            '---------------------------------------------------------------------------------------------------------------------------
            Stb.AppendLine(" ORDER BY")
            Stb.AppendLine("    #cte.Data_Movimento DESC ")
            If FF_TrackedData_Cod > 0 Then
                Stb.AppendLine("  , #cte.Ora DESC ")
            End If
            Stb.AppendLine("  , #cte.Id_Agenda DESC ")
            Stb.AppendLine("  , App_Nome ASC ")

            '--------------------------------------------------------------------------

            dt = EseguiQuery_Lettura(objParametri_Server, Stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            'Dim StbOrderBy As New System.Text.StringBuilder
            'StbOrderBy.AppendLine("    Data_Movimento DESC ")
            'If FF_TrackedData_Cod > 0 Then
            '    StbOrderBy.AppendLine("  , Ora DESC ")
            'End If
            'StbOrderBy.AppendLine("  , Id_Agenda DESC ")
            'StbOrderBy.AppendLine("  , App_Nome ASC ")

            'DT = (New DataView(DT) With {.Sort = StbOrderBy.ToString}).ToTable()


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        Finally
            Utility.VerificaChiudiConnessione(objParametri_Server, flagConnLocale)
        End Try

        Return dt

    End Function




    '##############################################################################################

    Public Function Leggi_x_Albero(ByVal Piva As String,
                                   ByVal Sa_Cod As Integer,
                                   ByVal Validita_Inizio As Date,
                                   ByVal Validita_Fine As Date,
                                   ByVal Tipo As String,
                                   ByRef objParametri_Server As AgronicaCoreParametri
                                   ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Operazioni_R.Leggi_x_Albero()"

        Dim messaggioErrore As String = ""
        Dim StbQuery As New Text.StringBuilder
        Dim dt As DataTable

        Try
            StbQuery.Length = 0

            StbQuery.Append(" SELECT    distinct    Agenda.PIVA, Agenda.Sa_Cod, Agenda.Id_Agenda, Agenda.Lav_Cod, Agenda.des_lib,  " & vbCrLf)
            StbQuery.Append(" 	GruppoOperazioni.Tipo, Movimenti.Data_Movimento " & vbCrLf)
            StbQuery.Append(" FROM            Agenda " & vbCrLf)
            StbQuery.Append(" 		INNER JOIN Operazioni ON Agenda.Lav_Cod = Operazioni.LAV_COD  " & vbCrLf)
            StbQuery.Append(" 		INNER JOIN GruppoOperazioni ON Operazioni.GRU_OP = GruppoOperazioni.GRU_COD  " & vbCrLf)
            StbQuery.Append(" 		INNER JOIN Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda  " & vbCrLf)
            StbQuery.Append(" WHERE Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "'   " & vbCrLf)
            If Sa_Cod <> 0 Then
                StbQuery.Append(" AND Agenda.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "    " & vbCrLf)
            End If

            If Tipo <> "" Then

                If Tipo.Contains(",") Then
                    StbQuery.Append(" AND GruppoOperazioni.Tipo in " & Agro_SQL_Save_Clausola_IN(Tipo, True) & "   " & vbCrLf)
                Else
                    StbQuery.Append(" AND GruppoOperazioni.Tipo = '" & Agro_SQL_SaveText(Tipo) & "'   " & vbCrLf)
                End If
            End If

            StbQuery.Append(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio) & "   " & vbCrLf)
            StbQuery.Append(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine) & "   " & vbCrLf)


            StbQuery.Append(" order by Data_Movimento   " & vbCrLf)


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Server, StbQuery.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
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
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[magnani]	28/04/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function Numero_Totale_Operazioni(ByRef objParametri As AgronicaCoreParametri) As Integer

        Dim num As Integer = 0
        Dim dt As DataTable

        dt = Leggi(0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)

        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            num = dt.Rows.Count
        End If

        Return num

    End Function

    <CacheableAttribute(False)>
    Public Function Leggi_Numero_Operazioni_PerMenuAgenda(ByVal Piva As String,
                                                          ByVal Sa_Cod As Integer,
                                                          ByVal Validita_Inizio As Date,
                                                          ByVal Validita_Fine As Date,
                                                          ByVal Veg_Cod As Integer,
                                                          ByVal Gru_Cod As Integer,
                                                          ByVal FiltroOperazioni As String,
                                                          ByVal Filtro_GruppoOperazioni As String,
                                                          ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                          ByRef objParametri_Server As AgronicaCoreParametri
                                                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Operazioni_R.Leggi_Numero_Operazioni_PerMenuAgenda()"

        Dim messaggioErrore As String = ""
        Dim Stb As New Text.StringBuilder
        Dim dt As DataTable

        Try

            Dim FiltroCentri As String = ""

            If Sa_Cod = 0 Then
                'leggo se ci sono filtri sui centri
                FiltroCentri = EstraiFiltroCentri(Piva, objParametri_Server)
            End If

            Stb.Length = 0

            Stb.AppendLine("SET NOCOUNT ON ")
            Stb.AppendLine(" DECLARE @TabellaAppoggio TABLE(piva VARCHAR(25), id_Agenda INT, lav_cod INT, lav_codXAttivita INT, veg_cod INT) ")
            Stb.AppendLine(" INSERT INTO @TabellaAppoggio ")
            Stb.AppendLine(" SELECT Agenda.Piva, Agenda.Id_Agenda, Agenda.lav_cod, AttivitaXOperazioni.lav_cod, filtro.veg_cod ")
            Stb.AppendLine(" FROM Agenda ")
            Stb.AppendLine(" LEFT JOIN AttivitaXOperazioni ON Agenda.Id_Attivita = AttivitaXOperazioni.ID_Attivita ")
            Stb.AppendLine(" LEFT JOIN Operazioni ON Agenda.Lav_Cod = Operazioni.Lav_Cod ")
            Stb.AppendLine(" LEFT JOIN GruppoOperazioni ON Operazioni.Gru_Op = GruppoOperazioni.Gru_Cod ")

            Stb.AppendLine(" LEFT JOIN (SELECT mov_destinazioni.piva, mov_destinazioni.id_agenda, MAX(Cultivar.Veg_Cod) AS Veg_Cod ")
            Stb.AppendLine("            FROM mov_destinazioni ")
            Stb.AppendLine("            INNER JOIN Reg_Impianti ON mov_destinazioni.piva = Reg_Impianti.piva AND mov_destinazioni.Sa_Cod = Reg_Impianti.Sa_Cod AND mov_destinazioni.Appezza = Reg_Impianti.APPEZZA AND mov_destinazioni.id_destinazione = Reg_Impianti.ID_REG ")
            Stb.AppendLine("            INNER JOIN cultivar ON cultivar.Cul_Cod=Reg_Impianti.CUL_COD ")
            Stb.AppendLine("            GROUP BY mov_destinazioni.piva, mov_destinazioni.id_agenda ")

            If Sa_Cod > 0 Then
                Stb.AppendLine("            WHERE mov_destinazioni.sa_cod = " & Sa_Cod)
            End If

            Stb.AppendLine(" ) filtro ON filtro.piva = Agenda.piva AND filtro.id_agenda = Agenda.id_agenda ")


            Stb.AppendLine(" WHERE Agenda.Lav_Cod > 0 AND Agenda.Lav_Cod <> 1008  AND Agenda.Lav_Cod <> 4500 ")
            Stb.AppendLine(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "'")

            If Filtro_GruppoOperazioni <> "" Then
                Stb.AppendLine(" AND ( " & Filtro_GruppoOperazioni & " )")
            End If

            If FiltroOperazioni <> "" Then
                Stb.AppendLine(" AND " & Agro_SQL_SaveText(FiltroOperazioni))
            End If

            If Gru_Cod <> 0 Then
                Stb.AppendLine(" AND GruppoOperazioni.GRU_COD = " & Agro_SQL_SaveNum(Gru_Cod))
            End If

            Stb.AppendLine(" AND Agenda.Validita_Inizio >= " & Agro_SQL_SaveDate(If(Validita_Inizio > objParametri_Server.FinestraTemporaleInizio, Validita_Inizio, objParametri_Server.FinestraTemporaleInizio)))
            Stb.AppendLine(" AND Agenda.Validita_Inizio <= " & Agro_SQL_SaveDate(If(Validita_Fine < objParametri_Server.FinestraTemporaleFine, Validita_Fine, objParametri_Server.FinestraTemporaleFine)))

            Stb.AppendLine("")
            '--------------------------

            Stb.AppendLine(" DECLARE @TabellaAppoggioRicette TABLE(ricetta_operazione_cod INT, W_Anagrafica_Stati_Cod INT) ")
            Stb.AppendLine(" INSERT INTO @TabellaAppoggioRicette ")
            Stb.AppendLine(" SELECT Ricette_Operazioni.ricetta_operazione_cod, Ricette_Operazioni.W_Anagrafica_Stati_Cod ")
            Stb.AppendLine(" FROM Ricette_Operazioni ")
            Stb.AppendLine(" INNER JOIN Ricette ON Ricette.Ricetta_Cod = Ricette_Operazioni.Ricetta_Cod ")

            If Veg_Cod > 0 OrElse Sa_Cod > 0 Then
                Stb.AppendLine(" INNER JOIN (SELECT TOP 1 Ricetta_Operazione_Cod ")
                Stb.AppendLine(" FROM Ricette_Destinazioni ")

                If Veg_Cod > 0 Then
                    Stb.AppendLine(" INNER JOIN Reg_Impianti ON Ricette_Destinazioni.piva = Reg_Impianti.piva AND Ricette_Destinazioni.Sa_Cod = Reg_Impianti.Sa_Cod AND Ricette_Destinazioni.Appezza = Reg_Impianti.APPEZZA AND Ricette_Destinazioni.Id_Reg = Reg_Impianti.ID_REG ")
                    Stb.AppendLine(" INNER JOIN cultivar ON cultivar.Cul_Cod=Reg_Impianti.CUL_COD ")
                    Stb.AppendLine(" WHERE Cultivar.Veg_Cod = " & Veg_Cod)
                End If

                If Sa_Cod > 0 Then
                    If Veg_Cod > 0 Then
                        Stb.AppendLine(" AND Ricette_Destinazioni.sa_cod = " & Sa_Cod)
                    Else
                        Stb.AppendLine(" WHERE Ricette_Destinazioni.sa_cod = " & Sa_Cod)
                    End If
                End If

                Stb.AppendLine(" ) filtro ON filtro.Ricetta_Operazione_Cod = Ricette_Operazioni.Ricetta_Operazione_Cod ")
            End If

            Stb.AppendLine(" WHERE Ricette_Operazioni.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri_Server.PivaSuperUser) & "'")
            Stb.AppendLine(" AND (Ricette.Piva = '' OR Ricette.Piva = '" & Agro_SQL_SaveText(Piva) & "')")

            Stb.AppendLine(" AND Ricette_Operazioni.Validita_Inizio >= " & Agro_SQL_SaveDate(If(Validita_Inizio > objParametri_Server.FinestraTemporaleInizio, Validita_Inizio, objParametri_Server.FinestraTemporaleInizio)))
            Stb.AppendLine(" AND Ricette_Operazioni.Validita_Inizio <= " & Agro_SQL_SaveDate(If(Validita_Fine < objParametri_Server.FinestraTemporaleFine, Validita_Fine, objParametri_Server.FinestraTemporaleFine)))

            Stb.AppendLine("")
            '--------------------------

            Stb.AppendLine(" SELECT ")
            If Veg_Cod > 0 Then
                Stb.AppendLine(" (SELECT COUNT(id_Agenda) FROM @TabellaAppoggio a WHERE veg_cod IS NULL OR veg_cod = " & Veg_Cod & ") AS op_TutteQdc, ")
                Stb.AppendLine(" (SELECT COUNT(id_Agenda) FROM @TabellaAppoggio a WHERE (Lav_Cod < 1000 OR Lav_Cod=5000 or Lav_Cod=5004) AND (lav_codXAttivita IS NULL OR lav_codXAttivita<>5007) AND veg_cod = " & Veg_Cod & ")  AS op_Colturali, ")
            Else
                Stb.AppendLine(" (SELECT COUNT(id_Agenda) FROM @TabellaAppoggio a) AS op_TutteQdc, ")
                Stb.AppendLine(" (SELECT COUNT(id_Agenda) FROM @TabellaAppoggio a WHERE (Lav_Cod < 1000 OR Lav_Cod=5000 or Lav_Cod=5004) AND (lav_codXAttivita IS NULL OR lav_codXAttivita<>5007)) AS op_Colturali, ")
            End If
            Stb.AppendLine(" (SELECT COUNT(id_Agenda) FROM @TabellaAppoggio a WHERE Lav_Cod IN (1000,1001,1002,1003,1020,1021,1022,1023,1025,1031,1033,1065,1079)) AS op_MagCont, ")
            Stb.AppendLine(" (SELECT COUNT(id_Agenda) FROM @TabellaAppoggio a WHERE Lav_Cod = 5005 OR Lav_Cod = 5006 OR Lav_Cod = 5007 OR Lav_Cod = 1080 OR lav_codXAttivita = 5007) AS op_Audit, ")
            Stb.AppendLine(" (SELECT COUNT(id_Agenda) FROM @TabellaAppoggio a WHERE Lav_Cod = 1500 OR Lav_Cod = 4000) AS op_Macchine, ")
            Stb.AppendLine(" (SELECT COUNT(id_Agenda) FROM @TabellaAppoggio a WHERE Lav_Cod >= 3000 AND Lav_Cod < 4000) AS op_Zoo, ")
            Stb.AppendLine(" (SELECT COUNT(ricetta_operazione_cod) FROM @TabellaAppoggioRicette a WHERE W_Anagrafica_Stati_Cod = 300) AS op_Ricette, ")
            Stb.AppendLine(" (SELECT COUNT(ricetta_operazione_cod) FROM @TabellaAppoggioRicette a WHERE W_Anagrafica_Stati_Cod = 301) AS op_Brogliaccio ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Server, Stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Leggi_Causali_Entrata_Prodotto(ByRef objParametri As AgronicaCoreParametri) As String

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_Causali()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim lavCod = New System.Collections.Generic.List(Of Integer) From {LAVCOD_DISTINTA_CARICO, LAVCOD_DISTINTA_CARICO_ACCETTAZIONE,
                LAVCOD_ACCETTAZIONE_DIVERSI, LAVCOD_AUTO_DDT_EMESSO, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE}
            Dim Operazione = From Operazioni In GiasContext.Operazioni
                             Where (lavCod.Contains(Operazioni.LAV_COD))
                             Select Operazioni.LAV_COD, Operazioni.LAV_DES

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            risposta = JsonConvert.SerializeObject(Operazione.ToList(), Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function



    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    Public Sub New()

    End Sub

    <CacheableAttribute(False)>
    Public Function Carica_Operazioni_ColturalixMenuAgenda_Fast_Senza_Avversita(
                            ByVal Piva As String,
                            ByVal Sa_Cod As Integer,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                            ByVal Veg_Cod As Integer,
                            ByVal Cul_Cod As Integer,
                            ByVal Tipo As String,
                            ByVal Gru_Cod As Integer,
                            ByVal Lav_Cod As Integer,
                            ByVal Flag_TerrenoNudo As Boolean,
                            ByVal Flag_Anche_Veg_Cod_Cul_Cod_0 As Boolean,
                            ByVal FiltroOperazioni As String,
                            ByVal FiltroImpianti As String,
                            ByVal Filtro_GruppoOperazioni As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri_Utenti As AgronicaCoreParametri,
                            ByRef objParametri_Server As AgronicaCoreParametri
                            ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Operazioni_R.Carica_Operazioni_ColturalixMenuAgenda_Fast_Senza_Avversita()"

        Dim messaggioErrore As String = ""
        Dim Stb As New Text.StringBuilder
        Dim dt As DataTable

        Try
            Dim NomeDB_Utenti As String = objParametri_Utenti.Recupera_NomeDB()
            Dim FiltroCentri As String = ""

            If Sa_Cod = 0 Then
                'leggo se ci sono filtri sui centri
                FiltroCentri = EstraiFiltroCentri(Piva, objParametri_Server)
            End If

            Stb.Length = 0
            Stb.AppendLine(" ( ")

            '/////////////////////////////////////////////////////////////////////////////////////////////
            '///// LA PRIMA RECUPERA LE OP COLTURALI CON I RELATIVI IMPIANTI E PRODOTTI IMPIEGATI ////////
            '/////////////////////////////////////////////////////////////////////////////////////////////


            Stb.AppendLine(" SELECT Agenda.Piva ")
            Stb.AppendLine(" , Agenda.Sa_Cod ")
            Stb.AppendLine(" , Agenda.Id_Agenda ")
            Stb.AppendLine(" , Movimenti_dettagli.Id_Mov_Det ")
            Stb.AppendLine(" , Agenda.Lav_Cod ")
            Stb.AppendLine(" , Agenda.Des_Lib ")
            Stb.AppendLine(" , Movimenti.Data_Movimento ")
            Stb.AppendLine(" , Movimenti.Ora ")
            Stb.AppendLine(" , Movimenti.Mov_Desc ")
            Stb.AppendLine(" , Movimenti.Cau_Mov ")
            Stb.AppendLine(" , ISNULL(Reg_Impianti.Cul_Cod, 0) AS Cul_Cod ")
            Stb.AppendLine(" , ISNULL(Cultivar.Veg_Cod, 0) AS Veg_Cod ")
            Stb.AppendLine(" , ISNULL(SpecieVegetali.Veg_Des, '') AS Veg_Des ")
            Stb.AppendLine(" , Agenda.Blocco_Flag ")
            Stb.AppendLine(" , ISNULL((Dettagli.Rag_Soc + Dettagli.Cognome + ' ' + Dettagli.Nome), 'N.D.') AS Tecnico ")
            Stb.AppendLine(" , ISNULL(Mov_Destinazioni.Tipo_Destinazione, -1) AS Tipo_Destinazione ")
            Stb.AppendLine(" , ISNULL(Appezzamento.APPEZZA, 0) AS Appezza ")
            Stb.AppendLine(" , ISNULL(Appezzamento.APP_NOME, '') AS App_Nome ")
            Stb.AppendLine(" , ISNULL(Movimenti_dettagli.Elem_Cod, 0) AS Elem_Cod ")
            Stb.AppendLine(" , ISNULL(Movimenti_dettagli.Mat_Cod, 0) AS Mat_Cod ")
            Stb.AppendLine(" , ISNULL(Movimenti_dettagli.Pro_Cod, 0) AS Pro_Cod ")
            Stb.AppendLine(" , ISNULL(FORMULATI.Fr_Des, '') AS Fr_Des ")
            Stb.AppendLine(" , ISNULL(FERTILIZZANTI.Fer_Des, '') AS Fer_Des ")
            Stb.AppendLine(" , ISNULL(TRAPPOLE.TRAP_DES, '') AS Trap_Des ")
            Stb.AppendLine(" , ISNULL(Materie_Prime.Mat_Des, '') AS Mat_Des ")
            Stb.AppendLine(" , ISNULL(Materie_Prime.Cod_Articolo, '') AS Cod_Articolo ")
            Stb.AppendLine(" , ISNULL(Centri_Aziendali.sa_nome , '') AS sa_nome ")

            'modifica per avversita
            'Stb.AppendLine(" , ISNULL(avversita.av_Des_Vol, '') AS av_Des_Vol ")
            'Stb.AppendLine(" , ISNULL(GruppoAvversita.av_gru_Des, '') AS av_gru_Des ")

            Stb.AppendLine(" , i.rag_soc ")
            Stb.AppendLine(" , Operazioni.lav_des ")
            Stb.AppendLine(" , GruppoOperazioni.gru_Des ")

            '(20/12/2016 fede) aggiunte varieta
            Stb.AppendLine(" , ISNULL(Cultivar.Cul_des, '') AS cul_des ")

            '(03/05/2017 MarcoG) aggiunti principi attivi
            Stb.AppendLine(" , ISNULL(Movimenti_dettagli.PrincipiAttivi, '') AS PrincipiAttivi ")

            '(04/05/2017 MarcoG) aggiunti campi
            Stb.AppendLine(" , ISNULL(Campi.Campo_des, '') AS campo_des ")

            '(09/10/2017 MarcoG) aggiunto il campo contabilizzato per la pianificazione
            Stb.AppendLine(" , ISNULL(Movimenti_dettagli.Contabilizzato, 0) AS contabilizzato ")

            '(12/10/2017 MarcoG) aggiunto il lotto
            Stb.AppendLine(" , ISNULL(Movimenti_dettagli.lotto,'') AS LottoProduzione ")

            '(12/10/2017 MarcoG) aggiunto le note a checkbox
            Stb.AppendLine(" , ISNULL(Note_Intervento.Nota_Des,'') AS Nota_Des ")

            '(13/10/2017 MarcoG) aggiunto la sup app e la sup trattata
            Stb.AppendLine(" , ISNULL(Reg_Impianti.ID_REG, 0) AS ID_REG ")
            Stb.AppendLine(" , ISNULL(Reg_Impianti.sup_imp, 0) AS SUP_APP ")
            Stb.AppendLine(" , ISNULL(Mov_Destinazioni.Qta2, 0) AS Sup_Trattata ")

            '(27/10/2017 MarcoG) aggiunto il lotto impianto (nome distinta, esercizio)
            Stb.AppendLine(" , ISNULL(Imprese_Progetti.Progetto_Nome, '') AS LottoImpianto ")

            '(23/11/2017 MarcoG) aggiunte attività per altre operazioni
            Stb.AppendLine(" , ISNULL(Attivita.Sigla, '') AS AttivitaSigla ")
            Stb.AppendLine(" , ISNULL(Attivita.[Desc], '') AS AttivitaDesc ")

            '(09/02/2018 MarcoG)  aggiunte le diciture per le destinazioni dei terreni nudi
            Stb.AppendLine(" , ISNULL(Reg_Impianti_Codici.id_cod, 0) AS DestinazioneTerreniNudi_Cod ")
            Stb.AppendLine(" , ISNULL(Codici_Anagrafe.descrizione, '') AS DestinazioneTerreniNudi_Des ")

            '(22/05/2018 MarcoG)  aggiunta la dicitura per le Ricette
            Stb.AppendLine(" , ISNULL(RicettexAgenda.Ricetta_Cod, 0) AS Ricetta_Cod ")
            Stb.AppendLine(" , ISNULL(Ricette.Ricetta_Numero, '') AS Ricetta_Numero ")

            '(23/08/2018 MarcoG)  aggiunta data di ultima modifica
            Stb.AppendLine(" , Agenda.Data_Modifica AS Data_Ultima_Modifica_Intervento ")

            '(05/12/2018 Fede)  aggiunta validita_inizio di Mov_Destinazioni per avere la Data Rilevata
            Stb.AppendLine(" , ISNULL(Mov_Destinazioni.validita_inizio, CONVERT(DateTime,'1900/01/01',120)) AS validita_inizio_destinazione ")

            Stb.AppendLine(" FROM AGENDA ")

            Stb.AppendLine(" INNER JOIN Movimenti ")
            Stb.AppendLine("    ON Agenda.PIVA = Movimenti.PIVA ")
            Stb.AppendLine("    AND Agenda.Id_Agenda = Movimenti.Id_Agenda ")

            Stb.AppendLine(" LEFT JOIN  Movimenti_dettagli ")
            Stb.AppendLine("    ON Movimenti_dettagli.PIVA = Movimenti.PIVA ")
            Stb.AppendLine("    AND Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda ")
            Stb.AppendLine("    AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov  ")

            Stb.AppendLine(" LEFT JOIN Mov_Destinazioni ")
            Stb.AppendLine("    ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva ")
            Stb.AppendLine("    AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod ")
            Stb.AppendLine("    AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda ")
            Stb.AppendLine("    AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov  ")
            Stb.AppendLine("    AND Movimenti_dettagli.Id_Mov_det = Mov_Destinazioni.Id_Mov_det ")

            Stb.AppendLine(" LEFT JOIN Reg_Impianti  ")
            Stb.AppendLine("    ON Reg_Impianti.PIVA = Mov_Destinazioni.Piva ")
            Stb.AppendLine("    AND Reg_Impianti.SA_COD = Mov_Destinazioni.Sa_Cod ")
            Stb.AppendLine("    AND Reg_Impianti.APPEZZA = Mov_Destinazioni.Appezza ")
            Stb.AppendLine("    AND Reg_Impianti.ID_REG = Mov_Destinazioni.Id_Destinazione ")

            Stb.AppendLine(" LEFT JOIN Appezzamento  ")
            Stb.AppendLine("    ON Appezzamento.PIVA = Reg_Impianti.PIVA  ")
            Stb.AppendLine("    AND Appezzamento.SA_COD = Reg_Impianti.SA_COD  ")
            Stb.AppendLine("    AND Appezzamento.APPEZZA = Reg_Impianti.APPEZZA  ")

            '(12/10/2017 MarcoG) aggiunto per il lotto
            Stb.AppendLine(" LEFT JOIN Imprese_Progetti ")
            Stb.AppendLine("    ON Imprese_Progetti.Piva = Mov_Destinazioni.PIVA ")
            Stb.AppendLine("    AND Imprese_Progetti.Sa_Cod = Mov_Destinazioni.SA_COD ")
            Stb.AppendLine("    AND Imprese_Progetti.Appezza = Mov_Destinazioni.APPEZZA ")
            Stb.AppendLine("    AND Imprese_Progetti.Id_Reg = Mov_Destinazioni.id_destinazione ")

            Stb.AppendLine(" LEFT JOIN Cultivar  ")
            Stb.AppendLine("    ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod  ")

            Stb.AppendLine(" LEFT JOIN SpecieVegetali  ")
            Stb.AppendLine("    ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ")

            Stb.AppendLine(" LEFT JOIN Formulati  ")
            Stb.AppendLine("    ON Movimenti_dettagli.Pro_Cod = Formulati.Fr_Cod  ")

            Stb.AppendLine(" LEFT JOIN Fertilizzanti  ")
            Stb.AppendLine("    ON Movimenti_dettagli.Pro_Cod = Fertilizzanti.Fer_Cod  ")

            Stb.AppendLine(" LEFT JOIN Trappole  ")
            Stb.AppendLine("    ON Movimenti_dettagli.Pro_Cod = Trappole.TRAP_COD  ")

            Stb.AppendLine(" LEFT JOIN Materie_Prime  ")
            Stb.AppendLine("    ON Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod  ")
            Stb.AppendLine("    AND Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod  ")

            Stb.AppendLine(" LEFT JOIN " & NomeDB_Utenti & ".dbo.Utenti_Dettagli Dettagli  ")
            Stb.AppendLine("    ON Dettagli.CodFisc = Agenda.Username_Creazione  ")

            ''modifica per avversita
            'Stb.AppendLine("    LEFT JOIN Mov_Dettaglio_Tecnico  ")
            ''Stb.AppendLine("        on Mov_Dettaglio_Tecnico.PIVA = Movimenti.PIVA  " )
            ''Stb.AppendLine("        AND Mov_Dettaglio_Tecnico.Sa_Cod = Movimenti.Sa_Cod  " )
            ''Stb.AppendLine("        AND Mov_Dettaglio_Tecnico.Id_Agenda = Movimenti.Id_Agenda  " )
            ''Stb.AppendLine("        and Mov_Dettaglio_Tecnico.id_mov=Movimenti.id_mov  " )
            ''Stb.AppendLine("        and ( Mov_Dettaglio_Tecnico.av_cod<>0 or Mov_Dettaglio_Tecnico.av_gru<>0)  " )
            ''Grilli 02/05/2017 FACCIO IL JOIN SU MOV DET ANZICHE MOV. QUESTO FARA' Si' CHE NON SI VEDANO PIU' I NESSUNO/NESSUNO DELL'AGENDA
            ''Con la Fede siamo d'accordo che ora è inevitabile per associare la giusta avversità al giusto prodotto e poi "ci andremo dietro"
            'Stb.AppendLine("        on Mov_Dettaglio_Tecnico.PIVA = Movimenti_dettagli.PIVA  ")
            'Stb.AppendLine("        And Mov_Dettaglio_Tecnico.Sa_Cod = Movimenti_dettagli.Sa_Cod  ")
            'Stb.AppendLine("        And Mov_Dettaglio_Tecnico.Id_Agenda = Movimenti_dettagli.Id_Agenda  ")
            'Stb.AppendLine("        And Mov_Dettaglio_Tecnico.id_mov=Movimenti_dettagli.id_mov  ")
            'Stb.AppendLine("        And Mov_Dettaglio_Tecnico.id_mov_det=Movimenti_dettagli.id_mov_det  ")
            'Stb.AppendLine("        And ( Mov_Dettaglio_Tecnico.av_cod<>0 Or Mov_Dettaglio_Tecnico.av_gru<>0)  ")
            'Stb.AppendLine("  ")
            'Stb.AppendLine("    LEFT JOIN Avversita  ")
            'Stb.AppendLine("        On Avversita.av_cod=Mov_Dettaglio_Tecnico.av_cod  ")
            'Stb.AppendLine("        And Mov_Dettaglio_Tecnico.av_cod<>0   ")
            'Stb.AppendLine("  ")
            'Stb.AppendLine("    LEFT JOIN GruppoAvversita  ")
            'Stb.AppendLine("        On GruppoAvversita.av_gru=Mov_Dettaglio_Tecnico.av_gru  ")
            'Stb.AppendLine("        And Mov_Dettaglio_Tecnico.av_gru<>0  ")


            'Modifica per gruppi operazioni e tipo
            Stb.AppendLine(" LEFT JOIN Operazioni ")
            Stb.AppendLine("    ON Agenda.Lav_Cod = Operazioni.Lav_Cod ")

            Stb.AppendLine(" LEFT JOIN GruppoOperazioni ")
            Stb.AppendLine("    ON Operazioni.GRU_OP = GruppoOperazioni.GRU_COD ")

            Stb.AppendLine(" INNER JOIN imprese i ")
            Stb.AppendLine("    ON i.piva = Agenda.PIVA ")

            Stb.AppendLine(" LEFT JOIN Centri_Aziendali ")
            Stb.AppendLine("    ON  Centri_Aziendali.piva = Agenda.piva ")
            Stb.AppendLine("    AND Centri_Aziendali.sa_cod = Agenda.Sa_Cod ")

            '(04/05/2017 MarcoG) aggiunti campi
            Stb.AppendLine(" LEFT JOIN Campi ")
            Stb.AppendLine("   ON Campi.Piva = Appezzamento.Piva ")
            Stb.AppendLine("   AND Campi.Sa_Cod = Appezzamento.Sa_Cod ")
            Stb.AppendLine("   AND Campi.Campo_Cod = Appezzamento.Campo_Cod ")

            '(12/10/2017 MarcoG)  aggiunte note testuali e a checkbox
            Stb.AppendLine(" LEFT JOIN AgendaxNote ")
            Stb.AppendLine("   ON Agenda.Id_Agenda = AgendaxNote.Id_Agenda ")
            Stb.AppendLine(" LEFT JOIN Note_Intervento ")
            Stb.AppendLine("   ON AgendaxNote.Nota_Cod = Note_Intervento.Nota_Cod ")

            '(23/11/2017 MarcoG)  aggiunte attività per altre operazioni
            Stb.AppendLine(" LEFT JOIN Attivita ")
            Stb.AppendLine("   ON Agenda.Id_Attivita = Attivita.Id_Attivita ")

            '(09/02/2018 MarcoG)  aggiunte le diciture per le destinazioni dei terreni nudi
            Stb.AppendLine(" LEFT JOIN Reg_Impianti_Codici ")
            Stb.AppendLine("   ON Reg_Impianti.piva = Reg_Impianti_Codici.piva ")
            Stb.AppendLine("   AND Reg_Impianti.SA_COD = Reg_Impianti_Codici.sa_cod ")
            Stb.AppendLine("   AND Reg_Impianti.APPEZZA = Reg_Impianti_Codici.appezza ")
            Stb.AppendLine("   AND Reg_Impianti.ID_REG = Reg_Impianti_Codici.Id_Reg ")
            Stb.AppendLine("   AND Reg_Impianti.Cul_Cod=0 ")
            Stb.AppendLine("   AND Reg_Impianti_Codici.id_cod BETWEEN 3000 And 3999 ")
            Stb.AppendLine("   LEFT JOIN Codici_Anagrafe ")
            Stb.AppendLine("   ON Codici_Anagrafe.codice = Reg_Impianti_Codici.id_cod ")

            '(22/05/2018 MarcoG)  aggiunte le diciture per le ricette
            Stb.AppendLine(" LEFT JOIN RicettexAgenda ")
            Stb.AppendLine("   ON RicettexAgenda.id_agenda = Agenda.id_agenda AND RicettexAgenda.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri_Server.PivaSuperUser) & "'")
            Stb.AppendLine(" LEFT JOIN Ricette ")
            Stb.AppendLine("   ON RicettexAgenda.Ricetta_SuperUser = Ricette.Ricetta_SuperUser AND RicettexAgenda.Ricetta_Cod = Ricette.Ricetta_Cod ")

            '(30/10/2018 MarcoG)  tolgo dalle operazioni colturali le altre operazioni figlie delle visite
            Stb.AppendLine(" LEFT JOIN AttivitaXOperazioni ")
            Stb.AppendLine("   ON AttivitaXOperazioni.id_attivita = Agenda.id_attivita AND AttivitaXOperazioni.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri_Server.PivaSuperUser) & "'")


            Stb.AppendLine(" WHERE Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "'")

            '--------------------------
            'modifica per magazzino e contabilita
            If Sa_Cod <> 0 Then
                Stb.AppendLine(" AND Agenda.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
            Else
                If FiltroCentri <> "" Then
                    Stb.AppendLine(" AND Agenda.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ")
                End If
            End If
            'If Sa_Cod <> 0 Then
            '    StbQuery.Append(" AND ( Agenda.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   or   Agenda.Sa_Cod =0 )   " )
            'End If
            '--------------------------


            If Filtro_GruppoOperazioni <> "" Then
                Stb.AppendLine(" AND ( " & Filtro_GruppoOperazioni & " ) ")
            End If

            If FiltroOperazioni <> "" Then
                Stb.AppendLine(" AND " & Agro_SQL_SaveText(FiltroOperazioni))
            End If

            If FiltroImpianti <> "" Then
                Stb.AppendLine(" AND " & Agro_SQL_SaveText(FiltroImpianti))
            End If

            If Tipo <> "" Then
                Stb.AppendLine(" AND GruppoOperazioni.Tipo = '" & Agro_SQL_SaveText(Tipo) & "'")
            End If

            If Gru_Cod <> 0 Then
                Stb.AppendLine(" AND GruppoOperazioni.GRU_COD = " & Agro_SQL_SaveNum(Gru_Cod))
            End If


            '--------------------------
            'modifica per magazzino e contabilita (1000,1001,1020,1021,1022,1023,1025,1031,1033
            'venivano escluse le operazioni contabili e di magazzino
            Stb.AppendLine(" AND (Agenda.Lav_Cod < " & LAVCOD_FATTURA_RICEVUTA & ")  ")
            '--------------------------

            '(30/10/2018 MarcoG)  tolgo dalle operazioni colturali le altre operazioni figlie delle visite
            Stb.AppendLine(" AND (AttivitaXOperazioni.Lav_Cod IS NULL OR AttivitaXOperazioni.Lav_Cod <> " & LAVCOD_VISITA & ")  ")

            '--------------------------
            'modifica per magazzino e contabilita
            Stb.AppendLine(" AND Movimenti.Cau_Mov IN ('" & CAU_TRATTAMENTO & "','" & CAU_RILIEVO_CAMPO & "','" & CAU_RILIEVO_RACCOLTA & "','" & CAU_LAVORAZIONE & "') ")
            '--------------------------

            Stb.AppendLine(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio))
            Stb.AppendLine(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine))

            Stb.AppendLine(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleInizio))
            Stb.AppendLine(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine))

            Stb.AppendLine(" AND (Movimenti.Data_Movimento >= Imprese_progetti.validita_inizio OR Imprese_progetti.validita_inizio IS NULL)")
            Stb.AppendLine(" AND (Movimenti.Data_Movimento <= Imprese_progetti.validita_fine OR Imprese_progetti.validita_fine IS NULL) ")

            If Not Flag_TerrenoNudo Then
                If Flag_Anche_Veg_Cod_Cul_Cod_0 Then
                    If Veg_Cod <> 0 AndAlso Veg_Cod <> -1 Then
                        Stb.AppendLine(" AND ( Cultivar.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " )")
                    End If
                    If Cul_Cod <> 0 Then
                        Stb.AppendLine(" AND ( Reg_Impianti.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & " )")
                    End If
                Else
                    If Veg_Cod <> 0 AndAlso Veg_Cod <> -1 Then
                        Stb.AppendLine(" AND Cultivar.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod))
                    End If
                    If Cul_Cod <> 0 Then
                        Stb.AppendLine(" AND Reg_Impianti.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod))
                    End If
                End If
            Else
                If Flag_Anche_Veg_Cod_Cul_Cod_0 Then
                    Stb.AppendLine(" AND ( Reg_Impianti.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & " ) ")
                Else
                    Stb.AppendLine(" AND Reg_Impianti.Cul_Cod = 0 ")
                End If
            End If


            Stb.AppendLine(" ) ")
            Stb.AppendLine(" UNION ALL ")
            Stb.AppendLine(" ( ")

            '/////////////////////////////////////////////////////////////////////////////////////////////
            '///////////////// LE OP COLTURALI POST RACCOLTA //////////////
            '/////////////////////////////////////////////////////////////////////////////////////////////

            Stb.AppendLine(" SELECT Agenda.Piva ")
            Stb.AppendLine("    , Agenda.Sa_Cod ")
            Stb.AppendLine("    , Agenda.Id_Agenda ")
            Stb.AppendLine("    , Movimenti_dettagli.Id_Mov_Det ")
            Stb.AppendLine("    , Agenda.Lav_Cod ")
            Stb.AppendLine("    , Agenda.Des_Lib ")
            Stb.AppendLine("    , Movimenti.Data_Movimento ")
            Stb.AppendLine("    , Movimenti.Ora ")
            Stb.AppendLine("    , Movimenti.Mov_Desc ")
            Stb.AppendLine("    , Movimenti.Cau_Mov ")
            Stb.AppendLine("    , ISNULL(Cultivar.Cul_Cod, 0) AS Cul_Cod ")
            Stb.AppendLine("    , ISNULL(Cultivar.Veg_Cod, 0) AS Veg_Cod ")
            Stb.AppendLine("    , ISNULL(SpecieVegetali.Veg_Des, '') AS Veg_Des ")
            Stb.AppendLine("    , Agenda.Blocco_Flag ")
            Stb.AppendLine("    , ISNULL((Dettagli.Rag_Soc + Dettagli.Cognome + ' ' + Dettagli.Nome), 'N.D.') AS Tecnico ")
            Stb.AppendLine("    , ISNULL(Mov_Destinazioni.Tipo_Destinazione, -1) AS Tipo_Destinazione ")
            Stb.AppendLine("    , 0 AS Appezza ")
            Stb.AppendLine("    , '' AS App_Nome ")
            Stb.AppendLine("    , ISNULL(Movimenti_dettagli.Elem_Cod, 0) AS Elem_Cod ")
            Stb.AppendLine("    , ISNULL(Movimenti_dettagli.Mat_Cod, 0) AS Mat_Cod ")
            Stb.AppendLine("    , ISNULL(Movimenti_dettagli.Pro_Cod, 0) AS Pro_Cod ")
            Stb.AppendLine("    , ISNULL(FORMULATI.Fr_Des, '') AS Fr_Des ")
            Stb.AppendLine("    , '' AS Fer_Des ")
            Stb.AppendLine("    , '' AS Trap_Des ")
            Stb.AppendLine("    , ISNULL(Materie_Prime.Mat_Des, '') AS Mat_Des ")
            Stb.AppendLine("    , ISNULL(Materie_Prime.Cod_Articolo, '') AS Cod_Articolo ")
            Stb.AppendLine("    , ISNULL(Centri_Aziendali.sa_nome , '') AS sa_nome ")

            'modifica per avversita
            'Stb.AppendLine(" , ISNULL(avversita.av_Des_Vol, '') AS av_Des_Vol ")
            'Stb.AppendLine(" , ISNULL(GruppoAvversita.av_gru_Des, '') AS av_gru_Des ")

            Stb.AppendLine(" , i.rag_soc ")
            Stb.AppendLine(" , Operazioni.lav_des ")
            Stb.AppendLine(" , GruppoOperazioni.gru_Des ")

            '(20/12/2016 fede) aggiunte varieta
            Stb.AppendLine(" , ISNULL(Cultivar.Cul_des, '') AS cul_des ")

            '(03/05/2017 MarcoG) aggiunti principi attivi
            Stb.AppendLine(" , ISNULL(Movimenti_dettagli.PrincipiAttivi, '') AS PrincipiAttivi ")

            '(04/05/2017 MarcoG) aggiunti campi
            Stb.AppendLine(" , '' AS campo_des ")

            '(09/10/2017 MarcoG) aggiunto il campo contabilizzato per la pianificazione
            Stb.AppendLine(" , ISNULL(Movimenti_dettagli.Contabilizzato, 0) AS contabilizzato ")

            '(12/10/2017 MarcoG) aggiunto il lotto
            Stb.AppendLine(" , ISNULL(Movimenti_dettagli.lotto,'') AS LottoProduzione ")

            '(12/10/2017 MarcoG) aggiunto le note a checkbox
            Stb.AppendLine(" , ISNULL(Note_Intervento.Nota_Des,'') as Nota_Des ")

            '(13/10/2017 MarcoG) aggiunto la sup app e la sup trattata
            Stb.AppendLine(" , ISNULL(Reg_Impianti.ID_REG, 0) AS ID_REG ")
            Stb.AppendLine(" , ISNULL(Reg_Impianti.sup_imp, 0) AS SUP_APP ")
            Stb.AppendLine(" , ISNULL(Mov_Destinazioni.Qta2, 0) AS Sup_Trattata ")

            '(27/10/2017 MarcoG) aggiunto il lotto impianto (nome distinta, esercizio)
            Stb.AppendLine(" , ISNULL(Imprese_Progetti.Progetto_Nome, '') AS LottoImpianto ")

            '(23/11/2017 MarcoG) aggiunte attività per altre operazioni
            Stb.AppendLine(" , '' AS AttivitaSigla ")
            Stb.AppendLine(" , '' AS AttivitaDesc ")

            '(09/02/2018 MarcoG)  aggiunte le diciture per le destinazioni dei terreni nudi
            Stb.AppendLine(" , ISNULL(Reg_Impianti_Codici.id_cod, 0) AS DestinazioneTerreniNudi_Cod ")
            Stb.AppendLine(" , ISNULL(Codici_Anagrafe.descrizione, '') AS DestinazioneTerreniNudi_Des ")

            '(22/05/2018 MarcoG)  aggiunta la dicitura per le Ricette
            Stb.AppendLine(" , ISNULL(RicettexAgenda.Ricetta_Cod, 0) AS Ricetta_Cod ")
            Stb.AppendLine(" , ISNULL(Ricette.Ricetta_Numero, '') AS Ricetta_Numero ")

            '(23/08/2018 MarcoG)  aggiunta data di ultima modifica
            Stb.AppendLine(" , Agenda.Data_Modifica AS Data_Ultima_Modifica_Intervento ")

            '(05/12/2018 Fede)  aggiunta validita_inizio di Mov_Destinazioni per avere la Data Rilevata
            Stb.AppendLine(" , CONVERT(DateTime,'1900/01/01',120) AS validita_inizio_destinazione ")

            Stb.AppendLine(" FROM Agenda ")

            Stb.AppendLine(" INNER JOIN Movimenti ")
            Stb.AppendLine("    ON Agenda.PIVA = Movimenti.PIVA ")
            Stb.AppendLine("    AND Agenda.Id_Agenda = Movimenti.Id_Agenda ")

            Stb.AppendLine(" LEFT JOIN Movimenti_dettagli ")
            Stb.AppendLine("    ON Movimenti_dettagli.PIVA = Movimenti.PIVA ")
            Stb.AppendLine("    AND Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda ")
            Stb.AppendLine("    AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov ")

            Stb.AppendLine(" LEFT JOIN Mov_Destinazioni ")
            Stb.AppendLine("    ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva ")
            Stb.AppendLine("    AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod ")
            Stb.AppendLine("    AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda ")
            Stb.AppendLine("    AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov ")
            Stb.AppendLine("    AND Movimenti_dettagli.Id_Mov_det = Mov_Destinazioni.Id_Mov_det ")

            '(13/10/2017 MarcoG) aggiunto per la superficie trattata
            Stb.AppendLine(" LEFT JOIN Reg_Impianti ")
            Stb.AppendLine("    ON Reg_Impianti.PIVA = Mov_Destinazioni.Piva ")
            Stb.AppendLine("    AND Reg_Impianti.SA_COD = Mov_Destinazioni.Sa_Cod ")
            Stb.AppendLine("    AND Reg_Impianti.APPEZZA = Mov_Destinazioni.Appezza ")
            Stb.AppendLine("    AND Reg_Impianti.ID_REG = Mov_Destinazioni.Id_Destinazione ")

            '(12/10/2017 MarcoG) aggiunto per il lotto
            Stb.AppendLine(" LEFT JOIN Imprese_Progetti ")
            Stb.AppendLine("    ON Imprese_Progetti.Piva = Mov_Destinazioni.PIVA ")
            Stb.AppendLine("    AND Imprese_Progetti.Sa_Cod = Mov_Destinazioni.SA_COD ")
            Stb.AppendLine("    AND Imprese_Progetti.Appezza = Mov_Destinazioni.APPEZZA ")
            Stb.AppendLine("    AND Imprese_Progetti.Id_Reg = Mov_Destinazioni.id_destinazione ")

            Stb.AppendLine(" LEFT JOIN Materie_Prime ")
            Stb.AppendLine("    ON Materie_Prime.Elem_Cod = Movimenti_dettagli.Elem_Cod ")
            Stb.AppendLine("    AND Materie_Prime.Mat_Cod = Movimenti_dettagli.Mat_Cod ")

            Stb.AppendLine(" LEFT JOIN SpecieVegetali ")
            Stb.AppendLine("    ON SpecieVegetali.Veg_Cod = Materie_Prime.Veg_Cod ")

            Stb.AppendLine(" LEFT JOIN Cultivar ")
            Stb.AppendLine("    ON Materie_Prime.Cul_Cod = Cultivar.Cul_Cod ")
            Stb.AppendLine("    AND SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod  ")

            Stb.AppendLine(" LEFT JOIN Formulati ")
            Stb.AppendLine("    ON Movimenti_dettagli.Pro_Cod = Formulati.Fr_Cod ")

            Stb.AppendLine(" LEFT JOIN " & NomeDB_Utenti & ".dbo.Utenti_Dettagli AS Dettagli ")
            Stb.AppendLine("    ON  Dettagli.CodFisc = Agenda.Username_Creazione ")

            Stb.AppendLine(" LEFT JOIN Operazioni  ")
            Stb.AppendLine("    ON Agenda.Lav_Cod = Operazioni.LAV_COD ")

            Stb.AppendLine(" LEFT JOIN GruppoOperazioni ")
            Stb.AppendLine("    ON GruppoOperazioni.GRU_COD = Operazioni.GRU_OP ")

            Stb.AppendLine(" INNER JOIN imprese i ")
            Stb.AppendLine("    ON i.piva = Agenda.PIVA ")

            Stb.AppendLine(" LEFT JOIN Centri_Aziendali ")
            Stb.AppendLine("    ON  Centri_Aziendali.piva = Agenda.piva ")
            Stb.AppendLine("    AND Centri_Aziendali.sa_cod = Agenda.Sa_Cod ")

            '(12/10/2017 MarcoG)  aggiunte note testuali e a checkbox
            Stb.AppendLine(" LEFT JOIN AgendaxNote ")
            Stb.AppendLine("    ON Agenda.Id_Agenda = AgendaxNote.Id_Agenda ")
            Stb.AppendLine(" LEFT JOIN Note_Intervento ")
            Stb.AppendLine("    ON AgendaxNote.Nota_Cod = Note_Intervento.Nota_Cod ")

            '(09/02/2018 MarcoG)  aggiunte le diciture per le destinazioni dei terreni nudi
            Stb.AppendLine(" LEFT JOIN Reg_Impianti_Codici ")
            Stb.AppendLine("    ON Reg_Impianti.piva = Reg_Impianti_Codici.piva ")
            Stb.AppendLine("    AND Reg_Impianti.SA_COD = Reg_Impianti_Codici.sa_cod ")
            Stb.AppendLine("    AND Reg_Impianti.APPEZZA = Reg_Impianti_Codici.appezza ")
            Stb.AppendLine("    AND Reg_Impianti.ID_REG = Reg_Impianti_Codici.Id_Reg ")
            Stb.AppendLine("    AND Reg_Impianti.Cul_Cod=0 ")
            Stb.AppendLine("    AND Reg_Impianti_Codici.id_cod BETWEEN 3000 And 3999 ")
            Stb.AppendLine(" LEFT JOIN Codici_Anagrafe ")
            Stb.AppendLine("    ON Codici_Anagrafe.codice = Reg_Impianti_Codici.id_cod ")

            '(22/05/2018 MarcoG)  aggiunte le diciture per le ricette
            Stb.AppendLine(" LEFT JOIN RicettexAgenda ")
            Stb.AppendLine("    ON RicettexAgenda.id_agenda = Agenda.id_agenda AND RicettexAgenda.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri_Server.PivaSuperUser) & "'")
            Stb.AppendLine(" LEFT JOIN Ricette ")
            Stb.AppendLine("    ON RicettexAgenda.Ricetta_SuperUser = Ricette.Ricetta_SuperUser AND RicettexAgenda.Ricetta_Cod = Ricette.Ricetta_Cod ")

            Stb.AppendLine(" WHERE Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "'")

            '--------------------------
            'modifica per magazzino e contabilita
            If Sa_Cod <> 0 Then
                Stb.AppendLine(" AND Agenda.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
            Else
                If FiltroCentri <> "" Then
                    Stb.AppendLine(" AND Agenda.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ")
                End If
            End If
            '--------------------------



            If Filtro_GruppoOperazioni <> "" Then
                Stb.AppendLine(" AND ( " & Filtro_GruppoOperazioni & " )")
            End If

            If FiltroOperazioni <> "" Then
                Stb.AppendLine(" AND " & Agro_SQL_SaveText(FiltroOperazioni))
            End If

            'ATTENZIONE, post raccolta non hanno impianti con specie collegato, quindi tolgo il filtro 
            If FiltroImpianti <> "" Then
                Dim FiltroImpianti2 As String = FiltroImpianti.Replace("Reg_Impianti", "Mov_Destinazioni")
                'devo togliere  and Reg_Impianti.Id_Reg in ( 74842113 )
                If FiltroImpianti2.IndexOf("Mov_Destinazioni.Id_Reg") > 0 Then
                    FiltroImpianti2 = FiltroImpianti2.Replace("Mov_Destinazioni.Id_Reg", " (1=1) ")
                    FiltroImpianti2 = FiltroImpianti2.Remove(FiltroImpianti2.Length - 30, 30)
                End If

                Stb.AppendLine(" AND " & Agro_SQL_SaveText(FiltroImpianti2))
            End If

            If Tipo <> "" Then
                Stb.AppendLine(" AND GruppoOperazioni.Tipo = '" & Agro_SQL_SaveText(Tipo) & "'")
            End If

            If Gru_Cod <> 0 Then
                Stb.AppendLine(" AND GruppoOperazioni.GRU_COD = " & Agro_SQL_SaveNum(Gru_Cod))
            End If


            '--------------------------
            'venivano escluse le operazioni contabili e di magazzino
            'Modifica per operazione di cura
            Stb.AppendLine(" AND (Agenda.Lav_Cod = " & LAVCOD_TRATTAMENTO_POST_RACCOLTA & " OR Agenda.Lav_Cod = " & LAVCOD_CURA & " OR Agenda.Lav_Cod = " & LAVCOD_TRASFORMAZIONI & ")")
            '--------------------------


            '--------------------------
            'modifica per magazzino e contabilita
            'Modifica per operazione di cura
            Stb.AppendLine(" AND Movimenti.Cau_Mov IN ('" & CAU_TRATTAMENTO & "','" & CAU_RILIEVO_CAMPO & "','" & CAU_RILIEVO_RACCOLTA & "','" & CAU_LAVORAZIONE & "','" & CAU_LINEA_PRODUZIONE & "','" & CAU_SCARICO & "')")

            '--------------------------

            Stb.AppendLine(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio))
            Stb.AppendLine(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine))

            Stb.AppendLine(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleInizio))
            Stb.AppendLine(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine))

            Stb.AppendLine(" AND (Movimenti.Data_Movimento >= Imprese_progetti.validita_inizio OR Imprese_progetti.validita_inizio IS NULL)")
            Stb.AppendLine(" AND (Movimenti.Data_Movimento <= Imprese_progetti.validita_fine OR Imprese_progetti.validita_fine IS NULL) ")

            If Veg_Cod <> 0 AndAlso Veg_Cod <> -1 Then
                Stb.AppendLine(" AND ( Cultivar.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ) ")
            End If

            If Cul_Cod <> 0 Then
                Stb.AppendLine(" AND ( Cultivar.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & " ) ")
            End If

            Stb.AppendLine(" ) ")


            '---------------------------------------------------------------------------------------------------------------------------
            Stb.AppendLine(" ORDER BY Movimenti.Data_Movimento DESC, Agenda.Id_Agenda DESC, App_Nome ASC ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Server, Stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    <CacheableAttribute(False)>
    Public Function Carica_Operazioni_MagContab(ByVal Piva As String,
                                                ByVal Sa_Cod As Integer,
                                                ByVal Validita_Inizio As Date,
                                                ByVal Validita_Fine As Date,
                                                ByVal Gru_Cod As Integer,
                                                ByVal Lav_Cod As Integer,
                                                ByVal FiltroOperazioni As String,
                                                ByVal FiltroImpianti As String,
                                                ByVal Filtro_GruppoOperazioni As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                ByRef objParametri_Server As AgronicaCoreParametri
                                                ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Operazioni_R.Carica_Operazioni_MagContab()"

        Dim messaggioErrore As String = ""
        Dim Stb As New Text.StringBuilder
        Dim dt As DataTable

        Try
            Dim NomeDB_Utenti As String = objParametri_Utenti.Recupera_NomeDB()
            Dim FiltroCentri As String = ""

            If Sa_Cod = 0 Then
                'leggo se ci sono filtri sui centri
                FiltroCentri = EstraiFiltroCentri(Piva, objParametri_Server)
            End If

            Stb.Length = 0

            '/////////////////////////////////////////////////////////////////////////////////////////////
            '///////////////// OP CONTABILI E PRODOTTI IMPIEGATI /////////////////////////////////////////
            '/////////////////////////////////////////////////////////////////////////////////////////////

            Stb.AppendLine(" SELECT ")
            Stb.AppendLine("   Agenda.Piva ")
            Stb.AppendLine(" , Agenda.Sa_Cod ")
            Stb.AppendLine(" , Agenda.Id_Agenda ")
            Stb.AppendLine(" , Agenda.Lav_Cod ")
            Stb.AppendLine(" , Agenda.Des_Lib ")
            Stb.AppendLine(" , Movimenti.Data_Movimento ")
            Stb.AppendLine(" , Movimenti.Ora ")
            'Stb.AppendLine(" , Movimenti.Mov_Desc ")
            Stb.AppendLine(" , Movimenti.Cau_Mov ")
            Stb.AppendLine(" , 0 AS Cul_Cod ")
            Stb.AppendLine(" , 0 AS Veg_Cod ")
            Stb.AppendLine(" , '' AS Veg_Des ")
            Stb.AppendLine(" , Agenda.Blocco_Flag ")
            Stb.AppendLine(" , ISNULL((Dettagli.Rag_Soc + Dettagli.Cognome + ' ' + Dettagli.Nome), 'N.D.') AS Tecnico ")
            Stb.AppendLine(" , ISNULL(Mov_Destinazioni.Tipo_Destinazione, -1) AS Tipo_Destinazione ")
            Stb.AppendLine(" , ISNULL(Movimenti_dettagli.Elem_Cod, 0) AS Elem_Cod ")
            Stb.AppendLine(" , ISNULL(Movimenti_dettagli.Mat_Cod, 0) AS Mat_Cod ")
            Stb.AppendLine(" , ISNULL(Movimenti_dettagli.Pro_Cod, 0) AS Pro_Cod ")
            Stb.AppendLine(" , ISNULL(FORMULATI.Fr_Des, '') AS Fr_Des ")
            Stb.AppendLine(" , ISNULL(FERTILIZZANTI.Fer_Des, '') AS Fer_Des ")
            Stb.AppendLine(" , ISNULL(TRAPPOLE.TRAP_DES, '') AS Trap_Des ")
            Stb.AppendLine(" , ISNULL(Materie_Prime.Mat_Des, '') AS Mat_Des ")
            Stb.AppendLine(" , ISNULL(Materie_Prime.Cod_Articolo, '') AS Cod_Articolo ")
            Stb.AppendLine(" , ISNULL(Centri_Aziendali.sa_nome , '') AS sa_nome ")

            Stb.AppendLine(" , i.rag_soc ")
            Stb.AppendLine(" , Operazioni.lav_des ")

            '(20/12/2016 fede) aggiunte varieta
            Stb.AppendLine(" , '' AS cul_des ")

            '(09/10/2017 MarcoG) aggiunto il campo contabilizzato per la pianificazione
            Stb.AppendLine(" , ISNULL(Movimenti_dettagli.Contabilizzato, 0) AS Contabilizzato ")

            '(09/05/2018 MarcoG)  aggiunta la dicitura per il numero ed il rif di DDT e Fatture 
            Stb.AppendLine(" , Movimenti.Doc_Numero_Sin + CONVERT(varchar(10), CONVERT(int, Movimenti.Doc_Numero)) + Movimenti.Doc_Numero_Des AS numDdtFatture ")
            Stb.AppendLine(" , ISNULL(LTRIM(RTRIM(ContattiDdtFatture.rag_soc + ContattiDdtFatture.Cognome + ' ' + ContattiDdtFatture.Nome)) , '') AS RifDdtFatture ")

            '(22/05/2018 MarcoG)  aggiunta la dicitura per le Ricette
            Stb.AppendLine(" , 0 AS Ricetta_Cod ")

            '(23/08/2018 MarcoG)  aggiunta data di ultima modifica
            Stb.AppendLine(" , Agenda.Data_Modifica AS Data_Ultima_Modifica_Intervento ")

            '(25/10/2018 MarcoG)  aggiunto il centro e il magazzino di destinazione
            Stb.AppendLine(" , ISNULL(ca_dest.sa_nome, '') AS sa_nome_dest ")
            Stb.AppendLine(" , ISNULL(Fabbricati.Fabbricato_Des, '') AS Fabbricato_Des ")

            Stb.AppendLine("   FROM Agenda ")

            Stb.AppendLine("   INNER JOIN Movimenti ")
            Stb.AppendLine("         ON Agenda.PIVA = Movimenti.PIVA ")
            Stb.AppendLine("         AND Agenda.Id_Agenda = Movimenti.Id_Agenda  ")

            Stb.AppendLine("   LEFT JOIN Movimenti_dettagli  ")
            Stb.AppendLine("         ON Movimenti_dettagli.PIVA = Movimenti.PIVA ")
            Stb.AppendLine("         AND Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda ")
            Stb.AppendLine("         AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov ")

            Stb.AppendLine("   LEFT JOIN Formulati ")
            Stb.AppendLine("        ON Movimenti_dettagli.Pro_Cod = Formulati.Fr_Cod ")

            Stb.AppendLine("   LEFT JOIN Fertilizzanti ")
            Stb.AppendLine("        ON Movimenti_dettagli.Pro_Cod = Fertilizzanti.Fer_Cod ")

            Stb.AppendLine("   LEFT JOIN Trappole ")
            Stb.AppendLine("        ON Movimenti_dettagli.Pro_Cod = Trappole.TRAP_COD ")

            Stb.AppendLine("   LEFT JOIN Materie_Prime ")
            Stb.AppendLine("        ON Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod ")
            Stb.AppendLine("        AND Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod ")

            Stb.AppendLine("   LEFT JOIN " & NomeDB_Utenti & ".dbo.Utenti_Dettagli Dettagli ")
            Stb.AppendLine("         ON Dettagli.CodFisc = Agenda.Username_Creazione ")

            '26/10/2018 Grilli: modificato perché all'apparenza più sensato ed in più per togliere il join con sa_cod che mi blocca nelle op contabili perché i dettagli hanno sa_cod ma la movimenti ha sempre 0
            Stb.AppendLine("   LEFT JOIN Mov_Destinazioni  ")
            Stb.AppendLine("        ON Movimenti_dettagli.Piva = Mov_Destinazioni.Piva ")
            Stb.AppendLine("        AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda  ")
            Stb.AppendLine("        AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov   ")
            Stb.AppendLine("        AND Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det  ")

            'Modifica per gruppi operazioni e tipo
            Stb.AppendLine("   LEFT JOIN Operazioni  ")
            Stb.AppendLine("        ON Agenda.Lav_Cod = Operazioni.Lav_Cod  ")

            Stb.AppendLine("   LEFT JOIN GruppoOperazioni  ")
            Stb.AppendLine("        ON Operazioni.GRU_OP = GruppoOperazioni.GRU_COD  ")

            Stb.AppendLine("   INNER JOIN imprese i ")
            Stb.AppendLine("         ON i.piva = Agenda.PIVA ")

            Stb.AppendLine("   LEFT JOIN Centri_Aziendali ")
            Stb.AppendLine("         ON  Centri_Aziendali.piva = Agenda.piva ")
            Stb.AppendLine("         AND Centri_Aziendali.sa_cod = Agenda.Sa_Cod ")

            '(09/05/2018 MarcoG)  aggiunta la dicitura per il rif di DDT e Fatture 
            Stb.AppendLine("   LEFT JOIN Risorse_Umane AS Risorse_UmaneDdtFatture ")
            Stb.AppendLine("        ON Risorse_UmaneDdtFatture.Cod_RisUm = Movimenti.Cod_RisUm ")
            Stb.AppendLine("   LEFT JOIN Contatti AS ContattiDdtFatture ")
            Stb.AppendLine("        ON Risorse_UmaneDdtFatture.Cod_Contatto = ContattiDdtFatture.Cod_Contatto AND Risorse_UmaneDdtFatture.Piva = ContattiDdtFatture.Piva ")

            '(25/10/2018 MarcoG)  aggiunto il centro e il magazzino di destinazione
            Stb.AppendLine("   LEFT JOIN Centri_Aziendali ca_dest")
            Stb.AppendLine("        ON ca_dest.Piva = Mov_Destinazioni.Piva AND ca_dest.sa_cod = Mov_Destinazioni.sa_cod ")
            Stb.AppendLine("   LEFT JOIN Fabbricati ")
            Stb.AppendLine("        ON Fabbricati.Piva = Mov_Destinazioni.Piva AND Fabbricati.sa_cod = Mov_Destinazioni.sa_cod AND Fabbricati.Fabbricato_cod = Mov_Destinazioni.id_destinazione AND Mov_Destinazioni.tipo_destinazione = 20 ")

            Stb.AppendLine("   WHERE Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")

            '--------------------------
            'modifica per magazzino e contabilita
            If Sa_Cod <> 0 Then
                Stb.AppendLine("   AND (Agenda.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " OR Agenda.Sa_Cod = 0) ")
            Else
                If FiltroCentri <> "" Then
                    Stb.AppendLine("   AND (Agenda.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") OR Agenda.Sa_Cod = 0 ) ")
                End If
            End If
            '--------------------------


            If Filtro_GruppoOperazioni <> "" Then
                Stb.AppendLine("   AND (" & Filtro_GruppoOperazioni & ") ")
            End If

            If FiltroOperazioni <> "" Then
                Stb.AppendLine("   AND " & Agro_SQL_SaveText(FiltroOperazioni))
            End If

            If Gru_Cod <> 0 Then
                Stb.AppendLine("   AND GruppoOperazioni.GRU_COD = " & Agro_SQL_SaveNum(Gru_Cod))
            End If

            '--------------------------
            'modifica per magazzino e contabilita (1000,1001,1020,1021,1022,1023,1025,1031,1033
            'venivano escluse le operazioni contabili e di magazzino
            Stb.AppendLine("   AND Agenda.Lav_Cod in (" & LAVCOD_FATTURA_RICEVUTA & "," & LAVCOD_FATTURA_EMESSA & "," &
                            LAVCOD_VENDITA & "," & LAVCOD_CORRISPETTIVO_VENDITA_SFUSO & "," & LAVCOD_ACQUISTO & "," &
                            LAVCOD_CARICO & "," & LAVCOD_SCARICO & "," &
                            LAVCOD_BOLLA_RICEVUTA & "," & LAVCOD_BOLLA_EMESSA & "," & LAVCOD_TRASFERIMENTO & "," &
                            LAVCOD_PROCEDURA_LIQUIDAZIONE_SOCI & "," & LAVCOD_NOTA_ACCREDITO_EMESSA & "," & LAVCOD_NOTA_ACCREDITO_RICEVUTA & ")")
            '--------------------------

            '--------------------------
            'modifica per magazzino e contabilita
            Stb.AppendLine(" AND Movimenti.Cau_Mov IN ('" & CAU_REGISTRAZIONI & "','" & CAU_CARICO & "','" & CAU_SCARICO & "','" & CAU_TRASFERIMENTO & "')  ")
            '--------------------------

            Stb.AppendLine(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio))
            Stb.AppendLine(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine))

            Stb.AppendLine(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleInizio))
            Stb.AppendLine(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine))

            '---------------------------------------------------------------------------------------------------------------------------
            Stb.AppendLine(" ORDER BY Movimenti.Data_Movimento DESC, Agenda.Id_Agenda DESC ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Server, Stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    <CacheableAttribute(False)>
    Public Function Carica_Operazioni_Audit(ByVal Piva As String,
                                            ByVal Sa_Cod As Integer,
                                            ByVal Validita_Inizio As Date,
                                            ByVal Validita_Fine As Date,
                                            ByVal Gru_Cod As Integer,
                                            ByVal Lav_Cod As Integer,
                                            ByVal FiltroOperazioni As String,
                                            ByVal FiltroImpianti As String,
                                            ByVal Filtro_GruppoOperazioni As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri_Utenti As AgronicaCoreParametri,
                                            ByRef objParametri_Server As AgronicaCoreParametri
                                            ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Operazioni_R.Carica_Operazioni_Audit()"

        Dim messaggioErrore As String = ""
        Dim Stb As New Text.StringBuilder
        Dim dt As DataTable

        Try
            Dim NomeDB_Utenti As String = objParametri_Utenti.Recupera_NomeDB()
            Dim FiltroCentri As String = ""

            If Sa_Cod = 0 Then
                'leggo se ci sono filtri sui centri
                FiltroCentri = EstraiFiltroCentri(Piva, objParametri_Server)
            End If

            Stb.Length = 0
            Stb.AppendLine(" ( ")

            '/////////////////////////////////////////////////////////////////////////////////////////////
            '//////////// RECUPERA AUDIT MONITORAGGI                       ///////////////////////////////
            '/////////////////////////////////////////////////////////////////////////////////////////////

            Stb.AppendLine(" SELECT Agenda.Piva ")
            Stb.AppendLine(" , Agenda.Sa_Cod ")
            Stb.AppendLine(" , Agenda.Id_Agenda ")
            Stb.AppendLine(" , Agenda.Lav_Cod ")
            Stb.AppendLine(" , Agenda.Des_Lib ")
            Stb.AppendLine(" , Agenda.validita_inizio as Data_Movimento ")
            Stb.AppendLine(" , '' as Ora ")
            Stb.AppendLine(" , '' as Mov_Desc ")
            Stb.AppendLine(" , '' as Cau_Mov ")
            Stb.AppendLine(" , 0 AS Cul_Cod ")
            Stb.AppendLine(" , 0 AS Veg_Cod ")
            Stb.AppendLine(" , '' AS Veg_Des ")
            Stb.AppendLine(" , Agenda.Blocco_Flag ")
            Stb.AppendLine(" , (Dettagli.Rag_Soc + Dettagli.Cognome + ' ' + Dettagli.Nome) AS Tecnico ")

            Stb.AppendLine(" , i.rag_soc ")
            Stb.AppendLine(" , Operazioni.lav_des ")
            Stb.AppendLine(" , GruppoOperazioni.gru_Des ")
            Stb.AppendLine(" , GruppoOperazioni.tipo ")

            '(20/12/2016 fede) aggiunte varieta
            Stb.AppendLine(" , '' AS cul_des ")

            '(09/10/2017 MarcoG) aggiunto il campo contabilizzato per la pianificazione
            Stb.AppendLine(" , 0 AS contabilizzato ")

            '(12/10/2017 MarcoG) aggiunto le note a checkbox
            Stb.AppendLine(" , ISNULL(Note_Intervento.Nota_Des,'') as Nota_Des ")

            '(23/11/2017 MarcoG) aggiunte attività per altre operazioni
            Stb.AppendLine(" , '' AS AttivitaSigla ")
            Stb.AppendLine(" , '' AS AttivitaDesc ")

            '(22/05/2018 MarcoG)  aggiunta la dicitura per le Ricette
            Stb.AppendLine(" , 0 AS Ricetta_Cod ")

            '(23/08/2018 MarcoG)  aggiunta data di ultima modifica
            Stb.AppendLine(" , Agenda.Data_Modifica AS Data_Ultima_Modifica_Intervento ")

            Stb.AppendLine(" FROM Agenda ")
            Stb.AppendLine(" INNER JOIN " & NomeDB_Utenti & ".dbo.Utenti_Dettagli Dettagli ")
            Stb.AppendLine("    ON Dettagli.CodFisc = Agenda.Username_Creazione ")

            'Modifica per gruppi operazioni e tipo
            Stb.AppendLine(" LEFT JOIN Operazioni  ")
            Stb.AppendLine("    ON Agenda.Lav_Cod = Operazioni.Lav_Cod ")
            Stb.AppendLine(" LEFT JOIN GruppoOperazioni ")
            Stb.AppendLine("    ON Operazioni.GRU_OP = GruppoOperazioni.GRU_COD ")

            Stb.AppendLine(" INNER JOIN imprese i ")
            Stb.AppendLine("    ON i.piva = Agenda.PIVA ")

            '(12/10/2017 MarcoG)  aggiunte note testuali e a checkbox
            Stb.AppendLine(" LEFT JOIN AgendaxNote ")
            Stb.AppendLine("    ON Agenda.Id_Agenda = AgendaxNote.Id_Agenda ")
            Stb.AppendLine(" LEFT JOIN Note_Intervento ")
            Stb.AppendLine("    ON AgendaxNote.Nota_Cod = Note_Intervento.Nota_Cod ")

            Stb.AppendLine(" WHERE Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "'")

            '--------------------------

            If Filtro_GruppoOperazioni <> "" Then
                Stb.AppendLine(" AND ( " & Filtro_GruppoOperazioni & " )")
            End If

            If FiltroOperazioni <> "" Then
                Stb.AppendLine(" AND " & Agro_SQL_SaveText(FiltroOperazioni))
            End If

            If Gru_Cod <> 0 Then
                Stb.AppendLine(" AND GruppoOperazioni.GRU_COD = " & Agro_SQL_SaveNum(Gru_Cod))
            End If

            '--------------------------

            Stb.AppendLine(" AND Agenda.Lav_Cod IN (" & LAVCOD_PRATICA_ECOLOGICA & "," & LAVCOD_FORMAZIONE & "," & LAVCOD_GESTIONE_RIFIUTI & ")")

            Stb.AppendLine(" AND Agenda.validita_inizio >= " & Agro_SQL_SaveDate(Validita_Inizio))
            Stb.AppendLine(" AND Agenda.validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine))

            Stb.AppendLine(" AND Agenda.validita_inizio >= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleInizio))
            Stb.AppendLine(" AND Agenda.validita_inizio <= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine))

            Stb.AppendLine(" ) ")

            Stb.AppendLine(" UNION ALL ")

            Stb.AppendLine(" ( ")

            '/////////////////////////////////////////////////////////////////////////////////////////////
            '//////////// VISITE                       ///////////////////////////////
            '/////////////////////////////////////////////////////////////////////////////////////////////

            Stb.AppendLine(" SELECT Agenda.Piva ")
            Stb.AppendLine(" , Agenda.Sa_Cod ")
            Stb.AppendLine(" , Agenda.Id_Agenda ")
            Stb.AppendLine(" , Agenda.Lav_Cod ")
            Stb.AppendLine(" , Agenda.Des_Lib ")
            Stb.AppendLine(" , Movimenti.Data_Movimento as Data_Movimento ")
            Stb.AppendLine(" , '' as Ora ")
            Stb.AppendLine(" , '' as Mov_Desc ")
            Stb.AppendLine(" , '' as Cau_Mov ")
            Stb.AppendLine(" , 0 AS Cul_Cod ")
            Stb.AppendLine(" , 0 AS Veg_Cod ")
            Stb.AppendLine(" , '' AS Veg_Des ")
            Stb.AppendLine(" , Agenda.Blocco_Flag ")
            Stb.AppendLine(" , (Dettagli.Rag_Soc + Dettagli.Cognome + ' ' + Dettagli.Nome) AS Tecnico ")

            Stb.AppendLine(" , i.rag_soc ")
            Stb.AppendLine(" , Operazioni.lav_des ")
            Stb.AppendLine(" , GruppoOperazioni.gru_Des ")
            Stb.AppendLine(" , GruppoOperazioni.tipo ")

            '(20/12/2016 fede) aggiunte varieta
            Stb.AppendLine(" , '' AS cul_des ")

            '(09/10/2017 MarcoG) aggiunto il campo contabilizzato per la pianificazione
            Stb.AppendLine(" , 0 AS contabilizzato ")

            '(12/10/2017 MarcoG) aggiunto le note a checkbox
            Stb.AppendLine(" , ISNULL(Note_Intervento.Nota_Des,'') as Nota_Des ")

            '(23/11/2017 MarcoG) aggiunte attività per altre operazioni
            Stb.AppendLine(" , ISNULL(Attivita.Sigla, '') AS AttivitaSigla ")
            Stb.AppendLine(" , ISNULL(Attivita.[Desc], '') AS AttivitaDesc ")

            '(22/05/2018 MarcoG)  aggiunta la dicitura per le Ricette
            Stb.AppendLine(" , 0 AS Ricetta_Cod ")

            '(23/08/2018 MarcoG)  aggiunta data di ultima modifica
            Stb.AppendLine(" , Agenda.Data_Modifica AS Data_Ultima_Modifica_Intervento ")

            Stb.AppendLine(" FROM Agenda ")
            Stb.AppendLine(" INNER JOIN Movimenti  ")
            Stb.AppendLine("    ON Agenda.PIVA = Movimenti.PIVA  ")
            Stb.AppendLine("    AND Agenda.Sa_Cod = Movimenti.Sa_Cod  ")
            Stb.AppendLine("    AND Agenda.Id_Agenda = Movimenti.Id_Agenda  ")

            Stb.AppendLine(" INNER JOIN " & NomeDB_Utenti & ".dbo.Utenti_Dettagli Dettagli ")
            Stb.AppendLine("    ON  Dettagli.CodFisc = Agenda.Username_Creazione  ")

            'Modifica per gruppi operazioni e tipo
            Stb.AppendLine(" LEFT JOIN Operazioni  ")
            Stb.AppendLine("    ON Agenda.Lav_Cod = Operazioni.Lav_Cod  ")
            Stb.AppendLine(" LEFT JOIN GruppoOperazioni  ")
            Stb.AppendLine("    ON Operazioni.GRU_OP = GruppoOperazioni.GRU_COD  ")

            Stb.AppendLine(" INNER JOIN imprese i ")
            Stb.AppendLine("   ON i.piva = Agenda.PIVA ")

            '(12/10/2017 MarcoG)  aggiunte note testuali e a checkbox
            Stb.AppendLine(" LEFT JOIN AgendaxNote ")
            Stb.AppendLine("   ON Agenda.Id_Agenda = AgendaxNote.Id_Agenda ")
            Stb.AppendLine(" LEFT JOIN Note_Intervento ")
            Stb.AppendLine("   ON AgendaxNote.Nota_Cod = Note_Intervento.Nota_Cod ")

            '(30/10/2018 MarcoG)  aggiunte attività per altre operazioni
            Stb.AppendLine(" LEFT JOIN AttivitaXOperazioni ")
            Stb.AppendLine("   ON AttivitaXOperazioni.id_attivita = Agenda.id_attivita And AttivitaXOperazioni.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri_Server.PivaSuperUser) & "'")
            Stb.AppendLine(" LEFT JOIN Attivita ")
            Stb.AppendLine("   ON Agenda.Id_Attivita = Attivita.Id_Attivita ")


            Stb.AppendLine(" WHERE Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "'")


            '--------------------------

            If Filtro_GruppoOperazioni <> "" Then
                Stb.AppendLine(" AND ( " & Filtro_GruppoOperazioni & " )")
            End If

            If FiltroOperazioni <> "" Then
                Stb.AppendLine(" AND " & Agro_SQL_SaveText(FiltroOperazioni))
            End If

            If Gru_Cod <> 0 Then
                Stb.AppendLine(" AND GruppoOperazioni.GRU_COD = " & Agro_SQL_SaveNum(Gru_Cod))
            End If

            '--------------------------

            '(30/10/2018 MarcoG)  tolgo dalle operazioni colturali le altre operazioni figlie delle visite
            Stb.AppendLine(" AND (AttivitaXOperazioni.Lav_Cod IS NULL OR AttivitaXOperazioni.Lav_Cod = " & LAVCOD_VISITA & ")  ")

            Stb.AppendLine(" AND (Agenda.Lav_Cod = " & LAVCOD_VISITA & " OR Agenda.Lav_Cod = " & LAVCOD_ALTRE_OPERAZIONI & ")")

            Stb.AppendLine(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio))
            Stb.AppendLine(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine))

            Stb.AppendLine(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleInizio))
            Stb.AppendLine(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine))

            Stb.AppendLine(" ) ")


            '---------------------------------------------------------------------------------------------------------------------------
            Stb.AppendLine(" ORDER BY Data_Movimento DESC, Agenda.Id_Agenda DESC ")


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Server, Stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    <CacheableAttribute(False)>
    Public Function Carica_Operazioni_Macchine(ByVal Piva As String,
                                               ByVal Sa_Cod As Integer,
                                               ByVal Validita_Inizio As Date,
                                               ByVal Validita_Fine As Date,
                                               ByVal Gru_Cod As Integer,
                                               ByVal Lav_Cod As Integer,
                                               ByVal FiltroOperazioni As String,
                                               ByVal FiltroImpianti As String,
                                               ByVal Filtro_GruppoOperazioni As String,
                                               ByVal xOrderBy As String,
                                               ByRef objParametri_Utenti As AgronicaCoreParametri,
                                               ByRef objParametri_Server As AgronicaCoreParametri
                                               ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Operazioni_R.Carica_Operazioni_Macchine()"

        Dim messaggioErrore As String = ""
        Dim Stb As New Text.StringBuilder
        Dim dt As DataTable

        Try
            Dim NomeDB_Utenti As String = objParametri_Utenti.Recupera_NomeDB()
            Dim FiltroCentri As String = ""

            If Sa_Cod = 0 Then
                'leggo se ci sono filtri sui centri
                FiltroCentri = EstraiFiltroCentri(Piva, objParametri_Server)
            End If

            Stb.Length = 0

            Stb.AppendLine(" SELECT Agenda.Piva ")
            Stb.AppendLine(" , Agenda.Sa_Cod ")
            Stb.AppendLine(" , Agenda.Id_Agenda ")
            Stb.AppendLine(" , Agenda.Lav_Cod ")
            Stb.AppendLine(" , Agenda.Des_Lib ")
            Stb.AppendLine(" , Movimenti.Data_Movimento ")
            Stb.AppendLine(" , Movimenti.Ora ")
            Stb.AppendLine(" , Movimenti.Mov_Desc ")
            Stb.AppendLine(" , Movimenti.Cau_Mov ")
            Stb.AppendLine(" , 0 AS Cul_Cod ")
            Stb.AppendLine(" , 0 AS Veg_Cod ")
            Stb.AppendLine(" , '' AS Veg_Des ")
            Stb.AppendLine(" , Agenda.Blocco_Flag ")
            Stb.AppendLine(" , (Dettagli.Rag_Soc + Dettagli.Cognome + ' ' + Dettagli.Nome) AS Tecnico ")
            Stb.AppendLine(" , ISNULL(( SELECT TOP 1 Centri_Aziendali.sa_nome FROM Centri_Aziendali WHERE Centri_Aziendali.sa_cod = Agenda.Sa_Cod and Centri_Aziendali.piva = Agenda.piva ) , '') AS sa_nome ")

            Stb.AppendLine(" , i.rag_soc ")
            Stb.AppendLine(" , Operazioni.lav_des ")
            Stb.AppendLine(" , GruppoOperazioni.gru_Des ")
            Stb.AppendLine(" , GruppoOperazioni.tipo ")

            '(20/12/2016 fede) aggiunte varieta
            Stb.AppendLine(" , '' AS cul_des ")

            '(09/10/2017 MarcoG) aggiunto il campo contabilizzato per la pianificazione
            Stb.AppendLine(" , 0 AS contabilizzato ")

            '(12/10/2017 MarcoG) aggiunto le note a checkbox
            Stb.AppendLine(" , ISNULL(Note_Intervento.Nota_Des,'') as Nota_Des ")

            '(22/05/2018 MarcoG)  aggiunta la dicitura per le Ricette
            Stb.AppendLine(" , 0 AS Ricetta_Cod ")

            '(23/08/2018 MarcoG)  aggiunta data di ultima modifica
            Stb.AppendLine(" , Agenda.Data_Modifica AS Data_Ultima_Modifica_Intervento ")

            Stb.AppendLine(" FROM Agenda ")
            Stb.AppendLine(" INNER JOIN Movimenti  ")
            Stb.AppendLine("    ON Agenda.PIVA = Movimenti.PIVA ")
            Stb.AppendLine("    AND Agenda.Sa_Cod = Movimenti.Sa_Cod ")
            Stb.AppendLine("    AND Agenda.Id_Agenda = Movimenti.Id_Agenda ")
            Stb.AppendLine(" INNER JOIN " & NomeDB_Utenti & ".dbo.Utenti_Dettagli Dettagli ")
            Stb.AppendLine("    ON  Dettagli.CodFisc = Agenda.Username_Creazione ")

            'Modifica per gruppi operazioni e tipo
            Stb.AppendLine(" LEFT JOIN Operazioni  ")
            Stb.AppendLine("    ON Agenda.Lav_Cod = Operazioni.Lav_Cod  ")
            Stb.AppendLine(" LEFT JOIN GruppoOperazioni  ")
            Stb.AppendLine("    ON Operazioni.GRU_OP = GruppoOperazioni.GRU_COD  ")

            Stb.AppendLine(" INNER JOIN imprese i ")
            Stb.AppendLine("    ON i.piva = Agenda.PIVA ")

            '(12/10/2017 MarcoG)  aggiunte note testuali e a checkbox
            Stb.AppendLine(" LEFT JOIN AgendaxNote ")
            Stb.AppendLine("    ON Agenda.Id_Agenda = AgendaxNote.Id_Agenda ")
            Stb.AppendLine(" LEFT JOIN Note_Intervento ")
            Stb.AppendLine("    ON AgendaxNote.Nota_Cod = Note_Intervento.Nota_Cod ")

            Stb.AppendLine(" WHERE Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "'")

            '--------------------------

            If Filtro_GruppoOperazioni <> "" Then
                Stb.AppendLine(" AND ( " & Filtro_GruppoOperazioni & " )")
            End If

            If FiltroOperazioni <> "" Then
                Stb.AppendLine(" AND " & Agro_SQL_SaveText(FiltroOperazioni))
            End If

            If Gru_Cod <> 0 Then
                Stb.AppendLine(" AND GruppoOperazioni.GRU_COD = " & Agro_SQL_SaveNum(Gru_Cod))
            End If

            '--------------------------
            Stb.AppendLine(" AND (Agenda.Lav_Cod = " & LAVCOD_MANUTENZIONE_MACCHINE & " OR Agenda.Lav_Cod = " & LAVCOD_REVISIONE_MACCHINE & ")")

            Stb.AppendLine(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio))
            Stb.AppendLine(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine))

            Stb.AppendLine(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleInizio))
            Stb.AppendLine(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine))

            '---------------------------------------------------------------------------------------------------------------------------
            Stb.AppendLine(" ORDER BY Data_Movimento DESC, Agenda.Id_Agenda DESC ")


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Server, Stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Leggi_Trappole(ByVal Piva As String,
                                   ByVal Sa_Cod As Integer,
                                    ByVal Validita_Inizio As Date,
                                    ByVal Validita_Fine As Date,
                                    ByVal Veg_Cod As Integer,
                                    ByVal Id_Cod As Integer,
                                    ByVal xOrderBy As String,
                                    ByVal objParametri_Utenti As AgronicaCoreParametri,
                                    ByVal objParametri_Server As AgronicaCoreParametri)

        Dim nomeRoutine = Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name & "." & Reflection.MethodBase.GetCurrentMethod().Name & "()"

        Dim messaggioErrore As String = ""
        Dim Stb As New Text.StringBuilder
        Dim dt As DataTable

        Try

            Stb.Length = 0

#Region "CTE che estrae le Installazioni Trappole/Catture di Massa"

            Stb.AppendLine("  ;with cte_InstallazioneTrappole AS ( ")
            Stb.AppendLine("    SELECT DISTINCT Agenda.Piva, Agenda.Lav_Cod, Agenda.Sa_Cod, Agenda.Id_Agenda, Agenda.Raccoglitore_Cod,Agenda.Blocco_Flag, ")
            Stb.AppendLine("    Movimenti_dettagli.Id_Mov, Movimenti_dettagli.Id_Mov_Det,")
            Stb.AppendLine("    Agenda.Des_Lib, Movimenti.Data_Movimento, Movimenti_Dettagli.Pro_Cod,Formulati.Fr_Des,")
            Stb.AppendLine("    ISNULL(Formulati.DurataFeromone,0) AS DurataFeromone_Installazione,")
            Stb.AppendLine("    (")
            Stb.AppendLine("        CASE")
            Stb.AppendLine("        WHEN ( ISNULL(Formulati.DurataFeromone,0) > 0)")
            Stb.AppendLine("            THEN  DATEADD(day,ISNULL(Formulati.DurataFeromone,0),Movimenti.Data_Movimento) ")
            Stb.AppendLine("            ELSE CONVERT(DateTime,'1900/01/01',120) ")
            Stb.AppendLine("        END ")
            Stb.AppendLine("     ) AS Data_Scadenza_Feromone_Installazione")
            Stb.AppendLine("    FROM Movimenti_Dettagli ")
            Stb.AppendLine("    INNER JOIN Movimenti ")
            Stb.AppendLine("    ON  Movimenti.Piva = Movimenti_Dettagli.Piva ")
            Stb.AppendLine("    AND Movimenti.Sa_Cod = Movimenti_Dettagli.Sa_Cod ")
            Stb.AppendLine("    AND Movimenti.Id_Agenda = Movimenti_Dettagli.Id_Agenda ")
            Stb.AppendLine("    AND Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov ")
            Stb.AppendLine("          AND Movimenti.Cau_Mov = '" & CAU_TRATTAMENTO & "'")
            Stb.AppendLine("    INNER JOIN Agenda ")
            Stb.AppendLine("    ON  Movimenti.Piva = Agenda.Piva")
            Stb.AppendLine("    AND Movimenti.Sa_Cod = Agenda.Sa_Cod")
            Stb.AppendLine("    AND Movimenti.Id_Agenda = Agenda.Id_Agenda")
            Stb.AppendLine("    INNER JOIN Formulati ")
            Stb.AppendLine("    ON Movimenti_Dettagli.Pro_Cod = Formulati.Fr_Cod ")
            Stb.AppendLine("    WHERE Agenda.Lav_Cod = " & LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA)
            '--------------------------

            If Piva <> "" Then
                Stb.AppendLine("      AND  Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If


            If Sa_Cod > 0 Then
                Stb.AppendLine("      AND  Agenda.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
            End If


            Stb.AppendLine("           AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio))
            Stb.AppendLine("           AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine))
            Stb.AppendLine("  ),")

#End Region

#Region "CTE che estrae i Reinneschi Trappole che sono collegate alle le Installazioni Trappole/Catture di Massa estratte in precedenza"

            Stb.AppendLine(" cte_ReinneschiTrappole AS ( ")
            Stb.AppendLine("    SELECT DISTINCT Mov_Dettagli_Riferimenti.Piva_Rif AS Piva_Installazione, Mov_Dettagli_Riferimenti.Sa_Cod_Rif AS Sa_Cod_Installazione, Mov_Dettagli_Riferimenti.Id_Agenda_Rif AS Id_Agenda_Installazione,")
            Stb.AppendLine("    Mov_Dettagli_Riferimenti.Id_Mov_Rif As Id_Mov_Installazione, Mov_Dettagli_Riferimenti.Id_Mov_Det_Rif AS Id_Mov_Det_Installazione,")
            Stb.AppendLine("    Movimenti.Data_Movimento AS Data_Reinnesco, Movimenti_Dettagli.Pro_Cod,Formulati.Fr_Des,")
            Stb.AppendLine("    ISNULL(Formulati.DurataFeromone,0) AS DurataFeromone_Reinnesco,")
            Stb.AppendLine("    (")
            Stb.AppendLine("        CASE")
            Stb.AppendLine("        WHEN ( ISNULL(Formulati.DurataFeromone,0) > 0)")
            Stb.AppendLine("            THEN  DATEADD(day,ISNULL(Formulati.DurataFeromone,0),Movimenti.Data_Movimento)")
            Stb.AppendLine("            ELSE CONVERT(DateTime,'1900/01/01',120) ")
            Stb.AppendLine("        END")
            Stb.AppendLine("    ) AS Data_Scadenza_Feromone_Reinnesco,")
            Stb.AppendLine("    CONCAT(Agenda.des_lib, '-', CONVERT(varchar, Movimenti.Data_Movimento, 3), ' ID: ' + CONVERT(varchar,Agenda.Id_Agenda)) AS Reinnesco_Des_Lib")
            Stb.AppendLine("    FROM Movimenti_Dettagli ")
            Stb.AppendLine("    INNER JOIN Movimenti ")
            Stb.AppendLine("    ON  Movimenti.Piva = Movimenti_Dettagli.Piva ")
            Stb.AppendLine("    AND Movimenti.Sa_Cod = Movimenti_Dettagli.Sa_Cod ")
            Stb.AppendLine("    AND Movimenti.Id_Agenda = Movimenti_Dettagli.Id_Agenda ")
            Stb.AppendLine("    AND Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov ")
            Stb.AppendLine("          AND Movimenti.Cau_Mov = '" & CAU_TRATTAMENTO & "'")
            Stb.AppendLine("    INNER JOIN Agenda ")
            Stb.AppendLine("    ON  Movimenti.Piva = Agenda.Piva")
            Stb.AppendLine("    AND Movimenti.Sa_Cod = Agenda.Sa_Cod")
            Stb.AppendLine("    AND Movimenti.Id_Agenda = Agenda.Id_Agenda")
            Stb.AppendLine("    INNER JOIN Mov_Dettagli_Riferimenti")
            Stb.AppendLine("    ON Movimenti_dettagli.Piva = Mov_Dettagli_Riferimenti.Piva")
            Stb.AppendLine("    AND Movimenti_dettagli.Sa_Cod = Mov_Dettagli_Riferimenti.Sa_Cod")
            Stb.AppendLine("    AND Movimenti_dettagli.Id_Agenda = Mov_Dettagli_Riferimenti.Id_Agenda")
            Stb.AppendLine("    AND Movimenti_dettagli.Id_Mov = Mov_Dettagli_Riferimenti.Id_Mov")
            Stb.AppendLine("    AND Movimenti_dettagli.Id_Mov_Det = Mov_Dettagli_Riferimenti.Id_Mov_Det")
            Stb.AppendLine("          AND Mov_Dettagli_Riferimenti.Lav_Cod_Rif = " & LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA)
            Stb.AppendLine("    INNER JOIN Movimenti AS Movimenti_Installazione")
            Stb.AppendLine("    ON  Mov_Dettagli_Riferimenti.Piva_Rif = Movimenti_Installazione.Piva")
            Stb.AppendLine("    AND Mov_Dettagli_Riferimenti.Sa_Cod_Rif = Movimenti_Installazione.Sa_Cod")
            Stb.AppendLine("    AND Mov_Dettagli_Riferimenti.Id_Agenda_Rif = Movimenti_Installazione.Id_Agenda")
            Stb.AppendLine("    AND Mov_Dettagli_Riferimenti.Id_Mov_Rif = Movimenti_Installazione.Id_Mov")
            Stb.AppendLine("    INNER JOIN Formulati")
            Stb.AppendLine("    ON Movimenti_Dettagli.Pro_Cod = Formulati.Fr_Cod ")
            Stb.AppendLine("    WHERE Agenda.Lav_Cod = " & LAVCOD_REINNESCO_TRAPPOLE)

            If Piva <> "" Then
                Stb.AppendLine("      AND  Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If Sa_Cod > 0 Then
                Stb.AppendLine("      AND  Agenda.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
            End If

            Stb.AppendLine("  ),")

#End Region

#Region "CTE che indica se il Feromone è Scaduto oppure è in Scadenza per ogni Installazione"

            Stb.AppendLine(" cte_InstallazioneTrappole_Info_Feromone AS (")
            Stb.AppendLine("    SELECT DISTINCT Piva, Lav_Cod, Sa_Cod, Id_Agenda, Raccoglitore_Cod,Blocco_Flag, ")
            Stb.AppendLine("    Id_Mov, Id_Mov_Det,Des_Lib, Data_Movimento,Pro_Cod, Fr_Des, DurataFeromone_Installazione, ")
            Stb.AppendLine("    -- Indico i Feromoni che sono scaduti  ")
            Stb.AppendLine("    ( ")
            Stb.AppendLine("        CASE ")
            Stb.AppendLine("            WHEN ( (DurataFeromone_Installazione > 0) AND ( CAST(Data_Scadenza_Feromone_Installazione AS DATE) < CAST(GETDATE() AS DATE) ))")
            Stb.AppendLine("            THEN 1 ")
            Stb.AppendLine("            ELSE 0")
            Stb.AppendLine("        END ")
            Stb.AppendLine("    ) AS Feromone_Scaduto_Installazione,")
            Stb.AppendLine(" -- Indico i Feromoni che stanno per scadere (con Data_Scadenza_Feromone inferiore di 7 Giorni) ")
            Stb.AppendLine("    ( ")
            Stb.AppendLine("        CASE")
            Stb.AppendLine("            WHEN ( (DurataFeromone_Installazione > 0) AND ")
            Stb.AppendLine("             ( ( DATEDIFF(day,CAST(GETDATE() AS DATE),CAST(Data_Scadenza_Feromone_Installazione AS DATE)) ) >= 0 AND ( DATEDIFF(day,CAST(GETDATE() AS DATE),CAST(Data_Scadenza_Feromone_Installazione AS DATE)) ) < 7 ))")
            Stb.AppendLine("            THEN 1 ")
            Stb.AppendLine("            ELSE 0")
            Stb.AppendLine("        END ")
            Stb.AppendLine("    ) AS Feromone_InScadenza_Installazione,")
            Stb.AppendLine(" Data_Scadenza_Feromone_Installazione")

            Stb.AppendLine(" FROM cte_InstallazioneTrappole ")

            Stb.AppendLine(" GROUP BY Piva, Lav_Cod, Sa_Cod, Id_Agenda, Raccoglitore_Cod,Blocco_Flag, ")
            Stb.AppendLine(" Id_Mov, Id_Mov_Det,Des_Lib, Data_Movimento,Pro_Cod,Fr_Des,DurataFeromone_Installazione,Data_Scadenza_Feromone_Installazione")
            Stb.AppendLine("),")

#End Region

#Region "CTE che indica la Data di Scadenza più alta del Feromone in un Reinnesco"

            Stb.AppendLine(" cte_ReinneschiTrappole_Ultima_Data_Scadenza AS (")
            Stb.AppendLine("    SELECT DISTINCT Piva_Installazione, Sa_Cod_Installazione, Id_Agenda_Installazione,")
            Stb.AppendLine("    Id_Mov_Installazione, Id_Mov_Det_Installazione, ")
            Stb.AppendLine("    Pro_Cod, DurataFeromone_Reinnesco, MAX(Data_Scadenza_Feromone_Reinnesco) AS Data_Scadenza_Feromone_Reinnesco,")
            Stb.AppendLine("    STRING_AGG(Reinnesco_Des_Lib,',') AS Reinneschi_Des_Lib")

            Stb.AppendLine("    FROM cte_ReinneschiTrappole ")
            Stb.AppendLine("    GROUP BY Piva_Installazione, Sa_Cod_Installazione, Id_Agenda_Installazione, ")
            Stb.AppendLine("    Id_Mov_Installazione, Id_Mov_Det_Installazione, Pro_Cod, DurataFeromone_Reinnesco")
            Stb.AppendLine("), ")

#End Region

#Region "CTE che indica se il Feromone è Scaduto oppure è in Scadenza nei Reinneschi"

            Stb.AppendLine(" cte_ReinneschiTrappole_Info_Feromone AS (")
            Stb.AppendLine("    SELECT cte_ReinneschiTrappole_Ultima_Data_Scadenza.*, ")
            Stb.AppendLine("    -- Indico i Feromoni che sono scaduti  ")
            Stb.AppendLine("    ( ")
            Stb.AppendLine("        CASE ")
            Stb.AppendLine("            WHEN ( (DurataFeromone_Reinnesco > 0) AND ( CAST(Data_Scadenza_Feromone_Reinnesco AS DATE) < CAST(GETDATE() AS DATE) ))")
            Stb.AppendLine("            THEN 1 ")
            Stb.AppendLine("            ELSE 0")
            Stb.AppendLine("        END ")
            Stb.AppendLine("    ) AS Feromone_Scaduto_Reinnesco,")
            Stb.AppendLine(" -- Indico i Feromoni che stanno per scadere (con Data_Scadenza_Feromone inferiore di 7 Giorni) ")
            Stb.AppendLine("    ( ")
            Stb.AppendLine("        CASE")
            Stb.AppendLine("            WHEN ( (DurataFeromone_Reinnesco > 0) AND ")
            Stb.AppendLine("            ( ( DATEDIFF(day,CAST(GETDATE() AS DATE),CAST(Data_Scadenza_Feromone_Reinnesco AS DATE)) ) >= 0 AND ( DATEDIFF(day,CAST(GETDATE() AS DATE),CAST(Data_Scadenza_Feromone_Reinnesco AS DATE)) ) < 7 ))")
            Stb.AppendLine("            THEN 1 ")
            Stb.AppendLine("            ELSE 0")
            Stb.AppendLine("        END ")
            Stb.AppendLine("    ) AS Feromone_InScadenza_Reinnesco")

            Stb.AppendLine(" FROM cte_ReinneschiTrappole_Ultima_Data_Scadenza ")
            Stb.AppendLine("), ")

#End Region

#Region "CTE che restituisce la Specie degli Impianti in cui è stata fatta l'Installazione Trappole"

            Stb.AppendLine(" cte_SpecieVarieta_Impianti AS (")
            Stb.AppendLine("    SELECT DISTINCT cte_InstallazioneTrappole_Info_Feromone.PIVA, cte_InstallazioneTrappole_Info_Feromone.Sa_Cod,")
            Stb.AppendLine("    cte_InstallazioneTrappole_Info_Feromone.Id_Agenda, cte_InstallazioneTrappole_Info_Feromone.Id_Mov, cte_InstallazioneTrappole_Info_Feromone.Id_Mov_Det,")
            Stb.AppendLine("    ISNULL(SpecieVegetali.Veg_Cod, -1) AS Veg_Cod,")
            Stb.AppendLine("    ISNULL(SpecieVegetali.Veg_Des, '') AS Veg_Des,")
            Stb.AppendLine("    ISNULL(Cultivar.Cul_Cod, 0) AS Cul_Cod,")
            Stb.AppendLine("    ISNULL(Cultivar.Cul_Des, '') AS Cul_Des,")
            Stb.AppendLine("    ISNULL(Codici_Anagrafe.codice, 0) AS Id_Cod,")
            Stb.AppendLine("    ISNULL(Codici_Anagrafe.descrizione, '') AS Id_Des,")

            Stb.AppendLine("    (")
            Stb.AppendLine("        CASE")
            Stb.AppendLine("            WHEN (SpecieVegetali.Veg_Cod IS NOT NULL AND Cultivar.Cul_Cod IS NOT NULL)")
            Stb.AppendLine("            THEN SpecieVegetali.Veg_Des + ' - ' + Cultivar.Cul_Des")
            Stb.AppendLine("            ELSE Codici_Anagrafe.descrizione")
            Stb.AppendLine("        END")
            Stb.AppendLine("    ) AS Specie_Destinazione")

            Stb.AppendLine("    FROM cte_InstallazioneTrappole_Info_Feromone")

            Stb.AppendLine("    LEFT JOIN Mov_Destinazioni")
            Stb.AppendLine("    ON Mov_Destinazioni.Piva = cte_InstallazioneTrappole_Info_Feromone.PIVA AND ")
            Stb.AppendLine("    Mov_Destinazioni.Sa_Cod = cte_InstallazioneTrappole_Info_Feromone.Sa_Cod AND ")
            Stb.AppendLine("    Mov_Destinazioni.Id_Agenda = cte_InstallazioneTrappole_Info_Feromone.Id_Agenda AND ")
            Stb.AppendLine("    Mov_Destinazioni.Id_Mov = cte_InstallazioneTrappole_Info_Feromone.Id_Mov AND ")
            Stb.AppendLine("    Mov_Destinazioni.Id_Mov_Det = cte_InstallazioneTrappole_Info_Feromone.Id_Mov_Det ")
            Stb.AppendLine("    LEFT JOIN Reg_Impianti")
            Stb.AppendLine("    ON Reg_Impianti.Piva = Mov_Destinazioni.PIVA AND ")
            Stb.AppendLine("    Reg_Impianti.Sa_Cod = Mov_Destinazioni.Sa_Cod AND ")
            Stb.AppendLine("    Reg_Impianti.APPEZZA = Mov_Destinazioni.APPEZZA AND ")
            Stb.AppendLine("    Reg_Impianti.Id_Reg = Mov_Destinazioni.Id_Destinazione ")
            Stb.AppendLine("    LEFT JOIN Cultivar")
            Stb.AppendLine("    ON Cultivar.Cul_Cod = Reg_Impianti.Cul_Cod AND")
            Stb.AppendLine("    Reg_Impianti.Cul_Cod > 0")
            Stb.AppendLine("    LEFT JOIN SpecieVegetali")
            Stb.AppendLine("    ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ")
            Stb.AppendLine("    LEFT JOIN Reg_Impianti_Codici ")
            Stb.AppendLine("    ON Reg_Impianti.piva = Reg_Impianti_Codici.piva ")
            Stb.AppendLine("    AND Reg_Impianti.SA_COD = Reg_Impianti_Codici.sa_cod ")
            Stb.AppendLine("    AND Reg_Impianti.APPEZZA = Reg_Impianti_Codici.appezza ")
            Stb.AppendLine("    AND Reg_Impianti.ID_REG = Reg_Impianti_Codici.Id_Reg ")
            Stb.AppendLine("    AND Reg_Impianti.Cul_Cod = 0 ")
            Stb.AppendLine("    AND Reg_Impianti_Codici.id_cod BETWEEN 3000 AND 3999 ")
            Stb.AppendLine("    LEFT JOIN Codici_Anagrafe ")
            Stb.AppendLine("    ON Codici_Anagrafe.codice = Reg_Impianti_Codici.id_cod ")

            Stb.AppendLine("    WHERE 1 = 1")

            If Veg_Cod <> -1 Then
                Stb.AppendLine("      AND SpecieVegetali.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod))
            End If

            If Id_Cod <> 0 Then
                Stb.AppendLine("     AND Codici_Anagrafe.codice = " & Agro_SQL_SaveNum(Id_Cod))
            End If

            Stb.AppendLine("), ")

#End Region

#Region "CTE con GROUP BY della Specie degli Impianti in cui è stata fatta l'Installazione Trappole"

            Stb.AppendLine(" cte_SpecieVarieta_Impianti_GROUP AS (")
            Stb.AppendLine("    SELECT PIVA, Sa_Cod, Id_Agenda,")
            Stb.AppendLine("    Veg_Des, Id_Des,")
            Stb.AppendLine("    STRING_AGG(Cul_Des,',') AS Cul_Des,")
            Stb.AppendLine("    STRING_AGG(Specie_Destinazione,',') AS Specie_Destinazione")

            Stb.AppendLine("    FROM cte_SpecieVarieta_Impianti")

            Stb.AppendLine("    GROUP BY PIVA,Sa_Cod,Id_Agenda,Veg_Des,Id_Des")

            Stb.AppendLine(") ")

#End Region

            Stb.AppendLine(" SELECT cte_InstallazioneTrappole_Info_Feromone.*,")
            Stb.AppendLine(" Imprese.Rag_Soc, Operazioni.Lav_Des, Centri_Aziendali.Sa_Nome, ")
            Stb.AppendLine(" (")
            Stb.AppendLine("    CASE")
            Stb.AppendLine("        WHEN (Feromone_Scaduto_Reinnesco IS NOT NULL)")
            Stb.AppendLine("   THEN Feromone_Scaduto_Reinnesco")
            Stb.AppendLine("   ELSE Feromone_Scaduto_Installazione")
            Stb.AppendLine("   END")
            Stb.AppendLine(" ) AS Feromone_Scaduto,")

            Stb.AppendLine(" (")
            Stb.AppendLine("    CASE")
            Stb.AppendLine("        WHEN (Feromone_InScadenza_Reinnesco IS NOT NULL)")
            Stb.AppendLine("        THEN Feromone_InScadenza_Reinnesco")
            Stb.AppendLine("        ELSE Feromone_InScadenza_Installazione")
            Stb.AppendLine("   END")
            Stb.AppendLine(" ) AS Feromone_InScadenza,")

            Stb.AppendLine(" (")
            Stb.AppendLine("    CASE")
            Stb.AppendLine("        WHEN ((Data_Scadenza_Feromone_Reinnesco IS NOT NULL) AND (Data_Scadenza_Feromone_Reinnesco > Data_Scadenza_Feromone_Installazione))")
            Stb.AppendLine("        THEN Data_Scadenza_Feromone_Reinnesco")
            Stb.AppendLine("        ELSE Data_Scadenza_Feromone_Installazione")
            Stb.AppendLine("   END")
            Stb.AppendLine(" ) AS Data_Scadenza_Feromone,")

            Stb.AppendLine(" (")
            Stb.AppendLine("    CASE")
            Stb.AppendLine("        WHEN (Data_Scadenza_Feromone_Reinnesco IS NOT NULL)")
            Stb.AppendLine("        THEN 1")
            Stb.AppendLine("        ELSE 0")
            Stb.AppendLine("   END")
            Stb.AppendLine(" ) AS Trappola_Reinnescata,")

            Stb.AppendLine(" cte_ReinneschiTrappole_Info_Feromone.Feromone_Scaduto_Reinnesco,")
            Stb.AppendLine(" cte_ReinneschiTrappole_Info_Feromone.Feromone_InScadenza_Reinnesco,")
            Stb.AppendLine(" cte_ReinneschiTrappole_Info_Feromone.Data_Scadenza_Feromone_Reinnesco,")
            Stb.AppendLine(" ISNULL(cte_ReinneschiTrappole_Info_Feromone.Reinneschi_Des_Lib, '') AS Reinneschi_Des_Lib,")
            Stb.AppendLine(" cte_SpecieVarieta_Impianti_GROUP.Veg_Des,")
            Stb.AppendLine(" cte_SpecieVarieta_Impianti_GROUP.Cul_Des,")
            Stb.AppendLine(" cte_SpecieVarieta_Impianti_GROUP.Id_Des,")
            Stb.AppendLine(" cte_SpecieVarieta_Impianti_GROUP.Specie_Destinazione")

            Stb.AppendLine(" FROM cte_InstallazioneTrappole_Info_Feromone")
            Stb.AppendLine(" INNER JOIN Imprese ")
            Stb.AppendLine(" ON Imprese.Piva = cte_InstallazioneTrappole_Info_Feromone.PIVA")
            Stb.AppendLine(" INNER JOIN Operazioni ")
            Stb.AppendLine(" ON Operazioni.Lav_Cod = cte_InstallazioneTrappole_Info_Feromone.Lav_Cod")
            Stb.AppendLine(" INNER JOIN Centri_Aziendali ")
            Stb.AppendLine(" ON Centri_Aziendali.Piva = cte_InstallazioneTrappole_Info_Feromone.PIVA")
            Stb.AppendLine(" AND Centri_Aziendali.Sa_Cod = cte_InstallazioneTrappole_Info_Feromone.Sa_Cod")
            Stb.AppendLine(" LEFT JOIN cte_ReinneschiTrappole_Info_Feromone")
            Stb.AppendLine(" ON cte_ReinneschiTrappole_Info_Feromone.Piva_Installazione = cte_InstallazioneTrappole_Info_Feromone.PIVA AND ")
            Stb.AppendLine(" cte_ReinneschiTrappole_Info_Feromone.Sa_Cod_Installazione = cte_InstallazioneTrappole_Info_Feromone.Sa_Cod AND ")
            Stb.AppendLine(" cte_ReinneschiTrappole_Info_Feromone.Id_Agenda_Installazione = cte_InstallazioneTrappole_Info_Feromone.Id_Agenda AND ")
            Stb.AppendLine(" cte_ReinneschiTrappole_Info_Feromone.Id_Mov_Installazione = cte_InstallazioneTrappole_Info_Feromone.Id_Mov AND ")
            Stb.AppendLine(" cte_ReinneschiTrappole_Info_Feromone.Id_Mov_Det_Installazione = cte_InstallazioneTrappole_Info_Feromone.Id_Mov_Det ")
            Stb.AppendLine(" LEFT JOIN cte_SpecieVarieta_Impianti_GROUP")
            Stb.AppendLine(" ON cte_SpecieVarieta_Impianti_GROUP.PIVA = cte_InstallazioneTrappole_Info_Feromone.PIVA AND ")
            Stb.AppendLine(" cte_SpecieVarieta_Impianti_GROUP.Sa_Cod = cte_InstallazioneTrappole_Info_Feromone.Sa_Cod AND ")
            Stb.AppendLine(" cte_SpecieVarieta_Impianti_GROUP.Id_Agenda = cte_InstallazioneTrappole_Info_Feromone.Id_Agenda ")

            Stb.AppendLine(" WHERE 1 = 1 ")

            If xOrderBy <> "" Then
                Stb.AppendLine(" ORDER BY " & xOrderBy)
            Else
                Stb.AppendLine(" ORDER BY Feromone_Scaduto DESC, Feromone_InScadenza DESC, Data_Scadenza_Feromone ASC, Data_Movimento DESC ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Server, Stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Leggi_Numero_Trappole_Registrate(ByVal IDTestataTemp As Integer,
                                                     ByVal Av_Cod As Integer,
                                                     ByVal xFiltroAggiuntivo As String,
                                                     ByVal xOrderBy As String,
                                                     ByVal objParametri_Server As AgronicaCoreParametri)

        Dim nomeRoutine = Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name & "." & Reflection.MethodBase.GetCurrentMethod().Name & "()"

        Dim messaggioErrore As String = ""
        Dim Stb As New Text.StringBuilder
        Dim dt As DataTable

        Try

            Stb.Length = 0

            Stb.AppendLine("SELECT f.Piva,f.Sa_Cod,f.Appezza, f.id_reg,")
            Stb.AppendLine("Avversita.Av_Cod, Avversita.Av_Des_Vol, Avversita.Av_Des_Lat, Avversita.Abbreviazione,")
            Stb.AppendLine("Formulati.Fr_Cod, Formulati.Fr_Des,")
            Stb.AppendLine("CASE Movimenti.Mezzo")
            Stb.AppendLine("         WHEN " & enum_Ripartizione_Trappole.Manuale & " THEN Mov_Destinazioni.Qta")
            Stb.AppendLine("         WHEN " & enum_Ripartizione_Trappole.Automatica & " THEN Movimenti_dettagli.Qta_Extra_Totale")
            Stb.AppendLine("END AS Dose,")
            Stb.AppendLine("Movimenti_dettagli.Extra_Int AS Udm_Cod")
            Stb.AppendLine("FROM Agenda")
            Stb.AppendLine("INNER JOIN Movimenti")
            Stb.AppendLine("on Movimenti.PIVA = Agenda.PIVA and")
            Stb.AppendLine("Movimenti.Sa_Cod = Agenda.Sa_Cod and")
            Stb.AppendLine("Movimenti.Id_Agenda = Agenda.Id_Agenda and")
            Stb.AppendLine("Movimenti.Cau_Mov = '" & CAU_TRATTAMENTO & "' ")

            Stb.AppendLine("INNER JOIN Movimenti_dettagli")
            Stb.AppendLine("on Movimenti_dettagli.PIVA = Movimenti.PIVA and")
            Stb.AppendLine("Movimenti_dettagli.Sa_Cod = Movimenti.Sa_Cod and ")
            Stb.AppendLine("Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda and")
            Stb.AppendLine("Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov and")
            Stb.AppendLine("Movimenti_dettagli.Elem_Cod = " & FORMULATI)

            Stb.AppendLine("INNER JOIN Formulati")
            Stb.AppendLine("on Formulati.Fr_Cod = Movimenti_dettagli.Pro_Cod")

            Stb.AppendLine("INNER JOIN Mov_Dettaglio_Tecnico")
            Stb.AppendLine("on Mov_Dettaglio_Tecnico.PIVA = Movimenti_dettagli.PIVA and")
            Stb.AppendLine("Mov_Dettaglio_Tecnico.Sa_Cod = Movimenti_dettagli.Sa_Cod and ")
            Stb.AppendLine("Mov_Dettaglio_Tecnico.Id_Agenda = Movimenti_dettagli.Id_Agenda and")
            Stb.AppendLine("Mov_Dettaglio_Tecnico.Id_Mov = Movimenti_dettagli.Id_Mov and")
            Stb.AppendLine("Mov_Dettaglio_Tecnico.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det ")

            Stb.AppendLine("INNER JOIN Avversita")
            Stb.AppendLine("on Avversita.Av_Cod = Mov_Dettaglio_Tecnico.Av_Cod")

            Stb.AppendLine("INNER JOIN Mov_Destinazioni")
            Stb.AppendLine("on Mov_Destinazioni.PIVA = Mov_Dettaglio_Tecnico.PIVA and")
            Stb.AppendLine("Mov_Destinazioni.Sa_Cod = Mov_Dettaglio_Tecnico.Sa_Cod and")
            Stb.AppendLine("Mov_Destinazioni.Id_Agenda = Mov_Dettaglio_Tecnico.Id_Agenda and")
            Stb.AppendLine("Mov_Destinazioni.Id_Mov = Mov_Dettaglio_Tecnico.Id_Mov and")
            Stb.AppendLine("Mov_Destinazioni.Id_Mov_Det = Mov_Dettaglio_Tecnico.Id_Mov_Det and")
            Stb.AppendLine("Mov_Destinazioni.Tipo_Destinazione = 0")

            If IDTestataTemp <> 0 Then
                Stb.AppendLine("INNER JOIN __tmp_FiltroImpianti f")
                Stb.AppendLine("On f.piva = Mov_Destinazioni.PIVA  ")
                Stb.AppendLine("And f.sa_cod = Mov_Destinazioni.SA_COD ")
                Stb.AppendLine("And f.appezza = Mov_Destinazioni.APPEZZA ")
                Stb.AppendLine("And f.id_reg = Mov_Destinazioni.Id_Destinazione ")
                Stb.AppendLine("And f.idTestataTemp = " & IDTestataTemp)
            End If

            Stb.AppendLine("WHERE Agenda.Lav_Cod = " & LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA)

            If Av_Cod <> 0 Then
                Stb.AppendLine("and Mov_Dettaglio_Tecnico.Av_Cod = " & Agro_SQL_SaveNum(Av_Cod))
            End If

            If xFiltroAggiuntivo <> "" Then
                Stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Server))
            End If

            If xOrderBy <> "" Then
                Stb.AppendLine("ORDER BY " & xOrderBy)
            Else
                Stb.AppendLine("ORDER BY Avversita.Av_Des_Vol, Avversita.Av_Des_Lat ASC")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Server, Stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        Finally

            If IDTestataTemp <> 0 Then
                Dim objVarie As New AgronicaCoreVarieDAL.__tmp_FiltroImpianti_W

                objVarie.CancellaRecordDaIDTestataTemp(IDTestataTemp, objParametri_Server)
            End If

        End Try

        Return dt

    End Function

    Private Function EstraiFiltroCentri(piva As String, objParametri_Server As AgronicaCoreParametri) As String

        Dim filtroCentri As String = ""

        Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
        Dim dtCentriVisibili As DataTable = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(piva) & "'", "", objParametri_Server)
        If Not IsNothing(dtCentriVisibili) Then
            For Each dr As DataRow In dtCentriVisibili.Rows
                filtroCentri &= dr.Item("sa_cod") & ","
            Next
        End If

        Return filtroCentri
    End Function

End Class

