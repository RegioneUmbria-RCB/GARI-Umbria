Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDTOStd.InData.Gis
Imports AgronicaCoreModelsSTD.baseClass
Imports AgronicaCoreModelsSTD.profilazione
Imports AgronicaCoreModelsSTD.utente
Imports AgronicaCoreUtentiBIZ.My.Resources


Public Class Tipologie

    Public Function Carica_Tipologie(objParametri_Utenti As AgronicaCoreParametri) As List(Of TipologiaUtente)
        Dim objUtentiDAL As New AgronicaCoreUtentiDAL.Utenti_Tipologie_R
        Dim getStringOrDefault = Function(row As DataRow, field As String) If(IsDBNull(row(field)), String.Empty, row(field))
        Dim tipologie = objUtentiDAL.Leggi(0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                 xFiltroAggiuntivo:=String.Empty, xOrderBy:=String.Empty, objParametri_Utenti).Select.
            Select(Function(row) New TipologiaUtente(row("Tipologia_Cod"), row("Tipologia_Des")) With
            {.Note = getStringOrDefault(row, "Note")}).ToList
        Return tipologie
    End Function

    Public Sub Scrivi_Tipologie(ByRef Tipologie As TipologiaUtente(), params As ObjParams)
        Dim objTipologie = New AgronicaCoreUtentiDAL.Utenti_Tipologie_W
        Dim objNuovoId As New AgronicaCoreDataProvider.Agro_Sequenze
        If IsNothing(params.ObjParametri_Utenti) OrElse IsNothing(params.ObjParametri_Server) Then
            Throw New ArgumentNullException("Argument params must have both objParametri_Utenti and ObjParametri_Server valorized.")
        End If
        For Each Profilo In Tipologie
            If ControllaTipologiaEsiste(Profilo, params.ObjParametri_Utenti) Then
                objTipologie.Modifica(Profilo.codice, Profilo.descrizione,
                                      "", params.ObjParametri_Utenti, Profilo.Note)
            Else
                Profilo.codice = objNuovoId.NuovoId_Tabella("Utenti_Tipologie", 0, 2000000000, params.objParametri_Server)
                objTipologie.Scrivi(Profilo.codice, Profilo.descrizione,
                                    AGRODATAINIZIO, AGRODATAFINE,
                                    params.ObjParametri_Utenti, Profilo.Note)
            End If
        Next
    End Sub

    Public sub CopiaTipologia(original As TipologiaUtente, newProfile As TipologiaUtente, copySettings As Boolean, params As ObjParams)
        Try
            Dim objTipologiaxPermessi_W As New AgronicaCoreUtentiDAL.Utenti_TipologiexPermessi_W
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, params.ObjParametri_Utenti)

            Scrivi_Tipologie({newProfile}, params)
            objTipologiaxPermessi_W.CopiaDaAltraTipologia(original.codice, newProfile.codice, params.ObjParametri_Utenti)
            
            If copySettings Then
                Dim settingsWriter As New AgronicaCoreUtentiBIZ.Utenti_Impostazioni_W
                settingsWriter.CopiaImpostazioni({newProfile.codice}, original.codice, params.ObjParametri_Server, params.ObjParametri_Utenti, False)
            End If

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(1, params.ObjParametri_Utenti)
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(params.ObjParametri_Utenti)
        Catch ex As Exception
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, params.ObjParametri_Utenti)
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(params.ObjParametri_Utenti)
            Throw
        End Try
    End sub

    ''' <summary>
    ''' Deletes a profile along with its permissions and its settings.
    ''' </summary>
    ''' <param name="Tipologia_Cod">Identity code of the profile to delete</param>
    ''' <exception cref="InvalidOperationException">If the profile still has some users attached to it.</exception>
    Public Sub Elimina_Tipologia(Tipologia_Cod As Integer,
                                 objParametri_Server As AgronicaCoreParametri,
                                 objParametri_Utenti As AgronicaCoreParametri)
        Dim ErrMsg As String = ""
        Dim objTipologie As New AgronicaCoreUtentiDAL.Utenti_Tipologie_W
        Dim objTipologiaxPermessi_W As New AgronicaCoreUtentiDAL.Utenti_TipologiexPermessi_W
        Dim objImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_W

        ErrMsg += ControllaNoUtentiCollegati(Tipologia_Cod, objParametri_Utenti)
        If ErrMsg <> "" Then
            Throw New InvalidOperationException(ErrMsg)
        End If

        AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, objParametri_Utenti)

        objTipologiaxPermessi_W.Cancella(Tipologia_Cod, "", objParametri_Utenti)
        objImpostazioni.CancellaPerUtente(New List(Of String) From { Tipologia_Cod.ToString() }, objParametri_Utenti)
        objTipologie.Cancella(Tipologia_Cod, "", objParametri_Utenti)

        AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Utenti)
        AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri_Utenti)
    End Sub

    Public Sub Aggiorna_Permessi_Tipologia(
        Tipologia_Cod As Integer, Attivita As BaseCodeDescr(),
        Id_Operazione As Integer, params As ObjParams
    )
        Dim objTipologiaxPermessi_W As New AgronicaCoreUtentiDAL.Utenti_TipologiexPermessi_W
        Dim newPerms = GetProfileNewPermissions(Tipologia_Cod, Attivita, Id_Operazione, params.ObjParametri_Utenti)
        Try
            '----- APRO CONNESSIONE AL DATABASE
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, params.ObjParametri_Utenti)

            ' Sovrascrivo i permessi della tipologia
            params.ObjParametri_Utenti.FlagCancellazioneLogica = 0
            objTipologiaxPermessi_W.Cancella(Tipologia_Cod, "", params.ObjParametri_Utenti)

            For Each permesso In newPerms
                objTipologiaxPermessi_W.Scrivi(
                    Tipologia_Cod, enum_Id_Servizio.GiasOnline,
                    permesso.Permesso_ID, permesso.Permesso_Tipo,
                    AGRODATAINIZIO, AGRODATAFINE, params.ObjParametri_Utenti
                )
            Next
            '----- CHIUDO CONNESSIONE AL DATABASE
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(1, params.ObjParametri_Utenti)
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(params.ObjParametri_Utenti)

            AggiornaPermessiUtentiCollegati(New TipologiaUtente(Tipologia_Cod, String.Empty), params)
        Catch ex As Exception
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, params.ObjParametri_Utenti)
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(params.ObjParametri_Utenti)
            Throw
        End Try
    End Sub

    ''' <summary>
    ''' Aggiorna i permessi degli utenti collegati alla tipologia specificata.
    ''' La funzione riporta i permessi correnti della tipologia a tutti gli utenti collegati.
    ''' Eventuali permessi previamente abilitati per gli utenti interessati dall'operazione
    ''' vengono eliminati in favore di quelli della tipologia.
    ''' </summary>
    ''' <param name="tipologia">Tipologia utente per cui si vogliono aggiornare i permessi degli utenti</param>
    ''' <param name="flagInitTransizione">Definisce se eseguire l'operazione in transizione. Default True.</param>
    Public Sub AggiornaPermessiUtentiCollegati(tipologia As TipologiaUtente, params As ObjParams, Optional flagInitTransizione As Boolean = True)
        Dim objUtentiBIZ = New AgronicaCoreUtentiBIZ.Utenti
        Dim utenti As List(Of UtentePermessi) = getUtentiCollegati(tipologia, params.ObjParametri_Utenti).ToList()
        objUtentiBIZ.AssociaProfilo(utenti, tipologia, False, params, flagInitTransizione)
    End Sub

    Public Sub AggiornaImpostzioniUtentiCollegati(indata As AssociaProfiloObj, params As ObjParams, Optional flagInitTransizione As Boolean = True)
        Dim scriviImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_W
        Dim users = getUtentiCollegati(indata.Profilo, params.ObjParametri_Utenti)
        Dim settingsCodes = indata.impostazioni.Select(Function(x) x.Impostazione_Cod)
        scriviImpostazioni.ScriviDaTipologiaMassivo(users, String.Empty, params.ObjParametri_Utenti, impostazioni:=settingsCodes)
    End Sub

    Public Function Carica_Permessi_Gerarchia(
        objParametri_Server As AgronicaCoreParametri,
        objParametri_Utenti As AgronicaCoreParametri
    ) As List(Of Utente_Permesso_Gerarchia)
        Dim utentiPermessi As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R
        Dim objTipologiePermessi = New AgronicaCoreUtentiDAL.Utenti_TipologiexPermessi_R

        Dim getStringOrDefault = Function(row As DataRow, field As String) If(IsDBNull(row(field)), String.Empty, row(field))
        Dim getIntOrDefault = Function(row As DataRow, field As String) If(IsDBNull(row(field)), 0, row(field))

        Dim TB_Permessi As DataTable = objTipologiePermessi.Leggi_Gerarchia_Permessi(objParametri_Server, objParametri_Utenti)
        Dim permessiCurrUt = utentiPermessi.Leggi_conPermessi(
            enum_Id_Servizio.GiasOnline, Id_Attivita:=-1, enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
            " Utenti_Dettagli.UserName LIKE '" & objParametri_Utenti.UtenteUsername & "' ", String.Empty, objParametri_Utenti
        ).Select.
        Select(Function(row) New Utente_Permesso(row("Id_Attivita"), row("Id_Operazione")))

        Dim nuova_gerarchia = TB_Permessi.Select.
            Where(Function(row) permessiCurrUt.Any(Function(p) p.Permesso_ID = getIntOrDefault(row, "attivita"))).
            Select(Function(row) New Utente_Permesso_Gerarchia With {
                .Attivita_Cod = getIntOrDefault(row, "attivita"),
                .Attivita_Des = getStringOrDefault(row, "NoteAgg"),
                .MenuPrimoLivello = New BaseCodeDescr(getIntOrDefault(row, "sezionepadre"), getStringOrDefault(row, "Padre")),
                .MenuSecondoLivello = New BaseCodeDescr(0, getStringOrDefault(row, "Funzioni")),
                .Ordinamento = getStringOrDefault(row, "ordinamento")
            }).ToList

        Return nuova_gerarchia
    End Function

    Public Function Carica_PermessixTipologia(Tipologia_Cod As Integer, params As ObjParams) As List(Of Utente_Permesso_Gerarchia)
        Return Carica_PermessixTipologia(Tipologia_Cod, params.ObjParametri_Server, params.ObjParametri_Utenti)
    End Function

    Public Function Carica_PermessixTipologia(
        Tipologia_Cod As Integer,
        objParametri_Server As AgronicaCoreParametri,
        objParametri_Utenti As AgronicaCoreParametri
    ) As List(Of Utente_Permesso_Gerarchia)

        'Dim USA_NUOVA_FUNZIONE_CARICAMENTO = True
        Dim objPermessi = New AgronicaCoreUtentiDAL.Utenti_TipologiexPermessi_R
        Dim soloLettura = 0
        Dim letturaScrittura = 2

        Dim TB_Permessi As DataTable = objPermessi.Leggi(Tipologia_Cod,
                enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                xFiltroAggiuntivo:=String.Empty, xOrderBy:=String.Empty, objParametri_Utenti
            )
        Dim pr = TB_Permessi.Select.
            GroupBy(Function(row) row("Id_Attivita")).
            Select(Function(gr) New With {.Id_Attivita = gr.Key, .count = gr.Count}).
            ToDictionary(Function(entry) entry.Id_Attivita)


        Dim GerarchiaPermessi = Carica_Permessi_Gerarchia(objParametri_Server, objParametri_Utenti).ToList()
        Dim Items As List(Of Utente_Permesso_Gerarchia) = GerarchiaPermessi.AsParallel.
            Where(Function(p) pr.ContainsKey(p.Attivita_Cod)).
            Select(Function(p)
                       If pr(p.Attivita_Cod).count = 1 Then
                           p.Id_Operazione = soloLettura
                       ElseIf pr(p.Attivita_Cod).count = 2 Then
                           p.Id_Operazione = letturaScrittura
                       End If
                       Return p
                   End Function).ToList
        Return Items
    End Function

    Public Function CaricaPermessiTipologie(params As ObjParams) As IEnumerable(Of Object)
        If IsNothing(params.ObjParametri_Utenti) OrElse IsNothing(params.ObjParametri_Server) Then
            Throw New NullReferenceException("Both objParametri_Utenti and ObjParametri_Server must be valorized")
        End If

        Dim tipologieDAL As New AgronicaCoreUtentiDAL.Utenti_TipologiexPermessi_R
        Dim GerarchiaPermessi = Carica_Permessi_Gerarchia(params.ObjParametri_Server, params.ObjParametri_Utenti)
        Dim tpDT = tipologieDAL.LeggiTutteTipologie(String.Empty, String.Empty, params.ObjParametri_Utenti).
            Select.AsParallel

        Dim getIdAttivita = Function(row As DataRow) If(IsDBNull(row("Id_Attivita")), 0, CInt(row("Id_Attivita")))
        Dim toVisualize = tpDT.Where(Function(row) GerarchiaPermessi.Exists(Function(p) p.Attivita_Cod = getIdAttivita(row)))

        Dim xType = toVisualize.GroupBy(Function(row) CInt(row("Tipologia_Cod"))).
            Select(Function(txp) New With {
                .Tipologia = New TipologiaUtente(txp.Key, txp.First.Item("Tipologia_Des")),
                .rows = txp.AsEnumerable
            })
        Dim parsed = xType.Select(Function(trows) New With {
                .Tipologia = trows.Tipologia,
                .Permessi = extractFromGerarchia(GerarchiaPermessi, trows.rows)
            }).ToList

        Return parsed
    End Function

    Private Function extractFromGerarchia(ByVal gerarchia As List(Of Utente_Permesso_Gerarchia), ByVal rows As IEnumerable(Of DataRow)) As List(Of Utente_Permesso_Gerarchia)
        Dim permissions = gerarchia.Where(Function(p) rows.Any(Function(row) row("Id_Attivita") = p.Attivita_Cod)).
            Select(Function(p) New With {
                 .permesso = p,
                 .row = rows.First(Function(row) row("Id_Attivita") = p.Attivita_Cod)
             }).ToList
        Dim applyAuth = Function(byval p As Utente_Permesso_Gerarchia, auth As enum_TipoOperazioneDB)
                            Dim copy = p.CreateCopy()
                            copy.Permesso_Tipo = auth
                            Return copy
                        End Function

        dim Rperm = permissions.Where(Function(x) x.row("Tipo") = 1).
            select(function(x) applyAuth(x.permesso, enum_TipoOperazioneDB.Lettura)).ToList()
        dim Wperm = permissions.Where(Function(x) x.row("Tipo") = 2).
            select(function(x) applyAuth(x.permesso, enum_TipoOperazioneDB.Modifica)).ToList()
        dim RWperm = permissions.Where(Function(x) x.row("Tipo") = 3).
            select(function(x) applyAuth(x.permesso, enum_TipoOperazioneDB.Modifica)).ToList()

        Return Wperm.Union(Rperm).union(RWperm).ToList
    End Function

    Public Function HaPermessiCollegati(TipologiaCod As Integer, objUtenti As AgronicaCoreParametri)
        Dim objPermessi = New AgronicaCoreUtentiDAL.Utenti_TipologiexPermessi_R
        Dim permessiTipologia = objPermessi.Leggi(
            TipologiaCod, enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
            String.Empty, String.Empty, objUtenti
        )
        Return permessiTipologia IsNot Nothing AndAlso permessiTipologia.Rows.Count > 0
    End Function

    Public Function HaImpostazioniCollegate(TipologiaCod As Integer, objUtenti As AgronicaCoreParametri)
        Dim objImpostazioni = New AgronicaCoreUtentiDAL.Utenti_Tipologie_R
        Dim impostazioniTipologia = objImpostazioni.LeggiImpostazioniCollegate(
            TipologiaCod, xFiltroAggiuntivo:=String.Empty, xOrderBy:=String.Empty, objUtenti
        )
        Return impostazioniTipologia IsNot Nothing AndAlso impostazioniTipologia.Rows.Count > 0
    End Function

#Region "Funzioni Private"

    Private Sub AddIfAbsent(ByRef List As IEnumerable(Of IPermesso), ByVal Value As IPermesso)
        If Not List.Any(Function(p) IsSamePermission(p, Value)) Then
            List = List.Append(Value)
        End If
    End Sub

    Private Sub RemoveIfPresent(ByRef List As IEnumerable(Of IPermesso), ByVal Value As IPermesso)
        If List.Any(Function(p) IsSamePermission(p, Value)) Then
            List = List.Where(Function(p) Not IsSamePermission(p, Value))
        End If
    End Sub

    Private Function IsSamePermission(p1 As IPermesso, p2 As IPermesso) As Boolean
        Return p1.Permesso_ID = p2.Permesso_ID AndAlso p1.Permesso_Tipo = p2.Permesso_Tipo
    End Function

    Private Function GetProfileNewPermissions(
        profileCod As Integer, permissions As IEnumerable(Of IBaseCodeDescr),
        operation As enum_TipoOperazioneDB, objParametri_Utenti As AgronicaCoreParametri
    ) As IEnumerable(Of IPermesso)
        Dim objTipologiaxPermessi_R As New AgronicaCoreUtentiDAL.Utenti_TipologiexPermessi_R
        Dim installation = GetInstallationPermissions(objParametri_Utenti).ToList
        Dim current As IEnumerable(Of IPermesso) = objTipologiaxPermessi_R.Leggi(
            profileCod, enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
            String.Empty, String.Empty, objParametri_Utenti
        ).Select.
        Select(Function(r) New Utente_Permesso(r.Item("Id_Attivita"), r.Item("Id_Operazione")))

        Select Case operation
            Case enum_TipoOperazioneDB.Modifica
                For Each permesso In permissions
                    AddIfAbsent(current, New Utente_Permesso(permesso.codice, enum_TipoOperazioneDB.Lettura))
                    AddIfAbsent(current, New Utente_Permesso(permesso.codice, enum_TipoOperazioneDB.Modifica))
                Next

            Case enum_TipoOperazioneDB.Lettura
                For Each permesso In permissions
                    RemoveIfPresent(current, New Utente_Permesso(permesso.codice, enum_TipoOperazioneDB.Modifica))
                    AddIfAbsent(current, New Utente_Permesso(permesso.codice, enum_TipoOperazioneDB.Lettura))
                Next

            Case enum_TipoOperazioneDB.Cancellazione
                current = New List(Of Utente_Permesso)

            Case Else
                For Each permesso In permissions
                    RemoveIfPresent(current, New Utente_Permesso(permesso.codice, enum_TipoOperazioneDB.Modifica))
                    RemoveIfPresent(current, New Utente_Permesso(permesso.codice, enum_TipoOperazioneDB.Lettura))
                Next
        End Select

        If installation.Any Then
            current = current.Where(Function(p) installation.Exists(Function(active) p.Permesso_ID = active.Permesso_ID AndAlso p.Permesso_Tipo <= active.Permesso_Tipo))
        End If
        Return current
    End Function

    Private Function GetInstallationPermissions(objParametri_Utenti As AgronicaCoreParametri) As IEnumerable(Of IPermesso)
        Dim objPermesso_R As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim installationPermissions As IEnumerable(Of IPermesso) = New List(Of Utente_Permesso)
        If objPermesso_R.VerificaEsistenzaTabellaClientePermessi(objParametri_Utenti) Then
            installationPermissions = objPermesso_R.LeggiCliente_Permessi(
                objParametri_Utenti.SuperUserUsername,
                String.Empty, String.Empty, objParametri_Utenti
            ).Select.
            Select(Function(dr) New Utente_Permesso(dr.Item("Id_Attivita"), dr.Item("Id_Operazione"))).
            ToList
        End If
        Return installationPermissions
    End Function

    Private Function ControllaNoUtentiCollegati(Tipologia_Cod As Integer, objParametri_Utenti As AgronicaCoreParametri) As String
        Dim Utenti As IEnumerable(Of UtentePermessi) = getUtentiCollegati(
            New TipologiaUtente() With {.codice = Tipologia_Cod}, objParametri_Utenti
        )
        If Utenti.Count <> 0 Then
            Return ResProfilazione.ErrTipologiaCollegata & vbCrLf
        Else
            Return ""
        End If
    End Function

    Private Function ControllaTipologiaEsiste(ByVal tipologia As TipologiaUtente, objParametri As AgronicaCoreParametri) As Boolean
        Dim ListaTipologie = Carica_Tipologie(objParametri)
        Return ListaTipologie.Where(Function(p) p.codice = tipologia.codice).Any()
    End Function

    Public Function getUtentiCollegati(tipologia As TipologiaUtente, objPUtenti As AgronicaCoreParametri) As IEnumerable(Of UtentePermessi)
        Dim readUtenti As New AgronicaCoreUtentiDAL.Utenti_Read
        Return readUtenti.Leggi(
                enumSelezioneVariabile.Selezione_TabellaCompleta,
                " Tipologia_Cod = " & tipologia.codice, "", objPUtenti
            ).Select.AsParallel.
            Select(Function(row) New UtentePermessi With {
                .UserName = row.Item("UserName"),
                .Tipologia = tipologia
            })
    End Function

#End Region

End Class
