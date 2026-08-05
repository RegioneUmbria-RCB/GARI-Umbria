Public Class matematicaGeometriaHelper

    ''' <summary>
    ''' calcola l'angolo fra segmenti
    ''' </summary>
    ''' <param name="pPuntiseg1"></param>
    ''' <param name="pPuntisegRiferimento"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function AngoloFraSegmenti(ByVal pPuntiseg1 As List(Of xyz), ByVal pPuntisegRiferimento As List(Of xyz), ByVal UnoSensoOrario_MenoUnoSensonAniorario As Double) As Double



        Dim iEndYPoint As Double = 0.0R
        Dim iStartYPoint As Double = 0.0R
        Dim iEndXPoint As Double = 0.0R
        Dim iStartXPoint As Double = 0.0R

        Dim i1EndYPoint As Double = 0.0R
        Dim i1StartYPoint As Double = 0.0R
        Dim i1EndXPoint As Double = 0.0R
        Dim i1StartXPoint As Double = 0.0R

        Dim vpPuntiseg1 As Array = _
            pPuntiseg1.ToArray

        Dim vpPuntisegRiferimento As Array = _
            pPuntisegRiferimento.ToArray

        iStartXPoint = CType(vpPuntiseg1(0), xyz).X
        iStartYPoint = CType(vpPuntiseg1(0), xyz).Y

        iEndXPoint = CType(vpPuntiseg1(1), xyz).X
        iEndYPoint = CType(vpPuntiseg1(1), xyz).Y

        i1StartXPoint = CType(vpPuntisegRiferimento(0), xyz).X
        i1StartYPoint = CType(vpPuntisegRiferimento(0), xyz).Y

        i1EndXPoint = CType(vpPuntisegRiferimento(1), xyz).X
        i1EndYPoint = CType(vpPuntisegRiferimento(1), xyz).Y

        'tratto da: http://social.msdn.microsoft.com/forums/en-US/csharpgeneral/thread/fa0cfeb6-70b7-4181-bc9b-fe625cd5e159
        'post Friday, October 26, 2007 3:25 PM
        'Double Angle = Math.Atan2(y2-y1, x2-x1) - Math.Atan2(y4-y3,x4-x3);

        Return UnoSensoOrario_MenoUnoSensonAniorario * (Math.Atan2(iEndYPoint - iStartYPoint, iEndXPoint - iStartXPoint) - Math.Atan2(i1EndYPoint - i1StartYPoint, i1EndXPoint - i1StartXPoint))

    End Function

    Public Shared Function GetPPuntiRiferimentoRotazione(ByVal pPuntiDaRuotare As List(Of xyz), ByVal Orientamento_1x_2y As Integer) As List(Of xyz)
        Dim pPuntiRiferimentoRotazione As New List(Of xyz)
        Dim vPuntiRiferimentoRotazione As Array = _
            pPuntiDaRuotare.ToArray

        Dim i1 As Integer
        Dim i2 As Integer
        If Orientamento_1x_2y = 1 Then
            i1 = 0
            i2 = 1
        Else
            i1 = 1
            i2 = 2
        End If

        pPuntiRiferimentoRotazione.Add(New xyz With {.X = CType(vPuntiRiferimentoRotazione(i1), xyz).X, .Y = CType(vPuntiRiferimentoRotazione(i1), xyz).Y})
        pPuntiRiferimentoRotazione.Add(New xyz With {.X = CType(vPuntiRiferimentoRotazione(i2), xyz).X, .Y = CType(vPuntiRiferimentoRotazione(i2), xyz).Y})
        Return pPuntiRiferimentoRotazione
    End Function
    Public Shared Function Rotazione(ByVal pPuntiDaRuotare As List(Of xyz), ByVal pPuntisegRiferimento As List(Of xyz), ByVal Orientamento_1x_2y As Integer, ByVal theta As Double, ByVal centro As xyz, ByVal distanza As Double) As List(Of xyz)

        If theta = 0 Then
            Dim pPuntiRiferimentoRotazione As List(Of xyz) = GetPPuntiRiferimentoRotazione(pPuntiDaRuotare, Orientamento_1x_2y)
            theta = AngoloFraSegmenti(pPuntiRiferimentoRotazione, pPuntisegRiferimento, -1)
        End If

        'tratto da: http://www.vb-helper.com/howto_rotate_polygon_points.html
        'X' = Cos(theta) * X - Sin(theta) * Y
        'Y' = Sin(theta) * X + Cos(theta) * Y


        Dim ppuntiRuotati As New List(Of xyz)
        Dim ppuntoRoutato As xyz
        For Each ppunto In pPuntiDaRuotare

            ppunto.X = ppunto.X - centro.X
            ppunto.Y = ppunto.Y - centro.Y

            ppuntoRoutato = New xyz
            ppuntoRoutato.X = Math.Cos(theta) * ppunto.X - Math.Sin(theta) * ppunto.Y
            ppuntoRoutato.Y = Math.Sin(theta) * ppunto.X + Math.Cos(theta) * ppunto.Y

            ppuntoRoutato.X = ppuntoRoutato.X + centro.X
            ppuntoRoutato.Y = ppuntoRoutato.Y + centro.Y

            ppuntiRuotati.Add(ppuntoRoutato)
        Next

        Return ppuntiRuotati
    End Function
End Class
