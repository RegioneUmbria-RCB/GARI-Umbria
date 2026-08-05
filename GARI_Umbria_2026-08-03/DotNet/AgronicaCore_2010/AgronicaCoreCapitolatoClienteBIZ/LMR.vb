Imports AgronicaCoreCapitolatoClienteDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.UtilityProvider

Imports <xmlns="http://ws_CapitolatoCliente_ElencoTabellaRMA">


Public Class LMR

    Public Function get_LMR(ByVal TAbellaRMA_COD As Integer, ByVal Pa_cod As Integer, ByVal Fam_cod As Integer, ByVal Veg_Cod As Integer, ByVal data As Date, ByVal objParametri As AgronicaCoreParametri) As Decimal
        Dim tbl As DataTable
        Dim hlp As New RMA_R
        tbl = hlp.Get_RMA(TAbellaRMA_COD, Pa_cod, Fam_cod, Veg_Cod, data, objParametri)

        If (Not IsNothing(tbl)) AndAlso tbl.Rows.Count > 0 Then
            Return tbl.Rows(0)("RMA")
        Else
            Return -1
        End If

    End Function

    Public Function get_ListaDiTabellaLMR(ByVal PivaSuperUser As String, ByVal objParametri As AgronicaCoreParametri) As String
        Dim tbl As DataTable
        Dim hlp As New RMA_R
        tbl = hlp.Get_TabellaRMA(PivaSuperUser, objParametri)

        Dim xmlDoc = <?xml version="1.0"?>
                     <root xmlns="http://ws_CapitolatoCliente_ElencoTabellaRMA">
                     </root>

        For Each dr As DataRow In tbl.Rows
            Dim xmlnode = <tabella_rma>
                              <tabellaRma_cod><%= dr("tabellaRma_cod") %></tabellaRma_cod>
                              <tabellaRma_Des><%= dr("TabellaRMA_Des") %></tabellaRma_Des>
                          </tabella_rma>

            xmlDoc.<root>.FirstOrDefault.Add(xmlnode)

        Next

        Return xmlDoc.ToString

    End Function


End Class
