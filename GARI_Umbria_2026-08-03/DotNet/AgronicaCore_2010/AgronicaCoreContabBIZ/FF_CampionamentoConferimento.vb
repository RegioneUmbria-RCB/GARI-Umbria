Imports System.Data
Imports AgronicaCoreDataProvider.UtilityProvider
Imports System.Transactions
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreContabDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class FF_CampionamentoConferimento
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

    Public Function Aggiorna_TestataGriglia_Campionamento(ByVal piva As String,
                                                          ByVal righeInserite As String,
                                                          ByVal righeModificate As String,
                                                          ByVal righeCancellate As String,
                                                          ByRef objParametri As AgronicaCoreParametri
                                                          ) As String

        Const nomeRoutine = "ContabBIZ.FF_CampionamentoConferimento.Aggiorna_TestataGriglia_Campionamento()"
        Dim messaggioErrore As String = String.Empty

        Try
            Dim campConf_R As New FF_CampionamentoConferimento_R

            Dim campConfTestataGriglia As New CampionamentoConferito_TestataGriglia
            Dim righeInseriteArray As JArray = JArray.Parse(righeInserite)
            Dim righeModificateArray As JArray = JArray.Parse(righeModificate)
            Dim righeCancellateArray As JArray = JArray.Parse(righeCancellate)
            Dim EFArrayToInsert As New ArrayList
            Dim EFArrayToUpdate As New ArrayList
            Dim EFArrayToDelete As New ArrayList
            For Each obj As JObject In righeInseriteArray
                campConfTestataGriglia = New CampionamentoConferito_TestataGriglia
                campConfTestataGriglia.Piva_SuperUser = objParametri.PivaSuperUser
                campConfTestataGriglia.PIVA = piva
                campConfTestataGriglia.des_TestataGriglia = obj("des_TestataGriglia")
                If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
                    campConfTestataGriglia.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio"), Format, Provider)
                End If
                If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
                    campConfTestataGriglia.Validita_Fine = Date.ParseExact(obj("Validita_Fine"), Format, Provider)
                End If
                campConfTestataGriglia.Data_Creazione = Date.Now
                campConfTestataGriglia.Username_Creazione = objParametri.UsernameOperazione
                campConfTestataGriglia.Data_Modifica = Date.Now
                campConfTestataGriglia.Username_Modifica = objParametri.UsernameOperazione
                campConfTestataGriglia.inviato = 0
                ' Controllo che non ci siano periodi sovrapposti già registrati
                messaggioErrore += CheckTestataGriglia(campConfTestataGriglia, objParametri, False, True, False, False)
                EFArrayToInsert.Add(campConfTestataGriglia)
            Next
            For Each obj As JObject In righeModificateArray
                campConfTestataGriglia = campConf_R.Leggi_Elem_Testata_GriglieCampionamento(piva, obj("Id_TestataGriglia"), objParametri)
                If campConfTestataGriglia Is Nothing Then
                    messaggioErrore += "Riga da aggiornare " & obj("des_TestataGriglia").ToString & " non trovata"
                Else
                    'Non faccio il controllo se è cambiato solo la descrizione
                    Dim controllaSeUtilizzato = False
                    ' Testo se già utilizzato solo se sono state cambiate le date
                    If campConfTestataGriglia.Validita_Inizio <> Date.ParseExact(obj("Validita_Inizio"), Format, Provider) OrElse
                        campConfTestataGriglia.Validita_Fine <> Date.ParseExact(obj("Validita_Fine"), Format, Provider) Then
                        controllaSeUtilizzato = True
                    End If
                    campConfTestataGriglia.des_TestataGriglia = obj("des_TestataGriglia")
                    If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
                        campConfTestataGriglia.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio"), Format, Provider)
                    End If
                    If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
                        campConfTestataGriglia.Validita_Fine = Date.ParseExact(obj("Validita_Fine"), Format, Provider)
                    End If
                    campConfTestataGriglia.Data_Modifica = Date.Now
                    campConfTestataGriglia.Username_Modifica = objParametri.UsernameOperazione
                    ' Controllo che non si possano fare modifiche se ci sono movimenti collegati e che non ci siano periodi sovrapposti già registrati
                    messaggioErrore += CheckTestataGriglia(campConfTestataGriglia, objParametri, controllaSeUtilizzato, True, False, False)
                    EFArrayToUpdate.Add(campConfTestataGriglia)
                End If
            Next
            For Each obj As JObject In righeCancellateArray
                campConfTestataGriglia = New CampionamentoConferito_TestataGriglia With {
                    .Piva_SuperUser = objParametri.PivaSuperUser,
                    .PIVA = piva,
                    .Id_TestataGriglia = obj("Id_TestataGriglia")
                }
                ' Controllo che non si possano fare modifiche se ci sono movimenti collegati
                messaggioErrore += CheckTestataGriglia(campConfTestataGriglia, objParametri, True, False, True, True)
                EFArrayToDelete.Add(campConfTestataGriglia)
            Next

            ' Controlla se ci sono periodi sovrapposti all'interno delle righe che si stanno gestendo
            If String.IsNullOrEmpty(messaggioErrore) Then
                Dim testataGrigliaElem As CampionamentoConferito_TestataGriglia = Nothing
                Dim testataGrigliaToCompare As CampionamentoConferito_TestataGriglia = Nothing
                Dim trovatoErrore As Boolean = False

                ' Righe nuove
                For i As Integer = 0 To EFArrayToInsert.Count - 1
                    testataGrigliaElem = EFArrayToInsert(i)
                    For x As Integer = 0 To EFArrayToInsert.Count - 1
                        testataGrigliaToCompare = EFArrayToInsert(x)
                        If i <> x Then
                            If testataGrigliaElem.des_TestataGriglia = testataGrigliaToCompare.des_TestataGriglia AndAlso
                               ((testataGrigliaToCompare.Validita_Inizio.HasValue AndAlso
                               testataGrigliaElem.Validita_Inizio <= testataGrigliaToCompare.Validita_Inizio AndAlso
                               testataGrigliaElem.Validita_Fine >= testataGrigliaToCompare.Validita_Inizio) OrElse
                               (testataGrigliaToCompare.Validita_Fine.HasValue AndAlso
                               testataGrigliaElem.Validita_Inizio <= testataGrigliaToCompare.Validita_Fine AndAlso
                               testataGrigliaElem.Validita_Fine >= testataGrigliaToCompare.Validita_Fine) OrElse
                               (testataGrigliaToCompare.Validita_Inizio.HasValue AndAlso
                               testataGrigliaToCompare.Validita_Fine.HasValue AndAlso
                               testataGrigliaElem.Validita_Inizio >= testataGrigliaToCompare.Validita_Inizio And
                               testataGrigliaElem.Validita_Fine <= testataGrigliaToCompare.Validita_Fine) OrElse
                               (testataGrigliaToCompare.Validita_Inizio.HasValue AndAlso
                               testataGrigliaToCompare.Validita_Fine.HasValue AndAlso
                               testataGrigliaToCompare.Validita_Inizio >= testataGrigliaElem.Validita_Inizio AndAlso
                               testataGrigliaToCompare.Validita_Fine <= testataGrigliaElem.Validita_Fine)) Then
                                trovatoErrore = True
                            End If
                        End If
                    Next
                    For x As Integer = 0 To EFArrayToUpdate.Count - 1
                        testataGrigliaToCompare = EFArrayToUpdate(x)
                        If testataGrigliaElem.des_TestataGriglia = testataGrigliaToCompare.des_TestataGriglia AndAlso
                               ((testataGrigliaToCompare.Validita_Inizio.HasValue AndAlso
                               testataGrigliaElem.Validita_Inizio <= testataGrigliaToCompare.Validita_Inizio AndAlso
                               testataGrigliaElem.Validita_Fine >= testataGrigliaToCompare.Validita_Inizio) OrElse
                               (testataGrigliaToCompare.Validita_Fine.HasValue AndAlso
                               testataGrigliaElem.Validita_Inizio <= testataGrigliaToCompare.Validita_Fine AndAlso
                               testataGrigliaElem.Validita_Fine >= testataGrigliaToCompare.Validita_Fine) OrElse
                               (testataGrigliaToCompare.Validita_Inizio.HasValue AndAlso
                               testataGrigliaToCompare.Validita_Fine.HasValue AndAlso
                               testataGrigliaElem.Validita_Inizio >= testataGrigliaToCompare.Validita_Inizio AndAlso
                               testataGrigliaElem.Validita_Fine <= testataGrigliaToCompare.Validita_Fine) OrElse
                               (testataGrigliaToCompare.Validita_Inizio.HasValue AndAlso
                               testataGrigliaToCompare.Validita_Fine.HasValue AndAlso
                               testataGrigliaToCompare.Validita_Inizio >= testataGrigliaElem.Validita_Inizio AndAlso
                               testataGrigliaToCompare.Validita_Fine <= testataGrigliaElem.Validita_Fine)) Then
                            trovatoErrore = True
                        End If
                    Next

                    If trovatoErrore Then
                        messaggioErrore &= "Esistono altre righe in periodi sovrapposti con la descrizione " & testataGrigliaElem.des_TestataGriglia & "<br/>"
                    End If
                Next

                If Not trovatoErrore Then
                    ' Righe modificate
                    For i As Integer = 0 To EFArrayToUpdate.Count - 1
                        testataGrigliaElem = EFArrayToUpdate(i)
                        For x As Integer = 0 To EFArrayToUpdate.Count - 1
                            testataGrigliaToCompare = EFArrayToUpdate(x)
                            If i <> x Then
                                If testataGrigliaElem.des_TestataGriglia = testataGrigliaToCompare.des_TestataGriglia AndAlso
                               ((testataGrigliaToCompare.Validita_Inizio.HasValue AndAlso
                               testataGrigliaElem.Validita_Inizio <= testataGrigliaToCompare.Validita_Inizio AndAlso
                               testataGrigliaElem.Validita_Fine >= testataGrigliaToCompare.Validita_Inizio) OrElse
                               (testataGrigliaToCompare.Validita_Fine.HasValue AndAlso
                               testataGrigliaElem.Validita_Inizio <= testataGrigliaToCompare.Validita_Fine AndAlso
                               testataGrigliaElem.Validita_Fine >= testataGrigliaToCompare.Validita_Fine) OrElse
                               (testataGrigliaToCompare.Validita_Inizio.HasValue AndAlso
                               testataGrigliaToCompare.Validita_Fine.HasValue AndAlso
                               testataGrigliaToCompare.Validita_Inizio <= testataGrigliaElem.Validita_Inizio AndAlso
                               testataGrigliaToCompare.Validita_Fine >= testataGrigliaElem.Validita_Fine) OrElse
                               (testataGrigliaToCompare.Validita_Inizio.HasValue AndAlso
                               testataGrigliaToCompare.Validita_Fine.HasValue AndAlso
                               testataGrigliaElem.Validita_Inizio <= testataGrigliaToCompare.Validita_Inizio AndAlso
                               testataGrigliaElem.Validita_Fine >= testataGrigliaToCompare.Validita_Fine)) Then
                                    trovatoErrore = True
                                End If
                            End If
                        Next
                        For x As Integer = 0 To EFArrayToInsert.Count - 1
                            testataGrigliaToCompare = EFArrayToInsert(x)
                            If testataGrigliaElem.des_TestataGriglia = testataGrigliaToCompare.des_TestataGriglia AndAlso
                               ((testataGrigliaToCompare.Validita_Inizio.HasValue AndAlso
                               testataGrigliaElem.Validita_Inizio <= testataGrigliaToCompare.Validita_Inizio AndAlso
                               testataGrigliaElem.Validita_Fine >= testataGrigliaToCompare.Validita_Inizio) OrElse
                               (testataGrigliaToCompare.Validita_Fine.HasValue AndAlso
                               testataGrigliaElem.Validita_Inizio <= testataGrigliaToCompare.Validita_Fine AndAlso
                               testataGrigliaElem.Validita_Fine >= testataGrigliaToCompare.Validita_Fine) OrElse
                               (testataGrigliaToCompare.Validita_Inizio.HasValue AndAlso
                               testataGrigliaToCompare.Validita_Fine.HasValue AndAlso
                               testataGrigliaToCompare.Validita_Inizio <= testataGrigliaElem.Validita_Inizio AndAlso
                               testataGrigliaToCompare.Validita_Fine >= testataGrigliaElem.Validita_Fine) OrElse
                               (testataGrigliaToCompare.Validita_Inizio.HasValue AndAlso
                               testataGrigliaToCompare.Validita_Fine.HasValue AndAlso
                               testataGrigliaElem.Validita_Inizio <= testataGrigliaToCompare.Validita_Inizio AndAlso
                               testataGrigliaElem.Validita_Fine >= testataGrigliaToCompare.Validita_Fine)) Then

                                trovatoErrore = True
                            End If
                        Next

                        If trovatoErrore Then
                            messaggioErrore &= "Esistono altre righe in periodi sovrapposti con la descrizione " & testataGrigliaElem.des_TestataGriglia & "<br/>"
                        End If
                    Next
                End If
            End If

            If String.IsNullOrEmpty(messaggioErrore) Then
                Dim campConf_W As New FF_CampionamentoConferimento_W

                messaggioErrore = campConf_W.Aggiorna_TestataGriglia_Campionamento(piva, EFArrayToInsert, EFArrayToUpdate,
                    EFArrayToDelete, objParametri)

            End If

        Catch ex As Exception
            messaggioErrore = "[" & nomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
        End Try

        If Not String.IsNullOrEmpty(messaggioErrore) Then
            Throw New Exception(messaggioErrore)
        End If

        Return messaggioErrore

    End Function
    
    Public Function CheckTestataGriglia(ByVal campConfTestataGriglia As CampionamentoConferito_TestataGriglia,
                                        ByRef objParametri As AgronicaCoreParametri,
                                        ByRef controllaSeGiaUtilizzato As Boolean,
                                        ByRef controllaSePeriodiSovrapposti As Boolean,
                                        ByRef controllaSeProdottiCollegati As Boolean,
                                        ByRef controllaSeListiniCollegati As Boolean
                                        ) As String

        Const nomeRoutine = "ContabBIZ.FF_CampionamentoConferimento.CheckTestataGriglia()"
        Dim messaggioErrore As String = String.Empty
        Dim campConf_R As New FF_CampionamentoConferimento_R

        Try

            'Controlli congruenza sulle date da <= a
            If campConfTestataGriglia.Validita_Fine.HasValue AndAlso
                campConfTestataGriglia.Validita_Inizio.HasValue AndAlso
                campConfTestataGriglia.Validita_Inizio > campConfTestataGriglia.Validita_Fine Then

                messaggioErrore &= campConfTestataGriglia.des_TestataGriglia & ": Valido Da deve essere minore di Valido fino a <br/>"

            End If

            ' Controllo se la griglia è già stata utilizzata
            If controllaSeGiaUtilizzato AndAlso String.IsNullOrEmpty(messaggioErrore) Then
                Dim nrMov As Integer = campConf_R.Controllo_Esistenza_CampionamentoConferito_Movimenti(campConfTestataGriglia.PIVA,
                       0, campConfTestataGriglia.Id_TestataGriglia, 0, objParametri)
                If nrMov > 0 Then
                    messaggioErrore &= "La griglia " & campConfTestataGriglia.des_TestataGriglia & " è già stata utilizzata in " & nrMov.ToString() & " movimenti, non è possibile modificarla <br/>"
                    ' 6/9/2017  controllo calibri / prodotti già presenti asteriscato perché non necessario
                    'Else
                    '    Dim calibriTrovati As Integer = 0
                    '    Dim prodottiTrovati As Integer = 0
                    '    Dim errFound As Boolean = campConf_R.Controllo_Esistenza_Prodotti_Calibri_Per_TestataGriglia(campConfTestataGriglia.PIVA,
                    '                                                                                  campConfTestataGriglia.Id_TestataGriglia, calibriTrovati, prodottiTrovati,
                    '                                                                                   objParametri)
                    '    If errFound Then
                    '        If calibriTrovati > 0 Then
                    '            If calibriTrovati = 1 Then
                    '                MessaggioErrore += "Modifica non possibile: esiste un calibro legato alla griglia " + campConfTestataGriglia.des_TestataGriglia + " <br/>"
                    '            Else
                    '                MessaggioErrore += "Modifica non possibile: sono stati trovati " + calibriTrovati.ToString() + " calibri legati alla griglia " + campConfTestataGriglia.des_TestataGriglia + " <br/>"
                    '            End If
                    '        End If
                    '        If prodottiTrovati > 0 Then
                    '            If prodottiTrovati = 1 Then
                    '                MessaggioErrore += "Modifica non possibile: esiste un prodotto legato alla griglia " + campConfTestataGriglia.des_TestataGriglia + " <br/>"
                    '            Else
                    '                MessaggioErrore += "Modifica non possibile: sono stati trovati " + prodottiTrovati.ToString() + " prodotti legati alla griglia " + campConfTestataGriglia.des_TestataGriglia + " <br/>"
                    '            End If
                    '        End If
                    '    End If
                End If
            End If

            'Controllo che non vi siano altre testate con periodi sovrapposti
            If controllaSePeriodiSovrapposti AndAlso String.IsNullOrEmpty(messaggioErrore) Then
                Dim myQueryCount As Integer =
                    campConf_R.Controlla_Date_Testata_GriglieCampionamento_Generica(campConfTestataGriglia, objParametri)
                If myQueryCount > 0 Then
                    messaggioErrore &= "Esistono altre righe con la stessa descrizione " & campConfTestataGriglia.des_TestataGriglia & " e periodi sovrapposti"
                End If
            End If

            'Controllo se la testata è collegata a dei prodotti
            If controllaSeProdottiCollegati Then
                'AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_TestataGriglia_Prodotti()
                Dim listProdotti = campConf_R.Leggi_TestataGriglia_Prodotti(campConfTestataGriglia.PIVA, campConfTestataGriglia.Id_TestataGriglia, "", objParametri)
                If listProdotti.Count > 0 Then
                    messaggioErrore += "<br/>Esistono prodotti collegati alla griglia di campionamento"
                End If
            End If

            'Controllo se la testata è collegata a dei listini
            If controllaSeListiniCollegati Then
                'Listini_CampionamentoConferito_X_Testata_Griglia_R.Leggi()
                Dim handleCampConfTestataGriglia As New Listini_CampionamentoConferito_X_Testata_Griglia_R()
                Dim elenListini = handleCampConfTestataGriglia.LeggiListiniAssociatiCampionamento(
                    campConfTestataGriglia.PIVA, 
                    campConfTestataGriglia.Id_TestataGriglia, 
                    objParametri)
                If elenListini.Count > 0 Then
                    messaggioErrore += "<br/>Esistono listini collegati alla griglia di campionamento"
                End If
            End If

        Catch ex As Exception
            messaggioErrore = "[" & nomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
        End Try

        If Not String.IsNullOrEmpty(messaggioErrore) Then
        End If

        Return messaggioErrore

    End Function

    Public Function CheckTestataGriglia_Prodotti(ByVal campConfTestataGriglia_Prodotti As CampionamentoConferito_TestataGriglia_Prodotti,
                                                 ByVal mat_des As String,
                                                 ByVal qualita_des As String,
                                                 ByVal calibro_des As String,
                                                 ByRef objParametri As AgronicaCoreParametri
                                                 ) As String

        Dim nomeRoutine As String = "ContabBIZ.FF_CampionamentoConferimento.CheckTestataGriglia_Prodotti()"
        Dim messaggioErrore As String = String.Empty
        Dim campConf_R As New FF_CampionamentoConferimento_R

        Try

            Dim campConfTestataGriglia As CampionamentoConferito_TestataGriglia =
                campConf_R.Leggi_Elem_Testata_GriglieCampionamento(campConfTestataGriglia_Prodotti.PIVA, campConfTestataGriglia_Prodotti.Id_TestataGriglia, objParametri)

            ' Controllo se la griglia è già stata utilizzata
            Dim nrMov As Integer = campConf_R.Controllo_Esistenza_CampionamentoConferito_Movimenti(campConfTestataGriglia_Prodotti.PIVA,
                       0, 0, campConfTestataGriglia_Prodotti.Id_TestataGriglia_Prod, objParametri)
            If nrMov > 0 Then
                messaggioErrore &= "La griglia " & campConfTestataGriglia.des_TestataGriglia & " per il prodotto " & mat_des & " " & qualita_des & " " & calibro_des & " è già stata utilizzata in " & nrMov.ToString() & " movimenti, non è possibile modificarla o cancellarla <br/>"
            Else

                'Controllo se ci sono listini collegati
                Dim elencoListini As New List(Of Listini_CampionamentoConferito_Prodotti)
                Dim elencoListini_equiv As New List(Of Listini_CampionamentoConferito_Prodotti_Equivalenti)
                elencoListini = campConf_R.Leggi_Elenco_Listini_Prodotti(campConfTestataGriglia_Prodotti.PIVA, Nothing,
                    campConfTestataGriglia_Prodotti.Id_TestataGriglia_Prod, False, objParametri)
                If elencoListini.Count = 0 Then
                    elencoListini = campConf_R.Leggi_Elenco_Listini_Prodotti(campConfTestataGriglia_Prodotti.PIVA, Nothing,
                    campConfTestataGriglia_Prodotti.Id_TestataGriglia_Prod, True, objParametri)
                End If
                If elencoListini.Count = 0 Then
                    elencoListini_equiv = campConf_R.Leggi_Elenco_Listini_Prodotti_Equivalenti(campConfTestataGriglia_Prodotti.PIVA, Nothing,
                    campConfTestataGriglia_Prodotti.Id_TestataGriglia_Prod, Nothing, objParametri)
                End If
                ' La prossima non è necessaria perché questo caso l'ho già intercettato con le letture sopra
                ' Lasciata solo in caso in un domani cambi la logica
                'If elencoListini.Count > 0 OrElse elencoListini_equiv.Count > 0 Then
                '    elencoListini_equiv = campConf_R.Leggi_Elenco_Listini_Prodotti_Equivalenti(campConfTestataGriglia_Prodotti.PIVA, Nothing,
                '    Nothing, campConfTestataGriglia_Prodotti.Id_TestataGriglia_Prod, objParametri)
                'End If
                If elencoListini.Count > 0 OrElse elencoListini_equiv.Count > 0 Then
                    messaggioErrore &= "La griglia " & campConfTestataGriglia.des_TestataGriglia & " per il prodotto " & mat_des & " " & qualita_des & " " & calibro_des & " è già stata utilizzata nei listini prezzi, non è possibile modificarla o cancellarla <br/>"
                End If

            End If


        Catch ex As Exception
            messaggioErrore = "[" & nomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
        End Try

        Return messaggioErrore

    End Function

    Public Function Aggiorna_TestataGriglia_Prodotti(ByVal piva As String,
                                                     ByVal Id_TestataGriglia As Integer,
                                                     ByVal righeInserite As String,
                                                     ByVal righeModificate As String,
                                                     ByVal righeCancellate As String,
                                                     ByRef objParametri As AgronicaCoreParametri
                                                     ) As String

        Const nomeRoutine = "ContabBIZ.FF_CampionamentoConferimento.Aggiorna_TestataGriglia_Prodotti()"
        Dim messaggioErrore As String = String.Empty
        Dim campConf_R As New FF_CampionamentoConferimento_R

        Try

            Dim campConfTestataGriglia_Prodotti As CampionamentoConferito_TestataGriglia_Prodotti
            Dim righeInseriteArray As JArray = JArray.Parse(righeInserite)
            Dim righeModificateArray As JArray = JArray.Parse(righeModificate)
            Dim righeCancellateArray As JArray = JArray.Parse(righeCancellate)
            Dim EFArrayToInsert As New ArrayList
            Dim EFArrayToUpdate As New ArrayList
            Dim EFArrayToDelete As New ArrayList
            For Each obj As JObject In righeInseriteArray
                campConfTestataGriglia_Prodotti = New CampionamentoConferito_TestataGriglia_Prodotti
                campConfTestataGriglia_Prodotti.Piva_SuperUser = objParametri.PivaSuperUser
                campConfTestataGriglia_Prodotti.PIVA = piva
                campConfTestataGriglia_Prodotti.Id_TestataGriglia = Id_TestataGriglia
                campConfTestataGriglia_Prodotti.Mat_Cod = obj("Mat_Cod")
                campConfTestataGriglia_Prodotti.Tabella_Par_Cod_Qualita = obj("qualita_cod")
                campConfTestataGriglia_Prodotti.Tabella_Par_Cod_Calibro = obj("calibro_cod")
                campConfTestataGriglia_Prodotti.Validita_Inizio = ValiditaInizio
                campConfTestataGriglia_Prodotti.Validita_Fine = ValiditaFine
                campConfTestataGriglia_Prodotti.Ordinamento = obj("Ordinamento")
                campConfTestataGriglia_Prodotti.Data_Creazione = Date.Now
                campConfTestataGriglia_Prodotti.Username_Creazione = objParametri.UsernameOperazione
                campConfTestataGriglia_Prodotti.Data_Modifica = Date.Now
                campConfTestataGriglia_Prodotti.Username_Modifica = objParametri.UsernameOperazione
                campConfTestataGriglia_Prodotti.inviato = 0
                EFArrayToInsert.Add(campConfTestataGriglia_Prodotti)
            Next
            For Each obj As JObject In righeModificateArray
                campConfTestataGriglia_Prodotti = campConf_R.Leggi_Elem_Testata_Griglie_Prodotti(piva, Id_TestataGriglia, obj("Id_TestataGriglia_Prod"), objParametri)
                If campConfTestataGriglia_Prodotti Is Nothing Then
                    messaggioErrore += "Riga da aggiornare " & obj("mat_des").ToString & " " & obj("qualita_des").ToString & " " & obj("calibro_des").ToString & " non trovata"
                Else
                    ' Non faccio il controllo se è cambiato solo l'ordinamento
                    Dim controllaSeUtilizzato = False
                    If campConfTestataGriglia_Prodotti.Mat_Cod <> CInt(obj("Mat_Cod")) OrElse
                        campConfTestataGriglia_Prodotti.Tabella_Par_Cod_Qualita <> CInt(obj("qualita_cod")) OrElse
                        campConfTestataGriglia_Prodotti.Tabella_Par_Cod_Calibro <> CInt(obj("calibro_cod")) Then
                        controllaSeUtilizzato = True
                    End If

                    campConfTestataGriglia_Prodotti.Mat_Cod = obj("Mat_Cod")
                    campConfTestataGriglia_Prodotti.Tabella_Par_Cod_Qualita = obj("qualita_cod")
                    campConfTestataGriglia_Prodotti.Tabella_Par_Cod_Calibro = obj("calibro_cod")
                    campConfTestataGriglia_Prodotti.Ordinamento = obj("Ordinamento")
                    campConfTestataGriglia_Prodotti.Data_Modifica = Date.Now
                    campConfTestataGriglia_Prodotti.Username_Modifica = objParametri.UsernameOperazione
                    ' Controllo che non si possano fare modifiche se ci sono movimenti collegati
                    If controllaSeUtilizzato Then
                        messaggioErrore += CheckTestataGriglia_Prodotti(campConfTestataGriglia_Prodotti, obj("Mat_Des").ToString(), obj("qualita_des").ToString(), obj("calibro_des").ToString(), objParametri)
                    End If
                    EFArrayToUpdate.Add(campConfTestataGriglia_Prodotti)
                End If
            Next
            For Each obj As JObject In righeCancellateArray
                campConfTestataGriglia_Prodotti = New CampionamentoConferito_TestataGriglia_Prodotti With {
                    .Piva_SuperUser = objParametri.PivaSuperUser,
                    .PIVA = piva,
                    .Id_TestataGriglia = Id_TestataGriglia,
                    .Id_TestataGriglia_Prod = obj("Id_TestataGriglia_Prod")
                }
                ' Controllo che non si possa cancellare se ci sono movimenti collegati
                messaggioErrore += CheckTestataGriglia_Prodotti(campConfTestataGriglia_Prodotti, obj("Mat_Des").ToString(), obj("qualita_des").ToString(), obj("calibro_des").ToString(), objParametri)
                EFArrayToDelete.Add(campConfTestataGriglia_Prodotti)
            Next

            If String.IsNullOrEmpty(messaggioErrore) Then
                Dim campConf_W As New FF_CampionamentoConferimento_W
                messaggioErrore = campConf_W.Aggiorna_TestataGriglia_Prodotti(piva, EFArrayToInsert, EFArrayToUpdate,
                    EFArrayToDelete, objParametri)
            End If

        Catch ex As Exception
            messaggioErrore = "[" & nomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
        End Try

        If Not String.IsNullOrEmpty(messaggioErrore) Then
            Throw New Exception(messaggioErrore)
        End If

        Return messaggioErrore

    End Function

    Public Function Aggiorna_TestataGriglia_Calibri(
            ByVal piva As String,
            ByVal Id_TestataGriglia As Integer,
            ByVal righeInserite As String,
            ByVal righeModificate As String,
            ByVal righeCancellate As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As String

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim MessaggioErrore As String = String.Empty

        Dim NomeRoutine As String = "ContabBIZ.FF_CampionamentoConferimento.Aggiorna_TestataGriglia_Calibri()"

        Try
            Dim campConf_R As New FF_CampionamentoConferimento_R

            Dim campConfTestataGriglia_Calibri As New CampionamentoConferito_TestataGriglia_Calibri
            Dim righeInseriteArray As JArray = JArray.Parse(righeInserite)
            Dim righeModificateArray As JArray = JArray.Parse(righeModificate)
            Dim righeCancellateArray As JArray = JArray.Parse(righeCancellate)
            Dim EFArrayToInsert As New ArrayList
            Dim EFArrayToUpdate As New ArrayList
            Dim EFArrayToDelete As New ArrayList
            For Each obj As JObject In righeInseriteArray
                campConfTestataGriglia_Calibri = New CampionamentoConferito_TestataGriglia_Calibri
                campConfTestataGriglia_Calibri.Piva_SuperUser = Piva_SuperUser
                campConfTestataGriglia_Calibri.PIVA = piva
                campConfTestataGriglia_Calibri.Id_TestataGriglia = Id_TestataGriglia
                campConfTestataGriglia_Calibri.Descr_qualita = obj("Descr_qualita")
                campConfTestataGriglia_Calibri.Descr_calibro = obj("Descr_calibro")
                campConfTestataGriglia_Calibri.Peso = obj("Peso")
                campConfTestataGriglia_Calibri.Ordinamento = obj("Ordinamento")
                campConfTestataGriglia_Calibri.Data_Creazione = Date.Now
                campConfTestataGriglia_Calibri.Username_Creazione = objParametri.UsernameOperazione
                campConfTestataGriglia_Calibri.Data_Modifica = Date.Now
                campConfTestataGriglia_Calibri.Username_Modifica = objParametri.UsernameOperazione
                campConfTestataGriglia_Calibri.inviato = 0
                campConfTestataGriglia_Calibri.Validita_Inizio = ValiditaInizio
                campConfTestataGriglia_Calibri.Validita_Fine = ValiditaFine
                EFArrayToInsert.Add(campConfTestataGriglia_Calibri)
            Next
            For Each obj As JObject In righeModificateArray
                campConfTestataGriglia_Calibri = campConf_R.Leggi_Elem_Testata_GriglieCalibri(piva, Id_TestataGriglia, obj("Id_Calibro"), objParametri)
                If campConfTestataGriglia_Calibri Is Nothing Then
                    MessaggioErrore += "Riga da aggiornare con calibro " & obj("Id_Calibro").ToString & " non trovata"
                Else
                    ' Non faccio il controllo se sono cambiati solo l'ordinamento o il peso
                    Dim controllaSeUtilizzato = False
                    If campConfTestataGriglia_Calibri.Descr_qualita <> CStr(obj("Descr_qualita")) OrElse
                        campConfTestataGriglia_Calibri.Descr_calibro <> CStr(obj("Descr_calibro")) Then
                        controllaSeUtilizzato = True
                    End If
                    campConfTestataGriglia_Calibri.Descr_qualita = obj("Descr_qualita")
                    campConfTestataGriglia_Calibri.Descr_calibro = obj("Descr_calibro")
                    campConfTestataGriglia_Calibri.Peso = obj("Peso")
                    campConfTestataGriglia_Calibri.Ordinamento = obj("Ordinamento")
                    campConfTestataGriglia_Calibri.Data_Modifica = Date.Now
                    campConfTestataGriglia_Calibri.Username_Modifica = objParametri.UsernameOperazione

                    Dim nrMov As Integer = 0
                    If controllaSeUtilizzato Then
                        ' Controllo che non si possono fare modifiche se ci sono movimenti collegati
                        nrMov = campConf_R.Controllo_Esistenza_CampionamentoConferito_Movimenti_Righe(campConfTestataGriglia_Calibri.PIVA,
                                                                                                   0,
                                                                                                   campConfTestataGriglia_Calibri.Id_TestataGriglia,
                                                                                                   0,
                                                                                                   campConfTestataGriglia_Calibri.Id_Calibro,
                                                                                                   objParametri)
                    End If

                    If nrMov > 0 Then
                        MessaggioErrore &= "Il calibro " & campConfTestataGriglia_Calibri.Descr_qualita & " " & campConfTestataGriglia_Calibri.Descr_calibro & " " &
                        "è già stato utilizzato in " & nrMov.ToString() & " movimenti, non è possibile modificarlo"
                    Else
                        EFArrayToUpdate.Add(campConfTestataGriglia_Calibri)
                    End If
                End If
            Next
            For Each obj As JObject In righeCancellateArray
                campConfTestataGriglia_Calibri = New CampionamentoConferito_TestataGriglia_Calibri
                campConfTestataGriglia_Calibri.Piva_SuperUser = Piva_SuperUser
                campConfTestataGriglia_Calibri.PIVA = piva
                campConfTestataGriglia_Calibri.Id_TestataGriglia = Id_TestataGriglia
                campConfTestataGriglia_Calibri.Id_Calibro = obj("Id_Calibro")
                Dim nrMov As Integer = campConf_R.Controllo_Esistenza_CampionamentoConferito_Movimenti_Righe(campConfTestataGriglia_Calibri.PIVA,
                                                                                                   0,
                                                                                                   campConfTestataGriglia_Calibri.Id_TestataGriglia,
                                                                                                             0,
                                                                                                   campConfTestataGriglia_Calibri.Id_Calibro,
                                                                                                   objParametri)
                If nrMov > 0 Then
                    MessaggioErrore &= "Il calibro " & campConfTestataGriglia_Calibri.Descr_qualita & " " & campConfTestataGriglia_Calibri.Descr_calibro & " " &
                    "è già stato utilizzato in " & nrMov.ToString() & " movimenti, non è possibile cancellarlo <br/>"
                End If

                Dim nrList As Integer = campConf_R.Controllo_Esistenza_Listini_Calibri(campConfTestataGriglia_Calibri.PIVA,
                                                                                                   campConfTestataGriglia_Calibri.Id_TestataGriglia,
                                                                                                   campConfTestataGriglia_Calibri.Id_Calibro,
                                                                                                   objParametri)
                If nrList > 0 Then
                    MessaggioErrore &= "Il calibro " & campConfTestataGriglia_Calibri.Descr_qualita & " " & campConfTestataGriglia_Calibri.Descr_calibro & " " &
                    "è già stato utilizzato in " & nrList.ToString() & " listini, non è possibile cancellarlo <br/>"
                End If
                If String.IsNullOrEmpty(MessaggioErrore) Then
                    EFArrayToDelete.Add(campConfTestataGriglia_Calibri)
                End If
            Next

            If String.IsNullOrEmpty(MessaggioErrore) Then
                Dim campConf_W As New FF_CampionamentoConferimento_W

                MessaggioErrore = campConf_W.Aggiorna_TestataGriglia_Calibri(piva, Id_TestataGriglia, EFArrayToInsert, EFArrayToUpdate,
                    EFArrayToDelete, objParametri)

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

    Public Function Aggiorna_Campioni_RigaConferimento(
            ByVal piva As String,
            ByVal Id_Mov_Det As Integer,
            ByVal Id_TestataGriglia_Prod As Integer,
            ByVal StatoCampionamento As String,
            ByVal metodoCampionamento As Short,
            ByVal Note As String,
            ByVal KgCampionati As Double,
            ByVal stringObjectsToSave As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As String

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim MessaggioErrore As String = String.Empty

        Dim NomeRoutine As String = "ContabBIZ.FF_CampionamentoConferimento.Aggiorna_Campioni_RigaConferimento()"

        Try
            Dim objToSaveArray As JArray = JArray.Parse(stringObjectsToSave)

            Dim campConf_Movim_Testata As New CampionamentoConferito_Movimenti
            campConf_Movim_Testata.Piva_SuperUser = Piva_SuperUser
            campConf_Movim_Testata.PIVA = piva
            campConf_Movim_Testata.Id_Mov_Det = Id_Mov_Det
            campConf_Movim_Testata.Id_TestataGriglia_Prod = Id_TestataGriglia_Prod
            campConf_Movim_Testata.Udm_QtaCampionata = 2
            campConf_Movim_Testata.QtaCampionata = KgCampionati
            campConf_Movim_Testata.StatoCampionamento = StatoCampionamento
            campConf_Movim_Testata.Automatico = metodoCampionamento
            campConf_Movim_Testata.Note = Note
            campConf_Movim_Testata.Data_Creazione = Date.Now
            campConf_Movim_Testata.Username_Creazione = objParametri.UsernameOperazione
            campConf_Movim_Testata.Data_Modifica = Date.Now
            campConf_Movim_Testata.Username_Modifica = objParametri.UsernameOperazione
            campConf_Movim_Testata.inviato = 0
            campConf_Movim_Testata.Validita_Inizio = ValiditaInizio
            campConf_Movim_Testata.Validita_Fine = ValiditaFine
            campConf_Movim_Testata.Dt_Inizio_Campionamento = Date.Now
            campConf_Movim_Testata.Dt_Fine_Campionamento = Date.Now

            Dim campConf_Mov_Righe As New CampionamentoConferito_Movimenti_Righe
            Dim EFArrayToInsertUpdate As New ArrayList
            Dim EFArrayToDelete As New ArrayList
            For Each obj As JObject In objToSaveArray

                If obj("PercentualeCampionato").ToString() = "" Then
                    obj("PercentualeCampionato") = 0
                End If
                campConf_Mov_Righe = New CampionamentoConferito_Movimenti_Righe
                campConf_Mov_Righe.Piva_SuperUser = Piva_SuperUser
                campConf_Mov_Righe.PIVA = piva
                campConf_Mov_Righe.Id_TestataGriglia_Prod = Id_TestataGriglia_Prod
                campConf_Mov_Righe.Id_Mov_Det = Id_Mov_Det
                campConf_Mov_Righe.Id_Calibro = obj("Id_Calibro")
                campConf_Mov_Righe.PercentualeCampionato = obj("PercentualeCampionato")
                campConf_Mov_Righe.Data_Creazione = Date.Now
                campConf_Mov_Righe.Username_Creazione = objParametri.UsernameOperazione
                campConf_Mov_Righe.Data_Modifica = Date.Now
                campConf_Mov_Righe.Username_Modifica = objParametri.UsernameOperazione
                campConf_Mov_Righe.inviato = 0
                campConf_Mov_Righe.Validita_Inizio = ValiditaInizio
                campConf_Mov_Righe.Validita_Fine = ValiditaFine
                If Convert.ToDouble(obj("PercentualeCampionato")) > 0 Then
                    EFArrayToInsertUpdate.Add(campConf_Mov_Righe)
                Else
                    EFArrayToDelete.Add(campConf_Mov_Righe)
                End If

            Next

            If String.IsNullOrEmpty(MessaggioErrore) Then
                Dim campConf_W As New FF_CampionamentoConferimento_W

                MessaggioErrore = campConf_W.Aggiorna_Campioni_RigaConferimento(campConf_Movim_Testata, EFArrayToInsertUpdate,
                    EFArrayToDelete, objParametri)

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

    Public Function CancellaCampionamentiRigaConferimento(
            ByVal piva As String,
            ByVal Id_Mov_Det As Integer,
            ByVal StatoCampionamentoFinale As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As String

        Dim MessaggioErrore As String = String.Empty
        Dim campConf_R As New FF_CampionamentoConferimento_R

        Dim NomeRoutine As String = "ContabBIZ.FF_CampionamentoConferimento.CancellaCampionamentiRigaConferimento()"

        Try
            Dim TestataElem As CampionamentoConferito_Movimenti = campConf_R.LeggiElem_CampionamentoRigaConferito(piva, Id_Mov_Det,
                    objParametri)
            If TestataElem Is Nothing Then
                MessaggioErrore += "Non sono stati trovati movimenti di campionamento da cancellare"
            Else
                If TestataElem.StatoCampionamento.Equals(StatoCampionamentoFinale) Then
                    MessaggioErrore += "Non è possibile eseguire la cancellazione in quanto il campionamento è già definitivo"
                End If
            End If

            ' Aggiornamento effettivo
            If String.IsNullOrEmpty(MessaggioErrore) Then
                Dim camConf_W As New FF_CampionamentoConferimento_W
                MessaggioErrore = camConf_W.Cancella_CampionamentoRigaConferito(TestataElem,
                    objParametri)
            End If

        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
        Finally

        End Try

        Return MessaggioErrore
    End Function

    Public Function Aggiorna_Listini_CampionamentoConferito_Dettagli(ByVal piva As String,
                                                                     ByVal KeyListinoProdottoCalibro As String,
                                                                     ByVal validoDal As String,
                                                                     ByVal validoAl As String,
                                                                     ByVal validoDalOrigine As String,
                                                                     ByVal validoAlOrigine As String,
                                                                     ByVal stringObjectsToSave As String,
                                                                     ByRef objParametri As AgronicaCoreParametri
                                                                     ) As String

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Const nomeRoutine = "ContabBIZ.FF_CampionamentoConferimento.Aggiorna_Listini_CampionamentoConferito_Dettagli()"
        Dim messaggioErrore As String = String.Empty

        Dim keys As String() = KeyListinoProdottoCalibro.Split("-")
        Dim key_piva As String = keys(0)
        Dim key_Listino_Cod As Integer = Integer.Parse(keys(1))
        Dim key_Id_TestataGriglia_Prod As Integer = Integer.Parse(keys(2))

        Dim validoDalDateTime As Nullable(Of DateTime)
        validoDalDateTime = Nothing
        If Not String.IsNullOrEmpty(validoDal) Then
            validoDalDateTime = Convert.ToDateTime(validoDal)
        End If

        Dim validoAlDateTime As Nullable(Of DateTime)
        validoAlDateTime = Nothing
        If Not String.IsNullOrEmpty(validoAl) Then
            validoAlDateTime = Convert.ToDateTime(validoAl)
        End If

        Dim validoDalDateTimeOrigine As DateTime = "01/01/1900"
        If Not String.IsNullOrEmpty(validoDalOrigine) Then
            validoDalDateTimeOrigine = Convert.ToDateTime(validoDalOrigine)
        End If

        Dim validoAlDateTimeOrigine As DateTime = "01/01/1900"
        If Not String.IsNullOrEmpty(validoAlOrigine) Then
            validoAlDateTimeOrigine = Convert.ToDateTime(validoAlOrigine)
        End If

        Try
            Dim campConf_R As New FF_CampionamentoConferimento_R

            Dim campConfTestataGriglia_Prodotti = campConf_R.Leggi_Elem_Testata_Griglie_Prodotti(piva, 0, key_Id_TestataGriglia_Prod, objParametri)

            Dim objToSaveArray As JArray = JArray.Parse(stringObjectsToSave)

            Dim listini_campConf_prodotti As New Listini_CampionamentoConferito_Prodotti
            listini_campConf_prodotti.Piva_SuperUser = Piva_SuperUser
            listini_campConf_prodotti.PIVA = piva
            listini_campConf_prodotti.Listino_Cod = key_Listino_Cod
            listini_campConf_prodotti.Id_TestataGriglia_Prod = key_Id_TestataGriglia_Prod
            listini_campConf_prodotti.prezzo_su_conferito = 0
            listini_campConf_prodotti.prezzo = 0.0
            listini_campConf_prodotti.Data_Creazione = Date.Now
            listini_campConf_prodotti.Username_Creazione = objParametri.UsernameOperazione
            listini_campConf_prodotti.Data_Modifica = Date.Now
            listini_campConf_prodotti.Username_Modifica = objParametri.UsernameOperazione
            listini_campConf_prodotti.inviato = 0
            listini_campConf_prodotti.Validita_Inizio = validoDalDateTime
            listini_campConf_prodotti.Validita_Fine = validoAlDateTime

            ' Controllo che non ci siano periodi sovrapposti già registrati
            ' Se le date di origine sono valorizzate significa che siamo in modifica
            ' Ma se le nuove date digitate sono entrambe  minori o entrambe maggiori di quelle precedenti viene considerato come se fosse un nuovo inserimento
            If Not String.IsNullOrEmpty(validoDalOrigine) AndAlso Not String.IsNullOrEmpty(validoAlOrigine) Then
                If (validoDalDateTime <validoDalDateTimeOrigine AndAlso validoAlDateTime < validoDalDateTimeOrigine) OrElse
                   (validoDalDateTime > validoAlDateTimeOrigine AndAlso validoAlDateTime > validoAlDateTimeOrigine) Then
                    messaggioErrore += CheckListiniProdotti(listini_campConf_prodotti, objParametri, True, False, True)
                Else
                    messaggioErrore += CheckListiniProdotti(listini_campConf_prodotti, objParametri, False, False, True)
                End If

            Else
                messaggioErrore += CheckListiniProdotti(listini_campConf_prodotti, objParametri, True, False, True)
            End If

            Dim listini_campConf_dettagli As New Listini_CampionamentoConferito_Dettagli
            Dim EFArrayToInsertUpdate As New ArrayList
            Dim EFArrayToDelete As New ArrayList
            Dim EFArrayRigheProdottiToDelete As New ArrayList

            For Each obj As JObject In objToSaveArray

                If obj("Prezzo").ToString() = "" Then
                    obj("Prezzo") = 0
                End If

                listini_campConf_dettagli = New Listini_CampionamentoConferito_Dettagli
                listini_campConf_dettagli.Piva_SuperUser = Piva_SuperUser
                listini_campConf_dettagli.PIVA = piva
                listini_campConf_dettagli.Listino_Cod = key_Listino_Cod
                listini_campConf_dettagli.Id_TestataGriglia = campConfTestataGriglia_Prodotti.Id_TestataGriglia
                listini_campConf_dettagli.Id_TestataGriglia_Prod = key_Id_TestataGriglia_Prod
                listini_campConf_dettagli.Id_Calibro = obj("Id_Calibro")
                listini_campConf_dettagli.Prezzo = obj("Prezzo")
                listini_campConf_dettagli.Data_Creazione = Date.Now
                listini_campConf_dettagli.Username_Creazione = objParametri.UsernameOperazione
                listini_campConf_dettagli.Data_Modifica = Date.Now
                listini_campConf_dettagli.Username_Modifica = objParametri.UsernameOperazione
                listini_campConf_dettagli.inviato = 0
                listini_campConf_dettagli.Validita_Inizio = validoDalDateTime
                listini_campConf_dettagli.Validita_Fine = validoAlDateTime

                If Convert.ToDecimal(obj("Prezzo")) <> 0 Then
                    EFArrayToInsertUpdate.Add(listini_campConf_dettagli)
                Else
                    EFArrayToDelete.Add(listini_campConf_dettagli)
                End If

            Next

            If String.IsNullOrEmpty(messaggioErrore) Then

                'Cancello tutte le eventuali righe prodotti e dettaglio esistenti con prezzo su campionato / conferito
                Dim RigheProdotti = campConf_R.Leggi_Elenco_Listini_Prodotti(piva, key_Listino_Cod,
                      key_Id_TestataGriglia_Prod, True, objParametri)

                If RigheProdotti IsNot Nothing Then
                    For Each riga In RigheProdotti
                        ' Contiene anche i dettagli
                        EFArrayRigheProdottiToDelete.Add(riga)
                    Next
                End If

                Dim campConf_W As New FF_CampionamentoConferimento_W

                messaggioErrore = campConf_W.Aggiorna_Listini_CampionamentoConferito_Dettagli(listini_campConf_prodotti, validoDalDateTimeOrigine,
                                                                                              validoAlDateTimeOrigine, EFArrayToInsertUpdate,
                    EFArrayToDelete, EFArrayRigheProdottiToDelete, objParametri)

            End If


        Catch ex As Exception
            messaggioErrore = "[" & nomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
        End Try

        If Not String.IsNullOrEmpty(messaggioErrore) Then
            Throw New Exception(messaggioErrore)
        End If

        Return messaggioErrore

    End Function

    Public Function Aggiorna_Listini_PrezzoSuConferito(ByVal piva As String,
                                                       ByVal KeyListinoProdottoCalibro As String,
                                                       ByVal righeInserite As String,
                                                       ByVal righeModificate As String,
                                                       ByVal righeCancellate As String,
                                                       ByRef objParametri As AgronicaCoreParametri
                                                       ) As String

        Const nomeRoutine = "ContabBIZ.FF_CampionamentoConferimento.Aggiorna_Listini_PrezzoSuConferito()"
        Dim messaggioErrore As String = String.Empty

        Dim keys As String() = KeyListinoProdottoCalibro.Split("-")
        Dim key_piva As String = keys(0)
        Dim key_Listino_Cod As Integer = Integer.Parse(keys(1))
        Dim key_Id_TestataGriglia_Prod As Integer = Integer.Parse(keys(2))

        Try
            Dim campConf_R As New FF_CampionamentoConferimento_R

            Dim listini_campConf_prodotti As New Listini_CampionamentoConferito_Prodotti

            Dim righeInseriteArray As JArray = JArray.Parse(righeInserite)
            Dim righeModificateArray As JArray = JArray.Parse(righeModificate)
            Dim righeCancellateArray As JArray = JArray.Parse(righeCancellate)
            Dim EFArrayToInsert As New ArrayList
            Dim EFArrayToUpdate As New ArrayList
            Dim EFArrayToDelete As New ArrayList
            Dim EFArrayRigheProdottiToDelete As New ArrayList
            For Each obj As JObject In righeInseriteArray

                listini_campConf_prodotti = New Listini_CampionamentoConferito_Prodotti
                listini_campConf_prodotti.Piva_SuperUser = objParametri.PivaSuperUser
                listini_campConf_prodotti.PIVA = piva
                listini_campConf_prodotti.Listino_Cod = key_Listino_Cod
                listini_campConf_prodotti.Id_TestataGriglia_Prod = key_Id_TestataGriglia_Prod
                listini_campConf_prodotti.prezzo_su_conferito = 1
                listini_campConf_prodotti.prezzo = obj("Prezzo")
                If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
                    listini_campConf_prodotti.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio"), Format, Provider)
                End If
                If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
                    listini_campConf_prodotti.Validita_Fine = Date.ParseExact(obj("Validita_Fine"), Format, Provider)
                End If
                listini_campConf_prodotti.Data_Creazione = Date.Now
                listini_campConf_prodotti.Username_Creazione = objParametri.UsernameOperazione
                listini_campConf_prodotti.Data_Modifica = Date.Now
                listini_campConf_prodotti.Username_Modifica = objParametri.UsernameOperazione
                listini_campConf_prodotti.inviato = 0
                ' Controllo che non ci siano periodi sovrapposti già registrati
                'N.B:  tolto controllo sovrapposizione perchè ora che si può modifica la data di una riga di listino a volte dava 
                '      errore considerando il periodo vecchio che invece essendo stato modificato era corretto
                '      Viene sfruttato il fatto che tanto vengono passate indietro tutte le righe della griglia, quindi basta il controllo
                '      finale che viene fatto qui sotto "Controlla se ci sono periodi sovrapposti all'interno delle righe che si stanno gestendo"
                'messaggioErrore += CheckListiniProdotti(listini_campConf_prodotti, objParametri, True, False, True)
                messaggioErrore += CheckListiniProdotti(listini_campConf_prodotti, objParametri, True, False, False)
                EFArrayToInsert.Add(listini_campConf_prodotti)
            Next

            For Each obj As JObject In righeModificateArray

                listini_campConf_prodotti = New Listini_CampionamentoConferito_Prodotti
                listini_campConf_prodotti.Piva_SuperUser = objParametri.PivaSuperUser
                listini_campConf_prodotti.PIVA = piva
                listini_campConf_prodotti.Listino_Cod = key_Listino_Cod
                listini_campConf_prodotti.Id_TestataGriglia_Prod = key_Id_TestataGriglia_Prod
                listini_campConf_prodotti.prezzo_su_conferito = 1
                listini_campConf_prodotti.inviato = 0

                Dim Key_List_Prodotti As String = obj("Key_List_Prodotti")
                Dim sep As Integer = Key_List_Prodotti.IndexOf("-")
                Dim dtDalOriginale As Date = Date.ParseExact(Key_List_Prodotti.Substring(4, 4) +
                                               Key_List_Prodotti.Substring(2, 2) +
                                               Key_List_Prodotti.Substring(0, 2), Format, Provider)
                Dim dtAlOriginale As Date = Date.ParseExact(Key_List_Prodotti.Substring(13, 4) +
                                               Key_List_Prodotti.Substring(11, 2) +
                                               Key_List_Prodotti.Substring(9, 2), Format, Provider)

                listini_campConf_prodotti = campConf_R.Leggi_Listino_Prodotto_Elem(piva, KeyListinoProdottoCalibro, dtDalOriginale, dtAlOriginale, objParametri)

                If listini_campConf_prodotti Is Nothing Then
                    messaggioErrore += "Riga da aggiornare non trovata"
                Else
                    'EF non permette modifica di un campo chiave, quindi faccio cancellazione + inserimento

                    Dim dtDalNuova As Date = Date.ParseExact(obj("Validita_Inizio"), Format, Provider)
                    Dim dtAlNuova As Date = Date.ParseExact(obj("Validita_Fine"), Format, Provider)

                    If dtDalOriginale <> dtDalNuova OrElse
                        dtAlOriginale <> dtAlNuova Then

                        Dim ToDelete_listini_campConf_prodotti = New Listini_CampionamentoConferito_Prodotti
                        ToDelete_listini_campConf_prodotti.Piva_SuperUser = objParametri.PivaSuperUser
                        ToDelete_listini_campConf_prodotti.PIVA = piva
                        ToDelete_listini_campConf_prodotti.Listino_Cod = key_Listino_Cod
                        ToDelete_listini_campConf_prodotti.Id_TestataGriglia_Prod = key_Id_TestataGriglia_Prod
                        ToDelete_listini_campConf_prodotti.prezzo_su_conferito = 1
                        ToDelete_listini_campConf_prodotti.inviato = 0
                        ToDelete_listini_campConf_prodotti = campConf_R.Leggi_Listino_Prodotto_Elem(piva, KeyListinoProdottoCalibro, dtDalOriginale, dtAlOriginale, objParametri)
                        EFArrayToDelete.Add(ToDelete_listini_campConf_prodotti)
                    End If

                    listini_campConf_prodotti.prezzo = obj("Prezzo")
                    listini_campConf_prodotti.Data_Modifica = Date.Now
                    listini_campConf_prodotti.Username_Modifica = objParametri.UsernameOperazione

                    If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
                        listini_campConf_prodotti.Validita_Inizio = dtDalNuova
                    End If
                    If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
                        listini_campConf_prodotti.Validita_Fine = dtAlNuova
                    End If

                    ' Controllo che non si possano fare modifiche se ci sono movimenti collegati e che non ci siano periodi sovrapposti già registrati
                    'N.B:  tolto controllo sovrapposizione perchè ora che si può modifica la data di una riga di listino a volte dava 
                    '      errore considerando il periodo vecchio che invece essendo stato modificato era corretto
                    '      Viene sfruttato il fatto che tanto vengono passate indietro tutte le righe della griglia, quindi basta il controllo
                    '      finale che viene fatto qui sotto "Controlla se ci sono periodi sovrapposti all'interno delle righe che si stanno gestendo"
                    'messaggioErrore += CheckListiniProdotti(listini_campConf_prodotti, objParametri, False, True, True)
                    messaggioErrore += CheckListiniProdotti(listini_campConf_prodotti, objParametri, False, True, False)

                    If dtDalOriginale <> dtDalNuova OrElse
                        dtAlOriginale <> dtAlNuova Then
                        EFArrayToInsert.Add(listini_campConf_prodotti)
                    Else
                        EFArrayToUpdate.Add(listini_campConf_prodotti)
                    End If

                End If
            Next

            For Each obj As JObject In righeCancellateArray
                listini_campConf_prodotti = New Listini_CampionamentoConferito_Prodotti
                listini_campConf_prodotti.Piva_SuperUser = objParametri.PivaSuperUser
                listini_campConf_prodotti.PIVA = piva
                listini_campConf_prodotti.Listino_Cod = key_Listino_Cod
                listini_campConf_prodotti.Id_TestataGriglia_Prod = key_Id_TestataGriglia_Prod
                listini_campConf_prodotti.prezzo_su_conferito = 1

                Dim Key_List_Prodotti As String = obj("Key_List_Prodotti")
                Dim sep As Integer = Key_List_Prodotti.IndexOf("-")
                Dim dtDal As Date = Date.ParseExact(Key_List_Prodotti.Substring(4, 4) +
                                               Key_List_Prodotti.Substring(2, 2) +
                                               Key_List_Prodotti.Substring(0, 2), Format, Provider)
                Dim dtAl As Date = Date.ParseExact(Key_List_Prodotti.Substring(13, 4) +
                                               Key_List_Prodotti.Substring(11, 2) +
                                               Key_List_Prodotti.Substring(9, 2), Format, Provider)
                listini_campConf_prodotti.Validita_Inizio = dtDal
                listini_campConf_prodotti.Validita_Fine = dtAl

                ' Controllo che non si possano fare modifiche se ci sono movimenti collegati
                messaggioErrore += CheckListiniProdotti(listini_campConf_prodotti, objParametri, False, True, False)
                EFArrayToDelete.Add(listini_campConf_prodotti)
            Next

            ' Controlla se ci sono periodi sovrapposti all'interno delle righe che si stanno gestendo
            If String.IsNullOrEmpty(messaggioErrore) Then
                Dim listini_campConf_prodottiElem As Listini_CampionamentoConferito_Prodotti = Nothing
                Dim listini_campConf_prodottiToCompare As Listini_CampionamentoConferito_Prodotti = Nothing
                Dim trovatoErrore As Boolean = False

                ' Righe nuove
                For i As Integer = 0 To EFArrayToInsert.Count - 1
                    listini_campConf_prodottiElem = EFArrayToInsert(i)
                    For x As Integer = 0 To EFArrayToInsert.Count - 1
                        listini_campConf_prodottiToCompare = EFArrayToInsert(x)
                        If i <> x Then
                            If ((listini_campConf_prodottiElem.Validita_Inizio <= listini_campConf_prodottiToCompare.Validita_Inizio AndAlso
                               listini_campConf_prodottiElem.Validita_Fine >= listini_campConf_prodottiToCompare.Validita_Inizio) OrElse
                               (listini_campConf_prodottiElem.Validita_Inizio <= listini_campConf_prodottiToCompare.Validita_Fine AndAlso
                               listini_campConf_prodottiElem.Validita_Fine >= listini_campConf_prodottiToCompare.Validita_Fine)) Then
                                trovatoErrore = True
                            End If
                        End If
                    Next
                    For x As Integer = 0 To EFArrayToUpdate.Count - 1
                        listini_campConf_prodottiToCompare = EFArrayToUpdate(x)
                        If ((listini_campConf_prodottiElem.Validita_Inizio <= listini_campConf_prodottiToCompare.Validita_Inizio AndAlso
                               listini_campConf_prodottiElem.Validita_Fine >= listini_campConf_prodottiToCompare.Validita_Inizio) OrElse
                               (listini_campConf_prodottiElem.Validita_Inizio <= listini_campConf_prodottiToCompare.Validita_Fine AndAlso
                               listini_campConf_prodottiElem.Validita_Fine >= listini_campConf_prodottiToCompare.Validita_Fine)) Then
                            trovatoErrore = True
                        End If
                    Next


                Next

                If Not trovatoErrore Then
                    ' Righe modificate
                    For i As Integer = 0 To EFArrayToUpdate.Count - 1
                        listini_campConf_prodottiElem = EFArrayToUpdate(i)
                        For x As Integer = 0 To EFArrayToUpdate.Count - 1
                            listini_campConf_prodottiToCompare = EFArrayToUpdate(x)
                            If i <> x Then
                                If ((listini_campConf_prodottiElem.Validita_Inizio <= listini_campConf_prodottiToCompare.Validita_Inizio AndAlso
                               listini_campConf_prodottiElem.Validita_Fine >= listini_campConf_prodottiToCompare.Validita_Inizio) OrElse
                               (listini_campConf_prodottiElem.Validita_Inizio <= listini_campConf_prodottiToCompare.Validita_Fine AndAlso
                               listini_campConf_prodottiElem.Validita_Fine >= listini_campConf_prodottiToCompare.Validita_Fine)) Then
                                    trovatoErrore = True
                                End If
                            End If
                        Next
                        For x As Integer = 0 To EFArrayToInsert.Count - 1
                            listini_campConf_prodottiToCompare = EFArrayToInsert(x)
                            If ((listini_campConf_prodottiElem.Validita_Inizio <= listini_campConf_prodottiToCompare.Validita_Inizio AndAlso
                               listini_campConf_prodottiElem.Validita_Fine >= listini_campConf_prodottiToCompare.Validita_Inizio) OrElse
                               (listini_campConf_prodottiElem.Validita_Inizio <= listini_campConf_prodottiToCompare.Validita_Fine AndAlso
                               listini_campConf_prodottiElem.Validita_Fine >= listini_campConf_prodottiToCompare.Validita_Fine)) Then

                                trovatoErrore = True
                            End If
                        Next
                    Next
                End If

                If trovatoErrore Then
                    messaggioErrore &= "Esistono righe in periodi sovrapposti al periodo " & listini_campConf_prodottiElem.Validita_Inizio.ToShortDateString & " - " & listini_campConf_prodottiElem.Validita_Fine.ToShortDateString & "<br/>"
                End If

            End If

            If String.IsNullOrEmpty(messaggioErrore) Then

                'Cancello tutte le eventuali righe prodotti e dettaglio esistenti con prezzo su campionato
                Dim RigheProdotti = campConf_R.Leggi_Elenco_Listini_Prodotti(piva, key_Listino_Cod,
                      key_Id_TestataGriglia_Prod, False, objParametri)

                If RigheProdotti IsNot Nothing Then
                    For Each riga In RigheProdotti
                        ' Contiene anche i dettagli
                        EFArrayRigheProdottiToDelete.Add(riga)
                    Next
                End If

                Dim campConf_W As New FF_CampionamentoConferimento_W

                messaggioErrore = campConf_W.Aggiorna_Listini_PrezzoSuConferito(piva, EFArrayToInsert, EFArrayToUpdate,
                    EFArrayToDelete, EFArrayRigheProdottiToDelete, objParametri)

            End If

        Catch ex As Exception
            messaggioErrore = "[" & nomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
        End Try

        If Not String.IsNullOrEmpty(messaggioErrore) Then
            Throw New Exception(messaggioErrore)
        End If

        Return messaggioErrore

    End Function


    Public Function CheckListiniProdotti(
            ByVal listini_campConf_prodotti As Listini_CampionamentoConferito_Prodotti,
            ByRef objParametri As AgronicaCoreParametri,
            ByRef nuovoInserimento As Boolean,
            ByRef controllaSeGiaUtilizzato As Boolean,
            ByRef controllaSePeriodiSovrapposti As Boolean
        ) As String

        Dim MessaggioErrore As String = String.Empty
        Dim campConf_R As New FF_CampionamentoConferimento_R

        Const NomeRoutine = "ContabBIZ.FF_CampionamentoConferimento.CheckListiniProdotti()"

        Try

            'Controlli congruenza sulle date da <= a
            If listini_campConf_prodotti.Validita_Inizio > listini_campConf_prodotti.Validita_Fine Then
                MessaggioErrore += "Validità inizio deve essere minore o uguale a Validità fine <br/>"
            End If

            ' Controllo se è già stata utilizzata
            If controllaSeGiaUtilizzato AndAlso String.IsNullOrEmpty(MessaggioErrore) Then
                'TODO Stefano in futuro se memorizziamo nelle liquidazioni la key di listino che ci ha portato al calcolo
            End If

            'Controllo che non vi siano altre testate con periodi sovrapposti
            If controllaSePeriodiSovrapposti AndAlso String.IsNullOrEmpty(MessaggioErrore) Then
                Dim myQueryCount As Integer =
                    campConf_R.Controlla_Date_Listino_Prodotto(listini_campConf_prodotti, objParametri)
                If (myQueryCount > 0 AndAlso nuovoInserimento) OrElse
                   (myQueryCount > 1 AndAlso Not nuovoInserimento) Then
                    MessaggioErrore &= "Esistono altre righe con periodi sovrapposti al listino con periodo " & listini_campConf_prodotti.Validita_Inizio.ToShortDateString() & " - " & listini_campConf_prodotti.Validita_Fine.ToShortDateString()
                End If
            End If

        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
        End Try

        If Not String.IsNullOrEmpty(MessaggioErrore) Then
        End If

        Return MessaggioErrore
    End Function

    Public Function CancellaPrezziListinoConferimento(ByVal piva As String,
                                                      ByVal KeyListinoProdottoCalibro As String,
                                                      ByVal validoDal As String,
                                                      ByVal validoAl As String,
                                                      ByRef objParametri As AgronicaCoreParametri
                                                      ) As String

        Const nomeRoutine = "ContabBIZ.FF_CampionamentoConferimento.CancellaPrezziListinoConferimento()"
        Dim messaggioErrore As String = String.Empty
        Dim campConf_R As New FF_CampionamentoConferimento_R

        Dim keys As String() = KeyListinoProdottoCalibro.Split("-")
        Dim key_piva As String = keys(0)
        Dim key_Listino_Cod As Integer = Integer.Parse(keys(1))
        Dim key_Id_TestataGriglia_Prod As Integer = Integer.Parse(keys(2))

        Dim validoDalDateTime As Date
        If Not String.IsNullOrEmpty(validoDal) Then
            validoDalDateTime = Convert.ToDateTime(validoDal)
        End If

        Dim validoAlDateTime As Date
        If Not String.IsNullOrEmpty(validoAl) Then
            validoAlDateTime = Convert.ToDateTime(validoAl)
        End If

        Try
            ' TODO Stefano
            '  In futuro sui movimenti di acconto / liquidazione sarebbe bene mettere un riferimento alla riga di listino
            '  che ha determinato il prezzo, quindi fare un controllo che non si cancelli il listino se esistono già
            '  movimenti

            ' Aggiornamento effettivo
            If String.IsNullOrEmpty(messaggioErrore) Then
                Dim camConf_W As New FF_CampionamentoConferimento_W
                messaggioErrore = camConf_W.CancellaPrezziListinoConferimento(piva, key_Listino_Cod, key_Id_TestataGriglia_Prod,
                    validoDalDateTime, validoAlDateTime, objParametri)
            End If

        Catch ex As Exception
            messaggioErrore = "[" & nomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
        End Try

    End Function

    Public Function Aggiorna_Listini_Prodotti_Equivalenti(ByVal piva As String,
                                                          ByVal KeyListinoProdottoCalibro As String,
                                                          ByVal stringObjectsToSave As String,
                                                          ByRef objParametri As AgronicaCoreParametri
                                                          ) As String

        Const nomeRoutine = "ContabBIZ.FF_CampionamentoConferimento.Aggiorna_Listini_Prodotti_Equivalenti()"
        Dim messaggioErrore As String = String.Empty

        Dim keys As String() = KeyListinoProdottoCalibro.Split("-")
        Dim key_piva As String = keys(0)
        Dim key_Listino_Cod As Integer = Integer.Parse(keys(1))
        Dim key_Id_TestataGriglia_Prod As Integer = Integer.Parse(keys(2))

        Try
            Dim objToSaveArray As JArray = JArray.Parse(stringObjectsToSave)
            Dim campConfTestataGriglia_Prodotti_Equivalenti As New Listini_CampionamentoConferito_Prodotti_Equivalenti
            Dim EFArrayToInsert As New ArrayList
            Dim EFArrayToDelete As New ArrayList
            For Each obj As JObject In objToSaveArray
                campConfTestataGriglia_Prodotti_Equivalenti = New Listini_CampionamentoConferito_Prodotti_Equivalenti
                campConfTestataGriglia_Prodotti_Equivalenti.Piva_SuperUser = objParametri.PivaSuperUser
                campConfTestataGriglia_Prodotti_Equivalenti.PIVA = piva
                campConfTestataGriglia_Prodotti_Equivalenti.Listino_Cod = key_Listino_Cod
                campConfTestataGriglia_Prodotti_Equivalenti.Id_TestataGriglia_Prod_Equivalente = key_Id_TestataGriglia_Prod
                campConfTestataGriglia_Prodotti_Equivalenti.Id_TestataGriglia_Prod = obj("Id_TestataGriglia_Prod")
                campConfTestataGriglia_Prodotti_Equivalenti.Data_Creazione = Date.Now
                campConfTestataGriglia_Prodotti_Equivalenti.Username_Creazione = objParametri.UsernameOperazione
                campConfTestataGriglia_Prodotti_Equivalenti.Data_Modifica = Date.Now
                campConfTestataGriglia_Prodotti_Equivalenti.Username_Modifica = objParametri.UsernameOperazione
                campConfTestataGriglia_Prodotti_Equivalenti.inviato = 0
                campConfTestataGriglia_Prodotti_Equivalenti.Validita_Inizio = ValiditaInizio
                campConfTestataGriglia_Prodotti_Equivalenti.Validita_Fine = ValiditaFine

                If Boolean.Parse(obj("Selected")) Then
                    EFArrayToInsert.Add(campConfTestataGriglia_Prodotti_Equivalenti)
                Else
                    'TODO Stefano
                    '  In futuro se su una tabella ci portiamo la chiave che ha portato a determinare il prezzo occorre inserire qualche controllo
                    'Dim campConf_R As New FF_CampionamentoConferimento_R
                    'Dim giaUsata As String = campConf_R.Controllo_Esistenza_CampionamentoConferito_Movimenti_PerProdotto(campConfTestataGriglia_Prodotti.PIVA,
                    '                                                                                campConfTestataGriglia_Prodotti.Id_TestataGriglia,
                    '                                                                                campConfTestataGriglia_Prodotti.Mat_Cod,
                    '                                                                                objParametri)
                    'If Not String.IsNullOrEmpty(giaUsata) Then
                    '    MessaggioErrore += "Associazione griglia / prodotti " + giaUsata + "  già utilizzata, non è possibile modificarla<br/>"
                    'Else
                    EFArrayToDelete.Add(campConfTestataGriglia_Prodotti_Equivalenti)
                    'End If
                End If
            Next

            If String.IsNullOrEmpty(messaggioErrore) Then
                Dim campConf_W As New FF_CampionamentoConferimento_W
                messaggioErrore = campConf_W.Aggiorna_Listini_Prodotti_Equivalenti(EFArrayToInsert,
                    EFArrayToDelete, objParametri)
            End If

        Catch ex As Exception
            messaggioErrore = "[" & nomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
        Finally

        End Try


        If Not String.IsNullOrEmpty(messaggioErrore) Then
            Throw New Exception(messaggioErrore)
        End If

        Return messaggioErrore

    End Function

    Public Function Aggiorna_Fattori_Variazione_ParametriQualitativi(
            ByVal piva As String,
            ByVal righeInserite As String,
            ByVal righeModificate As String,
            ByVal righeCancellate As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As String

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim MessaggioErrore As String = String.Empty

        Dim NomeRoutine As String = "ContabBIZ.FF_CampionamentoConferimento.Aggiorna_Fattori_Variazione_ParametriQualitativi()"

        Try
            Dim campConf_R As New FF_CampionamentoConferimento_R

            Dim campConfFattVarParQual As New CampionamentoConferito_Fattori_Variazione_ParamQualitativi
            Dim righeInseriteArray As JArray = JArray.Parse(righeInserite)
            Dim righeModificateArray As JArray = JArray.Parse(righeModificate)
            Dim righeCancellateArray As JArray = JArray.Parse(righeCancellate)
            Dim EFArrayToInsert As New ArrayList
            Dim EFArrayToUpdate As New ArrayList
            Dim EFArrayToDelete As New ArrayList

            For Each obj As JObject In righeInseriteArray
                campConfFattVarParQual = New CampionamentoConferito_Fattori_Variazione_ParamQualitativi
                campConfFattVarParQual.Piva_SuperUser = Piva_SuperUser
                campConfFattVarParQual.PIVA = piva
                campConfFattVarParQual.Tabella_Cod = obj("Tabella_ID")
                campConfFattVarParQual.Tabella_Par_Cod = obj("val_cod")
                campConfFattVarParQual.Data_Creazione = Date.Now
                campConfFattVarParQual.Username_Creazione = objParametri.UsernameOperazione
                campConfFattVarParQual.Data_Modifica = Date.Now
                campConfFattVarParQual.Username_Modifica = objParametri.UsernameOperazione
                campConfFattVarParQual.inviato = 0
                campConfFattVarParQual.Validita_Inizio = ValiditaInizio
                campConfFattVarParQual.Validita_Fine = ValiditaFine

                EFArrayToInsert.Add(campConfFattVarParQual)
            Next

            For Each obj As JObject In righeModificateArray
                campConfFattVarParQual = campConf_R.Leggi_Elem_Fattore_Variazione_ParamQualitativi(piva, obj("Id_fattore_variazione"), objParametri)
                If campConfFattVarParQual Is Nothing Then
                    MessaggioErrore += "Fattore di variazione con ID " & obj("Id_fattore_variazione").ToString & " non trovato <br/>"
                Else
                    campConfFattVarParQual.Tabella_Cod = obj("Tabella_ID")
                    campConfFattVarParQual.Tabella_Par_Cod = obj("val_cod")
                    campConfFattVarParQual.Data_Modifica = Date.Now
                    campConfFattVarParQual.Username_Modifica = objParametri.UsernameOperazione
                    EFArrayToUpdate.Add(campConfFattVarParQual)
                End If
            Next

            For Each obj As JObject In righeCancellateArray
                campConfFattVarParQual = New CampionamentoConferito_Fattori_Variazione_ParamQualitativi
                campConfFattVarParQual.Piva_SuperUser = Piva_SuperUser
                campConfFattVarParQual.PIVA = piva
                campConfFattVarParQual.Id_fattore_variazione = obj("Id_fattore_variazione")
                Dim righeEsistenti As Integer = 0
                ' Controllare che non si possono fare modifiche se ci sono righe con valori collegate
                Dim nrRighe As String = campConf_R.Leggi_ValoriFattoriVariazione(campConfFattVarParQual.PIVA, "",
                      campConfFattVarParQual.Id_fattore_variazione, righeEsistenti, objParametri)
                If righeEsistenti > 0 Then
                    MessaggioErrore &= "Il fattore di variazione " & obj("Tabella_Des").ToString() & " - " & obj("val_des").ToString() & " ha già " & righeEsistenti.ToString() & " righe con prezzo collegate, non è possibile cancellarlo <br/>"
                End If

                EFArrayToDelete.Add(campConfFattVarParQual)
            Next

            If String.IsNullOrEmpty(MessaggioErrore) Then
                Dim campConf_W As New FF_CampionamentoConferimento_W

                MessaggioErrore = campConf_W.Aggiorna_Fattori_Variazione_ParametriQualitativi(piva, EFArrayToInsert, EFArrayToUpdate,
                    EFArrayToDelete, objParametri)

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

    Public Function Aggiorna_Valori_Fattori_Variazione(
            ByVal piva As String,
            ByVal KeyListinoProdottoCalibro As String,
            ByVal righeInserite As String,
            ByVal righeModificate As String,
            ByVal righeCancellate As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As String

        Dim keys As String() = KeyListinoProdottoCalibro.Split("-")
        Dim key_piva As String = keys(0)
        Dim key_Listino_Cod As Integer = Integer.Parse(keys(1))
        Dim key_Id_TestataGriglia_Prod As Integer = Integer.Parse(keys(2))

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim MessaggioErrore As String = String.Empty

        Dim NomeRoutine As String = "ContabBIZ.FF_CampionamentoConferimento.Aggiorna_Valori_Fattori_Variazione()"

        Try
            Dim campConf_R As New FF_CampionamentoConferimento_R

            Dim listFattVar As New Listini_CampionamentoConferito_Fattori_Variazione

            Dim righeInseriteArray As JArray = JArray.Parse(righeInserite)
            Dim righeModificateArray As JArray = JArray.Parse(righeModificate)
            Dim righeCancellateArray As JArray = JArray.Parse(righeCancellate)
            Dim EFArrayToInsert As New ArrayList
            Dim EFArrayToUpdate As New ArrayList
            Dim EFArrayToDelete As New ArrayList
            For Each obj As JObject In righeInseriteArray
                listFattVar = New Listini_CampionamentoConferito_Fattori_Variazione
                listFattVar.Id_listino_fattore_variaz = 0
                listFattVar.Piva_SuperUser = Piva_SuperUser
                listFattVar.PIVA = piva
                listFattVar.Id_fattore_variazione = obj("Id_fattore_variazione")
                listFattVar.Listino_Cod = key_Listino_Cod
                listFattVar.Id_TestataGriglia_Prod = key_Id_TestataGriglia_Prod
                listFattVar.variazione_a_valore = obj("variazione_a_valore")
                listFattVar.valore_al_kg = obj("valore_al_kg")
                If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
                    listFattVar.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio"), Format, Provider)
                End If
                If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
                    listFattVar.Validita_Fine = Date.ParseExact(obj("Validita_Fine"), Format, Provider)
                End If
                listFattVar.Data_Creazione = Date.Now
                listFattVar.Username_Creazione = objParametri.UsernameOperazione
                listFattVar.Data_Modifica = Date.Now
                listFattVar.Username_Modifica = objParametri.UsernameOperazione
                listFattVar.inviato = 0

                ' Controllo che non ci siano periodi sovrapposti già registrati
                MessaggioErrore += CheckListinoFattoreVariazione(listFattVar, objParametri, False, True)
                EFArrayToInsert.Add(listFattVar)
            Next
            For Each obj As JObject In righeModificateArray
                listFattVar = campConf_R.Leggi_Elem_Listino_Fattore_Variazione(piva, obj("Id_listino_fattore_variaz"), objParametri)
                If listFattVar Is Nothing Then
                    MessaggioErrore += "Riga da aggiornare " & obj("Validita_Inizio").ToString & " - " & obj("Validita_Fine").ToString & " non trovata"
                Else
                    listFattVar.Id_fattore_variazione = obj("Id_fattore_variazione")
                    listFattVar.variazione_a_valore = obj("variazione_a_valore")
                    listFattVar.valore_al_kg = obj("valore_al_kg")
                    If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
                        listFattVar.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio"), Format, Provider)
                    End If
                    If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
                        listFattVar.Validita_Fine = Date.ParseExact(obj("Validita_Fine"), Format, Provider)
                    End If
                    listFattVar.Data_Modifica = Date.Now
                    listFattVar.Username_Modifica = objParametri.UsernameOperazione
                    ' Controllo che non si possano fare modifiche se ci sono movimenti collegati e che non ci siano periodi sovrapposti già registrati
                    MessaggioErrore += CheckListinoFattoreVariazione(listFattVar, objParametri, True, True)
                    EFArrayToUpdate.Add(listFattVar)
                End If
            Next
            For Each obj As JObject In righeCancellateArray
                listFattVar = New Listini_CampionamentoConferito_Fattori_Variazione
                listFattVar.Piva_SuperUser = Piva_SuperUser
                listFattVar.PIVA = piva
                listFattVar.Id_listino_fattore_variaz = obj("Id_listino_fattore_variaz")
                ' Controllo che non si possano fare modifiche se ci sono movimenti collegati
                MessaggioErrore += CheckListinoFattoreVariazione(listFattVar, objParametri, True, False)
                EFArrayToDelete.Add(listFattVar)
            Next

            ' Controlla se ci sono periodi sovrapposti all'interno delle righe che si stanno gestendo
            If String.IsNullOrEmpty(MessaggioErrore) Then
                Dim lstfattVarElem As Listini_CampionamentoConferito_Fattori_Variazione = Nothing
                Dim lstfattVarElemToCompare As Listini_CampionamentoConferito_Fattori_Variazione = Nothing
                Dim trovatoErrore As Boolean = False

                ' Righe nuove
                For i As Integer = 0 To EFArrayToInsert.Count - 1
                    lstfattVarElem = EFArrayToInsert(i)
                    For x As Integer = 0 To EFArrayToInsert.Count - 1
                        lstfattVarElemToCompare = EFArrayToInsert(x)
                        If i <> x Then
                            If (lstfattVarElem.Id_fattore_variazione = lstfattVarElemToCompare.Id_fattore_variazione AndAlso
                                ((lstfattVarElem.Validita_Inizio <= lstfattVarElemToCompare.Validita_Inizio AndAlso
                               lstfattVarElem.Validita_Fine >= lstfattVarElemToCompare.Validita_Inizio) OrElse
                               (lstfattVarElem.Validita_Inizio <= lstfattVarElemToCompare.Validita_Fine AndAlso
                               lstfattVarElem.Validita_Fine >= lstfattVarElemToCompare.Validita_Fine))) Then
                                trovatoErrore = True
                            End If
                        End If
                    Next
                    For x As Integer = 0 To EFArrayToUpdate.Count - 1
                        lstfattVarElemToCompare = EFArrayToUpdate(x)
                        If (lstfattVarElem.Id_fattore_variazione = lstfattVarElemToCompare.Id_fattore_variazione AndAlso
                                ((lstfattVarElem.Validita_Inizio <= lstfattVarElemToCompare.Validita_Inizio AndAlso
                             lstfattVarElem.Validita_Fine >= lstfattVarElemToCompare.Validita_Inizio) OrElse
                            (lstfattVarElem.Validita_Inizio <= lstfattVarElemToCompare.Validita_Fine AndAlso
                               lstfattVarElem.Validita_Fine >= lstfattVarElemToCompare.Validita_Fine))) Then
                            trovatoErrore = True
                        End If
                    Next

                    If trovatoErrore Then
                        MessaggioErrore &= "Esistono righe in periodi sovrapposti rispetto al periodo " &
                            lstfattVarElem.Validita_Inizio.ToString("dd/MM/yyyy") &
                            " - " &
                            lstfattVarElem.Validita_Fine.ToString("dd/MM/yyyy") &
                            "<br/>"
                    End If
                Next

                If Not trovatoErrore Then
                    ' Righe modificate
                    For i As Integer = 0 To EFArrayToUpdate.Count - 1
                        lstfattVarElem = EFArrayToUpdate(i)
                        For x As Integer = 0 To EFArrayToUpdate.Count - 1
                            lstfattVarElemToCompare = EFArrayToUpdate(x)
                            If i <> x Then
                                If (lstfattVarElem.Id_fattore_variazione = lstfattVarElemToCompare.Id_fattore_variazione AndAlso
                                    ((lstfattVarElem.Validita_Inizio <= lstfattVarElemToCompare.Validita_Inizio AndAlso
                                    lstfattVarElem.Validita_Fine >= lstfattVarElemToCompare.Validita_Inizio) OrElse
                                    (lstfattVarElem.Validita_Inizio <= lstfattVarElemToCompare.Validita_Fine AndAlso
                                    lstfattVarElem.Validita_Fine >= lstfattVarElemToCompare.Validita_Fine))) Then
                                    trovatoErrore = True
                                End If
                            End If
                        Next
                        For x As Integer = 0 To EFArrayToInsert.Count - 1
                            lstfattVarElemToCompare = EFArrayToInsert(x)
                            If (lstfattVarElem.Id_fattore_variazione = lstfattVarElemToCompare.Id_fattore_variazione AndAlso
                                ((lstfattVarElem.Validita_Inizio <= lstfattVarElemToCompare.Validita_Inizio AndAlso
                               lstfattVarElem.Validita_Fine >= lstfattVarElemToCompare.Validita_Inizio) OrElse
                               (lstfattVarElem.Validita_Inizio <= lstfattVarElemToCompare.Validita_Fine AndAlso
                               lstfattVarElem.Validita_Fine >= lstfattVarElemToCompare.Validita_Fine))) Then
                                trovatoErrore = True
                            End If
                        Next

                        If trovatoErrore Then
                            MessaggioErrore &= "Esistono righe in periodi sovrapposti rispetto al periodo " &
                            lstfattVarElem.Validita_Inizio.ToString("dd/MM/yyyy") &
                            " - " &
                            lstfattVarElem.Validita_Fine.ToString("dd/MM/yyyy") &
                            "<br/>"
                        End If
                    Next
                End If
            End If

            If String.IsNullOrEmpty(MessaggioErrore) Then
                Dim campConf_W As New FF_CampionamentoConferimento_W

                MessaggioErrore = campConf_W.Aggiorna_Valori_Fattori_Variazione(piva, EFArrayToInsert, EFArrayToUpdate,
                    EFArrayToDelete, objParametri)

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


    Public Function Aggiorna_Esclusione_Fattori_Variazione(
            ByVal piva As String,
            ByVal Id_listino_fattore_variaz As Integer,
            ByVal stringObjectsToSave As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As String

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim MessaggioErrore As String = String.Empty

        Dim NomeRoutine As String = "ContabBIZ.FF_CampionamentoConferimento.Aggiorna_Esclusione_Fattori_Variazione()"

        Try
            Dim objToSaveArray As JArray = JArray.Parse(stringObjectsToSave)
            Dim esclusione As New Listini_CampionamentoConferito_Esclusione_Fattore_Variazione
            Dim EFArrayToInsert As New ArrayList
            Dim EFArrayToDelete As New ArrayList
            For Each obj As JObject In objToSaveArray
                esclusione = New Listini_CampionamentoConferito_Esclusione_Fattore_Variazione
                esclusione.Piva_SuperUser = Piva_SuperUser
                esclusione.PIVA = piva
                esclusione.Id_listino_fattore_variaz = Id_listino_fattore_variaz
                If obj("Id_Calibro").ToString() <> "-1" Then
                    esclusione.Id_Calibro = obj("Id_Calibro")
                Else
                    esclusione.Id_Calibro = 0
                End If
                esclusione.Data_Creazione = Date.Now
                esclusione.Username_Creazione = objParametri.UsernameOperazione
                esclusione.Data_Modifica = Date.Now
                esclusione.Username_Modifica = objParametri.UsernameOperazione
                esclusione.inviato = 0
                esclusione.Validita_Inizio = ValiditaInizio
                esclusione.Validita_Fine = ValiditaFine

                If Boolean.Parse(obj("Selected")) Then
                    EFArrayToInsert.Add(esclusione)
                Else
                    'TODO Stefano
                    '  In futuro se su una tabella ci portiamo la chiave che ha portato a determinare il prezzo occorre inserire qualche controllo
                    'Dim campConf_R As New FF_CampionamentoConferimento_R
                    'Dim giaUsata As String = campConf_R.Controllo_Esistenza_CampionamentoConferito_Movimenti_PerProdotto(campConfTestataGriglia_Prodotti.PIVA,
                    '                                                                                campConfTestataGriglia_Prodotti.Id_TestataGriglia,
                    '                                                                                campConfTestataGriglia_Prodotti.Mat_Cod,
                    '                                                                                objParametri)
                    'If Not String.IsNullOrEmpty(giaUsata) Then
                    '    MessaggioErrore += "Associazione griglia / prodotti " + giaUsata + "  già utilizzata, non è possibile modificarla<br/>"
                    'Else
                    EFArrayToDelete.Add(esclusione)
                    'End If
                End If
            Next

            If String.IsNullOrEmpty(MessaggioErrore) Then
                Dim campConf_W As New FF_CampionamentoConferimento_W
                MessaggioErrore = campConf_W.Aggiorna_Esclusione_Fattori_Variazione(EFArrayToInsert,
                    EFArrayToDelete, objParametri)
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

    Public Function CheckListinoFattoreVariazione(
            ByVal listFattVarElem As Listini_CampionamentoConferito_Fattori_Variazione,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
            ByRef controllaSeGiaUtilizzato As Boolean,
            ByRef controllaSePeriodiSovrapposti As Boolean
        ) As String

        Dim MessaggioErrore As String = String.Empty
        Dim campConf_R As New FF_CampionamentoConferimento_R

        Dim NomeRoutine As String = "ContabBIZ.FF_CampionamentoConferimento.CheckListinoFattoreVariazione()"

        Try

            'Controlli congruenza sulle date da <= a
            If listFattVarElem.Validita_Inizio > listFattVarElem.Validita_Fine Then

                MessaggioErrore &= ": Data inizio " & listFattVarElem.Validita_Inizio.ToString("dd/MM/yyyy") &
                                    " deve essere minore di Data fine " & listFattVarElem.Validita_Fine.ToString("dd/MM/yyyy") &
                                    "<br/>"

            End If

            ' Controllo se la griglia è già stata utilizzata
            If controllaSeGiaUtilizzato AndAlso String.IsNullOrEmpty(MessaggioErrore) Then
                ' TODO Stefano
                '   In futuro controllare che non si possono fare modifiche se ci sono liquidazioni per le quali si è utilizzato
                '   questo fattore di variazione
            End If

            'Controllo che non vi siano altre testate con periodi sovrapposti
            If controllaSePeriodiSovrapposti AndAlso String.IsNullOrEmpty(MessaggioErrore) Then
                Dim myQueryCount As Integer =
                    campConf_R.Controlla_Date_Listini_FattoriVariazione(listFattVarElem, objParametri)
                If myQueryCount > 0 Then
                    MessaggioErrore &= "Esistono righe in periodi sovrapposti rispetto al periodo " &
                            listFattVarElem.Validita_Inizio.ToString("dd/MM/yyyy") &
                            " - " &
                            listFattVarElem.Validita_Fine.ToString("dd/MM/yyyy") &
                            "<br/>"
                End If
            End If

        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
        End Try

        Return MessaggioErrore
    End Function

    Public Function Nuovo_Elem_AnagAccontiLiquidazioni(ByVal piva As String,
                                                       ByRef objParametri As AgronicaCoreParametri
                                                       ) As String

        Dim risposta As String

        Dim anagElem As New AnagAccontiLiquidazioni_CampionamentoConferito With {
            .descrizione = "",
            .tipo_anagrafica = "L",
            .tipo_acconto = "C",
            .perc_valore_acconto = 0,
            .definitivo = 0
        }

        Dim serializerSettings As New JsonSerializerSettings With {
            .ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        }
        risposta = JsonConvert.SerializeObject(anagElem, Formatting.None, serializerSettings)

        Return risposta

    End Function

    Public Function Cancella_Elem_AnagAccontiLiquidazioni(ByVal piva As String,
                                                          ByVal IDAnag As Integer,
                                                          ByRef objParametri As AgronicaCoreParametri
                                                          ) As String

        Const nomeRoutine = "ContabBIZ.FF_CampionamentoConferimento.Cancella_Elem_AnagAccontiLiquidazioni()"
        Dim messaggioErrore As String = String.Empty
        Dim leggi_R As New FF_CampionamentoConferimento_R

        Try
            Dim Elem As AnagAccontiLiquidazioni_CampionamentoConferito = leggi_R.LeggiElem_AnagAccontiLiquidazioni(piva, IDAnag, objParametri)
            If Elem Is Nothing Then
                messaggioErrore += "Non è stato trovato l'elemento da cancellare"
            Else
                If Elem.tipo_anagrafica.Equals("L") Then
                    Dim count As Integer = leggi_R.ContaAccontiCollegatiLiquidazione(piva, Elem.id_anagrafica, objParametri)
                    If count > 0 Then
                        messaggioErrore &= "Non è possibile eseguire la cancellazione in quanto la liquidazione è collegata a " & CStr(count) & " acconto/i"
                    End If
                End If
            End If

            ' Aggiornamento effettivo
            If String.IsNullOrEmpty(messaggioErrore) Then
                Dim elem_W As New FF_CampionamentoConferimento_W
                messaggioErrore = elem_W.CancellaInteraLiquidazione(Elem.PIVA, Elem.id_anagrafica, objParametri)
            End If

        Catch ex As Exception
            messaggioErrore = "[" & nomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
        End Try

        Return messaggioErrore

    End Function

    Public Function Inserisci_Modifica_Elem_AnagAccontiLiquidazioni(ByVal piva As String,
                                                                    ByVal riga As String,
                                                                    ByRef objParametri As AgronicaCoreParametri
                                                                    ) As String

        Const nomeRoutine = "ContabBIZ.FF_CampionamentoConferimento.Inserisci_Modifica_Elem_AnagAccontiLiquidazioni()"
        Dim messaggioErrore As String = String.Empty

        Dim Elem As AnagAccontiLiquidazioni_CampionamentoConferito = JsonConvert.DeserializeObject(Of AnagAccontiLiquidazioni_CampionamentoConferito)(riga)

        If Elem.id_liquidazione_riferimento = 0 Then
            Elem.id_liquidazione_riferimento = Nothing
        End If

        '18/7/2018 Forzo a 100 il valore per le liquidazioni perché ora anche su queste si può avere una % parziale
        If Elem.tipo_anagrafica = "L" AndAlso Elem.perc_valore_acconto = 0 Then
            Elem.perc_valore_acconto = 100
        End If

        ' Aggiornamento effettivo
        If String.IsNullOrEmpty(messaggioErrore) Then

            Dim elem_W As New FF_CampionamentoConferimento_W

            If Elem.id_anagrafica <= 0 Then
                messaggioErrore = elem_W.InserisciAnagAccontiLiquidazioni(piva, Elem, objParametri)
            Else
                messaggioErrore = elem_W.ModificaAnagAccontiLiquidazioni(Elem, objParametri)
            End If
        End If

        Return messaggioErrore

    End Function

    Public Shared Function StringToInteger(st As String) As Integer
        Return CInt(st)
    End Function

    Public Function Aggiorna_Gruppi_Fatturazione_ParametriQualitativi(ByVal piva As String,
                                                                      ByVal righeModificate As String,
                                                                      ByRef objParametri_Server As AgronicaCoreParametri,
                                                                      ByRef objParametri_Utenti As AgronicaCoreParametri
                                                                      ) As String

        Const nomeRoutine = "ContabBIZ.FF_CampionamentoConferimento.Aggiorna_Gruppi_Fatturazione_ParametriQualitativi()"
        Dim messaggioErrore As String = String.Empty

        Try
            Dim leggi As New FF_CampionamentoConferimento_R
            Dim leggi_mp As New Materie_Prime_R

            Dim mp As String
            Dim otp As OTabelle_Parametri
            Dim mpd As Materie_Prime_Dettagli
            Dim righeModificateArray As JArray = JArray.Parse(righeModificate)
            Dim EFArrayToUpdate As New ArrayList
            Dim EFArrayToInsert As New ArrayList

            For Each obj As JObject In righeModificateArray
                mp = leggi_mp.Leggi_MateriePrime_Conferimento_ConControlloMovimentato(piva, TRASFORMATI_VEGETALI, obj("Mat_Cod"), False, "", False, objParametri_Server, objParametri_Utenti)
                If mp Is Nothing OrElse mp = "[]" Then
                    messaggioErrore += "Prodotto con ID " & obj("Mat_Cod").ToString & " non trovato <br/>"
                Else
                    Dim success As Boolean = True
                    Dim tabella_par_cod As Integer = obj("Tabella_Par_Cod")

                    If tabella_par_cod <> 0 Then
                        otp = leggi.Leggi_GruppoFatturazione(piva, tabella_par_cod, objParametri_Server)
                        If otp Is Nothing Then
                            messaggioErrore += "Gruppo fatturazione con ID " & obj("Tabella_Par_Cod").ToString & " non trovato <br/>"
                            success = False
                        End If
                    End If

                    If success Then

                        mpd = leggi.Leggi_MateriePrimeDettagli(piva, obj("Mat_Cod").ToString, objParametri_Server)

                        If mpd Is Nothing Then

                            mpd = New Materie_Prime_Dettagli
                            mpd.Piva_SuperUser = objParametri_Server.PivaSuperUser
                            mpd.Piva = piva
                            mpd.Mat_Cod = obj("Mat_Cod")
                            mpd.Extra_Int1 = obj("Tabella_Par_Cod")
                            mpd.Data_Creazione = Date.Now
                            mpd.Data_Modifica = Date.Now
                            mpd.Username_Creazione = objParametri_Server.UsernameOperazione
                            mpd.Username_Modifica = objParametri_Server.UsernameOperazione
                            mpd.Validita_Inizio = AGRODATAINIZIO
                            mpd.Validita_Fine = AGRODATAFINE

                            mpd.Extra_Smallint1 = 0
                            mpd.Extra_Smallint2 = 0
                            mpd.Extra_Smallint3 = 0
                            mpd.Extra_Smallint4 = 0
                            mpd.Extra_Smallint5 = 0
                            mpd.Extra_Smallint6 = 0
                            mpd.Extra_Int2 = 0
                            mpd.Extra_Int3 = 0
                            mpd.Extra_Int4 = 0
                            mpd.Extra_Int5 = 0
                            mpd.Extra_Int6 = 0
                            mpd.Extra_Dbl1 = 0
                            mpd.Extra_Dbl2 = 0
                            mpd.Extra_Dbl3 = 0
                            mpd.Extra_Dbl4 = 0
                            mpd.Extra_Dbl5 = 0
                            mpd.Extra_Dbl6 = 0
                            mpd.Extra_Str1 = ""
                            mpd.Extra_Str2 = ""
                            mpd.Extra_Str3 = ""
                            mpd.Extra_Str4 = ""
                            mpd.Extra_Str5 = ""
                            mpd.Extra_Str6 = ""
                            mpd.Extra_Date1 = AGRODATAINIZIO
                            mpd.Extra_Date2 = AGRODATAINIZIO
                            mpd.Extra_Date3 = AGRODATAINIZIO
                            mpd.Extra_Date4 = AGRODATAINIZIO
                            mpd.Extra_Date5 = AGRODATAINIZIO
                            mpd.Extra_Date6 = AGRODATAINIZIO

                            EFArrayToInsert.Add(mpd)

                        Else
                            mpd.Extra_Int1 = obj("Tabella_Par_Cod")
                            mpd.Data_Modifica = Date.Now
                            mpd.Username_Modifica = objParametri_Server.UsernameOperazione

                            EFArrayToUpdate.Add(mpd)

                        End If

                    End If

                End If

            Next

            If String.IsNullOrEmpty(messaggioErrore) Then
                Dim campConf_W As New FF_CampionamentoConferimento_W

                messaggioErrore = campConf_W.Aggiorna_Gruppi_Fatturazione_ParametriQualitativi(piva, EFArrayToInsert, EFArrayToUpdate, objParametri_Server)

            End If

        Catch ex As Exception
            messaggioErrore = "[" & nomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
        End Try

        If Not String.IsNullOrEmpty(messaggioErrore) Then
            Throw New Exception(messaggioErrore)
        End If

        Return messaggioErrore

    End Function


    Public Function TrovaErroriInLottoDaImportare(ByVal piva As String,
                                                  ByVal ID As Integer,
                                                  ByVal tipo_importatore As TipoImportatore,
                                                  ByRef objParametri As AgronicaCoreParametri
                                                  ) As String

        Const nomeRoutine = "AgronicaCoreContabDAL.FF_CampionamentoConferimento.TrovaErroriInLottoDaImportare()"
        Dim risposta As String = ""


        Dim campConf_R As New FF_CampionamentoConferimento_R

        Dim lotto As cbl_Calibrature = campConf_R.implo_LeggiLotto(ID, objParametri)
        If lotto Is Nothing Then
            Throw New Exception("Lotto campionamento con ID " & ID.ToString() & " non trovato")
        End If

        Dim errors As New List(Of ErroriImportazione)

        If tipo_importatore = TipoImportatore.Importatore_Calibratrice Then

            campConf_R.TrovaErroriInLottoDaImportare1(piva, lotto, errors, objParametri)

        ElseIf tipo_importatore = TipoImportatore.Importatore_Campionatrice Then

            Dim Id_TestataGriglia As Integer
            Dim Id_TestataGriglia_Prod As Integer
            Dim Id_Mov_Det As Integer

            campConf_R.TrovaErroriInLottoDaImportare2(piva, lotto, Id_TestataGriglia, Id_TestataGriglia_Prod, Id_Mov_Det, errors, objParametri)

        End If

        Dim strErrs As New List(Of String)
        Dim str_e As String
        For Each e In errors

            str_e = String.Format("{0:d}", e) & "|"

            Select Case e
                Case ErroriImportazione.LottoImportato
                    str_e &= "Lotto/Bolla già importati"
                Case ErroriImportazione.CalibroNonTrovato
                    str_e &= "Calibro importato non trovato in griglia"
                Case ErroriImportazione.TestataGrigliaNonTrovata
                    str_e &= "Testata griglia campionamento non trovata"
                Case ErroriImportazione.RigaBollaNonTrovata
                    str_e &= "Riga in bolla non trovata"
                Case ErroriImportazione.BollaNonTrovata
                    str_e &= "Bolla non trovata"
                Case ErroriImportazione.ProduttoreNonTrovato
                    str_e &= "Produttore non trovato"
                Case ErroriImportazione.NessunErrore
                    str_e &= "Nessun errore"
                Case ErroriImportazione.DettaglioNonEsiste
                    str_e &= "Dettaglio lotto inesistente"
                Case ErroriImportazione.ProduttoreDiverso
                    str_e &= "Produttore differente"
                Case ErroriImportazione.LottoCampionato
                    str_e &= "Lotto già campionato"
                Case ErroriImportazione.CalibroNonImportato
                    str_e &= "Calibro in griglia non trovato in importazione"
                Case ErroriImportazione.PercentualeNon100
                    str_e &= "Percentuale righe campionamento non uguale a 100%"
                Case ErroriImportazione.CalibroDuplicato
                    str_e &= "Calibro duplicato"
                Case ErroriImportazione.OrdinamentoNonRispettato
                    str_e &= "Ordinamento in importazione non rispettato"
                Case Else
                    str_e &= "Errore non identificato"
            End Select

            If e <> ErroriImportazione.NessunErrore Then
                strErrs.Add(str_e)
            End If

        Next

        lotto.Errore = String.Join("|", strErrs)

        Dim campConf_W As New FF_CampionamentoConferimento_W

        campConf_W.implo_ScriviLotto(lotto, objParametri)

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        risposta = JsonConvert.SerializeObject(lotto, Formatting.None, serializerSettings)

        Return risposta

    End Function

    Public Function ControllaLottiDaImportare(ByVal piva As String,
                                              ByVal righeDaControllare As String,
                                              ByRef objParametri As AgronicaCoreParametri
                                              ) As String

        Const nomeRoutine = "ContabBIZ.FF_CampionamentoConferimento.ControllaLottiDaImportare()"
        Dim messaggioErrore As String = String.Empty

        Dim counter As Integer = 0
        Try

            Dim righeDaControllareArray As JArray = JArray.Parse(righeDaControllare)

            For Each obj As JObject In righeDaControllareArray
                TrovaErroriInLottoDaImportare(piva, CInt(obj("ID_Lotto")), CInt(obj("Importatore")), objParametri)
                counter += 1
            Next

        Catch ex As Exception
            messaggioErrore = "[" & nomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
        End Try


        If Not String.IsNullOrEmpty(messaggioErrore) Then
            Throw New Exception(messaggioErrore)
        End If

        Return messaggioErrore

    End Function


    Public Function AggiornaDettaglioLottoDaImportare(ByVal piva As String,
                                                      ByVal lid As Integer,
                                                      ByVal stringObjectsToSave As String,
                                                      ByRef objParametri As AgronicaCoreParametri) As String

        Const nomeRoutine = "AgronicaCoreContabBIZ.FF_CampionamentoConferimento.AggiornaDettaglioLottoDaImportare()"
        Dim messaggioErrore As String = String.Empty

        Try
            Dim tmpArray As JArray = JArray.Parse(stringObjectsToSave)
            Dim objToSaveArray As IEnumerable(Of JToken) = tmpArray.OrderBy(Function(obj) obj("OrdineLogico"))

            Dim calibro As New cbl_CalibratureXCalibri
            Dim EFArrayToUpdate As New ArrayList

            For Each obj As JObject In objToSaveArray

                If obj("Perc").ToString() = "" Then
                    obj("Perc") = 0
                End If

                calibro = New cbl_CalibratureXCalibri

                calibro.ID = obj("ID")
                calibro.IDCalibro = lid
                calibro.Nome = obj("Nome")
                calibro.Qualita = obj("Qualita")
                calibro.Perc = obj("Perc")
                calibro.Peso = obj("Peso")

                EFArrayToUpdate.Add(calibro)

            Next

            If String.IsNullOrEmpty(messaggioErrore) Then
                Dim campConf_W As New FF_CampionamentoConferimento_W
                messaggioErrore = campConf_W.AggiornaDettaglioLottoDaImportare(lid, EFArrayToUpdate, objParametri)
            End If


        Catch ex As Exception
            messaggioErrore = "[" & nomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
        End Try

        If Not String.IsNullOrEmpty(messaggioErrore) Then
            Throw New Exception(messaggioErrore)
        End If

        Return messaggioErrore

    End Function


    Public Function Elimina_Griglia_LottiDaImportare(ByVal piva As String,
                                                     ByVal righeCancellate As String,
                                                     ByRef objParametri As AgronicaCoreParametri
                                                     ) As String

        Const nomeRoutine = "ContabBIZ.FF_CampionamentoConferimento.Elimina_Griglia_LottiDaImportare()"
        Dim messaggioErrore As String = String.Empty

        Try

            Dim cbl_Cal As cbl_Calibrature
            Dim righeCancellateArray As JArray = JArray.Parse(righeCancellate)
            Dim EFArrayToDelete As New ArrayList

            For Each obj As JObject In righeCancellateArray

                cbl_Cal = New cbl_Calibrature With {
                    .ID = obj("ID_Lotto")
                }

                EFArrayToDelete.Add(cbl_Cal)

            Next

            Dim campConf_W As New FF_CampionamentoConferimento_W

            messaggioErrore = campConf_W.Elimina_Griglia_LottiDaImportare(piva, EFArrayToDelete, objParametri)

        Catch ex As Exception
            messaggioErrore = "[" & nomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
        End Try

        If Not String.IsNullOrEmpty(messaggioErrore) Then
            Throw New Exception(messaggioErrore)
        End If

        Return messaggioErrore
    End Function
    
    Public Function Importa_LottiDaImportare(ByVal piva As String,
                                             ByVal righeDaImportare As String,
                                             ByVal statoCampionamento As String,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As String

        Const nomeRoutine = "ContabBIZ.FF_CampionamentoConferimento.Importa_LottiDaImportare()"
        Dim messaggioErrore As String = String.Empty

        Dim campConf_R As New FF_CampionamentoConferimento_R
        Dim campConf_W As New FF_CampionamentoConferimento_W

        Try

            Dim lotto As cbl_Calibrature
            Dim Id_Testata_Griglia_Prod As Integer
            Dim Id_Mov_Det As Integer
            Dim errs As New List(Of ErroriImportazione)
            Dim assocCal As New List(Of FF_CampionamentoConferimento_R.JoinedCal)
            Dim righeDaImportareArray As JArray = JArray.Parse(righeDaImportare)

            Dim movimento As FF_CampionamentoConferimento_W.MovimentoDaImportare
            Dim EFArrayToImport As New ArrayList

            For Each obj As JObject In righeDaImportareArray

                lotto = campConf_R.implo_LeggiLotto(obj("ID_Lotto"), objParametri)
                If lotto Is Nothing Then
                    Throw New Exception("Lotto campionamento con ID " & obj("ID_Lotto").ToString() & " non trovato")
                End If

                campConf_R.TrovaErroriInLottoDaImportare2(piva, lotto, 0, Id_Testata_Griglia_Prod, Id_Mov_Det, errs, objParametri, assocCal)

                Dim erroreBloccante As Boolean = False
                For Each e As ErroriImportazione In errs
                    If e < 0 Then
                        erroreBloccante = True
                        Exit For
                    End If
                Next
                If erroreBloccante Then
                    Throw New Exception("Un lotto di campionamento contiene errori")
                End If

                'Controllare gli errori bloccanti
                'errs.Contains(ProduttoreNonTrovato) -> Produttore non trovato -> ERRORE
                'errs.Contains(BollaNonTrovata) -> Bolla non trovata -> ERRORE
                'Id_Mov_Det = 0 -> Riga bolla non trovata -> ERRORE
                'Id_Testata_Griglia = 0 -> Testata griglia non trovate -> ERRORE 
                'errs.Contains(CalibroNonTrovato) -> Calibro non presente in Griglia -> ERRORE
                'errs.Contains(LottoImportato) -> Duplice importazione del file -> ERRORE
                'errs.Contains(PercentualeNon100) -> Percentuale non uguale a 100 -> ERRORE

                'errs contiene CalibroNonImportato -> Calibro presente in griglia, ma non in importazione -> Importo con % = 0 
                'errs contiene LottoCampionato -> Campionamento già esistente -> effettuare la media ponderata?

                movimento = New FF_CampionamentoConferimento_W.MovimentoDaImportare

                movimento.Lotto = lotto

                movimento.Testata = New CampionamentoConferito_Movimenti
                movimento.Testata.Piva_SuperUser = objParametri.PivaSuperUser
                movimento.Testata.PIVA = piva
                movimento.Testata.Id_Mov_Det = Id_Mov_Det
                movimento.Testata.Id_TestataGriglia_Prod = Id_Testata_Griglia_Prod
                movimento.Testata.Udm_QtaCampionata = 2
                movimento.Testata.QtaCampionata = lotto.PesoTot
                movimento.Testata.StatoCampionamento = statoCampionamento
                movimento.Testata.Automatico = enum_TipoCampionamento.TC_Automatico
                movimento.Testata.inviato = 0
                'movimento.Testata.datainvio =
                movimento.Testata.Data_Creazione = Date.Now
                movimento.Testata.Data_Modifica = Date.Now
                movimento.Testata.Username_Creazione = objParametri.UsernameOperazione
                movimento.Testata.Username_Modifica = objParametri.UsernameOperazione
                movimento.Testata.Validita_Inizio = ValiditaInizio
                movimento.Testata.Validita_Fine = ValiditaFine
                'movimento.Testata.Note = 
                movimento.Testata.Dt_Inizio_Campionamento = lotto.Data_Inizio
                movimento.Testata.Dt_Fine_Campionamento = lotto.Data_Fine
                movimento.Testata.NrBolla_Campionamento = lotto.NrBolla

                movimento.Righe = New List(Of CampionamentoConferito_Movimenti_Righe)

                For Each elem In assocCal

                    If elem.ID_Calibro = 0 Then
                        'Calibro non trovato in griglia -> Errore bloccante
                    End If

                    If elem.Flag_Importato = 0 Then
                        'Importo con percentuale 0 -> Avviso non bloccante
                    End If

                    Dim movim_Riga As New CampionamentoConferito_Movimenti_Righe
                    movim_Riga.Piva_SuperUser = objParametri.PivaSuperUser
                    movim_Riga.PIVA = piva
                    movim_Riga.Id_Mov_Det = Id_Mov_Det
                    movim_Riga.Id_TestataGriglia_Prod = Id_Testata_Griglia_Prod
                    movim_Riga.Id_Calibro = elem.ID_Calibro
                    movim_Riga.PercentualeCampionato = elem.Perc
                    movim_Riga.inviato = 0
                    'movim_Riga.datainvio = 
                    movim_Riga.Data_Creazione = Date.Now
                    movim_Riga.Data_Modifica = Date.Now
                    movim_Riga.Username_Creazione = objParametri.UsernameOperazione
                    movim_Riga.Username_Modifica = objParametri.UsernameOperazione
                    movim_Riga.Validita_Inizio = ValiditaInizio
                    movim_Riga.Validita_Fine = ValiditaFine

                    movimento.Righe.Add(movim_Riga)

                Next

                EFArrayToImport.Add(movimento)

            Next

            messaggioErrore = campConf_W.Importa_LottiDaImportare(piva, EFArrayToImport, objParametri)

        Catch ex As Exception
            messaggioErrore = "[" & nomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
        End Try

        If Not String.IsNullOrEmpty(messaggioErrore) Then
            Throw New Exception(messaggioErrore)
        End If

        Return messaggioErrore

    End Function
    
    Public Function Aggiorna_LottiDaImportare(ByVal piva As String,
                                              ByVal righeDaAggiornare As String,
                                              ByRef objParametri As AgronicaCoreParametri
                                              ) As String

        Const nomeRoutine = "ContabBIZ.FF_CampionamentoConferimento.Aggiorna_LottiDaImportare()"
        Dim messaggioErrore As String = String.Empty

        Try

            Dim objToSaveArray As JArray = JArray.Parse(righeDaAggiornare)

            Dim elem As New cbl_Calibrature
            Dim EFArrayToUpdate As New ArrayList

            For Each obj As JObject In objToSaveArray

                elem = New cbl_Calibrature

                elem.ID = obj("ID_Lotto")
                elem.Lotto = obj("Lotto")
                elem.Varieta = obj("Varieta")
                elem.Conferitore_Codice = obj("Conferitore_Codice")
                elem.Conferitore_Nome = obj("Conferitore_Nome")
                elem.Programma = obj("Programma")
                elem.NrBolla = obj("NrBolla")
                elem.RifBolla = obj("RifBolla")
                'PesoTot
                'Errore
                elem.Data_Modifica = Date.Now
                elem.Username_Modifica = objParametri.UsernameOperazione

                EFArrayToUpdate.Add(elem)

            Next

            If String.IsNullOrEmpty(messaggioErrore) Then

                Dim campConf_W As New FF_CampionamentoConferimento_W
                messaggioErrore = campConf_W.AggiornaLottiDaImportare(EFArrayToUpdate, objParametri)

            End If

        Catch ex As Exception
            messaggioErrore = "[" & nomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
        End Try

        If Not String.IsNullOrEmpty(messaggioErrore) Then
            Throw New Exception(messaggioErrore)
        End If

        Return messaggioErrore

    End Function

    Public Function CalcolaMedia(ByVal piva As String,
                                 ByVal IdMovDetSelected As String,
                                 ByVal statoCampionamento As String,
                                 ByRef objParametri As AgronicaCoreParametri
                                 ) As String

        Const nomeRoutine = "ContabBIZ.FF_CampionamentoConferimento.Esegui_Media_Righe_Non_Campionate()"
        Dim messaggioErrore As String = String.Empty

        Dim separators() As String = {"|"}
        Dim IdMovDetToUpdate As String()
        If Not String.IsNullOrEmpty(IdMovDetSelected) Then
            IdMovDetToUpdate = IdMovDetSelected.Split(separators,
                          StringSplitOptions.RemoveEmptyEntries)
        End If

        Dim leggi As New FF_CampionamentoConferimento_R

        Try

            Dim elem As New cbl_Calibrature
            Dim EFArrayToUpdate As New ArrayList

            For Each IdMovDet In IdMovDetToUpdate

                ' Leggo ogni riga di cui calcolare il campionamento in media 
                Dim mdR As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
                Dim dtEntrata = mdR.MovimentiDettagli_Leggi_FF(piva,
                                            0,
                                            "",
                                            0,
                                            0,
                                            IdMovDet,
                                            0,
                                            0,
                                            0,
                                            0,
                                            0,
                                            LOTTO_NONDEFINITO,
                                            0,
                                            0,
                                            Nothing,
                                            Nothing,
                                            0,
                                            1,
                                            "",
                                            "",
                                            "",
                                            "",
                                            objParametri
                                            )

                If dtEntrata IsNot Nothing AndAlso dtEntrata.Rows.Count > 0 Then

                    Dim _fornitori As String() = {dtEntrata.Rows(0).Item("Cod_Contatto")}
                    Dim varietaDaFiltrareArrayInt As Integer() = {}
                    Dim operazioniDaFiltrareArrayInt As Integer() = {}
                    Dim gruppoFatturazioneArrayInt As Integer() = {}

                    Dim Id_Testata_Griglia_Trovata As Integer
                    Dim Id_Testata_Griglia_Prod_Trovata As Integer

                    'Trovo la testata griglia corrente
                    Dim s As String = leggi.Leggi_Id_Testata_Griglia_Da_Movim_Conferimento(piva,
                                IdMovDet, Id_Testata_Griglia_Trovata, Id_Testata_Griglia_Prod_Trovata, False, False, objParametri)

                    If Id_Testata_Griglia_Prod_Trovata = 0 Then

                        messaggioErrore &= "Griglia di campionamento non trovata per il fornitore " &
                                dtEntrata.Rows(0).Item("Rag_Soc") & " - documento " &
                                dtEntrata.Rows(0).Item("Doc_Numero_Sin") &
                                dtEntrata.Rows(0).Item("Doc_Numero") &
                                dtEntrata.Rows(0).Item("Doc_Numero_Des") & " - prodotto " &
                                dtEntrata.Rows(0).Item("Mat_Des") & " - qualità " &
                                dtEntrata.Rows(0).Item("Qualità_Sigla") & " - calibro " &
                                 dtEntrata.Rows(0).Item("Calibro_Sigla") & " - lotto " &
                                 dtEntrata.Rows(0).Item("Lotto") & "<br/>"
                    Else

                        ' Cerco tutte le entrate di conferimento già campionate uguali a parità di 
                        ' nr documento, socio, prodotto, qualità. calibro e testata griglia
                        Dim trovatoConStessoNrDocumento As Boolean = False
                        Dim risposta As String =
                                leggi.Leggi_Righe_Conferimento_E_Calibri(piva,
                                dtEntrata.Rows(0).Item("Doc_Numero_Sin"), dtEntrata.Rows(0).Item("Doc_Numero"),
                                dtEntrata.Rows(0).Item("Doc_Numero_Des"), 0, "", "", -1,
                                varietaDaFiltrareArrayInt, dtEntrata.Rows(0).Item("Mat_Cod"),
                                0, dtEntrata.Rows(0).Item("Qualità_Cod"), dtEntrata.Rows(0).Item("Calibro_Cod"),
                                operazioniDaFiltrareArrayInt, _fornitori, gruppoFatturazioneArrayInt, "",
                                enum_TipoCampionamento.TC_Manuale, "", True, Id_Testata_Griglia_Prod_Trovata, objParametri)

                        Dim myList As JArray = JsonConvert.DeserializeObject(risposta)

                        ' Secondo tentativo senza nr documento
                        If myList.Count = 0 Then
                            risposta =
                                leggi.Leggi_Righe_Conferimento_E_Calibri(piva,
                                "", 0, "", 0, "", "", -1,
                                varietaDaFiltrareArrayInt, dtEntrata.Rows(0).Item("Mat_Cod"),
                                0, dtEntrata.Rows(0).Item("Qualità_Cod"), dtEntrata.Rows(0).Item("Calibro_Cod"),
                                operazioniDaFiltrareArrayInt, _fornitori, gruppoFatturazioneArrayInt, "",
                                enum_TipoCampionamento.TC_Manuale, "", True, Id_Testata_Griglia_Prod_Trovata, objParametri)

                            myList = JsonConvert.DeserializeObject(risposta)
                        Else
                            trovatoConStessoNrDocumento = True
                        End If

                        ' Terzo tentativo senza calibro
                        If myList.Count = 0 Then
                            risposta =
                            leggi.Leggi_Righe_Conferimento_E_Calibri(piva,
                            "", 0, "", 0, "", "", -1,
                            varietaDaFiltrareArrayInt, dtEntrata.Rows(0).Item("Mat_Cod"),
                            0, dtEntrata.Rows(0).Item("Qualità_Cod"), 0,
                            operazioniDaFiltrareArrayInt, _fornitori, gruppoFatturazioneArrayInt, "",
                            enum_TipoCampionamento.TC_Manuale, "", True, Id_Testata_Griglia_Prod_Trovata, objParametri)

                            myList = JsonConvert.DeserializeObject(risposta)
                        End If

                        If myList.Count = 0 Then
                            'Messaggio di avvertimento nel caso non abbia trovato riferimenti
                            messaggioErrore &= "Non è stato possibile calcolare la media per il fornitore " &
                            dtEntrata.Rows(0).Item("Rag_Soc") & " - documento " &
                            dtEntrata.Rows(0).Item("Doc_Numero_Sin") &
                            dtEntrata.Rows(0).Item("Doc_Numero") &
                            dtEntrata.Rows(0).Item("Doc_Numero_Des") & " - prodotto " &
                            dtEntrata.Rows(0).Item("Mat_Des") & " - qualità " &
                            dtEntrata.Rows(0).Item("Qualità_Sigla") & " - calibro " &
                             dtEntrata.Rows(0).Item("Calibro_Sigla") & " - lotto " &
                             dtEntrata.Rows(0).Item("Lotto") & "<br/>"

                        Else

                            Dim tipoCampionamento = enum_TipoCampionamento.TC_MediaReferenza

                            Dim htMedia = New Hashtable
                            Dim totKg As Decimal = 0
                            Dim totKgCampionati As Decimal = 0
                            For Each obj In myList
                                ' Nel caso in cui abbia trovato altre righe tramite cui fare la media all'interno 
                                ' dello stesso documento imposto come tipo campionamento quella delle altre righe
                                If trovatoConStessoNrDocumento AndAlso CShort(obj("Automatico")) <> enum_TipoCampionamento.TC_MediaReferenza Then
                                    tipoCampionamento = CShort(obj("Automatico"))
                                End If

                                If Not htMedia.ContainsKey(CInt(obj("Id_Calibro"))) Then
                                    htMedia.Add(CInt(obj("Id_Calibro")), 0)
                                End If
                                htMedia(CInt(obj("Id_Calibro"))) = htMedia(CInt(obj("Id_Calibro"))) + CDec(obj("SviluppoTotale"))
                                totKg += CDec(obj("SviluppoTotale"))
                                totKgCampionati += CDec(obj("SviluppoCampionato"))
                            Next

                            Dim percMediaKgTotaliCampionati As Decimal = totKgCampionati * 100 / totKg
                            Dim kgCampionatiQuestaBolla As Double = CInt(dtEntrata.Rows(0).Item("Qta_extra_Totale") / 100 * percMediaKgTotaliCampionati)

                            Dim scrivi As New FF_CampionamentoConferimento
                            Dim EFArrayToInsertUpdate As New ArrayList

                            ' Arrotondo a 2 decimali mettendo la differenza sul campione maggiore
                            Dim htMediaFinale = New Hashtable
                            Dim perc As Decimal
                            Dim residuoPerc As Decimal = 100
                            Dim keyMaggiorePerc As Integer = 0
                            Dim valueMaggiorePerc As Decimal = 0
                            For Each campione In htMedia.Keys
                                perc = Math.Floor(100 * htMedia(campione) / totKg * 100) / 100
                                If perc > valueMaggiorePerc Then
                                    valueMaggiorePerc = perc
                                    keyMaggiorePerc = campione
                                End If
                                residuoPerc = residuoPerc - perc
                                htMediaFinale.Add(campione, perc)
                            Next
                            If residuoPerc <> 0 Then
                                htMediaFinale(keyMaggiorePerc) = htMediaFinale(keyMaggiorePerc) + residuoPerc
                            End If

                            For Each campione In htMediaFinale.Keys
                                Dim obj As New MediaCampionato() With {.Id_Calibro = campione, .PercentualeCampionato = htMediaFinale(campione)}
                                EFArrayToInsertUpdate.Add(obj)
                            Next

                            'Eseguo l'aggiornamento
                            If htMediaFinale.Count > 0 Then
                                Dim serializerSettings As New JsonSerializerSettings()
                                serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                                messaggioErrore = scrivi.Aggiorna_Campioni_RigaConferimento(piva, IdMovDet, Id_Testata_Griglia_Prod_Trovata,
                            statoCampionamento, tipoCampionamento, "", kgCampionatiQuestaBolla,
                            JsonConvert.SerializeObject(EFArrayToInsertUpdate, Formatting.None, serializerSettings), objParametri)
                            End If

                        End If

                    End If

                End If

            Next

        Catch ex As Exception
            messaggioErrore = "[" & nomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
        End Try

        If Not String.IsNullOrEmpty(messaggioErrore) Then
            Throw New Exception(messaggioErrore)
        End If

        Return messaggioErrore

    End Function

    Public Function CopiaCampionamentoConferito_TestataGriglia(ByVal ID_TestataFrom As Integer,
                                                               ByVal Descrizione As String,
                                                               ByVal Validita_Inizio As Date,
                                                               ByVal Validita_Fine As Date,
                                                               ByRef objParametri_Server As AgronicaCoreParametri
                                                               ) As String

        Const nomeRoutine = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_W.CopiaCampionamentoConferito_TestataGriglia()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
        Try

            Dim transactionOptions As New TransactionOptions()
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            transactionOptions.Timeout = TransactionManager.MaximumTimeout

            Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    Dim Piva = (From FFTestata In GiasContext.CampionamentoConferito_TestataGriglia Where FFTestata.Id_TestataGriglia = ID_TestataFrom).FirstOrDefault.PIVA

                    Dim username = objParametri_Server.UsernameOperazione
                    Dim data = Date.Now

                    Dim objFF_CampionamentoConferimento_R As New AgronicaCoreContabDAL.FF_CampionamentoConferimento_R
                    Dim objFF_CampionamentoConferimento_W As New AgronicaCoreContabDAL.FF_CampionamentoConferimento_W
                    Dim campConfTestataGriglia As New CampionamentoConferito_TestataGriglia
                    campConfTestataGriglia = New CampionamentoConferito_TestataGriglia
                    campConfTestataGriglia.Piva_SuperUser = objParametri_Server.PivaSuperUser
                    campConfTestataGriglia.PIVA = Piva
                    campConfTestataGriglia.des_TestataGriglia = Descrizione
                    campConfTestataGriglia.Validita_Inizio = Validita_Inizio
                    campConfTestataGriglia.Validita_Fine = Validita_Fine
                    campConfTestataGriglia.Id_TestataGriglia = 0
                    campConfTestataGriglia.Data_Creazione = Date.Now
                    campConfTestataGriglia.Username_Creazione = objParametri_Server.UsernameOperazione
                    campConfTestataGriglia.Data_Modifica = Date.Now
                    campConfTestataGriglia.Username_Modifica = objParametri_Server.UsernameOperazione
                    campConfTestataGriglia.inviato = 0
                    ' Controllo che non ci siano periodi sovrapposti già registrati
                    Dim MessaggioErrore = CheckTestataGriglia(campConfTestataGriglia, objParametri_Server, False, True, False, False)

                    If Not String.IsNullOrEmpty(MessaggioErrore) Then
                        Throw New Exception(MessaggioErrore)
                    End If


                    Dim ObjSequenze As New Agro_Sequenze

                    'Creo la Testata
                    'campConfTestataGriglia = objFF_CampionamentoConferimento_W.Scrivi_TestataGriglia_Campionamento(Piva, campConfTestataGriglia, objParametri_Server)
                    Dim idSeq = ObjSequenze.NuovoId_Tabella_EF(GiasContext, "campionamentoconferito_testatagriglia", 0, 2000000000, objParametri_Server)
                    campConfTestataGriglia.Id_TestataGriglia = idSeq
                    GiasContext.CampionamentoConferito_TestataGriglia.Add(campConfTestataGriglia)
                    GiasContext.SaveChanges()


                    Dim CampionamentoConferito_TestataGriglia_Calibri_Old_Str = objFF_CampionamentoConferimento_R.Leggi_TestataGriglie_Calibri(Piva, ID_TestataFrom, "", objParametri_Server)
                    Dim CampionamentoConferito_TestataGriglia_Calibri_Old = JArray.Parse(CampionamentoConferito_TestataGriglia_Calibri_Old_Str)
                    For Each Elem_Calibri In CampionamentoConferito_TestataGriglia_Calibri_Old
                        Dim CampConf_TestataGriglia_Calibri = objFF_CampionamentoConferimento_R.Leggi_Elem_Testata_GriglieCalibri(Piva, ID_TestataFrom, Elem_Calibri("Id_Calibro"), objParametri_Server)
                        Dim campConfTestataGriglia_Calibri = Gias_EF_Utility.CopyEntity(GiasContext, CampConf_TestataGriglia_Calibri, Nothing, username, data)
                        campConfTestataGriglia_Calibri.Username_Creazione = username
                        campConfTestataGriglia_Calibri.Username_Modifica = username
                        campConfTestataGriglia_Calibri.Data_Creazione = data
                        campConfTestataGriglia_Calibri.Data_Modifica = data

                        'objFF_CampionamentoConferimento_W.Scrivi_TestataGriglia_Calibri(Piva, campConfTestataGriglia.Id_TestataGriglia, Copy, objParametri_Server)
                        idSeq = ObjSequenze.NuovoId_Tabella_EF(GiasContext,
                                               "campionamentoconferito_testatagriglia_calibri", 0, 2000000000, objParametri_Server)
                        campConfTestataGriglia_Calibri.Id_Calibro = idSeq

                        campConfTestataGriglia_Calibri.PIVA = Piva
                        campConfTestataGriglia_Calibri.Id_TestataGriglia = campConfTestataGriglia.Id_TestataGriglia

                        GiasContext.CampionamentoConferito_TestataGriglia_Calibri.Add(campConfTestataGriglia_Calibri)
                        GiasContext.SaveChanges()

                    Next

                    Dim campConf_R As New FF_CampionamentoConferimento_R
                    Dim List_CampionamentoConferito_TestataGriglia_Prodotti = objFF_CampionamentoConferimento_R.Leggi_TestataGriglia_Prodotti(Piva, ID_TestataFrom, "", objParametri_Server)
                    For Each Elem_Prodotti In List_CampionamentoConferito_TestataGriglia_Prodotti
                        Dim campConfTestataGrigliaProd = Gias_EF_Utility.CopyEntity(GiasContext, Elem_Prodotti, Nothing, username, data)
                        campConfTestataGrigliaProd.Username_Creazione = username
                        campConfTestataGrigliaProd.Username_Modifica = username
                        campConfTestataGrigliaProd.Data_Creazione = data
                        campConfTestataGrigliaProd.Data_Modifica = data

                        'objFF_CampionamentoConferimento_W.Scrivi_TestataGriglia_Prodotti(Piva, campConfTestataGriglia.Id_TestataGriglia, Copy, objParametri_Server)


                        'Richiedo un nuovo id sequenza
                        idSeq = ObjSequenze.NuovoId_Tabella_EF(GiasContext, "campionamentoconferito_testatagriglia_prodotti", 0, 2000000000, objParametri_Server)
                        campConfTestataGrigliaProd.Id_TestataGriglia_Prod = idSeq
                        campConfTestataGrigliaProd.PIVA = Piva
                        campConfTestataGrigliaProd.Id_TestataGriglia = campConfTestataGriglia.Id_TestataGriglia

                        'Controllo che non ci siano doppioni
                        MessaggioErrore = campConf_R.Controlla_Sovrapposizione_Riga_Prodotti(campConfTestataGrigliaProd, objParametri_Server, GiasContext)
                        If MessaggioErrore <> "" Then
                            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
                        End If

                        GiasContext.CampionamentoConferito_TestataGriglia_Prodotti.Add(campConfTestataGrigliaProd)
                        GiasContext.SaveChanges()

                    Next

                    scope.Complete()

                End Using
            End Using

        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try

        Return "Copia Effettuata correttamente"

    End Function

End Class

Public Class MediaCampionato
    Public Property Id_Calibro As String
    Public Property PercentualeCampionato As Decimal
End Class
