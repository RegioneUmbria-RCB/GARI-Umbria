Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDomandaIrriguaDAL
Public Class DatiAggiuntiviDocumenti_R
    Inherits AgronicaCoreDataProvider.DataProvider
    Public Function GetDatiAzienda(ByVal piva As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As AgronicaCoreDTOStd.InData.DomandaIrrigua.DatiAzienda
        Dim ret As New AgronicaCoreDTOStd.InData.DomandaIrrigua.DatiAzienda

        Dim nomeRoutine As String = "AgronicaCoreDomandaIrriguaBIZ.DomandaIrrigua_R.GetDatiAzienda()"

        Dim messaggioErrore As String = ""

        Try
            Dim imp_dal As New AgronicaCoreAnagrafeDAL.Imprese_Read

            Dim dtImp = imp_dal.Leggi_3(piva, True, 1, 0, True, False, False, False, False, False, False, False, False, False, "", "", objParametri)
            If dtImp.Rows.Count > 0 Then
                ret.PivaReale = dtImp.Rows(0)("PivaReale")
                ret.CUAA = dtImp.Rows(0)("CUAA")
                ret.RagioneSociale = dtImp.Rows(0)("Rag_Soc")
                ret.CodiceFiscale = ""
                ret.Cognome = ""
                ret.Nome = ""
                ret.indirizzo = New AgronicaCoreDTOStd.InData.DomandaIrrigua.IndirizzoAzienda() With {
                                                        .Via = dtImp.Rows(0)("ind_des"),
                                                        .Frazione = dtImp.Rows(0)("frz_des"),
                                                        .Provincia = dtImp.Rows(0)("pro_cod"),
                                                        .Comune = dtImp.Rows(0)("com_des"),
                                                        .CAP = dtImp.Rows(0)("CAP"),
                                                        .Stato = dtImp.Rows(0)("Stato")
                                                    }
            End If

        Catch ex As Exception
            Throw New Exception(ex.Message, ex.InnerException)
        End Try
        Return ret
    End Function
End Class
