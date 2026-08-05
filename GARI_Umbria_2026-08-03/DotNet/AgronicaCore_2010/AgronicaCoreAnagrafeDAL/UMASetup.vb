Imports System.Runtime.CompilerServices
Imports AgronicaCoreAnagrafeDAL.UMASetup_W
Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO



Public Class UMASetup_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiSetup(ByVal Anno As Integer,
                               ByRef objParametri As AgronicaCoreParametri) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMASetup.LeggiSetup()"

        ' ------- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Dim StrSQL As New System.Text.StringBuilder
            StrSQL.Length = 0

            StrSQL.Append("SELECT Anno, Per_Riduzione, Per_Mag_Terreno_B, Per_Mag_Terreno_Medio, 
                                Per_Mag_Terreno_Tenace, Altre_Cfg, Nr_Litri_Maggiorazione, Percentuale_Integrazione_Terzista, Validita_Inizio, Validita_Fine, 
                                Inviato, DataInvio, Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica
                           FROM UMA_Setup")

            If Anno <> 0 Then
                StrSQL.AppendLine(" WHERE Anno = " + Agro_SQL_SaveNum(Anno))
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

    Public Function VerificaElementoNonEsisteInDB(setup As UMASetupDto,
                                                  efConnString As String) As Boolean
        Using dal As New Gias_DeveloperServer_Entities(efConnString)
            Dim exists = dal.UMA_Setup.Any(Function(s) s.Anno = setup.Anno AndAlso
                                               s.Per_Riduzione = setup.Per_Riduzione AndAlso
                                               s.Per_Mag_Terreno_B = setup.Per_Mag_Terreno_B AndAlso
                                               s.Per_Mag_Terreno_Medio = setup.Per_Mag_Terreno_Medio AndAlso
                                               s.Per_Mag_Terreno_Tenace = setup.Per_Mag_Terreno_Tenace AndAlso
                                               s.Altre_Cfg = setup.Altre_Cfg AndAlso
                                               s.Nr_Litri_Maggiorazione = setup.Nr_Litri_Maggiorazione AndAlso
                                               s.Percentuale_Integrazione_Terzista = setup.Percentuale_Integrazione_Terzista)
            Return exists
        End Using
    End Function
End Class
Public Class UMASetup_W
    Inherits AgronicaCoreDataProvider.DataProvider

    ''' <summary>
    ''' Aggiungi ogni riga inserita dal utente nel database.
    ''' </summary>
    ''' <returns></returns>
    Public Function AggiungiNuovi(listLav As List(Of UMASetupDto),
                                  context As Gias_DeveloperServer_Entities,
                                  objParametri As AgronicaCoreParametri) As Boolean
        Try
            For Each l In listLav
                context.UMA_Setup.Add(l.ToSetupPoco())
            Next
            context.SaveChanges()
        Catch ex As Exception
            Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMASetup_W.AggiungiNuovi()"
            Dim MessaggioErrore As String

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return True
    End Function

    Public Function Rimuovi(lista As List(Of UMASetupDto), context As Gias_DeveloperServer_Entities, objParametri As AgronicaCoreParametri) As Boolean

        ' ------------- Variabili -------------
        Dim DT As DataTable
        Dim MessaggioErrore As String = ""

        Try
            For Each elem In lista
                Dim record As UMA_Setup = context.UMA_Setup.Where(Function(s) s.Anno = elem.Anno).FirstOrDefault()

                If Not record Is Nothing Then
                    context.UMA_Setup.Remove(record)
                End If
            Next
            context.SaveChanges()
        Catch ex As Exception
            Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMASetup_W.Rimuovi()"

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return True
    End Function

    Public Function Aggiorna(righeModificateArr As List(Of UMASetupDto),
                             context As Gias_DeveloperServer_Entities,
                             ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim DT As DataTable

        Try

            For Each elem In righeModificateArr
                'Dim result As UMA_Setup = context.UMA_Setup.FirstOrDefault(Function(s) s.Anno = elem.Anno)
                Dim result As UMA_Setup = (From a In context.UMA_Setup Where a.Anno = elem.Anno).FirstOrDefault()

                If Not result Is Nothing Then
                    result.Per_Riduzione = elem.Per_Riduzione
                    result.Per_Mag_Terreno_B = elem.Per_Mag_Terreno_B
                    result.Per_Mag_Terreno_Medio = elem.Per_Mag_Terreno_Medio
                    result.Per_Mag_Terreno_Tenace = elem.Per_Mag_Terreno_Tenace
                    result.Altre_Cfg = elem.Altre_Cfg
                    result.Nr_Litri_Maggiorazione = elem.Nr_Litri_Maggiorazione
                    result.Percentuale_Integrazione_Terzista = elem.Percentuale_Integrazione_Terzista


                    result.Validita_Inizio = elem.Validita_Inizio
                    result.Validita_Fine = elem.Validita_Fine
                    result.inviato = elem.Inviato
                    result.datainvio = elem.DataInvio
                    result.Data_Modifica = elem.Data_Modifica
                    result.Username_Modifica = elem.Username_Modifica
                End If
            Next
            context.SaveChanges()
        Catch ex As Exception
            Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMASetup_W.Aggiorna()"
            Dim MessaggioErrore As String = ""

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return True
    End Function


    Class UMASetupDto
        ' *************************** Collone chiavi primarie ***************************
        Public Anno As Integer

        ' ************************* Collone della tabella UMA_Setup *************************
        Public Per_Riduzione As Double
        Public Per_Mag_Terreno_B As Double
        Public Per_Mag_Terreno_Medio As Double
        Public Per_Mag_Terreno_Tenace As Double
        Public Altre_Cfg As String
        Public Nr_Litri_Maggiorazione As Double
        Public Percentuale_Integrazione_Terzista As Double

        Public Validita_Inizio As Date?
        Public Validita_Fine As Date?
        Public Inviato As Short
        Public DataInvio As Date?
        Public Data_Creazione As Date?
        Public Data_Modifica As Date?
        Public Username_Creazione As String
        Public Username_Modifica As String
    End Class

End Class


Partial Module Extensions
    <Extension()>
    Function ToSetupPoco(ByVal l As UMASetup_W.UMASetupDto) As AgronicaCoreEntityFramework_POCO.UMA_Setup
        Dim r As New AgronicaCoreEntityFramework_POCO.UMA_Setup With {
            .datainvio = l.DataInvio,
            .Data_Creazione = l.Data_Creazione,
            .Data_Modifica = l.Data_Modifica,
            .Anno = l.Anno,
            .Per_Riduzione = l.Per_Riduzione,
            .inviato = l.Inviato,
            .Per_Mag_Terreno_B = l.Per_Mag_Terreno_B,
            .Per_Mag_Terreno_Medio = l.Per_Mag_Terreno_Medio,
            .Per_Mag_Terreno_Tenace = l.Per_Mag_Terreno_Tenace,
            .Altre_Cfg = l.Altre_Cfg,
            .Nr_Litri_Maggiorazione = l.Nr_Litri_Maggiorazione,
            .Percentuale_Integrazione_Terzista = l.Percentuale_Integrazione_Terzista,
            .Username_Creazione = l.Username_Creazione,
            .Username_Modifica = l.Username_Modifica,
            .Validita_Fine = l.Validita_Fine,
            .Validita_Inizio = l.Validita_Inizio
        }

        Return r

    End Function
End Module
