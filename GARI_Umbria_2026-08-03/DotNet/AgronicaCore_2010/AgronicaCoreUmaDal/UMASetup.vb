Imports System.Runtime.CompilerServices
Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreUmaDal.UMASetup_W

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
                                Inviato, DataInvio, Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, Percentuale_Richieste_Anticipo,
                                CASE WHEN Altre_Cfg > 0 THEN 'Abilitato' Else 'Disabilitato' END AS 'Vincola_Rendicontazione_e_Richiesta'
                                ,Gestione_Biologico,Gestione_Rimanenze,Gestione_Anticipazioni_Colturali, Macchine_Targa_Obbligatoria
                                ,CASE WHEN Gestione_Biologico > 0 THEN 'Abilitato' Else 'Disabilitato' END AS 'Gestione_BiologicoDes'
                                ,CASE WHEN Gestione_Rimanenze = 1 THEN 'Abilitato' WHEN Gestione_Rimanenze = 2 THEN 'Abilitato Solo Rendicontazioni' Else 'Disabilitato' END AS 'Gestione_RimanenzeDes'
                                ,Tipologia_Report_Elas , Tipologia_Elenco_Inadempienti, Tipologia_Report_SegnalazioneAccise
                                ,stati_invio_mail_avanz_pratica, gruppi_utenti_invio_mail_avanz_pratica
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
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMASetup.AggiungiNouvi()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False


        Try
            For Each l In listLav
                'context.UMA_Setup.Add(l.ToSetupPoco())

                StrSQL = New System.Text.StringBuilder
                StrSQL.Append(" INSERT INTO [dbo].[UMA_Setup] ")
                StrSQL.Append("          ([Anno]                                                           ")
                StrSQL.Append("          ,[Per_Riduzione]                                                                  ")
                StrSQL.Append("          ,[Per_Mag_Terreno_B]                                                              ")
                StrSQL.Append("          ,[Per_Mag_Terreno_Medio]                                                          ")
                StrSQL.Append("          ,[Per_Mag_Terreno_Tenace]                                                         ")
                StrSQL.Append("          ,[Altre_Cfg]                                                                      ")
                StrSQL.Append("          ,[inviato]                                                                        ")
                StrSQL.Append("          ,[datainvio]                                                                      ")
                StrSQL.Append("          ,[Data_Creazione]                                                                 ")
                StrSQL.Append("          ,[Data_Modifica]                                                                  ")
                StrSQL.Append("          ,[Username_Creazione]                                                             ")
                StrSQL.Append("          ,[Username_Modifica]                                                              ")
                StrSQL.Append("          ,[Validita_Inizio]                                                                ")
                StrSQL.Append("          ,[Validita_Fine]                                                                  ")
                StrSQL.Append("          ,[Nr_Litri_Maggiorazione]                                                         ")
                StrSQL.Append("          ,[Percentuale_Integrazione_Terzista]                                              ")
                StrSQL.Append("          ,[Percentuale_Richieste_Anticipo]                                                 ")
                StrSQL.Append("          ,[Gestione_Biologico]                                                             ")
                StrSQL.Append("          ,[Gestione_Rimanenze]                                                             ")
                StrSQL.Append("          ,[Gestione_Anticipazioni_Colturali]                                               ")
                StrSQL.Append("          ,[Macchine_Targa_Obbligatoria]                                                    ")
                StrSQL.Append("          ,[Tipologia_Report_Elas]                                                          ")
                StrSQL.Append("          ,[Tipologia_Elenco_Inadempienti]                                                  ")
                StrSQL.Append("          ,[Tipologia_Report_SegnalazioneAccise]                                            ")
                StrSQL.Append("          ,[stati_invio_mail_avanz_pratica]                                                 ")
                StrSQL.Append("          ,[gruppi_utenti_invio_mail_avanz_pratica])                                        ")
                StrSQL.Append("    VALUES                                              ")
                StrSQL.Append($"         (  {Agro_SQL_SaveNum(l.Anno)} ")
                StrSQL.Append($"          , {Agro_SQL_SaveNum(l.Per_Riduzione)} ")
                StrSQL.Append($"          , {Agro_SQL_SaveNum(l.Per_Mag_Terreno_B)} ")
                StrSQL.Append($"          , {Agro_SQL_SaveNum(l.Per_Mag_Terreno_Medio)}     ")
                StrSQL.Append($"          , {Agro_SQL_SaveNum(l.Per_Mag_Terreno_Tenace)}     ")
                StrSQL.Append($"          ,'{Agro_SQL_SaveText(l.Altre_Cfg)}' ")
                StrSQL.Append($"          , {Agro_SQL_SaveNum(l.Inviato)}")
                StrSQL.Append($"          , {Agro_SQL_SaveDate(l.DataInvio)}")
                StrSQL.Append($"          , {Agro_SQL_SaveDate(l.Data_Creazione)}")
                StrSQL.Append($"          , {Agro_SQL_SaveDate(l.Data_Modifica)}")
                StrSQL.Append($"          ,'{Agro_SQL_SaveText(l.Username_Creazione)}'")
                StrSQL.Append($"          ,'{Agro_SQL_SaveText(l.Username_Modifica)}'")
                StrSQL.Append($"          , {Agro_SQL_SaveDate(l.Validita_Inizio)}")
                StrSQL.Append($"          , {Agro_SQL_SaveDate(l.Validita_Fine)}")
                StrSQL.Append($"          , {Agro_SQL_SaveNum(l.Nr_Litri_Maggiorazione)}")
                StrSQL.Append($"          , {Agro_SQL_SaveNum(l.Percentuale_Integrazione_Terzista)}")
                StrSQL.Append($"          , {Agro_SQL_SaveNum(l.Percentuale_Richieste_Anticipo)}")
                StrSQL.Append($"          , {Agro_SQL_SaveNum(l.Gestione_Biologico)}")
                StrSQL.Append($"          , {Agro_SQL_SaveNum(l.Gestione_Rimanenze)}")
                StrSQL.Append($"          , {Agro_SQL_SaveNum(l.Gestione_Anticipazioni_Colturali)} ")
                StrSQL.Append($"          ,'{Agro_SQL_SaveText(l.Macchine_Targa_ObbligatoriaString)}' ")
                StrSQL.Append($"          , {Agro_SQL_SaveNum(l.Tipologia_report_Elas)} ")
                StrSQL.Append($"          , {Agro_SQL_SaveNum(l.Tipologia_Elenco_Inadempienti)} ")
                StrSQL.Append($"          , {Agro_SQL_SaveNum(l.Tipologia_Report_SegnalazioneAccise)} ")
                StrSQL.Append($"          ,'{Agro_SQL_SaveText(l.Stati_Invio_MailString)}' ")
                StrSQL.Append($"          ,'{Agro_SQL_SaveText(l.Gruppi_Utenti_Invio_MailString)}') ")
                xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)

            Next
            'context.SaveChanges()
        Catch ex As Exception


            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "]  " & MessaggioErrore)
        End Try

        Return True
    End Function

    Public Function Rimuovi(lista As List(Of UMASetupDto), context As Gias_DeveloperServer_Entities, objParametri As AgronicaCoreParametri) As Boolean

        ' ------------- Variabili -------------
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMASetup_W.Rimuovi()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            For Each elem In lista

                StrSQL = New System.Text.StringBuilder
                StrSQL.Append(" Delete from  [UMA_Setup] ")

                StrSQL.Append($" where [Anno] = {Agro_SQL_SaveNum(elem.Anno)} ")

                xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)


                'Dim record As UMA_Setup = context.UMA_Setup.Where(Function(s) s.Anno = elem.Anno).FirstOrDefault()

                'If Not record Is Nothing Then
                '    context.UMA_Setup.Remove(record)
                'End If
            Next
            'context.SaveChanges()
        Catch ex As Exception


            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return True
    End Function

    Public Function Aggiorna(righeModificateArr As List(Of UMASetupDto),
                             context As Gias_DeveloperServer_Entities,
                             ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMASetup_W.Aggiorna()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False
        Try

            For Each l In righeModificateArr

                StrSQL = New System.Text.StringBuilder
                StrSQL.Append(" Update [UMA_Setup] set ")

                StrSQL.Append($"           [Per_Riduzione]                            =     {Agro_SQL_SaveNum(l.Per_Riduzione)} ")
                StrSQL.Append($"          ,[Per_Mag_Terreno_B]                        =     {Agro_SQL_SaveNum(l.Per_Mag_Terreno_B)} ")
                StrSQL.Append($"          ,[Per_Mag_Terreno_Medio]                    =     {Agro_SQL_SaveNum(l.Per_Mag_Terreno_Medio)}     ")
                StrSQL.Append($"          ,[Per_Mag_Terreno_Tenace]                   =     {Agro_SQL_SaveNum(l.Per_Mag_Terreno_Tenace)}     ")
                StrSQL.Append($"          ,[Altre_Cfg]                                =    '{Agro_SQL_SaveText(l.Altre_Cfg)}' ")
                StrSQL.Append($"          ,[inviato]                                  =     {Agro_SQL_SaveNum(l.Inviato)}")
                StrSQL.Append($"          ,[datainvio]                                =     {Agro_SQL_SaveDate(l.DataInvio)}")
                StrSQL.Append($"          ,[Data_Modifica]                            =     {Agro_SQL_SaveDate(l.Data_Modifica)}")
                StrSQL.Append($"          ,[Username_Modifica]                        =    '{Agro_SQL_SaveText(l.Username_Modifica)}'")
                StrSQL.Append($"          ,[Validita_Inizio]                          =     {Agro_SQL_SaveDate(l.Validita_Inizio)}")
                StrSQL.Append($"          ,[Validita_Fine]                            =     {Agro_SQL_SaveDate(l.Validita_Fine)}")
                StrSQL.Append($"          ,[Nr_Litri_Maggiorazione]                   =     {Agro_SQL_SaveNum(l.Nr_Litri_Maggiorazione)}")
                StrSQL.Append($"          ,[Percentuale_Integrazione_Terzista]        =     {Agro_SQL_SaveNum(l.Percentuale_Integrazione_Terzista)}")
                StrSQL.Append($"          ,[Percentuale_Richieste_Anticipo]           =     {Agro_SQL_SaveNum(l.Percentuale_Richieste_Anticipo)}")
                StrSQL.Append($"          ,[Gestione_Biologico]                       =     {Agro_SQL_SaveNum(l.Gestione_Biologico)}")
                StrSQL.Append($"          ,[Gestione_Rimanenze]                       =     {Agro_SQL_SaveNum(l.Gestione_Rimanenze)}")
                StrSQL.Append($"          ,[Gestione_Anticipazioni_Colturali]         =     {Agro_SQL_SaveNum(l.Gestione_Anticipazioni_Colturali)} ")
                StrSQL.Append($"          ,[Macchine_Targa_Obbligatoria]              =    '{Agro_SQL_SaveText(l.Macchine_Targa_ObbligatoriaString)}' ")
                StrSQL.Append($"          ,[Tipologia_Report_Elas]                    =     {Agro_SQL_SaveNum(l.Tipologia_report_Elas)} ")
                StrSQL.Append($"          ,[Tipologia_Elenco_Inadempienti]            =     {Agro_SQL_SaveNum(l.Tipologia_Elenco_Inadempienti)} ")
                StrSQL.Append($"          ,[Tipologia_Report_SegnalazioneAccise]      =     {Agro_SQL_SaveNum(l.Tipologia_Report_SegnalazioneAccise)} ")
                StrSQL.Append($"          ,[stati_invio_mail_avanz_pratica]           =    '{Agro_SQL_SaveText(l.Stati_Invio_MailString)}' ")
                StrSQL.Append($"          ,[gruppi_utenti_invio_mail_avanz_pratica]   =    '{Agro_SQL_SaveText(l.Gruppi_Utenti_Invio_MailString)}' ")

                StrSQL.Append($" where [Anno] = {Agro_SQL_SaveNum(l.Anno)} ")

                xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)


                'Dim result As UMA_Setup = context.UMA_Setup.FirstOrDefault(Function(s) s.Anno = elem.Anno)
                'Dim result As UMA_Setup = (From a In context.UMA_Setup Where a.Anno = elem.Anno).FirstOrDefault()

                'If Not result Is Nothing Then
                '    result.Per_Riduzione = elem.Per_Riduzione
                '    result.Per_Mag_Terreno_B = elem.Per_Mag_Terreno_B
                '    result.Per_Mag_Terreno_Medio = elem.Per_Mag_Terreno_Medio
                '    result.Per_Mag_Terreno_Tenace = elem.Per_Mag_Terreno_Tenace
                '    result.Altre_Cfg = elem.Altre_Cfg
                '    result.Nr_Litri_Maggiorazione = elem.Nr_Litri_Maggiorazione
                '    result.Percentuale_Integrazione_Terzista = elem.Percentuale_Integrazione_Terzista


                '    result.Validita_Inizio = elem.Validita_Inizio
                '    result.Validita_Fine = elem.Validita_Fine
                '    result.inviato = elem.Inviato
                '    result.datainvio = elem.DataInvio
                '    result.Data_Modifica = elem.Data_Modifica
                '    result.Username_Modifica = elem.Username_Modifica
                'End If
            Next
            'context.SaveChanges()
        Catch ex As Exception


            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
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
        Public Percentuale_Richieste_Anticipo As Double
        Public Gestione_Biologico As String
        Public Gestione_Rimanenze As String
        Public Gestione_Anticipazioni_Colturali As String
        Public Macchine_Targa_ObbligatoriaString As String
        Public Tipologia_report_Elas As Integer
        Public Tipologia_Elenco_Inadempienti As Integer
        Public Tipologia_Report_SegnalazioneAccise As Integer
        Public Stati_Invio_MailString As String
        Public Gruppi_Utenti_Invio_MailString As String
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
