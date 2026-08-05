Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEFatturaBIZ

Public Class AnagraficaValidator : Inherits ValidatoreBase : Implements IFatturaValidator

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

        Dim id_cf As Integer = 0
        Dim nome As String = ""
        Dim cognome As String = ""
        Dim denominazione As String = ""
        Dim isAssociazione As Boolean = False

        If _soggetto = EnuValidatoreSoggetto.Cedente Then
            Dim ana = DirectCast(oggettoDaValidare, DatiPrincipaliCedenteMap)
            id_cf = ana.Anagrafica.Id_CF
            nome = ana.Anagrafica.Nome
            cognome = ana.Anagrafica.Cognome
            denominazione = ana.Anagrafica.rag_soc
        Else
            Dim ana = DirectCast(oggettoDaValidare, DatiPrincipaliCessionarioMap)
            isAssociazione = ana.Anagrafica.Cod_Contatto.IsAssociazione()
            id_cf = ana.Anagrafica.Id_CF
            nome = ana.Anagrafica.Nome
            cognome = ana.Anagrafica.Cognome
            denominazione = ana.Anagrafica.rag_soc

            Dim cod_contatto = ana.Anagrafica.Cod_Contatto
            If String.IsNullOrEmpty(cod_contatto) OrElse (IsNumeric(cod_contatto) AndAlso ana.Anagrafica.Cod_Contatto < 0) Then
                listaErrori.Add(ComponiMessaggio(String.Format("Contatto generico con cod_contatto {0}", cod_contatto)))
            End If

        End If

        If id_cf = enum_Contatti_IdCf.PersonaFisica Then

            If isAssociazione Then

                If String.IsNullOrEmpty(denominazione) Then
                    denominazione = String.Concat(cognome, " ", nome)
                    If String.IsNullOrEmpty(denominazione) OrElse String.IsNullOrWhiteSpace(denominazione) Then
                        listaErrori.Add(ComponiMessaggio("Denominazione non può essere vuoto"))
                    End If
                End If
            Else
                If String.IsNullOrEmpty(nome) OrElse String.IsNullOrEmpty(cognome) Then
                    listaErrori.Add(ComponiMessaggio("Cognome / Nome non può essere vuoto"))
                End If
            End If
        Else
            If (String.IsNullOrEmpty(denominazione)) Then
                listaErrori.Add(ComponiMessaggio("Denominazione non può essere vuoto"))
            End If
        End If

        Return Not listaErrori.Any()

    End Function
End Class
