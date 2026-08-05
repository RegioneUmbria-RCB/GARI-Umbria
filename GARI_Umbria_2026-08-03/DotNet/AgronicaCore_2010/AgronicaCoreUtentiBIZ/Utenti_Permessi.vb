Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModelsSTD.baseClass
Imports AgronicaCoreModelsSTD.utente

Public Class Utenti_Permessi_R
    Public Function LeggiPemerssiUtenteEntitaGis(ByVal username As String,
                                                 ByRef ObjParametri_Utenti As AgronicaCoreParametri) As AgronicaCoreDTOStd.InData.Gis.PermessiUtenteEntitaGIS

        Dim ret As New AgronicaCoreDTOStd.InData.Gis.PermessiUtenteEntitaGIS
        Dim objUtentiPerm As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Try
            Dim dtp = objUtentiPerm.Leggi(ObjParametri_Utenti.UtenteUsername,
                                          5,
                                          0,
                                          9999,
                                          1,
                                          " Id_Attivita in (" & enum_Security_Attivita.Anagrafica_CentroAziendale & "," & enum_Security_Attivita.Anagrafica_Fabbricato & "," & enum_Security_Attivita.Anagrafica_Campo & "," & enum_Security_Attivita.Anagrafica_Appezzamento & "," & enum_Security_Attivita.Anagrafica_Impianto & ") and Id_Operazione in (" & enum_Security_Operazione.Lettura & "," & enum_Security_Operazione.Modifica & ") ",
                                          "",
                                          ObjParametri_Utenti)
            For Each row In dtp.Rows
                Select Case row("Id_Attivita")

                    Case enum_Security_Attivita.Anagrafica_CentroAziendale
                        Select Case row("Id_Operazione")
                            Case enum_Security_Operazione.Lettura
                                ret.centriAziendali.Lettura = True
                            Case enum_Security_Operazione.Modifica
                                ret.centriAziendali.Scrittura = True
                        End Select
                    Case enum_Security_Attivita.Anagrafica_Fabbricato
                        Select Case row("Id_Operazione")
                            Case enum_Security_Operazione.Lettura
                                ret.fabbricati.Lettura = True
                            Case enum_Security_Operazione.Modifica
                                ret.fabbricati.Scrittura = True
                        End Select
                    Case enum_Security_Attivita.Anagrafica_Campo
                        Select Case row("Id_Operazione")
                            Case enum_Security_Operazione.Lettura
                                ret.campi.Lettura = True
                            Case enum_Security_Operazione.Modifica
                                ret.campi.Scrittura = True
                        End Select
                    Case enum_Security_Attivita.Anagrafica_Appezzamento
                        Select Case row("Id_Operazione")
                            Case enum_Security_Operazione.Lettura
                                ret.appezzamenti.Lettura = True
                            Case enum_Security_Operazione.Modifica
                                ret.appezzamenti.Scrittura = True
                        End Select
                    Case enum_Security_Attivita.Anagrafica_Impianto
                        Select Case row("Id_Operazione")
                            Case enum_Security_Operazione.Lettura
                                ret.impianti.Lettura = True
                            Case enum_Security_Operazione.Modifica
                                ret.impianti.Scrittura = True
                        End Select
                End Select
            Next
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try
        Return ret

    End Function

    ''' <summary>
    ''' Carica la gerarchia permessi mostrando solo i permessi visibili all'utente attuale.
    ''' </summary>
    ''' <param name="filtraAttivi">Se True filtra i permessi caricando solo quelli gestibili dal cliente.</param>
    Public Function Carica_Permessi_Gerarchia(params As ObjParams, filtraAttivi As Boolean) As List(Of Utente_Permesso_Gerarchia)
        Dim objUtenti = New Utenti
        Dim utentiPermessi As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R
        Dim objPermesso_R As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

        Dim permessiCurrUt = utentiPermessi.Leggi_conPermessi(
                enum_Id_Servizio.GiasOnline, Id_Attivita:=-1, enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                " Utenti_Dettagli.UserName LIKE '" & params.ObjParametri_Utenti.UtenteUsername & "' ",
                String.Empty, params.ObjParametri_Utenti
            ).Select.
            Select(Function(row) New Utente_Permesso(row("Id_Attivita"), row("Id_Operazione")))

        Dim visibileCurrUser = Carica_Permessi_Gerarchia_Completa(params).ToList

        If filtraAttivi AndAlso objPermesso_R.VerificaEsistenzaTabellaClientePermessi(params.ObjParametri_Utenti) Then
            Dim idAttivi As IEnumerable(Of Integer) = objUtenti.LeggiCliente_Permessi(
                params.ObjParametri_Utenti.SuperUserUsername, params.ObjParametri_Utenti
                ).Select(Function(a) a.Permesso_ID).ToHashSet
            'Se tabella Cliente_Permessi è vuota non vedo nulla
            visibileCurrUser = visibileCurrUser.Where(Function(p) idAttivi.Contains(p.Permesso_ID)).ToList
        Else
            visibileCurrUser = visibileCurrUser.Where(Function(r) permessiCurrUt.Any(Function(p) p.Permesso_ID = r.Permesso_ID)).ToList
        End If

        Return visibileCurrUser
    End Function

    ''' <summary>
    ''' Carica tutti i permessi secondo la gerarchia. NON esegue filtri sui permessi caricati
    ''' </summary>
    ''' <seealso cref="Carica_Permessi_Gerarchia(ObjParams, Boolean)"/>
    Public Function Carica_Permessi_Gerarchia_Completa(params As ObjParams) As List(Of Utente_Permesso_Gerarchia)
        Dim objTipologiePermessi = New AgronicaCoreUtentiDAL.Utenti_TipologiexPermessi_R

        Dim getStringOrDefault = Function(row As DataRow, field As String) If(IsDBNull(row(field)), String.Empty, row(field))
        Dim getIntOrDefault = Function(row As DataRow, field As String) If(IsDBNull(row(field)), 0, row(field))

        Dim permessi = objTipologiePermessi.Leggi_Gerarchia_Permessi(params.ObjParametri_Server, params.ObjParametri_Utenti).
            Select.
            Select(Function(row) New Utente_Permesso_Gerarchia With {
                .Attivita_Cod = getIntOrDefault(row, "attivita"),
                .Attivita_Des = getStringOrDefault(row, "NoteAgg"),
                .MenuPrimoLivello = New BaseCodeDescr(getIntOrDefault(row, "sezionepadre"), getStringOrDefault(row, "Padre")),
                .MenuSecondoLivello = New BaseCodeDescr(0, getStringOrDefault(row, "Funzioni")),
                .Ordinamento = getStringOrDefault(row, "ordinamento")
            }).ToList

        Return permessi
    End Function

End Class

Public Class Utenti_Permessi_W

    ''' <summary>
    ''' Disabilita un permesso a livello globale rimuovendolo sia da Utenti_Permessi che da Utenti_TipologiexPermessi.
    ''' </summary>
    ''' <param name="permessi">Permessi da rimuovere</param>
    ''' <param name="operazione">Operazione da disabilitare</param>
    Public Sub DisabilitaGlobale(permessi As IEnumerable(Of IPermesso), operazione As enum_TipoPermesso, objParametri_Utenti As AgronicaCoreParametri)
        Dim objPermesso_W As New AgronicaCoreUtentiDAL.Utenti_Permessi_W
        Dim permessiTipologie As New AgronicaCoreUtentiDAL.Utenti_TipologiexPermessi_W
        Dim idAttivitaList = permessi.Select(Function(p) p.Permesso_ID)
        If operazione <> enum_TipoPermesso.DISABILITATO Then
            objPermesso_W.CancellaMultiplo(
                   enum_Id_Servizio.GiasOnline, idAttivitaList,
                   operazione, 0, String.Empty, objParametri_Utenti
               )
            permessiTipologie.CancellaMultiplo(
                enum_Id_Servizio.GiasOnline, idAttivitaList,
                operazione, String.Empty, objParametri_Utenti
            )
        End If
    End Sub

End Class