Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDTOStd.Identity
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO

Public Class DW_CDG_Costi_Ricavi_DAL_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '################################################################################
    Public Function Leggi_Tabellone_DW(ByVal _aziende As String,
                                        ByVal preset As Integer,
                                        ByVal budget As Integer,
                                        ByVal costi_ricavi As Integer,
                                        ByVal dataDal As String,
                                        ByVal dataAl As String,
                                        ByVal dataRifDistinta As String,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal filtro_codice_appezzamento As String,
                                        ByVal filtro_codice_impianto As String,
                                        ByVal filtro_azienda_padre As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        Optional ByVal bIncludiFiltroVisibilitaImprese As Boolean = False
                                        ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.DW_CDG_Costi_Ricavi_DAL_R.Leggi_Globale()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable
        Dim dt_aziende_padre As DataTable
        Dim filtro_piva_padre As String = ""
        Dim dataAlPlus1 As String = String.Empty
        If Not String.IsNullOrEmpty(dataAl) Then
            dataAlPlus1 = Convert.ToDateTime(dataAl).AddDays(1).AddMilliseconds(-1).ToString("dd/MM/yyyy HH:mm:ss.fff")
        End If

        Try

            ' Se richiesto filtro per azienda padre cerco le aziende figlie in gerarchia
            strSql.Length = 0
            If Not String.IsNullOrEmpty(filtro_azienda_padre) Then
                'Comprendo anche l'azienda padre
                filtro_piva_padre = "'" + filtro_azienda_padre + "'"

                strSql.AppendLine(" select figlio from GerarchiaImprese where padre = '" & filtro_azienda_padre & "'")
                '-------------------------------------------------------------------------------
                dt_aziende_padre = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
                '-------------------------------------------------------------------------------

                If Not dt_aziende_padre Is Nothing And dt_aziende_padre.Rows.Count > 0 Then
                    For Each r In dt_aziende_padre.Rows
                        filtro_piva_padre &= ",'" & r.Item("figlio") & "'"
                    Next
                End If
            End If

            strSql.Length = 0
            strSql.Append(" SELECT " & vbCrLf)
            strSql.Append(" DW_CDG_Costi_Ricavi.Id_DW, " & vbCrLf)
            strSql.Append(" DW_CDG_Costi_Ricavi.Piva_Superuser, DW_CDG_Costi_Ricavi.Piva, " & vbCrLf)
            strSql.Append(" (SELECT CASE WHEN ISNULL(partitaIvaReale, '') = '' THEN PIVA ELSE partitaIvaReale END FROM Imprese WHERE Piva = DW_CDG_Costi_Ricavi.Piva) PivaReale , " & vbCrLf)
            strSql.Append(" DW_CDG_Costi_Ricavi.Id_Cfg_DW, DW_CDG_Costi_Ricavi.Id_CDG, " & vbCrLf)
            strSql.Append(" DW_CDG_Costi_Ricavi.Id_CDG_Dettagli, DW_CDG_Costi_Ricavi.Criterio_Analisi, DW_CDG_Costi_Ricavi.Data_Inserimento, DW_CDG_Costi_Ricavi.Descr_Modalita_Imputazione, " & vbCrLf)
            strSql.Append(" DW_CDG_Costi_Ricavi.Id_Agenda, DW_CDG_Costi_Ricavi.Id_Mov, DW_CDG_Costi_Ricavi.Id_Mov_Det, DW_CDG_Costi_Ricavi.Mac_Cod, " & vbCrLf)
            strSql.Append(" DW_CDG_Costi_Ricavi.Cod_RisUm, DW_CDG_Costi_Ricavi.Elem_Cod, DW_CDG_Costi_Ricavi.Pro_Cod, DW_CDG_Costi_Ricavi.Mat_Cod, " & vbCrLf)
            strSql.Append(" DW_CDG_Costi_Ricavi.Udm_Cod, DW_CDG_Costi_Ricavi.Id_Attivita, DW_CDG_Costi_Ricavi.Qualifica_Cod, DW_CDG_Costi_Ricavi.Tariffa_Cod, " & vbCrLf)
            strSql.Append(" DW_CDG_Costi_Ricavi.Turno_Cod, DW_CDG_Costi_Ricavi.Conto_Cod, DW_CDG_Costi_Ricavi.Lotto, DW_CDG_Costi_Ricavi.Mezzo, " & vbCrLf)
            strSql.Append(" DW_CDG_Costi_Ricavi.Mezzo_Des, DW_CDG_Costi_Ricavi.Prezzo_Unitario, DW_CDG_Costi_Ricavi.Qta, " & vbCrLf)
            If costi_ricavi = -1 Then
                strSql.Append(" CASE WHEN DW_CDG_Costi_Ricavi.Costi_Ricavi = 0 THEN -DW_CDG_Costi_Ricavi.Valore ELSE DW_CDG_Costi_Ricavi.Valore END AS Valore, " & vbCrLf)
            Else
                strSql.Append(" DW_CDG_Costi_Ricavi.Valore, " & vbCrLf)
            End If
            strSql.Append(" DW_CDG_Costi_Ricavi.Percentuale_Ripart, DW_CDG_Costi_Ricavi.Descrizione, DW_CDG_Costi_Ricavi.Budget_Cons, DW_CDG_Costi_Ricavi.Budget_Cons_Des, " & vbCrLf)
            strSql.Append(" DW_CDG_Costi_Ricavi.Costi_Ricavi, DW_CDG_Costi_Ricavi.Costi_Ricavi_Des, DW_CDG_Costi_Ricavi.Sa_Cod, DW_CDG_Costi_Ricavi.Appezza, " & vbCrLf)
            strSql.Append(" DW_CDG_Costi_Ricavi.Id_Reg, DW_CDG_Costi_Ricavi.Id_Cod_reg_impianti_codici, DW_CDG_Costi_Ricavi.Id_Imputazione, DW_CDG_Costi_Ricavi.Tipo_Imputazione, " & vbCrLf)
            strSql.Append(" DW_CDG_Costi_Ricavi.Macchine_Cod, DW_CDG_Costi_Ricavi.Linea_Cod, DW_CDG_Costi_Ricavi.Veg_Cod, DW_CDG_Costi_Ricavi.Cul_Cod, " & vbCrLf)
            strSql.Append(" DW_CDG_Costi_Ricavi.Descrizione_Operazione, DW_CDG_Costi_Ricavi.Descrizione_Macchina, DW_CDG_Costi_Ricavi.Proprietario_Macchina, DW_CDG_Costi_Ricavi.Nome_Cognome, " & vbCrLf)
            strSql.Append(" DW_CDG_Costi_Ricavi.NrBadge, DW_CDG_Costi_Ricavi.Categoria, DW_CDG_Costi_Ricavi.Descrizione_Prodotto, DW_CDG_Costi_Ricavi.Attivita, " & vbCrLf)
            strSql.Append(" DW_CDG_Costi_Ricavi.Qualifica, DW_CDG_Costi_Ricavi.Tariffa, DW_CDG_Costi_Ricavi.Turno, DW_CDG_Costi_Ricavi.Conto, " & vbCrLf)
            strSql.Append(" DW_CDG_Costi_Ricavi.Ragione_Sociale_Azienda, DW_CDG_Costi_Ricavi.Indirizzo_Azienda, DW_CDG_Costi_Ricavi.Cap_Azienda, DW_CDG_Costi_Ricavi.Localita_Azienda, " & vbCrLf)
            strSql.Append(" DW_CDG_Costi_Ricavi.Prov_Azienda, DW_CDG_Costi_Ricavi.Descrizione_Centro, DW_CDG_Costi_Ricavi.Indirizzo_Centro, DW_CDG_Costi_Ricavi.Cap_Centro, " & vbCrLf)
            strSql.Append(" DW_CDG_Costi_Ricavi.Localita_Centro, DW_CDG_Costi_Ricavi.Prov_Centro, DW_CDG_Costi_Ricavi.Descrizione_Campo, DW_CDG_Costi_Ricavi.Nome_Appezzamento, " & vbCrLf)
            strSql.Append(" DW_CDG_Costi_Ricavi.Inizio_Appezzamento, DW_CDG_Costi_Ricavi.Fine_Appezzamento, DW_CDG_Costi_Ricavi.Descrizione_Gruppo_Vegetale, DW_CDG_Costi_Ricavi.Specie_impianto, " & vbCrLf)
            strSql.Append(" DW_CDG_Costi_Ricavi.Varieta_impianto, DW_CDG_Costi_Ricavi.Descrizione_Finalita, DW_CDG_Costi_Ricavi.Destinazione_Uso, DW_CDG_Costi_Ricavi.Inizio_Impianto, " & vbCrLf)
            strSql.Append(" DW_CDG_Costi_Ricavi.Fine_Impianto, DW_CDG_Costi_Ricavi.Codice_Impianto, DW_CDG_Costi_Ricavi.Progetto_Nome, DW_CDG_Costi_Ricavi.Regolamento, " & vbCrLf)
            strSql.Append(" DW_CDG_Costi_Ricavi.Disciplinare, DW_CDG_Costi_Ricavi.Esposizione_Appezzamento, DW_CDG_Costi_Ricavi.Ubicazione_Appezzamento, DW_CDG_Costi_Ricavi.Descrizione_Portinnesti, " & vbCrLf)
            strSql.Append(" DW_CDG_Costi_Ricavi.Descrizione_Irrigazioni, DW_CDG_Costi_Ricavi.Regione_Impianto, DW_CDG_Costi_Ricavi.Prov_Impianto, DW_CDG_Costi_Ricavi.Comune_Impianto, " & vbCrLf)
            strSql.Append(" DW_CDG_Costi_Ricavi.Sezione, DW_CDG_Costi_Ricavi.Foglio, DW_CDG_Costi_Ricavi.Numero, DW_CDG_Costi_Ricavi.Subalterno, " & vbCrLf)
            strSql.Append(" DW_CDG_Costi_Ricavi.Descrizione_Macchina_Input_Costi, DW_CDG_Costi_Ricavi.Progetto, DW_CDG_Costi_Ricavi.Classe_Progetto, DW_CDG_Costi_Ricavi.Tipo_Progetto, " & vbCrLf)
            strSql.Append(" DW_CDG_Costi_Ricavi.Linea_Produzione, DW_CDG_Costi_Ricavi.Campo_Cod, DW_CDG_Costi_Ricavi.Lotto_Input_Costi, DW_CDG_Costi_Ricavi.Progetto_Cod, " & vbCrLf)
            strSql.Append(" DW_CDG_Costi_Ricavi.Progetto_Des, DW_CDG_Costi_Ricavi.Progetto_Validita_Inizio, DW_CDG_Costi_Ricavi.Progetto_Validita_Fine, DW_CDG_Costi_Ricavi.Note, " & vbCrLf)
            strSql.Append(" DW_CDG_Costi_Ricavi.Vecchio_Tipo_Inser_Dati, DW_CDG_Costi_Ricavi.inviato, DW_CDG_Costi_Ricavi.datainvio, DW_CDG_Costi_Ricavi.Data_Creazione, " & vbCrLf)
            strSql.Append(" DW_CDG_Costi_Ricavi.Data_Modifica, DW_CDG_Costi_Ricavi.Username_Creazione, DW_CDG_Costi_Ricavi.Username_Modifica, DW_CDG_Costi_Ricavi.Validita_Inizio, " & vbCrLf)
            strSql.Append(" DW_CDG_Costi_Ricavi.Validita_Fine, DW_CDG_Costi_Ricavi.Cod_Animale, DW_CDG_Costi_Ricavi.Animale_Progetto, DW_CDG_Costi_Ricavi.Cod_Animale_Distinta, " & vbCrLf)
            strSql.Append(" DW_CDG_Costi_Ricavi.Animale_Distinta, DW_CDG_Costi_Ricavi.Sta_Num, DW_CDG_Costi_Ricavi.Stalla_Des, DW_CDG_Costi_Ricavi.Raggruppamento_Cod, " & vbCrLf)
            strSql.Append(" DW_CDG_Costi_Ricavi.Raggruppamento_Des, DW_CDG_Costi_Ricavi.Specie_Animale_Cod, DW_CDG_Costi_Ricavi.Specie_Animale_Des, DW_CDG_Costi_Ricavi.Razza_Animale_Cod, " & vbCrLf)
            strSql.Append(" DW_CDG_Costi_Ricavi.Razza_Animale_Des, DW_CDG_Costi_Ricavi.Tipo_Animale_Cod, DW_CDG_Costi_Ricavi.Tipo_Animale_Des, DW_CDG_Costi_Ricavi.Modalita_Imputazione, " & vbCrLf)
            strSql.Append(" DW_CDG_Costi_Ricavi.Cod_Articolo, DW_CDG_Costi_Ricavi.MetodoProduzioneAppezzamento, DW_CDG_Costi_Ricavi.Sup_app, DW_CDG_Costi_Ricavi.Sup_imp, " & vbCrLf)
            strSql.Append(" DW_CDG_Costi_Ricavi.Sup_Prog, DW_CDG_Costi_Ricavi.Data_Chiusura_Esercizio, DW_CDG_Costi_Ricavi.Codice_Appezzamento, DW_CDG_Costi_Ricavi.App_BIO, " & vbCrLf)
            strSql.Append(" DW_CDG_Costi_Ricavi.Data_Split, DW_CDG_Costi_Ricavi.ID_Attivita_Gruppo1, DW_CDG_Costi_Ricavi.Des_Attivita_Gruppo1, DW_CDG_Costi_Ricavi.ID_Attivita_Gruppo2, " & vbCrLf)
            strSql.Append(" DW_CDG_Costi_Ricavi.Des_Attivita_Gruppo2, DW_CDG_Costi_Ricavi.ID_Attivita_Gruppo3, DW_CDG_Costi_Ricavi.Des_Attivita_Gruppo3, DW_CDG_Costi_Ricavi.Ordine_Attivita, " & vbCrLf)
            strSql.Append(" DW_CDG_Costi_Ricavi.Piva_Padre, DW_CDG_Costi_Ricavi.Azienda_Padre, DW_CDG_Costi_Ricavi.Id_Agenda_Qdc, DW_CDG_Costi_Ricavi.Fabbricato_Des, " & vbCrLf)
            strSql.Append(" DW_CDG_Costi_Ricavi.PesoPagato, DW_CDG_Costi_Ricavi.PesoArrivo, DW_CDG_Costi_Ricavi.PesoUscito, " & vbCrLf)
            strSql.Append(" CASE WHEN DW_CDG_Costi_Ricavi.Destinazione_Uso <> '' THEN 'Sì' ELSE '' END AS Terreno_Nudo " & vbCrLf)
            strSql.Append(" FROM DW_CDG_Costi_Ricavi " & vbCrLf)

            strSql.Append(" WHERE 1 = 1  ")

            If _aziende <> "" Then
                strSql.Append(" AND DW_CDG_Costi_Ricavi.Piva In (" & Agro_SQL_Save_Clausola_IN("'" & Replace(_aziende, "|", "','") & "'", True) & ") ")
            End If
            ' Se budget = 0 significa che si cerca il consuntivo, diversamente una specifica versione di budget
            strSql.Append(" AND DW_CDG_Costi_Ricavi.Budget_Cons =  " & Agro_SQL_SaveNum(budget) & " ")
            If costi_ricavi <> -1 Then
                strSql.Append(" AND DW_CDG_Costi_Ricavi.Costi_Ricavi =  " & Agro_SQL_SaveNum(costi_ricavi) & " ")
            End If
            If Not String.IsNullOrEmpty(dataDal) Then
                strSql.Append(" AND DW_CDG_Costi_Ricavi.Data_Inserimento >= " & Agro_SQL_SaveDate(dataDal))
            End If
            If Not String.IsNullOrEmpty(dataAl) Then
                strSql.Append(" AND DW_CDG_Costi_Ricavi.Data_Inserimento <= " & Agro_SQL_SaveDateTime(dataAlPlus1))
            End If
            If Not String.IsNullOrEmpty(dataRifDistinta) Then
                strSql.Append(" AND DW_CDG_Costi_Ricavi.Progetto_Validita_Inizio <= " & Agro_SQL_SaveDate(dataRifDistinta))
                strSql.Append(" AND DW_CDG_Costi_Ricavi.Progetto_Validita_Fine >= " & Agro_SQL_SaveDate(dataRifDistinta))
            End If

            If Not String.IsNullOrEmpty(filtro_codice_appezzamento) Then
                'Costruzione stringa con apici
                strSql.AppendLine(" And Upper(DW_CDG_Costi_Ricavi.Nome_Appezzamento) In (" & Agro_SQL_Save_Clausola_IN("'" & Replace(filtro_codice_appezzamento, ",", "','") & "'", True) & ") ")
            End If

            If Not String.IsNullOrEmpty(filtro_codice_impianto) Then
                'Costruzione stringa con apici
                strSql.AppendLine(" And Upper(DW_CDG_Costi_Ricavi.Codice_Impianto) In (" & Agro_SQL_Save_Clausola_IN("'" & Replace(filtro_codice_impianto, ",", "','") & "'", True) & ") ")
            End If

            If Not String.IsNullOrEmpty(filtro_azienda_padre) Then
                'Costruzione stringa con apici
                strSql.AppendLine(" And DW_CDG_Costi_Ricavi.piva In (" & Agro_SQL_Save_Clausola_IN(filtro_piva_padre, True) & ") ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" And " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If


            If bIncludiFiltroVisibilitaImprese Then

                'Filtro Visibilità Imprese
                strSql.AppendLine(" and DW_CDG_Costi_Ricavi.Piva in ( ")

                strSql.AppendLine(" Select Distinct dbo.Imprese.PIVA ")
                strSql.AppendLine(" FROM  (( ")
                strSql.AppendLine(" Imprese INNER JOIN UtentiXImprese On Imprese.Piva = UtentixImprese.Piva) ")
                strSql.AppendLine(" INNER Join ImpresexIndirizzi ON Imprese.PIVA = ImpresexIndirizzi.PIVA)  ")
                strSql.AppendLine(" Where ImpresexIndirizzi.Tipo_Indirizzo = 1 ")
                strSql.AppendLine(" And   UtentixImprese.[User] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
                strSql.AppendLine(" And   Imprese.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                strSql.AppendLine(" And   Imprese.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")


                '----------------------------------------------------------------
                '--- Filtro associato all'utente 
                '----------------------------------------------------------------
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim UtenteProfiloImpreseSql As String = ""
                Dim UtenteProfiloCentriSql As String = ""
                Dim DtImpreseVisibili As DataTable
                Dim i As Integer

                DtImpreseVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Impresa, "", "", objParametri)
                If Not DtImpreseVisibili Is Nothing Then
                    For i = 0 To DtImpreseVisibili.Rows.Count - 1
                        UtenteProfiloImpreseSql &= "'" & DtImpreseVisibili.Rows(i).Item("Piva") & "',"
                    Next
                    If UtenteProfiloImpreseSql <> "" Then
                        UtenteProfiloImpreseSql = " AND DW_CDG_Costi_Ricavi.piva IN (" & Agro_SQL_Save_Clausola_IN(Left(UtenteProfiloImpreseSql, UtenteProfiloImpreseSql.Length - 1), True) & ") "
                    End If

                    strSql.AppendLine(UtenteProfiloImpreseSql)

                End If

                '-------------------------------------------------------------------------



            End If



            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.Append(" ORDER BY DW_CDG_Costi_Ricavi.Data_Inserimento" & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            Dim Ora As Integer
            Dim Minuti As Integer
            Dim parteIntera As Decimal
            Dim parteDecimali As Decimal

            dt.Columns.Add("QtaString", GetType(String))
            For Each r In dt.Rows

                If CInt(r.Item("Cod_Risum")) <> 0 OrElse CInt(r.Item("Mac_Cod") <> 0) Then
                    'Conversione intero minuti a data
                    parteIntera = Math.Truncate(CDec(r.Item("Qta")))
                    parteDecimali = CDec(r.Item("Qta")) - parteIntera
                    Ora = parteIntera
                    Minuti = 60 * parteDecimali
                    r.Item("QtaString") = Ora.ToString("D2") & ":" & Minuti.ToString("D2")
                Else
                    r.Item("QtaString") = CStr(r.Item("Qta"))
                End If
            Next

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("" & nomeRoutine & " : " & messaggioErrore)
        End Try


        Return dt

    End Function



    '################################################################################
    Public Function Leggi_Tabellone_DW_Report(ByVal _aziende As String,
                                              ByVal id_budget As Integer,
                                              ByVal costi_ricavi As Integer,
                                              ByVal dataDal As String,
                                              ByVal dataAl As String,
                                              ByVal dataRifDistinta As String,
                                              ByVal xFiltroAggiuntivo As String,
                                              ByVal filtro_codice_appezzamento As String,
                                              ByVal filtro_codice_impianto As String,
                                              ByVal filtro_azienda_padre As String,
                                              ByVal bBypassDistinteNonPresenti As Boolean,
                                              ByVal xOrderBy As String,
                                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                              ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.DW_CDG_Costi_Ricavi_DAL_R.Leggi_Tabellone_DW_Report()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable
        Dim dt_aziende_padre As DataTable
        Dim filtro_piva_padre As String = ""
        Dim TabellaPrefisso As String = ""

        Try

            ' Se richiesto filtro per azienda padre cerco le aziende figlie in gerarchia
            strSql.Length = 0
            If Not String.IsNullOrEmpty(filtro_azienda_padre) Then
                'Comprendo anche l'azienda padre
                filtro_piva_padre = "'" + filtro_azienda_padre + "'"

                strSql.AppendLine(" select figlio from GerarchiaImprese where padre = '" & filtro_azienda_padre & "' or padre in (select figlio from GerarchiaImprese where padre = '" & filtro_azienda_padre & "')")
                '-------------------------------------------------------------------------------
                dt_aziende_padre = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
                '-------------------------------------------------------------------------------

                If Not dt_aziende_padre Is Nothing AndAlso dt_aziende_padre.Rows.Count > 0 Then
                    For Each r In dt_aziende_padre.Rows
                        filtro_piva_padre &= ",'" & r.Item("figlio") & "'"
                    Next
                End If
            End If

            strSql.Length = 0
            strSql.Append(" SELECT DW_CDG_Costi_Ricavi.*, isnull(Attivita.Frazionabile,0) as Frazionabile, ")
            strSql.Append(" Case When Destinazione_Uso <> '' Then 'Sì' Else '' End As Terreno_Nudo " & vbCrLf)

            Select Case id_budget

                Case 0


                    strSql.Append(" ,isnull(Imprese_Progetti.FlagSecondoRaccolto, 0) As FlagSecondoRaccolto ")
                    strSql.Append(" ,isnull(mov_dettagli_riferimenti.Id_Agenda, 0) as Id_Agenda_Agenda ")
                    strSql.Append(" ,isnull(mov_dettagli_riferimenti.Lav_Cod, 0) as Lav_Cod_Agenda ")
                    strSql.Append(" ,isnull(operazioni.Lav_Des, '') as Lav_Des_Agenda ")
                    strSql.Append(" from DW_CDG_Costi_Ricavi " & vbCrLf)

                    strSql.Append(" Left outer join mov_dettagli_riferimenti on (DW_CDG_Costi_Ricavi.id_agenda = mov_dettagli_riferimenti.id_agenda_rif ) ")
                    strSql.Append(" Left outer join operazioni on (operazioni.Lav_Cod = mov_dettagli_riferimenti.Lav_Cod ) ")
                    strSql.Append(" Left outer join attivita on (DW_CDG_Costi_Ricavi.id_attivita = Attivita.id_attivita ) ")
                    strSql.Append(" Left outer join Imprese_Progetti on (DW_CDG_Costi_Ricavi.Piva = Imprese_Progetti.Piva And DW_CDG_Costi_Ricavi.Progetto_Cod = Imprese_Progetti.Progetto_Cod ) ")

                Case Else

                    TabellaPrefisso = "Budget_"

                    strSql.Append(" ,0 as Id_Agenda_Agenda ")
                    strSql.Append(" ,0 as Lav_Cod_Agenda ")
                    strSql.Append(" ,'' as Lav_Des_Agenda ")
                    strSql.Append(" ,0 as FlagSecondoRaccolto ")
                    strSql.Append(" from DW_CDG_Costi_Ricavi " & vbCrLf)

            End Select

            strSql.Append(" WHERE 1 = 1  ")

            If _aziende <> "" Then
                strSql.Append(" AND DW_CDG_Costi_Ricavi.Piva In (" & Agro_SQL_Save_Clausola_IN("'" & Replace(_aziende, "|", "','") & "'", True) & ") ")
            End If

            ' Se budget = 0 significa che si cerca il consuntivo, diversamente una specifica versione di budget
            strSql.Append(" AND DW_CDG_Costi_Ricavi.Budget_Cons =  " & Agro_SQL_SaveNum(id_budget) & " ")

            If costi_ricavi <> -1 Then
                strSql.Append(" AND DW_CDG_Costi_Ricavi.Costi_Ricavi =  " & Agro_SQL_SaveNum(costi_ricavi) & " ")
            End If
            If Not String.IsNullOrEmpty(dataDal) Then
                strSql.Append(" AND DW_CDG_Costi_Ricavi.Data_Inserimento >= " & Agro_SQL_SaveDate(dataDal))
            End If
            If Not String.IsNullOrEmpty(dataAl) Then
                strSql.Append(" AND DW_CDG_Costi_Ricavi.Data_Inserimento <= " & Agro_SQL_SaveDate(dataAl))
            End If
            If Not String.IsNullOrEmpty(dataRifDistinta) Then
                strSql.Append(" AND DW_CDG_Costi_Ricavi.Progetto_Validita_Inizio <= " & Agro_SQL_SaveDate(dataRifDistinta))
                strSql.Append(" AND DW_CDG_Costi_Ricavi.Progetto_Validita_Fine >= " & Agro_SQL_SaveDate(dataRifDistinta))
            End If

            If Not String.IsNullOrEmpty(filtro_codice_appezzamento) Then
                'Costruzione stringa con apici
                strSql.AppendLine(" And Upper(DW_CDG_Costi_Ricavi.Nome_Appezzamento) In (" & Agro_SQL_Save_Clausola_IN("'" & Replace(filtro_codice_appezzamento, ",", "','") & "'", True) & ") ")
            End If

            If Not String.IsNullOrEmpty(filtro_codice_impianto) Then
                'Costruzione stringa con apici
                strSql.AppendLine(" And Upper(DW_CDG_Costi_Ricavi.Codice_Impianto) In (" & Agro_SQL_Save_Clausola_IN("'" & Replace(filtro_codice_impianto, ",", "','") & "'", True) & ") ")
            End If

            If Not String.IsNullOrEmpty(filtro_azienda_padre) Then
                'Costruzione stringa con apici
                strSql.AppendLine(" And DW_CDG_Costi_Ricavi.piva In (" & Agro_SQL_Save_Clausola_IN(filtro_piva_padre, True) & ") ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" And " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If


            'Nota: Controllo coerenza impianti: bypasso i costi associati a distinte non presenti
            If bBypassDistinteNonPresenti Then
                strSql.AppendLine(" And (DW_CDG_Costi_Ricavi.Progetto_Cod = 0 Or Exists (select * from " & TabellaPrefisso & "Imprese_Progetti where " & TabellaPrefisso & "Imprese_Progetti.Piva = DW_CDG_Costi_Ricavi.Piva ")
                strSql.AppendLine(" And " & TabellaPrefisso & "Imprese_Progetti.Progetto_Cod = DW_CDG_Costi_Ricavi.Progetto_Cod))  ")
            End If


            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.Append(" ORDER BY DW_CDG_Costi_Ricavi.Data_Inserimento" & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            Dim Ora As Integer
            Dim Minuti As Integer
            Dim parteIntera As Decimal
            Dim parteDecimali As Decimal

            dt.Columns.Add("QtaString", GetType(String))
            For Each r In dt.Rows

                If CInt(r.Item("Cod_Risum")) <> 0 OrElse CInt(r.Item("Mac_Cod") <> 0) Then
                    'Conversione intero minuti a data
                    parteIntera = Math.Truncate(CDec(r.Item("Qta")))
                    parteDecimali = CDec(r.Item("Qta")) - parteIntera
                    Ora = parteIntera
                    Minuti = 60 * parteDecimali
                    r.Item("QtaString") = Ora.ToString("D2") & "" & Minuti.ToString("D2")
                Else
                    r.Item("QtaString") = CStr(r.Item("Qta"))
                End If
            Next

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("" & nomeRoutine & "  " & messaggioErrore)
        End Try


        Return dt

    End Function




    '################################################################################
    Public Function Leggi_Tabellone_DW_Valorizzazione(ByVal _aziende As String,
                                                      ByVal progetto_cod As Integer,
                                                      ByVal elem_cod As Integer,
                                                      ByVal mat_cod As Integer,
                                                      ByVal priorita As Integer,
                                                      ByVal dataDal As String,
                                                      ByVal dataAl As String,
                                                      ByVal xFiltroAggiuntivo As String,
                                                      ByVal bBypassDistinteNonPresenti As Boolean,
                                                      ByVal xOrderBy As String,
                                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                      ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.DW_CDG_Costi_Ricavi_DAL_R.Leggi_Tabellone_DW_Report()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim strSql_Raccolta As New System.Text.StringBuilder
        Dim dt As DataTable
        Dim filtro_piva_padre As String = ""
        Dim TabellaPrefisso As String = ""

        Try



            strSql.Length = 0
            strSql.Append(" SELECT DW_CDG_Costi_Ricavi.*, isnull(Attivita.Frazionabile,0) as Frazionabile, ")
            strSql.Append(" Case When Destinazione_Uso <> '' Then 'Sì' Else '' End As Terreno_Nudo " & vbCrLf)


            strSql.Append(" ,isnull(Imprese_Progetti.FlagSecondoRaccolto, 0) As FlagSecondoRaccolto ")
            strSql.Append(" ,isnull(mov_dettagli_riferimenti.Id_Agenda, 0) as Id_Agenda_Agenda ")
            strSql.Append(" ,isnull(mov_dettagli_riferimenti.Lav_Cod, 0) as Lav_Cod_Agenda ")
            strSql.Append(" ,isnull(operazioni.Lav_Des, '') as Lav_Des_Agenda ")
            strSql.Append(" from DW_CDG_Costi_Ricavi " & vbCrLf)

            strSql.Append(" Left outer join mov_dettagli_riferimenti on (DW_CDG_Costi_Ricavi.id_agenda = mov_dettagli_riferimenti.id_agenda_rif ) ")
            strSql.Append(" Left outer join operazioni on (operazioni.Lav_Cod = mov_dettagli_riferimenti.Lav_Cod ) ")
            strSql.Append(" Left outer join attivita on (DW_CDG_Costi_Ricavi.id_attivita = Attivita.id_attivita ) ")
            strSql.Append(" inner join Imprese_Progetti on (DW_CDG_Costi_Ricavi.Piva = Imprese_Progetti.Piva And DW_CDG_Costi_Ricavi.Progetto_Cod = Imprese_Progetti.Progetto_Cod ) ")

            strSql.Append(" WHERE 1 = 1  ")

            If _aziende <> "" Then
                strSql.Append(" AND DW_CDG_Costi_Ricavi.Piva In (" & Agro_SQL_Save_Clausola_IN("'" & Replace(_aziende, "|", "','") & "'", True) & ") ")
            End If

            If progetto_cod <> 0 Then
                strSql.Append(" AND DW_CDG_Costi_Ricavi.Progetto_Cod =  " & Agro_SQL_SaveNum(progetto_cod) & " ")
            End If

            If Not String.IsNullOrEmpty(dataDal) Then
                strSql.Append(" AND DW_CDG_Costi_Ricavi.Data_Inserimento >= " & Agro_SQL_SaveDate(dataDal))
            End If
            If Not String.IsNullOrEmpty(dataAl) Then
                strSql.Append(" AND DW_CDG_Costi_Ricavi.Data_Inserimento <= " & Agro_SQL_SaveDate(dataAl))
            End If

            'Costruzione Filtro Costi ASSOCIATI ALLA RACCOLTA
            strSql_Raccolta.Length = 0
            strSql_Raccolta.Append(" mov_dettagli_riferimenti.Lav_Cod = 125 And mov_dettagli_riferimenti.Id_Agenda In (Select AG.Id_Agenda From Agenda AG, Movimenti, Movimenti_Dettagli ")
            strSql_Raccolta.Append(" Where AG.Piva = Movimenti.Piva And AG.Id_Agenda = Movimenti.Id_Agenda And ")
            strSql_Raccolta.Append(" Movimenti.Id_Agenda = Movimenti_Dettagli.Id_Agenda And Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov And ")
            strSql_Raccolta.Append(" Movimenti.Cau_Mov = '2200' And Movimenti_Dettagli.Elem_Cod = " & elem_cod & " And Movimenti_Dettagli.Mat_Cod = " & mat_cod & ") ")



            Select Case priorita

                Case 0

                    'Tutti i costi ed i costi relativi alla raccolta del mat_cod
                    strSql.Append(" AND ( mov_dettagli_riferimenti.Lav_Cod <> 125 Or mov_dettagli_riferimenti.Lav_Cod Is null Or " & strSql_Raccolta.ToString & ")")

                Case Else

                    'Solo i costi relativi la raccolta del mat_cod
                    strSql.Append(" AND " & strSql_Raccolta.ToString & "  ")

            End Select


            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" And " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If


            'Nota: Controllo coerenza impianti: bypasso i costi associati a distinte non presenti
            If bBypassDistinteNonPresenti Then
                strSql.AppendLine(" And (DW_CDG_Costi_Ricavi.Progetto_Cod = 0 Or Exists(Select * from " & TabellaPrefisso & "Imprese_Progetti where " & TabellaPrefisso & "Imprese_Progetti.Piva = DW_CDG_Costi_Ricavi.Piva ")
                strSql.AppendLine(" And " & TabellaPrefisso & "Imprese_Progetti.Progetto_Cod = DW_CDG_Costi_Ricavi.Progetto_Cod))  ")
            End If


            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.Append(" ORDER BY DW_CDG_Costi_Ricavi.Data_Inserimento" & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            Dim Ora As Integer
            Dim Minuti As Integer
            Dim parteIntera As Decimal
            Dim parteDecimali As Decimal

            dt.Columns.Add("QtaString", GetType(String))
            For Each r In dt.Rows

                If CInt(r.Item("Cod_Risum")) <> 0 OrElse CInt(r.Item("Mac_Cod") <> 0) Then
                    'Conversione intero minuti a data
                    parteIntera = Math.Truncate(CDec(r.Item("Qta")))
                    parteDecimali = CDec(r.Item("Qta")) - parteIntera
                    Ora = parteIntera
                    Minuti = 60 * parteDecimali
                    r.Item("QtaString") = Ora.ToString("D2") & "" & Minuti.ToString("D2")
                Else
                    r.Item("QtaString") = CStr(r.Item("Qta"))
                End If
            Next

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("" & nomeRoutine & "  " & messaggioErrore)
        End Try


        Return dt

    End Function

    '################################################################################
    Public Function Determina_Validita_Inizio_Report(ByVal _aziende As String,
                                                     ByVal filtro_azienda_padre As String,
                                                     ByVal id_budget As Integer,
                                                     ByVal costi_ricavi As Integer,
                                                     ByVal dataDal As String,
                                                     ByVal dataDal_CDG As String,
                                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                     ) As String

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.DW_CDG_Costi_Ricavi_DAL_R.Leggi_Tabellone_DW_Report()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable
        Dim dt_aziende_padre As DataTable
        Dim filtro_piva_padre As String = ""

        'Determino la validità inizio inferiore delle aziende selezionate con distinte aperte alla data inizio AA e che hanno un costo
        'entro la data di fine validità cdg

        Try

            ' Se richiesto filtro per azienda padre cerco le aziende figlie in gerarchia
            strSql.Length = 0
            If Not String.IsNullOrEmpty(filtro_azienda_padre) Then
                'Comprendo anche l'azienda padre
                filtro_piva_padre = "'" + filtro_azienda_padre + "'"

                strSql.AppendLine(" select figlio from GerarchiaImprese where padre = '" & filtro_azienda_padre & "' or padre in (select figlio from GerarchiaImprese where padre = '" & filtro_azienda_padre & "')")
                '-------------------------------------------------------------------------------
                dt_aziende_padre = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
                '-------------------------------------------------------------------------------

                If Not dt_aziende_padre Is Nothing AndAlso dt_aziende_padre.Rows.Count > 0 Then
                    For Each r In dt_aziende_padre.Rows
                        filtro_piva_padre &= ",'" & r.Item("figlio") & "'"
                    Next
                End If
            End If

            strSql.Length = 0
            strSql.Append(" SELECT Min (DW_CDG_Costi_Ricavi.Progetto_Validita_Inizio) as Progetto_Validita_Inizio " & vbCrLf)

            strSql.Append(" from DW_CDG_Costi_Ricavi " & vbCrLf)

            strSql.Append(" WHERE DW_CDG_Costi_Ricavi.Progetto_Validita_Fine >= " & Agro_SQL_SaveDate(dataDal))
            strSql.Append(" And DW_CDG_Costi_Ricavi.Progetto_Validita_Inizio > " & Agro_SQL_SaveDate(AGRODATAINIZIO))
            strSql.Append(" And DW_CDG_Costi_Ricavi.Data_Inserimento <= " & Agro_SQL_SaveDate(dataDal_CDG))
            strSql.Append(" And Progetto_Cod In (Select Progetto_Cod From Imprese_Progetti) ")

            If _aziende <> "" Then
                strSql.Append(" And DW_CDG_Costi_Ricavi.Piva In (" & Agro_SQL_Save_Clausola_IN("'" & Replace(_aziende, "|", "','") & "'", True) & ") ")
            End If

            ' Se budget = 0 significa che si cerca il consuntivo, diversamente una specifica versione di budget
            strSql.Append(" AND DW_CDG_Costi_Ricavi.Budget_Cons =  " & Agro_SQL_SaveNum(id_budget) & " ")

            If costi_ricavi <> -1 Then
                strSql.Append(" AND DW_CDG_Costi_Ricavi.Costi_Ricavi =  " & Agro_SQL_SaveNum(costi_ricavi) & " ")
            End If

            If Not String.IsNullOrEmpty(filtro_azienda_padre) Then
                'Costruzione stringa con apici
                strSql.AppendLine(" And DW_CDG_Costi_Ricavi.piva In (" & Agro_SQL_Save_Clausola_IN(filtro_piva_padre, True) & ") ")
            End If



            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("" & nomeRoutine & "  " & messaggioErrore)
        End Try


        Return dt(0)("Progetto_Validita_Inizio")

    End Function




    Public Function TrovaRighe_By_Vecchio_Tipo_Inser_Dati(ByVal piva As String,
                                  ByVal Vecchio_Tipo_Inser_Dati As Integer,
                                  ByRef CDG_Dati_Letti As List(Of DW_CDG_Costi_Ricavi),
                                  ByRef objParametri As AgronicaCoreParametri
                                  ) As String

        Dim messaggioErrore As String = ""

        Dim risposta As Boolean = False

        Dim pivaSuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL.TrovaRighe_By_Vecchio_Tipo_Inser_Dati()"

        Try

            Dim gefutils As New Gias_EF_Utility

            Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

            'Dim transactionOptions As New System.Transactions.TransactionOptions()
            'transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted

            Using giasContext As New Gias_DeveloperServer_Entities(efConnString)

                ' GiasContext.ContextOptions.LazyLoadingEnabled = False

                giasContext.Database.CommandTimeout = 3600

                CDG_Dati_Letti = (From dati In giasContext.DW_CDG_Costi_Ricavi
                                  Where dati.Piva_Superuser.Equals(pivaSuperUser) AndAlso
                                        dati.Piva.Equals(piva) AndAlso
                                        dati.Vecchio_Tipo_Inser_Dati = Vecchio_Tipo_Inser_Dati).ToList()

            End Using

            'End Using

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("" & nomeRoutine & "  " & messaggioErrore)

        End Try

        Return messaggioErrore

    End Function

    '###############################################################################
    Public Function Leggi_ValorizzazioneProdotto(ByVal Tipo_Valorizzazione As Integer, ByVal Piva As String,
                                                 ByVal Elem_Cod As Integer, ByVal Pro_Cod As Integer,
                                                 ByVal Mat_Cod As Integer, ByVal Udm_Cod As Integer,
                                                 ByVal Lotto As String,
                                                 ByVal Data_Dal As Date, ByVal Data_Al As Date, ByVal budget As Integer,
                                                 ByRef objParametriServer As AgronicaCoreParametri) As Decimal

        'TIPO_VALORIZZAZIONE (Impostazione_Cod = 846)
        '1 = COSTO MEDIA PONDERATA
        '2 = RICAVO MEDIO PONDERATO
        '3 = Costo / Ricavo da tabella Prodotti_Costi (non gestita qui)
        '4 = ULTIMO COSTO
        '5 = ULTIMO RICAVO

        Dim ValorizzazioneProdotto As Decimal = 0
        Dim Lav_Cod_Costi = "1000, 1002, 1021, 1022, 1025, 1054, 1056, 1057, 1075, 1076, 1077, 1078, 3001, 3028"
        Dim Lav_Cod_Ricavi = "1001, 1003, 1020, 1023, 1031, 1053, 1052, 1055, 1058, 1028, 1069"
        Dim filtroProdotti = "Elem_Cod <> 0 And Elem_Cod <> 1"
        Dim filtroProdotti_Costi = "Prodotti_Costi.Elem_Cod <> 0 And Prodotti_Costi.Elem_Cod <> 1"
        Dim objGiacenze As New AgronicaCoreContabDAL.Giacenze_R

        Select Case Tipo_Valorizzazione
            Case 10, 20, 40, 50
                'Do Nothing --> Lotto Sensibile al Costo
            Case Else
                Lotto = "***Bypass***"
        End Select


        ' valore medio ponderato
        If Tipo_Valorizzazione = 1 OrElse Tipo_Valorizzazione = 2 Or Tipo_Valorizzazione = 10 Or Tipo_Valorizzazione = 20 Then
            Dim filtro = filtroProdotti & " And Agenda.Lav_Cod IN (" & If(Tipo_Valorizzazione = 1 Or Tipo_Valorizzazione = 10, Lav_Cod_Costi, Lav_Cod_Ricavi) & ") "
            Dim dt_Valore_Medio_Ponderato = objGiacenze.Leggi_ValoreMedioPonderato(Piva, Elem_Cod, Pro_Cod, Mat_Cod, Udm_Cod, Data_Dal, Data_Al, filtro, objParametriServer, Lotto)
            If dt_Valore_Medio_Ponderato.Rows.Count > 0 Then
                ValorizzazioneProdotto = dt_Valore_Medio_Ponderato.Rows(0).Item("Costo_Medio_Ponderato")
            End If
        End If

        ' anagrafica prodotti valore
        If Tipo_Valorizzazione = 3 Then
            Dim filtro = filtroProdotti_Costi
            Dim dt_Valore_Anagrafica_Prodotti = objGiacenze.Leggi_ValoreAnagraficaProdotti(Piva, Elem_Cod, Pro_Cod, Mat_Cod, Udm_Cod, Data_Al, Data_Al, filtro, budget, objParametriServer)
            If dt_Valore_Anagrafica_Prodotti.Rows.Count > 0 Then

                ValorizzazioneProdotto = dt_Valore_Anagrafica_Prodotti.Rows(0).Item("Prezzo_Unitario")

                ' Potrei avere due righe, una con prezzo specifico per azienda e una con prezzo prodotto pubblico
                If dt_Valore_Anagrafica_Prodotti.Rows.Count > 1 Then
                    For Each row In dt_Valore_Anagrafica_Prodotti.Rows
                        If row.Item("Piva") = Piva Then
                            ValorizzazioneProdotto = row.Item("Prezzo_Unitario")
                        End If
                    Next
                End If
            End If
        End If

        ' valore ultimo prodotto
        If Tipo_Valorizzazione = 4 OrElse Tipo_Valorizzazione = 5 Or Tipo_Valorizzazione = 40 OrElse Tipo_Valorizzazione = 50 Then
            Dim filtro = filtroProdotti & " And Agenda.Lav_Cod IN (" & If(Tipo_Valorizzazione = 4 Or Tipo_Valorizzazione = 40, Lav_Cod_Costi, Lav_Cod_Ricavi) & ") "
            Dim dt_Valore_Ultimo_Prodotto = objGiacenze.Leggi_ValoreUltimoProdotto(Piva, Elem_Cod, Pro_Cod, Mat_Cod, Udm_Cod, Data_Dal, Data_Al, filtro, objParametriServer, Lotto)

            If dt_Valore_Ultimo_Prodotto.Rows.Count > 0 Then
                ValorizzazioneProdotto = dt_Valore_Ultimo_Prodotto.Rows(0).Item("Costo_Ultimo_Prodotto")
            End If
        End If

        Return Format(ValorizzazioneProdotto, "###,###,##0.0#####")

    End Function

    Public Function Ipno_Valorizzazione_Prodotto(ByVal Tipo_Valorizzazione As Integer,
                                                 ByVal PIVA As String,
                                                 ByVal Elem_Cod As Long,
                                                 ByVal Pro_Cod As Long,
                                                 ByVal Mat_Cod As Long,
                                                 ByVal Udm_Cod As Long,
                                                 ByVal Cal_Cod As Long,
                                                 ByVal Cod_Progetto As Long,
                                                 ByVal Fase_Cod As Long,
                                                 ByVal Lotto As String,
                                                 ByVal strFiltro As String,
                                                 ByVal Dal_Data_Verifica As Date,
                                                 ByVal Al_Data_Verifica As Date,
                                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                 ) As Decimal

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.DW_CDG_Costi_Ricavi_DAL_R.Ipno_Valorizzazione_Prodotto()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim strJoin As New System.Text.StringBuilder
        Dim dt As DataTable
        Dim ValorizzazioneProdotto As Decimal = 0

        'TIPO_VALORIZZAZIONE
        '1 = COSTO MEDIA PONDERATA
        '2 = RICAVO MEDIO PONDERATO

        '3 = Costo / Ricavo da tabella Prodotti_Costi (non gestita qui)

        '4 = ULTIMO COSTO
        '5 = ULTIMO RICAVO

        If Tipo_Valorizzazione <> 3 Then

            Try

                strJoin.Length = 0

                'Condizioni Di Join
                strJoin.Append(" And Agenda.Piva = Movimenti.Piva ")
                strJoin.Append(" And Movimenti.Piva = Movimenti_Dettagli.Piva ")
                strJoin.Append(" And Movimenti_Dettagli.Piva = Mov_Destinazioni.Piva ")

                strJoin.Append(" And Agenda.Id_Agenda = Movimenti.Id_Agenda ")
                strJoin.Append(" And Movimenti.Id_Agenda = Movimenti_Dettagli.Id_Agenda ")
                strJoin.Append(" And Movimenti_Dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda ")

                strJoin.Append(" And Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov ")
                strJoin.Append(" And Movimenti_Dettagli.Id_Mov = Mov_Destinazioni.Id_Mov ")

                strJoin.Append(" And Movimenti_Dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")

                'Lettura mirata del prodotto

                strJoin.Append(" And Mov_Destinazioni.Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
                strJoin.Append(" AND Movimenti_Dettagli.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & " ")
                strJoin.Append(" AND Movimenti_Dettagli.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & " ")
                strJoin.Append(" AND Movimenti_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & " ")
                strJoin.Append(" AND Movimenti_Dettagli.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & " ")
                strJoin.Append(" AND Movimenti_Dettagli.Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & " ")

                If Fase_Cod <> 0 Then
                    strJoin.Append(" AND Movimenti_Dettagli.Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & " ")
                End If

                If Trim(Lotto) <> "***Bypass***" Then

                    If Trim(Lotto) = "" Then
                        strJoin.Append(" AND Upper(Movimenti_Dettagli.Lotto) In ('INDEFINITO', '" & UCase(Agro_SQL_SaveText(Lotto)) & "')   ")
                    Else
                        strJoin.Append(" AND Upper(Movimenti_Dettagli.Lotto) = '" & UCase(Agro_SQL_SaveText(Lotto)) & "' ")
                    End If

                End If


                If Trim(strFiltro) <> "" Then
                    strJoin.Append(" AND (" & strFiltro & ") ")
                End If


                strJoin.Append(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Dal_Data_Verifica))

                strJoin.Append(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Al_Data_Verifica))

                'La condizione necessaria è che il dettaglio abbia una imputazione economica e una quantità valida
                strJoin.Append(" AND Abs(Movimenti_Dettagli.Imponibile_Netto) > 0 AND Movimenti_Dettagli.Qta > 0 ")


                Select Case Tipo_Valorizzazione

                    Case 1, 4

                        '======================================================================================================================================================================
                        'COSTO PONDERATO --> Bolle Ricevute, Fatture Ricevute, Acquisto, Carico, Ricevimento Fatture Liquidazione, Emissione AutoFatture Liquidazione, Note di Accredito Emesse,
                        '                    Aumento Consistenze Zoo
                        '----------------------------------------------------------------------------------------------------------------------------------------------------------------------

                        strJoin.Append(" AND Agenda.Lav_Cod IN ( 1025, 1000, 1021, 1022, 1056, 1057, 1003, 3001) ")


                    Case 2, 5

                        '======================================================================================================================================================================
                        'RICAVO PONDERATO --> Bolle Emesse, Fatture Emesse, Vendita, Scarico, Emissione Ricevute Fiscali, Emissione Fatture Liquidazione, Ricevimento AutoFatture Liquidazione,
                        '                     Note di Accredito Ricevute, Autoconsumo
                        '----------------------------------------------------------------------------------------------------------------------------------------------------------------------
                        strJoin.Append(" AND Agenda.Lav_Cod IN ( 1031, 1001, 1020, 1023, 1053, 1052, 1055, 1058, 1002, 1028, 1069 ) ")

                    Case 3


                End Select


                Select Case Tipo_Valorizzazione

                    Case 1, 2 'Media Ponderata

                        strSql.Length = 0  'Nota: escludo le bolle allegate a fatture

                        strSql.Append(" Select SUM(Abs(IsNull(Movimenti_Dettagli.Imponibile_Netto,0))) as Delta, ")
                        strSql.Append(" SUM(IsNull(Movimenti_Dettagli.Qta ,0)) as Qta_Complessiva ")
                        strSql.Append(" FROM   Agenda, Movimenti, Movimenti_Dettagli, Mov_Destinazioni ")
                        strSql.Append(" WHERE  Movimenti_Dettagli.Id_Mov_Det NOT IN ")
                        strSql.Append("        (Select Distinct Id_Mov_Det_Rif From Mov_Dettagli_Riferimenti ")
                        strSql.Append("         Where Piva = '" & Agro_SQL_SaveText(PIVA) & "' And Lav_Cod In (1000, 1001) )")
                        strSql.Append(strJoin)




                    Case 4, 5 'Ultimo Valore

                        strSql.Length = 0

                        strSql.Append(" Select Top 1 (Abs(IsNull(Movimenti_Dettagli.Imponibile_Netto,0))) as Delta, ")
                        strSql.Append(" IsNull(Movimenti_Dettagli.Qta ,0) as Qta_Complessiva ")
                        strSql.Append(" FROM   Agenda, Movimenti, Movimenti_Dettagli, Mov_Destinazioni ")
                        strSql.Append(" WHERE  Agenda.Lav_Cod > 0 ")
                        strSql.Append(strJoin)

                        strSql.Append(" Order by Data_Movimento Desc ")

                End Select

                '--------------------------------------------------------------------------
                dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
                '--------------------------------------------------------------------------



            Catch ex As Exception
                messaggioErrore = ex.Message
                Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
                dt = Nothing
                Throw New Exception("" & nomeRoutine & " : " & messaggioErrore)
            End Try


            ValorizzazioneProdotto = 0

            For Each dr As DataRow In dt.Rows

                If IsNumeric(dr.Item("Delta")) AndAlso IsNumeric(dr.Item("Qta_Complessiva")) Then

                    Select Case CDbl(dr("Qta_Complessiva"))

                        Case 0 'Non esistono valorizzazioni del bene nel periodo competenza

                            ValorizzazioneProdotto = Format(0, "###,###,##0.0#")

                        Case Else 'Esistono valorizzazioni del bene nel periodo di competenza

                            ValorizzazioneProdotto = Format(dr.Item("Delta") / dr.Item("Qta_Complessiva"), "###,###,##0.0#")

                    End Select

                    Exit For

                End If

            Next


        End If

        Return Decimal.Parse(ValorizzazioneProdotto)

    End Function



    Public Function Ipno_Costo_Anagrafica(ByVal PIVA As String,
                                          ByVal Elem_Cod As Long,
                                           ByVal Riferimento As String,
                                           ByVal Pro_Cod As Long,
                                           ByVal Mat_Cod As Long,
                                           ByVal Veg_Cod As Long,
                                           ByVal Cul_Cod As Long,
                                           ByVal Udm_Cod As Long,
                                           ByVal FinestraTemp_Inizio As Date,
                                           ByVal FinestraTemp_Fine As Date,
                                          ByVal xFiltroAggiuntivo As String,
                                          ByVal budget As Integer,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                          ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.DW_CDG_Costi_Ricavi_DAL_R.Ipno_Costo_Anagrafica()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim strJoin As New System.Text.StringBuilder
        Dim dt As DataTable


        Try

            strSql.Length = 0

            strSql.Append(" SELECT Prodotti_Costi.* ")
            strSql.Append("  FROM   Prodotti_Costi ")
            strSql.Append(" WHERE Prodotti_Costi.Validita_inizio <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " ")
            strSql.Append(" AND Prodotti_Costi.Validita_Fine >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")
            strSql.Append(" AND Prodotti_Costi.Piva = '" & Agro_SQL_SaveText(PIVA) & "' And Prodotti_Costi.Id_Budget = " & budget & " ")

            If Elem_Cod <> 0 Then
                strSql.Append(" AND Prodotti_Costi.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & " ")
            End If

            If Riferimento <> "" Then
                strSql.Append(" AND Prodotti_Costi.Riferimento = '" & Agro_SQL_SaveText(Riferimento) & "'   ")
            End If

            If Pro_Cod <> 0 Then
                strSql.Append(" AND Prodotti_Costi.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
            End If

            If Mat_Cod <> 0 Then
                strSql.Append("  AND Prodotti_Costi.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
            End If

            If Veg_Cod <> 0 Then
                strSql.Append("  AND Prodotti_Costi.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "   ")
            End If

            If Cul_Cod <> 0 Then
                strSql.Append(" AND Prodotti_Costi.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & "   ")
            End If

            If Udm_Cod <> 0 Then
                strSql.Append(" AND Prodotti_Costi.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND  " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & "   ")
            End If

            strSql.Append(" ORDER BY Prodotti_Costi.Riferimento, Prodotti_Costi.Elem_Cod,  Prodotti_Costi.Pro_Cod,  Prodotti_Costi.Mat_Cod, Prodotti_Costi.Udm_Cod,  Prodotti_Costi.Veg_Cod,  Prodotti_Costi.Cul_Cod  ASC ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("" & nomeRoutine & " : " & messaggioErrore)
        End Try



        Return dt

    End Function





    '##############################################################################################
    Public Function Leggi_DW_CDG(ByVal piva As String,
                                 ByVal Id_Cfg_DW As Integer,
                                 ByRef objParametri As AgronicaCoreParametri,
                                 Optional ByVal xFiltroAggiuntivo As String = "",
                                 Optional ByVal Id_Budget As Int32 = 0) As DataTable

        Dim messaggioErrore As String = ""
        Dim DT As DataTable = Nothing
        Dim strSql As New System.Text.StringBuilder
        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_DW_CDG()"


        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT  ")
            strSql.AppendLine(" Piva, ")
            strSql.AppendLine(" Id_Cfg_DW, ")
            strSql.AppendLine(" Id_CDG, ")
            strSql.AppendLine(" Id_CDG_Dettagli, ")
            strSql.AppendLine(" Data_Inserimento, ")
            strSql.AppendLine(" Elem_Cod, ")
            strSql.AppendLine(" Pro_Cod, ")
            strSql.AppendLine(" Mat_Cod, ")
            strSql.AppendLine(" Macchine_Cod, ")
            strSql.AppendLine(" Cod_RisUm, ")
            strSql.AppendLine(" Mac_Cod, ")
            strSql.AppendLine(" 0 As Cal_Cod, ")
            strSql.AppendLine(" Progetto_Cod, ")
            strSql.AppendLine(" Lotto, ")
            strSql.AppendLine(" Udm_Cod, ")
            strSql.AppendLine(" Qta, ")
            strSql.AppendLine(" Prezzo_Unitario, ")
            strSql.AppendLine(" Valore, ")
            strSql.AppendLine(" Percentuale_Ripart, ")
            strSql.AppendLine(" Costi_Ricavi, ")
            strSql.AppendLine(" Id_Attivita, ")
            strSql.AppendLine(" Qualifica_Cod, ")
            strSql.AppendLine(" Tariffa_Cod, ")
            strSql.AppendLine(" Turno_Cod, ")
            strSql.AppendLine(" Data_Modifica ")
            strSql.AppendLine("  FROM   DW_CDG_Costi_Ricavi ")
            strSql.AppendLine(" WHERE  ")
            strSql.AppendLine(" Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            strSql.AppendLine(" AND Piva_Superuser = '" & Agro_SQL_SaveText(Piva_SuperUser) & "' ")
            strSql.AppendLine(" AND Id_Cfg_DW = " & Agro_SQL_SaveNum(Id_Cfg_DW) & " ")
            strSql.AppendLine(" AND Vecchio_Tipo_Inser_Dati = 0 ")
            strSql.AppendLine(" AND Costi_Ricavi = 0")
            strSql.AppendLine(" AND Modalita_Imputazione <> 4 ")
            strSql.AppendLine(" AND Budget_Cons = " + Agro_SQL_SaveNum(Id_Budget) + " ")

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND  " & xFiltroAggiuntivo)
            End If

            strSql.AppendLine(" ORDER BY Id_CDG ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            DT = Nothing
            Throw New Exception("" & nomeRoutine & " : " & messaggioErrore)
        End Try

        Return DT

    End Function


    Public Function GetFlagJoinCac(ByRef objParametriUtenti As AgronicaCoreParametri) As Boolean
        Const nomeRoutine = "GetFlagJoinCac"
        Dim flagJoinSuperUserCac As Boolean = False

        Try

            Dim objImp As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim joinSuperUser As String = objImp.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.SUPERUSER_JoinCacPivaSuperUser,
                                                                                          objParametriUtenti, 2)

            If joinSuperUser = "1" Then
                flagJoinSuperUserCac = True
            End If

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return flagJoinSuperUserCac
    End Function

    '##############################################################################################
    Public Function Leggi_CDG_BI_Esterna(ByVal piva As String,
                                 ByRef objParametri As AgronicaCoreParametri
                                 ) As DataTable

        Dim DT As DataTable = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_CDG_BI_Esterna()"

        Dim gefutils As New Gias_EF_Utility


        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)


        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim TestataElem =
               From DW In GiasContext.CDG_BI_Esterna
               Where DW.Piva.Equals(piva) AndAlso
                     DW.Piva_Superuser.Equals(Piva_SuperUser)
               Order By DW.Id_CDG
               Select New With {
              .Piva = DW.Piva,
              .Id_CDG = DW.Id_CDG,
              .Id_CDG_Dettagli = DW.Id_CDG_Dettagli,
              .Data_Inserimento = DW.Data_Inserimento,
              .Elem_Cod = DW.Elem_Cod,
              .Pro_Cod = DW.Pro_Cod,
              .Mat_Cod = DW.Mat_Cod,
              .Cod_Risum = DW.Cod_RisUm,
              .Mac_Cod = DW.Mac_Cod,
              .Lotto = DW.Lotto,
              .Udm_Cod = DW.Udm_Cod,
              .Qta = DW.Qta,
              .Prezzo_Unitario = DW.Prezzo_Unitario,
              .Valore = DW.Valore,
              .Percentuale_Ripart = DW.Percentuale_Ripart,
              .Id_Attivita = DW.Id_Attivita,
              .Qualifica_Cod = DW.Qualifica_Cod,
              .Tariffa_Cod = DW.Tariffa_Cod,
              .Turno_Cod = DW.Turno_Cod,
              .Data_Modifica = DW.Data_Modifica
              }

            Dim ut As New Gias_EF_Utility
            DT = ut.ObjectQueryToDataTable(TestataElem.Distinct().ToList())
        End Using

        Return DT

    End Function

    '##############################################################################################
    Public Function RicercaAziendePadreCdG(ByVal piva As String, ByVal budget As Integer, ByRef objParametri_Server As AgronicaCoreParametri
                                 ) As DataTable

        Dim DT As DataTable = Nothing
        Dim Piva_SuperUser = objParametri_Server.PivaSuperUser
        Dim strSql As New System.Text.StringBuilder
        Dim messaggioErrore As String

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.RicercaAziendePadreCdG()"

        Try


            strSql.Length = 0

            strSql.AppendLine(" Select Distinct Imprese.Piva as piva, Imprese.Rag_Soc as rag_soc From Imprese, GerarchiaImprese  ")
            strSql.AppendLine(" Where GerarchiaImprese.Padre = Imprese.Piva ")
            strSql.AppendLine(" And  GerarchiaImprese.Figlio in (Select  Piva From DW_CDG_Costi_Ricavi  ")
            strSql.AppendLine("                         Where DW_CDG_Costi_Ricavi.Piva_Superuser = '" & Agro_SQL_SaveText(Piva_SuperUser) & "' ")
            strSql.AppendLine("                         And  DW_CDG_Costi_Ricavi.Budget_Cons = " & budget & ") ")

            'Filtro Visibilità Imprese
            strSql.AppendLine(" and Gerarchiaimprese.Figlio in ( ")

            strSql.AppendLine(" Select Distinct dbo.Imprese.PIVA ")
            strSql.AppendLine(" FROM  (( ")
            strSql.AppendLine(" Imprese INNER JOIN UtentiXImprese On Imprese.Piva = UtentixImprese.Piva) ")
            strSql.AppendLine(" INNER Join ImpresexIndirizzi ON Imprese.PIVA = ImpresexIndirizzi.PIVA)  ")
            strSql.AppendLine(" Where ImpresexIndirizzi.Tipo_Indirizzo = 1 ")
            strSql.AppendLine(" And   UtentixImprese.[User] = '" & Agro_SQL_SaveText(objParametri_Server.PivaSuperUser) & "'")
            strSql.AppendLine(" And   Imprese.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine) & " ")
            strSql.AppendLine(" And   Imprese.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleInizio) & ") ")



            '----------------------------------------------------------------
            '--- Filtro associato all'utente 
            '----------------------------------------------------------------
            Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
            Dim UtenteProfiloImpreseSql As String = ""
            Dim UtenteProfiloCentriSql As String = ""
            Dim DtImpreseVisibili As DataTable
            Dim i As Integer

            DtImpreseVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Impresa, "", "", objParametri_Server)
            If Not DtImpreseVisibili Is Nothing Then
                For i = 0 To DtImpreseVisibili.Rows.Count - 1
                    UtenteProfiloImpreseSql &= "'" & DtImpreseVisibili.Rows(i).Item("Piva") & "',"
                Next
                If UtenteProfiloImpreseSql <> "" Then
                    UtenteProfiloImpreseSql = " AND Imprese.piva IN (" & Agro_SQL_Save_Clausola_IN(Left(UtenteProfiloImpreseSql, UtenteProfiloImpreseSql.Length - 1), True) & ") "
                End If

                strSql.AppendLine(UtenteProfiloImpreseSql)

            End If

            '-------------------------------------------------------------------------

            strSql.AppendLine(" Order by rag_soc Asc")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            DT = Nothing
            Throw New Exception("" & nomeRoutine & " :  " & messaggioErrore)
        End Try

        Return DT


    End Function



    ''##############################################################################################
    'Public Function RicercaAziendePadreCdG_Old(ByVal aziende_visibili As String(), ByVal budget As Integer, ByRef objParametri_Server As AgronicaCoreParametri
    '                             ) As DataTable

    '    Dim DT As DataTable = Nothing

    '    Dim Piva_SuperUser = objParametri_Server.PivaSuperUser

    '    '----- Descrizione
    '    Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.RicercaAziendePadreCdG()"

    '    Dim gefutils As New Gias_EF_Utility


    '    Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)


    '    Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

    '        Dim listFigli As New List(Of String)
    '        listFigli =
    '            (
    '                From DW In GiasContext.DW_CDG_Costi_Ricavi
    '                Where DW.Piva_Superuser.Equals(Piva_SuperUser) AndAlso DW.Budget_Cons = budget
    '                Select DW.Piva
    '               ).Distinct().ToList()

    '        Dim Pive_Padre =
    '           From Gerarchiaimprese In GiasContext.GerarchiaImprese
    '           Join imprese In GiasContext.Imprese
    '                 On Gerarchiaimprese.Padre Equals imprese.PIVA
    '           Where listFigli.Contains(Gerarchiaimprese.Figlio) AndAlso
    '               aziende_visibili.Contains(Gerarchiaimprese.Padre)
    '           Select New With {
    '          .piva = imprese.PIVA,
    '          .rag_soc = imprese.rag_soc
    '          }

    '        Dim ut As New Gias_EF_Utility
    '        DT = ut.ObjectQueryToDataTable(Pive_Padre.Distinct().ToList())
    '        If DT.Rows.Count > 0 Then
    '            DT.DefaultView.Sort = "rag_soc"
    '            DT = DT.DefaultView.ToTable()
    '        End If
    '    End Using

    '    Return DT

    'End Function

End Class




'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§




Public Class DW_CDG_Costi_Ricavi_DAL_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################

    Public Function AggiornaDW_Da_Lan(ByVal piva As String,
                                      ByVal Vecchio_Tipo_Inser_Dati As Integer,
                                      ByRef objParametri_Server As AgronicaCoreParametri,
                                      ByRef objParametri_Utenti As AgronicaCoreParametri
                                      ) As String

        Dim ObjSequenze = New Agro_Sequenze

        Dim retries As Integer = 3
        Dim success As Boolean = True

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_W.Scrivi_Export_Lan()"
        Dim messaggioErrore As String = ""

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)

        Try

            'Cancello i dati vecchi

            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                'Cancellazione Preventiva
                Dim leggi_DW_CDG_Costi_Ricavi As New DW_CDG_Costi_Ricavi_DAL_R

                Dim Record_ToDelete As List(Of DW_CDG_Costi_Ricavi) = Nothing

                'Cerco le righe da cancellare
                messaggioErrore = leggi_DW_CDG_Costi_Ricavi.TrovaRighe_By_Vecchio_Tipo_Inser_Dati(
                                        piva, Vecchio_Tipo_Inser_Dati,
                                        Record_ToDelete,
                                        objParametri_Server)

                If messaggioErrore = "" Then

                    ' Cancellazione lancio precedente
                    For Each cr As DW_CDG_Costi_Ricavi In Record_ToDelete
                        GiasContext.DW_CDG_Costi_Ricavi.Attach(cr)
                        GiasContext.DW_CDG_Costi_Ricavi.Remove(cr)
                    Next

                    GiasContext.SaveChanges()

                End If

            End Using

            'Inserisco i dati nuovi

            Dim xRisp = InserisciDW_Da_Lan(piva, Vecchio_Tipo_Inser_Dati, objParametri_Server, objParametri_Utenti)
            If Not xRisp Then
                messaggioErrore = "Errore durante la fase di creazione della tabella di DataWarehouse Costi - Ricavi: contattare l'assistenza"
            End If

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            Throw New Exception("" & nomeRoutine & " : " & messaggioErrore)

        End Try

        Return messaggioErrore

    End Function

    Public Function InserisciDW_Da_Lan(ByVal piva As String, ByVal Vecchio_Tipo_Inser_Dati As Integer,
                                       ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                       ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                           ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.DW_CDG_Costi_Ricavi_DAL_W.Scrivi()"


        '====================================================================================
        'Parametri opzionali :
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Dim pivaSuperUser = objParametri_Server.PivaSuperUser
        Dim Leggi_DW_CDG As New DW_CDG_Costi_Ricavi_DAL_R
        Dim flagJoinSuperUserCac As Boolean = Leggi_DW_CDG.GetFlagJoinCac(objParametri_Utenti)

        Try


            '---------------------------------------------
            strSql.Length = 0

            strSql.Append(" INSERT INTO DW_CDG_Costi_Ricavi ( " & vbCrLf)
            strSql.Append("  Piva_Superuser " & vbCrLf)
            strSql.Append(" ,Piva " & vbCrLf)
            strSql.Append(" ,Id_CDG " & vbCrLf)
            strSql.Append(" ,Id_CDG_Dettagli " & vbCrLf)
            strSql.Append(" ,Criterio_Analisi " & vbCrLf)
            strSql.Append(" ,Data_Inserimento " & vbCrLf)
            strSql.Append(" ,Descr_Modalita_Imputazione " & vbCrLf)
            strSql.Append(" ,Id_Agenda " & vbCrLf)
            strSql.Append(" ,Id_Mov " & vbCrLf)
            strSql.Append(" ,Id_Mov_Det " & vbCrLf)
            strSql.Append("  ,Mac_Cod " & vbCrLf)
            strSql.Append(" ,Cod_RisUm " & vbCrLf)
            strSql.Append(" ,Elem_Cod " & vbCrLf)
            strSql.Append(" ,Pro_Cod " & vbCrLf)
            strSql.Append(" ,Mat_Cod " & vbCrLf)
            strSql.Append(" ,Id_Attivita " & vbCrLf)
            strSql.Append(" ,Qualifica_Cod " & vbCrLf)
            strSql.Append(" ,Tariffa_Cod " & vbCrLf)
            strSql.Append(" ,Turno_Cod " & vbCrLf)
            strSql.Append(" ,Conto_Cod " & vbCrLf)
            strSql.Append(" ,Lotto " & vbCrLf)
            strSql.Append(" ,Mezzo " & vbCrLf)
            strSql.Append(" ,Mezzo_Des " & vbCrLf)
            strSql.Append(" ,Prezzo_Unitario " & vbCrLf)
            strSql.Append(" ,Qta " & vbCrLf)
            strSql.Append(" ,Valore " & vbCrLf)
            strSql.Append(" ,Percentuale_Ripart " & vbCrLf)
            strSql.Append(" ,Descrizione " & vbCrLf)
            strSql.Append(" ,Budget_Cons " & vbCrLf)
            strSql.Append(" ,Budget_Cons_Des " & vbCrLf)
            strSql.Append(" ,Costi_Ricavi " & vbCrLf)
            strSql.Append(" ,Costi_Ricavi_Des " & vbCrLf)
            strSql.Append(" ,Sa_Cod " & vbCrLf)
            strSql.Append(" ,Appezza " & vbCrLf)
            strSql.Append(" ,Id_Reg " & vbCrLf)
            strSql.Append(" ,Id_Cod_reg_impianti_codici " & vbCrLf)
            strSql.Append(" ,Id_Imputazione " & vbCrLf)
            strSql.Append(" ,Macchine_Cod " & vbCrLf)
            strSql.Append(" ,Linea_Cod " & vbCrLf)
            strSql.Append(", Veg_Cod  " & vbCrLf)
            strSql.Append(", Cul_Cod " & vbCrLf)
            strSql.Append(" , Descrizione_Operazione " & vbCrLf)
            strSql.Append(" , Descrizione_Macchina " & vbCrLf)
            strSql.Append(" , Nome_Cognome " & vbCrLf)

            strSql.Append(" , Categoria " & vbCrLf)

            strSql.Append(" , Descrizione_Prodotto " & vbCrLf)
            strSql.Append(" , Attivita " & vbCrLf)
            strSql.Append(" , Qualifica " & vbCrLf)
            strSql.Append(" , Tariffa " & vbCrLf)
            strSql.Append(" , Turno " & vbCrLf)
            strSql.Append(" , Conto " & vbCrLf)
            strSql.Append(" , Ragione_Sociale_Azienda " & vbCrLf)
            strSql.Append(" , Indirizzo_Azienda " & vbCrLf)
            strSql.Append(" , Cap_Azienda " & vbCrLf)
            strSql.Append(" , Localita_Azienda " & vbCrLf)
            strSql.Append(" , Prov_Azienda " & vbCrLf)
            strSql.Append(" , Descrizione_Centro " & vbCrLf)
            strSql.Append(" , Indirizzo_Centro " & vbCrLf)
            strSql.Append(" , Cap_Centro " & vbCrLf)
            strSql.Append(" , Localita_Centro " & vbCrLf)
            strSql.Append(" , Prov_Centro " & vbCrLf)
            strSql.Append(" , Descrizione_Campo " & vbCrLf)
            strSql.Append(" , Nome_Appezzamento " & vbCrLf)
            strSql.Append(" , Inizio_Appezzamento " & vbCrLf)
            strSql.Append(" , Fine_Appezzamento " & vbCrLf)
            strSql.Append(" , Descrizione_Gruppo_Vegetale " & vbCrLf)
            strSql.Append(" , Specie_impianto " & vbCrLf)
            strSql.Append(" , Varieta_impianto " & vbCrLf)
            strSql.Append(" , Descrizione_Finalita " & vbCrLf)
            strSql.Append(" , Destinazione_Uso " & vbCrLf)
            strSql.Append(" , Inizio_Impianto " & vbCrLf)
            strSql.Append(" , Fine_Impianto " & vbCrLf)

            strSql.Append(" , Progetto_Nome " & vbCrLf)
            strSql.Append(" , Regolamento " & vbCrLf)
            strSql.Append(" , Disciplinare " & vbCrLf)

            strSql.Append(" , Esposizione_Appezzamento " & vbCrLf)
            strSql.Append(" , Ubicazione_Appezzamento " & vbCrLf)
            strSql.Append(" , Descrizione_Portinnesti " & vbCrLf)
            strSql.Append(" , Descrizione_Irrigazioni " & vbCrLf)
            strSql.Append(" , Regione_Impianto " & vbCrLf)
            strSql.Append(" , Prov_Impianto " & vbCrLf)
            strSql.Append(" , Comune_Impianto " & vbCrLf)
            strSql.Append(" , Sezione " & vbCrLf)
            strSql.Append(" , Foglio " & vbCrLf)
            strSql.Append(" , Numero " & vbCrLf)
            strSql.Append(" , Subalterno " & vbCrLf)
            strSql.Append(" , Descrizione_Macchina_Input_Costi " & vbCrLf)
            strSql.Append(" , Progetto " & vbCrLf)
            strSql.Append(" , Classe_Progetto " & vbCrLf)
            strSql.Append(" , Tipo_Progetto " & vbCrLf)
            strSql.Append(" , Linea_Produzione " & vbCrLf)
            strSql.Append(" , Campo_Cod " & vbCrLf)
            strSql.Append(" , Lotto_Input_Costi " & vbCrLf)
            strSql.Append(" , Progetto_Cod " & vbCrLf)
            strSql.Append(" , Progetto_Des " & vbCrLf)
            strSql.Append(" , Progetto_Validita_Inizio " & vbCrLf)
            strSql.Append(" , Progetto_Validita_Fine " & vbCrLf)
            strSql.Append(" , Note " & vbCrLf)
            strSql.Append(" , Vecchio_Tipo_Inser_Dati " & vbCrLf)
            strSql.Append(" , inviato " & vbCrLf)
            strSql.Append(" , datainvio " & vbCrLf)
            strSql.Append(" , Data_Creazione " & vbCrLf)
            strSql.Append(" , Data_Modifica " & vbCrLf)
            strSql.Append(" , Username_Creazione " & vbCrLf)
            strSql.Append(" , Username_Modifica " & vbCrLf)
            strSql.Append(" , Validita_Inizio " & vbCrLf)
            strSql.Append(" , Validita_Fine " & vbCrLf)
            strSql.Append(" , Tipo_Imputazione " & vbCrLf)
            strSql.Append(" , Id_Cfg_DW " & vbCrLf)
            strSql.Append(" , Udm_Cod " & vbCrLf)
            strSql.Append(" , Codice_Impianto " & vbCrLf)
            strSql.Append(" , Cod_Animale " & vbCrLf)
            strSql.Append(" , Animale_Progetto " & vbCrLf)
            strSql.Append(" , Cod_Animale_Distinta " & vbCrLf)
            strSql.Append(" , Animale_Distinta " & vbCrLf)
            strSql.Append(" , Sta_Num " & vbCrLf)
            strSql.Append(" , Stalla_Des " & vbCrLf)
            strSql.Append(" , Raggruppamento_Cod " & vbCrLf)
            strSql.Append(" , Raggruppamento_Des " & vbCrLf)
            strSql.Append(" , Specie_Animale_Cod " & vbCrLf)
            strSql.Append(" , Specie_Animale_Des " & vbCrLf)
            strSql.Append(" , Razza_Animale_Cod " & vbCrLf)
            strSql.Append(" , Razza_Animale_Des " & vbCrLf)
            strSql.Append(" , Tipo_Animale_Cod " & vbCrLf)
            strSql.Append(" , Tipo_Animale_Des " & vbCrLf)
            strSql.Append(" , Modalita_Imputazione " & vbCrLf)
            strSql.Append(" , Cod_Articolo " & vbCrLf)
            strSql.Append(" , MetodoProduzioneAppezzamento " & vbCrLf)
            strSql.Append(" , Sup_app " & vbCrLf)
            strSql.Append(" , Sup_imp " & vbCrLf)
            strSql.Append(" , Sup_Prog " & vbCrLf)
            strSql.Append(" ) " & vbCrLf)

            strSql.Append(" Select " & vbCrLf)

            strSql.Append("  CDG_Testata.Piva_Superuser " & vbCrLf)
            strSql.Append(" , CDG_Testata.Piva " & vbCrLf)
            strSql.Append(", CDG_Testata.Id_CDG " & vbCrLf)
            strSql.Append(" ,CDG_Dettagli.Id_CDG_Dettagli " & vbCrLf)
            strSql.Append(" ,0 " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Data_Inserimento  " & vbCrLf)
            strSql.Append(" , Case  " & vbCrLf)
            strSql.Append("    When CDG_Testata.Modalita_imputazione = 0 Then 'Quaderno di campagna' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.Modalita_imputazione = 1 Then 'Diretto' " & vbCrLf)
            ' Il prossimo sarebbe TimeSheet
            strSql.Append("    WHEN CDG_Testata.Modalita_imputazione = 2 Then 'Diretto'  " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.Modalita_imputazione = 3 Then 'Contabilità' " & vbCrLf)
            strSql.Append("    ELSE  ''  " & vbCrLf)
            strSql.Append("     END  " & vbCrLf)
            strSql.Append("  ,CDG_Testata.Id_Agenda " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Id_Mov " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Id_Mov_Det " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Mac_Cod  " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Cod_RisUm  " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Elem_Cod  " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Pro_Cod  " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Mat_Cod  " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Id_Attivita " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Qualifica_Cod  " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Tariffa_Cod  " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Turno_Cod  " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Conto_Cod  " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Lotto  " & vbCrLf)
            strSql.Append(" ,CDG_Testata.udm_cod " & vbCrLf)
            strSql.Append(" ,CASE   WHEN CDG_Testata.udm_cod = 0 Then 'Ore' Else ISNULL(unitamisura.udm_sim, '') END  " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Prezzo_Unitario   " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Qta   " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Valore_Totale  " & vbCrLf)
            strSql.Append(" ,100  " & vbCrLf)
            strSql.Append(", ISNULL(CDG_Testata.Descrizione, '')    " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Budget   " & vbCrLf)
            strSql.Append(" ,CASE   WHEN CDG_Testata.Budget = 0 Then 'Consuntivo' Else 'Budget' END  " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Costi_Ricavi  " & vbCrLf)
            strSql.Append(" ,CASE   WHEN CDG_Testata.Costi_Ricavi = 0 Then 'Costi' Else 'Ricavi' END  " & vbCrLf)
            strSql.Append(" ,CDG_Dettagli.Sa_Cod  " & vbCrLf)
            strSql.Append(" ,CDG_Dettagli.Appezza  " & vbCrLf)
            strSql.Append(" ,CDG_Dettagli.Id_Reg  " & vbCrLf)
            strSql.Append(" ,CDG_Dettagli.Id_Cod_reg_impianti_codici  " & vbCrLf)
            strSql.Append(" ,CDG_Dettagli.Id_Imputazione  " & vbCrLf)
            strSql.Append(" ,CDG_Dettagli.Macchine_Cod  " & vbCrLf)
            strSql.Append(" ,CDG_Dettagli.Linea_Cod  " & vbCrLf)
            strSql.Append(", ISNULL(SpecieVegetali.Veg_Cod, 0)   " & vbCrLf)
            strSql.Append(", ISNULL(Cultivar.Cul_Cod, 0)   " & vbCrLf)
            strSql.Append(", ISNULL(Operazioni.LAV_DES, ISNULL(des_lib, '')) " & vbCrLf)
            strSql.Append(", ISNULL(Parco_Macchine.Mac_Des, '')   " & vbCrLf)
            strSql.Append(", ISNULL(Contatti.Cognome, '')  + ' ' +  ISNULL(Contatti.Nome, '')   " & vbCrLf)

            strSql.Append(" , CASE  " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.cod_risum != 0 Then 'Personale' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.mac_cod != 0 Then 'Macchine e attrezzature' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 501 Then 'Altri beni' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 555 Then 'Servizi' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 2   Then 'Carburanti' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 3   Then 'Fertilizzanti' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 4   Then 'Rifiuti' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 10  Then 'Semente e Materiale Vivaistico' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 191 Then 'Formulati' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 195 Then 'Coadiuvanti' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 196 Then 'Insetti utili' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 197 Then 'Trappole' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 198 Then 'Avversità' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 300 Then 'Consistenza Zootecnica' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 200 Then 'Altre Risorse' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 201 Then 'Semilavorati Vegetali' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 204 Then 'Materie Prime Vegetali' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 205 Then 'Beni Confezionamento Vegetale' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 210 Then 'Trasformati Vegetali' " & vbCrLf)
            strSql.Append("    ELSE  ''  " & vbCrLf)
            strSql.Append("     END  " & vbCrLf)


            strSql.Append(" , CASE  " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 501 And cdg_testata.pro_cod = 0 Then ISNULL(Movimenti_Dettagli.Mov_Det_Des, '') " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 555 And cdg_testata.pro_cod != 0 Then ISNULL(Categorie.descr, '') " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 2   And cdg_testata.pro_cod != 0 Then ISNULL(Carburanti.Car_Des, '') " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 3   And cdg_testata.pro_cod != 0 Then ISNULL(fertilizzanti.fer_des, '') " & vbCrLf)
            'strSql.Append("    WHEN CDG_Testata.elem_cod = 4   And cdg_testata.pro_cod != 0 Then ISNULL(CatalogoEuropeoRifiuti.Cer_Des, '') " & vbCrLf)
            'strSql.Append("    WHEN CDG_Testata.elem_cod = 10  And cdg_testata.pro_cod != 0 Then ISNULL(TipologieSementi.Sem_Des, '') " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 191 And cdg_testata.pro_cod != 0 Then ISNULL(formulati.fr_des, '')  " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 195 And cdg_testata.pro_cod != 0 Then ISNULL(Coadiuvante.Coad_Des, '')  " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 196 And cdg_testata.pro_cod != 0 Then ISNULL(InsettiUtili.Ins_Des, '')  " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 197 And cdg_testata.pro_cod != 0 Then ISNULL(trappole.trap_des, '') " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 198 And cdg_testata.pro_cod != 0 Then ISNULL(Avversita.Av_Des_Vol, '') " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 300 And cdg_testata.pro_cod != 0 Then ISNULL(Zoo_Animali.Matricola, '') " & vbCrLf)
            strSql.Append("    WHEN (CDG_Testata.elem_cod = 10 Or CDG_Testata.elem_cod = 200 Or CDG_Testata.elem_cod = 201 Or CDG_Testata.elem_cod = 204 Or CDG_Testata.elem_cod = 205 Or CDG_Testata.elem_cod = 210) And cdg_testata.pro_cod != 0 Then ISNULL(Materie_Prime.Mat_Des, '') " & vbCrLf)
            strSql.Append("    WHEN cdg_testata.pro_cod = 0 And cdg_testata.mat_cod != '' Then ISNULL(Materie_Prime.Mat_Des, '')   " & vbCrLf)
            strSql.Append("    ELSE  ''  " & vbCrLf)
            strSql.Append("     END  " & vbCrLf)
            strSql.Append(", ISNULL(Attivita.[Desc], '')   " & vbCrLf)
            strSql.Append(" , ISNULL(Qualifica_des, '')   " & vbCrLf)
            strSql.Append(", ISNULL(Tariffa_des, '')   " & vbCrLf)
            strSql.Append(" , ISNULL(Turno_Des, '')   " & vbCrLf)
            strSql.Append(", ISNULL(Conto_Descr, '')   " & vbCrLf)
            strSql.Append(" , ISNULL(Imprese.rag_soc, '')   " & vbCrLf)
            strSql.Append(", ISNULL(IndAzienda.ind_des, '')   " & vbCrLf)
            strSql.Append(" ,ISNULL(IndAzienda.CAP, '')   " & vbCrLf)
            strSql.Append(", ISNULL(IstatAzienda.LOCALITA, '') " & vbCrLf)
            strSql.Append(" , ISNULL(IstatAzienda.COMUNI_PROV, '') " & vbCrLf)
            strSql.Append(", ISNULL(Centri_Aziendali.sa_nome, '') " & vbCrLf)
            strSql.Append(" ,  ISNULL(IndCentro.ind_des, '')   " & vbCrLf)
            strSql.Append(", ISNULL(IndCentro.CAP, '') " & vbCrLf)
            strSql.Append(" , ISNULL(ISTATCentro.LOCALITA, '')  " & vbCrLf)
            strSql.Append(" , ISNULL(ISTATCentro.COMUNI_PROV, '') " & vbCrLf)
            strSql.Append(", ISNULL(Campi.Campo_Des, '') " & vbCrLf)
            strSql.Append(" ,  ISNULL(Appezzamento.APP_NOME, '') " & vbCrLf)
            strSql.Append(" , ISNULL(Appezzamento.Validita_Inizio, " & Agro_SQL_SaveDateTime(AGRODATAINIZIO) & ")  " & vbCrLf)
            strSql.Append(" , ISNULL(Appezzamento.Validita_Fine, " & Agro_SQL_SaveDateTime(AGRODATAFINE) & ")  " & vbCrLf)
            strSql.Append(" ,  ISNULL(GruppoVegetale.Gru_Des, '')  " & vbCrLf)
            strSql.Append(", ISNULL(SpecieVegetali.Veg_Des, '')   " & vbCrLf)
            strSql.Append(" ,  ISNULL(Cultivar.Cul_Des, '')   " & vbCrLf)

            strSql.Append(", ISNULL(GruppoFinalita.Grfi_Des, '')   " & vbCrLf)

            'strSql.Append(" ,  ISNULL(( SELECT TOP 1 Codici_Anagrafe.Descrizione  " & vbCrLf)
            'strSql.Append(" From Reg_Impianti_Codici  " & vbCrLf)
            'strSql.Append(" INNER Join Codici_Anagrafe ON Reg_Impianti_Codici.id_cod = Codici_Anagrafe.codice  " & vbCrLf)
            'strSql.Append(" WHERE(Reg_Impianti_Codici.PIVA = Reg_Impianti.PIVA) " & vbCrLf)
            'strSql.Append(" And   (Reg_Impianti_Codici.Sa_Cod = Reg_Impianti.sa_cod)  " & vbCrLf)
            'strSql.Append(" And   (Reg_Impianti_Codici.Appezza  = Reg_Impianti.Appezza)  " & vbCrLf)
            'strSql.Append(" And   (Reg_Impianti_Codici.Id_Reg = Reg_Impianti.Id_Reg)  " & vbCrLf)
            'strSql.Append(" And   (Reg_Impianti_Codici.Progetto_Cod = 0)  " & vbCrLf)
            'strSql.Append(" And   (Reg_Impianti_Codici.id_cod = CDG_Dettagli.Id_Cod_reg_impianti_codici)  " & vbCrLf)
            'strSql.Append(" And (Codici_Anagrafe.gruppo = 'TERRENO')  " & vbCrLf)
            'strSql.Append(" ) , '')   " & vbCrLf)
            ' IMPOSTO DIRETTAMENTE TERRENO NUDO
            strSql.Append(", Case  When  " & vbCrLf)
            strSql.Append("   ISNULL(Reg_Impianti.Id_Reg, 0) = 0 Or ISNULL(Reg_impianti.Cul_Cod, 0) != 0 Then '' " & vbCrLf)
            strSql.Append(" Else " & vbCrLf)
            strSql.Append("  ISNULL(( SELECT TOP 1 Codici_Anagrafe.Descrizione  " & vbCrLf)
            strSql.Append(" From Reg_Impianti_Codici  " & vbCrLf)
            strSql.Append(" INNER Join Codici_Anagrafe ON Reg_Impianti_Codici.id_cod = Codici_Anagrafe.codice  " & vbCrLf)
            strSql.Append(" WHERE(Reg_Impianti_Codici.PIVA = Reg_Impianti.PIVA) " & vbCrLf)
            strSql.Append(" And   (Reg_Impianti_Codici.Sa_Cod = Reg_Impianti.sa_cod)  " & vbCrLf)
            strSql.Append(" And   (Reg_Impianti_Codici.Appezza  = Reg_Impianti.Appezza)  " & vbCrLf)
            strSql.Append(" And   (Reg_Impianti_Codici.Id_Reg = Reg_Impianti.Id_Reg)  " & vbCrLf)
            strSql.Append(" And   (Reg_Impianti_Codici.Progetto_Cod = 0)  " & vbCrLf)
            strSql.Append(" And   (Reg_Impianti_Codici.id_cod = CDG_Dettagli.Id_Cod_reg_impianti_codici)  " & vbCrLf)
            strSql.Append(" And (Codici_Anagrafe.gruppo = 'TERRENO')  " & vbCrLf)
            strSql.Append(" ) , 'TERRENO NUDO')    " & vbCrLf)
            strSql.Append(" END " & vbCrLf)

            strSql.Append(", ISNULL(Convert(varchar(10), Reg_Impianti.Validita_Inizio, 103), '')  " & vbCrLf)
            strSql.Append(" , ISNULL(CONVERT(varchar(10), Reg_Impianti.Validita_Fine, 103), '')   " & vbCrLf)

            ' INIZIO In futuro quando i costi punteranno alle distinte questa andrà sostituita con la left join asteriscata sotto
            strSql.Append(" ,  ISNULL(( SELECT TOP 1 imprese_progetti.Progetto_Nome  " & vbCrLf)
            strSql.Append(" From Imprese_Progetti  " & vbCrLf)
            strSql.Append(" WHERE(Imprese_Progetti.PIVA = Reg_Impianti.PIVA) " & vbCrLf)
            strSql.Append(" And   (Imprese_Progetti.Sa_Cod = Reg_Impianti.sa_cod)  " & vbCrLf)
            strSql.Append(" And   (Imprese_Progetti.Appezza  = Reg_Impianti.Appezza)  " & vbCrLf)
            strSql.Append(" And   (Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg)  " & vbCrLf)
            strSql.Append(" And   (Imprese_Progetti.validita_inizio >= Reg_Impianti.validita_inizio)  " & vbCrLf)
            strSql.Append(" And   (Imprese_Progetti.validita_inizio <= Reg_Impianti.validita_fine)  " & vbCrLf)
            strSql.Append(" And   (Imprese_Progetti.validita_fine >= Reg_Impianti.validita_inizio)  " & vbCrLf)
            strSql.Append(" And   (Imprese_Progetti.validita_fine <= Reg_Impianti.validita_fine)  " & vbCrLf)
            strSql.Append(" Order by imprese_progetti.validita_inizio, imprese_progetti.validita_fine " & vbCrLf)
            strSql.Append(" ) , '')   " & vbCrLf)

            strSql.Append(" ,  ISNULL(( SELECT TOP 1 Regolamenti.Reg_Des  " & vbCrLf)
            strSql.Append(" From Imprese_Progetti  " & vbCrLf)
            strSql.Append(" Left Join  Regolamenti ON Imprese_Progetti.Regolamento_Cod = Regolamenti.Reg_Cod " & vbCrLf)
            strSql.Append(" WHERE(Imprese_Progetti.PIVA = Reg_Impianti.PIVA) " & vbCrLf)
            strSql.Append(" And   (Imprese_Progetti.Sa_Cod = Reg_Impianti.sa_cod)  " & vbCrLf)
            strSql.Append(" And   (Imprese_Progetti.Appezza  = Reg_Impianti.Appezza)  " & vbCrLf)
            strSql.Append(" And   (Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg)  " & vbCrLf)
            strSql.Append(" And   (Imprese_Progetti.validita_inizio >= Reg_Impianti.validita_inizio)  " & vbCrLf)
            strSql.Append(" And   (Imprese_Progetti.validita_inizio <= Reg_Impianti.validita_fine)  " & vbCrLf)
            strSql.Append(" And   (Imprese_Progetti.validita_fine >= Reg_Impianti.validita_inizio)  " & vbCrLf)
            strSql.Append(" And   (Imprese_Progetti.validita_fine <= Reg_Impianti.validita_fine)  " & vbCrLf)
            strSql.Append(" Order by imprese_progetti.validita_inizio, imprese_progetti.validita_fine " & vbCrLf)
            strSql.Append(" ) , '')   " & vbCrLf)

            strSql.Append(" ,  ISNULL(( SELECT TOP 1 dpi_Regolamenti.NomeEsteso  " & vbCrLf)
            strSql.Append(" From Imprese_Progetti  " & vbCrLf)
            strSql.Append(" Left Join  DPI_Regolamenti on Imprese_Progetti.Disciplinare_Cod=DPI_Regolamenti.COD_REGOLAMENTO AND Imprese_Progetti.Disciplinare_PubblicoPrivato=DPI_Regolamenti.Flag_Privato_Pubblico " & vbCrLf)
            strSql.Append(" WHERE(Imprese_Progetti.PIVA = Reg_Impianti.PIVA) " & vbCrLf)
            strSql.Append(" And   (Imprese_Progetti.Sa_Cod = Reg_Impianti.sa_cod)  " & vbCrLf)
            strSql.Append(" And   (Imprese_Progetti.Appezza  = Reg_Impianti.Appezza)  " & vbCrLf)
            strSql.Append(" And   (Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg)  " & vbCrLf)
            strSql.Append(" And   (Imprese_Progetti.validita_inizio >= Reg_Impianti.validita_inizio)  " & vbCrLf)
            strSql.Append(" And   (Imprese_Progetti.validita_inizio <= Reg_Impianti.validita_fine)  " & vbCrLf)
            strSql.Append(" And   (Imprese_Progetti.validita_fine >= Reg_Impianti.validita_inizio)  " & vbCrLf)
            strSql.Append(" And   (Imprese_Progetti.validita_fine <= Reg_Impianti.validita_fine)  " & vbCrLf)
            strSql.Append(" Order by imprese_progetti.validita_inizio, imprese_progetti.validita_fine " & vbCrLf)
            strSql.Append(" ) , '')   " & vbCrLf)
            ' FINE In futuro quando i costi punteranno alle distinte questa andrà sostituita con la left join asteriscata sotto

            strSql.Append(", ISNULL(Appezzamento.ESPOSIZ, '')   " & vbCrLf)
            strSql.Append(" ,  ISNULL(Appezzamento.ubicazione, '')   " & vbCrLf)
            strSql.Append("  , ISNULL(Portinnesti.Port_Des, '')  " & vbCrLf)
            strSql.Append(" , ISNULL(ImpiantiIrrigazioni.Imp_Des, '')   " & vbCrLf)
            ' INIZIO Con queste sotto ci sarebbe duplicazione di righe se l'appezzamento è su più particelle
            'strSql.Append("         ,   ISNULL(Lista_Regioni.Regione_Des, '')  " & vbCrLf)
            'strSql.Append(" , ISNULL(ISTATParticelle.COMUNI_PROV, '')  " & vbCrLf)
            'strSql.Append(", ISNULL(ISTATParticelle.LOCALITA, '')  " & vbCrLf)
            'strSql.Append(" , ISNULL(AppezzamentiXParticelle.SEZIONE, '')   " & vbCrLf)
            'strSql.Append(" ,ISNULL(AppezzamentiXParticelle.FOGLIO, -1)   " & vbCrLf)
            'strSql.Append(" ,ISNULL(AppezzamentiXParticelle.NUMERO, -1)   " & vbCrLf)
            'strSql.Append(" ,ISNULL(AppezzamentiXParticelle.SUBALTERNO, '')  " & vbCrLf)
            strSql.Append(" , ''  " & vbCrLf)
            strSql.Append(" , ''  " & vbCrLf)
            strSql.Append(", ''  " & vbCrLf)
            strSql.Append(" , ''  " & vbCrLf)
            strSql.Append(" , -1  " & vbCrLf)
            strSql.Append(" , -1   " & vbCrLf)
            strSql.Append(" , ''  " & vbCrLf)
            ' FINE Con queste sotto ci sarebbe duplicazione di righe se l'appezzamento è su più particelle
            strSql.Append("       , ISNULL(Parco_Macchine_Input_Costi.mac_des, '')   " & vbCrLf)
            strSql.Append(" , ISNULL(Imputazione_Nome, '')   " & vbCrLf)
            strSql.Append("  , ISNULL(Imputazione_Classe_Des, '')   " & vbCrLf)
            strSql.Append(" , ISNULL(Tipo_Imputazione_Des, '')  " & vbCrLf)
            strSql.Append(" , ISNULL(Linea_Des, '')  " & vbCrLf)
            strSql.Append(" ,  ISNULL(Appezzamento.Campo_Cod, 0) " & vbCrLf)
            strSql.Append(" , CDG_Dettagli.Lotto_Input_Costi  " & vbCrLf)
            strSql.Append(" ,  0 " & vbCrLf)
            strSql.Append(" ,  '' " & vbCrLf)
            strSql.Append(", ISNULL(Convert(varchar(10), Reg_Impianti.Validita_Inizio, 103), '')  " & vbCrLf)
            strSql.Append(" , ISNULL(CONVERT(varchar(10), Reg_Impianti.Validita_Fine, 103), '')   " & vbCrLf)
            strSql.Append("  , ISNULL(Movimenti.mov_desc, '') " & vbCrLf)
            strSql.Append("  , CDG_Testata.Vecchio_Tipo_Inser_Dati " & vbCrLf)
            strSql.Append(" ,CDG_Dettagli.inviato  " & vbCrLf)
            strSql.Append(" ,CDG_Dettagli.datainvio  " & vbCrLf)
            strSql.Append(" ,CDG_Dettagli.Data_Creazione " & vbCrLf)
            strSql.Append(" ,CDG_Dettagli.Data_Modifica " & vbCrLf)
            strSql.Append(" ,CDG_Dettagli.Username_Creazione " & vbCrLf)
            strSql.Append(" ,CDG_Dettagli.Username_Modifica " & vbCrLf)
            strSql.Append(" ,CDG_Dettagli.Validita_Inizio  " & vbCrLf)
            strSql.Append(" ,CDG_Dettagli.Validita_Fine  " & vbCrLf)
            strSql.Append(" ,Imputazioni_Tipi.Tipo_Imputazione  " & vbCrLf)
            strSql.Append(" ,  0 " & vbCrLf)
            strSql.Append(" ,  0 " & vbCrLf)
            strSql.Append(" ,  '' " & vbCrLf)
            strSql.Append(" , 0 " & vbCrLf)
            strSql.Append(" , '' " & vbCrLf)
            strSql.Append(" , 0 " & vbCrLf)
            strSql.Append(" , '' " & vbCrLf)
            strSql.Append(" , 0 " & vbCrLf)
            strSql.Append(" , '' " & vbCrLf)
            strSql.Append(" , 0 " & vbCrLf)
            strSql.Append(" , '' " & vbCrLf)
            strSql.Append(" , 0 " & vbCrLf)
            strSql.Append(" , '' " & vbCrLf)
            strSql.Append(" , 0 " & vbCrLf)
            strSql.Append(" , '' " & vbCrLf)
            strSql.Append(" , 0 " & vbCrLf)
            strSql.Append(" , '' " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Modalita_Imputazione " & vbCrLf)

            strSql.Append(" , Case  " & vbCrLf)
            strSql.Append("    When ISNULL(Materie_Prime.Cod_Articolo, '') <> '' Then Materie_Prime.Cod_Articolo " & vbCrLf)
            strSql.Append("    WHEN ISNULL(Materie_Prime.Cod_Articolo, '') = '' Then  " & vbCrLf)
            'filtro ulteriormente per elem cod, perché così se non c'è alcuna codifica per quella categoria faccio meno lavoro
            strSql.AppendLine("         CASE  ")
            strSql.AppendLine("             WHEN NOT EXISTS(SELECT 1 FROM CAC_Codifica_ProdottiAziendali WHERE Elem_Cod = CDG_Testata.Elem_Cod)  ")
            strSql.AppendLine("             THEN '' ")
            strSql.AppendLine("         ELSE   ")
            strSql.AppendLine("             ISNULL((SELECT TOP(1) Cod_Prodotto_Cliente ")
            strSql.AppendLine("                  FROM CAC_Codifica_ProdottiAziendali ")
            strSql.AppendLine("             WHERE Elem_Cod = CDG_Testata.Elem_Cod ")
            strSql.AppendLine("             AND Codice_GIAS = CDG_Testata.Pro_Cod ")
            If flagJoinSuperUserCac AndAlso pivaSuperUser <> "" Then
                strSql.AppendLine("             AND Piva_SuperUser = '" & Agro_SQL_SaveText(pivaSuperUser) & "'), '') ")
            Else
                strSql.AppendLine("             AND Piva = CDG_Testata.Piva), '')  ")
            End If
            strSql.Append("    END  " & vbCrLf)
            strSql.Append(" END " & vbCrLf)
            strSql.Append(" , Case  " & vbCrLf)
            strSql.Append("    When ISNULL(Appezzamento_codici.val_Cod, '') = '' Then '' " & vbCrLf)
            strSql.Append("    WHEN ISNULL(Appezzamento_codici.val_Cod, '') = '1' Then 'Integrato' " & vbCrLf)
            strSql.Append("    WHEN ISNULL(Appezzamento_codici.val_Cod, '') = '2' Then 'In Conversione' " & vbCrLf)
            strSql.Append("    WHEN ISNULL(Appezzamento_codici.val_Cod, '') = '3' Then 'Biologico' " & vbCrLf)
            strSql.Append("    ELSE  ''  " & vbCrLf)
            strSql.Append("     END  " & vbCrLf)
            strSql.Append(" , ISNULL(Appezzamento.Sup_app, 0)   " & vbCrLf)
            strSql.Append(" , ISNULL(Reg_Impianti.Sup_imp, 0)   " & vbCrLf)
            strSql.Append(" , 0   " & vbCrLf)

            ' ---- FROM
            strSql.Append(" 		   From CDG_Testata Join CDG_Dettagli On " & vbCrLf)
            strSql.Append(" CDG_Testata.piva_superuser = CDG_Dettagli.piva_superuser And " & vbCrLf)
            strSql.Append(" CDG_Testata.piva  = CDG_Dettagli.piva  And " & vbCrLf)
            strSql.Append(" CDG_Testata.id_cdg = CDG_Dettagli.id_cdg    " & vbCrLf)
            strSql.Append("  Left Join agenda on agenda.id_agenda = CDG_Testata.id_agenda " & vbCrLf)
            strSql.Append("  Left Join operazioni on agenda.lav_cod = operazioni.lav_cod " & vbCrLf)
            strSql.Append("  Left Join movimenti on movimenti.id_mov = CDG_Testata.id_mov " & vbCrLf)
            strSql.Append("  Left Join movimenti_dettagli on movimenti_dettagli.id_mov_det = CDG_Testata.id_mov_det " & vbCrLf)
            strSql.Append("  Left Join Parco_Macchine on  Parco_Macchine.Mac_Cod = CDG_Testata.Mac_Cod " & vbCrLf)
            strSql.Append("  Left Join Risorse_Umane on  Risorse_Umane.Cod_Risum = CDG_Testata.Cod_Risum " & vbCrLf)
            strSql.Append("  Left Join contatti on CDG_Testata.piva = Contatti.Piva And Risorse_Umane.cod_contatto = contatti.cod_contatto " & vbCrLf)

            strSql.Append("  Left Join Materie_Prime on CDG_Testata.Elem_cod = Materie_Prime.Elem_cod And CDG_Testata.mat_cod = Materie_Prime.mat_cod " & vbCrLf)
            strSql.Append("  And (  " & vbCrLf)
            strSql.Append("   CDG_Testata.mat_cod != 0  " & vbCrLf)
            strSql.Append("  or CDG_Testata.pro_cod = 10 " & vbCrLf)
            strSql.Append("  or CDG_Testata.pro_cod = 200 " & vbCrLf)
            strSql.Append("  or CDG_Testata.pro_cod = 201 " & vbCrLf)
            strSql.Append("  or CDG_Testata.pro_cod = 204 " & vbCrLf)
            strSql.Append("  or CDG_Testata.pro_cod = 205 " & vbCrLf)
            strSql.Append("  or CDG_Testata.pro_cod = 210 " & vbCrLf)
            strSql.Append("  )  " & vbCrLf)

            strSql.Append("  Left Join Categorie on Categorie.COD = 'S' + Right('000000' + CONVERT(varchar(6), CDG_Testata.pro_cod), 6)  " & vbCrLf)
            strSql.Append("  And CDG_Testata.elem_cod = 555  " & vbCrLf)

            strSql.Append("  Left Join carburanti on carburanti.car_cod = CDG_Testata.pro_cod  " & vbCrLf)
            strSql.Append("  And CDG_Testata.elem_cod = 2   " & vbCrLf)

            strSql.Append("  Left Join fertilizzanti on fertilizzanti.fer_cod = CDG_Testata.pro_cod  " & vbCrLf)
            strSql.Append("  And CDG_Testata.elem_cod = 3   " & vbCrLf)

            ' Se servirà tenere presente che Cer_Cod è varchar e un e di codice è "02 01 08" strSql.Append("  Left Join CatalogoEuropeoRifiuti on CatalogoEuropeoRifiuti.Cer_Cod = CDG_Testata.pro_cod  " & vbCrLf)
            'strSql.Append("  And CDG_Testata.elem_cod = 4   " & vbCrLf)

            'strSql.Append("  Left Join TipologieSementi on TipologieSementi.sem_cod = CDG_Testata.pro_cod  " & vbCrLf)
            'strSql.Append("  And CDG_Testata.elem_cod = 10   " & vbCrLf)

            strSql.Append("  Left Join formulati on formulati.fr_cod = CDG_Testata.pro_cod  " & vbCrLf)
            strSql.Append("  And CDG_Testata.elem_cod = 191   " & vbCrLf)

            strSql.Append("  Left Join Coadiuvante on Coadiuvante.coad_cod = CDG_Testata.pro_cod  " & vbCrLf)
            strSql.Append("  And CDG_Testata.elem_cod = 195   " & vbCrLf)

            strSql.Append("  Left Join insettiutili on insettiutili.ins_cod = CDG_Testata.pro_cod  " & vbCrLf)
            strSql.Append("  And CDG_Testata.elem_cod = 196   " & vbCrLf)

            strSql.Append("  Left Join trappole on trappole.trap_cod = CDG_Testata.pro_cod  " & vbCrLf)
            strSql.Append("  And CDG_Testata.elem_cod = 197   " & vbCrLf)

            strSql.Append("  Left Join Avversita on Avversita.av_cod = CDG_Testata.pro_cod  " & vbCrLf)
            strSql.Append("  And CDG_Testata.elem_cod = 198   " & vbCrLf)

            strSql.Append("  Left Join Zoo_Animali on Zoo_Animali.piva = CDG_Testata.piva  " & vbCrLf)
            strSql.Append("  And Zoo_Animali.cod_progetto = CDG_Testata.pro_cod  " & vbCrLf)
            strSql.Append("  And CDG_Testata.elem_cod = 300   " & vbCrLf)

            strSql.Append("  Left Join Attivita on CDG_Testata.id_attivita = Attivita.id_attivita " & vbCrLf)

            strSql.Append("  Left Join Qualifiche on CDG_Testata.Piva_SuperUser = Qualifiche.Piva_SuperUser And  " & vbCrLf)
            strSql.Append("  CDG_Testata.Piva = Qualifiche.Piva And " & vbCrLf)
            strSql.Append("  CDG_Testata.Qualifica_Cod = Qualifiche.Qualifica_Cod " & vbCrLf)

            strSql.Append("  Left Join Tariffe on  CDG_Testata.Piva_SuperUser = Tariffe.Piva And  " & vbCrLf)
            strSql.Append("  CDG_Testata.Tariffa_Cod = Tariffe.Tariffa_Cod " & vbCrLf)

            strSql.Append("  Left Join Turni on  CDG_Testata.Piva_SuperUser = Turni.Piva And  " & vbCrLf)
            strSql.Append("  CDG_Testata.Turno_Cod = Turni.Turno_Cod " & vbCrLf)

            strSql.Append("  Left Join Conti on  CDG_Testata.Piva = Conti.Piva And  " & vbCrLf)
            strSql.Append("  CDG_Testata.Conto_Cod = Conti.Cod_Conto " & vbCrLf)

            strSql.Append("  Left Join Centri_Aziendali ON CDG_Dettagli.Piva = Centri_Aziendali.Piva " & vbCrLf)
            strSql.Append(" And CDG_Dettagli.Sa_Cod = Centri_Aziendali.sa_cod " & vbCrLf)

            strSql.Append(" Left Join Appezzamento ON CDG_Dettagli.Piva = Appezzamento.Piva " & vbCrLf)
            strSql.Append(" And CDG_Dettagli.Sa_Cod = Appezzamento.sa_cod " & vbCrLf)
            strSql.Append(" And CDG_Dettagli.appezza = Appezzamento.appezza  " & vbCrLf)

            strSql.Append(" Left Join Appezzamento_codici  On " & vbCrLf)
            strSql.Append("   Appezzamento_codici.piva = appezzamento.piva and  Appezzamento_codici.SA_COD = appezzamento.SA_COD And " & vbCrLf)
            strSql.Append("   Appezzamento_codici.APPEZZA = appezzamento.APPEZZA  AND Appezzamento_codici.id_cod = 1018 " & vbCrLf)

            strSql.Append(" Left Join reg_impianti ON " & vbCrLf)
            strSql.Append("  CDG_Dettagli.Piva = reg_impianti.Piva  " & vbCrLf)
            strSql.Append(" And CDG_Dettagli.Sa_Cod = reg_impianti.sa_cod  " & vbCrLf)
            strSql.Append(" And CDG_Dettagli.appezza = reg_impianti.appezza  " & vbCrLf)
            strSql.Append(" And CDG_Dettagli.Id_Reg = reg_impianti.id_reg " & vbCrLf)

            'Da utilizzare in futuro quando i costi punteranno alle distinte e non agli impianti
            'strSql.Append(" LEFT JOIN Imprese_Progetti ON Reg_Impianti.PIVA = Imprese_Progetti.Piva AND Reg_Impianti.SA_COD = Imprese_Progetti.Sa_Cod AND  " + vbCrLf)
            'strSql.Append("     Reg_Impianti.APPEZZA = Imprese_Progetti.Appezza AND Reg_Impianti.ID_REG = Imprese_Progetti.Id_Reg  " + vbCrLf)
            'strSql.Append(" And   (Imprese_Progetti.validita_inizio >= Reg_Impianti.validita_inizio)  " & vbCrLf)
            'strSql.Append(" And   (Imprese_Progetti.validita_inizio <= Reg_Impianti.validita_fine)  " & vbCrLf)
            'strSql.Append(" And   (Imprese_Progetti.validita_fine >= Reg_Impianti.validita_inizio)  " & vbCrLf)
            'strSql.Append(" And   (Imprese_Progetti.validita_fine <= Reg_Impianti.validita_fine)  " & vbCrLf)

            'strSql.Append("  Left Join  Regolamenti ON Imprese_Progetti.Regolamento_Cod = Regolamenti.Reg_Cod " & vbCrLf)

            'strSql.Append("  LEFT JOIN  DPI_Regolamenti on Imprese_Progetti.Disciplinare_Cod=DPI_Regolamenti.COD_REGOLAMENTO AND Imprese_Progetti.Disciplinare_PubblicoPrivato=DPI_Regolamenti.Flag_Privato_Pubblico " + vbCrLf)

            strSql.Append(" Left Join Imprese ON Reg_Impianti.Piva = Imprese.Piva  " & vbCrLf)

            ' INIZIO Con queste sotto ci sarebbe duplicazione di righe se l'appezzamento è su più particelle
            'strSql.Append(" Left OUTER JOIN AppezzamentiXParticelle  " & vbCrLf)
            'strSql.Append(" On Reg_Impianti.Piva = AppezzamentiXParticelle.Piva  " & vbCrLf)
            'strSql.Append(" And Reg_Impianti.Sa_Cod = AppezzamentiXParticelle.Sa_Cod  " & vbCrLf)
            'strSql.Append(" And Reg_Impianti.Appezza = AppezzamentiXParticelle.Appezza   " & vbCrLf)

            'strSql.Append(" Left OUTER JOIN ParticelleCatastali  " & vbCrLf)
            'strSql.Append(" On ParticelleCatastali.PROV = AppezzamentiXParticelle.PROV  " & vbCrLf)
            'strSql.Append(" And ParticelleCatastali.COM = AppezzamentiXParticelle.COM  " & vbCrLf)
            'strSql.Append(" And ParticelleCatastali.SEZIONE = AppezzamentiXParticelle.SEZIONE  " & vbCrLf)
            'strSql.Append(" And ParticelleCatastali.FOGLIO = AppezzamentiXParticelle.FOGLIO  " & vbCrLf)
            'strSql.Append(" And ParticelleCatastali.NUMERO = AppezzamentiXParticelle.NUMERO  " & vbCrLf)
            'strSql.Append(" And ParticelleCatastali.SUBALTERNO = AppezzamentiXParticelle.SUBALTERNO  " & vbCrLf)

            'strSql.Append("  Left OUTER JOIN ISTAT IstatParticelle ON AppezzamentiXParticelle.PROV = ISTATParticelle.PROV And AppezzamentiXParticelle.COM = ISTATParticelle.COM  " & vbCrLf)

            'strSql.Append("  Left OUTER JOIN Lista_Province ON Lista_Province.SIGLA = ISTATParticelle.Comuni_Prov  " & vbCrLf)
            'strSql.Append(" Left OUTER JOIN Lista_Regioni ON Lista_Regioni.REG = Lista_Province.REG  " & vbCrLf)
            ' FINE Con queste sotto ci sarebbe duplicazione di righe se l'appezzamento è su più particelle

            strSql.Append(" Left OUTER JOIN  Campi ON Appezzamento.Piva = Campi.Piva  " & vbCrLf)
            strSql.Append(" And Appezzamento.Sa_Cod = Campi.Sa_Cod  " & vbCrLf)
            strSql.Append(" And Appezzamento.Campo_Cod = Campi.Campo_Cod  " & vbCrLf)

            strSql.Append("  Left OUTER JOIN Portinnesti ON Reg_Impianti.Port_COD = Portinnesti.Port_Cod  " & vbCrLf)

            strSql.Append("   Left OUTER JOIN ImpiantiIrrigazioni ON Reg_Impianti.Imp_COD = ImpiantiIrrigazioni.Imp_Cod  " & vbCrLf)

            strSql.Append(" Left OUTER JOIN CentrixIndirizzi  " & vbCrLf)
            strSql.Append(" On Centri_Aziendali.Piva = CentrixIndirizzi.Piva  " & vbCrLf)
            strSql.Append(" And Centri_Aziendali.sa_cod = CentrixIndirizzi.sa_cod  " & vbCrLf)
            strSql.Append(" And CentrixIndirizzi.tipo_indirizzo = 1  " & vbCrLf)

            strSql.Append(" Left OUTER JOIN Indirizzi IndCentro ON CentrixIndirizzi.cod_indirizzo = IndCentro.cod_indirizzo  " & vbCrLf)

            strSql.Append(" Left OUTER JOIN ISTAT IstatCentro ON IndCentro.pro_cod_istat = IstatCentro.PROV And IndCentro.com_cod_istat = IstatCentro.COM  " & vbCrLf)

            strSql.Append(" Left OUTER JOIN ImpresexIndirizzi  " & vbCrLf)
            strSql.Append(" On Imprese.Piva = ImpresexIndirizzi.Piva  " & vbCrLf)
            strSql.Append(" And ImpresexIndirizzi.tipo_indirizzo = 1  " & vbCrLf)

            strSql.Append(" Left OUTER JOIN Indirizzi IndAzienda ON ImpresexIndirizzi.cod_indirizzo = IndAzienda.cod_indirizzo  " & vbCrLf)

            strSql.Append(" Left OUTER JOIN ISTAT IstatAzienda ON IndAzienda.pro_cod_istat = IstatAzienda.PROV And IndAzienda.com_cod_istat = IstatAzienda.COM  " & vbCrLf)

            strSql.Append("  Left OUTER JOIN Cultivar ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod  " & vbCrLf)

            strSql.Append(" Left OUTER JOIN SpecieVegetali  ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod  " & vbCrLf)

            strSql.Append(" Left OUTER JOIN GruppoVegetale ON SpecieVegetali.Gru_Cod = GruppoVegetale.Gru_Cod  " & vbCrLf)
            strSql.Append("  Left OUTER JOIN GruppoFinalita ON Reg_Impianti.GRFI_COD = GruppoFinalita.Grfi_Cod  " & vbCrLf)

            strSql.Append(" Left Join Parco_Macchine As Parco_Macchine_Input_Costi on  Parco_Macchine_Input_Costi.Mac_Cod = CDG_Dettagli.Macchine_Cod " & vbCrLf)

            strSql.Append(" Left Join Imputazioni on Imputazioni.Piva_SuperUser = CDG_Dettagli.Piva_SuperUser And " & vbCrLf)
            strSql.Append(" Imputazioni.Piva = CDG_Dettagli.Piva And " & vbCrLf)
            strSql.Append(" Imputazioni.Imputazione_Cod = CDG_Dettagli.Id_Imputazione " & vbCrLf)

            strSql.Append("                Left Join Imputazioni_Classi on Imputazioni_Classi.Piva_SuperUser = Imputazioni.Piva_SuperUser And " & vbCrLf)
            strSql.Append(" Imputazioni_Classi.Piva = Imputazioni.Piva And " & vbCrLf)
            strSql.Append(" Imputazioni_Classi.Imputazione_Classe_Cod = Imputazioni.Imputazione_Classe_Cod " & vbCrLf)

            strSql.Append("                Left Join Imputazioni_Tipi on Imputazioni_Tipi.Piva_SuperUser = Imputazioni.Piva_SuperUser And " & vbCrLf)
            strSql.Append(" Imputazioni_Tipi.Piva = Imputazioni.Piva And " & vbCrLf)
            strSql.Append(" Imputazioni_Tipi.Tipo_Imputazione = Imputazioni.Tipo_Imputazione " & vbCrLf)

            strSql.Append("                Left OUTER JOIN Linee_Produzioni   ON CDG_Dettagli.Piva  = Linee_Produzioni.Piva  And " & vbCrLf)
            strSql.Append(" CDG_Dettagli.Linea_Cod = Linee_Produzioni.Linea_Cod " & vbCrLf)

            strSql.Append("  INNER JOIN unitamisura ON CDG_Testata.udm_cod = unitamisura.udm_Cod  " & vbCrLf)

            'WHERE
            strSql.Append(" WHERE CDG_Testata.Budget = 0 And Vecchio_Tipo_Inser_Dati = " & Vecchio_Tipo_Inser_Dati & vbCrLf)

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri_Server, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("" & nomeRoutine & " : " & messaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function DW_Delete(ByVal piva As String,
                              ByVal Vecchio_Tipo_Inser_Dati As Integer,
                              ByVal budget As Integer,
                              ByRef objParametri As AgronicaCoreParametri
                              ) As String

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.DW_CDG_Costi_Ricavi_DAL_R.DW_Delete()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False
        Dim TabellaPrefisso As String = ""
        Try

            If budget <> 0 Then
                TabellaPrefisso = "Budget_"
            End If

            ' N.B. con questa si intercettano anche le modifiche perché quando si salvano i costi/ricavi gli id di CDG_Testata
            '      vengono riassegnati
            strSql.Length = 0

            strSql.Append(" Delete from dw_cdg_costi_ricavi Where Exists (select * from Agronica_Log_Agenda where Agronica_Log_Agenda.Piva = dw_cdg_costi_ricavi.Piva " & " ")
            strSql.Append(" and Agronica_Log_Agenda.Id_Agenda = dw_cdg_costi_ricavi.Id_Agenda " & " ")
            strSql.Append(" and Agronica_Log_Agenda.Data_Ora_RegistrazioneLog >= dw_cdg_costi_ricavi.Data_Creazione) " & " ")
            strSql.Append(" And Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            strSql.Append(" And Vecchio_Tipo_Inser_Dati  = " & Agro_SQL_SaveNum(Vecchio_Tipo_Inser_Dati) & " ")

            If budget <> -1 Then
                'Consuntivo oppure testata budget puntuale
                strSql.Append(" And budget_cons = " & budget & vbCrLf)
            Else
                'Tutte le Testate Budget 
                strSql.Append(" And budget_cons <> 0 " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            'Cancellazione delle righe con ChkUtilizzaInReport != 1
            strSql.Length = 0

            strSql.AppendLine(" Delete from dw_cdg_costi_ricavi Where Id_Cdg in   ")
            strSql.AppendLine(" (select Id_Cdg from dw_cdg_costi_ricavi ")
            strSql.AppendLine(" Join Imputazioni on Imputazioni.Piva_SuperUser = dw_cdg_costi_ricavi.Piva_SuperUser And ")
            strSql.AppendLine(" Imputazioni.Piva = dw_cdg_costi_ricavi.Piva And ")
            strSql.AppendLine(" Imputazioni.Imputazione_Cod = dw_cdg_costi_ricavi.Id_Imputazione ")
            strSql.AppendLine(" Where Imputazioni.ChkUtilizzaInReport != 1 ")
            strSql.AppendLine(" ) ")
            strSql.AppendLine(" And Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            strSql.AppendLine(" And Vecchio_Tipo_Inser_Dati  = " & Agro_SQL_SaveNum(Vecchio_Tipo_Inser_Dati) & " ")


            If budget <> -1 Then
                'Consuntivo oppure testata budget puntuale
                strSql.Append(" And budget_cons = " & budget & vbCrLf)
            Else
                'Tutte le Testate Budget 
                strSql.Append(" And budget_cons <> 0 " & vbCrLf)
            End If


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------


            'Cancellazione delle righe con specie / varietà non valorizzata (ex terreno nudo) quando su impianto ora c'è specie / varietà
            strSql.Length = 0

            strSql.AppendLine("  Delete from dw_cdg_costi_ricavi  where Id_CDG in (   ")
            strSql.AppendLine("  select id_cdg from dw_cdg_costi_ricavi where (Cul_Cod = 0 or Veg_Cod = 0) and   ")
            strSql.AppendLine(" Sa_Cod != 0 And Appezza != 0 and Id_reg != 0 ")
            strSql.AppendLine(" and exists (select * from " & TabellaPrefisso & "Reg_Impianti where ")
            strSql.AppendLine(" " & TabellaPrefisso & "Reg_Impianti.piva = DW_CDG_Costi_Ricavi.piva And " & TabellaPrefisso & "Reg_Impianti.Sa_Cod = DW_CDG_Costi_Ricavi.Sa_Cod ")
            strSql.AppendLine(" and " & TabellaPrefisso & "Reg_Impianti.Appezza = DW_CDG_Costi_Ricavi.Appezza And " & TabellaPrefisso & "Reg_Impianti.Id_reg = DW_CDG_Costi_Ricavi.Id_reg ")
            strSql.AppendLine(" and " & TabellaPrefisso & "Reg_Impianti.cul_Cod != 0) ")
            strSql.AppendLine(" And Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            strSql.AppendLine(" And Vecchio_Tipo_Inser_Dati  = " & Agro_SQL_SaveNum(Vecchio_Tipo_Inser_Dati) & " ")

            If budget <> -1 Then
                'Consuntivo oppure testata budget puntuale
                strSql.Append(" And budget_cons = " & budget & vbCrLf)
            Else
                'Tutte le Testate Budget 
                strSql.Append(" And budget_cons <> 0 " & vbCrLf)
            End If

            strSql.AppendLine(" ) ")



            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("" & nomeRoutine & " : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function DW_Delete_DaBudget(ByVal Id_Budget As Integer,
                              ByRef objParametri As AgronicaCoreParametri
                              ) As String

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.DW_CDG_Costi_Ricavi_DAL_R.DW_Delete()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            ' N.B. con questa si intercettano anche le modifiche perché quando si salvano i costi/ricavi gli id di CDG_Testata
            '      vengono riassegnati
            strSql.Length = 0

            strSql.Append(" Delete from dw_cdg_costi_ricavi Where Budget_Cons = " + Id_Budget.ToString + " ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("" & nomeRoutine & " : " & messaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function Delete_DWCdg_ByAgenda(ByVal piva As String,
                              ByVal Vecchio_Tipo_Inser_Dati As Integer,
                              ByVal idsAgenda As String,
                                          ByRef objParametri As AgronicaCoreParametri
                              ) As String

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.DW_CDG_Costi_Ricavi_DAL_R.DW_Delete()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            ' N.B. con questa si intercettano anche le modifiche perché quando si salvano i costi/ricavi gli id di CDG_Testata
            '      vengono riassegnati
            strSql.Length = 0

            strSql.Append(" Delete from dw_cdg_costi_ricavi Where ")
            strSql.Append(" Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            strSql.Append(" And Vecchio_Tipo_Inser_Dati  = " & Agro_SQL_SaveNum(Vecchio_Tipo_Inser_Dati) & " ")
            strSql.Append(" And Id_Agenda In " & Agro_SQL_Save_Clausola_IN(idsAgenda) & " ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("" & nomeRoutine & " : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function DW_Imposta_Valorizzazione_Prodotto(ByVal piva As String,
                                                       ByVal Id_Cfg_DW As Integer,
                                                       ByVal Id_CDG As Integer,
                                                       ByVal Id_CDG_Dettagli As Integer,
                                                       ByVal Prezzo_Unitario As Double,
                                                       ByRef objParametri As AgronicaCoreParametri
                                                       ) As String

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.DW_Imposta_Valorizzazione_Prodotto()"


        '====================================================================================
        'Parametri opzionali :
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try


            '---------------------------------------------
            strSql.Length = 0

            strSql.Append(" Update DW_CDG_Costi_Ricavi ")
            strSql.Append(" Set Prezzo_Unitario = " & Agro_SQL_SaveNum(Prezzo_Unitario) & ", ")
            strSql.Append("     Valore =  Qta * " & Agro_SQL_SaveNum(Prezzo_Unitario) & ", ")
            strSql.Append("	 Data_Modifica =  " & Agro_SQL_SaveDateTime(Date.Now) & " ,  ")
            strSql.Append("  Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'  ")
            strSql.Append("  Where Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            strSql.Append("  And Id_Cfg_DW = " & Id_Cfg_DW & " ")
            strSql.Append("  And Id_CDG = " & Id_CDG & " ")
            strSql.Append("  And Id_CDG_Dettagli = " & Id_CDG_Dettagli & " ")


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("" & nomeRoutine & " : " & messaggioErrore)
        End Try

        Return xRisp


    End Function


    Public Function DW_Travaso_CDG(ByVal piva As String, ByVal Id_Cfg_DW As Integer, ByVal budget As Integer,
                                   ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                   ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.DW_CDG_Scrivi.DW_Travaso_CDG()"

        '====================================================================================
        'Parametri opzionali :
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Dim pivaSuperUser = objParametri_Server.PivaSuperUser
        Dim Leggi_DW_CDG As New DW_CDG_Costi_Ricavi_DAL_R
        Dim flagJoinSuperUserCac As Boolean = Leggi_DW_CDG.GetFlagJoinCac(objParametri_Utenti)
        Dim TabellaPrefisso As String = ""
        Try

            If budget <> 0 Then
                TabellaPrefisso = "Budget_"
            End If


            '---------------------------------------------
            strSql.Length = 0

            strSql.Append(" INSERT INTO DW_CDG_Costi_Ricavi ( " & vbCrLf)
            strSql.Append("  Piva_Superuser " & vbCrLf)
            strSql.Append(" ,Piva " & vbCrLf)
            strSql.Append(" ,Id_Cfg_DW " & vbCrLf)
            strSql.Append(" ,Id_CDG " & vbCrLf)
            strSql.Append(" ,Id_CDG_Dettagli " & vbCrLf)
            strSql.Append(" ,Criterio_Analisi " & vbCrLf)
            strSql.Append(" ,Data_Inserimento " & vbCrLf)
            strSql.Append(" ,Descr_Modalita_Imputazione " & vbCrLf)
            strSql.Append(" ,Id_Agenda " & vbCrLf)
            strSql.Append(" ,Id_Mov " & vbCrLf)
            strSql.Append(" ,Id_Mov_Det " & vbCrLf)
            strSql.Append("  ,Mac_Cod " & vbCrLf)
            strSql.Append(" ,Cod_RisUm " & vbCrLf)
            strSql.Append(" ,Elem_Cod " & vbCrLf)
            strSql.Append(" ,Pro_Cod " & vbCrLf)
            strSql.Append(" ,Mat_Cod " & vbCrLf)
            strSql.Append(" ,Udm_Cod " & vbCrLf)
            strSql.Append(" ,Id_Attivita " & vbCrLf)
            strSql.Append(" ,Qualifica_Cod " & vbCrLf)
            strSql.Append(" ,Tariffa_Cod " & vbCrLf)
            strSql.Append(" ,Turno_Cod " & vbCrLf)
            strSql.Append(" ,Conto_Cod " & vbCrLf)
            strSql.Append(" ,Lotto " & vbCrLf)
            strSql.Append(" ,Mezzo " & vbCrLf)
            strSql.Append(" ,Mezzo_Des " & vbCrLf)
            strSql.Append(" ,Prezzo_Unitario " & vbCrLf)
            strSql.Append(" ,Qta " & vbCrLf)
            strSql.Append(" ,Valore " & vbCrLf)
            strSql.Append(" ,Percentuale_Ripart " & vbCrLf)
            strSql.Append(" ,Descrizione " & vbCrLf)
            strSql.Append(" ,Budget_Cons " & vbCrLf)
            strSql.Append(" ,Budget_Cons_Des " & vbCrLf)
            strSql.Append(" ,Costi_Ricavi " & vbCrLf)
            strSql.Append(" ,Costi_Ricavi_Des " & vbCrLf)
            strSql.Append(" ,Sa_Cod " & vbCrLf)
            strSql.Append(" ,Appezza " & vbCrLf)
            strSql.Append(" ,Id_Reg " & vbCrLf)
            strSql.Append(" ,Id_Cod_reg_impianti_codici " & vbCrLf)
            strSql.Append(" ,Id_Imputazione " & vbCrLf)
            strSql.Append(" ,Tipo_Imputazione " & vbCrLf)
            strSql.Append(" ,Macchine_Cod " & vbCrLf)
            strSql.Append(" ,Linea_Cod " & vbCrLf)
            strSql.Append(", Veg_Cod  " & vbCrLf)
            strSql.Append(", Cul_Cod " & vbCrLf)
            strSql.Append(" , Descrizione_Operazione " & vbCrLf)
            strSql.Append(" , Descrizione_Macchina " & vbCrLf)
            strSql.Append(" , Proprietario_Macchina " & vbCrLf)
            strSql.Append(" , Nome_Cognome " & vbCrLf)
            strSql.Append(" , NrBadge " & vbCrLf)

            strSql.Append(" , Categoria " & vbCrLf)

            strSql.Append(" , Descrizione_Prodotto " & vbCrLf)
            strSql.Append(" , Attivita " & vbCrLf)
            strSql.Append(" , Qualifica " & vbCrLf)
            strSql.Append(" , Tariffa " & vbCrLf)
            strSql.Append(" , Turno " & vbCrLf)
            strSql.Append(" , Conto " & vbCrLf)
            strSql.Append(" , Ragione_Sociale_Azienda " & vbCrLf)
            strSql.Append(" , Indirizzo_Azienda " & vbCrLf)
            strSql.Append(" , Cap_Azienda " & vbCrLf)
            strSql.Append(" , Localita_Azienda " & vbCrLf)
            strSql.Append(" , Prov_Azienda " & vbCrLf)
            strSql.Append(" , Descrizione_Centro " & vbCrLf)
            strSql.Append(" , Indirizzo_Centro " & vbCrLf)
            strSql.Append(" , Cap_Centro " & vbCrLf)
            strSql.Append(" , Localita_Centro " & vbCrLf)
            strSql.Append(" , Prov_Centro " & vbCrLf)
            strSql.Append(" , Descrizione_Campo " & vbCrLf)
            strSql.Append(" , Nome_Appezzamento " & vbCrLf)
            strSql.Append(" , Inizio_Appezzamento " & vbCrLf)
            strSql.Append(" , Fine_Appezzamento " & vbCrLf)
            strSql.Append(" , Descrizione_Gruppo_Vegetale " & vbCrLf)
            strSql.Append(" , Specie_impianto " & vbCrLf)
            strSql.Append(" , Varieta_impianto " & vbCrLf)
            strSql.Append(" , Descrizione_Finalita " & vbCrLf)
            strSql.Append(" , Destinazione_Uso " & vbCrLf)
            strSql.Append(" , Inizio_Impianto " & vbCrLf)
            strSql.Append(" , Fine_Impianto " & vbCrLf)
            strSql.Append(" , Codice_Impianto " & vbCrLf)

            strSql.Append(" , Progetto_Nome " & vbCrLf)
            strSql.Append(" , Regolamento " & vbCrLf)
            strSql.Append(" , Disciplinare " & vbCrLf)

            strSql.Append(" , Esposizione_Appezzamento " & vbCrLf)
            strSql.Append(" , Ubicazione_Appezzamento " & vbCrLf)
            strSql.Append(" , Descrizione_Portinnesti " & vbCrLf)
            strSql.Append(" , Descrizione_Irrigazioni " & vbCrLf)
            strSql.Append(" , Regione_Impianto " & vbCrLf)
            strSql.Append(" , Prov_Impianto " & vbCrLf)
            strSql.Append(" , Comune_Impianto " & vbCrLf)
            strSql.Append(" , Sezione " & vbCrLf)
            strSql.Append(" , Foglio " & vbCrLf)
            strSql.Append(" , Numero " & vbCrLf)
            strSql.Append(" , Subalterno " & vbCrLf)
            strSql.Append(" , Descrizione_Macchina_Input_Costi " & vbCrLf)
            strSql.Append(" , Progetto " & vbCrLf)
            strSql.Append(" , Classe_Progetto " & vbCrLf)
            strSql.Append(" , Tipo_Progetto " & vbCrLf)
            strSql.Append(" , Linea_Produzione " & vbCrLf)
            strSql.Append(" , Campo_Cod " & vbCrLf)
            strSql.Append(" , Lotto_Input_Costi " & vbCrLf)
            strSql.Append(" , Progetto_Cod " & vbCrLf)
            strSql.Append(" , Progetto_Des " & vbCrLf)
            strSql.Append(" , Progetto_Validita_Inizio " & vbCrLf)
            strSql.Append(" , Progetto_Validita_Fine " & vbCrLf)
            strSql.Append(" , Note " & vbCrLf)
            strSql.Append(" , Vecchio_Tipo_Inser_Dati " & vbCrLf)
            strSql.Append(" , inviato " & vbCrLf)
            strSql.Append(" , datainvio " & vbCrLf)
            strSql.Append(" , Data_Creazione " & vbCrLf)
            strSql.Append(" , Data_Modifica " & vbCrLf)
            strSql.Append(" , Username_Creazione " & vbCrLf)
            strSql.Append(" , Username_Modifica " & vbCrLf)
            strSql.Append(" , Validita_Inizio " & vbCrLf)
            strSql.Append(" , Validita_Fine " & vbCrLf)

            'Zoo
            strSql.Append(" ,Cod_Animale " & vbCrLf)
            strSql.Append(" ,Animale_Progetto " & vbCrLf)
            strSql.Append(" ,Cod_Animale_Distinta " & vbCrLf)
            strSql.Append(" ,Animale_Distinta " & vbCrLf)
            strSql.Append(" ,Sta_Num " & vbCrLf)
            strSql.Append(" ,Stalla_Des " & vbCrLf)
            strSql.Append(" ,Raggruppamento_Cod  " & vbCrLf)
            strSql.Append(" ,Raggruppamento_Des  " & vbCrLf)
            strSql.Append(" ,Specie_Animale_Cod  " & vbCrLf)
            strSql.Append(" ,Specie_Animale_des  " & vbCrLf)
            strSql.Append(" ,Razza_Animale_Cod " & vbCrLf)
            strSql.Append(" ,Razza_Animale_Des " & vbCrLf)
            strSql.Append(" ,Tipo_Animale_Cod  " & vbCrLf)
            strSql.Append(" ,Tipo_Animale_Des  " & vbCrLf)
            strSql.Append(" ,Modalita_Imputazione  " & vbCrLf)

            strSql.Append(" , Cod_Articolo " & vbCrLf)
            strSql.Append(" , MetodoProduzioneAppezzamento " & vbCrLf)
            strSql.Append(" , Sup_app " & vbCrLf)
            strSql.Append(" , Sup_imp " & vbCrLf)
            strSql.Append(" , Sup_Prog " & vbCrLf)
            strSql.Append(" , Data_Chiusura_Esercizio " & vbCrLf)
            strSql.Append(" , Codice_Appezzamento " & vbCrLf)
            strSql.Append(" , App_BIO " & vbCrLf)
            strSql.Append(" , Data_Split " & vbCrLf)

            strSql.Append(" , ID_Attivita_Gruppo1 " & vbCrLf)
            strSql.Append(" , Des_Attivita_Gruppo1 " & vbCrLf)
            strSql.Append(" , ID_Attivita_Gruppo2 " & vbCrLf)
            strSql.Append(" , Des_Attivita_Gruppo2 " & vbCrLf)
            strSql.Append(" , ID_Attivita_Gruppo3 " & vbCrLf)
            strSql.Append(" , Des_Attivita_Gruppo3 " & vbCrLf)
            strSql.Append(" , Ordine_Attivita " & vbCrLf)
            strSql.Append(" , Piva_Padre " & vbCrLf)
            strSql.Append(" , Azienda_Padre " & vbCrLf)
            strSql.Append(" , Id_Agenda_Qdc " & vbCrLf)
            strSql.Append(" , Fabbricato_Des " & vbCrLf)
            strSql.Append(" , PesoPagato " & vbCrLf)
            strSql.Append(" , PesoArrivo " & vbCrLf)
            strSql.Append(" , PesoUscito " & vbCrLf)
            strSql.Append(" ) " & vbCrLf)

            '---------------------------------------------
            '---------------     Select   ----------------
            '---------------------------------------------
            strSql.Append(" Select  " & vbCrLf)

            strSql.Append("  CDG_Testata.Piva_Superuser " & vbCrLf)
            strSql.Append(" , CDG_Testata.Piva " & vbCrLf)

            strSql.Append(" , " & Id_Cfg_DW & " " & vbCrLf)


            strSql.Append(", CDG_Testata.Id_CDG " & vbCrLf)
            strSql.Append(" ,CDG_Dettagli.Id_CDG_Dettagli " & vbCrLf)
            strSql.Append(" ,0 " & vbCrLf)
            strSql.Append(" ,Convert(date, ISNULL(Movimenti.Data_Movimento, CDG_Testata.Data_Inserimento), 120) " & vbCrLf)
            strSql.Append(" , Case  " & vbCrLf)
            strSql.Append("    When CDG_Testata.Modalita_imputazione = 0 And Cdg_Dettagli.Cod_Animale = 0 Then 'Op. QdC' " & vbCrLf)
            strSql.Append("    When CDG_Testata.Modalita_imputazione = 0 And Cdg_Dettagli.Cod_Animale != 0 Then 'Op. Zoo' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.Modalita_imputazione = 1 And CDG_Testata.OrigineAPP = 0 Then 'Diretta' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.Modalita_imputazione = 1 And CDG_Testata.OrigineAPP IN (1,2,3,4) Then 'App' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.Modalita_imputazione = 2 Then 'TimeSheet' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.Modalita_imputazione = 4 Then 'Contabilità' " & vbCrLf)
            strSql.Append("    ELSE  ''  " & vbCrLf)
            strSql.Append("     END  " & vbCrLf)
            strSql.Append("  ,CDG_Testata.Id_Agenda " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Id_Mov " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Id_Mov_Det " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Mac_Cod  " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Cod_RisUm  " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Elem_Cod  " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Pro_Cod  " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Mat_Cod  " & vbCrLf)
            strSql.Append(", ISNULL(CDG_Testata.Udm_Cod, 0)  " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Id_Attivita " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Qualifica_Cod  " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Tariffa_Cod  " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Turno_Cod  " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Conto_Cod  " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Lotto  " & vbCrLf)
            strSql.Append(" ,CDG_Testata.udm_cod " & vbCrLf)
            strSql.Append(" ,CASE   WHEN CDG_Testata.udm_cod = 0 Then 'Ore' Else ISNULL(unitamisura.udm_sim, '') END  " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Prezzo_Unitario   " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Qta / 100 * CDG_Dettagli.Valore  " & vbCrLf)
            strSql.Append(" , CDG_Testata.Valore_Totale / 100 * CDG_Dettagli.Valore " & vbCrLf)
            strSql.Append(" , CDG_Dettagli.Valore  " & vbCrLf)
            ' Imposto la descrizione solo quando vengo da libera imputazione; negli altri casi il campo Descrizione contiene la decodifica del prodotto, 
            ' della persona, del prodotto e del servizio che ottengo già con le specifiche join
            strSql.Append(", CASE   WHEN CDG_Testata.Tab_Imputazione = 'LIBERA' Then ISNULL(CDG_Testata.Descrizione, '')  Else '' END    " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Budget   " & vbCrLf)
            strSql.Append(" ,CASE   WHEN CDG_Testata.Budget = 0 Then 'Consuntivo' Else 'Budget' END  " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Costi_Ricavi  " & vbCrLf)
            strSql.Append(" ,CASE   WHEN CDG_Testata.Costi_Ricavi = 0 Then 'Costi' Else 'Ricavi' END  " & vbCrLf)
            strSql.Append(" ,CDG_Dettagli.Sa_Cod  " & vbCrLf)
            strSql.Append(" ,CDG_Dettagli.Appezza  " & vbCrLf)
            strSql.Append(" ,CDG_Dettagli.Id_Reg  " & vbCrLf)
            strSql.Append(" ,CDG_Dettagli.Id_Cod_reg_impianti_codici  " & vbCrLf)
            strSql.Append(" ,CDG_Dettagli.Id_Imputazione  " & vbCrLf)
            strSql.Append(", ISNULL(Imputazioni.Tipo_Imputazione, 0)    " & vbCrLf)
            strSql.Append(" ,CDG_Dettagli.Macchine_Cod  " & vbCrLf)
            strSql.Append(" ,CDG_Dettagli.Linea_Cod  " & vbCrLf)
            strSql.Append(", ISNULL(SpecieVegetali.Veg_Cod, 0)   " & vbCrLf)
            strSql.Append(", ISNULL(Cultivar.Cul_Cod, 0)   " & vbCrLf)
            strSql.Append(", ISNULL(des_lib, '') " & vbCrLf)
            strSql.Append(", ISNULL(Parco_Macchine.Mac_Des, '')   " & vbCrLf)
            strSql.Append(", ISNULL(Parco_Macchine.Denominazione_Proprietario, '')   " & vbCrLf)
            strSql.Append(", ISNULL(Contatti.Cognome, '')  + ' ' +  ISNULL(Contatti.Nome, '')   " & vbCrLf)
            strSql.Append(", ISNULL(Contatti.NrBadge, '') " & vbCrLf)
            strSql.Append(" , CASE  " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.cod_risum != 0 Then 'Personale' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.mac_cod != 0 Then 'Macchine e attrezzature' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 501 Then 'Altri beni' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 555 Then 'Servizi' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 2   Then 'Carburanti' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 3   Then 'Fertilizzanti' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 4   Then 'Rifiuti' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 10  Then 'Semente e Materiale Vivaistico' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 191 Then 'Formulati' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 195 Then 'Coadiuvanti' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 196 Then 'Insetti utili' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 197 Then 'Trappole' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 198 Then 'Avversità' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 200 Then 'Altre Risorse' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 201 Then 'Semilavorati Vegetali' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 204 Then 'Materie Prime Vegetali' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 205 Then 'Beni Confezionamento Vegetale' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 210 Then 'Trasformati Vegetali' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 300 Then 'Consistenza Zootecnica' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 301 Then 'Semilavorati Animali' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 304 Then 'Materie Animali' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 305 Then 'Beni Confezionamento Animale' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 306 Then 'Mangimi' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 307 Then 'Farmaci' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 310 Then 'Trasformati Animali' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 401 Then 'Ricambi' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 700 Then 'Servizi Professionali' " & vbCrLf)

            strSql.Append("    ELSE  ''  " & vbCrLf)
            strSql.Append("     END  " & vbCrLf)


            strSql.Append(" , CASE  " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 501 And cdg_testata.pro_cod = 0 Then ISNULL(Movimenti_Dettagli.Mov_Det_Des, '') " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 555 And cdg_testata.pro_cod != 0 Then ISNULL(Categorie.descr, '') " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 2   And cdg_testata.pro_cod != 0 Then ISNULL(Carburanti.Car_Des, '') " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 3   And cdg_testata.pro_cod != 0 Then ISNULL(fertilizzanti.fer_des, '') " & vbCrLf)
            'strSql.Append("    WHEN CDG_Testata.elem_cod = 4   And cdg_testata.pro_cod != 0 Then ISNULL(CatalogoEuropeoRifiuti.Cer_Des, '') " & vbCrLf)
            'strSql.Append("    WHEN CDG_Testata.elem_cod = 10  And cdg_testata.pro_cod != 0 Then ISNULL(TipologieSementi.Sem_Des, '') " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 191 And cdg_testata.pro_cod != 0 Then ISNULL(formulati.fr_des, '')  " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 195 And cdg_testata.pro_cod != 0 Then ISNULL(Coadiuvante.Coad_Des, '')  " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 196 And cdg_testata.pro_cod != 0 Then ISNULL(InsettiUtili.Ins_Des, '')  " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 197 And cdg_testata.pro_cod != 0 Then ISNULL(trappole.trap_des, '') " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 198 And cdg_testata.pro_cod != 0 Then ISNULL(Avversita.Av_Des_Vol, '') " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 300 And cdg_testata.pro_cod != 0 Then ISNULL(Zoo_Animali_Test.Matricola, '') " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod in (10, 200,201,204,205,210,301,304,305,306,307,310,401,700) And cdg_testata.pro_cod = 0 And cdg_testata.mat_cod != 0 Then ISNULL(Materie_Prime.Mat_Des, '') " & vbCrLf)
            strSql.Append("    ELSE  ''  " & vbCrLf)
            strSql.Append("     END  " & vbCrLf)
            strSql.Append(", ISNULL(Attivita.[Desc], '')   " & vbCrLf)
            strSql.Append(" , ISNULL(Qualifica_des, '')   " & vbCrLf)
            strSql.Append(", ISNULL(Tariffa_des, '')   " & vbCrLf)
            strSql.Append(" , ISNULL(Turno_Des, '')   " & vbCrLf)
            strSql.Append(", ISNULL(Conto_Descr, '')   " & vbCrLf)
            strSql.Append(" , ISNULL(Imprese.rag_soc, '')   " & vbCrLf)
            strSql.Append(", ISNULL(IndAzienda.ind_des, '')   " & vbCrLf)
            strSql.Append(" ,ISNULL(IndAzienda.CAP, '')   " & vbCrLf)
            strSql.Append(", ISNULL(IstatAzienda.LOCALITA, '') " & vbCrLf)
            strSql.Append(" , ISNULL(IstatAzienda.COMUNI_PROV, '') " & vbCrLf)
            strSql.Append(", ISNULL(Centri_Aziendali.sa_nome, '') " & vbCrLf)
            strSql.Append(" ,  ISNULL(IndCentro.ind_des, '')   " & vbCrLf)
            strSql.Append(", ISNULL(IndCentro.CAP, '') " & vbCrLf)
            strSql.Append(" , ISNULL(ISTATCentro.LOCALITA, '')  " & vbCrLf)
            strSql.Append(" , ISNULL(ISTATCentro.COMUNI_PROV, '') " & vbCrLf)
            strSql.Append(", ISNULL(" & TabellaPrefisso & "Campi.Campo_Des, '') " & vbCrLf)
            strSql.Append(" ,  ISNULL(" & TabellaPrefisso & "Appezzamento.APP_NOME, '') " & vbCrLf)
            strSql.Append(" , ISNULL(" & TabellaPrefisso & "Appezzamento.Validita_Inizio, " & Agro_SQL_SaveDateTime(AGRODATAINIZIO) & ")  " & vbCrLf)
            strSql.Append(" , ISNULL(" & TabellaPrefisso & "Appezzamento.Validita_Fine, " & Agro_SQL_SaveDateTime(AGRODATAFINE) & ")  " & vbCrLf)
            strSql.Append(" ,  ISNULL(GruppoVegetale.Gru_Des, '')  " & vbCrLf)
            strSql.Append(", ISNULL(SpecieVegetali.Veg_Des, '')   " & vbCrLf)
            strSql.Append(" ,  ISNULL(Cultivar.Cul_Des, '')   " & vbCrLf)

            strSql.Append(", ISNULL(GruppoFinalita.Grfi_Des, '')   " & vbCrLf)


            ' IMPOSTO DIRETTAMENTE TERRENO NUDO
            strSql.Append(", Case  When  " & vbCrLf)
            strSql.Append("   ISNULL(" & TabellaPrefisso & "Reg_Impianti.Id_Reg, 0) = 0 Or ISNULL(" & TabellaPrefisso & "Reg_Impianti.Cul_Cod, 0) != 0 Then '' " & vbCrLf)
            strSql.Append(" Else " & vbCrLf)
            strSql.Append("  ISNULL(( SELECT TOP 1 Codici_Anagrafe.Descrizione  " & vbCrLf)
            strSql.Append(" From " & TabellaPrefisso & "Reg_Impianti_Codici  " & vbCrLf)
            strSql.Append(" INNER Join Codici_Anagrafe WITH(NOLOCK) on " & TabellaPrefisso & "Reg_Impianti_Codici.id_cod = Codici_Anagrafe.codice  " & vbCrLf)
            strSql.Append(" WHERE(" & TabellaPrefisso & "Reg_Impianti_Codici.PIVA = " & TabellaPrefisso & "Reg_Impianti.PIVA) " & vbCrLf)
            strSql.Append(" And   (" & TabellaPrefisso & "Reg_Impianti_Codici.Sa_Cod = " & TabellaPrefisso & "Reg_Impianti.sa_cod)  " & vbCrLf)
            strSql.Append(" And   (" & TabellaPrefisso & "Reg_Impianti_Codici.Appezza  = " & TabellaPrefisso & "Reg_Impianti.Appezza)  " & vbCrLf)
            strSql.Append(" And   (" & TabellaPrefisso & "Reg_Impianti_Codici.Id_Reg = " & TabellaPrefisso & "Reg_Impianti.Id_Reg)  " & vbCrLf)
            strSql.Append(" And   (" & TabellaPrefisso & "Reg_Impianti_Codici.Progetto_Cod = 0)  " & vbCrLf)
            strSql.Append(" And   (" & TabellaPrefisso & "Reg_Impianti_Codici.id_cod = CDG_Dettagli.Id_Cod_Reg_Impianti_codici)  " & vbCrLf)
            strSql.Append(" And (Codici_Anagrafe.gruppo = 'TERRENO')  " & vbCrLf)
            strSql.Append(" ) , 'TERRENO NUDO')    " & vbCrLf)
            strSql.Append(" END " & vbCrLf)

            strSql.AppendLine(", Case  When  ")
            strSql.AppendLine("   ISNULL(" & TabellaPrefisso & "Reg_Impianti.Id_Reg, 0) != 0 ")
            strSql.AppendLine("     Then    ISNULL(Convert(varchar(10), " & TabellaPrefisso & "Reg_Impianti.Validita_Inizio, 103), '')  ")
            strSql.AppendLine("   When  ")
            strSql.AppendLine("   ISNULL(Imputazioni.Imputazione_Cod, 0) != 0 ")
            strSql.AppendLine("     Then  ISNULL(Convert(varchar(10), Imputazioni.Validita_Inizio, 103), '')  ")
            strSql.AppendLine("   When  ")
            strSql.AppendLine("   ISNULL(Zoo_Animali_Distinte.Cod_Progetto, 0) != 0 ")
            strSql.AppendLine("     Then   ISNULL(Convert(varchar(10), Zoo_Animali.Validita_Inizio, 103), '')  ")
            strSql.AppendLine(" Else " & vbCrLf)
            strSql.AppendLine(Agro_SQL_SaveDateTime(AGRODATAINIZIO))
            strSql.Append(" END " & vbCrLf)

            strSql.AppendLine(", Case  When  ")
            strSql.AppendLine("   ISNULL(" & TabellaPrefisso & "Reg_Impianti.Id_Reg, 0) != 0 ")
            strSql.AppendLine("     Then    ISNULL(CONVERT(varchar(10), " & TabellaPrefisso & "Reg_Impianti.Validita_Fine, 103), '') ")
            strSql.AppendLine("   When  ")
            strSql.AppendLine("   ISNULL(Imputazioni.Imputazione_Cod, 0) != 0 ")
            strSql.AppendLine("     Then   ISNULL(Convert(varchar(10), Imputazioni.Validita_Fine, 103), '')  ")
            strSql.AppendLine("   When  ")
            strSql.AppendLine("   ISNULL(Zoo_Animali_Distinte.Cod_Progetto, 0) != 0 ")
            strSql.AppendLine("     Then   ISNULL(Convert(varchar(10), Zoo_Animali.Validita_Fine, 103), '')  ")
            strSql.AppendLine(" Else " & vbCrLf)
            strSql.AppendLine(Agro_SQL_SaveDateTime(AGRODATAFINE))
            strSql.Append(" END " & vbCrLf)

            strSql.Append(", Case  When  " & vbCrLf)
            strSql.Append("   ISNULL(Imputazioni.Imputazione_Cod, 0) != 0 Then Imputazioni.CodicePrincipale " & vbCrLf)
            strSql.Append(" Else " & vbCrLf)
            strSql.Append("   ISNULL(" & TabellaPrefisso & "Reg_Impianti_Codici_Codice_Impianto.Val_Cod, '') " & vbCrLf)
            strSql.Append(" END " & vbCrLf)

            strSql.Append(", Case  When  " & vbCrLf)
            strSql.Append("   ISNULL(Imputazioni.Imputazione_Cod, 0) != 0 Then Imputazioni.CodiceSecondario " & vbCrLf)
            strSql.Append(" Else " & vbCrLf)
            strSql.Append("   ISNULL(" & TabellaPrefisso & "Imprese_Progetti.Progetto_Nome, '')  " & vbCrLf)
            strSql.Append(" END " & vbCrLf)

            strSql.Append(" ,  ISNULL(Regolamenti.Reg_Des, '')  " & vbCrLf)
            strSql.Append(" ,  ISNULL(dpi_Regolamenti.NomeEsteso, '')  " & vbCrLf)

            strSql.Append(", ISNULL(" & TabellaPrefisso & "Appezzamento.ESPOSIZ, '')   " & vbCrLf)
            strSql.Append(" ,  ISNULL(" & TabellaPrefisso & "Appezzamento.ubicazione, '')   " & vbCrLf)
            strSql.Append("  , ISNULL(Portinnesti.Port_Des, '')  " & vbCrLf)
            strSql.Append(" , ISNULL(ImpiantiIrrigazioni.Imp_Des, '')   " & vbCrLf)
            ' INIZIO Con queste sotto ci sarebbe duplicazione di righe se l'appezzamento è su più particelle
            'strSql.Append("         ,   ISNULL(Lista_Regioni.Regione_Des, '')  " & vbCrLf)
            'strSql.Append(" , ISNULL(ISTATParticelle.COMUNI_PROV, '')  " & vbCrLf)
            'strSql.Append(", ISNULL(ISTATParticelle.LOCALITA, '')  " & vbCrLf)
            'strSql.Append(" , ISNULL(AppezzamentiXParticelle.SEZIONE, '')   " & vbCrLf)
            'strSql.Append(" ,ISNULL(AppezzamentiXParticelle.FOGLIO, -1)   " & vbCrLf)
            'strSql.Append(" ,ISNULL(AppezzamentiXParticelle.NUMERO, -1)   " & vbCrLf)
            'strSql.Append(" ,ISNULL(AppezzamentiXParticelle.SUBALTERNO, '')  " & vbCrLf)
            strSql.Append(" , ''  " & vbCrLf)
            strSql.Append(" , ''  " & vbCrLf)
            strSql.Append(", ''  " & vbCrLf)
            strSql.Append(" , ''  " & vbCrLf)
            strSql.Append(" , -1  " & vbCrLf)
            strSql.Append(" , -1   " & vbCrLf)
            strSql.Append(" , ''  " & vbCrLf)
            ' FINE Con queste sotto ci sarebbe duplicazione di righe se l'appezzamento è su più particelle

            strSql.Append("       , ISNULL(Parco_Macchine_Input_Costi.mac_des, '')   " & vbCrLf)
            strSql.Append(" , ISNULL(Imputazione_Nome, '')   " & vbCrLf)
            strSql.Append("  , ISNULL(Imputazione_Classe_Des, '')   " & vbCrLf)
            strSql.Append(" , ISNULL(Tipo_Imputazione_Des, '')  " & vbCrLf)
            strSql.Append(" , ISNULL(Linea_Des, '')  " & vbCrLf)
            strSql.Append(" ,  ISNULL(" & TabellaPrefisso & "Appezzamento.Campo_Cod, 0) " & vbCrLf)
            strSql.Append(" , CDG_Dettagli.Lotto_Input_Costi  " & vbCrLf)
            strSql.Append(" ,  ISNULL(" & TabellaPrefisso & "Imprese_Progetti.Progetto_Cod, 0)  " & vbCrLf)
            strSql.Append(" ,  ISNULL(" & TabellaPrefisso & "Imprese_Progetti.Progetto_Des, '')  " & vbCrLf)

            strSql.AppendLine(", Case  When  ")
            strSql.AppendLine("   ISNULL(" & TabellaPrefisso & "Reg_Impianti.Id_Reg, 0) != 0 ")
            strSql.AppendLine("     Then  ISNULL(" & TabellaPrefisso & "Imprese_Progetti.validita_inizio, '')  ")
            strSql.AppendLine("   When  ")
            strSql.AppendLine("   ISNULL(Imputazioni.Imputazione_Cod, 0) != 0 ")
            strSql.AppendLine("     Then   ISNULL(Imputazioni.validita_inizio, '')  ")
            strSql.AppendLine("   When  ")
            strSql.AppendLine("   ISNULL(Zoo_Animali_Distinte.Cod_Progetto, 0) != 0 ")
            strSql.AppendLine("     Then    ISNULL(Zoo_Animali_Distinte.validita_inizio, '')  ")
            strSql.AppendLine(" Else " & vbCrLf)
            strSql.AppendLine(Agro_SQL_SaveDateTime(AGRODATAINIZIO))
            strSql.Append(" END " & vbCrLf)

            strSql.AppendLine(", Case  When  ")
            strSql.AppendLine("   ISNULL(" & TabellaPrefisso & "Reg_Impianti.Id_Reg, 0) != 0 ")
            strSql.AppendLine("     Then    ISNULL(" & TabellaPrefisso & "Imprese_Progetti.validita_fine, '')  ")
            strSql.AppendLine("   When  ")
            strSql.AppendLine("   ISNULL(Imputazioni.Imputazione_Cod, 0) != 0 ")
            strSql.AppendLine("     Then    ISNULL(Imputazioni.validita_fine, '')  ")
            strSql.AppendLine("   When  ")
            strSql.AppendLine("   ISNULL(Zoo_Animali_Distinte.Cod_Progetto, 0) != 0 ")
            strSql.AppendLine("     Then   ISNULL(Zoo_Animali_Distinte.validita_fine, '')  ")
            strSql.AppendLine(" Else " & vbCrLf)
            strSql.AppendLine(Agro_SQL_SaveDateTime(AGRODATAFINE))
            strSql.Append(" END " & vbCrLf)

            strSql.Append("  , ISNULL(Movimenti.mov_desc, '') " & vbCrLf)
            strSql.Append("  , CDG_Testata.Vecchio_Tipo_Inser_Dati " & vbCrLf)
            strSql.Append(" ,CDG_Dettagli.inviato  " & vbCrLf)
            strSql.Append(" ,CDG_Dettagli.datainvio  " & vbCrLf)
            strSql.Append("	 , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            strSql.Append("	 , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            strSql.Append("  , '" & Agro_SQL_SaveText(objParametri_Server.UsernameOperazione) & "'  ")
            strSql.Append("  , '" & Agro_SQL_SaveText(objParametri_Server.UsernameOperazione) & "'  ")
            strSql.Append("	 , " & Agro_SQL_SaveDateTime(AGRODATAINIZIO) & "  ")
            strSql.Append("	 , " & Agro_SQL_SaveDateTime(AGRODATAFINE) & "  ")

            strSql.Append(" ,CDG_Dettagli.Cod_Animale " & vbCrLf)
            strSql.Append(" ,ISNULL(Zoo_Animali.Progetto, '') " & vbCrLf)
            strSql.Append(" ,CDG_Dettagli.Cod_Animale_Distinta " & vbCrLf)
            strSql.Append(" ,ISNULL(Zoo_Animali_Distinte.Progetto_Nome, '') " & vbCrLf)
            strSql.Append(" ,CDG_Dettagli.Sta_Num  " & vbCrLf)
            strSql.Append(" ,ISNULL(Stalla.Fabbricato_DES, '')  " & vbCrLf)
            strSql.Append(" ,CDG_Dettagli.Raggruppamento_Cod  " & vbCrLf)
            strSql.Append(" ,ISNULL(Stalla_Raggruppamenti.Raggruppamento_Des, '')  " & vbCrLf)
            strSql.Append(" ,ISNULL(Zoo_Animali.SPE_COD, 0) " & vbCrLf)
            strSql.Append(" ,ISNULL(Lista_Specie_Animali.SPE_DES, '') " & vbCrLf)
            strSql.Append(" ,ISNULL(Zoo_Animali.RAZ_COD, 0) " & vbCrLf)
            strSql.Append(" ,ISNULL(Lista_Razze_Animali.RAZ_DES, '') " & vbCrLf)
            strSql.Append(" ,ISNULL(Zoo_Animali.GEN_Cod, 0)  " & vbCrLf)
            strSql.Append(" ,ISNULL(Zoo_Animali_Lista_Tipi.Tipo_Des, '')  " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Modalita_Imputazione " & vbCrLf)


            strSql.Append(" , Case  " & vbCrLf)
            strSql.Append("    When ISNULL(Materie_Prime.Cod_Articolo, '') <> '' Then Materie_Prime.Cod_Articolo " & vbCrLf)
            strSql.Append("    WHEN ISNULL(Materie_Prime.Cod_Articolo, '') = '' Then  " & vbCrLf)
            'filtro ulteriormente per elem cod, perché così se non c'è alcuna codifica per quella categoria faccio meno lavoro
            strSql.AppendLine("         CASE  ")
            strSql.AppendLine("             WHEN NOT EXISTS(SELECT 1 FROM CAC_Codifica_ProdottiAziendali WHERE Elem_Cod = CDG_Testata.Elem_Cod)  ")
            strSql.AppendLine("             THEN '' ")
            strSql.AppendLine("         ELSE   ")
            strSql.AppendLine("             ISNULL((SELECT TOP(1) Cod_Prodotto_Cliente ")
            strSql.AppendLine("                  FROM CAC_Codifica_ProdottiAziendali ")
            strSql.AppendLine("             WHERE Elem_Cod = CDG_Testata.Elem_Cod ")
            strSql.AppendLine("             AND Codice_GIAS = CDG_Testata.Pro_Cod ")
            If flagJoinSuperUserCac AndAlso pivaSuperUser <> "" Then
                strSql.AppendLine("             AND Piva_SuperUser = '" & Agro_SQL_SaveText(pivaSuperUser) & "'), '') ")
            Else
                strSql.AppendLine("             AND Piva = CDG_Testata.Piva), '')  ")
            End If
            strSql.Append("    END  " & vbCrLf)
            strSql.Append(" END " & vbCrLf)
            strSql.Append(" , Case  " & vbCrLf)
            strSql.Append("    When ISNULL(" & TabellaPrefisso & "Appezzamento_codici.val_Cod, '') = '' Then '' " & vbCrLf)
            strSql.Append("    WHEN ISNULL(" & TabellaPrefisso & "Appezzamento_codici.val_Cod, '') = '1' Then 'Integrato' " & vbCrLf)
            strSql.Append("    WHEN ISNULL(" & TabellaPrefisso & "Appezzamento_codici.val_Cod, '') = '2' Then 'In Conversione' " & vbCrLf)
            strSql.Append("    WHEN ISNULL(" & TabellaPrefisso & "Appezzamento_codici.val_Cod, '') = '3' Then 'Biologico' " & vbCrLf)
            strSql.Append("    ELSE  ''  " & vbCrLf)
            strSql.Append("     END  " & vbCrLf)
            strSql.Append(" , ISNULL(" & TabellaPrefisso & "Appezzamento.Sup_app, 0)   " & vbCrLf)
            strSql.Append(" , ISNULL(" & TabellaPrefisso & "Reg_Impianti.Sup_imp, 0)   " & vbCrLf)
            strSql.Append(" , ISNULL(" & TabellaPrefisso & "Imprese_Progetti.Sup_Prog, 0)   " & vbCrLf)

            strSql.Append(" ,ISNULL(Convert(varchar(10), " & TabellaPrefisso & "Reg_Impianti_Codici_Chiusura_Esercizio.Data_Creazione, 103), '')  " & vbCrLf)
            strSql.AppendLine(" , ISNULL(apc_cod.val_Cod, '')  ")
            strSql.AppendLine(" , ISNULL(apc_BIO.val_Cod, '')  ")
            strSql.AppendLine(" , CASE WHEN Agenda.Split = 1 Then CDG_Testata.Data_Inserimento ELSE " & Agro_SQL_SaveDateTime(AGRODATAINIZIO) & " END ")


            strSql.AppendLine(" , ISNULL(attivita_gruppi_1.ID_Attivita_Gruppo , 0)")
            strSql.AppendLine(" , ISNULL(attivita_gruppi_1.Desc_Gruppo, '') ")
            strSql.AppendLine(" , ISNULL(attivita_gruppi_2.ID_Attivita_Gruppo, 0) ")
            strSql.AppendLine(" , ISNULL(attivita_gruppi_2.Desc_Gruppo, '') ")
            strSql.AppendLine(" , ISNULL(attivita_gruppi_3.ID_Attivita_Gruppo, 0) ")
            strSql.AppendLine(" , ISNULL(attivita_gruppi_3.Desc_Gruppo, '') ")
            strSql.AppendLine(" , Attivita.Ordine ")

            'strSql.AppendLine(" , ISNULL((select padre from GerarchiaImprese where figlio  = '" & Agro_SQL_SaveText(piva) & "' ), '') ")
            'strSql.AppendLine(" , ISNULL((select rag_soc from GerarchiaImprese g_i join Imprese WITH(NOLOCK) on g_i.padre = Imprese.piva where g_i.figlio = '" & Agro_SQL_SaveText(piva) & "' ), '') ")
            'Se si cono più padri, li concateno tutti altrimenti la query va in errore per subquery che ritorna più di una riga
            strSql.AppendLine(" , ISNULL((SELECT STRING_AGG(padre, ', ') FROM GerarchiaImprese WHERE figlio = '" & Agro_SQL_SaveText(piva) & "' ), '') ")
            strSql.AppendLine(" , ISNULL((SELECT STRING_AGG(Imprese.rag_soc, ', ') FROM GerarchiaImprese g_i JOIN Imprese WITH(NOLOCK) ON g_i.padre = Imprese.piva WHERE g_i.figlio = '" & Agro_SQL_SaveText(piva) & "' ), '') ")

            strSql.AppendLine(" , ISNULL(Mov_Dettagli_Riferimenti.Id_Agenda, 0) ")
            strSql.AppendLine(" , ISNULL(Magazzino.Fabbricato_Des, '') ")
            strSql.AppendLine(" , ISNULL(md_pesi.Qta, 0) ")
            strSql.AppendLine(" , CASE WHEN CDG_Testata.Costi_Ricavi = 0 THEN ISNULL(md_pesi.Qta_Dettaglio1, 0) ELSE 0 END ")
            strSql.AppendLine(" , CASE WHEN CDG_Testata.Costi_Ricavi = 0 THEN 0 ELSE ISNULL(md_pesi.Qta_Dettaglio1, 0) END ")


            ' -------------
            ' ---- FROM
            ' -------------
            strSql.Append(" 		   From CDG_Testata WITH(NOLOCK)  Join CDG_Dettagli WITH(NOLOCK) on " & vbCrLf)
            strSql.Append(" CDG_Testata.piva_superuser = CDG_Dettagli.piva_superuser And " & vbCrLf)
            strSql.Append(" CDG_Testata.piva  = CDG_Dettagli.piva  And " & vbCrLf)
            strSql.Append(" CDG_Testata.id_cdg = CDG_Dettagli.id_cdg    " & vbCrLf)
            strSql.Append("  Join agenda WITH(NOLOCK) on agenda.id_agenda = CDG_Testata.id_agenda " & vbCrLf)

            strSql.Append("  Left Join  Mov_Dettagli_Riferimenti WITH(NOLOCK) on CDG_Testata.id_agenda = Id_Agenda_Rif " & vbCrLf)
            strSql.Append("  And Lav_Cod_Rif = 4500 " & vbCrLf)

            strSql.Append("  Left Join operazioni WITH(NOLOCK) on agenda.lav_cod = operazioni.lav_cod " & vbCrLf)
            strSql.Append("  Left Join movimenti WITH(NOLOCK) on movimenti.id_mov = CDG_Testata.id_mov " & vbCrLf)
            strSql.Append("  Left Join movimenti m_ag WITH(NOLOCK) on m_ag.id_agenda = agenda.id_agenda " & vbCrLf)
            strSql.Append("  Left Join movimenti_dettagli WITH(NOLOCK) on movimenti_dettagli.id_mov_det = CDG_Testata.id_mov_det " & vbCrLf)
            strSql.Append("  Left Join Parco_Macchine WITH(NOLOCK) on  Parco_Macchine.Mac_Cod = CDG_Testata.Mac_Cod " & vbCrLf)
            strSql.Append("  Left Join Risorse_Umane WITH(NOLOCK) on  Risorse_Umane.Cod_Risum = CDG_Testata.Cod_Risum " & vbCrLf)
            strSql.Append("  Left Join contatti WITH(NOLOCK) on (CDG_Testata.piva = Contatti.Piva OR Contatti.sa_cod = -1) And Risorse_Umane.cod_contatto = Contatti.Cod_Contatto " & vbCrLf)

            strSql.Append("  Left Join Materie_Prime WITH(NOLOCK) on CDG_Testata.Elem_cod = Materie_Prime.Elem_cod And CDG_Testata.mat_cod = Materie_Prime.mat_cod " & vbCrLf)
            strSql.Append("  And (  " & vbCrLf)
            strSql.Append("    CDG_Testata.Elem_cod = 10 " & vbCrLf)
            strSql.Append("  Or CDG_Testata.Elem_cod = 200 " & vbCrLf)
            strSql.Append("  Or CDG_Testata.Elem_cod = 201 " & vbCrLf)
            strSql.Append("  Or CDG_Testata.Elem_cod = 204 " & vbCrLf)
            strSql.Append("  Or CDG_Testata.Elem_cod = 205 " & vbCrLf)
            strSql.Append("  Or CDG_Testata.Elem_cod = 210 " & vbCrLf)
            strSql.Append("  Or CDG_Testata.Elem_cod = 301 " & vbCrLf)
            strSql.Append("  Or CDG_Testata.Elem_cod = 304 " & vbCrLf)
            strSql.Append("  Or CDG_Testata.Elem_cod = 305 " & vbCrLf)
            strSql.Append("  Or CDG_Testata.Elem_cod = 306 " & vbCrLf)
            strSql.Append("  Or CDG_Testata.Elem_cod = 307 " & vbCrLf)
            strSql.Append("  Or CDG_Testata.Elem_cod = 310 " & vbCrLf)
            strSql.Append("  Or CDG_Testata.Elem_cod = 401 " & vbCrLf)
            strSql.Append("  Or CDG_Testata.Elem_cod = 700 " & vbCrLf)
            strSql.Append("  )  " & vbCrLf)

            strSql.Append("  Left Join Categorie WITH(NOLOCK) on Categorie.COD = 'S' + Right('000000' + CONVERT(varchar(6), CDG_Testata.pro_cod), 6)  " & vbCrLf)
            strSql.Append("  And CDG_Testata.elem_cod = 555  " & vbCrLf)

            'strSql.Append("  Left Join CAC_Codifica_ProdottiAziendali WITH(NOLOCK) on  " & vbCrLf)
            'strSql.Append(" CAC_Codifica_ProdottiAziendali.piva_superuser = CDG_Dettagli.piva_superuser And " & vbCrLf)
            'strSql.Append(" CAC_Codifica_ProdottiAziendali.piva  = CDG_Dettagli.piva  And " & vbCrLf)
            'strSql.Append(" CAC_Codifica_ProdottiAziendali.Tipo_Codifica  = " & enum_Tipo_CAC_Codifica_ProdottiAziendali.NonDefinito & " And " & vbCrLf)
            'strSql.Append(" CAC_Codifica_ProdottiAziendali.Elem_Cod  = CDG_Dettagli.Elem_Cod  And " & vbCrLf)
            'strSql.Append(" CAC_Codifica_ProdottiAziendali.piva  = CDG_Dettagli.piva  And " & vbCrLf)


            strSql.AppendLine(" LEFT JOIN Fabbricati as Magazzino  WITH(NOLOCK) on Magazzino.piva = CDG_Testata.Piva ")
            strSql.AppendLine(" And Magazzino.SA_COD=CDG_Testata.Sa_Cod ")
            strSql.AppendLine(" And Magazzino.Fabbricato_Cod=CDG_Testata.Id_Destinazione ")

            strSql.Append("  Left Join carburanti WITH(NOLOCK) on carburanti.car_cod = CDG_Testata.pro_cod  " & vbCrLf)
            strSql.Append("  And CDG_Testata.elem_cod = 2   " & vbCrLf)

            strSql.Append("  Left Join fertilizzanti WITH(NOLOCK) on fertilizzanti.fer_cod = CDG_Testata.pro_cod  " & vbCrLf)
            strSql.Append("  And CDG_Testata.elem_cod = 3   " & vbCrLf)

            'Se servirà tenere presente che Cer_Cod è varchar e un e di codice è "02 01 08" strSql.Append("  Left Join CatalogoEuropeoRifiuti WITH(NOLOCK) on CatalogoEuropeoRifiuti.Cer_Cod = CDG_Testata.pro_cod  " & vbCrLf)
            'strSql.Append("  And CDG_Testata.elem_cod = 4   " & vbCrLf)

            'strSql.Append("  Left Join TipologieSementi WITH(NOLOCK) on TipologieSementi.sem_cod = CDG_Testata.pro_cod  " & vbCrLf)
            'strSql.Append("  And CDG_Testata.elem_cod = 10   " & vbCrLf)

            strSql.Append("  Left Join formulati WITH(NOLOCK) on formulati.fr_cod = CDG_Testata.pro_cod  " & vbCrLf)
            strSql.Append("  And CDG_Testata.elem_cod = 191   " & vbCrLf)

            strSql.Append("  Left Join Coadiuvante WITH(NOLOCK) on Coadiuvante.coad_cod = CDG_Testata.pro_cod  " & vbCrLf)
            strSql.Append("  And CDG_Testata.elem_cod = 195   " & vbCrLf)

            strSql.Append("  Left Join insettiutili WITH(NOLOCK) on insettiutili.ins_cod = CDG_Testata.pro_cod  " & vbCrLf)
            strSql.Append("  And CDG_Testata.elem_cod = 196   " & vbCrLf)

            strSql.Append("  Left Join trappole WITH(NOLOCK) on trappole.trap_cod = CDG_Testata.pro_cod  " & vbCrLf)
            strSql.Append("  And CDG_Testata.elem_cod = 197   " & vbCrLf)

            strSql.Append("  Left Join Avversita WITH(NOLOCK) on Avversita.av_cod = CDG_Testata.pro_cod  " & vbCrLf)
            strSql.Append("  And CDG_Testata.elem_cod = 198   " & vbCrLf)

            strSql.Append("  Left Join Zoo_Animali Zoo_Animali_Test WITH(NOLOCK) on Zoo_Animali_Test.piva = CDG_Testata.piva  " & vbCrLf)
            strSql.Append("  And Zoo_Animali_Test.cod_progetto = CDG_Testata.pro_cod  " & vbCrLf)
            strSql.Append("  And CDG_Testata.elem_cod = 300   " & vbCrLf)

            strSql.Append("  Left Join Attivita WITH(NOLOCK) on CDG_Testata.id_attivita = Attivita.id_attivita " & vbCrLf)

            strSql.AppendLine("  Left Join attivita_gruppi as attivita_gruppi_1 WITH(NOLOCK) on Attivita.ID_Attivita_Gruppo = attivita_gruppi_1.ID_Attivita_Gruppo ")
            strSql.AppendLine("  And attivita_gruppi_1.tipologia_attivita = 1   ")
            strSql.AppendLine("  Left Join attivita_gruppi as attivita_gruppi_2 WITH(NOLOCK) on Attivita.ID_Attivita_Gruppo2 = attivita_gruppi_2.ID_Attivita_Gruppo ")
            strSql.AppendLine("  And attivita_gruppi_2.tipologia_attivita = 2   ")
            strSql.AppendLine("  Left Join attivita_gruppi as attivita_gruppi_3 WITH(NOLOCK) on Attivita.ID_Attivita_Gruppo3 = attivita_gruppi_3.ID_Attivita_Gruppo ")
            strSql.AppendLine("  And attivita_gruppi_3.tipologia_attivita = 3   ")

            strSql.AppendLine("  Left Join Qualifiche WITH(NOLOCK) on CDG_Testata.Piva_SuperUser = Qualifiche.Piva_SuperUser And  ")
            strSql.AppendLine("  CDG_Testata.Piva = Qualifiche.Piva And ")
            strSql.AppendLine("  CDG_Testata.Qualifica_Cod = Qualifiche.Qualifica_Cod ")

            strSql.AppendLine("  Left Join Tariffe WITH(NOLOCK) on  CDG_Testata.Piva_SuperUser = Tariffe.Piva And  ")
            strSql.AppendLine("  CDG_Testata.Tariffa_Cod = Tariffe.Tariffa_Cod ")

            strSql.AppendLine("  Left Join Turni WITH(NOLOCK) on  CDG_Testata.Piva_SuperUser = Turni.Piva And  ")
            strSql.AppendLine("  CDG_Testata.Turno_Cod = Turni.Turno_Cod ")

            strSql.AppendLine("  Left Join Conti WITH(NOLOCK) on  CDG_Testata.Piva = Conti.Piva And  ")
            strSql.AppendLine("  CDG_Testata.Conto_Cod = Conti.Cod_Conto ")

            strSql.AppendLine("  Left Join Centri_Aziendali WITH(NOLOCK) on CDG_Dettagli.Piva = Centri_Aziendali.Piva ")
            strSql.AppendLine(" And CDG_Dettagli.Sa_Cod = Centri_Aziendali.sa_cod ")

            strSql.AppendLine(" Left Join " & TabellaPrefisso & "Appezzamento WITH(NOLOCK) on CDG_Dettagli.Piva = " & TabellaPrefisso & "Appezzamento.Piva ")
            strSql.AppendLine(" And CDG_Dettagli.Sa_Cod = " & TabellaPrefisso & "Appezzamento.sa_cod ")
            strSql.AppendLine(" And CDG_Dettagli.appezza = " & TabellaPrefisso & "Appezzamento.appezza  ")

            strSql.AppendLine(" Left Join " & TabellaPrefisso & "Reg_Impianti WITH(NOLOCK) on ")
            strSql.AppendLine("  CDG_Dettagli.Piva = " & TabellaPrefisso & "Reg_Impianti.Piva  ")
            strSql.AppendLine(" And CDG_Dettagli.Sa_Cod = " & TabellaPrefisso & "Reg_Impianti.sa_cod  ")
            strSql.AppendLine(" And CDG_Dettagli.appezza = " & TabellaPrefisso & "Reg_Impianti.appezza  ")
            strSql.AppendLine(" And CDG_Dettagli.Id_Reg = " & TabellaPrefisso & "Reg_Impianti.id_reg ")

            strSql.AppendLine(" LEFT JOIN " & TabellaPrefisso & "Imprese_Progetti WITH(NOLOCK) on CDG_Dettagli.PIVA = " & TabellaPrefisso & "Imprese_Progetti.Piva ")
            strSql.AppendLine(" And CDG_Dettagli.SA_COD = " & TabellaPrefisso & "Imprese_Progetti.Sa_Cod ")
            strSql.AppendLine(" And CDG_Dettagli.APPEZZA = " & TabellaPrefisso & "Imprese_Progetti.Appezza ")
            strSql.AppendLine(" And CDG_Dettagli.ID_REG = " & TabellaPrefisso & "Imprese_Progetti.Id_Reg  ")
            strSql.AppendLine(" And CDG_Dettagli.Progetto_Cod = " & TabellaPrefisso & "Imprese_Progetti.Progetto_Cod ")

            strSql.AppendLine("  Left Join  Regolamenti WITH(NOLOCK) on " & TabellaPrefisso & "Imprese_Progetti.Regolamento_Cod = Regolamenti.Reg_Cod ")

            strSql.AppendLine("  LEFT JOIN  DPI_Regolamenti WITH(NOLOCK) on " & TabellaPrefisso & "Imprese_Progetti.Disciplinare_Cod=DPI_Regolamenti.COD_REGOLAMENTO  ")
            strSql.AppendLine("  And " & TabellaPrefisso & "Imprese_Progetti.Disciplinare_PubblicoPrivato=DPI_Regolamenti.Flag_Privato_Pubblico ")

            strSql.AppendLine(" Left Join Imprese WITH(NOLOCK) on " & TabellaPrefisso & "Reg_Impianti.Piva = Imprese.Piva  ")

            ' INIZIO Con queste sotto ci sarebbe duplicazione di righe se l'appezzamento è su più particelle
            'strSql.Append(" Left OUTER JOIN AppezzamentiXParticelle  " & vbCrLf)
            'strSql.Append(" WITH(NOLOCK) on " & TabellaPrefisso & "Reg_Impianti.Piva = AppezzamentiXParticelle.Piva  " & vbCrLf)
            'strSql.Append(" And " & TabellaPrefisso & "Reg_Impianti.Sa_Cod = AppezzamentiXParticelle.Sa_Cod  " & vbCrLf)
            'strSql.Append(" And " & TabellaPrefisso & "Reg_Impianti.Appezza = AppezzamentiXParticelle.Appezza   " & vbCrLf)

            'strSql.Append(" Left OUTER JOIN ParticelleCatastali  " & vbCrLf)
            'strSql.Append(" WITH(NOLOCK) on ParticelleCatastali.PROV = AppezzamentiXParticelle.PROV  " & vbCrLf)
            'strSql.Append(" And ParticelleCatastali.COM = AppezzamentiXParticelle.COM  " & vbCrLf)
            'strSql.Append(" And ParticelleCatastali.SEZIONE = AppezzamentiXParticelle.SEZIONE  " & vbCrLf)
            'strSql.Append(" And ParticelleCatastali.FOGLIO = AppezzamentiXParticelle.FOGLIO  " & vbCrLf)
            'strSql.Append(" And ParticelleCatastali.NUMERO = AppezzamentiXParticelle.NUMERO  " & vbCrLf)
            'strSql.Append(" And ParticelleCatastali.SUBALTERNO = AppezzamentiXParticelle.SUBALTERNO  " & vbCrLf)

            'strSql.Append("  Left OUTER JOIN ISTAT IstatParticelle WITH(NOLOCK) on AppezzamentiXParticelle.PROV = ISTATParticelle.PROV And AppezzamentiXParticelle.COM = ISTATParticelle.COM  " & vbCrLf)

            'strSql.Append("  Left OUTER JOIN Lista_Province WITH(NOLOCK) on Lista_Province.SIGLA = ISTATParticelle.Comuni_Prov  " & vbCrLf)
            'strSql.Append(" Left OUTER JOIN Lista_Regioni WITH(NOLOCK) on Lista_Regioni.REG = Lista_Province.REG  " & vbCrLf)
            ' FINE Con queste sotto ci sarebbe duplicazione di righe se l'appezzamento è su più particelle

            strSql.Append(" Left OUTER JOIN  " & TabellaPrefisso & "Campi WITH(NOLOCK) on " & TabellaPrefisso & "Appezzamento.Piva = " & TabellaPrefisso & "Campi.Piva  " & vbCrLf)
            strSql.Append(" And " & TabellaPrefisso & "Appezzamento.Sa_Cod = " & TabellaPrefisso & "Campi.Sa_Cod  " & vbCrLf)
            strSql.Append(" And " & TabellaPrefisso & "Appezzamento.Campo_Cod = " & TabellaPrefisso & "Campi.Campo_Cod  " & vbCrLf)

            strSql.Append(" Left Join " & TabellaPrefisso & "Appezzamento_codici  WITH(NOLOCK) on " & vbCrLf)
            strSql.Append("   " & TabellaPrefisso & "Appezzamento_codici.piva = " & TabellaPrefisso & "Appezzamento.piva and  " & TabellaPrefisso & "Appezzamento_codici.SA_COD = " & TabellaPrefisso & "Appezzamento.SA_COD And " & vbCrLf)
            strSql.Append("   " & TabellaPrefisso & "Appezzamento_codici.APPEZZA = " & TabellaPrefisso & "Appezzamento.APPEZZA  AND " & TabellaPrefisso & "Appezzamento_codici.id_cod = 1018 " & vbCrLf)

            strSql.Append("  Left OUTER JOIN Portinnesti WITH(NOLOCK) on " & TabellaPrefisso & "Reg_Impianti.Port_COD = Portinnesti.Port_Cod  " & vbCrLf)

            strSql.Append("   Left OUTER JOIN ImpiantiIrrigazioni WITH(NOLOCK) on " & TabellaPrefisso & "Reg_Impianti.Imp_COD = ImpiantiIrrigazioni.Imp_Cod  " & vbCrLf)

            strSql.Append(" Left OUTER JOIN CentrixIndirizzi  " & vbCrLf)
            strSql.Append(" WITH(NOLOCK) on Centri_Aziendali.Piva = CentrixIndirizzi.Piva  " & vbCrLf)
            strSql.Append(" And Centri_Aziendali.sa_cod = CentrixIndirizzi.sa_cod  " & vbCrLf)
            strSql.Append(" And CentrixIndirizzi.tipo_indirizzo = 1  " & vbCrLf)

            strSql.Append(" Left OUTER JOIN Indirizzi IndCentro WITH(NOLOCK) on CentrixIndirizzi.cod_indirizzo = IndCentro.cod_indirizzo  " & vbCrLf)

            strSql.Append(" Left OUTER JOIN ISTAT IstatCentro WITH(NOLOCK) on IndCentro.pro_cod_istat = IstatCentro.PROV And IndCentro.com_cod_istat = IstatCentro.COM  " & vbCrLf)

            strSql.Append(" Left OUTER JOIN ImpresexIndirizzi  " & vbCrLf)
            strSql.Append(" WITH(NOLOCK) on Imprese.Piva = ImpresexIndirizzi.Piva  " & vbCrLf)
            strSql.Append(" And ImpresexIndirizzi.tipo_indirizzo = 1  " & vbCrLf)

            strSql.Append(" Left OUTER JOIN Indirizzi IndAzienda WITH(NOLOCK) on ImpresexIndirizzi.cod_indirizzo = IndAzienda.cod_indirizzo  " & vbCrLf)

            strSql.Append(" Left OUTER JOIN ISTAT IstatAzienda WITH(NOLOCK) on IndAzienda.pro_cod_istat = IstatAzienda.PROV And IndAzienda.com_cod_istat = IstatAzienda.COM  " & vbCrLf)

            strSql.Append("  Left OUTER JOIN Cultivar WITH(NOLOCK) on " & TabellaPrefisso & "Reg_Impianti.CUL_COD = Cultivar.Cul_Cod  " & vbCrLf)

            strSql.Append(" Left OUTER JOIN SpecieVegetali  WITH(NOLOCK) on SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod  " & vbCrLf)

            strSql.Append(" Left OUTER JOIN GruppoVegetale WITH(NOLOCK) on SpecieVegetali.Gru_Cod = GruppoVegetale.Gru_Cod  " & vbCrLf)
            strSql.Append("  Left OUTER JOIN GruppoFinalita WITH(NOLOCK) on " & TabellaPrefisso & "Reg_Impianti.GRFI_COD = GruppoFinalita.Grfi_Cod  " & vbCrLf)

            strSql.Append(" Left Join Parco_Macchine As Parco_Macchine_Input_Costi WITH(NOLOCK) on  Parco_Macchine_Input_Costi.Mac_Cod = CDG_Dettagli.Macchine_Cod " & vbCrLf)

            strSql.Append(" Left Join Imputazioni WITH(NOLOCK) on Imputazioni.Piva_SuperUser = CDG_Dettagli.Piva_SuperUser And " & vbCrLf)
            strSql.Append(" Imputazioni.Piva = CDG_Dettagli.Piva And " & vbCrLf)
            strSql.Append(" Imputazioni.Imputazione_Cod = CDG_Dettagli.Id_Imputazione " & vbCrLf)

            strSql.Append("                Left Join Imputazioni_Classi WITH(NOLOCK) on Imputazioni_Classi.Piva_SuperUser = Imputazioni.Piva_SuperUser And " & vbCrLf)
            strSql.Append(" Imputazioni_Classi.Piva = Imputazioni.Piva And " & vbCrLf)
            strSql.Append(" Imputazioni_Classi.Imputazione_Classe_Cod = Imputazioni.Imputazione_Classe_Cod " & vbCrLf)

            strSql.Append("                Left Join Imputazioni_Tipi WITH(NOLOCK) on Imputazioni_Tipi.Piva_SuperUser = Imputazioni.Piva_SuperUser And " & vbCrLf)
            strSql.Append(" Imputazioni_Tipi.Piva = Imputazioni.Piva And " & vbCrLf)
            strSql.Append(" Imputazioni_Tipi.Tipo_Imputazione = Imputazioni.Tipo_Imputazione " & vbCrLf)

            strSql.Append("                Left OUTER JOIN Linee_Produzioni   WITH(NOLOCK) on CDG_Dettagli.Piva  = Linee_Produzioni.Piva  And " & vbCrLf)
            strSql.Append(" CDG_Dettagli.Linea_Cod = Linee_Produzioni.Linea_Cod " & vbCrLf)

            strSql.Append("  INNER JOIN unitamisura WITH(NOLOCK) on CDG_Testata.udm_cod = unitamisura.udm_Cod  " & vbCrLf)

            'Zoo
            strSql.Append("  Left Join Zoo_Animali WITH(NOLOCK) on Zoo_Animali.piva = CDG_Dettagli.piva  " & vbCrLf)
            strSql.Append("  And Zoo_Animali.cod_progetto = CDG_Dettagli.Cod_Animale  " & vbCrLf)

            strSql.AppendLine(" LEFT JOIN Zoo_Animali_Distinte WITH(NOLOCK) on Zoo_Animali.PIVA = Zoo_Animali_Distinte.PIVA " & vbCrLf)
            strSql.AppendLine(" And Zoo_animali.Cod_Progetto = Zoo_Animali_Distinte.Cod_Animale " & vbCrLf)
            strSql.AppendLine(" And CDG_Dettagli.Cod_Animale_Distinta = Zoo_Animali_Distinte.Cod_Progetto  " & vbCrLf)

            strSql.AppendLine(" LEFT JOIN Fabbricati as Stalla  WITH(NOLOCK) on Stalla.piva = CDG_Dettagli.Piva " & vbCrLf)
            strSql.AppendLine(" And Stalla.SA_COD=CDG_Dettagli.Sa_Cod " & vbCrLf)
            strSql.AppendLine(" And Stalla.Fabbricato_Cod=CDG_Dettagli.Sta_Num " & vbCrLf)

            strSql.AppendLine(" Left Join Stalla_Raggruppamenti WITH(NOLOCK) on CDG_Dettagli.Sa_Cod = Stalla_Raggruppamenti.sa_cod " & vbCrLf)
            strSql.AppendLine(" AND CDG_Dettagli.Raggruppamento_Cod = Stalla_Raggruppamenti.Raggruppamento_Cod " & vbCrLf)

            strSql.AppendLine(" Left JOIN Lista_Specie_Animali WITH(NOLOCK) on Zoo_Animali.GEN_COD = Lista_Specie_Animali.GEN_COD " & vbCrLf)
            strSql.AppendLine(" And Zoo_Animali.SPE_COD = Lista_Specie_Animali.SPE_COD " & vbCrLf)

            strSql.AppendLine(" Left JOIN Lista_Razze_Animali WITH(NOLOCK) on Zoo_Animali.GEN_COD = Lista_Razze_Animali.GEN_COD " & vbCrLf)
            strSql.AppendLine(" And Zoo_Animali.SPE_COD = Lista_Razze_Animali.SPE_COD " & vbCrLf)
            strSql.AppendLine(" And Zoo_Animali.RAZ_COD = Lista_Razze_Animali.RAZ_COD " & vbCrLf)

            strSql.AppendLine(" Left JOIN Zoo_Animali_Lista_Tipi WITH(NOLOCK) on Zoo_Animali.GEN_COD = Zoo_Animali_Lista_Tipi.GEN_COD " & vbCrLf)
            strSql.AppendLine("  And Zoo_Animali.SPE_COD = Zoo_Animali_Lista_Tipi.SPE_COD " & vbCrLf)
            strSql.AppendLine("  And Zoo_Animali.TIPO_COD = Zoo_Animali_Lista_Tipi.TIPO_COD " & vbCrLf)

            strSql.Append("  LEFT OUTER JOIN  " & TabellaPrefisso & "Reg_Impianti_Codici As " & TabellaPrefisso & "Reg_Impianti_Codici_Codice_Impianto ")
            strSql.Append(" WITH(NOLOCK) on " & TabellaPrefisso & "Reg_Impianti.PIVA=" & TabellaPrefisso & "Reg_Impianti_Codici_Codice_Impianto.PIVA And " & TabellaPrefisso & "Reg_Impianti.SA_COD=" & TabellaPrefisso & "Reg_Impianti_Codici_Codice_Impianto.SA_COD And " & TabellaPrefisso & "Reg_Impianti.APPEZZA=" & TabellaPrefisso & "Reg_Impianti_Codici_Codice_Impianto.APPEZZA And " & TabellaPrefisso & "Reg_Impianti.id_reg=" & TabellaPrefisso & "Reg_Impianti_Codici_Codice_Impianto.id_reg And " & TabellaPrefisso & "Reg_Impianti_Codici_Codice_Impianto.id_cod = " & enum_CodiciAnagrafe.Codice_Impianto & " " & vbCrLf)

            strSql.Append("  LEFT OUTER JOIN  " & TabellaPrefisso & "Reg_Impianti_Codici As " & TabellaPrefisso & "Reg_Impianti_Codici_Chiusura_Esercizio ")
            strSql.Append(" WITH(NOLOCK) on " & TabellaPrefisso & "Imprese_Progetti.PIVA=" & TabellaPrefisso & "Reg_Impianti_Codici_Chiusura_Esercizio.PIVA And " & TabellaPrefisso & "Imprese_Progetti.SA_COD=" & TabellaPrefisso & "Reg_Impianti_Codici_Chiusura_Esercizio.SA_COD And " & TabellaPrefisso & "Imprese_Progetti.APPEZZA=" & TabellaPrefisso & "Reg_Impianti_Codici_Chiusura_Esercizio.APPEZZA And " & TabellaPrefisso & "Imprese_Progetti.id_reg=" & TabellaPrefisso & "Reg_Impianti_Codici_Chiusura_Esercizio.id_reg And " & TabellaPrefisso & "Imprese_Progetti.progetto_cod=" & TabellaPrefisso & "Reg_Impianti_Codici_Chiusura_Esercizio.progetto_cod And " & TabellaPrefisso & "Reg_Impianti_Codici_Chiusura_Esercizio.id_cod = " & enum_CodiciAnagrafe.Distinta_Chiusa & " And " & TabellaPrefisso & "Reg_Impianti_Codici_Chiusura_Esercizio.val_cod = 1 " & vbCrLf)

            strSql.AppendLine(" Left Join " & TabellaPrefisso & "Appezzamento_Codici apc_BIO WITH(NOLOCK) on apc_BIO.piva = " & TabellaPrefisso & "Appezzamento.piva And apc_BIO.sa_cod = " & TabellaPrefisso & "Appezzamento.sa_cod ")
            strSql.AppendLine(" And apc_BIO.appezza = " & TabellaPrefisso & "Appezzamento.appezza  And apc_BIO.id_cod = " & enum_CodiciAnagrafe.Codice_Appezza_Biologico)

            strSql.AppendLine(" Left Join " & TabellaPrefisso & "Appezzamento_Codici apc_cod WITH(NOLOCK) on apc_cod.piva = " & TabellaPrefisso & "Appezzamento.piva And apc_cod.sa_cod = " & TabellaPrefisso & "Appezzamento.sa_cod ")
            strSql.AppendLine(" And apc_cod.appezza = " & TabellaPrefisso & "Appezzamento.appezza And apc_cod.id_cod = " & enum_CodiciAnagrafe.Riferimento_Alfanumerico_Appezzamento)

            strSql.Append("  Left Join Mov_Dettagli_Riferimenti mdr_pesi WITH(NOLOCK) on CDG_Testata.id_agenda = mdr_pesi.Id_Agenda_Rif " & vbCrLf)
            strSql.Append("  And mdr_pesi.Lav_Cod_Rif = 4500 " & vbCrLf)
            strSql.Append("  Left Join movimenti m_pesi WITH(NOLOCK) on m_pesi.Id_Agenda = mdr_pesi.Id_Agenda " & vbCrLf)
            strSql.Append("  And m_pesi.Cau_Mov = '3800' " & vbCrLf)
            strSql.Append("  OUTER APPLY( SELECT TOP 1 * FROM movimenti_dettagli md_pesi WITH(NOLOCK) WHERE md_pesi.Id_Mov = m_pesi.Id_Mov) md_pesi " & vbCrLf)

            'WHERE
            strSql.Append(" WHERE Vecchio_Tipo_Inser_Dati = 0" & vbCrLf)
            strSql.Append(" And cdg_Testata.piva = '" & Agro_SQL_SaveText(piva) & "'" & vbCrLf)


            If budget <> -1 Then
                'Consuntivo oppure testata budget puntuale
                strSql.Append(" And cdg_Testata.budget = " & budget & vbCrLf)
            Else
                'Tutte le Testate Budget 
                strSql.Append(" And cdg_Testata.budget <> 0 " & vbCrLf)
            End If

            strSql.Append(" And ISNull(Imputazioni.ChkUtilizzaInReport, 1) = 1 ")
            'Escludo i cdg già presenti
            strSql.Append(" And Not Exists (select * from dw_cdg_costi_ricavi as DW WITH(NOLOCK) where DW.Piva = cdg_Testata.Piva " & " ")
            strSql.Append(" And DW.Id_CDG = cdg_Testata.Id_CDG) " & " ")

            If LivelloCompatibilita(objParametri_Server) >= 150 Then
                strSql.AppendLine(" OPTION (USE HINT ('FORCE_LEGACY_CARDINALITY_ESTIMATION')) ")
            End If


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri_Server, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("" & nomeRoutine & " : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function DW_Aggiorna_Valore_Raccolto_QDC(ByVal piva As String, ByVal budget As Integer, ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                     ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.DW_CDG_Scrivi.DW_Aggiorna_Valore_Raccolto_QDC()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False
        Dim flagConnessioneLocale As Boolean = False
        Dim flagTransazioneLocale As Boolean = False

        Dim pivaSuperUser = objParametri_Server.PivaSuperUser
        Dim Leggi_DW_CDG As New DW_CDG_Costi_Ricavi_DAL_R
        Dim flagJoinSuperUserCac As Boolean = Leggi_DW_CDG.GetFlagJoinCac(objParametri_Utenti)

        Try

            ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)

            '==========================================================================================================================================================================
            '  FASE 1 - Cancello da DW_CDG_Costi_Ricavi tutte le righe di tipo Ricavo calcolato da raccolta in cui ID_Agenda non è presente in Agenda
            '==========================================================================================================================================================================

            strSql.Length = 0

            strSql.Append(" DELETE DW_CDG_Costi_Ricavi ")
            strSql.Append("  Where  Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            strSql.Append("  And Descr_Modalita_Imputazione = 'Quaderno di campagna' ")
            strSql.Append("  And Costi_Ricavi = 1 ")
            strSql.Append("  And ID_CDG = 0 ")
            strSql.Append("  And Id_CDG_Dettagli = 0 ")
            strSql.Append("  And Vecchio_Tipo_Inser_Dati = 0 ")
            strSql.Append("  And ID_Agenda Not IN ")
            strSql.Append("  (Select ID_Agenda from Agenda where Agenda.Piva = '" & Agro_SQL_SaveText(piva) & "') ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri_Server, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            '==========================================================================================================================================================================
            '  FASE 2 - Cancello da DW_CDG_Costi_Ricavi tutte le righe di tipo Ricavo calcolato da raccolta in cui Data ultima modifica è precedente a data modifica di Agenda
            '==========================================================================================================================================================================

            If xRisp Then

                strSql.Length = 0

                strSql.Append(" DELETE DW_CDG_Costi_Ricavi ")
                strSql.Append("  Where  Piva = '" & Agro_SQL_SaveText(piva) & "' ")
                strSql.Append("  And Descr_Modalita_Imputazione = 'Quaderno di campagna' ")
                strSql.Append("  And Costi_Ricavi = 1 ")
                strSql.Append("  And ID_CDG = 0 ")
                strSql.Append("  And Id_CDG_Dettagli = 0 ")
                strSql.Append("  And Vecchio_Tipo_Inser_Dati = 0 ")
                strSql.Append("  And ID_Agenda IN ")
                strSql.Append("  (Select ID_Agenda from Agenda where Agenda.Piva = '" & Agro_SQL_SaveText(piva) & "' and Agenda.Data_Modifica >= DW_CDG_Costi_Ricavi.Data_Modifica) ")

                '--------------------------------------------------------------------------
                xRisp = EseguiQuery_Scrittura(objParametri_Server, strSql.ToString, nomeRoutine)
                '--------------------------------------------------------------------------
            End If

            '==========================================================================================================================================================================
            '  FASE 3 - Inserisco tutte quelle mancanti
            '==========================================================================================================================================================================
            If xRisp Then

                ' TODO Cercare decodifica del budget
                Dim Budget_Cons_Des = "Consuntivo"
                If budget <> 0 Then
                    Budget_Cons_Des = "Budget"
                End If

                strSql.Length = 0

                strSql.Append(" INSERT INTO DW_CDG_Costi_Ricavi ( " & vbCrLf)
                strSql.Append("  Piva_Superuser " & vbCrLf)
                strSql.Append(" ,Piva " & vbCrLf)
                strSql.Append(" ,Id_CDG " & vbCrLf)
                strSql.Append(" ,Id_CDG_Dettagli " & vbCrLf)
                strSql.Append(" ,Criterio_Analisi " & vbCrLf)
                strSql.Append(" ,Data_Inserimento " & vbCrLf)
                strSql.Append(" ,Descr_Modalita_Imputazione " & vbCrLf)
                strSql.Append(" ,Id_Agenda " & vbCrLf)
                strSql.Append(" ,Id_Mov " & vbCrLf)
                strSql.Append(" ,Id_Mov_Det " & vbCrLf)
                strSql.Append("  ,Mac_Cod " & vbCrLf)
                strSql.Append(" ,Cod_RisUm " & vbCrLf)
                strSql.Append(" ,Elem_Cod " & vbCrLf)
                strSql.Append(" ,Pro_Cod " & vbCrLf)
                strSql.Append(" ,Mat_Cod " & vbCrLf)
                strSql.Append(" ,Id_Attivita " & vbCrLf)
                strSql.Append(" ,Qualifica_Cod " & vbCrLf)
                strSql.Append(" ,Tariffa_Cod " & vbCrLf)
                strSql.Append(" ,Turno_Cod " & vbCrLf)
                strSql.Append(" ,Conto_Cod " & vbCrLf)
                strSql.Append(" ,Lotto " & vbCrLf)
                strSql.Append(" ,Mezzo " & vbCrLf)
                strSql.Append(" ,Mezzo_Des " & vbCrLf)
                strSql.Append(" ,Prezzo_Unitario " & vbCrLf)
                strSql.Append(" ,Qta " & vbCrLf)
                strSql.Append(" ,Valore " & vbCrLf)
                strSql.Append(" ,Percentuale_Ripart " & vbCrLf)
                strSql.Append(" ,Descrizione " & vbCrLf)
                strSql.Append(" ,Budget_Cons " & vbCrLf)
                strSql.Append(" ,Budget_Cons_Des " & vbCrLf)
                strSql.Append(" ,Costi_Ricavi " & vbCrLf)
                strSql.Append(" ,Costi_Ricavi_Des " & vbCrLf)
                strSql.Append(" ,Sa_Cod " & vbCrLf)
                strSql.Append(" ,Appezza " & vbCrLf)
                strSql.Append(" ,Id_Reg " & vbCrLf)
                strSql.Append(" ,Id_Cod_reg_impianti_codici " & vbCrLf)
                strSql.Append(" ,Id_Imputazione " & vbCrLf)
                strSql.Append(" , Tipo_Imputazione " & vbCrLf)
                strSql.Append(" ,Macchine_Cod " & vbCrLf)
                strSql.Append(" ,Linea_Cod " & vbCrLf)
                strSql.Append(", Veg_Cod  " & vbCrLf)
                strSql.Append(", Cul_Cod " & vbCrLf)
                strSql.Append(" , Descrizione_Operazione " & vbCrLf)
                strSql.Append(" , Descrizione_Macchina " & vbCrLf)
                strSql.Append(" , Nome_Cognome " & vbCrLf)
                strSql.Append(" , NrBadge " & vbCrLf)

                strSql.Append(" , Categoria " & vbCrLf)

                strSql.Append(" , Descrizione_Prodotto " & vbCrLf)
                strSql.Append(" , Attivita " & vbCrLf)
                strSql.Append(" , Qualifica " & vbCrLf)
                strSql.Append(" , Tariffa " & vbCrLf)
                strSql.Append(" , Turno " & vbCrLf)
                strSql.Append(" , Conto " & vbCrLf)
                strSql.Append(" , Ragione_Sociale_Azienda " & vbCrLf)
                strSql.Append(" , Indirizzo_Azienda " & vbCrLf)
                strSql.Append(" , Cap_Azienda " & vbCrLf)
                strSql.Append(" , Localita_Azienda " & vbCrLf)
                strSql.Append(" , Prov_Azienda " & vbCrLf)
                strSql.Append(" , Descrizione_Centro " & vbCrLf)
                strSql.Append(" , Indirizzo_Centro " & vbCrLf)
                strSql.Append(" , Cap_Centro " & vbCrLf)
                strSql.Append(" , Localita_Centro " & vbCrLf)
                strSql.Append(" , Prov_Centro " & vbCrLf)
                strSql.Append(" , Descrizione_Campo " & vbCrLf)
                strSql.Append(" , Nome_Appezzamento " & vbCrLf)
                strSql.Append(" , Inizio_Appezzamento " & vbCrLf)
                strSql.Append(" , Fine_Appezzamento " & vbCrLf)
                strSql.Append(" , Descrizione_Gruppo_Vegetale " & vbCrLf)
                strSql.Append(" , Specie_impianto " & vbCrLf)
                strSql.Append(" , Varieta_impianto " & vbCrLf)
                strSql.Append(" , Descrizione_Finalita " & vbCrLf)
                strSql.Append(" , Destinazione_Uso " & vbCrLf)
                strSql.Append(" , Inizio_Impianto " & vbCrLf)
                strSql.Append(" , Fine_Impianto " & vbCrLf)

                strSql.Append(" , Progetto_Nome " & vbCrLf)
                strSql.Append(" , Regolamento " & vbCrLf)
                strSql.Append(" , Disciplinare " & vbCrLf)

                strSql.Append(" , Esposizione_Appezzamento " & vbCrLf)
                strSql.Append(" , Ubicazione_Appezzamento " & vbCrLf)
                strSql.Append(" , Descrizione_Portinnesti " & vbCrLf)
                strSql.Append(" , Descrizione_Irrigazioni " & vbCrLf)
                strSql.Append(" , Regione_Impianto " & vbCrLf)
                strSql.Append(" , Prov_Impianto " & vbCrLf)
                strSql.Append(" , Comune_Impianto " & vbCrLf)
                strSql.Append(" , Sezione " & vbCrLf)
                strSql.Append(" , Foglio " & vbCrLf)
                strSql.Append(" , Numero " & vbCrLf)
                strSql.Append(" , Subalterno " & vbCrLf)
                strSql.Append(" , Descrizione_Macchina_Input_Costi " & vbCrLf)
                strSql.Append(" , Progetto " & vbCrLf)
                strSql.Append(" , Classe_Progetto " & vbCrLf)
                strSql.Append(" , Tipo_Progetto " & vbCrLf)
                strSql.Append(" , Linea_Produzione " & vbCrLf)
                strSql.Append(" , Campo_Cod " & vbCrLf)
                strSql.Append(" , Lotto_Input_Costi " & vbCrLf)
                strSql.Append(" , Progetto_Cod " & vbCrLf)
                strSql.Append(" , Progetto_Des " & vbCrLf)
                strSql.Append(" , Progetto_Validita_Inizio " & vbCrLf)
                strSql.Append(" , Progetto_Validita_Fine " & vbCrLf)
                strSql.Append(" , Note " & vbCrLf)
                strSql.Append(" , Vecchio_Tipo_Inser_Dati " & vbCrLf)
                strSql.Append(" , inviato " & vbCrLf)
                strSql.Append(" , datainvio " & vbCrLf)
                strSql.Append(" , Data_Creazione " & vbCrLf)
                strSql.Append(" , Data_Modifica " & vbCrLf)
                strSql.Append(" , Username_Creazione " & vbCrLf)
                strSql.Append(" , Username_Modifica " & vbCrLf)
                strSql.Append(" , Validita_Inizio " & vbCrLf)
                strSql.Append(" , Validita_Fine " & vbCrLf)

                strSql.Append(" , Id_Cfg_DW " & vbCrLf)
                strSql.Append(" , Udm_Cod " & vbCrLf)
                strSql.Append(" , Modalita_Imputazione " & vbCrLf)

                strSql.Append(" , Animale_Distinta " & vbCrLf)
                strSql.Append(" , Animale_Progetto " & vbCrLf)
                strSql.Append(" , Cod_Animale " & vbCrLf)
                strSql.Append(" , Cod_Animale_Distinta " & vbCrLf)
                strSql.Append(" , Codice_Impianto " & vbCrLf)
                strSql.Append(" , Raggruppamento_Cod " & vbCrLf)
                strSql.Append(" , Raggruppamento_Des " & vbCrLf)
                strSql.Append(" , Razza_Animale_Cod " & vbCrLf)
                strSql.Append(" , Razza_Animale_Des " & vbCrLf)
                strSql.Append(" , Specie_Animale_Cod " & vbCrLf)
                strSql.Append(" , Specie_Animale_Des " & vbCrLf)
                strSql.Append(" , Sta_Num " & vbCrLf)
                strSql.Append(" , Stalla_Des " & vbCrLf)
                strSql.Append(" , Tipo_Animale_Cod " & vbCrLf)
                strSql.Append(" , Tipo_Animale_Des " & vbCrLf)

                strSql.Append(" , Cod_Articolo " & vbCrLf)
                strSql.Append(" , MetodoProduzioneAppezzamento " & vbCrLf)
                strSql.Append(" , Sup_app " & vbCrLf)
                strSql.Append(" , Sup_imp " & vbCrLf)
                strSql.Append(" , Sup_Prog " & vbCrLf)
                strSql.Append(" , Data_Chiusura_Esercizio " & vbCrLf)
                strSql.Append(" , Codice_Appezzamento " & vbCrLf)
                strSql.Append(" , App_BIO " & vbCrLf)
                strSql.Append(" , Data_Split " & vbCrLf)

                strSql.Append(" , ID_Attivita_Gruppo1 " & vbCrLf)
                strSql.Append(" , Des_Attivita_Gruppo1 " & vbCrLf)
                strSql.Append(" , ID_Attivita_Gruppo2 " & vbCrLf)
                strSql.Append(" , Des_Attivita_Gruppo2 " & vbCrLf)
                strSql.Append(" , ID_Attivita_Gruppo3 " & vbCrLf)
                strSql.Append(" , Des_Attivita_Gruppo3 " & vbCrLf)
                strSql.Append(" , Ordine_Attivita " & vbCrLf)
                strSql.Append(" , Piva_Padre " & vbCrLf)
                strSql.Append(" , Azienda_Padre " & vbCrLf)
                strSql.Append(" ) " & vbCrLf)

                '---------------------------------------------
                '---------------     Select   ----------------
                '---------------------------------------------
                strSql.Append(" Select " & vbCrLf)

                strSql.Append("  '" & Agro_SQL_SaveText(objParametri_Server.PivaSuperUser) & "'" & vbCrLf)
                strSql.Append(" , Agenda.Piva " & vbCrLf)
                strSql.Append(", 0 " & vbCrLf)
                strSql.Append(" ,0 " & vbCrLf)
                strSql.Append(" ,0 " & vbCrLf)
                strSql.Append(" ,Movimenti.Data_Movimento   " & vbCrLf)
                strSql.Append(" , 'Quaderno di campagna' " & vbCrLf)
                strSql.Append("  ,Movimenti_Dettagli.Id_Agenda " & vbCrLf)
                strSql.Append(" ,Movimenti_Dettagli.Id_Mov " & vbCrLf)
                strSql.Append(" ,Movimenti_Dettagli.Id_Mov_Det " & vbCrLf)
                strSql.Append(" ,0  " & vbCrLf)
                strSql.Append(" ,0 " & vbCrLf)
                strSql.Append(" ,Movimenti_Dettagli.Elem_Cod  " & vbCrLf)
                strSql.Append(" ,Movimenti_Dettagli.Pro_Cod  " & vbCrLf)
                strSql.Append(" ,Movimenti_Dettagli.Mat_Cod  " & vbCrLf)
                ' Attivita
                strSql.Append(" ,  ISNULL(( SELECT TOP 1 AttivitaXOperazioni.ID_Attivita  " & vbCrLf)
                strSql.Append(" From AttivitaXOperazioni  " & vbCrLf)
                strSql.AppendLine(" WHERE(AttivitaXOperazioni.Piva_SuperUser) = '" & Agro_SQL_SaveText(pivaSuperUser) & "'")
                strSql.AppendLine(" And   (Agenda.Lav_Cod = AttivitaXOperazioni.Lav_Cod)  ")
                strSql.Append(" Order by AttivitaXOperazioni.ID_Attivita " & vbCrLf)
                strSql.Append(" ) , 0)   " & vbCrLf)

                strSql.Append(" ,0  " & vbCrLf)
                strSql.Append(" ,0  " & vbCrLf)
                strSql.Append(" ,0  " & vbCrLf)
                strSql.Append(" ,0  " & vbCrLf)
                strSql.Append(" ,Movimenti_Dettagli.Lotto  " & vbCrLf)
                strSql.Append(" ,Movimenti_Dettagli.udm_cod " & vbCrLf)
                strSql.Append(" , ISNULL(unitamisura.udm_sim, '') " & vbCrLf)

                '-----------------
                'Prezzo_Unitario calcolato al passo 4
                strSql.Append(" ,0 " & vbCrLf)
                '-----------------
                strSql.Append(" ,Mov_Destinazioni.Qta   " & vbCrLf)
                strSql.Append(" ,0  " & vbCrLf)
                strSql.Append(" ,0  " & vbCrLf)
                strSql.Append(", Agenda.Des_Lib  " & vbCrLf)

                strSql.Append("," & Agro_SQL_SaveNum(budget) & "  " & vbCrLf)
                strSql.Append(" , '" + Budget_Cons_Des & "' " & vbCrLf)

                strSql.Append(" ,1 " & vbCrLf)
                strSql.Append(" , 'Ricavi' " & vbCrLf)
                strSql.Append(" ,Mov_Destinazioni.Sa_Cod  " & vbCrLf)
                strSql.Append(" ,Mov_Destinazioni.Appezza  " & vbCrLf)
                strSql.Append(" ,Mov_Destinazioni.Id_Destinazione   " & vbCrLf)
                strSql.Append(" ,0 " & vbCrLf)
                strSql.Append(" ,0 " & vbCrLf)
                strSql.Append(" ,0 " & vbCrLf)
                strSql.Append(" ,0 " & vbCrLf)
                strSql.Append(" ,0 " & vbCrLf)
                strSql.Append(", ISNULL(SpecieVegetali.Veg_Cod, 0)   " & vbCrLf)
                strSql.Append(", ISNULL(Cultivar.Cul_Cod, 0)   " & vbCrLf)
                strSql.Append(", ISNULL(Operazioni.LAV_DES, ISNULL(des_lib, '')) " & vbCrLf)
                strSql.Append(", ''   " & vbCrLf)
                strSql.Append(", ''  " & vbCrLf)
                strSql.Append(", ''  " & vbCrLf)
                strSql.Append(" , CASE  " & vbCrLf)
                strSql.Append("    WHEN Movimenti_Dettagli.elem_cod = 201 Then 'Semilavorati Vegetali' " & vbCrLf)
                strSql.Append("    WHEN Movimenti_Dettagli.elem_cod = 210 Then 'Trasformati Vegetali' " & vbCrLf)
                strSql.Append("    ELSE  ''  " & vbCrLf)
                strSql.Append("     END  " & vbCrLf)

                strSql.Append(" , CASE  " & vbCrLf)
                strSql.Append("    WHEN Movimenti_Dettagli.pro_cod = 0 And Movimenti_Dettagli.mat_cod != '' Then ISNULL(Materie_Prime.Mat_Des, '')   " & vbCrLf)
                strSql.Append("    ELSE  ''  " & vbCrLf)
                strSql.Append("     END  " & vbCrLf)

                'Descrizione attività
                strSql.Append(" ,  ISNULL(( SELECT TOP 1 Attivita.[Desc]  " & vbCrLf)
                strSql.Append(" From AttivitaXOperazioni join attivita  ON AttivitaXOperazioni.Id_Attivita = Attivita.ID_Attivita " & vbCrLf)
                strSql.AppendLine(" WHERE(AttivitaXOperazioni.Piva_SuperUser) = '" & Agro_SQL_SaveText(pivaSuperUser) & "'")
                strSql.AppendLine(" And   (Agenda.Lav_Cod = AttivitaXOperazioni.Lav_Cod)  ")
                strSql.Append(" Order by AttivitaXOperazioni.ID_Attivita " & vbCrLf)
                strSql.Append(" ) , '')   " & vbCrLf)

                strSql.Append(" , ''  " & vbCrLf)
                strSql.Append(" , ''  " & vbCrLf)
                strSql.Append(" , ''  " & vbCrLf)
                strSql.Append(" , ''  " & vbCrLf)
                strSql.Append(" , ISNULL(Imprese.rag_soc, '')   " & vbCrLf)
                strSql.Append(", ISNULL(IndAzienda.ind_des, '')   " & vbCrLf)
                strSql.Append(" ,ISNULL(IndAzienda.CAP, '')   " & vbCrLf)
                strSql.Append(", ISNULL(IstatAzienda.LOCALITA, '') " & vbCrLf)
                strSql.Append(" , ISNULL(IstatAzienda.COMUNI_PROV, '') " & vbCrLf)
                strSql.Append(", ISNULL(Centri_Aziendali.sa_nome, '') " & vbCrLf)
                strSql.Append(" ,  ISNULL(IndCentro.ind_des, '')   " & vbCrLf)
                strSql.Append(", ISNULL(IndCentro.CAP, '') " & vbCrLf)
                strSql.Append(" , ISNULL(ISTATCentro.LOCALITA, '')  " & vbCrLf)
                strSql.Append(" , ISNULL(ISTATCentro.COMUNI_PROV, '') " & vbCrLf)
                strSql.Append(", ISNULL(Campi.Campo_Des, '') " & vbCrLf)
                strSql.Append(" ,  ISNULL(Appezzamento.APP_NOME, '') " & vbCrLf)
                strSql.Append(" , ISNULL(Appezzamento.Validita_Inizio, " & Agro_SQL_SaveDateTime(AGRODATAINIZIO) & ")  " & vbCrLf)
                strSql.Append(" , ISNULL(Appezzamento.Validita_Fine, " & Agro_SQL_SaveDateTime(AGRODATAFINE) & ")  " & vbCrLf)
                strSql.Append(" ,  ISNULL(GruppoVegetale.Gru_Des, '')  " & vbCrLf)
                strSql.Append(", ISNULL(SpecieVegetali.Veg_Des, '')   " & vbCrLf)
                strSql.Append(" ,  ISNULL(Cultivar.Cul_Des, '')   " & vbCrLf)

                strSql.Append(", ISNULL(GruppoFinalita.Grfi_Des, '')   " & vbCrLf)

                'strSql.Append(" ,  ISNULL(( SELECT TOP 1 Codici_Anagrafe.Descrizione  " & vbCrLf)
                'strSql.Append(" From Reg_Impianti_Codici  " & vbCrLf)
                'strSql.Append(" INNER Join Codici_Anagrafe ON Reg_Impianti_Codici.id_cod = Codici_Anagrafe.codice  " & vbCrLf)
                'strSql.Append(" WHERE(Reg_Impianti_Codici.PIVA = Reg_Impianti.PIVA) " & vbCrLf)
                'strSql.Append(" And   (Reg_Impianti_Codici.Sa_Cod = Reg_Impianti.sa_cod)  " & vbCrLf)
                'strSql.Append(" And   (Reg_Impianti_Codici.Appezza  = Reg_Impianti.Appezza)  " & vbCrLf)
                'strSql.Append(" And   (Reg_Impianti_Codici.Id_Reg = Reg_Impianti.Id_Reg)  " & vbCrLf)
                'strSql.Append(" And   (Reg_Impianti_Codici.Progetto_Cod = 0)  " & vbCrLf)
                'strSql.Append(" And   (Reg_Impianti_Codici.id_cod = Movimenti_Dettagli.Id_Cod_reg_impianti_codici)  " & vbCrLf)
                'strSql.Append(" And (Codici_Anagrafe.gruppo = 'TERRENO')  " & vbCrLf)
                'strSql.Append(" ) , '')   " & vbCrLf)
                ' IMPOSTO DIRETTAMENTE TERRENO NUDO
                strSql.Append(", Case  When  " & vbCrLf)
                strSql.Append("   ISNULL(Reg_Impianti.Id_Reg, 0) = 0 Or ISNULL(Reg_impianti.Cul_Cod, 0) != 0 Then '' " & vbCrLf)
                strSql.Append(" Else " & vbCrLf)
                strSql.Append("  ISNULL(( SELECT TOP 1 Codici_Anagrafe.Descrizione  " & vbCrLf)
                strSql.Append(" From Reg_Impianti_Codici  " & vbCrLf)
                strSql.Append(" INNER Join Codici_Anagrafe ON Reg_Impianti_Codici.id_cod = Codici_Anagrafe.codice  " & vbCrLf)
                strSql.Append(" WHERE(Reg_Impianti_Codici.PIVA = Reg_Impianti.PIVA) " & vbCrLf)
                strSql.Append(" And   (Reg_Impianti_Codici.Sa_Cod = Reg_Impianti.sa_cod)  " & vbCrLf)
                strSql.Append(" And   (Reg_Impianti_Codici.Appezza  = Reg_Impianti.Appezza)  " & vbCrLf)
                strSql.Append(" And   (Reg_Impianti_Codici.Id_Reg = Reg_Impianti.Id_Reg)  " & vbCrLf)
                strSql.Append(" And   (Reg_Impianti_Codici.Progetto_Cod = 0)  " & vbCrLf)
                strSql.Append(" And   (Reg_Impianti_Codici.id_cod = 0)  " & vbCrLf)
                strSql.Append(" And (Codici_Anagrafe.gruppo = 'TERRENO')  " & vbCrLf)
                strSql.Append(" ) , 'TERRENO NUDO')    " & vbCrLf)
                strSql.Append(" END " & vbCrLf)

                strSql.Append(", ISNULL(Convert(varchar(10), Reg_Impianti.Validita_Inizio, 103), '')  " & vbCrLf)
                strSql.Append(" , ISNULL(CONVERT(varchar(10), Reg_Impianti.Validita_Fine, 103), '')   " & vbCrLf)

                strSql.Append(" ,  ISNULL(( SELECT TOP 1 imprese_progetti.Progetto_Nome  " & vbCrLf)
                strSql.Append(" From Imprese_Progetti  " & vbCrLf)
                strSql.Append(" WHERE(Imprese_Progetti.PIVA = Reg_Impianti.PIVA) " & vbCrLf)
                strSql.Append(" And   (Imprese_Progetti.Sa_Cod = Reg_Impianti.sa_cod)  " & vbCrLf)
                strSql.Append(" And   (Imprese_Progetti.Appezza  = Reg_Impianti.Appezza)  " & vbCrLf)
                strSql.Append(" And   (Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg)  " & vbCrLf)
                strSql.Append(" And   (Movimenti.Data_Movimento >= Imprese_Progetti.validita_inizio)  " & vbCrLf)
                strSql.Append(" And   (Movimenti.Data_Movimento <= Imprese_Progetti.validita_fine)  " & vbCrLf)
                strSql.Append(" And   (Imprese_Progetti.validita_inizio >= Reg_Impianti.validita_inizio)  " & vbCrLf)
                strSql.Append(" And   (Imprese_Progetti.validita_inizio <= Reg_Impianti.validita_fine)  " & vbCrLf)
                strSql.Append(" And   (Imprese_Progetti.validita_fine >= Reg_Impianti.validita_inizio)  " & vbCrLf)
                strSql.Append(" And   (Imprese_Progetti.validita_fine <= Reg_Impianti.validita_fine)  " & vbCrLf)
                strSql.Append(" Order by imprese_progetti.validita_inizio, imprese_progetti.validita_fine " & vbCrLf)
                strSql.Append(" ) , '')   " & vbCrLf)

                strSql.Append(" ,  ISNULL(( SELECT TOP 1 Regolamenti.Reg_Des  " & vbCrLf)
                strSql.Append(" From Imprese_Progetti  " & vbCrLf)
                strSql.Append(" Left Join  Regolamenti ON Imprese_Progetti.Regolamento_Cod = Regolamenti.Reg_Cod " & vbCrLf)
                strSql.Append(" WHERE(Imprese_Progetti.PIVA = Reg_Impianti.PIVA) " & vbCrLf)
                strSql.Append(" And   (Imprese_Progetti.Sa_Cod = Reg_Impianti.sa_cod)  " & vbCrLf)
                strSql.Append(" And   (Imprese_Progetti.Appezza  = Reg_Impianti.Appezza)  " & vbCrLf)
                strSql.Append(" And   (Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg)  " & vbCrLf)
                strSql.Append(" And   (Movimenti.Data_Movimento >= Imprese_Progetti.validita_inizio)  " & vbCrLf)
                strSql.Append(" And   (Movimenti.Data_Movimento <= Imprese_Progetti.validita_fine)  " & vbCrLf)
                strSql.Append(" And   (Imprese_Progetti.validita_inizio >= Reg_Impianti.validita_inizio)  " & vbCrLf)
                strSql.Append(" And   (Imprese_Progetti.validita_inizio <= Reg_Impianti.validita_fine)  " & vbCrLf)
                strSql.Append(" And   (Imprese_Progetti.validita_fine >= Reg_Impianti.validita_inizio)  " & vbCrLf)
                strSql.Append(" And   (Imprese_Progetti.validita_fine <= Reg_Impianti.validita_fine)  " & vbCrLf)
                strSql.Append(" Order by imprese_progetti.validita_inizio, imprese_progetti.validita_fine " & vbCrLf)
                strSql.Append(" ) , '')   " & vbCrLf)

                strSql.Append(" ,  ISNULL(( SELECT TOP 1 dpi_Regolamenti.NomeEsteso  " & vbCrLf)
                strSql.Append(" From Imprese_Progetti  " & vbCrLf)
                strSql.Append(" Left Join  DPI_Regolamenti on Imprese_Progetti.Disciplinare_Cod=DPI_Regolamenti.COD_REGOLAMENTO AND Imprese_Progetti.Disciplinare_PubblicoPrivato=DPI_Regolamenti.Flag_Privato_Pubblico " & vbCrLf)
                strSql.Append(" WHERE(Imprese_Progetti.PIVA = Reg_Impianti.PIVA) " & vbCrLf)
                strSql.Append(" And   (Imprese_Progetti.Sa_Cod = Reg_Impianti.sa_cod)  " & vbCrLf)
                strSql.Append(" And   (Imprese_Progetti.Appezza  = Reg_Impianti.Appezza)  " & vbCrLf)
                strSql.Append(" And   (Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg)  " & vbCrLf)
                strSql.Append(" And   (Movimenti.Data_Movimento >= Imprese_Progetti.validita_inizio)  " & vbCrLf)
                strSql.Append(" And   (Movimenti.Data_Movimento <= Imprese_Progetti.validita_fine)  " & vbCrLf)
                strSql.Append(" And   (Imprese_Progetti.validita_inizio >= Reg_Impianti.validita_inizio)  " & vbCrLf)
                strSql.Append(" And   (Imprese_Progetti.validita_inizio <= Reg_Impianti.validita_fine)  " & vbCrLf)
                strSql.Append(" And   (Imprese_Progetti.validita_fine >= Reg_Impianti.validita_inizio)  " & vbCrLf)
                strSql.Append(" And   (Imprese_Progetti.validita_fine <= Reg_Impianti.validita_fine)  " & vbCrLf)
                strSql.Append(" Order by imprese_progetti.validita_inizio, imprese_progetti.validita_fine " & vbCrLf)
                strSql.Append(" ) , '')   " & vbCrLf)

                strSql.Append(", ISNULL(Appezzamento.ESPOSIZ, '')   " & vbCrLf)
                strSql.Append(" ,  ISNULL(Appezzamento.ubicazione, '')   " & vbCrLf)
                strSql.Append("  , ISNULL(Portinnesti.Port_Des, '')  " & vbCrLf)
                strSql.Append(" , ISNULL(ImpiantiIrrigazioni.Imp_Des, '')   " & vbCrLf)
                ' INIZIO Con queste sotto ci sarebbe duplicazione di righe se l'appezzamento è su più particelle
                'strSql.Append("         ,   ISNULL(Lista_Regioni.Regione_Des, '')  " & vbCrLf)
                'strSql.Append(" , ISNULL(ISTATParticelle.COMUNI_PROV, '')  " & vbCrLf)
                'strSql.Append(", ISNULL(ISTATParticelle.LOCALITA, '')  " & vbCrLf)
                'strSql.Append(" , ISNULL(AppezzamentiXParticelle.SEZIONE, '')   " & vbCrLf)
                'strSql.Append(" ,ISNULL(AppezzamentiXParticelle.FOGLIO, -1)   " & vbCrLf)
                'strSql.Append(" ,ISNULL(AppezzamentiXParticelle.NUMERO, -1)   " & vbCrLf)
                'strSql.Append(" ,ISNULL(AppezzamentiXParticelle.SUBALTERNO, '')  " & vbCrLf)
                strSql.Append(" , ''  " & vbCrLf)
                strSql.Append(" , ''  " & vbCrLf)
                strSql.Append(", ''  " & vbCrLf)
                strSql.Append(" , ''  " & vbCrLf)
                strSql.Append(" , -1  " & vbCrLf)
                strSql.Append(" , -1   " & vbCrLf)
                strSql.Append(" , ''  " & vbCrLf)
                ' FINE Con queste sotto ci sarebbe duplicazione di righe se l'appezzamento è su più particelle
                strSql.Append(" , ''  " & vbCrLf)
                strSql.Append(" , ''  " & vbCrLf)
                strSql.Append(" , ''  " & vbCrLf)
                strSql.Append(" , 'Agricolo'  " & vbCrLf)
                strSql.Append(" , ''  " & vbCrLf)
                strSql.Append(" ,  ISNULL(Appezzamento.Campo_Cod, 0) " & vbCrLf)
                strSql.Append(" , ''   " & vbCrLf)

                strSql.Append(" ,  ISNULL(( SELECT TOP 1 imprese_progetti.Progetto_Cod  " & vbCrLf)
                strSql.Append(" From Imprese_Progetti  " & vbCrLf)
                strSql.Append(" WHERE(Imprese_Progetti.PIVA = Reg_Impianti.PIVA) " & vbCrLf)
                strSql.Append(" And   (Imprese_Progetti.Sa_Cod = Reg_Impianti.sa_cod)  " & vbCrLf)
                strSql.Append(" And   (Imprese_Progetti.Appezza  = Reg_Impianti.Appezza)  " & vbCrLf)
                strSql.Append(" And   (Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg)  " & vbCrLf)
                strSql.Append(" And   (Movimenti.Data_Movimento >= Imprese_Progetti.validita_inizio)  " & vbCrLf)
                strSql.Append(" And   (Movimenti.Data_Movimento <= Imprese_Progetti.validita_fine)  " & vbCrLf)
                strSql.Append(" And   (Imprese_Progetti.validita_inizio >= Reg_Impianti.validita_inizio)  " & vbCrLf)
                strSql.Append(" And   (Imprese_Progetti.validita_inizio <= Reg_Impianti.validita_fine)  " & vbCrLf)
                strSql.Append(" And   (Imprese_Progetti.validita_fine >= Reg_Impianti.validita_inizio)  " & vbCrLf)
                strSql.Append(" And   (Imprese_Progetti.validita_fine <= Reg_Impianti.validita_fine)  " & vbCrLf)
                strSql.Append(" Order by imprese_progetti.validita_inizio, imprese_progetti.validita_fine " & vbCrLf)
                strSql.Append(" ) , 0)   " & vbCrLf)

                strSql.Append(" ,  ISNULL(( SELECT TOP 1 imprese_progetti.Progetto_Des  " & vbCrLf)
                strSql.Append(" From Imprese_Progetti  " & vbCrLf)
                strSql.Append(" WHERE(Imprese_Progetti.PIVA = Reg_Impianti.PIVA) " & vbCrLf)
                strSql.Append(" And   (Imprese_Progetti.Sa_Cod = Reg_Impianti.sa_cod)  " & vbCrLf)
                strSql.Append(" And   (Imprese_Progetti.Appezza  = Reg_Impianti.Appezza)  " & vbCrLf)
                strSql.Append(" And   (Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg)  " & vbCrLf)
                strSql.Append(" And   (Movimenti.Data_Movimento >= Imprese_Progetti.validita_inizio)  " & vbCrLf)
                strSql.Append(" And   (Movimenti.Data_Movimento <= Imprese_Progetti.validita_fine)  " & vbCrLf)
                strSql.Append(" And   (Imprese_Progetti.validita_inizio >= Reg_Impianti.validita_inizio)  " & vbCrLf)
                strSql.Append(" And   (Imprese_Progetti.validita_inizio <= Reg_Impianti.validita_fine)  " & vbCrLf)
                strSql.Append(" And   (Imprese_Progetti.validita_fine >= Reg_Impianti.validita_inizio)  " & vbCrLf)
                strSql.Append(" And   (Imprese_Progetti.validita_fine <= Reg_Impianti.validita_fine)  " & vbCrLf)
                strSql.Append(" Order by imprese_progetti.validita_inizio, imprese_progetti.validita_fine " & vbCrLf)
                strSql.Append(" ) , '')   " & vbCrLf)

                strSql.Append(" ,  ISNULL(( SELECT TOP 1 ISNULL(Convert(varchar(10), Imprese_Progetti.validita_inizio, 103), '')  " & vbCrLf)
                strSql.Append(" From Imprese_Progetti  " & vbCrLf)
                strSql.Append(" WHERE(Imprese_Progetti.PIVA = Reg_Impianti.PIVA) " & vbCrLf)
                strSql.Append(" And   (Imprese_Progetti.Sa_Cod = Reg_Impianti.sa_cod)  " & vbCrLf)
                strSql.Append(" And   (Imprese_Progetti.Appezza  = Reg_Impianti.Appezza)  " & vbCrLf)
                strSql.Append(" And   (Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg)  " & vbCrLf)
                strSql.Append(" And   (Movimenti.Data_Movimento >= Imprese_Progetti.validita_inizio)  " & vbCrLf)
                strSql.Append(" And   (Movimenti.Data_Movimento <= Imprese_Progetti.validita_fine)  " & vbCrLf)
                strSql.Append(" And   (Imprese_Progetti.validita_inizio >= Reg_Impianti.validita_inizio)  " & vbCrLf)
                strSql.Append(" And   (Imprese_Progetti.validita_inizio <= Reg_Impianti.validita_fine)  " & vbCrLf)
                strSql.Append(" And   (Imprese_Progetti.validita_fine >= Reg_Impianti.validita_inizio)  " & vbCrLf)
                strSql.Append(" And   (Imprese_Progetti.validita_fine <= Reg_Impianti.validita_fine)  " & vbCrLf)
                strSql.Append(" Order by imprese_progetti.validita_inizio, imprese_progetti.validita_fine " & vbCrLf)
                strSql.Append(" ) , '')   " & vbCrLf)


                strSql.Append(" ,  ISNULL(( SELECT TOP 1 ISNULL(Convert(varchar(10), Imprese_Progetti.validita_fine, 103), '')  " & vbCrLf)
                strSql.Append(" From Imprese_Progetti  " & vbCrLf)
                strSql.Append(" WHERE(Imprese_Progetti.PIVA = Reg_Impianti.PIVA) " & vbCrLf)
                strSql.Append(" And   (Imprese_Progetti.Sa_Cod = Reg_Impianti.sa_cod)  " & vbCrLf)
                strSql.Append(" And   (Imprese_Progetti.Appezza  = Reg_Impianti.Appezza)  " & vbCrLf)
                strSql.Append(" And   (Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg)  " & vbCrLf)
                strSql.Append(" And   (Movimenti.Data_Movimento >= Imprese_Progetti.validita_inizio)  " & vbCrLf)
                strSql.Append(" And   (Movimenti.Data_Movimento <= Imprese_Progetti.validita_fine)  " & vbCrLf)
                strSql.Append(" And   (Imprese_Progetti.validita_inizio >= Reg_Impianti.validita_inizio)  " & vbCrLf)
                strSql.Append(" And   (Imprese_Progetti.validita_inizio <= Reg_Impianti.validita_fine)  " & vbCrLf)
                strSql.Append(" And   (Imprese_Progetti.validita_fine >= Reg_Impianti.validita_inizio)  " & vbCrLf)
                strSql.Append(" And   (Imprese_Progetti.validita_fine <= Reg_Impianti.validita_fine)  " & vbCrLf)
                strSql.Append(" Order by imprese_progetti.validita_inizio, imprese_progetti.validita_fine " & vbCrLf)
                strSql.Append(" ) , '')   " & vbCrLf)

                strSql.Append("  , ISNULL(Movimenti.mov_desc, '') " & vbCrLf)
                strSql.Append("  , 0 " & vbCrLf)
                strSql.Append(" ,Agenda.inviato  " & vbCrLf)
                strSql.Append(" ,Agenda.datainvio  " & vbCrLf)
                strSql.Append(" , " & Agro_SQL_SaveDate(Date.Now) & vbCrLf)
                strSql.Append(" , " & Agro_SQL_SaveDate(Date.Now) & vbCrLf)
                strSql.Append(" , Agenda.Username_Creazione " & vbCrLf)
                strSql.Append(" , Agenda.Username_Modifica " & vbCrLf)
                strSql.Append(" , Agenda.Validita_Inizio  " & vbCrLf)
                strSql.Append(" , Agenda.Validita_Fine  " & vbCrLf)
                strSql.Append(" , 0  " & vbCrLf)
                strSql.Append(" , Movimenti_dettagli.Udm_Cod  " & vbCrLf)
                strSql.Append(" ,1 " & vbCrLf)


                strSql.Append(" ,'' " & vbCrLf)
                strSql.Append(" ,'' " & vbCrLf)
                strSql.Append(" ,0 " & vbCrLf)
                strSql.Append(" ,0 " & vbCrLf)
                strSql.Append(" , ISNULL(Reg_Impianti_Codici_Codice_Impianto.Val_Cod, '') " & vbCrLf)
                strSql.Append(" ,0 " & vbCrLf)
                strSql.Append(" ,'' " & vbCrLf)
                strSql.Append(" ,0 " & vbCrLf)
                strSql.Append(" ,'' " & vbCrLf)
                strSql.Append(" ,0 " & vbCrLf)
                strSql.Append(" ,'' " & vbCrLf)
                strSql.Append(" ,0 " & vbCrLf)
                strSql.Append(" ,'' " & vbCrLf)
                strSql.Append(" ,0 " & vbCrLf)
                strSql.Append(" ,'' " & vbCrLf)

                strSql.Append(" , Case  " & vbCrLf)
                strSql.Append("    When ISNULL(Materie_Prime.Cod_Articolo, '') <> '' Then Materie_Prime.Cod_Articolo " & vbCrLf)
                strSql.Append("    WHEN ISNULL(Materie_Prime.Cod_Articolo, '') = '' Then  " & vbCrLf)
                'filtro ulteriormente per elem cod, perché così se non c'è alcuna codifica per quella categoria faccio meno lavoro
                strSql.AppendLine("         CASE  ")
                strSql.AppendLine("             WHEN NOT EXISTS(SELECT 1 FROM CAC_Codifica_ProdottiAziendali WHERE Elem_Cod = Movimenti_Dettagli.Elem_Cod)  ")
                strSql.AppendLine("             THEN '' ")
                strSql.AppendLine("         ELSE   ")
                strSql.AppendLine("             ISNULL((SELECT TOP(1) Cod_Prodotto_Cliente ")
                strSql.AppendLine("                  FROM CAC_Codifica_ProdottiAziendali ")
                strSql.AppendLine("             WHERE Elem_Cod = Movimenti_Dettagli.Elem_Cod ")
                strSql.AppendLine("             AND Codice_GIAS = Movimenti_Dettagli.Pro_Cod ")
                If flagJoinSuperUserCac AndAlso pivaSuperUser <> "" Then
                    strSql.AppendLine("             AND Piva_SuperUser = '" & Agro_SQL_SaveText(pivaSuperUser) & "'), '') ")
                Else
                    strSql.AppendLine("             AND Piva = Movimenti_Dettagli.Piva), '')  ")
                End If
                strSql.Append("    END  " & vbCrLf)
                strSql.Append(" END " & vbCrLf)
                strSql.Append(" , Case  " & vbCrLf)
                strSql.Append("    When ISNULL(Appezzamento_codici.val_Cod, '') = '' Then '' " & vbCrLf)
                strSql.Append("    WHEN ISNULL(Appezzamento_codici.val_Cod, '') = '1' Then 'Integrato' " & vbCrLf)
                strSql.Append("    WHEN ISNULL(Appezzamento_codici.val_Cod, '') = '2' Then 'In Conversione' " & vbCrLf)
                strSql.Append("    WHEN ISNULL(Appezzamento_codici.val_Cod, '') = '3' Then 'Biologico' " & vbCrLf)
                strSql.Append("    ELSE  ''  " & vbCrLf)
                strSql.Append("     END  " & vbCrLf)
                strSql.Append(" , ISNULL(Appezzamento.Sup_app, 0)   " & vbCrLf)
                strSql.Append(" , ISNULL(Reg_Impianti.Sup_imp, 0)   " & vbCrLf)
                strSql.Append(" , ISNULL(Imprese_Progetti.Sup_Prog, 0)   " & vbCrLf)

                strSql.Append(" ,ISNULL(Convert(varchar(10), Reg_Impianti_Codici_Chiusura_Esercizio.Data_Creazione, 103), '')  " & vbCrLf)
                strSql.AppendLine(" , ISNULL(apc_cod.val_Cod, '')  ")
                strSql.AppendLine(" , ISNULL(apc_BIO.val_Cod, '')  ")
                strSql.AppendLine(" , " & Agro_SQL_SaveDateTime(AGRODATAINIZIO))

                'Campi legati all'attività
                strSql.AppendLine(" ,  ISNULL(( SELECT TOP 1 attivita_gruppi_1.ID_Attivita_Gruppo  ")
                strSql.AppendLine(" From AttivitaXOperazioni join attivita  ON AttivitaXOperazioni.Id_Attivita = Attivita.ID_Attivita ")
                strSql.AppendLine("  Left Join attivita_gruppi as attivita_gruppi_1 on Attivita.ID_Attivita_Gruppo = attivita_gruppi_1.ID_Attivita_Gruppo ")
                strSql.AppendLine("  And attivita_gruppi_1.tipologia_attivita = 1   ")
                strSql.AppendLine(" WHERE(AttivitaXOperazioni.Piva_SuperUser) = '" & Agro_SQL_SaveText(pivaSuperUser) & "'")
                strSql.AppendLine(" And   (Agenda.Lav_Cod = AttivitaXOperazioni.Lav_Cod)  ")
                strSql.AppendLine(" Order by AttivitaXOperazioni.ID_Attivita ")
                strSql.AppendLine(" ) , 0)   ")

                strSql.AppendLine(" ,  ISNULL(( SELECT TOP 1 attivita_gruppi_1.Desc_Gruppo  ")
                strSql.AppendLine(" From AttivitaXOperazioni join attivita  ON AttivitaXOperazioni.Id_Attivita = Attivita.ID_Attivita ")
                strSql.AppendLine("  Left Join attivita_gruppi as attivita_gruppi_1 on Attivita.ID_Attivita_Gruppo = attivita_gruppi_1.ID_Attivita_Gruppo ")
                strSql.AppendLine("  And attivita_gruppi_1.tipologia_attivita = 1   ")
                strSql.AppendLine(" WHERE(AttivitaXOperazioni.Piva_SuperUser) = '" & Agro_SQL_SaveText(pivaSuperUser) & "'")
                strSql.AppendLine(" And   (Agenda.Lav_Cod = AttivitaXOperazioni.Lav_Cod)  ")
                strSql.AppendLine(" Order by AttivitaXOperazioni.ID_Attivita ")
                strSql.AppendLine(" ) , '')   ")

                strSql.AppendLine(" ,  ISNULL(( SELECT TOP 1 attivita_gruppi_2.ID_Attivita_Gruppo  ")
                strSql.AppendLine(" From AttivitaXOperazioni join attivita  ON AttivitaXOperazioni.Id_Attivita = Attivita.ID_Attivita ")
                strSql.AppendLine("  Left Join attivita_gruppi as attivita_gruppi_2 on Attivita.ID_Attivita_Gruppo2 = attivita_gruppi_2.ID_Attivita_Gruppo ")
                strSql.AppendLine("  And attivita_gruppi_2.tipologia_attivita = 2   ")
                strSql.AppendLine(" WHERE(AttivitaXOperazioni.Piva_SuperUser) = '" & Agro_SQL_SaveText(pivaSuperUser) & "'")
                strSql.AppendLine(" And   (Agenda.Lav_Cod = AttivitaXOperazioni.Lav_Cod)  ")
                strSql.AppendLine(" Order by AttivitaXOperazioni.ID_Attivita ")
                strSql.AppendLine(" ) , 0)   ")

                strSql.AppendLine(" ,  ISNULL(( SELECT TOP 1 attivita_gruppi_2.Desc_Gruppo  ")
                strSql.AppendLine(" From AttivitaXOperazioni join attivita  ON AttivitaXOperazioni.Id_Attivita = Attivita.ID_Attivita ")
                strSql.AppendLine("  Left Join attivita_gruppi as attivita_gruppi_2 on Attivita.ID_Attivita_Gruppo2 = attivita_gruppi_2.ID_Attivita_Gruppo ")
                strSql.AppendLine("  And attivita_gruppi_2.tipologia_attivita = 2  ")
                strSql.AppendLine(" WHERE(AttivitaXOperazioni.Piva_SuperUser) = '" & Agro_SQL_SaveText(pivaSuperUser) & "'")
                strSql.AppendLine(" And   (Agenda.Lav_Cod = AttivitaXOperazioni.Lav_Cod)  ")
                strSql.AppendLine(" Order by AttivitaXOperazioni.ID_Attivita ")
                strSql.AppendLine(" ) , '')   ")

                strSql.AppendLine(" ,  ISNULL(( SELECT TOP 1 attivita_gruppi_3.ID_Attivita_Gruppo  ")
                strSql.AppendLine(" From AttivitaXOperazioni join attivita  ON AttivitaXOperazioni.Id_Attivita = Attivita.ID_Attivita ")
                strSql.AppendLine("  Left Join attivita_gruppi as attivita_gruppi_3 on Attivita.ID_Attivita_Gruppo3 = attivita_gruppi_3.ID_Attivita_Gruppo ")
                strSql.AppendLine("  And attivita_gruppi_3.tipologia_attivita = 3   ")
                strSql.AppendLine(" WHERE(AttivitaXOperazioni.Piva_SuperUser) = '" & Agro_SQL_SaveText(pivaSuperUser) & "'")
                strSql.AppendLine(" And   (Agenda.Lav_Cod = AttivitaXOperazioni.Lav_Cod)  ")
                strSql.AppendLine(" Order by AttivitaXOperazioni.ID_Attivita ")
                strSql.AppendLine(" ) , 0)   ")

                strSql.AppendLine(" ,  ISNULL(( SELECT TOP 1 attivita_gruppi_3.Desc_Gruppo  ")
                strSql.AppendLine(" From AttivitaXOperazioni join attivita  ON AttivitaXOperazioni.Id_Attivita = Attivita.ID_Attivita ")
                strSql.AppendLine("  Left Join attivita_gruppi as attivita_gruppi_3 on Attivita.ID_Attivita_Gruppo3 = attivita_gruppi_3.ID_Attivita_Gruppo ")
                strSql.AppendLine("  And attivita_gruppi_3.tipologia_attivita = 3  ")
                strSql.AppendLine(" WHERE(AttivitaXOperazioni.Piva_SuperUser) = '" & Agro_SQL_SaveText(pivaSuperUser) & "'")
                strSql.AppendLine(" And   (Agenda.Lav_Cod = AttivitaXOperazioni.Lav_Cod)  ")
                strSql.AppendLine(" Order by AttivitaXOperazioni.ID_Attivita ")
                strSql.AppendLine(" ) , '')   ")

                strSql.AppendLine(" ,  ISNULL(( SELECT TOP 1 Attivita.Ordine  ")
                strSql.AppendLine(" From AttivitaXOperazioni join attivita  ON AttivitaXOperazioni.Id_Attivita = Attivita.ID_Attivita ")
                strSql.AppendLine(" WHERE(AttivitaXOperazioni.Piva_SuperUser) = '" & Agro_SQL_SaveText(pivaSuperUser) & "'")
                strSql.AppendLine(" And   (Agenda.Lav_Cod = AttivitaXOperazioni.Lav_Cod)  ")
                strSql.AppendLine(" Order by AttivitaXOperazioni.ID_Attivita ")
                strSql.AppendLine(" ) , 0)   ")

                strSql.AppendLine(" , ISNULL((select padre from GerarchiaImprese where figlio  = '" & Agro_SQL_SaveText(piva) & "' ), '') ")
                strSql.AppendLine(" , ISNULL((select rag_soc from GerarchiaImprese g_i join Imprese on g_i.padre = Imprese.piva where g_i.figlio = '" & Agro_SQL_SaveText(piva) & "' ), '') ")

                ' -------------
                ' ---- FROM
                ' -------------
                strSql.Append(" 		   From Agenda  " & vbCrLf)
                strSql.Append(" Join operazioni On agenda.lav_cod = operazioni.lav_cod " & vbCrLf)
                strSql.Append(" Join Movimenti " & vbCrLf)
                strSql.Append(" On Agenda.piva  = Movimenti.piva  And " & vbCrLf)
                strSql.Append(" Agenda.id_Agenda = Movimenti.id_Agenda    " & vbCrLf)
                strSql.Append(" Join Movimenti_Dettagli " & vbCrLf)
                strSql.Append(" On Movimenti_Dettagli.piva  = Movimenti.piva  And " & vbCrLf)
                strSql.Append(" Movimenti_Dettagli.id_Agenda = Movimenti.id_Agenda And   " & vbCrLf)
                strSql.Append(" Movimenti_Dettagli.Id_Mov = Movimenti.Id_Mov    " & vbCrLf)
                strSql.Append(" Join Mov_Destinazioni " & vbCrLf)
                strSql.Append(" On Movimenti_Dettagli.piva  = Mov_Destinazioni.piva  And " & vbCrLf)
                strSql.Append(" Movimenti_Dettagli.id_Agenda = Mov_Destinazioni.id_Agenda And   " & vbCrLf)
                strSql.Append(" Movimenti_Dettagli.Id_Mov = Mov_Destinazioni.Id_Mov And " & vbCrLf)
                strSql.Append(" Movimenti_Dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det    " & vbCrLf)

                strSql.Append("  Left Join Materie_Prime On Movimenti_Dettagli.mat_cod = Materie_Prime.mat_cod " & vbCrLf)

                strSql.Append("  Left Join Zoo_Animali On Zoo_Animali.piva = Movimenti_Dettagli.piva  " & vbCrLf)
                strSql.Append("  And Zoo_Animali.cod_progetto = Movimenti_Dettagli.cod_progetto   " & vbCrLf)
                strSql.Append("  And Movimenti_Dettagli.elem_cod = 300   " & vbCrLf)

                strSql.Append("  Left Join Centri_Aziendali On Movimenti_Dettagli.Piva = Centri_Aziendali.Piva " & vbCrLf)
                strSql.Append(" And Mov_Destinazioni.Sa_Cod = Centri_Aziendali.sa_cod " & vbCrLf)

                strSql.Append(" Left Join Appezzamento On Mov_Destinazioni.Piva = Appezzamento.Piva " & vbCrLf)
                strSql.Append(" And Mov_Destinazioni.Sa_Cod = Appezzamento.sa_cod " & vbCrLf)
                strSql.Append(" And Mov_Destinazioni.appezza = Appezzamento.appezza  " & vbCrLf)

                strSql.Append(" Left Join reg_impianti On " & vbCrLf)
                strSql.Append("  Mov_Destinazioni.Piva = reg_impianti.Piva  " & vbCrLf)
                strSql.Append(" And Mov_Destinazioni.Sa_Cod = reg_impianti.sa_cod  " & vbCrLf)
                strSql.Append(" And Mov_Destinazioni.appezza = reg_impianti.appezza  " & vbCrLf)
                strSql.Append(" And Mov_Destinazioni.Id_Destinazione  = reg_impianti.id_reg " & vbCrLf)

                strSql.Append(" LEFT JOIN Imprese_Progetti ON Mov_Destinazioni.PIVA = Imprese_Progetti.Piva " & vbCrLf)
                strSql.Append(" And Mov_Destinazioni.SA_COD = Imprese_Progetti.Sa_Cod " & vbCrLf)
                strSql.Append(" And Mov_Destinazioni.APPEZZA = Imprese_Progetti.Appezza " & vbCrLf)
                strSql.Append(" And Mov_Destinazioni.ID_Destinazione = Imprese_Progetti.Id_Reg  " & vbCrLf)
                strSql.Append(" And Movimenti.Data_Movimento >= Imprese_Progetti.validita_inizio  " & vbCrLf)
                strSql.Append(" And Movimenti.Data_Movimento <= Imprese_Progetti.validita_fine  " & vbCrLf)

                'strSql.Append("  Left Join  Regolamenti On Imprese_Progetti.Regolamento_Cod = Regolamenti.Reg_Cod " & vbCrLf)

                'strSql.Append("  LEFT JOIN  DPI_Regolamenti On Imprese_Progetti.Disciplinare_Cod=DPI_Regolamenti.COD_REGOLAMENTO And Imprese_Progetti.Disciplinare_PubblicoPrivato=DPI_Regolamenti.Flag_Privato_Pubblico " + vbCrLf)

                strSql.Append(" Left Join Imprese On Mov_Destinazioni.Piva = Imprese.Piva  " & vbCrLf)

                ' INIZIO Con queste sotto ci sarebbe duplicazione di righe se l'appezzamento è su più particelle
                'strSql.Append(" Left OUTER JOIN AppezzamentiXParticelle  " & vbCrLf)
                'strSql.Append(" On Reg_Impianti.Piva = AppezzamentiXParticelle.Piva  " & vbCrLf)
                'strSql.Append(" And Reg_Impianti.Sa_Cod = AppezzamentiXParticelle.Sa_Cod  " & vbCrLf)
                'strSql.Append(" And Reg_Impianti.Appezza = AppezzamentiXParticelle.Appezza   " & vbCrLf)

                'strSql.Append(" Left OUTER JOIN ParticelleCatastali  " & vbCrLf)
                'strSql.Append(" On ParticelleCatastali.PROV = AppezzamentiXParticelle.PROV  " & vbCrLf)
                'strSql.Append(" And ParticelleCatastali.COM = AppezzamentiXParticelle.COM  " & vbCrLf)
                'strSql.Append(" And ParticelleCatastali.SEZIONE = AppezzamentiXParticelle.SEZIONE  " & vbCrLf)
                'strSql.Append(" And ParticelleCatastali.FOGLIO = AppezzamentiXParticelle.FOGLIO  " & vbCrLf)
                'strSql.Append(" And ParticelleCatastali.NUMERO = AppezzamentiXParticelle.NUMERO  " & vbCrLf)
                'strSql.Append(" And ParticelleCatastali.SUBALTERNO = AppezzamentiXParticelle.SUBALTERNO  " & vbCrLf)

                'strSql.Append("  Left OUTER JOIN ISTAT IstatParticelle On AppezzamentiXParticelle.PROV = ISTATParticelle.PROV And AppezzamentiXParticelle.COM = ISTATParticelle.COM  " & vbCrLf)

                'strSql.Append("  Left OUTER JOIN Lista_Province On Lista_Province.SIGLA = ISTATParticelle.Comuni_Prov  " & vbCrLf)
                'strSql.Append(" Left OUTER JOIN Lista_Regioni On Lista_Regioni.REG = Lista_Province.REG  " & vbCrLf)
                ' FINE Con queste sotto ci sarebbe duplicazione di righe se l'appezzamento è su più particelle

                strSql.Append(" Left OUTER JOIN  Campi On Appezzamento.Piva = Campi.Piva  " & vbCrLf)
                strSql.Append(" And Appezzamento.Sa_Cod = Campi.Sa_Cod  " & vbCrLf)
                strSql.Append(" And Appezzamento.Campo_Cod = Campi.Campo_Cod  " & vbCrLf)

                strSql.Append(" Left Join Appezzamento_codici  On " & vbCrLf)
                strSql.Append("   Appezzamento_codici.piva = appezzamento.piva and  Appezzamento_codici.SA_COD = appezzamento.SA_COD And " & vbCrLf)
                strSql.Append("   Appezzamento_codici.APPEZZA = appezzamento.APPEZZA  AND Appezzamento_codici.id_cod = 1018 " & vbCrLf)

                strSql.Append("  Left OUTER JOIN Portinnesti On Reg_Impianti.Port_COD = Portinnesti.Port_Cod  " & vbCrLf)

                strSql.Append("   Left OUTER JOIN ImpiantiIrrigazioni On Reg_Impianti.Imp_COD = ImpiantiIrrigazioni.Imp_Cod  " & vbCrLf)

                strSql.Append(" Left OUTER JOIN CentrixIndirizzi  " & vbCrLf)
                strSql.Append(" On Centri_Aziendali.Piva = CentrixIndirizzi.Piva  " & vbCrLf)
                strSql.Append(" And Centri_Aziendali.sa_cod = CentrixIndirizzi.sa_cod  " & vbCrLf)
                strSql.Append(" And CentrixIndirizzi.tipo_indirizzo = 1  " & vbCrLf)

                strSql.Append(" Left OUTER JOIN Indirizzi IndCentro On CentrixIndirizzi.cod_indirizzo = IndCentro.cod_indirizzo  " & vbCrLf)

                strSql.Append(" Left OUTER JOIN ISTAT IstatCentro On IndCentro.pro_cod_istat = IstatCentro.PROV And IndCentro.com_cod_istat = IstatCentro.COM  " & vbCrLf)

                strSql.Append(" Left OUTER JOIN ImpresexIndirizzi  " & vbCrLf)
                strSql.Append(" On Imprese.Piva = ImpresexIndirizzi.Piva  " & vbCrLf)
                strSql.Append(" And ImpresexIndirizzi.tipo_indirizzo = 1  " & vbCrLf)

                strSql.Append(" Left OUTER JOIN Indirizzi IndAzienda On ImpresexIndirizzi.cod_indirizzo = IndAzienda.cod_indirizzo  " & vbCrLf)

                strSql.Append(" Left OUTER JOIN ISTAT IstatAzienda On IndAzienda.pro_cod_istat = IstatAzienda.PROV And IndAzienda.com_cod_istat = IstatAzienda.COM  " & vbCrLf)

                strSql.Append("  Left OUTER JOIN Cultivar On Reg_Impianti.CUL_COD = Cultivar.Cul_Cod  " & vbCrLf)

                strSql.Append(" Left OUTER JOIN SpecieVegetali  On SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod  " & vbCrLf)

                strSql.Append(" Left OUTER JOIN GruppoVegetale On SpecieVegetali.Gru_Cod = GruppoVegetale.Gru_Cod  " & vbCrLf)
                strSql.Append("  Left OUTER JOIN GruppoFinalita On Reg_Impianti.GRFI_COD = GruppoFinalita.Grfi_Cod  " & vbCrLf)

                strSql.Append("  INNER JOIN unitamisura On Movimenti_Dettagli.udm_cod = unitamisura.udm_Cod  " & vbCrLf)

                strSql.Append("  LEFT OUTER JOIN  Reg_Impianti_Codici As Reg_Impianti_Codici_Codice_Impianto ")
                strSql.Append(" On Reg_Impianti.PIVA=Reg_Impianti_Codici_Codice_Impianto.PIVA And Reg_Impianti.SA_COD=Reg_Impianti_Codici_Codice_Impianto.SA_COD And Reg_Impianti.APPEZZA=Reg_Impianti_Codici_Codice_Impianto.APPEZZA And Reg_Impianti.id_reg=Reg_Impianti_Codici_Codice_Impianto.id_reg And Reg_Impianti_Codici_Codice_Impianto.id_cod = " & enum_CodiciAnagrafe.Codice_Impianto & " " & vbCrLf)

                strSql.Append("  LEFT OUTER JOIN  Reg_Impianti_Codici As Reg_Impianti_Codici_Chiusura_Esercizio ")
                strSql.Append(" on Imprese_Progetti.PIVA=Reg_Impianti_Codici_Chiusura_Esercizio.PIVA And Imprese_Progetti.SA_COD=Reg_Impianti_Codici_Chiusura_Esercizio.SA_COD And Imprese_Progetti.APPEZZA=Reg_Impianti_Codici_Chiusura_Esercizio.APPEZZA And Imprese_Progetti.id_reg=Reg_Impianti_Codici_Chiusura_Esercizio.id_reg And Imprese_Progetti.progetto_cod=Reg_Impianti_Codici_Chiusura_Esercizio.progetto_cod And Reg_Impianti_Codici_Chiusura_Esercizio.id_cod = " & enum_CodiciAnagrafe.Distinta_Chiusa & " And Reg_Impianti_Codici_Chiusura_Esercizio.val_cod = 1 " & vbCrLf)

                strSql.AppendLine(" Left Join Appezzamento_Codici apc_BIO on apc_BIO.piva = Appezzamento.piva And apc_BIO.sa_cod = Appezzamento.sa_cod ")
                strSql.AppendLine(" And apc_BIO.appezza = Appezzamento.appezza  And apc_BIO.id_cod = " & enum_CodiciAnagrafe.Codice_Appezza_Biologico)

                strSql.AppendLine(" Left Join Appezzamento_Codici apc_cod on apc_cod.piva = Appezzamento.piva And apc_cod.sa_cod = Appezzamento.sa_cod ")
                strSql.AppendLine(" And apc_cod.appezza = Appezzamento.appezza And apc_cod.id_cod = " & enum_CodiciAnagrafe.Riferimento_Alfanumerico_Appezzamento)


                'WHERE
                strSql.Append(" WHERE Agenda.Lav_Cod < 1000 " & vbCrLf)
                strSql.Append(" And   Agenda.Lav_Cod > 0 " & vbCrLf)
                strSql.Append(" And   Cau_Mov In (  '2200' ) " & vbCrLf)
                strSql.Append(" AND   Agenda.Piva = '" & Agro_SQL_SaveText(piva) & "' ")
                strSql.Append(" AND   Mov_Destinazioni.Qta != 0 ")

                strSql.Append(" AND Not Exists (select * from DW_CDG_Costi_Ricavi where agenda.piva = DW_CDG_Costi_Ricavi.PIVA And agenda.id_agenda = DW_CDG_Costi_Ricavi.id_agenda ")
                strSql.Append(" And Descr_Modalita_Imputazione = 'Quaderno di campagna' ")
                strSql.Append(" And Costi_Ricavi = 1 ")
                strSql.Append(" And ID_CDG = 0 ")
                strSql.Append(" And Id_CDG_Dettagli = 0 ")
                strSql.Append(" And Vecchio_Tipo_Inser_Dati = 0 )")


                '--------------------------------------------------------------------------
                xRisp = EseguiQuery_Scrittura(objParametri_Server, strSql.ToString, nomeRoutine)
                '--------------------------------------------------------------------------

            End If

            '========================================================================================================================================================================================================================
            '  FASE 4 - Aggiorno i valori per tutte le righe di DW_CDG_Costi_Ricavi di tipo Ricavo calcolato da raccolta nel caso il valore sia cambiato successivamente o sia a 0 (quindi anche righe inserite in precedenza)
            '========================================================================================================================================================================================================================

            '4.1  Prima cerco i costi per PIVA specifica
            If xRisp Then

                strSql.Length = 0

                strSql.Append(" Update DW ")
                strSql.Append(" Set DW.Prezzo_Unitario = Costi.Prezzo_Unitario  ")
                'TODO Da modificare per ricavi poliennali ?
                strSql.Append("  ,   DW.Valore = ROUND(Costi.Prezzo_Unitario * DW.Qta, 2) ")
                strSql.Append("  ,   DW.Percentuale_Ripart = 100 ")
                strSql.Append("  ,   DW.Data_Modifica = " & Agro_SQL_SaveDate(Date.Now))
                strSql.Append("  FROM  DW_CDG_Costi_Ricavi DW ")
                strSql.Append("  INNER Join Prodotti_Costi Costi ON ")
                strSql.Append("  DW.PIVA = Costi.PIVA  ")
                strSql.Append("  And Costi.Id_Budget = 0 ")
                strSql.Append("  And DW.Elem_Cod = Costi.Elem_Cod ")
                strSql.Append("  And DW.Pro_Cod = Costi.Pro_Cod ")
                strSql.Append("  And DW.Mat_Cod = Costi.Mat_Cod ")
                strSql.Append("  And DW.Udm_Cod = Costi.Udm_Cod ")
                strSql.Append("  And DW.Data_Inserimento >= Costi.Validita_Inizio ")
                strSql.Append("  And DW.Data_Inserimento <= Costi.Validita_Fine ")

                strSql.Append("  Where DW.Piva = '" & Agro_SQL_SaveText(piva) & "' ")
                strSql.Append("  And DW.Descr_Modalita_Imputazione = 'Quaderno di campagna' ")
                strSql.Append("  And DW.Costi_Ricavi = 1 ")
                strSql.Append("  And DW.ID_CDG = 0 ")
                strSql.Append("  And DW.Id_CDG_Dettagli = 0 ")
                strSql.Append("  And DW.Vecchio_Tipo_Inser_Dati = 0 ")
                strSql.Append("  And ( DW.Data_Modifica <= Costi.Data_Modifica OR DW.Prezzo_Unitario = 0 ) ")

                '--------------------------------------------------------------------------
                xRisp = EseguiQuery_Scrittura(objParametri_Server, strSql.ToString, nomeRoutine)
                '--------------------------------------------------------------------------
            End If

            '4.2  Se non li ho trovati cerco i costi in caso di prodotti pubblici
            If xRisp Then

                strSql.Length = 0

                strSql.Append(" Update DW ")
                strSql.Append(" Set DW.Prezzo_Unitario = Costi.Prezzo_Unitario  ")
                'TODO Da modificare per ricavi poliennali ?
                strSql.Append("  ,   DW.Valore = ROUND(Costi.Prezzo_Unitario * DW.Qta, 2) ")
                strSql.Append("  ,   DW.Percentuale_Ripart = 100 ")
                strSql.Append("  ,   DW.Data_Modifica = " & Agro_SQL_SaveDate(Date.Now))

                strSql.Append("  FROM  DW_CDG_Costi_Ricavi DW ")
                strSql.AppendLine(" Left join materie_prime on DW.elem_cod = materie_prime.elem_cod And DW.mat_cod = materie_prime.mat_cod ")

                strSql.Append("  INNER Join Prodotti_Costi Costi ON ")
                strSql.AppendLine("  ISNULL(materie_prime.sa_cod, 0) = -1  ")
                strSql.Append("  And Costi.Id_Budget = 0 ")
                strSql.Append("  And DW.Elem_Cod = Costi.Elem_Cod ")
                strSql.Append("  And DW.Pro_Cod = Costi.Pro_Cod ")
                strSql.Append("  And DW.Mat_Cod = Costi.Mat_Cod ")
                strSql.Append("  And DW.Udm_Cod = Costi.Udm_Cod ")
                strSql.Append("  And DW.Data_Inserimento >= Costi.Validita_Inizio ")
                strSql.Append("  And DW.Data_Inserimento <= Costi.Validita_Fine ")

                strSql.Append("  Where DW.Piva = '" & Agro_SQL_SaveText(piva) & "' ")
                strSql.Append("  And DW.Descr_Modalita_Imputazione = 'Quaderno di campagna' ")
                strSql.Append("  And DW.Costi_Ricavi = 1 ")
                strSql.Append("  And DW.ID_CDG = 0 ")
                strSql.Append("  And DW.Id_CDG_Dettagli = 0 ")
                strSql.Append("  And DW.Vecchio_Tipo_Inser_Dati = 0 ")
                strSql.Append("  And DW.Data_Modifica <= Costi.Data_Modifica ")
                strSql.Append("  And DW.Prezzo_Unitario = 0 ")

                '--------------------------------------------------------------------------
                xRisp = EseguiQuery_Scrittura(objParametri_Server, strSql.ToString, nomeRoutine)
                '--------------------------------------------------------------------------
            End If

            If xRisp Then
                ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server) 'Flag_Commit1_Rollback2
            Else
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server) 'Flag_Commit1_Rollback2
            End If

        Catch ex As Exception

            ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server) 'Flag_Commit1_Rollback2

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            xRisp = False

        Finally

        End Try

        Return messaggioErrore

    End Function


    Public Function InserisciDW_VecchioTipo(ByVal piva As String, ByVal Vecchio_Tipo_Inser_Dati As Integer, ByVal idsAgenda As String, ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                           ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.DW_CDG_Costi_Ricavi_DAL_W.InserisciDW_VecchioTipo()"

        '====================================================================================
        'Parametri opzionali :
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Dim pivaSuperUser = objParametri_Server.PivaSuperUser
        Dim Leggi_DW_CDG As New DW_CDG_Costi_Ricavi_DAL_R
        Dim flagJoinSuperUserCac As Boolean = Leggi_DW_CDG.GetFlagJoinCac(objParametri_Utenti)

        Try


            '---------------------------------------------
            strSql.Length = 0

            strSql.Append(" INSERT INTO DW_CDG_Costi_Ricavi ( " & vbCrLf)
            strSql.Append("  Piva_Superuser " & vbCrLf)
            strSql.Append(" ,Piva " & vbCrLf)
            strSql.Append(" ,Id_Cfg_DW " & vbCrLf)
            strSql.Append(" ,Id_CDG " & vbCrLf)
            strSql.Append(" ,Id_CDG_Dettagli " & vbCrLf)
            strSql.Append(" ,Criterio_Analisi " & vbCrLf)
            strSql.Append(" ,Data_Inserimento " & vbCrLf)
            strSql.Append(" ,Descr_Modalita_Imputazione " & vbCrLf)
            strSql.Append(" ,Id_Agenda " & vbCrLf)
            strSql.Append(" ,Id_Mov " & vbCrLf)
            strSql.Append(" ,Id_Mov_Det " & vbCrLf)
            strSql.Append("  ,Mac_Cod " & vbCrLf)
            strSql.Append(" ,Cod_RisUm " & vbCrLf)
            strSql.Append(" ,Elem_Cod " & vbCrLf)
            strSql.Append(" ,Pro_Cod " & vbCrLf)
            strSql.Append(" ,Mat_Cod " & vbCrLf)
            strSql.Append(" ,Udm_Cod " & vbCrLf)
            strSql.Append(" ,Id_Attivita " & vbCrLf)
            strSql.Append(" ,Qualifica_Cod " & vbCrLf)
            strSql.Append(" ,Tariffa_Cod " & vbCrLf)
            strSql.Append(" ,Turno_Cod " & vbCrLf)
            strSql.Append(" ,Conto_Cod " & vbCrLf)
            strSql.Append(" ,Lotto " & vbCrLf)
            strSql.Append(" ,Mezzo " & vbCrLf)
            strSql.Append(" ,Mezzo_Des " & vbCrLf)
            strSql.Append(" ,Prezzo_Unitario " & vbCrLf)
            strSql.Append(" ,Qta " & vbCrLf)
            strSql.Append(" ,Valore " & vbCrLf)
            strSql.Append(" ,Percentuale_Ripart " & vbCrLf)
            strSql.Append(" ,Descrizione " & vbCrLf)
            strSql.Append(" ,Budget_Cons " & vbCrLf)
            strSql.Append(" ,Budget_Cons_Des " & vbCrLf)
            strSql.Append(" ,Costi_Ricavi " & vbCrLf)
            strSql.Append(" ,Costi_Ricavi_Des " & vbCrLf)
            strSql.Append(" ,Sa_Cod " & vbCrLf)
            strSql.Append(" ,Appezza " & vbCrLf)
            strSql.Append(" ,Id_Reg " & vbCrLf)
            strSql.Append(" ,Id_Cod_reg_impianti_codici " & vbCrLf)
            strSql.Append(" ,Id_Imputazione " & vbCrLf)
            strSql.Append(" ,Tipo_Imputazione " & vbCrLf)
            strSql.Append(" ,Macchine_Cod " & vbCrLf)
            strSql.Append(" ,Linea_Cod " & vbCrLf)
            strSql.Append(", Veg_Cod  " & vbCrLf)
            strSql.Append(", Cul_Cod " & vbCrLf)
            strSql.Append(" , Descrizione_Operazione " & vbCrLf)
            strSql.Append(" , Descrizione_Macchina " & vbCrLf)
            strSql.Append(" , Nome_Cognome " & vbCrLf)
            strSql.Append(" , NrBadge " & vbCrLf)
            strSql.Append(" , Categoria " & vbCrLf)

            strSql.Append(" , Descrizione_Prodotto " & vbCrLf)
            strSql.Append(" , Attivita " & vbCrLf)
            strSql.Append(" , Qualifica " & vbCrLf)
            strSql.Append(" , Tariffa " & vbCrLf)
            strSql.Append(" , Turno " & vbCrLf)
            strSql.Append(" , Conto " & vbCrLf)
            strSql.Append(" , Ragione_Sociale_Azienda " & vbCrLf)
            strSql.Append(" , Indirizzo_Azienda " & vbCrLf)
            strSql.Append(" , Cap_Azienda " & vbCrLf)
            strSql.Append(" , Localita_Azienda " & vbCrLf)
            strSql.Append(" , Prov_Azienda " & vbCrLf)
            strSql.Append(" , Descrizione_Centro " & vbCrLf)
            strSql.Append(" , Indirizzo_Centro " & vbCrLf)
            strSql.Append(" , Cap_Centro " & vbCrLf)
            strSql.Append(" , Localita_Centro " & vbCrLf)
            strSql.Append(" , Prov_Centro " & vbCrLf)
            strSql.Append(" , Descrizione_Campo " & vbCrLf)
            strSql.Append(" , Nome_Appezzamento " & vbCrLf)
            strSql.Append(" , Inizio_Appezzamento " & vbCrLf)
            strSql.Append(" , Fine_Appezzamento " & vbCrLf)
            strSql.Append(" , Descrizione_Gruppo_Vegetale " & vbCrLf)
            strSql.Append(" , Specie_impianto " & vbCrLf)
            strSql.Append(" , Varieta_impianto " & vbCrLf)
            strSql.Append(" , Descrizione_Finalita " & vbCrLf)
            strSql.Append(" , Destinazione_Uso " & vbCrLf)
            strSql.Append(" , Inizio_Impianto " & vbCrLf)
            strSql.Append(" , Fine_Impianto " & vbCrLf)
            strSql.Append(" , Codice_Impianto " & vbCrLf)

            strSql.Append(" , Progetto_Nome " & vbCrLf)
            strSql.Append(" , Regolamento " & vbCrLf)
            strSql.Append(" , Disciplinare " & vbCrLf)

            strSql.Append(" , Esposizione_Appezzamento " & vbCrLf)
            strSql.Append(" , Ubicazione_Appezzamento " & vbCrLf)
            strSql.Append(" , Descrizione_Portinnesti " & vbCrLf)
            strSql.Append(" , Descrizione_Irrigazioni " & vbCrLf)
            strSql.Append(" , Regione_Impianto " & vbCrLf)
            strSql.Append(" , Prov_Impianto " & vbCrLf)
            strSql.Append(" , Comune_Impianto " & vbCrLf)
            strSql.Append(" , Sezione " & vbCrLf)
            strSql.Append(" , Foglio " & vbCrLf)
            strSql.Append(" , Numero " & vbCrLf)
            strSql.Append(" , Subalterno " & vbCrLf)
            strSql.Append(" , Descrizione_Macchina_Input_Costi " & vbCrLf)
            strSql.Append(" , Progetto " & vbCrLf)
            strSql.Append(" , Classe_Progetto " & vbCrLf)
            strSql.Append(" , Tipo_Progetto " & vbCrLf)
            strSql.Append(" , Linea_Produzione " & vbCrLf)
            strSql.Append(" , Campo_Cod " & vbCrLf)
            strSql.Append(" , Lotto_Input_Costi " & vbCrLf)
            strSql.Append(" , Progetto_Cod " & vbCrLf)
            strSql.Append(" , Progetto_Des " & vbCrLf)
            strSql.Append(" , Progetto_Validita_Inizio " & vbCrLf)
            strSql.Append(" , Progetto_Validita_Fine " & vbCrLf)
            strSql.Append(" , Note " & vbCrLf)
            strSql.Append(" , Vecchio_Tipo_Inser_Dati " & vbCrLf)
            strSql.Append(" , inviato " & vbCrLf)
            strSql.Append(" , datainvio " & vbCrLf)
            strSql.Append(" , Data_Creazione " & vbCrLf)
            strSql.Append(" , Data_Modifica " & vbCrLf)
            strSql.Append(" , Username_Creazione " & vbCrLf)
            strSql.Append(" , Username_Modifica " & vbCrLf)
            strSql.Append(" , Validita_Inizio " & vbCrLf)
            strSql.Append(" , Validita_Fine " & vbCrLf)
            strSql.Append(" , Cod_Animale " & vbCrLf)
            strSql.Append(" , Animale_Progetto " & vbCrLf)
            strSql.Append(" , Cod_Animale_Distinta " & vbCrLf)
            strSql.Append(" , Animale_Distinta " & vbCrLf)
            strSql.Append(" , Sta_Num " & vbCrLf)
            strSql.Append(" , Stalla_Des " & vbCrLf)
            strSql.Append(" , Raggruppamento_Cod " & vbCrLf)
            strSql.Append(" , Raggruppamento_Des " & vbCrLf)
            strSql.Append(" , Specie_Animale_Cod " & vbCrLf)
            strSql.Append(" , Specie_Animale_Des " & vbCrLf)
            strSql.Append(" , Razza_Animale_Cod " & vbCrLf)
            strSql.Append(" , Razza_Animale_Des " & vbCrLf)
            strSql.Append(" , Tipo_Animale_Cod " & vbCrLf)
            strSql.Append(" , Tipo_Animale_Des " & vbCrLf)
            strSql.Append(" , Modalita_Imputazione " & vbCrLf)

            strSql.Append(" , Cod_Articolo " & vbCrLf)
            strSql.Append(" , MetodoProduzioneAppezzamento " & vbCrLf)
            strSql.Append(" , Sup_app " & vbCrLf)
            strSql.Append(" , Sup_imp " & vbCrLf)
            strSql.Append(" , Sup_Prog " & vbCrLf)
            strSql.Append(" , Data_Chiusura_Esercizio " & vbCrLf)

            strSql.Append(" ) " & vbCrLf)

            strSql.Append(" Select " & vbCrLf)

            strSql.Append("  CDG_Testata.Piva_Superuser " & vbCrLf)
            strSql.Append(" , CDG_Testata.Piva " & vbCrLf)
            strSql.Append(" ,  0 " & vbCrLf)
            strSql.Append(", CDG_Testata.Id_CDG " & vbCrLf)
            strSql.Append(" ,CDG_Dettagli.Id_CDG_Dettagli " & vbCrLf)
            strSql.Append(" ,0 " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Data_Inserimento  " & vbCrLf)
            strSql.Append(" , Case  " & vbCrLf)
            strSql.Append("    When CDG_Testata.Modalita_imputazione = 0 And Cdg_Dettagli.Cod_Animale = 0 Then 'Op. QdC' " & vbCrLf)
            strSql.Append("    When CDG_Testata.Modalita_imputazione = 0 And Cdg_Dettagli.Cod_Animale != 0 Then 'Op. Zoo' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.Modalita_imputazione = 1 And CDG_Testata.OrigineAPP = 0 Then 'Diretta' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.Modalita_imputazione = 1 And CDG_Testata.OrigineAPP IN (1,2,3,4) Then 'App' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.Modalita_imputazione = 2 Then 'TimeSheet' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.Modalita_imputazione = 4 Then 'Contabilità' " & vbCrLf)
            strSql.Append("    ELSE  ''  " & vbCrLf)
            strSql.Append("     END  " & vbCrLf)
            strSql.Append("  ,CDG_Testata.Id_Agenda " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Id_Mov " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Id_Mov_Det " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Mac_Cod  " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Cod_RisUm  " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Elem_Cod  " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Pro_Cod  " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Mat_Cod  " & vbCrLf)
            strSql.Append(", ISNULL(CDG_Testata.Udm_Cod, 0)  " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Id_Attivita " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Qualifica_Cod  " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Tariffa_Cod  " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Turno_Cod  " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Conto_Cod  " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Lotto  " & vbCrLf)
            strSql.Append(" ,CDG_Testata.udm_cod " & vbCrLf)
            strSql.Append(" ,CASE   WHEN CDG_Testata.udm_cod = 0 Then 'Ore' Else ISNULL(unitamisura.udm_sim, '') END  " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Prezzo_Unitario   " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Qta   " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Valore_Totale  " & vbCrLf)
            strSql.Append(" ,100  " & vbCrLf)
            strSql.Append(", ISNULL(CDG_Testata.Descrizione, '')    " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Budget   " & vbCrLf)
            strSql.Append(" ,CASE   WHEN CDG_Testata.Budget = 0 Then 'Consuntivo' Else 'Budget' END  " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Costi_Ricavi  " & vbCrLf)
            strSql.Append(" ,CASE   WHEN CDG_Testata.Costi_Ricavi = 0 Then 'Costi' Else 'Ricavi' END  " & vbCrLf)
            strSql.Append(" ,CDG_Dettagli.Sa_Cod  " & vbCrLf)
            strSql.Append(" ,CDG_Dettagli.Appezza  " & vbCrLf)
            strSql.Append(" ,CDG_Dettagli.Id_Reg  " & vbCrLf)
            strSql.Append(" ,CDG_Dettagli.Id_Cod_reg_impianti_codici  " & vbCrLf)
            strSql.Append(" ,CDG_Dettagli.Id_Imputazione  " & vbCrLf)
            strSql.Append(", ISNULL(Imputazioni.Tipo_Imputazione, 0)    " & vbCrLf)
            strSql.Append(" ,CDG_Dettagli.Macchine_Cod  " & vbCrLf)
            strSql.Append(" ,CDG_Dettagli.Linea_Cod  " & vbCrLf)
            strSql.Append(", ISNULL(SpecieVegetali.Veg_Cod, 0)   " & vbCrLf)
            strSql.Append(", ISNULL(Cultivar.Cul_Cod, 0)   " & vbCrLf)
            strSql.Append(", ISNULL(Operazioni.LAV_DES, ISNULL(des_lib, '')) " & vbCrLf)
            strSql.Append(", ISNULL(Parco_Macchine.Mac_Des, '')   " & vbCrLf)
            strSql.Append(", ISNULL(Contatti.Cognome, '')  + ' ' +  ISNULL(Contatti.Nome, '')   " & vbCrLf)

            strSql.Append(", ISNULL(Contatti.NrBadge, '') " & vbCrLf)

            strSql.Append(" , CASE  " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.cod_risum != 0 Then 'Personale' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.mac_cod != 0 Then 'Macchine e attrezzature' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 501 Then 'Altri beni' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 555 Then 'Servizi' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 2   Then 'Carburanti' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 3   Then 'Fertilizzanti' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 4   Then 'Rifiuti' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 10  Then 'Semente e Materiale Vivaistico' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 191 Then 'Formulati' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 195 Then 'Coadiuvanti' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 196 Then 'Insetti utili' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 197 Then 'Trappole' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 198 Then 'Avversità' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 200 Then 'Altre Risorse' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 201 Then 'Semilavorati Vegetali' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 204 Then 'Materie Prime Vegetali' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 205 Then 'Beni Confezionamento Vegetale' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 210 Then 'Trasformati Vegetali' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 300 Then 'Consistenza Zootecnica' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 301 Then 'Semilavorati Animali' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 304 Then 'Materie Animali' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 305 Then 'Beni Confezionamento Animale' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 306 Then 'Mangimi' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 307 Then 'Farmaci' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 310 Then 'Trasformati Animali' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 401 Then 'Ricambi' " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 700 Then 'Servizi Professionali' " & vbCrLf)
            strSql.Append("    ELSE  ''  " & vbCrLf)
            strSql.Append("     END  " & vbCrLf)


            strSql.Append(" , CASE  " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 501 And cdg_testata.pro_cod = 0 Then ISNULL(Movimenti_Dettagli.Mov_Det_Des, '') " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 555 And cdg_testata.pro_cod != 0 Then ISNULL(Categorie.descr, '') " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 2   And cdg_testata.pro_cod != 0 Then ISNULL(Carburanti.Car_Des, '') " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 3   And cdg_testata.pro_cod != 0 Then ISNULL(fertilizzanti.fer_des, '') " & vbCrLf)
            'strSql.Append("    WHEN CDG_Testata.elem_cod = 4   And cdg_testata.pro_cod != 0 Then ISNULL(CatalogoEuropeoRifiuti.Cer_Des, '') " & vbCrLf)
            'strSql.Append("    WHEN CDG_Testata.elem_cod = 10  And cdg_testata.pro_cod != 0 Then ISNULL(TipologieSementi.Sem_Des, '') " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 191 And cdg_testata.pro_cod != 0 Then ISNULL(formulati.fr_des, '')  " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 195 And cdg_testata.pro_cod != 0 Then ISNULL(Coadiuvante.Coad_Des, '')  " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 196 And cdg_testata.pro_cod != 0 Then ISNULL(InsettiUtili.Ins_Des, '')  " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 197 And cdg_testata.pro_cod != 0 Then ISNULL(trappole.trap_des, '') " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 198 And cdg_testata.pro_cod != 0 Then ISNULL(Avversita.Av_Des_Vol, '') " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod = 300 And cdg_testata.pro_cod != 0 Then ISNULL(Zoo_Animali.Matricola, '') " & vbCrLf)
            strSql.Append("    WHEN CDG_Testata.elem_cod in (10, 200,201,204,205,210,301,304,305,306,307,310,401,700) And cdg_testata.pro_cod = 0 And cdg_testata.mat_cod != 0 Then ISNULL(Materie_Prime.Mat_Des, '') " & vbCrLf)
            strSql.Append("    ELSE  ''  " & vbCrLf)
            strSql.Append("     END  " & vbCrLf)
            strSql.Append(", ISNULL(Attivita.[Desc], '')   " & vbCrLf)
            strSql.Append(" , ISNULL(Qualifica_des, '')   " & vbCrLf)
            strSql.Append(", ISNULL(Tariffa_des, '')   " & vbCrLf)
            strSql.Append(" , ISNULL(Turno_Des, '')   " & vbCrLf)
            strSql.Append(", ISNULL(Conto_Descr, '')   " & vbCrLf)
            strSql.Append(" , ISNULL(Imprese.rag_soc, '')   " & vbCrLf)
            strSql.Append(", ISNULL(IndAzienda.ind_des, '')   " & vbCrLf)
            strSql.Append(" ,ISNULL(IndAzienda.CAP, '')   " & vbCrLf)
            strSql.Append(", ISNULL(IstatAzienda.LOCALITA, '') " & vbCrLf)
            strSql.Append(" , ISNULL(IstatAzienda.COMUNI_PROV, '') " & vbCrLf)
            strSql.Append(", ISNULL(Centri_Aziendali.sa_nome, '') " & vbCrLf)
            strSql.Append(" ,  ISNULL(IndCentro.ind_des, '')   " & vbCrLf)
            strSql.Append(", ISNULL(IndCentro.CAP, '') " & vbCrLf)
            strSql.Append(" , ISNULL(ISTATCentro.LOCALITA, '')  " & vbCrLf)
            strSql.Append(" , ISNULL(ISTATCentro.COMUNI_PROV, '') " & vbCrLf)
            strSql.Append(", ISNULL(Campi.Campo_Des, '') " & vbCrLf)
            strSql.Append(" ,  ISNULL(Appezzamento.APP_NOME, '') " & vbCrLf)
            strSql.Append(" , ISNULL(Appezzamento.Validita_Inizio, " & Agro_SQL_SaveDateTime(AGRODATAINIZIO) & ")  " & vbCrLf)
            strSql.Append(" , ISNULL(Appezzamento.Validita_Fine, " & Agro_SQL_SaveDateTime(AGRODATAFINE) & ")  " & vbCrLf)
            strSql.Append(" ,  ISNULL(GruppoVegetale.Gru_Des, '')  " & vbCrLf)
            strSql.Append(", ISNULL(SpecieVegetali.Veg_Des, '')   " & vbCrLf)
            strSql.Append(" ,  ISNULL(Cultivar.Cul_Des, '')   " & vbCrLf)

            strSql.Append(", ISNULL(GruppoFinalita.Grfi_Des, '')   " & vbCrLf)

            'strSql.Append(" ,  ISNULL(( SELECT TOP 1 Codici_Anagrafe.Descrizione  " & vbCrLf)
            'strSql.Append(" From Reg_Impianti_Codici  " & vbCrLf)
            'strSql.Append(" INNER Join Codici_Anagrafe ON Reg_Impianti_Codici.id_cod = Codici_Anagrafe.codice  " & vbCrLf)
            'strSql.Append(" WHERE(Reg_Impianti_Codici.PIVA = Reg_Impianti.PIVA) " & vbCrLf)
            'strSql.Append(" And   (Reg_Impianti_Codici.Sa_Cod = Reg_Impianti.sa_cod)  " & vbCrLf)
            'strSql.Append(" And   (Reg_Impianti_Codici.Appezza  = Reg_Impianti.Appezza)  " & vbCrLf)
            'strSql.Append(" And   (Reg_Impianti_Codici.Id_Reg = Reg_Impianti.Id_Reg)  " & vbCrLf)
            'strSql.Append(" And   (Reg_Impianti_Codici.Progetto_Cod = 0)  " & vbCrLf)
            'strSql.Append(" And   (Reg_Impianti_Codici.id_cod = CDG_Dettagli.Id_Cod_reg_impianti_codici)  " & vbCrLf)
            'strSql.Append(" And (Codici_Anagrafe.gruppo = 'TERRENO')  " & vbCrLf)
            'strSql.Append(" ) , '')   " & vbCrLf)
            ' IMPOSTO DIRETTAMENTE TERRENO NUDO
            strSql.Append(", Case  When  " & vbCrLf)
            strSql.Append("   ISNULL(Reg_Impianti.Id_Reg, 0) = 0 Or ISNULL(Reg_impianti.Cul_Cod, 0) != 0 Then '' " & vbCrLf)
            strSql.Append(" Else " & vbCrLf)
            strSql.Append("  ISNULL(( SELECT TOP 1 Codici_Anagrafe.Descrizione  " & vbCrLf)
            strSql.Append(" From Reg_Impianti_Codici  " & vbCrLf)
            strSql.Append(" INNER Join Codici_Anagrafe ON Reg_Impianti_Codici.id_cod = Codici_Anagrafe.codice  " & vbCrLf)
            strSql.Append(" WHERE(Reg_Impianti_Codici.PIVA = Reg_Impianti.PIVA) " & vbCrLf)
            strSql.Append(" And   (Reg_Impianti_Codici.Sa_Cod = Reg_Impianti.sa_cod)  " & vbCrLf)
            strSql.Append(" And   (Reg_Impianti_Codici.Appezza  = Reg_Impianti.Appezza)  " & vbCrLf)
            strSql.Append(" And   (Reg_Impianti_Codici.Id_Reg = Reg_Impianti.Id_Reg)  " & vbCrLf)
            strSql.Append(" And   (Reg_Impianti_Codici.Progetto_Cod = 0)  " & vbCrLf)
            strSql.Append(" And   (Reg_Impianti_Codici.id_cod = CDG_Dettagli.Id_Cod_reg_impianti_codici)  " & vbCrLf)
            strSql.Append(" And (Codici_Anagrafe.gruppo = 'TERRENO')  " & vbCrLf)
            strSql.Append(" ) , 'TERRENO NUDO')    " & vbCrLf)
            strSql.Append(" END " & vbCrLf)

            strSql.Append(", ISNULL(Convert(varchar(10), Reg_Impianti.Validita_Inizio, 103), '')  " & vbCrLf)
            strSql.Append(" , ISNULL(CONVERT(varchar(10), Reg_Impianti.Validita_Fine, 103), '')   " & vbCrLf)

            strSql.Append(", Case  When  " & vbCrLf)
            strSql.Append("   ISNULL(Imputazioni.Imputazione_Cod, 0) != 0 Then Imputazioni.CodicePrincipale " & vbCrLf)
            strSql.Append(" Else " & vbCrLf)
            strSql.Append("  ISNULL(Reg_Impianti_Codici_Codice_Impianto.Val_Cod, '') " & vbCrLf)
            strSql.Append(" END " & vbCrLf)

            strSql.Append(", Case  When  " & vbCrLf)
            strSql.Append("   ISNULL(Imputazioni.Imputazione_Cod, 0) != 0 Then Imputazioni.CodiceSecondario " & vbCrLf)
            strSql.Append(" Else " & vbCrLf)
            strSql.Append("   ISNULL(imprese_progetti.Progetto_Nome, '')  " & vbCrLf)
            strSql.Append(" END " & vbCrLf)

            strSql.Append(" , ISNULL(Regolamenti.Reg_Des, '')   " & vbCrLf)
            strSql.Append(" , ISNULL(dpi_Regolamenti.NomeEsteso, '')   " & vbCrLf)

            strSql.Append(", ISNULL(Appezzamento.ESPOSIZ, '')   " & vbCrLf)
            strSql.Append(" ,  ISNULL(Appezzamento.ubicazione, '')   " & vbCrLf)
            strSql.Append("  , ISNULL(Portinnesti.Port_Des, '')  " & vbCrLf)
            strSql.Append(" , ISNULL(ImpiantiIrrigazioni.Imp_Des, '')   " & vbCrLf)
            ' INIZIO Con queste sotto ci sarebbe duplicazione di righe se l'appezzamento è su più particelle
            'strSql.Append("         ,   ISNULL(Lista_Regioni.Regione_Des, '')  " & vbCrLf)
            'strSql.Append(" , ISNULL(ISTATParticelle.COMUNI_PROV, '')  " & vbCrLf)
            'strSql.Append(", ISNULL(ISTATParticelle.LOCALITA, '')  " & vbCrLf)
            'strSql.Append(" , ISNULL(AppezzamentiXParticelle.SEZIONE, '')   " & vbCrLf)
            'strSql.Append(" ,ISNULL(AppezzamentiXParticelle.FOGLIO, -1)   " & vbCrLf)
            'strSql.Append(" ,ISNULL(AppezzamentiXParticelle.NUMERO, -1)   " & vbCrLf)
            'strSql.Append(" ,ISNULL(AppezzamentiXParticelle.SUBALTERNO, '')  " & vbCrLf)
            strSql.Append(" , ''  " & vbCrLf)
            strSql.Append(" , ''  " & vbCrLf)
            strSql.Append(", ''  " & vbCrLf)
            strSql.Append(" , ''  " & vbCrLf)
            strSql.Append(" , -1  " & vbCrLf)
            strSql.Append(" , -1   " & vbCrLf)
            strSql.Append(" , ''  " & vbCrLf)
            ' FINE Con queste sotto ci sarebbe duplicazione di righe se l'appezzamento è su più particelle

            strSql.Append("       , ISNULL(Parco_Macchine_Input_Costi.mac_des, '')   " & vbCrLf)
            strSql.Append(" , ISNULL(Imputazione_Nome, '')   " & vbCrLf)
            strSql.Append("  , ISNULL(Imputazione_Classe_Des, '')   " & vbCrLf)
            strSql.Append(" , ISNULL(Tipo_Imputazione_Des, '')  " & vbCrLf)
            strSql.Append(" , ISNULL(Linea_Des, '')  " & vbCrLf)
            strSql.Append(" ,  ISNULL(Appezzamento.Campo_Cod, 0) " & vbCrLf)
            strSql.Append(" , CDG_Dettagli.Lotto_Input_Costi  " & vbCrLf)
            strSql.Append(" ,  ISNULL(imprese_progetti.Progetto_Cod, 0)  " & vbCrLf)
            strSql.Append(" ,  ISNULL(imprese_progetti.Progetto_Des, '')  " & vbCrLf)
            strSql.Append(", ISNULL(Convert(varchar(10), Reg_Impianti.Validita_Inizio, 103), '')  " & vbCrLf)
            strSql.Append(" , ISNULL(CONVERT(varchar(10), Reg_Impianti.Validita_Fine, 103), '')   " & vbCrLf)

            strSql.Append("  , ISNULL(Movimenti.mov_desc, '') " & vbCrLf)
            strSql.Append("  , CDG_Testata.Vecchio_Tipo_Inser_Dati " & vbCrLf)
            strSql.Append(" ,CDG_Dettagli.inviato  " & vbCrLf)
            strSql.Append(" ,CDG_Dettagli.datainvio  " & vbCrLf)
            strSql.Append("	 , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            strSql.Append("	 , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            strSql.Append(" ,CDG_Dettagli.Username_Creazione " & vbCrLf)
            strSql.Append(" ,CDG_Dettagli.Username_Modifica " & vbCrLf)
            strSql.Append(" ,CDG_Dettagli.Validita_Inizio  " & vbCrLf)
            strSql.Append(" ,CDG_Dettagli.Validita_Fine  " & vbCrLf)

            strSql.Append(" ,  0 " & vbCrLf)
            strSql.Append(" ,  '' " & vbCrLf)
            strSql.Append(" , 0 " & vbCrLf)
            strSql.Append(" , '' " & vbCrLf)
            strSql.Append(" , 0 " & vbCrLf)
            strSql.Append(" , '' " & vbCrLf)
            strSql.Append(" , 0 " & vbCrLf)
            strSql.Append(" , '' " & vbCrLf)
            strSql.Append(" , 0 " & vbCrLf)
            strSql.Append(" , '' " & vbCrLf)
            strSql.Append(" , 0 " & vbCrLf)
            strSql.Append(" , '' " & vbCrLf)
            strSql.Append(" , 0 " & vbCrLf)
            strSql.Append(" , '' " & vbCrLf)
            strSql.Append(" ,CDG_Testata.Modalita_Imputazione  " & vbCrLf)

            strSql.Append(" , Case  " & vbCrLf)
            strSql.Append("    When ISNULL(Materie_Prime.Cod_Articolo, '') <> '' Then Materie_Prime.Cod_Articolo " & vbCrLf)
            strSql.Append("    WHEN ISNULL(Materie_Prime.Cod_Articolo, '') = '' Then  " & vbCrLf)
            'filtro ulteriormente per elem cod, perché così se non c'è alcuna codifica per quella categoria faccio meno lavoro
            strSql.AppendLine("         CASE  ")
            strSql.AppendLine("             WHEN NOT EXISTS(SELECT 1 FROM CAC_Codifica_ProdottiAziendali WHERE Elem_Cod = CDG_Testata.Elem_Cod)  ")
            strSql.AppendLine("             THEN '' ")
            strSql.AppendLine("         ELSE   ")
            strSql.AppendLine("             ISNULL((SELECT TOP(1) Cod_Prodotto_Cliente ")
            strSql.AppendLine("                  FROM CAC_Codifica_ProdottiAziendali ")
            strSql.AppendLine("             WHERE Elem_Cod = CDG_Testata.Elem_Cod ")
            strSql.AppendLine("             AND Codice_GIAS = CDG_Testata.Pro_Cod ")
            If flagJoinSuperUserCac AndAlso pivaSuperUser <> "" Then
                strSql.AppendLine("             AND Piva_SuperUser = '" & Agro_SQL_SaveText(pivaSuperUser) & "'), '') ")
            Else
                strSql.AppendLine("             AND Piva = CDG_Testata.Piva), '')  ")
            End If
            strSql.Append("    END  " & vbCrLf)
            strSql.Append(" END " & vbCrLf)
            strSql.Append(" , Case  " & vbCrLf)
            strSql.Append("    When ISNULL(Appezzamento_codici.val_Cod, '') = '' Then '' " & vbCrLf)
            strSql.Append("    WHEN ISNULL(Appezzamento_codici.val_Cod, '') = '1' Then 'Integrato' " & vbCrLf)
            strSql.Append("    WHEN ISNULL(Appezzamento_codici.val_Cod, '') = '2' Then 'In Conversione' " & vbCrLf)
            strSql.Append("    WHEN ISNULL(Appezzamento_codici.val_Cod, '') = '3' Then 'Biologico' " & vbCrLf)
            strSql.Append("    ELSE  ''  " & vbCrLf)
            strSql.Append("     END  " & vbCrLf)
            strSql.Append(" , ISNULL(Appezzamento.Sup_app, 0)   " & vbCrLf)
            strSql.Append(" , ISNULL(Reg_Impianti.Sup_imp, 0)   " & vbCrLf)
            strSql.Append(" , ISNULL(Imprese_Progetti.Sup_Prog, 0)   " & vbCrLf)
            strSql.Append(" ,ISNULL(Convert(varchar(10), Reg_Impianti_Codici_Chiusura_Esercizio.Data_Creazione, 103), '')  " & vbCrLf)

            ' ---- FROM
            strSql.Append(" 		   From CDG_Testata Join CDG_Dettagli On " & vbCrLf)
            strSql.Append(" CDG_Testata.piva_superuser = CDG_Dettagli.piva_superuser And " & vbCrLf)
            strSql.Append(" CDG_Testata.piva  = CDG_Dettagli.piva  And " & vbCrLf)
            strSql.Append(" CDG_Testata.id_cdg = CDG_Dettagli.id_cdg    " & vbCrLf)
            strSql.Append("  Join agenda on agenda.id_agenda = CDG_Testata.id_agenda " & vbCrLf)
            strSql.Append("  Left Join operazioni on agenda.lav_cod = operazioni.lav_cod " & vbCrLf)
            strSql.Append("  Left Join movimenti on movimenti.id_mov = CDG_Testata.id_mov " & vbCrLf)
            strSql.Append("  Left Join movimenti_dettagli on movimenti_dettagli.id_mov_det = CDG_Testata.id_mov_det " & vbCrLf)
            strSql.Append("  Left Join Parco_Macchine on  Parco_Macchine.Mac_Cod = CDG_Testata.Mac_Cod " & vbCrLf)
            strSql.Append("  Left Join Risorse_Umane on  Risorse_Umane.Cod_Risum = CDG_Testata.Cod_Risum " & vbCrLf)
            strSql.Append("  Left Join contatti on CDG_Testata.piva = Contatti.Piva And Risorse_Umane.cod_contatto = contatti.cod_contatto " & vbCrLf)

            strSql.Append("  Left Join Materie_Prime on CDG_Testata.Elem_cod = Materie_Prime.Elem_cod And CDG_Testata.mat_cod = Materie_Prime.mat_cod " & vbCrLf)
            strSql.Append("  And (  " & vbCrLf)
            strSql.Append("    CDG_Testata.Elem_cod = 10 " & vbCrLf)
            strSql.Append("  Or CDG_Testata.Elem_cod = 200 " & vbCrLf)
            strSql.Append("  or CDG_Testata.Elem_cod = 201 " & vbCrLf)
            strSql.Append("  or CDG_Testata.Elem_cod = 204 " & vbCrLf)
            strSql.Append("  or CDG_Testata.Elem_cod = 205 " & vbCrLf)
            strSql.Append("  or CDG_Testata.Elem_cod = 210 " & vbCrLf)
            strSql.Append("  Or CDG_Testata.Elem_cod = 301 " & vbCrLf)
            strSql.Append("  Or CDG_Testata.Elem_cod = 304 " & vbCrLf)
            strSql.Append("  Or CDG_Testata.Elem_cod = 305 " & vbCrLf)
            strSql.Append("  Or CDG_Testata.Elem_cod = 306 " & vbCrLf)
            strSql.Append("  Or CDG_Testata.Elem_cod = 307 " & vbCrLf)
            strSql.Append("  Or CDG_Testata.Elem_cod = 310 " & vbCrLf)
            strSql.Append("  Or CDG_Testata.Elem_cod = 401 " & vbCrLf)
            strSql.Append("  Or CDG_Testata.Elem_cod = 700 " & vbCrLf)
            strSql.Append("  )  " & vbCrLf)

            strSql.Append("  Left Join Categorie on Categorie.COD = 'S' + Right('000000' + CONVERT(varchar(6), CDG_Testata.pro_cod), 6)  " & vbCrLf)
            strSql.Append("  And CDG_Testata.elem_cod = 555  " & vbCrLf)

            strSql.Append("  Left Join carburanti on carburanti.car_cod = CDG_Testata.pro_cod  " & vbCrLf)
            strSql.Append("  And CDG_Testata.elem_cod = 2   " & vbCrLf)

            strSql.Append("  Left Join fertilizzanti on fertilizzanti.fer_cod = CDG_Testata.pro_cod  " & vbCrLf)
            strSql.Append("  And CDG_Testata.elem_cod = 3   " & vbCrLf)

            'Se servirà tenere presente che Cer_Cod è varchar e un e di codice è "02 01 08" strSql.Append("  Left Join CatalogoEuropeoRifiuti on CatalogoEuropeoRifiuti.Cer_Cod = CDG_Testata.pro_cod  " & vbCrLf)
            'strSql.Append("  And CDG_Testata.elem_cod = 4   " & vbCrLf)

            'strSql.Append("  Left Join TipologieSementi on TipologieSementi.sem_cod = CDG_Testata.pro_cod  " & vbCrLf)
            'strSql.Append("  And CDG_Testata.elem_cod = 10   " & vbCrLf)

            strSql.Append("  Left Join formulati on formulati.fr_cod = CDG_Testata.pro_cod  " & vbCrLf)
            strSql.Append("  And CDG_Testata.elem_cod = 191   " & vbCrLf)

            strSql.Append("  Left Join Coadiuvante on Coadiuvante.coad_cod = CDG_Testata.pro_cod  " & vbCrLf)
            strSql.Append("  And CDG_Testata.elem_cod = 195   " & vbCrLf)

            strSql.Append("  Left Join insettiutili on insettiutili.ins_cod = CDG_Testata.pro_cod  " & vbCrLf)
            strSql.Append("  And CDG_Testata.elem_cod = 196   " & vbCrLf)

            strSql.Append("  Left Join trappole on trappole.trap_cod = CDG_Testata.pro_cod  " & vbCrLf)
            strSql.Append("  And CDG_Testata.elem_cod = 197   " & vbCrLf)

            strSql.Append("  Left Join Avversita on Avversita.av_cod = CDG_Testata.pro_cod  " & vbCrLf)
            strSql.Append("  And CDG_Testata.elem_cod = 198   " & vbCrLf)

            strSql.Append("  Left Join Zoo_Animali on Zoo_Animali.piva = CDG_Testata.piva  " & vbCrLf)
            strSql.Append("  And Zoo_Animali.cod_progetto = CDG_Testata.pro_cod  " & vbCrLf)
            strSql.Append("  And CDG_Testata.elem_cod = 300   " & vbCrLf)

            strSql.Append("  Left Join Attivita on CDG_Testata.id_attivita = Attivita.id_attivita " & vbCrLf)

            strSql.Append("  Left Join Qualifiche on CDG_Testata.Piva_SuperUser = Qualifiche.Piva_SuperUser And  " & vbCrLf)
            strSql.Append("  CDG_Testata.Piva = Qualifiche.Piva And " & vbCrLf)
            strSql.Append("  CDG_Testata.Qualifica_Cod = Qualifiche.Qualifica_Cod " & vbCrLf)

            strSql.AppendLine("  Left Join Tariffe on  CDG_Testata.Piva_SuperUser = Tariffe.Piva And  ")
            strSql.AppendLine("  CDG_Testata.Tariffa_Cod = Tariffe.Tariffa_Cod ")

            strSql.AppendLine("  Left Join Turni on  CDG_Testata.Piva_SuperUser = Turni.Piva And  ")
            strSql.AppendLine("  CDG_Testata.Turno_Cod = Turni.Turno_Cod ")

            strSql.Append("  Left Join Conti on  CDG_Testata.Piva = Conti.Piva And  " & vbCrLf)
            strSql.Append("  CDG_Testata.Conto_Cod = Conti.Cod_Conto " & vbCrLf)

            strSql.Append("  Left Join Centri_Aziendali ON CDG_Dettagli.Piva = Centri_Aziendali.Piva " & vbCrLf)
            strSql.Append(" And CDG_Dettagli.Sa_Cod = Centri_Aziendali.sa_cod " & vbCrLf)

            strSql.Append(" Left Join Appezzamento ON CDG_Dettagli.Piva = Appezzamento.Piva " & vbCrLf)
            strSql.Append(" And CDG_Dettagli.Sa_Cod = Appezzamento.sa_cod " & vbCrLf)
            strSql.Append(" And CDG_Dettagli.appezza = Appezzamento.appezza  " & vbCrLf)

            strSql.Append(" Left Join reg_impianti ON " & vbCrLf)
            strSql.Append("  CDG_Dettagli.Piva = reg_impianti.Piva  " & vbCrLf)
            strSql.Append(" And CDG_Dettagli.Sa_Cod = reg_impianti.sa_cod  " & vbCrLf)
            strSql.Append(" And CDG_Dettagli.appezza = reg_impianti.appezza  " & vbCrLf)
            strSql.Append(" And CDG_Dettagli.Id_Reg = reg_impianti.id_reg " & vbCrLf)

            strSql.Append(" LEFT JOIN Imprese_Progetti ON CDG_Dettagli.PIVA = Imprese_Progetti.Piva " & vbCrLf)
            strSql.Append(" And CDG_Dettagli.SA_COD = Imprese_Progetti.Sa_Cod " & vbCrLf)
            strSql.Append(" And CDG_Dettagli.APPEZZA = Imprese_Progetti.Appezza " & vbCrLf)
            strSql.Append(" And CDG_Dettagli.ID_REG = Imprese_Progetti.Id_Reg  " & vbCrLf)
            strSql.Append(" And CDG_Dettagli.Progetto_Cod = Imprese_Progetti.Progetto_Cod " & vbCrLf)

            strSql.Append("  Left Join  Regolamenti ON Imprese_Progetti.Regolamento_Cod = Regolamenti.Reg_Cod " & vbCrLf)

            strSql.Append("  LEFT JOIN  DPI_Regolamenti on Imprese_Progetti.Disciplinare_Cod=DPI_Regolamenti.COD_REGOLAMENTO " & vbCrLf)
            strSql.Append("  And Imprese_Progetti.Disciplinare_PubblicoPrivato=DPI_Regolamenti.Flag_Privato_Pubblico " & vbCrLf)

            strSql.Append(" Left Join Imprese ON Reg_Impianti.Piva = Imprese.Piva  " & vbCrLf)

            ' INIZIO Con queste sotto ci sarebbe duplicazione di righe se l'appezzamento è su più particelle
            'strSql.Append(" Left OUTER JOIN AppezzamentiXParticelle  " & vbCrLf)
            'strSql.Append(" On Reg_Impianti.Piva = AppezzamentiXParticelle.Piva  " & vbCrLf)
            'strSql.Append(" And Reg_Impianti.Sa_Cod = AppezzamentiXParticelle.Sa_Cod  " & vbCrLf)
            'strSql.Append(" And Reg_Impianti.Appezza = AppezzamentiXParticelle.Appezza   " & vbCrLf)

            'strSql.Append(" Left OUTER JOIN ParticelleCatastali  " & vbCrLf)
            'strSql.Append(" On ParticelleCatastali.PROV = AppezzamentiXParticelle.PROV  " & vbCrLf)
            'strSql.Append(" And ParticelleCatastali.COM = AppezzamentiXParticelle.COM  " & vbCrLf)
            'strSql.Append(" And ParticelleCatastali.SEZIONE = AppezzamentiXParticelle.SEZIONE  " & vbCrLf)
            'strSql.Append(" And ParticelleCatastali.FOGLIO = AppezzamentiXParticelle.FOGLIO  " & vbCrLf)
            'strSql.Append(" And ParticelleCatastali.NUMERO = AppezzamentiXParticelle.NUMERO  " & vbCrLf)
            'strSql.Append(" And ParticelleCatastali.SUBALTERNO = AppezzamentiXParticelle.SUBALTERNO  " & vbCrLf)

            'strSql.Append("  Left OUTER JOIN ISTAT IstatParticelle ON AppezzamentiXParticelle.PROV = ISTATParticelle.PROV And AppezzamentiXParticelle.COM = ISTATParticelle.COM  " & vbCrLf)

            'strSql.Append("  Left OUTER JOIN Lista_Province ON Lista_Province.SIGLA = ISTATParticelle.Comuni_Prov  " & vbCrLf)
            'strSql.Append(" Left OUTER JOIN Lista_Regioni ON Lista_Regioni.REG = Lista_Province.REG  " & vbCrLf)
            ' FINE Con queste sotto ci sarebbe duplicazione di righe se l'appezzamento è su più particelle

            strSql.Append(" Left OUTER JOIN  Campi ON Appezzamento.Piva = Campi.Piva  " & vbCrLf)
            strSql.Append(" And Appezzamento.Sa_Cod = Campi.Sa_Cod  " & vbCrLf)
            strSql.Append(" And Appezzamento.Campo_Cod = Campi.Campo_Cod  " & vbCrLf)

            strSql.Append(" Left Join Appezzamento_codici  On " & vbCrLf)
            strSql.Append("   Appezzamento_codici.piva = appezzamento.piva and  Appezzamento_codici.SA_COD = appezzamento.SA_COD And " & vbCrLf)
            strSql.Append("   Appezzamento_codici.APPEZZA = appezzamento.APPEZZA  AND Appezzamento_codici.id_cod = 1018 " & vbCrLf)

            strSql.Append("  Left OUTER JOIN Portinnesti ON Reg_Impianti.Port_COD = Portinnesti.Port_Cod  " & vbCrLf)

            strSql.Append("   Left OUTER JOIN ImpiantiIrrigazioni ON Reg_Impianti.Imp_COD = ImpiantiIrrigazioni.Imp_Cod  " & vbCrLf)

            strSql.Append(" Left OUTER JOIN CentrixIndirizzi  " & vbCrLf)
            strSql.Append(" On Centri_Aziendali.Piva = CentrixIndirizzi.Piva  " & vbCrLf)
            strSql.Append(" And Centri_Aziendali.sa_cod = CentrixIndirizzi.sa_cod  " & vbCrLf)
            strSql.Append(" And CentrixIndirizzi.tipo_indirizzo = 1  " & vbCrLf)

            strSql.Append(" Left OUTER JOIN Indirizzi IndCentro ON CentrixIndirizzi.cod_indirizzo = IndCentro.cod_indirizzo  " & vbCrLf)

            strSql.Append(" Left OUTER JOIN ISTAT IstatCentro ON IndCentro.pro_cod_istat = IstatCentro.PROV And IndCentro.com_cod_istat = IstatCentro.COM  " & vbCrLf)

            strSql.Append(" Left OUTER JOIN ImpresexIndirizzi  " & vbCrLf)
            strSql.Append(" On Imprese.Piva = ImpresexIndirizzi.Piva  " & vbCrLf)
            strSql.Append(" And ImpresexIndirizzi.tipo_indirizzo = 1  " & vbCrLf)

            strSql.Append(" Left OUTER JOIN Indirizzi IndAzienda ON ImpresexIndirizzi.cod_indirizzo = IndAzienda.cod_indirizzo  " & vbCrLf)

            strSql.Append(" Left OUTER JOIN ISTAT IstatAzienda ON IndAzienda.pro_cod_istat = IstatAzienda.PROV And IndAzienda.com_cod_istat = IstatAzienda.COM  " & vbCrLf)

            strSql.Append("  Left OUTER JOIN Cultivar ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod  " & vbCrLf)

            strSql.Append(" Left OUTER JOIN SpecieVegetali  ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod  " & vbCrLf)

            strSql.Append(" Left OUTER JOIN GruppoVegetale ON SpecieVegetali.Gru_Cod = GruppoVegetale.Gru_Cod  " & vbCrLf)
            strSql.Append("  Left OUTER JOIN GruppoFinalita ON Reg_Impianti.GRFI_COD = GruppoFinalita.Grfi_Cod  " & vbCrLf)

            strSql.Append(" Left Join Parco_Macchine As Parco_Macchine_Input_Costi on  Parco_Macchine_Input_Costi.Mac_Cod = CDG_Dettagli.Macchine_Cod " & vbCrLf)

            strSql.Append(" Left Join Imputazioni on Imputazioni.Piva_SuperUser = CDG_Dettagli.Piva_SuperUser And " & vbCrLf)
            strSql.Append(" Imputazioni.Piva = CDG_Dettagli.Piva And " & vbCrLf)
            strSql.Append(" Imputazioni.Imputazione_Cod = CDG_Dettagli.Id_Imputazione " & vbCrLf)

            strSql.Append("                Left Join Imputazioni_Classi on Imputazioni_Classi.Piva_SuperUser = Imputazioni.Piva_SuperUser And " & vbCrLf)
            strSql.Append(" Imputazioni_Classi.Piva = Imputazioni.Piva And " & vbCrLf)
            strSql.Append(" Imputazioni_Classi.Imputazione_Classe_Cod = Imputazioni.Imputazione_Classe_Cod " & vbCrLf)

            strSql.Append("                Left Join Imputazioni_Tipi on Imputazioni_Tipi.Piva_SuperUser = Imputazioni.Piva_SuperUser And " & vbCrLf)
            strSql.Append(" Imputazioni_Tipi.Piva = Imputazioni.Piva And " & vbCrLf)
            strSql.Append(" Imputazioni_Tipi.Tipo_Imputazione = Imputazioni.Tipo_Imputazione " & vbCrLf)

            strSql.Append("                Left OUTER JOIN Linee_Produzioni   ON CDG_Dettagli.Piva  = Linee_Produzioni.Piva  And " & vbCrLf)
            strSql.Append(" CDG_Dettagli.Linea_Cod = Linee_Produzioni.Linea_Cod " & vbCrLf)

            strSql.Append("  INNER JOIN unitamisura ON CDG_Testata.udm_cod = unitamisura.udm_Cod  " & vbCrLf)

            strSql.Append("  LEFT OUTER JOIN  Reg_Impianti_Codici As Reg_Impianti_Codici_Codice_Impianto on Reg_Impianti.PIVA=Reg_Impianti_Codici_Codice_Impianto.PIVA And Reg_Impianti.SA_COD=Reg_Impianti_Codici_Codice_Impianto.SA_COD And Reg_Impianti.APPEZZA=Reg_Impianti_Codici_Codice_Impianto.APPEZZA And Reg_Impianti.id_reg=Reg_Impianti_Codici_Codice_Impianto.id_reg And Reg_Impianti_Codici_Codice_Impianto.id_cod = " & enum_CodiciAnagrafe.Codice_Impianto & " " & vbCrLf)

            strSql.Append("  LEFT OUTER JOIN  Reg_Impianti_Codici As Reg_Impianti_Codici_Chiusura_Esercizio on Imprese_Progetti.PIVA=Reg_Impianti_Codici_Chiusura_Esercizio.PIVA And Imprese_Progetti.SA_COD=Reg_Impianti_Codici_Chiusura_Esercizio.SA_COD And Imprese_Progetti.APPEZZA=Reg_Impianti_Codici_Chiusura_Esercizio.APPEZZA And Imprese_Progetti.id_reg=Reg_Impianti_Codici_Chiusura_Esercizio.id_reg And Imprese_Progetti.progetto_cod=Reg_Impianti_Codici_Chiusura_Esercizio.progetto_cod And Reg_Impianti_Codici_Chiusura_Esercizio.id_cod = " & enum_CodiciAnagrafe.Distinta_Chiusa & " And Reg_Impianti_Codici_Chiusura_Esercizio.val_cod = 1 " & vbCrLf)

            'WHERE
            strSql.Append(" WHERE CDG_Testata.Budget = 0 And CDG_Testata.Id_agenda in " & Agro_SQL_Save_Clausola_IN(idsAgenda) & " And CDG_Testata.Vecchio_Tipo_Inser_Dati = " & Vecchio_Tipo_Inser_Dati & vbCrLf)

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri_Server, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("" & nomeRoutine & " : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function CDG_BI_Esterna_Delete(ByVal piva As String,
                              ByRef objParametri As AgronicaCoreParametri
                              ) As String

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.DW_CDG_Costi_Ricavi_DAL_R.CDG_BI_Esterna_Delete()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            ' N.B. con questa si intercettano anche le modifiche perché quando si salvano i costi/ricavi gli id di CDG_Testata
            '      vengono riassegnati
            strSql.Length = 0

            strSql.Append(" Delete from CDG_BI_Esterna Where Exists (select * from Agronica_Log_Agenda where Agronica_Log_Agenda.Piva = CDG_BI_Esterna.Piva " & " ")
            strSql.Append(" and Agronica_Log_Agenda.Id_Agenda = CDG_BI_Esterna.Id_Agenda " & " ")
            strSql.Append(" and Agronica_Log_Agenda.Data_Ora_RegistrazioneLog >= CDG_BI_Esterna.Data_Creazione) " & " ")
            strSql.Append(" And Piva = '" & Agro_SQL_SaveText(piva) & "' ")


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("" & nomeRoutine & " : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function CDG_BI_Esterna_Estrazione(ByVal piva As String,
                                   ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                   ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.DW_CDG_Scrivi.DW_CDG_BI_Esterna_Estrazione()"

        '====================================================================================
        'Parametri opzionali :
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Dim pivaSuperUser = objParametri_Server.PivaSuperUser
        Dim Leggi_DW_CDG As New DW_CDG_Costi_Ricavi_DAL_R
        Dim flagJoinSuperUserCac As Boolean = Leggi_DW_CDG.GetFlagJoinCac(objParametri_Utenti)

        Try


            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" INSERT INTO CDG_BI_Esterna ( ")
            strSql.AppendLine("  Piva_Superuser ")
            strSql.AppendLine(" ,Piva ")
            strSql.AppendLine(" ,Id_Agenda ")
            strSql.AppendLine(" ,Id_CDG ")
            strSql.AppendLine(" ,Id_CDG_Dettagli ")
            strSql.AppendLine(" , Codice_Impianto ")
            strSql.AppendLine(" , Progetto_Nome ")
            strSql.AppendLine(" ,Tipologia_Movimento ")
            strSql.AppendLine(" ,Data_Inserimento ")
            strSql.AppendLine(" , Cod_Articolo ")
            strSql.AppendLine(" , NrBadge ")
            strSql.AppendLine(" ,Udm_Cod ")
            strSql.AppendLine(" ,Udm_Des ")
            strSql.AppendLine(" ,Prezzo_Unitario ")
            strSql.AppendLine(" ,Qta ")
            strSql.AppendLine(" ,Valore ")

            strSql.AppendLine(" ,Percentuale_Ripart ")
            strSql.AppendLine(" ,Lotto ")
            strSql.AppendLine(" ,Id_Attivita ")
            strSql.AppendLine(" , Data_Creazione ")
            strSql.AppendLine(" , Data_Modifica ")

            strSql.AppendLine("  ,Mac_Cod ")
            strSql.AppendLine(" ,Cod_RisUm ")
            strSql.AppendLine(" ,Elem_Cod ")
            strSql.AppendLine(" ,Pro_Cod ")
            strSql.AppendLine(" ,Mat_Cod ")

            strSql.AppendLine(" ,Qualifica_Cod ")
            strSql.AppendLine(" ,Tariffa_Cod ")
            strSql.AppendLine(" ,Turno_Cod ")

            '-------------------------

            '''''strSql.Append(" ,Descr_Modalita_Imputazione " & vbCrLf)
            '''''strSql.Append(" ,Id_Agenda " & vbCrLf)
            '''''strSql.Append(" ,Id_Mov " & vbCrLf)
            '''''strSql.Append(" ,Id_Mov_Det " & vbCrLf)

            '''''strSql.Append(" ,Conto_Cod " & vbCrLf)
            '''''strSql.Append(" ,Mezzo " & vbCrLf)


            '''''strSql.Append(" ,Descrizione " & vbCrLf)
            '''''strSql.Append(" ,Budget_Cons " & vbCrLf)
            '''''strSql.Append(" ,Budget_Cons_Des " & vbCrLf)
            '''''strSql.Append(" ,Costi_Ricavi " & vbCrLf)
            '''''strSql.Append(" ,Costi_Ricavi_Des " & vbCrLf)
            '''''strSql.Append(" ,Sa_Cod " & vbCrLf)
            '''''strSql.Append(" ,Appezza " & vbCrLf)
            '''''strSql.Append(" ,Id_Reg " & vbCrLf)
            '''''strSql.Append(" ,Id_Cod_reg_impianti_codici " & vbCrLf)
            '''''strSql.Append(" ,Id_Imputazione " & vbCrLf)
            '''''strSql.Append(" ,Tipo_Imputazione " & vbCrLf)
            '''''strSql.Append(" ,Macchine_Cod " & vbCrLf)
            '''''strSql.Append(" ,Linea_Cod " & vbCrLf)
            '''''strSql.Append(", Veg_Cod  " & vbCrLf)
            '''''strSql.Append(", Cul_Cod " & vbCrLf)
            '''''strSql.Append(" , Descrizione_Operazione " & vbCrLf)
            '''''strSql.Append(" , Descrizione_Macchina " & vbCrLf)
            '''''strSql.Append(" , Nome_Cognome " & vbCrLf)


            '''''strSql.Append(" , Categoria " & vbCrLf)

            '''''strSql.Append(" , Descrizione_Prodotto " & vbCrLf)
            '''''strSql.Append(" , Attivita " & vbCrLf)

            '''''strSql.Append(" , Conto " & vbCrLf)
            '''''strSql.Append(" , Ragione_Sociale_Azienda " & vbCrLf)
            '''''strSql.Append(" , Indirizzo_Azienda " & vbCrLf)
            '''''strSql.Append(" , Cap_Azienda " & vbCrLf)
            '''''strSql.Append(" , Localita_Azienda " & vbCrLf)
            '''''strSql.Append(" , Prov_Azienda " & vbCrLf)
            '''''strSql.Append(" , Descrizione_Centro " & vbCrLf)
            '''''strSql.Append(" , Indirizzo_Centro " & vbCrLf)
            '''''strSql.Append(" , Cap_Centro " & vbCrLf)
            '''''strSql.Append(" , Localita_Centro " & vbCrLf)
            '''''strSql.Append(" , Prov_Centro " & vbCrLf)
            '''''strSql.Append(" , Descrizione_Campo " & vbCrLf)
            '''''strSql.Append(" , Nome_Appezzamento " & vbCrLf)
            '''''strSql.Append(" , Inizio_Appezzamento " & vbCrLf)
            '''''strSql.Append(" , Fine_Appezzamento " & vbCrLf)
            '''''strSql.Append(" , Descrizione_Gruppo_Vegetale " & vbCrLf)
            '''''strSql.Append(" , Specie_impianto " & vbCrLf)
            '''''strSql.Append(" , Varieta_impianto " & vbCrLf)
            '''''strSql.Append(" , Descrizione_Finalita " & vbCrLf)
            '''''strSql.Append(" , Destinazione_Uso " & vbCrLf)
            '''''strSql.Append(" , Inizio_Impianto " & vbCrLf)
            '''''strSql.Append(" , Fine_Impianto " & vbCrLf)

            '''''strSql.Append(" , Progetto_Nome " & vbCrLf)
            '''''strSql.Append(" , Regolamento " & vbCrLf)
            '''''strSql.Append(" , Disciplinare " & vbCrLf)

            '''''strSql.Append(" , Esposizione_Appezzamento " & vbCrLf)
            '''''strSql.Append(" , Ubicazione_Appezzamento " & vbCrLf)
            '''''strSql.Append(" , Descrizione_Portinnesti " & vbCrLf)
            '''''strSql.Append(" , Descrizione_Irrigazioni " & vbCrLf)
            '''''strSql.Append(" , Regione_Impianto " & vbCrLf)
            '''''strSql.Append(" , Prov_Impianto " & vbCrLf)
            '''''strSql.Append(" , Comune_Impianto " & vbCrLf)
            '''''strSql.Append(" , Sezione " & vbCrLf)
            '''''strSql.Append(" , Foglio " & vbCrLf)
            '''''strSql.Append(" , Numero " & vbCrLf)
            '''''strSql.Append(" , Subalterno " & vbCrLf)
            '''''strSql.Append(" , Descrizione_Macchina_Input_Costi " & vbCrLf)
            '''''strSql.Append(" , Progetto " & vbCrLf)
            '''''strSql.Append(" , Classe_Progetto " & vbCrLf)
            '''''strSql.Append(" , Tipo_Progetto " & vbCrLf)
            '''''strSql.Append(" , Linea_Produzione " & vbCrLf)
            '''''strSql.Append(" , Campo_Cod " & vbCrLf)
            '''''strSql.Append(" , Lotto_Input_Costi " & vbCrLf)
            '''''strSql.Append(" , Progetto_Des " & vbCrLf)
            '''''strSql.Append(" , Progetto_Validita_Inizio " & vbCrLf)
            '''''strSql.Append(" , Progetto_Validita_Fine " & vbCrLf)
            '''''strSql.Append(" , Note " & vbCrLf)
            '''''strSql.Append(" , Vecchio_Tipo_Inser_Dati " & vbCrLf)
            '''''strSql.Append(" , inviato " & vbCrLf)
            '''''strSql.Append(" , datainvio " & vbCrLf)

            '''''strSql.Append(" , Username_Creazione " & vbCrLf)
            '''''strSql.Append(" , Username_Modifica " & vbCrLf)
            '''''strSql.Append(" , Validita_Inizio " & vbCrLf)
            '''''strSql.Append(" , Validita_Fine " & vbCrLf)

            ''''''Zoo
            '''''strSql.Append(" ,Cod_Animale " & vbCrLf)
            '''''strSql.Append(" ,Animale_Progetto " & vbCrLf)
            '''''strSql.Append(" ,Cod_Animale_Distinta " & vbCrLf)
            '''''strSql.Append(" ,Animale_Distinta " & vbCrLf)
            '''''strSql.Append(" ,Sta_Num " & vbCrLf)
            '''''strSql.Append(" ,Stalla_Des " & vbCrLf)
            '''''strSql.Append(" ,Raggruppamento_Cod  " & vbCrLf)
            '''''strSql.Append(" ,Raggruppamento_Des  " & vbCrLf)
            '''''strSql.Append(" ,Specie_Animale_Cod  " & vbCrLf)
            '''''strSql.Append(" ,Specie_Animale_des  " & vbCrLf)
            '''''strSql.Append(" ,Razza_Animale_Cod " & vbCrLf)
            '''''strSql.Append(" ,Razza_Animale_Des " & vbCrLf)
            '''''strSql.Append(" ,Tipo_Animale_Cod  " & vbCrLf)
            '''''strSql.Append(" ,Tipo_Animale_Des  " & vbCrLf)
            '''''strSql.Append(" ,Modalita_Imputazione  " & vbCrLf)


            '''''strSql.Append(" , MetodoProduzioneAppezzamento " & vbCrLf)
            '''''strSql.Append(" , Sup_app " & vbCrLf)
            '''''strSql.Append(" , Sup_imp " & vbCrLf)
            '''''strSql.Append(" , Sup_Prog " & vbCrLf)
            '''''strSql.Append(" , Data_Chiusura_Esercizio " & vbCrLf)
            strSql.Append(" ) " & vbCrLf)

            strSql.AppendLine(" Select ")

            strSql.AppendLine("  CDG_Testata.Piva_Superuser ")
            strSql.AppendLine(" , CDG_Testata.Piva ")
            strSql.AppendLine("  ,CDG_Testata.Id_Agenda ")
            strSql.AppendLine(", CDG_Testata.Id_CDG ")
            strSql.AppendLine(" ,CDG_Dettagli.Id_CDG_Dettagli ")

            strSql.AppendLine(", Case  When  ")
            strSql.AppendLine("   ISNULL(Imputazioni.Imputazione_Cod, 0) != 0 Then Imputazioni.CodicePrincipale ")
            strSql.AppendLine(" Else ")
            strSql.AppendLine("   ISNULL(Reg_Impianti_Codici_Codice_Impianto.Val_Cod, '') " & vbCrLf)
            strSql.AppendLine(" END ")

            strSql.AppendLine(", Case  When  ")
            strSql.AppendLine("   ISNULL(Imputazioni.Imputazione_Cod, 0) != 0 Then Imputazioni.CodiceSecondario ")
            strSql.AppendLine(" Else ")
            strSql.AppendLine("   ISNULL(imprese_progetti.Progetto_Nome, '')  ")
            strSql.AppendLine(" END ")

            strSql.AppendLine(" , Case  ")
            strSql.AppendLine("    When CDG_Testata.Cod_Risum != 0 Then 'MOD' ")
            strSql.AppendLine("    When ISNULL(Imputazioni.Imputazione_Cod, 0) != 0 Then 'MOI' ")
            strSql.AppendLine("    When CDG_Testata.Elem_Cod IN (201, 210)   Then 'PRODOTTO FINITO' ")
            strSql.AppendLine("    When CDG_Testata.Elem_Cod IN (3, 10, 191, 304)  Then 'COMPONENTE' ")
            strSql.AppendLine("    When CDG_Testata.Elem_Cod IN (700)  Then 'CONTO LAVORO' ")
            strSql.AppendLine("    When CDG_Testata.mac_cod != 0 Then 'MOD 2' ")
            'strSql.AppendLine("    When ????? Then 'SOTTOPRODOTTO' " )
            'strSql.AppendLine("    When CDG_Testata.Elem_Cod IN (3, 10, 191, 304) 0 Then 'AFFITTO' " )
            'strSql.AppendLine("    When CDG_Testata.Elem_Cod IN (3, 10, 191, 304) 0 Then 'Irrigazione' " )
            strSql.AppendLine("    ELSE  ''  ")
            strSql.AppendLine("     END  ")

            strSql.AppendLine(" ,CDG_Testata.Data_Inserimento  ")
            strSql.AppendLine(" , Case  ")
            strSql.AppendLine("    When ISNULL(Materie_Prime.Cod_Articolo, '') <> '' Then Materie_Prime.Cod_Articolo ")
            strSql.AppendLine("    WHEN ISNULL(Materie_Prime.Cod_Articolo, '') = '' Then  ")
            'filtro ulteriormente per elem cod, perché così se non c'è alcuna codifica per quella categoria faccio meno lavoro
            strSql.AppendLine("         CASE  ")
            strSql.AppendLine("             WHEN NOT EXISTS(SELECT 1 FROM CAC_Codifica_ProdottiAziendali WHERE Elem_Cod = CDG_Testata.Elem_Cod)  ")
            strSql.AppendLine("             THEN '' ")
            strSql.AppendLine("         ELSE   ")
            strSql.AppendLine("             ISNULL((SELECT TOP(1) Cod_Prodotto_Cliente ")
            strSql.AppendLine("                  FROM CAC_Codifica_ProdottiAziendali ")
            strSql.AppendLine("             WHERE Elem_Cod = CDG_Testata.Elem_Cod ")
            strSql.AppendLine("             AND Codice_GIAS = CDG_Testata.Pro_Cod ")
            If flagJoinSuperUserCac AndAlso pivaSuperUser <> "" Then
                strSql.AppendLine("             AND Piva_SuperUser = '" & Agro_SQL_SaveText(pivaSuperUser) & "'), '') ")
            Else
                strSql.AppendLine("             AND Piva = CDG_Testata.Piva), '')  ")
            End If
            strSql.AppendLine("    END  ")
            strSql.AppendLine(" END ")
            strSql.AppendLine(", ISNULL(Contatti.NrBadge, '') ")
            strSql.AppendLine(" ,CDG_Testata.udm_cod ")
            strSql.AppendLine(" ,CASE   WHEN CDG_Testata.udm_cod = 0 Then 'Ore' Else ISNULL(unitamisura.udm_sim, '') END  ")
            strSql.AppendLine(" ,CDG_Testata.Prezzo_Unitario   ")
            strSql.AppendLine(" ,CDG_Testata.Qta / 100 * CDG_Dettagli.Valore  ")
            strSql.AppendLine(" , CDG_Testata.Valore_Totale / 100 * CDG_Dettagli.Valore ")
            strSql.AppendLine(" , CDG_Dettagli.Valore  ")
            strSql.AppendLine(" ,CDG_Testata.Lotto  ")
            strSql.AppendLine(" ,CDG_Testata.Id_Attivita ")
            strSql.AppendLine("	 , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            strSql.AppendLine("	 , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            strSql.AppendLine(" ,CDG_Testata.Mac_Cod  ")
            strSql.AppendLine(" ,CDG_Testata.Cod_RisUm  ")
            strSql.AppendLine(" ,CDG_Testata.Elem_Cod  ")
            strSql.AppendLine(" ,CDG_Testata.Pro_Cod  ")
            strSql.AppendLine(" ,CDG_Testata.Mat_Cod  ")
            strSql.AppendLine(" ,CDG_Testata.Qualifica_Cod  ")
            strSql.AppendLine(" ,CDG_Testata.Tariffa_Cod  ")
            strSql.AppendLine(" ,CDG_Testata.Turno_Cod  ")
            '-------------------------
            '''''strSql.Append(" ,0 ")
            '''''strSql.Append(" , Case  ")
            '''''strSql.Append("    When CDG_Testata.Modalita_imputazione = 0 And Cdg_Dettagli.Cod_Animale = 0 Then 'Op. QdC' ")
            '''''strSql.Append("    When CDG_Testata.Modalita_imputazione = 0 And Cdg_Dettagli.Cod_Animale != 0 Then 'Op. Zoo' " & vbCrLf)
            '''''strSql.Append("    WHEN CDG_Testata.Modalita_imputazione = 1 And CDG_Testata.OrigineAPP = 0 Then 'Diretta' " & vbCrLf)
            '''''strSql.Append("    WHEN CDG_Testata.Modalita_imputazione = 1 And CDG_Testata.OrigineAPP IN (1,2,3,4) Then 'App' " & vbCrLf)
            '''''strSql.Append("    WHEN CDG_Testata.Modalita_imputazione = 2 Then 'TimeSheet' " & vbCrLf)
            '''''strSql.Append("    WHEN CDG_Testata.Modalita_imputazione = 4 Then 'Contabilità' " & vbCrLf)
            '''''strSql.Append("    ELSE  ''  " & vbCrLf)
            '''''strSql.Append("     END  " & vbCrLf)

            '''''strSql.Append(" ,CDG_Testata.Id_Mov " & vbCrLf)
            '''''strSql.Append(" ,CDG_Testata.Id_Mov_Det " & vbCrLf)


            '''''strSql.Append(" ,CDG_Testata.Conto_Cod  " & vbCrLf)

            '''''' Imposto la descrizione solo quando vengo da libera imputazione; negli altri casi il campo Descrizione contiene la decodifica del prodotto, 
            '''''' della persona, del prodotto e del servizio che ottengo già con le specifiche join
            '''''strSql.Append(", CASE   WHEN CDG_Testata.Tab_Imputazione = 'LIBERA' Then ISNULL(CDG_Testata.Descrizione, '')  Else '' END    " & vbCrLf)
            '''''strSql.Append(" ,CDG_Testata.Budget   " & vbCrLf)
            '''''strSql.Append(" ,CASE   WHEN CDG_Testata.Budget = 0 Then 'Consuntivo' Else 'Budget' END  " & vbCrLf)
            '''''strSql.Append(" ,CDG_Testata.Costi_Ricavi  " & vbCrLf)
            '''''strSql.Append(" ,CASE   WHEN CDG_Testata.Costi_Ricavi = 0 Then 'Costi' Else 'Ricavi' END  " & vbCrLf)
            '''''strSql.Append(" ,CDG_Dettagli.Sa_Cod  " & vbCrLf)
            '''''strSql.Append(" ,CDG_Dettagli.Appezza  " & vbCrLf)
            '''''strSql.Append(" ,CDG_Dettagli.Id_Reg  " & vbCrLf)
            '''''strSql.Append(" ,CDG_Dettagli.Id_Cod_reg_impianti_codici  " & vbCrLf)
            '''''strSql.Append(" ,CDG_Dettagli.Id_Imputazione  " & vbCrLf)
            '''''strSql.Append(", ISNULL(Imputazioni.Tipo_Imputazione, 0)    " & vbCrLf)
            '''''strSql.Append(" ,CDG_Dettagli.Macchine_Cod  " & vbCrLf)
            '''''strSql.Append(" ,CDG_Dettagli.Linea_Cod  " & vbCrLf)
            '''''strSql.Append(", ISNULL(SpecieVegetali.Veg_Cod, 0)   " & vbCrLf)
            '''''strSql.Append(", ISNULL(Cultivar.Cul_Cod, 0)   " & vbCrLf)
            '''''strSql.Append(", ISNULL(des_lib, '') " & vbCrLf)
            '''''strSql.Append(", ISNULL(Parco_Macchine.Mac_Des, '')   " & vbCrLf)
            '''''strSql.Append(", ISNULL(Contatti.Cognome, '')  + ' ' +  ISNULL(Contatti.Nome, '')   " & vbCrLf)

            '''''strSql.Append(" , CASE  " & vbCrLf)
            '''''strSql.Append("    WHEN CDG_Testata.cod_risum != 0 Then 'Personale' " & vbCrLf)
            '''''strSql.Append("    WHEN CDG_Testata.mac_cod != 0 Then 'Macchine e attrezzature' " & vbCrLf)
            '''''strSql.Append("    WHEN CDG_Testata.elem_cod = 501 Then 'Altri beni' " & vbCrLf)
            '''''strSql.Append("    WHEN CDG_Testata.elem_cod = 555 Then 'Servizi' " & vbCrLf)
            '''''strSql.Append("    WHEN CDG_Testata.elem_cod = 2   Then 'Carburanti' " & vbCrLf)
            '''''strSql.Append("    WHEN CDG_Testata.elem_cod = 3   Then 'Fertilizzanti' " & vbCrLf)
            '''''strSql.Append("    WHEN CDG_Testata.elem_cod = 4   Then 'Rifiuti' " & vbCrLf)
            '''''strSql.Append("    WHEN CDG_Testata.elem_cod = 10  Then 'Semente e Materiale Vivaistico' " & vbCrLf)
            '''''strSql.Append("    WHEN CDG_Testata.elem_cod = 191 Then 'Formulati' " & vbCrLf)
            '''''strSql.Append("    WHEN CDG_Testata.elem_cod = 195 Then 'Coadiuvanti' " & vbCrLf)
            '''''strSql.Append("    WHEN CDG_Testata.elem_cod = 196 Then 'Insetti utili' " & vbCrLf)
            '''''strSql.Append("    WHEN CDG_Testata.elem_cod = 197 Then 'Trappole' " & vbCrLf)
            '''''strSql.Append("    WHEN CDG_Testata.elem_cod = 198 Then 'Avversità' " & vbCrLf)
            '''''strSql.Append("    WHEN CDG_Testata.elem_cod = 300 Then 'Consistenza Zootecnica' " & vbCrLf)
            '''''strSql.Append("    WHEN CDG_Testata.elem_cod = 200 Then 'Altre Risorse' " & vbCrLf)
            '''''strSql.Append("    WHEN CDG_Testata.elem_cod = 201 Then 'Semilavorati Vegetali' " & vbCrLf)
            '''''strSql.Append("    WHEN CDG_Testata.elem_cod = 204 Then 'Materie Prime Vegetali' " & vbCrLf)
            '''''strSql.Append("    WHEN CDG_Testata.elem_cod = 205 Then 'Beni Confezionamento Vegetale' " & vbCrLf)
            '''''strSql.Append("    WHEN CDG_Testata.elem_cod = 210 Then 'Trasformati Vegetali' " & vbCrLf)
            '''''strSql.Append("    ELSE  ''  " & vbCrLf)
            '''''strSql.Append("     END  " & vbCrLf)


            '''''strSql.Append(" , CASE  " & vbCrLf)
            '''''strSql.Append("    WHEN CDG_Testata.elem_cod = 501 And cdg_testata.pro_cod = 0 Then ISNULL(Movimenti_Dettagli.Mov_Det_Des, '') " & vbCrLf)
            '''''strSql.Append("    WHEN CDG_Testata.elem_cod = 555 And cdg_testata.pro_cod != 0 Then ISNULL(Categorie.descr, '') " & vbCrLf)
            '''''strSql.Append("    WHEN CDG_Testata.elem_cod = 2   And cdg_testata.pro_cod != 0 Then ISNULL(Carburanti.Car_Des, '') " & vbCrLf)
            '''''strSql.Append("    WHEN CDG_Testata.elem_cod = 3   And cdg_testata.pro_cod != 0 Then ISNULL(fertilizzanti.fer_des, '') " & vbCrLf)
            ''''''strSql.Append("    WHEN CDG_Testata.elem_cod = 4   And cdg_testata.pro_cod != 0 Then ISNULL(CatalogoEuropeoRifiuti.Cer_Des, '') " & vbCrLf)
            '''''strSql.Append("    WHEN CDG_Testata.elem_cod = 10  And cdg_testata.pro_cod != 0 Then ISNULL(TipologieSementi.Sem_Des, '') " & vbCrLf)
            '''''strSql.Append("    WHEN CDG_Testata.elem_cod = 191 And cdg_testata.pro_cod != 0 Then ISNULL(formulati.fr_des, '')  " & vbCrLf)
            '''''strSql.Append("    WHEN CDG_Testata.elem_cod = 195 And cdg_testata.pro_cod != 0 Then ISNULL(Coadiuvante.Coad_Des, '')  " & vbCrLf)
            '''''strSql.Append("    WHEN CDG_Testata.elem_cod = 196 And cdg_testata.pro_cod != 0 Then ISNULL(InsettiUtili.Ins_Des, '')  " & vbCrLf)
            '''''strSql.Append("    WHEN CDG_Testata.elem_cod = 197 And cdg_testata.pro_cod != 0 Then ISNULL(trappole.trap_des, '') " & vbCrLf)
            '''''strSql.Append("    WHEN CDG_Testata.elem_cod = 198 And cdg_testata.pro_cod != 0 Then ISNULL(Avversita.Av_Des_Vol, '') " & vbCrLf)
            '''''strSql.Append("    WHEN CDG_Testata.elem_cod = 300 And cdg_testata.pro_cod != 0 Then ISNULL(Zoo_Animali_Test.Matricola, '') " & vbCrLf)
            '''''strSql.Append("    WHEN (CDG_Testata.elem_cod = 200 Or CDG_Testata.elem_cod = 201 Or CDG_Testata.elem_cod = 204 Or CDG_Testata.elem_cod = 205 Or CDG_Testata.elem_cod = 210) And cdg_testata.pro_cod != 0 Then ISNULL(Materie_Prime.Mat_Des, '') " & vbCrLf)
            '''''strSql.Append("    WHEN cdg_testata.pro_cod = 0 And cdg_testata.mat_cod != '' Then ISNULL(Materie_Prime.Mat_Des, '')   " & vbCrLf)
            '''''strSql.Append("    ELSE  ''  " & vbCrLf)
            '''''strSql.Append("     END  " & vbCrLf)
            '''''strSql.Append(", ISNULL(Attivita.[Desc], '')   " & vbCrLf)
            '''''strSql.Append(" , ISNULL(Qualifica_des, '')   " & vbCrLf)
            '''''strSql.Append(", ISNULL(Tariffa_des, '')   " & vbCrLf)
            '''''strSql.Append(" , ISNULL(Turno_Des, '')   " & vbCrLf)
            '''''strSql.Append(", ISNULL(Conto_Descr, '')   " & vbCrLf)
            '''''strSql.Append(" , ISNULL(Imprese.rag_soc, '')   " & vbCrLf)
            '''''strSql.Append(", ISNULL(IndAzienda.ind_des, '')   " & vbCrLf)
            '''''strSql.Append(" ,ISNULL(IndAzienda.CAP, '')   " & vbCrLf)
            '''''strSql.Append(", ISNULL(IstatAzienda.LOCALITA, '') " & vbCrLf)
            '''''strSql.Append(" , ISNULL(IstatAzienda.COMUNI_PROV, '') " & vbCrLf)
            '''''strSql.Append(", ISNULL(Centri_Aziendali.sa_nome, '') " & vbCrLf)
            '''''strSql.Append(" ,  ISNULL(IndCentro.ind_des, '')   " & vbCrLf)
            '''''strSql.Append(", ISNULL(IndCentro.CAP, '') " & vbCrLf)
            '''''strSql.Append(" , ISNULL(ISTATCentro.LOCALITA, '')  " & vbCrLf)
            '''''strSql.Append(" , ISNULL(ISTATCentro.COMUNI_PROV, '') " & vbCrLf)
            '''''strSql.Append(", ISNULL(Campi.Campo_Des, '') " & vbCrLf)
            '''''strSql.Append(" ,  ISNULL(Appezzamento.APP_NOME, '') " & vbCrLf)
            '''''strSql.Append(" , ISNULL(Appezzamento.Validita_Inizio, " & Agro_SQL_SaveDateTime(AGRODATAINIZIO) & ")  " & vbCrLf)
            '''''strSql.Append(" , ISNULL(Appezzamento.Validita_Fine, " & Agro_SQL_SaveDateTime(AGRODATAFINE) & ")  " & vbCrLf)
            '''''strSql.Append(" ,  ISNULL(GruppoVegetale.Gru_Des, '')  " & vbCrLf)
            '''''strSql.Append(", ISNULL(SpecieVegetali.Veg_Des, '')   " & vbCrLf)
            '''''strSql.Append(" ,  ISNULL(Cultivar.Cul_Des, '')   " & vbCrLf)

            '''''strSql.Append(", ISNULL(GruppoFinalita.Grfi_Des, '')   " & vbCrLf)

            ''''''strSql.Append(" ,  ISNULL(( SELECT TOP 1 Codici_Anagrafe.Descrizione  " & vbCrLf)
            ''''''strSql.Append(" From Reg_Impianti_Codici  " & vbCrLf)
            ''''''strSql.Append(" INNER Join Codici_Anagrafe ON Reg_Impianti_Codici.id_cod = Codici_Anagrafe.codice  " & vbCrLf)
            ''''''strSql.Append(" WHERE(Reg_Impianti_Codici.PIVA = Reg_Impianti.PIVA) " & vbCrLf)
            ''''''strSql.Append(" And   (Reg_Impianti_Codici.Sa_Cod = Reg_Impianti.sa_cod)  " & vbCrLf)
            ''''''strSql.Append(" And   (Reg_Impianti_Codici.Appezza  = Reg_Impianti.Appezza)  " & vbCrLf)
            ''''''strSql.Append(" And   (Reg_Impianti_Codici.Id_Reg = Reg_Impianti.Id_Reg)  " & vbCrLf)
            ''''''strSql.Append(" And   (Reg_Impianti_Codici.Progetto_Cod = 0)  " & vbCrLf)
            ''''''strSql.Append(" And   (Reg_Impianti_Codici.id_cod = CDG_Dettagli.Id_Cod_reg_impianti_codici)  " & vbCrLf)
            ''''''strSql.Append(" And (Codici_Anagrafe.gruppo = 'TERRENO')  " & vbCrLf)
            ''''''strSql.Append(" ) , '')   " & vbCrLf)
            '''''' IMPOSTO DIRETTAMENTE TERRENO NUDO
            '''''strSql.Append(", Case  When  " & vbCrLf)
            '''''strSql.Append("   ISNULL(Reg_Impianti.Id_Reg, 0) = 0 Or ISNULL(Reg_impianti.Cul_Cod, 0) != 0 Then '' " & vbCrLf)
            '''''strSql.Append(" Else " & vbCrLf)
            '''''strSql.Append("  ISNULL(( SELECT TOP 1 Codici_Anagrafe.Descrizione  " & vbCrLf)
            '''''strSql.Append(" From Reg_Impianti_Codici  " & vbCrLf)
            '''''strSql.Append(" INNER Join Codici_Anagrafe ON Reg_Impianti_Codici.id_cod = Codici_Anagrafe.codice  " & vbCrLf)
            '''''strSql.Append(" WHERE(Reg_Impianti_Codici.PIVA = Reg_Impianti.PIVA) " & vbCrLf)
            '''''strSql.Append(" And   (Reg_Impianti_Codici.Sa_Cod = Reg_Impianti.sa_cod)  " & vbCrLf)
            '''''strSql.Append(" And   (Reg_Impianti_Codici.Appezza  = Reg_Impianti.Appezza)  " & vbCrLf)
            '''''strSql.Append(" And   (Reg_Impianti_Codici.Id_Reg = Reg_Impianti.Id_Reg)  " & vbCrLf)
            '''''strSql.Append(" And   (Reg_Impianti_Codici.Progetto_Cod = 0)  " & vbCrLf)
            '''''strSql.Append(" And   (Reg_Impianti_Codici.id_cod = CDG_Dettagli.Id_Cod_reg_impianti_codici)  " & vbCrLf)
            '''''strSql.Append(" And (Codici_Anagrafe.gruppo = 'TERRENO')  " & vbCrLf)
            '''''strSql.Append(" ) , 'TERRENO NUDO')    " & vbCrLf)
            '''''strSql.Append(" END " & vbCrLf)

            '''''strSql.Append(" ,ISNULL(Convert(varchar(10), Reg_Impianti.Validita_Inizio, 103), '')  " & vbCrLf)
            '''''strSql.Append(" , ISNULL(CONVERT(varchar(10), Reg_Impianti.Validita_Fine, 103), '')   " & vbCrLf)

            '''''strSql.Append(" ,  ISNULL(Regolamenti.Reg_Des, '')  " & vbCrLf)
            '''''strSql.Append(" ,  ISNULL(dpi_Regolamenti.NomeEsteso, '')  " & vbCrLf)

            '''''strSql.Append(", ISNULL(Appezzamento.ESPOSIZ, '')   " & vbCrLf)
            '''''strSql.Append(" ,  ISNULL(Appezzamento.ubicazione, '')   " & vbCrLf)
            '''''strSql.Append("  , ISNULL(Portinnesti.Port_Des, '')  " & vbCrLf)
            '''''strSql.Append(" , ISNULL(ImpiantiIrrigazioni.Imp_Des, '')   " & vbCrLf)
            '''''' INIZIO Con queste sotto ci sarebbe duplicazione di righe se l'appezzamento è su più particelle
            ''''''strSql.Append("         ,   ISNULL(Lista_Regioni.Regione_Des, '')  " & vbCrLf)
            ''''''strSql.Append(" , ISNULL(ISTATParticelle.COMUNI_PROV, '')  " & vbCrLf)
            ''''''strSql.Append(", ISNULL(ISTATParticelle.LOCALITA, '')  " & vbCrLf)
            ''''''strSql.Append(" , ISNULL(AppezzamentiXParticelle.SEZIONE, '')   " & vbCrLf)
            ''''''strSql.Append(" ,ISNULL(AppezzamentiXParticelle.FOGLIO, -1)   " & vbCrLf)
            ''''''strSql.Append(" ,ISNULL(AppezzamentiXParticelle.NUMERO, -1)   " & vbCrLf)
            ''''''strSql.Append(" ,ISNULL(AppezzamentiXParticelle.SUBALTERNO, '')  " & vbCrLf)
            '''''strSql.Append(" , ''  " & vbCrLf)
            '''''strSql.Append(" , ''  " & vbCrLf)
            '''''strSql.Append(", ''  " & vbCrLf)
            '''''strSql.Append(" , ''  " & vbCrLf)
            '''''strSql.Append(" , -1  " & vbCrLf)
            '''''strSql.Append(" , -1   " & vbCrLf)
            '''''strSql.Append(" , ''  " & vbCrLf)
            '''''' FINE Con queste sotto ci sarebbe duplicazione di righe se l'appezzamento è su più particelle

            '''''strSql.Append("       , ISNULL(Parco_Macchine_Input_Costi.mac_des, '')   " & vbCrLf)
            '''''strSql.Append(" , ISNULL(Imputazione_Nome, '')   " & vbCrLf)
            '''''strSql.Append("  , ISNULL(Imputazione_Classe_Des, '')   " & vbCrLf)
            '''''strSql.Append(" , ISNULL(Tipo_Imputazione_Des, '')  " & vbCrLf)
            '''''strSql.Append(" , ISNULL(Linea_Des, '')  " & vbCrLf)
            '''''strSql.Append(" ,  ISNULL(Appezzamento.Campo_Cod, 0) " & vbCrLf)
            '''''strSql.Append(" , CDG_Dettagli.Lotto_Input_Costi  " & vbCrLf)
            '''''strSql.Append(" ,  ISNULL(imprese_progetti.Progetto_Cod, 0)  " & vbCrLf)
            '''''strSql.Append(" ,  ISNULL(imprese_progetti.Progetto_Des, '')  " & vbCrLf)
            '''''strSql.Append(" ,  ISNULL(imprese_progetti.validita_inizio, '')  " & vbCrLf)
            '''''strSql.Append(" ,  ISNULL(imprese_progetti.validita_fine, '')  " & vbCrLf)

            '''''strSql.Append("  , ISNULL(Movimenti.mov_desc, '') " & vbCrLf)
            '''''strSql.Append("  , CDG_Testata.Vecchio_Tipo_Inser_Dati " & vbCrLf)
            '''''strSql.Append(" ,CDG_Dettagli.inviato  " & vbCrLf)
            '''''strSql.Append(" ,CDG_Dettagli.datainvio  " & vbCrLf)
            '''''strSql.Append("	 , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            '''''strSql.Append("	 , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            '''''strSql.Append("  , '" & Agro_SQL_SaveText(objParametri_Server.UsernameOperazione) & "'  ")
            '''''strSql.Append("  , '" & Agro_SQL_SaveText(objParametri_Server.UsernameOperazione) & "'  ")
            '''''strSql.Append("	 , " & Agro_SQL_SaveDateTime(AGRODATAINIZIO) & "  ")
            '''''strSql.Append("	 , " & Agro_SQL_SaveDateTime(AGRODATAFINE) & "  ")

            '''''strSql.Append(" ,CDG_Dettagli.Cod_Animale " & vbCrLf)
            '''''strSql.Append(" ,ISNULL(Zoo_Animali.Progetto, '') " & vbCrLf)
            '''''strSql.Append(" ,CDG_Dettagli.Cod_Animale_Distinta " & vbCrLf)
            '''''strSql.Append(" ,ISNULL(Zoo_Animali_Distinte.Progetto_Nome, '') " & vbCrLf)
            '''''strSql.Append(" ,CDG_Dettagli.Sta_Num  " & vbCrLf)
            '''''strSql.Append(" ,ISNULL(Stalla.Fabbricato_DES, '')  " & vbCrLf)
            '''''strSql.Append(" ,CDG_Dettagli.Raggruppamento_Cod  " & vbCrLf)
            '''''strSql.Append(" ,ISNULL(Stalla_Raggruppamenti.Raggruppamento_Des, '')  " & vbCrLf)
            '''''strSql.Append(" ,ISNULL(Zoo_Animali.SPE_COD, 0) " & vbCrLf)
            '''''strSql.Append(" ,ISNULL(Lista_Specie_Animali.SPE_DES, '') " & vbCrLf)
            '''''strSql.Append(" ,ISNULL(Zoo_Animali.RAZ_COD, 0) " & vbCrLf)
            '''''strSql.Append(" ,ISNULL(Lista_Razze_Animali.RAZ_DES, '') " & vbCrLf)
            '''''strSql.Append(" ,ISNULL(Zoo_Animali.GEN_Cod, 0)  " & vbCrLf)
            '''''strSql.Append(" ,ISNULL(Zoo_Animali_Lista_Tipi.Tipo_Des, '')  " & vbCrLf)
            '''''strSql.Append(" ,CDG_Testata.Modalita_Imputazione " & vbCrLf)



            '''''strSql.Append(" , Case  " & vbCrLf)
            '''''strSql.Append("    When ISNULL(Appezzamento_codici.val_Cod, '') = '' Then '' " & vbCrLf)
            '''''strSql.Append("    WHEN ISNULL(Appezzamento_codici.val_Cod, '') = '1' Then 'Convenzionale' " & vbCrLf)
            '''''strSql.Append("    WHEN ISNULL(Appezzamento_codici.val_Cod, '') = '2' Then 'In Conversione' " & vbCrLf)
            '''''strSql.Append("    WHEN ISNULL(Appezzamento_codici.val_Cod, '') = '3' Then 'Biologico' " & vbCrLf)
            '''''strSql.Append("    ELSE  ''  " & vbCrLf)
            '''''strSql.Append("     END  " & vbCrLf)
            '''''strSql.Append(" , ISNULL(Appezzamento.Sup_app, 0)   " & vbCrLf)
            '''''strSql.Append(" , ISNULL(Reg_Impianti.Sup_imp, 0)   " & vbCrLf)
            '''''strSql.Append(" , ISNULL(Imprese_Progetti.Sup_Prog, 0)   " & vbCrLf)

            '''''strSql.Append(" ,ISNULL(Convert(varchar(10), Reg_Impianti_Codici_Chiusura_Esercizio.Data_Creazione, 103), '')  " & vbCrLf)
            '''
            ' ---- FROM
            strSql.AppendLine(" 		   From CDG_Testata Join CDG_Dettagli On ")
            strSql.AppendLine(" CDG_Testata.piva_superuser = CDG_Dettagli.piva_superuser And ")
            strSql.AppendLine(" CDG_Testata.piva  = CDG_Dettagli.piva  And ")
            strSql.AppendLine(" CDG_Testata.id_cdg = CDG_Dettagli.id_cdg    ")
            '''''strSql.Append("  Join agenda on agenda.id_agenda = CDG_Testata.id_agenda " & vbCrLf)

            'strSql.Append("  Left Join  Mov_Dettagli_Riferimenti ON CDG_Testata.id_agenda = Id_Agenda_Rif " & vbCrLf)
            'strSql.Append("  And Lav_Cod_Rif = 4500 " & vbCrLf)

            '''''strSql.Append("  Left Join operazioni on agenda.lav_cod = operazioni.lav_cod " & vbCrLf)
            strSql.AppendLine("  Left Join movimenti on movimenti.id_mov = CDG_Testata.id_mov ")
            strSql.AppendLine("  Left Join movimenti_dettagli on movimenti_dettagli.id_mov_det = CDG_Testata.id_mov_det ")
            strSql.AppendLine("  Left Join Parco_Macchine on  Parco_Macchine.Mac_Cod = CDG_Testata.Mac_Cod ")
            strSql.AppendLine("  Left Join Risorse_Umane on  Risorse_Umane.Cod_Risum = CDG_Testata.Cod_Risum ")
            strSql.AppendLine("  Left Join contatti on (CDG_Testata.piva = Contatti.Piva OR Contatti.sa_cod = -1) And Risorse_Umane.cod_contatto = contatti.cod_contatto ")

            strSql.AppendLine("  Left Join Materie_Prime on CDG_Testata.mat_cod = Materie_Prime.mat_cod ")
            strSql.AppendLine("  And (  ")
            strSql.AppendLine("   CDG_Testata.mat_cod != 0  ")
            strSql.AppendLine("  Or CDG_Testata.Elem_cod = 10 ")
            strSql.AppendLine("  Or CDG_Testata.Elem_cod = 200 ")
            strSql.AppendLine("  Or CDG_Testata.Elem_cod = 201 ")
            strSql.AppendLine("  Or CDG_Testata.Elem_cod = 204 ")
            strSql.AppendLine("  Or CDG_Testata.Elem_cod = 205 ")
            strSql.AppendLine("  Or CDG_Testata.Elem_cod = 210 ")
            strSql.AppendLine("  )  ")

            strSql.AppendLine("  Left Join Categorie on Categorie.COD = 'S' + Right('000000' + CONVERT(varchar(6), CDG_Testata.pro_cod), 6)  ")
            strSql.AppendLine("  And CDG_Testata.elem_cod = 555  ")

            'strSql.Append("  Left Join CAC_Codifica_ProdottiAziendali on  " & vbCrLf)
            'strSql.Append(" CAC_Codifica_ProdottiAziendali.piva_superuser = CDG_Dettagli.piva_superuser And " & vbCrLf)
            'strSql.Append(" CAC_Codifica_ProdottiAziendali.piva  = CDG_Dettagli.piva  And " & vbCrLf)
            'strSql.Append(" CAC_Codifica_ProdottiAziendali.Tipo_Codifica  = " & enum_Tipo_CAC_Codifica_ProdottiAziendali.NonDefinito & " And " & vbCrLf)
            'strSql.Append(" CAC_Codifica_ProdottiAziendali.Elem_Cod  = CDG_Dettagli.Elem_Cod  And " & vbCrLf)
            'strSql.Append(" CAC_Codifica_ProdottiAziendali.piva  = CDG_Dettagli.piva  And " & vbCrLf)


            strSql.AppendLine("  Left Join carburanti on carburanti.car_cod = CDG_Testata.pro_cod  ")
            strSql.AppendLine("  And CDG_Testata.elem_cod = 2   ")

            strSql.AppendLine("  Left Join fertilizzanti on fertilizzanti.fer_cod = CDG_Testata.pro_cod  ")
            strSql.AppendLine("  And CDG_Testata.elem_cod = 3   ")

            'Se servirà tenere presente che Cer_Cod è varchar e un e di codice è "02 01 08" strSql.Append("  Left Join CatalogoEuropeoRifiuti on CatalogoEuropeoRifiuti.Cer_Cod = CDG_Testata.pro_cod  " )
            'strSql.Append("  And CDG_Testata.elem_cod = 4   " )

            'strSql.AppendLine("  Left Join TipologieSementi on TipologieSementi.sem_cod = CDG_Testata.pro_cod  ")
            'strSql.AppendLine("  And CDG_Testata.elem_cod = 10   ")

            strSql.AppendLine("  Left Join formulati on formulati.fr_cod = CDG_Testata.pro_cod  ")
            strSql.AppendLine("  And CDG_Testata.elem_cod = 191   ")

            strSql.AppendLine("  Left Join Coadiuvante on Coadiuvante.coad_cod = CDG_Testata.pro_cod  ")
            strSql.AppendLine("  And CDG_Testata.elem_cod = 195   ")

            strSql.AppendLine("  Left Join insettiutili on insettiutili.ins_cod = CDG_Testata.pro_cod  ")
            strSql.AppendLine("  And CDG_Testata.elem_cod = 196   ")

            strSql.AppendLine("  Left Join trappole on trappole.trap_cod = CDG_Testata.pro_cod  ")
            strSql.AppendLine("  And CDG_Testata.elem_cod = 197   ")

            strSql.AppendLine("  Left Join Avversita on Avversita.av_cod = CDG_Testata.pro_cod  ")
            strSql.AppendLine("  And CDG_Testata.elem_cod = 198   ")

            '''''strSql.AppendLine("  Left Join Zoo_Animali Zoo_Animali_Test on Zoo_Animali_Test.piva = CDG_Testata.piva  ")
            '''''strSql.AppendLine("  And Zoo_Animali_Test.cod_progetto = CDG_Testata.pro_cod  ")
            '''''strSql.AppendLine("  And CDG_Testata.elem_cod = 300   ")

            '''''strSql.Append("  Left Join Attivita on CDG_Testata.id_attivita = Attivita.id_attivita " & vbCrLf)

            '''''strSql.Append("  Left Join Qualifiche on CDG_Testata.Piva_SuperUser = Qualifiche.Piva_SuperUser And  " & vbCrLf)
            '''''strSql.Append("  CDG_Testata.Piva = Qualifiche.Piva And " & vbCrLf)
            '''''strSql.Append("  CDG_Testata.Qualifica_Cod = Qualifiche.Qualifica_Cod " & vbCrLf)

            '''''strSql.Append("  Left Join Tariffe on  CDG_Testata.Piva_SuperUser = Tariffe.Piva And  " & vbCrLf)
            '''''strSql.Append("  CDG_Testata.Tariffa_Cod = Tariffe.Tariffa_Cod " & vbCrLf)

            '''''strSql.Append("  Left Join Turni on  CDG_Testata.Piva_SuperUser = Turni.Piva And  " & vbCrLf)
            '''''strSql.Append("  CDG_Testata.Turno_Cod = Turni.Turno_Cod " & vbCrLf)

            '''''strSql.Append("  Left Join Conti on  CDG_Testata.Piva = Conti.Piva And  " & vbCrLf)
            '''''strSql.Append("  CDG_Testata.Conto_Cod = Conti.Cod_Conto " & vbCrLf)

            '''''strSql.Append("  Left Join Centri_Aziendali ON CDG_Dettagli.Piva = Centri_Aziendali.Piva " & vbCrLf)
            '''''strSql.Append(" And CDG_Dettagli.Sa_Cod = Centri_Aziendali.sa_cod " & vbCrLf)

            '''''strSql.Append(" Left Join Appezzamento ON CDG_Dettagli.Piva = Appezzamento.Piva " & vbCrLf)
            '''''strSql.Append(" And CDG_Dettagli.Sa_Cod = Appezzamento.sa_cod " & vbCrLf)
            '''''strSql.Append(" And CDG_Dettagli.appezza = Appezzamento.appezza  " & vbCrLf)

            strSql.AppendLine(" Left Join reg_impianti ON ")
            strSql.AppendLine("  CDG_Dettagli.Piva = reg_impianti.Piva  ")
            strSql.AppendLine(" And CDG_Dettagli.Sa_Cod = reg_impianti.sa_cod  ")
            strSql.AppendLine(" And CDG_Dettagli.appezza = reg_impianti.appezza  ")
            strSql.AppendLine(" And CDG_Dettagli.Id_Reg = reg_impianti.id_reg ")

            strSql.AppendLine(" LEFT JOIN Imprese_Progetti ON CDG_Dettagli.PIVA = Imprese_Progetti.Piva ")
            strSql.AppendLine(" And CDG_Dettagli.SA_COD = Imprese_Progetti.Sa_Cod ")
            strSql.AppendLine(" And CDG_Dettagli.APPEZZA = Imprese_Progetti.Appezza ")
            strSql.AppendLine(" And CDG_Dettagli.ID_REG = Imprese_Progetti.Id_Reg  ")
            strSql.AppendLine(" And CDG_Dettagli.Progetto_Cod = Imprese_Progetti.Progetto_Cod ")

            '''''strSql.Append("  Left Join  Regolamenti ON Imprese_Progetti.Regolamento_Cod = Regolamenti.Reg_Cod " & vbCrLf)

            '''''strSql.Append("  LEFT JOIN  DPI_Regolamenti on Imprese_Progetti.Disciplinare_Cod=DPI_Regolamenti.COD_REGOLAMENTO  " & vbCrLf)
            '''''strSql.Append("  And Imprese_Progetti.Disciplinare_PubblicoPrivato=DPI_Regolamenti.Flag_Privato_Pubblico " + vbCrLf)

            '''''strSql.Append(" Left Join Imprese ON Reg_Impianti.Piva = Imprese.Piva  " & vbCrLf)

            ' INIZIO Con queste sotto ci sarebbe duplicazione di righe se l'appezzamento è su più particelle
            'strSql.Append(" Left OUTER JOIN AppezzamentiXParticelle  " & vbCrLf)
            'strSql.Append(" On Reg_Impianti.Piva = AppezzamentiXParticelle.Piva  " & vbCrLf)
            'strSql.Append(" And Reg_Impianti.Sa_Cod = AppezzamentiXParticelle.Sa_Cod  " & vbCrLf)
            'strSql.Append(" And Reg_Impianti.Appezza = AppezzamentiXParticelle.Appezza   " & vbCrLf)

            'strSql.Append(" Left OUTER JOIN ParticelleCatastali  " & vbCrLf)
            'strSql.Append(" On ParticelleCatastali.PROV = AppezzamentiXParticelle.PROV  " & vbCrLf)
            'strSql.Append(" And ParticelleCatastali.COM = AppezzamentiXParticelle.COM  " & vbCrLf)
            'strSql.Append(" And ParticelleCatastali.SEZIONE = AppezzamentiXParticelle.SEZIONE  " & vbCrLf)
            'strSql.Append(" And ParticelleCatastali.FOGLIO = AppezzamentiXParticelle.FOGLIO  " & vbCrLf)
            'strSql.Append(" And ParticelleCatastali.NUMERO = AppezzamentiXParticelle.NUMERO  " & vbCrLf)
            'strSql.Append(" And ParticelleCatastali.SUBALTERNO = AppezzamentiXParticelle.SUBALTERNO  " & vbCrLf)

            'strSql.Append("  Left OUTER JOIN ISTAT IstatParticelle ON AppezzamentiXParticelle.PROV = ISTATParticelle.PROV And AppezzamentiXParticelle.COM = ISTATParticelle.COM  " & vbCrLf)

            'strSql.Append("  Left OUTER JOIN Lista_Province ON Lista_Province.SIGLA = ISTATParticelle.Comuni_Prov  " & vbCrLf)
            'strSql.Append(" Left OUTER JOIN Lista_Regioni ON Lista_Regioni.REG = Lista_Province.REG  " & vbCrLf)
            ' FINE Con queste sotto ci sarebbe duplicazione di righe se l'appezzamento è su più particelle

            '''''strSql.Append(" Left OUTER JOIN  Campi ON Appezzamento.Piva = Campi.Piva  " & vbCrLf)
            '''''strSql.Append(" And Appezzamento.Sa_Cod = Campi.Sa_Cod  " & vbCrLf)
            '''''strSql.Append(" And Appezzamento.Campo_Cod = Campi.Campo_Cod  " & vbCrLf)

            '''''strSql.Append(" Left Join Appezzamento_codici  On " & vbCrLf)
            '''''strSql.Append("   Appezzamento_codici.piva = appezzamento.piva and  Appezzamento_codici.SA_COD = appezzamento.SA_COD And " & vbCrLf)
            '''''strSql.Append("   Appezzamento_codici.APPEZZA = appezzamento.APPEZZA  AND Appezzamento_codici.id_cod = 1018 " & vbCrLf)

            '''''strSql.Append("  Left OUTER JOIN Portinnesti ON Reg_Impianti.Port_COD = Portinnesti.Port_Cod  " & vbCrLf)

            '''''strSql.Append("   Left OUTER JOIN ImpiantiIrrigazioni ON Reg_Impianti.Imp_COD = ImpiantiIrrigazioni.Imp_Cod  " & vbCrLf)

            '''''strSql.Append(" Left OUTER JOIN CentrixIndirizzi  " & vbCrLf)
            '''''strSql.Append(" On Centri_Aziendali.Piva = CentrixIndirizzi.Piva  " & vbCrLf)
            '''''strSql.Append(" And Centri_Aziendali.sa_cod = CentrixIndirizzi.sa_cod  " & vbCrLf)
            '''''strSql.Append(" And CentrixIndirizzi.tipo_indirizzo = 1  " & vbCrLf)

            '''''strSql.Append(" Left OUTER JOIN Indirizzi IndCentro ON CentrixIndirizzi.cod_indirizzo = IndCentro.cod_indirizzo  " & vbCrLf)

            '''''strSql.Append(" Left OUTER JOIN ISTAT IstatCentro ON IndCentro.pro_cod_istat = IstatCentro.PROV And IndCentro.com_cod_istat = IstatCentro.COM  " & vbCrLf)

            '''''strSql.Append(" Left OUTER JOIN ImpresexIndirizzi  " & vbCrLf)
            '''''strSql.Append(" On Imprese.Piva = ImpresexIndirizzi.Piva  " & vbCrLf)
            '''''strSql.Append(" And ImpresexIndirizzi.tipo_indirizzo = 1  " & vbCrLf)

            '''''strSql.Append(" Left OUTER JOIN Indirizzi IndAzienda ON ImpresexIndirizzi.cod_indirizzo = IndAzienda.cod_indirizzo  " & vbCrLf)

            '''''strSql.Append(" Left OUTER JOIN ISTAT IstatAzienda ON IndAzienda.pro_cod_istat = IstatAzienda.PROV And IndAzienda.com_cod_istat = IstatAzienda.COM  " & vbCrLf)

            '''''strSql.Append("  Left OUTER JOIN Cultivar ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod  " & vbCrLf)

            '''''strSql.Append(" Left OUTER JOIN SpecieVegetali  ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod  " & vbCrLf)

            '''''strSql.Append(" Left OUTER JOIN GruppoVegetale ON SpecieVegetali.Gru_Cod = GruppoVegetale.Gru_Cod  " & vbCrLf)
            '''''strSql.Append("  Left OUTER JOIN GruppoFinalita ON Reg_Impianti.GRFI_COD = GruppoFinalita.Grfi_Cod  " & vbCrLf)

            '''''strSql.Append(" Left Join Parco_Macchine As Parco_Macchine_Input_Costi on  Parco_Macchine_Input_Costi.Mac_Cod = CDG_Dettagli.Macchine_Cod " & vbCrLf)

            strSql.AppendLine(" Left Join Imputazioni on Imputazioni.Piva_SuperUser = CDG_Dettagli.Piva_SuperUser And ")
            strSql.AppendLine(" Imputazioni.Piva = CDG_Dettagli.Piva And ")
            strSql.AppendLine(" Imputazioni.Imputazione_Cod = CDG_Dettagli.Id_Imputazione ")

            '''''strSql.Append("                Left Join Imputazioni_Classi on Imputazioni_Classi.Piva_SuperUser = Imputazioni.Piva_SuperUser And " & vbCrLf)
            '''''strSql.Append(" Imputazioni_Classi.Piva = Imputazioni.Piva And " & vbCrLf)
            '''''strSql.Append(" Imputazioni_Classi.Imputazione_Classe_Cod = Imputazioni.Imputazione_Classe_Cod " & vbCrLf)

            '''''strSql.Append("                Left Join Imputazioni_Tipi on Imputazioni_Tipi.Piva_SuperUser = Imputazioni.Piva_SuperUser And " & vbCrLf)
            '''''strSql.Append(" Imputazioni_Tipi.Piva = Imputazioni.Piva And " & vbCrLf)
            '''''strSql.Append(" Imputazioni_Tipi.Tipo_Imputazione = Imputazioni.Tipo_Imputazione " & vbCrLf)

            '''''strSql.Append("                Left OUTER JOIN Linee_Produzioni   ON CDG_Dettagli.Piva  = Linee_Produzioni.Piva  And " & vbCrLf)
            '''''strSql.Append(" CDG_Dettagli.Linea_Cod = Linee_Produzioni.Linea_Cod " & vbCrLf)

            strSql.AppendLine("  INNER JOIN unitamisura ON CDG_Testata.udm_cod = unitamisura.udm_Cod  ")

            'Zoo
            '''''strSql.Append("  Left Join Zoo_Animali on Zoo_Animali.piva = CDG_Dettagli.piva  " & vbCrLf)
            '''''strSql.Append("  And Zoo_Animali.cod_progetto = CDG_Dettagli.Cod_Animale  " & vbCrLf)

            '''''strSql.AppendLine(" LEFT JOIN Zoo_Animali_Distinte ON Zoo_Animali.PIVA = Zoo_Animali_Distinte.PIVA " & vbCrLf)
            '''''strSql.AppendLine(" And Zoo_animali.Cod_Progetto = Zoo_Animali_Distinte.Cod_Animale " & vbCrLf)
            '''''strSql.AppendLine(" And CDG_Dettagli.Cod_Animale_Distinta = Zoo_Animali_Distinte.Cod_Progetto  " & vbCrLf)

            '''''strSql.AppendLine(" LEFT JOIN Fabbricati as Stalla  ON Stalla.piva = CDG_Dettagli.Piva " & vbCrLf)
            '''''strSql.AppendLine(" And Stalla.SA_COD=CDG_Dettagli.Sa_Cod " & vbCrLf)
            '''''strSql.AppendLine(" And Stalla.Fabbricato_Cod=CDG_Dettagli.Sta_Num " & vbCrLf)

            '''''strSql.AppendLine(" Left Join Stalla_Raggruppamenti ON CDG_Dettagli.Sa_Cod = Stalla_Raggruppamenti.sa_cod " & vbCrLf)
            '''''strSql.AppendLine(" AND CDG_Dettagli.Raggruppamento_Cod = Stalla_Raggruppamenti.Raggruppamento_Cod " & vbCrLf)

            '''''strSql.AppendLine(" Left JOIN Lista_Specie_Animali ON Zoo_Animali.GEN_COD = Lista_Specie_Animali.GEN_COD " & vbCrLf)
            '''''strSql.AppendLine(" And Zoo_Animali.SPE_COD = Lista_Specie_Animali.SPE_COD " & vbCrLf)

            '''''strSql.AppendLine(" Left JOIN Lista_Razze_Animali ON Zoo_Animali.GEN_COD = Lista_Razze_Animali.GEN_COD " & vbCrLf)
            '''''strSql.AppendLine(" And Zoo_Animali.SPE_COD = Lista_Razze_Animali.SPE_COD " & vbCrLf)
            '''''strSql.AppendLine(" And Zoo_Animali.RAZ_COD = Lista_Razze_Animali.RAZ_COD " & vbCrLf)

            '''''strSql.AppendLine(" Left JOIN Zoo_Animali_Lista_Tipi ON Zoo_Animali.GEN_COD = Zoo_Animali_Lista_Tipi.GEN_COD " & vbCrLf)
            '''''strSql.AppendLine("  And Zoo_Animali.SPE_COD = Zoo_Animali_Lista_Tipi.SPE_COD " & vbCrLf)
            '''''strSql.AppendLine("  And Zoo_Animali.TIPO_COD = Zoo_Animali_Lista_Tipi.TIPO_COD " & vbCrLf)

            strSql.AppendLine("  LEFT OUTER JOIN  Reg_Impianti_Codici As Reg_Impianti_Codici_Codice_Impianto on Reg_Impianti.PIVA=Reg_Impianti_Codici_Codice_Impianto.PIVA And Reg_Impianti.SA_COD=Reg_Impianti_Codici_Codice_Impianto.SA_COD And Reg_Impianti.APPEZZA=Reg_Impianti_Codici_Codice_Impianto.APPEZZA And Reg_Impianti.id_reg=Reg_Impianti_Codici_Codice_Impianto.id_reg And Reg_Impianti_Codici_Codice_Impianto.id_cod = " & enum_CodiciAnagrafe.Codice_Impianto)

            '''''strSql.Append("  LEFT OUTER JOIN  Reg_Impianti_Codici As Reg_Impianti_Codici_Chiusura_Esercizio on Imprese_Progetti.PIVA=Reg_Impianti_Codici_Chiusura_Esercizio.PIVA And Imprese_Progetti.SA_COD=Reg_Impianti_Codici_Chiusura_Esercizio.SA_COD And Imprese_Progetti.APPEZZA=Reg_Impianti_Codici_Chiusura_Esercizio.APPEZZA And Imprese_Progetti.id_reg=Reg_Impianti_Codici_Chiusura_Esercizio.id_reg And Imprese_Progetti.progetto_cod=Reg_Impianti_Codici_Chiusura_Esercizio.progetto_cod And Reg_Impianti_Codici_Chiusura_Esercizio.id_cod = " & enum_CodiciAnagrafe.Distinta_Chiusa & " And Reg_Impianti_Codici_Chiusura_Esercizio.val_cod = 1 " + vbCrLf)

            'WHERE
            strSql.Append(" WHERE CDG_Testata.Budget = 0 And cdg_Testata.piva = '" & Agro_SQL_SaveText(piva) & "'" & vbCrLf)
            'Escludo i cdg già presenti
            strSql.AppendLine(" And Not Exists (select * from CDG_BI_Esterna as BI where BI.Piva = cdg_Testata.Piva ")
            strSql.AppendLine(" And BI.Id_CDG = cdg_Testata.Id_CDG) ")

            strSql.AppendLine(" And (cdg_dettagli.progetto_cod != 0 Or  cdg_dettagli.Id_Imputazione  != 0) ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri_Server, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("" & nomeRoutine & " : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function CDG_BI_Esterna_Imposta_Valorizzazione_Prodotto(ByVal piva As String,
                                                       ByVal Id_CDG As Integer,
                                                       ByVal Id_CDG_Dettagli As Integer,
                                                       ByVal Prezzo_Unitario As Double,
                                                       ByRef objParametri As AgronicaCoreParametri
                                                       ) As String

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.DW_Imposta_Valorizzazione_Prodotto()"


        '====================================================================================
        'Parametri opzionali :
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try


            '---------------------------------------------
            strSql.Length = 0

            strSql.Append(" Update CDG_BI_Esterna ")
            strSql.Append(" Set Prezzo_Unitario = " & Agro_SQL_SaveNum(Prezzo_Unitario) & ", ")
            strSql.Append("     Valore =  Qta * " & Agro_SQL_SaveNum(Prezzo_Unitario) & ", ")
            strSql.Append("	 Data_Modifica =  " & Agro_SQL_SaveDateTime(Date.Now) & " ,  ")
            strSql.Append("  Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'  ")
            strSql.Append("  Where Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            strSql.Append("  And Id_CDG = " & Id_CDG & " ")
            strSql.Append("  And Id_CDG_Dettagli = " & Id_CDG_Dettagli & " ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("" & nomeRoutine & " : " & messaggioErrore)
        End Try

        Return xRisp


    End Function

End Class
