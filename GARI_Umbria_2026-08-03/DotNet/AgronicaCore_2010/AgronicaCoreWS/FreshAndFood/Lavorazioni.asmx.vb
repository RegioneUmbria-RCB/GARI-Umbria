Imports System.Web.Services
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreModello
Imports System.Reflection
Imports Newtonsoft.Json
Imports AgronicaCoreContabDAL
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri



' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
Public Class Lavorazioni
    Inherits System.Web.Services.WebService

#Region "Metodi Web Pubblici"
    <WebMethod()>
    Public Function OttieniRispostaAlgoritmo(ByVal objP_server As String,
                                             ByVal lavorazione As AgronicaCoreModello.Lavorazione,
                                             ByVal nomeClasse As String,
                                             ByVal nomeFunzione As String) As rispostaStandard(Of RispostaAlgoritmo)

        Dim rispostaStd As New rispostaStandard(Of RispostaAlgoritmo)
        Dim rispostaAlg As New RispostaAlgoritmo
        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)

        Try
            'Controlli input
            If IsNothing(objP_server) Then
                rispostaAlg.RisultatoErrore = "Parametri server non indicati"
                Exit Try
            End If
            If IsNothing(lavorazione) Then
                rispostaAlg.RisultatoErrore = "Dati lavorazione non indicati"
                Exit Try
            End If
            If String.IsNullOrEmpty(nomeClasse) Then
                rispostaAlg.RisultatoErrore = "Nome classe non indicato"
                Exit Try
            End If
            If String.IsNullOrEmpty(nomeFunzione) Then
                rispostaAlg.RisultatoErrore = "Nome funzione non indicato"
                Exit Try
            End If

            'Istanzio dinamicamente la classe dell'algoritmo
            Dim parametriAlgoritmo As New ParametriAlgoritmo With
            {
                .Parametri = New List(Of Object) From {objParametri_Server, lavorazione}
            }
            Dim classeAlgoritmo As IAlgoritmoEsterno = IstanziaClasseAlgoritmo(nomeClasse, parametriAlgoritmo)

            'Lancio dinamicamente la funzione della classe algoritmo
            Dim metodoDaLanciare = classeAlgoritmo.GetType().GetMethod(nomeFunzione)
            rispostaAlg = DirectCast(metodoDaLanciare.Invoke(classeAlgoritmo, Nothing), RispostaAlgoritmo)
            rispostaStd.RispostaOK = True

        Catch ex As Exception
            rispostaStd.RispostaOK = False
            rispostaStd.Errore = ex.Message
        Finally
            rispostaStd.RispostaStringa = rispostaAlg
        End Try

        Return rispostaStd

    End Function

    <WebMethod()>
    Public Function LeggiLineeMacchineLavorazione(ByVal objP_server As String,
                                                  ByVal objP_utenti As String,
                                                  piva As String) As RispostaStandard


        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_utenti)

        Try

            Dim dal As New Linee_Macchine_Lavorazione_R

            Dim dt As DataTable = dal.Leggi(piva, String.Empty, String.Empty, String.Empty, String.Empty, objParametri_Server)

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(dt)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod()>
    Public Function CtrUnivocitaIdenLavorazione(ByVal objP_server As String,
                                                ByVal piva As String,
                                                ByVal idAgenda As Integer,
                                                ByVal lavCod As Integer,
                                                ByVal idenLav As String,
                                                ByVal tipoControllo As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

        Try

            If IsNothing(objP_server) Then
                Throw New Exception("Parametri server non indicati")
            End If

            If lavCod = 0 Then
                Throw New Exception("Natura documento non indicata")
            End If

            If String.IsNullOrEmpty(Trim(idenLav)) Then
                r.RispostaStringa = "E' obbligatorio indicare un identificativo lavorazione"
            End If

            If r.RispostaStringa = String.Empty Then

                r.RispostaStringa = ControllaUnivIdenLav(objParametri_Server,
                                                         piva,
                                                         idAgenda,
                                                         lavCod,
                                                         idenLav,
                                                         tipoControllo)

            End If

            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod()>
    Public Function SeCtrUnivocitaIdenLavDaMacch(ByVal objP_server As String,
                                                 ByVal codMacchinaLav As String,
                                                 ByVal piva As String,
                                                 ByVal lavCod As Integer,
                                                 ByVal idenLav As String,
                                                 ByVal tipoControllo As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

        Try

            If IsNothing(objP_server) Then
                Throw New Exception("Parametri server non indicati")
            End If

            If String.IsNullOrEmpty(codMacchinaLav) Then
                Throw New Exception("Codice macchina lavorazione non indicato")
            End If

            If lavCod = 0 Then
                Throw New Exception("Natura documento non indicata")
            End If

            If String.IsNullOrEmpty(Trim(idenLav)) Then
                r.RispostaStringa = "E' obbligatorio indicare un identificativo lavorazione"
            End If

            If r.RispostaStringa = String.Empty Then

                Dim esisteFunzioneLottoTestata = SeEsisteFunzioneLottoTestata(objParametri_Server,
                                                                              codMacchinaLav,
                                                                              piva)

                If esisteFunzioneLottoTestata Then

                    r.RispostaStringa = ControllaUnivIdenLav(objParametri_Server,
                                                             piva,
                                                             0,
                                                             lavCod,
                                                             idenLav,
                                                             tipoControllo)

                End If

            End If

            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod()>
    Public Function LeggiOrdiniLavorazioniColleg(ByVal objP_server As String,
                                                 ByVal piva As String,
                                                 ByVal lavCod As Integer,
                                                 ByVal idAgenda As Integer,
                                                 ByVal idAgendaColleg As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)

        Try

            'Dati ordine/lavorazione corrente
            Dim filtriAggiuntivi = " AGENDA.PIVA = '{0}' "
            filtriAggiuntivi += "AND AGENDA.LAV_COD = {1} "
            'Escludo ordine/lavorazione corrente
            filtriAggiuntivi += "AND AGENDA.ID_AGENDA <> {2} "
            'Solo ordine/lavorazione aperti o quello già referenziato
            filtriAggiuntivi += "AND (MOVIMENTI.EXTRA_INT = 0 OR AGENDA.ID_AGENDA = {3}) "
            'Solo ordine/lavorazione non referenziato escludendo quello già referenziato
            filtriAggiuntivi += "AND AGENDA.ID_AGENDA NOT IN ("
            filtriAggiuntivi += "SELECT MDRIF.ID_AGENDA_RIF FROM MOV_DETTAGLI_RIFERIMENTI MDRIF "
            filtriAggiuntivi += "WHERE MDRIF.LAV_COD = '{1}' AND MDRIF.ID_MOV = -1 AND MDRIF.ID_MOV_DET = -1 AND MDRIF.CAU_MOV = -1 "
            filtriAggiuntivi += "AND MDRIF.LAV_COD_RIF = '{1}' AND MDRIF.ID_MOV_RIF = -1 AND MDRIF.ID_MOV_DET_RIF = -1 AND MDRIF.CAU_MOV_RIF = -1 "
            filtriAggiuntivi += "AND MDRIF.ID_AGENDA_RIF <> {3} "
            filtriAggiuntivi += ") "
            filtriAggiuntivi = String.Format(filtriAggiuntivi, piva, lavCod, idAgenda, idAgendaColleg)

            Dim dtRidotto = LeggiElencoAgende(piva, lavCod, filtriAggiuntivi, objParametri_Server)

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(dtRidotto)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod()>
    Public Function LeggiOrdineLavorazione(ByVal objP_server As String,
                                           ByVal piva As String,
                                           ByVal idAgenda As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)

        Try

            Dim objLav As New AgronicaCoreContabBIZ.FF_LavorazioneBIZ
            Dim lavorazione = objLav.Leggi_Lavorazioni(piva, idAgenda,
                                                       0, "", "",
                                                       Nothing, Nothing,
                                                       "T", False,
                                                       objParametri_Server, LavCod:=LAVCOD_TESTATE_ORDINE_LAVORAZIONE)

            r.RispostaOK = True
            r.RispostaStringa = lavorazione

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod()>
    Public Function LeggiElencoOrdiniLavorazione(ByVal objP_server As String,
                                                 ByVal piva As String,
                                                 ByVal lavCod As Integer,
                                                 ByVal idAgenda As Integer,
                                                 ByVal idAgendaColleg As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)

        Try

            'Dati lavorazione corrente
            Dim filtriAggiuntivi = " AGENDA.PIVA = '{0}' "
            filtriAggiuntivi += "AND AGENDA.LAV_COD = {1} "
            'Solo ordini aperti o quello già referenziato
            filtriAggiuntivi += "AND (MOVIMENTI.EXTRA_INT = 0 OR AGENDA.ID_AGENDA = {3}) "
            filtriAggiuntivi = String.Format(filtriAggiuntivi, piva, lavCod, idAgenda, idAgendaColleg)

            Dim dtRidotto = LeggiElencoAgende(piva, lavCod, filtriAggiuntivi, objParametri_Server)

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(dtRidotto)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

#End Region

#Region "Metodi Interni"
    Private Function IstanziaClasseAlgoritmo(ByVal nomeClasse As String,
                                             ByVal parametri As ParametriAlgoritmo) As IAlgoritmoEsterno

        Dim classeAlgoritmi As IAlgoritmoEsterno = Nothing

        'Determino nome assembly dalla prima parte del nome classe
        Dim nomeAssembly = nomeClasse.Split(".")(0)

        'Cerco nell'assembly in esecuzione quello relativo alla classe che devo istanziare
        Dim nomeAssReferenz = Assembly.GetExecutingAssembly.GetReferencedAssemblies().FirstOrDefault(Function(a) a.Name.Equals(nomeAssembly))

        'Carico l'assembly referenziato
        Dim AssReferenz = Assembly.Load(nomeAssReferenz)

        'Istanzio dinamicamente la classe
        classeAlgoritmi = Activator.CreateInstance(AssReferenz.GetType(nomeClasse), (New List(Of Object) From {parametri}).ToArray)

        Return classeAlgoritmi

    End Function

    Private Function ControllaUnivIdenLav(objParametri_Server As AgronicaCoreParametri,
                                          piva As String,
                                          idAgenda As Integer,
                                          lavCod As String,
                                          idenLav As String,
                                          tipoControllo As String) As String

        Dim esitoControllo As String = ""

        Dim ObjMovDet As New Movimenti_Dettagli_R

        Dim filtriAggiuntivi As String = OttieniFiltroLotto(idenLav, lavCod, idAgenda, tipoControllo)

        Dim dtMovDet As DataTable = ObjMovDet.Leggi(piva, 0, 0, 0, 0,
                                                    0, 0, 0, CAU_CARICO, 0,
                                                    0, 0, 0, 0, 0,
                                                    enumSelezioneVariabile.Selezione_JoinCompleta,
                                                    filtriAggiuntivi, String.Empty, objParametri_Server)

        If dtMovDet.Rows.Count > 0 Then
            Dim descrizione As String = dtMovDet.Rows(0).Item("des_lib")
            Dim dataMov As Date = dtMovDet.Rows(0).Item("data_movimento")
            esitoControllo = "Identificativo lavorazione già utilizzato in " & descrizione & " del " & dataMov.ToString("dd/MM/yyyy")
        End If

        Return esitoControllo

    End Function

    Private Function OttieniFiltroLotto(lotto As String, lavCod As String, idAgenda As Integer, tipoControllo As String) As String

        Dim filtriAggiuntivi As String

        filtriAggiuntivi = " AGENDA.LAV_COD = {0} "
        filtriAggiuntivi += "AND MOVIMENTI_DETTAGLI.LOTTO = '{1}' "
        filtriAggiuntivi += "AND MOVIMENTI_DETTAGLI.JOLLY_INT = 1 "
        filtriAggiuntivi += "AND MOVIMENTI_DETTAGLI.EXTRA_INT > 0 "
        filtriAggiuntivi += "AND MOVIMENTI_DETTAGLI.EXTRA_STR <> '' "
        filtriAggiuntivi = String.Format(filtriAggiuntivi, lavCod, lotto)

        If idAgenda <> 0 Then
            filtriAggiuntivi += "AND AGENDA.ID_AGENDA <> {0} "
            filtriAggiuntivi = String.Format(filtriAggiuntivi, idAgenda)
        End If

        If tipoControllo = enum_CtrUnivIdenLav.UnivocoSoloDocAperti Then
            filtriAggiuntivi += "AND AGENDA.ID_AGENDA IN ( "
            filtriAggiuntivi += "SELECT MOV2.ID_AGENDA FROM MOVIMENTI MOV2 "
            filtriAggiuntivi += "WHERE MOV2.ID_AGENDA = AGENDA.ID_AGENDA AND MOV2.CAU_MOV = {0} AND MOV2.EXTRA_INT = 0 "
            filtriAggiuntivi += ") "
            filtriAggiuntivi = String.Format(filtriAggiuntivi, CAU_LINEA_PRODUZIONE)
        End If

        Return filtriAggiuntivi
    End Function

    Private Function SeEsisteFunzioneLottoTestata(objParametri_Server As AgronicaCoreParametri,
                                                  codMacchinaLav As String,
                                                  piva As String)

        Dim objLineeMacchLav As New Linee_Macchine_Lavorazione_R

        Dim dtLineeMacchLav As DataTable = objLineeMacchLav.Leggi(piva,
                                                                  codMacchinaLav,
                                                                  "",
                                                                  "",
                                                                  "",
                                                                  objParametri_Server)

        Dim esisteFunzioneLottoTestata As Boolean = False

        If Not IsNothing(dtLineeMacchLav) AndAlso dtLineeMacchLav.Rows.Count = 1 Then

            Dim classeAlg As String = dtLineeMacchLav.Rows(0).Item("nome_classe_algoritmi")

            Dim funzioniAlg As String = dtLineeMacchLav.Rows(0).Item("funzioni_algoritmi")

            If Not String.IsNullOrEmpty(classeAlg) AndAlso Not String.IsNullOrEmpty(funzioniAlg) Then

                Dim elencoFunzioniAlg = funzioniAlg.Split(";")

                For i As Integer = 0 To (elencoFunzioniAlg.Length - 1)

                    Dim funzione = elencoFunzioniAlg(i).Split("=")

                    If funzione(0) = TipoFunzioneAlgoritmoLottoTestata Then

                        esisteFunzioneLottoTestata = True

                        Exit For

                    End If

                Next

            End If

        End If

        Return esisteFunzioneLottoTestata

    End Function

    Private Function LeggiElencoAgende(piva As String,
                                       lavCod As Integer,
                                       filtriAggiuntivi As String,
                                       objParametri_Server As AgronicaCoreParametri
                                       ) As DataTable

        Dim ordinamento = "MOVIMENTI.DATA_MOVIMENTO DESC, AGENDA.DES_LIB"

        Dim dal As New Movimenti_R

        Dim dt As DataTable = dal.Leggi(piva, 0, 0, 0, 0, CAU_LINEA_PRODUZIONE,
                                        enumSelezioneVariabile.Selezione_TabellaDatiMinimi, filtriAggiuntivi,
                                        ordinamento, objParametri_Server)

        Dim dv As New DataView(dt)
        Dim colonneDaRestituire() As String = {"id_agenda", "des_lib", "data_movimento", "extra_int"}

        Dim dtRidotto = dv.ToTable(False, colonneDaRestituire)

        Dim colonnaDescrComposta = "descrizione_composta"
        dtRidotto.Columns.Add(colonnaDescrComposta, GetType(String))

        For indice = 0 To (dtRidotto.Rows.Count - 1)
            Dim descrizioneComposta = "{0} del {1} {2}"
            Dim dataMov As Date = dtRidotto.Rows(indice).Item("data_movimento")
            Dim stato As String
            If dtRidotto.Rows(indice).Item("extra_int") = 0 Then
                stato = ""
            Else
                If lavCod = LAVCOD_TRASFORMAZIONI Then
                    stato = "(CHIUSA) "
                Else
                    stato = "(CHIUSO) "
                End If

            End If
            descrizioneComposta = String.Format(descrizioneComposta,
                                                dtRidotto.Rows(indice).Item("des_lib"),
                                                dataMov.ToString("dd/MM/yyyy"),
                                                stato)
            dtRidotto.Rows(indice).Item(colonnaDescrComposta) = descrizioneComposta
        Next

        Return dtRidotto

    End Function

#End Region

End Class