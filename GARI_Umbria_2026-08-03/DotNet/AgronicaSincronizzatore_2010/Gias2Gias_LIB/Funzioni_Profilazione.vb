Imports System.Xml
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports System.Text

Partial Public Class Funzioni


#Region "Profilazione"

    Public Sub Elabora_Profilazione_Salva( _
                                ByVal objOpzioni As clsOpzioni, _
                                ByRef Log_Import As StringBuilder, _
                                ByRef Log_Errori As StringBuilder, _
                                ByRef Log_Riepilogo As StringBuilder, _
                                ByVal Piva_Origine As String, _
                                ByVal Piva_Destinazione As String, _
                                ByVal Flag_DefaultGlobali As Boolean _
                                )

        Const nomeFunzione As String = "Elabora_Profilazione_Salva"


        Try

            Dim objProfil_R As New AgronicaCoreProfilazioneDAL.Profilazione_R
            Dim objProfil_W As New AgronicaCoreProfilazioneDAL.Profilazione_W

            Dim DT_Profil As DataTable
            Dim Filtro_Aggiuntivo As String = ""

            If Flag_DefaultGlobali = True Then
                Piva_Origine = ""
                Piva_Destinazione = ""
                Filtro_Aggiuntivo = " Profilazione_Dati.Piva = '' "
            End If

            DT_Profil = objProfil_R.Leggi(Piva_Origine, _
                                           0, _
                                           "", _
                                           "", _
                                           Filtro_Aggiuntivo, _
                                            "", _
                                            0, _
                                            0, _
                                            False, _
                                           objOpzioni.objParametri_Server_GIAS_ORIGINE)

            Dim ID_Profilo_Dati_DESTINAZIONE As Integer
            Dim ObjSequenze As New AgronicaCoreDataProvider.Agro_Sequenze
            Dim Codice_Chiave As String 'rimane uguale
            Dim Quesito As String 'rimane uguale
            Dim Id_Gruppo As String 'rimane uguale
            Dim Lav_Cod As Integer 'rimane uguale
            Dim Veg_Cod As Integer 'rimane uguale

            Dim Valore_Salvato_ORIGINE As String 'va rimappato
            Dim Valore_Salvato_DESTINAZIONE As String 'va rimappato

            If Not DT_Profil Is Nothing Then

                For Each Dr_Profil In DT_Profil.Rows

                    ID_Profilo_Dati_DESTINAZIONE = ObjSequenze.NuovoId_Tabella( _
                                                    "profilazione_dati", _
                                                    0, _
                                                    2000000000,
                                                    objOpzioni.objParametri_Server_GIAS_DESTINAZIONE _
                                                    )

                    Codice_Chiave = CType(Dr_Profil("Codice_Chiave"), String)
                    Quesito = CType(Dr_Profil("Quesito"), String)
                    Id_Gruppo = CType(Dr_Profil("Id_Gruppo"), String)
                    Lav_Cod = CType(Dr_Profil("Lav_Cod"), Integer)
                    Veg_Cod = CType(Dr_Profil("Veg_Cod"), Integer)
                    Valore_Salvato_ORIGINE = CType(Dr_Profil("Valore_Salvato"), String)

                    Valore_Salvato_DESTINAZIONE = Decripta_Valore_Salvato_Profilazione(objOpzioni, _
                                                                                       Valore_Salvato_ORIGINE)

                    objProfil_W.Scrivi(Piva_Destinazione, _
                                        ID_Profilo_Dati_DESTINAZIONE, _
                                        Codice_Chiave, _
                                        Quesito, _
                                        Id_Gruppo, _
                                       Valore_Salvato_DESTINAZIONE, _
                                       CType(Dr_Profil("Validita_Inizio"), Date), _
                                        CType(Dr_Profil("Validita_Fine"), Date), _
                                        Lav_Cod, _
                                        Veg_Cod, _
                                        objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)


                Next

            End If

        Catch ex As Exception

            Dim msg As String
            msg = CStr(Date.Now) + " - " + nomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try


    End Sub

    '################################################
    Private Function Decripta_Valore_Salvato_Profilazione(ByVal objOpzioni As clsOpzioni, _
                                                          ByVal Valore_Salvato_ORIGINE As String) As String

        Dim value_pipe() As String
        Dim value_graffa As String
        Dim indice_barra_inizio As Integer
        Dim indice_graffa_inizio As Integer
        Dim indice_graffa_fine As Integer
        Dim len_val_graffa As Integer
        Dim value_virgola() As String
        Dim value_barra As String = ""
        Dim n_i, nv_i As Integer

        Dim TestoCampo As String = ""

        Dim Cod_ORIGINE As Integer
        Dim Cod_DEST As Integer

        Dim Valore_Salvato_DESTINAZIONE As String = ""

        'valori separati da pipe
        value_pipe = Valore_Salvato_ORIGINE.Split("|")

        'Select Case Id_Gruppo.ToLower
        '    Case "note"
        '        'per le note: “notautilizzo_cod=-1|nota_cod={13,27}”
        '        'notautilizzo_cod coincide con il valore di Codice_Chiave
        '        'nota_cod è la chiave della nota
        '    Case "macxlav"
        '        'per le macXlav: “lav_cod=63|mac_cod={15,34,96}/1:30”
        '        'lav_cod coincide con il valore di Codice_Chiave
        '        'mac_cod = è il codice della macchina, ovvero il mac_cod
        '        'cod_cont = è il codice del contatto, ovvero il cod_risum
        '        'dopo la barra “/” viene indicato il tempo in ore impiegato ad ettaro (per il calcolo dei costi accesori, dove a partire dalla sup impianto, si ricavano le ore impiegate dalla amcchina/contatto)
        'End Select

        For n_i = 0 To value_pipe.Length - 1

            If InStr(value_pipe(n_i), "notautilizzo_cod=") > 0 Then
                value_pipe(n_i) = Replace(value_pipe(n_i), "notautilizzo_cod=", "")
                TestoCampo = "notautilizzo_cod"
            End If
            If InStr(value_pipe(n_i), "nota_cod=") > 0 Then
                value_pipe(n_i) = Replace(value_pipe(n_i), "nota_cod=", "")
                TestoCampo = "nota_cod"
            End If
            If InStr(value_pipe(n_i), "lav_cod=") > 0 Then
                value_pipe(n_i) = Replace(value_pipe(n_i), "lav_cod=", "")
                TestoCampo = "lav_cod"
            End If
            If InStr(value_pipe(n_i), "mac_cod=") > 0 Then
                value_pipe(n_i) = Replace(value_pipe(n_i), "mac_cod=", "")
                TestoCampo = "mac_cod"
            End If
            If InStr(value_pipe(n_i), "cod_cont=") > 0 Then
                value_pipe(n_i) = Replace(value_pipe(n_i), "cod_cont=", "")
                TestoCampo = "cod_cont"
            End If

            Valore_Salvato_DESTINAZIONE += TestoCampo + "="

            If InStr(value_pipe(n_i), "{") > 0 Then

                If InStr(value_pipe(n_i), "/") > 0 Then
                    indice_barra_inizio = value_pipe(n_i).LastIndexOf("/")
                    'devo aggiungere tutto dalla barra (inclusa) in poi
                    value_barra = Mid(value_pipe(n_i), indice_barra_inizio)
                End If

                Valore_Salvato_DESTINAZIONE += "{"

                'se il carattere è alla prima posizione, restituisce 0
                'ma occhio al mid che parte da 1, non da 0!
                indice_graffa_inizio = value_pipe(n_i).LastIndexOf("{")
                indice_graffa_fine = value_pipe(n_i).LastIndexOf("}")
                len_val_graffa = indice_graffa_fine - indice_graffa_inizio - 1

                '+2= +1 (dal carattere dopo, +1 perchè il mid parte da 1)
                value_graffa = Mid(value_pipe(n_i), indice_graffa_inizio + 2, len_val_graffa)

                If value_graffa <> "" Then
                    If InStr(value_graffa, ",") > 0 Then
                        value_virgola = value_graffa.Split(",")

                        'più valori separati da virgola
                        For nv_i = 0 To value_virgola.Length - 1
                            Cod_ORIGINE = value_virgola(nv_i)
                            Cod_DEST = Cod_DESTINAZIONE_from_Cod_ORIGINE(objOpzioni, Cod_ORIGINE, TestoCampo)
                            If Cod_DEST <> 0 Then
                                Valore_Salvato_DESTINAZIONE += CStr(Cod_DEST)
                            End If
                            If nv_i = value_virgola.Length - 1 Then
                                'se sono all'ultimo valore della virgola, chiudo la graffa
                                Valore_Salvato_DESTINAZIONE += "}"
                                If value_barra <> "" Then
                                    Valore_Salvato_DESTINAZIONE += value_barra
                                End If
                            Else
                                If Cod_DEST <> 0 Then
                                    'aggiungo tranne che all'ultimo giro
                                    'e tranne se non ha mappato il cod dest
                                    Valore_Salvato_DESTINAZIONE += ","
                                End If
                            End If

                        Next
                    Else
                        'un valore unico
                        Cod_ORIGINE = value_graffa
                        Cod_DEST = Cod_DESTINAZIONE_from_Cod_ORIGINE(objOpzioni, Cod_ORIGINE, TestoCampo)
                        If Cod_DEST <> 0 Then
                            Valore_Salvato_DESTINAZIONE += CStr(Cod_DEST)
                        End If
                        'chiudo subito la graffa
                        Valore_Salvato_DESTINAZIONE += "}"
                        If value_barra <> "" Then
                            Valore_Salvato_DESTINAZIONE += value_barra
                        End If
                    End If
                Else
                    'value_graffa=""
                    Valore_Salvato_DESTINAZIONE += "}"
                    If value_barra <> "" Then
                        Valore_Salvato_DESTINAZIONE += value_barra
                    End If
                End If

            Else
                'valore senza graffa
                If value_pipe(n_i) <> "" Then
                    Cod_ORIGINE = value_pipe(n_i)
                    Cod_DEST = Cod_DESTINAZIONE_from_Cod_ORIGINE(objOpzioni, Cod_ORIGINE, TestoCampo)
                    If Cod_DEST <> 0 Then
                        Valore_Salvato_DESTINAZIONE += CStr(Cod_DEST)
                    End If
                End If
            End If

            If n_i <> value_pipe.Length - 1 Then
                'aggiungo tranne che all'ultimo giro
                Valore_Salvato_DESTINAZIONE += "|"
            End If
        Next


        Return Valore_Salvato_DESTINAZIONE


    End Function

    '#######################################################################
    Private Function Cod_DESTINAZIONE_from_Cod_ORIGINE(ByVal objOpzioni As clsOpzioni, _
                                                       ByVal Cod_ORIGINE As Integer, _
                                                        ByVal TestoCampo As String) As Integer

        Dim Cod_DEST As Integer = 0

        Select Case TestoCampo.ToLower

            Case "notautilizzo_cod"
                Cod_DEST = FunzioniGLOBAL.recode_return_NotaUtilizzoCod(objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser, Cod_ORIGINE)

            Case "nota_cod"
                Cod_DEST = FunzioniGLOBAL.recode_return_NotaCod(objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser, Cod_ORIGINE)

            Case "mac_cod"
                Cod_DEST = recode_return_MacCod(Cod_ORIGINE, objOpzioni.SuperUser_CodFiscale_DESTINAZIONE)

            Case "cod_cont"
                Cod_DEST = recode_return_CodRisUm(Cod_ORIGINE, objOpzioni.SuperUser_CodFiscale_DESTINAZIONE)

            Case "lav_cod"
                Cod_DEST = Cod_ORIGINE

        End Select

        If Cod_DEST = 0 Then
            'bisogna capire se è 0 perchè non è stato mappato per un qualche motivo
            'o se è 0 perchè l'elemento è stato cancellato e non esiste più

            Select Case TestoCampo.ToLower
                Case "mac_cod"
                    Dim objPM As New AgronicaCoreContabDAL.Parco_Macchine_R
                    Dim flag As Boolean
                    flag = objPM.Esiste_MacCod(Cod_ORIGINE, _
                                                objOpzioni.objParametri_Server_GIAS_ORIGINE)
                    If flag = True Then
                        'la macchina esiste, quindi non è stato mappato il codice
                        Throw New Exception("Codice Destinazione = 0 x profilazione dati.")
                    Else
                        'la macchina non esiste, è stata cancellata dall'archivio, ma è rimasta profilata
                        Cod_DEST = 0
                    End If
                    '-------
                Case "cod_cont"
                    Dim objPM As New AgronicaCoreAnagrafeDAL.Risorse_Umane_R
                    Dim flag As Boolean
                    flag = objPM.Esiste_CodRisUm(Cod_ORIGINE, _
                                                    objOpzioni.objParametri_Server_GIAS_ORIGINE)
                    If flag = True Then
                        'la risorsa umana esiste, quindi non è stato mappato il codice
                        Throw New Exception("Codice Destinazione = 0 x profilazione dati.")
                    Else
                        'la risorsa umana non esiste, è stata cancellata dall'archivio, ma è rimasta profilata
                        Cod_DEST = 0
                    End If
                    '-------
                Case "nota_cod"
                    Dim objPM As New AgronicaCoreContabDAL.Note_Intervento_R
                    Dim flag As Boolean
                    flag = objPM.Esiste_NotaCod(Cod_ORIGINE, _
                                                 objOpzioni.objParametri_Server_GIAS_ORIGINE)
                    If flag = True Then
                        'la nota esiste, quindi non è stato mappato il codice
                        Throw New Exception("Codice Destinazione = 0 x profilazione dati.")
                    Else
                        'la nota non esiste, è stata cancellata dall'archivio, ma è rimasta profilata
                        Cod_DEST = 0
                    End If
                    '-------
                Case Else
                    Throw New Exception("Codice Destinazione = 0 x profilazione dati.")
                    '-------
            End Select
        End If

        Return Cod_DEST

    End Function


#End Region

#Region "SpecieVegetali_Default"

    Public Sub Elabora_SpecieVegetali_Default_Salva( _
                                ByVal objOpzioni As clsOpzioni, _
                                ByRef Log_Import As StringBuilder, _
                                ByRef Log_Errori As StringBuilder, _
                                ByRef Log_Riepilogo As StringBuilder, _
                                ByVal Piva_Origine As String, _
                                ByVal Piva_Destinazione As String _
                                )

        Const nomeFunzione As String = "SpecieVegetali_Default"


        Try

            Dim objSVD_R As New AgronicaCoreAnagrafeDAL.SpecieVegetali_Default_R
            Dim objSVD_W As New AgronicaCoreAnagrafeDAL.SpecieVegetali_Default_W

            Dim DT_SVD As DataTable = _
                objSVD_R.Leggi(Piva_Origine, _
                               0, 0, 0, 0, _
                               AGRODATAINIZIO, _
                               AGRODATAFINE, _
                                enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                "", _
                                "", _
                                objOpzioni.objParametri_Server_GIAS_ORIGINE _
                                )


            Dim ID_ORIGINE As Integer
            Dim ID_DESTINAZIONE As Integer
            Dim ObjSequenze As New AgronicaCoreDataProvider.Agro_Sequenze

            If Not DT_SVD Is Nothing Then

                For Each rowSVD In DT_SVD.Rows

                    ID_DESTINAZIONE = ObjSequenze.NuovoId_Tabella("SpecieVegetali_Default", _
                                                                      objOpzioni.BaseCode_DESTINAZIONE, _
                                                                      objOpzioni.TopCode_DESTINAZIONE,
                                                                      objOpzioni.objParametri_Server_GIAS_DESTINAZIONE _
                                                                      )

                    ID_ORIGINE = rowSVD("Id")

                    'SpecieVegetaliDefault.Add( _
                    'New recode_SpecieVegetaliDefault With { _
                    '    .From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser, _
                    '    .To_PivaSuperUSer = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser, _
                    '    .From_Id = ID_ORIGINE, _
                    '    .To_Id = ID_DESTINAZIONE _
                    '})

                    objSVD_W.Scrivi(ID_DESTINAZIONE, _
                                    Piva_Destinazione, _
                                    CType(rowSVD("Veg_Cod"), Integer), _
                                    CType(rowSVD("Cul_Cod"), Integer), _
                                    CType(rowSVD("Codice"), Integer), _
                                    CType(rowSVD("Valore"), String), _
                                    CType(rowSVD("numero_ciclo"), Integer), _
                                    CType(rowSVD("Validita_Inizio"), Date), _
                                    CType(rowSVD("Validita_Fine"), Date), _
                                      objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)

                Next

            End If

        Catch ex As Exception
            Dim msg As String
            msg = CStr(Date.Now) + " - " + nomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try


    End Sub


#End Region


End Class
