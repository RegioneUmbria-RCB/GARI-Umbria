Imports AgronicaCoreDataProvider
Imports AgronicaCoreMetaSchemaBIZ
Imports AgronicaCoreModelsSTD
Imports AgronicaCoreModelsSTD.anagrafiche
Imports System.Data

Public Class STD_PianoConcimazione
    Public Function LeggiClasseTessitura(Sabbia As Decimal,
                                         Argilla As Decimal,
                                         objParametri_Super_Server As AgronicaCoreParametri,
                                         objParametri_Server As AgronicaCoreParametri,
                                         objParametri_Utenti As AgronicaCoreParametri) As ClasseTessitura

        Dim Id_ClasseTessitura As Integer = 0
        Dim Descrizione As String = ""

        Sabbia = Agro_Math.ArrotondaVal_0(CDec(CDbl(Sabbia)))
        Argilla = Agro_Math.ArrotondaVal_0(CDec(CDbl(Argilla)))

        If Sabbia <> -99 AndAlso Argilla <> -99 Then

            Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PUA_ClassiTessitura_Analisi_input With {
                .Argilla = Argilla,
                .Sabbia = Sabbia,
                .verbose = True,
                .Url = ""
            }

            Dim objAgroWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
            If IsNothing(objAgroWebConfig.GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione) Then
                Dim agroWs As String
                Dim objConfigurazione_Siti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                agroWs = objConfigurazione_Siti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametri_Server)
                If agroWs = "" Then
                    agroWs = objConfigurazione_Siti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametri_Super_Server)
                End If
                objParametriIngresso.Url = agroWs
            End If

            Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PUA_ClassiTessitura_Analisi_output
            Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
            objParametriUscita = objPC_WS.ClassiTessituraDaAnalisi(objParametriIngresso)

            If objParametriUscita IsNot Nothing AndAlso
                objParametriUscita.ListaClassiTessitura IsNot Nothing AndAlso
                objParametriUscita.ListaClassiTessitura.Count = 1 Then
                Id_ClasseTessitura = objParametriUscita.ListaClassiTessitura(0).Id_ClasseTessitura
                Descrizione = objParametriUscita.ListaClassiTessitura(0).Descrizione
            End If
        End If

        If Id_ClasseTessitura <> 0 Then
            Return New ClasseTessitura(Id_ClasseTessitura, Descrizione)
        Else
            Return Nothing
        End If

    End Function
End Class
