Imports System.Data.OleDb
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Macchine_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal class_code As String,
                          ByVal agroParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          Optional ByVal xFiltroAggiuntivo As String = ""
                          ) As DataTable

        Dim q As String = "SELECT * FROM Macchine (NOLOCK)"

        Select Case agroParametri.FlagVisibilita
            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                q += " WHERE   Inviato >=0 "
            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                q += " WHERE   Inviato =-1 "
            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                q += " WHERE   Inviato >=-1"
            Case Else
                Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
        End Select

        If (Not class_code.Equals("")) Then
            q += " AND class_code='" + Agro_SQL_SaveText(class_code) + "'"
        End If

        If xFiltroAggiuntivo <> "" Then
            q += "AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , agroParametri) & " "
        End If

        q += " ORDER BY class_desc"

        'se c'è qualcosa ok...
        Return MyBase.EseguiQuery_Lettura(agroParametri, q, "AgronicaCoreMetaSchemaDAL.Macchine_R.Leggi")



    End Function

    Public Function Leggi2(ByVal class_code As String,
                          ByVal CLASS_DESC As String,
                        ByVal agroParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim q As String = "SELECT * FROM Macchine"

        Select Case agroParametri.FlagVisibilita
            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                q += " WHERE   Inviato >=0 "
            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                q += " WHERE   Inviato =-1 "
            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                q += " WHERE   Inviato >=-1"
            Case Else
                Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
        End Select

        If (Not class_code.Equals("")) Then
            q += " AND class_code='" + Agro_SQL_SaveText(class_code) + "'"
        End If

        If (Not CLASS_DESC.Equals("")) Then
            q += " AND CLASS_DESC='" + Agro_SQL_SaveText(CLASS_DESC) + "'"
        End If

        q += " ORDER BY class_desc"

        'se c'è qualcosa ok...
        Return MyBase.EseguiQuery_Lettura(agroParametri, q, "AgronicaCoreMetaSchemaDAL.Macchine_R.Leggi2")



    End Function


    Public Function MacchinaTipo_from_MacchinaCod(ByVal Tipo_Cod As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim Dt As DataTable

        'Creo gli oggetti COM+
        Dim objMacchine As New AgronicaCoreMetaSchemaDAL.Macchine_R

        'Recupero le informazioni		
        Dt = objMacchine.Leggi(CStr(Tipo_Cod), objParametri)

        'Elimino gli oggetti COM
        objMacchine = Nothing

        'Se il recordset non è chiuso allora ...	
        If Not Dt Is Nothing AndAlso Dt.Rows.Count > 0 Then

            Return Dt.Rows(0).Item("Class_Desc")

        End If

        'Elimino il recordset
        Dt = Nothing

    End Function

    Public Function MacchinaCODE_from_MacchinaDESC(ByVal CLASS_DESC As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim Dt As DataTable

        'Creo gli oggetti COM+
        Dim objMacchine As New AgronicaCoreMetaSchemaDAL.Macchine_R

        'Recupero le informazioni		
        Dt = objMacchine.Leggi2("", CStr(CLASS_DESC), objParametri)

        'Elimino gli oggetti COM
        objMacchine = Nothing

        'Se il recordset non è chiuso allora ...	
        If Dt.Rows.Count = 1 Then

            Return Dt.Rows(0).Item("class_code")
        Else
            Throw New Exception
        End If


    End Function

    Public Function LeggiTipoMacchineCompresso(
        ByVal class_code As String,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
     ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0



            Stb.AppendLine("   Select ")
            Stb.AppendLine("       c1.CLASS_CODE as macchinaTipoCod ")
            Stb.AppendLine("  , c1.CLASS_DESC as macchinaTipoDes ")
            Stb.AppendLine("  , c2.CLASS_CODE as macchinaDettaglio1TipoCod ")
            Stb.AppendLine("  , c2.CLASS_DESC as macchinaDettaglio1TipoDes ")
            Stb.AppendLine("  , c3.CLASS_CODE as macchinaDettaglio2TipoCod ")
            Stb.AppendLine("  , c3.CLASS_DESC as macchinaDettaglio2TipoDes     ")
            Stb.AppendLine("   ")
            Stb.AppendLine(" from ")
            Stb.AppendLine(" ( ")
            Stb.AppendLine("  Select class_code, CLASS_DESC ")
            Stb.AppendLine("     From Macchine ")
            Stb.AppendLine("     Where Len(class_code) = 2 ")
            Stb.AppendLine("  ")
            Stb.AppendLine(" ) c1 inner join ( ")
            Stb.AppendLine("  Select class_code, CLASS_DESC ")
            Stb.AppendLine("     From Macchine ")
            Stb.AppendLine("     Where Len(class_code) = 5 ")
            Stb.AppendLine(" ) c2 on c1.CLASS_CODE = SUBSTRING(c2.class_code, 1, 2) ")
            Stb.AppendLine("  ")
            Stb.AppendLine(" inner Join( ")
            Stb.AppendLine("  Select class_code, CLASS_DESC ")
            Stb.AppendLine("     From Macchine ")
            Stb.AppendLine("     Where Len(class_code) = 8 ")

            If class_code <> "" Then
                Stb.AppendLine("     and class_code = '" & Agro_SQL_SaveText(class_code) & "'")

            End If

            Stb.AppendLine(" ) c3 on c2.CLASS_CODE = substring(c3.class_code, 1, 5) ")
            Stb.AppendLine(" ")



            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT




    End Function




End Class
