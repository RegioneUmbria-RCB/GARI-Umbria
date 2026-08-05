Public Class SerializzazioneXml

    '############################################################################################################
    Public Function DeserializzaXml(ByVal _NomeFile As String) As Sincro_Agrea2Gias.MyWsAgriRER.FascicoloSiar2Response

        Dim myFascicoloSiarResponse As New Sincro_Agrea2Gias.MyWsAgriRER.FascicoloSiar2Response

        Try

            If IO.File.Exists(_NomeFile) = True Then
                Dim reader As New System.Xml.Serialization.XmlSerializer(GetType(Sincro_Agrea2Gias.MyWsAgriRER.FascicoloSiar2Response))
                Dim file As New System.IO.StreamReader(_NomeFile)

                myFascicoloSiarResponse = CType(reader.Deserialize(file), Sincro_Agrea2Gias.MyWsAgriRER.FascicoloSiar2Response)
                file.Close()
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try

        Return myFascicoloSiarResponse

    End Function



    '##########################################################################################################################################
    Public Function SerializzaXml(ByVal _NomeFile As String, ByVal myFascicoloSiarResponse As Sincro_Agrea2Gias.MyWsAgriRER.FascicoloSiar2Response)

        Dim bRet As Boolean = True      ' valore di ritorno della funzione

        Try

            If _NomeFile <> "" Then
                Dim writer As New System.Xml.Serialization.XmlSerializer(GetType(Sincro_Agrea2Gias.MyWsAgriRER.FascicoloSiar2Response))
                Dim file As New System.IO.StreamWriter(_NomeFile)
                writer.Serialize(file, myFascicoloSiarResponse)
                file.Close()

            End If

        Catch ex As Exception
            bRet = False
            MsgBox(ex.ToString, MsgBoxStyle.Critical)
        End Try

        Return bRet

    End Function


    '############################################################################################################
    Public Function DeserializzaXmlEffluenti(ByVal _NomeFile As String) As Sincro_Agrea2Gias.MyWsEffluentiRER.EffluentiResponse

        Dim myFascicoloSiarResponse As New Sincro_Agrea2Gias.MyWsEffluentiRER.EffluentiResponse

        Try

            If IO.File.Exists(_NomeFile) = True Then
                Dim reader As New System.Xml.Serialization.XmlSerializer(GetType(Sincro_Agrea2Gias.MyWsEffluentiRER.EffluentiResponse))
                Dim file As New System.IO.StreamReader(_NomeFile)

                myFascicoloSiarResponse = CType(reader.Deserialize(file), Sincro_Agrea2Gias.MyWsEffluentiRER.EffluentiResponse)
                file.Close()
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try

        Return myFascicoloSiarResponse

    End Function




End Class
