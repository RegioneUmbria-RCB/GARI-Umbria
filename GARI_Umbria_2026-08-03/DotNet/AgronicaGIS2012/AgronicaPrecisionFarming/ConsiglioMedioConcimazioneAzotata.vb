Imports System.Xml

Public Class ConsiglioMedioConcimazioneAzotata
    'I parametri di input sono tutte stringhe in quanto mi servono solamente x produrre un xml per il web service
    'in questo modo inoltre evito eventuali problemi di tipo valori Nothing x parametri non obbligatori di tipo value
    Public Shared Function Calcola(ByVal pDoorKey As String _
                                , ByVal pUserName As String _
                                , ByVal pPwd As String _
                                , ByVal pCodiceSpecie As String _
                                , ByVal pCodiceFinalita As String _
                                , ByVal pCodiceFase As String _
                                , ByVal pCodiceEpoca As String _
                                , ByVal pDurataAnni As String _
                                , ByVal pResa As String _
                                , ByVal pCodiceSpeciePrecessione As String _
                                , ByVal pCodiceFinalitaPrecessione As String _
                                , ByVal pCodiceCondizioniTerreno As String _
                                , ByVal pSabbia As String _
                                , ByVal pLimo As String _
                                , ByVal pArgilla As String _
                                , ByVal ppH As String _
                                , ByVal pCaCO3 As String _
                                , ByVal pSO As String _
                                , ByVal pNTOT As String _
                                , ByVal pP2O5 As String _
                                , ByVal pK2O As String _
                                , ByVal pQuantita_FertReg As String _
                                , ByVal pFrequenza_FertReg As String _
                                , ByVal pN_FertReg As String _
                                , ByVal pP2O5_FertReg As String _
                                , ByVal pK2O_FertReg As String _
                                , ByVal pSS_FertReg As String _
                                , ByVal pQuantita_FertSalt As String _
                                , ByVal pN_FertSalt As String _
                                , ByVal pP2O5_FertSalt As String _
                                , ByVal pK2O_FertSalt As String _
                                , ByVal pSS_FertSalt As String _
                                , ByVal pPiovosita As String _
                                , ByVal pAreaVulnerabile As String _
                                ) As Double
        Dim XmlIn, XmlOut As String
        Dim RetVal As Double
        Dim Errore As Boolean = False

        XmlIn = XmlInput(pDoorKey _
                                , pUserName _
                                , pPwd _
                                , pCodiceSpecie _
                                , pCodiceFinalita _
                                , pCodiceFase _
                                , pCodiceEpoca _
                                , pDurataAnni _
                                , pResa _
                                , pCodiceSpeciePrecessione _
                                , pCodiceFinalitaPrecessione _
                                , pCodiceCondizioniTerreno _
                                , pSabbia _
                                , pLimo _
                                , pArgilla _
                                , ppH _
                                , pCaCO3 _
                                , pSO _
                                , pNTOT _
                                , pP2O5 _
                                , pK2O _
                                , pQuantita_FertReg _
                                , pFrequenza_FertReg _
                                , pN_FertReg _
                                , pP2O5_FertReg _
                                , pK2O_FertReg _
                                , pSS_FertReg _
                                , pQuantita_FertSalt _
                                , pN_FertSalt _
                                , pP2O5_FertSalt _
                                , pK2O_FertSalt _
                                , pSS_FertSalt _
                                , pPiovosita _
                                , pAreaVulnerabile _
                                )

        Try
            'TODO: call web service
            '...

            '-----
            'DEBUG
            If CBool(pAreaVulnerabile) Then
                'Esempio di xml senza errori
                XmlOut = "<?xml version='1.0' ?><pianoconcimazione><dati_output><azoto><quantitamassima>129,7</quantitamassima></azoto></dati_output></pianoconcimazione>"
            Else
                'Esempio di xml con errori
                XmlOut = "<?xml version='1.0' ?><pianoconcimazione><errore><codice>1</codice><messaggio>Errore x debug. X eliminare l'errore selezionare la check Area Vulnerabile.</messaggio></errore><errore><codice></codice><messaggio>Errore non specificato.</messaggio></errore><dati_output><azoto><quantitamassima>0</quantitamassima></azoto></dati_output></pianoconcimazione>"
            End If
            '/DEBUG
            '------
        Catch ex As Exception
            Errore = True
            Throw New Exception("Errore in fase di connessione al servizio per il calcolo del consiglio medio di concimazione.", ex)
        End Try

        'elaborazione output
        If Not Errore Then
            RetVal = OutputFromXml(XmlOut)
        End If

        Return RetVal
    End Function

    Private Shared Function XmlInput(ByVal pDoorKey As String _
                                , ByVal pUserName As String _
                                , ByVal pPwd As String _
                                , ByVal pCodiceSpecie As String _
                                , ByVal pCodiceFinalita As String _
                                , ByVal pCodiceFase As String _
                                , ByVal pCodiceEpoca As String _
                                , ByVal pDurataAnni As String _
                                , ByVal pResa As String _
                                , ByVal pCodiceSpeciePrecessione As String _
                                , ByVal pCodiceFinalitaPrecessione As String _
                                , ByVal pCodiceCondizioniTerreno As String _
                                , ByVal pSabbia As String _
                                , ByVal pLimo As String _
                                , ByVal pArgilla As String _
                                , ByVal ppH As String _
                                , ByVal pCaCO3 As String _
                                , ByVal pSO As String _
                                , ByVal pNTOT As String _
                                , ByVal pP2O5 As String _
                                , ByVal pK2O As String _
                                , ByVal pQuantita_FertReg As String _
                                , ByVal pFrequenza_FertReg As String _
                                , ByVal pN_FertReg As String _
                                , ByVal pP2O5_FertReg As String _
                                , ByVal pK2O_FertReg As String _
                                , ByVal pSS_FertReg As String _
                                , ByVal pQuantita_FertSalt As String _
                                , ByVal pN_FertSalt As String _
                                , ByVal pP2O5_FertSalt As String _
                                , ByVal pK2O_FertSalt As String _
                                , ByVal pSS_FertSalt As String _
                                , ByVal pPiovosita As String _
                                , ByVal pAreaVulnerabile As String _
                                ) As String

        Dim XmlIn As String
        Dim XmlDoc As New XmlDocument
        Dim XmlElement As XmlNode

        XmlDoc.LoadXml("<?xml version='1.0' ?><pianoconcimazione></pianoconcimazione>")

        'sezione autenticazione
        XmlElement = AddNode(XmlDoc, XmlDoc.DocumentElement, "autenticazione")
        AddValueNode(XmlDoc, XmlElement, "doorkey", pDoorKey)
        AddValueNode(XmlDoc, XmlElement, "login", pUserName)
        AddValueNode(XmlDoc, XmlElement, "password", pPwd)

        'sezione dati
        'NOTA: utilizzo la funzione ValeTo0 per i campi non obbligatori in quanto per questi campi
        '       le specifiche di Agronica impongono che venga passato il valore 0
        XmlElement = AddNode(XmlDoc, XmlDoc.DocumentElement, "dati_input")
        AddValueNode(XmlDoc, XmlElement, "colturaprincipale_veg_cod", pCodiceSpecie)
        AddValueNode(XmlDoc, XmlElement, "dettagli_finalita_grfi_cod", pCodiceFinalita)
        AddValueNode(XmlDoc, XmlElement, "faseciclocolturale_id_fase", pCodiceFase)
        AddValueNode(XmlDoc, XmlElement, "ciclo_ep_cod", pCodiceEpoca)
        AddValueNode(XmlDoc, XmlElement, "listdurataanni", pDurataAnni)
        AddValueNode(XmlDoc, XmlElement, "resa", pResa)
        AddValueNode(XmlDoc, XmlElement, "precessione_veg_cod", pCodiceSpeciePrecessione)
        AddValueNode(XmlDoc, XmlElement, "finalitaprecessione_grfi_cod", pCodiceFinalitaPrecessione)
        AddValueNode(XmlDoc, XmlElement, "condizionidelterreno_id_terreno", ValueTo0(pCodiceCondizioniTerreno))
        AddValueNode(XmlDoc, XmlElement, "sabbia", pSabbia)
        AddValueNode(XmlDoc, XmlElement, "limo", pLimo)
        AddValueNode(XmlDoc, XmlElement, "argilla", pArgilla)
        AddValueNode(XmlDoc, XmlElement, "ph", ppH)
        AddValueNode(XmlDoc, XmlElement, "caco3", pCaCO3)
        AddValueNode(XmlDoc, XmlElement, "so", pSO)
        AddValueNode(XmlDoc, XmlElement, "ntot", pNTOT)
        AddValueNode(XmlDoc, XmlElement, "p2o5", pP2O5)
        AddValueNode(XmlDoc, XmlElement, "k2o", pK2O)
        AddValueNode(XmlDoc, XmlElement, "quantita", ValueTo0(pQuantita_FertReg))
        AddValueNode(XmlDoc, XmlElement, "frequenza", ValueTo0(pFrequenza_FertReg))
        AddValueNode(XmlDoc, XmlElement, "fn", ValueTo0(pN_FertReg))
        AddValueNode(XmlDoc, XmlElement, "fp2o5", ValueTo0(pP2O5_FertReg))
        AddValueNode(XmlDoc, XmlElement, "fk2o", ValueTo0(pK2O_FertReg))
        AddValueNode(XmlDoc, XmlElement, "fss", ValueTo0(pSS_FertReg))
        AddValueNode(XmlDoc, XmlElement, "quantita1", ValueTo0(pQuantita_FertSalt))
        AddValueNode(XmlDoc, XmlElement, "f1n", ValueTo0(pN_FertSalt))
        AddValueNode(XmlDoc, XmlElement, "f1p2o5", ValueTo0(pP2O5_FertSalt))
        AddValueNode(XmlDoc, XmlElement, "f1k2o", ValueTo0(pK2O_FertSalt))
        AddValueNode(XmlDoc, XmlElement, "f1ss", ValueTo0(pSS_FertSalt))
        AddValueNode(XmlDoc, XmlElement, "piovosita", pPiovosita)
        AddValueNode(XmlDoc, XmlElement, "areavulnerabile", pAreaVulnerabile)

        XmlIn = XmlDoc.OuterXml()

        'DEBUG
        'Try
        '    Dim sw As New System.IO.StreamWriter("D:\SITI_HTTP\CitimapMaster\Temp\xmlin.txt")
        '    sw.Write(XmlIn)
        '    sw.Close()
        'Catch ex As Exception

        'End Try
        'DEBUG

        Return XmlIn
    End Function

    Private Shared Function OutputFromXml(ByVal strXmlOut As String) As Double
        Dim XmlDoc As XmlDocument
        Dim RetVal As Double
        Dim NodiErrore, NodiRisultato As Xml.XmlNodeList
        Dim NodoErrore As Xml.XmlNode
        Dim Ex As Exception
        Dim i As Integer
        Dim Msg As String

        XmlDoc = New XmlDocument()
        XmlDoc.LoadXml(strXmlOut)
        'Suppongo che la stringa xml sia formattata come segue
        '
        '<pianoconcimazione>
        '   <errore>
        '       <codice></codice>
        '       <messaggio></messaggio>
        '   </errore>
        '   <dati_output>
        '       <azoto>
        '	        <quantitamassima></quantitamassima>
        '       </azoto>
        '   </dati_output>
        '</pianoconcimazione>
        '
        ' dove il tag <errore> può essere multiplo ed è opzionale

        'Verifico che non ci siano errori
        NodiErrore = XmlDoc.DocumentElement.GetElementsByTagName("errore")
        If Not NodiErrore Is Nothing AndAlso NodiErrore.Count > 0 Then
            'Genero un eccezione con i dettagli degli errori riportati nell'xml
            For i = 0 To NodiErrore.Count - 1
                NodoErrore = NodiErrore.Item(i)
                Try
                    Msg = "Codice: " & NodoErrore.ChildNodes(0).InnerText & " "
                    Msg &= "Descrizione: " & NodoErrore.ChildNodes(1).InnerText
                    If Not Ex Is Nothing Then
                        Ex = New Exception(Msg, Ex)
                    Else
                        Ex = New Exception(Msg)
                    End If
                Catch ex1 As Exception

                End Try
            Next
            If Not Ex Is Nothing Then
                Throw New Exception("Si sono verificati alcuni errori nel calcolo della quantità annua massima di concime.", Ex)
            Else
                Throw New Exception("Si sono verificati alcuni errori nel calcolo della quantità annua massima di concime.")
            End If
        End If

        NodiRisultato = XmlDoc.GetElementsByTagName("dati_output")
        If NodiRisultato Is Nothing OrElse NodiRisultato.Count = 0 Then
            Throw New Exception("La procedura per il calcolo della quantità annua massima di concime non ha restituito alcun risultato.")
        End If

        RetVal = CType(NodiRisultato.Item(0).ChildNodes(0).ChildNodes(0).InnerText, Double)

        Return RetVal
    End Function

    Private Shared Function AddNode(ByRef pXmlDoc As XmlDocument _
                                , ByRef pParentNode As XmlElement _
                                , ByVal pName As String _
                                ) As XmlElement

        Dim xeValue As XmlElement = pXmlDoc.CreateElement(pName)

        pParentNode.AppendChild(xeValue)
        Return xeValue
    End Function

    Private Shared Sub AddValueNode(ByRef pXmlDoc As XmlDocument _
                                , ByRef pParentNode As XmlElement _
                                , ByVal pName As String _
                                , ByVal pValue As String _
                                )

        Dim xeValue As XmlElement = AddNode(pXmlDoc, pParentNode, pName)

        xeValue.InnerText = pValue
    End Sub

    Private Shared Function ValueTo0(ByVal pValue As String) As String
        If pValue <> String.Empty Then
            Return pValue
        Else
            Return "0"
        End If
    End Function
End Class
