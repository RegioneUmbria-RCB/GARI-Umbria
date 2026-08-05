Imports System.Text
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class Mappatura_Specie_Distanze_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi( _
            ByVal filtro_Aggiuntivo As String, _
            ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri _
        ) As DataTable

        Dim NomeRoutine As String = "Sementieri_Sportello_InterferenzePerConferma_R.Sportello_LeggiXData()"
        Dim MessaggioErrore As String = ""

        Dim dt As DataTable
        Dim stb As New StringBuilder

        Try
            stb.Append("SELECT * " & vbCrLf)

            stb.Append("FROM         Mappatura_Specie_Distanze " & vbCrLf)

            stb.Append(" where 1=1 " & vbCrLf)

            If filtro_Aggiuntivo <> "" Then
                stb.Append(" and  " & filtro_Aggiuntivo & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_server, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return dt
    End Function

 
End Class


Public Class Mappatura_Specie_Distanze_W
    Inherits AgronicaCoreDataProvider.DataProvider

    'Public Function Modifica( _
    '                            ByVal Interferenze_cod As Integer, _
    '                            ByVal Stato_cod As Int32, _
    '                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                                ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Sementieri_Sportello_InterferenzePerConferma_W.Modifica()"

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    Try

    '        '---------------------------------------------

    '        StrSQL.Length = 0
    '        StrSQL.Append("UPDATE Sementieri_Sportello_InterferenzePerConferma SET ")
    '        StrSQL.Append("    Data_Conferma     =  " & Agro_SQL_SaveDateTime(Date.Now))
    '        StrSQL.Append("   ,Stato_cod           =  " & Agro_SQL_SaveNum(Stato_cod) & " ")

    '        StrSQL.Append(" WHERE PivasuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
    '        StrSQL.Append(" AND   Interferenze_cod = " & Agro_SQL_SaveNum(Interferenze_cod) & " ")


    '        '--------------------------------------------------------------------------
    '        xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
    '        '--------------------------------------------------------------------------

    '    Catch ex As Exception
    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
    '    End Try

    '    Return xRisp

    'End Function


    'Public Function Modifica_Flag( _
    '                            ByVal Interferenze_Cod As Integer, _
    '                            ByVal Flag_Attivo As Int32, _
    '                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                                ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Sementieri_Sportello_InterferenzePerConferma_W.Modifica()"

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    Try

    '        '---------------------------------------------

    '        StrSQL.Length = 0
    '        StrSQL.Append("UPDATE Sementieri_Sportello_InterferenzePerConferma SET ")
    '        StrSQL.Append("    Flag_Attivo     =  " & Agro_SQL_SaveNum(Flag_Attivo))

    '        StrSQL.Append(" WHERE PivasuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
    '        StrSQL.Append(" AND   Interferenze_Cod = " & Agro_SQL_SaveNum(Interferenze_Cod) & " ")

    '        '--------------------------------------------------------------------------
    '        xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
    '        '--------------------------------------------------------------------------

    '    Catch ex As Exception
    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
    '    End Try

    '    Return xRisp

    'End Function


    'Public Function Modifica_Flag_Entita_Cod_Propietario( _
    '                            ByVal Entita_Cod_Propietario As Integer, _
    '                            ByVal Flag_Attivo As Int32, _
    '                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                                ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Sementieri_Sportello_InterferenzePerConferma_W.Modifica()"

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    Try

    '        '---------------------------------------------

    '        StrSQL.Length = 0
    '        StrSQL.Append("UPDATE Sementieri_Sportello_InterferenzePerConferma SET ")
    '        StrSQL.Append("    Flag_Attivo     =  " & Agro_SQL_SaveNum(Flag_Attivo))

    '        StrSQL.Append(" WHERE PivasuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
    '        StrSQL.Append(" AND   Entita_Cod_Propietario = " & Agro_SQL_SaveNum(Entita_Cod_Propietario) & " ")

    '        '--------------------------------------------------------------------------
    '        xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
    '        '--------------------------------------------------------------------------

    '    Catch ex As Exception
    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
    '    End Try

    '    Return xRisp

    'End Function

    'Public Function Modifica_Flag_Entita_Cod_Interferente( _
    '                        ByVal Entita_Cod_Interferente As Integer, _
    '                        ByVal Flag_Attivo As Int32, _
    '                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                            ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Sementieri_Sportello_InterferenzePerConferma_W.Modifica()"

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    Try

    '        '---------------------------------------------

    '        StrSQL.Length = 0
    '        StrSQL.Append("UPDATE Sementieri_Sportello_InterferenzePerConferma SET ")
    '        StrSQL.Append("    Flag_Attivo     =  " & Agro_SQL_SaveNum(Flag_Attivo))

    '        StrSQL.Append(" WHERE PivasuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
    '        StrSQL.Append(" AND   Entita_Cod_Interferente = " & Agro_SQL_SaveNum(Entita_Cod_Interferente) & " ")

    '        '--------------------------------------------------------------------------
    '        xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
    '        '--------------------------------------------------------------------------

    '    Catch ex As Exception
    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
    '    End Try

    '    Return xRisp

    'End Function



    'Public Function Modifica_Descrizione_Casella_Conflitto( _
    '                        ByVal Entita_Cod_Interferente As Integer, _
    '                        ByVal Descrizione_Casella_Conflitto As String, _
    '                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                            ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Sementieri_Sportello_InterferenzePerConferma_W.Modifica()"

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    Try

    '        '---------------------------------------------

    '        StrSQL.Length = 0
    '        StrSQL.Append("UPDATE Sementieri_Sportello_InterferenzePerConferma SET ")
    '        StrSQL.Append("    Descrizione_Casella_Conflitto     =  '" & Agro_SQL_SaveText(Descrizione_Casella_Conflitto) & "'")

    '        StrSQL.Append(" WHERE PivasuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
    '        StrSQL.Append(" AND   Entita_Cod_Interferente = " & Agro_SQL_SaveNum(Entita_Cod_Interferente) & " ")

    '        '--------------------------------------------------------------------------
    '        xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
    '        '--------------------------------------------------------------------------

    '    Catch ex As Exception
    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
    '    End Try

    '    Return xRisp

    'End Function





    'Public Function Modifica_Tutti_Interferenti( _
    '                            ByVal Entita_Cod_Interferente As Integer, _
    '                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                                ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Sementieri_Sportello_InterferenzePerConferma_W.Modifica()"

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    Try

    '        '---------------------------------------------

    '        StrSQL.Length = 0
    '        StrSQL.Append("UPDATE Sementieri_Sportello_InterferenzePerConferma SET ")
    '        StrSQL.Append("    flag_Attivo     =  " & Agro_SQL_SaveNum(False))

    '        StrSQL.Append(" WHERE PivasuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
    '        StrSQL.Append(" AND   Entita_Cod_Interferente = " & Agro_SQL_SaveNum(Entita_Cod_Interferente) & " ")


    '        '--------------------------------------------------------------------------
    '        xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
    '        '--------------------------------------------------------------------------

    '    Catch ex As Exception
    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
    '    End Try

    '    Return xRisp

    'End Function


    'Public Function Scrivi( _
    '                        ByVal Interferenze_Cod As Integer, _
    '                        ByVal Entita_Cod_Propietario As Integer, _
    '                        ByVal Entita_Cod_Interferente As Integer, _
    '                        ByVal Flag_Attivo As Integer, _
    '                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                               ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Sementieri_Sportello_InterferenzePerConferma_W.Scrivi()"

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    Try

    '        '---------------------------------------------
    '        StrSQL.Length = 0

    '        StrSQL.Append("INSERT INTO Sementieri_Sportello_InterferenzePerConferma (Pivasuperuser, ")
    '        StrSQL.Append("                    interferenze_cod, ")
    '        StrSQL.Append("                    stato_cod , ")
    '        StrSQL.Append("                    Entita_Cod_Propietario, ")
    '        StrSQL.Append("                    Entita_Cod_Interferente, ")
    '        StrSQL.Append("                    Flag_Attivo ")
    '        StrSQL.Append("                    ) ")
    '        StrSQL.Append("VALUES (")
    '        StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
    '        StrSQL.Append("         ," & Agro_SQL_SaveNum(Interferenze_Cod))
    '        StrSQL.Append("         , 1 ")
    '        StrSQL.Append("         ," & Agro_SQL_SaveNum(Entita_Cod_Propietario))
    '        StrSQL.Append("         ," & Agro_SQL_SaveNum(Entita_Cod_Interferente))
    '        StrSQL.Append("         ," & Agro_SQL_SaveNum(Flag_Attivo))
    '        StrSQL.Append(")")
    '        '---------------------------------------------

    '        '--------------------------------------------------------------------------
    '        xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
    '        '--------------------------------------------------------------------------

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return xRisp

    'End Function


End Class