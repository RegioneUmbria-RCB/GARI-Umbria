Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Public Class Interscambio_Analisi_Testata_R
    Public Function Leggi_Tabella_Interscambio_ChiaveGIAS(Sistema_Cod As Integer,
                                                          Analisi_Testata_Cod As Integer,
                                                          ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim objReadDAL As New AgronicaCoreInterscambioDAL.Interscambio_Analisi_Testata_R

        Return objReadDAL.Leggi(Sistema_Cod, Analisi_Testata_Cod, "", AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server)

    End Function

    Public Function Leggi_Tabella_Interscambio_ChiaveEsterna(Sistema_Cod As Integer,
                                                             Codice_Esterno As String,
                                                             ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim objReadDAL As New AgronicaCoreInterscambioDAL.Interscambio_Analisi_Testata_R

        Return objReadDAL.Leggi(Sistema_Cod, 0, Codice_Esterno, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server)

    End Function

    Public Function Check_Interscambio_ChiaveEsterna(Sistema_Cod As Integer,
                                                     Codice_Esterno As String,
                                                     ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim dataTableInterscambio = Leggi_Tabella_Interscambio_ChiaveEsterna(Sistema_Cod, Codice_Esterno, objParametri_Server)
        Dim result As Boolean

        If dataTableInterscambio.Rows.Count = 0 Then
            result = False
        Else
            result = True
        End If

        Return result

    End Function

End Class


Public Class Interscambio_Analisi_Testata_W
    Public Sub Scrivi_Tabella_Interscambio(Sistema_Cod As Integer,
                                           Analisi_Testata_Cod As Integer,
                                           Codice_Esterno As String,
                                           ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri)

        'transazione

        'controllo che le chiavi passate siano formattate correttamente
        If Analisi_Testata_Cod <= 0 Then
            Throw New Exception(String.Format(My.Resources.AgronicaCoreInterscambioBIZ.ChiaveGiasNonValidaAnalisi, Analisi_Testata_Cod))
        End If

        'non controllo che ci sia chiave doppia in quanto l'indice sul DB lo garantisce
        Dim objReadBIZ As New Interscambio_Analisi_Testata_R

        Dim checkExistsChiaveEsterna As Boolean = objReadBIZ.Check_Interscambio_ChiaveEsterna(Sistema_Cod, Codice_Esterno, objParametri_Server)

        'se count = 1, occorre verificare che le chiavi importate siano lo stesse

        'costruire codice GIAS e passarlo al DAL

        'se entrambe non sono presenti nella tabella di interscambio, scrivo
        If checkExistsChiaveEsterna = False Then

            Dim objWriteDAL As New AgronicaCoreInterscambioDAL.Interscambio_Analisi_Testata_W

            Dim result = objWriteDAL.Scrivi(Sistema_Cod,
                                            Analisi_Testata_Cod,
                                            Codice_Esterno,
                                            AGRODATAINIZIO, AGRODATAFINE,
                                            objParametri_Server)
        End If

    End Sub
End Class
