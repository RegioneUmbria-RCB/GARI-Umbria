Imports System.Web.Services
Imports System.Text
Imports System.Data
Imports System.Web
Imports System.Web.Script.Serialization
Imports System.Web.Script.Services

Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreMetaSchemaDAL
Imports System.ComponentModel
Imports System.IO



''' <summary>
''' Per utilizzare questo controllo è necessario includere all'iterno della pagina i seguenti script 
''' jquery.cookie.js
''' jquery.hotkeys.js
''' jquery.jstree.js
''' RICORDARE DI AGGIUNGERE LA FUNZIONE JS
''' function DoPostBack_Combo($_combo,valoreOpt) {
'''        if($_combo.attr("id").endsWith("ComboFertilizzanti")){
'''            //salvo nel campo nascosto il nome della funzione da richiamare
'''            $("#<%=fooName_PostedBack.ClientID %>").val("ComboFertilizzanti_IndexChanged");
'''            //scatena il postback simulato il click del btn nascosto
'''            $("#<%=btnDummyPostback.ClientID %>").click();
'''        }
''' </summary>
''' <remarks></remarks>

<DefaultProperty("Text"), ToolboxData("<{0}:ComboMisuraXAvversitaInputData runat=server></{0}:ComboMisuraXAvversitaInputData>")> Public Class ComboMisuraXAvversitaInputData
    Inherits System.Web.UI.WebControls.WebControl





    Public Sub New()




    End Sub

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)

        MyBase.OnInit(e)

    End Sub

    'Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)

    '    MyBase.OnPreRender(e)
    'End Sub


    Private Shared Sub getResource(ByVal risorsa As String, ByRef str_e As String)
        Try
            'Dim readStream  New FileStream("c:\testBinary.dat", FileMode.Open)
            Dim readBinary As New BinaryReader(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(risorsa))

            Dim length As Integer = readBinary.BaseStream.Length
            Dim pos As Integer = 0
            str_e = readBinary.ReadChars(1)
            str_e = ""
            While pos < length
                ' Read the integer.
                str_e = str_e & readBinary.ReadChars(1)

                pos += 1
            End While

            readBinary.Close()
        Catch ex As Exception
        End Try
    End Sub

    Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)

        Dim htmlText As New StringBuilder

        Dim ComponentePrincipale As String = ""
        getResource("AgronicaControlli_2010.htmlComboMisuraXAvversitaInputData.htm", ComponentePrincipale)

        htmlText.Append(ComponentePrincipale)

        writer.Write(htmlText.ToString)

        MyBase.Render(writer)
    End Sub


    Private _MxAV_Cod As Integer
    Public Property MxAV_Cod As Integer
        Get
            Return _MxAV_Cod
        End Get
        Set(ByVal value As Integer)
            _MxAV_Cod = value
        End Set
    End Property





    Public Shared Function CaricaComboMisuraXAvversitaInputData(ByVal MxAV_Cod As String, ByVal av_cod As String, ByVal udm_cod As String, ByVal veg_cod As String) As String

        Dim Dt As DataTable

        Dim oLeggi As New AgronicaCoreMetaSchemaDAL.MisuraxAvversita_Anagrafiche_R

        Dt = oLeggi.Leggi( _
             MxAV_Cod, _
             av_cod, _
             udm_cod, _
             veg_cod, _
             "", _
             "", _
            HttpContext.Current.Session("ASG_objParametri_Server") _
        )



        Dim rval As String = ""

        If Not IsNothing(Dt) Then


            Dt.TableName = "mav"

            For Each dd In Dt.Rows

                rval &= "<option value='" & dd("anag_valore") & "'>" & dd("anag_des") & "</option>"

            Next


        End If

        Dt = Nothing


        Return rval

    End Function


End Class

