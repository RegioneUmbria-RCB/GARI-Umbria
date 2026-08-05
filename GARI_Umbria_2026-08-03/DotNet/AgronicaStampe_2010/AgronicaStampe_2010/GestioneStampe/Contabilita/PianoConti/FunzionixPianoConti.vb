Public Class FunzionixPianoConti

    'Se si modifica questo modulo, bisogna modificare anche il suo gemello, nel GiasOnline


    '###############################################################################
    Public Function Prepara_FiltroQuery_IdRicl(ByVal Stringa_Id_Ricl As String)

        If Stringa_Id_Ricl <> "" Then

            Dim Vet_IdRicl As String()
            Dim i As Integer

            Vet_IdRicl = Stringa_Id_Ricl.Split("&")

            Stringa_Id_Ricl = " AND (RicXConti.Id_Riclassificazione IN ( "

            For i = 0 To Vet_IdRicl.Length - 1

                Stringa_Id_Ricl += "'" + CStr(Vet_IdRicl(i)) + "',"

            Next

            Stringa_Id_Ricl = Mid(Stringa_Id_Ricl, 1, Stringa_Id_Ricl.Length - 1)

            Stringa_Id_Ricl += ") ) "

            Return Stringa_Id_Ricl

        Else
            Return ""
        End If

    End Function


    '###############################################################################
    Public Sub Prepara_FiltroQuery_IdRicl_ConfrontoBilanci(ByVal Stringa_Id_Ricl As String, _
                                                                ByRef FiltroQuery1 As String, _
                                                                ByRef FiltroQuery2 As String, _
                                                                ByRef FiltroQuery3 As String)

        Dim FiltroQuery_a As String
        Dim FiltroQuery_b As String

        If Stringa_Id_Ricl <> "" Then

            Dim Vet_IdRicl As String()
            Dim i As Integer

            Vet_IdRicl = Stringa_Id_Ricl.Split("&")

            FiltroQuery_a = " (RicXConti_Anno1.Id_Riclassificazione IN ( "
            FiltroQuery_b = " AND (RicXConti_Anno2.Id_Riclassificazione IN ( "
            FiltroQuery2 = " (RicXConti.Id_Riclassificazione IN ( "
            FiltroQuery3 = " (RicXConti.Id_Riclassificazione IN ( "

            For i = 0 To Vet_IdRicl.Length - 1

                FiltroQuery_a += "'" + CStr(Vet_IdRicl(i)) + "',"
                FiltroQuery_b += "'" + CStr(Vet_IdRicl(i)) + "',"
                FiltroQuery2 += "'" + CStr(Vet_IdRicl(i)) + "',"
                FiltroQuery3 += "'" + CStr(Vet_IdRicl(i)) + "',"

            Next

            FiltroQuery_a = Mid(FiltroQuery_a, 1, FiltroQuery_a.Length - 1)
            FiltroQuery_b = Mid(FiltroQuery_b, 1, FiltroQuery_b.Length - 1)
            FiltroQuery2 = Mid(FiltroQuery2, 1, FiltroQuery2.Length - 1)
            FiltroQuery3 = Mid(FiltroQuery3, 1, FiltroQuery3.Length - 1)

            FiltroQuery_a += ") ) "
            FiltroQuery_b += ") ) "
            FiltroQuery2 += ") ) "
            FiltroQuery3 += ") ) "

            FiltroQuery1 = FiltroQuery_a + vbCrLf + FiltroQuery_b

        End If

    End Sub







End Class
