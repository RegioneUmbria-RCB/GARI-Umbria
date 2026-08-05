Imports AgronicaCoreDataProvider
Imports AgronicaCoreDTOStd.InData.Gis
Imports AgronicaCoreModelsSTD.Gis

Public Class Permessi_Maschera_R
    Public Function LeggiPermessiUtenteByMascheraCod(ByVal Maschera_Cod As Int32,
                                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                     ) As ElencoPermessiMaschera

        Dim elencoPermessiUtente As New ElencoPermessiMaschera

        Dim xRead As New AgronicaCoreGisDAL.Permessi_Maschera_R

        Dim DT As DataTable

        DT = xRead.LeggiPermessiUtenteByMascheraCod(Maschera_Cod, objParametri)

        If DT IsNot Nothing AndAlso DT.Rows.Count > 0 Then

            elencoPermessiUtente.Maschera_Cod = Maschera_Cod
            elencoPermessiUtente.elencoPermessiMaschera = New List(Of PermessoMaschera)

            For Each row In DT.Rows
                Dim permessoUtente As New PermessoMaschera With {
                    .UserName = row("UserName").ToString,
                    .Flag_Amministrazione = CInt(row("Flag_Amministrazione")),
                    .Flag_Cancellazione = CInt(row("Flag_Cancellazione")),
                    .Flag_Informazioni = CInt(row("Flag_Informazioni")),
                    .Flag_Inserimento = CInt(row("Flag_Inserimento")),
                    .Flag_Modifica = CInt(row("Flag_Modifica")),
                    .Flag_Attivazione = CInt(row("Flag_Attivazione"))
                }

                elencoPermessiUtente.elencoPermessiMaschera.Add(permessoUtente)
            Next

        End If

        Return elencoPermessiUtente
    End Function
    Public Function LeggiPermessiGruppiUtenteByMascheraCod(ByVal Maschera_Cod As Int32,
                                                                 ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                                 ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                ) As ElencoPermessiMaschera

        Dim elencoPermessiUtente As New ElencoPermessiMaschera

        Dim xRead As New AgronicaCoreGisDAL.Permessi_Maschera_R

        Dim DT As DataTable

        DT = xRead.LeggiPermessiGruppiUtenteByMascheraCod(Maschera_Cod, objParametri_Server, objParametri_Utenti)

        If DT IsNot Nothing AndAlso DT.Rows.Count > 0 Then

            elencoPermessiUtente.Maschera_Cod = Maschera_Cod
            elencoPermessiUtente.elencoPermessiMaschera = New List(Of PermessoMaschera)

            For Each row In DT.Rows

                Dim permessoUtente As New PermessoMaschera With {
                    .Gruppi_Utente_cod = CInt(row("Gruppi_Utente_cod")),
                    .Gruppi_Utente_des = row("Gruppi_Utente_des").ToString,
                    .Flag_Amministrazione = CInt(row("Flag_Amministrazione")),
                    .Flag_Cancellazione = CInt(row("Flag_Cancellazione")),
                    .Flag_Informazioni = CInt(row("Flag_Informazioni")),
                    .Flag_Inserimento = CInt(row("Flag_Inserimento")),
                    .Flag_Modifica = CInt(row("Flag_Modifica")),
                    .Flag_Attivazione = CInt(row("Flag_Attivazione"))
                }

                elencoPermessiUtente.elencoPermessiMaschera.Add(permessoUtente)
            Next

        End If

        Return elencoPermessiUtente
    End Function
    Public Function LeggiPermessiDaUtente(ByRef objParametri_Utente As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                          ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                          ByVal Optional maschera_cod As Int32 = 0
                                         ) As ElencoPermessiUtenteMaschere

        Dim elencoPermessiMaschera As New ElencoPermessiUtenteMaschere With {
            .elencoPermessiUtente = New List(Of ElencoPermessiMaschera)
        }

        Dim xRead As New AgronicaCoreGisDAL.Permessi_Maschera_R
        Dim xReadGruppi As New AgronicaCoreUtentiDAL.Gruppi_Utente_R

        Dim gruppi_appartenenza As New List(Of Int32)
        Dim DT As DataTable

        DT = xReadGruppi.LeggiGruppiDaUtente(objParametri_Utente)

        For Each row In DT.Rows
            gruppi_appartenenza.Add(CInt(row("Gruppi_Utente_cod")))
        Next

        DT = xRead.LeggiPermessiDaUtenteOGruppo(gruppi_appartenenza, maschera_cod, objParametri_Utente, objParametri_Server)

        Dim mascheraPrecedente As Int32 = 0
        Dim elenco As New ElencoPermessiMaschera
        Dim permesso As New PermessoMaschera

        For Each row In DT.Rows
            If Not mascheraPrecedente = CInt(row("Maschera_Cod")) Then
                If Not mascheraPrecedente = 0 Then
                    elenco.elencoPermessiMaschera.Add(permesso)
                    elencoPermessiMaschera.elencoPermessiUtente.Add(elenco)
                End If

                mascheraPrecedente = CInt(row("Maschera_Cod"))

                elenco = New ElencoPermessiMaschera With {
                    .Maschera_Cod = mascheraPrecedente,
                    .elencoPermessiMaschera = New List(Of PermessoMaschera)
                }

                permesso = New PermessoMaschera With {
                    .UserName = objParametri_Utente.UtenteUsername,
                    .Flag_Amministrazione = 0,
                    .Flag_Cancellazione = 0,
                    .Flag_Informazioni = 0,
                    .Flag_Inserimento = 0,
                    .Flag_Modifica = 0,
                    .Flag_Attivazione = 0
                }
            End If

            permesso.Flag_Amministrazione = Convert.ToInt32(CBool(permesso.Flag_Amministrazione) Or CBool(row("Flag_Amministrazione")))
            permesso.Flag_Cancellazione = Convert.ToInt32(CBool(permesso.Flag_Cancellazione) Or CBool(row("Flag_Cancellazione")))
            permesso.Flag_Informazioni = Convert.ToInt32(CBool(permesso.Flag_Informazioni) Or CBool(row("Flag_Informazioni")))
            permesso.Flag_Inserimento = Convert.ToInt32(CBool(permesso.Flag_Inserimento) Or CBool(row("Flag_Inserimento")))
            permesso.Flag_Attivazione = Convert.ToInt32(CBool(permesso.Flag_Attivazione) Or CBool(row("Flag_Attivazione")))
            permesso.Flag_Modifica = Convert.ToInt32(CBool(permesso.Flag_Modifica) Or CBool(row("Flag_Modifica")))
        Next

        If elenco.elencoPermessiMaschera Is Nothing Then
            Return elencoPermessiMaschera
        End If

        elenco.elencoPermessiMaschera.Add(permesso)
        elencoPermessiMaschera.elencoPermessiUtente.Add(elenco)

        Return elencoPermessiMaschera
    End Function

End Class
Public Class Permessi_Maschera_W
    Public Function OperazioniPermessiMaschera(ByVal elencoModifiche As ModifichePermessiMaschera,
                                               ByRef objParametri As AgronicaCoreParametri
                                               ) As Boolean

        Dim xWrite As New AgronicaCoreGisDAL.Permessi_Maschera_W
        Dim xRisp As Boolean = True

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try

            ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                           FlagTransazioneLocale,
                                                           objParametri)

            For Each permesso In elencoModifiche.InsertPermessi
                xRisp = xWrite.InsertPermesso(permesso.Flag_Inserimento,
                                              permesso.Flag_Modifica,
                                              permesso.Flag_Cancellazione,
                                              permesso.Flag_Informazioni,
                                              permesso.Flag_Amministrazione,
                                              permesso.Flag_Attivazione,
                                              elencoModifiche.Maschera_Cod,
                                              permesso.UserName,
                                              permesso.Gruppi_Utente_cod,
                                              objParametri)

                If Not xRisp Then
                    Throw New Exception("Errore nell'inserimento dei permessi.")
                End If
            Next

            For Each permesso In elencoModifiche.UpdatePermessi
                xRisp = xWrite.UpdatePermesso(permesso.Flag_Inserimento,
                                              permesso.Flag_Modifica,
                                              permesso.Flag_Cancellazione,
                                              permesso.Flag_Informazioni,
                                              permesso.Flag_Amministrazione,
                                              permesso.Flag_Attivazione,
                                              elencoModifiche.Maschera_Cod,
                                              permesso.UserName,
                                              permesso.Gruppi_Utente_cod,
                                              objParametri)

                If Not xRisp Then
                    Throw New Exception("Errore nell'aggiornamento dei permessi.")
                End If
            Next

            For Each permesso In elencoModifiche.DeletePermessi
                xRisp = xWrite.DeletePermesso(elencoModifiche.Maschera_Cod,
                                              permesso.UserName,
                                              permesso.Gruppi_Utente_cod,
                                              objParametri)

                If Not xRisp Then
                    Throw New Exception("Errore nell'inserimento dei permessi.")
                End If
            Next

            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

        Catch ex As Exception
            If Not objParametri.objTransazione Is Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            Throw ex
        Finally
            'Chiudo connessione DB
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)
        End Try

        Return xRisp
    End Function
End Class
