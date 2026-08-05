Imports System.Xml
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports System.Text

Partial Public Class Funzioni



#Region "Grafica"

#Region "Rasters"

    Public Sub Elabora_XML_CentriXSfondi_Salva( _
                            ByVal objOpzioni As clsOpzioni, _
                            ByRef Log_Import As StringBuilder, _
                            ByRef Log_Errori As StringBuilder, _
                            ByRef Log_Riepilogo As StringBuilder, _
                            ByVal Piva_Origine As String, _
                            ByVal Sa_cod_Origine As Integer, _
                            ByVal Piva_Destinazione As String, _
                            ByVal Sa_cod_Destinazione As Integer _
                    )

        Const nomeFunzione As String = "Elabora_XML_CentriXSfondi_Salva"

        Try

            Dim centroRead As New AgronicaCoreAnagrafeBIZ.CentroAziendale_R
            Dim xmlCentro As String = centroRead.CentroAziendale_Leggi( _
                Piva_Origine, _
                Sa_cod_Origine, _
                False, _
                True, _
                AGRODATAINIZIO, _
                AGRODATAFINE, _
                objOpzioni.objParametri_Server_GIAS_ORIGINE, _
                True
            )

            xmlCentro = Elabora_XML_CentriXSfondi_Sistemaxml( _
                objOpzioni, _
                Log_Import, _
                Log_Errori, _
                Log_Riepilogo, _
                xmlCentro, _
                Piva_Origine, _
                Sa_cod_Origine, _
                Piva_Destinazione, _
                Sa_cod_Destinazione _
            )


            Dim outputPiva As String = ""
            Dim outputSa_cod As Integer

            Dim centroScrivi As New AgronicaCoreAnagrafeBIZ.CentroAziendale_W
            centroScrivi.CentroAziendale_Scrivi( _
                xmlCentro, _
                outputPiva, _
                outputSa_cod, _
                objOpzioni.objParametri_Server_GIAS_DESTINAZIONE, _
                objOpzioni.objParametri_Utenti_GIAS_DESTINAZIONE _
            )

        Catch ex As Exception

            Dim msg As String
            msg = CStr(Date.Now) + " - " + nomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)

        End Try

    End Sub


    Public Function Elabora_XML_CentriXSfondi_Sistemaxml( _
                    ByVal objOpzioni As clsOpzioni, _
                    ByRef Log_Import As StringBuilder, _
                    ByRef Log_Errori As StringBuilder, _
                    ByRef Log_Riepilogo As StringBuilder, _
                    ByRef Str_XML_CentriXSfondi As String, _
                    ByVal Piva_Origine As String, _
                    ByVal saCod_Origine As Integer, _
                    ByVal Piva_Destinazione As String, _
                    ByVal saCod_Destinazione As Integer) As String

        Const NomeFunzione As String = "ElaboraXML_Grafica_SistemaXML"


        Dim XmlDocCont As New XmlDocument
        XmlDocCont.LoadXml(Str_XML_CentriXSfondi)

        Dim Str_XML_Output As String = ""

        Dim XMLs_Nodi As XmlNodeList
        Dim XML_Nodo As XmlElement
        Dim i As Integer

        Try

            'tipo operazione = scrittura
            XMLs_Nodi = XmlDocCont.SelectNodes("//DatiCentriAziendali/CentroAziendale/ListaSfondi/CentriXSfondi")
            For i = 0 To XMLs_Nodi.Count - 1
                XML_Nodo = XMLs_Nodi(i)
                XML_Nodo.SetAttribute("TipoOperazioneDB", enum_TipoOperazioneDB.Scrittura)
                XML_Nodo.SetAttribute("piva", Piva_Destinazione)
                XML_Nodo.SetAttribute("sa_cod", saCod_Destinazione)
                XML_Nodo.SetAttribute("basecode", objOpzioni.BaseCode_DESTINAZIONE)
                XML_Nodo.SetAttribute("topcode", objOpzioni.TopCode_DESTINAZIONE)

            Next

            Str_XML_Output = XmlDocCont.OuterXml

        Catch ex As Exception
            Dim msg As String
            msg = CStr(Date.Now) + " - " + NomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try

        Return Str_XML_Output

    End Function

#End Region

#Region "Vettoriali"

    Public Function GetDatiXML_Grafica(ByVal TipoOperazione_DB As Integer, ByVal objOpzioni As clsOpzioni, ByVal Piva_Origine As String, ByVal Sa_cod_Origine As Integer, ByVal DatiLayers As String) As String

        Dim lElencoLay As String = ""
        If DatiLayers <> "" Then
            Dim splitted As String() = DatiLayers.Split(",")
            Dim dl As String = ""
            For Each s In splitted
                dl &= s.Split("|")(0) & ","
            Next

            lElencoLay = "'" & dl.TrimEnd(",").Replace(",", "','") & "'"
        End If




        Dim sXML_Grafica As String
        Dim AppezzaR As New AgronicaCoreGraficaBIZ.Grafica_Read
        sXML_Grafica = AppezzaR.Grafica_Leggi( _
        Piva_Origine, _
        Sa_cod_Origine, _
        "", _
        "", _
        False, _
        objOpzioni.objParametri_Server_GIAS_ORIGINE, _
        TipoOperazioneDB:=TipoOperazione_DB, _
        XFiltroAggiuntivo:=lElencoLay
    )
        Return sXML_Grafica
    End Function

    Public Sub Elabora_XML_Grafica_Salva( _
                        ByVal objOpzioni As clsOpzioni, _
                        ByRef Log_Import As StringBuilder, _
                        ByRef Log_Errori As StringBuilder, _
                        ByRef Log_Riepilogo As StringBuilder, _
                        ByVal Piva_Origine As String, _
                        ByVal Sa_cod_Origine As Integer, _
                        ByVal Piva_Destinazione As String, _
                        ByVal Sa_cod_Destinazione As Integer, _
                        ByVal TipoOperazione_DB As Integer
                )

        Const nomeFunzione As String = "Elabora_XML_Grafica_Salva"




        Try

            Dim sXML_Grafica As String
            sXML_Grafica = GetDatiXML_Grafica(TipoOperazione_DB, objOpzioni, Piva_Origine, Sa_cod_Origine, "I")

            If sXML_Grafica <> "" Then
                sXML_Grafica = ElaboraXML_Grafica_SistemaXML( _
                    objOpzioni, _
                    Log_Import, _
                    Log_Errori, _
                    Log_Riepilogo, _
                    sXML_Grafica, _
                    Piva_Origine, _
                    Sa_cod_Origine, _
                    Piva_Destinazione, _
                    Sa_cod_Destinazione _
                )


                Dim ObjGrafica_W As New AgronicaCoreGraficaBIZ.Grafica_Write

                'Dim objxmlGrafica As New XmlDocument
                'objxmlGrafica.LoadXml(sXML_Grafica)
                'objxmlGrafica.Save(".\XML_Maribo\" & Piva_Destinazione & "_" & Sa_cod_Destinazione & ".xml")

                'Salvo l'entità e ricavo il nuovo codice
                ObjGrafica_W.Grafica_Scrivi( _
                                sXML_Grafica, _
                                Piva_Destinazione, _
                                Sa_cod_Destinazione, _
                                1, _
                                objOpzioni.objParametri_Server_GIAS_DESTINAZIONE, _
                                False
                        )

            End If


        Catch ex As Exception
            Dim msg As String
            msg = CStr(Date.Now) + " - " + nomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)

        End Try

    End Sub


    Private Function ElaboraXML_Grafica_SistemaXML(ByVal objOpzioni As clsOpzioni, _
                                                    ByRef Log_Import As StringBuilder, _
                                                    ByRef Log_Errori As StringBuilder, _
                                                    ByRef Log_Riepilogo As StringBuilder, _
                                                    ByRef Str_XML_Appezzamento As String, _
                                                    ByVal Piva_Origine As String, _
                                                    ByVal saCod_Origine As Integer, _
                                                    ByVal Piva_Destinazione As String, _
                                                    ByVal saCod_Destinazione As Integer) As String

        Const NomeFunzione As String = "ElaboraXML_Grafica_SistemaXML"


        Dim XmlDocCont As New XmlDocument
        XmlDocCont.LoadXml(Str_XML_Appezzamento)

        Dim Str_XML_Output As String = ""

        Dim XMLs_Nodi As XmlNodeList
        Dim XML_Nodo As XmlElement
        Dim i As Integer
        Dim NuovoCodice As String = ""

        Try

            'tipo operazione = scrittura
            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@TipoOperazioneDB]")
            For i = 0 To XMLs_Nodi.Count - 1
                XML_Nodo = XMLs_Nodi(i)
                XML_Nodo.SetAttribute("TipoOperazioneDB", enum_TipoOperazioneDB.Scrittura)
                XML_Nodo.SetAttribute("piva", Piva_Destinazione)
                XML_Nodo.SetAttribute("sa_cod", saCod_Destinazione)
                XML_Nodo.SetAttribute("codice", 1)
                XML_Nodo.SetAttribute("basecode", objOpzioni.BaseCode_DESTINAZIONE)
                XML_Nodo.SetAttribute("topcode", objOpzioni.TopCode_DESTINAZIONE)

                NuovoCodice = resetCodice(Piva_Origine, saCod_Destinazione, saCod_Origine, XML_Nodo.GetAttribute("id"))

                'A volte esiste il poligono ma non l'entita' associata (sporcizia nel database)
                If NuovoCodice <> "" Then
                    XML_Nodo.SetAttribute("id", NuovoCodice)
                Else
                    XML_Nodo.SetAttribute("TipoOperazioneDB", enum_TipoOperazioneDB.Lettura)
                    XML_Nodo.SetAttribute("id", "")
                End If

            Next

            Str_XML_Output = XmlDocCont.OuterXml

        Catch ex As Exception
            Dim msg As String
            msg = CStr(Date.Now) + " - " + NomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try

        Return Str_XML_Output


    End Function

    Private Function resetCodice(ByVal oldpiva As String, ByVal newSa_Cod As Integer, ByVal oldSa_cod As Integer, ByVal oldCod As String) As String
        Dim rval As String = oldCod.Chars(0)

        Select Case oldCod.Chars(0)

            Case "A"
                Dim appezzaOld As Long = Long.Parse(oldCod.Substring(1, 8), Globalization.NumberStyles.HexNumber)

                Dim appezzaNew As String = _
                    (From m In _AppezzamentiMappati _
                     Where m.From_Piva = oldpiva _
                     And m.From_sa_cod = oldSa_cod _
                     And m.From_appezza = appezzaOld _
                     Select m.To_appezza).FirstOrDefault


                '----------
                'A volte esiste il poligono ma non l'appezzamento associato (sporcizia nel database)

                'rval &= Hex(Long.Parse(appezzaNew))

                If Not IsNothing(appezzaNew) Then
                    rval &= Hex(Long.Parse(appezzaNew))
                Else
                    rval = ""
                End If

                '----------


            Case "I", "F"
                Dim appezzaOld As Long = Long.Parse(oldCod.Substring(1, 4), Globalization.NumberStyles.HexNumber)
                Dim regImpiantoOld As Long = Long.Parse(oldCod.Substring(5, 4), Globalization.NumberStyles.HexNumber)

                Dim basecodOld As Integer
                Dim baseCodNew As Integer

                basecodOld = GetBasecod(oldSa_cod)
                baseCodNew = GetBasecod(newSa_Cod)

                appezzaOld = appezzaOld + basecodOld
                regImpiantoOld = regImpiantoOld + basecodOld

                Dim impiantonew = _
                    (From m In _ImpiantiMappati _
                        Where m.From_Piva = oldpiva _
                        And m.From_sa_cod = oldSa_cod _
                        And m.From_appezza = appezzaOld _
                        And m.From_Id_reg = regImpiantoOld _
                        Select m.To_appezza, m.To_Id_reg).FirstOrDefault

                '----------
                'A volte esiste il poligono ma non l'impianto associato (sporcizia nel database)

                'Dim sAppezza As String = Hex(Long.Parse(impiantonew.To_appezza - baseCodNew))
                'Dim sImpianto As String = Hex(Long.Parse(impiantonew.To_Id_reg - baseCodNew))
                'rval &= sAppezza.PadLeft(4, "0") & sImpianto.PadLeft(4, "0")

                If Not IsNothing(impiantonew) Then
                    Dim sAppezza As String = Hex(Long.Parse(impiantonew.To_appezza - baseCodNew))
                    Dim sImpianto As String = Hex(Long.Parse(impiantonew.To_Id_reg - baseCodNew))
                    rval &= sAppezza.PadLeft(4, "0") & sImpianto.PadLeft(4, "0")
                Else
                    rval = ""
                End If

                '----------

            Case Else
                rval = oldCod
        End Select

        Return rval

    End Function

    Private Shared Function GetBasecod(ByVal Sa_cod As Integer) As Integer

        Return (Sa_cod \ (2 ^ 17)) * (2 ^ 17)

    End Function
#End Region
#End Region

End Class
