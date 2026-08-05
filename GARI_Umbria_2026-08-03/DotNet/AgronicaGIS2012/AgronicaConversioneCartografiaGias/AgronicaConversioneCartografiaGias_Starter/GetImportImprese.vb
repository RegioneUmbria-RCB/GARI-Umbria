Imports <xmlns="">

Imports AgronicaCoreModello
Imports GIAS2GIAS_LOCALE

Public Class GetImportImprese

    Public Shared Function getImprese(ByVal imprese As XDocument) As List(Of clsImpresa)
        Dim listOfDataImport As List(Of clsImpresa) = _
            (From d In imprese.<dati>.<impresa> _
        Select New clsImpresa With { _
                 .Piva_ORIGINE = d.<origine>.Value, _
                 .Piva_DESTINAZIONE = d.<destinazione>.Value, _
                 .PivaPadre_DESTINAZIONE = d.<padre>.Value, _
                 .RagioneSociale = d.<rag_soc>.Value, _
                 .Opzionale_Sa_Cod_Origine = CInt(d.<sa_cod>.Value) _
             }).ToList

        Return listOfDataImport
    End Function



End Class
