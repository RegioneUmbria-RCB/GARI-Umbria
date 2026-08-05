Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
Public Class PDC_Codice_Analisi_Fruttagel_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiAnnoProgressivi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer
        Dim NomeRoutine As String = "AgronicaCoreAnalisiDAL.PDC_Campioni_R.LeggiAnnoProgressivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.AppendLine("SELECT MAX(Anno) AS AnnoProgressivo FROM PDC_Codice_Analisi_Fruttagel")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        If DT.Rows.Count > 0 Then
            Return DT.Rows(0).Item("AnnoProgressivo")
        End If

        Return 0

    End Function


    Public Function GetProgressivo_Specie(ByVal ID_PDC_Campione As Int32, _
                                          ByVal Analisi_Testata_Cod As Int32, _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                              ) As String
        Dim NomeRoutine As String = "AgronicaCoreAnalisiDAL.PDC_Campioni_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            '---------------------------------------------

            Dim str As String
            str = " WHERE     "
            If ID_PDC_Campione <> 0 Then
                str = str & "(PDC_Campioni.ID_PDC_Campione = " & ID_PDC_Campione & ")) "
            End If
            If Analisi_Testata_Cod <> 0 Then
                str = str & "(PDC_Analisi.Analisi_Testata_Cod = " & Analisi_Testata_Cod & ")) "
            End If



            Dim str_anno As String = " (SELECT     YEAR(PDC_Campioni.Data_Campionamento) " & _
                    " FROM         PDC_Campioni  " & _
                    "    INNER JOIN PDC_Dettagli ON PDC_Campioni.PivaSuperUser = PDC_Dettagli.PivaSuperUser AND PDC_Campioni.ID_PDC_Testata = PDC_Dettagli.ID_PDC_Testata AND PDC_Campioni.ID_PDC_Dettagli = PDC_Dettagli.ID_PDC_Dettagli " & _
                    "    LEFT JOIN PDC_Analisi ON PDC_Campioni.PivaSuperUser = PDC_Analisi.PivaSuperUser AND PDC_Campioni.ID_PDC_Testata = PDC_Analisi.ID_PDC_Testata AND PDC_Campioni.ID_PDC_Dettagli = PDC_Analisi.ID_PDC_Dettagli AND PDC_Campioni.ID_PDC_Campione = PDC_Analisi.ID_PDC_Campione "

            Dim str_Veg_Cod As String = "(SELECT     PDC_Dettagli.Veg_Cod " & _
                            " FROM         PDC_Dettagli  " & _
                            "    INNER JOIN PDC_Campioni ON PDC_Dettagli.PivaSuperUser = PDC_Campioni.PivaSuperUser AND PDC_Dettagli.ID_PDC_Testata = PDC_Campioni.ID_PDC_Testata AND  PDC_Dettagli.ID_PDC_Dettagli = PDC_Campioni.ID_PDC_Dettagli " & _
                            "    LEFT JOIN PDC_Analisi ON PDC_Campioni.PivaSuperUser = PDC_Analisi.PivaSuperUser AND PDC_Campioni.ID_PDC_Testata = PDC_Analisi.ID_PDC_Testata AND PDC_Campioni.ID_PDC_Dettagli = PDC_Analisi.ID_PDC_Dettagli AND PDC_Campioni.ID_PDC_Campione = PDC_Analisi.ID_PDC_Campione "

            Dim str_Cul_Cod As String = "(SELECT     PDC_Dettagli.Cul_Cod " & _
                         " FROM         PDC_Dettagli  " & _
                         "    INNER JOIN PDC_Campioni ON PDC_Dettagli.PivaSuperUser = PDC_Campioni.PivaSuperUser AND PDC_Dettagli.ID_PDC_Testata = PDC_Campioni.ID_PDC_Testata AND  PDC_Dettagli.ID_PDC_Dettagli = PDC_Campioni.ID_PDC_Dettagli " & _
                         "    LEFT JOIN PDC_Analisi ON PDC_Campioni.PivaSuperUser = PDC_Analisi.PivaSuperUser AND PDC_Campioni.ID_PDC_Testata = PDC_Analisi.ID_PDC_Testata AND PDC_Campioni.ID_PDC_Dettagli = PDC_Analisi.ID_PDC_Dettagli AND PDC_Campioni.ID_PDC_Campione = PDC_Analisi.ID_PDC_Campione "



            StrSQL.AppendLine(" select TOP(1) due.abbreviazione + right ('0000'+ cast(uno.progressivo as varchar(4)),4) as Progressivo ,due.cul_cod  from ")

            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine(" Select isnull((progressivo), 0) + 1 as Progressivo, cul_cod  ")
            StrSQL.AppendLine(" FROM PDC_Codice_Analisi_Fruttagel INNER JOIN ")
            StrSQL.AppendLine("     PDC_Codice_Analisi_Fruttagel_Veg_Cod ON PDC_Codice_Analisi_Fruttagel.PivaSuperUser = PDC_Codice_Analisi_Fruttagel_Veg_Cod.PivaSuperUser AND ")
            StrSQL.AppendLine("     PDC_Codice_Analisi_Fruttagel.Codice_Cod = PDC_Codice_Analisi_Fruttagel_Veg_Cod.Codice_Cod ")

            StrSQL.AppendLine(" where PDC_Codice_Analisi_Fruttagel.anno = ")
            'vincolo su anno
            StrSQL.AppendLine(str_anno)
            StrSQL.AppendLine(str)

            'vincolo su specie
            StrSQL.AppendLine(" and PDC_Codice_Analisi_Fruttagel_Veg_Cod.Veg_Cod = ")
            StrSQL.AppendLine(str_Veg_Cod)
            StrSQL.AppendLine(str)

            'vincolo su cul_cod
            StrSQL.AppendLine(" and ( (PDC_Codice_Analisi_Fruttagel_Veg_Cod.Cul_Cod is null)  ")
            StrSQL.AppendLine("     OR ")
            StrSQL.AppendLine("     (PDC_Codice_Analisi_Fruttagel_VEG_COD.Cul_Cod = ")
            StrSQL.AppendLine(str_Cul_Cod)
            StrSQL.AppendLine(str)
            StrSQL.AppendLine(" )  ")
            StrSQL.AppendLine(" )  ")

            StrSQL.AppendLine(" )uno inner join ( ")

            StrSQL.AppendLine(" Select abbreviazione , cul_cod ")
            StrSQL.AppendLine(" FROM PDC_Codice_Analisi_Fruttagel INNER JOIN ")
            StrSQL.AppendLine("     PDC_Codice_Analisi_Fruttagel_Veg_Cod ON PDC_Codice_Analisi_Fruttagel.PivaSuperUser = PDC_Codice_Analisi_Fruttagel_Veg_Cod.PivaSuperUser AND PDC_Codice_Analisi_Fruttagel.Codice_Cod = PDC_Codice_Analisi_Fruttagel_Veg_Cod.Codice_Cod ")

            StrSQL.AppendLine(" where PDC_Codice_Analisi_Fruttagel.anno = ")
            'vincolo su anno
            StrSQL.AppendLine(str_anno)
            StrSQL.AppendLine(str)

            'vincolo su specie
            StrSQL.AppendLine(" and PDC_Codice_Analisi_Fruttagel_Veg_Cod.Veg_Cod =  ")
            StrSQL.AppendLine(str_Veg_Cod)
            StrSQL.AppendLine(str)


            'vincolo su cul_cod
            StrSQL.AppendLine(" and ( (PDC_Codice_Analisi_Fruttagel_Veg_Cod.Cul_Cod is null)  ")
            StrSQL.AppendLine("     OR ")
            StrSQL.AppendLine("     (PDC_Codice_Analisi_Fruttagel_VEG_COD.Cul_Cod = ")
            StrSQL.AppendLine(str_Cul_Cod)
            StrSQL.AppendLine(str)
            StrSQL.AppendLine(" )  ")
            StrSQL.AppendLine(" )  ")
            StrSQL.AppendLine(" ) due on 1=1 and  ((uno.Cul_Cod = due.cul_cod) or (due.Cul_Cod is null)) ")
            StrSQL.AppendLine(" order by due.cul_cod desc  ")



            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try
        If DT.Rows.Count = 0 Then
            Return -1
        End If
        Return DT.Rows(0).Item("Progressivo")


    End Function


End Class



'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

Public Class PDC_Codice_Analisi_Fruttagel_W
    Inherits AgronicaCoreDataProvider.DataProvider



#Region "Scrivi"

    Public Function ScriviProgressiviAnno(ByVal Anno_From As Integer, ByVal Anno_To As Integer,
                                          objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnalisiDal.PDC_Codice_Analisi_Fruttagel_W.ScriviProgressiviAnno()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine("INSERT INTO PDC_Codice_Analisi_Fruttagel (PivaSuperUser,Codice_Cod,Abbreviazione,Progressivo,Anno,Data_Creazione,Data_Modifica,Username_Creazione ,Username_Modifica) ")
            StrSQL.AppendLine("SELECT PivaSuperUser, Codice_Cod, Abbreviazione, 0, " & Anno_To & ", GETDATE(), GETDATE(), Username_Creazione, Username_Modifica ")
            StrSQL.AppendLine("FROM PDC_Codice_Analisi_Fruttagel WHERE Anno=" & Anno_From)

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

    Public Function Scrivi( _
                           ByVal Veg_Cod As Integer, _
                           ByVal Abbreviazione As String, _
                           ByVal Progressivo As Integer, _
                           ByVal Anno As Integer, _
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                               ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnalisiDal.PDC_Codice_Analisi_Fruttagel_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("INSERT INTO PDC_Codice_Analisi_Fruttagel(PivaSuperUser , Veg_Cod, Abbreviazione, Progressivo, ")
            StrSQL.AppendLine("         Anno,  ")
            StrSQL.AppendLine("         Data_Creazione , Data_Modifica , Username_Creazione, Username_Modifica")
            StrSQL.AppendLine("  ) ")

            StrSQL.AppendLine("  VALUES (")
            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Veg_Cod))
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(Abbreviazione) & "'")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Progressivo))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Anno))
            StrSQL.AppendLine("         ," & Agro_SQL_SaveDate(Now.Date) & "")
            StrSQL.AppendLine("         ," & Agro_SQL_SaveDate(Now.Date) & "")
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            StrSQL.AppendLine(")")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

#End Region

#Region "Modifica"

    Public Function ModificaProgressivo(ByVal ID_Pdc_Campione As Integer, _
                                        ByVal Analisi_testata_cod As Integer, _
                                        ByVal Veg_Cod As Integer, _
                           ByVal Progressivo As Integer, _
                           ByVal Anno As Integer, _
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                               ) As Boolean

        Dim NomeRoutine As String = "AgronicaAnalisiDAL.PDC_W.Modifica()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try


            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" UPDATE PDC_Codice_Analisi_Fruttagel SET ")

            StrSQL.AppendLine("   Progressivo   =  " & Agro_SQL_SaveNum(Progressivo) & "   ")
            StrSQL.AppendLine("   ,Username_Modifica   =  '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'   ")


            StrSQL.AppendLine(" WHERE PivaSuperUser      = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            StrSQL.AppendLine(" AND Codice_Cod = ( ")


            StrSQL.AppendLine(" SELECT     top 1 PDC_Codice_Analisi_Fruttagel_Veg_Cod.Codice_Cod ")
            StrSQL.AppendLine(" FROM         PDC_Codice_Analisi_Fruttagel_Veg_Cod INNER JOIN ")
            StrSQL.AppendLine("         PDC_Codice_Analisi_Fruttagel ON PDC_Codice_Analisi_Fruttagel_Veg_Cod.PivaSuperUser = PDC_Codice_Analisi_Fruttagel.PivaSuperUser AND ")
            StrSQL.AppendLine("         PDC_Codice_Analisi_Fruttagel_Veg_Cod.Codice_Cod = PDC_Codice_Analisi_Fruttagel.Codice_Cod ")
            StrSQL.AppendLine(" where 1= 1  ")

            If Anno <> 0 Then
                StrSQL.AppendLine(" AND   Anno    =  " & Agro_SQL_SaveNum(Anno) & "   ")
            End If
            If Veg_Cod <> 0 Then
                StrSQL.AppendLine(" AND   Veg_Cod    =  " & Agro_SQL_SaveNum(Veg_Cod) & "   ")
            End If

            StrSQL.AppendLine(" and ((PDC_Codice_Analisi_Fruttagel_Veg_Cod.cul_cod is null ) ")
            StrSQL.AppendLine(" or (PDC_Codice_Analisi_Fruttagel_Veg_Cod.Cul_Cod = ( ")
            StrSQL.AppendLine("        SELECT     PDC_Dettagli.Cul_Cod ")
            StrSQL.AppendLine("        FROM         PDC_Analisi INNER JOIN ")
            StrSQL.AppendLine("          PDC_Campioni ON PDC_Analisi.PivaSuperUser = PDC_Campioni.PivaSuperUser AND PDC_Analisi.ID_PDC_Testata = PDC_Campioni.ID_PDC_Testata AND PDC_Analisi.ID_PDC_Dettagli = PDC_Campioni.ID_PDC_Dettagli AND PDC_Analisi.ID_PDC_Campione = PDC_Campioni.ID_PDC_Campione INNER JOIN ")
            StrSQL.AppendLine("          PDC_Dettagli ON PDC_Campioni.PivaSuperUser = PDC_Dettagli.PivaSuperUser AND PDC_Campioni.ID_PDC_Testata = PDC_Dettagli.ID_PDC_Testata AND PDC_Campioni.ID_PDC_Dettagli = PDC_Dettagli.ID_PDC_Dettagli ")
            StrSQL.AppendLine(" where( 1=1 ")
            If ID_Pdc_Campione <> 0 Then
                StrSQL.AppendLine(" AND   PDC_Campioni.ID_Pdc_Campione    =  " & Agro_SQL_SaveNum(ID_Pdc_Campione) & "   ")
            End If
            If Analisi_testata_cod <> 0 Then
                StrSQL.AppendLine(" AND   PDC_Analisi.Analisi_Testata_Cod    =  " & Agro_SQL_SaveNum(Analisi_testata_cod) & "   ")
            End If
            StrSQL.AppendLine("  )  ")
            StrSQL.AppendLine(" ))) order by cul_cod desc    ")



            StrSQL.AppendLine(" ) ")


            If Anno <> 0 Then
                StrSQL.AppendLine(" AND   Anno    =  " & Agro_SQL_SaveNum(Anno) & "   ")
            End If


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp
    End Function

#End Region

#Region "Cancellazione"

    Public Function Cancella( _
                           ByVal Veg_Cod As Int32, _
                           ByVal Anno As Int32, _
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                               ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnalisiDal.PDC_Campioni_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("DELETE FROM PDC_Codice_Analisi_Fruttagel ")

            StrSQL.AppendLine("  WHERE ")
            StrSQL.AppendLine("         PivaSuperUser =  '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Veg_Cod <> 0 Then
                StrSQL.AppendLine("     AND Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod))
            End If
            If Anno <> 0 Then
                StrSQL.AppendLine("     AND Anno = " & Agro_SQL_SaveNum(Anno))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function



#End Region



End Class




'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
 