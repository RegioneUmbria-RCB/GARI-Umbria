Imports AgronicaCoreDataProvider

Public Class AlgoritmiLavorazioni : Implements IAlgoritmoEsterno

    Private ReadOnly _parametri As ParametriAlgoritmo
    Private ReadOnly _paramPrefissoLotto As String
    Private ReadOnly _separatore As String

    Public Sub New()
        _paramPrefissoLotto = "PrefissoLotto"
        _separatore = "-"
    End Sub

    Public Sub New(ByVal parametri As ParametriAlgoritmo)
        Me.New()
        _parametri = parametri
    End Sub

    Public Function PrefissoAnnoGiornoGiulianoTipoLav() As RispostaAlgoritmo

        Dim rispostaAlg As RispostaAlgoritmo = New RispostaAlgoritmo
        Dim lotto As String = Nothing
        Dim paramServer As New AgronicaCoreParametri
        Dim lavorazione As New Lavorazione
        Dim prefissoLotto As String = String.Empty

        Try

            If IsNothing(_parametri) OrElse IsNothing(_parametri.Parametri) OrElse Not _parametri.Parametri.Any Then
                rispostaAlg.RisultatoErrore = "Parametri funzione non indicati"
                Return rispostaAlg
            End If

            paramServer = _parametri.Parametri(0)
            lavorazione = _parametri.Parametri(1)

            If lavorazione.Proprieta_Estese.ContainsKey(_paramPrefissoLotto) Then
                prefissoLotto = lavorazione.Proprieta_Estese(_paramPrefissoLotto)
            End If

            If IsNothing(prefissoLotto) Or prefissoLotto = String.Empty Then
                Throw New Exception("Prefisso lotto non indicato")
            End If
            If IsNothing(lavorazione.DataMovimento) Then
                Throw New Exception("Data movimento non indicata")
            End If
            If IsNothing(lavorazione.PreparazioneCod) Then
                Throw New Exception("Codice preparazione non indicato")
            End If

            Dim data As DateTime = lavorazione.DataMovimento

            Dim objLeggiLineePrep = New AgronicaCoreContabDAL.Linee_Preparazioni_R

            Dim dtLeggiLineePrep As DataTable

            dtLeggiLineePrep = objLeggiLineePrep.Leggi(lavorazione.Piva,
                                                       lavorazione.PreparazioneCod,
                                                       Modulo_Generazione:=0,
                                                       Codice_Generazione:=0,
                                                       xFiltroAggiuntivo:=String.Empty,
                                                       xOrderBy:=String.Empty,
                                                       paramServer)

            Dim tipoLav As String = dtLeggiLineePrep.Rows(0).Item("preparazione_sigla")

            lotto = prefissoLotto & _separatore & data.ToString("yy") & data.DayOfYear.ToString() & _separatore & tipoLav.ToUpper()

            rispostaAlg.Risultato = lotto
            rispostaAlg.RisultatoOK = True

        Catch ex As Exception
            rispostaAlg.RisultatoErrore = ex.Message
        End Try

        Return rispostaAlg

    End Function

    Public Function PrefissoAnnoGiornoGiuliano() As RispostaAlgoritmo

        Dim rispostaAlg As RispostaAlgoritmo = New RispostaAlgoritmo
        Dim lotto As String = Nothing
        Dim paramServer As New AgronicaCoreParametri
        Dim lavorazione As New Lavorazione
        Dim prefissoLotto As String = String.Empty

        Try

            If IsNothing(_parametri) OrElse IsNothing(_parametri.Parametri) OrElse Not _parametri.Parametri.Any Then
                rispostaAlg.RisultatoErrore = "Parametri funzione non indicati"
                Return rispostaAlg
            End If

            paramServer = _parametri.Parametri(0)
            lavorazione = _parametri.Parametri(1)

            If lavorazione.Proprieta_Estese.ContainsKey(_paramPrefissoLotto) Then
                prefissoLotto = lavorazione.Proprieta_Estese(_paramPrefissoLotto)
            End If

            If IsNothing(prefissoLotto) Or prefissoLotto = String.Empty Then
                Throw New Exception("Prefisso lotto non indicato")
            End If
            If IsNothing(lavorazione.DataMovimento) Then
                Throw New Exception("Data movimento non indicata")
            End If

            Dim data As DateTime = lavorazione.DataMovimento

            lotto = prefissoLotto + _separatore + data.ToString("yy") + data.DayOfYear.ToString()

            rispostaAlg.Risultato = lotto
            rispostaAlg.RisultatoOK = True

        Catch ex As Exception
            rispostaAlg.RisultatoErrore = ex.Message
        End Try

        Return rispostaAlg

    End Function

End Class
