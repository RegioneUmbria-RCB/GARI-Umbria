Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModelsSTD.exceptions

Public Class Conversioni

    '################################################################################
    Public Shared Function IdCodCliente_fromCodiceGIAS(ByVal Codice_Gias As enum_CodiceGIAS_Clienti) As Integer

        Dim Id_Cod_Cliente As enum_CodiceAnagrafe_Clienti

        Select Case Codice_Gias

            Case enum_CodiceGIAS_Clienti.Apofruit
                Id_Cod_Cliente = enum_CodiceAnagrafe_Clienti.Apofruit

            Case enum_CodiceGIAS_Clienti.GranfruttaZani
                Id_Cod_Cliente = enum_CodiceAnagrafe_Clienti.GranfruttaZani

            Case enum_CodiceGIAS_Clienti.Valdoca
                Id_Cod_Cliente = enum_CodiceAnagrafe_Clienti.Valdoca

            Case enum_CodiceGIAS_Clienti.FruitModena
                Id_Cod_Cliente = enum_CodiceAnagrafe_Clienti.FruitModena

            Case enum_CodiceGIAS_Clienti.Italfrutta
                Id_Cod_Cliente = enum_CodiceAnagrafe_Clienti.Italfrutta

            Case enum_CodiceGIAS_Clienti.Ruggeri
                Id_Cod_Cliente = enum_CodiceAnagrafe_Clienti.Ruggeri

            Case enum_CodiceGIAS_Clienti.ApoConerpo,
                enum_CodiceGIAS_Clienti.OrtofruttaGrosseto
                Id_Cod_Cliente = enum_CodiceAnagrafe_Clienti.ApoConerpo

            Case enum_CodiceGIAS_Clienti.CoFruTa
                Id_Cod_Cliente = enum_CodiceAnagrafe_Clienti.CoFruTa

            Case enum_CodiceGIAS_Clienti.Costea
                Id_Cod_Cliente = enum_CodiceAnagrafe_Clienti.Costea

            Case Else
                Id_Cod_Cliente = 0

        End Select

        Return Id_Cod_Cliente

    End Function


    '################################################################################
    Public Shared Function Ettari_from_EttariAreCentiare(ByVal Ettari As Decimal,
                                                         ByVal Are As Decimal,
                                                         ByVal Centiare As Decimal
                                                         ) As Decimal

        Dim supHa As Decimal

        'Calcolo 
        supHa = CInt(Ettari) + (Are / 100) + (Centiare / 10000)

        Return supHa
    End Function


    '################################################################################
    Public Shared Sub EttariAreCentiare_from_Ettari(ByVal EttariAreCentiare As Decimal,
                                                    ByRef Ettari As Decimal,
                                                    ByRef Are As Decimal,
                                                    ByRef Centiare As Decimal)

        Dim ettariAreCentiareLocal As Decimal

        Dim supAreCentiare As Decimal
        Dim supCentiare As Decimal

        If EttariAreCentiare < 0 Then
            ettariAreCentiareLocal = (-1) * EttariAreCentiare
        Else
            ettariAreCentiareLocal = EttariAreCentiare
        End If

        Ettari = Int(ettariAreCentiareLocal)
        supAreCentiare = ettariAreCentiareLocal - Ettari + 0.00001D
        Are = Int(supAreCentiare * 100)
        supCentiare = supAreCentiare * 100 - Are
        Centiare = Int(supCentiare * 100)

        If EttariAreCentiare < 0 Then
            Ettari = (-1) * Ettari
        End If

    End Sub


    '################################################################################
    Public Shared Function Sup_Utilizzata_Data(
                                              ByVal Data As Date,
                                              ByVal Dr() As DataRow,
                                              Optional UsaDateAppezzamento As Boolean = True
                                              ) As Decimal

        Dim Sup_Utilizzata_Tot As Decimal = 0

        If UsaDateAppezzamento Then
            Sup_Utilizzata_Tot = CustomDecimalFieldAtDate(Data, Dr, "area", "validita_inizio", "validita_fine")
        Else
            Sup_Utilizzata_Tot = CustomDecimalFieldAtDate(Data, Dr, "area", "ValiditaInizio_SupDisponibile", "ValiditaFine_SupDisponibile")
        End If

        Return Sup_Utilizzata_Tot
    End Function

    ''' <summary>
    ''' Returns the <paramref name="fieldName"/> sum at a given <paramref name="validityDate"/> from the <paramref name="drs"/> collection
    ''' </summary>
    ''' <param name="validityDate"></param>
    ''' <param name="drs"></param>
    ''' <param name="fieldName"></param>
    ''' <param name="validityStartName"></param>
    ''' <param name="validityEndName"></param>
    ''' <returns></returns>
    Public Shared Function CustomDecimalFieldAtDate(ByVal validityDate As Date,
                                                    ByVal drs() As DataRow,
                                                    ByVal fieldName As String,
                                                    ByVal validityStartName As String,
                                                    ByVal validityEndName As String
                                                    ) As Decimal

        Dim fieldAddendum As Decimal = 0
        Dim filedSum As Decimal = 0

        If fieldName <> "" AndAlso validityStartName <> "" AndAlso validityEndName <> "" Then
            For n As Integer = 0 To drs.Length - 1
                If validityDate >= CDate(drs(n).Item(validityStartName)) AndAlso validityDate <= CDate(drs(n).Item(validityEndName)) Then
                    fieldAddendum = CDec(drs(n).Item(fieldName))
                    filedSum += fieldAddendum
                End If
            Next
        Else
            Throw New GiasException("No valid name provided")
        End If

        Return filedSum
    End Function


    '########################################################################################
    Public Shared Function DataGGMMYYYY_From_DataYYYYMMGG(ByVal Data As String) As String

        Dim Giorno As String
        Dim Mese As String
        Dim Anno As String

        Giorno = Mid(Data, 7, 2)
        Mese = Mid(Data, 5, 2)
        Anno = Mid(Data, 1, 4)

        Return Giorno & "/" & Mese & "/" & Anno

    End Function

    Public Shared Function DateTime_To_DataintYYYYMMGG(ByVal Data As DateTime) As String

        Return Year(Data) & CStr(Month(Data)).PadLeft(2, "0"c) & CStr(Day(Data)).PadLeft(2, "0"c)

    End Function

    '===============================================================================
    '------------------------ CONVERSIONI BY METAL MAGA ----------------------------
    '===============================================================================

    Public Shared Function BinarioFromDecimale(ByVal Decimale As Long) As String
        Dim Dec As Long
        Dim Bin As String
        Dec = Decimale
        Bin = ""
        Do While Dec > 1
            Bin = Trim$(Dec Mod 2) & Bin
            Dec = Int(Dec / 2)
        Loop
        If Decimale > 1 Then
            Bin = "1" & Bin
        End If
        Return Bin

    End Function



    Public Shared Function DecimaleFromBinario(ByVal Binario As String) As Long
        Dim dec As Long

        Dim lunghezza As Integer
        lunghezza = Len(Trim(Binario))
        If lunghezza = 0 Then Exit Function
        For i As Integer = 0 To lunghezza - 1
            dec = dec + Val(Mid(Binario, lunghezza - i, 1)) * 2 ^ i
        Next
        Return Trim$(dec)

    End Function

    Public Shared Function Data_from_DataGiornoGiulianoJDE(ByVal DataGG As Integer) As Date
        'function aaaggg_aaaammgg( int aaa, int ggg ) {

        '==> Tutte le date in formato JDE  sono sostanzialmente delle date giuliane, e l'algoritmo è il seguente:
        'es.oggi 11 / 2 / 2020 = 42° giorno dell'anno 2020 = 120042
        'quindi le prime 3 cifre identificano l'anno e le seconde 3 rappresentano il progressivo da 001 a 366 che identifica il giorno all'interno dell'anno.
        'Questo campo NON contiene l'ora, che è memorizzata invece nel campo ABUPMT nel formato hhmmss.

        Dim DataOK As Date
        Dim AnnoGG, Anno, GiornoGG As Integer

        'quindi le prime 3 cifre identificano l'anno
        AnnoGG = CInt(Left(CStr(DataGG), 3))
        Anno = 1900 + AnnoGG

        'le seconde 3 rappresentano il progressivo da 001 a 366 che identifica il giorno all'interno dell'anno
        GiornoGG = CInt(Right(CStr(DataGG), 3))

        Dim mesi As Integer() = {31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31} '   // i giorni nei 12 mesi. l array parte da 0 ( gennaio )

        'se aaaa divisibile per 4 ( aaaa % 4 == 0 ) allora mesi[1] = 29    // febbraio bisestile
        If Anno Mod 4 = 0 Then
            mesi(1) = 29
            ''se aaaa non divisibile per 400 ( aaaa % 400 != 0 ) allora mesi[1] = 28 // solo i secolari divisibili per 400 sono bisestili
            'If Anno Mod 400 <> 0 Then
            '    mesi(1) = 28
            'End If
        End If

        Dim MM, GG As Integer

        'ciclo(m = 0 ; m < 12 ; m++ ) {
        '        ggg = ggg - mesi[m]     // sottrae da ggg ( l ordinale del giorno In input ) il numero di giorni dei mesi
        '        se ggg <= 0 {           // se ho sottratto troppi mesi
        '                gg = ggg + mesi[m]      // gli risommo i giorni del mese corrente
        '                mm = m // i mesi contano da zero, ma sono andato sotto zero con i giorni, quindi si compensa
        '        }
        '}

        'sottrai i mesi finché non vai sotto zero e fermi il ciclo, poi risommi i giorni dell ultimo mese
        'es 68 
        '68 - 31 = 37    m=0
        '37 - 29 = 8      m=1
        '8 - 31 = -23     m=2 e stop del ciclo 
        '-23 + 31 = 8   m=2 si si deve sommare 1, m=3
        For m = 0 To 11
            GiornoGG = GiornoGG - mesi(m)     '// sottrae da ggg ( l ordinale del giorno In input ) il numero di giorni dei mesi
            If GiornoGG <= 0 Then  '// se ho sottratto troppi mesi
                'gg = ggg + mesi[m]      // gli risommo i giorni del mese corrente
                GG = GiornoGG + mesi(m)
                MM = m + 1 '// i mesi contano da zero, quindi devo aggiungere 1
                Exit For
            End If
        Next
        'per farlo migliore: invece di andare sotto zero e poi risommare, fermare il ciclo se il numero residuo di giorni e' <= a quello del mese successivo ( mesi[m+1] )


        DataOK = CDate(Right("0" & CStr(GG), 2) & "/" & Right("0" & CStr(MM), 2) & "/" & CStr(Anno))

        Return DataOK



    End Function

    Public Shared Function DataGiornoGiulianoJDE_from_Data(ByVal Data As Date) As Integer

        Dim DataGG As Integer = 0

        Dim Anno As Integer = Data.Year
        Dim AnnoGG As String = Right("000" & CStr(Anno - 1900), 3)
        Dim GiornoGG As String = Right("000" & CStr(Data.DayOfYear), 3)

        DataGG = CInt(AnnoGG & GiornoGG)

        Return DataGG

    End Function



    '=============================================================================================
    '=============================================================================================

End Class
