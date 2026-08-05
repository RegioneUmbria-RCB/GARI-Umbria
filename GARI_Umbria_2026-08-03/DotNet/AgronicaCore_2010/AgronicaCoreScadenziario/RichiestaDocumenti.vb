Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework

'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################


Public Class RichiestaDocumenti_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiRichiestaDocumenti(ByRef objParametri As AgronicaCoreParametri, ByVal piva As String, ByVal Richiesta_Cod As Integer, ByRef objParametri_Utenti As AgronicaCoreParametri) As DataTable

        Const NomeRoutine = "AgronicaCoreAnagrafeDAL.SchemaDocumenti.LeggiRichiestaDocumenti()"

        Dim MessaggioErrore As String = ""
        Dim DT As DataTable

        Dim bPermessixUtente As Boolean = False 'Booleano per la presenza di record in tabella obj_CategTipologiaDocumentiXUtenti
        'In caso non esistano record viene bypassato il filtro

        Dim iLivelloGerarchia As Integer = 0 'Livello Max della tabella GerarchiaImprese (in modo da costruire un filtro dinamico)

        Dim obj_CategTipologiaDocumentiXUtenti As New AgronicaCoreScadenziario.CategTipologiaDocumentiXUtenti_R

        Try
            Dim StrSQL As New StringBuilder
            StrSQL.Length = 0


            '********************************************estrae dati in base a Piva e Richiesta_Cod ******************************

            Dim DTDati As DataTable = EstraiDati(objParametri, piva, Richiesta_Cod)

            'StrSQL.Append("SELECT   UMA_Richieste_Testata.Pratica_Cod,
            '               UMA_Richieste_Testata.Richiesta_Cod,
            '               UMA_Richieste_Testata.Avanzamento_Richiesta,
            '               UMA_Richieste_Testata.Tipo_Richiesta,
            '               Pratiche.Anno,
            '               Pratiche.Servizio_Cod,
            '               Pratiche.Pratica_Cod,
            '                        Pratiche_Stati_Attuali.Stato_Cod,
            '                        WAnagraficaStati.WAnagraficaStati_Cod,
            '                        WAnagraficaStati.WAnagraficaStati_Des
            '               FROM Pratiche
            '               JOIN Pratiche_Stati_Attuali ON Pratiche.Pratica_Cod = Pratiche_Stati_Attuali.Pratica_Cod
            '               LEFT JOIN UMA_Richieste_Testata ON UMA_Richieste_Testata.Pratica_Cod = Pratiche.Pratica_Cod
            '               LEFT JOIN WAnagraficaStati ON Pratiche_Stati_Attuali.Stato_Cod = WAnagraficaStati.WAnagraficaStati_Cod
            '               WHERE UMA_Richieste_Testata.Piva = '" & piva & "' AND UMA_Richieste_Testata.Richiesta_Cod = " & Richiesta_Cod & "
            '               ORDER BY Anno")

            'DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            Dim praticaCod As Integer = 0
            Dim avanzamento As Integer = 0
            Dim tipoRichiesta As Integer = 0
            Dim annoQuery As Integer = 0
            Dim servizioCod As Integer = 0
            Dim statoAttuale As Integer = 0
            Dim statoAttualeDes As String = ""

            If Not IsNothing(DTDati) AndAlso DTDati.Rows.Count > 0 Then

                praticaCod = If(Not IsDBNull(DTDati.Rows(0)("Pratica_Cod")), CInt(DTDati.Rows(0)("Pratica_Cod")), -1)
                avanzamento = If(Not IsDBNull(DTDati.Rows(0)("Avanzamento_Richiesta")), CInt(DTDati.Rows(0)("Avanzamento_Richiesta")), -1)
                tipoRichiesta = If(Not IsDBNull(DTDati.Rows(0)("Tipo_Richiesta")), CInt(DTDati.Rows(0)("Tipo_Richiesta")), -1)
                annoQuery = If(Not IsDBNull(DTDati.Rows(0)("anno")), CInt(DTDati.Rows(0)("anno")), 0)
                servizioCod = If(Not IsDBNull(DTDati.Rows(0)("Servizio_Cod")), CInt(DTDati.Rows(0)("Servizio_Cod")), 0)
                statoAttuale = If(Not IsDBNull(DTDati.Rows(0)("Stato_Cod")), CInt(DTDati.Rows(0)("Stato_Cod")), 0)
                statoAttualeDes = If(Not IsDBNull(DTDati.Rows(0)("WAnagraficaStati_Des")), DTDati.Rows(0)("WAnagraficaStati_Des"), "")

            End If


            '******************In base ai dati recuperati estrae tutte le Richieste_Cod dell'anno**************


            'StrSQL.Append("SELECT UMA_Richieste_Testata.Richiesta_Cod,
            '                      UMA_Richieste_Testata.Avanzamento_Richiesta,
            '                      UMA_Richieste_Testata.Tipo_Richiesta,
            '                      UMA_Richieste_Testata.Piva
            '               FROM Pratiche
            '               LEFT JOIN UMA_Richieste_Testata ON UMA_Richieste_Testata.Pratica_Cod = Pratiche.Pratica_Cod
            '               WHERE Pratiche.Anno = " & annoQuery & "
            '               AND UMA_Richieste_Testata.Piva = '" & piva & "' 
            '               AND UMA_Richieste_Testata.Avanzamento_Richiesta = " & avanzamento & " 
            '               AND UMA_Richieste_Testata.Tipo_Richiesta = " & tipoRichiesta & "
            '               ORDER BY Richiesta_Cod ")

            'DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)

            'Dim richiesteAssociate As New List(Of Integer)

            'If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

            '    For Each r As DataRow In DT.Rows
            '        richiesteAssociate.Add(If(Not IsDBNull(r("Richiesta_Cod")), CInt(r("Richiesta_Cod")), 0))
            '    Next

            'End If
            'If Not richiesteAssociate.Any Then
            '    richiesteAssociate.Add(-1)
            'End If

            'Dim fase As Integer = 0

            If (praticaCod <> -1) Then

                Dim fase As Integer = EstraiFase(objParametri, piva, avanzamento, tipoRichiesta, annoQuery, Richiesta_Cod, praticaCod, statoAttuale, objParametri_Utenti)

                '    '************stabilisce la fase in base alle Richieste Associate ************
                '    If (Richiesta_Cod = richiesteAssociate(0)) Then

                '        If (avanzamento = 0) Then

                '            If (statoAttuale = enum_WWorflow_WAnagraficaStati.Quaderno_Campagna_In_Compilazione) Then

                '                fase = enum_ID_Area_Tipologia.UMA_prima_richiesta

                '            ElseIf (statoAttuale = enum_WWorflow_WAnagraficaStati.Quaderno_Campagna_Verifica_in_corso) Then

                '                fase = enum_ID_Area_Tipologia.UMA_approvazione_prima_richiesta

                '            End If

                '        ElseIf (avanzamento = 1) Then

                '            If (statoAttuale = enum_WWorflow_WAnagraficaStati.Quaderno_Campagna_In_Compilazione) Then

                '                fase = enum_ID_Area_Tipologia.UMA_rendicontazione

                '            ElseIf (statoAttuale = enum_WWorflow_WAnagraficaStati.Quaderno_Campagna_Verifica_in_corso) Then

                '                fase = enum_ID_Area_Tipologia.UMA_approvazione_rendicontazione

                '            End If

                '        End If

                '    Else

                '        If (avanzamento = 0) Then

                '            If (statoAttuale = enum_WWorflow_WAnagraficaStati.Quaderno_Campagna_In_Compilazione) Then

                '                fase = enum_ID_Area_Tipologia.UMA_richiesta_integrativa

                '            ElseIf (statoAttuale = enum_WWorflow_WAnagraficaStati.Quaderno_Campagna_Verifica_in_corso) Then

                '                fase = enum_ID_Area_Tipologia.UMA_approvazione_richiesta_integrativa

                '            End If

                '        ElseIf (avanzamento = 1) Then

                '            If (statoAttuale = enum_WWorflow_WAnagraficaStati.Quaderno_Campagna_In_Compilazione) Then

                '                fase = enum_ID_Area_Tipologia.UMA_rendicontazione

                '            ElseIf (statoAttuale = enum_WWorflow_WAnagraficaStati.Quaderno_Campagna_Verifica_in_corso) Then

                '                fase = enum_ID_Area_Tipologia.UMA_approvazione_rendicontazione

                '            End If

                '        End If

                '    End If

                'StrSQL.Length = 0

                StrSQL.Append("select Schema_Documenti_Template.Fase,
                                      Schema_Documenti_Template.Ambito,
                                      Schema_Documenti_Template.Id_Schema_Template,
	                                  Schema_Documenti_Template.Servizio_Cod
                               from Schema_Documenti_Template
                               where schema_Documenti_Template.Fase = " & fase & " and Schema_Documenti_Template.Ambito = '" & enum_Ambiti_UMA.UMA_Carburanti & "' AND Schema_Documenti_Template.Servizio_Cod = " & servizioCod & " ")

                DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
                Dim idSchemaTemplate As New List(Of Integer)

                If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

                    For Each r As DataRow In DT.Rows
                        idSchemaTemplate.Add(If(Not IsDBNull(r("Id_Schema_Template")), CInt(r("Id_Schema_Template")), 0))
                    Next

                End If
                If Not idSchemaTemplate.Any Then
                    idSchemaTemplate.Add(-1)
                End If

                obj_CategTipologiaDocumentiXUtenti.Controllo_Se_Vengono_Gestiti_Permessi(iLivelloGerarchia, bPermessixUtente, piva, objParametri)

                StrSQL.Length = 0

                '********************************************da aggiungere in base ai risultati della query sovrastante**********************************

                StrSQL.Append("SELECT      Alert_entita_inner.Richiesta_Cod,
										   isnull(Alert_entita_inner.Nr_Documenti_Presenti, 0) as Nr_Documenti_Presenti,
                                           Alert_Tipologia.Nome as Tipologia_Des,
	                                       Schema_Documenti_Template.Id_Schema_Template,
                                           Schema_Documenti_Template.Ambito,
                                           Schema_Documenti_Template.Fase,
                                           Schema_Documenti_Template.Flag_Firmato_Digit,
                                           Schema_Documenti_Template.Flag_obbligatorio,
                                           Schema_Documenti_Template.Servizio_Cod,
                                           Schema_Documenti_Template.Ordine,
                                           Schema_Documenti_Template.Tipologia AS ID_Tipologia,
                                           Schema_Documenti_Template.Suffisso_File,
                                           Schema_Documenti_Template.Descrizione,
                                           Schema_Documenti_Template.Flag_Obbligatorio,
                                           Schema_Documenti_Template.Flag_Firmato_Digit,
                                           Schema_Documenti_Template.Nr_Documenti,
	                                       CASE
		                                       WHEN Schema_Documenti_Template.Ambito = " & enum_Ambiti_UMA.UMA_Carburanti & " THEN 'UMA Carburanti'
		                                       WHEN Schema_Documenti_Template.Ambito = " & enum_Ambiti_UMA.Check_List & " THEN 'Check List'
		                                       ELSE 'Altro'
	                                       END as Ambito_Des,
	                                       CASE
		                                       WHEN Schema_Documenti_Template.Fase = " & enum_Fasi_UMA.UMA_prima_richiesta & " THEN 'UMA Prima Richiesta'
		                                       WHEN Schema_Documenti_Template.Fase = " & enum_Fasi_UMA.UMA_approvazione_prima_richiesta & " THEN 'UMA Approvazione Prima Richiesta'
		                                       WHEN Schema_Documenti_Template.Fase = " & enum_Fasi_UMA.UMA_richiesta_integrativa & " THEN 'UMA Richiesta Integrativa'
		                                       WHEN Schema_Documenti_Template.Fase = " & enum_Fasi_UMA.UMA_approvazione_richiesta_integrativa & " THEN 'UMA Approvazione Richiesta integrativa'
		                                       WHEN Schema_Documenti_Template.Fase = " & enum_Fasi_UMA.UMA_rendicontazione & " THEN 'UMA Rendicontazione'
		                                       WHEN Schema_Documenti_Template.Fase = " & enum_Fasi_UMA.UMA_approvazione_rendicontazione & " THEN 'UMA Approvazione Rendicontazione'
		                                       ELSE 'Altro'
	                                       END as Fase_Des,
                                           CASE
		                                       WHEN Schema_Documenti_Template.Flag_Firmato_Digit = 0 THEN 'No'
		                                       ELSE 'Si'
	                                       END as Firmato,
                                           CASE
		                                       WHEN Schema_Documenti_Template.Flag_obbligatorio = 0 THEN 'No'
		                                       ELSE 'Si'
	                                       END as Obbligatorio ")

                If bPermessixUtente Then

                    obj_CategTipologiaDocumentiXUtenti.Controllo_Permessi_SELECT(StrSQL, iLivelloGerarchia)

                Else
                    StrSQL.Append(", 1 as Autorizzato ")
                End If

                StrSQL.Append("      FROM Schema_Documenti_Template 
                                       LEFT JOIN (	
			                                        SELECT COUNT (*) AS Nr_Documenti_Presenti, Richiesta_Cod, Alert_entita.Id_Schema_Template 
			                                        FROM Alert_entita
			                                        INNER JOIN Allegati_Documenti on Alert_Entita.Allegati_Documenti_Cod = Allegati_Documenti.Allegati_Documenti_Cod
			                                        WHERE Alert_Entita.Richiesta_Cod = " & Richiesta_Cod & " 
			                                        GROUP BY Id_Schema_Template, Richiesta_Cod

			                                      ) AS Alert_entita_inner 
			                           on Alert_entita_inner.Id_Schema_Template = schema_Documenti_Template.Id_Schema_Template
                                       LEFT JOIN Alert_Tipologia ON Schema_Documenti_Template.Tipologia = Alert_Tipologia.ID_Tipologia ")

                If bPermessixUtente Then

                    obj_CategTipologiaDocumentiXUtenti.Controllo_Permessi_JOIN(StrSQL, iLivelloGerarchia,
                                                                            piva, False, objParametri)
                End If

                StrSQL.Append("       WHERE Schema_Documenti_Template.Id_Schema_Template IN ( " & Agro_SQL_Save_Clausola_IN(String.Join(",", idSchemaTemplate), False) & " ) ")

                If bPermessixUtente Then

                    obj_CategTipologiaDocumentiXUtenti.Controllo_Permessi_Filtro_WHERE(StrSQL, iLivelloGerarchia, Tipo_Permesso_Documentale.Lettura, False)

                End If

                'StrSQL.Append("       GROUP BY Alert_Entita.Allegati_Documenti_Cod,
                '                         Alert_Entita.Richiesta_Cod,
                '                               Alert_Tipologia.Nome, 
                '       Schema_Documenti_Template.Id_Schema_Template,
                '       Schema_Documenti_Template.Ambito,
                '       Schema_Documenti_Template.Fase,
                '       Schema_Documenti_Template.Flag_Firmato_Digit,
                '                               Schema_Documenti_Template.Flag_obbligatorio,
                '                               Schema_Documenti_Template.Servizio_Cod,
                '                               Schema_Documenti_Template.Ordine,
                '                               Schema_Documenti_Template.Tipologia,
                '                               Schema_Documenti_Template.Suffisso_File,
                '                               Schema_Documenti_Template.Descrizione,
                '                               Schema_Documenti_Template.Flag_Obbligatorio,
                '                               Schema_Documenti_Template.Flag_Firmato_Digit,
                '                               Schema_Documenti_Template.Nr_Documenti,
                '                               Alert_Tipologia.ID_Tipologia")
                'If bPermessixUtente Then

                '    StrSQL.AppendLine(" , permessi.Autorizzato")
                '    StrSQL.AppendLine(" , permessi.Id_Tipologia")

                '    If iLivelloGerarchia > 1 Then
                '        StrSQL.AppendLine(" , permessi_padre.Autorizzato")
                '        StrSQL.AppendLine(" , permessi_padre.Id_Tipologia")
                '    End If

                '    If iLivelloGerarchia > 2 Then
                '        StrSQL.AppendLine(" , permessi_nonno.Autorizzato")
                '        StrSQL.AppendLine(" , permessi_nonno.Id_Tipologia")
                '    End If

                '    If iLivelloGerarchia > 3 Then
                '        StrSQL.AppendLine(" , permessi_bis_nonno.Autorizzato")
                '        StrSQL.AppendLine(" , permessi_bis_nonno.Id_Tipologia")
                '    End If

                '    If iLivelloGerarchia > 4 Then
                '        StrSQL.AppendLine(" , permessi_tris_nonno.Autorizzato")
                '        StrSQL.AppendLine(" , permessi_tris_nonno.Id_Tipologia")
                '    End If

                '    If iLivelloGerarchia > 5 Then
                '        StrSQL.AppendLine(" , permessi_quad_nonno.Autorizzato")
                '        StrSQL.AppendLine(" , permessi_quad_nonno.Id_Tipologia")
                '    End If

                'End If

                If bPermessixUtente Then

                    obj_CategTipologiaDocumentiXUtenti.Controllo_Permessi_ORDER_BY(StrSQL, False)

                End If

            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If bPermessixUtente Then

                obj_CategTipologiaDocumentiXUtenti.Controllo_Permessi_Filtro_DataTable(DT, Tipo_Permesso_Documentale.Lettura, False)

            End If

            DT.Columns.Add("Pratica_Cod", GetType(Int32))
            DT.Columns.Add("WAnagraficaStati_Cod", GetType(Int32))
            DT.Columns.Add("WAnagraficaStati_Des", GetType(String))

            For Each r As DataRow In DT.Rows
                r("Pratica_Cod") = praticaCod
                r("WAnagraficaStati_Cod") = statoAttuale
                r("WAnagraficaStati_Des") = statoAttualeDes
            Next

            Return DT
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
    End Function







    Public Function EstraiDati(ByRef objParametri As AgronicaCoreParametri, ByVal piva As String, ByVal Richiesta_Cod As Integer) As DataTable

        Const NomeRoutine = "AgronicaCoreAnagrafeDAL.SchemaDocumenti.EstraiDati()"

        Dim MessaggioErrore As String = ""
        Dim DT As DataTable

        Dim bPermessixUtente As Boolean = False 'Booleano per la presenza di record in tabella obj_CategTipologiaDocumentiXUtenti
        'In caso non esistano record viene bypassato il filtro

        Dim iLivelloGerarchia As Integer = 0 'Livello Max della tabella GerarchiaImprese (in modo da costruire un filtro dinamico)

        Dim obj_CategTipologiaDocumentiXUtenti As New AgronicaCoreScadenziario.CategTipologiaDocumentiXUtenti_R

        Try
            Dim StrSQL As New StringBuilder
            StrSQL.Length = 0


            '********************************************estrae dati in base a Piva e Richiesta_Cod ******************************


            StrSQL.Append("SELECT   UMA_Richieste_Testata.Pratica_Cod,
			                        UMA_Richieste_Testata.Richiesta_Cod,
			                        UMA_Richieste_Testata.Avanzamento_Richiesta,
			                        UMA_Richieste_Testata.Tipo_Richiesta,
			                        Pratiche.Anno,
			                        Pratiche.Servizio_Cod,
			                        Pratiche.Pratica_Cod,
                                    Pratiche_Stati_Attuali.Stato_Cod,
                                    WAnagraficaStati.WAnagraficaStati_Cod,
                                    WAnagraficaStati.WAnagraficaStati_Des
                           FROM Pratiche
                           JOIN Pratiche_Stati_Attuali ON Pratiche.Pratica_Cod = Pratiche_Stati_Attuali.Pratica_Cod
                           LEFT JOIN UMA_Richieste_Testata ON UMA_Richieste_Testata.Pratica_Cod = Pratiche.Pratica_Cod
                           LEFT JOIN WAnagraficaStati ON Pratiche_Stati_Attuali.Stato_Cod = WAnagraficaStati.WAnagraficaStati_Cod
                           WHERE UMA_Richieste_Testata.Piva = '" & Agro_SQL_SaveText(piva) & "' AND UMA_Richieste_Testata.Richiesta_Cod = " & Richiesta_Cod & "
                           ORDER BY Anno")


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            Return DT

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

    End Function




    Public Function EstraiFase(ByRef objParametri As AgronicaCoreParametri,
                               ByVal piva As String,
                               ByVal avanzamento As Integer,
                               ByVal tipoRichiesta As Integer,
                               ByVal annoQuery As Integer,
                               ByVal Richiesta_Cod As Integer,
                               ByVal praticaCod As Integer,
                               ByVal statoAttuale As Integer,
                               ByRef objParametri_Utenti As AgronicaCoreParametri
                               ) As Integer

        Const NomeRoutine = "AgronicaCoreAnagrafeDAL.SchemaDocumenti.EstraiFase()"

        Dim MessaggioErrore As String = ""
        Dim DT As DataTable

        Dim bPermessixUtente As Boolean = False 'Booleano per la presenza di record in tabella obj_CategTipologiaDocumentiXUtenti
        'In caso non esistano record viene bypassato il filtro

        Dim iLivelloGerarchia As Integer = 0 'Livello Max della tabella GerarchiaImprese (in modo da costruire un filtro dinamico)

        Dim obj_CategTipologiaDocumentiXUtenti As New AgronicaCoreScadenziario.CategTipologiaDocumentiXUtenti_R

        Try
            Dim StrSQL As New StringBuilder
            StrSQL.Length = 0


            '********************************************estrae dati in base a Piva e Richiesta_Cod ******************************


            StrSQL.Append("SELECT UMA_Richieste_Testata.Richiesta_Cod,
                                  UMA_Richieste_Testata.Avanzamento_Richiesta,
                                  UMA_Richieste_Testata.Tipo_Richiesta,
                                  UMA_Richieste_Testata.Piva
                           FROM Pratiche
                           LEFT JOIN UMA_Richieste_Testata ON UMA_Richieste_Testata.Pratica_Cod = Pratiche.Pratica_Cod
                           WHERE Pratiche.Anno = " & annoQuery & "
                           AND UMA_Richieste_Testata.Piva = '" & Agro_SQL_SaveText(piva) & "' 
                           AND UMA_Richieste_Testata.Avanzamento_Richiesta = " & avanzamento & " 
                           AND UMA_Richieste_Testata.Tipo_Richiesta = " & tipoRichiesta & "
                           ORDER BY Richiesta_Cod ")

            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)

            Dim richiesteAssociate As New List(Of Integer)

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

                For Each r As DataRow In DT.Rows
                    richiesteAssociate.Add(If(Not IsDBNull(r("Richiesta_Cod")), CInt(r("Richiesta_Cod")), 0))
                Next

            End If
            If Not richiesteAssociate.Any Then
                richiesteAssociate.Add(-1)
            End If

            Dim fase As Integer = 0

            Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

            Dim permessoRichiesta = objPermessi.Controlla_Permessi_Utente(
                                            objParametri_Utenti.UtenteUsername,
                                            5,
                                            enum_Security_Attivita.Richiesta_UMA,
                                            enum_Security_Operazione.Modifica,
                                            Date.Now,
                                            "",
                                            objParametri_Utenti)

            Dim permessoApprovazioneRichiesta = objPermessi.Controlla_Permessi_Utente(
                                            objParametri_Utenti.UtenteUsername,
                                            5,
                                            enum_Security_Attivita.Approvazione_Richiesta_UMA,
                                            enum_Security_Operazione.Modifica,
                                            Date.Now,
                                            "",
                                            objParametri_Utenti)

            Dim permessoRendicontazione = objPermessi.Controlla_Permessi_Utente(
                                            objParametri_Utenti.UtenteUsername,
                                            5,
                                            enum_Security_Attivita.Rendicontazione_UMA,
                                            enum_Security_Operazione.Modifica,
                                            Date.Now,
                                            "",
                                            objParametri_Utenti)

            Dim permessoApprovazioneRendicontazione = objPermessi.Controlla_Permessi_Utente(
                                            objParametri_Utenti.UtenteUsername,
                                            5,
                                            enum_Security_Attivita.Approvazione_Rendicontazione_UMA,
                                            enum_Security_Operazione.Modifica,
                                            Date.Now,
                                            "",
                                            objParametri_Utenti)

            If (praticaCod <> -1) Then


                '************stabilisce la fase in base alle Richieste Associate ************
                If (Richiesta_Cod = richiesteAssociate(0)) Then

                    If (avanzamento = 0 OrElse avanzamento = -1) Then

                        If (permessoRichiesta) Then

                            fase = enum_Fasi_UMA.UMA_prima_richiesta

                        ElseIf (permessoApprovazioneRichiesta) Then

                            fase = enum_Fasi_UMA.UMA_approvazione_prima_richiesta

                        End If

                    ElseIf (avanzamento = 1) Then

                        If (permessoRendicontazione) Then

                            fase = enum_Fasi_UMA.UMA_rendicontazione

                        ElseIf (permessoApprovazioneRendicontazione) Then

                            fase = enum_Fasi_UMA.UMA_approvazione_rendicontazione

                        End If

                    End If

                Else

                    If (avanzamento = 0 OrElse avanzamento = -1) Then

                        If (permessoRichiesta) Then

                            fase = enum_Fasi_UMA.UMA_richiesta_integrativa

                        ElseIf (permessoApprovazioneRichiesta) Then

                            fase = enum_Fasi_UMA.UMA_approvazione_richiesta_integrativa

                        End If

                    ElseIf (avanzamento = 1) Then

                        If (permessoRendicontazione) Then

                            fase = enum_Fasi_UMA.UMA_rendicontazione

                        ElseIf (permessoApprovazioneRendicontazione) Then

                            fase = enum_Fasi_UMA.UMA_approvazione_rendicontazione

                        End If

                    End If

                End If

            End If


            Return fase
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
    End Function


















    Public Function Leggi(ByVal Id_Schema_Template As Integer,
                          ByVal Ambito As Integer,
                          ByVal Fase As Integer,
                          ByVal Tipologia As Integer,
                          ByVal Nome_Modello As String,
                          ByVal Nr_Documenti As Integer,
                          ByVal Validita_Inizio As Date,
                          ByVal Validita_Fine As Date,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByRef objParametri_Server As AgronicaCoreParametri,
                          ByRef objParametri_Utenti As AgronicaCoreParametri,
                          Optional ByVal inviato As Date = Nothing,
                          Optional ByVal datainvio As Date = AGRODATAINIZIO,
                          Optional ByVal Servizio_Cod As Integer = 0,
                          Optional ByVal Stato_Da As Integer = 0,
                          Optional ByVal Stato_A As Integer = 0,
                          Optional ByVal Ordine As Integer = 0,
                          Optional ByVal Suffisso_File As String = "",
                          Optional ByVal Descrizione As String = "",
                          Optional ByVal Flag_Obbligatorio As Integer = 0,
                          Optional ByVal Flag_Firmato_Digit As Integer = 0
                          ) As DataTable


        Dim NomeRoutine As String = "AgronicaScadenzarioDAL.RichiestaDocumenti_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim PivaSuperUser = objParametri_Server.PivaSuperUser

        Try
            Select Case xSelezioneVariabile
                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT * ")
                    StrSQL.AppendLine(" FROM  Schema_Documenti_Template  ")
                    StrSQL.AppendLine(" WHERE 1 = 1 ")

                    If Id_Schema_Template = Nothing Then
                        StrSQL.AppendLine("AND Id_Schema_Template = " & Agro_SQL_SaveText_NULL(Id_Schema_Template) & " ")
                    End If

                    If Ambito > -14 OrElse Ambito < -15 Then
                        StrSQL.AppendLine("AND Ambito = " & Agro_SQL_SaveText_NULL(Ambito) & " ")
                    End If

                    If Fase > -16 OrElse Fase < -21 Then
                        StrSQL.AppendLine("AND Fase = " & Agro_SQL_SaveText_NULL(Fase) & " ")
                    End If

                    If Tipologia = Nothing Then
                        StrSQL.AppendLine("AND Tipologia = " & Agro_SQL_SaveText_NULL(Tipologia) & " ")
                    End If

                    If Nome_Modello <> "" Then
                        StrSQL.AppendLine("AND Nome_Modello = " & Agro_SQL_SaveText_NULL(Nome_Modello) & " ")
                    End If

                    If Nr_Documenti = Nothing Then
                        StrSQL.AppendLine("AND Nr_Documenti = " & Agro_SQL_SaveText_NULL(Nr_Documenti) & " ")
                    End If

            End Select

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

    'Class VoceElencoDiDropdown
    '    Public Code As Integer
    '    Public Descrizione As String

    '    Public Sub New(cod As Integer, des As String)
    '        Code = cod
    '        Descrizione = des
    '    End Sub
    'End Class

    Public Function VerificaElementoNonEsisteInDB(lav As RichiestaDocumenti_W.RichiestaDocumentiDto,
                                                efConnString As String) As Boolean
        Using dal As New Gias_DeveloperServer_Entities(efConnString)
            Dim exists = dal.Alert_Entita.Any(Function(s) s.ID_Alert_Entita = lav.ID_Alert_Entita)
            Return exists
        End Using
    End Function

End Class

Public Class RichiestaDocumenti_W
    Inherits AgronicaCoreDataProvider.DataProvider

    'Public Function AggiungiNouvi(scdoc As List(Of RichiestaDocumentiDto),
    '                              context As Gias_DeveloperServer_Entities,
    '                              objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.RichiestaDocumenti.AggiungiNouvi()"
    '    Dim MessaggioErrore As String = ""

    '    Try
    '        For Each sd In scdoc
    '            context.Alert_Entita.Add(sd.ToRichiestaDocumentiDB())
    '        Next
    '        context.SaveChanges()
    '    Catch ex As Exception
    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
    '    End Try

    '    Return True
    'End Function

    'Public Function Rimuovi(lista As List(Of RichiestaDocumentiDto), context As Gias_DeveloperServer_Entities, objParametri As AgronicaCoreParametri) As Boolean
    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.RichiestaDocumenti_W.Rimuovi()"

    '    ' ------------- Variabili -------------
    '    Dim DT As DataTable
    '    Dim MessaggioErrore As String = ""

    '    Try
    '        For Each elem In lista
    '            Dim record As Alert_Entita = context.Alert_Entita.Where(Function(s) s.ID_Alert_Entita = elem.ID_Alert_Entita).FirstOrDefault()

    '            If Not record Is Nothing Then
    '                context.Alert_Entita.Remove(record)
    '            End If
    '        Next
    '        context.SaveChanges()

    '    Catch ex As Exception
    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        DT = Nothing
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
    '    End Try

    '    Return True
    'End Function

    'Public Function Aggiorna(righeModificateArr As List(Of RichiestaDocumentiDto),
    '                         context As Gias_DeveloperServer_Entities,
    '                         ByRef objParametri As AgronicaCoreParametri) As Boolean
    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.RichiestaDocumenti_W.Aggiorna()"

    '    ' ------------- Variabili -------------
    '    Dim DT As DataTable
    '    Dim MessaggioErrore As String = ""

    '    Try

    '        For Each elem In righeModificateArr
    '            Dim result As Alert_Entita = context.Alert_Entita.FirstOrDefault(Function(s) s.ID_Alert_Entita = elem.ID_Alert_Entita)

    '            If Not result Is Nothing Then
    '                result.Id_Schema_Template = elem.Id_Schema_Template
    '                result.Richiesta_Cod = elem.Richiesta_Cod
    '            End If
    '        Next
    '        context.SaveChanges()
    '    Catch ex As Exception
    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        DT = Nothing
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
    '    End Try

    '    Return True
    'End Function




    Class RichiestaDocumentiDto

        Public ID_Alert_Entita As Integer
        Public Allegati_Documenti_Cod As Integer
        Public Richiesta_Cod As Integer
        Public Tipologia_Des As String
        Public Ambito_Des As String
        Public Fase_Des As String
        Public Firmato As String
        Public Obbligatorio As String
        'Public Autorizzato As Integer
        'Public Id_Tipologia As Integer
        ''Public Autorizzato As Integer
        ''Public Id_Tipologia As Integer
        ''Public Autorizzato As Integer
        ''Public Id_Tipologia As Integer
        ''Public Autorizzato As Integer
        ''Public Id_Tipologia As Integer
        ''Public Autorizzato As Integer
        ''Public Id_Tipologia As Integer
        ''Public Id_Tipologia As Integer



        ' ************************* Collone della tabella Schema_Documenti_Template *************************
        Public Id_Schema_Template As Integer

        Public Ambito As Integer
        Public Servizio_Cod As Integer
        Public Stato_Da As Integer
        Public Stato_A As Integer
        Public Fase As Integer
        Public Ordine As Integer
        Public Tipologia As Integer
        Public Suffisso_File As String
        Public Descrizione As String
        Public Flag_Obbligatorio As Integer
        Public Flag_Firmato_Digit As Integer
        Public Nome_Modello As String
        Public Nr_Documenti As Integer
        Public inviato As Integer
        Public datainvio As Date?
        Public Data_Creazione As Date?
        Public Data_Modifica As Date?
        Public Username_Creazione As String
        Public Username_Modifica As String
        Public Validita_Inizio As Date?
        Public Validita_Fine As Date?
    End Class

End Class

'Module Extensions
'    <Extension()>
'    Function ToRichiestaDocumentiDB(ByVal sd As RichiestaDocumenti_W.RichiestaDocumentiDto) As AgronicaCoreEntityFramework_POCO.Alert_Entita
'        Dim r As New AgronicaCoreEntityFramework_POCO.Alert_Entita
'        r.Id_Schema_Template = sd.Id_Schema_Template
'        ' r.Richiesta_Cod = sd.Richiesta_Cod
'        r.Allegati_Documenti_Cod = sd.Allegati_Documenti_Cod
'        Return r
'    End Function
'End Module