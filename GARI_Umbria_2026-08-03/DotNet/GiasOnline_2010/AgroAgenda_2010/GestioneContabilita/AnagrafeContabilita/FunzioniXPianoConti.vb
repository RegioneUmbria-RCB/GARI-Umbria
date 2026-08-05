Module FunzioniXPianoConti

    '###############################################################################
    Public Function Prepara_FiltroQuery_IdRicl(ByVal Stringa_Id_Ricl As String)

        If Stringa_Id_Ricl <> "" Then

            Dim Vet_IdRicl As String()
            Dim i As Integer

            Vet_IdRicl = Stringa_Id_Ricl.Split("&")

            Stringa_Id_Ricl = " AND (RicXConti.Id_Riclassificazione IN ( "
            ' Stringa_Id_Ricl = "  (RicXConti.Id_Riclassificazione IN ( "

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


End Module
