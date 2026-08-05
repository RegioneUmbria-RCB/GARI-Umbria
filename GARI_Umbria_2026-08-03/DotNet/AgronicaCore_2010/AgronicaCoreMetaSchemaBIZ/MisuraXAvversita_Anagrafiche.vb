

Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreMetaSchemaDAL
Imports AgronicaCoreModelsSTD.metaschema
Imports Newtonsoft.Json.Linq

Public Class MisuraXAvversita_Anagrafiche_R

    Public Function LeggiMisure(ByRef objParametriServer As AgronicaCoreParametri) As List(Of MisuraPerAvversitaAnagraficaExtended)

        Dim res As New List(Of MisuraPerAvversitaAnagraficaExtended)
        Dim xRead As New AgronicaCoreMetaSchemaDAL.MisuraxAvversita_Anagrafiche_R

        Dim dt = xRead.LeggiAnagrafiche(objParametriServer)

        For Each row In dt.Rows
            Dim misura As New MisuraPerAvversitaAnagraficaExtended
            misura.CodiceMisura = CInt(row("MxAV_Cod"))
            misura.CodiceAnagrafica = CInt(row("Anag_Cod"))
            misura.Descrizione = row("Anag_Des").ToString
            misura.DPI_FlagPrivatoPubblico = CInt(row("DPI_FlagPrivatoPubblico"))
            misura.DPI_COD = CInt(row("DPI_COD"))
            misura.valoreAnagrafica = CInt(row("Anag_Valore"))

            Dim utilizzo As Boolean = xRead.VerificaUtilizzoAnagrafica(misura.CodiceMisura, objParametriServer)

            misura.cancellabile = Not utilizzo
            misura.modificabile = Not utilizzo

            misura.Veg_Cod = CInt(row("Veg_Cod"))
            misura.Veg_Des = row("Veg_Des").ToString()
            misura.Av_Cod = CInt(row("Av_Cod"))
            misura.DescrizioneAvversita = $"{row("Av_Des_Vol")} ({row("UDM_DES")})"

            res.Add(misura)
        Next

        Return res
    End Function

    Public Function LeggiMisureDaCodice(ByVal codice As Int32,
                                        ByVal DPI_FlagPrivatoPubblico As Integer,
                                        ByVal DPI_Cod As Integer,
                                        ByRef objParametri As AgronicaCoreParametri
                                        ) As List(Of MisuraPerAvversitaAnagrafica)

        Dim resp As New List(Of MisuraPerAvversitaAnagrafica)

        Dim xRead As New AgronicaCoreMetaSchemaDAL.MisuraxAvversita_Anagrafiche_R

        Dim xFiltroAggiuntivo As String = "DPI_FlagPrivatoPubblico = " & DPI_FlagPrivatoPubblico & " AND DPI_Cod = " & DPI_Cod

        Dim DT = xRead.LeggiDaCodice(codice, 0, 0, xFiltroAggiuntivo, "", objParametri)

        Dim utilizzo As Boolean

        utilizzo = xRead.VerificaUtilizzoAnagrafica(codice, objParametri)

        For Each row In DT.Rows
            Dim misura As New MisuraPerAvversitaAnagrafica With {
                .CodiceMisura = CInt(row("MxAV_Cod")),
                .CodiceAnagrafica = CInt(row("Anag_Cod")),
                .Descrizione = row("Anag_Des").ToString,
                .valoreAnagrafica = CInt(row("Anag_Valore")),
                .DPI_FlagPrivatoPubblico = CInt(row("DPI_FlagPrivatoPubblico")),
                .DPI_COD = CInt(row("DPI_COD")),
                .cancellabile = Not utilizzo,
                .modificabile = Not utilizzo
                }

            resp.Add(misura)
        Next

        Return resp

    End Function

End Class
Public Class MisuraXAvversita_Anagrafiche_W
    Inherits AgronicaCoreDataProvider.LogProvider


#Region "Costruttori"

    Public Sub New()
        Provider = Globalization.CultureInfo.InvariantCulture
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

    Private _provider As Globalization.CultureInfo
    Public Shadows Property Provider() As Globalization.CultureInfo
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

    Public Function InserisciMisure(ByVal listaMisureDaInserire As List(Of MisuraPerAvversitaAnagrafica),
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim resp As Boolean

        Dim xWrite As New AgronicaCoreMetaSchemaDAL.MisuraXAvversita_Anagrafiche_W
        Dim objSequenze As New Agro_Sequenze

        For Each misura In listaMisureDaInserire

            If misura.CodiceMisura = 0 Then
                Throw New Exception("Impossibile inserire una nuova anagrafica senza MxAV_Cod")
            End If

            If misura.CodiceAnagrafica = 0 Then
                misura.CodiceAnagrafica =
                    objSequenze.NuovoId_Tabella("MisuraPerAvversitaAnagrafica",
                                                0,
                                                Int32.MaxValue,
                                                objParametri,
                                                True)
            End If

            resp = xWrite.ScriviAnagrafica(misura.CodiceAnagrafica,
                                           misura.CodiceMisura,
                                           misura.Descrizione,
                                           misura.valoreAnagrafica,
                                           misura.DPI_FlagPrivatoPubblico,
                                           misura.DPI_COD,
                                           objParametri)

            If Not resp Then
                Exit For
            End If

        Next

        Return resp
    End Function

    Public Function Aggiorna_MisuraXAvversita_Anagrafiche(
            ByVal righeInserite As String,
            ByVal righeModificate As String,
            ByVal righeCancellate As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As String

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim MessaggioErrore As String = String.Empty

        Dim NomeRoutine As String = "MisuraXAvversita_Anagrafiche_W.Aggiorna_MisuraXAvversita_Anagrafiche_W()"

        Try
            Dim mavAnagRead As New AgronicaCoreMetaSchemaDAL.MisuraxAvversita_Anagrafiche_R

            Dim curMisuraXAvversita_Anagrafiche As New MisuraXAvversita_Anagrafiche

            Dim righeInseriteArray As JArray = JArray.Parse(righeInserite)
            Dim righeModificateArray As JArray = JArray.Parse(righeModificate)
            Dim righeCancellateArray As JArray = JArray.Parse(righeCancellate)
            Dim EFArrayToInsert As New ArrayList
            Dim EFArrayToUpdate As New ArrayList
            Dim EFArrayToDelete As New ArrayList
            For Each obj As JObject In righeInseriteArray

                Dim kendoKey As String = obj("kendoKey")
                Dim MxAV_Cod As Integer = kendoKey.Split("-")(0)

                curMisuraXAvversita_Anagrafiche = New MisuraXAvversita_Anagrafiche
                curMisuraXAvversita_Anagrafiche.MxAV_Cod = MxAV_Cod
                curMisuraXAvversita_Anagrafiche.Anag_des = obj("Anag_Des")
                curMisuraXAvversita_Anagrafiche.Anag_valore = obj("Anag_Valore")

                If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
                    curMisuraXAvversita_Anagrafiche.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio"), Format, Provider)
                Else
                    curMisuraXAvversita_Anagrafiche.Validita_Inizio = ValiditaInizio
                End If
                If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
                    curMisuraXAvversita_Anagrafiche.Validita_Fine = Date.ParseExact(obj("Validita_Fine"), Format, Provider)
                Else
                    curMisuraXAvversita_Anagrafiche.Validita_Fine = ValiditaFine
                End If
                curMisuraXAvversita_Anagrafiche.Data_Creazione = Date.Now
                curMisuraXAvversita_Anagrafiche.Username_Creazione = objParametri.UsernameOperazione
                curMisuraXAvversita_Anagrafiche.Data_Modifica = Date.Now
                curMisuraXAvversita_Anagrafiche.Username_Modifica = objParametri.UsernameOperazione
                curMisuraXAvversita_Anagrafiche.inviato = 0

                EFArrayToInsert.Add(curMisuraXAvversita_Anagrafiche)
            Next
            For Each obj As JObject In righeModificateArray

                Dim kendoKey As String = obj("kendoKey")
                Dim MxAV_Cod As Integer = kendoKey.Split("-")(0)

                curMisuraXAvversita_Anagrafiche = mavAnagRead.Leggi(MxAV_Cod, obj("Anag_Valore"), objParametri)
                If curMisuraXAvversita_Anagrafiche Is Nothing Then
                    MessaggioErrore += "Riga da aggiornare non trovata"
                Else

                    curMisuraXAvversita_Anagrafiche.Anag_des = obj("Anag_Des")

                    If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
                        curMisuraXAvversita_Anagrafiche.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio"), Format, Provider)
                    Else
                        curMisuraXAvversita_Anagrafiche.Validita_Inizio = ValiditaInizio
                    End If
                    If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
                        curMisuraXAvversita_Anagrafiche.Validita_Fine = Date.ParseExact(obj("Validita_Fine"), Format, Provider)
                    Else
                        curMisuraXAvversita_Anagrafiche.Validita_Fine = ValiditaFine
                    End If
                    curMisuraXAvversita_Anagrafiche.Data_Modifica = Date.Now
                    curMisuraXAvversita_Anagrafiche.Username_Modifica = objParametri.UsernameOperazione
                    EFArrayToUpdate.Add(curMisuraXAvversita_Anagrafiche)

                End If
            Next
            For Each obj As JObject In righeCancellateArray
                curMisuraXAvversita_Anagrafiche = New MisuraXAvversita_Anagrafiche
                Dim kendoKey As String = obj("kendoKey")
                Dim MxAV_Cod As Integer = kendoKey.Split("-")(0)
                curMisuraXAvversita_Anagrafiche.MxAV_Cod = MxAV_Cod
                curMisuraXAvversita_Anagrafiche.Anag_valore = obj("Anag_Valore")
                EFArrayToDelete.Add(curMisuraXAvversita_Anagrafiche)
            Next


            If String.IsNullOrEmpty(MessaggioErrore) Then
                Dim campConf_W As New AgronicaCoreMetaSchemaDAL.MisuraXAvversita_Anagrafiche_W

                MessaggioErrore = campConf_W.Aggiorna_MisuraXAvversita_Anagrafiche(
                      EFArrayToInsert,
                      EFArrayToUpdate,
                      EFArrayToDelete,
                      objParametri
                 )

            End If

        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
        Finally

        End Try

        If Not String.IsNullOrEmpty(MessaggioErrore) Then
            Throw New Exception(MessaggioErrore)
        End If

        Return MessaggioErrore
    End Function

    Public Function AggiornaMisure(ByVal listaMisureDaAggiornare As List(Of MisuraPerAvversitaAnagrafica),
                                   ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim resp As Boolean

        Dim xWrite As New AgronicaCoreMetaSchemaDAL.MisuraXAvversita_Anagrafiche_W
        Dim xRead As New AgronicaCoreMetaSchemaDAL.MisuraxAvversita_Anagrafiche_R

        Dim DT As DataTable

        Dim objSequenze As New Agro_Sequenze

        For Each misura In listaMisureDaAggiornare

            If misura.CodiceMisura = 0 Then
                Throw New Exception("Impossibile aggiornare un'anagrafica senza MxAV_Cod.")
            End If

            If misura.CodiceAnagrafica = 0 Then
                Throw New Exception("Impossibile aggiornare un'anagrafica senza Anag_Cod.")
            End If

            DT = xRead.LeggiDaCodice(misura.CodiceMisura, misura.CodiceAnagrafica, 0, "", "", objParametri)

            If DT Is Nothing OrElse DT.Rows.Count = 0 Then
                Throw New Exception("Anagrafica misura non trovata")
            End If

            Dim utilizzo = xRead.VerificaUtilizzoAnagrafica(misura.CodiceMisura, objParametri)

            If utilizzo Then
                Throw New Exception("Anagrafica misura utilizzata in un rilievo, impossibile modificare.")
            End If

            resp = xWrite.AggiornaAnagrafica(misura.CodiceAnagrafica,
                                             misura.CodiceMisura,
                                             misura.Descrizione,
                                             misura.valoreAnagrafica,
                                             misura.DPI_FlagPrivatoPubblico,
                                             misura.DPI_COD,
                                             objParametri)

            If Not resp Then
                Exit For
            End If

        Next

        Return resp

    End Function

    Public Function EliminaMisure(ByVal listaMisureDaEliminare As List(Of MisuraPerAvversitaAnagrafica),
                                  ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim resp As Boolean

        Dim xWrite As New AgronicaCoreMetaSchemaDAL.MisuraXAvversita_Anagrafiche_W
        Dim xRead As New AgronicaCoreMetaSchemaDAL.MisuraxAvversita_Anagrafiche_R

        Dim DT As DataTable

        Dim objSequenze As New Agro_Sequenze

        For Each misura In listaMisureDaEliminare

            If misura.CodiceMisura = 0 Then
                Throw New Exception("Impossibile eliminare un'anagrafica senza MxAV_Cod")
            End If

            If misura.CodiceAnagrafica = 0 Then
                Throw New Exception("Impossibile eliminare un'anagrafica senza Anag_Cod")
            End If

            DT = xRead.LeggiDaCodice(misura.CodiceMisura, misura.CodiceAnagrafica, 0, "", "", objParametri)

            If DT Is Nothing OrElse DT.Rows.Count = 0 Then
                Throw New Exception("Anagrafica misura non trovata")
            End If

            Dim utilizzo = xRead.VerificaUtilizzoAnagrafica(misura.CodiceMisura, objParametri)

            If utilizzo Then
                Throw New Exception("Anagrafica misura utilizzata in un rilievo, impossibile eliminare.")
            End If

            resp = xWrite.EliminaAnagrafica(misura.CodiceAnagrafica,
                                            misura.CodiceMisura,
                                            objParametri)

            If Not resp Then
                Exit For
            End If

        Next

        Return resp

    End Function
End Class