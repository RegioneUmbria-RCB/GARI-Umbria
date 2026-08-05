Imports System.ComponentModel
Imports System.Text
''' <summary>
''' da ricordarsi di implementare il metodo DoPostBack_Eliminazione(str) all'interno del javascript
''' </summary>
''' <remarks></remarks>
<DefaultProperty("Text"), ToolboxData("<{0}:DialogAvvertimenti runat=server></{0}:DialogAvvertimenti>")>
Public Class DialogAvvertimenti
    Inherits System.Web.UI.WebControls.WebControl

    Private _PathImmagini As String = ""
    Private Literal As LiteralControl

    Public Property PathImmagini() As String
        Get
            Return _PathImmagini
        End Get
        Set(ByVal value As String)
            If value.EndsWith("/") Then
                value = value.Substring(0, value.Length - 1)
            End If
            _PathImmagini = value
        End Set
    End Property




    Private Function GetJS()
        Dim StrSelect As New StringBuilder

        If _PathImmagini.Length = 0 Then
            _PathImmagini = "../AB_Immagini/Messaggi"
        End If
        StrSelect.AppendLine("<script>")
        'Funzione per il messaggio di buon fine
        StrSelect.AppendLine("function ScritturaOK(str) {")
        StrSelect.AppendLine("     $('.dialogAvvertimentoControlli').get(0).innerHTML = '<table class=""centerTab""><tr><td>' + ")
        StrSelect.AppendLine("          str + ")
        StrSelect.AppendLine("          '<br>' +    ")
        StrSelect.AppendLine("'     </td>' +")
        StrSelect.AppendLine("'     <td><img alt=""Errore"" src=""" & _PathImmagini & "/ValidazioneSI.ico"" /></td></tr></table>';")
        StrSelect.AppendLine("      $('.dialogAvvertimentoControlli').dialog();")
        StrSelect.AppendLine("      $('.dialogAvvertimentoControlli').dialog('open');")
        StrSelect.AppendLine("} ")


        'Funzione per il messaggio di Errore
        StrSelect.AppendLine("function MessaggioErrore(str) {")
        StrSelect.AppendLine("      $('.dialogErroreControlli').html('<table class=""centerTab""><tr><td>' + str + '<br>' +")
        StrSelect.AppendLine("      '</td>' +")
        StrSelect.AppendLine("      '<td><img alt=""Errore"" src=""" & _PathImmagini & "/IconError.jpg"" /></td></tr></table>');")
        StrSelect.AppendLine("      $('.dialogErroreControlli').dialog();")
        StrSelect.AppendLine("      $('.dialogErroreControlli').dialog('open');")
        '        setTimeout(function () { $('.dialogErrore').dialog('open'); }, 500);
        StrSelect.AppendLine("  } ")


        'Funzione Eliminazione
        StrSelect.AppendLine("function ConfermaEliminaControlli(str) { ")
        StrSelect.AppendLine("      $('.variabileDeleteControlli').html(str);")
        StrSelect.AppendLine("      $('.dialogAvvertimentoDeleteControlli').dialog('open');")
        StrSelect.AppendLine("}")

        StrSelect.AppendLine("function ConfermaEliminaControlliBis(str,msg) { ")
        StrSelect.AppendLine("      $('.dialogAvvertimentoDeleteControlli').html('<table class=""centerTab""><tr><td>' + msg + '<br>' +")
        StrSelect.AppendLine("      '</td>' +")
        StrSelect.AppendLine("      '<td></td></tr></table>');")
        StrSelect.AppendLine("      $('.variabileDeleteControlli').html(str);")
        StrSelect.AppendLine("      $('.dialogAvvertimentoDeleteControlli').dialog('open');")
        StrSelect.AppendLine("}")


        'ConfermaControlliSiNo
        StrSelect.AppendLine("function ConfermaControlliSiNo(messaggio, str) {")
        StrSelect.AppendLine("     $('.dialogControlliSiNo').get(0).innerHTML = '<table class=""centerTab""><tr><td>' + ")
        StrSelect.AppendLine("          messaggio + ")
        StrSelect.AppendLine("          '<br>' +    ")
        StrSelect.AppendLine("'     </td>' +")
        StrSelect.AppendLine("'     <td></td></tr></table>';")
        StrSelect.AppendLine("      $('.variabileSiNo').html(str);")
        StrSelect.AppendLine("      $('.dialogControlliSiNo').dialog();")
        StrSelect.AppendLine("      $('.dialogControlliSiNo').dialog('open');")
        StrSelect.AppendLine("} ")




        'ConfermaControlliSiNo_Bis
        StrSelect.AppendLine("function ConfermaControlliSiNo_Bis(messaggio, str) {")
        StrSelect.AppendLine("     $('.dialogControlliSiNo_Bis').get(0).innerHTML = '<table class=""centerTab""><tr><td>' + ")
        StrSelect.AppendLine("          messaggio + ")
        StrSelect.AppendLine("          '<br>' +    ")
        StrSelect.AppendLine("'     </td>' +")
        StrSelect.AppendLine("'     <td></td></tr></table>';")
        StrSelect.AppendLine("      $('.variabileSiNo_Bis').html(str);")
        StrSelect.AppendLine("      $('.dialogControlliSiNo_Bis').dialog();")
        StrSelect.AppendLine("      $('.dialogControlliSiNo_Bis').dialog('open');")
        StrSelect.AppendLine("} ")




        'Al Document Ready
        StrSelect.AppendLine("$(document).ready(function () { ")

        'ERRORE
        StrSelect.AppendLine("      $('.dialogErroreControlli').dialog({ ")
        StrSelect.AppendLine("              autoOpen: false,")
        StrSelect.AppendLine("              width: 300,")
        StrSelect.AppendLine("              modal: true,")
        StrSelect.AppendLine("              buttons: {")
        StrSelect.AppendLine("                  'Chiudi': function () {")
        StrSelect.AppendLine("                      $(this).dialog('close');")
        StrSelect.AppendLine("                  }")
        StrSelect.AppendLine("              }")
        StrSelect.AppendLine("      });")


        'Avvertimento
        StrSelect.AppendLine("      $('.dialogAvvertimentoControlli').dialog({ ")
        StrSelect.AppendLine("              autoOpen: false,")
        StrSelect.AppendLine("              width: 300,")
        StrSelect.AppendLine("              modal: true,")
        StrSelect.AppendLine("              buttons: {")
        StrSelect.AppendLine("                  'Chiudi': function () {")
        StrSelect.AppendLine("                      $(this).dialog('close');")
        StrSelect.AppendLine("                  }")
        StrSelect.AppendLine("              }")
        StrSelect.AppendLine("      });")


        'Conferma
        StrSelect.AppendLine("   $('.dialogAvvertimentoDeleteControlli').dialog({ ")
        StrSelect.AppendLine("          autoOpen: false, ")
        StrSelect.AppendLine("          width: 300, ")
        StrSelect.AppendLine("          modal: true, ")
        StrSelect.AppendLine("          buttons: { ")
        StrSelect.AppendLine("              'Elimina': function () { ")
        StrSelect.AppendLine("                  $(this).dialog('close'); ")
        StrSelect.AppendLine("                  DoPostBack_EliminazioneControlli($('.variabileDeleteControlli').html());  ")
        StrSelect.AppendLine("              }, ")
        StrSelect.AppendLine("              'Annulla': function () { ")
        StrSelect.AppendLine("                      $(this).dialog('close'); ")
        StrSelect.AppendLine("                  return false; ")
        StrSelect.AppendLine("              } ")
        StrSelect.AppendLine("       } ")
        StrSelect.AppendLine("     }); ")





        'Si NO
        StrSelect.AppendLine("      $('.dialogControlliSiNo').dialog({ ")
        StrSelect.AppendLine("              autoOpen: false,")
        StrSelect.AppendLine("              width: 300,")
        StrSelect.AppendLine("              modal: true,")
        StrSelect.AppendLine("              buttons: {")
        StrSelect.AppendLine("              'Si': function () { ")
        StrSelect.AppendLine("                  $(this).dialog('close'); ")
        StrSelect.AppendLine("                  DoPostBack_ControlliSiNo($('.variabileSiNo').html());  ")
        StrSelect.AppendLine("              }, ")
        StrSelect.AppendLine("              'No': function () { ")
        StrSelect.AppendLine("                      $(this).dialog('close'); ")
        StrSelect.AppendLine("                  return false; ")
        StrSelect.AppendLine("              } ")
        StrSelect.AppendLine("              }")
        StrSelect.AppendLine("      });")







        'Si NO Bis
        StrSelect.AppendLine("      $('.dialogControlliSiNo_Bis').dialog({ ")
        StrSelect.AppendLine("              autoOpen: false,")
        StrSelect.AppendLine("              width: 300,")
        StrSelect.AppendLine("              modal: true,")
        StrSelect.AppendLine("              buttons: {")
        StrSelect.AppendLine("              'Si': function () { ")
        StrSelect.AppendLine("                  $(this).dialog('close'); ")
        StrSelect.AppendLine("                  DoPostBack_ControlliSiNo('si|'+$('.variabileSiNo_Bis').html());  ")
        StrSelect.AppendLine("              }, ")
        StrSelect.AppendLine("              'No': function () { ")
        StrSelect.AppendLine("                      $(this).dialog('close'); ")

        StrSelect.AppendLine("                  DoPostBack_ControlliSiNo('no|'+$('.variabileSiNo_Bis').html());  ")
        StrSelect.AppendLine("                  return false; ")
        StrSelect.AppendLine("              } ")
        StrSelect.AppendLine("              }")
        StrSelect.AppendLine("      });")





        StrSelect.AppendLine("});")

        StrSelect.AppendLine("</script>")
        Return StrSelect.ToString
    End Function

    Private Function GetHTML()
        If _PathImmagini.Length = 0 Then
            _PathImmagini = "../AB_Immagini/Messaggi"
        End If

        Dim StrSelect As New StringBuilder
        StrSelect.AppendLine("<!--Dialog di Avvertimento-->")
        StrSelect.AppendLine("<span id='dialogAvvertimentoControlli' class='dialogAvvertimentoControlli'></span>")

        StrSelect.AppendLine("<!--Dialog di Informazioni-->")
        StrSelect.AppendLine("<span id='dialogErroreControlli' class='dialogErroreControlli'></span>")

        StrSelect.AppendLine("<!--Dialog per l'eliminazione -->")
        StrSelect.AppendLine("<div class='variabileDeleteControlli' style='display:none;'></div>")
        StrSelect.AppendLine("<div class='dialogAvvertimentoDeleteControlli' title='Eliminare?'>")
        StrSelect.AppendLine("  <table class='centerTab'>")
        StrSelect.AppendLine("      <tr>")
        StrSelect.AppendLine("      <td>")
        StrSelect.AppendLine("          Confermare l'eliminazione del dato?<br />")
        StrSelect.AppendLine("          L'operazione non è annullabile.")
        StrSelect.AppendLine("      </td>")
        StrSelect.AppendLine("      <td>")
        StrSelect.AppendLine("         <img src='" & _PathImmagini & "/cestino.png' runat='server' />")
        StrSelect.AppendLine("      </td>")
        StrSelect.AppendLine("      </tr>")
        StrSelect.AppendLine("  </table>")
        StrSelect.AppendLine("</div>")



        StrSelect.AppendLine("<!--Dialog di SI NO -->")
        StrSelect.AppendLine("<div class='variabileSiNo' style='display:none;'></div>")
        StrSelect.AppendLine("<span id='dialogControlliSiNo' class='dialogControlliSiNo'></span>")


        StrSelect.AppendLine("<!--Dialog di SI NO BIS -->")
        StrSelect.AppendLine("<div class='variabileSiNo_Bis' style='display:none;'></div>")
        StrSelect.AppendLine("<span id='dialogControlliSiNo_Bis' class='dialogControlliSiNo_Bis'></span>")



        Return StrSelect.ToString
    End Function

    'Protected Overrides Sub OnInit(ByVal e As System.EventArgs)

    '    Literal = New LiteralControl
    '    Literal.ID = Me.ClientID & "DialogAvvertimentiControlli"

    '    Literal.Text = GetJS() + GetHTML()

    '    Me.Controls.Add(Literal)
    '    MyBase.OnInit(e)
    'End Sub

    Protected Overrides Sub OnPreRender(e As System.EventArgs)
        Literal = New LiteralControl
        Literal.ID = Me.ClientID & "DialogAvvertimentiControlli"

        Literal.Text = GetJS() + GetHTML()

        Me.Controls.Add(Literal)

        MyBase.OnPreRender(e)
    End Sub




End Class