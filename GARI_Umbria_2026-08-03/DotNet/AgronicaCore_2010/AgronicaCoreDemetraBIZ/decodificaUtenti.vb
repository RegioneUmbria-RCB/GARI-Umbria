Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDTOStd.InData
Imports AgronicaCoreModelsSTD.profilazione
Imports AgronicaCoreUtility

Public Class decodificaUtenti

    Public Function decoficaUtenteGiasDaUtenteDemetra(ByVal codice_utente_demetra As String,
                                                             ByVal cuaa As String,
                                                             ByRef ObjParametri_Utenti As AgronicaCoreParametri) As String
        Dim ret As String = ""

        Try
            Dim xUtenti_R As New AgronicaCoreUtentiDAL.Utenti_Read
            Dim xUtenti_Dettagli_R As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R

            If codice_utente_demetra = "" Then
                '1 cerco l'utente dell'azienda agricola tramite cuaa
                Dim dt1 = xUtenti_Dettagli_R.Leggi("", 0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, " Utenti_Dettagli.CodFisc='" & cuaa & "' ", "", ObjParametri_Utenti)
                If dt1.Rows.Count > 0 Then
                    ret = dt1.Rows(0)("UserName").ToString()
                Else
                    ret = ObjParametri_Utenti.UsernameOperazione
                End If
            Else
                '0 cerco l'utente con lo stesso codice utente demetra
                Dim dt = xUtenti_R.Leggi2(codice_utente_demetra, "", "", ObjParametri_Utenti)
                If dt.Rows.Count > 0 Then
                    ret = dt.Rows(0)("UserName").ToString()
                Else
                    '1 cerco l'utente dell'azienda agricola tramite cuaa
                    Dim dt1 = xUtenti_Dettagli_R.Leggi("", 0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, " Utenti_Dettagli.CodFisc='" & cuaa & "' ", "", ObjParametri_Utenti)
                    If dt1.Rows.Count > 0 Then
                        ret = dt1.Rows(0)("UserName").ToString()
                    Else
                        ret = ObjParametri_Utenti.UsernameOperazione
                    End If
                End If
            End If
        Catch ex As Exception
            ret = ""
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function


End Class


Public Class gestione_utenti_permessi
    Public Function AggiornaUtentiTipologiaCambioGestioneQDC(ByVal cuaa As String,
                                                             ByVal new_Tipologia As Integer,
                                                             ByVal StatoDestinazione As Integer,
                                                             ByRef ObjParametri_SuperServer As AgronicaCoreParametri,
                                                             ByRef ObjParametri_Server As AgronicaCoreParametri,
                                                             ByRef ObjParametri_Utenti As AgronicaCoreParametri) As Boolean
        Dim ret As Boolean = False

        Try
            Dim xUtenti_R As New AgronicaCoreUtentiDAL.Utenti_Read
            Dim objUtentiBIZ = New AgronicaCoreUtentiBIZ.Utenti
            Dim xUtenti_Dettagli_R As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R

            '1 cerco l'utente dell'azienda agricola tramite cuaa
            Dim dt1 = xUtenti_Dettagli_R.Leggi("", 0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, " Utenti_Dettagli.CodFisc='" & cuaa & "' ", "", ObjParametri_Utenti)
            If dt1.Rows.Count > 0 Then
                Dim username = dt1.Rows(0)("UserName").ToString()
                If username = "" Then
                    If StatoDestinazione = enum_WWorflow_WAnagraficaStati.QdC_Bluarancio_Demetra_Servizio_4_Mani Then
                        Throw New Exception(String.Format(My.Resources.AgronicaCoreDemetraBIZ.UtenteCUAAnontrovato, cuaa))
                    Else
                        'se lo stato di destinazione è diverso da QdC_Bluarancio_Demetra_Servizio_4_Mani non blocco l'avanzamento in quanto l'utente non è obbligatorio
                        ret = True
                    End If
                Else
                    Dim dt = xUtenti_R.Leggi2(username, "", "", ObjParametri_Utenti)
                    If dt.Rows.Count > 0 Then
                        Dim Operazione As enum_TipoOperazioneDB = enum_TipoOperazioneDB.Modifica
                        Dim ASG_IdServizio As Integer = 5

                        Dim utente As New List(Of UtentePermessi)

                        utente.Add(New UtentePermessi() With {
                            .UserName = username,
                            .ValiditaInizioPermessi = AGRODATAINIZIO,
                            .ValiditaFinePermessi = AGRODATAFINE,
                            .Tipologia = New TipologiaUtente With {.codice = new_Tipologia, .descrizione = ""}
                                })

                        Dim objParams As New ObjParams With {
                            .ObjParametri_Server = ObjParametri_Server,
                            .ObjParametri_SuperServer = ObjParametri_SuperServer,
                            .ObjParametri_Utenti = ObjParametri_Utenti
                        }

                        objUtentiBIZ.AssociaProfilo(utente, New TipologiaUtente With {.codice = new_Tipologia, .descrizione = ""}, True, objParams, If(ObjParametri_Server.objTransazione Is Nothing, True, False))

                        ret = True
                    Else
                        Throw New ColdirettiPDSException(String.Format(My.Resources.AgronicaCoreDemetraBIZ.UtenteCUAAnontrovato, cuaa))
                    End If
                End If
            Else
                If StatoDestinazione = enum_WWorflow_WAnagraficaStati.QdC_Bluarancio_Demetra_Servizio_4_Mani Then
                    Throw New ColdirettiPDSException(String.Format(My.Resources.AgronicaCoreDemetraBIZ.UtenteCUAAnontrovato, cuaa))
                Else
                    ret = True
                End If

            End If
        Catch ex As ColdirettiPDSException
            ret = False
            Throw New ColdirettiPDSException(ex.Message, ex)
        Catch ex As Exception
            ret = False
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function
End Class