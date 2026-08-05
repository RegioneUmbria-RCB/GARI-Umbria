Imports AgronicaCoreModelsSTD.Gis.PermessiLayer

Public Class GIS_Permessi_Layer_R

    Public Function LeggiPermessiDaUtente(ByVal LayerElementiGrafici_Cod As Int32,
                                          ByRef objParametri_Utente As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                          ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                          ByVal Optional Utente As String = ""
                                          ) As LeggiPermessiLayerUtenti_Out

        Dim elencoPermessiUtente As New LeggiPermessiLayerUtenti_Out With {
            .LayerCod = LayerElementiGrafici_Cod,
            .utenti_permessi = New List(Of PermessiXUtente)
        }

        Dim xRead As New AgronicaCoreGisDAL.GIS_Permessi_Layer_R
        Dim xReadGruppi As New AgronicaCoreUtentiDAL.Gruppi_Utente_R

        Dim gruppi_appartenenza As New List(Of Int32)
        Dim DT As DataTable

        Dim User = objParametri_Utente.UtenteUsername

        If Not Utente.Equals("") Then
            User = Utente
        End If

        DT = xReadGruppi.LeggiGruppiDaUtente(objParametri_Utente, User)

        For Each row In DT.Rows
            gruppi_appartenenza.Add(CInt(row("Gruppi_Utente_cod")))
        Next

        DT = xRead.LeggiPermessiDaUtenteOGruppo(gruppi_appartenenza, LayerElementiGrafici_Cod, User, objParametri_Server)

        Dim permesso = New PermessiXUtente With {
                    .Username = User,
                    .Flag_Amministrazione = 0,
                    .Flag_Cancellazione = 0,
                    .Flag_Informazioni = 0,
                    .Flag_Inserimento = 0,
                    .Flag_Modifica = 0
                }

        For Each row In DT.Rows
            permesso.Flag_Amministrazione = Convert.ToInt32(CBool(permesso.Flag_Amministrazione) Or CBool(row("Flag_Amministrazione")))
            permesso.Flag_Cancellazione = Convert.ToInt32(CBool(permesso.Flag_Cancellazione) Or CBool(row("Flag_Cancellazione")))
            permesso.Flag_Informazioni = Convert.ToInt32(CBool(permesso.Flag_Informazioni) Or CBool(row("Flag_Informazioni")))
            permesso.Flag_Inserimento = Convert.ToInt32(CBool(permesso.Flag_Inserimento) Or CBool(row("Flag_Inserimento")))
            permesso.Flag_Modifica = Convert.ToInt32(CBool(permesso.Flag_Modifica) Or CBool(row("Flag_Modifica")))
        Next

        elencoPermessiUtente.utenti_permessi.Add(permesso)

        Return elencoPermessiUtente
    End Function

End Class
Public Class GIS_Permessi_Layer_W

End Class
