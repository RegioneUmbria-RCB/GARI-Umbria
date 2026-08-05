Imports AgronicaCoreDataProvider
Imports AgronicaCoreDTOStd.InData.Gis

Public Class GIS_Permessi_Configurazione_R
    Public Function LeggiPermessiUtenteByConfigurazioneCod(ByVal LayerAnalysisConfig_Cod As Int32,
                                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                           ) As ElencoPermessiConfigurazione

        Dim elencoPermessiUtente As New ElencoPermessiConfigurazione With {
            .LayerAnalysisConfig_Cod = LayerAnalysisConfig_Cod,
            .elencoPermessiConfigurazione = New List(Of PermessoConfigurazione)
        }

        Dim xRead As New AgronicaCoreGisDAL.GIS_Permessi_Configurazione_R

        Dim DT As DataTable

        DT = xRead.LeggiPermessiUtenteByConfigurazioneCod(LayerAnalysisConfig_Cod, objParametri)

        For Each row In DT.Rows
            Dim permessoUtente As New PermessoConfigurazione With {
                .UserName = row("UserName").ToString,
                .Flag_Amministrazione = CInt(row("Flag_Amministrazione")),
                .Flag_Cancellazione = CInt(row("Flag_Cancellazione")),
                .Flag_GestioneInteroLayer = CInt(row("Flag_GestioneInteroLayer")),
                .Flag_Informazioni = CInt(row("Flag_Informazioni")),
                .Flag_Inserimento = CInt(row("Flag_Inserimento")),
                .Flag_Modifica = CInt(row("Flag_Modifica"))
            }

            elencoPermessiUtente.elencoPermessiConfigurazione.Add(permessoUtente)
        Next

        Return elencoPermessiUtente
    End Function
    Public Function LeggiPermessiGruppiUtenteByConfigurazioneCod(ByVal LayerAnalysisConfig_Cod As Int32,
                                                                 ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                                 ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                ) As ElencoPermessiConfigurazione

        Dim elencoPermessiUtente As New ElencoPermessiConfigurazione With {
            .LayerAnalysisConfig_Cod = LayerAnalysisConfig_Cod,
            .elencoPermessiConfigurazione = New List(Of PermessoConfigurazione)
        }

        Dim xRead As New AgronicaCoreGisDAL.GIS_Permessi_Configurazione_R

        Dim DT As DataTable

        DT = xRead.LeggiPermessiGruppiUtenteByConfigurazioneCod(LayerAnalysisConfig_Cod, objParametri_Server, objParametri_Utenti)

        For Each row In DT.Rows
            Dim permessoUtente As New PermessoConfigurazione With {
                .Gruppi_Utente_cod = CInt(row("Gruppi_Utente_cod")),
                .Gruppi_Utente_des = row("Gruppi_Utente_des").ToString,
                .Flag_Amministrazione = CInt(row("Flag_Amministrazione")),
                .Flag_Cancellazione = CInt(row("Flag_Cancellazione")),
                .Flag_GestioneInteroLayer = CInt(row("Flag_GestioneInteroLayer")),
                .Flag_Informazioni = CInt(row("Flag_Informazioni")),
                .Flag_Inserimento = CInt(row("Flag_Inserimento")),
                .Flag_Modifica = CInt(row("Flag_Modifica"))
            }

            elencoPermessiUtente.elencoPermessiConfigurazione.Add(permessoUtente)
        Next

        Return elencoPermessiUtente
    End Function

    Public Function LeggiPermessiDaUtente(ByRef objParametri_Utente As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                          ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                          ByVal Optional configurazione_cod As Int32 = 0
                                         ) As ElencoPermessiUtente

        Dim elencoPermessiUtente As New ElencoPermessiUtente With {
            .elencoPermessiUtente = New List(Of ElencoPermessiConfigurazione)
        }

        Dim xRead As New AgronicaCoreGisDAL.GIS_Permessi_Configurazione_R
        Dim xReadGruppi As New AgronicaCoreUtentiDAL.Gruppi_Utente_R

        Dim gruppi_appartenenza As New List(Of Int32)
        Dim DT As DataTable

        DT = xReadGruppi.LeggiGruppiDaUtente(objParametri_Utente)

        For Each row In DT.Rows
            gruppi_appartenenza.Add(CInt(row("Gruppi_Utente_cod")))
        Next

        DT = xRead.LeggiPermessiDaUtenteOGruppo(gruppi_appartenenza, configurazione_cod, objParametri_Utente, objParametri_Server)

        Dim configurazionePrecedente As Int32 = 0
        Dim elenco As New ElencoPermessiConfigurazione
        Dim permesso As New PermessoConfigurazione

        For Each row In DT.Rows
            If Not configurazionePrecedente = CInt(row("LayerAnalysisConfig_Cod")) Then
                If Not configurazionePrecedente = 0 Then
                    elenco.elencoPermessiConfigurazione.Add(permesso)
                    elencoPermessiUtente.elencoPermessiUtente.Add(elenco)
                End If

                configurazionePrecedente = CInt(row("LayerAnalysisConfig_Cod"))

                elenco = New ElencoPermessiConfigurazione With {
                    .LayerAnalysisConfig_Cod = configurazionePrecedente,
                    .elencoPermessiConfigurazione = New List(Of PermessoConfigurazione)
                }

                permesso = New PermessoConfigurazione With {
                    .UserName = objParametri_Utente.UtenteUsername,
                    .Flag_Amministrazione = 0,
                    .Flag_Cancellazione = 0,
                    .Flag_GestioneInteroLayer = 0,
                    .Flag_Informazioni = 0,
                    .Flag_Inserimento = 0,
                    .Flag_Modifica = 0
                }
            End If

            permesso.Flag_Amministrazione = Convert.ToInt32(CBool(permesso.Flag_Amministrazione) Or CBool(row("Flag_Amministrazione")))
            permesso.Flag_Cancellazione = Convert.ToInt32(CBool(permesso.Flag_Cancellazione) Or CBool(row("Flag_Cancellazione")))
            permesso.Flag_GestioneInteroLayer = Convert.ToInt32(CBool(permesso.Flag_GestioneInteroLayer) Or CBool(row("Flag_GestioneInteroLayer")))
            permesso.Flag_Informazioni = Convert.ToInt32(CBool(permesso.Flag_Informazioni) Or CBool(row("Flag_Informazioni")))
            permesso.Flag_Inserimento = Convert.ToInt32(CBool(permesso.Flag_Inserimento) Or CBool(row("Flag_Inserimento")))
            permesso.Flag_Modifica = Convert.ToInt32(CBool(permesso.Flag_Modifica) Or CBool(row("Flag_Modifica")))
        Next

        If elenco.elencoPermessiConfigurazione Is Nothing Then
            Return elencoPermessiUtente
        End If

        elenco.elencoPermessiConfigurazione.Add(permesso)
        elencoPermessiUtente.elencoPermessiUtente.Add(elenco)

        Return elencoPermessiUtente
    End Function

End Class

Public Class GIS_Permessi_Configurazione_W

    Public Function OperazioniPermessiProiezioni(ByVal elencoModifiche As ModifichePermessiConfigurazione,
                                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                 ) As Boolean

        Dim xWrite As New AgronicaCoreGisDAL.GIS_Permessi_Configurazione_W
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
                                              permesso.Flag_GestioneInteroLayer,
                                              elencoModifiche.LayerAnalysisConfig_Cod,
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
                                              permesso.Flag_GestioneInteroLayer,
                                              elencoModifiche.LayerAnalysisConfig_Cod,
                                              permesso.UserName,
                                              permesso.Gruppi_Utente_cod,
                                              objParametri)

                If Not xRisp Then
                    Throw New Exception("Errore nell'aggiornamento dei permessi.")
                End If
            Next

            For Each permesso In elencoModifiche.DeletePermessi
                xRisp = xWrite.DeletePermesso(elencoModifiche.LayerAnalysisConfig_Cod,
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
