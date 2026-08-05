Imports System.Data
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModelsSTD.metaschema
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.attivita
Imports AgronicaCoreDataProvider.My.Resources


Public Class STD_UnitaDiMisura
    Public Function LeggiUnitaDiMisura(UdM_default As UnitaDiMisura,
                                       elem_cod As Integer,
                                       avversitaGruppo As avversita.AvversitaGruppo,
                                       lavorazione As Lavorazione,
                                       tipo_attivita As Attivita.Tipo_Attivita,
                                       tipo_ricetta As Attivita.Tipo_Ricetta,
                                       doseEtichetta As DoseEtichetta,
                                       dettagliotrattamento As dettagli.DettaglioTrattamento,
                                       dettagliosemina As dettagli.DettaglioSemina,
                                       dettagliofertilizzazione As dettagli.DettaglioFertilizzazione,
                                       objParametri_Server As AgronicaCoreParametri) As List(Of UnitaDiMisura)

        Dim unitadimisuraList As New List(Of UnitaDiMisura)

        Dim objUDM As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R

        'Dim dtUDM As DataTable = objUDM.Leggi(0, 0,
        '                                            "", "",
        '                                            enumSelezioneVariabile.Selezione_TabellaCompleta,
        '                                            "", "",
        '                                            objParametri_Server
        '                                            )

        'If IsNothing(dtUDM) OrElse dtUDM.Rows.Count = 0 Then
        '    Return unitadimisuraList
        'End If

        Dim Tutte_unitadimisuraList As New List(Of UnitaDiMisura)

        Tutte_unitadimisuraList = LeggiUnitaDiMisuraConTipoControllo(objParametri_Server)

        If IsNothing(Tutte_unitadimisuraList) OrElse Tutte_unitadimisuraList.Count = 0 Then
            Return unitadimisuraList
        End If

        Dim Lav_Cod As Integer = lavorazione.getCodice()

        Dim UdM_prodotto_magazzino As UnitaDiMisura = Nothing

        Select Case elem_cod
            Case FERTILIZZANTI
                If Not IsNothing(dettagliofertilizzazione) AndAlso Not IsNothing(dettagliofertilizzazione.MagazziniMovimentazioni) AndAlso dettagliofertilizzazione.MagazziniMovimentazioni.Count = 1 Then
                    UdM_prodotto_magazzino = dettagliofertilizzazione.MagazziniMovimentazioni(0).udm
                End If
            Case FORMULATI, INSETTI
                If Not IsNothing(dettagliotrattamento) AndAlso Not IsNothing(dettagliotrattamento.MagazziniMovimentazioni) AndAlso dettagliotrattamento.MagazziniMovimentazioni.Count = 1 Then
                    UdM_prodotto_magazzino = dettagliotrattamento.MagazziniMovimentazioni(0).udm
                End If
            Case SEMENTI
                If Not IsNothing(dettagliosemina) AndAlso Not IsNothing(dettagliosemina.MagazziniMovimentazioni) AndAlso dettagliosemina.MagazziniMovimentazioni.Count = 1 Then
                    UdM_prodotto_magazzino = dettagliosemina.MagazziniMovimentazioni(0).udm
                End If
            Case INNESCHI
                If Not IsNothing(avversitaGruppo) AndAlso Not IsNothing(avversitaGruppo.MagazziniMovimentazioni) AndAlso avversitaGruppo.MagazziniMovimentazioni.Count = 1 Then
                    UdM_prodotto_magazzino = avversitaGruppo.MagazziniMovimentazioni(0).udm
                End If
        End Select

        Dim UdM_doseetichetta As UnitaDiMisura = Nothing

        If Not IsNothing(doseEtichetta) Then
            UdM_doseetichetta = doseEtichetta.Udm
        End If

        Dim leggiUdmDaDb As Boolean = False

        'Questo caricamento replica in parte il AgronicaCoreUtility.CaricaListControl.UnitaMisuraAgenda

        Dim elementiDaCaricareList As New List(Of Integer)
        Select Case elem_cod

            Case FERTILIZZANTI

                If Not CaricaUdm_da_magazzino_etichetta(UdM_doseetichetta, UdM_prodotto_magazzino, Tutte_unitadimisuraList, unitadimisuraList) Then
                    leggiUdmDaDb = True

                    elementiDaCaricareList = New List(Of Integer)({2, 3, 4, 29, 101, 104, 304})
                End If


            Case FORMULATI

                If Not CaricaUdm_da_magazzino_etichetta(UdM_doseetichetta, UdM_prodotto_magazzino, Tutte_unitadimisuraList, unitadimisuraList) Then
                    leggiUdmDaDb = True

                    elementiDaCaricareList = New List(Of Integer)({2, 3, 29, 101, 104})
                End If

            Case SEMENTI

                leggiUdmDaDb = False

                'Semente Senza Magazzino
                If Not CaricaUdm_da_magazzino_etichetta(UdM_doseetichetta, UdM_prodotto_magazzino, Tutte_unitadimisuraList, unitadimisuraList) Then

                    unitadimisuraList = New List(Of UnitaDiMisura)

                    CaricaKG(unitadimisuraList, Tutte_unitadimisuraList)

                    unitadimisuraList.Add(Tutte_unitadimisuraList.Where(Function(u) u.codice = enum_UnitaMisura.Unita_Seme)(0))

                    unitadimisuraList.Add(Tutte_unitadimisuraList.Where(Function(u) u.codice = enum_UnitaMisura.Num_Piante)(0))

                    unitadimisuraList.Add(Tutte_unitadimisuraList.Where(Function(u) u.codice = enum_UnitaMisura.Confezioni)(0))
                End If

            Case INSETTI
                unitadimisuraList.Add(Tutte_unitadimisuraList.Where(Function(u) u.codice = enum_UnitaMisura.Numero)(0))

            Case INNESCHI
                If Not CaricaUdm_da_magazzino_etichetta(Nothing, UdM_prodotto_magazzino, Tutte_unitadimisuraList, unitadimisuraList) Then
                    leggiUdmDaDb = True

                    elementiDaCaricareList = New List(Of Integer)({enum_UnitaMisura.Numero})
                End If

        End Select


        If leggiUdmDaDb Then

            For Each UdM As UnitaDiMisura In Tutte_unitadimisuraList

                If elementiDaCaricareList.Count > 0 Then

                    If elementiDaCaricareList.Contains(UdM.codice) Then
                        unitadimisuraList.Add(UdM)
                    End If

                Else

                    unitadimisuraList.Add(UdM)

                End If

            Next

        End If


        'Escludo alcune UdM
        Select Case elem_cod
            Case FERTILIZZANTI

                'TODO Da sviluppare il caso della distribuzione ammendanti con tipo ricetta PUA (funzione Cambiato_Fertilizzante riga 8074 della Trattamenti_2)

                'se sono in Distribuzione Concime o distribuzione ammendanti elimino unità misura grammi e cm2
                If Lav_Cod = LAVCOD_DISTRIBUZIONE_CONCIME Or
                   Lav_Cod = LAVCOD_DISTRIBUZIONE_AMMENDANTI Then
                    'aggiungo metri cubi
                    Dim unitadimisura = Tutte_unitadimisuraList.Where(Function(u) u.codice = enum_UnitaMisura.Metri_Cubi)(0)

                    unitadimisuraList.Add(unitadimisura)

                    unitadimisuraList = unitadimisuraList.Where(Function(udm) udm.codice <> enum_UnitaMisura.Millilitri AndAlso udm.codice <> enum_UnitaMisura.Grammi AndAlso udm.codice <> enum_UnitaMisura.CentimetriCubi
                                                                ).ToList

                End If


        End Select

        'aggiungo l'udm di default se non è presente
        If Not IsNothing(UdM_default) AndAlso UdM_default.codice > 0 AndAlso unitadimisuraList.FindIndex(Function(u) u.codice = UdM_default.codice) = -1 Then
            unitadimisuraList.Add(UdM_default)
        End If


        Return unitadimisuraList
    End Function

    Public Function LeggiUnitaDiMisuraConTipoControllo(ByVal objParametri_Server As AgronicaCoreParametri, Optional ByVal xFiltroAggiuntivo As String = "") As List(Of UnitaDiMisura)
        Dim unitadimisuraList As New List(Of UnitaDiMisura)
        Dim objUDM As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R
        Dim dtUDM As DataTable = objUDM.Leggi(0, 0, "", "",
                                              enumSelezioneVariabile.Selezione_TabellaCompleta,
                                              xFiltroAggiuntivo, "", objParametri_Server
                                             )
        If IsNothing(dtUDM) OrElse dtUDM.Rows.Count = 0 Then
            Return unitadimisuraList
        End If
        For Each row In dtUDM.Rows()
            Dim udm = New UnitaDiMisura(row.item("UDM_COD"))
            udm.simbolo = row.item("UDM_SIM")
            udm.descrizione = row.item("UDM_DES")
            udm.tipoControllo = New AgronicaCoreModelsSTD.baseClass.BaseCodeDescr()
            If IsNothing(row.item("TipoControllo_Cod")) OrElse TypeOf row.item("TipoControllo_Cod") Is DBNull Then
                udm.tipoControllo.codice = 0
            Else
                udm.tipoControllo.codice = row.item("TipoControllo_Cod")
            End If
            udm.tipoControllo.descrizione = row.item("TipoControllo_Des")
            unitadimisuraList.Add(udm)
        Next
        Return unitadimisuraList
    End Function

    Public Function LeggiTipoControllo(ByVal Udm_Cod As Integer, ByVal objParametri_Server As AgronicaCoreParametri) As AgronicaCoreModelsSTD.baseClass.BaseCodeDescr

        Dim tipoControllo As New AgronicaCoreModelsSTD.baseClass.BaseCodeDescr()
        Dim objUDM As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R
        Dim dtUDM As DataTable = objUDM.Leggi(Udm_Cod, 0, "", "",
                                                  enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                  "", "", objParametri_Server)

        If Not IsNothing(dtUDM) AndAlso dtUDM.Rows.Count > 0 Then
            If IsNothing(dtUDM.Rows(0).Item("TipoControllo_Cod")) OrElse TypeOf dtUDM.Rows(0).Item("TipoControllo_Cod") Is DBNull Then
                tipoControllo.codice = 0
            Else
                tipoControllo.codice = dtUDM.Rows(0).Item("TipoControllo_Cod")
            End If
            tipoControllo.descrizione = dtUDM.Rows(0).Item("TipoControllo_Des")
        End If

        Return tipoControllo
    End Function

    Private Function CaricaUdm_da_magazzino_etichetta(ByVal UdM_doseetichetta As UnitaDiMisura, ByVal UdM_prodotto_magazzino As UnitaDiMisura, ByVal Tutte_unitadimisuraList As List(Of UnitaDiMisura), ByRef unitadimisuraList As List(Of UnitaDiMisura))

        Dim Udm_caricate As Boolean = False

        If (Not IsNothing(UdM_doseetichetta) AndAlso UdM_doseetichetta.codice > 0) OrElse
                   (Not IsNothing(UdM_prodotto_magazzino) AndAlso UdM_prodotto_magazzino.codice > 0) Then

            Udm_caricate = True

            If Not IsNothing(UdM_doseetichetta) AndAlso UdM_doseetichetta.codice > 0 AndAlso
                Not IsNothing(UdM_prodotto_magazzino) AndAlso UdM_prodotto_magazzino.codice > 0 Then

                'In questo caso se sono popolate sia l'udm della dose etichetta sia l'udm del prodotto a magazzino restituisco tutte le udm compatibili con entrambi

                CaricaUdmRadice_e_UdM_Compatibili(unitadimisuraList, Tutte_unitadimisuraList, UdM_doseetichetta)

                CaricaUdmRadice_e_UdM_Compatibili(unitadimisuraList, Tutte_unitadimisuraList, UdM_prodotto_magazzino)

            Else

                If Not IsNothing(UdM_doseetichetta) AndAlso UdM_doseetichetta.codice > 0 Then

                    CaricaUdmRadice_e_UdM_Compatibili(unitadimisuraList, Tutte_unitadimisuraList, UdM_doseetichetta)

                End If

                If Not IsNothing(UdM_prodotto_magazzino) AndAlso UdM_prodotto_magazzino.codice > 0 Then

                    CaricaUdmRadice_e_UdM_Compatibili(unitadimisuraList, Tutte_unitadimisuraList, UdM_prodotto_magazzino)

                End If

            End If

        End If

        Return Udm_caricate

    End Function

    Private Sub CaricaUdmRadice_e_UdM_Compatibili(ByRef unitadimisuraList As List(Of UnitaDiMisura), ByVal Tutte_unitadimisuraList As List(Of UnitaDiMisura), ByVal UdM_Da_Analizzare As UnitaDiMisura)

        Dim objUDM As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R

        Dim Udm_Radice = objUDM.Converti_Kg_L_from_UdmCod(UdM_Da_Analizzare.codice, "", "")

        'Evito di caricare udm già presenti
        Select Case Udm_Radice
            Case enum_UnitaMisura.KG
                If unitadimisuraList.FindIndex(Function(udm) udm.codice = enum_UnitaMisura.KG) = -1 Then
                    CaricaKG(unitadimisuraList, Tutte_unitadimisuraList)
                End If
            Case enum_UnitaMisura.Litri
                If unitadimisuraList.FindIndex(Function(udm) udm.codice = enum_UnitaMisura.Litri) = -1 Then
                    CaricaLitri(unitadimisuraList, Tutte_unitadimisuraList)
                End If
            Case Else

                objUDM.ScomponiUdm(Udm_Radice, 0, UdM_Da_Analizzare.codice)

                If unitadimisuraList.FindIndex(Function(udm) udm.codice = Udm_Radice) = -1 Then

                    If Udm_Radice = UdM_Da_Analizzare.codice Then
                        unitadimisuraList.Add(Tutte_unitadimisuraList.Where(Function(u) u.codice = Udm_Radice)(0))
                    End If

                End If
        End Select
    End Sub

    'Carica i kg e le sue Udm compatibili (replica il comportamento delle funzione omonima  della Trattamenti_2)
    Private Sub CaricaKG(ByRef unitadimisuraList As List(Of UnitaDiMisura), ByVal Tutte_unitadimisuraList As List(Of UnitaDiMisura))

        unitadimisuraList.Add(Tutte_unitadimisuraList.Where(Function(u) u.codice = enum_UnitaMisura.Grammi)(0))

        unitadimisuraList.Add(Tutte_unitadimisuraList.Where(Function(u) u.codice = enum_UnitaMisura.KG)(0))

        unitadimisuraList.Add(Tutte_unitadimisuraList.Where(Function(u) u.codice = enum_UnitaMisura.Quintali)(0))

        unitadimisuraList.Add(Tutte_unitadimisuraList.Where(Function(u) u.codice = enum_UnitaMisura.Tonnellate)(0))

    End Sub

    'Carica i l e le sue Udm compatibili (replica il comportamento delle funzione omonima  della Trattamenti_2)
    Private Sub CaricaLitri(ByRef unitadimisuraList As List(Of UnitaDiMisura), ByVal Tutte_unitadimisuraList As List(Of UnitaDiMisura))

        unitadimisuraList.Add(Tutte_unitadimisuraList.Where(Function(u) u.codice = enum_UnitaMisura.Millilitri)(0))

        unitadimisuraList.Add(Tutte_unitadimisuraList.Where(Function(u) u.codice = enum_UnitaMisura.CentimetriCubi)(0))

        unitadimisuraList.Add(Tutte_unitadimisuraList.Where(Function(u) u.codice = enum_UnitaMisura.Litri)(0))

    End Sub



End Class
