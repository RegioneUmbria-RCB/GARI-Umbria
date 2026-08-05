Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class Interscambio_Fabbricati_R

    Public Function Leggi_Tabella_Interscambio_ChiaveGIAS(Sistema_Cod As Integer,
                                                           Piva As String,
                                                           Sa_Cod As Integer,
                                                           Fabbricato_Cod As Integer,
                                                           ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim objReadDAL As New AgronicaCoreInterscambioDAL.Interscambio_Fabbricati_R

        Return objReadDAL.Leggi(Sistema_Cod, Piva, Sa_Cod, Fabbricato_Cod, "", "", "", objParametri_Server)

    End Function

    Public Function Leggi_Tabella_Interscambio_ChiaveEsterna(Sistema_Cod As Integer,
                                                           Codice_Esterno As String,
                                                           ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim objReadDAL As New AgronicaCoreInterscambioDAL.Interscambio_Fabbricati_R

        Return objReadDAL.Leggi(Sistema_Cod, "", 0, 0, Codice_Esterno, "", "", objParametri_Server)

    End Function

    Public Function Check_Interscambio_ChiaveEsterna(Sistema_Cod As Integer,
                                                    Codice_Esterno As String,
                                                    ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim dataTableInterscambio = Leggi_Tabella_Interscambio_ChiaveEsterna(Sistema_Cod, Codice_Esterno, objParametri_Server)
        Dim result As Boolean

        If Sistema_Cod = enum_SistemiEsterni.demetra AndAlso dataTableInterscambio.Rows.Count > 1 Then
            Throw New Exception(String.Format(My.Resources.AgronicaCoreInterscambioBIZ.ChiaveEsternaDoppia, Codice_Esterno))
        End If

        If dataTableInterscambio.Rows.Count = 0 Then
            result = False
        Else
            result = True
        End If

        Return result

    End Function

End Class

Public Class Interscambio_Fabbricati_W

    Public Sub Scrivi_Tabella_Interscambio(Sistema_Cod As Integer,
                                            Piva As String,
                                            Sa_Cod As Integer,
                                            Fabbricato_Cod As Integer,
                                            Codice_Esterno As String,
                                            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            )

        'transazione

        'controllo che le chiavi passate siano formattate correttamente
        If Piva = "" OrElse Sa_Cod <= 0 OrElse Fabbricato_Cod <= 0 Then
            Throw New Exception(String.Format(My.Resources.AgronicaCoreInterscambioBIZ.ChiaveGiasNonValida, Piva, Sa_Cod, Fabbricato_Cod))
        End If

        'non controllo che ci sia chiave doppia in quanto l'indice sul DB lo garantisce
        Dim objReadBIZ As New Interscambio_Fabbricati_R

        Dim checkExistsChiaveEsterna As Boolean = objReadBIZ.Check_Interscambio_ChiaveEsterna(Sistema_Cod, Codice_Esterno, objParametri_Server)

        'se count = 1, occorre verificare che ke chiavi importate siano lo stesse

        'costruire codice GIAS e passarlo al DAL

        Dim Codice_Gias As String = Piva + "_" + Sa_Cod.ToString() + "_" + Fabbricato_Cod.ToString()

        'se entrambe non sono presenti nella tabella di interscambio, scrivo
        If checkExistsChiaveEsterna = False Then

            Dim objWriteDAL As New AgronicaCoreInterscambioDAL.Interscambio_Fabbricati_W

            Dim result = objWriteDAL.Scrivi(Sistema_Cod,
                                            Piva,
                                            Sa_Cod,
                                            Fabbricato_Cod,
                                            Codice_Gias,
                                            Codice_Esterno,
                                            objParametri_Server)
        End If

    End Sub

End Class
