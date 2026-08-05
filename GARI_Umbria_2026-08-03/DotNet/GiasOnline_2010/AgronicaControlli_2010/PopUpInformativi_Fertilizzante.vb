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


''' <summary>
''' Per utilizzare questo controllo è necessario includere all'iterno della pagina i seguenti script 
''' jquery.cookie.js
''' </summary>
''' <remarks></remarks>

<DefaultProperty("Text"), ToolboxData("<{0}:PopUpInformativi_Fertilizzante runat=server></{0}:PopUpInformativi_Fertilizzante>")> Public Class PopUpInformativi_Fertilizzante
    Inherits System.Web.UI.WebControls.WebControl

    'Public Class ComboOperazioni
    '    Inherits ScriptControl
    '    Implements IScriptControl


    'Private _Fer_Cod As Integer
    Private Literal As LiteralControl

    Public Sub New()
        '_Fer_Cod = 0

    End Sub
 

#Region "Proprietà"



    'Public Property Fer_Cod() As Integer
    '    Get
    '        Return _Fer_Cod
    '    End Get
    '    Set(ByVal value As Integer)
    '        _Fer_Cod = value
    '    End Set
    'End Property


#End Region




    'Public Function GetJS()
    '    Dim StrSelect As New StringBuilder

    '    StrSelect.AppendLine("<script>")



    '    StrSelect.AppendLine("$(document).ready(function () { ")

    '    ''dialogPupUpInformativiFertilizzante
    '    StrSelect.AppendLine(getScriptDialog())

    '    StrSelect.AppendLine("      });")
    '    StrSelect.AppendLine("</script>")
    '    Return StrSelect.ToString
    'End Function

    'Private Function getScriptDialog()
    '    Dim StrSelect As New StringBuilder
    '    StrSelect.AppendLine("      $('.dialogPupUpInformativiFertilizzante').dialog({ ")
    '    StrSelect.AppendLine("              autoOpen: false,")
    '    StrSelect.AppendLine("              width: 500,")
    '    StrSelect.AppendLine("              modal: true,")
    '    StrSelect.AppendLine("              buttons: {")
    '    StrSelect.AppendLine("                  'Chiudi': function () {")
    '    StrSelect.AppendLine("                      $(this).dialog('close');")
    '    StrSelect.AppendLine("                  }")
    '    StrSelect.AppendLine("              }")
    '    StrSelect.AppendLine("      });")
    '    Return StrSelect.ToString
    'End Function

    'public Function GetHTML()


    '    Dim StrSelect As New StringBuilder
    '    StrSelect.AppendLine("<!--DIALOG dialogPupUpInformativiFertilizzante-->")
    '    StrSelect.AppendLine("<span id='dialogPupUpInformativiFertilizzante' class='dialogPupUpInformativiFertilizzante'></span>")

    '    Return StrSelect.ToString
    'End Function

    'Protected Overrides Sub OnInit(ByVal e As System.EventArgs)

    '    Literal = New LiteralControl
    '    Literal.ID = Me.ClientID & "DialogPopUpInformativi_Fertilizzanti"

    '    'Literal.Text = GetHTML()
    '    Literal.Text = GetJS() + GetHTML()

    '    Me.Controls.Add(Literal)
    '    MyBase.OnInit(e)
    'End Sub

    'Public Function getInizializza()
    '    'Dim StrSelect As New StringBuilder
    '    'StrSelect.AppendLine(GetJS() + GetHTML())
    '    'Return StrSelect.ToString
    'End Function

    'Public Function getInfoFertilizzante(ByVal Fer_Cod As Integer)
    '    Dim StrSelect As New StringBuilder

    '    Dim rnd As New Random
    '    Dim numero As Integer
    '    numero = rnd.Next()
    '    Dim NomeFunzione As String
    '    NomeFunzione = "Mostra_PopUP_" & numero.ToString & "()"


    '    'StrSelect.Append(GetHTML())



    '    'Funzione per il dialogPupUpInformativiFertilizzante
    '    'StrSelect.Append("function " & NomeFunzione & " {")
    '    'StrSelect.Append("     $('.dialogPupUpInformativiFertilizzante').get(0).innerHTML ='<table class=""centerTab""><tr><td>' + ")
    '    'StrSelect.Append("          " & getStringaInformazioniFertilizzanti(Fer_Cod) & "  + ")
    '    'StrSelect.Append("          ")
    '    'StrSelect.Append("'     </td>' +")
    '    'StrSelect.Append("'     </tr></table>';")
    '    ''StrSelect.AppendLine(getScriptDialog())
    '    'StrSelect.Append("      $('.dialogPupUpInformativiFertilizzante').dialog('open');")
    '    'StrSelect.Append("} ")


    '    StrSelect.Append("$(document).ready(function () { ")



    '    'StrSelect.AppendLine("      $('.dialogPupUpInformativiFertilizzante').dialog({ ")
    '    'StrSelect.AppendLine("              autoOpen: false,")
    '    'StrSelect.AppendLine("              width: 500,")
    '    'StrSelect.AppendLine("              modal: true,")
    '    'StrSelect.AppendLine("              buttons: {")
    '    'StrSelect.AppendLine("                  'Chiudi': function () {")
    '    'StrSelect.AppendLine("                      $('.dialogPupUpInformativiFertilizzante').dialog('close');")
    '    'StrSelect.AppendLine("                  }")
    '    'StrSelect.AppendLine("              }")
    '    'StrSelect.AppendLine("      });")
    '    StrSelect.Append("      $('.dialogPupUpInformativiFertilizzante').dialog('open');")
    '    'StrSelect.Append("      " & NomeFunzione & ";")
    '    StrSelect.Append("});")

    '    Return StrSelect.ToString
    'End Function


    Public Function getStringaInformazioniFertilizzanti(ByVal Fer_Cod As Integer)
        Dim Str As New StringBuilder


        Dim DtTemp As DataTable
        Dim objFertilizzanti As New AgronicaCoreMetaSchemaDAL.Fertilizzanti_R

        'Recupero le informazioni		
        DtTemp = objFertilizzanti.Leggi(CInt(Fer_Cod), _
                                        "", AGRODATAINIZIO, AGRODATAFINE, _
                                        AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                        "", "", HttpContext.Current.Session("ASG_objParametri_Server"))
        Dim i As Integer
        '<table width='100%'>
        '   <tr><td colspan='2'></td> </tr>
        '   <tr><td style='width:50%'></td><td style='width:50%'></td></tr>
        '</table>
        Dim colSpan_unaCella As Integer = 2

        Str.Append("<table width=""100%"">")




        'nome fertilizzante
        Str.Append("<tr><td colspan=""" & colSpan_unaCella & """><b>" & DtTemp.Rows(0).Item("Fer_Des") & "</b></td></tr>")

        'Denominazione
        If Not IsDBNull(DtTemp.Rows(0).Item("Denominazione")) Then
            Str.Append("<tr><td colspan=""" & colSpan_unaCella & """><i>" & DtTemp.Rows(0).Item("Denominazione") & "</i></td></tr>")
        End If

        Dim str_ditta As String = objFertilizzanti.DittaDes_from_FerCod(CInt(Fer_Cod), Nothing, HttpContext.Current.Session("ASG_objParametri_Server"))
        If str_ditta.Trim() <> "" Then
            Str.Append("<tr><td colspan=""" & colSpan_unaCella & """><b>Ditta</b>: <i>" & _
                       str_ditta & _
                       "</i></td></tr>")
        End If

        Str.Append("<tr>")
        Str.Append("<td><b>N</b>: <i>" & DtTemp.Rows(0).Item("N") & "</i></td>")
        Str.Append("<td><b>P205</b>: <i>" & DtTemp.Rows(0).Item("P2O5") & "</i></td>")
        Str.Append("</tr>")


        Str.Append("<tr>")
        Str.Append("<td><b>K2O</b>: <i>" & DtTemp.Rows(0).Item("K2O") & "</i></td>")
        Str.Append("<td><b>MgO</b>: <i>" & DtTemp.Rows(0).Item("MgO") & "</i></td>")
        Str.Append("</tr>")



        Str.Append("<tr>")
        Str.Append("<td><b>SO3</b>: <i>" & DtTemp.Rows(0).Item("SO3") & "</i></td>")
        Str.Append("<td><b>Mn</b>: <i>" & DtTemp.Rows(0).Item("Mn") & "</i></td>")
        Str.Append("</tr>")

        Str.Append("<tr>")
        Str.Append("<td><b>Cl</b>: <i>" & DtTemp.Rows(0).Item("Cl") & "</i></td>")
        Str.Append("<td><b>Fe</b>: <i>" & DtTemp.Rows(0).Item("Fe") & "</i></td>")
        Str.Append("</tr>")

        Str.Append("<tr>")
        Str.Append("<td><b>S_O</b>: <i>" & DtTemp.Rows(0).Item("S_O") & "</i></td>")
        Str.Append("<td><b>Cu</b>: <i>" & DtTemp.Rows(0).Item("Cu") & "</i></td>")
        Str.Append("</tr>")

        Str.Append("<tr>")
        Str.Append("<td><b>Mo</b>: <i>" & DtTemp.Rows(0).Item("Mo") & "</i></td>")
        Str.Append("<td><b>Zn</b>: <i>" & DtTemp.Rows(0).Item("Zn") & "</i></td>")
        Str.Append("</tr>")

        Str.Append("<tr>")
        Str.Append("<td><b>CaO</b>: <i>" & DtTemp.Rows(0).Item("CaO") & "</i></td>")
        Str.Append("<td><b>Na2O</b>:<i>" & DtTemp.Rows(0).Item("Na2O") & "</i></td>")
        Str.Append("</tr>")

        Str.Append("<tr>")
        Str.Append("<td><b>B</b>: <i>" & DtTemp.Rows(0).Item("B") & "</i></td>")
        Str.Append("<td><b>SS</b>: <i>" & DtTemp.Rows(0).Item("SS") & "</i></td>")
        Str.Append("</tr>")



        If Not IsDBNull(DtTemp.Rows(0).Item("Precauzione")) AndAlso DtTemp.Rows(0).Item("Precauzione").ToString.Trim <> "" Then
            Str.Append("<tr><td colspan=""" & colSpan_unaCella & """><b>Precauzione</b>: <i>" & DtTemp.Rows(0).Item("Precauzione") & "</i></td></tr>")
        End If

        If Not IsDBNull(DtTemp.Rows(0).Item("Rapporti")) AndAlso DtTemp.Rows(0).Item("Rapporti").ToString.Trim <> "" Then
            Str.Append("<tr><td colspan=""" & colSpan_unaCella & """><b>Rapporti</b>: <i>" & DtTemp.Rows(0).Item("Rapporti") & "</i></td></tr>")
        End If

        If Not IsDBNull(DtTemp.Rows(0).Item("Prezzo")) AndAlso DtTemp.Rows(0).Item("Prezzo").ToString.Trim <> "" Then
            Str.Append("<tr><td colspan=""" & colSpan_unaCella & """><b>Prezzo</b>: <i>" & DtTemp.Rows(0).Item("Prezzo") & "</i></td></tr>")
        End If


        Dim str_class As String = objFertilizzanti.Classificazioni_Fertilizzante_from_FerCod(CInt(Fer_Cod), _
                                                                              AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                                                              "", "", _
                                                                              HttpContext.Current.Session("ASG_objParametri_Server"))
        If str_class.Trim <> "" Then
            Str.Append("<tr><td colspan=""" & colSpan_unaCella & """><b>Classificazioni</b>: <i>" & _
                                                                str_class & _
                                                                "</i></td></tr>")
        End If

        Dim str_formulazioni As String = objFertilizzanti.FormulazioniFertilizzanti_FORM_FER_DES(CInt(Fer_Cod), _
                                                                              AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                                                              "", "", _
                                                                              HttpContext.Current.Session("ASG_objParametri_Server"))
        Str.Append("<tr><td colspan=""" & colSpan_unaCella & """><b>Formulazioni</b>: <i>" & _
                                                           str_formulazioni & _
                                                           "</i></td></tr>")




        Dim objRxF As New AgronicaCoreMetaSchemaDAL.RegolamentixFertilizza_R
        'Leggo le imprese associate al profilo selezionato			
        DtTemp = objRxF.Leggi(0, _
                              CInt(Fer_Cod), _
                              AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta, _
                              "", "", HttpContext.Current.Session("ASG_objParametri_Server"))

        'Se il recordset non è chiuso allora ...	
        If Not DtTemp Is Nothing AndAlso DtTemp.Rows.Count > 0 Then
            'Inserisco i record trovati
            Dim str_app As String = ""
            For i = 0 To DtTemp.Rows.Count - 1
                str_app = str_app + DtTemp.Rows(i).Item("Reg_Des") & ","
            Next
            If str_app <> "" Then
                str_app = Left(str_app, str_app.Length - 1)
            End If
            Str.Append("<tr><td colspan=""" & colSpan_unaCella & """><b>Regolamenti</b>: <i>" & _
                                            str_app & _
                                            "</i></td></tr>")

        End If

        'Elimino il recordset
        DtTemp = Nothing




        '------------------------------------------
        '-----  TIPOLOGIE
        '------------------------------------------

        Dim ObjTipologie As New AgronicaCoreMetaSchemaDAL.FertilizzantixTipologie_R

        DtTemp = ObjTipologie.Leggi(0, 0, CInt(Fer_Cod), _
                                AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                "", "", HttpContext.Current.Session("ASG_objParametri_Server"))

        ObjTipologie = Nothing
        'Se il recordset non è chiuso allora ...	
        If Not DtTemp Is Nothing AndAlso DtTemp.Rows.Count > 0 Then

            Dim str_app As String = ""
            'Inserisco i record trovati
            For i = 0 To DtTemp.Rows.Count - 1
                str_app = str_app + DtTemp.Rows(i).Item("Tp_Des") & ","
            Next
            If str_app <> "" Then
                str_app = Left(str_app, str_app.Length - 1)
            End If
            Str.Append("<tr><td colspan=""" & colSpan_unaCella & """><b>Tipologie</b>: <i>" & _
                                                          str_app & _
                                                          "</i></td></tr>")

        End If





        '------------------------------------------
        '-----  TIPOLOGIE CE
        '------------------------------------------

        Dim ObjTipologieCE As New AgronicaCoreMetaSchemaDAL.FertilizzantixTipoCE_R

        DtTemp = ObjTipologieCE.Leggi(0, CInt(Fer_Cod), _
                                      AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                      "", "", HttpContext.Current.Session("ASG_objParametri_Server"))



        'Se il recordset non è chiuso allora ...	
        If Not DtTemp Is Nothing AndAlso DtTemp.Rows.Count > 0 Then

            Dim str_app As String = ""
            'Inserisco i record trovati
            For i = 0 To DtTemp.Rows.Count - 1
                str_app = str_app + DtTemp.Rows(i).Item("TipCE_Des") & ","
            Next
            If str_app <> "" Then
                str_app = Left(str_app, str_app.Length - 1)
            End If
            Str.Append("<tr><td colspan=""" & colSpan_unaCella & """><b>Tipologie CE</b>: <i>" & _
                                                         str_app & _
                                                          "</i></td></tr>")

        End If



        'chiudo la tabella
        Str.Append("</table>")

        Return Str.ToString
    End Function


End Class
