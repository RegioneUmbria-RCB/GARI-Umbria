Imports System.Data
Imports System.Xml
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Analisi_Certificato_R
    Inherits AgronicaCoreDataProvider.LogProvider

    Public Function Analisi_Certificato_Leggi(ByVal Analisi_Certificato_Cod As Long,
                                              ByVal Piva As String,
                                              ByVal Sa_Cod As Long,
                                              ByVal Campo_Cod As Long,
                                              ByVal Appezza As Long,
                                              ByVal Id_Imp As Long,
                                              ByVal Fabbricato_Cod As Long,
                                              ByVal prov As String,
                                              ByVal com As String,
                                              ByVal sezione As String,
                                              ByVal foglio As Long,
                                              ByVal Numero As Long,
                                              ByVal subalterno As String,
                                              ByVal Id_Oggetto_Grafico As String,
                                              ByVal ForDelete As Boolean,
                                              ByRef objParametri As AgronicaCoreParametri
                                              ) As String

        Const nomeRoutine = "AnagrafeBIZ.Analisi_Certificato_R.Analisi_Certificato_Leggi()"

        Dim messaggioErrore As String = ""

        Dim risultatoFunzione As String = String.Empty

        Dim xmlDoc As New XmlDocument

        Dim xmlCertificato As XmlElement
        Dim xmlDatiTestate As XmlElement

        Dim FlagConnessioneLocale As Boolean

        Try

            '------------------------------
            'Verifico se è stata impostata una connessione
            If IsNothing(objParametri.objConnessione) Then
                FlagConnessioneLocale = True
                objParametri.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
            End If
            '------------------------------



            '#################################
            '##########  Certificati  ############
            '#################################

            xmlDatiTestate = xmlDoc.CreateElement("DatiTestate")

            If Analisi_Certificato_Cod <> 0 Then

                Dim dtCertificato As DataTable
                Dim objCertificato As New AgronicaCoreAnagrafeDAL.Analisi_Certificato_Read

                dtCertificato = objCertificato.Leggi(CLng(Analisi_Certificato_Cod),
                                                     enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                     "",
                                                     "",
                                                     objParametri)

                Dim j As Integer
                For j = 0 To dtCertificato.Rows.Count - 1

                    '----- < Certificato > -----
                    xmlCertificato = xmlDoc.CreateElement("Certificato")

                    xmlCertificato.SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                    xmlCertificato.SetAttribute("analisi_superuser", Agro_SQL_Load(dtCertificato.Rows(j).Item("Analisi_SuperUser")))
                    xmlCertificato.SetAttribute("analisi_certificato_cod", Agro_SQL_Load(Analisi_Certificato_Cod))

                    xmlCertificato.SetAttribute("analisi_certificato_des", Agro_SQL_Load(dtCertificato.Rows(j).Item("analisi_certificato_des")))
                    xmlCertificato.SetAttribute("analisi_certificato_data_inizio", Agro_SQL_Load(dtCertificato.Rows(j).Item("analisi_certificato_data_inizio")))
                    xmlCertificato.SetAttribute("analisi_certificato_data_fine", Agro_SQL_Load(dtCertificato.Rows(j).Item("analisi_certificato_data_fine")))
                    xmlCertificato.SetAttribute("analisi_certificato_laboratorio", Agro_SQL_Load(dtCertificato.Rows(j).Item("analisi_certificato_laboratorio")))
                    xmlCertificato.SetAttribute("analisi_certificato_tipologiacod", Agro_SQL_Load(dtCertificato.Rows(j).Item("analisi_certificato_tipologiacod")))
                    xmlCertificato.SetAttribute("analisi_certificato_tipocampione", Agro_SQL_Load(dtCertificato.Rows(j).Item("analisi_certificato_tipocampione")))
                    xmlCertificato.SetAttribute("analisi_certificato_provenienza", Agro_SQL_Load(dtCertificato.Rows(j).Item("analisi_certificato_provenienza")))
                    xmlCertificato.SetAttribute("analisi_certificato_verbale", Agro_SQL_Load(dtCertificato.Rows(j).Item("analisi_certificato_verbale")))
                    xmlCertificato.SetAttribute("analisi_certificato_richiedente", Agro_SQL_Load(dtCertificato.Rows(j).Item("analisi_certificato_richiedente")))
                    xmlCertificato.SetAttribute("analisi_certificato_prelevatoda", Agro_SQL_Load(dtCertificato.Rows(j).Item("Analisi_Certificato_PrelevatoDa")))
                    xmlCertificato.SetAttribute("analisi_certificato_comune", Agro_SQL_Load(dtCertificato.Rows(j).Item("Analisi_Certificato_Comune")))
                    xmlCertificato.SetAttribute("analisi_certificato_protocollo", Agro_SQL_Load(dtCertificato.Rows(j).Item("Analisi_Certificato_Protocollo")))
                    xmlCertificato.SetAttribute("analisi_certificato_numregistro", Agro_SQL_Load(dtCertificato.Rows(j).Item("Analisi_Certificato_NumRegistro")))
                    xmlCertificato.SetAttribute("analisi_certificato_sezione", Agro_SQL_Load(dtCertificato.Rows(j).Item("Analisi_Certificato_Sezione")))
                    xmlCertificato.SetAttribute("analisi_certificato_datafirma", Agro_SQL_Load(dtCertificato.Rows(j).Item("Analisi_Certificato_DataFirma")))
                    xmlCertificato.SetAttribute("analisi_certificato_responsabile", Agro_SQL_Load(dtCertificato.Rows(j).Item("Analisi_Certificato_Responsabile")))
                    xmlCertificato.SetAttribute("analisi_certificato_analista", Agro_SQL_Load(dtCertificato.Rows(j).Item("Analisi_Certificato_Analista")))
                    xmlCertificato.SetAttribute("validita_inizio", Agro_SQL_Load(dtCertificato.Rows(j).Item("validita_inizio")))
                    xmlCertificato.SetAttribute("validita_fine", Agro_SQL_Load(dtCertificato.Rows(j).Item("validita_fine")))

                    xmlDatiTestate.AppendChild(xmlCertificato)
                    '----- < / Certificato > -----

                Next

            End If

            xmlDoc.AppendChild(xmlDatiTestate)

            risultatoFunzione = xmlDoc.InnerXml
            '----- < / Documento XML > ----- 


        Catch ex As Exception
            risultatoFunzione = ""
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        Finally

            If FlagConnessioneLocale Then
                objParametri.objConnessione.Close()
                objParametri.objConnessione.Dispose()
            End If

        End Try

        Return risultatoFunzione

    End Function

End Class

'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

