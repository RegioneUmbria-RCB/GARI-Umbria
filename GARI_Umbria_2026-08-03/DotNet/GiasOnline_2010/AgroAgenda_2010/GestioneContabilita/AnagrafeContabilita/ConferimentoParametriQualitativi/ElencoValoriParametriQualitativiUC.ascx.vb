Imports AgronicaCoreDataProvider
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports AgroAgenda_2010.Resources
Imports AgronicaCoreContabDAL

Public Class ElencoValoriParametriQualitativiUC
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Friend Shared Function LeggiElencoValoriParametriQualitativi(piva As String) As RispostaStandard
        Dim r As New RispostaStandard
        Dim DT As New DataTable

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim objOModuli_Referenze_Config_Dettagli = New AgronicaCoreAnagrafeDAL.OTabelle_R
            DT = objOModuli_Referenze_Config_Dettagli.LeggiParametriQualitativi(piva, objParametri_Server, "Tipo IN (1, 2)")

            Dim colonneDaTenere As String() = {"Tabella_Cod", "Tabella_Cod_Des", "Modulo_Generazione"}
            Dim colonnePresenti As DataColumn() = DT.Columns.OfType(Of DataColumn).ToArray()

            For Each col As DataColumn In colonnePresenti
                If Not colonneDaTenere.Contains(col.ColumnName) Then
                    DT.Columns.Remove(col)
                End If
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

    Friend Shared Function Leggi_ValoriParametriQualitativi(piva As String) As RispostaStandard
        Dim r As New RispostaStandard
        Dim DT As New DataTable

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim orderBy As String = "OTabelle.Tabella_Cod, OTabelle_Parametri.Tabella_Par_Cod"
            Dim filtroAggiuntivo As New StringBuilder()
            filtroAggiuntivo.AppendLine($" OTabelle_Parametri.Piva IN ('AAAAAAAAAAA', '{piva}')")

            Dim leggiAnagrafeLog As New OGenerazioni_Anagrafe_Moduli_Log_R
            Dim moduliConferimento = leggiAnagrafeLog.LeggiModuliParametriQualitativi(piva, objParametri_Server)

            Dim objOModuli_Referenze_Config_Dettagli = New OTabelle_Parametri_R
            DT = objOModuli_Referenze_Config_Dettagli.LeggiJoinParametri(objParametri_Server, filtroAggiuntivo.ToString(), orderBy)

            DT.Columns.Remove("piva")
            DT.Columns("piva1").ColumnName = "Piva"
            DT.Columns.Add("IdParam", GetType(String))
            DT.Columns.Add("Valore_Minimo_Parametro", GetType(String)).AllowDBNull = True
            DT.Columns.Add("Valore_Massimo_Parametro", GetType(String)).AllowDBNull = True
            For Each col As DataColumn In moduliConferimento.Columns
                DT.Columns.Add(col.ColumnName, col.DataType)
            Next
            DT.Columns.Add("Rimuovere", GetType(Boolean))

            For Each row As DataRow In DT.Rows
                row("IdParam") = row("piva") & "_" & row("Tabella_Cod") & "_" & row("Tabella_Par_Cod") & "_" & row("Modulo_Generazione")

                Dim mc As DataRow = moduliConferimento.AsEnumerable() _
                    .FirstOrDefault(Function(x) CInt(x("Modulo_Cod")) = CInt(row("Modulo_Generazione")))

                row("Rimuovere") = IsNothing(mc)
                If Not IsNothing(mc) Then
                    For Each col As DataColumn In moduliConferimento.Columns
                        row(col.ColumnName) = mc(col.ColumnName)
                    Next
                End If

                If IsDBNull(row("Valore_Min")) OrElse CInt(row("Valore_Min")) = -2000000000 Then
                    row("Valore_Minimo_Parametro") = Nothing
                Else
                    row("Valore_Minimo_Parametro") = row("Valore_Min").ToString()
                End If

                If IsDBNull(row("Valore_Max")) OrElse CInt(row("Valore_Max")) = 2000000000 Then
                    row("Valore_Massimo_Parametro") = Nothing
                Else
                    row("Valore_Massimo_Parametro") = row("Valore_Max").ToString()
                End If

            Next

            DT.Columns.Remove("Valore_Min")
            DT.Columns.Remove("Valore_Max")
            DT.Columns("Valore_Minimo_Parametro").ColumnName = "Valore_Min"
            DT.Columns("Valore_Massimo_Parametro").ColumnName = "Valore_Max"

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

    Friend Shared Function AggiornaDaGrigliaElencoValoriParametriQualitativi(ByVal piva As String,
                                                                             ByVal righeInserite As String,
                                                                             ByVal righeModificate As String,
                                                                             ByVal righeCancellate As String,
                                                                             ByVal tutteleRighe As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim testata = New AgronicaCoreContabDAL.OTabelle_Parametri_W
            r.RispostaStringa = testata.AggiornaRecordParametriModificati(piva, righeInserite, righeModificate, righeCancellate, tutteleRighe, objParametri_Server)

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