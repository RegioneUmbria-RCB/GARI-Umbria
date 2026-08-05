
Imports AgronicaCoreMetaSchemaDAL
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Formulati


    Public Sub LavCodDataClassificazioneProdotto(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, Pro_Cod As Integer, ByRef LavCodDatoProdotto As Integer, ByRef lavDesDatoProdotto As String)


        Dim objClass As New FormulatixClassifica_R

        Dim DtFC As New DataTable
        Dim strFiltro As String = "Formulati.FR_COD=" & Agro_SQL_SaveNum(Pro_Cod)

        DtFC = objClass.LeggiXClassificazione("", enum_TipoFormulato.Antiparassitari, AGRODATAINIZIO, AGRODATAFINE, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, strFiltro, "", objParametri_Server)
        If Not DtFC Is Nothing AndAlso DtFC.Rows.Count > 0 Then

            LavCodDatoProdotto = LAVCOD_TRATTAMENTO_ANTIPARASSITARIO
            lavDesDatoProdotto = "Trattamento Antiparassitario"
        Else

            DtFC = objClass.LeggiXClassificazione("", enum_TipoFormulato.Fitoregolatori, AGRODATAINIZIO, AGRODATAFINE, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, strFiltro, "", objParametri_Server)
            If Not DtFC Is Nothing AndAlso DtFC.Rows.Count > 0 Then

                LavCodDatoProdotto = LAVCOD_TRATTAMENTO_FITOREGOLATORE
                lavDesDatoProdotto = "Trattamento Fitoregolatore"

            Else
                DtFC = objClass.LeggiXClassificazione("", enum_TipoFormulato.Concianti, AGRODATAINIZIO, AGRODATAFINE, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, strFiltro, "", objParametri_Server)
                If Not DtFC Is Nothing AndAlso DtFC.Rows.Count > 0 Then
                    LavCodDatoProdotto = LAVCOD_CONCIA_SEME
                    lavDesDatoProdotto = "Concia del Seme"
                Else

                    DtFC = objClass.LeggiXClassificazione("", enum_TipoFormulato.Geodisinfestanti, AGRODATAINIZIO, AGRODATAFINE, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, strFiltro, "", objParametri_Server)
                    If Not DtFC Is Nothing AndAlso DtFC.Rows.Count > 0 Then
                        LavCodDatoProdotto = LAVCOD_GEODISINFESTAZIONE
                        lavDesDatoProdotto = "Geodisinfestazione"
                    Else

                        DtFC = objClass.LeggiXClassificazione("", enum_TipoFormulato.Disseccanti, AGRODATAINIZIO, AGRODATAFINE, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, strFiltro, "", objParametri_Server)
                        If Not DtFC Is Nothing AndAlso DtFC.Rows.Count > 0 Then
                            LavCodDatoProdotto = LAVCOD_DISSECCAMENTO
                            lavDesDatoProdotto = "Disseccamento"
                        End If
                    End If
                End If

            End If

        End If
    End Sub


End Class
