Imports CrystalDecisions.CrystalReports.Engine
Imports System.Xml
Imports System.Web
Imports AgronicaCoreGestioneRichieste

Public Class PrintUtility


    'Invia il file da stampare al servizio di utility di Net-Agree con un comando di stampa associato
    Public Function InviaStampaToServizioUtility(ByRef Request As HttpRequest, _
                                                ByVal rpt As ReportDocument, _
                                                ByVal objAgroWebconfig As AgronicaCoreGestioneRichieste.AgroWebConfig, _
                                                ByRef MsgErrore As String, _
                                                ByVal NomeStampante As String, _
                                                ByVal NomeFileDati As String, _
                                                Optional ByVal NumeroCopie As Integer = 1, _
                                                Optional ByVal Str_Flag_Fascicola As String = "true", _
                                                Optional ByVal Start_Page As Integer = 0, _
                                                Optional ByVal End_Page As Integer = 0) As Boolean


        Dim ip As String = Request.UserHostAddress()

        Dim NomeFileComandi As String
        'Dim NomeFileDati As String
        Dim IstanteCorrente As Date
        Dim Ok As Boolean

        Dim PathComandiServizioWinStampa, PathDatiServizioWinStampa As String


        Ok = True

        '-----------------------------------------------------------
        'Leggo le impostazioni relative ai percorsi del servizio utility

        Try

            'percorso dati
            If Not IsNothing(objAgroWebconfig.PathDati_AgroWinSrvc_PrintUtility) Then
                PathDatiServizioWinStampa = objAgroWebconfig.PathDati_AgroWinSrvc_PrintUtility
            Else
                PathDatiServizioWinStampa = ""
            End If

            If Not PathDatiServizioWinStampa.EndsWith("\") Then PathDatiServizioWinStampa &= "\"


            'percorso comandi
            If Not IsNothing(objAgroWebconfig.PathComandi_AgroWinSrvc_PrintUtility) Then
                PathComandiServizioWinStampa = objAgroWebconfig.PathComandi_AgroWinSrvc_PrintUtility
            Else
                PathComandiServizioWinStampa = ""
            End If

            If Not PathComandiServizioWinStampa.EndsWith("\") Then PathComandiServizioWinStampa &= "\"

        Catch ex As Exception
            Ok = False
            MsgErrore = "Impossibile completare l'operazione. Errore in fase di lettura delle impostazioni relative ai percorsi del servizio di stampa. " & ex.Message
        End Try
        '-----------------------------------------------------------


        If Ok = True Then

            IstanteCorrente = Now

            '-----------------------------------------------------------
            'Salvo il report nella directory dei dati del servizio di utility

            Try

                If NomeFileDati = "" Then
                    NomeFileDati = Year(IstanteCorrente) & Right("0" & Month(IstanteCorrente), 2) & Right("0" & Day(IstanteCorrente), 2) & _
                                    Right("0" & Hour(IstanteCorrente), 2) & Right("0" & Minute(IstanteCorrente), 2) & Right("0" & Second(IstanteCorrente), 2) & Right("00" & IstanteCorrente.Millisecond, 3)

                    NomeFileDati &= "_"
                    NomeFileDati &= HttpContext.Current.Session.SessionID
                    NomeFileDati &= ".rpt"
                End If

                'rpt.SaveAs(PathDatiServizioWinStampa & NomeFileDati, True)
                rpt.SaveAs(PathDatiServizioWinStampa & NomeFileDati, CrystalDecisions.[Shared].ReportFileFormat.VSNetFileFormat)

                rpt.Close()

                rpt.Dispose()

                rpt = Nothing

                FileSystem.ChDir("C:\")

                GC.Collect()


            Catch ex As Exception
                Ok = False
                MsgErrore = "Impossibile completare l'operazione. Errore in fase di salvataggio del report nella directory dei dati del servizio di stampa. " & ex.Message
            End Try
            '-----------------------------------------------------------



            If Ok = True Then

                '-----------------------------
                'Creo e salvo il file dei comandi
                Try

                    'Creo il file dei comandi
                    Dim XmlDocComandi As XmlDocument
                    XmlDocComandi = Crea_File_Xml_Comandi(NomeFileDati, NomeStampante, NumeroCopie, Str_Flag_Fascicola, Start_Page, End_Page)

                    'salvo il file su disco nella directory dei comandi del servizio
                    NomeFileComandi = Year(IstanteCorrente) & Right("0" & Month(IstanteCorrente), 2) & Right("0" & Day(IstanteCorrente), 2) & _
                                    Right("0" & Hour(IstanteCorrente), 2) & Right("0" & Minute(IstanteCorrente), 2) & Right("0" & Second(IstanteCorrente), 2) & Right("00" & IstanteCorrente.Millisecond, 3)

                    NomeFileComandi &= "_"
                    NomeFileComandi &= HttpContext.Current.Session.SessionID
                    NomeFileComandi &= ".xml"

                    XmlDocComandi.Save(PathComandiServizioWinStampa & NomeFileComandi)


                Catch ex As Exception
                    Ok = False
                    MsgErrore = "Impossibile completare l'operazione. Errore in fase di creazione o salvataggio del file dei comandi di stampa. " & ex.Message
                End Try
                '-----------------------------------------------------------


            End If 'ok

        End If 'ok


        Return Ok



    End Function




    '####################################################################
    Public Function Crea_File_Xml_Comandi(ByVal NomeFileDati As String, _
                                            ByVal NomeStampante As String, _
                                            ByVal NumeroCopie As Integer, _
                                            ByVal Str_Flag_Fascicola As String, _
                                            ByVal Start_Page As Integer, _
                                            ByVal End_Page As Integer) As XmlDocument


        Dim XmlDocComandi As XmlDocument
        Dim NodoRoot, Nodo, NodoDocumento As XmlElement

        XmlDocComandi = New XmlDocument

        'radice
        NodoRoot = XmlDocComandi.CreateElement("dataroot")

        XmlDocComandi.AppendChild(NodoRoot)


        'comando
        Nodo = XmlDocComandi.CreateElement("comando")

        Nodo.InnerText = "PRINT"

        NodoRoot.AppendChild(Nodo)


        'documento da stampare
        NodoDocumento = XmlDocComandi.CreateElement("documento")

        NodoRoot.AppendChild(NodoDocumento)


        'parametri documento da stampare
        '. formato
        Nodo = XmlDocComandi.CreateElement("formato")

        Nodo.InnerText = "CrystalReports"

        NodoDocumento.AppendChild(Nodo)


        '. file
        Nodo = XmlDocComandi.CreateElement("file")

        Nodo.InnerText = NomeFileDati

        NodoDocumento.AppendChild(Nodo)


        '. stampante
        Nodo = XmlDocComandi.CreateElement("stampante")

        Nodo.InnerText = NomeStampante

        NodoDocumento.AppendChild(Nodo)


        '. numeroCopie
        Nodo = XmlDocComandi.CreateElement("numeroCopie")

        Nodo.InnerText = CStr(NumeroCopie)

        NodoDocumento.AppendChild(Nodo)


        '. collated
        Nodo = XmlDocComandi.CreateElement("collated")

        Nodo.InnerText = Str_Flag_Fascicola

        NodoDocumento.AppendChild(Nodo)


        '. startPage
        Nodo = XmlDocComandi.CreateElement("startPage")

        Nodo.InnerText = CStr(Start_Page)

        NodoDocumento.AppendChild(Nodo)


        '. endPage
        Nodo = XmlDocComandi.CreateElement("endPage")

        Nodo.InnerText = CStr(End_Page)

        NodoDocumento.AppendChild(Nodo)


        Return XmlDocComandi


    End Function

    '####################################################################
    Public Function Recupera_Lista_Stampanti(ByVal objAgroWebConfig As AgroWebConfig) As String()

        Dim Lista_Stampanti As String = ""
        Dim Vet_Stampanti As String()


        If Not IsNothing(objAgroWebConfig.Lista_Stampanti) Then
            Lista_Stampanti = CStr(objAgroWebConfig.Lista_Stampanti)
        End If

        If InStr(Lista_Stampanti, "|", ) > 0 Then
            Vet_Stampanti = Lista_Stampanti.Split("|")
        Else
            ReDim Vet_Stampanti(0)
            Vet_Stampanti(0) = Lista_Stampanti
        End If

        Return Vet_Stampanti

    End Function



End Class
