Namespace Agro_GoogleMaps

    Partial Class MapFinder_Tester
        Inherits System.Web.UI.Page

        Private NuovoRitaglio As Boolean
        Public piloro As String
        Public piloro2 As String

        Private Sub Ciao()


            TxtLatitudine.Text = "ciao"
            TxtLongitudine.Text = "ciao"
            TxtZoom.Text = "ciao"

            TxtLatitudineN.Text = "ciao"
            TxtLongitudineO.Text = "ciao"
            TxtLatitudineS.Text = "ciao"
            TxtLongitudineE.Text = "ciao"

        End Sub


        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


            piloro = "0000000001"
            piloro2 = "1234"

            If RitaglioGoogle.Value = "1" Then
                NuovoRitaglio = True
            Else
                NuovoRitaglio = False
            End If


            If Not Page.IsPostBack Then

                '==========================================
                '===== Pagina caricata per la prima volta
                '==========================================

            Else

                '==========================================
                '===== Pagina ricaricata in POSTBACK
                '==========================================

                If NuovoRitaglio = True Then
                    Ciao()
                End If

                'Evito di re-inizializzare i controlli
                Exit Sub

            End If

        End Sub
    End Class

End Namespace