Imports System.Runtime.CompilerServices
Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreUmaDal.UMA_Configurazione_MacrousixLavorazioni_R
Imports AgronicaCoreUmaDal.UMALavorazioniAlternative_W

Public Class UMALAvorazioniAlternative_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Dropdown_LavUMA_Lav_UMA_Des(objparametri_Server As AgronicaCoreParametri) As List(Of VoceElencoDiDropdown)

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_.UMALavorazioniAlternative_W.Dropdown_Gruppo_Colturale_UMA()"

        Dim MessaggioErrore As String = ""
        Dim elenco As New List(Of VoceElencoDiDropdown)
        Dim stb As New System.Text.StringBuilder

        Try
            Dim gefutils As New Gias_EF_Utility
            Dim efConnString As String = gefutils.GetEntityConnectionString(objparametri_Server.StringaConnessione)

            Using db As New Gias_DeveloperServer_Entities(efConnString)
                Dim dbElem = db.UMA_Lavorazioni.ToList()
                elenco = dbElem.Select(Function(s) New VoceElencoDiDropdown(s.Lav_UMA_Cod, s.Lav_UMA_Des)).ToList()
            End Using
            Return elenco

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objparametri_Server, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
    End Function
    'Public Function Dropdown_LavUMAAlt_Lav_UMA_Des(objparametri_Server As AgronicaCoreParametri) As List(Of VoceElencoDiDropdown)

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_.UMALavorazioniAlternative_W.Dropdown_Gruppo_Colturale_UMA()"

    '    Dim MessaggioErrore As String = ""
    '    Dim elenco As New List(Of VoceElencoDiDropdown)
    '    Dim stb As New System.Text.StringBuilder

    '    Try
    '        Dim gefutils As New Gias_EF_Utility
    '        Dim efConnString As String = gefutils.GetEntityConnectionString(objparametri_Server.StringaConnessione)

    '        Using db As New Gias_DeveloperServer_Entities(efConnString)
    '            Dim dbElem = db.UMA_Lavorazioni.ToList()
    '            elenco = dbElem.Select(Function(s) New VoceElencoDiDropdown(s.Lav_UMA_Cod, s.Lav_UMA_Des)).ToList()
    '        End Using
    '        Return elenco

    '    Catch ex As Exception
    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objparametri_Server, NomeRoutine, MessaggioErrore)
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
    '    End Try
    'End Function
    Public Function Dropdown_Gruppo_Colturale_UMA(objparametri_Server As AgronicaCoreParametri) As List(Of VoceElencoDiDropdown)

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_.UMALavorazioniAlternative_W.Dropdown_Gruppo_Colturale_UMA()"

        Dim MessaggioErrore As String = ""
        Dim elenco As New List(Of VoceElencoDiDropdown)
        Dim stb As New System.Text.StringBuilder

        Try
            Dim gefutils As New Gias_EF_Utility
            Dim efConnString As String = gefutils.GetEntityConnectionString(objparametri_Server.StringaConnessione)

            Using db As New Gias_DeveloperServer_Entities(efConnString)
                Dim dbElem = db.UMA_Macrousi.ToList()
                elenco = dbElem.Select(Function(s) New VoceElencoDiDropdown(s.Macrouso_UMA_Cod, s.Macrouso_UMA_Des)).ToList()
            End Using
            Return elenco

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objparametri_Server, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
    End Function
    Public Function LeggiLavorazioniAlternative(ByRef objParametri As AgronicaCoreParametri, InizioValidita As String, FineValidita As String) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_Configurazione_MacrousixLavorazioni.LeggiLavorazioniAlternative()"

        ' ------- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Dim StrSQL As New System.Text.StringBuilder
            StrSQL.Length = 0

            StrSQL.Append("SELECT UMA_Macrousi.Macrouso_UMA_Des as 'Macrouso_UMA_Des', l1.Lav_UMA_Des as 'LavUMA_Lav_UMA_Des', l2.Lav_UMA_Des as 'LavUMAAlt_Lav_UMA_Des', UMA_Lavorazioni_Alternative.*
                           FROM UMA_Lavorazioni_Alternative
                           JOIN UMA_Macrousi ON UMA_Lavorazioni_Alternative.Gruppo_Colturale_UMA = UMA_Macrousi.Macrouso_UMA_Cod
                           JOIN UMA_Lavorazioni l1 ON UMA_Lavorazioni_Alternative.Lavorazione_UMA = l1.Lav_UMA_Cod
                           JOIN UMA_Lavorazioni l2 ON UMA_Lavorazioni_Alternative.Lavorazione_UMA_Alt = l2.Lav_UMA_Cod")

            StrSQL.AppendLine(" WHERE 1 = 1  ")

            If Not String.IsNullOrEmpty(FineValidita) Then
                StrSQL.AppendLine(" AND UMA_Lavorazioni_Alternative.Validita_Inizio <=  " & Agro_SQL_SaveDate(CDate(FineValidita), False))
            End If

            If Not String.IsNullOrEmpty(InizioValidita) Then
                StrSQL.AppendLine(" AND UMA_Lavorazioni_Alternative.Validita_Fine >=  " & Agro_SQL_SaveDate(CDate(InizioValidita), False))
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
    Public Function VerificaElementoNonEsisteInDB(ByRef objParametri As AgronicaCoreParametri, lav As UMALavorazioniAlteranativeDto) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_Configurazione_MacrousixLavorazioni.VerificaElementoNonEsisteInDB()"

        ' ------- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Dim StrSQL As New System.Text.StringBuilder
            StrSQL.Length = 0

            StrSQL.Append("SELECT * FROM UMA_Lavorazioni_Alternative ")

            StrSQL.Append(" where ")
            StrSQL.Append($" Gruppo_Colturale_UMA =  '{Agro_SQL_SaveText(lav.Gruppo_Colturale_UMA)}'")
            StrSQL.Append($" and Lavorazione_UMA =  '{Agro_SQL_SaveText(lav.Lavorazione_UMA)}'")
            StrSQL.Append($" and Lavorazione_UMA_Alt =  '{Agro_SQL_SaveText(lav.Lavorazione_UMA_Alt)}'")
            If lav.Regolamento_Cod <> 0 Then
                StrSQL.Append($" and Regolamento_Cod in (0,{Agro_SQL_SaveNum(lav.Regolamento_Cod)})")
            Else
                StrSQL.Append($" and Regolamento_Cod in (0,1,4)")
            End If



            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
            If DT.Rows.Count > 0 Then
                Return True
            Else
                Return False
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
    End Function
    'Public Function VerificaElementoNonEsisteInDB(lav As UMALavorazioniAlteranativeDto,
    '                                              efConnString As String) As Boolean
    '    Using dal As New Gias_DeveloperServer_Entities(efConnString)
    '        Dim exists = dal.UMA_Lavorazioni_Alternative.Any(Function(s) s.Gruppo_Colturale_UMA = lav.Gruppo_Colturale_UMA AndAlso s.Lavorazione_UMA = lav.Lavorazione_UMA AndAlso s.Lavorazione_UMA_Alt = lav.Lavorazione_UMA_Alt)
    '        Return exists
    '    End Using
    'End Function
End Class
Public Class UMALavorazioniAlternative_W
    Inherits AgronicaCoreDataProvider.DataProvider

    ''' <summary>
    ''' Aggiungi ogni riga inserita dal utente nel database.
    ''' </summary>
    ''' <returns></returns>
    Public Function AggiungiNouvi(listLav As List(Of UMALavorazioniAlteranativeDto),
                                  context As Gias_DeveloperServer_Entities,
                                  objParametri As AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMALavorazioniAlternative_W.AggiungiNouvi()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False




        Try
            For Each l In listLav
                ' context.UMA_Lavorazioni_Alternative.Add(l.ToLavorazioneAlternativaPoco())
                StrSQL = New System.Text.StringBuilder

                StrSQL.Append("INSERT INTO [UMA_Lavorazioni_Alternative] ( ")
                StrSQL.Append("       [Gruppo_Colturale_UMA]    ")
                StrSQL.Append("      ,[Lavorazione_UMA]         ")
                StrSQL.Append("      ,[Lavorazione_UMA_Alt]     ")
                StrSQL.Append("      ,[Validita_Inizio]         ")
                StrSQL.Append("      ,[Validita_Fine]           ")
                StrSQL.Append("      ,[Inviato]                 ")
                StrSQL.Append("      ,[DataInvio]               ")
                StrSQL.Append("      ,[Data_Creazione]          ")
                StrSQL.Append("      ,[Data_Modifica]           ")
                StrSQL.Append("      ,[Username_Creazione]      ")
                StrSQL.Append("      ,[Username_Modifica]       ")
                StrSQL.Append("      ,[Regolamento_Cod])        ")
                StrSQL.Append("VALUES  (                        ")
                StrSQL.Append($"   '{Agro_SQL_SaveText(l.Gruppo_Colturale_UMA)}' ")
                StrSQL.Append($"  ,'{Agro_SQL_SaveText(l.Lavorazione_UMA)}'")
                StrSQL.Append($"  ,'{Agro_SQL_SaveText(l.Lavorazione_UMA_Alt)}' ")
                StrSQL.Append($"  ,{Agro_SQL_SaveDate(l.Validita_Inizio)} ")
                StrSQL.Append($"  ,{Agro_SQL_SaveDate(l.Validita_Fine)} ")
                StrSQL.Append($"  ,{Agro_SQL_SaveNum(l.Inviato)} ")
                StrSQL.Append($"  ,{Agro_SQL_SaveDate(l.DataInvio)} ")
                StrSQL.Append($"  ,{Agro_SQL_SaveDate(l.Data_Creazione)} ")
                StrSQL.Append($"  ,{Agro_SQL_SaveDate(l.Data_Modifica)} ")
                StrSQL.Append($"  ,'{Agro_SQL_SaveText(l.Username_Creazione)}' ")
                StrSQL.Append($"  ,'{Agro_SQL_SaveText(l.Username_Modifica)}' ")
                StrSQL.Append($"  ,{Agro_SQL_SaveNum(l.Regolamento_Cod)} ")
                StrSQL.Append($" )")

                xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            Next
            'context.SaveChanges()
        Catch ex As Exception


            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return True
    End Function

    Public Function Rimuovi(lista As List(Of UMALavorazioniAlteranativeDto), context As Gias_DeveloperServer_Entities, objParametri As AgronicaCoreParametri) As Boolean

        ' ------------- Variabili -------------
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMALavorazioniAlternative_W.Rimuovi()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False


        Try
            For Each L In lista


                StrSQL = New System.Text.StringBuilder

                StrSQL.Append("DELETE from  [UMA_Lavorazioni_Alternative] ")
                StrSQL.Append($" where    [Gruppo_Colturale_UMA]  =  '{Agro_SQL_SaveText(L.Gruppo_Colturale_UMA)}' ")
                StrSQL.Append($"    and [Lavorazione_UMA]        =  '{Agro_SQL_SaveText(L.Lavorazione_UMA)}'")
                StrSQL.Append($"    and [Lavorazione_UMA_Alt]     =  '{Agro_SQL_SaveText(L.Lavorazione_UMA_Alt)}' ")
                StrSQL.Append($"    and [Regolamento_Cod]       =  {Agro_SQL_SaveNum(L.Regolamento_Cod)} ")


                xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)

                'Dim record As UMA_Lavorazioni_Alternative = context.UMA_Lavorazioni_Alternative.Where(Function(s) s.Gruppo_Colturale_UMA = elem.Gruppo_Colturale_UMA AndAlso s.Lavorazione_UMA = elem.Lavorazione_UMA AndAlso s.Lavorazione_UMA_Alt = elem.Lavorazione_UMA_Alt).FirstOrDefault()

                'If Not record Is Nothing Then
                '    context.UMA_Lavorazioni_Alternative.Remove(record)
                'End If
            Next
            context.SaveChanges()
        Catch ex As Exception


            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return True
    End Function

    Public Function Aggiorna(righeModificateArr As List(Of UMALavorazioniAlteranativeDto),
                             context As Gias_DeveloperServer_Entities,
                             ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMALavorazioniAlternative_W.Aggiorna()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            For Each l In righeModificateArr

                StrSQL = New System.Text.StringBuilder

                StrSQL.Append("UPDATE [UMA_Lavorazioni_Alternative] set ")
                StrSQL.Append($"     [Validita_Inizio]        =  {Agro_SQL_SaveDate(l.Validita_Inizio)} ")
                StrSQL.Append($"    ,[Validita_Fine]           =  {Agro_SQL_SaveDate(l.Validita_Fine)} ")
                StrSQL.Append($"    , [Inviato]                =  {Agro_SQL_SaveNum(l.Inviato)} ")
                StrSQL.Append($"    ,[DataInvio]               =  {Agro_SQL_SaveDate(l.DataInvio)} ")
                StrSQL.Append($"    ,[Data_Modifica]           =  {Agro_SQL_SaveDate(l.Data_Modifica)} ")
                StrSQL.Append($"    ,[Username_Modifica]       =  '{Agro_SQL_SaveText(l.Username_Modifica)}' ")
                StrSQL.Append($" where    [Gruppo_Colturale_UMA]  =  '{Agro_SQL_SaveText(l.Gruppo_Colturale_UMA)}' ")
                StrSQL.Append($"    and [Lavorazione_UMA]        =  '{Agro_SQL_SaveText(l.Lavorazione_UMA)}'")
                StrSQL.Append($"    and [Lavorazione_UMA_Alt]     =  '{Agro_SQL_SaveText(l.Lavorazione_UMA_Alt)}' ")
                StrSQL.Append($"    and [Regolamento_Cod]       =  {Agro_SQL_SaveNum(l.Regolamento_Cod)} ")


                xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)




            Next
            'context.SaveChanges()
        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return True
    End Function


    Class UMALavorazioniAlteranativeDto
        ' *************************** Collone prese in join con altre tabelle ***************************
        Public Macrouso_UMA_Des As String
        Public LavUMA_Lav_UMA_Des As String
        Public LavUMAAlt_Lav_UMA_Des As String
        ' *************************** Collone chiavi primarie ***************************
        Public Gruppo_Colturale_UMA As String
        Public Lavorazione_UMA As String
        Public Lavorazione_UMA_Alt As String

        ' ************************* Collone della tabella UMA_Lavorazioni_Alternative *************************
        Public Validita_Inizio As Date?
        Public Validita_Fine As Date?
        Public Inviato As Short
        Public DataInvio As Date?
        Public Data_Creazione As Date?
        Public Data_Modifica As Date?
        Public Username_Creazione As String
        Public Username_Modifica As String
        Public Regolamento_Cod As String
    End Class

End Class


Partial Module Extensions
    <Extension()>
    Function ToLavorazioneAlternativaPoco(ByVal l As UMALavorazioniAlternative_W.UMALavorazioniAlteranativeDto) As AgronicaCoreEntityFramework_POCO.UMA_Lavorazioni_Alternative
        Dim r As New AgronicaCoreEntityFramework_POCO.UMA_Lavorazioni_Alternative With {
            .DataInvio = l.DataInvio,
            .Data_Creazione = l.Data_Creazione,
            .Data_Modifica = l.Data_Modifica,
            .Gruppo_Colturale_UMA = l.Gruppo_Colturale_UMA,
            .Inviato = l.Inviato,
            .Lavorazione_UMA = l.Lavorazione_UMA,
            .Lavorazione_UMA_Alt = l.Lavorazione_UMA_Alt,
            .Username_Creazione = l.Username_Creazione,
            .Username_Modifica = l.Username_Modifica,
            .Validita_Fine = l.Validita_Fine,
            .Validita_Inizio = l.Validita_Inizio
        }

        Return r
    End Function
End Module