Imports System.Data.Entity
Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO

Public Class EFGerarchiaMacchina
    Public Shared Sub Gerarchia_Cancella_EF(ByVal DatiGerarchia As AgronicaCoreModelsSTD.anagrafiche.MacchinaGerarchia,
                                       ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                       Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                       Optional ByVal NewTransaction As Boolean = True
                                       )

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.EFGerarchiaMacchina.Gerarchia_Cancella_EF"
        Dim messaggioErrore As String = ""
        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Try
            Dim gerarchieUnitarie = From gerarchiaSel In GiasContext.GerarchiaParco_Macchine
                                    Where gerarchiaSel.ID = DatiGerarchia.ID
                                    Select gerarchiaSel

            Dim gerarchia = gerarchieUnitarie.FirstOrDefault()

            GiasContext.GerarchiaParco_Macchine.Remove(gerarchia)
            GiasContext.SaveChanges()

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If

    End Sub

    Public Shared Sub CreateOrUpdateGerarchia(ByVal DatiGerarchia As AgronicaCoreModelsSTD.anagrafiche.MacchinaGerarchia,
                                           ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                           ByVal username As String,
                                           ByVal mac_cod As Integer,
                                           Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                           Optional ByVal NewTransaction As Boolean = True)
        Gerarchia_Scrivi_EF(DatiGerarchia, objParametriServer, username, mac_cod)
    End Sub

    Public Shared Function Gerarchia_Scrivi_EF(ByVal gerarchia As AgronicaCoreModelsSTD.anagrafiche.MacchinaGerarchia,
                                              ByRef objParametriServer As AgronicaCoreParametri,
                                              ByVal username As String,
                                              ByVal mac_cod As Integer,
                                              Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                              Optional ByVal NewTransaction As Boolean = True
                                              ) As GerarchiaParco_Macchine

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.EFGerarchiaMacchina.Gerarchia_Scrivi_EF"
        Dim messaggioErrore As String = ""
        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False
        Dim gerarchiaUnitaria As New GerarchiaParco_Macchine()

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If

        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Try

            gerarchiaUnitaria = CreateGerarchia(username,
                                                 mac_cod,
                                                 objParametriServer
                                                 )
            Dim ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze
            gerarchiaUnitaria.ID = gerarchia.ID
            gerarchiaUnitaria.Mac_Cod_Figlio = gerarchia.macchina.codice
            gerarchiaUnitaria.Qta = gerarchia.qta
            gerarchiaUnitaria.Tipo_Legame = gerarchia.legame.codice
            gerarchiaUnitaria.Descr_Legame = gerarchia.desclegame
            gerarchiaUnitaria.Udm_Cod = gerarchia.udm.codice
            gerarchiaUnitaria.validita_inizio = gerarchia.validita_inizio
            gerarchiaUnitaria.validita_fine = gerarchia.validita_fine

            If gerarchiaUnitaria.ID = 0 Then
                GiasContext.GerarchiaParco_Macchine.Add(gerarchiaUnitaria)
            Else
                Dim GerarchiaDaModificare = GiasContext.GerarchiaParco_Macchine.Find(gerarchiaUnitaria.ID)
                GerarchiaDaModificare.Mac_Cod_Figlio = gerarchiaUnitaria.Mac_Cod_Figlio
                GerarchiaDaModificare.Qta = gerarchiaUnitaria.Qta
                GerarchiaDaModificare.Tipo_Legame = gerarchiaUnitaria.Tipo_Legame
                GerarchiaDaModificare.Descr_Legame = gerarchiaUnitaria.Descr_Legame
                GerarchiaDaModificare.Udm_Cod = gerarchiaUnitaria.Udm_Cod
                GerarchiaDaModificare.validita_inizio = gerarchiaUnitaria.validita_inizio
                GerarchiaDaModificare.validita_fine = gerarchiaUnitaria.validita_fine
                GiasContext.GerarchiaParco_Macchine.Attach(GerarchiaDaModificare)
                GiasContext.Entry(GerarchiaDaModificare).State = EntityState.Modified
            End If
            GiasContext.SaveChanges()

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch ex As Exception
            gerarchiaUnitaria = Nothing
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If

        Return gerarchiaUnitaria
    End Function

    Public Shared Function CreateGerarchia(ByRef username As String,
                                                ByRef Mac_Cod As Integer,
                                                ByRef objParametri As AgronicaCoreParametri) As AgronicaCoreEntityFramework_POCO.GerarchiaParco_Macchine

        Dim gerarchie As New AgronicaCoreEntityFramework_POCO.GerarchiaParco_Macchine

        gerarchie.Mac_Cod_Padre = Mac_Cod
        gerarchie.inviato = 0
        gerarchie.datainvio = DateTime.Now
        gerarchie.data_creazione = DateTime.Now
        gerarchie.data_modifica = DateTime.Now
        gerarchie.username_creazione = username
        gerarchie.username_modifica = username

        Return gerarchie

    End Function

End Class
