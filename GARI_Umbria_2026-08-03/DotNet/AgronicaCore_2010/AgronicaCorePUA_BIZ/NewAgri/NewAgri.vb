Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDTOStd.OutData.NewAgri.ResponseNDistribuito
Imports AgronicaCoreDTOStd.InData.NewAgri
Imports AgronicaCoreDTOStd
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreUtility
Imports AgronicaCorePianoConcimazioneBIZ
Imports AgronicaCoreDTOStd.OutData.NewAgri
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDTOStd.InData.Agenda
Imports OutData.NewAgri
Imports AgronicaCoreModelsSTD.Utility


Public Class NewAgri

    Private Function GetPivaFromCUAA(cuaa As String, objParametri_Server As AgronicaCoreParametri) As String
        Dim xImpCodR As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        Return xImpCodR.Piva_from_CUAA(cuaa, objParametri_Server)
    End Function

    Public Function N_Distribuito(datiRequest As RequestNDistribuito,
                                  ByRef errorMessage As String,
                                  objParametri_Super_Server As AgronicaCoreParametri,
                                  objParametri_Server As AgronicaCoreParametri,
                                  objParametri_Utenti As AgronicaCoreParametri
                                  ) As AgronicaCoreDTOStd.OutData.NewAgri.ResponseNDistribuito

        Dim ResponseNDistribuito As New ResponseNDistribuito With {
                .ListaLimitiMas = New List(Of Appezzamento)
            }
        Dim AppxLimitiMas As New List(Of Appezzamento)

        Dim chiaviImpianti As New List(Of (String, Integer, Integer, Integer))

        Dim piva = GetPivaFromCUAA(datiRequest.CUAA, objParametri_Server)

        If piva = "" Then
            errorMessage = String.Format(Gias.CuaaNonEsistente, datiRequest.CUAA)
            Exit Function
        End If

        Dim regolamento_cod As String = datiRequest.Regolamento_Cod
        Dim appezzamenti As List(Of AgronicaCoreModelsSTD.NewAgri.Appezzamento) = datiRequest.ElencoAppezzamenti

        Dim filtroFerCodOrganici As String = ""
        Dim HashFerCodDigestati As New Hashtable
        Dim HashFerCodLetami As New Hashtable
        Dim HashFerCodLiquami As New Hashtable

        getFerCodLetamiLiquami(
            filtroFerCodOrganici, HashFerCodDigestati, HashFerCodLetami, HashFerCodLiquami,
            regolamento_cod, objParametri_Super_Server, objParametri_Server
        )
        getChiaviAGEAxImpiantoGIAS(appezzamenti, objParametri_Server, chiaviImpianti, Nothing, Nothing, True)

        For Each appezzamento In appezzamenti
            Dim copy As New Appezzamento
            copy = appezzamento.PropertyCopier(copy)
            AppxLimitiMas.Add(copy)
        Next

        Dim objRegImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dim dt As DataTable = objRegImpianti.Leggi_NDistribuito_PerImpianto(
            chiaviImpianti, regolamento_cod, filtroFerCodOrganici,
            HashFerCodDigestati, HashFerCodLetami, HashFerCodLiquami,
            objParametri_Server
        )

        For Each appezzamento In AppxLimitiMas
            Dim current As DataRow = dt.Select("Chiave_AGEA = '" & appezzamento.Chiave_Appezzamento & "'").FirstOrDefault
            If current IsNot Nothing Then
                appezzamento.N_FabbisognoSoddisfatto = CDec(current.Item("N_FabbisognoSoddisfatto"))
                appezzamento.N_Zootecnico = CDec(current.Item("N_FabbisognoSoddisfattoOrganico")) + CDec(current.Item("N_SoddisfattoDigestato"))
                appezzamento.N_Zootecnico_Letame = CDec(current.Item("N_Zootecnico_Letame"))
                appezzamento.N_Zootecnico_Liquame = CDec(current.Item("N_Zootecnico_Liquame"))
                appezzamento.N_BilancioAzotato_Utile = appezzamento.N_FabbisognoSoddisfatto - appezzamento.N_Mas
                appezzamento.N_TotaleSoddisfatto = CDec(current.Item("N_TotaleSoddisfatto"))
                appezzamento.N_BilancioAzotato_Totale = appezzamento.N_TotaleSoddisfatto - appezzamento.N_Mas

                ResponseNDistribuito.ListaLimitiMas.Add(appezzamento)
            End If
        Next

        Return ResponseNDistribuito
    End Function

    ''' <summary>
    ''' Basato sull'algoritmo di getChiaviAGEAxImpiantoGIAS, ma con la possibilità
    ''' di leggere anche il progetto_cod degli esercizi associatidi e gestire il
    ''' lancio delle eccezioni.
    ''' </summary>
    ''' <param name="appezzamenti">Appezzamenti con chiavi agea</param>
    ''' <param name="faultyIDs_unexistent">Eventualemnte</param>
    ''' <param name="faultyIDs_doubleAssociation"></param>
    ''' <param name="throwException"></param>
    ''' <returns>Un dizionario con le chiavi AGEA e i dati associati (piva, sa_cod, appezza, id_reg, progetto_cod)</returns>
    Private Function getChiaviAGEAxImpiantoGIAS(
        appezzamenti As List(Of AgronicaCoreModelsSTD.NewAgri.Appezzamento),
        objParametri_Server As AgronicaCoreParametri,
        Optional ByRef chiaviImpianti As List(Of (String, Integer, Integer, Integer)) = Nothing,
        Optional ByRef faultyIDs_unexistent As ICollection(Of String) = Nothing,
        Optional ByRef faultyIDs_doubleAssociation As ICollection(Of String) = Nothing,
        Optional throwException As Boolean = True,
        Optional estraiEsercizio As Boolean = False
    ) As Dictionary(Of String, (piva As String, saCod As Integer, appezza As Integer, idReg As Integer, progettoCod As Integer))

        Dim chiaviAGEAxImpiantoGIAS As New Dictionary(Of String, (String, Integer, Integer, Integer, Integer))
        Dim chiaviAGEA = appezzamenti.Select(Function(a) a.Chiave_Appezzamento).ToList

        If IsNothing(faultyIDs_unexistent) Then
            faultyIDs_unexistent = New List(Of String)
        End If
        If IsNothing(faultyIDs_doubleAssociation) Then
            faultyIDs_doubleAssociation = New List(Of String)
        End If

        Dim objRegImpiantiCodici As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R
        Dim dt = objRegImpiantiCodici.ChiaveImpianto_From_ChiaveAGEA_Massivo(chiaviAGEA, objParametri_Server, estraiEsercizio:=estraiEsercizio)

        For Each chiaveAGEA In chiaviAGEA
            Dim dummy = dt.Select("Chiave_AGEA = '" & chiaveAGEA & "'").ToList()

            If dummy IsNot Nothing AndAlso dummy.Count > 0 Then
                If dummy.Count > 1 Then
                    'plot_id associato a più impianti
                    faultyIDs_doubleAssociation.Add(chiaveAGEA)
                Else
                    Dim current = dummy(0)
                    chiaviAGEAxImpiantoGIAS.Add(
                        chiaveAGEA, (
                        current.Item("piva"),
                        current.Item("sa_cod"),
                        current.Item("appezza"),
                        current.Item("id_reg"),
                        current.Item("Progetto_Cod")
                    ))

                    If chiaviImpianti IsNot Nothing Then
                        chiaviImpianti.Add((current.Item("piva"), current.Item("sa_cod"), current.Item("appezza"), current.Item("id_reg")))
                    End If
                End If
            Else
                'Nessun impianto GIAS associato al plot_id
                faultyIDs_unexistent.Add(chiaveAGEA)
            End If
        Next

        If throwException AndAlso faultyIDs_unexistent IsNot Nothing AndAlso faultyIDs_unexistent.Count > 0 Then
            Dim strFaultyIDs As String = String.Join(", ", faultyIDs_unexistent)
            Throw New InvalidConstraintException(String.Format(Gias.ImpiantiNonEsistenti, strFaultyIDs))
        End If
        If throwException AndAlso faultyIDs_doubleAssociation IsNot Nothing AndAlso faultyIDs_doubleAssociation.Count > 0 Then
            Dim strFaultyIDs As String = String.Join(", ", faultyIDs_doubleAssociation)
            Throw New InvalidConstraintException(String.Format(Gias.ImpiantiNonUnivoci, strFaultyIDs))
        End If

        Return chiaviAGEAxImpiantoGIAS
    End Function

    Private Sub getFerCodLetamiLiquami(ByRef filtroFerCodOrganici As String,
                                       ByRef HashFerCodDigestati As Hashtable,
                                       ByRef HashFerCodLetami As Hashtable,
                                       ByRef HashFerCodLiquami As Hashtable,
                                       regolamento_cod As Integer,
                                       objParametri_Super_Server As AgronicaCoreParametri,
                                       objParametri_Server As AgronicaCoreParametri)

        Dim listaFerCodOrganici As New List(Of Integer)
        Dim HashFerCodOrganici As New Hashtable

        'recupero effluenti e relativi fertilizzanti per l'analisi dell'azoto organico
        Dim objEff As New AgronicaCorePUA_DAL.Pua_Effluente_R
        Dim DtEffluenti As DataTable = objEff.Leggi(regolamento_cod, 0, 0, " azoto_qta > 0 ", "", objParametri_Server)

        Dim objEffluentiInput As New PUA_Effluenti_input With {
            .Regolamento_Cod = regolamento_cod,
            .Includi_Efficienza_Rif = True
        }

        Dim objPC As New AgronicaCoreWebService.PianoConcimazione_WS
        Dim objEffluentiOutput As New PUA_Effluenti_output
        objEffluentiOutput = objPC.Effluenti(objEffluentiInput, objParametri_Server, objParametri_Super_Server)

        Dim Perc_Zootecnico As Decimal = 100

        If objEffluentiOutput IsNot Nothing Then
            For Each eff As PUA_Effluente In objEffluentiOutput.ListaEffluenti

                Perc_Zootecnico = 100
                If eff.MatricePrevalente = 1 AndAlso DtEffluenti IsNot Nothing AndAlso DtEffluenti.Select("eff_cod = " & eff.Eff_Cod).Length > 0 Then
                    Perc_Zootecnico = DtEffluenti.Select("eff_cod = " & eff.Eff_Cod)(0).Item("perc_zootecnico")
                End If

                If eff.SpecieAllevamento = 1 Then
                    HashFerCodOrganici.Add(eff.Fer_Cod, Perc_Zootecnico)
                    listaFerCodOrganici.Add(eff.Fer_Cod)
                End If
                If eff.MatricePrevalente = 1 Then
                    HashFerCodDigestati.Add(eff.Fer_Cod, Perc_Zootecnico)
                End If

                Select Case eff.Id_tp_fer
                    Case enum_PUA_TipoFertilizzante.Ammendante
                        HashFerCodLetami.Add(eff.Fer_Cod, Perc_Zootecnico)
                    Case enum_PUA_TipoFertilizzante.Liquame
                        HashFerCodLiquami.Add(eff.Fer_Cod, Perc_Zootecnico)
                End Select

            Next
        End If

        If listaFerCodOrganici.Count > 0 Then
            filtroFerCodOrganici = "(" & String.Join(",", listaFerCodOrganici) & ")"
        End If
    End Sub

    Private Function AggiornaN(
        mapAgeaKeys As Dictionary(Of String, (piva As String, saCod As Integer, appezza As Integer, idReg As Integer, progettoCod As Integer)),
        app As AgronicaCoreModelsSTD.NewAgri.Appezzamento,
regCod As Integer,
        params As ObjParams
    ) As UpdateNPlant
        Dim objRegImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_W
        Dim objImpProgettiW As New Impresa_Progetti_W
        Dim objLogAnagrafe As New AgronicaCoreAnagrafeDAL.AgronicaLogAnagrafe_W

        Dim re As New UpdateNPlant With {.Chiave_Appezzamento = app.Chiave_Appezzamento}

        Dim FlagConnessioneLocale, FlagTransazioneLocale As Boolean

        Try
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, params.ObjParametri_Server, System.Data.IsolationLevel.ReadUncommitted)
            Dim key = mapAgeaKeys(app.Chiave_Appezzamento)
            Dim ok As Boolean = objRegImpianti.ModificaxProgetto2(
                key.piva, key.saCod, key.appezza, key.idReg, key.progettoCod,
                enum_CodiciAnagrafe.Impianto_LimiteN, app.N_Pua,
                AGRODATAINIZIO, AGRODATAFINE,
                xFiltroAggiuntivo:=String.Empty, params.ObjParametri_Server
            )
            If ok Then
                ok = objImpProgettiW.Modifica_Parametrizzata(
                        key.piva, key.saCod, key.appezza, key.idReg, key.progettoCod,
                        "Regolamento_Concimazioni_Cod",
                        regCod,
                        xFiltroAggiuntivo:=String.Empty,
                        params.ObjParametri_Server
                     )
                If ok Then
                    ok = objLogAnagrafe.Scrivi(enum_TipoOperazioneDB.Modifica,
                                               enum_TipoEntita_Des.Progetti,
                                               CStr(key.piva), CStr(key.progettoCod),
                                               CStr(key.saCod), CStr(key.appezza),
                                               CStr(key.idReg), Nothing,
                                               $"[NewAgri] AggiornaN: Impianto_LimiteN (id_cod:{CInt(enum_CodiciAnagrafe.Impianto_LimiteN).ToString()}) = {CDec(app.N_Pua).ToString()}, Regolamento_Concimazioni_Cod = {CInt(regCod).ToString()} ",
                                               enum_Id_Servizio.GiasOnline,
                                               params.ObjParametri_Server)
                End If
            End If

            If ok Then
                ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, params.ObjParametri_Server)
                re.message = Api_Response_Message_Type.Ok
            Else
                ConnessioniTransazioni.ChiudiTransazione(2, params.ObjParametri_Server)
                re.errore = "Errore durante l'aggiornamento"
            End If
        Catch ex As Exception
            ConnessioniTransazioni.ChiudiTransazione(2, params.ObjParametri_Server)
            re.errore = "Errore durante l'aggiornamento"
        Finally
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, params.ObjParametri_Server)
        End Try

        Return re
    End Function

    Public Function Aggiorna_Impianti_N_Pua(data As RequestNDistribuito, params As ObjParams) As ResponseUpdateNPlant
        Dim keysNotFound As New List(Of String)
        Dim doubleAssociationKeys As New List(Of String)
        Dim mapAgeaKeys = getChiaviAGEAxImpiantoGIAS(
            data.ElencoAppezzamenti, params.ObjParametri_Server, Nothing,
            keysNotFound, doubleAssociationKeys, False, estraiEsercizio:=True
        )
        Dim response As New ResponseUpdateNPlant
        For Each app In data.ElencoAppezzamenti
            Dim re As UpdateNPlant
            If keysNotFound.Contains(app.Chiave_Appezzamento) Then
                re = New UpdateNPlant With {
                    .Chiave_Appezzamento = app.Chiave_Appezzamento,
                    .errore = "La chiave indicata non ha corrispondenze nel database."
                }
            ElseIf doubleAssociationKeys.Contains(app.Chiave_Appezzamento) Then
                re = New UpdateNPlant With {
                    .Chiave_Appezzamento = app.Chiave_Appezzamento,
                    .errore = "La chiave indicata non risulta univoca nel database!"
                }
            Else
                re = AggiornaN(mapAgeaKeys, app, data.Regolamento_Cod, params)
            End If
            response.ElencoAppezzamenti.Add(re)
        Next
        Return response
    End Function

    Public Function ImportUtenteNewAgri(utente As RequestUtente, params As ObjParams) As ResponseUtente
        Dim response As New ResponseUtente

        response.result = True
        response.error = ""

        Try
            Dim objUtentiR As New AgronicaCoreUtentiDAL.Utenti_Read
            Dim dtUtenti = objUtentiR.Leggi2(utente.username, "", "", params.ObjParametri_Utenti)

            If utente.bloccato = True Then

                If dtUtenti.Rows.Count > 0 Then
                    Dim objUtentiPermessiR As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
                    Dim strFiltroData = "Validita_Fine > " & UtilityProvider.Agro_SQL_SaveDate(Date.Now)
                    Dim dtUtentiPermessi = objUtentiPermessiR.Leggi(utente.username, enum_Id_Servizio.GiasOnline, 0, 9999, 0, strFiltroData, "", params.ObjParametri_Utenti)

                    If dtUtentiPermessi.Rows.Count > 0 Then
                        Dim objUtentiPermessiW As New AgronicaCoreUtentiDAL.Utenti_Permessi_W
                        If Not objUtentiPermessiW.Modifica_Validita(utente.username, enum_Id_Servizio.GiasOnline, AGRODATAINIZIO, Date.Now, strFiltroData, params.ObjParametri_Utenti) Then
                            response.result = False
                            response.error = "Errore durante l'importazione: utente non bloccato."
                        End If
                    Else
                        response.result = False
                        response.error = "Utente già bloccato."
                    End If
                Else
                    response.result = False
                    response.error = "Utente da bloccare non esistente a sistema."
                End If

            ElseIf dtUtenti.Rows.Count > 0 Then

                Dim objUtentiW As New AgronicaCoreUtentiDAL.Utenti_Write
                Dim objUtentiDettagliW As New AgronicaCoreUtentiDAL.Utenti_Dettagli_W
                If Not (objUtentiDettagliW.Modifica_Parametrizzata(utente.username, "Cognome", utente.cognome, "", params.ObjParametri_Utenti) And
                        objUtentiDettagliW.Modifica_Parametrizzata(utente.username, "Nome", utente.nome, "", params.ObjParametri_Utenti) And
                        objUtentiDettagliW.Modifica_Parametrizzata(utente.username, "Email", utente.mail, "", params.ObjParametri_Utenti) And
                        objUtentiW.ModificaFlagSPID(utente.username, utente.spid, params.ObjParametri_Utenti)) Then
                    response.result = False
                    response.error = "Errore durante l'importazione: utente non aggiornato."
                End If

                Dim objUtentiPermessiR As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
                Dim strFiltroData = "Validita_Fine <= " & UtilityProvider.Agro_SQL_SaveDate(Date.Now)
                Dim dtUtentiPermessi = objUtentiPermessiR.Leggi(utente.username, enum_Id_Servizio.GiasOnline, 0, 9999, 0, strFiltroData, "", params.ObjParametri_Utenti)

                If dtUtentiPermessi.Rows.Count > 0 Then
                    Dim objUtentiPermessiW As New AgronicaCoreUtentiDAL.Utenti_Permessi_W
                    If Not (objUtentiPermessiW.Cancella(utente.username, 0, 0, 0, 0, "", params.ObjParametri_Utenti) And
                            objUtentiPermessiW.ScriviDaTipologia(utente.username, "", params.ObjParametri_Utenti)) Then
                        response.result = False
                        response.error = "Errore durante l'importazione: permessi utenti non aggiornati."
                    End If
                End If

            Else

                Dim Sistema_Esterno_NewAgri As Integer = TipiEnumerativi.enum_SistemiEsterni.NewAgri
                Dim Tipologia_Cod As Integer = 0

                Dim objUtentiTipologiexRuoliSistemiEsterni As New AgronicaCoreUtentiDAL.Utenti_TipologiexRuoli_SistemiEsterni_R
                Dim strRuoli As String = String.Join(",", utente.ruoli)
                Dim dtUtentiTipologiexSistemiEsterni = objUtentiTipologiexRuoliSistemiEsterni.Leggi(Sistema_Esterno_NewAgri, strRuoli, "", "", params.ObjParametri_Utenti)

                If dtUtentiTipologiexSistemiEsterni.Rows.Count > 0 Then
                    Tipologia_Cod = dtUtentiTipologiexSistemiEsterni.Rows(0)("Tipologia_Cod")

                Else
                    dtUtentiTipologiexSistemiEsterni = objUtentiTipologiexRuoliSistemiEsterni.Leggi(Sistema_Esterno_NewAgri, "", "", "", params.ObjParametri_Utenti)
                    If dtUtentiTipologiexSistemiEsterni.Rows.Count > 0 Then
                        For Each ruolo In utente.ruoli
                            For Each row In dtUtentiTipologiexSistemiEsterni.Rows
                                If ruolo = row("Ruolo_SistemaEsterno") Then
                                    Tipologia_Cod = row("Tipologia_Cod")
                                    Exit For
                                End If
                            Next
                            If Tipologia_Cod > 0 Then
                                Exit For
                            End If
                        Next
                    End If
                End If

                If Tipologia_Cod > 0 Then
                    Dim objGruppiUtenteR As New AgronicaCoreUtentiDAL.Gruppi_Utente_R
                    Dim strFiltroGruppo As String = "Gruppi_Utente_Identificativo = '" & UtilityProvider.Agro_SQL_SaveText(params.ObjParametri_Utenti.PivaSuperUser) + "' "
                    Dim dtGruppiUtente = objGruppiUtenteR.Leggi(0, strFiltroGruppo, "", params.ObjParametri_Utenti)

                    If dtGruppiUtente.Rows.Count > 0 Then
                        Dim gruppoUtentiCod As Integer = dtGruppiUtente.Rows(0)("Gruppi_Utente_cod")
                        Dim objUserWriter As New AgronicaCoreUtentiDAL.UserWriter
                        If Not objUserWriter.CreateFromImportNewAgriData(utente.username, utente.nome, utente.cognome, utente.codice_Fiscale, utente.mail, Tipologia_Cod, params, setFlagAccessoSpid:=utente.spid, gruppoUtenti:=gruppoUtentiCod) Then
                            response.result = False
                            response.error = "Errore durante l'importazione: nuovo utente non creato."
                        End If
                    Else
                        response.result = False
                        response.error = "Gruppo utenti da assegnare non trovato."
                    End If
                Else
                    response.result = False
                    response.error = "Nessuna tipologia di utente trovata per i ruoli specificati."
                End If
            End If

        Catch ex As Exception
            response.result = False
            response.error = ex.Message
        End Try

        Return response
    End Function

End Class
