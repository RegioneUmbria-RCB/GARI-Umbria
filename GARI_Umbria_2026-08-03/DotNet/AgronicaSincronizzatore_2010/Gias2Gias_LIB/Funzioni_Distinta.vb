Imports System.Xml
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreModello.Anagrafe

Imports AgronicaCoreDataProvider

Imports System.Text
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModello

Partial Public Class Funzioni

#Region "Ditinta"

    Public Function Elabora_XML_Distinta_Salva_sistemaXML(ByVal objOpzioni As clsOpzioni,
                                                          ByRef Log_Import As StringBuilder,
                                                          ByRef Log_Errori As StringBuilder,
                                                          ByRef Log_Riepilogo As StringBuilder,
                                                          ByVal Piva_Origine As String,
                                                          ByVal Piva_Destinazione As String,
                                                          ByVal Sa_cod_Origine As Integer,
                                                          ByVal sa_cod_destinazione As Integer,
                                                          ByVal strXMLDistinta As String,
                                                          ByVal Progetto_cod_Destinazione As String,
                                                          ByVal TipoOperazioneDB As enum_TipoOperazioneDB
                                                          ) As String

        Const nomeFunzione = "Elabora_XML_Distinta_Salva_sistemaXML"

        Dim XmlDocCont As New XmlDocument
        XmlDocCont.LoadXml(strXMLDistinta)

        Try
            'tipo operazione = scrittura
            For Each XML_Nodo As XmlElement In XmlDocCont.SelectNodes("//*[@TipoOperazioneDB]")
                XML_Nodo.SetAttribute("TipoOperazioneDB", TipoOperazioneDB)
            Next

            'chiavi
            For Each XML_Nodo As XmlElement In XmlDocCont.SelectNodes("//*[@appezza]")

                Dim oldAppezza As Integer = XML_Nodo.Attributes("appezza").Value
                Dim oldIdReg As Integer = XML_Nodo.Attributes("id_reg").Value
                
                Dim decode As G2G_Recode_Impianti = recode_return_Impianto(Piva_Origine, Sa_cod_Origine,
                                                                           oldAppezza, oldIdReg, 
                                                                           objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser)

                If decode Is Nothing Then
                    Throw New Exception("Impianto non trovato")
                End If

                'azzero il cod_indirizzo che verrà assegnato dal core
                XML_Nodo.SetAttribute("progetto_cod", Progetto_cod_Destinazione)
                XML_Nodo.SetAttribute("piva", Piva_Destinazione)
                XML_Nodo.SetAttribute("sa_cod", sa_cod_destinazione)
                XML_Nodo.SetAttribute("appezza", decode.To_Appezza)
                XML_Nodo.SetAttribute("id_reg", decode.To_Id_Reg)
                XML_Nodo.SetAttribute("basecode", objOpzioni.BaseCode_DESTINAZIONE)
                XML_Nodo.SetAttribute("topcode", objOpzioni.TopCode_DESTINAZIONE)
                
            Next

        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) + " - " + nomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try

        Return XmlDocCont.OuterXml

    End Function
    
    Public Sub Elabora_XML_Distinta_Salva(ByVal objOpzioniImportImpresa As clsImpresa,
                                          ByVal objOpzioni As clsOpzioni,
                                          ByRef Log_Import As StringBuilder,
                                          ByRef Log_Errori As StringBuilder,
                                          ByRef Log_Riepilogo As StringBuilder,
                                          ByVal Piva_Origine As String,
                                          ByVal Piva_Destinazione As String,
                                          ByVal Sa_cod_Origine As Integer,
                                          ByVal sa_cod_destinazione As Integer)

        Const nomeFunzione = "Elabora_Distinta_Salva"

        Dim leggiProgetto As New AgronicaCoreAnagrafeBIZ.Progetto_R
        Dim scriviProgetto As New AgronicaCoreAnagrafeBIZ.Progetto_W
        Dim conteggioProgettiNuovi As Integer = 0
        Dim xmlProgetto As String
        Dim xmlDocProgetto As New XmlDocument

        Try

            'memorizza il dato precedente per successivo recupero
            Dim FinestraTemporaleFinePrecedente As Date
            Dim FinestraTemporaleInizioPrecedente As Date

            FinestraTemporaleRecupera(objOpzioni, FinestraTemporaleInizioPrecedente, FinestraTemporaleFinePrecedente)

            'imposta il filtro temporale
            objOpzioni.objParametri_Server_GIAS_ORIGINE.FinestraTemporaleInizio =
                objOpzioniImportImpresa.ValiditaInizio_pianocolturale

            objOpzioni.objParametri_Server_GIAS_ORIGINE.FinestraTemporaleFine =
                objOpzioniImportImpresa.ValiditaFine_pianocolturale


            xmlProgetto = leggiProgetto.Impresa_Progetti_Leggi(Piva_Origine,
                                                               0,
                                                               "",
                                                               0,
                                                               Sa_cod_Origine,
                                                               0,
                                                               0,
                                                               0,
                                                               0,
                                                               False,
                                                               objOpzioni.objParametri_Server_GIAS_ORIGINE,
                                                               isGias2Gias:=True,
                                                               TipoG2G:=1)

            If Not xmlProgetto Is Nothing AndAlso xmlProgetto <> "" Then

                xmlDocProgetto.LoadXml(xmlProgetto)

                For Each xProgettoNode As XmlNode In xmlDocProgetto.SelectNodes("//DatiProgetto/Progetto")

                    Dim idlePiva As String = ""
                    Dim Nuovo_Progetto_COD As Integer = -1

                    Dim oldAppezza As Integer = xProgettoNode.Attributes("appezza").Value
                    Dim oldIdReg As Integer = xProgettoNode.Attributes("id_reg").Value

                    ' VAnni: 20/5/2019: verifico se la distinta va inviata o meno rispetto alla lista degli impianti.. 
                    '    esiste appezzamento oppure lista vuota = invia tutto

                    Dim xInvioDistinta As Boolean = ((From iC In objOpzioniImportImpresa.Impianti
                                                      Where iC.Sa_Cod = Sa_cod_Origine And
                                                            iC.Appezza = oldAppezza And
                                                              iC.ID_Reg = oldIdReg
                                                            ).ToList.Count > 0)

                    If objOpzioniImportImpresa.Impianti.Count = 0 OrElse xInvioDistinta Then

                        Dim sStringaDaSalvare As String
                        sStringaDaSalvare =
                            "<DatiProgetto>" &
                                Elabora_XML_Distinta_Salva_sistemaXML(
                                    objOpzioni,
                                    Log_Import,
                                    Log_Errori,
                                    Log_Riepilogo,
                                    Piva_Origine,
                                    Piva_Destinazione,
                                    Sa_cod_Origine,
                                    sa_cod_destinazione,
                                    xProgettoNode.OuterXml,
                                    0,
                                    enum_TipoOperazioneDB.Scrittura
                                ) & "</DatiProgetto>"

                        If objOpzioni.isGias2Gias_local Then

                            Dim okSalva As Boolean = scriviProgetto.Impresa_Progetto_Scrivi(sStringaDaSalvare,
                                                                                            idlePiva,
                                                                                            Nuovo_Progetto_COD,
                                                                                            objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)

                        Else

                            Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)
                            Dim wsImportazione As New WS_Importa_GIAS_2014.ImportaWS With {
                                .Timeout = _timeout_ws_Importa,
                                .Url = objOpzioni.wsimportaGiasURl
                            }

                            '  Marco Grilli, 17/06/2014 14:57:29: zippo la stringa in gzip per velocizzare il trasferimento
                            sStringaDaSalvare = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, sStringaDaSalvare)

                            Nuovo_Progetto_COD = wsImportazione.Scrivi_DistintaXmlPrivato(Str_Credenziali_WS,
                                                                                          sStringaDaSalvare)

                            If Nuovo_Progetto_COD = -1 Then
                                Throw New Exception("Errore WS in Scrivi_DistintaXmlPrivato")
                            End If

                        End If

                        Dim lProgettoCod As String = xProgettoNode.Attributes("progetto_cod").Value

                        DistintaADD(
                            New G2G_Recode_Distinta With {
                                        .From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser,
                                        .To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser,
                                       .From_Piva = Piva_Origine,
                                       .From_Progetto_cod = lProgettoCod,
                                       .To_Piva = Piva_Destinazione,
                                       .To_Progetto_cod = Nuovo_Progetto_COD
                                   })

                        conteggioProgettiNuovi += 1

                    End If

                Next

            End If

            ' log esercizi
            If conteggioProgettiNuovi > 0 Then
                Log_Import.Append("L'impresa " & Piva_Origine & " ha " & conteggioProgettiNuovi & " esercizi nuovi" + vbCrLf)
            Else
                'Log_Import.Append("L'impresa " & Piva_Origine & " non ha esercizi nuovi" + vbCrLf)
            End If

            FinestraTemporaleImpostaValori(objOpzioni, FinestraTemporaleInizioPrecedente, FinestraTemporaleFinePrecedente)

        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) + " - " + nomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try

    End Sub

    Public Sub Elabora_XML_Distinta_Modifica(ByVal objOpzioni As clsOpzioni,
                                             ByRef Log_Import As StringBuilder,
                                             ByRef Log_Errori As StringBuilder,
                                             ByRef Log_Riepilogo As StringBuilder,
                                             ByVal Piva_Origine As String,
                                             ByVal Piva_Destinazione As String,
                                             ByVal Sa_cod_Origine As Integer,
                                             ByVal sa_cod_destinazione As Integer)

        Const nomeFunzione = "Elabora_Distinta_Modifica"

        Dim leggiProgetto As New AgronicaCoreAnagrafeBIZ.Progetto_R
        Dim scriviProgetto As New AgronicaCoreAnagrafeBIZ.Progetto_W
        Dim conteggioProgettiModificati As Integer = 0
        Dim xmlDocProgetto As New XmlDocument

        Try

            Dim xmlProgetto As String = leggiProgetto.Impresa_Progetti_Leggi(Piva_Origine,
                                                                             0,
                                                                             "",
                                                                             0,
                                                                             Sa_cod_Origine,
                                                                             0,
                                                                             0,
                                                                             0,
                                                                             0,
                                                                             False,
                                                                             objOpzioni.objParametri_Server_GIAS_ORIGINE,
                                                                             isGias2Gias:=True,
                                                                             TipoG2G:=2)

            If Not xmlProgetto Is Nothing AndAlso xmlProgetto <> "" Then

                xmlDocProgetto.LoadXml(xmlProgetto)

                For Each xProgettoNode As XmlNode In xmlDocProgetto.SelectNodes("//DatiProgetto/Progetto")

                    Dim idlePiva As String = ""
                    Dim Nuovo_Progetto_COD As Integer = -1

                    Dim lProgettoCod As String = xProgettoNode.Attributes("progetto_cod").Value

                    Dim distinta As G2G_Recode_Distinta = recode_return_Distinta(Piva_Origine, lProgettoCod,
                                                                                 objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser)

                    Dim Progetto_cod_Destinazione As Integer = 0
                    If Not distinta Is Nothing Then
                        Progetto_cod_Destinazione = distinta.To_Progetto_cod
                    End If

                    ' VAnni: 22/5/2019: in modifica si può evitare di passare il filtro impianti, lista vuota fa al caso nostro .. (New List(Of Impianto_Colturale),)
                    Dim sStringaDaSalvare As String =
                        "<DatiProgetto>" &
                            Elabora_XML_Distinta_Salva_sistemaXML(
                                objOpzioni,
                                Log_Import,
                                Log_Errori,
                                Log_Riepilogo,
                                Piva_Origine,
                                Piva_Destinazione,
                                Sa_cod_Origine,
                                sa_cod_destinazione,
                                xProgettoNode.OuterXml,
                                Progetto_cod_Destinazione,
                                enum_TipoOperazioneDB.Modifica
                            ) & "</DatiProgetto>"

                    If objOpzioni.isGias2Gias_local Then

                        Dim okSalva As Boolean = scriviProgetto.Impresa_Progetto_Scrivi(sStringaDaSalvare,
                                                                                        idlePiva,
                                                                                        Nuovo_Progetto_COD,
                                                                                        objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)

                    Else

                        Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)
                        Dim wsImportazione As New WS_Importa_GIAS_2014.ImportaWS With {
                            .Timeout = _timeout_ws_Importa,
                            .Url = objOpzioni.wsimportaGiasURl
                        }

                        '  Marco Grilli, 17/06/2014 14:57:29: zippo la stringa in gzip per velocizzare il trasferimento
                        sStringaDaSalvare = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, sStringaDaSalvare)

                        Nuovo_Progetto_COD = wsImportazione.Scrivi_DistintaXmlPrivato(Str_Credenziali_WS,
                                                                                      sStringaDaSalvare)

                        If Nuovo_Progetto_COD = -1 Then
                            Throw New Exception("Errore WS in Scrivi_DistintaXmlPrivato")
                        End If
                    End If

                    If Not distinta Is Nothing Then
                        DistintaEDIT(distinta)
                        conteggioProgettiModificati += 1
                    End If

                Next

            End If

            ' log esercizi
            If conteggioProgettiModificati > 0 Then
                Log_Import.Append("L'impresa " & Piva_Origine & " ha " & conteggioProgettiModificati & " esercizi modificati" + vbCrLf)
            Else
                'Log_Import.Append("L'impresa " & Piva_Origine & " non ha esercizi modificati" + vbCrLf)
            End If

        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) + " - " + nomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try

    End Sub

    Public Function recode_return_Distinta(ByVal oldPiva As String,
                                           ByVal oldProgetto_Cod As Integer,
                                           ByVal piva_SuperUser_destinazione As String
                                           ) As G2G_Recode_Distinta

        If NuovaLogicaRecodePC Then
            Return (From x In _efG2G.G2G_Recode_Distinta
                    Where x.From_Piva = oldPiva AndAlso
                           x.From_Progetto_cod = oldProgetto_Cod AndAlso
                           x.To_PivaSuperUser = piva_SuperUser_destinazione
                    Select x).FirstOrDefault
        Else
            Return (From x In _Distinta
                    Where x.From_Piva = oldPiva AndAlso
                           x.From_Progetto_cod = oldProgetto_Cod AndAlso
                           x.To_PivaSuperUser = piva_SuperUser_destinazione
                    Select x).FirstOrDefault
        End If

    End Function

    Public Function recode_return_DistintaReverse(ByVal oldPiva As String,
                                           ByVal oldProgetto_Cod As Integer,
                                           ByVal piva_SuperUser_destinazione As String
                                           ) As G2G_Recode_Distinta

        If NuovaLogicaRecodePC Then
            Return (From x In _efG2G.G2G_Recode_Distinta
                    Where x.From_Piva = oldPiva AndAlso
                           x.To_Progetto_cod = oldProgetto_Cod AndAlso
                           x.From_PivaSuperUser = piva_SuperUser_destinazione
                    Select x).FirstOrDefault
        Else
            Return (From x In _Distinta
                    Where x.From_Piva = oldPiva AndAlso
                           x.To_Progetto_cod = oldProgetto_Cod AndAlso
                           x.From_PivaSuperUser = piva_SuperUser_destinazione
                    Select x).FirstOrDefault
        End If

    End Function

#End Region

End Class
