Imports System.Web.UI.WebControls

Public Class CaricaListControl
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''' 	[magnani]	22/04/2011	Created
    '''' </history>
    '''' -----------------------------------------------------------------------------
    Public Shared Sub Gis_LayerElementiGrafici(ByRef Controllo As ListControl,
                             ByVal PrimaRiga_Flag As Boolean,
                             ByVal PrimaRiga_Text As String,
                             ByVal PrimaRiga_Value As String,
                             ByVal xFiltroAggiuntivo As String,
                             ByVal xOrderBy As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             )

        Dim Dt As DataTable
        Dim i As Integer
        Dim obj As New AgronicaCoreGisDAL.GIS_LayerElementiGrafici_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag = True Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = obj.Leggi(objParametri.PivaSuperUser, objParametri.SuperUserUsername, 0, 0, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, xFiltroAggiuntivo, xOrderBy, objParametri)

        If Not IsNothing(Dt) Then

            If Dt.Rows.Count <> 0 Then

                For i = 0 To Dt.Rows.Count - 1

                    Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("LayerElementiGrafici_Des"),
                                                     Dt.Rows(i).Item("LayerElementiGrafici_Cod")))


                Next

            End If

        End If

    End Sub
End Class
