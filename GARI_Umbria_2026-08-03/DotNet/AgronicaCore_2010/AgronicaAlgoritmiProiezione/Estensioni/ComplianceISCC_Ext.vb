
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDTOStd.InData.Gis
Imports Newtonsoft.Json

Public Class ComplianceISCC_Ext
    Implements ICheckListExtension

    Public Sub New()
    End Sub

    Public Function Esegui(ByVal Esecuzione_cod As Integer,
                           ByVal Esecuzione_GUID As String,
                           ByVal ParametriEsecuzione As String,
                           ByVal Intersection As String,
                           ByRef ObjParametri As AgronicaCoreParametri) As Boolean Implements ICheckListExtension.Esegui
        Dim ret As Boolean = False

        Dim xISCCw As New AgronicaCoreGisBIZ.Compliance_ISCC
        Dim xPLr As New AgronicaCoreGisDAL.ProiezioniLayer_R
        Dim xEntr As New AgronicaCoreGisDAL.GIS_Entita_R

        Try
            'deserializza i parametri
            Dim ObjParamExec = JsonConvert.DeserializeObject(Of ParametriAggiuntiviAlgoritmiCartografici_ISCC)(ParametriEsecuzione)

            'recupero i dati della entita collegata alla richiesta di elaborazione (entita_cod_1)

            Dim dtExec = xPLr.LeggiEsecuzioneDaCodice(Esecuzione_cod, ObjParametri)
            If dtExec.Rows.Count <= 0 Then
                Throw New Exception(String.Format("Nessuna richiesta elaborazione trovata per il guid {0}", Esecuzione_GUID))
            Else
                Dim dtEnt = xEntr.LeggiCompleto_Entita_cod(dtExec.Rows(0)("Entita_cod_1"), "", "", ObjParametri)
                If dtEnt.Rows.Count <= 0 Then
                    Throw New Exception(String.Format("Nessuna entita gis trovata per il codice {0}", dtExec.Rows(0)("Entita_cod_1")))
                Else
                    'aggiorna esito richiesta ComplianceISCC
                    ret = xISCCw.VerificaAggiornaStatoElaborazioneRicheistaElaborazioneISCC(ObjParamExec.Elaborazione_ID,
                                                                                            dtEnt.Rows(0)("piva"),
                                                                                            dtEnt.Rows(0)("sa_cod"),
                                                                                            dtEnt.Rows(0)("appezza"),
                                                                                            dtEnt.Rows(0)("id_imp"),
                                                                                            Esecuzione_cod,
                                                                                            If(Intersection.Equals(""), 1, 0),
                                                                                            ObjParametri)
                End If
            End If



        Catch ex As Exception
            ret = False
        End Try

        Return ret
    End Function
End Class
