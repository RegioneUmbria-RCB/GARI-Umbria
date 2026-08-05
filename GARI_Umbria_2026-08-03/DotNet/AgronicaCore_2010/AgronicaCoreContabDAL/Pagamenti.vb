Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Agro_Math
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDTOStd.Identity

Public Class Pagamenti_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Piva As String,
                          ByVal Sa_Cod As Integer,
                          ByVal Id_Agenda As Integer,
                          ByVal Id_Mov As Integer,
                          ByVal Cod_Pagamento As Integer,
                          ByVal Cod_Liquidita As Integer,
                          ByVal Cau_Pagamento As Integer,
                          ByVal Flag_SoloScadute As Boolean,
                          ByVal Flag_Insolvenze As Boolean,
                          ByVal Flag_Riscossioni As Boolean,
                          ByVal Cau_Mov As String,
                          ByVal Cod_RisUm As Integer,
                          ByVal Piva_Contatto As String,
                          ByVal Cod_Contatto As String,
                          ByVal Cod_Conto As Integer,
                          ByVal FinestraTemp_Inizio As String,
                          ByVal FinestraTemp_Fine As String,
                          ByVal Cod_RisUm_Origine As Integer,
                          ByVal Piva_SuperUser_Origine As String,
                          ByVal Flag_AncheImportati As Boolean,
                          ByVal Flag_CostiAccessori_Corrispettivi As Boolean,
                          ByVal Flag_Contabilita As Boolean,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Pagamenti_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""            =>  
        '   Sa_Cod = 0           =>  
        '   Id_Agenda = 0        =>
        '   Id_Mov = 0           =>  
        '   Cod_Pagamento = 0    =>  
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    strSql.AppendLine(" SELECT Movimenti.PIVA, Movimenti.Sa_Cod, Movimenti.Id_Agenda, Movimenti.Id_Mov, Imprese.rag_soc AS Impresa, Agenda.Lav_Cod, Agenda.des_lib,  ")
                    strSql.AppendLine(" Movimenti.Cau_Mov, Movimenti.Doc_Numero_Sin, Movimenti.Doc_Numero, Movimenti.Doc_Numero_Des, Movimenti.Mov_Desc, Movimenti.Scadenza, Movimenti.Data_Movimento, ")
                    strSql.AppendLine(" ISNULL(Movimenti.Num_Protocollo,0) AS Num_Protocollo, Movimenti.Causale_Trasporto,  Movimenti.Scadenza_Extra,  ")
                    strSql.AppendLine(" Pagamenti.*, Liquidita_DARE.Cod_Contatto AS Cod_Contatto_DARE,  ")
                    strSql.AppendLine(" Liquidita_DARE.Numero AS Numero_DARE, Liquidita_DARE.Abi AS Abi_DARE, Liquidita_DARE.Cab AS Cab_DARE, Liquidita_DARE.Cin AS Cin_DARE,   ")
                    strSql.AppendLine("  Liquidita_DARE.Cifre_Controllo AS Cifre_Controllo_DARE, Liquidita_DARE.Nazione AS Nazione_DARE, Liquidita_DARE.Bic AS Bic_DARE, ")
                    strSql.AppendLine("  Liquidita_DARE.Interbancario AS Interbancario_DARE, Liquidita_DARE.Saldo_Attuale AS Saldo_Attuale_DARE,  ")
                    strSql.AppendLine("  Liquidita_DARE.Saldo_Iniziale AS Saldo_Iniziale_DARE, Liquidita_DARE.Cau_Risorsa AS Cau_Risorsa_DARE, ")
                    strSql.AppendLine("  Liquidita_DARE.Avviso AS Avviso_DARE, Liquidita_DARE.Importo_Avviso AS Importo_Avviso_DARE, Liquidita_DARE.Note AS Note_DARE, ")
                    strSql.AppendLine(" Liquidita_DARE.Cod_Istituto AS Cod_Istituto_DARE, Ist_Credito_DARE.Istituto_Des AS Istituto_Des_DARE, Ist_Credito_DARE.Filiale AS Filiale_DARE,  ")
                    strSql.AppendLine(" Liquidita_AVERE.Cod_Contatto AS Cod_Contatto_AVERE, Liquidita_AVERE.Numero AS Numero_AVERE, Liquidita_AVERE.Abi AS Abi_AVERE,  ")
                    strSql.AppendLine("  Liquidita_AVERE.Cab AS Cab_AVERE, Liquidita_AVERE.Interbancario AS Interbancario_AVERE,   ")
                    strSql.AppendLine("  Liquidita_AVERE.Saldo_Attuale AS Saldo_Attuale_AVERE, Liquidita_AVERE.Saldo_Iniziale AS Saldo_Iniziale_AVERE,  ")
                    strSql.AppendLine("  Liquidita_AVERE.Cau_Risorsa AS Cau_Risorsa_AVERE, Liquidita_AVERE.Cod_Istituto AS Cod_Istituto_AVERE,  ")
                    strSql.AppendLine("   Liquidita_AVERE.Avviso AS Avviso_AVERE, Liquidita_AVERE.Importo_Avviso AS Importo_Avviso_AVERE, Liquidita_AVERE.Note AS Note_AVERE,  ")
                    strSql.AppendLine(" Liquidita_AVERE.Cin AS Cin_AVERE, Liquidita_AVERE.Cifre_Controllo AS Cifre_Controllo_AVERE, Liquidita_AVERE.Nazione AS Nazione_AVERE,  ")
                    strSql.AppendLine("  Liquidita_AVERE.Bic AS Bic_AVERE, Ist_Credito_AVERE.Istituto_Des AS Istituto_Des_AVERE, Ist_Credito_AVERE.Filiale AS Filiale_AVERE, ")
                    strSql.AppendLine(" Movimenti_dettagli.Cod_Iva, Movimenti_dettagli.Cod_Conto, Movimenti_dettagli.Anno, Movimenti_dettagli.Ric_Cod, Movimenti_dettagli.Imponibile, ")
                    strSql.AppendLine(" Movimenti_dettagli.Iva, Movimenti_dettagli.Imponibile_Netto, RicXConti.Id_Riclassificazione, RicXConti.Dare_Avere, RicXConti.Saldo,   ")
                    strSql.AppendLine(" RicXConti.Imputabile, Conti.Conto_Descr ")
                    strSql.AppendLine("  ")

                    If Flag_CostiAccessori_Corrispettivi Then
                        strSql.AppendLine(", ")
                        strSql.AppendLine(" RisUm_Corrisp.Cod_RisUm AS Cod_RisUm_Corrisp, RisUm_Corrisp.Settore_Des AS Settore_Des_Corrisp, RisUm_Corrisp.Attivita_Des AS Attivita_Des_Corrisp, Contatti_Corrisp.Id_CF AS Id_CF_Corrisp, Contatti_Corrisp.Rag_Soc AS Rag_Soc_Corrisp, Contatti_Corrisp.Codice_Fiscale AS Codice_Fiscale_Corrisp, Contatti_Corrisp.Sa_Cod AS Sa_Cod_Corrisp, Contatti_Corrisp.Cod_Contatto AS Cod_Contatto_Corrisp   ")
                    End If

                    If Flag_Contabilita Then
                        strSql.AppendLine(", ")
                        strSql.AppendLine(" RisUm_Contab.Cod_RisUm AS Cod_RisUm_Contab, RisUm_Contab.Settore_Des AS Settore_Des_Contab, RisUm_Contab.Attivita_Des AS Attivita_Des_Contab, Contatti_Contab.Id_CF AS Id_CF_Contab, Contatti_Contab.Rag_Soc AS Rag_Soc_Contab, Contatti_Contab.Codice_Fiscale AS Codice_Fiscale_Contab, Contatti_Contab.Sa_Cod AS Sa_Cod_Contab, Contatti_Contab.Cod_Contatto AS Cod_Contatto_Contab  ")
                    End If

                    strSql.AppendLine("   ")
                    strSql.AppendLine("  ")
                    strSql.AppendLine("  ")
                    strSql.AppendLine("  ")

                    'JOIN AGENDA - MOVIMENTI
                    strSql.AppendLine(" FROM    Agenda INNER JOIN Movimenti ")
                    strSql.AppendLine(" ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda")

                    'JOIN AGENDA - MOVIMENTI CONTAB
                    strSql.AppendLine(" INNER JOIN Movimenti Movimenti_Contab ")
                    strSql.AppendLine(" ON Agenda.PIVA = Movimenti_Contab.PIVA AND Agenda.Sa_Cod = Movimenti_Contab.Sa_Cod AND Agenda.Id_Agenda = Movimenti_Contab.Id_Agenda")

                    'JOIN IMPRESE - AGENDA
                    strSql.AppendLine(" INNER JOIN Imprese ON Imprese.Piva = Agenda.Piva ")

                    'JOIN MOVIMENTI - MOVIMENTI DETTAGLI
                    strSql.AppendLine(" INNER JOIN Movimenti_dettagli ")
                    strSql.AppendLine(" ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov ")

                    'JOIN MOVIMENTI DETTAGLI - RICXCONTI 
                    strSql.AppendLine(" LEFT OUTER JOIN  RicXConti ON Movimenti_dettagli.PIVA = RicXConti.Piva AND Movimenti_dettagli.Cod_Conto = RicXConti.Cod_Conto AND Movimenti_dettagli.Anno = RicXConti.Anno AND Movimenti_dettagli.Ric_Cod = RicXConti.Ric_Cod ")

                    'JOIN RICXCONTI - CONTI
                    strSql.AppendLine(" LEFT OUTER JOIN Conti ON RicXConti.Cod_Conto = Conti.Cod_Conto ")

                    If Flag_CostiAccessori_Corrispettivi Then
                        'JOIN MOVIMENTI DETTAGLI - RISORSE UMANE CORRISPETTIVI
                        'non mettere in join la piva, mi raccomando!!!!!!
                        strSql.AppendLine(" LEFT OUTER JOIN Risorse_Umane RisUm_Corrisp ON RisUm_Corrisp.Cod_RisUm = Movimenti_dettagli.Mat_Cod ")

                        'JOIN RISORSE UMANE - CONTATTI CORRISPETTIVI
                        strSql.AppendLine(" LEFT OUTER JOIN Contatti Contatti_Corrisp ON RisUm_Corrisp.Piva = Contatti_Corrisp.Piva AND RisUm_Corrisp.Cod_Contatto = Contatti_Corrisp.Cod_Contatto")
                    End If

                    If Flag_Contabilita Then
                        'JOIN MOVIMENTI DETTAGLI - RISORSE UMANE CONTAB
                        'non mettere in join la piva, mi raccomando!!!!!!
                        strSql.AppendLine(" LEFT OUTER JOIN Risorse_Umane RisUm_Contab ON RisUm_Contab.Cod_RisUm = Movimenti_Contab.Cod_RisUm ")

                        'JOIN RISORSE UMANE - CONTATTI CONTAB
                        strSql.AppendLine(" LEFT OUTER JOIN Contatti Contatti_Contab ON RisUm_Contab.Piva = Contatti_Contab.Piva AND RisUm_Contab.Cod_Contatto = Contatti_Contab.Cod_Contatto")
                    End If

                    'JOIN MOVIMENTI CONTAB - PAGAMENTI
                    strSql.AppendLine(" INNER JOIN Pagamenti ")
                    strSql.AppendLine(" ON Pagamenti.PIVA = Movimenti_Contab.PIVA AND Pagamenti.Id_Agenda = Movimenti_Contab.Id_Agenda AND Pagamenti.Id_Mov = Movimenti_Contab.Id_Mov ")

                    'PAGAMENTI - LIQUIDITA DARE
                    strSql.AppendLine(" LEFT OUTER JOIN  Liquidita Liquidita_DARE ON Liquidita_DARE.Piva = Pagamenti.Piva AND Liquidita_DARE.Sa_Cod = Pagamenti.Sa_Cod AND Liquidita_DARE.Cod_Liquidita = Pagamenti.Cod_Liquidita_Dare ")

                    'LIQUIDITA DARE - IST CREDITO DARE
                    'StrSQL.AppendLine(" LEFT OUTER JOIN Ist_Credito Ist_Credito_DARE ON Ist_Credito_DARE.Piva = Liquidita_DARE.Piva AND Ist_Credito_DARE.Sa_Cod = Liquidita_DARE.Sa_Cod AND Ist_Credito_DARE.Cod_Istituto = Liquidita_DARE.Cod_Istituto ")
                    'CORREZIONE BUG DEL 07/01/2013: la piva di ist_credito è quella del superuser!
                    strSql.AppendLine(" LEFT OUTER JOIN Ist_Credito Ist_Credito_DARE ON Ist_Credito_DARE.Cod_Istituto = Liquidita_DARE.Cod_Istituto ")
                    strSql.AppendLine("                 AND Ist_Credito_DARE.Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

                    'PAGAMENTI - LIQUIDITA AVERE
                    strSql.AppendLine(" LEFT OUTER JOIN  Liquidita Liquidita_AVERE ON Liquidita_AVERE.Piva = Pagamenti.Piva AND Liquidita_AVERE.Sa_Cod = Pagamenti.Sa_Cod AND Liquidita_AVERE.Cod_Liquidita = Pagamenti.Cod_Liquidita_Avere ")

                    'LIQUIDITA AVERE - IST CREDITO AVERE
                    'StrSQL.AppendLine(" LEFT OUTER JOIN Ist_Credito Ist_Credito_AVERE ON Ist_Credito_AVERE.Piva = Liquidita_AVERE.Piva AND Ist_Credito_AVERE.Sa_Cod = Liquidita_AVERE.Sa_Cod AND Ist_Credito_AVERE.Cod_Istituto = Liquidita_AVERE.Cod_Istituto ")
                    'CORREZIONE BUG DEL 07/01/2013: la piva di ist_credito è quella del superuser!
                    strSql.AppendLine(" LEFT OUTER JOIN Ist_Credito Ist_Credito_AVERE ON Ist_Credito_AVERE.Cod_Istituto = Liquidita_AVERE.Cod_Istituto ")
                    strSql.AppendLine("                 AND Ist_Credito_AVERE.Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

                    'WHERE
                    strSql.AppendLine(" WHERE   Pagamenti.Validita_Inizio <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " ")
                    strSql.AppendLine(" AND     Pagamenti.Validita_Fine >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")

                    strSql.AppendLine(" AND     Pagamenti.Data_Pagamento <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " ")
                    strSql.AppendLine(" AND     Pagamenti.Data_Pagamento >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")

                    strSql.AppendLine(" AND     Movimenti_Contab.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'   ")

                    If Piva <> "" Then
                        strSql.AppendLine(" AND Pagamenti.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If

                    If Sa_Cod <> 0 Then
                        strSql.AppendLine(" AND Pagamenti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Id_Agenda <> 0 Then
                        strSql.AppendLine(" AND Pagamenti.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
                    End If

                    If Id_Mov <> 0 Then
                        strSql.AppendLine(" AND Pagamenti.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
                    End If

                    If Cod_Pagamento <> 0 Then
                        strSql.AppendLine(" AND Pagamenti.Cod_Pagamento = " & Agro_SQL_SaveNum(Cod_Pagamento) & "   ")
                    End If

                    If Cau_Pagamento <> 0 Then
                        strSql.AppendLine(" AND  Pagamenti.Cau_Pagamento = " & Agro_SQL_SaveNum(Cau_Pagamento) & "   ")
                    End If

                    If Cod_Liquidita <> 0 Then
                        strSql.AppendLine(" AND   (Pagamenti.Cod_Liquidita_Dare = " & Agro_SQL_SaveNum(Cod_Liquidita) & " OR Pagamenti.Cod_Liquidita_Avere = " & Agro_SQL_SaveNum(Cod_Liquidita) & ")")
                    End If

                    If Cod_RisUm <> 0 Then
                        strSql.AppendLine(" AND Movimenti.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   ")
                    End If

                    If Flag_Contabilita Then
                        If Cod_Contatto <> "" Then
                            strSql.AppendLine(" AND Contatti_Contab.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "'   ")
                        End If
                        If Piva_Contatto <> "" Then
                            strSql.AppendLine(" AND Contatti_Contab.Piva = '" & Agro_SQL_SaveText(Piva_Contatto) & "'   ")
                        End If
                        'gias2gias
                        If Cod_RisUm_Origine <> 0 Then
                            strSql.AppendLine(" AND  RisUm_Contab.Cod_RisUm_Origine = " & Agro_SQL_SaveNum(Cod_RisUm_Origine) & "   ")
                        End If
                        If Piva_SuperUser_Origine <> "" Then
                            strSql.AppendLine(" AND  RisUm_Contab.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'")
                        End If
                        If Not Flag_AncheImportati Then
                            strSql.AppendLine(" AND  RisUm_Contab.Cod_RisUm_Origine = " & Agro_SQL_SaveNum(0) & " ")
                        End If
                        'fine gias2gias

                    ElseIf Flag_CostiAccessori_Corrispettivi Then
                        If Cod_Contatto <> "" Then
                            strSql.AppendLine(" AND Contatti_Corrisp.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "'   ")
                        End If
                        If Piva_Contatto <> "" Then
                            strSql.AppendLine(" AND Contatti_Corrisp.Piva = '" & Agro_SQL_SaveText(Piva_Contatto) & "'   ")
                        End If
                        'gias2gias
                        If Cod_RisUm_Origine <> 0 Then
                            strSql.AppendLine(" AND  RisUm_Corrisp.Cod_RisUm_Origine = " & Agro_SQL_SaveNum(Cod_RisUm_Origine) & "   ")
                        End If
                        If Piva_SuperUser_Origine <> "" Then
                            strSql.AppendLine(" AND  RisUm_Corrisp.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'")
                        End If
                        If Not Flag_AncheImportati Then
                            strSql.AppendLine(" AND  RisUm_Corrisp.Cod_RisUm_Origine = " & Agro_SQL_SaveNum(0) & " ")
                        End If
                        'fine gias2gias
                    End If

                    If Cau_Mov <> "" Then
                        strSql.AppendLine(" AND Movimenti.Cau_Mov = '" & Agro_SQL_SaveText(Cau_Mov) & "'   ")
                    End If

                    If Flag_Insolvenze Then
                        strSql.AppendLine(" AND    Pagamenti.Data_Pagamento = " & Agro_SQL_SaveDate(AGRODATAFINE) & " ")
                    ElseIf Flag_Riscossioni Then
                        strSql.AppendLine(" AND    Pagamenti.Data_Pagamento < " & Agro_SQL_SaveDate(AGRODATAFINE) & " ")
                    End If

                    If Flag_SoloScadute Then
                        strSql.AppendLine(" AND Movimenti.Scadenza < " & Agro_SQL_SaveDate(Now.Date) & " ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    If xOrderBy <> "" Then
                        strSql.AppendLine(Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Agenda.PIVA, Pagamenti.Data_Pagamento DESC ")
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

    Public Function Leggi(ByVal Piva As String,
                          ByVal Sa_Cod As Integer,
                          ByVal Id_Agenda As Integer,
                          ByVal Id_Mov As Integer,
                          ByVal Cod_Pagamento As Integer,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Pagamenti_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""            =>  
        '   Sa_Cod = 0           =>  
        '   Id_Agenda = 0        =>
        '   Id_Mov = 0           =>  
        '   Cod_Pagamento = 0    =>  
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    stb.Length = 0
                    stb.AppendLine(" SELECT Pagamenti.*   ")
                    stb.AppendLine(" FROM   Pagamenti WITH(NOLOCK)")
                    stb.AppendLine(" WHERE  Pagamenti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    stb.AppendLine(" AND    Pagamenti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then
                        stb.AppendLine(" AND Pagamenti.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If

                    If Sa_Cod <> 0 Then
                        stb.AppendLine(" AND Pagamenti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Id_Agenda <> 0 Then
                        stb.AppendLine(" AND Pagamenti.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
                    End If

                    If Id_Mov <> 0 Then
                        stb.AppendLine(" AND Pagamenti.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
                    End If

                    If Cod_Pagamento <> 0 Then
                        stb.AppendLine(" AND Pagamenti.Cod_Pagamento = " & Agro_SQL_SaveNum(Cod_Pagamento) & "   ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            stb.AppendLine(" AND   Pagamenti.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            stb.AppendLine(" AND   Pagamenti.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        stb.AppendLine(" ORDER BY Pagamenti.Piva ASC, Pagamenti.Sa_Cod, Pagamenti.Id_Agenda ASC ")
                    End If


                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    stb.Length = 0
                    stb.AppendLine(" SELECT Pagamenti.*, Agenda.Lav_Cod, Agenda.Des_Lib, Movimenti.Cod_RisUm, Movimenti.Cau_Mov, Movimenti.Scadenza, Movimenti.Data_Movimento, Movimenti.Num_Protocollo " & "    ")
                    stb.AppendLine(" FROM   Pagamenti WITH(NOLOCK), Agenda WITH(NOLOCK), Movimenti WITH(NOLOCK)")
                    stb.AppendLine(" WHERE  Pagamenti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    stb.AppendLine(" AND    Pagamenti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    stb.AppendLine(" AND    Agenda.Piva = Movimenti.Piva" & " ")
                    stb.AppendLine(" AND    Agenda.Sa_Cod = Movimenti.Sa_Cod" & " ")
                    stb.AppendLine(" AND    Agenda.Id_Agenda = Movimenti.Id_Agenda" & " ")
                    stb.AppendLine(" AND    Movimenti.Piva = Pagamenti.Piva" & " ")
                    stb.AppendLine(" AND    Movimenti.Sa_Cod = Pagamenti.Sa_Cod" & " ")
                    stb.AppendLine(" AND    Movimenti.Id_Agenda = Pagamenti.Id_Agenda" & " ")
                    stb.AppendLine(" AND    Movimenti.Id_Mov = Pagamenti.Id_Mov" & " ")

                    If Piva <> "" Then
                        stb.AppendLine(" AND Pagamenti.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If

                    If Sa_Cod <> 0 Then
                        stb.AppendLine(" AND Pagamenti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Id_Agenda <> 0 Then
                        stb.AppendLine(" AND Pagamenti.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
                    End If

                    If Id_Mov <> 0 Then
                        stb.AppendLine(" AND Pagamenti.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
                    End If

                    If Cod_Pagamento <> 0 Then
                        stb.AppendLine(" AND Pagamenti.Cod_Pagamento = " & Agro_SQL_SaveNum(Cod_Pagamento) & "   ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            stb.AppendLine(" AND   Agenda.Inviato >=0 ")
                            stb.AppendLine(" AND   Movimenti.Inviato >=0 ")
                            stb.AppendLine(" AND   Pagamenti.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            stb.AppendLine(" AND   Agenda.Inviato =-1 ")
                            stb.AppendLine(" AND   Movimenti.Inviato =-1 ")
                            stb.AppendLine(" AND   Pagamenti.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        stb.AppendLine(" ORDER BY Pagamenti.Piva ASC, Pagamenti.Sa_Cod, Pagamenti.Id_Agenda ASC ")
                    End If


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni
                    stb.Length = 0
                    stb.AppendLine(" SELECT  ")
                    stb.AppendLine("      p.* ")
                    stb.AppendLine("    , a.Lav_Cod ")
                    stb.AppendLine("    , a.Des_Lib ")
                    stb.AppendLine("    , m.Cod_RisUm ")
                    stb.AppendLine("    , m.Cau_Mov ")
                    stb.AppendLine("    , m.Scadenza ")
                    stb.AppendLine("    , m.Data_Movimento ")
                    stb.AppendLine("    , m.Num_Protocollo ")
                    stb.AppendLine("    , pc.* ")

                    stb.AppendLine(" FROM Pagamenti p WITH(NOLOCK)")
                    stb.AppendLine("    inner join Agenda a  WITH(NOLOCK)")
                    stb.AppendLine("        on a.Piva = p.Piva ")
                    stb.AppendLine("             AND    a.Sa_Cod = p.Sa_Cod ")
                    stb.AppendLine("             AND    a.Id_Agenda = p.Id_Agenda ")
                    stb.AppendLine("  ")
                    stb.AppendLine("    inner join Movimenti  m WITH(NOLOCK)")
                    stb.AppendLine("        on a.Piva = m.Piva ")
                    stb.AppendLine("                 AND    p.Sa_Cod = m.Sa_Cod ")
                    stb.AppendLine("                 AND    p.Id_Agenda = m.Id_Agenda ")
                    stb.AppendLine("                AND    p.Id_Mov = m.Id_Mov ")
                    stb.AppendLine("  ")
                    stb.AppendLine("    inner join Pagamenti_Causali pc WITH(NOLOCK)")
                    stb.AppendLine("        on pc.Cau_Pagamento = p.Cau_Pagamento ")


                    stb.AppendLine(" WHERE  p.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    stb.AppendLine(" and p.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")


                    If Piva <> "" Then
                        stb.AppendLine(" AND p.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If

                    If Sa_Cod <> 0 Then
                        stb.AppendLine(" AND p.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Id_Agenda <> 0 Then
                        stb.AppendLine(" AND p.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
                    End If

                    If Id_Mov <> 0 Then
                        stb.AppendLine(" AND p.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
                    End If

                    If Cod_Pagamento <> 0 Then
                        stb.AppendLine(" AND p.Cod_Pagamento = " & Agro_SQL_SaveNum(Cod_Pagamento) & "   ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            stb.AppendLine(" AND   a.Inviato >=0 ")
                            stb.AppendLine(" AND   m.Inviato >=0 ")
                            stb.AppendLine(" AND   p.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            stb.AppendLine(" AND   a.Inviato =-1 ")
                            stb.AppendLine(" AND   m.Inviato =-1 ")
                            stb.AppendLine(" AND   p.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        stb.AppendLine(" ORDER BY p.Piva ASC, p.Sa_Cod, p.Id_Agenda ASC ")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinCompleta


            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    '#####################################################################################################
    'fa la query e poi chiama Riscosso_NonRiscosso_ParzialmenteRiscosso
    Public Sub Riscosso_NonRiscosso_ParzialmenteRiscosso_byQuery(ByVal Piva As String,
                                                                 ByVal Sa_Cod As Integer,
                                                                 ByVal Id_Agenda As Integer,
                                                                 ByVal Id_Mov As Integer,
                                                                 ByVal Lav_Cod As Integer,
                                                                 ByVal Totale_Documento As Decimal,
                                                                 ByRef Flag_Riscosso As Boolean,
                                                                 ByRef Flag_NonRiscosso As Boolean,
                                                                 ByRef Flag_ParzialmenteRiscosso As Boolean,
                                                                 ByRef Tot_Importo_gia_Pagato As Decimal,
                                                                 ByRef Tot_Importo_da_Pagare As Decimal,
                                                                 ByVal xFiltroAggiuntivo As String,
                                                                 ByVal xOrderBy As String,
                                                                 ByRef objParametri As AgronicaCoreParametri
                                                                 )

        Const nomeRoutine = "AgronicaCoreContabDAL.Pagamenti_R.Riscosso_NonRiscosso_ParzialmenteRiscosso_byQuery()"
        Dim messaggioErrore As String = ""

        Try

            Dim dtPag As DataTable

            dtPag = Leggi(Piva,
                          0,
                          Id_Agenda,
                          Id_Mov,
                          0,
                          enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                          "", "",
                          objParametri)

            If Not IsNothing(dtPag) AndAlso dtPag.Rows.Count > 0 Then

                Riscosso_NonRiscosso_ParzialmenteRiscosso(dtPag,
                                                          Piva,
                                                          Id_Agenda,
                                                          Id_Mov,
                                                          Lav_Cod,
                                                          Totale_Documento,
                                                          Flag_Riscosso,
                                                          Flag_NonRiscosso,
                                                          Flag_ParzialmenteRiscosso,
                                                          Tot_Importo_gia_Pagato,
                                                          Tot_Importo_da_Pagare,
                                                          objParametri)

            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

    '#####################################################################################################
    'passato un datatable dei pagamenti, determina se il documento è stato riscosso, non riscosso, parzialmente riscosso
    Public Sub Riscosso_NonRiscosso_ParzialmenteRiscosso_OLD(ByVal DT_Pag As DataTable,
                                                             ByVal Piva As String,
                                                             ByVal Sa_Cod As Integer,
                                                             ByVal Id_Agenda As Integer,
                                                             ByVal Id_Mov As Integer,
                                                             ByVal Totale_Documento As Decimal,
                                                             ByRef Flag_Riscosso As Boolean,
                                                             ByRef Flag_NonRiscosso As Boolean,
                                                             ByRef Flag_ParzialmenteRiscosso As Boolean,
                                                             ByRef Importo_Pagato As Decimal,
                                                             ByRef objParametri As AgronicaCoreParametri)

        Const nomeRoutine = "AgronicaCoreContabDAL.Pagamenti_R.Riscosso_NonRiscosso_ParzialmenteRiscosso_OLD()"
        Dim messaggioErrore As String = ""

        Importo_Pagato = 0

        Try

            Flag_Riscosso = False
            Flag_NonRiscosso = True
            Flag_ParzialmenteRiscosso = False

            If Not IsNothing(DT_Pag) AndAlso DT_Pag.Rows.Count > 0 Then

                Dim drPag As DataRow()

                drPag = DT_Pag.Select(" piva = '" & Agro_SQL_SaveText(Piva, False) & "'" &
                                      " AND sa_cod = " & Agro_SQL_SaveNum(Sa_Cod, False) &
                                      " AND id_agenda = " & Agro_SQL_SaveNum(Id_Agenda, False) &
                                      " AND id_mov =" & Agro_SQL_SaveNum(Id_Mov, False))

                Dim x_Percentuale_Pagamento As Integer
                Dim x_Importo_Pagamento As Decimal
                ' Dim x_Importo_NonPagamento As Decimal
                Dim AlmenoUno_NonPagato As Boolean = False

                If Not IsNothing(drPag) AndAlso drPag.Length > 0 Then
                    Dim i As Integer
                    For i = 0 To drPag.Length - 1

                        x_Percentuale_Pagamento = drPag(i).Item("percentuale")

                        x_Importo_Pagamento = drPag(i).Item("importo")

                        Importo_Pagato += x_Importo_Pagamento

                        ' x_Data_Pagamento = .Item("data_pagamento")

                        If x_Percentuale_Pagamento = 0 Then
                            'If x_Data_Pagamento <> AGRODATAFINE Then
                            '--------------------------
                            'PAGAMENTO GIA' EFFETTUATO
                            '--------------------------
                            'End If
                        Else
                            '------------------------------
                            'PAGAMENTO ANCORA DA EFFETTUARE
                            '------------------------------
                            AlmenoUno_NonPagato = True
                        End If

                    Next

                    If Not AlmenoUno_NonPagato Then
                        'se le percentuali sono tutte 0,
                        'allora il documento è riscosso
                        Flag_Riscosso = True

                        Flag_NonRiscosso = False
                        Flag_ParzialmenteRiscosso = False

                    Else
                        'verifico se è stato pagamento in parte oppure no
                        Select Case Importo_Pagato
                            Case 0 'non riscosso
                                Flag_NonRiscosso = True

                                Flag_Riscosso = False
                                Flag_ParzialmenteRiscosso = False

                            Case Else 'parzialmente riscosso
                                Flag_ParzialmenteRiscosso = True

                                Flag_Riscosso = False
                                Flag_NonRiscosso = False

                        End Select
                    End If

                End If 'dr

            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub


    '#####################################################################################################
    Public Sub Riscosso_NonRiscosso_ParzialmenteRiscosso(ByVal Dt_Pag As DataTable,
                                                         ByVal Piva As String,
                                                         ByVal Id_Agenda As Integer,
                                                         ByVal Id_Mov As Integer,
                                                         ByVal Lav_Cod As Integer,
                                                         ByVal x_Num_Protocollo As Decimal,
                                                         ByRef Flag_Riscosso As Boolean,
                                                         ByRef Flag_NonRiscosso As Boolean,
                                                         ByRef Flag_ParzialmenteRiscosso As Boolean,
                                                         ByRef Tot_Importo_gia_Pagato As Decimal,
                                                         ByRef Tot_Importo_da_Pagare As Decimal,
                                                         ByRef objParametri As AgronicaCoreParametri)

        Const nomeRoutine = "AgronicaCoreContabDAL.Pagamenti_R.Riscosso_NonRiscosso_ParzialmenteRiscosso()"
        Dim messaggioErrore As String = ""

        Try

            'Modalita_Pag_Effettuata = ""
            'Flag_Riscossioni = 0
            Flag_Riscosso = False
            Flag_NonRiscosso = False
            Flag_ParzialmenteRiscosso = False
            Tot_Importo_gia_Pagato = 0
            Tot_Importo_da_Pagare = 0

            If Not IsNothing(Dt_Pag) AndAlso Dt_Pag.Rows.Count > 0 Then

                Dim j As Integer

                'Dim x_Note_Pagamento As String
                Dim x_Percentuale_Pagamento As Decimal
                Dim x_Importo_Pagamento As Decimal
                'Dim x_Data_Pagamento As Date
                'Dim x_Cau_Pagamento_Sigla As String
                'Dim x_Tipo_Pagamento As Integer
                'Dim Mod_Pag_PercImporto, Mod_Banche As String
                Dim Importo_Pagato_Corrente As Decimal = 0


                'x_Num_Protocollo = ArrotondaVal_2(x_Num_Protocollo)


                '===============================================================================
                '================          PAGAMENTO EFFETTUATO        =======================
                '===============================================================================

                Dim drPagEffettuato As DataRow()

                drPagEffettuato = Dt_Pag.Select(" piva = '" & Agro_SQL_SaveText(Piva, False) & "'" &
                                                " AND id_agenda = " & Agro_SQL_SaveNum(Id_Agenda, False) &
                                                " AND Previsto_Avvenuto = 1 ")

                If Not IsNothing(drPagEffettuato) AndAlso drPagEffettuato.Length > 0 Then
                    For j = 0 To drPagEffettuato.Length - 1
                        With drPagEffettuato(j)

                            x_Percentuale_Pagamento = CDec(.Item("percentuale"))
                            'x_Cau_Pagamento_Sigla = .Item("Cau_Pagamento_Sigla")
                            x_Importo_Pagamento = .Item("importo")
                            'x_Data_Pagamento = .Item("data_pagamento")
                            'x_Tipo_Pagamento = CInt(.Item("Tipo_Pagamento"))
                            'x_Note_Pagamento = .Item("note")

                            Select Case x_Percentuale_Pagamento
                                Case 0
                                    'gestione importo fisso salvato dall'utente
                                    Importo_Pagato_Corrente = x_Importo_Pagamento
                                Case 100
                                    'modifica del 18/02/2013:
                                    'anche se la percentuale è 100%, può essere che l'importo non coincide con il totale
                                    'ad esempio la molinelli aveva questo caso:
                                    'totale= 930,47
                                    '1 tranche =930,46
                                    '2 tranche = 0,01
                                    'Importo_Pagato_Corrente = x_Num_Protocollo
                                    Importo_Pagato_Corrente = x_Importo_Pagamento
                                Case Else
                                    If x_Importo_Pagamento = 0 Then
                                        'gestione con percentuali
                                        'TODO: CALCOLO (direi che è inevitabile se il dato sotto non c'è, cmq nelle nuove versioni dovrebbe sempre esserci)
                                        Importo_Pagato_Corrente = (x_Percentuale_Pagamento * x_Num_Protocollo) / 100
                                        Importo_Pagato_Corrente = ArrotondaVal_2(Importo_Pagato_Corrente)
                                    Else
                                        'gestione importo fisso salvato dall'utente
                                        Importo_Pagato_Corrente = x_Importo_Pagamento
                                    End If
                            End Select

                            '  _2b_LeggiDati_IBAN(DrPagEffettuato(j), Lav_Cod, x_Tipo_Pagamento, Mod_Banche)

                            Tot_Importo_gia_Pagato += Importo_Pagato_Corrente

                            'If Modalita_Pag_Effettuata <> "" Then
                            '    Modalita_Pag_Effettuata &= "; "
                            'End If
                            'Modalita_Pag_Effettuata &= x_Cau_Pagamento_Sigla & " per " & CStr(Importo_Pagato_Corrente) & " €" & Mod_Banche

                            'If x_Note_Pagamento <> "" Then
                            '    Modalita_Pag_Effettuata &= " (" & x_Note_Pagamento & ")"
                            'End If

                        End With

                    Next

                End If 'effettuato

                Tot_Importo_gia_Pagato = ArrotondaVal_2(Tot_Importo_gia_Pagato)

                'Flag_Riscossioni:
                '1 = totalmente da riscuotere+
                '2 = parzialmente da riscuotere
                '3 = riscossa interamente
                Select Case Tot_Importo_gia_Pagato
                    Case 0
                        'Flag_Riscossioni = 1 '  totalmente da riscuotere
                        Flag_NonRiscosso = True
                    Case Is = x_Num_Protocollo
                        'Flag_Riscossioni = 3 'totalmente riscossa
                        Flag_Riscosso = True
                    Case Else
                        ' Flag_Riscossioni = 2 '  parzialmente da riscuotere
                        Flag_ParzialmenteRiscosso = True
                End Select

                Tot_Importo_da_Pagare = x_Num_Protocollo - Tot_Importo_gia_Pagato

                Select Case Lav_Cod
                    Case LAVCOD_NOTA_ACCREDITO_EMESSA, LAVCOD_NOTA_ACCREDITO_RICEVUTA
                        Tot_Importo_da_Pagare = -1 * Tot_Importo_da_Pagare
                        Tot_Importo_gia_Pagato = -1 * Tot_Importo_gia_Pagato
                End Select

                Tot_Importo_da_Pagare = ArrotondaVal_2(Tot_Importo_da_Pagare)


            End If 'dt pagamenti

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' query di lettura per poi update che risolve un giga bug del giaslan nel salvataggio dei dati della partita doppia sui pagamenti dei documenti
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function LeggiXUpdate_TipoCodAvere_PagamentiDoc(ByRef objParametri As AgronicaCoreParametri
                                                           ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Pagamenti_R.LeggiXUpdate_TipoCodAvere_PagamentiDoc"

        Dim messaggioErrore As String = ""
        Dim stbSql As New StringBuilder
        Dim dt As DataTable

        Try

            stbSql.Length = 0

            stbSql.AppendLine(" SELECT lav_cod, des_lib, Agenda.validita_inizio, Pagamenti.*  ")
            stbSql.AppendLine(" FROM Pagamenti  ")
            stbSql.AppendLine(" INNER JOIN Agenda ON Pagamenti.Piva = Agenda.Piva AND Pagamenti.Id_Agenda = Agenda.Id_Agenda  ")
            stbSql.AppendLine(" WHERE Tipo_Cod_Avere = 0 ")
            stbSql.AppendLine(" AND Tipo_Avere = 1 ")
            stbSql.AppendLine(" AND Tipo_Dare = 0 ")
            stbSql.AppendLine(" AND Tipo_Cod_Dare <> 0 ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbSql.ToString, nomeRoutine)
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
    ''' query di lettura per poi update che risolve un problema del giaslan nel salvataggio dei dati della partita doppia sui pagamenti delle note di accredito
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function LeggiXUpdate_PagamentiNoteAccreditoRicevute(ByRef objParametri As AgronicaCoreParametri
                                                                ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Pagamenti_R.LeggiXUpdate_PagamentiNoteAccreditoRicevute"

        Dim messaggioErrore As String = ""
        Dim stbSql As New StringBuilder
        Dim dt As DataTable

        Try

            stbSql.Length = 0

            stbSql.AppendLine(" SELECT * FROM Pagamenti  ")
            stbSql.AppendLine(" WHERE EXISTS ( ")
            stbSql.AppendLine("             SELECT 1 ")
            stbSql.AppendLine("             FROM agenda  ")
            stbSql.AppendLine("             WHERE agenda.id_agenda=pagamenti.id_agenda and agenda.piva=pagamenti.piva ")
            stbSql.AppendLine("             AND Lav_Cod= " & Agro_SQL_SaveNum(LAVCOD_NOTA_ACCREDITO_RICEVUTA) & " ")
            stbSql.AppendLine("             ) ")
            stbSql.AppendLine(" and Previsto_Avvenuto= 1 ")
            stbSql.AppendLine(" and tipo_avere = 1 ")
            stbSql.AppendLine(" and Tipo_Cod_Avere <> 0 ")
            stbSql.AppendLine(" and Cod_Conto_Pat_Avere = " & Agro_SQL_SaveNum(enum_Conti_Patrimoniali.CreditiVersoClienti) & " ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbSql.ToString, nomeRoutine)
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
    ''' query di lettura per poi update che risolve un problema del giaslan nel salvataggio dei dati della partita doppia sui pagamenti delle note di accredito
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function LeggiXUpdate_PagamentiNoteAccreditoEmesse(ByRef objParametri As AgronicaCoreParametri
                                                              ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Pagamenti_R.LeggiXUpdate_PagamentiNoteAccreditoEmesse"

        Dim messaggioErrore As String = ""
        Dim stbSql As New StringBuilder
        Dim dt As DataTable

        Try

            stbSql.Length = 0

            stbSql.AppendLine(" SELECT * FROM Pagamenti  ")
            stbSql.AppendLine(" WHERE EXISTS ( ")
            stbSql.AppendLine("             SELECT 1 ")
            stbSql.AppendLine("             FROM agenda  ")
            stbSql.AppendLine("             WHERE agenda.id_agenda=pagamenti.id_agenda and agenda.piva=pagamenti.piva ")
            stbSql.AppendLine("             AND Lav_Cod= " & Agro_SQL_SaveNum(LAVCOD_NOTA_ACCREDITO_EMESSA) & " ")
            stbSql.AppendLine("             ) ")
            stbSql.AppendLine(" and Previsto_Avvenuto= 1 ")
            stbSql.AppendLine(" and tipo_dare = 1 ")
            stbSql.AppendLine(" and Tipo_Cod_dare <> 0 ")
            stbSql.AppendLine(" and Cod_Conto_Pat_dare = " & Agro_SQL_SaveNum(enum_Conti_Patrimoniali.DebitiVersoFornitori) & " ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbSql.ToString, nomeRoutine)
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
    ''' query di update che risolve un  bug del giaslan nel salvataggio dell'anno del piano dei conti
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function LeggiXUpdate_Anno_PagamentiDoc(ByRef objParametri As AgronicaCoreParametri
                                                   ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Pagamenti_R.LeggiXUpdate_Anno_PagamentiDoc"

        Dim messaggioErrore As String = ""
        Dim stbSql As New StringBuilder
        Dim dt As DataTable

        Try

            stbSql.Length = 0

            stbSql.AppendLine(" SELECT lav_cod, des_lib, Agenda.validita_inizio, Pagamenti.*  ")
            stbSql.AppendLine(" FROM Pagamenti  ")
            stbSql.AppendLine(" INNER JOIN Agenda ON Pagamenti.Piva = Agenda.Piva AND Pagamenti.Id_Agenda = Agenda.Id_Agenda  ")
            stbSql.AppendLine(" WHERE anno <> year(data_pagamento)  ")
            stbSql.AppendLine(" and year(data_pagamento)<>2100 ")
            stbSql.AppendLine(" and year(data_pagamento)<>1900 ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbSql.ToString, nomeRoutine)
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


Public Class Pagamenti_W
    Inherits AgronicaCoreDataProvider.DataProvider


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' query di update che risolve un giga bug del giaslan nel salvataggio dei dati della partita doppia sui pagamenti dei documenti
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function Update_CodContoPatAvere_NoteAccreditoRicevute(ByRef objParametri As AgronicaCoreParametri
                                                                  ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Pagamenti_W.Update_CodContoPatAvere_NoteAccreditoRicevute"

        Dim messaggioErrore As String = ""
        Dim stbSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            stbSql.Length = 0

            'i pagamenti delle note di accredito ricevute dal fornitore per un bug impostavano 
            'il conto crediti v/clienti invece di debiti v/fornitori
            stbSql.AppendLine(" UPDATE Pagamenti  ")
            stbSql.AppendLine(" SET Cod_Conto_Pat_Avere = " & Agro_SQL_SaveNum(enum_Conti_Patrimoniali.DebitiVersoFornitori) & " ")
            stbSql.AppendLine(" WHERE EXISTS ( ")
            stbSql.AppendLine("             SELECT 1 ")
            stbSql.AppendLine("             FROM agenda  ")
            stbSql.AppendLine("             WHERE agenda.id_agenda=pagamenti.id_agenda and agenda.piva=pagamenti.piva ")
            stbSql.AppendLine("             AND Lav_Cod= " & Agro_SQL_SaveNum(LAVCOD_NOTA_ACCREDITO_RICEVUTA) & " ")
            stbSql.AppendLine("             ) ")
            stbSql.AppendLine(" and Previsto_Avvenuto= 1 ")
            stbSql.AppendLine(" and tipo_avere = 1 ")
            stbSql.AppendLine(" and Tipo_Cod_Avere <> 0 ")
            stbSql.AppendLine(" and Cod_Conto_Pat_Avere = " & Agro_SQL_SaveNum(enum_Conti_Patrimoniali.CreditiVersoClienti) & " ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stbSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' query di update che risolve un giga bug del giaslan nel salvataggio dei dati della partita doppia sui pagamenti dei documenti
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function Update_CodContoPatDare_NoteAccreditoEmesse(ByRef objParametri As AgronicaCoreParametri
                                                               ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Pagamenti_W.Update_CodContoPatDare_NoteAccreditoEmesse"

        Dim messaggioErrore As String = ""
        Dim stbSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            stbSql.Length = 0

            'i pagamenti delle note di accredito emesse al cliente per un bug impostavano 
            'il conto debiti v/fornitori invece di crediti v/clienti
            stbSql.AppendLine(" UPDATE Pagamenti  ")
            stbSql.AppendLine(" SET Cod_Conto_Pat_dare = " & Agro_SQL_SaveNum(enum_Conti_Patrimoniali.CreditiVersoClienti) & " ")
            stbSql.AppendLine(" WHERE EXISTS ( ")
            stbSql.AppendLine("             SELECT 1 ")
            stbSql.AppendLine("             FROM agenda  ")
            stbSql.AppendLine("             WHERE agenda.id_agenda=pagamenti.id_agenda and agenda.piva=pagamenti.piva ")
            stbSql.AppendLine("             AND Lav_Cod= " & Agro_SQL_SaveNum(LAVCOD_NOTA_ACCREDITO_EMESSA) & " ")
            stbSql.AppendLine("             ) ")
            stbSql.AppendLine(" and Previsto_Avvenuto= 1 ")
            stbSql.AppendLine(" and tipo_dare = 1 ")
            stbSql.AppendLine(" and Tipo_Cod_dare <> 0 ")
            stbSql.AppendLine(" and Cod_Conto_Pat_dare = " & Agro_SQL_SaveNum(enum_Conti_Patrimoniali.DebitiVersoFornitori) & " ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stbSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' query di update che risolve un giga bug del giaslan nel salvataggio dei dati della partita doppia sui pagamenti dei documenti
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function Update_TipoCodAvere_PagamentiDoc(ByRef objParametri As AgronicaCoreParametri
                                                     ) As Boolean

        Dim nomeRoutine = "AgronicaCoreContabDAL.Pagamenti_W.Update_TipoCodAvere_PagamentiDoc"

        Dim messaggioErrore As String = ""
        Dim stbSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            nomeRoutine &= " Query1 " & vbCrLf

            stbSql.Length = 0

            'query che imposta il cod_risum del contatto della documento
            'da Tipo_Cod_Dare a Tipo_Cod_Avere
            'se doveva essere in Tipo_Cod_Avere poiché Tipo_Avere = 1
            'e non in Tipo_Cod_Dare poiché Tipo_Dare = 0
            stbSql.AppendLine(" UPDATE Pagamenti  ")
            stbSql.AppendLine(" SET Tipo_Cod_Avere = Tipo_Cod_Dare ")
            stbSql.AppendLine(" WHERE Tipo_Cod_Avere = 0 ")
            stbSql.AppendLine(" AND Tipo_Avere = 1 ")
            stbSql.AppendLine(" AND Tipo_Dare = 0 ")
            stbSql.AppendLine(" AND Tipo_Cod_Dare <> 0 ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stbSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            nomeRoutine &= " Query2 " & vbCrLf

            stbSql.Length = 0

            'azzera Tipo_Cod_Dare per pulire il campo, dopo l'update sopra
            stbSql.AppendLine(" UPDATE Pagamenti  ")
            stbSql.AppendLine(" SET Tipo_Cod_Dare = 0 ")
            stbSql.AppendLine(" WHERE Tipo_Cod_Avere <> 0 ")
            stbSql.AppendLine(" AND Tipo_Avere = 1 ")
            stbSql.AppendLine(" AND Tipo_Dare = 0 ")
            stbSql.AppendLine(" AND Tipo_Cod_Dare <> 0 ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stbSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' query di update che risolve un bug del giaslan nel salvataggio del campo anno del piano dei conti
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function Update_Anno_PagamentiDoc(ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Pagamenti_W.Update_Anno_PagamentiDoc"

        Dim messaggioErrore As String = ""
        Dim stbSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            nomeRoutine &= " Query1 " & vbCrLf

            stbSql.Length = 0

            'query che imposta anno = year(data_pagamento)
            stbSql.AppendLine(" UPDATE Pagamenti  ")
            stbSql.AppendLine(" SET anno = YEAR(data_pagamento) ")
            stbSql.AppendLine(" WHERE anno <> year(data_pagamento) ")
            stbSql.AppendLine(" AND YEAR(data_pagamento) <> 2100 ")
            stbSql.AppendLine(" AND YEAR(data_pagamento) <> 1900 ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stbSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            nomeRoutine &= " Query2 " & vbCrLf

            stbSql.Length = 0

            'azzera Anno per 1900 e 2100
            stbSql.AppendLine(" UPDATE Pagamenti  ")
            stbSql.AppendLine(" SET Anno = 0 ")
            stbSql.AppendLine(" WHERE YEAR(data_pagamento) = 2100 ")
            stbSql.AppendLine(" OR YEAR(data_pagamento) = 1900 ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stbSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function Scrivi(ByVal Piva As String,
                           ByVal Sa_Cod As Integer,
                           ByVal Id_Agenda As Integer,
                           ByVal Id_Mov As Integer,
                           ByVal Cod_Pagamento As Integer,
                           ByVal Importo As Decimal,
                           ByVal Percentuale As Decimal,
                           ByVal Data_Pagamento As Date,
                           ByVal Note As String,
                           ByVal Cau_Risorsa As String,
                           ByVal Cod_Liquidita_Dare As Integer,
                           ByVal Cod_Liquidita_Avere As Integer,
                           ByVal Cau_Pagamento As Integer,
                           ByVal Extra_Str As String,
                           ByVal Extra_Int As Integer,
                           ByVal Extra_Date As Date,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = "",
                           Optional ByVal Cod_Conto_Dare As Integer = 0,
                           Optional ByVal Cod_Conto_Avere As Integer = 0,
                           Optional ByVal Cod_Conto_Pat_Dare As Integer = 0,
                           Optional ByVal Cod_Conto_Pat_Avere As Integer = 0,
                           Optional ByVal Tipo_Dare As Integer = 0,
                           Optional ByVal Tipo_Avere As Integer = 0,
                           Optional ByVal Tipo_Cod_Dare As Integer = 0,
                           Optional ByVal Tipo_Cod_Avere As Integer = 0,
                           Optional ByVal Anno As Integer = 0,
                           Optional ByVal Ric_Cod As Integer = 0,
                           Optional ByVal Ric_Cod_Pat As Integer = 0,
                           Optional ByVal Previsto_Avvenuto As Integer = 0,
                           Optional ByVal cbi_causale As Integer = 0,
                           Optional ByVal ChkDataScadenza_Manuale As Integer = 0,
                           Optional ByVal DataScadenza_Manuale As Date = AGRODATAFINE
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Pagamenti_W.Scrivi()"

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

            strSql.AppendLine(" INSERT INTO Pagamenti ")
            strSql.AppendLine("         ( ")
            strSql.AppendLine("          Piva,          Sa_Cod,             Id_Agenda,            Id_Mov,      Cod_Pagamento,")
            strSql.AppendLine("          Importo,       Percentuale,        Data_Pagamento, Note,")
            strSql.AppendLine("          Cau_Risorsa,   Cod_Liquidita_Dare, Cod_Liquidita_Avere,")
            strSql.AppendLine("          Cau_Pagamento, Extra_Str,          Extra_Int,            Extra_Date,")

            strSql.AppendLine("          Inviato,            DataInvio, ")
            strSql.AppendLine("          Data_Creazione,     Data_Modifica, ")
            strSql.AppendLine("          UserName_Creazione, UserName_Modifica, ")
            strSql.AppendLine("          Validita_Inizio,    Validita_Fine, ")
            strSql.AppendLine("          Cod_Conto_Dare,     Cod_Conto_Avere, ")
            strSql.AppendLine("          Cod_Conto_Pat_Dare, Cod_Conto_Pat_Avere, ")
            strSql.AppendLine("          Tipo_Dare,          Tipo_Avere, ")
            strSql.AppendLine("          Tipo_Cod_Dare,      Tipo_Cod_Avere, ")
            strSql.AppendLine("          Anno, Ric_Cod,      Ric_Cod_Pat,   Previsto_Avvenuto,     cbi_causale, ")
            strSql.AppendLine("          ChkDataScadenza_Manuale,           DataScadenza_Manuale ")
            strSql.AppendLine("         ) ")

            strSql.AppendLine(" VALUES ( ")
            strSql.AppendLine("          '" & Agro_SQL_SaveText(Piva) & "'  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Id_Agenda) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Id_Mov) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Cod_Pagamento) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Importo) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Percentuale) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Data_Pagamento) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Note) & "'  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(UCase(Cau_Risorsa)) & "'  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Cod_Liquidita_Dare) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Cod_Liquidita_Avere) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Cau_Pagamento) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Extra_Str) & "'  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Extra_Int) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Extra_Date) & "  ")

            strSql.AppendLine("         , 0  ")
            strSql.AppendLine("         , Null  ")

            strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            strSql.AppendLine("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            strSql.AppendLine("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")



            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Cod_Conto_Dare) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Cod_Conto_Avere) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Cod_Conto_Pat_Dare) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Cod_Conto_Pat_Avere) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Tipo_Dare) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Tipo_Avere) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Tipo_Cod_Dare) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Tipo_Cod_Avere) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Anno) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Ric_Cod) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Ric_Cod_Pat) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Previsto_Avvenuto) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(cbi_causale) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(ChkDataScadenza_Manuale) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(DataScadenza_Manuale) & "  ")

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

    Public Function Modifica(ByRef objParametri As AgronicaCoreParametri,
                             ByVal pk_piva As String,
                             ByVal pk_saCod As Integer,
                             ByVal pk_idAgenda As Integer,
                             ByVal pk_idMov As Integer,
                             ByVal pk_codPagamento As Integer,
                             Optional ByVal Data_modifica As Date? = Nothing,
                             Optional ByVal username_modifica As String = Nothing,
                             Optional ByVal Importo As Decimal? = Nothing,
                             Optional ByVal Percentuale As Decimal? = Nothing,
                             Optional ByVal Data_Pagamento As Date? = Nothing,
                             Optional ByVal Note As String = Nothing,
                             Optional ByVal Cau_Risorsa As String = Nothing,
                             Optional ByVal Cod_Liquidita_Dare As Integer? = Nothing,
                             Optional ByVal Cod_Liquidita_Avere As Integer? = Nothing,
                             Optional ByVal Cau_Pagamento As Integer? = Nothing,
                             Optional ByVal Extra_Str As String = Nothing,
                             Optional ByVal Extra_Int As Integer? = Nothing,
                             Optional ByVal Extra_Date As Date? = Nothing,
                             Optional ByVal Validita_Inizio As Date? = Nothing,
                             Optional ByVal Validita_Fine As Date? = Nothing,
                             Optional ByVal Cod_Conto_Dare As Integer? = Nothing,
                             Optional ByVal Cod_Conto_Avere As Integer? = Nothing,
                             Optional ByVal Cod_Conto_Pat_Dare As Integer? = Nothing,
                             Optional ByVal Cod_Conto_Pat_Avere As Integer? = Nothing,
                             Optional ByVal Tipo_Dare As Integer? = Nothing,
                             Optional ByVal Tipo_Avere As Integer? = Nothing,
                             Optional ByVal Tipo_Cod_Dare As Integer? = Nothing,
                             Optional ByVal Tipo_Cod_Avere As Integer? = Nothing,
                             Optional ByVal Anno As Integer? = Nothing,
                             Optional ByVal Ric_Cod As Integer? = Nothing,
                             Optional ByVal Ric_Cod_Pat As Integer? = Nothing,
                             Optional ByVal Previsto_Avvenuto As Integer? = Nothing,
                             Optional ByVal cbi_causale As Integer? = Nothing,
                             Optional ByVal ChkDataScadenza_Manuale As Integer? = Nothing,
                             Optional ByVal DataScadenza_Manuale As Date? = Nothing) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Pagamenti_W.Modifica()"

        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            strSql.Length = 0

            If username_modifica Is Nothing Then
                username_modifica = objParametri.UsernameOperazione
            End If

            If Not Data_modifica.HasValue Then
                Data_modifica = Now
            End If

            strSql.AppendLine(" UPDATE Pagamenti ")
            strSql.AppendLine(" SET ")
            strSql.AppendLine(" Username_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "' ")
            strSql.AppendLine(" ,Data_Modifica = " & Agro_SQL_SaveDateTime(Data_modifica) & " ")

            If Importo.HasValue Then
                strSql.AppendLine(" ,Importo = " & Agro_SQL_SaveNum(Importo))
            End If
            If Percentuale.HasValue Then
                strSql.AppendLine(" ,Percentuale = " & Agro_SQL_SaveNum(Percentuale))
            End If
            If Data_Pagamento.HasValue Then
                strSql.AppendLine(" ,Data_Pagamento = " & Agro_SQL_SaveDate(Data_Pagamento))
            End If
            If Note IsNot Nothing Then
                strSql.AppendLine(" ,Note = '" & Agro_SQL_SaveText(Note) & "'")
            End If
            If Cau_Risorsa IsNot Nothing Then
                strSql.AppendLine(" ,Cau_Risorsa = '" & Agro_SQL_SaveText(Cau_Risorsa) & "'")
            End If
            If Cod_Liquidita_Dare.HasValue Then
                strSql.AppendLine(" ,Cod_Liquidita_Dare = " & Agro_SQL_SaveNum(Cod_Liquidita_Dare))
            End If
            If Cod_Liquidita_Avere.HasValue Then
                strSql.AppendLine(" ,Cod_Liquidita_Avere = " & Agro_SQL_SaveNum(Cod_Liquidita_Avere))
            End If
            If Cau_Pagamento.HasValue Then
                strSql.AppendLine(" ,Cau_Pagamento = " & Agro_SQL_SaveNum(Cau_Pagamento))
            End If
            If Extra_Str IsNot Nothing Then
                strSql.AppendLine(" ,Extra_Str = '" & Agro_SQL_SaveText(Extra_Str) & "'")
            End If
            If Extra_Int.HasValue Then
                strSql.AppendLine(" ,Extra_Int = " & Agro_SQL_SaveNum(Extra_Int))
            End If
            If Extra_Date.HasValue Then
                strSql.AppendLine(" ,Extra_Date = " & Agro_SQL_SaveDate(Extra_Date))
            End If
            If Validita_Inizio.HasValue Then
                strSql.AppendLine(" ,Validita_Inizio = " & Agro_SQL_SaveDate(Validita_Inizio))
            End If
            If Validita_Fine.HasValue Then
                strSql.AppendLine(" ,Validita_Fine = " & Agro_SQL_SaveDate(Validita_Fine))
            End If
            If Cod_Conto_Dare.HasValue Then
                strSql.AppendLine(" ,Cod_Conto_Dare = " & Agro_SQL_SaveNum(Cod_Conto_Dare))
            End If
            If Cod_Conto_Avere.HasValue Then
                strSql.AppendLine(" ,Cod_Conto_Avere = " & Agro_SQL_SaveNum(Cod_Conto_Avere))
            End If
            If Cod_Conto_Pat_Dare.HasValue Then
                strSql.AppendLine(" ,Cod_Conto_Pat_Dare = " & Agro_SQL_SaveNum(Cod_Conto_Pat_Dare))
            End If
            If Cod_Conto_Pat_Avere.HasValue Then
                strSql.AppendLine(" ,Cod_Conto_Pat_Avere = " & Agro_SQL_SaveNum(Cod_Conto_Pat_Avere))
            End If
            If Tipo_Dare.HasValue Then
                strSql.AppendLine(" ,Tipo_Dare = " & Agro_SQL_SaveNum(Tipo_Dare))
            End If
            If Tipo_Avere.HasValue Then
                strSql.AppendLine(" ,Tipo_Avere = " & Agro_SQL_SaveNum(Tipo_Avere))
            End If
            If Tipo_Cod_Dare.HasValue Then
                strSql.AppendLine(" ,Tipo_Cod_Dare = " & Agro_SQL_SaveNum(Tipo_Cod_Dare))
            End If
            If Tipo_Cod_Avere.HasValue Then
                strSql.AppendLine(" ,Tipo_Cod_Avere = " & Agro_SQL_SaveNum(Tipo_Cod_Avere))
            End If
            If Anno.HasValue Then
                strSql.AppendLine(" ,Anno = " & Agro_SQL_SaveNum(Anno))
            End If
            If Ric_Cod.HasValue Then
                strSql.AppendLine(" ,Ric_Cod = " & Agro_SQL_SaveNum(Ric_Cod))
            End If
            If Ric_Cod_Pat.HasValue Then
                strSql.AppendLine(" ,Ric_Cod_Pat = " & Agro_SQL_SaveNum(Ric_Cod_Pat))
            End If
            If Previsto_Avvenuto.HasValue Then
                strSql.AppendLine(" ,Previsto_Avvenuto = " & Agro_SQL_SaveNum(Previsto_Avvenuto))
            End If
            If cbi_causale.HasValue Then
                strSql.AppendLine(" ,cbi_causale = " & Agro_SQL_SaveNum(cbi_causale))
            End If
            If ChkDataScadenza_Manuale.HasValue Then
                strSql.AppendLine(" ,ChkDataScadenza_Manuale = " & Agro_SQL_SaveNum(ChkDataScadenza_Manuale))
            End If
            If DataScadenza_Manuale.HasValue Then
                strSql.AppendLine(" ,DataScadenza_Manuale = " & Agro_SQL_SaveDate(DataScadenza_Manuale))
            End If

            strSql.AppendLine("")
            strSql.AppendLine(" WHERE piva = '" & Agro_SQL_SaveText(pk_piva) & "' ")
            strSql.AppendLine(" AND sa_cod = " & Agro_SQL_SaveNum(pk_saCod))
            strSql.AppendLine(" AND id_agenda = " & Agro_SQL_SaveNum(pk_idAgenda))
            strSql.AppendLine(" AND id_mov = " & Agro_SQL_SaveNum(pk_idMov))
            strSql.AppendLine(" AND cod_pagamento = " & Agro_SQL_SaveNum(pk_codPagamento))

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return xRisp

    End Function

    Public Function Cancella(ByVal Piva As String,
                             ByVal Sa_Cod As Integer,
                             ByVal Id_Agenda As Integer,
                             ByVal Id_Mov As Integer,
                             ByVal Cod_Pagamento As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Pagamenti_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""
        '   Sa_Cod = 0
        '   Id_Agenda = 0
        '   Id_Mov = 0
        '   Cod_Pagamento = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.Length = 0
                strSql.AppendLine(" UPDATE Pagamenti ")
                strSql.AppendLine(" SET ")
                strSql.AppendLine("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.AppendLine("      ,Inviato = -1 ")
                strSql.AppendLine(" WHERE  Inviato >= 0 ")

            Else
                strSql.Length = 0
                strSql.AppendLine(" DELETE ")
                strSql.AppendLine(" FROM Pagamenti ")
                strSql.AppendLine(" WHERE  1=1 ")
            End If

            If Piva <> String.Empty Then
                strSql.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
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

            If Cod_Pagamento <> 0 Then
                strSql.AppendLine(" AND Cod_Pagamento = " & Agro_SQL_SaveNum(Cod_Pagamento) & "   ")
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

    Public Function CBI_RiBa_EliminaMarcaInFaseDiInvio(ByVal Pagamento_Cod As Integer,
                                                       ByRef objparametri As AgronicaCoreParametri
                                                       ) As Boolean


        Const nomeRoutine = "AgronicaCoreContabDAL.Pagamenti_W.CBI_RiBa_EliminaMarcaInFaseDiInvio()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""
        '   Sa_Cod = 0
        '   Id_Agenda = 0
        '   Id_Mov = 0
        '   Cod_Pagamento = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try


            strSql.Length = 0
            strSql.AppendLine(" UPDATE Pagamenti ")
            strSql.AppendLine(" SET ")
            strSql.AppendLine("      cbi_Causale = 0 ")
            strSql.AppendLine("      , Username_Modifica = '" & Agro_SQL_SaveText(objparametri.UsernameOperazione) & "' ")
            strSql.AppendLine("      , Data_Modifica = " & Agro_SQL_SaveDateTime(Now.Date) & "")

            strSql.AppendLine(" WHERE  cod_pagamento = " & Agro_SQL_SaveNum(Pagamento_Cod))

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objparametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objparametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function CBI_RiBa_MarcaInFaseDiInvio(ByVal Pagamento_Cod As Integer,
                                                ByRef objparametri As AgronicaCoreParametri
                                                ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Pagamenti_W.CBI_RiBa_MarcaInFaseDiInvio()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""
        '   Sa_Cod = 0
        '   Id_Agenda = 0
        '   Id_Mov = 0
        '   Cod_Pagamento = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            strSql.Length = 0
            strSql.AppendLine(" UPDATE Pagamenti ")
            strSql.AppendLine(" SET ")
            strSql.AppendLine("      cbi_Causale = 30000 ")
            strSql.AppendLine("      , Username_Modifica = '" & Agro_SQL_SaveText(objparametri.UsernameOperazione) & "' ")
            strSql.AppendLine("      , Data_Modifica = " & Agro_SQL_SaveDateTime(Now.Date) & "")

            strSql.AppendLine(" WHERE  cod_pagamento = " & Agro_SQL_SaveNum(Pagamento_Cod))

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objparametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objparametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function CBI_RiBa_MarcaInsoluto(ByVal Pagamento_Cod As Integer,
                                           ByVal Scadenza As Date,
                                           ByVal CausaleInsoluto As Integer,
                                           ByRef objparametri As AgronicaCoreParametri
                                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Pagamenti_W.CBI_RiBa_MarcaInsoluto()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""
        '   Sa_Cod = 0
        '   Id_Agenda = 0
        '   Id_Mov = 0
        '   Cod_Pagamento = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        strSql.AppendLine(" Select p1.Cod_Pagamento ")
        strSql.AppendLine(" from pagamenti p ")
        strSql.AppendLine("    inner join Movimenti mov ")
        strSql.AppendLine("        on p.Piva = mov.PIVA  ")
        strSql.AppendLine("        and p.Id_Agenda = mov.Id_Agenda  ")
        strSql.AppendLine("        and p.Id_Mov = mov.Id_Mov  ")
        strSql.AppendLine("    inner join Pagamenti p1 ")
        strSql.AppendLine("        on p1.piva = mov.piva  ")
        strSql.AppendLine("        and p1.Id_Agenda = mov.Id_Agenda  ")
        strSql.AppendLine("        and p1.Id_Mov = mov.Id_Mov       ")
        strSql.AppendLine("        and p1.Previsto_Avvenuto = 1 ")
        strSql.AppendLine("    inner join movimenti mov1 ")
        strSql.AppendLine("        on  mov1.Piva = p1.PIVA  ")
        strSql.AppendLine("        and mov1.Id_Agenda = p1.Id_Agenda  ")
        strSql.AppendLine("        and mov1.Id_Mov = p1.Id_Mov ")
        strSql.AppendLine("        and mov1.Scadenza = " & Agro_SQL_SaveDate(Scadenza) & " ")
        strSql.AppendLine(" where p.Cod_Pagamento =  " & Pagamento_Cod)

        Dim xApp As DataTable
        xApp = EseguiQuery_Lettura(objparametri, strSql.ToString, "xApp")

        Dim pagamentoCodAnnulla As Integer = -1
        If xApp.Rows.Count <> 1 Then
            Throw New Exception("Errore durante il recupero del codice cod_pagamento da annullare")
        End If

        pagamentoCodAnnulla = xApp.Rows(0)("Cod_Pagamento")

        Try

            strSql.Length = 0
            strSql.AppendLine(" UPDATE Pagamenti ")
            strSql.AppendLine(" SET ")
            strSql.AppendLine("       cbi_Causale = " & Agro_SQL_SaveNum(CausaleInsoluto) & " ")
            strSql.AppendLine("     , Username_Modifica = '" & Agro_SQL_SaveText(objparametri.UsernameOperazione) & "' ")
            strSql.AppendLine("     , Inviato = -1 ")
            strSql.Appendline(" WHERE  cod_pagamento = " & Agro_SQL_SaveNum(pagamentoCodAnnulla))

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objparametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objparametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

End Class
