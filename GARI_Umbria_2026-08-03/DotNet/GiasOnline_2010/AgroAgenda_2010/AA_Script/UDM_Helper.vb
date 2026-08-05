Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider

Public Class UDM_Helper

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


    Public Shared Sub CambiaIntestazioneGridview(ByVal index As Integer, ByVal intestazioneGridView As String, ByVal datoDaSostituire As String, ByRef gw As GridView, ByVal StartFromIDX As Integer)

        Dim trovato As Boolean = False

        If index = 0 Then
            For Each c As DataColumn In CType(gw.DataSource, DataTable).Columns

                If c.ColumnName.ToLower = intestazioneGridView.ToLower Then
                    trovato = True
                    Exit For
                End If

                StartFromIDX += 1



            Next
        Else
            trovato = True
            StartFromIDX = index
        End If

        If trovato Then
            gw.Columns(StartFromIDX).HeaderText = String.Format(gw.Columns(StartFromIDX).HeaderText, datoDaSostituire)
        End If



    End Sub


End Class
