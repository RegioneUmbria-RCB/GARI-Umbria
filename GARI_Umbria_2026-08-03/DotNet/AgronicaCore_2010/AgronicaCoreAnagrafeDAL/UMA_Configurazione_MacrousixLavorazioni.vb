Imports AgronicaCoreDataProvider
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports System.Text
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports Newtonsoft.Json
Imports System.Runtime.CompilerServices
Imports System.Transactions

Public Class UMA_Configurazione_MacrousixLavorazioni_R
    Inherits AgronicaCoreDataProvider.DataProvider
    Public Function Leggi(ByVal Regione_Cod As String,
                          ByVal Macrouso_UMA_Cod As String,
                          ByVal Lav_UMA_Cod As String,
                          ByVal Lav_Cod As Integer,
                          ByVal Id_Attivita As Integer,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          Optional xFiltroAggiuntivo As String = "") As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_Richieste.Leggi()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable
        Dim PivaSuperUser = objParametri.PivaSuperUser

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT UMA_Configurazione_MacrousixLavorazioni.*, ")
            stb.AppendLine("        UMA_Macrousi.Macrouso_UMA_Des, ")
            stb.AppendLine("        UMA_Macrousi.Macrouso_UMA_Cod, ")
            stb.AppendLine("        UMA_Lavorazioni.Lav_UMA_Des, ")
            stb.AppendLine("        UMA_Lavorazioni.Lav_UMA_Cod, ")
            stb.AppendLine("        UMA_Lavorazioni.Maggiorazione_Terreno_MedioTenace, ")
            stb.AppendLine("        UMA_Lavorazioni.GestioneTerzista, ")
            stb.AppendLine("        Operazioni.Lav_Des, ")
            stb.AppendLine("        Operazioni.Lav_Cod, ")
            stb.AppendLine("        Attivita.Id_Attivita, ")
            stb.AppendLine("        Attivita.[Desc] ")
            stb.AppendLine(" FROM UMA_Configurazione_MacrousixLavorazioni ")
            stb.AppendLine(" JOIN Operazioni ON UMA_Configurazione_MacrousixLavorazioni.Lav_Cod = Operazioni.Lav_Cod ")
            stb.AppendLine(" JOIN UMA_Macrousi ON UMA_Configurazione_MacrousixLavorazioni.Macrouso_UMA_Cod = UMA_Macrousi.Macrouso_UMA_Cod ")
            stb.AppendLine(" JOIN UMA_Lavorazioni ON UMA_Configurazione_MacrousixLavorazioni.Lav_UMA_Cod = UMA_Lavorazioni.Lav_UMA_Cod ")
            stb.AppendLine(" LEFT JOIN Attivita ON UMA_Configurazione_MacrousixLavorazioni.Id_Attivita = Attivita.Id_Attivita ")
            stb.AppendLine(" WHERE 1 = 1 ")

            If Regione_Cod <> "" Then
                stb.AppendLine(" AND UMA_Configurazione_MacrousixLavorazioni.Regione_Cod = " & Agro_SQL_SaveText_NULL(Regione_Cod) & " ")
            End If

            If Macrouso_UMA_Cod <> "" Then
                stb.AppendLine(" AND UMA_Configurazione_MacrousixLavorazioni.Macrouso_UMA_Cod = " & Agro_SQL_SaveText_NULL(Macrouso_UMA_Cod) & " ")
            End If

            If Lav_UMA_Cod <> "" Then
                stb.AppendLine(" AND UMA_Configurazione_MacrousixLavorazioni.Lav_UMA_Cod = " & Agro_SQL_SaveText_NULL(Lav_UMA_Cod) & " ")
            End If

            If Lav_Cod <> 0 Then
                stb.AppendLine(" AND UMA_Configurazione_MacrousixLavorazioni.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & " ")
            End If

            If Id_Attivita <> 0 Then
                stb.AppendLine(" AND UMA_Configurazione_MacrousixLavorazioni.Id_Attivita = " & Agro_SQL_SaveNum(Id_Attivita) & " ")
            End If

            If Not String.IsNullOrEmpty(xFiltroAggiuntivo) Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                'stb.AppendLine(" AND getdate() BETWEEN UMA_Configurazione_MacrousixLavorazioni.Validita_Inizio AND UMA_Configurazione_MacrousixLavorazioni.Validita_Fine ")
            End If


            stb.AppendLine(" ORDER BY UMA_Configurazione_MacrousixLavorazioni.Macrouso_UMA_Cod, UMA_Configurazione_MacrousixLavorazioni.Lav_UMA_Cod, UMA_Configurazione_MacrousixLavorazioni.Ordinamento ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Leggi(ByRef objParametri As AgronicaCoreParametri) As DataTable
        'Dim FlagConnessioneLocale As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_Configurazione_MacrousixLavorazioni.AgronicaCoreDataProvider_Leggi()"

        ' ------- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Dim StrSQL As New System.Text.StringBuilder
            StrSQL.Length = 0

            StrSQL.Append("SELECT op.LAV_DES,
                              macroUsi.Macrouso_UMA_Des,
                              lavorazioni.Lav_Uma_Des, 
                              Attivita.[Desc],
                              UMA_Configurazione_MacrousixLavorazioni.* 
                           FROM UMA_Configurazione_MacrousixLavorazioni
                           INNER JOIN Operazioni op ON UMA_Configurazione_MacrousixLavorazioni.Lav_Cod = op.LAV_COD
                           INNER JOIN UMA_Macrousi macroUsi ON UMA_Configurazione_MacrousixLavorazioni.Regione_Cod = macroUsi.Regione_Cod
                              AND UMA_Configurazione_MacrousixLavorazioni.Macrouso_UMA_Cod = macroUsi.Macrouso_UMA_Cod
                           INNER JOIN UMA_Lavorazioni lavorazioni ON UMA_Configurazione_MacrousixLavorazioni.Regione_Cod = lavorazioni.Regione_Cod
                              AND UMA_Configurazione_MacrousixLavorazioni.Lav_UMA_Cod = lavorazioni.Lav_UMA_Cod
                           LEFT JOIN Attivita ON Attivita.ID_Attivita = UMA_Configurazione_MacrousixLavorazioni.Id_Attivita")


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function

    Public Function VerificaElementoNonEsisteInDB(lav As UMA_Configurazione_MacrousixLavorazioni_W.LavorazioneUMA,
                                                  efConnString As String) As Boolean
        Using dal As New Gias_DeveloperServer_Entities(efConnString)
            Dim exists = dal.UMA_Configurazione_MacrousixLavorazioni.Any(Function(s) s.Regione_Cod = lav.RegioneCod AndAlso s.Macrouso_UMA_Cod = lav.MacrousoUMACod AndAlso s.Lav_UMA_Cod = lav.LavUMACod AndAlso s.Lav_Cod = lav.LavCod AndAlso s.Id_Attivita = lav.IdAttivita)
            Return exists
        End Using
    End Function

    ' ******************************************** Query UMA_Macrousi e crea l'elenco ********************************************
    Public Function LeggiDropdown_UMA_Macrousi(objparametri_Server As AgronicaCoreParametri) As List(Of VoceElencoDiDropdown)

        Dim NomeRoutine As String = "UMA_Configurazione_MacrousixLavorazioni.UMA_.UMA_Configurazione_MacrousixLavorazioni_R.LeggiDropdown_UMA_Macrousi()"

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

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objparametri_Server, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return elenco
    End Function

    ' ******************************************** Query UMA_Lavorazioni e crea l'elenco ********************************************
    Public Function LeggiDropdown_UMALavorazioni(objparametri_Server As AgronicaCoreParametri) As List(Of VoceElencoDiDropdown)

        Dim NomeRoutine As String = "UMA_Configurazione_MacrousixLavorazioni.UMA_.UMA_Configurazione_MacrousixLavorazioni_R.LeggiDropdown_UMALavorazioni()"

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

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objparametri_Server, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return elenco
    End Function

    ' ******************************************** Query Operazioni e crea l'elenco ********************************************
    Public Function LeggiDropdown_Operazioni(objparametri_Server As AgronicaCoreParametri) As List(Of VoceElencoDiDropdown)

        Dim NomeRoutine As String = "UMA_Configurazione_MacrousixLavorazioni.UMA_.UMA_Configurazione_MacrousixLavorazioni_R.LeggiDropdown_Operazioni()"

        Dim MessaggioErrore As String = ""
        Dim elenco As New List(Of VoceElencoDiDropdown)
        Dim stb As New System.Text.StringBuilder

        Try
            Dim gefutils As New Gias_EF_Utility
            Dim efConnString As String = gefutils.GetEntityConnectionString(objparametri_Server.StringaConnessione)

            Using db As New Gias_DeveloperServer_Entities(efConnString)
                Dim dbElem = db.Operazioni.ToList()
                elenco = dbElem.Select(Function(s) New VoceElencoDiDropdown(s.LAV_COD, s.LAV_DES)).ToList()
            End Using

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objparametri_Server, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return elenco
    End Function

    ' ******************************************** Query Attivita e crea l'elenco ********************************************
    Public Function LeggiDropdown_Attivita(objparametri_Server As AgronicaCoreParametri) As List(Of VoceElencoDiDropdown)

        Dim NomeRoutine As String = "UMA_Configurazione_MacrousixLavorazioni.UMA_.UMA_Configurazione_MacrousixLavorazioni_R.LeggiDropdown_Attivita()"

        Dim MessaggioErrore As String = ""
        Dim elenco As New List(Of VoceElencoDiDropdown)
        Dim stb As New System.Text.StringBuilder

        Try
            Dim gefutils As New Gias_EF_Utility
            Dim efConnString As String = gefutils.GetEntityConnectionString(objparametri_Server.StringaConnessione)

            Using db As New Gias_DeveloperServer_Entities(efConnString)
                Dim dbElem = db.Attivita.ToList()
                elenco = dbElem.Select(Function(s) New VoceElencoDiDropdown(s.ID_Attivita, s.Desc)).ToList()
            End Using

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objparametri_Server, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return elenco
    End Function



    Class VoceElencoDiDropdown
        Public Code As Integer
        Public Descrizione As String

        Public Sub New(cod As Integer, des As String)
            Code = cod
            Descrizione = des
        End Sub
    End Class
End Class

Public Class UMA_Configurazione_MacrousixLavorazioni_W
    Inherits AgronicaCoreDataProvider.DataProvider

    ''' <summary>
    ''' Aggiungi ogni riga inserita dal utente nel database. SaveChanges() è 
    ''' chiamato ulteriormente dal metodo Salva_GrigliaLavorazioniUMA.
    ''' </summary>
    ''' <returns></returns>
    Public Function AggiungiNouvi(listLav As List(Of LavorazioneUMA),
                                  context As Gias_DeveloperServer_Entities,
                                  objParametri As AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_Configurazione_MacrousixLavorazioni.AggiungiNouvi()"
        Dim MessaggioErrore As String = ""

        Try
            For Each l In listLav
                context.UMA_Configurazione_MacrousixLavorazioni.Add(l.ToLavorazioneUMADB())
            Next
            context.SaveChanges()
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return True
    End Function

    Public Function Rimuovi(l As LavorazioneUMA, context As Gias_DeveloperServer_Entities, objParametri As AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_Configurazione_MacrousixLavorazioni.Rimuovi()"

        ' ------------- Variabili -------------
        Dim DT As DataTable
        Dim MessaggioErrore As String = ""

        Try
            Dim record As UMA_Configurazione_MacrousixLavorazioni = context.UMA_Configurazione_MacrousixLavorazioni.Where(Function(s) s.Regione_Cod = l.RegioneCod AndAlso s.Lav_UMA_Cod = l.LavUMACod AndAlso s.Macrouso_UMA_Cod = l.MacrousoUMACod AndAlso s.Tipo_Operazione = l.TipoOperazioneCod AndAlso s.Id_Attivita = l.IdAttivita).FirstOrDefault()

            If Not record Is Nothing Then
                context.UMA_Configurazione_MacrousixLavorazioni.Remove(record)
                context.SaveChanges()
            End If
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return True
    End Function

    Public Function Aggiorna(righeModificateArr As List(Of LavorazioneUMA),
                             context As Gias_DeveloperServer_Entities,
                             ByRef objParametri As AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_Configurazione_MacrousixLavorazioni.Aggiorna()"

        ' ------------- Variabili -------------
        Dim DT As DataTable
        Dim MessaggioErrore As String = ""

        Try

            For Each elem In righeModificateArr
                Dim result As UMA_Configurazione_MacrousixLavorazioni = context.UMA_Configurazione_MacrousixLavorazioni.FirstOrDefault(Function(s) s.Regione_Cod = elem.RegioneCod AndAlso s.Macrouso_UMA_Cod = elem.MacrousoUMACod AndAlso s.Lav_UMA_Cod = elem.LavUMACod AndAlso s.Lav_Cod = elem.LavCod AndAlso s.Id_Attivita = elem.IdAttivita)

                If Not result Is Nothing Then
                    result.Tipo_Operazione = elem.TipoOperazioneCod
                    result.Gasolio_Lt = elem.GasolioLt
                    result.Benzina_Lt = elem.BenzinaLt
                    result.Ordinamento = elem.OrdinamentoCod
                    result.N_Max_Operazioni = elem.NMaxOperazioni
                    result.Default = elem.Default
                    result.Data_Modifica = elem.DataModifica
                    result.Username_Modifica = elem.UsernameModifica
                    result.Validita_Inizio = elem.ValiditaInizio
                    result.Validita_Fine = elem.ValiditaFine
                    result.Udm_Alternativa = elem.UDMAlternativa
                    result.Gasolio_LtxBiologico = elem.GasolinoLTxBiologico
                    result.Benzina_LtxBiologico = elem.BensinaLTxBiologico
                    result.Limite_Max = elem.LimiteMax
                    result.Max_xHa = elem.MaxxHa
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


    Class LavorazioneUMA
        ' *************************** Collone prese in join con altre tabelle ***************************
        Public Operazioni_LavDeS As String
        Public UMAMacrousi_MacrousoUMADes As String
        Public UMALavorazioni_LavUmaDes As String
        Public Attivita_Desc As String

        ' ************************* Collone della tabella UMA_Configurazione_MacrousixLavorazioni *************************
        Public RegioneCod As String
        Public MacrousoUMACod As String
        Public LavUMACod As String
        Public LavCod As Integer
        Public IdAttivita As Integer
        Public TipoOperazioneCod As Integer
        Public GasolioLt As Double
        Public BenzinaLt As Double
        Public OrdinamentoCod As Integer
        Public NMaxOperazioni As Integer
        Public [Default] As Integer?
        Public Inviato As Short
        Public DataInvio As Date?
        Public DataCreazione As Date?
        Public DataModifica As Date?
        Public UsernameCreazione As String
        Public UsernameModifica As String
        Public ValiditaInizio As Date?
        Public ValiditaFine As Date?
        Public UDMAlternativa As String
        Public GasolinoLTxBiologico As Double
        Public BensinaLTxBiologico As Double
        Public LimiteMax As Integer
        Public MaxxHa As Double
    End Class

End Class



Module Extensions
    <Extension()>
    Function ToLavorazioneUMADB(ByVal l As UMA_Configurazione_MacrousixLavorazioni_W.LavorazioneUMA) As AgronicaCoreEntityFramework_POCO.UMA_Configurazione_MacrousixLavorazioni
        Dim r As New AgronicaCoreEntityFramework_POCO.UMA_Configurazione_MacrousixLavorazioni
        r.Regione_Cod = l.RegioneCod
        r.Macrouso_UMA_Cod = l.MacrousoUMACod
        r.Lav_UMA_Cod = l.LavUMACod
        r.Lav_Cod = l.LavCod
        r.Id_Attivita = l.IdAttivita
        r.Tipo_Operazione = l.TipoOperazioneCod
        r.Gasolio_Lt = l.GasolioLt
        r.Benzina_Lt = l.BenzinaLt
        r.Ordinamento = l.OrdinamentoCod
        r.N_Max_Operazioni = l.NMaxOperazioni
        r.Default = l.Default
        r.inviato = 0
        r.datainvio = l.DataInvio

        r.Data_Creazione = l.DataCreazione
        r.Data_Modifica = l.DataModifica

        r.Username_Creazione = l.UsernameCreazione
        r.Username_Modifica = l.UsernameModifica
        r.Validita_Inizio = l.ValiditaInizio
        r.Validita_Fine = l.ValiditaFine
        r.Udm_Alternativa = l.UDMAlternativa
        r.Gasolio_LtxBiologico = l.GasolinoLTxBiologico
        r.Benzina_LtxBiologico = l.BensinaLTxBiologico
        r.Limite_Max = l.LimiteMax
        r.Max_xHa = l.MaxxHa
        Return r
    End Function
End Module
