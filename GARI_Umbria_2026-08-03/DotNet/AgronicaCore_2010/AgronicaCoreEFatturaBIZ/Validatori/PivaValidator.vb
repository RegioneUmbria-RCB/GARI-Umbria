Imports AgronicaCoreEFatturaBIZ
Imports AgronicaCoreUtility.ItaliaAnagrafeFisco
Public Class PivaValidator : Inherits ValidatoreBase : Implements IFatturaValidator

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

    Public Function Validate(oggettoDaValidare As Object, fattura As FatturaGias, ByRef listaErrori As List(Of String)) As Boolean Implements IFatturaValidator.Validate

        Dim anagrafica = DirectCast(oggettoDaValidare, AnagraficaMap)
        Dim id_cf = anagrafica.Id_CF
        Dim piva = anagrafica.PIVA
        Dim pivaToCheck As Boolean = True

        If _soggetto = EnuValidatoreSoggetto.Cessionario Then
            Dim titolarePIVA = fattura.CessionarioTitolarePIVA()
            pivaToCheck = titolarePIVA
            If pivaToCheck Then piva = NormalizzaPIVA(piva, fattura.Cessionario.DatiPrincipali.Indirizzo.NormalizzaStato(id_cf))
        Else
            piva = NormalizzaPIVA(piva, fattura.Cedente.DatiPrincipali.Indirizzo.NormalizzaStato(id_cf))
        End If

        If pivaToCheck Then
            If Not IsPivaValida(piva) Then
                listaErrori.Add(ComponiMessaggio("PIVA non valida"))
                Return False
            End If

        End If

        Return True

    End Function
End Class
