Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class Limitazioni_WS

    Public Shared Function Localizzazioni_Elenco(ByVal Disciplinare_Cod As Integer,
                                                 ByVal Id_RcDpi As Integer,
                                                 ByVal Fr_Cod As Integer,
                                                 ByVal strPa_Cod As String,
                                                 ByVal TipoTestata As Integer,
                                                 ByVal Veg_Cod As Integer,
                                                 ByVal Av_Cod As Integer,
                                                 ByVal Av_Gru As Integer,
                                                 ByVal Avversita_Filtro As String,
                                                 ByVal Data As Date,
                                                 ByVal NomeUtente As String,
                                                 ByRef objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                 ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                 ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                 ByRef strErr As String) As DataTable


        Dim ObjDownloadWs As WS_Disciplinari.AgroWS_Disciplinari


        Dim DTLocalizzazioni As DataTable

        Try

            Dim agroWs As String
            Dim objConfigurazione_Siti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            agroWs = objConfigurazione_Siti.Leggi_Valore(0, "GiasOnline_WS_Disciplinari_AgroWS_Disciplinari", "", "", objParametri_Server)
            If agroWs = "" Then
                agroWs = objConfigurazione_Siti.Leggi_Valore(0, "GiasOnline_WS_Disciplinari_AgroWS_Disciplinari", "", "", objParametri_Super_Server)
            End If

            ObjDownloadWs = New WS_Disciplinari.AgroWS_Disciplinari
            Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS
            objWs.NewWS(ObjDownloadWs,
                            agroWs,
                            objParametri_Utenti)

            'TODO: LUKE DA CAMBIARE - NON COMPILA!
            DTLocalizzazioni = ObjDownloadWs.Leggi_Localizzazioni(Disciplinare_Cod,
                                                      Id_RcDpi,
                                                      Fr_Cod,
                                                      strPa_Cod,
                                                      TipoTestata,
                                                      Veg_Cod,
                                                      Av_Cod,
                                                      Av_Gru,
                                                      Avversita_Filtro,
                                                      Data,
                                                      NomeUtente,
                                                      strErr)



            Return DTLocalizzazioni


        Catch ex As Exception

            strErr = "Si sono verificati errori in fase di chiamata al WebService Discliplinari!" & Chr(13) &
                       ex.Message

            Return DTLocalizzazioni

        End Try

        Return DTLocalizzazioni

    End Function

End Class
