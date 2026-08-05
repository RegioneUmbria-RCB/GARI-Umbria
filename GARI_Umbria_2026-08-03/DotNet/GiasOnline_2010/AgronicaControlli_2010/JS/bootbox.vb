Imports AgronicaCoreVarieDAL

'''
'''http://bootboxjs.com/#download
'''
'''x ALERT
'''bootbox.alert("Hello world!", function() {
'''  Example.show("Hello world callback");
'''});

'''x CONFIRM
'''bootbox.confirm("Are you sure?", function(result) {
'''  Example.show("Confirm result: "+result);
'''}); 

'''x PROMPT
'''bootbox.prompt("What is your name?", function(result) {                
'''  if (result === null) {                                             
'''    Example.show("Prompt dismissed");                              
'''  } else {
'''    Example.show("Hi <b>"+result+"</b>");                          
'''  }
'''});

'''x DIALOG
'''bootbox.dialog({
'''  message: "I am a custom dialog",
'''  title: "Custom title",
'''  buttons: {
'''    success: {
'''      label: "Success!",
'''      className: "btn-success",
'''      callback: function() {
'''        Example.show("great success");
'''      }
'''    },
'''    danger: {
'''      label: "Danger!",
'''      className: "btn-danger",
'''      callback: function() {
'''        Example.show("uh oh, look out!");
'''      }
'''    },
'''    main: {
'''      label: "Click ME!",
'''      className: "btn-primary",
'''      callback: function() {
'''        Example.show("Primary button");
'''      }
'''    }
'''  }
'''});

Public Class bootbox
    Inherits System.Web.UI.WebControls.Label

#Region "Methods & Event Handlers"

    Protected Overrides Sub OnPreRender(e As EventArgs)
        Page.ClientScript.RegisterClientScriptInclude("bootbox", Page.ClientScript.GetWebResourceUrl(Me.[GetType](), "AgronicaControlli_2010.bootbox.min.js"))

    End Sub


#End Region

End Class
