Imports AgronicaCoreContabDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Public Class Numeratore_PrefissoSuffisso_UC

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

    Public Shared Function CaricaNumeratoriPSB(ByVal piva As String) As RispostaStandard

        Dim r = New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim objNumPS As New Numeratore_PrefissoSuffisso_R
            Dim numeratori = objNumPS.Leggi(piva, 0, "", "", objParametri_Server)

            Dim listaNum = (From num In numeratori.AsEnumerable()
                            Select New Numeratore_PS_Model With
                               {
                                    .Id = num.Item("Id"),
                                    .Piva = num.Item("Piva").ToString(),
                                    .PivaSuperUser = num.Item("PivaSuperUser").ToString(),
                                    .Descrizione = num.Item("Descrizione").ToString(),
                                    .NumTipo_Descr = num.Item("DescrizioneTipo").ToString(),
                                    .NumTipo_Cod = CInt(num.Item("Numeratore_Tipo")),
                                    .Doc_Numero_Sin = If(num.Item("Doc_Numero_Sin") Is DBNull.Value, "", num.Item("Doc_Numero_Sin")),
                                    .Doc_Numero_Des = If(num.Item("Doc_Numero_Des") Is DBNull.Value, "", num.Item("Doc_Numero_Des")),
                                    .Lunghezza_Centro = If(num.Item("Lunghezza_Centro") Is DBNull.Value, 0, CInt(num.Item("Lunghezza_Centro"))),
                                    .CarattereFormattazione = If(num.Item("CarattereFormattazione") Is DBNull.Value, "", num.Item("CarattereFormattazione")),
                                    .Validita_Inizio = If(num.Item("Validita_Inizio") Is DBNull.Value, AGRODATAINIZIO, CDate(num.Item("Validita_Inizio"))),
                                    .Validita_Fine = If(num.Item("Validita_Fine") Is DBNull.Value, AGRODATAFINE, CDate(num.Item("Validita_Fine")))
                               }).ToList()

            listaNum.ForEach(Function(s)
                                 s.Key_Numeratore_PS = String.Format("{0}-{1}", s.PivaSuperUser, s.Piva)
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

    Public Shared Function CheckPreSalva_NumeratoriPS(ByVal paramString As String) As RispostaStandard

        Dim r = New RispostaStandard
        Dim parametri As ParametriSalvaModel = Nothing

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim objDefault As New Documento_Default_R

        Try

            parametri = JsonConvert.DeserializeObject(Of ParametriSalvaModel)(paramString)

            Dim cancellati As List(Of Numeratore_PS_Model) = New List(Of Numeratore_PS_Model)()
            If Not String.IsNullOrEmpty(parametri.RigheCancellate) Then
                cancellati.AddRange(JsonConvert.DeserializeObject(Of List(Of Numeratore_PS_Model))(parametri.RigheCancellate, settingLoc))
            End If

            'eliminati
            If cancellati.Any Then
                For Each m As Numeratore_PS_Model In cancellati

                    'prima di cancellare controllo se utilizzato
                    Dim dsDefault = objDefault.Leggi(
                        m.Piva, Nothing, "", m.NumTipo_Cod, Nothing, Nothing, Nothing,
                        "", "", AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, objParametri_Server)

                    If Not dsDefault Is Nothing And dsDefault.Rows.Count > 0 Then
                        r.RispostaStringa = "Questo Prefisso/Suffisso è utilizzato nei Defaults. Eliminarlo comunque? "
                        Exit For
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

    Public Shared Function AggiornaNumeratoriPS(ByVal paramString As String) As RispostaStandard

        Dim r = New RispostaStandard
        Dim parametri As ParametriSalvaModel = Nothing

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim objNumPS As New Numeratore_PrefissoSuffisso_W
        Dim objDefault As New Documento_Default_R

        Dim Validita_Inizio As DateTime = AGRODATAINIZIO
        Dim Validita_Fine As DateTime = AGRODATAFINE
        Dim sovrapposizioneTemporale As Boolean = False

        Try

            parametri = JsonConvert.DeserializeObject(Of ParametriSalvaModel)(paramString)

            Dim cancellati As List(Of Numeratore_PS_Model) = New List(Of Numeratore_PS_Model)()
            If Not String.IsNullOrEmpty(parametri.RigheCancellate) Then
                cancellati.AddRange(JsonConvert.DeserializeObject(Of List(Of Numeratore_PS_Model))(parametri.RigheCancellate, settingLoc))
            End If

            If Not cancellati.Any Then
                Dim tutti As List(Of Numeratore_PS_Model) = New List(Of Numeratore_PS_Model)()
                tutti.AddRange(JsonConvert.DeserializeObject(Of List(Of Numeratore_PS_Model))(parametri.TutteLeRighe, settingLoc))

                If tutti.Any() Then
                    Dim numeratoriTipo = tutti.Select(Function(s) s.NumTipo_Cod).Distinct

                    For Each nt As Integer? In numeratoriTipo

                        Dim prefissiSuffissi = tutti.Where(Function(s) s.NumTipo_Cod.Value.Equals(nt.Value))
                        If Not prefissiSuffissi Is Nothing AndAlso prefissiSuffissi.Count() > 0 Then

                            Dim intervallo = New PeriodoTemporale(
                            prefissiSuffissi.FirstOrDefault().Validita_Inizio,
                            prefissiSuffissi.FirstOrDefault().Validita_Fine)

                            For i As Integer = 1 To prefissiSuffissi.Count() - 1
                                Dim intervalloConfronto = New PeriodoTemporale(
                                                    prefissiSuffissi(i).Validita_Inizio,
                                                    prefissiSuffissi(i).Validita_Fine)

                                ' Controllo se le date sono intersecate
                                If intervallo.IntersecaCon(intervalloConfronto) Then
                                    sovrapposizioneTemporale = True
                                    Exit For
                                End If

                            Next

                        End If

                    Next

                End If

                If sovrapposizioneTemporale Then

                    r.RispostaOK = False
                    r.RispostaStringa = "Sono presenti sovrapposizioni temporali non ammesse per lo stesso tipo di numeratore."
                    Return r

                End If

            End If


            'inseriti
            If Not String.IsNullOrEmpty(parametri.RigheInserite) Then
                Dim inseriti As List(Of Numeratore_PS_Model) = New List(Of Numeratore_PS_Model)()
                inseriti.AddRange(JsonConvert.DeserializeObject(Of List(Of Numeratore_PS_Model))(parametri.RigheInserite, settingLoc))
                For Each m As Numeratore_PS_Model In inseriti

                    If Not m.Validita_Inizio Is Nothing Then Validita_Inizio = m.Validita_Inizio
                    If Not m.Validita_Fine Is Nothing Then Validita_Fine = m.Validita_Fine

                    objNumPS.Scrivi(parametri.Piva, m.NumTipo_Cod,
                                    m.Doc_Numero_Sin, m.Doc_Numero_Des, m.Descrizione,
                                    m.Lunghezza_Centro, m.CarattereFormattazione,
                                    Validita_Inizio, Validita_Fine, "", "", objParametri_Server)
                Next


            End If

            'modificati
            If Not String.IsNullOrEmpty(parametri.RigheModificate) Then
                Dim modificati As List(Of Numeratore_PS_Model) = New List(Of Numeratore_PS_Model)()
                modificati.AddRange(JsonConvert.DeserializeObject(Of List(Of Numeratore_PS_Model))(parametri.RigheModificate, settingLoc))
                For Each m As Numeratore_PS_Model In modificati

                    If Not m.Validita_Inizio Is Nothing Then Validita_Inizio = m.Validita_Inizio
                    If Not m.Validita_Fine Is Nothing Then Validita_Fine = m.Validita_Fine

                    objNumPS.Aggiorna(m.Id, m.Piva, m.NumTipo_Cod,
                                      m.Doc_Numero_Sin, m.Doc_Numero_Des, m.Descrizione,
                                      m.Lunghezza_Centro, m.CarattereFormattazione,
                                      Validita_Inizio, Validita_Fine, "", objParametri_Server)
                Next
            End If

            'eliminati
            If cancellati.Any Then
                For Each m As Numeratore_PS_Model In cancellati
                    objNumPS.Cancella(m.Id, "", objParametri_Server)
                Next
            End If

            r.RispostaOK = True
            r.RispostaStringa = ""

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

#Region "Modelli per Rendering GUI"
    Private Class Numeratore_PS_Model

        Public Key_Numeratore_PS As String
        Public Id As Integer?
        Public PivaSuperUser As String
        Public Piva As String
        Public NumTipo_Cod As Integer?
        Public NumTipo_Descr As String
        Public Doc_Numero_Sin As String
        Public Doc_Numero_Des As String
        Public Descrizione As String
        Public Lunghezza_Centro As Integer?
        Public CarattereFormattazione As String
        Public Validita_Inizio As DateTime?
        Public Validita_Fine As DateTime?

    End Class

    Private Class ParametriSalvaModel

        Public Piva As String
        Public RigheInserite As String
        Public RigheModificate As String
        Public RigheCancellate As String
        Public TutteLeRighe As String

    End Class

#End Region

    Private Class PeriodoTemporale
        Public StartP As Date?
        Public EndP As Date?

        Public Sub New(ByVal startP As Date?, ByVal endP As Date?)
            Me.StartP = If(Not startP Is Nothing AndAlso startP.HasValue, startP, AGRODATAINIZIO)
            Me.EndP = If(Not endP Is Nothing AndAlso endP.HasValue, endP, AGRODATAFINE)
        End Sub

        Public Function IntersecaCon(ByVal otherPeriod As PeriodoTemporale) As Boolean

            Return Not (StartP > otherPeriod.EndP Or EndP < otherPeriod.StartP)

        End Function


    End Class


End Class
