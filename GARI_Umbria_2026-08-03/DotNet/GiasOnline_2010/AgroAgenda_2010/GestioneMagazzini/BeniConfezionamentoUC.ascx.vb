Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class BeniConfezionamentoUC
    Inherits System.Web.UI.UserControl

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Public objparametri_server_string, objparametri_utenti_string As String
    Dim objParametriAgenda As ParametriAgenda

    'Private Sub inizializzoObjParametri()
    '    '---
    '    objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))
    '    objParametri_Utenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
    '    objparametri_server_string = Utility.convertOBJparametritoString(objParametri_Server)
    '    '---
    'End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        
        Dim hdPiva As HtmlInputHidden = Parent.FindControl("hdPiva")
        Dim hdLavCod As HtmlInputHidden = Parent.FindControl("hdLavCod")
        Dim hdIdAgenda As HtmlInputHidden = Parent.FindControl("hdIdAgenda")
        Dim hdModalita As HtmlInputHidden = Parent.FindControl("hdModalita")
        Dim hdId_Mov_Testata As HtmlInputHidden = Parent.FindControl("hdId_Mov_Testata")
        Dim hdId_Mov_BC_Principale As HtmlInputHidden = Parent.FindControl("hdId_Mov_BC_Principale")
        Dim hdCau_Mov_BC_Principale As HtmlInputHidden = Parent.FindControl("hdCau_Mov_BC_Principale")
        Dim hdData As HtmlInputHidden = Parent.FindControl("hdData")
        Dim hdPiva_Scarico As HtmlInputHidden = Parent.FindControl("hdPiva_Scarico")
        Dim hdId_Agenda_Scarico As HtmlInputHidden = Parent.FindControl("hdId_Agenda_Scarico")
        Dim hdId_Mov_Testata_Scarico As HtmlInputHidden = Parent.FindControl("hdId_Mov_Testata_Scarico")
        Dim hdId_Mov_Scarico As HtmlInputHidden = Parent.FindControl("hdId_Mov_Scarico")
        Dim hdCod_Risum_Scarico As HtmlInputHidden = Parent.FindControl("hdCod_Risum_Scarico")
        Dim hdLav_Cod_Scarico As HtmlInputHidden = Parent.FindControl("hdLav_Cod_Scarico")

        'Impostazione Chiavi

        hdModalita.Value = 0
        hdId_Mov_Testata.Value = 0
        hdId_Mov_BC_Principale.Value = 0
        If String.IsNullOrEmpty(hdCau_Mov_BC_Principale.Value) Then
            hdCau_Mov_BC_Principale.Value = CAU_CARICO
        End If
        hdData.Value = JToken.Parse(JsonConvert.SerializeObject(Now))

        hdPiva_Scarico.Value = hdPiva.Value
        hdId_Agenda_Scarico.Value = 0
        hdId_Mov_Testata_Scarico.Value = 0
        hdId_Mov_Scarico.Value = 0
        hdLav_Cod_Scarico.Value = LAVCOD_BOLLA_EMESSA
        hdCod_Risum_Scarico.Value = 0

        If IsNumeric(hdIdAgenda.Value) AndAlso hdIdAgenda.Value <> 0 Then

            Dim dt As DataTable

            'Lettura Agenda
            dt = DocContabile_WS.Leggi_Agenda_BC(hdPiva.Value, hdIdAgenda.Value)

            If dt.Rows.Count > 0 Then

                For Each dr As DataRow In dt.Rows

                    hdLavCod.Value = dr("Lav_Cod")

                    Select Case dr("Cau_Mov")

                        Case CAU_REGISTRAZIONI

                            hdId_Mov_Testata.Value = dr("Id_Mov")

                        Case hdCau_Mov_BC_Principale.Value

                            hdId_Mov_BC_Principale.Value = dr("Id_Mov")

                    End Select

                Next

            End If




            'Lettura Riferimento
            dt = DocContabile_WS.Leggi_Agenda_Riferimento_BC(hdPiva.Value, hdIdAgenda.Value)

            If dt.Rows.Count > 0 Then

                For Each dr As DataRow In dt.Rows

                    hdId_Agenda_Scarico.Value = dr("Id_Agenda")
                    hdLav_Cod_Scarico.Value = dr("Lav_Cod")

                    Select Case dr("Cau_Mov")

                        Case CAU_REGISTRAZIONI

                            hdId_Mov_Testata_Scarico.Value = dr("Id_Mov")

                        Case CAU_SCARICO

                            hdId_Mov_Scarico.Value = dr("Id_Mov")

                    End Select

                Next

            End If

        End If

    End Sub

End Class
