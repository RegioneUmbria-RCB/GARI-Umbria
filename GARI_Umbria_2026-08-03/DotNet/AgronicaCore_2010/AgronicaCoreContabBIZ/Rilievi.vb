
Imports InData.Operazione

Public Class Rilievi_R
    Inherits AgronicaCoreDataProvider.LogProvider

    Public Function LeggiRilieviNew(leggiRilievi As LeggiRilievi,
                                    ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                    ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim dt As DataTable
        Dim rilievi_Dal As New AgronicaCoreContabDAL.Rilievi_R
        Dim Piva As String = ""
        Dim SaCod As String = ""
        Dim VegCod As Integer = -1
        Dim IdCod As Integer = -1
        Dim LavCod As Integer = 0
        Dim validitaInizio As DateTime = New DateTime(1900, 1, 1)
        Dim validitaFine As DateTime = New DateTime(2100, 12, 31)

        If leggiRilievi IsNot Nothing Then

            If leggiRilievi.AziendaRilievi IsNot Nothing Then
                Piva = leggiRilievi.AziendaRilievi.partitaIva
            End If

            If leggiRilievi.CentroAziendaleRilievi IsNot Nothing Then
                SaCod = leggiRilievi.CentroAziendaleRilievi.primaryKey.codice
            End If

            If (leggiRilievi.SpecieRilievi IsNot Nothing) Then
                If leggiRilievi.SpecieRilievi.classType = "Varieta" Then
                    VegCod = CType(leggiRilievi.SpecieRilievi, AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta).specie.codice
                Else
                    IdCod = leggiRilievi.SpecieRilievi.codice
                End If
            End If

            If leggiRilievi.TipoRilievi IsNot Nothing Then
                LavCod = leggiRilievi.TipoRilievi.codice
            End If

            validitaInizio = leggiRilievi.Da
            validitaFine = leggiRilievi.A

        End If

        dt = rilievi_Dal.Leggi_Rilievi_New(Piva, SaCod, VegCod, IdCod, LavCod, validitaInizio, validitaFine, objParametri_Server, objParametri_Utenti)

        'If dt.Rows.Count = 0 Then
        '    Throw New Exception("Nessun Rilievo")
        'End If

        Return dt

    End Function

End Class
