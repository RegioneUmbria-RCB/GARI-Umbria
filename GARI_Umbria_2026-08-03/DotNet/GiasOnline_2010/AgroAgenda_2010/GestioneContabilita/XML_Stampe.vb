Imports System.Xml
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreXML.XML_Stampe

Public Module XML_Stampe


    '####################################################################################
    'Funzione da utilizzare per il nuovo modo di chiamata all'AgronicaStampe
    'Crea il primo elemento con gli attributi necessari
    'a questo elemento si aggiunge poi tutto il contenuto necessario per ogni report
    Public Function Crea_Nodo_XML_VariabiliStampe(ByRef XmlDoc As XmlDocument, _
                                                    ByVal Codice_Report As enum_CodificaStampe, _
                                                    ByVal Piva As String) As XmlElement

        Dim XML_VariabiliStampe As XmlElement

        If IsNothing(XmlDoc) Then
            XmlDoc = New XmlDocument
        End If

        XML_VariabiliStampe = XmlDoc.CreateElement("VariabiliStampe")
        XmlDoc.AppendChild(XML_VariabiliStampe)

        XML_VariabiliStampe.SetAttribute(CStr("Codice_Report").ToLower, Codice_Report)
        XML_VariabiliStampe.SetAttribute(CStr("Piva").ToLower, Piva)

        Return XML_VariabiliStampe


    End Function

    '##########################################################################################
    'nuovo metodo: sostituisce XML_VariabiliStampe
    Public Sub CreaInserisci_SottoNodo_XML_VarStampa(ByRef XmlDoc As XmlDocument, _
                                                    ByVal VetVariabili() As ElementoStampe) 'As String

        If IsNothing(XmlDoc) Then
            XmlDoc = New XmlDocument
        End If

        Dim XML_VariabiliStampe As XmlElement
        Dim XML_VarStampa As XmlElement
        Dim i As Integer

        'Try

        If VetVariabili.Length > 0 Then

            'Creo il nodo 
            XML_VarStampa = XmlDoc.CreateElement("VarStampa")

            For i = 0 To VetVariabili.GetUpperBound(0)

                'Imposto gli attributi come coppie nome-valore
                XML_VarStampa.SetAttribute(LCase(VetVariabili(i).Nome), VetVariabili(i).Valore)

            Next

            XML_VariabiliStampe = XmlDoc.SelectSingleNode("VariabiliStampe")
            XML_VariabiliStampe.AppendChild(XML_VarStampa)

        End If

        'Catch ex As Exception
        '    XmlDoc = New System.Xml.XmlDocument
        '    xmlErrore = XmlDoc.CreateElement("VariabiliStampe")
        '    xmlErrore.SetAttribute("Errore", ex.Message)
        '    XmlDoc.AppendChild(xmlErrore)
        'End Try

        ''Restituisco in uscita la stringa creata
        'Return XmlDoc.InnerXml


    End Sub



    '####################################################################################
    'test x chiamata stampe excel agrisfera
    Public Function StrXmlParametri_Report_89() As String

        Dim str_XML As String = ""
        'Dim xmldoc As New XmlDocument

        'xmldoc.Load("c:\XML_Produzione_Standard2.xml")

        'str_XML = xmldoc.OuterXml

        Return str_XML

    End Function

    '################################################################################
    'Questa funzione prepara l'xml da inviare alla stampe/e relativa/e
    'ovvero il nodo varabiliStampe e il relativo contenuto (che varia in base al report)
    'il nodo radice Parametri e il nodo VariabiliSessione vengono creati dopo
    Public Function StrXmlParametri_Schede_Magazzino(ByVal Codice_Report As enum_CodificaStampe, _
                                                    ByVal Piva As String, _
                                                    ByVal Sa_Cod As Integer, _
                                                    ByVal Fabbricato_Cod As Integer, _
                                                    ByVal Elem_Cod As Integer, _
                                                    ByVal Pro_Cod As Integer, _
                                                    ByVal Mat_Cod As Integer, _
                                                    ByVal Data_Stampa As Date, _
                                                    ByVal Data_Inizio As Date, _
                                                    ByVal Data_Fine As Date) As String

        Dim XmlDoc As New XmlDocument
        Dim XML_VariabiliStampe As XmlElement

        XML_VariabiliStampe = Crea_Nodo_XML_VariabiliStampe(XmlDoc, Codice_Report, Piva)

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



        CreaInserisci_SottoNodo_XML_VarStampa(XmlDoc, vVarStampe)


        Return XML_VariabiliStampe.OuterXml

    End Function




    '##########################################################################################
    'vecchio metodo
    Public Function XML_VariabiliStampe(ByVal VetVariabili() As ElementoStampe) As String


        Dim XmlDoc As System.Xml.XmlDocument
        Dim XmlTxt As System.Xml.XmlElement
        Dim xmlErrore As System.Xml.XmlElement
        Dim i As Integer

        Try

            XmlDoc = New System.Xml.XmlDocument

            If VetVariabili.Length > 0 Then

                'Creo il nodo 
                XmlTxt = XmlDoc.CreateElement("VariabiliStampe")

                For i = 0 To VetVariabili.GetUpperBound(0)

                    'Imposto gli attributi

                    XmlTxt.SetAttribute(LCase(VetVariabili(i).Nome), VetVariabili(i).Valore)

                Next

                'Imposto XmlTxt come figlio del documento principale
                XmlDoc.AppendChild(XmlTxt)

            End If

        Catch ex As Exception
            XmlDoc = New System.Xml.XmlDocument
            xmlErrore = XmlDoc.CreateElement("VariabiliStampe")
            xmlErrore.SetAttribute("Errore", ex.Message)
            XmlDoc.AppendChild(xmlErrore)
        End Try

        'Restituisco in uscita la stringa creata
        Return XmlDoc.InnerXml

        'Distruggo gli oggetti
        XmlTxt = Nothing
        XmlDoc = Nothing

    End Function

    '###############################################################################################
    'vecchio metodo
    Public Sub XML_EstraiVariabiliStampe(ByVal strXml As String, ByRef htVariabiliStampe As System.Collections.Hashtable, ByRef strErr As String)

        Dim XmlDoc As New System.Xml.XmlDocument

        Dim XML_VariabiliStampe As System.Xml.XmlElement
        Dim xmlAttributo As System.Xml.XmlAttribute
        Dim i As Integer
        Dim risp As String

        Try

            'Carico la stringa xml in un nuovo documento
            XmlDoc = New System.Xml.XmlDocument
            XmlDoc.LoadXml(strXml)

            If XmlDoc.HasChildNodes Then

                i = 0
                htVariabiliStampe = New System.Collections.Hashtable

                'Recupero l'insieme dei nodi Movimento
                XML_VariabiliStampe = XmlDoc.FirstChild
                If XML_VariabiliStampe.HasAttributes Then
                    For Each xmlAttributo In XML_VariabiliStampe.Attributes()

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

    '################################################################################
    'Questa funzione prepara l'xml da inviare alla stampe/e relativa/e
    'ovvero il nodo varabiliStampe e il relativo contenuto (che varia in base al report)
    'il nodo radice Parametri e il nodo VariabiliSessione vengono creati dopo
    Public Function StrXmlParametri_GestioneEtichette_Trasformati(ByVal Codice_Report As enum_CodificaStampe, _
                                                                ByVal Piva As String, _
                                                                ByVal Mat_Cod As Integer) As String

        Dim XmlDoc As New XmlDocument
        Dim XML_VariabiliStampe As XmlElement

        XML_VariabiliStampe = Crea_Nodo_XML_VariabiliStampe(XmlDoc, Codice_Report, Piva)

        Dim vVarStampe(0) As ElementoStampe

        vVarStampe(0).Nome = "mat_cod"
        vVarStampe(0).Valore = Mat_Cod

        CreaInserisci_SottoNodo_XML_VarStampa(XmlDoc, vVarStampe)

        Return XML_VariabiliStampe.OuterXml

    End Function

    '################################################################################
    'Questa funzione prepara l'xml da inviare alla stampe/e relativa/e
    'ovvero il nodo varabiliStampe e il relativo contenuto (che varia in base al report)
    'il nodo radice Parametri e il nodo VariabiliSessione vengono creati dopo
    Public Function StrXmlParametri_GestioneSchedeBio(ByVal Codice_Report As enum_CodificaStampe, _
                                                    ByVal Piva As String) As String

        Dim XmlDoc As New XmlDocument
        Dim XML_VariabiliStampe As XmlElement

        XML_VariabiliStampe = Crea_Nodo_XML_VariabiliStampe(XmlDoc, Codice_Report, Piva)

        Return XML_VariabiliStampe.OuterXml

    End Function





End Module
