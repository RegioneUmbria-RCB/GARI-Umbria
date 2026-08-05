Imports AgronicaCoreModelloSTD

Public Class Ricette_Dettagli_Prodotti
    Inherits Ricette_Dettagli
    Implements iRicette_Dettagli

    Public Property Prodotto As Prodotto

    ''' <summary>
    ''' unità di magazzino (sempre kg o litri)
    ''' </summary>
    ''' <returns></returns>
    Public Property Udm As UnitaMisura

    ''' <summary>
    ''' Unità indicata dall'utente
    ''' </summary>
    ''' <returns></returns>
    Public Property Udm_Indicata As UnitaMisura


    Public Property Dose_QtaTotale As Integer

    Public Property Dose_Ha_Reale As Decimal
    Public Property Dose_Hl_Reale As Decimal
    Public Property Dose_Totale_Reale As Decimal
    Public Property Ha_Hl As Integer

    Public ReadOnly Property Prodotto_Qta() As String
        Get
            Dim quantita As String = Udm_Indicata.udm_sim & " " & Decimal.Round(Dose_Ha_Reale, 2) & "/ha; "
            If Operazione.Acqua > 0 Then
                quantita &= Decimal.Round(Dose_Hl_Reale, 2) & "/hl; "
            End If
            quantita &= Decimal.Round(Dose_Totale_Reale, 2) & "/tot"
            Return quantita
        End Get
    End Property

    Public Property MagazziniMovimentazioni As RilevamentoDiMagazzino

End Class
