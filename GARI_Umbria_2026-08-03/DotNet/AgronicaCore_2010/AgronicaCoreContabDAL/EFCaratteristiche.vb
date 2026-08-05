Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO

Public Class EFCaratteristiche
    Public Shared Sub Caratteristiche_Cancella_EF(
                                                 ByVal DatiCaratteristica As AgronicaCoreModelsSTD.anagrafiche.ParcoMacchineCaratteristiche,
                                                 ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                 Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                 Optional ByVal NewTransaction As Boolean = True,
                                                 Optional ByVal saveChanges As Boolean = True
                                                 )

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.EFMacchine.Caratteristiche_Cancella_EF"
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
            Dim caratteristicheUnitarie = From caratteristsicaSel In GiasContext.Parco_MacchinexCaratteristiche
                                          Where caratteristsicaSel.ID = DatiCaratteristica.codice
                                          Select caratteristsicaSel

            Dim caratteristsica = caratteristicheUnitarie.FirstOrDefault()

            GiasContext.Parco_MacchinexCaratteristiche.Remove(caratteristsica)
            If saveChanges Then
                GiasContext.SaveChanges()
            End If

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

    Public Shared Sub CreateOrUpdateCaratteristiche(ByVal DatiCaratteristica As AgronicaCoreModelsSTD.anagrafiche.ParcoMacchineCaratteristiche,
                                           ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                           ByVal username As String,
                                           ByVal mac_cod As Integer,
                                           Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                           Optional ByVal NewTransaction As Boolean = True)
        Caratteristiche_Scrivi_EF(DatiCaratteristica, objParametriServer, username, mac_cod)
    End Sub

    Public Shared Function Caratteristiche_Scrivi_EF(ByVal caratteristica As AgronicaCoreModelsSTD.anagrafiche.ParcoMacchineCaratteristiche,
                                              ByRef objParametriServer As AgronicaCoreParametri,
                                              ByVal username As String,
                                              ByVal mac_cod As Integer,
                                              Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                              Optional ByVal NewTransaction As Boolean = True
                                              ) As Parco_MacchinexCaratteristiche

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.EFMacchine.Caratteristiche_Scrivi_EF"
        Dim messaggioErrore As String = ""
        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False
        Dim caratteristicaUnitaria As New Parco_MacchinexCaratteristiche()

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If

        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Try

            caratteristicaUnitaria = CreateCaratteristica(username,
                                                 mac_cod,
                                                 objParametriServer
                                                 )


            caratteristicaUnitaria.Mac_Car_Cod = caratteristica.caratteristica.codice
            caratteristicaUnitaria.Valore = caratteristica.valore
            caratteristicaUnitaria.Validita_Inizio = caratteristica.validita_inizio
            caratteristicaUnitaria.Validita_Fine = caratteristica.validita_fine

            GiasContext.Parco_MacchinexCaratteristiche.Add(caratteristicaUnitaria)
            GiasContext.SaveChanges()

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch ex As Exception
            caratteristicaUnitaria = Nothing
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If

        Return caratteristicaUnitaria
    End Function

    Public Shared Function CreateCaratteristica(ByRef username As String,
                                                ByRef Mac_Cod As Integer,
                                                ByRef objParametri As AgronicaCoreParametri) As AgronicaCoreEntityFramework_POCO.Parco_MacchinexCaratteristiche

        Dim caratteristiche As New AgronicaCoreEntityFramework_POCO.Parco_MacchinexCaratteristiche

        caratteristiche.Mac_Cod = Mac_Cod
        caratteristiche.inviato = 0
        caratteristiche.datainvio = DateTime.Now
        caratteristiche.Data_Creazione = DateTime.Now
        caratteristiche.Data_Modifica = DateTime.Now
        caratteristiche.Username_Creazione = username
        caratteristiche.Username_Modifica = username

        Return caratteristiche

    End Function

End Class
