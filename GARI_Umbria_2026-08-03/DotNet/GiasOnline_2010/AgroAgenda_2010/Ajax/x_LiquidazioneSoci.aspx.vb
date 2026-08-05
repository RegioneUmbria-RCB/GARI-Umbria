
Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility.CaricaListControl
Imports AgronicaCoreUtility
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class x_LiquidazioneSoci
    Inherits System.Web.UI.Page



#Region "Script services"




    <WebMethod(EnableSession:=True)> _
    Public Shared Function GetPivaSoci(ByVal query As String) As String

        If HttpContext.Current.Session.IsNewSession Then
            Return "SessioneScaduta"
        End If


        Dim objParametri_Server As AgronicaCoreParametri
        objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")

        If objParametri_Server Is Nothing Then
            Return "SessioneScaduta"
        End If


        Dim str_RISPOSTA As String = "["

        Try

            'Inserire il codice QUI..

            Dim ddl As New DropDownList

            Dim i As Integer


            Contatti_2(ddl, False, "", "", "", "", 0, COD_FORNITORE, True, False, 0, 0, False, 0, 1, "", "", objParametri_Server)

            If i > 0 Then
                str_RISPOSTA = str_RISPOSTA & ","
            End If

            Dim j As Integer
            For j = 0 To ddl.Items.Count - 1
                If j > 0 Then
                    str_RISPOSTA = str_RISPOSTA & ","
                End If
                str_RISPOSTA = str_RISPOSTA & "{""codice"":""" & ddl.Items(j).Value & """, ""descrizione"":""" & ddl.Items(j).Text & """}"
            Next


            str_RISPOSTA = str_RISPOSTA & "]"


            Return "ok" & str_RISPOSTA


        Catch ex As Exception

            Return "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try


    End Function




#End Region

End Class