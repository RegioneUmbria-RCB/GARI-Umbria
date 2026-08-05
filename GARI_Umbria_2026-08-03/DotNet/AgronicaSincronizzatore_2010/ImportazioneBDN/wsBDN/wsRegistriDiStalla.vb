Imports System.IO
Imports System.ServiceModel
Imports System.Xml
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class ChiamawsRegistriDiStalla
    Inherits WSBDN

    Dim soap_Autenticazione As wsRegistriUpd.SOAPAutenticazione
    Dim ws As wsRegistriUpd.wsRegistriUpdSoapClient
    Public Sub New(objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri, token As String, ruolo As String, valore_ruolo_codice As String)
        MyBase.New(objParametri_Server, objParametri_Utenti, token, ruolo, valore_ruolo_codice)
        initializeWebService()
    End Sub

    Public Sub New(objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri, link As String, username As String, password As String, token As String, ruolo As String, valore_ruolo_codice As String)
        MyBase.New(objParametri_Server, objParametri_Utenti, link, username, password, token, ruolo, valore_ruolo_codice)
        initializeWebService()
    End Sub

    Private Sub initializeWebService()
        serviceEnpoint &= "wsRegistriDiStalla/wsRegistriUpd.asmx"
        soap_Autenticazione = New wsRegistriUpd.SOAPAutenticazione
        ws = New wsRegistriUpd.wsRegistriUpdSoapClient(binding, theEndpoint)
    End Sub

    Public Function Insert_Aziende(
                                   P_INGRESSO_ID As Integer,
                                    P_INS_VAR As String,
                                    P_NUM_MODELLO As String,
                                    P_DATA_EMISSIONE_MODELLO As String,
                                    P_CAPO_CODICE As String,
                                    P_SPECIE_DESTINAZIONE_CODICE As String,
                                    P_AZIENDA_DESTINAZIONE_CODICE As String,
                                    P_ALLEV_DESTINAZIONE_ID_FISCALE As String,
                                    P_SPECIE_ORIGINE_CODICE As String,
                                    P_AZIENDA_ORIGINE_CODICE As String,
                                    P_ALLEVAMENTO_ORIGINE_IDFISCALE As String,
                                    P_ASL_CODICE As String,
                                    P_DISTRETTO_CODICE As String,
                                    P_MOTIVO As String,
                                    P_DATA_INGRESSO As String,
                                    P_DATA_COMUNICAZIONE_INGRESSO As String,
                                    P_NOTE_REGISTRO As String,
                                    P_ID_FISCALE_SOCC As String,
                                    P_DT_INIZIO_SOCCIDA As String
                                   )
        Try
            Dim a = SoapRequest(Reflection.MethodBase.GetCurrentMethod().Name, New Dictionary(Of String, String) From {
                                {"P_INGRESSO_ID", P_INGRESSO_ID},
                                {"P_INS_VAR", P_INS_VAR},
                                {"P_NUM_MODELLO", P_NUM_MODELLO},
                                {"P_DATA_EMISSIONE_MODELLO", P_DATA_EMISSIONE_MODELLO},
                                {"P_CAPO_CODICE", P_CAPO_CODICE},
                                {"P_SPECIE_DESTINAZIONE_CODICE", P_SPECIE_DESTINAZIONE_CODICE},
                                {"P_AZIENDA_DESTINAZIONE_CODICE", P_AZIENDA_DESTINAZIONE_CODICE},
                                {"P_ALLEV_DESTINAZIONE_ID_FISCALE", P_ALLEV_DESTINAZIONE_ID_FISCALE},
                                {"P_SPECIE_ORIGINE_CODICE", P_SPECIE_ORIGINE_CODICE},
                                {"P_AZIENDA_ORIGINE_CODICE", P_AZIENDA_ORIGINE_CODICE},
                                {"P_ALLEVAMENTO_ORIGINE_IDFISCALE", P_ALLEVAMENTO_ORIGINE_IDFISCALE},
                                {"P_ASL_CODICE", P_ASL_CODICE},
                                {"P_DISTRETTO_CODICE", P_DISTRETTO_CODICE},
                                {"P_MOTIVO", P_MOTIVO},
                                {"P_DATA_INGRESSO", P_DATA_INGRESSO},
                                {"P_DATA_COMUNICAZIONE_INGRESSO", P_DATA_COMUNICAZIONE_INGRESSO},
                                {"P_NOTE_REGISTRO", P_NOTE_REGISTRO},
                                {"P_ID_FISCALE_SOCC", P_ID_FISCALE_SOCC},
                                {"P_DT_INIZIO_SOCCIDA", P_DT_INIZIO_SOCCIDA}
                                })
            Dim dt = MyBase.generateDTfromXml(a)
            Return dt
        Catch ex As ExpiredTokenBDNException
            Throw ex
        Catch ex As BDNException
            Throw New BDNException("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function Insert_Ingresso(P_INGRESSO_ID As Integer,
                                    P_INS_VAR As String,
                                    P_NUM_MODELLO As String,
                                    P_DATA_EMISSIONE_MODELLO As String,
                                    P_CAPO_CODICE As String,
                                    P_SPECIE_DESTINAZIONE_CODICE As String,
                                    P_AZIENDA_DESTINAZIONE_CODICE As String,
                                    P_ALLEV_DESTINAZIONE_ID_FISCALE As String,
                                    P_SPECIE_ORIGINE_CODICE As String,
                                    P_AZIENDA_ORIGINE_CODICE As String,
                                    P_ALLEVAMENTO_ORIGINE_IDFISCALE As String,
                                    P_ASL_CODICE As String,
                                    P_DISTRETTO_CODICE As String,
                                    P_MOTIVO As String,
                                    P_DATA_INGRESSO As String,
                                    P_DATA_COMUNICAZIONE_INGRESSO As String,
                                    P_NOTE_REGISTRO As String,
                                    P_ID_FISCALE_SOCC As String,
                                    P_DT_INIZIO_SOCCIDA As String
                                   )
        Dim dsInserimentoIngresso As New DataSet

        Try

            Dim xml_InserisciIngresso As New dsINGRESSI_IUS()
            xml_InserisciIngresso.INGRESSO.AddINGRESSORow(P_INGRESSO_ID, P_INS_VAR, P_NUM_MODELLO, P_DATA_EMISSIONE_MODELLO, P_CAPO_CODICE,
                                                          P_SPECIE_DESTINAZIONE_CODICE, P_AZIENDA_DESTINAZIONE_CODICE, P_ALLEV_DESTINAZIONE_ID_FISCALE,
                                                          P_SPECIE_ORIGINE_CODICE, P_AZIENDA_ORIGINE_CODICE, P_ALLEVAMENTO_ORIGINE_IDFISCALE,
                                                          P_ASL_CODICE, P_DISTRETTO_CODICE, P_MOTIVO, P_DATA_INGRESSO, P_DATA_COMUNICAZIONE_INGRESSO,
                                                          P_NOTE_REGISTRO, P_ID_FISCALE_SOCC, P_DT_INIZIO_SOCCIDA)

            'genera la stringa dall'xml
            Dim strXml_InserisciIngresso As String = xml_InserisciIngresso.GetXml()

            'genera la stringa con Escape dalla stringa dell'xml
            Dim esStrXml_InserisciIngresso As String = System.Security.SecurityElement.Escape(strXml_InserisciIngresso)

            Dim reqInsert As XmlNode = SoapRequestV2(esStrXml_InserisciIngresso, Reflection.MethodBase.GetCurrentMethod().Name)

            dsInserimentoIngresso = MyBase.generateDSfromXml(reqInsert)

        Catch ex As ExpiredTokenBDNException
            Throw ex
        Catch ex As BDNException
            Throw New BDNException("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)

        Catch ex As Exception
            Throw New Exception("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)

        End Try

        Return dsInserimentoIngresso

    End Function

    Public Function Delete_Ingresso(ByVal P_INGRESSO_ID As Integer,
                                    ByVal P_AZIENDA_CODICE As String,
                                    ByVal P_ALLEV_ID_FISCALE As String,
                                    ByVal P_CAPO_CODICE As String,
                                    ByVal P_CODICE_SPECIE As String,
                                    ByVal P_DT_INGRESSO As Date) As DataSet
        Dim dsCancellazioneIngresso As New DataSet

        Try

            Dim xml_CancellaIngresso As New dsINGRESSI_D()
            xml_CancellaIngresso.INGRESSO.AddINGRESSORow(P_INGRESSO_ID, P_AZIENDA_CODICE, P_ALLEV_ID_FISCALE,
                                                         P_CAPO_CODICE, P_CODICE_SPECIE, P_DT_INGRESSO)

            'genera la stringa dall'xml
            Dim strXml_CancellaIngresso As String = xml_CancellaIngresso.GetXml()

            'gestione xml
            Dim docXmlSwap As New XmlDocument
            docXmlSwap.LoadXml(strXml_CancellaIngresso)

            'conversione date in formato ("yyyy-MM-dd")
            Dim XmlNode_Date As XmlNode
            Dim root As XmlElement = docXmlSwap.DocumentElement

            'P_DT_INGRESSO
            XmlNode_Date = root.FirstChild.Item("P_DT_INGRESSO")
            If CDate(XmlNode_Date.InnerText) < AGRODATAINIZIO Then
                XmlNode_Date.InnerText = ""
            Else
                XmlNode_Date.InnerText = CDate(XmlNode_Date.InnerText).Date.ToString("yyyy-MM-dd")
            End If

            'dopo i vari aggiustamenti al dannato xml, lo salva in stringa
            Dim finalXml_CancellaIngresso As String = docXmlSwap.InnerXml

            'genera la stringa con Escape dalla stringa dell'xml
            Dim esStrXml_CancellaIngresso As String = System.Security.SecurityElement.Escape(finalXml_CancellaIngresso)

            Dim reqInsert As XmlNode = SoapRequestV2(esStrXml_CancellaIngresso, Reflection.MethodBase.GetCurrentMethod().Name)

            dsCancellazioneIngresso = MyBase.generateDSfromXml(reqInsert)

        Catch ex As ExpiredTokenBDNException
            Throw ex
        Catch ex As BDNException
            Throw New BDNException("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)
        Catch ex As Exception
            Throw New Exception("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)
        End Try

        Return dsCancellazioneIngresso

    End Function

    Public Function Insert_Uscita_Allevamento(ByVal P_USCITA_ID As Integer,
                                              ByVal P_INS_VAR As String,
                                              ByVal P_MOV_INGRESSO_ID As Integer,
                                              ByVal P_ASL_CODICE As String,
                                              ByVal P_DISTRETTO_CODICE As String,
                                              ByVal P_CAPO_CODICE As String,
                                              ByVal P_CODICE_SPECIE As String,
                                              ByVal P_AZIENDA_CODICE As String,
                                              ByVal P_ALLEV_ID_FISCALE As String,
                                              ByVal P_MOTIVO As String,
                                              ByVal P_DATA_USCITA As String,
                                              ByVal P_DATA_COMUNICAZIONE_USCITA As String,
                                              ByVal P_DATA_ULTIMO_AGGIORNAMENTO As String,
                                              ByVal P_SPECIE_DESTINAZIONE_CODICE As String,
                                              ByVal P_AZIENDA_DESTINAZIONE_CODICE As String,
                                              ByVal P_ALLEV_DESTINAZIONE_ID_FISCALE As String,
                                              ByVal P_ESTREMI_DESTINAZIONE As String,
                                              ByVal P_RIF_MODELLO As String,
                                              ByVal P_DATA_MODELLO As String,
                                              ByVal P_FLAG_RIENTRO_IN_ALLEVAMENTO As String) As DataSet
        Dim dsInserimentoCapo As New DataSet

        Try

            Dim xml_InserisciCapo As New dsUSCITE_ALL_IUS()
            xml_InserisciCapo.USCITA_ALL.AddUSCITA_ALLRow(P_USCITA_ID,
                                                        P_INS_VAR,
                                                        P_MOV_INGRESSO_ID,
                                                        P_ASL_CODICE,
                                                        P_DISTRETTO_CODICE,
                                                        P_CAPO_CODICE,
                                                        P_CODICE_SPECIE,
                                                        P_AZIENDA_CODICE,
                                                        P_ALLEV_ID_FISCALE,
                                                        P_MOTIVO,
                                                        P_DATA_USCITA,
                                                        P_DATA_COMUNICAZIONE_USCITA,
                                                        P_DATA_ULTIMO_AGGIORNAMENTO,
                                                        P_SPECIE_DESTINAZIONE_CODICE,
                                                        P_AZIENDA_DESTINAZIONE_CODICE,
                                                        P_ALLEV_DESTINAZIONE_ID_FISCALE,
                                                        P_ESTREMI_DESTINAZIONE,
                                                        P_RIF_MODELLO,
                                                        P_DATA_MODELLO,
                                                        P_FLAG_RIENTRO_IN_ALLEVAMENTO)
            'genera la stringa dall'xml
            Dim strXml_InserisciCapo As String = xml_InserisciCapo.GetXml()

            'genera la stringa con Escape dalla stringa dell'xml
            Dim esStrXml_InserisciCapo As String = System.Security.SecurityElement.Escape(strXml_InserisciCapo)

            Dim reqInsert As XmlNode = SoapRequestV2(esStrXml_InserisciCapo, Reflection.MethodBase.GetCurrentMethod().Name)

            dsInserimentoCapo = MyBase.generateDSfromXml(reqInsert)


        Catch ex As ExpiredTokenBDNException
            Throw ex
        Catch ex As BDNException
            Throw New BDNException("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)

        Catch ex As Exception
            Throw New Exception("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)

        End Try

        Return dsInserimentoCapo

    End Function

    Public Function Insert_Uscita_Macello(ByVal P_USCITA_ID As Integer,
                                          ByVal P_INS_VAR As String,
                                          ByVal P_MOV_INGRESSO_ID As Integer,
                                          ByVal P_ASL_CODICE As String,
                                          ByVal P_DISTRETTO_CODICE As String,
                                          ByVal P_CAPO_CODICE As String,
                                          ByVal P_SPECIE_CODICE As String,
                                          ByVal P_AZIENDA_CODICE As String,
                                          ByVal P_ALLEV_ID_FISCALE As String,
                                          ByVal P_MOTIVO As String,
                                          ByVal P_DATA_USCITA As String,
                                          ByVal P_DATA_COMUNICAZIONE_USCITA As String,
                                          ByVal P_SPECIE_DESTINAZIONE_CODICE As String,
                                          ByVal P_REGIONI_CODICE As String,
                                          ByVal P_MACELLO_CODICE As String,
                                          ByVal P_MACELLO_ID_FISCALE As String,
                                          ByVal P_ESTREMI_DESTINAZIONE As String,
                                          ByVal P_RIF_MODELLO As String,
                                          ByVal P_DATA_MODELLO As String,
                                          ByVal P_FLAG_RIENTRO_IN_ALLEVAMENTO As String) As DataSet
        Dim dsInserimentoCapo As New DataSet

        Try

            Dim xml_InserisciCapo As New dsUSCITE_MAC_IUS()
            xml_InserisciCapo.USCITA_MAC.AddUSCITA_MACRow(P_USCITA_ID,
                                                        P_INS_VAR,
                                                        P_MOV_INGRESSO_ID,
                                                        P_ASL_CODICE,
                                                        P_DISTRETTO_CODICE,
                                                        P_CAPO_CODICE,
                                                        P_SPECIE_CODICE,
                                                        P_AZIENDA_CODICE,
                                                        P_ALLEV_ID_FISCALE,
                                                        P_MOTIVO,
                                                        P_DATA_USCITA,
                                                        P_DATA_COMUNICAZIONE_USCITA,
                                                        P_SPECIE_DESTINAZIONE_CODICE,
                                                        P_REGIONI_CODICE,
                                                        P_MACELLO_CODICE,
                                                        P_MACELLO_ID_FISCALE,
                                                        P_ESTREMI_DESTINAZIONE,
                                                        P_RIF_MODELLO,
                                                        P_DATA_MODELLO,
                                                        P_FLAG_RIENTRO_IN_ALLEVAMENTO)
            'genera la stringa dall'xml
            Dim strXml_InserisciCapo As String = xml_InserisciCapo.GetXml()

            'genera la stringa con Escape dalla stringa dell'xml
            Dim esStrXml_InserisciCapo As String = System.Security.SecurityElement.Escape(strXml_InserisciCapo)

            Dim reqInsert As XmlNode = SoapRequestV2(esStrXml_InserisciCapo, Reflection.MethodBase.GetCurrentMethod().Name)

            dsInserimentoCapo = MyBase.generateDSfromXml(reqInsert)


        Catch ex As ExpiredTokenBDNException
            Throw ex
        Catch ex As BDNException
            Throw New BDNException("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)

        Catch ex As Exception
            Throw New Exception("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)

        End Try

        Return dsInserimentoCapo

    End Function


End Class
