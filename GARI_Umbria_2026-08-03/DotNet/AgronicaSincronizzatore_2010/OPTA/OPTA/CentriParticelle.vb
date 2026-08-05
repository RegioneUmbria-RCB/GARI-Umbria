
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi


Public Class CentriParticelle



    Sub avvia(Chiave_Cliente As Integer, _
              Global_CodiceGias As Integer, _
              PivaSuperuserCheck As String, _
              Anno As Integer, _
              Socio As String, _
              non_usato As String, _
                      Directoryfile As String, _
                      File_CatastoSoci As String, _
                                        LinkWSImportaGIAS As String, _
                      ByRef objparametriserver As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                      ByRef objparametriutenti As AgronicaCoreDataProvider.AgronicaCoreParametri)



        If objparametriserver.PivaSuperUser <> PivaSuperuserCheck Then
            Throw New Exception("objparametriserver.PivaSuperUser <> 03261930543")
        End If



        Dim LogDirectory As String = Directoryfile
        Dim LogErroriFileName As String = "Impportatore_OPTA_" & Date.Now.ToString.Replace("/", "").Replace("\", "").Replace(".", "").Replace(":", "").Replace(" ", "".Replace("-", "")).Replace(",", "") & "_Risultato.log"


        Dim Dt_CatastoSoci As DataTable


        leggiTabelle(Socio, non_usato, _
                      Directoryfile, _
                      File_CatastoSoci, _
                      Anno, _
                      Dt_CatastoSoci, _
                      LogDirectory, LogErroriFileName, objparametriserver)

        'tolgo i non in produzione

        If Dt_CatastoSoci.Rows.Count = 0 Then

            Throw New Exception("Una tabella è vuota")

        End If

        For j = 0 To Dt_CatastoSoci.Rows.Count - 1

            Dt_CatastoSoci.Rows(j).Item("LOCA") = Dt_CatastoSoci.Rows(j).Item("LOCA").ToString.Trim.ToUpper
            Dt_CatastoSoci.Rows(j).Item("PROV") = CInt(Dt_CatastoSoci.Rows(j).Item("PROV")).ToString.Trim.PadLeft(3, "0")
            Dt_CatastoSoci.Rows(j).Item("COMU") = CInt(Dt_CatastoSoci.Rows(j).Item("COMU")).ToString.Trim.ToUpper.PadLeft(3, "0")
            Dt_CatastoSoci.Rows(j).Item("PART") = CInt(Dt_CatastoSoci.Rows(j).Item("PART")).ToString.Trim.ToUpper.PadLeft(5, "0")
            Dt_CatastoSoci.Rows(j).Item("VARI") = CInt(Dt_CatastoSoci.Rows(j).Item("VARI")).ToString.Trim.ToUpper.PadLeft(3, "0")
            Dt_CatastoSoci.Rows(j).Item("GRUP") = CInt(Dt_CatastoSoci.Rows(j).Item("GRUP")).ToString.Trim.ToUpper.PadLeft(2, "0")

            If Dt_CatastoSoci.Rows(j).Item("LOCA").ToString.Trim.ToUpper = "CITTA' DI CASTELLO".ToUpper Then
                Dt_CatastoSoci.Rows(j).Item("LOCA") = "Città di Castello"
            End If
            If Dt_CatastoSoci.Rows(j).Item("LOCA").ToString.Trim.ToUpper = "CITTA' DELLA PIEVE".ToUpper Then
                Dt_CatastoSoci.Rows(j).Item("LOCA") = "Città della Pieve"
            End If
            If Dt_CatastoSoci.Rows(j).Item("LOCA").ToString.Trim.ToUpper = "ALBAREDO D'ADIGE".ToUpper Then
                Dt_CatastoSoci.Rows(j).Item("LOCA") = "Albaredo d Adige"
            End If
            If Dt_CatastoSoci.Rows(j).Item("LOCA").ToString.Trim.ToUpper = "SCORZE'".ToUpper Then
                Dt_CatastoSoci.Rows(j).Item("LOCA") = "Scorzè"
            End If
            If Dt_CatastoSoci.Rows(j).Item("LOCA").ToString.Trim.ToUpper.Contains("'") Then
                Dt_CatastoSoci.Rows(j).Item("LOCA") = Dt_CatastoSoci.Rows(j).Item("LOCA").ToString.Trim.Replace("'", " ")
            End If
        Next

        'Dim view = New DataView(Dt_AnagraficaSoci)
        'Dim Dt_Soci As DataTable = view.ToTable(True, "CODICE")


        Dim view_CatastoSoci_Tutti = New DataView(Dt_CatastoSoci)


        Dim Dt_Aziende_Tutte As DataTable = view_CatastoSoci_Tutti.ToTable(True, {"CUAA", "ANNO"})
        Dim Rows_Aziende As DataRow() = Dt_Aziende_Tutte.Select("ANNO= " & Anno)

        For i = 0 To Rows_Aziende.Count - 1

            Dim annorif As Integer = CInt(Rows_Aziende(i).Item("ANNO").ToString.Trim)
            If annorif <> Anno Then
                Throw New Exception
            End If
            Dim CUAA As String = Rows_Aziende(i).Item("CUAA").ToString.Trim
            If IsNumeric(CUAA) Then
                CUAA = CUAA.PadLeft(11, "0")
            End If
            Dim Codice_Socio As String = CUAA

            Dim objImpresa As New AgronicaCoreAnagrafeDAL.Imprese_Read



            Dim dtc As DataTable = objImpresa.RecuperaDatiImpresa_From_Cuaa(CUAA, "", AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objparametriserver)
            If dtc.Rows.Count <> 1 Then
                Dim messaggio As String = "ATTENZIONE ------------- Cuaa " & CUAA & " non trovato , dtc.Rows.Count  " & dtc.Rows.Count & " "
                Log("", LogDirectory, LogErroriFileName, objparametriserver, messaggio)
                'Throw New Exception
                'salto
            Else



                Dim Piva As String = dtc.Rows(0).Item("PIVA")


                Dim ImpresaPresente As Boolean
                Dim DataModificaImpresa_Gias As Date
                Dim TipOperazione_Impresa As Integer
                Dim CodiceSocioRet As String
                objImpresa.VerificaEsistenza_PivaGIAS(Piva, _
                               Chiave_Cliente, _
                               ImpresaPresente, _
                               DataModificaImpresa_Gias, _
                               CodiceSocioRet, _
                               objparametriserver)



                'controllo se l'impresa esiste già
                Dim TipoGerarchia As String = "1"
                Dim ImpresaPadre As String = objparametriserver.PivaSuperUser
                If ImpresaPresente = False Then
                    Throw New Exception
                Else

                    If Piva = objparametriserver.PivaSuperUser Then
                        TipoGerarchia = "0"
                        ImpresaPadre = "#"
                        TipOperazione_Impresa = 0
                    Else

                        TipOperazione_Impresa = 0

                        If CStr(Codice_Socio) <> CodiceSocioRet Then
                            'Throw New Exception("CStr(Codice_Socio) <> CodiceSocioRet")
                        End If
                    End If
                End If


                Dim objXmlAnagrafe As New AgronicaCoreXML.AnagrafeXML

                Dim XmlDoc As New System.Xml.XmlDocument

                Dim Xml_Utente As System.Xml.XmlElement
                Xml_Utente = objXmlAnagrafe.Xml_Pubblico_Utente(XmlDoc, _
                                                                "#", _
                                                                "#", _
                                                                Global_CodiceGias)





                Dim Xml_Impresa As System.Xml.XmlElement


                Dim comune = "#"
                Dim provincia = "#"
                Dim RL_Comune = "#"
                Dim RL_Prov = "#"
                Dim RL_Nascita_Comune = "#"
                Dim RL_Nascita_Provincia = "#"
                Xml_Impresa = objXmlAnagrafe.Xml_Pubblico_Impresa( _
                                              TipOperazione_Impresa, _
                                              Piva, _
                                              Codice_Socio, _
                                              CUAA, _
                                              CUAA, _
                                             Codice_Socio, _
                                             "#", _
                                             ImpresaPadre, _
                                             TipoGerarchia, _
                                             "1", _
                                             "#", _
                                             "#", _
                                             "xxx", _
                                             "xxx", _
                                             "47023", _
                                             comune, _
                                             provincia, _
                                             "#", _
                                             "#", _
                                             "001", _
                                             "001", _
                                             "", _
                                             "#", "#", _
                                             "#", _
                                             "#", _
                                             "#", _
                                             "#", _
                                             "#", _
                                             "#", _
                                             "#", _
                                             RL_Comune, _
                                             RL_Prov, _
                                             "#", _
                                             "#", _
                                             "#", _
                                             "#", _
                                             "#", _
                                             "#", _
                                             "#", _
                                             "#", _
                                             "#", _
                                             "#", _
                                             "#", _
                                             "#", _
                                             "#", _
                                             "#", _
                                             "#", _
                                             "#", _
                                             "#", _
                                             "#", _
                                             "#", _
                                             "#", _
                                             "#", _
                                             Codice_Socio, _
                                             XmlDoc)

                'alla fine appendo impresa all'utente e salvo




                Dim Dt_Centri_Tutti As DataTable = view_CatastoSoci_Tutti.ToTable(True, {"CUAA", "ANNO", "LOCA", "PROV", "COMU"})
                Dim Rows_Centri As DataRow() = Dt_Centri_Tutti.Select("CUAA= '" & Rows_Aziende(i).Item("CUAA") & "' AND ANNO=" & Rows_Aziende(i).Item("ANNO"))
                'il centro è dato dal socio+localita+zona da filtrare nell'unica tabella del catasto.
                'il nome centro è dato da nome localita - nome zona

                For rigacentro = 0 To Rows_Centri.Count - 1

                    Dim loca As String = Rows_Centri(rigacentro).Item("LOCA").trim

                    'ci possono essere dei codici che hanno la stessa descrizione,
                    'questo creerebbe due centrri con lo stesso nome e non è corretto,
                    'devo quindi controllare questa cosa e prendere sempre lo stesso codice, ad esempio il più piccolo
                    'la cosa migliore per evitare errori è modificare subito la tabella del catasto appena creata 
                    'sostituendo la voce CODLOC con quella piu piccola che ha lo stesso nome


                    Dim Chiave_Podere As String = Codice_Socio & _
                                    "/" & loca


                    Dim sa_nome As String = loca

                    Dim Centri_Codici_Read As New AgronicaCoreAnagrafeDAL.Centri_Codici_Read
                    Dim Sa_Cod As Integer
                    Dim TipOperazione_Centro As Integer
                    Dim DataModificaCentro_Gias As Date
                    Dim sa_nome_ret As String
                    Centri_Codici_Read.Esiste_Centro(Piva, _
                                                              Chiave_Cliente, _
                                                              Chiave_Podere, _
                                                              Sa_Cod, _
                                                              sa_nome_ret, _
                                                              DataModificaCentro_Gias, _
                                                              "", _
                                                              objparametriserver)

                    If Sa_Cod = 0 Then

                        Dim centr As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
                        Dim dtce As DataTable = centr.Leggi(Piva, 0, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, " sa_nome =  '" & AgronicaCoreDataProvider.UtilityProvider.Agro_SQL_SaveText(sa_nome) & "' ", "", objparametriserver)

                        'Dim ccod As New AgronicaCoreAnagrafeDAL.Centri_Codici_Read
                        'Dim dtcd As DataTable=ccod.Leggi(

                        If dtce.Rows.Count > 0 Then
                            Sa_Cod = dtce.Rows(0).Item("Sa_Cod")
                            TipOperazione_Centro = enum_TipoOperazioneDB.Lettura
                            Chiave_Podere = "#"
                        Else
                            TipOperazione_Centro = enum_TipoOperazioneDB.Scrittura
                        End If
                    Else
                        sa_nome = sa_nome_ret
                        Chiave_Podere = "#"
                        TipOperazione_Centro = enum_TipoOperazioneDB.Lettura
                    End If

                    Dim TitoloPossesso As Integer = 1

                    Dim c_indirizzo As String = "via"
                    Dim c_frazione As String = "#"
                    Dim c_cap As String = "#"
                    Dim c_comune As String = loca
                    Dim c_provincia As String = "#"
                    Dim c_note_indirizzo As String = ""
                    Dim c_codice_istat_comune As String = CInt(Rows_Centri(rigacentro).Item("COMU")).ToString.Trim.PadLeft(3, "0")
                    Dim c_codice_istat_provincia As String = CInt(Rows_Centri(rigacentro).Item("PROV")).ToString.Trim.PadLeft(3, "0")

                    Dim Xml_CentroAziendale As System.Xml.XmlElement
                    Xml_CentroAziendale = objXmlAnagrafe.Xml_Pubblico_CentroAziendale(TipOperazione_Centro, _
                                     Sa_Cod, _
                                     sa_nome, _
                                     "#", _
                                     "#", _
                                     "#", _
                                     "#", _
                                     "#", _
                                     "#", _
                                     "#", _
                                     c_indirizzo, _
                                     c_frazione, _
                                     c_cap, _
                                     c_comune, _
                                     c_provincia, _
                                     "#", _
                                     c_note_indirizzo, _
                                     c_codice_istat_comune, _
                                     c_codice_istat_provincia, _
                                      "#", "#", _
                                     "#", "#", _
                                     "#", "#", _
                                     "#", "#", _
                                     "#", _
                                     Chiave_Podere, _
                                        XmlDoc)


                    If rigacentro = 0 AndAlso TipOperazione_Centro = 1 Then

                        'se non ci sono centri creo il magazzino
                        Dim centri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
                        Dim dtca As DataTable = centri.Leggi(Piva, 0, _
                                                             AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objparametriserver)

                        If dtca.Rows.Count = 0 Then
                            Dim Chiave_Magazzino As String = Chiave_Podere & "\" & 1
                            Dim Xml_Fabbricato As System.Xml.XmlElement
                            Xml_Fabbricato = objXmlAnagrafe.Xml_Pubblico_Fabbricato(1, _
                                                        "0", _
                                                        "Magazzino Aziendale", _
                                                        "20", _
                                                        "#", _
                                                        "#", _
                                                        "#", _
                                                        "#", _
                                                        "#", _
                                                        "#", _
                                                        "#", _
                                                        "#", _
                                                        "#", _
                                                        "#", _
                                                        "#", _
                                                        "#", _
                                                        "#", _
                                                        "#", _
                                                        "#", _
                                                        "#", _
                                                        "#", _
                                                        Chiave_Magazzino, _
                                                       XmlDoc)


                            Xml_CentroAziendale.AppendChild(Xml_Fabbricato)
                        End If

                    End If


                    'Inserisco tutte le particelle con se sup sommando le inita vitate
                    Dim Dt_ParticelleCentr As DataTable = view_CatastoSoci_Tutti.ToTable(True, {"CUAA", "ANNO", "GRUP", "CONT", "VARI", "PROV", "COMU", "SEZI", "FOGL", "PART", "SUBA", "LOCA", "S_CADI", "S_COLT", "S_AMME"})
                    Dim Rows_ParticelleCentro As DataRow() = Dt_ParticelleCentr.Select("CUAA= '" & Rows_Centri(rigacentro).Item("CUAA") & "' AND ANNO=" & Rows_Centri(rigacentro).Item("ANNO") & _
                                                                         " AND LOCA= '" & Rows_Centri(rigacentro).Item("LOCA") & "'  ")
                    For ipc = 0 To Rows_ParticelleCentro.Count - 1

                        Dim GRUP As String = Rows_ParticelleCentro(ipc).Item("GRUP")
                        Dim CONT As String = Rows_ParticelleCentro(ipc).Item("CONT")
                        Dim VARI As String = Rows_ParticelleCentro(ipc).Item("VARI")
                        Dim PROV As String = CInt(Rows_ParticelleCentro(ipc).Item("PROV")).ToString.Trim.PadLeft(3, "0")
                        Dim COMU As String = CInt(Rows_ParticelleCentro(ipc).Item("COMU")).ToString.Trim.PadLeft(3, "0")
                        Dim SEZI As String = ""
                        If Not IsDBNull(Rows_ParticelleCentro(ipc).Item("SEZI")) Then
                            SEZI = Rows_ParticelleCentro(ipc).Item("SEZI")
                        End If

                        Dim FOGL As String = ""
                        If Not IsDBNull(Rows_ParticelleCentro(ipc).Item("FOGL")) Then
                            FOGL = Rows_ParticelleCentro(ipc).Item("FOGL")
                        End If

                        Dim PART As String = CInt(Rows_ParticelleCentro(ipc).Item("PART")).ToString
                        Dim SUBA As String = ""
                        If Not IsDBNull(Rows_ParticelleCentro(ipc).Item("SUBA")) Then
                            SUBA = Rows_ParticelleCentro(ipc).Item("SUBA")
                        End If

                        'Dim LOCA As String
                        Dim S_CADI As Double = Rows_ParticelleCentro(ipc).Item("S_CADI")
                        Dim S_COLT As Double = Rows_ParticelleCentro(ipc).Item("S_COLT")
                        Dim S_AMME As Double = Rows_ParticelleCentro(ipc).Item("S_AMME")

                        Dim supPartToto As Double = objXmlAnagrafe.Ettari_from_EttariAreCentiare(0, 0, S_CADI)
                        Dim supPartColt As Double = objXmlAnagrafe.Ettari_from_EttariAreCentiare(0, 0, S_COLT)
                        Dim supPartAmm As Double = objXmlAnagrafe.Ettari_from_EttariAreCentiare(0, 0, S_AMME)


                        Dim ettari, are, centiare As Double
                        objXmlAnagrafe.EttariAreCentiare_from_Ettari(supPartToto, ettari, are, centiare)


                        Dim TipOperazione_Particella As Integer

                        Dim pa As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R

                        If Sa_Cod = 0 Then
                            TipOperazione_Particella = 1
                        Else
                            Dim dtp As Boolean = pa.Esiste_CentroxParticella(Piva, Sa_Cod, PROV, COMU, SEZI, FOGL, PART, SUBA, "", "", objparametriserver)
                            If dtp Then
                                TipOperazione_Particella = 2
                            Else
                                TipOperazione_Particella = 1
                            End If
                        End If


                        Dim xml_centriparticella As System.Xml.XmlElement
                        xml_centriparticella = objXmlAnagrafe.Xml_Pubblico_Particella( _
                            TipOperazione_Particella, _
                                             "0", _
                                             COMU, _
                                             PROV, _
                                             SEZI, _
                                             FOGL, _
                                             PART, _
                                             SUBA, _
                                             "#", _
                                             ettari, are, centiare, "#", "#", "#", "#", "#", supPartColt, _
                                             AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO, _
                                             AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAFINE, _
                                             XmlDoc)

                        Xml_CentroAziendale.AppendChild(xml_centriparticella)

                    Next




                    Xml_Impresa.AppendChild(Xml_CentroAziendale)

                Next


                Xml_Utente.AppendChild(Xml_Impresa)

                XmlDoc.AppendChild(Xml_Utente)
                Dim _Password As String = New AgronicaCoreUtentiDAL.Utenti_Read().Password_From_UserName(objparametriserver.SuperUserUsername, objparametriutenti)

                Dim risp As Boolean = AvviaImportazione(Piva, Chiave_Cliente, XmlDoc, _
                                             LinkWSImportaGIAS, _
                                    objparametriserver.SuperUserUsername, _
                                    _Password, _
                                    objparametriserver.PivaSuperUser, _
                                    LogDirectory, LogErroriFileName, objparametriserver, objparametriutenti)

                If Not risp Then
                    Log("", LogDirectory, LogErroriFileName, objparametriserver, "Impresa " & Piva & " NON Importata")

                Else
                    Log("", LogDirectory, LogErroriFileName, objparametriserver, "Impresa " & Piva & " Importata")
                End If



            End If



        Next


    End Sub



    Function leggiTabelle( _
                                        ByVal ID_Socio_Filtro As String, _
                                        ByVal Global_CarattereSeparatore As String, _
                                        ByVal Directoryfile As String, _
                                        ByVal File_CatastoSoci As String, _
                                        ByVal Anno As Integer, _
                                         ByRef Dt_CatastoSoci As DataTable, _
                                        ByVal LogDirectory As String, _
                                        ByVal LogFileName As String, _
                                        ByRef objparametriserver As AgronicaCoreDataProvider.AgronicaCoreParametri)

        'Dim Dt_Fornitori As New DataTable
        'Dim Dt_Poderi As New DataTable
        'Dim Dt_Impianti As New DataTable
        'Dim Dt_Particelle As New DataTable
        'Dim Dt_ImpiantixParticelle As New DataTable

        'Leggo i File
        Crea_Dt(Directoryfile, ID_Socio_Filtro, Dt_CatastoSoci, File_CatastoSoci, Anno, Global_CarattereSeparatore, LogDirectory, LogFileName, objparametriserver)


    End Function

    Private Sub Crea_Dt(ByVal Directoryfile As String, _
                        ID_Socio_Filtro As String, ByRef Dt As DataTable, File As String, Anno As Integer, Global_CarattereSeparatore As String, LogDirectory As String, LogFileName As String, objparametriserver As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim NOME_FOGLIO_EXCEL As String = "Foglio1" 'File.Replace(".xls", "") 'file
        Dim importazioneDaFile As New AgronicaExcel_DataProvider.ImportazioneDaExcel
        Dim provider As String = ""

        If Environment.Is64BitProcess Then
            provider = "PROVIDER=Microsoft.ACE.OLEDB.12.0"
        Else
            provider = "PROVIDER=Microsoft.Jet.OLEDB.4.0"
        End If

        Try

            Dim queryExcel As String
            Dim ConnectionStringExcel As String

            queryExcel = " SELECT * FROM [" & NOME_FOGLIO_EXCEL & "$] "

            ConnectionStringExcel = provider & ";Data Source=" & Directoryfile & "\" & File & ";Extended Properties=""EXCEL 8.0;HDR=YES; IMEX=1"";"

            Dt = importazioneDaFile.FillTableFromExcel(queryExcel, ConnectionStringExcel)

        Catch ex As Exception

        End Try

    End Sub



    '########################################################################################
    Private Shared Function AvviaImportazione(ByVal piva As String, _
                                              ByVal Chiave_Cliente As Integer, _
                                         ByVal Documento As System.Xml.XmlDocument, _
                                         ByVal LinkWSImportaGIAS As String, _
                                         ByVal Username As String, _
                                         ByVal Password As String, _
                                         ByVal PivaSuperUser As String, _
                                              ByVal LogDirectory As String, _
                                              ByVal LogFileName As String, _
                                              ByRef objparametriserver As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                               ByRef objparametriutenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim Documento_Finale As New System.Xml.XmlDocument

        Dim XML_Risultato As System.Xml.XmlElement
        Dim XML_Risposta As System.Xml.XmlElement
        Dim XMLs_Risposta As System.Xml.XmlNodeList
        Dim XML_Errore As System.Xml.XmlElement
        Dim XMLs_Errore As System.Xml.XmlNodeList

        Dim StrFinale As String


        Dim x As Integer

        Try




            If LinkWSImportaGIAS <> "" Then

                Dim ws_importatore As New Ws_Importa_Gias.ImportaWS

                ws_importatore.Url = "http://" & LinkWSImportaGIAS & "/WS_Importa_GIAS/ImportaWS.asmx"

                ws_importatore.Timeout = Integer.MaxValue

                Dim objcoreXML As New AgronicaCoreXML.XML_WS_Importa_Gias
                Dim strCredenziali As String
                strCredenziali = objcoreXML.Genera_Stringa_Credenziali(True, Nothing, Username, _
                                                                       Password, _
                                                                       PivaSuperUser, _
                                                                       True, _
                                                                       "", "", "", "", "", "", _
                                                                       objparametriserver.StringaConnessione, _
                                                                       objparametriutenti.StringaConnessione)

                StrFinale = ws_importatore.Importa_DocumentoPubblico_SuperServer(strCredenziali, Documento.OuterXml, CInt(Chiave_Cliente))


                Documento_Finale.LoadXml(StrFinale)

                XML_Risultato = Documento_Finale.SelectSingleNode("Risultato")
                Dim Str_Risultato As String

                If XML_Risultato.HasAttribute("errore") Then
                    Str_Risultato &= "Sincro anagrafe ERRORE: " & XML_Risultato.GetAttribute("errore").ToString + vbCrLf

                    Log("", LogDirectory, LogFileName, objparametriserver, "Piva: " & piva & " - " & Str_Risultato)
                    Return False
                Else

                    XMLs_Risposta = XML_Risultato.GetElementsByTagName("Risposta")

                    Dim strRisp As String = String.Empty

                    For x = 0 To XMLs_Risposta.Count - 1

                        XML_Risposta = XMLs_Risposta.Item(x)

                        strRisp = XML_Risposta.GetAttribute("Ris")

                        Str_Risultato &= "- " & strRisp & IIf(InStr(strRisp, "Errore"), "", " - Terminata correttamente") & "<br/>"


                    Next

                    Log("", LogDirectory, LogFileName, objparametriserver, "Piva: " & piva & " - " & Str_Risultato)
                    Return True

                End If





            Else

                MsgBox("Inserire l'indirizzo del Web Service Gias per continuare.", MsgBoxStyle.Information, "Sincronizzatore Universale Gias")

            End If

        Catch ex As Exception

            MsgBox(ex.Message, MsgBoxStyle.Information)

        End Try

        Return False

    End Function


    Private Shared Sub Log(ByVal piva As String, ByVal LogDirectory As String, LogFileName As String, objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri, Messaggio As String)

        If LogDirectory <> "" AndAlso LogFileName <> "" Then

            Dim objLog As New AgronicaCoreDataProvider.LogProvider

            Dim Msg1 As String = "Piva: " & piva & " "
            Dim Msg As String = "- Messaggio: " & Messaggio & " "
            ' Msg &= "- Timestamp: " & data & " "

            Dim customLOGParams As New CustomLOGParams With {
                .LogDescrizioneUtente = objParametriServer.UtenteUsername,
                .LogDirectory = LogDirectory,
                .LogFileName = LogFileName
            }

            objLog.Scrivi_LOG(objParametriServer,
                       Msg1,
                       Messaggio,
                       CustomLOGParams:=customLOGParams)

        End If

    End Sub







End Class
