Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class ImputazioneImpiantiUC
    Inherits System.Web.UI.UserControl


    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Public objparametri_server_string, objparametri_utenti_string As String
    Dim objParametriAgenda As ParametriAgenda

    Private Sub inizializzoObjParametri()
        '---
        objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objparametri_server_string = Utility.convertOBJparametritoString(objParametri_Server)
        '---
    End Sub



    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'Master.flag_pag_ImputazioneImpianti = True

        Dim hdPiva As HtmlInputHidden = Parent.FindControl("hdPiva")
        Dim hdIdAgenda As HtmlInputHidden = Parent.FindControl("hdIdAgenda")
        Dim hdCod_Risum_Scarico As HtmlInputHidden = Parent.FindControl("hdCod_Risum_Scarico")




    End Sub


End Class