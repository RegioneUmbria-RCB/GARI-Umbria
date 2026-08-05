Imports System.Xml
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class XML_Stampe

    Public Structure ElementoStampe
        Public Nome As String
        Public Valore As String
    End Structure

    '##########################################################################################
    'vecchio metodo
    Public Function XML_VariabiliStampe(ByVal vetVariabili() As ElementoStampe) As String


        Dim xmlDoc As XmlDocument
        Dim xmlTxt As XmlElement
        Dim xmlErrore As XmlElement

        Try

            xmlDoc = New XmlDocument

            If vetVariabili.Length > 0 Then

                'Creo il nodo 
                xmlTxt = xmlDoc.CreateElement("VariabiliStampe")

                For Each vetVariabile As ElementoStampe In vetVariabili

                    xmlTxt.SetAttribute(LCase(vetVariabile.Nome), vetVariabile.Valore)

                Next

                'Imposto xmlTxt come figlio del documento principale
                xmlDoc.AppendChild(xmlTxt)

            End If

        Catch ex As Exception
            xmlDoc = New XmlDocument
            xmlErrore = xmlDoc.CreateElement("VariabiliStampe")
            xmlErrore.SetAttribute("Errore", ex.Message)
            xmlDoc.AppendChild(xmlErrore)
        End Try

        'Restituisco in uscita la stringa creata
        Return xmlDoc.InnerXml

    End Function

    '###############################################################################################
    'vecchio metodo
    Public Sub XML_EstraiVariabiliStampe(ByVal strXml As String, ByRef htVariabiliStampe As Hashtable, ByRef strErr As String)

        Dim xmlDoc As XmlDocument

        Dim xmlVariabiliStampe As XmlElement
        Dim xmlAttributo As XmlAttribute

        Try

            'Carico la stringa xml in un nuovo documento
            xmlDoc = New XmlDocument
            xmlDoc.LoadXml(strXml)

            If xmlDoc.HasChildNodes Then

                htVariabiliStampe = New Hashtable

                'Recupero l'insieme dei nodi Movimento
                xmlVariabiliStampe = xmlDoc.FirstChild
                If xmlVariabiliStampe.HasAttributes Then
                    For Each xmlAttributo In xmlVariabiliStampe.Attributes()

                        If Not htVariabiliStampe.ContainsKey(xmlAttributo.Name) Then

                            htVariabiliStampe.Add(xmlAttributo.Name, xmlAttributo.Value)

                        End If

                    Next

                End If

            End If

        Catch ex As Exception
            strErr = ex.Message
            htVariabiliStampe = Nothing
        End Try

    End Sub

    '###############################################################################################
    '@Paolo: metodo per leggere anche gli attributi di nodi figli
    Public Sub XML_EstraiVariabiliStampe2(ByVal strXml As String, ByRef htVariabiliStampe As Hashtable, ByRef strErr As String)

        Dim xmlDoc As XmlDocument

        Dim xmlVariabiliStampe As XmlElement
        Dim xmlVariabiliStampe2 As XmlElement
        Dim xmlAttributo As XmlAttribute

        Try

            'Carico la stringa xml in un nuovo documento
            xmlDoc = New XmlDocument
            xmlDoc.LoadXml(strXml)

            If xmlDoc.HasChildNodes Then

                htVariabiliStampe = New Hashtable

                'Recupero l'insieme dei nodi Movimento
                xmlVariabiliStampe = xmlDoc.FirstChild
                If xmlVariabiliStampe.HasAttributes Then
                    For Each xmlAttributo In xmlVariabiliStampe.Attributes()

                        If Not htVariabiliStampe.ContainsKey(xmlAttributo.Name) Then

                            htVariabiliStampe.Add(xmlAttributo.Name, xmlAttributo.Value)

                        End If

                    Next

                End If

                If xmlVariabiliStampe.HasChildNodes Then
                    xmlVariabiliStampe2 = xmlVariabiliStampe.FirstChild

                    For Each xmlAttributo2 In xmlVariabiliStampe2.Attributes()

                        If Not htVariabiliStampe.ContainsKey(xmlAttributo2.Name) Then

                            htVariabiliStampe.Add(xmlAttributo2.Name, xmlAttributo2.Value)

                        End If

                    Next

                End If

            End If

        Catch ex As Exception
            strErr = ex.Message
            htVariabiliStampe = Nothing
        End Try

    End Sub


    '####################################################################################
    'Funzione da utilizzare per il nuovo modo di chiamata all'AgronicaStampe
    'Crea il primo elemento con gli attributi necessari
    'a questo elemento si aggiunge poi tutto il contenuto necessario per ogni report
    Public Function Crea_Nodo_XML_VariabiliStampe(ByRef xmlDoc As XmlDocument,
                                                  ByVal codiceReport As enum_CodificaStampe,
                                                  ByVal piva As String
                                                  ) As XmlElement

        Dim xmlVariabiliStampe As XmlElement

        If IsNothing(xmlDoc) Then
            xmlDoc = New XmlDocument
        End If

        xmlVariabiliStampe = xmlDoc.CreateElement("VariabiliStampe")
        xmlDoc.AppendChild(xmlVariabiliStampe)

        xmlVariabiliStampe.SetAttribute(CStr("Codice_Report").ToLower, codiceReport)
        xmlVariabiliStampe.SetAttribute(CStr("Report").ToLower, codiceReport)
        xmlVariabiliStampe.SetAttribute(CStr("Piva").ToLower, piva)

        Return xmlVariabiliStampe


    End Function

    '##########################################################################################
    'nuovo metodo: sostituisce XML_VariabiliStampe
    Public Sub CreaInserisci_SottoNodo_XML_VarStampa(ByRef xmlDoc As XmlDocument,
                                                     ByVal vetVariabili() As ElementoStampe) 'As String

        If IsNothing(xmlDoc) Then
            xmlDoc = New XmlDocument
        End If

        Dim xmlVariabiliStampe As XmlElement
        Dim xmlVarStampa As XmlElement
        Dim i As Integer

        'Try

        If vetVariabili.Length > 0 Then

            'Creo il nodo 
            xmlVarStampa = xmlDoc.CreateElement("VarStampa")

            For i = 0 To vetVariabili.GetUpperBound(0)

                'Imposto gli attributi come coppie nome-valore
                xmlVarStampa.SetAttribute(LCase(vetVariabili(i).Nome), vetVariabili(i).Valore)

            Next

            xmlVariabiliStampe = xmlDoc.SelectSingleNode("VariabiliStampe")
            xmlVariabiliStampe.AppendChild(xmlVarStampa)

        End If

        'Catch ex As Exception
        '    xmlDoc = New XmlDocument
        '    xmlErrore = XmlDoc.CreateElement("VariabiliStampe")
        '    xmlErrore.SetAttribute("Errore", ex.Message)
        '    xmlDoc.AppendChild(xmlErrore)
        'End Try

        ''Restituisco in uscita la stringa creata
        'Return xmlDoc.InnerXml

    End Sub

    'nuovo metodo: sostituisce XML_VariabiliStampe
    Public Sub CreaInserisci_SottoNodo_XML_VariabiliStampe(ByRef xmlDoc As XmlDocument,
                                                           ByVal vetVariabili() As ElementoStampe) 'As String

        If IsNothing(xmlDoc) Then
            xmlDoc = New XmlDocument
        End If

        Dim xmlVariabiliStampe As XmlElement
        Dim xmlVarStampa As XmlElement
        Dim i As Integer

        'Try

        If vetVariabili.Length > 0 Then

            'Creo il nodo 
            xmlVarStampa = xmlDoc.CreateElement("VariabiliStampe")

            For i = 0 To vetVariabili.GetUpperBound(0)

                'Imposto gli attributi come coppie nome-valore
                xmlVarStampa.SetAttribute(LCase(vetVariabili(i).Nome), vetVariabili(i).Valore)

            Next

            xmlVariabiliStampe = xmlDoc.SelectSingleNode("Pippo")
            xmlVariabiliStampe.AppendChild(xmlVarStampa)

        End If

        'Catch ex As Exception
        '    xmlDoc = New XmlDocument
        '    xmlErrore = XmlDoc.CreateElement("VariabiliStampe")
        '    xmlErrore.SetAttribute("Errore", ex.Message)
        '    xmlDoc.AppendChild(xmlErrore)
        'End Try

        ''Restituisco in uscita la stringa creata
        'Return xmlDoc.InnerXml

    End Sub



    '####################################################################################
    'test x chiamata stampe excel Agrisfera
    Public Function StrXmlParametri_Report_89() As String

        Dim str_XML As String = ""
        'Dim xmlDoc As New XmlDocument

        'xmlDoc.Load("c:\XML_Produzione_Standard2.xml")

        'str_XML = xmlDoc.OuterXml

        Return str_XML

    End Function

    '################################################################################
    'Questa funzione prepara l'xml da inviare alla stampe/e relativa/e
    'ovvero il nodo varabiliStampe e il relativo contenuto (che varia in base al report)
    'il nodo radice Parametri e il nodo VariabiliSessione vengono creati dopo
    Public Function StrXmlParametri_Schede_Magazzino(ByVal Codice_Report As enum_CodificaStampe,
                                                     ByVal Piva As String,
                                                     ByVal Sa_Cod As Integer,
                                                     ByVal Fabbricato_Cod As Integer,
                                                     ByVal Elem_Cod As Integer,
                                                     ByVal Pro_Cod As Integer,
                                                     ByVal Mat_Cod As Integer,
                                                     ByVal Data_Stampa As Date,
                                                     ByVal Data_Inizio As Date,
                                                     ByVal Data_Fine As Date
                                                     ) As String

        Dim xmlDoc As New XmlDocument
        Dim xmlVariabiliStampe As XmlElement

        xmlVariabiliStampe = Crea_Nodo_XML_VariabiliStampe(xmlDoc, Codice_Report, Piva)

        Dim vVarStampe(5) As ElementoStampe

        'If Data_Inizio <> AGRODATAINIZIO And Data_Fine <> AGRODATAFINE Then
        '    Dim vVarStampe(7) As ElementiStampe.ElementoStampe
        'Else
        '    Dim vVarStampe(5) As ElementiStampe.ElementoStampe
        'End If

        vVarStampe(0).Nome = "sa_cod"
        vVarStampe(0).Valore = Sa_Cod

        vVarStampe(1).Nome = "fabbricato_cod"
        vVarStampe(1).Valore = Fabbricato_Cod

        vVarStampe(2).Nome = "elem_cod"
        vVarStampe(2).Valore = Elem_Cod

        vVarStampe(3).Nome = "pro_cod"
        vVarStampe(3).Valore = Pro_Cod

        vVarStampe(4).Nome = "mat_cod"
        vVarStampe(4).Valore = Mat_Cod

        If Data_Stampa <> AGRODATAINIZIO Then
            vVarStampe(5).Nome = "data_stampa"
            vVarStampe(5).Valore = Data_Stampa
        Else
            vVarStampe(5).Nome = "data_stampa"
            vVarStampe(5).Valore = Date.Today
        End If

        'If Data_Inizio <> AGRODATAINIZIO Then
        '    vVarStampe(5).Nome = "data_inizio"
        '    vVarStampe(5).Valore = Data_Inizio
        'End If

        'If Data_Fine <> AGRODATAFINE Then
        '    vVarStampe(6).Nome = "data_fine"
        '    vVarStampe(6).Valore = Data_Fine
        'End If

        CreaInserisci_SottoNodo_XML_VarStampa(xmlDoc, vVarStampe)

        Return xmlVariabiliStampe.OuterXml

    End Function

    '##########################################################################################
    Public Function XML_VariabiliSessione(ByVal Progressivo_Gias As Integer,
                                          ByVal Id_Servizio As Integer,
                                          ByVal PathFileINI As String,
                                          ByVal Cn_Server As String,
                                          ByVal Cn_Utenti As String,
                                          ByVal Stringa_Cn_Server As String,
                                          ByVal Stringa_Cn_Utenti As String,
                                          ByVal Cn_LogAccessi As String,
                                          ByVal FinestraTemporale_Inizio As Date,
                                          ByVal FinestraTemporale_Fine As Date,
                                          ByVal Utente_Usr As String,
                                          ByVal Utente_Pwd As String,
                                          ByVal Utente_Usr_Crypt As String,
                                          ByVal Utente_Pwd_Crypt As String,
                                          ByVal Utente_CodFiscale As String,
                                          ByVal SuperUser_Usr As String,
                                          ByVal SuperUser_Pwd As String,
                                          ByVal SuperUser_Usr_Crypt As String,
                                          ByVal SuperUser_Pwd_Crypt As String,
                                          ByVal SuperUser_Piva As String,
                                          ByVal AgronicaCore_Flag_CancellazioneLogica As Integer,
                                          ByVal AgronicaCore_Flag_Visibilita As Integer,
                                          ByVal AgronicaCore_FileNameLOG As String,
                                          ByVal PathDirectoryLOG As String
                                          ) As String

        Dim xmlDoc As New XmlDocument
        Dim xmlTxt As XmlElement

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        xmlTxt = xmlDoc.CreateElement("VariabiliSessione")

        'Imposto gli attributi
        xmlTxt.SetAttribute(LCase("Progressivo_Gias"), CStr(Progressivo_Gias))
        xmlTxt.SetAttribute(LCase("Id_Servizio"), CStr(Id_Servizio))
        xmlTxt.SetAttribute(LCase("PathFileINI"), CStr(PathFileINI))
        xmlTxt.SetAttribute(LCase("Cn_Server"), CStr(Cn_Server))
        xmlTxt.SetAttribute(LCase("Cn_Utenti"), CStr(Cn_Utenti))
        xmlTxt.SetAttribute(LCase("Stringa_Cn_Server"), CStr(Stringa_Cn_Server))
        xmlTxt.SetAttribute(LCase("Stringa_Cn_Utenti"), CStr(Stringa_Cn_Utenti))
        xmlTxt.SetAttribute(LCase("Cn_LogAccessi"), CStr(Cn_LogAccessi))
        xmlTxt.SetAttribute(LCase("FinestraTemporale_Inizio"), CStr(FinestraTemporale_Inizio))
        xmlTxt.SetAttribute(LCase("FinestraTemporale_Fine"), CStr(FinestraTemporale_Fine))
        xmlTxt.SetAttribute(LCase("Utente_Usr"), CStr(Utente_Usr))
        xmlTxt.SetAttribute(LCase("Utente_Pwd"), CStr(Utente_Pwd))
        xmlTxt.SetAttribute(LCase("Utente_Usr_Crypt"), CStr(Utente_Usr_Crypt))
        xmlTxt.SetAttribute(LCase("Utente_Pwd_Crypt"), CStr(Utente_Pwd_Crypt))
        xmlTxt.SetAttribute(LCase("Utente_CodFiscale"), CStr(Utente_CodFiscale))
        xmlTxt.SetAttribute(LCase("SuperUser_Usr"), CStr(SuperUser_Usr))
        xmlTxt.SetAttribute(LCase("SuperUser_Pwd"), CStr(SuperUser_Pwd))
        xmlTxt.SetAttribute(LCase("SuperUser_Usr_Crypt"), CStr(SuperUser_Usr_Crypt))
        xmlTxt.SetAttribute(LCase("SuperUser_Pwd_Crypt"), CStr(SuperUser_Pwd_Crypt))
        xmlTxt.SetAttribute(LCase("SuperUser_Piva"), CStr(SuperUser_Piva))
        xmlTxt.SetAttribute(LCase("AgronicaCore_Flag_CancellazioneLogica"), CStr(AgronicaCore_Flag_CancellazioneLogica))
        xmlTxt.SetAttribute(LCase("AgronicaCore_Flag_Visibilita"), CStr(AgronicaCore_Flag_Visibilita))
        xmlTxt.SetAttribute(LCase("AgronicaCore_FileNameLOG"), CStr(AgronicaCore_FileNameLOG))
        xmlTxt.SetAttribute(LCase("AgronicaCore_DirectoryLOG"), CStr(PathDirectoryLOG))
        xmlTxt.SetAttribute(LCase("PathDirectoryLOG"), CStr(PathDirectoryLOG))

        'Imposto XmlTxt come figlio del documento principale
        xmlDoc.AppendChild(xmlTxt)

        'Restituisco in uscita la stringa creata
        Return xmlDoc.InnerXml

    End Function


    '################################################################################
    'Questa funzione prepara l'xml da inviare alla stampe/e relativa/e
    'ovvero il nodo varabiliStampe e il relativo contenuto (che varia in base al report)
    'il nodo radice Parametri e il nodo VariabiliSessione vengono creati dopo
    Public Function StrXmlParametri_GestioneEtichette_Trasformati(ByVal Codice_Report As enum_CodificaStampe,
                                                                  ByVal Piva As String,
                                                                  ByVal Mat_Cod As Integer
                                                                  ) As String

        Dim xmlDoc As New XmlDocument
        Dim xmlVariabiliStampe As XmlElement

        xmlVariabiliStampe = Crea_Nodo_XML_VariabiliStampe(xmlDoc, Codice_Report, Piva)

        Dim vVarStampe(0) As ElementoStampe

        vVarStampe(0).Nome = "mat_cod"
        vVarStampe(0).Valore = Mat_Cod

        CreaInserisci_SottoNodo_XML_VarStampa(xmlDoc, vVarStampe)

        Return xmlVariabiliStampe.OuterXml

    End Function

    '################################################################################
    'Questa funzione prepara l'xml da inviare alla stampe/e relativa/e
    'ovvero il nodo varabiliStampe e il relativo contenuto (che varia in base al report)
    'il nodo radice Parametri e il nodo VariabiliSessione vengono creati dopo
    Public Function StrXmlParametri_GestioneSchedeBio(ByVal Codice_Report As enum_CodificaStampe,
                                                      ByVal piva As String) As String

        Dim xmlDoc As New XmlDocument
        Dim xmlVariabiliStampe As XmlElement

        xmlVariabiliStampe = Crea_Nodo_XML_VariabiliStampe(xmlDoc, Codice_Report, piva)

        Return xmlVariabiliStampe.OuterXml

    End Function

    '################################################################################
    'Questa funzione prepara l'xml da inviare alla stampe/e relativa/e
    'ovvero il nodo varabiliStampe e il relativo contenuto (che varia in base al report)
    'il nodo radice Parametri e il nodo VariabiliSessione vengono creati dopo
    Public Function StrXmlParametri_GestioneReportIncongruenze(ByVal Codice_Report As enum_CodificaStampe,
                                                               ByVal piva As String) As String

        Dim xmlDoc As New XmlDocument
        Dim xmlVariabiliStampe As XmlElement

        xmlVariabiliStampe = Crea_Nodo_XML_VariabiliStampe(xmlDoc, Codice_Report, piva)

        Return xmlVariabiliStampe.OuterXml

    End Function

    Public Function StrXmlParametri_FiltroElaboratiContabili(ByVal codiceReport As enum_CodificaStampe,
                                                             ByVal piva As String,
                                                             ByVal ragSoc As String,
                                                             ByVal anno As Integer,
                                                             ByVal dataInizio As String,
                                                             ByVal dataFine As String
                                                             ) As String

        Dim xmlDoc As New XmlDocument
        Dim xmlVariabiliStampe As XmlElement

        xmlVariabiliStampe = Crea_Nodo_XML_VariabiliStampe(xmlDoc, codiceReport, piva)

        xmlVariabiliStampe.SetAttribute("piva", piva)
        xmlVariabiliStampe.SetAttribute("anno", anno)
        xmlVariabiliStampe.SetAttribute("data_inizio", dataInizio)
        xmlVariabiliStampe.SetAttribute("data_fine", dataFine)
        xmlVariabiliStampe.SetAttribute("rag_soc", ragSoc)

        Return xmlVariabiliStampe.OuterXml

    End Function

    Public Function GeneraXmlStampaDocumento(ByVal piva As String,
                                             ByVal idAgenda As Integer,
                                             ByVal lavCod As Integer,
                                             ByVal report As enum_CodificaStampe,
                                             ByVal idCodCliente As Integer,
                                             ByRef objParametriSuperServer As AgronicaCoreParametri,
                                             ByRef objParametriServer As AgronicaCoreParametri,
                                             ByRef objParametriUtenti As AgronicaCoreParametri,
                                             Optional ByVal sitoOrigine As Enum_SiteRedirector = Enum_SiteRedirector.GiasLan
                                             ) As String

        Dim nomeRoutine As String = "AgronicaCoreXML.XML_Stampe.GeneraXmlStampaDocumento()"
        Dim xmlDoc As New XmlDocument
        
        Try

            Dim objConnessioni As New Connessioni
            Dim idDbServer As Integer = objConnessioni.Recupera_IdDb(enum_Tipo_DB.GIAS_SERVER,
                                                                     "", objParametriServer.Recupera_NomeDB(),
                                                                     "", "", "", "", "", 0, "", "", "",
                                                                     objParametriSuperServer)

            Dim idDbUtenti As Integer = objConnessioni.Recupera_IdDb(enum_Tipo_DB.GIAS_UTENTI,
                                                                     "", objParametriUtenti.Recupera_NomeDB(),
                                                                     "", "", "", "", "", 0, "", "", "",
                                                                     objParametriSuperServer)

            'Dim objXmlStampe As New AgronicaCoreXML.XML_Stampe
            Dim xmlVariabiliSessione As String = XML_VariabiliSessione(Progressivo_Gias:=idCodCliente,
                                                                       Id_Servizio:=enum_Id_Servizio.GiasOnline,
                                                                       PathFileINI:="",
                                                                       Cn_Server:=CStr(idDbServer),
                                                                       Cn_Utenti:=CStr(idDbUtenti),
                                                                       Stringa_Cn_Server:="",
                                                                       Stringa_Cn_Utenti:="",
                                                                       Cn_LogAccessi:="",
                                                                       FinestraTemporale_Inizio:=objParametriServer.FinestraTemporaleInizio,
                                                                       FinestraTemporale_Fine:=objParametriServer.FinestraTemporaleFine,
                                                                       Utente_Usr:=objParametriServer.UtenteUsername,
                                                                       Utente_Pwd:="",
                                                                       Utente_Usr_Crypt:="",
                                                                       Utente_Pwd_Crypt:="",
                                                                       Utente_CodFiscale:=objParametriServer.UtenteCodFiscale,
                                                                       SuperUser_Usr:=objParametriServer.SuperUserUsername,
                                                                       SuperUser_Pwd:="",
                                                                       SuperUser_Usr_Crypt:="",
                                                                       SuperUser_Pwd_Crypt:="",
                                                                       SuperUser_Piva:=objParametriServer.PivaSuperUser,
                                                                       AgronicaCore_Flag_CancellazioneLogica:=CInt(objParametriServer.FlagCancellazioneLogica),
                                                                       AgronicaCore_Flag_Visibilita:=CInt(objParametriServer.FlagVisibilita),
                                                                       AgronicaCore_FileNameLOG:=objParametriServer.LogFileName,
                                                                       PathDirectoryLOG:=objParametriServer.LogDirectory)

            Dim xmlParametri As XmlElement = xmlDoc.CreateElement("Parametri")
            xmlParametri.SetAttribute("sito_origine", CInt(sitoOrigine))
            xmlParametri.SetAttribute("sito_destinazione", CInt(Enum_SiteRedirector.Sito_AgronicaStampe_2010))

            Dim xmlVariabiliStampe As XmlElement = xmlDoc.CreateElement("ParametriAgronicaStampe_2010")
            xmlVariabiliStampe.SetAttribute("username", objParametriUtenti.UtenteUsername)
            xmlVariabiliStampe.SetAttribute("report", CInt(report))
            xmlVariabiliStampe.SetAttribute("codice_report", CInt(report))
            xmlVariabiliStampe.SetAttribute("piva", piva)
            xmlVariabiliStampe.SetAttribute("user_profilo", objParametriUtenti.PivaSuperUser) '?? verifica
            'xmlVariabiliStampe.SetAttribute("UserProfilo_CodFisc", idCodCliente)
            'xmlVariabiliStampe.SetAttribute("utente_codfiscale", objParametriUtenti.UtenteCodFiscale)

            Dim vVarStampe() As ElementoStampe = {
                New ElementoStampe With {.Nome = "piva", .Valore = piva},
                New ElementoStampe With {.Nome = "id_agenda", .Valore = idAgenda},
                New ElementoStampe With {.Nome = "lav_cod", .Valore = lavCod},
                New ElementoStampe With {.Nome = "printcode", .Valore = lavCod},
                New ElementoStampe With {.Nome = "printtoprinter", .Valore = 0},
                New ElementoStampe With {.Nome = "printname", .Valore = ""},
                New ElementoStampe With {.Nome = "preview", .Valore = 0}
            }

            'New ElementoStampe With {.Nome = "printtoprinter", .Valore = PrintToPrinter},
            'New ElementoStampe With {.Nome = "printname", .Valore = UtilityProvider.XML_SaveText(PrintName)},

            'New ElementoStampe With {.Nome = "mode_preview", .Valore = Abs(bA5_PREVIEW)},
            'New ElementoStampe With {.Nome = "stampante", .Valore = UtilityProvider.XML_SaveText(A5_PRINTER)},
            'New ElementoStampe With {.Nome = "stampante_predefinita", .Valore = UtilityProvider.XML_SaveText(Printer.DeviceName)},

            Dim xmlVarStampa As String = XML_VariabiliStampe(vVarStampe)
            xmlVariabiliStampe.InnerXml &= xmlVarStampa

            xmlParametri.AppendChild(xmlVariabiliStampe)

            xmlDoc.AppendChild(xmlParametri)

            xmlParametri.InnerXml &= xmlVariabiliSessione

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return xmlDoc.InnerXml

    End Function


    Public Function GeneraXmlStampaSchedaCampagna(ByVal piva As String,
                                                  ByVal VariabiliStampeFiltroXMl As String,
                                                  ByVal report As enum_CodificaStampe,
                                                  ByVal idCodCliente As Integer,
                                                  ByRef objParametriSuperServer As AgronicaCoreParametri,
                                                  ByRef objParametriServer As AgronicaCoreParametri,
                                                  ByRef objParametriUtenti As AgronicaCoreParametri,
                                                  Optional ByVal sitoOrigine As Enum_SiteRedirector = Enum_SiteRedirector.GiasLan
                                                  ) As String

        Dim nomeRoutine As String = "AgronicaCoreXML.XML_Stampe.GeneraXmlStampaSchedaCampagna()"
        Dim xmlDoc As New XmlDocument

        Try

            Dim objConnessioni As New Connessioni
            Dim idDbServer As Integer = objConnessioni.Recupera_IdDb(enum_Tipo_DB.GIAS_SERVER,
                                                                     "", objParametriServer.Recupera_NomeDB(),
                                                                     "", "", "", "", "", 0, "", "", "",
                                                                     objParametriSuperServer)

            Dim idDbUtenti As Integer = objConnessioni.Recupera_IdDb(enum_Tipo_DB.GIAS_UTENTI,
                                                                     "", objParametriUtenti.Recupera_NomeDB(),
                                                                     "", "", "", "", "", 0, "", "", "",
                                                                     objParametriSuperServer)

            'Dim objXmlStampe As New AgronicaCoreXML.XML_Stampe
            Dim xmlVariabiliSessione As String = XML_VariabiliSessione(Progressivo_Gias:=idCodCliente,
                                                                       Id_Servizio:=enum_Id_Servizio.GiasOnline,
                                                                       PathFileINI:="",
                                                                       Cn_Server:=CStr(idDbServer),
                                                                       Cn_Utenti:=CStr(idDbUtenti),
                                                                       Stringa_Cn_Server:="",
                                                                       Stringa_Cn_Utenti:="",
                                                                       Cn_LogAccessi:="",
                                                                       FinestraTemporale_Inizio:=objParametriServer.FinestraTemporaleInizio,
                                                                       FinestraTemporale_Fine:=objParametriServer.FinestraTemporaleFine,
                                                                       Utente_Usr:=objParametriServer.UtenteUsername,
                                                                       Utente_Pwd:="",
                                                                       Utente_Usr_Crypt:="",
                                                                       Utente_Pwd_Crypt:="",
                                                                       Utente_CodFiscale:=objParametriServer.UtenteCodFiscale,
                                                                       SuperUser_Usr:=objParametriServer.SuperUserUsername,
                                                                       SuperUser_Pwd:="",
                                                                       SuperUser_Usr_Crypt:="",
                                                                       SuperUser_Pwd_Crypt:="",
                                                                       SuperUser_Piva:=objParametriServer.PivaSuperUser,
                                                                       AgronicaCore_Flag_CancellazioneLogica:=CInt(objParametriServer.FlagCancellazioneLogica),
                                                                       AgronicaCore_Flag_Visibilita:=CInt(objParametriServer.FlagVisibilita),
                                                                       AgronicaCore_FileNameLOG:=objParametriServer.LogFileName,
                                                                       PathDirectoryLOG:=objParametriServer.LogDirectory)

            Dim xmlParametri As XmlElement = xmlDoc.CreateElement("Parametri")
            xmlParametri.SetAttribute("sito_origine", CInt(sitoOrigine))
            xmlParametri.SetAttribute("sito_destinazione", CInt(Enum_SiteRedirector.Sito_AgronicaStampe_2010))

            Dim xmlVariabiliStampe As XmlElement = xmlDoc.CreateElement("ParametriAgronicaStampe_2010")
            xmlVariabiliStampe.SetAttribute("username", objParametriUtenti.UtenteUsername)
            xmlVariabiliStampe.SetAttribute("report", CInt(report))
            xmlVariabiliStampe.SetAttribute("codice_report", CInt(report))
            xmlVariabiliStampe.SetAttribute("piva", piva)
            xmlVariabiliStampe.SetAttribute("user_profilo", objParametriUtenti.PivaSuperUser) '?? verifica
            'xmlVariabiliStampe.SetAttribute("UserProfilo_CodFisc", idCodCliente)
            'xmlVariabiliStampe.SetAttribute("utente_codfiscale", objParametriUtenti.UtenteCodFiscale)


            xmlVariabiliStampe.SetAttribute("StampaDiretta", 1)







            'Dim vVarStampe() As ElementoStampe = {
            '    New ElementoStampe With {.Nome = "piva", .Valore = piva},
            '    New ElementoStampe With {.Nome = "id_agenda", .Valore = idAgenda},
            '    New ElementoStampe With {.Nome = "lav_cod", .Valore = lavCod},
            '    New ElementoStampe With {.Nome = "printcode", .Valore = lavCod},
            '    New ElementoStampe With {.Nome = "printtoprinter", .Valore = 0},
            '    New ElementoStampe With {.Nome = "printname", .Valore = ""},
            '    New ElementoStampe With {.Nome = "preview", .Valore = 0}
            '}

            'New ElementoStampe With {.Nome = "printtoprinter", .Valore = PrintToPrinter},
            'New ElementoStampe With {.Nome = "printname", .Valore = UtilityProvider.XML_SaveText(PrintName)},

            'New ElementoStampe With {.Nome = "mode_preview", .Valore = Abs(bA5_PREVIEW)},
            'New ElementoStampe With {.Nome = "stampante", .Valore = UtilityProvider.XML_SaveText(A5_PRINTER)},
            'New ElementoStampe With {.Nome = "stampante_predefinita", .Valore = UtilityProvider.XML_SaveText(Printer.DeviceName)},

            ' Dim xmlVarStampa As String = XML_VariabiliStampe(vVarStampe)

            xmlVariabiliStampe.InnerXml &= VariabiliStampeFiltroXMl

            xmlParametri.AppendChild(xmlVariabiliStampe)

            xmlDoc.AppendChild(xmlParametri)

            xmlParametri.InnerXml &= xmlVariabiliSessione

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return xmlDoc.InnerXml

    End Function





    Public Function GeneraXmlStampaChecklistGlobalGap(ByVal piva As String,
                                                  ByVal VariabiliStampeFiltroXMl As String,
                                                  ByVal report As enum_CodificaStampe,
                                                  ByVal idCodCliente As Integer,
                                                  ByRef objParametriSuperServer As AgronicaCoreParametri,
                                                  ByRef objParametriServer As AgronicaCoreParametri,
                                                  ByRef objParametriUtenti As AgronicaCoreParametri,
                                                  Optional ByVal sitoOrigine As Enum_SiteRedirector = Enum_SiteRedirector.GiasLan
                                                  ) As String

        Dim nomeRoutine As String = "AgronicaCoreXML.XML_Stampe.GeneraXmlStampaSchedaCampagna()"
        Dim xmlDoc As New XmlDocument

        Try

            Dim objConnessioni As New Connessioni
            Dim idDbServer As Integer = objConnessioni.Recupera_IdDb(enum_Tipo_DB.GIAS_SERVER,
                                                                     "", objParametriServer.Recupera_NomeDB(),
                                                                     "", "", "", "", "", 0, "", "", "",
                                                                     objParametriSuperServer)

            Dim idDbUtenti As Integer = objConnessioni.Recupera_IdDb(enum_Tipo_DB.GIAS_UTENTI,
                                                                     "", objParametriUtenti.Recupera_NomeDB(),
                                                                     "", "", "", "", "", 0, "", "", "",
                                                                     objParametriSuperServer)

            'Dim objXmlStampe As New AgronicaCoreXML.XML_Stampe
            Dim xmlVariabiliSessione As String = XML_VariabiliSessione(Progressivo_Gias:=idCodCliente,
                                                                       Id_Servizio:=enum_Id_Servizio.GiasOnline,
                                                                       PathFileINI:="",
                                                                       Cn_Server:=CStr(idDbServer),
                                                                       Cn_Utenti:=CStr(idDbUtenti),
                                                                       Stringa_Cn_Server:="",
                                                                       Stringa_Cn_Utenti:="",
                                                                       Cn_LogAccessi:="",
                                                                       FinestraTemporale_Inizio:=objParametriServer.FinestraTemporaleInizio,
                                                                       FinestraTemporale_Fine:=objParametriServer.FinestraTemporaleFine,
                                                                       Utente_Usr:=objParametriServer.UtenteUsername,
                                                                       Utente_Pwd:="",
                                                                       Utente_Usr_Crypt:="",
                                                                       Utente_Pwd_Crypt:="",
                                                                       Utente_CodFiscale:=objParametriServer.UtenteCodFiscale,
                                                                       SuperUser_Usr:=objParametriServer.SuperUserUsername,
                                                                       SuperUser_Pwd:="",
                                                                       SuperUser_Usr_Crypt:="",
                                                                       SuperUser_Pwd_Crypt:="",
                                                                       SuperUser_Piva:=objParametriServer.PivaSuperUser,
                                                                       AgronicaCore_Flag_CancellazioneLogica:=CInt(objParametriServer.FlagCancellazioneLogica),
                                                                       AgronicaCore_Flag_Visibilita:=CInt(objParametriServer.FlagVisibilita),
                                                                       AgronicaCore_FileNameLOG:=objParametriServer.LogFileName,
                                                                       PathDirectoryLOG:=objParametriServer.LogDirectory)

            Dim xmlParametri As XmlElement = xmlDoc.CreateElement("Parametri")
            xmlParametri.SetAttribute("sito_origine", CInt(sitoOrigine))
            xmlParametri.SetAttribute("sito_destinazione", CInt(Enum_SiteRedirector.Sito_AgronicaStampe_2010))

            Dim xmlVariabiliStampe As XmlElement = xmlDoc.CreateElement("ParametriAgronicaStampe_2010")
            xmlVariabiliStampe.SetAttribute("username", objParametriUtenti.UtenteUsername)
            xmlVariabiliStampe.SetAttribute("report", CInt(report))
            xmlVariabiliStampe.SetAttribute("codice_report", CInt(report))
            xmlVariabiliStampe.SetAttribute("piva", piva)
            xmlVariabiliStampe.SetAttribute("user_profilo", objParametriUtenti.PivaSuperUser) '?? verifica
            'xmlVariabiliStampe.SetAttribute("UserProfilo_CodFisc", idCodCliente)
            'xmlVariabiliStampe.SetAttribute("utente_codfiscale", objParametriUtenti.UtenteCodFiscale)


            xmlVariabiliStampe.SetAttribute("StampaDiretta", 1)







            'Dim vVarStampe() As ElementoStampe = {
            '    New ElementoStampe With {.Nome = "piva", .Valore = piva},
            '    New ElementoStampe With {.Nome = "id_agenda", .Valore = idAgenda},
            '    New ElementoStampe With {.Nome = "lav_cod", .Valore = lavCod},
            '    New ElementoStampe With {.Nome = "printcode", .Valore = lavCod},
            '    New ElementoStampe With {.Nome = "printtoprinter", .Valore = 0},
            '    New ElementoStampe With {.Nome = "printname", .Valore = ""},
            '    New ElementoStampe With {.Nome = "preview", .Valore = 0}
            '}

            'New ElementoStampe With {.Nome = "printtoprinter", .Valore = PrintToPrinter},
            'New ElementoStampe With {.Nome = "printname", .Valore = UtilityProvider.XML_SaveText(PrintName)},

            'New ElementoStampe With {.Nome = "mode_preview", .Valore = Abs(bA5_PREVIEW)},
            'New ElementoStampe With {.Nome = "stampante", .Valore = UtilityProvider.XML_SaveText(A5_PRINTER)},
            'New ElementoStampe With {.Nome = "stampante_predefinita", .Valore = UtilityProvider.XML_SaveText(Printer.DeviceName)},

            ' Dim xmlVarStampa As String = XML_VariabiliStampe(vVarStampe)

            xmlVariabiliStampe.InnerXml &= VariabiliStampeFiltroXMl

            xmlParametri.AppendChild(xmlVariabiliStampe)

            xmlDoc.AppendChild(xmlParametri)

            xmlParametri.InnerXml &= xmlVariabiliSessione

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return xmlDoc.InnerXml

    End Function

End Class
