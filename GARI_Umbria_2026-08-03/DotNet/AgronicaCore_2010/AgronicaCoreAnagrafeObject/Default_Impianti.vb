Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.DataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Default_Impianti

    Public Property PivaSuperUser As String

    Public Property Piva As String

    Public Property Veg_Cod As Integer

    Public Property Cul_Cod As Integer

    Public Property DataSemina As Date

    Public Property DataFioritura As Date

    Public Property DataRaccolta As Date

    Public Property Resa As Decimal

    Public Property NCiclo As Integer


    Public Sub New()
        PivaSuperUser = ""
        Piva = ""
        Veg_Cod = 0
        Cul_Cod = 0
        DataSemina = Nothing
        DataFioritura = Nothing
        DataRaccolta = Nothing
        Resa = 0.0
        NCiclo = 0
    End Sub

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Costruttore parametrizzato.
    ''' </summary>
    ''' <param name="pivaSuperUser_par"></param>
    ''' <param name="piva_par"></param>
    ''' <param name="veg_cod_par"></param>
    ''' <param name="cul_cod_par"></param>
    ''' <param name="dataSemina_par"></param>
    ''' <param name="dataFioritura_par"></param>
    ''' <param name="dataRaccolta_par"></param>
    ''' <param name="Resa_par"></param>
    ''' <param name="NCiclo_par"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	26/04/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Sub New(ByVal pivaSuperUser_par As String, ByVal piva_par As String, ByVal veg_cod_par As Integer,
                   ByVal cul_cod_par As Integer, ByVal dataSemina_par As Date, ByVal dataFioritura_par As Date,
                   ByVal dataRaccolta_par As Date, ByVal Resa_par As Decimal, ByVal NCiclo_par As Integer)

        PivaSuperUser = pivaSuperUser_par
        Piva = piva_par
        Veg_Cod = veg_cod_par
        Cul_Cod = cul_cod_par
        DataSemina = dataSemina_par
        DataFioritura = dataFioritura_par
        DataRaccolta = dataRaccolta_par
        Resa = Resa_par
        NCiclo = NCiclo_par

    End Sub

End Class


Public Class Default_Impianti_Helper

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Subroutine per il caricamento dei dati di default.
    ''' </summary>
    ''' <param name="objDefault"></param>
    ''' <param name="piva"></param>
    ''' <param name="Veg_Cod"></param>
    ''' <param name="Cul_Cod"></param>
    ''' <param name="validitaInizio"></param>
    ''' <param name="validitaFine"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	29/04/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub Carica_Default(ByRef objDefault As Default_Impianti,
                                     ByVal piva As String,
                                     ByVal Veg_Cod As Integer,
                                     ByVal Cul_Cod As Integer,
                                     ByVal validitaInizio As Date,
                                     ByVal validitaFine As Date,
                                     ByVal xFiltroAggiuntivo As String,
                                     ByVal xOrderBy As String,
                                     ByRef objParametri As AgronicaCoreParametri)

        Const nomeRoutine = "AgronicaCoreAnagrafeObject.Default_Impianti_Helper.Carica_Default()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim dt As DataTable
        Dim objDataProvider As New AgronicaCoreDataProvider.DataProvider
        Dim objDefaultDal As New AgronicaCoreAnagrafeDAL.SpecieVegetali_Default_R

        Dim hSemineCicli As Hashtable = Nothing
        Dim hFioritureCicli As Hashtable = Nothing
        Dim hRaccolteCicli As Hashtable = Nothing
        Dim hReseCicli As Hashtable = Nothing
        Dim hDistanzeCicli As Hashtable = Nothing

        Dim stringaSemina, stringaFioritura, stringaRaccolta As String
        Dim dataSemina, dataFioritura, dataRaccolta As Date

        Dim arrayTemp As Decimal() = Nothing

        Dim ciclo As Integer

        Try
            strSql.Length = 0
            strSql.Append(" SELECT PivaSuperUser, Piva, Veg_Cod, Cul_Cod, Codice, Valore, Numero_Ciclo")
            strSql.Append(" FROM  SpecieVegetali_Default ")
            strSql.Append(" WHERE SpecieVegetali_Default.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            strSql.Append(" AND (SpecieVegetali_Default.Piva = ''")
            If piva <> "" Then
                strSql.Append(" OR SpecieVegetali_Default.Piva = '" & Agro_SQL_SaveText(piva) & "'")
            End If
            strSql.Append(") ")
            strSql.Append(" AND SpecieVegetali_Default.Veg_Cod = " & Veg_Cod & " ")
            strSql.Append(" AND (SpecieVegetali_Default.Cul_Cod = " & Cul_Cod & " OR SpecieVegetali_Default.Cul_Cod = 0) ")
            strSql.Append(" AND Validita_Inizio <= " & Agro_SQL_SaveDate(validitaFine) & " ")
            strSql.Append(" AND Validita_Fine >= " & Agro_SQL_SaveDate(validitaInizio) & " ")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.Append(" AND   SpecieVegetali_Default.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.Append(" AND   SpecieVegetali_Default.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy))
            Else

                'Restituisco i risultati in ordine decrescente per partita iva e per varietà in modo che l'elemento 
                'con priorità maggiore sia in testa al vettore

                strSql.Append(" ORDER BY Piva DESC, Cul_Cod DESC, Numero_Ciclo")
            End If

            '--------------------------------------------------------------------------
            dt = objDataProvider.EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            'Costruisco 4 hashtable contenenti i dati con maggiore priorità per ogni ciclo colturale

            Costruisci_HashTables(dt, hSemineCicli, hFioritureCicli, hRaccolteCicli, hReseCicli)


            'In base alle date presenti nelle hashtable caricate precedentemente, calcolo le distanze dei vari
            'cicli colturali dalla data di inizio distinta (il vettore arrayTemp conterrà le distanze ordinate
            'in ordine crescente

            Calcola_Distanze(hSemineCicli, hFioritureCicli, hRaccolteCicli, hDistanzeCicli,
                             arrayTemp, validitaInizio, validitaFine, objParametri)

            'Cerco il ciclo colturale corrispondente alla distanza più breve

            For Each ciclo In hDistanzeCicli.Keys
                If hDistanzeCicli.Item(ciclo) = arrayTemp(0) Then
                    Exit For
                End If
            Next

            If hSemineCicli.Item(ciclo) IsNot Nothing Then
                stringaSemina = objDefaultDal.FormattaDefault(hSemineCicli.Item(ciclo),
                                                              validitaInizio,
                                                              validitaFine,
                                                              validitaInizio,
                                                              objParametri)
                dataSemina = CDate(stringaSemina)
            Else
                dataSemina = Nothing
            End If

            If hFioritureCicli.Item(ciclo) IsNot Nothing Then
                stringaFioritura = objDefaultDal.FormattaDefault(hFioritureCicli.Item(ciclo),
                                                                 validitaInizio,
                                                                 validitaFine,
                                                                 validitaInizio,
                                                                 objParametri)
                dataFioritura = CDate(stringaFioritura)
            Else
                dataFioritura = Nothing
            End If

            If hRaccolteCicli.Item(ciclo) IsNot Nothing Then
                stringaRaccolta = objDefaultDal.FormattaDefault(hRaccolteCicli.Item(ciclo),
                                                                validitaInizio,
                                                                validitaFine,
                                                                validitaInizio,
                                                                objParametri)
                dataRaccolta = CDate(stringaRaccolta)
            Else
                dataRaccolta = Nothing
            End If


            'Costruisco l'oggetto che conterrà i dati di default del ciclo colturale corrispondente alla
            'distinta
            objDefault = New Default_Impianti(objParametri.PivaSuperUser, piva, Veg_Cod, Cul_Cod,
                                              dataSemina, dataFioritura, dataRaccolta,
                                              hReseCicli.Item(ciclo), ciclo)

        Catch ex As Exception
            messaggioErrore = ex.Message
            objDataProvider.Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Subroutine che, data la tabella dei default, costruisce le 4 hashtable.
    ''' </summary>
    ''' <param name="Input_DT"></param>
    ''' <param name="Semine"></param>
    ''' <param name="Fioriture"></param>
    ''' <param name="Raccolte"></param>
    ''' <param name="Rese"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	29/04/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Shared Sub Costruisci_HashTables(ByVal Input_DT As DataTable,
                                             ByRef Semine As Hashtable,
                                             ByRef Fioriture As Hashtable,
                                             ByRef Raccolte As Hashtable,
                                             ByRef Rese As Hashtable)

        Dim i As Integer
        Dim tempPiva As String
        Dim tempCulCod As Integer
        Dim tempN_Ciclo As Integer

        tempPiva = "tempPiva"
        tempCulCod = -2
        tempN_Ciclo = 0

        'Inizializzo le hashtable
        Semine = New Hashtable
        Fioriture = New Hashtable
        Raccolte = New Hashtable
        Rese = New Hashtable

        For i = 0 To Input_DT.Rows.Count - 1

            If CStr(Input_DT.Rows(i).Item("Piva")) <> tempPiva OrElse
               CInt(Input_DT.Rows(i).Item("Cul_Cod")) <> tempCulCod OrElse
               CInt(Input_DT.Rows(i).Item("Numero_Ciclo")) <> tempN_Ciclo Then
                'Se un dato fra piva, varietà o numero ciclo cambia, cambia il default

                tempPiva = CStr(Input_DT.Rows(i).Item("Piva"))
                tempCulCod = CInt(Input_DT.Rows(i).Item("Cul_Cod"))
                tempN_Ciclo = CInt(Input_DT.Rows(i).Item("Numero_Ciclo"))

            End If

            'Inserisco il valore di default all'interno dell'opportuna hashtable coerentemente
            'in base al codice.
            Select Case (CInt(Input_DT.Rows(i).Item("Codice")))

                Case Enum_CodiciDefault.Semina

                    If Not Semine.ContainsKey(Input_DT.Rows(i).Item("Numero_Ciclo")) Then
                        Semine.Add(Input_DT.Rows(i).Item("Numero_Ciclo"), Input_DT.Rows(i).Item("Valore"))
                    End If

                Case Enum_CodiciDefault.Fioritura

                    If Not Fioriture.ContainsKey(Input_DT.Rows(i).Item("Numero_Ciclo")) Then
                        Fioriture.Add(Input_DT.Rows(i).Item("Numero_Ciclo"), Input_DT.Rows(i).Item("Valore"))
                    End If

                Case Enum_CodiciDefault.Raccolta

                    If Not Raccolte.ContainsKey(Input_DT.Rows(i).Item("Numero_Ciclo")) Then
                        Raccolte.Add(Input_DT.Rows(i).Item("Numero_Ciclo"), Input_DT.Rows(i).Item("Valore"))
                    End If

                Case Enum_CodiciDefault.Resa

                    If Not Rese.ContainsKey(Input_DT.Rows(i).Item("Numero_Ciclo")) Then
                        Rese.Add(Input_DT.Rows(i).Item("Numero_Ciclo"), Input_DT.Rows(i).Item("Valore"))
                    End If
            End Select

        Next

    End Sub


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Questa subroutine calcola le distanze dei cicli colturali dalla data di 
    ''' inizio distinta e le inserisce sia all'interno di una hashtable che
    ''' all'interno di un array ordinato in ordine crescente.
    ''' </summary>
    ''' <param name="Semine"></param>
    ''' <param name="Fioriture"></param>
    ''' <param name="Raccolte"></param>
    ''' <param name="Distanze"></param>
    ''' <param name="ArrayDistanze"></param>
    ''' <param name="Validita_Inizio_Distinta"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	29/04/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Shared Sub Calcola_Distanze(ByVal Semine As Hashtable,
                                        ByVal Fioriture As Hashtable,
                                        ByVal Raccolte As Hashtable,
                                        ByRef Distanze As Hashtable,
                                        ByRef ArrayDistanze() As Decimal,
                                        ByVal Validita_Inizio_Distinta As Date,
                                        ByVal Validita_Fine_Distinta As Date,
                                        ByRef objParametri As AgronicaCoreParametri)

        Dim i As Integer
        Dim key As Object
        Dim distanza As Decimal

        Dim objDefault As New AgronicaCoreAnagrafeDAL.SpecieVegetali_Default_R

        Distanze = New Hashtable

        'Calcolo le distanze delle date di semina prevista e memorizzo per ogni ciclo quella più vicina
        'alla data di inizio distinta.
        For Each key In Semine.Keys
            distanza = CDate(objDefault.FormattaDefault(Semine.Item(key),
                                                        Validita_Inizio_Distinta,
                                                        Validita_Fine_Distinta,
                                                        Validita_Inizio_Distinta,
                                                        objParametri)
                            ).Subtract(Validita_Inizio_Distinta).TotalDays
            If Not Distanze.ContainsKey(key) Then
                Distanze.Add(key, distanza)
            ElseIf distanza < CDbl(Distanze.Item(key)) Then
                Distanze.Remove(key)
                Distanze.Add(key, distanza)
            End If
        Next

        'Calcolo le distanze delle date di fioritura prevista e memorizzo per ogni ciclo quella più vicina
        'alla data di inizio distinta.
        For Each key In Fioriture.Keys
            Dim dataFioProfilata As String = objDefault.FormattaDefault(Fioriture.Item(key),
                                                                        Validita_Inizio_Distinta,
                                                                        Validita_Fine_Distinta,
                                                                        Validita_Inizio_Distinta,
                                                                        objParametri)
            'se '' allora probabile che nella riga profilazione ci sia gg/mm e non un valore 05/06
            'quindi salto
            If dataFioProfilata <> "" AndAlso IsDate(dataFioProfilata) Then
                distanza = CDate(dataFioProfilata).Subtract(Validita_Inizio_Distinta).TotalDays
                If Not Distanze.ContainsKey(key) Then
                    Distanze.Add(key, distanza)
                ElseIf distanza < CDbl(Distanze.Item(key)) Then
                    Distanze.Remove(key)
                    Distanze.Add(key, distanza)
                End If
            End If

        Next

        'Calcolo le distanze delle date di raccolta prevista e memorizzo per ogni ciclo quella più vicina
        'alla data di inizio distinta.
        For Each key In Raccolte.Keys
            distanza = CDate(objDefault.FormattaDefault(Raccolte.Item(key),
                                                        Validita_Inizio_Distinta,
                                                        Validita_Fine_Distinta,
                                                        Validita_Inizio_Distinta,
                                                        objParametri)
                            ).Subtract(Validita_Inizio_Distinta).TotalDays
            If Not Distanze.ContainsKey(key) Then
                Distanze.Add(key, distanza)
            ElseIf distanza < CDbl(Distanze.Item(key)) Then
                Distanze.Remove(key)
                Distanze.Add(key, distanza)
            End If
        Next

        'Inizializzo l'array
        ReDim ArrayDistanze(Distanze.Count - 1)

        'Inserisco le distanze nell'array
        i = 0
        For Each key In Distanze.Keys
            ArrayDistanze(i) = Distanze.Item(key)
            i += 1
        Next

        'Ordino l'array delle distanze
        Array.Sort(ArrayDistanze)

    End Sub

End Class
