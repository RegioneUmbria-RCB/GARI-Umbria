Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO


'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################


Public Class SchemaDocumenti_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiSchemaDocumenti(ByRef objParametri As AgronicaCoreParametri) As DataTable

        Const NomeRoutine = "AgronicaCoreScadenziario_DAL.SchemaDocumenti.LeggiSchemaDocumenti()"

        Dim MessaggioErrore As String = ""
        Dim DT As DataTable

        Try
            Dim StrSQL As New StringBuilder
            StrSQL.Length = 0

            StrSQL.Append("SELECT Servizi.Servizio_Des,
	                                    CASE Schema_Documenti_Template.Stato_Da 
											WHEN 0 THEN 'Nessuno'
											WHEN -1 THEN 'Tutti'
											ELSE stato_da.WAnagraficaStati_Des 
										END
										as Stato_Da_Des,
	                                    CASE Schema_Documenti_Template.Stato_A 
											WHEN 0 THEN 'Nessuno'
											WHEN -1 THEN 'Tutti'
											ELSE stato_a.WAnagraficaStati_Des 
										END
										as Stato_A_Des,
                                        Nome as Tipologia_Des,
	                                    Schema_Documenti_Template.*,
	                                    CASE
		                                    WHEN Schema_Documenti_Template.Ambito = -14 THEN 'UMA Carburanti'
		                                    WHEN Schema_Documenti_Template.Ambito = -15 THEN 'Check List'
		                                    ELSE 'Altro'
	                                    END as Ambito_Des,
	                                    CASE
		                                    WHEN Schema_Documenti_Template.Fase = -16 THEN 'UMA Prima Richiesta'
		                                    WHEN Schema_Documenti_Template.Fase = -17 THEN 'UMA Approvazione Prima Richiesta'
		                                    WHEN Schema_Documenti_Template.Fase = -18 THEN 'UMA Richiesta Integrativa'
		                                    WHEN Schema_Documenti_Template.Fase = -19 THEN 'UMA Approvazione Richiesta integrativa'
		                                    WHEN Schema_Documenti_Template.Fase = -20 THEN 'UMA Rendicontazione'
		                                    WHEN Schema_Documenti_Template.Fase = -21 THEN 'UMA Approvazione Rendicontazione'
		                                    ELSE 'Altro'
	                                    END as Fase_Des,
                                        CASE
		                                    WHEN Schema_Documenti_Template.Flag_Firmato_Digit = 0 THEN 'No'
		                                    ELSE 'Si'
	                                    END as Firmato,
                                        CASE
		                                    WHEN Schema_Documenti_Template.Flag_obbligatorio = 0 THEN 'No'
		                                    ELSE 'Si'
	                                    END as Obbligatorio
                                FROM Schema_Documenti_Template
                                LEFT JOIN Servizi ON Schema_Documenti_Template.Servizio_Cod = Servizi.Servizio_Cod
                                LEFT JOIN WAnagraficaStati stato_da ON Schema_Documenti_Template.Stato_Da = stato_da.WAnagraficaStati_Cod
                                LEFT JOIN WAnagraficaStati stato_a ON Schema_Documenti_Template.Stato_A = stato_a.WAnagraficaStati_Cod
                                JOIN alert_tipologia ON Schema_Documenti_Template.Tipologia = ID_Tipologia")


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

    Public Function Leggi(ByVal Id_Schema_Template As String,
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


        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.SchemaDocumenti_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim PivaSuperUser = objParametri_Server.PivaSuperUser

        Try
            Select Case xSelezioneVariabile
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
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

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT Id_Schema_Template ")
                    StrSQL.AppendLine(" FROM  Schema_Documenti_Template  ")
                    StrSQL.AppendLine(" WHERE 1 = 1 ")

                    If Tipologia <> 0 Then
                        StrSQL.AppendLine("AND Tipologia = " & Agro_SQL_SaveNum(Tipologia) & " ")
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

    Public Function CheckDocumentiObbligatori(ByVal pratica_cod As Integer,
                                              ByVal servizio_cod As Integer,
                                              ByVal anno As Integer,
                                              ByVal stato_da As Integer,
                                              ByVal stato_a As Integer,
                                              ByVal xFiltroAggiuntivo As String,
                                              ByVal xOrderBy As String,
                                              ByRef ObjParametri As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.SchemaDocumenti.CheckDocumentiObbligatori()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim fase As Integer

        Try

            'sezione preparazione variabili necessarie ∀ servizio
            Select Case servizio_cod
                Case 2007
                    Dim richieste As New AgronicaCoreUmaDal.UMA_Richieste_Testata_R
                    Dim testata As DataTable = richieste.Leggi("", 0, pratica_cod, AGRODATAINIZIO, AGRODATAFINE, ObjParametri)
                    If (testata.Rows.Count > 0 AndAlso stato_a <> 2010) Then 'Stato verifica in corso da assegnare
                        Dim ric As DataTable = richieste.Leggi(testata.Rows.Item(0).Item("Piva"), 0, 0, AGRODATAINIZIO, AGRODATAFINE, ObjParametri, isTerzista:=testata.Rows.Item(0).Item("Tipo_Richiesta"),
                                                                avanzamento:=testata.Rows.Item(0).Item("Avanzamento_Richiesta"), anno:=anno)

                        If (ric.Rows.Item(0).Item("Avanzamento_Richiesta") = 0) Then

                            If (testata.Rows.Item(0).Item("richiesta_Cod") = ric.Rows.Item(0).Item("richiesta_cod")) Then

                                If (stato_da = enum_WWorflow_WAnagraficaStati.Quaderno_Campagna_In_Compilazione) Then

                                    fase = enum_Fasi_UMA.UMA_prima_richiesta

                                ElseIf (stato_da = enum_WWorflow_WAnagraficaStati.Quaderno_Campagna_Verifica_in_corso) Then

                                    fase = enum_Fasi_UMA.UMA_approvazione_prima_richiesta

                                End If
                            Else

                                If (stato_da = enum_WWorflow_WAnagraficaStati.Quaderno_Campagna_In_Compilazione) Then

                                    fase = enum_Fasi_UMA.UMA_richiesta_integrativa

                                ElseIf (stato_da = enum_WWorflow_WAnagraficaStati.Quaderno_Campagna_Verifica_in_corso) Then

                                    fase = enum_Fasi_UMA.UMA_approvazione_richiesta_integrativa

                                End If

                            End If


                        ElseIf (stato_da = enum_WWorflow_WAnagraficaStati.Quaderno_Campagna_In_Compilazione) Then

                            fase = enum_Fasi_UMA.UMA_rendicontazione

                        ElseIf (stato_da = enum_WWorflow_WAnagraficaStati.Quaderno_Campagna_Verifica_in_corso) Then

                            fase = enum_Fasi_UMA.UMA_approvazione_rendicontazione

                        End If

                    End If
            End Select

            StrSQL.Length = 0

            StrSQL.AppendLine("SELECT * FROM Schema_Documenti_Template sdt ")
            Select Case servizio_cod
                Case 2007
                    StrSQL.AppendLine(" LEFT JOIN (SELECT ae.Id_Schema_Template, p.Pratica_Cod  ")
                    StrSQL.AppendLine("            FROM alert_Entita ae ")
                    StrSQL.AppendLine("            JOIN UMA_Richieste_Testata t ON t.Richiesta_Cod = ae.Richiesta_Cod ")
                    StrSQL.AppendLine("            JOIN Pratiche p ON p.Pratica_Cod = t.Pratica_Cod ")
                    StrSQL.AppendLine("            WHERE p.Pratica_Cod = " & pratica_cod & " ) doc ON doc.Id_Schema_Template = sdt.Id_Schema_Template ")
            End Select

            StrSQL.AppendLine(" WHERE 1 = 1 ")
            StrSQL.AppendLine(" AND sdt.Servizio_Cod = " & servizio_cod.ToString & " ")
            StrSQL.AppendLine(" AND sdt.Flag_Obbligatorio = 1 ")
            StrSQL.AppendLine(" AND sdt.Stato_Da = " & stato_da.ToString & " ")
            StrSQL.AppendLine(" AND sdt.Fase = " & fase.ToString & " ")

            'sezione where ∀ servizio
            Select Case servizio_cod
                Case 2007

            End Select

            If (xFiltroAggiuntivo <> "") Then
                StrSQL.AppendLine(" " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo) & " ")
            End If

            If (xOrderBy <> "") Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy) & " ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(ObjParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            Return DT

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

    End Function

    Class VoceElencoDiDropdown
        Public Code As Integer
        Public Descrizione As String

        Public Sub New(cod As Integer, des As String)
            Code = cod
            Descrizione = des
        End Sub
    End Class

    Public Function VerificaElementoNonEsisteInDB(lav As SchemaDocumenti_W.SchemaDocumentiDto,
                                                efConnString As String) As Boolean
        Using dal As New Gias_DeveloperServer_Entities(efConnString)
            Dim exists = dal.Schema_Documenti_Template.Any(Function(s) s.Id_Schema_Template = lav.Id_Schema_Template)
            Return exists
        End Using
    End Function

End Class

Public Class SchemaDocumenti_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function AggiungiNouvi(scdoc As List(Of SchemaDocumentiDto),
                                  context As Gias_DeveloperServer_Entities,
                                  objParametri As AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreScadenziarioDAL.SchemaDocumenti.AggiungiNouvi()"
        Dim MessaggioErrore As String = ""

        Try
            For Each sd In scdoc
                context.Schema_Documenti_Template.Add(sd.ToSchemaDocumentiDB())
            Next
            context.SaveChanges()
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return True
    End Function

    Public Function Rimuovi(lista As List(Of SchemaDocumentiDto), context As Gias_DeveloperServer_Entities, objParametri As AgronicaCoreParametri) As Boolean

        Const NomeRoutine = "AgronicaCoreScadenziario_DAL.SchemaDocumenti.Rimuovi()"

        Dim DT As DataTable
        Dim MessaggioErrore As String = ""

        Try
            For Each elem In lista
                Dim record As Schema_Documenti_Template = context.Schema_Documenti_Template.Where(Function(s) s.Id_Schema_Template = elem.Id_Schema_Template).FirstOrDefault()

                If record IsNot Nothing Then
                    context.Schema_Documenti_Template.Remove(record)
                End If
            Next
            context.SaveChanges()

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return True
    End Function

    Public Function Aggiorna(righeModificateArr As List(Of SchemaDocumentiDto),
                             context As Gias_DeveloperServer_Entities,
                             ByRef objParametri As AgronicaCoreParametri) As Boolean

        Const NomeRoutine = "AgronicaCoreScadenziario_DAL.SchemaDocumenti.Aggiorna()"

        Dim DT As DataTable
        Dim MessaggioErrore As String = ""

        Try

            For Each elem In righeModificateArr
                Dim result As Schema_Documenti_Template = context.Schema_Documenti_Template.FirstOrDefault(Function(s) s.Id_Schema_Template = elem.Id_Schema_Template)

                If result IsNot Nothing Then
                    result.Id_Schema_Template = elem.Id_Schema_Template
                    result.Ambito = elem.Ambito
                    result.Servizio_Cod = elem.Servizio_Cod
                    result.Stato_Da = elem.Stato_Da
                    result.Stato_A = elem.Stato_A
                    result.Fase = elem.Fase
                    result.Ordine = elem.Ordine
                    result.Tipologia = elem.Tipologia
                    result.Suffisso_File = elem.Suffisso_File
                    result.Descrizione = elem.Descrizione
                    result.Flag_Obbligatorio = elem.Flag_Obbligatorio
                    result.Flag_Firmato_Digit = elem.Flag_Firmato_Digit
                    result.Nome_Modello = elem.Nome_Modello
                    result.Nr_Documenti = elem.Nr_Documenti
                    result.inviato = elem.inviato
                    result.datainvio = elem.datainvio
                    result.Data_Creazione = elem.Data_Creazione
                    result.Data_Modifica = elem.Data_Modifica
                    result.Username_Creazione = elem.Username_Creazione
                    result.Username_Modifica = elem.Username_Modifica
                    result.Validita_Inizio = elem.Validita_Inizio
                    result.Validita_Fine = elem.Validita_Fine
                End If
            Next
            context.SaveChanges()
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return True
    End Function




    Class SchemaDocumentiDto

        Public Servizio_Des As String
        Public Stato_Da_Des As String
        Public Stato_A_Des As String
        Public Tipologia_Des As String
        Public Ambito_Des As String
        Public Fase_Des As String
        Public Firmato As String
        Public Obbligatorio As String



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

Module Extensions
    <Runtime.CompilerServices.Extension()>
    Function ToSchemaDocumentiDB(ByVal sd As SchemaDocumenti_W.SchemaDocumentiDto) As AgronicaCoreEntityFramework_POCO.Schema_Documenti_Template
        Dim r As New AgronicaCoreEntityFramework_POCO.Schema_Documenti_Template
        r.Id_Schema_Template = sd.Id_Schema_Template
        r.Ambito = sd.Ambito
        r.Servizio_Cod = sd.Servizio_Cod
        r.Stato_Da = sd.Stato_Da
        r.Stato_A = sd.Stato_A
        r.Fase = sd.Fase
        r.Ordine = sd.Ordine
        r.Tipologia = sd.Tipologia
        r.Suffisso_File = sd.Suffisso_File
        r.Descrizione = sd.Descrizione
        r.Flag_Obbligatorio = sd.Flag_Obbligatorio
        r.Flag_Firmato_Digit = sd.Flag_Firmato_Digit
        r.Nome_Modello = sd.Nome_Modello
        r.Nr_Documenti = sd.Nr_Documenti
        r.inviato = sd.inviato
        r.datainvio = sd.datainvio
        r.Data_Creazione = sd.Data_Creazione
        r.Data_Modifica = sd.Data_Modifica
        r.Username_Creazione = sd.Username_Creazione
        r.Username_Modifica = sd.Username_Modifica
        r.Validita_Inizio = sd.Validita_Inizio
        r.Validita_Fine = sd.Validita_Fine
        Return r
    End Function
End Module
