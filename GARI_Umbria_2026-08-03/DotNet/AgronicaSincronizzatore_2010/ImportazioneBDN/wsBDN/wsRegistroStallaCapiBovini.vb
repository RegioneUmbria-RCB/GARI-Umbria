Imports System.IO
Imports System.ServiceModel
Imports System.Xml
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class ChiamaWs_RegistroCapiStalla
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
        serviceEnpoint &= "wsCapiBovini/wsCapiUpd.asmx"
        soap_Autenticazione = New wsRegistriUpd.SOAPAutenticazione
        ws = New wsRegistriUpd.wsRegistriUpdSoapClient(binding, theEndpoint)

    End Sub

    Public Function Insert_Capi(ByVal P_CAPO_ID As Integer,
                                ByVal P_INS_VAR As String,
                                ByVal P_CAPO_CODICE_IDENTIFICATIVO As String,
                                ByVal P_SESSO As String,
                                ByVal P_RAZZA_CODICE As String,
                                ByVal P_DT_NASCITA As String,
                                ByVal P_COD_MARCHIO_MADRE As String,
                                ByVal P_COD_MARCHIO_PADRE As String,
                                ByVal P_TAG As String,
                                ByVal P_CODICE_PRECEDENTE As String,
                                ByVal P_DT_INIZIO_LATTAZIONE As String,
                                ByVal P_DT_FINE_LATTAZIONE As String,
                                ByVal P_DT_APPLICAZIONE_MARCHIO As String,
                                ByVal P_DT_ISCRIZIONE_IN_ANAGRAFE As String,
                                ByVal P_DT_COMPILAZIONE_CEDOLA As String,
                                ByVal P_FLAG_INSEMINAZIONE As String,
                                ByVal P_TIPO_ORIGINE As String,
                                ByVal P_CODICE_PARTITE_ANIMO As String,
                                ByVal P_CODICE_STATI As String,
                                ByVal P_ALLEV_ID_FISCALE As String,
                                ByVal P_AZIENDA_CODICE As String,
                                ByVal P_MARCHE_PRODOTTE_CODICE As String,
                                ByVal P_CODICE_LIBRO As String,
                                ByVal P_CODICE_SPECIE As String,
                                ByVal P_ID_FISCALE_DETEN As String,
                                ByVal P_CODICE_AZIENDA_DETEN As String,
                                ByVal P_CODICE_SPECIE_DETEN As String,
                                ByVal P_DT_INGRESSO_DETEN As String,
                                ByVal P_ASL_CODICE As String,
                                ByVal P_DISTRETTO_CODICE As String,
                                ByVal P_DT_INGRESSO As String,
                                ByVal P_MOTIVO_INGRESSO As String,
                                ByVal P_DT_RILASCIO_PASSAPORTO As String,
                                ByVal P_TIPO_PASSAPORTO As String,
                                ByVal P_NUMERO_RIF_LOCALE As String,
                                ByVal P_ID_FISCALE_MARCATORE As String,
                                ByVal P_COD_MADRE_GENETICA As String,
                                ByVal P_RIF_MODELLO As String,
                                ByVal P_DT_MODELLO As String,
                                ByVal P_CODICE_AZIENDA_PROV As String,
                                ByVal P_ID_FISCALE_PROV As String,
                                ByVal P_CODICE_SPECIE_PROV As String,
                                ByVal P_CODICE_FM_PROV As String,
                                ByVal P_CODICE_STATI_ORIGINE As String) As DataSet
        Dim dsInserimentoCapo As New DataSet

        Try
            Dim xml_InserisciCapo As New dsCAPI_IUS()
            xml_InserisciCapo.CAPO.AddCAPORow(P_CAPO_ID, P_INS_VAR, P_CAPO_CODICE_IDENTIFICATIVO, P_SESSO,
                                              P_RAZZA_CODICE, CDate(P_DT_NASCITA), P_COD_MARCHIO_MADRE, P_COD_MARCHIO_PADRE,
                                              P_TAG, P_CODICE_PRECEDENTE, CDate(P_DT_INIZIO_LATTAZIONE), CDate(P_DT_FINE_LATTAZIONE),
                                              CDate(P_DT_APPLICAZIONE_MARCHIO), CDate(P_DT_ISCRIZIONE_IN_ANAGRAFE),
                                              CDate(P_DT_COMPILAZIONE_CEDOLA), P_FLAG_INSEMINAZIONE, P_TIPO_ORIGINE,
                                              P_CODICE_PARTITE_ANIMO, P_CODICE_STATI, P_ALLEV_ID_FISCALE,
                                              P_AZIENDA_CODICE, P_MARCHE_PRODOTTE_CODICE, P_CODICE_LIBRO, P_CODICE_SPECIE,
                                              P_ID_FISCALE_DETEN, P_CODICE_AZIENDA_DETEN, P_CODICE_SPECIE_DETEN,
                                              CDate(P_DT_INGRESSO_DETEN), P_ASL_CODICE, P_DISTRETTO_CODICE, CDate(P_DT_INGRESSO),
                                              P_MOTIVO_INGRESSO, CDate(P_DT_RILASCIO_PASSAPORTO), P_TIPO_PASSAPORTO,
                                              P_NUMERO_RIF_LOCALE, P_ID_FISCALE_MARCATORE, P_COD_MADRE_GENETICA,
                                              P_RIF_MODELLO, CDate(P_DT_MODELLO), P_CODICE_AZIENDA_PROV, P_ID_FISCALE_PROV,
                                              P_CODICE_SPECIE_PROV, P_CODICE_FM_PROV, P_CODICE_STATI_ORIGINE)

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

    Public Function Insert_Capi_Moviment(ByVal P_CAPO_ID As Integer,
                                ByVal P_INS_VAR As String,
                                ByVal P_CAPO_CODICE_IDENTIFICATIVO As String,
                                ByVal P_SESSO As String,
                                ByVal P_RAZZA_CODICE As String,
                                ByVal P_DT_NASCITA As String,
                                ByVal P_COD_MARCHIO_MADRE As String,
                                ByVal P_COD_MARCHIO_PADRE As String,
                                ByVal P_TAG As String,
                                ByVal P_CODICE_PRECEDENTE As String,
                                ByVal P_DT_INIZIO_LATTAZIONE As String,
                                ByVal P_DT_FINE_LATTAZIONE As String,
                                ByVal P_DT_APPLICAZIONE_MARCHIO As String,
                                ByVal P_DT_ISCRIZIONE_IN_ANAGRAFE As String,
                                ByVal P_DT_COMPILAZIONE_CEDOLA As String,
                                ByVal P_FLAG_INSEMINAZIONE As String,
                                ByVal P_TIPO_ORIGINE As String,
                                ByVal P_CODICE_PARTITE_ANIMO As String,
                                ByVal P_CODICE_STATI As String,
                                ByVal P_ALLEV_ID_FISCALE As String,
                                ByVal P_AZIENDA_CODICE As String,
                                ByVal P_MARCHE_PRODOTTE_CODICE As String,
                                ByVal P_CODICE_LIBRO As String,
                                ByVal P_CODICE_SPECIE As String,
                                ByVal P_ID_FISCALE_DETEN As String,
                                ByVal P_CODICE_AZIENDA_DETEN As String,
                                ByVal P_CODICE_SPECIE_DETEN As String,
                                ByVal P_DT_INGRESSO_DETEN As String,
                                ByVal P_ASL_CODICE As String,
                                ByVal P_DISTRETTO_CODICE As String,
                                ByVal P_DT_INGRESSO As String,
                                ByVal P_MOTIVO_INGRESSO As String,
                                ByVal P_DT_RILASCIO_PASSAPORTO As String,
                                ByVal P_TIPO_PASSAPORTO As String,
                                ByVal P_NUMERO_RIF_LOCALE As String,
                                ByVal P_ID_FISCALE_MARCATORE As String,
                                ByVal P_COD_MADRE_GENETICA As String,
                                ByVal P_RIF_MODELLO As String,
                                ByVal P_DT_MODELLO As String,
                                ByVal P_CODICE_AZIENDA_PROV As String,
                                ByVal P_ID_FISCALE_PROV As String,
                                ByVal P_CODICE_SPECIE_PROV As String,
                                ByVal P_CODICE_FM_PROV As String,
                                ByVal P_CODICE_STATI_ORIGINE As String) As DataSet
        Dim dsInserimentoCapo As New DataSet

        Try
            Dim xml_InserisciCapo As New dsCAPI_IUS()
            xml_InserisciCapo.CAPO.AddCAPORow(P_CAPO_ID, P_INS_VAR, P_CAPO_CODICE_IDENTIFICATIVO, P_SESSO,
                                              P_RAZZA_CODICE, CDate(P_DT_NASCITA), P_COD_MARCHIO_MADRE, P_COD_MARCHIO_PADRE,
                                              P_TAG, P_CODICE_PRECEDENTE, CDate(P_DT_INIZIO_LATTAZIONE), CDate(P_DT_FINE_LATTAZIONE),
                                              CDate(P_DT_APPLICAZIONE_MARCHIO), CDate(P_DT_ISCRIZIONE_IN_ANAGRAFE),
                                              CDate(P_DT_COMPILAZIONE_CEDOLA), P_FLAG_INSEMINAZIONE, P_TIPO_ORIGINE,
                                              P_CODICE_PARTITE_ANIMO, P_CODICE_STATI, P_ALLEV_ID_FISCALE,
                                              P_AZIENDA_CODICE, P_MARCHE_PRODOTTE_CODICE, P_CODICE_LIBRO, P_CODICE_SPECIE,
                                              P_ID_FISCALE_DETEN, P_CODICE_AZIENDA_DETEN, P_CODICE_SPECIE_DETEN,
                                              CDate(P_DT_INGRESSO_DETEN), P_ASL_CODICE, P_DISTRETTO_CODICE, CDate(P_DT_INGRESSO),
                                              P_MOTIVO_INGRESSO, CDate(P_DT_RILASCIO_PASSAPORTO), P_TIPO_PASSAPORTO,
                                              P_NUMERO_RIF_LOCALE, P_ID_FISCALE_MARCATORE, P_COD_MADRE_GENETICA,
                                              P_RIF_MODELLO, CDate(P_DT_MODELLO), P_CODICE_AZIENDA_PROV, P_ID_FISCALE_PROV,
                                              P_CODICE_SPECIE_PROV, P_CODICE_FM_PROV, P_CODICE_STATI_ORIGINE)

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

    'Public Function Insert_Ingresso(ByVal P_INGRESSO_ID As Integer,
    '                                ByVal P_INS_VAR As String,
    '                                ByVal P_NUM_MODELLO As String,
    '                                ByVal P_DATA_EMISSIONE_MODELLO As String,
    '                                ByVal P_CAPO_CODICE As String,
    '                                ByVal P_SPECIE_DESTINAZIONE_CODICE As String,
    '                                ByVal P_AZIENDA_DESTINAZIONE_CODICE As String,
    '                                ByVal P_ALLEV_DESTINAZIONE_ID_FISCALE As String,
    '                                ByVal P_SPECIE_ORIGINE_CODICE As String,
    '                                ByVal P_AZIENDA_ORIGINE_CODICE As String,
    '                                ByVal P_ALLEVAMENTO_ORIGINE_IDFISCALE As String,
    '                                ByVal P_ASL_CODICE As String,
    '                                ByVal P_DISTRETTO_CODICE As String,
    '                                ByVal P_MOTIVO As String,
    '                                ByVal P_DATA_INGRESSO As String,
    '                                ByVal P_DATA_COMUNICAZIONE_INGRESSO As String,
    '                                ByVal P_NOTE_REGISTRO As String,
    '                                ByVal P_ID_FISCALE_SOCC As String,
    '                                ByVal P_DT_INIZIO_SOCCIDA As Date) As DataSet
    '    Dim dsInserimentoIngresso As New DataSet

    '    Try

    '        Dim xml_InserisciIngresso As New dsIngressi_IUS()
    '        xml_InserisciIngresso.INGRESSO.AddINGRESSORow(P_INGRESSO_ID, P_INS_VAR, P_NUM_MODELLO, P_DATA_EMISSIONE_MODELLO, P_CAPO_CODICE,
    '                                                      P_SPECIE_DESTINAZIONE_CODICE, P_AZIENDA_DESTINAZIONE_CODICE, P_ALLEV_DESTINAZIONE_ID_FISCALE,
    '                                                      P_SPECIE_ORIGINE_CODICE, P_AZIENDA_ORIGINE_CODICE, P_ALLEVAMENTO_ORIGINE_IDFISCALE,
    '                                                      P_ASL_CODICE, P_DISTRETTO_CODICE, P_MOTIVO, P_DATA_INGRESSO, P_DATA_COMUNICAZIONE_INGRESSO,
    '                                                      P_NOTE_REGISTRO, P_ID_FISCALE_SOCC, P_DT_INIZIO_SOCCIDA)

    '        'genera la stringa dall'xml
    '        Dim strXml_InserisciIngresso As String = "" xml_InserisciIngresso.GetXml()

    '        'genera la stringa con Escape dalla stringa dell'xml
    '        Dim esStrXml_InserisciIngresso As String = System.Security.SecurityElement.Escape(strXml_InserisciIngresso)

    '        Dim reqInsert As XmlNode = SoapRequestV2(esStrXml_InserisciIngresso, Reflection.MethodBase.GetCurrentMethod().Name)

    '        dsInserimentoIngresso = MyBase.generateDSfromXml(reqInsert)

    '    Catch ex As BDNException
    '        Throw New BDNException("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)

    '    Catch ex As Exception
    '        Throw New Exception("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)

    '    End Try

    '    Return dsInserimentoIngresso


    'End Function

    'Public Function Insert_Ingresso(ByVal P_INGRESSO_ID As Integer,
    '                                ByVal P_INS_VAR As String,
    '                                ByVal P_NUM_MODELLO As String,
    '                                ByVal P_DATA_EMISSIONE_MODELLO As String,
    '                                ByVal P_CAPO_CODICE As String,
    '                                ByVal P_SPECIE_DESTINAZIONE_CODICE As String,
    '                                ByVal P_AZIENDA_DESTINAZIONE_CODICE As String,
    '                                ByVal P_ALLEV_DESTINAZIONE_ID_FISCALE As String,
    '                                ByVal P_SPECIE_ORIGINE_CODICE As String,
    '                                ByVal P_AZIENDA_ORIGINE_CODICE As String,
    '                                ByVal P_ALLEVAMENTO_ORIGINE_IDFISCALE As String,
    '                                ByVal P_ASL_CODICE As String,
    '                                ByVal P_DISTRETTO_CODICE As String,
    '                                ByVal P_MOTIVO As String,
    '                                ByVal P_DATA_INGRESSO As String,
    '                                ByVal P_DATA_COMUNICAZIONE_INGRESSO As String,
    '                                ByVal P_NOTE_REGISTRO As String,
    '                                ByVal P_ID_FISCALE_SOCC As String,
    '                                ByVal P_DT_INIZIO_SOCCIDA As Date) As DataSet
    '    Dim dsInserimentoIngresso As New DataSet

    '    Try

    '        Dim xml_InserisciIngresso As New dsIngressi_IUS()
    '        xml_InserisciIngresso.INGRESSO.AddINGRESSORow(P_INGRESSO_ID, P_INS_VAR, P_NUM_MODELLO, P_DATA_EMISSIONE_MODELLO, P_CAPO_CODICE,
    '                                                      P_SPECIE_DESTINAZIONE_CODICE, P_AZIENDA_DESTINAZIONE_CODICE, P_ALLEV_DESTINAZIONE_ID_FISCALE,
    '                                                      P_SPECIE_ORIGINE_CODICE, P_AZIENDA_ORIGINE_CODICE, P_ALLEVAMENTO_ORIGINE_IDFISCALE,
    '                                                      P_ASL_CODICE, P_DISTRETTO_CODICE, P_MOTIVO, P_DATA_INGRESSO, P_DATA_COMUNICAZIONE_INGRESSO,
    '                                                      P_NOTE_REGISTRO, P_ID_FISCALE_SOCC, P_DT_INIZIO_SOCCIDA)

    '        'genera la stringa dall'xml
    '        Dim strXml_InserisciIngresso As String = "" xml_InserisciIngresso.GetXml()

    '        'genera la stringa con Escape dalla stringa dell'xml
    '        Dim esStrXml_InserisciIngresso As String = System.Security.SecurityElement.Escape(strXml_InserisciIngresso)

    '        Dim reqInsert As XmlNode = SoapRequestV2(esStrXml_InserisciIngresso, Reflection.MethodBase.GetCurrentMethod().Name)

    '        dsInserimentoIngresso = MyBase.generateDSfromXml(reqInsert)

    '    Catch ex As BDNException
    '        Throw New BDNException("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)

    '    Catch ex As Exception
    '        Throw New Exception("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)

    '    End Try

    '    Return dsInserimentoIngresso


    'End Function

    'Public Function Insert_Ingresso(ByVal P_INGRESSO_ID As Integer,
    '                                ByVal P_INS_VAR As String,
    '                                ByVal P_NUM_MODELLO As String,
    '                                ByVal P_DATA_EMISSIONE_MODELLO As String,
    '                                ByVal P_CAPO_CODICE As String,
    '                                ByVal P_SPECIE_DESTINAZIONE_CODICE As String,
    '                                ByVal P_AZIENDA_DESTINAZIONE_CODICE As String,
    '                                ByVal P_ALLEV_DESTINAZIONE_ID_FISCALE As String,
    '                                ByVal P_SPECIE_ORIGINE_CODICE As String,
    '                                ByVal P_AZIENDA_ORIGINE_CODICE As String,
    '                                ByVal P_ALLEVAMENTO_ORIGINE_IDFISCALE As String,
    '                                ByVal P_ASL_CODICE As String,
    '                                ByVal P_DISTRETTO_CODICE As String,
    '                                ByVal P_MOTIVO As String,
    '                                ByVal P_DATA_INGRESSO As String,
    '                                ByVal P_DATA_COMUNICAZIONE_INGRESSO As String,
    '                                ByVal P_NOTE_REGISTRO As String,
    '                                ByVal P_ID_FISCALE_SOCC As String,
    '                                ByVal P_DT_INIZIO_SOCCIDA As Date) As DataSet
    '    Dim dsInserimentoIngresso As New DataSet

    '    Try

    '        Dim xml_InserisciIngresso As New dsIngressi_IUS()
    '        xml_InserisciIngresso.INGRESSO.AddINGRESSORow(P_INGRESSO_ID, P_INS_VAR, P_NUM_MODELLO, P_DATA_EMISSIONE_MODELLO, P_CAPO_CODICE,
    '                                                      P_SPECIE_DESTINAZIONE_CODICE, P_AZIENDA_DESTINAZIONE_CODICE, P_ALLEV_DESTINAZIONE_ID_FISCALE,
    '                                                      P_SPECIE_ORIGINE_CODICE, P_AZIENDA_ORIGINE_CODICE, P_ALLEVAMENTO_ORIGINE_IDFISCALE,
    '                                                      P_ASL_CODICE, P_DISTRETTO_CODICE, P_MOTIVO, P_DATA_INGRESSO, P_DATA_COMUNICAZIONE_INGRESSO,
    '                                                      P_NOTE_REGISTRO, P_ID_FISCALE_SOCC, P_DT_INIZIO_SOCCIDA)

    '        'genera la stringa dall'xml
    '        Dim strXml_InserisciIngresso As String = "" xml_InserisciIngresso.GetXml()

    '        'genera la stringa con Escape dalla stringa dell'xml
    '        Dim esStrXml_InserisciIngresso As String = System.Security.SecurityElement.Escape(strXml_InserisciIngresso)

    '        Dim reqInsert As XmlNode = SoapRequestV2(esStrXml_InserisciIngresso, Reflection.MethodBase.GetCurrentMethod().Name)

    '        dsInserimentoIngresso = MyBase.generateDSfromXml(reqInsert)

    '    Catch ex As BDNException
    '        Throw New BDNException("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)

    '    Catch ex As Exception
    '        Throw New Exception("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)

    '    End Try

    '    Return dsInserimentoIngresso


    'End Function
    '    Return dsInserimentoIngresso


    'End Function

    Public Function Update_Capi(ByVal P_CAPO_ID As Integer,
                                ByVal P_INS_VAR As String,
                                ByVal P_CAPO_CODICE_IDENTIFICATIVO As String,
                                ByVal P_SESSO As String,
                                ByVal P_RAZZA_CODICE As String,
                                ByVal P_DT_NASCITA As String,
                                ByVal P_COD_MARCHIO_MADRE As String,
                                ByVal P_COD_MARCHIO_PADRE As String,
                                ByVal P_TAG As String,
                                ByVal P_CODICE_PRECEDENTE As String,
                                ByVal P_DT_INIZIO_LATTAZIONE As String,
                                ByVal P_DT_FINE_LATTAZIONE As String,
                                ByVal P_DT_APPLICAZIONE_MARCHIO As String,
                                ByVal P_DT_ISCRIZIONE_IN_ANAGRAFE As String,
                                ByVal P_DT_COMPILAZIONE_CEDOLA As String,
                                ByVal P_FLAG_INSEMINAZIONE As String,
                                ByVal P_TIPO_ORIGINE As String,
                                ByVal P_CODICE_PARTITE_ANIMO As String,
                                ByVal P_CODICE_STATI As String,
                                ByVal P_ALLEV_ID_FISCALE As String,
                                ByVal P_AZIENDA_CODICE As String,
                                ByVal P_MARCHE_PRODOTTE_CODICE As String,
                                ByVal P_CODICE_LIBRO As String,
                                ByVal P_CODICE_SPECIE As String,
                                ByVal P_ID_FISCALE_DETEN As String,
                                ByVal P_CODICE_AZIENDA_DETEN As String,
                                ByVal P_CODICE_SPECIE_DETEN As String,
                                ByVal P_DT_INGRESSO_DETEN As String,
                                ByVal P_ASL_CODICE As String,
                                ByVal P_DISTRETTO_CODICE As String,
                                ByVal P_DT_INGRESSO As String,
                                ByVal P_MOTIVO_INGRESSO As String,
                                ByVal P_DT_RILASCIO_PASSAPORTO As String,
                                ByVal P_TIPO_PASSAPORTO As String,
                                ByVal P_NUMERO_RIF_LOCALE As String,
                                ByVal P_ID_FISCALE_MARCATORE As String,
                                ByVal P_COD_MADRE_GENETICA As String,
                                ByVal P_RIF_MODELLO As String,
                                ByVal P_DT_MODELLO As String,
                                ByVal P_CODICE_AZIENDA_PROV As String,
                                ByVal P_ID_FISCALE_PROV As String,
                                ByVal P_CODICE_SPECIE_PROV As String,
                                ByVal P_CODICE_FM_PROV As String,
                                ByVal P_CODICE_STATI_ORIGINE As String) As DataSet
        Dim dsModificaCapo As New DataSet

        Try
            Dim xml_ModificaCapo As New dsCAPI_IUS()
            xml_ModificaCapo.CAPO.AddCAPORow(P_CAPO_ID, P_INS_VAR, P_CAPO_CODICE_IDENTIFICATIVO, P_SESSO,
                                             P_RAZZA_CODICE, CDate(P_DT_NASCITA), P_COD_MARCHIO_MADRE,
                                             P_COD_MARCHIO_PADRE, P_TAG, P_CODICE_PRECEDENTE,
                                             CDate(P_DT_INIZIO_LATTAZIONE), CDate(P_DT_FINE_LATTAZIONE),
                                             CDate(P_DT_APPLICAZIONE_MARCHIO), CDate(P_DT_ISCRIZIONE_IN_ANAGRAFE),
                                             CDate(P_DT_COMPILAZIONE_CEDOLA), P_FLAG_INSEMINAZIONE, P_TIPO_ORIGINE,
                                             P_CODICE_PARTITE_ANIMO, P_CODICE_STATI, P_ALLEV_ID_FISCALE,
                                             P_AZIENDA_CODICE, P_MARCHE_PRODOTTE_CODICE, P_CODICE_LIBRO,
                                             P_CODICE_SPECIE, P_ID_FISCALE_DETEN, P_CODICE_AZIENDA_DETEN,
                                             P_CODICE_SPECIE_DETEN, CDate(P_DT_INGRESSO_DETEN), P_ASL_CODICE,
                                             P_DISTRETTO_CODICE, CDate(P_DT_INGRESSO), P_MOTIVO_INGRESSO,
                                             CDate(P_DT_RILASCIO_PASSAPORTO), P_TIPO_PASSAPORTO, P_NUMERO_RIF_LOCALE,
                                             P_ID_FISCALE_MARCATORE, P_COD_MADRE_GENETICA, P_RIF_MODELLO,
                                             CDate(P_DT_MODELLO), P_CODICE_AZIENDA_PROV, P_ID_FISCALE_PROV,
                                             P_CODICE_SPECIE_PROV, P_CODICE_FM_PROV, P_CODICE_STATI_ORIGINE)

            'genera la stringa dall'xml
            Dim strXml_ModificaCapo As String = xml_ModificaCapo.GetXml()

            'genera la stringa con Escape dalla stringa dell'xml
            Dim esStrXml_ModificaCapo As String = System.Security.SecurityElement.Escape(strXml_ModificaCapo)

            Dim reqInsert As XmlNode = SoapRequestV2(esStrXml_ModificaCapo, Reflection.MethodBase.GetCurrentMethod().Name)

            dsModificaCapo = MyBase.generateDSfromXml(reqInsert)

        Catch ex As ExpiredTokenBDNException
            Throw ex

        Catch ex As BDNException
            Throw New BDNException("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)

        Catch ex As Exception
            Throw New Exception("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)

        End Try

        Return dsModificaCapo

    End Function

    Public Function Delete_Capi(ByVal P_CAPO_ID As Integer,
                                ByVal P_CAPO_CODICE_IDENTIFICATIVO As String,
                                ByVal P_ALLEV_ID_FISCALE As String,
                                ByVal P_AZIENDA_CODICE As String,
                                ByVal P_CODICE_SPECIE As String) As DataSet
        Dim dsEliminazioneCapo As New DataSet

        Try

            'Dim xml_CancellaCapo As New dsCAPI_D()
            'xml_CancellaCapo.CAPO.AddCAPORow(P_CAPO_ID, P_CAPO_CODICE_IDENTIFICATIVO,
            '                                 P_ALLEV_ID_FISCALE, P_AZIENDA_CODICE, P_CODICE_SPECIE)

            'genera la stringa dall'xml
            Dim strXml_CancellaCapo As String = ""  'xml_CancellaCapo.GetXml()

            'genera la stringa con Escape dalla stringa dell'xml
            Dim esStrXml_CancellaCapo As String = System.Security.SecurityElement.Escape(strXml_CancellaCapo)

            Dim reqInsert As XmlNode = SoapRequestV2(esStrXml_CancellaCapo, Reflection.MethodBase.GetCurrentMethod().Name)

            dsEliminazioneCapo = MyBase.generateDSfromXml(reqInsert)

        Catch ex As ExpiredTokenBDNException
            Throw ex

        Catch ex As BDNException
            Throw New BDNException("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)

        Catch ex As Exception
            Throw New Exception("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)

        End Try

        Return dsEliminazioneCapo

    End Function

    ''' <summary>
    ''' deleteIngresso (wsRegistriDiStalla.pdf, pg.59)
    ''' </summary>
    ''' <param name="P_INGRESSO_ID"></param>
    ''' <param name="P_AZIENDA_CODICE"></param>
    ''' <param name="P_ALLEV_ID_FISCALE"></param>
    ''' <param name="P_CAPO_CODICE"></param>
    ''' <param name="P_CODICE_SPECIE"></param>
    ''' <param name="P_DT_INGRESSO"></param>
    ''' <returns></returns>
    Public Function Delete_Ingresso(ByVal P_INGRESSO_ID As Integer,
                                    ByVal P_AZIENDA_CODICE As String,
                                    ByVal P_ALLEV_ID_FISCALE As String,
                                    ByVal P_CAPO_CODICE As String,
                                    ByVal P_CODICE_SPECIE As String,
                                    ByVal P_DT_INGRESSO As String) As DataSet
        Dim dsEliminazioneIngresso As New DataSet

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

            dsEliminazioneIngresso = MyBase.generateDSfromXml(reqInsert)

        Catch ex As ExpiredTokenBDNException
            Throw ex

        Catch ex As BDNException
            Throw New BDNException("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)

        Catch ex As Exception
            Throw New Exception("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)

        End Try

        Return dsEliminazioneIngresso

    End Function

    ''' <summary>
    ''' deleteUscita (wsRegistriDiStalla.pdf, pg.49)
    ''' </summary>
    ''' <param name="P_USCITA_ID"></param>
    ''' <param name="P_AZIENDA_CODICE"></param>
    ''' <param name="P_ALLEV_ID_FISCALE"></param>
    ''' <param name="P_CAPO_CODICE"></param>
    ''' <param name="P_CODICE_SPECIE"></param>
    ''' <param name="P_MOTIVO"></param>
    ''' <param name="P_DT_USCITA"></param>
    ''' <returns></returns>
    Public Function Delete_Uscita(ByVal P_USCITA_ID As Integer,
                                  ByVal P_AZIENDA_CODICE As String,
                                  ByVal P_ALLEV_ID_FISCALE As String,
                                  ByVal P_CAPO_CODICE As String,
                                  ByVal P_CODICE_SPECIE As String,
                                  ByVal P_MOTIVO As String,
                                  ByVal P_DT_USCITA As String) As DataSet
        Dim dsEliminazioneUscita As New DataSet

        Try

            Dim xml_CancellaUscita As New dsUSCITE_D()
            xml_CancellaUscita.USCITA.AddUSCITARow(P_USCITA_ID, P_AZIENDA_CODICE, P_ALLEV_ID_FISCALE,
                                                   P_CAPO_CODICE, P_CODICE_SPECIE, P_MOTIVO, P_DT_USCITA)

            'genera la stringa dall'xml
            Dim strXml_CancellaUscita As String = xml_CancellaUscita.GetXml()

            'gestione xml
            Dim docXmlSwap As New XmlDocument
            docXmlSwap.LoadXml(strXml_CancellaUscita)

            'conversione date in formato ("yyyy-MM-dd")
            Dim XmlNode_Date As XmlNode
            Dim root As XmlElement = docXmlSwap.DocumentElement

            'P_DT_USCITA
            XmlNode_Date = root.FirstChild.Item("P_DT_USCITA")
            If CDate(XmlNode_Date.InnerText) < AGRODATAINIZIO Then
                XmlNode_Date.InnerText = ""
            Else
                XmlNode_Date.InnerText = CDate(XmlNode_Date.InnerText).Date.ToString("yyyy-MM-dd")
            End If

            'dopo i vari aggiustamenti al dannato xml, lo salva in stringa
            Dim finalXml_CancellaUscita As String = docXmlSwap.InnerXml

            'genera la stringa con Escape dalla stringa dell'xml
            Dim esStrXml_CancellaUscita As String = System.Security.SecurityElement.Escape(finalXml_CancellaUscita)

            Dim reqInsert As XmlNode = SoapRequestV2(esStrXml_CancellaUscita, Reflection.MethodBase.GetCurrentMethod().Name)

            dsEliminazioneUscita = MyBase.generateDSfromXml(reqInsert)

        Catch ex As ExpiredTokenBDNException
            Throw ex

        Catch ex As BDNException
            Throw New BDNException("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)

        Catch ex As Exception
            Throw New Exception("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)

        End Try

        Return dsEliminazioneUscita

    End Function

    Public Function Insert_Decessi(ByVal P_DECESSO_ID As Integer,
                                    ByVal P_INS_VAR As String,
                                    ByVal P_MOVIMENTO_ID As Integer,
                                    ByVal P_ALLEV_ID_FISCALE As String,
                                    ByVal P_AZIENDA_CODICE As String,
                                    ByVal P_SPECIE_CODICE As String,
                                    ByVal P_CAPO_CODICE As String,
                                    ByVal P_CAUSALE_CODICE As String,
                                    ByVal P_DATA_MORTE As String,
                                    ByVal P_DATA_TEST_BSE As String,
                                    ByVal P_DATA_COMUNICAZIONE As String) As DataSet
        Dim dsInserimentoCapo As New DataSet

        Try

            Dim xml_InserisciCapo As New dsDECESSI_IUS()
            xml_InserisciCapo.DECESSO.AddDECESSORow(P_DECESSO_ID,
                                                    P_INS_VAR,
                                                    P_MOVIMENTO_ID,
                                                    P_ALLEV_ID_FISCALE,
                                                    P_AZIENDA_CODICE,
                                                    P_SPECIE_CODICE,
                                                    P_CAPO_CODICE,
                                                    P_CAUSALE_CODICE,
                                                    CDate(P_DATA_MORTE),
                                                    CDate(P_DATA_TEST_BSE),
                                                    CDate(P_DATA_COMUNICAZIONE))
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

    Public Function Delete_Decessi(ByVal P_DECESSO_ID As Integer,
                                   ByVal P_CAPO_CODICE As String,
                                   ByVal P_AZIENDA_CODICE As String,
                                   ByVal P_ALLEV_ID_FISCALE As String,
                                   ByVal P_SPECIE_CODICE As String) As DataSet
        Dim dsCancellazioneDecesso As New DataSet

        Try
            Dim xml_CancellaDecesso As New dsDECESSI_D()
            xml_CancellaDecesso.DECESSO.AddDECESSORow(P_DECESSO_ID, P_CAPO_CODICE, P_AZIENDA_CODICE,
                                                      P_ALLEV_ID_FISCALE, P_SPECIE_CODICE)
            'genera la stringa dall'xml
            Dim strXml_CancellaDecesso As String = xml_CancellaDecesso.GetXml()

            'genera la stringa con Escape dalla stringa dell'xml
            Dim esStrXml_CancellaDecesso As String = System.Security.SecurityElement.Escape(strXml_CancellaDecesso)

            Dim reqInsert As XmlNode = SoapRequestV2(esStrXml_CancellaDecesso, Reflection.MethodBase.GetCurrentMethod().Name)

            dsCancellazioneDecesso = MyBase.generateDSfromXml(reqInsert)

        Catch ex As ExpiredTokenBDNException
            Throw ex
        Catch ex As BDNException
            Throw New BDNException("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)
        Catch ex As Exception
            Throw New Exception("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)
        End Try

        Return dsCancellazioneDecesso

    End Function

    Private Function SwapXmlNode(ByVal XmlNode_A As XmlNode, ByVal XmlNode_B As XmlNode)

    End Function

End Class