Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreModelsSTD.baseClass
Imports AgronicaCoreModelsSTD.profilazione

Public Class WTransazioniDiStatoConfigurazione

#Region "Lettura"

    ''' <summary>
    ''' Carica i servizi per le pratiche visualizzati durante la scelta delle
    ''' transizione di stato utilizzzabili da un gruppo utente.
    ''' </summary>
    ''' <returns></returns>
    Public Function LeggiServiziPratiche(objParametri_Server As AgronicaCoreParametri) As IEnumerable(Of BaseCodeDescr)
        Dim noSpecifica = ""
        Dim tuttiServizi = 0
        Dim objServizi As New AgronicaCoreMetaSchemaDAL.Servizi_R
        Dim DT = objServizi.Leggi(
            tuttiServizi, noSpecifica, AGRODATAINIZIO, AGRODATAFINE,
            noSpecifica, noSpecifica, objParametri_Server
        )
        Return DT.AsEnumerable().
            Select(Function(row) New BaseCodeDescr(row.Item("Servizio_Cod"), row.Item("Servizio_Des")))
    End Function

    ''' <summary>
    ''' Carica le transizioni di stato utilizzabili in riferimento a un particolare servizio.
    ''' </summary>
    ''' <param name="codiceServizio">Codice del servizio per cui caricare le transizioni di stato utilizzabili</param>
    ''' <returns></returns>
    Public Function LeggiTransizioniServizio(codiceServizio As Integer, objParametri_Server As AgronicaCoreParametri) As IEnumerable(Of BaseCodeDescrStr)
        Dim noSpecifica = ""
        Dim noOrigineDestinazione = 0
        Dim objTransizioniStato As New AgronicaCoreMetaSchemaDAL.WTransizioniDiStatoConfigurazione_R
        Dim DT = objTransizioniStato.Leggi(
            codiceServizio, noOrigineDestinazione, noOrigineDestinazione,
            AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
            noSpecifica, noSpecifica, objParametri_Server
        )
        Return DT.AsEnumerable().
            Select(Function(row) New BaseCodeDescrStr(
                row.Item("Stato_Origine_Cod") & "_" & row.Item("Stato_Destinazione_Cod") & "_" & row.Item("Servizio_Cod"),
                row.Item("Stato_Origine_Des") & " -> " & row.Item("Stato_Destinazione_Des") & " [" & row.Item("WTransizioniDiStatoConfigurazione_Des") & "]"
            ))
    End Function

    Public Function LeggiTransizioniStatoXGruppoUtente(gruppo As GruppoUtente, objParametri_Utenti As AgronicaCoreParametri, objParametri_Server As AgronicaCoreParametri) As IEnumerable(Of BaseCodeDescrStr)
        Dim noSpecifica = ""
        Dim objR As New AgronicaCoreUtentiDAL.GruppiUtente_TransizioniDiStato_R
        Dim objTransizioniStato As New AgronicaCoreMetaSchemaDAL.WTransizioniDiStatoConfigurazione_R
        Dim transizioni = LeggiTransizioniServizio(0, objParametri_Server)
        Dim chiaviTxG As IEnumerable(Of String) = objR.Leggi(
                objParametri_Utenti.PivaSuperUser, gruppo.codice,
                0, 0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta,
                noSpecifica, noSpecifica, objParametri_Utenti
            ).AsEnumerable.
            Select(Function(row) row.Item("Stato_Origine_Cod") & "_" & row.Item("Stato_Destinazione_Cod") & "_" & row.Item("Servizio_Cod")).
            Cast(Of String)()
        Return transizioni.Where(Function(t) chiaviTxG.Contains(t.codice))
    End Function

#End Region

#Region "Scrittura"

    Public Function AggiungiTransizioniUsateXGruppo(codiceGruppo As Integer, transizioni As IEnumerable(Of BaseCodeDescrStr), objParametri_Utenti As AgronicaCoreParametri)
        Dim noFiltro = ""
        Dim enu = transizioni.GetEnumerator
        Dim objW As New AgronicaCoreUtentiDAL.GruppiUtente_TransizioniDiStato_W
        Dim objR As New AgronicaCoreUtentiDAL.GruppiUtente_TransizioniDiStato_R

        '' TODO: mettere check operazione per distinguere semplice aggiunta da sovrascrittura?
        '' Possibile sovrascrivere interamente o solo aggiunta/rimozione singola?
        'objW.Cancella(codiceGruppo, noFiltro, objParametri_Utenti)

        Dim currKeys As IEnumerable(Of String) = objR.Leggi(
                objParametri_Utenti.PivaSuperUser, codiceGruppo,
                0, 0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta,
                noFiltro, noFiltro, objParametri_Utenti
            ).AsEnumerable.
            Select(Function(row) row.Item("Stato_Origine_Cod") & "_" & row.Item("Stato_Destinazione_Cod") & "_" & row.Item("Servizio_Cod")).
            Cast(Of String)()
        While enu.MoveNext
            Dim curr = enu.Current
            If Not currKeys.Contains(curr.codice) Then
                Dim codici = enu.Current.codice.Split("_").
                    Select(Function(s) Integer.Parse(s))
                objW.Scrivi(
                    objParametri_Utenti.PivaSuperUser, codiceGruppo,
                    codici(0), codici(1), codici(2),
                    AGRODATAINIZIO, AGRODATAFINE, objParametri_Utenti
                )
            End If
        End While

    End Function

    Public Function RimuoviTransizioneXGruppo(codiceGruppo As Integer, transizioni As IEnumerable(Of BaseCodeDescrStr), objParametri_Utenti As AgronicaCoreParametri)
        Dim enu = transizioni.GetEnumerator
        Dim filtro = ""
        Dim objW As New AgronicaCoreUtentiDAL.GruppiUtente_TransizioniDiStato_W

        While enu.MoveNext

            Dim codici = enu.Current.codice.Split("_")
            filtro += "Stato_Origine_Cod = '" + codici(0) + "' AND Stato_Destinazione_Cod = '" + codici(1) + "' AND Servizio_Cod = '" + codici(2) + "' "

        End While

        objW.Cancella(codiceGruppo, filtro, objParametri_Utenti)
    End Function

#End Region

End Class
