Imports System.Web.Services
Imports AgroAgenda_2010.Resources
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class RicercaTrasferimenti
    Inherits System.Web.UI.Page


    <WebMethod(EnableSession:=True)>
    Public Shared Function ElencoTrasferimenti(ByVal piva As String,
                                               ByVal idAgenda As Integer,
                                               ByVal dataDal As String,
                                               ByVal dataAl As String,
                                               ByVal des_TestataGriglia As String
                                               ) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try
            Dim objAgenda As New AgronicaCoreContabBIZ.FF_MagazzinoBIZ
            r.RispostaStringa = objAgenda.Leggi_Trasferimenti(piva, idAgenda, 0, "",
                                                            des_TestataGriglia,
                                                            dataDal, dataAl,
                                                            objParametriServer)
            'sceltaAperteChiuseTutte,
            'chkTotCarScar,


            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function ElencoTrasferimentiDettagli(ByVal piva As String,
                                                       ByVal dataDal As String,
                                                       ByVal dataAl As String,
                                                       ByVal des As String,
                                                       ByVal categorieProdotti As String,
                                                       ByVal codiciProdotti As String,
                                                       ByVal specie As String,
                                                       ByVal varieta As String) As RispostaStandard

        'Controllo Sessione
        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Lingua.Gias_InizializzaCultura_DaSession()

        'Controllo dei parametri in ingresso
        Dim dataDalObj As Date
        If Not Date.TryParse(dataDal, dataDalObj)
            dataDalObj = AGRODATAINIZIO
        End If
        Dim dataAlObj As Date
        If Not Date.TryParse(dataAl, dataAlObj)
            dataAlObj = AGRODATAFINE
        End If

        'Impostazione dei filtri aggiuntivi
        Dim filtroAggiuntivo As New StringBuilder()
        filtroAggiuntivo.AppendLine()
        filtroAggiuntivo.AppendLine("(  Agenda.des_lib LIKE '%" & Agro_SQL_SaveText(des) & "%'")
        filtroAggiuntivo.AppendLine(" AND movimenti.data_movimento BETWEEN " & Agro_SQL_SaveDate(dataDalObj) & " AND " & Agro_SQL_SaveDate(dataAlObj))
        filtroAggiuntivo.AppendLine(" AND Movimenti_Dettagli.Ordine_Det <> 1000")

        If Not String.IsNullOrEmpty(codiciProdotti) Then
            filtroAggiuntivo.Append(" AND ( ")
            Dim elenco As New System.Text.StringBuilder("")

            'La variabile codiciProdotti è formata da coppie di codici con la seguente sintassi:
            '<categoria prodotto>_<prodotto>
            'e sono separate fra loro da una pipe |
            Dim prodArray As String() = codiciProdotti.Split("|")

            For Each p In prodArray
                If Not elenco.ToString() = "" Then
                    elenco.Append(" OR ")
                End If
                elenco.Append(" ( ")

                Dim dueParti As String() = p.Split("_")
                elenco.Append(" Movimenti_Dettagli.Elem_Cod =  " & CInt(dueParti(0)) & " AND ")
                
                'Per convenzione ho i codici prodotti positivi nella colonna "Pro_Cod", 
                'mentre quelli negativi nella colonna "Mat_Cod"
                If CInt(dueParti(1)) > 0 Then
                    elenco.Append(" Movimenti_Dettagli.Pro_Cod =  " & CInt(dueParti(1)))
                Else
                    elenco.Append(" Movimenti_Dettagli.Mat_Cod =  " & CInt(dueParti(1)) * -1)
                End If

                elenco.AppendLine(" ) ")
            Next

            filtroAggiuntivo.Append(elenco.ToString())
            filtroAggiuntivo.AppendLine(" ) ")
        End If

        If Not String.IsNullOrEmpty(categorieProdotti) Then
            filtroAggiuntivo.AppendLine(" AND (Materie_Prime.Elem_Cod IN (" & Agro_SQL_Save_Clausola_IN(Replace(categorieProdotti, "|", ","), False) & ") ) ")
        End If

        If Not String.IsNullOrEmpty(specie) Then
            filtroAggiuntivo.AppendLine(" AND Materie_Prime.Veg_Cod IN (" & Agro_SQL_Save_Clausola_IN(Replace(specie, "|", ","), False) & ")")
        End If

        If Not String.IsNullOrEmpty(varieta) Then
            filtroAggiuntivo.AppendLine(" AND Materie_Prime.Cul_Cod IN (" & Agro_SQL_Save_Clausola_IN(Replace(varieta, "|", ","), False) & ")")
        End If

        filtroAggiuntivo.Append("  )")
        Dim orderby As String = " Movimenti.Data_Movimento DESC, Agenda.Id_Agenda ASC"

        Try
            Dim objMagazzinoBIZ As New AgronicaCoreContabBIZ.FF_MagazzinoBIZ
            r.RispostaStringa =
            objMagazzinoBIZ.Leggi_Movimenti_Carico_Scarico(piva,
                0, CAU_SCARICO, 0, LAVCOD_TRASFERIMENTO, True,
                Nothing, objParametri_Server, objParametriUtenti, filtroAggiuntivo.ToString(), orderby, , , True, contestoDocContabile:=True)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False, source:=True)
        End Try

        Return r

    End Function

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()
    End Sub

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Public objparametri_server_string, objparametri_utenti_string As String
    Dim objParametriAgenda As ParametriAgenda

    Private Sub inizializzoObjParametri()
        objParametriAgenda = New ParametriAgenda
        '---
        objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objparametri_server_string = Utility.convertOBJparametritoString(objParametri_Server)
        '---
    End Sub


#Region "Caricamento"

    Private Sub caricaControlli()
    End Sub

#End Region

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        hdPiva.Value = Stringa_Decodifica(CStr(Request.QueryString("p")), AgroKey_EncoderDecoder)

        inizializzoObjParametri()

        If Not IsPostBack Then
            InizializzoParametriPagina()
        End If

        'Controllo se l'utente ha i permessi per accedere
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim UtenteAbilitatoLettura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                            Session("ASG_Utente_Username"),
                                            Session("ASG_IdServizio"),
                                            enum_Security_Attivita.Trasferimenti_Magazzino,
                                            enum_Security_Operazione.Lettura,
                                            Date.Now,
                                            "",
                                            objParametri_Utenti)

        Dim UtenteAbilitatoScrittura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                           Session("ASG_Utente_Username"),
                                           Session("ASG_IdServizio"),
                                           enum_Security_Attivita.Trasferimenti_Magazzino,
                                           enum_Security_Operazione.Modifica,
                                           Date.Now,
                                           "",
                                           objParametri_Utenti)

        'Imposto le variabili di ponte con il client
        hf_UtenteAbilitatoLettura.Value = UtenteAbilitatoLettura
        hf_UtenteAbilitatoScrittura.Value = UtenteAbilitatoScrittura

        If UtenteAbilitatoLettura = False Then
            Response.Redirect("~/classi/Agro_Pages/AccessoNonConsentito.aspx")
        End If

        If UtenteAbilitatoScrittura = False Then

        End If

        If Not Page.IsPostBack Then
            caricaControlli()
        End If
        
        Master().Lbl_Titolo.Text = AgronicaAgenda_2010.RicercaTrasferimenti.ToUpper()

    End Sub

    Private Sub InizializzoParametriPagina()
        Master.SetTitoloPagina(192)
    End Sub

    Public Shadows ReadOnly Property Master() As AgroAgenda_2010.AgendaBootstrap
        Get
            Return CType(MyBase.Master, AgroAgenda_2010.AgendaBootstrap)
        End Get
    End Property

    <WebMethod(EnableSession:=True)>
    Public Shared Function ApriModificaTrasferimento(ByVal piva As String,
                                                     ByVal idAgenda As Integer,
                                                     ByVal tipoOperazione As Integer
                                                     ) As RispostaStandard

        Dim r As New RispostaStandard
        Dim paginaLink As String = ""

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If
        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim objParametriAgenda As New ParametriAgenda With {
                .Tipo_Operazione = tipoOperazione,
                .Lav_Cod = LAVCOD_TRASFERIMENTO,
                .Id_Agenda = idAgenda
            }

            paginaLink = "Trasferimento.aspx"
            paginaLink &= "?p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder) &
                          "&orig=" & Stringa_Codifica(enum_PagineAgenda_2010.Pagina_Ricercatrasferimenti, AgroKey_EncoderDecoder) &
                          "&Id_Agenda=" & objParametriAgenda.Id_Agenda

                          '"&o=" & Stringa_Codifica(objParametriAgenda.Tipo_Operazione, AgroKey_EncoderDecoder) &

            r.RispostaOK = True
            r.RispostaStringa = paginaLink

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function CancellaTotaleTrasferimento(ByVal piva As String,
                                                       ByVal idAgenda As Integer,
                                                       ByVal messaggioDettagliato As Boolean
                                                       ) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objOutput As New AgronicaCoreModello.Trasferimento_Output
        Dim msgError As String = ""

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) OrElse IsNothing(objParametriUtenti) Then
            r.Sessione = False
            Return r
        End If

        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim objTrasfHelper As New AgronicaCoreModello.TrasferimentoHelper(objParametriServer, objParametriUtenti)
            objOutput = objTrasfHelper.CancellaDocumentoTrasferimento(piva, idAgenda, messaggioDettagliato, msgError)

            r.RispostaOK = objOutput.Risultato
            r.RispostaStringa = JsonConvert.SerializeObject(objOutput, settingLoc)

        Catch ex As Exception

            If objOutput.MsgError = "" Then
                objOutput.MsgError = AgronicaAgenda_2010.ErroreCancellazione
            End If

            r.RispostaOK = False
            r.RispostaStringa = JsonConvert.SerializeObject(objOutput, settingLoc)
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Specie() As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If
        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim SpecieVegetali_R As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R()
            Dim dtSpecie = SpecieVegetali_R.Leggi(0, 0, "", "", enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametriServer)

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            r.RispostaStringa = JsonConvert.SerializeObject(dtSpecie, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Varieta(filtro_specie As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim filtro_aggiuntivo As String = ""
            If Trim(filtro_specie) <> "" Then
                filtro_aggiuntivo = " Cultivar.Veg_Cod IN (" & filtro_specie & ") "
            End If

            Dim Cultivar_R As New AgronicaCoreMetaSchemaDAL.Cultivar_R
            Dim dtVarieta = Cultivar_R.Leggi(0, 0, "", enumSelezioneVariabile.Selezione_JoinDescrizioni, filtro_aggiuntivo, "", objParametri_Server)

            Dim jArrayParamQual As New JArray()
            For Each dr As DataRow In dtVarieta.Rows
                jArrayParamQual.Add(New JObject(New JProperty("Cul_Cod", dr.Item("Cul_Cod")), New JProperty("Cul_Des", dr.Item("Veg_Des") & " - " & dr.Item("Cul_Des"))))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayParamQual, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Categorie_Magazzino() As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If
        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim objCM As New AgronicaCoreMetaSchemaDAL.Categorie_Magazzino_R
            Dim dtCategMag = objCM.Leggi(0, "", False, " Elem_Cod NOT IN (1,4,300) ", "", objParametriServer)

            Dim jArrayCategorie As New JArray()
            For Each dr As DataRow In dtCategMag.Rows
                jArrayCategorie.Add(New JObject(New JProperty("Elem_Cod", CStr(dr.Item("Elem_Cod"))), New JProperty("Elem_Des", dr.Item("NomeComune"))))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayCategorie, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

End Class