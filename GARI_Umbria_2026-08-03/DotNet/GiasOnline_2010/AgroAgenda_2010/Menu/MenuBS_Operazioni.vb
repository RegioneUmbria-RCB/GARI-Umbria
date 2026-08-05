
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class MenuBS_Operazioni




    Public Shared Function Gestione_Operazione( _
            ByVal id_Agenda As String, _
            ByVal UtenteAbilitato_Modifica As Boolean, _
            ByVal MenuAgenda_SelectedValue As String, _
            ByVal ASG_Utente_Username As String, _
            ByVal ASG_IdServizio As String, _
            ByVal Data As String, _
            ByVal Lav_Cod As String, _
            ByVal Blocco_Flag As String, _
            ByVal Piva As String, _
            ByVal veg_cod As String, _
            ByVal sa_cod As String, _
            ByVal objParametri_Server As AgronicaCoreParametri, _
            ByVal objParametri_Utenti As AgronicaCoreParametri, _
            ByVal PaginaRitorno As TipiEnumerativi.enum_PagineAgenda_2010 _
        ) As String


        Dim rval As String = ""
        Dim objParametriAgenda As New ParametriAgenda

        'Abilito / Disabilito il menu
        If Not UtenteAbilitato_Modifica Then
            If MenuAgenda_SelectedValue = "1" Or
                MenuAgenda_SelectedValue = "2" Or
                MenuAgenda_SelectedValue = "3" Or
                MenuAgenda_SelectedValue = "4" Or
                MenuAgenda_SelectedValue = "5" Or
                MenuAgenda_SelectedValue = "5b" Or
                MenuAgenda_SelectedValue = "5c" Or
                MenuAgenda_SelectedValue = "5d" Or
                MenuAgenda_SelectedValue = "6b" Or
                MenuAgenda_SelectedValue = "6c" Or
                MenuAgenda_SelectedValue = "6d" Then
                rval &= "Non si hanno i permessi per questa operazione!"
                Return "ERR" & rval
                'Exit Function
            End If
        End If


        Dim TargetUrl As String = ""

        'TODO, impostare tutte queste variabili        

        Dim Data2 As Date
        Dim gru_cod As String = ""
        Dim TIPO As String = ""
        Dim Lav_Des As String = ""

        Dim Dt_Operazioni As DataTable



        Select Case MenuAgenda_SelectedValue

            '################################################
            '#####  INFO - MODIFICA  ########################
            '################################################

            Case "0", "2"   'info - modifica



                'verifico permesso op contabili e magazzino
                'permessi op contabili e magazzino
                'TODO
                'Dim permesso As Boolean = Utility_NS.Utility_Operazioni.PermessiOpContabiliEMagazzino(Lav_Cod, MenuAgenda_SelectedValue, objParametri_Server, objParametri_Utenti, Session)
                'If permesso = False Then
                '    rval &= "Non si hanno i permessi per questa operazione su questo gruppo di operazioni"
                '    Exit Function
                'End If


                If Blocco_Flag <> 1 Or (Blocco_Flag = 1 AndAlso (MenuAgenda_SelectedValue = "0" OrElse Lav_Cod = "4500")) Then

                    objParametriAgenda.Data = Data
                    objParametriAgenda.Id_Agenda = id_Agenda
                    objParametriAgenda.Piva = Piva
                    objParametriAgenda.Sa_Cod = sa_cod
                    objParametriAgenda.Lav_Cod = Lav_Cod
                    objParametriAgenda.TipoOperazioneAgenda = TipiEnumerativi.enum_Tipo_Operazione_Agenda.QuadernoDiCampagna
                    objParametriAgenda.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Reale
                    objParametriAgenda.Programmazione_Cod = 0

                    objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
                    objParametriAgenda.PaginaSitoOrigine = PaginaRitorno

                    If MenuAgenda_SelectedValue = "0" Then
                        objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Lettura
                    Else
                        If veg_cod > 0 Then


                            Dim objSpecVeg As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
                            Dim Dt As DataTable = objSpecVeg.SpecieVegetali_GestioneFiltroUtente_Leggi(veg_cod,
                                                                             0,
                                                                             "",
                                                                             "",
                                                                             "",
                                                                             "",
                                                                             objParametri_Utenti)
                            If Dt.Rows.Count = 0 Then
                                objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Lettura
                                rval &= Resources.AgronicaAgenda_2010.NoModificaNoPermessoSpecie
                                Return "ERR" & rval
                                'Exit Function
                            End If

                        End If

                        objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica

                    End If

                    Dim OpUtil As New AgronicaCoreModello.Utility_Operazioni
                    TargetUrl = OpUtil.LinkPagina_from_LavCod_NEW(Lav_Cod, objParametriAgenda,
                                                                  PaginaSitoAgendaOrigine:=PaginaRitorno, strErrore:=rval)

                    'Se dalla creazione link errore ritorna un errore, esco
                    If rval <> "" Then
                        TargetUrl = ""
                        Return "ERR " & rval
                    End If

                    If objParametriAgenda.Lav_Cod = LAVCOD_CURA Then
                        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
                        Dim UtenteAbilitato As Boolean = objPermessi.Controlla_Permessi_Utente(
                                                    ASG_Utente_Username,
                                                    ASG_IdServizio,
                                                    enum_Security_Attivita.Agenda_Operazione_Di_Cura,
                                                    enum_Security_Operazione.Lettura,
                                                    Date.Now,
                                                    "",
                                                    objParametri_Utenti)
                        If Not UtenteAbilitato Then
                            TargetUrl = ""
                            rval &= "Non si hanno i permessi per l'operazione di cura"
                            Return "ERR" & rval
                            'Exit Function
                        End If
                    End If

                    'impedisco di modificare una raccolta senza selezionare prima il centro
                    If objParametriAgenda.Lav_Cod = LAVCOD_RACCOLTA AndAlso objParametriAgenda.Sa_Cod = "0" Then

                        Dim cf As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                        Dim dt As DataTable = cf.Leggi(0, "RaccoltaNew", " valore = 'true' ", "", HttpContext.Current.Session("ASG_objParametri_Server"))
                        If dt.Rows.Count = 1 Then
                            'raccolta new

                        Else
                            TargetUrl = ""
                            rval &= Resources.AgronicaAgenda_2010.PerRaccoltaSelezionaCentro
                            Return "ERR" & rval
                            'Exit Function
                        End If



                    End If

                    'Controllo blocchi se non sono in modifica
                    If MenuAgenda_SelectedValue <> "0" Then

                        Try
                            Dim matrice_delete(,) As String
                            If ControllaOperazione(matrice_delete, objParametriAgenda, objParametri_Server, "", False, objParametri_Utenti:=objParametri_Utenti) Then
                            End If
                        Catch ex As Exception
                            rval &= ex.Message
                            Return "ERR" & rval
                            'Exit Function
                        End Try

                    End If

                Else

                    'alert blocco
                    'If MenuAgenda_SelectedValue = "2" Then
                    rval &= Resources.AgronicaAgenda_2010.ImpossibileModificareOperazioneBlocc
                    Return "ERR" & rval
                    ' Exit Function
                    ' End If

                End If


        End Select


        If TargetUrl <> "" Then
            rval = TargetUrl
        Else
            rval = "ERR" & Resources.AgronicaAgenda_2010.OperazioneInManutenzione
        End If


        Return rval

    End Function



End Class
