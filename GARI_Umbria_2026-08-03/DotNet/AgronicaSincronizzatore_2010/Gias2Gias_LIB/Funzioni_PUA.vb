Imports System.Xml
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider
Imports AgronicaCoreModello
Imports System.Text

Partial Public Class Funzioni

    Public Sub Elabora_XML_PUA_Salva(
                            ByVal objOpzioniImportImpresa As AgronicaCoreModello.clsImpresa,
                            ByVal objOpzioni As clsOpzioni,
                            ByRef Log_Import As StringBuilder,
                            ByRef Log_Errori As StringBuilder,
                            ByRef Log_Riepilogo As StringBuilder,
                            ByVal Piva_Origine As String,
                            ByVal Piva_Destinazione As String,
                            ByVal Piva_Padre_Destinazione As String,
                            ByVal Piva_SuperUser_Destinazione As String
                    )

        Const NomeFunzione As String = "Elabora_XML_Pua_Salva"

        Try

            Dim leggiPua As New AgronicaCoreG2GLocalDal.G2GPUA_R
            Dim oPua As AgronicaCoreModello.G2G_Pua = leggiPua.LeggiPerGias2Gias(Piva_Origine, Piva_Destinazione, objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser, objOpzioniImportImpresa, objOpzioni.objParametri_Server_GIAS_ORIGINE)

            Dim sXmlPua As String = AgronicaCoreUtility.XMLUtility.SerializzaOggetto(Of G2G_Pua)(oPua, "")

            Dim mainDoc As String = Funzioni_MasterG2G.XmlMasterDataPiva(enum_TipoOperazioneDB.Scrittura, Piva_Origine, "", objOpzioni)
            mainDoc = AgronicaCoreUtility.XDocUtils.IniettaSottoAlberoDaStringaXml(mainDoc, sXmlPua, "//utente/DatiImprese/Impresa", "DatiPua")

            Dim wsimportazione As New WS_Importa_GIAS_2014.ImportaWS With {.Url = objOpzioni.wsimportaGiasURl, .Timeout = _timeout_ws_Importa}
            Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)

            '  Marco Grilli, 17/06/2014 14:57:29: zippo la stringa in gzip per velocizzare il trasferimento
            Dim sStringaDaSalvare As String = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, mainDoc)
            Dim outputPua As String = wsimportazione.Scrivi_XmlFull(Str_Credenziali_WS, sStringaDaSalvare)

            'leggo la struttura restituita contenente i recode da inserire/modificare/cancellare
            Dim xmlRisposta As XDocument = XDocument.Parse(outputPua)
            Dim rispostaPua As String = xmlRisposta.Element("Risposta").Element("Risposta_DatiPua").FirstNode.ToString

            Dim risposta As AgronicaCoreModello.G2G_Recode = AgronicaCoreUtility.XMLUtility.DeserializzaOggetto(Of AgronicaCoreModello.G2G_Recode)(rispostaPua, "")

            Dim MessaggioErrore As String = ""
            ' se la scrittura su destinazione è andata a buon fine aggiorno i recode sull'origine
            If String.IsNullOrEmpty(risposta.MessaggioErrore) Then
                MessaggioErrore = AgronicaCoreG2GLocalDal.G2GUtility.Scrivi_G2G_Recode(risposta, objOpzioni.objParametri_Server_GIAS_ORIGINE)
            Else
                MessaggioErrore = risposta.MessaggioErrore
            End If

            ' log trasferimento dati
            If String.IsNullOrEmpty(MessaggioErrore) Then
                If Not String.IsNullOrEmpty(risposta.LogRecode) Then
                    Log_Import.AppendLine("Trasferimento Pua: " & risposta.LogRecode)
                End If
            Else
                Log_Import.AppendLine("Trasferimento Pua ERRORE: " & MessaggioErrore)
            End If

            'nuove, modificate, eliminate
            Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Pua: " & oPua.pua_testata_insert.Count & " nuovi " & oPua.pua_testata_update.Count & " modificati " & oPua.Recode.G2GRecodePuaToDelete.Count & " cancellati ")

        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: " & ex.Message
            Log_Import.AppendLine(msg)
            Log_Errori.AppendLine(msg)
            Throw New Exception(msg)
        End Try

    End Sub

    Public Sub Elabora_XML_PUA_SalvaReverse(
                            ByVal objOpzioniImportImpresa As AgronicaCoreModello.clsImpresa,
                            ByVal objOpzioni As clsOpzioni,
                            ByRef Log_Import As StringBuilder,
                            ByRef Log_Errori As StringBuilder,
                            ByRef Log_Riepilogo As StringBuilder,
                            ByVal Piva_Origine As String,
                            ByVal Piva_Destinazione As String,
                            ByVal Piva_Padre_Destinazione As String,
                            ByVal Piva_SuperUser_Destinazione As String
                    )

        Const NomeFunzione As String = "Elabora_XML_PUA_SalvaReverse"
        Dim outputPua As String = ""
        Dim sXmlPua As String = ""
        Try

            Dim leggiPua As New AgronicaCoreG2GLocalDal.G2GPUA_R
            Dim oPua As AgronicaCoreModello.G2G_Pua_Reverse = leggiPua.LeggiPerGias2GiasReverse(Piva_Origine, Piva_Destinazione, objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser, objOpzioniImportImpresa, objOpzioni.objParametri_Server_GIAS_ORIGINE)

            sXmlPua = AgronicaCoreUtility.XMLUtility.SerializzaOggetto(Of G2G_Pua_Reverse)(oPua, "")

            Dim mainDoc As String = Funzioni_MasterG2G.XmlMasterDataPiva(enum_TipoOperazioneDB.Scrittura, Piva_Origine, "", objOpzioni)
            mainDoc = AgronicaCoreUtility.XDocUtils.IniettaSottoAlberoDaStringaXml(mainDoc, sXmlPua, "//utente/DatiImprese/Impresa", "DatiPua")

            Dim wsimportazione As New WS_Importa_GIAS_2014.ImportaWS With {.Url = objOpzioni.wsimportaGiasURl, .Timeout = _timeout_ws_Importa}
            Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)

            '  Marco Grilli, 17/06/2014 14:57:29: zippo la stringa in gzip per velocizzare il trasferimento
            Dim sStringaDaSalvare As String = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, mainDoc)
            outputPua = wsimportazione.Scrivi_XmlFull(Str_Credenziali_WS, sStringaDaSalvare)

            'leggo la struttura restituita contenente i recode da inserire/modificare/cancellare
            Dim xmlRisposta As XDocument = XDocument.Parse(outputPua)
            Dim rispostaPua As String = xmlRisposta.Element("Risposta").Element("Risposta_DatiPua").FirstNode.ToString

            Dim risposta As AgronicaCoreModello.G2G_Recode = AgronicaCoreUtility.XMLUtility.DeserializzaOggetto(Of AgronicaCoreModello.G2G_Recode)(rispostaPua, "")

            Dim MessaggioErrore As String = ""
            ' se la scrittura su destinazione è andata a buon fine aggiorno i recode sull'origine
            If String.IsNullOrEmpty(risposta.MessaggioErrore) Then
                MessaggioErrore = AgronicaCoreG2GLocalDal.G2GUtility.Scrivi_G2G_Recode(risposta, objOpzioni.objParametri_Server_GIAS_ORIGINE)
            Else
                MessaggioErrore = risposta.MessaggioErrore
            End If

            ' log trasferimento dati
            If String.IsNullOrEmpty(MessaggioErrore) Then
                If Not String.IsNullOrEmpty(risposta.LogRecode) Then
                    Log_Import.AppendLine("Trasferimento Pua: " & risposta.LogRecode)
                End If
            Else
                Log_Import.AppendLine("Trasferimento Pua ERRORE: " & MessaggioErrore)
            End If

            'nuove, modificate, eliminate
            Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Pua: " & oPua.pua_testata_insert.Count & " nuovi " & oPua.pua_testata_update.Count & " modificati " & oPua.Recode.G2GRecodePuaToDelete.Count & " cancellati ")

        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: " & ex.Message
            If ex.InnerException IsNot Nothing Then
                msg &= vbCrLf & "InnerException:" & ex.InnerException.Message
            End If
            msg &= vbCrLf & "sXmlPua:" & sXmlPua
            msg &= vbCrLf & "outputPua:" & outputPua
            Log_Import.AppendLine(msg)
            Log_Errori.AppendLine(msg)
            Throw New Exception(msg)
        End Try

    End Sub


    Public Sub Elabora_PUA_Salva(
                                    ByVal objOpzioni As clsOpzioni,
                                    ByRef Log_Import As StringBuilder,
                                    ByRef Log_Errori As StringBuilder,
                                    ByRef Log_Riepilogo As StringBuilder,
                                    ByVal Piva_Origine As String,
                                    ByVal Piva_Destinazione As String
                            )

        Const nomeFunzione As String = "Elabora_PUA_Salva"


        Try

            Dim LeggiFertilizzazione As New AgronicaCorePUA_DAL.Fertilizzazione_R
            Dim ScriviFertilizzazione As New AgronicaCorePUA_DAL.Fertilizzazione_W


            Dim Leggi_Pua_Testata As New AgronicaCorePUA_DAL.PUA_Testata_R
            Dim Scrivi_Pua_Testata As New AgronicaCorePUA_DAL.PUA_Testata_W
            Dim dtPua_Testata As DataTable

            dtPua_Testata = Leggi_Pua_Testata.Leggi(
                0, 0,
                Piva_Origine,
                 AGRODATAINIZIO, AGRODATAFINE,
                 "",
                "",
                objOpzioni.objParametri_Server_GIAS_ORIGINE
            )

            Dim ObjSequenze As New AgronicaCoreDataProvider.Agro_Sequenze
            Dim nuovoPUA_cod As Integer

            For Each rowPua_Testata In dtPua_Testata.Rows



                nuovoPUA_cod = ObjSequenze.NuovoId_Tabella(
                    "PUA_Testata",
                    0,
                    20000000,
                    objOpzioni.objParametri_Server_GIAS_DESTINAZIONE
                )

                Scrivi_Pua_Testata.Scrivi(
                    nuovoPUA_cod,
                    Piva_Destinazione,
                    rowPua_Testata("sa_cod"),
                    rowPua_Testata("PUA_Anno"),
                    rowPua_Testata("PUA_Tipo"),
                    rowPua_Testata("Sup_Lagoni"),
                    rowPua_Testata("Coeff_Efficienza"),
                    rowPua_Testata("Colture_Vernine"),
                    rowPua_Testata("Legge59"),
                    rowPua_Testata("Note"),
                    rowPua_Testata("regolamento_cod"),
                    rowPua_Testata("tipo_allevamento"),
                    rowPua_Testata("Perc_Zootecnico"),
                    rowPua_Testata("Matrice_Prevalente"),
                    rowPua_Testata("Validita_Fine"),
                    rowPua_Testata("Validita_Inizio"),
                    rowPua_Testata("Flag_NonUtilizzo_Fertilizzanti"),
                    objOpzioni.objParametri_Server_GIAS_DESTINAZIONE,
                    rowPua_Testata("Data_Creazione"),
                    rowPua_Testata("Data_Modifica"),
                    rowPua_Testata("Username_Creazione"),
                    rowPua_Testata("Username_Modifica")
                )

                Dim Leggi_PUA_Consistenze_Animali As New AgronicaCorePUA_DAL.PUA_Consistenze_Animali_R
                Dim Scrivi_PUA_Consistenze_Animali As New AgronicaCorePUA_DAL.PUA_Consistenze_Animali_W

                Dim dtPUA_Consistenze_Animali As DataTable
                dtPUA_Consistenze_Animali = Leggi_PUA_Consistenze_Animali.Leggi(
                    rowPua_Testata("PUA_COD"),
                    "",
                    "",
                    objOpzioni.objParametri_Server_GIAS_ORIGINE
                )

                For Each rowPUA_Consistenze_Animali In dtPUA_Consistenze_Animali.Rows

                    Scrivi_PUA_Consistenze_Animali.Scrivi(
                        nuovoPUA_cod,
                        rowPUA_Consistenze_Animali("Cat_Cod"),
                        rowPUA_Consistenze_Animali("Cod_Fabb"),
                        rowPUA_Consistenze_Animali("Cons_Cod"),
                        rowPUA_Consistenze_Animali("Gen_Cod"),
                        rowPUA_Consistenze_Animali("Ipro_Cod"),
                        rowPUA_Consistenze_Animali("num_capi"),
                        rowPUA_Consistenze_Animali("Spe_Cod"),
                        rowPUA_Consistenze_Animali("regolamento_Cod"),
                        rowPUA_Consistenze_Animali("Validita_Fine"),
                        rowPUA_Consistenze_Animali("Validita_Inizio"),
                        objOpzioni.objParametri_Server_GIAS_DESTINAZIONE,
                        rowPUA_Consistenze_Animali("Data_Creazione"),
                        rowPUA_Consistenze_Animali("Data_Modifica"),
                        rowPUA_Consistenze_Animali("Username_Creazione"),
                        rowPUA_Consistenze_Animali("Username_Modifica")
                    )




                Next 'pua_consistenze_Animali

                Dim LeggiPUA_Effluente As New AgronicaCorePUA_DAL.Pua_Effluente_R
                Dim ScriviPUA_Effluente As New AgronicaCorePUA_DAL.Pua_Effluente_W

                Dim dtPUA_Effluente As DataTable
                dtPUA_Effluente = LeggiPUA_Effluente.Leggi(
                    rowPua_Testata("regolamento_cod"), rowPua_Testata("PUA_COD"), 0,
                    "",
                    "",
                    objOpzioni.objParametri_Server_GIAS_ORIGINE
                )

                For Each rowPUA_Effluente In dtPUA_Effluente.Rows

                    'TODO: EFF_COD da metaschema?

                    Dim lRowPUA_Effluente As Integer? = DBNullToNothing(rowPUA_Effluente("Giorni_stoccaggio"))
                    If lRowPUA_Effluente Is Nothing Then
                        lRowPUA_Effluente = 0
                    End If

                    ScriviPUA_Effluente.Scrivi(rowPUA_Effluente("id"),
                        nuovoPUA_cod,
                        rowPUA_Effluente("Eff_Cod"),
                        rowPUA_Effluente("Azoto_Qta"),
                        rowPUA_Effluente("Azoto_Titoli"),
                        rowPUA_Effluente("Capacita_stoccaggio"),
                        rowPUA_Effluente("Carico"),
                        lRowPUA_Effluente,
                        rowPUA_Effluente("Riempimento"),
                        rowPUA_Effluente("Regolamento_cod"),
                        rowPUA_Effluente("Tipo_Allevamento"),
                        rowPUA_Effluente("Perc_Zootecnico"),
                        rowPUA_Effluente("Matrice_Prevalente"),
                        rowPUA_Effluente("Flag_ProvenienzaEsterna"),
                        rowPUA_Effluente("Validita_Fine"),
                        rowPUA_Effluente("Validita_Inizio"),
                        rowPUA_Effluente("Data_Inizio_Divieto"),
                        rowPUA_Effluente("Data_Fine_Divieto"),
                        objOpzioni.objParametri_Server_GIAS_DESTINAZIONE,
                        rowPUA_Effluente("Data_Creazione"),
                        rowPUA_Effluente("Data_Modifica"),
                        rowPUA_Effluente("Username_Creazione"),
                        rowPUA_Effluente("Username_Modifica")
                    )


                Next ' PUA_Effluente




                Dim LeggiPUA_TrattamentoEffluenti As New AgronicaCorePUA_DAL.Pua_TrattamentoEffluenti_R
                Dim ScriviPUA_TrattamentoEffluenti As New AgronicaCorePUA_DAL.Pua_TrattamentoEffluenti_W

                Dim dtPUA_TrattamentoEffluenti As DataTable

                dtPUA_TrattamentoEffluenti = LeggiPUA_TrattamentoEffluenti.Leggi(
                    rowPua_Testata("PUA_COD"),
                    "",
                    "",
                    objOpzioni.objParametri_Server_GIAS_ORIGINE
                )


                For Each rowPUA_TrattamentoEffluenti In dtPUA_TrattamentoEffluenti.Rows

                    ScriviPUA_TrattamentoEffluenti.Scrivi(
                        nuovoPUA_cod,
                        rowPUA_TrattamentoEffluenti("Cons_Cod"),
                        rowPUA_TrattamentoEffluenti("Piva_SuperUser"),
                        rowPUA_TrattamentoEffluenti("Tratt_Cod"),
                        rowPUA_TrattamentoEffluenti("Compostato_Maturato"),
                        rowPUA_TrattamentoEffluenti("Efficienza_Cod"),
                        rowPUA_TrattamentoEffluenti("Inerte_Aggiunto"),
                        rowPUA_TrattamentoEffluenti("Liq_Trattati"),
                        rowPUA_TrattamentoEffluenti("Regolamento_cod"),
                        rowPUA_TrattamentoEffluenti("Validita_Fine"),
                        rowPUA_TrattamentoEffluenti("Validita_Inizio"),
                        objOpzioni.objParametri_Server_GIAS_DESTINAZIONE,
                        rowPUA_TrattamentoEffluenti("Data_Creazione"),
                        rowPUA_TrattamentoEffluenti("Data_Modifica"),
                        rowPUA_TrattamentoEffluenti("Username_Creazione"),
                        rowPUA_TrattamentoEffluenti("Username_Modifica")
                    )

                Next 'PUA_TrattamentoEffluenti


                Dim LeggiPUA_Planning As New AgronicaCorePUA_DAL.Pua_Programmazione_R
                Dim ScriviPUA_Planning As New AgronicaCorePUA_DAL.Pua_Programmazione_W

                Dim dtPUA_Planning As DataTable

                dtPUA_Planning = LeggiPUA_Planning.Leggi(
                    rowPua_Testata("PUA_COD"),
                    "",
                    "",
                    objOpzioni.objParametri_Server_GIAS_ORIGINE
                )


                For Each rowPUA_planning In dtPUA_Planning.Rows

                    Dim nuovo_programmazione_cod As Integer = 0

                    nuovo_programmazione_cod = recode_dammi_programmazione_cod(
                        rowPUA_planning("piva_superuser"),
                        rowPUA_planning("programmazione_cod")
                    )


                    ScriviPUA_Planning.Scrivi(
                        nuovoPUA_cod,
                        nuovo_programmazione_cod,
                        rowPUA_planning("Regolamento_cod"),
                        rowPUA_planning("Validita_Fine"),
                        rowPUA_planning("Validita_Inizio"),
                        objOpzioni.objParametri_Server_GIAS_DESTINAZIONE,
                        rowPUA_planning("Data_Creazione"),
                        rowPUA_planning("Data_Modifica"),
                        rowPUA_planning("Username_Creazione"),
                        rowPUA_planning("Username_Modifica")
                    )


                    Dim lRowPUA_planning As Integer = rowPUA_planning("programmazione_cod")
                    Dim listaProgrammazione_entita_cod As List(Of recode_planning) =
                            (
                                From pl In _funzioniGLOBAL.Planning
                                Where pl.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser _
                                And pl.From_Piva = Piva_Origine _
                                And pl.From_Programmazione_Cod = lRowPUA_planning _
                                And pl.From_Programmazione_Entita_cod > 0
                                Select pl
                                ).ToList

                    For Each iProgrammazione_entita_cod As recode_planning In listaProgrammazione_entita_cod

                        Dim dtLeggiFertilizzazione As DataTable
                        dtLeggiFertilizzazione = LeggiFertilizzazione.Leggi(
                            iProgrammazione_entita_cod.From_Programmazione_Entita_cod,
                            "",
                            "",
                            objOpzioni.objParametri_Server_GIAS_ORIGINE
                        )

                        For Each rowLeggiFertilizzazione In dtLeggiFertilizzazione.Rows

                            ScriviFertilizzazione.Scrivi(
                                rowLeggiFertilizzazione("ApportoxDistrib"),
                                rowLeggiFertilizzazione("ApportoxHa"),
                                rowLeggiFertilizzazione("Data_Fert"),
                                rowLeggiFertilizzazione("Eff_Perc"),
                                rowLeggiFertilizzazione("EM_Cod"),
                                rowLeggiFertilizzazione("Fer_Cod"),
                                rowLeggiFertilizzazione("Id_Tp_Fer"),
                                rowLeggiFertilizzazione("N_Reale"),
                                rowLeggiFertilizzazione("NnettoxDistrib"),
                                rowLeggiFertilizzazione("NnettoxHa"),
                                rowLeggiFertilizzazione("NutilexDistrib"),
                                rowLeggiFertilizzazione("NutilexHa"),
                                iProgrammazione_entita_cod.To_Programmazione_Entita_cod,
                                rowLeggiFertilizzazione("Validita_Fine"),
                                rowLeggiFertilizzazione("Validita_Inizio"),
                                objOpzioni.objParametri_Server_GIAS_DESTINAZIONE,
                                rowLeggiFertilizzazione("Data_Creazione"),
                                rowLeggiFertilizzazione("Data_Modifica"),
                                rowLeggiFertilizzazione("Username_Creazione"),
                                rowLeggiFertilizzazione("Username_Modifica")
                            )

                        Next 'Fertilizzazione


                    Next 'entità legate a pua_planning da query linq

                Next 'pua Planning

            Next 'pua_testata





        Catch ex As Exception

            Dim msg As String
            msg = CStr(Date.Now) + " - " + nomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)

        End Try

    End Sub



End Class
