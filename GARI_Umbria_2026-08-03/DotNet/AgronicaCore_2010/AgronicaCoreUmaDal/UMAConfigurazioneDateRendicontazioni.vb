Imports System.Runtime.CompilerServices
Imports AgronicaCoreUmaDal.UMAConfigurazioneDateRendicontazioni_W
Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Public Class UMAConfigurazioneDateRendicontazioni_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiDateRendicontazioni(ByRef objParametri As AgronicaCoreParametri,
                                             Optional anno As Integer = 0,
                                             Optional xfiltroAggiuntivo As String = "",
                                             Optional xOrderBy As String = "") As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreUMADAL.UMAConfigurazioneDateRendicontazioni.LeggiDateRendicontazioni()"

        ' ------- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Dim StrSQL As New System.Text.StringBuilder
            StrSQL.Length = 0


            StrSQL.AppendLine("SELECT Tipo_Azienda, Anno_Richiesta, Data_Inizio_Rendicontazione, Data_Limite_INS_Azienda_Terzista, Data_Fine_Rendicontazione, Termine_Ultimo_Rendicontazione, Data_Limite_INS_Richiesta_Anticipo, Data_Fine_Blocco_Rendic_Conto_Proprio")
            'StrSQL.AppendLine(", Inviato, DataInvio, Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine, Data_Limite_INS_Richiesta_Anticipo")
            StrSQL.AppendLine("FROM UMA_Setup_Date_Rendicontazione")

            StrSQL.AppendLine(" WHERE 1=1 ")

            If anno > 0 Then
                StrSQL.AppendLine(" AND Anno_Richiesta = " + Agro_SQL_SaveNum(anno))
            End If

            If xfiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xfiltroAggiuntivo))
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & xOrderBy)
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

    Public Function VerificaElementoNonEsisteInDB(setup As UMASetupDateRendicontazioneDto,
                                                  efConnString As String) As Boolean
        Using dal As New Gias_DeveloperServer_Entities(efConnString)
            Dim exists = dal.UMA_Setup_Date_Rendicontazione.Any(Function(s) s.Tipo_Azienda = setup.Tipo_Azienda AndAlso
                                               s.Anno_Richiesta = setup.Anno_Richiesta AndAlso
                                               s.Data_Inizio_Rendicontazione = setup.Data_Inizio_Rendicontazione AndAlso
                                               s.Data_Limite_INS_Azienda_Terzista = setup.Data_Limite_INS_Azienda_Terzista AndAlso
                                               s.Data_Fine_Rendicontazione = setup.Data_Fine_Rendicontazione AndAlso
                                               s.Termine_Ultimo_Rendicontazione = setup.Termine_Ultimo_Rendicontazione)
            Return exists
        End Using
    End Function

End Class

Public Class UMAConfigurazioneDateRendicontazioni_W

    Inherits AgronicaCoreDataProvider.DataProvider

    ''' <summary>
    ''' Aggiungi ogni riga inserita dal utente nel database.
    ''' </summary>
    ''' <returns></returns>
    Public Function AggiungiNuovi(listLav As List(Of UMASetupDateRendicontazioneDto),
                                  context As Gias_DeveloperServer_Entities,
                                  objParametri As AgronicaCoreParametri) As Boolean
        Try
            For Each l In listLav
                context.UMA_Setup_Date_Rendicontazione.Add(l.ToSetupDateRendicontazionePoco())
            Next
            context.SaveChanges()
        Catch ex As Exception
            Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMAConfigurazioneDateRendicontazioni_W.AggiungiNuovi()"
            Dim MessaggioErrore As String

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return True
    End Function

    Public Function Rimuovi(lista As List(Of UMASetupDateRendicontazioneDto), context As Gias_DeveloperServer_Entities, objParametri As AgronicaCoreParametri) As Boolean

        ' ------------- Variabili -------------
        Dim DT As DataTable
        Dim MessaggioErrore As String = ""

        Try
            For Each elem In lista
                Dim record As UMA_Setup_Date_Rendicontazione = context.UMA_Setup_Date_Rendicontazione.Where(Function(s) s.Anno_Richiesta = elem.Anno_Richiesta And s.Tipo_Azienda = elem.Tipo_Azienda).FirstOrDefault()

                If Not record Is Nothing Then
                    context.UMA_Setup_Date_Rendicontazione.Remove(record)
                End If
            Next
            context.SaveChanges()
        Catch ex As Exception
            Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMAConfigurazioneDateRendicontazioni_W.Rimuovi()"

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return True
    End Function

    Public Function Aggiorna(righeModificateArr As List(Of UMASetupDateRendicontazioneDto),
                             context As Gias_DeveloperServer_Entities,
                             ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim DT As DataTable

        Try

            For Each elem In righeModificateArr
                'Dim result As UMA_Setup = context.UMA_Setup.FirstOrDefault(Function(s) s.Anno = elem.Anno)
                Dim result As UMA_Setup_Date_Rendicontazione = (From a In context.UMA_Setup_Date_Rendicontazione Where a.Anno_Richiesta = elem.Anno_Richiesta And a.Tipo_Azienda = elem.Tipo_Azienda).FirstOrDefault()

                If Not result Is Nothing Then
                    result.Data_Inizio_Rendicontazione = elem.Data_Inizio_Rendicontazione
                    result.Data_Limite_INS_Azienda_Terzista = elem.Data_Limite_INS_Azienda_Terzista
                    result.Data_Fine_Rendicontazione = elem.Data_Fine_Rendicontazione
                    result.Termine_Ultimo_Rendicontazione = elem.Termine_Ultimo_Rendicontazione
                    result.Inviato = elem.Inviato
                    result.DataInvio = elem.DataInvio
                    result.Data_Modifica = elem.Data_Modifica
                    result.Username_Modifica = elem.Username_Modifica
                    result.Validita_Inizio = elem.Validita_Inizio
                    result.Validita_Fine = elem.Validita_Fine
                    result.Data_Limite_INS_Richiesta_Anticipo = elem.Data_Limite_INS_Richiesta_Anticipo
                    result.Data_Fine_Blocco_Rendic_Conto_Proprio = elem.Data_Fine_Blocco_Rendic_Conto_Proprio
                End If
            Next
            context.SaveChanges()
        Catch ex As Exception
            Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMAConfigurazioneDateRendicontazioni_W.Aggiorna()"
            Dim MessaggioErrore As String = ""

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return True
    End Function




    Class UMASetupDateRendicontazioneDto
        ' *************************** Collone chiavi primarie ***************************
        Public Tipo_Azienda As Integer
        Public Anno_Richiesta As Integer

        ' ************************* Collone della tabella UMA_Setup_Date_Rendicontazione *************************
        Public Data_Inizio_Rendicontazione As Date
        Public Data_Limite_INS_Azienda_Terzista As Date?
        Public Data_Fine_Rendicontazione As Date
        Public Termine_Ultimo_Rendicontazione As Date

        Public Inviato As Short
        Public DataInvio As Date?
        Public Data_Creazione As Date?
        Public Data_Modifica As Date?
        Public Username_Creazione As String
        Public Username_Modifica As String
        Public Validita_Inizio As Date?
        Public Validita_Fine As Date?
        Public Data_Limite_INS_Richiesta_Anticipo As Date?
        Public Data_Fine_Blocco_Rendic_Conto_Proprio As Date?
    End Class

End Class

Partial Module Extensions
    <Extension()>
    Function ToSetupDateRendicontazionePoco(ByVal l As UMAConfigurazioneDateRendicontazioni_W.UMASetupDateRendicontazioneDto) As AgronicaCoreEntityFramework_POCO.UMA_Setup_Date_Rendicontazione
        Dim r As New AgronicaCoreEntityFramework_POCO.UMA_Setup_Date_Rendicontazione With {
            .Tipo_Azienda = l.Tipo_Azienda,
            .Anno_Richiesta = l.Anno_Richiesta,
            .Data_Inizio_Rendicontazione = l.Data_Inizio_Rendicontazione,
            .Data_Limite_INS_Azienda_Terzista = l.Data_Limite_INS_Azienda_Terzista,
            .Data_Fine_Rendicontazione = l.Data_Fine_Rendicontazione,
            .Termine_Ultimo_Rendicontazione = l.Termine_Ultimo_Rendicontazione,
            .Inviato = l.Inviato,
            .DataInvio = l.DataInvio,
            .Data_Creazione = l.Data_Creazione,
            .Data_Modifica = l.Data_Modifica,
            .Username_Creazione = l.Username_Creazione,
            .Username_Modifica = l.Username_Modifica,
            .Validita_Fine = l.Validita_Fine,
            .Validita_Inizio = l.Validita_Inizio,
            .Data_Limite_INS_Richiesta_Anticipo = l.Data_Limite_INS_Richiesta_Anticipo,
            .Data_Fine_Blocco_Rendic_Conto_Proprio = l.Data_Fine_Blocco_Rendic_Conto_Proprio
        }

        Return r

    End Function
End Module




