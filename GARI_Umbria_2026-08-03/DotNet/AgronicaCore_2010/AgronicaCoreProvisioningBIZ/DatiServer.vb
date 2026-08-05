Public Class DatiServer_R

    Public Function GetDatiServer(ByVal tipoServer As Int32,
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                  ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                  ByRef objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
                                  ) As DataTable

        Dim xRead As New AgronicaCoreProvisioningDAL.DatiServer_R

        Dim connBuilder = New Common.DbConnectionStringBuilder

        Dim DT As New DataTable

        Select Case CType(tipoServer, AgronicaCoreDTOStd.InData.Provisioning.DatiServerRequest.enum_Scelta_Server)
            Case AgronicaCoreDTOStd.InData.Provisioning.DatiServerRequest.enum_Scelta_Server.Server
                connBuilder.ConnectionString = objParametri.StringaConnessione
            Case AgronicaCoreDTOStd.InData.Provisioning.DatiServerRequest.enum_Scelta_Server.Utenti
                connBuilder.ConnectionString = objParametri_Utenti.StringaConnessione
            Case Else
                Throw New Exception("Scegliere un tipo di server")
        End Select

        If connBuilder Is Nothing Then
            Throw New Exception("DB non riconosciuto")
        End If

        DT = xRead.Leggi(connBuilder("Server"),
                         connBuilder("Initial Catalog"),
                         AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                         "",
                         "",
                         objParametri,
                         objParametri_Utenti,
                         objParametri_Super_Server)

        Return DT

    End Function

    Public Function GetConnessioneDB(ByVal ID_DB As Int32,
                                     ByRef objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable

        Dim xRead As New AgronicaCoreProvisioningDAL.DatiServer_R

        Dim connBuilder = New Common.DbConnectionStringBuilder

        Dim DT As DataTable

        DT = xRead.LeggiConnessioneDB(ID_DB,
                                      AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                      "",
                                      "",
                                      objParametri_Super_Server)

        If DT Is Nothing OrElse DT.Rows.Count = 0 Then
            Throw New Exception("DB non riconosciuto")
        End If

        Return DT

    End Function

End Class
Public Class DatiServer_W

End Class
