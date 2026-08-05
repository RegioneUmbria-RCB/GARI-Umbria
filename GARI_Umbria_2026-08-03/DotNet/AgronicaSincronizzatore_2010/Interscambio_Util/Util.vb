Imports System.IO
Imports System.Text
Imports System.Xml
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModello
Imports AgronicaCoreModelsSTD.Utility
Imports AgronicaCoreUtility
Imports AgronicaCoreXMLUniversale
Imports Interscambio_Util.ParametriExtra
Imports Newtonsoft.Json
Imports RestSharp
Imports AgronicaCoreVarieBIZ

Public Class Util

    Public Const PIVA_ABOCA As String = "01704430519"
    Public Const PIVA_SAM As String = "02969160544"

    Public Const ID_SOTTOCAT_SEMENTI As String = "SEM"
    Public Const ID_SOTTOCAT_PIANTINE As String = "PIANT"

    Public Const UDM_SAP_NUMERO As String = "PZ"
    Public Const UDM_SAP_KG As String = "KG"

    Public Const UDM_PREF_CONSUMI As String = "PREF_CONSUMI"

    Public Const CONTATTO_ORIGINE_GIAS As String = "CONTATTO_NATO_SU_GIAS"
    Public Const RUOLO_ORIGINE_GIAS As String = "RUOLO_NATO_SU_GIAS"
    Public Const FABBRICATO_ORIGINE_GIAS As String = "FABBRICATO_NATO_SU_GIAS"
    Public Const AGENDA_ORIGINE_GIAS As String = "AGENDA_NATA_SU_GIAS"

    Public Enum enum_SezioniExport
        Nessuno = 0
        CREATO = 1
        MODIFICATO = 2
        CANCELLATO = 3
        CREATO_MOD = 4
    End Enum

    Public Enum enum_XmlLog_SospesoTipo
        NESSUNO = 0
        CONTATTO = 1
        PRODOTTO = 2
        PARAM_QUAL = 3
    End Enum

    Public Structure DetFile
        Dim NumTotali As Integer
        Dim NumInseriti As Integer
        Dim NumModificati As Integer
        Dim NumErrori As Integer
        Dim NumOk As Integer
        'NumFile in errore di processo (vengono lasciati lì per riprovare in seguito, quando i prerequisiti necessari potrebbero essere andati a posto)
        'Oppure in caso di export file che non vengono inviati (ad esempio record cancellato, ma di cui non avevo mai mandato la creazione)
        Dim NumSaltati As Integer
        Dim NumCambioStato As Integer   'NumFile che hanno cambiato tipo (erano modificati ma vengono inviati come creati)

        Dim ElementiOk As List(Of DetFileElemento)
        Dim ElementiErrore As List(Of DetFileElemento)
    End Structure

    Public Structure DetFileElemento
        Dim Tipo As String
        Dim Piva As String 'Piva padre
        Dim CodContatto As String 'Codice specifico del contatto
        Dim CUAA As String 'Payload (vs Cuaa relativo alla piva padre)
        Dim ChiaveFile As String 'chiave_altro|chiave_gias
        Dim ChiaveGias As String '
        Dim ChiaveAltro As String '
        Dim Errore As String
        Dim TipoOperazione As enum_TipoOperazioneDB
    End Structure

    Public Structure DetTask
        Dim Risultato As Boolean
        Dim Messaggio As String
    End Structure

    Public Structure ChiaveParametroAltro
        Dim Piva As String
        Dim IdAltro As String
        Dim IdGias As String
    End Structure

    Private ReadOnly _mappingPiva As New Dictionary(Of String, Integer) From {
        {PIVA_ABOCA, 1},
        {PIVA_SAM, 2}
    }

    Private ReadOnly _mappingOrigineOp As New Dictionary(Of String, Integer) From {
        {enum_TipoOrigineOp.Agri.ToString, enum_TipoOrigineOp.Agri},
        {enum_TipoOrigineOp.Zoo.ToString, enum_TipoOrigineOp.Zoo},
        {enum_TipoOrigineOp.Imputazioni.ToString, enum_TipoOrigineOp.Imputazioni}
    }

    Public ListCatSementi As List(Of String)

    ''' <summary>
    ''' cat_sap, cat_gias, udm_sap, udm_gias
    ''' </summary>
    Public MappingSementi As ListMappingSementi

#Region "Costruttori"

    Public ReadOnly Property Log As InterscambioLog

    Private Sub New()
        Log = New InterscambioLog()
    End Sub

    Public Sub New(ByVal configurazioneServizio As AgronicaCoreVarieDAL.Configurazione_Servizio, objParametriServer As AgronicaCoreParametri)
        Log = New InterscambioLog(configurazioneServizio.Tipo_Sincro.ToString & "_log.txt",
                                  configurazioneServizio.DirectoryLOG,
                                  configurazioneServizio.Tipo_Sincro.ToString,
                                  objParametriServer)
    End Sub

#End Region

    Public Shared Function FormattaPivaItaliana(ByVal piva As String) As String

        Dim pivaRes As String = piva
        If IsNumeric(piva) AndAlso piva.Length < 11 Then
            pivaRes = Right("00000000000" & piva, 11)
        End If

        Return pivaRes

    End Function

    Public Shared Function GetStatoMappingDesc(ByVal resMap As enum_StatoMapping) As String
        Select Case resMap
            Case enum_StatoMapping.Fallito
                Return "Mapping Prodotti Banca Dati Fallito"
            Case enum_StatoMapping.ParzialmenteRiuscito
                Return "Mapping Prodotti Banca Dati Parzialmente Riuscito"
            Case enum_StatoMapping.NonNecessario
                Return "Mapping Prodotti Banca Dati Non Necessario"
            Case enum_StatoMapping.Necessario
                Return "Mapping Prodotti Banca Dati Necessario"
            Case enum_StatoMapping.Riuscito
                Return "Mapping Prodotti Banca Dati Riuscito"
            Case Else
                Return ""
        End Select
    End Function

    Public Shared Function SetMappingPiva(ByRef mappingPiva As Dictionary(Of String, Integer), ByVal piva As String) As Integer

        Const nomeRoutine = "SetMappingPiva"

        Try

            If Not mappingPiva.ContainsKey(piva) Then
                Throw New NotImplementedException("Caso con piva = [" & piva & "] non gestito.")
            End If

            Return mappingPiva.Item(piva)

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
    End Function

    Public Shared Function GetMappingPiva(ByRef mappingPiva As Dictionary(Of String, Integer), ByVal pivaCodice As Integer) As String

        Const nomeRoutine = "GetMappingPiva"

        Try

            If Not mappingPiva.ContainsValue(pivaCodice) Then
                Throw New NotImplementedException(String.Format("Caso con pivaCodice = [{0}] non gestito.", pivaCodice))
            End If

            Return (From kp As KeyValuePair(Of String, Integer) In mappingPiva
                    Where kp.Value = pivaCodice
                    Select kp.Key).FirstOrDefault()

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
    End Function

    Public Shared Function SetMappingTipoOrigineOp(ByRef mappingOrigineOp As Dictionary(Of String, Integer), ByVal tipoOrigine As enum_TipoOrigineOp) As enum_TipoOrigineOp

        Const nomeRoutine = "SetMappingTipoOrigineOp"

        Try

            If Not mappingOrigineOp.ContainsKey(tipoOrigine.ToString) Then
                Throw New NotImplementedException(String.Format("Caso con tipoOrigine = [{0}] non gestito.", tipoOrigine.ToString))
            End If

            Return mappingOrigineOp.Item(tipoOrigine.ToString)

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
    End Function

    Public Shared Function GetMappingTipoOrigineOp(ByRef mappingOrigineOp As Dictionary(Of String, Integer), ByVal tipoOrigineCodice As enum_TipoOrigineOp) As enum_TipoOrigineOp

        Const nomeRoutine = "GetMappingTipoOrigineOp"

        Try

            If Not mappingOrigineOp.ContainsValue(tipoOrigineCodice) Then
                Throw New NotImplementedException(String.Format("Caso con tipoOrigineCodice = [{0}] non gestito.", tipoOrigineCodice))
            End If

            Dim tipoOrigineStr As String = (From kp As KeyValuePair(Of String, Integer) In mappingOrigineOp
                                            Where kp.Value = tipoOrigineCodice
                                            Select kp.Key).FirstOrDefault()

            Dim tipoOrigine As enum_TipoOrigineOp

            If tipoOrigineStr Is Nothing OrElse tipoOrigineStr = "" Then
                Return enum_TipoOrigineOp.Nessuno

            ElseIf [Enum].TryParse(tipoOrigineStr, tipoOrigine) Then

                If [Enum].IsDefined(GetType(enum_TipoOrigineOp), tipoOrigine) Then
                    Return tipoOrigine
                Else
                    Return enum_TipoOrigineOp.Nessuno
                End If
            Else
                Return enum_TipoOrigineOp.Nessuno
            End If

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
    End Function

    Public Shared Function CreaChiaveProgetto(ByVal piva As String, ByVal saCod As Integer, ByVal appezza As Integer, ByVal idReg As Integer) As String
        Return piva & "_" & CStr(saCod) & "_" & CStr(appezza) & "_" & CStr(idReg)
    End Function

    ''' <summary>
    ''' Introdotta perché SAP ha un limite di 40 caratteri, 
    ''' quindi viene eliminato un underscore solo su file XML, nelle tabelle rimane invariato
    ''' </summary>
    Public Shared Function CreaChiaveProgettoMonca(ByVal piva As String, ByVal saCod As Integer, ByVal appezza As Integer, ByVal idReg As Integer) As String
        Return piva & CStr(saCod) & "_" & CStr(appezza) & "_" & CStr(idReg)
    End Function

    Public Shared Function CreaChiaveProgettoZoo(ByVal piva As String, ByVal saCod As Integer, ByVal codProgetto As Integer) As String
        Return piva & "_" & CStr(saCod) & "_" & CStr(codProgetto)
    End Function

    Public Shared Function CreaChiaveProgettoImputazione(ByVal piva As String, ByVal imputazioneCod As Integer) As String
        Return piva & "_" & CStr(imputazioneCod)
    End Function

    Public Function CreaChiaveOp(ByVal tipoOrigine As enum_TipoOrigineOp, ByVal piva As String, ByVal progettoImputazioneCod As Integer, ByVal codOp As String) As String

        Const nomeRoutine = "CreaChiaveOp"

        Try

            ' Giulia: 25/2/2019: Con nuova versione per agri e zoo esco con la chiave che poi finirà in nome breve

            Select Case tipoOrigine

                Case enum_TipoOrigineOp.Agri

                    Return codOp
                    'Return CStr(SetMappingTipoOrigineOp(_mappingOrigineOp, enum_TipoOrigineOp.Agri)) &
                    '       CStr(SetMappingPiva(_mappingPiva, piva)) &
                    '       CStr(progettoImputazioneCod)
                    'Return piva & "_" & CStr(progettoImputazioneCod)

                Case enum_TipoOrigineOp.Zoo

                    Return codOp
                    'Return CStr(SetMappingTipoOrigineOp(_mappingOrigineOp, enum_TipoOrigineOp.Zoo)) &
                    '       CStr(SetMappingPiva(_mappingPiva, piva)) &
                    '       CStr(progettoImputazioneCod)
                    'Return "Z_" & piva & "_" & CStr(progettoImputazioneCod)

                Case enum_TipoOrigineOp.Imputazioni

                    Return codOp
                    'Return CStr(SetMappingTipoOrigineOp(_mappingOrigineOp, enum_TipoOrigineOp.Imputazioni)) &
                    '       CStr(SetMappingPiva(_mappingPiva, piva)) &
                    '       CStr(progettoImputazioneCod)
                    'Return "V_" & piva & "_" & CStr(progettoImputazioneCod)

                Case Else

                    'se lo passo come nessuno nessuno, allora mi serve la chiave per le tabelle, quindi Piva_progettoImputazioneCod
                    Return piva & "_" & CStr(progettoImputazioneCod)

            End Select

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
    End Function

    Public Sub GetChiaveOp(ByVal chiave As String, ByRef tipoOrigine As enum_TipoOrigineOp, ByRef piva As String, ByRef progettoImputazioneCod As Integer)

        Const nomeRoutine = "GetChiaveOp"

        Try

            'TODO: quanto segue va bene per Aboca/SAP, in caso di altri import andrà gestito questo caso particolare tramite codice cliente

            If chiave.Length < 3 Then
                Throw New Exception(String.Format("La lunghezza della chiave [{0}] è uguale a {1}", chiave, chiave.Length))
            ElseIf Not IsNumeric(chiave) Then
                Throw New Exception(String.Format("La chiave [{0}] non è numerica", chiave))
            End If

            Dim codiceTipoOrigine As Integer = CInt(chiave.Substring(0, 1))
            Dim codicePiva As Integer = CInt(chiave.Substring(1, 1))

            tipoOrigine = GetMappingTipoOrigineOp(_mappingOrigineOp, codiceTipoOrigine)
            piva = GetMappingPiva(_mappingPiva, codicePiva)

            progettoImputazioneCod = CInt(chiave.Substring(2, (chiave.Length - 2)))

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
    End Sub

    Public Shared Function CreaChiaveAgenda(ByVal piva As String, ByVal idAgenda As Integer) As String
        Return piva & "_" & CStr(idAgenda)
    End Function


    Public Shared Function CreaChiaveDettaglio(ByVal piva As String, ByVal Id_CDG As Integer, ByVal Id_CDG_Dettaglio As Integer) As String
        Return piva & "_" & CStr(Id_CDG) & "_" & CStr(Id_CDG_Dettaglio)
    End Function

    Public Shared Function IsSplit(ByVal piva As String, ByVal idAgenda As Integer, ByRef dal As Gias_DeveloperServer_Entities) As Boolean
        Const nomeRoutine = "IsSplit"
        Try

            Dim agenda = (From a In dal.Agenda Where a.PIVA = piva AndAlso a.Id_Agenda = idAgenda Select a).FirstOrDefault()

            If agenda Is Nothing Then
                Throw New Exception(String.Format("ERRORE: Non è presente l'agenda Costi {0}_{1}",
                                                  piva, idAgenda))
            End If

            If agenda.Split = CInt(1) Then
                Return True
            End If

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return False
    End Function

    Public Shared Function CreaListaOggettiString(ByVal dt As DataTable, ByVal colName As String) As List(Of String)
        Dim listObj As New List(Of String)
        For Each r As DataRow In dt.Rows
            listObj.Add(CStr(r(colName)))
        Next

        Return listObj
    End Function

    Public Shared Function CreaListaOggettiInteger(ByVal dt As DataTable, ByVal colName As String) As List(Of Integer)
        Dim listObj As New List(Of Integer)
        For Each r As DataRow In dt.Rows
            listObj.Add(CInt(r(colName)))
        Next

        Return listObj
    End Function

    Public Shared Function CreaListaOggettiDictionaryIntString(ByVal dt As DataTable, ByVal colName1 As String, ByVal colName2 As String) As Dictionary(Of Integer, String)
        Dim dicObj As New Dictionary(Of Integer, String)
        For Each r As DataRow In dt.Rows
            dicObj.Add(CInt(r(colName1)), CStr(r(colName2)))
        Next

        Return dicObj
    End Function

    Public Shared Function SerializeList(ByVal lista As List(Of String)) As String
        Dim stb As New StringBuilder

        For Each item In lista

            If stb.Length <> 0 Then
                stb.Append(",")
            End If

            stb.Append(CStr(item))
        Next

        Return stb.ToString()
    End Function

    Public Shared Function GetFirstElementDynamicByProperty(ByRef elencoInput As Object(),
                                                            ByVal nameProperty As String,
                                                            ByVal valToFind As Object
                                                            ) As Object
        Return elencoInput.FirstOrDefault(Function(x) x.GetType().GetProperty(nameProperty).GetValue(x, Nothing) = valToFind)
    End Function

    Public Shared Function GetFirstElementDynamicByPropertyContains(ByRef elencoInput As Object(),
                                                                    ByVal nameProperty As String,
                                                                    ByVal valToFind As IList
                                                                    ) As Object
        Return elencoInput.FirstOrDefault(Function(x) valToFind.Contains(x.GetType().GetProperty(nameProperty).GetValue(x, Nothing)))
    End Function


    Public Function CreaDetFileDefault() As DetFile
        Return New DetFile With {
            .NumTotali = 0,
            .NumInseriti = 0,
            .NumModificati = 0,
            .NumErrori = 0,
            .NumOk = 0,
            .NumSaltati = 0,
            .NumCambioStato = 0,
            .ElementiOk = New List(Of DetFileElemento),
            .ElementiErrore = New List(Of DetFileElemento)
        }
    End Function

    Public Sub AggiornaInfoModifica(ByRef obj As Object, ByRef objParametri As AgronicaCoreParametri)

        obj.Data_Modifica = DateTime.Now
        obj.Username_Modifica = objParametri.UsernameOperazione

    End Sub

    Public Function AllineaCostiCdg(ByVal piva As String, ByRef objParametri As AgronicaCoreParametri, ByRef ObjParametri_Utenti As AgronicaCoreParametri) As String

        Const nomeRoutine = "AllineaCostiCdg"
        Dim errMsg As String = ""

        Try
            Dim objCdgBiz As New AgronicaCoreContabBIZ.CDG_BIZ_W

            errMsg = objCdgBiz.AllineaCosti_BombardinoMultiplo(piva, objParametri, ObjParametri_Utenti)

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return errMsg
    End Function

    Public Function RecordGiaEsportato(ByRef dal As Gias_DeveloperServer_Entities, ByRef codiceSecondario As String, ByVal chiave As String, ByVal tipoExport As String) As Boolean

        Const nomeRoutine = "RecordGiaEsportato"

        Try

            Dim listLog As List(Of XML_Log) = (From x In dal.XML_Log
                                               Where x.Chiave = chiave AndAlso x.Tipo = tipoExport AndAlso x.Stato = CInt(enum_WFlow_XML_Log_Universale.Completato)
                                               Order By x.Data_Operazione Descending
                                               Select x).ToList()

            If listLog IsNot Nothing AndAlso listLog.Count > 0 Then
                'qui c'è salvato il codice Progetto/OP
                codiceSecondario = listLog(0).CodiceSecondario
                Return True
            Else
                Return False
            End If

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
    End Function

    Public Function RicavaCodiceSecondarioOp(ByRef dal As Gias_DeveloperServer_Entities,
                                             ByVal tipo As enum_TipoOrigineOp,
                                             ByVal piva As String,
                                             ByVal progettoCod As Integer
                                             ) As String

        Const nomeRoutine = "RicavaCodiceSecondarioOp"
        Dim codOp As String = ""

        Try

            Select Case tipo

                Case enum_TipoOrigineOp.Agri
                    codOp = (From d In dal.Imprese_Progetti
                             Where d.Piva = piva AndAlso d.Progetto_Cod = progettoCod
                             Select d.Progetto_Nome).FirstOrDefault()

                Case enum_TipoOrigineOp.Zoo
                    codOp = (From d In dal.Zoo_Animali_Distinte
                             Where d.PIVA = piva AndAlso d.Cod_Progetto = progettoCod
                             Select d.Codice_Distinta).FirstOrDefault()

                Case enum_TipoOrigineOp.Imputazioni
                    codOp = (From i In dal.Imputazioni
                             Where i.Piva = piva AndAlso i.Imputazione_Cod = progettoCod
                             Select i.CodiceSecondario).FirstOrDefault()

            End Select

            If String.IsNullOrEmpty(codOp) Then
                Throw New Exception(String.Format("Impossibile ricavare il Codice Op di tipo {0} per [{1}-{2}]", tipo.ToString, piva, progettoCod))
            End If

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return codOp
    End Function

    Public Function RicavaCodTraverso(ByRef dal As Gias_DeveloperServer_Entities,
                                      ByVal tipo As enum_TipoOrigineOp,
                                      ByVal piva As String,
                                      ByVal saCod As Integer,
                                      ByVal appezza As Integer,
                                      ByVal idReg As Integer
                                      ) As String

        Const nomeRoutine = "RicavaCodTraverso"
        Dim codTraverso As String = ""

        Try

            Select Case tipo

                Case enum_TipoOrigineOp.Agri
                    codTraverso = (From c In dal.Campi
                                   Join a In dal.Appezzamento
                                       On c.Piva Equals a.PIVA And c.Sa_Cod Equals a.SA_COD And a.Campo_Cod Equals c.Campo_Cod
                                   Join r In dal.Reg_Impianti
                                       On a.PIVA Equals r.PIVA And a.SA_COD Equals r.SA_COD And a.APPEZZA Equals r.APPEZZA
                                   Where r.PIVA = piva AndAlso
                                         r.SA_COD = saCod AndAlso
                                         r.APPEZZA = appezza AndAlso
                                         r.ID_REG = idReg
                                   Select c.Campo_Des).FirstOrDefault()

                Case enum_TipoOrigineOp.Zoo
                    Throw New Exception("Il Codice Traverso non è previsto per il tipo Zoo")

                Case enum_TipoOrigineOp.Imputazioni
                    Throw New Exception("Il Codice Traverso non è previsto per il tipo Imputazioni")

            End Select

            If String.IsNullOrEmpty(codTraverso) Then
                Throw New Exception(String.Format("Impossibile ricavare il Codice Traverso di tipo {0} per [{1}-{2}-{3}-{4}]",
                                                  tipo.ToString, piva, saCod, appezza, idReg))
            End If

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return codTraverso
    End Function

    Public Function RicavaCodCommessa(ByRef dal As Gias_DeveloperServer_Entities,
                                      ByVal tipo As enum_TipoOrigineOp,
                                      ByVal piva As String,
                                      ByVal saCod As Integer,
                                      ByVal appezza As Integer,
                                      ByVal idReg As Integer
                                      ) As String

        Const nomeRoutine = "RicavaCodCommessa"
        Dim codCommessa As String = ""

        Try

            Select Case tipo

                Case enum_TipoOrigineOp.Agri
                    codCommessa = (From r In dal.Reg_Impianti_Codici
                                   Where r.PIVA = piva AndAlso
                                         r.sa_cod = saCod AndAlso
                                         r.appezza = appezza AndAlso
                                         r.Id_Reg = idReg AndAlso
                                         r.Progetto_Cod = 0 AndAlso
                                         r.id_cod = CInt(enum_CodiciAnagrafe.Codice_Impianto)
                                   Select r.val_cod).FirstOrDefault()

                Case enum_TipoOrigineOp.Zoo
                    Throw New Exception("Il Codice Commessa non è previsto per il tipo Zoo")

                Case enum_TipoOrigineOp.Imputazioni
                    Throw New Exception("Il Codice Commessa non è previsto per il tipo Imputazioni")

            End Select

            If String.IsNullOrEmpty(codCommessa) Then
                Throw New Exception(String.Format("Impossibile ricavare il Codice Commessa di tipo {0} per [{1}-{2}-{3}-{4}]",
                                                  tipo.ToString, piva, saCod, appezza, idReg))
            End If

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return codCommessa
    End Function

    Public Function RicavaSuperficie(ByRef dal As Gias_DeveloperServer_Entities,
                                     ByVal tipo As enum_TipoOrigineOp,
                                     ByVal piva As String,
                                     ByVal saCod As Integer,
                                     ByVal appezza As Integer,
                                     ByVal idReg As Integer,
                                     ByVal idAgendaCosti As Integer
                                     ) As Decimal

        Const nomeRoutine = "RicavaSuperficie"
        Dim superficie As Decimal? = 0

        Try

            Select Case tipo

                Case enum_TipoOrigineOp.Agri


                    'se c'è prendo la superficie utilizzata nell'operazione di campagna collegata,
                    'altrimenti prendo quella di anagrafica

                    Dim superficieAg As Decimal?
                    Dim superficieAn As Decimal?

                    Dim querySuperficieImpiegata = (From mdr In dal.Mov_Dettagli_Riferimenti
                                                    Join mdest In dal.Mov_Destinazioni
                                        On mdr.Piva Equals mdest.Piva And
                                           mdr.Id_Agenda Equals mdest.Id_Agenda
                                                    Where mdr.Piva = piva AndAlso
                                         mdr.Id_Agenda_Rif = idAgendaCosti AndAlso
                                         mdest.Piva = piva AndAlso
                                         mdest.Sa_Cod = saCod AndAlso
                                         mdest.Appezza = appezza AndAlso
                                         mdest.Id_Destinazione = idReg
                                                    Select mdest.Qta2)

                    If querySuperficieImpiegata.Count = 1 Then
                        superficieAg = querySuperficieImpiegata.First()
                    End If

                    If superficieAg Is Nothing OrElse superficieAg = 0 Then
                        superficieAn = (From r In dal.Reg_Impianti
                                        Where r.PIVA = piva AndAlso
                                             r.SA_COD = saCod AndAlso
                                             r.APPEZZA = appezza AndAlso
                                             r.ID_REG = idReg
                                        Select r.Sup_Imp).FirstOrDefault()
                        superficie = If(superficieAn, 0D)
                    Else
                        superficie = superficieAg
                    End If

                    superficie = Agro_Math.ArrotondaVal_4(superficie)

                Case enum_TipoOrigineOp.Zoo
                    Throw New Exception("La Superficie Utilizzata non è prevista per il tipo Zoo")

                Case enum_TipoOrigineOp.Imputazioni
                    Throw New Exception("La Superficie Utilizzata non è prevista per il tipo Imputazioni")

            End Select

            If superficie Is Nothing Then
                Throw New Exception(String.Format("Impossibile ricavare la Superficie Utilizzata di tipo {0} per [{1}-{2}-{3}-{4}]",
                                                  tipo.ToString, piva, saCod, appezza, idReg))
            End If

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return superficie
    End Function

    Public Function RicavaDistinta(ByVal tipo As enum_TipoOrigineOp,
                                   ByVal piva As String,
                                   ByVal codiceOp As String,
                                   ByRef objParametriServer As AgronicaCoreParametri,
                                   ByRef objSettings As ParametriExtra,
                                   ByRef distintaCod As Integer
                                   ) As Object

        Const nomeRoutine = "RicavaDistinta"
        Dim distinta As Object = Nothing

        Try

            Dim gEfUtils As New Gias_EF_Utility
            Dim efConnString As String = gEfUtils.GetEntityConnectionString(objParametriServer.StringaConnessione)

            Using dal As New Gias_DeveloperServer_Entities(efConnString)

                Select Case tipo

                    Case enum_TipoOrigineOp.Agri

                        distinta = (From d In dal.Imprese_Progetti
                                    Where d.Piva = piva AndAlso d.Progetto_Nome = codiceOp
                                    Select d).FirstOrDefault

                        If distinta IsNot Nothing Then
                            distintaCod = distinta.Progetto_Cod
                        End If

                    Case enum_TipoOrigineOp.Zoo

                        distinta = (From z In dal.Zoo_Animali_Distinte
                                    Where z.PIVA = piva AndAlso z.Codice_Distinta = codiceOp
                                    Select z).FirstOrDefault

                        If distinta IsNot Nothing Then
                            distintaCod = distinta.Cod_Progetto
                        End If

                    Case enum_TipoOrigineOp.Imputazioni

                        distinta = (From i In dal.Imputazioni
                                    Where i.Piva = piva AndAlso i.CodiceSecondario = codiceOp
                                    Select i).FirstOrDefault()

                        If distinta IsNot Nothing Then
                            distintaCod = distinta.Imputazione_Cod
                        End If

                End Select

            End Using

            If distinta Is Nothing AndAlso objSettings.PermettiOpEsterni = False Then
                Throw New Exception(String.Format("Impossibile ricavare la distinta di tipo {0} per il codice OP [{1}] e piva [{2}]", tipo.ToString, codiceOp, piva))
            End If

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return distinta
    End Function

    Public Sub LeggiImpostazioniUtente(ByRef objParametriUtenti As AgronicaCoreParametri, ByRef layOutPeso As Integer, ByRef layOutPrezzo As Integer, ByRef layOutLitri As Integer, ByRef aspettoBeni As String, ByRef causaleTrasporto As String)

        Dim dt As DataTable
        Dim objImpost As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim i As Integer
        'Dim Valore As String

        dt = objImpost.Leggi(0, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametriUtenti)

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then

            For i = 0 To dt.Rows.Count - 1

                'Valore = DT.Rows(i).Item("Impostazione_Valore_1")

                Select Case dt.Rows(i).Item("Impostazione_Cod")
                    Case enum_Impostazioni_Utenti.SuperUser_StampaLitriDDTFatture
                        layOutLitri = CInt(dt.Rows(i).Item("Impostazione_Valore_1"))

                    Case enum_Impostazioni_Utenti.SuperUser_LayOut_Peso_DDT
                        layOutPeso = dt.Rows(i).Item("Impostazione_Valore_1")

                    Case enum_Impostazioni_Utenti.SuperUser_LayOut_Prezzo_DDT
                        layOutPrezzo = dt.Rows(i).Item("Impostazione_Valore_1")

                    Case enum_Impostazioni_Utenti.SUPERUSER_COD_Aspetto_Beni_Default
                        aspettoBeni = dt.Rows(i).Item("Impostazione_Valore_1")

                    Case enum_Impostazioni_Utenti.SUPERUSER_COD_CAUSALE_TRASPORTO_DEFAULT
                        causaleTrasporto = dt.Rows(i).Item("Impostazione_Valore_1")
                End Select

            Next

        End If
    End Sub

    Public Shared Function GetCauMov(ByVal lavCod As Integer) As String

        Const nomeRoutine = "GetCauMov"
        Dim cauMov As String = ""

        Try

            Select Case lavCod

                Case LAVCOD_SCARICO, LAVCOD_BOLLA_EMESSA, LAVCOD_FATTURA_EMESSA, LAVCOD_VENDITA

                    cauMov = CAU_SCARICO

                Case LAVCOD_CARICO, LAVCOD_BOLLA_RICEVUTA, LAVCOD_FATTURA_RICEVUTA, LAVCOD_NOTA_ACCREDITO_EMESSA,
                    LAVCOD_ACCETTAZIONE_DIVERSI

                    cauMov = CAU_CARICO

                Case Else
                    Throw New Exception(String.Format("Il Lav_Cod {0} non è gestito correttamente", lavCod))
            End Select

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return cauMov

    End Function

    Public Shared Function GetTipoListino(ByVal lavCod As Integer) As enum_TipoListinoClasse

        Const nomeRoutine = "GetTipoListino"
        Dim listinoClasse As enum_TipoListinoClasse = enum_TipoListinoClasse.ListinoIndefinito

        Try

            Select Case lavCod

                'TODO: Verificare se la nota di credito emessa usa Listino Vendita o Acquisto
                Case LAVCOD_SCARICO, LAVCOD_BOLLA_EMESSA, LAVCOD_FATTURA_EMESSA, LAVCOD_VENDITA, LAVCOD_NOTA_ACCREDITO_EMESSA

                    listinoClasse = enum_TipoListinoClasse.ListinoVendita

                Case LAVCOD_CARICO, LAVCOD_BOLLA_RICEVUTA, LAVCOD_FATTURA_RICEVUTA, LAVCOD_ACCETTAZIONE_DIVERSI

                    listinoClasse = enum_TipoListinoClasse.ListinoAcquisto

                Case Else
                    Throw New Exception(String.Format("Il Lav_Cod {0} non è gestito correttamente", lavCod))
            End Select

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return listinoClasse

    End Function

    Public Shared Function GetMovDescCaricoScarico(ByVal flagDocPassivo As Boolean,
                                                   ByVal lavCod As Integer,
                                                   ByVal nDocSin As String,
                                                   ByVal nDoc As Integer,
                                                   ByVal nDocDes As String,
                                                   ByVal dataDoc As Date
                                                   ) As String

        Const nomeRoutine = "GetMovDescCaricoScarico"
        Dim movDesc As String = ""

        Try

            If flagDocPassivo = True Then
                movDesc = "Carico"
            Else
                movDesc = "Scarico"
            End If

            movDesc &= " di Articoli relativi "

            Select Case lavCod
                Case LAVCOD_VENDITA 'Corrispettivo
                    movDesc &= "al Corrispettivo Vendita"
                Case LAVCOD_BOLLA_EMESSA, LAVCOD_BOLLA_RICEVUTA
                    movDesc &= "al D.D.T."
                Case LAVCOD_FATTURA_EMESSA
                    movDesc &= "alla Fattura Commerciale"
                Case LAVCOD_NOTA_ACCREDITO_EMESSA
                    movDesc &= "alla Nota di Accredito al Cliente"
                Case LAVCOD_ACCETTAZIONE_DIVERSI
                    movDesc &= "ad Accettazione + DDT Ricevuto"
            End Select

            movDesc &= " n. " & nDocSin & " " & nDoc.ToString & " " & nDocDes & " del " & dataDoc.Date.ToShortDateString

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return movDesc

    End Function


#Region "File System"

    Public Shared Function GetNomeFileUnivoco(ByVal nomeFileOrig As String) As String

        Dim d As DateTime = DateTime.Now

        Return d.Year & "_" & d.Month.ToString.PadLeft(2, "0") & "_" & d.Day.ToString.PadLeft(2, "0") & "_" & d.Hour.ToString.PadLeft(2, "0") & "_" & d.Minute.ToString.PadLeft(2, "0") & "_" & d.Second.ToString.PadLeft(2, "0") & "___" & nomeFileOrig
    End Function

    Public Function GetNomeFileProgressivo(ByVal direzioneFile As enum_DirezioneFile, ByVal tipoFile As enum_TipoNomeFile, ByRef objParametriServer As AgronicaCoreParametri) As String

        Const nomeRoutine = "GetNomeFileProgressivo"
        Dim docId As String = ""

        Try

            Dim objSeqProgrR As New Sequenza_Progressivi_R
            Dim progressivo As Integer = 0
            Dim anno As Integer = Now.Year

            progressivo = objSeqProgrR.Nuovo_Progressivo_UpdateImmediato(Piva:=objParametriServer.PivaSuperUser,
                                                                         Anno:=anno,
                                                                         Tipo_Progressivo:=enum_SequenzaProgressiviTipi.Interscambio_File_XML,
                                                                         Doc_Numero_Sin:=direzioneFile.ToString,
                                                                         Doc_Numero_Des:=tipoFile.ToString,
                                                                         Sezionale_Cod:=0,
                                                                         objParametri:=objParametriServer)

            docId = direzioneFile.ToString & tipoFile.ToString & anno.ToString.Substring(2, 2) & progressivo.ToString.PadLeft(9, "0")

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return docId
    End Function

    Public Sub CreaCartellaSeNonEsiste(ByVal dir As String)
        If Not Directory.Exists(dir) Then
            Log.Logga("Cartella non presente: " & dir)
            Directory.CreateDirectory(dir)
            Log.Logga("Cartella creata: " & dir)
        End If
    End Sub

    Public Sub CreaCartellaSeNonEsiste(ByVal dir As String, ByRef mailMsg As StringBuilder)
        If Not Directory.Exists(dir) Then
            Log.Logga("Cartella non presente: " & dir)
            mailMsg.Append("Cartella non presente: " & dir & "." & vbCrLf)
            Directory.CreateDirectory(dir)
            Log.Logga("Cartella creata: " & dir)
            mailMsg.Append("Cartella creata: " & dir & "." & vbCrLf)
        End If
    End Sub

    Public Function VerificaScritturaFile(ByVal fileWithPath As String, ByRef mailMsg As StringBuilder) As Boolean

        Const nomeRoutine = "VerificaScritturaFile"
        Dim xRisp As Boolean = False

        Try
            'controllo se è possibile la lettura/scrittura ==> il file è stato creato correttamente

            Dim stream As FileStream = Nothing
            Dim fileApri As New FileInfo(fileWithPath)
            stream = fileApri.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None)
            stream.Close()
            fileApri = Nothing
            stream = Nothing

            xRisp = True

        Catch ex As Exception
            Log.Logga("Errore creazione file: " & fileWithPath)
            mailMsg.Append("Errore creazione file: " & fileWithPath & "." & vbCrLf)
            Return False
        End Try

        Return xRisp
    End Function

    ' percorso interscambio
    Public Shared Function GetPercorsoInterscambio(ByVal percorso As String, ByVal sottoCartella As String) As String

        'If String.IsNullOrEmpty(percorso) Then
        '    Dim objConfigSiti As New Configurazione_Siti_R
        '    percorso = objConfigSiti.Leggi_Valore(0, "GestioneEsportazioni_Repository", "", "", objParametri_Server)
        'End If

        If Not String.IsNullOrEmpty(sottoCartella) Then
            percorso = Path.Combine(percorso, sottoCartella)
        End If

        If Not Directory.Exists(percorso) Then
            Directory.CreateDirectory(percorso)
        End If

        ' Assicura che la cartella di destinazione termini con il separatore corretto
        If Not percorso.EndsWith(Path.DirectorySeparatorChar) Then
            percorso &= Path.DirectorySeparatorChar
        End If

        Return percorso

    End Function

#End Region

#Region "Mapping Vari"

    ''' <summary>
    ''' Per sdoppiamento Sementi mi serve sottoCategoria, catAltro o catGias
    ''' perché a seconda del contesto potrei aver già ricavato l'elemCod
    ''' </summary>
    Public Function MappaUdm(ByVal piva As String,
                             ByRef udmDict As Dictionary(Of ChiaveParametroAltro, Integer),
                             ByRef objSettings As ParametriExtra,
                             ByRef objParametriInterscambio As AgronicaCoreParametri,
                             ByVal udmAltro As String,
                             ByRef sottoCategoria As String,
                             Optional ByVal catAltro As String = Nothing,
                             Optional ByVal catGias As Integer? = Nothing
                             ) As Integer

        Const nomeRoutine = "MappaUdm"
        Dim udmGias As Integer = 0
        Dim udmGiasStr As String = ""

        Try

            'L'udm che mi arriva sul file è la stessa nostra, quindi non mi serve il mapping
            If objSettings.NoMapUdm = True Then

                If udmAltro Is Nothing OrElse udmAltro = "" OrElse Not IsNumeric(udmAltro) Then
                    Throw New Exception(String.Format("L'Unità di Misura {0} [{1}] non è uguale alla UdM GIAS, ma da impostazione dovrebbe esserlo", objSettings.SuffissoAltroGestionale, If(udmAltro, "")))
                End If

                Return CInt(udmAltro)
            End If

            Dim objChiaveUdm As New ChiaveParametroAltro With {.Piva = piva, .IdAltro = udmAltro}

            'vedo se ho già incontrato questa udm
            If udmDict.ContainsKey(objChiaveUdm) Then
                udmGias = udmDict.Item(objChiaveUdm)
            Else
                'non ho ancora incontrato questa udm
                Dim objInterscambioR As New Gias_Interscambio_R

                Dim numRisultati As Integer = 0
                Dim idParam As Integer = 0
                If objSettings.TipoImport = ParametriExtra.enum_TipoInterscambio.SoloCodici Then
                    numRisultati = objInterscambioR.Leggi_CodiceParametro_GIAS_From_ALTRO(piva, True, idParam, INTERSCAMBIO_PARAMETRI_UNITA_MISURA, udmGiasStr, udmAltro, objParametriInterscambio, objSettings.SuffissoAltroGestionale)
                Else
                    numRisultati = objInterscambioR.Leggi_Parametro_GIAS_From_ALTRO(INTERSCAMBIO_PARAMETRI_UNITA_MISURA, udmGiasStr, udmAltro, objParametriInterscambio)
                End If

                If numRisultati = 0 Then
                    Throw New Exception(String.Format("Unità Misura {0} non trovata in GIAS INTERSCAMBIO.", udmAltro))
                End If

                If IsDBNull(udmGiasStr) OrElse udmGiasStr = "" OrElse Not IsNumeric(udmGiasStr) Then
                    Throw New Exception(String.Format("Unità Misura {0} trovata in GIAS INTERSCAMBIO, ma non mappata.", udmAltro))
                End If

                udmGias = CInt(udmGiasStr)

                'aggiungo la nuova udm al dizionario
                udmDict.Add(objChiaveUdm, udmGias)

            End If

            If ListCatSementi Is Nothing Then
                ListCatSementi = New List(Of String) From {
                    objSettings.SottoCat_Sementi, objSettings.SottoCat_Piantine, CStr(SEMENTI)
                }
            End If

            If MappingSementi Is Nothing Then
                'per qualche motivo sono arrivata qui senza avere ancora inizializzato  questa lista, quindi lo faccio qui al volo, tanto mi basta objSettings
                MappingSementi = New ListMappingSementi(objSettings)
            End If

            'Prima mappo e otterrò 38, e di lì lo devo trasformare in 92 o 93,

            'in alcuni casi ho solo l'udm (ad es pesi colli) e di sicuro non saranno mai sementi,
            'quindi questa verifica è inutile che la faccio
            If ((catAltro IsNot Nothing AndAlso catAltro <> "" AndAlso ListCatSementi.Contains(catAltro)) OrElse
                (catGias IsNot Nothing AndAlso ListCatSementi.Contains(catGias)) OrElse
                (sottoCategoria IsNot Nothing AndAlso sottoCategoria <> "")) AndAlso
               objSettings.SdoppiaSementi = True Then

                Return MappingSementi.GetUdmGias(objSettings, sottoCategoria, udmGias, catAltro)

            End If

        Catch ex As Exception
            Dim innerMsg As String = If(ex.InnerException Is Nothing, "", " [" & ex.InnerException.Message & "]")
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message & innerMsg)
        End Try

        Return udmGias
    End Function

    Public Function MappaMagazzino(ByRef magazziniDict As Dictionary(Of ChiaveParametroAltro, String),
                                   ByVal magazAltro As String,
                                   ByRef objSettings As ParametriExtra,
                                   ByRef objInfoLogHelper As XML_Universal_Import_Log_Helper,
                                   ByRef objParametriServer As AgronicaCoreParametri,
                                   ByRef objParametriInterscambio As AgronicaCoreParametri,
                                   ByRef errore As String,
                                   ByRef messaggioErrore As String,
                                   ByRef mailMsg As StringBuilder
                                   ) As String

        Dim magazGias As String = ""

        Try
            Dim objChiaveMag As New ChiaveParametroAltro With {.Piva = objInfoLogHelper.Piva, .IdAltro = magazAltro}

            'vedo se ho già incontrato questo magazzino
            If magazziniDict.ContainsKey(objChiaveMag) Then
                magazGias = magazziniDict.Item(objChiaveMag)
            Else
                'non ho ancora incontrato questo magazzino
                Dim objInterscambioR As New Gias_Interscambio_R

                'INTERSCAMBIO --> Leggo magazzino
                Dim numRisultati As Integer = 0
                Dim idParam As Integer = 0
                If objSettings.TipoImport = enum_TipoInterscambio.SoloCodici Then
                    numRisultati = objInterscambioR.Leggi_CodiceParametro_GIAS_From_ALTRO(objInfoLogHelper.Piva, False, idParam,
                                                                                          INTERSCAMBIO_PARAMETRI_MAGAZZINO,
                                                                                          magazGias, magazAltro,
                                                                                          objParametriInterscambio,
                                                                                          objSettings.SuffissoAltroGestionale)
                Else
                    numRisultati = objInterscambioR.Leggi_Parametro_GIAS_From_ALTRO(INTERSCAMBIO_PARAMETRI_MAGAZZINO,
                                                                                    magazGias, magazAltro,
                                                                                    objParametriInterscambio)
                End If

                If numRisultati = 0 Then
                    Throw New Exception(String.Format("Magazzino {0} non trovato in GIAS INTERSCAMBIO.", magazAltro))
                End If

                If IsDBNull(magazGias) OrElse magazGias = "" Then
                    Throw New Exception(String.Format("Magazzino {0} trovato in GIAS INTERSCAMBIO, ma non mappato.", magazAltro))
                End If

                'aggiungo il nuovo magazzino al dizionario
                magazziniDict.Add(objChiaveMag, magazGias)

            End If

        Catch ex As Exception
            Dim innerMsg As String = If(ex.InnerException Is Nothing, "", ex.InnerException.Message)
            Dim msg As String = String.Format("{0}{1} [{2}]", objInfoLogHelper.Sezione, ex.Message, innerMsg)

            Log.LoggaOperazioneDbXML(objInfoLogHelper.Piva, objInfoLogHelper,
                                 enum_WFlow_Import_XML_Universale.File_Importazione_Errore,
                                 msg, Now, objParametriServer)

            mailMsg.AppendLine(String.Format("[{0}] File: {1} - {2}.",
                                             objInfoLogHelper.TipoXml, objInfoLogHelper.NomeFileWork, msg))

            errore &= "Errore " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True, vbCrLf)
            messaggioErrore = ex.Message
            Return ""
        End Try

        Return magazGias

    End Function

    Public Function MappaCella(ByRef celleDict As Dictionary(Of ChiaveParametroAltro, String),
                               ByVal cellaAltro As String,
                               ByRef objSettings As ParametriExtra,
                               ByRef objInfoLogHelper As XML_Universal_Import_Log_Helper,
                               ByRef objParametriServer As AgronicaCoreParametri,
                               ByRef objParametriInterscambio As AgronicaCoreParametri,
                               ByRef errore As String,
                               ByRef messaggioErrore As String,
                               ByRef mailMsg As StringBuilder
                               ) As String

        Dim cellaGias As String = ""

        Try
            Dim objChiaveCella As New ChiaveParametroAltro With {.Piva = objInfoLogHelper.Piva, .IdAltro = cellaAltro}

            'vedo se ho già incontrato questa cella
            If celleDict.ContainsKey(objChiaveCella) Then
                cellaGias = celleDict.Item(objChiaveCella)
            Else
                'non ho ancora incontrato questa cella
                Dim objInterscambioR As New Gias_Interscambio_R

                'INTERSCAMBIO --> Leggo cella
                Dim numRisultati As Integer = 0
                Dim idParam As Integer = 0
                If objSettings.TipoImport = enum_TipoInterscambio.SoloCodici Then
                    numRisultati = objInterscambioR.Leggi_CodiceParametro_GIAS_From_ALTRO(objInfoLogHelper.Piva, False, idParam,
                                                                                          INTERSCAMBIO_PARAMETRI_CELLA,
                                                                                          cellaGias, cellaAltro,
                                                                                          objParametriInterscambio,
                                                                                          objSettings.SuffissoAltroGestionale)
                Else
                    numRisultati = objInterscambioR.Leggi_Parametro_GIAS_From_ALTRO(INTERSCAMBIO_PARAMETRI_CELLA,
                                                                                    cellaGias, cellaAltro,
                                                                                    objParametriInterscambio)
                End If

                If numRisultati = 0 Then
                    Throw New Exception(String.Format("Cella {0} non trovata in GIAS INTERSCAMBIO.", cellaAltro))
                End If

                If IsDBNull(cellaGias) OrElse cellaGias = "" Then
                    Throw New Exception(String.Format("Cella {0} trovata in GIAS INTERSCAMBIO, ma non mappata.", cellaAltro))
                End If

                'aggiungo la nuova cella al dizionario
                celleDict.Add(objChiaveCella, cellaGias)

            End If

        Catch ex As Exception
            Dim innerMsg As String = If(ex.InnerException Is Nothing, "", ex.InnerException.Message)
            Dim msg As String = String.Format("{0}{1} [{2}]", objInfoLogHelper.Sezione, ex.Message, innerMsg)

            Log.LoggaOperazioneDbXML(objInfoLogHelper.Piva, objInfoLogHelper,
                                 enum_WFlow_Import_XML_Universale.File_Importazione_Errore,
                                 msg, Now, objParametriServer)

            mailMsg.AppendLine(String.Format("[{0}] File: {1} - {2}.",
                                             objInfoLogHelper.TipoXml, objInfoLogHelper.NomeFileWork, msg))

            errore &= "Errore " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True, vbCrLf)
            messaggioErrore = ex.Message
            Return ""
        End Try

        Return cellaGias

    End Function

    Public Function MappaMagazzinoAltro(ByRef magazziniDict As Dictionary(Of ChiaveParametroAltro, String),
                                        ByVal piva As String,
                                        ByVal magazGias As String,
                                        ByRef objSettings As ParametriExtra,
                                        ByRef objInfoLogHlp As XML_Universal_Log_Helper,
                                        ByRef objParametriServer As AgronicaCoreParametri,
                                        ByRef objParametriInterscambio As AgronicaCoreParametri,
                                        ByRef errore As String,
                                        ByRef messaggioErrore As String,
                                        ByRef mailMsg As StringBuilder
                                        ) As String

        Const nomeRoutine = "MappaMagazzinoAltro"
        Dim magazAltro As String = ""

        Try
            Dim objChiaveMag As New ChiaveParametroAltro With {.Piva = piva, .IdGias = magazGias}

            'vedo se ho già incontrato questo magazzino
            If magazziniDict.ContainsKey(objChiaveMag) Then
                magazAltro = magazziniDict.Item(objChiaveMag)
            Else
                'non ho ancora incontrato questo magazzino
                Dim objInterscambioR As New Gias_Interscambio_R

                'INTERSCAMBIO --> Leggo magazzino
                Dim numRisultati As Integer = 0
                Dim idParam As Integer = 0
                If objSettings.TipoImport = enum_TipoInterscambio.SoloCodici Then
                    numRisultati = objInterscambioR.Leggi_CodiceParametro_ALTRO_From_GIAS(piva, False, idParam,
                                                                                          INTERSCAMBIO_PARAMETRI_MAGAZZINO,
                                                                                          magazGias, magazAltro,
                                                                                          objParametriInterscambio,
                                                                                          objSettings.SuffissoAltroGestionale)
                Else
                    numRisultati = objInterscambioR.Leggi_Parametro_ALTRO_From_GIAS(INTERSCAMBIO_PARAMETRI_MAGAZZINO,
                                                                                    magazGias, magazAltro,
                                                                                    objParametriInterscambio)
                End If

                If numRisultati = 0 Then
                    Throw New Exception(String.Format("Magazzino {0} non trovato in GIAS INTERSCAMBIO.", magazGias))
                End If

                If IsDBNull(magazAltro) OrElse magazAltro = "" Then
                    Throw New Exception(String.Format("Magazzino {0} trovato in GIAS INTERSCAMBIO, ma non mappato.", magazGias))
                End If

                'aggiungo il nuovo magazzino al dizionario
                magazziniDict.Add(objChiaveMag, magazAltro)

            End If

        Catch ex As Exception
            Dim innerMsg As String = If(ex.InnerException Is Nothing, "", ex.InnerException.Message)
            Dim msg As String = String.Format("{0}{1} [{2}]", objInfoLogHlp.Sezione, ex.Message, innerMsg)

            'Log.LoggaOperazioneDbXML(piva, objInfoLogHlp,
            '                     enum_WFlow_Import_XML_Universale.File_Importazione_Errore,
            '                     msg, Now, objParametriServer)

            mailMsg.AppendLine(String.Format("[{0}] File: {1} - {2}.",
                                             objInfoLogHlp.TipoXml.ToString(), objInfoLogHlp.NomeFile, msg))

            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)

            'errore &= "Errore " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True, vbCrLf)
            'messaggioErrore = ex.Message
            'Return ""
        End Try

        Return magazAltro

    End Function

    ''' <summary>
    ''' Per sdoppiamento Sementi mi serve sottoCategoria, catAltro o catGias
    ''' perché a seconda del contesto potrei aver già ricavato l'elemCod
    ''' </summary>
    Public Function MappaUdmAltro(ByVal piva As String,
                                  ByRef udmDict As Dictionary(Of ChiaveParametroAltro, String),
                                  ByRef objSettings As ParametriExtra,
                                  ByRef objParametriInterscambio As AgronicaCoreParametri,
                                  ByVal udmGias As Integer,
                                  ByRef sottoCategoria As String,
                                  Optional ByVal catAltro As String = Nothing,
                                  Optional ByVal catGias As Integer? = Nothing,
                                  Optional ByVal udmPref As String = ""
                                  ) As String

        Const nomeRoutine = "MappaUdmAltro"
        Dim udmAltro As String = ""

        Try

            'TODO: ?!?

            ''L'udm che metto sul sul file è la nostra, quindi non mi serve il mapping
            'If objSettings.NoMapUdm = True Then

            '    If udmGias <> 0 Then
            '        Throw New Exception(String.Format("L'Unità di Misura [{0}] è zero", udmGias))
            '    End If

            '    udmAltro = CStr(udmGias)
            '    Return udmAltro
            'End If

            Dim objChiaveUdm As New ChiaveParametroAltro With {.Piva = piva, .IdGias = udmGias}


            ''TODO: se 92 o 93 devo prima sostituirlo con 38
            ''Prima mappo e otterrò 38, e di lì lo devo trasformare in 92 o 93,

            ''in alcuni casi ho solo l'udm (ad es pesi colli) e di sicuro non saranno mai sementi,
            ''quindi questa verifica è inutile che la faccio
            'If ((catAltro IsNot Nothing AndAlso catAltro <> "" AndAlso ListCatSementi.Contains(catAltro)) OrElse
            '    (catGias IsNot Nothing AndAlso ListCatSementi.Contains(catGias)) OrElse
            '    (sottoCategoria IsNot Nothing AndAlso sottoCategoria <> "")) AndAlso objSettings.SdoppiaSementi = True Then

            '    Return MappingSementi.GetUdmAltro(objSettings, sottoCategoria, udmGias, catGias)

            'End If



            'vedo se ho già incontrato questa udm
            If udmDict.ContainsKey(objChiaveUdm) Then
                udmAltro = udmDict.Item(objChiaveUdm)
            Else
                'non ho ancora incontrato questa udm
                Dim objInterscambioR As New Gias_Interscambio_R

                'INTERSCAMBIO --> Leggo udm
                Dim numRisultati As Integer = 0
                Dim idParam As Integer = 0
                If objSettings.TipoImport = ParametriExtra.enum_TipoInterscambio.SoloCodici Then
                    numRisultati = objInterscambioR.Leggi_CodiceParametro_ALTRO_From_GIAS(piva, True, idParam,
                                                                                          INTERSCAMBIO_PARAMETRI_UNITA_MISURA,
                                                                                          CStr(udmGias), udmAltro,
                                                                                          objParametriInterscambio,
                                                                                          objSettings.SuffissoAltroGestionale, xOrderBy:="")
                Else
                    numRisultati = objInterscambioR.Leggi_Parametro_ALTRO_From_GIAS(INTERSCAMBIO_PARAMETRI_UNITA_MISURA,
                                                                                    CStr(udmGias), udmAltro,
                                                                                    objParametriInterscambio)
                End If

                If numRisultati = 0 Then
                    Throw New Exception(String.Format("Unità Misura {0} non trovata in GIAS INTERSCAMBIO.", udmGias))
                End If


                'TODO: se 38 potrei avere 2 record (PZ o NR), quindi devo leggere una nuova opzione che mi dica qual è il default...

                If numRisultati > 1 AndAlso Not String.IsNullOrEmpty(udmPref) Then
                    'Per qualche motivo ho un mapping 1 udm GIAS --> molti udm ALTRO,
                    'quindi provo a rifare la lettura per vedere se su una delle due udm ho aggiunto la descrizione "PREF_CONSUMI"

                    Dim udmAltroTemp As String = udmAltro
                    Dim filtro As String = String.Format(" Descr_Param LIKE '%({0})%' ", udmPref)

                    If objSettings.TipoImport = ParametriExtra.enum_TipoInterscambio.SoloCodici Then
                        numRisultati = objInterscambioR.Leggi_CodiceParametro_ALTRO_From_GIAS(piva, True, idParam,
                                                                                              INTERSCAMBIO_PARAMETRI_UNITA_MISURA,
                                                                                              CStr(udmGias), udmAltro,
                                                                                              objParametriInterscambio,
                                                                                              objSettings.SuffissoAltroGestionale,
                                                                                              xFiltroAggiuntivo:=filtro)
                    Else
                        numRisultati = objInterscambioR.Leggi_Parametro_ALTRO_From_GIAS(INTERSCAMBIO_PARAMETRI_UNITA_MISURA,
                                                                                        CStr(udmGias), udmAltro,
                                                                                        objParametriInterscambio,
                                                                                        xFiltroAggiuntivo:=filtro)
                    End If

                    If numRisultati = 0 Then
                        'La rilettura non è stata utile (non avevo marcato nessuna udm come preferita), quindi rimetto quella precedente (che era stata la prima trovata)
                        udmAltro = udmAltroTemp
                    ElseIf numRisultati = 1 Then
                        'Sono riuscita ad identificare un'unica udm da usare
                        'udmAltro va già bene, perché è quella predefinita
                    Else
                        'La rilettura non è stata utile (perché ho cmq più righe), quindi scelgo una dell'ultima lettura (di default prende il primo record )
                        'udmAltro letta per ultima va bene
                    End If


                    '???
                End If

                If IsDBNull(udmAltro) OrElse udmAltro = "" Then
                    Throw New Exception(String.Format("Unità Misura {0} trovata in GIAS INTERSCAMBIO, ma non mappata.", udmGias))
                End If

                'aggiungo la nuova udm al dizionario
                udmDict.Add(objChiaveUdm, udmAltro)

            End If

        Catch ex As Exception
            Dim innerMsg As String = If(ex.InnerException Is Nothing, "", ex.InnerException.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message & "[" & innerMsg & "]")
        End Try

        Return udmAltro

    End Function

#End Region

#Region "Verifiche"

    ' verifica piva
    Public Shared Function VerificaPiva(ByVal piva As String,
                                        ByRef objParametriServer As AgronicaCoreParametri,
                                        ByRef objParametriInterscambio As AgronicaCoreParametri,
                                        Optional ByVal autoMapImpresaGestita As Boolean = True
                                        ) As Boolean

        Dim interscambioR As New Gias_Interscambio_R
        Dim interscambioW As New Gias_Interscambio_W
        Dim dt As DataTable = interscambioR.LeggiImpreseGestite(0, piva, "", "", objParametriInterscambio, Now)

        If dt Is Nothing OrElse dt.Rows.Count = 0 Then

            Dim impreseR As New AgronicaCoreAnagrafeDAL.Imprese_Read
            Dim dtImprese As DataTable = impreseR.EsisteRecordInTabellaImprese(piva, objParametriServer)

            If dtImprese Is Nothing OrElse dtImprese.Rows.Count = 0 Then
                Return False
            End If

            If autoMapImpresaGestita Then
                interscambioW.Scrivi_ImpreseGestite(piva, objParametriInterscambio)
            Else
                Throw New Exception(String.Format("L'impresa {0} non è tra quelle gestite", piva))
            End If
        End If

        Return True

    End Function

    ' verifica contatto
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="codContattoAltro">Usato per filtro</param>
    ''' <param name="suffissoAltro">Usato per filtro</param>
    ''' <param name="pivaProprietariaContattoGias">Usata come filtro in tab Contatti in caso di inserimento record in tab interscambio</param>
    ''' <param name="codContattoGias">Eventuale inserimento record in tab interscambio</param>
    ''' <param name="codContattoGiasInterscambio">Usato come ritorno</param>
    ''' <param name="pivaContattoInterscambio">Usato come ritorno</param>
    ''' <param name="objParametriServer"></param>
    ''' <param name="objParametriInterscambio"></param>
    ''' <returns></returns>
    Public Shared Function VerificaContatto(ByVal codContattoAltro As String,
                                            ByVal suffissoAltro As String,
                                            ByVal pivaProprietariaContattoGias As String,
                                            ByVal codContattoGias As String,
                                            ByRef codContattoGiasInterscambio As String,
                                            ByRef pivaContattoInterscambio As String,
                                            ByRef objParametriServer As AgronicaCoreParametri,
                                            ByRef objParametriInterscambio As AgronicaCoreParametri
                                            ) As String

        Dim mesContattoNonMappato As String = ""

        Dim interscambioR As New Gias_Interscambio_R
        Dim interscambioW As New Gias_Interscambio_W
        Dim numElemTrovati = interscambioR.Leggi_CodiceContatto_GIAS_From_ALTRO("", Nothing, codContattoAltro, codContattoGiasInterscambio, pivaContattoInterscambio, objParametriInterscambio, suffissoAltro)

        If numElemTrovati = 0 Then

            Dim handleContatti As New Contatti_R()

            Dim dtContatti = handleContatti.Leggi(pivaProprietariaContattoGias, codContattoGias, 0, 0, True, False, 0, 0, False, 0, ID_CF_NOFILTRO, 0, "", False, 0, 0, 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE, False, "", "", objParametriServer)

            If dtContatti.Rows.Count > 0 Then

                Dim descrContatto As String = dtContatti.Rows(0)("Rag_Soc") & " " & dtContatti.Rows(0)("Nome") & " " & dtContatti.Rows(0)("Cognome")
                Dim piva As String = dtContatti.Rows(0)("Piva")

                Dim ris = interscambioW.Scrivi_CodiciAnagrafiche(codContattoAltro, codContattoGias, descrContatto.Trim(), piva, objParametriInterscambio, suffissoAltro)
                codContattoGiasInterscambio = codContattoGias
                pivaContattoInterscambio = piva
            Else
                mesContattoNonMappato = String.Format("Contatto {0} non trovato in Gias", codContattoGias)
            End If

        End If

        Return mesContattoNonMappato

    End Function

    ' verifica magazzino
    Public Shared Function VerificaMagazzino(ByVal piva As String,
                                             ByVal magazzino As String,
                                             ByRef objParametriServer As AgronicaCoreParametri,
                                             ByRef objParametriInterscambio As AgronicaCoreParametri,
                                             Optional ByVal autoMapMagazzino As Boolean = True,
                                             Optional ByVal chiaveMagazGias As String = "",
                                             Optional ByVal desMagazGias As String = ""
                                             ) As Boolean


        Dim interscambioR As New Gias_Interscambio_R
        Dim interscambioW As New Gias_Interscambio_W

        Dim idParam As Integer = 0
        Dim magazGias As String = ""
        Dim numRisultati = interscambioR.Leggi_CodiceParametro_GIAS_From_ALTRO(
            piva, False, idParam, INTERSCAMBIO_PARAMETRI_MAGAZZINO, magazGias, magazzino,
            objParametriInterscambio,)

        If numRisultati = 0 Then

            If chiaveMagazGias = "" Then
                Dim magazziniR As New AgronicaCoreAnagrafeDAL.Fabbricati_R
                Dim dt = magazziniR.Leggi(piva, 0, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametriServer)

                If dt Is Nothing OrElse dt.Rows.Count = 0 Then
                    Return False
                End If

                magazGias = dt.Rows(0).Item("Sa_Cod") & "-" & dt.Rows(0).Item("Fabbricato_Cod")
                desMagazGias = dt.Rows(0).Item("Fabbricato_Des")
            Else
                'Parametro opzionale per poter mappare su uno specifico magazzino, attraverso l'autoMap
                magazGias = chiaveMagazGias
            End If

            If autoMapMagazzino Then
                interscambioW.Scrivi_CodiciParametri(INTERSCAMBIO_PARAMETRI_MAGAZZINO, magazGias, magazzino,
                                                     desMagazGias, piva, objParametriInterscambio)
            Else
                'TODO: lanciare eccezione?!?
            End If

        End If

        Return True

    End Function

#End Region

#Region "Export Tramite REST API"

    Public Shared Function InvioRestInterscambio(Of T)(ByRef objSettings As ParametriExtra,
                                                       ByVal piva As String,
                                                       ByRef objToExport As T,
                                                       ByRef erroriInvio As String,
                                                       ByRef msgInvio As String,
                                                       Optional ByRef Dati_Inviati As String = ""
                                                       ) As Boolean

        Const nomeRoutine = "InvioRestInterscambio"
        Dim xRisp As Boolean = False

        Try

            'Devo sapere qual è endpoint da chiamare e l'autorizzazione
            If String.IsNullOrEmpty(objSettings.WsEndpoint) Then
                Throw New Exception("Non è stato specificato l'Endpoint API da richiamare")
            End If

            If String.IsNullOrEmpty(objSettings.WsAuthorization.Chiave) Then
                Throw New Exception("Non è stato specificato l'autorizzazione per l'API da richiamare")
            End If



            'TODO: costruisci oggetto con piva = piva e dati = base64 di objToExport serializzato

            Dim xmlSettings As New XmlWriterSettings() With {.Encoding = New UTF8Encoding(False), .Indent = True}
            Dim xmlString As String = XMLUtility.SerializeToString(Of T)(objToExport, xmlSettings)


            Dim objToApi As Object = New With {
                .piva = piva,
                .dati = AgroZip.CompressioneBase64(1, xmlString)
            }

            Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
            'Dim jsonInput As String = JsonConvert.SerializeObject(objApiListaRichieste, settingLoc)

            If objSettings.ModalitaDemetra Then

                Dim xmlSettingsLog As New XmlWriterSettings() With {.Encoding = New UTF8Encoding(False), .Indent = False}
                Dim xmlStringLog As String = XMLUtility.SerializeToString(Of T)(objToExport, xmlSettingsLog)

                Dim objToLog As Object = New With {
                    .piva = piva,
                    .dati = xmlStringLog
                }

                Dati_Inviati = JsonConvert.SerializeObject(objToLog, settingLoc)
            End If


            'Eseguo la chiamata ed invio l'oggetto
            Dim customHeaders As System.Net.WebHeaderCollection = Nothing
            If objSettings.WsAuthorization.Chiave <> "NO AUTH" Then

                If objSettings.WsAuthorizationType = enum_TipoAutorizzazioneWS.OAuth2 Then

                    If String.IsNullOrEmpty(objSettings.TokenBearer) Then
                        Dim token As RispostaStandard = APICalls.GetToken(objSettings.WsOAuth2)

                        If token.RispostaOK AndAlso Not String.IsNullOrEmpty(token.RispostaStringa) Then
                            objSettings.TokenBearer = token.RispostaStringa
                        Else
                            Throw New Exception(String.Format("Autorizzazione negata: [{0}]", token.Errore))
                        End If
                    End If

                    Dim valoreTokenHeader As String = String.Format("Bearer {0}", objSettings.TokenBearer)

                    customHeaders = New System.Net.WebHeaderCollection From {
                        {objSettings.WsAuthorization.Chiave, valoreTokenHeader}
                    }

                ElseIf objSettings.WsAuthorizationType = enum_TipoAutorizzazioneWS.Token_Bearer Then

                    'Questa è la modalità che usiamo con la maggior parte dei clienti;
                    'per evitare di dover passare da tutti a cambiare la configurazione lascio per il momento
                    'il codice come era scritto precedentemente (anche se uguale al caso default)
                    customHeaders = New System.Net.WebHeaderCollection From {
                        {objSettings.WsAuthorization.Chiave, objSettings.WsAuthorization.Valore}
                    }

                Else
                    customHeaders = New System.Net.WebHeaderCollection From {
                        {objSettings.WsAuthorization.Chiave, objSettings.WsAuthorization.Valore}
                    }
                End If

            End If

            Dim objHttp As New AgronicaCoreUtility.Http
            Dim response As IRestResponse = objHttp.chiamaWS_RestShapr(objToApi,
                                                                       "",
                                                                       objSettings.WsEndpoint,
                                                                       "application/json",
                                                                       Method.POST,
                                                                       "",
                                                                       "",
                                                                       customHeaders,
                                                                       TimeOut:=objSettings.WSTimeOut)

            'Verifico il responso
            Dim messaggio As String = ""
            Dim objOutputG2G As AgronicaCoreModello.G2G_Api_Response = Nothing
            Dim objOutputGeneric As AgronicaCoreModelsSTD.Utility.Api_Response = Nothing
            Dim objOutputBF_BC As BonificheFerraresi_BC_OK = Nothing

            Dim errCodeHttp As Integer = CInt(response.StatusCode)
            Dim errCodString As String = "Generic Error"
            If response.StatusCode <> 0 Then
                errCodString = [Enum].GetName(GetType(Net.HttpStatusCode), response.StatusCode)
            End If

            Select Case response.StatusCode

                Case Net.HttpStatusCode.OK, Net.HttpStatusCode.Created, Net.HttpStatusCode.Accepted

                    messaggio = response.Content

                    'Il formato della risposta potrebbe essere di tipo "G2G_Api_Response"
                    '{"messaggio":"", "errori":"", "dati": ""}

                    'ma potrebbe arrivare anche nel formato "Api_Response"
                    '{"message":"", "errore":""}

                    'Pertanto le controllo tutte e due
                    Try
                        objOutputG2G = JsonConvert.DeserializeObject(response.Content, GetType(G2G_Api_Response), settingLoc)
                    Catch exDes As Exception
                        'Lasciato volutamente vuoto per evitare che l'errore in deserializzazione possa nascondere
                        'l'eventuale errore arrivato su content, ma non con il json dell'API 
                    End Try

                    Try
                        objOutputGeneric = JsonConvert.DeserializeObject(response.Content, GetType(Api_Response), settingLoc)
                    Catch exDes As Exception
                        'Lasciato volutamente vuoto per evitare che l'errore in deserializzazione possa nascondere
                        'l'eventuale errore arrivato su content, ma non con il json dell'API 
                    End Try

                    'Per invio consumi a BC questo è il formato con cui ci rispondono se va tutto bene
                    '{"@odata.context":"https://api.businesscentral.dynamics.com/v2.0/......","value":"OK"}
                    Try
                        objOutputBF_BC = JsonConvert.DeserializeObject(response.Content, GetType(BonificheFerraresi_BC_OK), settingLoc)
                    Catch exDes As Exception
                        'Lasciato volutamente vuoto per evitare che l'errore in deserializzazione possa nascondere
                        'l'eventuale errore arrivato su content, ma non con il json dell'API 
                    End Try



                    'Verifico contenuto
                    If objOutputG2G IsNot Nothing AndAlso
                       objOutputG2G.messaggio = "OK" Then

                        If (Not String.IsNullOrEmpty(objOutputG2G.errori) AndAlso objOutputG2G.errori <> "null") OrElse
                           (Not String.IsNullOrEmpty(objOutputG2G.dati) AndAlso objOutputG2G.dati <> "null") Then
                            msgInvio = messaggio
                        End If

                        xRisp = True

                    ElseIf objOutputGeneric IsNot Nothing AndAlso
                           objOutputGeneric.message = Api_Response_Message_Type.Ok Then

                        If (Not String.IsNullOrEmpty(objOutputGeneric.errore) AndAlso objOutputGeneric.errore <> "null") Then
                            msgInvio = messaggio
                        End If

                        xRisp = True

                    ElseIf objOutputBF_BC IsNot Nothing AndAlso
                           Not String.IsNullOrEmpty(objOutputBF_BC.value) AndAlso
                           objOutputBF_BC.value.ToLower() = "ok" Then

                        'If (Not String.IsNullOrEmpty(objOutputBF_BC.errore) AndAlso objOutputBF_BC.errore <> "null") Then
                        '    msgInvio = messaggio
                        'End If

                        xRisp = True

                    Else
                        erroriInvio = String.Format("HTTP: {0} ({1}) ma Errore: {2}", errCodeHttp, errCodString, messaggio)
                        xRisp = False
                    End If

                Case Else

                    If errCodeHttp = 0 Then
                        erroriInvio = String.Format("TCP: {0} ({1}) ",
                                                    CInt(response.ResponseStatus), [Enum].GetName(GetType(ResponseStatus), response.ResponseStatus))
                    Else
                        erroriInvio = String.Format("HTTP: {0} ({1}) ", errCodeHttp, errCodString)
                    End If


                    If response.Content <> "" Then
                        Dim erroriInvioInt = response.Content

                        'Anche qui stessa cosa, devo verificare entrambi i tipi di ritorno
                        Try
                            objOutputG2G = JsonConvert.DeserializeObject(response.Content, GetType(G2G_Api_Response), settingLoc)
                        Catch exDes As Exception
                            'Lasciato volutamente vuoto per evitare che l'errore in deserializzazione possa nascondere
                            'l'eventuale errore arrivato su content, ma non con il json dell'API 
                        End Try

                        Try
                            objOutputGeneric = JsonConvert.DeserializeObject(response.Content, GetType(Api_Response), settingLoc)
                        Catch exDes As Exception
                            'Lasciato volutamente vuoto per evitare che l'errore in deserializzazione possa nascondere
                            'l'eventuale errore arrivato su content, ma non con il json dell'API 
                        End Try

                        If objOutputG2G IsNot Nothing AndAlso
                           (Not String.IsNullOrEmpty(objOutputG2G.messaggio) OrElse (Not String.IsNullOrEmpty(objOutputG2G.errori))) Then
                            erroriInvioInt = Trim(If(objOutputG2G.messaggio, "") & " " & If(objOutputG2G.errori, ""))
                        ElseIf objOutputGeneric IsNot Nothing AndAlso
                               (Not String.IsNullOrEmpty(objOutputGeneric.message) OrElse (Not String.IsNullOrEmpty(objOutputGeneric.errore))) Then
                            erroriInvioInt = Trim(If(objOutputGeneric.message, "") & " " & If(objOutputGeneric.errore, ""))
                        End If

                        erroriInvio &= erroriInvioInt

                    Else
                        Dim exc = response.ErrorException
                        If exc IsNot Nothing Then
                            erroriInvio &= exc.Message & If(IsNothing(exc.InnerException), "", " [" & exc.InnerException.Message & "]")
                        Else
                            erroriInvio &= "ERRORE chiamata Rest API"
                        End If
                    End If

                    xRisp = False
                    'Throw New Exception(erroriInvio)
            End Select

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return xRisp

    End Function

#End Region



    ' nome file interscambio
    Public Function GetNomeFileInterscambioWS(ByVal piva As String,
                                              ByVal tipo As enum_TipoNomeFile,
                                              ByRef objParametri_Server As AgronicaCoreParametri
                                              ) As String

        Dim DocNum As String = GetNomeFileProgressivo(enum_DirezioneFile.M2G, tipo, objParametri_Server)
        Return piva & "_" & DocNum & ".xml"
    End Function

    ' connessione interscambio
    Public Shared Function LeggiConnessioneInterscambio(ByVal parametri As String, ByRef objParametri_Server As AgronicaCoreParametri) As AgronicaCoreParametri

        Dim connessione As String = ""
        If String.IsNullOrEmpty(parametri) Then
            Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim DTConfigSiti As DataTable = objConfigSiti.Leggi(0, "Connessione_Interscambio", "", "", objParametri_Server)
            If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 Then
                connessione = DTConfigSiti.Rows(0).Item("Valore")
            End If
        Else
            connessione = ParametriExtra.GetParametroStr(parametri, "ConnessioneInterscambio", Nothing)
            connessione = connessione.Replace("[", "").Replace("]", "").Replace(":", "=")
        End If

        If Not String.IsNullOrEmpty(connessione) Then
            Dim objParametri_Interscambio = New AgronicaCoreParametri(objParametri_Server) With {
                    .StringaConnessione = connessione
                    }
            Return objParametri_Interscambio
        End If

        Return Nothing

    End Function

#Region "Modalità Demetra"

    Public Shared Function OttieniMappaturaContatto(ByVal idContattoFile As String,
                                                    ByRef id_pivaContattoInternoGias As String,
                                                    ByRef id_CodContattoInternoGias As String,
                                                    ByRef objParametriServer As AgronicaCoreParametri,
                                                    Optional ByVal BloccaSeNonEsistenteGias As Boolean = True,
                                                    Optional ByRef contattoIsCancellato As Boolean = False
                                                    ) As String

        Dim uidContEsterno As String = ""

        Dim arrayCodiciSistemi As String() = idContattoFile.Split({"|"c}, StringSplitOptions.None)
        If arrayCodiciSistemi.Length <> 2 Then
            Throw New Exception(String.Format("Il formato del codice contatto id [{0}] non è corretto", idContattoFile))
        End If

        Dim codiceContattoSistemaDemetra As String = arrayCodiciSistemi(0)
        Dim codiceContattoSistemaGias As String = arrayCodiciSistemi(1)

        'se presente la chiave demetra, allora sulla tabella di mapping va scritto solo il codice demetra dentro a Cod_Contatto_Altro
        If Not String.IsNullOrEmpty(codiceContattoSistemaDemetra) Then
            uidContEsterno = codiceContattoSistemaDemetra
        Else
            uidContEsterno = CONTATTO_ORIGINE_GIAS
        End If

        If Not String.IsNullOrEmpty(codiceContattoSistemaGias) Then
            Dim arrayCodiciGias As String() = codiceContattoSistemaGias.Split({"_"c}, StringSplitOptions.None)
            If arrayCodiciGias.Length <> 2 Then
                Throw New Exception(String.Format("Il formato del codice contatto id [{0}] non è corretto nella parte Gias", idContattoFile))
            End If

            id_pivaContattoInternoGias = arrayCodiciGias(0)
            id_CodContattoInternoGias = arrayCodiciGias(1)

            If String.IsNullOrEmpty(id_pivaContattoInternoGias) OrElse String.IsNullOrEmpty(id_CodContattoInternoGias) Then
                Throw New Exception(String.Format("Il formato del codice contatto id [{0}] non è corretto nella parte Gias", idContattoFile))
            End If

            Dim objContR As New AgronicaCoreAnagrafeDAL.Contatti_R
            Dim contattoEsistenteGias As Boolean = objContR.VerificaEsistenza_CodContatto(id_pivaContattoInternoGias, id_CodContattoInternoGias, objParametriServer)

            If Not contattoEsistenteGias Then
                contattoIsCancellato = True

                If BloccaSeNonEsistenteGias Then
                    Throw New Exception(String.Format("Il contatto [{0}] non è presente su Gias", codiceContattoSistemaGias))
                End If
            End If

            uidContEsterno = CONTATTO_ORIGINE_GIAS
        End If

        Return uidContEsterno

    End Function

    Public Shared Function OttieniMappaturaRisorsaUmana(ByVal idRuoloFile As String,
                                                        ByVal codSistemaEsterno As enum_SistemiEsterni,
                                                        ByVal piva As String,
                                                        ByRef codRisum As String,
                                                        ByRef ruoloCancellato As Boolean,
                                                        ByRef objParametriServer As AgronicaCoreParametri) As String

        Dim arrayCodiciSistemi = idRuoloFile.Split({"|"c}, StringSplitOptions.None)
        If arrayCodiciSistemi.Length <> 2 Then
            Throw New Exception(String.Format("Il formato dell'id ruolo [{0}] non è corretto", idRuoloFile))
        End If

        Dim idRuoloEsterno As String = arrayCodiciSistemi(0)
        codRisum = arrayCodiciSistemi(1)

        If String.IsNullOrEmpty(idRuoloEsterno) Then
            idRuoloEsterno = RUOLO_ORIGINE_GIAS
        End If

        If Not String.IsNullOrEmpty(codRisum) Then

            Dim handleRisUm As New Risorse_Umane_R()
            Dim dtRisUm = handleRisUm.Leggi3("", "", codRisum, 0, "", "", objParametriServer)

            If dtRisUm.Rows.Count = 0 Then
                ruoloCancellato = True
            Else
                idRuoloEsterno = RUOLO_ORIGINE_GIAS
            End If

        ElseIf idRuoloEsterno <> RUOLO_ORIGINE_GIAS Then

            Dim bizInterRisUm As New AgronicaCoreInterscambioBIZ.Interscambio_Risorse_Umane_R()
            Dim dtInterRisum = bizInterRisUm.Leggi_Tabella_Interscambio_ChiaveEsterna(codSistemaEsterno, idRuoloEsterno, objParametriServer)

            'Se il dt ha zero righe, sono in scrittura di un nuovo ruolo
            If dtInterRisum.Rows.Count = 1 Then
                codRisum = dtInterRisum(0)("Cod_Risum")
                idRuoloEsterno = RUOLO_ORIGINE_GIAS
            ElseIf dtInterRisum.Rows.Count > 1 Then
                Throw New Exception(String.Format("Il ruolo [{0}] è codificato in Gias su più risorse umane diverse", idRuoloEsterno))
            End If

        End If

        Return idRuoloEsterno

    End Function

    Public Function OttieniMappaturaFabbricato(ByVal idFabbricatoFile As String,
                                                      ByVal codSistemaEsterno As enum_SistemiEsterni,
                                                      ByRef fabbricato_pivaGias As String,
                                                      ByRef fabbricato_saCodGias As String,
                                                      ByRef fabbricato_codGias As String,
                                                      ByRef objParametriServer As AgronicaCoreParametri) As String

        Dim codEsterno As String

        Dim arrayCodiciSistemi = idFabbricatoFile.Split({"|"c}, StringSplitOptions.None)
        If arrayCodiciSistemi.Length <> 2 Then
            Throw New Exception(String.Format("Il formato del codice fabbricato [{0}] non è corretto", idFabbricatoFile))
        End If

        Dim idFabbricatoEsterno As String = arrayCodiciSistemi(0)
        Dim idFabbricatoGias As String = arrayCodiciSistemi(1)

        'se presente la chiave demetra, allora sulla tabella di mapping va scritto solo il codice demetra dentro a Cod_Contatto_Altro
        If Not String.IsNullOrEmpty(idFabbricatoEsterno) Then
            codEsterno = idFabbricatoEsterno
        Else
            codEsterno = FABBRICATO_ORIGINE_GIAS
        End If

        If Not String.IsNullOrEmpty(idFabbricatoGias) Then

            Dim arrayCodiciGias As String() = idFabbricatoGias.Split({"_"c}, StringSplitOptions.None)
            If arrayCodiciGias.Length <> 3 Then
                Throw New Exception(String.Format("Il formato del codice fabbricato [{0}] non è corretto nella parte Gias", idFabbricatoFile))
            End If

            fabbricato_pivaGias = arrayCodiciGias(0)
            fabbricato_saCodGias = arrayCodiciGias(1)
            fabbricato_codGias = arrayCodiciGias(2)

            If String.IsNullOrEmpty(fabbricato_pivaGias) OrElse String.IsNullOrEmpty(fabbricato_saCodGias) OrElse String.IsNullOrEmpty(fabbricato_codGias) Then
                Throw New Exception(String.Format("Il formato del codice fabbricato [{0}] non è corretto nella parte Gias", idFabbricatoFile))
            End If

            Dim handleFabbricati As New Fabbricati_R
            Dim dtFabbricati = handleFabbricati.Leggi(fabbricato_pivaGias, fabbricato_saCodGias, fabbricato_codGias, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametriServer)

            If dtFabbricati.Rows.Count = 0 Then
                Throw New Exception(String.Format("Il fabbricato [{0}] non è presente in Gias", idFabbricatoGias))
            End If

            codEsterno = FABBRICATO_ORIGINE_GIAS

        ElseIf codEsterno <> FABBRICATO_ORIGINE_GIAS Then
            Dim handleFabbricatiCodifica As New AgronicaCoreInterscambioBIZ.Interscambio_Fabbricati_R()
            Dim dtFabbricatoInterscambio = handleFabbricatiCodifica.Leggi_Tabella_Interscambio_ChiaveEsterna(codSistemaEsterno, codEsterno, objParametriServer)

            If dtFabbricatoInterscambio.Rows.Count = 0 Then
                Throw New Exception(String.Format("Il fabbricato [{0}] non è presente nella tabella di mappatura in Gias", idFabbricatoEsterno))
            ElseIf dtFabbricatoInterscambio.Rows.Count > 1 Then
                Throw New Exception(String.Format("Il fabbricato [{0}] è codificato in Gias su più fabbricati diversi", idFabbricatoEsterno))
            End If

            fabbricato_pivaGias = dtFabbricatoInterscambio(0)("Piva")
            fabbricato_saCodGias = dtFabbricatoInterscambio(0)("Sa_Cod")
            fabbricato_codGias = dtFabbricatoInterscambio(0)("Fabbricato_Cod")

            If String.IsNullOrEmpty(fabbricato_pivaGias) OrElse String.IsNullOrEmpty(fabbricato_saCodGias) OrElse String.IsNullOrEmpty(fabbricato_codGias) Then
                Throw New Exception(String.Format("Il fabbricato [{0}] non è completamente codificato nella tabella di mappatura in Gias", idFabbricatoEsterno))
            End If

        Else
            'Vorrebbe dire che nell'xml era stato scritto come valore il solo carattere separatore: '|'
            Throw New Exception(String.Format("Il formato del codice fabbricato [{0}] non è corretto", idFabbricatoFile))
        End If

        Return codEsterno

    End Function

    Public Function OttieniMappaturaAgenda(ByVal idDocFile As String,
                                                      ByRef agenda_pivaGias As String,
                                                      ByRef agenda_IdGias As String,
                                                      ByRef objParametriInterscambio As AgronicaCoreParametri) As String

        Dim codEsterno As String

        Dim arrayCodiciSistemi = idDocFile.Split({"|"c}, StringSplitOptions.None)
        If arrayCodiciSistemi.Length <> 2 Then
            Throw New Exception(String.Format("Il formato dell'id documento [{0}] non è corretto", idDocFile))
        End If

        Dim idXmlEsterno As String = arrayCodiciSistemi(0)
        Dim idXmlGias As String = arrayCodiciSistemi(1)

        If Not String.IsNullOrEmpty(idXmlEsterno) Then
            codEsterno = idXmlEsterno
        Else
            codEsterno = AGENDA_ORIGINE_GIAS
        End If

        If Not String.IsNullOrEmpty(idXmlGias) Then

            Dim arrayCodiciGias As String() = idXmlGias.Split({"_"c}, StringSplitOptions.None)
            If arrayCodiciGias.Length <> 2 Then
                Throw New Exception(String.Format("Il formato dell'id documento [{0}] non è corretto nella parte Gias", idDocFile))
            End If

            agenda_pivaGias = arrayCodiciGias(0)
            agenda_IdGias = arrayCodiciGias(1)

            If String.IsNullOrEmpty(agenda_pivaGias) OrElse String.IsNullOrEmpty(agenda_IdGias) Then
                Throw New Exception(String.Format("Il formato dell'id documento [{0}] non è corretto nella parte Gias", idDocFile))
            End If

            'TODO Verificare se aggiungere il riferimento al progetto e l'objparametriserver per poter fare questo controllo
            'Dim handleAgenda As New AgronicaCoreContabDAL.Agenda_R()
            'Dim dt = handleAgenda.Leggi(...)

            'If dt.Rows.Count = 0 Then
            '    Throw New Exception(String.Format("Il documento [{0}] non è presente in Gias", idXmlGias))
            'End If

            'ElseIf codEsterno <> AGENDA_ORIGINE_GIAS Then
            '    Dim handleDocProcessati As New Gias_Interscambio_R()
            '    Dim dt = handleDocProcessati.LeggiDocProcessati("", "", codEsterno, "", "", objParametriInterscambio)

            '    'Non va bene, perché il doc potrebbe essere in inserimento:
            '    'If dt.Rows.Count = 0 Then
            '    '    Throw New Exception(String.Format("Il documento [{0}] non è presente nella tabella di mappatura in Gias", idXmlEsterno))
            '    'End If

            '    If dt.Rows.Count > 0 Then
            '        agenda_pivaGias = dt(0)("Piva")
            '        agenda_IdGias = dt(0)("Id_Agenda")
            '    End If

            'Else
            '    'Vorrebbe dire che nell'xml era stato scritto come valore il solo carattere separatore: '|'
            '    Throw New Exception(String.Format("Il formato dell'id documento [{0}] non è corretto", idDocFile))
        End If

        Return codEsterno

    End Function

    'Public Sub Scrivi_Log_Invio_Contatti(Tipo_Esportazione As enum_Esportazioni_Sistema_Cod,
    '                                     TipoOperazione As enum_TipoOperazioneDB,
    '                                     Dati_Inviati As String,
    '                                     Esito As String,
    '                                     Dati_Ricevuti As String,
    '                                     Chiave As String,
    '                                     Chiave_Esterna As String,
    '                                     Piva As String,
    '                                     Sa_Cod As Integer,
    '                                     Cod_Contatto As String,
    '                                     Note As String,
    '                                     ObjParametri_Server As AgronicaCoreParametri,
    '                                     GiasContext As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
    '                                     Optional UtilizzaTransazione As Boolean = True,
    '                                     Optional Data_Invio As DateTime? = Nothing)

    '    Dim objLogInvioChiamate As New AgronicaCoreVarieDAL.Agronica_Log_Invio_Chiamate_W
    '    Dim objLogInvioContatti As New AgronicaCoreVarieDAL.Agronica_Log_Invio_Contatti_W

    '    Dim Tipo_Operazione As String = ""
    '    Select Case TipoOperazione
    '        Case enum_TipoOperazioneDB.Scrittura
    '            Tipo_Operazione = "INS"
    '        Case enum_TipoOperazioneDB.Modifica
    '            Tipo_Operazione = "UPD"
    '        Case enum_TipoOperazioneDB.Cancellazione
    '            Tipo_Operazione = "DEL"
    '    End Select

    '    Dim ID_LogInvioChiamata = objLogInvioChiamate.Scrivi(Tipo_Esportazione,
    '                                                         Dati_Inviati, Data_Invio,
    '                                                         Esito, Dati_Ricevuti,
    '                                                         "0", Tipo_Operazione,
    '                                                         ObjParametri_Server,
    '                                                         UtilizzaTransazione:=UtilizzaTransazione)

    '    objLogInvioContatti.Create_Agronica_Log_Invio_Contatti(Tipo_Esportazione, ID_LogInvioChiamata, 0,
    '                                                           Chiave, Chiave_Esterna,
    '                                                           Piva, 0, Cod_Contatto,
    '                                                           Note,
    '                                                           ObjParametri_Server, GiasContext)
    'End Sub

    'Public Sub Scrivi_Log_Invio_Agenda(Tipo_Esportazione As enum_Esportazioni_Sistema_Cod,
    '                                     TipoOperazione As enum_TipoOperazioneDB,
    '                                     Dati_Inviati As String,
    '                                     Esito As String,
    '                                     Dati_Ricevuti As String,
    '                                     Chiave As String,
    '                                     Chiave_Esterna As String,
    '                                     Piva As String,
    '                                     Id_Agenda As Integer,
    '                                     ObjParametri_Server As AgronicaCoreParametri,
    '                                     GiasContext As Gias_DeveloperServer_Entities,
    '                                     Optional UtilizzaTransazione As Boolean = True)

    '    Dim objLogInvioChiamate As New AgronicaCoreVarieDAL.Agronica_Log_Invio_Chiamate_W
    '    Dim objLogInvioAgenda As New AgronicaCoreVarieDAL.Agronica_Log_Invio_Agenda_W

    '    Dim Tipo_Operazione As String = ""
    '    Select Case TipoOperazione
    '        Case enum_TipoOperazioneDB.Scrittura
    '            Tipo_Operazione = "INS"
    '        Case enum_TipoOperazioneDB.Modifica
    '            Tipo_Operazione = "UPD"
    '        Case enum_TipoOperazioneDB.Cancellazione
    '            Tipo_Operazione = "DEL"
    '    End Select

    '    Dim ID_LogInvioChiamata = objLogInvioChiamate.Scrivi(Tipo_Esportazione,
    '                                                         Dati_Inviati, DateTime.Now,
    '                                                         Esito, Dati_Ricevuti,
    '                                                         0, Tipo_Operazione,
    '                                                         ObjParametri_Server,
    '                                                         Chiave, Chiave_Esterna, "",
    '                                                         UtilizzaTransazione:=UtilizzaTransazione)

    '    If GiasContext Is Nothing Then

    '        Dim gEfUtils As New Gias_EF_Utility
    '        Dim efConnString As String = gEfUtils.GetEntityConnectionString(ObjParametri_Server.StringaConnessione)

    '        GiasContext = New Gias_DeveloperServer_Entities(efConnString)

    '    End If

    '    'Nota: non valorizzo il parametro Id_Operazione_Esterna con la variabile Chiave_Esterna perché il primo è integer e la seconda string
    '    objLogInvioAgenda.Create_Agronica_Log_Invio_Agenda(Tipo_Esportazione, Id_Agenda, 0, ID_LogInvioChiamata,
    '                                                       ObjParametri_Server, GiasContext)
    'End Sub
#End Region

End Class


Public Class LogInvioModelChiamate
    Public Property TipoImportExport As enum_Esportazioni_Sistema_Cod
    Public Property Payload As String
    Public Property DataInvio As Date
    Public Property Esito As String
    Public Property Messaggio As String

    'Queste 4 proprietà hanno senso a questo livello per l'agenda e non per i contatti
    Public Property TipoOperazione As enum_TipoOperazioneDB
    Public Property ChiaveFile As String
    Public Property ChiaveEsterna As String
    Public Property Piva As String

    Public Property ListaAgende As List(Of LogInvioModelAgenda) 'Valutare di chiamarla ListaEntitaGias e di fare una classe base dal quale derivare classe di Agenda e Contatti

    Public Sub New()
        Me.TipoImportExport = 0
        Me.Payload = ""
        Me.DataInvio = Date.Now
        Me.Esito = ""
        Me.Messaggio = ""
        Me.TipoOperazione = enum_TipoOperazioneDB.Scrittura
        Me.ChiaveFile = ""
        Me.ChiaveEsterna = ""
        Me.Piva = ""
        Me.ListaAgende = New List(Of LogInvioModelAgenda)()
    End Sub

    Public Sub New(tipoImportExport As enum_Esportazioni_Sistema_Cod, payload As String, dataInvio As Date, esito As String, messaggio As String, tipoOperazione As enum_TipoOperazioneDB, chiaveFile As String, chiaveEsterna As String, piva As String, listaAgende As List(Of LogInvioModelAgenda))
        Me.TipoImportExport = tipoImportExport
        Me.Payload = payload
        Me.DataInvio = dataInvio
        Me.Esito = esito
        Me.Messaggio = messaggio
        Me.TipoOperazione = tipoOperazione
        Me.ChiaveFile = chiaveFile
        Me.ChiaveEsterna = chiaveEsterna
        Me.Piva = piva
        Me.ListaAgende = listaAgende
    End Sub
End Class

Public Class LogInvioModelAgenda
    Public Property Piva As String
    Public Property IdAgenda As Integer

    Public Function ChiaveGias() As String
        Return Piva & "_" & IdAgenda
    End Function
End Class
