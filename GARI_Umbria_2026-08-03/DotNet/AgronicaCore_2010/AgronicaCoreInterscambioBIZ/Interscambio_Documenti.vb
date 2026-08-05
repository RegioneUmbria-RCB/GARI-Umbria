Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class Interscambio_Documenti_R

    Public Function Leggi_Tabella_Interscambio_ChiaveGIAS(Sistema_Cod As Integer,
                                                          Piva As String,
                                                          Cod_Contatto As String,
                                                          ID_Elenco As Integer,
                                                          ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim objReadDAL As New AgronicaCoreInterscambioDAL.Interscambio_Documenti_R

        Return objReadDAL.Leggi(Sistema_Cod, Piva, Cod_Contatto, ID_Elenco, "", AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server)

    End Function

    Public Function Leggi_Tabella_Interscambio_ChiaveEsterna(Sistema_Cod As Integer,
                                                             Codice_Esterno As String,
                                                             ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim objReadDAL As New AgronicaCoreInterscambioDAL.Interscambio_Documenti_R

        Return objReadDAL.Leggi(Sistema_Cod, "", "", 0, Codice_Esterno, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server)

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

Public Class Interscambio_Documenti_W

    Public Sub Scrivi_Tabella_Interscambio(Sistema_Cod As Integer,
                                           Piva As String,
                                           Cod_Contatto As String,
                                           ID_Elenco As Integer,
                                           Codice_Esterno As String,
                                           ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            )

        'transazione

        'controllo che le chiavi passate siano formattate correttamente
        If Piva = "" OrElse Cod_Contatto = "" OrElse ID_Elenco <= 0 Then
            Throw New Exception(String.Format(My.Resources.AgronicaCoreInterscambioBIZ.ChiaveGiasNonValida, Piva, ID_Elenco))
        End If

        'non controllo che ci sia chiave doppia in quanto l'indice sul DB lo garantisce
        Dim objReadBIZ As New Interscambio_Documenti_R

        Dim checkExistsChiaveEsterna As Boolean = objReadBIZ.Check_Interscambio_ChiaveEsterna(Sistema_Cod, Codice_Esterno, objParametri_Server)

        'se count = 1, occorre verificare che ke chiavi importate siano lo stesse

        'costruire codice GIAS e passarlo al DAL

        Dim Codice_Gias As String = Piva + "_" + Cod_Contatto + "_" + ID_Elenco.ToString()

        'se entrambe non sono presenti nella tabella di interscambio, scrivo
        If checkExistsChiaveEsterna = False Then

            Dim objWriteDAL As New AgronicaCoreInterscambioDAL.Interscambio_Documenti_W

            Dim result = objWriteDAL.Scrivi(Sistema_Cod,
                                            Piva,
                                            Cod_Contatto,
                                            ID_Elenco,
                                            Codice_Gias,
                                            Codice_Esterno,
                                            objParametri_Server)
        End If

    End Sub

    Public Sub Elimina_Tabella_Interscambio(Sistema_Cod As Integer,
                                           Piva As String,
                                           Cod_Contatto As String,
                                           ID_Elenco As Integer,
                                           Codice_Esterno As String,
                                           ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim objWriteDAL As New AgronicaCoreInterscambioDAL.Interscambio_Documenti_W
        objWriteDAL.Elimina(Sistema_Cod,
                            Piva,
                            Cod_Contatto,
                            ID_Elenco,
                            Codice_Esterno,
                            objParametri_Server)

    End Sub

    Public Function Elimina_Tabella_Interscambio(Sistema_Cod As Integer,
                                           Codice_Esterno As String,
                                           ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim objWriteDAL As New AgronicaCoreInterscambioDAL.Interscambio_Documenti_W
        Dim retVal As Boolean = objWriteDAL.Elimina(Sistema_Cod, Codice_Esterno, objParametri_Server)
        Return retVal

    End Function

End Class
