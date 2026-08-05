Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports System.Xml

Public Class UDS_CentriForniMacchine

    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim ASG_ProgressivoGIAS As Integer


    Sub New(ByVal objparametriserver As AgronicaCoreParametri, _
            ByVal objparametriutenti As AgronicaCoreParametri, _
            ByVal ASGProgressivoGIAS As Integer)

        objParametri_Server = objparametriserver
        objParametri_Utenti = objparametriutenti
        ASG_ProgressivoGIAS = ASGProgressivoGIAS

    End Sub



    Public Sub ImportaCentriForni()
        Dim dp As New AgronicaCoreDataProvider.DataProvider
        Dim BaseCode, TopCode As Integer
        Call Calcola_BaseCode_TopCode(BaseCode, _
              TopCode, _
              ASG_ProgressivoGIAS)

        Dim strsql As String = " Select * from __ImportForni order by id "
        Dim dt As DataTable = dp.EseguiQuery_Lettura(objParametri_Server, strsql, "OptaImport")

        For i = 0 To dt.Rows.Count - 1

            Dim identificativo As String = dt.Rows(i).Item("Identificativo").ToString.Trim
            If identificativo <> "" Then

                Dim Cuaa As String = identificativo.Split("|")(0).Trim

                Dim objCuaa As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
                Dim piva As String = objCuaa.Piva_from_IdCodValCod(CA_CUAA, Cuaa, objParametri_Server)
                If Not IsNumeric(piva) Or (piva = "") Then
                    Throw New Exception("piva non trovata")
                End If

                If IsNumeric(Cuaa) AndAlso (Cuaa <> piva) Then
                    Throw New Exception("Qualcosa non va")
                End If

                Dim idCentro As String = ""
                Dim Centro As String = ""
                If identificativo.Split("|").Count = 2 Then
                    idCentro = identificativo.Split("|")(1).Trim.PadLeft(3, "0")
                    Centro = dt.Rows(i).Item("Centro").ToString.Trim
                End If
                If identificativo.Split("|").Count = 1 Then
                    idCentro = "UDS"
                    Centro = "Centro di Essiccazione Aziendale" ' dt.Rows(i).Item("Centro").ToString.Trim 'se aziendale c'è la rag sociale, al massimo se lo cambiano dopo
                End If
                If identificativo.Split("|").Count > 2 Then
                    Throw New Exception()
                End If

                Dim Citta As String = dt.Rows(i).Item("Citta").ToString.Trim
                Dim Prov As String = dt.Rows(i).Item("Prov").ToString.Trim
                Dim Cap As String = dt.Rows(i).Item("Cap").ToString.Trim
                Dim Indirizzo As String = dt.Rows(i).Item("Indirizzo").ToString.Trim

                Dim Numero As String = dt.Rows(i).Item("Numero").ToString.Trim.PadLeft(3, "0")
                Numero = Numero.Replace("°", ".")

                Dim Tipo As String = dt.Rows(i).Item("Tipo").ToString.Trim

                Dim Combustibile As String = ""
                If dt.Rows(i).Item("Gaso").ToString.Trim.ToLower = "x" Then
                    Combustibile = "Gaso"
                End If
                If dt.Rows(i).Item("Met").ToString.Trim.ToLower = "x" Then
                    Combustibile = "Met"
                End If
                If dt.Rows(i).Item("Gas").ToString.Trim.ToLower = "x" Then
                    Combustibile = "Gas"
                End If
                If dt.Rows(i).Item("Altro").ToString.Trim.ToLower = "x" Then
                    Combustibile = "Altro"
                End If

                Dim Fiamma As String = ""
                If dt.Rows(i).Item("Diretta").ToString.Trim.ToLower = "x" Then
                    Fiamma = "Diretta"
                End If
                If dt.Rows(i).Item("Indiretta").ToString.Trim.ToLower = "x" Then
                    Fiamma = "Indiretta"
                End If
                If dt.Rows(i).Item("Recup").ToString.Trim.ToLower = "x" Then
                    Fiamma = "Recup"
                End If


                Dim Cantiere As String = ""
                If dt.Rows(i).Item("Cassoni").ToString.Trim.ToLower = "x" Then
                    Cantiere = "Cassoni"
                End If
                If dt.Rows(i).Item("Telaini").ToString.Trim.ToLower = "x" Then
                    Cantiere = "Telaini"
                End If


                Dim Umidificazione As String = ""
                If dt.Rows(i).Item("Acqua").ToString.Trim.ToLower = "x" Then
                    Umidificazione = "Acqua"
                End If
                If dt.Rows(i).Item("Vapore").ToString.Trim.ToLower = "x" Then
                    Umidificazione = "Vapore"
                End If


                'Centro di essicazione crera o leggi
                Dim sa_cod As Integer = GetSa_cod(BaseCode, TopCode, piva, idCentro, Centro, Citta, Prov, Cap, Indirizzo)


                'Forno nel centro
                Dim Fabbricato_cod As Integer = GetFabbricato_cod(BaseCode, TopCode, piva, sa_cod, Numero, Tipo, Combustibile, Fiamma, Cantiere, Umidificazione, Centro, Citta, Prov, Cap, Indirizzo)

                Dim salta As Boolean = False


                aggiungiAggiornaCodiceFabbricato(piva, sa_cod, Fabbricato_cod, _
                                                           enum_CodiciAnagrafe.Fabbricato_Forno_Tipo, Tipo)



                Dim Comb As enum_Fabbricato_Forno_Combustibile
                Select Case Combustibile
                    Case "Gaso"
                        Comb = enum_Fabbricato_Forno_Combustibile.Gasolio
                    Case "Met"
                        Comb = enum_Fabbricato_Forno_Combustibile.Metano
                    Case "Gas"
                        Comb = enum_Fabbricato_Forno_Combustibile.Gas
                    Case "Cippato"
                        Comb = enum_Fabbricato_Forno_Combustibile.Cippato
                    Case "Altro"
                        Comb = enum_Fabbricato_Forno_Combustibile.Altro
                    Case Else
                        If dt.Rows(i).Item("Gaso").ToString.Trim.ToLower.Contains("ferm") Then
                            'nanno scritto fermo nel forno, salto i codici
                            salta = True
                        Else
                            Throw New Exception
                        End If
                End Select

                If salta Then
                    If Fabbricato_cod = 0 Or sa_cod = 0 Then
                        Throw New Exception
                    End If
                    Dim Fabbr_W As New AgronicaCoreAnagrafeDAL.Fabbricati_W
                    Fabbr_W.AggiornaValiditaFine(piva, sa_cod, Fabbricato_cod, "31/12/2013", "", objParametri_Server)
                    Dim Fabbr_r As New AgronicaCoreAnagrafeDAL.Fabbricati_R
                    Dim des As String = AgronicaCoreAnagrafeDAL.Fabbricati_R.FabbricatoDes_from_FabbricatoCod(piva, sa_cod, Fabbricato_cod, objParametri_Server)
                    Fabbr_W.AggiornaFabbricato_Des(piva, sa_cod, Fabbricato_cod, des & " (FERMO)", "", objParametri_Server)
                Else

                    aggiungiAggiornaCodiceFabbricato(piva, sa_cod, Fabbricato_cod, _
                                 enum_CodiciAnagrafe.Fabbricato_Forno_Combustibile, Comb.ToString)


                    Dim Fiam As enum_Fabbricato_Forno_Fiamma
                    Select Case Fiamma
                        Case "Diretta"
                            Fiam = enum_Fabbricato_Forno_Fiamma.Diretta
                        Case "Indiretta"
                            Fiam = enum_Fabbricato_Forno_Fiamma.Indiretta
                        Case "Recup"
                            Fiam = enum_Fabbricato_Forno_Fiamma.Recupero
                        Case Else
                            Throw New Exception
                    End Select
                    aggiungiAggiornaCodiceFabbricato(piva, sa_cod, Fabbricato_cod, _
                                                     enum_CodiciAnagrafe.Fabbricato_Forno_Fiamma, Fiam.ToString)


                    Dim Cant As enum_Fabbricato_Forno_Cantiere
                    Select Case Cantiere
                        Case "Cassoni"
                            Cant = enum_Fabbricato_Forno_Cantiere.Cassone
                        Case "Telaini"
                            Cant = enum_Fabbricato_Forno_Cantiere.Telaini
                        Case Else
                            Throw New Exception
                    End Select
                    aggiungiAggiornaCodiceFabbricato(piva, sa_cod, Fabbricato_cod, _
                                                     enum_CodiciAnagrafe.Fabbricato_Forno_Cantiere, Cant.ToString)


                    Dim Umi As enum_Fabbricato_Forno_Umidificazione
                    Select Case Umidificazione
                        Case "Acqua"
                            Umi = enum_Fabbricato_Forno_Umidificazione.Acqua
                        Case "Vapore"
                            Umi = enum_Fabbricato_Forno_Umidificazione.Vapore
                        Case Else
                            Throw New Exception
                    End Select
                    aggiungiAggiornaCodiceFabbricato(piva, sa_cod, Fabbricato_cod, _
                                                     enum_CodiciAnagrafe.Fabbricato_Forno_Umidificazione, Umi.ToString)


                End If




            End If


        Next
    End Sub



    Public Sub ImportaMacchine()
        Dim dp As New AgronicaCoreDataProvider.DataProvider
        Dim strsql As String = " Select * from __ImportMacchine  order by id "
        Dim dt As DataTable = dp.EseguiQuery_Lettura(objParametri_Server, strsql, "OptaImport")

        For i = 0 To dt.Rows.Count - 1

            Dim identificativo As String = dt.Rows(i).Item("Identificativo").ToString.Trim

            If identificativo <> "" Then
                Dim Marca As String = dt.Rows(i).Item("Marca").ToString.Trim

                Dim ok As Boolean = False
                Select Case Marca.ToLower
                    Case "Verde".ToLower
                    Case "Cernita".ToLower
                    Case "Magazzino".ToLower
                    Case "Forni".ToLower
                    Case "Serre".ToLower
                    Case "ZONE".ToLower
                    Case Else
                        ok = True
                End Select

                If ok Then


                    Dim Cuaa As String = identificativo.Split("|")(0).Trim

                    Dim objCuaa As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
                    Dim piva As String = objCuaa.Piva_from_IdCodValCod(CA_CUAA, Cuaa, objParametri_Server)
                    If Not IsNumeric(piva) Or (piva = "") Then
                        Throw New Exception("piva non trovata")
                    End If

                    If IsNumeric(Cuaa) AndAlso (Cuaa <> piva) Then
                        Throw New Exception("Qualcosa non va")
                    End If


                    Dim idCentro As String = ""
                    Dim Centro As String = ""
                    If identificativo.Split("|").Count = 2 Then
                        idCentro = identificativo.Split("|")(1).Trim.PadLeft(3, "0")
                        Centro = dt.Rows(i).Item("Centro").ToString.Trim
                    End If

                    If identificativo.Split("|").Count = 1 Then
                        idCentro = "UDS"
                        Centro = "Centro di Essiccazione Aziendale" ' dt.Rows(i).Item("Centro").ToString.Trim 'se aziendale c'è la rag sociale, al massimo se lo cambiano dopo
                        'Seunsolocentroallorasalvolamacchinacomeaziendale o no? 
                    End If
                    If identificativo.Split("|").Count > 2 Then
                        Throw New Exception()
                    End If

                    Dim TipoMacchina As String = dt.Rows(i).Item("TipoMacchina").ToString.Trim

                    Dim tipo As String = TipoMacchina.Split(" ")(0).Trim
                    Dim descrizMacchina As String = ""
                    If TipoMacchina.Split(" ").Length > 1 Then
                        descrizMacchina = TipoMacchina.Replace(tipo, "").Trim
                    Else
                        descrizMacchina = "N.1"
                    End If
                    descrizMacchina = descrizMacchina.Replace("°", ".")


                    Dim Modello As String = dt.Rows(i).Item("Modello").ToString.Trim
                    Modello = Modello.Replace("°", ".")


                    If Marca.ToLower = "Carbazzoli".ToLower Or Marca.ToLower = "Car Bazzoli".ToLower Or Marca.ToLower = "Bazzoli".ToLower Or Marca.ToLower = "ICARBazzoli".ToLower Or Marca.ToLower = "ICAR Bazzoli".ToLower Or Marca.ToLower = "I.C.A.R. Bazzoli".ToLower Or Marca.ToLower = "I.C.A.R.Bazzoli".ToLower Then
                        Marca = "I.C.A.R. DI BAZZOLI"
                    End If

                    If Marca.ToLower = "OMIU".ToLower Then
                        Marca = "OMNIUM"
                    End If

                    If Marca.ToLower = "OMNIU".ToLower Then
                        Marca = "OMNIUM"
                    End If

                    If Marca.ToLower = "AYSTER".ToLower Then
                        Marca = "Hyster"
                    End If

                    If Marca.ToLower = "cat".ToLower Then
                        Marca = "CATERPILLAR"
                    End If

                    If Marca.ToLower = "still".ToLower Then
                        Marca = "OM" 'sarebbe 'om still'
                    End If

                    If Marca.ToLower = "DeCloet".ToLower Then
                        Marca = "DE CLOET SRL"
                    End If

                    If Marca.ToLower = "De Cloet".ToLower Then
                        Marca = "DE CLOET SRL"
                    End If

                    If Marca.ToLower = "DeClot".ToLower Then
                        Marca = "DE CLOET SRL"
                    End If


                    If Marca.ToLower = "Mencagli".ToLower Then
                        Marca = "MAC s.n.c. DI MENCAGLI ANGELO & C."
                    End If


                    If Marca.ToLower = "terrmacch".ToLower Then
                        Marca = "Terrmacch s.r.l."
                    End If

                    If Marca.ToLower = "Termacch".ToLower Then
                        Marca = "Terrmacch s.r.l."
                    End If

                    If Marca.ToLower = "coman".ToLower Then
                        Marca = "Coman Group"
                    End If

                    If Marca.ToLower = "COMAZZU".ToLower Then
                        Marca = "Komatsu"
                    End If

                    If Marca.ToLower = "ANNOVI".ToLower Then
                        Marca = "ANNOVI F.LLI"
                    End If

                    If Marca.ToLower = "Bosco".ToLower Then
                        Marca = "MAGNABOSCO"
                    End If

                    If Marca.ToLower = "MANITOU".ToLower Then
                        Marca = "MANITOU BF"
                    End If

                    If Marca.ToLower = "MANITU".ToLower Then
                        Marca = "MANITOU BF"
                    End If

                    If Marca.ToLower = "Camec".ToLower Then
                        Marca = "Camec Mechanics"
                    End If


                    If Marca.ToLower = "FIN CAVE".ToLower Then
                        Marca = "Fin.Cave s.p.a."
                    End If

                    If Marca.ToLower = "Calderini".ToLower Then
                        Marca = "" 'non c'è
                    End If

                    If Marca.ToLower = "Artigianale".ToLower Then
                        Marca = "" 'non c'è
                    End If

                    If Marca.ToLower = "NO".ToLower Then
                        Marca = "" 'non c'è
                    End If


                    Dim Ditta_Cod As Integer = 0
                    If Marca <> "" Then
                        Dim dt2 As DataTable = New AgronicaCoreMetaSchemaDAL.Ditte_R().Leggi(0, "M", "(  Ditta_Des like '" & Marca & "' )", "", objParametri_Server)
                        If dt2.Rows.Count <> 1 Then
                            Throw New Exception()
                        Else
                            Ditta_Cod = dt2.Rows(0).Item("Ditta_Cod")
                        End If
                    End If

                    Select Case tipo.ToLower
                        Case "cimatrice".ToLower
                            tipo = "cimatrice"
                        Case "raccolta".ToLower, "Raccogitrice".ToLower, "Raccoglitrice".ToLower
                            tipo = "Macchine per la raccolta del Tabacco"
                        Case "trattore".ToLower
                            tipo = "trattori a ruote"
                        Case "cernita".ToLower
                            tipo = "Macchina di Cernita"
                        Case "trattamenti".ToLower
                            tipo = "Macchine per la difesa chimica"

                    End Select

                    Dim Class_Code As String = New AgronicaCoreMetaSchemaDAL.Macchine_R().MacchinaCODE_from_MacchinaDESC(tipo, objParametri_Server)



                    Dim salvaComeAziendaliMacchineDaCampo As Boolean = False
                    If identificativo.Split("|").Count = 1 Then
                        Select Case Class_Code
                            Case "18.01", "18.02", "18.03", "18.04", "18.05", "18.06", "10.07"
                                salvaComeAziendaliMacchineDaCampo = False 'lavorazione tabacco
                            Case Else
                                salvaComeAziendaliMacchineDaCampo = True 'trattori etc
                        End Select
                    End If

                    Dim sa_cod As Integer = 0

                    If salvaComeAziendaliMacchineDaCampo Then
                        sa_cod = 0
                    Else
                        Dim CentriAziendali_Read As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
                        Dim filtro As String = " (sa_nome like '" & idCentro & " - %' ) "
                        Dim dtcentri As DataTable = CentriAziendali_Read.Leggi(piva, 0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
                                                                            filtro, "", objParametri_Server)

                        If dtcentri.Rows.Count <> 1 Then
                            Throw New Exception()
                        End If
                        sa_cod = dtcentri.Rows(0).Item("Sa_Cod")
                    End If


                    Dim maccod As Integer = New AgronicaCoreAnagrafeBIZ.Importa_GIAS().Crea_Macchina_Semplificata( _
                        piva, sa_cod, Class_Code, Ditta_Cod, Modello, descrizMacchina, objParametri_Server)

                End If
            End If


        Next
    End Sub



    Private Function GetSa_cod(ByVal BaseCode As Integer, ByVal TopCode As Integer, ByVal piva As String, ByVal idCentro As String, ByVal Centro As String, ByVal Citta As String, ByVal Prov As String, ByVal Cap As String, ByVal Indirizzo As String) As Integer
        Dim sa_cod As Integer
        Dim CentriAziendali_Read As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
        Dim filtro As String = " (sa_nome like '" & idCentro & " - %' ) "
        Dim dtcentri As DataTable = CentriAziendali_Read.Leggi(piva, 0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
                                                            filtro, "", objParametri_Server)

        If dtcentri.Rows.Count > 1 Then
            Throw New Exception()
        End If


        If dtcentri.Rows.Count = 1 Then
            sa_cod = dtcentri.Rows(0).Item("Sa_Cod")
        Else
            'scrivo nuovo centro
            Dim sa_nome As String = idCentro & " - " & Centro
            Dim AnagrafeXML As New AgronicaCoreXML.XML_Anagrafe
            Dim XmlDoc As New XmlDocument
            Dim log As String
            sa_cod = 0

            Dim objIstat As New AgronicaCoreMetaSchemaDAL.Istat_R
            Dim ProIstat, ComIstat As String
            objIstat.CodIstat_from_SiglaProvincia_and_StringaComune(Prov, Citta, "", ProIstat, ComIstat, objParametri_Server)


            Dim Xml_CentroAziendale As XmlElement = AnagrafeXML.XML_2_CentriAziendali(log, _
                                                                            XmlDoc, _
                                                                            BaseCode, _
                                                                            TopCode, _
                                                                            enum_TipoOperazioneDB.Scrittura, _
                                                                            piva, _
                                                                            sa_cod, _
                                                                            sa_nome, _
                                                                            1, _
                                                                            ProIstat, _
                                                                            ComIstat, _
                                                                            0, _
                                                                            Indirizzo, _
                                                                            "", _
                                                                            Cap, _
                                                                            "IT", _
                                                                            "", _
                                                                            Nothing, _
                                                                            Nothing, _
                                                                            , , , , , , , , , _
                                                                             , _
                                                                            enum_TipoCentro.Stabilimento, _
                                                                            , , , , , , , , _
                                                                            AGRODATAINIZIO, _
                                                                            AGRODATAFINE)


            Dim CentroAziendale_W As New AgronicaCoreAnagrafeBIZ.CentroAziendale_W
            Dim OUTPUT_Piva As String
            CentroAziendale_W.CentroAziendale_Scrivi(Xml_CentroAziendale.OuterXml, OUTPUT_Piva, sa_cod, objParametri_Server, objParametri_Utenti)


            If OUTPUT_Piva <> piva Then
                Throw New Exception
            End If
            If sa_cod = 0 Then
                Throw New Exception
            End If


        End If
        Return sa_cod
    End Function



    Private Function GetFabbricato_cod(BaseCode As Integer, TopCode As Integer, piva As String, sa_cod As Integer, Numero As String, Tipo As String, Combustibile As String, Fiamma As String, Cantiere As String, Umidificazione As String, Centro As String, Citta As String, Prov As String, Cap As String, Indirizzo As String) As Integer


        Dim Fabbricato_cod As Integer = 0
        Dim Fabbricati_R As New AgronicaCoreAnagrafeDAL.Fabbricati_R
        Dim filtro As String = " (Fabbricato_Des like '" & Numero & " - %' ) "
        Dim dt As DataTable = Fabbricati_R.Leggi(piva, sa_cod, 0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
                                                             filtro, "", objParametri_Server)

        If dt.Rows.Count > 1 Then
            Throw New Exception()
        End If


        If dt.Rows.Count = 1 Then
            Fabbricato_cod = dt.Rows(0).Item("Fabbricato_cod")
        Else
            'scrivo nuovo centro
            Dim Fabbricato_Des As String = Numero & " - " & Tipo
            Dim AnagrafeXML As New AgronicaCoreXML.XML_Anagrafe
            Dim XmlDoc As New XmlDocument
            Dim log As String

            Dim objIstat As New AgronicaCoreMetaSchemaDAL.Istat_R
            Dim ProIstat, ComIstat As String
            objIstat.CodIstat_from_SiglaProvincia_and_StringaComune(Prov, Citta, "", ProIstat, ComIstat, objParametri_Server)

            Dim tipoFabbricato As TipiEnumerativi.enum_FabbricatiTipi
            tipoFabbricato = TipiEnumerativi.enum_FabbricatiTipi.essiccatoio

            Dim Xml_fabbricato As XmlElement = AnagrafeXML.XML_2_Fabbricati(log, _
                                                                            XmlDoc, _
                                                                            BaseCode, _
                                                                            TopCode, _
                                                                            enum_TipoOperazioneDB.Scrittura, _
                                                                            piva, _
                                                                            sa_cod, _
                                                                            Fabbricato_cod, _
                                                                            Fabbricato_Des, _
                                                                            tipoFabbricato, _
                                                                            1, _
                                                                            ProIstat, _
                                                                            ComIstat, _
                                                                            0, _
                                                                            Indirizzo, _
                                                                            "", _
                                                                            Cap, _
                                                                            "IT", _
                                                                            "", _
                                                                            Nothing, _
                                                                            Nothing, _
                                                                            Nothing, Nothing, Nothing)


            Dim Fabbricato_W As New AgronicaCoreAnagrafeBIZ.Fabbricato_W
            Dim OUTPUT_Piva, OUTPUT_sa_cod As String
            Fabbricato_W.Fabbricato_Scrivi(Xml_fabbricato.OuterXml, OUTPUT_Piva, OUTPUT_sa_cod, Fabbricato_cod, objParametri_Server)


            If OUTPUT_Piva <> piva Then
                Throw New Exception
            End If
            If OUTPUT_sa_cod <> sa_cod.ToString Then
                Throw New Exception
            End If
            If Fabbricato_cod = 0 Then
                Throw New Exception
            End If


        End If
        Return Fabbricato_cod

    End Function



    Private Sub aggiungiAggiornaCodiceFabbricato(PIVA As String, SA_COD As Integer, fabbricato_cod As Integer, enum_CodiciAnagrafe As enum_CodiciAnagrafe, valore As String)
        If PIVA = "" Then
            Throw New Exception("aggiungiAggiornaCodiceFabbricato: PIVA parametro obbligatorio")
        End If
        If enum_CodiciAnagrafe = 0 Then
            Throw New Exception("aggiungiAggiornaCodiceFabbricato: id_cod parametro obbligatorio")
        End If
        If SA_COD = 0 Then
            Throw New Exception("aggiungiAggiornaCodiceFabbricato: sa_cod parametro obbligatorio")
        End If
        If fabbricato_cod = 0 Then
            Throw New Exception("aggiungiAggiornaCodiceFabbricato: fabbricato_cod parametro obbligatorio")
        End If

        valore = valore.Trim

        If valore = "" Then
            Throw New Exception
        End If

        Dim w As New AgronicaCoreAnagrafeDAL.Fabbricati_Codici_W

        w.Cancella(PIVA, SA_COD, fabbricato_cod, enum_CodiciAnagrafe, "", objParametri_Server)
        w.Scrivi(PIVA, SA_COD, fabbricato_cod, enum_CodiciAnagrafe, AGRODATAINIZIO, AGRODATAFINE, valore, objParametri_Server)

    End Sub




End Class
