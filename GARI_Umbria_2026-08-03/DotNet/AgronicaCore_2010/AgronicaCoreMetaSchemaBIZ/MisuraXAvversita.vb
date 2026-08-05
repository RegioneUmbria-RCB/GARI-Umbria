Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreMetaSchemaDAL
Imports Newtonsoft.Json.Linq

Public Class MisuraXAvversita_W
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


    Public Function Aggiorna_MisuraXAvversita(
            ByVal righeInserite As String,
            ByVal righeModificate As String,
            ByVal righeCancellate As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As String


        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim MessaggioErrore As String = String.Empty

        Dim NomeRoutine As String = "MisuraXAvversita_W.Aggiorna_MisuraXAvversita_W()"

        Try
            Dim campConf_R As New MisuraXAvversita_R

            Dim curMisuraXAvversita As New MisuraxAvversita

            Dim righeInseriteArray As JArray = JArray.Parse(righeInserite)
            Dim righeModificateArray As JArray = JArray.Parse(righeModificate)
            Dim righeCancellateArray As JArray = JArray.Parse(righeCancellate)
            Dim EFArrayToInsert As New ArrayList
            Dim EFArrayToUpdate As New ArrayList
            Dim EFArrayToDelete As New ArrayList
            For Each obj As JObject In righeInseriteArray
                curMisuraXAvversita = New MisuraxAvversita
                curMisuraXAvversita.Cod = 0
                curMisuraXAvversita.Av_Cod = obj("Av_Cod")
                curMisuraXAvversita.Veg_Cod = obj("Veg_Cod")
                curMisuraXAvversita.FF_Cod = obj("FF_Cod")
                curMisuraXAvversita.Ordine = obj("Ordine")
                curMisuraXAvversita.Udm_Cod = obj("Udm_Cod")
                curMisuraXAvversita.Fondamentale = obj("Fondamentale")

                If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
                    curMisuraXAvversita.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio"), Format, Provider)
                Else
                    curMisuraXAvversita.Validita_Inizio = ValiditaInizio
                End If
                If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
                    curMisuraXAvversita.Validita_Fine = Date.ParseExact(obj("Validita_Fine"), Format, Provider)
                Else
                    curMisuraXAvversita.Validita_Fine = ValiditaFine
                End If
                curMisuraXAvversita.Data_Creazione = Date.Now
                curMisuraXAvversita.Username_Creazione = objParametri.UsernameOperazione
                curMisuraXAvversita.Data_Modifica = Date.Now
                curMisuraXAvversita.Username_Modifica = objParametri.UsernameOperazione
                curMisuraXAvversita.inviato = 0

                EFArrayToInsert.Add(curMisuraXAvversita)
            Next
            For Each obj As JObject In righeModificateArray
                curMisuraXAvversita = campConf_R.Leggi(obj("COD"), objParametri)
                If curMisuraXAvversita Is Nothing Then
                    MessaggioErrore += "Riga da aggiornare " & obj("Validita_Inizio").ToString & " - " & obj("Validita_Fine").ToString & " non trovata"
                Else
                    curMisuraXAvversita.Av_Cod = obj("Av_Cod")
                    curMisuraXAvversita.Veg_Cod = obj("Veg_Cod")
                    curMisuraXAvversita.FF_Cod = obj("FF_Cod")
                    curMisuraXAvversita.Ordine = obj("Ordine")
                    curMisuraXAvversita.Udm_Cod = obj("Udm_Cod")
                    curMisuraXAvversita.Fondamentale = obj("Fondamentale")

                    If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
                        curMisuraXAvversita.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio"), Format, Provider)
                    End If
                    If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
                        curMisuraXAvversita.Validita_Fine = Date.ParseExact(obj("Validita_Fine"), Format, Provider)
                    End If
                    curMisuraXAvversita.Data_Modifica = Date.Now
                    curMisuraXAvversita.Username_Modifica = objParametri.UsernameOperazione
                    EFArrayToUpdate.Add(curMisuraXAvversita)

                End If
            Next
            For Each obj As JObject In righeCancellateArray
                curMisuraXAvversita = New MisuraxAvversita
                curMisuraXAvversita.Cod = obj("COD")
                EFArrayToDelete.Add(curMisuraXAvversita)
            Next

            If String.IsNullOrEmpty(MessaggioErrore) Then
                Dim campConf_W As New AgronicaCoreMetaSchemaDAL.MisuraXAvversita_W

                MessaggioErrore = campConf_W.Aggiorna_MisuraXAvversita(
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


End Class

Public Class MisuraXAvv_R

    Public Sub New()

    End Sub

    Public Function MisuraXAvversita(ByVal Input As MisuraXAvversita_input,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                            ByRef ObjParametri_Disciplinari As AgronicaCoreDataProvider.AgronicaCoreParametri) _
                                                As MisuraXAvversita_output

        Dim Output As New MisuraXAvversita_output
        Dim Elemento As MisuraXAvv
        Dim Elemento1 As MisuraXAvversita_Anagrafica

        Dim Dt As DataTable
        Dim DR() As DataRow
        Dim objCore As New AgronicaCoreMetaSchemaDAL.MisuraXAvversita_R

        If Not IsNothing(Input.Lingua_Cod) AndAlso Input.Lingua_Cod > 0 Then
            objParametri.Lingua_Cod = Input.Lingua_Cod
        End If


        Dt = objCore.Leggi_WS(Input.Veg_Cod,
                                    Input.Av_Cod,
                                    Input.Av_Gru,
                                    Input.Dpi_Cod,
                                    Input.Id_Rcdpi,
                                    Input.TipoTestata,
                                    Input.SoloVisibili,
                                    Input.strFiltro,
                                    Input.strOrdinamento,
                                    objParametri,
                                    ObjParametri_Disciplinari,
                                    Input.Personalizzate,
                                    Input.Piva_Superuser,
                                    Input.EstraiPersonalizzatePerAPP)

        Dim HashInseriti As New Hashtable

        For i = 0 To Dt.Rows.Count - 1

            If Not HashInseriti.ContainsKey(Dt.Rows(i).Item("Cod")) OrElse
                Input.EstraiPersonalizzatePerAPP = True Then

                Elemento = New MisuraXAvv

                Elemento.Cod = Dt.Rows(i).Item("Cod")
                Elemento.Veg_Cod = Dt.Rows(i).Item("Veg_Cod")

                Elemento.Av_Cod = Dt.Rows(i).Item("Av_Cod")
                Elemento.Av_Gru = Dt.Rows(i).Item("Av_Gru")

                Elemento.Udm_Cod = Dt.Rows(i).Item("Udm_Cod")
                Elemento.TipoControllo_Cod = Dt.Rows(i).Item("TipoControllo_Cod")

                Elemento.Av_Des_Vol = Dt.Rows(i).Item("Av_Des_Vol")
                Elemento.Av_Des_Lat = Dt.Rows(i).Item("Av_Des_Lat")
                Elemento.Av_Abbreviazione = Dt.Rows(i).Item("Abbreviazione")
                Elemento.Av_Gru_Des = Dt.Rows(i).Item("Av_Gru_Des")
                Elemento.Av_Gru_Des_Lat = Dt.Rows(i).Item("Av_Gru_Des_Lat")

                Elemento.Udm_Des = Dt.Rows(i).Item("Udm_Des")
                Elemento.Udm_Sim = Dt.Rows(i).Item("Udm_Sim")

                Elemento.Visibile = Dt.Rows(i).Item("Fondamentale")
                Elemento.FF_Cod = Dt.Rows(i).Item("Fondamentale")
                Elemento.Ordine = Dt.Rows(i).Item("Ordine")

                If Input.Dpi_Cod > 0 Then
                    If Not IsDBNull(Dt.Rows(i).Item("si_cod")) Then
                        Elemento.Soglia = 1
                    End If
                End If

                If Input.EstraiPersonalizzatePerAPP = True Then
                    If Not IsDBNull(Dt.Rows(i).Item("Piva_superUser")) Then
                        Elemento.Pivasuperuser = Dt.Rows(i).Item("Piva_superUser")
                    End If
                End If

                Elemento.ListaMisureXAvversita_Anagrafiche = New List(Of MisuraXAvversita_Anagrafica)

                DR = Dt.Select("COD=" & Elemento.Cod)

                If Not DR Is Nothing Then
                    For j = 0 To DR.Length - 1
                        If Not IsDBNull(DR(j)("Anag_DES")) Then
                            Elemento1 = New MisuraXAvversita_Anagrafica
                            Elemento1.Cod = Elemento.Cod
                            Elemento1.Descrizione = DR(j).Item("Anag_DES")
                            Elemento1.Valore = DR(j).Item("Anag_Valore")
                            If Input.Personalizzate Then
                                Elemento1.PivaSuperUser = Elemento.Pivasuperuser
                            End If
                            Elemento.ListaMisureXAvversita_Anagrafiche.Add(Elemento1)
                        End If
                    Next
                End If

                Output.ListaMisureXAvversita.Add(Elemento)

                If Not HashInseriti.ContainsKey(Dt.Rows(i).Item("Cod")) Then
                    HashInseriti.Add(Dt.Rows(i).Item("Cod"), "")
                End If
            End If

        Next

        Return Output

    End Function


End Class

Public Class MisuraXAvversita_input

    Public Veg_Cod As Integer
    Public Av_Cod As Integer
    Public Av_Gru As Integer

    Public Dpi_Cod As Integer
    Public Id_Rcdpi As Integer
    Public Dpi_Pubblico_Privato As Integer
    Public TipoTestata As Integer

    Public SoloVisibili As Boolean
    Public strFiltro As String
    Public strOrdinamento As String

    Public Personalizzate As Boolean
    Public Piva_Superuser As String

    Public Lingua_Cod As Integer

    Public Url As String

    Public EstraiPersonalizzatePerAPP As Boolean

    Sub New()

        Veg_Cod = 0
        Av_Cod = 0
        Av_Gru = 0
        Dpi_Cod = 0
        Id_Rcdpi = 0
        Dpi_Pubblico_Privato = 0
        TipoTestata = 0
        SoloVisibili = True
        strFiltro = ""
        strOrdinamento = ""
        Personalizzate = False
        Piva_Superuser = ""
        Lingua_Cod = 0
        Url = ""
        EstraiPersonalizzatePerAPP = False
    End Sub

End Class

Public Class MisuraXAvversita_output

    Public ListaMisureXAvversita As List(Of MisuraXAvv)

    Public MessaggioErrore As String

    Public Sub New()

        ListaMisureXAvversita = New List(Of MisuraXAvv)
        MessaggioErrore = ""

    End Sub

End Class

Public Class MisuraXAvv

    Public Cod As Integer
    Public Veg_Cod As Integer
    Public Av_Cod As Integer
    Public Av_Gru As Integer

    Public Udm_Cod As Integer
    Public TipoControllo_Cod As Integer

    Public Av_Des_Vol As String
    Public Av_Des_Lat As String
    Public Av_Abbreviazione As String
    Public Av_Gru_Des As String
    Public Av_Gru_Des_Lat As String

    Public Udm_Des As String
    Public Udm_Sim As String

    Public Visibile As Integer 'fondamentale

    Public FF_Cod As Integer
    Public Ordine As Integer

    Public Soglia As Integer

    Public Pivasuperuser As String

    Public ListaMisureXAvversita_Anagrafiche As List(Of MisuraXAvversita_Anagrafica)

    Sub New()

        Cod = 0
        Veg_Cod = 0
        Av_Cod = 0
        Av_Gru = 0
        Udm_Cod = 0
        TipoControllo_Cod = 0

        Av_Des_Vol = ""
        Av_Des_Lat = ""
        Av_Abbreviazione = ""
        Av_Gru_Des = ""
        Av_Gru_Des_Lat = ""

        Udm_Des = ""
        Udm_Sim = ""

        Visibile = 0
        FF_Cod = 0
        Ordine = 0

        Soglia = 0

        pivasuperuser = ""

        ListaMisureXAvversita_Anagrafiche = New List(Of MisuraXAvversita_Anagrafica)

    End Sub

End Class

Public Class MisuraXAvversita_Anagrafica

    Public Cod As Integer
    Public Descrizione As String
    Public Valore As Integer
    Public PivaSuperUser As String

    Sub New()

        Cod = 0
        Descrizione = ""
        Valore = 0
        PivaSuperUser = ""
    End Sub

End Class



