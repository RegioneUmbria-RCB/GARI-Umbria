Imports System.Data.Entity
Imports System.Text
Imports System.Transactions
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreXMLUniversale
Imports AgronicaCoreXMLUniversale.Gias_Interscambio_R

Public Class MappingBancaDati

    Public Const STRINGA_UDM As String = "Udm="
    Public Const STRINGA_COSTO As String = "Costo="

    Private _objLog As LogProvider
    Private _logFileName As String
    Private _logDirectory As String
    Private _logDescrizioneUtente As String
    Private _objParametriServer As AgronicaCoreParametri
    Private _customLOGParams As CustomLOGParams

#Region "Costruttori"

    Private Sub New()

    End Sub

    Public Sub New(ByVal configurazioneServizio As AgronicaCoreVarieDAL.Configurazione_Servizio, objParametriServer As AgronicaCoreParametri)

        _objLog = New LogProvider

        _logFileName = configurazioneServizio.Tipo_Sincro.ToString & "_log.txt"
        _logDirectory = configurazioneServizio.DirectoryLOG
        _logDescrizioneUtente = configurazioneServizio.Tipo_Sincro.ToString

        _objParametriServer = objParametriServer

        _customLOGParams = New CustomLOGParams With {
            .LogDescrizioneUtente = _logDescrizioneUtente,
            .LogDirectory = _logDirectory,
            .LogFileName = _logFileName
        }

    End Sub

#End Region

    Public Function CreaMappingBancaDati(ByRef mailMsg As StringBuilder,
                                         ByVal objSettings As ParametriExtra,
                                         ByRef objParametriServer As AgronicaCoreParametri,
                                         ByRef objParametriInterscambio As AgronicaCoreParametri
                                         ) As enum_StatoMapping

        Const nomeRoutine = "CreaMappingBancaDati"

        Dim resMap As enum_StatoMapping = enum_StatoMapping.NonNecessario
        Dim dictMap As Dictionary(Of enum_StatoMappingCacInterscambio, List(Of MappingProdottoBancaDati)) = Nothing
        Dim objInterscambioR As New Gias_Interscambio_R

        Try
            Dim gEfUtils As New Gias_EF_Utility
            Dim efConnString As String = gEfUtils.GetEntityConnectionString(objParametriServer.StringaConnessione)

            'TODO: in realtà dovrei passare prima dal mapping delle categorie, per ottenere questo elenco (per filtro Altro)
            Dim listCategorie As New List(Of String) From {
                CStr(CostantiPersonalizzate.FERTILIZZANTI),
                CStr(INSETTI),
                CStr(CostantiPersonalizzate.TRAPPOLE),
                CStr(INNESCHI)
            }

            ' Giulia: 7/5/2019: se ho la deroga sul NoMapFito allora potrei avere alcuni Fito anche su CAC che necessitano mapping, mentre altri no
            If objSettings.NoMapFitofarmaci = False OrElse objSettings.DerogaSuNoMapFito = True Then
                listCategorie.Add(CStr(CostantiPersonalizzate.FORMULATI))
            End If

            ' rimappo le categorie articoli
            Dim listCategorieAltro As List(Of String)
            Dim formulatiAltro As String = CStr(CostantiPersonalizzate.FORMULATI)
            If objSettings.NoMapCategoria Then
                listCategorieAltro = listCategorie
            Else
                ' escludo i fitofarmaci con reg. ministeriale dalle categorie mappate
                Dim filtroAggiuntivo As String = " Codice_" & objSettings.SuffissoAltroGestionale & "<> '" & objSettings.SottoCat_Fitofarmaci & "' "
                listCategorieAltro = New List(Of String)
                For Each categoria In listCategorie
                    Dim idParam As Integer = 0
                    Dim categoriaALTRO As String = ""
                    objInterscambioR.Leggi_CodiceParametro_ALTRO_From_GIAS("", True, idParam, INTERSCAMBIO_PARAMETRI_CATEGORIE, categoria, categoriaALTRO, objParametriInterscambio, objSettings.SuffissoAltroGestionale, xFiltroAggiuntivo:=filtroAggiuntivo)
                    If categoriaALTRO <> "" Then
                        listCategorieAltro.Add(categoriaALTRO)
                        If categoria = CStr(CostantiPersonalizzate.FORMULATI) Then
                            formulatiAltro = categoriaALTRO
                        End If
                    End If
                Next
            End If

            Dim filtroCategorieGias As String = String.Join(", ", listCategorie)

            'le categorie ALTRO sono stringhe, quindi devo aggiungere gli apici
            Dim filtroCategorieAltro As String = String.Join(", ", listCategorieAltro.Select(Function(x) String.Format("'{0}'", x)))

            Dim filtroCategorieAltroInterscambio As String
            Dim prefissoFito As String = Nothing

            If objSettings.NoMapFitofarmaci = True AndAlso objSettings.DerogaSuNoMapFito = True Then
                'devo eliminare 191 dall'elenco, perché in questo caso devo fare un filtro completamente diverso sulla categoria
                'per evitare che consideri i prodotti NoMap come prodotti precedentemente mappati ed ora eliminati su CAC
                listCategorieAltro.Remove(formulatiAltro)
                filtroCategorieAltroInterscambio = String.Join(", ", listCategorieAltro.Select(Function(x) String.Format("'{0}'", x)))
                prefissoFito = objSettings.PrefissoFito
            Else
                filtroCategorieAltroInterscambio = filtroCategorieAltro
            End If


            'Cambiare paradigma

            'ogni volta devo completamente "sincronizzare" le due tabelle:
            '- se un prodotto l'avevo precedentemente mappato e poi ho cambiato o eliminato la voce in cac, devo rispecchiare la cosa in interscambio
            '- se un prodotto è mappato in cac, ma non in interscambio lo devo sistemare (è quello che già c'è)

            '(MAYBE?!?) devo anche andare a verificare che se il prodotto l'ho già usato (quindi ho fatto già un carico con un determinato pro_cod),
            'non posso andare a cambiarlo, perché dovrei cambiare anche tutti i carichi

            'Elenco dei prodotti non ancora mappati/cambiati/cancellati
            dictMap = objInterscambioR.LeggiProdottiMappatiDifferenti(objParametriServer, objParametriInterscambio,
                                                                      suffissoColonnaAltro:=objSettings.SuffissoAltroGestionale,
                                                                      filtroCategorieGias:=filtroCategorieGias,
                                                                      filtroCategorieAltro:=filtroCategorieAltro,
                                                                      filtroCategorieAltroInterscambio:=filtroCategorieAltroInterscambio,
                                                                      prefissoFito:=prefissoFito)

            If Not dictMap Is Nothing AndAlso dictMap.Count > 0 Then

                For Each item In dictMap

                    Dim stato As enum_StatoMappingCacInterscambio = item.Key
                    Dim listProd As List(Of MappingProdottoBancaDati) = item.Value

                    Select Case stato

                        Case enum_StatoMappingCacInterscambio.Non_Ancora_Mappato

                            If Not listProd Is Nothing AndAlso listProd.Count > 0 Then

                                GetResMapNecessario(resMap)

                                For Each mapProd In listProd

                                    MappaNuovoProdotto(objParametriServer, objParametriInterscambio, objSettings,
                                                       efConnString, mapProd, resMap, mailMsg)

                                Next

                            End If

                        Case enum_StatoMappingCacInterscambio.Cambiato_Su_Cac

                            If Not listProd Is Nothing AndAlso listProd.Count > 0 Then

                                GetResMapNecessario(resMap)

                                For Each mapProd In listProd

                                    'verificare se il vecchio prodotto è già stato usato
                                    Dim isOldProdUsed As Boolean = VerificaProdottoUsato(objParametriServer,
                                                                                         mapProd.Categoria_Gias, mapProd.Prodotto_Gias)

                                    CambiaMapping(objParametriInterscambio, objSettings, efConnString,
                                                  isOldProdUsed, mapProd, resMap, mailMsg)

                                Next

                            End If

                        Case enum_StatoMappingCacInterscambio.Cancellato_su_Cac

                            If Not listProd Is Nothing AndAlso listProd.Count > 0 Then

                                GetResMapNecessario(resMap)

                                For Each mapProd In listProd

                                    'verificare se il prodotto (così com'era su interscambio) è già stato usato
                                    Dim isOldProdUsed As Boolean = VerificaProdottoUsato(objParametriServer,
                                                                                         mapProd.Categoria_Gias, mapProd.Prodotto_Gias)

                                    CancellaMapping(objParametriInterscambio, objSettings, efConnString,
                                                    isOldProdUsed, mapProd, resMap, mailMsg)

                                Next

                            End If

                        Case Else
                            Throw New Exception(String.Format("Lo stato mapping {0} non è gestito.", stato.ToString))
                    End Select

                Next

            End If

        Catch ex As Exception
            Dim msg As String = ex.Message & " [" & If(ex.InnerException Is Nothing, "", ex.InnerException.Message) & "]."
            mailMsg.Append(msg)
            Logga(msg)
            Throw New Exception("[" & nomeRoutine & "] : " & msg)
        End Try

        Return resMap

    End Function

    <Obsolete>
    Public Function CreaMappingBancaDatiOLD(ByRef mailMsg As StringBuilder,
                                            ByRef objSettings As ParametriExtra,
                                            ByRef objParametriServer As AgronicaCoreParametri,
                                            ByRef objParametriInterscambio As AgronicaCoreParametri
                                            ) As Integer

        Const nomeRoutine = "CreaMappingBancaDati"
        Dim resMap As enum_StatoMapping = enum_StatoMapping.NonNecessario
        Dim dictCat As Dictionary(Of String, List(Of MappingProdotto))

        Dim objCacW As New CAC_Codifica_ProdottiAziendali_W
        Dim objInterscambioR As New Gias_Interscambio_R
        Dim objInterscambioW As New Gias_Interscambio_W

        Dim dal As Gias_DeveloperServer_Entities = Nothing
        Dim msg As String = ""

        Try
            Dim gEfUtils As New Gias_EF_Utility
            Dim efConnString As String = gEfUtils.GetEntityConnectionString(objParametriServer.StringaConnessione)

            'TODO: in realtà dovrei passare prima dal mapping delle categorie, per ottenere questo elenco (per filtro Altro)
            Dim listCategorie As New List(Of String) From {
                CStr(CostantiPersonalizzate.FERTILIZZANTI),
                CStr(INSETTI),
                CStr(CostantiPersonalizzate.TRAPPOLE),
                CStr(INNESCHI)
            }

            If objSettings.NoMapFitofarmaci = False Then
                listCategorie.Add(CStr(CostantiPersonalizzate.FORMULATI))
            End If

            Dim filtroCategorieGias As String = String.Join(", ", listCategorie)

            'le categorie ALTRO sono stringhe, quindi devo aggiungere gli apici
            Dim filtroCategorieAltro As String = String.Join(", ", listCategorie.Select(Function(x) String.Format("'{0}'", x)))

            'Elenco dei prodotti non ancora mappati
            dictCat = objInterscambioR.LeggiProdottiNonMappati(objParametriInterscambio,
                                                               flagGetNoMapGias:=True,
                                                               flagGetNoMapAltro:=False,
                                                               filtroCategorie:=filtroCategorieAltro,
                                                               suffissoColonnaAltro:=objSettings.SuffissoAltroGestionale)

            If Not dictCat Is Nothing Then

                For Each item In dictCat

                    Dim elemCod As Integer = CInt(item.Key)
                    Dim listProd As List(Of MappingProdotto) = item.Value
                    Dim cacList As List(Of CAC_Codifica_ProdottiAziendali) = Nothing

                    Dim listOfCod As IEnumerable(Of String) = listProd.Select(Function(x) CStr(x.Cod_Prodotto))

                    Using dalNoTrans As New Gias_DeveloperServer_Entities(efConnString)

                        'Leggo gli elementi di cac con uid in quell'elenco e con codice_gias <> 0
                        cacList = (From c As CAC_Codifica_ProdottiAziendali In dalNoTrans.CAC_Codifica_ProdottiAziendali
                                   Where c.Elem_Cod = elemCod And
                                         c.Tipo_Codifica = 0 And
                                         c.Codice_GIAS <> 0 And
                                         listOfCod.Contains(c.Cod_Prodotto_Cliente)
                                   Select c).ToList
                    End Using

                    If Not cacList Is Nothing AndAlso cacList.Count > 0 Then

                        If resMap = enum_StatoMapping.NonNecessario Then
                            resMap = enum_StatoMapping.Necessario
                        End If

                        For Each cac In cacList

                            Using ts As New TransactionScope(TransactionScopeOption.Required)

                                Try

                                    dal = New Gias_DeveloperServer_Entities(efConnString)
                                    'dal.ContextOptions.UseLegacyPreserveChangesBehavior = False
                                    'Open the contextObject connection state explicitly
                                    dal.Database.Connection.Open()

                                    'Creo la voce su Prodotti_Costi con il pro_cod giusto
                                    If Not cac.Note Is Nothing AndAlso cac.Note <> "" Then
                                        Dim noteSplit As String() = cac.Note.Split(New Char() {"|"c}, StringSplitOptions.RemoveEmptyEntries)

                                        If noteSplit.Count = 2 Then
                                            Dim udmStr As String = Array.Find(noteSplit, Function(x) x.StartsWith(STRINGA_UDM))
                                            Dim costoStr As String = Array.Find(noteSplit, Function(x) x.StartsWith(STRINGA_COSTO))

                                            If Not String.IsNullOrEmpty(udmStr) AndAlso IsNumeric(udmStr.Replace(STRINGA_UDM, "")) AndAlso
                                               Not String.IsNullOrEmpty(costoStr) AndAlso IsNumeric(costoStr.Replace(STRINGA_COSTO, "")) Then

                                                'valorizza costi per questo pro_Cod
                                                Dim costo As Prodotti_Costi = Nothing
                                                ValorizzaProdottiCostiBancaDati(dal, costo, cac.Piva,
                                                                                cac.Elem_Cod, cac.Codice_GIAS,
                                                                                CInt(udmStr.Replace(STRINGA_UDM, "")),
                                                                                CDec(costoStr.Replace(STRINGA_COSTO, "")),
                                                                                objParametriServer)
                                                'costo.MarkAsModified()
                                                dal.Entry(costo).State = EntityState.Modified

                                                'a questo punto non mi serve più l'informazione quindi ri-azzero le note
                                                objCacW.Modifica2(cac.Piva, cac.Elem_Cod, cac.Cod_Prodotto_Cliente,
                                                                  objParametriServer, Note:="")
                                            Else
                                                'Non posso creare record prodotti costi
                                            End If
                                        End If

                                    End If

                                    dal.SaveChanges()
                                    'dal.AcceptAllChanges()

                                    'Scrivo mapping con pro_cod in interscambio
                                    Dim elem = listProd.Find(Function(x) x.Cod_Prodotto = cac.Cod_Prodotto_Cliente)

                                    If Not elem Is Nothing Then
                                        Dim xRisp As Boolean = False
                                        xRisp = objInterscambioW.Modifica_CodiciProdotti(elem.Id_Prod,
                                                                                         objParametriInterscambio,
                                                                                         objSettings.SuffissoAltroGestionale,
                                                                                         codCategoria_GIAS:=cac.Elem_Cod,
                                                                                         codProdotto_GIAS:=cac.Codice_GIAS,
                                                                                         piva:=cac.Piva,
                                                                                         usernameModifica:="agronica")

                                        If xRisp = False Then
                                            Throw New Exception("Fallita scrittura mapping in Interscambio.")
                                        End If
                                    End If

                                    ts.Complete()

                                    Select Case resMap
                                        Case enum_StatoMapping.Necessario, enum_StatoMapping.Riuscito
                                            resMap = enum_StatoMapping.Riuscito
                                        Case Else
                                            resMap = enum_StatoMapping.ParzialmenteRiuscito
                                    End Select

                                    msg = String.Format(" *** MAPPING Banca Dati [{0}-{1}] RIUSCITA ", elemCod, cac.Codice_GIAS) & vbCrLf

                                Catch ex As Exception
                                    ts.Dispose()

                                    Select Case resMap
                                        Case enum_StatoMapping.ParzialmenteRiuscito, enum_StatoMapping.Riuscito
                                            resMap = enum_StatoMapping.ParzialmenteRiuscito
                                        Case Else
                                            resMap = enum_StatoMapping.Fallito
                                    End Select

                                    Dim innerMsg As String = If(ex.InnerException Is Nothing, "", ex.InnerException.Message)
                                    msg = String.Format(" *** MAPPING Banca Dati [{0}-{1}] ERRORE = {2} [{3}].",
                                                        elemCod, cac.Codice_GIAS, ex.Message, innerMsg) & vbCrLf

                                Finally
                                    mailMsg.Append(msg)
                                    Logga(msg)

                                    'Close the opened connection
                                    If Not dal Is Nothing AndAlso dal.Database.Connection.State = ConnectionState.Open Then
                                        dal.Database.Connection.Close()
                                    End If
                                End Try

                            End Using

                        Next

                    End If

                Next

            End If

        Catch ex As Exception
            Dim innerMsg As String = If(ex.InnerException Is Nothing, "", ex.InnerException.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message & " [" & innerMsg & "].")
        End Try

        Return resMap

    End Function

    Private Function VerificaProdottoUsato(ByRef objParametri As AgronicaCoreParametri,
                                           ByVal elemCod As Integer,
                                           ByVal proCod As Integer
                                           ) As Boolean

        Const nomeRoutine = "VerificaProdottoUsato"
        Dim isProdottoUsato As Boolean = False

        Try
            Dim gEfUtils As New Gias_EF_Utility
            Dim efConnString As String = gEfUtils.GetEntityConnectionString(objParametri.StringaConnessione)

            Dim oldProdUse As Integer = 0

            Using dal As New Gias_DeveloperServer_Entities(efConnString)

                'TODO: aggiungere verifica anche per Piva?!?

                oldProdUse = (From m In dal.Movimenti_dettagli
                              Where m.Elem_Cod = elemCod AndAlso m.Pro_Cod = proCod
                              Select m).Count

                If oldProdUse = 0 Then
                    oldProdUse = (From c In dal.CDG_Testata
                                  Where c.Elem_Cod = elemCod AndAlso c.Pro_Cod = proCod AndAlso c.Budget = 0
                                  Select c).Count
                End If

            End Using

            If oldProdUse > 0 Then
                isProdottoUsato = True
            End If

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return isProdottoUsato

    End Function

    Private Sub ValorizzaProdottiCostiBancaDati(ByRef dal As Gias_DeveloperServer_Entities,
                                                ByRef costo As Prodotti_Costi,
                                                ByVal pivaPadre As String,
                                                ByVal elemCod As Integer,
                                                ByVal proCod As Integer,
                                                ByVal udm As Integer,
                                                ByVal prezzo As Decimal,
                                                ByRef objParametriServer As AgronicaCoreParametri)

        Const nomeRoutine = "ValorizzaProdottiCostiBancaDati"

        Try

            costo = EFMaterie_Prime.CreateProdotti_Costi(dal, objParametriServer,
                                                         pivaPadre, elemCod, proCod, 0,
                                                         objParametriServer.UsernameOperazione)
            'Per i prodotti è sempre -1
            costo.Mezzo = -1

            costo.Prezzo_Unitario = prezzo
            costo.Udm_Cod = udm

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

    End Sub

    Private Shared Sub GetResMapNecessario(ByRef resMap As enum_StatoMapping)
        If resMap = enum_StatoMapping.NonNecessario Then
            resMap = enum_StatoMapping.Necessario
        End If
    End Sub

    Private Shared Sub GetResMapRiuscito(ByRef resMap As enum_StatoMapping)
        Select Case resMap
            Case enum_StatoMapping.Necessario, enum_StatoMapping.Riuscito
                resMap = enum_StatoMapping.Riuscito
            Case Else
                resMap = enum_StatoMapping.ParzialmenteRiuscito
        End Select
    End Sub

    Private Shared Sub GetResMapErrore(ByRef resMap As enum_StatoMapping)
        Select Case resMap
            Case enum_StatoMapping.ParzialmenteRiuscito, enum_StatoMapping.Riuscito
                resMap = enum_StatoMapping.ParzialmenteRiuscito
            Case Else
                resMap = enum_StatoMapping.Fallito
        End Select
    End Sub

    Private Sub MappaNuovoProdotto(ByRef objParametriServer As AgronicaCoreParametri,
                                   ByRef objParametriInterscambio As AgronicaCoreParametri,
                                   ByRef objSettings As ParametriExtra,
                                   ByVal efConnString As String,
                                   ByRef mapProd As MappingProdottoBancaDati,
                                   ByRef resMap As enum_StatoMapping,
                                   ByRef mailMsg As StringBuilder)

        Dim objCacW As New CAC_Codifica_ProdottiAziendali_W
        Dim objInterscambioW As New Gias_Interscambio_W
        Dim dal As Gias_DeveloperServer_Entities = Nothing
        Dim msg As String = ""

        Using ts As New TransactionScope(TransactionScopeOption.Required)

            Try

                dal = New Gias_DeveloperServer_Entities(efConnString)
                'dal.ContextOptions.UseLegacyPreserveChangesBehavior = False
                'Open the contextObject connection state explicitly
                dal.Database.Connection.Open()

                'Creo la voce su Prodotti_Costi con il pro_cod giusto
                If Not mapProd.Note_Cac Is Nothing AndAlso mapProd.Note_Cac <> "" Then
                    Dim noteSplit As String() = mapProd.Note_Cac.Split(New Char() {"|"c}, StringSplitOptions.RemoveEmptyEntries)

                    If noteSplit.Count = 2 Then
                        Dim udmStr As String = Array.Find(noteSplit, Function(x) x.StartsWith(STRINGA_UDM))
                        Dim costoStr As String = Array.Find(noteSplit, Function(x) x.StartsWith(STRINGA_COSTO))

                        If Not String.IsNullOrEmpty(udmStr) AndAlso IsNumeric(udmStr.Replace(STRINGA_UDM, "")) AndAlso
                           Not String.IsNullOrEmpty(costoStr) AndAlso IsNumeric(costoStr.Replace(STRINGA_COSTO, "")) Then

                            'valorizza costi per questo pro_Cod
                            Dim costo As Prodotti_Costi = Nothing
                            ValorizzaProdottiCostiBancaDati(dal, costo, mapProd.Piva_Cac,
                                                            mapProd.Categoria_Cac, mapProd.ProCod_Cac,
                                                            CInt(udmStr.Replace(STRINGA_UDM, "")),
                                                            CDec(costoStr.Replace(STRINGA_COSTO, "")),
                                                            objParametriServer)
                            'costo.MarkAsModified()
                            dal.Entry(costo).State = EntityState.Modified

                            'a questo punto non mi serve più l'informazione quindi ri-azzero le note
                            objCacW.Modifica2(mapProd.Piva_Cac, mapProd.Categoria_Cac, mapProd.Prodotto_Cac,
                                              objParametriServer, Note:="")
                        Else
                            'Non posso creare record prodotti costi
                        End If
                    End If

                End If

                dal.SaveChanges()
                'dal.AcceptAllChanges()

                'Scrivo mapping con pro_cod in interscambio
                Dim xRisp As Boolean = False
                xRisp = objInterscambioW.Modifica_CodiciProdotti(mapProd.Id_Prod,
                                                                 objParametriInterscambio,
                                                                 objSettings.SuffissoAltroGestionale,
                                                                 codCategoria_GIAS:=mapProd.Categoria_Cac,
                                                                 codProdotto_GIAS:=mapProd.ProCod_Cac,
                                                                 piva:=mapProd.Piva_Cac,
                                                                 usernameModifica:="agronica")

                If xRisp = False Then
                    Throw New Exception("Fallita scrittura mapping in Interscambio.")
                End If

                ts.Complete()

                GetResMapRiuscito(resMap)
                msg = String.Format(" *** MAPPING Banca Dati [{0}-{1}] RIUSCITA ",
                                    mapProd.Categoria_Cac, mapProd.ProCod_Cac) & vbCrLf

            Catch ex As Exception
                ts.Dispose()
                GetResMapErrore(resMap)

                Dim innerMsg As String = If(ex.InnerException Is Nothing, "", ex.InnerException.Message)
                msg = String.Format(" *** MAPPING Banca Dati [{0}-{1}] ERRORE = {2} [{3}].",
                                    mapProd.Categoria_Cac, mapProd.ProCod_Cac, ex.Message, innerMsg) & vbCrLf

            Finally
                mailMsg.Append(msg)
                Logga(msg)

                'Close the opened connection
                If Not dal Is Nothing AndAlso dal.Database.Connection.State = ConnectionState.Open Then
                    dal.Database.Connection.Close()
                End If
            End Try

        End Using

    End Sub

    Private Sub CambiaMapping(ByVal objParametriInterscambio As AgronicaCoreParametri,
                              ByVal objSettings As ParametriExtra,
                              ByVal efConnString As String,
                              ByVal isOldProdUsed As Boolean,
                              ByVal mapProd As MappingProdottoBancaDati,
                              ByRef resMap As enum_StatoMapping,
                              ByRef mailMsg As StringBuilder)

        Dim objInterscambioW As New Gias_Interscambio_W
        Dim dal As Gias_DeveloperServer_Entities = Nothing
        Dim msg As String = ""

        Using ts As New TransactionScope(TransactionScopeOption.Required)

            Try

                If isOldProdUsed = True Then
                    'se è stato usato devo segnalare errore (perché lo si potrà risolvere solo a mano?!?)
                    Throw New Exception(String.Format("Impossibile cambiare mappatura, il prodotto GIAS [{0}-{1}] su cui era mappato è usato.",
                                                      mapProd.Categoria_Gias, mapProd.Prodotto_Gias))
                Else

                    'se non è stato usato devo andare a prendere i costi del prodotto attuale in interscambio, sincronizzare la tab interscambio,
                    'scrivere nuovo record con costi associato al nuovo prodotto;
                    'TODO: il costo del vecchio prodotto lo lascio?!?

                    dal = New Gias_DeveloperServer_Entities(efConnString)
                    'dal.ContextOptions.UseLegacyPreserveChangesBehavior = False
                    'Open the contextObject connection state explicitly
                    dal.Database.Connection.Open()


                    'Cambio il pro cod del costo prodotto (la chiave è identity, quindi posso cambiare senza problemi)
                    Dim objCostiList As List(Of Prodotti_Costi) = (From p In dal.Prodotti_Costi
                                                                   Where p.Piva = objSettings.Piva AndAlso
                                                                         p.Elem_Cod = mapProd.Categoria_Gias AndAlso
                                                                         p.Pro_Cod = mapProd.Prodotto_Gias AndAlso
                                                                         p.Validita_Inizio = AGRODATAINIZIO AndAlso
                                                                         p.Validita_Fine = AGRODATAFINE
                                                                   Select p).ToList()

                    objCostiList.ForEach(Sub(objCosti)
                                             objCosti.Elem_Cod = mapProd.Categoria_Cac
                                             objCosti.Pro_Cod = mapProd.ProCod_Cac
                                             objCosti.Data_Modifica = DateTime.Now
                                             objCosti.Username_Modifica = objParametriInterscambio.UsernameOperazione
                                             dal.Entry(objCosti).State = EntityState.Modified
                                             'objCosti.MarkAsModified()
                                         End Sub)

                    dal.SaveChanges()
                    'dal.AcceptAllChanges()


                    'Aggiorno mapping con pro_cod in interscambio
                    Dim xRisp As Boolean = False
                    xRisp = objInterscambioW.Modifica_CodiciProdotti(mapProd.Id_Prod,
                                                                     objParametriInterscambio,
                                                                     objSettings.SuffissoAltroGestionale,
                                                                     codCategoria_GIAS:=mapProd.Categoria_Cac,
                                                                     codProdotto_GIAS:=mapProd.ProCod_Cac,
                                                                     piva:=mapProd.Piva_Cac,
                                                                     usernameModifica:="agronica")

                    If xRisp = False Then
                        Throw New Exception("Fallita scrittura mapping in Interscambio.")
                    End If

                    ts.Complete()

                    GetResMapRiuscito(resMap)
                    msg = String.Format(" *** MAPPING Banca Dati Cambiamento [{0}-{1}] RIUSCITA ",
                                        mapProd.Categoria_Altro, mapProd.Prodotto_Altro) & vbCrLf

                End If

            Catch ex As Exception
                ts.Dispose()
                GetResMapErrore(resMap)

                Dim innerMsg As String = If(ex.InnerException Is Nothing, "", ex.InnerException.Message)
                msg = String.Format(" *** MAPPING Banca Dati Cambiamento {0} [{1}-{2}] ERRORE = {3} [{4}].",
                                    mapProd.DescProdotto_Altro, mapProd.Categoria_Altro, mapProd.Prodotto_Altro,
                                    ex.Message, innerMsg) & vbCrLf

            Finally
                mailMsg.Append(msg)
                Logga(msg)

                'Close the opened connection
                If Not dal Is Nothing AndAlso dal.Database.Connection.State = ConnectionState.Open Then
                    dal.Database.Connection.Close()
                End If
            End Try

        End Using
    End Sub

    Private Sub CancellaMapping(ByRef objParametriInterscambio As AgronicaCoreParametri,
                                ByVal objSettings As ParametriExtra,
                                ByVal efConnString As String,
                                ByVal isOldProdUsed As Boolean,
                                ByVal mapProd As MappingProdottoBancaDati,
                                ByRef resMap As enum_StatoMapping,
                                ByRef mailMsg As StringBuilder)

        Dim objInterscambioW As New Gias_Interscambio_W
        Dim dal As Gias_DeveloperServer_Entities = Nothing
        Dim msg As String = ""

        Using ts As New TransactionScope(TransactionScopeOption.Required)

            Try

                If isOldProdUsed = True Then
                    'se è stato usato devo segnalare errore (perché lo si potrà risolvere solo a mano?!?)
                    Throw New Exception(String.Format("Impossibile cancellare, il prodotto GIAS [{0}-{1}] su cui era mappato è usato.",
                                                      mapProd.Categoria_Gias, mapProd.Prodotto_Gias))
                Else

                    'se non è stato usato devo cancellare l'associazione su interscambio e i costi

                    dal = New Gias_DeveloperServer_Entities(efConnString)
                    'dal.ContextOptions.UseLegacyPreserveChangesBehavior = False
                    'Open the contextObject connection state explicitly
                    dal.Database.Connection.Open()


                    'TODO: eliminazione voce prodotti costi (solo se è l'unico?!?)
                    Dim objCostiList As List(Of Prodotti_Costi) = (From p In dal.Prodotti_Costi
                                                                   Where p.Piva = objSettings.Piva AndAlso
                                                                         p.Elem_Cod = mapProd.Categoria_Gias AndAlso
                                                                         p.Pro_Cod = mapProd.Prodotto_Gias AndAlso
                                                                         p.Validita_Inizio = AGRODATAINIZIO AndAlso
                                                                         p.Validita_Fine = AGRODATAFINE
                                                                   Select p).ToList()
                    objCostiList.ForEach(Sub(objCosti) dal.Prodotti_Costi.Remove(objCosti))

                    dal.SaveChanges()
                    'dal.AcceptAllChanges()

                    'Resetto mapping con 0 in interscambio
                    Dim xRisp As Boolean = False
                    xRisp = objInterscambioW.Modifica_CodiciProdotti(mapProd.Id_Prod,
                                                                     objParametriInterscambio,
                                                                     objSettings.SuffissoAltroGestionale,
                                                                     codCategoria_GIAS:=0,
                                                                     codProdotto_GIAS:=0,
                                                                     descrProdotto:="",
                                                                     usernameModifica:="agronica")

                    If xRisp = False Then
                        Throw New Exception("Fallita scrittura mapping in Interscambio.")
                    End If

                    ts.Complete()

                    GetResMapRiuscito(resMap)
                    msg = String.Format(" *** MAPPING Banca Dati Cancellazione [{0}-{1}] RIUSCITA ",
                                        mapProd.Categoria_Altro, mapProd.Prodotto_Altro) & vbCrLf

                End If

            Catch ex As Exception
                ts.Dispose()
                GetResMapErrore(resMap)

                Dim innerMsg As String = If(ex.InnerException Is Nothing, "", ex.InnerException.Message)
                msg = String.Format(" *** MAPPING Banca Dati Cancellazione {0} [{1}-{2}] ERRORE = {3} [{4}].",
                                    mapProd.DescProdotto_Altro, mapProd.Categoria_Altro, mapProd.Prodotto_Altro,
                                    ex.Message, innerMsg) & vbCrLf

            Finally
                mailMsg.Append(msg)
                Logga(msg)

                'Close the opened connection
                If Not dal Is Nothing AndAlso dal.Database.Connection.State = ConnectionState.Open Then
                    dal.Database.Connection.Close()
                End If
            End Try

        End Using
    End Sub

    Private Sub Logga(ByVal msg As String)
        _objLog.Scrivi_LOG(_objParametriServer, "", msg, CustomLOGParams:=_customLOGParams)
    End Sub

End Class
