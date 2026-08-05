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


<DefaultProperty("Text"), ToolboxData("<{0}:Albero_Gerarchia_Imprese runat=server></{0}:Albero_Gerarchia_Imprese>")> _
Public Class Albero_Gerarchia_Imprese
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





    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)

        Hidden = New HiddenField
        Hidden.ID = "HiddenSelezioneAlbero_Gerarchia_Imprese" & Me.ClientID

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
    <WebMethod(EnableSession:=True)> _
    Public Function GetNodesGerarchia(ByVal id As String, ByVal PathRoot As String) As String

        Dim results As New List(Of AjaxTreeNodeJsonObject)
        Dim ser As New JavaScriptSerializer()

        Dim Livello_Impresa As New List(Of AjaxTreeNodeJsonObject)
        Dim Testo As String
        Dim xChiave As String
        '************************
        '***** NODO UTENTE ******
        '************************
        Dim objUtente As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R
        Dim DTUtente As DataTable

        DTUtente = objUtente.Leggi(HttpContext.Current.Session("ASG_Utente_Username"), _
                                   5, _
                                   enumSelezioneVariabile.Selezione_JoinDescrizioni, _
                                   "", "", HttpContext.Current.Session("ASG_objParametri_Utenti"))
        Dim TipoUtente As Integer

        'Prelevo la Ragione Sociale oppure Nome e Cognome
        If DTUtente.Rows.Count > 0 Then
            'Verifico il tipo di utente ... Azienda/Persona
            TipoUtente = DTUtente.Rows(0).Item("Flag_Azienda_Persona")
            If TipoUtente = 1 Then
                Testo = DTUtente.Rows(0).Item("Rag_Soc")
            Else
                Testo = DTUtente.Rows(0).Item("Cognome") & " " & _
                        DTUtente.Rows(0).Item("Nome")
            End If
        Else
            Testo = HttpContext.Current.Session("ASG_Utente_Username")
        End If
        'Genero la chiave
        Call Albero.ChiaveAlbero_Codifica_x_json(xChiave, _
                                    enum_TipoNodo.Utente, _
                                    , , , , , , , , , , , , , )

        Dim Radice As AjaxTreeNodeJsonObject

        Radice = New AjaxTreeNodeJsonObject(xChiave, AgroPrefix_Utente & Testo, "", "", "#", Livello_Impresa)


        Radice.icon = PathRoot + Albero.RitornaPathImg(enum_TipoNodo.Utente, "")
        Radice.state = "open"


        ''''''''''''
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





        'identifico le PIVA che posso vedere con l'utente selezionato
        Dim listaPiva As String = GetPive()
        If listaPiva.Length > 0 Then
            listaPiva = listaPiva + " , ''"
        End If
        If _Cod_Sementi <> "" Then
            DT_Gerarchie = objGerImprese.LeggixAlberoImpresexSementi(listaPiva, _Cod_Sementi, validita_inizio, validita_fine, HttpContext.Current.Session("ASG_objParametri_Server"))
        Else
            DT_Gerarchie = objGerImprese.LeggixGerarchiaAlberoImprese(listaPiva, validita_inizio, validita_fine, HttpContext.Current.Session("ASG_objParametri_Server"))
        End If


        Dim pivaPadre As String = ""
        If CType(HttpContext.Current.Session("ASG_objParametri_Server"), AgronicaCoreDataProvider.AgronicaCoreParametri).UtenteUsername <> _
            CType(HttpContext.Current.Session("ASG_objParametri_Server"), AgronicaCoreDataProvider.AgronicaCoreParametri).SuperUserUsername Then
            pivaPadre = CType(HttpContext.Current.Session("ASG_objParametri_Server"), AgronicaCoreDataProvider.AgronicaCoreParametri).UsernameOperazione
        End If

        RestituisciNodoImpresa(results, DT_Gerarchie, pivaPadre, PathRoot)

        ser.MaxJsonLength = 50000000
        Return ser.Serialize(results)
    End Function



    Private Function GetPive()
        Dim Dt_Imprese As DataTable
        Dim i As Integer
        Dim ClassJoin As New JoinFiltrone
        Dim classFiltrone As New AgronicaCoreUtility.Filtrone
        Dim xFiltroAggiuntivo As String = ""

        'Per il momento imposto sempre a true perchè può capitare
        'che il filtro associato all'utente vada a controllare il campo Padre e/o Foglia
        '(di GerarchiaImprese), ma non essendo specificata la tabella GerarchiaImprese
        'prima del nome del campo, la funzione ImpostaVariabiliJOIN_xFiltroUtente non la trova e non imposta il join
        ClassJoin.bGerarchiaImprese = True

        classFiltrone.ImpostaVariabiliJOIN_xFiltroUtente(xFiltroAggiuntivo, ClassJoin)
        Dt_Imprese = classFiltrone.CreaDTFiltrone(HttpContext.Current.Session("ASG_objParametri_Server"),
                                                  xFiltroAggiuntivo, enum_TipoSelect_FiltroneSuperNova.Imprese, "", ClassJoin)

        Dim listaPiva As String = ""
        For i = 0 To Dt_Imprese.Rows.Count - 1
            If listaPiva = "" Then
                listaPiva = " '" & Dt_Imprese.Rows(i).Item("Piva") & "' "
            Else
                listaPiva = listaPiva + ", '" & Dt_Imprese.Rows(i).Item("Piva") & "' "
            End If
        Next
        Return listaPiva
    End Function


    Private Sub RestituisciNodoImpresa(ByRef results As List(Of AjaxTreeNodeJsonObject), _
                                            ByRef DT_Gerarchie As DataTable, ByRef PivaPadre As String, _
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
                        avanti = False
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
                Call Albero.ChiaveAlbero_Codifica_x_json(xChiave, _
                                            TipoImpresa, _
                                            Piva:=xPiva, _
                                            PivaPadre:=PivaPadre _
                                        )

                Dim Livello2 As New List(Of AjaxTreeNodeJsonObject)
                If Foglia = True Then
                    Livello2 = Nothing
                Else
                    RestituisciNodoImpresa(Livello2, DT_Gerarchie, xPiva, PathRoot)
                End If


                Dim Impresa As New AjaxTreeNodeJsonObject
                If IsNothing(Livello2) Then
                    Impresa = New AjaxTreeNodeJsonObject(xChiave,
                                                   xRag_Soc, "jstree-no-checkboxes", "", "#", False)
                Else
                    Impresa = New AjaxTreeNodeJsonObject(xChiave,
                                                   xRag_Soc, "jstree-no-checkboxes", "", "#", Livello2)
                End If



                If PivaPadre = "" Then
                    Impresa.state = "open"
                Else
                    Impresa.state = "close"
                End If

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
        'js.AppendLine("function OnGetNodesGerarchia(n){		")
        'js.AppendLine("     var obj = { ")
        'js.AppendLine("         id: n.attr ? n.attr('id') : '0'")
        'js.AppendLine("         , PathRoot : '" + path + "'")
        'js.AppendLine("			}")
        'js.AppendLine("			return Sys.Serialization.JavaScriptSerializer.serialize(obj); ")
        'js.AppendLine("     }")


        js.AppendLine("function OnGetNodesGerarchia(n){		")
        js.AppendLine("     var port = location.port; ")
        js.AppendLine("     if (port != '' ) { port = ':'+ port;} ")

        js.AppendLine("     var obj = { ")
        js.AppendLine("         id: n.attr ? n.attr('id') : '0'")
        js.AppendLine("         , PathRoot : 'http://' + location.hostname + port + '/" & path & "'")
        js.AppendLine("			}")
        js.AppendLine("			return Sys.Serialization.JavaScriptSerializer.serialize(obj); ")
        js.AppendLine("     }")




        js.AppendLine("function OnNodesRetrievedSuccessGerarchia(data,textstatus,xhr){ ")
        js.AppendLine("     return Sys.Serialization.JavaScriptSerializer.deserialize(data.d); ")
        js.AppendLine("} ")

        js.AppendLine(" function OnNodesRetrievedErrorGerarchia(xhr,textstatus,errorThrown){")
        js.AppendLine("     alert('ERRORE!!!' +errorThrown); ")
        js.AppendLine(" } ")


        js.AppendLine("$(document).ready(function () { ")

        js.AppendLine("    $('#" + IDDiv + "').jstree({ ")
        js.AppendLine("        core : { ")
        js.AppendLine("             'initially_open' : [ " & IdNodiAperti & "] ")
        js.AppendLine("        }, ")
        js.AppendLine("        ui : { ")
        js.AppendLine("             'select_limit' : 1,'initially_select' :[ '' ] ")
        js.AppendLine("        }, ")

        js.AppendLine("        themes : { ")
        js.AppendLine("        'theme' : 'apple' ")
        js.AppendLine("        }, ")

        js.AppendLine("        plugins: ['themes', 'json_data',  'ui',  'hotkeys'],")
        js.AppendLine("        json_data: { ")
        js.AppendLine("		            ajax: {")
        js.AppendLine("                     url:    GetNameofPageGerarchia() +'/GetNodesGerarchia',")
        js.AppendLine("                     async: true,")
        js.AppendLine("                 contentType:  'application/json; charset=utf-8',")
        js.AppendLine("                 dataType:  'json',")
        js.AppendLine("                 type:   'POST',")
        js.AppendLine("                 data: function (n) { return OnGetNodesGerarchia(n); },")
        js.AppendLine("                 success: function (data, textstatus, xhr) {")
        js.AppendLine("                     return OnNodesRetrievedSuccessGerarchia(data, textstatus, xhr)")
        js.AppendLine("		            },")
        js.AppendLine("                 error: function (xhr, textstatus, errorThrown) {")
        js.AppendLine("                     OnNodesRetrievedErrorGerarchia(xhr, textstatus, errorThrown)")
        js.AppendLine("                 }")
        js.AppendLine("             }")
        js.AppendLine("         }")
        js.AppendLine("     });")


        'gestione del dbClick
        js.AppendLine("     $(document).on('click', '#" + IDDiv + " ul li a', function () { ")
        'js.AppendLine("     $('#" + IDDiv + " ul li a').live('click', function(){ ")
        'controllo se il nodo è aperto o chiuso
        js.AppendLine("         if($(this).parent().attr('class')=='jstree-open') { ")
        js.AppendLine("         $('#" + IDDiv + "').jstree('close_node', this); } else {")
        js.AppendLine("         $('#" + IDDiv + "').jstree('open_node', this); } ")
        js.AppendLine("     });")



        js.AppendLine(" });")





        js.AppendLine("function GetNameofPageGerarchia() {")
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
        StrSelect.AppendLine("$(document).ready(function () { ")

        'StrSelect.AppendLine("  $('#" + ID + " ul').delegate('li', 'click', function(){ ")
        StrSelect.AppendLine("  $('#" + ID + " ul').on('click','li a', function(){ ")

        StrSelect.AppendLine("      $('#" & Hidden.ClientID & "').val($(this).parent('li').attr('id')); ")

        StrSelect.AppendLine("  var nodi='';")
        StrSelect.AppendLine("  nodi =$(this).parent('li').attr('id'); ")
        'da gestire la deselezione perchè viene duplicato un nodo Se il nodo è doppio è da rimuovere
        StrSelect.AppendLine("  nodi =$('#" & Hidden.ClientID & "').val(nodi); ")

        StrSelect.AppendLine("  }); ")

        StrSelect.AppendLine("});")

        StrSelect.AppendLine("</script>")
        Return StrSelect.ToString
    End Function


End Class
