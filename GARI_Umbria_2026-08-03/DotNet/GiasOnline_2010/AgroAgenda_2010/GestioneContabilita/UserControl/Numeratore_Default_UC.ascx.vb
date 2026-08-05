Imports AgronicaCoreContabDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreMetaSchemaDAL
Imports AgronicaCoreContabBIZ

Public Class Numeratore_Default_UC
    Inherits System.Web.UI.UserControl

    '----- Gestione della pagina transazionale
    Dim EseguitaOperazione As Boolean
    Dim PremutoAnnulla As Boolean

    '----- variabili globali
    Dim Operazione As Integer
    Dim Messaggio As String = ""

    Shared _elencoCausali As List(Of CausaleDocumento) = Nothing

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        inizializzoObjParametri()
        InizializzaCausaliDocumento()

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

    Private Sub InizializzaCausaliDocumento()

        Dim objDefault As New Documento_Default_BIZ
        _elencoCausali = objDefault.ElencoCausali

    End Sub

    Private Shared Function gettimesep() As String
        Return System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.TimeSeparator
    End Function

    Public Shared Function CaricaDocumentiDefault(ByVal piva As String) As RispostaStandard

        Dim r = New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim defaultR As New Documento_Default_R
            Dim dtDefault = defaultR.Leggi(piva, Nothing, "", Nothing, Nothing,
                                Nothing, Nothing, "", "",
                                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta,
                                objParametri_Server)

            Dim defaults = (From d In dtDefault.AsEnumerable()
                            Select New Documento_Default_Model With
                            {
                               .Piva = d.Item("Piva"),
                               .CausaleDoc_Cod = CInt(d.Item("Causale_Cod")),
                               .CausaleDoc_Descr = d.Item("CausaleDoc_Descr"),
                               .Lav_Cod = CInt(d.Item("Lav_Cod")),
                               .Lav_Descr = d.Item("Lav_Descr"),
                               .NumTipo_Cod = CInt(d.Item("NumTipo_Cod")),
                               .NumTipo_Descr = d.Item("NumTipo_Descr"),
                               .Sa_Cod = CInt(d.Item("Sa_Cod")),
                               .Sa_Descr = d.Item("Sa_Descr"),
                               .Sezionale_Cod = CInt(d.Item("Sezionale_Cod")),
                               .Sezionale_Descr = d.Item("Sezionale_Descr"),
                               .Vincolante = CBool(d.Item("Vincolante")),
                                .TipoFattura_Cod = d.Item("TipoFattura_Cod"),
                                .TipoFattura_Descr = d.Item("TipoFattura_Descr")
                            }).ToList()


            defaults.ForEach(Function(s)
                                 s.Key_Default = String.Format("{0} - {1} - {2} - {3}",
                                                s.Piva, s.Sa_Cod, s.Lav_Cod, s.TipoFattura_Cod)
                             End Function)

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(defaults, Newtonsoft.Json.Formatting.None)

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
        AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    Public Shared Function CaricaDropDownNumeratoriPS_Default(ByVal piva As String) As RispostaStandard

        Dim r = New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim cauTraspR = New Causali_Trasporto_R
        Dim dtCauTrasp As DataTable = Nothing

        Dim centriAzR As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
        Dim dtCentriAz As DataTable = Nothing

        Dim sezionaliR As New Imprese_Sezionali_R
        Dim dtSezionali As DataTable = Nothing

        Dim numeratoriR As New Numeratore_Tipo_R
        Dim dtNumeratori As DataTable = Nothing

        Dim tipiDocR As New Operazioni_R
        Dim dtTipiDoc As DataTable = Nothing

        Try


            dtCauTrasp = cauTraspR.Leggi(0, "", "", -1, enum_Tipo_CausaliTrasporto.Non_Impostato, -1, "", " Causale_Trasporto_Des ASC", objParametri_Server)
            dtCentriAz = centriAzR.Leggi(piva, 0, 0, "", " Sa_Nome ASC", objParametri_Server)
            dtSezionali = sezionaliR.Leggi(piva, 0, "", " Sezionale_Des ASC", objParametri_Server)
            dtNumeratori = numeratoriR.Leggi(piva, 0, "", "", " Descrizione ASC", objParametri_Server)
            dtTipiDoc = tipiDocR.Leggi(0, 0, 0, "", 0, "", "", False, False, False, False,
                            AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                            " gru_op In (6,10,11) ", " Lav_Des ASC", objParametri_Server)

            Dim causaliTrasporto = (From ct In dtCauTrasp.AsEnumerable()
                                    Select New With
                                   {
                                        .CausaleDoc_Cod = CInt(ct.Item("Causale_Trasporto_Cod")),
                                        .CausaleDoc_Descr = If(ct.Item("Causale_Trasporto_Des") Is DBNull.Value, "", ct.Item("Causale_Trasporto_Des").ToString()),
                                        .Sigla = If(ct.Item("Causale_Trasporto_Sigla") Is DBNull.Value, "", ct.Item("Causale_Trasporto_Sigla").ToString()),
                                        .DocumentiValidi = AgronicaCoreContabHLP.Contabilita.CausaliTrasportoXLavCod(ct.Item("Tipo"), dtTipiDoc)
                                   }).ToList
            causaliTrasporto.Insert(0, New With {.CausaleDoc_Cod = -99, .CausaleDoc_Descr = "", .Sigla = "", .DocumentiValidi = New Integer() {}})

            Dim centriAziendali = (From ca In dtCentriAz.AsEnumerable()
                                   Select New With
                                      {
                                            .Sa_Cod = CInt(ca.Item("Sa_cod")),
                                            .Sa_Descr = ca.Item("Sa_Nome").ToString()
                                      }).ToList()
            centriAziendali.Insert(0, New With
                                   {
                                        .Sa_Cod = CInt(-1),
                                        .Sa_Descr = "* Tutti *"
                                   })


            Dim sezionali = (From s In dtSezionali.AsEnumerable()
                             Select New With
                                    {
                                        .Sezionale_Cod = CInt(s.Item("Sezionale_Cod")),
                                        .Sezionale_Descr = If(s.Item("Sezionale_Des") Is DBNull.Value, "", s.Item("Sezionale_Des").ToString())
                                    }).ToList
            sezionali.Insert(0, New With {.Sezionale_Cod = -99, .Sezionale_Descr = ""})

            Dim numeratoriTipo = (From num In dtNumeratori.AsEnumerable()
                                  Select New With
                               {
                                    .NumTipo_Cod = CInt(num.Item("Tipo")),
                                    .NumTipo_Descr = num.Item("Descrizione").ToString()
                               }).ToList()
            numeratoriTipo.Insert(0, New With {.NumTipo_Cod = -99, .NumTipo_Descr = ""})

            Dim tipiDocumento = (From num In dtTipiDoc.AsEnumerable()
                                 Select New With
                            {
                                .Lav_Cod = CInt(num.Item("Lav_Cod")),
                                .Lav_Descr = num.Item("Lav_Des").ToString()
                            }).ToList()
            'tipiDocumento.Insert(0, New With {.Lav_Cod = -10002, .Lav_Descr = "- TRASFERIMENTI -"})
            tipiDocumento.Insert(0, New With {.Lav_Cod = -10001, .Lav_Descr = "- VENDITE -"})
            tipiDocumento.Insert(0, New With {.Lav_Cod = -10000, .Lav_Descr = "- ACQUISTI -"})
            tipiDocumento.Insert(0, New With {.Lav_Cod = 0, .Lav_Descr = "* Tutti *"})

            Dim risposta = New With {
                                causaliTrasporto,
                                centriAziendali,
                                sezionali,
                                numeratoriTipo,
                                tipiDocumento
                            }

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(risposta, Formatting.None)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    Public Shared Function AggiornaDocumentiDefault(ByVal paramString As String) As RispostaStandard

        Dim r = New RispostaStandard
        Dim parametri As ParametriSalvaModel = Nothing

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim objDefaultW As New Documento_Default_W

        Dim Validita_Inizio As DateTime = AGRODATAINIZIO
        Dim Validita_Fine As DateTime = AGRODATAFINE
        Dim NumTipo_Cod As Integer? = Nothing
        Dim Sezionale_Cod As Integer? = Nothing
        Dim CausaleDoc_Cod As Integer? = Nothing

        Try

            parametri = JsonConvert.DeserializeObject(Of ParametriSalvaModel)(paramString)

            'tutti per controlli univocità
            Dim tutti As List(Of Documento_Default_Model) = New List(Of Documento_Default_Model)()
            tutti.AddRange(JsonConvert.DeserializeObject(Of List(Of Documento_Default_Model))(parametri.TutteLeRighe, settingLoc))
            tutti.ForEach(Function(d)
                              Dim piva As String = If(String.IsNullOrEmpty(d.Piva), parametri.Piva, d.Piva)
                              d.Key_Default = String.Format("{0}-{1}-{2}-{3}", piva, d.Sa_Cod, d.Lav_Cod, d.TipoFattura_Cod)
                          End Function)


            Dim chiaviRaggruppate = (From d In tutti
                                     Order By d.Key_Default Ascending
                                     Group By chiave = d.Key_Default
                                    Into dRagg = Group, Count())

            If chiaviRaggruppate.Any(Function(s) s.Count > 1) Then
                r.RispostaOK = False
                r.RispostaStringa = "Non è possibile definire più di un Default con stesso Centro Aziendale / Tipo Documento / Tipo Fattura"
                Return r
            End If

            'controllo se presenti conteporanemante dei default con:
            ' centro aziendale = -1 / lav_cod != 0 
            ' centro aziendale != -1 / lav_cod = 0
            Dim incompatibili As New List(Of Documento_Default_Model)
            incompatibili.AddRange(tutti.Where(Function(s) s.Sa_Cod = -1 AndAlso s.Lav_Cod <> 0).Take(1))
            incompatibili.AddRange(tutti.Where(Function(s) s.Sa_Cod <> -1 AndAlso s.Lav_Cod = 0).Take(1))
            If incompatibili.Count >= 2 Then
                r.RispostaOK = False
                r.RispostaStringa = "Non è possibile salvare questo Default. ??????"
                Return r
            End If

            'inseriti
            If Not String.IsNullOrEmpty(parametri.RigheInserite) Then
                Dim inseriti As List(Of Documento_Default_Model) = New List(Of Documento_Default_Model)()
                inseriti.AddRange(JsonConvert.DeserializeObject(Of List(Of Documento_Default_Model))(parametri.RigheInserite, settingLoc))
                For Each m As Documento_Default_Model In inseriti

                    ' Controllo su Gruppi documenti / documenti puntuali per ogni Sa_Cod
                    Dim defaultsStessoCentro = tutti.Where(Function(s) s.Sa_Cod = m.Sa_Cod)
                    If m.Lav_Cod = -10001 OrElse m.Lav_Cod = -10000 Then
                        Dim type As String = If(m.Lav_Cod = 10001, "V", "A")
                        Dim causaliDelGruppo = _elencoCausali.Where(Function(s) s.TYPE = type) _
                                                    .Select(Function(s) s.LAV_COD).ToList()

                        For Each c As String In causaliDelGruppo
                            If defaultsStessoCentro.Any(Function(s) s.Lav_Cod = c) Then
                                r.RispostaOK = False
                                r.RispostaStringa = "Non è possibile salvare un Default in quanto ne esiste gia uno più specifico "
                            End If
                        Next

                    Else

                        For Each d As Documento_Default_Model In defaultsStessoCentro
                            If d.Lav_Cod = -10001 OrElse d.Lav_Cod = -10000 Then
                                Dim type As String = If(d.Lav_Cod = 10001, "V", "A")
                                Dim causaliDelGruppo = _elencoCausali.Where(Function(s) s.TYPE = type) _
                                                            .Select(Function(s) s.LAV_COD).ToList()
                                If causaliDelGruppo.Contains(m.Lav_Cod) Then
                                    r.RispostaOK = False
                                    r.RispostaStringa = "Non è possibile salvare questo Default in quanto ne esiste già uno definito con un gruppo di causali documento che contiene la causale specificata"

                                End If

                            End If
                        Next

                    End If

                    If Not String.IsNullOrEmpty(r.RispostaStringa) Then
                        Return r
                    End If

                    If m.Validita_Inizio IsNot Nothing Then Validita_Inizio = m.Validita_Inizio
                    If m.Validita_Fine IsNot Nothing Then Validita_Fine = m.Validita_Fine
                    If m.NumTipo_Cod <> -99 Then NumTipo_Cod = m.NumTipo_Cod
                    If m.CausaleDoc_Cod <> -99 Then CausaleDoc_Cod = m.CausaleDoc_Cod
                    If m.Sezionale_Cod <> -99 Then Sezionale_Cod = m.Sezionale_Cod

                    objDefaultW.Scrivi(parametri.Piva, m.Sa_Cod, m.Lav_Cod,
                                m.TipoFattura_Cod, NumTipo_Cod, Sezionale_Cod, CausaleDoc_Cod,
                                m.Vincolante, Validita_Inizio, Validita_Fine,
                                "", "", objParametri_Server)

                Next

            End If

            'modificati
            If Not String.IsNullOrEmpty(parametri.RigheModificate) Then
                Dim modificati As List(Of Documento_Default_Model) = New List(Of Documento_Default_Model)()
                modificati.AddRange(JsonConvert.DeserializeObject(Of List(Of Documento_Default_Model))(parametri.RigheModificate, settingLoc))
                For Each m As Documento_Default_Model In modificati

                    If m.Validita_Inizio IsNot Nothing Then Validita_Inizio = m.Validita_Inizio
                    If m.Validita_Fine IsNot Nothing Then Validita_Fine = m.Validita_Fine
                    If m.NumTipo_Cod <> -99 Then NumTipo_Cod = m.NumTipo_Cod
                    If m.CausaleDoc_Cod <> -99 Then CausaleDoc_Cod = m.CausaleDoc_Cod
                    If m.Sezionale_Cod <> -99 Then Sezionale_Cod = m.Sezionale_Cod

                    objDefaultW.Aggiorna(m.Piva, m.Sa_Cod, m.Lav_Cod,
                                        m.TipoFattura_Cod, NumTipo_Cod, Sezionale_Cod, CausaleDoc_Cod,
                                        m.Vincolante, Validita_Inizio, Validita_Fine,
                                        "", objParametri_Server)

                Next
            End If

            'eliminati
            If Not String.IsNullOrEmpty(parametri.RigheCancellate) Then
                Dim cancellati As List(Of Documento_Default_Model) = New List(Of Documento_Default_Model)()
                cancellati.AddRange(JsonConvert.DeserializeObject(Of List(Of Documento_Default_Model))(parametri.RigheCancellate, settingLoc))
                For Each m As Documento_Default_Model In cancellati

                    'TODO Check se utilizzato prima di eliminarlo ???
                    objDefaultW.Cancella(m.Piva, m.Sa_Cod, m.Lav_Cod, m.TipoFattura_Cod, "", objParametri_Server)

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
    Private Class Documento_Default_Model

        Public Key_Default As String
        Public Piva As String
        Public Sa_Cod As Integer?
        Public Sa_Descr As String
        Public Sezionale_Cod As Integer?
        Public Sezionale_Descr As String
        Public CausaleDoc_Cod As Integer?
        Public CausaleDoc_Descr As String
        Public NumTipo_Cod As Integer?
        Public NumTipo_Descr As String
        Public Lav_Cod As Integer?
        Public Lav_Descr As String
        Public TipoFattura_Cod As String
        Public TipoFattura_Descr As String
        Public Vincolante As Boolean?
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


End Class
