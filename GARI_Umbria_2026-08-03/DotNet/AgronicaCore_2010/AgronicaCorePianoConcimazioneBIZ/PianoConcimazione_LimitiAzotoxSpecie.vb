Public Class PianoConcimazione_LimitiAzotoxSpecie

    Public Function LimitiAzotoxSpecie(ByVal LimiteMASInput As PianoConcimazione_LimiteMAS_input,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) _
                                       As PianoConcimazione_LimiteMAS_output

        Dim LimiteMASOutput As New PianoConcimazione_LimiteMAS_output

        Dim N As Decimal = -1
        Dim Resa As Decimal = -1
        Dim FattoreCorrettivo_N = 0

        Dim Regolamento_Cod_Dir_Nitrati As Integer = 0
        Dim N_Dir_Nitrati As Decimal = -1
        Dim Resa_Dir_Nitrati As Decimal = -1
        Dim FattoreCorrettivo_N_Dir_Nitrati = 0

        Dim objLimitiAzotoxSpecie As New AgronicaCoreMetaSchemaDAL.LimitiAzotoxSpecie_R

        N = objLimitiAzotoxSpecie.RecuperaAzotoResaFromVegCod_Regolamento(LimiteMASInput.Veg_Cod,
                                                                          LimiteMASInput.Grfi_Cod,
                                                                          LimiteMASInput.Stato_Cod,
                                                                          LimiteMASInput.ValoreMax,
                                                                          LimiteMASInput.Regolamento_Cod,
                                                                              Resa, FattoreCorrettivo_N,
                                                                              objParametri)

        'leggo la corrispondente direttiva nitrati (per regione e data)
        If LimiteMASInput.Regolamento_Cod > 0 Then

            Dim objRegolamenti As New AgronicaCoreMetaSchemaDAL.PUA_Regolamenti_R
            Dim DtReg As New DataTable
            DtReg = objRegolamenti.Leggi_Dir_Nitrati_DaRegolamentoCod(LimiteMASInput.Regolamento_Cod, "", "", objParametri)
            If Not DtReg Is Nothing AndAlso DtReg.Rows.Count > 0 Then
                'verifico prima con la stessa finalita
                N_Dir_Nitrati = objLimitiAzotoxSpecie.RecuperaAzotoResaFromVegCod_Regolamento(LimiteMASInput.Veg_Cod,
                                                                                  LimiteMASInput.Grfi_Cod,
                                                                                  LimiteMASInput.Stato_Cod,
                                                                                  LimiteMASInput.ValoreMax,
                                                                                  DtReg.Rows(0).Item("regolamento_cod"),
                                                                                      Resa_Dir_Nitrati, FattoreCorrettivo_N_Dir_Nitrati,
                                                                                      objParametri)
                If N_Dir_Nitrati < 0 Then
                    N_Dir_Nitrati = objLimitiAzotoxSpecie.RecuperaAzotoResaFromVegCod_Regolamento(LimiteMASInput.Veg_Cod,
                                                                                      0,
                                                                                      LimiteMASInput.Stato_Cod,
                                                                                      LimiteMASInput.ValoreMax,
                                                                                      DtReg.Rows(0).Item("regolamento_cod"),
                                                                                          Resa_Dir_Nitrati, FattoreCorrettivo_N_Dir_Nitrati,
                                                                                          objParametri)
                End If

            End If

            LimiteMASOutput.N_Dir_Nitrati = N_Dir_Nitrati
            LimiteMASOutput.Resa_Dir_Nitrati = Resa_Dir_Nitrati
            LimiteMASOutput.FattoreCorrettivo_N_Dir_Nitrati = FattoreCorrettivo_N_Dir_Nitrati

        End If


        LimiteMASOutput.N = N
        LimiteMASOutput.Resa = Resa
        LimiteMASOutput.FattoreCorrettivo_N = FattoreCorrettivo_N

        Return LimiteMASOutput

    End Function

    Public Function LimitiAzotoxSpecie_Elenco(ByVal LimiteMASInput As PianoConcimazione_LimiteMAS_input,
                                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) _
                                               As PianoConcimazione_LimiteMAS_Elenco_output

        Dim Output As New PianoConcimazione_LimiteMAS_Elenco_output
        Dim Elemento As PianoConcimazione_LimiteMAS_output

        Dim objLimitiAzotoxSpecie As New AgronicaCoreMetaSchemaDAL.LimitiAzotoxSpecie_R
        Dim Dt As DataTable

        Dt = objLimitiAzotoxSpecie.RecuperaAzotoResaFromVegCod_Regolamento_Elenco(LimiteMASInput.Veg_Cod_Elenco,
                                                                          LimiteMASInput.ValoreMax,
                                                                          LimiteMASInput.Regolamento_Cod,
                                                                              objParametri)

        For i = 0 To Dt.Rows.Count - 1

            Elemento = New PianoConcimazione_LimiteMAS_output
            Elemento.Regolamento_Cod = Dt.Rows(i).Item("Regolamento_Cod")
            Elemento.Veg_Cod = Dt.Rows(i).Item("Veg_Cod")
            Elemento.Grfi_Cod = Dt.Rows(i).Item("Grfi_Cod")
            Elemento.Stato_Cod = Dt.Rows(i).Item("stato_cod")
            If Not IsDBNull(Dt.Rows(i).Item("N")) Then
                Elemento.N = Dt.Rows(i).Item("N")
            End If
            If Not IsDBNull(Dt.Rows(i).Item("Resa")) Then
                Elemento.Resa = Dt.Rows(i).Item("Resa")
            End If
            If Not IsDBNull(Dt.Rows(i).Item("FattoreCorrettivo_N")) Then
                Elemento.FattoreCorrettivo_N = Dt.Rows(i).Item("FattoreCorrettivo_N")
            End If
            Output.ListaLimitiMas.Add(Elemento)

        Next

        Return Output

    End Function

End Class

Public Class PianoConcimazione_LimiteMAS_input

    Public Regolamento_Cod As Integer

    Public Veg_Cod As Integer
    Public Grfi_Cod As Integer
    Public Stato_Cod As Integer

    Public Veg_Cod_Elenco As String

    Public ValoreMax As Boolean 'utilizzato in passato per il pomodoro che aveva diversi mas in funzione del trapianto (prima o dopo il 5 maggio)


    Sub New()

        ValoreMax = True

    End Sub

End Class

Public Class PianoConcimazione_LimiteMAS_output

    Public Regolamento_Cod As Integer
    Public Veg_Cod As Integer
    Public Grfi_Cod As Integer
    Public Stato_Cod As Integer
    Public N As Decimal
    Public Resa As Decimal
    Public FattoreCorrettivo_N As Decimal

    Public N_Dir_Nitrati As Decimal
    Public Resa_Dir_Nitrati As Decimal
    Public FattoreCorrettivo_N_Dir_Nitrati As Decimal

    Public MessaggioErrore As String

    Sub New()

        Regolamento_Cod = 0
        Veg_Cod = 0
        Grfi_Cod = 0
        Stato_Cod = 0

        N = 0
        Resa = 0
        FattoreCorrettivo_N = 0

        N_Dir_Nitrati = 0
        Resa_Dir_Nitrati = 0
        FattoreCorrettivo_N_Dir_Nitrati = 0

        MessaggioErrore = ""

    End Sub

End Class

Public Class PianoConcimazione_LimiteMAS_Elenco_output

    Public ListaLimitiMas As List(Of PianoConcimazione_LimiteMAS_output)

    Public MessaggioErrore As String

    Sub New()

        ListaLimitiMas = New List(Of PianoConcimazione_LimiteMAS_output)
        MessaggioErrore = ""

    End Sub

End Class




