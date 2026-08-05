Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEFatturaBIZ

Public Class IndirizzoMapValidator : Inherits ValidatoreBase : Implements IFatturaValidator

    Public Sub New(ByVal objectType As Type,
                   ByVal riferimento As String,
                   ByVal soggetto As EnuValidatoreSoggetto,
                   ByVal mapper As IDecodificheMapper)
        MyBase.New(objectType, riferimento, soggetto, mapper)
    End Sub

    Public ReadOnly Property AllowNullObject As Boolean Implements IFatturaValidator.AllowNullObject
        Get
            Return False
        End Get
    End Property

    Public Function Validate(ByVal oggettoDaValidare As Object,
                             ByVal fattura As FatturaGias,
                             ByRef listaErrori As List(Of String)) As Boolean Implements IFatturaValidator.Validate

        Dim id_cf = 0
        Select Case _soggetto
            Case EnuValidatoreSoggetto.Cedente
                id_cf = fattura.Cedente.DatiPrincipali.Anagrafica.Id_CF
            Case EnuValidatoreSoggetto.Cessionario
                id_cf = fattura.Cessionario.DatiPrincipali.Anagrafica.Id_CF
            Case EnuValidatoreSoggetto.CessionarioDiverso
                id_cf = fattura.CessionarioDiverso.Anagrafica.Id_CF
        End Select

        Dim indirizzo = DirectCast(CTypeDynamic(oggettoDaValidare, _objectType), IndirizzoMap)

        If _riferimento = "StabileOrganizzazione" Then
            If oggettoDaValidare Is Nothing OrElse StabileOrganizzazioneVuota(indirizzo) Then Return True
        End If

        If String.IsNullOrEmpty(indirizzo.ind_des) Then
            listaErrori.Add(ComponiMessaggio("Indirizzo non può essere vuoto"))
        End If

        'cap sempre obbligatorio anche se estero
        If String.IsNullOrEmpty(indirizzo.CAP) Then
            listaErrori.Add(ComponiMessaggio("CAP non può essere vuoto"))
        End If

        'lunghezza del cap la controllo solo se non è contatto estero
        If id_cf <> enum_Contatti_IdCf.ContattoEstero AndAlso indirizzo.CAP.Length <> 5 Then
            listaErrori.Add(ComponiMessaggio(String.Format("CAP non valido ({0})", indirizzo.CAP)))
        End If

        Dim prov = indirizzo.NormalizzaProvincia(_mapper)
        If id_cf <> enum_Contatti_IdCf.ContattoEstero Then
            If String.IsNullOrEmpty(prov) OrElse prov = "00" Then
                listaErrori.Add(MyBase.ComponiMessaggio("Provincia non può essere vuoto"))
            End If
        End If

        Dim comune = indirizzo.NormalizzaComune(id_cf, _mapper)
        If String.IsNullOrEmpty(comune) Then
            listaErrori.Add(MyBase.ComponiMessaggio("Comune non può essere vuoto"))
        End If
        If String.IsNullOrEmpty(indirizzo.NormalizzaStato(id_cf)) Then
            listaErrori.Add(MyBase.ComponiMessaggio("Nazione non può essere vuoto"))
        End If

        Return Not listaErrori.Any()


    End Function

    Private Function StabileOrganizzazioneVuota(ByVal indirizzo As IndirizzoMap) As Boolean

        If String.IsNullOrEmpty(indirizzo.CAP) AndAlso
                String.IsNullOrEmpty(indirizzo.ind_des) AndAlso
                String.IsNullOrEmpty(indirizzo.com_des) AndAlso
                (String.IsNullOrEmpty(indirizzo.pro_cod) OrElse indirizzo.pro_cod = "0" OrElse indirizzo.pro_cod = "00") Then
            Return True
        End If

        Return False

    End Function

End Class
