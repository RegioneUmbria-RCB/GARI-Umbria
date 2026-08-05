Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class AccettazioneDaDiversi_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '###############################################################################
    Public Function RecuperaDatiBollaxMonitoraggioCE_OLD(ByVal Piva As String,
                                                         ByVal Doc_Numero_Sin_Bolla As String,
                                                         ByVal Doc_Numero_Bolla As Decimal,
                                                         ByVal Doc_Numero_Des_Bolla As String,
                                                         ByVal Doc_Numero_Sin_DDT As String,
                                                         ByVal Doc_Numero_DDT As Decimal,
                                                         ByVal Doc_Numero_Des_DDT As String,
                                                         ByVal Anno_Bolla As Integer,
                                                         ByVal Data_DDT As String,
                                                         ByVal Cod_Contatto_Produttore As String,
                                                         ByVal Sa_Cod As Integer,
                                                         ByVal Fabbricato_Cod As Integer,
                                                         ByVal xFiltroAggiuntivo As String,
                                                         ByVal xOrderBy As String,
                                                         ByRef objParametri As AgronicaCoreParametri
                                                         ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.AccettazioneDaDiversi_R.RecuperaDatiBollaxMonitoraggioCE_OLD()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append("SELECT   -- Mov_Accett.Tara_Veicolo, Mov_Accett.Tara_Imballi, Mov_Accett.Tipo_Peso,  " & vbCrLf)
            StrSQL.Append("         Mov_Accett.Data_Movimento AS Data_Accett_Bolla, Mov_Accett.Ora AS Ora_Accett, " & vbCrLf)
            StrSQL.Append("         Mov_Accett.Doc_Numero_Sin as Doc_Numero_Sin_Bolla, Mov_Accett.Doc_Numero as Doc_Numero_Bolla,Mov_Accett.Doc_Numero_Des as Doc_Numero_Des_Bolla, Mov_Accett.Mov_Desc AS Note, " & vbCrLf)
            StrSQL.Append("         ISNULL(Contatti_Conferenti.Rag_Soc, '') AS Rag_Soc_Conferente, " & vbCrLf)
            StrSQL.Append("         ISNULL(Contatti_Produttori.Cod_Contatto,'') AS Cod_Contatto_Produttore,  " & vbCrLf)
            StrSQL.Append("         ISNULL(Contatti_Produttori.Rag_Soc, '') AS Rag_Soc_Produttore, ISNULL(Contatti_Produttori.Codice_Fiscale,'') AS Codice_Fiscale_Produttore,  " & vbCrLf)
            StrSQL.Append("         ISNULL(Contatti_Produttori.Nome,'') AS Nome_Produttore, ISNULL(Contatti_Produttori.Cognome,'') AS Cognome_Produttore,  " & vbCrLf)
            StrSQL.Append("         -- Mov_Accett.Cod_IndirizzoDestinazione, Indirizzi_Produttori.ind_des AS ind_des_Produttore, Indirizzi_Produttori.frz_des AS frz_des_Produttore, Indirizzi_Produttori.CAP AS cap_Produttore,  " & vbCrLf)
            StrSQL.Append("         -- Indirizzi_Produttori.stato AS stato_Produttore, Indirizzi_Produttori.pro_cod_istat AS pro_cod_istat_Produttore,  " & vbCrLf)
            StrSQL.Append("         -- Indirizzi_Produttori.com_cod_istat AS com_cod_istat_Produttore, Istat_Produttori.LOCALITA AS localita_Produttore, Istat_Produttori.COMUNI_PROV AS comuni_prov_Produttore, " & vbCrLf)
            StrSQL.Append("         Mov_Conf.Data_Movimento AS Data_Conf, Mov_Conf.Doc_Numero_Sin AS Doc_Numero_Sin_Conf, Mov_Conf.Doc_Numero AS Doc_Numero_Conf,  " & vbCrLf)
            StrSQL.Append("         Mov_Conf.Doc_Numero_Des AS Doc_Numero_Des_Conf,   " & vbCrLf)
            StrSQL.Append("         Mov_Raccolta.Data_Movimento AS Data_Raccolta,  " & vbCrLf)
            StrSQL.Append("         Mov_Raccolta.Ora AS Ora_Raccolta," & vbCrLf)
            StrSQL.Append("         Mov_Dett_Raccolta.Elem_Cod AS Elem_Cod_Raccolta,  " & vbCrLf)
            StrSQL.Append("         Mov_Dett_Raccolta.Pro_Cod AS Pro_Cod_Raccolta, Mov_Dett_Raccolta.Mat_Cod AS Mat_Cod_Raccolta,  " & vbCrLf)
            StrSQL.Append("         Mov_Dett_Raccolta.Cod_Progetto AS Cod_Progetto_Raccolta, Mov_Dett_Raccolta.Fase_Cod AS Fase_Cod_Raccolta,  " & vbCrLf)
            StrSQL.Append("         Mov_Dett_Raccolta.Lotto AS Lotto_Raccolta, Mov_Dett_Raccolta.Cal_Cod AS Cal_Cod_Raccolta, Mov_Dett_Raccolta.Udm_Cod AS Udm_Cod_Raccolta,  " & vbCrLf)
            StrSQL.Append("         Mov_Dett_Raccolta.Qta AS Qta_Raccolta, Mov_Dett_Raccolta.Variazione AS Variazione_Raccolta,  " & vbCrLf)
            StrSQL.Append("         MP_Raccolta.Cod_Articolo AS Cod_Articolo_Raccolta,  " & vbCrLf)
            StrSQL.Append("         MP_Raccolta.Mat_Des AS Mat_Des_Raccolta, MP_Raccolta.Cul_Cod, MP_Raccolta.Veg_Cod, MP_Raccolta.GRVA_COD_VEG,  " & vbCrLf)
            StrSQL.Append("         MP_Raccolta.Regolamento " & vbCrLf)
            StrSQL.Append("         , ISNULL( (SELECT TOP 1 ISNULL( materie_prime_calibri.cal_des , ' ') AS Calibro " & vbCrLf)
            StrSQL.Append("                     FROM materie_prime_calibri  " & vbCrLf)
            StrSQL.Append("                     INNER JOIN Materie_Prime_Campionature MP_Camp_Calibro ON MP_Camp_Calibro.tipo_cod =  materie_prime_calibri.cal_cod " & vbCrLf)
            StrSQL.Append("                     WHERE MP_Camp_Calibro.progressivo = Mov_Dett_Raccolta.cal_cod  " & vbCrLf)
            StrSQL.Append("                     AND MP_Camp_Calibro.tipo = 'calibro'), 0 ) AS Calibro " & vbCrLf)
            StrSQL.Append("         , ISNULL( (SELECT TOP 1 VAL_COD  " & vbCrLf)
            StrSQL.Append("                     FROM Materie_Prime_Campionature MP_Camp_Indice  " & vbCrLf)
            StrSQL.Append("                     WHERE MP_Camp_Indice.progressivo = Mov_Dett_Raccolta.cal_cod  " & vbCrLf)
            StrSQL.Append("                     AND MP_Camp_Indice.tipo = 'indice' " & vbCrLf)
            'StrSQL.Append("                    AND MP_Camp_Indice.tipo_cod = " + CStr(FRUTTAGEL_INDMATCOD_PUNTEGGIO) + " ), 0 ) AS Punteggio " + vbCrLf)
            StrSQL.Append("                     AND MP_Camp_Indice.tipo_cod IN ( " & CStr(FRUTTAGEL_INDMATCOD_PUNTEGGIO) & ", " & CStr(FRUTTAGEL_INDMATCOD_GRADOTEND) & ", " & CStr(FRUTTAGEL_INDMATCOD_GRADOBRIX) & " )  ), 0 ) AS Punteggio " & vbCrLf)
            StrSQL.Append(" ,  Fabbr_Raccolta.Fabbricato_Des AS Stabilimento " & vbCrLf)

            StrSQL.Append(" FROM Agenda " & vbCrLf)

            StrSQL.Append(" INNER JOIN Movimenti Mov_Accett ON Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Sa_Cod = Mov_Accett.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda " & vbCrLf)

            StrSQL.Append(" INNER JOIN Risorse_Umane Risorse_Umane_Conferenti ON Mov_Accett.Cod_RisUm = Risorse_Umane_Conferenti.Cod_RisUm  " & vbCrLf)
            StrSQL.Append(" INNER JOIN Contatti Contatti_Conferenti ON Risorse_Umane_Conferenti.Piva = Contatti_Conferenti.Piva AND Risorse_Umane_Conferenti.Cod_Contatto = Contatti_Conferenti.Cod_Contatto  " & vbCrLf)

            StrSQL.Append(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Produttori ON Mov_Accett.Cod_Destinazione = Risorse_Umane_Produttori.Cod_RisUm  " & vbCrLf)
            StrSQL.Append(" LEFT OUTER JOIN Contatti Contatti_Produttori ON Risorse_Umane_Produttori.Piva = Contatti_Produttori.Piva AND Risorse_Umane_Produttori.Cod_Contatto = Contatti_Produttori.Cod_Contatto  " & vbCrLf)
            'StrSQL.Append(" LEFT OUTER JOIN Indirizzi Indirizzi_Produttori ON  Mov_Accett.Cod_IndirizzoDestinazione = Indirizzi_Produttori.cod_indirizzo " + vbCrLf)
            'StrSQL.Append(" LEFT OUTER JOIN ISTAT Istat_Produttori ON Indirizzi_Produttori.pro_cod_istat = Istat_Produttori.PROV AND Indirizzi_Produttori.com_cod_istat = Istat_Produttori.COM " + vbCrLf)

            StrSQL.Append(" INNER JOIN Movimenti Mov_Conf ON Agenda.PIVA = Mov_Conf.PIVA AND Agenda.Sa_Cod = Mov_Conf.Sa_Cod AND Agenda.Id_Agenda = Mov_Conf.Id_Agenda " & vbCrLf)

            StrSQL.Append(" INNER JOIN Movimenti Mov_Raccolta ON Agenda.PIVA = Mov_Raccolta.PIVA AND Agenda.Sa_Cod = Mov_Raccolta.Sa_Cod AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda " & vbCrLf)
            StrSQL.Append(" INNER JOIN Movimenti_Dettagli Mov_Dett_Raccolta ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov " & vbCrLf)
            StrSQL.Append(" INNER JOIN Materie_Prime MP_Raccolta ON Mov_Dett_Raccolta.Elem_Cod = MP_Raccolta.Elem_Cod AND Mov_Dett_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod " & vbCrLf)

            StrSQL.Append(" INNER JOIN Mov_Destinazioni Mov_Dest_Raccolta ON Mov_Dett_Raccolta.PIVA = Mov_Dest_Raccolta.Piva AND Mov_Dett_Raccolta.Sa_Cod = Mov_Dest_Raccolta.Sa_Cod AND  Mov_Dett_Raccolta.Id_Agenda = Mov_Dest_Raccolta.Id_Agenda AND Mov_Dett_Raccolta.Id_Mov = Mov_Dest_Raccolta.Id_Mov AND Mov_Dett_Raccolta.Id_Mov_Det = Mov_Dest_Raccolta.Id_Mov_Det " & vbCrLf)
            StrSQL.Append(" INNER JOIN Fabbricati Fabbr_Raccolta ON Mov_Dest_Raccolta.Piva = Fabbr_Raccolta.PIVA AND Mov_Dest_Raccolta.Sa_Cod = Fabbr_Raccolta.SA_COD AND Mov_Dest_Raccolta.Id_Destinazione = Fabbr_Raccolta.Fabbricato_Cod " & vbCrLf)

            StrSQL.Append(" WHERE Agenda.Lav_Cod = " & Agro_SQL_SaveNum(CStr(LAVCOD_ACCETTAZIONE_DIVERSI)) & "" & vbCrLf)

            StrSQL.Append(" AND Mov_Accett.Cau_Mov      = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'" & vbCrLf)
            StrSQL.Append(" AND Mov_Conf.Cau_Mov        = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI_ALLEGATE) & "'" & vbCrLf)
            StrSQL.Append(" AND Mov_Raccolta.Cau_Mov    = '" & Agro_SQL_SaveText(CAU_ACCETTAZIONE_BENI_DA_DIVERSI) & "'" & vbCrLf)

            StrSQL.Append(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            If Doc_Numero_Sin_Bolla <> "" Then
                StrSQL.Append(" AND Mov_Accett.Doc_Numero_Sin = '" & Agro_SQL_SaveText(Doc_Numero_Sin_Bolla) & "'  " & vbCrLf)
            End If

            If Doc_Numero_Bolla <> 0 Then
                StrSQL.Append(" AND Mov_Accett.Doc_Numero = " & Agro_SQL_SaveNum(Doc_Numero_Bolla) & "  " & vbCrLf)
            End If

            If Doc_Numero_Des_Bolla <> "" Then
                StrSQL.Append(" AND Mov_Accett.Doc_Numero_Des = '" & Agro_SQL_SaveText(Doc_Numero_Des_Bolla) & "'  " & vbCrLf)
            End If

            If Anno_Bolla <> 0 Then
                StrSQL.Append(" AND Mov_Accett.Data_Movimento >= '01/01/" & Agro_SQL_SaveNum(Anno_Bolla) & "'  " & vbCrLf)
                StrSQL.Append(" AND Mov_Accett.Data_Movimento <= '31/12/" & Agro_SQL_SaveNum(Anno_Bolla) & "'  " & vbCrLf)
            End If

            If Doc_Numero_Sin_DDT <> "" Then
                StrSQL.Append(" AND Mov_Conf.Doc_Numero_Sin = '" & Agro_SQL_SaveText(Doc_Numero_Sin_DDT) & "'  " & vbCrLf)
            End If

            If Doc_Numero_DDT <> 0 Then
                StrSQL.Append(" AND Mov_Conf.Doc_Numero = " & Agro_SQL_SaveNum(Doc_Numero_DDT) & "  " & vbCrLf)
            End If

            If Doc_Numero_Des_DDT <> "" Then
                StrSQL.Append(" AND Mov_Conf.Doc_Numero_Des = '" & Agro_SQL_SaveText(Doc_Numero_Des_DDT) & "'  " & vbCrLf)
            End If

            'If Anno_DDT <> 0 Then
            '    StrSQL.Append(" AND Mov_Conf.Data_Movimento >= '01/01/" & Agro_SQL_SaveNum(Anno_DDT) & "'  " + vbCrLf)
            '    StrSQL.Append(" AND Mov_Conf.Data_Movimento <= '31/12/" & Agro_SQL_SaveNum(Anno_DDT) & "'  " + vbCrLf)
            'End If

            If Data_DDT <> "" Then
                StrSQL.Append(" AND Mov_Conf.Data_Movimento = " & Agro_SQL_SaveDate(Data_DDT) & " " & vbCrLf)
            End If

            If Cod_Contatto_Produttore <> "" Then
                StrSQL.Append(" AND Contatti_Produttori.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto_Produttore) & "'  " & vbCrLf)
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND     Mov_Dest_Raccolta.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  " & vbCrLf)
            End If
            If Fabbricato_Cod <> 0 Then
                StrSQL.Append(" AND     Mov_Dest_Raccolta.Id_Destinazione = " & Agro_SQL_SaveNum(Fabbricato_Cod) & "  " & vbCrLf)
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Mov_Accett.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Mov_Accett.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' In base al filtro impostato legge gli id_agenda dei certificati di pomodoro,
    ''' utilizzata da Leggi_Range_IdAgenda_Bolle per la composizione degli id_agenda per la stampa massiva
    ''' Default:
    ''' ByVal Data_Inizio As Date = AGRODATAINIZIO
    ''' ByVal Data_Fine As Date = AGRODATAFINE
    ''' ByVal Codice_Specie As String = ""
    ''' ByVal FiltroSpecie As String = ""
    ''' ByVal Codice_Conferente As String = ""
    ''' ByVal FiltroConferenti As String = ""
    ''' ByVal Piva_Produttore As String = ""
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function CertificatiPomodoro_DatiMinimi_Leggi(ByVal Piva As String,
                                                         ByVal Id_Agenda As Integer,
                                                         ByVal xFiltroAggiuntivo As String,
                                                         ByVal xOrderBy As String,
                                                         ByRef objParametri As AgronicaCoreParametri
                                                         ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.AccettazioneDaDiversi.CertificatiPomodoro_DatiMinimi_Leggi"

        Dim messaggioErrore As String = ""
        Dim StbSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StbSQL.Length = 0

            StbSQL.Append("SELECT   Agenda.Id_Agenda, Agenda.des_lib, " & vbCrLf)
            StbSQL.Append("         Mov_Certificato.Data_Movimento, " & vbCrLf)
            StbSQL.Append("         Mov_Accett.Doc_Numero_Sin AS Doc_Numero_Sin_Bolla, Mov_Accett.Doc_Numero AS Doc_Numero_Bolla, Mov_Accett.Doc_Numero_Des AS Doc_Numero_Des_Bolla,  " & vbCrLf)
            StbSQL.Append("         Mov_Certificato.Doc_Numero_Sin AS Doc_Numero_Sin_Cert, Mov_Certificato.Doc_Numero AS Doc_Numero_Cert, Mov_Certificato.Doc_Numero_Des AS Doc_Numero_Des_Cert  " & vbCrLf)

            StbSQL.Append(" FROM    Agenda " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti Mov_Accett ON Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Sa_Cod = Mov_Accett.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti Mov_Certificato ON Agenda.PIVA = Mov_Certificato.PIVA AND Agenda.Sa_Cod = Mov_Certificato.Sa_Cod AND Agenda.Id_Agenda = Mov_Certificato.Id_Agenda   " & vbCrLf)

            'StbSQL.Append(" INNER JOIN Risorse_Umane Risorse_Umane_Conferenti ON Mov_Accett.Cod_RisUm = Risorse_Umane_Conferenti.Cod_RisUm  " + vbCrLf)
            'StbSQL.Append(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Produttori ON Mov_Accett.Cod_Destinazione = Risorse_Umane_Produttori.Cod_RisUm  " + vbCrLf)

            'StbSQL.Append(" INNER JOIN Movimenti Mov_Raccolta ON Agenda.PIVA = Mov_Raccolta.PIVA AND Agenda.Sa_Cod = Mov_Raccolta.Sa_Cod AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda  " + vbCrLf)
            'StbSQL.Append(" INNER JOIN Movimenti_Dettagli Mov_Dett_Raccolta ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov   " + vbCrLf)
            'StbSQL.Append(" INNER JOIN Materie_Prime MP_Raccolta ON Mov_Dett_Raccolta.Elem_Cod = MP_Raccolta.Elem_Cod AND Mov_Dett_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod " + vbCrLf)

            'If Tipo_Certificato = enum_CodificaStampe.Certificato_Pomodoro_Esterno Then
            '    'il certificato esterno va stampato solo per i non surgelati
            '    StbSQL.Append(" INNER JOIN Materie_Prime_Dettagli MP_Dettagli_Raccolta ON MP_Dettagli_Raccolta.Piva = MP_Raccolta.Piva AND MP_Dettagli_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod  " + vbCrLf)
            'End If

            StbSQL.Append(" WHERE Agenda.Lav_Cod            = " & Agro_SQL_SaveNum(CStr(LAVCOD_ACCETTAZIONE_DIVERSI)) & "" & vbCrLf)
            StbSQL.Append(" AND Agenda.Tipo_Accettazione    = -1 " & vbCrLf)
            StbSQL.Append(" AND Agenda.Piva                 = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            StbSQL.Append(" AND Mov_Accett.Cau_Mov          = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'" & vbCrLf)
            StbSQL.Append(" AND Mov_Certificato.Cau_Mov     = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI_TERZIARIA) & "'" & vbCrLf)
            'StbSQL.Append(" AND Mov_Raccolta.Cau_Mov        = '" + Agro_SQL_SaveText(CAU_ACCETTAZIONE_BENI_DA_DIVERSI) + "' " + vbCrLf)
            ''StbSQL.Append(" AND Mov_Certificato.Data_Movimento   <=" & Agro_SQL_SaveDate(Validita_Fine) & " " + vbCrLf)
            ''StbSQL.Append(" AND Mov_Certificato.Data_Movimento   >=" & Agro_SQL_SaveDate(Validita_Inizio) & " " + vbCrLf)
            'StbSQL.Append(" AND Mov_Accett.Data_Movimento   <=" & Agro_SQL_SaveDate(Data_Fine) & " " + vbCrLf)
            ' StbSQL.Append(" AND Mov_Accett.Data_Movimento   >=" & Agro_SQL_SaveDate(Data_Inizio) & " " + vbCrLf)

            'If Tipo_Certificato = enum_CodificaStampe.Certificato_Pomodoro_Esterno Then
            '    'il certificato esterno va stampato solo per i non surgelati
            '    StbSQL.Append(" AND MP_Dettagli_Raccolta.Extra_Smallint4 = 0  " + vbCrLf)
            'End If

            If Id_Agenda <> 0 Then
                StbSQL.Append(" AND Agenda.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " " & vbCrLf)
            End If

            If Piva <> "" Then
                StbSQL.Append(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.Append(" ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StbSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function CertificatiPomodoro_DistinctAnni_Leggi(ByVal Piva As String,
                                                           ByVal xFiltroAggiuntivo As String,
                                                           ByVal xOrderBy As String,
                                                           ByRef objParametri As AgronicaCoreParametri
                                                           ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.AccettazioneDaDiversi.CertificatiPomodoro_DistinctAnni_Leggi"

        Dim messaggioErrore As String = ""
        Dim StbSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StbSQL.Length = 0

            'StbSQL.Append("SELECT   Agenda.Id_Agenda, Agenda.des_lib, " + vbCrLf)
            'StbSQL.Append("         Mov_Certificato.Data_Movimento, " + vbCrLf)
            'StbSQL.Append("         Mov_Accett.Doc_Numero_Sin AS Doc_Numero_Sin_Bolla, Mov_Accett.Doc_Numero AS Doc_Numero_Bolla, Mov_Accett.Doc_Numero_Des AS Doc_Numero_Des_Bolla,  " + vbCrLf)
            'StbSQL.Append("         Mov_Certificato.Doc_Numero_Sin AS Doc_Numero_Sin_Cert, Mov_Certificato.Doc_Numero AS Doc_Numero_Cert, Mov_Certificato.Doc_Numero_Des AS Doc_Numero_Des_Cert  " + vbCrLf)

            StbSQL.Append(" SELECT   DISTINCT YEAR(Mov_Certificato.Data_Movimento) AS Anno_Certificato " & vbCrLf)

            StbSQL.Append(" FROM    Agenda " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti Mov_Accett ON Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Sa_Cod = Mov_Accett.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti Mov_Certificato ON Agenda.PIVA = Mov_Certificato.PIVA AND Agenda.Sa_Cod = Mov_Certificato.Sa_Cod AND Agenda.Id_Agenda = Mov_Certificato.Id_Agenda   " & vbCrLf)

            'StbSQL.Append(" INNER JOIN Risorse_Umane Risorse_Umane_Conferenti ON Mov_Accett.Cod_RisUm = Risorse_Umane_Conferenti.Cod_RisUm  " + vbCrLf)
            'StbSQL.Append(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Produttori ON Mov_Accett.Cod_Destinazione = Risorse_Umane_Produttori.Cod_RisUm  " + vbCrLf)

            'StbSQL.Append(" INNER JOIN Movimenti Mov_Raccolta ON Agenda.PIVA = Mov_Raccolta.PIVA AND Agenda.Sa_Cod = Mov_Raccolta.Sa_Cod AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda  " + vbCrLf)
            'StbSQL.Append(" INNER JOIN Movimenti_Dettagli Mov_Dett_Raccolta ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov   " + vbCrLf)
            'StbSQL.Append(" INNER JOIN Materie_Prime MP_Raccolta ON Mov_Dett_Raccolta.Elem_Cod = MP_Raccolta.Elem_Cod AND Mov_Dett_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod " + vbCrLf)

            'If Tipo_Certificato = enum_CodificaStampe.Certificato_Pomodoro_Esterno Then
            '    'il certificato esterno va stampato solo per i non surgelati
            '    StbSQL.Append(" INNER JOIN Materie_Prime_Dettagli MP_Dettagli_Raccolta ON MP_Dettagli_Raccolta.Piva = MP_Raccolta.Piva AND MP_Dettagli_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod  " + vbCrLf)
            'End If

            StbSQL.Append(" WHERE Agenda.Lav_Cod            = " & Agro_SQL_SaveNum(CStr(LAVCOD_ACCETTAZIONE_DIVERSI)) & "" & vbCrLf)
            StbSQL.Append(" AND Agenda.Tipo_Accettazione    = -1 " & vbCrLf)
            StbSQL.Append(" AND Agenda.Piva                 = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            StbSQL.Append(" AND Mov_Accett.Cau_Mov          = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'" & vbCrLf)
            StbSQL.Append(" AND Mov_Certificato.Cau_Mov     = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI_TERZIARIA) & "'" & vbCrLf)
            'StbSQL.Append(" AND Mov_Raccolta.Cau_Mov        = '" + Agro_SQL_SaveText(CAU_ACCETTAZIONE_BENI_DA_DIVERSI) + "' " + vbCrLf)
            ''StbSQL.Append(" AND Mov_Certificato.Data_Movimento   <=" & Agro_SQL_SaveDate(Validita_Fine) & " " + vbCrLf)
            ''StbSQL.Append(" AND Mov_Certificato.Data_Movimento   >=" & Agro_SQL_SaveDate(Validita_Inizio) & " " + vbCrLf)
            'StbSQL.Append(" AND Mov_Accett.Data_Movimento   <=" & Agro_SQL_SaveDate(Data_Fine) & " " + vbCrLf)
            ' StbSQL.Append(" AND Mov_Accett.Data_Movimento   >=" & Agro_SQL_SaveDate(Data_Inizio) & " " + vbCrLf)

            'If Tipo_Certificato = enum_CodificaStampe.Certificato_Pomodoro_Esterno Then
            '    'il certificato esterno va stampato solo per i non surgelati
            '    StbSQL.Append(" AND MP_Dettagli_Raccolta.Extra_Smallint4 = 0  " + vbCrLf)
            'End If

            'If Id_Agenda <> 0 Then
            '    StbSQL.Append(" AND Agenda.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " " + vbCrLf)
            'End If

            If Piva <> "" Then
                StbSQL.Append(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.Append(" ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StbSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    '##################################################################################
    Public Function AnnoCertificatoPomodoro_from_IdAgenda(ByVal Piva As String,
                                                          ByVal Id_Agenda As Integer,
                                                          ByRef objParametri As AgronicaCoreParametri
                                                          ) As Integer

        Dim dt As DataTable
        Dim anno As Integer = 0

        dt = CertificatiPomodoro_DatiMinimi_Leggi(Piva, Id_Agenda, "", "", objParametri)

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            anno = CDate(dt.Rows(0).Item("Data_Movimento")).Year
        End If

        Return anno

    End Function


    '##################################################################################
    Public Function AnnoCertificatiPomodoro_from_Str_Id_Agenda(ByVal Piva As String,
                                                               ByVal Str_Id_Agenda_Cert As String,
                                                               ByRef MessaggioErrore As String,
                                                               ByRef objParametri As AgronicaCoreParametri
                                                               ) As Integer

        Dim dt As DataTable
        Dim anno As Integer = 0
        Dim filtro As String

        MessaggioErrore = ""

        If Str_Id_Agenda_Cert <> "" Then

            Str_Id_Agenda_Cert = Str_Id_Agenda_Cert.Replace("|", ",")

            filtro = " Agenda.Id_Agenda IN (" & Agro_SQL_Save_Clausola_IN(Str_Id_Agenda_Cert) & ")"

            dt = CertificatiPomodoro_DistinctAnni_Leggi(Piva, filtro, "", objParametri)

            If Not IsNothing(dt) Then

                Select Case dt.Rows.Count
                    Case 0 'ERRORE
                        MessaggioErrore = "Errore, anno non restituito."
                    Case 1 'CASO OK
                        anno = dt.Rows(0).Item("Anno_Certificato")
                    Case Else 'ERRORE
                        MessaggioErrore = "Errore, sono stati selezionati certificati di anni diversi, occorre selezionare certificati dello stesso anno."
                End Select
            Else
                MessaggioErrore = "Errore, dati non restituiti."
            End If

        Else
            MessaggioErrore = "Filtro id_agenda vuoto."
        End If

        Return anno

    End Function

End Class


'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§


Public Class AccettazioneDaDiversi_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function ModificaNumCertificato_ByIdAgenda(ByVal Piva As String,
                                                      ByVal Sa_Cod As Int32,
                                                      ByVal Id_Agenda As Int32,
                                                      ByVal Doc_Numero_Sin As String,
                                                      ByVal Doc_Numero As Integer,
                                                      ByVal Doc_Numero_Des As String,
                                                      ByVal xFiltroAggiuntivo As String,
                                                      ByRef objParametri As AgronicaCoreParametri
                                                      ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.AccettazioneDaDiversi_W.ModificaNumCertificato_ByIdAgenda()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            'If Sa_Cod = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            'End If

            If Id_Agenda = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Agenda obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" UPDATE Mov_Certificato SET ")
            StrSQL.Append("    Doc_Numero_Sin    = '" & Agro_SQL_SaveText(Doc_Numero_Sin) & "'   ")
            StrSQL.Append("   ,Doc_Numero        =  " & Agro_SQL_SaveNum(Doc_Numero) & "    ")
            StrSQL.Append("   ,Doc_Numero_Des    = '" & Agro_SQL_SaveText(Doc_Numero_Des) & "'   ")
            StrSQL.Append("   ,Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            StrSQL.Append(" FROM Movimenti Mov_Certificato ")
            'StrSQL.Append(" INNER JOIN Agenda.PIVA = Mov_Certificato.PIVA AND Agenda.Sa_Cod = Mov_Certificato.Sa_Cod AND Agenda.Id_Agenda = Mov_Certificato.Id_Agenda   " + vbCrLf)
            'StrSQL.Append(" INNER JOIN Movimenti Mov_Accett ON Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Sa_Cod = Mov_Accett.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda " + vbCrLf)

            StrSQL.Append(" WHERE Piva      = '" & Agro_SQL_SaveText(Piva) & "'  ")
            StrSQL.Append(" AND   Sa_Cod    =  " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            StrSQL.Append(" AND   Id_Agenda =  " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
            'IMPORTANTE IL FILTRO SUL CAU_MOV! altrimenti aggiorna doc_numero di tutti i record e sovrascrive il numero bolla!
            StrSQL.Append(" AND   Cau_Mov   = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI_TERZIARIA) & "'  ")

            '----------------------------------------------------------------------
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

End Class
