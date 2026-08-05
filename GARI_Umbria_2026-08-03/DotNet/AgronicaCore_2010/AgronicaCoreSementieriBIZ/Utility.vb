

Imports System.Web.UI.HtmlControls



Public Class Utility


    '========================================================================
    Public Shared Sub ElaboraCellaHTML(
                            ByRef Cella As HtmlTableCell,
                            ByVal Bordo_Spessore As Integer,
                            ByVal Bordo_Tipo As String,
                            ByVal Bordo_Colore As String,
                            ByVal Sfondo_Colore As String,
                            ByVal Testo_Allineamento As String,
                            Optional ByVal Testo_Allineamento_Verticale As String = "top",
                            Optional ByVal Colspan As Integer = 0,
                            Optional ByVal Font_Weigth As String = "",
                            Optional ByVal Font_Size As String = "",
                            Optional ByVal Font_Color As String = "")


        'ESEMPIO : Call ElaboraCellaHTML(Riga.Cells(2), 1, "", "", "silver", "right")

        'Dim Riga As HtmlTableRow
        'Dim Cella As HtmlTableCell
        Dim TestoStile As String

        Dim xBordoSpessore As String
        Dim xBordoColore As String
        Dim xBordoTipo As String


        '-----------------------
        '----- BORDO CELLA -----
        '-----------------------

        If Bordo_Spessore > 0 Then

            'SPESSORE BORDO
            Select Case Bordo_Spessore
                Case 1
                    xBordoSpessore = "thin"
                Case 2
                    xBordoSpessore = "1px"
                Case 3
                    xBordoSpessore = "2px"
                Case Else
                    xBordoSpessore = "2px"
            End Select

            'COLORE BORDO
            If Bordo_Colore = "" Then
                xBordoColore = "black"
            End If

            'TIPO DI BORDO
            If Bordo_Tipo = "" Then
                xBordoTipo = "solid"
            End If


            TestoStile = xBordoColore & " " & xBordoSpessore & " " & xBordoTipo

            Cella.Style.Item("border-right") = TestoStile
            Cella.Style.Item("border-top") = TestoStile
            Cella.Style.Item("border-left") = TestoStile
            Cella.Style.Item("border-bottom") = TestoStile

        Else

            Cella.Style.Item("border-right") = "none"
            Cella.Style.Item("border-top") = "none"
            Cella.Style.Item("border-left") = "none"
            Cella.Style.Item("border-bottom") = "none"

        End If


        'BACKGROUND CELLA
        If Sfondo_Colore <> "" Then
            Cella.Style.Item("background-color") = Sfondo_Colore
        End If

        'COLSPAN
        If Colspan <> 0 Then
            Cella.ColSpan = Colspan
        End If

        'ALLINEAMENTO ORIZZONTALE TESTO CELLA
        If Testo_Allineamento <> "" Then
            Cella.Style.Item("text-align") = Testo_Allineamento
        End If

        'ALLINEAMENTO VERTICALE TESTO
        Cella.Style.Item("vertical-align") = Testo_Allineamento_Verticale
        Cella.VAlign = Testo_Allineamento_Verticale

        'STILE TESTO CELLA
        If Font_Weigth <> "" Then
            Cella.Style.Item("font-weight") = Font_Weigth
        End If

        'SIZE TESTO CELLA
        If Font_Size <> "" Then
            Cella.Style.Item("font-size") = Font_Size
        End If

        'COLORE TESTO CELLA
        If Font_Color <> "" Then
            Cella.Style.Item("color") = Font_Color
        End If


    End Sub



End Class
