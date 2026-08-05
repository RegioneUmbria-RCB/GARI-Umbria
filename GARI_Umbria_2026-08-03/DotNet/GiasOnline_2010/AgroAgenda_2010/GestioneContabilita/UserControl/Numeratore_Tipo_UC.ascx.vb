Imports AgronicaCoreContabDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Public Class Numeratore_Tipo_UC
    Inherits System.Web.UI.UserControl

    '----- Gestione della pagina transazionale
    Dim EseguitaOperazione As Boolean
    Dim PremutoAnnulla As Boolean

    '----- variabili globali
    Dim Operazione As Integer
    Dim Messaggio As String = ""

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        inizializzoObjParametri()

        Try

            ' Nothing

        Catch ex As Exception

            'Messaggio di errore
            Messaggio = "Si e' verificato un'errore : " & Chr(13) & ex.Message.ToString()

            'Visualizzo il messaggio di errore
            Call Messaggi.AgroMsgBox(Messaggio, Page, "Form1")
            '------------------------------------------------

        End Try

    End Sub

    Private Sub inizializzoObjParametri()
        '---
        objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        '---
    End Sub

    Private Shared Function gettimesep() As String
        Return System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.TimeSeparator
    End Function

    Public Shared Function CaricaDropDownNumeratoriTipo(ByVal piva As String) As RispostaStandard

        Dim r = New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim objNumTipo As New Numeratore_Tipo_R
            Dim numeratori = objNumTipo.Leggi(piva, 0, "", "", "", objParametri_Server)

            Dim listaNum = (From num In numeratori.AsEnumerable()
                            Select New With
                               {
                                    .NumTipo_Cod = CInt(num.Item("Tipo")),
                                    .NumTipo_Descr = num.Item("Descrizione").ToString()
                               }).ToList()

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(listaNum, Newtonsoft.Json.Formatting.None)

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function
    Public Shared Function CaricaNumeratoriTipo(ByVal piva As String) As RispostaStandard

        Dim r = New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim objNumTipo As New Numeratore_Tipo_R
            Dim numeratori = objNumTipo.Leggi(piva, 0, "", "", "", objParametri_Server)

            Dim listaNum = (From num In numeratori.AsEnumerable()
                            Select New Numeratore_Tipo_Model With
                               {
                                    .Piva = num.Item("Piva").ToString(),
                                    .PivaSuperUser = num.Item("PivaSuperUser").ToString(),
                                    .Descrizione = num.Item("Descrizione").ToString(),
                                    .Sigla = num.Item("Sigla").ToString(),
                                    .Tipo = CInt(num.Item("Tipo")),
                                    .Validita_Inizio = If(num.Item("Validita_Inizio") Is DBNull.Value, AGRODATAINIZIO, CDate(num.Item("Validita_Inizio"))),
                                    .Validita_Fine = If(num.Item("Validita_Fine") Is DBNull.Value, AGRODATAFINE, CDate(num.Item("Validita_Fine")))
                               }).ToList()

            listaNum.ForEach(Function(s)
                                 s.Key_Numeratore_Tipo = String.Format("{0}-{1}-{2}", s.PivaSuperUser, s.Piva, s.Tipo)
                             End Function)


            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(listaNum, Newtonsoft.Json.Formatting.None)

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function
    Public Shared Function AggiornaNumeratoriTipo(ByVal paramString As String) As RispostaStandard

        Dim r = New RispostaStandard
        Dim parametri As ParametriSalvaModel = Nothing

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim objNumTipiW As New Numeratore_Tipo_W
        Dim objNumPs As New Numeratore_PrefissoSuffisso_R

        Dim Validita_Inizio As DateTime = AGRODATAINIZIO
        Dim Validita_Fine As DateTime = AGRODATAFINE

        Try

            parametri = JsonConvert.DeserializeObject(Of ParametriSalvaModel)(paramString)

            'inseriti
            If Not String.IsNullOrEmpty(parametri.RigheInserite) Then
                Dim inseriti As List(Of Numeratore_Tipo_Model) = New List(Of Numeratore_Tipo_Model)()
                inseriti.AddRange(JsonConvert.DeserializeObject(Of List(Of Numeratore_Tipo_Model))(parametri.RigheInserite, settingLoc))
                For Each m As Numeratore_Tipo_Model In inseriti

                    If Not m.Validita_Inizio Is Nothing Then Validita_Inizio = m.Validita_Inizio
                    If Not m.Validita_Fine Is Nothing Then Validita_Fine = m.Validita_Fine

                    objNumTipiW.Scrivi(parametri.Piva, m.Sigla, m.Descrizione,
                                       Validita_Inizio, Validita_Fine, "", "", objParametri_Server)
                Next


            End If

            'modificati
            If Not String.IsNullOrEmpty(parametri.RigheModificate) Then
                Dim modificati As List(Of Numeratore_Tipo_Model) = New List(Of Numeratore_Tipo_Model)()
                modificati.AddRange(JsonConvert.DeserializeObject(Of List(Of Numeratore_Tipo_Model))(parametri.RigheModificate, settingLoc))
                For Each m As Numeratore_Tipo_Model In modificati

                    If Not m.Validita_Inizio Is Nothing Then Validita_Inizio = m.Validita_Inizio
                    If Not m.Validita_Fine Is Nothing Then Validita_Fine = m.Validita_Fine

                    objNumTipiW.Aggiorna(m.Piva, m.Tipo, m.Sigla, m.Descrizione,
                                         Validita_Inizio, Validita_Fine, "", objParametri_Server)
                Next
            End If

            'eliminati
            If Not String.IsNullOrEmpty(parametri.RigheCancellate) Then
                Dim cancellati As List(Of Numeratore_Tipo_Model) = New List(Of Numeratore_Tipo_Model)()
                cancellati.AddRange(JsonConvert.DeserializeObject(Of List(Of Numeratore_Tipo_Model))(parametri.RigheCancellate, settingLoc))
                For Each m As Numeratore_Tipo_Model In cancellati

                    'Check se utilizzato prima di eliminarlo
                    Dim dtPS = objNumPs.Leggi(m.Piva, m.Tipo, "", "", objParametri_Server)
                    If Not dtPS Is Nothing And dtPS.Rows.Count > 0 Then
                        r.RispostaStringa = "Non è possibile eliminare questo Tipo Numeratore perchè utilizzato nei Prefissi/Suffissi."
                        Return r
                    Else
                        objNumTipiW.Cancella(m.Piva, m.Tipo, "", objParametri_Server)
                    End If

                Next
            End If

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function
#Region "Modelli per Rendering GUI"
    Private Class Numeratore_Tipo_Model

        Public Key_Numeratore_Tipo As String
        Public PivaSuperUser As String
        Public Piva As String
        Public Tipo As Integer?
        Public Sigla As String
        Public Descrizione As String
        Public Validita_Inizio As DateTime?
        Public Validita_Fine As DateTime?

    End Class

    Private Class ParametriSalvaModel

        Public Piva As String
        Public RigheInserite As String
        Public RigheModificate As String
        Public RigheCancellate As String

    End Class

#End Region


End Class
