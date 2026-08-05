Imports System.Data.Entity
Imports System.Data.Entity.Core.Metadata.Edm
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreContabDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports DocumentFormat.OpenXml.Spreadsheet
Imports Newtonsoft.Json.Linq

Public Class Linee_Produzione


#Region "Costruttori"

    Public Sub New()
        Provider = System.Globalization.CultureInfo.InvariantCulture
        Format = "yyyyMMdd"
        ValiditaInizio = Date.ParseExact("19000101", Format, Provider)
        ValiditaFine = Date.ParseExact("21001231", Format, Provider)
    End Sub

#End Region

    Private _format As String
    Public Shadows Property Format() As String
        Get
            Return _format
        End Get
        Set
            _format = Value
        End Set
    End Property

    Private _provider As System.Globalization.CultureInfo
    Public Shadows Property Provider() As System.Globalization.CultureInfo
        Get
            Return _provider
        End Get
        Set
            _provider = Value
        End Set
    End Property

    Private _validitaInizio As Date
    Public Shadows Property ValiditaInizio() As Date
        Get
            Return _validitaInizio
        End Get
        Set
            _validitaInizio = Value
        End Set
    End Property

    Private _validitaFine As Date
    Public Shadows Property ValiditaFine() As Date
        Get
            Return _validitaFine
        End Get
        Set
            _validitaFine = Value
        End Set
    End Property

    Public Function AggiornaRecordParametriModificati(ByVal piva As String,
           ByVal righeInserite As String,
           ByVal righeModificate As String,
           ByVal righeCancellate As String,
           ByVal tutteleRighe As String,
           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
       ) As String

        Dim Piva_SuperUser = objParametri.PivaSuperUser
        Dim esitoAggioramento As String = String.Empty
        Dim MessaggioErrore As String = String.Empty
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.LineeProduttive_W.AggiornaRecordParametriModificati()"
        ' Controlla se ci sono periodi sovrapposti all'interno delle righe che si stanno gestendo
        Try
            Dim righeInseriteArray As JArray = JArray.Parse(righeInserite)
            Dim righeModificateArray As JArray = JArray.Parse(righeModificate)
            Dim righeCancellateArray As JArray = JArray.Parse(righeCancellate)
            Dim tutteleRigheArray As JArray = JArray.Parse(tutteleRighe)

            'Array che ti servono per la parte di scrittura
            Dim EFArrayToInsert As New List(Of LineaProduttiva)
            Dim EFArrayToUpdate As New List(Of LineaProduttiva)
            Dim EFArrayToDelete As New List(Of LineaProduttiva)

            Dim isValide As Boolean = ImpostaRigheParametriInserire(righeInseriteArray, EFArrayToInsert, MessaggioErrore, objParametri)
            isValide = isValide AndAlso ImpostaRigheParametriModificate(piva, righeModificateArray, EFArrayToUpdate, MessaggioErrore, objParametri)
            isValide = isValide AndAlso ImpostaRigheParametriCancellate(piva, righeCancellateArray, EFArrayToDelete, MessaggioErrore, objParametri)

            If isValide Then
                'Parte Di scrittura
                esitoAggioramento = (New LineeProduttive_W).ScriviLineeProduzioni(piva, EFArrayToInsert, EFArrayToUpdate, EFArrayToDelete, objParametri)
            Else
                Throw New Exception(MessaggioErrore)
            End If


        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            'Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
        Finally

        End Try

        Return MessaggioErrore
    End Function

    Private Function ImpostaRigheParametriInserire(righeArray As JArray,
                                          EFArray As List(Of LineaProduttiva),
                                          ByRef messaggioErrore As String,
                                          objParametri As AgronicaCoreParametri) As Boolean
        Dim result As Boolean = False
        Dim objLineePreparazione As New AgronicaCoreAnagrafeDAL.LineePreparazioni_R
        Dim objGenerazioniAnagrafeLog As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Log_R
        messaggioErrore = String.Empty

        Dim lineeRaggruppate = righeArray _
            .GroupBy(Function(key) key("Linea_Cod_Des"), Function(gr) gr,
                     Function(k, g) New KeyValuePair(Of JToken, JToken())(g.First(), g)) _
            .ToList()

        For Each obj As KeyValuePair(Of JToken, JToken()) In lineeRaggruppate
            messaggioErrore = VerificaRigaParametroValida(obj.Key, objParametri)

            If String.IsNullOrEmpty(messaggioErrore) Then
                Dim lineaProduzione As New LineaProduttiva
                ImpostaTabellaLineeProduzioniEF(obj.Key, lineaProduzione, objParametri)

                If Not lineaProduzione.LineaPresente Then
                    lineaProduzione.Linee_Produzioni_Mix = ImpostaLineeProduzioniMix(lineaProduzione.Linee_Produzioni, objParametri.PivaSuperUser)
                End If

                Dim codiciLineePreparazioni As Integer() = obj.Value _
                    .Select(Function(x) x("Preparazione_Cod").Value(Of Integer)) _
                    .Distinct() _
                    .ToArray()

                Dim lineePreparazioni = objLineePreparazione _
                        .ImpostaLineePreparazioni(lineaProduzione.Linee_Produzioni.Piva,
                                                  codiciLineePreparazioni,
                                                  objParametri)
                If Not IsNothing(lineePreparazioni) Then
                    ImpostaTabellaLineeProduzioniXPreparazioniEF(lineaProduzione, lineePreparazioni)
                End If


                Dim statiProdotto As String() = obj.Value _
                    .Select(Function(x) x("StatoProdotto").Value(Of String)) _
                    .Distinct() _
                    .ToArray()

                    lineaProduzione.RelazioneMateriePrime = objGenerazioniAnagrafeLog _
                    .ImpostaRelazioneMateriePrimeLineaProduzione(lineaProduzione.Linee_Produzioni,
                                                                 statiProdotto,
                                                                 objParametri)


                    EFArray.Add(lineaProduzione)
                Else
                    Exit For
            End If
        Next
        result = String.IsNullOrEmpty(messaggioErrore)

        Return result

    End Function

    Private Function ImpostaRigheParametriModificate(piva As String,
                                          righeArray As JArray,
                                          EFArray As List(Of LineaProduttiva),
                                          ByRef messaggioErrore As String,
                                          objParametri As AgronicaCoreParametri) As Boolean
        Dim result As Boolean = False
        messaggioErrore = String.Empty

        Dim lineeRaggruppate = righeArray _
            .GroupBy(Function(key) key("Linea_Cod_Des"), Function(gr) gr,
                     Function(k, g) g.First()) _
            .ToList()

        For Each obj As JObject In lineeRaggruppate
            messaggioErrore = VerificaRigaParametroValida(obj, objParametri)

            If String.IsNullOrEmpty(messaggioErrore) Then
                Dim objGenerazioniAnagrafeLog As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Log_R
                Dim lineaProduzione As New LineaProduttiva With
                    {
                        .Linee_Produzioni = (New LineeProduttive_R).Leggi_LineaProduzione(obj("Piva"), obj("Linea_Cod"), objParametri)
                    }
                ImpostaTabellaLineeProduzioniEF(obj, lineaProduzione, objParametri)
                If Not String.IsNullOrEmpty(obj("Data_Creazione")) Then
                    lineaProduzione.Linee_Produzioni.Data_Creazione = Date.ParseExact(obj("Data_Creazione").ToString, Format, Provider)
                End If
                lineaProduzione.Linee_Produzioni.Username_Creazione = obj("Username_Creazione").ToString
                lineaProduzione.Linee_Produzioni.Data_Modifica = CDate(FormatDateTime(Now, 2).ToString() & " 00:00:00")
                lineaProduzione.Linee_Produzioni.Username_Modifica = objParametri.UsernameOperazione
                lineaProduzione.Linee_Produzioni.inviato = obj("inviato")

                lineaProduzione.RelazioneMateriePrime = objGenerazioniAnagrafeLog _
                            .ImpostaRelazioneMateriePrimeLineaProduzione(lineaProduzione.Linee_Produzioni, objParametri)

                EFArray.Add(lineaProduzione)
            Else
                Exit For
            End If
        Next
        result = String.IsNullOrEmpty(messaggioErrore)

        Return result

    End Function

    Private Function ImpostaRigheParametriCancellate(piva As String,
                                          righeArray As JArray,
                                          EFArray As List(Of LineaProduttiva),
                                          ByRef messaggioErrore As String,
                                          objParametri As AgronicaCoreParametri) As Boolean
        Dim objLineeProduttive As New AgronicaCoreAnagrafeDAL.LineeProduttive_R
        Dim objLineePreparazione As New AgronicaCoreAnagrafeDAL.LineePreparazioni_R
        Dim objGenerazioniAnagrafeLog As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Log_R
        Dim result As Boolean = False
        messaggioErrore = String.Empty

        Dim lineeRaggruppate = righeArray _
            .GroupBy(Function(key) key("Linea_Cod_Des"), Function(gr) gr,
                     Function(k, g) New KeyValuePair(Of JToken, JToken())(g.First(), g)) _
            .ToList()

        For Each obj As KeyValuePair(Of JToken, JToken()) In lineeRaggruppate
            Dim lineaProduzione As New LineaProduttiva
            Dim deleteAll = obj.Key("deleteAll").Value(Of Boolean)
            Dim codiciLineePreparazioni As Integer() = obj.Value _
                    .Select(Function(x) x("Preparazione_Cod").Value(Of Integer)) _
                    .Distinct() _
                    .ToArray()

            messaggioErrore = VerificaRigaParametroCancellazioneValida(piva,
                                                                       obj.Key("Linea_Cod").Value(Of Integer),
                                                                       obj.Key("Linea_Cod_Des").Value(Of String),
                                                                       deleteAll,
                                                                       codiciLineePreparazioni,
                                                                       objParametri)

            If String.IsNullOrEmpty(messaggioErrore) Then
                Dim lineaCod As Integer = obj.Key("Linea_Cod").Value(Of Integer)
                If deleteAll Then
                    lineaProduzione.Linee_Produzioni = New Linee_Produzioni With {
                        .Piva = piva,
                        .Linea_Cod = lineaCod
                    }

                    lineaProduzione.Linee_Produzioni_Mix = objLineeProduttive.Leggi_LineeProduttiveMix(lineaProduzione.Linee_Produzioni.Piva,
                                                                                               lineaProduzione.Linee_Produzioni.Linea_Cod,
                                                                                               objParametri)
                End If

                Dim relazioneLineeProduzioneXPreparazioni = objLineePreparazione _
                    .LeggiLineeProduzioneXPreparazioni(piva, lineaCod, codiciLineePreparazioni, objParametri)

                lineaProduzione.RelazioneLineePreparazioni = New List(Of RelazioneLineePreparazioni)
                If Not IsNothing(relazioneLineeProduzioneXPreparazioni) Then
                    lineaProduzione.RelazioneLineePreparazioni = relazioneLineeProduzioneXPreparazioni _
                    .Select(Function(r) New RelazioneLineePreparazioni With
                        {
                            .Linee_Preparazioni = New Linee_Preparazioni With
                            {
                                .Piva = r.Piva,
                                .Preparazione_Cod = r.Preparazione_Cod
                            },
                            .Linee_ProduzionixPreparazioni = r
                        }
                    ) _
                    .ToList()
                End If

                If deleteAll Then
                    Dim statiProdotto As String() = obj.Value _
                            .Select(Function(x) x("StatoProdotto").Value(Of String)) _
                            .Distinct() _
                            .ToArray()

                    Dim relazioneMateriePrime = objGenerazioniAnagrafeLog _
                    .LeggiRelazioneLineeProduzioneMateriePrime(piva,
                                                               lineaCod,
                                                               enum_Omni_Modulo_Generazione.FreshFood,
                                                               statiProdotto,
                                                               objParametri)
                    If Not IsNothing(relazioneMateriePrime) Then
                        lineaProduzione.RelazioneMateriePrime = relazioneMateriePrime
                    End If
                End If

                EFArray.Add(lineaProduzione)
            Else
                Exit For
            End If
        Next
        result = String.IsNullOrEmpty(messaggioErrore)


        Return result

    End Function

    Private Sub ImpostaTabellaLineeProduzioniEF(obj As JObject, ByRef lineeProduzioni As LineaProduttiva, objParametri As AgronicaCoreParametri)
        'Dim objLineeProduzione As New LineeProduttive_R

        lineeProduzioni.Linee_Produzioni = (New LineeProduttive_R).Leggi_LineaProduzione(obj("Piva"), obj("Linea_Cod"), objParametri)

        If Not IsNothing(lineeProduzioni.Linee_Produzioni) Then
            lineeProduzioni.LineaPresente = True
        Else
            lineeProduzioni.Linee_Produzioni = New Linee_Produzioni()
            lineeProduzioni.Linee_Produzioni.Piva = obj("Piva")
            lineeProduzioni.Linee_Produzioni.Linea_Cod = obj("Linea_Cod")
            lineeProduzioni.Linee_Produzioni.Modulo_Generazione = obj("Modulo_Generazione")
            lineeProduzioni.Linee_Produzioni.Cod_Contatto_Terzi = obj("Piva")
            lineeProduzioni.Linee_Produzioni.Tipo_Denominazione = 1

            lineeProduzioni.Linee_Produzioni.Colore = 0
            lineeProduzioni.Linee_Produzioni.ChkVisualizzazione_Risorse = 0
            lineeProduzioni.Linee_Produzioni.Resa = 0
            lineeProduzioni.Linee_Produzioni.Linea_Classe_Cod_Preparazione = 0
            lineeProduzioni.Linee_Produzioni.Colore_Default = 0
            lineeProduzioni.Linee_Produzioni.ChkFine_Automatica = 0
            lineeProduzioni.Linee_Produzioni.Denominazione = ""
            lineeProduzioni.Linee_Produzioni.Tipo_Lotto_Identificativo = 0
            lineeProduzioni.Linee_Produzioni.Categoria_Gias_Cod = 0
            lineeProduzioni.Linee_Produzioni.Classificazione_Gias_Cod = 0
            lineeProduzioni.Linee_Produzioni.Linea_DPI_Cod = 0
            lineeProduzioni.Linee_Produzioni.Linea_Modello_Cod = 0
            lineeProduzioni.Linee_Produzioni.ChkParametri_Automatici = 0
            lineeProduzioni.Linee_Produzioni.Grfi_Cod = 0
            lineeProduzioni.Linee_Produzioni.Filtro_Invisibili = ""
            lineeProduzioni.Linee_Produzioni.Dicitura_Gias_Cod = 0
            lineeProduzioni.Linee_Produzioni.ChkAggiornamento = 0
            lineeProduzioni.Linee_Produzioni.Deno_Gias_Cod = 0
            lineeProduzioni.Linee_Produzioni.OFiltro_Denominazione = ""
            lineeProduzioni.Linee_Produzioni.OFiltro_Colore = ""
            lineeProduzioni.Linee_Produzioni.OFiltro_Categoria = ""
            lineeProduzioni.Linee_Produzioni.OFiltro_Classificazione = ""
            lineeProduzioni.Linee_Produzioni.OFiltro_Regolamento = ""
            lineeProduzioni.Linee_Produzioni.OFiltro_Finalita = ""
            lineeProduzioni.Linee_Produzioni.OFiltro_Dicitura = ""
            lineeProduzioni.Linee_Produzioni.ChkControllo_Manuale = 0
            lineeProduzioni.Linee_Produzioni.OFiltro_Piva = ""
            lineeProduzioni.Linee_Produzioni.Note_Amministratore = ""
            lineeProduzioni.Linee_Produzioni.Caratteristica_Gias_Cod = 0
            lineeProduzioni.Linee_Produzioni.OFiltro_Caratteristica = ""
            lineeProduzioni.Linee_Produzioni.Lotto_Configurazione = ""
            lineeProduzioni.Linee_Produzioni.Separatore_Lotto = ""
            lineeProduzioni.Linee_Produzioni.ChkDisabilitaPesate = 0

            If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
                lineeProduzioni.Linee_Produzioni.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio").ToString, Format, Provider)
            End If
            If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
                lineeProduzioni.Linee_Produzioni.Validita_Fine = Date.ParseExact(obj("Validita_Fine").ToString, Format, Provider)
            End If

            lineeProduzioni.Linee_Produzioni.Data_Creazione = CDate(FormatDateTime(Now, 2).ToString() & " 00:00:00")
            lineeProduzioni.Linee_Produzioni.Username_Creazione = objParametri.UsernameOperazione
            lineeProduzioni.Linee_Produzioni.Data_Modifica = CDate(FormatDateTime(Now, 2).ToString() & " 00:00:00")
            lineeProduzioni.Linee_Produzioni.Username_Modifica = objParametri.UsernameOperazione
            lineeProduzioni.Linee_Produzioni.inviato = 0
        End If
        lineeProduzioni.Linee_Produzioni.Linea_Cod_Des = obj("Linea_Cod_Des")
        lineeProduzioni.Linee_Produzioni.Linea_Des = obj("Linea_Des")
        lineeProduzioni.Linee_Produzioni.Linea_Classe_Cod = obj("Linea_Classe_Cod")
        lineeProduzioni.Linee_Produzioni.Veg_Cod = obj("Veg_Cod")
        lineeProduzioni.Linee_Produzioni.Cul_Cod = obj("Cul_Cod")
        lineeProduzioni.Linee_Produzioni.Reg_Cod = obj("Reg_Cod")

    End Sub

    Private Sub ImpostaTabellaLineeProduzioniXPreparazioniEF(ByRef lineaProduzione As LineaProduttiva, lineePreparazioni As List(Of Linee_Preparazioni))
        Dim linea = lineaProduzione.Linee_Produzioni
        lineaProduzione.RelazioneLineePreparazioni = lineePreparazioni _
            .Select(Function(lp) New RelazioneLineePreparazioni With
                {
                    .Linee_Preparazioni = lp,
                    .Linee_ProduzionixPreparazioni = New Linee_ProduzionixPreparazioni() With
                        {
                            .Piva = linea.Piva,
                            .Linea_Cod = linea.Linea_Cod,
                            .Preparazione_Cod = lp.Preparazione_Cod,
                            .Livello = 0,
                            .ChkLoop_Start = 0,
                            .ChkLoop_End = 0,
                            .Preparazione_Cod_Vincolo = 0,
                            .inviato = linea.inviato,
                            .datainvio = linea.datainvio,
                            .Data_Creazione = linea.Data_Creazione,
                            .Data_Modifica = linea.Data_Modifica,
                            .Username_Creazione = linea.Username_Creazione,
                            .Username_Modifica = linea.Username_Modifica,
                            .Validita_Inizio = linea.Validita_Inizio,
                            .Validita_Fine = linea.Validita_Fine,
                            .Opzionale = 0,
                            .Invisibile = 0,
                            .Linea_Modello_Cod = 0,
                            .Qta_Piano = 0
                        }
                }
            ) _
            .ToList()
    End Sub

    Private Function ImpostaLineeProduzioniMix(lineaProduzione As Linee_Produzioni, pivaSuperUser As String) As Linee_Produzioni_Mix
        Dim result As New Linee_Produzioni_Mix() With {
            .Piva_SuperUser = pivaSuperUser,
            .Piva = lineaProduzione.Piva,
            .Sa_Cod = 0,
            .Linea_Cod = lineaProduzione.Linea_Cod,
            .Veg_Cod = lineaProduzione.Veg_Cod,
            .Cul_Cod = lineaProduzione.Cul_Cod,
            .Gen_Cod = 0,
            .Spe_Cod = 0,
            .Raz_Cod = 0,
            .inviato = lineaProduzione.inviato,
            .datainvio = lineaProduzione.datainvio,
            .Data_Creazione = lineaProduzione.Data_Creazione,
            .Data_Modifica = lineaProduzione.Data_Modifica,
            .Username_Creazione = lineaProduzione.Username_Creazione,
            .Username_Modifica = lineaProduzione.Username_Modifica,
            .Validita_Inizio = lineaProduzione.Validita_Inizio,
            .Validita_Fine = lineaProduzione.Validita_Fine,
            .Livello = 0
        }

        Return result
    End Function

    Private Function VerificaRigaParametroCancellazioneValida(piva As String, lineaCod As Integer, lineaDes As String,
                                                              deleteAll As Boolean, codiciLineePreparazioni As Integer(),
                                                              ByRef objParametri As AgronicaCoreParametri) As String
        Dim messaggio As String = String.Empty

        Try
            Dim objLineeProduzioni As New LineeProduttive_R
            messaggio = objLineeProduzioni.VerificaCancellazioneLineaProduzione(piva, lineaCod, lineaDes, deleteAll,
                                                                                codiciLineePreparazioni, objParametri)

        Catch ex As Exception
            If Not String.IsNullOrEmpty(messaggio) Then
                messaggio += "<br />"
            End If
            messaggio += ex.Message
        End Try

        Return messaggio
    End Function

    Private Function VerificaRigaParametroValida(obj As JObject, ByRef objParametri As AgronicaCoreParametri) As String
        Dim messaggio As String = String.Empty
        Dim descrizione As String = obj("Linea_Cod_Des")

        Dim objLineeProduzioni As New AgronicaCoreAnagrafeDAL.LineeProduttive_R
        Dim doppio As Boolean = objLineeProduzioni.VerificaDescrizioneLineaProduzione(obj("Piva"), obj("Linea_Cod"), descrizione, objParametri)

        If doppio Then
            If Not String.IsNullOrEmpty(messaggio) Then
                messaggio += "<br />"
            End If
            messaggio = "La descrizione '" + descrizione + "' risulta già in uso per un'altra linea"
        End If

        If obj("Linea_Cod").Value(Of Integer) = 0 Then
            Dim presente As Boolean = objLineeProduzioni.VerificaConfigurazioneLineaProduzione(obj("Piva"), obj("Reg_Cod"),
                                                                                           obj("Veg_Cod"), obj("Cul_Cod"),
                                                                                           objParametri)

            If presente Then
                If Not String.IsNullOrEmpty(messaggio) Then
                    messaggio += "<br />"
                End If
                messaggio = "La configurazione della linea '" + descrizione + "' risulta già in uso per un'altra"
            End If
        End If


        Return messaggio
    End Function

End Class
