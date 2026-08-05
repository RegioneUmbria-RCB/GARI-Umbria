Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class Interscambio_Risorse_Umane_R

    Public Function Leggi_Tabella_Interscambio_ChiaveGIAS(Sistema_Cod As Integer,
                                                          Piva As String,
                                                          Cod_Risum As Integer,
                                                          ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim objReadDAL As New AgronicaCoreInterscambioDAL.Interscambio_Risorse_Umane_R

        Return objReadDAL.Leggi(Sistema_Cod, Piva, Cod_Risum, "", AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server)

    End Function

    Public Function Leggi_Tabella_Interscambio_ChiaveEsterna(Sistema_Cod As Integer,
                                                             Codice_Esterno As String,
                                                             ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim objReadDAL As New AgronicaCoreInterscambioDAL.Interscambio_Risorse_Umane_R

        Return objReadDAL.Leggi(Sistema_Cod, String.Empty, 0, Codice_Esterno, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server)

    End Function

    Public Function Check_Interscambio_ChiaveEsterna(Sistema_Cod As Integer,
                                                    Codice_Esterno As String,
                                                    ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim dataTableInterscambio = Leggi_Tabella_Interscambio_ChiaveEsterna(Sistema_Cod, Codice_Esterno, objParametri_Server)
        Dim result As Boolean

        If dataTableInterscambio.Rows.Count > 1 Then
            Throw New Exception("Chiave doppia " + Codice_Esterno)
        End If

        If dataTableInterscambio.Rows.Count = 0 Then
            result = False
        Else
            result = True
        End If

        Return result

    End Function

End Class

Public Class Interscambio_Risorse_Umane_W

    Public Sub Scrivi_Tabella_Interscambio(Sistema_Cod As Integer,
                                           Piva As String,
                                           Cod_Risum As Integer,
                                           Codice_Esterno As String,
                                           ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
                                           )

        'transazione

        'controllo che le chiavi passate siano formattate correttamente
        If String.IsNullOrEmpty(Piva) OrElse Cod_Risum <= 0 Then
            Throw New Exception(String.Format(My.Resources.AgronicaCoreInterscambioBIZ.ChiaveGiasNonValida, Piva, Cod_Risum))
        End If

        Dim objReadBIZ As New Interscambio_Risorse_Umane_R
        Dim checkExistsChiaveEsterna As Boolean = objReadBIZ.Check_Interscambio_ChiaveEsterna(Sistema_Cod, Codice_Esterno, objParametri_Server)

        'costruire codice GIAS e passarlo al DAL
        Dim Codice_Gias As String = Cod_Risum

        'se entrambe non sono presenti nella tabella di interscambio, scrivo
        If checkExistsChiaveEsterna = False Then

            Dim objWriteDAL As New AgronicaCoreInterscambioDAL.Interscambio_Risorse_Umane_W

            Dim result = objWriteDAL.Scrivi(Sistema_Cod,
                                            Piva,
                                            Cod_Risum,
                                            Codice_Gias,
                                            Codice_Esterno,
                                            AGRODATAINIZIO,
                                            AGRODATAFINE,
                                            objParametri_Server)
        End If

    End Sub

    Public Function Cancella_Tabella_Interscambio(Sistema_Cod As Integer,
                                             Codice_Esterno As String,
                                             ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim objWriteDAL As New AgronicaCoreInterscambioDAL.Interscambio_Risorse_Umane_W
        Dim result = objWriteDAL.Cancella(Sistema_Cod, Codice_Esterno, objParametri_Server)
        Return result

    End Function

End Class
