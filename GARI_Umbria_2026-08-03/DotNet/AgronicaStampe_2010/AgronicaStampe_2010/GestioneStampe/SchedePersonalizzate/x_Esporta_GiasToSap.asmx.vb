Imports System.ComponentModel
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility
Imports AgronicaCoreUtility.CaricaListControl
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
Public Class x_Esporta_GiasToSap
    Inherits System.Web.Services.WebService

    <Script.Services.ScriptMethod()> <WebMethod(EnableSession:=True)>
    Public Function GetCooperative() As String
        Dim objParametri_server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim grImpreseObj As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R
        Dim objanagrafeDAL As New AgronicaCoreAnagrafeDAL.Imprese_Read

        Dim dt As DataTable = grImpreseObj.CaricaBoxList_Cooperativa(objParametri_server)

        dt.DefaultView.Sort = "rag_soc asc"
        dt = dt.DefaultView.ToTable

        Dim c As New Integer
        c = 0

        For Each row In dt.Rows
            Dim str As String
            str = objanagrafeDAL.RagSoc_from_Piva(row("figlio").ToString, objParametri_server)
            dt.Rows(c).Item("rag_soc") = str
            c += 1
        Next

        Dim lista As New List(Of String)
        For Each row As DataRow In dt.Rows
            lista.Add("{""des"":""" & jSon.Escape(row.Item("rag_soc")) & """, ""val"":""" & row.Item("figlio") & """}")
        Next

        Dim strRisp As String = "[" & String.Join(",", lista) & "]"

        Return strRisp

    End Function

    '###############################################################################

    <Script.Services.ScriptMethod()> <WebMethod(EnableSession:=True)>
    Public Function GetPianiSemina() As String
        Dim objParametri_server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim grImpreseObj As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R
        Dim objanagrafeDAL As New AgronicaCoreAnagrafeDAL.Imprese_Read

        Dim dt As DataTable = grImpreseObj.CaricaList_PianiSemina(objParametri_server)

        Dim lista As New List(Of String)
        For Each row As DataRow In dt.Rows
            If row.Item("val_cod") = "" Then

            Else
                lista.Add("{""des"":""" & jSon.Escape(row.Item("val_cod")) & """, ""val"":""" & row.Item("val_cod") & """}")
            End If

        Next

        Dim strRisp As String = "[" & String.Join(",", lista) & "]"

        Return strRisp

    End Function

    '###############################################################################

    <Script.Services.ScriptMethod()> <WebMethod(EnableSession:=True)>
    Public Function GetSpecieVegetali(ByVal Gru_Cod As String) As String

        Dim objParametri_server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Dim str_filtoro As String = If(Gru_Cod = "0", "", " SpecieVegetali.Gru_Cod in (" & Gru_Cod & ") ")

        Dim cblSpecie As New DropDownList
        CaricaCheckBoxList_SpecieVegetale_Optimize(cblSpecie, 0, True, str_filtoro, "", 0, 0, objParametri_server, objParametri_Utenti)

        Dim lista As New List(Of String)

        'Aggiunta di una row vuota
        lista.Add("{""codice"":""" & "" & """, ""descrizione"":""" & "" & """}")

        For Each item As ListItem In cblSpecie.Items
            lista.Add("{""codice"":""" & item.Value & """, ""descrizione"":""" & jSon.Escape(item.Text) & """}")
        Next

        Dim strRisp As String = "[" & String.Join(",", lista) & "]"


        Return strRisp

    End Function

    '###############################################################################

    <Script.Services.ScriptMethod()> <WebMethod(EnableSession:=True)>
    Public Function GetCultivar(ByVal Veg_Cod As String) As String

        Dim objParametri_server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Dim CBL_Cultivar As New DropDownList
        Dim strFiltro As String = " SpecieVegetali.Veg_cod IN (" & Veg_Cod & ") "

        Cultivar(CBL_Cultivar, False, "", "", 0, 0, "", True, 0, 0, strFiltro, "", objParametri_server, objParametri_Utenti)

        Dim lista As New List(Of String)

        Dim objMatrice As New AgronicaCoreMetaSchemaDAL.Cultivar_R
        Dim Dt As DataTable = objMatrice.GestioneFiltroUtente_Leggi(0, 0, "", AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                                    strFiltro, "", objParametri_Utenti)

        For Each dr As DataRow In Dt.Rows
            lista.Add("{""codice"":""" & dr.Item("Cul_Cod") & """, ""descrizione"":""" & jSon.Escape(dr.Item("Cul_Des")) & """, ""veg_des"":""" & jSon.Escape(dr.Item("Veg_Des")) & """}")
        Next

        Dim strRisp As String = "[" & String.Join(",", lista) & "]"

        Return strRisp

    End Function

    '###############################################################################

    <Script.Services.ScriptMethod()> <WebMethod(EnableSession:=True)>
    Public Function stamp(ByVal Cooperativa As Object,
                          ByVal PianoSemina As Object,
                          ByVal SpecieVegetali As Object,
                          ByVal Varieta As Object,
                          ByVal DataInizio As Object,
                          ByVal DataFine As Object,
                          ByVal Intervallo As Object) As String

        Dim objstamp As New AgronicaStampe_2010.Esporta_GiasToSap
        Dim str As String = objstamp.stampa(Cooperativa, PianoSemina, SpecieVegetali, Varieta, DataInizio, DataFine, Intervallo)
        Return str

    End Function

End Class