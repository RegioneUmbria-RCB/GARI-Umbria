Imports AgronicaCoreDataProvider
Imports AgronicaCoreModello
Imports AgronicaCoreModelsSTD.Gis

Public Class FiltroneImpiantiDbTemp

    Public Shared Sub OperazioneCorrente_EstraiDAL_FiltroImpianti(IDTestataTemp As Integer,
                                                                  ByVal dest As List(Of OperazioneAgenda_Temp.Movimento_Destinazione),
                                                                  ByRef listImp As List(Of String))

        Dim xDestImp As List(Of Impianto2010) = (
            From i1 In dest
            Select New Impianto2010 With {
                .Piva = i1.Piva,
                .Sa_Cod = i1.Sa_Cod,
                .Appezza = i1.Appezza,
                .Id_Reg = i1.Id_Destinazione
                }
            ).ToList

        OperazioneCorrente_EstraiDAL_FiltroImpianti(IDTestataTemp, xDestImp, listImp, Nothing)

    End Sub

    Public Shared Sub OperazioneCorrente_EstraiDAL_FiltroImpianti(ByVal IDTestataTemp As Integer,
                                                                  ByVal dest As List(Of Impianto2010),
                                                                  ByRef listImp As List(Of String),
                                                                  Optional ByVal objParametri As AgronicaCoreParametri = Nothing)

        Dim hs As New Hashtable

        Dim listImp1 As New List(Of String)

        Dim bUsername As Boolean = objParametri IsNot Nothing

        Dim contatoreDati As Integer = 1

        For Each iDest In dest

            Dim chiaveHash As String = "('" & iDest.Piva & "'," & iDest.Sa_Cod & "," & iDest.Appezza & "," & iDest.Id_Reg & "," & IDTestataTemp

            If objParametri IsNot Nothing Then
                chiaveHash &= ", '" & objParametri.UtenteUsername & "', '" & objParametri.UtenteUsername & "'"
            End If

            chiaveHash &= ")"

            If Not hs.ContainsKey(chiaveHash) Then

                hs.Add(chiaveHash, iDest)

                listImp1.Add(chiaveHash)

                If contatoreDati Mod 1000 = 0 Then
                    popolaConImpianti(listImp, listImp1, bUsername)
                    listImp1.Clear()
                End If

            Else

                'xdebug
                Dim idle As Integer = 0

            End If

            contatoreDati += 1

        Next

        If dest.Count Mod 1000 <> 0 Then
            popolaConImpianti(listImp, listImp1, bUsername)
        End If

    End Sub

    Public Shared Sub OperazioneCorrente_EstraiDAL_FiltroMovimenti(ByVal IDTestataTemp As Integer,
                                                                   ByVal dest As List(Of OperazioneAgenda_Temp.Movimento),
                                                                   ByRef listMov As List(Of String),
                                                                   Optional ByVal objParametri As AgronicaCoreParametri = Nothing)

        Dim hs As New Hashtable

        Dim listMov1 As New List(Of String)

        Dim bUsername As Boolean = objParametri IsNot Nothing

        Dim contatoreDati As Integer = 1

        For Each iDest In dest

            Dim chiaveHash As String = "('" & IDTestataTemp & "', '" & iDest.Piva & "' ," & iDest.Id_Agenda & ", 0)"

            If Not hs.ContainsKey(chiaveHash) Then

                hs.Add(chiaveHash, iDest)

                listMov1.Add(chiaveHash)


                If contatoreDati Mod 1000 = 0 Then
                    popolaConMovimenti(listMov, listMov1, bUsername)
                    listMov1.Clear()
                End If

            Else

                'xdebug
                Dim idle As Integer = 0

            End If

            contatoreDati += 1

        Next

        If dest.Count Mod 1000 <> 0 Then
            popolaConMovimenti(listMov, listMov1, bUsername)
        End If

    End Sub

    Private Shared Sub popolaConImpianti(listImp As List(Of String),
                                         listImp1 As List(Of String),
                                         ByVal bUsername As Boolean)

        Dim sInsertStmt2 As New StringBuilder

        Dim line As String = " insert into __tmp_FiltroImpianti (piva, sa_cod, appezza, id_reg, IDTestataTemp"

        If bUsername Then
            line &= ", Username_Creazione, Username_Modifica"
        End If

        line &= ") values "

        sInsertStmt2.AppendLine(line)

        sInsertStmt2.Append(String.Join(",", listImp1))

        listImp.Add(sInsertStmt2.ToString)

    End Sub

    Private Shared Sub popolaConMovimenti(listMov As List(Of String),
                                          listMov1 As List(Of String),
                                          ByVal bUsername As Boolean)

        Dim sInsertStmt2 As New StringBuilder

        Dim line As String = " insert into __tmp_Agenda (IDTestataTemp, piva, id_agenda, lav_cod) values "

        sInsertStmt2.AppendLine(line)

        sInsertStmt2.Append(String.Join(",", listMov1))

        listMov.Add(sInsertStmt2.ToString)

    End Sub

    Public Shared Sub OperazioneCorrente_EstraiFiltrone(ByVal IDTestataTemp As Integer,
                                                        ByVal Impianti As List(Of Impianto2010),
                                                        ByRef OperazioneCorrente_FiltroImpianti As String,
                                                        Optional ByVal objParametri As AgronicaCoreParametri = Nothing)

        Dim listImp As New List(Of String)

        OperazioneCorrente_EstraiDAL_FiltroImpianti(IDTestataTemp, Impianti, listImp, objParametri)

        OperazioneCorrente_FiltroImpianti = String.Join("|", listImp.ToArray)

    End Sub

    Public Shared Sub OperazioneCorrente_EstraiFiltroneMov(ByVal IDTestataTemp As Integer,
                                                           ByVal Movimenti As List(Of OperazioneAgenda_Temp.Movimento),
                                                           ByRef OperazioneCorrente_FiltroImpianti As String,
                                                           Optional ByVal objParametri As AgronicaCoreParametri = Nothing)

        Dim listImp As New List(Of String)

        OperazioneCorrente_EstraiDAL_FiltroMovimenti(IDTestataTemp, Movimenti, listImp, objParametri)

        OperazioneCorrente_FiltroImpianti = String.Join("|", listImp.ToArray)

    End Sub

    Public Shared Sub PopolaTabellaFiltroImpianti(operazioneCorrente_FiltroImpianti As String,
                                                  ByVal objParametri As AgronicaCoreParametri)

        Dim vQry As String() = operazioneCorrente_FiltroImpianti.Split("|")

        Dim scriviImpianti As New DataProvider

        For Each stmt In vQry

            Dim rVal As Boolean = scriviImpianti.EseguiQuery_Scrittura(objParametri, stmt, "PopolaTabellaFiltroImpianti")

        Next

    End Sub

End Class