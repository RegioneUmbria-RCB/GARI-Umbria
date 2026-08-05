Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreModello.ParametriAgenda_Temp

Public Class Visite_R

    Public Function Leggi(
                       ByVal Piva As String,
                       ByVal Sa_Cod As Integer,
                       ByVal Id_Agenda As Integer,
                       ByVal xFiltroAggiuntivo As String,
                       ByVal xOrderBy As String,
                       ByRef objParametri_Server As AgronicaCoreParametri,
                       ByRef objParametri_Utenti As AgronicaCoreParametri,
                       Optional numeroRigheDaEstrattare As Integer? = Nothing
                       ) As DataTable

        Dim dal As New AgronicaCoreVisiteDAL.Visite_R
        Return dal.Leggi(Piva, Sa_Cod, Id_Agenda, xFiltroAggiuntivo, xOrderBy, objParametri_Server, objParametri_Utenti, numeroRigheDaEstrattare)

    End Function

    Public Function LeggiDettagli(
                   ByVal Piva As String,
                   ByVal Sa_Cod As Integer,
                   ByVal Id_Agenda As Integer,
                   ByVal xFiltroAggiuntivo As String,
                   ByVal xOrderBy As String,
                   ByRef objParametri_Server As AgronicaCoreParametri,
                   ByRef objParametri_Utenti As AgronicaCoreParametri,
                   Optional ByVal topNRighe As Integer? = Nothing
                   ) As DataTable

        Dim dal As New AgronicaCoreVisiteDAL.Visite_R
        Return dal.LeggiDettagli(Piva, Sa_Cod, Id_Agenda, xFiltroAggiuntivo, xOrderBy, objParametri_Server, objParametri_Utenti)

    End Function


    Public Function Leggi_Visite_APP(ByVal piva As String, ByVal username As String,
                                     ByRef objParametri_Server As AgronicaCoreParametri,
                                     ByRef objParametri_Utenti As AgronicaCoreParametri,
                                     Optional ByVal Rilievi As Boolean = False) As DataTable

        Dim dal As New AgronicaCoreVisiteDAL.Visite_R
        Return dal.LeggiVisiteAPP(piva, 0, 0, enum_WWorflow_WAnagraficaStati.QdC_Da_Eseguire, username, "", "", objParametri_Server, objParametri_Utenti, Rilievi)
    End Function


    Public Function LeggiDettagliVisite(
               ByVal Sa_Cod As Integer,
               ByVal Id_Agenda As Integer,
               ByVal xFiltroAggiuntivo As String,
               ByVal xOrderBy As String,
               ByRef objParametri_Server As AgronicaCoreParametri,
               ByRef objParametri_Utenti As AgronicaCoreParametri,
               Optional ByVal topNRighe As Integer? = Nothing
               ) As DataTable

        Dim dal As New AgronicaCoreVisiteDAL.Visite_R
        Dim listaTecniciBIZ As New AgronicaCoreAnagrafeBIZ.Contatti_MultiHost_R
        Dim dtListaTecnici As DataTable
        Dim usernames As String() = Nothing

        Dim isSuperUser As Boolean = (objParametri_Utenti.SuperUserUsername.ToLower() = objParametri_Utenti.UtenteUsername.ToLower())

        If Not isSuperUser Then
            dtListaTecnici = listaTecniciBIZ.LeggiListaTecnici(objParametri_Utenti, objParametri_Server)
            usernames = dtListaTecnici.AsEnumerable().Select(Function(r) r("username").ToString()).ToArray()
        End If

        Return dal.LeggiDettagliVisite(Sa_Cod, Id_Agenda, xFiltroAggiuntivo, xOrderBy, objParametri_Server, objParametri_Utenti, usernames)

    End Function

    Public Function LeggiDettagliVisiteNew(
               ByVal veg_cod As Integer,
               ByVal id_cod As Integer,
               ByVal usernameFilter As String,
               ByVal pivaAzienda As String,
               ByVal tipoVisita As Integer,
               ByVal dataDa As Date,
               ByVal dataA As Date,
               ByVal pivaCentro As String,
               ByVal saCodCentro As String,
               ByVal impiantiSelezionati As String(),
               ByVal operazioniFilter As Integer(),
               ByVal Gen_Cod As Integer,
               ByVal Spe_Cod As Integer,
               ByVal IPro_Cod As Integer,
               ByVal xFiltroAggiuntivo As String,
               ByVal xOrderBy As String,
               ByRef objParametri_Server As AgronicaCoreParametri,
               ByRef objParametri_Utenti As AgronicaCoreParametri,
               Optional ByVal topNRighe As Integer? = Nothing,
               Optional ByVal withDettaglioRilievo As Boolean? = False
               ) As DataTable

        Dim dal As New AgronicaCoreVisiteDAL.Visite_R
        Dim objBIZ As New AgronicaCoreAnagrafeBIZ.Contatti_MultiHost_R
        Dim dtListaTecnici As DataTable
        Dim usernames As String() = Nothing
        Dim existsImpresa As Boolean = False

        Dim result As DataTable

        Dim isSuperUser As Boolean = (objParametri_Utenti.SuperUserUsername.ToLower = objParametri_Utenti.UtenteUsername.ToLower)

        If Not isSuperUser Then
            dtListaTecnici = objBIZ.LeggiListaTecnici(objParametri_Utenti, objParametri_Server)
            usernames = dtListaTecnici.AsEnumerable().Select(Function(r) r("username").ToString()).ToArray()
            existsImpresa = objBIZ.Check_Impresa_Associata(objParametri_Utenti.UtenteUsername, objParametri_Utenti, objParametri_Server)
        End If

        If withDettaglioRilievo Then
            result = dal.LeggiDettagliVisiteNew_DettaglioRilievo(veg_cod, id_cod, usernameFilter, pivaAzienda, tipoVisita, dataDa, dataA, pivaCentro, saCodCentro, impiantiSelezionati, operazioniFilter, Gen_Cod, Spe_Cod, IPro_Cod, xFiltroAggiuntivo, xOrderBy, objParametri_Server, objParametri_Utenti, usernames, existsImpresa, topNRighe)
        Else
            result = dal.LeggiDettagliVisiteNew(veg_cod, id_cod, usernameFilter, pivaAzienda, tipoVisita, dataDa, dataA, pivaCentro, saCodCentro, impiantiSelezionati, operazioniFilter, Gen_Cod, Spe_Cod, IPro_Cod, xFiltroAggiuntivo, xOrderBy, objParametri_Server, objParametri_Utenti, usernames, existsImpresa, topNRighe)
        End If

        Return result

    End Function

    Public Function Leggi_Agenzie_Visibilita_Utente_Visite(
                                                            ByRef objParametri_Utente As AgronicaCoreParametri,
                                                            ByRef objParametri_Server As AgronicaCoreParametri,
                                                            ByVal username As String
                                                            ) As DataTable

        Dim dalVisite As New AgronicaCoreVisiteDAL.Visite_R()
        Dim objBIZ As New AgronicaCoreAnagrafeBIZ.Contatti_MultiHost_R
        Dim dtListaAziende As DataTable
        Dim existsImpresaUserLogged As Boolean
        Dim existsImpresaTecnico As Boolean

        Dim typeOperatore As enum_TipoOperatoreVisita = objBIZ.LeggiUserTecnicoOCapo(objParametri_Utente.UtenteUsername, objParametri_Server)
        existsImpresaUserLogged = objBIZ.Check_Impresa_Associata(objParametri_Utente.UtenteUsername, objParametri_Utente, objParametri_Server)
        existsImpresaTecnico = objBIZ.Check_Impresa_Associata(username, objParametri_Utente, objParametri_Server)

        dtListaAziende = dalVisite.Leggi_Agenzie_Visibilita_Utente_Visite(objParametri_Server, objParametri_Utente, username, True, typeOperatore, existsImpresaUserLogged, existsImpresaTecnico)

        Return dtListaAziende

    End Function

    Public Function LeggiRisorseZootecniche(
        ByRef objParametri_Utente As AgronicaCoreParametri,
        ByRef objParametri_Server As AgronicaCoreParametri
    ) As List(Of AgronicaCoreModelsSTD.attivita.risorse.RisorsaZootecnica)

        Dim dalRisorseZoo As New AgronicaCoreZooDAL.Lista_Indprod_Animali_R
        Dim dtRisorseZoo As DataTable

        Dim risorseZooList As New List(Of AgronicaCoreModelsSTD.attivita.risorse.RisorsaZootecnica)

        dtRisorseZoo = dalRisorseZoo.LeggiRisorseZootecniche(0, 0, -1, "", "", objParametri_Server)

        If dtRisorseZoo IsNot Nothing AndAlso dtRisorseZoo.Rows.Count > 0 Then

            For i = 0 To dtRisorseZoo.Rows.Count - 1
                Dim risorsaZoo = New AgronicaCoreModelsSTD.attivita.risorse.RisorsaZootecnica

                Dim genCod = dtRisorseZoo.Rows(i).Item("GEN_COD").ToString()
                risorsaZoo.genere = New AgronicaCoreModelsSTD.metaschema.Genere(CInt(genCod))

                Dim speCod = dtRisorseZoo.Rows(i).Item("SPE_COD").ToString()
                risorsaZoo.specie = New AgronicaCoreModelsSTD.metaschema.utilizzi.Specie(CInt(speCod))

                Dim iproCod = dtRisorseZoo.Rows(i).Item("IPRO_COD").ToString()
                risorsaZoo.indirizzoProd = New AgronicaCoreModelsSTD.metaschema.IndirizzoProduttivo(CInt(iproCod))

                risorsaZoo.descrizione = dtRisorseZoo.Rows(i).Item("ZOO_DES").ToString()

                risorseZooList.Add(risorsaZoo)
            Next
        End If

        Return risorseZooList

    End Function

    Public Shared Function getStrFaseFenologica(ByVal FF_CLASSE As String,
                                                ByRef objParametri_Server As AgronicaCoreParametri,
                                                ByRef objParametri_Utenti As AgronicaCoreParametri) As String

        Dim objMovBIZ As New AgronicaCoreContabBIZ.Movimenti_R

        Dim fasifeno As String = ""
        Dim objParametriUscitaFasiNew As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output = Nothing
        Dim objParametriUscitaFasiOld As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output = Nothing

        Dim listaDetTec As New List(Of String)

        Dim objParametriIngresso As New AgronicaCoreMetaSchemaBIZ.FasiFenologiche_input
        Dim objFasi_WS As New AgronicaCoreWebService.FasiFenologiche_WS

        Dim Filtro_cod_ss As String = ""
        Dim Filtro_ff_cod As String = ""
        Dim Hash_cod_ss As New Hashtable
        Dim Hash_ff_cod As New Hashtable
        Dim Leggi_impostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim imp As String = Leggi_impostazioni.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_FASI_FENOLOGICHE, objParametri_Utenti, 2)
        If imp = "1" Then
            objParametriIngresso.Personalizzate = True
        End If
        objParametriIngresso.Lingua_Cod = objParametri_Server.Lingua_Cod

        Select Case CInt(FF_CLASSE)
            Case < 1000
                If Not Hash_ff_cod.ContainsKey(FF_CLASSE) Then
                    Hash_ff_cod.Add(FF_CLASSE, "")
                    Filtro_ff_cod &= CInt(FF_CLASSE) & ","
                End If
            Case Else
                If Not Hash_cod_ss.ContainsKey(FF_CLASSE) Then
                    Hash_cod_ss.Add(FF_CLASSE, "")
                    Filtro_cod_ss &= CInt(FF_CLASSE) & ","
                End If
        End Select

        If Filtro_ff_cod <> "" Then
            objParametriIngresso.strFiltro = " fs.ff_cod in (" & Left(Filtro_ff_cod, Filtro_ff_cod.Length - 1) & ")"
            objParametriUscitaFasiOld = objFasi_WS.FasiFenologiche_OLD(objParametriIngresso)
        End If
        If Filtro_cod_ss <> "" Then
            objParametriIngresso.strFiltro = " ss.cod_ss in (" & Left(Filtro_cod_ss, Filtro_cod_ss.Length - 1) & ")"
            objParametriUscitaFasiNew = objFasi_WS.FasiFenologiche(objParametriIngresso)
        End If

        Dim listaFasi As New List(Of String)

        Dim Fase_Des As String = ""
        If CInt(FF_CLASSE) <> 0 Then

            Select Case CInt(FF_CLASSE)
                Case < 1000 'caso vecchio av_cod = ff_cod
                    If objParametriUscitaFasiOld IsNot Nothing Then
                        Fase_Des = (From aa In objParametriUscitaFasiOld.ListaFasiFenologiche
                                    Where aa.FF_Cod = CInt(FF_CLASSE)
                                    Select aa.Descrizione
                                            ).FirstOrDefault
                    End If

                Case Else 'caso nuovo av_cod = cod_css
                    If objParametriUscitaFasiNew IsNot Nothing Then
                        Fase_Des = (From aa In objParametriUscitaFasiNew.ListaFasiFenologiche
                                    Where aa.Cod_SS = CInt(FF_CLASSE)
                                    Select aa.Descrizione & " (BBCH " & aa.Stadio & ")"
                                            ).FirstOrDefault
                    End If

            End Select
            If Fase_Des <> "" Then
                listaFasi.Add(Fase_Des)
            End If

        End If

        fasifeno = String.Join(", ", listaFasi)

        Dim testoDetTec As String = String.Join(" - ", {fasifeno}.Where(Function(s) Not String.IsNullOrEmpty(s)))
        If testoDetTec <> "" AndAlso Not listaDetTec.Contains(testoDetTec) Then
            listaDetTec.Add(testoDetTec)
        End If

        Return String.Join(", ", listaDetTec)
    End Function

End Class

Public Class Visite_W

    Public Function EliminaRilieviVisite(ByVal Piva_visita As String,
                                         ByVal Id_Agenda_visita As Integer,
                                         ByRef objParametri_Server As AgronicaCoreParametri,
                                         ByRef objParametri_Utenti As AgronicaCoreParametri) As Boolean

        Dim objRiferimentoHelper As New Agenda_Movimenti_Dettagli_Riferimenti_Helper
        Dim objComRif = New AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R
        Dim objAgendaScrivi As New Agenda_Operazione_Helper

        Dim dtRifEsistenti = objComRif.LeggixChiave(
                                Piva_visita,
                                0,
                                Id_Agenda_visita,
                                0, 0, 0, 0, 0,
                                LAVCOD_VISITA,
                                CAU_VISITE_ISPETTIVE,
                                "",
                                objParametri_Server)

        If dtRifEsistenti IsNot Nothing AndAlso dtRifEsistenti.Rows.Count > 0 Then
            For Each drRifEsistenti As DataRow In dtRifEsistenti.Rows

                Dim pIva_rilievo = CStr(drRifEsistenti.Item("Piva_Rif"))
                Dim idAgenda_rilievo = CStr(drRifEsistenti("Id_Agenda_Rif"))
                Dim lavCod_rilievo = CStr(drRifEsistenti.Item("Lav_Cod_Rif"))

                Dim objParametriAgenda As New ParametriAgenda(False) With {
                    .Piva = pIva_rilievo,
                    .Id_Agenda = idAgenda_rilievo,
                    .Lav_Cod = lavCod_rilievo,
                    .Tipo_Operazione = CStr(enum_TipoOperazioneDB.Cancellazione)
                }

                Dim messaggio As String = ""
                Dim CancellataOperazione As Boolean = GestisciCancellazione(objParametriAgenda, objParametri_Server, messaggio, True, objParametri_Utenti:=objParametri_Utenti)
                'Dim CancellataOperazione As Boolean = objAgendaScrivi.Cancella(CStr(drRifEsistenti.Item("Piva_Rif")), 0, CInt(drRifEsistenti("Id_Agenda_Rif")), False, objParametri_Server)

                If CancellataOperazione Then
                    objRiferimentoHelper.Cancella(CStr(drRifEsistenti.Item("Piva")), 0, CInt(drRifEsistenti("Id_Agenda")), CInt(drRifEsistenti("Id_Mov")), CInt(drRifEsistenti("Id_Mov_Det")), objParametri_Server)
                End If

            Next
        End If

        Return True

    End Function
End Class
