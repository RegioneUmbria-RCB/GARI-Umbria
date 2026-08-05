Imports System.Web
Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreUtility
Imports AgronicaCoreUtility.CaricaListControl
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreEntityFramework
Imports System.Web.Script.Serialization
Imports AgroAgenda_2010.Resources
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreXML.XML_Stampe
Imports AgronicaCoreModelsSTD.Gis
Imports AgronicaCoreDataProvider.My.Resources
Imports System.Xml
Imports AgronicaCoreModelsSTD.exceptions

Public Class MenuBS_Anagrafica
    Inherits System.Web.UI.Page

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()

    End Sub

    Public PageMode As String


    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_OperazioniAgenda(ByVal ultimaSelezioneAlbero As String)

        Dim result As String() = ultimaSelezioneAlbero.Split("§")
        Dim piva As String = result(1)
        Dim sa_cod As String = result(2)
        Dim campo_cod As String = result(3)
        Dim appezza As String = result(4)
        Dim imp As String = result(5)

        'Dim Part_cod As String = result(6)
        'Dim prov As String = result(7)
        'Dim comune As String = result(8)
        'Dim sezione As String = result(9)
        'Dim foglio As String = result(10)
        'Dim numero As String = result(11)
        'Dim subalterno As String = result(12)

        Dim fabbricato_cod As String = result(14)

        Dim objParametriAgenda As New ParametriAgenda

        objParametriAgenda.Piva = If(piva <> "", piva, "")
        objParametriAgenda.Sa_Cod = If(sa_cod <> "", CInt(sa_cod), 0)
        objParametriAgenda.Campo_Cod = If(campo_cod <> "", CInt(campo_cod), 0)
        objParametriAgenda.Appezza = If(appezza <> "", CInt(appezza), 0)
        objParametriAgenda.Id_Imp = If(imp <> "", CInt(imp), 0)
        objParametriAgenda.Fabbricato = If(fabbricato_cod <> "", CInt(fabbricato_cod), 0)
        'objParametriAgenda.Particelle = If(Part_cod <> "", CInt(Part_cod), 0)

    End Function

    ''' <summary>
    ''' Per il caricamento dell'albero Anagrafico
    ''' </summary>
    ''' <param name="id"></param>
    ''' <param name="PathRoot"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod(EnableSession:=True)>
    Public Shared Function AggiornaDati(ByVal elemento As String, ByVal tipo As Integer) As String

        Select Case elemento

            Case 1
                'azienda
                Return CaricaAzienda().RispostaStringa
            Case 2
                'centro
                Return CaricaCentri().RispostaStringa
            Case 3
                'campo
                Return CaricaCampi().RispostaStringa
            Case 4
                'appezzamento
                Return CaricaAppezzamenti().RispostaStringa
            Case 5
                'impianto
                Return CaricaImpianti().RispostaStringa
            Case 6
                'catasto
                Return CaricaCatasto().RispostaStringa
            Case 7
                'RaggruppamentiStalla
                Return CaricaRaggruppamentiStalla().RispostaStringa
            Case 8
                'Stalla
                Return CaricaStalle().RispostaStringa
            Case 9
                'Zoo
                Return CaricaZoo().RispostaStringa
            Case 10
                'fabbricati
                Return CaricaFabbricati().RispostaStringa
            Case 11
                'contatti
                Return CaricaContatti().RispostaStringa
            Case 12
                'Macchine
                Return CaricaMacchine().RispostaStringa
        End Select
    End Function

    Private Sub CambiaImpresa(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim TargetUrl As String
        Dim Origine As String
        Dim Destinazione As String

        'azzero il filtro scelto x le operazioni multi-aziendali
        Session("VariabiliFiltro") = Nothing

        'Costruisco il link
        Origine = Stringa_Codifica(
                        "../MenuAnagrafica/MenuBS_Anagrafica.aspx",
                        AgroKey_EncoderDecoder, Server)

        Destinazione = Stringa_Codifica(
                        "../MenuAnagrafica/MenuBS_Anagrafica.aspx",
                        AgroKey_EncoderDecoder, Server)


        TargetUrl = "../Filtrino/FiltrinoImprese.aspx" &
                    "?o=" & Origine &
                    "&d=" & Destinazione

        Response.Redirect(TargetUrl)

    End Sub

    ''' <summary>
    ''' Per il caricamento dell'albero Anagrafico
    ''' </summary>
    ''' <param name="id"></param>
    ''' <param name="PathRoot"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod(EnableSession:=True)>
    Public Shared Function settaSa_Cod(ByVal sa_cod As String) As Boolean
        Dim objParametriAgenda As New ParametriAgenda
        objParametriAgenda.Sa_Cod = sa_cod
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function setta_modalita(ByVal modalita As String) As Boolean
        HttpContext.Current.Session("modalita") = modalita
    End Function

    ''' <summary>
    ''' Per il caricamento dell'albero Anagrafico
    ''' </summary>
    ''' <param name="id"></param>
    ''' <param name="PathRoot"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod(EnableSession:=True)>
    Public Shared Function GetNodesAlberoAnagrafe(ByVal id As String,
                                                  ByVal PathRoot As String) As String


        Lingua.Gias_InizializzaCultura_DaSession()

        Dim objAnagraficaDettaglio As New AgronicaControlli_2010.AlberoAnagraficaDettaglio
        'leggo la piva, il sa_cod, la varietà e la specie
        Dim objParametriAgenda As New ParametriAgenda
        'objParametriAgenda.Leggi()
        objAnagraficaDettaglio.Piva = objParametriAgenda.Piva
        objAnagraficaDettaglio.Sa_Cod = objParametriAgenda.Sa_Cod

        objAnagraficaDettaglio.Flag_CatastoAziendale = True

        objAnagraficaDettaglio.Flag_Anagrafica = True
        objAnagraficaDettaglio.Flag_Contatti = True
        objAnagraficaDettaglio.Flag_Fabbricati = True
        objAnagraficaDettaglio.Flag_ParcoMacchine = True
        objAnagraficaDettaglio.Flag_Carica_Primo_Giro = True

        'lasciare false o con le cab si impianta!
        objAnagraficaDettaglio.Flag_Esplodi_Tutto = False

        If Not IsNothing(HttpContext.Current.Session("visualizzaRiferimentoAlfanumericoImpianto")) Then
            objAnagraficaDettaglio.visualizzaRiferimentoAlfanumericoImpianto = HttpContext.Current.Session("visualizzaRiferimentoAlfanumericoImpianto")
        End If

        If Not IsNothing(HttpContext.Current.Session("ordinaDataUltimoImpianto")) Then
            objAnagraficaDettaglio.visualizzaRiferimentoAlfanumericoImpianto = HttpContext.Current.Session("ordinaDataUltimoImpianto")
        End If

        If objParametriAgenda.Veg_Cod.Split("/")(0) <> -1 Then
            objAnagraficaDettaglio.Veg_Cod = objParametriAgenda.Veg_Cod.Split("/")(0)
        End If


        Return objAnagraficaDettaglio.GetNodesAlberoAnagrafe(id, PathRoot)
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function PermessoModificaContatto(ByVal xChiave As String) As RispostaStandard

        'Dim result As String
        Dim result As New RispostaStandard

        Dim objParametriAgenda As New ParametriAgenda
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim xPiva As String = xChiave.Split("_")(0)
        Dim xSa_Cod As String = xChiave.Split("_")(1)
        Dim xCod_Contatto As String = xChiave.Split("_")(2)

        Dim possoModificare As Boolean = False
        Dim strRes As String = ""
        If xSa_Cod <> "-1" Then
            possoModificare = True
        Else
            If xPiva = objParametriAgenda.Piva Then
                possoModificare = True
            Else
                strRes = AgronicaAgenda_2010.ContattoCreatoDaAltraImpresa
            End If
        End If

        result.RispostaOK = True
        result.RispostaConferma = possoModificare
        result.RispostaStringa = strRes

        Return result
    End Function



    <WebMethod(EnableSession:=True)>
    Public Shared Function DeleteElemento(ByVal xTipoNodo As String, ByVal xChiave As String) As RispostaStandard

        'Dim result As String
        Dim result As New RispostaStandard
        Dim risp As Boolean
        Dim Dati As String
        Dim objParametriAgenda As New ParametriAgenda

        Dim objParametri_Server = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        Dim r As New RispostaStandard

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim NoteLog As String = NOTELOG_ANAGRAFE_BOOTSTRAP

        Try

            Select Case xTipoNodo

                Case 1 'Azienda

                    '###############################################################################
                    '################## IMPRESA    ####################################
                    '###############################################################################

                    Dim xPiva As String = xChiave

                    'Dim StrXmlCancella As String
                    'Dim intLastAz As Integer

                    'Dim objAzR As New AgronicaCoreAnagrafeBIZ.Impresa_R
                    'Dim objAzW As New AgronicaCoreAnagrafeBIZ.Impresa_W


                    'StrXmlCancella = objAzR.Impresa_Leggi(piva, _
                    '                                True, True, objParametri_Server)

                    'intLastAz = objAzW.Impresa_Scrivi( _
                    '                                CStr(StrXmlCancella), _
                    '                                piva, _
                    '                                    objParametri_Server, _
                    '                                    HttpContext.Current.Session("ASG_objParametri_Utenti"))

                    Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
                    Dim UtenteAbilitato As Boolean = objPermessi.Controlla_Permessi_Utente(
                                                HttpContext.Current.Session("ASG_Utente_Username"),
                                                HttpContext.Current.Session("ASG_IdServizio"),
                                                enum_Security_Attivita.Anagrafica_Impresa,
                                                enum_Security_Operazione.Modifica,
                                                Date.Now,
                                                "",
                                                HttpContext.Current.Session("ASG_objParametri_Utenti"))

                    If Not UtenteAbilitato Then
                        result.RispostaOK = False
                        result.Errore = AgronicaAgenda_2010.NonSiDisponePermessiCancellareImpresa
                        Return result
                    End If


                    '-------------------------------------------------------------------------
                    'Controllo che l'impresa non sia SuperUser
                    '-------------------------------------------------------------------------
                    If xPiva = HttpContext.Current.Session("ASG_SuperUser_CodFiscale") Then

                        '' AgroMsgBox("Impossibile eliminare l'impresa selezionata!", Page)
                        'Throw New Exception("Impossibile eliminare l'impresa selezionata!")
                        ''Exit Sub

                        result.RispostaOK = False
                        result.Errore = AgronicaAgenda_2010.NonSiDisponePermessiCancellareImpresa
                        Return result

                    End If


                    '-------------------------------------------------------------------------
                    '-------------------------------------------------------------------------
                    '-------------------------------------------------------------------------
                    '  Controllo che l'impresa non abbia:
                    '- FIGLI
                    '- MOVIMENTI D'AGENDA
                    '- CONTATTI
                    '- MACCHINE
                    '- MATERIE PRIME
                    '-------------------------------------------------------------------------
                    '-------------------------------------------------------------------------
                    '-------------------------------------------------------------------------


                    '-------------------------------------------------------------------------
                    'Figli
                    '-------------------------------------------------------------------------

                    Dim objGerachia As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R
                    Dim DTGerarchia As DataTable

                    'Leggo la stringa XML dell'oggetto
                    DTGerarchia = objGerachia.Leggi(
                                            CStr(xPiva),
                                            0,
                                                enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                "",
                                                "",
                                                objParametri_Server)

                    objGerachia = Nothing

                    If DTGerarchia.Rows.Count > 0 Then

                        'Throw New Exception("Impossibile eliminare l'impresa poichè esistono delle imprese ad essa associate!" & Chr(13) & _
                        '       " Per poter procedere con l'eliminazione occorre cancellare tutte le imprese!")


                        result.RispostaOK = False
                        result.Errore = String.Format(AgronicaAgenda_2010.MenuBS_Anagrafica_DeleteElemento_ImpossibileEliminareImpresaMadre, Chr(13))
                        Return result

                    End If



                    '====================================================================================================
                    '------------------------------------------------------------------------
                    'Pulitore Giacenze per Eliminazione Id_Agenda Giacenze Ghost
                    '------------------------------------------------------------------------
                    Dim Dummy As String
                    Dim ObjGiacenze As New AgronicaCoreContabBIZ.Giacenze_W

                    Dummy = ObjGiacenze.Giacenza_Pulizia(CStr(xPiva),
                                                         CInt(0),
                                                         CInt(0),
                                                         CInt(0),
                                                         CInt(0),
                                                         CInt(0),
                                                         CInt(0),
                                                         CInt(0),
                                                         CInt(0),
                                                         "",
                                                         objParametri_Server)

                    ObjGiacenze = Nothing
                    '====================================================================================================


                    '-------------------------------------------------------------------------
                    'Movimenti d'Agenda
                    '-------------------------------------------------------------------------
                    Dim ObjAgenda As New AgronicaCoreContabDAL.Agenda_R
                    Dim DtAgenda As DataTable

                    DtAgenda = ObjAgenda.Leggi(CStr(xPiva),
                                                0,
                                                0,
                                                0,
                                                enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                " (Lav_Cod <> 1008) ",
                                                "",
                                                objParametri_Server)

                    ObjAgenda = Nothing

                    If Not IsNothing(DtAgenda) AndAlso DtAgenda.Rows.Count <> 0 Then

                        result.RispostaOK = False
                        result.Errore = String.Format(AgronicaAgenda_2010.MenuBS_Anagrafica_DeleteElemento_ImpossibileEliminareImpresaConMovimentiAgenda, Chr(13))

                        Return result

                        'result.RispostaOK = False
                        'result.Errore = "Impossibile eliminare l'impresa poichè esistono delle registrazioni ad essa associate!" & Chr(13) & _
                        '            " Per poter procedere con l'eliminazione occorre cancellare i dati tramite il modulo 'Agenda'!"
                        'Return result

                        ' @Paolo: Cancello a cascata tutti i record di Agenda
                        'If DtAgenda.Rows.Count > 0 Then
                        '    Dim ObjAgenda2 As New AgronicaCoreContabDAL.Agenda_W

                        '    For i = 0 To DtAgenda.Rows.Count - 1
                        '        ObjAgenda2.Cancella(DtAgenda.Rows(i).Item("PIVA"), DtAgenda.Rows(i).Item("Sa_Cod"), DtAgenda.Rows(i).Item("Id_Agenda"), "", objParametri_Server)
                        '    Next

                        'End If

                    End If

                    '-------------------------------------------------------------------------
                    'Costi CDG
                    '-------------------------------------------------------------------------
                    Dim objControllo As New AgronicaCoreAnagrafeBIZ.Progetto_W
                    Dim controllo = objControllo.controllo_CdGxEliminazione(Nothing, CStr(xPiva),
                                                                            0, 0, 0, 0,
                                                                            objParametri_Server)

                    objControllo = Nothing

                    If controllo.errore = True Then
                        result.RispostaOK = False
                        result.Errore = String.Format(AgronicaAgenda_2010.MenuBS_Anagrafica_DeleteElemento_ImpossibileEliminareImpresaConCdG, Chr(13))

                        Return result
                    End If

                    '-------------------------------------------------------------------------
                    'Contatti
                    '-------------------------------------------------------------------------

                    Dim objRisUm As New AgronicaCoreAnagrafeDAL.Risorse_Umane_R
                    Dim dtRisUm As DataTable
                    Dim Cod_RisUm As Integer
                    Dim Filtro As String

                    '=========================================================================
                    'Contatti creati sotto questa impresa: verificare se sono usati in operazioni
                    '=========================================================================

                    'cod_contatto diverso da se stesso -> lo controllo dopo
                    'Filtro = " Risorse_Umane.Cod_Contatto <> '" & SQL_SaveText(xPiva) & "'"
                    Filtro = " Risorse_Umane.Cod_Contatto <> '" & CStr(xPiva) & "'"

                    'cerco tutti i contatti creati sotto questa piva
                    dtRisUm = objRisUm.Leggi(xPiva,
                                            "",
                                            0, 0, 0, "", False,
                                             False,
                                             Filtro,
                                             " Risorse_Umane.Cod_Contatto ",
                                             objParametri_Server)

                    If Not IsNothing(dtRisUm) AndAlso dtRisUm.Rows.Count > 0 Then

                        'result.RispostaOK = False
                        'result.Errore = "Impossibile eliminare l'impresa poichè esistono dei contatti ad essa associati." & Chr(13) & _
                        '             " Per poter procedere con l'eliminazione occorre prima cancellare questi contatti."
                        'Return result

                        '@Paolo elimino a cascata tutti i Contatti
                        If dtRisUm.Rows.Count > 0 Then
                            Dim objRisUm2 As New AgronicaCoreAnagrafeDAL.Risorse_Umane_W

                            For i = 0 To dtRisUm.Rows.Count - 1
                                objRisUm2.Cancella(dtRisUm.Rows(i).Item("Cod_RisUm"), dtRisUm.Rows(i).Item("Cod_Contatto"), dtRisUm.Rows(i).Item("Cod_Rapporto"), "", objParametri_Server)
                            Next

                        End If

                    End If


                    Dim Messaggio As String = ""
                    '=========================================================================
                    'Contatto-Impresa: impresa in anagrafica gias --> verificare le operazioni
                    '=========================================================================

                    'il contatto impresa è creato sotto la piva del superuser
                    'ma in archivi vecchi la piva potrebbe essere la stessa dell'impresa
                    Filtro = " ( Risorse_Umane.Piva = '" & CStr(objParametri_Server.PivaSuperUser) & "'" &
                                 " OR Risorse_Umane.Piva= '" & CStr(xPiva) & "' ) "

                    dtRisUm = objRisUm.Leggi("",
                                            xPiva,
                                            0, 0, 0, "", False,
                                             False,
                                             Filtro,
                                             "",
                                             objParametri_Server)

                    If Not IsNothing(dtRisUm) AndAlso dtRisUm.Rows.Count > 0 Then
                        'l'impresa è salvata correttamente anche come contatto
                        '---> verificare se esistono operazioni

                        Dim ObjMovNC As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
                        Dim DtNonContabili As DataTable

                        Dim ObjMovC As New AgronicaCoreContabDAL.Movimenti_R
                        Dim DtContabili As DataTable

                        Dim iRis As Integer
                        For iRis = 0 To dtRisUm.Rows.Count - 1

                            Cod_RisUm = dtRisUm.Rows(iRis).Item("Cod_RisUm")

                            '--------------------------
                            'Movimenti Non Contabili 
                            '--------------------------

                            DtNonContabili = ObjMovNC.Leggi("", 0, 0, 0, 0, 0, 0,
                                                      Cod_RisUm,
                                                      "", 0, 0, 0, 0, 0, 0,
                                                        enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                        " (Elem_Cod = 0 ) ",
                                                        "",
                                                        objParametri_Server)

                            If DtNonContabili.Rows.Count <> 0 Then

                                'result.RispostaOK = False
                                'result.Errore = "Esistono operazioni di agenda che riguardano il contatto-impresa." & _
                                '            "Andare nella gestione contatti, cercare l'impresa e visualizzare i movimenti." & Chr(13)
                                'Return result



                                '@Paolo elimino a cascata tutti i record
                                If DtNonContabili.Rows.Count > 0 Then
                                    Dim ObjMovNC2 As New AgronicaCoreContabDAL.Movimenti_Dettagli_W

                                    For i = 0 To DtNonContabili.Rows.Count - 1
                                        ObjMovNC2.Cancella(DtNonContabili.Rows(i).Item("Piva"), DtNonContabili.Rows(i).Item("Sa_Cod"), DtNonContabili.Rows(i).Item("Id_Agenda"), DtNonContabili.Rows(i).Item("Id_Mov"), DtNonContabili.Rows(i).Item("Id_Mov_Det"), "", objParametri_Server)
                                    Next

                                End If

                            Else
                                '----------------------------
                                ' Movimenti Contabili
                                '----------------------------
                                DtContabili = ObjMovC.Leggi("",
                                                         0, 0, 0,
                                                        Cod_RisUm,
                                                         "",
                                                        enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                        "",
                                                        "",
                                                        objParametri_Server)


                                If DtContabili.Rows.Count <> 0 Then

                                    'result.RispostaOK = False
                                    'result.Errore = "Esistono documenti contabili che riguardano il contatto-impresa." & _
                                    '        "Andare nella gestione contatti, cercare l'impresa e visualizzare i movimenti." & Chr(13)
                                    'Return result

                                    '@Paolo elimino a cascata tutti i record
                                    If DtContabili.Rows.Count > 0 Then
                                        Dim ObjMovC2 As New AgronicaCoreContabDAL.Movimenti_W

                                        For i = 0 To DtContabili.Rows.Count - 1
                                            ObjMovC2.Cancella(DtContabili.Rows(i).Item("Piva"), DtContabili.Rows(i).Item("Sa_Cod"), DtContabili.Rows(i).Item("Id_Agenda"), DtContabili.Rows(i).Item("Id_Mov"), "", objParametri_Server)
                                        Next

                                    End If

                                End If

                            End If

                        Next 'x ogni rapporto contabile associato al contatto

                        If Messaggio <> "" Then
                            'Throw New Exception("Impossibile eliminare l'impresa selezionata!" & Chr(13) & _
                            '                    Messaggio & vbCrLf + _
                            '                    "Per poter procedere con l'eliminazione occorre prima cancellare questi movimenti!")

                            result.RispostaOK = False
                            result.Errore = String.Format(AgronicaAgenda_2010.MenuBS_Anagrafica_DeleteElemento_ImpossibileEliminareImpresaConMovimenti, vbCrLf, Messaggio)
                            Return result

                        Else

                            'posso cancellare il contatto-impresa

                            Dim objContatti_R As New AgronicaCoreAnagrafeBIZ.Contatti_MultiHost_R

                            Dati = objContatti_R.Contatto_Leggi(objParametri_Server.PivaSuperUser,
                                                                CStr(xPiva),
                                                                "",
                                                                True,
                                                                objParametri_Server)

                            objContatti_R = Nothing

                            If Dati <> "" Then

                                Dim objContatti_W As New AgronicaCoreAnagrafeBIZ.Contatti_W

                                risp = objContatti_W.Contatto_Scrivi(Dati,
                                                                Nothing,
                                                                Nothing,
                                                                objParametri_Server)

                            End If

                        End If

                    End If



                    '-------------------------------------------------------------------------
                    'Macchine
                    '-------------------------------------------------------------------------

                    Dim ObjMacchine As New AgronicaCoreContabDAL.Parco_Macchine_R
                    Dim DtMacchine As DataTable

                    'modifica del 02/04/2013 (comple maga)
                    'la query restituiva anche le macchine pubbliche, quindi l'azienda non si riusciva più a cancellare
                    'DtMacchine = ObjMacchine.LeggiParcoMacchinexSuperUser(CStr(xPiva), _
                    '                                                     0, 0, "", True, _
                    '                                                    enumSelezioneVariabile.Selezione_JoinDescrizioni, _
                    '                                                    "", _
                    '                                                    "", _
                    '                                                    objParametri_Server)
                    DtMacchine = ObjMacchine.Leggi2(CStr(xPiva),
                                                    0,
                                                    SACOD_NOFILTRO,
                                                    0, "",
                                                    True,
                                                    "", "",
                                                    objParametri_Server)

                    ObjMacchine = Nothing

                    If Not IsNothing(DtMacchine) AndAlso DtMacchine.Rows.Count <> 0 Then

                        'result.RispostaOK = False
                        'result.Errore = "Impossibile eliminare l'impresa poichè esistono delle macchine ad essa associate!" & Chr(13) & _
                        '             " Per poter procedere con l'eliminazione occorre cancellare le macchine!"
                        'Return result

                        '@Paolo elimino a cascata tutti i record
                        If DtMacchine.Rows.Count > 0 Then
                            Dim ObjMacchine2 As New AgronicaCoreContabDAL.Parco_Macchine_W

                            For i = 0 To DtMacchine.Rows.Count - 1
                                ObjMacchine2.Cancella(DtMacchine.Rows(i).Item("Piva"), DtMacchine.Rows(i).Item("Mac_Cod"), "", "", objParametri_Server)
                            Next

                        End If

                    End If

                    '-------------------------------------------------------------------------
                    'Materie Prime
                    '-------------------------------------------------------------------------

                    Dim ObjMateriePrime As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
                    Dim DtMateriePrime As DataTable

                    'Leggo le materie prime     
                    DtMateriePrime = ObjMateriePrime.Leggi(CStr(xPiva),
                                                            0, 0, 0, "", 0, 0, 0, 0, 0, 0, 0, "", 0, "",
                                                            True,
                                                            True,
                                                            "",
                                                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                            "",
                                                            "",
                                                            objParametri_Server)

                    ObjMateriePrime = Nothing

                    If Not IsNothing(DtMateriePrime) AndAlso DtMateriePrime.Rows.Count <> 0 Then

                        'result.RispostaOK = False
                        'result.Errore = "Impossibile eliminare l'impresa poichè esistono delle materie prime ad essa associate!" & Chr(13) & _
                        '          " Per poter procedere con l'eliminazione occorre cancellare le materie prime!"
                        'Return result

                        '@Paolo elimino a cascata tutti i record
                        If DtMateriePrime.Rows.Count > 0 Then
                            Dim ObjMateriePrime2 As New AgronicaCoreAnagrafeDAL.Materie_Prime_W

                            For i = 0 To DtMateriePrime.Rows.Count - 1
                                ObjMateriePrime2.Cancella(DtMateriePrime.Rows(i).Item("Elem_Cod"), DtMateriePrime.Rows(i).Item("Mat_Cod"), DtMateriePrime.Rows(i).Item("Piva"), "", objParametri_Server)
                            Next

                        End If

                    End If


                    '-------------------------------------------------------------------------
                    'Planning
                    '-------------------------------------------------------------------------

                    Dim ObjPlanning As New AgronicaCoreAnagrafeDAL.Programmazione_Testata_R
                    Dim DtPlanning As DataTable

                    DtPlanning = ObjPlanning.Leggi("",
                                                   0,
                                                   CStr(xPiva),
                                                   "", 1,
                                                   AGRODATAINIZIO, AGRODATAFINE,
                                                    enum_TipoRicetta.Standard,
                                                    enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                   "",
                                                   "",
                                                   objParametri_Server)

                    ObjPlanning = Nothing

                    If Not IsNothing(DtPlanning) AndAlso DtPlanning.Rows.Count > 0 Then

                        'result.RispostaOK = False
                        'result.Errore = "Impossibile eliminare l'impresa poichè esistono delle Pianificazioni ad essa associate!" & Chr(13) & _
                        '          " Per poter procedere con l'eliminazione occorre cancellare le Pianificazioni!"
                        'Return result

                        '@Paolo elimino a cascata tutti i record
                        If DtPlanning.Rows.Count > 0 Then
                            Dim ObjPlanning2 As New AgronicaCoreAnagrafeDAL.Programmazione_Testata_W

                            For i = 0 To DtPlanning.Rows.Count - 1
                                ObjPlanning2.Cancella(DtPlanning.Rows(i).Item("Programmazione_Cod"), "", objParametri_Server)
                            Next

                        End If


                    End If


                    '-------------------------------------------------------------------------
                    'PUA
                    '-------------------------------------------------------------------------

                    Dim ObjPua As New AgronicaCorePUA_DAL.PUA_Testata_R
                    Dim DtPua As DataTable

                    DtPua = ObjPua.Leggi(0, 0,
                                        CStr(xPiva),
                                        AGRODATAINIZIO, AGRODATAFINE,
                                        "",
                                        "",
                                        objParametri_Server)

                    ObjPua = Nothing

                    If Not IsNothing(DtPua) AndAlso DtPua.Rows.Count > 0 Then

                        result.RispostaOK = False
                        result.Errore = String.Format(AgronicaAgenda_2010.MenuBS_Anagrafica_DeleteElemento_ImpossibileEliminareImpresaConPUA, Chr(13))

                        Return result

                    End If

                    '-------------------------------------------------------------------------
                    'Audit_Interviste
                    '-------------------------------------------------------------------------

                    Dim ObjAuditInterviste As New AgronicaCoreAuditDAL.Audit_Interviste_R
                    Dim DtAuditInterviste As DataTable

                    DtAuditInterviste = ObjAuditInterviste.Leggi(0,
                                        0, 0,
                                        CStr(xPiva),
                                        AGRODATAINIZIO, AGRODATAFINE,
                                        "",
                                        "",
                                        objParametri_Server)

                    ObjAuditInterviste = Nothing

                    If Not IsNothing(DtAuditInterviste) AndAlso DtAuditInterviste.Rows.Count > 0 Then

                        result.RispostaOK = False
                        result.Errore = String.Format(AgronicaAgenda_2010.MenuBS_Anagrafica_DeleteElemento_ImpossibileEliminareImpresaConCheckList, Chr(13))

                        Return result

                    End If

                    '-------------------------------------------------------------------------
                    'Audit
                    '-------------------------------------------------------------------------

                    Dim ObjAudit As New AgronicaCoreAuditDAL.Audit_R
                    Dim DtAudit As DataTable

                    DtAudit = ObjAudit.Leggi(0,
                                        0, 0,
                                        CStr(xPiva),
                                        AGRODATAINIZIO, AGRODATAFINE,
                                        "",
                                        "",
                                        objParametri_Server)

                    ObjAudit = Nothing

                    If Not IsNothing(DtAudit) AndAlso DtAudit.Rows.Count > 0 Then
                        'Throw New Exception("Impossibile eliminare l'impresa poichè esistono delle check-list ad essa associate!" & Chr(13) & _
                        '          " Per poter procedere con l'eliminazione occorre cancellare le check-list!")

                        result.RispostaOK = False
                        result.Errore = String.Format(AgronicaAgenda_2010.MenuBS_Anagrafica_DeleteElemento_ImpossibileEliminareImpresaConCheckList, Chr(13))
                        Return result
                    End If


                    '-------------------------------------------------------------------------
                    'In caso i controlli abbiano esito negativo elimino l'impresa
                    '-------------------------------------------------------------------------

                    Dim ObjImpresaR As New AgronicaCoreAnagrafeBIZ.Impresa_R
                    Dim ObjImpresaW As New AgronicaCoreAnagrafeBIZ.Impresa_W

                    'Leggo la stringa XML dell'oggetto
                    Dati = ObjImpresaR.Impresa_Leggi(CStr(xPiva),
                                                        True,
                                                        True,
                                                        objParametri_Server)

                    ObjImpresaR = Nothing

                    If Dati <> "" Then
                        'Cancello l'elemento
                        risp = ObjImpresaW.Impresa_Scrivi(Dati,
                                                            Nothing,
                                                            objParametri_Server,
                                                            HttpContext.Current.Session("ASG_objParametri_Utenti"))

                        ObjImpresaW = Nothing
                    End If

                    '#############################################################################################################




                    result.RispostaOK = True
                    result.RispostaStringa = AgronicaAgenda_2010.AziendaCorrettamenteCancellata

                    ImpostaObjP_Agenda(CInt(xTipoNodo), xChiave, True)

                    Return result


                Case 2 'Centro

                    '###############################################################################
                    '################## CENTRO AZIENDALE    ####################################
                    '###############################################################################

                    Dim xPiva As String = xChiave.Split("_")(0)
                    Dim xSa_Cod As String = xChiave.Split("_")(1)

                    'Dim StrXmlCancella As String
                    'Dim intLastCentro As Integer

                    'Dim objCentroR As New AgronicaCoreAnagrafeBIZ.CentroAziendale_R
                    'Dim objCentroW As New AgronicaCoreAnagrafeBIZ.CentroAziendale_W

                    'StrXmlCancella = objCentroR.CentroAziendale_Leggi(piva, _
                    '                                sa_cod, _
                    '                                True, True, "", "", objParametri_Server)

                    'intLastCentro = objCentroW.CentroAziendale_Scrivi( _
                    '                                CStr(StrXmlCancella), _
                    '                                piva, _
                    '                                sa_cod, _
                    '                                    objParametri_Server, _
                    '                                    HttpContext.Current.Session("ASG_objParametri_Utenti"))

                    Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
                    Dim UtenteAbilitato As Boolean = objPermessi.Controlla_Permessi_Utente(
                                                HttpContext.Current.Session("ASG_Utente_Username"),
                                                HttpContext.Current.Session("ASG_IdServizio"),
                                                enum_Security_Attivita.Anagrafica_CentroAziendale,
                                                enum_Security_Operazione.Modifica,
                                                Date.Now,
                                                "",
                                                HttpContext.Current.Session("ASG_objParametri_Utenti"))

                    If Not UtenteAbilitato Then
                        result.RispostaOK = False
                        result.Errore = AgronicaAgenda_2010.NonSiDisponePermessiCancellazioneCentroAziendale
                        Return result
                    End If


                    '====================================================================================================
                    '------------------------------------------------------------------------
                    'Pulitore Giacenze per Eliminazione Id_Agenda Giacenze Ghost
                    '------------------------------------------------------------------------

                    Dim ObjGiacenze As New AgronicaCoreContabBIZ.Giacenze_W
                    Dim Dummy As String

                    Dummy = ObjGiacenze.Giacenza_Pulizia(CStr(xPiva),
                                                         CInt(xSa_Cod),
                                                         CInt(0),
                                                         CInt(0),
                                                         CInt(0),
                                                         CInt(0),
                                                         CInt(0),
                                                         CInt(0),
                                                         CInt(0),
                                                         "",
                                                         objParametri_Server)

                    ObjGiacenze = Nothing



                    '-------------------------------------------------------------------------
                    'Controllo che il centro non abbia:
                    'Movimenti d'Agenda

                    Dim objAgenda As New AgronicaCoreContabDAL.Agenda_R
                    Dim DtAgenda As DataTable

                    DtAgenda = objAgenda.Leggi(CStr(xPiva),
                                                CInt(xSa_Cod),
                                                0,
                                                0,
                                                enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                " Lav_Cod <> 1008 ",
                                                "",
                                                objParametri_Server)

                    objAgenda = Nothing

                    If Not IsNothing(DtAgenda) AndAlso DtAgenda.Rows.Count <> 0 Then

                        result.RispostaOK = False
                        result.Errore = String.Format(AgronicaAgenda_2010.MenuBS_Anagrafica_DeleteElemento_ImpossibileEliminareCentroAziendaleConRegistrazioni, Chr(13))

                        Return result

                    End If

                    '---------------------------------------------------------------------
                    'controllo se ci sono giacenze sul centro

                    Dim objMov_Destinazioni_R As New AgronicaCoreContabDAL.Mov_Destinazioni_R

                    Dim DtMov_Destinazioni As DataTable
                    DtMov_Destinazioni = objMov_Destinazioni_R.Leggi(xPiva, CInt(xSa_Cod), 0, 0, 0, 0, 0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                    If Not IsNothing(DtMov_Destinazioni) AndAlso DtMov_Destinazioni.Rows.Count <> 0 Then

                        result.RispostaOK = False
                        result.Errore = String.Format(AgronicaAgenda_2010.MenuBS_Anagrafica_DeleteElemento_ImpossibileEliminareCentroAziendaleMagazzino, Chr(13))

                        Return result

                    End If

                    '-------------------------------------------------------------------------
                    'Costi CDG
                    '-------------------------------------------------------------------------
                    Dim objControllo As New AgronicaCoreAnagrafeBIZ.Progetto_W
                    Dim controllo = objControllo.controllo_CdGxEliminazione(Nothing, CStr(xPiva),
                                                                            CInt(xSa_Cod), 0, 0, 0,
                                                                            objParametri_Server)

                    objControllo = Nothing

                    If controllo.errore = True Then
                        result.RispostaOK = False
                        result.Errore = String.Format(AgronicaAgenda_2010.MenuBS_Anagrafica_DeleteElemento_ImpossibileEliminareCentroConCdG, Chr(13))

                        Return result
                    End If


                    '-------------------------------------------------------------------------
                    '-------------------------------------------------------------------------
                    'Cancella il centro aziendale

                    Dim objCentroR As New AgronicaCoreAnagrafeBIZ.CentroAziendale_R
                    Dim objCentroW As New AgronicaCoreAnagrafeBIZ.CentroAziendale_W

                    'Leggo la stringa XML dell'oggetto
                    Dati = objCentroR.CentroAziendale_Leggi(
                                            CStr(xPiva),
                                            CInt(xSa_Cod),
                                            CBool(True),
                                            CBool(True),
                                            CDate(HttpContext.Current.Session("ASG_FinestraTemporale_Inizio").ToString),
                                            CDate(HttpContext.Current.Session("ASG_FinestraTemporale_Fine").ToString),
                                            objParametri_Server)

                    objCentroR = Nothing

                    If Dati <> "" Then
                        'Cancello l'elemento
                        objCentroW.CentroAziendale_Scrivi(
                                                        CStr(Dati),
                                                        Nothing,
                                                        Nothing,
                                                        objParametri_Server,
                                                        HttpContext.Current.Session("ASG_objParametri_Utenti"))

                        objCentroW = Nothing
                    End If

                    '######################################################################################



                    result.RispostaOK = True
                    result.RispostaStringa = AgronicaAgenda_2010.CentroAziendaleCorrettamenteCancellato

                    ImpostaObjP_Agenda(xTipoNodo, xChiave, True)

                    Return result

                Case 3 'Campo

                    '###############################################################################
                    '################## CAMPO   ####################################
                    '###############################################################################

                    Dim xPiva As String = xChiave.Split("_")(0)
                    Dim xSa_Cod As String = xChiave.Split("_")(1)
                    Dim xCampo_Cod As String = xChiave.Split("_")(2)

                    'Dim StrXmlCancella As String
                    'Dim intLastCampo As Integer

                    'Dim objCampoR As New AgronicaCoreAnagrafeBIZ.Campo_R
                    'Dim objCampoW As New AgronicaCoreAnagrafeBIZ.Campo_W

                    'StrXmlCancella = objCampoR.Campo_Leggi(piva, _
                    '                                sa_cod, _
                    '                                campo_cod, "", "", True, True, objParametri_Server)

                    'intLastCampo = objCampoW.Campo_Scrivi( _
                    '                                CStr(StrXmlCancella), _
                    '                                piva, _
                    '                                sa_cod, _
                    '                                 campo_cod, _
                    '                                 True,
                    '                                    objParametri_Server, _
                    '                                    HttpContext.Current.Session("ASG_objParametri_Utenti"))




                    Dim strDummy As String


                    Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
                    Dim UtenteAbilitato As Boolean = objPermessi.Controlla_Permessi_Utente(
                                                HttpContext.Current.Session("ASG_Utente_Username"),
                                                HttpContext.Current.Session("ASG_IdServizio"),
                                                enum_Security_Attivita.Anagrafica_Campo,
                                                enum_Security_Operazione.Modifica,
                                                Date.Now,
                                                "",
                                                HttpContext.Current.Session("ASG_objParametri_Utenti"))

                    If Not UtenteAbilitato Then
                        result.RispostaOK = False
                        result.Errore = AgronicaAgenda_2010.NonSiDisponePermessiCancellazioneCentroAziendale
                        Return result
                    End If


                    'controllo che non ci siano registrazioni di agenda riferite al campo


                    'Leggo tutti gli appezzamenti del campo
                    Dim ObjAppezza As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
                    Dim ObjAgenda As New AgronicaCoreContabDAL.Mov_Destinazioni_R
                    Dim ObjRicette As New AgronicaCoreContabDAL.Ricette_Destinazioni_R
                    Dim DtAppezza As DataTable
                    Dim DtAgenda As DataTable
                    Dim DtRicette As DataTable
                    Dim TrovateOperazioni As Boolean = False
                    Dim TrovateRicette As Boolean = False
                    Dim TrovatiCdG As Boolean = False

                    DtAppezza = ObjAppezza.Leggi(CStr(xPiva),
                                               CInt(xSa_Cod),
                                                0,
                                               enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                CStr(" Campo_Cod = " & CStr(xCampo_Cod)),
                                                "",
                                                objParametri_Server)

                    If Not IsNothing(DtAppezza) AndAlso DtAppezza.Rows.Count <> 0 Then

                        'Dim Dr() As DataRow
                        'Dr = DtAppezza.Select(" Campo_Cod = " & xCampo_Cod)

                        'se ci sono appezzamenti associati a quel campo
                        'controllo che nn ci siano operazioni d'agenda sopra...
                        Dim iApp As Integer
                        For iApp = 0 To DtAppezza.Rows.Count - 1

                            'Do While Not iApp < Dr.Length

                            DtAgenda = ObjAgenda.Leggi(CStr(xPiva),
                                                       CInt(xSa_Cod),
                                                       0,
                                                       0,
                                                       0,
                                                       CInt(DtAppezza.Rows(iApp).Item("Appezza")),
                                                       0,
                                                       0,
                                                       enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                       "",
                                                       "",
                                                       objParametri_Server)

                            If DtAgenda.Rows.Count <> 0 Then
                                TrovateOperazioni = True
                                Exit For
                            End If

                            DtRicette = ObjRicette.Leggi(0, 0, 0, 0, 0,
                                                         CStr(xPiva),
                                                         CInt(xSa_Cod),
                                                         CInt(DtAppezza.Rows(iApp).Item("Appezza")),
                                                         0,
                                                         AGRODATAINIZIO,
                                                         AGRODATAFINE,
                                                         enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                         "",
                                                         "",
                                                         objParametri_Server)

                            If DtRicette.Rows.Count <> 0 Then
                                TrovateRicette = True
                                Exit For
                            End If


                            '-------------------------------------------------------------------------
                            'Costi CDG
                            '-------------------------------------------------------------------------
                            Dim objControllo As New AgronicaCoreAnagrafeBIZ.Progetto_W
                            Dim controllo = objControllo.controllo_CdGxEliminazione(Nothing, CStr(xPiva),
                                                                                    CInt(xSa_Cod),
                                                                                    CInt(DtAppezza.Rows(iApp).Item("Appezza")), 0, 0,
                                                                                    objParametri_Server)

                            objControllo = Nothing

                            If controllo.errore = True Then
                                TrovatiCdG = True
                                Exit For
                            End If
                        Next

                    End If

                    '-------------------------------------------------------
                    '-------------------------------------------------------

                    'se non ci sono appezzamenti associati a quel campo
                    'o non ci sono operazioni sopra ai suoi appezzamenti
                    'lo elimino
                    If TrovateOperazioni = False And TrovateRicette = False And TrovatiCdG = False Then

                        Dim objCampoR As New AgronicaCoreAnagrafeBIZ.Campo_R
                        Dim objCampoW As New AgronicaCoreAnagrafeBIZ.Campo_W

                        'Leggo la stringa XML dell'oggetto
                        Dati = objCampoR.Campo_Leggi(
                                                CStr(xPiva),
                                                CInt(xSa_Cod),
                                                CInt(xCampo_Cod),
                                                CDate(HttpContext.Current.Session("ASG_FinestraTemporale_Inizio").ToString),
                                                CDate(HttpContext.Current.Session("ASG_FinestraTemporale_Fine").ToString),
                                                CBool(True),
                                                CBool(True),
                                                objParametri_Server)

                        objCampoR = Nothing

                        If Dati <> "" Then
                            'Cancello l'elemento
                            objCampoW.Campo_Scrivi(
                                                    CStr(Dati),
                                                    Nothing,
                                                    Nothing,
                                                    Nothing,
                                                    CBool(False),
                                                    objParametri_Server,
                                                    HttpContext.Current.Session("ASG_objParametri_Utenti"))

                            objCampoW = Nothing
                        End If

                    Else
                        'Throw New Exception("Impossibile eliminare il campo poichè esistono delle registrazioni ad esso associate!" & Chr(13) & _
                        '    " Per poter procedere con l'eliminazione occorre cancellare i dati tramite il modulo 'Agenda'!")

                        result.RispostaOK = False
                        result.Errore = String.Format(AgronicaAgenda_2010.MenuBS_Anagrafica_DeleteElemento_ImpossibileEliminareCampoConRegistrazioni, Chr(13))
                        Return result
                    End If

                    '######################################################################################



                    result.RispostaOK = True
                    result.RispostaStringa = AgronicaAgenda_2010.CampoCorrettamenteCancellato

                    ImpostaObjP_Agenda(xTipoNodo, xChiave, True)

                    Return result



                Case 4 'Appezzamenti

                    '###############################################################################
                    '################## APPEZZAMENTO   ####################################
                    '###############################################################################

                    Dim xPiva As String = xChiave.Split("_")(0)
                    Dim xSa_Cod As String = xChiave.Split("_")(1)
                    Dim xAppezza As String = xChiave.Split("_")(2)

                    'Dim StrXmlCancella As String
                    'Dim intLastAppe As Integer
                    'Dim objAppezzamentoR As New AgronicaCoreAnagrafeBIZ.Appezzamento_R
                    'Dim objAppezzamentoW As New AgronicaCoreAnagrafeBIZ.Appezzamento_W 'Object  'Agro_Anagrafe.Appezzamento_W


                    'StrXmlCancella = objAppezzamentoR.Appezzamento_Leggi(piva, _
                    '                                sa_cod, _
                    '                                0, appezza, _
                    '                                 True, True, False, False, objParametri_Server)
                    'intLastAppe = objAppezzamentoW.Appezzamento_Scrivi( _
                    '                                CStr(StrXmlCancella), _
                    '                                piva, _
                    '                                sa_cod, _
                    '                                appezza, _
                    '                                    objParametri_Server, _
                    '                                    HttpContext.Current.Session("ASG_objParametri_Utenti"))
                    '-----------------

                    Dim objAppezzamentoW As New AgronicaCoreAnagrafeBIZ.Appezzamento_W

                    result = objAppezzamentoW.Cancella(xPiva, xSa_Cod, xAppezza,
                        HttpContext.Current.Session("ASG_Utente_Username"),
                        HttpContext.Current.Session("ASG_IdServizio"),
                        objParametri_Server, HttpContext.Current.Session("ASG_objParametri_Utenti"))

                    ImpostaObjP_Agenda(xTipoNodo, xChiave, True)

                Case 5 'Impianto

                    '###############################################################################
                    '################## IMPIANTO   ####################################
                    '###############################################################################

                    Dim xPiva As String = xChiave.Split("_")(0)
                    Dim xSa_Cod As String = xChiave.Split("_")(1)
                    Dim xAppezza As String = xChiave.Split("_")(2)
                    Dim xID_Imp As String = xChiave.Split("_")(3)

                    'Dim StrXmlCancella As String
                    'Dim intLastImp As Integer
                    'Dim objImpiantoR As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_R
                    'Dim objImpiantoW As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_W

                    'StrXmlCancella = objImpiantoR.Reg_Impianto_Leggi(piva, _
                    '                                sa_cod, _
                    '                             appezza, id_reg, _
                    '                                 True, True, objParametri_Server)

                    'intLastImp = objImpiantoW.Reg_Impianto_Scrivi(CStr(StrXmlCancella), piva, sa_cod, appezza, id_reg, "", objParametri_Server)

                    Dim objImpiantoW As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_W

                    result = objImpiantoW.Cancella(xPiva, xSa_Cod, xAppezza, xID_Imp,
                        HttpContext.Current.Session("ASG_Utente_Username"),
                        HttpContext.Current.Session("ASG_IdServizio"),
                        objParametri_Server, HttpContext.Current.Session("ASG_objParametri_Utenti"),
                        NoteLog:=NoteLog)

                    ImpostaObjP_Agenda(xTipoNodo, xChiave, True)

                '########################################################################
                Case 7
                    'Raggruppamento Stalla
                    Dim objRaggruppamenti_W As New AgronicaCoreAnagrafeDAL.Stalla_Raggruppamenti_W
                    Dim gefutils As New Gias_EF_Utility
                    Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
                    Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
                    Dim chiaveArr = xChiave.Split("_")
                    Dim Piva = chiaveArr(0)
                    Dim Sa_Cod = CInt(chiaveArr(1))
                    Dim Sta_Num = CInt(chiaveArr(2))
                    Dim Raggruppamento_Cod = CInt(chiaveArr(3))

                    '-------------------------------------------------------------------------
                    'Verifico la presenza di movimenti nel box da cancellare
                    '-------------------------------------------------------------------------
                    Dim ObjMovDestinazioni As New AgronicaCoreContabDAL.Mov_Destinazioni_R
                    Dim DtMovDestinazioni As DataTable

                    DtMovDestinazioni = ObjMovDestinazioni.Leggi(Piva,
                                                Sa_Cod,
                                                0,
                                                0,
                                                0,
                                                0,
                                                Raggruppamento_Cod,
                                                TIPO_DESTINAZIONE_RAGGRUPPAMENTO_STALLA,
                                                enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                "",
                                                "",
                                                objParametri_Server)

                    ObjMovDestinazioni = Nothing

                    If Not IsNothing(DtMovDestinazioni) AndAlso DtMovDestinazioni.Rows.Count <> 0 Then

                        result.RispostaOK = False
                        result.Errore = String.Format(AgronicaAgenda_2010.MenuBS_Anagrafica_DeleteElemento_ImpossibileEliminareRaggruppamentoConMovimenti, Chr(13))

                        Return result
                    End If

                    objRaggruppamenti_W.Cancella(Piva, Sa_Cod, Sta_Num, Raggruppamento_Cod, GiasContext, objParametri_Server)

                    result.RispostaOK = True
                    result.RispostaStringa = "Gruppo cancellato correttamente"

                Case 10 'Particella

                    Dim xPiva As String = xChiave.Split("_")(0)
                    Dim xSa_Cod As String = xChiave.Split("_")(1)
                    Dim xPROV As String = xChiave.Split("_")(2)
                    Dim xCOM As String = xChiave.Split("_")(3)
                    Dim xSezione As String = xChiave.Split("_")(4)
                    Dim xFoglio As String = xChiave.Split("_")(5)
                    Dim xNumero As String = xChiave.Split("_")(6)
                    Dim xSubalterno As String = xChiave.Split("_")(7)

                    'Codice fiscale del Utente
                    Dim UtenteCF = HttpContext.Current.Session("ASG_Utente_CodFiscale").ToString

                    'Codice fiscale del Utente-Profilo
                    Dim UtenteProfiloCF = HttpContext.Current.Session("ASG_SuperUser_CodFiscale").ToString

                    Dim eseguitaOperazione As Boolean
                    Dim MessaggioErrore As String
                    Cancellazione_PARTICELLA(UtenteCF, UtenteProfiloCF, xPiva, xSa_Cod, xPROV, xCOM, xSezione, xFoglio, xNumero, xSubalterno, eseguitaOperazione, MessaggioErrore, objParametri_Server)

                    If (eseguitaOperazione And MessaggioErrore = "") Then
                        ImpostaObjP_Agenda(xTipoNodo, xChiave, True)
                        result.RispostaOK = True
                        result.RispostaStringa = AgronicaAgenda_2010.ParticellaEliminataCorrettamente
                    Else
                        result.RispostaOK = False
                        result.Errore = MessaggioErrore
                    End If
                '########################################################################

                Case 22 'Fabbricato

                    '###############################################################################
                    '################## FABBRICATO   ####################################
                    '###############################################################################

                    Dim xPiva As String = xChiave.Split("_")(0)
                    Dim xSa_Cod As String = xChiave.Split("_")(1)
                    Dim xFabbricato_Cod As String = xChiave.Split("_")(2)


                    'Dim strDummy As String
                    'Dim UtenteAbilitato As Boolean

                    'UtenteAbilitato = Controlla_Permessi_Utente_2( _
                    '              Server, Session, Page, _
                    '              Session("ASG_Utente_Username"), _
                    '              Session("ASG_IdServizio"), _
                    '              enum_Security_Attivita.Anagrafica_Fabbricato, _
                    '              enum_Security_Operazione.Modifica, _
                    '              strDummy)

                    'If UtenteAbilitato = False Then
                    '    'Throw New Exception("Non si dispone dei permessi per cancellare il fabbricato.")
                    '    AgroMsgBox("Non si dispone dei permessi per cancellare il fabbricato.", Page)
                    '    Exit Function
                    'End If


                    Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
                    Dim UtenteAbilitato As Boolean = objPermessi.Controlla_Permessi_Utente(
                                                HttpContext.Current.Session("ASG_Utente_Username"),
                                                HttpContext.Current.Session("ASG_IdServizio"),
                                                enum_Security_Attivita.Anagrafica_Fabbricato,
                                                enum_Security_Operazione.Modifica,
                                                Date.Now,
                                                "",
                                                HttpContext.Current.Session("ASG_objParametri_Utenti"))

                    If Not UtenteAbilitato Then
                        result.RispostaOK = False
                        result.Errore = AgronicaAgenda_2010.NonSiDisponePermessiCancellazioneFabbricato
                        Return result
                    End If

                    'Verifico se è un fabbricato di tipo stalla e se esistono movimenti associati ai raggruppamenti della stalla
                    Dim objFabbricato As New AgronicaCoreAnagrafeBIZ.Fabbricato_R

                    Dim CodTipoFabbricato = objFabbricato.LeggiTipoCodiceFabbricato(xPiva, CInt(xSa_Cod), CInt(xFabbricato_Cod), objParametri_Server)

                    If CodTipoFabbricato = TIPO_FABBRICATO_STALLA Then
                        'Verifico la presenza di movimenti nel box da cancellare
                        Dim movimentiPresenti = objFabbricato.PresenzaMovimentiBoxDaStalla(xPiva, CInt(xSa_Cod), CInt(xFabbricato_Cod), objParametri_Server)
                        If movimentiPresenti Then
                            result.RispostaOK = False
                            result.Errore = AgronicaAgenda_2010.MenuBS_Anagrafica_DeleteElemento_ImpossibileEliminareStallaConMovimenti
                            Return result
                        End If
                    End If

                    '====================================================================================================
                    '------------------------------------------------------------------------
                    'Pulitore Giacenze per Eliminazione Id_Agenda Giacenze Ghost
                    '------------------------------------------------------------------------
                    Dim ObjGiacenze As New AgronicaCoreContabBIZ.Giacenze_W

                    Dim Dummy As String = ObjGiacenze.Giacenza_Pulizia(CStr(xPiva),
                                                         CInt(xSa_Cod),
                                                         CInt(0),
                                                         CInt(0),
                                                         CInt(0),
                                                         CInt(0),
                                                         CInt(0),
                                                         CInt(0),
                                                         CInt(0),
                                                         CStr(""),
                                                         objParametri_Server)

                    ObjGiacenze = Nothing
                    '====================================================================================================

                    'controllo se ci sono giacenze
                    'Dim Dt_Giacenze As New DataTable

                    ''legge le giacenze 
                    'Dt_Giacenze = NewCom_Leggi_Giacenze(Server, Session, Page, _
                    '                                    CStr(xPiva), _
                    '                                    CInt(xSa_Cod), _
                    '                                    , , , _
                    '                                    CInt(xFabbricato_Cod), _
                    '                                    MAGAZZINO, _
                    '                                    , , , , , , , , , , , , , , )

                    Dim ObjAgenda As New AgronicaCoreContabDAL.Mov_Destinazioni_R

                    Dim DtAgenda As DataTable = ObjAgenda.Leggi(CStr(xPiva),
                                             CInt(xSa_Cod),
                                             0,
                                             0,
                                             0,
                                             0,
                                             CInt(xFabbricato_Cod),
                                             MAGAZZINO,
                                             enumSelezioneVariabile.Selezione_TabellaCompleta,
                                             "",
                                             "",
                                             objParametri_Server)


                    If Not DtAgenda Is Nothing AndAlso DtAgenda.Rows.Count > 0 Then
                        'Throw New Exception("Impossibile eliminare il fabbricato poichè sono stati inseriti dei movimenti ad esso associati!")
                        result.RispostaOK = False
                        result.Errore = AgronicaAgenda_2010.MenuBS_Anagrafica_DeleteElemento_ImpossibileEliminareFabbricatoConMovimenti
                        Return result
                    End If

                    DtAgenda = Nothing



                    '--------------------------------------------------------------
                    'Cancello l'eventuale Allegati_EntitaxDocumenti
                    Dim objAll As New AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_W
                    objAll.Cancella_Fabbricato(xPiva, xSa_Cod, xFabbricato_Cod, "", objParametri_Server)

                    '--------------------------------------------------------------

                    'Cancella il fabbricato

                    Dim objFabbricatoR As New AgronicaCoreAnagrafeBIZ.Fabbricato_R
                    Dim objFabbricatoW As New AgronicaCoreAnagrafeBIZ.Fabbricato_W

                    'Leggo la stringa XML dell'oggetto
                    Dati = objFabbricatoR.Fabbricato_Leggi(
                                            CStr(xPiva),
                                            CInt(xSa_Cod),
                                            CInt(xFabbricato_Cod),
                                            CBool(True),
                                            objParametri_Server)

                    objFabbricatoR = Nothing

                    If Dati <> "" Then
                        'Cancello l'elemento
                        objFabbricatoW.Fabbricato_Scrivi(
                                                CStr(Dati),
                                                Nothing,
                                                Nothing,
                                                Nothing,
                                                objParametri_Server,
                                                NoteLog:=NoteLog)

                        objFabbricatoW = Nothing
                    End If

                    result.RispostaOK = True
                    result.RispostaStringa = AgronicaAgenda_2010.FabbricatoEliminatoCorrettamente
                '###############################################################################


                Case 36 'Contatto

                    '###############################################################################
                    '################## CONTATTO   ####################################
                    '###############################################################################

                    Dim xPiva As String = xChiave.Split("_")(0)
                    Dim xSa_Cod As String = xChiave.Split("_")(1)
                    Dim xCod_Contatto As String = xChiave.Split("_")(2)

                    Dim msg As String = ""
                    Dim EseguitaOperazione As Boolean = False

                    EliminaContatto(xPiva, xSa_Cod, xCod_Contatto, msg, EseguitaOperazione, objParametri_Server, objParametriAgenda)

                    If Not EseguitaOperazione Then
                        result.RispostaOK = False
                        result.Errore = msg
                        Return result
                    End If
                    '###############################################################################


                    result.RispostaOK = True
                    result.RispostaStringa = AgronicaAgenda_2010.ContattoCorrettamenteCancellato
                    Return result

                Case 12
                    'Macchine

                    '###############################################################################
                    '################## CONTATTO   ####################################
                    '###############################################################################

                    Dim xPiva As String = xChiave.Split("_")(0)
                    Dim xSa_Cod As String = xChiave.Split("_")(1)
                    Dim xMac_Cod As String = xChiave.Split("_")(2)

                    If RegistrazioniParcoMacchine(xMac_Cod, objParametri_Server, xPiva) Then
                        result.RispostaOK = False
                        result.Errore = AgronicaAgenda_2010.MenuBS_Anagrafica_DeleteElemento_ImpossibileCancellareMacchinarioConMovimenti
                        Return result
                    End If


                    'Cancellazione Concessa
                    'Dim ObjDelete_ParcoMacchine As Agro_Contab_AD.Parco_Macchine_W
                    Dim ObjDelete_ParcoMacchine As New AgronicaCoreContabDAL.Parco_Macchine_W  'Object  'new Agro_Contab_AD.Parco_Macchine_W

                    '   *   CreateCANCELLATOObject("Agro_Contab_AD.Parco_Macchine_W")

                    If (ObjDelete_ParcoMacchine.Cancella(xPiva, CInt(xMac_Cod), "", "", objParametri_Server)) Then

                        result.RispostaOK = True
                        result.RispostaStringa = AgronicaAgenda_2010.MacchinaCancellataCorrettamente
                        Return result

                    Else
                        result.RispostaOK = False
                        result.Errore = AgronicaAgenda_2010.ImpossibileCancellareIlDato
                        Return result
                    End If



            End Select

        Catch ex As Exception

            result.RispostaOK = False
            result.Errore = ex.Message

        End Try

        Return result
    End Function


    Private Shared Function EliminaContatto(Piva As String, Sa_Cod As Integer, Cod_Contatto As String, ByRef Msg As String, ByRef EseguitaOperazione As Boolean, objParametri_Server As AgronicaCoreParametri, objParametri_Agenda As ParametriAgenda)

        Dim ObjMovNC As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
        Dim DtNonContabili As DataTable

        Dim ObjMovC As New AgronicaCoreContabDAL.Movimenti_R
        Dim DtContabili As DataTable

        Dim ObjRisorseUmane As New AgronicaCoreAnagrafeDAL.Risorse_Umane_R
        Dim DtRisUm As DataTable

        Dim objSquadre As New AgronicaCoreContabDAL.CDG_DAL_R
        Dim DtSquadre As DataTable

        Dim Rag_Soc, Descrizione, StringaXML, strDummy As String
        Dim Cod_RisUm As Integer
        Dim isGIAS As Boolean

        Dim Dt As New DataTable
        Dim Dr As DataRow

        Dim i, j As Integer

        Dim esistenzaSquadre As Boolean = False
        Dim listaSquadre As New List(Of String)


        'griglia dei movimenti
        Dt.Columns.Add(New DataColumn("Data", GetType(String)))
        Dt.Columns.Add(New DataColumn("Movimento", GetType(String)))
        Dt.Columns.Add(New DataColumn("Descrizione", GetType(String)))
        Dt.Columns.Add(New DataColumn("Rag_Soc", GetType(String)))

        'se è una partita iva
        If IsNumeric(Cod_Contatto) Then

            'controllo se è un'impresa GIAS
            isGIAS = VerificaEsistenza_PivaGIAS(objParametri_Server, Cod_Contatto)

        Else

            'è una persona fisica
            isGIAS = False

        End If

        If Not isGIAS Then


            '===========================================================================
            'Lettura dei movimenti associati ad una risorsa umana di un contatto
            '---------------------------------------------------------------------------

            'non va bene!!!!!!
            'non cerca per chiave contatto,
            'cerca il cod_contatto 
            ' e piva = oppure contatto pubblico
            'quindi se c'è lo stesso cod-contatto su + imprese, viene letto + volte
            'DtRisUm = ObjRisorseUmane.LeggiContattixSuperUser(CStr(Piva_Impresa), _
            '                                                  0, _
            '                                                  Qs_CodContatto, _
            '                                                  0, 0, "", True, _
            '                                                  AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta, _
            '                                                  "", "", objParametri_Server)

            DtRisUm = ObjRisorseUmane.LeggiSoloContatto(CStr(Piva),
                                                        0,
                                                        Cod_Contatto,
                                                        0, 0, "", True,
                                                        AGRODATAINIZIO, AGRODATAFINE,
                                                        AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                        "", "", objParametri_Server)


            ' Check Preeseistenti
            If Not DtRisUm Is Nothing AndAlso DtRisUm.Rows.Count > 0 Then

                For i = 0 To DtRisUm.Rows.Count - 1

                    Rag_Soc = DtRisUm.Rows(i).Item("Rag_Soc")

                    Cod_RisUm = DtRisUm.Rows(i).Item("Cod_RisUm")


                    '===========================================================================
                    'Lettura dei Movimenti Non Contabili (Lavorazioni su campo..)
                    '---------------------------------------------------------------------------

                    DtNonContabili = ObjMovNC.Leggi("", 0, 0, 0, 0, 0, 0, Cod_RisUm, "", 0, 0, 0,
                                                    0, 0, 0,
                                                    AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta,
                                                    "Elem_Cod = 0", "", objParametri_Server)

                    If Not DtNonContabili Is Nothing AndAlso DtNonContabili.Rows.Count > 0 Then

                        For j = 0 To DtNonContabili.Rows.Count - 1

                            'istanzio una nuova riga
                            Dr = Dt.NewRow

                            Dr.Item("Data") = CDate(DtNonContabili.Rows(j).Item("Data_Movimento")).ToShortDateString

                            Dr.Item("Movimento") = DtNonContabili.Rows(j).Item("Des_Lib")

                            'i18n
                            'Impostazione Descrizione del movimento
                            Select Case DtNonContabili.Rows(j).Item("Cau_Mov")
                                Case enum_Agenda_Causali.IMPUTAZIONE_MANODOPERA
                                    Descrizione = "Manodopera come dipendente" ' per " & RsNonContabili.Fields("Qta").Value ' all'ora o all'ettaro

                                Case enum_Agenda_Causali.IMPUTAZIONE_TERZISTI
                                    Descrizione = "Manodopera come terzista" ' per " & RsNonContabili.Fields("Qta").Value  ' all'ora o all'ettaro

                            End Select


                            Dr.Item("Descrizione") = Descrizione


                            Dr.Item("rag_soc") = DtNonContabili.Rows(j).Item("rag_soc")

                            'aggiungo la riga al datatable
                            Dt.Rows.Add(Dr)

                        Next

                    End If


                    '----------------------------
                    ' MOVIMENTI CONTABILI
                    '----------------------------

                    DtContabili = ObjMovC.Leggi("", 0, 0, 0,
                                                Cod_RisUm,
                                                CAU_REGISTRAZIONI,
                                                AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                "", "", objParametri_Server)


                    If Not DtContabili Is Nothing AndAlso DtContabili.Rows.Count > 0 Then

                        For j = 0 To DtContabili.Rows.Count - 1

                            'istanzio una nuova riga
                            Dr = Dt.NewRow

                            Dr.Item("Data") = CDate(DtContabili.Rows(j).Item("Data_Movimento")).ToShortDateString

                            Dr.Item("Movimento") = DtContabili.Rows(j).Item("Des_Lib")

                            Dr.Item("Descrizione") = DtContabili.Rows(j).Item("Mov_Desc")

                            Dr.Item("rag_soc") = DtContabili.Rows(j).Item("rag_soc")

                            'aggiungo la riga al datatable
                            Dt.Rows.Add(Dr)

                        Next

                    End If


                    '----------------------------
                    ' SQUADRE CDG
                    '----------------------------
                    DtSquadre = objSquadre.Leggi_SquadrexAttvita("", 0, 0, 0, 0, " SquadrexAttivita.cod_risum_list LIKE '" & Agro_SQL_SaveText("%" & Cod_RisUm & "%") & "'", AGRODATAINIZIO, objParametri_Server)


                    For Each squadra In DtSquadre.Rows
                        esistenzaSquadre = True

                        Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
                        Dim impresa_rif As String = objImprese.RagSoc_from_Piva(squadra.item("piva"), objParametri_Server)

                        listaSquadre.Add("- " & impresa_rif & " - " & squadra.item("des_squadra"))

                    Next
                Next


            End If

            Dim possoEliminare As Boolean = True
            If Dt.Rows.Count <> 0 OrElse esistenzaSquadre Then
                possoEliminare = False

                If esistenzaSquadre Then
                    Msg = String.Format(Gias.ImpossibileEliminareContattoXAssegnatoSquadre, Rag_Soc) & ":" & NEWLINE & String.Join(NEWLINE, listaSquadre)
                Else
                    Msg = String.Format(Gias.ContattoEliminaMovimentiCollegati, Rag_Soc, vbCrLf)
                End If

            Else

                ' TODO Check Aggiunti 14/11/2019 GIANLUCA
                Dim objContatti As New AgronicaCoreAnagrafeBIZ.Contatti_MultiHost_R
                Dim dtAltriRiferimenti = objContatti.ContattoRiferimenti(Piva, Cod_Contatto, 0, Sa_Cod, "", objParametri_Server)
                If dtAltriRiferimenti.Rows.Count > 0 Then
                    possoEliminare = False

                    Dim listaImprese = (From imp In dtAltriRiferimenti.AsEnumerable
                                        Select imp.Item("rag_soc")).ToList()

                    Msg = String.Format(Gias.ContattoEliminaUtilizziImprese,
                                        vbCrLf, String.Join(vbCrLf, listaImprese))


                End If

            End If

            If possoEliminare Then

                'CANCELLA IL CONTATTO

                Dim objContatti As New AgronicaCoreAnagrafeBIZ.Contatti_MultiHost_R   'New Agro_Contab.Contatti_MultiHost_R
                '   *   CreateCANCELLATOObject("Agro_Contab.Contatti_MultiHost_R")

                'Leggo la stringa facendomi ritornare Tipooperazione = 3 (x la cancellazione)
                StringaXML = objContatti.Contatto_Leggi(
                                                        Piva,
                                                        Cod_Contatto,
                                                        "",
                                                        True,
                                                        objParametri_Server)


                Dim objContattiCancella As New AgronicaCoreAnagrafeBIZ.Contatti_W 'New Agro_Contab.Contatti_W
                '   *   CreateCANCELLATOObject("Agro_Contab.Contatti_W")


                strDummy = objContattiCancella.Contatto_Scrivi(
                                     CStr(StringaXML),
                                     Nothing,
                                     Nothing,
                                     objParametri_Server, Nothing, NOTELOG_ANAGRAFE_BOOTSTRAP)

                Dim alert_W As New AgronicaCoreScadenziario_BIZ.Alert_W
                alert_W.Cancella_Allegati_Contatto(Piva, Cod_Contatto, objParametri_Server)


                objContattiCancella = Nothing
                objContatti = Nothing

                '************ Agronica_Log_Contatti

                EseguitaOperazione = True


            End If


            ObjMovC = Nothing
            ObjMovNC = Nothing
            DtContabili = Nothing
            DtNonContabili = Nothing
            DtRisUm = Nothing
            ObjRisorseUmane = Nothing


        Else

            Msg = Gias.ContattoImpresaNonCancellabile

        End If

    End Function

    '##############################################################################################
    Private Shared Function RegistrazioniParcoMacchine(ByVal Mac_Cod As Long,
                                                       ByVal agroParam As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                       Optional ByVal Piva As String = "") As Boolean

        Dim Rag_Soc As String
        Dim Mac_Des As String


        Dim Trovato As Boolean = False

        '===========================================================================
        'Lettura dei Movimenti Associati alla Macchina Attrezzatura
        '---------------------------------------------------------------------------
        RegistrazioniParcoMacchine = False

        Dim ObjMacchine As New AgronicaCoreContabDAL.Parco_Macchine_R '   *   CreateCANCELLATOObject("Agro_Contab_AD.Parco_Macchine_R")

        'Leggo le informazioni sul macchinario selezionato			
        Dim dtParcoMacchine As DataTable = ObjMacchine.LeggiParcoMacchinexSuperUser(Piva, Mac_Cod, 0,
                                            "", True,
                                            AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                             "", "", agroParam)


        If dtParcoMacchine.Rows.Count > 0 Then

            Dim drParcoMacchine() As DataRow

            If Piva <> "" Then
                'Considero le sole macchine dell'impresa
                drParcoMacchine = dtParcoMacchine.Select("Piva = '" & Piva & "'")
            Else
                drParcoMacchine = dtParcoMacchine.Select()
            End If

            If drParcoMacchine.Length > 0 Then

                For Each dr As DataRow In drParcoMacchine

                    Dim ObjMov As New AgronicaCoreContabDAL.Movimenti_Dettagli_R '   *   CreateCANCELLATOObject("Agro_Contab_AD.Movimenti_Dettagli_R")

                    Dim dtMov As DataTable = ObjMov.Leggi("", 0, 0, 0, 0, 1, 0, CInt(dr("Mac_Cod")), "", 0, 0, 0, 0, 0, 0,
                                                    AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta,
                                                    "", "", agroParam)

                    If dtMov.Rows.Count > 0 Then

                        'Elimino il Lav_Cod relativo la giacenza
                        If (dtMov.Select("Lav_Cod <> 1008").Length > 0) Then
                            Trovato = True
                        Else
                            Trovato = False
                        End If

                    Else

                        Trovato = False

                    End If

                Next
            End If
        End If


        Return Trovato

    End Function

    '########################################################################################
    Private Shared Function Cancellazione_PARTICELLA(
                                    ByVal UtenteCF As String,
                                    ByVal UtenteProfiloCF As String,
                                    ByVal Piva As String,
                                    ByVal Sa_Cod As Integer,
                                    ByVal Istat_Provincia As String,
                                    ByVal Istat_Comune As String,
                                    ByVal Sezione As String,
                                    ByVal Foglio As Integer,
                                    ByVal Numero As Integer,
                                    ByVal Subalterno As String,
                                    ByRef EseguitaOperazione As Boolean,
                                    ByRef MessaggioErrore As String,
                                    objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri)

        '-------------------------------------------------------
        '----- Dimensiono le variabili
        '-------------------------------------------------------

        Dim XmlDati As String
        Dim Messaggio As String
        Dim StrSQL As String
        Dim IntDummy As Integer


        '--------------------------------------------------------
        '----- Eseguo la cancellazione
        '--------------------------------------------------------



        '============================
        '=====  Aggiornamento  ======
        '============================

        Try

            Dim objParticellaR As New AgronicaCoreAnagrafeBIZ.Particella_R
            Dim objParticellaW As New AgronicaCoreAnagrafeBIZ.Particella_W

            'Leggo la stringa XML dell'oggetto
            XmlDati = objParticellaR.ImpresexParticelle_Leggi(CStr(Piva),
                                                            CInt(Sa_Cod),
                                                            0,
                                                            CStr(Istat_Provincia),
                                                            CStr(Istat_Comune),
                                                            CStr(Sezione),
                                                            CInt(Foglio),
                                                            CInt(Numero),
                                                            CStr(Subalterno),
                                                            CBool(True),
                                                            AGRODATAINIZIO,
                                                            AGRODATAFINE,
                                                            objParametri_Server)

            objParticellaR = Nothing

            'Cancello l'elemento
            objParticellaW.Particella_Scrivi(CStr(XmlDati),
                                             objParametri_Server)


            objParticellaW = Nothing

            '--------------------------

            'Operazione di cancellazione eseguita
            EseguitaOperazione = True

        Catch exc As Exception

            '------------------------------------------------
            'Si e' verificata una eccezione !!!!!!
            '------------------------------------------------

            'Messaggio di errore
            Messaggio = exc.Message.ToString()

            'Faccio abortire la transazione
            'System.EnterpriseServices.ContextUtil.SetAbort()

            'Messaggio di errore ...
            MessaggioErrore = String.Format(AgronicaAgenda_2010.MenuBS_Anagrafica_Cancellazione_PARTICELLA_Errore, Chr(13), Messaggio)

            '------------------------------------------------

        End Try

        '============================
        '===  Fine Aggiornamento  ===
        '============================

    End Function

    Private Sub Particella_Edit_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        'AddHandler CType(Page.Master.FindControl("ImgBtn_AnnullaTutto"), ImageButton).Click, AddressOf Me.AnnullaTutto
        AddHandler CType(Me.Master, AgendaBootstrap).ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto
        AddHandler CType(Me.Master, AgendaBootstrap).ImgBtnFiltro.Click, AddressOf Me.CambiaImpresa

    End Sub
    '##########################################################################################################################################
    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim objParametriAgenda As New ParametriAgenda

        Dim objParametriAgenda_2010 As New AgronicaCoreGestioneRichieste.ParametriAgenda_2010
        objParametriAgenda_2010.Leggi()

        objParametriAgenda_2010.PaginaRichiesta = enum_PagineAgenda_2010.Pagina_Anagrafica_Menu
        objParametriAgenda_2010.PaginaProvenienza = enum_PagineAgronicaSincro.ImportazionePC_Anteprima

        Dim TargetUrl As String

        If Master.flag_MenuBS_2017 Then
            TargetUrl = "../Menu/MenuBS_2017.aspx"
        Else
            TargetUrl = Master.TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAgenda, objParametriAgenda)
        End If

        Response.Redirect(TargetUrl)
    End Sub

    ' Per modifica multipla zoo
    Public objImpianto As New JObject()
    Dim xPiva As String
    Dim xSa_Cod As String
    Dim xAppezza As String
    Dim xId_Imp As String


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        ' Elimino tutte le variabili in Sessione utilizzate nelle pagine interne
        HttpContext.Current.Session("dt_Codici") = Nothing
        HttpContext.Current.Session("dt_Padri") = Nothing
        HttpContext.Current.Session("dt_Rubrica") = Nothing

        ' Setto la visibilità dei bottoni in Master
        Master.flag_pag_Anagrafica = True

        ' Forzo titolo pagina in base ai parametri
        If Master.flag_MenuBS_2017 Then
            Dim IDSezione = enum_Sezioni_MenuBS_2017.Anagrafiche
            Dim visibilita = Request.QueryString("visibilita")
            Dim GruppoEdit = Request.QueryString("GruppoEdit")
            If visibilita = 2 Then
                IDSezione = enum_Sezioni_MenuBS_2017.Anagrafiche_Zoo
            ElseIf visibilita = 3 Then
                IDSezione = enum_Sezioni_MenuBS_2017.MacroCategoria_Magazzini
            ElseIf GruppoEdit = 1 Then
                IDSezione = enum_Sezioni_MenuBS_2017.Configurazione_Stazioni_Meteo_DSS
            End If
            Master.SetTitoloPagina(IDSezione)

            'Se apro il Menu Anagrafica dal Menu Anagrafe forzo la grafica vecchia
            If IDSezione = enum_Sezioni_MenuBS_2017.Anagrafiche Then
                Master.Master_versione = VERSIONE_MASTER_DEFAULT
                Master.Header_versione = VERSIONE_HEADER_DEFAULT
            End If

        End If

        Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        Dim objParametri_Super_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Super_Server"))
        Dim objParametri_Utenti As New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))

        Dim objParametriAgenda As New ParametriAgenda




        Dim FiltroCentri As String = ""
        Dim DtCentriVisibili As DataTable
        Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R

        DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, "", "", objParametri_Server)

        If Not DtCentriVisibili Is Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
            For i = 0 To DtCentriVisibili.Rows.Count - 1
                FiltroCentri &= " (Centri_Aziendali.Piva='" & DtCentriVisibili.Rows(i).Item("Piva") & "' " &
                                " AND Centri_Aziendali.sa_cod=" & DtCentriVisibili.Rows(i).Item("Sa_Cod") & ") OR "
            Next
            If FiltroCentri <> "" Then
                FiltroCentri = " (" & Left(FiltroCentri, FiltroCentri.Length - 3) & ")"
            End If
        End If

        CaricaAlberoCentriDropdown(objParametriAgenda.Piva, objParametri_Server)

        Master.flag_pag_MenuAgenda = True

        '@Paolo
        ' Setto la modalità di base del filtraggio dei contatti (0 = tutti appartenenti all'impresa)
        HttpContext.Current.Session("modalita") = "0"

        Dim Leggi_impostazioni As New Utenti_Impostazioni_Read
        Dim modificaMultipla As String

        If objParametri_Server.SuperUserUsername.ToLower() = objParametri_Server.UtenteUsername.ToLower() Then
            modificaMultipla = "2"
        Else
            modificaMultipla = Leggi_impostazioni.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.SuperUser_Ereditatore, objParametri_Utenti, 2)  'to do...
        End If

        hidden_modificaMultipla.Value = modificaMultipla
        hidden_azienda.Value = objParametriAgenda.Piva
        hidden_sa_cod.Value = objParametriAgenda.Sa_Cod
        hidden_impedisci_eliminazione_contatti.Value = Leggi_impostazioni.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.SUPERUSER_IMPEDISCI_ELIMINAZIONE_CONTATTI_E_RISORSE_UMANE, objParametri_Utenti, 2)

        'Per nuovo albero anagrafico
        AlberoAnagrafica2017.AlberoAnagrafica2017_headerPlaceHeader = gisHeader
        AlberoAnagrafica2017Configura()

        'Impostazione SuperUser x gestione esercizi

        hidden_gest_esercizi.Value = Leggi_impostazioni.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.SuperUser_Gestione_Esercizi, objParametri_Utenti, 2)

        'Impostazione SuperUser x ereditatore
        If objParametri_Server.SuperUserUsername.ToLower() = objParametri_Server.UtenteUsername.ToLower() Then
            hidden_ereditatore.Value = "2"
        Else
            hidden_ereditatore.Value = Leggi_impostazioni.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.SuperUser_Ereditatore, objParametri_Utenti, 2)
        End If

        ' Per modifica multipla zoo

        objParametriAgenda = New ParametriAgenda
        xPiva = objParametriAgenda.Piva
        xSa_Cod = objParametriAgenda.Sa_Cod
        xAppezza = objParametriAgenda.Appezza
        xId_Imp = objParametriAgenda.Id_Imp

        objImpianto.Add(New JProperty("piva", xPiva))
        objImpianto.Add(New JProperty("sa_cod", xSa_Cod))
        objImpianto.Add(New JProperty("appezza", xAppezza))
        objImpianto.Add(New JProperty("id_reg", xId_Imp))

        Dim objCentriAz As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
        objImpianto.Add(New JProperty("sa_nome", objCentriAz.SaNome_from_SaCod(xPiva, xSa_Cod, objParametri_Server)))

    End Sub

    Private Sub CaricaAlberoCentriDropdown(piva As String, objPServer As AgronicaCoreParametri)
        Dim albero As New AgronicaControlli_2010.AlberoAnagrafica2017

        Dim dtAnagrafica = albero.CaricaAlberoCentriDropdown(piva, objPServer)

        AlberoCentriDropdown.DataSource = dtAnagrafica
        AlberoCentriDropdown.DataTextField = "sa_nome"
        AlberoCentriDropdown.DataValueField = "sa_cod"
        AlberoCentriDropdown.DataBind()
        AlberoCentriDropdown.Items.Insert(0, New ListItem("Visualizzazione di tutti i Centri Aziendali", "0"))
    End Sub

    Private Sub AlberoAnagrafica2017Configura()

        'Dim objParametriAgenda As ParametriAgenda = HttpContext.Current.Session("objParametriAgenda")
        Dim objParametriAgenda = New ParametriAgenda
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        'Impostazione SuperUser x albero anagrafiche
        Dim Leggi_impostazioni As New Utenti_Impostazioni_Read
        hidden_config_albero.Value = Leggi_impostazioni.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.SuperUser_Albero_Anagrafiche, objParametri_Utenti, 2)

        Dim permessi = New PermessiUtente()
        Dim cfgAlberoAnagrafica2017 As New ConfigurazioneAlbero
        cfgAlberoAnagrafica2017.Flag_Anagrafica = permessi.getPermesso(enum_Security_Attivita.Anagrafica_Appezzamento).Lettura
        If hidden_config_albero.Value = "2" Then
            cfgAlberoAnagrafica2017.Flag_CatastoAziendale = permessi.getPermesso(enum_Security_Attivita.Anagrafica_ParticellaCatastale).Lettura
        End If
        cfgAlberoAnagrafica2017.Flag_Fabbricati = permessi.getPermesso(enum_Security_Attivita.Anagrafica_Fabbricato).Lettura
        cfgAlberoAnagrafica2017.Flag_ParcoMacchine = permessi.getPermesso(enum_Security_Attivita.Anagrafica_ParcoMacchine).Lettura
        cfgAlberoAnagrafica2017.Flag_Contatti = permessi.getPermesso(enum_Security_Attivita.Anagrafica_Contatto).Lettura

        cfgAlberoAnagrafica2017.Flag_Esplodi_Tutto = True
        cfgAlberoAnagrafica2017.Flag_Appezzamenti_Filtra_Tecnico = False
        cfgAlberoAnagrafica2017.ParametriAgendaData = AGRODATAINIZIO
        cfgAlberoAnagrafica2017.dataInizio = objParametri_Server.FinestraTemporaleInizio
        cfgAlberoAnagrafica2017.dataFine = objParametri_Server.FinestraTemporaleFine
        cfgAlberoAnagrafica2017.Elenco_Icone_SpecieVegetali = HttpContext.Current.Session("Elenco_Icone_SpecieVegetali")
        cfgAlberoAnagrafica2017.Contesto = ConfigurazioneAlbero.enum_Contesto.GIS
        cfgAlberoAnagrafica2017.Flag_CheckBox = False
        'cfgAlberoAnagrafica2017.Flag_Esercizio = True

        cfgAlberoAnagrafica2017.Piva = objParametriAgenda.Piva
        cfgAlberoAnagrafica2017.Sa_Cod = objParametriAgenda.Sa_Cod

        hdAlberoAnagrafica2017cfg.Value = Newtonsoft.Json.JsonConvert.SerializeObject(cfgAlberoAnagrafica2017)
    End Sub

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaAzienda() As RispostaStandard
        Dim r As New RispostaStandard
        Try
            Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
            Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            'Dati Principali
            Dim objParametriAgenda As New ParametriAgenda
            Dim dtAnagrafica As DataTable = objImprese.Leggi_x_anagrafica(objParametriAgenda.Piva, "", "", objParametri_Server)
            dtAnagrafica.Columns.AddRange({New DataColumn("Superficie_Totale", Type.GetType("System.Decimal")),
                                           New DataColumn("Superficie_Tare", Type.GetType("System.Decimal")),
                                           New DataColumn("SAU_Totale", Type.GetType("System.Decimal")),
                                           New DataColumn("Cooperativa_Referente", Type.GetType("System.String"))
                                          })


            Dim objGerarchiaImprese As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R

            For i As Integer = 0 To dtAnagrafica.Rows.Count - 1

                'Recupero le Superfici
                Dim Sup_Totale, Sup_Bosco, Sup_Prati, Sup_Tare, SAU_Totale, SAU_Biologico, SAU_Conversione, SAU_Convenzionale As Double
                objImprese.Recupera_Superfici_Impresa(dtAnagrafica.Rows(i).Item("Piva"),
                                                    Sup_Totale, Sup_Bosco, Sup_Prati, Sup_Tare,
                                                    SAU_Totale, SAU_Biologico, SAU_Conversione, SAU_Convenzionale,
                                                    Date.Today, objParametri_Server)

                dtAnagrafica.Rows(i).Item("Superficie_Totale") = Format(Sup_Totale, "0.0000")
                dtAnagrafica.Rows(i).Item("Superficie_Tare") = Format(Sup_Tare, "0.0000")
                dtAnagrafica.Rows(i).Item("SAU_Totale") = Format(SAU_Totale, "0.0000")



                'Recupero le Cooperative Referenti
                Dim dtGerarchia As DataTable = objGerarchiaImprese.LeggixFiglio(dtAnagrafica.Rows(i).Item("Piva"), AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                Dim Cooperativa_Referente As String = ""
                If Not IsNothing(dtGerarchia) AndAlso dtGerarchia.Rows.Count > 0 Then
                    For Each dr As DataRow In dtGerarchia.Rows
                        If CStr(dr.Item("padre")) <> "" Then
                            Cooperativa_Referente &= objImprese.RagSoc_from_Piva(CStr(dr.Item("padre")), objParametri_Server) & "  "
                        End If
                    Next
                End If
                dtAnagrafica.Rows(i).Item("Cooperativa_Referente") = Cooperativa_Referente.Trim()

            Next

            'creo la lista delle colonne da visualizzare
            Dim l As New List(Of ColonneNome)

            l.Add(New ColonneNome("chiave", "chiave", "string") With {._hidden = True})
            l.Add(New ColonneNome("rag_soc", AgronicaAgenda_2010.RagioneSociale, "string"))
            l.Add(New ColonneNome("partitaIvaReale", AgronicaAgenda_2010.PartitaIVA, "string"))
            l.Add(New ColonneNome("Codice_Fiscale", AgronicaAgenda_2010.CodiceFiscale, "string"))
            l.Add(New ColonneNome("Codice_Cuaa", "CUAA", "string")) 'i18n
            l.Add(New ColonneNome("Codice_Socio", AgronicaAgenda_2010.CodiceSocio, "string") With {._Display = False})
            l.Add(New ColonneNome("Contratto_Produzione", AgronicaAgenda_2010.ContrattoProduzione, "string") With {._Display = False})
            l.Add(New ColonneNome("Indirizzo", AgronicaAgenda_2010.Indirizzo, "string"))
            l.Add(New ColonneNome("Tecnico_Referente", AgronicaAgenda_2010.TecnicoReferente, "string"))
            l.Add(New ColonneNome("Cooperativa_Referente", AgronicaAgenda_2010.CooperativaReferente, "string"))
            l.Add(New ColonneNome("Superficie_Totale", AgronicaAgenda_2010.SuperficieTotale & " [Ha]", "number"))
            l.Add(New ColonneNome("Superficie_Tare", AgronicaAgenda_2010.SuperficieTare & " [Ha]", "number"))
            l.Add(New ColonneNome("SAU_Totale", AgronicaAgenda_2010.SAUTotale & " [Ha]", "number"))

            l.Add(New ColonneNome("Data_Creazione", AgronicaAgenda_2010.DataCreazione, "date"))
            l.Add(New ColonneNome("Utente_Creazione", AgronicaAgenda_2010.UtenteCreazione, "string"))
            l.Add(New ColonneNome("Data_Modifica", AgronicaAgenda_2010.DataModifica, "date"))
            l.Add(New ColonneNome("Utente_Modifica", AgronicaAgenda_2010.UtenteModifica, "string"))

            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            Dim risp As String = js.JSON_DataTable_Kendo(dtAnagrafica, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.Menu)

            r.RispostaOK = True
            r.RispostaStringa = risp

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaCentri() As RispostaStandard
        Dim r As New RispostaStandard
        Try

            Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
            Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Dim objParametriAgenda As New ParametriAgenda

            Dim FinestraTemporaleInizio = objParametri_Server.FinestraTemporaleInizio
            Dim FinestraTemporaleFine = objParametri_Server.FinestraTemporaleFine

            If objParametriAgenda.Data <> AGRODATAINIZIO Then
                objParametri_Server.FinestraTemporaleInizio = objParametriAgenda.Data.Date
                objParametri_Server.FinestraTemporaleFine = objParametriAgenda.Data.Date
            End If

            Dim dtAnagrafica As DataTable = objCentri.Leggi_x_anagrafica(objParametriAgenda.Piva, 0, "", "", objParametri_Server)

            If objParametriAgenda.Data <> AGRODATAINIZIO Then
                objParametri_Server.FinestraTemporaleInizio = objParametriAgenda.Data.Date
                objParametri_Server.FinestraTemporaleFine = objParametriAgenda.Data.Date
            End If

            dtAnagrafica.Columns.AddRange({New DataColumn("Superficie_Totale", Type.GetType("System.Decimal")),
                                       New DataColumn("Superficie_Tare", Type.GetType("System.Decimal")),
                                       New DataColumn("SAU_Totale", Type.GetType("System.Decimal")),
                                       New DataColumn("tipoAttivitaDes", Type.GetType("System.String"))
                                      })


            For i As Integer = 0 To dtAnagrafica.Rows.Count - 1

                'Recupero le Superfici
                Dim Sup_Totale, Sup_Bosco, Sup_Prati, Sup_Tare, SAU_Totale, SAU_Biologico, SAU_Conversione, SAU_Convenzionale As Double
                objCentri.Recupera_Superfici_CentroAziendale(dtAnagrafica.Rows(i).Item("Piva"), dtAnagrafica.Rows(i).Item("sa_cod"),
                                                    Sup_Totale, Sup_Bosco, Sup_Prati, Sup_Tare,
                                                    SAU_Totale, SAU_Biologico, SAU_Conversione, SAU_Convenzionale,
                                                    Date.Today, objParametri_Server)

                Sup_Totale = SAU_Convenzionale + SAU_Biologico + SAU_Conversione
                Sup_Tare = Sup_Totale - SAU_Totale

                dtAnagrafica.Rows(i).Item("Superficie_Totale") = Format(Sup_Totale, "0.0000")
                dtAnagrafica.Rows(i).Item("Superficie_Tare") = Format(Sup_Tare, "0.0000")
                dtAnagrafica.Rows(i).Item("SAU_Totale") = Format(SAU_Totale, "0.0000")



            Next

            'creo la lista delle colonne da visualizzare
            Dim l As New List(Of ColonneNome)

            l.Add(New ColonneNome("chiave", "chiave", "string") With {._hidden = True})
            l.Add(New ColonneNome("piva", "Partita IVA", "string") With {._Display = False})
            l.Add(New ColonneNome("rag_soc", "Ragione Sociale", "string") With {._Display = False})
            l.Add(New ColonneNome("sa_nome", "Nome", "string"))
            l.Add(New ColonneNome("ind_des", "Indirizzo", "string"))
            l.Add(New ColonneNome("com_des", "Comune", "string"))
            l.Add(New ColonneNome("pro_cod", "Provincia", "string"))
            l.Add(New ColonneNome("CAP", "CAP", "string"))
            l.Add(New ColonneNome("Stato", "Stato", "string"))
            Dim c = New ColonneNome("Superficie_Totale", "Superficie Totale [Ha]", "number")
            'c._sum = True
            'c._formatNr = "n4"
            l.Add(c)

            c = New ColonneNome("Superficie_Tare", "Superficie Tare [Ha]", "number")
            'c._sum = True
            'c._formatNr = "n4"
            l.Add(c)

            c = New ColonneNome("SAU_Totale", "Sau Totale [Ha]", "number")
            'c._sum = True
            'c._formatNr = "n4"
            l.Add(c)

            c = New ColonneNome("Validita_Inizio", "Inizio Validità", "date")
            l.Add(c)

            c = New ColonneNome("Validita_Fine", "Fine Validità", "date")
            l.Add(c)

            'l.Add(New ColonneNome("Validita_Inizio", "Valida dal", "date"))
            'l.Add(New ColonneNome("Validita_Fine", "Valida al", "date"))
            'l.Add(New ColonneNome("Data_Modifica", "Ultima Modifica", "date"))
            'l.Add(New ColonneNome("Utente_Modifica", "Utente Modifica", "string"))
            l.Add(New ColonneNome("Data_Creazione", "Data Creazione", "date"))
            l.Add(New ColonneNome("Utente_Creazione", "Utente Creazione", "string"))
            l.Add(New ColonneNome("Data_Modifica", "Data Modifica", "date"))
            l.Add(New ColonneNome("Utente_Modifica", "Utente Modifica", "string"))

            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            Dim risp As String = js.JSON_DataTable_Kendo(dtAnagrafica, l, False, False, TipoFiltroKendo_colonne.CasellaTesto,)
            r.RispostaOK = True
            r.RispostaStringa = risp

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r


    End Function

    Private Shared Sub LeggiCodiciCampi(ByVal Piva As String, ByRef codici As Dictionary(Of String, String), ByRef objParametri_Server As AgronicaCoreParametri)

        Dim objCampoCodici As New AgronicaCoreAnagrafeDAL.Campi_Codici_Read
        Dim dtCodCampo = objCampoCodici.Leggi2(Piva, 0, 0, enum_CodiciAnagrafe.Riferimento_Alfanumerico_Campo, "", AGRODATAINIZIO, AGRODATAFINE, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
        Dim dtSupContratto = objCampoCodici.Leggi2(Piva, 0, 0, enum_CodiciAnagrafe.Sup_Contratto, "", AGRODATAINIZIO, AGRODATAFINE, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
        Dim dtFiliera = objCampoCodici.Leggi2(Piva, 0, 0, enum_CodiciAnagrafe.Filiera, "", AGRODATAINIZIO, AGRODATAFINE, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

        For Each codice In dtCodCampo.Rows
            Dim chiave = "rif_alfanumerico_" & codice("sa_cod") & "_" & codice("campo_cod")
            If Not codici.ContainsKey(chiave) Then
                codici.Add(chiave, codice("val_cod"))
            End If
        Next

        For Each codice In dtSupContratto.Rows
            Dim chiave = "sup_contratto_" & codice("sa_cod") & "_" & codice("campo_cod")
            If Not codici.ContainsKey(chiave) Then
                codici.Add(chiave, codice("val_cod"))
            End If
        Next

        For Each codice In dtFiliera.Rows
            Dim chiave = "filiera_" & codice("sa_cod") & "_" & codice("campo_cod")
            If Not codici.ContainsKey(chiave) Then
                codici.Add(chiave, codice("val_cod"))
            End If
        Next

    End Sub

    Private Shared Function GetCodiceCampo(ByVal codice As String, ByVal sa_cod As Integer, ByVal campo_cod As Integer, ByRef codici As Dictionary(Of String, String)) As String
        Dim chiave As String = codice & "_" & sa_cod & "_" & campo_cod
        Return If(codici.ContainsKey(chiave), codici(chiave), "")
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaCampi() As RispostaStandard
        Dim r As New RispostaStandard
        Try

            Dim objCampi As New AgronicaCoreAnagrafeDAL.Campi_R
            Dim objAppezzamento As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
            Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Dim objParametriAgenda As New ParametriAgenda

            Dim FinestraTemporaleInizio = objParametri_Server.FinestraTemporaleInizio
            Dim FinestraTemporaleFine = objParametri_Server.FinestraTemporaleFine

            If objParametriAgenda.Data <> AGRODATAINIZIO Then
                objParametri_Server.FinestraTemporaleInizio = objParametriAgenda.Data.Date
                objParametri_Server.FinestraTemporaleFine = objParametriAgenda.Data.Date
            End If

            Dim dt As DataTable = objCampi.Leggi_x_anagrafica(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, 0, "", "", objParametri_Server)

            If objParametriAgenda.Data <> AGRODATAINIZIO Then
                objParametri_Server.FinestraTemporaleInizio = objParametriAgenda.Data.Date
                objParametri_Server.FinestraTemporaleFine = objParametriAgenda.Data.Date
            End If

            dt.Columns.AddRange({New DataColumn("Superficie_Totale", Type.GetType("System.Decimal")),
                                       New DataColumn("Superficie_Convenzionale", Type.GetType("System.Decimal")),
                                       New DataColumn("Superficie_Biologico", Type.GetType("System.Decimal")),
                                       New DataColumn("Superficie_Conversione", Type.GetType("System.String")),
                                       New DataColumn("Superficie_Catastale", Type.GetType("System.String"))
                                      })

            ' aggiungo codici campo
            Dim codiciCampo As New Dictionary(Of String, String)
            LeggiCodiciCampi(objParametriAgenda.Piva, codiciCampo, objParametri_Server)
            dt.Columns.Add(New DataColumn("rif_alfanumerico", Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("sup_contratto", Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("filiera", Type.GetType("System.String")))

            For i As Integer = 0 To dt.Rows.Count - 1

                'Recupero le Superfici
                Dim Sup_Totale, Sup_Tare, SAU_Totale, SAU_Biologico, SAU_Conversione, SAU_Convenzionale, SAU_Catastale As Double
                If Not IsDBNull(dt.Rows(i).Item("Piva")) AndAlso dt.Rows(i).Item("Piva") <> "" AndAlso Not IsDBNull(dt.Rows(i).Item("sa_cod")) AndAlso IsNumeric(dt.Rows(i).Item("sa_cod")) Then
                    objAppezzamento.Recupera_Superfici_Campo(dt.Rows(i).Item("Piva"), dt.Rows(i).Item("sa_cod"), dt.Rows(i).Item("Campo_Cod"),
                                                    Sup_Totale, SAU_Biologico, SAU_Conversione, SAU_Convenzionale, SAU_Catastale, "", "", objParametri_Server)

                    dt.Rows(i).Item("Superficie_Totale") = Format(Sup_Totale, "0.0000")
                    dt.Rows(i).Item("Superficie_Biologico") = Format(SAU_Biologico, "0.0000")
                    dt.Rows(i).Item("Superficie_Convenzionale") = Format(SAU_Convenzionale, "0.0000")
                    dt.Rows(i).Item("Superficie_Conversione") = Format(SAU_Conversione, "0.0000")
                    dt.Rows(i).Item("Superficie_Catastale") = Format(SAU_Catastale, "0.0000")

                    dt.Rows(i).Item("rif_alfanumerico") = GetCodiceCampo("rif_alfanumerico", dt.Rows(i).Item("sa_cod"), dt.Rows(i).Item("Campo_Cod"), codiciCampo)
                    dt.Rows(i).Item("sup_contratto") = GetCodiceCampo("sup_contratto", dt.Rows(i).Item("sa_cod"), dt.Rows(i).Item("Campo_Cod"), codiciCampo)
                    dt.Rows(i).Item("filiera") = GetCodiceCampo("filiera", dt.Rows(i).Item("sa_cod"), dt.Rows(i).Item("Campo_Cod"), codiciCampo)

                End If

            Next

            'creo la lista delle colonne da visualizzare
            Dim l As New List(Of ColonneNome)

            l.Add(New ColonneNome("chiave", "Nome", "string") With {._hidden = True})
            'If objParametriAgenda.Sa_Cod = 0 Then
            l.Add(New ColonneNome("sa_nome", "Centro", "string"))
            'End If
            l.Add(New ColonneNome("Campo", "chiave", "string"))
            l.Add(New ColonneNome("Validita_Inizio", "Inizio Validità", "date"))
            l.Add(New ColonneNome("Validita_Fine", "Fine Validità", "date"))
            l.Add(New ColonneNome("Gru_Des", "Gruppo Vegetale", "string"))
            l.Add(New ColonneNome("Veg_Des", "Specie", "string"))
            l.Add(New ColonneNome("Superficie_Totale", "Superficie Totale", "number"))
            l.Add(New ColonneNome("Superficie_Biologico", "Superficie Biologico", "number"))
            l.Add(New ColonneNome("Superficie_Convenzionale", "Superficie Convenzionale", "number"))
            l.Add(New ColonneNome("Superficie_Conversione", "Superficie Conversione", "number"))
            l.Add(New ColonneNome("Superficie_Catastale", "Superficie Catastale", "number"))

            l.Add(New ColonneNome("rif_alfanumerico", "Codice Campo", "string"))
            l.Add(New ColonneNome("sup_contratto", "Sup. Contratto", "string"))
            l.Add(New ColonneNome("filiera", "Filiera", "string"))

            l.Add(New ColonneNome("Data_Creazione", "Data Creazione", "date"))
            l.Add(New ColonneNome("Utente_Creazione", "Utente Creazione", "string"))
            l.Add(New ColonneNome("Data_Modifica", "Data Modifica", "date"))
            l.Add(New ColonneNome("Utente_Modifica", "Utente Modifica", "string"))


            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            Dim risp As String = js.JSON_DataTable_Kendo(dt, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa)
            r.RispostaOK = True
            r.RispostaStringa = risp

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r

    End Function

    Private Shared Sub LeggiCodiciAppezzamenti(ByVal Piva As String, ByRef codici As Dictionary(Of String, String), ByRef objParametri_Server As AgronicaCoreParametri)

        Dim objAppezzamentoCodici As New AgronicaCoreAnagrafeDAL.Appezzamento_Codici_R
        Dim dtCodAppezzamento = objAppezzamentoCodici.Leggi(Piva, 0, 0, enum_CodiciAnagrafe.Riferimento_Alfanumerico_Appezzamento, "", enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
        Dim dtCodBiologico = objAppezzamentoCodici.Leggi(Piva, 0, 0, enum_CodiciAnagrafe.Codice_Appezza_Biologico, "", enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
        Dim dtCodIsola = objAppezzamentoCodici.Leggi(Piva, 0, 0, enum_CodiciAnagrafe.Isola, "", enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
        Dim dtMetodoProduzione = objAppezzamentoCodici.Leggi(Piva, 0, 0, enum_CodiciAnagrafe.MetodoDiProduzione, "", enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)

        For Each codice In dtCodAppezzamento.Rows
            Dim chiave = "rif_alfanumerico_" & codice("sa_cod") & "_" & codice("appezza")
            If Not codici.ContainsKey(chiave) Then
                codici.Add(chiave, codice("val_cod"))
            End If
        Next

        For Each codice In dtCodBiologico.Rows
            Dim chiave = "cod_biologico_" & codice("sa_cod") & "_" & codice("appezza")
            If Not codici.ContainsKey(chiave) Then
                codici.Add(chiave, codice("val_cod"))
            End If
        Next

        For Each codice In dtCodIsola.Rows
            Dim chiave = "isola_" & codice("sa_cod") & "_" & codice("appezza")
            If Not codici.ContainsKey(chiave) Then
                codici.Add(chiave, codice("val_cod"))
            End If
        Next

        For Each codice In dtMetodoProduzione.Rows
            Dim chiave = "metodoproduzione_" & codice("sa_cod") & "_" & codice("appezza")
            If Not codici.ContainsKey(chiave) Then
                codici.Add(chiave, codice("val_cod"))
            End If
        Next

    End Sub

    Private Shared Function GetCodiceAppezzamento(ByVal codice As String, ByVal sa_cod As Integer, ByVal appezza As Integer, ByRef codici As Dictionary(Of String, String)) As String
        Dim chiave As String = codice & "_" & sa_cod & "_" & appezza
        Return If(codici.ContainsKey(chiave), codici(chiave), "")
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaAppezzamenti() As RispostaStandard
        Dim r As New RispostaStandard
        Try
            Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

            Dim objAppezzamento As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
            Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
            Dim objImpreseProgetti As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Dim objParametriAgenda As New ParametriAgenda

            Dim FinestraTemporaleInizio = objParametri_Server.FinestraTemporaleInizio
            Dim FinestraTemporaleFine = objParametri_Server.FinestraTemporaleFine

            If objParametriAgenda.Data <> AGRODATAINIZIO Then
                objParametri_Server.FinestraTemporaleInizio = objParametriAgenda.Data.Date
                objParametri_Server.FinestraTemporaleFine = objParametriAgenda.Data.Date
            End If

            Dim dt As DataTable = objAppezzamento.Leggi_x_anagrafica(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, objParametriAgenda.Campo_Cod, 0, "", "", objParametri_Server)


            If objParametriAgenda.Sa_Cod = "" Then
                objParametriAgenda.Sa_Cod = "0"
            End If

            If objParametriAgenda.Appezza = "" Then
                objParametriAgenda.Appezza = "0"
            End If

            If objParametriAgenda.Id_Imp = "" Then
                objParametriAgenda.Id_Imp = "0"
            End If

            If objParametriAgenda.Campo_Cod = "" Then
                objParametriAgenda.Campo_Cod = "0"
            End If

            Dim DTImpianti = objImpianti.Leggi_x_anagrafica2(objParametriAgenda.Piva,
                                                             objParametriAgenda.Sa_Cod,
                                                             objParametriAgenda.Appezza,
                                                             objParametriAgenda.Id_Imp,
                                                             objParametriAgenda.Campo_Cod,
                                                             "",
                                                             "",
                                                             objParametri_Server)

            Dim dtDistinta = objImpreseProgetti.LeggiDistinta_Attiva_inDataxAnagrafica(objParametriAgenda.Piva,
                                                                  objParametriAgenda.Sa_Cod,
                                                                  objParametriAgenda.Appezza,
                                                                  objParametriAgenda.Id_Imp,
                                                                  objParametriAgenda.Data,
                                                                  enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                  "",
                                                                  " Imprese_Progetti.Validita_Inizio DESC ",
                                                                  objParametri_Server)

            If objParametriAgenda.Data <> AGRODATAINIZIO Then
                objParametri_Server.FinestraTemporaleInizio = FinestraTemporaleInizio
                objParametri_Server.FinestraTemporaleFine = FinestraTemporaleFine
            End If

            dt.Columns.Add(New DataColumn("utilizzo"))
            dt.Columns.Add(New DataColumn("cod_kpin"))
            dt.Columns.Add(New DataColumn("cod_block"))

            For Each row In dt.Rows

                Dim rowsImpianti = DTImpianti.Select(" sa_Cod = " & row("sa_cod") &
                                                " AND Appezza = " & row("Appezza") & " ")

                If rowsImpianti IsNot Nothing AndAlso rowsImpianti.Length > 0 Then
                    Dim strUtilizzo As String = ""
                    For Each rowImpianti In rowsImpianti
                        strUtilizzo &= rowImpianti("utilizzo") & " "
                    Next
                    row("utilizzo") = strUtilizzo
                End If

                'row("utilizzo") = Utilizzi_Da_Appezzamento(row("piva"), row("sa_cod"), row("appezza"), objParametri_Server)

                Dim rowDist = dtDistinta.Select(" sa_Cod = " & row("sa_cod") &
                                                " AND Appezza = " & row("Appezza") & " ")


                If rowDist IsNot Nothing AndAlso rowDist.Length > 0 Then
                    row("cod_kpin") = rowDist(0)("cod_kpin")
                    row("cod_block") = rowDist(0)("cod_block")
                End If

            Next

            dt.Columns.Add(New DataColumn("Selected"))
            'creo la lista delle colonne da visualizzare
            Dim l As New List(Of ColonneNome)

            l.Add(New ColonneNome("chiave", "chiave", "string") With {._hidden = True})

            l.Add(New ColonneNome("Piva", "Piva", "string") With {._hidden = True})
            l.Add(New ColonneNome("Sa_Cod", "Sa_Cod", "number") With {._hidden = True})
            l.Add(New ColonneNome("Appezza", "Appezza", "number") With {._hidden = True})
            l.Add(New ColonneNome("Campo_Cod", "Campo_Cod", "number") With {._hidden = True})
            'If objParametriAgenda.Sa_Cod = 0 Then
            l.Add(New ColonneNome("sa_nome", "Centro", "string"))
            'End If
            'If objParametriAgenda.Campo_Cod = 0 Then
            l.Add(New ColonneNome("Campo_Des", "Campo", "string"))
            'End If
            l.Add(New ColonneNome("app_nome", "Nome", "string"))
            l.Add(New ColonneNome("sup_app", "Sup.", "number"))
            l.Add(New ColonneNome("Data_Modifica", "Ultima Modifica", "date"))
            l.Add(New ColonneNome("Utente_Modifica", "Utente Modifica", "string"))
            l.Add(New ColonneNome("Data_Creazione", "Data Creazione", "date"))
            l.Add(New ColonneNome("Utente_Creazione", "Utente Creazione", "string"))
            l.Add(New ColonneNome("Validita_Inizio", "Valida Dal", "date"))
            l.Add(New ColonneNome("Validita_Fine", "Valida Al", "date"))
            l.Add(New ColonneNome("blk_flag", "blk_flag", "number"))
            l.Add(New ColonneNome("utilizzo", "utilizzo", "string"))
            l.Add(New ColonneNome("rif_alfanumerico", "Rif. Alfanumerico", "string"))
            l.Add(New ColonneNome("isola", "Isola", "string"))
            l.Add(New ColonneNome("cod_biologico", "N. App. Bio", "string"))
            l.Add(New ColonneNome("cod_kpin", "KPIN", "string"))
            l.Add(New ColonneNome("cod_block", "BLOCK", "string"))
            l.Add(New ColonneNome("MetodoProduzione_Des", "Metodo Produzione", "string"))
            'l.Add(New ColonneNome("Selected", "Selected", "string"))

            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            Dim risp As String = js.JSON_DataTable_Kendo(dt, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa)
            r.RispostaOK = True
            r.RispostaStringa = risp

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r

    End Function

    Private Shared Function Utilizzi_Da_Appezzamento(Piva As String, sa_cod As Integer, appezza As Integer, objParametri_Server As AgronicaCoreParametri) As String

        Dim objImpianti_R As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dim dt = objImpianti_R.Leggi(Piva, sa_cod, appezza, 0, enumSelezioneVariabile.Selezione_JoinDescrizioni, "", "", objParametri_Server)
        Dim returnString = ""
        Dim i = 0
        For Each row In dt.Rows
            Dim utilizzo As String = ""
            If row("cul_cod") = 0 Then

                Dim val = objImpianti_R.Codice_Anagrafe_from_PivaSaCodAppezzaIdImp(row("PIVA"), row("SA_COD"), row("APPEZZA"), row("ID_REG"), objParametri_Server)
                If val = "" Then
                    val = "Terreno Nudo"
                End If
                utilizzo = val
            Else
                utilizzo = row("veg_des") & " - " & row("cul_des")
            End If
            If i = 0 Then
                returnString &= utilizzo
            Else
                returnString &= ", " & utilizzo
            End If
        Next

        Return returnString
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Appezzamenti_Catasto(_particella As String) As RispostaStandard
        Dim r As New RispostaStandard

        Try

            Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Dim particella = _particella.Split("_")
            Dim objAppezzamenti As New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R
            Dim dt = objAppezzamenti.AppezzamentixParticelle_Leggi(particella(0), particella(1), 0, particella(2), particella(3), particella(4), particella(5), particella(6), particella(7), enumSelezioneVariabile.Selezione_JoinDescrizioni, "", "", objParametri_Server, False)

            Dim l As New List(Of ColonneNome)

            l.Add(New ColonneNome("Piva", "Piva", "string") With {._hidden = True})
            l.Add(New ColonneNome("Sa_Cod", "Sa_Cod", "number") With {._hidden = True})
            l.Add(New ColonneNome("Appezza", "Appezza", "number") With {._hidden = True})
            l.Add(New ColonneNome("sa_nome", "Centro", "string"))
            l.Add(New ColonneNome("Campo_Des", "Campo", "string"))
            l.Add(New ColonneNome("app_nome", "Appezzamento", "string"))
            l.Add(New ColonneNome("Prov", "Prov", "string"))
            l.Add(New ColonneNome("Comuni_Prov", "Comuni_Prov", "string"))
            l.Add(New ColonneNome("Com", "Com", "string"))
            l.Add(New ColonneNome("Localita", "Localita", "string"))
            l.Add(New ColonneNome("Sezione", "Sezione", "string"))
            l.Add(New ColonneNome("Foglio", "Foglio", "number"))
            l.Add(New ColonneNome("Numero", "Numero", "number"))
            l.Add(New ColonneNome("Subalterno", "Subalterno", "string"))
            l.Add(New ColonneNome("Area", "Area", "number"))
            l.Add(New ColonneNome("sup_app", "Superficie", "number"))
            l.Add(New ColonneNome("Validita_Inizio_App", "Appezzamento Dal", "date"))
            l.Add(New ColonneNome("Validita_Fine_App", "Appezzamento Al", "date"))

            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            Dim risp As String = js.JSON_DataTable_Kendo(dt, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa)

            r.RispostaOK = True
            r.RispostaStringa = risp

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Catasto_Appezzamento(_piva As String, _sa_cod As Integer, _appezza As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Try

            Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Dim objAppezzamenti As New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R
            Dim dt = objAppezzamenti.AppezzamentixParticelle_Leggi(_piva, _sa_cod, _appezza, "", "", "0", 0, 0, "0", enumSelezioneVariabile.Selezione_JoinDescrizioni, "", "", objParametri_Server, False)

            Dim l As New List(Of ColonneNome)

            l.Add(New ColonneNome("Piva", "Piva", "string") With {._hidden = True})
            l.Add(New ColonneNome("Sa_Cod", "Sa_Cod", "number") With {._hidden = True})
            l.Add(New ColonneNome("Appezza", "Appezza", "number") With {._hidden = True})
            l.Add(New ColonneNome("Prov", "Prov", "string"))
            l.Add(New ColonneNome("Comuni_Prov", "Comuni_Prov", "string"))
            l.Add(New ColonneNome("Com", "Com", "string"))
            l.Add(New ColonneNome("Localita", "Localita", "string"))
            l.Add(New ColonneNome("Sezione", "Sezione", "string"))
            l.Add(New ColonneNome("Foglio", "Foglio", "number"))
            l.Add(New ColonneNome("Numero", "Numero", "number"))
            l.Add(New ColonneNome("Subalterno", "Subalterno", "string"))
            l.Add(New ColonneNome("Area", "Area", "number"))

            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            Dim risp As String = js.JSON_DataTable_Kendo(dt, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa)

            r.RispostaOK = True
            r.RispostaStringa = risp

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Catasto_Campo(_piva As String, _sa_cod As Integer, _campo_cod As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Try

            Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Dim objcampi As New AgronicaCoreAnagrafeDAL.CampixParticelle_R
            Dim dt = objcampi.Leggi(_piva, _sa_cod, _campo_cod, "", "", "", 0, 0, "", AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta, "", "", objParametri_Server)

            Dim l As New List(Of ColonneNome)

            l.Add(New ColonneNome("Piva", "Piva", "string") With {._hidden = True})
            l.Add(New ColonneNome("Sa_Cod", "Sa_Cod", "number") With {._hidden = True})
            l.Add(New ColonneNome("Campo_Cod", "Campo_Cod", "number") With {._hidden = True})
            l.Add(New ColonneNome("Prov", "Prov", "string"))
            l.Add(New ColonneNome("Comuni_Prov", "Comuni_Prov", "string"))
            l.Add(New ColonneNome("Com", "Com", "string"))
            l.Add(New ColonneNome("Localita", "Localita", "string"))
            l.Add(New ColonneNome("Sezione", "Sezione", "string"))
            l.Add(New ColonneNome("Foglio", "Foglio", "number"))
            l.Add(New ColonneNome("Numero", "Numero", "number"))
            l.Add(New ColonneNome("Subalterno", "Subalterno", "string"))
            l.Add(New ColonneNome("Area", "Area", "number"))

            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            Dim risp As String = js.JSON_DataTable_Kendo(dt, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa)

            r.RispostaOK = True
            r.RispostaStringa = risp

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaImpianti() As RispostaStandard
        Dim r As New RispostaStandard
        Try

            Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
            Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Dim objParametriAgenda As New ParametriAgenda
            If objParametriAgenda.Appezza = "" Then
                objParametriAgenda.Appezza = 0
            End If

            Dim FinestraTemporaleInizio = objParametri_Server.FinestraTemporaleInizio
            Dim FinestraTemporaleFine = objParametri_Server.FinestraTemporaleFine

            If objParametriAgenda.Data <> AGRODATAINIZIO Then
                objParametri_Server.FinestraTemporaleInizio = objParametriAgenda.Data.Date
                objParametri_Server.FinestraTemporaleFine = objParametriAgenda.Data.Date
            End If

            Dim dt As DataTable = objImpianti.Leggi_x_anagrafica2(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, objParametriAgenda.Appezza, 0, objParametriAgenda.Campo_Cod, "", "", objParametri_Server, Date.Now)

            If objParametriAgenda.Data <> AGRODATAINIZIO Then
                objParametri_Server.FinestraTemporaleInizio = FinestraTemporaleInizio
                objParametri_Server.FinestraTemporaleFine = FinestraTemporaleFine
            End If

            'dt.Columns.Add(New DataColumn("codice_anagrafe"))
            'dt.Columns.Add(New DataColumn("utilizzo"))
            'dt.Columns.Add(New DataColumn("varieta"))
            dt.Columns.Add(New DataColumn("lotto"))
            dt.Columns.Add(New DataColumn("Descrizione"))
            dt.Columns.Add(New DataColumn("regolamento"))
            dt.Columns.Add(New DataColumn("disciplinare"))
            dt.Columns.Add(New DataColumn("stato_impianto"))
            dt.Columns.Add(New DataColumn("stato_impianto_des"))

            dt.Columns.Add(New DataColumn("limite_N"))
            dt.Columns.Add(New DataColumn("limite_P"))
            dt.Columns.Add(New DataColumn("limite_K"))
            dt.Columns.Add(New DataColumn("cod_kpin"))
            dt.Columns.Add(New DataColumn("cod_block"))

            dt.Columns.Add(New DataColumn("piante_ha"))
            dt.Columns.Add(New DataColumn("piante_impianto"))

            'dt.Columns.Add(New DataColumn("CoverB", GetType(Boolean)))
            'dt.Columns.Add(New DataColumn("MonitoratoB", GetType(Boolean)))

            Dim objImpreseProgetti As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
            Dim objGruppoFinalita As New AgronicaCoreMetaSchemaDAL.GruppoFinalita_R
            Dim objReg As New AgronicaCoreMetaSchemaDAL.Regolamenti_R

            'Dim codiciImpianti As New Dictionary(Of String, String)
            'LeggiCodiciImpianti(objParametriAgenda.Piva, codiciImpianti, objParametri_Server)

            'Dim codiciEsercizi As New Dictionary(Of String, String)
            'LeggiCodiciEsercizi(objParametriAgenda.Piva, codiciEsercizi, objParametri_Server)

            If objParametriAgenda.Sa_Cod = "" Then
                objParametriAgenda.Sa_Cod = "0"
            End If

            If objParametriAgenda.Appezza = "" Then
                objParametriAgenda.Appezza = "0"
            End If

            If objParametriAgenda.Id_Imp = "" Then
                objParametriAgenda.Id_Imp = "0"
            End If

            If objParametriAgenda.Campo_Cod = "" Then
                objParametriAgenda.Campo_Cod = "0"
            End If

            Dim dtDistinta = objImpreseProgetti.LeggiDistinta_Attiva_inDataxAnagrafica(objParametriAgenda.Piva,
                                                                  objParametriAgenda.Sa_Cod,
                                                                  objParametriAgenda.Appezza,
                                                                  objParametriAgenda.Id_Imp,
                                                                  objParametriAgenda.Data,
                                                                  enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                  "",
                                                                  " Imprese_Progetti.Validita_Inizio DESC ",
                                                                  objParametri_Server)

            Dim dataRiferimento = AGRODATAINIZIO

            If objParametriAgenda.Data <> AGRODATAINIZIO Then
                dataRiferimento = objParametriAgenda.Data
            End If

            For Each data In dt.Rows

                If data("su_fila_m") <> "" AndAlso data("tra_fila_m") <> "" AndAlso data("germinabilita") <> "" Then
                    CalcolaPiante(CDbl(data("su_fila_m")), CDbl(data("tra_fila_m")), If(data("interbina") <> "", CDbl(data("interbina")), 0), CDbl(data("germinabilita")), data("sup_imp"), data)
                End If

                ' Leggi dati distina attiva alla data corrente
                Dim rowDist As DataRow()

                rowDist = dtDistinta.Select(" sa_Cod = " & data("sa_cod") &
                                                " AND Appezza = " & data("Appezza") &
                                                " AND ID_reg = " & data("ID_reg") & " ")


                If rowDist IsNot Nothing AndAlso rowDist.Length > 0 Then
                    Dim i = 0

                    data("lotto") = rowDist(0)("Progetto_Nome")
                    data("descrizione") = rowDist(0)("Progetto_Des")
                    data("stato_impianto_des") = rowDist(0)("stato_impianto_des")
                    data("cod_kpin") = rowDist(0)("cod_kpin")
                    data("cod_block") = rowDist(0)("cod_block")
                    data("regolamento") = rowDist(0)("regolamento")

                End If

            Next

            'creo la lista delle colonne da visualizzare
            Dim l As New List(Of ColonneNome)

            l.Add(New ColonneNome("chiave", "chiave", "string") With {._hidden = True})
            l.Add(New ColonneNome("Piva", "Piva", "string") With {._hidden = True})
            l.Add(New ColonneNome("Sa_Cod", "Sa_Cod", "number") With {._hidden = True})
            l.Add(New ColonneNome("Campo_Cod", "Campo_Cod", "number") With {._hidden = True})
            l.Add(New ColonneNome("Appezza", "Appezza", "number") With {._hidden = True})
            l.Add(New ColonneNome("Id_Reg", "Id_Reg", "number") With {._hidden = True})
            l.Add(New ColonneNome("veg_cod", "veg_cod", "number") With {._hidden = True})
            l.Add(New ColonneNome("cul_cod", "cul_cod", "number") With {._hidden = True})
            l.Add(New ColonneNome("grfi_cod", "grfi_cod", "number") With {._hidden = True})
            l.Add(New ColonneNome("grva_cod_veg", "grva_cod_veg", "number") With {._hidden = True})

            l.Add(New ColonneNome("Sa_Nome", "Centro", "string"))
            l.Add(New ColonneNome("Campo_Des", "Campo", "string"))
            l.Add(New ColonneNome("app_nome", "Nome Appezzamento", "string"))
            l.Add(New ColonneNome("Codice_Impianto", "Codice Impianto", "string"))
            l.Add(New ColonneNome("utilizzo", "Utilizzo", "string"))
            l.Add(New ColonneNome("varieta", "Varietà", "string"))
            l.Add(New ColonneNome("gru_des", "Gruppo Vegetale", "string"))
            l.Add(New ColonneNome("grfi_des", "Finalità", "string"))
            l.Add(New ColonneNome("grva_des", "Tipologia Varietale", "string"))

            'Superficie formattata
            Dim c = New ColonneNome("sup_imp", "Sup. [Ha]", "number")
            c._formatNr = "n4"
            l.Add(c)


            l.Add(New ColonneNome("Validita_Inizio", "Valida dal", "date"))
            l.Add(New ColonneNome("Validita_Fine", "Valida al", "date"))

            l.Add(New ColonneNome("tra_fila_m", "Tra Fila", "number"))
            l.Add(New ColonneNome("su_fila_m", "Su Fila", "number"))
            l.Add(New ColonneNome("piante_ha", "Piante/Ha", "number"))
            l.Add(New ColonneNome("piante_impianto", "Piante/Impianto", "number"))

            l.Add(New ColonneNome("port_cod", "port_cod", "number"))
            l.Add(New ColonneNome("port_des", "Portinnesto", "string"))

            l.Add(New ColonneNome("foral_cod", "foral_cod", "number"))
            l.Add(New ColonneNome("foral_des", "Forma Allevamento", "string"))

            l.Add(New ColonneNome("cop_cod", "cop_cod", "number"))
            l.Add(New ColonneNome("cop_des", "Copertura", "string"))

            l.Add(New ColonneNome("setup_cod", "Semina/Trapianto", "string"))
            l.Add(New ColonneNome("CoverB", "Cover Crops", "boolean"))
            l.Add(New ColonneNome("MonitoratoB", "Monitorato", "boolean"))

            l.Add(New ColonneNome("lotto", "lotto", "string"))
            l.Add(New ColonneNome("descrizione", "descrizione", "string"))
            l.Add(New ColonneNome("stato_impianto_des", "Stato Impianto", "string"))
            l.Add(New ColonneNome("regolamento", "Regolamento", "string"))
            l.Add(New ColonneNome("cod_kpin", "KPIN", "string"))
            l.Add(New ColonneNome("cod_block", "BLOCK", "string"))

            l.Add(New ColonneNome("Data_Modifica", "Ultima Modifica", "date"))
            l.Add(New ColonneNome("Utente_Modifica", "Utente Modifica", "string"))

            l.Add(New ColonneNome("Data_Creazione", "Data Creazione", "date"))
            l.Add(New ColonneNome("Utente_Creazione", "Utente Creazione", "string"))

            l.Add(New ColonneNome("blk_flag", "blk_flag", "number"))

            l.Add(New ColonneNome("Data_Inizio_Portinnesto", "Data_Inizio_Portinnesto", "date"))


            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            Dim risp As String = js.JSON_DataTable_Kendo(dt, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipoFiltroKendo_colonne.Menu)
            r.RispostaOK = True
            r.RispostaStringa = risp

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    Private Shared Sub LeggiCodiciImpianti(ByVal Piva As String, ByRef codici As Dictionary(Of String, String), ByRef objParametri_Server As AgronicaCoreParametri)

        Dim objImpiantiCodici As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R
        Dim dtTraFila = objImpiantiCodici.Leggi(Piva, 0, 0, 0, "", enum_CodiciAnagrafe.Impianto_TraFila_Maschio, "", enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
        Dim dtSuFila = objImpiantiCodici.Leggi(Piva, 0, 0, 0, "", enum_CodiciAnagrafe.Impianto_SuFila_Maschio, "", enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
        Dim dtInterbina = objImpiantiCodici.Leggi(Piva, 0, 0, 0, "", enum_CodiciAnagrafe.Impianto_Interbina, "", enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
        Dim dtGerminabilita = objImpiantiCodici.Leggi(Piva, 0, 0, 0, "", enum_CodiciAnagrafe.Impianto_Germinabilita, "", enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)

        For Each codice In dtTraFila.Rows
            Dim chiave = "tra_fila_" & codice("sa_cod") & "_" & codice("appezza") & "_" & codice("id_reg")
            If Not codici.ContainsKey(chiave) Then
                codici.Add(chiave, codice("val_cod"))
            End If
        Next

        For Each codice In dtSuFila.Rows
            Dim chiave = "su_fila_" & codice("sa_cod") & "_" & codice("appezza") & "_" & codice("id_reg")
            If Not codici.ContainsKey(chiave) Then
                codici.Add(chiave, codice("val_cod"))
            End If
        Next

        For Each codice In dtInterbina.Rows
            Dim chiave = "interbina_" & codice("sa_cod") & "_" & codice("appezza") & "_" & codice("id_reg")
            If Not codici.ContainsKey(chiave) Then
                codici.Add(chiave, codice("val_cod"))
            End If
        Next

        For Each codice In dtGerminabilita.Rows
            Dim chiave = "germinabilita_" & codice("sa_cod") & "_" & codice("appezza") & "_" & codice("id_reg")
            If Not codici.ContainsKey(chiave) Then
                codici.Add(chiave, codice("val_cod"))
            End If
        Next

    End Sub

    Private Shared Function GetCodiceImpianto(ByVal codice As String, ByVal sa_cod As Integer, ByVal appezza As Integer, ByVal id_reg As Integer, ByRef codici As Dictionary(Of String, String)) As String
        Dim chiave As String = codice & "_" & sa_cod & "_" & appezza & "_" & id_reg
        Return If(codici.ContainsKey(chiave), codici(chiave), "")
    End Function

    Private Shared Function CalcolaPiante(ByVal dist_su As Double, ByVal dist_tra As Double, ByVal interb As Double, ByVal germin As Double, ByVal superficie As Double, ByRef data As Object) As Boolean
        If dist_su > 0 AndAlso dist_tra > 0 AndAlso germin > 0 Then
            ' Dim denominatore = If(interb > 0, (interb / 2) * dist_su, dist_su * dist_tra)
            Dim denominatore = If(interb > 0 AndAlso dist_tra <> interb, Math.Abs(dist_tra - interb) * dist_su, dist_su * dist_tra)
            Dim PianteHa = 10000 / denominatore * (germin / 100)
            Dim PianteImpianto = PianteHa * superficie
            data("piante_ha") = Int(PianteHa)
            data("piante_impianto") = Int(PianteImpianto)
            Return True
        End If
        Return False
    End Function

    Private Shared Sub LeggiCodiciEsercizi(ByVal Piva As String, ByRef codici As Dictionary(Of String, String), ByRef objParametri_Server As AgronicaCoreParametri)

        Dim objImpiantiCodici As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R
        Dim dtCodKPin = objImpiantiCodici.LeggixProgetto(Piva, 0, 0, 0, "", 0, enum_CodiciAnagrafe.Zespri_Codice_kPIN, "", enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
        Dim dtCodBlock = objImpiantiCodici.LeggixProgetto(Piva, 0, 0, 0, "", 0, enum_CodiciAnagrafe.Zespri_Block_Name, "", enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)

        For Each codice In dtCodKPin.Rows
            Dim chiave = "cod_kpin_" & codice("progetto_cod")
            If Not codici.ContainsKey(chiave) Then
                codici.Add(chiave, codice("val_cod"))
            End If
        Next

        For Each codice In dtCodBlock.Rows
            Dim chiave = "cod_block_" & codice("progetto_cod")
            If Not codici.ContainsKey(chiave) Then
                codici.Add(chiave, codice("val_cod"))
            End If
        Next

    End Sub

    Private Shared Function GetCodiceEsercizio(ByVal codice As String, ByVal progetto_cod As Integer, ByRef codici As Dictionary(Of String, String)) As String
        Dim chiave As String = codice & "_" & progetto_cod
        Return If(codici.ContainsKey(chiave), codici(chiave), "")
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaEsercizi() As RispostaStandard

        Dim r As New RispostaStandard
        Try

            Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
            Dim objImprese_Progetti As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
            Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Dim objParametriAgenda As New ParametriAgenda
            If objParametriAgenda.Appezza = "" Then
                objParametriAgenda.Appezza = 0
            End If

            If objParametriAgenda.Id_Imp = "" Then
                objParametriAgenda.Id_Imp = 0
            End If

            Dim dt As New DataTable

#Region "Non usati"
            'dt.Columns.Add("chiave", Type.GetType("System.String"))
            'dt.Columns.Add("Piva", Type.GetType("System.String"))
            'dt.Columns.Add("Sa_Cod", Type.GetType("System.Int32"))
            'dt.Columns.Add("Campo_Cod", Type.GetType("System.Int32"))
            'dt.Columns.Add("Appezza", Type.GetType("System.Int32"))
            'dt.Columns.Add("Id_Reg", Type.GetType("System.Int32"))
            'dt.Columns.Add("Progetto_Cod", Type.GetType("System.Int32"))
            'dt.Columns.Add("Sa_Nome", Type.GetType("System.String"))
            'dt.Columns.Add("Campo_Des", Type.GetType("System.String"))
            'dt.Columns.Add("app_nome", Type.GetType("System.String"))
            'dt.Columns.Add("Codice_Impianto", Type.GetType("System.String"))
            'dt.Columns.Add("progetto_nome", Type.GetType("System.String"))
            'dt.Columns.Add("progetto_des", Type.GetType("System.String"))
            'dt.Columns.Add("cod_kpin", Type.GetType("System.String"))
            'dt.Columns.Add("cod_block", Type.GetType("System.String"))
            ''dt.Columns.Add("stato_impianto", Type.GetType("System.String"))
            'dt.Columns.Add("utilizzo", Type.GetType("System.String"))
            'dt.Columns.Add("gru_des", Type.GetType("System.String"))
            'dt.Columns.Add("veg_cod", Type.GetType("System.Int32"))
            'dt.Columns.Add("cul_cod", Type.GetType("System.Int32"))
            'dt.Columns.Add("grfi_cod", Type.GetType("System.Int32"))
            'dt.Columns.Add("grfi_des", Type.GetType("System.String"))
            'dt.Columns.Add("sup_imp", Type.GetType("System.Decimal"))
            ''dt.Columns.Add("Sup_Prog", Type.GetType("System.Decimal"))
            'dt.Columns.Add("Validita_Inizio", Type.GetType("System.DateTime"))
            'dt.Columns.Add("Validita_Fine", Type.GetType("System.DateTime"))
            'dt.Columns.Add("Distinta_Chiusa", Type.GetType("System.String"))
            'dt.Columns.Add("blk_flag", Type.GetType("System.String"))
            'dt.Columns.Add("Resa", Type.GetType("System.Decimal"))
#End Region

            Dim objImpreseProgetti As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
            Dim objGruppoFinalita As New AgronicaCoreMetaSchemaDAL.GruppoFinalita_R
            Dim objRegImpiantiCodici As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R

            Dim FinestraTemporaleInizio = objParametri_Server.FinestraTemporaleInizio
            Dim FinestraTemporaleFine = objParametri_Server.FinestraTemporaleFine

            If objParametriAgenda.Data <> AGRODATAINIZIO Then
                objParametri_Server.FinestraTemporaleInizio = objParametriAgenda.Data.Date
                objParametri_Server.FinestraTemporaleFine = objParametriAgenda.Data.Date
            End If

            'Dim dtImpianti As DataTable = objImprese_Progetti.Leggi_x_anagrafica(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, objParametriAgenda.Appezza, objParametriAgenda.Id_Imp, objParametriAgenda.Campo_Cod, "", "", objParametri_Server, Date.Now)
            dt = objImprese_Progetti.Leggi_x_anagrafica(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, objParametriAgenda.Appezza, objParametriAgenda.Id_Imp, objParametriAgenda.Campo_Cod, "", "", objParametri_Server, Date.Now)

#Region "Non usati"
            'dtImpianti.Columns.Add(New DataColumn("codice_anagrafe"))
            'dtImpianti.Columns.Add(New DataColumn("utilizzo"))

            'Dim codiciEsercizi As New Dictionary(Of String, String)
            'LeggiCodiciEsercizi(objParametriAgenda.Piva, codiciEsercizi, objParametri_Server)

            'For Each data In dtImpianti.Rows

            '    If data("cul_cod") = 0 Then

            '        Dim val = objImpianti.Codice_Anagrafe_from_PivaSaCodAppezzaIdImp(data("PIVA"), data("SA_COD"), data("APPEZZA"), data("ID_REG"), objParametri_Server)
            '        If val = "" Then
            '            val = "Terreno Nudo"
            '        End If

            '        data("codice_anagrafe") = val
            '        data("utilizzo") = val

            '    Else

            '        data("utilizzo") = data("veg_des") & " - " & data("cul_des")

            '    End If

            '    Dim dtDistinta = objImpreseProgetti.LeggiDistinta(data("piva"),
            '                                                      data("sa_cod"),
            '                                                      data("Appezza"),
            '                                                      data("id_reg"),
            '                                                      "",
            '                                                      enumSelezioneVariabile.Selezione_TabellaCompleta,
            '                                                      "",
            '                                                      " Imprese_Progetti.Validita_Fine DESC ",
            '                                                      objParametri_Server)

            '    For Each distinta In dtDistinta.Rows

            '        Dim Distinta_Chiusa = objRegImpiantiCodici.LeggiValCod_2(distinta("Piva"), 0, 0, 0, distinta("Progetto_Cod"), enum_CodiciAnagrafe.Distinta_Chiusa, False, "", "", objParametri_Server)

            '        Dim dr = dt.NewRow
            '        dr.Item("chiave") = data("chiave") & "_" & distinta("Progetto_Cod")
            '        dr.Item("Piva") = data("Piva")
            '        dr.Item("Sa_Cod") = data("Sa_Cod")
            '        dr.Item("Campo_Cod") = data("Campo_Cod")
            '        dr.Item("Appezza") = data("Appezza")
            '        dr.Item("Id_Reg") = data("Id_Reg")
            '        dr.Item("Progetto_Cod") = distinta("Progetto_Cod")
            '        dr.Item("Sa_Nome") = data("Sa_Nome")
            '        dr.Item("Campo_Des") = data("Campo_Des")
            '        dr.Item("app_nome") = data("app_nome")
            '        dr.Item("Codice_Impianto") = data("Codice_Impianto")
            '        dr.Item("utilizzo") = data("utilizzo")
            '        dr.Item("sup_imp") = data("sup_imp")
            '        dr.Item("gru_des") = data("gru_des")
            '        dr.Item("veg_cod") = data("veg_cod")
            '        dr.Item("cul_cod") = data("cul_cod")
            '        dr.Item("grfi_cod") = data("grfi_cod")
            '        dr.Item("grfi_des") = data("grfi_des")
            '        dr.Item("blk_flag") = data("blk_flag")
            '        dr.Item("Progetto_Nome") = distinta("Progetto_Nome")
            '        dr.Item("Progetto_Des") = distinta("Progetto_Des")
            '        dr.Item("cod_kpin") = GetCodiceEsercizio("cod_kpin", distinta("Progetto_Cod"), codiciEsercizi)
            '        dr.Item("cod_block") = GetCodiceEsercizio("cod_block", distinta("Progetto_Cod"), codiciEsercizi)
            '        'dr.Item("Sup_Prog") = If(distinta("Sup_Prog") = 0, data("sup_imp"), distinta("Sup_Prog"))
            '        'dr.Item("stato_impianto") = objGruppoFinalita.StatoImpianto_from_GrfiCod(distinta("Stato_Impianto"), objParametri_Server)
            '        dr.Item("Distinta_Chiusa") = If(Distinta_Chiusa = "1", "SI", "NO")
            '        dr.Item("Validita_Inizio") = distinta("Validita_Inizio")
            '        dr.Item("Validita_Fine") = distinta("Validita_Fine")
            '        dr.Item("Resa") = distinta("Produzione_Prevista")
            '        dt.Rows.Add(dr)
            '    Next

            'Next

#End Region

            If objParametriAgenda.Data <> AGRODATAINIZIO Then
                objParametri_Server.FinestraTemporaleInizio = FinestraTemporaleInizio
                objParametri_Server.FinestraTemporaleFine = FinestraTemporaleFine
            End If

            'creo la lista delle colonne da visualizzare
            Dim l As New List(Of ColonneNome)
            l.Add(New ColonneNome("chiave", "chiave", "string") With {._hidden = True})
            l.Add(New ColonneNome("Piva", "Piva", "string") With {._hidden = True})
            l.Add(New ColonneNome("Sa_Cod", "Sa_Cod", "number") With {._hidden = True})
            l.Add(New ColonneNome("Campo_Cod", "Campo_Cod", "number") With {._hidden = True})
            l.Add(New ColonneNome("Appezza", "Appezza", "number") With {._hidden = True})
            l.Add(New ColonneNome("Id_Reg", "Id_Reg", "number") With {._hidden = True})
            l.Add(New ColonneNome("Progetto_Cod", "Progetto_Cod", "number") With {._hidden = True})
            l.Add(New ColonneNome("Sa_Nome", "Centro", "string"))
            l.Add(New ColonneNome("Campo_Des", "Campo", "string"))
            l.Add(New ColonneNome("app_nome", "Nome Appezzamento", "string"))
            l.Add(New ColonneNome("Codice_Impianto", "Codice Impianto", "string"))
            l.Add(New ColonneNome("utilizzo", "Utilizzo", "string"))
            l.Add(New ColonneNome("cul_des", "Varieta", "string"))
            l.Add(New ColonneNome("gru_des", "Gruppo Vegetale", "string"))
            l.Add(New ColonneNome("grfi_des", "Finalità", "string"))
            l.Add(New ColonneNome("veg_cod", "veg_cod", "number") With {._hidden = True})
            l.Add(New ColonneNome("cul_cod", "cul_cod", "number") With {._hidden = True})
            l.Add(New ColonneNome("grfi_cod", "grfi_cod", "number") With {._hidden = True})

            'Superficie formattata
            Dim c = New ColonneNome("sup_imp", "Sup. [Ha] impianto", "number")
            c._formatNr = "n4"
            l.Add(c)

            l.Add(New ColonneNome("Progetto_Nome", "Lotto", "string"))
            l.Add(New ColonneNome("Progetto_Des", "Descrizione", "string"))

            l.Add(New ColonneNome("cod_kpin", "KPIN", "string"))
            l.Add(New ColonneNome("cod_block", "BLOCK", "string"))

            'Superficie formattata
            'Dim c2 = New ColonneNome("Sup_Prog", "Sup. [Ha] esercizio", "number")
            'c2._formatNr = "n4"
            'l.Add(c2)

            'l.Add(New ColonneNome("stato_impianto", "Stato Impianto", "string"))
            l.Add(New ColonneNome("Distinta_Chiusa", "Chiuso", "string"))
            l.Add(New ColonneNome("Validita_Inizio", "Valida dal", "date"))
            l.Add(New ColonneNome("Validita_Fine", "Valida al", "date"))
            l.Add(New ColonneNome("Distinta_Chiusa", "Chiuso", "string"))
            l.Add(New ColonneNome("blk_flag", "blk_flag", "number"))

            l.Add(New ColonneNome("Resa", "Resa", "number"))

            'Anna 21/04/22 - Aggiunte colonne Data_Fioritura, Data_Raccolta, Data_Semina alla griglia esercizi
            l.Add(New ColonneNome("Data_Fioritura_Prevista", "Data Fioritura Prevista", "date") With {._Display = False})
            l.Add(New ColonneNome("Data_Raccolta_Prevista", "Data Raccolta Prevista", "date") With {._Display = False})
            l.Add(New ColonneNome("Data_Semina_Prevista", "Data Semina/Trapianto Prevista", "date") With {._Display = False})

            l.Add(New ColonneNome("FlagSecondoRaccolto", "Secondo Raccolto", "string") With {._Display = False})


            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            Dim risp As String = js.JSON_DataTable_Kendo(dt, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipoFiltroKendo_colonne.Menu)

            r.RispostaOK = True
            r.RispostaStringa = risp

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaCatasto() As RispostaStandard
        Dim r As New RispostaStandard
        Try

            Dim objParticelle As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_R
            Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Dim objParametriAgenda As New ParametriAgenda

            Dim FinestraTemporaleInizio = objParametri_Server.FinestraTemporaleInizio
            Dim FinestraTemporaleFine = objParametri_Server.FinestraTemporaleFine

            If objParametriAgenda.Data <> AGRODATAINIZIO Then
                objParametri_Server.FinestraTemporaleInizio = objParametriAgenda.Data.Date
                objParametri_Server.FinestraTemporaleFine = objParametriAgenda.Data.Date
            End If

            Dim dt As DataTable = objParticelle.Leggi_x_anagrafica_desc(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, "", "", "", 0, 0, "", "", "", objParametri_Server)

            If objParametriAgenda.Data <> AGRODATAINIZIO Then
                objParametri_Server.FinestraTemporaleInizio = objParametriAgenda.Data.Date
                objParametri_Server.FinestraTemporaleFine = objParametriAgenda.Data.Date
            End If

            dt.Columns.Add(New DataColumn("Macrousi"))
            dt.Columns.Add(New DataColumn("Utilizzi"))

            Dim prov = ""
            Dim com = ""
            Dim sezione = ""
            Dim foglio = 0
            Dim numero = 0
            Dim subalterno = ""
            Dim obj_Macrousi As New AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousi_R
            Dim obj_Utilizzo As New AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousixUtilizzo_R
            For Each row In dt.Rows
                prov = row("PROV")
                com = row("COM")
                sezione = row("Sezione")
                foglio = row("Foglio")
                numero = row("Numero")
                subalterno = row("Subalterno")

                'Dim DTMacrousi = obj_Macrousi.Leggi(objParametriAgenda.Piva, prov, com, sezione, foglio, numero, subalterno, "", enumSelezioneVariabile.Selezione_JoinDescrizioni, "", "", objParametri_Server)

                'Dim strMacrousi = "<table>"
                'For Each rowMacrouso In DTMacrousi.Rows

                '    strMacrousi &= "<tr><td><b>" & rowMacrouso("Macrouso_Des") & "</b></td><td><i>" & rowMacrouso("Superficie") & "</i></td></tr>"
                'Next
                'strMacrousi &= "</table>"
                'row("Macrousi") = strMacrousi

                'Dim DTUtilizzi = obj_Utilizzo.Leggi(objParametriAgenda.Piva, prov, com, sezione, foglio, numero, subalterno, "", "", "", enumSelezioneVariabile.Selezione_JoinDescrizioni, "", "", objParametri_Server)
                'Dim strUtilizzi = "<table>"
                'For Each rowUtilizzo In DTUtilizzi.Rows
                '    strUtilizzi &= "<tr><td><b>" & rowUtilizzo("Veg_Des_Agea") & " - " & rowUtilizzo("Cul_Des_Agea") & "</b></td><td><i>" & rowUtilizzo("Superficie") & "</i></td></tr>"
                'Next
                'strUtilizzi &= "</table>"
                'row("Utilizzi") = strUtilizzi

            Next


            'creo la lista delle colonne da visualizzare
            Dim l As New List(Of ColonneNome)

            l.Add(New ColonneNome("chiave", "chiave", "string") With {._hidden = True})
            l.Add(New ColonneNome("part_cod", "part_cod", "number"))
            l.Add(New ColonneNome("cod_particella", "Codice Particella", "string"))
            'If objParametriAgenda.Sa_Cod = 0 Then
            l.Add(New ColonneNome("Sa_Nome", "Centro", "string"))
            'End If
            l.Add(New ColonneNome("Piva", "Piva", "string"))
            l.Add(New ColonneNome("Prov", "Prov Istat", "string"))
            l.Add(New ColonneNome("Com", "Com Istat", "string"))
            l.Add(New ColonneNome("COMUNE", "Com.", "string"))
            l.Add(New ColonneNome("PROVINCIA", "Prov.", "string"))
            l.Add(New ColonneNome("SEZIONE", "Sez.", "string"))
            l.Add(New ColonneNome("FOGLIO", "Fgl.", "number"))
            l.Add(New ColonneNome("NUMERO", "Num.", "number"))
            l.Add(New ColonneNome("SUBALTERNO", "S.", "string"))
            l.Add(New ColonneNome("Macrousi", "Macrousi", "string"))
            l.Add(New ColonneNome("Utilizzi", "Utilizzi", "string"))
            l.Add(New ColonneNome("Titolo_possesso", "Possesso", "string"))
            l.Add(New ColonneNome("Sup_Catastale", "Sup. Catastale [ha]", "number"))
            l.Add(New ColonneNome("sup_Condotta", "Sup. Condotta [ha]", "number"))
            l.Add(New ColonneNome("Validita_Inizio", "Validità Inizio", "date"))
            l.Add(New ColonneNome("Validita_Fine", "Validità Fine", "date"))

            l.Add(New ColonneNome("Data_Creazione", "Data Creazione", "date"))
            l.Add(New ColonneNome("Utente_Creazione", "Utente Creazione", "string"))
            l.Add(New ColonneNome("Data_Modifica", "Data Modifica", "date"))
            l.Add(New ColonneNome("Utente_Modifica", "Utente Modifica", "string"))

            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            Dim risp As String = js.JSON_DataTable_Kendo(dt, l, False, False, TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa, "")

            r.RispostaOK = True
            r.RispostaStringa = risp

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaRaggruppamentiStalla() As RispostaStandard
        Dim r As New RispostaStandard
        Try

            Dim objRaggr_Stalla As New AgronicaCoreAnagrafeDAL.Stalla_Raggruppamenti_R
            Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
            Dim objParametri_Utenti As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim oLeggiPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            Dim permessoBDN_R As Boolean = oLeggiPermessi.Controlla_Permessi_Utente(objParametri_Utenti.UtenteUsername,
                                                                                5, 99, 0,
                                                                                Date.Now, "",
                                                                                objParametri_Utenti)

            Dim objParametriAgenda As New ParametriAgenda
            Dim dt As DataTable = objRaggr_Stalla.Leggi_x_anagrafica(objParametri_Server.PivaSuperUser,
                                                                     objParametriAgenda.Piva,
                                                                     objParametriAgenda.Sa_Cod,
                                                                     objParametriAgenda.Fabbricato,
                                                                     0,
                                                                     "",
                                                                     "",
                                                                     objParametri_Server)

            'creo la lista delle colonne da visualizzare
            Dim l As New List(Of ColonneNome)

            l.Add(New ColonneNome("chiave", "chiave", "string") With {._hidden = True})
            If objParametriAgenda.Sa_Cod = 0 Then
                l.Add(New ColonneNome("Sa_Nome", AgronicaAgenda_2010.CentroAziendale, "string"))
            End If
            If objParametriAgenda.Fabbricato = 0 Then
                l.Add(New ColonneNome("sta_des", AgronicaAgenda_2010.Stalla, "string"))
            End If
            l.Add(New ColonneNome("sa_cod", "sa_cod", "string") With {._hidden = True})
            l.Add(New ColonneNome("piva", "piva", "string") With {._hidden = True})
            l.Add(New ColonneNome("rag_soc", "rag_soc", "string") With {._hidden = True})
            l.Add(New ColonneNome("STA_NUM", "STA_NUM", "string") With {._hidden = True})
            l.Add(New ColonneNome("raggruppamento_cod", "raggruppamento_cod", "string") With {._hidden = True})
            l.Add(New ColonneNome("raggruppamento_Des", AgronicaAgenda_2010.Nome, "string"))
            l.Add(New ColonneNome("raggruppamento_tipo_Des", AgronicaAgenda_2010.Tipo, "string"))

            If permessoBDN_R Then
                l.Add(New ColonneNome("Flag_BDN", AgronicaAgenda_2010.PresenteBDN, "string"))
            End If

            l.Add(New ColonneNome("Data_Modifica", AgronicaAgenda_2010.UltimaModifica, "date"))
            l.Add(New ColonneNome("Utente_Modifica", AgronicaAgenda_2010.UtenteModifica, "string"))

            l.Add(New ColonneNome("Data_Creazione", AgronicaAgenda_2010.DataCreazione, "date"))
            l.Add(New ColonneNome("Utente_Creazione", AgronicaAgenda_2010.UtenteCreazione, "string"))

            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            Dim risp As String = js.JSON_DataTable_Kendo(dt, l, False, False, TipoFiltroKendo_colonne.CasellaDiscesa, "")

            r.RispostaOK = True
            r.RispostaStringa = risp

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaFabbricati() As RispostaStandard
        Dim r As New RispostaStandard
        Try

            Dim objFabbricati As New AgronicaCoreAnagrafeDAL.Fabbricati_R
            Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim objParametriAgenda As New ParametriAgenda

            Dim FinestraTemporaleInizio = objParametri_Server.FinestraTemporaleInizio
            Dim FinestraTemporaleFine = objParametri_Server.FinestraTemporaleFine

            If objParametriAgenda.Data <> AGRODATAINIZIO Then
                objParametri_Server.FinestraTemporaleInizio = objParametriAgenda.Data.Date
                objParametri_Server.FinestraTemporaleFine = objParametriAgenda.Data.Date
            End If

            Dim dt As DataTable = objFabbricati.Leggi_x_anagrafica(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, 0, "", "", objParametri_Server)

            If objParametriAgenda.Data <> AGRODATAINIZIO Then
                objParametri_Server.FinestraTemporaleInizio = FinestraTemporaleInizio
                objParametri_Server.FinestraTemporaleFine = FinestraTemporaleFine
            End If

            'creo la lista delle colonne da visualizzare
            Dim l As New List(Of ColonneNome)

            l.Add(New ColonneNome("chiave", "chiave", "string") With {._hidden = True})
            l.Add(New ColonneNome("Fabbricato", AgronicaAgenda_2010.Fabbricato, "string"))
            l.Add(New ColonneNome("Tipo_Fabbricato_Cod", "Tipo_Fabbricato_Cod", "string") With {._hidden = True})
            l.Add(New ColonneNome("tipo", AgronicaAgenda_2010.Tipo, "string"))

            l.Add(New ColonneNome("Data_Creazione", AgronicaAgenda_2010.DataCreazione, "date"))
            l.Add(New ColonneNome("Utente_Creazione", AgronicaAgenda_2010.UtenteCreazione, "string"))
            l.Add(New ColonneNome("Data_Modifica", AgronicaAgenda_2010.DataModifica, "date"))
            l.Add(New ColonneNome("Utente_Modifica", AgronicaAgenda_2010.UtenteModifica, "string"))

            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            Dim risp As String = js.JSON_DataTable_Kendo(dt, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa)
            r.RispostaOK = True
            r.RispostaStringa = risp

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaStalle() As RispostaStandard
        Dim r As New RispostaStandard
        Try

            Dim objFabbricati As New AgronicaCoreAnagrafeDAL.Fabbricati_R
            Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
            Dim objParametri_Utenti As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim oLeggiPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            Dim permessoBDN_R As Boolean = oLeggiPermessi.Controlla_Permessi_Utente(objParametri_Utenti.UtenteUsername,
                                                                                    5, 99, 0,
                                                                                    Date.Now, "",
                                                                                    objParametri_Utenti)

            Dim objParametriAgenda As New ParametriAgenda
            Dim dt As DataTable = objFabbricati.LeggiStalle_x_anagrafica(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, 0, "", "", objParametri_Server)

            'creo la lista delle colonne da visualizzare
            Dim l As New List(Of ColonneNome)

            l.Add(New ColonneNome("chiave", "chiave", "string") With {._hidden = True})
            l.Add(New ColonneNome("Fabbricato", AgronicaAgenda_2010.Denominazione, "string"))
            l.Add(New ColonneNome("Tipo_Fabbricato_Cod", "Tipo_Fabbricato_Cod", "string") With {._hidden = True})
            l.Add(New ColonneNome("SPE_DES", AgronicaAgenda_2010.Specie, "string"))
            l.Add(New ColonneNome("IPRO_DES", AgronicaAgenda_2010.IndirizzoProduttivo, "string"))
            l.Add(New ColonneNome("tipo", AgronicaAgenda_2010.Tipo, "string"))

            If permessoBDN_R Then
                l.Add(New ColonneNome("BDN_Codice_Azienda", AgronicaAgenda_2010.CodiceASL, "string"))
            End If

            l.Add(New ColonneNome("Data_Modifica", AgronicaAgenda_2010.UltimaModifica, "date"))
            l.Add(New ColonneNome("Utente_Modifica", AgronicaAgenda_2010.UtenteModifica, "string"))
            l.Add(New ColonneNome("Data_Creazione", AgronicaAgenda_2010.DataCreazione, "date"))
            l.Add(New ColonneNome("Utente_Creazione", AgronicaAgenda_2010.UtenteCreazione, "string"))

            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            Dim risp As String = js.JSON_DataTable_Kendo(dt, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaDiscesa)
            r.RispostaOK = True
            r.RispostaStringa = risp

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r


    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaZoo() As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

        Dim oLeggiPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim permessoBDN_R As Boolean = oLeggiPermessi.Controlla_Permessi_Utente(objParametri_Utenti.UtenteUsername,
                                                                                5, 99, 0,
                                                                                Date.Now, "",
                                                                                objParametri_Utenti)

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
        Dim GiasContext As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities = New Gias_DeveloperServer_Entities(EFConnString)

        Try

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Dim objParametriAgenda As New ParametriAgenda
            Dim objZooAnimali_R As New AgronicaCoreAnagrafeDAL.Zoo_Animali
            'Dim objZoo As New AgronicaCoreZooDAL.Zoo_Animali_R

            Dim filtroData As Date = objParametriAgenda.Data

            If filtroData < AGRODATAINIZIO Then
                filtroData = AGRODATAINIZIO
            ElseIf filtroData > AGRODATAFINE Then
                filtroData = AGRODATAFINE
            End If

            Dim dt As New DataTable

            Dim Sta_Num As Integer = 0
            If IsNumeric(objParametriAgenda.Fabbricato) Then
                Sta_Num = objParametriAgenda.Fabbricato
            End If

            Dim Raggruppamento_Cod As Integer = 0
            If IsNothing(objParametriAgenda.Raggruppamento_Cod) OrElse objParametriAgenda.Raggruppamento_Cod <> "" Then
                Raggruppamento_Cod = objParametriAgenda.Raggruppamento_Cod
            End If
            Dim filtraGiacenze1 = True
            Dim filtraFornitori = True
            Dim sa_cod = objParametriAgenda.Sa_Cod
            If filtroData = AGRODATAINIZIO Then
                filtraGiacenze1 = False
                sa_cod = 0
                Sta_Num = 0
                Raggruppamento_Cod = 0
            End If

            Dim objProfilo As New AgronicaCoreUtentiDAL.Utenti_Profili_Read
            Dim Filtro_Visibilita_Utente = Not objProfilo.HasFullVisibility(objParametri_Utenti.UtenteUsername, objParametri_Utenti)

            Dim newFiltroData As New DateTime(filtroData.Year, filtroData.Month, filtroData.Day, 23, 59, 59)

            dt = objZooAnimali_R.Leggi_Giacenze(objParametriAgenda.Piva, sa_cod,
                                                Sta_Num, Raggruppamento_Cod, 0,
                                                newFiltroData, objParametri_Server, , Filtro_Visibilita_Utente, , filtraGiacenze1, filtraFornitori)

            If Not filtraGiacenze1 Then
                dt.Columns.Remove("BDN_Codice_Azienda")
                dt.Columns.Remove("Sa_Cod")
                dt.Columns.Remove("sa_nome")
                dt.Columns.Remove("Id_Destinazione")
                dt.Columns.Remove("Tipo_Destinazione")
                dt.Columns.Remove("STA_DES")
                dt.Columns.Remove("STA_NUM")
                dt.Columns.Remove("Raggruppamento_Des")
                dt.Columns.Remove("Raggruppamento_Cod")
                dt.Columns.Remove("Giacenza")
                If dt.Rows.Count > 0 Then
                    dt = dt.AsEnumerable().Distinct(DataRowComparer.Default).CopyToDataTable()
                End If
            End If

            'creo la lista delle colonne da visualizzare
            Dim l As New List(Of ColonneNome)

            'i18n: Non tradotte perché sovrascritte dalle label lato javascript

            l.Add(New ColonneNome("chiave", "chiave", "string"))
            l.Add(New ColonneNome("Cod_Animale", "Cod_Animale", "number"))
            '
            'l.Add(New ColonneNome("Cod_Animale", "chiave", "string") With {._hidden = True})
            l.Add(New ColonneNome("Matricola", "Matricola", "string"))
            l.Add(New ColonneNome("Tag", "Tag", "string"))
            l.Add(New ColonneNome("Matricola_Breve", "Matricola Breve", "string"))
            l.Add(New ColonneNome("Matricola_Breve4", "Matricola Breve", "string"))
            l.Add(New ColonneNome("SPE_DES", "Specie", "string"))
            l.Add(New ColonneNome("RAZ_DES", "Razza", "string"))
            l.Add(New ColonneNome("IPRO_DES", "Indirizzo Produttivo", "string"))
            l.Add(New ColonneNome("Progetto", "Progetto", "string"))

            If filtraGiacenze1 Then
                'dt.Columns.Remove("BDN_Codice_Azienda")
                l.Add(New ColonneNome("BDN_Codice_Azienda", "BDN Codice Azienda", "string"))
                l.Add(New ColonneNome("sa_nome", "Centro", "string"))
                l.Add(New ColonneNome("sa_nome", "Centro", "string"))
                l.Add(New ColonneNome("STA_DES", "Stalla", "string"))
                l.Add(New ColonneNome("Raggruppamento_Des", "Gruppo", "string"))
                l.Add(New ColonneNome("Cod_Progetto", "Cod_Progetto", "number") With {._hidden = True})
                l.Add(New ColonneNome("STA_DES", "STA_DES", "string") With {._hidden = True})
                l.Add(New ColonneNome("Progetto_Nome", "Distinta Nome", "string"))
                l.Add(New ColonneNome("Codice_Distinta", "Lotto", "string"))
                l.Add(New ColonneNome("Metodo_Produzione", "Metodo Produzione", "string"))
                l.Add(New ColonneNome("Stato_Des", "Stato", "string"))
            End If
            l.Add(New ColonneNome("Nome", "Nome", "string"))
            l.Add(New ColonneNome("Sesso", "Sesso", "string"))
            l.Add(New ColonneNome("Validato", "Validato", "string"))
            l.Add(New ColonneNome("Tipo_Des", "Tipo", "string"))

            l.Add(New ColonneNome("Mat_Madre", "Matricola Madre", "string"))
            l.Add(New ColonneNome("RazDes_Madre", "Razza Madre", "string"))
            l.Add(New ColonneNome("Mat_Padre", "Matricola Padre", "string"))
            l.Add(New ColonneNome("RazDes_Padre", "Razza Padre", "string"))
            l.Add(New ColonneNome("Lotto_Fornitore", "Lotto_Fornitore", "string"))
            l.Add(New ColonneNome("Cod_Contatto", "C.F. Fornitore", "string"))

            l.Add(New ColonneNome("Piva", "Piva", "string") With {._hidden = True})


            'dati BDN
            If permessoBDN_R Then
                l.Add(New ColonneNome("FlagBDN", "FlagBDN", "string"))
                l.Add(New ColonneNome("Certificato", "Num Certificato", "string"))
                l.Add(New ColonneNome("Id_Capo_BDN", "ID Capo BDN", "string"))
            End If

            l.Add(New ColonneNome("Rag_Soc", "Fornitore", "string"))
            l.Add(New ColonneNome("CF_PROPRIETARIO", "C.F. Proprietario", "string"))
            l.Add(New ColonneNome("CF_DETENTORE", "C.F. Detentore", "string"))
            l.Add(New ColonneNome("AUSL_AZI_NASCITA", "Azienda Nascita", "string"))

            l.Add(New ColonneNome("Dat_Nascita", "Data Nascita", "date"))
            'template: '#= (kendo.toString(Validita_Inizio, "dd/MM/yyyy" ) == "01/01/1900") ? "" : kendo.toString(Validita_Inizio, "dd/MM/yyyy" ) #'
            Dim c As New ColonneNome("Validita_Inizio", "Data Inizio", "date")
            'c._FormatoParticolare = "#= kendo.toString(Validita_Inizio, dd / MM / yyyy' ) == '01/01/1900') ? '' : kendo.toString(Validita_Inizio, 'dd/MM/yyyy'#"
            l.Add(c)

            Dim c1 As New ColonneNome("Validita_Fine", "Data Fine", "date")
            'c1._FormatoParticolare = "#= kendo.toString(Validita_Inizio, 'dd/MM/yyyy' ) == '31/12/2100') ? '' : kendo.toString(Validita_Inizio, 'dd/MM/yyyy'#"
            l.Add(c1)

            If filtraFornitori Then
                l.Add(New ColonneNome("RagSoc_FornFatt", "Fornitore Fatturazione", "string"))
                l.Add(New ColonneNome("RagSoc_FornProv", "Fornitore Provenienza", "string"))
            End If

            l.Add(New ColonneNome("Data_Documento_Ingresso", "Data Documento Ingresso", "date"))
            l.Add(New ColonneNome("Data_Documento_Uscita", "Data Documento Uscita", "date"))

            l.Add(New ColonneNome("Modello4_Ingresso_Numero", "N. Modello 4 Ingresso", "string"))
            l.Add(New ColonneNome("Modello4_Ingresso_Prenotazione", "Codice Modello 4 Ingresso", "string"))
            l.Add(New ColonneNome("Modello4_Uscita_Numero", "N. Modello 4 Uscita", "string"))
            l.Add(New ColonneNome("Modello4_Uscita_Prenotazione", "Codice Modello 4 Uscita", "string"))
            l.Add(New ColonneNome("Codice_Azienda_Uscita", "Codice Azienda Uscita", "string"))
            l.Add(New ColonneNome("RagSoc_StallaSvezz", "Stalla Svezzamento", "string"))
            l.Add(New ColonneNome("Incremento_Teorico", "Incremento Teorico", "number"))

            l.Add(New ColonneNome("Codice_Azienda_Fornitore", "Codice Azienda Fornitore", "string"))
            l.Add(New ColonneNome("N_Bolla_Fornitore", "Numero DDT Ingresso", "string"))
            l.Add(New ColonneNome("Data_DDT_Ingresso", "Data DDT Ingresso", "date"))
            l.Add(New ColonneNome("N_Bolla_Uscita", "Numero DDT Uscita", "string"))
            l.Add(New ColonneNome("Data_DDT_Uscita", "Data DDT Uscita", "date"))


            l.Add(New ColonneNome("Data_Modifica", "Ultima Modifica", "date"))
            l.Add(New ColonneNome("Utente_Modifica", "Utente Modifica", "string"))

            l.Add(New ColonneNome("Data_Creazione", "Data Creazione", "date"))
            l.Add(New ColonneNome("Utente_Creazione", "Utente Creazione", "string"))

            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            Dim risp As String = js.JSON_DataTable_Kendo(dt, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa)
            r.RispostaOK = True
            r.RispostaStringa = risp

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function AvModAlg_Inizializza(
            ByVal piva As String,
            ByVal sa_cod As Integer,
            ByVal sorgentedati_cod As Integer,
            ByVal origine_cod As Integer
        ) As rispostaStandard(Of MenuBS_Anagrafica_AvModAlg_Inizializza_Response)

        Dim r As New rispostaStandard(Of MenuBS_Anagrafica_AvModAlg_Inizializza_Response)
        r.RispostaStringa = New MenuBS_Anagrafica_AvModAlg_Inizializza_Response

        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

        Try

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If


            Dim objParametriAgenda As New ParametriAgenda
            Dim objZooR As New AgronicaCoreMeteoDAL.DSS_Centri_Aziendali_Agronica_Stazioni_Meteo_R
            Dim objModStazioneR As New AgronicaCoreMeteoDAL.DSS_Agronica_Stazioni_Meteo_ModelliPrevisionali_R
            Dim objZooW As New AgronicaCoreMeteoDAL.DSS_Centri_Aziendali_Agronica_Stazioni_Meteo_W
            Dim dt As New DataTable


            dt = objZooR.Leggi(" piva = '" & piva & "' and  sa_cod = " & sa_cod & " and  Stazione_Cod = " & origine_cod & " and tipo_sorgente = " & sorgentedati_cod, "", objParametri_Server)
            If dt.Rows.Count = 1 Then
                r.RispostaStringa.AssociazioneCentroOrigine = True
            End If

            Dim xWS As New AgronicaCoreWebService.Meteo


            Dim ss As String = xWS.AvModAlg_Inizializza(piva, "-1", objParametri_Server)

            Dim jss = New JavaScriptSerializer()
            Dim rispostaStandardModelli As rispostaStandard(Of List(Of AgronicaCoreModelliPrevisionaliBIZ.cAvModAlg)) =
                jss.Deserialize(Of rispostaStandard(Of List(Of AgronicaCoreModelliPrevisionaliBIZ.cAvModAlg)))(ss)

            r.RispostaStringa.ModelliPrevisionaliTree = New List(Of KendoHierarchicalDataSource)
            r.RispostaStringa.ModelliPrevisionaliTreeListaCheck = New List(Of String)



            Dim xFiltro As New List(Of String)
            For Each modAlg In rispostaStandardModelli.RispostaStringa

                Dim cModAlg As String() =
                    modAlg.AvModAlg_Cod.Split("-")

                Dim item As String = "'" & cModAlg(2) & "-" & cModAlg(3) & "'"

                If Not xFiltro.Contains(item) Then
                    xFiltro.Add(item)
                End If

            Next

            Dim xFiltroQuery As String = ""

            If xFiltro.Count > 0 Then
                xFiltroQuery = " cast(Mod_Cod as varchar(50)) + '-' + cast(Algoritmo_cod as varchar(50)) in ( " & String.Join(",", xFiltro.ToArray()) & ")"
            End If

            Dim dtModelliAssociati As DataTable =
                objModStazioneR.LeggiPerStazione(sorgentedati_cod, origine_cod, xFiltroQuery, "", objParametri_Server)

            Dim ListaNodiPerRicerca As New KendoHierarchicalDataSource With {
                .id = "",
                .items = New List(Of KendoHierarchicalDataSource)
            }

            For Each modAlg In rispostaStandardModelli.RispostaStringa
                Dim cModAlg As String() =
                    modAlg.AvModAlg_Cod.Split("-")

                Dim xTest As DataRow() = dtModelliAssociati.Select(" mod_cod =  " & cModAlg(2) & " and algoritmo_cod = " & cModAlg(3))
                If xTest.Count > 0 Then
                    modAlg.Selezionato = True
                End If

                Dim addSpecie As Boolean = False
                Dim itmSpecie As KendoHierarchicalDataSource = (From vk In r.RispostaStringa.ModelliPrevisionaliTree
                                                                Where vk.id = modAlg.Veg_Cod).FirstOrDefault

                If itmSpecie Is Nothing Then

                    itmSpecie = New KendoHierarchicalDataSource With {
                            .id = modAlg.Veg_Cod,
                            .text = modAlg.Veg_Des,
                            .items = New List(Of KendoHierarchicalDataSource)
                        }
                    addSpecie = True
                End If

                itmSpecie.items.Add(New KendoHierarchicalDataSource With {
                            .id = modAlg.AvModAlg_Cod & "|" & modAlg.AvModAlg_Des,
                            .text = modAlg.AvModAlg_Des,
                            .checked = modAlg.Selezionato
                        })

                If addSpecie Then
                    r.RispostaStringa.ModelliPrevisionaliTree.Add(itmSpecie)
                    ListaNodiPerRicerca.items.Add(itmSpecie)
                End If

            Next



            r.RispostaStringa.ModelliPrevisionali = rispostaStandardModelli.RispostaStringa

            Dim estrazioneChecked As New KendoDataHelper
            estrazioneChecked.ListaFlatElementiCheck(ListaNodiPerRicerca, r.RispostaStringa.ModelliPrevisionaliTreeListaCheck)

            r.RispostaOK = rispostaStandardModelli.RispostaOK
            r.Errore = rispostaStandardModelli.Errore


        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r


    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function EliminaStazioniMeteo(ByVal ChiaveStazione As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

        Try

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Dim objParametriAgenda As New ParametriAgenda
            Dim objZooR As New AgronicaCoreMeteoDAL.DSS_Centri_Aziendali_Agronica_Stazioni_Meteo_R
            Dim objZooW As New AgronicaCoreMeteoDAL.DSS_Centri_Aziendali_Agronica_Stazioni_Meteo_W
            Dim dt As New DataTable

            Dim vKey As String() = ChiaveStazione.Split("-")

            Dim piva As String = vKey(0)
            Dim sa_cod As Integer = vKey(1)
            Dim sorgentedati_cod As Integer = vKey(2)
            Dim origine_cod As Integer = vKey(3)

            Dim objModelliW As New AgronicaCoreMeteoDAL.DSS_Agronica_Stazioni_Meteo_ModelliPrevisionali_W
            objModelliW.CancellaStazione(sorgentedati_cod, origine_cod, "", objParametri_Server)

            dt = objZooR.Leggi(" piva = '" & piva & "' and  sa_cod = " & sa_cod & " and  Stazione_Cod = " & origine_cod & " and tipo_sorgente = " & sorgentedati_cod, "", objParametri_Server)
            If dt.Rows.Count = 1 Then
                objZooW.Cancella(piva, sa_cod, sorgentedati_cod, origine_cod, "", objParametri_Server)
            End If

            r.RispostaOK = True
            r.RispostaStringa = AgronicaAgenda_2010.EliminazioneAvvenutaConSuccesso

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r


    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function DssMeteoMemorizzaNuovaStazione(ByVal piva As String, ByVal sa_cod As Integer, sorgentedati_cod As Integer, origine_cod As Integer, origine_des As String, ModelliPrevisionali As List(Of AgronicaCoreModelliPrevisionaliBIZ.cAvModAlg)) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

        Try

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Dim objParametriAgenda As New ParametriAgenda
            Dim objZooR As New AgronicaCoreMeteoDAL.DSS_Centri_Aziendali_Agronica_Stazioni_Meteo_R
            Dim objZooW As New AgronicaCoreMeteoDAL.DSS_Centri_Aziendali_Agronica_Stazioni_Meteo_W
            Dim objModelliR As New AgronicaCoreMeteoDAL.DSS_Agronica_Stazioni_Meteo_ModelliPrevisionali_R
            Dim objModelliW As New AgronicaCoreMeteoDAL.DSS_Agronica_Stazioni_Meteo_ModelliPrevisionali_W
            Dim dt As New DataTable

            dt = objZooR.Leggi(" piva = '" & piva & "' and  sa_cod = " & sa_cod & " and  Stazione_Cod = " & origine_cod & " and tipo_sorgente = " & sorgentedati_cod, "", objParametri_Server)
            If dt.Rows.Count = 0 Then
                objZooW.Scrivi(piva, sa_cod, sorgentedati_cod, origine_cod, origine_des, objParametri_Server)
            End If

            If Not ModelliPrevisionali Is Nothing AndAlso ModelliPrevisionali.Count > 0 Then

                objModelliW.CancellaStazione(sorgentedati_cod, origine_cod, "", objParametri_Server)


                For Each modelloAssociato In ModelliPrevisionali
                    Dim cMod As String() = modelloAssociato.AvModAlg_Cod.Split("-")
                    Dim ParametriElaborazione As String = JsonConvert.SerializeObject(New With {
                                                                                      .Veg_Cod = cMod(1)
                                                                                      })
                    If cMod.Length > 1 Then
                        Dim dtMod As DataTable =
                            objModelliR.LeggiPerStazione(sorgentedati_cod, origine_cod, " mod_cod =  " & cMod(2) & " AND algoritmo_cod = " & cMod(3) & " AND ParametriElaborazione = '" & ParametriElaborazione & "' ", "", objParametri_Server)
                        If dtMod.Rows.Count = 0 Then
                            objModelliW.Scrivi(sorgentedati_cod, origine_cod, cMod(2), cMod(3), ParametriElaborazione, 1, 300, modelloAssociato.DescrModello, objParametri_Server)
                        End If

                    End If

                Next
            End If

            r.RispostaOK = True
            r.RispostaStringa = AgronicaAgenda_2010.SalvataggioAvvenutoConSuccesso

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r


    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaStazioniMeteo(ByVal piva As String, ByVal sa_cod As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))
        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim objParametriAgenda As New ParametriAgenda
            Dim objZoo As New AgronicaCoreMeteoDAL.DSS_Agronica_Stazioni_Meteo_ModelliPrevisionali_R
            Dim dt As New DataTable

            dt = objZoo.LeggiPerCentroAziendale(piva, sa_cod, "", "", objParametri_Server)

            'ciclo per ottenere le descrizioni delle stazioni

            'creo la lista delle colonne da visualizzare
            Dim l As New List(Of ColonneNome)

            l.Add(New ColonneNome("chiave", "chiave", "string") With {._hidden = True})

            l.Add(New ColonneNome("Centro", AgronicaAgenda_2010.Centro, "string"))
            l.Add(New ColonneNome("TipoSorgente_Des", AgronicaAgenda_2010.CategoriaSorgenteDati, "string"))
            l.Add(New ColonneNome("Origine", AgronicaAgenda_2010.OrigineDatiMeteo, "string"))
            l.Add(New ColonneNome("Modelli", AgronicaAgenda_2010.ModelliPrevisionaliAssociatiOrigineDatiMeteo, "string") With {._RemoveHtmlEncode = True})


            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            Dim risp As String = js.JSON_DataTable_Kendo(dt, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa)
            r.RispostaOK = True
            r.RispostaStringa = risp

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r


    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaStalle_ddl(ByVal sa_cod As Integer) As RispostaStandard
        Dim r As New RispostaStandard
        Try
            Dim objParametriAgenda = New ParametriAgenda
            Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
            objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
            Dim dt As DataTable
            'Dim objFabbr As New AgronicaCoreAnagrafeDAL.Fabbricati_R
            'dt = objFabbr.Stalle_Leggi(objParametri_Server.PivaSuperUser, "", objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, 0, False, False, False, "", "", objParametri_Server)

            Dim objStall As New AgronicaCoreAnagrafeDAL.Stalla_R
            dt = objStall.Leggi(objParametriAgenda.Piva, sa_cod, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

            ' Inserisco la scelta Stalla
            Dim i As Integer
            Dim jObj As New JObject
            Dim jsonArray As New JArray
            For i = 0 To dt.Rows.Count - 1
                jObj = New JObject
                jObj.Add(New JProperty("des", dt.Rows(i).Item("STA_DES")))
                jObj.Add(New JProperty("val", dt.Rows(i).Item("STA_NUM")))
                jsonArray.Add(jObj)
            Next

            r.RispostaOK = True
            r.RispostaStringa = jsonArray.ToString

        Catch ex As Exception
            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaContatti() As RispostaStandard
        Dim r As New RispostaStandard
        Try

            Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R
            Dim objparametri_server As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("asg_objparametri_server"))

            Dim objparametriagenda As New ParametriAgenda

            Dim modalita As String = HttpContext.Current.Session("modalita")

            If objparametriagenda.Data <> AGRODATAINIZIO Then
                objparametri_server.ImpostaFinestre_con_SalvataggioTemporale(objparametriagenda.Data.Date, objparametriagenda.Data.Date)
            End If

            Dim dt As DataTable = objContatti.leggi_x_anagrafica2(objparametriagenda.Piva, modalita, "", "", objparametri_server)
            Dim dt1 As DataTable = objContatti.leggi_x_anagrafica2(objparametriagenda.Piva, "-1", "", "", objparametri_server)

            If objparametriagenda.Data <> AGRODATAINIZIO Then
                objparametri_server.ResettaFinestra()
            End If

            dt.Merge(dt1)
            dt.Columns.Add(New DataColumn("Tipo_Contatto", GetType(String)))
            dt.Columns.Add(New DataColumn("Impresa_Referente", GetType(String)))

            For Each row In dt.Rows
                If row("sa_cod") = -1 Then
                    row("Tipo_Contatto") = "Pubblico"
                Else
                    row("Tipo_Contatto") = "Privato"
                End If
            Next

            'creo la lista delle colonne da visualizzare
            Dim l As New List(Of ColonneNome)

            l.Add(New ColonneNome("chiave", "chiave", "string") With {._hidden = True})
            l.Add(New ColonneNome("CF", "CF", "string"))
            l.Add(New ColonneNome("sa_cod", "sa_cod", "string"))
            l.Add(New ColonneNome("Contatto_Des", "Nome del Contatto", "string"))
            'l.Add(New ColonneNome("Validita_inizio", "Validita_inizio", "string"))
            'l.Add(New ColonneNome("Validita_Fine", "Validita_Fine", "string"))
            l.Add(New ColonneNome("Rapporto_Des", "Rapporto", "string"))
            l.Add(New ColonneNome("Tipo_Contatto", "Tipo", "string"))
            l.Add(New ColonneNome("Impresa", "Impresa Referente", "string"))
            l.Add(New ColonneNome("Settore_Des", "Codice Contatto", "string"))
            'l.Add(New ColonneNome("Cod_Indirizzo", "Cod_Indirizzo", "string"))
            'l.Add(New ColonneNome("Tipo_Indirizzo", "Tipo_Indirizzo", "string"))
            'l.Add(New ColonneNome("Ind_Des", "Ind_Des", "string"))
            'l.Add(New ColonneNome("frz_des", "frz_des", "string"))
            'l.Add(New ColonneNome("CAP", "CAP", "string"))
            'l.Add(New ColonneNome("Com_Des", "Com_Des", "string"))
            'l.Add(New ColonneNome("Pro_cod", "Pro_cod", "string"))
            'l.Add(New ColonneNome("stato", "stato", "string"))
            'l.Add(New ColonneNome("note", "note", "string"))
            'l.Add(New ColonneNome("pro_cod_istat", "pro_cod_istat", "string"))
            'l.Add(New ColonneNome("com_cod_istat", "com_cod_istat", "string"))
            'l.Add(New ColonneNome("cod_rubrica", "cod_rubrica", "string"))
            'l.Add(New ColonneNome("Numero", "Numero", "string"))
            'l.Add(New ColonneNome("descr", "descr", "string"))
            'l.Add(New ColonneNome("Elem_cod", "Elem_cod", "string"))
            'l.Add(New ColonneNome("Mezzo", "Mezzo", "string"))
            'l.Add(New ColonneNome("Prezzo_Unitario", "Prezzo_Unitario", "string"))
            'l.Add(New ColonneNome("Costo_Inizio", "Costo_Inizio", "string"))
            'l.Add(New ColonneNome("Costo_Fine", "Costo_Fine", "string"))

            l.Add(New ColonneNome("Data_Creazione", "Data Creazione", "date"))
            l.Add(New ColonneNome("Utente_Creazione", "Utente Creazione", "string"))
            l.Add(New ColonneNome("Data_Modifica", "Data Modifica", "date"))
            l.Add(New ColonneNome("Utente_Modifica", "Utente Modifica", "string"))

            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            Dim risp As String = js.JSON_DataTable_Kendo(dt, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa)
            r.RispostaStringa = risp
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
    Public Shared Function CaricaMacchine() As RispostaStandard
        Dim r As New RispostaStandard
        Try

            Dim objMacchine As New AgronicaCoreContabDAL.Parco_Macchine_R
            Dim objparametri_server As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("asg_objparametri_server"))

            Dim objparametriagenda As New ParametriAgenda

            ' 25/02/2020 -> Aggiunto filtro aggiuntivo per caricare solo le macchine dell'impresa
            ' Dim filtroAggiuntivo As String = " Parco_Macchine.Cod_Contatto = ''"

            If objparametriagenda.Data <> AGRODATAINIZIO Then
                objparametri_server.ImpostaFinestre_con_SalvataggioTemporale(objparametriagenda.Data.Date, objparametriagenda.Data.Date)
            End If

            Dim dt As DataTable = objMacchine.leggi_x_anagrafica(objparametriagenda.Piva, "", "", objparametri_server)

            If objparametriagenda.Data <> AGRODATAINIZIO Then
                objparametri_server.ResettaFinestra()
            End If

            'creo la lista delle colonne da visualizzare
            Dim l As New List(Of ColonneNome)

            l.Add(New ColonneNome("chiave", "chiave", "string") With {._hidden = True})
            l.Add(New ColonneNome("Cod_Contatto", "Cod_Contatto", "string") With {._hidden = True})
            l.Add(New ColonneNome("Contatto_Des", "Contatto_Des", "string"))
            l.Add(New ColonneNome("Tipologia", "Tipologia", "string"))
            l.Add(New ColonneNome("sa_cod", "sa_cod", "string"))
            l.Add(New ColonneNome("Ditta_Des", "Marca", "string"))
            l.Add(New ColonneNome("Modello", "Modello", "string"))
            l.Add(New ColonneNome("Macchina", "Macchina", "string"))
            l.Add(New ColonneNome("Telaio", "Telaio", "string"))
            l.Add(New ColonneNome("Targa", "Targa", "string"))
            l.Add(New ColonneNome("Codice", "Codice", "string"))

            l.Add(New ColonneNome("Data_Creazione", "Data Creazione", "date"))
            l.Add(New ColonneNome("Utente_Creazione", "Utente Creazione", "string"))
            l.Add(New ColonneNome("Data_Modifica", "Data Modifica", "date"))
            l.Add(New ColonneNome("Utente_Modifica", "Utente Modifica", "string"))


            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            Dim risp As String = js.JSON_DataTable_Kendo(dt, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa)
            r.RispostaOK = True
            r.RispostaStringa = risp

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r

    End Function

    ' @Paolo
    ' Creo funzione che mi restituisce i dati da visualizzare per il percorso di selezione
    ' ---> Particella
    <WebMethod(EnableSession:=True)>
    Public Shared Function Dati_Relativi_Percorso_Selezione(ByVal tipo As Integer) As RispostaStandard
        Dim r As New RispostaStandard
        Try

            Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
            objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
            Dim objParametriAgenda As New ParametriAgenda
            Dim dt As DataTable

            Dim risp As New ArrayList

            Select Case tipo

                Case 4
                    ' campo
                    Dim objImprese As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
                    dt = objImprese.Leggi_x_anagrafica(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, "", "", objParametri_Server)

                    risp.Add("<div class='single_label'><img class="" single_label_img"" src="" ../AB_Immagini/icone24/x03_Centro.png""> " & dt.Rows(0).Item("sa_nome") & "</div>")

                Case 5
                    ' appezzamento
                    Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
                    dt = objCentri.Leggi_x_anagrafica(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, "", "", objParametri_Server)

                    risp.Add("<div class='single_label'><img class="" single_label_img"" src="" ../AB_Immagini/icone24/x03_Centro.png""> " & dt.Rows(0).Item("sa_nome") & "</div>")

                    If objParametriAgenda.Campo_Cod <> 0 Then
                        Dim objCampo As New AgronicaCoreAnagrafeDAL.Campi_R

                        Dim dt2 As DataTable = objCampo.Leggi(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, objParametriAgenda.Campo_Cod, "", "", "", objParametri_Server)

                        risp.Add("<div class='single_label'><img class="" single_label_img"" src="" ../AB_Immagini/icone24/x04_Campo.png""> " & dt2.Rows(0).Item("Campo_Des") & "</div>")
                    End If

                Case 6, 7, 8
                    ' impianto
                    Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
                    dt = objCentri.Leggi_x_anagrafica(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, "", "", objParametri_Server)

                    risp.Add("<div class='single_label'><img class="" single_label_img"" src="" ../AB_Immagini/icone24/x03_Centro.png""> " & dt.Rows(0).Item("sa_nome") & "</div>")

                    If objParametriAgenda.Campo_Cod <> 0 Then
                        Dim objCampo As New AgronicaCoreAnagrafeDAL.Campi_R

                        Dim dt2 As DataTable = objCampo.Leggi(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, objParametriAgenda.Campo_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

                        risp.Add("<div class='single_label'><img class="" single_label_img"" src="" ../AB_Immagini/icone24/x04_Campo.png""> " & dt2.Rows(0).Item("Campo_Des") & "</div>")
                    End If

                    Dim objAppezza As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
                    Dim dt3 As DataTable = objAppezza.Leggi(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, objParametriAgenda.Appezza, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

                    risp.Add("<div class='single_label'><img class="" single_label_img"" src="" ../AB_Immagini/icone24/x05_Appezzamento.png""> " & dt3.Rows(0).Item("APP_NOME") & "</div>")

                Case 10
                    ' particella
                    Dim objImprese As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
                    dt = objImprese.Leggi_x_anagrafica(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, "", "", objParametri_Server)

                    risp.Add("<div class='single_label'><img class="" single_label_img"" src="" ../AB_Immagini/icone24/x03_Centro.png""> " & dt.Rows(0).Item("sa_nome") & "</div>")

                Case 22
                    ' fabbricato
                    Dim objImprese As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
                    dt = objImprese.Leggi_x_anagrafica(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, "", "", objParametri_Server)

                    risp.Add("<div class='single_label'><img class="" single_label_img"" src="" ../AB_Immagini/icone24/x03_Centro.png""> " & dt.Rows(0).Item("sa_nome") & "</div>")

            End Select


            Dim myArr As String() = New String() {}
            myArr = CType(risp.ToArray(GetType(String)), String())
            risp.Clear()
            r.RispostaOK = True
            r.RispostaStringa = Newtonsoft.Json.JsonConvert.SerializeObject(myArr)

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Dati_Relativi_Percorso_Selezione2(ByVal tipo As Integer) As RispostaStandard
        Dim r As New RispostaStandard
        Try

            Dim objParametri_Server As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
            Dim objParametri_Utenti As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))
            Dim objParametriAgenda As New ParametriAgenda
            Dim dt As DataTable

            If objParametriAgenda.Sa_Cod = -1 Then
                objParametriAgenda.Sa_Cod = 0
            End If

            Dim risp As New ArrayList
            Dim id As String = ""

            Select Case tipo
                Case 1
                    ' Impresa
                    id = "2§" & objParametriAgenda.Piva & "§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§§0§0"
                    Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
                    If objParametriAgenda.Piva <> "" Then
                        dt = objImprese.Leggi_x_anagrafica(objParametriAgenda.Piva, "", "", objParametri_Server)
                        If dt.Rows.Count > 0 Then
                            risp.Add("<li class='breadcrumb-item'><span class='badge'><img class="" single_label_img"" src="" ../AB_Immagini/icone24/x24 - 256 - Impresa.bmp""> " & dt.Rows(0).Item("Rag_Soc") & "</span></li>")
                        End If
                    End If

                Case 2
                    ' Centro
                    id = "3§" & objParametriAgenda.Piva & "§" & objParametriAgenda.Sa_Cod & "§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§§0§0"
                    Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
                    If objParametriAgenda.Piva <> "" Then
                        dt = objImprese.Leggi_x_anagrafica(objParametriAgenda.Piva, "", "", objParametri_Server)
                        If dt.Rows.Count > 0 Then
                            risp.Add("<li class='breadcrumb-item'><span class='badge'><img class="" single_label_img"" src="" ../AB_Immagini/icone24/x24 - 256 - Impresa.bmp""> " & dt.Rows(0).Item("Rag_Soc") & "</span></li>")
                        End If
                    End If
                    Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
                    If objParametriAgenda.Sa_Cod <> 0 Then
                        dt = objCentri.Leggi_x_anagrafica(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, "", "", objParametri_Server)
                        If dt.Rows.Count > 0 Then
                            risp.Add("<li class='breadcrumb-item'><span class='badge'><img class="" single_label_img"" src="" ../AB_Immagini/icone24/x03_Centro.png""> " & dt.Rows(0).Item("sa_nome") & "<button onClick='eliminaFiltro(2)' type='button' class='close' aria-label='Close'><small><sup><i class='fa fa-times fa-1' aria-hidden='True'></sup></small></i></button></span></li>")
                        End If
                    End If
                Case 3
                    ' campo
                    id = "4§" & objParametriAgenda.Piva & "§" & objParametriAgenda.Sa_Cod & "§" & objParametriAgenda.Campo_Cod & "§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§§0§0"
                    Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
                    If objParametriAgenda.Piva <> "" Then
                        dt = objImprese.Leggi_x_anagrafica(objParametriAgenda.Piva, "", "", objParametri_Server)
                        If dt.Rows.Count > 0 Then
                            risp.Add("<li class='breadcrumb-item'><span class='badge'><img class="" single_label_img"" src="" ../AB_Immagini/icone24/x24 - 256 - Impresa.bmp""> " & dt.Rows(0).Item("Rag_Soc") & "</span></li>")
                        End If

                    End If
                    Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
                    If objParametriAgenda.Sa_Cod <> 0 Then
                        dt = objCentri.Leggi_x_anagrafica(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, "", "", objParametri_Server)
                        If dt.Rows.Count > 0 Then
                            risp.Add("<li class='breadcrumb-item'><span class='badge'><img class="" single_label_img"" src="" ../AB_Immagini/icone24/x03_Centro.png""> " & dt.Rows(0).Item("sa_nome") & "<button onClick='eliminaFiltro(2)' type='button' class='close' aria-label='Close'><small><sup><i class='fa fa-times fa-1' aria-hidden='True'></i></sup></small></button></span></li>")
                        End If

                    End If
                    Dim objCampi As New AgronicaCoreAnagrafeDAL.Campi_R
                    If objParametriAgenda.Campo_Cod <> 0 Then
                        dt = objCampi.Leggi_x_anagrafica(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, objParametriAgenda.Campo_Cod, "", "", objParametri_Server)
                        If dt.Rows.Count > 0 Then
                            risp.Add("<li class='breadcrumb-item'><span class='badge'><img class="" single_label_img"" src="" ../AB_Immagini/icone24/CampoNew2.ico""> " & dt.Rows(0).Item("Campo") & "<button onClick='eliminaFiltro(3)' type='button' class='close' aria-label='Close'><small><sup><i class='fa fa-times fa-1' aria-hidden='True'></i></sup></small></button></span></li>")
                        End If

                    End If
                Case 4
                    ' appezzamento
                    id = "5§" & objParametriAgenda.Piva & "§" & objParametriAgenda.Sa_Cod & "§" & objParametriAgenda.Campo_Cod & "§" & objParametriAgenda.Appezza & "§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§§0§0"
                    Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
                    If objParametriAgenda.Piva <> "" Then
                        dt = objImprese.Leggi_x_anagrafica(objParametriAgenda.Piva, "", "", objParametri_Server)
                        If dt.Rows.Count > 0 Then
                            risp.Add("<li class='breadcrumb-item'><span class='badge'><img class="" single_label_img"" src="" ../AB_Immagini/icone24/x24 - 256 - Impresa.bmp""> " & dt.Rows(0).Item("Rag_Soc") & "</span></li>")
                        End If

                    End If
                    Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
                    If objParametriAgenda.Sa_Cod <> 0 Then
                        dt = objCentri.Leggi_x_anagrafica(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, "", "", objParametri_Server)
                        If dt.Rows.Count > 0 Then
                            risp.Add("<li class='breadcrumb-item'><span class='badge'><img class="" single_label_img"" src="" ../AB_Immagini/icone24/x03_Centro.png""> " & dt.Rows(0).Item("sa_nome") & "<button onClick='eliminaFiltro(2)' type='button' class='close' aria-label='Close'><small><sup><i class='fa fa-times fa-1' aria-hidden='True'></i></sup></small></button></span></li>")
                        End If

                    End If
                    Dim objCampi As New AgronicaCoreAnagrafeDAL.Campi_R
                    If objParametriAgenda.Campo_Cod <> 0 Then
                        dt = objCampi.Leggi_x_anagrafica(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, objParametriAgenda.Campo_Cod, "", "", objParametri_Server)
                        If dt.Rows.Count > 0 Then
                            risp.Add("<li class='breadcrumb-item'><span class='badge'><img class="" single_label_img"" src="" ../AB_Immagini/icone24/CampoNew2.ico""> " & dt.Rows(0).Item("Campo") & "<button onClick='eliminaFiltro(3)' type='button' class='close' aria-label='Close'><small><sup><i class='fa fa-times fa-1' aria-hidden='True'></i></sup></small></button></span></li>")
                        End If

                    End If

                    Dim objAppezza As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
                    If objParametriAgenda.Appezza <> "" AndAlso objParametriAgenda.Appezza <> 0 Then
                        dt = objAppezza.Leggi_x_anagrafica(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, objParametriAgenda.Campo_Cod, objParametriAgenda.Appezza, "", "", objParametri_Server)
                        If dt.Rows.Count > 0 Then
                            risp.Add("<li class='breadcrumb-item'><span class='badge'><img class="" single_label_img"" src="" ../AB_Immagini/icone24/Anagrafica_24_Appezzamento.ico""> " & dt.Rows(0).Item("APP_NOME") & "<button onClick='eliminaFiltro(4)' type='button' class='close' aria-label='Close'><small><sup><i class='fa fa-times fa-1' aria-hidden='True'></i></sup></small></button></span></li>")
                        End If

                    End If

                Case 5
                    ' impianto
                    id = "7§" & objParametriAgenda.Piva & "§" & objParametriAgenda.Sa_Cod & "§" & objParametriAgenda.Campo_Cod & "§" & objParametriAgenda.Appezza & "§" & objParametriAgenda.Id_Imp & "§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§§0§0"
                    Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
                    If objParametriAgenda.Piva <> "" Then
                        dt = objImprese.Leggi_x_anagrafica(objParametriAgenda.Piva, "", "", objParametri_Server)
                        If dt.Rows.Count > 0 Then
                            risp.Add("<li class='breadcrumb-item'><span class='badge'><img class="" single_label_img"" src="" ../AB_Immagini/icone24/x24 - 256 - Impresa.bmp""> " & dt.Rows(0).Item("Rag_Soc") & "</span></li>")
                        End If
                    End If
                    Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
                    If objParametriAgenda.Sa_Cod <> 0 Then
                        dt = objCentri.Leggi_x_anagrafica(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, "", "", objParametri_Server)
                        If dt.Rows.Count > 0 Then
                            risp.Add("<li class='breadcrumb-item'><span class='badge'><img class="" single_label_img"" src="" ../AB_Immagini/icone24/x03_Centro.png""> " & dt.Rows(0).Item("sa_nome") & "<button onClick='eliminaFiltro(2)' type='button' class='close' aria-label='Close'><small><sup><i class='fa fa-times fa-1' aria-hidden='True'></i></sup></small></button></span></li>")
                        End If
                    End If
                    Dim objCampi As New AgronicaCoreAnagrafeDAL.Campi_R
                    If objParametriAgenda.Campo_Cod <> 0 Then
                        dt = objCampi.Leggi_x_anagrafica(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, objParametriAgenda.Campo_Cod, "", "", objParametri_Server)
                        If dt.Rows.Count > 0 Then
                            risp.Add("<li class='breadcrumb-item'><span class='badge'><img class="" single_label_img"" src="" ../AB_Immagini/icone24/CampoNew2.ico""> " & dt.Rows(0).Item("Campo") & "<button onClick='eliminaFiltro(3)' type='button' class='close' aria-label='Close'><small><sup><i class='fa fa-times fa-1' aria-hidden='True'></i></sup></small></button></span></li>")
                        End If
                    End If

                    Dim objAppezza As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
                    If objParametriAgenda.Appezza <> "" AndAlso objParametriAgenda.Appezza <> 0 Then
                        dt = objAppezza.Leggi_x_anagrafica(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, objParametriAgenda.Campo_Cod, objParametriAgenda.Appezza, "", "", objParametri_Server)
                        If dt.Rows.Count > 0 Then
                            risp.Add("<li class='breadcrumb-item'><span class='badge'><img class="" single_label_img"" src="" ../AB_Immagini/icone24/Anagrafica_24_Appezzamento.ico""> " & dt.Rows(0).Item("APP_NOME") & "<button onClick='eliminaFiltro(4)' type='button' class='close' aria-label='Close'><small><sup><i class='fa fa-times fa-1' aria-hidden='True'></i></sup></small></button></span></li>")
                        End If
                    End If

                    Dim objImp As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                    If objParametriAgenda.Id_Imp <> "" AndAlso objParametriAgenda.Id_Imp <> "null" AndAlso objParametriAgenda.Id_Imp <> 0 Then
                        dt = objImp.Leggi_x_anagrafica2(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, objParametriAgenda.Appezza, objParametriAgenda.Id_Imp, objParametriAgenda.Campo_Cod, "", "", objParametri_Server)
                        If dt.Rows.Count > 0 Then
                            risp.Add("<li class='breadcrumb-item'><span class='badge'><img class="" single_label_img"" src="" ../AB_Immagini/icone24/Anagrafica_24_Impianto.ico""> " & dt.Rows(0).Item("veg_des") & "-" & dt.Rows(0).Item("cul_des") & "<button onClick='eliminaFiltro(5)' type='button' class='close' aria-label='Close'><small><sup><i class='fa fa-times fa-1' aria-hidden='True'></i></sup></small></button></span></li>")
                        End If
                    End If

                Case 6
                    ' particella
                    Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
                    If objParametriAgenda.Piva <> "" Then
                        dt = objImprese.Leggi_x_anagrafica(objParametriAgenda.Piva, "", "", objParametri_Server)
                        If dt.Rows.Count > 0 Then
                            risp.Add("<li class='breadcrumb-item'><span class='badge'><img class="" single_label_img"" src="" ../AB_Immagini/icone24/x24 - 256 - Impresa.bmp""> " & dt.Rows(0).Item("Rag_Soc") & "</span></li>")
                        End If

                    End If
                    Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
                    If objParametriAgenda.Sa_Cod <> 0 Then
                        dt = objCentri.Leggi_x_anagrafica(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, "", "", objParametri_Server)
                        If dt.Rows.Count > 0 Then
                            risp.Add("<li class='breadcrumb-item'><span class='badge'><img class="" single_label_img"" src="" ../AB_Immagini/icone24/x03_Centro.png""> " & dt.Rows(0).Item("sa_nome") & "<button onClick='eliminaFiltro(2)' type='button' class='close' aria-label='Close'><small><sup><i class='fa fa-times fa-1' aria-hidden='True'></i></sup></small></button></span></li>")
                        End If
                    End If
                    If objParametriAgenda.Particelle.Count > 0 Then
                        Dim particella = objParametriAgenda.Particelle.FirstOrDefault
                        id = "10§" & objParametriAgenda.Piva & "§" & objParametriAgenda.Sa_Cod & "§0§0§0§" & particella.Part_Cod & "§" & particella.Provincia & "§" & particella.Comune & "§" & particella.Sezione & "§" & particella.Foglio & "§" & particella.Numero & "§" & particella.Subalterno & "§0§0§0§0§0§0§0§0§0§0§0§0§0§§0§0"
                    End If
                Case 7
                    ' Raggruppamento stalla
                    Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
                    If objParametriAgenda.Piva <> "" Then
                        dt = objImprese.Leggi_x_anagrafica(objParametriAgenda.Piva, "", "", objParametri_Server)
                        If dt.Rows.Count > 0 Then
                            risp.Add("<li class='breadcrumb-item'><span class='badge'><img class="" single_label_img"" src="" ../AB_Immagini/icone24/x24 - 256 - Impresa.bmp""> " & dt.Rows(0).Item("Rag_Soc") & "</span></li>")
                        End If

                    End If
                    Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
                    If objParametriAgenda.Sa_Cod <> 0 Then
                        dt = objCentri.Leggi_x_anagrafica(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, "", "", objParametri_Server)
                        If dt.Rows.Count > 0 Then
                            risp.Add("<li class='breadcrumb-item'><span class='badge'><img class="" single_label_img"" src="" ../AB_Immagini/icone24/x03_Centro.png""> " & dt.Rows(0).Item("sa_nome") & "<button onClick='eliminaFiltro(2)' type='button' class='close' aria-label='Close'><small><sup><i class='fa fa-times fa-1' aria-hidden='True'></i></sup></small></button></span></li>")
                        End If

                    End If
                    Dim objFabbr As New AgronicaCoreAnagrafeDAL.Fabbricati_R
                    dt = objFabbr.Leggi(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, objParametriAgenda.Fabbricato, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                    If objParametriAgenda.Fabbricato <> "" And objParametriAgenda.Fabbricato <> 0 And dt.Rows.Count > 0 Then
                        risp.Add("<li class='breadcrumb-item'><span class='badge'><img class="" single_label_img"" src="" ../AB_Immagini/icone24/x03_Centro.png""> " & dt.Rows(0).Item("Fabbricato_des") & "<button onClick='eliminaFiltro(2)' type='button' class='close' aria-label='Close'><small><sup><i class='fa fa-times fa-1' aria-hidden='True'></i></sup></small></button></span></li>")
                    End If

            End Select

            ' se attivo seleziono l'elemento dell'albero anagrafica
            Dim Leggi_impostazioni As New Utenti_Impostazioni_Read
            Dim config_albero = Leggi_impostazioni.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.SuperUser_Albero_Anagrafiche, objParametri_Utenti, 2)
            If config_albero <> "" AndAlso config_albero <> "0" AndAlso Not String.IsNullOrEmpty(id) Then
                risp.Add("<script>SelezionaAlberoAnagrafica('" & id & "');</script>")
            End If

            Dim myArr As String() = New String() {}
            myArr = CType(risp.ToArray(GetType(String)), String())
            risp.Clear()
            r.RispostaOK = True
            r.RispostaStringa = Newtonsoft.Json.JsonConvert.SerializeObject(myArr)

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Dati_Relativi_Percorso_Selezione3(ByVal tipo As Integer,
                                                             ByVal visibilita As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Try

            Dim objParametri_Server As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
            Dim objParametri_Utenti As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim objParametriAgenda As New ParametriAgenda
            Dim dt As DataTable

            If objParametriAgenda.Sa_Cod = -1 Then
                objParametriAgenda.Sa_Cod = 0
            End If

            Dim risp As New ArrayList
            Dim id As String = ""
            Dim dimension = ""

            Select Case tipo
                Case 1
                    ' Impresa
                    id = "2§" & objParametriAgenda.Piva & "§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§§0§0"
                    Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
                    If objParametriAgenda.Piva <> "" Then
                        dt = objImprese.Leggi_x_anagrafica(objParametriAgenda.Piva, "", "", objParametri_Server)

                        If dt.Rows.Count > 0 Then
                            If visibilita = 2 Then
                                risp.Add("<li class='breadcrumb-item'><span class='badge'><p><i class='fa fa-industry " & dimension & "' aria-hidden='true'></i> " & AgronicaAgenda_2010.FiltroAzienda & ": " & AgronicaAgenda_2010.Nessuno & "</p></span></li>")
                            Else
                                risp.Add("<li class='breadcrumb-item'><span class='badge'><p><i class='fa fa-industry " & dimension & "' aria-hidden='true'></i> " & dt.Rows(0).Item("Rag_Soc") & "</p></span></li>")
                            End If
                        End If
                    End If

                Case 2
                    ' Centro
                    id = "3§" & objParametriAgenda.Piva & "§" & objParametriAgenda.Sa_Cod & "§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§§0§0"
                    Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
                    If objParametriAgenda.Piva <> "" Then
                        dt = objImprese.Leggi_x_anagrafica(objParametriAgenda.Piva, "", "", objParametri_Server)

                        If dt.Rows.Count > 0 Then
                            If visibilita = 2 Then
                                risp.Add("<li class='breadcrumb-item'><span class='badge'><p><i class='fa fa-industry " & dimension & "' aria-hidden='true'></i> " & AgronicaAgenda_2010.FiltroAzienda & ":</p></span></li>")
                            Else
                                risp.Add("<li class='breadcrumb-item'><span class='badge'><p><i class='fa fa-industry " & dimension & "' aria-hidden='true'></i> " & dt.Rows(0).Item("Rag_Soc") & "</p></span></li>")
                            End If
                        End If
                    End If
                    Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
                    If objParametriAgenda.Sa_Cod <> 0 Then
                        dt = objCentri.Leggi_x_anagrafica(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, "", "", objParametri_Server)

                        If dt.Rows.Count > 0 Then
                            risp.Add("<li class='breadcrumb-item'><span class='badge'><p><i class='fa fa-building " & dimension & "' aria-hidden='true'></i> " & dt.Rows(0).Item("sa_nome") & "<button onClick='eliminaFiltro(2)' type='button' class='close' aria-label='Close'><small><sup><i class='fa fa-times fa-1' aria-hidden='True'></sup></small></i></button></p></span></li>")
                        Else
                            If visibilita = 2 Then
                                risp(0) = "<li class='breadcrumb-item'><span class='badge'><p><i class='fa fa-industry " & dimension & "' aria-hidden='true'></i> " & AgronicaAgenda_2010.FiltroAzienda & ": " & AgronicaAgenda_2010.Nessuno & "</p></span></li>"
                            End If
                        End If
                    Else
                        If visibilita = 2 Then
                            risp(0) = "<li class='breadcrumb-item'><span class='badge'><p><i class='fa fa-industry " & dimension & "' aria-hidden='true'></i> " & AgronicaAgenda_2010.FiltroAzienda & ": " & AgronicaAgenda_2010.Nessuno & "</p></span></li>"
                        End If
                    End If
                Case 3
                    ' campo
                    id = "4§" & objParametriAgenda.Piva & "§" & objParametriAgenda.Sa_Cod & "§" & objParametriAgenda.Campo_Cod & "§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§§0§0"
                    Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
                    If objParametriAgenda.Piva <> "" Then
                        dt = objImprese.Leggi_x_anagrafica(objParametriAgenda.Piva, "", "", objParametri_Server)
                        If dt.Rows.Count > 0 Then
                            risp.Add("<li class='breadcrumb-item'><span class='badge'><p><i class='fa fa-industry " & dimension & "' aria-hidden='true'></i> " & dt.Rows(0).Item("Rag_Soc") & "</span></li>")
                        End If

                    End If
                    Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
                    If objParametriAgenda.Sa_Cod <> 0 Then
                        dt = objCentri.Leggi_x_anagrafica(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, "", "", objParametri_Server)
                        If dt.Rows.Count > 0 Then
                            risp.Add("<li class='breadcrumb-item'><span class='badge'><p><i class='fa fa-building " & dimension & "' aria-hidden='true'></i> " & dt.Rows(0).Item("sa_nome") & "<button onClick='eliminaFiltro(2)' type='button' class='close' aria-label='Close'><small><sup><i class='fa fa-times fa-1' aria-hidden='True'></i></sup></small></button></p></span></li>")
                        End If

                    End If
                    Dim objCampi As New AgronicaCoreAnagrafeDAL.Campi_R
                    If objParametriAgenda.Campo_Cod <> 0 Then
                        dt = objCampi.Leggi_x_anagrafica(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, objParametriAgenda.Campo_Cod, "", "", objParametri_Server)
                        If dt.Rows.Count > 0 Then
                            risp.Add("<li class='breadcrumb-item'><span class='badge'><p><i class='fa fa-object-ungroup " & dimension & "' aria-hidden='true'></i> " & dt.Rows(0).Item("Campo") & "<button onClick='eliminaFiltro(3)' type='button' class='close' aria-label='Close'><small><sup><i class='fa fa-times fa-1' aria-hidden='True'></i></sup></small></button></p></span></li>")
                        End If

                    End If
                Case 4
                    ' appezzamento
                    id = "5§" & objParametriAgenda.Piva & "§" & objParametriAgenda.Sa_Cod & "§" & objParametriAgenda.Campo_Cod & "§" & objParametriAgenda.Appezza & "§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§§0§0"
                    Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
                    If objParametriAgenda.Piva <> "" Then
                        dt = objImprese.Leggi_x_anagrafica(objParametriAgenda.Piva, "", "", objParametri_Server)
                        If dt.Rows.Count > 0 Then
                            risp.Add("<li class='breadcrumb-item'><span class='badge'><p><i class='fa fa-industry " & dimension & "' aria-hidden='true'></i> " & dt.Rows(0).Item("Rag_Soc") & "</span></li>")
                        End If

                    End If
                    Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
                    If objParametriAgenda.Sa_Cod <> 0 Then
                        dt = objCentri.Leggi_x_anagrafica(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, "", "", objParametri_Server)
                        If dt.Rows.Count > 0 Then
                            risp.Add("<li class='breadcrumb-item'><span class='badge'><p><i class='fa fa-building " & dimension & "' aria-hidden='true'></i> " & dt.Rows(0).Item("sa_nome") & "<button onClick='eliminaFiltro(2)' type='button' class='close' aria-label='Close'><small><sup><i class='fa fa-times fa-1' aria-hidden='True'></i></sup></small></button></p></span></li>")
                        End If

                    End If
                    Dim objCampi As New AgronicaCoreAnagrafeDAL.Campi_R
                    If objParametriAgenda.Campo_Cod <> 0 Then
                        dt = objCampi.Leggi_x_anagrafica(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, objParametriAgenda.Campo_Cod, "", "", objParametri_Server)
                        If dt.Rows.Count > 0 Then
                            risp.Add("<li class='breadcrumb-item'><span class='badge'><p><i class='fa fa-object-ungroup " & dimension & "' aria-hidden='true'></i> " & dt.Rows(0).Item("Campo") & "<button onClick='eliminaFiltro(3)' type='button' class='close' aria-label='Close'><small><sup><i class='fa fa-times fa-1' aria-hidden='True'></i></sup></small></button></p></span></li>")
                        End If

                    End If

                    Dim objAppezza As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
                    If objParametriAgenda.Appezza <> "" AndAlso objParametriAgenda.Appezza <> 0 Then
                        dt = objAppezza.Leggi_x_anagrafica(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, objParametriAgenda.Campo_Cod, objParametriAgenda.Appezza, "", "", objParametri_Server)
                        If dt.Rows.Count > 0 Then
                            risp.Add("<li class='breadcrumb-item'><span class='badge'><p><i class='fa fa-map " & dimension & "' aria-hidden='true'></i> " & dt.Rows(0).Item("APP_NOME") & "<button onClick='eliminaFiltro(4)' type='button' class='close' aria-label='Close'><small><sup><i class='fa fa-times fa-1' aria-hidden='True'></i></sup></small></button></p></span></li>")
                        End If

                    End If

                Case 5
                    ' impianto
                    id = "7§" & objParametriAgenda.Piva & "§" & objParametriAgenda.Sa_Cod & "§" & objParametriAgenda.Campo_Cod & "§" & objParametriAgenda.Appezza & "§" & objParametriAgenda.Id_Imp & "§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§§0§0"
                    Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
                    If objParametriAgenda.Piva <> "" Then
                        dt = objImprese.Leggi_x_anagrafica(objParametriAgenda.Piva, "", "", objParametri_Server)
                        If dt.Rows.Count > 0 Then
                            risp.Add("<li class='breadcrumb-item'><span class='badge'><p><i class='fa fa-industry " & dimension & "' aria-hidden='true'></i> " & dt.Rows(0).Item("Rag_Soc") & "</span></li>")
                        End If
                    End If
                    Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
                    If objParametriAgenda.Sa_Cod <> 0 Then
                        dt = objCentri.Leggi_x_anagrafica(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, "", "", objParametri_Server)
                        If dt.Rows.Count > 0 Then
                            risp.Add("<li class='breadcrumb-item'><span class='badge'><p><i class='fa fa-building " & dimension & "' aria-hidden='true'></i> " & dt.Rows(0).Item("sa_nome") & "<button onClick='eliminaFiltro(2)' type='button' class='close' aria-label='Close'><small><sup><i class='fa fa-times fa-1' aria-hidden='True'></i></sup></small></button></p></span></li>")
                        End If
                    End If
                    Dim objCampi As New AgronicaCoreAnagrafeDAL.Campi_R
                    If objParametriAgenda.Campo_Cod <> 0 Then
                        dt = objCampi.Leggi_x_anagrafica(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, objParametriAgenda.Campo_Cod, "", "", objParametri_Server)
                        If dt.Rows.Count > 0 Then
                            risp.Add("<li class='breadcrumb-item'><span class='badge'><p><i class='fa fa-object-ungroup " & dimension & "' aria-hidden='true'></i> " & dt.Rows(0).Item("Campo") & "<button onClick='eliminaFiltro(3)' type='button' class='close' aria-label='Close'><small><sup><i class='fa fa-times fa-1' aria-hidden='True'></i></sup></small></button></p></span></li>")
                        End If
                    End If

                    Dim objAppezza As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
                    If objParametriAgenda.Appezza <> "" AndAlso objParametriAgenda.Appezza <> 0 Then
                        dt = objAppezza.Leggi_x_anagrafica(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, objParametriAgenda.Campo_Cod, objParametriAgenda.Appezza, "", "", objParametri_Server)
                        If dt.Rows.Count > 0 Then
                            risp.Add("<li class='breadcrumb-item'><span class='badge'><p><i class='fa fa-map " & dimension & "' aria-hidden='true'></i> " & dt.Rows(0).Item("APP_NOME") & "<button onClick='eliminaFiltro(4)' type='button' class='close' aria-label='Close'><small><sup><i class='fa fa-times fa-1' aria-hidden='True'></i></sup></small></button></p></span></li>")
                        End If
                    End If

                    Dim objImp As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                    If objParametriAgenda.Id_Imp <> "" AndAlso objParametriAgenda.Id_Imp <> "null" AndAlso objParametriAgenda.Id_Imp <> 0 Then
                        dt = objImp.Leggi_x_anagrafica2(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, objParametriAgenda.Appezza, objParametriAgenda.Id_Imp, objParametriAgenda.Campo_Cod, "", "", objParametri_Server)
                        If dt.Rows.Count > 0 Then
                            risp.Add("<li class='breadcrumb-item'><span class='badge'><p><i class='fa fa-lemon-o " & dimension & "' aria-hidden='true'></i> " & dt.Rows(0).Item("utilizzo") & "<button onClick='eliminaFiltro(5)' type='button' class='close' aria-label='Close'><small><sup><i class='fa fa-times fa-1' aria-hidden='True'></i></sup></small></button></p></span></li>")
                        End If
                    End If

                Case 6
                    ' particella
                    Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
                    If objParametriAgenda.Piva <> "" Then
                        dt = objImprese.Leggi_x_anagrafica(objParametriAgenda.Piva, "", "", objParametri_Server)
                        If dt.Rows.Count > 0 Then
                            risp.Add("<li class='breadcrumb-item'><span class='badge'><p><i class='fa fa-industry " & dimension & "' aria-hidden='true'></i> " & dt.Rows(0).Item("Rag_Soc") & "</span></li>")
                        End If

                    End If
                    Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
                    If objParametriAgenda.Sa_Cod <> 0 Then
                        dt = objCentri.Leggi_x_anagrafica(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, "", "", objParametri_Server)
                        If dt.Rows.Count > 0 Then
                            risp.Add("<li class='breadcrumb-item'><span class='badge'><p><i class='fa fa-building " & dimension & "' aria-hidden='true'></i> " & dt.Rows(0).Item("sa_nome") & "<button onClick='eliminaFiltro(2)' type='button' class='close' aria-label='Close'><small><sup><i class='fa fa-times fa-1' aria-hidden='True'></i></sup></small></button></p></span></li>")
                        End If
                    End If
                    If objParametriAgenda.Particelle.Count > 0 Then
                        Dim particella = objParametriAgenda.Particelle.FirstOrDefault
                        id = "10§" & objParametriAgenda.Piva & "§" & objParametriAgenda.Sa_Cod & "§0§0§0§" & particella.Part_Cod & "§" & particella.Provincia & "§" & particella.Comune & "§" & particella.Sezione & "§" & particella.Foglio & "§" & particella.Numero & "§" & particella.Subalterno & "§0§0§0§0§0§0§0§0§0§0§0§0§0§§0§0"
                    End If
                Case 7
                    'Stalla
                    Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
                    If objParametriAgenda.Piva <> "" Then
                        dt = objImprese.Leggi_x_anagrafica(objParametriAgenda.Piva, "", "", objParametri_Server)
                        If dt.Rows.Count > 0 Then
                            If visibilita = 2 Then
                                risp.Add("<li class='breadcrumb-item'><span class='badge'><p><i class='fa fa-industry " & dimension & "' aria-hidden='true'></i> " & AgronicaAgenda_2010.FiltroAzienda & ":</p></span></li>")
                            Else
                                risp.Add("<li class='breadcrumb-item'><span class='badge'><p><i class='fa fa-industry " & dimension & "' aria-hidden='true'></i> " & dt.Rows(0).Item("Rag_Soc") & "</span></li>")
                            End If
                        End If
                    End If

                    Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
                    If objParametriAgenda.Sa_Cod <> 0 Then
                        dt = objCentri.Leggi_x_anagrafica(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, "", "", objParametri_Server)
                        If dt.Rows.Count > 0 Then
                            risp.Add("<li class='breadcrumb-item'><span class='badge'><p><i class='fa fa-building " & dimension & "' aria-hidden='true'></i> " & dt.Rows(0).Item("sa_nome") & "<button onClick='eliminaFiltro(2)' type='button' class='close' aria-label='Close'><small><sup><i class='fa fa-times fa-1' aria-hidden='True'></i></sup></small></button></p></span></li>")
                        Else
                            If visibilita = 2 Then
                                risp(0) = "<li class='breadcrumb-item'><span class='badge'><p><i class='fa fa-industry " & dimension & "' aria-hidden='true'></i> " & AgronicaAgenda_2010.FiltroAzienda & ": " & AgronicaAgenda_2010.Nessuno & "</p></span></li>"
                            End If
                        End If
                    Else
                        If visibilita = 2 Then
                            risp(0) = "<li class='breadcrumb-item'><span class='badge'><p><i class='fa fa-industry " & dimension & "' aria-hidden='true'></i> " & AgronicaAgenda_2010.FiltroAzienda & ": " & AgronicaAgenda_2010.Nessuno & "</p></span></li>"
                        End If

                    End If

                    Dim objFabbr As New AgronicaCoreAnagrafeDAL.Fabbricati_R
                    If objParametriAgenda.Fabbricato = "" Then
                        objParametriAgenda.Fabbricato = 0
                    End If
                    dt = objFabbr.Leggi(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, objParametriAgenda.Fabbricato, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                    If objParametriAgenda.Fabbricato <> "" And objParametriAgenda.Fabbricato <> 0 And dt.Rows.Count > 0 Then
                        risp.Add("<li class='breadcrumb-item'><span class='badge'><p><i class='fa fa-building " & dimension & "' aria-hidden='true'></i> " & dt.Rows(0).Item("Fabbricato_des") & "<button onClick='eliminaFiltro(7)' type='button' class='close' aria-label='Close'><small><sup><i class='fa fa-times fa-1' aria-hidden='True'></i></sup></small></button></p></span></li>")

                    End If
                Case 8
                    'Raggruppamento stalla
                    Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
                    If objParametriAgenda.Piva <> "" Then
                        dt = objImprese.Leggi_x_anagrafica(objParametriAgenda.Piva, "", "", objParametri_Server)
                        If dt.Rows.Count > 0 Then
                            If visibilita = 2 Then
                                risp.Add("<li class='breadcrumb-item'><span class='badge'><p><i class='fa fa-industry " & dimension & "' aria-hidden='true'></i> " & AgronicaAgenda_2010.FiltroAzienda & ":</p></span></li>")
                            Else
                                risp.Add("<li class='breadcrumb-item'><span class='badge'><p><i class='fa fa-industry " & dimension & "' aria-hidden='true'></i> " & dt.Rows(0).Item("Rag_Soc") & "</span></li>")
                            End If
                        End If
                    End If

                    Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
                    If objParametriAgenda.Sa_Cod <> 0 Then
                        dt = objCentri.Leggi_x_anagrafica(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, "", "", objParametri_Server)
                        If dt.Rows.Count > 0 Then
                            risp.Add("<li class='breadcrumb-item'><span class='badge'><p><i class='fa fa-building " & dimension & "' aria-hidden='true'></i> " & dt.Rows(0).Item("sa_nome") & "<button onClick='eliminaFiltro(2)' type='button' class='close' aria-label='Close'><small><sup><i class='fa fa-times fa-1' aria-hidden='True'></i></sup></small></button></p></span></li>")
                        Else
                            If visibilita = 2 Then
                                risp(0) = "<li class='breadcrumb-item'><span class='badge'><p><i class='fa fa-industry " & dimension & "' aria-hidden='true'></i> " & AgronicaAgenda_2010.FiltroAzienda & ": " & AgronicaAgenda_2010.Nessuno & "</p></span></li>"
                            End If
                        End If
                    Else
                        If visibilita = 2 Then
                            risp(0) = "<li class='breadcrumb-item'><span class='badge'><p><i class='fa fa-industry " & dimension & "' aria-hidden='true'></i> " & AgronicaAgenda_2010.FiltroAzienda & ": " & AgronicaAgenda_2010.Nessuno & "</p></span></li>"
                        End If

                    End If

                    If objParametriAgenda.Fabbricato <> "" And objParametriAgenda.Fabbricato <> 0 Then
                        Dim objFabbr As New AgronicaCoreAnagrafeDAL.Fabbricati_R
                        dt = objFabbr.Leggi(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, objParametriAgenda.Fabbricato, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

                        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                            risp.Add("<li class='breadcrumb-item'><span class='badge'><p><i class='fa fa-building " & dimension & "' aria-hidden='true'></i> " & dt.Rows(0).Item("Fabbricato_des") & "<button onClick='eliminaFiltro(7)' type='button' class='close' aria-label='Close'><small><sup><i class='fa fa-times fa-1' aria-hidden='True'></i></sup></small></button></p></span></li>")
                        End If
                    End If

                    Dim objRaggr As New AgronicaCoreAnagrafeDAL.Stalla_Raggruppamenti_R
                    If objParametriAgenda.Raggruppamento_Cod <> "" And objParametriAgenda.Raggruppamento_Cod <> 0 Then
                        dt = objRaggr.Leggi_x_anagrafica(objParametri_Server.PivaSuperUser, objParametriAgenda.Piva, objParametriAgenda.Sa_Cod,
                                                         objParametriAgenda.Fabbricato, objParametriAgenda.Raggruppamento_Cod,
                                                         "", "", objParametri_Server)

                        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                            risp.Add("<li class='breadcrumb-item'><span class='badge'><p><i class='fa fa-building " & dimension & "' aria-hidden='true'></i> " & dt.Rows(0).Item("Raggruppamento_Des") & "<button onClick='eliminaFiltro(8)' type='button' class='close' aria-label='Close'><small><sup><i class='fa fa-times fa-1' aria-hidden='True'></i></sup></small></button></p></span></li>")
                        End If
                    End If

            End Select

            ' se attivo seleziono l'elemento dell'albero anagrafica
            Dim Leggi_impostazioni As New Utenti_Impostazioni_Read
            Dim config_albero = Leggi_impostazioni.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.SuperUser_Albero_Anagrafiche, objParametri_Utenti, 2)
            If config_albero <> "" AndAlso config_albero <> "0" AndAlso Not String.IsNullOrEmpty(id) Then
                risp.Add("<script>SelezionaAlberoAnagrafica('" & id & "');</script>")
            End If

            Dim myArr As String() = New String() {}
            myArr = CType(risp.ToArray(GetType(String)), String())
            risp.Clear()
            r.RispostaOK = True
            r.RispostaStringa = Newtonsoft.Json.JsonConvert.SerializeObject(myArr)

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r

    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function ImpostaObjP_Agenda(ByVal tipo As Integer, chiave As String, reset As Boolean) As RispostaStandard
        Dim r As New RispostaStandard
        Try
            Dim objParametriAgenda As New ParametriAgenda
            Select Case tipo
                Case 0
                    If reset Then
                        objParametriAgenda.Sa_Cod = 0
                        objParametriAgenda.Campo_Cod = 0
                        objParametriAgenda.Appezza = 0
                        objParametriAgenda.Id_Imp = 0
                        objParametriAgenda.Fabbricato = 0
                        objParametriAgenda.Raggruppamento_Cod = 0
                        objParametriAgenda.Impianti = New List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)
                        objParametriAgenda.Particelle = New List(Of Particella)
                        objParametriAgenda.GruppoOperazioneColturale = ""
                        objParametriAgenda.Cod_Contatto = ""
                        objParametriAgenda.Mac_Cod = 0
                    End If
                    objParametriAgenda.Data = If(String.IsNullOrEmpty(chiave), AGRODATAINIZIO, CDate(chiave))
                Case 1
                    If Not reset Then
                        objParametriAgenda.Piva = chiave
                    Else
                        objParametriAgenda.Piva = ""
                    End If
                    objParametriAgenda.Sa_Cod = 0
                    objParametriAgenda.Campo_Cod = 0
                    objParametriAgenda.Appezza = 0
                    objParametriAgenda.Id_Imp = 0
                    objParametriAgenda.Fabbricato = 0
                    objParametriAgenda.Raggruppamento_Cod = 0
                    objParametriAgenda.Impianti = New List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)
                    objParametriAgenda.Particelle = New List(Of Particella)
                    objParametriAgenda.GruppoOperazioneColturale = ""
                    objParametriAgenda.Cod_Contatto = ""
                    objParametriAgenda.Mac_Cod = 0
                Case 2
                    If Not reset Then
                        objParametriAgenda.Piva = chiave.Split("_")(0)
                    End If
                    If reset Then
                        objParametriAgenda.Sa_Cod = 0
                        'objParametriAgenda.Data = Date.Now
                    Else
                        objParametriAgenda.Sa_Cod = CInt(chiave.Split("_")(1))
                    End If
                    objParametriAgenda.Campo_Cod = 0
                    objParametriAgenda.Appezza = 0
                    objParametriAgenda.Id_Imp = 0
                    objParametriAgenda.Fabbricato = 0
                    objParametriAgenda.Raggruppamento_Cod = 0
                    objParametriAgenda.Impianti = New List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)
                    objParametriAgenda.Particelle = New List(Of Particella)
                    objParametriAgenda.GruppoOperazioneColturale = ""
                    objParametriAgenda.Cod_Contatto = ""
                    objParametriAgenda.Mac_Cod = 0
                Case 3
                    If Not reset Then
                        objParametriAgenda.Piva = chiave.Split("_")(0)
                        objParametriAgenda.Sa_Cod = CInt(chiave.Split("_")(1))
                    End If
                    If reset Then
                        objParametriAgenda.Campo_Cod = 0
                    Else
                        objParametriAgenda.Campo_Cod = CInt(chiave.Split("_")(2))
                    End If
                    objParametriAgenda.Appezza = 0
                    objParametriAgenda.Id_Imp = 0
                    objParametriAgenda.Fabbricato = 0
                    objParametriAgenda.Raggruppamento_Cod = 0
                    objParametriAgenda.Impianti = New List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)
                    objParametriAgenda.Particelle = New List(Of Particella)
                    objParametriAgenda.GruppoOperazioneColturale = ""
                    objParametriAgenda.Cod_Contatto = ""
                    objParametriAgenda.Mac_Cod = 0
                Case 4
                    If Not reset Then
                        objParametriAgenda.Piva = chiave.Split("_")(0)
                        objParametriAgenda.Sa_Cod = CInt(chiave.Split("_")(1))
                        If chiave.Split("_").Length > 3 Then
                            objParametriAgenda.Campo_Cod = CInt(chiave.Split("_")(3))
                        End If
                    End If
                    If reset Then
                        objParametriAgenda.Appezza = 0
                    Else
                        objParametriAgenda.Appezza = CInt(chiave.Split("_")(2))
                    End If
                    objParametriAgenda.Id_Imp = 0
                    objParametriAgenda.Fabbricato = 0
                    objParametriAgenda.Raggruppamento_Cod = 0
                    objParametriAgenda.Impianti = New List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)
                    objParametriAgenda.Particelle = New List(Of Particella)
                    objParametriAgenda.GruppoOperazioneColturale = ""
                    objParametriAgenda.Cod_Contatto = ""
                    objParametriAgenda.Mac_Cod = 0
                Case 5
                    If Not reset Then
                        objParametriAgenda.Piva = chiave.Split("_")(0)
                        objParametriAgenda.Sa_Cod = CInt(chiave.Split("_")(1))
                        If chiave.Split("_").Length > 5 Then
                            objParametriAgenda.Campo_Cod = CInt(chiave.Split("_")(5))
                        End If
                        objParametriAgenda.Appezza = CInt(chiave.Split("_")(2))
                    End If
                    If reset Then
                        objParametriAgenda.Id_Imp = 0
                    Else
                        objParametriAgenda.Id_Imp = CInt(chiave.Split("_")(3))
                    End If
                    objParametriAgenda.Impianti = New List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)
                    objParametriAgenda.Particelle = New List(Of Particella)
                    objParametriAgenda.GruppoOperazioneColturale = ""
                    objParametriAgenda.Fabbricato = 0
                    objParametriAgenda.Raggruppamento_Cod = 0
                    objParametriAgenda.Cod_Contatto = ""
                    objParametriAgenda.Mac_Cod = 0
                Case 6
                    objParametriAgenda.Piva = chiave.Split("_")(0)
                    objParametriAgenda.Sa_Cod = CInt(chiave.Split("_")(1))
                    objParametriAgenda.Appezza = 0
                    objParametriAgenda.Id_Imp = 0
                    objParametriAgenda.Fabbricato = 0
                    objParametriAgenda.Raggruppamento_Cod = 0
                    objParametriAgenda.Impianti = New List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)
                    objParametriAgenda.Particelle = New List(Of Particella)
                    objParametriAgenda.GruppoOperazioneColturale = ""
                    objParametriAgenda.Cod_Contatto = ""
                    objParametriAgenda.Mac_Cod = 0

                    Dim p As New Particella
                    p.Provincia = chiave.Split("_")(2)
                    p.Comune = chiave.Split("_")(3)
                    p.Sezione = chiave.Split("_")(4)
                    p.Foglio = chiave.Split("_")(5)
                    p.Numero = chiave.Split("_")(6)
                    p.Subalterno = chiave.Split("_")(7)
                    p.Part_Cod = chiave.Split("_")(8)
                    objParametriAgenda.Particelle.Add(p)

                Case 7
                    If Not reset Then
                        objParametriAgenda.Piva = chiave.Split("_")(0)
                        objParametriAgenda.Sa_Cod = CInt(chiave.Split("_")(1))
                    End If
                    If reset Then
                        objParametriAgenda.Fabbricato = 0
                    Else
                        Dim impianti = New List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)
                        impianti.Add(New AgronicaCoreModello.ParametriAgenda_Temp.Impianto With {
                            .ID_Reg = CInt(chiave.Split("_")(2))
                        })
                        objParametriAgenda.Impianti = impianti
                        objParametriAgenda.Cod_Progetto = CInt(chiave.Split("_")(2))
                    End If
                    objParametriAgenda.Fabbricato = 0
                    objParametriAgenda.Raggruppamento_Cod = 0
                    'objParametriAgenda.Impianti = New List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)
                    objParametriAgenda.Particelle = New List(Of Particella)
                    objParametriAgenda.GruppoOperazioneColturale = ""
                    objParametriAgenda.Cod_Contatto = ""
                    objParametriAgenda.Mac_Cod = 0
                Case 8
                    If Not reset Then
                        objParametriAgenda.Piva = chiave.Split("_")(0)
                        objParametriAgenda.Sa_Cod = CInt(chiave.Split("_")(1))
                        objParametriAgenda.Fabbricato = CInt(chiave.Split("_")(2))
                    End If
                    If reset Then
                        objParametriAgenda.Raggruppamento_Cod = 0
                    Else
                        objParametriAgenda.Raggruppamento_Cod = CInt(chiave.Split("_")(3))
                    End If
                    objParametriAgenda.Impianti = New List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)
                    objParametriAgenda.Particelle = New List(Of Particella)
                    objParametriAgenda.GruppoOperazioneColturale = ""
                    objParametriAgenda.Cod_Contatto = ""
                    objParametriAgenda.Mac_Cod = 0
            End Select

            r.RispostaOK = True
            r.RispostaStringa = AgronicaCoreDataProvider.Utility.convertOBJtoString(objParametriAgenda, False)

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function GetFiltrino() As String

        Dim Origine As String
        Dim Destinazione As String

        'azzero il filtro scelto x le operazioni multi-aziendali
        HttpContext.Current.Session("VariabiliFiltro") = Nothing

        'Costruisco il link
        Origine = Stringa_Codifica(
                        "../MenuAnagrafica/Menubs_anagrafica.aspx",
                        AgroKey_EncoderDecoder)

        Destinazione = Stringa_Codifica(
                        "../MenuAnagrafica/Menubs_anagrafica.aspx",
                        AgroKey_EncoderDecoder)


        Dim TargetUrl = "../Filtrino/FiltrinoImprese.aspx" &
                    "?o=" & Origine &
                    "&d=" & Destinazione

        Return TargetUrl
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function EditCatasto(ByVal chiave As String) As String
        Dim objParametriAgenda = New ParametriAgenda
        objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
        objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Anagrafica_Menu
        objParametriAgenda.Piva = chiave.Split("_")(0)
        objParametriAgenda.Sa_Cod = chiave.Split("_")(1)

        Dim p As New Particella

        p.Provincia = chiave.Split("_")(2)
        p.Comune = chiave.Split("_")(3)
        p.Sezione = chiave.Split("_")(4)
        p.Foglio = chiave.Split("_")(5)
        p.Numero = chiave.Split("_")(6)
        p.Subalterno = chiave.Split("_")(7)
        p.Part_Cod = chiave.Split("_")(8)

        objParametriAgenda.Particelle = New List(Of Particella)
        objParametriAgenda.Particelle.Add(p)

        objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica

        Return "ok"
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiWarningParticella(ByVal chiave As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

        ' sessione scaduta
        If objParametri_Server Is Nothing Then
            r.Sessione = False
            r.RispostaOK = vbFalse
            Return r
        End If

        Try

            Dim piva As String = chiave.Split("_")(0)
            Dim sa_cod As Integer = CInt(chiave.Split("_")(1))
            Dim prov As String = chiave.Split("_")(2)
            Dim com As String = chiave.Split("_")(3)
            Dim sezione As String = chiave.Split("_")(4)
            Dim foglio As Integer = CInt(chiave.Split("_")(5))
            Dim numero As Integer = CInt(chiave.Split("_")(6))
            Dim subalterno As String = chiave.Split("_")(7)

            Dim objParticellaR As New AgronicaCoreAnagrafeBIZ.Particella_R

            If Not objParticellaR.CheckParticellaxModificaCancellazione(True, prov, com, sezione, foglio, numero, subalterno, objParametri_Server) Then
                r.Errore = "<b>Cancellazione NON consentita !!!</b><br><br>La particella risulta associata a SQNPI, GIS o Analisi.<br>Rimuovere l'associazione prima di procedere con la cancellazione."
                r.RispostaOK = False
            Else
                Dim messaggio As New StringBuilder
                Dim listaWarning = objParticellaR.LeggiParticellaxWarning(piva, sa_cod, prov, com, sezione, foglio, numero, subalterno, objParametri_Server)
                If listaWarning.Count > 0 Then
                    messaggio.Append("La particella che si vuole cancellare risulta collegata ai seguenti elementi:<br><ul>")
                    For Each warning In listaWarning
                        Dim tokens = warning.Split("|")
                        Dim descrizione = tokens(0)
                        If tokens.Length > 1 Then
                            descrizione &= " (" & tokens(1) & ")"
                        End If
                        messaggio.Append("<li>" & descrizione & "</li>")
                    Next
                    messaggio.Append("</ul>")
                End If
                r.RispostaStringa = messaggio.ToString
                r.RispostaOK = True
            End If

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function InfoCatasto(ByVal chiave As String) As String
        Dim objParametriAgenda = New ParametriAgenda
        objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
        objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Anagrafica_Menu
        objParametriAgenda.Piva = chiave.Split("_")(0)
        objParametriAgenda.Sa_Cod = chiave.Split("_")(1)

        Dim p As New Particella

        p.Provincia = chiave.Split("_")(2)
        p.Comune = chiave.Split("_")(3)
        p.Sezione = chiave.Split("_")(4)
        p.Foglio = chiave.Split("_")(5)
        p.Numero = chiave.Split("_")(6)
        p.Subalterno = chiave.Split("_")(7)
        p.Part_Cod = chiave.Split("_")(8)

        objParametriAgenda.Particelle = New List(Of Particella)
        objParametriAgenda.Particelle.Add(p)

        objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Lettura

        Return "ok"
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function EditImpresa(ByVal chiave As String) As String
        Dim objParametriAgenda = New ParametriAgenda
        objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
        objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Anagrafica_Menu
        objParametriAgenda.Piva = chiave.Split("_")(0)
        objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica

        Return "ok"
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function EditRaggruppamentoStalla(ByVal chiave As String) As String
        Dim objParametriAgenda = New ParametriAgenda
        objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
        objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Anagrafica_Menu
        objParametriAgenda.Piva = chiave.Split("_")(0)
        objParametriAgenda.Sa_Cod = chiave.Split("_")(1)
        objParametriAgenda.Fabbricato = chiave.Split("_")(2)
        objParametriAgenda.Raggruppamento_Cod = chiave.Split("_")(3)
        objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica

        Return "ok"
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function consistenzeZoo(ByVal chiave As String) As String
        Dim objParametriAgenda = New ParametriAgenda
        objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
        objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Anagrafica_Menu
        objParametriAgenda.Piva = chiave.Split("_")(0)
        objParametriAgenda.Sa_Cod = chiave.Split("_")(1)
        objParametriAgenda.Fabbricato = chiave.Split("_")(2)
        objParametriAgenda.Raggruppamento_Cod = 0
        objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function movimentazioneZoo(ByVal chiave As String) As String
        Dim objParametriAgenda = New ParametriAgenda
        objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
        objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Anagrafica_Menu
        objParametriAgenda.Piva = chiave.Split("_")(0)
        objParametriAgenda.Sa_Cod = chiave.Split("_")(1)
        objParametriAgenda.Fabbricato = chiave.Split("_")(2)
        objParametriAgenda.Raggruppamento_Cod = 0
        objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
        objParametriAgenda.TipoOperazioneAgenda = enum_TipoOperazioneDB.Scrittura
        objParametriAgenda.Lav_Cod = 3001
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function InfoImpresa(ByVal chiave As String) As String
        Dim objParametriAgenda = New ParametriAgenda
        objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
        objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Anagrafica_Menu
        objParametriAgenda.Piva = chiave.Split("_")(0)
        objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Lettura

        Return "ok"
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function EditMacchina(ByVal chiave As String) As String
        Dim objParametriAgenda = New ParametriAgenda
        objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
        objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Anagrafica_Menu
        objParametriAgenda.Piva = chiave.Split("_")(0)
        objParametriAgenda.Sa_Cod = chiave.Split("_")(1)
        objParametriAgenda.Mac_Cod = chiave.Split("_")(2)
        objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica

        Return "ok"
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function InfoMacchina(ByVal chiave As String) As String
        Dim objParametriAgenda = New ParametriAgenda
        objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
        objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Anagrafica_Menu
        objParametriAgenda.Piva = chiave.Split("_")(0)
        objParametriAgenda.Sa_Cod = chiave.Split("_")(1)
        objParametriAgenda.Mac_Cod = chiave.Split("_")(2)
        objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Lettura

        Return "ok"
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function EditCentro(ByVal chiave As String) As String
        Dim objParametriAgenda = New ParametriAgenda
        objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
        objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Anagrafica_Menu
        objParametriAgenda.Piva = chiave.Split("_")(0)
        objParametriAgenda.Sa_Cod = chiave.Split("_")(1)
        objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica

        Return "ok"
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function InfoCentro(ByVal chiave As String) As String
        Dim objParametriAgenda = New ParametriAgenda
        objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
        objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Anagrafica_Menu
        objParametriAgenda.Piva = chiave.Split("_")(0)
        objParametriAgenda.Sa_Cod = chiave.Split("_")(1)
        objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Lettura
        Return "ok"
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function EditAppezzamento(ByVal chiave As String) As String
        Dim objParametriAgenda = New ParametriAgenda
        objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
        objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Anagrafica_Menu
        objParametriAgenda.Piva = chiave.Split("_")(0)
        objParametriAgenda.Sa_Cod = chiave.Split("_")(1)
        objParametriAgenda.Appezza = chiave.Split("_")(2)
        objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica
        Return "ok"
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function InfoAppezzamento(ByVal chiave As String) As String
        Dim objParametriAgenda = New ParametriAgenda
        objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
        objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Anagrafica_Menu
        objParametriAgenda.Piva = chiave.Split("_")(0)
        objParametriAgenda.Sa_Cod = chiave.Split("_")(1)
        objParametriAgenda.Appezza = chiave.Split("_")(2)
        objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Lettura
        Return "ok"
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function EditFabbricato(ByVal chiave As String) As String
        Dim objParametriAgenda = New ParametriAgenda
        objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
        objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Anagrafica_Menu
        objParametriAgenda.Piva = chiave.Split("_")(0)
        objParametriAgenda.Sa_Cod = chiave.Split("_")(1)
        objParametriAgenda.Fabbricato = chiave.Split("_")(2)
        objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica

        Return "ok"
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function InfoFabbricato(ByVal chiave As String) As String
        Dim objParametriAgenda = New ParametriAgenda
        objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
        objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Anagrafica_Menu
        objParametriAgenda.Piva = chiave.Split("_")(0)
        objParametriAgenda.Sa_Cod = chiave.Split("_")(1)
        objParametriAgenda.Fabbricato = chiave.Split("_")(2)
        objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Lettura

        Return "ok"
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function InfoRaggruppamentoStalla(ByVal chiave As String) As String
        Dim objParametriAgenda = New ParametriAgenda
        objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
        objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Anagrafica_Menu
        objParametriAgenda.Piva = chiave.Split("_")(0)
        objParametriAgenda.Sa_Cod = chiave.Split("_")(1)
        objParametriAgenda.Fabbricato = chiave.Split("_")(2)
        objParametriAgenda.Raggruppamento_Cod = chiave.Split("_")(3)
        objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Lettura

        Return "ok"
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function EditImpianto(ByVal chiave As String) As String
        Dim objParametriAgenda = New ParametriAgenda
        objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
        objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Anagrafica_Menu
        objParametriAgenda.Piva = chiave.Split("_")(0)
        objParametriAgenda.Sa_Cod = chiave.Split("_")(1)
        objParametriAgenda.Appezza = chiave.Split("_")(2)
        objParametriAgenda.Id_Imp = chiave.Split("_")(3)
        objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica

        Return "ok"
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function InfoImpianto(ByVal chiave As String) As String
        Dim objParametriAgenda = New ParametriAgenda
        objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
        objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Anagrafica_Menu
        objParametriAgenda.Piva = chiave.Split("_")(0)
        objParametriAgenda.Sa_Cod = chiave.Split("_")(1)
        objParametriAgenda.Appezza = chiave.Split("_")(2)
        objParametriAgenda.Id_Imp = chiave.Split("_")(3)
        objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Lettura

        Return "ok"
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function OperazioniImpianto(ByVal chiave As String) As String
        Dim objParametriAgenda = New ParametriAgenda
        objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
        objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Anagrafica_Menu
        objParametriAgenda.Piva = chiave.Split("_")(0)
        objParametriAgenda.Sa_Cod = chiave.Split("_")(1)
        objParametriAgenda.Appezza = chiave.Split("_")(2)
        objParametriAgenda.Id_Imp = chiave.Split("_")(3)
        Dim impianti = New List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)
        Dim impianto As New AgronicaCoreModello.ParametriAgenda_Temp.Impianto
        impianto.Piva = objParametriAgenda.Piva
        impianto.Sa_Cod = objParametriAgenda.Sa_Cod
        impianto.Appezza = objParametriAgenda.Appezza
        impianto.ID_Reg = objParametriAgenda.Id_Imp
        impianti.Add(impianto)

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objReg_Impianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dim veg_cod As Integer = objReg_Impianti.VegCod_from_PivaSaCodAppezzaIdimp(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, objParametriAgenda.Appezza, objParametriAgenda.Id_Imp, "", "", objParametri_Server)
        Dim id_cod As Integer = objReg_Impianti.Leggi_DestinazioneUso_Impianto(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, objParametriAgenda.Appezza, objParametriAgenda.Id_Imp, "", "", objParametri_Server)
        If veg_cod <> 0 Then
            objParametriAgenda.Veg_Cod = CStr(veg_cod)
        Else
            objParametriAgenda.Veg_Cod = CStr(veg_cod) & "/" & CStr(id_cod)
        End If
        objParametriAgenda.Impianti = impianti

        objParametriAgenda.salva()

        Dim objParametriAgenda_2010 = New ParametriAgenda_2010

        Dim PaginaLink = "../Menu/MenuBS_Agenda_Nuovo.aspx?OperazioniImpianto=1"
        PaginaLink &= "&PaginaOrigine=" & CStr(enum_PagineGiasOnline.MenuAnagrafica)

        Return PaginaLink
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function EditContatto(ByVal chiave As String) As String

        Dim objParametriAgenda = New ParametriAgenda
        objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
        objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Anagrafica_Menu
        'Salvo la PIVA del centro aziendale per poter risettare corretamente il filtro delle tabelle al torna indietro
        objParametriAgenda.Piva_Origine = ""
        objParametriAgenda.Piva_Origine = objParametriAgenda.Piva
        objParametriAgenda.Sa_Cod = chiave.Split("_")(1)

        Dim url As String = "../Anagrafica/New_Contatto_Edit.aspx"
        Dim operazione As String = "?o=" & Stringa_Codifica(enum_TipoOperazioneDB.Modifica, AgroKey_EncoderDecoder, Nothing)
        Dim cod_contatto As String = "&codcont=" & Stringa_Codifica(chiave.Split("_")(2), AgroKey_EncoderDecoder, Nothing)
        Dim piva As String = "&piva=" & Stringa_Codifica(chiave.Split("_")(0), AgroKey_EncoderDecoder, Nothing)

        Dim queryString As String = operazione & cod_contatto & piva
        Return url & queryString

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function InfoContatto(ByVal chiave As String) As String
        Dim objParametriAgenda = New ParametriAgenda
        objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
        objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Anagrafica_Menu

        'Salvo la PIVA del centro aziendale per poter risettare corretamente il filtro delle tabelle al torna indietro
        objParametriAgenda.Piva_Origine = ""
        objParametriAgenda.Piva_Origine = objParametriAgenda.Piva
        objParametriAgenda.Sa_Cod = chiave.Split("_")(1)

        Dim url As String = "../Anagrafica/New_Contatto_Edit.aspx"
        Dim operazione As String = "?o=" & Stringa_Codifica(enum_TipoOperazioneDB.Lettura, AgroKey_EncoderDecoder, Nothing)
        Dim cod_contatto As String = "&codcont=" & Stringa_Codifica(chiave.Split("_")(2), AgroKey_EncoderDecoder, Nothing)
        Dim piva As String = "&piva=" & Stringa_Codifica(chiave.Split("_")(0), AgroKey_EncoderDecoder, Nothing)

        Dim queryString As String = operazione & cod_contatto & piva
        Return url & queryString


    End Function




    <WebMethod(EnableSession:=True)>
    Public Shared Function EditCampo(ByVal chiave As String) As String
        Dim objParametriAgenda = New ParametriAgenda
        objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
        objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Anagrafica_Menu
        objParametriAgenda.Piva = chiave.Split("_")(0)
        objParametriAgenda.Sa_Cod = chiave.Split("_")(1)
        objParametriAgenda.Campo_Cod = chiave.Split("_")(2)
        objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica
        Return "ok"
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function InfoCampo(ByVal chiave As String) As String
        Dim objParametriAgenda = New ParametriAgenda
        objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
        objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Anagrafica_Menu
        objParametriAgenda.Piva = chiave.Split("_")(0)
        objParametriAgenda.Sa_Cod = chiave.Split("_")(1)
        objParametriAgenda.Campo_Cod = chiave.Split("_")(2)
        objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Lettura
        Return "ok"
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function getURLSincronizzaBDN(ByVal chiave As String) As RispostaStandard
        Dim r As New RispostaStandard

        Try
            Dim targetUrl As Object = CreaParametriSincroZoo(chiave, enum_PagineAgronicaSincro.SincronizzatoreBDN, "&tipoSincro=1")
            r.RispostaOK = True
            r.RispostaStringa = targetUrl

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function getURLImportazioneModello4(ByVal chiave As String) As RispostaStandard
        Dim r As New RispostaStandard

        Try
            Dim targetUrl As Object = CreaParametriSincroZoo(chiave, enum_PagineAgronicaSincro.SincronizzazioneStalleBDN, "&tipoSincro=1")
            r.RispostaOK = True
            r.RispostaStringa = targetUrl
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function

    Private Shared Function CreaParametriSincroZoo(chiave As String, paginaRichiesta As enum_PagineAgronicaSincro, paramAggiuntiQueryString As String) As Object
        Dim objParametriAgenda = New ParametriAgenda
        objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
        objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Anagrafica_Menu

        'Salvo la PIVA del centro aziendale per poter risettare corretamente il filtro delle tabelle al torna indietro
        objParametriAgenda.Piva_Origine = ""
        objParametriAgenda.Piva_Origine = chiave.Split("_")(0)
        objParametriAgenda.Sa_Cod = chiave.Split("_")(1)

        Dim objParametriSincro As New ParametriSincronizzatore_2010()

        objParametriSincro.Piva = objParametriAgenda.Piva_Origine
        objParametriSincro.Pagina_Richiesta = paginaRichiesta

        objParametriSincro.ParametriQueryString = "chiave=" & Stringa_Codifica(chiave, AgroKey_EncoderDecoder)

        If Not String.IsNullOrWhiteSpace(paramAggiuntiQueryString) Then
            objParametriSincro.ParametriQueryString &= paramAggiuntiQueryString
        End If
        Dim targetUrl = RedirectGestione.IndirizzoCompleto_SitoSincronizzatore_PassandoDirettamente_ParametriSincro_2010(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objParametriSincro)
        Return targetUrl
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function getURLSincronizzaVetInfo(ByVal chiave As String) As RispostaStandard
        Dim r As New RispostaStandard

        Try
            Dim objParametriAgenda = New ParametriAgenda
            objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
            objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Anagrafica_Menu

            'Salvo la PIVA del centro aziendale per poter risettare corretamente il filtro delle tabelle al torna indietro
            objParametriAgenda.Piva_Origine = ""
            objParametriAgenda.Piva_Origine = chiave.Split("_")(0)
            objParametriAgenda.Sa_Cod = chiave.Split("_")(1)

            Dim objParametriSincro As New ParametriSincronizzatore_2010()

            objParametriSincro.Piva = objParametriAgenda.Piva_Origine
            objParametriSincro.Pagina_Richiesta = enum_PagineAgronicaSincro.SincronizzatoreBDN

            objParametriSincro.ParametriQueryString = "tipoSincro=9" & "&chiave=" & Stringa_Codifica(chiave, AgroKey_EncoderDecoder)

            Dim targetUrl = RedirectGestione.IndirizzoCompleto_SitoSincronizzatore_PassandoDirettamente_ParametriSincro_2010(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objParametriSincro)
            r.RispostaOK = True
            r.RispostaStringa = targetUrl

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function getURLInviaTrattamentiVetInfo(ByVal chiave As String) As RispostaStandard
        Dim r As New RispostaStandard

        Try
            Dim objParametriAgenda = New ParametriAgenda
            objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
            objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Anagrafica_Menu

            'Salvo la PIVA del centro aziendale per poter risettare corretamente il filtro delle tabelle al torna indietro
            objParametriAgenda.Piva_Origine = ""
            objParametriAgenda.Piva_Origine = chiave.Split("_")(0)
            objParametriAgenda.Sa_Cod = chiave.Split("_")(1)

            Dim objParametriSincro As New ParametriSincronizzatore_2010()

            objParametriSincro.Piva = objParametriAgenda.Piva_Origine
            objParametriSincro.Pagina_Richiesta = enum_PagineAgronicaSincro.InvioTrattamentiZooVetInfo

            objParametriSincro.ParametriQueryString = "tipoSincro=1&chiave=" & Stringa_Codifica(chiave, AgroKey_EncoderDecoder)

            Dim targetUrl = RedirectGestione.IndirizzoCompleto_SitoSincronizzatore_PassandoDirettamente_ParametriSincro_2010(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objParametriSincro)
            r.RispostaOK = True
            r.RispostaStringa = targetUrl

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function getURLGiacenzeGiasVetInfo(ByVal chiave As String) As RispostaStandard
        Dim r As New RispostaStandard

        Try
            Dim objParametriAgenda = New ParametriAgenda
            objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
            objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Anagrafica_Menu

            ' Salva la PIVA del centro aziendale per poter risettare corretamente il filtro delle tabelle al torna indietro
            objParametriAgenda.Piva_Origine = chiave.Split("_")(0)
            objParametriAgenda.Sa_Cod = chiave.Split("_")(1)

            Dim objParametriSincro As New ParametriSincronizzatore_2010()

            objParametriSincro.Piva = objParametriAgenda.Piva_Origine
            objParametriSincro.Pagina_Richiesta = enum_PagineAgronicaSincro.InvioTrattamentiZooVetInfo

            objParametriSincro.ParametriQueryString = "tipoSincro=4&chiave=" & Stringa_Codifica(chiave, AgroKey_EncoderDecoder)

            Dim targetUrl = RedirectGestione.IndirizzoCompleto_SitoSincronizzatore_PassandoDirettamente_ParametriSincro_2010(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objParametriSincro)
            r.RispostaOK = True
            r.RispostaStringa = targetUrl

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function getURLGestioneCapiBDN(ByVal chiave As String) As RispostaStandard
        Dim r As New RispostaStandard

        Try
            Dim objParametriAgenda = New ParametriAgenda
            objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
            objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Anagrafica_Menu

            Dim objParametriSincro As New ParametriSincronizzatore_2010()

            objParametriSincro.Piva = objParametriAgenda.Piva
            objParametriSincro.Pagina_Richiesta = enum_PagineAgronicaSincro.SincronizzatoreBDN
            Dim objOggetto As New JObject
            Dim objArray = JArray.Parse(chiave)
            objOggetto("CapiAnimali") = objArray
            objParametriSincro.Xml_Generico = chiave

            objParametriSincro.ParametriQueryString = "tipoSincro=2" '& "&chiave_arr=" & Stringa_Codifica(chiave, AgroKey_EncoderDecoder)

            Dim targetUrl = RedirectGestione.IndirizzoCompleto_SitoSincronizzatore_PassandoDirettamente_ParametriSincro_2010(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objParametriSincro)
            r.RispostaOK = True
            r.RispostaStringa = targetUrl

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r


    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function getURLGestioneCarichiBDN(ByVal chiave As String, ByVal del As Boolean) As RispostaStandard
        Dim r As New RispostaStandard

        Try
            Dim objParametriAgenda = New ParametriAgenda
            objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
            objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Anagrafica_Menu

            'Salvo la PIVA del centro aziendale per poter risettare corretamente il filtro delle tabelle al torna indietro
            objParametriAgenda.Piva_Origine = ""
            objParametriAgenda.Piva_Origine = chiave.Split("_")(0)
            objParametriAgenda.Sa_Cod = chiave.Split("_")(1)

            Dim objParametriSincro As New ParametriSincronizzatore_2010()

            objParametriSincro.Piva = objParametriAgenda.Piva_Origine
            objParametriSincro.Pagina_Richiesta = enum_PagineAgronicaSincro.SincronizzatoreBDN

            'gestione creazione/cancellazione ingressi
            If del Then
                objParametriSincro.ParametriQueryString = "tipoSincro=7"
            Else
                objParametriSincro.ParametriQueryString = "tipoSincro=4"
            End If
            objParametriSincro.ParametriQueryString &= "&chiave=" & Stringa_Codifica(chiave, AgroKey_EncoderDecoder)

            Dim targetUrl = RedirectGestione.IndirizzoCompleto_SitoSincronizzatore_PassandoDirettamente_ParametriSincro_2010(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objParametriSincro)
            r.RispostaOK = True
            r.RispostaStringa = targetUrl

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r


    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function getURLGestioneScarichiBDN(ByVal chiave As String, ByVal del As Boolean) As RispostaStandard
        Dim r As New RispostaStandard

        Try
            Dim objParametriAgenda = New ParametriAgenda
            objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
            objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Anagrafica_Menu

            'Salvo la PIVA del centro aziendale per poter risettare corretamente il filtro delle tabelle al torna indietro
            objParametriAgenda.Piva_Origine = ""
            objParametriAgenda.Piva_Origine = chiave.Split("_")(0)
            objParametriAgenda.Sa_Cod = chiave.Split("_")(1)

            Dim objParametriSincro As New ParametriSincronizzatore_2010()

            objParametriSincro.Piva = objParametriAgenda.Piva_Origine
            objParametriSincro.Pagina_Richiesta = enum_PagineAgronicaSincro.SincronizzatoreBDN

            'gestione creazione/cancellazione uscite
            If del Then
                objParametriSincro.ParametriQueryString = "tipoSincro=8"
            Else
                objParametriSincro.ParametriQueryString = "tipoSincro=5"
            End If
            objParametriSincro.ParametriQueryString &= "&chiave=" & Stringa_Codifica(chiave, AgroKey_EncoderDecoder)

            Dim targetUrl = RedirectGestione.IndirizzoCompleto_SitoSincronizzatore_PassandoDirettamente_ParametriSincro_2010(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objParametriSincro)
            r.RispostaOK = True
            r.RispostaStringa = targetUrl

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r


    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function EditZoo(ByVal chiave As String) As String
        Dim objParametriAgenda = New ParametriAgenda
        objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
        objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Anagrafica_Menu
        If (IsNumeric(chiave.Split("_")(2))) Then
            objParametriAgenda.Cod_Progetto = CInt(chiave.Split("_")(2))
        End If
        objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica

        Return "ok"
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function InfoZoo(ByVal chiave As String) As String
        Dim objParametriAgenda = New ParametriAgenda
        objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
        objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Anagrafica_Menu
        If (IsNumeric(chiave.Split("_")(2))) Then
            objParametriAgenda.Cod_Progetto = CInt(chiave.Split("_")(2))
        End If
        objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Lettura
        Return "ok"
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function gotoNewElement(ByVal tipo As Integer, ByVal chiave As String) As RispostaStandard
        Dim objParametriAgenda = New ParametriAgenda
        objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
        objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Anagrafica_Menu
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim risp = New RispostaStandard


        'TODO controllo se sessione scaduta e 
        If objParametri_Server Is Nothing Then
            risp.Sessione = False
            risp.RispostaOK = vbFalse
            Return risp
        End If

        Dim permessi = New PermessiUtente()

        Try
            risp.RispostaOK = True
            Select Case tipo
                Case 1
                    If permessi.getPermesso(enum_Security_Attivita.Anagrafica_Impresa).Scrittura = True Then


                        risp.RispostaStringa = "../Anagrafica/Impresa_Edit.aspx"
                    Else
                        Throw New Exception(AgronicaAgenda_2010.MancanzaPermessiCreazioneImpresa)
                    End If
                Case 2
                    If permessi.getPermesso(enum_Security_Attivita.Anagrafica_CentroAziendale).Scrittura = True Then

                        objParametriAgenda.Sa_Cod = ""
                        risp.RispostaStringa = "../Anagrafica/Centro_Edit.aspx"

                    Else
                        Throw New Exception(AgronicaAgenda_2010.MancanzaPermessiCreazioneCentroAziendale)
                    End If
                Case 3
                    If permessi.getPermesso(enum_Security_Attivita.Anagrafica_Campo).Scrittura = True Then

                        Dim parts As String() = chiave.Split(New Char() {"-"c})

                        objParametriAgenda.Sa_Cod = parts(0)
                        HttpContext.Current.Session("Serra") = parts(1)
                        objParametriAgenda.Campo_Cod = "0"
                        risp.RispostaStringa = "../Anagrafica/Campo_Edit.aspx"

                    Else
                        Throw New Exception(AgronicaAgenda_2010.MancanzaPermessiCreazioneCampo)
                    End If
                Case 4
                    If permessi.getPermesso(enum_Security_Attivita.Anagrafica_Appezzamento).Scrittura = True Then

                        ' appezzamento
                        Dim parts As String() = chiave.Split(New Char() {"-"c})

                        objParametriAgenda.Piva = parts(0)
                        objParametriAgenda.Sa_Cod = parts(1)
                        objParametriAgenda.Campo_Cod = parts(2)

                        risp.RispostaStringa = "../Anagrafica/Appezzamento_Nuovo.aspx"
                    Else
                        Throw New Exception(AgronicaAgenda_2010.MancanzaPermessiCreazioneAppezzamento)
                    End If


                Case 5
                    If permessi.getPermesso(enum_Security_Attivita.Anagrafica_Impianto).Scrittura = True Then

                        ' impianto
                        Dim parts As String() = chiave.Split(New Char() {"-"c})

                        objParametriAgenda.Piva = parts(0)
                        objParametriAgenda.Sa_Cod = parts(1)
                        objParametriAgenda.Campo_Cod = parts(2)
                        objParametriAgenda.Appezza = parts(3)
                        objParametriAgenda.Id_Imp = "0"
                        risp.RispostaStringa = "../Anagrafica/Impianto_Edit2.aspx"
                    Else
                        Throw New Exception(AgronicaAgenda_2010.MancanzaPermessiCreazioneImpianto)
                    End If

                Case 6
                    If permessi.getPermesso(enum_Security_Attivita.Anagrafica_ParticellaCatastale).Scrittura = True Then

                        ' particella
                        objParametriAgenda.Sa_Cod = chiave
                        risp.RispostaStringa = "../Anagrafica/Catasto_Edit.aspx"
                    Else
                        Throw New Exception(AgronicaAgenda_2010.MancanzaPermessiCreazioneParticella)
                    End If
                Case 7
                    If permessi.getPermesso(enum_Security_Attivita.Gest_Stalle).Scrittura = True Then
                        ' Raggruppamento Stalle Edit

                        Dim parts As String() = chiave.Split(New Char() {"-"c})

                        objParametriAgenda.Piva = parts(0)
                        objParametriAgenda.Sa_Cod = parts(1)
                        objParametriAgenda.Fabbricato = parts(2)
                        risp.RispostaStringa = "../Anagrafica/Stalla_Raggruppamenti_Edit.aspx"
                    Else
                        Throw New Exception(AgronicaAgenda_2010.MancanzaPermessiCreazioneRaggruppamentoStalla)
                    End If
                Case 9
                    If permessi.getPermesso(enum_Security_Attivita.Gest_Stalle).Scrittura = True Then
                        ' Raggruppamento Stalle Edit

                        Dim parts As String() = chiave.Split(New Char() {"-"c})

                        objParametriAgenda.Piva = parts(0)
                        objParametriAgenda.Sa_Cod = parts(1)
                        objParametriAgenda.Fabbricato = parts(2)
                        objParametriAgenda.TipoOperazioneAgenda = "1"
                        objParametriAgenda.Tipo_Operazione = "1"
                        objParametriAgenda.Lav_Cod = LAVCOD_INCREMENTO_CONSISTENZE_ZOO
                        objParametriAgenda.salva()
                        risp.RispostaStringa = "../Zoo/Zoo_Carico.aspx"
                    Else
                        Throw New Exception(AgronicaAgenda_2010.MancanzaPermessiCreazioneZoo)
                    End If


                Case 10
                    If permessi.getPermesso(enum_Security_Attivita.Anagrafica_Fabbricato).Scrittura = True Then

                        ' fabbricato
                        objParametriAgenda.Sa_Cod = chiave
                        risp.RispostaStringa = "../Anagrafica/Fabbricato_Edit.aspx"
                    Else
                        Throw New Exception(AgronicaAgenda_2010.MancanzaPermessiCreazioneFabbricato)
                    End If
                Case 11
                    If permessi.getPermesso(enum_Security_Attivita.Anagrafica_Contatto).Scrittura = True Then

                        ' contatto
                        objParametriAgenda.Sa_Cod = ""
                        objParametriAgenda.Cod_Contatto = ""
                        risp.RispostaStringa = "../Anagrafica/New_Contatto_Edit.aspx"


                    Else
                        Throw New Exception(AgronicaAgenda_2010.MancanzaPermessiCreazioneContatto)
                    End If
                Case 12
                    If permessi.getPermesso(enum_Security_Attivita.Anagrafica_ParcoMacchine).Scrittura = True Then

                        ' macchina
                        objParametriAgenda.Sa_Cod = 0
                        objParametriAgenda.Mac_Cod = 0
                        risp.RispostaStringa = "../Anagrafica/Macchina_Edit.aspx"

                    Else
                        Throw New Exception(AgronicaAgenda_2010.MancanzaPermessiCreazioneMacchina)
                    End If
                Case 15
                    If permessi.getPermesso(enum_Security_Attivita.Angrafica_Prodotti).Scrittura = True Then
                        risp.RispostaStringa = NuovoProdotto(chiave)
                    Else
                        Throw New Exception(AgronicaAgenda_2010.MancanzaPermessiCreazioneRaggruppamentoStalla)
                    End If
            End Select
            objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
            objParametriAgenda.Particelle = New List(Of Particella)
        Catch ex As Exception
            risp.RispostaOK = False
            risp.Errore = ex.Message
        End Try


        Return risp
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaCentro_ddl() As RispostaStandard
        Dim r As New RispostaStandard
        Try
            Dim objParametriAgenda = New ParametriAgenda
            Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
            objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
            Dim dt As DataTable
            Dim objCentro As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
            dt = objCentro.Leggi(objParametriAgenda.Piva, 0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

            ' Inserisco la scelta Centro
            Dim i As Integer
            Dim jObj As New JObject
            Dim jsonArray As New JArray
            For i = 0 To dt.Rows.Count - 1
                jObj = New JObject
                jObj.Add(New JProperty("des", dt.Rows(i).Item("Sa_Nome")))
                jObj.Add(New JProperty("val", dt.Rows(i).Item("Sa_Cod")))
                jsonArray.Add(jObj)
            Next

            r.RispostaOK = True
            r.RispostaStringa = jsonArray.ToString

        Catch ex As Exception
            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaCampo_ddl(ByVal sa_cod As Integer) As RispostaStandard
        Dim r As New RispostaStandard
        Try
            Dim objParametriAgenda = New ParametriAgenda
            Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
            objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

            Dim dt As DataTable
            Dim objCampi As New AgronicaCoreAnagrafeDAL.Campi_R
            dt = objCampi.Leggi(objParametriAgenda.Piva, sa_cod, 0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

            Dim jsonCampi As New JArray

            Dim i As Integer

            ' Inserisco la scelta Campo vuoto
            Dim jObj As New JObject
            'jObj.Add(New JProperty("des", "NESSUNO"))
            'jObj.Add(New JProperty("val", "0"))
            'jsonCampi.Add(jObj)
            For i = 0 To dt.Rows.Count - 1
                jObj = New JObject
                jObj.Add(New JProperty("des", dt.Rows(i).Item("Campo_des")))
                jObj.Add(New JProperty("val", dt.Rows(i).Item("campo_cod")))
                jsonCampi.Add(jObj)
            Next

            r.RispostaOK = True
            r.RispostaStringa = jsonCampi.ToString
        Catch ex As Exception
            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaAppezzamento_ddl(ByVal sa_cod As Integer, ByVal campo_cod As Integer) As RispostaStandard
        Dim r As New RispostaStandard
        Try

            Dim objParametriAgenda = New ParametriAgenda
            Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
            objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

            Dim dt As DataTable
            Dim objAppezzamento As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
            dt = objAppezzamento.Leggi(objParametriAgenda.Piva, sa_cod, 0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, " campo_cod =" & campo_cod, "", objParametri_Server)

            ' Inserisco la scelta Appezzamento
            Dim i As Integer
            Dim jObj As New JObject
            Dim jsonArray As New JArray
            For i = 0 To dt.Rows.Count - 1
                jObj = New JObject
                jObj.Add(New JProperty("des", dt.Rows(i).Item("app_nome")))
                jObj.Add(New JProperty("val", dt.Rows(i).Item("appezza")))
                jsonArray.Add(jObj)
            Next

            r.RispostaOK = True
            r.RispostaStringa = jsonArray.ToString

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function



    'Delete
    <WebMethod(EnableSession:=True)>
    Public Shared Function EliminaElemento(ByVal tipo As Integer, ByVal chiave As String) As String

    End Function

    <WebMethod(EnableSession:=True)>
    Private Function PreparaperCancellazione(ByVal xTipoNodo As Integer, ByVal xChiave As String)
        Dim Testo As String
        Dim UrlTarget As String

        Dim objParametriAgenda = New ParametriAgenda

        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))


        Dim r_msg As String = ""

        '----------------------------------------------------------------
        '------ MODIFICA x GIAS2GIAS (28/03/2008) -------
        '----------------------------------------------------------------
        'Verifico se l'Impresa è stata generata dal SuperUser corrente...
        'In caso contrario BLOCCO qualsiasi Operazione!!!


        ' Verifico se ESISTE la PivaSuperUser Origine Dato

        ' Se ESISTE
        '   Verifico se corrisponde al SuperUserCorrente 
        '       Se UGUALE proseguo
        '       Se DIVERSO ---> blocco l'operazione

        ' Se NON ESISTE
        '   Creo il record nella tabella Imprese_Codici
        Dim objCodici As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read

        Dim SuperUser_Corrente As Boolean
        Dim PivaSuperUser_Origine As String

        If objCodici.EsistePivaSuperUser_OrigineDato(objParametriAgenda.Piva, Session("ASG_SuperUser_CodFiscale"), PivaSuperUser_Origine, SuperUser_Corrente,
                                                        "", objParametri_Server) = True Then


            If SuperUser_Corrente = True Then
                'OK
            Else
                'blocco
                If SuperUser_Corrente = False Then
                    r_msg = String.Format(AgronicaAgenda_2010.MenuBS_Anagrafica_PreparaperCancellazione_ArchivioDiverso, vbCrLf)
                    Exit Function
                End If

            End If

        Else
            ' SE NON esiste lo creo
            ' Inserisci_CodiceImpresa2(xPiva, enum_CodiciAnagrafe.PivaSuperUser_Origine_Dato, CStr(Session("ASG_SuperUser_CodFiscale")), objParametri_Server)

            Dim objImpreseCodici As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Write

            'Inserisco
            Dim IntDummy As Integer = objImpreseCodici.Scrivi(objParametriAgenda.Piva,
                                               enum_CodiciAnagrafe.PivaSuperUser_Origine_Dato,
                                               CStr(Session("ASG_SuperUser_CodFiscale")),
                                               AGRODATAINIZIO,
                                               AGRODATAFINE,
                                               objParametri_Server)

            objImpreseCodici = Nothing

        End If

        '----------------------------------------------------------------
        '-------- Fine Modifica -----------------------------------------
        '----------------------------------------------------------------

        'Azzero la variabile di appoggio
        Testo = ""

        'Verifico il tipo del nodo selezionato
        Select Case xTipoNodo

            Case enum_TipoNodo.Utente

                '------------------------------------------------
            Case enum_TipoNodo.Impresa

                If objParametriAgenda.Piva <> Session("ASG_SuperUser_CodFiscale") Then

                    'Carico la pagina di cancellazione
                    UrlTarget = "Cancella_Elemento.aspx" &
                                    "?k=" & Stringa_Codifica(xChiave, AgroKey_EncoderDecoder, Server) &
                                    "&o=" & Stringa_Codifica(enum_TipoOperazioneDB.Cancellazione, AgroKey_EncoderDecoder, Server) &
                                    "&p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder, Server) &
                                    "&r=" & Stringa_Codifica("../AlberoImprese/AlberoImprese.aspx", AgroKey_EncoderDecoder, Server)

                    'Carico la pagina
                    Response.Redirect(UrlTarget)

                Else
                    r_msg = AgronicaAgenda_2010.ImpossibileEliminareImpresaSelezionata
                    Exit Function

                End If

                '------------------------------------------------
            Case enum_TipoNodo.Centro

                'Carico la pagina di cancellazione
                UrlTarget = "Cancella_Elemento.aspx" &
                                "?k=" & Stringa_Codifica(xChiave, AgroKey_EncoderDecoder, Server) &
                                "&o=" & Stringa_Codifica(enum_TipoOperazioneDB.Cancellazione, AgroKey_EncoderDecoder, Server) &
                                "&p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder, Server) &
                                "&r=" & Stringa_Codifica("AlberoImprese.aspx", AgroKey_EncoderDecoder, Server)

                'Carico la pagina
                Response.Redirect(UrlTarget)

                '------------------------------------------------
            Case enum_TipoNodo.Campo,
                 enum_TipoNodo.Serra

                'Carico la pagina di cancellazione
                UrlTarget = "Cancella_Elemento.aspx" &
                                "?k=" & Stringa_Codifica(xChiave, AgroKey_EncoderDecoder, Server) &
                                "&o=" & Stringa_Codifica(enum_TipoOperazioneDB.Cancellazione, AgroKey_EncoderDecoder, Server) &
                                "&p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder, Server) &
                                "&r=" & Stringa_Codifica("AlberoImprese.aspx", AgroKey_EncoderDecoder, Server)

                'Carico la pagina
                Response.Redirect(UrlTarget)

                '------------------------------------------------
            Case enum_TipoNodo.Appezzamento

                'Carico la pagina di cancellazione
                UrlTarget = "Cancella_Elemento.aspx" &
                                "?k=" & Stringa_Codifica(xChiave, AgroKey_EncoderDecoder, Server) &
                                "&o=" & Stringa_Codifica(enum_TipoOperazioneDB.Cancellazione, AgroKey_EncoderDecoder, Server) &
                                "&p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder, Server) &
                                "&r=" & Stringa_Codifica("AlberoImprese.aspx", AgroKey_EncoderDecoder, Server)

                'Carico la pagina
                Response.Redirect(UrlTarget)

                '------------------------------------------------
            Case enum_TipoNodo.ImpiantoNudo,
                 enum_TipoNodo.ImpiantoArborea,
                 enum_TipoNodo.ImpiantoErbacea,
                 enum_TipoNodo.ImpiantoOrticola

                'Carico la pagina di cancellazione
                UrlTarget = "Cancella_Elemento.aspx" &
                                "?k=" & Stringa_Codifica(xChiave, AgroKey_EncoderDecoder, Server) &
                                "&o=" & Stringa_Codifica(enum_TipoOperazioneDB.Cancellazione, AgroKey_EncoderDecoder, Server) &
                                "&p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder, Server) &
                                "&r=" & Stringa_Codifica("AlberoImprese.aspx", AgroKey_EncoderDecoder, Server)

                'Carico la pagina
                Response.Redirect(UrlTarget)

                '------------------------------------------------
            Case enum_TipoNodo.CatastoAziendale

                '------------------------------------------------
            Case enum_TipoNodo.Particella

                'Carico la pagina di cancellazione
                '''UrlTarget = "Cancella_Elemento.aspx" & _
                '''                "?k=" & Stringa_Codifica(xChiave, AgroKey_EncoderDecoder, Server) & _
                '''                "&o=" & Stringa_Codifica(enum_TipoOperazioneDB.Cancellazione, AgroKey_EncoderDecoder, Server) & _
                '''                "&p=" & Stringa_Codifica(objParametriAgenda.piva, AgroKey_EncoderDecoder, Server)

                UrlTarget = "../GestioneCatasto/GestioneCatasto_Particella_Delete.aspx" &
                                 "?k=" & Stringa_Codifica(xChiave, AgroKey_EncoderDecoder, Server) &
                                 "&o=" & Stringa_Codifica(enum_TipoOperazioneDB.Cancellazione, AgroKey_EncoderDecoder, Server) &
                                 "&p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder, Server) &
                                 "&r=" & Stringa_Codifica("../AlberoImprese/AlberoImprese.aspx", AgroKey_EncoderDecoder, Server)

                'Carico la pagina
                Response.Redirect(UrlTarget)

                '------------------------------------------------
            Case enum_TipoNodo.Fabbricato_Generico,
                 enum_TipoNodo.f_Abitazione,
                 enum_TipoNodo.f_CellaFrigorifera,
                 enum_TipoNodo.f_ImpiantoLavorazione,
                 enum_TipoNodo.f_Magazzino,
                 enum_TipoNodo.f_Silos,
                 enum_TipoNodo.f_Stalla,
                 enum_TipoNodo.f_Fienile,
                 enum_TipoNodo.f_Essiccatoio

                'Carico la pagina di cancellazione
                UrlTarget = "Cancella_Elemento.aspx" &
                                "?k=" & Stringa_Codifica(xChiave, AgroKey_EncoderDecoder, Server) &
                                "&o=" & Stringa_Codifica(enum_TipoOperazioneDB.Cancellazione, AgroKey_EncoderDecoder, Server) &
                                "&p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder, Server) &
                                "&r=" & Stringa_Codifica("AlberoImprese.aspx", AgroKey_EncoderDecoder, Server)

                'Carico la pagina
                Response.Redirect(UrlTarget)

                '------------------------------------------------
            Case enum_TipoNodo.Persona

                'Carico la pagina di cancellazione
                UrlTarget = "Cancella_Elemento.aspx" &
                                "?k=" & Stringa_Codifica(xChiave, AgroKey_EncoderDecoder, Server) &
                                "&o=" & Stringa_Codifica(enum_TipoOperazioneDB.Cancellazione, AgroKey_EncoderDecoder, Server) &
                                "&p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder, Server) &
                                "&r=" & Stringa_Codifica("AlberoImprese.aspx", AgroKey_EncoderDecoder, Server)

                'Carico la pagina
                Response.Redirect(UrlTarget)

                '------------------------------------------------

            Case Else

                r_msg = AgronicaAgenda_2010.MenuBS_Anagrafica_PreparaperCancellazione_SelezionareNodo
                Exit Function

        End Select
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Permessi_Zoo() As RispostaStandard
        Dim r As New RispostaStandard
        Try
            Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
            Dim objParametri_Utenti As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

            Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            Dim UtenteAbilitato As Boolean = objPermessi.Controlla_Permessi_Utente(
                                            HttpContext.Current.Session("ASG_Utente_Username"),
                                            HttpContext.Current.Session("ASG_IdServizio"),
                                            enum_Security_Attivita.Gest_Stalle,
                                            enum_Security_Operazione.Modifica,
                                            Date.Now,
                                            "",
                                            objParametri_Utenti)

            r.RispostaOK = True
            r.RispostaStringa = "" & UtenteAbilitato & ""

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Permessi_Pagina() As rispostaStandard(Of MenuBS_Anagrafica_Permessi)
        Dim r As New rispostaStandard(Of MenuBS_Anagrafica_Permessi)
        r.RispostaStringa = New MenuBS_Anagrafica_Permessi

        Try
            Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
            Dim objParametri_Utenti As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

            Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            Dim UtenteAbilitato As Boolean = objPermessi.Controlla_Permessi_Utente(
                                            HttpContext.Current.Session("ASG_Utente_Username"),
                                            HttpContext.Current.Session("ASG_IdServizio"),
                                            enum_Security_Attivita.Analisi_Dati_Meteo,
                                            enum_Security_Operazione.Modifica,
                                            Date.Now,
                                            "",
                                            objParametri_Utenti)

            r.RispostaStringa.PermessiMeteoDSS = UtenteAbilitato

            UtenteAbilitato = objPermessi.Controlla_Permessi_Utente(
                                            HttpContext.Current.Session("ASG_Utente_Username"),
                                            HttpContext.Current.Session("ASG_IdServizio"),
                                            enum_Security_Attivita.Gest_Stalle,
                                            enum_Security_Operazione.Modifica,
                                            Date.Now,
                                            "",
                                            objParametri_Utenti)
            r.RispostaStringa.PermessiZoo = UtenteAbilitato

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
    Public Shared Function InfoMacrousiCatasto(Piva As String, Prov As String, Com As String, SEZIONE As String, FOGLIO As Integer, NUMERO As Integer, SUBALTERNO As String) As RispostaStandard
        Dim r As New RispostaStandard

        Try
            Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
            Dim objParametri_Utenti As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

            Dim objCatastoMacrousi As New AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousi_R
            Dim DT = objCatastoMacrousi.Leggi(Piva, Prov, Com, SEZIONE, FOGLIO, NUMERO, SUBALTERNO, "", enumSelezioneVariabile.Selezione_JoinDescrizioni, "", "", objParametri_Server)

            Dim l As New List(Of ColonneNome)

            l.Add(New ColonneNome("Macrouso_Cod", AgronicaAgenda_2010.CodiceMacrousoAbbr, "string"))
            l.Add(New ColonneNome("Macrouso_Des", AgronicaAgenda_2010.Macrouso, "string"))
            l.Add(New ColonneNome("Superficie", AgronicaAgenda_2010.SuperficieAbbr & " [ha]", "number"))

            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            Dim risp As String = js.JSON_DataTable_Kendo(DT, l, False, False, TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa, "")

            r.RispostaOK = True
            r.RispostaStringa = risp

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function InfoUtilizziCatasto(Piva As String, Prov As String, Com As String, SEZIONE As String, FOGLIO As Integer, NUMERO As Integer, SUBALTERNO As String) As RispostaStandard
        Dim r As New RispostaStandard

        Try
            Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
            Dim objParametri_Utenti As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

            Dim objCatastoUtilizzi As New AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousixUtilizzo_R
            Dim DT = objCatastoUtilizzi.Leggi(Piva, Prov, Com, SEZIONE, FOGLIO, NUMERO, SUBALTERNO, "", "", "", enumSelezioneVariabile.Selezione_JoinDescrizioni, "", "", objParametri_Server)

            Dim l As New List(Of ColonneNome)

            l.Add(New ColonneNome("Veg_Des_Agea", AgronicaAgenda_2010.Specie, "string"))
            l.Add(New ColonneNome("Cul_Des_Agea", AgronicaAgenda_2010.Varietà, "string"))
            l.Add(New ColonneNome("Superficie", AgronicaAgenda_2010.SuperficieAbbr & " [ha]", "number"))

            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            Dim risp As String = js.JSON_DataTable_Kendo(DT, l, False, False, TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa, "")

            r.RispostaOK = True
            r.RispostaStringa = risp

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function InfoClassamentoCatasto(Piva As String, Prov As String, Com As String, SEZIONE As String, FOGLIO As Integer, NUMERO As Integer, SUBALTERNO As String) As RispostaStandard
        Dim r As New RispostaStandard

        Try
            Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
            Dim objParametri_Utenti As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

            Dim objCatastoMacrousi As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_R
            Dim DT = objCatastoMacrousi.LeggixChiave_conClassamento(Prov, Com, SEZIONE, FOGLIO, NUMERO, SUBALTERNO, enumSelezioneVariabile.Selezione_JoinDescrizioni, "", "", objParametri_Server)

            Dim l As New List(Of ColonneNome)

            l.Add(New ColonneNome("Porzione", AgronicaAgenda_2010.Porzione, "string"))
            l.Add(New ColonneNome("Qualita_Des", AgronicaAgenda_2010.Qualità, "string"))
            l.Add(New ColonneNome("Classe", AgronicaAgenda_2010.Classe, "string"))
            l.Add(New ColonneNome("Sup_Classe", AgronicaAgenda_2010.Superficie & " [ha]", "number"))
            l.Add(New ColonneNome("Reddito_Dominicale", AgronicaAgenda_2010.RedditoDominicale, "string"))
            l.Add(New ColonneNome("Reddito_Agrario", AgronicaAgenda_2010.RedditoAgrario, "string"))

            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            Dim risp As String = js.JSON_DataTable_Kendo(DT, l, False, False, TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa, "")

            r.RispostaOK = True
            r.RispostaStringa = risp

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function InfoAppezzamentiCatasto(Piva As String, Prov As String, Com As String, SEZIONE As String, FOGLIO As Integer, NUMERO As Integer, SUBALTERNO As String) As RispostaStandard
        Dim r As New RispostaStandard

        Try
            Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
            Dim objParametri_Utenti As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

            Dim objAppezzaxParticelle As New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R
            Dim DT = objAppezzaxParticelle.AppezzamentixParticelle_Leggi(
                                                                  CStr(Piva),
                                                                  0,
                                                                  CInt(0),
                                                                  Prov,
                                                                  Com,
                                                                  SEZIONE,
                                                                  FOGLIO,
                                                                  NUMERO,
                                                                  SUBALTERNO,
                                                                  AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                  "",
                                                                  "",
                                                                  objParametri_Server,
                                                                  False)

            DT.Columns.Add(New DataColumn("Date"))
            DT.Columns.Add(New DataColumn("Impianto"))
            DT.Columns.Add(New DataColumn("Cul_Cod_Agea", GetType(String)))
            DT.Columns.Add(New DataColumn("Validita_Inizio_Impianto", GetType(String)))
            DT.Columns.Add(New DataColumn("Validita_Fine_Impianto", GetType(String)))

            Dim DataInizio As String
            Dim DataFine As String

            Dim DataInizioImpianto As String
            Dim DataFineImpianto As String

            Dim Cul_Cod As Integer
            Dim Cul_Des As String
            Dim Veg_Des As String
            Dim Testo As String
            Dim Data As String = ""

            Dim objRegImp As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read

            For Each row In DT.Rows

                If CDate(row("xValidita_Inizio")).ToShortDateString <> "01/01/1900" Then
                    DataInizio = CDate(row("xValidita_Inizio")).ToShortDateString
                Else
                    DataInizio = "..."
                End If

                If CDate(row("xValidita_Fine")).ToShortDateString <> "31/12/2100" Then
                    DataFine = CDate(row("xValidita_Fine")).ToShortDateString
                Else
                    DataFine = "..."
                End If

                row("Date") = "Dal " & DataInizio & " al " & DataFine

                Data = ""

                Dim DTImpianto = objRegImp.Leggi(Piva,
                                                 0,
                                                 CInt(row("Appezza")),
                                                 0,
                                                 AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                 "",
                                                 "",
                                                 objParametri_Server)

                'Se il recordset non e' nullo
                If DTImpianto.Rows.Count > 0 Then

                    Cul_Cod = DTImpianto.Rows(0).Item("Cul_cod")

                    If Cul_Cod <> 0 Then

                        Cul_Des = DTImpianto.Rows(0).Item("Cul_Des")
                        Veg_Des = DTImpianto.Rows(0).Item("Veg_Des")

                        Testo = Data & "-" & Veg_Des & "-" & Cul_Des
                    Else
                        Testo = Data & "-Terreno Nudo"

                    End If

                    'se l'impianto non è scaduto..
                    If DTImpianto.Rows(0).Item("Validita_Fine") > Now.Today Then

                        If DTImpianto.Rows(0).Item("Validita_Inizio") <> "01/01/1900" Then
                            Data = DTImpianto.Rows(0).Item("Validita_Inizio")
                        Else
                            Data = ""
                        End If

                    Else
                        Data = ""
                    End If

                    row("Validita_Inizio_Impianto") = CDate(DTImpianto.Rows(0).Item("Validita_Inizio")).ToShortDateString
                    row("Validita_Fine_Impianto") = CDate(DTImpianto.Rows(0).Item("Validita_Fine")).ToShortDateString

                    row("impianto") = Testo

                Else
                    row("impianto") = "."
                End If

            Next

            Dim l As New List(Of ColonneNome)

            l.Add(New ColonneNome("Date", AgronicaAgenda_2010.PeriodoIntersezione, "string"))
            l.Add(New ColonneNome("App_Nome", AgronicaAgenda_2010.Nome, "string"))
            l.Add(New ColonneNome("Impianto", AgronicaAgenda_2010.ImpiantoIntersecato, "string"))
            l.Add(New ColonneNome("Validita_Inizio_Impianto", "Validita Inizio Impianto", "string"))
            l.Add(New ColonneNome("Validita_Fine_Impianto", "Validita Fine Impianto", "string"))
            l.Add(New ColonneNome("AREA", AgronicaAgenda_2010.SuperficieIntersecataAbbr & " [ha]", "number"))
            l.Add(New ColonneNome("Sup_App", AgronicaAgenda_2010.SuperficieAppezzamentoAbbr & " [ha]", "number"))

            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            Dim risp As String = js.JSON_DataTable_Kendo(DT, l, False, False, TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa, "")

            r.RispostaOK = True
            r.RispostaStringa = risp

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function sblocca_appezzamenti(ByVal chiavi_appezza As String()) As RispostaStandard
        Dim r As New RispostaStandard

        Try
            Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
            Dim objParametri_Utenti As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

            Dim risp As String = ""

            Dim objAppezza_r As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
            Dim objAppezza_w As New AgronicaCoreAnagrafeDAL.Appezzamento_Write
            Dim lista_nomi As New List(Of String)

            For Each chiave In chiavi_appezza
                Dim Piva = chiave.Split("_")(0)
                Dim sa_cod = CInt(chiave.Split("_")(1))
                Dim appezza = CInt(chiave.Split("_")(2))

                If objAppezza_r.VerificaAppezzamentoBloccato(Piva, sa_cod, appezza, objParametri_Server) Then

                    If objAppezza_w.Appezza_Sblocca(Piva, sa_cod, appezza, "", objParametri_Server) Then

                        Dim app_nome As String = objAppezza_r.AppezzamentoNome_from_Appezza(Piva, sa_cod, appezza, objParametri_Server)
                        lista_nomi.Add(app_nome)

                    Else

                        r.RispostaOK = False
                        r.Errore = ""
                        Return r
                    End If

                End If

            Next

            If lista_nomi.Count = 0 Then
                r.RispostaOK = True
                r.RispostaStringa = AgronicaAgenda_2010.MenuBS_Anagrafica_sblocca_appezzamenti_Seleziona
                Return r

            ElseIf lista_nomi.Count = 1 Then

                r.RispostaOK = True
                r.RispostaStringa = String.Format(AgronicaAgenda_2010.MenuBS_Anagrafica_sblocca_appezzamenti_Sbloccato, lista_nomi(0))
                Return r

            Else

                r.RispostaOK = True
                risp = AgronicaAgenda_2010.MenuBS_Anagrafica_sblocca_appezzamenti_Seguenti

                Dim i = 0
                For Each app_nome In lista_nomi
                    If i = 0 Then
                        risp &= app_nome
                    Else
                        risp &= ", " & app_nome & ""
                    End If
                    i += 1
                Next
                risp &= AgronicaAgenda_2010.MenuBS_Anagrafica_sblocca_appezzamenti_Sbloccati
                r.RispostaStringa = risp
                Return r

            End If

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function blocca_appezzamenti(ByVal chiavi_appezza As String()) As RispostaStandard
        Dim r As New RispostaStandard

        Try
            Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
            Dim objParametri_Utenti As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

            Dim obj_permessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            Dim UtenteAbilitato As Boolean = False
            UtenteAbilitato = obj_permessi.Controlla_Permessi_Utente(HttpContext.Current.Session("ASG_Utente_Username"),
                                                                    HttpContext.Current.Session("ASG_IdServizio"),
                                                                    enum_Security_Attivita.ManutenzioneArchivi_SbloccaAnagrafe,
                                                                    enum_Security_Operazione.Modifica, Date.Now, "", objParametri_Utenti)
            If Not UtenteAbilitato Then
                r.RispostaConferma = False
                r.Errore = AgronicaAgenda_2010.UtenteNonAbilitato
                Return r
            End If


            Dim risp As String = ""

            Dim objAppezza_r As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
            Dim objAppezza_w As New AgronicaCoreAnagrafeDAL.Appezzamento_Write
            Dim lista_nomi As New List(Of String)

            For Each chiave In chiavi_appezza
                Dim Piva = chiave.Split("_")(0)
                Dim sa_cod = CInt(chiave.Split("_")(1))
                Dim appezza = CInt(chiave.Split("_")(2))

                If Not objAppezza_r.VerificaAppezzamentoBloccato(Piva, sa_cod, appezza, objParametri_Server) Then

                    If objAppezza_w.Appezza_Blocca(Piva, sa_cod, appezza, "", objParametri_Server) Then

                        Dim app_nome As String = objAppezza_r.AppezzamentoNome_from_Appezza(Piva, sa_cod, appezza, objParametri_Server)
                        lista_nomi.Add(app_nome)

                    Else

                        r.RispostaOK = False
                        r.Errore = ""
                        Return r
                    End If

                End If

            Next

            If lista_nomi.Count = 0 Then
                r.RispostaOK = True
                r.RispostaStringa = AgronicaAgenda_2010.MenuBS_Anagrafica_blocca_appezzamenti_Seleziona
                Return r

            ElseIf lista_nomi.Count = 1 Then

                r.RispostaOK = True
                r.RispostaStringa = String.Format(AgronicaAgenda_2010.MenuBS_Anagrafica_blocca_appezzamenti_Bloccato, lista_nomi(0))
                Return r

            Else

                r.RispostaOK = True
                risp = AgronicaAgenda_2010.MenuBS_Anagrafica_blocca_appezzamenti_Seguenti

                Dim i = 0
                For Each app_nome In lista_nomi
                    If i = 0 Then
                        risp &= app_nome
                    Else
                        risp &= ", " & app_nome & ""
                    End If
                    i += 1
                Next
                risp &= AgronicaAgenda_2010.MenuBS_Anagrafica_blocca_appezzamenti_Bloccati
                r.RispostaStringa = risp
                Return r

            End If

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function ModificaMultipla(ByVal parametri As String, ByVal dati As String) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            Dim objParametri_Server As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

            Dim objEreditatore As New AgronicaCoreAnagrafeBIZ.Ereditatore
            r = objEreditatore.ModificaMultipla(parametri, dati, objParametri_Server)

        Catch ex As GiasException
            'Errore gestito
            r.RispostaStringa = "Impossibile proseguire con il salvataggio: <br>" & ex.Message
            r.RispostaOK = False
        Catch ex As Exception
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiDisciplinarePrivato() As RispostaStandard

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

            Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

            Dim leggiPrivato As String = "false"
            Dim ret = objConfSiti.Leggi_Valore(0, "Flag_DisciplinarePrivato", "", "", objParametri_Server)

            If ret <> "" Then
                r.RispostaStringa = ret.ToLower
            End If
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Specie() As RispostaStandard

        Return Prodotto_Edit_UC.Leggi_Specie()

    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Varieta(filtro_specie As String) As RispostaStandard

        Return Prodotto_Edit_UC.Leggi_Varieta(filtro_specie)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Categorie_Commerciali(ByVal piva As String) As RispostaStandard
        Return Prodotto_Edit_UC.Leggi_Categorie_Commerciali(piva)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function EditProdotto(ByVal piva As String,
                                        ByVal mat_cod As String,
                                        ByVal elem_cod As String,
                                        ByVal prodotto_des As String,
                                        ByVal pro_cod As String,
                                        ByVal duplica As Boolean,
                                        ByVal proprietario As Boolean,
                                        ByVal sa_cod As String,
                                        ByVal isalias As Boolean) As String


        Dim objParametriAgenda = New ParametriAgenda
        objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
        objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Anagrafica_Menu
        'Salvo la PIVA del centro aziendale per poter risettare corretamente il filtro delle tabelle al torna indietro
        objParametriAgenda.Piva_Origine = ""
        objParametriAgenda.Piva_Origine = objParametriAgenda.Piva
        'objParametriAgenda.Sa_Cod = chiave.Split("_")(1)

        Dim operazione As String
        Dim url As String = "../Anagrafica/Prodotto_Edit.aspx"

        'Se duplica è true allora si è cliccata il pulsante Duplica
        If (duplica = True) Then
            operazione = "?o=" & Stringa_Codifica(enum_TipoOperazioneDB.Copia, AgroKey_EncoderDecoder, Nothing)
            'Altrimenti siamo solo in Modifica
        Else
            operazione = "?o=" & Stringa_Codifica(enum_TipoOperazioneDB.Modifica, AgroKey_EncoderDecoder, Nothing)
        End If
        Dim pivaParam As String = "&piva=" & Stringa_Codifica(piva, AgroKey_EncoderDecoder, Nothing)
        Dim matCodParam As String = "&mat_cod=" & Stringa_Codifica(mat_cod, AgroKey_EncoderDecoder, Nothing)
        Dim elemCodParam As String = "&elem_cod=" & Stringa_Codifica(elem_cod, AgroKey_EncoderDecoder, Nothing)
        Dim duplicaParam As String = "&duplica=" & duplica
        Dim tuttiTabParam As String = "&proprietario=" & Stringa_Codifica(proprietario, AgroKey_EncoderDecoder, Nothing)
        Dim proCodParam As String = "&pro_cod=" & Stringa_Codifica(pro_cod, AgroKey_EncoderDecoder, Nothing)
        Dim prodottoDesParam As String = "&prodotto_des=" & Stringa_Codifica(prodotto_des, AgroKey_EncoderDecoder, Nothing)
        Dim saCodParam As String = "&sa_cod=" & Stringa_Codifica(sa_cod, AgroKey_EncoderDecoder, Nothing)
        Dim isAliasParam As String = "&isalias=" & Stringa_Codifica(isalias, AgroKey_EncoderDecoder, Nothing)

        Dim queryString As String = operazione & pivaParam & matCodParam & elemCodParam & duplicaParam & tuttiTabParam & proCodParam & prodottoDesParam & saCodParam & isAliasParam
        Return url & queryString

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function InfoProdotto(ByVal piva As String,
                                        ByVal mat_cod As String,
                                        ByVal elem_cod As String,
                                        ByVal prodotto_des As String,
                                        ByVal pro_cod As String,
                                        ByVal sa_cod As String,
                                        ByVal isalias As Boolean) As String

        Dim objParametriAgenda = New ParametriAgenda
        objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
        objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Anagrafica_Menu
        'Salvo la PIVA del centro aziendale per poter risettare corretamente il filtro delle tabelle al torna indietro
        objParametriAgenda.Piva_Origine = ""
        objParametriAgenda.Piva_Origine = objParametriAgenda.Piva
        'objParametriAgenda.Sa_Cod = chiave.Split("_")(1)

        Dim url As String = "../Anagrafica/Prodotto_Edit.aspx"
        Dim operazione As String = "?o=" & Stringa_Codifica(enum_TipoOperazioneDB.Lettura, AgroKey_EncoderDecoder, Nothing)
        Dim pivaParam As String = "&piva=" & Stringa_Codifica(piva, AgroKey_EncoderDecoder, Nothing)
        Dim matCodParam As String = "&mat_cod=" & Stringa_Codifica(mat_cod, AgroKey_EncoderDecoder, Nothing)
        Dim elemCodParam As String = "&elem_cod=" & Stringa_Codifica(elem_cod, AgroKey_EncoderDecoder, Nothing)
        Dim proCodParam As String = "&pro_cod=" & Stringa_Codifica(pro_cod, AgroKey_EncoderDecoder, Nothing)
        Dim prodottoDesParam As String = "&prodotto_des=" & Stringa_Codifica(prodotto_des, AgroKey_EncoderDecoder, Nothing)
        Dim saCodParam As String = "&sa_cod=" & Stringa_Codifica(sa_cod, AgroKey_EncoderDecoder, Nothing)
        Dim isAliasParam As String = "&isalias=" & Stringa_Codifica(isalias, AgroKey_EncoderDecoder, Nothing)

        Dim queryString As String = operazione & pivaParam & matCodParam & elemCodParam & proCodParam & prodottoDesParam & saCodParam & isAliasParam
        Return url & queryString

    End Function

    Public Shared Function NuovoProdotto(ByVal elem_cod As String, Optional ByVal isalias As Boolean = False) As String

        Dim objParametriAgenda = New ParametriAgenda
        objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
        objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Anagrafica_Menu
        'Salvo la PIVA del centro aziendale per poter risettare corretamente il filtro delle tabelle al torna indietro
        objParametriAgenda.Piva_Origine = ""
        objParametriAgenda.Piva_Origine = objParametriAgenda.Piva


        Dim url As String = "../Anagrafica/Prodotto_Edit.aspx"

        Dim operazione As String = "?o=" & Stringa_Codifica(enum_TipoOperazioneDB.Scrittura, AgroKey_EncoderDecoder, Nothing)
        Dim pivaParam As String = "&piva=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder, Nothing)
        Dim matCodParam As String = "&mat_cod=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Nothing)
        Dim elemCodParam As String = "&elem_cod=" & Stringa_Codifica(elem_cod, AgroKey_EncoderDecoder, Nothing)
        Dim proCodParam As String = "&pro_cod=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Nothing)
        Dim tuttiTabParam As String = "&proprietario=" & Stringa_Codifica(True, AgroKey_EncoderDecoder, Nothing)
        Dim saCodParam As String = "&sa_cod=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Nothing)
        Dim isAliasParam As String = "&isalias=" & Stringa_Codifica(isalias, AgroKey_EncoderDecoder, Nothing)

        Dim queryString As String = operazione & pivaParam & matCodParam & elemCodParam & proCodParam & tuttiTabParam & saCodParam & isAliasParam
        Return url & queryString

    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function PermessoModificaProdotto(ByVal piva As String, ByVal sa_cod As Integer) As RispostaStandard
        Return Prodotto_Edit_UC.PermessoModificaProdotto(piva, sa_cod)
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function CreaOperazione(ByVal lav_cod As Integer, ByVal gru_cod As Integer) As RispostaStandard
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
            Dim gru_cod_list As New List(Of Integer)({1, 2, 3, 4, 5, 100})
            Dim veg_cod = 0
            Dim PaginaLink As String = MenuBS_Agenda_Nuovo.NuovaOperazioneAgenda(lav_cod, veg_cod)
            If PaginaLink = "error" Then
                r.Errore = "TODO Non si hanno i permessi per visualizzare questo tipo di operazione"
                Return r
            End If

            'PaginaLink &= "?PaginaOrigine=" & CStr(enum_PagineGiasOnline.MenuAnagrafica) 'Quando creo un redirect per Angular, si schianta 
            Dim objParametri_Agenda As New ParametriAgenda
            Dim objParametriAgenda = New ParametriAgenda
            Dim impiantiList = New List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)

            Dim id_cod As Integer = 0

            Dim obj_Impianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
            Dim obj_Impianti_codici As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R
            Dim dt_impianto As New DataTable

            If gru_cod_list.Contains(gru_cod) Then
                dt_impianto = obj_Impianti.Leggi(objParametriAgenda.Piva,
                                                 objParametriAgenda.Sa_Cod,
                                                 objParametriAgenda.Appezza,
                                                 objParametriAgenda.Id_Imp,
                                                 enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                 "", "", objParametri_Server)
            End If

            Dim data_Operazione = DateTime.Today

            If gru_cod_list.Contains(gru_cod) AndAlso dt_impianto.Rows.Count > 0 Then

                Dim validita_inizio = CDate(dt_impianto.Rows(0)("Validita_Inizio"))
                Dim validita_fine = CDate(dt_impianto.Rows(0)("Validita_Fine"))

                If validita_inizio > Date.Now Then
                    data_Operazione = validita_inizio
                ElseIf validita_fine < Date.Now Then
                    data_Operazione = validita_fine
                End If

                If lav_cod = LAVCOD_SEMINA Or
                    lav_cod = LAVCOD_TRAPIANTO Or
                    lav_cod = LAVCOD_TRAPIANTO_IN_SERRA Then
                    data_Operazione = validita_inizio
                End If

            End If

            objParametriAgenda.Data = data_Operazione

            Dim cul_cod = 0

            If gru_cod_list.Contains(gru_cod) Then
                cul_cod = obj_Impianti.CulCod_from_PivaSaCodAppezzaIdimp(objParametriAgenda.Piva,
                                                            objParametriAgenda.Sa_Cod,
                                                            objParametriAgenda.Appezza,
                                                            objParametriAgenda.Id_Imp,
                                                            objParametri_Server)

                If cul_cod = 0 Then
                    id_cod = obj_Impianti.Leggi_DestinazioneUso_Impianto(objParametriAgenda.Piva,
                                                                objParametriAgenda.Sa_Cod,
                                                                objParametriAgenda.Appezza,
                                                                objParametriAgenda.Id_Imp,
                                                                "", "", objParametri_Server)
                End If
            End If

            Dim objCultivar As New AgronicaCoreMetaSchemaDAL.Cultivar_R
            Dim veg_cod_s As String = ""

            If gru_cod_list.Contains(gru_cod) Then
                If cul_cod <> 0 Then
                    veg_cod_s = objCultivar.VegCod_from_CulCod(cul_cod, objParametri_Server)
                    veg_cod = veg_cod_s
                Else
                    veg_cod_s = "0/" & id_cod
                End If
            End If



            objParametriAgenda.Veg_Cod = veg_cod_s

            If gru_cod_list.Contains(gru_cod) Then
                Dim imp = New AgronicaCoreModello.ParametriAgenda_Temp.Impianto
                imp.Piva = objParametriAgenda.Piva
                imp.Sa_Cod = objParametriAgenda.Sa_Cod
                imp.Appezza = objParametriAgenda.Appezza
                imp.ID_Reg = objParametriAgenda.Id_Imp
                impiantiList.Add(imp)
                objParametriAgenda.Impianti = impiantiList
            End If

            objParametriAgenda.TipoOperazioneAgenda = "1"

            'objParametriAgenda.Piva = piva
            'objParametriAgenda.Sa_Cod = sa_cod
            'objParametriAgenda.Appezza = appezza
            'objParametriAgenda.Id_Imp = id_reg

            objParametriAgenda.Lav_Cod = lav_cod
            objParametriAgenda.PaginaSitoOrigine = enum_PagineGiasOnline.MenuAnagrafica
            objParametriAgenda.salva()

            Dim objParametriAgenda_2010 As New ParametriAgenda_2010
            objParametriAgenda_2010.PaginaRichiesta = enum_PagineAgenda_2010.Pagina_Lavorazioni
            objParametriAgenda_2010.PaginaProvenienza = enum_PagineAgenda_2010.Pagina_Anagrafica_Impianto
            'Dim PaginaLink As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriAgenda_2010(
            '    Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objParametriAgenda_2010
            ')

            r.RispostaOK = True
            r.RispostaStringa = PaginaLink

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function dateValiditaAgenda() As RispostaStandard
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

            Dim objRes As New JObject

            objRes("Validita_Inizio") = CDate(objParametri_Server.FinestraTemporaleInizio)
            objRes("Validita_Fine") = CDate(objParametri_Server.FinestraTemporaleFine)

            r.RispostaOK = True
            r.RispostaStringa = objRes.ToString

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function

    <Script.Services.ScriptMethod()> <WebMethod(EnableSession:=True)>
    Public Shared Function GestioneStampe(ByVal report As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametriAgenda As New ParametriAgenda
        Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        Dim objParametri_Utenti As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

        r.RispostaOK = True
        r.Tipo = ""
        r.ParametroDue_stringa = ""
        r.RispostaStringa = ""

        Dim piva As String = objParametriAgenda.Piva
        Dim sa_cod As String = objParametriAgenda.Sa_Cod
        Dim Origine As String = Stringa_Codifica("../Stampe/MenuStampe.aspx", AgroKey_EncoderDecoder, objParametri_Server)
        Dim Destinazione As String = Stringa_Codifica("../GestioneStampe/ChiamaStampe.aspx", AgroKey_EncoderDecoder, objParametri_Server)
        Dim Funzione As String = Stringa_Codifica(CStr(enum_TipoFiltrone.Stampa), AgroKey_EncoderDecoder, objParametri_Server)

        Select Case report

            Case enum_CodificaStampe.Quadro_P, enum_CodificaStampe.RiepilogoImpiegoSuperfici

                Dim vVarStampe(1) As ElementoStampe
                vVarStampe(0).Nome = "piva"
                vVarStampe(0).Valore = piva

                vVarStampe(1).Nome = "sa_cod"
                vVarStampe(1).Valore = sa_cod

                Dim objVS As New AgronicaCoreXML.XML_Stampe
                Dim StrNodo As String = objVS.XML_VariabiliStampe(vVarStampe)
                Dim StrNodiVariabili As String = StrNodo

                Dim ParametriAgronicaStampe As New AgronicaCoreGestioneRichieste.ParametriAgronicaStampe
                ParametriAgronicaStampe.report = report
                ParametriAgronicaStampe.username = CStr(HttpContext.Current.Session("ASG_Utente_Username"))
                ParametriAgronicaStampe.user_profilo = CStr(HttpContext.Current.Session("ASG_ProgressivoGIAS"))
                ParametriAgronicaStampe.Xml_Generico.Length = 0
                ParametriAgronicaStampe.Xml_Generico.Append(StrNodiVariabili)
                'RedirectURL = RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(Enum_SiteRedirector.Sito_AgronicaStampe_2010, objAgronicaStampe)
                'RedirectURL &= "?p=" & Stringa_Codifica(piva, AgroKey_EncoderDecoder, HttpContext.Current.Session)

                'Dim ParametriAgronicaStampe As ParametriAgronicaStampe = RedirectGestione.Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(report, piva, HttpContext.Current.Session, objParametri_Server, Sa_Cod:=sa_cod)
                'Dim strJS As String = RedirectGestione.ApriPopUp_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, ParametriAgronicaStampe)
                Dim strJS = RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, ParametriAgronicaStampe)
                r.Tipo = "1"
                r.ParametroDue_stringa = strJS

        End Select

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function modificaMultiplaRedirect(ByVal obj_Impianto_str, ByVal capiSelezionatiModificaMultipla, Qs_Visibilita) As RispostaStandard

        Dim r As New RispostaStandard



        Try
            Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
            Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))
            Dim obj_Impianto = Newtonsoft.Json.JsonConvert.DeserializeObject(obj_Impianto_str)
            Dim objImpostazioni_Utenti As New Utenti_Impostazioni_Read
            Dim piva As String = obj_Impianto.GetValue("piva").ToString()
            Dim sa_cod As Integer = CInt(obj_Impianto.GetValue("sa_cod"))
            Dim DefaultRegolamento = objImpostazioni_Utenti.Impostazione_Valore_From_Impostazione_Cod_Utente_Poi_SuperUser(enum_Impostazioni_Utenti.UTENTE_COD_REGOLAMENTO, objParametri_Utenti)

            If IsNumeric(DefaultRegolamento) AndAlso CInt(DefaultRegolamento) < 1 Then
                DefaultRegolamento = 1
            End If

            Dim NoteLog As String = NOTELOG_ANAGRAFE_BOOTSTRAP

            r.ParametroDue = True
            r.ParametroDue_stringa = ""

            Dim TargetUrl = "../Anagrafica/Modifica_Multipla_zoo.aspx"
            If Qs_Visibilita <> 0 Then
                TargetUrl &= "?visibilita=" & CStr(Qs_Visibilita)
            End If

            Dim objParametriAgenda = New ParametriAgenda
            objParametriAgenda.Svuota_DatiOperazione()
            objParametriAgenda.Piva = piva
            objParametriAgenda.Raggruppamento_Cod = 0
            Dim ListaImpianti As New List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)
            Dim counter As Integer = 0

            'Loop creato perchè creando un loop con la lista impianti senza prima inizializzarla ogni Progetto_Cod precedente veniva sovrascritto con l'ultimo Progetto_Cod nel loop.
            For a = 1 To capiSelezionatiModificaMultipla.Length
                ListaImpianti.Add(New AgronicaCoreModello.ParametriAgenda_Temp.Impianto)
            Next

            Dim Imp As New AgronicaCoreModello.ParametriAgenda_Temp.Impianto

            For Each i In capiSelezionatiModificaMultipla
                System.Diagnostics.Debug.WriteLine(i.item("Cod_Animale"))
                Imp.Progetto_Cod = i.item("Cod_Animale")
                ListaImpianti(counter).Progetto_Cod = Imp.Progetto_Cod
                counter += 1
            Next

            objParametriAgenda.Impianti = ListaImpianti
            objParametriAgenda.TipoOperazioneAgenda = "1"
            objParametriAgenda.Lav_Cod = 0
            objParametriAgenda.salva()

            r.ParametroDue_stringa = TargetUrl
            r.RispostaOK = True

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try


        Return r

    End Function

End Class