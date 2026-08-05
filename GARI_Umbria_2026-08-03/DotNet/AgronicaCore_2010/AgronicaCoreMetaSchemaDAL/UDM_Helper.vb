Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider

Public Class UDM_Helper

 


    Public Shared Function GetUdmSim_AREA(ByVal objparametri_server As AgronicaCoreParametri, ByVal objParametri_Utenti As AgronicaCoreParametri) As String
        Return GetUdmSim(DammiCodiceUDM(objParametri_Utenti), objparametri_server)
    End Function

    Public Shared Function GetUdmSim(ByVal lUdm As Integer, ByVal objparametri_server As AgronicaCoreParametri) As String
        Dim udmSim As String
        Dim udmLeggi As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R
        Dim dtUdm As DataTable = _
        udmLeggi.Leggi( _
            lUdm, _
            0, _
            "", _
            "", _
            enumSelezioneVariabile.Selezione_TabellaCompleta, _
            "", _
            "", _
            objparametri_server _
            )

        udmSim = dtUdm.Rows(0)("udm_sim")
        Return udmSim
    End Function

    Public Shared Function DammiCodiceUDM(ByVal lobjParametri As AgronicaCoreParametri) As Integer


        Dim rval As Integer = 0
        Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim Dt_Impostazioni As DataTable

        Dt_Impostazioni = ObjUtenti.Leggi(0, _
                                          1, _
                                          AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                          "", _
                                          "", _
                                          lobjParametri)

        Dim fatto As Boolean = False

        If Not Dt_Impostazioni Is Nothing AndAlso Dt_Impostazioni.Rows.Count > 0 Then

            For i = 0 To Dt_Impostazioni.Rows.Count - 1

                Select Case CInt(Dt_Impostazioni.Rows(i).Item("Impostazione_Cod"))

                    Case enum_Impostazioni_Utenti.UTENTE_UDM_Area_COD
                        Dim lUdm As Integer = CInt(Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1"))
                        Return lUdm
                End Select

            Next
        End If

        Return 2123


    End Function



End Class
