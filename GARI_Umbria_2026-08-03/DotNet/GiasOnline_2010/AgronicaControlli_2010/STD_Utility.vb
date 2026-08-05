Imports System.Data
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreModelsSTD.metaschema
Imports AgronicaCoreDataProvider
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.metaschema.avversita
Imports AgronicaCoreModelsSTD
Imports AgronicaCoreModelsSTD.attivita.Attivita
Imports AgronicaCoreModelsSTD.exceptions

Public Class STD_Utility

    Public Shared Cop_Cod_Senza_Copertura As New List(Of Integer)({0, 1, 3, 4, 5, 6}) 'antigrandine + nessuna varia    

    Public Shared Function getStato(Piva As String, Sa_Cod_List As List(Of Integer), objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim Stato_Cod As String = ""
        Dim Stato_Cod_Corrente As String

        Dim isFirst As Boolean = True

        Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read

        For Each sa_cod In Sa_Cod_List

            Dim dtAnagrafica As DataTable = objCentri.Leggi_x_anagraficaNG(Piva, sa_cod, "", "", objParametri_Server, False)

            If Not IsNothing(dtAnagrafica) AndAlso dtAnagrafica.Rows.Count > 0 AndAlso Not IsDBNull(dtAnagrafica.Rows(0).Item("Stato_Cod")) Then
                Stato_Cod_Corrente = dtAnagrafica.Rows(0).Item("Stato_Cod")
            Else
                Stato_Cod_Corrente = ""
            End If

            If isFirst Then
                isFirst = False
                Stato_Cod = Stato_Cod_Corrente
            Else
                If Stato_Cod_Corrente <> Stato_Cod Then
                    Throw New GiasException(My.Resources.AgronicaControlli_2010.StatoIncompatibileMulticentro)
                End If
            End If

        Next

        Return Stato_Cod

    End Function

    Public Shared Function getTipoTestata(Lav_Cod As String) As Integer

        Dim Tipo_Testata As Integer = 0

        Select Case Lav_Cod

            Case LAVCOD_RILIEVO_AVVERSITA_CAMPO, LAVCOD_VISITA
                Tipo_Testata = enum_Disciplinare_Tipo_Testata.Difesa

            Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_CONCIA_SEME, LAVCOD_GEODISINFESTAZIONE,
                 LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE, LAVCOD_TRATTAMENTO_POST_RACCOLTA,
                 LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA, LAVCOD_REINNESCO_TRAPPOLE
                Tipo_Testata = enum_Disciplinare_Tipo_Testata.Difesa

            Case LAVCOD_DISSECCAMENTO, LAVCOD_DISERBO
                Tipo_Testata = enum_Disciplinare_Tipo_Testata.Diserbo

            Case LAVCOD_TRATTAMENTO_FITOREGOLATORE
                Tipo_Testata = enum_Disciplinare_Tipo_Testata.Fitoregolatore

            Case LAVCOD_DISTRIBUZIONE_CONCIME, LAVCOD_SARCHIATURA_CONCIMAZIONE, LAVCOD_DISTRIBUZIONE_AMMENDANTI, LAVCOD_CONCIMAZIONE_FOGLIARE, LAVCOD_FERTIRRIGAZIONE, LAVCOD_TRATTAMENTO_ANTIBUTTERATURA
                Tipo_Testata = enum_Disciplinare_Tipo_Testata.Fertilizzazione

        End Select

        Return Tipo_Testata
    End Function

    Public Shared Function getTipoOperazione(tipoAttivita As attivita.Attivita.Tipo_Attivita, statoAttivita As attivita.Attivita.Stati) As enum_Tipo_Operazione_Agenda

        Dim Tipo_Operazione_Agenda As enum_Tipo_Operazione_Agenda

        Select Case tipoAttivita
            Case AgronicaCoreModelsSTD.attivita.Attivita.Tipo_Attivita.QuadernoDiCampagna
                Tipo_Operazione_Agenda = enum_Tipo_Operazione_Agenda.QuadernoDiCampagna

            Case AgronicaCoreModelsSTD.attivita.Attivita.Tipo_Attivita.Ricetta
                If statoAttivita = AgronicaCoreModelsSTD.attivita.Attivita.Stati.Eseguita Then
                    Tipo_Operazione_Agenda = enum_Tipo_Operazione_Agenda.RicettaBrogliaccio
                Else
                    Tipo_Operazione_Agenda = enum_Tipo_Operazione_Agenda.Ricetta
                End If

        End Select

        Return Tipo_Operazione_Agenda

    End Function

    Public Shared Function getListaComuni(lstImpianti As AgronicaCoreModelsSTD.anagrafiche.Impianto(), objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As List(Of Comune)

        'TODO_DT: valutare performance, se troppo lento valutare query diretta
        Dim ListaComuni As New List(Of AgronicaControlli_2010.Comune)

        Dim objCOM As New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R
        Dim DT As DataTable
        Dim ComunePresente As Boolean = False
        If lstImpianti IsNot Nothing AndAlso lstImpianti.Count > 0 Then

            For Each impianto In lstImpianti
                DT = objCOM.LeggiParticelle_Da_Appezzamento(CStr(impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva),
                                                            CInt(impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice),
                                                            CInt(impianto.primaryKey.appezzamentoPK.codice),
                                                            enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                            "", "", objParametri_Server)

                For Each drParticella In DT.Rows
                    ComunePresente = False
                    Dim CodProvincia = drParticella.Item("Prov")
                    Dim CodComune = drParticella.Item("Com")
                    If CodProvincia <> "" OrElse CodComune <> "" Then
                        For c = 0 To ListaComuni.Count - 1
                            If ListaComuni(c).Prov = CodProvincia And ListaComuni(c).Com = CodComune Then
                                ComunePresente = True
                                Exit For
                            End If
                        Next
                        If ComunePresente = False Then
                            Dim Comune As New AgronicaControlli_2010.Comune
                            Comune.Prov = CodProvincia
                            Comune.Com = CodComune
                            ListaComuni.Add(Comune)
                        End If
                    End If
                Next
            Next
        End If

        'se gli appezzamenti non hanno catasto 
        'prendo gli indirizzi dell'appezzamento
        If ListaComuni.Count = 0 Then
            Dim objAppezzaXIndirizzi As New AgronicaCoreAnagrafeDAL.AppezzamentixIndirizzi_Read

            If lstImpianti IsNot Nothing AndAlso lstImpianti.Count > 0 Then

                Dim lstimpiantidistinctAppezza As New List(Of Impianto)

                For Each impianto In lstImpianti

                    If lstimpiantidistinctAppezza.FindIndex(Function(i) i.primaryKey.appezzamentoPK.codice = impianto.primaryKey.appezzamentoPK.codice AndAlso
                                                                         i.primaryKey.appezzamentoPK.centroAziendalePK.codice = impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice AndAlso
                                                                         i.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva = impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva) = -1 Then

                        lstimpiantidistinctAppezza.Add(impianto)

                    End If
                Next

                If Not IsNothing(lstimpiantidistinctAppezza) AndAlso lstimpiantidistinctAppezza.Count > 0 Then

                    For Each impianto In lstImpianti
                        Dim dtAppezzaXIndirizzi = objAppezzaXIndirizzi.Leggi(impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva, impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice, impianto.primaryKey.appezzamentoPK.codice, 0, "", "", objParametri_Server)

                        If Not IsNothing(dtAppezzaXIndirizzi) AndAlso dtAppezzaXIndirizzi.Rows.Count > 0 Then
                            For Each Indirizzi In dtAppezzaXIndirizzi.Rows
                                Dim CodProvincia = Indirizzi.Item("pro_cod_istat")
                                Dim CodComune = Indirizzi.Item("com_cod_istat")
                                If CodProvincia <> "" OrElse CodComune <> "" Then
                                    For c = 0 To ListaComuni.Count - 1
                                        If ListaComuni(c).Prov = CodProvincia And ListaComuni(c).Com = CodComune Then
                                            ComunePresente = True
                                            Exit For
                                        End If
                                    Next
                                    If ComunePresente = False Then
                                        Dim Comune As New AgronicaControlli_2010.Comune
                                        Comune.Prov = CodProvincia
                                        Comune.Com = CodComune
                                        ListaComuni.Add(Comune)
                                    End If
                                End If
                            Next
                        End If

                    Next

                End If

            End If

        End If


        'se gli appezzamenti non hanno catasto e non hanno neanche un indirizzo con provincia e comune
        'prendo l'indirizzo del centro (o dei centri)
        If ListaComuni.Count = 0 Then
            Dim objCentro As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
            Dim DtCentro As DataTable

            If lstImpianti IsNot Nothing AndAlso lstImpianti.Count > 0 Then
                For Each impianto In lstImpianti
                    Dim centroAziendale As New CentroAziendale(impianto.primaryKey.appezzamentoPK.centroAziendalePK)
                    If centroAziendale IsNot Nothing AndAlso centroAziendale.primaryKey IsNot Nothing Then
                        DtCentro = objCentro.Leggi(centroAziendale.primaryKey.partitaIva, centroAziendale.primaryKey.codice, enumSelezioneVariabile.Selezione_JoinDescrizioni, "", "", objParametri_Server)
                        If DtCentro.Rows.Count > 0 Then
                            ComunePresente = False
                            Dim CodProvincia = DtCentro.Rows(0).Item("pro_cod_istat")
                            Dim CodComune = DtCentro.Rows(0).Item("com_cod_istat")
                            If CodProvincia <> "" OrElse CodComune <> "" Then
                                For c = 0 To ListaComuni.Count - 1
                                    If ListaComuni(c).Prov = CodProvincia And ListaComuni(c).Com = CodComune Then
                                        ComunePresente = True
                                        Exit For
                                    End If
                                Next
                                If ComunePresente = False Then
                                    Dim Comune As New AgronicaControlli_2010.Comune
                                    Comune.Prov = CodProvincia
                                    Comune.Com = CodComune
                                    ListaComuni.Add(Comune)
                                End If
                            End If
                        End If
                    End If
                Next
            End If

        End If

        Return ListaComuni

    End Function

    Public Shared Function IdentificaGrfi_Cod_ProdottiDaTrattare(piva As String, veg_cod As Integer, lav_Cod As Integer, prodottiDaTrattare As attivita.MovimentoDiMagazzino(), objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer
        Dim grfi_cod As Integer = 0

        If prodottiDaTrattare IsNot Nothing AndAlso prodottiDaTrattare.Count > 0 Then

            If prodottiDaTrattare(0).Prodotto IsNot Nothing AndAlso prodottiDaTrattare(0).Prodotto.finalita IsNot Nothing Then
                grfi_cod = prodottiDaTrattare(0).Prodotto.finalita.codice

                For Each prodotto In prodottiDaTrattare
                    If prodotto.Prodotto.finalita.codice <> grfi_cod Then
                        Return -1
                    End If
                Next

            Else

                Dim listMatCod As New List(Of Integer)
                Dim prodotto_string As String = ""
                Dim i As Integer
                For Each prodotto In prodottiDaTrattare
                    listMatCod.Add(prodotto.Prodotto.codice)
                Next

                If listMatCod.Count = 0 Then
                    Return -999
                Else
                    prodotto_string = " Mat_Cod IN (" & String.Join(",", listMatCod) & ")"
                End If

                Dim Elem_Cod As Integer = 0
                Select Case lav_Cod
                    Case LAVCOD_TRATTAMENTO_POST_RACCOLTA
                        Elem_Cod = TRASFORMATI_VEGETALI
                    Case LAVCOD_CONCIA_SEME
                        Elem_Cod = SEMENTI
                    Case Else
                        Return -999
                End Select

                Dim objMateriePrime As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
                Dim dt = objMateriePrime.LeggiFinalitaMateriePrime(piva, Elem_Cod, 0, veg_cod, prodotto_string, "", objParametri_Server)

                Try
                    grfi_cod = dt.Rows(0).Item("Grfi_Cod")
                Catch ex As Exception
                    grfi_cod = 0
                End Try

                For i = 1 To dt.Rows.Count - 1
                    If Not IsDBNull(dt.Rows(i).Item("grfi_cod")) Then
                        If dt.Rows(i).Item("grfi_cod") <> grfi_cod Then
                            Return -1
                        End If
                    Else
                        Return -1
                    End If
                Next
            End If

        End If

        Return grfi_cod

    End Function

    Public Shared Function IdentificaGrfi_Cod(lstImpianti As AgronicaCoreModelsSTD.anagrafiche.Impianto(), objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer
        Dim grfi_cod As Integer = 0

        If lstImpianti IsNot Nothing AndAlso lstImpianti.Count > 0 Then

            If lstImpianti(0).gruppoFinalita IsNot Nothing Then
                grfi_cod = lstImpianti(0).gruppoFinalita.codice

                For Each impianto In lstImpianti
                    If impianto.gruppoFinalita.codice <> grfi_cod Then
                        Return -1
                    End If
                Next
            Else
                Dim appezza_string As String = ""
                Dim i As Integer
                For Each impianto In lstImpianti
                    If appezza_string.Length = 0 Then
                        appezza_string = " (Sa_Cod =" & impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice & " And Appezza =" & impianto.primaryKey.appezzamentoPK.codice & " And ID_REG = " & impianto.primaryKey.codice & ")"
                    Else
                        appezza_string = appezza_string & " Or (Sa_Cod =" & impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice & " And Appezza =" & impianto.primaryKey.appezzamentoPK.codice & " And ID_REG = " & impianto.primaryKey.codice & ")"
                    End If
                Next

                If appezza_string.Length = 0 Then
                    Return -999
                Else
                    appezza_string = "( " & appezza_string & " )"
                End If

                Dim dt As DataTable
                Dim objAppezza As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                dt = objAppezza.Leggi_Lista_grfi_cod(lstImpianti(0).primaryKey.appezzamentoPK.centroAziendalePK.partitaIva, appezza_string, objParametri_Server)

                Try
                    grfi_cod = dt.Rows(0).Item("Grfi_Cod")
                Catch ex As Exception
                    grfi_cod = 0
                End Try

                For i = 1 To dt.Rows.Count - 1
                    If Not IsDBNull(dt.Rows(i).Item("grfi_cod")) Then
                        If dt.Rows(i).Item("grfi_cod") <> grfi_cod Then
                            Return -1
                        End If
                    Else
                        Return -1
                    End If
                Next
            End If

        End If

        Return grfi_cod

    End Function

    'dato l'elenco di impianti restituisce copertura se almeno un impianto ha la copertura
    Public Shared Function IdentificaCopertura(lstImpianti As AgronicaCoreModelsSTD.anagrafiche.Impianto(), objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String
        Dim copertura As String = ""

        If lstImpianti IsNot Nothing AndAlso lstImpianti.Count > 0 Then

            Dim lstImpianti_Con_Copertura_Valorizzata = lstImpianti.Where(Function(imp) Not IsNothing(imp.copertura)).ToList()

            If Not IsNothing(lstImpianti_Con_Copertura_Valorizzata) AndAlso lstImpianti_Con_Copertura_Valorizzata.Count > 0 Then
                For Each imp In lstImpianti_Con_Copertura_Valorizzata
                    If Cop_Cod_Senza_Copertura.Contains(imp.copertura.codice) Then
                        If InStr(copertura, "0") = 0 Then
                            copertura &= "0,"
                        End If
                    Else
                        If InStr(copertura, "1") = 0 Then
                            copertura &= "1,"
                        End If
                    End If
                Next
            End If

            Dim lstImpianti_Senza_Copertura_Valorizzata = lstImpianti.Where(Function(imp) IsNothing(imp.copertura)).ToList()

            If Not IsNothing(lstImpianti_Senza_Copertura_Valorizzata) AndAlso lstImpianti_Senza_Copertura_Valorizzata.Count > 0 Then

                Dim appezza_string As String = ""
                Dim i As Integer
                For Each impianto In lstImpianti_Senza_Copertura_Valorizzata
                    If appezza_string.Length = 0 Then
                        appezza_string = " (Sa_Cod =" & impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice & " And Appezza =" & impianto.primaryKey.appezzamentoPK.codice & " And ID_REG = " & impianto.primaryKey.codice & ")"
                    Else
                        appezza_string = appezza_string & " Or (Sa_Cod =" & impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice & " And Appezza =" & impianto.primaryKey.appezzamentoPK.codice & " And ID_REG = " & impianto.primaryKey.codice & ")"
                    End If
                Next

                If appezza_string.Length = 0 Then
                    Return "-999"
                Else
                    appezza_string = "( " & appezza_string & " )"
                End If

                Dim dt As DataTable
                Dim objAppezza As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read

                dt = objAppezza.Leggi_Lista_cop_cod(lstImpianti_Senza_Copertura_Valorizzata(0).primaryKey.appezzamentoPK.centroAziendalePK.partitaIva, appezza_string, objParametri_Server)

                For i = 0 To dt.Rows.Count - 1
                    If Not IsDBNull(dt.Rows(i).Item("cop_cod")) Then
                        If Cop_Cod_Senza_Copertura.Contains(CInt(dt.Rows(i).Item("cop_cod"))) Then
                            If InStr(copertura, "0") = 0 Then
                                copertura &= "0,"
                            End If
                        Else
                            If InStr(copertura, "1") = 0 Then
                                copertura &= "1,"
                            End If
                        End If
                    End If
                Next

            End If

        End If

        If copertura <> "" Then
            copertura = Left(copertura, copertura.Length - 1)
        End If

        Return copertura

    End Function


    Public Function getCategoriaMagazzino(ByVal lav_cod As Integer) As Integer

        Dim elem_cod = 0

        Select Case lav_cod

            Case LAVCOD_DISTRIBUZIONE_CONCIME, LAVCOD_SARCHIATURA_CONCIMAZIONE, LAVCOD_DISTRIBUZIONE_AMMENDANTI, LAVCOD_CONCIMAZIONE_FOGLIARE, LAVCOD_FERTIRRIGAZIONE, LAVCOD_TRATTAMENTO_ANTIBUTTERATURA
                elem_cod = FERTILIZZANTI

            Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_DISERBO, LAVCOD_DISSECCAMENTO, LAVCOD_GEODISINFESTAZIONE, LAVCOD_CONCIA_SEME, LAVCOD_TRATTAMENTO_FITOREGOLATORE, LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE,
                 LAVCOD_TRATTAMENTO_POST_RACCOLTA, LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA, LAVCOD_REINNESCO_TRAPPOLE
                elem_cod = FORMULATI

            Case LAVCOD_SEMINA, LAVCOD_TRAPIANTO, LAVCOD_SOVESCIO, LAVCOD_SOD_SEDDING
                elem_cod = SEMENTI

            Case LAVCOD_DISTRIBUZIONE_INSETTI
                elem_cod = INSETTI

        End Select

        Return elem_cod

    End Function

    Public Shared Function getSoglia(soglia_cod As Integer, disciplinare As Disciplinare, avversitaGruppo As AgronicaCoreModelsSTD.metaschema.avversita.AvversitaGruppo, objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As Soglia
        Dim soglia As Soglia = Nothing

        Dim AvversitaBiz As New AgronicaControlli_2010.STD_Avversita
        Dim sogliaList = AvversitaBiz.LeggiSoglieAvversita(disciplinare,
                                                                avversitaGruppo,
                                                                objParametri_Super_Server,
                                                                objParametri_Server,
                                                                objParametri_Utenti)

        If sogliaList IsNot Nothing AndAlso sogliaList.Count > 0 Then
            soglia = sogliaList.Where(Function(x) x.codice = soglia_cod).FirstOrDefault() 'DT: non c'è la chiave salvata su db, se ce ne fossero più di una si prende la prima (in attesa di valutare se salvare for_veg_av_cod)
        End If

        Return soglia

    End Function

    Public Shared Function BuildBufferStr(ByVal buffer As BufferZone) As String
        Dim strBuffer = ""

        If buffer IsNot Nothing Then
            If buffer.minimo <> 0 OrElse buffer.massimo <> 0 Then
                strBuffer = buffer.minimo & "|" & buffer.massimo
            End If
        End If

        Return strBuffer

    End Function

    Public Shared Sub Stringhe_from_DosiEtichetta(ByVal dosiEtichetta As List(Of DoseEtichetta),
                                  ByRef DoseText As String,
                                  ByRef DoseValue As String,
                                  ByVal objParametri_Server As AgronicaCoreParametri)

        If dosiEtichetta Is Nothing OrElse dosiEtichetta.Count = 0 Then
            Exit Sub
        End If

        DoseText = ""
        DoseValue = ""
        For Each objDoseEtichetta In dosiEtichetta

            Dim DoseEtichetta As String = ""

            'DT: se le etichette arrivano già popolate, non vengono ricostruite per rilettura da DB
            If Not String.IsNullOrEmpty(objDoseEtichetta.DescrizioneConcatenata) Then
                DoseEtichetta = objDoseEtichetta.DescrizioneConcatenata
            Else

                DoseEtichetta &= objDoseEtichetta.DoseMin & "-" & objDoseEtichetta.DoseMax & " " & If(objDoseEtichetta.Udm IsNot Nothing, objDoseEtichetta.Udm.simbolo, "")

                If objDoseEtichetta.AcquaMin <> 0 Or objDoseEtichetta.AcquaMax <> 0 Then
                    DoseEtichetta &= String.Format(" (Vol.Acqua {0}-{1} {2})", objDoseEtichetta.AcquaMin, objDoseEtichetta.AcquaMax, If(objDoseEtichetta.UdmAcqua IsNot Nothing, objDoseEtichetta.UdmAcqua.simbolo, ""))
                End If

                If objDoseEtichetta.Limite <> 0 Then
                    DoseEtichetta &= String.Format(" (Max {0} interventi {1})", objDoseEtichetta.Limite, If(objDoseEtichetta.UdmLimite IsNot Nothing, objDoseEtichetta.UdmLimite.simbolo, ""))
                End If

                If objDoseEtichetta.Flag_Fioritura IsNot Nothing AndAlso objDoseEtichetta.Flag_Fioritura.codice <> 0 Then
                    DoseEtichetta &= " Sospendere i trattamenti a fine fioritura "
                End If

                If objDoseEtichetta.IntervalloTrattamenti_Min <> 0 Or objDoseEtichetta.IntervalloTrattamenti_Max <> 0 Then
                    DoseEtichetta &= " da effettuare da " & objDoseEtichetta.IntervalloTrattamenti_Min & " - " & objDoseEtichetta.IntervalloTrattamenti_Max & " gg. dal precedente trattamento"
                End If

                Dim strEpoca As String = getEpocaDes(objDoseEtichetta, objParametri_Server)

                If strEpoca <> "" Then
                    DoseEtichetta &= strEpoca
                End If

                If objDoseEtichetta.Mdi IsNot Nothing AndAlso objDoseEtichetta.Mdi.codice <> 0 Then
                    DoseEtichetta &= " - " & STD_Utility.getMidDes(objDoseEtichetta, objParametri_Server)
                End If

                If objDoseEtichetta.Flag_Protetto IsNot Nothing Then
                    Select Case objDoseEtichetta.Flag_Protetto.codice
                        Case 1
                            DoseEtichetta &= " - Serra "
                        Case 2
                            DoseEtichetta &= " - Pieno Campo "
                    End Select
                End If

                If objDoseEtichetta.DataSmaltimentoScorte <> "" Then
                    DoseEtichetta &= " --- [Fine Scorta al " & objDoseEtichetta.DataSmaltimentoScorte & "]"
                End If

            End If

            If Not String.IsNullOrEmpty(DoseText) Then
                DoseText &= "<br>"
            End If
            DoseText &= DoseEtichetta

            'verifico se è dose Ettaro o dose HL
            Dim Udm_Radice, per_ha_hl As Integer
            Dim objUdm As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R
            objUdm.ScomponiUdm(Udm_Radice, per_ha_hl, If(objDoseEtichetta.Udm IsNot Nothing, objDoseEtichetta.Udm.codice, 0))

            If Udm_Radice <> -1 Then

                If Not String.IsNullOrEmpty(DoseValue) Then
                    DoseValue &= "<br>"
                End If

                'DT: se le etichette arrivano già popolate, non vengono ricostruite per rilettura da DB
                If Not String.IsNullOrEmpty(objDoseEtichetta.CodiceConcatenato) Then
                    DoseValue &= objDoseEtichetta.CodiceConcatenato
                Else

                    DoseValue &= objDoseEtichetta.codice & "$" & 'i-->0
                                objDoseEtichetta.DoseMin & "$" & 'i-->1
                                objDoseEtichetta.DoseMax & "$" & 'i-->2
                                If(objDoseEtichetta.Udm IsNot Nothing, objDoseEtichetta.Udm.codice, 0) & "$" & 'i-->3
                                If(objDoseEtichetta.Udm IsNot Nothing, objDoseEtichetta.Udm.simbolo, "") & "$" & 'i-->4
                                objDoseEtichetta.AcquaMin & "$" & 'i-->5
                                objDoseEtichetta.AcquaMax & "$" & 'i-->6
                                If(objDoseEtichetta.UdmAcqua IsNot Nothing, objDoseEtichetta.UdmAcqua.codice, 0) & "$" & 'i-->7
                                If(objDoseEtichetta.UdmAcqua IsNot Nothing, objDoseEtichetta.UdmAcqua.simbolo, "") & "$" & 'i-->8
                                objDoseEtichetta.Da_Epoca & "$" & 'i-->9
                                objDoseEtichetta.A_Epoca & "$" & 'i-->10
                                objDoseEtichetta.Limite & "$" & 'i-->11
                                If(objDoseEtichetta.UdmLimite IsNot Nothing, objDoseEtichetta.UdmLimite.codice, 0) & "$" & 'i-->12
                                If(objDoseEtichetta.UdmLimite IsNot Nothing, objDoseEtichetta.UdmLimite.simbolo, "") & "$" & 'i-->13
                                "" & "$" & 'i-->14 'DT: strCLTOSS_Grado non più gestito, ma va tenuto lo spazio
                                If(objDoseEtichetta.Flag_Fioritura IsNot Nothing, objDoseEtichetta.Flag_Fioritura.codice, 0) & "$" & 'i-->15
                                objDoseEtichetta.IntervalloTrattamenti_Min & "$" & 'i-->16
                                objDoseEtichetta.IntervalloTrattamenti_Max & "$" & 'i-->17
                                If(objDoseEtichetta.Mdi IsNot Nothing, objDoseEtichetta.Mdi.codice, 0) & "$" & 'i-->18
                                If(objDoseEtichetta.Flag_Protetto IsNot Nothing, objDoseEtichetta.Flag_Protetto.codice, 0) & "$" & 'i-->19
                                objDoseEtichetta.FormulatiXAllegatiNormative_IDRiga & "$" & 'i-->20
                                objDoseEtichetta.DataSmaltimentoScorte & "$" & 'i-->21
                                objDoseEtichetta.Gruppo_Dosaggi & "$" & 'i-->22
                                objDoseEtichetta.Num_Max_Interventi_Globali 'i-->23
                End If

            End If

        Next

    End Sub

    Public Shared Function getEpocaDes(objDoseEtichetta As DoseEtichetta, objParametri_Server As AgronicaCoreParametri) As String

        Dim strEpoca As String = ""

        Dim daEpoca As Integer = If(String.IsNullOrEmpty(objDoseEtichetta.Da_Epoca), 0, objDoseEtichetta.Da_Epoca)
        Dim aEpoca As Integer = If(String.IsNullOrEmpty(objDoseEtichetta.A_Epoca), 0, objDoseEtichetta.A_Epoca)

        If daEpoca <> 0 And aEpoca <> 0 Then
            If daEpoca <> aEpoca Then
                If daEpoca <> 0 Then
                    Dim objEpoca As New AgronicaCoreMetaSchemaDAL.Epoche_R
                    strEpoca = " DA " & objEpoca.EpocaDes_from_EpocaCod(daEpoca, objParametri_Server)
                End If
                If aEpoca <> 0 Then
                    Dim objEpoca As New AgronicaCoreMetaSchemaDAL.Epoche_R
                    strEpoca &= " A " & objEpoca.EpocaDes_from_EpocaCod(aEpoca, objParametri_Server)
                End If
            Else
                Dim objEpoca As New AgronicaCoreMetaSchemaDAL.Epoche_R
                strEpoca &= " in " & objEpoca.EpocaDes_from_EpocaCod(aEpoca, objParametri_Server) 'todo, in come si indica nelle risorse??
            End If
        Else
            If daEpoca <> 0 Then
                Dim objEpoca As New AgronicaCoreMetaSchemaDAL.Epoche_R
                strEpoca = " DA " & objEpoca.EpocaDes_from_EpocaCod(daEpoca, objParametri_Server)
            End If
            If aEpoca <> 0 Then
                Dim objEpoca As New AgronicaCoreMetaSchemaDAL.Epoche_R
                strEpoca &= " A " & objEpoca.EpocaDes_from_EpocaCod(aEpoca, objParametri_Server)
            End If
        End If

        Return strEpoca
    End Function

    Public Shared Function getFlagFiorituraDes(objDoseEtichetta As DoseEtichetta, objParametri_Server As AgronicaCoreParametri) As String

        Dim strFlagFiorituraDes As String = ""

        If objDoseEtichetta.Flag_Fioritura IsNot Nothing AndAlso objDoseEtichetta.Flag_Fioritura.codice <> 0 Then
            strFlagFiorituraDes = "Sospendere i trattamenti a fine fioritura"
        End If

        Return strFlagFiorituraDes

    End Function

    Public Shared Function getFlagProtettoDes(objDoseEtichetta As DoseEtichetta, objParametri_Server As AgronicaCoreParametri) As String

        Dim strFlagFiorituraDes As String = ""
        If objDoseEtichetta.Flag_Protetto IsNot Nothing Then

            Select Case objDoseEtichetta.Flag_Protetto.codice
                Case 1
                    strFlagFiorituraDes = "Serra"
                Case 2
                    strFlagFiorituraDes = "Pieno Campo"
            End Select

        End If

        Return strFlagFiorituraDes

    End Function

    Public Shared Function getMidDes(objDoseEtichetta As DoseEtichetta, objParametri_Server As AgronicaCoreParametri) As String

        Dim strMdiDes As String = ""

        If objDoseEtichetta.Mdi IsNot Nothing AndAlso objDoseEtichetta.Mdi.codice <> 0 Then
            Dim objModalita As New AgronicaCoreMetaSchemaDAL.ModalitaImpiego_R
            strMdiDes = objModalita.MdiDes_from_MdiCod(objDoseEtichetta.Mdi.codice, objParametri_Server)
        End If

        Return strMdiDes
    End Function

    Public Shared Sub GetAssettoMagazzino(tipoAttivita As attivita.Attivita.Tipo_Attivita, statoAttivita As attivita.Attivita.Stati, gestioneLotto As enum_Gestione_Lotti, gestioneMagazzino As Integer, gestioneGiacenze As enum_Gestione_Giacenze, bloccaGiacenze_Utente As String, escludiGiacenzeZero As Boolean, ByRef usaLotto As Boolean, ByRef usaMagazzino As Boolean, ByRef usaAnagrafica As Boolean, ByRef flagQtaMaggioreZero As Boolean)
        usaLotto = False
        usaMagazzino = False
        usaAnagrafica = False
        flagQtaMaggioreZero = False

        Select Case gestioneLotto
            Case enum_Gestione_Lotti.Nessuna
                usaLotto = False
            Case Else
                usaLotto = True
        End Select

        Dim tipoOperazione As enum_Tipo_Operazione_Agenda = getTipoOperazione(tipoAttivita, statoAttivita)
        Select Case tipoOperazione

            Case enum_Tipo_Operazione_Agenda.QuadernoDiCampagna, enum_Tipo_Operazione_Agenda.RicettaBrogliaccio

                Select Case gestioneMagazzino
                    Case 0
                        usaMagazzino = False
                        usaAnagrafica = True
                        flagQtaMaggioreZero = False
                    Case 1
                        usaMagazzino = True

                        Select Case gestioneGiacenze
                            Case enum_Gestione_Giacenze.SoloMovimentati
                                usaAnagrafica = False
                                flagQtaMaggioreZero = False

                                If escludiGiacenzeZero Then
                                    flagQtaMaggioreZero = True
                                End If

                            Case enum_Gestione_Giacenze.SoloPresenti
                                usaAnagrafica = False
                                flagQtaMaggioreZero = True

                            Case enum_Gestione_Giacenze.TuttiProdotti
                                usaAnagrafica = True
                                flagQtaMaggioreZero = False

                                If escludiGiacenzeZero Then
                                    usaAnagrafica = False
                                    flagQtaMaggioreZero = True
                                End If

                        End Select

                        If bloccaGiacenze_Utente = "1" Then
                            flagQtaMaggioreZero = True
                        End If

                End Select

            Case enum_Tipo_Operazione_Agenda.Ricetta

                Select Case gestioneMagazzino
                    Case 0
                        usaMagazzino = False
                        usaAnagrafica = True
                        flagQtaMaggioreZero = False

                    Case 1
                        usaMagazzino = True
                        usaAnagrafica = True
                        flagQtaMaggioreZero = False

                        If escludiGiacenzeZero Then
                            usaAnagrafica = False
                            flagQtaMaggioreZero = True
                        End If

                End Select

        End Select

    End Sub

    Public Shared Function getUdmBasefromUdmIndicata(udmIndicata As UnitaDiMisura, ByRef qta As Decimal, objParametri_Server As AgronicaCoreParametri) As UnitaDiMisura

        Dim udmBase As UnitaDiMisura = udmIndicata

        Select Case udmIndicata.codice

            Case enum_UnitaMisura.Grammi, enum_UnitaMisura.Milligrammi, enum_UnitaMisura.Quintali, enum_UnitaMisura.Tonnellate
                udmBase = New UnitaDiMisura(enum_UnitaMisura.KG)

                Select Case udmIndicata.codice
                    Case enum_UnitaMisura.Grammi
                        qta = qta / 1000
                    Case enum_UnitaMisura.Milligrammi
                        qta = qta / 1000000
                    Case enum_UnitaMisura.Quintali
                        qta = qta * 100
                    Case enum_UnitaMisura.Tonnellate
                        qta = qta * 1000
                End Select

            Case enum_UnitaMisura.Millilitri, enum_UnitaMisura.CentimetriCubi, enum_UnitaMisura.Metri_Cubi
                udmBase = New UnitaDiMisura(enum_UnitaMisura.Litri)

                Select Case udmIndicata.codice
                    Case enum_UnitaMisura.Millilitri
                        qta = qta / 1000
                    Case enum_UnitaMisura.CentimetriCubi
                        qta = qta / 1000
                    Case enum_UnitaMisura.Metri_Cubi
                        qta = qta * 1000
                End Select

        End Select

        Dim objDesUM As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R
        Dim simboloUdM As String = ""
        udmBase.descrizione = objDesUM.UdmDes_from_UdmCod(udmBase.codice, simboloUdM, objParametri_Server)
        udmBase.simbolo = simboloUdM

        Return udmBase

    End Function

    Public Shared Function getSaCodDaImpianti(impianti As Impianto()) As List(Of Integer)

        Dim sa_cod_lst As New List(Of Integer)

        If impianti IsNot Nothing AndAlso impianti.Count > 0 Then

            sa_cod_lst = From impianto In impianti
                         Select impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice
                         Distinct.ToList

        End If

        Return sa_cod_lst

    End Function

    Public Shared Function getSaCodDaProdottiDaTrattare(prodottiDaTrattare As attivita.MovimentoDiMagazzino()) As List(Of Integer)

        Dim sa_cod_lst As New List(Of Integer)

        If prodottiDaTrattare IsNot Nothing AndAlso prodottiDaTrattare.Count > 0 Then

            sa_cod_lst = From prodotto In prodottiDaTrattare
                         Select prodotto.Magazzino.primaryKey.centroAziendalePK.codice
                         Distinct.ToList

        End If

        Return sa_cod_lst

    End Function

    Public Shared Function getPivaDaImpianti(impianti As Impianto()) As String

        Dim piva = ""
        If impianti IsNot Nothing AndAlso impianti.Count > 0 Then
            piva = impianti(0).primaryKey.appezzamentoPK.centroAziendalePK.partitaIva
        End If

        Return piva

    End Function

    Public Shared Function getPivaDaProdottiDaTrattare(prodottiDaTrattare As attivita.MovimentoDiMagazzino()) As String

        Dim piva = ""
        If prodottiDaTrattare IsNot Nothing AndAlso prodottiDaTrattare.Count > 0 Then
            piva = prodottiDaTrattare(0).Magazzino.primaryKey.centroAziendalePK.partitaIva
        End If

        Return piva

    End Function

    Public Shared Sub GetCodiciAvversita(avversitaGruppo As AvversitaGruppo, tipoFormulato As Integer, ByRef av_cod As Integer, ByRef av_gru As Integer)

        av_cod = 0
        av_gru = 0

        Select Case tipoFormulato

            Case enum_TipoFormulato.Coadiuvanti
                av_cod = -1
                av_gru = -1

            Case enum_TipoFormulato.Corroboranti_Fisiofarmaci
                av_cod = -2
                av_gru = -2

            Case enum_TipoFormulato.Fitoregolatori, enum_TipoFormulato.Disseccanti
                av_cod = -3
                av_gru = -3

            Case Else
                If avversitaGruppo IsNot Nothing AndAlso avversitaGruppo.codice <> 0 Then
                    Select Case avversitaGruppo.classType
                        Case AgronicaCoreModelsSTD.costanti.ClassType.Avversita
                            av_cod = avversitaGruppo.codice
                            'DT: se avversità singola con codice negativo, valorizzare con lo stesso valore il gruppo
                            If avversitaGruppo.codice < 0 Then
                                av_gru = av_cod
                            End If
                        Case AgronicaCoreModelsSTD.costanti.ClassType.GruppoAvversita
                            av_cod = 0
                            av_gru = avversitaGruppo.codice
                    End Select
                End If

        End Select
    End Sub

    Public Shared Function getfiltroMagazziniEsterni(ByVal Piva As String, ByVal tipoAttivita As attivita.Attivita.Tipo_Attivita, ByVal statoAttivita As attivita.Attivita.Stati,
                                               ByVal magazziniAgenzie As Boolean, ByVal magazziniEsterni As Boolean, ByRef chiaviMagazziniUso_da_terzi As List(Of Fabbricato.PK), ByVal objParametri_Server As AgronicaCoreParametri, ByVal objParametri_Utenti As AgronicaCoreParametri) As List(Of String)

        Dim filtroMagazziniEsterni As List(Of String) = Nothing

        Dim objAnagrafeDAL As New AgronicaCoreAnagrafeDAL.Imprese_Read

        Dim objFabbricatoBIZ As New AgronicaCoreAnagrafeBIZ.Fabbricato_R

        If magazziniAgenzie AndAlso Not magazziniEsterni Then
            Dim tipoOperazione = STD_Utility.getTipoOperazione(tipoAttivita, statoAttivita)
            If tipoOperazione = enum_Tipo_Operazione_Agenda.Ricetta Then
                filtroMagazziniEsterni = objAnagrafeDAL.Imprese_Leggi_VisibilitaUtente_Agenzie_PIVA(objParametri_Server, objParametri_Utenti)
            End If
        End If

        If magazziniEsterni AndAlso Not magazziniAgenzie Then
            Dim listFabbricati_con_Uso_da_Terzi = objFabbricatoBIZ.Fabbricati_con_Uso_da_Terzi_Visibilita_Utente(objParametri_Server, objParametri_Utenti)

            If Not IsNothing(listFabbricati_con_Uso_da_Terzi) AndAlso listFabbricati_con_Uso_da_Terzi.Count > 0 Then

                filtroMagazziniEsterni = New List(Of String)

                chiaviMagazziniUso_da_terzi = New List(Of Fabbricato.PK)

                For Each fabbricato As Fabbricato In listFabbricati_con_Uso_da_Terzi
                    If Piva <> fabbricato.primaryKey.centroAziendalePK.partitaIva Then
                        If Not filtroMagazziniEsterni.Contains(fabbricato.primaryKey.centroAziendalePK.partitaIva) Then
                            filtroMagazziniEsterni.Add(fabbricato.primaryKey.centroAziendalePK.partitaIva)
                        End If

                        chiaviMagazziniUso_da_terzi.Add(fabbricato.primaryKey)
                    End If
                Next
            End If
        End If

        Return filtroMagazziniEsterni
    End Function


    Public Shared Sub FiltraGiacenze_X_MagazziniUso_da_terzi(ByVal chiaviMagazziniUso_da_terzi As List(Of Fabbricato.PK), ByRef Dt_Giacenze As DataTable, ByRef Dt_Giacenze_Tot As DataTable)

        If Not IsNothing(chiaviMagazziniUso_da_terzi) Then

            Dim Dt_Giacenze_Filtrato As New DataTable

            Dim Dt_Giacenze_Filtrato_Tot As New DataTable

            For Each chiave In chiaviMagazziniUso_da_terzi

                Dim Dr_ As DataRow()

                Dr_ = Dt_Giacenze.Select("Sa_Cod = " & chiave.centroAziendalePK.codice & " And Id_Destinazione = " & chiave.codice)

                If Not IsNothing(Dr_) AndAlso Dr_.Count > 0 Then
                    Dt_Giacenze_Filtrato.Merge(Dr_.CopyToDataTable())
                End If

                Dr_ = Dt_Giacenze_Tot.Select("Sa_Cod = " & chiave.centroAziendalePK.codice & " And Id_Destinazione = " & chiave.codice)

                If Not IsNothing(Dr_) AndAlso Dr_.Count > 0 Then
                    Dt_Giacenze_Filtrato_Tot.Merge(Dr_.CopyToDataTable())
                End If
            Next

            Dt_Giacenze = Dt_Giacenze_Filtrato

            Dt_Giacenze_Tot = Dt_Giacenze_Filtrato_Tot
        End If

    End Sub


    ''' <summary>
    ''' Restituisce il codice del disciplinare da utilizzare per le chiamate ai WebService
    ''' Se il disciplinare è pubblico, restituisce il codice positivo, se privato restituisce il codice negativo
    ''' </summary>
    ''' <param name="disciplinare"></param>
    ''' <returns></returns>
    Public Shared Function getDpiCod(ByVal disciplinare As Disciplinare) As Integer

        Dim codDisciplinare As Integer = 0
        If Not IsNothing(disciplinare) Then
            If disciplinare.disciplinarePubblicoPrivato = 2 Then
                codDisciplinare = -disciplinare.codice
            Else
                codDisciplinare = disciplinare.codice
            End If
        End If
        Return codDisciplinare

    End Function

    Public Function GetTipiFormulatiRichiesti(Lav_Cod As String) As String

        Dim TipiRichiesti As New List(Of Integer)

        If DictionaryLavCodTipoFormulato.ContainsKey(Lav_Cod) Then
            TipiRichiesti = DictionaryLavCodTipoFormulato(Lav_Cod)
        End If

        Dim strTipiRichiesti As String = ""

        If TipiRichiesti.Count > 0 Then
            For t = 0 To TipiRichiesti.Count - 1
                strTipiRichiesti &= TipiRichiesti(t) & ","
            Next
            strTipiRichiesti = strTipiRichiesti.Substring(0, strTipiRichiesti.Length - 1)
        End If

        Return strTipiRichiesti
    End Function

End Class
