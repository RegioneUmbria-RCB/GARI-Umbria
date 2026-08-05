Imports AgronicaCoreDataProvider
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDTOStd.InData.Notifiche
Imports Newtonsoft.Json
Imports AgronicaCoreDTOStd.InData

Public Class DataPublish_Poligoni_NonValidi_R


End Class
Public Class DataPublish_Poligoni_NonValidi_W
    Public Function RegistraNuovoPoligonoInvalido(ByVal Id_Notifica_SistemaEsterno As Integer,
                                                   ByVal CUAA As String,
                                                   ByVal Campagna As Integer,
                                                   ByVal Id_Appezzamento_Esterno As Long,
                                                  ByVal srid As Integer,
                                                   ByVal Poligono_WKT As String,
                                                   ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim xResp As Boolean = False
        Dim xWriter As New AgronicaCoreVarieDAL.DataPublish_Poligoni_NonValidi_W
        Dim xReader As New AgronicaCoreVarieDAL.DataPublish_Poligoni_NonValidi_R
        Dim messaggioErrore As String = ""

        Try
            Dim scriviPoligono As Boolean = True
            Dim dt = xReader.Leggi(0, Id_Notifica_SistemaEsterno, CUAA, Campagna, Id_Appezzamento_Esterno, "", "", objParametriServer)
            If dt.Rows.Count > 0 Then
                For Each row In dt.Rows
                    If row("Poligono_WKT") = Poligono_WKT Then
                        scriviPoligono = False
                        Exit For
                    End If
                Next
            End If
            If scriviPoligono Then
                xResp = xWriter.Scrivi(Id_Notifica_SistemaEsterno,
                                 CUAA,
                                 Campagna,
                                 Id_Appezzamento_Esterno,
                                 srid,
                                 Poligono_WKT,
                                 objParametriServer)
            Else
                'per non far scattare eccezione in caso abbia già un poligono uguale con gli stessi riferimenti di quello che si sta elaborando
                xResp = True
            End If
        Catch ex As Exception
            xResp = False
            Throw ex
        End Try
        Return xResp
    End Function

End Class
