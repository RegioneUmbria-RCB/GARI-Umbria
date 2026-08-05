Imports System.Runtime.CompilerServices
Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreUmaDal.UMAConfigurazioneAllevamenti_W

Public Class UMAConfigurazioneAllevamenti_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiUMAConfigurazioneAllevamenti(ByRef objParametri As AgronicaCoreParametri, Optional InizioValidita As String = "", Optional FineValidita As String = "") As DataTable

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

            StrSQL.AppendLine(" WHERE 1 = 1  ")

            If Not String.IsNullOrEmpty(FineValidita) Then
                StrSQL.AppendLine(" AND UMA_Configurazione_Allevamenti.Validita_Inizio <=  " & Agro_SQL_SaveDate(CDate(FineValidita), False))
            End If

            If Not String.IsNullOrEmpty(InizioValidita) Then
                StrSQL.AppendLine(" AND UMA_Configurazione_Allevamenti.Validita_Fine >=  " & Agro_SQL_SaveDate(CDate(InizioValidita), False))
            End If

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

    Public Function VerificaPeriodiSovrappostiDB(righe As List(Of UMAConfigurazioneAllevamentiDto),
                                                righeCancellate As List(Of UMAConfigurazioneAllevamentiDto),
                                                 efConnString As String,
                                                objParametri As AgronicaCoreParametri) As String
        Dim Errori As String = ""
        Dim Soprapposto As Boolean = False


        For Each S In righe
            Dim Lavorazione = From R In righe
                              Where (S.UMA_All_Cod = R.UMA_All_Cod _
                              AndAlso S.Regione_Cod = R.Regione_Cod) _
                                AndAlso ((S.Validita_Inizio >= R.Validita_Inizio AndAlso S.Validita_Inizio <= R.Validita_Fine) _
                                      OrElse
                                      (S.Validita_Fine >= R.Validita_Inizio AndAlso S.Validita_Fine <= R.Validita_Fine) _
                                      OrElse
                                      (S.Validita_Inizio <= R.Validita_Inizio AndAlso S.Validita_Fine >= R.Validita_Fine))
                              Select R

            If Lavorazione.Count() > 1 Then
                Dim sov = From R In Lavorazione
                          Where R.Validita_Inizio <> S.Validita_Inizio OrElse R.Validita_Fine <> S.Validita_Fine OrElse R.ID <> S.ID
                Dim valInizDaControllare As Date = S.Validita_Inizio
                Dim valFineDaControllare As Date = S.Validita_Fine
                If sov.Count() = 0 Then
                    Errori = $"L'elemento {S.UMA_All_Des} nel periodo dal {valInizDaControllare.ToString("dd/MM/yyyy")} al {valFineDaControllare.ToString("dd/MM/yyyy")} si sovrappone ad uno dei periodi in corso di modifica"
                Else
                    Dim valInizSovrapposto As Date = sov.FirstOrDefault().Validita_Inizio
                    Dim valFineSovrapposto As Date = sov.FirstOrDefault().Validita_Fine
                    Errori = $"L'elemento {S.UMA_All_Des} nel periodo dal {valInizDaControllare.ToString("dd/MM/yyyy")} al {valFineDaControllare.ToString("dd/MM/yyyy")} si sovrappone al periodo dal {valInizSovrapposto.ToString("dd/MM/yyyy")} al {valFineSovrapposto.ToString("dd/MM/yyyy")}"
                End If
                Return Errori
            End If

        Next

        Dim StrNotin As String = ""
        For Each C In righeCancellate
            If StrNotin <> "" Then
                StrNotin += ","
            End If
            StrNotin += C.ID.ToString()
        Next
        For Each C In righe
            If C.ID > 0 Then
                If StrNotin <> "" Then
                    StrNotin += ","
                End If
                StrNotin += C.ID.ToString()
            End If
        Next
        Return Leggi_Esistente(objParametri, righe, StrNotin)





    End Function
    Public Function Leggi_Esistente(ByRef objParametri As AgronicaCoreParametri, Righe As List(Of UMAConfigurazioneAllevamentiDto), strNotIn As String) As String
        'Dim FlagConnessioneLocale As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Configurazione_MacrousixLavorazioni.AgronicaCoreDataProvider_Leggi_Esistente()"
        Dim Errori As String = ""
        ' ------- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Utility.VerificaApriConnessione(objParametri, False)

            For Each RigadaControllare In Righe


                Dim StrSQL As New System.Text.StringBuilder
                StrSQL.Length = 0

                StrSQL.Append("SELECT * 
                           FROM UMA_Configurazione_Allevamenti ")
                StrSQL.Append(" where ")
                StrSQL.Append($" (uma_all_cod = '{Agro_SQL_SaveText(RigadaControllare.UMA_All_Cod)}' and Regione_Cod = '{Agro_SQL_SaveText(RigadaControllare.Regione_Cod)}' ")
                If String.IsNullOrEmpty(strNotIn) = False Then StrSQL.Append($" And id not in ({strNotIn}) ")
                StrSQL.Append($" ) and ((  {Agro_SQL_SaveDate(RigadaControllare.Validita_Inizio)} >= Validita_Inizio and {Agro_SQL_SaveDate(RigadaControllare.Validita_Inizio)} <= Validita_Fine ) ")
                StrSQL.Append($" or (  {Agro_SQL_SaveDate(RigadaControllare.Validita_Fine)}  >= Validita_Inizio And {Agro_SQL_SaveDate(RigadaControllare.Validita_Fine)} <= Validita_Fine )")
                StrSQL.Append($" or ( {Agro_SQL_SaveDate(RigadaControllare.Validita_Inizio)} <= Validita_Inizio And {Agro_SQL_SaveDate(RigadaControllare.Validita_Fine)} >= Validita_Fine ))")

                '--------------------------------------------------------------------------
                DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
                '--------------------------------------------------------------------------
                If DT.Rows.Count > 0 Then
                    Dim valInizDaControllare As Date = RigadaControllare.Validita_Inizio
                    Dim valFineDaControllare As Date = RigadaControllare.Validita_Fine
                    Dim valInizSovrapposto As Date = DT.Rows(0)("Validita_Inizio")
                    Dim valFineSovrapposto As Date = DT.Rows(0)("Validita_Fine")
                    Errori = $"L'elemento {RigadaControllare.UMA_All_Des} nel periodo dal {valInizDaControllare.ToString("dd/MM/yyyy")} al {valFineDaControllare.ToString("dd/MM/yyyy")} si sovrappone al periodo dal {valInizSovrapposto.ToString("dd/MM/yyyy")} al {valFineSovrapposto.ToString("dd/MM/yyyy")}"
                    Return Errori
                End If
            Next


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Errori
    End Function
End Class

Public Class UMAConfigurazioneAllevamenti_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function AggiungiNuovi(scdoc As List(Of UMAConfigurazioneAllevamentiDto),
                                  ByRef context As Gias_DeveloperServer_Entities,
                                  objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMAConfigurazioneAllevamenti.AggiungiNouvi()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            For Each sd In scdoc

                StrSQL = New System.Text.StringBuilder
                StrSQL.Append(" INSERT INTO [UMA_Configurazione_Allevamenti] ")
                StrSQL.Append("          ([Regione_Cod]                                                       ")
                StrSQL.Append("          ,[UMA_All_Cod]                                                       ")
                StrSQL.Append("          ,[Tipo_Operazione]                                                   ")
                StrSQL.Append("          ,[Gasolio_Lt]                                                        ")
                StrSQL.Append("          ,[Benzina_Lt]                                                        ")
                StrSQL.Append("          ,[Qta_Aggiuntiva_Carro]                                              ")
                StrSQL.Append("          ,[N_Max_Allevamenti]                                                 ")
                StrSQL.Append("          ,[inviato]                                                           ")
                StrSQL.Append("          ,[datainvio]                                                         ")
                StrSQL.Append("          ,[Data_Creazione]                                                    ")
                StrSQL.Append("          ,[Data_Modifica]                                                     ")
                StrSQL.Append("          ,[Username_Creazione]                                                ")
                StrSQL.Append("          ,[Username_Modifica]                                                 ")
                StrSQL.Append("          ,[Validita_Inizio]                                                   ")
                StrSQL.Append("          ,[Validita_Fine]                                                     ")
                StrSQL.Append("          ,[ufl_min]                                                           ")
                StrSQL.Append("          ,[ufl_max]                                                           ")
                StrSQL.Append("          ,[ufc_min]                                                           ")
                StrSQL.Append("          ,[ufc_max])                                                          ")
                StrSQL.Append("    VALUES (")
                StrSQL.Append($"      '{Agro_SQL_SaveText(sd.Regione_Cod)}'")
                StrSQL.Append($"     ,'{Agro_SQL_SaveText(sd.UMA_All_Cod)}'")
                StrSQL.Append($"     , {Agro_SQL_SaveNum(sd.Tipo_Operazione)}")
                StrSQL.Append($"     , {Agro_SQL_SaveNum(sd.Gasolio_Lt)}")
                StrSQL.Append($"     , {Agro_SQL_SaveNum(sd.Benzina_Lt)}")
                StrSQL.Append($"     , {Agro_SQL_SaveNum(sd.Qta_Aggiuntiva_Carro)}")
                StrSQL.Append($"     , {Agro_SQL_SaveNum(sd.N_Max_Allevamenti)}")
                StrSQL.Append($"     , {Agro_SQL_SaveNum(sd.inviato)}")
                StrSQL.Append($"     ,{Agro_SQL_SaveDate(sd.datainvio)}")
                StrSQL.Append($"     ,{Agro_SQL_SaveDate(sd.Data_Creazione)}")
                StrSQL.Append($"     ,{Agro_SQL_SaveDate(sd.Data_Modifica)}")
                StrSQL.Append($"     ,'{Agro_SQL_SaveText(sd.Username_Creazione)}'")
                StrSQL.Append($"     ,'{Agro_SQL_SaveText(sd.Username_Modifica)}'")
                StrSQL.Append($"     ,{Agro_SQL_SaveDate(sd.Validita_Inizio)}")
                StrSQL.Append($"     ,{Agro_SQL_SaveDate(sd.Validita_Fine)} ")
                StrSQL.Append($"     , {Agro_SQL_SaveNum(sd.ufl_min)}")
                StrSQL.Append($"     , {Agro_SQL_SaveNum(sd.ufl_max)}")
                StrSQL.Append($"     , {Agro_SQL_SaveNum(sd.ufc_min)}")
                StrSQL.Append($"     , {Agro_SQL_SaveNum(sd.ufc_max)}) ")

                xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)

            Next

            'Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            '    ' Contiene anche i dettagli

            '    For Each sd In scdoc
            '        context.UMA_Configurazione_Allevamenti.Add(sd.ToUMAConfigurazioneAllevamentiDB())
            '    Next

            '    GiasContext.SaveChanges()
            '    'Gias Context.SaveChanges() è come se fosse una transazione se c'è un errore,
            '    'nelle righe inserite,cancellate o modificate viene annullata tutta la scrittura
            'End Using
            ''For Each sd In scdoc
            ''    context.UMA_Configurazione_Allevamenti.Add(sd.ToUMAConfigurazioneAllevamentiDB())
            ''Next
            ''context.SaveChanges()
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "]  " & MessaggioErrore)
        End Try

        Return True
    End Function

    Public Function Rimuovi(lista As List(Of UMAConfigurazioneAllevamentiDto), context As Gias_DeveloperServer_Entities, objParametri As AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMAConfigurazioneAllevamenti.Rimuovi()"

        ' ------------- Variabili -------------
        Dim DT As DataTable
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            For Each l In lista


                StrSQL = New System.Text.StringBuilder

                StrSQL.Append("DELETE FROM UMA_Configurazione_Allevamenti where ID =  " & Agro_SQL_SaveNum(l.ID) & " ")


                xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)

                'Dim record As UMA_Configurazione_Allevamenti = context.UMA_Configurazione_Allevamenti.Where(Function(s) s.UMA_All_Cod = elem.UMA_All_Cod).FirstOrDefault()

                'If Not record Is Nothing Then
                '    context.UMA_Configurazione_Allevamenti.Remove(record)
                'End If
            Next
            'context.SaveChanges()

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "]  " & MessaggioErrore)
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
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            For Each sd In righeModificateArr

                StrSQL = New System.Text.StringBuilder
                StrSQL.Append(" Update [UMA_Configurazione_Allevamenti] set ")
                StrSQL.Append($"     [Regione_Cod]               =       '{Agro_SQL_SaveText(sd.Regione_Cod)}'")
                StrSQL.Append($"     ,[UMA_All_Cod]              =       '{Agro_SQL_SaveText(sd.UMA_All_Cod)}'")
                StrSQL.Append($"     ,[Tipo_Operazione]          =        {Agro_SQL_SaveNum(sd.Tipo_Operazione)}")
                StrSQL.Append($"     ,[Gasolio_Lt]               =        {Agro_SQL_SaveNum(sd.Gasolio_Lt)}")
                StrSQL.Append($"     ,[Benzina_Lt]               =        {Agro_SQL_SaveNum(sd.Benzina_Lt)}")
                StrSQL.Append($"     ,[Qta_Aggiuntiva_Carro]     =        {Agro_SQL_SaveNum(sd.Qta_Aggiuntiva_Carro)}")
                StrSQL.Append($"     ,[N_Max_Allevamenti]        =        {Agro_SQL_SaveNum(sd.N_Max_Allevamenti)}")
                StrSQL.Append($"     ,[inviato]                  =        {Agro_SQL_SaveNum(sd.inviato)}")
                StrSQL.Append($"     ,[datainvio]                =       {Agro_SQL_SaveDate(sd.datainvio)}")
                StrSQL.Append($"     ,[Data_Modifica]            =       {Agro_SQL_SaveDate(sd.Data_Modifica)}")
                StrSQL.Append($"     ,[Username_Modifica]        =       '{Agro_SQL_SaveText(sd.Username_Modifica)}'")
                StrSQL.Append($"     ,[Validita_Inizio]          =       {Agro_SQL_SaveDate(sd.Validita_Inizio)}")
                StrSQL.Append($"     ,[Validita_Fine]            =       {Agro_SQL_SaveDate(sd.Validita_Fine)}")
                StrSQL.Append($"     ,[ufl_min]            =       {Agro_SQL_SaveNum(sd.ufl_min)}")
                StrSQL.Append($"     ,[ufl_max]            =       {Agro_SQL_SaveNum(sd.ufl_max)}")
                StrSQL.Append($"     ,[ufc_min]            =       {Agro_SQL_SaveNum(sd.ufc_min)}")
                StrSQL.Append($"     ,[ufc_max]            =       {Agro_SQL_SaveNum(sd.ufc_max)}")
                StrSQL.Append(" where ID =  " & Agro_SQL_SaveNum(sd.ID) & " ")
                xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)



                'Dim result As UMA_Configurazione_Allevamenti = context.UMA_Configurazione_Allevamenti.FirstOrDefault(Function(s) s.UMA_All_Cod = elem.UMA_All_Cod)

                'If Not result Is Nothing Then
                '    result.Regione_Cod = elem.Regione_Cod
                '    result.UMA_All_Cod = elem.UMA_All_Cod
                '    result.Tipo_Operazione = elem.Tipo_Operazione
                '    result.Gasolio_Lt = elem.Gasolio_Lt
                '    result.Benzina_Lt = elem.Benzina_Lt
                '    result.Qta_Aggiuntiva_Carro = elem.Qta_Aggiuntiva_Carro
                '    result.N_Max_Allevamenti = elem.N_Max_Allevamenti
                '    result.inviato = elem.inviato
                '    result.datainvio = elem.datainvio
                '    result.Data_Creazione = elem.Data_Creazione
                '    result.Data_Modifica = elem.Data_Modifica
                '    result.Username_Creazione = elem.Username_Creazione
                '    result.Username_Modifica = elem.Username_Modifica
                '    result.Validita_Inizio = elem.Validita_Inizio
                '    result.Validita_Fine = elem.Validita_Fine
                'End If

            Next
            'context.SaveChanges()
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
        Public ufl_min As Decimal?
        Public ufl_max As Decimal?
        Public ufc_min As Decimal?
        Public ufc_max As Decimal?
        Public ID As Integer

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

