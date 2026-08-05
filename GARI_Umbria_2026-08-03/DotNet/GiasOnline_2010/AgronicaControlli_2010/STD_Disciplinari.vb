Imports System.Data
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModelsSTD.metaschema

Public Class STD_Disciplinari

    Public Function Leggi_Disciplinari_Testata_conRegolamentoConcimazione(
                                               ByVal Cod_Regolamento As Integer,
                                               ByVal Veg_Cod As Integer,
                                               ByVal Reg_Cod As Integer,
                                               ByVal Lav_Cod_List As Integer(),
                                               ByVal Data As Date,
                                               ByVal objParametri_Super_Server As AgronicaCoreParametri,
                                               ByVal objParametri_Server As AgronicaCoreParametri,
                                               ByVal objParametri_Utenti As AgronicaCoreParametri) As List(Of Disciplinare)

        'DT: il disciplinare -999 NessunDpiNessunaEtichetta non sarebbe applicabile alle Fertilizzazioni, ma viene comunque restituito
        'per non obbligare l'utente, in caso di multioperazione, a deselezionare il -999 per selezionare il 0.
        'Al momento del salvataggio però nelle fertilizzazioni il -999 verrà impostato a 0

        Dim disciplinari_testataList As New List(Of Disciplinare)

        Dim disciplinari_Difesa_testataList As New List(Of Disciplinare)
        Dim disciplinari_Diserbo_testataList As New List(Of Disciplinare)
        Dim disciplinari_Fitoregolatori_testataList As New List(Of Disciplinare)
        Dim disciplinari_Fertilizzazione_testataList As New List(Of Disciplinare)


        Dim Tipo_Testata As Integer = -1
        Dim tipoTestataList = New List(Of Integer)

        Dim codice_NessunoNessuno As String = enum_Disciplinare_Operazione.NessunDpiNessunaEtichetta
        Dim descrizione_NessunoNessuno As String = My.Resources.AgronicaControlli_2010.NessunDisciplinareNessunaEtichetta

        Dim codice_Nessuno As String = "0"
        Dim descrizione_Nessuno As String = My.Resources.AgronicaControlli_2010.SoloEtichetta

        Dim codice_Biologico As String = enum_Disciplinare_Operazione.Biologico
        Dim descrizione_Biologico As String = Descrizione_Regolamento_Bio_New

        Dim disciplinariDefault = GetDisciplinariDefault()
        Dim disciplinareNessuno = disciplinariDefault.disciplinareNessuno
        Dim disciplinareNessunoNessuno = disciplinariDefault.disciplinareNessunoNessuno
        Dim disciplinareBio = disciplinariDefault.disciplinareBio

        'DT 06 02 2023: per ora mostriamo il BIO anche per il diserbo, verrà segnalata la non conformità
        'Dim isDiserbo As Boolean = False
        'For Each Lav_Cod In Lav_Cod_List
        '    If Lav_Cod = LAVCOD_DISERBO Then
        '        isDiserbo = True
        '        Exit For
        '    End If
        'Next

        'DT: contiene le liste Difesa, Diserbo, Fitoregolatori, solo se effettivamente richiamate in base ai lav_cod passati in input.
        'DT: Di esse andrà poi fatta l'intersezione
        Dim multipleList As New List(Of List(Of AgronicaCoreModelsSTD.metaschema.Disciplinare))
        Dim processaFertilizzazione As Boolean = False
        Dim processaNonFertilizzazione As Boolean = False

        For Each Lav_Cod In Lav_Cod_List

            Tipo_Testata = STD_Utility.getTipoTestata(Lav_Cod)

            If Tipo_Testata <> -1 AndAlso Not tipoTestataList.Contains(Tipo_Testata) Then
                tipoTestataList.Add(Tipo_Testata)

                Select Case Tipo_Testata

                    Case enum_Disciplinare_Tipo_Testata.Difesa
                        multipleList.Add(disciplinari_Difesa_testataList)
                        processaNonFertilizzazione = True

                    Case enum_Disciplinare_Tipo_Testata.Diserbo
                        multipleList.Add(disciplinari_Diserbo_testataList)
                        processaNonFertilizzazione = True

                    Case enum_Disciplinare_Tipo_Testata.Fitoregolatore
                        multipleList.Add(disciplinari_Fitoregolatori_testataList)
                        processaNonFertilizzazione = True

                    Case enum_Disciplinare_Tipo_Testata.Fertilizzazione
                        processaFertilizzazione = True

                End Select

            End If

        Next

        Try
            If Veg_Cod > 0 Then

                For Each Tipo_Testata In tipoTestataList

                    Dim dt = AgronicaCoreWebService.Disciplinari_WS.Disciplinari_Elenco_xTestata_conRegolamentoConcimazione(objParametri_Super_Server,
                                                                                            objParametri_Server,
                                                                                            objParametri_Utenti,
                                                                                            Cod_Regolamento,
                                                                                            Reg_Cod,
                                                                                            Veg_Cod,
                                                                                            Id_RcDpi:=0,
                                                                                            Data,
                                                                                            Tipo_Testata)

                    If dt.Rows.Count > 0 Then
                        For Each row As DataRow In dt.Rows
                            Dim disciplinare = New Disciplinare() With
                                    {
                                        .codice = row("Dpi_Cod"),
                                        .descrizione = row("Dpi_Des"),
                                        .disciplinarePubblicoPrivato = row("PubblicoPrivato"),
                                        .flagProtetto = row("Flag_Protetto"),
                                        .gruppoFinalita = New utilizzi.GruppoFinalita(row("Grfi_Cod")),
                                        .raggruppamentiColturaliDPI = If(row("IdRcdpi") IsNot Nothing, New RaggruppamentiColturaliDPI() With {.codice = row("IdRcdpi"), .descrizione = row("DesRcdpi")}, Nothing),
                                        .regolamentoConcimazione = If(row("PUA_Regolamento_Cod") IsNot Nothing, New RegolamentoConcimazione() With {.codice = row("PUA_Regolamento_Cod"), .tipo = row("PUA_Regolamento_Tipo")}, Nothing)
                                    }

                            Select Case Tipo_Testata
                                Case enum_Disciplinare_Tipo_Testata.Difesa
                                    disciplinari_Difesa_testataList.Add(disciplinare)
                                Case enum_Disciplinare_Tipo_Testata.Diserbo
                                    disciplinari_Diserbo_testataList.Add(disciplinare)
                                Case enum_Disciplinare_Tipo_Testata.Fitoregolatore
                                    disciplinari_Fitoregolatori_testataList.Add(disciplinare)
                                Case enum_Disciplinare_Tipo_Testata.Fertilizzazione
                                    disciplinari_Fertilizzazione_testataList.Add(disciplinare)
                            End Select

                        Next

                    End If

                Next

                Dim intersezioneDifesaDiserboFitoregolatori = GetIntersezioneDifesaDiserboFitoregolatori(multipleList)
                For Each disciplinare In intersezioneDifesaDiserboFitoregolatori
                    'DT: se ci sono più disciplinari con raggruppamento colturale diverso, aggiungo il nome del raggruppamento colturale al nome del disciplinare per poterli distinguere
                    Dim countStessoCodice = intersezioneDifesaDiserboFitoregolatori.Where(Function(item) item.codice = disciplinare.codice).Count
                    If countStessoCodice > 1 Then
                        disciplinare.descrizione = disciplinare.descrizione & " - " & disciplinare.raggruppamentiColturaliDPI.descrizione
                    End If
                Next


                If processaFertilizzazione Then
                    If intersezioneDifesaDiserboFitoregolatori.Count > 0 Then
                        disciplinari_testataList = GetIntersezioneConFertilizzazione_New(disciplinari_Fertilizzazione_testataList, intersezioneDifesaDiserboFitoregolatori)
                    Else
                        disciplinari_testataList = disciplinari_Fertilizzazione_testataList
                    End If
                Else
                    disciplinari_testataList = intersezioneDifesaDiserboFitoregolatori
                End If

                disciplinari_testataList.Insert(0, disciplinareNessuno)

                'If Not isDiserbo Then
                disciplinari_testataList.Insert(1, disciplinareBio)
                'End If

            Else

                'If Not isDiserbo Then
                disciplinari_testataList.Add(disciplinareBio)
                'End If

            End If

            'DT: se ci sono solo fertilizzazioni, non va mostrato il disciplinare "nessuno - nessuno"
            If Not (processaFertilizzazione AndAlso Not processaNonFertilizzazione) Then
                disciplinari_testataList.Add(disciplinareNessunoNessuno)
            End If

            '26/01/2023 Lorenzo: Mostro prima le Linee Guida Nazionali poi gli altri DPI in ordine alfabetico i DPI disciplinareNessuno, disciplinareBio e disciplinareNessunoNessuno mantegono la posizione originale

            Dim disciplinari = disciplinari_testataList.Where(Function(d) Not LCase(d.descrizione).Contains("linee guida nazionale") AndAlso d.codice <> codice_Nessuno AndAlso
                                                                  d.codice <> codice_Biologico AndAlso d.codice <> codice_NessunoNessuno).OrderBy(Function(d) d.descrizione).ToList()

            If disciplinari.Count > 0 Then

                Dim index_disciplinareNessuno As Integer = disciplinari_testataList.FindIndex(Function(d) d.codice = codice_Nessuno)
                Dim index_disciplinareBio As Integer = disciplinari_testataList.FindIndex(Function(d) d.codice = codice_Biologico)
                Dim index_disciplinareNessunoNessuno As Integer = disciplinari_testataList.FindIndex(Function(d) d.codice = codice_NessunoNessuno)

                Dim disciplinari_linee_guida = disciplinari_testataList.Where(Function(d) LCase(d.descrizione).Contains("linee guida nazionale")).OrderBy(Function(d) d.descrizione).ToList()

                If disciplinari_linee_guida.Count > 0 Then

                    For index As Integer = 0 To disciplinari_linee_guida.Count - 1
                        disciplinari.Insert(index, disciplinari_linee_guida(index))
                    Next

                End If

                If index_disciplinareNessuno > -1 Then
                    disciplinari.Insert(index_disciplinareNessuno, disciplinareNessuno)
                End If

                If index_disciplinareBio > -1 Then
                    disciplinari.Insert(index_disciplinareBio, disciplinareBio)
                End If

                If index_disciplinareNessunoNessuno > -1 Then
                    disciplinari.Insert(index_disciplinareNessunoNessuno, disciplinareNessunoNessuno)
                End If

                disciplinari_testataList = disciplinari
            End If

        Catch ex As Exception

            disciplinari_testataList.Clear()

        End Try

        Return disciplinari_testataList

    End Function

    Public Function GetDisciplinariDefault() As DisciplinariDefault
        '26/01/2023 Lorenzo:
        'Cambiate le descrizioni dei Disciplinari:
        '- NessunDpiNessunaEtichetta diventa "Nessun Disciplinare – Nessuna Etichetta"
        '- Nessun Disciplinare  diventa "Solo Etichetta"
        '- Descrizione_Regolamento_Bio diventa "BIO"

        Dim codice_NessunoNessuno As String = enum_Disciplinare_Operazione.NessunDpiNessunaEtichetta
        Dim descrizione_NessunoNessuno As String = My.Resources.AgronicaControlli_2010.NessunDisciplinareNessunaEtichetta

        Dim codice_Nessuno As String = "0"
        Dim descrizione_Nessuno As String = My.Resources.AgronicaControlli_2010.SoloEtichetta

        Dim codice_Biologico As String = enum_Disciplinare_Operazione.Biologico
        Dim descrizione_Biologico As String = Descrizione_Regolamento_Bio_New

        Dim disciplinareNessuno = New Disciplinare(codice_Nessuno) With
                        {
                        .descrizione = descrizione_Nessuno,
                        .regolamentoConcimazione = New RegolamentoConcimazione(codice_Nessuno) With {.descrizione = descrizione_Nessuno}
                        }

        Dim disciplinareNessunoNessuno = New Disciplinare(codice_NessunoNessuno) With
                    {
                        .descrizione = descrizione_NessunoNessuno,
                        .regolamentoConcimazione = New RegolamentoConcimazione(codice_NessunoNessuno) With {.descrizione = descrizione_NessunoNessuno}
                    }

        Dim disciplinareBio = New Disciplinare(codice_Biologico) With
                    {
                        .descrizione = descrizione_Biologico,
                        .regolamentoConcimazione = New RegolamentoConcimazione(codice_Biologico) With {.descrizione = descrizione_Biologico}
                    }

        Return New DisciplinariDefault With {
            .disciplinareBio = disciplinareBio,
            .disciplinareNessuno = disciplinareNessuno,
            .disciplinareNessunoNessuno = disciplinareNessunoNessuno
        }

    End Function
    Public Function Leggi_Disciplinari_Testata_DirettivaNitrati(ByVal Cod_Regolamento As Integer,
                                               ByVal Lav_Cod_List As Integer(),
                                               ByVal Data As Date,
                                               ByVal objParametri_Super_Server As AgronicaCoreParametri,
                                               ByVal objParametri_Server As AgronicaCoreParametri,
                                               ByVal objParametri_Utenti As AgronicaCoreParametri) As List(Of AgronicaCoreModelsSTD.metaschema.Disciplinare)

        Dim disciplinari_testataList As New List(Of AgronicaCoreModelsSTD.metaschema.Disciplinare)

        If Lav_Cod_List.Contains(LAVCOD_DISTRIBUZIONE_AMMENDANTI) Then

            Try

                Dim agroWs As String
                Dim objConfigurazione_Siti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                agroWs = objConfigurazione_Siti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametri_Server)
                If agroWs = "" Then
                    agroWs = objConfigurazione_Siti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametri_Super_Server)
                End If

                Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Regolamenti_input
                objParametriIngresso.Regolamento_Cod = Cod_Regolamento
                objParametriIngresso.DataInizio = Data
                objParametriIngresso.DataFine = Data
                objParametriIngresso.strFiltro = " Tipo=" & enum_PUARegolamenti_Tipo.PUA
                objParametriIngresso.url = agroWs

                Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Regolamenti_output
                Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
                objParametriUscita = objPC_WS.Regolamenti(objParametriIngresso)

                For i = 0 To objParametriUscita.ListaRegolamenti.Count - 1

                    If objParametriUscita.ListaRegolamenti(i).Tipo = enum_PUARegolamenti_Tipo.PUA Then

                        disciplinari_testataList.Add(New AgronicaCoreModelsSTD.metaschema.Disciplinare() With
                            {
                                .regolamentoConcimazione = New RegolamentoConcimazione(objParametriUscita.ListaRegolamenti(i).Codice) With
                                {
                                    .descrizione = objParametriUscita.ListaRegolamenti(i).Descrizione,
                                    .tipo = objParametriUscita.ListaRegolamenti(i).Tipo
                                }
                            })
                    End If

                Next

            Catch ex As Exception

                disciplinari_testataList.Clear()

            End Try

        End If

        Return disciplinari_testataList

    End Function

    Private Function GetIntersezioneDifesaDiserboFitoregolatori(multipleList As List(Of List(Of Disciplinare))) As List(Of Disciplinare)

        Dim oldList As New Dictionary(Of String, Disciplinare)

        If multipleList IsNot Nothing AndAlso multipleList.Count > 0 Then
            'DT: intersezione rispetto alla chiave: dpi_cod, id_rcdpi, pubblico/privato
            oldList = multipleList.First().ToDictionary(Function(x) x.codice & "_" & If(x.raggruppamentiColturaliDPI IsNot Nothing, x.raggruppamentiColturaliDPI.codice, 0) & "_" & x.disciplinarePubblicoPrivato, Function(x) x)

            Dim newList As New Dictionary(Of String, Disciplinare)

            If oldList.Count > 0 Then
                For Each list As List(Of Disciplinare) In multipleList.Skip(1)

                    For Each disciplinare As Disciplinare In list

                        Dim disciplinareKey As String = disciplinare.codice & "_" & If(disciplinare.raggruppamentiColturaliDPI IsNot Nothing, disciplinare.raggruppamentiColturaliDPI.codice, 0) & "_" & disciplinare.disciplinarePubblicoPrivato
                        If oldList.Keys.Contains(disciplinareKey) Then
                            If Not newList.Keys.Contains(disciplinareKey) Then
                                newList.Add(disciplinareKey, disciplinare)
                            End If
                        End If
                    Next

                    oldList = New Dictionary(Of String, Disciplinare)(newList)
                    newList.Clear()
                Next
            End If

        End If

        Return oldList.Values.ToList()

    End Function

    'Private Function GetIntersezioneConFertilizzazione_OLD_CON_DISTINZIONE_PUBBLICO_PRIVATO(fertilizzazioneList As List(Of Disciplinare), intersezioneDifesaDiserboFitoregolatori As List(Of Disciplinare)) As List(Of Disciplinare)

    '    Dim oldList As New Dictionary(Of String, Disciplinare)

    '    If fertilizzazioneList IsNot Nothing AndAlso fertilizzazioneList.Count > 0 Then

    '        DT: intersezione rispetto alla chiave: dpi_cod (anzi, la descrizione perchè potrei averci concatenato il raggruppamento colturale), pubblico/privato (che per la fertilizzazione è sempre pubblico)
    '        oldList = fertilizzazioneList.ToDictionary(Function(x) x.codice & "_" & x.disciplinarePubblicoPrivato, Function(x) x)

    '        If oldList.Count > 0 Then

    '            Dim newList As New Dictionary(Of String, Disciplinare)

    '            For Each disciplinare As Disciplinare In intersezioneDifesaDiserboFitoregolatori

    '                Dim disciplinareKey As String = disciplinare.codice & "_" & disciplinare.disciplinarePubblicoPrivato
    '                If oldList.Keys.Contains(disciplinareKey) Then
    '                    DT: la chiave del dictionary è la descrizione perchè a parità di codice, potrei avere più disciplinari con descrizione diversa, dovuto allo split per raggruppamento colturale
    '                    newList.Add(disciplinare.descrizione & "_" & disciplinare.disciplinarePubblicoPrivato, disciplinare)
    '                End If

    '            Next

    '            oldList = New Dictionary(Of String, Disciplinare)(newList)

    '        End If

    '    End If

    '    Return oldList.Values.ToList()

    'End Function

    Private Function GetIntersezioneConFertilizzazione(fertilizzazioneList As List(Of Disciplinare), intersezioneDifesaDiserboFitoregolatori As List(Of Disciplinare)) As List(Of Disciplinare)
        'DT: intersezione rispetto alla chiave: dpi_cod (anzi, la descrizione perchè potrei averci concatenato il raggruppamento colturale)
        'DT: tolto il pubblico/privato come criterio di intersezione, vedi metodo commentato sopra

        Dim oldList As New Dictionary(Of String, Disciplinare)

        If fertilizzazioneList IsNot Nothing AndAlso fertilizzazioneList.Count > 0 Then


            oldList = fertilizzazioneList.ToDictionary(Function(x) x.descrizione.Replace("(In Aggiornamento)", "").Trim(), Function(x) x)

            If oldList.Count > 0 Then

                Dim newList As New Dictionary(Of String, Disciplinare)

                For Each disciplinare As Disciplinare In intersezioneDifesaDiserboFitoregolatori

                    Dim disciplinareKey As String = disciplinare.descrizione
                    disciplinareKey = (disciplinareKey.Replace("(In Aggiornamento)", "")).Trim()
                    If oldList.Keys.Contains(disciplinareKey) Then
                        newList.Add(disciplinare.descrizione, disciplinare)
                    End If

                Next

                oldList = New Dictionary(Of String, Disciplinare)(newList)

            End If

        End If

        Return oldList.Values.ToList()

    End Function

    ''' <summary>
    '''Creo la nuova lista di disciplinari con l'intersezione tra i disciplinari della Difesa, Diserbo, Fitoregolatori e con quelli della Fertilizzazione
    ''' </summary>
    Private Function GetIntersezioneConFertilizzazione_New(fertilizzazioneList As List(Of Disciplinare), intersezioneDifesaDiserboFitoregolatori As List(Of Disciplinare)) As List(Of Disciplinare)

        Dim newList As New List(Of Disciplinare)

        If Not IsNothing(fertilizzazioneList) AndAlso Not IsNothing(intersezioneDifesaDiserboFitoregolatori) Then

            newList = intersezioneDifesaDiserboFitoregolatori

            newList = newList.Where(Function(d) fertilizzazioneList.FindIndex(Function(f) f.codice = d.codice) > -1).ToList()

        End If

        Return newList

    End Function

    Public Function Leggi_Disciplinari_Testata_PianoNutrizionale(
                                               ByVal Cod_Regolamento As Integer,
                                               ByVal Veg_Cod As Integer,
                                               ByVal Data As Date,
                                               ByVal objParametri_Super_Server As AgronicaCoreParametri,
                                               ByVal objParametri_Server As AgronicaCoreParametri,
                                               ByVal objParametri_Utenti As AgronicaCoreParametri) As List(Of Disciplinare)

        Dim disciplinari_testataList As New List(Of Disciplinare)

        Try
            If Veg_Cod > 0 Then

                Try

                    Dim agroWs As String
                    Dim objConfigurazione_Siti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                    agroWs = objConfigurazione_Siti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametri_Server)
                    If agroWs = "" Then
                        agroWs = objConfigurazione_Siti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametri_Super_Server)
                    End If

                    Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Regolamenti_input
                    objParametriIngresso.Regolamento_Cod = Cod_Regolamento
                    objParametriIngresso.DataInizio = Data
                    objParametriIngresso.DataFine = Data
                    objParametriIngresso.strFiltro = " Tipo=" & enum_PUARegolamenti_Tipo.PianoNutrizionale_IBF
                    objParametriIngresso.url = agroWs

                    Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Regolamenti_output
                    Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
                    objParametriUscita = objPC_WS.Regolamenti(objParametriIngresso)

                    For i = 0 To objParametriUscita.ListaRegolamenti.Count - 1

                        If objParametriUscita.ListaRegolamenti(i).Tipo = enum_PUARegolamenti_Tipo.PianoNutrizionale_IBF Then

                            disciplinari_testataList.Add(New AgronicaCoreModelsSTD.metaschema.Disciplinare() With
                                {
                                    .codice = objParametriUscita.ListaRegolamenti(i).Codice,
                                    .descrizione = objParametriUscita.ListaRegolamenti(i).Descrizione,
                                    .regolamentoConcimazione = New RegolamentoConcimazione(objParametriUscita.ListaRegolamenti(i).Codice) With
                                    {
                                        .descrizione = objParametriUscita.ListaRegolamenti(i).Descrizione,
                                        .tipo = objParametriUscita.ListaRegolamenti(i).Tipo
                                    }
                                })
                        End If

                    Next

                Catch ex As Exception

                    disciplinari_testataList.Clear()

                End Try

            End If

        Catch ex As Exception

            disciplinari_testataList.Clear()

        End Try

        Return disciplinari_testataList

    End Function

End Class

Public Class DisciplinariDefault
    Public Property disciplinareNessuno As Disciplinare
    Public Property disciplinareNessunoNessuno As Disciplinare
    Public Property disciplinareBio As Disciplinare
End Class