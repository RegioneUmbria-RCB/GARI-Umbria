Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports Newtonsoft.Json.Linq

'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################


Public Class Alert_Indice_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal piva As String,
                          ByVal ID_Indice As Integer,
                          ByRef objParametri As AgronicaCoreParametri,
                          Optional ByVal xSelezioneVariabile As enumSelezioneVariabile? = enumSelezioneVariabile.Selezione_TabellaCompleta
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreScadenziario_DAL.Alert_Indice_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable
        Dim pivaSuperUser = objParametri.PivaSuperUser

        Try
            Select Case xSelezioneVariabile
                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT Distinct Alert_Indice.* ")
                    StrSQL.Append(" FROM  Alert_Indice ")
                    StrSQL.Append(" WHERE Alert_Indice.PivaSuperUser = '" & Agro_SQL_SaveText(pivaSuperUser) & "' ")

                    If piva <> "" Then
                        If DataProviderFactory.Instance.TipoProvider = enum_DataProvidersType.OleDbProvider Then
                            StrSQL.Append(" And Alert_Indice.piva In ('" & Agro_SQL_Save_Clausola_IN(piva, True) & "', '') ")
                        Else
                            If DataProviderFactory.Instance.ParametrizzaQuery Then
                                StrSQL.Append(" And Alert_Indice.piva In (" & Agro_SQL_Save_Clausola_IN(piva, True) & ", '') ")
                            Else
                                StrSQL.Append(" And Alert_Indice.piva In ('" & Agro_SQL_Save_Clausola_IN(piva, True) & "', '') ")
                            End If

                        End If
                    End If

                    If ID_Indice <> 0 Then
                        StrSQL.Append(" AND Alert_Indice.ID_Indice = " & Agro_SQL_SaveNum(ID_Indice) & " ")
                    End If


                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT ID_Indice, TitoloIndice ,ChkObbligatorio AS ChkObbligatorio_Tipologia")
                    StrSQL.Append(" FROM  Alert_Indice ")
                    StrSQL.Append(" WHERE Alert_Indice.PivaSuperUser = '" & Agro_SQL_SaveText(pivaSuperUser) & "' ")

                    If piva <> "" Then
                        If DataProviderFactory.Instance.TipoProvider = enum_DataProvidersType.OleDbProvider Then
                            StrSQL.Append(" And Alert_Indice.piva In ('" & Agro_SQL_Save_Clausola_IN(piva, True) & "', '') ")
                        Else
                            If DataProviderFactory.Instance.ParametrizzaQuery Then
                                StrSQL.Append(" And Alert_Indice.piva In (" & Agro_SQL_Save_Clausola_IN(piva, True) & ", '') ")
                            Else
                                StrSQL.Append(" And Alert_Indice.piva In ('" & Agro_SQL_Save_Clausola_IN(piva, True) & "', '') ")
                            End If
                        End If
                    End If

                    If ID_Indice <> 0 Then
                        StrSQL.Append(" AND Alert_Indice.ID_Indice = " & Agro_SQL_SaveNum(ID_Indice) & " ")
                    End If

                    StrSQL.Append("ORDER BY  Alert_Indice.TitoloIndice ")

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


    Public Function LeggiIndicixTipologia(ByVal Piva As String,
                                          ByVal ID_Area As Integer,
                                          ByVal ID_Tipologia As Integer,
                                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                          ByVal ID_Indice As Integer,
                                          ByRef objParametri As AgronicaCoreParametri,
                                          Optional Data_Creazione As String = ""
                                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreScadenziario_DAL.Alert_Indice_R.LeggiIndicixTipologia()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable
        Dim pivaSuperUser = objParametri.PivaSuperUser

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT * ")
                    StrSQL.AppendLine(" FROM  Alert_IndicexTipologia ")
                    StrSQL.AppendLine(" WHERE PivaSuperUser = '" & Agro_SQL_SaveText(pivaSuperUser) & "' ")

                    'If Piva <> "" Then
                    '    StrSQL.AppendLine(" And Alert_IndicexTipologia.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    'End If

                    If ID_Area <> 0 Then
                        StrSQL.AppendLine(" AND ID_Area = " & Agro_SQL_SaveNum(ID_Area) & " ")
                    End If

                    If ID_Tipologia <> 0 Then
                        StrSQL.AppendLine(" AND ID_Tipologia = " & Agro_SQL_SaveNum(ID_Tipologia) & " ")
                    End If

                    If ID_Indice <> 0 Then
                        StrSQL.AppendLine(" AND ID_Indice = " & Agro_SQL_SaveNum(ID_Indice) & " ")
                    End If

                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT Distinct Alert_Indice.*, ID_Area, ID_Tipologia, Ordinamento, ChkObbligatorio_Tipologia ")
                    StrSQL.AppendLine(" FROM  Alert_Indice, Alert_IndicexTipologia ")
                    StrSQL.AppendLine(" WHERE Alert_Indice.PivaSuperUser =  Alert_IndicexTipologia.PivaSuperUser ")
                    StrSQL.AppendLine(" And   Alert_Indice.PivaSuperUser = '" & Agro_SQL_SaveText(pivaSuperUser) & "' ")
                    StrSQL.AppendLine(" And   Alert_Indice.ID_Indice =  Alert_IndicexTipologia.ID_Indice ")

                    If Data_Creazione <> "" Then
                        StrSQL.AppendLine(" And   Alert_Indice.Validita_Fine > " & Agro_SQL_SaveDate(Data_Creazione) & " ")
                    End If

                    'If Piva <> "" Then
                    '    StrSQL.AppendLine(" And Alert_Indice.Piva In ( '" & Agro_SQL_SaveText(Piva) & "', '') ")
                    'End If

                    If ID_Area <> 0 Then
                        StrSQL.AppendLine(" AND Alert_IndicexTipologia.ID_Area = " & Agro_SQL_SaveNum(ID_Area) & " ")
                    End If

                    If ID_Tipologia <> 0 Then
                        StrSQL.AppendLine(" AND Alert_IndicexTipologia.ID_Tipologia = " & Agro_SQL_SaveNum(ID_Tipologia) & " ")
                    End If

                    StrSQL.AppendLine(" Order By Ordinamento ")


                'Caricamento ListBox IndiciXTipologie
                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT DISTINCT Alert_Indice.ID_Indice , Alert_Indice.TitoloIndice ,  Alert_IndicexTipologia.Ordinamento , ")
                    StrSQL.AppendLine(" Alert_Indice.ChkObbligatorio, Alert_IndicexTipologia.ChkObbligatorio_Tipologia ,Alert_IndicexTipologia.ID_Area,  Alert_IndicexTipologia.ID_Tipologia")
                    StrSQL.AppendLine(" FROM  Alert_Indice , Alert_IndicexTipologia ")
                    StrSQL.AppendLine(" WHERE Alert_Indice.PivaSuperUser =  Alert_IndicexTipologia.PivaSuperUser ")
                    StrSQL.AppendLine(" And   Alert_Indice.PivaSuperUser = '" & Agro_SQL_SaveText(pivaSuperUser) & "' ")
                    StrSQL.AppendLine(" And   Alert_Indice.ID_Indice =  Alert_IndicexTipologia.ID_Indice ")


                    'If Piva <> "" Then
                    '    StrSQL.AppendLine(" And Alert_Indice.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    'End If

                    If ID_Area <> 0 Then
                        StrSQL.AppendLine(" AND Alert_IndicexTipologia.ID_Area = " & Agro_SQL_SaveNum(ID_Area) & " ")
                    End If

                    If ID_Tipologia <> 0 Then
                        StrSQL.AppendLine(" AND Alert_IndicexTipologia.ID_Tipologia = " & Agro_SQL_SaveNum(ID_Tipologia) & " ")
                    End If

                    If ID_Indice <> 0 Then
                        StrSQL.AppendLine(" And Alert_IndicexTipologia.ID_Indice = " & Agro_SQL_SaveNum(ID_Indice) & " ")
                    End If

                    StrSQL.AppendLine(" Order By Alert_IndicexTipologia.Ordinamento ")

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


    Public Function LeggiTipologieBloccate(ByVal piva As String,
                                           ByVal id_Indice As Integer,
                                           ByVal FiltroAggiuntivo As String,
                                           ByRef objParametri As AgronicaCoreParametri
                                           ) As DataTable

        Const nomeRoutine = "AgronicaCoreScadenziario_DAL.Alert_Indice_R.LeggiTipologieBloccate()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable
        Dim pivaSuperUser = objParametri.PivaSuperUser

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT Distinct Alert_Indice.Id_Tipologia ")
            StrSQL.Append(" FROM  Alert_Indice, Alert_EntitaxIndici ")
            StrSQL.Append(" WHERE Alert_Indice.Id_Indice = Alert_EntitaxIndici.Id_Indice")
            StrSQL.Append(" AND Alert_EntitaxIndici.Id_Indice = " & Agro_SQL_SaveNum(id_Indice) & " ")

            If pivaSuperUser <> "" Then
                StrSQL.Append(" And Alert_EntitaxIndici.PivaSuperUser = '" & Agro_SQL_SaveText(pivaSuperUser) & "' ")
            End If

            If piva <> "" Then
                StrSQL.Append(" And Alert_EntitaxIndici.Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            If FiltroAggiuntivo <> "" Then
                StrSQL.Append(" And (" & FiltroAggiuntivo & ") ")
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


    Public Function LeggiEntitaxIndici(ByVal Piva As String,
                                       ByVal ID_Alert_Entita As Integer,
                                       ByVal FiltroAggiuntivo As String,
                                       ByRef objParametri As AgronicaCoreParametri
                                       ) As DataTable

        Const nomeRoutine = "AgronicaCoreScadenziario_DAL.Alert_Indice_R.LeggiEntitaxIndici()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable
        Dim pivaSuperUser = objParametri.PivaSuperUser

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT Distinct Alert_EntitaxIndici.*, Alert_Indice.TitoloIndice, Alert_Indice.TipoCampo, Alert_Indice.TipoDato,  Alert_Indice.Elenco_Tipo, Alert_Indice.Elenco_Cod, Isnull(Alert_Indice_Dettagli.Valore, '') as Valore, Ordinamento, ALERT_IndicexTipologia.Id_Tipologia ")
            StrSQL.Append(" FROM  ALERT_IndicexTipologia, Alert_Indice, Alert_EntitaxIndici Left Outer Join Alert_Indice_Dettagli On (Alert_EntitaxIndici.ID_Indice_Det = Alert_Indice_Dettagli.ID_Indice_Det) ")
            StrSQL.Append(" WHERE Alert_Indice.Id_Indice = Alert_EntitaxIndici.Id_Indice")
            StrSQL.Append(" And   ALERT_IndicexTipologia.Id_Indice = Alert_EntitaxIndici.Id_Indice")

            If pivaSuperUser <> "" Then
                StrSQL.Append(" And Alert_EntitaxIndici.PivaSuperUser = '" & Agro_SQL_SaveText(pivaSuperUser) & "' ")
            End If

            'If Piva <> "" Then
            '    StrSQL.Append(" And Alert_EntitaxIndici.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            'End If

            If ID_Alert_Entita <> 0 Then
                StrSQL.Append(" AND Alert_EntitaxIndici.ID_Alert_Entita = " & Agro_SQL_SaveNum(ID_Alert_Entita) & " ")
            End If

            If FiltroAggiuntivo <> "" Then
                StrSQL.Append(" And (" & Agro_SQL_Save_xFiltroAggiuntivo(FiltroAggiuntivo,, objParametri) & ") ")
            End If

            StrSQL.Append(" Order by Ordinamento ")

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


    Public Function LeggiDDLIndice(ByVal Piva As String,
                                   ByVal ID_Indice As Integer,
                                   ByVal TipoCampo As Integer,
                                   ByVal Elenco_Tipo As Integer,
                                   ByVal Elenco_Cod As Integer,
                                   ByVal Data_Riferimento As DateTime,
                                   ByRef objParametri As AgronicaCoreParametri,
                                   Optional Elenco_Cod_String As String = ""
                                   ) As DataTable

        Const nomeRoutine = "AgronicaCoreScadenziario_DAL.Alert_Indice_R.LeggiDDLIndice()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As New DataTable
        Dim pivaSuperUser = objParametri.PivaSuperUser

        Try

            StrSQL.Length = 0

            Select Case TipoCampo

                Case 0 'Scelta Libera

                Case 1 'Valori

                    StrSQL.Append(" SELECT ID_Indice_Det as Valore_Cod, Valore as Valore_Des ")
                    StrSQL.Append(" FROM  Alert_Indice_Dettagli ")
                    'StrSQL.Append(" Inner Join Alert_Indice_Dettagli On ( Alert_Indice_Dettagli.Piva = Alert_Indice.Piva And Alert_Indice_Dettagli.Id_Indice = Alert_Indice.Id_Indice ) ")
                    StrSQL.Append(" WHERE Alert_Indice_Dettagli.ID_Indice = " & Agro_SQL_SaveNum(ID_Indice) & " ")
                    StrSQL.Append(" And Alert_Indice_Dettagli.PivaSuperUser = '" & Agro_SQL_SaveText(pivaSuperUser) & "' ")
                    'Controllo validità
                    StrSQL.Append(" And Alert_Indice_Dettagli.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Riferimento) & " ")
                    StrSQL.Append(" And Alert_Indice_Dettagli.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Riferimento) & " ")
                    StrSQL.Append(" Order by Valore_Des ")

                Case 2 'Elenco

                    Select Case Elenco_Tipo

                        Case Enum_ElencoTipoEntita.Impresa 'Imprese Gias

                            StrSQL.Append(" SELECT Distinct Imprese.Piva as Valore_Cod, Imprese.Rag_Soc as Valore_Des ")

                            StrSQL.Append(" FROM Imprese INNER JOIN UtentiXImprese On (Imprese.Piva = UtentixImprese.Piva) " & vbCrLf)
                            StrSQL.Append(" Where   UtentixImprese.[User] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' " & vbCrLf)

                            'Controllo validità
                            StrSQL.Append(" And Imprese.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Riferimento) & " ")
                            StrSQL.Append(" And Imprese.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Riferimento) & " ")

                            '----------------------------------------------------------------
                            '--- Filtro associato all'utente 
                            '----------------------------------------------------------------
                            Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                            Dim UtenteProfiloImpreseSql As String = ""
                            Dim UtenteProfiloCentriSql As String = ""
                            Dim DtImpreseVisibili As DataTable
                            Dim i As Integer

                            DtImpreseVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Impresa, "", "", objParametri)
                            If DtImpreseVisibili IsNot Nothing Then
                                For i = 0 To DtImpreseVisibili.Rows.Count - 1
                                    UtenteProfiloImpreseSql &= "'" & DtImpreseVisibili.Rows(i).Item("Piva") & "',"
                                Next
                                If UtenteProfiloImpreseSql <> "" Then
                                    UtenteProfiloImpreseSql = " AND Imprese.piva IN (" & Agro_SQL_Save_Clausola_IN(Left(UtenteProfiloImpreseSql, UtenteProfiloImpreseSql.Length - 1), True) & ") "
                                End If
                            End If

                            StrSQL.Append(UtenteProfiloImpreseSql & vbCrLf)

                            StrSQL.Append(" Order by Valore_Des ")


                        Case Enum_ElencoTipoEntita.ContattoGenerico 'Contatti Gias

                            StrSQL.Append(" SELECT Distinct Contatti.Cod_Contatto as Valore_Cod, (Contatti.Cognome + ' ' + Contatti.Nome + Contatti.Rag_Soc ) as Valore_Des ")
                            StrSQL.Append(" FROM  Contatti ")
                            StrSQL.Append(" Where (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' Or Sa_Cod = -1) ")
                            StrSQL.Append(" And Contatti.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Riferimento) & " ")
                            StrSQL.Append(" And Contatti.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Riferimento) & " ")
                            StrSQL.Append(" Order by Valore_Des ")

                        Case Enum_ElencoTipoEntita.ContattoSpecifico 'Rapporti Contabili

                            StrSQL.Append(" SELECT Distinct Contatti.Cod_Contatto as Valore_Cod, (Contatti.Cognome + ' ' + Contatti.Nome + Contatti.Rag_Soc ) as Valore_Des ")
                            StrSQL.Append(" FROM  Contatti, Risorse_Umane ")
                            StrSQL.Append(" Where (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' Or Contatti.Sa_Cod = -1) ")
                            StrSQL.Append(" And Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto ")
                            StrSQL.Append(" And Risorse_Umane.Cod_Rapporto = " & Elenco_Cod & " ")
                            StrSQL.Append(" And Risorse_Umane.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Riferimento) & " ")
                            StrSQL.Append(" And Risorse_Umane.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Riferimento) & " ")
                            StrSQL.Append(" Order by Valore_Des ")


                        Case Enum_ElencoTipoEntita.SpecieVegetale 'Specie Vegetale

                            StrSQL.Append(" SELECT Distinct SpecieVegetali.Veg_Cod as Valore_Cod, SpecieVegetali.Veg_Des as Valore_Des ")
                            StrSQL.Append(" FROM  SpecieVegetali ")
                            StrSQL.Append(" Order by Valore_Des ")


                        Case Enum_ElencoTipoEntita.MacchinaGenerica, Enum_ElencoTipoEntita.MacchinaSpecifica 'Macchine

                            StrSQL.Append(" SELECT Distinct Parco_Macchine.Mac_Cod as Valore_Cod, ")
                            StrSQL.Append("  Class_Desc COLLATE DATABASE_DEFAULT ")
                            StrSQL.Append("  + CASE WHEN ISNULL(Ditte.Ditta_Des, '') <> '' THEN ' - ' + Ditte.Ditta_Des COLLATE DATABASE_DEFAULT ELSE '' END")
                            StrSQL.Append("  + CASE WHEN ISNULL(Modello, '') <> '' THEN ' - ' + Modello COLLATE DATABASE_DEFAULT ELSE '' END")
                            StrSQL.Append("  + CASE WHEN ISNULL(Parco_Macchine.Mac_Des, '') <> '' THEN ' - ' + Parco_Macchine.Mac_Des COLLATE DATABASE_DEFAULT ELSE '' END AS Valore_Des")
                            StrSQL.Append(" FROM  Parco_Macchine ")
                            StrSQL.Append(" INNER JOIN Macchine ON Macchine.class_code = Parco_Macchine.class_code ")
                            StrSQL.Append(" LEFT JOIN Ditte ON Ditte.Ditta_Cod = Parco_Macchine.Ditta_Cod ")
                            StrSQL.Append(" Where (Parco_Macchine.Piva = '" & Agro_SQL_SaveText(Piva) & "' Or Parco_Macchine.Sa_Cod = -1) ")
                            If Not String.IsNullOrEmpty(Elenco_Cod_String) Then
                                StrSQL.Append($" And (Parco_Macchine.Class_Code = '{Agro_SQL_SaveText(Elenco_Cod_String)}' OR Parco_Macchine.Class_Code LIKE '{Agro_SQL_SaveText(Elenco_Cod_String & "%")}') ")
                            End If
                            StrSQL.Append(" And Parco_Macchine.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Riferimento) & " ")
                            StrSQL.Append(" And Parco_Macchine.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Riferimento) & " ")
                            StrSQL.Append(" Order by Valore_Des ")

                        Case Enum_ElencoTipoEntita.CentroAziendale 'Centri Aziendali

                            StrSQL.Append(" SELECT Distinct (CONVERT(varchar(100),Centri_Aziendali.Piva) + '*' + CONVERT(varchar(100),Centri_Aziendali.Sa_Cod)) as Valore_Cod, Trim(left(Centri_Aziendali.Sa_Nome,100)) as Valore_Des ")
                            StrSQL.Append(" FROM  Centri_Aziendali ")
                            StrSQL.Append(" Where Centri_Aziendali.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

                            'Controllo validità
                            StrSQL.Append(" And Centri_Aziendali.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Riferimento) & " ")
                            StrSQL.Append(" And Centri_Aziendali.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Riferimento) & " ")

                            StrSQL.Append(" Order by Valore_Des ")


                        Case Enum_ElencoTipoEntita.Campo 'Campi

                            StrSQL.Append(" Select Distinct (CONVERT(varchar(100),Campi.Piva) + '*' + CONVERT(varchar(100),Campi.Sa_Cod) + '*' + CONVERT(varchar(100),Campi.Campo_Cod)) as Valore_Cod, (trim(Left(Centri_Aziendali.Sa_Nome, 100)) + ' ' + Campi.Campo_Des) as Valore_Des ")
                            StrSQL.Append(" FROM  Campi, Centri_Aziendali ")
                            StrSQL.Append(" Where Campi.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                            StrSQL.Append(" And   Campi.Piva = Centri_Aziendali.Piva ")
                            StrSQL.Append(" And   Campi.Sa_Cod = Centri_Aziendali.Sa_Cod ")

                            'Controllo validità
                            StrSQL.Append(" And Campi.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Riferimento) & " ")
                            StrSQL.Append(" And Campi.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Riferimento) & " ")

                            StrSQL.Append(" Order by Valore_Des ")


                        Case Enum_ElencoTipoEntita.Appezzamento ' Appezzamenti

                            ''Nota: metto 0 nel campo perché mi risulta esserci un tool per spostare gli appezzamenti in un altro campo
                            'StrSQL.Append(" SELECT Distinct (CONVERT(varchar(100),Appezzamento.Piva) + '*' + CONVERT(varchar(100),Appezzamento.Sa_Cod) + '*0*' + CONVERT(varchar(100),Appezzamento.Appezza)) as Valore_Cod, (Trim(left(Centri_Aziendali.Sa_Nome,100)) + ' ' + Appezzamento.App_Nome) as Valore_Des ")
                            'StrSQL.Append(" FROM  Appezzamento, Centri_Aziendali Left Outer Join Campi On (Centri_Aziendali.Piva = Campi.Piva And Centri_Aziendali.Sa_Cod = Campi.Sa_Cod) ")
                            'StrSQL.Append(" Where Appezzamento.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                            'StrSQL.Append(" And   Appezzamento.Piva = Centri_Aziendali.Piva ")
                            'StrSQL.Append(" And   Appezzamento.Sa_Cod = Centri_Aziendali.Sa_Cod ")
                            ''Anna 02/05/2022 Commentati perché escludevano gli appezzamenti non associati ad un campo 
                            ''StrSQL.Append(" And   Campi.Piva = Centri_Aziendali.Piva ")
                            ''StrSQL.Append(" And   Campi.Sa_Cod = Centri_Aziendali.Sa_Cod ")

                            ''Controllo validità
                            'StrSQL.Append(" And Appezzamento.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Riferimento) & " ")
                            'StrSQL.Append(" And Appezzamento.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Riferimento) & " ")

                            'StrSQL.Append(" Order by Valore_Des ")

                            'Anna 05/05/22 Riscritta e aggiunta grower number
                            StrSQL.Append(" SELECT DISTINCT " & vbCrLf)
                            StrSQL.Append(" (CONVERT(VARCHAR(100),a.Piva) + '*' + CONVERT(VARCHAR(100),a.Sa_Cod) + '*0*' + CONVERT(VARCHAR(100),a.Appezza)) AS Valore_Cod, " & vbCrLf)
                            StrSQL.Append(" (TRIM(LEFT(ca.Sa_Nome,100)) + ' ' + a.App_Nome + CASE WHEN val_cod <> '' THEN ' [GROWER NR: ' + val_cod + ']' ELSE '' END ) AS Valore_Des " & vbCrLf)

                            StrSQL.Append(" FROM Appezzamento a " & vbCrLf)
                            StrSQL.Append(" INNER JOIN Centri_Aziendali ca ON ca.PIVA = a.piva AND a.SA_COD = ca.sa_cod " & vbCrLf)
                            StrSQL.Append(" LEFT JOIN Campi c ON ca.Piva = c.Piva And ca.Sa_Cod = c.Sa_Cod " & vbCrLf)
                            StrSQL.Append(" LEFT JOIN Reg_Impianti_Codici rc ON rc.piva = a.piva AND rc.SA_COD = a.SA_COD AND a.APPEZZA = rc.APPEZZA AND id_cod = 1317 AND rc.data_creazione BETWEEN a.Validita_Inizio AND a.Validita_Fine " & vbCrLf)
                            StrSQL.Append("                                 AND id_cod = " & enum_CodiciAnagrafe.Zepri_Grower_Number & " " & vbCrLf)
                            StrSQL.Append("                                 AND rc.validita_fine >= " & Agro_SQL_SaveDate(Data_Riferimento) & " " & vbCrLf)
                            StrSQL.Append(" WHERE a.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

                            StrSQL.Append(" AND a.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Riferimento) & " " & vbCrLf)
                            StrSQL.Append(" AND a.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Riferimento) & " " & vbCrLf)

                            StrSQL.Append(" ORDER BY Valore_Des " & vbCrLf)
                            StrSQL.Append(" ")

                    End Select

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


    'Caricamento Griglia grid_tipologiexindice per IndiciXTipologie_UC
    Public Function LeggiIndicixTipologiaxGriglia(ByVal Piva As String,
                                                  ByVal ID_Area As Integer,
                                                  ByVal ID_Tipologia As Integer,
                                                  ByRef objParametri As AgronicaCoreParametri
                                                  ) As DataTable

        Const nomeRoutine = "AgronicaCoreScadenziario_DAL.Alert_Indice_R.LeggiIndicixTipologiaxGriglia()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable
        Dim pivaSuperUser = objParametri.PivaSuperUser

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT  Alert_IndicexTipologia.ID_Indice , Alert_Indice.TitoloIndice , Alert_IndicexTipologia.Ordinamento ,")
            StrSQL.Append(" Alert_IndicexTipologia.ID_Area,  Alert_IndicexTipologia.ID_Tipologia ,")
            StrSQL.Append(" Alert_Tipologia.Nome AS NomeTipologia, Alert_Area.Nome AS NomeArea ,")
            StrSQL.Append(" (CASE When Alert_IndicexTipologia.ChkObbligatorio_Tipologia=1 THEN 'Sì' When Alert_IndicexTipologia.ChkObbligatorio_Tipologia=0 THEN 'No' End) AS Obbligatorio")
            StrSQL.Append(" FROM  Alert_Indice , Alert_IndicexTipologia , Alert_Tipologia , Alert_Area ")
            StrSQL.Append(" WHERE Alert_Indice.PivaSuperUser =  Alert_IndicexTipologia.PivaSuperUser ")
            StrSQL.Append(" AND   Alert_Indice.PivaSuperUser = '" & Agro_SQL_SaveText(pivaSuperUser) & "' ")
            StrSQL.Append(" AND   Alert_Indice.ID_Indice =  Alert_IndicexTipologia.ID_Indice ")
            StrSQL.Append(" AND   Alert_IndicexTipologia.ID_Tipologia =  Alert_Tipologia.ID_Tipologia ")
            StrSQL.Append(" AND   Alert_IndicexTipologia.ID_Area =  Alert_Area.ID_Area ")

            'If Piva <> "" Then
            '    StrSQL.Append(" And Alert_Indice.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            'End If

            If ID_Area <> 0 Then
                StrSQL.Append(" AND Alert_IndicexTipologia.ID_Area = " & Agro_SQL_SaveNum(ID_Area) & " ")
            End If

            If ID_Tipologia <> 0 Then
                StrSQL.Append(" AND Alert_IndicexTipologia.ID_Tipologia = " & Agro_SQL_SaveNum(ID_Tipologia) & " ")
            End If

            'StrSQL.Append(" Group By Alert_Tipologia.ID_Tipologia , Alert_Area.ID_Area ,Alert_Indice.ID_Indice")
            StrSQL.Append(" Order By Alert_IndicexTipologia.Ordinamento , Alert_Area.Nome ")
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


    Public Function LeggiValoreIndice(ByVal Piva As String,
                                      ByVal Id_Agenda As Integer,
                                      ByVal ID_Tipologia As Integer,
                                      ByVal FiltroAggiuntivo As String,
                                      ByRef objParametri As AgronicaCoreParametri
                                      ) As DataTable

        Const nomeRoutine = "AgronicaCoreScadenziario_DAL.Alert_Indice_R.LeggiValoreIndice()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT  dbo.Alert_Entita.ID_Alert_Entita, dbo.Alert_EntitaxIndici.Valore_Des ")
            StrSQL.Append(" FROM    dbo.Alert_Entita INNER JOIN ")
            StrSQL.Append(" dbo.Alert_Elenco ON dbo.Alert_Entita.PivaSuperUser = dbo.Alert_Elenco.PivaSuperUser AND dbo.Alert_Entita.ID_Alert_Entita = dbo.Alert_Elenco.ID_Alert_Entita INNER JOIN ")
            StrSQL.Append(" dbo.Alert_EntitaxIndici ON dbo.Alert_Entita.PivaSuperUser = dbo.Alert_EntitaxIndici.PivaSuperUser AND dbo.Alert_Entita.ID_Alert_Entita = dbo.Alert_EntitaxIndici.ID_Alert_Entita ")
            StrSQL.Append(" Where 1 = 1 ")

            If Piva <> 0 Then
                StrSQL.Append(" AND dbo.Alert_Entita.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Id_Agenda <> 0 Then
                StrSQL.Append(" AND dbo.Alert_Entita.ID_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")
            End If

            If ID_Tipologia <> 0 Then
                StrSQL.Append(" AND dbo.Alert_Elenco.ID_Tipologia = '" & Agro_SQL_SaveText(ID_Tipologia) & "' ")
            End If

            If FiltroAggiuntivo <> "" Then
                StrSQL.Append(" And (" & FiltroAggiuntivo & ") ")
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

End Class



Public Class Alert_Indice_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal piva As String,
                           ByVal id_indice As Integer,
                           ByVal titoloindice As String,
                           ByVal chkobbligatorio As Integer,
                           ByVal chkindice_speciale As Integer,
                           ByVal tipocampo As Integer,
                           ByVal tipodato As String,
                           ByVal elenco_tipo As Integer,
                           ByVal elenco_cod As Integer,
                           ByVal elenco_val As String,
                           ByVal validita_inizio As Date,
                           ByVal validita_fine As Date,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional elenco_cod_string As String = ""
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreScadenziario_DAL.Alert_Indice_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False
        Dim pivaSuperUser = objParametri.PivaSuperUser

        Try

            piva = "" 'Non gestita

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO Alert_Indice ")
            StrSQL.Append("  (PivaSuperUser, Piva, Id_Indice, TitoloIndice, ChkObbligatorio, ChkIndice_Speciale, TipoCampo, TipoDato, Elenco_Tipo, Elenco_Cod, Elenco_Val, ")
            StrSQL.Append("   Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, Inviato, DataInvio, validita_inizio, validita_fine, elenco_cod_string   ")


            StrSQL.Append(" ) ")

            StrSQL.Append("VALUES (")

            StrSQL.Append("          '" & Agro_SQL_SaveText(pivaSuperUser) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(piva) & "' ")
            StrSQL.Append("         ," & Agro_vb_SaveNum(id_indice) & " ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(titoloindice) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(chkobbligatorio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(chkindice_speciale) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(tipocampo) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(tipodato) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(elenco_tipo) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(elenco_cod) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(elenco_val) & "' ")
            StrSQL.Append("		    , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("		    , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("		    ,'" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "' ")
            StrSQL.Append("		    ,'" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "' ")
            StrSQL.Append("         , 0 ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("         ," & Agro_SQL_SaveDate(validita_inizio) & " ")
            StrSQL.Append("         ," & Agro_SQL_SaveDate(validita_fine) & " ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(elenco_cod_string) & "' ")

            StrSQL.Append(" )")

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

    Public Function Modifica(ByVal piva As String,
                             ByVal id_indice As Integer,
                             ByVal titoloindice As String,
                             ByVal chkobbligatorio As Integer,
                             ByVal chkindice_speciale As Integer,
                             ByVal tipocampo As Integer,
                             ByVal tipodato As String,
                             ByVal elenco_tipo As Integer,
                             ByVal elenco_cod As Integer,
                             ByVal elenco_val As String,
                             ByVal validita_inizio As Date,
                             ByVal validita_fine As Date,
                             ByRef objParametri As AgronicaCoreParametri,
                             Optional elenco_cod_string As String = ""
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreScadenziario_DAL.Alert_Indice_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False
        Dim pivaSuperUser = objParametri.PivaSuperUser

        Try

            StrSQL.Length = 0

            StrSQL.Append(" UPDATE Alert_Indice SET ")
            StrSQL.Append("  piva = '" & Agro_SQL_SaveText(piva) & "' ")
            StrSQL.Append(" ,titoloindice = '" & Agro_SQL_SaveText(titoloindice) & "' ")
            StrSQL.Append(" ,tipocampo = " & Agro_vb_SaveNum(tipocampo) & " ")
            StrSQL.Append(" ,tipodato = '" & Agro_SQL_SaveText(tipodato) & "' ")
            StrSQL.Append(" ,chkobbligatorio = " & Agro_vb_SaveNum(chkobbligatorio) & " ")
            StrSQL.Append(" ,chkindice_speciale = " & Agro_vb_SaveNum(chkindice_speciale) & " ")
            StrSQL.Append(" ,elenco_tipo = " & Agro_vb_SaveNum(elenco_tipo) & " ")
            StrSQL.Append(" ,elenco_cod = " & Agro_vb_SaveNum(elenco_cod) & " ")
            StrSQL.Append(" ,elenco_val = '" & Agro_SQL_SaveText(elenco_val) & "' ")
            StrSQL.Append(" ,Data_Modifica = " & Agro_SQL_SaveDate(Date.Now) & " ")
            StrSQL.Append(" ,Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "' ")
            StrSQL.Append(" ,validita_inizio = " & Agro_SQL_SaveDate(validita_inizio) & " ")
            StrSQL.Append(" ,validita_fine = " & Agro_SQL_SaveDate(validita_fine) & " ")
            StrSQL.Append(" ,elenco_cod_string = '" & Agro_SQL_SaveText(elenco_cod_string) & "' ")

            StrSQL.Append(" WHERE id_indice = " & Agro_SQL_SaveNum(id_indice) & " ")

            If pivaSuperUser <> "" Then
                StrSQL.AppendLine(" AND PivaSuperUser = '" & Agro_SQL_SaveText(pivaSuperUser) & "'")
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


    Public Function ModificaChiavi(ByVal piva As String,
                                   ByVal id_indice_new As Integer,
                                   ByVal id_indice_old As Integer,
                                   ByRef objParametri As AgronicaCoreParametri
                                   ) As Boolean

        Const nomeRoutine = "AgronicaCoreScadenziario_DAL.Alert_Indice_W.ModificaChiavi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False
        Dim pivaSuperUser = objParametri.PivaSuperUser

        Try

            'Alert_Indice
            StrSQL.Length = 0

            StrSQL.Append(" UPDATE Alert_Indice SET ")
            StrSQL.Append(" ID_indice = " & Agro_vb_SaveNum(id_indice_new) & " ")

            StrSQL.Append(" WHERE ID_indice = " & Agro_SQL_SaveNum(id_indice_old) & " ")

            If pivaSuperUser <> "" Then
                StrSQL.AppendLine(" AND PivaSuperUser = '" & Agro_SQL_SaveText(pivaSuperUser) & "'")
            End If

            If piva <> "" Then
                StrSQL.AppendLine(" AND piva = '" & Agro_SQL_SaveText_NULL(piva) & "'")
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            'Alert_Indice_Dettagli
            StrSQL.Length = 0

            StrSQL.Append(" UPDATE Alert_Indice_Dettagli SET ")
            StrSQL.Append(" ID_indice = " & Agro_vb_SaveNum(id_indice_new) & " ")

            StrSQL.Append(" WHERE ID_indice = " & Agro_SQL_SaveNum(id_indice_old) & " ")

            If pivaSuperUser <> "" Then
                StrSQL.AppendLine(" AND PivaSuperUser = '" & Agro_SQL_SaveText(pivaSuperUser) & "'")
            End If

            If piva <> "" Then
                StrSQL.AppendLine(" AND piva = '" & Agro_SQL_SaveText_NULL(piva) & "'")
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            'Alert_EntitaxIndici
            StrSQL.Length = 0


            StrSQL.Append(" UPDATE Alert_EntitaxIndici SET ")
            StrSQL.Append(" ID_indice = " & Agro_vb_SaveNum(id_indice_new) & " ")

            StrSQL.Append(" WHERE ID_indice = " & Agro_SQL_SaveNum(id_indice_old) & " ")

            If pivaSuperUser <> "" Then
                StrSQL.AppendLine(" AND PivaSuperUser = '" & Agro_SQL_SaveText(pivaSuperUser) & "'")
            End If

            'If piva <> "" Then
            '    StrSQL.AppendLine(" AND piva = '" & Agro_SQL_SaveText_NULL(piva) & "'")
            'End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------


            'Alert_IndicexTipologia
            StrSQL.Length = 0


            StrSQL.Append(" UPDATE Alert_IndicexTipologia SET ")
            StrSQL.Append(" ID_indice = " & Agro_vb_SaveNum(id_indice_new) & " ")

            StrSQL.Append(" WHERE ID_indice = " & Agro_SQL_SaveNum(id_indice_old) & " ")

            If pivaSuperUser <> "" Then
                StrSQL.AppendLine(" AND PivaSuperUser = '" & Agro_SQL_SaveText(pivaSuperUser) & "'")
            End If

            'If piva <> "" Then
            '    StrSQL.AppendLine(" AND piva = '" & Agro_SQL_SaveText_NULL(piva) & "'")
            'End If

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


    Public Function ModificaChkObbligatorio(ByVal piva As String,
                                            ByVal id_indice As Integer,
                                            ByRef objParametri As AgronicaCoreParametri
                                            ) As Boolean

        Const nomeRoutine = "AgronicaCoreScadenziario_DAL.Alert_Indice_W.ModificaChkObbligatorio()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False
        Dim pivaSuperUser = objParametri.PivaSuperUser

        Try

            StrSQL.Length = 0

            StrSQL.Append(" UPDATE Alert_IndicexTipologia SET ")
            StrSQL.Append(" ChkObbligatorio_Tipologia = 1 ")

            StrSQL.Append(" WHERE ID_indice = " & Agro_SQL_SaveNum(id_indice) & " ")

            If pivaSuperUser <> "" Then
                StrSQL.AppendLine(" AND PivaSuperUser = '" & Agro_SQL_SaveText(pivaSuperUser) & "'")
            End If

            'If piva <> "" Then
            '    StrSQL.AppendLine(" AND piva = '" & Agro_SQL_SaveText_NULL(piva) & "'")
            'End If

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


    Public Function ModificaID_Indice_Det(ByVal id_Alert_Enttita As Integer,
                                          ByVal id_indice As Integer,
                                          ByVal id_Indice_Det As Integer,
                                          ByRef objParametri As AgronicaCoreParametri
                                          ) As Boolean

        Const nomeRoutine = "AgronicaCoreScadenziario_DAL.Alert_Indice_W.ModificaID_Indice_Det()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False
        Dim pivaSuperUser = objParametri.PivaSuperUser

        Try

            'Alert_EntitaxIndici
            StrSQL.Length = 0

            StrSQL.Append(" UPDATE Alert_EntitaxIndici SET ")
            StrSQL.Append(" Id_Indice_Det = " & Agro_vb_SaveNum(id_Indice_Det) & " ")
            StrSQL.Append(" WHERE ID_Alert_Entita = " & Agro_SQL_SaveNum(id_Alert_Enttita) & " ")

            If pivaSuperUser <> "" Then
                StrSQL.AppendLine(" AND PivaSuperUser = '" & Agro_SQL_SaveText(pivaSuperUser) & "'")
            End If

            If id_indice <> 0 Then
                StrSQL.AppendLine(" AND Id_indice = " & Agro_SQL_SaveNum_NULL(id_indice))
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


    Public Function Cancella(ByVal piva As String,
                             ByVal id_indice As Integer,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreScadenziario_DAL.Alert_Indice_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False
        Dim pivaSuperUser = objParametri.PivaSuperUser

        Try

            StrSQL.Length = 0
            StrSQL.Append(" DELETE ")
            StrSQL.Append(" FROM Alert_Indice ")
            StrSQL.Append(" WHERE id_indice = " & Agro_SQL_SaveNum(id_indice) & " ")
            StrSQL.Append(" AND PivaSuperUser = '" & Agro_SQL_SaveText(pivaSuperUser) & "' ")

            'If Trim(piva) <> "" Then
            '    StrSQL.Append(" AND piva = '" & Agro_SQL_SaveText(piva) & "' ")
            'End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------


            StrSQL.Length = 0
            StrSQL.Append(" DELETE ")
            StrSQL.Append(" FROM Alert_IndicexTipologia ")
            StrSQL.Append(" WHERE id_indice = " & Agro_SQL_SaveNum(id_indice) & " ")
            StrSQL.Append(" AND PivaSuperUser = '" & Agro_SQL_SaveText(pivaSuperUser) & "' ")

            'If Trim(piva) <> "" Then
            '    StrSQL.Append(" AND piva = '" & Agro_SQL_SaveText(piva) & "' ")
            'End If

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


    Public Function ScriviEntitaxIndice(ByVal piva As String,
                                        ByVal id_alert_entita As Integer,
                                        ByVal id_indice As Integer,
                                        ByVal id_indice_det As Integer,
                                        ByVal elenco_val As String,
                                        ByVal valore_des As String,
                                        ByVal validita_inizio As Date,
                                        ByVal validita_fine As Date,
                                        ByRef objParametri As AgronicaCoreParametri
                                        ) As Boolean

        Const nomeRoutine = "AgronicaCoreScadenziario_DAL.Alert_Indice_W.ScriviEntitaxIndice()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False
        Dim pivaSuperUser = objParametri.PivaSuperUser

        Try

            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO Alert_EntitaxIndici ")
            StrSQL.Append("                   ( PivaSuperUser, Piva, ID_Alert_Entita, ID_Indice, ID_Indice_Det, Elenco_Val, Valore_Des, Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, Inviato, DataInvio, Validita_Inizio, Validita_Fine   ")


            StrSQL.Append(" ) ")

            StrSQL.Append("VALUES (")

            StrSQL.Append("          '" & Agro_SQL_SaveText(pivaSuperUser) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(piva) & "' ")
            StrSQL.Append("         ," & Agro_vb_SaveNum(id_alert_entita) & " ")
            StrSQL.Append("         ," & Agro_vb_SaveNum(id_indice) & " ")
            StrSQL.Append("         ," & Agro_vb_SaveNum(id_indice_det) & " ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(elenco_val) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(valore_des) & "' ")
            StrSQL.Append("		    , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            StrSQL.Append("		    , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            StrSQL.Append("		    ,'" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "' ")
            StrSQL.Append("		    ,'" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "' ")
            StrSQL.Append("         , 0 ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.Append("         ," & Agro_SQL_SaveDate(validita_inizio) & " ")
            StrSQL.Append("         ," & Agro_SQL_SaveDate(validita_fine) & " ")

            StrSQL.Append(" )")

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


    Public Function CancellaEntitaxIndice(ByVal piva As String,
                                          ByVal id_alert_entita As Integer,
                                          ByRef objParametri As AgronicaCoreParametri
                                          ) As Boolean

        Const nomeRoutine = "AgronicaCoreScadenziario_DAL.Alert_Indice_W.CancellaEntitaxIndice()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False
        Dim pivaSuperUser = objParametri.PivaSuperUser

        Try

            'Cancellazione Preventiva
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("Delete From Alert_EntitaxIndici ")
            StrSQL.Append(" Where Id_Alert_Entita = " & id_alert_entita & " ")

            If Trim(pivaSuperUser) <> "" Then
                StrSQL.Append(" AND PivaSuperUser = '" & Agro_SQL_SaveText(pivaSuperUser) & "' ")
            End If

            'If Trim(piva) <> "" Then
            '    StrSQL.Append(" AND piva = '" & Agro_SQL_SaveText(piva) & "' ")
            'End If

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


    'Scrittura su Alert_IndicexTipologie
    Public Function Scrivi_Alert_IndicexTipologie(ByVal Id_Indice As Integer,
                                                  ByVal ChkObbligatorio_Tipologia As Integer,
                                                  ByVal ID_Tipologia As Integer,
                                                  ByVal ID_Area As Integer,
                                                  ByRef objParametri As AgronicaCoreParametri
                                                  ) As Boolean

        Const nomeRoutine = "AgronicaCoreScadenziario_DAL.Alert_Indice_W.Scrivi_Alert_IndicexTipologie()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False
        Dim pivaSuperUser = objParametri.PivaSuperUser

        Try
            'Scrivo il campo piva come Pivasuperuser perché non è per azienda l'associazione IndicexTipologia,
            'ma è per singola installazione.
            '---------------------------------------------

            StrSQL.Length = 0
            StrSQL.AppendLine("INSERT INTO Alert_IndicexTipologia ")
            StrSQL.AppendLine("  (PivaSuperUser, Piva, ID_Indice, ID_Area, ID_Tipologia, ChkObbligatorio_Tipologia, Ordinamento , Data_Creazione , Data_Modifica ,Username_Creazione , Username_Modifica , Validita_Inizio , Validita_Fine")

            StrSQL.AppendLine(" ) ")

            StrSQL.AppendLine("VALUES (")

            StrSQL.AppendLine(" '" & Agro_SQL_SaveText(pivaSuperUser) & "' ")
            StrSQL.AppendLine(" ,'" & Agro_SQL_SaveText(pivaSuperUser) & "' ")
            StrSQL.AppendLine(" ," & Agro_SQL_SaveNum(Id_Indice) & " ")
            StrSQL.AppendLine(" ," & Agro_SQL_SaveNum(ID_Area) & " ")
            StrSQL.AppendLine(" ," & Agro_SQL_SaveNum(ID_Tipologia) & " ")
            StrSQL.AppendLine(" ," & Agro_SQL_SaveNum(ChkObbligatorio_Tipologia) & " ")
            StrSQL.AppendLine(" , (SELECT COALESCE(MAX(Ordinamento) + 1 , 1) FROM Alert_IndicexTipologia AL WHERE ID_Area = " & Agro_SQL_SaveNum(ID_Area) & " AND ID_Tipologia = " & Agro_SQL_SaveNum(ID_Tipologia) & " )")
            StrSQL.AppendLine(" , " & Agro_SQL_SaveDateTime(Date.Now) & " ")
            StrSQL.AppendLine(" , " & Agro_SQL_SaveDateTime(Date.Now) & " ")
            StrSQL.AppendLine(" , '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "'")
            StrSQL.AppendLine(" , '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "'")
            StrSQL.AppendLine(" , " & Agro_SQL_SaveDate(AGRODATAINIZIO) & " ")
            StrSQL.AppendLine(" , " & Agro_SQL_SaveDate(AGRODATAFINE) & " ")
            StrSQL.AppendLine(" )")

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

    'Cancella Alert_IndicexTipologie
    Function Cancella_Alert_IndicexTipologie(ByVal piva As String,
                                             ByVal Id_Indice As Integer,
                                             ByVal ID_Tipologia As Integer,
                                             ByVal ID_Area As Integer,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreScadenziario_DAL.Alert_Indice_W.Cancella_Alert_IndicexTipologie()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False
        Dim pivaSuperUser = objParametri.PivaSuperUser

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" DELETE FROM Alert_IndicexTipologia ")
            StrSQL.Append(" WHERE PivaSuperUser = '" & Agro_SQL_SaveText(pivaSuperUser) & "' ")

            If Trim(piva) <> "" Then
                StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            If Id_Indice <> 0 Then
                StrSQL.Append(" AND Id_Indice = " & Agro_SQL_SaveNum(Id_Indice) & " ")
            End If

            If ID_Area <> 0 Then
                StrSQL.Append(" AND ID_Area = " & Agro_SQL_SaveNum(ID_Area) & " ")
            End If

            If ID_Tipologia <> 0 Then
                StrSQL.Append(" AND ID_Tipologia = " & Agro_SQL_SaveNum(ID_Tipologia) & " ")
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

    'Cambia l'ordinamento in Alert_IndicexTipologia
    Public Function Modifica_Ordine_Alert_IndicexTipologia(ByVal piva As String,
                                                           ByRef objParametri As AgronicaCoreParametri,
                                                           ByVal righeAggiornateArray As JArray
                                                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreScadenziario_DAL.Alert_Indice_W.Modifica_Ordine_Alert_IndicexTipologie()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False
        Dim pivaSuperUser = objParametri.PivaSuperUser

        Try

            'apro una transazione
            ConnessioniTransazioni.ApriConnessione(True, objParametri)

            'INIZIO FOR CON UPDATE PER OGNI RIGA
            For Each obj As JObject In righeAggiornateArray
                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Alert_IndicexTipologia SET ")

                StrSQL.AppendLine(" ChkObbligatorio_Tipologia = " & Agro_SQL_SaveNum(obj("ChkObbligatorio_Tipologia")) & " ,")
                StrSQL.AppendLine(" Ordinamento = " & Agro_SQL_SaveNum(CInt(obj("Ordinamento").ToString)) & ", ")
                StrSQL.AppendLine(" Data_Modifica = " & Agro_SQL_SaveDateTime(Date.Now) & ", ")
                StrSQL.AppendLine(" Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "' ")
                StrSQL.Append(" WHERE PivaSuperUser = '" & Agro_SQL_SaveText(pivaSuperUser) & "'")

                If Trim(piva) <> "" Then
                    StrSQL.AppendLine(" AND piva = '" & Agro_SQL_SaveText(piva) & "'")
                End If

                StrSQL.AppendLine(" AND ID_Indice = " & Agro_SQL_SaveNum(CInt(obj("ID_Indice").ToString)) & " ")
                StrSQL.AppendLine(" AND ID_Area = " & Agro_SQL_SaveNum(CInt(obj("ID_Area").ToString)) & " ")
                StrSQL.AppendLine(" AND ID_Tipologia = " & Agro_SQL_SaveNum(CInt(obj("ID_Tipologia").ToString)) & " ")

                '--------------------------------------------------------------------------
                xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
                '--------------------------------------------------------------------------
            Next

            'Se è andato tutto bene
            ConnessioniTransazioni.ChiudiTransazione(1, objParametri)

        Catch ex As Exception
            ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        Finally
            ConnessioniTransazioni.ChiudiConnessione(objParametri)
        End Try

        Return xRisp

    End Function

    'Cambia l obbligatorietà in Alert_IndicexTipologia
    Public Function Modifica_Obbligo_Alert_IndicexTipologia(ByVal piva As String,
                                                            ByVal Id_Indice As Integer,
                                                            ByVal ID_Tipologia As Integer,
                                                            ByVal ID_Area As Integer,
                                                            ByVal Obbligatorio As Double,
                                                            ByRef objParametri As AgronicaCoreParametri
                                                            ) As Boolean

        Const nomeRoutine = "AgronicaCoreScadenziario_DAL.Alert_Indice_W.Modifica_Obbligo_Alert_IndicexTipologia()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False
        Dim pivaSuperUser = objParametri.PivaSuperUser

        Try
            StrSQL.Length = 0
            StrSQL.Append(" UPDATE Alert_IndicexTipologia SET ")

            StrSQL.AppendLine(" ChkObbligatorio_Tipologia = " & Agro_SQL_SaveNum(Obbligatorio) & ", ")
            StrSQL.AppendLine(" Data_Modifica = " & Agro_SQL_SaveDateTime(Date.Now) & ", ")
            StrSQL.AppendLine(" Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "' ")

            StrSQL.Append(" WHERE PivaSuperUser = '" & Agro_SQL_SaveText(pivaSuperUser) & "'")

            If Trim(piva) <> "" Then
                StrSQL.AppendLine(" AND piva = '" & Agro_SQL_SaveText(piva) & "'")
            End If

            StrSQL.AppendLine(" AND ID_Indice = " & Agro_SQL_SaveNum(Id_Indice) & " ")

            If Trim(ID_Area) <> "" Then
                StrSQL.AppendLine(" AND ID_Area = " & Agro_SQL_SaveNum(ID_Area) & " ")
            End If

            If Trim(ID_Tipologia) <> "" Then
                StrSQL.AppendLine(" AND ID_Tipologia = " & Agro_SQL_SaveNum(ID_Tipologia) & " ")
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
