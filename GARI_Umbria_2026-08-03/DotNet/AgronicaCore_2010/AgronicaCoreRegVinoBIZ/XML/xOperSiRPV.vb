
Imports AgronicaCoreDataProvider
Imports AgronicaCoreRegVinoDAL
Imports AgronicaCoreUtility

Public Class xOperSiRPV

    Dim reader As xDBOperazioniSiRPV_R
    Dim writer As xDBOperazioniSiRPV_W
    Dim teleregistriManager As TeleregistriManager
    Dim sendAllAttr As Boolean

    Private ObjParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    Private ObjParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri

    Public Sub New(server As AgronicaCoreDataProvider.AgronicaCoreParametri, utenti As AgronicaCoreDataProvider.AgronicaCoreParametri, ByVal checkObligatory As Boolean, manager As TeleregistriManager)
        ObjParametri_Server = server
        ObjParametri_Utenti = utenti
        teleregistriManager = manager
        reader = New xDBOperazioniSiRPV_R()
        writer = New xDBOperazioniSiRPV_W()
        Me.sendAllAttr = sendAllAttr
    End Sub

    Public Function sOperSiRPV(ByVal Username As String, ByVal Password As String, _
                                      ByVal codiciOperazioni As List(Of String), _
                                      ByVal tipoRichiesta As Integer, _
                                      ByVal codIcqrf As String, _
                                      ByVal codOperCF As String, _
                                      ByVal codOperPersonaFisica As Boolean) As String
        If True Then
            Dim xml = Utility.getIntestazione(Username, Password, Utility.OperSiRPV)

            Dim oper As New OperSiRPVInput
            oper.CodOper = Utility.getCodOper(codOperCF, codOperPersonaFisica)
            oper.TipoRichiesta = tipoRichiesta
            oper.CodiceIcqrf = codIcqrf
            oper.Operazione = getOperazioniObj(codiciOperazioni)
            'Dim listaTipi() As Type = {GetType(ProdottoGiin1)}

            Dim req = XMLUtility.getBodyRequest(oper, "wsm", "http://cooperazione.sian.it/schema/wsmrga/")
            XMLUtility.addBody(xml, req, Utility.getSoapenv)
            Utility.cleanXmlSend(xml)
            Return xml.ToString
        Else
            Dim xml = Utility.getIntestazione(Username, Password, Utility.OperSiRPV)


            Dim oper As New OperSiRPVInput

            'oper.CodOper = Utility.getCodOper(codOperCF, codOperPersonaFisica)

            'oper.CodiceIcqrf = codIcqrf

            'oper.TipoRichiesta = tipoRichiesta

            'oper.Operazione = getOperazioni(codiciOperazioni)

            'oper.Operazione = getOperazioniFasulle()


            Dim xmlO = "<OperSiRPVInput>" &
                    "<CodOper>"
            If codOperPersonaFisica Then
                xmlO += "<PersonaFisica>" & codOperCF & "</PersonaFisica>"
            Else
                xmlO += "<PersonaGiuridica>" & codOperCF & "</PersonaGiuridica>"
            End If
            xmlO += "</CodOper>"
            xmlO += "<CodiceIcqrf>" & codIcqrf & "</CodiceIcqrf>"
            xmlO += "<TipoRichiesta>"
            If tipoRichiesta = 0 Then
                xmlO += "I"
            Else
                xmlO += "A"
            End If
            xmlO += "</TipoRichiesta>"
            xmlO += getOperazioni(codiciOperazioni)

            xmlO += "</OperSiRPVInput>"

            'Dim req = Utility.getBodyRequest(oper)
            Dim xel As XElement = XElement.Parse(xmlO)
            AgronicaGIS2012.Commons.xmlHelper.SetDefaultNamespace(xel, Utility.getWsm)
            XMLUtility.addBody(xml, xel.ToString, Utility.getSoapenv)
            Utility.cleanXmlSend(xml)
            Return xml.ToString
        End If
    End Function

    Public Function sGetOperSiRPV(ByVal Username As String, ByVal Password As String, _
                                      ByVal idTrasmissione As String) As String
        Dim xml = Utility.getIntestazione(Username, Password, Utility.GetOperSiRPV)

        Dim getOper As New GetOperSiRPVInput
        getOper.IdTrasmissione = idTrasmissione

        Dim req = XMLUtility.getBodyRequest(getOper, "wsm", "http://cooperazione.sian.it/schema/wsmrga/")

        XMLUtility.addBody(xml, req, Utility.getSoapenv)
        Utility.cleanXmlSend(xml)
        Return xml.ToString

    End Function

    Public Function sCancOperSiRPV(ByVal Username As String, ByVal Password As String, _
                                          ByVal codIcqrf As String, _
                                          ByVal codOperCF As String, _
                                          ByVal codOperPersonaFisica As Boolean, _
                                          ByVal codiciOperazioni As List(Of String)) As String

        Dim xml = Utility.getIntestazione(Username, Password, Utility.CancOperSiRPV)

        Dim cancOper As New CancOperSiRPVInput

        cancOper.CodOper = Utility.getCodOper(codOperCF, codOperPersonaFisica)
        cancOper.CodiceIcqrf = codIcqrf
        cancOper.OperElimina = getOperazioniElimina(codiciOperazioni)

        Dim req = XMLUtility.getBodyRequest(cancOper, "wsm", "http://cooperazione.sian.it/schema/wsmrga/")

        XMLUtility.addBody(xml, req, Utility.getSoapenv)
        Utility.cleanXmlSend(xml)
        Return xml.ToString

    End Function

    Public Function sGetCancOperSiRPV(ByVal Username As String, ByVal Password As String, ByVal idTrasmissione As String) As String
        Dim xml = Utility.getIntestazione(Username, Password, Utility.GetCancOperSiRPV)

        Dim getcancOper As New GetCancOperSiRPVInput()
        getcancOper.IdTrasmissione = idTrasmissione

        Dim req = XMLUtility.getBodyRequest(getcancOper, "wsm", "http://cooperazione.sian.it/schema/wsmrga/")

        XMLUtility.addBody(xml, req, Utility.getSoapenv)
        Utility.cleanXmlSend(xml)
        Return xml.ToString
    End Function

#Region "Supporto alle Operazioni"

    Private Function getOperazioni(codiciOperazioni As List(Of String)) As String
        'Dim operazioni As New List(Of OperazioneSpec)
        'For Each codiceOperazione As String In codiciOperazioni
        '    operazioni.Add(getOperazione(codiceOperazione))
        'Next
        'Return operazioni.ToArray
        Dim xmlS As String = ""
        For Each codiceOperazione As String In codiciOperazioni
            Try
                xmlS += getOperazione(codiceOperazione)
                writer.aggiornaStato(ObjParametri_Server, codiceOperazione, Utility.StatoGIAS.valida_per_invio)
            Catch ex As Exception
                writer.aggiornaStato(ObjParametri_Server, codiceOperazione, Utility.StatoGIAS.non_valida_per_invio)
                teleregistriManager.logga(3, "AgronicaCoreRegVinoBIZ.xOperSiRPV.getOperazioni", $"getOperazioni: {ex.Message}")
            End Try
        Next
        Return xmlS
    End Function

    Private Function getOperazioniObj(codiciOperazioni As List(Of String)) As OperazioneSpec()
        Dim operazioni As New List(Of OperazioneSpec)
        For Each CodiceOperazione As String In codiciOperazioni
            Dim errore As Boolean = False
            Try
                operazioni.Add(getOperazioneObj(CodiceOperazione, errore))
                If errore = False Then
                    writer.aggiornaStato(ObjParametri_Server, CodiceOperazione, Utility.StatoGIAS.valida_per_invio)
                Else
                    writer.aggiornaStato(ObjParametri_Server, CodiceOperazione, Utility.StatoGIAS.non_valida_per_invio)
                    teleregistriManager.logga(3, "AgronicaCoreRegVinoBIZ.xOperSiRPV.getOperazioniObj", "getOperazioniObj:Errore=true")
                End If
            Catch ex As Exception
                writer.aggiornaStato(ObjParametri_Server, CodiceOperazione, Utility.StatoGIAS.non_valida_per_invio)
                teleregistriManager.logga(3, "AgronicaCoreRegVinoBIZ.xOperSiRPV.getOperazioniObj", $"getOperazioniObj: {ex.Message}")
            End Try
        Next
        Return operazioni.ToArray
    End Function

    Private Function getOperazione(codiceOperazione As String) As String
        'Dim opspec As New OperazioneSpec
        'opspec.Item = getOperazioneSpec(codiceOperazione)
        'Return opspec

        Dim xml As String = "<Operazione>" + getOperazioneSpec(codiceOperazione).ToString + "</Operazione>"
        Dim xel As XElement = XElement.Parse(xml)
        Return xml
    End Function

    Private Function getOperazioneObj(codiceOperazione As String, ByRef errore As Boolean) As OperazioneSpec
        Dim a As New OperazioneSpec
        a.Item = getOperazioneSpecObj(codiceOperazione, errore)
        Return a
    End Function

    Private Function getOperazioneSpec(codiceOperazione As String) As XElement
        Dim op = reader.LeggiOperazione(ObjParametri_Server, codiceOperazione)
        Dim row = op.Rows(0)
        Dim operationType As String = row.Item("CodiceOperazione")
        Dim operation As String = getXmlOperazione(row)
        Dim xel As XElement = XElement.Parse(operation)
        Return xel
    End Function

    Private Function getOperazioniElimina(codiciOperazioni As List(Of String)) As OperElimina()
        Dim listOp As New List(Of OperElimina)
        For Each codiceOperazione As String In codiciOperazioni
            listOp.Add(getOperazioneElimina(codiceOperazione))
            writer.aggiornaStato(ObjParametri_Server, codiceOperazione, Utility.StatoGIAS.valida_per_invio)
        Next
        Return listOp.ToArray
    End Function

    Private Function getOperazioneElimina(codiceOperazione As String) As OperElimina
        Dim operEl As New OperElimina
        Dim op = reader.LeggiOperazione(ObjParametri_Server, CInt(codiceOperazione))
        If op.Rows.Count > 0 Then
            Dim operazione = op.Rows(0)
            Dim codiceOperazioneStr = operazione.Item("CodiceOperazione")
            Select Case codiceOperazioneStr
                Case "ARMC"
                    operEl.CodOperazione = AgronicaCoreRegVinoBIZ.CodiceOperazione.ARMC
                Case "GIIN"
                    operEl.CodOperazione = AgronicaCoreRegVinoBIZ.CodiceOperazione.GIIN
                Case "CASD"
                    operEl.CodOperazione = AgronicaCoreRegVinoBIZ.CodiceOperazione.CASD
                Case "USSD"
                    operEl.CodOperazione = AgronicaCoreRegVinoBIZ.CodiceOperazione.USSD
                Case "IMBO"
                    operEl.CodOperazione = AgronicaCoreRegVinoBIZ.CodiceOperazione.IMBO
                Case "ACID"
                    operEl.CodOperazione = AgronicaCoreRegVinoBIZ.CodiceOperazione.ACID
                Case "PIGI"
                    operEl.CodOperazione = AgronicaCoreRegVinoBIZ.CodiceOperazione.PIGI
                Case "SVIN"
                    operEl.CodOperazione = AgronicaCoreRegVinoBIZ.CodiceOperazione.SVIN
                Case "DOLC"
                    operEl.CodOperazione = AgronicaCoreRegVinoBIZ.CodiceOperazione.DOLC
                Case "TAGL"
                    operEl.CodOperazione = AgronicaCoreRegVinoBIZ.CodiceOperazione.TAGL
                Case "CERT"
                    operEl.CodOperazione = AgronicaCoreRegVinoBIZ.CodiceOperazione.CERT
                Case "DENT"
                    operEl.CodOperazione = AgronicaCoreRegVinoBIZ.CodiceOperazione.DENT
                Case "AUCO"
                    operEl.CodOperazione = AgronicaCoreRegVinoBIZ.CodiceOperazione.AUCO
                Case "PERD"
                    operEl.CodOperazione = AgronicaCoreRegVinoBIZ.CodiceOperazione.PERD
                Case "SFEC"
                    operEl.CodOperazione = AgronicaCoreRegVinoBIZ.CodiceOperazione.SFEC
                Case "AVLT"
                    operEl.CodOperazione = AgronicaCoreRegVinoBIZ.CodiceOperazione.AVLT
                Case "SPGS"
                    operEl.CodOperazione = AgronicaCoreRegVinoBIZ.CodiceOperazione.SPGS
                Case "SPAB"
                    operEl.CodOperazione = AgronicaCoreRegVinoBIZ.CodiceOperazione.SPAB
                Case "SCDS"
                    operEl.CodOperazione = AgronicaCoreRegVinoBIZ.CodiceOperazione.SCDS
                Case "ELMC"
                    operEl.CodOperazione = AgronicaCoreRegVinoBIZ.CodiceOperazione.ELMC
                Case "FRGS"
                    operEl.CodOperazione = AgronicaCoreRegVinoBIZ.CodiceOperazione.FRGS
                Case "SCZC"
                    operEl.CodOperazione = AgronicaCoreRegVinoBIZ.CodiceOperazione.SCZC
                Case "LIEL"
                    operEl.CodOperazione = AgronicaCoreRegVinoBIZ.CodiceOperazione.LIEL
                Case "EVAL"
                    operEl.CodOperazione = AgronicaCoreRegVinoBIZ.CodiceOperazione.EVAL
                Case "FRAB"
                    operEl.CodOperazione = AgronicaCoreRegVinoBIZ.CodiceOperazione.FRAB
                Case "AARD"
                    operEl.CodOperazione = AgronicaCoreRegVinoBIZ.CodiceOperazione.AARD
                Case "DISA"
                    operEl.CodOperazione = AgronicaCoreRegVinoBIZ.CodiceOperazione.DISA
                Case "BABS"
                    operEl.CodOperazione = AgronicaCoreRegVinoBIZ.CodiceOperazione.BABS
                Case "ETIC"
                    operEl.CodOperazione = AgronicaCoreRegVinoBIZ.CodiceOperazione.ETIC
                Case "SUPE"
                    operEl.CodOperazione = AgronicaCoreRegVinoBIZ.CodiceOperazione.SUPE
                Case "DERI"
                    operEl.CodOperazione = AgronicaCoreRegVinoBIZ.CodiceOperazione.DERI
                Case "DIST"
                    operEl.CodOperazione = AgronicaCoreRegVinoBIZ.CodiceOperazione.DIST
                Case "APRT"
                    operEl.CodOperazione = AgronicaCoreRegVinoBIZ.CodiceOperazione.APRT
                Case "TRSO"
                    operEl.CodOperazione = AgronicaCoreRegVinoBIZ.CodiceOperazione.TRSO
                Case "ACET"
                    operEl.CodOperazione = AgronicaCoreRegVinoBIZ.CodiceOperazione.ACET
                Case Else
                    Return Nothing
            End Select
            operEl.DataOperazione = CDate(operazione.Item("DataOperazione"))
            operEl.NumOperazione = CInt(operazione.Item("NumOperazione"))
        End If


        Return operEl
    End Function

    Private Function getXmlOperazione(operazione As DataRow) As String
        Dim operationCode As String = operazione.Item("CodiceOperazione")
        Dim xml As String = "<" + operationCode + ">"
        'Inserisco dati generali sull'operazione
        Dim listaAttributiOperazione = reader.getAttributiProdotto(ObjParametri_Server, operationCode, "", "operazione")
        For Each attributo In listaAttributiOperazione
            xml += getXmlCampo(attributo, operationCode, "", "operazione", operazione)
        Next
        xml += getXmlProdotti(operazione.Item("ws_RegVino_Operazione_cod"), operationCode)

        xml += "</" + operationCode + ">"
        Return xml
    End Function

    Private Function getXmlProdotti(codiceOperazione As String, operationCode As String) As String
        Dim xml As String = ""
        Dim dt As DataTable = reader.leggiProdotti(ObjParametri_Server, codiceOperazione)
        For Each row As DataRow In dt.Rows
            xml += getXmlProdotto(row, operationCode)
        Next
        Return xml
    End Function

    Private Function getXmlProdotto(row As DataRow, operationCode As String) As String
        Dim prodottoCode As String = row.Item("ElementoXML")
        Dim xml As String = "<" + prodottoCode + ">"
        'Inserisco dati sul Prodotto
        'xml += "<Prodotto>"
        xml += getDesignazioneProdotto(row, operationCode, prodottoCode)
        'xml += "</Prodotto>"
        Dim listaAttributiProdotto = reader.getAttributiProdotto(ObjParametri_Server, operationCode, prodottoCode, "prodotto")
        For Each attributo As String In listaAttributiProdotto
            xml += getXmlCampo(attributo, operationCode, prodottoCode, "prodotto", row)
        Next

        Select Case operationCode
            Case Utility.IMBO
                xml += getXmlTipoPartita("TipoPartitaImbo", operationCode, prodottoCode, row)
            Case Utility.ETIC
                xml += getXmlTipoPartita("TipoPartitaImbo", operationCode, prodottoCode, row)
            Case Utility.DERI
                xml += getXmlTipoPartita("TipoPartitaImbo", operationCode, prodottoCode, row)
            Case Else
                xml += getXmlTipoPartita("TipoPartita", operationCode, prodottoCode, row)
        End Select

        'xml += getCodiceValueProdotto("CodRecipiente", CStr(row.Item("Vasi")), operationCode, prodottoCode, Utility.CodRecipiente)

        'xml += getLinearValueProdotto("Quantita", CDbl(row.Item("Qta")), operationCode, prodottoCode, Utility.Quantita)

        xml += "</" + prodottoCode + ">"
        Return xml
    End Function

    Private Function getDesignazioneProdotto(row As DataRow, operationCode As String, prodottoCode As String) As String
        Dim tagName = row.Item("Designazione")
        Dim xml As String = "<" + tagName + ">"
        xml += "<Designazione>"
        Dim listaAttributi As List(Of String)
        listaAttributi = reader.getAttributiProdotto(ObjParametri_Server, operationCode, prodottoCode, "designazione")
        If listaAttributi IsNot Nothing Then
            For Each attributo As String In listaAttributi
                Dim tagXML As String = ""
                Dim colName As String = ""
                Dim tipolineare As Boolean = True
                Dim toSend = True
                xml += getXmlCampo(attributo, operationCode, prodottoCode, "designazione", row)
            Next
        End If
        xml += "</Designazione>"
        xml += "</" + tagName + ">"
        Return xml
    End Function

    Private Function getXmlCampo(attributo As String, operationCode As String, prodottoCode As String, locoXml As String, row As DataRow) As String
        Dim xml As String = ""
        Dim DT As DataTable
        DT = reader.leggiCodificaAttributo(ObjParametri_Server, attributo, locoXml)
        Dim tagXML As String = DT.Rows(0).Item("ElementoXml")
        Dim colName As String = DT.Rows(0).Item("Colonna")
        Dim tipo As String = Split(DT.Rows(0).Item("Tipo"), "|")(1)
        Dim tipoLineare As String = Split(DT.Rows(0).Item("Tipo"), "|")(2)

        If colName <> "" Then
            Select Case tipoLineare
                Case "linear"
                    Dim valore As String = ""
                    If IsDBNull(row.Item(colName)) = False Then
                        valore = CStr(row.Item(colName))
                    End If
                    Select Case tipo
                        Case "double"
                            valore = valore.Replace(",", ".")
                        Case "int"
                            Try
                                Dim i = CInt(valore)
                                valore = CStr(i)
                            Catch ex As Exception

                            End Try
                    End Select
                    xml += getLinearValueProdotto(tagXML, valore, operationCode, prodottoCode, attributo)
                Case "code"
                    Select Case tipo
                        Case "TipoPartita"



                    End Select
                    Dim valore As String = ""
                    If IsDBNull(row.Item(colName)) = False Then
                        valore = CStr(row.Item(colName))
                    End If
                    xml += getCodiceValueProdotto(tagXML, valore, operationCode, prodottoCode, attributo)
                Case Else
                    xml += getLinearValueProdotto(tagXML, CStr(row.Item(colName)), operationCode, prodottoCode, attributo)
            End Select
        End If
        Return xml
    End Function

    Private Function getLinearValueProdotto(ByRef tagValue As String, _
                                    ByRef value As Object, _
                                    ByRef operationCode As String, _
                                    ByRef prodottoCode As String, _
                                    ByRef dbCode As String) As String
        Dim xml As String = ""
        If reader.toSendProdotto(ObjParametri_Server, operationCode, prodottoCode, dbCode, sendAllAttr) Then
            If (Not value Is Nothing) Then
                Dim valueStr = CStr(value)
                If valueStr <> "" Then
                    xml += "<" + tagValue + ">"
                    xml += valueStr
                    xml += "</" + tagValue + ">"
                End If
            End If

        End If
        Return xml
    End Function

    Private Function getLinearValueOperazione(ByRef tagValue As String, _
                                    ByRef value As Object, _
                                    ByRef operationCode As String, _
                                    ByRef dbCode As String) As String
        Dim xml As String = ""
        If reader.toSendProdotto(ObjParametri_Server, operationCode, dbCode, sendAllAttr) Then
            If (Not value Is Nothing) Then
                Try
                    Dim valueStr = CStr(value)
                    If valueStr <> "" Then
                        xml += "<" + tagValue + ">"
                        xml += valueStr
                        xml += "</" + tagValue + ">"
                    End If
                Catch ex As Exception
                    Return xml
                End Try
            End If
        End If
        Return xml
    End Function

    Private Function getCodiceValueProdotto(ByRef tagValue As String, _
                                    ByRef value As Object, _
                                    ByRef operationCode As String, _
                                    ByRef prodottoCode As String, _
                                    ByRef dbCode As String) As String
        Dim xml As String = ""
        If reader.toSendProdotto(ObjParametri_Server, operationCode, prodottoCode, dbCode, sendAllAttr) Then
            For Each cod As String In CStr(value).Split(Utility.separator)
                Dim codice = cod.Trim()
                If Not codice Is Nothing Then
                    If codice <> "" Then
                        xml += "<" + tagValue + ">"
                        xml += "<Codice>" + codice + "</Codice>"
                        xml += "</" + tagValue + ">"
                    End If
                End If
            Next
        End If
        Return xml
    End Function

    Private Function getCodiceValueOperazione(ByRef tagValue As String, _
                                    ByRef value As Object, _
                                    ByRef operationCode As String, _
                                    ByRef dbCode As String) As String
        Dim xml As String = ""
        If reader.toSendProdotto(ObjParametri_Server, operationCode, dbCode, sendAllAttr) Then
            For Each cod As String In CStr(value).Split(Utility.separator)
                Dim codice = cod.Trim()
                If Not codice Is Nothing Then
                    If codice <> "" Then
                        xml += "<" + tagValue + ">"
                        xml += "<Codice>" + codice + "</Codice>"
                        xml += "</" + tagValue + ">"
                    End If
                End If
            Next
        End If
        Return xml
    End Function

#End Region

#Region "OperazioniObj"

    Private Function getOperazioneSpecObj(codiceOperazione As String, ByRef errore As Boolean) As Object
        'COMMENTO
        Dim op = reader.LeggiOperazione(ObjParametri_Server, CInt(codiceOperazione)).Rows(0)

        Select Case CStr(op.Item("CodiceOperazione"))
            Case "ARMC"
                Dim opObj = New ARMCOperazione
                opObj.CodCommittente = inserisciValoreLineare(op.Item("CodCommittente"))
                If inserisciValoreData(op.Item("DataGiustificativo")) IsNot Nothing Then
                    opObj.DataGiustificativo = inserisciValoreData(op.Item("DataGiustificativo"))
                End If
                opObj.DataOperazione = inserisciValoreData(op.Item("DataOperazione"))
                opObj.Note = inserisciValoreLineare(op.Item("Note"))
                opObj.NumGiustificativo = inserisciValoreLineare(op.Item("NumGiustificativo"))
                opObj.NumOperazione = inserisciValoreInteger(op.Item("NumOperazione"))
                getProdottiARMC(opObj, codiceOperazione, errore)
                Return opObj
            Case "GIIN"
                Dim opObj = New GiinOperazione
                opObj.CodCommittente = inserisciValoreLineare(op.Item("CodCommittente"))
                opObj.DataOperazione = inserisciValoreData(op.Item("DataOperazione"))
                opObj.Note = inserisciValoreLineare(op.Item("Note"))
                opObj.NumOperazione = inserisciValoreInteger(op.Item("NumOperazione"))
                getProdottiGIIN(opObj, codiceOperazione, errore)
                Return opObj
            Case "CASD"
                Dim opObj = New CasdOperazione
                opObj.CodFornitore = inserisciValoreLineare(op.Item("CodFornitore"))
                opObj.CodCommittente = inserisciValoreLineare(op.Item("CodCommittente"))
                If inserisciValoreData(op.Item("DataGiustificativo")) IsNot Nothing Then
                    opObj.DataGiustificativo = inserisciValoreData(op.Item("DataGiustificativo"))
                    opObj.DataGiustificativoSpecified = True
                End If
                opObj.DataOperazione = inserisciValoreData(op.Item("DataOperazione"))
                opObj.Note = inserisciValoreLineare(op.Item("Note"))
                opObj.NumGiustificativo = inserisciValoreLineare(op.Item("NumGiustificativo"))
                opObj.NumOperazione = inserisciValoreInteger(op.Item("NumOperazione"))
                opObj.EsoneroDeroga = inserisciValoreLineare(op.Item("EsoneroDeroga"))
                getProdottiCASD(opObj, codiceOperazione, errore)
                Return opObj
            Case "USSD"
                Dim opObj = New UssdOperazione
                opObj.CodDestinatario = inserisciValoreLineare(op.Item("CodDestinatario"))
                opObj.CodCommittente = inserisciValoreLineare(op.Item("CodCommittente"))
                If inserisciValoreData(op.Item("DataGiustificativo")) IsNot Nothing Then
                    opObj.DataGiustificativo = inserisciValoreData(op.Item("DataGiustificativo"))
                    opObj.DataGiustificativoSpecified = True
                End If
                opObj.DataOperazione = inserisciValoreData(op.Item("DataOperazione"))
                opObj.Note = inserisciValoreLineare(op.Item("Note"))
                opObj.NumGiustificativo = inserisciValoreLineare(op.Item("NumGiustificativo"))
                opObj.CodDestinatario = inserisciValoreLineare(op.Item("CodDestinatario"))
                opObj.NumOperazione = inserisciValoreInteger(op.Item("NumOperazione"))
                opObj.EsoneroDeroga = inserisciValoreLineare(op.Item("EsoneroDeroga"))
                getProdottiUSSD(opObj, codiceOperazione, errore)
                Return opObj
            Case "IMBO"
                Dim opObj = New ImboOperazione
                opObj.CodCommittente = inserisciValoreLineare(op.Item("CodCommittente"))
                opObj.DataOperazione = inserisciValoreData(op.Item("DataOperazione"))
                opObj.Note = inserisciValoreLineare(op.Item("Note"))
                opObj.NumOperazione = inserisciValoreInteger(op.Item("NumOperazione"))
                getProdottiIMBO(opObj, codiceOperazione, errore)
                Return opObj
            Case "ACID"
                Dim opObj = New AcidOperazione
                opObj.CodCommittente = inserisciValoreLineare(op.Item("CodCommittente"))
                opObj.DataOperazione = inserisciValoreData(op.Item("DataOperazione"))
                opObj.Note = inserisciValoreLineare(op.Item("Note"))
                opObj.NumOperazione = inserisciValoreInteger(op.Item("NumOperazione"))
                getProdottiACID(opObj, codiceOperazione, errore)
                Return opObj
            Case "PIGI"
                Dim opObj = New PigiOperazione
                opObj.CodCommittente = inserisciValoreLineare(op.Item("CodCommittente"))
                opObj.DataOperazione = inserisciValoreData(op.Item("DataOperazione"))
                opObj.Note = inserisciValoreLineare(op.Item("Note"))
                opObj.NumOperazione = inserisciValoreInteger(op.Item("NumOperazione"))
                getProdottiPIGI(opObj, codiceOperazione, errore)
                Return opObj
            Case "SVIN"
                Dim opObj = New SvinOperazione
                opObj.CodCommittente = inserisciValoreLineare(op.Item("CodCommittente"))
                opObj.DataOperazione = inserisciValoreData(op.Item("DataOperazione"))
                opObj.Note = inserisciValoreLineare(op.Item("Note"))
                opObj.NumOperazione = inserisciValoreInteger(op.Item("NumOperazione"))
                getProdottiSVIN(opObj, codiceOperazione, errore)
                Return opObj
            Case "DOLC"
                Dim opObj = New DolcOperazione
                opObj.CodCommittente = inserisciValoreLineare(op.Item("CodCommittente"))
                opObj.DataOperazione = inserisciValoreData(op.Item("DataOperazione"))
                opObj.Note = inserisciValoreLineare(op.Item("Note"))
                opObj.NumOperazione = inserisciValoreInteger(op.Item("NumOperazione"))
                getProdottiDOLC(opObj, codiceOperazione, errore)
                Return opObj
            Case "TAGL"
                Dim opObj = New TaglOperazione
                opObj.CodCommittente = inserisciValoreLineare(op.Item("CodCommittente"))
                opObj.DataOperazione = inserisciValoreData(op.Item("DataOperazione"))
                opObj.Note = inserisciValoreLineare(op.Item("Note"))
                opObj.NumOperazione = inserisciValoreInteger(op.Item("NumOperazione"))
                getProdottiTAGL(opObj, codiceOperazione, errore)
                Return opObj
            Case "CERT"
                Dim opObj = New CertOperazione
                opObj.CodCommittente = inserisciValoreLineare(op.Item("CodCommittente"))
                opObj.DataOperazione = inserisciValoreData(op.Item("DataOperazione"))
                opObj.Note = inserisciValoreLineare(op.Item("Note"))
                opObj.NumOperazione = inserisciValoreInteger(op.Item("NumOperazione"))
                getProdottiCERT(opObj, codiceOperazione, errore)
                Return opObj
            Case "DENT"
                Dim opObj = New DentOperazione
                opObj.CodCommittente = inserisciValoreLineare(op.Item("CodCommittente"))
                opObj.DataOperazione = inserisciValoreData(op.Item("DataOperazione"))
                opObj.Note = inserisciValoreLineare(op.Item("Note"))
                opObj.NumOperazione = inserisciValoreInteger(op.Item("NumOperazione"))
                getProdottiDENT(opObj, codiceOperazione, errore)
                Return opObj
            Case "AUCO"
                Dim opObj = New AucoOperazione
                opObj.DataOperazione = inserisciValoreData(op.Item("DataOperazione"))
                opObj.Note = inserisciValoreLineare(op.Item("Note"))
                opObj.NumOperazione = inserisciValoreInteger(op.Item("NumOperazione"))
                getProdottiAUCO(opObj, codiceOperazione, errore)
                Return opObj
            Case "PERD"
                Dim opObj = New PerdOperazione
                opObj.CodCommittente = inserisciValoreLineare(op.Item("CodCommittente"))
                opObj.DataOperazione = inserisciValoreData(op.Item("DataOperazione"))
                opObj.Note = inserisciValoreLineare(op.Item("Note"))
                opObj.NumOperazione = inserisciValoreInteger(op.Item("NumOperazione"))
                getProdottiPERD(opObj, codiceOperazione, errore)
                Return opObj
            Case "SFEC"
                Dim opObj = New SfecOperazione
                opObj.CodCommittente = inserisciValoreLineare(op.Item("CodCommittente"))
                opObj.DataOperazione = inserisciValoreData(op.Item("DataOperazione"))
                opObj.Note = inserisciValoreLineare(op.Item("Note"))
                opObj.NumOperazione = inserisciValoreInteger(op.Item("NumOperazione"))
                getProdottiSFEC(opObj, codiceOperazione, errore)
                Return opObj
            Case "AVLT"
                Dim opObj = New AvltOperazione
                opObj.CodCommittente = inserisciValoreLineare(op.Item("CodCommittente"))
                opObj.EsoneroDeroga = inserisciValoreLineare(op.Item("EsoneroDeroga"))
                If inserisciValoreData(op.Item("DataGiustificativo")) IsNot Nothing Then
                    opObj.DataGiustificativo = inserisciValoreData(op.Item("DataGiustificativo"))
                    opObj.DataGiustificativoSpecified = True
                End If
                opObj.DataOperazione = inserisciValoreData(op.Item("DataOperazione"))
                opObj.Note = inserisciValoreLineare(op.Item("Note"))
                opObj.NumGiustificativo = inserisciValoreLineare(op.Item("NumGiustificativo"))
                opObj.NumOperazione = inserisciValoreInteger(op.Item("NumOperazione"))
                getProdottiAVLT(opObj, codiceOperazione, errore)
                Return opObj
            Case "SPGS"
                Dim opObj = New SpgsOperazione
                opObj.CodCommittente = inserisciValoreLineare(op.Item("CodCommittente"))
                opObj.DataOperazione = inserisciValoreData(op.Item("DataOperazione"))
                opObj.Note = inserisciValoreLineare(op.Item("Note"))
                opObj.NumOperazione = inserisciValoreInteger(op.Item("NumOperazione"))
                getProdottiSPGS(opObj, codiceOperazione, errore)
                Return opObj
            Case "SPAB"
                Dim opObj = New SpabOperazione
                opObj.CodCommittente = inserisciValoreLineare(op.Item("CodCommittente"))
                If inserisciValoreData(op.Item("DataGiustificativo")) IsNot Nothing Then
                    opObj.DataGiustificativo = inserisciValoreData(op.Item("DataGiustificativo"))
                    opObj.DataGiustificativoSpecified = True
                End If
                opObj.DataOperazione = inserisciValoreData(op.Item("DataOperazione"))
                opObj.EsoneroDeroga = inserisciValoreLineare(op.Item("EsoneroDeroga"))
                opObj.Note = inserisciValoreLineare(op.Item("Note"))
                opObj.NumGiustificativo = inserisciValoreLineare(op.Item("NumGiustificativo"))
                opObj.NumOperazione = inserisciValoreInteger(op.Item("NumOperazione"))
                getProdottiSPAB(opObj, codiceOperazione, errore)
                Return opObj
            Case "SCDS"
                Dim opObj = New ScdsOperazione
                opObj.CodCommittente = inserisciValoreLineare(op.Item("CodCommittente"))
                If inserisciValoreData(op.Item("DataGiustificativo")) IsNot Nothing Then
                    opObj.DataGiustificativo = inserisciValoreData(op.Item("DataGiustificativo"))
                    opObj.DataGiustificativoSpecified = True
                End If
                opObj.DataOperazione = inserisciValoreData(op.Item("DataOperazione"))
                opObj.EsoneroDeroga = inserisciValoreLineare(op.Item("EsoneroDeroga"))
                opObj.Note = inserisciValoreLineare(op.Item("Note"))
                opObj.NumGiustificativo = inserisciValoreLineare(op.Item("NumGiustificativo"))
                opObj.NumOperazione = inserisciValoreInteger(op.Item("NumOperazione"))
                getProdottiSCDS(opObj, codiceOperazione, errore)
                Return opObj
            Case "ELMC"
                Dim opObj = New ElmcOperazione
                opObj.CodCommittente = inserisciValoreLineare(op.Item("CodCommittente"))
                If inserisciValoreData(op.Item("DataGiustificativo")) IsNot Nothing Then
                    opObj.DataGiustificativo = inserisciValoreData(op.Item("DataGiustificativo"))
                    opObj.DataGiustificativoSpecified = True
                End If
                opObj.DataOperazione = inserisciValoreData(op.Item("DataOperazione"))
                opObj.EsoneroDeroga = inserisciValoreLineare(op.Item("EsoneroDeroga"))
                opObj.Note = inserisciValoreLineare(op.Item("Note"))
                opObj.NumGiustificativo = inserisciValoreLineare(op.Item("NumGiustificativo"))
                opObj.NumOperazione = inserisciValoreInteger(op.Item("NumOperazione"))
                getProdottiELMC(opObj, codiceOperazione, errore)
                Return opObj
            Case "FRGS"
                Dim opObj = New FrgsOperazione
                opObj.CodCommittente = inserisciValoreLineare(op.Item("CodCommittente"))
                opObj.DataOperazione = inserisciValoreData(op.Item("DataOperazione"))
                opObj.Note = inserisciValoreLineare(op.Item("Note"))
                opObj.NumOperazione = inserisciValoreInteger(op.Item("NumOperazione"))
                getProdottiFRGS(opObj, codiceOperazione, errore)
                Return opObj
            Case "SCZC"
                Dim opObj = New SczcOperazione
                opObj.CodCommittente = inserisciValoreLineare(op.Item("CodCommittente"))
                If inserisciValoreData(op.Item("DataGiustificativo")) IsNot Nothing Then
                    opObj.DataGiustificativo = inserisciValoreData(op.Item("DataGiustificativo"))
                    opObj.DataGiustificativoSpecified = True
                End If
                opObj.DataOperazione = inserisciValoreData(op.Item("DataOperazione"))
                opObj.Note = inserisciValoreLineare(op.Item("Note"))
                opObj.EsoneroDeroga = inserisciValoreLineare(op.Item("EsoneroDeroga"))
                opObj.NumGiustificativo = inserisciValoreLineare(op.Item("NumGiustificativo"))
                opObj.NumOperazione = inserisciValoreInteger(op.Item("NumOperazione"))
                getProdottiSCZC(opObj, codiceOperazione, errore)
                Return opObj
            Case "LIEL"
                Dim opObj = New LielOperazione
                opObj.CodCommittente = inserisciValoreLineare(op.Item("CodCommittente"))
                If inserisciValoreData(op.Item("DataGiustificativo")) IsNot Nothing Then
                    opObj.DataGiustificativo = inserisciValoreData(op.Item("DataGiustificativo"))
                    opObj.DataGiustificativoSpecified = True
                End If
                opObj.EsoneroDeroga = inserisciValoreLineare(op.Item("EsoneroDeroga"))
                opObj.DataOperazione = inserisciValoreData(op.Item("DataOperazione"))
                opObj.Note = inserisciValoreLineare(op.Item("Note"))
                opObj.NumGiustificativo = inserisciValoreLineare(op.Item("NumGiustificativo"))
                opObj.NumOperazione = inserisciValoreInteger(op.Item("NumOperazione"))
                getProdottiLIEL(opObj, codiceOperazione, errore)
                Return opObj
            Case "EVAL"
                Dim opObj = New EvalOperazione
                opObj.CodCommittente = inserisciValoreLineare(op.Item("CodCommittente"))
                opObj.DataOperazione = inserisciValoreData(op.Item("DataOperazione"))
                opObj.Note = inserisciValoreLineare(op.Item("Note"))
                opObj.NumOperazione = inserisciValoreInteger(op.Item("NumOperazione"))
                getProdottiEVAL(opObj, codiceOperazione, errore)
                Return opObj
            Case "FRAB"
                Dim opObj = New FrabOperazione
                opObj.CodCommittente = inserisciValoreLineare(op.Item("CodCommittente"))
                opObj.DataOperazione = inserisciValoreData(op.Item("DataOperazione"))
                opObj.Note = inserisciValoreLineare(op.Item("Note"))
                opObj.NumOperazione = inserisciValoreInteger(op.Item("NumOperazione"))
                getProdottiFRAB(opObj, codiceOperazione, errore)
                Return opObj
            Case "AARD"
                Dim opObj = New AardOperazione
                opObj.CodCommittente = inserisciValoreLineare(op.Item("CodCommittente"))
                If inserisciValoreData(op.Item("DataGiustificativo")) IsNot Nothing Then
                    opObj.DataGiustificativo = inserisciValoreData(op.Item("DataGiustificativo"))
                    opObj.DataGiustificativoSpecified = True
                End If
                opObj.DataOperazione = inserisciValoreData(op.Item("DataOperazione"))
                opObj.EsoneroDeroga = inserisciValoreLineare(op.Item("EsoneroDeroga"))
                opObj.Note = inserisciValoreLineare(op.Item("Note"))
                opObj.NumGiustificativo = inserisciValoreLineare(op.Item("NumGiustificativo"))
                opObj.NumOperazione = inserisciValoreInteger(op.Item("NumOperazione"))
                getProdottiAARD(opObj, codiceOperazione, errore)
                Return opObj
            Case "DISA"
                Dim opObj = New DisaOperazione
                opObj.CodCommittente = inserisciValoreLineare(op.Item("CodCommittente"))
                opObj.DataOperazione = inserisciValoreData(op.Item("DataOperazione"))
                opObj.Note = inserisciValoreLineare(op.Item("Note"))
                opObj.NumOperazione = inserisciValoreInteger(op.Item("NumOperazione"))
                getProdottiDISA(opObj, codiceOperazione, errore)
                Return opObj
            Case "BABS"
                Dim opObj = New BabsOperazione
                opObj.CodCommittente = inserisciValoreLineare(op.Item("CodCommittente"))
                If inserisciValoreData(op.Item("DataGiustificativo")) IsNot Nothing Then
                    opObj.DataGiustificativo = inserisciValoreData(op.Item("DataGiustificativo"))
                    opObj.DataGiustificativoSpecified = True
                End If
                opObj.DataOperazione = inserisciValoreData(op.Item("DataOperazione"))
                opObj.EsoneroDeroga = inserisciValoreLineare(op.Item("EsoneroDeroga"))
                opObj.Note = inserisciValoreLineare(op.Item("Note"))
                opObj.NumGiustificativo = inserisciValoreLineare(op.Item("NumGiustificativo"))
                opObj.NumOperazione = inserisciValoreInteger(op.Item("NumOperazione"))
                getProdottiBABS(opObj, codiceOperazione, errore)
                Return opObj
            Case "ETIC"
                Dim opObj = New EticOperazione
                opObj.CodCommittente = inserisciValoreLineare(op.Item("CodCommittente"))
                opObj.DataOperazione = inserisciValoreData(op.Item("DataOperazione"))
                opObj.Note = inserisciValoreLineare(op.Item("Note"))
                opObj.NumOperazione = inserisciValoreInteger(op.Item("NumOperazione"))
                getProdottiETIC(opObj, codiceOperazione, errore)
                Return opObj
            Case "SUPE"
                Dim opObj = New SupeOperazione
                opObj.CodCommittente = inserisciValoreLineare(op.Item("CodCommittente"))
                opObj.DataOperazione = inserisciValoreData(op.Item("DataOperazione"))
                opObj.Note = inserisciValoreLineare(op.Item("Note"))
                opObj.NumOperazione = inserisciValoreInteger(op.Item("NumOperazione"))
                getProdottiSUPE(opObj, codiceOperazione, errore)
                Return opObj
            Case "DERI"
                Dim opObj = New DeriOperazione
                opObj.CodCommittente = inserisciValoreLineare(op.Item("CodCommittente"))
                If inserisciValoreData(op.Item("DataGiustificativo")) IsNot Nothing Then
                    opObj.DataGiustificativo = inserisciValoreData(op.Item("DataGiustificativo"))
                    opObj.DataGiustificativoSpecified = True
                End If
                opObj.DataOperazione = inserisciValoreData(op.Item("DataOperazione"))
                opObj.EsoneroDeroga = inserisciValoreLineare(op.Item("EsoneroDeroga"))
                opObj.Note = inserisciValoreLineare(op.Item("Note"))
                opObj.NumGiustificativo = inserisciValoreLineare(op.Item("NumGiustificativo"))
                opObj.NumOperazione = inserisciValoreInteger(op.Item("NumOperazione"))
                getProdottiDERI(opObj, codiceOperazione, errore)
                Return opObj
            Case "DIST"
                Dim opObj = New DistOperazione
                opObj.CodCommittente = inserisciValoreLineare(op.Item("CodCommittente"))
                opObj.DataOperazione = inserisciValoreData(op.Item("DataOperazione"))
                opObj.Note = inserisciValoreLineare(op.Item("Note"))
                opObj.NumOperazione = inserisciValoreInteger(op.Item("NumOperazione"))
                getProdottiDIST(opObj, codiceOperazione, errore)
                Return opObj
            Case "APRT"
                Dim opObj = New AprtOperazione
                opObj.CodCommittente = inserisciValoreLineare(op.Item("CodCommittente"))
                opObj.DataOperazione = inserisciValoreData(op.Item("DataOperazione"))
                opObj.Note = inserisciValoreLineare(op.Item("Note"))
                opObj.NumOperazione = inserisciValoreInteger(op.Item("NumOperazione"))
                getProdottiAPRT(opObj, codiceOperazione, errore)
                Return opObj
            Case "TRSO"
                Dim opObj = New TrsoOperazione
                opObj.CodCommittente = inserisciValoreLineare(op.Item("CodCommittente"))
                opObj.DataOperazione = inserisciValoreData(op.Item("DataOperazione"))
                opObj.Note = inserisciValoreLineare(op.Item("Note"))
                opObj.NumOperazione = inserisciValoreInteger(op.Item("NumOperazione"))
                getProdottiTRSO(opObj, codiceOperazione, errore)
                Return opObj
            Case "ACET"
                Dim opObj = New AcetOperazione
                opObj.CodCommittente = inserisciValoreLineare(op.Item("CodCommittente"))
                opObj.DataOperazione = inserisciValoreData(op.Item("DataOperazione"))
                opObj.Note = inserisciValoreLineare(op.Item("Note"))
                opObj.NumOperazione = inserisciValoreInteger(op.Item("NumOperazione"))
                getProdottiACET(opObj, codiceOperazione, errore)
                Return opObj
            Case Else
                Return Nothing
        End Select
    End Function

    Private Function inserisciValoreLineare(val As Object) As String
        If Not IsDBNull(val) Then
            If val IsNot Nothing AndAlso val <> "" AndAlso val <> "0" Then
                Return CStr(val)
            End If
            Return Nothing
        End If
        Return Nothing
    End Function

    Private Function inserisciValoreInteger(val As Object) As Integer
        If Not IsDBNull(val) Then
            Return CInt(val)
        End If
        Return 0
    End Function

    Private Function inserisciValoreIntegerNullable(val As Object) As Integer?
        If Not IsDBNull(val) Then
            If CInt(val) <> 0 Then
                Return CInt(val)
            End If
            Return Nothing
        End If
        Return Nothing
    End Function

    Private Function inserisciValoreDouble(val As Object) As Double?
        If Not IsDBNull(val) Then
            If val IsNot Nothing AndAlso CStr(val) <> "0" Then
                Return CDbl(val)
            End If
            Return Nothing
        End If
        Return 0
    End Function

    Private Function inserisciValoreData(val As Object) As Date?
        If Not IsDBNull(val) Then
            If val IsNot Nothing Then
                If IsDate(val) Then
                    If CDate(val) <> CostantiPersonalizzate.AGRODATAFINE Then
                        Return CDate(val)
                    End If
                End If
            End If
            Return Nothing
        End If
        Return Nothing
    End Function

    Private Function CodRecipienti(val As Object) As CodRecipiente()
        If Not IsDBNull(val) AndAlso CStr(val) <> "0" Then
            Dim codici As New List(Of CodRecipiente)
            If CStr(val) <> "0" Then
                Dim valori = CStr(val).Split("|")
                For Each valore In valori
                    If valore <> "" AndAlso valore <> "0" Then
                        Dim cod As New CodRecipiente
                        cod.Codice = valore
                        codici.Add(cod)
                    End If
                Next
                Return codici.ToArray
            End If
        End If
        Return Nothing
    End Function

    Private Function inserisciValorePartita(val As DataRow) As TipoPartita()
        Dim partite As New List(Of TipoPartita)
        Dim partita As New TipoPartita
        Dim volNominale As Double = inserisciValoreDouble(val.Item("VolNominale"))
        If volNominale = 0 Then
            Return Nothing
        Else
            partita.VolNominale = volNominale
            partita.NumConf = inserisciValoreInteger(val.Item("NumConf"))
            If val.Item("NumFinContr") IsNot Nothing AndAlso val.Item("NumFinContr") <> "" Then
                partita.NumFinContr = inserisciValoreLineare(val.Item("NumFinContr"))
            End If
            If val.Item("NumIniContr") IsNot Nothing AndAlso val.Item("NumIniContr") <> "" Then
                partita.NumIniContr = inserisciValoreLineare(val.Item("NumIniContr"))
            End If
            If val.Item("NumSerie") IsNot Nothing AndAlso val.Item("NumSerie") <> "" Then
                partita.NumSerie = inserisciValoreLineare(val.Item("NumSerie"))
            End If
            If val.Item("Lotto_Sian") IsNot Nothing AndAlso val.Item("Lotto_Sian") <> "" Then
                partita.Lotto = inserisciValoreLineare(val.Item("Lotto_Sian"))
            Else
                If val.Item("Lotto") IsNot Nothing AndAlso val.Item("Lotto") <> "" Then
                    partita.Lotto = inserisciValoreLineare(val.Item("Lotto"))
                End If
            End If


            partite.Add(partita)
            End If
            Return partite.ToArray
        'If Not IsDBNull(val) Then
        '    Dim partite As New List(Of TipoPartita)
        '    Dim partiteStr = val.Split("|")
        '    For Each partitaStr In partiteStr
        '        Dim partita As New TipoPartita
        '        Dim i = 0
        '        For Each valore In partitaStr.Split("*")
        '            If valore <> "" Then
        '                Select Case i
        '                    Case 0
        '                        partita.VolNominale = inserisciValoreDouble(valore)
        '                    Case 1
        '                        partita.NumConf = inserisciValoreInteger(valore)
        '                    Case 2
        '                        partita.NumIniContr = inserisciValoreLineare(valore)
        '                    Case 3
        '                        partita.NumFinContr = inserisciValoreLineare(valore)
        '                    Case 4
        '                        partita.Lotto = inserisciValoreLineare(valore)
        '                End Select
        '            End If
        '            i += 1
        '        Next
        '        partite.Add(partita)
        '    Next
        '    Return partite.ToArray
        'End If
        'Return Nothing
    End Function

    Private Function inserisciValorePartitaImbo(val As DataRow) As TipoPartitaImbo
        Dim partite As New List(Of TipoPartitaImbo)
        Dim partita As New TipoPartitaImbo
        Dim volNominale As Double = inserisciValoreDouble(val.Item("VolNominale"))
        If volNominale = 0 Then
            Return Nothing
        Else
            partita.VolNominale = volNominale
            If val.Item("Lotto_Sian") IsNot Nothing AndAlso val.Item("Lotto_Sian") <> "" Then
                partita.Lotto = inserisciValoreLineare(val.Item("Lotto_Sian"))
            Else
                partita.Lotto = inserisciValoreLineare(val.Item("Lotto"))
            End If
            partita.NumConf = inserisciValoreInteger(val.Item("NumConf"))
            partita.NumFinContr = inserisciValoreLineare(val.Item("NumFinContr"))
            partita.NumIniContr = inserisciValoreLineare(val.Item("NumIniContr"))
            partita.NumSerie = inserisciValoreLineare(val.Item("NumSerie"))
            partite.Add(partita)
        End If
        'Return partite.ToArray
        Return partita
        'If Not IsDBNull(val) Then
        '    Dim partite As New List(Of TipoPartitaImbo)
        '    Dim partiteStr = val.Split("|")
        '    For Each partitaStr In partiteStr
        '        Dim partita As New TipoPartitaImbo
        '        Dim i = 0
        '        For Each valore In partitaStr.Split("*")
        '            If valore <> "" Then
        '                Select Case i
        '                    Case 0
        '                        partita.VolNominale = inserisciValoreDouble(valore)
        '                    Case 1
        '                        partita.NumConf = inserisciValoreInteger(valore)
        '                    Case 2
        '                        partita.NumIniContr = inserisciValoreLineare(valore)
        '                    Case 3
        '                        partita.NumFinContr = inserisciValoreLineare(valore)
        '                    Case 4
        '                        partita.Lotto = inserisciValoreLineare(valore)
        '                End Select
        '            End If
        '            i += 1
        '        Next
        '        partite.Add(partita)
        '    Next
        '    Return partite.ToArray
        'End If
        'Return Nothing
    End Function

    Private Function inserisciValorePartitaNoContr(val As Object) As TipoPartitaNoContr
        Dim partite As New List(Of TipoPartitaNoContr)
        Dim partita As New TipoPartitaNoContr
        Dim volNominale As Double = inserisciValoreDouble(val.Item("VolNominale"))
        If volNominale = 0 Then
            Return Nothing
        Else
            partita.VolNominale = volNominale
            If val.Item("Lotto_Sian") IsNot Nothing AndAlso val.Item("Lotto_Sian") <> "" Then
                partita.Lotto = inserisciValoreLineare(val.Item("Lotto_Sian"))
            Else
                partita.Lotto = inserisciValoreLineare(val.Item("Lotto"))
            End If

            partita.NumConf = inserisciValoreInteger(val.Item("NumConf"))
            partite.Add(partita)
        End If
        Return partita
        'If Not IsDBNull(val) Then
        '    Dim partite As New List(Of TipoPartitaNoContr)
        '    Dim partiteStr = val.Split("|")
        '    For Each partitaStr In partiteStr
        '        Dim partita As New TipoPartitaNoContr
        '        Dim i = 0
        '        For Each valore In partitaStr.Split("*")
        '            If valore <> "" Then
        '                Select Case i
        '                    Case 0
        '                        partita.VolNominale = inserisciValoreDouble(valore)
        '                    Case 1
        '                        partita.NumConf = inserisciValoreInteger(valore)
        '                    Case 2

        '                    Case 3

        '                    Case 4
        '                        partita.Lotto = inserisciValoreLineare(valore)
        '                End Select
        '            End If
        '            i += 1
        '        Next
        '        partite.Add(partita)
        '    Next
        '    Return partite.ToArray
        'End If
        'Return Nothing
    End Function

    Private Function inserisciCodiceProdotto(row As DataRow) As CodiceProdotto
        Dim codiceProdotto As New CodiceProdotto
        codiceProdotto.CodPrimario = CStr(row.Item("Mat_Cod"))
        codiceProdotto.CodSecondario = CStr(row.Item("Lotto"))
        Return codiceProdotto
    End Function

#Region "ProdottiObj"

    Private Sub getProdottiARMC(opObj As ARMCOperazione, codiceOperazione As String, ByRef errore As Boolean)
        'Prod Base
        Dim prodBaseDT = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "ProdottoBase")
        If prodBaseDT.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT.Rows(0)
            Dim prodBase As New ProdottoBase
            prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
            prodBase.MetodoPE = inserisciValoreLineare((prodBaseDB.Item("MetodoPE")))
            If prodBaseDB.Item("PercIgp") IsNot Nothing AndAlso (prodBaseDB.Item("PercIgp") <> 0) Then
                prodBase.PercIgp = inserisciValoreDouble((prodBaseDB.Item("PercIgp")))
                prodBase.PercIgpSpecified = True
            End If
            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                prodBase.TitoloAlcolTot = inserisciValoreDouble((prodBaseDB.Item("TitoloAlcolTot")))
                prodBase.TitoloAlcolTotSpecified = True
            End If
            Dim prodotto As New ProdottoBaseProdotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New ProdottoARMC1)
            prodBase.Prodotto = prodotto
            opObj.ProdBase = prodBase
        End If


        'ProdottoAggiunto
        Dim prodAggiuntoDT = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "ProdottoAggiunto")
        If prodAggiuntoDT.Rows.Count > 0 Then
            Dim prodDB = prodAggiuntoDT.Rows(0)
            Dim prodAgg As New ProdottoAggiunto
            prodAgg.CodRecipiente = CodRecipienti((prodDB.Item("vasi")))
            prodAgg.Quantita = inserisciValoreDouble((prodDB.Item("Qta")))
            If prodDB.Item("TitoloAlcolPot") IsNot Nothing AndAlso (prodDB.Item("TitoloAlcolPot") <> 0) Then
                prodAgg.TitoloAlcolPot = inserisciValoreDouble((prodDB.Item("TitoloAlcolPot")))
            End If
            Dim prodotto As New ProdottoAggiuntoProdotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodDB), New ProdottoARMC2)
            prodAgg.Prodotto = prodotto
            opObj.ProdAggiunto = prodAgg
        End If

        'ProdottoArricchito
        Dim prodArricchitoDT = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "ProdottoArricchito")
        If prodArricchitoDT.Rows.Count > 0 Then
            Dim prodDB = prodArricchitoDT.Rows(0)
            Dim prodAgg As New ProdottoArricchito
            prodAgg.CodRecipiente = CodRecipienti((prodDB.Item("vasi")))
            prodAgg.MetodoPE = inserisciValoreLineare((prodDB.Item("MetodoPE")))
            If prodDB.Item("PercIgp") IsNot Nothing AndAlso (prodDB.Item("PercIgp") <> 0) Then
                prodAgg.PercIgp = inserisciValoreDouble((prodDB.Item("PercIgp")))
                prodAgg.PercIgpSpecified = True
            End If
            prodAgg.Quantita = inserisciValoreDouble((prodDB.Item("Qta")))
            If prodDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodDB.Item("TitoloAlcolTot") <> 0) Then
                prodAgg.TitoloAlcolTot = inserisciValoreDouble((prodDB.Item("TitoloAlcolTot")))
                prodAgg.TitoloAlcolTotSpecified = True
            End If
            Dim prodotto As New ProdottoArricchitoProdotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodDB), New ProdottoARMC1)
            prodAgg.Prodotto = prodotto
            opObj.ProdArricchito = prodAgg
        End If

    End Sub

    'ProdGiin Tabella 
    Private Sub getProdottiGIIN(opObj As GiinOperazione, codiceOperazione As String, ByRef errore As Boolean)
        Dim prodBaseDT = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "ProdGiin")
        If prodBaseDT.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT.Rows(0)
            Dim prodBase As New ProdottoGiin
            prodBase.CodRecipiente = CodRecipienti(prodBaseDB.Item("vasi"))
            prodBase.Quantita = inserisciValoreDouble(prodBaseDB.Item("Qta"))
            prodBase.CodTenoreZucc = inserisciValoreLineare(prodBaseDB.Item("CodTenoreZucchero"))
            Dim gg = inserisciValoreInteger(prodBaseDB.Item("GgInvecchiamento"))
            If gg <> 0 Then
                prodBase.GgInvecchiamento = gg
            End If
            prodBase.Partita = inserisciValorePartita(prodBaseDB)
            If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                prodBase.TitoloAlcolTotSpecified = True
            End If
            If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
                prodBase.TitoloAlcolEffSpecified = True
            End If
            If prodBaseDB.Item("TitoloAlcolPot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolPot") <> 0) Then
                prodBase.TitoloAlcolPot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolPot"))
                prodBase.TitoloAlcolPotSpecified = True
            End If
            If prodBaseDB.Item("PercIgp") IsNot Nothing AndAlso (prodBaseDB.Item("PercIgp") <> 0) Then
                prodBase.PercIgp = inserisciValoreDouble((prodBaseDB.Item("PercIgp")))
                prodBase.PercIgpSpecified = True
            End If
            Dim prodGiin = New ProdottoGiinProdGiin1()
            Dim prodgiin1 = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New ProdottoGiin1)
            prodGiin.Item = prodgiin1
            prodBase.ProdGiin1 = prodGiin
            opObj.ProdGiin = prodBase
        Else
            errore = True
        End If
    End Sub

    Private Sub getProdottiACET(opObj As AcetOperazione, codiceOperazione As String, ByRef errore As Boolean)
        Dim prodBaseDT = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "AcetProdotto1")
        Dim prodottiAcet1 As New List(Of AcetProdotto1)
        If prodBaseDT.Rows.Count > 0 Then
            For Each prodBaseDB In prodBaseDT.Rows
                Dim prodBase As New AcetProdotto1
                prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
                prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
                prodBase.CodTenoreZucc = inserisciValoreLineare((prodBaseDB.Item("CodTenoreZucchero")))
                If prodBaseDB.Item("montegradi") IsNot Nothing AndAlso prodBaseDB.Item("montegradi") <> "0" Then
                    prodBase.Montegradi = inserisciValoreDouble(prodBaseDB.Item("montegradi"))
                    prodBase.MontegradiSpecified = True
                End If
                If prodBaseDB.Item("GradoAcidita") IsNot Nothing AndAlso prodBaseDB.Item("GradoAcidita") <> "0" Then
                    prodBase.GradoAcidita = inserisciValoreDouble(prodBaseDB.Item("GradoAcidita"))
                    prodBase.GradoAciditaSpecified = True
                End If
                If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                    prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
                End If
                Dim prodotto As New AcetProdotto1Prodotto
                prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New AcetDesignProd1)
                prodBase.Prodotto = prodotto
                prodottiAcet1.Add(prodBase)
            Next
            opObj.AcetProdotto1 = prodottiAcet1.ToArray
        End If

        Dim prodBaseDT2 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "AcetProdotto2")
        If prodBaseDT2.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT2.Rows(0)
            Dim prodBase As New AcetProdotto2
            Dim prodotto As New AcetProdotto2Prodotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New AcetDesignProd2)
            prodBase.Prodotto = prodotto
            opObj.AcetProdotto2 = prodBase
        End If

        Dim prodBaseDT3 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "AcetProdotto3")
        If prodBaseDT3.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT3.Rows(0)
            Dim prodBase As New AcetProdotto3
            'prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            'prodBase.TitoloAlcolEff = inserisciValoreDouble((prodBaseDB.Item("TitoloAlcolEff")))
            'If prodBaseDB.Item("montegradi") IsNot Nothing AndAlso prodBaseDB.Item("montegradi") <> "0" Then
            '    prodBase.Montegradi = inserisciValoreDouble(prodBaseDB.Item("montegradi"))
            '    prodBase.MontegradiSpecified = True
            'End If
            'If prodBaseDB.Item("GradoAcidita") IsNot Nothing AndAlso prodBaseDB.Item("GradoAcidita") <> "0" Then
            '    prodBase.GradoAcidita = inserisciValoreDouble(prodBaseDB.Item("GradoAcidita"))
            '    prodBase.GradoAciditaSpecified = True
            'End If
            Dim prodotto As New AcetProdotto3Prodotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New AcetDesignProd3)
            prodBase.Prodotto = prodotto
            opObj.AcetProdotto3 = prodBase
        End If
    End Sub

    Private Sub getProdottiTRSO(opObj As TrsoOperazione, codiceOperazione As String, ByRef errore As Boolean)
        Dim prodBaseDT = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "TrsoProdotto1")
        If prodBaseDT.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT.Rows(0)
            Dim prodBase As New TrsoProdotto1
            prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            prodBase.CodTenoreZucc = inserisciValoreLineare((prodBaseDB.Item("CodTenoreZucchero")))
            If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                prodBase.TitoloAlcolTotSpecified = True
            End If
            If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
                prodBase.TitoloAlcolEffSpecified = True
            End If
            If prodBaseDB.Item("TitoloAlcolPot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolPot") <> 0) Then
                prodBase.TitoloAlcolPot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolPot"))
                prodBase.TitoloAlcolPotSpecified = True
            End If
            If prodBaseDB.Item("PercIgp") IsNot Nothing AndAlso (prodBaseDB.Item("PercIgp") <> 0) Then
                prodBase.PercIgp = inserisciValoreDouble((prodBaseDB.Item("PercIgp")))
                prodBase.PercIgpSpecified = True
            End If
            Dim prodotto As New TrsoProdotto1Prodotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New TrsoDesignProd1)
            prodBase.Prodotto = prodotto
            opObj.TrsoProdotto1 = prodBase
        End If

        Dim prodBaseDT2 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "TrsoProdotto2")
        If prodBaseDT2.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT2.Rows(0)
            Dim prodBase As New TrsoProdotto2
            prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            prodBase.CodTenoreZucc = inserisciValoreLineare((prodBaseDB.Item("CodTenoreZucchero")))
            If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                prodBase.TitoloAlcolTotSpecified = True
            End If
            If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
                prodBase.TitoloAlcolEffSpecified = True
            End If
            If prodBaseDB.Item("TitoloAlcolPot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolPot") <> 0) Then
                prodBase.TitoloAlcolPot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolPot"))
                prodBase.TitoloAlcolPotSpecified = True
            End If
            If prodBaseDB.Item("PercIgp") IsNot Nothing AndAlso (prodBaseDB.Item("PercIgp") <> 0) Then
                prodBase.PercIgp = inserisciValoreDouble((prodBaseDB.Item("PercIgp")))
                prodBase.PercIgpSpecified = True
            End If
            Dim prodotto As New TrsoProdotto2Prodotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New TrsoDesignProd2)
            prodBase.Prodotto = prodotto
            opObj.TrsoProdotto2 = prodBase
        End If
    End Sub

    Private Sub getProdottiAPRT(opObj As AprtOperazione, codiceOperazione As String, ByRef errore As Boolean)
        Dim prodBaseDT = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "AprtProdotto1")
        If prodBaseDT.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT.Rows(0)
            Dim prodBase As New AprtProdotto1
            prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            prodBase.CodTenoreZucc = inserisciValoreLineare((prodBaseDB.Item("CodTenoreZucchero")))
            If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                prodBase.TitoloAlcolTotSpecified = True
            End If
            If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
                prodBase.TitoloAlcolEffSpecified = True
            End If
            If prodBaseDB.Item("TitoloAlcolPot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolPot") <> 0) Then
                prodBase.TitoloAlcolPot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolPot"))
                prodBase.TitoloAlcolPotSpecified = True
            End If
            If prodBaseDB.Item("PercIgp") IsNot Nothing AndAlso (prodBaseDB.Item("PercIgp") <> 0) Then
                prodBase.PercIgp = inserisciValoreDouble((prodBaseDB.Item("PercIgp")))
                prodBase.PercIgpSpecified = True
            End If
            Dim prodotto As New AprtProdotto1Prodotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New AprtDesignProd1)
            prodBase.Prodotto = prodotto
            opObj.AprtProdotto1 = prodBase
        End If

        Dim prodBaseDT2 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "AprtProdotto2")
        If prodBaseDT2.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT2.Rows(0)
            Dim prodBase As New AprtProdotto2
            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            Dim prodotto As New AprtProdotto2Prodotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New AprtDesignProd2)
            prodBase.Prodotto = prodotto
            opObj.AprtProdotto2 = prodBase
        End If

        Dim prodBaseDT3 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "AprtProdotto3")
        If prodBaseDT3.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT3.Rows(0)
            Dim prodBase As New AprtProdotto3
            prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            prodBase.CodTenoreZucc = inserisciValoreLineare((prodBaseDB.Item("CodTenoreZucchero")))
            If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                prodBase.TitoloAlcolTotSpecified = True
            End If
            If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
                prodBase.TitoloAlcolEffSpecified = True
            End If
            If prodBaseDB.Item("TitoloAlcolPot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolPot") <> 0) Then
                prodBase.TitoloAlcolPot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolPot"))
                prodBase.TitoloAlcolPotSpecified = True
            End If
            If prodBaseDB.Item("PercIgp") IsNot Nothing AndAlso (prodBaseDB.Item("PercIgp") <> 0) Then
                prodBase.PercIgp = inserisciValoreDouble((prodBaseDB.Item("PercIgp")))
                prodBase.PercIgpSpecified = True
            End If
            Dim prodotto As New AprtProdotto3Prodotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New AprtDesignProd3)
            prodBase.Prodotto = prodotto
            opObj.AprtProdotto3 = prodBase
        End If
    End Sub

    Private Sub getProdottiDIST(opObj As DistOperazione, codiceOperazione As String, ByRef errore As Boolean)
        Dim prodBaseDT = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "DistProdotto1")
        Dim prodottiDist1 As New List(Of DistProdotto1)
        If prodBaseDT.Rows.Count > 0 Then
            For Each prodBaseDB In prodBaseDT.Rows
                Dim prodBase As New DistProdotto1
                prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
                prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
                If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                    prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                    prodBase.TitoloAlcolTotSpecified = True
                End If
                If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                    prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
                    prodBase.TitoloAlcolEffSpecified = True
                End If
                If prodBaseDB.Item("TitoloAlcolPot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolPot") <> 0) Then
                    prodBase.TitoloAlcolPot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolPot"))
                    prodBase.TitoloAlcolPotSpecified = True
                End If
                If prodBaseDB.Item("PercIgp") IsNot Nothing AndAlso (prodBaseDB.Item("PercIgp") <> 0) Then
                    prodBase.PercIgp = inserisciValoreDouble((prodBaseDB.Item("PercIgp")))
                    prodBase.PercIgpSpecified = True
                End If
                Dim prodotto As New DistProdotto1Prodotto
                prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New DistDesignProd1)
                prodBase.Prodotto = prodotto
                prodottiDist1.Add(prodBase)
            Next
            opObj.DistProdotto1 = prodottiDist1.ToArray
        End If

        Dim prodBaseDT2 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "DistProdotto2")
        If prodBaseDT2.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT2.Rows(0)
            Dim prodBase As New DistProdotto2
            prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
            End If
            Dim prodotto As New DistProdotto2Prodotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New DistDesignProd2)
            prodBase.Prodotto = prodotto
            opObj.DistProdotto2 = prodBase
        End If

        Dim prodBaseDT3 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "DistProdotto3")
        If prodBaseDT3.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT3.Rows(0)
            Dim prodBase As New DistProdotto3
            prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))

            If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
            End If
            
            Dim prodotto As New DistProdotto3Prodotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New DistDesignProd3)
            prodBase.Prodotto = prodotto
            opObj.DistProdotto3 = prodBase
        End If

    End Sub

    Private Sub getProdottiDERI(opObj As DeriOperazione, codiceOperazione As String, ByRef errore As Boolean)
        Dim prodBaseDT = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "DeriProdotto1")
        If prodBaseDT.Rows.Count > 0 Then
            Dim i = 1
            Dim Qta As Double = 0
            Dim partite As New List(Of TipoPartita)
            Dim prodBaseI As New DeriProdotto1
            Dim prodottoI As New DeriProdotto1Prodotto
            For Each prodBaseDB In prodBaseDT.Rows
                If i = 1 Then
                    prodBaseI.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))

                    prodBaseI.CodTenoreZucc = inserisciValoreLineare((prodBaseDB.Item("CodTenoreZucchero")))
                    If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                        prodBaseI.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                        prodBaseI.TitoloAlcolTotSpecified = True
                    End If
                    If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                        prodBaseI.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
                        prodBaseI.TitoloAlcolEffSpecified = True
                    End If
                    If prodBaseDB.Item("PercIgp") IsNot Nothing AndAlso (prodBaseDB.Item("PercIgp") <> 0) Then
                        prodBaseI.PercIgp = inserisciValoreDouble((prodBaseDB.Item("PercIgp")))
                        prodBaseI.PercIgpSpecified = True
                    End If
                    prodottoI.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New DeriDesignProd1)
                    Qta += inserisciValoreDouble((prodBaseDB.Item("Qta")))
                    If inserisciValorePartita(prodBaseDB) IsNot Nothing Then
                        For Each part In inserisciValorePartita(prodBaseDB)
                            partite.Add(part)
                        Next
                    End If
                Else
                    Qta += inserisciValoreDouble((prodBaseDB.Item("Qta")))
                    If inserisciValorePartita(prodBaseDB) IsNot Nothing Then
                        For Each part In inserisciValorePartita(prodBaseDB)
                            partite.Add(part)
                        Next
                    End If
                End If
                i += 1
            Next
            prodBaseI.Quantita = Qta
            prodBaseI.Partita = partite.ToArray
            prodBaseI.Prodotto = prodottoI
            opObj.DeriProdotto1 = prodBaseI
            'Dim prodBaseDB = prodBaseDT.Rows(0)
            'Dim prodBase As New DeriProdotto1
            'prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
            'prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            'prodBase.CodTenoreZucc = inserisciValoreLineare((prodBaseDB.Item("CodTenoreZucchero")))
            'If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
            '    prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
            '    prodBase.TitoloAlcolTotSpecified = True
            'End If
            'If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
            '    prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
            '    prodBase.TitoloAlcolEffSpecified = True
            'End If
            'If prodBaseDB.Item("PercIgp") IsNot Nothing AndAlso (prodBaseDB.Item("PercIgp") <> 0) Then
            '    prodBase.PercIgp = inserisciValoreDouble((prodBaseDB.Item("PercIgp")))
            '    prodBase.PercIgpSpecified = True
            'End If
            'Dim prodotto As New DeriProdotto1Prodotto
            'prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New DeriDesignProd1)
            'prodBase.Prodotto = prodotto
            'opObj.DeriProdotto1 = prodBase
        End If

        Dim prodBaseDT2 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "DeriProdotto2")
        If prodBaseDT2.Rows.Count > 0 Then
            Dim i = 1
            Dim Qta As Double = 0
            Dim partite As New List(Of TipoPartitaNoContr)
            Dim prodBaseI As New DeriProdotto2
            Dim prodottoI As New DeriProdotto2Prodotto
            For Each prodBaseDB In prodBaseDT2.Rows
                If i = 1 Then
                    prodBaseI.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))

                    prodBaseI.CodTenoreZucc = inserisciValoreLineare((prodBaseDB.Item("CodTenoreZucchero")))
                    If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                        prodBaseI.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                        prodBaseI.TitoloAlcolTotSpecified = True
                    End If
                    If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                        prodBaseI.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
                        prodBaseI.TitoloAlcolEffSpecified = True
                    End If
                    If prodBaseDB.Item("PercIgp") IsNot Nothing AndAlso (prodBaseDB.Item("PercIgp") <> 0) Then
                        prodBaseI.PercIgp = inserisciValoreDouble((prodBaseDB.Item("PercIgp")))
                        prodBaseI.PercIgpSpecified = True
                    End If
                    prodottoI.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New DeriDesignProd2)
                    Qta += inserisciValoreDouble((prodBaseDB.Item("Qta")))
                    partite.Add(inserisciValorePartitaNoContr(prodBaseDB))
                Else
                    Qta += inserisciValoreDouble((prodBaseDB.Item("Qta")))
                    partite.Add(inserisciValorePartitaNoContr(prodBaseDB))
                End If
                i += 1
            Next
            prodBaseI.Quantita = Qta
            prodBaseI.Partita = partite.ToArray
            prodBaseI.Prodotto = prodottoI
            opObj.DeriProdotto2 = prodBaseI
            'Dim prodBaseDB = prodBaseDT2.Rows(0)
            'Dim prodBase As New DeriProdotto2
            'prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
            'prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            'prodBase.CodTenoreZucc = inserisciValoreLineare((prodBaseDB.Item("CodTenoreZucchero")))
            'If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
            '    prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
            '    prodBase.TitoloAlcolTotSpecified = True
            'End If
            'If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
            '    prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
            '    prodBase.TitoloAlcolEffSpecified = True
            'End If
            'If prodBaseDB.Item("PercIgp") IsNot Nothing AndAlso (prodBaseDB.Item("PercIgp") <> 0) Then
            '    prodBase.PercIgp = inserisciValoreDouble((prodBaseDB.Item("PercIgp")))
            '    prodBase.PercIgpSpecified = True
            'End If
            'Dim prodotto As New DeriProdotto2Prodotto
            'prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New DeriDesignProd2)
            'prodBase.Prodotto = prodotto
            'opObj.DeriProdotto2 = prodBase
        End If
    End Sub

    Private Sub getProdottiSUPE(opObj As SupeOperazione, codiceOperazione As String, ByRef errore As Boolean)
        Dim prodBaseDT = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "SupeProdotto1")
        Dim prodottiSupe1 As New List(Of SupeProdotto1)
        If prodBaseDT.Rows.Count > 0 Then
            For Each prodBaseDB In prodBaseDT.Rows
                Dim prodBase As New SupeProdotto1
                prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
                If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                    prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                    prodBase.TitoloAlcolTotSpecified = True
                End If
                If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                    prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
                    prodBase.TitoloAlcolEffSpecified = True
                End If
                If prodBaseDB.Item("TitoloAlcolPot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolPot") <> 0) Then
                    prodBase.TitoloAlcolPot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolPot"))
                    prodBase.TitoloAlcolPotSpecified = True
                End If
                If prodBaseDB.Item("PercIgp") IsNot Nothing AndAlso (prodBaseDB.Item("PercIgp") <> 0) Then
                    prodBase.PercIgp = inserisciValoreDouble((prodBaseDB.Item("PercIgp")))
                    prodBase.PercIgpSpecified = True
                End If
                Dim prodotto As New SupeProdotto1Prodotto
                prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New SupeDesignProd1)
                prodBase.Prodotto = prodotto
                prodottiSupe1.Add(prodBase)
            Next
            opObj.SupeProdotto1 = prodottiSupe1.ToArray
        End If

        Dim prodBaseDT2 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "SupeProdotto2")
        If prodBaseDT2.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT2.Rows(0)
            Dim prodBase As New SupeProdotto2
            prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            Dim prodotto As New SupeProdotto2Prodotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New SupeDesignProd2)
            prodBase.Prodotto = prodotto
            opObj.SupeProdotto2 = prodBase
        End If

    End Sub

    Private Sub getProdottiETIC(opObj As EticOperazione, codiceOperazione As String, ByRef errore As Boolean)
        Dim prodBaseDT = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "EticProdotto1")
        Dim Qta As Double = 0
        Dim partiteI As New List(Of TipoPartita)
        Dim i = 1
        Dim prodBaseI As New EticProdotto1
        Dim prodottoI As New EticProdotto1Prodotto
        For Each prodBaseDB In prodBaseDT.Rows
            If i = 1 Then

                prodBaseI.CodTenoreZucc = inserisciValoreLineare((prodBaseDB.Item("CodTenoreZucchero")))
                If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                    prodBaseI.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                    prodBaseI.TitoloAlcolTotSpecified = True
                End If
                If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                    prodBaseI.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
                    prodBaseI.TitoloAlcolEffSpecified = True
                End If
                If prodBaseDB.Item("TitoloAlcolPot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolPot") <> 0) Then
                    prodBaseI.TitoloAlcolPot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolPot"))
                    prodBaseI.TitoloAlcolPotSpecified = True
                End If
                prodottoI.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New EticDesignProd1)
                Qta += inserisciValoreDouble((prodBaseDB.Item("Qta")))
                'For Each part In inserisciValorePartita(prodBaseDB)
                '    partiteI.Add(part)
                'Next
                Dim partiteS = inserisciValorePartita(prodBaseDB)
                If partiteS IsNot Nothing Then
                    For Each part In partiteS
                        partiteI.Add(part)
                    Next
                End If
            Else
                Qta += inserisciValoreDouble((prodBaseDB.Item("Qta")))
                'For Each part In inserisciValorePartita(prodBaseDB)
                '    partiteI.Add(part)
                'Next
                Dim partiteS = inserisciValorePartita(prodBaseDB)
                If partiteS IsNot Nothing Then
                    For Each part In partiteS
                        partiteI.Add(part)
                    Next
                End If
            End If
            i += 1
        Next
        prodBaseI.Quantita = Qta
        prodBaseI.Partita = partiteI.ToArray
        prodBaseI.Prodotto = prodottoI
        opObj.EticProdotto1 = prodBaseI
        'If prodBaseDT.Rows.Count > 0 Then
        '    Dim prodBaseDB = prodBaseDT.Rows(0)
        '    Dim prodBase As New EticProdotto1
        '    prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
        '    prodBase.CodTenoreZucc = inserisciValoreLineare((prodBaseDB.Item("CodTenoreZucchero")))
        '    If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
        '        prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
        '        prodBase.TitoloAlcolTotSpecified = True
        '    End If
        '    If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
        '        prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
        '        prodBase.TitoloAlcolEffSpecified = True
        '    End If
        '    If prodBaseDB.Item("TitoloAlcolPot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolPot") <> 0) Then
        '        prodBase.TitoloAlcolPot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolPot"))
        '        prodBase.TitoloAlcolPotSpecified = True
        '    End If
        '    prodBase.Partita = inserisciValorePartita(prodBaseDB)
        '    Dim prodotto As New EticProdotto1Prodotto
        '    prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New EticDesignProd1)
        '    prodBase.Prodotto = prodotto
        '    opObj.EticProdotto1 = prodBase
        'End If

        Dim prodBaseDT2 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "EticProdotto2")
        Qta = 0
        Dim partiteI1 As New List(Of TipoPartita)
        i = 1
        Dim prodBaseI1 As New EticProdotto2
        Dim prodottoI1 As New EticProdotto2Prodotto
        For Each prodBaseDB In prodBaseDT2.Rows
            If i = 1 Then
                prodBaseI1.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
                prodBaseI1.CodTenoreZucc = inserisciValoreLineare((prodBaseDB.Item("CodTenoreZucchero")))
                If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                    prodBaseI1.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                    prodBaseI1.TitoloAlcolTotSpecified = True
                End If
                If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                    prodBaseI1.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
                    prodBaseI1.TitoloAlcolEffSpecified = True
                End If
                If prodBaseDB.Item("TitoloAlcolPot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolPot") <> 0) Then
                    prodBaseI1.TitoloAlcolPot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolPot"))
                    prodBaseI1.TitoloAlcolPotSpecified = True
                End If
                prodBaseI1.Partita = inserisciValorePartita(prodBaseDB)
                prodottoI1.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New EticDesignProd2)
                Qta += inserisciValoreDouble((prodBaseDB.Item("Qta")))
                'For Each part In inserisciValorePartita(prodBaseDB)
                '    partiteI1.Add(part)
                'Next
                Dim partiteS = inserisciValorePartita(prodBaseDB)
                If partiteS IsNot Nothing Then
                    For Each part In partiteS
                        partiteI1.Add(part)
                    Next
                End If
            Else
                Qta += inserisciValoreDouble((prodBaseDB.Item("Qta")))
                'For Each part In inserisciValorePartita(prodBaseDB)
                '    partiteI1.Add(part)
                'Next
                Dim partiteS = inserisciValorePartita(prodBaseDB)
                If partiteS IsNot Nothing Then
                    For Each part In partiteS
                        partiteI1.Add(part)
                    Next
                End If
            End If
            i += 1
        Next
        prodBaseI1.Quantita = Qta
        prodBaseI1.Partita = partiteI1.ToArray
        prodBaseI1.Prodotto = prodottoI1
        opObj.EticProdotto2 = prodBaseI1

        'If prodBaseDT2.Rows.Count > 0 Then
        '    Dim prodBaseDB = prodBaseDT2.Rows(0)
        '    Dim prodBase As New EticProdotto2
        '    prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
        '    prodBase.CodTenoreZucc = inserisciValoreLineare((prodBaseDB.Item("CodTenoreZucchero")))
        '    If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
        '        prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
        '        prodBase.TitoloAlcolTotSpecified = True
        '    End If
        '    If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
        '        prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
        '        prodBase.TitoloAlcolEffSpecified = True
        '    End If
        '    If prodBaseDB.Item("TitoloAlcolPot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolPot") <> 0) Then
        '        prodBase.TitoloAlcolPot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolPot"))
        '        prodBase.TitoloAlcolPotSpecified = True
        '    End If
        '    prodBase.Partita = inserisciValorePartita(prodBaseDB)
        '    Dim prodotto As New EticProdotto2Prodotto
        '    prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New EticDesignProd2)
        '    prodBase.Prodotto = prodotto
        '    opObj.EticProdotto2 = prodBase
        'End If
    End Sub

    Private Sub getProdottiBABS(opObj As BabsOperazione, codiceOperazione As String, ByRef errore As Boolean)
        Dim prodBaseDT = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "BabsProdotto1")
        Dim prodottibabs1 As New List(Of BabsProdotto1)
        If prodBaseDT.Rows.Count > 0 Then
            For Each prodBaseDB In prodBaseDT.Rows
                Dim prodBase As New BabsProdotto1
                prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
                prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
                prodBase.CodTenoreZucc = inserisciValoreLineare((prodBaseDB.Item("CodTenoreZucchero")))
                If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                    prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                    prodBase.TitoloAlcolTotSpecified = True
                End If
                If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                    prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
                    prodBase.TitoloAlcolEffSpecified = True
                End If
                Dim prodotto As New BabsProdotto1Prodotto
                prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New BabsDesignProd1)
                prodBase.Prodotto = prodotto
                prodottibabs1.Add(prodBase)
            Next
            opObj.BabsProdotto1 = prodottibabs1.ToArray
        End If

        Dim prodBaseDT2 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "BabsProdotto2")
        Dim prodottiBabs2 As New List(Of BabsProdotto2)
        If prodBaseDT2.Rows.Count > 0 Then
            For Each prodBaseDB In prodBaseDT2.Rows
                Dim prodBase As New BabsProdotto2
                prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
                prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
                If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                    prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                    prodBase.TitoloAlcolTotSpecified = True
                End If
                If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                    prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
                    prodBase.TitoloAlcolEffSpecified = True
                End If
                Dim prodotto As New BabsProdotto2Prodotto
                prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New BabsDesignProd2)
                prodBase.Prodotto = prodotto
                prodottiBabs2.Add(prodBase)
            Next
            opObj.BabsProdotto2 = prodottiBabs2.ToArray
        End If

        Dim prodBaseDT3 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "BabsProdotto3")
        If prodBaseDT3.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT3.Rows(0)
            Dim prodBase As New BabsProdotto3
            prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                prodBase.TitoloAlcolTotSpecified = True
            End If
            If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
                prodBase.TitoloAlcolEffSpecified = True
            End If
            If prodBaseDB.Item("PercIgp") IsNot Nothing AndAlso (prodBaseDB.Item("PercIgp") <> 0) Then
                prodBase.PercIgp = inserisciValoreDouble((prodBaseDB.Item("PercIgp")))
                prodBase.PercIgpSpecified = True
            End If
            Dim prodotto As New BabsProdotto3Prodotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New BabsDesignProd3)
            prodBase.Prodotto = prodotto
            opObj.BabsProdotto3 = prodBase
        End If

    End Sub

    Private Sub getProdottiDISA(opObj As DisaOperazione, codiceOperazione As String, ByRef errore As Boolean)
        Dim prodBaseDT = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "DisaProdotto1")
        If prodBaseDT.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT.Rows(0)
            Dim prodBase As New DisaProdotto1
            prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))

            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            prodBase.CodTenoreZucc = inserisciValoreLineare((prodBaseDB.Item("CodTenoreZucchero")))
            If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                prodBase.TitoloAlcolTotSpecified = True
            End If
            If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
                prodBase.TitoloAlcolEffSpecified = True
            End If
            If prodBaseDB.Item("TitoloAlcolPot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolPot") <> 0) Then
                prodBase.TitoloAlcolPot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolPot"))
                prodBase.TitoloAlcolPotSpecified = True
            End If
            If prodBaseDB.Item("PercIgp") IsNot Nothing AndAlso (prodBaseDB.Item("PercIgp") <> 0) Then
                prodBase.PercIgp = inserisciValoreDouble((prodBaseDB.Item("PercIgp")))
                prodBase.PercIgpSpecified = True
            End If
            Dim prodotto As New DisaProdotto1Prodotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New DisaDesignProd1)
            prodBase.Prodotto = prodotto
            opObj.DisaProdotto1 = prodBase
        End If

        Dim prodBaseDT2 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "DisaProdotto2")
        If prodBaseDT2.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT2.Rows(0)
            Dim prodBase As New DisaProdotto2
            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            Dim prodotto As New DisaProdotto2Prodotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New DisaDesignProd2)
            prodBase.Prodotto = prodotto
            opObj.DisaProdotto2 = prodBase
        End If

        Dim prodBaseDT3 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "DisaProdotto3")
        If prodBaseDT3.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT3.Rows(0)
            Dim prodBase As New DisaProdotto3
            prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))

            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            prodBase.CodTenoreZucc = inserisciValoreLineare((prodBaseDB.Item("CodTenoreZucchero")))
            If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                prodBase.TitoloAlcolTotSpecified = True
            End If
            If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
                prodBase.TitoloAlcolEffSpecified = True
            End If
            If prodBaseDB.Item("TitoloAlcolPot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolPot") <> 0) Then
                prodBase.TitoloAlcolPot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolPot"))
                prodBase.TitoloAlcolPotSpecified = True
            End If
            If prodBaseDB.Item("PercIgp") IsNot Nothing AndAlso (prodBaseDB.Item("PercIgp") <> 0) Then
                prodBase.PercIgp = inserisciValoreDouble((prodBaseDB.Item("PercIgp")))
                prodBase.PercIgpSpecified = True
            End If
            Dim prodotto As New DisaProdotto3Prodotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New DisaDesignProd3)
            prodBase.Prodotto = prodotto
            opObj.DisaProdotto3 = prodBase
        End If
    End Sub

    Private Sub getProdottiLIEL(opObj As LielOperazione, codiceOperazione As String, ByRef errore As Boolean)
        Dim prodBaseDT = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "LielProdotto1")
        Dim prodottiLiel As New List(Of LielProdotto1)
        If prodBaseDT.Rows.Count > 0 Then
            For Each prodBaseDB In prodBaseDT.Rows
                Dim prodBase As New LielProdotto1
                prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))

                prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
                If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                    prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                    prodBase.TitoloAlcolTotSpecified = True
                End If
                If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                    prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
                    prodBase.TitoloAlcolEffSpecified = True
                End If
                If prodBaseDB.Item("TitoloAlcolPot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolPot") <> 0) Then
                    prodBase.TitoloAlcolPot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolPot"))
                    prodBase.TitoloAlcolPotSpecified = True
                End If
                If prodBaseDB.Item("PercIgp") IsNot Nothing AndAlso (prodBaseDB.Item("PercIgp") <> 0) Then
                    prodBase.PercIgp = inserisciValoreDouble((prodBaseDB.Item("PercIgp")))
                    prodBase.PercIgpSpecified = True
                End If
                Dim prodotto As New LielProdotto1Prodotto
                prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New LielDesignProd1)
                prodBase.Prodotto = prodotto
                prodottiLiel.Add(prodBase)
            Next
            opObj.LielProdotto1 = prodottiLiel.ToArray
        End If

        Dim prodBaseDT2 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "LielProdotto2")
        Dim prodottiLiel2 As New List(Of LielProdotto2)
        If prodBaseDT2.Rows.Count > 0 Then
            For Each prodBaseDB In prodBaseDT2.Rows
                Dim prodBase As New LielProdotto2
                prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
                prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
                If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                    prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                    prodBase.TitoloAlcolTotSpecified = True
                End If
                If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                    prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
                    prodBase.TitoloAlcolEffSpecified = True
                End If
                If prodBaseDB.Item("TitoloAlcolPot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolPot") <> 0) Then
                    prodBase.TitoloAlcolPot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolPot"))
                    prodBase.TitoloAlcolPotSpecified = True
                End If
                Dim prodotto As New LielProdotto2Prodotto
                prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New LielDesignProd2)
                prodBase.Prodotto = prodotto
                prodottiLiel2.Add(prodBase)
            Next
            opObj.LielProdotto2 = prodottiLiel2.ToArray
        End If

        Dim prodBaseDT3 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "LielProdotto3")
        If prodBaseDT3.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT3.Rows(0)
            Dim prodBase As New LielProdotto3
            prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                prodBase.TitoloAlcolTotSpecified = True
            End If
            If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
                prodBase.TitoloAlcolEffSpecified = True
            End If
            If prodBaseDB.Item("PercIgp") IsNot Nothing AndAlso (prodBaseDB.Item("PercIgp") <> 0) Then
                prodBase.PercIgp = inserisciValoreDouble((prodBaseDB.Item("PercIgp")))
                prodBase.PercIgpSpecified = True
            End If
            Dim prodotto As New LielProdotto3Prodotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New LielDesignProd3)
            prodBase.Prodotto = prodotto
            opObj.LielProdotto3 = prodBase
        End If

    End Sub

    Private Sub getProdottiEVAL(opObj As EvalOperazione, codiceOperazione As String, ByRef errore As Boolean)
        Dim prodBaseDT = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "EvalProdotto1")
        If prodBaseDT.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT.Rows(0)
            Dim prodBase As New EvalProdotto1
            prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                prodBase.TitoloAlcolTotSpecified = True
            End If
            If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
                prodBase.TitoloAlcolEffSpecified = True
            End If
            
            If prodBaseDB.Item("PercIgp") IsNot Nothing AndAlso (prodBaseDB.Item("PercIgp") <> 0) Then
                prodBase.PercIgp = inserisciValoreDouble((prodBaseDB.Item("PercIgp")))
                prodBase.PercIgpSpecified = True
            End If
            Dim prodotto As New EvalProdotto1Prodotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New EvalDesignProd1)
            prodBase.Prodotto = prodotto
            opObj.EvalProdotto1 = prodBase
        End If

        Dim prodBaseDT2 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "EvalProdotto2")
        If prodBaseDT2.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT2.Rows(0)
            Dim prodBase As New EvalProdotto2
            prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                prodBase.TitoloAlcolTotSpecified = True
            End If
            If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
            End If
            Dim prodotto As New EvalProdotto2Prodotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New EvalDesignProd2)
            prodBase.Prodotto = prodotto
            opObj.EvalProdotto2 = prodBase
        End If

        Dim prodBaseDT3 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "EvalProdotto3")
        If prodBaseDT3.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT3.Rows(0)
            Dim prodBase As New EvalProdotto3
            prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                prodBase.TitoloAlcolTotSpecified = True
            End If
            If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
                prodBase.TitoloAlcolEffSpecified = True
            End If
            Dim prodotto As New EvalProdotto3Prodotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New EvalDesignProd3)
            prodBase.Prodotto = prodotto
            opObj.EvalProdotto3 = prodBase
        End If
    End Sub

    Private Sub getProdottiFRAB(opObj As FrabOperazione, codiceOperazione As String, ByRef errore As Boolean)
        Dim prodBaseDT = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "FrabProdotto1")
        Dim prodottiFrab1 As New List(Of FrabProdotto1)
        If prodBaseDT.Rows.Count > 0 Then
            For Each prodBaseDB In prodBaseDT.Rows
                Dim prodBase As New FrabProdotto1
                prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
                prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
                If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                    prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                    prodBase.TitoloAlcolTotSpecified = True
                End If
                If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                    prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
                    prodBase.TitoloAlcolEffSpecified = True
                End If
                If prodBaseDB.Item("TitoloAlcolPot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolPot") <> 0) Then
                    prodBase.TitoloAlcolPot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolPot"))
                    prodBase.TitoloAlcolPotSpecified = True
                End If
                If prodBaseDB.Item("PercIgp") IsNot Nothing AndAlso (prodBaseDB.Item("PercIgp") <> 0) Then
                    prodBase.PercIgp = inserisciValoreDouble((prodBaseDB.Item("PercIgp")))
                    prodBase.PercIgpSpecified = True
                End If
                Dim prodotto As New FrabProdotto1Prodotto
                prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New FrabDesignProd1)
                prodBase.Prodotto = prodotto
                prodottiFrab1.Add(prodBase)
            Next
            opObj.FrabProdotto1 = prodottiFrab1.ToArray
        End If

        Dim prodBaseDT2 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "FrabProdotto2")
        Dim prodottiFrab2 As New List(Of FrabProdotto2)
        If prodBaseDT2.Rows.Count > 0 Then
            For Each prodBaseDB In prodBaseDT2.Rows
                Dim prodBase As New FrabProdotto2
                prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
                prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
                If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                    prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                    prodBase.TitoloAlcolTotSpecified = True
                End If
                If prodBaseDB.Item("TitoloAlcolPot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolPot") <> 0) Then
                    prodBase.TitoloAlcolPot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolPot"))
                    prodBase.TitoloAlcolPotSpecified = True
                End If
                If prodBaseDB.Item("PercIgp") IsNot Nothing AndAlso (prodBaseDB.Item("PercIgp") <> 0) Then
                    prodBase.PercIgp = inserisciValoreDouble((prodBaseDB.Item("PercIgp")))
                    prodBase.PercIgpSpecified = True
                End If
                Dim prodotto As New FrabProdotto2Prodotto
                prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New FrabDesignProd2)
                prodBase.Prodotto = prodotto
                prodottiFrab2.Add(prodBase)
            Next
            opObj.FrabProdotto2 = prodottiFrab2.ToArray
        End If

        Dim prodBaseDT3 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "FrabProdotto3")
        If prodBaseDT3.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT3.Rows(0)
            Dim prodBase As New FrabProdotto3
            prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            prodBase.CodTenoreZucc = inserisciValoreLineare((prodBaseDB.Item("CodTenoreZucchero")))
            If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
            End If
            If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
                prodBase.TitoloAlcolEffSpecified = True
            End If
            If prodBaseDB.Item("PercIgp") IsNot Nothing AndAlso (prodBaseDB.Item("PercIgp") <> 0) Then
                prodBase.PercIgp = inserisciValoreDouble((prodBaseDB.Item("PercIgp")))
                prodBase.PercIgpSpecified = True
            End If
            Dim prodotto As New FrabProdotto3Prodotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New FrabDesignProd3)
            prodBase.Prodotto = prodotto
            opObj.FrabProdotto3 = prodBase
        End If

    End Sub

    Private Sub getProdottiAARD(opObj As AardOperazione, codiceOperazione As String, ByRef errore As Boolean)
        Dim prodBaseDT = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "AardProdotto1")
        If prodBaseDT.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT.Rows(0)
            Dim prodBase As New AardProdotto1
            prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                prodBase.TitoloAlcolTotSpecified = True
            End If
            If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
                prodBase.TitoloAlcolEffSpecified = True
            End If
            If prodBaseDB.Item("TitoloAlcolPot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolPot") <> 0) Then
                prodBase.TitoloAlcolPot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolPot"))
                prodBase.TitoloAlcolPotSpecified = True
            End If
            If prodBaseDB.Item("PercIgp") IsNot Nothing AndAlso (prodBaseDB.Item("PercIgp") <> 0) Then
                prodBase.PercIgp = inserisciValoreDouble((prodBaseDB.Item("PercIgp")))
                prodBase.PercIgpSpecified = True
            End If
            Dim prodotto As New AardProdotto1Prodotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New AardDesignProd1)
            prodBase.Prodotto = prodotto
            opObj.AardProdotto1 = prodBase
        End If

        Dim prodBaseDT2 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "AardProdotto2")
        If prodBaseDT2.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT2.Rows(0)
            Dim prodBase As New AardProdotto2
            prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                prodBase.TitoloAlcolTotSpecified = True
            End If
            If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
                prodBase.TitoloAlcolEffSpecified = True
            End If
            If prodBaseDB.Item("TitoloAlcolPot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolPot") <> 0) Then
                prodBase.TitoloAlcolPot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolPot"))
                prodBase.TitoloAlcolPotSpecified = True
            End If
            If prodBaseDB.Item("PercIgp") IsNot Nothing AndAlso (prodBaseDB.Item("PercIgp") <> 0) Then
                prodBase.PercIgp = inserisciValoreDouble((prodBaseDB.Item("PercIgp")))
                prodBase.PercIgpSpecified = True
            End If
            Dim prodotto As New AardProdotto2Prodotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New AardDesignProd2)
            prodBase.Prodotto = prodotto
            opObj.AardProdotto2 = prodBase
        End If
    End Sub

    Private Sub getProdottiSCZC(opObj As SczcOperazione, codiceOperazione As String, ByRef errore As Boolean)
        Dim prodBaseDT = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "SczcProdotto1")
        Dim prodottiSczc1 As New List(Of SczcProdotto1)
        If prodBaseDT.Rows.Count > 0 Then
            For Each prodBaseDB In prodBaseDT.Rows
                Dim prodBase As New SczcProdotto1
                prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
                prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
                If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                    prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                    prodBase.TitoloAlcolTotSpecified = True
                End If
                If prodBaseDB.Item("TitoloAlcolPot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolPot") <> 0) Then
                    prodBase.TitoloAlcolPot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolPot"))
                    prodBase.TitoloAlcolPotSpecified = True
                End If
                If prodBaseDB.Item("PercIgp") IsNot Nothing AndAlso (prodBaseDB.Item("PercIgp") <> 0) Then
                    prodBase.PercIgp = inserisciValoreDouble((prodBaseDB.Item("PercIgp")))
                    prodBase.PercIgpSpecified = True
                End If
                Dim prodotto As New SczcProdotto1Prodotto
                prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New SczcDesignProd1)
                prodBase.Prodotto = prodotto
                prodottiSczc1.Add(prodBase)
            Next
            opObj.SczcProdotto1 = prodottiSczc1.ToArray
        End If

        Dim prodBaseDT2 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "SczcProdotto2")
        If prodBaseDT2.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT2.Rows(0)
            Dim prodBase As New SczcProdotto2
            prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            Dim prodotto As New SczcProdotto2Prodotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New SczcDesignProd2)
            prodBase.Prodotto = prodotto
            opObj.SczcProdotto2 = prodBase
        End If

    End Sub

    Private Sub getProdottiFRGS(opObj As FrgsOperazione, codiceOperazione As String, ByRef errore As Boolean)
        Dim prodBaseDT = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "FrgsProdotto1")
        Dim prodottiFrgs1 As New List(Of FrgsProdotto1)
        If prodBaseDT.Rows.Count > 0 Then
            For Each prodBaseDB In prodBaseDT.Rows
                Dim prodBase As New FrgsProdotto1
                prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
                prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
                If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                    prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                    prodBase.TitoloAlcolTotSpecified = True
                End If
                If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                    prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
                    prodBase.TitoloAlcolEffSpecified = True
                End If
                If prodBaseDB.Item("TitoloAlcolPot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolPot") <> 0) Then
                    prodBase.TitoloAlcolPot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolPot"))
                    prodBase.TitoloAlcolPotSpecified = True
                End If
                Dim prodotto As New FrgsProdotto1Prodotto
                prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New FrgsDesignProd1)
                prodBase.Prodotto = prodotto
                prodottiFrgs1.Add(prodBase)
            Next
            opObj.FrgsProdotto1 = prodottiFrgs1.ToArray
        End If

        Dim prodBaseDT2 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "FrgsProdotto2")
        If prodBaseDT2.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT2.Rows(0)
            Dim prodBase As New FrgsProdotto2
            prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            prodBase.CodTenoreZucc = inserisciValoreLineare((prodBaseDB.Item("CodTenoreZucchero")))
            If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                prodBase.TitoloAlcolTotSpecified = True
            End If
            If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
                prodBase.TitoloAlcolEffSpecified = True
            End If
            Dim prodotto As New FrgsProdotto2Prodotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New FrgsDesignProd2)
            prodBase.Prodotto = prodotto
            opObj.FrgsProdotto2 = prodBase
        End If

    End Sub

    Private Sub getProdottiELMC(opObj As ElmcOperazione, codiceOperazione As String, ByRef errore As Boolean)
        Dim prodBaseDT = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "ElmcProdotto1")
        If prodBaseDT.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT.Rows(0)
            Dim prodBase As New ElmcProdotto1
            prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            Dim prodotto As New ElmcProdotto1Prodotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New ElmcDesignProd1)
            prodBase.Prodotto = prodotto
            opObj.ElmcProdotto1 = prodBase
        End If


        Dim prodBaseDT2 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "ElmcProdotto2")
        If prodBaseDT2.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT2.Rows(0)
            Dim prodBase As New ElmcProdotto2
            prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            If prodBaseDB.Item("TitoloAlcolPot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolPot") <> 0) Then
                prodBase.TitoloAlcolPot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolPot"))
                prodBase.TitoloAlcolPotSpecified = True
            End If
            Dim prodotto As New ElmcProdotto2Prodotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New ElmcDesignProd2)
            prodBase.Prodotto = prodotto
            opObj.ElmcProdotto2 = prodBase
        End If
    End Sub

    Private Sub getProdottiSCDS(opObj As ScdsOperazione, codiceOperazione As String, ByRef errore As Boolean)
        Dim prodBaseDT = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "ScdsProdotto1")
        If prodBaseDT.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT.Rows(0)
            Dim prodBase As New ScdsProdotto1
            prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            prodBase.CodTenoreZucc = inserisciValoreLineare((prodBaseDB.Item("CodTenoreZucchero")))
            If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                prodBase.TitoloAlcolTotSpecified = True
            End If
            If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
            End If
            If prodBaseDB.Item("PercIgp") IsNot Nothing AndAlso (prodBaseDB.Item("PercIgp") <> 0) Then
                prodBase.PercIgp = inserisciValoreDouble((prodBaseDB.Item("PercIgp")))
                prodBase.PercIgpSpecified = True
            End If
            Dim prodotto As New ScdsProdotto1Prodotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New ScdsDesignProd1)
            prodBase.Prodotto = prodotto
            opObj.ScdsProdotto1 = prodBase
        End If

        Dim prodBaseDT2 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "ScdsProdotto2")
        Dim prodottiScds2 As New List(Of ScdsProdotto2)
        If prodBaseDT2.Rows.Count > 0 Then
            For Each prodBaseDB In prodBaseDT2.Rows
                Dim prodBase As New ScdsProdotto2
                prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
                prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
                prodBase.Partita = inserisciValorePartita(prodBaseDB)
                If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                    prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                    prodBase.TitoloAlcolTotSpecified = True
                End If
                If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                    prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
                    prodBase.TitoloAlcolEffSpecified = True
                End If
                If prodBaseDB.Item("TitoloAlcolPot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolPot") <> 0) Then
                    prodBase.TitoloAlcolPot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolPot"))
                    prodBase.TitoloAlcolPotSpecified = True
                End If           
                Dim prodotto As New ScdsProdotto2Prodotto
                prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New ScdsDesignProd2)
                prodBase.Prodotto = prodotto
                prodottiScds2.Add(prodBase)
            Next
            opObj.ScdsProdotto2 = prodottiScds2.ToArray
        End If

        Dim prodBaseDT3 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "ScdsProdotto3")
        If prodBaseDT3.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT3.Rows(0)
            Dim prodBase As New ScdsProdotto3
            prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            prodBase.CodTenoreZucc = inserisciValoreLineare((prodBaseDB.Item("CodTenoreZucchero")))
            If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                prodBase.TitoloAlcolTotSpecified = True
            End If
            If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
            End If
            Dim prodotto As New ScdsProdotto3Prodotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New ScdsDesignProd3)
            prodBase.Prodotto = prodotto
            opObj.ScdsProdotto3 = prodBase
        End If
    End Sub

    Private Sub getProdottiSPGS(opObj As SpgsOperazione, codiceOperazione As String, ByRef errore As Boolean)

        Dim prodBaseDT = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "SpgsProdotto1")
        Dim prodottiSpgs1 As New List(Of SpgsProdotto1)
        If prodBaseDT.Rows.Count > 0 Then
            For Each prodBaseDB In prodBaseDT.Rows
                Dim prodBase As New SpgsProdotto1
                prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
                prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
                If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                    prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                    prodBase.TitoloAlcolTotSpecified = True
                End If
                If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                    prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
                    prodBase.TitoloAlcolEffSpecified = True
                End If
                Dim prodotto As New SpgsProdotto1Prodotto
                prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New SpgsDesignProd1)
                prodBase.Prodotto = prodotto
                prodottiSpgs1.Add(prodBase)
            Next
            opObj.SpgsProdotto1 = prodottiSpgs1.ToArray
        End If

        Dim prodBaseDT2 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "SpgsProdotto2")
        If prodBaseDT2.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT2.Rows(0)
            Dim prodBase As New SpgsProdotto2
            prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            prodBase.CodTenoreZucc = inserisciValoreLineare((prodBaseDB.Item("CodTenoreZucchero")))
            If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                prodBase.TitoloAlcolTotSpecified = True
            End If
            If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
                prodBase.TitoloAlcolEffSpecified = True
            End If
            Dim prodotto As New SpgsProdotto2Prodotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New SpgsDesignProd2)
            prodBase.Prodotto = prodotto
            opObj.SpgsProdotto2 = prodBase
        End If
    End Sub

    Private Sub getProdottiSPAB(opObj As SpabOperazione, codiceOperazione As String, ByRef errore As Boolean)
        '1..n
        Dim prodBaseDT = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "SpabProdotto1")
        Dim prodottiSpab1 As New List(Of SpabProdotto1)
        If prodBaseDT.Rows.Count > 0 Then
            For Each prodBaseDB In prodBaseDT.Rows
                Dim prodBase As New SpabProdotto1
                prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
                prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
                If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                    prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                    prodBase.TitoloAlcolTotSpecified = True
                End If
                If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                    prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
                    prodBase.TitoloAlcolEffSpecified = True
                End If
                If prodBaseDB.Item("TitoloAlcolPot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolPot") <> 0) Then
                    prodBase.TitoloAlcolPot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolPot"))
                    prodBase.TitoloAlcolPotSpecified = True
                End If
                If prodBaseDB.Item("PercIgp") IsNot Nothing AndAlso (prodBaseDB.Item("PercIgp") <> 0) Then
                    prodBase.PercIgp = inserisciValoreDouble((prodBaseDB.Item("PercIgp")))
                    prodBase.PercIgpSpecified = True
                End If
                Dim prodotto As New SpabProdotto1Prodotto
                prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New SpabDesignProd1)
                prodBase.Prodotto = prodotto
                prodottiSpab1.Add(prodBase)
            Next
            opObj.SpabProdotto1 = prodottiSpab1.ToArray
        End If

        '0,1
        Dim prodBaseDT2 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "SpabProdotto2")
        If prodBaseDT2.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT2.Rows(0)
            Dim prodBase As New SpabProdotto2
            prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            Dim prodotto As New SpabProdotto2Prodotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New SpabDesignProd2)
            prodBase.Prodotto = prodotto
            opObj.SpabProdotto2 = prodBase
        End If

        '0..n
        Dim prodBaseDT3 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "SpabProdotto3")
        Dim prodottiSpab3 As New List(Of SpabProdotto3)
        If prodBaseDT3.Rows.Count > 0 Then
            For Each prodBaseDB In prodBaseDT3.Rows
                Dim prodBase As New SpabProdotto3
                prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
                prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
                If prodBaseDB.Item("TitoloAlcolPot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolPot") <> 0) Then
                    prodBase.TitoloAlcolPot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolPot"))
                    prodBase.TitoloAlcolPotSpecified = True
                End If
                Dim prodotto As New SpabProdotto3Prodotto
                prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New SpabDesignProd3)
                prodBase.Prodotto = prodotto
                prodottiSpab3.Add(prodBase)
            Next
            opObj.SpabProdotto3 = prodottiSpab3.ToArray
        End If

        '0..n
        Dim prodBaseDT4 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "SpabProdotto4")
        Dim prodottiSpab4 As New List(Of SpabProdotto4)
        If prodBaseDT4.Rows.Count > 0 Then
            For Each prodBaseDB In prodBaseDT4.Rows
                Dim prodBase As New SpabProdotto4
                prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))

                prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
                If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                    prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                    prodBase.TitoloAlcolTotSpecified = True
                End If
                If prodBaseDB.Item("PercIgp") IsNot Nothing AndAlso (prodBaseDB.Item("PercIgp") <> 0) Then
                    prodBase.PercIgp = inserisciValoreDouble((prodBaseDB.Item("PercIgp")))
                    prodBase.PercIgpSpecified = True
                End If
                Dim prodotto As New SpabProdotto4Prodotto
                prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New SpabDesignProd4)
                prodBase.Prodotto = prodotto
                prodottiSpab4.Add(prodBase)
            Next
            opObj.SpabProdotto4 = prodottiSpab4.ToArray
        End If

        '1
        Dim prodBaseDT5 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "SpabProdotto5")
        If prodBaseDT5.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT5.Rows(0)
            Dim prodBase As New SpabProdotto5
            prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))

            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            prodBase.CodTenoreZucc = inserisciValoreLineare((prodBaseDB.Item("CodTenoreZucchero")))
            If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
            End If
            If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
                prodBase.TitoloAlcolEffSpecified = True
            End If
            If prodBaseDB.Item("PercIgp") IsNot Nothing AndAlso (prodBaseDB.Item("PercIgp") <> 0) Then
                prodBase.PercIgp = inserisciValoreDouble((prodBaseDB.Item("PercIgp")))
                prodBase.PercIgpSpecified = True
            End If
            Dim prodotto As New SpabProdotto5Prodotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New SpabDesignProd5)
            prodBase.Prodotto = prodotto
            opObj.SpabProdotto5 = prodBase
        End If

    End Sub

    Private Sub getProdottiAVLT(opObj As AvltOperazione, codiceOperazione As String, ByRef errore As Boolean)
        Dim prodBaseDT = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "AvltProdotto1")
        Dim prodottiAvlt1 As New List(Of AvltProdotto1)
        If prodBaseDT.Rows.Count > 0 Then
            For Each prodBaseDB In prodBaseDT.Rows
                Dim prodBase As New AvltProdotto1
                prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
                If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                    prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                    prodBase.TitoloAlcolTotSpecified = True
                End If
                If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                    prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
                    prodBase.TitoloAlcolEffSpecified = True
                End If
                If prodBaseDB.Item("TitoloAlcolPot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolPot") <> 0) Then
                    prodBase.TitoloAlcolPot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolPot"))
                    prodBase.TitoloAlcolPotSpecified = True
                End If
                Dim prodotto As New AvltProdotto1Prodotto
                prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New AvltDesignProd1)
                prodBase.Prodotto = prodotto
                prodottiAvlt1.Add(prodBase)
            Next
            opObj.AvltProdotto1 = prodottiAvlt1.ToArray
        End If

        Dim prodBaseDT2 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "AvltProdotto2")
        If prodBaseDT2.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT2.Rows(0)
            Dim prodBase As New AvltProdotto2
            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            Dim prodotto As New AvltProdotto2Prodotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New AvltDesignProd2)
            prodBase.Prodotto = prodotto
            opObj.AvltProdotto2 = prodBase
        End If

        Dim prodBaseDT3 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "AvltProdotto3")
        If prodBaseDT3.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT3.Rows(0)
            Dim prodBase As New AvltProdotto3
            prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
            End If
            Dim prodotto As New AvltProdotto3Prodotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New AvltDesignProd3)
            prodBase.Prodotto = prodotto
            opObj.AvltProdotto3 = prodBase
        End If

    End Sub

    Private Sub getProdottiSFEC(opObj As SfecOperazione, codiceOperazione As String, ByRef errore As Boolean)
        Dim prodBaseDT = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "SfecProdotto1")
        If prodBaseDT.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT.Rows(0)
            Dim prodBase As New SfecProdotto1
            prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
            If prodBaseDB.Item("PercIgp") IsNot Nothing AndAlso (prodBaseDB.Item("PercIgp") <> 0) Then
                prodBase.PercIgp = inserisciValoreDouble((prodBaseDB.Item("PercIgp")))
                prodBase.PercIgpSpecified = True
            End If
            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            Dim prodotto As New SfecProdotto1Prodotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New SfecDesignProd1)
            prodBase.Prodotto = prodotto
            opObj.SfecProdotto1 = prodBase
        End If

        Dim prodBaseDT2 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "SfecProdotto2")
        If prodBaseDT2.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT2.Rows(0)
            Dim prodBase As New SfecProdotto2
            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            Dim prodotto As New SfecProdotto2Prodotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New SfecDesignProd2)
            prodBase.Prodotto = prodotto
            opObj.SfecProdotto2 = prodBase
        End If

        Dim prodBaseDT3 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "SfecProdotto3")
        Dim prodottiSfec3 As New List(Of SfecProdotto3)
        If prodBaseDT3.Rows.Count > 0 Then
            For Each prodBaseDB In prodBaseDT3.Rows
                Dim prodBase As New SfecProdotto3
                prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
                If prodBaseDB.Item("PercIgp") IsNot Nothing AndAlso (prodBaseDB.Item("PercIgp") <> 0) Then
                    prodBase.PercIgp = inserisciValoreDouble((prodBaseDB.Item("PercIgp")))
                    prodBase.PercIgpSpecified = True
                End If
                prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
                Dim prodotto As New SfecProdotto3Prodotto
                prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New SfecDesignProd3)
                prodBase.Prodotto = prodotto
                prodottiSfec3.Add(prodBase)
            Next
            opObj.SfecProdotto3 = prodottiSfec3.ToArray
        End If

    End Sub

    Private Sub getProdottiAUCO(opObj As AucoOperazione, codiceOperazione As String, ByRef errore As Boolean)
        Dim prodBaseDT = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "AucoProdotto1")
        If prodBaseDT.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT.Rows(0)
            Dim prodBase As New AucoProdotto1
            prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            prodBase.CodTenoreZucc = inserisciValoreLineare((prodBaseDB.Item("CodTenoreZucchero")))
            If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                prodBase.TitoloAlcolTotSpecified = True
            End If
            If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
                prodBase.TitoloAlcolEffSpecified = True
            End If
            If prodBaseDB.Item("TitoloAlcolPot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolPot") <> 0) Then
                prodBase.TitoloAlcolPot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolPot"))
                prodBase.TitoloAlcolPotSpecified = True
            End If
            If prodBaseDB.Item("PercIgp") IsNot Nothing AndAlso (prodBaseDB.Item("PercIgp") <> 0) Then
                prodBase.PercIgp = inserisciValoreDouble((prodBaseDB.Item("PercIgp")))
                prodBase.PercIgpSpecified = True
            End If
            Dim prodotto As New AucoProdotto1Prodotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New AucoDesignProd1)
            prodBase.Prodotto = prodotto
            opObj.AucoProdotto1 = prodBase
        End If
    End Sub

    Private Sub getProdottiPERD(opObj As PerdOperazione, codiceOperazione As String, ByRef errore As Boolean)
        Dim prodBaseDT = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "PerdProdotto1")
        If prodBaseDT.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT.Rows(0)
            Dim prodBase As New PerdProdotto1
            prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            prodBase.CodTenoreZucc = inserisciValoreLineare((prodBaseDB.Item("CodTenoreZucchero")))
            If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                prodBase.TitoloAlcolTotSpecified = True
            End If
            If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
                prodBase.TitoloAlcolEffSpecified = True
            End If
            If prodBaseDB.Item("TitoloAlcolPot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolPot") <> 0) Then
                prodBase.TitoloAlcolPot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolPot"))
                prodBase.TitoloAlcolPotSpecified = True
            End If
            If prodBaseDB.Item("PercIgp") IsNot Nothing AndAlso (prodBaseDB.Item("PercIgp") <> 0) Then
                prodBase.PercIgp = inserisciValoreDouble((prodBaseDB.Item("PercIgp")))
                prodBase.PercIgpSpecified = True
            End If
            Dim prodotto As New PerdProdotto1Prodotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New PerdDesignProd1)
            prodBase.Prodotto = prodotto
            opObj.PerdProdotto1 = prodBase
        End If
    End Sub

    Private Sub getProdottiDENT(opObj As DentOperazione, codiceOperazione As String, ByRef errore As Boolean)
        Dim prodBaseDT = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "DentProdotto1")
        If prodBaseDT.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT.Rows(0)
            Dim prodBase As New DentProdotto1
            prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            prodBase.CodTenoreZucc = inserisciValoreLineare((prodBaseDB.Item("CodTenoreZucchero")))
            If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                prodBase.TitoloAlcolTotSpecified = True
            End If
            If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
                prodBase.TitoloAlcolEffSpecified = True
            End If
            If prodBaseDB.Item("TitoloAlcolPot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolPot") <> 0) Then
                prodBase.TitoloAlcolPot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolPot"))
                prodBase.TitoloAlcolPotSpecified = True
            End If
            If prodBaseDB.Item("PercIgp") IsNot Nothing AndAlso (prodBaseDB.Item("PercIgp") <> 0) Then
                prodBase.PercIgp = inserisciValoreDouble((prodBaseDB.Item("PercIgp")))
                prodBase.PercIgpSpecified = True
            End If
            Dim prodotto As New DentProdotto1Prodotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New DentDesignProd1)
            prodBase.Prodotto = prodotto
            opObj.DentProdotto1 = prodBase
        End If

        Dim prodBaseDT2 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "DentProdotto2")
        If prodBaseDT2.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT2.Rows(0)
            Dim prodBase As New DentProdotto2
            Dim prodotto As New DentProdotto2Prodotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New DentDesignProd2)
            prodBase.Prodotto = prodotto
            opObj.DentProdotto2 = prodBase
        End If

        Dim prodBaseDT3 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "DentProdotto3")
        If prodBaseDT3.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT3.Rows(0)
            Dim prodBase As New DentProdotto3
            prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))

            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            prodBase.CodTenoreZucc = inserisciValoreLineare((prodBaseDB.Item("CodTenoreZucchero")))
            If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                prodBase.TitoloAlcolTotSpecified = True
            End If
            If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
                prodBase.TitoloAlcolEffSpecified = True
            End If
            If prodBaseDB.Item("TitoloAlcolPot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolPot") <> 0) Then
                prodBase.TitoloAlcolPot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolPot"))
                prodBase.TitoloAlcolPotSpecified = True
            End If
            If prodBaseDB.Item("PercIgp") IsNot Nothing AndAlso (prodBaseDB.Item("PercIgp") <> 0) Then
                prodBase.PercIgp = inserisciValoreDouble((prodBaseDB.Item("PercIgp")))
                prodBase.PercIgpSpecified = True
            End If
            Dim prodotto As New DentProdotto3Prodotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New DentDesignProd3)
            prodBase.Prodotto = prodotto
            opObj.DentProdotto3 = prodBase
        End If
    End Sub

    Private Sub getProdottiCERT(opObj As CertOperazione, codiceOperazione As String, ByRef errore As Boolean)
        Dim prodBaseDT = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "CertProdotto1")
        Dim Qta As Double = 0
        Dim partite As New List(Of TipoPartitaNoContr)
        Dim i = 1
        Dim prodBaseI As New CertProdotto1
        Dim prodottoI As New CertProdotto1Prodotto
        For Each ProdBaseDB In prodBaseDT.Rows
            If i = 1 Then
                prodBaseI.CodRecipiente = CodRecipienti((ProdBaseDB.Item("vasi")))
                'prodBaseI.Quantita = inserisciValoreDouble((ProdBaseDB.Item("Qta")))
                prodBaseI.CodTenoreZucc = inserisciValoreLineare((ProdBaseDB.Item("CodTenoreZucchero")))
                'prodBaseI.Partita = inserisciValorePartitaNoContr(ProdBaseDB)
                If ProdBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (ProdBaseDB.Item("TitoloAlcolTot") <> 0) Then
                    prodBaseI.TitoloAlcolTot = inserisciValoreDouble(ProdBaseDB.Item("TitoloAlcolTot"))
                    prodBaseI.TitoloAlcolTotSpecified = True
                End If
                If ProdBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (ProdBaseDB.Item("TitoloAlcolEff") <> 0) Then
                    prodBaseI.TitoloAlcolEff = inserisciValoreDouble(ProdBaseDB.Item("TitoloAlcolEff"))
                    prodBaseI.TitoloAlcolEffSpecified = True
                End If
                Qta += inserisciValoreDouble((ProdBaseDB.Item("Qta")))
                partite.Add(inserisciValorePartitaNoContr(ProdBaseDB))
                prodottoI.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(ProdBaseDB), New CertDesignProd1)
            Else
                Qta += inserisciValoreDouble((ProdBaseDB.Item("Qta")))
                partite.Add(inserisciValorePartitaNoContr(ProdBaseDB))
            End If
            i += 1
        Next
        prodBaseI.Quantita = Qta
        prodBaseI.Partita = partite.ToArray
        prodBaseI.Prodotto = prodottoI
        opObj.CertProdotto1 = prodBaseI

        Qta = 0
        Dim partite1 As New List(Of TipoPartita)
        i = 1
        Dim prodBaseI1 As New CertProdotto2
        Dim prodottoI1 As New CertProdotto2Prodotto
        Dim prodBaseDT1 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "CertProdotto2")
        For Each ProdBaseDB In prodBaseDT1.Rows
            If i = 1 Then
                Dim prodBase As New CertProdotto2
                prodBase.CodRecipiente = CodRecipienti((ProdBaseDB.Item("vasi")))
                prodBase.CodTenoreZucc = inserisciValoreLineare((ProdBaseDB.Item("CodTenoreZucchero")))
                If ProdBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (ProdBaseDB.Item("TitoloAlcolTot") <> 0) Then
                    prodBase.TitoloAlcolTot = inserisciValoreDouble(ProdBaseDB.Item("TitoloAlcolTot"))
                    prodBase.TitoloAlcolTotSpecified = True
                End If
                If ProdBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (ProdBaseDB.Item("TitoloAlcolEff") <> 0) Then
                    prodBase.TitoloAlcolEff = inserisciValoreDouble(ProdBaseDB.Item("TitoloAlcolEff"))
                    prodBase.TitoloAlcolEffSpecified = True
                End If
                prodottoI1.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(ProdBaseDB), New CertDesignProd2)
                Qta += inserisciValoreDouble((ProdBaseDB.Item("Qta")))
                Dim partiteS = inserisciValorePartita(ProdBaseDB)
                If partiteS IsNot Nothing Then
                    For Each part In partiteS
                        partite1.Add(part)
                    Next
                End If
            Else
                Qta += inserisciValoreDouble((ProdBaseDB.Item("Qta")))
                Dim partiteS = inserisciValorePartita(ProdBaseDB)
                If partiteS IsNot Nothing Then
                    For Each part In partiteS
                        partite1.Add(part)
                    Next
                End If
            End If
            i += 1
        Next
        prodBaseI1.Quantita = Qta
        prodBaseI1.Partita = partite1.ToArray
        prodBaseI1.Prodotto = prodottoI1
        opObj.CertProdotto2 = prodBaseI1

        'If prodBaseDT1.Rows.Count > 0 Then
        '    Dim prodBaseDB = prodBaseDT1.Rows(0)
        '    Dim prodBase As New CertProdotto2
        '    prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
        '    prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
        '    prodBase.CodTenoreZucc = inserisciValoreLineare((prodBaseDB.Item("CodTenoreZucchero")))
        '    prodBase.Partita = inserisciValorePartita(prodBaseDB)
        '    If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
        '        prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
        '        prodBase.TitoloAlcolTotSpecified = True
        '    End If
        '    If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
        '        prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
        '        prodBase.TitoloAlcolEffSpecified = True
        '    End If
        '    Dim prodotto As New CertProdotto2Prodotto
        '    prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New CertDesignProd2)
        '    prodBase.Prodotto = prodotto
        '    opObj.CertProdotto2 = prodBase
        'End If

    End Sub

    Private Sub getProdottiDOLC(opObj As DolcOperazione, codiceOperazione As String, ByRef errore As Boolean)
        Dim prodBaseDT = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "DolcProdotto1")
        If prodBaseDT.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT.Rows(0)
            Dim prodBase As New DolcProdotto1
            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
            If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
            End If
            If prodBaseDB.Item("PercIgp") IsNot Nothing AndAlso (prodBaseDB.Item("PercIgp") <> 0) Then
                prodBase.PercIgp = inserisciValoreDouble((prodBaseDB.Item("PercIgp")))
                prodBase.PercIgpSpecified = True
            End If
            Dim prodotto As New DolcProdotto1Prodotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New DolcDesignProd1)
            prodBase.Prodotto = prodotto
            opObj.DolcProdotto1 = prodBase
        End If

        Dim prodBaseDT2 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "DolcProdotto2")
        Dim listaDolcProdotto2 As New List(Of DolcProdotto2)
        If prodBaseDT2.Rows.Count > 0 Then
            For Each prodBaseDB In prodBaseDT2.Rows
                'Dim prodBaseDB = prodBaseDT2.Rows(0)
                Dim prodBase As New DolcProdotto2
                prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
                prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))

                If prodBaseDB.Item("TitoloAlcolPot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolPot") <> 0) Then
                    prodBase.TitoloAlcolPot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolPot"))
                End If
                Dim prodotto As New DolcProdotto2Prodotto
                prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New DolcDesignProd2)
                prodBase.Prodotto = prodotto
                listaDolcProdotto2.Add(prodBase)
            Next
            opObj.DolcProdotto2 = listaDolcProdotto2.ToArray
        End If

        Dim prodBaseDT3 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "DolcProdotto3")
        If prodBaseDT3.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT3.Rows(0)
            Dim prodBase As New DolcProdotto3
            prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))

            End If
            If prodBaseDB.Item("PercIgp") IsNot Nothing AndAlso (prodBaseDB.Item("PercIgp") <> 0) Then
                prodBase.PercIgp = inserisciValoreDouble((prodBaseDB.Item("PercIgp")))
                prodBase.PercIgpSpecified = True
            End If
            Dim prodotto As New DolcProdotto3Prodotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New DolcDesignProd3)
            prodBase.Prodotto = prodotto
            opObj.DolcProdotto3 = prodBase
        End If
    End Sub

    Private Sub getProdottiTAGL(opObj As TaglOperazione, codiceOperazione As String, ByRef errore As Boolean)
        Dim prodBaseDT = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "TaglProdotto1")
        Dim prodottiTagl1 As New List(Of TaglProdotto1)
        If prodBaseDT.Rows.Count > 0 Then
            For Each prodBaseDB In prodBaseDT.Rows
                Dim prodBase As New TaglProdotto1
                prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
                prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
                prodBase.CodTenoreZucc = inserisciValoreLineare((prodBaseDB.Item("CodTenoreZucchero")))
                If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                    prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                    prodBase.TitoloAlcolTotSpecified = True
                End If
                If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                    prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
                    prodBase.TitoloAlcolEffSpecified = True
                End If
                If prodBaseDB.Item("PercIgp") IsNot Nothing AndAlso (prodBaseDB.Item("PercIgp") <> 0) Then
                    prodBase.PercIgp = inserisciValoreDouble((prodBaseDB.Item("PercIgp")))
                    prodBase.PercIgpSpecified = True
                End If
                Dim prodotto As New TaglProdotto1Prodotto
                prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New TaglDesignProd1)
                prodBase.Prodotto = prodotto
                prodottiTagl1.Add(prodBase)
            Next
            opObj.TaglProdotto1 = prodottiTagl1.ToArray
        End If

        Dim prodBaseDT1 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "TaglProdotto2")
        If prodBaseDT1.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT1.Rows(0)
            Dim prodBase As New TaglProdotto2
            prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            prodBase.CodTenoreZucc = inserisciValoreLineare((prodBaseDB.Item("CodTenoreZucchero")))
            If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                prodBase.TitoloAlcolTotSpecified = True
            End If
            If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
                prodBase.TitoloAlcolEffSpecified = True
            End If
            If prodBaseDB.Item("PercIgp") IsNot Nothing AndAlso (prodBaseDB.Item("PercIgp") <> 0) Then
                prodBase.PercIgp = inserisciValoreDouble((prodBaseDB.Item("PercIgp")))
                prodBase.PercIgpSpecified = True
            End If
            Dim prodotto As New TaglProdotto2Prodotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New TaglDesignProd2)
            prodBase.Prodotto = prodotto
            opObj.TaglProdotto2 = prodBase
        End If

    End Sub

    Private Sub getProdottiSVIN(opObj As SvinOperazione, codiceOperazione As String, ByRef errore As Boolean)
        Dim prodBaseDT = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "SvinProdotto1")
        If prodBaseDT.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT.Rows(0)
            Dim prodBase As New SvinProdotto1
            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
            If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                prodBase.TitoloAlcolTotSpecified = True
            End If
            If prodBaseDB.Item("TitoloAlcolPot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolPot") <> 0) Then
                prodBase.TitoloAlcolPot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolPot"))
                prodBase.TitoloAlcolPotSpecified = True
            End If
            If prodBaseDB.Item("PercIgp") IsNot Nothing AndAlso (prodBaseDB.Item("PercIgp") <> 0) Then
                prodBase.PercIgp = inserisciValoreDouble((prodBaseDB.Item("PercIgp")))
                prodBase.PercIgpSpecified = True
            End If
            Dim prodotto As New SvinProdotto1SvinDesignProd1
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New SvinDesignProd1)
            prodBase.SvinDesignProd1 = prodotto
            opObj.SvinProdotto1 = prodBase
        End If

        Dim prodBaseDT2 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "SvinProdotto2")
        If prodBaseDT2.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT2.Rows(0)
            Dim prodBase As New SvinProdotto2
            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
                prodBase.TitoloAlcolEffSpecified = True
            End If
            Dim prodotto As New SvinProdotto2Prodotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New SvinDesignProd2)
            prodBase.Prodotto = prodotto
            opObj.SvinProdotto2 = prodBase
        End If

        Dim prodBaseDT3 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "SvinProdotto3")
        If prodBaseDT3.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT3.Rows(0)
            Dim prodBase As New SvinProdotto3
            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            Dim prodotto As New SvinProdotto3SvinDesignProd3
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New SvinDesignProd3)
            prodBase.SvinDesignProd3 = prodotto
            opObj.SvinProdotto3 = prodBase
        End If

        Dim prodBaseDT4 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "SvinProdotto4")
        If prodBaseDT4.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT4.Rows(0)
            Dim prodBase As New SvinProdotto4
            prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
            If prodBaseDB.Item("PercIgp") IsNot Nothing AndAlso (prodBaseDB.Item("PercIgp") <> 0) Then
                prodBase.PercIgp = inserisciValoreDouble((prodBaseDB.Item("PercIgp")))
                prodBase.PercIgpSpecified = True
            End If
            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            Dim prodotto As New SvinProdotto4SvinDesignProd4
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New SvinDesignProd4)
            prodBase.SvinDesignProd4 = prodotto
            opObj.SvinProdotto4 = prodBase
        End If
    End Sub

    Private Sub getProdottiIMBO(opObj As ImboOperazione, codiceOperazione As String, ByRef errore As Boolean)
        Dim prodBaseDT = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "ProdottoImboS")
        If prodBaseDT.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT.Rows(0)
            Dim prodBase As New ProdottoImboS
            prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            prodBase.CodTenoreZucc = inserisciValoreLineare((prodBaseDB.Item("CodTenoreZucchero")))
            If prodBaseDB.Item("QuantitaPerdita") IsNot Nothing AndAlso (prodBaseDB.Item("QuantitaPerdita") <> 0) Then
                prodBase.QuantitaPerdita = inserisciValoreDouble((prodBaseDB.Item("QuantitaPerdita")))
                prodBase.QuantitaPerditaSpecified = True
            End If
            If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                prodBase.TitoloAlcolTotSpecified = True
            End If
            If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
                prodBase.TitoloAlcolEffSpecified = True
            End If
            If prodBaseDB.Item("TitoloAlcolPot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolPot") <> 0) Then
                prodBase.TitoloAlcolPot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolPot"))
                prodBase.TitoloAlcolPotSpecified = True
            End If
            If prodBaseDB.Item("PercIgp") IsNot Nothing AndAlso (prodBaseDB.Item("PercIgp") <> 0) Then
                prodBase.PercIgp = inserisciValoreDouble((prodBaseDB.Item("PercIgp")))
                prodBase.PercIgpSpecified = True
            End If
            Dim prodotto As New ProdottoImboSProdImboS1
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New ProdottoImboA)
            prodBase.ProdImboS1 = prodotto
            opObj.ProdImboS = prodBase
        End If

        Dim prodBaseDT1 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "ProdottoImboI")
        If prodBaseDT1.Rows.Count = 0 Then
            prodBaseDT1 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "ProdottoImbol")
        End If
        Dim Qta As Double = 0
        Dim partite As New List(Of TipoPartitaImbo)
        Dim i = 1
        Dim prodBaseI As New ProdottoImboI
        Dim prodottoI As New ProdottoImboIProdImboI1
        For Each ProdBaseDB In prodBaseDT1.Rows
            If i = 1 Then
                Qta += inserisciValoreDouble((ProdBaseDB.Item("Qta")))
                prodBaseI.CodTenoreZucc = inserisciValoreLineare((ProdBaseDB.Item("CodTenoreZucchero")))
                partite.Add(inserisciValorePartitaImbo(ProdBaseDB))
                If ProdBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (ProdBaseDB.Item("TitoloAlcolTot") <> 0) Then
                    prodBaseI.TitoloAlcolTot = inserisciValoreDouble(ProdBaseDB.Item("TitoloAlcolTot"))
                    prodBaseI.TitoloAlcolTotSpecified = True
                End If
                If ProdBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (ProdBaseDB.Item("TitoloAlcolEff") <> 0) Then
                    prodBaseI.TitoloAlcolEff = inserisciValoreDouble(ProdBaseDB.Item("TitoloAlcolEff"))
                    prodBaseI.TitoloAlcolEffSpecified = True
                End If
                If ProdBaseDB.Item("TitoloAlcolPot") IsNot Nothing AndAlso (ProdBaseDB.Item("TitoloAlcolPot") <> 0) Then
                    prodBaseI.TitoloAlcolPot = inserisciValoreDouble(ProdBaseDB.Item("TitoloAlcolPot"))
                    prodBaseI.TitoloAlcolPotSpecified = True
                End If
                prodottoI.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(ProdBaseDB), New ProdottoImboB)
            Else
                Qta += inserisciValoreDouble((ProdBaseDB.Item("Qta")))
                partite.Add(inserisciValorePartitaImbo(ProdBaseDB))
            End If
            i += 1
        Next
        prodBaseI.Quantita = Qta
        prodBaseI.Partita = partite.ToArray
        prodBaseI.ProdImboI1 = prodottoI
        opObj.ProdImboI = prodBaseI

        'If prodBaseDT1.Rows.Count > 0 Then
        '    Dim prodBaseDB = prodBaseDT1.Rows(0)
        '    Dim prodBase As New ProdottoImboI
        '    prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
        '    prodBase.CodTenoreZucc = inserisciValoreLineare((prodBaseDB.Item("CodTenoreZucchero")))
        '    prodBase.Partita = inserisciValorePartitaImbo(prodBaseDB)
        '    If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
        '        prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
        '        prodBase.TitoloAlcolTotSpecified = True
        '    End If
        '    If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
        '        prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
        '        prodBase.TitoloAlcolEffSpecified = True
        '    End If
        '    If prodBaseDB.Item("TitoloAlcolPot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolPot") <> 0) Then
        '        prodBase.TitoloAlcolPot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolPot"))
        '        prodBase.TitoloAlcolPotSpecified = True
        '    End If
        '    Dim prodotto As New ProdottoImboIProdImboI1
        '    prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New ProdottoImboB)
        '    prodBase.ProdImboI1 = prodotto
        '    opObj.ProdImboI = prodBase
        'End If

    End Sub

    Private Sub getProdottiPIGI(opObj As PigiOperazione, codiceOperazione As String, ByRef errore As Boolean)
        Dim prodBaseDT = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "PigiProdotto1")
        If prodBaseDT.Rows.Count > 0 Then
            Dim Qta_Tot As Double = 0
            Dim prodBase As New PigiProdotto1
            For Each prodBaseDTR In prodBaseDT.Rows
                Dim prodBaseDB = prodBaseDTR
                Qta_Tot += inserisciValoreDouble((prodBaseDB.Item("Qta")))
                If prodBaseDB.Item("TitoloAlcolPot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolPot") <> 0) Then
                    prodBase.TitoloAlcolPot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolPot"))
                    prodBase.TitoloAlcolPotSpecified = True
                End If
                If prodBaseDB.Item("PercIgp") IsNot Nothing AndAlso (prodBaseDB.Item("PercIgp") <> 0) Then
                    prodBase.PercIgp = inserisciValoreDouble((prodBaseDB.Item("PercIgp")))
                    prodBase.PercIgpSpecified = True
                End If
                Dim prodotto As New PigiProdotto1PigiDesignProd1
                prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New PigiDesignProd1)
                prodBase.PigiDesignProd1 = prodotto
                opObj.PigiProdotto1 = prodBase
            Next
            prodBase.Quantita = Qta_Tot
        End If

        Dim prodBaseDT2 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "PigiProdotto2")
        If prodBaseDT2.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT2.Rows(0)
            Dim prodBase As New PigiProdotto2
            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            Dim prodotto As New PigiProdotto2PigiDesignProd2
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New PigiDesignProd2)
            prodBase.PigiDesignProd2 = prodotto
            opObj.PigiProdotto2 = prodBase
        End If

        Dim prodBaseDT3 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "PigiProdotto3")
        If prodBaseDT3.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT3.Rows(0)
            Dim prodBase As New PigiProdotto3
            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))

            If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
                prodBase.TitoloAlcolEffSpecified = True
            End If
            If prodBaseDB.Item("TitoloAlcolPot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolPot") <> 0) Then
                prodBase.TitoloAlcolPot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolPot"))
                prodBase.TitoloAlcolPotSpecified = True
            End If

            Dim prodotto As New PigiProdotto3Prodotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New PigiDesignProd3)
            prodBase.Prodotto = prodotto
            opObj.PigiProdotto3 = prodBase
        End If

        Dim prodBaseDT4 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "PigiProdotto4")
        If prodBaseDT4.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT4.Rows(0)
            Dim prodBase As New PigiProdotto4
            prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                prodBase.TitoloAlcolTotSpecified = True
            End If
            If prodBaseDB.Item("TitoloAlcolPot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolPot") <> 0) Then
                prodBase.TitoloAlcolPot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolPot"))
                prodBase.TitoloAlcolPotSpecified = True
            End If
            If prodBaseDB.Item("PercIgp") IsNot Nothing AndAlso (prodBaseDB.Item("PercIgp") <> 0) Then
                prodBase.PercIgp = inserisciValoreDouble((prodBaseDB.Item("PercIgp")))
                prodBase.PercIgpSpecified = True
            End If
            Dim prodotto As New PigiProdotto4PigiDesignProd4
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New PigiDesignProd4)
            prodBase.PigiDesignProd4 = prodotto
            opObj.PigiProdotto4 = prodBase
        End If


    End Sub

    Private Sub getProdottiUSSD(opObj As UssdOperazione, codiceOperazione As String, ByRef errore As Boolean)
        Dim prodBaseDT = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "ProdUssd")
        Dim prodottiUssd As New List(Of ProdottoUssd)
        If prodBaseDT.Rows.Count > 0 Then
            For Each prodBaseDB In prodBaseDT.Rows
                Dim prodBase As New ProdottoUssd
                prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
                prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
                prodBase.CodTenoreZucc = inserisciValoreLineare((prodBaseDB.Item("CodTenoreZucchero")))
                prodBase.Partita = inserisciValorePartita(prodBaseDB)
                If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                    prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                    prodBase.TitoloAlcolTotSpecified = True
                End If
                If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                    prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
                    prodBase.TitoloAlcolEffSpecified = True
                End If
                If prodBaseDB.Item("TitoloAlcolPot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolPot") <> 0) Then
                    prodBase.TitoloAlcolPot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolPot"))
                    prodBase.TitoloAlcolPotSpecified = True
                End If
                If prodBaseDB.Item("PercIgp") IsNot Nothing AndAlso (prodBaseDB.Item("PercIgp") <> 0) Then
                    prodBase.PercIgp = inserisciValoreDouble((prodBaseDB.Item("PercIgp")))
                    prodBase.PercIgpSpecified = True
                End If
                Dim prodotto As New ProdottoUssdProdUssd1
                prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New ProdottoTipoB)
                prodBase.ProdUssd1 = prodotto
                prodottiUssd.Add(prodBase)
            Next
            opObj.ProdUssd = prodottiUssd.ToArray
        End If
    End Sub

    Private Sub getProdottiCASD(opObj As CasdOperazione, codiceOperazione As String, ByRef errore As Boolean)
        Dim prodBaseDT = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "ProdCasd")
        Dim prodottiCasd As New List(Of ProdottoCasd)
        If prodBaseDT.Rows.Count > 0 Then
            For Each prodBaseDB In prodBaseDT.Rows
                Dim prodBase As New ProdottoCasd
                prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
                prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
                prodBase.CodTenoreZucc = inserisciValoreLineare((prodBaseDB.Item("CodTenoreZucchero")))
                Dim gg = inserisciValoreInteger(prodBaseDB.Item("GgInvecchiamento"))
                If gg <> 0 Then
                    prodBase.GgInvecchiamento = gg
                End If
                prodBase.Partita = inserisciValorePartita(prodBaseDB)
                If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                    prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                    prodBase.TitoloAlcolTotSpecified = True
                End If
                If prodBaseDB.Item("TitoloAlcolEff") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolEff") <> 0) Then
                    prodBase.TitoloAlcolEff = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolEff"))
                    prodBase.TitoloAlcolEffSpecified = True
                End If
                If prodBaseDB.Item("TitoloAlcolPot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolPot") <> 0) Then
                    prodBase.TitoloAlcolPot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolPot"))
                    prodBase.TitoloAlcolPotSpecified = True
                End If
                If prodBaseDB.Item("PercIgp") IsNot Nothing AndAlso (prodBaseDB.Item("PercIgp") <> 0) Then
                    prodBase.PercIgp = inserisciValoreDouble((prodBaseDB.Item("PercIgp")))
                    prodBase.PercIgpSpecified = True
                End If
                Dim prodotto As New ProdottoCasdProdCasd1
                prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New ProdottoTipoA)
                prodBase.ProdCasd1 = prodotto
                prodottiCasd.Add(prodBase)
            Next
            opObj.ProdCasd = prodottiCasd.ToArray
        End If
    End Sub

    Private Sub getProdottiACID(opObj As AcidOperazione, codiceOperazione As String, ByRef errore As Boolean)
        Dim prodBaseDT = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "AcidProdotto1")
        If prodBaseDT.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT.Rows(0)
            Dim prodBase As New AcidProdotto1
            prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            prodBase.CodTenoreZucc = inserisciValoreLineare((prodBaseDB.Item("CodTenoreZucchero")))
            If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                prodBase.TitoloAlcolTotSpecified = True
            End If
            If prodBaseDB.Item("PercIgp") IsNot Nothing AndAlso (prodBaseDB.Item("PercIgp") <> 0) Then
                prodBase.PercIgp = inserisciValoreDouble((prodBaseDB.Item("PercIgp")))
                prodBase.PercIgpSpecified = True
            End If
            Dim prodotto As New AcidProdotto1Prodotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New AcidDesignProd1)
            prodBase.Prodotto = prodotto
            opObj.AcidProdotto1 = prodBase
        End If

        Dim prodBaseDT2 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "AcidProdotto2")
        If prodBaseDT2.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT2.Rows(0)
            Dim prodBase As New AcidProdotto2
            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            Dim prodotto As New AcidProdotto2Prodotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New AcidDesignProd2)
            prodBase.Prodotto = prodotto
            opObj.AcidProdotto2 = prodBase
        End If

        Dim prodBaseDT3 = reader.leggiProdotti(ObjParametri_Server, codiceOperazione, "AcidProdotto3")
        If prodBaseDT3.Rows.Count > 0 Then
            Dim prodBaseDB = prodBaseDT3.Rows(0)
            Dim prodBase As New AcidProdotto3
            prodBase.CodRecipiente = CodRecipienti((prodBaseDB.Item("vasi")))
            prodBase.Quantita = inserisciValoreDouble((prodBaseDB.Item("Qta")))
            prodBase.CodTenoreZucc = inserisciValoreLineare((prodBaseDB.Item("CodTenoreZucchero")))
            prodBase.MetodoPE = inserisciValoreLineare((prodBaseDB.Item("MetodoPE")))
            If prodBaseDB.Item("TitoloAlcolTot") IsNot Nothing AndAlso (prodBaseDB.Item("TitoloAlcolTot") <> 0) Then
                prodBase.TitoloAlcolTot = inserisciValoreDouble(prodBaseDB.Item("TitoloAlcolTot"))
                prodBase.TitoloAlcolTotSpecified = True
            End If
            If prodBaseDB.Item("PercIgp") IsNot Nothing AndAlso (prodBaseDB.Item("PercIgp") <> 0) Then
                prodBase.PercIgp = inserisciValoreDouble((prodBaseDB.Item("PercIgp")))
                prodBase.PercIgpSpecified = True
            End If
            Dim prodotto As New AcidProdotto3Prodotto
            prodotto.Item = inserisciProdottoTipoB(inserisciCodiceProdottoTipoB(prodBaseDB), New AcidDesignProd3)
            prodBase.Prodotto = prodotto
            opObj.AcidProdotto3 = prodBase
        End If

    End Sub

#End Region

#Region "Designazione Prodotti"

    Private Function inserisciProdottoGiin1(row As DataRow) As ProdottoGiin1
        Dim des As New ProdottoGiin1
        des.CodCategoria = inserisciValoreLinear(row.Item("CodCategoria"))
        des.AttoCert = inserisciValoreLinear(row.Item("AttoCert"))
        des.CodClassificazione = inserisciValoreLinear(row.Item("CodClassificazione"))
        des.CodDopIgp = inserisciValoreLinear(row.Item("CodDopIgp"))
        des.CodEbacchus = inserisciValoreLinear(row.Item("CodEbacchus"))
        des.OrigineUve = inserisciValoreLinear(row.Item("OrigineUve"))
        Dim provenienza = inserisciValoreLinear(row.Item("Provenienza"))
        If provenienza = "IT" Then
            des.Provenienza = "01"
        Else
            des.Provenienza = inserisciValoreLinear(row.Item("Provenienza"))
        End If
        des.PaesiProvenienza = inserisciPaesiProvenienza(row.Item("PaesiProvenienza"))
        des.CodZonaViticola = inserisciValoreLinear(row.Item("CodZonaViticola"))
        des.Varieta = inserisciCod_ValorePerc(row.Item("Varieta"))
        des.AltreVarieta = inserisciValoreLinear(row.Item("AltreVarieta"))
        des.CodSottozona = inserisciValoreLinear(row.Item("CodSottozona"))
        des.CodVigna = inserisciValoreLinear(row.Item("CodVigna"))
        des.CodColore = inserisciValoreLinear(row.Item("CodColore"))
        des.Menzioni = inserisciMenzioni(row.Item("Menzioni"))
        Dim biologico As String = inserisciValoreLinear(row.Item("Biologico"))
        If biologico <> "0" Then
            des.Biologico = inserisciValoreLinear(row.Item("Biologico"))
        End If
        des.PraticheEnologiche = inserisciPraticheEnologiche(row.Item("PraticheEnologiche"))
        des.CodPartita = inserisciValoreLinear(row.Item("CodPartita"))
        des.Annata = inserisciTipoAnnata(row.Item("Annata"))
        des.MassaVolumica = CDbl(inserisciValoreLinear(row.Item("MassaVolumica")))
        des.CodStatoFisico = inserisciValoreLinear(row.Item("CodStatoFisico"))
        Dim datacertDop As Date? = inserisciValoreDate(row.Item("DataCertDOP"))
        If datacertDop IsNot Nothing AndAlso datacertDop <> CDate("31-12-2016") Then
            des.DataCertDOP = datacertDop
        End If
        des.NumCertDOP = ControlloCertificato(inserisciValoreLinear(row.Item("NumCertDOP")))
        Return des
    End Function

    Private Function inserisciCodiceProdottoGiin1(row As DataRow) As ProdottoGiin1
        Dim des As New ProdottoGiin1
        des.CodCategoria = inserisciValoreLinear(row.Item("CodCategoria"))
        des.AttoCert = inserisciValoreLinear(row.Item("AttoCert"))
        des.CodClassificazione = inserisciValoreLinear(row.Item("CodClassificazione"))
        des.CodDopIgp = inserisciValoreLinear(row.Item("CodDopIgp"))
        des.CodEbacchus = inserisciValoreLinear(row.Item("CodEbacchus"))
        des.OrigineUve = inserisciValoreLinear(row.Item("OrigineUve"))
        Dim provenienza = inserisciValoreLinear(row.Item("Provenienza"))
        If provenienza = "IT" Then
            des.Provenienza = "01"
        Else
            des.Provenienza = inserisciValoreLinear(row.Item("Provenienza"))
        End If
        des.PaesiProvenienza = inserisciPaesiProvenienza(row.Item("PaesiProvenienza"))
        des.CodZonaViticola = inserisciValoreLinear(row.Item("CodZonaViticola"))
        des.Varieta = inserisciCod_ValorePerc(row.Item("Varieta"))
        des.AltreVarieta = inserisciValoreLinear(row.Item("AltreVarieta"))
        des.CodSottozona = inserisciValoreLinear(row.Item("CodSottozona"))
        des.CodVigna = inserisciValoreLinear(row.Item("CodVigna"))
        des.CodColore = inserisciValoreLinear(row.Item("CodColore"))
        des.Menzioni = inserisciMenzioni(row.Item("Menzioni"))
        Dim biologico As String = inserisciValoreLinear(row.Item("Biologico"))
        If biologico <> "0" Then
            des.Biologico = inserisciValoreLinear(row.Item("Biologico"))
        End If
        des.PraticheEnologiche = inserisciPraticheEnologiche(row.Item("PraticheEnologiche"))
        des.CodPartita = inserisciValoreLinear(row.Item("CodPartita"))
        des.Annata = inserisciTipoAnnata(row.Item("Annata"))
        des.MassaVolumica = CDbl(inserisciValoreLinear(row.Item("MassaVolumica")))
        des.CodStatoFisico = inserisciValoreLinear(row.Item("CodStatoFisico"))
        Dim datacertDop As Date? = inserisciValoreDate(row.Item("DataCertDOP"))
        If datacertDop IsNot Nothing AndAlso datacertDop <> CDate("31-12-2016") Then
            des.DataCertDOP = datacertDop
        End If
        des.NumCertDOP = ControlloCertificato(inserisciValoreLinear(row.Item("NumCertDOP")))
        Return des
    End Function

    Private Function inserisciProdottoTipoB(Of T)(val As ProdottoTipoB, returnVal As T) As T
        Dim xmlTipoB = XMLUtility.getStringFromObject(val)
        Dim prodottoTipoBStr = val.GetType.Name
        Dim ProdGiin1Str = returnVal.GetType.Name
        xmlTipoB = xmlTipoB.Replace(prodottoTipoBStr, ProdGiin1Str)
        returnVal = XMLUtility.getObjectFromXml(xmlTipoB, returnVal)
        Return returnVal
    End Function


    Private Function inserisciCodiceProdottoTipoB(row As DataRow) As ProdottoTipoB
        Dim des As New ProdottoTipoB
        des.CodCategoria = inserisciCategoria(inserisciValoreLinear(row.Item("CodCategoria")))
        If Not IsDBNull(row.Item("AttoCert")) AndAlso row.Item("AttoCert") <> "" Then
            des.AttoCert = CStr(row.Item("AttoCert"))
        End If
        des.CodClassificazione = inserisciValoreLinear(row.Item("CodClassificazione"))
        If row.Item("CodDopIgp") IsNot Nothing AndAlso row.Item("CodDopIgp") <> "0" AndAlso row.Item("CodDopIgp") <> "" Then
            des.CodDopIgp = inserisciValoreLinear(row.Item("CodDopIgp"))
        End If
        des.CodEbacchus = inserisciValoreLinear(row.Item("CodEbacchus"))
        des.OrigineUve = inserisciValoreLinear(row.Item("OrigineUve"))
        Dim provenienza = inserisciValoreLinear(row.Item("Provenienza"))
        If provenienza = "IT" Then
            des.Provenienza = "01"
        Else
            des.Provenienza = inserisciValoreLinear(row.Item("Provenienza"))
        End If
        des.PaesiProvenienza = inserisciPaesiProvenienza(row.Item("PaesiProvenienza"))
        des.CodZonaViticola = inserisciValoreLinear(row.Item("CodZonaViticola"))
        des.Varieta = inserisciCod_ValorePerc(row)
        des.AltreVarieta = inserisciValoreLinear(row.Item("AltreVarieta"))
        des.CodSottozona = inserisciValoreLinear(row.Item("CodSottozona"))
        des.CodVigna = inserisciValoreLinear(row.Item("CodVigna"))
        des.CodColore = inserisciValoreLinear(row.Item("CodColore"))
        des.Menzioni = inserisciMenzioni(row.Item("Menzioni"))
        Dim biologico As String = inserisciValoreLinear(row.Item("Biologico"))
        If biologico <> "0" Then
            des.Biologico = inserisciValoreLinear(row.Item("Biologico"))
        End If
        des.PraticheEnologiche = inserisciPraticheEnologiche(row.Item("PraticheEnologiche"))
        des.CodPartita = inserisciValoreLinear(row.Item("CodPartita"))
        des.Annata = inserisciTipoAnnata(row)
        If CDbl(inserisciValoreLinear(row.Item("MassaVolumica"))) <> 0D Then
            des.MassaVolumica = CDbl(inserisciValoreLinear(row.Item("MassaVolumica")))
            des.MassaVolumicaSpecified = True
        End If
        des.CodStatoFisico = inserisciValoreLinear(row.Item("CodStatoFisico"))
        Dim datacertDop As Date? = inserisciValoreDate(row.Item("DataCertDOP"))
        Dim dataFinale As Date = #12/31/2100#
        If datacertDop IsNot Nothing AndAlso datacertDop <> dataFinale Then
            des.DataCertDOP = datacertDop
            des.DataCertDOPSpecified = True
        End If
        des.NumCertDOP = ControlloCertificato(inserisciValoreLinear(row.Item("NumCertDOP")))
        Return des
    End Function

    Public Function ControlloCertificato(ByRef value As String) As String
        If value IsNot Nothing Then
            value = value.Replace("°", " ")
        End If
        Return value
    End Function

    Public Function inserisciCategoria(ByRef value As String) As String
        Dim categoria As String = Nothing
        If value IsNot Nothing Then
            Try
                Dim intValue = CInt(value)
                If intValue < 10 Then
                    categoria = intValue.ToString("D2")
                Else
                    categoria = intValue
                End If
            Catch ex As Exception
                Return value
            End Try
        End If
        Return categoria
    End Function

    Public Function inserisciValoreDate(ByRef val As Object) As Date?
        If Not IsDBNull(val) Then
            If val IsNot Nothing Then
                If CStr(val) <> "" Then
                    Return CDate(val)
                Else
                    Return Nothing
                End If
            End If
        End If
        Return Nothing
    End Function

    Public Function inserisciValoreLinear(ByRef val As Object) As String
        If Not IsDBNull(val) Then
            If val IsNot Nothing Then
                If CStr(val) <> "" AndAlso CStr(val) <> "0" Then
                    Return val
                Else
                    Return Nothing
                End If
            End If
        End If
        Return Nothing
    End Function

    Public Function inserisciPaesiProvenienza(ByRef val As Object) As PaesiProvenienza()
        If Not IsDBNull(val) Then
            If val IsNot Nothing AndAlso val <> "0" Then
                Dim paesi As New List(Of PaesiProvenienza)
                Dim paesiVal As String = CStr(val)
                For Each strPaese In paesiVal.Split("|")
                    If strPaese IsNot Nothing AndAlso strPaese <> "" Then
                        Dim paese As New PaesiProvenienza
                        paese.Codice = strPaese
                        paesi.Add(paese)
                    End If
                Next
                Return paesi.ToArray
            End If
        End If
        Return Nothing
    End Function

    Public Function inserisciCod_ValorePerc(ByRef val As DataRow) As Cod_ValorePerc()
        Dim varietas = val.Item("varieta")
        If Not IsDBNull(varietas) Then
            Dim valori As New List(Of Cod_ValorePerc)
            If varietas IsNot Nothing AndAlso CStr(varietas) <> "" AndAlso CStr(varietas) <> "0" Then
                Dim valoriStrL As New List(Of String)
                Dim valoriStr = varietas.Split("|")
                valoriStrL.AddRange(valoriStr)
                Dim deleted = True
                While (deleted)
                    deleted = valoriStrL.Remove("")
                End While
                valoriStr = valoriStrL.ToArray
                Dim valoreAttuale As Integer = 0
                For Each strValore In valoriStr
                    If strValore <> "" AndAlso strValore <> "0" Then
                        Dim varieta As New Cod_ValorePerc
                        varieta.Codice = getValoreVarieta(strValore)
                        Dim perVarieta As Double = getValorePercVarieta(CStr(val.Item("percVarieta")), valoreAttuale)
                        If perVarieta <> 0 Then
                            varieta.Percentuale = perVarieta
                            varieta.PercentualeSpecified = True
                        End If
                        valori.Add(varieta)
                        valoreAttuale += 1
                    End If
                Next
                Return valori.ToArray
            End If
        End If
        Return Nothing
    End Function

    Public Function getValoreVarieta(ByRef value As String) As String
        Dim varieta As String = Nothing
        If value IsNot Nothing Then
            Dim intValue As Integer
            If Integer.TryParse(value, intValue) Then
                varieta = intValue.ToString("D3")
            Else
                varieta = value
            End If

        End If
        Return varieta
    End Function

    Public Function getValorePercVarieta(val As String, indice As Integer) As Double

        If val IsNot Nothing AndAlso val <> "" AndAlso val <> "0" Then
            Dim valoriStrL As New List(Of String)
            Dim valoriStr = val.Split("|")
            valoriStrL.AddRange(valoriStr)
            Dim deleted = True
            While (deleted)
                deleted = valoriStrL.Remove("")
            End While
            valoriStr = valoriStrL.ToArray
            If valoriStr.Count > indice Then
                Dim valStr = valoriStr(indice)
                If valStr IsNot Nothing AndAlso valStr <> "" AndAlso valStr <> "0" Then
                    Return CDbl(valStr)
                Else
                    Return 0
                End If
            Else
                Return 0
            End If
        Else
            Return 0
        End If
    End Function

    Public Function inserisciMenzioni(ByRef val As Object) As Menzioni()
        If Not IsDBNull(val) Then
            If val IsNot Nothing AndAlso CStr(val) <> "0" Then
                Dim menzioni As New List(Of Menzioni)
                Dim valStr = CStr(val)
                For Each strMenzione In valStr.Split("|")
                    If strMenzione IsNot Nothing AndAlso strMenzione <> "" Then
                        Dim menzione As New Menzioni
                        menzione.Codice = strMenzione
                        menzioni.Add(menzione)
                    End If
                Next
                Return menzioni.ToArray
            End If
        End If
        Return Nothing
    End Function

    Public Function inserisciPraticheEnologiche(ByRef val As Object) As PraticheEnologiche()
        If Not IsDBNull(val) Then
            If val IsNot Nothing AndAlso val <> "0" Then
                Dim pratiche As New List(Of PraticheEnologiche)
                Dim valStr = CStr(val)
                For Each strPratica In valStr.Split("|")
                    If strPratica IsNot Nothing AndAlso strPratica <> "" Then
                        Dim pratica As New PraticheEnologiche
                        pratica.Codice = getCodicePraticaEnologica(strPratica)
                        pratiche.Add(pratica)
                    End If
                Next
                Return pratiche.ToArray
            End If
        End If
        Return Nothing
    End Function

    Public Function getCodicePraticaEnologica(ByRef value As String) As String
        Dim pratica As String = Nothing
        If value IsNot Nothing Then
            Dim intValue As Integer
            If Integer.TryParse(value, intValue) Then
                pratica = CInt(value).ToString("D2")
            Else
                pratica = value
            End If
        End If
        Return pratica
    End Function

    Public Function inserisciTipoAnnata(ByRef val As DataRow) As TipoAnnata
        If Not IsDBNull(val.Item("annata")) Then
            Dim annata = val.Item("annata")
            If annata IsNot Nothing AndAlso CStr(annata) <> "" AndAlso CStr(annata) <> "0" Then
                Dim tAnnata As New TipoAnnata
                tAnnata.Codice = CInt(annata)
                Dim percannata = val.Item("PercAnnata")
                If Not IsDBNull(percannata) Then
                    If percannata IsNot Nothing AndAlso CStr(percannata) <> "" AndAlso CStr(percannata) <> "0" Then
                        tAnnata.Percentuale = CDbl(percannata)
                        tAnnata.PercentualeSpecified = True
                    End If
                End If
                Return tAnnata
            End If
            Return Nothing
        End If
        Return Nothing
    End Function
#End Region

#End Region

    Private Function getXmlTipoPartita(tagXml As String, operationCode As String, prodottoCode As String, row As DataRow) As String
        Dim xml As String = ""
        xml += "<" + tagXml + ">"
        Dim listaAttributiProdotto = reader.getAttributiProdotto(ObjParametri_Server, operationCode, prodottoCode, "tipoPartita")
        Dim modified As Integer = 0
        For Each attributo As String In listaAttributiProdotto
            Dim a As String
            a = getXmlCampo(attributo, operationCode, prodottoCode, "tipoPartita", row)
            If a <> "" Then
                xml += a
                modified += 1
            End If
        Next
        xml += "</" + tagXml + ">"
        If modified > 0 Then
            Return xml
        Else
            Return ""
        End If
    End Function

End Class
