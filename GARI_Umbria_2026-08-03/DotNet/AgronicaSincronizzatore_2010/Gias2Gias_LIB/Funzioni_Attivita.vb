Imports System.Xml
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports System.Text
Imports AgronicaCoreModello.Anagrafe
Imports AgronicaCoreModello
Imports AgronicaCoreG2GLocalDal
Imports AgronicaCoreUtility
Imports AgronicaCoreEntityFramework_POCO

Partial Public Class Funzioni

    Public Sub LeggiAttivitaXML(ByVal Piva_Origine As String,
                                     ByVal Piva_Destinazione As String,
                                     ByRef objOpzioni As clsOpzioni,
                                     ByRef Log_Import As StringBuilder,
                                     ByRef Log_Errori As StringBuilder,
                                     ByRef Log_Riepilogo As StringBuilder,
                                     ByVal Flag_Pubblico As Boolean,
                                     ByVal Configurazione As G2G_Configurazione_FiltriReq_Attivita,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                     ByRef listaImpreseAnalisi As List(Of String))

        Dim From_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_ORIGINE
        Dim To_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_DESTINAZIONE
        Dim From_Piva = Piva_Origine
        Dim To_Piva = Piva_Destinazione

        ' per i contatti pubblici forzo la piva destinazione
        If String.IsNullOrEmpty(Piva_Destinazione) Then
            To_Piva = To_PivaSuperUser
        End If

        Dim listaImprese = New List(Of String)
        Dim listaCodRapporto = New List(Of Integer)
        Dim dataValidita = AGRODATAINIZIO

        If Configurazione IsNot Nothing Then
            listaImprese = Configurazione.listaImprese
        End If

        Dim objLeggi As New G2GAttivita_R
        Dim g2g = objLeggi.Leggi_Attivita_G2G(From_Piva, objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser, Configurazione, objParametri, listaImpreseAnalisi)

        Dim sXmlAttivita As String = AgronicaCoreUtility.XMLUtility.SerializzaOggetto(Of G2G_Attivita)(g2g, "")

        Dim mainDoc As String = Funzioni_MasterG2G.XmlMasterDataPiva(enum_TipoOperazioneDB.Scrittura, Piva_Origine, "", objOpzioni)

        Dim sStringaDaSalvare As String = AgronicaCoreUtility.XDocUtils.IniettaSottoAlberoDaStringaXml(mainDoc, sXmlAttivita, "//utente/DatiGlobali", "DatiAttivita")


        Dim wsimportazione As New WS_Importa_GIAS_2014.ImportaWS With {.Url = objOpzioni.wsimportaGiasURl, .Timeout = _timeout_ws_Importa}
        Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)

        '  Marco Grilli, 17/06/2014 14:57:29: zippo la stringa in gzip per velocizzare il trasferimento
        sStringaDaSalvare = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, sStringaDaSalvare)

        Dim outputAttivita As String = wsimportazione.Scrivi_XmlFull(Str_Credenziali_WS, sStringaDaSalvare)

        'leggo la struttura restituita contenente i recode da inserire/modificare/cancellare
        Dim xmlRisposta As XDocument = XDocument.Parse(outputAttivita)
        Dim xmlRispostaAttivita As XElement = xmlRisposta.Element("Risposta").Element("Risposta_DatiAttivita")
        Dim rispostaAttivita = xmlRispostaAttivita.FirstNode.ToString
        'Dim rispostaParcoMacchine = AgroZip.DeCompressioneBase64(1, wsimportazione.Scrivi_ParcoMacchineXmlPrivato(Str_Credenziali_WS, AgroZip.CompressioneBase64(1, sXmlParcoMacchine)))
        Dim risposta As G2G_Recode = XMLUtility.DeserializzaOggetto(Of G2G_Recode)(rispostaAttivita, "")

        Dim MessaggioErrore As String = ""
        ' se la scrittura su destinazione è andata a buon fine aggiorno i recode sull'origine
        If String.IsNullOrEmpty(risposta.MessaggioErrore) Then
            MessaggioErrore = G2GUtility.Scrivi_G2G_Recode(risposta, objOpzioni.objParametri_Server_GIAS_ORIGINE)
        Else
            MessaggioErrore = risposta.MessaggioErrore
        End If

        ' log trasferimento dati
        If String.IsNullOrEmpty(MessaggioErrore) Then
            If Not String.IsNullOrEmpty(risposta.LogRecode) Then
                Log_Import.AppendLine("Trasferimento Ricette: " & risposta.LogRecode)
            End If
        Else
            Log_Import.AppendLine("Trasferimento Ricette ERRORE: " & MessaggioErrore)
        End If

        'nuove, modificate, eliminate
        Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Attivita: " & g2g.attivita_insert.Count & " nuovi " & g2g.attivita_update.Count & " modificati " & g2g.G2G_Attivita_Recode_delete.Count & " cancellati ")

    End Sub

    Public Sub LeggiAttivitaXMLReverse(ByVal Piva_Origine As String,
                                     ByVal Piva_Destinazione As String,
                                     ByRef objOpzioni As clsOpzioni,
                                     ByRef Log_Import As StringBuilder,
                                     ByRef Log_Errori As StringBuilder,
                                     ByRef Log_Riepilogo As StringBuilder,
                                     ByVal Flag_Pubblico As Boolean,
                                     ByVal Configurazione As G2G_Configurazione_FiltriReq_Attivita,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                     ByRef listaImpreseAnalisi As List(Of String))

        Dim From_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_ORIGINE
        Dim To_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_DESTINAZIONE
        Dim From_Piva = Piva_Origine
        Dim To_Piva = Piva_Destinazione

        ' per i contatti pubblici forzo la piva destinazione
        If String.IsNullOrEmpty(Piva_Destinazione) Then
            To_Piva = To_PivaSuperUser
        End If

        Dim listaImprese = New List(Of String)
        Dim listaCodRapporto = New List(Of Integer)
        Dim dataValidita = AGRODATAINIZIO

        If Configurazione IsNot Nothing Then
            listaImprese = Configurazione.listaImprese
        End If

        Dim objLeggi As New G2GAttivita_R
        Dim g2g = objLeggi.Leggi_Attivita_G2GReverse(From_Piva, objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser, Configurazione, objParametri, listaImpreseAnalisi)

        Dim sXmlAttivita As String = AgronicaCoreUtility.XMLUtility.SerializzaOggetto(Of G2G_Attivita_Reverse)(g2g, "")

        Dim mainDoc As String = Funzioni_MasterG2G.XmlMasterDataPiva(enum_TipoOperazioneDB.Scrittura, Piva_Origine, "", objOpzioni)

        Dim sStringaDaSalvare As String = AgronicaCoreUtility.XDocUtils.IniettaSottoAlberoDaStringaXml(mainDoc, sXmlAttivita, "//utente/DatiGlobali", "DatiAttivita")


        Dim wsimportazione As New WS_Importa_GIAS_2014.ImportaWS With {.Url = objOpzioni.wsimportaGiasURl, .Timeout = _timeout_ws_Importa}
        Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)

        '  Marco Grilli, 17/06/2014 14:57:29: zippo la stringa in gzip per velocizzare il trasferimento
        sStringaDaSalvare = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, sStringaDaSalvare)

        Dim outputAttivita As String = wsimportazione.Scrivi_XmlFull(Str_Credenziali_WS, sStringaDaSalvare)

        'leggo la struttura restituita contenente i recode da inserire/modificare/cancellare
        Dim xmlRisposta As XDocument = XDocument.Parse(outputAttivita)
        Dim xmlRispostaAttivita As XElement = xmlRisposta.Element("Risposta").Element("Risposta_DatiAttivita")
        Dim rispostaAttivita = xmlRispostaAttivita.FirstNode.ToString
        'Dim rispostaParcoMacchine = AgroZip.DeCompressioneBase64(1, wsimportazione.Scrivi_ParcoMacchineXmlPrivato(Str_Credenziali_WS, AgroZip.CompressioneBase64(1, sXmlParcoMacchine)))
        Dim risposta As G2G_Recode = XMLUtility.DeserializzaOggetto(Of G2G_Recode)(rispostaAttivita, "")

        Dim MessaggioErrore As String = ""
        ' se la scrittura su destinazione è andata a buon fine aggiorno i recode sull'origine
        If String.IsNullOrEmpty(risposta.MessaggioErrore) Then
            MessaggioErrore = G2GUtility.Scrivi_G2G_Recode(risposta, objOpzioni.objParametri_Server_GIAS_ORIGINE)
        Else
            MessaggioErrore = risposta.MessaggioErrore
        End If

        ' log trasferimento dati
        If String.IsNullOrEmpty(MessaggioErrore) Then
            If Not String.IsNullOrEmpty(risposta.LogRecode) Then
                Log_Import.AppendLine("Trasferimento Ricette: " & risposta.LogRecode)
            End If
        Else
            Log_Import.AppendLine("Trasferimento Ricette ERRORE: " & MessaggioErrore)
        End If

        'nuove, modificate, eliminate
        Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Attivita: " & g2g.attivita_insert.Count & " nuovi " & g2g.attivita_update.Count & " modificati " & g2g.G2G_Attivita_Recode_delete.Count & " cancellati ")

    End Sub

    Public Function recode_return_ID_Attivita(ByVal nodo_attivita As XmlNode,
                                         ByVal piva_destinazione As String,
                                         ByVal piva_SuperUser_destinazione As String
                                         ) As Integer

        Dim ID_Attivita As Integer
        If nodo_attivita.Attributes("ID_Attivita") IsNot Nothing Then
            ID_Attivita = nodo_attivita.Attributes("ID_Attivita").Value
        ElseIf nodo_attivita.Attributes("Id_Attivita") IsNot Nothing Then
            ID_Attivita = nodo_attivita.Attributes("Id_Attivita").Value
        Else
            ID_Attivita = nodo_attivita.Attributes("id_attivita").Value
        End If

        Return recode_return_ID_Attivita(ID_Attivita, piva_destinazione, piva_SuperUser_destinazione)

    End Function

    Public Function recode_return_ID_AttivitaReverse(ByVal nodo_attivita As XmlNode,
                                         ByVal piva_destinazione As String,
                                         ByVal piva_SuperUser_destinazione As String
                                         ) As Integer

        Dim ID_Attivita As Integer
        If nodo_attivita.Attributes("ID_Attivita") IsNot Nothing Then
            ID_Attivita = nodo_attivita.Attributes("ID_Attivita").Value
        ElseIf nodo_attivita.Attributes("Id_Attivita") IsNot Nothing Then
            ID_Attivita = nodo_attivita.Attributes("Id_Attivita").Value
        Else
            ID_Attivita = nodo_attivita.Attributes("id_attivita").Value
        End If

        Return recode_return_ID_AttivitaReverse(ID_Attivita, piva_destinazione, piva_SuperUser_destinazione)

    End Function

    Public Function recode_return_ID_Attivita(ByVal ID_Attivita As Integer,
                                         ByVal piva_destinazione As String,
                                         ByVal piva_SuperUser_destinazione As String
                                         ) As Integer

        Dim rval As Integer = 0

        If ID_Attivita <> 0 Then

            rval = (From rs In _efG2G.G2G_Recode_Attivita
                    Where rs.FromId_Attivita = ID_Attivita AndAlso
                                  rs.To_PivaSuperUser = piva_SuperUser_destinazione
                    Select rs.ToId_Attivita).FirstOrDefault

            If rval = 0 Then
                Dim messaggio As String
                messaggio = "ID_Attivita ORIGINE = " & CStr(ID_Attivita) &
                            " Errore, non esiste una decodifica dell'attività."

                Throw New Exception(messaggio)
            End If

        End If

        Return rval

    End Function

    Public Function recode_return_ID_AttivitaReverse(ByVal ID_Attivita As Integer,
                                         ByVal piva_destinazione As String,
                                         ByVal piva_SuperUser_destinazione As String
                                         ) As Integer

        Dim rval As Integer = 0

        If ID_Attivita <> 0 Then

            rval = (From rs In _efG2G.G2G_Recode_Attivita
                    Where rs.ToId_Attivita = ID_Attivita AndAlso
                                  rs.From_PivaSuperUser = piva_SuperUser_destinazione
                    Select rs.FromId_Attivita).FirstOrDefault

            If rval = 0 Then
                Dim messaggio As String
                messaggio = "ID_Attivita ORIGINE = " & CStr(ID_Attivita) &
                            " Errore, non esiste una decodifica dell'attività."

                Throw New Exception(messaggio)
            End If

        End If

        Return rval

    End Function
End Class
