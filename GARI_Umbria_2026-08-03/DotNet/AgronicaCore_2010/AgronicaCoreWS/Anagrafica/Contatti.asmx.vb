Imports System.Web.Services
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreDTOStd.InData.Anagrafica
Imports AgronicaCoreModelsSTD.anagrafiche
Imports InData.Anagrafica
Imports AgronicaCoreModello

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class Contatti
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Contatti_Per_Piva(ByVal objP_server As String, ByVal piva As String) As RispostaStandard
        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R
            Dim dtContatti As DataTable = objContatti.Contatti_Contatto_Leggi(piva,
                                                                              "", 0, 0, False, False, 0, 0, False, 0,
                                                                              ID_CF_NOFILTRO,
                                                                              0, "", False, 0, 0, 0, 0, 0,
                                                                              enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                                              "", "", objParametriServer)

            Dim jArrayListaOp As New JArray()

            For Each dr As DataRow In dtContatti.Rows
                Dim ragSoc As String = CStr(dr.Item("Rag_Soc")) & CStr(dr.Item("Cognome")) & " " & CStr(dr.Item("Nome")) & " (" & dr.Item("Rapporto_Des") & ")"

                jArrayListaOp.Add(New JObject(New JProperty("cod_contatto", dr.Item("cod_contatto")),
                                              New JProperty("nome", ragSoc.Replace("""", "'"))))
            Next

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Contatti_Per_Piva_NG(InData As Object) As RispostaStandard
        Dim r As New RispostaStandard()


        InData = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData))

        Dim piva As String = InData.InData

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If


        Try
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R
            Dim dtContatti As DataTable = objContatti.Contatti_Contatto_Leggi(piva,
                                                                              "", 0, 0, False, False, 0, 0, False, 0,
                                                                              ID_CF_NOFILTRO,
                                                                              0, "", False, 0, 0, 0, 0, 0,
                                                                              enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                                              "", "", objParametriServer)

            Dim jArrayListaOp As New JArray()

            For Each dr As DataRow In dtContatti.Rows
                Dim ragSoc As String = CStr(dr.Item("Rag_Soc")) & CStr(dr.Item("Cognome")) & " " & CStr(dr.Item("Nome")) & " (" & dr.Item("Rapporto_Des") & ")"

                jArrayListaOp.Add(New JObject(New JProperty("cod_contatto", dr.Item("cod_contatto")),
                                              New JProperty("nome", ragSoc.Replace("""", "'"))))
            Next

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function EliminaContatto(InData As CoreWS_Generic(Of AgronicaCoreModelsSTD.anagrafiche.Contatto)) As RispostaStandard

        Dim r As New RispostaStandard()
        Dim msg As String
        Dim esito As Boolean = False

        Try
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
            Dim contatto As AgronicaCoreModelsSTD.anagrafiche.Contatto = InData.InData

            If objParametri_Server Is Nothing Then
                r.Errore = "objP_server non valorizzato"
                Return r
            End If
            If objParametri_Utenti Is Nothing Then
                r.Errore = "objP_utenti non valorizzato"
                Return r
            End If
            If objParametri_Super_Server Is Nothing Then
                r.Errore = "objP_super_server non valorizzato"
                Return r
            End If

            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            Dim contattiW As New AgronicaCoreAnagrafeBIZ.Contatti_W
            msg = contattiW.EliminaContatto(contatto.primaryKey.partitaIva, contatto.sa_cod,
                                      contatto.primaryKey.codice, esito,
                                      objParametri_Server)

            r.RispostaOK = esito
            r.RispostaStringa = msg

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function AssociaUtente(InData As CoreWS_Generic(Of AssociaUtente)) As RispostaStandard

        Dim r As New RispostaStandard()
        Dim msg As String

        Try
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
            Dim iData = InData.InData

            If objParametri_Server Is Nothing Then
                r.Errore = "objP_server non valorizzato"
                Return r
            End If
            If objParametri_Utenti Is Nothing Then
                r.Errore = "objP_utenti non valorizzato"
                Return r
            End If
            If objParametri_Super_Server Is Nothing Then
                r.Errore = "objP_super_server non valorizzato"
                Return r
            End If

            Dim contattiW As New AgronicaCoreAnagrafeBIZ.Contatti_W
            msg = contattiW.AssociaUtente(iData.piva, iData.contatto_cod, iData.username, objParametri_Server)

            r.RispostaOK = True
            r.RispostaStringa = msg

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiUtentiDaAssociare(InData As CoreWS_Generic(Of String)) As RispostaStandard

        Dim r As New RispostaStandard()
        Dim dt As DataTable

        Try
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            If objParametri_Server Is Nothing Then
                r.Errore = "objP_server non valorizzato"
                Return r
            End If
            If objParametri_Utenti Is Nothing Then
                r.Errore = "objP_utenti non valorizzato"
                Return r
            End If
            If objParametri_Super_Server Is Nothing Then
                r.Errore = "objP_super_server non valorizzato"
                Return r
            End If

            Dim contattiR As New AgronicaCoreAnagrafeBIZ.Contatti_MultiHost_R
            dt = contattiR.LeggiUtentiDaAssociare(objParametri_Server, objParametri_Utenti)

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(dt)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiUtenteAssociato(InData As CoreWS_Generic(Of AssociaUtente)) As RispostaStandard

        Dim r As New RispostaStandard()
        Dim dt As DataTable

        Try
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            If objParametri_Server Is Nothing Then
                r.Errore = "objP_server non valorizzato"
                Return r
            End If
            If objParametri_Utenti Is Nothing Then
                r.Errore = "objP_utenti non valorizzato"
                Return r
            End If
            If objParametri_Super_Server Is Nothing Then
                r.Errore = "objP_super_server non valorizzato"
                Return r
            End If

            Dim iData = InData.InData

            Dim contattiR As New AgronicaCoreAnagrafeBIZ.Contatti_MultiHost_R
            dt = contattiR.LeggiUtenteAssociato(iData.piva, iData.contatto_cod, objParametri_Server)

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(dt)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiRapportoSpecifico(ByVal objP_server As String,
                                           ByVal piva As String,
                                           ByVal rapportoAttivo As Boolean,
                                           ByVal cliente As Boolean,
                                           ByVal fornitore As Boolean,
                                           ByVal dipendente As Boolean,
                                           ByVal terzista As Boolean,
                                           ByVal legale As Boolean,
                                           ByVal agente As Boolean,
                                           ByVal consulente As Boolean,
                                           ByVal Cod_Contatto As String
                                           ) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim objContatti As New AgronicaCoreAnagrafeDAL.Risorse_Umane_R

            Dim filtro As String = ""
            If Cod_Contatto <> "" Then
                filtro = " Contatti.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "' "
            End If

            Dim dtContatti As New DataTable
            dtContatti = objContatti.LeggiRapportoSpecifico(piva,
                                                            filtro,
                                                            " Contatti.Rag_Soc ASC, Contatti.Cognome ASC, Contatti.Nome ASC ",
                                                            objParametriServer,
                                                            Rapporto_Attivo:=rapportoAttivo,
                                                            Cliente:=cliente,
                                                            Fornitore:=fornitore,
                                                            Dipendente:=dipendente,
                                                            Terzista:=terzista,
                                                            Legale:=legale,
                                                            Agente:=agente,
                                                            Consulente:=consulente)

            Dim jArrayListaOp As New JArray()

            For Each dr As DataRow In dtContatti.Rows

                Dim partitaIva As String = ""
                Dim codContattoRiga = Trim(dr.Item("Cod_Contatto"))

                If Not codContattoRiga.StartsWith("-") Then
                    partitaIva = codContattoRiga
                End If

                Dim ragSocCompleta = CStr(Trim(dr.Item("Rag_Soc_Contatto"))).Replace("""", "'") & " (" & dr.Item("Rapporto_Des") & ")"

                jArrayListaOp.Add(New JObject(New JProperty("Piva_Proprietaria", dr.Item("Piva_Contatto")),
                                              New JProperty("Sa_Cod", dr.Item("Sa_Cod_Contatto")),
                                              New JProperty("IsImpresaGias", CBool(dr.Item("IsImpresaGias"))),
                                              New JProperty("Rag_Soc", CStr(Trim(dr.Item("Rag_Soc_Contatto"))).Replace("""", "'")),
                                              New JProperty("Rag_Soc_Completa", ragSocCompleta),
                                              New JProperty("Cod_RisUm", dr.Item("Cod_RisUm")),
                                              New JProperty("Cod_Contatto", dr.Item("Cod_Contatto")),
                                              New JProperty("Id_CF", CInt(dr.Item("Id_CF"))),
                                              New JProperty("Partita_Iva", partitaIva),
                                              New JProperty("Codice_Fiscale", dr.Item("Codice_Fiscale")),
                                              New JProperty("Tipo_Indirizzo_Default", dr.Item("Tipo_Indirizzo_Default")),
                                              New JProperty("Cod_Risum_Destinazione_Diversa", dr.Item("Cod_Risum_Destinazione_Diversa")),
                                              New JProperty("Tipo_Indirizzo_Default_Destinazione_Diversa", dr.Item("Tipo_Indirizzo_Default_Destinazione_Diversa")),
                                              New JProperty("Vettore_Cod", dr.Item("Vettore_Cod")),
                                              New JProperty("Agente_Cod", dr.Item("Agente_Cod")),
                                              New JProperty("Provvigione", dr.Item("Provvigione")),
                                              New JProperty("CapoArea_Cod", dr.Item("CapoArea_Cod")),
                                              New JProperty("Provvigione_CapoArea", dr.Item("Provvigione_CapoArea")),
                                              New JProperty("Attivita_Des", dr.Item("Attivita_Des")),
                                              New JProperty("Progressivo", dr.Item("Settore_Des")),
                                              New JProperty("Validita_Inizio", CDate(dr.Item("XValidita_Inizio"))),
                                              New JProperty("Validita_Fine", CDate(dr.Item("XValidita_Fine"))))
                                  )
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    '<WebMethod()>
    '<Script.Services.ScriptMethod()>
    'Public Function Contatti_Contatto(ByVal Piva As String,
    '                                  ByVal Cod_Contatto As String,
    '                                  ByVal Cod_RisUm As Integer,
    '                                  ByVal Cod_Rapporto As Integer,
    '                                  ByVal FlagPubblico As Boolean,
    '                                  ByVal FlagIndirizzi As Boolean,
    '                                  ByVal TipoIndirizzo As Integer,
    '                                  ByVal CodIndirizzo As Integer,
    '                                  ByVal FlagRubrica As Boolean,
    '                                  ByVal CodRubrica As Integer,
    '                                  ByVal ID_CF As Integer,
    '                                  ByVal Cod_RisUm_Origine As Integer,
    '                                  ByVal Piva_SuperUser_Origine As String,
    '                                  ByVal Flag_AncheImportati As Boolean,
    '                                  ByVal Cliente As Integer,
    '                                  ByVal Fornitore As Integer,
    '                                  ByVal Dipendente As Integer,
    '                                  ByVal Terzista As Integer,
    '                                  ByVal Legale As Integer,
    '                                  ByVal objP_server As String) As RispostaStandard
    '    Dim r As New RispostaStandard()

    '    If objP_server = "" Then
    '        r.Errore = "objP_server non valorizzato"
    '        Return r
    '    End If

    '    Try
    '        Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

    '        Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R
    '        Dim dtContatti As DataTable = objContatti.Contatti_Contatto_Leggi(Piva,
    '                                                                          Cod_Contatto,
    '                                                                          Cod_RisUm,
    '                                                                          Cod_Rapporto,
    '                                                                          FlagPubblico,
    '                                                                          FlagIndirizzi,
    '                                                                          TipoIndirizzo,
    '                                                                          CodIndirizzo,
    '                                                                          FlagRubrica,
    '                                                                          CodRubrica,
    '                                                                          ID_CF_NOFILTRO,
    '                                                                          0, "", False, 0, 0, 0, 0, 0,
    '                                                                          enumSelezioneVariabile.Selezione_JoinDescrizioni,
    '                                                                          "", "", objParametriServer)

    '        Dim jArrayListaOp As New JArray()

    '        For Each dr As DataRow In dtContatti.Rows
    '            Dim ragSoc As String = CStr(dr.Item("Rag_Soc")) & CStr(dr.Item("Cognome")) & " " & CStr(dr.Item("Nome")) & " (" & dr.Item("Rapporto_Des") & ")"

    '            jArrayListaOp.Add(New JObject(New JProperty("cod_contatto", dr.Item("cod_contatto")),
    '                                          New JProperty("nome", ragSoc.Replace("""", "'"))))
    '        Next

    '        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
    '        r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaOp, Formatting.None)
    '        r.RispostaOK = True

    '    Catch ex As Exception
    '        r.RispostaOK = False
    '        r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
    '    End Try

    '    Return r

    'End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiRapportoSpecifico_NG(ByVal InData As CoreWS_Generic(Of LeggiRapportoSpecifico)) As RispostaStandard

        Dim r As New RispostaStandard()

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim objContatti As New AgronicaCoreAnagrafeDAL.Risorse_Umane_R

            Dim filtro As String = ""
            If InData.InData.Cod_Contatto <> "" Then
                filtro = " Contatti.Cod_Contatto = '" & Agro_SQL_SaveText(InData.InData.Cod_Contatto) & "' "
            End If

            Dim dtContatti As New DataTable
            dtContatti = objContatti.LeggiRapportoSpecifico(InData.InData.piva,
                                                            filtro,
                                                            " Contatti.Rag_Soc ASC, Contatti.Cognome ASC, Contatti.Nome ASC ",
                                                            objParametriServer,
                                                            Rapporto_Attivo:=InData.InData.rapportoAttivo,
                                                            Cliente:=InData.InData.cliente,
                                                            Fornitore:=InData.InData.fornitore,
                                                            Dipendente:=InData.InData.dipendente,
                                                            Terzista:=InData.InData.terzista,
                                                            Legale:=InData.InData.legale,
                                                            Agente:=InData.InData.agente,
                                                            Consulente:=InData.InData.consulente)

            Dim jArrayListaOp As New JArray()

            For Each dr As DataRow In dtContatti.Rows

                Dim partitaIva As String = ""
                Dim codContattoRiga = Trim(dr.Item("Cod_Contatto"))

                If Not codContattoRiga.StartsWith("-") Then
                    partitaIva = codContattoRiga
                End If

                Dim ragSocCompleta = CStr(Trim(dr.Item("Rag_Soc_Contatto"))).Replace("""", "'") & " (" & dr.Item("Rapporto_Des") & ")"

                jArrayListaOp.Add(New JObject(New JProperty("Piva_Proprietaria", dr.Item("Piva_Contatto")),
                                              New JProperty("Sa_Cod", dr.Item("Sa_Cod_Contatto")),
                                              New JProperty("IsImpresaGias", CBool(dr.Item("IsImpresaGias"))),
                                              New JProperty("Rag_Soc", CStr(Trim(dr.Item("Rag_Soc_Contatto"))).Replace("""", "'")),
                                              New JProperty("Rag_Soc_Completa", ragSocCompleta),
                                              New JProperty("Cod_RisUm", dr.Item("Cod_RisUm")),
                                              New JProperty("Cod_Contatto", dr.Item("Cod_Contatto")),
                                              New JProperty("Id_CF", CInt(dr.Item("Id_CF"))),
                                              New JProperty("Partita_Iva", partitaIva),
                                              New JProperty("Codice_Fiscale", dr.Item("Codice_Fiscale")),
                                              New JProperty("Tipo_Indirizzo_Default", dr.Item("Tipo_Indirizzo_Default")),
                                              New JProperty("Cod_Risum_Destinazione_Diversa", dr.Item("Cod_Risum_Destinazione_Diversa")),
                                              New JProperty("Tipo_Indirizzo_Default_Destinazione_Diversa", dr.Item("Tipo_Indirizzo_Default_Destinazione_Diversa")),
                                              New JProperty("Vettore_Cod", dr.Item("Vettore_Cod")),
                                              New JProperty("Agente_Cod", dr.Item("Agente_Cod")),
                                              New JProperty("Provvigione", dr.Item("Provvigione")),
                                              New JProperty("CapoArea_Cod", dr.Item("CapoArea_Cod")),
                                              New JProperty("Provvigione_CapoArea", dr.Item("Provvigione_CapoArea")),
                                              New JProperty("Attivita_Des", dr.Item("Attivita_Des")),
                                              New JProperty("Progressivo", dr.Item("Settore_Des")),
                                              New JProperty("Validita_Inizio", CDate(dr.Item("XValidita_Inizio"))),
                                              New JProperty("Validita_Fine", CDate(dr.Item("XValidita_Fine"))))
                                  )
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="objP_server"></param>
    ''' <param name="piva"></param>
    ''' <param name="rapportoAttivo"></param>
    ''' <param name="tipoRapporto">0 = Clienti, 1 = Fornitori, 2 = Professionisti, 3 = Dipendenti+Terzisti, 4 = Agenti, 5 = Capo Area, 6 = Conferenti x Accettazione, 7 = Vettori, 8 = Clienti+Fornitori, 9 = Terzisti(no filtro), 10 = Dipendenti</param>
    ''' <returns></returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiRapportoDocumenti_NG(ByVal InData As CoreWS_Generic(Of LeggiRapportoDocumenti)) As RispostaStandard

        Dim r As New RispostaStandard()

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If


        Try

            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim xFIltroAggiuntivo As String = String.Empty

            'TODO: lo lascio qui o lo sposo fuori?!?
            Dim rapportiSpecificiStrIN As String = ""
            If InData.InData.tipoRapporto = 6 Then
                rapportiSpecificiStrIN = "-11," &
                                         enum_Rapporti_Contabili_Standard.Conferente & "," &
                                         enum_Rapporti_Contabili_Standard.Fornitore_Ortofrutta
            End If

            If InData.InData.tipoRapporto = COD_ALLEVATORE Or InData.InData.tipoRapporto = COD_MACELLO Or InData.InData.tipoRapporto = COD_VETERINARIO Then
                rapportiSpecificiStrIN = InData.InData.tipoRapporto.ToString
                InData.InData.tipoRapporto = 0
            End If

            If InData.InData.codRisUm <> 0 OrElse InData.InData.codContatto <> "" Then

                If InData.InData.codContatto <> "" Then
                    xFIltroAggiuntivo &= " ( Contatti.Cod_Contatto = '" & Agro_SQL_SaveText(InData.InData.codContatto) & "' ) "
                End If

                If InData.InData.codRisUm <> 0 Then
                    If xFIltroAggiuntivo.Length > 0 Then
                        xFIltroAggiuntivo &= " AND "
                    End If
                    xFIltroAggiuntivo &= " ( Risorse_Umane.Cod_RisUm = " & Agro_SQL_SaveNum(InData.InData.codRisUm) & " ) "
                End If

                If xFIltroAggiuntivo.Length > 0 Then
                    xFIltroAggiuntivo = " ( " & xFIltroAggiuntivo & " ) "
                End If
            End If

            Dim obj As New AgronicaCoreAnagrafeDAL.Risorse_Umane_R
            Dim dtContatti As DataTable = obj.LeggiRapportoSpecificoxDocumenti(InData.InData.piva,
                                                                               InData.InData.rapportoAttivo,
                                                                               InData.InData.tipoRapporto,
                                                                               InData.InData.cercaSoloValidi,
                                                                               rapportiSpecificiStrIN,
                                                                               xFIltroAggiuntivo,
                                                                               " Contatti.Rag_Soc ASC, Contatti.Cognome ASC, Contatti.Nome ASC ",
                                                                               objParametriServer,
                                                                               InData.InData.accettazioneConGerarchia,
                                                                               InData.InData.pivaPadreGerarchia,
                                                                               dataValidita:=CDate(If(InData.InData.dataValidita, #2/1/1900#)),
                                                                               TestoRicerca:=InData.InData.testoRicerca,
                                                                               includiIndirizzo:=InData.InData.includiIndirizzo,
                                                                               checkRaccolteNonCollegateConferimento:=InData.InData.checkRaccolte,
                                                                               dataFineRaccolte:=InData.InData.dataFineRaccolte)

            Dim righe() As DataRow
            If InData.InData.filtraSoloValidi = True Then
                righe = dtContatti.Select(" Valido = 1 ")
            Else
                righe = dtContatti.Select(" 1 = 1 ")
            End If

            Dim jArrayListaOp As New JArray()

            For Each dr As DataRow In righe

                Dim partitaIva As String = ""
                Dim codContattoRiga = Trim(dr.Item("Cod_Contatto"))

                If Not codContattoRiga.StartsWith("-") Then
                    partitaIva = codContattoRiga
                End If


                Dim ragSocCompleta = CStr(Trim(dr.Item("Rag_Soc_Contatto"))).Replace("""", "'") & " (" & dr.Item("Rapporto_Des")

                If InData.InData.includiIndirizzo = True Then
                    Select Case dr.Item("Stato")
                        Case "ITALIA", "IT", "ITA", "ITALY", ""
                            ragSocCompleta &= If(dr.Item("Pro_Cod_ISTAT") = "", "", " " & dr.Item("Provincia_Des"))
                        Case Else
                            ragSocCompleta &= " " & dr.Item("Stato_Des")
                    End Select
                End If
                ragSocCompleta &= ")"

                Dim ragSocCompletaSettoreDes = ragSocCompleta & " - " & dr.Item("Settore_Des")

                jArrayListaOp.Add(New JObject(New JProperty("Piva_Proprietaria", dr.Item("Piva_Contatto")),
                                              New JProperty("Sa_Cod", dr.Item("Sa_Cod_Contatto")),
                                              New JProperty("IsImpresaGias", CBool(dr.Item("IsImpresaGias"))),
                                              New JProperty("Rag_Soc", CStr(Trim(dr.Item("Rag_Soc_Contatto"))).Replace("""", "'")),
                                              New JProperty("Rapporto_Des", dr.Item("Rapporto_Des")),
                                              New JProperty("Rag_Soc_Completa", ragSocCompleta),
                                              New JProperty("Rag_Soc_Progressivo", ragSocCompletaSettoreDes),
                                              New JProperty("Cod_RisUm", CInt(dr.Item("Cod_RisUm"))),
                                              New JProperty("Cod_Contatto", dr.Item("Cod_Contatto")),
                                              New JProperty("Id_CF", CInt(dr.Item("Id_CF"))),
                                              New JProperty("Partita_Iva", partitaIva),
                                              New JProperty("Codice_Fiscale", dr.Item("Codice_Fiscale")),
                                              New JProperty("Tipo_Indirizzo_Default", CInt(dr.Item("Tipo_Indirizzo_Default"))),
                                              New JProperty("Cod_Risum_Destinazione_Diversa", CInt(dr.Item("Cod_Risum_Destinazione_Diversa"))),
                                              New JProperty("Tipo_Indirizzo_Default_Destinazione_Diversa", CInt(dr.Item("Tipo_Indirizzo_Default_Destinazione_Diversa"))),
                                              New JProperty("Vettore_Cod", CInt(dr.Item("Vettore_Cod"))),
                                              New JProperty("Agente_Cod", CInt(dr.Item("Agente_Cod"))),
                                              New JProperty("Provvigione", CDec(dr.Item("Provvigione"))),
                                              New JProperty("CapoArea_Cod", CInt(dr.Item("CapoArea_Cod"))),
                                              New JProperty("Provvigione_CapoArea", CDec(dr.Item("Provvigione_CapoArea"))),
                                              New JProperty("Attivita_Des", dr.Item("Attivita_Des")),
                                              New JProperty("Progressivo", dr.Item("Settore_Des")),
                                              New JProperty("Validita_Inizio", CDate(dr.Item("XValidita_Inizio"))),
                                              New JProperty("Validita_Fine", CDate(dr.Item("XValidita_Fine"))),
                                              New JProperty("LavCod_Raccolta", CInt(dr.Item("LavCod_Raccolta")))
                                              )
                                  )
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiRapportoDocumenti(ByVal objP_server As String,
                                           ByVal piva As String,
                                           ByVal rapportoAttivo As Boolean,
                                           ByVal dataValidita As Date?,
                                           ByVal tipoRapporto As Integer,
                                           ByVal filtraSoloValidi As Boolean,
                                           ByVal cercaSoloValidi As Boolean,
                                           ByVal includiIndirizzo As Boolean,
                                           ByVal accettazioneConGerarchia As Integer,
                                           ByVal pivaPadreGerarchia As String,
                                           ByVal testoRicerca As String,
                                           ByVal codRisUm As Integer,
                                           ByVal codContatto As String,
                                           ByVal checkRaccolte As Boolean,
                                           ByVal dataFineRaccolte As Date?
                                           ) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try

            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim xFIltroAggiuntivo As String = String.Empty

            'TODO: lo lascio qui o lo sposto fuori?!?
            Dim rapportiSpecificiStrIN As String = ""
            If tipoRapporto = 6 Then
                rapportiSpecificiStrIN = "-11," &
                                         enum_Rapporti_Contabili_Standard.Conferente & "," &
                                         enum_Rapporti_Contabili_Standard.Fornitore_Ortofrutta
            End If

            If tipoRapporto = COD_ALLEVATORE Or tipoRapporto = COD_MACELLO Or tipoRapporto = COD_VETERINARIO Then
                rapportiSpecificiStrIN = tipoRapporto.ToString
                tipoRapporto = 0
            End If

            If codRisUm <> 0 OrElse codContatto <> "" Then

                If codContatto <> "" Then
                    xFIltroAggiuntivo &= " ( Contatti.Cod_Contatto = '" & Agro_SQL_SaveText(codContatto) & "' ) "
                End If

                If codRisUm <> 0 Then
                    If xFIltroAggiuntivo.Length > 0 Then
                        xFIltroAggiuntivo &= " AND "
                    End If
                    xFIltroAggiuntivo &= " ( Risorse_Umane.Cod_RisUm = " & Agro_SQL_SaveNum(codRisUm) & " ) "
                End If

                If xFIltroAggiuntivo.Length > 0 Then
                    xFIltroAggiuntivo = " ( " & xFIltroAggiuntivo & " ) "
                End If
            End If

            Dim obj As New AgronicaCoreAnagrafeDAL.Risorse_Umane_R
            Dim dtContatti As DataTable = obj.LeggiRapportoSpecificoxDocumenti(piva,
                                                                               rapportoAttivo,
                                                                               tipoRapporto,
                                                                               cercaSoloValidi,
                                                                               rapportiSpecificiStrIN,
                                                                               xFIltroAggiuntivo,
                                                                               " Contatti.Rag_Soc ASC, Contatti.Cognome ASC, Contatti.Nome ASC ",
                                                                               objParametriServer,
                                                                               accettazioneConGerarchia,
                                                                               pivaPadreGerarchia,
                                                                               dataValidita:=CDate(If(dataValidita, #2/1/1900#)),
                                                                               TestoRicerca:=testoRicerca,
                                                                               includiIndirizzo:=includiIndirizzo,
                                                                               checkRaccolteNonCollegateConferimento:=checkRaccolte,
                                                                               dataFineRaccolte:=dataFineRaccolte)

            Dim righe() As DataRow
            If filtraSoloValidi Then
                righe = dtContatti.Select(" Valido = 1 ")
            Else
                righe = dtContatti.Select(" 1 = 1 ")
            End If

            Dim jArrayListaOp As New JArray()

            For Each dr As DataRow In righe

                Dim partitaIva As String = ""
                Dim codContattoRiga = Trim(dr.Item("Cod_Contatto"))

                If Not codContattoRiga.StartsWith("-") Then
                    partitaIva = codContattoRiga
                End If


                Dim ragSocCompleta = CStr(Trim(dr.Item("Rag_Soc_Contatto"))).Replace("""", "'") & " (" & dr.Item("Rapporto_Des")

                If includiIndirizzo Then
                    Select Case dr.Item("Stato")
                        Case "ITALIA", "IT", "ITA", "ITALY", ""
                            ragSocCompleta &= If(dr.Item("Pro_Cod_ISTAT") = "", "", " " & dr.Item("Provincia_Des"))
                        Case Else
                            ragSocCompleta &= " " & dr.Item("Stato_Des")
                    End Select
                End If
                ragSocCompleta &= ")"

                Dim ragSocCompletaSettoreDes = ragSocCompleta & " - " & dr.Item("Settore_Des")

                jArrayListaOp.Add(New JObject(New JProperty("Piva_Proprietaria", dr.Item("Piva_Contatto")),
                                              New JProperty("Sa_Cod", dr.Item("Sa_Cod_Contatto")),
                                              New JProperty("IsImpresaGias", CBool(dr.Item("IsImpresaGias"))),
                                              New JProperty("Compliance_ISCC", dr.Item("Compliance_ISCC")),
                                              New JProperty("Rag_Soc", CStr(Trim(dr.Item("Rag_Soc_Contatto"))).Replace("""", "'")),
                                              New JProperty("Rapporto_Des", dr.Item("Rapporto_Des")),
                                              New JProperty("Rag_Soc_Completa", ragSocCompleta),
                                              New JProperty("Rag_Soc_Progressivo", ragSocCompletaSettoreDes),
                                              New JProperty("Cod_RisUm", CInt(dr.Item("Cod_RisUm"))),
                                              New JProperty("Cod_Contatto", dr.Item("Cod_Contatto")),
                                              New JProperty("Id_CF", CInt(dr.Item("Id_CF"))),
                                              New JProperty("Partita_Iva", partitaIva),
                                              New JProperty("Codice_Fiscale", dr.Item("Codice_Fiscale")),
                                              New JProperty("Tipo_Indirizzo_Default", CInt(dr.Item("Tipo_Indirizzo_Default"))),
                                              New JProperty("Cod_Risum_Destinazione_Diversa", CInt(dr.Item("Cod_Risum_Destinazione_Diversa"))),
                                              New JProperty("Tipo_Indirizzo_Default_Destinazione_Diversa", CInt(dr.Item("Tipo_Indirizzo_Default_Destinazione_Diversa"))),
                                              New JProperty("Vettore_Cod", CInt(dr.Item("Vettore_Cod"))),
                                              New JProperty("Agente_Cod", CInt(dr.Item("Agente_Cod"))),
                                              New JProperty("Provvigione", CDec(dr.Item("Provvigione"))),
                                              New JProperty("CapoArea_Cod", CInt(dr.Item("CapoArea_Cod"))),
                                              New JProperty("Provvigione_CapoArea", CDec(dr.Item("Provvigione_CapoArea"))),
                                              New JProperty("Attivita_Des", dr.Item("Attivita_Des")),
                                              New JProperty("Progressivo", dr.Item("Settore_Des")),
                                              New JProperty("Validita_Inizio", CDate(dr.Item("XValidita_Inizio"))),
                                              New JProperty("Validita_Fine", CDate(dr.Item("XValidita_Fine"))),
                                              New JProperty("LavCod_Raccolta", CInt(dr.Item("LavCod_Raccolta"))),
                                              New JProperty("Modalita_Pagamento", CInt(dr.Item("Modalita_Pagamento")))
                                              )
                                  )
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Contatti_APP(ByVal piva As String, ByVal objP_super_server As String, ByVal objP_server As String, ByVal objP_utenti As String) As RispostaStandard

        Dim r As New RispostaStandard()

        Try

            Dim objParametri_SuperServer As AgronicaCoreDataProvider.AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim filtriList As New List(Of String)
            ' vecchio filtro per app: filtriList.Add("Rapporti_Contabili.Cod_Rapporto in (-4,-21)")
            filtriList.Add("((Rapporti_Contabili.Cod_Rapporto in (-1,-4,-6,-5)) or Rapporti_Contabili.Dipendente=1 or Rapporti_Contabili.Legale=1 or Rapporti_Contabili.Terzista=1)")

            Dim pivaEffettiva As String
            Dim caricaPubbliche As Boolean
            If Not String.IsNullOrEmpty(piva) Then
                ' Solo persone di un'azienda specifica
                pivaEffettiva = piva
                caricaPubbliche = False
                filtriList.Add("(Contatti.Sa_Cod != -1)")
            Else
                ' Tutte le macchine comuni tranne quelle di un'azienda specifica
                pivaEffettiva = "99999999999"
                caricaPubbliche = True
            End If

            Dim xFiltroAggiuntivo As String = String.Join(" AND ", filtriList)

            Dim Data_Validita = Date.Now.Date
            objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(Data_Validita, Data_Validita)

            Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R
            Dim dtContatti As DataTable = objContatti.Contatti_Contatto_Leggi(pivaEffettiva,
                "", 0, 0, caricaPubbliche, False, 0, 0, False, 0,
                ID_CF_NOFILTRO,
                0, "", False, 0, 0, 0, 0, 0,
                enumSelezioneVariabile.Selezione_JoinDescrizioni,
                xFiltroAggiuntivo, "", objParametri_Server,
                leggi_NrBadge:=True, leggi_Patentino:=True)

            objParametri_Server.ResettaFinestra()

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(dtContatti, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Fornitori_APP(ByVal piva As String, ByVal objP_super_server As String, ByVal objP_server As String, ByVal objP_utenti As String) As RispostaStandard


        Dim r As New RispostaStandard()

        Try

            Dim objParametri_SuperServer As AgronicaCoreDataProvider.AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim xFiltroAggiuntivo = " Rapporti_Contabili.Cod_Rapporto in ("
            xFiltroAggiuntivo += CStr(enum_Rapporti_Contabili_Standard.Vivaio)
            xFiltroAggiuntivo += ", "
            xFiltroAggiuntivo += CStr(enum_Rapporti_Contabili_Standard.Fornitore_Agrofarmaci)
            xFiltroAggiuntivo += ")"

            Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R
            Dim dtContatti As DataTable = Nothing

            Dim Data_Validita = Date.Now.Date
            objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(Data_Validita, Data_Validita)

            If Not String.IsNullOrEmpty(piva) Then
                ' Solo persone di un'azienda specifica
                xFiltroAggiuntivo += " And Contatti.Sa_Cod != -1 "
                dtContatti = objContatti.Contatti_Contatto_Leggi(piva,
                    "", 0, 0, False, False, 0, 0, False, 0,
                    ID_CF_NOFILTRO,
                    0, "", False, 0, 0, 0, 0, 0,
                    enumSelezioneVariabile.Selezione_JoinDescrizioni,
                    xFiltroAggiuntivo, "", objParametri_Server, leggi_NrBadge:=True)
            Else
                dtContatti = objContatti.Contatti_Contatto_Leggi("99999999999",
                    "", 0, 0, True, False, 0, 0, False, 0,
                    ID_CF_NOFILTRO,
                    0, "", False, 0, 0, 0, 0, 0,
                    enumSelezioneVariabile.Selezione_JoinDescrizioni,
                    xFiltroAggiuntivo, "", objParametri_Server, leggi_NrBadge:=True)
            End If

            objParametri_Server.ResettaFinestra()

            Dim jArrayListaOp As New JArray()

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(dtContatti, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboCmb_Riferimento_Trasferimento_Dati(
        ByVal objP_super_server As String, ByVal objP_server As String, ByVal objP_utenti As String,
        ByVal piva As String
    ) As RispostaStandard

        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

        Try

            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod,
                                                                      AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                      "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)


            r.RispostaOK = True
            Dim objOR As New AgronicaCoreAnagrafeDAL.RiferimentoTrasferimentoDati_R
            Dim Dt_Padri = objOR.Leggi(True, piva, "", "", "", objParametri_Server)

            Dim jArrayListaOp As New JArray()

            For Each dr As DataRow In Dt_Padri.Rows

                jArrayListaOp.Add(New JObject(New JProperty("text", dr.Item("piva_padre") & "-" & dr.Item("ragsoc_padre")),
                                              New JProperty("value", dr.Item("piva_padre"))))


            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaOp, Formatting.None)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function
    '<WebMethod()>
    '<Script.Services.ScriptMethod()>
    'Public Function CaricaComboCmb_Riferimento_Trasferimento_Dati(InData As Object) As RispostaStandard

    '    Dim r As New RispostaStandard

    '    InData = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData))

    '    Dim piva As String = InData.InData

    '    If InData.objP.objP_server = "" Then
    '        r.Errore = "objP_server non valorizzato"
    '        Return r
    '    End If
    '    If InData.objP.objP_utenti = "" Then
    '        r.Errore = "objP_server non valorizzato"
    '        Return r
    '    End If
    '    If InData.objP.objP_super_server = "" Then
    '        r.Errore = "objP_server non valorizzato"
    '        Return r
    '    End If

    '    Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
    '    Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

    '    Try

    '        Dim leggiLingua As New Lingue_Read
    '        Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod,
    '                                                                  AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
    '                                                                  "", "", objParametri_Utenti)
    '        Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
    '        Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)


    '        r.RispostaOK = True
    '        Dim objOR As New AgronicaCoreAnagrafeDAL.RiferimentoTrasferimentoDati_R
    '        Dim Dt_Padri = objOR.Leggi(True, piva, "", "", "", objParametri_Server)

    '        Dim jArrayListaOp As New JArray()

    '        For Each dr As DataRow In Dt_Padri.Rows

    '            jArrayListaOp.Add(New JObject(New JProperty("text", dr.Item("piva_padre") & "-" & dr.Item("ragsoc_padre")),
    '                                          New JProperty("value", dr.Item("piva_padre"))))


    '        Next

    '        r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaOp, Formatting.None)

    '    Catch ex As Exception

    '        r.RispostaOK = False
    '        r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

    '    End Try

    '    Return r
    'End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboCmb_Riferimento_Trasferimento_Dati_Modello(
        InData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Anagrafica.FiltroImpresa)
    ) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.anagrafiche.Contatto))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.anagrafiche.Contatto))

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

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

        Try

            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod,
                                                                      AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                      "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)


            r.RispostaOK = True
            Dim objOR As New AgronicaCoreAnagrafeDAL.RiferimentoTrasferimentoDati_R
            Dim Dt_Padri = objOR.Leggi(True, InData.InData.impresa.partitaIva, "", "", "", objParametri_Server)

            Dim listItems = (From dr In Dt_Padri.Rows
                             Select New AgronicaCoreModelsSTD.anagrafiche.Contatto() With {
                                     .ragione_Sociale = dr.Item("ragsoc_padre"),
                                     .primaryKey = New AgronicaCoreModelsSTD.anagrafiche.Contatto.PK(InData.InData.impresa.partitaIva, dr("piva_padre"))
                                  }).ToList

            r.RispostaStringa = listItems

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboCmb_OrganismoReferente(ByVal objP_server As String,
                                           ByVal piva As String
                                           ) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim objOR As New AgronicaCoreAnagrafeDAL.OrganismoReferente_Read
            Dim Dt_Padri = objOR.Leggi(True, piva, "", "", "", objParametriServer)

            Dim jArrayListaOp As New JArray()

            For Each dr As DataRow In Dt_Padri.Rows

                jArrayListaOp.Add(New JObject(New JProperty("text", dr.Item("piva_padre") & "-" & dr.Item("ragsoc_padre")),
                                              New JProperty("value", dr.Item("piva_padre"))))


            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboCmb_OrganismoReferente_NG(InData As Object) As RispostaStandard

        Dim r As New RispostaStandard()

        InData = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData))

        Dim piva As String = InData.InData

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim objOR As New AgronicaCoreAnagrafeDAL.OrganismoReferente_Read
            Dim Dt_Padri = objOR.Leggi(True, piva, "", "", "", objParametriServer)

            Dim jArrayListaOp As New JArray()

            For Each dr As DataRow In Dt_Padri.Rows

                jArrayListaOp.Add(New JObject(New JProperty("text", dr.Item("piva_padre") & "-" & dr.Item("ragsoc_padre")),
                                              New JProperty("value", dr.Item("piva_padre"))))


            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboCmb_OrganismoReferente_Modello(InData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Anagrafica.FiltroImpresa)
                                           ) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.anagrafiche.Contatto))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.anagrafiche.Contatto))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim objOR As New AgronicaCoreAnagrafeDAL.OrganismoReferente_Read
            Dim Dt_Padri = objOR.Leggi(True, InData.InData.impresa.partitaIva, "", "", "", objParametri_Server)

            Dim tipo As String = "contatto"
            Dim listItems = (From dr In Dt_Padri.Rows
                             Select New AgronicaCoreModelsSTD.anagrafiche.Contatto() With {
                                 .ragione_Sociale = dr.Item("ragsoc_padre"),
                                 .primaryKey = New AgronicaCoreModelsSTD.anagrafiche.Contatto.PK(InData.InData.impresa.partitaIva, dr("piva_padre")),
                                 .tipo = tipo
                                 }).ToList

            'Aggiungo anche tutti i padri in gerarchia
            listItems.AddRange(aggiungiPadriGerarchia(InData.InData.impresa.partitaIva, objParametri_Server))

            listItems = listItems.GroupBy(Function(x) x.primaryKey.codice).Select(Function(x) x.First).ToList()

            r.RispostaStringa = listItems
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    Private Function aggiungiPadriGerarchia(PIVA As String,
                                          objParametri_Server As AgronicaCoreParametri
                                          ) As List(Of AgronicaCoreModelsSTD.anagrafiche.Contatto)

        Dim objGerarchia As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R
        Dim DT_Gerarchia = objGerarchia.LeggiPadriGerarchia("", PIVA, 0, 0, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server)

        Dim listItems = (From dr In DT_Gerarchia.Rows
                         Select New AgronicaCoreModelsSTD.anagrafiche.Contatto() With {
                             .ragione_Sociale = dr.Item("RagSoc_Padre"),
                             .primaryKey = New AgronicaCoreModelsSTD.anagrafiche.Contatto.PK(PIVA, dr("padre")),
                             .tipo = "azienda"
                             }).ToList
        Return listItems

    End Function



    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboCmb_MagazzinoConferimento(ByVal objP_server As String,
                                           ByVal piva As String
                                           ) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim objOR As New AgronicaCoreAnagrafeDAL.OrganismoReferente_Read
            Dim FiltroAggiuntivoMag As String = " (Fabbricati.Tipo_Fabbricato_Cod = 20 OR Fabbricati.Tipo_Fabbricato_Cod = 50 OR Fabbricati.Tipo_Fabbricato_Cod = 120 OR Fabbricati.Tipo_Fabbricato_Cod = 121 OR Fabbricati.Tipo_Fabbricato_Cod = 122 OR Fabbricati.Tipo_Fabbricato_Cod = 123) "

            Dim objFabbricati As New AgronicaCoreAnagrafeDAL.Fabbricati_R

            Dim Dt_Mag = objFabbricati.Leggi_Magazzini_Organismoreferente(piva,
                                                                    0,
                                                                    0,
                                                                    0,
                                                                    FiltroAggiuntivoMag,
                                                                    "",
                                                                    objParametriServer)
            Dim jArrayListaOp As New JArray()

            For Each dr As DataRow In Dt_Mag.Rows

                jArrayListaOp.Add(New JObject(New JProperty("text", CStr(dr("Fabbricato_Des")) & " (" & CStr(dr("rag_soc")) & ")"),
                                              New JProperty("value", CStr(dr("Fabbricato_Cod")) & "|" & CStr(dr("Sa_Cod") & "|" & CStr(dr("Piva"))))))


            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboCmb_MagazzinoConferimento_NG(InData As Object) As RispostaStandard

        Dim r As New RispostaStandard()

        InData = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData))

        Dim piva As String = InData.InData

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim objOR As New AgronicaCoreAnagrafeDAL.OrganismoReferente_Read
            Dim FiltroAggiuntivoMag As String = " (Fabbricati.Tipo_Fabbricato_Cod = 20 OR Fabbricati.Tipo_Fabbricato_Cod = 50 OR Fabbricati.Tipo_Fabbricato_Cod = 120 OR Fabbricati.Tipo_Fabbricato_Cod = 121 OR Fabbricati.Tipo_Fabbricato_Cod = 122 OR Fabbricati.Tipo_Fabbricato_Cod = 123) "

            Dim objFabbricati As New AgronicaCoreAnagrafeDAL.Fabbricati_R

            Dim Dt_Mag = objFabbricati.Leggi_Magazzini_Organismoreferente(piva,
                                                                    0,
                                                                    0,
                                                                    0,
                                                                    FiltroAggiuntivoMag,
                                                                    "",
                                                                    objParametriServer)
            Dim jArrayListaOp As New JArray()

            For Each dr As DataRow In Dt_Mag.Rows

                jArrayListaOp.Add(New JObject(New JProperty("text", CStr(dr("Fabbricato_Des")) & " (" & CStr(dr("rag_soc")) & ")"),
                                              New JProperty("value", CStr(dr("Fabbricato_Cod")) & "|" & CStr(dr("Sa_Cod") & "|" & CStr(dr("Piva"))))))


            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboCmb_MagazzinoConferimento_Modello(InData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Anagrafica.FiltroImpresa)
                                           ) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.anagrafiche.Fabbricato))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.anagrafiche.Fabbricato))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim objOR As New AgronicaCoreAnagrafeDAL.OrganismoReferente_Read
            Dim FiltroAggiuntivoMag As String = " (Fabbricati.Tipo_Fabbricato_Cod = 20 OR Fabbricati.Tipo_Fabbricato_Cod = 50 OR Fabbricati.Tipo_Fabbricato_Cod = 120 OR Fabbricati.Tipo_Fabbricato_Cod = 121 OR Fabbricati.Tipo_Fabbricato_Cod = 122 OR Fabbricati.Tipo_Fabbricato_Cod = 123) "

            Dim objFabbricati As New AgronicaCoreAnagrafeDAL.Fabbricati_R

            Dim Dt_Mag = objFabbricati.Leggi_Magazzini_Organismoreferente(InData.InData.impresa.partitaIva,
                                                                    0,
                                                                    0,
                                                                    0,
                                                                    FiltroAggiuntivoMag,
                                                                    "",
                                                                    objParametriServer)

            'Dim jArrayListaOp As New JArray()
            'For Each dr As DataRow In Dt_Mag.Rows
            '    jArrayListaOp.Add(New JObject(New JProperty("text", CStr(dr("Fabbricato_Des")) & " (" & CStr(dr("rag_soc")) & ")"),
            '                                  New JProperty("value", CStr(dr("Fabbricato_Cod")) & "|" & CStr(dr("Sa_Cod") & "|" & CStr(dr("Piva"))))))
            'Next

            Dim listItems = (From dr In Dt_Mag.Rows
                             Select New AgronicaCoreModelsSTD.anagrafiche.Fabbricato() With {
                                .primaryKey = New AgronicaCoreModelsSTD.anagrafiche.Fabbricato.PK() With {
                                    .codice = dr("Fabbricato_Cod"),
                                    .centroAziendalePK = New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK(dr("sa_cod"), dr("Piva"))
                                },
                                .descrizione = CStr(dr("Fabbricato_Des")) & " (" & CStr(dr("rag_soc")) & ")"
                             }).ToList

            r.RispostaStringa = listItems
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod()>
    Public Function LeggiFornitori_FF(ByVal objP_server As String, ByVal objP_utenti As String, ByVal piva As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_utenti)

        Try
            Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R
            r.RispostaStringa = objContatti.Leggi_Fornitori_FF(piva, objParametri_Server)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                        AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function
    <WebMethod()>
    Public Function LeggiFornitori_FF_NG(InData As Object) As RispostaStandard

        Dim r As New RispostaStandard
        InData = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData))

        Dim piva As String = InData.InData

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(InData.objP.objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

        Try
            Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R
            r.RispostaStringa = objContatti.Leggi_Fornitori_FF(piva, objParametri_Server)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                        AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboCmb_Tecnici(ByVal piva As String, ByVal objP_server As String) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim jArrayListaOp As New JArray()

            Dim objContattiR As New AgronicaCoreAnagrafeDAL.Contatti_R
            Dim dt = objContattiR.Contatti_Contatto_Leggi(CStr(piva),
                                                  "",
                                                  0,
                                                  enum_Rapporti_Contabili_Standard.Tecnico,
                                                  True,
                                                  False,
                                                  0,
                                                  0,
                                                  False,
                                                  0,
                                                  0,
                                                  0,
                                                  "", True, 0, 0, 0, 0, 0,
                                                  enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                  "", "", objParametriServer)

            For Each dr As DataRow In dt.Rows
                Dim Rag_Soc = CStr(dr.Item("Rag_Soc")) +
                         CStr(dr.Item("Cognome")) + " " +
                             CStr(dr.Item("Nome"))

                jArrayListaOp.Add(New JObject(New JProperty("text", Rag_Soc),
                                              New JProperty("value", dr.Item("Cod_Contatto"))))


            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboCmb_Tecnici_NG(InData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.LeggiFiltro)) As RispostaStandard

        Dim r As New RispostaStandard()

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim jArrayListaOp As New JArray()

            Dim objContattiR As New AgronicaCoreAnagrafeDAL.Contatti_R
            Dim dt = objContattiR.Contatti_Contatto_Leggi(CStr(InData.InData.Ricerca),
                                                  "",
                                                  0,
                                                  -6,
                                                  True,
                                                  False,
                                                  0,
                                                  0,
                                                  False,
                                                  0,
                                                  0,
                                                  0,
                                                  "", True, 0, 0, 0, 0, 0,
                                                  enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                  "", "", objParametriServer)

            For Each dr As DataRow In dt.Rows
                Dim Rag_Soc = CStr(dr.Item("Rag_Soc")) +
                         CStr(dr.Item("Cognome")) + " " +
                             CStr(dr.Item("Nome"))

                jArrayListaOp.Add(New JObject(New JProperty("text", Rag_Soc),
                                              New JProperty("value", dr.Item("Cod_Contatto"))))


            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboCmb_Tecnici_Modello(ByVal piva As String, ByVal objP_server As String) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.anagrafiche.Contatto))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.anagrafiche.Contatto))

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim objRisorseUmane As New AgronicaCoreAnagrafeBIZ.RisorseUmane_R

            r.RispostaStringa = objRisorseUmane.Leggi_Tecnici(piva, "", objParametriServer)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboCmb_Tecnici_Modello_NG(InData As CoreWS_Generic(Of String)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.anagrafiche.Contatto))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.anagrafiche.Contatto))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim objRisorseUmane As New AgronicaCoreAnagrafeBIZ.RisorseUmane_R

            r.RispostaStringa = objRisorseUmane.Leggi_Tecnici(InData.InData, "", objParametriServer)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboCmb_OrganismiODC(ByVal piva As String, ByVal objP_server As String) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim jArrayListaOp As New JArray()

            Dim Organismi_R As New AgronicaCoreAnagrafeDAL.Contatti_R
            Dim dt = Organismi_R.Leggi_Contatti_ByCod_Rapporto(True, "", "", enum_Rapporti_Contabili_Standard.Organismo_Di_Controllo, "", "", objParametriServer)

            For Each dr As DataRow In dt.Rows
                Dim Rag_Soc = CStr(dr.Item("Rag_Soc"))

                jArrayListaOp.Add(New JObject(New JProperty("text", Rag_Soc),
                                              New JProperty("value", dr.Item("Cod_Risum"))))


            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboCmb_OrganismiODC_NG(InData As CoreWS_Generic(Of String)) As RispostaStandard

        Dim r As New RispostaStandard()

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim jArrayListaOp As New JArray()

            Dim Organismi_R As New AgronicaCoreAnagrafeDAL.Contatti_R
            Dim dt = Organismi_R.Leggi_Contatti_ByCod_Rapporto(True, "", "", enum_Rapporti_Contabili_Standard.Organismo_Di_Controllo, "", "", objParametriServer)

            For Each dr As DataRow In dt.Rows
                Dim Rag_Soc = CStr(dr.Item("Rag_Soc"))

                jArrayListaOp.Add(New JObject(New JProperty("text", Rag_Soc),
                                              New JProperty("value", dr.Item("Cod_Risum"))))


            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Leggi_Contatti_Anagrafica(ByVal objP_super_server As String,
                                                  ByVal objP_server As String,
                                                  ByVal objP_utenti As String,
                                                  ByVal InData As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim objParametriAgenda = InData

            '''QUA NEI CORE WS NON FUNZIONERA
            Dim modalita As String = HttpContext.Current.Session("modalita")

            If objParametriAgenda.Data <> AGRODATAINIZIO Then
                objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(objParametriAgenda.Data.Date, objParametriAgenda.Data.Date)
            End If

            Dim dt As DataTable = objContatti.leggi_x_anagrafica2(objParametriAgenda.Piva, "0", "", "", objParametri_Server)
            Dim dt1 As DataTable = objContatti.leggi_x_anagrafica2(objParametriAgenda.Piva, "-1", "", "", objParametri_Server)

            If objParametriAgenda.Data <> AGRODATAINIZIO Then
                objParametri_Server.ResettaFinestra()
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

            l.Add(New ColonneNome("Rapporto_Des", "Rapporto", "string"))
            l.Add(New ColonneNome("Tipo_Contatto", "Tipo", "string"))
            l.Add(New ColonneNome("Impresa", "Impresa Referente", "string"))
            l.Add(New ColonneNome("Settore_Des", "Codice Contatto", "string"))

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
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function
    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Leggi_Contatti_Anagrafica_NG(ByVal InData As CoreWS_Generic(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG)) As RispostaStandard
        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim objParametriAgenda = InData.InData

            '''QUA NEI CORE WS NON FUNZIONERA
            'Dim modalita As String = HttpContext.Current.Session("modalita")

            If objParametriAgenda.Data <> AGRODATAINIZIO Then
                objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(objParametriAgenda.Data.Date, objParametriAgenda.Data.Date)
            End If

            Dim dt As DataTable = objContatti.leggi_x_anagrafica2(objParametriAgenda.Piva, "0", "", "", objParametri_Server)
            Dim dt1 As DataTable = objContatti.leggi_x_anagrafica2(objParametriAgenda.Piva, "-1", "", "", objParametri_Server)

            If objParametriAgenda.Data <> AGRODATAINIZIO Then
                objParametri_Server.ResettaFinestra()
            End If

            dt.Merge(dt1)
            Dim col As New DataColumn("Tipo_Contatto", GetType(String))
            col.Expression = "IIF(sa_cod = -1, 'Pubblico', 'Privato')"
            dt.Columns.Add(col)
            dt.Columns.Add(New DataColumn("Impresa_Referente", GetType(String)))


            'creo la lista delle colonne da visualizzare
            Dim l As New List(Of ColonneNome)

            l.Add(New ColonneNome("chiave", "chiave", "string") With {._hidden = True})
            l.Add(New ColonneNome("CF", "CF", "string"))
            l.Add(New ColonneNome("sa_cod", "sa_cod", "string"))
            l.Add(New ColonneNome("Contatto_Des", "Nome del Contatto", "string"))

            l.Add(New ColonneNome("Rapporto_Des", "Rapporto", "string"))
            l.Add(New ColonneNome("Cod_Rapp", "Cod_Rapp", "string") With {._hidden = True})
            l.Add(New ColonneNome("Cod_Risum", "Cod_Risum", "string") With {._hidden = True})

            l.Add(New ColonneNome("Tipo_Contatto", "Tipo", "string"))
            l.Add(New ColonneNome("Impresa", "Impresa Referente", "string"))
            l.Add(New ColonneNome("Settore_Des", "Codice Contatto", "string"))
            l.Add(New ColonneNome("Attivita_Des", "Codice", "string"))

            l.Add(New ColonneNome("Data_Creazione", "Data Creazione", "date"))
            l.Add(New ColonneNome("Utente_Creazione", "Utente Creazione", "string"))
            l.Add(New ColonneNome("Data_Modifica", "Data Modifica", "date"))
            l.Add(New ColonneNome("Utente_Modifica", "Utente Modifica", "string"))

            l.Add(New ColonneNome("Validita_Inizio", "Validita Inizio", "date"))
            l.Add(New ColonneNome("Validita_Fine", "Validita Fine", "date"))

            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            'Dim risp As String = js.JSON_DataTable_Kendo(dt, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa)
            r.RispostaStringa = JsonConvert.SerializeObject(dt)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboCmb_OrganismiODC_Modello(ByVal piva As String, ByVal objP_server As String) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.anagrafiche.RisorseUmane))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.anagrafiche.RisorseUmane))

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objRisorseUmane_R As New AgronicaCoreAnagrafeBIZ.RisorseUmane_R

            Dim l = objRisorseUmane_R.Leggi_RisorseUmane_ByCod_Rapporto(piva,
                                                                        enum_Rapporti_Contabili_Standard.Organismo_Di_Controllo,
                                                                        objParametriServer)

            r.RispostaStringa = l
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboCmb_OrganismiODC_Modello_NG(InData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.LeggiFiltro)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.anagrafiche.RisorseUmane))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.anagrafiche.RisorseUmane))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objRisorseUmane_R As New AgronicaCoreAnagrafeBIZ.RisorseUmane_R

            Dim l = objRisorseUmane_R.Leggi_RisorseUmane_ByCod_Rapporto(InData.InData.Ricerca,
                                                                        enum_Rapporti_Contabili_Standard.Organismo_Di_Controllo,
                                                                        objParametriServer)

            r.RispostaStringa = l
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Leggi_Contatti_Anagrafica1(InData As CoreWS_Generic(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG)) As RispostaStandard
        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            'Lingua.Gias_InizializzaCultura_DaSession()

            Dim objParametriAgenda = InData.InData

            If InData.InData.Data <> AGRODATAINIZIO Then
                objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(objParametriAgenda.Data.Date, objParametriAgenda.Data.Date)
            End If

            Dim dt As DataTable = objContatti.leggi_x_anagrafica2(objParametriAgenda.Piva, "0", "", "", objParametri_Server)
            Dim dt1 As DataTable = objContatti.leggi_x_anagrafica2(objParametriAgenda.Piva, "-1", "", "", objParametri_Server)

            If objParametriAgenda.Data <> AGRODATAINIZIO Then
                objParametri_Server.ResettaFinestra()
            End If

            dt.Merge(dt1)
            'dt.Columns.Add(New DataColumn("Tipo_Contatto", GetType(String)))

            Dim col As New DataColumn("Tipo_Contatto", GetType(String))
            col.Expression = "IIF(sa_cod = -1, 'Pubblico', 'Privato')"
            dt.Columns.Add(col)

            dt.Columns.Add(New DataColumn("Impresa_Referente", GetType(String)))

            'For Each row In dt.Rows
            '    If row("sa_cod") = -1 Then
            '        row("Tipo_Contatto") = "Pubblico"
            '    Else
            '        row("Tipo_Contatto") = "Privato"
            '    End If
            'Next

            'creo la lista delle colonne da visualizzare
            Dim l As New List(Of ColonneNome)

            l.Add(New ColonneNome("chiave", "chiave", "string") With {._hidden = True})
            l.Add(New ColonneNome("CF", "CF", "string"))
            l.Add(New ColonneNome("sa_cod", "sa_cod", "string"))
            l.Add(New ColonneNome("Contatto_Des", "Nome del Contatto", "string"))

            l.Add(New ColonneNome("Rapporto_Des", "Rapporto", "string"))
            l.Add(New ColonneNome("Cod_Rapp", "Cod_Rapp", "string") With {._hidden = True})
            l.Add(New ColonneNome("Cod_Risum", "Cod_Risum", "string") With {._hidden = True})

            l.Add(New ColonneNome("Tipo_Contatto", "Tipo", "string"))
            l.Add(New ColonneNome("Impresa", "Impresa Referente", "string"))
            l.Add(New ColonneNome("Settore_Des", "Codice Contatto", "string"))
            l.Add(New ColonneNome("Attivita_Des", "Codice", "string"))

            l.Add(New ColonneNome("Data_Creazione", "Data Creazione", "date"))
            l.Add(New ColonneNome("Utente_Creazione", "Utente Creazione", "string"))
            l.Add(New ColonneNome("Data_Modifica", "Data Modifica", "date"))
            l.Add(New ColonneNome("Utente_Modifica", "Utente Modifica", "string"))

            l.Add(New ColonneNome("Validita_Inizio", "Validita Inizio", "date"))
            l.Add(New ColonneNome("Validita_Fine", "Validita Fine", "date"))

            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            Dim risp As String = js.JSON_DataTable_Kendo(dt, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa)
            r.RispostaStringa = risp
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_CodiceAUSL_Contatto(ByVal objP_server As String, ByVal piva As String, ByVal Cod_Contatto As String) As RispostaStandard
        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim objContatti As New AgronicaCoreAnagrafeBIZ.Indirizzi_R
            Dim codiceAUSL As String = objContatti.CodiceAslDatoContatto(piva, Cod_Contatto, objParametriServer)

            r.RispostaStringa = codiceAUSL
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function CaricaListaPersone(InData As CoreWS_Generic(Of AgronicaCoreModelsSTD.attivita.ModificaMultipla_Attivita)) As RispostaStandard
        Dim r As New RispostaStandard

        Try
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
            Dim data As AgronicaCoreModelsSTD.attivita.ModificaMultipla_Attivita = (InData.InData)

            If objParametri_Server Is Nothing Then
                r.Errore = "objP_server non valorizzato"
                Return r
            End If
            If objParametri_Utenti Is Nothing Then
                r.Errore = "objP_utenti non valorizzato"
                Return r
            End If
            If objParametri_Super_Server Is Nothing Then
                r.Errore = "objP_super_server non valorizzato"
                Return r
            End If

            Dim contattiR As New AgronicaCoreAnagrafeBIZ.Contatti_MultiHost_R
            Dim DT_Contatti = contattiR.CaricaListaPersone(data.Attivita_list, data.Solo_Aziendali, objParametri_Server)

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(DT_Contatti)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Leggi_Contatti_Macchine(InData As CoreWS_Generic(Of LeggiContattiMacchina)) As rispostaStandard(Of List(Of Contatto))
        Dim r As New rispostaStandard(Of List(Of Contatto))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            'Lingua.Gias_InizializzaCultura_DaSession()

            Dim objParametriAgenda = InData.InData.objNG
            Dim iData = InData.InData

            If InData.InData.objNG.Data <> AGRODATAINIZIO Then
                objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(objParametriAgenda.Data.Date, objParametriAgenda.Data.Date)
            End If

            Dim dt As DataTable = objContatti.LeggiDatiMinimi(objParametriAgenda.Piva, iData.Cod_Contatto, "", objParametri_Server, iData.Flag_Pubblico_Privato, iData.Flag_Visibilita_Centri)

            If objParametriAgenda.Data <> AGRODATAINIZIO Then
                objParametri_Server.ResettaFinestra()
            End If

            Dim listItems = (From dr In dt.Rows
                             Select New AgronicaCoreModelsSTD.anagrafiche.Contatto() With {
                                 .ragione_Sociale = dr.Item("Rag_Soc"),
                                 .primaryKey = New AgronicaCoreModelsSTD.anagrafiche.Contatto.PK(objParametriAgenda.Piva, dr("Cod_Contatto")),
                                 .sa_cod = dr.Item("Sa_Cod")
                                 }).ToList

            r.RispostaStringa = listItems
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Leggi_Contatti_Stazioni_Meteo(InData As CoreWS_Generic(Of String)) As rispostaStandard(Of List(Of Contatto))
        Dim r As New rispostaStandard(Of List(Of Contatto))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            'Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim Data_Validita = Date.Now.Date
            objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(Data_Validita, Data_Validita)

            Dim piva = InData.InData
            Dim dt As DataTable = objContatti.LeggiContattiStazioniMeteo(piva, objParametri_Server)

            Dim listItems = (From dr In dt.Rows
                             Select New AgronicaCoreModelsSTD.anagrafiche.Contatto() With {
                                 .ragione_Sociale = dr.Item("Rag_Soc") & dr.Item("Nome") & " " & dr.Item("Cognome"),
                                 .primaryKey = New AgronicaCoreModelsSTD.anagrafiche.Contatto.PK(piva, dr("Cod_Contatto")),
                                 .sa_cod = dr.Item("Sa_Cod")
                                 }).ToList

            objParametri_Server.ResettaFinestra()

            r.RispostaStringa = listItems
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Leggi_Contatti_Stazioni_Meteo_APP(ByVal piva As String,
                                     ByVal objP_super_server As String,
                                ByVal objP_server As String,
                                ByVal objP_utenti As String) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim objAppHelper As New AppHelper
            Dim impostazioni = objAppHelper.Leggi_Impostazioni_APP(objParametri_Utenti)
            If impostazioni IsNot Nothing AndAlso impostazioni.StazioniMeteo = False Then
                r.RispostaOK = True
                Return r
            End If

            Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R

            Dim Data_Validita = Date.Now.Date
            objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(Data_Validita, Data_Validita)

            Dim dt As DataTable = objContatti.LeggiContattiStazioniMeteo(piva, objParametri_Server)

            objParametri_Server.ResettaFinestra()

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

#Region "COMBO MODIFICA MULTIPLA"
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboCmb_RiferimentoTrasferimentoDati(ByVal objP_server As String, piva As String) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim objOR As New AgronicaCoreAnagrafeDAL.RiferimentoTrasferimentoDati_R
            Dim DT As DataTable = objOR.Leggi(True, piva, "", "", "", objParametri_Server)

            Dim jArray As New JArray()
            For Each dr As DataRow In DT.Rows
                jArray.Add(New JObject(New JProperty("text", dr.Item("ragsoc_padre")),
                                                   New JProperty("value", dr.Item("piva_padre"))))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArray, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function
#End Region
End Class