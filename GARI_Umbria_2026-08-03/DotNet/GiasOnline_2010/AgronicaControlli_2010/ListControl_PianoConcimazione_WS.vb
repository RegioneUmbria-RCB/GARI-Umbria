
Imports System.Data
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class ListControl_PianoConcimazione_WS
    Public Shared Sub PUA_Regolamento_WS(ByRef Controllo As ListControl,
            ByVal PrimaRiga_Flag As Boolean,
            ByVal PrimaRiga_Text As String,
            ByVal PrimaRiga_Value As String,
            ByVal Tipo As Int32,
            ByVal TipoMetodo As Int32,
            ByVal xFiltroAggiuntivo As String,
            ByVal xOrderBy As String,
            Optional ByVal DataInizio As Date = AGRODATAINIZIO,
            Optional ByVal DataFine As Date = AGRODATAFINE,
            Optional ByVal VisualizzaPrivati As Boolean = False,
            Optional ByVal Piva_Superuser As String = ""
    )

        Dim Dt As New DataTable
        Dim i As Integer = 0

        Controllo.Items.Clear()

        If PrimaRiga_Flag = True Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Regolamenti_input
        objParametriIngresso.strFiltro = xFiltroAggiuntivo
        objParametriIngresso.Tipo = Tipo
        objParametriIngresso.TipoMetodo = TipoMetodo
        objParametriIngresso.VisualizzaPrivati = VisualizzaPrivati
        objParametriIngresso.Piva_Superuser = Piva_Superuser
        objParametriIngresso.strOrdinamento = xOrderBy
        objParametriIngresso.DataInizio = DataInizio
        objParametriIngresso.DataFine = DataFine

        Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Regolamenti_output
        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        objParametriUscita = objPC_WS.Regolamenti(objParametriIngresso)

        For i = 0 To objParametriUscita.ListaRegolamenti.Count - 1
            Controllo.Items.Add(New ListItem(objParametriUscita.ListaRegolamenti(i).Descrizione,
                                            objParametriUscita.ListaRegolamenti(i).Codice))
        Next


    End Sub

    ''' <remarks>
    ''' Ripropone i dati caricati da <see cref="PUA_Regolamento_WS"/> utilizzand come default per il caricamento i valori indicati nella pagina delle impostazioni utente.
    ''' Usata nel caricamento dei regolamenti selezionabili nelle impostazioni utente, categoria nitrati.
    ''' </remarks>
    ''' <param name="objServer">Introdotto per poter caricare l'url di riferimento dalle impostazioni (introdotto 29/01/2024).</param>
    ''' <param name="objSuperServer">Introdotto per poter caricare l'url di riferimento dalle impostazioni (introdotto 29/01/2024).</param>
    Public Shared Function PUA_Regolamento_WS(
            objServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
            objSuperServer As AgronicaCoreDataProvider.AgronicaCoreParametri
    )
        Dim Dt As New DataTable
        Dim i As Integer = 0

        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Regolamenti_input
        Dim objAgroWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig(False, objServer, objSuperServer)

        objParametriIngresso.strFiltro = " ( Tipo=1 OR Tipo=3 ) "
        objParametriIngresso.Tipo = 0
        objParametriIngresso.TipoMetodo = 0
        objParametriIngresso.VisualizzaPrivati = False
        objParametriIngresso.Piva_Superuser = objServer.PivaSuperUser
        objParametriIngresso.strOrdinamento = " Ordine desc "
        objParametriIngresso.DataInizio = AGRODATAINIZIO
        objParametriIngresso.DataFine = AGRODATAFINE

        objParametriIngresso.url = objAgroWebConfig.GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione

        Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Regolamenti_output
        objParametriUscita = objPC_WS.Regolamenti(objParametriIngresso)

        Return objParametriUscita.ListaRegolamenti.
            Select(Function(reg) New AgronicaCoreModelsSTD.baseClass.BaseCodeDescr(reg.Codice, reg.Descrizione)).
            ToList()
    End Function

    Public Shared Function PUA_Regolamento_Da_Ente(Ente_Cod As String,
                                ByVal Tipo As Int32,
                                ByVal TipoMetodo As Int32,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                Optional ByVal DataInizio As Date = AGRODATAINIZIO,
                                Optional ByVal DataFine As Date = AGRODATAFINE
                                ) As Integer

        Dim Dt As New DataTable
        Dim i As Integer = 0

        Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Regolamenti_input
        objParametriIngresso.strFiltro = xFiltroAggiuntivo
        objParametriIngresso.Tipo = Tipo
        objParametriIngresso.TipoMetodo = TipoMetodo
        objParametriIngresso.strOrdinamento = xOrderBy
        objParametriIngresso.DataInizio = DataInizio
        objParametriIngresso.DataFine = DataFine

        Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Regolamenti_output
        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        objParametriUscita = objPC_WS.Regolamenti(objParametriIngresso)

        For i = 0 To objParametriUscita.ListaRegolamenti.Count - 1
            'If Ente_Cod = objParametriUscita.ListaRegolamenti(i).idEnte Then
            '    Return objParametriUscita.ListaRegolamenti(i).Codice
            'End If
        Next

        Return 0

    End Function

    'temporaneo cambia le descrizioni in attesa di collegare il regolamento dpi al regolamento_pua
    Public Shared Sub PUA_Regolamento_WS_xAgenda(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                                 ByVal Includi_Biologico As Boolean,
                                ByVal Tipo As Int32,
                                                 ByVal DataInizio As Date,
                                                 ByVal DataFine As Date,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String
                                )

        Dim Dt As New DataTable
        Dim i As Integer = 0

        Controllo.Items.Clear()

        If PrimaRiga_Flag = True Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If
        If Includi_Biologico = True Then
            Controllo.Items.Add(New ListItem(Descrizione_Regolamento_Bio, Tipo_Regolamento_Bio))
        End If


        Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Regolamenti_input
        objParametriIngresso.Tipo = Tipo
        objParametriIngresso.DataInizio = DataInizio
        objParametriIngresso.DataFine = DataFine
        objParametriIngresso.strFiltro = xFiltroAggiuntivo

        Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Regolamenti_output
        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        objParametriUscita = objPC_WS.Regolamenti(objParametriIngresso)

        Dim Desc As String
        For i = 0 To objParametriUscita.ListaRegolamenti.Count - 1
            Desc = ""
            Select Case objParametriUscita.ListaRegolamenti(i).Tipo
                Case enum_PUARegolamenti_Tipo.PianoComcimazione, enum_PUARegolamenti_Tipo.PUA
                    Desc = objParametriUscita.ListaRegolamenti(i).Descrizione
                    'Case enum_PUARegolamenti_Tipo.PUA
                    '    Desc = "Direttiva Nitrati"
                Case Else
                    Continue For
            End Select

            Controllo.Items.Add(New ListItem(Desc,
                        objParametriUscita.ListaRegolamenti(i).Codice & "/" & objParametriUscita.ListaRegolamenti(i).Tipo))
        Next


    End Sub

    Public Shared Sub PUA_Effluenti_WS(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                 ByVal Regolamento_Cod As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                ByVal objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim Dt As New DataTable
        Dim i As Integer = 0

        Controllo.Items.Clear()

        If PrimaRiga_Flag = True Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PUA_Effluenti_input
        objParametriIngresso.Regolamento_Cod = Regolamento_Cod

        Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PUA_Effluenti_output
        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        objParametriUscita = objPC_WS.Effluenti(objParametriIngresso, objParametri_Server, objParametri_Super_Server)

        For i = 0 To objParametriUscita.ListaEffluenti.Count - 1
            Controllo.Items.Add(New ListItem(objParametriUscita.ListaEffluenti(i).Eff_Des,
                                            objParametriUscita.ListaEffluenti(i).Eff_Cod))
        Next


    End Sub

    Public Shared Sub PUA_EffluentiXFrequenza_GetEffluente_WS(ByRef Controllo As ListControl,
                                                              ByVal PrimaRiga_Flag As Boolean,
                                                              ByVal PrimaRiga_Text As String,
                                                              ByVal PrimaRiga_Value As String,
                                                              ByVal Regolamento_Cod As Integer,
                                                              ByVal xFiltroAggiuntivo As String,
                                                              ByVal xOrderBy As String,
                                                              Optional ByVal url As String = "")

        Dim i As Integer = 0

        Controllo.Items.Clear()

        If PrimaRiga_Flag = True Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PUA_EffluentiXFrequenza_input
        objParametriIngresso.Regolamento_Cod = Regolamento_Cod
        objParametriIngresso.Eff_Cod = 0
        objParametriIngresso.ID_Fre = 0
        objParametriIngresso.Url = url

        Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PUA_EffluentiXFrequenza_output
        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        objParametriUscita = objPC_WS.EffluentiXFrequenza(objParametriIngresso)

        'potrei avere dei doppioni che devo scartare
        Dim hashCod As New Hashtable

        For Each item In objParametriUscita.ListaEffluentiXFrequenza

            Dim codice As Integer = item.Eff_Cod
            Dim descrizione As String = item.Eff_Des

            If Not hashCod.ContainsKey(codice) Then
                Controllo.Items.Add(New ListItem(descrizione, codice))

                hashCod.Add(codice, descrizione)
            End If

        Next

    End Sub


    Public Shared Sub PC_PrecessioneColturale_WS(ByRef Controllo As ListControl,
                                                 ByVal PrimaRiga_Flag As Boolean,
                                                 ByVal PrimaRiga_Text As String,
                                                 ByVal PrimaRiga_Value As String,
                                                 ByVal Regolamento_Cod As Integer,
                                                 ByVal Pua_Tipo As Integer,
                                                 ByVal xFiltroAggiuntivo As String,
                                                 ByVal xOrderBy As String,
                                                 Optional ByVal url As String = "")

        Dim i As Integer = 0

        Controllo.Items.Clear()

        If PrimaRiga_Flag = True Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Precessione_input
        objParametriIngresso.Regolamento_Cod = Regolamento_Cod
        objParametriIngresso.PUA_Tipo = Pua_Tipo
        objParametriIngresso.Url = url

        Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Precessione_output
        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        objParametriUscita = objPC_WS.Precessione(objParametriIngresso)

        For i = 0 To objParametriUscita.ListaPrecessione.Count - 1
            Controllo.Items.Add(New ListItem(objParametriUscita.ListaPrecessione(i).Descrizione,
                                            objParametriUscita.ListaPrecessione(i).Codice))
        Next

    End Sub

    Public Shared Sub PC_Ubicazione_WS(ByRef Controllo As ListControl,
                                       ByVal PrimaRiga_Flag As Boolean,
                                       ByVal PrimaRiga_Text As String,
                                       ByVal PrimaRiga_Value As String,
                                       ByVal Regolamento_Cod As Integer,
                                       ByVal xFiltroAggiuntivo As String,
                                       ByVal xOrderBy As String,
                                       Optional ByVal url As String = "")

        Dim i As Integer = 0

        Controllo.Items.Clear()

        If PrimaRiga_Flag = True Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Ubicazione_input
        objParametriIngresso.Regolamento_Cod = Regolamento_Cod
        objParametriIngresso.Url = url

        Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Ubicazione_output
        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        objParametriUscita = objPC_WS.Ubicazione(objParametriIngresso)

        For i = 0 To objParametriUscita.ListaUbicazione.Count - 1
            Controllo.Items.Add(New ListItem(objParametriUscita.ListaUbicazione(i).Descrizione,
                                            objParametriUscita.ListaUbicazione(i).Codice))
        Next


    End Sub

    Public Shared Sub PC_TipoAcqua_WS(ByRef Controllo As ListControl,
                                      ByVal PrimaRiga_Flag As Boolean,
                                      ByVal PrimaRiga_Text As String,
                                      ByVal PrimaRiga_Value As String,
                                      ByVal Regolamento_Cod As Integer,
                                      ByVal TipoZona As String,
                                      ByVal xFiltroAggiuntivo As String,
                                      ByVal xOrderBy As String,
                                      Optional ByVal url As String = "")

        Controllo.Items.Clear()

        If PrimaRiga_Flag = True Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_TipoAcqua_input With {
            .Regolamento_Cod = Regolamento_Cod,
            .TipoZona = TipoZona,
            .Url = url
        }

        Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_TipoAcqua_output
        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        objParametriUscita = objPC_WS.TipoAcqua(objParametriIngresso)

        'potrei avere dei doppioni che devo scartare
        Dim hashCod As New Hashtable

        For Each item In objParametriUscita.ListaTipiAcqua

            Dim codice As Integer = item.Codice
            Dim descrizione As String = item.Descrizione

            If Not hashCod.ContainsKey(codice) Then
                Controllo.Items.Add(New ListItem(descrizione, codice))

                hashCod.Add(codice, descrizione)
            End If

        Next

    End Sub

    Public Shared Sub PC_DisponibilitaOssigeno_WS(ByRef Controllo As ListControl,
                                               ByVal PrimaRiga_Flag As Boolean,
                                               ByVal PrimaRiga_Text As String,
                                               ByVal PrimaRiga_Value As String,
                                               ByVal Regolamento_Cod As Int32,
                                               ByVal xFiltroAggiuntivo As String,
                                               ByVal xOrderBy As String
                                               )

        Dim Dt As New DataTable
        Dim i As Integer = 0

        Controllo.Items.Clear()

        If PrimaRiga_Flag = True Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_DisponibilitaOssigeno_input
        objParametriIngresso.Regolamento_Cod = Regolamento_Cod

        Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_DisponibilitaOssigeno_output
        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        objParametriUscita = objPC_WS.DisponibilitaOssigeno(objParametriIngresso)

        For i = 0 To objParametriUscita.ListaDisponibilitaOssigeno.Count - 1
            Controllo.Items.Add(New ListItem(objParametriUscita.ListaDisponibilitaOssigeno(i).Descrizione,
                                            objParametriUscita.ListaDisponibilitaOssigeno(i).Codice))
        Next

    End Sub

    Public Shared Sub PC_Frequenza_WS(ByRef Controllo As ListControl,
                                      ByVal PrimaRiga_Flag As Boolean,
                                      ByVal PrimaRiga_Text As String,
                                      ByVal PrimaRiga_Value As String,
                                      ByVal Regolamento_Cod As Integer,
                                      ByVal xFiltroAggiuntivo As String,
                                      ByVal xOrderBy As String,
                                      Optional ByVal url As String = "")

        Dim i As Integer = 0

        Controllo.Items.Clear()

        If PrimaRiga_Flag = True Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Frequenza_input
        objParametriIngresso.Regolamento_Cod = Regolamento_Cod
        objParametriIngresso.Url = url

        Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Frequenza_output
        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        objParametriUscita = objPC_WS.Frequenza(objParametriIngresso)

        For i = 0 To objParametriUscita.ListaFrequenza.Count - 1
            Controllo.Items.Add(New ListItem(objParametriUscita.ListaFrequenza(i).Descrizione,
                                            objParametriUscita.ListaFrequenza(i).Codice))
        Next

    End Sub

    Public Shared Sub PC_MatriciOrganiche_WS(ByRef Controllo As ListControl,
                                             ByVal PrimaRiga_Flag As Boolean,
                                             ByVal PrimaRiga_Text As String,
                                             ByVal PrimaRiga_Value As String,
                                             ByVal Regolamento_Cod As Integer,
                                             ByVal xFiltroAggiuntivo As String,
                                             ByVal xOrderBy As String,
                                             Optional ByVal url As String = "")

        Dim i As Integer = 0

        Controllo.Items.Clear()

        If PrimaRiga_Flag = True Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_MatriciOrganiche_input
        objParametriIngresso.Regolamento_Cod = Regolamento_Cod
        objParametriIngresso.Url = url

        Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_MatriciOrganiche_output
        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        objParametriUscita = objPC_WS.MatriciOrganiche(objParametriIngresso)

        For i = 0 To objParametriUscita.ListaMatriciOrganiche.Count - 1
            Controllo.Items.Add(New ListItem(objParametriUscita.ListaMatriciOrganiche(i).Descrizione,
                                            objParametriUscita.ListaMatriciOrganiche(i).Codice))
        Next

    End Sub

    Public Shared Sub Finalita_Rer_WS(ByRef Controllo As ListControl,
                                   ByVal PrimaRiga_Flag As Boolean,
                                   ByVal PrimaRiga_Text As String,
                                   ByVal PrimaRiga_Value As String,
                                   ByVal Regolamento_Cod As Integer,
                                   ByVal Veg_Cod As Integer,
                                   ByVal Grfi_Cod As Integer,
                                   ByVal Cerca_GrfiDes As String,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByVal xOrderBy As String,
                                   Optional ByVal url As String = ""
                                   )


        Dim Dt As New DataTable
        Dim i As Integer = 0

        Controllo.Items.Clear()

        If PrimaRiga_Flag = True Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FinalitaRER_input
        objParametriIngresso.Regolamento_Cod = Regolamento_Cod
        objParametriIngresso.Veg_Cod = Veg_Cod
        objParametriIngresso.Grfi_Cod = Grfi_Cod
        objParametriIngresso.Url = url

        Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FinalitaRER_output
        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        objParametriUscita = objPC_WS.FinalitaRER(objParametriIngresso)

        For i = 0 To objParametriUscita.ListaFinalita.Count - 1
            Controllo.Items.Add(New ListItem(objParametriUscita.ListaFinalita(i).Descrizione,
                                            objParametriUscita.ListaFinalita(i).Codice))
        Next

    End Sub

    Public Shared Sub Finalita_Rer_BPerc_WS(ByRef Controllo As ListControl,
                                   ByVal PrimaRiga_Flag As Boolean,
                                   ByVal PrimaRiga_Text As String,
                                   ByVal PrimaRiga_Value As String,
                                   ByVal Regolamento_Cod As Integer,
                                   ByVal Veg_Cod As Integer,
                                   ByVal Grfi_Cod As Integer,
                                   ByVal Cerca_GrfiDes As String,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByVal xOrderBy As String,
                                   Optional ByVal url As String = ""
                                   )


        Dim Dt As New DataTable
        Dim i As Integer = 0

        Controllo.Items.Clear()

        If PrimaRiga_Flag = True Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PUA_CoefficienteB_Coltura_input
        objParametriIngresso.Regolamento_Cod = Regolamento_Cod
        objParametriIngresso.Veg_Cod = Veg_Cod
        objParametriIngresso.Grfi_Cod = Grfi_Cod
        objParametriIngresso.Url = url

        Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PUA_CoefficienteB_Coltura_output
        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        objParametriUscita = objPC_WS.CoefficienteB_Coltura(objParametriIngresso)

        'potrei avere dei doppioni che devo scartare
        Dim hashCod As New Hashtable

        For i = 0 To objParametriUscita.ListaCoefficienteB.Count - 1
            If Not hashCod.ContainsKey(objParametriUscita.ListaCoefficienteB(i).Grfi_Cod_RER & "|" & objParametriUscita.ListaCoefficienteB(i).B_Perc) Then
                Controllo.Items.Add(New ListItem(objParametriUscita.ListaCoefficienteB(i).Grfi_Des_RER,
                                            objParametriUscita.ListaCoefficienteB(i).Grfi_Cod_RER & "|" & objParametriUscita.ListaCoefficienteB(i).B_Perc))
                hashCod.Add(objParametriUscita.ListaCoefficienteB(i).Grfi_Cod_RER & "|" & objParametriUscita.ListaCoefficienteB(i).B_Perc, "")
            End If
        Next

    End Sub

    Public Shared Sub Finalita_Rer_BPercNMasResa_WS(ByRef Controllo As ListControl,
                                   ByVal PrimaRiga_Flag As Boolean,
                                   ByVal PrimaRiga_Text As String,
                                   ByVal PrimaRiga_Value As String,
                                   ByVal Regolamento_Cod As Integer,
                                   ByVal Veg_Cod As Integer,
                                   ByVal Grfi_Cod As Integer,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByVal xOrderBy As String,
                                   Optional ByVal url As String = ""
                                   )


        Dim Dt As New DataTable
        Dim i As Integer = 0

        Controllo.Items.Clear()

        If PrimaRiga_Flag = True Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PUA_CoefficienteB_Coltura_input
        objParametriIngresso.Regolamento_Cod = Regolamento_Cod
        objParametriIngresso.Veg_Cod = Veg_Cod
        objParametriIngresso.Grfi_Cod = Grfi_Cod
        objParametriIngresso.Includi_NMasResa = True
        objParametriIngresso.Url = url

        Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PUA_CoefficienteB_Coltura_output
        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        objParametriUscita = objPC_WS.CoefficienteB_Coltura(objParametriIngresso)

        'potrei avere dei doppioni che devo scartare
        Dim hashCod As New Hashtable

        For i = 0 To objParametriUscita.ListaCoefficienteB.Count - 1

            If Not hashCod.ContainsKey(objParametriUscita.ListaCoefficienteB(i).Grfi_Cod_RER & "|" &
                                       objParametriUscita.ListaCoefficienteB(i).B_Perc & "|" &
                                       objParametriUscita.ListaCoefficienteB(i).N & "|" &
                                       objParametriUscita.ListaCoefficienteB(i).Resa & "|" &
                                       objParametriUscita.ListaCoefficienteB(i).FattoreCorrettivo_N) Then

                Controllo.Items.Add(New ListItem(objParametriUscita.ListaCoefficienteB(i).Grfi_Des_RER,
                                            objParametriUscita.ListaCoefficienteB(i).Grfi_Cod_RER & "|" &
                                       objParametriUscita.ListaCoefficienteB(i).B_Perc & "|" &
                                       objParametriUscita.ListaCoefficienteB(i).N & "|" &
                                       objParametriUscita.ListaCoefficienteB(i).Resa & "|" &
                                       objParametriUscita.ListaCoefficienteB(i).FattoreCorrettivo_N))

                hashCod.Add(objParametriUscita.ListaCoefficienteB(i).Grfi_Cod_RER & "|" &
                                       objParametriUscita.ListaCoefficienteB(i).B_Perc & "|" &
                                       objParametriUscita.ListaCoefficienteB(i).N & "|" &
                                       objParametriUscita.ListaCoefficienteB(i).Resa & "|" &
                                       objParametriUscita.ListaCoefficienteB(i).FattoreCorrettivo_N, "")

            End If
        Next

    End Sub


    Public Shared Sub PC_FasiCicloColturale_WS(ByRef Controllo As ListControl,
                                               ByVal PrimaRiga_Flag As Boolean,
                                               ByVal PrimaRiga_Text As String,
                                               ByVal PrimaRiga_Value As String,
                                               ByVal Regolamento_Cod As Integer,
                                               ByVal Veg_Cod As Integer,
                                               ByVal xFiltroAggiuntivo As String,
                                               ByVal xOrderBy As String,
                                               Optional ByVal url As String = "")

        Dim i As Integer = 0

        Controllo.Items.Clear()

        If PrimaRiga_Flag = True Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FasiCicloColturale_input
        objParametriIngresso.Regolamento_Cod = Regolamento_Cod
        objParametriIngresso.Veg_Cod = Veg_Cod
        objParametriIngresso.Url = url

        Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FasiCicloColturale_output
        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        objParametriUscita = objPC_WS.FasiCicloColturale(objParametriIngresso)

        For i = 0 To objParametriUscita.ListaFasi.Count - 1
            Controllo.Items.Add(New ListItem(objParametriUscita.ListaFasi(i).Descrizione,
                                            objParametriUscita.ListaFasi(i).Codice))
        Next

    End Sub

    Public Shared Sub PC_SpecieConcimazione_WS(ByRef Controllo As ListControl,
                                              ByVal PrimaRiga_Flag As Boolean,
                                              ByVal PrimaRiga_Text As String,
                                              ByVal PrimaRiga_Value As String,
                                              ByVal Regolamento_Cod As Integer,
                                              ByVal xFiltroAggiuntivo As String,
                                              ByVal xOrderBy As String
                                              )

        Dim Dt As New DataTable
        Dim i As Integer = 0

        Controllo.Items.Clear()

        If PrimaRiga_Flag = True Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_SpecieConcimazione_input
        objParametriIngresso.Regolamento_Cod = Regolamento_Cod

        Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_SpecieConcimazione_output
        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        objParametriUscita = objPC_WS.SpecieConcimazione(objParametriIngresso)

        For i = 0 To objParametriUscita.ListaSpecieConcimazione.Count - 1
            Controllo.Items.Add(New ListItem(objParametriUscita.ListaSpecieConcimazione(i).Descrizione,
                                            objParametriUscita.ListaSpecieConcimazione(i).Codice))
        Next

    End Sub

    Public Shared Sub PUA_FasiCicloColturale_WS(ByRef Controllo As ListControl,
                                               ByVal PrimaRiga_Flag As Boolean,
                                               ByVal PrimaRiga_Text As String,
                                               ByVal PrimaRiga_Value As String,
                                               ByVal Regolamento_Cod As Integer,
                                               ByVal Veg_Cod As Integer,
                                               ByVal xFiltroAggiuntivo As String,
                                               ByVal xOrderBy As String,
                                               Optional ByVal url As String = "")

        Dim i As Integer = 0

        Controllo.Items.Clear()

        If PrimaRiga_Flag = True Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FasiCicloColturale_input
        objParametriIngresso.Regolamento_Cod = Regolamento_Cod
        objParametriIngresso.Veg_Cod = Veg_Cod
        objParametriIngresso.Url = url

        Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FasiCicloColturale_output
        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        objParametriUscita = objPC_WS.FasiCicloColturaleStatoImpianto(objParametriIngresso)

        For i = 0 To objParametriUscita.ListaFasi.Count - 1
            Controllo.Items.Add(New ListItem(objParametriUscita.ListaFasi(i).Descrizione,
                                            objParametriUscita.ListaFasi(i).Codice))
        Next

    End Sub

    Public Shared Sub PUA_EpocheModalita_WS(ByRef Controllo As ListControl,
                            ByVal PrimaRiga_Flag As Boolean,
                            ByVal PrimaRiga_Text As String,
                            ByVal PrimaRiga_Value As String,
                                               ByVal Regolamento_Cod As Integer,
                                               ByVal Veg_Cod As Integer,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String
                            )

        Dim i As Integer = 0

        Controllo.Items.Clear()

        If PrimaRiga_Flag = True Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_EpocheModalitaxSpecie_input
        objParametriIngresso.Specie_Cod = Veg_Cod
        objParametriIngresso.Regolamento_Cod = Regolamento_Cod

        Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_EpocheModalitaxSpecie_output
        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        objParametriUscita = objPC_WS.EpocheModalitaxSpecie(objParametriIngresso)

        For i = 0 To objParametriUscita.ListaEpocheModalitaxSpecie.Count - 1
            Controllo.Items.Add(New ListItem(objParametriUscita.ListaEpocheModalitaxSpecie(i).Epoca_Des,
                                            objParametriUscita.ListaEpocheModalitaxSpecie(i).Epoca_Cod))
        Next


    End Sub

End Class
