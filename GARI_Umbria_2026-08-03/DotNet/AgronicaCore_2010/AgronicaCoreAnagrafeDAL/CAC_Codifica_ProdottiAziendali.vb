Imports System.Text
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.DataProviderExtensions
Public Class CAC_Codifica_ProdottiAziendali_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    'campo Tipo_Codifica aggiunto dal migra 296
    Public Function Leggi(ByVal Piva As String,
                          ByVal Tipo_Codifica As enum_Tipo_CAC_Codifica_ProdottiAziendali,
                          ByVal Elem_Cod As Integer,
                          ByVal Cod_Prodotto_Cliente As String,
                          ByVal Cod_Articolo As String,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.CAC_Codifica_ProdottiAziendali_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT *  ")
            strSql.AppendLine(" FROM  CAC_Codifica_ProdottiAziendali ")
            strSql.AppendLine(" WHERE     (Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "')  ")


            If Piva <> "" Then
                strSql.AppendLine(" AND    (Piva = '" & Agro_SQL_SaveText(Piva) & "')   ")
            End If

            If Tipo_Codifica <> -1 Then
                strSql.AppendLine(" AND     (Tipo_Codifica = " & Agro_SQL_SaveNum(Tipo_Codifica) & ")   ")
            End If

            If Elem_Cod <> 0 Then
                strSql.AppendLine(" AND     (Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & ")   ")
            End If

            If Cod_Prodotto_Cliente <> "" Then
                strSql.AppendLine(" AND    (Cod_Prodotto_Cliente = '" & Agro_SQL_SaveText(Cod_Prodotto_Cliente) & "')   ")
            End If

            If Cod_Articolo <> "" Then
                strSql.AppendLine(" AND    (Cod_Articolo = '" & Agro_SQL_SaveText(Cod_Articolo) & "')   ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function


    '###################################################################################
    'campo Tipo_Codifica aggiunto dal migra 296
    Public Function Recupera_CodiceGIAS_from_CodProdCliente(ByVal Piva As String,
                                                            ByVal Tipo_Codifica As enum_Tipo_CAC_Codifica_ProdottiAziendali,
                                                            ByVal Elem_Cod As Integer,
                                                            ByVal Cod_Prod_Cliente As String,
                                                            ByVal Cod_Articolo As String,
                                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                            ) As Integer

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.CAC_Codifica_ProdottiAziendali_R.Recupera_CodiceGIAS_from_CodProdCliente()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim codiceGias As Integer = 0

        Try

            dt = Leggi(Piva, Tipo_Codifica, Elem_Cod, Cod_Prod_Cliente, Cod_Articolo,
                       "", "",
                       objParametri)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                codiceGias = dt.Rows(0).Item("Codice_GIAS")
            End If

            dt = Nothing

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return codiceGias

    End Function

    '###################################################################################
    'campo Tipo_Codifica aggiunto dal migra 296
    Public Function Recupera_CodArticolo_from_CodiceGIAS(ByVal Piva As String,
                                                         ByVal Tipo_Codifica As enum_Tipo_CAC_Codifica_ProdottiAziendali,
                                                         ByVal Elem_Cod As Integer,
                                                         ByVal Codice_GIAS As Integer,
                                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                         ) As String

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.CAC_Codifica_ProdottiAziendali_R.Recupera_CodiceGIAS_from_CodProdCliente()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim codArticolo As String = ""

        Try

            dt = Leggi(Piva, Tipo_Codifica, Elem_Cod, "", "", _
                       " (Codice_GIAS = " & Codice_GIAS & ") ", "", _
                       objParametri)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                codArticolo = dt.Rows(0).Item("Cod_Articolo")
            End If

            dt = Nothing

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return codArticolo

    End Function
    '################################################################################
    'campo Tipo_Codifica aggiunto dal migra 296
    Public Function Verifica_EsisteCodifica(ByVal Piva As String,
                                            ByVal Tipo_Codifica As enum_Tipo_CAC_Codifica_ProdottiAziendali,
                                            ByVal Elem_Cod As Integer,
                                            ByVal Cod_Prodotto_Cliente As String,
                                            ByVal Cod_Articolo As String,
                                            ByRef Codice_GIAS As Integer,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As Boolean

        Dim dt As DataTable
        Dim flagTrovato As Boolean = False

        Codice_GIAS = 0

        dt = Leggi(Piva,
                   Tipo_Codifica,
                   Elem_Cod,
                   Cod_Prodotto_Cliente,
                   Cod_Articolo,
                   "", "",
                   objParametri)

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            Codice_GIAS = dt.Rows(0).Item("Codice_GIAS")
            flagTrovato = True
        End If

        Return flagTrovato

    End Function


End Class

'###################################################################################
'###################################################################################
'###################################################################################
'###################################################################################
'###################################################################################

Public Class CAC_Codifica_ProdottiAziendali_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '###############################################################################
    'campo Tipo_Codifica aggiunto dal migra 296
    Public Function Scrivi(ByVal Piva As String,
                           ByVal Tipo_Codifica As enum_Tipo_CAC_Codifica_ProdottiAziendali,
                           ByVal Elem_Cod As Integer,
                           ByVal Cod_Prodotto_Cliente As String,
                           ByVal Cod_Articolo As String,
                           ByVal Desc_Prodotto_Cliente As String,
                           ByVal Categoria_Prodotto_Cliente As String,
                           ByVal Codice_GIAS As Integer,
                           ByVal Desc_GIAS As String,
                           ByVal Note As String,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.CAC_Codifica_ProdottiAziendali_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            strSql.Length = 0
            strSql.AppendLine(" INSERT INTO CAC_Codifica_ProdottiAziendali ")
            strSql.AppendLine("             ( ")
            strSql.AppendLine("              Piva_SuperUser, Piva, Tipo_Codifica, Elem_Cod, ")
            strSql.AppendLine("              Cod_Prodotto_Cliente, Cod_Articolo, Desc_Prodotto_Cliente, ")
            strSql.AppendLine("              Categoria_Prodotto_Cliente, Codice_GIAS, ")
            strSql.AppendLine("              Desc_GIAS, Note, ")
            strSql.AppendLine("              UserName_Creazione, UserName_Modifica, ")
            strSql.AppendLine("              Data_Creazione,     Data_Modifica ")
            strSql.AppendLine("              ) ")
            strSql.AppendLine(" VALUES (")
            strSql.AppendLine("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(Piva) & "'  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Tipo_Codifica) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Elem_Cod) & "  ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(Cod_Prodotto_Cliente) & "'  ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(Cod_Articolo) & "'  ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(Desc_Prodotto_Cliente) & "'  ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(Categoria_Prodotto_Cliente) & "'  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Codice_GIAS) & "  ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(Desc_GIAS) & "'  ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(Note) & "'  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            strSql.AppendLine(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return xRisp

    End Function


    '############################################################
    ''' <summary>
    ''' Se non si vuole filtrare su Codice_Gias, passare a quella variabile il valore -99
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="Tipo_Codifica"></param>
    ''' <param name="Elem_Cod"></param>
    ''' <param name="Cod_Prodotto_Cliente"></param>
    ''' <param name="Cod_Articolo"></param>
    ''' <param name="Codice_GIAS">No Filtro = -99</param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    Public Function Cancella(ByVal Piva As String,
                             ByVal Tipo_Codifica As enum_Tipo_CAC_Codifica_ProdottiAziendali,
                             ByVal Elem_Cod As Integer,
                             ByVal Cod_Prodotto_Cliente As String,
                             ByVal Cod_Articolo As String,
                             ByVal Codice_GIAS As Integer,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.CAC_Codifica_ProdottiAziendali_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            strSql.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.Length = 0
                strSql.AppendLine(" UPDATE CAC_Codifica_ProdottiAziendali ")
                strSql.AppendLine(" SET ")
                strSql.AppendLine("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.AppendLine("         ,Data_Modifica= " & Agro_SQL_SaveDateTime(Date.Now) & " ")
                strSql.AppendLine("         ,Inviato = -1 ")

                strSql.AppendLine(" WHERE     (Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "')  ")

                strSql.AppendLine(" AND     Inviato >= 0 ")

            Else

                strSql.Length = 0
                strSql.AppendLine(" DELETE ")
                strSql.AppendLine(" FROM    CAC_Codifica_ProdottiAziendali ")
                strSql.AppendLine(" WHERE     (Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "')  ")

            End If

            If Elem_Cod <> 0 Then
                strSql.AppendLine(" AND     Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & " ")
            End If

            If Cod_Prodotto_Cliente <> "" Then
                strSql.AppendLine(" AND    Cod_Prodotto_Cliente = '" & Agro_SQL_SaveText(Cod_Prodotto_Cliente) & "' ")
            End If

            If Codice_GIAS <> -99 Then
                strSql.AppendLine(" AND     Codice_GIAS = " & Agro_SQL_SaveNum(Codice_GIAS) & " ")
            End If

            If Piva <> "" Then
                strSql.AppendLine(" AND    Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Cod_Articolo <> "" Then
                strSql.AppendLine(" AND    Cod_Articolo = '" & Agro_SQL_SaveText(Cod_Articolo) & "' ")
            End If

            If Tipo_Codifica <> 0 Then
                strSql.AppendLine(" AND     Tipo_Codifica = " & Agro_SQL_SaveNum(Tipo_Codifica) & " ")
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return xRisp

    End Function


    '####################################################################
    Public Function Modifica(ByVal Piva As String,
                             ByVal Tipo_Codifica As enum_Tipo_CAC_Codifica_ProdottiAziendali,
                             ByVal Elem_Cod As Integer,
                             ByVal Cod_Prodotto_Cliente As String,
                             ByVal Cod_Articolo As String,
                             ByVal Desc_Prodotto_Cliente As String,
                             ByVal Categoria_Prodotto_Cliente As String,
                             ByVal Codice_GIAS As Integer,
                             ByVal Desc_GIAS As String,
                             ByVal Note As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.CAC_Codifica_ProdottiAziendali_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            strSql.Length = 0


            strSql.AppendLine("UPDATE CAC_Codifica_ProdottiAziendali SET ")

            strSql.AppendLine(" Desc_Prodotto_Cliente = '" & Agro_SQL_SaveText(Trim(Desc_Prodotto_Cliente)) & "' ")
            strSql.AppendLine(" ,Categoria_Prodotto_Cliente = '" & Agro_SQL_SaveText(Trim(Categoria_Prodotto_Cliente)) & "' ")
            strSql.AppendLine(" ,Codice_GIAS = " & Agro_SQL_SaveNum(Trim(Codice_GIAS)) & " ")
            strSql.AppendLine(" ,Desc_GIAS = '" & Agro_SQL_SaveText(Trim(Desc_GIAS)) & "' ")
            strSql.AppendLine(" ,Note = '" & Agro_SQL_SaveText(Trim(Note)) & "' ")
            strSql.AppendLine(" ,Data_Modifica        =  " & Agro_SQL_SaveDateTime(Date.Now))
            strSql.AppendLine(" ,UserName_Modifica    = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            strSql.AppendLine(" WHERE   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            strSql.AppendLine(" AND     Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            strSql.AppendLine(" AND     Tipo_Codifica = " & Agro_SQL_SaveNum(Tipo_Codifica) & "   ")
            strSql.AppendLine(" AND     Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
            strSql.AppendLine(" AND     Cod_Prodotto_Cliente = '" & Agro_SQL_SaveText(Cod_Prodotto_Cliente) & "'   ")
            strSql.AppendLine(" AND     Cod_Articolo = '" & Agro_SQL_SaveText(Cod_Articolo) & "'   ")
            '---------------------------------------------

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return xRisp

    End Function

    '####################################################################
    Public Function ModificaAncheLaChiave(ByVal Piva As String,
                                         ByVal Tipo_Codifica As enum_Tipo_CAC_Codifica_ProdottiAziendali,
                                         ByVal Elem_Cod As Integer,
                                         ByVal Cod_Prodotto_Cliente_OLD As String,
                                         ByVal Codice_GIAS_OLD As Integer,
                                         ByVal Cod_Prodotto_Cliente_NEW As String,
                                         ByVal Desc_Prodotto_Cliente As String,
                                         ByVal Categoria_Prodotto_Cliente As String,
                                         ByVal Codice_GIAS_NEW As Integer,
                                         ByVal Desc_GIAS As String,
                                         ByVal Cod_Articolo As String,
                                         ByVal Note As String,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                         ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.CAC_Codifica_ProdottiAziendali_W.ModificaAncheLaChiave()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            strSql.Length = 0


            strSql.AppendLine("UPDATE CAC_Codifica_ProdottiAziendali SET ")

            strSql.AppendLine(" Cod_Prodotto_Cliente = '" & Agro_SQL_SaveText(Trim(Cod_Prodotto_Cliente_NEW)) & "' ")
            strSql.AppendLine(" ,Desc_Prodotto_Cliente = '" & Agro_SQL_SaveText(Trim(Desc_Prodotto_Cliente)) & "' ")
            strSql.AppendLine(" ,Categoria_Prodotto_Cliente = '" & Agro_SQL_SaveText(Trim(Categoria_Prodotto_Cliente)) & "' ")
            strSql.AppendLine(" ,Codice_GIAS = " & Agro_SQL_SaveNum(Trim(Codice_GIAS_NEW)) & " ")
            strSql.AppendLine(" ,Desc_GIAS = '" & Agro_SQL_SaveText(Trim(Desc_GIAS)) & "' ")
            strSql.AppendLine(" ,Note = '" & Agro_SQL_SaveText(Trim(Note)) & "' ")
            strSql.AppendLine(" ,Data_Modifica        =  " & Agro_SQL_SaveDateTime(Date.Now))
            strSql.AppendLine(" ,UserName_Modifica    = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            strSql.AppendLine(" WHERE   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            strSql.AppendLine(" AND     Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            strSql.AppendLine(" AND     Tipo_Codifica = " & Agro_SQL_SaveNum(Tipo_Codifica) & "   ")
            strSql.AppendLine(" AND     Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
            strSql.AppendLine(" AND     Cod_Prodotto_Cliente = '" & Agro_SQL_SaveText(Cod_Prodotto_Cliente_OLD) & "'   ")
            strSql.AppendLine(" AND     Codice_GIAS = " & Agro_SQL_SaveNum(Codice_GIAS_OLD) & "   ")
            '---------------------------------------------

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    '####################################################################
    Public Function Modifica2(ByVal Piva As String,
                              ByVal Elem_Cod As Integer,
                              ByVal Cod_Prodotto_Cliente As String,
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                              Optional ByVal Tipo_Codifica As enum_Tipo_CAC_Codifica_ProdottiAziendali = enum_Tipo_CAC_Codifica_ProdottiAziendali.NonDefinito,
                              Optional ByVal Categoria_Prodotto_Cliente As String = Nothing,
                              Optional ByVal Cod_Articolo As String = Nothing,
                              Optional ByVal Desc_Prodotto_Cliente As String = Nothing,
                              Optional ByVal Codice_GIAS As Integer? = Nothing,
                              Optional ByVal Desc_GIAS As String = Nothing,
                              Optional ByVal Note As String = Nothing,
                              Optional ByVal Data_Modifica As DateTime = #2/1/1900#,
                              Optional ByVal Username_Modifica As String = ""
                              ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.CAC_Codifica_ProdottiAziendali_W.Modifica2()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_Modifica = #2/1/1900# Then
                Data_Modifica = Date.Now
            End If

            If Username_Modifica = "" Then
                Username_Modifica = objParametri.UsernameOperazione
            End If


            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE CAC_Codifica_ProdottiAziendali ")
            strSql.AppendLine(" SET Data_Modifica =  " & Agro_SQL_SaveDateTime(Data_Modifica))
            strSql.AppendLine("   , UserName_Modifica = '" & Agro_SQL_SaveText(Username_Modifica) & "'")

            If Categoria_Prodotto_Cliente IsNot Nothing Then
                strSql.AppendLine("   , Categoria_Prodotto_Cliente = '" & Agro_SQL_SaveText(Trim(Categoria_Prodotto_Cliente)) & "' ")
            End If

            If Cod_Articolo IsNot Nothing Then
                strSql.AppendLine("   , Cod_Articolo = '" & Agro_SQL_SaveText(Trim(Cod_Articolo)) & "' ")
            End If

            If Desc_Prodotto_Cliente IsNot Nothing Then
                strSql.AppendLine("   , Desc_Prodotto_Cliente = '" & Agro_SQL_SaveText(Trim(Desc_Prodotto_Cliente)) & "' ")
            End If

            If Codice_GIAS IsNot Nothing Then
                strSql.AppendLine("   , Codice_GIAS = " & Agro_SQL_SaveNum(Codice_GIAS) & " ")
            End If

            If Desc_GIAS IsNot Nothing Then
                strSql.AppendLine("   , Desc_GIAS = '" & Agro_SQL_SaveText(Trim(Desc_GIAS)) & "' ")
            End If

            If Note IsNot Nothing Then
                strSql.AppendLine("   , Note = '" & Agro_SQL_SaveText(Trim(Note)) & "' ")
            End If

            strSql.AppendLine(" WHERE   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            strSql.AppendLine(" AND     Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            strSql.AppendLine(" AND     Tipo_Codifica = " & Agro_SQL_SaveNum(Tipo_Codifica) & "   ")
            strSql.AppendLine(" AND     Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
            strSql.AppendLine(" AND     Cod_Prodotto_Cliente = '" & Agro_SQL_SaveText(Cod_Prodotto_Cliente) & "'   ")
            '---------------------------------------------

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

End Class
