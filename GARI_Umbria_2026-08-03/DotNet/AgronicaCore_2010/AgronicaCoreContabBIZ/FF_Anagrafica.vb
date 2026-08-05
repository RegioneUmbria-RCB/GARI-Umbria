
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider 
Imports AgronicaCoreVarieBIZ

Public Class FF_Anagrafica


    Public Function LeggiImpresaConParametriFFDaGerarchia(ByVal Piva As String, ByVal objPrametri_Server As AgronicaCoreParametri) As RispostaStandard

        Dim rval As New RispostaStandard
        rval.RispostaOK = false

        Try

            Dim ricercaPivaParametri As New AgronicaCoreContabDAL.FF_Anagrafica_R
            Dim trovato As Boolean

            Dim pivaCorrente As String = piva
            Dim pivaRiferimento As string = ""
            Dim dtRicerca As DataTable
            While Not trovato
                dtRicerca = 
                    ricercaPivaParametri.LeggiImpresaConParametriFFDaGerarchia(pivaCorrente, "", "", objPrametri_Server)

                If dtRicerca.Rows.Count = 0 then
                    Exit While
                End If

                pivaRiferimento  = 
                    dtRicerca(0)("pivaRiferimento")

                If pivaRiferimento <> "" Then
                    pivaCorrente = pivaRiferimento
                    trovato = true
                Else 
                    pivaCorrente = dtRicerca(0)("padre")
                End If

            End While
            'trovato
            
            
            rval.RispostaOK = true
            rval.RispostaStringa = pivaRiferimento
            


        Catch ex As Exception

            rval.RispostaOK = false
            rval.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return rval

    End Function

End Class
