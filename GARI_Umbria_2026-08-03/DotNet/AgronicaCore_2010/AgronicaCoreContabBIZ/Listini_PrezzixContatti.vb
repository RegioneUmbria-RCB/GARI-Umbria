Imports AgronicaCoreContabDAL
Imports AgronicaCoreDataProvider

Public Class Listini_PrezzixContatti
    Public Function InsertMultipleAssociazioniListini(
            ByVal piva As String, ByVal arrListinoCod As Integer(), ByVal arrSaCod As Integer(), 
            ByVal arrCodRapporto As Integer(), ByVal arrCodContatto As String(), 
            ByVal arrCodContattoProduttore As String(), ByVal charContattiRapporti As Char, 
            ByVal flagProduttori As Boolean, ByRef objParametri As AgronicaCoreParametri
        ) As List(Of String)

        Dim listErrori As New List(Of String)

        Dim handleAssociaListiniW As New Listini_PrezzixContatti_W()
        
        If arrListinoCod.Length = 0 Then
            listErrori.Add("Selezionare almeno un listino da associare")
            Return listErrori
        End If

        'L'istruzione ReDim con l'argomento a zero, reimposta l'array ad un solo elemento col valore di default del suo tipo
        Select Case charContattiRapporti
            Case "c"
                If arrCodContatto.Length = 0 Then
                    listErrori.Add("Selezionare almeno un contatto")
                    Return listErrori
                End If

                If flagProduttori = True Then

                    If arrCodContattoProduttore.Length > 0 And arrCodContatto.Length > 1 Then
                        listErrori.Add("Essendo selezionati dei produttori, è necessario selezionare un solo fornitore")
                        Return listErrori
                    Else
                        If arrCodContattoProduttore.Length = 0 Then
                            ReDim arrCodContattoProduttore(0)
                            arrCodContattoProduttore(0) = ""
                        End If
                    End If

                Else

                    ReDim arrCodContattoProduttore(0)
                    arrCodContattoProduttore(0) = ""
                End If
                
                ReDim arrCodRapporto(0)
                arrCodRapporto(0) = 0

            Case "r"
                if arrCodRapporto.Length = 0 Then
                    listErrori.Add("Selezionare almeno un rapporto contabile")
                    Return listErrori
                End If
                
                ReDim arrCodContatto(0)
                arrCodContatto(0) = ""

                ReDim arrCodContattoProduttore(0)
                arrCodContattoProduttore(0) = ""

            Case Else
                ReDim arrCodRapporto(0)
                arrCodRapporto(0) = 0
                
                ReDim arrCodContatto(0)
                arrCodContatto(0) = ""

                ReDim arrCodContattoProduttore(0)
                arrCodContattoProduttore(0) = ""
        End Select

        If arrSaCod.Length = 0 And 
            (arrCodRapporto.Length = 0 Or (arrCodRapporto.Length = 1 AndAlso arrCodRapporto(0) = 0) ) And 
            (arrCodContatto.Length = 0 Or (arrCodContatto.Length = 1 AndAlso arrCodContatto(0) = "") ) Then

            listErrori.Add("Selezionare almeno uno fra i centri aziendali, rapporti contabili e contatti")
            Return listErrori
        End If

        'Ho completato i controlli

        If arrSaCod.Length = 0 Then
            ReDim arrSaCod(0)
            arrSaCod(0) = 0
        End If

        Dim FlagConnessioneLocale As Boolean = False
        Dim FlagTransazioneLocale As Boolean = False

        Try
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Apro la connessione al DB
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                            FlagTransazioneLocale,
                                                                            objParametri)

            For Each listinoCod As Integer In arrListinoCod
                For Each saCod As Integer In arrSaCod
                    For Each rapportoContab As Integer In arrCodRapporto
                        For Each codContatto As String In arrCodContatto
                            For Each codContattoProduttore As String In arrCodContattoProduttore
                                'Try
                                    handleAssociaListiniW.Insert(piva, listinoCod, saCod, rapportoContab, codContatto, codContattoProduttore, objParametri)

                                'Catch ex As Exception
                                '    Dim flagChiaveDuplicata As Boolean = False

                                '    Select Case DataProviderFactory.Instance.TipoProvider()
                                '        Case TipiEnumerativi.enum_DataProvidersType.OleDbProvider
                                '            For Each oledbproviderError As OleDb.OleDbError In DirectCast(ex, OleDb.OleDbException).Errors
                                '                If oledbproviderError.NativeError = 2627 Then
                                '                    flagChiaveDuplicata = True
                                '                    Exit For
                                '                End If
                                '            Next

                                '        Case TipiEnumerativi.enum_DataProvidersType.SqlDataProvider
                                '            For Each sqldataproviderError As SqlClient.SqlError In DirectCast(ex, SqlClient.SqlException).Errors
                                '                if sqldataproviderError.Number = 2627 Then
                                '                    flagChiaveDuplicata = True
                                '                    Exit For
                                '                End If
                                '            Next
                                '    End Select

                                '    'L'eccezione della chiave duplicata la ignoro, non è un errore imprevisto
                                '    If flagChiaveDuplicata = False Then
                                '        Throw ex
                                '    End If
                                'End Try

                            Next
                        Next
                    Next
                Next
            Next
            
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Chiudo la transazione
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        Catch ex As Exception
            'Faccio il rollback della transazione
            If Not objParametri.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            listErrori.Add(Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True))

        Finally
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Chiudo la transazione e la connessione al DB
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagTransazioneLocale, objParametri)
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        End Try

        Return listErrori
    End Function

    Public Function DeleteMultipleAssociazioniListini(ByVal piva As String, ByVal arrAssocDaCanc As Object(), ByRef objParametri As AgronicaCoreParametri) As List(Of String)
        Dim listErrori As New List(Of String)

        Dim FlagConnessioneLocale As Boolean = False
        Dim FlagTransazioneLocale As Boolean = False
        Try
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Apro la connessione al DB
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                            FlagTransazioneLocale,
                                                                            objParametri)

            Dim handleAssociaListiniW As New Listini_PrezzixContatti_W()
            For Each assocDaCanc As Dictionary(Of String, Object) In arrAssocDaCanc
                Dim erroriCanc = handleAssociaListiniW.Delete(
                    assocDaCanc("Piva"),
                    assocDaCanc("Listino_Cod"),
                    assocDaCanc("Sa_Cod"),
                    assocDaCanc("Cod_Rapporto"),
                    assocDaCanc("Cod_Contatto"),
                    assocDaCanc("Cod_Contatto_Produttore"),
                    objParametri
                )
            Next

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Chiudo la transazione
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        Catch ex As Exception
            'Faccio il rollback della transazione
            If Not objParametri.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            listErrori.Add(Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True))
        Finally
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Chiudo la transazione e la connessione al DB
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagTransazioneLocale, objParametri)
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        End Try

        Return listErrori
    End Function
    
End Class
