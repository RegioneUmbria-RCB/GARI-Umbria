Imports System.Web.Services

Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.LogProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.DataProvider

Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreVarieBIZ
Imports System.Xml
Imports Newtonsoft.Json
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreAnagrafeDAL.ContattiXIndirizzi_R
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreContabDAL
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreMetaSchemaDAL
Imports AgronicaCoreEntityFramework_POCO
Imports AgroAgenda_2010.Resources
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreDataProvider.My.Resources

Public Class New_Contatto_Edit
    Inherits Page

    Dim objParametri_Server, objParametri_Utenti, objParametri_Super_Server As AgronicaCoreParametri
    Public permessi As PermessiUtente

    Dim xPiva As String
    Dim xSa_Cod As String
    Dim xCod_Contatto As String
    Public XTipoOperazione As Integer
    Public xId_Cf As Integer = -1
    Dim Qs_Visibilita As Integer = 0

    Dim xValiditaInizio As Date
    Dim xValiditaFine As Date

    Dim BaseCode As Integer
    Dim TopCode As Integer

    Public jsIndirizzi As String
    Public jsRapporti As String
    Public jsRubrica As String
    Public jsIndirizziNew As String
    Public jsCostiNew As String
    Public jsLiquidita As String
    Public jsConti As String

    Dim Qs_Operazione As String
    Dim Qs_Piva As String
    Dim Qs_Rapporto_Cod As Integer
    Public Shared Qs_Origine As String
    Dim Qs_CodContatto As String
    Dim Qs_LavCod As Integer
    Dim Qs_IdCf As Integer = -1
    Public Shared Qs_apertodaGiasNG As String

    Dim ApriDatiPatentino As Boolean = False
    Public Shared AperturaDaPopup As Boolean = False

    Private Const LEN_MAX_MEMO As Integer = 8000

    Private Shared ReadOnly _listTipiIndirizziGiu As New List(Of Integer) From {
        INDIRIZZO_SEDE_LEGALE, INDIRIZZO_SEDE_OPERATIVA, INDIRIZZO_SEDE_AZIENDALE, INDIRIZZO_STABILIMENTO
    }
    Private Shared ReadOnly _listTipiIndirizziEst As New List(Of Integer) From {
        INDIRIZZO_SEDE_LEGALE, INDIRIZZO_SEDE_OPERATIVA, INDIRIZZO_SEDE_AZIENDALE, INDIRIZZO_STABILIMENTO, INDIRIZZO_STABILE_ORGANIZZAZIONE
    }
    Private Shared ReadOnly _listTipiIndirizziFis As New List(Of Integer) From {
        INDIRIZZO_RESIDENZA, INDIRIZZO_DOMICILIO, INDIRIZZO_RESIDENZA_ESTIVA
    }

    Public ReadOnly Property PageViewState As StateBag

        Get
            Return ViewState
        End Get

    End Property

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()

    End Sub

    <WebMethod(EnableSession:=True)>
    Public Shared Function DropDownRapportoContabilePrincipale2(ByVal tipoContatto As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim ObjRapp As New AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R
        Dim rapportiContabili = ObjRapp.Contatti_RapportiContabili_Leggi(SACOD_CONTATTO_NONDEFINITO,
                                                  0,
                                                  False, False, False, False, False, False, False,
                                                   AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                   "",
                                                   "",
                                                   objParametri_Server)

        Dim lista = (From d In rapportiContabili.AsEnumerable() Select New With
                                                                 {
                                                                    .Cod_Rapporto = d.Item("Cod_Rapporto").ToString(),
                                                                    .Cod_RisUm_Des = d.Item("Rapporto_Des").ToString(),
                                                                    .SA_Cod = CInt(d.Item("Sa_Cod"))
                                                                 }).ToList()

        Dim sa_cod_in_select As New List(Of Integer) From {0}
        If tipoContatto = 0 Then
            sa_cod_in_select.Add(2)
        Else
            sa_cod_in_select.Add(1)
        End If

        Dim listaFiltrata = lista.Where(Function(s) sa_cod_in_select.Contains(s.SA_Cod)).ToList()

        listaFiltrata.Insert(0, New With
                     {
                        .Cod_Rapporto = "0",
                        .Cod_RisUm_Des = AgronicaAgenda_2010.Seleziona & "...",
                        .SA_Cod = -1
                     })

        r.RispostaOK = True
        r.RispostaStringa = GetJson(listaFiltrata)

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function TabellaRapportiContabili() As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim ObjRapp As New AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R
        Dim rapportiContabili = ObjRapp.Contatti_RapportiContabili_Leggi(SACOD_CONTATTO_NONDEFINITO,
                                                  0,
                                                  False, False, False, False, False, False, False,
                                                   AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                   "",
                                                   "",
                                                   objParametri_Server)

        r.RispostaStringa = GetJson(rapportiContabili)
        r.RispostaOK = True

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function TabellaQualifiche() As RispostaStandard

        Dim r As New RispostaStandard

        If HttpContext.Current.Session("Qualifiche") IsNot Nothing Then
            r.RispostaStringa = HttpContext.Current.Session("Qualifiche").ToString()
        Else
            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Dim objQualifica As New Qualifiche_R()
            Dim qualifiche = objQualifica.Leggi(0, "", "", objParametri_Server)
            Dim rispJson = GetJson(qualifiche)

            HttpContext.Current.Session("Qualifiche") = rispJson

            r.RispostaStringa = rispJson

        End If

        r.RispostaOK = True

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaIndirizziTipo(ByVal piva As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim dtIndirizzi As New DataTable

        Dim APPLICABILITA_FISICA = AgronicaAgenda_2010.PersoneFisiche
        Dim APPLICABILITA_GIURIDICA = AgronicaAgenda_2010.PersoneGiuridiche

        Try
            Dim dal = New IndirizzoTipo_R
            dtIndirizzi = dal.LeggiEstesa(piva, -1, "-99", -1, "", "", objParametri_Server)

            Dim indirizziPredefiniti As New List(Of InidirizzoTipoModel)
            indirizziPredefiniti.Add(New InidirizzoTipoModel With
                                     {
                                        .IndirizzoTipoDes = AgronicaAgenda_2010.SedeOperativa,
                                        .InidrizzoTipoCod = 1,
                                        .ApplicabilitaCod = PERSONA_GIURIDICA,
                                        .ApplicabilitaDes = APPLICABILITA_GIURIDICA,
                                        .Cod_Contatto = "",
                                        .IndirizzoDiSistema = True
                                     })
            indirizziPredefiniti.Add(New InidirizzoTipoModel With
                                     {
                                        .IndirizzoTipoDes = AgronicaAgenda_2010.SedeLegale,
                                        .InidrizzoTipoCod = 101,
                                        .ApplicabilitaCod = PERSONA_GIURIDICA,
                                        .ApplicabilitaDes = APPLICABILITA_GIURIDICA,
                                        .Cod_Contatto = "",
                                        .IndirizzoDiSistema = True
                                     })
            indirizziPredefiniti.Add(New InidirizzoTipoModel With
                                     {
                                        .IndirizzoTipoDes = AgronicaAgenda_2010.SedeAziendale,
                                        .InidrizzoTipoCod = 102,
                                        .ApplicabilitaCod = PERSONA_GIURIDICA,
                                        .ApplicabilitaDes = APPLICABILITA_GIURIDICA,
                                        .Cod_Contatto = "",
                                        .IndirizzoDiSistema = True
                                     })
            indirizziPredefiniti.Add(New InidirizzoTipoModel With
                                     {
                                        .IndirizzoTipoDes = AgronicaAgenda_2010.Stabilimento,
                                        .InidrizzoTipoCod = 103,
                                        .ApplicabilitaCod = PERSONA_GIURIDICA,
                                        .ApplicabilitaDes = APPLICABILITA_GIURIDICA,
                                        .Cod_Contatto = "",
                                        .IndirizzoDiSistema = True
                                     })
            indirizziPredefiniti.Add(New InidirizzoTipoModel With
                                     {
                                        .IndirizzoTipoDes = DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/New_Contatto_Edit.aspx", "StabileOrganizzazione"), String),
                                        .InidrizzoTipoCod = 201,
                                        .ApplicabilitaCod = PERSONA_GIURIDICA,
                                        .ApplicabilitaDes = APPLICABILITA_GIURIDICA,
                                        .Cod_Contatto = "",
                                        .IndirizzoDiSistema = True
                                     })
            indirizziPredefiniti.Add(New InidirizzoTipoModel With
                                     {
                                        .IndirizzoTipoDes = AgronicaAgenda_2010.Domicilio,
                                        .InidrizzoTipoCod = 2,
                                        .ApplicabilitaCod = PERSONA_FISICA,
                                        .ApplicabilitaDes = APPLICABILITA_FISICA,
                                        .Cod_Contatto = "",
                                        .IndirizzoDiSistema = True
                                     })
            indirizziPredefiniti.Add(New InidirizzoTipoModel With
                                     {
                                        .IndirizzoTipoDes = AgronicaAgenda_2010.Residenza,
                                        .InidrizzoTipoCod = 3,
                                        .ApplicabilitaCod = PERSONA_FISICA,
                                        .ApplicabilitaDes = APPLICABILITA_FISICA,
                                        .Cod_Contatto = "",
                                        .IndirizzoDiSistema = True
                                     })
            indirizziPredefiniti.Add(New InidirizzoTipoModel With
                                     {
                                        .IndirizzoTipoDes = AgronicaAgenda_2010.ResidenzaEstiva,
                                        .InidrizzoTipoCod = 4,
                                        .ApplicabilitaCod = PERSONA_FISICA,
                                        .ApplicabilitaDes = APPLICABILITA_FISICA,
                                        .Cod_Contatto = "",
                                        .IndirizzoDiSistema = True
                                     })
            indirizziPredefiniti.Add(New InidirizzoTipoModel With
                                     {
                                        .IndirizzoTipoDes = AgronicaAgenda_2010.LuogoDiNascita,
                                        .InidrizzoTipoCod = 5,
                                        .ApplicabilitaCod = PERSONA_FISICA,
                                        .ApplicabilitaDes = APPLICABILITA_FISICA,
                                        .Cod_Contatto = "",
                                        .IndirizzoDiSistema = True
                                     })



            Dim indirizziPersonalizzati = New List(Of InidirizzoTipoModel)

            For Each it As DataRow In dtIndirizzi.AsEnumerable

                Dim ragSoc As String = If(it.Item("Rag_Soc") Is DBNull.Value, "", it.Item("Rag_Soc"))
                Dim cognome As String = If(it.Item("cognome") Is DBNull.Value, "", it.Item("cognome"))
                Dim nome As String = If(it.Item("nome") Is DBNull.Value, "", it.Item("nome"))

                indirizziPersonalizzati.Add(New InidirizzoTipoModel With
                                  {
                                        .IndirizzoTipoDes = it("Descrizione").ToString(),
                                        .InidrizzoTipoCod = it("IndirizzoTipo_Cod").ToString(),
                                        .Cod_Contatto = If(it.Item("Cod_Contatto") Is DBNull.Value, "", it.Item("Cod_Contatto")),
                                        .Contatto_Des = String.Format("{0}{1} {2}", Trim(ragSoc), Trim(cognome), Trim(nome)),
                                        .ApplicabilitaCod = CInt(it.Item("Contatto_Tipo"))
                                  })

            Next

            indirizziPersonalizzati.ForEach(Sub(s)
                                                s.ApplicabilitaDes = If(s.ApplicabilitaCod = 1, APPLICABILITA_GIURIDICA, APPLICABILITA_FISICA)
                                            End Sub)

            Dim tutti As List(Of InidirizzoTipoModel) = indirizziPredefiniti.Concat(indirizziPersonalizzati).ToList()
            tutti.ForEach(Sub(s)
                              s.key_tipo_indirizzo = String.Format("{0}-{1}", piva, s.InidrizzoTipoCod)
                          End Sub)

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(tutti, Newtonsoft.Json.Formatting.None)
        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaIndirizziPersonalizzati(
                                                        ByVal Piva As String,
                                                        ByVal Cod_Contatto As String,
                                                        ByVal Id_CF As Integer,
                                                        ByVal ContattoEstero As Boolean) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim dtIndirizzi As New DataTable

        Try
            Dim dal = New IndirizzoTipo_R
            dtIndirizzi = dal.Leggi(Piva, -1, Cod_Contatto, -1, "", "", objParametri_Server)

            Dim jarrayIndirizzi As New JArray()

            If Id_CF = PERSONA_GIURIDICA OrElse Id_CF = CONTATTO_ESTERO Then
                jarrayIndirizzi.Add(
                New JObject(
                        New JProperty("IndirizzoTipo_Cod", 1), New JProperty("Descrizione", AgronicaAgenda_2010.SedeOperativa)
                    ))
                jarrayIndirizzi.Add(
                    New JObject(
                            New JProperty("IndirizzoTipo_Cod", 101), New JProperty("Descrizione", AgronicaAgenda_2010.SedeLegale)
                        ))
                jarrayIndirizzi.Add(
                    New JObject(
                            New JProperty("IndirizzoTipo_Cod", 102), New JProperty("Descrizione", AgronicaAgenda_2010.SedeAziendale)
                        ))
                jarrayIndirizzi.Add(
                    New JObject(
                            New JProperty("IndirizzoTipo_Cod", 103), New JProperty("Descrizione", AgronicaAgenda_2010.Stabilimento)
                        ))
            End If

            If Id_CF = CONTATTO_ESTERO Then
                jarrayIndirizzi.Add(
                New JObject(
                        New JProperty("IndirizzoTipo_Cod", 201), New JProperty("Descrizione", DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/New_Contatto_Edit.aspx", "StabileOrganizzazione"), String))
                    ))
            End If

            If Id_CF = PERSONA_FISICA Then
                jarrayIndirizzi.Add(
                New JObject(
                        New JProperty("IndirizzoTipo_Cod", 2), New JProperty("Descrizione", AgronicaAgenda_2010.Domicilio)
                    ))
                jarrayIndirizzi.Add(
                    New JObject(
                            New JProperty("IndirizzoTipo_Cod", 3), New JProperty("Descrizione", AgronicaAgenda_2010.Residenza)
                        ))
                jarrayIndirizzi.Add(
                    New JObject(
                            New JProperty("IndirizzoTipo_Cod", 4), New JProperty("Descrizione", AgronicaAgenda_2010.ResidenzaEstiva)
                        ))
                jarrayIndirizzi.Add(
                    New JObject(
                            New JProperty("IndirizzoTipo_Cod", 5), New JProperty("Descrizione", AgronicaAgenda_2010.LuogoDiNascita)
                        ))

            End If

            For Each dr As DataRow In dtIndirizzi.Rows

                Dim descrizione = dr("Descrizione").ToString()
                Dim codice = dr("IndirizzoTipo_Cod").ToString()

                jarrayIndirizzi.Add(
                    New JObject(
                        New JProperty("IndirizzoTipo_Cod", dr("IndirizzoTipo_Cod").ToString()),
                        New JProperty("Descrizione", dr("Descrizione").ToString())))

            Next

            Dim sorted As JArray = New JArray(jarrayIndirizzi.OrderBy(Function(obj) CStr(obj("Descrizione"))))

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(sorted, Newtonsoft.Json.Formatting.None)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaRappresentantiFiscali() As RispostaStandard

        Dim r As New RispostaStandard

        Dim parametriAgenda = New ParametriAgenda()
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim piva As String = parametriAgenda.Piva
        Dim objRappFisc As New Rapporti_Contabili_R()
        Dim rf = objRappFisc.RapportiContabili_X_Documenti_Leggi(piva, True, enum_Rapporti_Contabili_Standard.Rappresentante_Fiscale, "", objParametri_Server)
        r.RispostaStringa = GetJson(rf)
        r.RispostaOK = True

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Origini_Spedizione() As RispostaStandard

        Dim r As New RispostaStandard

        Dim parametriAgenda = New ParametriAgenda()
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim piva As String = parametriAgenda.Piva
            Dim objACCDAA As New CodiciOriginiSpedione_R(objParametri_Server)
            Dim rf = objACCDAA.Leggi(0)
            r.RispostaStringa = GetJson(rf)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Tipologie_Destinazione() As RispostaStandard

        Dim r As New RispostaStandard

        Dim parametriAgenda = New ParametriAgenda()
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim piva As String = parametriAgenda.Piva
            Dim objACCDAA As New TipologiaDestinazione_R(objParametri_Server)
            Dim rf = objACCDAA.Leggi(0)
            r.RispostaStringa = GetJson(rf)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Liquidita() As RispostaStandard

        Dim r As New RispostaStandard

        Dim parametriAgenda = New ParametriAgenda()
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim xFiltro = " ChkAbilitazione = 1 "
            Dim dal = New Liquidita_R()
            Dim dt As DataTable = dal.LeggiRisFinanziarie_BYCodContatto("", 0, 0, 0, parametriAgenda.Piva, enum_Liquidita_CauRisorsa.RisorsaFinanziaria, xFiltro, "", objParametri_Server)

            Dim JArrayListaOp As New JArray()
            JArrayListaOp.Add(
                New JObject(
                        New JProperty("Codice", 0),
                        New JProperty("Descrizione", DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/New_Contatto_Edit.aspx", "RisorsaFinanziariaDefault"), String))
                    ))

            For Each dr As DataRow In dt.Rows

                Dim descrizione = OttieniDescrizioneLiquidita(dr)

                JArrayListaOp.Add(
                    New JObject(
                        New JProperty("Codice", dr("Cod_Liquidita")),
                        New JProperty("Descrizione", descrizione)))

            Next

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            r.RispostaStringa = JsonConvert.SerializeObject(JArrayListaOp, Newtonsoft.Json.Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    Private Shared Function OttieniDescrizioneLiquidita(ByVal dr As DataRow) As String

        'IBAN_Composizione = IIf(bPrefisso, "IBAN:", "") & UCase(rs("Nazione")) & rs("Cifre_Controllo") & UCase(rs("Cin")) & rs("Abi") & rs("Cab") & rs("Numero")
        'RsRisorse.Fields("Istituto_Des"))

        Return String.Format("{0}{1}{2}{3}{4}{5} - {6}",
                        dr("Nazione").ToString().ToUpper(),
                        dr("Cifre_Controllo").ToString().ToUpper(),
                        dr("Cin").ToString().ToUpper(),
                        dr("Abi").ToString().ToUpper(),
                        dr("Cab").ToString().ToUpper(),
                        dr("Numero").ToString().ToUpper(),
                        dr("Istituto_Des").ToString())

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Uffici_Dogane() As RispostaStandard

        Dim r As New RispostaStandard

        Dim parametriAgenda = New ParametriAgenda()
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim jArrayCUD = New JArray()

        Try

            Dim piva As String = parametriAgenda.Piva
            Dim objACCDAA As New UfficiDA_R(objParametri_Server)
            Dim ufficiDogane = objACCDAA.Leggi(0)

            For Each ud As ACCDAA_ANAG_TA03_UfficiDA In ufficiDogane

                Dim descrizione = String.Format("{0} - ( {1} )", ud.Descrizione, ud.Codice)
                Dim codice = ud.Codice

                jArrayCUD.Add(
                    New JObject(
                        New JProperty("Codice", codice.ToString()),
                        New JProperty("Descrizione", descrizione)))

            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayCUD, Newtonsoft.Json.Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function TipiRubrica() As RispostaStandard

        Dim r As New RispostaStandard

        Dim parametriAgenda = New ParametriAgenda
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objRappFisc As New Rapporti_Contabili_R()
        Dim rf = objRappFisc.RapportiContabili_X_Documenti_Leggi(parametriAgenda.Piva, True, enum_Rapporti_Contabili_Standard.Rappresentante_Fiscale, "", objParametri_Server)
        r.RispostaStringa = GetJson(rf)
        r.RispostaOK = True

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function TabellaMansioni() As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objMansioni As New AgronicaCoreAnagrafeDAL.Mansioni_R()
        Dim qualifiche = objMansioni.Leggi(0, "", "", objParametri_Server)
        r.RispostaStringa = GetJson(qualifiche)
        r.RispostaOK = True

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function TabellaClassificazioneRisUm() As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objClass As New Risorse_Umane_Classificazioni_R()
        Dim classificazioni = objClass.Leggi_EF("", 0, "", "", "", 0, 0, True, False, objParametri_Server, Nothing)
        r.RispostaStringa = GetJson((From c In classificazioni Select New With
                                                                  {
                                                                    .Classificazione_Cod = c.Id_RisUm_CL,
                                                                    .Classificazione_Des = c.Descrizione,
                                                                    c.Codice,
                                                                    c.Descr_Breve
                                                                  }).ToList())
        r.RispostaOK = True

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function KendoListRapportiContabiliSelezionati(
        ByVal piva As String,
        ByVal codContatto As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim dtRappCont As DataTable

        Try
            If Not HttpContext.Current.Session("RapportiContabili_X_Contatto_Leggi") Is Nothing Then
                dtRappCont = DirectCast(HttpContext.Current.Session("RapportiContabili_X_Contatto_Leggi"), DataTable)
            Else
                Dim dal = New Rapporti_Contabili_R()
                dtRappCont = dal.RapportiContabili_X_Contatto_Leggi(piva, codContatto, 0, 0, "", objParametri_Server)
                HttpContext.Current.Session("RapportiContabili_X_Contatto_Leggi") = dtRappCont
            End If

            Dim listaRappo = New List(Of Object)
            For Each rc As DataRow In dtRappCont.Rows

                Dim descrizione = rc.Item("Rapporto_Des").ToString()
                Dim cod_rapporto = rc.Item("Cod_Rapporto").ToString()
                Dim dataInizio = If(rc.Item("Validita_Inizio") Is DBNull.Value, AGRODATAINIZIO, CDate(rc.Item("Validita_Inizio")))
                Dim dataFine = If(rc.Item("Validita_Fine") Is DBNull.Value, AGRODATAINIZIO, CDate(rc.Item("Validita_Fine")))

                listaRappo.Add(New With
                    {
                        .value = cod_rapporto,
                        .text = String.Format("{0} - ({1} - {2})", descrizione, dataInizio.ToString("dd/MM/yyyy"), dataFine.ToString("dd/MM/yyyy"))
                    }
                )

            Next

            r.RispostaOK = True
            r.RispostaStringa = GetJson(listaRappo)

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function DropDownRapportiContabili(ByVal piva As String, ByVal codContatto As String) As RispostaStandard

        Dim r As New RispostaStandard

        Try
            Dim lista = InnerDropDownRapportiContabili(piva, codContatto)

            r.RispostaStringa = GetJson(lista.ToList())
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    Private Shared Function InnerDropDownRapportiContabili(
        ByVal piva As String,
        ByVal codContatto As String) As IEnumerable(Of Object)

        Dim r As New RispostaStandard
        Dim dtRappCont As DataTable

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return New List(Of Object)()
        End If

        If Not HttpContext.Current.Session("RapportiContabili_X_Contatto_Leggi") Is Nothing Then
            dtRappCont = DirectCast(HttpContext.Current.Session("RapportiContabili_X_Contatto_Leggi"), DataTable)
        Else
            Dim dal = New Rapporti_Contabili_R()
            dtRappCont = dal.RapportiContabili_X_Contatto_Leggi(piva, codContatto, 0, 0, "", objParametri_Server)
            HttpContext.Current.Session("RapportiContabili_X_Contatto_Leggi") = dtRappCont
        End If

        Dim lista = (From d In dtRappCont.AsEnumerable() Select New With
                                                                 {
                                                                    .Cod_RisUm = d.Item("Cod_RisUm").ToString(),
                                                                    .Cod_Rapporto = d.Item("Cod_Rapporto").ToString(),
                                                                    .Cod_RisUm_Des = d.Item("Rapporto_Des").ToString(),
                                                                    .Cod_Contatto = d.Item("Cod_Contatto"),
                                                                    .Validita_Inizio = d.Item("Validita_Inizio"),
                                                                    .Validita_Fine = d.Item("Validita_Fine"),
                                                                    .Dipendente = d.Item("Dipendente"),
                                                                    .Terzista = d.Item("Terzista"),
                                                                    .Legale = d.Item("Legale")
                                                                 }).ToList()


        'Dim rapportiAmmessi As Integer() = {enum_Rapporti_Contabili_Standard.Dipendente, enum_Rapporti_Contabili_Standard.Terzista}
        Dim listaFiltrata = lista.Where(Function(s) s.Dipendente = 1 OrElse s.Terzista = 1 OrElse s.Legale = 1)
        Dim listaFinale = (From d In listaFiltrata Select New With
                                                  {
                                                    d.Cod_RisUm,
                                                    .Cod_RisUm_Des = d.Cod_RisUm_Des & " - " & d.Cod_Contatto & " ( " & d.Validita_Inizio & " - " & d.Validita_Fine & ")",
                                                    d.Cod_Rapporto,
                                                    d.Validita_Inizio,
                                                    d.Validita_Fine
                                                  })
        Return listaFinale.ToList()


    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function GetConvenevoli(ByVal tipoContatto As Integer, ByVal convenevoli As String) As RispostaStandard

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim lista = New List(Of Object)
        Dim risp As String = ""
        'i18n Text è l'elemento visto dall'utente e Value è quello passato al database? Controllando l'html c'è un elemento nascosto associato
        'il cui valore corrisponde comunque a Text
        Select Case tipoContatto

            Case 1

                lista.Add(New With {.Text = "Spett.le", .Value = "Spett.le"})

            Case 0
                lista.Add(New With {.Text = "Egregio", .Value = "Egregio"})
                lista.Add(New With {.Text = "Gent.mo", .Value = "Gent.mo"})
                lista.Add(New With {.Text = "Gent.ma", .Value = "Gent.ma"})

        End Select

        If Not String.IsNullOrEmpty(convenevoli) Then
            Dim esisteGia = Not lista.FirstOrDefault(Function(i) i.Value.Equals(convenevoli)) Is Nothing
            If Not esisteGia Then
                lista.Add(New With {.Text = convenevoli, .Value = convenevoli})
            End If
        End If

        Return New RispostaStandard With
        {
            .RispostaOK = True,
            .RispostaStringa = GetJson(lista)
        }

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function AggiungiRapportoContabilePrincilapele(
                    ByVal piva As String,
                    ByVal codRapporto As Integer,
                    ByVal desRapporto As String,
                    ByVal codContatto As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try
            Dim rapportoPrincipale = New With
                                    {
                                        .Cod_RisUm = 0,
                                        .piva = piva,
                                        .sa_cod = 0,
                                        .Cod_Contatto = codContatto,
                                        .Cod_Rapporto = codRapporto,
                                        .Settore_Des = "",
                                        .Rapporto_Des = desRapporto,
                                        .Qualifica_Cod = 0,
                                        .Qualifica_Des = "",
                                        .Mansione_Cod = 0,
                                        .Mansione_Des = "",
                                        .Attivita_Des = "",
                                        .occasionale = 0,
                                        .Ore_Settimanali = 0,
                                        .Info_Famiglia = "",
                                        .Classificazione_Cod = 0,
                                        .Classificazione_Des = "",
                                        .TipoRapporto_Cod = 0,
                                        .TipoRapporto_Des = DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/New_Contatto_Edit.aspx", "Continuativo"), String),
                                        .Patentino = "",
                                        .Ente_di_rilascio = "",
                                        .key_rap_cont = "",
                                        .Data_Rilascio_Patentino = AGRODATAINIZIO,
                                        .Data_Scadenza_Patentino = AGRODATAFINE,
                                        .Validita_Inizio = AGRODATAINIZIO,
                                        .Validita_Fine = AGRODATAFINE
                                    }

            r.RispostaOK = True
            r.RispostaStringa = GetJson(rapportoPrincipale)
        Catch ex As Exception
            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    Private Sub CaricaLiquidita(ByVal doc As XDocument)

        If doc Is Nothing Then
            jsLiquidita = "[]"
        Else
            Dim liquidita = (From l In doc.<DatiContatti>.<Contatto>.<DatiLiquidita>
                             Select New With
                                 {
                                    .piva = l.@piva,
                                    .Cod_Liquidita = l.@cod_liquidita,
                                    .Cod_Istituto = l.@cod_istituto,
                                    .Istituto_Des = l.@istituto_des,
                                    .Nazione = l.@nazione,
                                    .Cifre_Controllo = l.@cifre_controllo,
                                    .Cin = l.@cin,
                                    .Abi = l.@abi,
                                    .Cab = l.@cab,
                                    .Numero = l.@numero,
                                    .Bic = l.@bic,
                                    .ChkAbilitazione = l.@chkabilitazione,
                                    .Abilitazione_Des = l.@abilitazione_des,
                                    .Validita_Inizio = l.@validita_inizio,
                                    .Validita_Fine = l.@validita_fine,
                                    .Note = l.@note,
                                    .ChkDefault = CInt(l.@chkdefault) <> 0
            }).ToList()

            jsLiquidita = GetJson(liquidita)
        End If

    End Sub

    Private Sub CaricaConti(ByVal doc As XDocument)

        If doc Is Nothing Then
            jsConti = "[]"
        Else
            Dim conti = (From l In doc.<DatiContatti>.<Contatto>.<DatiConti>
                         Select New With
                                 {
                                    .piva = l.@piva,
                                    .Cod_Conto = l.@Cod_Conto,
                                    .ID_Riclassificazione = l.@ID_Riclassificazione,
                                    .Conto_Descr = l.@Conto_Descr,
                                    .Validita_Inizio = l.@Validita_Inizio,
                                    .Validita_Fine = l.@Validita_Fine,
                                    .key_conto = String.Format("{0}-{1}-{2}", l.@piva, l.@Cod_Conto, l.@cod_contatto)
                         }).ToList()

            jsConti = GetJson(conti)
        End If

    End Sub

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Istituti_Credito() As RispostaStandard

        Dim r As New RispostaStandard
        Dim dtIstCred As DataTable

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim dal = New Ist_Credito_R()
            dtIstCred = dal.Leggi(CODISTITUTO_NOFILTRO, 0, 0, "", "", objParametri_Server)

            r.RispostaStringa = GetJson(dtIstCred)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Codici_Lingue() As RispostaStandard

        Dim r As New RispostaStandard
        Dim dtLingue As DataTable

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objParametriAgenda = New ParametriAgenda

        Try

            Dim dal = New ACCDAA_ANAG_T001_TabellaCodiceDelleLingue_R()
            dtLingue = dal.Leggi("", "", "", objParametri_Server)

            r.RispostaStringa = GetJson(dtLingue)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Piano_Conti(ByVal piva As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim dtConti As DataTable

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim jarrayConti As New JArray()

            Dim dal = New RicxConti_R()
            dtConti = dal.Leggi_New(piva, 2, Now.Year, 0, "", "", "", "", objParametri_Server)

            For Each dr As DataRow In dtConti.Rows

                Dim idRic As String = dr("ID_Riclassificazione").ToString().Trim()
                Dim descrizione As String = dr("Conto_Descr").ToString()

                If CInt(dr("Imputabile")) = 1 Then

                    jarrayConti.Add(
                    New JObject(
                        New JProperty("Cod_Conto", dr("Cod_Conto").ToString()),
                        New JProperty("Conto_Descr", String.Format("{0} - {1}", idRic.PadLeft(20, " "), descrizione)),
                        New JProperty("ID_Riclassificazione", dr("ID_Riclassificazione").ToString())))

                End If

            Next



            r.RispostaStringa = JsonConvert.SerializeObject(jarrayConti, Newtonsoft.Json.Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaRapportiContabili(
        ByVal piva As String,
        ByVal codContatto As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim dtRappCont As DataTable

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            If Not HttpContext.Current.Session("RapportiContabili_X_Contatto_Leggi") Is Nothing Then
                dtRappCont = DirectCast(HttpContext.Current.Session("RapportiContabili_X_Contatto_Leggi"), DataTable)
            Else
                Dim dal = New Rapporti_Contabili_R()
                dtRappCont = dal.RapportiContabili_X_Contatto_Leggi(piva, codContatto, 0, 0, "", objParametri_Server)
                HttpContext.Current.Session("RapportiContabili_X_Contatto_Leggi") = dtRappCont
            End If


            Dim rapportiContabili = (From rc In dtRappCont.AsEnumerable
                                     Select New RapportoContabileModel With {
                                        .key_rap_cont = If(rc.Item("key_rap_cont") Is DBNull.Value, "", rc.Item("key_rap_cont")),
                                        .Cod_RisUm = If(rc.Item("Cod_RisUm") Is DBNull.Value, Nothing, CInt(rc.Item("Cod_RisUm"))),
                                        .Qualifica_Cod = If(rc.Item("Qualifica_Cod") Is DBNull.Value, Nothing, CInt(rc.Item("Qualifica_Cod"))),
                                        .Settore_Des = If(rc.Item("Settore_Des") Is DBNull.Value, "", rc.Item("Settore_Des")),
                                        .Qualifica_Des = If(rc.Item("Qualifica_Des") Is DBNull.Value, "", rc.Item("Qualifica_Des")),
                                        .Mansione_Des = If(rc.Item("Mansione_Des") Is DBNull.Value, "", rc.Item("Mansione_Des")),
                                        .Validita_Inizio = If(rc.Item("Validita_Inizio") Is DBNull.Value, Nothing, CDate(rc.Item("Validita_Inizio"))),
                                        .Validita_Fine = If(rc.Item("Validita_Fine") Is DBNull.Value, Nothing, CDate(rc.Item("Validita_Fine"))),
                                        .Attivita_Des = If(rc.Item("Attivita_Des") Is DBNull.Value, "", rc.Item("Attivita_Des")),
                                        .TipoRapporto_Des = If(rc.Item("TipoRapporto_Des") Is DBNull.Value, "", rc.Item("TipoRapporto_Des")),
                                        .Ore_Settimanali = If(rc.Item("Ore_Settimanali") Is DBNull.Value, Nothing, CDec(rc.Item("Ore_Settimanali"))),
                                        .Info_Famiglia = If(rc.Item("Info_Famiglia") Is DBNull.Value, "", rc.Item("Info_Famiglia")),
                                        .Classificazione_Des = If(rc.Item("Classificazione_Des") Is DBNull.Value, "", rc.Item("Classificazione_Des")),
                                        .Cod_Rapporto = If(rc.Item("Cod_Rapporto") Is DBNull.Value, Nothing, CInt(rc.Item("Cod_Rapporto"))),
                                        .Rapporto_Des = If(rc.Item("Rapporto_Des") Is DBNull.Value, "", rc.Item("Rapporto_Des")),
                                        .Mansione_Cod = If(rc.Item("Mansione_Cod") Is DBNull.Value, Nothing, CInt(rc.Item("Mansione_Cod"))),
                                        .TipoRapporto_Cod = If(rc.Item("TipoRapporto_Cod") Is DBNull.Value, Nothing, CInt(rc.Item("TipoRapporto_Cod"))),
                                        .Classificazione_Cod = If(rc.Item("Classificazione_Cod") Is DBNull.Value, Nothing, CInt(rc.Item("Classificazione_Cod"))),
                                        .Cod_IVA = If(rc.Item("Cod_Iva_Contatto") Is DBNull.Value, -1, CInt(rc.Item("Cod_Iva_Contatto"))),
                                        .Cod_Conto = If(rc.Item("Cod_Conto_Econ") Is DBNull.Value, Nothing, CInt(rc.Item("Cod_Conto_Econ"))),
                                        .Cod_Conto_Pat = If(rc.Item("Cod_Conto_Pat") Is DBNull.Value, Nothing, CInt(rc.Item("Cod_Conto_Pat"))),
                                        .Descrizione = If(rc.Item("Descrizione") Is DBNull.Value, "", CStr(rc.Item("Descrizione"))),
                                        .Conto_Descr = If(rc.Item("Conto_Descr") Is DBNull.Value, "", CStr(rc.Item("Conto_Descr"))),
                                        .Descr_Conto_Pat = If(rc.Item("Descr_Conto_Pat") Is DBNull.Value, "", CStr(rc.Item("Descr_Conto_Pat")))
                                     }).ToList()

            rapportiContabili.ForEach(Sub(s)
                                          s.Cod_Rapporto_Origine = s.Cod_Rapporto
                                          s.Rapporto_Des_Origine = s.Rapporto_Des
                                      End Sub)

            r.RispostaStringa = GetJson(rapportiContabili)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    Private Shared Function GetJson(ByVal dt As DataTable) As String
        Dim dictionary = From dr As DataRow In dt.Rows Select dt.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col))
        Dim serializerSettings As New JsonSerializerSettings() With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
        Return JsonConvert.SerializeObject(dictionary, Newtonsoft.Json.Formatting.None, serializerSettings)
    End Function
    Private Shared Function GetJson(Of T)(ByVal lista As List(Of T)) As String
        Dim serializerSettings As New JsonSerializerSettings() With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
        Return JsonConvert.SerializeObject(lista, Newtonsoft.Json.Formatting.None, serializerSettings)
    End Function
    Private Shared Function GetJson(ByVal oggetto As Object) As String
        Dim serializerSettings As New JsonSerializerSettings() With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
        Return JsonConvert.SerializeObject(oggetto, Newtonsoft.Json.Formatting.None, serializerSettings)
    End Function

    Private Sub CaricaAltriDati(ByVal risultato As XElement)

        hf_note.Value = risultato.@note
        hf_note2.Value = risultato.@note2
        hf_fittizio.Value = False

        hf_noteOperazioni.Value = risultato.@note_operazioni
        hf_noteOpererazioni2.Value = risultato.@note2_operazioni

        hf_tipo_destinazione.Value = risultato.@tipo_destinazione
        hf_origineDestinazione.Value = risultato.@tipo_speditore
        hf_dettCont_tipo_ind_default.Value = risultato.@tipo_indirizzo_default
        hf_Memo.Value = risultato.@memo

        If Not IsNothing(risultato.@chkfittizio) AndAlso IsNumeric(risultato.@chkfittizio) Then
            hf_fittizio.Value = Convert.ToBoolean(CInt(risultato.@chkfittizio))
        End If


    End Sub

    Private Sub CaricaDettagliContabili(ByVal risultato As XElement)


        hf_dettCont_sconto_add1.Value = 0
        hf_dettCont_sconto_add2.Value = 0
        hf_dettCont_sconto_add3.Value = 0
        hf_dettCont_sconto_cliente.Value = 0
        hf_dettCont_provvigione_agente.Value = 0
        hf_dettCont_provvigione_capoarea.Value = 0
        hf_dettCont_agente_cod.Value = 0
        hf_dettCont_capoarea_cod.Value = 0
        hf_dettCont_vettore_cod.Value = 0
        hf_dettCont_iva_default.Value = -1
        hf_dettCont_conto_economico_default.Value = 0
        hf_dettCont_conto_patrimoniale_default.Value = 0
        hf_dettCont_fatturazione_automatica.Value = -1
        hf_dettCont_documento_fatturazione.Value = -1
        hf_dettCont_destinazione_diversa.Value = -1

        If Not risultato.@sconto_testo Is Nothing AndAlso Not String.IsNullOrEmpty(risultato.@sconto_testo) Then
            Dim scontiAddizionali = risultato.@sconto_testo.Split("-")
            hf_dettCont_sconto_add1.Value = scontiAddizionali(0)
            If scontiAddizionali.Count() > 1 Then
                hf_dettCont_sconto_add2.Value = scontiAddizionali(1)
            End If
            If scontiAddizionali.Count() > 2 Then
                hf_dettCont_sconto_add3.Value = scontiAddizionali(2)
            End If
        End If

        If risultato.@provvigione IsNot Nothing AndAlso Not String.IsNullOrEmpty(risultato.@provvigione) Then
            hf_dettCont_provvigione_agente.Value = risultato.@provvigione
        End If

        If risultato.@provvigione_capoarea IsNot Nothing AndAlso Not String.IsNullOrEmpty(risultato.@provvigione_capoarea) Then
            hf_dettCont_provvigione_capoarea.Value = risultato.@provvigione_capoarea
        End If

        If risultato.@agente_cod IsNot Nothing AndAlso Not String.IsNullOrEmpty(risultato.@agente_cod) Then
            hf_dettCont_agente_cod.Value = risultato.@agente_cod
        End If
        If Not risultato.@capoarea_cod Is Nothing AndAlso Not String.IsNullOrEmpty(risultato.@capoarea_cod) Then
            hf_dettCont_capoarea_cod.Value = risultato.@capoarea_cod
        End If

        If Not risultato.@vettore_cod Is Nothing AndAlso Not String.IsNullOrEmpty(risultato.@vettore_cod) Then
            hf_dettCont_vettore_cod.Value = risultato.@vettore_cod
        End If

        If Not risultato.@cod_iva_contatto Is Nothing AndAlso Not String.IsNullOrEmpty(risultato.@cod_iva_contatto) Then
            hf_dettCont_iva_default.Value = risultato.@cod_iva_contatto
        End If

        If Not risultato.@cod_conto_econ Is Nothing AndAlso Not String.IsNullOrEmpty(risultato.@cod_conto_econ) Then
            hf_dettCont_conto_economico_default.Value = risultato.@cod_conto_econ
        End If

        If Not risultato.@cod_conto_pat Is Nothing AndAlso Not String.IsNullOrEmpty(risultato.@cod_conto_pat) Then
            hf_dettCont_conto_patrimoniale_default.Value = risultato.@cod_conto_pat
        End If

        If Not risultato.@modalita_fatturazione Is Nothing AndAlso Not String.IsNullOrEmpty(risultato.@modalita_fatturazione) Then
            hf_dettCont_fatturazione_automatica.Value = risultato.@modalita_fatturazione
        End If

        If Not risultato.@documento_fatturazione Is Nothing AndAlso Not String.IsNullOrEmpty(risultato.@documento_fatturazione) Then
            hf_dettCont_documento_fatturazione.Value = risultato.@documento_fatturazione
        End If

        If Not risultato.@cod_risum_destinazione_diversa Is Nothing AndAlso Not String.IsNullOrEmpty(risultato.@cod_risum_destinazione_diversa) Then
            hf_dettCont_destinazione_diversa.Value = risultato.@cod_risum_destinazione_diversa
        End If

        If Not risultato.@tipo_indirizzo_default_destinazione_diversa Is Nothing AndAlso Not String.IsNullOrEmpty(risultato.@tipo_indirizzo_default_destinazione_diversa) Then
            fh_dettCont_tipo_indirizzo_default_destinazione_diversa.Value = risultato.@tipo_indirizzo_default_destinazione_diversa
        End If

    End Sub

    Private Sub CaricaIndirizzi(ByVal doc As XDocument, ByRef Dt_Indirizzi As DataTable, ByVal piva As String)

        Dim objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        objParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim indiTipoDal = New IndirizzoTipo_R()
        Dim indirizziTipo = TipiIndirizzoToList(indiTipoDal.Leggi(piva, -1, -99, -1, "", "", objParametri))

        Dim risultato_Indirizzi = (From c In doc.<DatiContatti>.<Contatto>.<Indirizzo>
                                   Select c).ToList

        ' indirizzi
        If Not ViewState("Dt_Indirizzi") Is Nothing Then

            Dt_Indirizzi = ViewState("Dt_Indirizzi")

            Dt_Indirizzi.Columns.Add("Gestione_Gerarchia_Geografica", GetType(Integer))

            For i = 0 To risultato_Indirizzi.Count - 1

                Dim drInd As DataRow = Dt_Indirizzi.NewRow

                drInd.Item("Cod_Indirizzo") = risultato_Indirizzi(i).@cod_indirizzo
                drInd.Item("Tipo_Indirizzo") = risultato_Indirizzi(i).@tipo_indirizzo

                Select Case risultato_Indirizzi(i).@tipo_indirizzo
                    Case INDIRIZZO_RESIDENZA
                        drInd.Item("Tipo_Indirizzo_Desc") = AgronicaAgenda_2010.Residenza
                    Case INDIRIZZO_LUOGO_NASCITA
                        drInd.Item("Tipo_Indirizzo_Desc") = AgronicaAgenda_2010.LuogoDiNascita
                    Case INDIRIZZO_DOMICILIO
                        drInd.Item("Tipo_Indirizzo_Desc") = AgronicaAgenda_2010.Domicilio
                    Case INDIRIZZO_RESIDENZA_ESTIVA
                        drInd.Item("Tipo_Indirizzo_Desc") = AgronicaAgenda_2010.ResidenzaEstiva
                    Case INDIRIZZO_SEDE_OPERATIVA
                        drInd.Item("Tipo_Indirizzo_Desc") = AgronicaAgenda_2010.SedeOperativa
                    Case INDIRIZZO_SEDE_LEGALE
                        drInd.Item("Tipo_Indirizzo_Desc") = AgronicaAgenda_2010.SedeLegale
                    Case INDIRIZZO_SEDE_AZIENDALE
                        drInd.Item("Tipo_Indirizzo_Desc") = AgronicaAgenda_2010.SedeAziendale
                    Case INDIRIZZO_STABILIMENTO
                        drInd.Item("Tipo_Indirizzo_Desc") = AgronicaAgenda_2010.Stabilimento
                    Case INDIRIZZO_STABILE_ORGANIZZAZIONE
                        drInd.Item("Tipo_Indirizzo_Desc") = DirectCast(GetLocalResourceObject("StabileOrganizzazione"), String)
                    Case Else
                        Dim tipoIndirizzo = indirizziTipo.FirstOrDefault(Function(ti) ti.IndirizzoTipo_Cod = risultato_Indirizzi(i).@tipo_indirizzo)
                        drInd.Item("Tipo_Indirizzo_Desc") = If(tipoIndirizzo Is Nothing, "", tipoIndirizzo.Descrizione)
                End Select

                drInd.Item("Via") = risultato_Indirizzi(i).@ind_des
                drInd.Item("Frazione") = risultato_Indirizzi(i).@frz_des
                drInd.Item("cap") = risultato_Indirizzi(i).@cap

                drInd.Item("Provincia_cod") = risultato_Indirizzi(i).@pro_cod_istat

                Dim hasGestGerarchiaGeografica As Integer = 1
                'If (risultato_Indirizzi(i).@stato.ToUpper() <> "IT" And risultato_Indirizzi(i).@stato.ToUpper() <> "ITALIA") Then
                If Not String.IsNullOrWhiteSpace(risultato_Indirizzi(i).@stato.Replace("&nbsp;", "")) Then
                    'Lettura Gestione_Gerarchia_Geografica
                    Dim DT_Nazioni As DataTable
                    Dim objNazioni As New AgronicaCoreMetaSchemaDAL.ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166_R
                    DT_Nazioni = objNazioni.Leggi(risultato_Indirizzi(i).@stato, "", "Descrizione", objParametri_Server)
                    hasGestGerarchiaGeografica = CInt(DT_Nazioni(0).Item("Gestione_Gerarchia_Geografica"))
                End If

                If hasGestGerarchiaGeografica = 0 Then

                    'Nessuna gestione geografica
                    drInd.Item("Citta_Des") = risultato_Indirizzi(i).@frz_des
                    drInd.Item("Comune_des") = ""
                    drInd.Item("Provincia_des") = ""
                    drInd.Item("Frazione") = ""
                Else
                    If (risultato_Indirizzi(i).@com_des.ToLower() = "non definita") Then
                        drInd.Item("Comune_des") = "Non Definito"
                    Else
                        drInd.Item("Comune_des") = risultato_Indirizzi(i).@com_des
                    End If
                    'drInd.Item("Comune_des") = risultato_Indirizzi(i).@com_des
                    drInd.Item("Provincia_des") = risultato_Indirizzi(i).@pro_des
                End If

                drInd.Item("Comune_cod") = risultato_Indirizzi(i).@com_cod_istat
                drInd.Item("Sigla_Prov") = risultato_Indirizzi(i).@pro_cod
                drInd.Item("Stato") = risultato_Indirizzi(i).@stato
                drInd.Item("Stato_Des") = risultato_Indirizzi(i).@stato_des
                drInd.Item("Note") = risultato_Indirizzi(i).@note
                drInd.Item("Codice_Lingua") = risultato_Indirizzi(i).@codice_lingua
                drInd.Item("Lingua_Des") = risultato_Indirizzi(i).@lingua_des
                drInd.Item("Gestione_Gerarchia_Geografica") = hasGestGerarchiaGeografica

                Dt_Indirizzi.Rows.Add(drInd)
            Next

        End If

        ViewState("Dt_Indirizzi") = Dt_Indirizzi
        HttpContext.Current.Session("Dt_Indirizzi") = Dt_Indirizzi

    End Sub

    Private Sub CaricaRubricaNew(ByVal doc As XDocument)

        If doc Is Nothing Then
            jsRubrica = "[]"
        Else
            Dim rubrica = (
                        From c In doc.<DatiContatti>.<Contatto>.<Rubrica>
                        Select New With
                                {
                                    .Cod_Rubrica = c.@cod_rubrica,
                                    .Numero = c.@numero,
                                    .Descrizione = c.@descr,
                                    .TipoRubrica_Cod = set_TipoRubricaCod(c.@descr),
                                    .TipoRubrica_Des = set_TipoRubricaDes(c.@descr),
                                    .Key_Rubrica = Guid.NewGuid.ToString()
                                }
                        ).ToList()
            jsRubrica = GetJson(rubrica)
        End If

    End Sub

    Private Function set_TipoRubricaCod(ByVal descrizione As String) As Integer
        Dim tipoCod As Integer
        Select Case descrizione
            Case "Telefono", "Telefono:", "N. Telefono:", "N. Telefono"
                tipoCod = 0
            Case "Fax", "Fax:"
                tipoCod = 1
            Case "Cellulare", "Cellulare:"
                tipoCod = 2
            Case "E-Mail", "E-Mail:", "Email:", "Email"
                tipoCod = 3
            Case "Sito Web", "Sito Web:"
                tipoCod = 4
            Case Else
                tipoCod = 5
        End Select

        Return tipoCod
    End Function

    Private Function set_TipoRubricaDes(ByVal descrizione As String) As String

        Dim tipoDes As String
        Select Case descrizione
            Case "Telefono", "Telefono:", "N. Telefono:", "N. Telefono"
                tipoDes = "Telefono"
            Case "Fax", "Fax:"
                tipoDes = "Fax"
            Case "Cellulare", "Cellulare:"
                tipoDes = "Cellulare"
            Case "E-Mail", "E-Mail:", "Email:", "Email"
                tipoDes = "E-Mail"
            Case "Sito Web", "Sito Web:"
                tipoDes = "Sito Web"
            Case Else
                tipoDes = "Non Assegnato"
        End Select

        Return tipoDes

    End Function

    Private Sub CaricaCosti(ByVal doc As XDocument, ByVal piva As String, ByVal codContatto As String)

        If doc Is Nothing Then
            jsCostiNew = "[]"
        Else
            Dim costi = (From c In doc.<DatiContatti>.<Contatto>.<RapCon>.<DatiProdotti_Costi>.<Prodotto_Costo>
                         Select New With
                                 {
                                    .Id = c.@id,
                                    .Udm_Cod = c.@mezzo,
                                    .InizioPrezzo = c.@validita_inizio,
                                    .FinePrezzo = c.@validita_fine,
                                    .Prezzo = c.@prezzo_unitario,
                                    .Cod_RisUm = c.@mat_cod,
                                    .Cod_RisUm_Des = "",
                                    .Udm_Des = ""
                }).ToList()
            For Each c As Object In costi
                c.Udm_Des = IIf(c.Udm_Cod = enum_TipoMezzo.Ettaro, "HA", AgronicaAgenda_2010.Ora)
            Next

            Dim rappContabili = InnerDropDownRapportiContabili(piva, codContatto)

            For Each costo In costi

                Dim rapCont = rappContabili.FirstOrDefault(Function(rc) rc.Cod_RisUm = costo.Cod_RisUm)
                If Not rapCont Is Nothing Then
                    costo.Cod_RisUm_Des = rapCont.Cod_RisUm_Des
                End If

            Next

            jsCostiNew = GetJson(costi)
        End If

    End Sub
    Private Sub CaricaContattiCodici(ByVal doc As XDocument)

        ' Contatti Codici
        Dim contattiCodici = (From cc In doc.<DatiContatti>.<Contatto>.<Contatto_Codice>
                              Select New With {
                                    cc.@id_cod,
                                    cc.@val_cod,
                                    cc.@validita_inizio,
                                    cc.@validita_fine
        }).ToList()

        Dim valCod = contattiCodici.Where(Function(c) c.id_cod = enum_CodiciAnagrafe.PecContatto).FirstOrDefault()
        Txt_Pec.Text = If(Not valCod Is Nothing, valCod.val_cod, "")
        valCod = contattiCodici.Where(Function(c) c.id_cod = enum_CodiciAnagrafe.CodiceSDI).FirstOrDefault()
        Txt_Codice_SDI.Text = If(Not valCod Is Nothing, valCod.val_cod, "")
        valCod = contattiCodici.Where(Function(c) c.id_cod = enum_CodiciAnagrafe.TipoContattoFattura).FirstOrDefault()
        If valCod IsNot Nothing Then
            Ddl_Tipo_Contatto.SelectedValue = valCod.val_cod
        End If

        valCod = contattiCodici.Where(Function(c) c.id_cod = enum_CodiciAnagrafe.RappresentanteFiscale).FirstOrDefault()
        If valCod IsNot Nothing Then
            hf_RappresentanteFiscale.Value = CInt(valCod.val_cod)
        End If

        ' Natura Intento numero protocollo
        valCod = contattiCodici.Where(Function(c) c.id_cod = enum_CodiciAnagrafe.DichiarazioneIntentoNumeroProtocollo).FirstOrDefault()
        If valCod IsNot Nothing Then
            txt_dich_intenti_protocollo.Text = valCod.val_cod
        End If

        ' Natura Intento data protocollo
        valCod = contattiCodici.Where(Function(c) c.id_cod = enum_CodiciAnagrafe.DichiarazioneIntentoDataRicezione).FirstOrDefault()
        If valCod IsNot Nothing Then
            If IsDate(valCod.val_cod) Then
                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.Value = CDate(valCod.val_cod)
            End If
        End If

        valCod = contattiCodici.Where(Function(c) c.id_cod = enum_CodiciAnagrafe.CodiceAccisa).FirstOrDefault()
        If valCod IsNot Nothing Then
            Txt_Cod_Accisa.Text = valCod.val_cod
        End If

        valCod = contattiCodici.Where(Function(c) c.id_cod = enum_CodiciAnagrafe.ScontoContattoDefault).FirstOrDefault()
        If valCod IsNot Nothing Then
            hf_dettCont_sconto_cliente.Value = valCod.val_cod
        End If

        valCod = contattiCodici.Where(Function(c) c.id_cod = enum_CodiciAnagrafe.acciseDAA_CodiceAccisa_UfficioDoganale).FirstOrDefault()
        If valCod IsNot Nothing Then
            hf_ufficioDogane.Value = valCod.val_cod
        End If

        valCod = contattiCodici.Where(Function(c) c.id_cod = enum_CodiciAnagrafe.acciseDAA_CodiceAccisa_Destinatario).FirstOrDefault()
        If Not valCod Is Nothing Then
            Txt_Rif_Dep_Fisc.Text = valCod.val_cod
        End If

        valCod = contattiCodici.Where(Function(c) c.id_cod = enum_CodiciAnagrafe.acciseDAA_CodiceAccisa_codiceUA).FirstOrDefault()
        If valCod IsNot Nothing Then
            Txt_Cod_UA.Text = valCod.val_cod
        End If

        valCod = contattiCodici.Where(Function(c) c.id_cod = enum_CodiciAnagrafe.acciseDAA_Accise_Conto_Garanzia).FirstOrDefault()
        If valCod IsNot Nothing Then
            Txt_Cod_Conto_Gar.Text = valCod.val_cod
        End If

        ' Gestione Vettore
        valCod = contattiCodici.Where(Function(c) c.id_cod = 1315).FirstOrDefault()
        If valCod IsNot Nothing Then
            hf_dettCont_gestionevettore_cod.Value = valCod.val_cod
        End If

        valCod = contattiCodici.Where(Function(c) c.id_cod = enum_CodiciAnagrafe.IBANDefault).FirstOrDefault()
        If valCod IsNot Nothing Then
            hf_dettCont_iban_default.Value = valCod.val_cod
        End If

        valCod = contattiCodici.Where(Function(c) c.id_cod = enum_CodiciAnagrafe.ModalitaPagamentoDefault).FirstOrDefault()
        If valCod IsNot Nothing Then
            hf_dettCont_mod_pag_default.Value = valCod.val_cod
        End If

        valCod = contattiCodici.Where(Function(c) c.id_cod = enum_CodiciAnagrafe.ListinoPrezziAcquistoDefault).FirstOrDefault()
        If valCod IsNot Nothing Then
            hf_dettCont_listini_prezzi_acq.Value = valCod.val_cod
        End If

        valCod = contattiCodici.Where(Function(c) c.id_cod = enum_CodiciAnagrafe.ListinoPrezziVenditaDefault).FirstOrDefault()
        If valCod IsNot Nothing Then
            hf_dettCont_listini_prezzi_ven.Value = valCod.val_cod
        End If
        valCod = contattiCodici.Where(Function(c) c.id_cod = enum_CodiciAnagrafe.ReferenteConferimento).FirstOrDefault()
        If valCod IsNot Nothing Then
            hf_dettCont_referenteConferimento_cod.Value = valCod.val_cod
        End If

        valCod = contattiCodici.Where(Function(c) c.id_cod = enum_CodiciAnagrafe.CaloPesoDefault_daContatto).FirstOrDefault()
        If valCod IsNot Nothing AndAlso IsNumeric(valCod.val_cod) Then
            hf_caloPeso.Value = valCod.val_cod

        End If

        valCod = contattiCodici.Where(Function(c) c.id_cod = enum_CodiciAnagrafe.CoeffCaloPesoContatto).FirstOrDefault()
        If valCod IsNot Nothing AndAlso IsNumeric(valCod.val_cod) Then
            hf_coeffCaloPeso.Value = valCod.val_cod
        End If

    End Sub

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Comuni(ByVal provincia As String) As Array


        Dim cmb_comuni2 As New DropDownList
        Dim rval(1) As String
        Dim options As String = ""

        If provincia <> "" Then

            AgronicaCoreUtility.CaricaListControl.Comuni(cmb_comuni2,
                                                             True, "", "",
                                                            provincia, False, 1, "", "", HttpContext.Current.Session("ASG_objParametri_Server"))


            For Each itm As ListItem In cmb_comuni2.Items
                options &= "<Option value=""" & itm.Value & """>" & itm.Text & "</Option>"
            Next

            rval(0) = options

            Dim objI As New AgronicaCoreMetaSchemaDAL.Istat_R
            'Imposto la textbox del CODICE ISTAT
            rval(1) = objI.CodIstat_from_Provincia(provincia, HttpContext.Current.Session("ASG_objParametri_Server"))


        End If

        Return rval

    End Function


    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function GetRandomPiva() As String
        Return GeneraRandom()
    End Function


    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function GetRandomCodFisc() As String
        Return GeneraRandom()
    End Function

    Private Shared Function GeneraRandom() As String

        Dim objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        objParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        objParametri.ImpostaFinestre_con_SalvataggioTemporale(AGRODATAINIZIO, AGRODATAFINE)

        Dim ok = False


        Dim objAgroSe As New AgronicaCoreDataProvider.Agro_Sequenze
        Dim objCont As New AgronicaCoreAnagrafeDAL.Contatti_R
        Dim dt As DataTable

        Dim str_r As String
        While ok = False
            str_r = objAgroSe.NuovoId_Tabella("impresa", 0, 0, objParametri).ToString.Replace("-", "F")
            'controllo se è già usato 
            dt = objCont.LeggiContattoSpecifico("", str_r, 0, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri)
            If dt.Rows.Count = 0 Then
                ok = True
            End If
        End While
        objParametri.ResettaFinestra()

        Return str_r

    End Function


    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Cambia_provincia_salva_codice(ByVal targa As String) As String
        Dim objSchemaDal As New AgronicaCoreMetaSchemaDAL.Istat_R
        Dim codice_prov As String

        codice_prov = objSchemaDal.CodIstat_from_Provincia(targa, HttpContext.Current.Session("ASG_objParametri_Server"))

        Return codice_prov

    End Function


    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Cambia_comune_salva_codice(ByVal provincia As String, ByVal nome_comune As String) As String

        Dim objSchemaDal As New AgronicaCoreMetaSchemaDAL.Istat_R
        Dim Comune As String
        Dim Pro_Cod_Istat As String
        Dim Com_Cod_Istat As String

        objSchemaDal.CodIstat_from_SiglaProvincia_and_StringaComune(provincia, nome_comune, Comune, Pro_Cod_Istat, Com_Cod_Istat, HttpContext.Current.Session("ASG_objParametri_Server"))

        Return Comune & "|" & Com_Cod_Istat

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_Rapporti_In_Session(ByVal dati As String) As String

        Dim DtRapporti As DataTable

        DtRapporti = HttpContext.Current.Session("DT_Rapporti")

        ' Pulisco tutta la tabella (checked = 0)
        For i = 0 To DtRapporti.Rows.Count - 1
            DtRapporti.Rows(i).Item("checked") = 0
        Next

        Dim rap_chk As String() = dati.Split(New Char() {"|"c})

        ' Per ogni chiave di riga di Appezzamento checked
        For Each chiave As String In rap_chk
            If chiave <> "" Then

                ' Ciclo su DT di tutti gli Appezzamenti disponibili per ritrovare la riga
                For i = 0 To DtRapporti.Rows.Count - 1

                    ' Controllo se le chiavi coincidono
                    If CInt(chiave) = DtRapporti.Rows(i).Item("Cod_Rapporto") Then
                        DtRapporti.Rows(i).Item("checked") = 1
                    End If
                Next

            End If
        Next


        HttpContext.Current.Session("DT_Rapporti") = DtRapporti

        Return "ok"

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Controlla_IndirizzoTipo(ByVal IndirizzoTipo_Cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim dalL = New IndirizzoTipo_R
        Dim inUso = dalL.InUso(IndirizzoTipo_Cod, objParametri_Server)
        If inUso Then
            r.RispostaOK = False
            r.Errore = DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/New_Contatto_Edit.aspx", "ImpossibileCancellareTipoIndirizzoInUso"), String)
        End If

        Return r

    End Function



    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Svuota_Indirizzi(ByVal tipoOperazione As Integer) As RispostaStandard

        Dim DT_Indirizzo As DataTable
        Dim r As New RispostaStandard

        DT_Indirizzo = HttpContext.Current.Session("DT_Indirizzo")

        If DT_Indirizzo.Rows.Count > 0 Then
            DT_Indirizzo.Rows.Clear()
        End If
        HttpContext.Current.Session("DT_Indirizzo") = DT_Indirizzo

        r.RispostaOK = True
        r.RispostaStringa = DT_to_Json_Indirizzi(DT_Indirizzo, tipoOperazione)

        Return r

    End Function



    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Aggiungi_Indirizzo(ByVal parametri As String,
                                              ByVal tipoOperazione As Integer,
                                              ByVal modello As String) As RispostaStandard

        Dim DT_Indirizzo As DataTable
        Dim r As New RispostaStandard
        Dim trovato As Boolean = False

        Dim model As IndirizzoModel = JsonConvert.DeserializeObject(Of IndirizzoModel)(modello)

        DT_Indirizzo = HttpContext.Current.Session("DT_Indirizzo")


        Dim par As String() = parametri.Split(New Char() {"|"c})

        ' Controllo se il tipo di Indirizzo esiste gia
        For i = 0 To DT_Indirizzo.Rows.Count - 1

            'If DT_Indirizzo.Rows(i).Item("Tipo_Indirizzo") = model.Tipo_Indirizzo Then
            '    trovato = True
            '    r.RispostaOK = False
            '    r.Errore = DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/New_Contatto_Edit.aspx", "IndirizzoDellaTipologiaEsisteGià"), String)

            'End If

        Next

        If Not trovato Then
            Dim dr As DataRow = DT_Indirizzo.NewRow

            dr.Item("Cod_Indirizzo") = 0
            dr.Item("Tipo_Indirizzo") = model.Tipo_Indirizzo
            dr.Item("Tipo_Indirizzo_Desc") = model.Tipo_Indirizzo_Desc
            dr.Item("Via") = model.Via
            dr.Item("Sigla_Prov") = model.Sigla_Prov
            dr.Item("Provincia_des") = model.Provincia_des
            dr.Item("Provincia_cod") = model.Provincia_cod
            dr.Item("Comune_cod") = model.Comune_cod
            dr.Item("Comune_des") = model.Comune_des
            dr.Item("Frazione") = model.Frazione
            dr.Item("Cap") = model.Cap
            dr.Item("Stato") = model.Stato
            dr.Item("Note") = model.Note
            dr.Item("Codice_Lingua") = model.Codice_Lingua

            DT_Indirizzo.Rows.Add(dr)

            HttpContext.Current.Session("DT_Indirizzo") = DT_Indirizzo

            r.RispostaOK = True
            r.RispostaStringa = DT_to_Json_Indirizzi(DT_Indirizzo, tipoOperazione)

        End If

        Return r

    End Function

    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Modifica_Indirizzo(ByVal parametri As String,
                                              ByVal tipoOperazione As Integer,
                                              ByVal modello As String) As String

        Dim DT_Indirizzi As DataTable
        Dim risp
        Dim model As IndirizzoModel = JsonConvert.DeserializeObject(Of IndirizzoModel)(modello)


        DT_Indirizzi = HttpContext.Current.Session("Dt_Indirizzi")

        Dim par As String() = parametri.Split(New Char() {"|"c})

        For i = 0 To DT_Indirizzi.Rows.Count - 1

            If DT_Indirizzi.Rows(i).Item("Tipo_Indirizzo") = model.Tipo_Indirizzo Then

                DT_Indirizzi.Rows(i).Item("Tipo_Indirizzo_Desc") = model.Tipo_Indirizzo_Desc
                DT_Indirizzi.Rows(i).Item("Via") = model.Via
                DT_Indirizzi.Rows(i).Item("Sigla_Prov") = model.Sigla_Prov
                DT_Indirizzi.Rows(i).Item("Provincia_des") = model.Provincia_des
                DT_Indirizzi.Rows(i).Item("Provincia_cod") = model.Provincia_cod
                DT_Indirizzi.Rows(i).Item("Comune_cod") = model.Comune_cod
                DT_Indirizzi.Rows(i).Item("Comune_des") = model.Comune_des
                DT_Indirizzi.Rows(i).Item("Frazione") = model.Frazione
                DT_Indirizzi.Rows(i).Item("Cap") = model.Cap
                DT_Indirizzi.Rows(i).Item("Stato") = model.Stato
                DT_Indirizzi.Rows(i).Item("Note") = model.Note

                If model.Codice_Lingua = "" Then
                    model.Codice_Lingua = If(model.Stato.ToString().ToLower() = "it", "it", "en")
                End If
                DT_Indirizzi.Rows(i).Item("Codice_Lingua") = model.Codice_Lingua

            End If

        Next

        risp = DT_to_Json_Indirizzi(DT_Indirizzi, tipoOperazione)
        HttpContext.Current.Session("Dt_Indirizzi") = DT_Indirizzi

        Return risp

    End Function

    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function DT_to_Json_Indirizzi(ByVal dt As DataTable, ByVal tipoOperazione As Integer) As String

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        If IsNothing(dt) Then
            Return "[]"
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim cn As New ColonneNome("Cod_Indirizzo", "", "string")
        cn._FormatoParticolare = "<span></span>"
        cn._Filtrabile = False
        'cn._css = "prova"
        l.Add(cn)

        ''aggiungo i pulsanti per modifica ed eliminazione
        Dim c = New ColonneNome("Cod_Indirizzo", "Tool", "string")

        Dim listaBtn = New List(Of btnAzioni)

        If tipoOperazione <> enum_TipoOperazioneDB.Lettura Then
            listaBtn.Add(New btnAzioni("fa-pencil-square-o edit_elem", "PopolaCampiIndirizzo(this);"))
            'listaBtn.Add(New btnAzioni("fa-trash-o del_elem", "EliminaIndirizzo(this);"))
        End If

        Dim tool As New ToolStandard(listaBtn)
        tool.cssColonna = "colmodmovimenti"
        c._FormatoParticolare = tool.toString()
        c._Filtrabile = False
        c._ColonnaDiSelezione = True
        l.Add(c)

        cn = New ColonneNome("Cod_Indirizzo", "Cod_Indirizzo", "string")
        cn._css = "wacol_cod"
        cn._hidden = True
        l.Add(cn)

        cn = New ColonneNome("Tipo_Indirizzo", "Tipo_Indirizzo", "string")
        cn._css = "wacol_tipologia_cod"
        cn._hidden = True
        l.Add(cn)


        cn = New ColonneNome("Tipo_Indirizzo_Desc", "Tipologia", "string")
        cn._css = "wacol_tipologia"
        l.Add(cn)

        cn = New ColonneNome("Via", "Indirizzo", "string")
        cn._css = "wacol_indirizzo"
        l.Add(cn)

        cn = New ColonneNome("Provincia_cod", "Provincia_cod", "string")
        cn._hidden = True
        l.Add(cn)

        cn = New ColonneNome("Sigla_Prov", "Prov", "string")
        cn._css = "wacol_provincia"
        l.Add(cn)

        cn = New ColonneNome("Comune_cod", "Comune_cod", "string")
        cn._hidden = True
        cn._css = "wacol_comune_cod"
        l.Add(cn)

        cn = New ColonneNome("Comune_des", "Comune", "string")
        cn._css = "wacol_comune"
        l.Add(cn)

        cn = New ColonneNome("Frazione", "Frazione", "string")
        cn._css = "wacol_frazione"
        l.Add(cn)

        cn = New ColonneNome("Cap", "Cap", "string")
        cn._css = "wacol_cap"
        l.Add(cn)

        cn = New ColonneNome("Stato", "Stato", "string")
        cn._css = "wacol_stato"
        l.Add(cn)

        cn = New ColonneNome("Note", "Note", "string")
        cn._css = "wacol_note"
        l.Add(cn)

        cn = New ColonneNome("Codice_Lingua", "Codice_Lingua", "string")
        cn._css = "wacol_codice_lingua"
        l.Add(cn)

        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable(dt, l)
        Return risp
    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function DT_to_Json_Rapporti(ByVal dt As DataTable) As String
        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        If IsNothing(dt) Then
            Return "[]"
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim cn As New ColonneNome("Cod_Rapporto", "Cod_Rapporto", "string")
        cn._Filtrabile = False
        cn._ColonnaDiSelezione = True
        cn._hidden = True
        l.Add(cn)

        cn = New ColonneNome("Rapporto_Des", AgronicaAgenda_2010.RapportoContabile, "string")
        cn._css = "wacol_rapporto_desc"
        l.Add(cn)

        cn = New ColonneNome("Sa_Cod", "Sa_Cod", "string")
        cn._css = "wacol_sa_cod"
        cn._hidden = True
        l.Add(cn)

        cn = New ColonneNome("Cod_RisUm", "Cod_RisUm", "string")
        cn._css = "wacol_Cod_RisUm"
        cn._hidden = True
        l.Add(cn)

        cn = New ColonneNome("validita_inizio", "validita_inizio", "string")
        cn._css = "wacol_validita_inizio"
        cn._hidden = True
        l.Add(cn)

        cn = New ColonneNome("validita_fine", "validita_fine", "string")
        cn._css = "wacol_validita_fine"
        cn._hidden = True
        l.Add(cn)

        cn = New ColonneNome("checked", "checked", "string")
        cn._css = "wacol_checked"
        cn._hidden = True
        l.Add(cn)


        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable(dt, l)
        Return risp
    End Function


    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Aggiorna_RapportiWS(ByVal tipo_persona As Integer) As String

        Dim DtRapporti As DataTable
        Dim DtRapportiFiltrati As DataTable
        Dim Dr() As DataRow
        Dim strFiltro As String = ""
        Dim i As Integer

        Dim risp As String = ""

        If Not HttpContext.Current.Session("Dt_Rapporti") Is Nothing Then

            DtRapporti = HttpContext.Current.Session("Dt_Rapporti")
            DtRapportiFiltrati = DtRapporti.Clone

            If tipo_persona = 0 Then
                'persona fisica
                strFiltro = " Sa_Cod in (0,2) "
            Else
                'giuridica
                strFiltro = " Sa_Cod in (0,1) "
            End If

            Dr = DtRapporti.Select(strFiltro)

            For i = 0 To Dr.Length - 1
                DtRapportiFiltrati.ImportRow(Dr(i))
            Next

            Dim DtKey(1) As String
            DtKey(0) = "Cod_Rapporto"
            DtKey(1) = "Cod_RisUm"

            HttpContext.Current.Session("RBL_TipoUtente") = tipo_persona
            risp = DT_to_Json_Rapporti(DtRapportiFiltrati)

        End If

        Return risp

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Aggiorna_Visibilita(ByVal tipo_persona As Integer, ByVal piva As String) As String

        Dim Cmb_CentriAziendali As New DropDownList
        Dim risp As String = ""

        Lingua.Gias_InizializzaCultura_DaSession()

        Select Case tipo_persona

            Case 0

                'solo le persone fisiche rendo possibile l'assegnazione ad un centro
                AgronicaCoreUtility.CaricaListControl.Centri_Aziendali(Cmb_CentriAziendali, True, AgronicaAgenda_2010.ContattoAziendale, "0", piva, False, 2, "", "", HttpContext.Current.Session("ASG_objParametri_Server"))
                Select Case HttpContext.Current.Session("UtenteAbilitato_Pubblico")
                    Case True
                        Cmb_CentriAziendali.Items.Add(New ListItem(AgronicaAgenda_2010.ContattoMovimentabileDaTutteLeImprese, "-1"))
                End Select

            Case 1, 2

                Cmb_CentriAziendali.Items.Add(New ListItem(AgronicaAgenda_2010.ContattoAziendale, "0"))
                Select Case HttpContext.Current.Session("UtenteAbilitato_Pubblico")
                    Case True
                        Cmb_CentriAziendali.Items.Add(New ListItem(AgronicaAgenda_2010.ContattoMovimentabileDaTutteLeImprese, "-1"))
                End Select

        End Select

        Dim rval As String = ""
        For Each itm As ListItem In Cmb_CentriAziendali.Items
            rval &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
        Next


        Return rval

    End Function

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        permessi = New PermessiUtente()

        Master.flag_pag_Anagrafica = True
        '----- Verifico che l'utente sia autenticato
        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("~/Custom500.aspx")
        End If

        '---
        objParametri_Super_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Super_Server"))
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        '---

        If Request.QueryString("visibilita") IsNot Nothing AndAlso IsNumeric(Request.QueryString("visibilita")) Then
            Qs_Visibilita = CInt(Request.QueryString("visibilita"))
        End If

        If Not String.IsNullOrWhiteSpace(Request.QueryString("o")) Then
            Qs_Operazione = Stringa_Decodifica(Request.QueryString("o").ToString,
                                    AgroKey_EncoderDecoder,
                                    Server)
        End If

        If Not String.IsNullOrWhiteSpace(Request.QueryString("tipo_rapporto")) Then
            Qs_Rapporto_Cod = Stringa_Decodifica(Request.QueryString("tipo_rapporto").ToString,
                                        AgroKey_EncoderDecoder,
                                        Server)
            hf_Cod_Rapporto.Value = Qs_Rapporto_Cod

        End If

        If Not String.IsNullOrWhiteSpace(Request.QueryString("lav_cod")) Then
            Qs_LavCod = Stringa_Decodifica(Request.QueryString("lav_cod").ToString,
                                AgroKey_EncoderDecoder,
                                Server)
        End If

        If Not String.IsNullOrWhiteSpace(Request.QueryString("orig")) Then
            Qs_Origine = Stringa_Decodifica(Request.QueryString("orig").ToString,
                                        AgroKey_EncoderDecoder,
                                        Server)
        End If

        If Not String.IsNullOrWhiteSpace(Request.QueryString("codcont")) Then
            Qs_CodContatto = Stringa_Decodifica(Request.QueryString("codcont").ToString,
                                        AgroKey_EncoderDecoder,
                                        Server)
        End If

        If Not String.IsNullOrWhiteSpace(Request.QueryString("id_Cf")) Then
            Qs_IdCf = Stringa_Decodifica(Request.QueryString("id_Cf").ToString,
                                       AgroKey_EncoderDecoder,
                                       Server)
            xId_Cf = Qs_IdCf
        End If

        If Not String.IsNullOrWhiteSpace(Request.QueryString("piva")) Then
            Qs_Piva = Stringa_Decodifica(Request.QueryString("piva").ToString,
                                        AgroKey_EncoderDecoder,
                                        Server)
        End If

        If Not String.IsNullOrWhiteSpace(Request.QueryString("apertodaGiasNG")) Then
            Qs_apertodaGiasNG = Stringa_Decodifica(Request.QueryString("apertodaGiasNG").ToString,
                                                    AgroKey_EncoderDecoder,
                                                    Server)
        End If

        If Qs_apertodaGiasNG = "True" OrElse Qs_apertodaGiasNG = "true" Then
            hf_apertodaGiasNG.Value = "True"
        Else
            hf_apertodaGiasNG.Value = "False"
        End If

        ApriDatiPatentino = Request.QueryString.AllKeys.Contains("datiPat")
        AperturaDaPopup = Request.QueryString.AllKeys.Contains("orig")

        If AperturaDaPopup Then
            Master.flag_MostraHeader = False
            Master.flag_MostraFooter = False
        End If

        Dim objParametriAgenda As New ParametriAgenda

        ' Fisso la piva
        objParametriAgenda = New ParametriAgenda
        If String.IsNullOrEmpty(Qs_Piva) Then
            xPiva = objParametriAgenda.Piva
        Else
            xPiva = Qs_Piva
        End If
        hf_Piva_Azienda.Value = xPiva

        xSa_Cod = objParametriAgenda.Sa_Cod

        ' Fisso Cod_Contatto
        If String.IsNullOrEmpty(Qs_CodContatto) Then
            xCod_Contatto = objParametriAgenda.Cod_Contatto
        Else
            If Qs_CodContatto <> "0" Then
                xCod_Contatto = Qs_CodContatto
            Else
                xCod_Contatto = ""
            End If
        End If
        hf_xCodContatto.Value = xCod_Contatto

        'Fisso tipo operazione
        If String.IsNullOrEmpty(Qs_Operazione) Then
            XTipoOperazione = objParametriAgenda.Tipo_Operazione
        Else
            XTipoOperazione = CInt(Qs_Operazione)
        End If
        hf_TipoOperazioneContatto.Value = XTipoOperazione

        Select Case XTipoOperazione
            Case enum_TipoOperazioneDB.Scrittura
                Master.Lbl_Titolo.Text = AgronicaAgenda_2010.CreazioneNuovoContatto
                objParametriAgenda.Sa_Cod = 0
            Case enum_TipoOperazioneDB.Lettura
                Master.Lbl_Titolo.Text = DirectCast(GetLocalResourceObject("LetturaContatto"), String)
            Case enum_TipoOperazioneDB.Modifica
                Master.Lbl_Titolo.Text = AgronicaAgenda_2010.ModificaContatto
        End Select

        ' Impresa Gias
        hf_ImpresaGias.Value = False

        '==================================
        '======= VERIFICA PERMESSI ========
        '==================================

        Dim UtenteAbilitato_Lettura As Boolean = False

        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim UtenteAbilitato_Modifica As Boolean = False
        UtenteAbilitato_Lettura = permessi.getPermesso(enum_Security_Attivita.Anagrafica_Contatto).Lettura
        UtenteAbilitato_Modifica = permessi.getPermesso(enum_Security_Attivita.Anagrafica_Contatto).Scrittura

        Dim UtenteAbiliato_Modifica_Associazione_Listini As Boolean = False
        UtenteAbiliato_Modifica_Associazione_Listini = permessi.getPermesso(enum_Security_Attivita.Gestione_Listini).Scrittura
        hf_UtenteAbiliato_Modifica_Associazione_Listini.Value = UtenteAbiliato_Modifica_Associazione_Listini

        Session("UtenteAbilitato_Modifica") = UtenteAbilitato_Modifica
        Session("UtenteAbilitato_Lettura") = UtenteAbilitato_Lettura

        If Not UtenteAbilitato_Modifica Then
            Response.Redirect("../MenuAnagrafica/Menubs_anagrafica.aspx")
            Exit Sub
        End If

        Dim impDict As New Dictionary(Of String, Object)
        hf_OpzioniContatti.Value = CaricaImpostazioniUtente(impDict)

        HttpContext.Current.Session("Dt_Rubrica") = Nothing
        HttpContext.Current.Session("RapportiContabili_X_Contatto_Leggi") = Nothing
        HttpContext.Current.Session("DT_Rapporti") = Nothing
        HttpContext.Current.Session("Dt_Indirizzi") = Nothing

        Dim UtenteAbilitato_Pubblico As Boolean = False
        UtenteAbilitato_Pubblico = objPermessi.Controlla_Permessi_Utente(
                                Session("ASG_Utente_Username"),
                                Session("ASG_IdServizio"),
                                enum_Security_Attivita.Modifica_Contatti_Pubblici,
                                enum_Security_Operazione.Modifica,
                                Date.Now,
                                "",
                                objParametri_Utenti)

        Session("UtenteAbilitato_Pubblico") = UtenteAbilitato_Pubblico

        If Not Page.IsPostBack Then


            GeneraDTIndirizzi()
            GeneraDTRapportiContabili()

            ViewState("nPrezzi") = -1

            HttpContext.Current.Session("RBL_TipoUtente") = 0

            Dim paginaRedirect As String = Master.TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAnagrafica, objParametriAgenda)
            hdPaginaRedirect.Value = paginaRedirect

            Dim Dt_Rapporti As DataTable
            Dim Dt_Indirizzi As DataTable

            If XTipoOperazione = enum_TipoOperazioneDB.Scrittura Then

                If Not AperturaDaPopup Then
                    RBL_TipoUtente.SelectedValue = PERSONA_FISICA.ToString()
                Else

                    If (xId_Cf <> -1) Then
                        RBL_TipoUtente.SelectedValue = xId_Cf.ToString()
                    Else

                        Select Case Qs_LavCod
                            Case LAVCOD_FATTURA_EMESSA, LAVCOD_BOLLA_EMESSA, LAVCOD_FATTURA_RICEVUTA, LAVCOD_BOLLA_RICEVUTA, LAVCOD_NOTA_ACCREDITO_EMESSA, LAVCOD_NOTA_ACCREDITO_RICEVUTA
                                RBL_TipoUtente.SelectedValue = PERSONA_GIURIDICA.ToString()
                                RBL_TipoUtente.Enabled = False
                            Case Else
                                RBL_TipoUtente.SelectedValue = PERSONA_FISICA.ToString()
                        End Select

                    End If

                End If
                hf_IDCF.Value = RBL_TipoUtente.SelectedValue


                'solo le persone fisiche rendo possibile l'assegnazione ad un centro
                AgronicaCoreUtility.CaricaListControl.Centri_Aziendali(Cmb_CentriAziendali, True, AgronicaAgenda_2010.ContattoAziendale, "0", xPiva, False, 2, "", "", objParametri_Server)
                Select Case Session("UtenteAbilitato_Pubblico")
                    Case True
                        Cmb_CentriAziendali.Items.Add(New ListItem(AgronicaAgenda_2010.ContattoMovimentabileDaTutteLeImprese, "-1"))
                End Select

                Dt_Rapporti = Nothing

                GeneraDTRapportiContabili()

                Dt_Rapporti = ViewState("Dt_Rapporti")
                Dt_Indirizzi = ViewState("Dt_Indirizzi")
                CaricaCosti(Nothing, "", "")
                CaricaRubricaNew(Nothing)
                CaricaLiquidita(Nothing)
                CaricaConti(Nothing)

                HttpContext.Current.Session("Dt_Rapporti") = Dt_Rapporti
                HttpContext.Current.Session("Dt_Indirizzi") = Dt_Indirizzi

                rowModificaCF.Visible = False
                rowModificaPiva.Visible = False

            ElseIf XTipoOperazione = enum_TipoOperazioneDB.Modifica OrElse XTipoOperazione = enum_TipoOperazioneDB.Lettura Then

                '##############################################################
                '#####  Se sono in MODIFICA carico i dati  ####################
                '##############################################################

                If XTipoOperazione = enum_TipoOperazioneDB.Modifica Then
                    rowModificaCF.Visible = True
                    rowModificaPiva.Visible = True
                End If

                Dim objContatto_R As New AgronicaCoreAnagrafeBIZ.Contatti_MultiHost_R
                Dim Dati As String
                Dati = objContatto_R.Contatto_Leggi(xPiva, xCod_Contatto, "", False, objParametri_Server)

                'sono in info di un contatto legato ad un'altra azienda
                If Dati = "" Then
                    Dati = objContatto_R.Contatto_Leggi("", xCod_Contatto, "", False, objParametri_Server)
                End If

                If Dati <> "" Then

                    Dim doc As XDocument
                    doc = XDocument.Parse(Dati)
                    Dim risultato = (From c In doc.<DatiContatti>.<Contatto>
                                     Select c).FirstOrDefault

                    'controllo se è un impresa gias
                    Dim objA As New AgronicaCoreAnagrafeDAL.Imprese_Read
                    Dim isGIAS = objA.VerificaEsistenza_PivaGIAS(risultato.@cod_contatto, objParametri_Server)
                    hf_ImpresaGias.Value = isGIAS

                    doc = XDocument.Parse(Dati)

                    Cmb_CentriAziendali.Items.Clear()

                    ImgBtn_CF.Disabled = True

                    hf_IDCF.Value = risultato.@id_cf

                    Select Case risultato.@id_cf.ToString()
                        Case PERSONA_GIURIDICA

                            RBL_TipoUtente.SelectedValue = 1
                            Cmb_CentriAziendali.Items.Add(New ListItem(AgronicaAgenda_2010.ContattoAziendale, "0"))
                            Select Case Session("UtenteAbilitato_Pubblico")
                                Case True
                                    Cmb_CentriAziendali.Items.Add(New ListItem(AgronicaAgenda_2010.ContattoMovimentabileDaTutteLeImprese, "-1"))
                            End Select
                            Txt_Piva.Text = risultato.@cod_contatto
                            Txt_Rag_Soc.Text = risultato.@rag_soc
                            Txt_Piva.Enabled = False
                            Txt_CF_Estero.Text = risultato.@codice_fiscale
                            Txt_Nome_Breve_Azienda.Text = risultato.@nome_breve

                        Case PERSONA_FISICA

                            RBL_TipoUtente.SelectedValue = 0
                            Txt_CF.Text = risultato.@cod_contatto
                            Txt_Nome.Text = risultato.@nome
                            Txt_Cognome.Text = risultato.@cognome
                            If Txt_Nome.Text = "" AndAlso Txt_Cognome.Text = "" Then
                                Txt_Nome.Text = risultato.@rag_soc
                            End If
                            Txt_Nome_Breve_Persona.Text = risultato.@nome_breve
                            If risultato.@data_nascita <> AGRODATAINIZIO Then
                                Txt_DataNascita.Text = risultato.@data_nascita
                            End If
                            If Not String.IsNullOrEmpty(risultato.@sesso) Then
                                ddl_Sesso.SelectedValue = risultato.@sesso
                            End If

                            Txt_CF.Enabled = False

                            'solo le persone fisiche rendo possibile l'assegnazione ad un centro
                            AgronicaCoreUtility.CaricaListControl.Centri_Aziendali(Cmb_CentriAziendali, True, AgronicaAgenda_2010.ContattoAziendale, "0", xPiva, False, 2, "", "", objParametri_Server)
                            Select Case Session("UtenteAbilitato_Pubblico")
                                Case True
                                    Cmb_CentriAziendali.Items.Add(New ListItem(AgronicaAgenda_2010.ContattoMovimentabileDaTutteLeImprese, "-1"))
                            End Select

                        Case CONTATTO_ESTERO

                            RBL_TipoUtente.SelectedValue = 1
                            Cmb_CentriAziendali.Items.Add(New ListItem(AgronicaAgenda_2010.ContattoAziendale, "0"))
                            Select Case Session("UtenteAbilitato_Pubblico")
                                Case True
                                    Cmb_CentriAziendali.Items.Add(New ListItem(AgronicaAgenda_2010.ContattoMovimentabileDaTutteLeImprese, "-1"))
                            End Select
                            Txt_Piva.Text = risultato.@cod_contatto
                            Txt_Rag_Soc.Text = risultato.@rag_soc
                            Txt_Nome_Breve_Azienda.Text = risultato.@nome_breve
                            Txt_Piva.Enabled = False
                            Txt_CF_Estero.Text = risultato.@codice_fiscale

                    End Select

                    Me.Cmb_CentriAziendali.SelectedIndex =
                    Cmb_CentriAziendali.Items.IndexOf(
                        Cmb_CentriAziendali.Items.FindByValue(risultato.@sa_cod))

                    If risultato.@sa_cod <> "0" AndAlso risultato.@sa_cod <> "-1" Then
                        Me.Cmb_CentriAziendali.Enabled = False
                    End If

                    hf_Convenevoli.Value = risultato.@convenevoli

                    CaricaAltriDati(risultato)
                    CaricaDettagliContabili(risultato)
                    CaricaIndirizzi(doc, Dt_Indirizzi, xPiva)
                    CaricaRubricaNew(doc)
                    CaricaCosti(doc, xPiva, xCod_Contatto)
                    CaricaContattiCodici(doc)
                    CaricaLiquidita(doc)
                    CaricaConti(doc)

                    Dim Patentino As String = ""
                    Dim Patentino_Rilascio As String = ""
                    Dim Patentino_Scadenza As String = ""
                    Dim Patentino_Ente As String = ""
                    Dim nrBadge As String = ""
                    Dim EUDR As Boolean = False

                    Dim ID As Integer
                    Dim ArrayID() As Integer
                    Dim i_id As Integer = 0
                    Dim UdmCod_Prezzo As Integer
                    Dim UdmDes_Prezzo As String
                    Dim InizioPrezzo As Date
                    Dim FinePrezzo As Date
                    Dim Prezzo As Decimal

                    'controllo il rapporto contabile
                    Dim risultato_Rapp = (From c In doc.<DatiContatti>.<Contatto>.<RapCon>
                                          Select c).ToList
                    If Not ViewState("Dt_Rapporti") Is Nothing Then

                        Dt_Rapporti = ViewState("Dt_Rapporti")

                        For i = 0 To risultato_Rapp.Count - 1

                            For j = 0 To Dt_Rapporti.Rows.Count - 1
                                If risultato_Rapp(i).@cod_rapporto = Dt_Rapporti.Rows(j).Item("cod_rapporto") Then
                                    Dt_Rapporti.Rows(j).Item("Cod_RisUm") = risultato_Rapp(i).@cod_risum
                                    If risultato_Rapp(i).@validita_inizio = AGRODATAINIZIO Then
                                        Dt_Rapporti.Rows(j).Item("validita_inizio") = ""
                                    Else
                                        Dt_Rapporti.Rows(j).Item("validita_inizio") = risultato_Rapp(i).@validita_inizio
                                    End If
                                    If risultato_Rapp(i).@validita_fine = AGRODATAFINE Then
                                        Dt_Rapporti.Rows(j).Item("validita_fine") = ""
                                    Else
                                        Dt_Rapporti.Rows(j).Item("validita_fine") = risultato_Rapp(i).@validita_fine
                                    End If
                                    Dt_Rapporti.Rows(j).Item("checked") = 1
                                    Dim risultato_Costi = (From c In risultato_Rapp(i).<DatiProdotti_Costi>.<Prodotto_Costo>
                                                           Select c).ToList


                                    For x = 0 To risultato_Costi.Count - 1

                                        ID = risultato_Costi(x).@id
                                        ReDim Preserve ArrayID(i_id)
                                        ArrayID(i_id) = ID
                                        i_id += 1

                                        UdmCod_Prezzo = risultato_Costi(x).@mezzo
                                        Select Case UdmCod_Prezzo
                                            Case enum_TipoMezzo.Ettaro
                                                UdmDes_Prezzo = "HA"
                                            Case Else
                                                UdmDes_Prezzo = "ORA"
                                        End Select
                                        InizioPrezzo = risultato_Costi(x).@validita_inizio
                                        FinePrezzo = risultato_Costi(x).@validita_fine
                                        Prezzo = risultato_Costi(x).@prezzo_unitario

                                    Next

                                    Exit For

                                End If
                            Next

                            If risultato.@id_cf = PERSONA_FISICA Then
                                If Patentino = "" Then
                                    Patentino = risultato_Rapp(i).@patentino
                                    If risultato_Rapp(i).@data_rilascio_patentino <> AGRODATAINIZIO Then
                                        Patentino_Rilascio = risultato_Rapp(i).@data_rilascio_patentino
                                    End If
                                    If risultato_Rapp(i).@data_scadenza_patentino <> AGRODATAFINE Then
                                        Patentino_Scadenza = risultato_Rapp(i).@data_scadenza_patentino
                                    End If
                                    Patentino_Ente = risultato_Rapp(i).@ente_di_rilascio
                                End If
                            End If

                        Next

                    End If
                    Txt_Badge.Text = risultato.@nrBadge

                    hf_EUDR.Value = False
                    If Not IsNothing(risultato.@eudr) AndAlso IsNumeric(risultato.@eudr) Then
                        hf_EUDR.Value = Convert.ToBoolean(CInt(risultato.@eudr))
                    End If

                    ViewState("ArrayID") = ArrayID
                    ViewState("Dt_Rapporti") = Dt_Rapporti
                    HttpContext.Current.Session("Dt_Rapporti") = Dt_Rapporti

                End If
            End If

            jsRapporti = DT_to_Json_Rapporti(Dt_Rapporti)
            jsIndirizzi = DT_to_Json_Indirizzi(Dt_Indirizzi, XTipoOperazione)
            jsIndirizziNew = GetJson(Dt_Indirizzi)

            ' Disabilito tutti i controlli se in Lettura
            If XTipoOperazione = enum_TipoOperazioneDB.Lettura Then

                UtenteAbilitato_Modifica = False
                RBL_TipoUtente.Enabled = False
                Cmb_CentriAziendali.Enabled = False
                Txt_CF.Enabled = False
                Txt_Cognome.Enabled = False
                Txt_CF_Estero.Enabled = False
                Txt_Nome.Enabled = False
                Txt_DataNascita.Enabled = False
                ddl_Sesso.Enabled = False
                Txt_Rag_Soc.Enabled = False
                Txt_Badge.Enabled = False
                ImgBtn_CF.Visible = False
                ImageButton1.Visible = False
                Txt_Codice_SDI.Enabled = False
                Txt_Pec.Enabled = False
                txt_dich_intenti_protocollo.Enabled = False
                'Ddl_Rappresentante_Fiscale.Enabled = False
                Ddl_Tipo_Contatto.Enabled = False
                Txt_Cod_Accisa.Enabled = False
                Txt_Cod_UA.Enabled = False
                Txt_Cod_Conto_Gar.Enabled = False
                Txt_Rif_Dep_Fisc.Enabled = False
            End If

            hf_UtenteAbilitatoLettura.Value = UtenteAbilitato_Lettura
            hf_UtenteAbilitatoScrittura.Value = UtenteAbilitato_Modifica
            hf_AperturaDaPoup.Value = AperturaDaPopup
            hf_ApriDatiPatentino.Value = ApriDatiPatentino
        End If

    End Sub

    Private Function CaricaImpostazioniUtente(ByRef impDict As Dictionary(Of String, Object)) As String

        'leggo e Jsonizzo le impostazioni
        Dim obj As New AgronicaCoreUtentiBIZ.Utenti_Impostazioni_R
        impDict = obj.LeggiOpzioni_Contatti(objParametri_Utenti)

        Return JsonConvert.SerializeObject(impDict, Newtonsoft.Json.Formatting.None)

    End Function
    Private Sub GeneraDTRapportiContabili()

        Dim DT As New DataTable
        Dim Dr As DataRow

        DT.Columns.Add(New DataColumn("Rapporto_Des", GetType(String)))
        DT.Columns.Add(New DataColumn("Cod_Rapporto", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))

        DT.Columns.Add(New DataColumn("Cod_RisUm", GetType(Integer)))
        DT.Columns.Add(New DataColumn("validita_inizio", GetType(String)))
        DT.Columns.Add(New DataColumn("validita_fine", GetType(String)))
        DT.Columns.Add(New DataColumn("checked", GetType(Integer)))
        Dim Dt_Rapporti As New DataTable
        Dim i As Integer

        Dim ObjRapp As New AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R
        Dt_Rapporti = ObjRapp.Contatti_RapportiContabili_Leggi(SACOD_CONTATTO_NONDEFINITO,
                                                  0,
                                                  False, False, False, False, False, False, False,
                                                   AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                   "",
                                                   "",
                                                   objParametri_Server)

        For i = 0 To Dt_Rapporti.Rows.Count - 1
            Dr = DT.NewRow
            Dr.Item("Cod_Rapporto") = Dt_Rapporti.Rows(i).Item("Cod_Rapporto")
            Dr.Item("Rapporto_Des") = Dt_Rapporti.Rows(i).Item("Rapporto_Des")
            Dr.Item("Sa_Cod") = Dt_Rapporti.Rows(i).Item("Sa_Cod")
            Dr.Item("Cod_RisUm") = 0
            Dr.Item("validita_inizio") = AGRODATAINIZIO
            Dr.Item("validita_fine") = AGRODATAFINE
            Dr.Item("checked") = 0
            DT.Rows.Add(Dr)
        Next

        ViewState("Dt_Rapporti") = DT

    End Sub

    Private Sub GeneraDtRubrica()

        Dim DT As New DataTable
        DT.Columns.Add("Cod_Indirizzo", Type.GetType("System.Int32"))
        DT.Columns.Add("Tipo_Indirizzo", Type.GetType("System.Int32"))
        DT.Columns.Add("Tipo_Indirizzo_Desc", Type.GetType("System.String"))
        DT.Columns.Add("Via", Type.GetType("System.String"))
        DT.Columns.Add("Frazione", Type.GetType("System.String"))
        DT.Columns.Add("Provincia_des", Type.GetType("System.String"))
        DT.Columns.Add("Provincia_cod", Type.GetType("System.String"))
        DT.Columns.Add("Comune_des", Type.GetType("System.String"))
        DT.Columns.Add("Comune_cod", Type.GetType("System.String"))
        DT.Columns.Add("Cap", Type.GetType("System.String"))
        DT.Columns.Add("Sigla_Prov", Type.GetType("System.String"))
        DT.Columns.Add("Stato", Type.GetType("System.String"))
        DT.Columns.Add("Note", Type.GetType("System.String"))

        'Vettore di DataColumn
        Dim DtKeys(1) As DataColumn

        'Valorizzo le celle del vettore
        DtKeys(0) = DT.Columns("Cod_Indirizzo")
        DtKeys(1) = DT.Columns("Tipo_Indirizzo")

        'Assegno il vettore delle chiavi al DataTable
        DT.PrimaryKey = DtKeys

        ViewState("Dt_Rubrica") = DT
        HttpContext.Current.Session("Dt_Rubrica") = DT

    End Sub

    Private Sub GeneraDTIndirizzi()

        Dim DT As New DataTable
        DT.Columns.Add("Cod_Indirizzo", Type.GetType("System.Int32"))
        DT.Columns.Add("Tipo_Indirizzo", Type.GetType("System.Int32"))
        DT.Columns.Add("Tipo_Indirizzo_Desc", Type.GetType("System.String"))
        DT.Columns.Add("Via", Type.GetType("System.String"))
        DT.Columns.Add("Frazione", Type.GetType("System.String"))
        DT.Columns.Add("Provincia_des", Type.GetType("System.String"))
        DT.Columns.Add("Provincia_cod", Type.GetType("System.String"))
        DT.Columns.Add("Comune_des", Type.GetType("System.String"))
        DT.Columns.Add("Comune_cod", Type.GetType("System.String"))
        DT.Columns.Add("Cap", Type.GetType("System.String"))
        DT.Columns.Add("Sigla_Prov", Type.GetType("System.String"))
        DT.Columns.Add("Stato", Type.GetType("System.String"))
        DT.Columns.Add("Stato_Des", Type.GetType("System.String"))
        DT.Columns.Add("Note", Type.GetType("System.String"))
        DT.Columns.Add("Codice_Lingua", Type.GetType("System.String"))
        DT.Columns.Add("Lingua_Des", Type.GetType("System.String"))
        DT.Columns.Add("Citta_Des", Type.GetType("System.String"))


        'Vettore di DataColumn
        Dim DtKeys(1) As DataColumn

        'Valorizzo le celle del vettore
        DtKeys(0) = DT.Columns("Cod_Indirizzo")
        DtKeys(1) = DT.Columns("Tipo_Indirizzo")

        'Assegno il vettore delle chiavi al DataTable
        DT.PrimaryKey = DtKeys

        ViewState("Dt_Indirizzi") = DT
        HttpContext.Current.Session("DT_Indirizzo") = DT

    End Sub

    Private Sub GeneraDTCosti()

        Dim DT As New DataTable

        '----- Definisco la struttura del DataTable
        DT.Columns.Add(New DataColumn("ID", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Udm_Cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Udm_Des", GetType(String)))
        DT.Columns.Add(New DataColumn("strValidita_Inizio", GetType(String)))
        DT.Columns.Add(New DataColumn("strValidita_Fine", GetType(String)))
        DT.Columns.Add(New DataColumn("Validita_Inizio", GetType(Date)))
        DT.Columns.Add(New DataColumn("Validita_Fine", GetType(Date)))
        DT.Columns.Add(New DataColumn("Prezzo_Unitario", GetType(String)))

        'Vettore di DataColumn
        Dim DtKeys(0) As DataColumn

        'Valorizzo le celle del vettore
        DtKeys(0) = DT.Columns("ID")

        'Assegno il vettore delle chiavi al DataTable
        DT.PrimaryKey = DtKeys

    End Sub


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaTipologiaIndirizzo(
                                                   ByVal piva As String,
                                                   ByVal tipo_persona As Integer,
                                                   ByVal codContatto As String,
                                                   ByVal itaEste As Integer) As String

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            'r.Sessione = False
            'Return r
            'TODO
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim tipIndDal = New IndirizzoTipo_R()

        Dim indirizzi = TipiIndirizzoToList(tipIndDal.Leggi(piva, -1, codContatto, tipo_persona, "", "", objParametri_Server)).ToList()
        Dim ddl_tipo_Indirizzo As New DropDownList

        Select Case tipo_persona
            Case PERSONA_FISICA
                ddl_tipo_Indirizzo.Items.Add(New ListItem(AgronicaAgenda_2010.Residenza, 3))
                ddl_tipo_Indirizzo.Items.Add(New ListItem(AgronicaAgenda_2010.LuogoDiNascita, 5))
                ddl_tipo_Indirizzo.Items.Add(New ListItem(AgronicaAgenda_2010.Domicilio, 2))
                ddl_tipo_Indirizzo.Items.Add(New ListItem(AgronicaAgenda_2010.ResidenzaEstiva, 4))
                For Each i As Object In indirizzi
                    ddl_tipo_Indirizzo.Items.Add(New ListItem(i.Descrizione, i.IndirizzoTipo_Cod))
                Next

                Dim lista As New List(Of String)
                lista.Add(3)
                lista.Add(5)
                lista.Add(2)
                lista.Add(4)
                For Each i As Object In indirizzi
                    lista.Add(i.IndirizzoTipo_Cod)
                Next
                HttpContext.Current.Session("lista_indirizzi") = lista
                'Session("lista_indirizzi") = lista
            Case PERSONA_GIURIDICA
                ddl_tipo_Indirizzo.Items.Add(New ListItem(AgronicaAgenda_2010.SedeOperativa, 1))
                ddl_tipo_Indirizzo.Items.Add(New ListItem(AgronicaAgenda_2010.SedeLegale, 101))
                ddl_tipo_Indirizzo.Items.Add(New ListItem(AgronicaAgenda_2010.SedeAziendale, 102))
                ddl_tipo_Indirizzo.Items.Add(New ListItem(AgronicaAgenda_2010.Stabilimento, 103))

                ' Se contatto estero aggiungo la stabile organizzazione
                If itaEste = 2 Then
                    ddl_tipo_Indirizzo.Items.Add(New ListItem(DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/New_Contatto_Edit.aspx", "StabileOrganizzazione"), String), 201))
                End If

                For Each i As Object In indirizzi
                    ddl_tipo_Indirizzo.Items.Add(New ListItem(i.Descrizione, i.IndirizzoTipo_Cod))
                Next

                Dim lista As New List(Of String)
                lista.Add(1)
                lista.Add(101)
                lista.Add(102)
                lista.Add(103)
                If itaEste = 2 Then
                    lista.Add(201)
                End If

                For Each i As Object In indirizzi
                    lista.Add(i.IndirizzoTipo_Cod)
                Next
                HttpContext.Current.Session("lista_indirizzi") = lista
        End Select

        Dim rval As String = ""
        For Each itm As ListItem In ddl_tipo_Indirizzo.Items
            rval &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
        Next

        Return rval

    End Function


    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaTipologiaIndirizzoKendo(
                                                   ByVal piva As String,
                                                   ByVal tipo_persona As Integer,
                                                   ByVal codContatto As String,
                                                   ByVal itaEste As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        ' Dim NomeRoutine = "AgronicaAgenda_2010.New_Contatto_Edit.CaricaTipologiaIndirizzoKendo()"

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim DT_Indirizzi As New DataTable
        Dim jarrayIndirizzi As New JArray()


        Dim tipIndPersonalizzatiDal = New IndirizzoTipo_R()

        Dim indirizziPersonalizzati = TipiIndirizzoToList(tipIndPersonalizzatiDal.Leggi(piva, -1, codContatto, tipo_persona, "", "", objParametri_Server)).ToList()

        Try

            Dim tipIndDal = New IndirizzoTipo_R

            Dim indirizzi = TipiIndirizzoToList(tipIndDal.Leggi(piva, -1, codContatto, tipo_persona, "", "", objParametri_Server)).ToList()

            DT_Indirizzi.Columns.Add(New DataColumn("Tipo_Indirizzo", GetType(Integer)))
            DT_Indirizzi.Columns.Add(New DataColumn("Tipo_Indirizzo_Desc", GetType(String)))

            Select Case tipo_persona
                Case PERSONA_FISICA
                    Dim d = DT_Indirizzi.NewRow
                    d("Tipo_Indirizzo_Desc") = AgronicaAgenda_2010.Residenza
                    d("Tipo_Indirizzo") = 3
                    DT_Indirizzi.Rows.Add(d)

                    d = DT_Indirizzi.NewRow
                    d("Tipo_Indirizzo_Desc") = AgronicaAgenda_2010.LuogoDiNascita
                    d("Tipo_Indirizzo") = 5
                    DT_Indirizzi.Rows.Add(d)

                    d = DT_Indirizzi.NewRow
                    d("Tipo_Indirizzo_Desc") = AgronicaAgenda_2010.Domicilio
                    d("Tipo_Indirizzo") = 2
                    DT_Indirizzi.Rows.Add(d)

                    d = DT_Indirizzi.NewRow
                    d("Tipo_Indirizzo_Desc") = AgronicaAgenda_2010.ResidenzaEstiva
                    d("Tipo_Indirizzo") = 4
                    DT_Indirizzi.Rows.Add(d)

                Case PERSONA_GIURIDICA
                    Dim d = DT_Indirizzi.NewRow
                    d("Tipo_Indirizzo_Desc") = AgronicaAgenda_2010.SedeOperativa
                    d("Tipo_Indirizzo") = 1
                    DT_Indirizzi.Rows.Add(d)

                    d = DT_Indirizzi.NewRow
                    d("Tipo_Indirizzo_Desc") = AgronicaAgenda_2010.SedeLegale
                    d("Tipo_Indirizzo") = 101
                    DT_Indirizzi.Rows.Add(d)

                    d = DT_Indirizzi.NewRow
                    d("Tipo_Indirizzo_Desc") = AgronicaAgenda_2010.SedeAziendale
                    d("Tipo_Indirizzo") = 102
                    DT_Indirizzi.Rows.Add(d)

                    d = DT_Indirizzi.NewRow
                    d("Tipo_Indirizzo_Desc") = AgronicaAgenda_2010.Stabilimento
                    d("Tipo_Indirizzo") = 103
                    DT_Indirizzi.Rows.Add(d)
            End Select

            If (itaEste = 2) Then
                Dim d = DT_Indirizzi.NewRow
                d = DT_Indirizzi.NewRow
                d("Tipo_Indirizzo_Desc") = "Stabile Organizzazione"
                d("Tipo_Indirizzo") = 201
                DT_Indirizzi.Rows.Add(d)
            End If



            For Each dr As DataRow In DT_Indirizzi.Rows

                Dim descrizione = dr("Tipo_Indirizzo_Desc").ToString()
                Dim codice = dr("Tipo_Indirizzo").ToString()

                jarrayIndirizzi.Add(
                            New JObject(
                                New JProperty("Tipo_Indirizzo", codice),
                                New JProperty("Tipo_Indirizzo_Desc", descrizione)))

            Next

            For Each i As Object In indirizziPersonalizzati
                jarrayIndirizzi.Add(
                            New JObject(
                                New JProperty("Tipo_Indirizzo", i.IndirizzoTipo_Cod),
                                New JProperty("Tipo_Indirizzo_Desc", i.Descrizione)))
            Next

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(jarrayIndirizzi, Newtonsoft.Json.Formatting.None)
            'cmb_Stato.Items.Add(New ListItem("SELEZIONA", ""))

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaProvinceKendo(stato As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objListaProv As New AgronicaCoreMetaSchemaDAL.Lista_Province_R

        Try

            Dim DT As New DataTable
            Dim jarrayProvincie As New JArray()


            DT = objListaProv.Leggi("", "", "", "", "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server, stato)

            For Each dr As DataRow In DT.Rows

                Dim Provincia_des = If(dr("PROVINCIA") Is DBNull.Value, "", dr("PROVINCIA").ToString())
                Dim Provincia_cod = If(dr("PROV") Is DBNull.Value, "", dr("PROV").ToString())
                Dim Sigla_Prov = If(dr("SIGLA") Is DBNull.Value, "", dr("SIGLA").ToString())
                Dim REG = If(dr("REG") Is DBNull.Value, "", dr("REG").ToString())

                jarrayProvincie.Add(
                                New JObject(
                                New JProperty("Provincia_des", Provincia_des),
                                New JProperty("Provincia_cod", Provincia_cod),
                                New JProperty("Sigla_Prov", Sigla_Prov),
                                New JProperty("REG", REG)
                                )
                          )

            Next

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(jarrayProvincie, Newtonsoft.Json.Formatting.None)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaComuniKendo(ByVal provincia As String, ByVal comuni_prov As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objComuni As New AgronicaCoreMetaSchemaDAL.Istat_R

        Dim Ordina_Alfabetico1_Istat2 = 1
        Dim xFiltroAggiuntivo = ""
        Dim xOrderBy = ""
        Dim stringaOrderBy As String

        Try

            Dim DT As New DataTable
            Dim jarrayComuni As New JArray()

            'Inserisco l'ordinamento aggiuntivo
            If xOrderBy <> "" Then
                stringaOrderBy = xOrderBy
            Else
                If Ordina_Alfabetico1_Istat2 = 2 Then
                    'Ordino per codice ISTAT
                    stringaOrderBy = "Com asc"
                Else
                    'Ordino i record
                    stringaOrderBy = "Localita asc"
                End If
            End If


            DT = objComuni.Leggi(provincia, "", comuni_prov, "", "", enumSelezioneVariabile.Selezione_TabellaCompleta,
                             xFiltroAggiuntivo, stringaOrderBy, objParametri_Server)

            For Each dr As DataRow In DT.Rows

                Dim Comune_des = If(dr("LOCALITA") Is DBNull.Value, "", dr("LOCALITA").ToString())
                Dim Comune_cod = If(dr("COM") Is DBNull.Value, "", dr("COM").ToString())
                Dim CAP = If(dr("CAP") Is DBNull.Value, "", dr("CAP").ToString())

                jarrayComuni.Add(
                                New JObject(
                             New JProperty("Comune_des", Comune_des),
                                    New JProperty("Comune_cod", Comune_cod),
                                    New JProperty("CAP", CAP)
                                    )
                                )

            Next

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(jarrayComuni, Newtonsoft.Json.Formatting.None)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function















    Private Shared Function TipiIndirizzoToList(ByVal DT As DataTable) As IEnumerable(Of Object)

        Dim lista = From ind In DT.AsEnumerable()
                    Select New With
                    {
                        .IndirizzoTipo_Cod = CInt(ind("IndirizzoTipo_Cod")),
                        .Descrizione = ind("Descrizione")
                    }

        Return lista.ToList()

    End Function

    Private Shared Function Controlla_Liquidita(ByVal piva As String, ByVal codContatto As String, ByVal param As ParametriSalvaTutto, ByRef errori As String) As Boolean

        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim righeArray_Dettaglio As JArray = Nothing
        Dim almenoUnoSbagliato As Boolean = False

        errori = String.Empty
        Dim inseritiModificati As List(Of LiquiditaModel) = New List(Of LiquiditaModel)()
        Dim eliminati As List(Of LiquiditaModel) = New List(Of LiquiditaModel)()
        Dim tutti As List(Of LiquiditaModel) = New List(Of LiquiditaModel)()

        If Not String.IsNullOrEmpty(param.righeInseriteGrid_Liquidita) Then
            inseritiModificati.AddRange(JsonConvert.DeserializeObject(Of List(Of LiquiditaModel))(param.righeInseriteGrid_Liquidita, settingLoc))
        End If
        If Not String.IsNullOrEmpty(param.righeModificateGrid_Liquidita) Then
            inseritiModificati.AddRange(JsonConvert.DeserializeObject(Of List(Of LiquiditaModel))(param.righeModificateGrid_Liquidita, settingLoc))
        End If
        If Not String.IsNullOrEmpty(param.righeCancellateGrid_Liquidita) Then
            eliminati.AddRange(JsonConvert.DeserializeObject(Of List(Of LiquiditaModel))(param.righeCancellateGrid_Liquidita, settingLoc))
        End If
        If Not String.IsNullOrEmpty(param.righeNonCancellateGrid_Liquidita) Then
            tutti.AddRange(JsonConvert.DeserializeObject(Of List(Of LiquiditaModel))(param.righeNonCancellateGrid_Liquidita, settingLoc))
        End If

        If tutti.Any() Then
            Dim selezionati = tutti.Where(Function(s) s.ChkDefault)
            If selezionati.Count() > 1 Then
                errori = DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/New_Contatto_Edit.aspx", "SelezionareUnaSolaCoordinataBancariaDiDefault"), String)
                Return False
            End If
        End If

        If inseritiModificati.Any() Then
            For Each l As LiquiditaModel In inseritiModificati
                If String.IsNullOrEmpty(l.Istituto_Des) Then
                    errori = DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/New_Contatto_Edit.aspx", "SpecificareIstitutoDiCreditoPerOgniRisorsaFinanziaria"), String)
                    Return False
                End If
            Next
        End If

        If eliminati.Any() Then

        End If

        Return True

    End Function

    Private Shared Function Controlla_Conti(ByVal piva As String, ByVal codContatto As String, ByVal param As ParametriSalvaTutto, ByRef errori As String) As Boolean

        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim righeArray_Dettaglio As JArray = Nothing
        Dim almenoUnoSbagliato As Boolean = False

        errori = String.Empty
        Dim inseritiModificati As List(Of ContiModel) = New List(Of ContiModel)()
        Dim eliminati As List(Of ContiModel) = New List(Of ContiModel)()
        Dim tutti As List(Of ContiModel) = New List(Of ContiModel)

        If Not String.IsNullOrEmpty(param.righeInseriteGrid_Conti) Then
            inseritiModificati.AddRange(JsonConvert.DeserializeObject(Of List(Of ContiModel))(param.righeInseriteGrid_Conti, settingLoc))
        End If
        If Not String.IsNullOrEmpty(param.righeModificateGrid_Conti) Then
            inseritiModificati.AddRange(JsonConvert.DeserializeObject(Of List(Of ContiModel))(param.righeModificateGrid_Conti, settingLoc))
        End If
        If Not String.IsNullOrEmpty(param.righeCancellateGrid_Conti) Then
            eliminati.AddRange(JsonConvert.DeserializeObject(Of List(Of ContiModel))(param.righeCancellateGrid_Conti, settingLoc))
        End If
        If Not String.IsNullOrEmpty(param.righeNonCancellateGrid_Conti) Then
            tutti.AddRange(JsonConvert.DeserializeObject(Of List(Of ContiModel))(param.righeNonCancellateGrid_Conti, settingLoc))
        End If

        If inseritiModificati.Any() Then
            For Each c As ContiModel In inseritiModificati
                If String.IsNullOrEmpty(c.Conto_Descr) Then
                    almenoUnoSbagliato = True
                    errori = DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/New_Contatto_Edit.aspx", "RigheContiConDatiMancanti"), String)
                    Return False
                End If
            Next
        End If

        If tutti.Any() Then
            Dim codiciConti = tutti.Select(Function(s) s.Cod_Conto).Distinct().ToList()
            For Each c As Integer In codiciConti
                Dim righeCount = tutti.Where(Function(s) s.Cod_Conto.Value.Equals(c)).Count()
                If (righeCount > 1) Then
                    almenoUnoSbagliato = True
                    Dim contoErrato = tutti.FirstOrDefault(Function(s) s.Cod_Conto.Value.Equals(c))
                    errori = String.Format(DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/New_Contatto_Edit.aspx", "ContoSpecificatoNvolte"), String), contoErrato.Conto_Descr)
                    Return False
                End If
            Next
        End If

        Return True

    End Function

    Private Shared Function Controlla_Efattura(ByVal piva As String, ByVal codContatto As String, ByVal param As ParametriSalvaTutto, ByRef errori As String) As Boolean

        errori = String.Empty

        If Not String.IsNullOrEmpty(param.PEC) Then
            If Not UtilityProvider.VerificaEspressioneRegolare(param.PEC, "", TipiEnumerativi.enum_EspressioniRegolari.RegExp_Email) Then
                errori = DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/New_Contatto_Edit.aspx", "SpecificareUnaMailValidaNelCampoPEC"), String)
                Return False
            End If
        End If

        If Not String.IsNullOrEmpty(param.TipologiaContatto) Then
            Select Case CInt(param.TipologiaContatto)
                Case enumTipoContattoFattura.Privato
                    If Not String.IsNullOrEmpty(param.CodiceSDI) AndAlso param.CodiceSDI.Length <> 7 Then
                        errori = DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/New_Contatto_Edit.aspx", "CodiceSDIPrivatiErrato"), String)
                        Return False
                    End If
                Case enumTipoContattoFattura.PA
                    If Not String.IsNullOrEmpty(param.CodiceSDI) AndAlso param.CodiceSDI.Length <> 6 Then
                        errori = DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/New_Contatto_Edit.aspx", "CodiceSDIPubblicheAmministrazioniErrato"), String)
                        Return False
                    End If
            End Select

        End If

        If Not String.IsNullOrEmpty(param.DichIntentiData) Then
            Dim outData As DateTime = DateTime.MinValue
            If Not DateTime.TryParse(param.DichIntentiData, outData) Then
                errori = DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/New_Contatto_Edit.aspx", "DichiarazioneIntentiDataErrata"), String)
                Return False
            End If

        End If

        Return True

    End Function
    Private Shared Function Controlla_Altri_Dati(ByVal piva As String, ByVal codContatto As String, ByVal param As ParametriSalvaTutto, ByRef errori As String) As Boolean

        errori = String.Empty
        If piva <> codContatto Then

            If Not String.IsNullOrEmpty(param.AltriDati.CodiceAccisa) AndAlso param.AltriDati.TipoDestinazione = "0" Then
                errori = DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/New_Contatto_Edit.aspx", "CodiceAccisaNecessitaTipoDestinazione"), String)
                Return False
            End If

            If String.IsNullOrEmpty(param.AltriDati.CodiceAccisa) AndAlso param.AltriDati.TipoDestinazione <> "0" Then
                errori = DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/New_Contatto_Edit.aspx", "TipoDestinazioneNecessitaCodiceAccisa"), String)
                Return False
            End If
        Else
            If Not String.IsNullOrEmpty(param.AltriDati.CodiceAccisa) AndAlso param.AltriDati.OrigineDestinazione = "0" Then
                errori = DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/New_Contatto_Edit.aspx", "CodiceAccisaNecessitaTipoOrigineSpedizione"), String)
                Return False
            End If

            If String.IsNullOrEmpty(param.AltriDati.CodiceAccisa) AndAlso param.AltriDati.OrigineDestinazione <> "0" Then
                errori = DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/New_Contatto_Edit.aspx", "TipoOrigineSpedizioneNecessitaCodiceAccisa"), String)
                Return False
            End If
        End If

        If Not String.IsNullOrEmpty(param.Memo) Then
            If param.Memo.Length > LEN_MAX_MEMO Then
                errori = String.Format(
                    DirectCast(System.Web.HttpContext.GetLocalResourceObject(
                        "~/Anagrafica/New_Contatto_Edit.aspx", "CampoMemoEccedenteLunghezzaMassima"),
                        String
                    ),
                    param.Memo.Length, LEN_MAX_MEMO, Chr(10)
                )
                Return False
            End If
        End If

        If param.GestisciContabilita Then
            If Not String.IsNullOrEmpty(param.DettagliContabilita.DestinazioneDiversa_Cod) _
                AndAlso param.DettagliContabilita.DestinazioneDiversa_Cod <> "0" AndAlso
                (String.IsNullOrEmpty(param.DettagliContabilita.IndirizzoDestinazioneDiversa_Cod) OrElse
                    param.DettagliContabilita.IndirizzoDestinazioneDiversa_Cod = "0") Then
                errori = DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/New_Contatto_Edit.aspx", "SpecificareIndirizzoDestinazioneDiversa"), String)

                Return False

            End If
        End If

        Return True


    End Function

    Public Shared Function Controlla_IndirizziTipo(ByVal piva As String,
                                                   ByVal param As String, ByRef errori As String) As Boolean

        Dim parametri As ParametriSalvaIndirizziTipoModel = JsonConvert.DeserializeObject(Of ParametriSalvaIndirizziTipoModel)(param)
        errori = String.Empty
        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim inseritiModificati As List(Of InidirizzoTipoModel) = New List(Of InidirizzoTipoModel)
        Dim tutteLeRighe As List(Of InidirizzoTipoModel) = New List(Of InidirizzoTipoModel)
        Dim eliminati As List(Of InidirizzoTipoModel) = New List(Of InidirizzoTipoModel)

        Try
            If Not String.IsNullOrEmpty(parametri.righeInserite) Then
                inseritiModificati.AddRange(JsonConvert.DeserializeObject(Of List(Of InidirizzoTipoModel))(parametri.righeInserite, settingLoc))
            End If
            If Not String.IsNullOrEmpty(parametri.righeModificate) Then
                inseritiModificati.AddRange(JsonConvert.DeserializeObject(Of List(Of InidirizzoTipoModel))(parametri.righeModificate, settingLoc))
            End If
            If Not String.IsNullOrEmpty(parametri.righeCancellate) Then
                inseritiModificati.AddRange(JsonConvert.DeserializeObject(Of List(Of InidirizzoTipoModel))(parametri.righeCancellate, settingLoc))
            End If
            If Not String.IsNullOrEmpty(parametri.righeNonCancellate) Then
                tutteLeRighe.AddRange(JsonConvert.DeserializeObject(Of List(Of InidirizzoTipoModel))(parametri.righeNonCancellate, settingLoc))
            End If

            If tutteLeRighe.Any() Then

                ' Check su descrizioni doppie
                Dim indTipiDescr = tutteLeRighe.Select(Function(s) s.IndirizzoTipoDes.Trim().ToLower().Replace(" ", "")).Distinct().ToList()
                For Each i As String In indTipiDescr
                    Dim righeCount = tutteLeRighe.Where(Function(s) s.IndirizzoTipoDes.Trim.ToLower.Replace(" ", "").Equals(i)).Count()
                    If (righeCount > 1) Then
                        Dim descriErrata = tutteLeRighe.FirstOrDefault(Function(s) s.IndirizzoTipoDes.Trim.ToLower.Replace(" ", "").Equals(i))
                        errori = String.Format(
                            DirectCast(System.Web.HttpContext.GetLocalResourceObject(
                                "~/Anagrafica/New_Contatto_Edit.aspx", "TipoIndirizzoSedeSpecificatoPiùVolte"),
                                String
                            ),
                            descriErrata.IndirizzoTipoDes
                        )
                        Return False
                    End If
                Next

                ' Check su descrizioni mancanti
                For Each it As InidirizzoTipoModel In tutteLeRighe
                    If String.IsNullOrEmpty(it.IndirizzoTipoDes) OrElse String.IsNullOrWhiteSpace(it.IndirizzoTipoDes) Then
                        errori = DirectCast(System.Web.HttpContext.GetLocalResourceObject(
                            "~/Anagrafica/New_Contatto_Edit.aspx", "DescrizioneNonSpecificataPerAlcuniTipiIndirizzoSede"), String)
                        Return False
                    End If
                Next

            End If

            Return True

        Catch ex As Exception
            errori = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

    End Function

    Private Shared Function Controlla_Costi(ByVal piva As String, ByVal codContatto As String, ByVal param As ParametriSalvaTutto, ByRef errori As String) As Boolean

        errori = String.Empty

        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim inseritiModificati As New List(Of CostiModel)()
        Dim tutteLeRighe As New List(Of CostiModel)()
        Dim almenoUnoSbagliato As Boolean = False

        If Not String.IsNullOrEmpty(param.righeInseriteGrid_Costi) Then
            inseritiModificati.AddRange(JsonConvert.DeserializeObject(Of List(Of CostiModel))(param.righeInseriteGrid_Costi, settingLoc))
        End If
        If Not String.IsNullOrEmpty(param.righeModificateGrid_Costi) Then
            inseritiModificati.AddRange(JsonConvert.DeserializeObject(Of List(Of CostiModel))(param.righeModificateGrid_Costi, settingLoc))
        End If

        If inseritiModificati.Any() Then
            For Each c As CostiModel In inseritiModificati
                If c.Cod_RisUm Is Nothing OrElse c.Udm_Cod Is Nothing OrElse c.Prezzo Is Nothing Then
                    almenoUnoSbagliato = True
                    errori = DirectCast(System.Web.HttpContext.GetLocalResourceObject(
                        "~/Anagrafica/New_Contatto_Edit.aspx", "AttenzioneUnoOPiùCostiNonCompilatiCorrettamente"), String)
                    Exit For
                End If
            Next
        End If

        If almenoUnoSbagliato Then
            Return False
        End If


        If Not String.IsNullOrEmpty(param.righeNonCancellateGrid_Costi) Then
            tutteLeRighe.AddRange(JsonConvert.DeserializeObject(Of List(Of CostiModel))(param.righeNonCancellateGrid_Costi, settingLoc))
        End If

        If tutteLeRighe.Any() Then

            Dim risUms = tutteLeRighe.Select(Function(i) i.Cod_RisUm).Distinct().ToList()

            For Each ru As String In risUms
                Dim costiXRisUm = tutteLeRighe.Where(Function(c) c.Cod_RisUm = ru).ToList()

                If Not costiXRisUm Is Nothing AndAlso costiXRisUm.Count() > 0 Then

                    Dim periodo = New Period(
                            costiXRisUm.FirstOrDefault().InizioPrezzo.Value,
                            costiXRisUm.FirstOrDefault().FinePrezzo.Value)

                    For i As Integer = 1 To costiXRisUm.Count() - 1
                        Dim peridoConfronto = New Period(
                                                costiXRisUm(i).InizioPrezzo.Value,
                                                costiXRisUm(i).FinePrezzo.Value)

                        ' Controllo se le date sono intersecate
                        If periodo.IntersectsWith(peridoConfronto) Then
                            almenoUnoSbagliato = True
                            errori = DirectCast(System.Web.HttpContext.GetLocalResourceObject(
                                "~/Anagrafica/New_Contatto_Edit.aspx", "LeDateInseriteSiIntersecanoConAlcunePrecedentementeInserite"), String)
                            Exit For
                        End If

                    Next

                End If
            Next
        End If

        If almenoUnoSbagliato Then
            Return False
        End If

        Controlla_Costi = True

    End Function

    Private Shared Function Controlla_IndirizziKendo(ByVal param As ParametriSalvaTutto,
                                                     ByRef errori As String) As Boolean

        errori = ""
        Dim retVal As Boolean = True
        Dim arrErrori As New JArray

        ' Contatto Italiano
        If param.ItaEste = 0 Then
            If (Not IsNothing(param.righeInseriteGrid_Indirizzi) AndAlso param.righeInseriteGrid_Indirizzi <> "" AndAlso Not IsNothing(param.righeModificateGrid_Indirizzi) AndAlso param.righeModificateGrid_Indirizzi <> "") Then

                Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

                'Dim inseritiModificati As New List(Of IndirizzoModel)()
                Dim tutteLeRighe As New List(Of IndirizzoModel)()
                Dim righeNonCancellate As New List(Of IndirizzoModel)

                tutteLeRighe.AddRange(JsonConvert.DeserializeObject(Of List(Of IndirizzoModel))(param.righeInseriteGrid_Indirizzi, settingLoc))
                tutteLeRighe.AddRange(JsonConvert.DeserializeObject(Of List(Of IndirizzoModel))(param.righeModificateGrid_Indirizzi, settingLoc))

                righeNonCancellate.AddRange(JsonConvert.DeserializeObject(Of List(Of IndirizzoModel))(param.righeNonCancellateGrid_Indirizzi, settingLoc))

                If ((From a In righeNonCancellate Select a.Tipo_Indirizzo).Distinct().Count <> righeNonCancellate.Count) Then
                    errori = DirectCast(HttpContext.GetLocalResourceObject("~/Anagrafica/New_Contatto_Edit.aspx", "MassimoUnIndirizzoPerTipologia"), String)
                    Return False
                End If


                For Each row As IndirizzoModel In righeNonCancellate
                    If (row.Tipo_Indirizzo = "0" OrElse IsNothing(row.Tipo_Indirizzo)) Then
                        errori = DirectCast(HttpContext.GetLocalResourceObject("~/Anagrafica/New_Contatto_Edit.aspx", "SpecificareTipologiaPerOgniIndirizzo"), String)
                        Return False
                    End If
                Next
                Return True
            Else
                Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
                Dim righeNonCancellate As New List(Of IndirizzoModel)

                If Not IsNothing(param.righeNonCancellateGrid_Indirizzi) AndAlso Not String.IsNullOrEmpty(param.righeNonCancellateGrid_Indirizzi) Then
                    righeNonCancellate.AddRange(JsonConvert.DeserializeObject(Of List(Of IndirizzoModel))(param.righeNonCancellateGrid_Indirizzi, settingLoc))

                    For Each row As IndirizzoModel In righeNonCancellate
                        If (row.Tipo_Indirizzo = "0" OrElse IsNothing(row.Tipo_Indirizzo)) Then
                            errori = DirectCast(HttpContext.GetLocalResourceObject("~/Anagrafica/New_Contatto_Edit.aspx", "SpecificareTipologiaPerOgniIndirizzo"), String)
                            Return False
                        End If
                    Next
                    Return True
                End If
            End If
        Else
            If (Not IsNothing(param.righeNonCancellateGrid_Indirizzi) AndAlso param.righeNonCancellateGrid_Indirizzi <> "[]" AndAlso param.righeNonCancellateGrid_Indirizzi <> "") Then

                Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

                Dim righeNonCancellate As List(Of IndirizzoModel) = New List(Of IndirizzoModel)

                If Not IsNothing(param.righeNonCancellateGrid_Indirizzi) AndAlso Not String.IsNullOrEmpty(param.righeNonCancellateGrid_Indirizzi) Then
                    righeNonCancellate.AddRange(JsonConvert.DeserializeObject(Of List(Of IndirizzoModel))(param.righeNonCancellateGrid_Indirizzi, settingLoc))
                End If

                If ((From a In righeNonCancellate Select a.Tipo_Indirizzo).Distinct().Count <> righeNonCancellate.Count) Then
                    errori = DirectCast(HttpContext.GetLocalResourceObject("~/Anagrafica/New_Contatto_Edit.aspx", "MassimoUnIndirizzoPerTipologia"), String)
                    Return False
                End If

                For Each row As IndirizzoModel In righeNonCancellate
                    If (row.Tipo_Indirizzo = "0" OrElse IsNothing(row.Tipo_Indirizzo)) Then
                        errori = DirectCast(HttpContext.GetLocalResourceObject("~/Anagrafica/New_Contatto_Edit.aspx", "SpecificareTipologiaPerOgniIndirizzo"), String)
                        Return False
                    End If
                Next

                ' Se contatto estero controlla tipo indirizzo 101, Sede Legale in Italia
                If (righeNonCancellate.Count() <> 0) Then
                    Dim indirizzi As New List(Of Object)


                    For Each row As IndirizzoModel In righeNonCancellate

                        indirizzi.Add(New With
                              {
                                    .Tipo_Indirizzo = CInt(row.Tipo_Indirizzo),
                                    .Stato = row.Stato
                              })
                        If (row.Tipo_Indirizzo = "0" OrElse IsNothing(row.Tipo_Indirizzo)) Then
                            errori = DirectCast(HttpContext.GetLocalResourceObject("~/Anagrafica/New_Contatto_Edit.aspx", "SpecificareTipologiaPerOgniIndirizzo"), String)
                            Return False
                        End If

                        Select Case (row.Tipo_Indirizzo)
                            Case 1 : If (row.Stato.ToString().ToUpper().Equals("ITALIA") OrElse row.Stato.ToString().ToUpper().Equals("IT")) Then
                                    errori = DirectCast(HttpContext.GetLocalResourceObject("~/Anagrafica/New_Contatto_Edit.aspx", "ErroreStatoSedeOperativa"), String)
                                    retVal = False
                                End If
                            Case 102 : If (row.Stato.ToString().ToUpper().Equals("ITALIA") OrElse row.Stato.ToString().ToUpper().Equals("IT")) Then
                                    errori = DirectCast(HttpContext.GetLocalResourceObject("~/Anagrafica/New_Contatto_Edit.aspx", "ErroreStatoSedeAziendale"), String)
                                    retVal = False
                                End If
                            Case 103 : If (row.Stato.ToString().ToUpper().Equals("ITALIA") OrElse row.Stato.ToString().ToUpper().Equals("IT")) Then
                                    errori = DirectCast(HttpContext.GetLocalResourceObject("~/Anagrafica/New_Contatto_Edit.aspx", "ErroreSatoStabilimento"), String)
                                    retVal = False
                                End If
                            Case 201 : If (Not row.Stato.ToString().ToUpper().Equals("ITALIA") AndAlso Not row.Stato.ToString().ToUpper().Equals("IT")) Then
                                    errori = DirectCast(HttpContext.GetLocalResourceObject("~/Anagrafica/New_Contatto_Edit.aspx", "ErroreStatoStabileOrganizzazione"), String)
                                    retVal = False
                                End If
                        End Select
                        If errori <> "" Then
                            Dim objErrore As New JObject()
                            'objErrore("Tipo") = row.Tipo_Indirizzo
                            objErrore("Msg") = errori
                            arrErrori.Add(objErrore)
                        End If
                        errori = ""
                    Next

                    Dim so = indirizzi.FirstOrDefault(Function(s) s.Tipo_Indirizzo = 101)

                    If Not IsNothing(so) Then

                        If String.IsNullOrEmpty(so.Stato) Then
                            errori = DirectCast(System.Web.HttpContext.GetLocalResourceObject(
                        "~/Anagrafica/New_Contatto_Edit.aspx", "ContattoEsteroNecessitaStatoInIndirizzoPrincipale"), String)
                            retVal = False
                        Else
                            If so.Stato.ToString().ToUpper().Equals("ITALIA") OrElse so.Stato.ToString().ToUpper().Equals("IT") Then
                                errori = DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/New_Contatto_Edit.aspx", "StatoIndirizzoPrincipaleNonPuòEssereItalia"), String)
                                retVal = False
                            End If
                        End If

                        If errori <> "" Then
                            Dim objErrore As New JObject()
                            'objErrore("Tipo") = so.Tipo_Indirizzo
                            objErrore("Msg") = errori
                            arrErrori.Add(objErrore)
                        End If

                    Else
                        errori = DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/New_Contatto_Edit.aspx", "ContattoEsteroNecessitaIndirizzoPrincipaleValido"), String)

                        If errori <> "" Then
                            Dim objErrore As New JObject()
                            'objErrore("Tipo") = so.Tipo_Indirizzo
                            objErrore("Msg") = errori
                            arrErrori.Add(objErrore)
                        End If

                        retVal = False

                    End If

                End If


                If (arrErrori.Count <> 0) Then
                    Dim messaggi As New List(Of String)

                    For Each o As JObject In arrErrori
                        messaggi.Add(o.Item("Msg").ToString)
                    Next
                    errori = String.Join("<br>", messaggi)
                    Return retVal
                End If

                Return True

            End If

            Return True

        End If

    End Function
    Private Shared Function Controlla_RapportiCOntabili(
                            ByVal piva As String,
                            ByVal codContatto As String,
                            ByVal param As ParametriSalvaTutto,
                            ByVal objParametriServer As AgronicaCoreParametri,
                            ByRef errori As String) As Boolean

        errori = ""

        Dim opzioniContatti = JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(param.OpzioniContatti)

        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim inseritiModificati As New List(Of RapportoContabileModel)()
        Dim eliminati As New List(Of RapportoContabileModel)()
        Dim tutteLeRighe As New List(Of RapportoContabileModel)()
        Dim almenoUnoSbagliato As Boolean = False

        If Not String.IsNullOrEmpty(param.righeCancellateGrid_Rapporti_Contabili) Then
            eliminati.AddRange(JsonConvert.DeserializeObject(Of List(Of RapportoContabileModel))(param.righeCancellateGrid_Rapporti_Contabili, settingLoc))
        End If

        If Not String.IsNullOrEmpty(param.righeInseriteGrid_Rapporti_Contabili) Then
            inseritiModificati.AddRange(JsonConvert.DeserializeObject(Of List(Of RapportoContabileModel))(param.righeInseriteGrid_Rapporti_Contabili, settingLoc))
        End If
        If Not String.IsNullOrEmpty(param.righeModificateGrid_Rapporti_Contabili) Then
            inseritiModificati.AddRange(JsonConvert.DeserializeObject(Of List(Of RapportoContabileModel))(param.righeModificateGrid_Rapporti_Contabili, settingLoc))
        End If
        If Not String.IsNullOrEmpty(param.righeNonCancellateGrid_Rapporti_Contabili) Then
            tutteLeRighe.AddRange(JsonConvert.DeserializeObject(Of List(Of RapportoContabileModel))(param.righeNonCancellateGrid_Rapporti_Contabili, settingLoc))
        End If
        If Not inseritiModificati.Any() AndAlso param.righeTotaliGrid_Rapporti_Contabili = 0 Then
            errori = DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/New_Contatto_Edit.aspx", "AttenzioneImpossibileSalvareContattoSenzaRapportoContabile"), String)
            Return False
        Else
            For Each e As RapportoContabileModel In inseritiModificati
                If e.Cod_Rapporto Is Nothing OrElse e.Cod_Rapporto = 0 Then
                    almenoUnoSbagliato = True
                    errori = DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/New_Contatto_Edit.aspx", "AttenzioneUnoORapportiContabiliNonCompilatiCorrettamente"), String)
                    Exit For
                End If
            Next
        End If

        If almenoUnoSbagliato Then
            Return False
        End If

        If tutteLeRighe.Any Then

            ' ricavo rapporti contabili ragruppati per tipo rapporto
            Dim tipiRapporto = tutteLeRighe.Select(Function(s) s.Cod_Rapporto).Distinct()

            ' controllo che per lo stesso tipo rapporto le date non si incrocino
            For Each tr As Integer In tipiRapporto

                Dim righeRapporto = tutteLeRighe.Where(Function(s) s.Cod_Rapporto.Equals(tr))

                Dim periodo = New Period(
                    righeRapporto.FirstOrDefault().Validita_Inizio.Value,
                    righeRapporto.FirstOrDefault().Validita_Fine.Value
                )

                For i As Integer = 1 To righeRapporto.Count() - 1

                    Dim periodoConfronto = New Period(
                                righeRapporto(i).Validita_Inizio.Value,
                                righeRapporto(i).Validita_Fine.Value
                                )

                    If periodo.IntersectsWith(periodoConfronto) Then
                        almenoUnoSbagliato = True
                        errori = DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/New_Contatto_Edit.aspx", "DateDiValiditàEFineSiIntersecanoPerPiùRapportiContabili"), String)
                        Exit For
                    End If
                Next
                If almenoUnoSbagliato Then
                    Return False
                End If

            Next

            ' Controllo validita CodContatti
            Dim eseguiCheck As Boolean =
                opzioniContatti.ContainsKey(enum_Impostazioni_Utenti.SUPERUSER_Consenti_ContattoCod_Duplicato.ToString()) AndAlso
                opzioniContatti(enum_Impostazioni_Utenti.SUPERUSER_Consenti_ContattoCod_Duplicato.ToString()).ToString() = "0"

            If eseguiCheck Then

                Dim dal = New Contatti_R()

                For Each rc As RapportoContabileModel In inseritiModificati

                    If Not String.IsNullOrEmpty(rc.Settore_Des) Then
                        Dim check = dal.Check_SettoreDes(piva, rc.Cod_Contatto, "", rc.Settore_Des, codContatto, objParametriServer)
                        If Not String.IsNullOrEmpty(check) Then
                            almenoUnoSbagliato = True
                            errori = String.Format(DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/New_Contatto_Edit.aspx", "CodiceContattoGiàAssegnatoASettore"), String), rc.Settore_Des, check)
                            Exit For
                        End If
                    End If

                Next
            End If

        End If

        If almenoUnoSbagliato Then
            Return False
        End If

        Dim dtMov As DataTable
        Dim objContatto_R As New AgronicaCoreAnagrafeBIZ.Contatti_MultiHost_R

        'Controllo se sono state fatte modifiche sul tipo di rapporto
        For Each rc As RapportoContabileModel In inseritiModificati

            Dim cod_risum = If(rc.Cod_RisUm Is Nothing, 0, rc.Cod_RisUm)
            If cod_risum <> 0 Then
                Dim cod_raporto As Integer = rc.Cod_Rapporto
                Dim cod_rapporto_ori As Integer = rc.Cod_Rapporto_Origine
                Dim rapporto_des_ori As String = rc.Rapporto_Des_Origine
                Dim rapporto_des As String = rc.Rapporto_Des

                If cod_raporto <> cod_rapporto_ori Then
                    dtMov = objContatto_R.Contatto_ControllaMovimentiXRisorsaUmana(piva, codContatto, cod_risum, "", objParametriServer)
                    If dtMov.Rows.Count <> 0 Then
                        almenoUnoSbagliato = True
                        errori = String.Format(
                            DirectCast(System.Web.HttpContext.GetLocalResourceObject(
                                "~/Anagrafica/New_Contatto_Edit.aspx", "ImpossibileTrasformareIlTipoDiUnRapportoContabileConMovimentiAssociati"),
                                String
                            ),
                        rapporto_des_ori, rapporto_des)
                        Exit For
                    Else
                        ' TODO Check Aggiunti 03/12/2019 GIANLUCA
                        Dim objContatti As New AgronicaCoreAnagrafeBIZ.Contatti_MultiHost_R
                        Dim dtAltriRiferimenti = objContatti.ContattoRiferimenti(piva, codContatto, cod_risum, param.Visibilita, "", objParametriServer)

                        If dtAltriRiferimenti.Rows.Count > 0 Then
                            almenoUnoSbagliato = True
                            errori = String.Format(
                            DirectCast(System.Web.HttpContext.GetLocalResourceObject(
                                "~/Anagrafica/New_Contatto_Edit.aspx", "ImpossibileTrasformareIlTipoDiUnRapportoContabileConMovimentiAssociati"),
                                String
                            ),
                        rapporto_des_ori, rapporto_des)
                            Exit For
                        End If

                    End If

                End If

            End If
        Next

        If almenoUnoSbagliato Then
            Return False
        End If


        '----------------------------
        ' SQUADRE CDG
        '----------------------------
        Dim objSquadre As New AgronicaCoreContabDAL.CDG_DAL_R
        Dim DtSquadre As DataTable

        Dim dic_listaSquadre_x_CodRapporto As New Dictionary(Of String, List(Of String))

        For Each del In eliminati.OrderBy(Function(x) x.Cod_Rapporto).ToList()
            Dim listaSquadre As New List(Of String)
            DtSquadre = objSquadre.Leggi_SquadrexAttvita("", 0, 0, 0, 0, " SquadrexAttivita.cod_risum_list LIKE '%" & del.Cod_RisUm & "%'", AGRODATAINIZIO, objParametriServer)

            For Each squadra In DtSquadre.Rows
                almenoUnoSbagliato = True

                Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
                Dim impresa_rif As String = objImprese.RagSoc_from_Piva(squadra.item("piva"), objParametriServer)

                listaSquadre.Add("- " & impresa_rif & " - " & squadra.item("des_squadra"))
            Next

            If listaSquadre.Count > 0 Then
                If dic_listaSquadre_x_CodRapporto.ContainsKey(del.Rapporto_Des) Then
                    dic_listaSquadre_x_CodRapporto(del.Rapporto_Des).AddRange(listaSquadre)
                Else
                    dic_listaSquadre_x_CodRapporto(del.Rapporto_Des) = listaSquadre
                End If
            End If
        Next

        If almenoUnoSbagliato Then
            For Each dic In dic_listaSquadre_x_CodRapporto
                errori &= String.Format(Gias.ImpossibileEliminareRapportoContabileXAssegnatoSquadre, dic.Key) & ":" & NEWLINE & String.Join(NEWLINE, dic.Value) & NEWLINE
            Next

            Return False
        End If

        Return True

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function ControllaContattoRiferimenti(
                            ByVal piva As String,
                            ByVal codContatto As String,
                            ByVal saCod As Integer) As RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim r As New RispostaStandard

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim objContatti As New AgronicaCoreAnagrafeBIZ.Contatti_MultiHost_R
            Dim dtAltriRiferimenti = objContatti.ContattoRiferimenti(piva, codContatto, 0, saCod, "", objParametri_Server)
            If dtAltriRiferimenti.Rows.Count > 0 Then

                Dim listaImprese = (From imp In dtAltriRiferimenti.AsEnumerable
                                    Select imp.Item("rag_soc")).ToList()


                r.Errore = String.Format(AgronicaAgenda_2010.MenuBS_Anagrafica_Cambio_Visibilita_CancellareRegistrazioni,
                                    "<br />", String.Join("<br />", listaImprese))

            End If
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.RispostaStringa = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function ControllaEsistenzaSquadreCdG(ByVal piva As String,
                                                        ByVal codContatto As String,
                                                        ByVal codRisUm As Integer) As RispostaStandard

        Dim objSquadre As New AgronicaCoreContabDAL.CDG_DAL_R
        Dim DtSquadre As DataTable

        Dim esistenzaSquadre As Boolean = False
        Dim listaSquadre As New List(Of String)

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim r As New RispostaStandard

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            If codRisUm <> 0 Then

                DtSquadre = objSquadre.Leggi_SquadrexAttvita("", 0, 0, 0, 0, " SquadrexAttivita.cod_risum_list LIKE '%" & codRisUm & "%'", AGRODATAINIZIO, objParametri_Server)

                For Each squadra In DtSquadre.Rows
                    esistenzaSquadre = True

                    Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
                    Dim impresa_rif As String = objImprese.RagSoc_from_Piva(squadra.item("piva"), objParametri_Server)

                    listaSquadre.Add("- " & impresa_rif & " - " & squadra.item("des_squadra"))
                Next

                If esistenzaSquadre Then
                    r.RispostaOK = False
                    r.Errore = Gias.ImpossibileEliminareRapportoContabileAssegnatoSquadre & ":" & NEWLINE & String.Join(NEWLINE, listaSquadre) & NEWLINE
                End If
            Else
                r.RispostaOK = True
            End If

        Catch ex As Exception
            r.RispostaStringa = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function ControllaMovimentiContabili(
                            ByVal piva As String,
                            ByVal codContatto As String,
                            ByVal codRisUm As Integer,
                            ByVal sa_Cod As Integer) As RispostaStandard

        Dim dtMov As DataTable
        Dim objContatto_R As New AgronicaCoreAnagrafeBIZ.Contatti_MultiHost_R
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim r As New RispostaStandard

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            If codRisUm <> 0 Then

                dtMov = objContatto_R.Contatto_ControllaMovimentiXRisorsaUmana(piva, codContatto, codRisUm, "", objParametri_Server)
                If dtMov.Rows.Count <> 0 Then
                    r.RispostaOK = False
                    r.Errore = DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/New_Contatto_Edit.aspx", "EsistonoMovimentiContabiliPerIlContattoImpossibileEliminareRapportoContabile"), String)
                Else

                    ' TODO Check Aggiunti 03/12/2019 GIANLUCA
                    Dim objContatti As New AgronicaCoreAnagrafeBIZ.Contatti_MultiHost_R
                    Dim dtAltriRiferimenti = objContatti.ContattoRiferimenti(piva, codContatto, codRisUm, sa_Cod, "", objParametri_Server)

                    If dtAltriRiferimenti.Rows.Count > 0 Then
                        Dim listaImprese = (From imp In dtAltriRiferimenti.AsEnumerable
                                            Select imp.Item("rag_soc")).ToList()

                        r.RispostaOK = False
                        r.Errore = String.Format(DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/New_Contatto_Edit.aspx", "RapportoContabileUtilizzatoNelleSeguentiImpreseCancellarneRegistrazioniPerEliminarlo"), String),
                                            vbCrLf, String.Join(vbCrLf, listaImprese))

                    Else
                        r.RispostaOK = True
                    End If

                End If
            Else
                r.RispostaOK = True
            End If


        Catch ex As Exception
            r.RispostaStringa = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try


        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function ControllaLiquidita(ByVal piva As String,
                                              ByVal codContatto As String,
                                              ByVal codLiquidita As Integer
                                              ) As RispostaStandard

        Dim objParametriAgenda As New ParametriAgenda
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim r As New RispostaStandard
        Dim dt As DataTable

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            If codLiquidita <> 0 Then
                Dim dal = New Pagamenti_R()
                dt = dal.Leggi(piva, objParametriAgenda.Sa_Cod, 0, 0, 0, codLiquidita, 0, False, False, False,
                               "", 0, "", codContatto, 0, objParametri_Server.FinestraTemporaleInizio,
                               objParametri_Server.FinestraTemporaleFine, 0, "", False, False, False,
                               enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

                If dt.Rows.Count <> 0 Then
                    r.RispostaOK = False
                    r.Errore = DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/New_Contatto_Edit.aspx", "EsistonoPagamentiAssociatiRisorsaFinanziariaCancellazioneNonConsentita"), String)
                Else
                    r.RispostaOK = True
                End If
            Else
                r.RispostaOK = True
            End If

        Catch ex As Exception
            r.RispostaStringa = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
            Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CheckPreSalva(ByVal piva As String, ByVal param As String) As RispostaStandard

        Dim parametri As ParametriSalvaTutto = JsonConvert.DeserializeObject(Of ParametriSalvaTutto)(param)

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim tipoOperazioneDB_Contatto As Integer = parametri.TipoOperazione
        Dim codContatto As String = ""
        Dim res As String = String.Empty

        Try


            If parametri.TipoUtente = "0" Then

                ' PERSONA FISICA

                codContatto = parametri.CodiceFiscale.Trim()
                If codContatto = "" OrElse String.IsNullOrEmpty(parametri.Nome) OrElse String.IsNullOrEmpty(parametri.Cognome) Then
                    r.RispostaStringa = AgronicaAgenda_2010.IndicareCodiceFiscaleNomeECognome
                    r.RispostaOK = False
                    Return r
                End If

                ' Se contatto non fittizio controlli sulla PIVA
                If Not parametri.Fittizio Then
                    If codContatto.Length = 11 AndAlso (codContatto.Chars(0) = "F" OrElse codContatto.Chars(0) = "-") AndAlso IsNumeric(codContatto.Substring(1)) Then
                        'contatto generato automaticamente
                    Else
                        If codContatto.Length <> 16 Then
                            r.RispostaStringa = AgronicaAgenda_2010.CodiceFiscaleNonCorrettoInserireUnValoreOGenerarneUno
                            r.RispostaOK = False
                            Return r
                        End If
                    End If
                End If

            Else

                ' PERSONA GIURIDICA

                codContatto = parametri.PIVA.Trim()
                If codContatto = "" OrElse String.IsNullOrEmpty(parametri.RagioneSociale) Then
                    r.RispostaStringa = AgronicaAgenda_2010.IndicarePartitaIvaERagioneSociale
                    r.RispostaOK = False
                    Return r
                End If

                ' Se fittizio salto controlli sulla PIVA
                If Not parametri.Fittizio Then


                    If Not parametri.ItaEste = 2 Then

                        ' italiano non fittizio persona giuridica PIVA 11 Char

                        If codContatto.Length = 11 AndAlso (codContatto.Chars(0) = "F" OrElse codContatto.Chars(0) = "-") AndAlso IsNumeric(codContatto.Substring(1)) Then
                            'contatto generato automaticamente
                        Else
                            If codContatto.Length <> 11 OrElse Not IsNumeric(codContatto) Then
                                r.RispostaStringa = AgronicaAgenda_2010.PartitaIvaNonCorrettaInserireValoreOGenerarneUna
                                r.RispostaOK = False
                                Return r
                            End If
                        End If

                    Else
                        'estero non fittizio persona giuridica PIVA 25 Char
                        If codContatto.Length = 11 AndAlso (codContatto.Chars(0) = "F" OrElse codContatto.Chars(0) = "-") AndAlso IsNumeric(codContatto.Substring(1)) Then
                            'contatto generato automaticamente
                        Else
                            If codContatto.Length > 25 Then
                                r.RispostaStringa = "La partita iva non è corretta. Inserire una partita iva lunga al massimo 25 caratteri o generarne una con il pulsante apposito."
                                r.RispostaOK = False
                                Return r
                            End If
                        End If
                    End If
                End If


            End If

            If tipoOperazioneDB_Contatto = enum_TipoOperazioneDB.Scrittura Then

                Dim objCont As New AgronicaCoreAnagrafeDAL.Contatti_R
                Dim dtConta As DataTable = objCont.ControllaSePresente(piva, codContatto, objParametri_Server)
                If dtConta.Rows.Count > 0 Then
                    r.RispostaStringa = AgronicaAgenda_2010.ContattoGiàPresente
                    r.RispostaOK = False
                    Return r
                End If
            Else

            End If

            ' Se non è una impresa gias controllo gli indirizzi
            If Not parametri.ImpresaGias Then
                'If Not Controlla_Indirizzi(parametri, res) Then
                '    r.RispostaStringa = res
                '    r.RispostaOK = False
                '    Return r
                'End If
                If Not Controlla_IndirizziKendo(parametri, res) Then
                    r.RispostaStringa = res
                    r.RispostaOK = False
                    Return r
                End If
            End If

            If Not Controlla_RapportiCOntabili(piva, codContatto, parametri, objParametri_Server, res) Then
                r.RispostaStringa = res
                r.RispostaOK = False
                Return r
            End If

            If Not Controlla_Costi(piva, codContatto, parametri, res) Then
                r.RispostaStringa = res
                r.RispostaOK = False
                Return r
            End If

            If Not Controlla_Efattura(piva, codContatto, parametri, res) Then
                r.RispostaStringa = res
                r.RispostaOK = False
                Return r
            End If

            If Not Controlla_Altri_Dati(piva, codContatto, parametri, res) Then
                r.RispostaStringa = res
                r.RispostaOK = False
                Return r
            End If

            If parametri.GestisciContabilita Then

                If Not Controlla_Liquidita(piva, codContatto, parametri, res) Then
                    r.RispostaStringa = res
                    r.RispostaOK = False
                    Return r
                End If

                If Not Controlla_Conti(piva, codContatto, parametri, res) Then
                    r.RispostaStringa = res
                    r.RispostaOK = False
                    Return r
                End If

            End If



            r.RispostaStringa = ""
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaStringa = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
            Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function SalvaIndirizziTipo(ByVal piva As String, ByVal param As String) As RispostaStandard

        Dim parametri As ParametriSalvaIndirizziTipoModel = JsonConvert.DeserializeObject(Of ParametriSalvaIndirizziTipoModel)(param)
        Dim errori As String = String.Empty

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim Provider = Globalization.CultureInfo.InvariantCulture
        Dim Format As String = "yyyyMMdd"

        Dim checkOk As Boolean = Controlla_IndirizziTipo(piva, param, errori)

        If Not checkOk Then
            r.Errore = errori
            Return r
        Else

            Try

                Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
                Dim inseritiModificati As List(Of InidirizzoTipoModel) = New List(Of InidirizzoTipoModel)
                Dim eliminati As List(Of InidirizzoTipoModel) = New List(Of InidirizzoTipoModel)

                If Not String.IsNullOrEmpty(parametri.righeInserite) Then
                    inseritiModificati.AddRange(JsonConvert.DeserializeObject(Of List(Of InidirizzoTipoModel))(parametri.righeInserite, settingLoc))
                End If
                If Not String.IsNullOrEmpty(parametri.righeModificate) Then
                    inseritiModificati.AddRange(JsonConvert.DeserializeObject(Of List(Of InidirizzoTipoModel))(parametri.righeModificate, settingLoc))
                End If
                If Not String.IsNullOrEmpty(parametri.righeCancellate) Then
                    eliminati.AddRange(JsonConvert.DeserializeObject(Of List(Of InidirizzoTipoModel))(parametri.righeCancellate, settingLoc))
                End If

                Dim dal = New IndirizzoTipo_W

                ' Cancellazione dei record eliminati
                For Each it As InidirizzoTipoModel In eliminati
                    Dim codContatto As String = If(it.Cod_Contatto Is Nothing, "", it.Cod_Contatto)
                    dal.Cancella(piva, it.InidrizzoTipoCod, codContatto, "", objParametri_Server)
                Next

                For Each it As InidirizzoTipoModel In inseritiModificati

                    Dim codContatto As String = If(it.Cod_Contatto Is Nothing, "", it.Cod_Contatto)

                    If it.InidrizzoTipoCod Is Nothing OrElse it.InidrizzoTipoCod = 0 Then
                        ' inserimento
                        dal.Scrivi(piva, codContatto, it.IndirizzoTipoDes,
                                    it.ApplicabilitaCod, AGRODATAINIZIO, AGRODATAFINE, objParametri_Server)
                    Else
                        ' Modifica
                        dal.Modifica(piva,
                                     it.InidrizzoTipoCod,
                                     codContatto,
                                     it.IndirizzoTipoDes,
                                     it.ApplicabilitaCod,
                                     AGRODATAINIZIO,
                                     AGRODATAFINE, "", objParametri_Server)
                    End If

                Next

                r.RispostaOK = True

            Catch ex As Exception
                errori = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
                r.Errore = errori
            End Try

        End If

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function SalvaTutto(
            ByVal piva As String,
            ByVal param As String
        ) As RispostaStandard

        Dim parametri As ParametriSalvaTutto = JsonConvert.DeserializeObject(Of ParametriSalvaTutto)(param)

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        If parametri.PIVA = piva Then
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.PivaUgualeAQuellaAziendale
            r.RispostaStringa = AgronicaAgenda_2010.PivaUgualeAQuellaAziendale
            Return r
        End If

        Dim Provider = Globalization.CultureInfo.InvariantCulture
        Dim Format As String = "yyyyMMdd"

        Dim codContatto As String = parametri.CodContatto

        Dim objContatto_R As New AgronicaCoreAnagrafeBIZ.Contatti_MultiHost_R
        Dim objXML_Utility As New AgronicaCoreXML.XML_Utility

        Dim DT_RisUm As DataTable = objXML_Utility.CaricaGriglia_RisUm_for_XML()
        Dim Dt_Prodotti As DataTable = objXML_Utility.CaricaGriglia_ProdottiCosti_for_XML()
        Dim Dt_Indirizzi As DataTable = objXML_Utility.CaricaGriglia_Indirizzi_for_XML()
        Dim DT_Codici As DataTable = objXML_Utility.CaricaGriglia_CodiciContatto_for_XML()
        Dim DT_Liquidita As DataTable = objXML_Utility.CaricaGriglia_Liquidita_for_XML()
        Dim DT_Conti As DataTable = objXML_Utility.CaricaGriglia_Conti_for_XML()

        Dim lista As List(Of String) = HttpContext.Current.Session("lista_indirizzi")
        Dim Dt_Rubrica As DataTable = objXML_Utility.CaricaGriglia_Rubrica_for_XML()

        Dim Nome As String = parametri.Nome
        Dim Cognome As String = parametri.Cognome
        Dim Rag_Soc As String = parametri.RagioneSociale
        Dim COnvenevoli As String = parametri.Convenevoli
        Dim nrBadge As String = parametri.NumeroBadge
        Dim sesso As String = ""
        Dim id_cf As Integer
        Dim Cod_Contatto As String = ""
        Dim CodiceFiscaleEstero As String = If(String.IsNullOrEmpty(parametri.CodiceFiscaleEstero), "", parametri.CodiceFiscaleEstero)
        Dim Fittizio As Boolean = parametri.Fittizio
        Dim dataNascita As Date = AGRODATAINIZIO
        Dim streerore As String = ""
        Dim tipoOperazioneDB_Contatto As Integer = parametri.TipoOperazione
        Dim noteOperazioni As String = If(String.IsNullOrEmpty(parametri.AltriDati.NoteOperazioni), "", "|" & parametri.AltriDati.NoteOperazioni & "|")
        Dim noteOperazioni2 As String = If(String.IsNullOrEmpty(parametri.AltriDati.NoteOperazioni2), "", "|" & parametri.AltriDati.NoteOperazioni2 & "|")

        Dim tipoSpeditore As Integer = If(String.IsNullOrEmpty(parametri.AltriDati.OrigineDestinazione), 0, CInt(parametri.AltriDati.OrigineDestinazione))
        Dim tipoDestinazione As Integer = If(String.IsNullOrEmpty(parametri.AltriDati.TipoDestinazione), 0, CInt(parametri.AltriDati.TipoDestinazione))
        Dim destinazioneDiversaCod As Integer = If(String.IsNullOrEmpty(parametri.DettagliContabilita.DestinazioneDiversa_Cod), 0, CInt(parametri.DettagliContabilita.DestinazioneDiversa_Cod))
        Dim tipIndDestDiversa_Cod As Integer = If(String.IsNullOrEmpty(parametri.DettagliContabilita.IndirizzoDestinazioneDiversa_Cod), 0, CInt(parametri.DettagliContabilita.IndirizzoDestinazioneDiversa_Cod))
        Dim indirizzoFatturazione As Integer = If(String.IsNullOrEmpty(parametri.DettagliContabilita.IndirizzoFatturazione_Cod), 0, CInt(parametri.DettagliContabilita.IndirizzoFatturazione_Cod))
        Dim Cod_Fisc_giuridica As String
        Dim Sa_Cod As Integer = parametri.Visibilita
        Dim NomeBreve As String = ""
        Dim EUDR As Boolean = parametri.EUDR

        Try

            ' PERSONA FISICA / GIURIDICA
            If parametri.TipoUtente = "0" Then
                ' fISICA
                id_cf = PERSONA_FISICA
                Rag_Soc = ""
                sesso = parametri.Sesso
                Cod_Contatto = parametri.CodiceFiscale.Trim()
                If Not String.IsNullOrEmpty(parametri.DataNascita) AndAlso IsDate(parametri.DataNascita) Then
                    dataNascita = CDate(parametri.DataNascita)
                End If
                Cod_Fisc_giuridica = parametri.CodiceFiscale
                NomeBreve = parametri.Nome_Breve_Persona
            Else
                ' Giuridica
                id_cf = If(parametri.ItaEste = 0, PERSONA_GIURIDICA, CONTATTO_ESTERO)
                Nome = ""
                Cognome = ""
                sesso = ""
                Cod_Contatto = parametri.PIVA.Trim()
                Cod_Fisc_giuridica = parametri.CodiceFiscaleEstero
                NomeBreve = parametri.Nome_Breve_Azienda
            End If

            ' ***********************************************************************
            ' INDIRIZZI
            ' ***********************************************************************
            If Not parametri.ImpresaGias Then
                'PreparaSalvataggioIndirizzi(piva, codContatto, Dt_Indirizzi)
                PreparaSalvataggioIndirizziKendo(parametri.ItaEste,
                                                 id_cf,
                                                 piva, codContatto,
                                                 parametri.righeInseriteGrid_Indirizzi,
                                                 parametri.righeModificateGrid_Indirizzi,
                                                 parametri.righeCancellateGrid_Indirizzi,
                                                 parametri.righeNonCancellateGrid_Indirizzi,
                                                 Dt_Indirizzi)

            End If

            ' *************************************************************************
            ' RUBRICA
            ' *************************************************************************
            If Not parametri.ImpresaGias Then
                PreparaSalvataggioRubrica(parametri.righeInseriteGrid_Rubrica,
                                        parametri.righeModificateGrid_Rubrica,
                                        parametri.righeCancellateGrid_Rubrica,
                                        Dt_Rubrica)
            End If

            '**************************************************************************
            ' RAPPORTI CONTABILI
            '**************************************************************************
            PreparaSalvataggioRapportiContabili(objParametri_Server, piva, codContatto, Sa_Cod,
                                        parametri.righeInseriteGrid_Rapporti_Contabili,
                                        parametri.righeModificateGrid_Rapporti_Contabili,
                                        parametri.righeCancellateGrid_Rapporti_Contabili,
                                        DT_RisUm, Dt_Prodotti)

            '**************************************************************************
            ' COSTI
            '**************************************************************************
            PreparaSalvataggioCosti(piva, parametri.righeInseriteGrid_Costi,
                                    parametri.righeModificateGrid_Costi,
                                    parametri.righeCancellateGrid_Costi, Dt_Prodotti)

            PreparaSalvataggioProdottiCosti(piva, DT_RisUm, Dt_Prodotti)

            '**************************************************************************
            ' CODICI
            '**************************************************************************
            PreparaSalvataggioCodici(piva, codContatto, parametri, DT_Codici)

            If (parametri.GestisciContabilita) Then
                '**************************************************************************
                ' LIQUIDITA
                '**************************************************************************
                PreparaSalvataggioLiquidita(piva, If(codContatto = "", Cod_Contatto, codContatto), parametri, DT_Liquidita)

                '**************************************************************************
                ' LIQUIDITA
                '**************************************************************************
                PreparaSalvataggioConti(piva, codContatto, parametri, DT_Conti)

            End If


            '**************************************************************************
            ' DETTAGLI CONTABILI
            '**************************************************************************

            Dim arrayScontiAdd = New List(Of String)
            If Not String.IsNullOrEmpty(parametri.DettagliContabilita.Sconto_Add1) Then
                arrayScontiAdd.Add(parametri.DettagliContabilita.Sconto_Add1)
            End If
            If Not String.IsNullOrEmpty(parametri.DettagliContabilita.Sconto_Add2) Then
                arrayScontiAdd.Add(parametri.DettagliContabilita.Sconto_Add2)
            End If
            If Not String.IsNullOrEmpty(parametri.DettagliContabilita.Sconto_Add3) Then
                arrayScontiAdd.Add(parametri.DettagliContabilita.Sconto_Add3)
            End If
            Dim ScontoTesto As String = If(arrayScontiAdd.Any(), String.Join("-", arrayScontiAdd), "")
            Dim Agente_Cod As Integer = If(String.IsNullOrEmpty(parametri.DettagliContabilita.Agente_Cod), 0, CInt(parametri.DettagliContabilita.Agente_Cod))
            Dim CapoArea_Cod As Integer = If(String.IsNullOrEmpty(parametri.DettagliContabilita.CapoArea_Cod), 0, CInt(parametri.DettagliContabilita.CapoArea_Cod))
            Dim Vettore_Cod As Integer = If(String.IsNullOrEmpty(parametri.DettagliContabilita.Vettore_Cod), 0, CInt(parametri.DettagliContabilita.Vettore_Cod))
            Dim Modalita_Fatturazione As Integer = If(String.IsNullOrEmpty(parametri.DettagliContabilita.Fatturazione_Automatica), 0, CInt(parametri.DettagliContabilita.Fatturazione_Automatica))
            Dim Cod_Iva_Contatto As Integer = If(String.IsNullOrEmpty(parametri.DettagliContabilita.Iva_Default), 0, CInt(parametri.DettagliContabilita.Iva_Default))

            Dim Cod_Conto_Econ As Integer = If(String.IsNullOrEmpty(parametri.DettagliContabilita.Conto_Econ), 0, CInt(parametri.DettagliContabilita.Conto_Econ))
            Dim Cod_Conto_Pat As Integer = If(String.IsNullOrEmpty(parametri.DettagliContabilita.Conto_Pat), 0, CInt(parametri.DettagliContabilita.Conto_Pat))

            Dim Provvigione As Decimal = If(String.IsNullOrEmpty(parametri.DettagliContabilita.Provvigione_Agente), 0, CDbl(parametri.DettagliContabilita.Provvigione_Agente))
            Dim Provvigione_CapoArea As Decimal = If(String.IsNullOrEmpty(parametri.DettagliContabilita.Provvigione_Capo_area), 0, CDbl(parametri.DettagliContabilita.Provvigione_Capo_area))
            Dim TipoInidrizzoDefault As Integer = If(String.IsNullOrEmpty(parametri.DettagliContabilita.TipoIndirizzoDefault), 0, CInt(parametri.DettagliContabilita.TipoIndirizzoDefault))

            Dim objxml As New AgronicaCoreXML.XML_Anagrafe
            Dim objContattoW As New AgronicaCoreAnagrafeBIZ.Contatti_W
            Dim xmlDoc As New XmlDocument '= Nothing
            Dim xContatto As XmlElement
            Dim Base, Top As Integer

            UtilityProvider.Calcola_BaseCode_TopCode(Base, Top,
                                                     HttpContext.Current.Session("ASG_ProgressivoGIAS"))

            xContatto = objxml.XML_2_Contatti(streerore,
                            xmlDoc, Base, Top, False, tipoOperazioneDB_Contatto, piva, Cod_Contatto,
                            Dt_Indirizzi, DT_RisUm, parametri.Visibilita, id_cf, Rag_Soc, COnvenevoli,
                            Cod_Fisc_giuridica, indirizzoFatturazione, Nome, Cognome, dataNascita, sesso, , , ,
                            Dt_Rubrica, DT_Codici,
                            nrBadge:=parametri.NumeroBadge,
                            Tipo_Speditore:=tipoSpeditore,
                            Tipo_Destinazione:=tipoDestinazione,
                            Note:=parametri.AltriDati.Note,
                            Note2:=parametri.AltriDati.Note2,
                            Note_Operazioni:=noteOperazioni,
                            Note2_Operazioni:=noteOperazioni2,
                            Memo:=parametri.Memo,
                            DT_Liquidita:=DT_Liquidita,
                            Sconto_Testo:=ScontoTesto,
                            Agente_Cod:=Agente_Cod,
                            CapoArea_Cod:=CapoArea_Cod,
                            Vettore_Cod:=Vettore_Cod,
                            Modalita_Fatturazione:=Modalita_Fatturazione,
                            Cod_Iva_Contatto:=Cod_Iva_Contatto,
                            Cod_Conto_Economico_Default:=Cod_Conto_Econ,
                            Cod_Conto_Patrimoniale_Default:=Cod_Conto_Pat,
                            Provvigione:=Provvigione,
                            Provvigione_CapoArea:=Provvigione_CapoArea,
                            DT_Conti:=DT_Conti,
                            Cod_Risum_Destinazione_Diversa:=destinazioneDiversaCod,
                            Tipo_Indirizzo_Default_Destinazione_Diversa:=tipIndDestDiversa_Cod,
                            ChkFittizio:=Fittizio,
                            Nome_Breve:=NomeBreve,
                            EUDR:=EUDR
                            )

            Dim risp_boolean As Boolean
            Dim output_piva As String = ""
            Dim output_cod_contatto As String = ""

            ' CHiamata a funzione slava vera e propia
            streerore = ""
            risp_boolean = objContattoW.Contatto_Scrivi(xContatto.OuterXml, output_piva, output_cod_contatto, objParametri_Server, True, NoteXLog:=NOTELOG_ANAGRAFE_BOOTSTRAP)
            If risp_boolean Then
                ' TODO verificare tutte le session
                HttpContext.Current.Session("RapportiContabili_X_Contatto_Leggi") = Nothing
                r.RispostaOK = True
                r.RispostaStringa = output_cod_contatto
            Else
                r.RispostaStringa = streerore
            End If

        Catch ex As Exception
            streerore = ex.Message
            r.RispostaStringa = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    Private Shared Sub PreparaSalvataggioIndirizzi(ByVal piva As String, ByVal codContatto As String, ByRef Dt_Indirizzi As DataTable)

        Dim GridViewIndirizzi As DataTable
        Dim objXML_Utility As New AgronicaCoreXML.XML_Utility
        Dim tipoOperazioneDB_INDIRIZZI As enum_TipoOperazioneDB
        GridViewIndirizzi = HttpContext.Current.Session("Dt_Indirizzi")
        Dim lista As List(Of String) = HttpContext.Current.Session("lista_indirizzi")

        Dim pro_cod As String = "000"
        Dim com_cod As String = "000"

        For i = 0 To GridViewIndirizzi.Rows.Count - 1

            Dim Tipo_Indirizzo As String = GridViewIndirizzi.Rows(i).Item("Tipo_Indirizzo")
            Dim Cod_Indirizzo As String = GridViewIndirizzi.Rows(i).Item("Cod_Indirizzo")
            Dim Comune_Cod = GridViewIndirizzi.Rows(i).Item("Comune_cod")
            Dim Provincia_Cod = GridViewIndirizzi.Rows(i).Item("Provincia_cod")

            If Comune_Cod <> "0" AndAlso Not String.IsNullOrEmpty(Comune_Cod) Then
                com_cod = Comune_Cod
            End If
            If Provincia_Cod.ToString() <> "00" AndAlso Not String.IsNullOrEmpty(Provincia_Cod.ToString()) Then
                pro_cod = Provincia_Cod
            End If

            If Not Comune_Cod Is DBNull.Value AndAlso Not String.IsNullOrEmpty(Comune_Cod) AndAlso Comune_Cod.ToString().Length > 3 Then
                pro_cod = GridViewIndirizzi.Rows(i).Item("Provincia_cod").Substring(0, 3)
                com_cod = GridViewIndirizzi.Rows(i).Item("Comune_cod").Substring(3, 3)
            End If

            Dim Via As String = GridViewIndirizzi.Rows(i).Item("Via")
            Dim Frazione As String = GridViewIndirizzi.Rows(i).Item("Frazione")
            Dim Istat_Prov As String = pro_cod
            Dim Istat_Com As String = com_cod
            Dim Cap As String = GridViewIndirizzi.Rows(i).Item("Cap")
            Dim Stato As String = GridViewIndirizzi.Rows(i).Item("Stato")
            Dim Note As String = GridViewIndirizzi.Rows(i).Item("Note")
            Dim SiglaProvincia As String = GridViewIndirizzi.Rows(i).Item("Sigla_Prov")
            Dim CodiceLingua As String = GridViewIndirizzi.Rows(i).Item("Codice_Lingua")
            'Dim Citta_Des As String = GridViewIndirizzi.Rows(i).Item("Citta_Des")


            Select Case Cod_Indirizzo
                Case 0
                    tipoOperazioneDB_INDIRIZZI = enum_TipoOperazioneDB.Scrittura
                Case Else
                    tipoOperazioneDB_INDIRIZZI = enum_TipoOperazioneDB.Modifica
            End Select
            objXML_Utility.Inserisci_Riga_Dt_Indirizzi_for_XML(Dt_Indirizzi, tipoOperazioneDB_INDIRIZZI, piva,
                                                  codContatto,
                                                  Tipo_Indirizzo,
                                                   Istat_Prov,
                                                   Istat_Com,
                                                   Cod_Indirizzo,
                                                   Via,
                                                   Frazione,
                                                   Cap,
                                                   Stato,
                                                   Note,,,
                                                   SiglaProvincia,
                                                   CodiceLingua)

            For k = 0 To lista.Count - 1
                If lista(k) = Tipo_Indirizzo Then
                    lista.RemoveAt(k)
                    Exit For
                End If
            Next

        Next

        'aggiongo quelli rimasti e non associati
        For i = 0 To lista.Count - 1
            objXML_Utility.Inserisci_Riga_Dt_Indirizzi_for_XML(Dt_Indirizzi, enum_TipoOperazioneDB.Scrittura, piva, codContatto, lista(i), "000", "000")
        Next

    End Sub

    Private Shared Sub PreparaSalvataggioIndirizziKendo(ByVal itaEste As Integer,
                                                        ByVal id_cf As Integer,
                                                        ByVal piva As String,
                                                        ByVal codContatto As String,
                                                        ByVal righeInseriteGrid_Indirizzi As String,
                                                        ByVal righeModificateGrid_Indirizzi As String,
                                                        ByVal righeEliminateGrid_Indirizzi As String,
                                                        ByVal righeNonCancellateGrid_Indirizzi As String,
                                                        ByRef Dt_Indirizzi As DataTable)

        Dim objXML_Utility As New AgronicaCoreXML.XML_Utility
        Dim parametriAgenda = New ParametriAgenda

        Dim listaString As List(Of String) = HttpContext.Current.Session("lista_indirizzi")
        Dim lista As New List(Of Integer)
        Select Case id_cf
            Case PERSONA_FISICA
                lista = _listTipiIndirizziFis
            Case PERSONA_GIURIDICA
                lista = _listTipiIndirizziGiu
            Case CONTATTO_ESTERO
                lista = _listTipiIndirizziEst
        End Select


        Dim tipoOperazioneDB As Integer
        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim inseritiModificati As List(Of IndirizzoModel) = New List(Of IndirizzoModel)()
        Dim nonCancellati As List(Of IndirizzoModel) = New List(Of IndirizzoModel)()
        Dim eliminati As List(Of IndirizzoModel) = New List(Of IndirizzoModel)()

        If Not String.IsNullOrEmpty(righeModificateGrid_Indirizzi) Then
            inseritiModificati.AddRange(JsonConvert.DeserializeObject(Of List(Of IndirizzoModel))(righeModificateGrid_Indirizzi, settingLoc))
        End If
        If Not String.IsNullOrEmpty(righeInseriteGrid_Indirizzi) Then
            inseritiModificati.AddRange(JsonConvert.DeserializeObject(Of List(Of IndirizzoModel))(righeInseriteGrid_Indirizzi, settingLoc))
        End If
        If Not String.IsNullOrEmpty(righeNonCancellateGrid_Indirizzi) Then
            nonCancellati.AddRange(JsonConvert.DeserializeObject(Of List(Of IndirizzoModel))(righeNonCancellateGrid_Indirizzi, settingLoc))
        End If
        If Not String.IsNullOrEmpty(righeEliminateGrid_Indirizzi) Then
            eliminati.AddRange(JsonConvert.DeserializeObject(Of List(Of IndirizzoModel))(righeEliminateGrid_Indirizzi, settingLoc))
        End If

        If eliminati.Any() Then
            For Each l As IndirizzoModel In eliminati

                objXML_Utility.Inserisci_Riga_Dt_Indirizzi_for_XML(
                    Dt_Indirizzi,
                    enum_TipoOperazioneDB.Cancellazione, piva, codContatto,
                    l.Tipo_Indirizzo, l.Provincia_cod, l.Comune_cod,
                    l.Cod_Indirizzo, l.Via, l.Frazione, l.Cap, l.Stato,
                    l.Note, AGRODATAINIZIO, AGRODATAFINE, l.Sigla_Prov, l.Codice_Lingua)
            Next
        End If


        Dim pro_cod As String = "000"
        Dim com_cod As String = "000"

        For Each l As IndirizzoModel In nonCancellati

            If l.Cod_Indirizzo = 0 Then
                tipoOperazioneDB = enum_TipoOperazioneDB.Scrittura
            Else
                tipoOperazioneDB = enum_TipoOperazioneDB.Modifica
            End If

            If l.Cap.Length > 5 AndAlso l.Stato = "IT" Then
                Throw New Exception("Il CAP non può essere più lungo di 5")
            End If

            If (l.Gestione_Gerarchia_Geografica <> 1 AndAlso l.Stato <> "") Then
                l.Frazione = l.Citta_Des
            End If

            Dim Tipo_Indirizzo As String = If(l.Tipo_Indirizzo Is Nothing, 0, l.Tipo_Indirizzo)
            Dim Cod_Indirizzo As String = If(l.Cod_Indirizzo Is Nothing, 0, l.Cod_Indirizzo)
            Dim Comune_Cod As String = If(l.Comune_cod Is Nothing, "000", l.Comune_cod)
            Dim Provincia_Cod As String = If(l.Sigla_Prov Is Nothing, "00", l.Sigla_Prov)

            If Comune_Cod <> "0" AndAlso Not String.IsNullOrEmpty(Comune_Cod) Then
                com_cod = Comune_Cod
            End If
            If Provincia_Cod <> "00" AndAlso Not String.IsNullOrEmpty(Provincia_Cod) Then
                pro_cod = Provincia_Cod
            End If

            If Not Comune_Cod Is DBNull.Value AndAlso Not String.IsNullOrEmpty(Comune_Cod) AndAlso Comune_Cod.ToString().Length > 3 Then
                pro_cod = l.Sigla_Prov 'GridViewIndirizzi.Rows(i).Item("Comune_cod").Substring(0, 3)
                com_cod = l.Comune_cod 'GridViewIndirizzi.Rows(i).Item("Comune_cod").Substring(3, 3)
            End If

            Dim Via As String = If(l.Via Is Nothing, "Non Definita", l.Via)
            Dim Frazione As String = If(l.Frazione Is Nothing, "Non Definita", l.Frazione)
            Dim Istat_Prov As String = If(l.Provincia_cod Is Nothing, "000", l.Provincia_cod)
            Dim Istat_Com As String = If(l.Comune_cod Is Nothing, "000", l.Comune_cod)
            Dim Cap As String = If(l.Cap Is Nothing, "00000", l.Cap)
            Dim Stato As String = If(l.Stato Is Nothing, 0, l.Stato)
            Dim SiglaProvincia As String = If(l.Sigla_Prov Is Nothing, "00", l.Sigla_Prov)
            Dim Note As String = If(l.Note Is Nothing, "", l.Note)
            Dim CodiceLingua As String = If(l.Codice_Lingua Is Nothing, "00", l.Codice_Lingua)
            Dim Citta_Des As String = If(l.Citta_Des Is Nothing, "Non Definita", l.Citta_Des)
            Dim Tipo_Indirizzo_Desc As String = If(l.Tipo_Indirizzo_Desc Is Nothing, "", l.Tipo_Indirizzo_Desc)
            Dim Comune_Des As String = If(l.Comune_des Is Nothing, "Non Definito", l.Comune_des)


            Select Case Cod_Indirizzo
                Case 0
                    tipoOperazioneDB = enum_TipoOperazioneDB.Scrittura
                Case Else
                    tipoOperazioneDB = enum_TipoOperazioneDB.Modifica
            End Select

            objXML_Utility.Inserisci_Riga_Dt_Indirizzi_for_XML(Dt_Indirizzi, tipoOperazioneDB, piva,
                                                codContatto,
                                                Tipo_Indirizzo,
                                                Istat_Prov,
                                                Istat_Com,
                                                Cod_Indirizzo,
                                                Via,
                                                Frazione,
                                                Cap,
                                                Stato,
                                                Note,,,
                                                SiglaProvincia,
                                                CodiceLingua,
                                                Citta_Des,
                                                Comune_Des)

        Next


        For Each l As IndirizzoModel In nonCancellati

            Dim Tipo_Indirizzo As String = If(l.Tipo_Indirizzo Is Nothing, 0, l.Tipo_Indirizzo)

            For k = 0 To lista.Count - 1
                If lista(k) = Tipo_Indirizzo Then
                    lista.RemoveAt(k)
                    Exit For
                End If
            Next
        Next

        'aggiungo quelli rimasti e non associati
        For i = 0 To lista.Count - 1
            objXML_Utility.Inserisci_Riga_Dt_Indirizzi_for_XML(Dt_Indirizzi, enum_TipoOperazioneDB.Scrittura, piva, codContatto, lista(i), "000", "000", Stato:=If(itaEste = 0, "IT", ""))
        Next


    End Sub





    Private Shared Sub PreparaSalvataggioLiquidita(
        ByVal piva As String,
        ByVal codContatto As String,
        ByVal parametri As ParametriSalvaTutto,
        ByRef DT_Liquidita As DataTable
    )

        Dim objXML_Utility As New AgronicaCoreXML.XML_Utility
        Dim parametriAgenda = New ParametriAgenda

        Dim tipoOperazioneDB As Integer
        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim inseritiModificati As List(Of LiquiditaModel) = New List(Of LiquiditaModel)()
        Dim eliminati As List(Of LiquiditaModel) = New List(Of LiquiditaModel)()

        If Not String.IsNullOrEmpty(parametri.righeModificateGrid_Liquidita) Then
            inseritiModificati.AddRange(JsonConvert.DeserializeObject(Of List(Of LiquiditaModel))(parametri.righeModificateGrid_Liquidita, settingLoc))
        End If
        If Not String.IsNullOrEmpty(parametri.righeInseriteGrid_Liquidita) Then
            inseritiModificati.AddRange(JsonConvert.DeserializeObject(Of List(Of LiquiditaModel))(parametri.righeInseriteGrid_Liquidita, settingLoc))
        End If
        If Not String.IsNullOrEmpty(parametri.righeCancellateGrid_Liquidita) Then
            eliminati.AddRange(JsonConvert.DeserializeObject(Of List(Of LiquiditaModel))(parametri.righeCancellateGrid_Liquidita, settingLoc))
        End If

        If eliminati.Any() Then
            For Each l As LiquiditaModel In eliminati

                objXML_Utility.Inserisci_Riga_Dt_Liquidita_for_XML(
                    DT_Liquidita,
                    enum_TipoOperazioneDB.Cancellazione, piva, parametriAgenda.Sa_Cod, codContatto,
                    "", "", "", "", l.Cod_Liquidita, 0, "", "", "", "", "", "", "", 0, 0, Nothing, Nothing, "")
            Next
        End If

        If inseritiModificati.Any() Then
            For Each l As LiquiditaModel In inseritiModificati

                If l.Cod_Liquidita = 0 Then
                    tipoOperazioneDB = enum_TipoOperazioneDB.Scrittura
                Else
                    tipoOperazioneDB = enum_TipoOperazioneDB.Modifica
                End If

                Dim Sa_Cod = parametriAgenda.Sa_Cod
                Dim Cod_Istituto As Integer = If(l.Cod_Istituto Is Nothing, 0, l.Cod_Istituto)
                Dim Nazione As String = If(l.Nazione Is Nothing, "", l.Nazione)
                Dim Cifre_Controllo As String = If(l.Cifre_Controllo Is Nothing, "", l.Cifre_Controllo)
                Dim Cin As String = If(l.Cin Is Nothing, "", l.Cin)
                Dim Abi As String = If(l.Abi Is Nothing, "", l.Abi)
                Dim Cab As String = If(l.Cab Is Nothing, "", l.Cab)
                Dim Numero As String = If(l.Numero Is Nothing, "", l.Numero)
                Dim Bic As String = If(l.Bic Is Nothing, "", l.Bic)
                Dim ChkAbilitazione As Integer = If(l.ChkAbilitazione Is Nothing, 0, l.ChkAbilitazione)
                Dim Validita_Inizio As DateTime = If(l.Validita_Inizio Is Nothing, AGRODATAINIZIO, l.Validita_Inizio)
                Dim Validita_Fine As DateTime = If(l.Validita_Fine Is Nothing, AGRODATAFINE, l.Validita_Fine)
                Dim Note As String = If(l.Note Is Nothing, "", l.Note)

                Dim ChkDefault As Integer = 0
                If Not IsNothing(l.ChkDefault) Then

                    ChkDefault = Convert.ToInt32(l.ChkDefault)
                End If

                objXML_Utility.Inserisci_Riga_Dt_Liquidita_for_XML(
                    DT_Liquidita, tipoOperazioneDB, piva, Sa_Cod,
                    codContatto, codContatto, CStr(enum_Liquidita_CauRisorsa.RisorsaFinanziaria), "", "", l.Cod_Liquidita, Cod_Istituto, Nazione, Cifre_Controllo, Cin,
                    Abi, Cab, Numero, Bic, ChkAbilitazione, ChkDefault, Validita_Inizio, Validita_Fine, Note)

            Next

        End If

    End Sub

    Private Shared Sub PreparaSalvataggioConti(ByVal piva As String, ByVal codContatto As String, ByVal parametri As ParametriSalvaTutto,
        ByRef DT_Conti As DataTable)

        Dim objXML_Utility As New AgronicaCoreXML.XML_Utility
        Dim parametriAgenda = New ParametriAgenda

        Dim tipoOperazioneDB As Integer
        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim inseriti As List(Of ContiModel) = New List(Of ContiModel)()
        Dim modificati As List(Of ContiModel) = New List(Of ContiModel)()
        Dim eliminati As List(Of ContiModel) = New List(Of ContiModel)()

        If Not String.IsNullOrEmpty(parametri.righeModificateGrid_Conti) Then
            modificati.AddRange(JsonConvert.DeserializeObject(Of List(Of ContiModel))(parametri.righeModificateGrid_Conti, settingLoc))
        End If
        If Not String.IsNullOrEmpty(parametri.righeInseriteGrid_Conti) Then
            inseriti.AddRange(JsonConvert.DeserializeObject(Of List(Of ContiModel))(parametri.righeInseriteGrid_Conti, settingLoc))
        End If
        If Not String.IsNullOrEmpty(parametri.righeCancellateGrid_Conti) Then
            eliminati.AddRange(JsonConvert.DeserializeObject(Of List(Of ContiModel))(parametri.righeCancellateGrid_Conti, settingLoc))
        End If

        If eliminati.Any() Then
            For Each l As ContiModel In eliminati

                objXML_Utility.Inserisci_Riga_Dt_Conti_for_XML(
                    DT_Conti, enum_TipoOperazioneDB.Cancellazione, piva, l.Cod_Conto, codContatto, l.Validita_Inizio, l.Validita_Fine)
            Next
        End If

        If modificati.Any() Then
            For Each l As ContiModel In modificati
                Dim Cod_Conto As Integer = If(l.Cod_Conto Is Nothing, 0, l.Cod_Conto)
                Dim Validita_Inizio As DateTime = If(l.Validita_Inizio Is Nothing, AGRODATAINIZIO, l.Validita_Inizio)
                Dim Validita_Fine As DateTime = If(l.Validita_Fine Is Nothing, AGRODATAFINE, l.Validita_Fine)

                objXML_Utility.Inserisci_Riga_Dt_Conti_for_XML(
                    DT_Conti, enum_TipoOperazioneDB.Cancellazione, piva, Cod_Conto, codContatto, Validita_Inizio, Validita_Fine)

                objXML_Utility.Inserisci_Riga_Dt_Conti_for_XML(
                    DT_Conti, enum_TipoOperazioneDB.Scrittura, piva, Cod_Conto, codContatto, Validita_Inizio, Validita_Fine)
            Next

        End If

        If inseriti.Any() Then
            For Each l As ContiModel In inseriti

                tipoOperazioneDB = enum_TipoOperazioneDB.Scrittura
                Dim Cod_Conto As Integer = If(l.Cod_Conto Is Nothing, 0, l.Cod_Conto)
                Dim Validita_Inizio As DateTime = If(l.Validita_Inizio Is Nothing, AGRODATAINIZIO, l.Validita_Inizio)
                Dim Validita_Fine As DateTime = If(l.Validita_Fine Is Nothing, AGRODATAFINE, l.Validita_Fine)

                objXML_Utility.Inserisci_Riga_Dt_Conti_for_XML(
                    DT_Conti, tipoOperazioneDB, piva, Cod_Conto, codContatto, Validita_Inizio, Validita_Fine)

            Next

        End If



    End Sub

    Private Shared Sub PreparaSalvataggioCodici(
        ByVal piva As String, ByVal codContatto As String, ByVal parametri As ParametriSalvaTutto,
        ByRef DT_Codici As DataTable)

        Dim objXML_Utility As New AgronicaCoreXML.XML_Utility

        ' pec
        objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Cancellazione,
                                                                parametri.PIVA, codContatto, enum_CodiciAnagrafe.PecContatto,
                                                                parametri.Visibilita, parametri.PEC, AGRODATAINIZIO, AGRODATAFINE)
        objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Scrittura,
                                                                parametri.PIVA, codContatto, enum_CodiciAnagrafe.PecContatto,
                                                                parametri.Visibilita, parametri.PEC, AGRODATAINIZIO, AGRODATAFINE)

        ' codice sdi
        objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Cancellazione,
                                                                parametri.PIVA, codContatto, enum_CodiciAnagrafe.CodiceSDI,
                                                                parametri.Visibilita, parametri.CodiceSDI, AGRODATAINIZIO, AGRODATAFINE)

        objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Scrittura,
                                                                parametri.PIVA, codContatto, enum_CodiciAnagrafe.CodiceSDI,
                                                                parametri.Visibilita, parametri.CodiceSDI, AGRODATAINIZIO, AGRODATAFINE)

        ' tipo Contatto
        objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Cancellazione,
                                                               parametri.PIVA, codContatto, enum_CodiciAnagrafe.TipoContattoFattura,
                                                               parametri.Visibilita, parametri.TipologiaContatto, AGRODATAINIZIO, AGRODATAFINE)
        objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Scrittura,
                                                                parametri.PIVA, codContatto, enum_CodiciAnagrafe.TipoContattoFattura,
                                                                parametri.Visibilita, parametri.TipologiaContatto, AGRODATAINIZIO, AGRODATAFINE)

        ' rap fiscale
        objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Cancellazione,
                                                                      parametri.PIVA, codContatto, enum_CodiciAnagrafe.RappresentanteFiscale,
                                                                      parametri.Visibilita, parametri.RappFiscale, AGRODATAINIZIO, AGRODATAFINE)
        objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Scrittura,
                                                                parametri.PIVA, codContatto, enum_CodiciAnagrafe.RappresentanteFiscale,
                                                                parametri.Visibilita, parametri.RappFiscale, AGRODATAINIZIO, AGRODATAFINE)

        ' dichiarazione intenti protocollo
        objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Cancellazione,
                                                               parametri.PIVA, codContatto, enum_CodiciAnagrafe.DichiarazioneIntentoNumeroProtocollo,
                                                               parametri.Visibilita, parametri.DichIntentiProtocollo, AGRODATAINIZIO, AGRODATAFINE)
        objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Scrittura,
                                                                parametri.PIVA, codContatto, enum_CodiciAnagrafe.DichiarazioneIntentoNumeroProtocollo,
                                                                parametri.Visibilita, parametri.DichIntentiProtocollo, AGRODATAINIZIO, AGRODATAFINE)

        ' dichiarazione intenti data protocollo
        objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Cancellazione,
                                                               parametri.PIVA, codContatto, enum_CodiciAnagrafe.DichiarazioneIntentoDataRicezione,
                                                               parametri.Visibilita, parametri.DichIntentiData, AGRODATAINIZIO, AGRODATAFINE)
        objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Scrittura,
                                                                parametri.PIVA, codContatto, enum_CodiciAnagrafe.DichiarazioneIntentoDataRicezione,
                                                                parametri.Visibilita, parametri.DichIntentiData, AGRODATAINIZIO, AGRODATAFINE)


        ' Codice accisa
        objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Cancellazione,
                                                                      parametri.PIVA, codContatto, enum_CodiciAnagrafe.CodiceAccisa,
                                                                      parametri.Visibilita, parametri.AltriDati.CodiceAccisa, AGRODATAINIZIO, AGRODATAFINE)
        objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Scrittura,
                                                                parametri.PIVA, codContatto, enum_CodiciAnagrafe.CodiceAccisa,
                                                                parametri.Visibilita, parametri.AltriDati.CodiceAccisa, AGRODATAINIZIO, AGRODATAFINE)

        ' Codice Ufficio Dogane
        objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Cancellazione,
                                                                      parametri.PIVA, codContatto, enum_CodiciAnagrafe.Codice_Ufficio_Doganale,
                                                                      parametri.Visibilita, parametri.AltriDati.UfficioDogane, AGRODATAINIZIO, AGRODATAFINE)
        objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Scrittura,
                                                                parametri.PIVA, codContatto, enum_CodiciAnagrafe.Codice_Ufficio_Doganale,
                                                                parametri.Visibilita, parametri.AltriDati.UfficioDogane, AGRODATAINIZIO, AGRODATAFINE)


        If parametri.GestisciContabilita Then


            ' Sconto cliente
            objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Cancellazione,
                                                                      parametri.PIVA, codContatto, enum_CodiciAnagrafe.ScontoContattoDefault,
                                                                      parametri.Visibilita,
                                                                      If(parametri.DettagliContabilita.Sconto_Cliente Is Nothing, 0, parametri.DettagliContabilita.Sconto_Cliente),
                                                                      AGRODATAINIZIO, AGRODATAFINE)
            objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Scrittura,
                                                                    parametri.PIVA, codContatto, enum_CodiciAnagrafe.ScontoContattoDefault,
                                                                    parametri.Visibilita,
                                                                    If(parametri.DettagliContabilita.Sconto_Cliente Is Nothing, 0, parametri.DettagliContabilita.Sconto_Cliente),
                                                                    AGRODATAINIZIO, AGRODATAFINE)

            ' modalita pagamento default
            objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Cancellazione,
                                                                          parametri.PIVA, codContatto, enum_CodiciAnagrafe.ModalitaPagamentoDefault,
                                                                          parametri.Visibilita,
                                                                          If(parametri.DettagliContabilita.Mod_Pag_Default Is Nothing, 0, parametri.DettagliContabilita.Mod_Pag_Default),
                                                                          AGRODATAINIZIO, AGRODATAFINE)
            objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Scrittura,
                                                                    parametri.PIVA, codContatto, enum_CodiciAnagrafe.ModalitaPagamentoDefault,
                                                                    parametri.Visibilita,
                                                                    If(parametri.DettagliContabilita.Mod_Pag_Default Is Nothing, 0, parametri.DettagliContabilita.Mod_Pag_Default),
                                                                    AGRODATAINIZIO, AGRODATAFINE)

            ' Iban Default 
            objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Cancellazione,
                                                                          parametri.PIVA, codContatto, enum_CodiciAnagrafe.IBANDefault,
                                                                          parametri.Visibilita,
                                                                          If(parametri.DettagliContabilita.Iban_Default Is Nothing, 0, parametri.DettagliContabilita.Iban_Default),
                                                                          AGRODATAINIZIO, AGRODATAFINE)
            objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Scrittura,
                                                                    parametri.PIVA, codContatto, enum_CodiciAnagrafe.IBANDefault,
                                                                    parametri.Visibilita,
                                                                    If(parametri.DettagliContabilita.Iban_Default Is Nothing, 0, parametri.DettagliContabilita.Iban_Default),
                                                                    AGRODATAINIZIO, AGRODATAFINE)

            ' ListinoPrezziAcquistoDefault
            objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Cancellazione,
                                                                          parametri.PIVA, codContatto, enum_CodiciAnagrafe.ListinoPrezziAcquistoDefault,
                                                                          parametri.Visibilita,
                                                                          If(parametri.DettagliContabilita.Listino_Prezzi_Acq Is Nothing, 0, parametri.DettagliContabilita.Listino_Prezzi_Acq),
                                                                          AGRODATAINIZIO, AGRODATAFINE)
            objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Scrittura,
                                                                    parametri.PIVA, codContatto, enum_CodiciAnagrafe.ListinoPrezziAcquistoDefault,
                                                                    parametri.Visibilita,
                                                                    If(parametri.DettagliContabilita.Listino_Prezzi_Acq Is Nothing, 0, parametri.DettagliContabilita.Listino_Prezzi_Acq),
                                                                    AGRODATAINIZIO, AGRODATAFINE)

            ' ListinoPrezziVenditaDefault
            objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Cancellazione,
                                                                          parametri.PIVA, codContatto, enum_CodiciAnagrafe.ListinoPrezziVenditaDefault,
                                                                          parametri.Visibilita,
                                                                          If(parametri.DettagliContabilita.Listino_Prezzi_Ven Is Nothing, 0, parametri.DettagliContabilita.Listino_Prezzi_Ven),
                                                                          AGRODATAINIZIO, AGRODATAFINE)
            objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Scrittura,
                                                                    parametri.PIVA, codContatto, enum_CodiciAnagrafe.ListinoPrezziVenditaDefault,
                                                                    parametri.Visibilita,
                                                                    If(parametri.DettagliContabilita.Listino_Prezzi_Ven Is Nothing, 0, parametri.DettagliContabilita.Listino_Prezzi_Ven),
                                                                    AGRODATAINIZIO, AGRODATAFINE)

            'Referente Conferimento
            objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Cancellazione,
                                                                          parametri.PIVA, codContatto, enum_CodiciAnagrafe.ReferenteConferimento,
                                                                          parametri.Visibilita,
                                                                          If(parametri.DettagliContabilita.Referente_Conferimento Is Nothing, 0, parametri.DettagliContabilita.Referente_Conferimento),
                                                                          AGRODATAINIZIO, AGRODATAFINE)
            objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Scrittura,
                                                                    parametri.PIVA, codContatto, enum_CodiciAnagrafe.ReferenteConferimento,
                                                                    parametri.Visibilita,
                                                                    If(parametri.DettagliContabilita.Referente_Conferimento Is Nothing, 0, parametri.DettagliContabilita.Referente_Conferimento),
                                                                    AGRODATAINIZIO, AGRODATAFINE)


            ' Gestione Vettore 1315
            objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Cancellazione,
                                                                          parametri.PIVA, codContatto, enum_CodiciAnagrafe.Gestione_Vettore_Default,
                                                                          parametri.Visibilita,
                                                                          If(parametri.DettagliContabilita.GestioneVettore_Cod Is Nothing, 0, parametri.DettagliContabilita.GestioneVettore_Cod),
                                                                          AGRODATAINIZIO, AGRODATAFINE)
            objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Scrittura,
                                                                parametri.PIVA, codContatto, enum_CodiciAnagrafe.Gestione_Vettore_Default,
                                                                parametri.Visibilita,
                                                                If(parametri.DettagliContabilita.GestioneVettore_Cod Is Nothing, 0, parametri.DettagliContabilita.GestioneVettore_Cod),
                                                                AGRODATAINIZIO, AGRODATAFINE)

        End If

        ' Calo Peso
        If parametri.CaloPesoDefault_daContatto Then
            If parametri.AltriDati.CaloPeso <> "" AndAlso parametri.AltriDati.CaloPeso <> "0" Then
                objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Cancellazione,
                                                                      parametri.PIVA, codContatto, enum_CodiciAnagrafe.CaloPesoDefault_daContatto,
                                                                      parametri.Visibilita, parametri.AltriDati.CaloPeso, AGRODATAINIZIO, AGRODATAFINE)
                objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Scrittura,
                                                                    parametri.PIVA, codContatto, enum_CodiciAnagrafe.CaloPesoDefault_daContatto,
                                                                    parametri.Visibilita, parametri.AltriDati.CaloPeso, AGRODATAINIZIO, AGRODATAFINE)
            Else
                objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Cancellazione,
                                                                      parametri.PIVA, codContatto, enum_CodiciAnagrafe.CaloPesoDefault_daContatto,
                                                                      parametri.Visibilita, "", AGRODATAINIZIO, AGRODATAFINE)
            End If

            If parametri.AltriDati.CoeffCaloPeso <> "" AndAlso parametri.AltriDati.CoeffCaloPeso <> "0" Then
                objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Cancellazione,
                                                                      parametri.PIVA, codContatto, enum_CodiciAnagrafe.CoeffCaloPesoContatto,
                                                                      parametri.Visibilita, parametri.AltriDati.CoeffCaloPeso, AGRODATAINIZIO, AGRODATAFINE)
                objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Scrittura,
                                                                    parametri.PIVA, codContatto, enum_CodiciAnagrafe.CoeffCaloPesoContatto,
                                                                    parametri.Visibilita, parametri.AltriDati.CoeffCaloPeso, AGRODATAINIZIO, AGRODATAFINE)
            Else
                objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Cancellazione,
                                                                      parametri.PIVA, codContatto, enum_CodiciAnagrafe.CoeffCaloPesoContatto,
                                                                      parametri.Visibilita, "", AGRODATAINIZIO, AGRODATAFINE)
            End If
        End If

        Dim objParametri = New ParametriAgenda()

        If Trim(codContatto.ToUpper()) = objParametri.Piva Then

            objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Cancellazione,
                                                                      parametri.PIVA, codContatto, enum_CodiciAnagrafe.Codice_Accise_UA,
                                                                      parametri.Visibilita, parametri.AltriDati.CodiceUA, AGRODATAINIZIO, AGRODATAFINE)
            objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Scrittura,
                                                                parametri.PIVA, codContatto, enum_CodiciAnagrafe.Codice_Accise_UA,
                                                                parametri.Visibilita, parametri.AltriDati.CodiceUA, AGRODATAINIZIO, AGRODATAFINE)


            objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Cancellazione,
                                                                      parametri.PIVA, codContatto, enum_CodiciAnagrafe.Codice_Accise_Conto_Garanzia,
                                                                      parametri.Visibilita, parametri.AltriDati.CodContoGaranzia, AGRODATAINIZIO, AGRODATAFINE)
            objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Scrittura,
                                                                    parametri.PIVA, codContatto, enum_CodiciAnagrafe.Codice_Accise_Conto_Garanzia,
                                                                    parametri.Visibilita, parametri.AltriDati.CodContoGaranzia, AGRODATAINIZIO, AGRODATAFINE)

            objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Cancellazione,
                                                                     parametri.PIVA, codContatto, enum_CodiciAnagrafe.Codice_Magazzino_Fiscale,
                                                                     parametri.Visibilita, parametri.AltriDati.RifDepositoFiscale, AGRODATAINIZIO, AGRODATAFINE)
            objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Scrittura,
                                                                    parametri.PIVA, codContatto, enum_CodiciAnagrafe.Codice_Magazzino_Fiscale,
                                                                    parametri.Visibilita, parametri.AltriDati.RifDepositoFiscale, AGRODATAINIZIO, AGRODATAFINE)

        End If


    End Sub

    Private Shared Sub PreparaSalvataggioProdottiCosti(ByVal piva As String,
                                                       ByRef DT_RisUm As DataTable, ByVal Dt_Prodotti As DataTable)

        Dim objXML_Utility As New AgronicaCoreXML.XML_Utility
        Dim risorseUmane = (From row In Dt_Prodotti.AsEnumerable()
                            Select row.Field(Of Integer)("mat_cod") Distinct).ToList()

        For Each ru As Integer In risorseUmane
            Dim dtProdXRu = objXML_Utility.CaricaGriglia_ProdottiCosti_for_XML()

            Dim costi As DataRow() = Dt_Prodotti.Select("mat_cod = " & ru)
            For Each row As DataRow In costi.AsEnumerable()
                dtProdXRu.ImportRow(row)
            Next

            Dim risorsaUmana As DataRow = DT_RisUm.Select("Cod_Risum = " & ru).FirstOrDefault()
            risorsaUmana("DT_ProdottiCosti") = dtProdXRu

        Next

    End Sub

    Private Shared Sub PreparaSalvataggioCosti(
        ByVal piva As String,
        ByVal righeInseriteGrid_Costi As String,
        ByVal righeModificateGrid_Costi As String,
        ByVal righeEliminateGrid_Costi As String,
        ByRef DT_Prodotti As DataTable)

        Dim objXML_Utility As New AgronicaCoreXML.XML_Utility

        Dim id As Integer
        Dim cod_risum As Integer
        Dim udm_Cod As Integer
        Dim prezzo As Decimal
        Dim inizioPrezzo As DateTime = AGRODATAINIZIO
        Dim finePrezzo As DateTime = AGRODATAFINE

        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        ' 1 - RIGHE ELIMINATE
        Dim righeEliminate As List(Of CostiModel) = New List(Of CostiModel)()
        If Not String.IsNullOrEmpty(righeEliminateGrid_Costi) Then
            righeEliminate.AddRange(JsonConvert.DeserializeObject(Of List(Of CostiModel))(righeEliminateGrid_Costi, settingLoc))
        End If
        For Each c As CostiModel In righeEliminate
            LeggiCosto(c, id, cod_risum, udm_Cod, prezzo, inizioPrezzo, finePrezzo)
            objXML_Utility.Inserisci_Riga_Dt_ProdottiCosti_for_XML(DT_Prodotti, enum_TipoOperazioneDB.Cancellazione, id, piva, "", 0, 0, cod_risum, 0, udm_Cod, prezzo, 0, 0, inizioPrezzo, finePrezzo)
        Next

        '' 2 RIGHE INSERITE / MODIFICATE
        Dim inseritiModificati As List(Of CostiModel) = New List(Of CostiModel)()
        If Not String.IsNullOrEmpty(righeInseriteGrid_Costi) Then
            inseritiModificati.AddRange(JsonConvert.DeserializeObject(Of List(Of CostiModel))(righeInseriteGrid_Costi, settingLoc))
        End If
        If Not String.IsNullOrEmpty(righeModificateGrid_Costi) Then
            inseritiModificati.AddRange(JsonConvert.DeserializeObject(Of List(Of CostiModel))(righeModificateGrid_Costi, settingLoc))
        End If
        For Each c As CostiModel In inseritiModificati
            LeggiCosto(c, id, cod_risum, udm_Cod, prezzo, inizioPrezzo, finePrezzo)
            Dim tipoOperazione As Integer = If(id > 0, enum_TipoOperazioneDB.Modifica, enum_TipoOperazioneDB.Scrittura)
            objXML_Utility.Inserisci_Riga_Dt_ProdottiCosti_for_XML(DT_Prodotti, tipoOperazione, id, piva, "", 0, 0, cod_risum, 0, udm_Cod, prezzo, 0, 0, inizioPrezzo, finePrezzo)
        Next

    End Sub

    Private Shared Sub LeggiCosto(
            ByVal c As CostiModel,
            ByRef id As Integer,
            ByRef cod_risum As Integer,
            ByRef udm_Cod As Integer,
            ByRef prezzo As Decimal,
            ByRef inizioPrezzo As DateTime,
            ByRef finePrezzo As DateTime
        )

        id = 0
        If Not c.Id Is Nothing AndAlso c.Id.HasValue Then
            id = c.Id
        End If

        cod_risum = c.Cod_RisUm.Value

        prezzo = 0
        If Not c.Prezzo Is Nothing AndAlso c.Prezzo.HasValue Then
            prezzo = c.Prezzo.Value
        End If

        udm_Cod = 0
        If Not c.Udm_Cod Is Nothing AndAlso c.Udm_Cod.HasValue Then
            udm_Cod = c.Udm_Cod.Value
        End If

        inizioPrezzo = AGRODATAINIZIO
        If Not c.InizioPrezzo Is Nothing AndAlso c.InizioPrezzo.HasValue Then
            inizioPrezzo = c.InizioPrezzo
        End If

        finePrezzo = AGRODATAFINE
        If Not c.FinePrezzo Is Nothing AndAlso c.FinePrezzo.HasValue Then
            finePrezzo = c.FinePrezzo
        End If

    End Sub
    Private Shared Sub PreparaSalvataggioRubrica(ByVal righeInseriteGrid_Rubrica As String,
                                                 ByVal righeModificateGrid_Rubrica As String,
                                                 ByVal righeEliminateGrid_Rubrica As String,
                                                 ByRef Dt_Rubrica As DataTable)

        Dim objXML_Utility As New AgronicaCoreXML.XML_Utility

        Dim cod_rubrica As Long
        Dim descrizione As String = ""
        Dim numero As String = ""

        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim inseritiModificati As New List(Of RubricaModel)()
        Dim elimintati As New List(Of RubricaModel)()

        If Not String.IsNullOrEmpty(righeEliminateGrid_Rubrica) Then
            elimintati.AddRange(JsonConvert.DeserializeObject(Of List(Of RubricaModel))(righeEliminateGrid_Rubrica, settingLoc))
        End If

        If Not String.IsNullOrEmpty(righeInseriteGrid_Rubrica) Then
            inseritiModificati.AddRange(JsonConvert.DeserializeObject(Of List(Of RubricaModel))(righeInseriteGrid_Rubrica, settingLoc))
        End If
        If Not String.IsNullOrEmpty(righeModificateGrid_Rubrica) Then
            inseritiModificati.AddRange(JsonConvert.DeserializeObject(Of List(Of RubricaModel))(righeModificateGrid_Rubrica, settingLoc))
        End If

        If elimintati.Any() Then
            For Each r As RubricaModel In elimintati
                cod_rubrica = If(r.Cod_Rubrica Is Nothing, 0, r.Cod_Rubrica)
                descrizione = If(String.IsNullOrEmpty(r.Descrizione), "", r.Descrizione)
                numero = If(String.IsNullOrEmpty(r.Numero), "", r.Numero)

                objXML_Utility.Inserisci_Riga_Dt_Rubrica_for_XML(Dt_Rubrica, enum_TipoOperazioneDB.Cancellazione, cod_rubrica,
                                                             numero, descrizione)
            Next
        End If


        If inseritiModificati.Any() Then

            For Each r As RubricaModel In inseritiModificati
                cod_rubrica = If(r.Cod_Rubrica Is Nothing, 0, r.Cod_Rubrica)
                descrizione = If(String.IsNullOrEmpty(r.Descrizione), "", r.Descrizione)
                numero = If(String.IsNullOrEmpty(r.Numero), "", r.Numero)
                If cod_rubrica = 0 Then
                    objXML_Utility.Inserisci_Riga_Dt_Rubrica_for_XML(Dt_Rubrica, enum_TipoOperazioneDB.Scrittura, 0,
                                                                   numero, descrizione)
                Else
                    objXML_Utility.Inserisci_Riga_Dt_Rubrica_for_XML(Dt_Rubrica, enum_TipoOperazioneDB.Modifica, cod_rubrica,
                                                                   numero, descrizione)
                End If
            Next

        End If

    End Sub

    Private Shared Sub PreparaSalvataggioRapportiContabili(
            ByVal objParametri_Server As AgronicaCoreParametri,
            ByVal piva As String,
            ByVal codContatto As String,
            ByVal sa_cod As Integer,
            ByVal righeInseriteGrid_Rapporti_Contabili As String,
            ByVal righeModificate_Rapporti_Contabili As String,
            ByVal righeEliminate_Rapporti_Contabili As String,
            ByRef DT_RisUm As DataTable,
            ByRef Dt_Prodotti As DataTable
        )

        Dim righeArray_Dettaglio As JArray
        Dim objContatto_R As New AgronicaCoreAnagrafeBIZ.Contatti_MultiHost_R
        Dim objXML_Utility As New AgronicaCoreXML.XML_Utility
        Dim cod_rapp As Integer
        Dim cod_risum As Integer
        Dim tipoOperazioneDB_RapCont As Integer
        Dim dal As DateTime = AGRODATAINIZIO
        Dim al As DateTime = AGRODATAFINE
        Dim progressivo As String = ""
        Dim attivita As String = ""
        Dim numero_patentino As String = ""
        Dim Ente_di_rilascio As String = ""
        Dim data_rilascio As DateTime = AGRODATAINIZIO
        Dim data_scadenza As DateTime = AGRODATAFINE
        Dim oreSettimanali As Decimal = 0
        Dim occasionale As Integer = 0
        Dim mansioneCod As Integer = 0
        Dim classificazioneCod As Integer = 0
        Dim qualificaCod As Integer = 0
        Dim infoFamiglia As String = ""
        Dim Cod_Iva_Contatto As Integer = -1
        Dim Cod_Conto_Econ As Integer = 0
        Dim Cod_Conto_Pat As Integer = 0
        Dim Sa_Cod_RisUm As Integer = 0

        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim inseritiModificati As New List(Of RapportoContabileModel)()
        '**************************************************************************
        ' 1) RAPPORTI CONTABILI
        '**************************************************************************

        ' 1A - RIGHE ELIMINATE
        If righeEliminate_Rapporti_Contabili <> "" Then
            righeArray_Dettaglio = JArray.Parse(righeEliminate_Rapporti_Contabili)
            For Each rc As JObject In righeArray_Dettaglio

                cod_risum = Val(rc("Cod_RisUm")).ToString()

                'objXML_Utility.Inserisci_Riga_Dt_ProdottiCosti_for_XML(Dt_Prodotti, enum_TipoOperazioneDB.Cancellazione, 0, piva, "", 0, 0, cod_risum, 0, 0, 0, 0, 0)
                objXML_Utility.Inserisci_Riga_Dt_RisUm_for_XML(DT_RisUm, enum_TipoOperazioneDB.Cancellazione, piva, codContatto, , cod_risum, cod_rapp, , , ,
                                                          , , , , , , , , , , , Nothing, )
            Next

        End If

        If Not String.IsNullOrEmpty(righeInseriteGrid_Rapporti_Contabili) Then
            inseritiModificati.AddRange(JsonConvert.DeserializeObject(Of List(Of RapportoContabileModel))(righeInseriteGrid_Rapporti_Contabili, settingLoc))
        End If
        If Not String.IsNullOrEmpty(righeModificate_Rapporti_Contabili) Then
            inseritiModificati.AddRange(JsonConvert.DeserializeObject(Of List(Of RapportoContabileModel))(righeModificate_Rapporti_Contabili, settingLoc))
        End If

        For Each rc In inseritiModificati
            cod_risum = If(rc.Cod_RisUm Is Nothing, 0, rc.Cod_RisUm)
            Sa_Cod_RisUm = sa_cod
            If cod_risum = 0 Then
                tipoOperazioneDB_RapCont = enum_TipoOperazioneDB.Scrittura
            Else
                tipoOperazioneDB_RapCont = enum_TipoOperazioneDB.Modifica
            End If
            cod_rapp = rc.Cod_Rapporto
            dal = rc.Validita_Inizio
            al = rc.Validita_Fine
            progressivo = rc.Settore_Des
            attivita = rc.Attivita_Des
            oreSettimanali = rc.Ore_Settimanali
            occasionale = If(rc.TipoRapporto_Cod, 0)
            mansioneCod = If(rc.Mansione_Cod, 0)
            'occasionale = If(rc.TipoRapporto_Cod, 0)
            classificazioneCod = If(rc.Classificazione_Cod, 0)
            qualificaCod = If(rc.Qualifica_Cod, 0)
            infoFamiglia = rc.Info_Famiglia
            Cod_Iva_Contatto = If(rc.Cod_IVA, -1)
            Cod_Conto_Econ = If(rc.Cod_Conto, 0)
            Cod_Conto_Pat = If(rc.Cod_Conto_Pat, 0)

            objXML_Utility.Inserisci_Riga_Dt_RisUm_for_XML(DT_RisUm,
                                                          tipoOperazioneDB_RapCont,
                                                          piva,
                                                          codContatto,
                                                          Sa_Cod_RisUm,
                                                          cod_risum,
                                                          cod_rapp,
                                                          progressivo,
                                                          attivita,
                                                          occasionale,
                                                          oreSettimanali, , , ,
                                                          numero_patentino,
                                                          data_rilascio,
                                                          data_scadenza, , ,
                                                          dal,
                                                          al,
                                                          Nothing,
                                                          Ente_di_rilascio, , , ,
                                                          qualificaCod,
                                                          mansioneCod,
                                                          classificazioneCod,
                                                          infoFamiglia,
                                                          Cod_Iva_Contatto:=Cod_Iva_Contatto,
                                                          Cod_Conto_Econ:=Cod_Conto_Econ,
                                                          Cod_Conto_Pat:=Cod_Conto_Pat)

        Next

    End Sub

    Private Sub clear_form()

        Cmb_CentriAziendali.ClearSelection()
        Txt_CF.Text = ""
        Txt_Cognome.Text = ""
        Txt_Nome.Text = ""
        Txt_DataNascita.Text = ""
        ddl_Sesso.ClearSelection()

    End Sub


    '##########################################################################################################################################
    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim objParametriAgenda As New ParametriAgenda

        ' Setto nuovamente la PIVA del centro aziendale x filtro su menu
        If objParametriAgenda.Piva_Origine <> "" AndAlso objParametriAgenda.Piva_Origine <> objParametriAgenda.Piva Then
            objParametriAgenda.Piva = objParametriAgenda.Piva_Origine
        End If

        Dim TargetUrl As String = ""

        If objParametriAgenda.SitoOrigine = Enum_SiteRedirector.GiasNG Then

            MenuBS_2017_RedirectGestione.RedirectGenerico(objParametriAgenda.Piva,
                                                          Enum_SiteRedirector.GiasNG,
                                                          enum_PagineGiasNG.Pagina_Menu_Anagrafica_Contatti,
                                                          TargetUrl,
                                                          objParametri_Server)

        Else


            TargetUrl = Master.TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAnagrafica, objParametriAgenda, , Qs_Visibilita)



        End If

        Response.Redirect(TargetUrl)

    End Sub

    Private Sub AnnullaDaPopup(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim strJS1 As New StringBuilder
        strJS1.AppendLine("$(document).ready(Function() { ")

        If Session("ModificatoCF") Is Nothing Then
            strJS1.AppendLine("      gestisciValore_esci(); ")
        Else
            'se ho modificato il CF devo ricaricare l'elenco nella contatti_manager ...
            strJS1.AppendLine("      gestisciValore('" & Txt_CF.Text & "'); ")
            Session("ModificatoCF") = Nothing
        End If

        strJS1.AppendLine(" });")
        ScriptManager.RegisterStartupScript(Script_Panel, Script_Panel.GetType,
                                                                    String.Format("jQuery_{0}", Script_Panel),
                                                                    strJS1.ToString, True)

    End Sub

    Private Sub Contatto_Edit_Init(sender As Object, e As System.EventArgs) Handles Me.Init

        ' VAnni: 2/10/2017: a seguito di AgroMaseterPage
        'AddHandler CType(Page.Master.FindControl("ImgBtn_AnnullaTutto"), ImageButton).Click, AddressOf Me.AnnullaTutto
        AddHandler CType(Me.Master, AgendaBootstrap).ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto
        AddHandler ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaDaPopup
        CType(Me.Master, AgendaBootstrap).flag_MostraBtnIndietro = True

    End Sub

    Private Sub RBL_TipoUtente_SelectedIndexChanged(sender As Object, e As EventArgs) Handles RBL_TipoUtente.SelectedIndexChanged

    End Sub

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function CambiaCF(ByVal CF_Old As String, ByVal CF_Nuovo As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim objTabelle_R As New AgronicaCoreVarieDAL.System_Database_R
        Dim objTabelle_W As New AgronicaCoreVarieDAL.System_Database_W
        Dim ModificatoCampo As Boolean
        Dim RecordModificati As Integer = 0
        Dim DT_Tabelle As DataTable
        Dim i, j As Integer

        '------------------------------
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try

            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri_Server)

            DT_Tabelle = objTabelle_R.Leggi_ElencoNomiTabelle_2(objParametri_Server)

            If Not DT_Tabelle Is Nothing Then
                For i = 0 To DT_Tabelle.Rows.Count - 1
                    Dim Dt_Campi As DataTable
                    Dt_Campi = objTabelle_R.Leggi_ElencoNomiColonneTabella_3(DT_Tabelle.Rows(i).Item("TABLE_NAME"), objParametri_Server)
                    If Not Dt_Campi Is Nothing Then
                        For j = 0 To Dt_Campi.Rows.Count - 1
                            If LCase(Dt_Campi.Rows(j).Item("NAME")) = "cod_contatto" Then
                                ModificatoCampo = objTabelle_W.Modifica_Valore_Campo_Tabella(DT_Tabelle.Rows(i).Item("TABLE_NAME"), Dt_Campi.Rows(j).Item("NAME"), CF_Old, CF_Nuovo, "", objParametri_Server)
                                If ModificatoCampo Then
                                    RecordModificati += 1
                                End If
                                Select Case LCase(DT_Tabelle.Rows(i).Item("TABLE_NAME"))
                                    Case "liquidita"
                                        ModificatoCampo = objTabelle_W.Modifica_Valore_Campo_Tabella(DT_Tabelle.Rows(i).Item("TABLE_NAME"), "Riferimento", CF_Old, CF_Nuovo, "", objParametri_Server)
                                        If ModificatoCampo Then
                                            RecordModificati += 1
                                        End If
                                End Select

                            End If
                        Next
                    End If
                Next
            End If

            r.RispostaOK = True

            If RecordModificati > 0 Then
                r.RispostaConferma = True
            Else
                r.RispostaConferma = False
            End If

            r.RispostaStringa = DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/New_Contatto_Edit.aspx", "OperazioneEseguitaCorrettamente"), String)

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)

        Catch ex As Exception


            'Faccio il rollback della transazione
            If Not objParametri_Server.objTransazione Is Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If


            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        Finally

            'Chiudo la connessione se è stata aperta in questa routine
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

        End Try

        Return r

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function RedirectToMenuAnagrafica(ByVal targetUrl As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim objParametriAgenda As New ParametriAgenda

            ' Setto nuovamente la PIVA del centro aziendale x filtro su menu
            If objParametriAgenda.Piva_Origine <> "" AndAlso objParametriAgenda.Piva_Origine <> objParametriAgenda.Piva Then
                objParametriAgenda.Piva = objParametriAgenda.Piva_Origine
            End If

            If objParametriAgenda.SitoOrigine = Enum_SiteRedirector.GiasNG Then

                MenuBS_2017_RedirectGestione.RedirectGenerico(objParametriAgenda.Piva,
                                                              Enum_SiteRedirector.GiasNG,
                                                              enum_PagineGiasNG.Pagina_Menu_Anagrafica_Contatti,
                                                              targetUrl,
                                                              objParametri_Server)

                'targetUrl &= "?visibilita=1"

            End If

            r.RispostaStringa = targetUrl
            r.RispostaOK = True

        Catch ex As Exception

            If Not objParametri_Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try


        Return r

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function CambiaPiva(ByVal Piva_Old As String, ByVal Piva_Nuovo As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim objTabelle_R As New AgronicaCoreVarieDAL.System_Database_R
        Dim objTabelle_W As New AgronicaCoreVarieDAL.System_Database_W
        Dim ModificatoCampo As Boolean
        Dim RecordModificati As Integer = 0
        Dim DT_Tabelle As DataTable
        Dim i, j As Integer

        '------------------------------
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try

            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri_Server)

            DT_Tabelle = objTabelle_R.Leggi_ElencoNomiTabelle_2(objParametri_Server)

            If Not DT_Tabelle Is Nothing Then
                For i = 0 To DT_Tabelle.Rows.Count - 1
                    Dim Dt_Campi As DataTable
                    Dt_Campi = objTabelle_R.Leggi_ElencoNomiColonneTabella_3(DT_Tabelle.Rows(i).Item("TABLE_NAME"), objParametri_Server)
                    If Not Dt_Campi Is Nothing Then
                        For j = 0 To Dt_Campi.Rows.Count - 1
                            If LCase(Dt_Campi.Rows(j).Item("NAME")) = "cod_contatto" Then
                                ModificatoCampo = objTabelle_W.Modifica_Valore_Campo_Tabella(DT_Tabelle.Rows(i).Item("TABLE_NAME"), Dt_Campi.Rows(j).Item("NAME"), Piva_Old, Piva_Nuovo, "", objParametri_Server)
                                If ModificatoCampo Then
                                    RecordModificati += 1
                                End If
                                Select Case LCase(DT_Tabelle.Rows(i).Item("TABLE_NAME"))
                                    Case "liquidita"
                                        ModificatoCampo = objTabelle_W.Modifica_Valore_Campo_Tabella(DT_Tabelle.Rows(i).Item("TABLE_NAME"), "Riferimento", Piva_Old, Piva_Nuovo, "", objParametri_Server)
                                        If ModificatoCampo Then
                                            RecordModificati += 1
                                        End If
                                End Select
                            End If
                        Next
                    End If
                Next
            End If

            r.RispostaOK = True

            If RecordModificati > 0 Then
                r.RispostaConferma = True
            Else
                r.RispostaConferma = False
            End If

            r.RispostaStringa = DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/New_Contatto_Edit.aspx", "OperazioneEseguitaCorrettamente"), String)

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)

        Catch ex As Exception


            'Faccio il rollback della transazione
            If Not objParametri_Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        Finally

            'Chiudo la connessione se è stata aperta in questa routine
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

        End Try

        Return r

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaStati() As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim DT_Nazioni As DataTable
        Dim jarrayNazioni As New JArray()

        Try

            Dim objNazioni As New AgronicaCoreMetaSchemaDAL.ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166_R
            DT_Nazioni = objNazioni.Leggi("", "", "", objParametri_Server)


            For Each dr As DataRow In DT_Nazioni.Rows

                Dim descrizione = dr("Descrizione").ToString()
                Dim codice = dr("Codice").ToString()
                Dim gestione_gerarchia_geografica = dr("Gestione_Gerarchia_Geografica").ToString()

                jarrayNazioni.Add(
                    New JObject(
                        New JProperty("Stato", codice),
                        New JProperty("Stato_Des", descrizione),
                        New JProperty("Gestione_Gerarchia_Geografica", gestione_gerarchia_geografica)))

            Next

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(jarrayNazioni, Newtonsoft.Json.Formatting.None)
            'cmb_Stato.Items.Add(New ListItem("SELEZIONA", ""))

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function


    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaLingue() As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim DT_Lingue As DataTable
        Dim jarrayLingue As New JArray()

        Try

            Dim objLingue As New AgronicaCoreMetaSchemaDAL.ACCDAA_ANAG_T001_TabellaCodiceDelleLingue_R()
            DT_Lingue = objLingue.Leggi("", "", "", objParametri_Server)


            For Each dr As DataRow In DT_Lingue.Rows

                Dim descrizione = dr("Descrizione").ToString()
                Dim codice = dr("Codice").ToString()

                jarrayLingue.Add(
                    New JObject(
                        New JProperty("Codice_Lingua", codice),
                        New JProperty("Lingua_Des", descrizione)))

            Next

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(jarrayLingue, Newtonsoft.Json.Formatting.None)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Ottieni_Link_PosiIt(
        ByVal piva As String,
        ByVal Cod_RisUm As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim PaginaLink As String
        PaginaLink = "../Editor/Editor.aspx"

        Try

            PaginaLink &=
                            "?p=" & Stringa_Codifica(piva, AgroKey_EncoderDecoder) &
                            "&Cod_RisUm=" & Stringa_Codifica(Cod_RisUm, AgroKey_EncoderDecoder)
            r.RispostaOK = True
            r.RispostaStringa = PaginaLink

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function Ottieni_Link_Modifica_Contatto(ByVal piva, ByVal cod_contatto) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim objParametriAgenda = New ParametriAgenda
        objParametriAgenda.Cod_Contatto = cod_contatto
        objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica
        Dim PaginaLink As String


        PaginaLink = "../Anagrafica/New_Contatto_Edit.aspx"

        Try

            PaginaLink &=
                            "?o=" & Stringa_Codifica(enum_TipoOperazioneDB.Modifica, AgroKey_EncoderDecoder) &
                            "&piva=" & Stringa_Codifica(piva, AgroKey_EncoderDecoder) &
                            "&codcont=" & Stringa_Codifica(cod_contatto, AgroKey_EncoderDecoder) &
                            "&datiPat=" & Stringa_Codifica("1", AgroKey_EncoderDecoder)

            If AperturaDaPopup Then
                PaginaLink &= "&orig="

                If Not String.IsNullOrWhiteSpace(Qs_Origine) Then
                    PaginaLink &= Stringa_Codifica(Qs_Origine, AgroKey_EncoderDecoder)
                End If
            End If

            If Not String.IsNullOrWhiteSpace(Qs_apertodaGiasNG) Then
                PaginaLink &= "&apertodaGiasNG=" & Stringa_Codifica(Qs_apertodaGiasNG, AgroKey_EncoderDecoder)
            End If

            r.RispostaOK = True
            r.RispostaStringa = PaginaLink

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function GestioneAllegati(ByVal Piva As String, ByVal Cod_Contatto As String) As String

        Return AgronicaCoreGestioneRichieste.RedirectGestione.GetLinkGestioneAllegati(
            Enum_SiteRedirector.Sito_AgronicaAgenda_2010, Piva, Cod_Contatto, enum_ID_Area_Tipologia.Patentino_trattamenti)

    End Function

    Friend Class InidirizzoTipoModel
        Public key_tipo_indirizzo As String = String.Empty
        Public InidrizzoTipoCod As Integer?
        Public IndirizzoTipoDes As String = String.Empty
        Public ApplicabilitaCod As Integer?
        Public ApplicabilitaDes As String = String.Empty
        Public Cod_Contatto As String = String.Empty
        Public Contatto_Des As String = String.Empty
        Public IndirizzoDiSistema As Boolean
    End Class
    Friend Class ParametriSalvaIndirizziTipoModel

        Public righeInserite As String
        Public righeModificate As String
        Public righeCancellate As String
        Public righeNonCancellate As String

    End Class

#Region "script services user control associa listini"
    <WebMethod(EnableSession:=True)>
    Public Shared Function CercaAssociazioniListini(ByVal piva As String, ByVal tipoClasse As Integer, ByVal codContatto As String) As RispostaStandard
        Return ListiniPrezzixContattiUC.CercaAssociazioniListini(piva, tipoClasse, codContatto)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function AssociaListini(
            ByVal piva As String, ByVal listinoCod As Integer(), ByVal saCod As Integer(), ByVal codRapporto As Integer(),
            ByVal codContatto As String(), ByVal codContattoProduttore As String(), ByVal charContattiRapporti As Char, ByVal flagProduttori As Boolean
        ) As RispostaStandard

        Return ListiniPrezzixContattiUC.AssociaListini(piva, listinoCod, saCod, codRapporto, codContatto, codContattoProduttore, charContattiRapporti, flagProduttori)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CancellaAssociazioniListini(ByVal piva As String, ByVal arrAssocDaCanc As Object()) As RispostaStandard
        Return ListiniPrezzixContattiUC.CancellaAssociazioniListini(piva, arrAssocDaCanc)
    End Function

    Private Sub New_Contatto_Edit_PreInit(sender As Object, e As EventArgs) Handles Me.PreInit

    End Sub

#End Region

    ''' <summary>
    ''' ANNA 03 / 24 -- WIP ALLINEAMENTO PATENTINO PASSANDO DA DOCUMENTALE.INTERROTTO, RIPRENDERE IN FUTURO!
    ''' </summary>
    ''' <param name="piva"></param>
    ''' <param name="cod_contatto"></param>
    ''' <param name="operazione"></param>
    ''' <returns></returns>
    <WebMethod(EnableSession:=True)>
    Public Shared Function GetUrlDocAgenda2010(ByVal piva As String, ByVal cod_contatto As String, ByVal operazione As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim link As String = ""

            Dim ParametriScadenziario As New ParametriScadenziario
            Select Case operazione
                Case "Add"
                    ParametriScadenziario.Pagina_Richiesta = enum_PagineAgenda_2010.Pagina_Scadenzario_CreaModifica
                Case "Read"
                    ParametriScadenziario.Pagina_Richiesta = enum_PagineAgenda_2010.Pagina_Scadenzario_Lista
            End Select

            ParametriScadenziario.Piva = piva
            ParametriScadenziario.Id_Tipologia = enum_ID_Area_Tipologia.Patentino_trattamenti
            ParametriScadenziario.Id_Area = enum_ID_Area_Alert.Contatti

            ParametriScadenziario.QueryStringFiltrino += "&Cod_Contatto=" & cod_contatto

            link = RedirectGestione.IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriScadenziario(Enum_SiteRedirector.Sito_GiasOnline_2010, ParametriScadenziario)

            r.RispostaOK = True
            r.RispostaStringa = link

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function
End Class


Public Class ParametriSalvaTutto

    Public righeInseriteGrid_Rapporti_Contabili As String
    Public righeModificateGrid_Rapporti_Contabili As String
    Public righeCancellateGrid_Rapporti_Contabili As String
    Public righeTotaliGrid_Rapporti_Contabili As Long
    Public righeNonCancellateGrid_Rapporti_Contabili As String

    Public righeInseriteGrid_Costi As String
    Public righeModificateGrid_Costi As String
    Public righeCancellateGrid_Costi As String
    Public righeNonCancellateGrid_Costi As String

    Public righeInseriteGrid_Rubrica As String
    Public righeModificateGrid_Rubrica As String
    Public righeCancellateGrid_Rubrica As String

    Public righeInseriteGrid_Liquidita As String
    Public righeModificateGrid_Liquidita As String
    Public righeCancellateGrid_Liquidita As String
    Public righeNonCancellateGrid_Liquidita As String

    Public righeInseriteGrid_Indirizzi As String
    Public righeModificateGrid_Indirizzi As String
    Public righeCancellateGrid_Indirizzi As String
    Public righeNonCancellateGrid_Indirizzi As String

    Public righeInseriteGrid_Conti As String
    Public righeModificateGrid_Conti As String
    Public righeCancellateGrid_Conti As String
    Public righeNonCancellateGrid_Conti As String

    Public CodContatto As String
    Public RagioneSociale As String

    Public Nome_Breve_Azienda As String
    Public Nome_Breve_Persona As String
    Public Cognome As String
    Public Nome As String
    Public DataNascita As String
    Public Sesso As String
    Public Visibilita As Integer
    Public TipoUtente As String
    Public PIVA As String
    Public CodiceFiscale As String
    Public NumeroBadge As String
    Public Convenevoli As String
    Public CodiceFiscaleEstero As String
    Public Fittizio As Boolean
    Public ItaEste As Integer
    Public EUDR As Boolean

    Public TipologiaContatto As String
    Public RappFiscale As String
    Public PEC As String
    Public CodiceSDI As String
    Public DichIntentiProtocollo As String
    Public DichIntentiData As String


    Public AltriDati As New ParametriSalva_AltriDati()
    Public DettagliContabilita As New ParametriSalva_DettagliContabilita()

    Public Memo As String

    Public TipoOperazione As Integer
    Public OpzioniContatti As String
    Public TipoSalva As String
    Public GestisciContabilita As Boolean
    Public ImpresaGias As Boolean
    Public CaloPesoDefault_daContatto As Boolean

End Class

Public Class ParametriSalva_AltriDati

    Public Note As String
    Public Note2 As String
    Public NoteOperazioni As String
    Public NoteOperazioni2 As String
    Public CodiceAccisa As String
    Public CodiceUA As String
    Public CodContoGaranzia As String
    Public TipoDestinazione As String
    Public OrigineDestinazione As String
    Public UfficioDogane As String
    Public RifDepositoFiscale As String
    Public CaloPeso As String
    Public CoeffCaloPeso As String

End Class

Public Class ParametriSalva_DettagliContabilita

    Public Sconto_Cliente As String
    Public Sconto_Add1 As String
    Public Sconto_Add2 As String
    Public Sconto_Add3 As String
    Public Provvigione_Capo_area As String
    Public Provvigione_Agente As String
    Public Agente_Cod As String
    Public CapoArea_Cod As String
    Public Vettore_Cod As String
    Public Referente_Conferimento As String
    Public GestioneVettore_Cod As String
    Public Iva_Default As String
    Public Conto_Econ As String
    Public Conto_Pat As String
    Public Iban_Default As String
    Public Mod_Pag_Default As String
    Public Fatturazione_Automatica As String
    Public Documento_Fatturazione As String
    Public Destinazione_Diversa As String
    Public Listino_Prezzi_Acq As String
    Public Listino_Prezzi_Ven As String
    Public DestinazioneDiversa_Cod As String
    Public IndirizzoDestinazioneDiversa_Cod As String
    Public TipoIndirizzoDefault As String
    Public IndirizzoFatturazione_Cod As String

End Class

Public Class RapportoContabileModel

    Public Cod_Rapporto_Origine As Integer?
    Public Cod_RisUm As Integer?
    Public piva As String
    Public sa_cod As Integer?
    Public Cod_Contatto As String
    Public Cod_Rapporto As Integer?
    Public Settore_Des As String
    Public Rapporto_Des As String
    Public Rapporto_Des_Origine As String
    Public Qualifica_Cod As Integer?
    Public Qualifica_Des As String
    Public Mansione_Cod As Integer?
    Public Mansione_Des As String
    Public Validita_Inizio As DateTime?
    Public Validita_Fine As DateTime?
    Public Attivita_Des As String
    Public occasionale As Integer?
    Public Ore_Settimanali As Decimal?
    Public Info_Famiglia As String
    Public Classificazione_Cod As Integer?
    Public Classificazione_Des As String
    Public TipoRapporto_Cod As Integer?
    Public TipoRapporto_Des As String
    Public key_rap_cont As String
    Public Cod_IVA As Integer?
    Public Cod_Conto As Integer?
    Public Cod_Conto_Pat As Integer?
    Public Descrizione As String
    Public Conto_Descr As String
    Public Descr_Conto_Pat As String

End Class

Public Class CostiModel

    Public Id As Integer?
    Public Cod_RisUm As Integer?
    Public Cod_RisUm_Des As String
    Public Udm_Des As String
    Public Prezzo As Decimal?
    Public InizioPrezzo As DateTime?
    Public FinePrezzo As DateTime?
    Public Udm_Cod As Integer?

End Class

Public Class ContiModel

    Public key_conto As String
    Public Cod_Conto As Integer?
    Public Conto_Descr As String
    Public ID_Riclassificazione As String
    Public Validita_Inizio As DateTime?
    Public Validita_Fine As DateTime?

End Class

Public Class IndirizzoModel

    Public Cod_Indirizzo As String

    Public Tipo_Indirizzo As String
    Public Tipo_Indirizzo_Desc As String
    Public Via As String
    Public Sigla_Prov As String
    Public Provincia_des As String

    ' par(5)
    Public Provincia_cod As String

    ' par(6)
    Public Comune_cod As String

    Public Comune_des As String
    Public Frazione As String
    Public Cap As String
    Public Stato As String
    Public Stato_Des As String
    Public Note As String

    Public Codice_Lingua As String
    Public LIngua_Des As String

    Public ComCodIstat As String
    Public ProCodIstat As String

    Public Citta_Des As String
    Public Gestione_Gerarchia_Geografica As Integer

End Class


Public Class LiquiditaModel

    Public Sa_Cod As Integer?
    Public Cod_Liquidita As Integer?
    Public Cod_Istituto As Integer?
    Public Cod_Contatto As String
    Public Istituto_Des As String
    Public Nazione As String
    Public Cifre_Controllo As String
    Public Cin As String
    Public Abi As String
    Public Cab As String
    Public Numero As String
    Public Bic As String
    Public ChkAbilitazione As Integer?
    Public Abilitazione_Des As String
    Public Validita_Inizio As DateTime?
    Public Validita_Fine As DateTime?
    Public Note As String
    Public Selected As Boolean
    Public ChkDefault? As Boolean

End Class

Public Class RubricaModel

    Public Key_Rubrica As String
    Public Cod_Rubrica As Long?
    Public TipoRubrica_Des As String
    Public Numero As String
    Public Descrizione As String

End Class

Public Class Period
    Public StartP As Date
    Public EndP As Date

    Public Sub New(ByVal startP As Date, ByVal endP As Date)
        Me.StartP = startP
        Me.EndP = endP
    End Sub

    Public Function IntersectsWith(ByVal otherPeriod As Period) As Boolean

        Return Not (StartP > otherPeriod.EndP OrElse EndP < otherPeriod.StartP)

    End Function


End Class