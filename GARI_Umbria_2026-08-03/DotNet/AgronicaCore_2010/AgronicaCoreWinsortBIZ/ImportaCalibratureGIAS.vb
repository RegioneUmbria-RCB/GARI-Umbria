Imports AgronicaCoreDataProvider
Imports AgronicaCoreWinsortDAL

Public Class ImportaCalibratureGIAS
    Dim objPar As AgronicaCoreParametri
    Dim reader As DBCalibrature_R
    Dim writer As DBCalibrature_W

    Public Sub New(objParametri_Server As AgronicaCoreParametri)
        objPar = objParametri_Server
        reader = New DBCalibrature_R(objPar)
        writer = New DBCalibrature_W(objPar)
    End Sub

#Region "Verifica Calibratura"

    Public Sub verificaCalibratura(id As Integer)
        Try
            Dim stato As Integer = reader.getStatoCalibratura(id)
            If stato = TipiEnumerativi.statoImportazione.fileImportato Or stato = TipiEnumerativi.statoImportazione.Errori_in_Fase_di_Verifica Then
                Dim verificata = avviaVerifica(id)
                If verificata Then
                    writer.aggiornaStatoCalibratura(id, TipiEnumerativi.statoImportazione.Verificata)
                Else
                    writer.aggiornaStatoCalibratura(id, TipiEnumerativi.statoImportazione.Errori_in_Fase_di_Verifica)
                End If
            End If
        Catch ex As Exception
            Dim a As Integer
            a = -1
        End Try
    End Sub

    Private Function avviaVerifica(id As Integer) As Boolean
        Dim dtCalibratura = reader.leggiCalibratura(id)

        Dim conf = verificaConferitore(dtCalibratura)
        Dim specVar = verificaSpecieVarieta(dtCalibratura)

        Dim verCal = True
        If specVar Then

            Dim dtCalibri = reader.leggiCalibriXCalibratura(id)

            For Each row As DataRow In dtCalibri.Rows
                If Not verificaCalibro(row) Then
                    verCal = False
                End If
            Next
        End If
        If conf And specVar And verCal Then
            Return True
        Else
            Return False
        End If
    End Function

    Private Function verificaConferitore(dtCalibratura As DataTable) As Boolean
        If IsDBNull(dtCalibratura.Rows(0).Item("Cod_ContattoGIAS")) Then
            Dim nomeConferitore As String = CStr(dtCalibratura.Rows(0).Item("Conferitore_nome"))
            Dim idContatto = reader.getTranscodificaConferitore(nomeConferitore)
            If idContatto <> 0 Then
                writer.scriviCodContatto(CInt(dtCalibratura.Rows(0).Item("ID")), idContatto)
                Return True
            Else
                Return False
            End If
        Else
            Return True
        End If
    End Function

    Private Function verificaSpecieVarieta(dtCalibratura As DataTable) As Boolean
        If IsDBNull(dtCalibratura.Rows(0).Item("Cod_VarietaGIAS")) Or IsDBNull(dtCalibratura.Rows(0).Item("Cod_SpecieVegetaleGIAS")) Then
            Dim nomeSpecieVar As String = CStr(dtCalibratura.Rows(0).Item("Varieta"))
            Dim specie As Integer = reader.getTranscodificaVarieta(nomeSpecieVar)(0)
            Dim varieta As Integer = reader.getTranscodificaVarieta(nomeSpecieVar)(1)
            If specie <> 0 And varieta <> 0 Then
                writer.scriviSpecieVarieta(CInt(dtCalibratura.Rows(0).Item("ID")), specie, varieta)
                Return True
            Else
                Return False
            End If
        Else
            Return True
        End If
    End Function

    Private Function verificaCalibro(row As DataRow) As Boolean
        If IsDBNull(row.Item("Cod_CalibroGIAS")) Then
            Dim nomeCalibro As String = CStr(row.Item("nome"))
            Dim calibro_par_cod As Integer = reader.getTranscodificaCalibro(nomeCalibro)
            If calibro_par_cod <> 0 Then
                writer.scriviCalibroParCod(CInt(row.Item("IDCalibro")), nomeCalibro, calibro_par_cod)
                Return True
            Else
                Return False
            End If
        Else
            Return True
        End If
    End Function

#End Region

#Region "Importa Calibratura"

    Public Sub importaCalibratura(id As Integer)

    End Sub

#End Region

End Class
