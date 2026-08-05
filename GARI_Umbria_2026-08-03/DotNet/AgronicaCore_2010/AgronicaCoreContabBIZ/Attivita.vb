Imports System.Web
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.My.Resources

Public Class Attivita_Gruppi_Tipologia_R

    Public Function Trova_Tipologie(ByVal piva As String) As DataTable
        Dim dt As DataTable
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        Try

            Dim dtPub As DataTable
            Dim dtRow As DataRow
            Dim dtRowWhere As DataRow
            Dim lastID As Integer = 0
            Dim primoAssente As Boolean
            Dim secondoAssente As Boolean
            Dim terzoAssente As Boolean
            Dim Obj As New AgronicaCoreContabDAL.Attivita_Tipologie_R

            dt = Obj.Trova_Tipologie(piva, True, True, True, "", "", objParametriServer)

            primoAssente = dt.AsEnumerable.Where(Function(x) x.Item("Tipologia_Attivita") = 1).Count = 0
            secondoAssente = dt.AsEnumerable.Where(Function(x) x.Item("Tipologia_Attivita") = 2).Count = 0
            terzoAssente = dt.AsEnumerable.Where(Function(x) x.Item("Tipologia_Attivita") = 3).Count = 0

            If dt.Rows.Count < 3 Then

                dtPub = Obj.Trova_Tipologie("", primoAssente, secondoAssente, terzoAssente, "", "", objParametriServer)

                If dtPub.Rows.Count > 0 Then
                    Do While dt.Rows.Count < 3 AndAlso dtPub.AsEnumerable.Where(Function(x) x.Item("Tipologia_Attivita") > lastID).Count > 0
                        dtRowWhere = dtPub.AsEnumerable.Where(Function(x) x.Item("Tipologia_Attivita") > lastID).First
                        lastID = dtRowWhere.Item("Tipologia_Attivita")
                        dtRow = dt.NewRow
                        dtRow.Item("Tipologia_Attivita") = dtRowWhere.Item("Tipologia_Attivita")
                        dtRow.Item("Tipologia_Attivita_Desc") = dtRowWhere.Item("Tipologia_Attivita_Desc")
                        dt.Rows.Add(dtRow)
                        Select Case lastID
                            Case 1 : primoAssente = False
                            Case 2 : secondoAssente = False
                            Case Else : terzoAssente = False
                        End Select
                    Loop

                End If

                If dt.Rows.Count < 3 Then

                    If primoAssente Then
                        dtRow = dt.NewRow
                        dtRow.Item("Tipologia_Attivita") = 1
                        dtRow.Item("Tipologia_Attivita_Desc") = Gias.GruppoAttivita & " 1"
                        dt.Rows.Add(dtRow)
                    End If
                    If secondoAssente Then
                        dtRow = dt.NewRow
                        dtRow.Item("Tipologia_Attivita") = 2
                        dtRow.Item("Tipologia_Attivita_Desc") = Gias.GruppoAttivita & " 2"
                        dt.Rows.Add(dtRow)
                    End If
                    If terzoAssente Then
                        dtRow = dt.NewRow
                        dtRow.Item("Tipologia_Attivita") = 3
                        dtRow.Item("Tipologia_Attivita_Desc") = Gias.GruppoAttivita & " 3"
                        dt.Rows.Add(dtRow)
                    End If

                End If

            End If

        Catch ex As Exception

            dt = Nothing

        End Try

        Return dt

    End Function

End Class
