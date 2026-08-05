Imports System.Runtime.CompilerServices
Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreAnagrafeDAL.UMAConfigurazioneAllevamenti_W

Public Class UMAConfigurazioneAllevamenti_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiUMAConfigurazioneAllevamenti(ByRef objParametri As AgronicaCoreParametri) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMAConfigurazioneAllevamenti.LeggiUMAConfigurazioneAllevamenti()"

        ' ------- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Dim StrSQL As New System.Text.StringBuilder
            StrSQL.Length = 0

            StrSQL.Append("SELECT UMA_Allevamenti.UMA_All_Des,
		                          UMA_Configurazione_Allevamenti.*,
		                          UMA_Configurazione_Allevamenti.Regione_Cod + '_' + UMA_Configurazione_Allevamenti.UMA_All_Cod as Chiave,
                                  CASE
		                                WHEN UMA_Configurazione_Allevamenti.tipo_operazione = 0 THEN 'Ordinaria'
		                                WHEN UMA_Configurazione_Allevamenti.tipo_operazione = 1 THEN 'Straordinaria'
									    ELSE 'Altro'
								  END AS Tipo_Operazione_Des
		                          FROM UMA_Configurazione_Allevamenti
		                   LEFT JOIN UMA_Allevamenti ON UMA_Configurazione_Allevamenti.UMA_All_Cod = UMA_Allevamenti.UMA_All_Cod")


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

    Public Function Leggi(ByVal Regione_Cod As Integer,
                          ByVal UMA_All_Cod As Integer,
                          ByVal Tipo_Operazione As Integer,
                          ByVal Gasolio_Lt As Double,
                          ByVal Benzina_Lt As Double,
                          ByVal Qta_Aggiuntiva_Carro As Double,
                          ByVal N_Max_Allevamenti As Integer,
                          ByVal Validita_Inizio As Date,
                          ByVal Validita_Fine As Date,
                          ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                          ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          Optional ByVal inviato As Integer = Nothing,
                          Optional ByVal datainvio As Date = AGRODATAINIZIO
                         ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMAConfigurazioneAllevamenti_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim PivaSuperUser = objParametri_Server.PivaSuperUser

        Try
            Select Case xSelezioneVariabile
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT * ")
                    StrSQL.AppendLine(" FROM  UMA_Configurazione_Allevamenti  ")
                    StrSQL.AppendLine(" WHERE 1 = 1 ")

                    If Regione_Cod = Nothing Then
                        StrSQL.AppendLine("AND Regione_Cod = " & Agro_SQL_SaveText_NULL(Regione_Cod) & " ")
                    End If

                    If UMA_All_Cod = Nothing Then
                        StrSQL.AppendLine("AND UMA_All_Cod = " & Agro_SQL_SaveText_NULL(UMA_All_Cod) & " ")
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

    Class VoceElencoDiDropdown
        Public Code As Integer
        Public Descrizione As String

        Public Sub New(cod As Integer, des As String)
            Code = cod
            Descrizione = des
        End Sub
    End Class

    Public Function VerificaElementoNonEsisteInDB(lav As UMAConfigurazioneAllevamentiDto,
                                                efConnString As String) As Boolean
        'Using dal As New Gias_DeveloperServer_Entities(efConnString)
        '    Dim exists = dal.UMA_Configurazione_Allevamenti.Any(Function(s) s.UMA_All_Cod = lav.UMA_All_Cod)
        '    Return exists
        'End Using
    End Function

End Class

Public Class UMAConfigurazioneAllevamenti_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function AggiungiNuovi(scdoc As List(Of UMAConfigurazioneAllevamentiDto),
                                  ByRef context As Gias_DeveloperServer_Entities,
                                  objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMAConfigurazioneAllevamenti.AggiungiNouvi()"
        Dim MessaggioErrore As String = ""
        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                ' Contiene anche i dettagli

                For Each sd In scdoc
                    context.UMA_Configurazione_Allevamenti.Add(sd.ToUMAConfigurazioneAllevamentiDB())
                Next

                GiasContext.SaveChanges()
                'Gias Context.SaveChanges() è come se fosse una transazione se c'è un errore,
                'nelle righe inserite,cancellate o modificate viene annullata tutta la scrittura
            End Using
            'For Each sd In scdoc
            '    context.UMA_Configurazione_Allevamenti.Add(sd.ToUMAConfigurazioneAllevamentiDB())
            'Next
            'context.SaveChanges()
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return True
    End Function

    Public Function Rimuovi(lista As List(Of UMAConfigurazioneAllevamentiDto), context As Gias_DeveloperServer_Entities, objParametri As AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMAConfigurazioneAllevamenti.Rimuovi()"

        ' ------------- Variabili -------------
        Dim DT As DataTable
        Dim MessaggioErrore As String = ""

        Try
            For Each elem In lista
                Dim record As UMA_Configurazione_Allevamenti = context.UMA_Configurazione_Allevamenti.Where(Function(s) s.UMA_All_Cod = elem.UMA_All_Cod).FirstOrDefault()

                If Not record Is Nothing Then
                    context.UMA_Configurazione_Allevamenti.Remove(record)
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

    Public Function Aggiorna(righeModificateArr As List(Of UMAConfigurazioneAllevamentiDto),
                             context As Gias_DeveloperServer_Entities,
                             ByRef objParametri As AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMAConfigurazioneAllevamenti.Aggiorna()"

        ' ------------- Variabili -------------
        Dim DT As DataTable
        Dim MessaggioErrore As String = ""

        Try

            For Each elem In righeModificateArr
                Dim result As UMA_Configurazione_Allevamenti = context.UMA_Configurazione_Allevamenti.FirstOrDefault(Function(s) s.UMA_All_Cod = elem.UMA_All_Cod)

                If Not result Is Nothing Then
                    result.Regione_Cod = elem.Regione_Cod
                    result.UMA_All_Cod = elem.UMA_All_Cod
                    result.Tipo_Operazione = elem.Tipo_Operazione
                    result.Gasolio_Lt = elem.Gasolio_Lt
                    result.Benzina_Lt = elem.Benzina_Lt
                    result.Qta_Aggiuntiva_Carro = elem.Qta_Aggiuntiva_Carro
                    result.N_Max_Allevamenti = elem.N_Max_Allevamenti
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




    Class UMAConfigurazioneAllevamentiDto

        Public UMA_All_Des As String

        ' ************************* Collone della tabella UMA_Configurazione_Allevamenti *************************
        Public Regione_Cod As String
        Public UMA_All_Cod As String

        Public Tipo_Operazione As Integer
        Public Gasolio_Lt As Double
        Public Benzina_Lt As Double
        Public Qta_Aggiuntiva_Carro As Double
        Public N_Max_Allevamenti As Integer
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
    <Extension()>
    Function ToUMAConfigurazioneAllevamentiDB(ByVal sd As UMAConfigurazioneAllevamenti_W.UMAConfigurazioneAllevamentiDto) As AgronicaCoreEntityFramework_POCO.UMA_Configurazione_Allevamenti
        Dim r As New AgronicaCoreEntityFramework_POCO.UMA_Configurazione_Allevamenti
        r.Regione_Cod = sd.Regione_Cod
        r.UMA_All_Cod = sd.UMA_All_Cod
        r.Gasolio_Lt = sd.Gasolio_Lt
        r.Benzina_Lt = sd.Benzina_Lt
        r.Qta_Aggiuntiva_Carro = sd.Qta_Aggiuntiva_Carro
        r.N_Max_Allevamenti = sd.N_Max_Allevamenti
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

