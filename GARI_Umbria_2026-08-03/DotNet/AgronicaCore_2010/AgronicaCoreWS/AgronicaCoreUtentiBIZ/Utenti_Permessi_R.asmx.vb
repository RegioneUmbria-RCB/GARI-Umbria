Imports System.ComponentModel
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDTOStd.InData.Metaschema
Imports AgronicaCoreModello
Imports AgronicaCoreModelsSTD.utente
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq


' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class Utenti_Permessi_R
    Inherits System.Web.Services.WebService
    Private Class JSON_TipologiaxPermesso_Result
        Public Tipologia_Cod As Integer
        Public Permessi()
    End Class

    Private Class JSON_TipologiaxUtente_Result
        Public Tipologia_Cod As Integer
        Public UserName As String
        Public LeggiUtenti As Boolean = False
    End Class

    Private Function GetObjParams(Of T)(inData As CoreWS_Generic(Of T)) As ObjParams
        Return New ObjParams With {
            .ObjParametri_SuperServer = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(inData.objP.objP_super_server),
            .ObjParametri_Server = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(inData.objP.objP_server),
            .ObjParametri_Utenti = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(inData.objP.objP_utenti)
        }
    End Function

    <WebMethod()>
    Public Function HelloWorld() As String
        Return "Hello World"
    End Function

    ''' <summary>
    ''' Verifica del singolo permesso sull'utente
    ''' </summary>
    ''' <param name="UserName"></param>
    ''' <param name="Id_Servizio"></param>
    ''' <param name="Id_Attivita"></param>
    ''' <param name="Id_Operazione"></param>
    ''' <param name="DataOraControllo"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="objParametri_Utenti"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Controlla_Permessi_Utente(
        ByVal UserName As String,
        ByVal Id_Servizio As Integer,
        ByVal Id_Attivita As Integer,
        ByVal Id_Operazione As Integer,
        ByVal DataOraControllo As DateTime,
        ByVal xFiltroAggiuntivo As String,
        ByVal objParametri_Utenti As String
    ) As String

        Dim objParametri_Utenti1 As AgronicaCoreDataProvider.AgronicaCoreParametri =
            Utility.convertStringtoOBJparametri(objParametri_Utenti)

        Dim oLeggiPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Return oLeggiPermessi.Controlla_Permessi_Utente(
            UserName,
            Id_Servizio,
            Id_Attivita,
            Id_Operazione,
            DataOraControllo,
            "",
             objParametri_Utenti1
        ).ToString


    End Function

    ''' <summary>
    ''' Verifica del singolo permesso sull'utente
    ''' </summary>    
    ''' <param name="Id_Servizio"></param>
    ''' <param name="Id_Attivita"></param>
    ''' <param name="Id_Operazione"></param>
    ''' <param name="DataOraControllo"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="objParametri_Utenti"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Controlla_Permessi_Utente(
        ByVal Id_Servizio As Integer,
        ByVal Id_Attivita As Integer,
        ByVal Id_Operazione As Integer,
        ByVal DataOraControllo As DateTime,
        ByVal xFiltroAggiuntivo As String,
        ByVal objParametri_Utenti As String
    ) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Utenti1 As AgronicaCoreDataProvider.AgronicaCoreParametri =
            Utility.convertStringtoOBJparametri(objParametri_Utenti)

        Try


            Dim oLeggiPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

            r.RispostaOK = True
            r.RispostaStringa =
                oLeggiPermessi.Controlla_Permessi_Utente(
                    objParametri_Utenti1.UtenteUsername,
                    Id_Servizio,
                    Id_Attivita,
                    Id_Operazione,
                    DataOraControllo,
                    "",
                     objParametri_Utenti1
                ).ToString.ToLower

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            r.RispostaOK = False
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Controlla_Permessi_UtenteR(
       ByVal UserName As String,
       ByVal Id_Servizio As Integer,
       ByVal Id_Attivita As Integer,
       ByVal Id_Operazione As Integer,
       ByVal DataOraControllo As Date,
       ByVal xFiltroAggiuntivo As String,
       ByVal objP_utenti As String
   ) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)
        Try
            Dim oLeggiPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            Dim result = oLeggiPermessi.Controlla_Permessi_Utente(
                UserName,
                Id_Servizio,
                Id_Attivita,
                Id_Operazione,
                DataOraControllo,
                "",
                 objParametri_Utenti
            ).ToString()

            r.RispostaOK = True
            r.RispostaStringa = result
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    ''' <summary>
    ''' Carica la gerarchia dei permessi totale o riferita a una tipologia (profilo utente).
    ''' </summary>
    ''' <remarks>
    ''' Usata in:
    ''' <list type="bullet">Caricamento permessi associati a una determinata tipologia. <tt>Tipologia_Cod</tt> in InData è valorizzato.</list>
    ''' <list type="bullet">Caricamento della gerarchia permessi completa: per la visualizzazione e modifica dei
    '''      permessi dell'installazione. Vengono caricati anche i  permessi non attivi a livello di installazione.
    '''      <tt>Tipologia_Cod</tt> in InData NON è valorizzato.</list>
    ''' </remarks>
    ''' <returns>RispostaStandard contente una lista di Utente_Permesso_Gerarchia</returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Carica_Gerarchia_Permessi_NG(InData As CoreWS_Generic(Of Object))
        Dim res As New RispostaStandard
        Dim params = GetObjParams(InData)
        Dim args = JsonConvert.DeserializeObject(Of JSON_TipologiaxUtente_Result)(JsonConvert.SerializeObject(InData.InData))

        Dim read As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R
        Dim objTipologiaBIZ = New AgronicaCoreUtentiBIZ.Tipologie

        Dim result As New With {
            .Permessi = New List(Of Object),
            .Utenti = New List(Of Object)
        }

        Dim Permessi As IEnumerable(Of Object)
        Dim Utenti As IEnumerable(Of Object) = New List(Of Object)
        If args.LeggiUtenti Then
            Dim utentiRead As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R
            Utenti = utentiRead.Leggi_anchePermessi(
                enum_Id_Servizio.GiasOnline, -1, enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                "", "", params.objParametri_Utenti
            ).Select.AsParallel.
            Select(Function(row) New With {
                .Username = row("UserName"),
                .Tipologia_Cod = row("Tipologia_Cod"),
                .Tipologia_Des = row("Tipologia_Des"),
                .Validita_Inizio = row("Validita_Inizio"),
                .Validita_Fine = row("Validita_Fine")
            })
        End If

        If args.Tipologia_Cod = Nothing OrElse args.Tipologia_Cod = -1 Then
            Dim permessiBiz As New AgronicaCoreUtentiBIZ.Utenti_Permessi_R
            Permessi = permessiBiz.Carica_Permessi_Gerarchia_Completa(params)
        Else
            Permessi = objTipologiaBIZ.Carica_PermessixTipologia(args.Tipologia_Cod, params)
            Utenti = Utenti.Where(Function(u) u.Tipologia_Cod = args.Tipologia_Cod)
        End If

        If args.UserName IsNot Nothing AndAlso args.UserName <> "" Then
            Utenti = Utenti.Where(Function(u) u.UserName = args.UserName)
        End If

        result.Permessi = Permessi.ToList
        result.Utenti = Utenti.ToList

        res.RispostaOK = True
        res.RispostaStringa = JsonConvert.SerializeObject(result, Formatting.None)

        Return res
    End Function

    ''' <summary>
    ''' Carica i permessi attivi a livello di installazione.
    ''' </summary>
    ''' <remarks>
    ''' Usata nel caricamento della gerarchia "completa" di permessi attribuibili ai profili.
    ''' I permessi caricati sono comunque filtrati tra quelli attivi nell'installazione.
    ''' </remarks>
    ''' <returns>RispostaStandard contente una lista di Utente_Permesso_Gerarchia</returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Carica_Gerarchia_Permessi_Attivi(InData As CoreWS_Generic(Of String))
        Dim res As New RispostaStandard
        Dim params = GetObjParams(InData)
        Dim utentiBIZ As New AgronicaCoreUtentiBIZ.Utenti
        Dim objPermessoDal As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim objPermessoBiz As New AgronicaCoreUtentiBIZ.Utenti_Permessi_R
        Try
            Dim Permessi As IEnumerable(Of Utente_Permesso_Gerarchia) = objPermessoBiz.Carica_Permessi_Gerarchia(params, True)
            Dim result = Permessi.ToList

            If objPermessoDal.VerificaEsistenzaTabellaClientePermessi(params.objParametri_Utenti) Then
                Dim idAttivi As IEnumerable(Of Integer) = utentiBIZ.LeggiCliente_Permessi(
                    params.objParametri_Utenti.SuperUserUsername, params.objParametri_Utenti
                ).Select(Function(a) a.Permesso_ID).ToHashSet
                'Se tabella Cliente_Permessi è vuota non vedo nulla
                result = Permessi.Where(Function(p) idAttivi.Contains(p.Permesso_ID)).ToList
            End If

            res.RispostaOK = True
            res.RispostaStringa = JsonConvert.SerializeObject(result, Formatting.None)
        Catch ex As Exception
            res.RispostaOK = False
        End Try
        Return res
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function LeggiCliente_Permessi(InData As CoreWS_Generic(Of String)) As rispostaStandard(Of List(Of Cliente_Permesso))
        Dim r As New rispostaStandard(Of List(Of Cliente_Permesso))
        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If
        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If
        If InData.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        Try
            Dim objUtenti As New AgronicaCoreUtentiBIZ.Utenti
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
            Dim username As String = InData.InData
            'Dopo aver letto le funzionalità considero solo quelle con Id_Operazione con permesso maggiore
            'in modo tale che se ho una funzionalità con permessi in lettura e scrittura mi mantiene solo
            'il record indicante il permesso di scrittura (do per scontato che se posso scrivere posso anche leggere)
            Dim result = objUtenti.LeggiCliente_Permessi(username, objParametri_Utenti).
                GroupBy(Function(f) f.Id_Attivita).
                Select(Function(grouping) grouping.OrderBy(Function(f) f.Id_Operazione).Last).
                ToList
            r.RispostaOK = True
            r.RispostaStringa = result
        Catch ex As NotSupportedException
            r.RispostaOK = True
            r.RispostaStringa = New List(Of Cliente_Permesso)
        Catch ex As Exception
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try
        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ControllaEsistenzaTabellaCliente_Permessi(InData As CoreWS_Generic(Of String)) As rispostaStandard(Of Boolean)
        Dim r As New rispostaStandard(Of Boolean)
        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If
        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If
        If InData.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        Try
            Dim objUtenti As New AgronicaCoreUtentiBIZ.Utenti
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
            Dim result = objUtenti.VerificaEsistenzaCliente_Permessi(objParametri_Utenti)

            r.RispostaOK = True
            r.RispostaStringa = result
        Catch ex As Exception
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try
        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Carica_Gerarchia_Permessi(data As Object, objParametri_Server As String, objParametri_Utenti As String)
        Dim res As New RispostaStandard
        Dim params As New ObjParams With { 'TODO: non ci sono info su ObjParametri_SuperServer?
            .ObjParametri_SuperServer = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objParametri_Server),
            .ObjParametri_Server = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objParametri_Server),
            .objParametri_Utenti = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objParametri_Utenti)
        }
        Dim args = JsonConvert.DeserializeObject(Of JSON_TipologiaxUtente_Result)(data)

        Dim objUtenti = New AgronicaCoreUtentiBIZ.Utenti
        Dim objTipologiaBIZ = New AgronicaCoreUtentiBIZ.Tipologie

        Dim result As New With {
            .Permessi = New List(Of Object),
            .Utenti = New List(Of Object)
        }

        Dim Permessi As IEnumerable(Of Object)
        Dim TB_Utenti As DataTable = objUtenti.Carica_Utenti(String.Empty, params)
        Dim Utenti As IEnumerable(Of Object) = (From row In TB_Utenti.Rows
                                                Select (New With {
                                                      .UserName = row(0),
                                                      .Tipologia_Cod = row(20),
                                                      .Tipologia_Des = row(21),
                                                      .Validita_Inizio = row(13),
                                                      .Validita_Fine = row(14)
                                                })).ToList

        If args.Tipologia_Cod = Nothing OrElse args.Tipologia_Cod = -1 Then
            Permessi = objTipologiaBIZ.Carica_Permessi_Gerarchia(params.ObjParametri_Server, params.objParametri_Utenti)
        Else
            Permessi = objTipologiaBIZ.Carica_PermessixTipologia(args.Tipologia_Cod, params.ObjParametri_Server, params.objParametri_Utenti)
            Utenti = Utenti.Where(Function(u)
                                      Return u.Tipologia_Cod = args.Tipologia_Cod
                                  End Function)
        End If


        If args.UserName IsNot Nothing AndAlso args.UserName <> "" Then
            Utenti = Utenti.Where(Function(u)
                                      Return u.UserName = args.UserName
                                  End Function)
        End If


        result.Permessi = Permessi.ToList
        result.Utenti = Utenti.ToList

        res.RispostaOK = True
        res.RispostaStringa = JsonConvert.SerializeObject(result, Formatting.None)

        Return res
    End Function


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Get_Permessi_APP(ByVal objP_server As String, ByVal objP_utenti As String) As RispostaStandard

        Dim r As New RispostaStandard()

        Dim permessiAPP As New PermessiAPP

        If objP_utenti = "" Then
            r.Errore = "objP_Utenti non valorizzato"
            Return r
        End If

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

        Try

            Dim leggiPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            Dim f = Function(attivita As enum_Security_Attivita) As Integer
                        Dim s = leggiPermessi.Controlla_Permessi_Utente(objParametri_Utenti.UtenteUsername, enum_Id_Servizio.GiasOnline, attivita, enum_Security_Operazione.Modifica, Date.Now, "", objParametri_Utenti)
                        If s Then Return 1
                        Dim l = leggiPermessi.Controlla_Permessi_Utente(objParametri_Utenti.UtenteUsername, enum_Id_Servizio.GiasOnline, attivita, enum_Security_Operazione.Lettura, Date.Now, "", objParametri_Utenti)
                        Return If(l, 2, 0)
                    End Function

            permessiAPP = New PermessiAPP With {
                .BloccoPermessi = f(enum_Security_Attivita.GiasAPP_Permessi) = 0,
                .Ricette = f(enum_Security_Attivita.GiasAPP_NUOVA_RICETTA),
                .Attivita = f(enum_Security_Attivita.GiasAPP_NUOVO_INTERVENTO),
                .AttivitaPianificate = f(enum_Security_Attivita.GiasAPP_INTERVENTI_DA_FARE),
                .ScaricoOre = f(enum_Security_Attivita.GiasAPP_SCARICO_ORE),
                .EntrataUscita = f(enum_Security_Attivita.GiasAPP_ENTRATAUSCITA),
                .LaMiaPosizione = f(enum_Security_Attivita.GiasAPP_LAMIAPOSIZIONE),
                .Visite = f(enum_Security_Attivita.GiasAPP_VISITE),
                .Documenti = f(enum_Security_Attivita.GiasAPP_DOCUMENTI),
                .Rilievi = f(enum_Security_Attivita.GiasAPP_RILIEVI),
                .GIS = f(enum_Security_Attivita.GiasAPP_GIS),
                .InCab = f(enum_Security_Attivita.GiasAPP_InCab),
                .PianoColturale = f(enum_Security_Attivita.GiasAPP_PianoColturale),
                .Magazzini = f(enum_Security_Attivita.GiasAPP_Magazzini),
                .Macchine = f(enum_Security_Attivita.GiasAPP_Macchine),
                .Manutenzioni = f(enum_Security_Attivita.GiasAPP_Manutenzioni),
                .DDT_Movimenti = f(enum_Security_Attivita.GiasAPP_DDT_Movimenti),
                .Gias = f(enum_Security_Attivita.GiasAPP_Gias),
                .Aziende = f(enum_Security_Attivita.GiasAPP_Aziende),
                .Centri = f(enum_Security_Attivita.GiasAPP_Centri),
                .Isolamenti = f(enum_Security_Attivita.GiasAPP_Isolamenti),
                .DSS_Difesa = f(enum_Security_Attivita.GiasAPP_DSS_Difesa),
                .Consiglio_Irriguo = f(enum_Security_Attivita.GiasAPP_Consiglio_Irriguo),
                .Consiglio_Fertirriguo = f(enum_Security_Attivita.GiasAPP_Consiglio_Fertirriguo),
                .Precision_Farming = f(enum_Security_Attivita.GiasAPP_Precision_Farming),
                .Monitoraggio_Meteo = f(enum_Security_Attivita.GiasAPP_Monitoraggio_Meteo),
                .Lavoratori = f(enum_Security_Attivita.GiasAPP_Lavoratori),
                .Widget_Rischi_Meteo = f(enum_Security_Attivita.GiasAPP_Widget_Rischi_Meteo),
                .Widget_Rischi_Difesa = f(enum_Security_Attivita.GiasAPP_Widget_Rischi_Difesa),
                .Widget_Consiglio_Semina = f(enum_Security_Attivita.GiasAPP_Widget_Consiglio_Semina),
                .Widget_Consiglio_Nutrizione = f(enum_Security_Attivita.GiasAPP_Widget_Consiglio_Nutrizione),
                .Widget_Consiglio_Irriguo = f(enum_Security_Attivita.GiasAPP_Widget_Consiglio_Irriguo),
                .Widget_Consiglio_Raccolta = f(enum_Security_Attivita.GiasAPP_Widget_Consiglio_Raccolta),
                .Chatbot = f(enum_Security_Attivita.GiasAPP_Chatbot),
                .Allarmi_Widget = f(enum_Security_Attivita.GiasAPP_Allarmi_Widget),
                .Dati_Meteo_Storici = f(enum_Security_Attivita.GiasAPP_Dati_Meteo_Storici),
                .Indici_Satellitari = f(enum_Security_Attivita.GiasAPP_Indici_Satellitari),
                .Geofoto = f(enum_Security_Attivita.GiasAPP_Geofoto),
                .Consultazione_Dati_Sensori = f(enum_Security_Attivita.GiasAPP_Consultazione_Dati_Sensori),
                .Squadre_Lavoratori = f(enum_Security_Attivita.GiasAPP_Squadre_Lavoratori),
                .Creazione_Fornitore = f(enum_Security_Attivita.GiasAPP_Creazione_Fornitore),
                .Gestione_Tracce = f(enum_Security_Attivita.GiasAPP_Gestione_Tracce),
                .Gestione_Fasi_Fenologiche = f(enum_Security_Attivita.GiasAPP_Gestione_Fasi_Fenologiche),
                .Gestione_Trappole = f(enum_Security_Attivita.GiasAPP_Gestione_Trappole),
                .Notizie_Coldiretti = f(enum_Security_Attivita.GiasAPP_Notizie_Coldiretti),
                .Messaggi = f(enum_Security_Attivita.GiasAPP_Messaggi),
                .Promemoria = f(enum_Security_Attivita.GiasAPP_Promemoria),
                .Registrazione_Rilievi_Pedologici = f(enum_Security_Attivita.GiasAPP_Registrazione_Rilievi_Pedologici),
                .Modulo_BeLeaf = f(enum_Security_Attivita.GiasAPP_Modulo_BeLeaf)
            }

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(permessiAPP, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

End Class

