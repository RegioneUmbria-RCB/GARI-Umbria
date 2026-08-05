Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class PianoConcimazione_EpocheModalitaxSpecie

    Public Sub New()

    End Sub

    Public Function EpocheModalitaxSpecie(ByVal Input As PianoConcimazione_EpocheModalitaxSpecie_input,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) _
                                       As PianoConcimazione_EpocheModalitaxSpecie_output

        Dim Output As New PianoConcimazione_EpocheModalitaxSpecie_output
        Dim Epoca As EpocheModalitaxSpecie

        Dim Dt As DataTable
        Dim objCore As New AgronicaCoreMetaSchemaDAL.EpocheModalita_R

        '(21/04/2021 fede) 
        'verifico se arriva un regolamento_cod di piano concimazione
        'nel caso verifico se la sua regione ha un pua e nel caso leggo le epoche di quello

        'se arriva regolamento_cod = 0 o non ho trovato un regolamento pua 
        'setto il default ultima direttiva nitrati ER

        Dim objReg As New AgronicaCoreMetaSchemaDAL.PUA_Regolamenti_R
        Dim DtReg As DataTable

        If Input.Regolamento_Cod > 0 Then

            DtReg = objReg.Leggi(Input.Regolamento_Cod, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)
            If Not DtReg Is Nothing AndAlso DtReg.Rows.Count > 0 Then
                Select Case DtReg.Rows(0).Item("tipo")
                    Case enum_PUARegolamenti_Tipo.PianoComcimazione
                        Dim DtReg1 As DataTable
                        Dim Filtro As String = " IDEnte=" & DtReg.Rows(0).Item("IDEnte")
                        DtReg1 = objReg.Leggi(0, enum_PUARegolamenti_Tipo.PUA, DtReg.Rows(0).Item("validita_inizio"), DtReg.Rows(0).Item("validita_fine"), Filtro, "", objParametri)
                        If Not DtReg1 Is Nothing AndAlso DtReg1.Rows.Count > 0 Then
                            Input.Regolamento_Cod = DtReg1.Rows(0).Item("Regolamento_Cod")
                        Else
                            Input.Regolamento_Cod = 0
                        End If
                End Select
            End If

        End If

        'se non ho trovato un regolamento pua setto il default ultima direttiva nitrati ER
        If Input.Regolamento_Cod = 0 Then

            DtReg = objReg.Leggi_DaRegione("08", AgronicaCoreDataProvider.TipiEnumerativi.enum_PUARegolamenti_Tipo.PUA,
                                            Today, Today, "", "", objParametri)
            If Not DtReg Is Nothing AndAlso DtReg.Rows.Count > 0 Then
                Input.Regolamento_Cod = DtReg.Rows(0).Item("Regolamento_Cod")
            End If

        End If

        Dt = objCore.Leggi(Input.Epoca_Cod, Input.Epoca_Gruppo,
                           Input.Specie_Cod, Input.Regolamento_Cod,
                           "", "", objParametri)

        '(19/03/2021 fede) introdotto controllo doppioni per app
        Dim HashInserite As New Hashtable

        For i = 0 To Dt.Rows.Count - 1

            If Input.Specie_Cod > 0 Then

                If Not HashInserite.ContainsKey(Dt.Rows(i).Item("Em_Cod") & "_" & Dt.Rows(i).Item("Veg_Cod")) Then

                    Epoca = New EpocheModalitaxSpecie
                    Epoca.Epoca_Des = Dt.Rows(i).Item("Em_Des")
                    Epoca.Epoca_Cod = Dt.Rows(i).Item("Em_Cod")
                    Epoca.Specie_Cod = Dt.Rows(i).Item("Veg_Cod")
                    Output.ListaEpocheModalitaxSpecie.Add(Epoca)

                    HashInserite.Add(Dt.Rows(i).Item("Em_Cod") & "_" & Dt.Rows(i).Item("Veg_Cod"), "")


                End If

            Else

                '(21/04/2021 fede) introdotto controllo doppioni per terreni nudi

                If Not HashInserite.ContainsKey(Dt.Rows(i).Item("Em_Cod")) Then

                    Epoca = New EpocheModalitaxSpecie
                    Epoca.Epoca_Des = Dt.Rows(i).Item("Em_Des")
                    Epoca.Epoca_Cod = Dt.Rows(i).Item("Em_Cod")
                    Epoca.Specie_Cod = 0
                    Output.ListaEpocheModalitaxSpecie.Add(Epoca)

                    HashInserite.Add(Dt.Rows(i).Item("Em_Cod"), "")

                End If

            End If

        Next

        Return Output

    End Function

End Class


Public Class PianoConcimazione_EpocheModalitaxSpecie_input

    Public Regolamento_Cod As Integer
    Public Epoca_Cod As Integer
    Public Epoca_Gruppo As Integer
    Public Specie_Cod As Integer

    Public Url As String

    Sub New()

        Regolamento_Cod = 0
        Epoca_Cod = 0
        Epoca_Gruppo = 0
        Specie_Cod = 0

    End Sub

End Class

Public Class PianoConcimazione_EpocheModalitaxSpecie_output

    Public ListaEpocheModalitaxSpecie As List(Of EpocheModalitaxSpecie)

    Public MessaggioErrore As String

    Public Sub New()

        ListaEpocheModalitaxSpecie = New List(Of EpocheModalitaxSpecie)
        MessaggioErrore = ""

    End Sub

End Class

Public Class EpocheModalitaxSpecie

    Public Epoca_Des As String
    Public Epoca_Cod As Integer
    Public Specie_Cod As Integer

    Sub New()

        Epoca_Des = ""
        Epoca_Cod = 0
        Specie_Cod = 0

    End Sub

End Class



