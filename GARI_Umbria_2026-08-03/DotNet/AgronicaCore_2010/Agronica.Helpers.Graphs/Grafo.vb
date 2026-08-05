Imports System.Data.Common
Imports System.Text
Imports AgronicaCoreDataProvider

Public Class grafo


    Private _ConnectionString As String = ""
    Dim m_bSolved As Boolean
    Dim daticaricati As Boolean = False

    Private _UsaLivelliInRicerca As Boolean = True

    Public Property UsaLivelliInRicerca() As Boolean
        Get
            Return _UsaLivelliInRicerca
        End Get
        Set(value As Boolean)
            _UsaLivelliInRicerca = value
        End Set
    End Property


    Public Property ConnectionString() As String
        Get
            Return _ConnectionString
        End Get
        Set(value As String)
            _ConnectionString = value
        End Set
    End Property


    Private _QueryArchi As String = ""
    Private _fattoLivello1 As Boolean = False

    ''' <summary>
    ''' query, dove come alias si imposta "select a as vertice1, b as vertice2 , c as peso from tabellaArchi"
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property QueryArchi() As String
        Get
            Return _QueryArchi
        End Get
        Set(value As String)
            _QueryArchi = value
        End Set
    End Property

    Private _QueryVertici As String = ""

    ''' <summary>
    ''' query, dove come alias si imposta "select a as id, b as descrizione from tabellaVertici"
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property QueryVertici() As String
        Get
            Return _QueryVertici
        End Get
        Set(value As String)
            _QueryVertici = value
        End Set
    End Property

    Private _GrafoOrientato As Boolean = False
    Public Property GrafoOrientato() As Boolean
        Get
            Return _GrafoOrientato
        End Get
        Set(value As Boolean)
            _GrafoOrientato = value
        End Set
    End Property

    'Private m_vPrimoVertice As Vertice
    'Private m_vSecondoVertice As Vertice
    Private m_lstArchiFinali As New List(Of Arco)
    Private m_lstArchiInizali As New List(Of Arco)
    Private m_lstVertici As New Hashtable


    Public Sub CaricaDati()

        m_lstVertici = New Hashtable

        Dim con As DbConnection = DataProviderFactory.Instance.CreaNuovaConnessione(_ConnectionString)
        Dim cmdVertici As DbCommand = DataProviderFactory.Instance.CreaCommand(_QueryVertici, con)
        Dim cmdArchi As DbCommand = DataProviderFactory.Instance.CreaCommand(_QueryArchi, con)



        Try
            con.Open()
            Using drVertici As DbDataReader = cmdVertici.ExecuteReader

                While drVertici.Read

                    Try
                        m_lstVertici.Add(drVertici.GetValue(0), New Vertice(drVertici.GetValue(0), drVertici.GetValue(1), drVertici.GetValue(2), drVertici.GetValue(3)))
                    Catch ex As Exception
                        'MsgBox(ex.Message)
                    End Try



                End While
            End Using

            Dim v1, v2 As Vertice
            Using drConnections As DbDataReader = cmdArchi.ExecuteReader

                While drConnections.Read

                    v1 = m_lstVertici(drConnections.GetValue(0))
                    v2 = m_lstVertici(drConnections.GetValue(1))

                    Try
                        v1.listaSuccessori.Add(v2.Name, New Arco(v1, v2, drConnections.GetValue(2)))

                        If Not _GrafoOrientato Then
                            If Not v2.listaSuccessori.ContainsKey(v1.Name) Then
                                v2.listaSuccessori.Add(v1.Name, New Arco(v2, v1, drConnections.GetValue(2)))
                            End If
                        End If

                    Catch ex As Exception
                        'MsgBox(ex.Message)
                    End Try



                End While
            End Using

            daticaricati = True
        Catch ex As Exception
            'MsgBox(ex.Message)
        Finally
            cmdVertici.Dispose()
            cmdArchi.Dispose()
            con.Close()
            con.Dispose()
        End Try

    End Sub

    Public Function CalcolaPercorsi(ByVal StartID As Integer, Optional ByVal FlagLogga As Boolean = False, Optional ByRef Log As String = "", Optional ByVal objParametri_Fitofarmaci As AgronicaCoreDataProvider.AgronicaCoreParametri = Nothing) As List(Of Vertice)
        Dim out As New List(Of Vertice)
        Dim DaElaborare As New Queue(Of Vertice)
        Dim start As Vertice
        Dim current As Vertice
        Dim cVicino As Arco
        Dim StartLevel As Integer

        Dim objAvGru As AgronicaCoreMetaSchemaDAL.GruppoAvversita_R

        If FlagLogga = True Then
            objAvGru = New AgronicaCoreMetaSchemaDAL.GruppoAvversita_R
        End If

        If Not daticaricati Then
            CaricaDati()
        End If


        Dim stb As New StringBuilder

        Try
            _fattoLivello1 = False
            start = m_lstVertici(StartID)
            If start.listaSuccessori.Count Then
                StartLevel = DammiLivelloPartenza(start.listaSuccessori)
                If FlagLogga = True Then
                    stb.Append("Livello partenza: " & StartLevel.ToString & " <br />")
                End If
            End If


            DaElaborare.Enqueue(start)
            While DaElaborare.Count <> 0
                current = DaElaborare.Dequeue
                out.Add(current)
                If FlagLogga = True Then
                    stb.Append("Nodo corrente: " & current.Name & " = " & objAvGru.AvGruDes_from_AvGruCod(current.Name, "", AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Fitofarmaci) & " <br />")
                End If
                For Each vicino As DictionaryEntry In current.listaSuccessori
                    cVicino = CType(vicino.Value, Arco)
                    If Not out.Contains(cVicino.V2) And Not DaElaborare.Contains(cVicino.V2) Then
                        If FlagLogga = True Then
                            stb.Append("Vicino (di livello: " & cVicino.Cost & " ) da elaborare: " & cVicino.V2.Name & " = " & objAvGru.AvGruDes_from_AvGruCod(cVicino.V2.Name, "", AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Fitofarmaci) & " <br />")
                        End If

                        If _UsaLivelliInRicerca Then

                            If cVicino.Cost > StartLevel Then
                                DaElaborare.Enqueue(cVicino.V2)
                            Else
                                SvuotaSuccessori(cVicino.V2, FlagLogga, stb, objParametri_Fitofarmaci)
                                DaElaborare.Enqueue(cVicino.V2)

                            End If

                        Else
                            DaElaborare.Enqueue(cVicino.V2)
                        End If

                    End If
                Next
                If FlagLogga = True Then
                    stb.Append("<br />")
                End If
            End While

        Catch ex As Exception
            'MsgBox(ex.Message)
        End Try

        Log = stb.ToString

        Return out

    End Function



    Private Sub SvuotaSuccessori(ByRef v As Vertice, Optional ByVal FlagLogga As Boolean = False, Optional ByRef stb As StringBuilder = Nothing, Optional ByVal objParametri_Fitofarmaci As AgronicaCoreDataProvider.AgronicaCoreParametri = Nothing)

        Dim current As Arco
        Dim toRemove As New List(Of Arco)

        Dim livello As Integer = DammiLivelloPartenza(v.listaSuccessori)


        If FlagLogga = True Then
            stb.Append("<br />")
        End If

        '10/10/2014 18.35 vanni, nuova realizzazione
        If v.LivelloDiStopDiscesa <> 1 Then
            _fattoLivello1 = (livello = v.LivelloDiStopDiscesa)
        End If


        For Each succ In v.listaSuccessori
            current = CType(succ.value, Arco)
            If current.Cost > livello Or _fattoLivello1 Then
                toRemove.Add(current)
            Else
                If FlagLogga = True Then
                    stb.Append("Considero i successori (Livello: " & current.Cost & "  < Livello Partenza: " & livello & "  ) = " & current.V1.Name & " --> " & current.V2.Name & " _fattoLivello1=" & _fattoLivello1.ToString & "<br />")
                End If
            End If

        Next

        For Each del In toRemove
            v.listaSuccessori.Remove(del.V2.Name)
        Next

        '10/10/2014 18.35 vanni, per retrocompatibilità
        If v.LivelloDiStopDiscesa = 1 Then
            _fattoLivello1 = (livello = 1)
        End If


        If FlagLogga = True Then
            stb.Append("<br /><br />")
        End If


    End Sub

    Private Function DammiLivelloPartenza(ByVal lst As Hashtable) As Integer
        Dim min As Integer = 99999
        Dim current As Integer

        For Each succ In lst
            current = CType(succ.value, Arco).Cost
            If min > current Then
                min = current
            End If

        Next

        Return min
    End Function

    'Public Function CalcolaPercorsi(ByVal StartID As Integer, ByVal EndID As Integer, ByVal a As String) As List(Of Arco)
    '    Dim startvertice As New Vertice(StartID, "", "")
    '    Dim Endvertice As New Vertice(EndID, "", "")

    '    Dim journey As New Journey(startvertice, Endvertice)
    '    journey.Calculate()



    'End Function

    ''' <summary>
    ''' applica l'algoritmo di kruskal per trovare il minimum cost spanning tree
    ''' </summary>    
    ''' <param name="nTotalCost"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ApplicaKruskal(ByRef nTotalCost As Integer) As List(Of Arco)

        'm_vPrimoVertice = m_lstNodi.Item(IDNodoIniziale)

        Arco.QuickSort(m_lstArchiInizali, 0, m_lstArchiInizali.Count - 1)
        Dim lstEdgesReturn As New List(Of Arco)(m_lstArchiInizali.Count)
        For Each ed As Arco In m_lstArchiInizali
            Dim vRoot1 As Vertice, vRoot2 As Vertice
            vRoot1 = ed.V1.GetRoot()
            vRoot2 = ed.V2.GetRoot()
            If vRoot1.Name <> vRoot2.Name Then
                nTotalCost += ed.Cost
                Vertice.Join(vRoot1, vRoot2)
                lstEdgesReturn.Add(New Arco(ed.V1, ed.V2, ed.Cost))
            End If
        Next
        Return lstEdgesReturn
    End Function


End Class
