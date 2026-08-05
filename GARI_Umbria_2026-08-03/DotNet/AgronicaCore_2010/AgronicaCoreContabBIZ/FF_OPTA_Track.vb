
Imports <xmlns="http://www.agronica.it/track/">
Imports AgronicaCoreContabDAL
Imports AgronicaCoreDataProvider


Public Class FF_OPTA_Track


    Public Sub TrackImpiantiFromBarcode(
        ByVal Barcode As String,
        ByVal percentuale_contributo_precedente As Decimal,
        ByVal ordine As Integer,
        ByVal progressivo As Integer,
        ByVal trk As FF_Trace_R,
        ByRef elem As XElement,
        ByVal idAg As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
        ByVal AggiungiDettaglioAziendaAppezza As Boolean
        )

        Dim dtTrKOperazioniConf As DataTable

        Dim trkOpta As New AgronicaCoreContabDAL.FF_Opta_Track

        Dim outpuNode = <output></output>

        If Not String.IsNullOrEmpty(idAg) Then
            idAg = " A.Lav_cod <> 125 and A.id_agenda not in (" & idAg & ")"
        Else
            idAg = " A.Lav_cod <> 125 "
        End If

        dtTrKOperazioniConf = trkOpta.LeggiOPAgendaSuImpiantiDatoBarcode(
            Barcode,
            idAg,
            "",
            objParametri
        )



        For Each dd As DataRow In dtTrKOperazioniConf.Rows

            Dim lPercentuale_contributo As Decimal
            lPercentuale_contributo = dd("percentuale_contributo") * percentuale_contributo_precedente

            Dim nn = <trasformazione_dati>
                         <datiGias>
                             <piva><%= dd("piva") %></piva>
                             <sa_cod><%= dd("sa_cod") %></sa_cod>
                             <id_agenda><%= dd("id_agenda") %></id_agenda>
                             <lav_cod><%= dd("lav_Cod") %></lav_cod>
                             <Des_lib><%= dd("Des_lib") %></Des_lib>
                             <Data_Movimento><%= dd("Data_Movimento") %></Data_Movimento>
                             <id_mov><%= dd("id_mov") %></id_mov>
                             <id_mov_det><%= dd("id_mov_det") %></id_mov_det>
                             <Elem_Cod><%= dd("Elem_Cod") %></Elem_Cod>
                             <Pro_Cod><%= dd("Pro_cod") %></Pro_Cod>
                             <mat_cod><%= dd("mat_Cod") %></mat_cod>
                             <udm_cod><%= dd("udm_cod") %></udm_cod>
                             <cal_Cod><%= dd("cal_cod") %></cal_Cod>
                             <Lotto><%= dd("Lotto") %></Lotto>
                             <qta><%= dd("qta") %></qta>
                             <percentuale_contributo><%= lPercentuale_contributo %></percentuale_contributo>
                             <ordine><%= ordine %></ordine>
                             <progressivo><%= progressivo %></progressivo>

                         </datiGias>
                     </trasformazione_dati>

            elem.<trasformazioni>.FirstOrDefault.Add(nn)

            If dd("percentuale_contributo") > 1 Then
                lPercentuale_contributo = percentuale_contributo_precedente
            End If



        Next



    End Sub



End Class


