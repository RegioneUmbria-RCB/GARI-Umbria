Imports System.Web.Services
Imports System.Text
Imports System.Data
Imports System.Web
Imports System.Web.Script.Serialization
Imports System.Web.Script.Services

Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreMetaSchemaDAL
Imports System.ComponentModel
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieDAL


''' <summary>
''' Per utilizzare questo controllo è necessario includere all'iterno della pagina i seguenti script 
''' jquery.cookie.js
''' jquery.hotkeys.js
''' jquery.jstree.js
''' </summary>
''' <remarks></remarks>


<DefaultProperty("Text"), ToolboxData("<{0}:Albero_Gerarchia_Imprese_FAST runat=server></{0}:Albero_Gerarchia_Imprese_FAST>")> _
Public Class Albero_Gerarchia_Imprese_FAST
    Inherits System.Web.UI.WebControls.WebControl


    Public Hidden As HiddenField

    Public c_sementi As String = ""
    Public Property _Cod_Sementi() As String
        Get

            Return c_sementi
        End Get
        Set(ByVal value As String)
            c_sementi = value
        End Set
    End Property

    Public c_figli As Boolean = True
    Public Property _c_figli() As Boolean
        Get

            Return c_figli
        End Get
        Set(ByVal value As Boolean)
            c_figli = value
        End Set
    End Property



    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)

        Hidden = New HiddenField
        Hidden.ID = "HiddenSelezioneAlbero_Gerarchia_Imprese_FAST"


        Me.Controls.Add(Hidden)
        MyBase.OnInit(e)
    End Sub


    Public Property Valore_Albero() As String
        Get

            Return Hidden.Value
        End Get
        Set(ByVal value As String)
            Hidden.Value = value
        End Set
    End Property


    ''' <summary>
    ''' Funzione che viene invocata quando si richiede il caricamento del sottonodo
    ''' NB ricordarsi di far "l'overload" all'interno della pagina chiamante come nell'esempio seguente
    ''' WebMethod(EnableSession:=True)> _
    ''' Public Shared Function GetNodes(ByVal id As String) As String
    '''    Return AgronicaControlli_2010.ServerControl1.GetNodesAgenda(id)
    ''' End Function
    ''' </summary>
    ''' <param name="id"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>




    <WebMethod(EnableSession:=True, CacheDuration:=43200)> _
    Public Function GetNodesGerarchia(ByVal id As String, ByVal PathRoot As String) As String

        'Dim strP As String = HttpContext.Current.Session("ASG_Utente_Username") & "_" & "ListaProvincie_" & id & "_" & c_figli & "_" & c_sementi

        'If IsNothing(System.Web.HttpContext.Current.Cache(strP)) Then



        Dim results As List(Of AjaxTreeNode_FAST_JsonObject) = GetNodesGearchiaObj(
            PathRoot,
            HttpContext.Current.Session("ASG_Utente_Username"),
            HttpContext.Current.Session("ASG_objParametri_Server"),
            HttpContext.Current.Session("ASG_objParametri_Utenti"))

        Dim ser As New JavaScriptSerializer()
        ser.MaxJsonLength = 50000000
        'System.Web.HttpContext.Current.Cache(strP) = ser.Serialize(results)
        Return ser.Serialize(results)
        'Else
        'Return System.Web.HttpContext.Current.Cache(strP)
        'End If
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="PathRoot"></param>
    ''' <param name="ASG_Utente_Username">HttpContext.Current.Session("ASG_Utente_Username")</param>
    ''' <param name="objparametri_Server">HttpContext.Current.Session("ASG_objParametri_Server")</param>
    ''' <param name="objparametri_Utenti">HttpContext.Current.Session("ASG_objParametri_Utenti")</param>
    ''' <returns></returns>
    Public Function GetNodesGearchiaObj(PathRoot As String, ASG_Utente_Username As String, objparametri_Server As AgronicaCoreParametri, objparametri_Utenti As AgronicaCoreParametri) As List(Of AjaxTreeNode_FAST_JsonObject)
        Dim results As New List(Of AjaxTreeNode_FAST_JsonObject)
        Dim Livello_Impresa As New List(Of AjaxTreeNode_FAST_JsonObject)
        Dim Testo As String
        Dim xChiave As String
        '************************
        '***** NODO UTENTE ******
        '************************
        Dim objUtente As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R
        Dim DTUtente As DataTable
        DTUtente = objUtente.Leggi(ASG_Utente_Username, 5,
                                   enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                   "", "", objparametri_Utenti)
        Dim TipoUtente As Integer

        'Prelevo la Ragione Sociale oppure Nome e Cognome
        If DTUtente.Rows.Count > 0 Then
            'Verifico il tipo di utente ... Azienda/Persona
            TipoUtente = DTUtente.Rows(0).Item("Flag_Azienda_Persona")
            If TipoUtente = 1 Then
                Testo = DTUtente.Rows(0).Item("Rag_Soc")
            Else
                Testo = DTUtente.Rows(0).Item("Cognome") & " " &
                        DTUtente.Rows(0).Item("Nome")
            End If
        Else
            Testo = ASG_Utente_Username
        End If
        'Genero la chiave
        Call Albero.ChiaveAlbero_Codifica_x_json(xChiave, enum_TipoNodo.Utente, , , , , , , , , , , , , , )

        Dim Radice As AjaxTreeNode_FAST_JsonObject
        Radice = New AjaxTreeNode_FAST_JsonObject(xChiave, AgroPrefix_Utente & Testo, "", "", "#", Livello_Impresa)


        Radice.icon = PathRoot + Albero.RitornaPathImg(enum_TipoNodo.Utente, "")
        Radice.state.opened = "false"

        ' parto dalla pivasuperuser
        Dim objGerImprese As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R
        Dim DT_Gerarchie As DataTable

        'Dim Cod_Sementi As Integer = HttpContext.Current.Session("Sementi").split("|")(4)
        Dim validita_inizio As String
        Dim validita_fine As String

        If Not IsNothing(HttpContext.Current.Session("Sementi")) Then
            _Cod_Sementi = HttpContext.Current.Session("Sementi").split("|")(4)

            Try
                validita_inizio = HttpContext.Current.Session("Sementi").ToString.Split("|")(6)
            Catch ex As Exception
                validita_inizio = AGRODATAINIZIO
            End Try
            'validita_fine = HttpContext.Current.Session("Sportello_Validita_Fine")
            Try
                validita_fine = HttpContext.Current.Session("Sementi").ToString.Split("|")(7)
            Catch ex As Exception
                validita_fine = AGRODATAFINE
            End Try
        Else
            _Cod_Sementi = ""
            validita_inizio = AGRODATAINIZIO
            validita_fine = AGRODATAFINE
        End If

        If _Cod_Sementi <> "" Then
            'identifico le PIVA che posso vedere con l'utente selezionato
            Dim listaPiva As String = GetPive(objparametri_Server)
            If listaPiva.Length > 0 Then
                listaPiva = listaPiva + " , ''"
            End If
            DT_Gerarchie = objGerImprese.LeggixAlberoImpresexSementi(listaPiva, _Cod_Sementi, validita_inizio, validita_fine, objparametri_Server)
        Else
            ''controllo se l'utente ha una visibilità ridotta
            Dim objProfilo As New AgronicaCoreUtentiDAL.Utenti_Profili_Read
            Dim Filtro_Visibilita_Utente = Not objProfilo.HasFullVisibility(objparametri_Server.UtenteUsername, objparametri_Utenti)
            DT_Gerarchie = objGerImprese.LeggixGerarchiaAlberoImprese_Visibilita(Filtro_Visibilita_Utente, validita_inizio, validita_fine, objparametri_Server)

            'Dim listaPiva As String = GetPive(objparametri_Server)
            'If listaPiva.Length > 0 Then
            '    listaPiva = listaPiva + " , ''"
            'End If
            'Dim DT_Gerarchie1 = objGerImprese.LeggixGerarchiaAlberoImprese(listaPiva, validita_inizio, validita_fine, objparametri_Server)

        End If

        Dim pivaPadre As String = ""
        'If objparametri_Server.UtenteUsername <> objparametri_Server.SuperUserUsername Then
        '    'pivaPadre = CType(HttpContext.Current.Session("ASG_objParametri_Server"), AgronicaCoreDataProvider.AgronicaCoreParametri).UsernameOperazione
        'End If

        If c_figli = False Then
            Dim view = New DataView(DT_Gerarchie)
            view.RowFilter = "foglia <> 1"
            DT_Gerarchie = view.ToTable
        End If

        RestituisciNodoImpresa(results, DT_Gerarchie, pivaPadre, PathRoot)
        Radice.children.AddRange(results)

        Dim listaRadice As New List(Of AjaxTreeNode_FAST_JsonObject)
        listaRadice.Add(Radice)

        Return listaRadice
    End Function

    Private Function GetPive(objParametri_Server As AgronicaCoreParametri)
        Dim Dt_Imprese As DataTable
        Dim ClassJoin As New JoinFiltrone
        Dim classFiltrone As New AgronicaCoreUtility.Filtrone
        Dim xFiltroAggiuntivo As String = ""

        'Per il momento imposto sempre a true perchè può capitare
        'che il filtro associato all'utente vada a controllare il campo Padre e/o Foglia
        '(di GerarchiaImprese), ma non essendo specificata la tabella GerarchiaImprese
        'prima del nome del campo, la funzione ImpostaVariabiliJOIN_xFiltroUtente non la trova e non imposta il join
        ClassJoin.bGerarchiaImprese = True

        classFiltrone.ImpostaVariabiliJOIN_xFiltroUtente(xFiltroAggiuntivo, ClassJoin)
        Dt_Imprese = classFiltrone.CreaDTFiltrone(
            objParametri_Server, xFiltroAggiuntivo, enum_TipoSelect_FiltroneSuperNova.Imprese, "", ClassJoin)

        Dim vLista As List(Of String) = (
            From dd In Dt_Imprese.AsEnumerable
            Select "'" & CStr(dd("piva")) & "'").ToList

        Return String.Join(",", vLista)
    End Function


    Private Sub RestituisciNodoImpresa(ByRef results As List(Of AjaxTreeNode_FAST_JsonObject),
                                            ByRef DT_Gerarchie As DataTable, ByRef PivaPadre As String,
                                            ByVal PathRoot As String)

        Dim TipoImpresaGerarchia As Integer
        Dim xPiva As String = ""
        Dim ValiditaFineImpresa As Date
        Dim xRag_Soc As String = ""
        Dim ValidazioneNodo As String = ""
        Dim CertificatiBloccati As String = ""
        Dim xChiave As String


        'seleziono i nodi 
        Dim DR() As DataRow
        DR = DT_Gerarchie.Select("Padre = '" & PivaPadre & "'")
        Dim i As Integer

        If PivaPadre = "" AndAlso DR.Length = 0 Then
            Dim DTPadri = DT_Gerarchie.DefaultView.ToTable(True, "Padre")
            For j = 0 To DTPadri.Rows.Count - 1
                Dim padre As String = DTPadri.Rows(j)(0)
                Dim eFiglio = DT_Gerarchie.Select("Piva = '" & padre & "'")
                If eFiglio.Length = 0 Then
                    RestituisciNodoImpresa(results, DT_Gerarchie, padre, PathRoot)
                End If
            Next
            Return
        End If



        For i = 0 To DR.Length - 1
            Dim avanti As Boolean
            avanti = True

            'prima controllo se è foglia 
            Dim Foglia As Boolean
            If DR(i).Item("Foglia") = 1 Then
                Foglia = True
            Else
                Foglia = False
                'controllo se non ha figli non lo metto
                If DR(i).Item("Piva") <> "" Then
                    Dim DR_padre() = DT_Gerarchie.Select("Padre = '" & DR(i).Item("Piva") & "'")
                    If DR_padre.Length = 0 Then
                        If c_figli = True Then
                            avanti = False
                        End If
                    End If
                End If
            End If
            If avanti = True Then
                xPiva = DR(i).Item("Piva")
                xRag_Soc = DR(i).Item("Rag_Soc")
                ValidazioneNodo = DR(i).Item("Validazione") & ""
                CertificatiBloccati = DR(i).Item("Blk_Flag") & ""
                ValiditaFineImpresa = CDate(DR(i).Item("validita_fine"))
                TipoImpresaGerarchia = DR(i).Item("TipoImpresaGerarchia")


                '----- Creo il nodo IMPRESA sul albero
                If CertificatiBloccati = "-1" Then
                    xRag_Soc = "--SOSPESA-- " & xRag_Soc
                End If

                Dim TipoImpresa As enum_TipoNodo
                TipoImpresa = GetTipoImpresa(TipoImpresaGerarchia)
                'Genero una chiave
                Call Albero.ChiaveAlbero_Codifica_x_json(xChiave,
                                            TipoImpresa,
                                            Piva:=xPiva,
                                            PivaPadre:=PivaPadre
                                        )

                Dim Livello2 As New List(Of AjaxTreeNode_FAST_JsonObject)
                If Foglia = True Then
                    Livello2 = Nothing
                Else
                    RestituisciNodoImpresa(Livello2, DT_Gerarchie, xPiva, PathRoot)
                End If


                Dim Impresa As New AjaxTreeNode_FAST_JsonObject
                If IsNothing(Livello2) Then
                    Impresa = New AjaxTreeNode_FAST_JsonObject(xChiave,
                                                   xRag_Soc, "jstree-no-checkboxes", "", "#", False)
                Else
                    Impresa = New AjaxTreeNode_FAST_JsonObject(xChiave,
                                                   xRag_Soc, "jstree-no-checkboxes", "", "#", Livello2)
                End If



                If PivaPadre = "" Then
                    Impresa.state.opened = "true"
                Else
                    Impresa.state.opened = "false"
                End If

                Impresa.state.selected = "false"
                Impresa.icon = PathRoot + Albero.RitornaPathImg(TipoImpresa)

                '----------------------
                'FINE - IMPRESA
                '----------------------
                results.Add(Impresa)
            End If
        Next

    End Sub


    Private Function GetTipoImpresa(ByVal TipoImpresaGerarchia As String)
        Select Case TipoImpresaGerarchia
            Case 1
                Return enum_TipoNodo.Impresa
            Case 2
                Return enum_TipoNodo.x_Cooperativa
            Case 3
                Return enum_TipoNodo.x_Consorzio
            Case 4
                Return enum_TipoNodo.x_OP
        End Select
        Return 1
    End Function









    ''' <summary>
    ''' Per Renderizzare il controllo
    ''' </summary>
    ''' <param name="writer"></param>
    ''' <remarks></remarks>
    Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
        Dim IDDiv As String = "treeGerarchia" & Me.ClientID

        Dim provahtml As New StringBuilder
        provahtml.Append("<div id='" + IDDiv + "'></div>")
        writer.Write(GetJS(IDDiv) + GetScriptSelect(IDDiv) + provahtml.ToString)
        MyBase.Render(writer)

    End Sub

    ''' <summary>
    ''' Funzione per Farsi Restituire la stringa contenente il codice JS da inserire all'interno della pagina
    ''' !!! Da estendere eventualmente con la funzione del precaricamento tramite cookies !!!
    ''' </summary>
    ''' <param name="IDDiv">ID del DIV su cui applicare il plugin</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetJS(ByVal IDDiv As String) As String
        Dim js As New StringBuilder

        Dim IdNodiAperti As String = "''"

        Dim path As String
        Dim objAgroWebConfig As New AgroWebConfig
        path = objAgroWebConfig.LinkAgronicaAgenda2010.Replace("/GestioneRichieste.aspx", "")

        js.AppendLine("<script type='text/javascript'>")

        js.AppendLine("             var pippo; ")

        js.AppendLine("             function replaceAll(find, replace, str) { ")
        js.AppendLine("               return str.replace(new RegExp(find, 'g'), replace); ")
        js.AppendLine("             } ")

        js.AppendLine(" function getPathRoot_FAST() {")
        js.AppendLine("     var port = location.port; ")
        js.AppendLine("     if (port != '' ) { port = ':'+ port;} ")
        js.AppendLine("     var ppp = 'http://' + location.hostname + port + '" & path & "'; ")
        js.AppendLine("     return ppp; ")
        js.AppendLine("     } ")

        js.AppendLine(" function CaricaGerarchia_FAST() {")
        js.AppendLine("     var ppp = getPathRoot_FAST(); ")
        js.AppendLine("     var id = '0'; ")

        js.AppendLine("     var v = ""{'id': '0', 'PathRoot': '"" + ppp + ""'}""; ")

        js.AppendLine("            pippo= v; ")

        js.AppendLine("     $.ajax({ ")
        js.AppendLine("         type:   'POST',")
        js.AppendLine("         url:    GetNameofPageGerarchia_FAST()  + '/GetNodesGerarchia_FAST', ")

        js.AppendLine("         data: v, ")
        'js.AppendLine("         data: ""{'id': '0', 'PathRoot': 'http://localhost/AgronicaAgenda_2010'}"", ")
        js.AppendLine("         contentType:  'application/json; charset=utf-8', ")
        js.AppendLine("         dataType:  'json', ")
        js.AppendLine("         success: function (r) { ")
        js.AppendLine("             var p= r.d; ")

        js.AppendLine("             p = replaceAll('""false""', 'false',p); ")
        js.AppendLine("             p = replaceAll('""true""', 'true',p); ")

        js.AppendLine("             $('#" + IDDiv + "').jstree({")
        js.AppendLine("                 'core' : { ")
        js.AppendLine("                 'themes' : { 'stripes' : true }, ")
        js.AppendLine("                 'data' :  JSON.parse(p) ")
        js.AppendLine("             } ")
        js.AppendLine("                 ,'plugins' : ['checkbox'] ")
        js.AppendLine("		    }); ")
        js.AppendLine(" } ")
        js.AppendLine("  }); ")
        js.AppendLine(" } ")




        js.AppendLine("$(document).ready(function () { ")
        js.AppendLine("     CaricaGerarchia_FAST();")
        js.AppendLine("  }); ")
















        'js.AppendLine("function OnGetNodesGerarchia_FAST(n){		")
        'js.AppendLine("     var port = location.port; ")
        'js.AppendLine("     if (port != '' ) { port = ':'+ port;} ")

        'js.AppendLine("     var obj = { ")
        'js.AppendLine("         id: n.attr ? n.attr('id') : '0'")
        'js.AppendLine("         , PathRoot : 'http://' + location.hostname + port + '/" & path & "'")
        'js.AppendLine("			}")
        'js.AppendLine("			return Sys.Serialization.JavaScriptSerializer.serialize(obj); ")
        'js.AppendLine("     }")




        'js.AppendLine("function OnNodesRetrievedSuccessGerarchia_FAST(data,textstatus,xhr){ ")
        'js.AppendLine("     return Sys.Serialization.JavaScriptSerializer.deserialize(data.d); ")
        'js.AppendLine("} ")

        'js.AppendLine(" function OnNodesRetrievedErrorGerarchia_FAST(xhr,textstatus,errorThrown){")
        'js.AppendLine("     alert('ERRORE!!!' +errorThrown); ")
        'js.AppendLine(" } ")


        'js.AppendLine("$(document).ready(function () { ")

        'js.AppendLine("    $('#" + IDDiv + "').jstree({ ")
        'js.AppendLine("        core : { ")
        'js.AppendLine("             'initially_open' : [ " & IdNodiAperti & "] ")
        'js.AppendLine("        }, ")
        'js.AppendLine("        ui : { ")
        'js.AppendLine("             'select_limit' : 1,'initially_select' :[ '' ] ")
        'js.AppendLine("        }, ")

        'js.AppendLine("        themes : { ")
        'js.AppendLine("        'theme' : 'apple' ")
        'js.AppendLine("        }, ")

        'js.AppendLine("        plugins: ['themes', 'json_data',  'ui',  'hotkeys'],")
        'js.AppendLine("        json_data: { ")
        'js.AppendLine("		            ajax: {")
        'js.AppendLine("                     url:    GetNameofPageGerarchia_FAST() +'/GetNodesGerarchia_FAST',")
        'js.AppendLine("                     async: true,")
        'js.AppendLine("                 contentType:  'application/json; charset=utf-8',")
        'js.AppendLine("                 dataType:  'json',")
        'js.AppendLine("                 type:   'POST',")
        'js.AppendLine("                 data: function (n) { return OnGetNodesGerarchia_FAST(n); },")
        'js.AppendLine("                 success: function (data, textstatus, xhr) {")
        'js.AppendLine("                     return OnNodesRetrievedSuccessGerarchia_FAST(data, textstatus, xhr)")
        'js.AppendLine("		            },")
        'js.AppendLine("                 error: function (xhr, textstatus, errorThrown) {")
        'js.AppendLine("                     OnNodesRetrievedErrorGerarchia_FAST(xhr, textstatus, errorThrown)")
        'js.AppendLine("                 }")
        'js.AppendLine("             }")
        'js.AppendLine("         }")
        'js.AppendLine("     });")


        'gestione del dbClick
        'js.AppendLine("     $(document).on('click', '#" + IDDiv + " ul li a', function () { ")
        ''js.AppendLine("     $('#" + IDDiv + " ul li a').live('click', function(){ ")
        ''controllo se il nodo è aperto o chiuso
        'js.AppendLine("         if($(this).parent().attr('class')=='jstree-open') { ")
        'js.AppendLine("         $('#" + IDDiv + "').jstree('close_node', this); } else {")
        'js.AppendLine("         $('#" + IDDiv + "').jstree('open_node', this); } ")
        'js.AppendLine("     });")



        'js.AppendLine(" });")





        js.AppendLine("function GetNameofPageGerarchia_FAST() {")
        js.AppendLine(" var pa = new String(window.location.pathname); ")
        js.AppendLine("var p2 = pa.split('/');")
        js.AppendLine("    return p2[p2.length - 1]; ")
        js.AppendLine("}")

        js.AppendLine("</script>")
        Return js.ToString
    End Function


    Private Function GetScriptSelect(ByVal ID As String)
        Dim StrSelect As New StringBuilder

        StrSelect.AppendLine("<script type='text/javascript'>")

        StrSelect.AppendLine("function GetSelezionatoFast() { ")
        StrSelect.AppendLine(" var key = ''; ")
        StrSelect.AppendLine(" $('.jstree-clicked').each(function() { ")
        StrSelect.AppendLine("      if (key!='') key =key + '|'; ")
        StrSelect.AppendLine("       key =key + $(this).parent().attr('id'); ")
        StrSelect.AppendLine(" }); ")

        StrSelect.AppendLine("       return key; ")
        StrSelect.AppendLine("} ")

        StrSelect.AppendLine("$(document).ready(function () { ")

        'StrSelect.AppendLine("  $('#" + ID + " ul').delegate('li', 'click', function(){ ")
        StrSelect.AppendLine("  $('#" + ID + "').on('click','.jstree-node', function(){ ")
        'StrSelect.AppendLine("  $('body').on('click','.jstree-node', function(){ ")

        StrSelect.AppendLine("      $('#" & Hidden.ClientID & "').val(GetSelezionatoFast()); ")

        StrSelect.AppendLine("  }); ")

        StrSelect.AppendLine("});")

        StrSelect.AppendLine("</script>")
        Return StrSelect.ToString
    End Function


End Class
