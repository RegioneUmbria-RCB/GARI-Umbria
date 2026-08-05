Public Class CoreWS_ConversioneAltUdmLeggi

    Public Class LeggiTassoConversione
        Inherits APICallsBasic

        Public Property Udm_Cod_From As Integer
        Public Property Udm_Cod_Alt As Integer

        ''' <summary>
        ''' Lettura tasso conversione da udm a udm alternativa in tabella Conversione_UnitaMisura_Alternative
        ''' </summary>
        ''' <param name="objP_super_server"></param>
        ''' <param name="objP_server"></param>
        ''' <param name="objP_utenti"></param>
        ''' <param name="piva"></param>
        ''' <param name="udmCodFrom"></param>
        ''' <param name="udmCodAlt"></param>
        Public Sub New(ByVal objP_super_server As String, ByVal objP_server As String, ByVal objP_utenti As String,
                       Udm_Cod_From As Integer,
                       Udm_Cod_Alt As Integer)

            MyBase.New(objP_super_server, objP_server, objP_utenti)

            Me.Udm_Cod_From = Udm_Cod_From
            Me.Udm_Cod_Alt = Udm_Cod_Alt

        End Sub

    End Class

    Public Class GetSuperficieEttari
        Inherits APICallsBasic

        Public Property Superificie As Double
        Public Property Udm_Cod_Alt As Integer

        ''' <summary>
        ''' Ottiene il valore in ettari della superficie in unità di misura passata
        ''' </summary>
        ''' <param name="objP_super_server"></param>
        ''' <param name="objP_server"></param>
        ''' <param name="objP_utenti"></param>
        ''' <param name="superificie"></param>
        ''' <param name="udmCodAlt"></param>
        Public Sub New(ByVal objP_super_server As String, ByVal objP_server As String, ByVal objP_utenti As String,
                       Superificie As Double,
                       Udm_Cod_Alt As Integer)

            MyBase.New(objP_super_server, objP_server, objP_utenti)

            Me.Superificie = Superificie
            Me.Udm_Cod_Alt = Udm_Cod_Alt

        End Sub

    End Class
End Class

