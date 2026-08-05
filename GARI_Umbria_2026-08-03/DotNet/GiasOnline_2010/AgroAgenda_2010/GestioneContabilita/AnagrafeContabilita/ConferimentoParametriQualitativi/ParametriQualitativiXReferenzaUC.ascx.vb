Imports AgronicaCoreDataProvider
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports AgroAgenda_2010.Resources
Imports AgronicaCoreContabDAL

Public Class ParametriQualitativiXReferenzaUC
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Friend Shared Function Leggi_Elenco_GruppiReferenze(piva As String) As RispostaStandard
        Dim r As New RispostaStandard
        Dim DT As New DataTable

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim objOModuli_Referenze_Config_Testata = New AgronicaCoreContabDAL.OModuli_Referenze_Config_Testata_R
            DT = objOModuli_Referenze_Config_Testata.LeggiModuliConferimento(piva, objParametri_Server)

            Dim colonneDaTenere As String() = {"Id_Testata", "Descrizione", "Modulo_Generazione"}
            Dim colonnePresenti As DataColumn() = DT.Columns.OfType(Of DataColumn).ToArray()

            For Each col As DataColumn In colonnePresenti
                If Not colonneDaTenere.Contains(col.ColumnName) Then
                    DT.Columns.Remove(col)
                End If
            Next
            DT.Columns("Descrizione").ColumnName = "Id_Testata_Des"


            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(DT, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    Friend Shared Function Leggi_Elenco_ParametriQualitativi(piva As String) As RispostaStandard
        Dim r As New RispostaStandard
        Dim DT As New DataTable
        Dim toLower As Boolean = True

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            DT.Columns.Add("Tabella_ID", GetType(String))
            DT.Columns.Add("Tabella_Key", GetType(String))
            DT.Columns.Add("Modulo_Generazione", GetType(Integer))


            Dim leggiAnagrafeLog As New OGenerazioni_Anagrafe_Moduli_Log_R
            Dim moduliConferimento = leggiAnagrafeLog.LeggiModuliParametriQualitativi(piva, objParametri_Server)

            Dim objOModuli_Referenze_Config_Dettagli = New AgronicaCoreAnagrafeDAL.OTabelle_R
            Dim parametri = objOModuli_Referenze_Config_Dettagli.LeggiParametriQualitativi(piva, objParametri_Server, "Tipo <> 2", toLower)

            DT.Columns.Add("Rimuovere", GetType(Boolean))
            For Each row As DataRow In parametri.Rows
                Dim mc As DataRow = moduliConferimento.AsEnumerable() _
                    .FirstOrDefault(Function(x) CInt(x("Modulo_Cod")) = CInt(row("Modulo_Generazione")))
                Dim rimuovere As Boolean = IsNothing(mc)

                DT.Rows.Add(New Object() {row("Tabella_Cod"), row("Tabella_Cod_Des"), CInt(row("Modulo_Generazione")), rimuovere})
            Next

            Dim daRimuovere = DT.AsEnumerable() _
                    .Where(Function(x) x("Rimuovere") = True) _
                    .ToList()

            For Each row As DataRow In daRimuovere
                DT.Rows.Remove(row)
            Next

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(DT, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    Friend Shared Function Leggi_ParametriQualitativiXReferenza(piva As String) As RispostaStandard
        Dim r As New RispostaStandard
        Dim DT As New DataTable

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim leggiAnagrafeLog As New OModuli_Referenze_Config_Dettagli_R
            r.RispostaStringa = leggiAnagrafeLog.LeggiParametriQualitativiXConfigurazione(piva, objParametri_Server)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    Friend Shared Function AggiornaDaGrigliaParametriQualitativiXReferenza(ByVal piva As String, ByVal righeInserite As String, ByVal righeModificate As String, ByVal righeCancellate As String, ByVal tutteleRighe As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim leggiAnagrafeLog As New OModuli_Referenze_Config_Dettagli_W
            r.RispostaStringa = leggiAnagrafeLog.AggiornaRecordModificati(piva, righeInserite, righeModificate, righeCancellate, tutteleRighe, objParametri_Server)

            If String.IsNullOrEmpty(r.RispostaStringa) Then
                r.RispostaOK = True
            Else
                Throw New Exception(r.RispostaStringa)
            End If

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False, source:=False)
        End Try

        Return r
    End Function

End Class