Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreEntityFramework
Imports System.Data.Entity
Imports System.Globalization
Imports System.Transactions
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreDataProvider.LogProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModelsSTD
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreModelsSTD.anagrafiche

Public Class EFCostoUnitario
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Shared Function CreateProdotti_Costi(ByRef piva As String,
                                                ByRef username As String,
                                                ByRef Mac_Cod As Integer,
                                                Elem_cod As Integer,
                                                Pro_Cod As Integer,
                                                ByRef objParametri As AgronicaCoreParametri) As AgronicaCoreEntityFramework_POCO.Prodotti_Costi

        Dim prodotti_costi As New AgronicaCoreEntityFramework_POCO.Prodotti_Costi

        'prodotti_costi.ID = CreateNuovo_ID(objParametri)

        prodotti_costi.Piva = piva
        prodotti_costi.Riferimento = ""
        prodotti_costi.Elem_Cod = Elem_cod
        prodotti_costi.Pro_Cod = Pro_Cod
        prodotti_costi.Mat_Cod = Mac_Cod
        prodotti_costi.Udm_Cod = 0
        prodotti_costi.Mezzo = 0
        prodotti_costi.Prezzo_Unitario = 0
        prodotti_costi.Veg_Cod = 0
        prodotti_costi.Cul_Cod = 0
        prodotti_costi.Inviato = 0
        prodotti_costi.DataInvio = DateTime.Now
        prodotti_costi.Data_Creazione = DateTime.Now
        prodotti_costi.Data_Modifica = DateTime.Now
        prodotti_costi.Username_Creazione = username
        prodotti_costi.Username_Modifica = username
        prodotti_costi.Validita_Inizio = AGRODATAINIZIO
        prodotti_costi.Validita_Fine = AGRODATAFINE
        prodotti_costi.Id_Budget = 0

        Return prodotti_costi

    End Function

    Public Shared Function ProdottiCosti_Scrivi_EF(ByVal costo As AgronicaCoreModelsSTD.anagrafiche.CostoUnitario,
                                              ByRef objParametriServer As AgronicaCoreParametri,
                                              ByVal piva As String,
                                              ByVal username As String,
                                              ByVal mac_cod As Integer,
                                              ByVal pro_cod As Integer,
                                              ByVal elem_cod As Integer,
                                              Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                              Optional ByVal NewTransaction As Boolean = True
                                              ) As Prodotti_Costi

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.EFMacchine.ProdottiCosti_Scrivi_EF"
        Dim messaggioErrore As String = ""
        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False
        Dim costoUnitario As New Prodotti_Costi()

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If

        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Try

            costoUnitario = CreateProdotti_Costi(piva,
                                                 username,
                                                 mac_cod,
                                                 0, 0,
                                                 objParametriServer
                                                 )


            costoUnitario.Prezzo_Unitario = costo.prezzo
            'costoUnitario.Udm_Cod = costo.unitaDiMisura.codice
            costoUnitario.Mezzo = costo.unitaDiMisura.codice
            costoUnitario.Validita_Inizio = costo.validita.inizio
            costoUnitario.Validita_Fine = costo.validita.fine
            costoUnitario.Elem_Cod = CostantiPersonalizzate.MACCHINE
            costoUnitario.Id_Budget = 0

            GiasContext.Prodotti_Costi.Add(costoUnitario)
            GiasContext.SaveChanges()

            ''Scrittura tabella Agronica_Log_Anagrafe
            Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
            Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.ProdottiCosti,
                                                                                 CStr(costoUnitario.Piva),
                                                                                 CStr(costoUnitario.ID),
                                                                                 Nothing, Nothing,
                                                                                 Nothing, Nothing,
                                                                                 enum_TipoOperazioneDB.Scrittura,
                                                                                 objParametriServer, enum_Id_Servizio.GiasOnline)

            GiasContext.Agronica_Log_Anagrafe.Add(log)
            GiasContext.SaveChanges()

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch ex As Exception
            costoUnitario = Nothing
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If

        Return costoUnitario
    End Function

    Public Shared Sub CreateOrUpdateCosts(ByVal DatiCostoUnitario As AgronicaCoreModelsSTD.anagrafiche.CostoUnitario,
                                           ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                           ByVal piva As String,
                                           ByVal username As String,
                                           ByVal mac_cod As Integer,
                                           ByVal pro_cod As Integer,
                                           ByVal elem_cod As Integer,
                                           Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                           Optional ByVal NewTransaction As Boolean = True)
        If DatiCostoUnitario.codice <> 0 Then
            ProdottiCosti_Modifica_EF(DatiCostoUnitario, objParametriServer, username)
        Else
            ProdottiCosti_Scrivi_EF(DatiCostoUnitario, objParametriServer, piva, username, mac_cod, pro_cod, elem_cod)
        End If
    End Sub

    Public Shared Function ProdottiCosti_Modifica_EF(ByVal DatiCostoUnitario As AgronicaCoreModelsSTD.anagrafiche.CostoUnitario,
                                                    ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                    ByVal username As String,
                                                    Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                    Optional ByVal NewTransaction As Boolean = True
                                                    ) As Prodotti_Costi

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.EFMacchine.ProdottiCosti_Modifica_EF"
        Dim messaggioErrore As String = String.Empty
        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Dim costiUnitari = From costoItem In GiasContext.Prodotti_Costi
                           Where costoItem.ID = DatiCostoUnitario.codice AndAlso costoItem.Id_Budget = 0
                           Select costoItem

        Dim costo = costiUnitari.FirstOrDefault()

        Try

            'If costiUnitari.Count = 0 Then
            '    costo = New Prodotti_Costi()
            'Else
            '    costo = costiUnitari.FirstOrDefault()
            'End If
            If costo IsNot Nothing Then

                costo.Prezzo_Unitario = DatiCostoUnitario.prezzo
                costo.Mezzo = DatiCostoUnitario.unitaDiMisura.codice
                'costo.Udm_Cod = DatiCostoUnitario.unitaDiMisura.codice
                costo.Validita_Inizio = DatiCostoUnitario.validita.inizio
                costo.Validita_Fine = DatiCostoUnitario.validita.fine

                GiasContext.Prodotti_Costi.Attach(costo)
                GiasContext.Entry(costo).State = EntityState.Modified
                GiasContext.SaveChanges()

                ''Scrittura tabella Agronica_Log_Anagrafe
                Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
                Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.ProdottiCosti,
                                                                                CStr(costo.Piva), CStr(costo.ID),
                                                                                Nothing, Nothing,
                                                                                Nothing, Nothing,
                                                                                enum_TipoOperazioneDB.Modifica,
                                                                                objParametriServer, enum_Id_Servizio.GiasOnline)

                GiasContext.Agronica_Log_Anagrafe.Add(log)
                GiasContext.SaveChanges()

                If NewTransaction Then
                    scope.Complete()
                    scope.Dispose()
                End If

            End If

        Catch ex As Exception
            costo = Nothing
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If

        Return costo
    End Function

    Public Shared Sub ProdottiCosti_Cancella_EF(ByVal DatiCostoUnitario As AgronicaCoreModelsSTD.anagrafiche.CostoUnitario,
                                       ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                       Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                       Optional ByVal NewTransaction As Boolean = True,
                                       Optional ByVal saveChanges As Boolean = True
                                       )

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.EFMacchine.ProdottiCosti_Cancella_EF"
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
            Dim costiUnitari = From costoItem In GiasContext.Prodotti_Costi
                               Where costoItem.ID = DatiCostoUnitario.codice AndAlso costoItem.Id_Budget = 0
                               Select costoItem

            Dim costo = costiUnitari.FirstOrDefault()

            GiasContext.Prodotti_Costi.Remove(costo)

            ''Scrittura tabella Agronica_Log_Anagrafe
            Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
            Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(
                enum_TipoEntita_Des.ProdottiCosti,
                CStr(costo.Piva), CStr(costo.ID),
                Nothing, Nothing,
                Nothing, Nothing,
                enum_TipoOperazioneDB.Cancellazione,
                objParametriServer,
                enum_Id_Servizio.GiasOnline
                )

            GiasContext.Agronica_Log_Anagrafe.Add(log)
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

End Class
