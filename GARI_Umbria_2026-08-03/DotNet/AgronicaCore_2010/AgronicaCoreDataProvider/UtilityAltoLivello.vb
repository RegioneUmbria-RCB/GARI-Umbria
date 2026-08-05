Imports System.Data
Imports System.Data.OleDb
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class Utility
    Public Shared Sub VerificaApriTransazione(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                      ByRef Flag_ConnessioneLocale As Boolean, _
                                      ByRef Flag_TransazioneLocale As Boolean)
        VerificaApriConnessione(objParametri, Flag_ConnessioneLocale)
        'controllo la connessione
        If IsNothing(objParametri.objTransazione) Then
            Flag_TransazioneLocale = True
            ApriTransazione(objParametri)
        End If
    End Sub
    Public Shared Sub VerificaApriConnessione(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                      ByRef Flag_ConnessioneLocale As Boolean)
        'controllo la connessione
        If IsNothing(objParametri.objConnessione) Then
            Flag_ConnessioneLocale = True
        Else
            If objParametri.objConnessione.State <> ConnectionState.Open Then
                Flag_ConnessioneLocale = True
            End If
        End If
        If Flag_ConnessioneLocale = True Then
            ApriConnessione(objParametri)
        End If
    End Sub
    Public Shared Sub ApriConnessione(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)
        'Creo la connessione localmente
        objParametri.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
        objParametri.objConnessione.Open()
        objParametri.objTransazione = Nothing
    End Sub
    Public Shared Sub ApriTransazione(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)
        'Creo la connessione localmente
        objParametri.objTransazione = objParametri.objConnessione.BeginTransaction
    End Sub



    Public Shared Sub VerificaChiudiTransazione(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                  ByRef Flag_TransazioneLocale As Boolean)

        If Flag_TransazioneLocale = True Then
            objParametri.objTransazione.Commit()
            objParametri.objTransazione = Nothing
        End If
    End Sub

    Public Shared Sub VerificaAnnullaTransazione(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                 ByRef Flag_TransazioneLocale As Boolean)
        'controllo la connessione
        If Flag_TransazioneLocale = True Then
            If objParametri.objTransazione IsNot Nothing Then
                objParametri.objTransazione.Rollback()
                objParametri.objTransazione = Nothing
            End If
        End If
    End Sub
    Public Shared Sub VerificaChiudiConnessione(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                      ByRef Flag_ConnessioneLocale As Boolean)
        If Flag_ConnessioneLocale = True Then
            objParametri.objConnessione.Close()
            objParametri.objConnessione = Nothing
        End If
    End Sub




    Public Shared Function convertStringtoOBJparametri(ByVal objparametriString As String) As AgronicaCoreDataProvider.AgronicaCoreParametri
        'to do 
        'modifica con il decrypt della string 
        Dim str = Stringa_Decodifica_LANCompatibile(
                            objparametriString,
                            AgroKey_EncoderDecoder)

        'Dim obj As JObject = JsonConvert.DeserializeObject(str)
        'Return obj.ToObject(Of AgronicaCoreDataProvider.AgronicaCoreParametri)()
        Dim obj As AgronicaCoreParametri = JsonConvert.DeserializeObject(Of AgronicaCoreParametri)(str)

        ' decodifica la stringa di connessione per evitare che dia errore quando si eseguono task paralleli
        ' dal momento che in quel contesto non sono definite le variabili application
        If obj.StringaConnessioneEncrypted Then
            obj.StringaConnessione = obj.StringaConnessione
        End If

        Return obj
    End Function

    Public Shared Sub convertStringtoOBJ(ByVal objparametriString As String, ByRef objType As Type, ByRef obj As Object)
        'to do 
        'modifica con il decrypt della string 
        Dim str = Stringa_Decodifica_LANCompatibile(
                            objparametriString,
                            AgroKey_EncoderDecoder)
        obj = JsonConvert.DeserializeObject(str, objType)
    End Sub

    Public Shared Function convertOBJparametritoString(ByVal objparametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String
        'to do 
        'modifica con il decrypt della string 
        Dim str As String = JsonConvert.SerializeObject(objparametri)
        Dim Address_Crypt As String = Stringa_Codifica_LANCompatibile(
                            str,
                            AgroKey_EncoderDecoder)

        Return Address_Crypt
    End Function

    Public Shared Function convertOBJtoString(ByVal obj As Object, crypt As Boolean) As String
        'to do 
        'modifica con il decrypt della string 
        Dim str As String = JsonConvert.SerializeObject(obj)
        Dim returnVal As String = ""
        If crypt Then
            Dim Address_Crypt As String = Stringa_Codifica_LANCompatibile(
                            str,
                            AgroKey_EncoderDecoder)
        Else
            returnVal = str
        End If

        Return returnVal
    End Function


End Class