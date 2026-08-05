Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Gias_Interscambio_R
    Inherits AgronicaCoreDataProvider.DataProvider

#Region "INTERSCAMBIO Tipo 1 (Ruggeri - Ferrarini)"

    '##############################################################################################
    ''' <summary>
    ''' Legge il contenuto della tabella Anagrafica in GIAS INTERSCAMBIO
    ''' </summary>
    ''' <param name="piva">Piva della nostra installazione(opzionale = "")</param>
    ''' <param name="idAnagrafica">(opzionale = 0)</param>
    ''' <param name="tipoRapporto">(opzionale = "")</param>
    ''' <param name="codContatto_GIAS">(opzionale = "")</param>
    ''' <param name="codIndirizzo_GIAS">(opzionale = 0)</param>
    ''' <param name="codAnag_ALTRO">(opzionale = "")</param>
    ''' <param name="codIndirizzo_ALTRO">(opzionale = "")</param>
    ''' <param name="partitaIvaContatto">Partita Iva del contatto (opzionale = "")</param>
    ''' <param name="codiceFiscaleContatto">Codice Fiscale del contatto (opzionale = "")</param>
    ''' <param name="tipoRappStringIN">verrà costruita come Tipo_Rapporto IN (contenuto variabile) (opzionale = "")</param>
    ''' <param name="xFiltroAggiuntivo">(opzionale = "")</param>
    ''' <param name="xOrderBy">(opzionale = "")</param>
    ''' <param name="objParametri"></param>
    ''' <returns>DataTable contenente i dati della tabella</returns>
    ''' <remarks></remarks>
    Public Function LeggiAnagrafica(ByVal piva As String,
                                    ByVal idAnagrafica As Integer,
                                    ByVal tipoRapporto As String,
                                    ByVal codContatto_GIAS As String,
                                    ByVal codIndirizzo_GIAS As Integer,
                                    ByVal codAnag_ALTRO As String,
                                    ByVal codIndirizzo_ALTRO As String,
                                    ByVal partitaIvaContatto As String,
                                    ByVal codiceFiscaleContatto As String,
                                    ByVal tipoRappStringIN As String,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreParametri
                                    ) As DataTable

        '====================================================================================
        'Parametri opzionali :
        '   piva = ""
        '   idAnagrafica = 0
        '   tipoRapporto = ""
        '   codContatto_GIAS = ""
        '   codIndirizzo_GIAS = 0
        '   codAnag_ALTRO = ""
        '   codIndirizzo_ALTRO = ""
        '   partitaIvaContatto = ""
        '   codiceFiscaleContatto = ""
        '====================================================================================

        Const nomeRoutine = "Gias_Interscambio_R.LeggiAnagrafica()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM Anagrafica ")
            stb.AppendLine(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")


            If piva <> "" Then
                stb.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            If idAnagrafica <> 0 Then
                stb.AppendLine(" AND Id_Anag = " & Agro_SQL_SaveNum(idAnagrafica) & " ")
            End If

            If tipoRapporto <> "" Then
                stb.AppendLine(" AND Tipo_Rapporto = '" & Agro_SQL_SaveText(tipoRapporto) & "' ")
            End If

            If tipoRappStringIN <> "" Then
                stb.AppendLine(" AND Tipo_Rapporto IN ( " & Agro_SQL_Save_Clausola_IN(tipoRappStringIN, True) & " ) ")
            End If

            If codContatto_GIAS <> "" Then
                stb.AppendLine(" AND Cod_Contatto_GIAS = '" & Agro_SQL_SaveText(codContatto_GIAS) & "' ")
            End If

            If codIndirizzo_GIAS <> 0 Then
                stb.AppendLine(" AND Cod_Indirizzo_GIAS = " & Agro_SQL_SaveNum(codIndirizzo_GIAS) & " ")
            End If

            If codAnag_ALTRO <> "" Then
                stb.AppendLine(" AND Cod_Anag_ALTRO = '" & Agro_SQL_SaveText(codAnag_ALTRO) & "' ")
            End If

            If codIndirizzo_ALTRO <> "" Then
                stb.AppendLine(" AND Cod_Indirizzo_ALTRO = '" & Agro_SQL_SaveText(codIndirizzo_ALTRO) & "' ")
            End If

            If partitaIvaContatto <> "" Then
                stb.AppendLine(" AND Partita_Iva = '" & Agro_SQL_SaveText(partitaIvaContatto) & "' ")
            End If

            If codiceFiscaleContatto <> "" Then
                stb.AppendLine(" AND Codice_Fiscale = '" & Agro_SQL_SaveText(codiceFiscaleContatto) & "' ")
            End If


            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '##############################################################################################
    Public Function Leggi_Contatto_GIAS_From_ALTRO(ByVal tipoRapporto As String,
                                                   ByVal tipoRappStringIN As String,
                                                   ByVal Cod_Anag_ALTRO As String,
                                                   ByVal Cod_Indirizzo_ALTRO As String,
                                                   ByRef Cod_Contatto_GIAS As String,
                                                   ByRef Cod_Indirizzo_GIAS As Integer,
                                                   ByRef objParametri As AgronicaCoreParametri
                                                   ) As Integer

        Const nomeRoutine = "Gias_Interscambio_R.Leggi_Contatto_ALTRO_Interscambio()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim numElementi As Integer = 0

        Try
            dt = LeggiAnagrafica("", 0, tipoRapporto, "", 0,
                                 Cod_Anag_ALTRO, Cod_Indirizzo_ALTRO,
                                 "", "", tipoRappStringIN, "", "", objParametri)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                numElementi = dt.Rows.Count
                Cod_Contatto_GIAS = If(IsDBNull(dt.Rows(0).Item("Cod_Contatto_GIAS")), "", CStr(dt.Rows(0).Item("Cod_Contatto_GIAS")))
                Cod_Indirizzo_GIAS = CInt(If(IsDBNull(dt.Rows(0).Item("Cod_Indirizzo_GIAS")), 0, CInt(dt.Rows(0).Item("Cod_Indirizzo_GIAS"))))
            Else
                numElementi = 0
                Cod_Contatto_GIAS = ""
                Cod_Indirizzo_GIAS = 0
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Cod_Contatto_GIAS = ""
            Cod_Indirizzo_GIAS = 0
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return numElementi

    End Function

    '##############################################################################################
    Public Function Leggi_Contatto_ALTRO_From_GIAS(ByVal tipoRapporto As String,
                                                   ByVal tipoRappStringIN As String,
                                                   ByRef Cod_Anag_ALTRO As String,
                                                   ByRef Cod_Indirizzo_ALTRO As String,
                                                   ByVal Cod_Contatto_GIAS As String,
                                                   ByVal Cod_Indirizzo_GIAS As Integer,
                                                   ByRef objParametri As AgronicaCoreParametri
                                                   ) As Integer

        Const nomeRoutine = "Gias_Interscambio_R.Leggi_Contatto_ALTRO_Interscambio()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim numElementi As Integer = 0

        Try
            dt = LeggiAnagrafica("", 0, tipoRapporto,
                                 Cod_Contatto_GIAS, Cod_Indirizzo_GIAS,
                                 "", "", "", "", tipoRappStringIN, "", "", objParametri)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                numElementi = dt.Rows.Count
                Cod_Anag_ALTRO = CStr(dt.Rows(0).Item("Cod_Anag_ALTRO"))
                Cod_Indirizzo_ALTRO = CStr(dt.Rows(0).Item("Cod_Indirizzo_ALTRO"))
            Else
                numElementi = 0
                Cod_Anag_ALTRO = ""
                Cod_Indirizzo_ALTRO = ""
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Cod_Anag_ALTRO = ""
            Cod_Indirizzo_ALTRO = ""
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return numElementi

    End Function

    '##############################################################################################
    Public Function Leggi_Piva_GIAS_From_ALTRO(ByVal Tipo_Rapporto As String,
                                               ByVal TipoRappStringIN As String,
                                               ByVal Cod_Anag_ALTRO As String,
                                               ByVal Cod_Indirizzo_ALTRO As String,
                                               ByRef Cod_Contatto_GIAS As String,
                                               ByRef Cod_Indirizzo_GIAS As Integer,
                                               ByRef Piva As String,
                                               ByRef objParametri As AgronicaCoreParametri
                                               ) As Integer

        Const nomeRoutine = "Gias_Interscambio_R.Leggi_Piva_GIAS_From_ALTRO()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim numElementi As Integer = 0

        Try
            dt = LeggiAnagrafica("", 0, Tipo_Rapporto, "", 0,
                                 Cod_Anag_ALTRO, Cod_Indirizzo_ALTRO,
                                 "", "", TipoRappStringIN, "", "", objParametri)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                numElementi = dt.Rows.Count
                Cod_Contatto_GIAS = If(IsDBNull(dt.Rows(0).Item("Cod_Contatto_GIAS")), "", CStr(dt.Rows(0).Item("Cod_Contatto_GIAS")))
                Cod_Indirizzo_GIAS = CInt(If(IsDBNull(dt.Rows(0).Item("Cod_Indirizzo_GIAS")), 0, CInt(dt.Rows(0).Item("Cod_Indirizzo_GIAS"))))
                Piva = If(IsDBNull(dt.Rows(0).Item("Partita_Iva")), "", CStr(dt.Rows(0).Item("Partita_Iva")))
            Else
                numElementi = 0
                Cod_Contatto_GIAS = ""
                Cod_Indirizzo_GIAS = 0
                Piva = ""
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Cod_Contatto_GIAS = ""
            Cod_Indirizzo_GIAS = 0
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return numElementi

    End Function

    '##############################################################################################
    ''' <summary>
    ''' Legge il contenuto della tabella Parametri in GIAS INTERSCAMBIO
    ''' </summary>
    ''' <param name="Id_Parametro">(opzionale = 0)</param>
    ''' <param name="Tipo">Tipo parametro: utilizzare le costanti INTERSCAMBIO_PARAMETRI_* (opzionale = "")</param>
    ''' <param name="Codice_GIAS">Codice del parametro in GIAS (opzionale = "")</param>
    ''' <param name="Codice_ALTRO">Codice del parametro in un altro programma (opzionale = "")</param>
    ''' <param name="xFiltroAggiuntivo">Filtro aggiuntivo per la query di select (opzionale = "")</param>
    ''' <param name="xOrderBy">Criteri di ordinamento (opzionale = "")</param>
    ''' <param name="objParametri">AgronicaCoreParametri per la connessione</param>
    ''' <returns>DataTable contenente i dati della tabella</returns>
    ''' <remarks>Per il tipo è possibile utilizzare le costanti personalizzate che cominciano per INTERSCAMBIO_PARAMETRI_*</remarks>
    Public Function LeggiParametri(ByVal Id_Parametro As Integer,
                                   ByVal Tipo As String,
                                   ByVal Codice_GIAS As String,
                                   ByVal Codice_ALTRO As String,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByVal xOrderBy As String,
                                   ByRef objParametri As AgronicaCoreParametri
                                   ) As DataTable

        '====================================================================================
        'Parametri opzionali :
        '   Tipo = ""
        '   Codice_GIAS = ""
        '   Codice_ALTRO = ""
        '====================================================================================
        '
        '   Per il tipo è possibile utilizzare le costanti personalizzate che cominciano per INTERSCAMBIO_PARAMETRI_*
        '
        '====================================================================================

        Const nomeRoutine = "Gias_Interscambio_R.LeggiParametri()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM Parametri ")
            stb.AppendLine(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")

            If Id_Parametro <> 0 Then
                stb.AppendLine(" AND Id_Param = " & Agro_SQL_SaveNum(Id_Parametro) & " ")
            End If

            If Tipo <> "" Then
                stb.AppendLine(" AND Tipo = '" & Agro_SQL_SaveText(Tipo) & "' ")
            End If

            If Codice_GIAS <> "" Then
                stb.AppendLine(" AND Codice_GIAS = '" & Agro_SQL_SaveText(Codice_GIAS) & "' ")
            End If

            If Codice_ALTRO <> "" Then
                stb.AppendLine(" AND Codice_ALTRO = '" & Agro_SQL_SaveText(Codice_ALTRO) & "' ")
            End If


            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '##############################################################################################
    Public Function Leggi_Parametro_GIAS_From_ALTRO(ByVal Tipo As String,
                                                    ByRef Codice_GIAS As String,
                                                    ByVal Codice_ALTRO As String,
                                                    ByRef objParametri As AgronicaCoreParametri
                                                    ) As Integer

        Const nomeRoutine = "Gias_Interscambio_R.Leggi_Parametro_GIAS_From_ALTRO()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim numElementi As Integer = 0

        Try
            dt = LeggiParametri(0, Tipo, "", Codice_ALTRO, "", "", objParametri)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                numElementi = dt.Rows.Count
                Codice_GIAS = CStr(IIf(IsDBNull(dt.Rows(0).Item("Codice_GIAS")), "", dt.Rows(0).Item("Codice_GIAS")))
            Else
                numElementi = 0
                Codice_GIAS = ""
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Codice_GIAS = ""
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return numElementi

    End Function

    '##############################################################################################
    Public Function Leggi_Parametro_ALTRO_From_GIAS(ByVal Tipo As String,
                                                    ByVal Codice_GIAS As String,
                                                    ByRef Codice_ALTRO As String,
                                                    ByRef objParametri As AgronicaCoreParametri,
                                                    Optional ByVal xFiltroAggiuntivo As String = ""
                                                    ) As Integer

        Const nomeRoutine = "Gias_Interscambio_R.Leggi_Parametro_ALTRO_From_GIAS()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim numElementi As Integer = 0

        Try
            dt = LeggiParametri(0, Tipo, Codice_GIAS, "", xFiltroAggiuntivo, "", objParametri)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                numElementi = dt.Rows.Count
                Codice_ALTRO = CStr(dt.Rows(0).Item("Codice_ALTRO"))
            Else
                numElementi = 0
                Codice_ALTRO = ""
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Codice_ALTRO = ""
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return numElementi

    End Function

    '##############################################################################################
    ''' <summary>
    ''' Legge il contenuto della tabella Prodotti in GIAS INTERSCAMBIO
    ''' </summary>
    ''' <param name="Id_Prodotto">(opzionale = 0)</param>
    ''' <param name="Elem_Cod">Codice categoria GIAS (opzionale = 0)</param>
    ''' <param name="Cod_Prodotto_Cliente">Codice prodotto altro programma (opzionale = "")</param>
    ''' <param name="Categoria_Prodotto_Cliente">Codice categoria prodotto altro programma (opzionale = "")</param>
    ''' <param name="Codice_GIAS">Codice prodotto GIAS (opzionale = 0)</param>
    ''' <param name="Piva">(opzionale = "")</param>
    ''' <param name="Cod_Articolo">Codice Articolo su GIAS/altro programma (opzionale = "")</param>
    ''' <param name="Tipo_Codifica">enum_Tipo_CAC_Codifica_ProdottiAziendali (opzionale = 0)</param>
    ''' <param name="xFiltroAggiuntivo">Filtro aggiuntivo per la query di select (opzionale = "")</param>
    ''' <param name="xOrderBy">Criteri di ordinamento (opzionale = "")</param>
    ''' <param name="objParametri">AgronicaCoreParametri per la connessione</param>
    ''' <returns>DataTable contenente i dati della tabella</returns>
    ''' <remarks></remarks>
    Public Function LeggiProdotti(ByVal Id_Prodotto As Integer,
                                  ByVal Elem_Cod As Integer,
                                  ByVal Cod_Prodotto_Cliente As String,
                                  ByVal Categoria_Prodotto_Cliente As String,
                                  ByVal Codice_GIAS As Integer,
                                  ByVal Piva As String,
                                  ByVal Cod_Articolo As String,
                                  ByVal Tipo_Codifica As enum_Tipo_CAC_Codifica_ProdottiAziendali,
                                  ByVal xFiltroAggiuntivo As String,
                                  ByVal xOrderBy As String,
                                  ByRef objParametri As AgronicaCoreParametri
                                  ) As DataTable

        '====================================================================================
        'Parametri opzionali :
        '   Id_Prodotto = 0
        '   Elem_Cod = 0
        '   Cod_Prodotto_Cliente = ""
        '   Categoria_Prodotto_Cliente = ""
        '   Codice_GIAS = 0
        '   Piva = ""
        '   Cod_Articolo = ""
        '   Tipo_Codifica = 0
        '====================================================================================

        Const nomeRoutine = "Gias_Interscambio_R.LeggiProdotti()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            'Piva e Piva_SuperUser se li filtro devo vedere anche dove è stringa vuota perché la controparte potrebbe non averla scritta

            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM Prodotti ")
            stb.AppendLine(" WHERE ( Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' OR Piva_SuperUser = '' ) ")


            If Id_Prodotto <> 0 Then
                stb.AppendLine(" AND Id_Prod = " & Agro_SQL_SaveNum(Id_Prodotto) & " ")
            End If

            If Elem_Cod <> 0 Then
                stb.AppendLine(" AND Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & " ")
            End If

            If Cod_Prodotto_Cliente <> "" Then
                stb.AppendLine(" AND Cod_Prodotto_Cliente = '" & Agro_SQL_SaveText(Cod_Prodotto_Cliente) & "' ")
            End If

            If Categoria_Prodotto_Cliente <> "" Then
                stb.AppendLine(" AND Categoria_Prodotto_Cliente = '" & Agro_SQL_SaveText(Categoria_Prodotto_Cliente) & "' ")
            End If

            If Codice_GIAS <> 0 Then
                stb.AppendLine(" AND Codice_GIAS = " & Agro_SQL_SaveNum(Codice_GIAS) & " ")
            End If

            If Piva <> "" Then
                stb.AppendLine(" AND ( Piva = '" & Agro_SQL_SaveText(Piva) & "' OR Piva = '' ) ")
            End If

            If Cod_Articolo <> "" Then
                stb.AppendLine(" AND Cod_Articolo = '" & Agro_SQL_SaveText(Cod_Articolo) & "' ")
            End If

            If Tipo_Codifica <> enum_Tipo_CAC_Codifica_ProdottiAziendali.NonDefinito Then
                stb.AppendLine(" AND Tipo_Codifica = " & Agro_SQL_SaveNum(Tipo_Codifica) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '##############################################################################################
    Public Function Leggi_Prodotto_GIAS_From_ALTRO(ByRef Elem_Cod As Integer,
                                                   ByRef Codice_GIAS As Integer,
                                                   ByRef Id_Prod As Long,
                                                   ByVal Categoria_Prodotto_Cliente As String,
                                                   ByVal Cod_Prodotto_Cliente As String,
                                                   ByVal Piva As String,
                                                   ByVal Cod_Articolo As String,
                                                   ByRef objParametri As AgronicaCoreParametri
                                                   ) As Integer

        Const nomeRoutine = "Gias_Interscambio_R.Leggi_Prodotto_GIAS_From_ALTRO()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim numElementi As Integer = 0

        Try
            dt = LeggiProdotti(Id_Prodotto:=0,
                               Elem_Cod:=0,
                               Cod_Prodotto_Cliente:=Cod_Prodotto_Cliente,
                               Categoria_Prodotto_Cliente:=Categoria_Prodotto_Cliente,
                               Codice_GIAS:=0,
                               Piva:=Piva,
                               Cod_Articolo:=Cod_Articolo,
                               Tipo_Codifica:=enum_Tipo_CAC_Codifica_ProdottiAziendali.NonDefinito,
                               xFiltroAggiuntivo:="",
                               xOrderBy:="",
                               objParametri:=objParametri)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                numElementi = dt.Rows.Count
                Elem_Cod = CInt(dt.Rows(0).Item("Elem_Cod"))
                Codice_GIAS = CInt(dt.Rows(0).Item("Codice_GIAS"))
                Id_Prod = CLng(dt.Rows(0).Item("Id_Prod"))
            Else
                numElementi = 0
                Elem_Cod = 0
                Codice_GIAS = 0
                Id_Prod = 0
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Elem_Cod = 0
            Codice_GIAS = 0
            Id_Prod = 0
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return numElementi

    End Function

#End Region

#Region "INTERSCAMBIO Tipo 2 (Aboca)"

    '##############################################################################################
    ''' <summary>
    ''' Legge il contenuto della tabella Codici_Anagrafiche in GIAS INTERSCAMBIO
    ''' </summary>
    ''' <param name="piva">Piva della nostra installazione(opzionale = "")</param>
    ''' <param name="idAnagrafica">(opzionale = 0)</param>
    ''' <param name="codContatto_ALTRO">(opzionale = "")</param>
    ''' <param name="codContatto_GIAS">(opzionale = "")</param>
    ''' <param name="xFiltroAggiuntivo">(opzionale = "")</param>
    ''' <param name="xOrderBy">(opzionale = "")</param>
    ''' <param name="objParametri"></param>
    ''' <param name="suffissoColonnaAltro">suffisso sul nome della colonna per l'altro gestionale (ad es: "SAP" che verrà aggiunto a "Cod_Contatto_" (se non specificato = "ALTRO")</param>
    ''' <param name="dataValidita">restituisce solo i valori validi a quella data</param>
    ''' <param name="cuaa">Usato per modalità Demetra: il Cod_Contatto_Altro da solo non sempre è univoco, ma a volte va usato in abbinato con il cuaa</param>
    ''' <returns>DataTable contenente i dati della tabella</returns>
    Public Function LeggiCodiciAnagrafiche(ByVal piva As String,
                                           ByVal idAnagrafica As Integer,
                                           ByVal codContatto_ALTRO As String,
                                           ByVal codContatto_GIAS As String,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByVal xOrderBy As String,
                                           ByRef objParametri As AgronicaCoreParametri,
                                           Optional ByVal suffissoColonnaAltro As String = "ALTRO",
                                           Optional ByVal dataValidita As Date = #2/1/1900#,
                                           Optional ByVal cuaa As String = Nothing
                                           ) As DataTable

        '====================================================================================
        'Parametri opzionali :
        '   piva = ""
        '   idAnagrafica = 0
        '   codContatto_GIAS = ""
        '   codContatto_ALTRO = ""
        '====================================================================================

        Const nomeRoutine = "Gias_Interscambio_R.LeggiCodiciAnagrafiche()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM Codici_Anagrafiche ")
            stb.AppendLine(" WHERE 1 = 1 ")


            If piva <> "" Then
                stb.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            If idAnagrafica <> 0 Then
                stb.AppendLine(" AND Id_Anag = " & Agro_SQL_SaveNum(idAnagrafica) & " ")
            End If

            If codContatto_GIAS <> "" Then
                stb.AppendLine(" AND Cod_Contatto_GIAS = '" & Agro_SQL_SaveText(codContatto_GIAS) & "' ")
            End If

            If codContatto_ALTRO <> "" Then
                stb.AppendLine(" AND [Cod_Contatto_" & suffissoColonnaAltro & "] = '" & Agro_SQL_SaveText(codContatto_ALTRO) & "' ")
            End If

            If dataValidita <> #2/1/1900# Then
                stb.AppendLine(" AND Validita_Inizio <= " & Agro_SQL_SaveDate(dataValidita) & " ")
                stb.AppendLine(" AND Validita_Fine >= " & Agro_SQL_SaveDate(dataValidita) & " ")
            End If

            If cuaa IsNot Nothing Then
                stb.AppendLine(" AND CuaaInput = '" & Agro_SQL_SaveText(cuaa) & "' ")
            End If


            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '##############################################################################################
    Public Function Leggi_CodiceContatto_GIAS_From_ALTRO(ByVal piva As String,
                                                         ByRef idAnag As Integer,
                                                         ByVal codContatto_ALTRO As String,
                                                         ByRef codContatto_GIAS As String,
                                                         ByRef pivaContattoInterscambio As String,
                                                         ByRef objParametri As AgronicaCoreParametri,
                                                         Optional ByVal suffissoColonnaAltro As String = "ALTRO",
                                                         Optional ByVal dataValidita As Date = #2/1/1900#,
                                                         Optional ByVal cuaa As String = Nothing
                                                         ) As Integer

        Const nomeRoutine As String = "Gias_Interscambio_R.Leggi_CodiceContatto_GIAS_From_ALTRO()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim numElementi As Integer = 0

        Try
            dt = LeggiCodiciAnagrafiche(piva, 0, codContatto_ALTRO, "",
                                        "", "", objParametri,
                                        suffissoColonnaAltro, dataValidita, cuaa)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                numElementi = dt.Rows.Count
                codContatto_GIAS = Trim(If(IsDBNull(dt.Rows(0).Item("Cod_Contatto_GIAS")), "", CStr(dt.Rows(0).Item("Cod_Contatto_GIAS"))))
                pivaContattoInterscambio = Trim(If(IsDBNull(dt.Rows(0).Item("Piva")), "", CStr(dt.Rows(0).Item("Piva"))))
                idAnag = CInt(dt.Rows(0).Item("Id_Anag"))
            Else
                numElementi = 0
                codContatto_GIAS = ""
                pivaContattoInterscambio = ""
                idAnag = 0
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            codContatto_GIAS = ""
            idAnag = 0
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return numElementi

    End Function

    '##############################################################################################
    Public Function Leggi_CodiceContatto_ALTRO_From_GIAS(ByVal piva As String,
                                                         ByRef idAnag As Integer,
                                                         ByRef codContatto_ALTRO As String,
                                                         ByVal codContatto_GIAS As String,
                                                         ByRef objParametri As AgronicaCoreParametri,
                                                         Optional ByVal suffissoColonnaAltro As String = "ALTRO",
                                                         Optional ByVal dataValidita As Date = #2/1/1900#,
                                                         Optional ByVal cuaa As String = Nothing
                                                         ) As Integer

        Const nomeRoutine = "Gias_Interscambio_R.Leggi_CodiceContatto_ALTRO_From_GIAS()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim numElementi As Integer = 0

        Try
            dt = LeggiCodiciAnagrafiche(piva, 0, "", codContatto_GIAS,
                                        "", "", objParametri,
                                        suffissoColonnaAltro, dataValidita, cuaa)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                numElementi = dt.Rows.Count
                codContatto_ALTRO = CStr(dt.Rows(0).Item("Cod_Contatto_" & suffissoColonnaAltro))
                idAnag = CInt(dt.Rows(0).Item("Id_Anag"))
            Else
                numElementi = 0
                codContatto_ALTRO = ""
                idAnag = 0
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            codContatto_ALTRO = ""
            idAnag = 0
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return numElementi

    End Function

    '##############################################################################################
    ''' <summary>
    ''' Legge il contenuto della tabella Codici_AnagraficheConIndirizzi in GIAS INTERSCAMBIO
    ''' </summary>
    ''' <param name="piva">Piva della nostra installazione(opzionale = "")</param>
    ''' <param name="idAnagrafica">(opzionale = 0)</param>
    ''' <param name="codContatto_ALTRO">(opzionale = "")</param>
    ''' <param name="codIndirizzo_ALTRO">(opzionale = "")</param>
    ''' <param name="codContatto_GIAS">(opzionale = "")</param>
    ''' <param name="codIndirizzo_GIAS">(opzionale = 0)</param>
    ''' <param name="xFiltroAggiuntivo">(opzionale = "")</param>
    ''' <param name="xOrderBy">(opzionale = "")</param>
    ''' <param name="objParametri"></param>
    ''' <param name="suffissoColonnaAltro">suffisso sul nome della colonna per l'altro gestionale (ad es: "SAP" che verrà aggiunto a "Cod_Contatto_" (se non specificato = "ALTRO")</param>
    ''' <param name="dataValidita">restituisce solo i valori validi a quella data</param>
    ''' <returns>DataTable contenente i dati della tabella</returns>
    Public Function LeggiCodiciAnagraficheConIndirizzi(ByVal piva As String,
                                                       ByVal idAnagrafica As Integer,
                                                       ByVal codContatto_ALTRO As String,
                                                       ByVal codIndirizzo_ALTRO As String,
                                                       ByVal codContatto_GIAS As String,
                                                       ByVal codIndirizzo_GIAS As Integer,
                                                       ByVal xFiltroAggiuntivo As String,
                                                       ByVal xOrderBy As String,
                                                       ByRef objParametri As AgronicaCoreParametri,
                                                       Optional ByVal suffissoColonnaAltro As String = "ALTRO",
                                                       Optional ByVal dataValidita As Date = #2/1/1900#
                                                       ) As DataTable

        '====================================================================================
        'Parametri opzionali :
        '   piva = ""
        '   idAnagrafica = 0
        '   codContatto_ALTRO = ""
        '   codIndirizzo_ALTRO = ""
        '   codContatto_GIAS = ""
        '   codIndirizzo_GIAS = 0
        '====================================================================================

        Const nomeRoutine = "Gias_Interscambio_R.LeggiCodiciAnagraficheConIndirizzi()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM Codici_AnagraficheConIndirizzi ")
            stb.AppendLine(" WHERE 1 = 1 ")


            If piva <> "" Then
                stb.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            If idAnagrafica <> 0 Then
                stb.AppendLine(" AND Id_AnagInd = " & Agro_SQL_SaveNum(idAnagrafica) & " ")
            End If

            If codContatto_ALTRO <> "" Then
                stb.AppendLine(" AND [Cod_Contatto_" & suffissoColonnaAltro & "] = '" & Agro_SQL_SaveText(codContatto_ALTRO) & "' ")
            End If

            If codIndirizzo_ALTRO <> "" Then
                stb.AppendLine(" AND [Cod_Indirizzo_" & suffissoColonnaAltro & "] = '" & Agro_SQL_SaveText(codIndirizzo_ALTRO) & "' ")
            End If

            If codContatto_GIAS <> "" Then
                stb.AppendLine(" AND Cod_Contatto_GIAS = '" & Agro_SQL_SaveText(codContatto_GIAS) & "' ")
            End If

            If codIndirizzo_GIAS <> 0 Then
                stb.AppendLine(" AND Cod_Indirizzo_GIAS = " & Agro_SQL_SaveNum(codIndirizzo_GIAS) & " ")
            End If

            If dataValidita <> #2/1/1900# Then
                stb.AppendLine(" AND Validita_Inizio <= " & Agro_SQL_SaveDate(dataValidita) & " ")
                stb.AppendLine(" AND Validita_Fine >= " & Agro_SQL_SaveDate(dataValidita) & " ")
            End If


            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '##############################################################################################
    Public Function Leggi_CodiceContattoConIndirizzo_GIAS_From_ALTRO(ByVal piva As String,
                                                                     ByRef idAnag As Integer,
                                                                     ByVal codContatto_ALTRO As String,
                                                                     ByVal codIndirizzo_ALTRO As String,
                                                                     ByRef codContatto_GIAS As String,
                                                                     ByRef codIndirizzo_GIAS As Integer,
                                                                     ByRef pivaContattoInterscambio As String,
                                                                     ByRef objParametri As AgronicaCoreParametri,
                                                                     Optional ByVal suffissoColonnaAltro As String = "ALTRO",
                                                                     Optional ByVal dataValidita As Date = #2/1/1900#
                                                                     ) As Integer

        Const nomeRoutine As String = "Gias_Interscambio_R.Leggi_CodiceContattoConIndirizzo_GIAS_From_ALTRO()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim numElementi As Integer = 0

        Try
            dt = LeggiCodiciAnagraficheConIndirizzi(piva, 0,
                                                    codContatto_ALTRO, codIndirizzo_ALTRO,
                                                    "", 0,
                                                    "", "", objParametri,
                                                    suffissoColonnaAltro, dataValidita)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                numElementi = dt.Rows.Count
                codContatto_GIAS = Trim(If(IsDBNull(dt.Rows(0).Item("Cod_Contatto_GIAS")), "", CStr(dt.Rows(0).Item("Cod_Contatto_GIAS"))))
                codIndirizzo_GIAS = If(IsDBNull(dt.Rows(0).Item("Cod_Indirizzo_GIAS")), 0, CInt(dt.Rows(0).Item("Cod_Indirizzo_GIAS")))
                pivaContattoInterscambio = Trim(If(IsDBNull(dt.Rows(0).Item("Piva")), "", CStr(dt.Rows(0).Item("Piva"))))
                idAnag = CInt(dt.Rows(0).Item("Id_AnagInd"))
            Else
                numElementi = 0
                codContatto_GIAS = ""
                codIndirizzo_GIAS = 0
                pivaContattoInterscambio = ""
                idAnag = 0
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            codContatto_GIAS = ""
            codIndirizzo_GIAS = 0
            idAnag = 0
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return numElementi

    End Function

    '##############################################################################################
    Public Function Leggi_CodiceContattoConIndirizzo_ALTRO_From_GIAS(ByVal piva As String,
                                                                     ByRef idAnag As Integer,
                                                                     ByRef codContatto_ALTRO As String,
                                                                     ByRef codIndirizzo_ALTRO As String,
                                                                     ByVal codContatto_GIAS As String,
                                                                     ByVal codIndirizzo_GIAS As Integer,
                                                                     ByRef objParametri As AgronicaCoreParametri,
                                                                     Optional ByVal suffissoColonnaAltro As String = "ALTRO",
                                                                     Optional ByVal dataValidita As Date = #2/1/1900#,
                                                                     Optional ByVal xFiltroAggiuntivo As String = ""
                                                                     ) As Integer

        Const nomeRoutine = "Gias_Interscambio_R.Leggi_CodiceContattoConIndirizzo_ALTRO_From_GIAS()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim numElementi As Integer = 0

        Try
            dt = LeggiCodiciAnagraficheConIndirizzi(piva, 0,
                                                    "", "",
                                                    codContatto_GIAS, codIndirizzo_GIAS,
                                                    xFiltroAggiuntivo, "", objParametri,
                                                    suffissoColonnaAltro, dataValidita)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                numElementi = dt.Rows.Count
                codContatto_ALTRO = Trim(If(IsDBNull(dt.Rows(0).Item("Cod_Contatto_" & suffissoColonnaAltro)), "", CStr(dt.Rows(0).Item("Cod_Contatto_" & suffissoColonnaAltro))))
                codIndirizzo_ALTRO = Trim(If(IsDBNull(dt.Rows(0).Item("Cod_Indirizzo_" & suffissoColonnaAltro)), "", CStr(dt.Rows(0).Item("Cod_Indirizzo_" & suffissoColonnaAltro))))
                idAnag = CInt(dt.Rows(0).Item("Id_AnagInd"))
            Else
                numElementi = 0
                codContatto_ALTRO = ""
                codIndirizzo_ALTRO = ""
                idAnag = 0
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            codContatto_ALTRO = ""
            codIndirizzo_ALTRO = ""
            idAnag = 0
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return numElementi

    End Function


    '##############################################################################################
    ''' <summary>
    ''' Legge il contenuto della tabella Codici_Attivita in GIAS INTERSCAMBIO
    ''' </summary>
    ''' <param name="piva">Piva della nostra installazione(opzionale = "")</param>
    ''' <param name="idAttivita">(opzionale = 0)</param>
    ''' <param name="tipo">Tipologia della risorsa: utilizzare le costanti INTERSCAMBIO_ATTIVITA_* (opzionale = "")</param>
    ''' <param name="codAttivita_GIAS">(opzionale = "")</param>
    ''' <param name="codAttivita_ALTRO">(opzionale = "")</param>
    ''' <param name="codCentroLavoro_ALTRO">(opzionale = "")</param>
    ''' <param name="xFiltroAggiuntivo">(opzionale = "")</param>
    ''' <param name="xOrderBy">(opzionale = "")</param>
    ''' <param name="objParametri"></param>
    ''' <param name="suffissoColonnaAltro">suffisso sul nome della colonna per l'altro gestionale (ad es: "SAP" che verrà aggiunto a "Cod_Attivita_" (se non specificato = "ALTRO")</param>
    ''' <param name="dataValidita">restituisce solo i valori validi a quella data</param>
    ''' <returns>DataTable contenente i dati della tabella</returns>
    ''' <remarks>Per il tipo è possibile utilizzare le costanti personalizzate che cominciano per INTERSCAMBIO_ATTIVITA_*</remarks>
    Public Function LeggiCodiciAttivita(ByVal piva As String,
                                        ByVal idAttivita As Integer,
                                        ByVal tipo As String,
                                        ByVal codAttivita_GIAS As Integer,
                                        ByVal codAttivita_ALTRO As String,
                                        ByVal codCentroLavoro_ALTRO As String,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreParametri,
                                        Optional ByVal suffissoColonnaAltro As String = "ALTRO",
                                        Optional ByVal dataValidita As Date = #2/1/1900#
                                        ) As DataTable

        '====================================================================================
        'Parametri opzionali :
        '   piva = ""
        '   idAttivita = 0
        '   tipo = ""
        '   codAttivita_GIAS = 0
        '   codAttivita_ALTRO = ""
        '   codCentroLavoro_ALTRO = ""
        '====================================================================================

        Const nomeRoutine = "Gias_Interscambio_R.LeggiCodiciAttivita()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM Codici_Attivita ")
            stb.AppendLine(" WHERE 1 = 1 ")


            If piva <> "" Then
                stb.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            If idAttivita <> 0 Then
                stb.AppendLine(" AND Id_Attivita = " & Agro_SQL_SaveNum(idAttivita) & " ")
            End If

            If tipo <> "" Then
                stb.AppendLine(" AND Tipo = '" & Agro_SQL_SaveText(tipo) & "' ")
            End If

            If codAttivita_GIAS <> 0 Then
                stb.AppendLine(" AND Cod_Attivita_GIAS = " & Agro_SQL_SaveNum(codAttivita_GIAS) & " ")
            End If

            If codAttivita_ALTRO <> "" Then
                stb.AppendLine(" AND [Cod_Attivita_" & suffissoColonnaAltro & "] = '" & Agro_SQL_SaveText(codAttivita_ALTRO) & "' ")
            End If

            If codCentroLavoro_ALTRO <> "" Then
                stb.AppendLine(" AND [Cod_CentroLavoro_" & suffissoColonnaAltro & "] = '" & Agro_SQL_SaveText(codCentroLavoro_ALTRO) & "' ")
            End If

            If dataValidita <> #2/1/1900# Then
                stb.AppendLine(" AND Validita_Inizio <= " & Agro_SQL_SaveDate(dataValidita) & " ")
                stb.AppendLine(" AND Validita_Fine >= " & Agro_SQL_SaveDate(dataValidita) & " ")
            End If


            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '##############################################################################################
    ''' <summary>
    ''' Legge il contenuto della tabella Codici_Parametri in GIAS INTERSCAMBIO
    ''' </summary>
    ''' <param name="piva">Piva dell'impresa Gias/superUser a seconda del tipo (opzionale = "")</param>
    ''' <param name="idParametro">(opzionale = 0)</param>
    ''' <param name="tipo">Tipo parametro: utilizzare le costanti INTERSCAMBIO_PARAMETRI_* (opzionale = "")</param>
    ''' <param name="codice_GIAS">Codice del parametro in GIAS (opzionale = "")</param>
    ''' <param name="codice_ALTRO">Codice del parametro in un altro programma (opzionale = "")</param>
    ''' <param name="xFiltroAggiuntivo">Filtro aggiuntivo per la query di select (opzionale = "")</param>
    ''' <param name="xOrderBy">Criteri di ordinamento (opzionale = "")</param>
    ''' <param name="objParametri">AgronicaCoreParametri per la connessione</param>
    ''' <param name="suffissoColonnaAltro">suffisso sul nome della colonna per l'altro gestionale (ad es: "SAP" che verrà aggiunto a "Codice_" (se non specificato = "ALTRO")</param>
    ''' <param name="dataValidita">restituisce solo i valori validi a quella data</param>
    ''' <returns>DataTable contenente i dati della tabella</returns>
    ''' <remarks>Per il tipo è possibile utilizzare le costanti personalizzate che cominciano per INTERSCAMBIO_PARAMETRI_*</remarks>
    Public Function LeggiCodiciParametri(ByVal piva As String,
                                         ByVal consentiPivaNonSpecificata As Boolean,
                                         ByVal idParametro As Integer,
                                         ByVal tipo As String,
                                         ByVal codice_GIAS As String,
                                         ByVal codice_ALTRO As String,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByVal xOrderBy As String,
                                         ByRef objParametri As AgronicaCoreParametri,
                                         Optional ByVal suffissoColonnaAltro As String = "ALTRO",
                                         Optional ByVal dataValidita As Date = #2/1/1900#
                                         ) As DataTable

        '====================================================================================
        'Parametri opzionali :
        '   piva = ""
        '   idParametro = 0
        '   tipo = ""
        '   codice_GIAS = ""
        '   codice_ALTRO = ""
        '====================================================================================
        '
        '   Per il tipo è possibile utilizzare le costanti personalizzate che cominciano per INTERSCAMBIO_PARAMETRI_*
        '
        '====================================================================================

        Const nomeRoutine = "Gias_Interscambio_R.LeggiCodiciParametri()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM Codici_Parametri ")
            stb.AppendLine(" WHERE 1 = 1 ")

            If piva <> "" Then
                stb.AppendLine(" AND ( Piva = '" & Agro_SQL_SaveText(piva) & "' ")
                If consentiPivaNonSpecificata = True Then
                    stb.AppendLine("       OR Piva = '' ")
                End If
                stb.AppendLine(" )")
            End If

            If idParametro <> 0 Then
                stb.AppendLine(" AND Id_Param = " & Agro_SQL_SaveNum(idParametro) & " ")
            End If

            If tipo <> "" Then
                stb.AppendLine(" AND Tipo = '" & Agro_SQL_SaveText(tipo) & "' ")
            End If

            If codice_GIAS <> "" Then
                stb.AppendLine(" AND Codice_GIAS = '" & Agro_SQL_SaveText(codice_GIAS) & "' ")
            End If

            If codice_ALTRO <> "" Then
                stb.AppendLine(" AND [Codice_" & suffissoColonnaAltro & "] = '" & Agro_SQL_SaveText(codice_ALTRO) & "' ")
            End If

            If dataValidita <> #2/1/1900# Then
                stb.AppendLine(" AND Validita_Inizio <= " & Agro_SQL_SaveDate(dataValidita) & " ")
                stb.AppendLine(" AND Validita_Fine >= " & Agro_SQL_SaveDate(dataValidita) & " ")
            End If


            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '##############################################################################################
    Public Function Leggi_CodiceParametro_GIAS_From_ALTRO(ByVal piva As String,
                                                          ByVal consentiPivaNonSpecificata As Boolean,
                                                          ByRef idParam As Integer,
                                                          ByVal tipo As String,
                                                          ByRef codice_GIAS As String,
                                                          ByVal codice_ALTRO As String,
                                                          ByRef objParametri As AgronicaCoreParametri,
                                                          Optional ByVal suffissoColonnaAltro As String = "ALTRO",
                                                          Optional ByVal dataValidita As Date = #2/1/1900#
                                                          ) As Integer

        Const nomeRoutine = "Gias_Interscambio_R.Leggi_CodiceParametro_GIAS_From_ALTRO()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim numElementi As Integer = 0

        Try
            dt = LeggiCodiciParametri(piva, consentiPivaNonSpecificata, 0, tipo, "", codice_ALTRO, "", "", objParametri, suffissoColonnaAltro, dataValidita)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                numElementi = dt.Rows.Count
                codice_GIAS = CStr(If(IsDBNull(dt.Rows(0).Item("Codice_GIAS")), "", dt.Rows(0).Item("Codice_GIAS")))
                idParam = CInt(dt.Rows(0).Item("Id_Param"))
            Else
                numElementi = 0
                codice_GIAS = ""
                idParam = 0
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Codice_GIAS = ""
            idParam = 0
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return numElementi

    End Function

    '##############################################################################################
    Public Function Leggi_CodiceParametro_ALTRO_From_GIAS(ByVal piva As String,
                                                          ByVal consentiPivaNonSpecificata As Boolean,
                                                          ByRef idParam As Integer,
                                                          ByVal tipo As String,
                                                          ByVal codice_GIAS As String,
                                                          ByRef codice_ALTRO As String,
                                                          ByRef objParametri As AgronicaCoreParametri,
                                                          Optional ByVal suffissoColonnaAltro As String = "ALTRO",
                                                          Optional ByVal dataValidita As Date = #2/1/1900#,
                                                          Optional ByVal xFiltroAggiuntivo As String = "",
                                                          Optional ByVal xOrderBy As String = ""
                                                          ) As Integer

        Const nomeRoutine = "Gias_Interscambio_R.Leggi_CodiceParametro_ALTRO_From_GIAS()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim numElementi As Integer = 0

        Try

            dt = LeggiCodiciParametri(piva, consentiPivaNonSpecificata, 0, tipo, codice_GIAS, "", xFiltroAggiuntivo, xOrderBy, objParametri, suffissoColonnaAltro, dataValidita)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                numElementi = dt.Rows.Count
                codice_ALTRO = CStr(IIf(IsDBNull(dt.Rows(0).Item("Codice_" & suffissoColonnaAltro)), "", dt.Rows(0).Item("Codice_" & suffissoColonnaAltro)))
                idParam = CInt(dt.Rows(0).Item("Id_Param"))
            Else
                numElementi = 0
                codice_ALTRO = ""
                idParam = 0
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            codice_ALTRO = ""
            idParam = 0
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return numElementi

    End Function

    '##############################################################################################
    ''' <summary>
    ''' Legge il contenuto della tabella Codici_Prodotti in GIAS INTERSCAMBIO
    ''' </summary>
    ''' <param name="piva">(opzionale = "")</param>
    ''' <param name="idProdotto">(opzionale = 0)</param>
    ''' <param name="codCategoria_ALTRO">Codice categoria prodotto altro programma (opzionale = "")</param>
    ''' <param name="codProdotto_ALTRO">Codice prodotto altro programma (opzionale = "")</param>
    ''' <param name="codCategoria_GIAS">Codice categoria GIAS (opzionale = 0)</param>
    ''' <param name="codProdotto_GIAS">Codice prodotto GIAS (opzionale = 0)</param>
    ''' <param name="xFiltroAggiuntivo">Filtro aggiuntivo per la query di select (opzionale = "")</param>
    ''' <param name="xOrderBy">Criteri di ordinamento (opzionale = "")</param>
    ''' <param name="objParametri">AgronicaCoreParametri per la connessione</param>
    ''' <param name="suffissoColonnaAltro">suffisso sul nome della colonna per l'altro gestionale (ad es: "SAP" che verrà aggiunto a "Cod_Prodotto_" (se non specificato = "ALTRO")</param>
    ''' <param name="dataValidita">restituisce solo i valori validi a quella data</param>
    ''' <returns>DataTable contenente i dati della tabella</returns>
    Public Function LeggiCodiciProdotti(ByVal piva As String,
                                        ByVal idProdotto As Integer,
                                        ByVal codCategoria_ALTRO As String,
                                        Byval codProdotto_ALTRO As String,
                                        ByVal codCategoria_GIAS As Integer,
                                        ByVal codProdotto_GIAS As Integer,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreParametri,
                                        Optional ByVal suffissoColonnaAltro As String = "ALTRO",
                                        Optional ByVal dataValidita As Date = #2/1/1900#
                                        ) As DataTable

        '====================================================================================
        'Parametri opzionali :
        '   piva = ""
        '   idProdotto = 0
        '   codCategoria_ALTRO = ""
        '   codProdotto_ALTRO = ""
        '   codCategoria_GIAS = 0
        '   codProdotto_GIAS = 0
        '====================================================================================

        Const nomeRoutine = "Gias_Interscambio_R.LeggiCodiciProdotti()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            'Piva e Piva_SuperUser se li filtro devo vedere anche dove è stringa vuota perché la controparte potrebbe non averla scritta

            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM Codici_Prodotti ")
            stb.AppendLine(" WHERE 1 = 1 ")


            If piva <> "" Then
                stb.AppendLine(" AND ( Piva = '" & Agro_SQL_SaveText(piva) & "' OR Piva = '' ) ")
            End If

            If idProdotto <> 0 Then
                stb.AppendLine(" AND Id_Prod = " & Agro_SQL_SaveNum(idProdotto) & " ")
            End If

            If codCategoria_ALTRO <> "" Then
                stb.AppendLine(" AND [Cod_Categoria_" & suffissoColonnaAltro & "] = '" & Agro_SQL_SaveText(codCategoria_ALTRO) & "' ")
            End If

            If codProdotto_ALTRO <> "" Then
                stb.AppendLine(" AND [Cod_Prodotto_" & suffissoColonnaAltro & "] = '" & Agro_SQL_SaveText(codProdotto_ALTRO) & "' ")
            End If

            If codCategoria_GIAS <> 0 Then
                stb.AppendLine(" AND Cod_Categoria_GIAS = " & Agro_SQL_SaveNum(codCategoria_GIAS) & " ")
            End If

            If codProdotto_GIAS <> 0 Then
                stb.AppendLine(" AND Cod_Prodotto_GIAS = " & Agro_SQL_SaveNum(codProdotto_GIAS) & " ")
            End If

            If dataValidita <> #2/1/1900# Then
                stb.AppendLine(" AND Validita_Inizio <= " & Agro_SQL_SaveDate(dataValidita) & " ")
                stb.AppendLine(" AND Validita_Fine >= " & Agro_SQL_SaveDate(dataValidita) & " ")
            End If


            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '##############################################################################################
    Public Function Leggi_CodiceProdotto_GIAS_From_ALTRO(ByVal piva As String,
                                                         ByRef idProd As Integer,
                                                         ByVal codCategoria_ALTRO As String,
                                                         ByVal codProdotto_ALTRO As String,
                                                         ByRef codCategoria_GIAS As Integer,
                                                         ByRef codProdotto_GIAS As Integer,
                                                         ByRef objParametri As AgronicaCoreParametri,
                                                         Optional ByVal suffissoColonnaAltro As String = "ALTRO",
                                                         Optional ByVal dataValidita As Date = #2/1/1900#
                                                         ) As Integer

        Const nomeRoutine = "Gias_Interscambio_R.Leggi_CodiceProdotto_GIAS_From_ALTRO()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim numElementi As Integer = 0

        Try

            dt = LeggiCodiciProdotti(piva, 0,
                                     codCategoria_ALTRO, codProdotto_ALTRO, 0, 0,
                                     "", "", objParametri,
                                     suffissoColonnaAltro, dataValidita)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                numElementi = dt.Rows.Count
                codCategoria_GIAS = CInt(IIf(IsDBNull(dt.Rows(0).Item("Cod_Categoria_GIAS")), 0, dt.Rows(0).Item("Cod_Categoria_GIAS")))
                codProdotto_GIAS = CInt(IIf(IsDBNull(dt.Rows(0).Item("Cod_Prodotto_GIAS")), 0, dt.Rows(0).Item("Cod_Prodotto_GIAS")))
                idProd = CInt(dt.Rows(0).Item("Id_Prod"))
            Else
                numElementi = 0
                codCategoria_GIAS = 0
                codProdotto_GIAS = 0
                idProd = 0
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            codCategoria_GIAS = 0
            codProdotto_GIAS = 0
            idProd = 0
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return numElementi

    End Function

    '##############################################################################################
    Public Function Leggi_CodiceProdotto_ALTRO_From_GIAS(ByVal piva As String,
                                                         ByRef idProd As Integer,
                                                         ByRef codCategoria_ALTRO As String,
                                                         ByRef codProdotto_ALTRO As String,
                                                         ByVal codCategoria_GIAS As Integer,
                                                         ByVal codProdotto_GIAS As Integer,
                                                         ByRef objParametri As AgronicaCoreParametri,
                                                         Optional ByVal suffissoColonnaAltro As String = "ALTRO",
                                                         Optional ByVal dataValidita As Date = #2/1/1900#
                                                         ) As Integer

        Const nomeRoutine = "Gias_Interscambio_R.Leggi_CodiceProdotto_ALTRO_From_GIAS()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim numElementi As Integer = 0

        Try

            dt = LeggiCodiciProdotti(piva, 0,
                                     "", "", codCategoria_GIAS, codProdotto_GIAS,
                                     "", "", objParametri,
                                     suffissoColonnaAltro, dataValidita)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                numElementi = dt.Rows.Count
                codCategoria_ALTRO = CStr(If(IsDBNull(dt.Rows(0).Item("Cod_Categoria_" & suffissoColonnaAltro)), "",
                                             dt.Rows(0).Item("Cod_Categoria_" & suffissoColonnaAltro)))
                codProdotto_ALTRO = CStr(If(IsDBNull(dt.Rows(0).Item("Cod_Prodotto_" & suffissoColonnaAltro)), "",
                                            dt.Rows(0).Item("Cod_Prodotto_" & suffissoColonnaAltro)))
                idProd = CInt(dt.Rows(0).Item("Id_Prod"))
            Else
                numElementi = 0
                codCategoria_ALTRO = ""
                codProdotto_ALTRO = ""
                idProd = 0
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            codCategoria_ALTRO = ""
            codProdotto_ALTRO = ""
            idProd = 0
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return numElementi

    End Function

    '##############################################################################################
    ''' <summary>
    ''' Legge il contenuto della tabella Log_GIAS_*(ALTRO/SAP) in GIAS INTERSCAMBIO
    ''' </summary>
    ''' <param name="idLog">(opzionale = 0)</param>
    ''' <param name="tipo">(opzionale = "")</param>
    ''' <param name="nomeFile">(opzionale = "")</param>
    ''' <param name="stato">Stato del file (A = Aperto, C = Completato, E = Errore): utilizzare le costanti INTERSCAMBIO_STATO_* (opzionale = "")</param>
    ''' <param name="sistemaOrigine">Sistema che ha esportato il file (opzionale = "")</param>
    ''' <param name="sistemaDestinazione">Sistema che ha importato il file (opzionale = "")</param>
    ''' <param name="xFiltroAggiuntivo">Filtro aggiuntivo per la query di select (opzionale = "")</param>
    ''' <param name="xOrderBy">Criteri di ordinamento (opzionale = "")</param>
    ''' <param name="objParametri">AgronicaCoreParametri per la connessione</param>
    ''' <param name="suffissoTabella">suffisso sul nome della tabella per l'altro gestionale (ad es: "SAP" che verrà aggiunto a "Log_GIAS_" (se non specificato = "ALTRO")</param>
    ''' <returns>DataTable contenente i dati della tabella</returns>
    Public Function LeggiLogGiasAltro(ByVal idLog As Integer,
                                      ByVal tipo As String,
                                      Byval nomeFile As String,
                                      ByVal stato As String,
                                      ByVal sistemaOrigine As String,
                                      ByVal sistemaDestinazione As String,
                                      ByVal xFiltroAggiuntivo As String,
                                      ByVal xOrderBy As String,
                                      ByRef objParametri As AgronicaCoreParametri,
                                      Optional ByVal suffissoTabella As String = "ALTRO"
                                      ) As DataTable

        '====================================================================================
        'Parametri opzionali :
        '   idLog = 0
        '   tipo = ""
        '   nomeFile = ""
        '   stato = ""
        '   sistemaOrigine = ""
        '   sistemaDestinazione = ""
        '====================================================================================

        Const nomeRoutine = "Gias_Interscambio_R.LeggiLogGiasAltro()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM [Log_GIAS_" & suffissoTabella & "] ")
            stb.AppendLine(" WHERE 1 = 1 ")


            If idLog <> 0 Then
                stb.AppendLine(" AND Id_Log = " & Agro_SQL_SaveNum(idLog) & " ")
            End If

            If tipo <> "" Then
                stb.AppendLine(" AND Tipo = '" & Agro_SQL_SaveText(tipo) & "' ")
            End If

            If nomeFile <> "" Then
                stb.AppendLine(" AND Nome_File = '" & Agro_SQL_SaveText(nomeFile) & "' ")
            End If

            If stato <> "" Then
                stb.AppendLine(" AND Stato = '" & Agro_SQL_SaveText(stato) & "' ")
            End If

            If sistemaOrigine <> "" Then
                stb.AppendLine(" AND Sistema_Origine = '" & Agro_SQL_SaveText(sistemaOrigine) & "' ")
            End If

            If sistemaDestinazione <> "" Then
                stb.AppendLine(" AND Sistema_Destinazione = '" & Agro_SQL_SaveText(sistemaDestinazione) & "' ")
            End If


            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function LeggiLogGiasSapItf(ByVal idLog As Integer,
                                       ByVal codInterfaccia As String,
                                       Byval type As String,
                                       ByVal nomeFile As String,
                                       ByVal autoreRecord As String,
                                       ByVal tipoLog As String,
                                       ByVal xFiltroAggiuntivo As String,
                                       ByVal xOrderBy As String,
                                       ByRef objParametri As AgronicaCoreParametri
                                       ) As DataTable

        '====================================================================================
        'Parametri opzionali :
        '   idLog = 0
        '   codInterfaccia = ""
        '   type = ""
        '   nomeFile = ""
        '   tipoLog = ""
        '====================================================================================

        Const nomeRoutine = "Gias_Interscambio_R.LeggiLogGiasSapItf()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM [SAP_ITF_LOG] ")
            stb.AppendLine(" WHERE 1 = 1 ")


            If idLog <> 0 Then
                stb.AppendLine(" AND Id_Log = " & Agro_SQL_SaveNum(idLog) & " ")
            End If

            If codInterfaccia <> "" Then
                stb.AppendLine(" AND ITF_ID = '" & Agro_SQL_SaveText(codInterfaccia) & "' ")
            End If

            If type <> "" Then
                stb.AppendLine(" AND ITF_DESC = '" & Agro_SQL_SaveText(type) & "' ")
            End If

            If nomeFile <> "" Then
                stb.AppendLine(" AND PKG_NAME = '" & Agro_SQL_SaveText(nomeFile) & "' ")
            End If

            If autoreRecord <> "" Then
                stb.AppendLine(" AND SOURCE_NAME = '" & Agro_SQL_SaveText(autoreRecord) & "' ")
            End If

            If tipoLog <> "" Then
                stb.AppendLine(" AND MSG_TYPE = '" & Agro_SQL_SaveText(tipoLog) & "' ")
            End If


            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    Public Function Leggi_LogDettaglioExport(ByVal NomeFile As String,
                                             ByVal KeyAgenda As String,
                                             ByVal KeyDettaglio As String,
                                             ByVal KeyExport As String,
                                             ByVal TipoRisorsa As String,
                                             ByVal xFiltroAggiuntivo As String,
                                             ByVal xOrderBy As String,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As DataTable


        Const nomeRoutine = "Gias_Interscambio_R.Leggi_LogDettaglioExport()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM GIAS_Dettaglio_Export_Log ")
            stb.AppendLine(" WHERE 1 = 1 ")

            If NomeFile <> "" Then
                stb.AppendLine(" AND Upper(NomeFile) = '" & UCase(Agro_SQL_SaveText(NomeFile)) & "' ")
            End If

            If KeyAgenda <> "" Then
                stb.AppendLine(" AND KeyAgenda = '" & Agro_SQL_SaveText(KeyAgenda) & "' ")
            End If

            If KeyDettaglio <> "" Then
                stb.AppendLine(" AND KeyDettaglio = '" & Agro_SQL_SaveText(KeyDettaglio) & "' ")
            End If

            If KeyExport <> "" Then
                stb.AppendLine(" AND KeyExport = '" & Agro_SQL_SaveText(KeyExport) & "' ")
            End If

            If TipoRisorsa <> "" Then
                stb.AppendLine(" AND Upper(TipoRisorsa) = '" & Agro_SQL_SaveText(UCase(TipoRisorsa)) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    Public Function Leggi_LastLogDettaglioExport(ByVal KeyAgenda As String,
                                                 ByVal TipoRisorsa As String,
                                                 ByVal xFiltroAggiuntivo As String,
                                                 ByRef objParametri As AgronicaCoreParametri
                                                 ) As DataTable


        Const nomeRoutine = "Gias_Interscambio_R.Leggi_LastLogDettaglioExport()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT Top 1 NomeFile ")
            stb.AppendLine(" FROM GIAS_Dettaglio_Export_Log ")
            stb.AppendLine(" WHERE Upper(TipoOperazione) <> 'CANC' ")

            If KeyAgenda <> "" Then
                stb.AppendLine(" AND KeyAgenda = '" & Agro_SQL_SaveText(KeyAgenda) & "' ")
            End If

            If TipoRisorsa <> "" Then
                stb.AppendLine(" AND Upper(TipoRisorsa) = '" & Agro_SQL_SaveText(UCase(TipoRisorsa)) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function



    Public Class MappingProdotto
        Public Property Id_Prod As Integer
        Public Property Cod_Prodotto As String
    End Class

    '##############################################################################################
    ''' <summary>
    ''' Restituisce dizionario con chiave la categoria del prodotto (ALTRO) e valore una lista di MappingProdotto con codici prodotto (ALTRO) e Id_prod x quella categoria
    ''' </summary>
    ''' <returns></returns>
    Public Function LeggiProdottiNonMappati(ByRef objParametri As AgronicaCoreParametri,
                                            Optional ByVal flagGetNoMapGias As Boolean = False,
                                            Optional ByVal flagGetNoMapAltro As Boolean = False,
                                            Optional ByVal filtroCategorie As String = "",
                                            Optional ByVal xFiltroAggiuntivo As String = "",
                                            Optional ByVal xOrderBy As String = "",
                                            Optional ByVal suffissoColonnaAltro As String = "ALTRO",
                                            Optional ByVal dataValidita As Date = #2/1/1900#
                                            ) As Dictionary(Of String, List(Of MappingProdotto))
        
        Const nomeRoutine = "Gias_Interscambio_R.LeggiProdottiNonMappati()"

        Dim messaggioErrore As String = ""
        Dim dictCat As Dictionary(Of String, List(Of MappingProdotto)) = Nothing

        Try

            If flagGetNoMapAltro = flagGetNoMapGias Then
                Throw New Exception("Solo uno tra flagGetNoMapGias e flagGetNoMapAltro può essere True")
            End If
            
            Dim dt As DataTable = QueryLeggiProdottiNonMappati(objParametri,
                                                               flagGetNoMapGias, flagGetNoMapAltro,
                                                               filtroCategorie,
                                                               xFiltroAggiuntivo, xOrderBy,
                                                               suffissoColonnaAltro, dataValidita)
            
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then

                dictCat = new Dictionary(Of String, List(Of MappingProdotto))
                Dim nomeColCat As String = "Cod_Categoria_GIAS"
                Dim nomeColProd As String = "Cod_Prodotto_GIAS"

                If flagGetNoMapGias = True Then
                    nomeColCat = "Cod_Categoria_" & suffissoColonnaAltro
                    nomeColProd = "Cod_Prodotto_" & suffissoColonnaAltro
                End If

                Dim subTables As List(Of DataTable) = dt.AsEnumerable() _
                        .GroupBy(Function(x) x.Field(Of String)(nomeColCat)) _
                        .Select(Function(p) p.CopyToDataTable()) _
                        .ToList()

                For Each dtCat In subTables
                    If dtCat IsNot Nothing AndAlso dtCat.Rows.Count > 0 Then
                        Dim chiave As String = dtCat.Rows(0).Item(nomeColCat).ToString() 'valore categoria

                        'Dim valore As List(Of String()) = dtCat.Rows.Cast(Of DataRow)() _
                        '        .Select(Function(row) New String() {row(nomeColProd).ToString(), row("Id_prod").ToString()}) _
                        '        .ToList()   'lista valori prodotto

                        Dim valore as List(Of MappingProdotto) = dtCat.Rows.Cast(Of DataRow)() _
                                .Select(Function(row) New MappingProdotto() With {
                                           .Id_Prod = CInt(row("Id_Prod")),
                                           .Cod_Prodotto = row(nomeColProd).ToString() }) _
                                .ToList()   'lista valori prodotto + id_cod

                        '.Select(Function(row) row(nomeColProd).ToString()) _

                        dictCat.Add(chiave, valore)
                    End If
                Next

            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dictCat

    End Function

    Private Function QueryLeggiProdottiNonMappati(ByRef objParametri As AgronicaCoreParametri,
                                                  Optional ByVal flagGetNoMapGias As Boolean = False,
                                                  Optional ByVal flagGetNoMapAltro As Boolean = False,
                                                  Optional ByVal filtroCategorie As String = "",
                                                  Optional ByVal xFiltroAggiuntivo As String = "",
                                                  Optional ByVal xOrderBy As String = "",
                                                  Optional ByVal suffissoColonnaAltro As String = "ALTRO",
                                                  Optional ByVal dataValidita As Date = #2/1/1900#
                                                  ) As DataTable
        
        Const nomeRoutine = "Gias_Interscambio_R.QueryLeggiProdottiNonMappati()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim stb As New StringBuilder

        Try

            If flagGetNoMapAltro = flagGetNoMapGias Then
                Throw New Exception("Solo uno tra flagGetNoMapGias e flagGetNoMapAltro può essere True")
            End If

            stb.Length = 0

            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM Codici_Prodotti ")
            stb.AppendLine(" WHERE 1 = 1 ")

            If flagGetNoMapGias = True Then
                stb.AppendLine(" AND (Cod_Prodotto_GIAS IS NULL OR Cod_Prodotto_GIAS = 0) ")

                If filtroCategorie <> "" Then
                    stb.AppendLine(" AND [Cod_Categoria_" & suffissoColonnaAltro & "] IN (" & Agro_SQL_Save_Clausola_IN(filtroCategorie) & ") ")
                End If

            End If

            If flagGetNoMapAltro = True Then
                stb.AppendLine(" AND ([Cod_Prodotto_" & suffissoColonnaAltro & "] IS NULL OR [Cod_Prodotto_" & suffissoColonnaAltro & "] = 0) ")

                If filtroCategorie <> "" Then
                    stb.AppendLine(" AND Cod_Categoria_GIAS IN (" & Agro_SQL_Save_Clausola_IN(filtroCategorie) & ") ")
                End If

            End If

            If dataValidita <> #2/1/1900# Then
                stb.AppendLine(" AND Validita_Inizio <= " & Agro_SQL_SaveDate(dataValidita) & " ")
                stb.AppendLine(" AND Validita_Fine >= " & Agro_SQL_SaveDate(dataValidita) & " ")
            End If


            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
            
        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Class MappingProdottoBancaDati
        Public Property Id_Prod As Integer
        Public Property Piva_Cac As String
        Public Property Categoria_Cac As Integer?
        Public Property Prodotto_Cac As String
        Public Property DescProdotto_Cac As String
        Public Property ProCod_Cac As Integer?
        Public Property Note_Cac As String
        Public Property Categoria_Altro As String
        Public Property Prodotto_Altro As String
        Public Property Categoria_Gias As Integer?
        Public Property Prodotto_Gias As Integer?
        Public Property DescProdotto_Altro As String
    End Class

    Public Enum enum_StatoMappingCacInterscambio
        Non_Importato = 0
        Mappato_OK = 1
        Non_Ancora_Mappato = 2
        Cambiato_Su_Cac = 3
        Cancellato_su_Cac = 4
    End Enum

    '##############################################################################################
    ''' <summary>
    ''' ATTENZIONE: funziona solo se i due database (Server e Interscambio) sono sulla medesima istanza SQL
    ''' </summary>
    Public Function LeggiProdottiMappatiDifferenti(ByRef objParametriServer As AgronicaCoreParametri,
                                                   ByRef objParametriInterscambio As AgronicaCoreParametri,
                                                   Optional ByVal suffissoColonnaAltro As String = "ALTRO",
                                                   Optional ByVal filtroCategorieGias As String = "",
                                                   Optional ByVal filtroCategorieAltro As String = "",
                                                   Optional ByVal filtroCategorieAltroInterscambio As String = "",
                                                   Optional ByVal prefissoFito As String = Nothing
                                                   ) As Dictionary(Of enum_StatoMappingCacInterscambio, List(Of MappingProdottoBancaDati))
        
        Const nomeRoutine = "Gias_Interscambio_R.LeggiProdottiMappatiDifferenti()"

        Dim messaggioErrore As String = ""
        Dim dictMap As Dictionary(Of enum_StatoMappingCacInterscambio, List(Of MappingProdottoBancaDati)) = Nothing

        Try

            'TODO: ATTENZIONE: funziona solo se i due database (Server e Interscambio) sono sulla medesima istanza SQL

            Dim dt As DataTable = QueryLeggiProdottiMappatiDifferenti(objParametriServer, objParametriInterscambio,
                                                                      suffissoColonnaAltro,
                                                                      filtroCategorieGias, filtroCategorieAltro,
                                                                      filtroCategorieAltroInterscambio, prefissoFito)
            
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then

                dictMap = New Dictionary(Of enum_StatoMappingCacInterscambio, List(Of MappingProdottoBancaDati))

                Dim mappingList As List(Of MappingProdottoBancaDati)
                mappingList  = (From p In dt.AsEnumerable()
                                Select New MappingProdottoBancaDati With {
                                    .Id_Prod = p.Field(Of Integer)("Id_Prod"),
                                    .Piva_Cac = p.Field(Of String)("Piva"),
                                    .Categoria_Cac = p.Field(Of Integer?)("Elem_Cod"),
                                    .Prodotto_Cac = p.Field(Of String)("Cod_Prodotto_Cliente"),
                                    .DescProdotto_Cac = p.Field(Of String)("Desc_Prodotto_Cliente"),
                                    .ProCod_Cac = p.Field(Of Integer?)("Codice_GIAS"),
                                    .Note_Cac = p.Field(Of String)("Note"),
                                    .Categoria_Altro = p.Field(Of String)("Cod_Categoria_" & suffissoColonnaAltro),
                                    .Prodotto_Altro = p.Field(Of String)("Cod_Prodotto_" & suffissoColonnaAltro),
                                    .Categoria_Gias = p.Field(Of Integer?)("Cod_Categoria_GIAS"),
                                    .Prodotto_Gias = p.Field(Of Integer?)("Cod_Prodotto_GIAS"),
                                    .DescProdotto_Altro = p.Field(Of String)("Descr_Prodotto")
                                }).ToList()
                
                'Devo raggruppare i risultati per tipologia di stato

                'il prodotto è stato mappato su CAC, ma devo ancora rispecchiare il mapping in Interscambio
                EstraiMappatiCacNoInter(mappingList, dictMap)

                'il prodotto era già stato mappato in interscambio, ma ora su CAC è mappato diversamente
                EstraiCambiatiCac(mappingList, dictMap)
                
                'il prodotto era stato mappato in Interscambio, ma ora è stato cancellato da CAC (devo resettare anche il mapping in interscambio?!?)
                EstraiCancellatiCac(mappingList, dictMap)

            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dictMap

    End Function

    ''' <summary>
    ''' ATTENZIONE: funziona solo se i due database (Server e Interscambio) sono sulla medesima istanza SQL
    ''' </summary>
    Private Function QueryLeggiProdottiMappatiDifferenti(ByRef objParametriServer As AgronicaCoreParametri,
                                                         ByRef objParametriInterscambio As AgronicaCoreParametri,
                                                         Optional ByVal suffissoColonnaAltro As String = "ALTRO",
                                                         Optional ByVal filtroCategorieGias As String = "",
                                                         Optional ByVal filtroCategorieAltro As String = "",
                                                         Optional ByVal filtroCategorieAltroInterscambio As String = "",
                                                         Optional ByVal prefissoFito As String = Nothing,
                                                         Optional ByVal codCategoriaAltroFito As String = "191"
                                                         ) As DataTable
        
        Const nomeRoutine = "Gias_Interscambio_R.QueryLeggiProdottiMappatiDifferenti()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim stb As New StringBuilder

        Try

            'TODO: ATTENZIONE: funziona solo se i due database (Server e Interscambio) sono sulla medesima istanza SQL

            Dim nomeDbServer As String = objParametriServer.Recupera_NomeDB()
            If String.IsNullOrEmpty(nomeDbServer) Then
                Throw New Exception("Impossibile ricavare il Nome DB da objParametri Server")
            End If

            Dim nomeDbInterscambio As String = objParametriInterscambio.Recupera_NomeDB()
            If String.IsNullOrEmpty(nomeDbInterscambio) Then
                Throw New Exception("Impossibile ricavare il Nome DB da objParametri Interscambio")
            End If

            stb.Length = 0

            stb.AppendLine(" ( ")
            stb.AppendLine("     -- Guida è CAC ")
            stb.AppendLine("     SELECT cac.Piva, cac.Elem_Cod, cac.Cod_Prodotto_Cliente, cac.Desc_Prodotto_Cliente, cac.Codice_GIAS, cac.Note, ")
            stb.AppendLine("            inter.Id_Prod, inter.[Cod_Categoria_" & suffissoColonnaAltro & "], inter.[Cod_Prodotto_" & suffissoColonnaAltro & "], ")
            stb.AppendLine("            inter.Cod_Categoria_GIAS, inter.Cod_Prodotto_GIAS, inter.Descr_Prodotto  ")
            stb.AppendLine("     FROM [" & nomeDbServer & "].dbo.CAC_Codifica_ProdottiAziendali cac ")
            stb.AppendLine("     LEFT JOIN [" & nomeDbInterscambio & "].dbo.Codici_Prodotti inter ")
            stb.AppendLine("               ON inter.[Cod_Prodotto_" & suffissoColonnaAltro & "] COLLATE Latin1_General_CI_AS = cac.Cod_Prodotto_Cliente ")
            stb.AppendLine("               AND inter.Piva COLLATE Latin1_General_CI_AS = cac.Piva ")
            stb.AppendLine("     WHERE (NOT (cac.Elem_Cod = inter.Cod_Categoria_GIAS AND cac.Codice_GIAS = inter.Cod_Prodotto_GIAS) OR inter.Cod_Categoria_GIAS IS NULL) ")

            If filtroCategorieGias <> "" Then
                stb.AppendLine("     AND cac.Elem_Cod IN (" & Agro_SQL_Save_Clausola_IN(filtroCategorieGias) & ") ")
            End If

            If filtroCategorieAltro <> "" Then
                stb.AppendLine("     AND (inter.[Cod_Categoria_" & suffissoColonnaAltro & "] IN (" & Agro_SQL_Save_Clausola_IN(filtroCategorieAltro, True) & ")) -- OR inter.[Cod_Categoria_" & suffissoColonnaAltro & "] IS NULL) ")
            End If

            stb.AppendLine(" ) ")

            stb.AppendLine("")
            stb.AppendLine(" UNION  ")
            stb.AppendLine("")

            stb.AppendLine(" ( ")
            stb.AppendLine("     -- Guida è INTERSCAMBIO (vuol dire che ho eliminato dei record in cac) ")
            stb.AppendLine("     SELECT cac.Piva, cac.Elem_Cod, cac.Cod_Prodotto_Cliente, cac.Desc_Prodotto_Cliente, cac.Codice_GIAS, cac.Note, ")
            stb.AppendLine("            inter.Id_Prod, inter.[Cod_Categoria_" & suffissoColonnaAltro & "], inter.[Cod_Prodotto_" & suffissoColonnaAltro & "], ")
            stb.AppendLine("            inter.Cod_Categoria_GIAS, inter.Cod_Prodotto_GIAS, inter.Descr_Prodotto ")
            stb.AppendLine("     FROM [" & nomeDbInterscambio & "].dbo.Codici_Prodotti inter ")
            stb.AppendLine("     LEFT JOIN [" & nomeDbServer & "].dbo.CAC_Codifica_ProdottiAziendali cac ")
            stb.AppendLine("               ON inter.[Cod_Prodotto_" & suffissoColonnaAltro & "] COLLATE Latin1_General_CI_AS = cac.Cod_Prodotto_Cliente ")
            stb.AppendLine("               AND inter.Piva COLLATE Latin1_General_CI_AS = cac.Piva ")
            stb.AppendLine("     WHERE (NOT (cac.Elem_Cod = inter.Cod_Categoria_GIAS AND cac.Codice_GIAS = inter.Cod_Prodotto_GIAS) OR cac.Elem_Cod IS NULL) ")

            If filtroCategorieGias <> "" Then
                stb.AppendLine("     AND (cac.Elem_Cod IN (" & Agro_SQL_Save_Clausola_IN(filtroCategorieGias) & ") OR cac.Elem_Cod IS NULL) ")
            End If

            If filtroCategorieAltroInterscambio <> "" Then
                stb.AppendLine("     AND (inter.[Cod_Categoria_" & suffissoColonnaAltro & "] IN (" & Agro_SQL_Save_Clausola_IN(filtroCategorieAltroInterscambio, True) & ") ")

                If prefissoFito IsNot Nothing Then
                    'se PrefissoFito non è Nothing, vuol dire che o è valorizzato o stringa vuota,
                    'ma cmq implica che sto derogando al principio NoMapFito
                    stb.AppendLine("          OR (inter.[Cod_Categoria_" & suffissoColonnaAltro & "] IN (" & Agro_SQL_Save_Clausola_IN(codCategoriaAltroFito, True) & ") ")
                    stb.AppendLine("              AND NOT ( ")

                    If prefissoFito <> "" Then
                        'Se c'è prefisso
                        stb.AppendLine("                       inter.[Cod_Prodotto_" & suffissoColonnaAltro & "] LIKE '" & Agro_SQL_SaveText(prefissoFito & "%") & "' ")
                        stb.AppendLine("                       AND ISNUMERIC(SUBSTRING(inter.[Cod_Prodotto_" & suffissoColonnaAltro & "], LEN('" & Agro_SQL_SaveText(prefissoFito) & "') + 1, LEN(inter.[Cod_Prodotto_" & suffissoColonnaAltro & "]) - LEN('" & Agro_SQL_SaveText(prefissoFito) & "'))) = 1 ")
                    Else
                        'Se non c'è prefisso è direttamente il numero di registrazione (quindi numerico)
                        stb.AppendLine("                       ISNUMERIC(inter.[Cod_Prodotto_" & suffissoColonnaAltro & "]) = 1 ")
                    End If

                    stb.AppendLine("                       ) ")
                    stb.AppendLine("              ) ")
                End If

                stb.AppendLine("          ) ")

            End If

            stb.AppendLine(" )")

            'If dataValidita <> #2/1/1900# Then
            '    stb.AppendLine(" AND Validita_Inizio <= " & Agro_SQL_SaveDate(dataValidita) & " ")
            '    stb.AppendLine(" AND Validita_Fine >= " & Agro_SQL_SaveDate(dataValidita) & " ")
            'End If


            'If xFiltroAggiuntivo <> "" Then
            '    stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            'End If

            'If xOrderBy <> "" Then
            '    stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            'End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametriServer, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Private Sub EstraiMappatiCacNoInter(ByRef mappingList As List(Of MappingProdottoBancaDati),
                                        ByRef dictMap As Dictionary(Of enum_StatoMappingCacInterscambio, List(Of MappingProdottoBancaDati)))

        Const nomeRoutine = "EstraiMappatiCacNoInter"

        Try

            'il prodotto è stato mappato su CAC, ma devo ancora rispecchiare il mapping in Interscambio

            Dim chiave = enum_StatoMappingCacInterscambio.Non_Ancora_Mappato
            Dim valore As List(Of MappingProdottoBancaDati)
            valore = mappingList.Where(Function(x) (x.Categoria_Gias Is Nothing OrElse x.Categoria_Gias = 0) AndAlso
                                                   (x.Prodotto_Gias Is Nothing OrElse x.Prodotto_Gias = 0) AndAlso
                                                   (IsNumeric(x.Categoria_Cac) AndAlso x.Categoria_Cac <> 0) AndAlso
                                                   (IsNumeric(x.ProCod_Cac) AndAlso x.ProCod_Cac <> 0)).ToList()

            If valore IsNot Nothing AndAlso valore.Count > 0 Then
                dictMap.Add(chiave, valore)
            End If

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
    End Sub

    Private Sub EstraiCambiatiCac(ByRef mappingList As List(Of MappingProdottoBancaDati),
                                  ByRef dictMap As Dictionary(Of enum_StatoMappingCacInterscambio, List(Of MappingProdottoBancaDati)))

        Const nomeRoutine = "EstraiCambiatiCac"

        Try

            'il prodotto era già stato mappato in interscambio, ma ora su CAC è mappato diversamente

            Dim chiave = enum_StatoMappingCacInterscambio.Cambiato_Su_Cac
            Dim valore As List(Of MappingProdottoBancaDati)
            valore = mappingList.Where(Function(x) (IsNumeric(x.Categoria_Gias) AndAlso x.Categoria_Gias <> 0) AndAlso
                                                   (IsNumeric(x.Prodotto_Gias) AndAlso x.Prodotto_Gias <> 0) AndAlso
                                                   (IsNumeric(x.Categoria_Cac) AndAlso x.Categoria_Cac <> 0) AndAlso
                                                   (IsNumeric(x.ProCod_Cac) AndAlso x.ProCod_Cac <> 0) AndAlso
                                                   (x.Categoria_Gias <> x.Categoria_Cac OrElse x.Prodotto_Gias <> x.ProCod_Cac)).ToList()

            If valore IsNot Nothing AndAlso valore.Count > 0 Then
                dictMap.Add(chiave, valore)
            End If

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
    End Sub

    Private Sub EstraiCancellatiCac(ByRef mappingList As List(Of MappingProdottoBancaDati),
                                    ByRef dictMap As Dictionary(Of enum_StatoMappingCacInterscambio, List(Of MappingProdottoBancaDati)))

        Const nomeRoutine = "EstraiCancellatiCac"

        Try

            'il prodotto era stato mappato in Interscambio, ma ora è stato cancellato da CAC

            Dim chiave = enum_StatoMappingCacInterscambio.Cancellato_su_Cac
            Dim valore As List(Of MappingProdottoBancaDati)
            valore = mappingList.Where(Function(x) (IsNumeric(x.Categoria_Gias) AndAlso x.Categoria_Gias <> 0) AndAlso
                                                   (IsNumeric(x.Prodotto_Gias) AndAlso x.Prodotto_Gias <> 0) AndAlso
                                                   (x.Categoria_Cac Is Nothing OrElse x.Categoria_Cac = 0) AndAlso
                                                   (x.ProCod_Cac Is Nothing OrElse x.ProCod_Cac = 0)).ToList()

            If valore IsNot Nothing AndAlso valore.Count > 0 Then
                dictMap.Add(chiave, valore)
            End If

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
    End Sub

#End Region

#Region "Imprese Gestite"

    '##############################################################################################
    Public Function LeggiImpreseGestite(ByVal id As Integer,
                                        ByVal piva As String,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreParametri,
                                        Optional ByVal dataValidita As Date = #2/1/1900#
                                        ) As DataTable

        '====================================================================================
        'Parametri opzionali :
        '   id = 0
        '   piva = ""
        '====================================================================================

        Const nomeRoutine = "Gias_Interscambio_R.LeggiImpreseGestite()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM Imprese_Gestite ")
            stb.AppendLine(" WHERE 1 = 1 ")

            If id <> 0 Then
                stb.AppendLine(" AND Id = " & Agro_SQL_SaveNum(id) & " ")
            End If

            If piva <> "" Then
                stb.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            If dataValidita <> #2/1/1900# Then
                stb.AppendLine(" AND Validita_Inizio <= " & Agro_SQL_SaveDate(dataValidita) & " ")
                stb.AppendLine(" AND Validita_Fine >= " & Agro_SQL_SaveDate(dataValidita) & " ")
            End If


            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

#End Region

#Region "Codici Parametri Qualitativi"

    '##############################################################################################
    ''' <summary>
    ''' Legge il contenuto della tabella Codici_Parametri_Qualitativi in GIAS INTERSCAMBIO
    ''' </summary>
    ''' <param name="piva">(opzionale = "")</param>
    ''' <param name="idParametro">(opzionale = 0)</param>
    ''' <param name="tipo_ALTRO">Tipo parametro qualitativo in un altro programma (opzionale = "")</param>
    ''' <param name="codice_ALTRO">Codice del parametro qualitativo in un altro programma (opzionale = "")</param>
    ''' <param name="tipo_GIAS">Tipo parametro qualitativo GIAS (opzionale = "")</param>
    ''' <param name="codice_GIAS">Codice del parametro qualitativo in GIAS (opzionale = 0)</param>
    ''' <param name="xFiltroAggiuntivo">Filtro aggiuntivo per la query di select (opzionale = "")</param>
    ''' <param name="xOrderBy">Criteri di ordinamento (opzionale = "")</param>
    ''' <param name="objParametri">AgronicaCoreParametri per la connessione</param>
    ''' <param name="suffissoColonnaAltro">suffisso sul nome della colonna per l'altro gestionale (ad es: "SAP" che verrà aggiunto a "Codice_" (se non specificato = "ALTRO")</param>
    ''' <param name="dataValidita">restituisce solo i valori validi a quella data</param>
    ''' <returns>DataTable contenente i dati della tabella</returns>
    Public Function LeggiCodiciParametriQualitativi(ByVal piva As String,
                                                    ByVal idParametro As Integer,
                                                    ByVal tipo_ALTRO As String,
                                                    ByVal codice_ALTRO As String,
                                                    ByVal tipo_GIAS As String,
                                                    ByVal codice_GIAS As Integer,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreParametri,
                                                    Optional ByVal suffissoColonnaAltro As String = "ALTRO",
                                                    Optional ByVal dataValidita As Date = #2/1/1900#
                                                    ) As DataTable

        '====================================================================================
        'Parametri opzionali :
        '   piva = ""
        '   idParametro = 0
        '   tipo_ALTRO = ""
        '   codice_ALTRO = ""
        '   tipo_GIAS = ""
        '   codice_GIAS = 0
        '====================================================================================

        Const nomeRoutine = "Gias_Interscambio_R.LeggiCodiciParametriQualitativi()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM Codici_Parametri_Qualitativi ")
            stb.AppendLine(" WHERE 1 = 1 ")

            If piva <> "" Then
                stb.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            If idParametro <> 0 Then
                stb.AppendLine(" AND Id_Param = " & Agro_SQL_SaveNum(idParametro) & " ")
            End If

            If tipo_GIAS <> "" Then
                stb.AppendLine(" AND Tipo_GIAS = '" & Agro_SQL_SaveText(tipo_GIAS) & "' ")
            End If

            If codice_ALTRO <> "" Then
                stb.AppendLine(" AND [Codice_" & suffissoColonnaAltro & "] = '" & Agro_SQL_SaveText(codice_ALTRO) & "' ")
            End If

            If tipo_ALTRO <> "" Then
                stb.AppendLine(" AND Tipo_ALTRO = '" & Agro_SQL_SaveText(tipo_ALTRO) & "' ")
            End If

            If codice_GIAS <> 0 Then
                stb.AppendLine(" AND Codice_GIAS = " & Agro_SQL_SaveNum(codice_GIAS) & " ")
            End If

            If dataValidita <> #2/1/1900# Then
                stb.AppendLine(" AND Validita_Inizio <= " & Agro_SQL_SaveDate(dataValidita) & " ")
                stb.AppendLine(" AND Validita_Fine >= " & Agro_SQL_SaveDate(dataValidita) & " ")
            End If


            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '##############################################################################################
    Public Function Leggi_CodiceParametroQualitativo_GIAS_From_ALTRO(ByVal piva As String,
                                                                     ByRef idParam As Integer,
                                                                     ByVal tipo_ALTRO As String,
                                                                     ByVal codice_ALTRO As String,
                                                                     ByRef tipo_GIAS As String,
                                                                     ByRef codice_GIAS As Integer,
                                                                     ByRef objParametri As AgronicaCoreParametri,
                                                                     Optional ByVal suffissoColonnaAltro As String = "ALTRO",
                                                                     Optional ByVal dataValidita As Date = #2/1/1900#
                                                                     ) As Integer

        Const nomeRoutine = "Gias_Interscambio_R.Leggi_CodiceParametroQualitativo_GIAS_From_ALTRO()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim numElementi As Integer = 0

        Try
            dt = LeggiCodiciParametriQualitativi(piva, 0,
                                                 tipo_ALTRO, codice_ALTRO,
                                                 "", 0,
                                                 "", "",
                                                 objParametri, suffissoColonnaAltro, dataValidita)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                numElementi = dt.Rows.Count
                tipo_GIAS = If(IsDBNull(dt.Rows(0).Item("Tipo_GIAS")), "", CStr(dt.Rows(0).Item("Tipo_GIAS")))
                codice_GIAS = If(IsDBNull(dt.Rows(0).Item("Codice_GIAS")), 0, CInt(dt.Rows(0).Item("Codice_GIAS")))
                idParam = CInt(dt.Rows(0).Item("Id_Param"))
            Else
                numElementi = 0
                tipo_GIAS = ""
                codice_GIAS = 0
                idParam = 0
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            tipo_GIAS = ""
            codice_GIAS = 0
            idParam = 0
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return numElementi

    End Function

    '##############################################################################################
    Public Function Leggi_CodiceParametroQualitativo_ALTRO_From_GIAS(ByVal piva As String,
                                                                     ByRef idParam As Integer,
                                                                     ByRef tipo_ALTRO As String,
                                                                     ByRef codice_ALTRO As String,
                                                                     ByVal tipo_GIAS As String,
                                                                     ByVal codice_GIAS As Integer,
                                                                     ByRef objParametri As AgronicaCoreParametri,
                                                                     Optional ByVal suffissoColonnaAltro As String = "ALTRO",
                                                                     Optional ByVal dataValidita As Date = #2/1/1900#
                                                                     ) As Integer

        Const nomeRoutine = "Gias_Interscambio_R.Leggi_CodiceParametroQualitativo_ALTRO_From_GIAS()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim numElementi As Integer = 0

        Try
            dt = LeggiCodiciParametriQualitativi(piva, 0,
                                                 tipo_ALTRO, codice_ALTRO,
                                                 tipo_GIAS, codice_GIAS,
                                                 "", "",
                                                 objParametri, suffissoColonnaAltro, dataValidita)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                numElementi = dt.Rows.Count
                tipo_ALTRO = If(IsDBNull(dt.Rows(0).Item("Codice_" & suffissoColonnaAltro)), "", CStr(dt.Rows(0).Item("Codice_" & suffissoColonnaAltro)))
                codice_ALTRO = If(IsDBNull(dt.Rows(0).Item("Codice_" & suffissoColonnaAltro)), "", CStr(dt.Rows(0).Item("Codice_" & suffissoColonnaAltro)))
                idParam = CInt(dt.Rows(0).Item("Id_Param"))
            Else
                numElementi = 0
                tipo_ALTRO = ""
                codice_ALTRO = ""
                idParam = 0
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            tipo_ALTRO = ""
            codice_ALTRO = ""
            idParam = 0
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return numElementi

    End Function

#End Region

#Region "Doc Processati"

    '##############################################################################################
    ''' <summary>
    ''' Legge il contenuto della tabella Doc_Processati in GIAS INTERSCAMBIO
    ''' </summary>
    ''' <param name="piva">(opzionale = "")</param>
    ''' <param name="idAgenda">(opzionale = 0)</param>
    ''' <param name="idDocAltro">Identificativo univoco del documento così come ricevuto dall'altro gestionale (opzionale = "")</param>
    ''' <param name="xFiltroAggiuntivo">Filtro aggiuntivo per la query di select (opzionale = "")</param>
    ''' <param name="xOrderBy">Criteri di ordinamento (opzionale = "")</param>
    ''' <param name="objParametri">AgronicaCoreParametri per la connessione</param>
    ''' <returns>DataTable contenente i dati della tabella</returns>
    Public Function LeggiDocProcessati(ByVal piva As String,
                                       ByVal idAgenda As Integer,
                                       ByVal idDocAltro As String,
                                       ByVal xFiltroAggiuntivo As String,
                                       ByVal xOrderBy As String,
                                       ByRef objParametri As AgronicaCoreParametri
                                       ) As DataTable

        '====================================================================================
        'Parametri opzionali :
        '   piva = ""
        '   idAgenda = 0
        '   idDocAltro = ""
        '====================================================================================

        Const nomeRoutine = "Gias_Interscambio_R.LeggiDocProcessati()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM Doc_Processati ")
            stb.AppendLine(" WHERE 1 = 1 ")

            If piva <> "" Then
                stb.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            If idAgenda <> 0 Then
                stb.AppendLine(" AND Id_Agenda = " & Agro_SQL_SaveNum(idAgenda) & " ")
            End If

            If idDocAltro <> "" Then
                stb.AppendLine(" AND Id_Doc_Altro = '" & Agro_SQL_SaveText(idDocAltro) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

#End Region

#Region "Agenzie importate Hash"
    Public Function LeggiHashAgenziaImportata(
                                    ByVal codiceMagazzinoAgenzia As String,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreParametri,
                                    Optional ByRef filtroData As Date = CostantiPersonalizzate.AGRODATAINIZIO
    ) As String

        '====================================================================================
        'Parametri opzionali :
        '   piva = ""
        '====================================================================================

        Const nomeRoutine = "Gias_Interscambio_R.LeggiHashAgenziaImportata()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT Hash ")
            StrSQL.AppendLine(" FROM Agenzie_Importate_Hash ")
            StrSQL.AppendLine(" WHERE 1 = 1 ")

            If filtroData <> AGRODATAINIZIO Then
                StrSQL.AppendLine($" AND Validita_inizio <= {Agro_SQL_SaveDate(filtroData)} ")
                StrSQL.AppendLine($" AND Validita_Fine >= {Agro_SQL_SaveDate(filtroData)} ")
            End If

            If codiceMagazzinoAgenzia <> "" Then
                StrSQL.AppendLine($" AND CodiceMagazzinoAgenzia = '{Agro_SQL_SaveText(codiceMagazzinoAgenzia)}' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine($" AND {Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri)} ")
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine($" ORDER BY {Agro_SQL_Save_xOrderBy(xOrderBy, objParametri)} ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception($"[{nomeRoutine}] : {messaggioErrore}")
        End Try

        If dt.Rows.Count > 0 Then
            Return dt.Rows(0).Field(Of String)("Hash")
        End If

        Return ""

    End Function
#End Region

#Region "Aziende importate Hash"
    Public Function LeggiHashAziendaImportata(
                                ByVal piva As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreParametri,
                                Optional ByRef filtroData As Date = CostantiPersonalizzate.AGRODATAINIZIO
    ) As String

        '====================================================================================
        'Parametri opzionali :
        '   piva = ""
        '====================================================================================

        Const nomeRoutine = "Gias_Interscambio_R.LeggiHashAziendaImportata()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT Hash ")
            StrSQL.AppendLine(" FROM Aziende_Importate_Hash ")
            StrSQL.AppendLine(" WHERE 1 = 1 ")

            If filtroData <> AGRODATAINIZIO Then
                StrSQL.AppendLine($" AND Validita_inizio <= {Agro_SQL_SaveDate(filtroData)} ")
                StrSQL.AppendLine($" AND Validita_Fine >= {Agro_SQL_SaveDate(filtroData)} ")
            End If

            If piva <> "" Then
                StrSQL.AppendLine($" AND Piva = '{Agro_SQL_SaveText(piva)}' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine($" AND {Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri)} ")
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine($" ORDER BY {Agro_SQL_Save_xOrderBy(xOrderBy, objParametri)} ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception($"[{nomeRoutine}] : {messaggioErrore}")
        End Try

        If dt.Rows.Count > 0 Then
            Return dt.Rows(0).Field(Of String)("Hash")
        End If

        Return ""

    End Function
#End Region

End Class


'#################################################################
'#################################################################
'#################################################################

Public Class Gias_Interscambio_W
    Inherits AgronicaCoreDataProvider.DataProvider

#Region "INTERSCAMBIO Tipo 1 (Ruggeri - Ferrarini)"

    '##############################################################################################
    ''' <summary>
    ''' Aggiorna il Cod_Contatto_GIAS e il Cod_Indirizzo_GIAS in un record nella tabella Anagrafica di GIAS INTERSCAMBIO
    ''' </summary>
    ''' <param name="Id_Anag">Primary Key</param>
    ''' <param name="Cod_Contatto_GIAS">Codice contatto GIAS</param>
    ''' <param name="Cod_Indirizzo_GIAS">Codice indirizzo GIAS</param>
    ''' <param name="objParametri">AgronicaCoreParametri per la connessione</param>
    ''' <param name="Data_modifica"></param>
    ''' <param name="username_modifica"></param>
    ''' <returns>True se la scrittura è andata a buon fine, altrimenti False</returns>
    Public Function Aggiorna_Codici_GIAS_Anagrafica(ByVal Id_Anag As Long,
                                                    ByVal Cod_Contatto_GIAS As String,
                                                    ByVal Cod_Indirizzo_GIAS As Integer,
                                                    ByRef objParametri As AgronicaCoreParametri,
                                                    Optional ByVal Data_modifica As Date = #2/1/1900#,
                                                    Optional ByVal username_modifica As String = ""
                                                    ) As Boolean


        Const nomeRoutine = "Gias_Interscambio_W.Aggiorna_Codici_GIAS_Anagrafica()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If

            '---------------------------------------------
            stb.Length = 0
            stb.AppendLine(" UPDATE Anagrafica ")

            stb.AppendLine(" SET ")
            stb.AppendLine(" Cod_Contatto_GIAS = " & Agro_SQL_SaveText_NULL(Cod_Contatto_GIAS))
            stb.AppendLine(" , Cod_Indirizzo_GIAS = " & Agro_SQL_SaveNum_NULL(Cod_Indirizzo_GIAS))
            stb.AppendLine(" , Data_Modifica = " & Agro_SQL_SaveDateTime(Data_modifica))
            stb.AppendLine(" , Username_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "' ")

            stb.AppendLine(" WHERE Id_Anag = " & Agro_SQL_SaveNum(Id_Anag))

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return xRisp

    End Function

    '##############################################################################################
    ''' <summary>
    ''' Scrive un record nella tabella Anagrafica di GIAS INTERSCAMBIO
    ''' </summary>
    ''' <param name="Tipo_Rapporto">Tipo rapporto contabile (C/F/V/....)</param>
    ''' <param name="Piva">Partita Iva Impresa</param>
    ''' <param name="Cod_Contatto_GIAS">Codice contatto GIAS</param>
    ''' <param name="Cod_Indirizzo_GIAS">Codice indirizzo GIAS</param>
    ''' <param name="Cod_Anag_ALTRO">Codice contatto altro programma</param>
    ''' <param name="Cod_Indirizzo_ALTRO">Codice Indirizzo altro programma</param>
    ''' <param name="Rag_Soc">Ragione Sociale contatto (o Cognome e Nome per persona fisica)</param>
    ''' <param name="Indirizzo">Indirizzo contatto</param>
    ''' <param name="Frazione"></param>
    ''' <param name="CAP"></param>
    ''' <param name="Pro_Cod_ISTAT">Codice Provincia ISTAT</param>
    ''' <param name="Pro_cod">Sigla Provincia</param>
    ''' <param name="Com_Cod_ISTAT">Codice Comune ISTAT</param>
    ''' <param name="Com_Des">Nome Comune</param>
    ''' <param name="Stato">Stato</param>
    ''' <param name="Piva_Contatto">Partita Iva Reale del Contatto</param>
    ''' <param name="Codice_Fiscale_Contatto">Coice Fiscale Reale del Contatto</param>
    ''' <param name="objParametri">AgronicaCoreParametri per la connessione</param>
    ''' <param name="Data_creazione"></param>
    ''' <param name="Data_modifica"></param>
    ''' <param name="username_creazione"></param>
    ''' <param name="username_modifica"></param>
    ''' <returns>True se la scrittura è andata a buon fine, altrimenti False</returns>
    ''' <remarks>La PK è una colonna Identity: Id_Anag che viene scritta automaticamente</remarks>
    Public Function ScriviAnagrafica(ByVal Tipo_Rapporto As String,
                                     ByVal Piva As String,
                                     ByVal Cod_Contatto_GIAS As String,
                                     ByVal Cod_Indirizzo_GIAS As Integer,
                                     ByVal Cod_Anag_ALTRO As String,
                                     ByVal Cod_Indirizzo_ALTRO As String,
                                     ByVal Rag_Soc As String,
                                     ByVal Indirizzo As String,
                                     ByVal Frazione As String,
                                     ByVal CAP As String,
                                     ByVal Pro_Cod_ISTAT As String,
                                     ByVal Pro_cod As String,
                                     ByVal Com_Cod_ISTAT As String,
                                     ByVal Com_Des As String,
                                     ByVal Stato As String,
                                     ByVal Piva_Contatto As String,
                                     ByVal Codice_Fiscale_Contatto As String,
                                     ByRef objParametri As AgronicaCoreParametri,
                                     Optional ByVal Data_creazione As Date = #2/1/1900#,
                                     Optional ByVal Data_modifica As Date = #2/1/1900#,
                                     Optional ByVal username_creazione As String = "",
                                     Optional ByVal username_modifica As String = ""
                                     ) As Boolean

        Const nomeRoutine = "Gias_Interscambio_W.ScriviAnagrafica()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_creazione = #2/1/1900# Then
                Data_creazione = Date.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If



            '---------------------------------------------
            stb.Length = 0
            stb.AppendLine(" INSERT INTO Anagrafica ")

            stb.AppendLine("              (")
            stb.AppendLine("              Tipo_Rapporto,        Cod_Contatto_GIAS,      Cod_Indirizzo_GIAS, ")
            stb.AppendLine("              Cod_Anag_ALTRO,       Cod_Indirizzo_ALTRO, ")

            stb.AppendLine("              Partita_Iva,          Codice_Fiscale, ")
            stb.AppendLine("              Rag_Soc,      Indirizzo,          Frazione, ")
            stb.AppendLine("              CAP,          Pro_Cod_ISTAT,      Provincia,    Com_Cod_ISTAT,  Comune,   Stato, ")

            stb.AppendLine("              Piva_SuperUser,     Piva, ")

            stb.AppendLine("              Data_Creazione,     Data_Modifica, ")
            stb.AppendLine("              UserName_Creazione, UserName_Modifica ")
            stb.AppendLine("              ) ")

            stb.AppendLine(" VALUES ( ")


            stb.AppendLine("          '" & Agro_SQL_SaveText(Tipo_Rapporto) & "'  ")
            stb.AppendLine("         , " & Agro_SQL_SaveText_NULL(Cod_Contatto_GIAS) & "  ")
            stb.AppendLine("         , " & Agro_SQL_SaveNum_NULL(Cod_Indirizzo_GIAS) & "  ")

            stb.AppendLine("         , " & Agro_SQL_SaveText_NULL(Cod_Anag_ALTRO) & "  ")
            stb.AppendLine("         , " & Agro_SQL_SaveText_NULL(Cod_Indirizzo_ALTRO) & "  ")

            stb.AppendLine("         , '" & Agro_SQL_SaveText(Piva_Contatto) & "'  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(Codice_Fiscale_Contatto) & "'  ")

            stb.AppendLine("         , '" & Agro_SQL_SaveText(Rag_Soc) & "'  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(Indirizzo) & "'  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(Frazione) & "'  ")

            stb.AppendLine("         , '" & Agro_SQL_SaveText(CAP) & "'  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(Pro_Cod_ISTAT) & "'  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(Pro_cod) & "'  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(Com_Cod_ISTAT) & "'  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(Com_Des) & "'  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(Stato) & "'  ")

            stb.AppendLine("         , '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(Piva) & "'  ")

            stb.AppendLine("		 , " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            stb.AppendLine("		 , " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            stb.AppendLine("		 ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            stb.AppendLine("		 ,'" & Agro_SQL_SaveText(username_modifica) & "' ")



            stb.AppendLine(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return xRisp

    End Function

    '##############################################################################################
    ''' <summary>
    ''' Scrive un record nella tabella Parametri di GIAS INTERSCAMBIO
    ''' </summary>
    ''' <param name="Tipo">Tipo parametro: utilizzare le costanti INTERSCAMBIO_PARAMETRI_* (obbligatorio = PK)</param>
    ''' <param name="Codice_GIAS">Codice del parametro in GIAS (obbligatorio = PK)</param>
    ''' <param name="Codice_ALTRO">Codice del parametro in un altro programma (opzionale = "NULL")</param>
    ''' <param name="objParametri">AgronicaCoreParametri per la connessione</param>
    ''' <param name="Data_creazione"></param>
    ''' <param name="Data_modifica"></param>
    ''' <param name="username_creazione"></param>
    ''' <param name="username_modifica"></param>
    ''' <returns>True se la scrittura è andata a buon fine, altrimenti False</returns>
    ''' <remarks>Per il tipo è possibile utilizzare le costanti personalizzate che cominciano per INTERSCAMBIO_PARAMETRI_*</remarks>
    Public Function ScriviParametri(ByVal tipo As String,
                                    ByVal Codice_GIAS As String,
                                    ByVal Codice_ALTRO As String,
                                    ByRef objParametri As AgronicaCoreParametri,
                                    Optional ByVal Data_creazione As Date = #2/1/1900#,
                                    Optional ByVal Data_modifica As Date = #2/1/1900#,
                                    Optional ByVal username_creazione As String = "",
                                    Optional ByVal username_modifica As String = ""
                                    ) As Boolean

        Const nomeRoutine = "Gias_Interscambio_W.ScriviParametri()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            If tipo Is Nothing OrElse IsDBNull(tipo) OrElse tipo.ToUpper = "NULL" OrElse String.IsNullOrWhiteSpace(tipo) Then
                Throw New Exception("Tipo è Primary Key quindi è obbligatorio")
            End If

            If Codice_GIAS Is Nothing OrElse IsDBNull(Codice_GIAS) OrElse Codice_GIAS.ToUpper = "NULL" OrElse String.IsNullOrWhiteSpace(Codice_GIAS) Then
                Throw New Exception("Codice_GIAS è Primary Key quindi è obbligatorio")
            End If

            If Data_creazione = #2/1/1900# Then
                Data_creazione = Date.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If

            '---------------------------------------------
            stb.Length = 0
            stb.AppendLine(" INSERT INTO Parametri ")

            stb.AppendLine("              (")
            stb.AppendLine("              Tipo,               Codice_GIAS,      Codice_ALTRO, ")
            stb.AppendLine("              Piva_SuperUser, ")

            stb.AppendLine("              Data_Creazione,     Data_Modifica, ")
            stb.AppendLine("              UserName_Creazione, UserName_Modifica ")
            stb.AppendLine("              ) ")

            stb.AppendLine(" VALUES ( ")


            stb.AppendLine("          '" & Agro_SQL_SaveText(tipo) & "'  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(Codice_GIAS) & "'  ")
            stb.AppendLine("         , " & Agro_SQL_SaveText_NULL(Codice_ALTRO) & "  ")

            stb.AppendLine("         , '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")

            stb.AppendLine("		 , " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            stb.AppendLine("		 , " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            stb.AppendLine("		 ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            stb.AppendLine("		 ,'" & Agro_SQL_SaveText(username_modifica) & "' ")

            stb.AppendLine(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    '##############################################################################################
    ''' <summary>
    ''' Aggiorna Elem_Cod, Codice_GIAS e Desc_GIAS in un record nella tabella Prodotti di GIAS INTERSCAMBIO
    ''' </summary>
    ''' <param name="Id_Prod">Primary Key</param>
    ''' <param name="Elem_Cod">Codice categoria GIAS</param>
    ''' <param name="Codice_GIAS">Codice prodotto GIAS</param>
    ''' <param name="Desc_GIAS">Descrizione prodotto GIAS</param>
    ''' <param name="objParametri">AgronicaCoreParametri per la connessione</param>
    ''' <param name="Data_modifica"></param>
    ''' <param name="username_modifica"></param>
    ''' <returns>True se la scrittura è andata a buon fine, altrimenti False</returns>
    Public Function Aggiorna_Codici_GIAS_Prodotti(ByVal Id_Prod As Long,
                                                  ByVal Elem_Cod As Integer,
                                                  ByVal Codice_GIAS As Integer,
                                                  ByVal Desc_GIAS As String,
                                                  ByVal Cod_Articolo As String,
                                                  ByVal Piva As String,
                                                  ByRef objParametri As AgronicaCoreParametri,
                                                  Optional ByVal Data_modifica As Date = #2/1/1900#,
                                                  Optional ByVal username_modifica As String = ""
                                                  ) As Boolean

        Const nomeRoutine = "Gias_Interscambio_W.Aggiorna_Codici_GIAS_Prodotti()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If

            '---------------------------------------------
            stb.Length = 0
            stb.AppendLine(" UPDATE Prodotti ")

            stb.AppendLine(" SET ")
            stb.AppendLine(" Elem_Cod = " & Agro_SQL_SaveNum_NULL(Elem_Cod))
            stb.AppendLine(" , Codice_GIAS = " & Agro_SQL_SaveNum_NULL(Codice_GIAS))
            stb.AppendLine(" , Desc_GIAS = " & Agro_SQL_SaveText_NULL(Desc_GIAS))

            stb.AppendLine(" , Cod_Articolo = (CASE WHEN Cod_Articolo = '' THEN '" & Agro_SQL_SaveText(Cod_Articolo) & "' ELSE Cod_Articolo END) ")

            stb.AppendLine(" , Piva_SuperUser = (CASE WHEN Piva_SuperUser = '' THEN '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ELSE Piva_SuperUser END) ")

            stb.AppendLine(" , Piva = (CASE WHEN Piva = '' THEN '" & Agro_SQL_SaveText(Piva) & "' ELSE Piva END) ")

            stb.AppendLine(" , Data_Modifica = " & Agro_SQL_SaveDateTime(Data_modifica))
            stb.AppendLine(" , Username_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "' ")

            stb.AppendLine(" WHERE Id_Prod = " & Agro_SQL_SaveNum(Id_Prod))

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return xRisp

    End Function

    '##############################################################################################
    ''' <summary>
    ''' Aggiorna Categoria_ALTRO, Codice_ALTRO, Desc_ALTRO e Note in un record nella tabella Prodotti di GIAS INTERSCAMBIO
    ''' </summary>
    ''' <param name="Id_Prod">Primary Key</param>
    ''' <param name="Categoria_ALTRO">Categoria ALTRO</param>
    ''' <param name="Codice_ALTRO">Codice prodotto ALTRO</param>
    ''' <param name="Desc_ALTRO">Descrizione prodotto ALTRO</param>
    ''' <param name="Note">Note</param>
    ''' <param name="objParametri">AgronicaCoreParametri per la connessione</param>
    ''' <param name="Data_modifica"></param>
    ''' <param name="username_modifica"></param>
    ''' <returns>True se la scrittura è andata a buon fine, altrimenti False</returns>
    Public Function Aggiorna_Codici_ALTRO_Prodotti(ByVal Id_Prod As Long,
                                                   ByVal Categoria_ALTRO As String,
                                                   ByVal Codice_ALTRO As String,
                                                   ByVal Desc_ALTRO As String,
                                                   ByVal Note As String,
                                                   ByRef objParametri As AgronicaCoreParametri,
                                                   Optional ByVal Data_modifica As Date = #2/1/1900#,
                                                   Optional ByVal username_modifica As String = ""
                                                   ) As Boolean

        Const nomeRoutine = "Gias_Interscambio_W.Aggiorna_Codici_ALTRO_Prodotti"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If

            '---------------------------------------------
            stb.Length = 0
            stb.AppendLine(" UPDATE Prodotti ")

            stb.AppendLine(" SET ")
            stb.AppendLine(" Cod_Prodotto_Cliente = (CASE WHEN Cod_Prodotto_Cliente = '' THEN '" & Agro_SQL_SaveText(Codice_ALTRO) & "' ELSE Cod_Prodotto_Cliente END) ")
            stb.AppendLine(" , Desc_Prodotto_Cliente = (CASE WHEN Desc_Prodotto_Cliente = '' THEN '" & Agro_SQL_SaveText(Desc_ALTRO) & "' ELSE Desc_Prodotto_Cliente END) ")
            stb.AppendLine(" , Categoria_Prodotto_Cliente = (CASE WHEN Categoria_Prodotto_Cliente = '' THEN '" & Agro_SQL_SaveText(Categoria_ALTRO) & "' ELSE Categoria_Prodotto_Cliente END) ")

            stb.AppendLine(" , Note = '" & Agro_SQL_SaveText(Note) & "' ")

            stb.AppendLine(" , Data_Modifica = " & Agro_SQL_SaveDateTime(Data_modifica))
            stb.AppendLine(" , Username_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "' ")

            stb.AppendLine(" WHERE Id_Prod = " & Agro_SQL_SaveNum(Id_Prod))

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return xRisp

    End Function


    '##############################################################################################
    ''' <summary>
    ''' Scrive un record nella tabella Prodotti di GIAS INTERSCAMBIO
    ''' </summary>
    ''' <param name="Elem_Cod">Codice categoria GIAS (obbligatorio = PK)</param>
    ''' <param name="Cod_Prodotto_Cliente">Codice prodotto altro programma (obbligatorio = PK = "")</param>
    ''' <param name="Desc_Prodotto_Cliente">Descrizione prodotto altro programma (opzionale = "")</param>
    ''' <param name="Categoria_Prodotto_Cliente">Codice categoria prodotto altro programma (opzionale = "")</param>
    ''' <param name="Codice_GIAS">Codice prodotto GIAS (obbligatorio = PK)</param>
    ''' <param name="Desc_GIAS">Descrizione prodotto GIAS (opzionale = "")</param>
    ''' <param name="Note"></param>
    ''' <param name="Piva"></param>
    ''' <param name="Cod_Articolo">Codice Articolo su GIAS/altro programma (obbligatorio = PK = "")</param>
    ''' <param name="Tipo_Codifica">enum_Tipo_CAC_Codifica_ProdottiAziendali (obbligatorio = PK = 0)</param>
    ''' <param name="objParametri">AgronicaCoreParametri per la connessione</param>
    ''' <param name="Data_creazione"></param>
    ''' <param name="Data_modifica"></param>
    ''' <param name="username_creazione"></param>
    ''' <param name="username_modifica"></param>
    ''' <returns>True se la scrittura è andata a buon fine, altrimenti False</returns>
    ''' <remarks></remarks>
    Public Function ScriviProdotti(ByVal Elem_Cod As Integer,
                                   ByVal Cod_Prodotto_Cliente As String,
                                   ByVal Desc_Prodotto_Cliente As String,
                                   ByVal Categoria_Prodotto_Cliente As String,
                                   ByVal Codice_GIAS As Integer,
                                   ByVal Desc_GIAS As String,
                                   ByVal Note As String,
                                   ByVal Piva As String,
                                   ByVal Cod_Articolo As String,
                                   ByVal Tipo_Codifica As enum_Tipo_CAC_Codifica_ProdottiAziendali,
                                   ByRef objParametri As AgronicaCoreParametri,
                                   Optional ByVal Data_creazione As Date = #2/1/1900#,
                                   Optional ByVal Data_modifica As Date = #2/1/1900#,
                                   Optional ByVal username_creazione As String = "",
                                   Optional ByVal username_modifica As String = ""
                                   ) As Boolean

        Const nomeRoutine = "Gias_Interscambio_W.ScriviProdotti()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_creazione = #2/1/1900# Then
                Data_creazione = Date.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If

            '---------------------------------------------
            stb.Length = 0
            stb.AppendLine(" INSERT INTO Prodotti ")

            stb.AppendLine("              (")
            stb.AppendLine("              Elem_Cod,                     Codice_GIAS,            Desc_GIAS, ")
            stb.AppendLine("              Categoria_Prodotto_Cliente,   Cod_Prodotto_Cliente,   Desc_Prodotto_Cliente, ")
            stb.AppendLine("              Note,                         Cod_Articolo,           Tipo_Codifica, ")

            stb.AppendLine("              Piva_SuperUser,               Piva, ")

            stb.AppendLine("              Data_Creazione,               Data_Modifica, ")
            stb.AppendLine("              UserName_Creazione,           UserName_Modifica ")
            stb.AppendLine("              ) ")

            stb.AppendLine(" VALUES ( ")

            stb.AppendLine("          " & Agro_SQL_SaveNum(Elem_Cod) & "  ")
            stb.AppendLine("         , " & Agro_SQL_SaveNum(Codice_GIAS) & "  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(Desc_GIAS) & "'  ")

            stb.AppendLine("         , '" & Agro_SQL_SaveText(Categoria_Prodotto_Cliente) & "'  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(Cod_Prodotto_Cliente) & "'  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(Desc_Prodotto_Cliente) & "'  ")

            stb.AppendLine("         , '" & Agro_SQL_SaveText(Note) & "'  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(Cod_Articolo) & "'  ")
            stb.AppendLine("         , " & Agro_SQL_SaveNum(Tipo_Codifica) & "  ")

            stb.AppendLine("         , '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(Piva) & "'  ")

            stb.AppendLine("		 , " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            stb.AppendLine("		 , " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            stb.AppendLine("		 ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            stb.AppendLine("		 ,'" & Agro_SQL_SaveText(username_modifica) & "' ")

            stb.AppendLine(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return xRisp

    End Function

#End Region

#Region "INTERSCAMBIO Tipo 2 (Aboca)"

    '##############################################################################################
    ''' <summary>
    ''' Scrive un record nella tabella Codici_Anagrafiche di GIAS INTERSCAMBIO
    ''' </summary>
    ''' <param name="Cod_Contatto_ALTRO">Codice contatto altro programma</param>
    ''' <param name="Cod_Contatto_GIAS">Codice contatto GIAS</param>
    ''' <param name="Piva">Partita Iva Impresa</param>
    ''' <param name="objParametri">AgronicaCoreParametri per la connessione</param>
    ''' <param name="suffissoColonnaAltro">suffisso sul nome della colonna per l'altro gestionale (ad es: "SAP" che verrà aggiunto a "Cod_Contatto_" (se non specificato = "ALTRO")</param>
    ''' <param name="Validita_Inizio"></param>
    ''' <param name="Validita_Fine"></param>
    ''' <param name="Data_creazione"></param>
    ''' <param name="Data_modifica"></param>
    ''' <param name="username_creazione"></param>
    ''' <param name="username_modifica"></param>
    ''' <param name="cuaa">Usato per modalità Demetra: il Cod_Contatto_Altro da solo non sempre è univoco, ma a volte va usato in abbinato con il cuaa.Se non gestito passare Nothing, perchè altrimenti si aspetta di trovare la colonna</param>
    ''' <returns>True se la scrittura è andata a buon fine, altrimenti False</returns>
    ''' <remarks>La PK è una colonna Identity: Id_Anag che viene scritta automaticamente</remarks>
    Public Function Scrivi_CodiciAnagrafiche(ByVal Cod_Contatto_ALTRO As String,
                                             ByVal Cod_Contatto_GIAS As String,
                                             ByVal Descr_Contatto As String,
                                             ByVal Piva As String,
                                             ByRef objParametri As AgronicaCoreParametri,
                                             Optional ByVal suffissoColonnaAltro As String = "ALTRO",
                                             Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                                             Optional ByVal Validita_Fine As Date = AGRODATAFINE,
                                             Optional ByVal Data_creazione As Date = #2/1/1900#,
                                             Optional ByVal Data_modifica As Date = #2/1/1900#,
                                             Optional ByVal username_creazione As String = "",
                                             Optional ByVal username_modifica As String = "",
                                             Optional ByVal cuaa As String = Nothing
                                             ) As Boolean

        Const nomeRoutine = "Gias_Interscambio_W.Scrivi_CodiciAnagrafiche()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_creazione = #2/1/1900# Then
                Data_creazione = Date.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If


            '---------------------------------------------
            stb.Length = 0
            stb.AppendLine(" INSERT INTO Codici_Anagrafiche ")

            stb.AppendLine("              (")
            stb.AppendLine("              [Cod_Contatto_" & suffissoColonnaAltro & "], ")
            stb.AppendLine("              Cod_Contatto_GIAS, ")

            stb.AppendLine("              Descr_Contatto,     Piva, ")

            stb.AppendLine("              Data_Creazione,     Data_Modifica, ")
            stb.AppendLine("              Username_Creazione, Username_Modifica, ")
            stb.AppendLine("              Validita_Inizio,    Validita_Fine ")

            If cuaa IsNot Nothing Then
                stb.AppendLine("              , CuaaInput ")
            End If

            stb.AppendLine("              ) ")

            stb.AppendLine(" VALUES ( ")

            stb.AppendLine("          " & Agro_SQL_SaveText_NULL(Cod_Contatto_ALTRO) & "  ")
            stb.AppendLine("         , " & Agro_SQL_SaveText_NULL(Cod_Contatto_GIAS) & "  ")

            stb.AppendLine("         , '" & Agro_SQL_SaveText(Descr_Contatto) & "'  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(Piva) & "'  ")

            stb.AppendLine("		 , " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            stb.AppendLine("		 , " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            stb.AppendLine("		 ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            stb.AppendLine("		 ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            stb.AppendLine("		 , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            stb.AppendLine("		 , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            If cuaa IsNot Nothing Then
                stb.AppendLine("              ,'" & Agro_SQL_SaveText(cuaa) & "' ")
            End If

            stb.AppendLine(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return xRisp

    End Function

    '##############################################################################################
    Public Function Modifica_CodiciAnagrafiche(ByVal Id_Anag As Integer,
                                               ByRef objParametri As AgronicaCoreParametri,
                                               Optional ByVal suffissoColonnaAltro As String = "ALTRO",
                                               Optional ByVal Cod_Contatto_ALTRO As String = Nothing,
                                               Optional ByVal Cod_Contatto_GIAS As String = Nothing,
                                               Optional ByVal Descr_Contatto As String = Nothing,
                                               Optional ByVal Piva As String = Nothing,
                                               Optional ByVal Validita_Inizio As Date? = Nothing,
                                               Optional ByVal Validita_Fine As Date? = Nothing,
                                               Optional ByVal Data_Modifica As DateTime = #2/1/1900#,
                                               Optional ByVal Username_Modifica As String = ""
                                               ) As Boolean

        Const nomeRoutine = "Gias_Interscambio_W.Modifica_CodiciAnagrafiche()"

        '====================================================================================
        'Parametri opzionali :
        '   Tutti i valori non chiave (se impostati a nothing o non passati 
        '   non ne verrà fatto l'aggiornamento e rimarranno i valori precedenti)
        '====================================================================================

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

            If Id_Anag = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Anag obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE Codici_Anagrafiche ")
            strSql.AppendLine(" SET Data_Modifica = " & Agro_SQL_SaveDateTime(Data_Modifica) & " ")
            strSql.AppendLine("   , Username_Modifica = '" & Agro_SQL_SaveText(Username_Modifica) & "' ")

            If Not IsNothing(Cod_Contatto_ALTRO) Then
                strSql.AppendLine("   , [Cod_Contatto_" & suffissoColonnaAltro & "] = '" & Agro_SQL_SaveText(Cod_Contatto_ALTRO) & "' ")
            End If

            If Not IsNothing(Cod_Contatto_GIAS) Then
                strSql.AppendLine("   , Cod_Contatto_GIAS = '" & Agro_SQL_SaveText(Cod_Contatto_GIAS) & "' ")
            End If

            If Not IsNothing(Descr_Contatto) Then
                strSql.AppendLine("   , Descr_Contatto = '" & Agro_SQL_SaveText(Descr_Contatto) & "' ")
            End If

            If Not IsNothing(Piva) Then
                strSql.AppendLine("   , Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Not IsNothing(Validita_Inizio) Then
                strSql.AppendLine("   , Validita_Inizio = " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            End If

            If Not IsNothing(Validita_Fine) Then
                strSql.AppendLine("   , Validita_Fine = " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            End If



            strSql.AppendLine(" WHERE Id_Anag = " & Agro_SQL_SaveNum(Id_Anag) & " ")

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

    '##############################################################################################
    ''' <summary>
    ''' Elimina un record nella tabella Codici_Anagrafiche di GIAS INTERSCAMBIO
    ''' </summary>
    ''' <param name="Id_Anag">Chiave della tabella</param>
    ''' <returns>True se la cancellazione è andata a buon fine, altrimenti False</returns>
    ''' <remarks>La PK è una colonna Identity: Id_Anag che viene scritta automaticamente</remarks>
    Public Function Cancella_CodiciAnagrafiche(ByVal Id_Anag As Integer,
                                               ByRef objParametri As AgronicaCoreParametri
                                               ) As Boolean

        Const nomeRoutine = "Gias_Interscambio_W.Cancella_CodiciAnagrafiche()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Id_Anag = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Anag obbligatorio)")
            End If

            stb.Length = 0
            stb.AppendLine(" DELETE FROM Codici_Anagrafiche ")
            stb.AppendLine(" WHERE Id_Anag = " & Agro_SQL_SaveNum(Id_Anag) & " ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    '##############################################################################################
    ''' <summary>
    ''' Scrive un record nella tabella Codici_AnagraficheConIndirizzi di GIAS INTERSCAMBIO
    ''' </summary>
    ''' <param name="Cod_Contatto_ALTRO">Codice contatto altro programma</param>
    ''' <param name="Cod_Indirizzo_ALTRO">Codice indirizzo altro programma</param>
    ''' <param name="Cod_Contatto_GIAS">Codice contatto GIAS</param>
    ''' <param name="Cod_Indirizzo_GIAS">Codice indirizzo GIAS</param>
    ''' <param name="Piva">Partita Iva Impresa</param>
    ''' <param name="objParametri">AgronicaCoreParametri per la connessione</param>
    ''' <param name="suffissoColonnaAltro">suffisso sul nome della colonna per l'altro gestionale (ad es: "SAP" che verrà aggiunto a "Cod_Contatto_" (se non specificato = "ALTRO")</param>
    ''' <param name="Validita_Inizio"></param>
    ''' <param name="Validita_Fine"></param>
    ''' <param name="Data_creazione"></param>
    ''' <param name="Data_modifica"></param>
    ''' <param name="username_creazione"></param>
    ''' <param name="username_modifica"></param>
    ''' <returns>True se la scrittura è andata a buon fine, altrimenti False</returns>
    ''' <remarks>La PK è una colonna Identity: Id_AnagInd che viene scritta automaticamente</remarks>
    Public Function Scrivi_CodiciAnagraficheConIndirizzi(ByVal Cod_Contatto_ALTRO As String,
                                                         ByVal Cod_Indirizzo_ALTRO As String,
                                                         ByVal Cod_Contatto_GIAS As String,
                                                         ByVal Cod_Indirizzo_GIAS As Integer,
                                                         ByVal Descr_Anagrafica As String,
                                                         ByVal Piva As String,
                                                         ByRef objParametri As AgronicaCoreParametri,
                                                         Optional ByVal suffissoColonnaAltro As String = "ALTRO",
                                                         Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                                                         Optional ByVal Validita_Fine As Date = AGRODATAFINE,
                                                         Optional ByVal Data_creazione As Date = #2/1/1900#,
                                                         Optional ByVal Data_modifica As Date = #2/1/1900#,
                                                         Optional ByVal username_creazione As String = "",
                                                         Optional ByVal username_modifica As String = ""
                                                         ) As Boolean

        Const nomeRoutine = "Gias_Interscambio_W.Scrivi_CodiciAnagraficheConIndirizzi()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_creazione = #2/1/1900# Then
                Data_creazione = Date.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If


            '---------------------------------------------
            stb.Length = 0
            stb.AppendLine(" INSERT INTO Codici_AnagraficheConIndirizzi ")

            stb.AppendLine("              (")
            stb.AppendLine("              [Cod_Contatto_" & suffissoColonnaAltro & "], ")
            stb.AppendLine("              [Cod_Indirizzo_" & suffissoColonnaAltro & "], ")
            stb.AppendLine("              Cod_Contatto_GIAS, ")
            stb.AppendLine("              Cod_Indirizzo_GIAS, ")

            stb.AppendLine("              Descr_Anagrafica,     Piva, ")

            stb.AppendLine("              Data_Creazione,     Data_Modifica, ")
            stb.AppendLine("              Username_Creazione, Username_Modifica, ")
            stb.AppendLine("              Validita_Inizio,    Validita_Fine ")
            stb.AppendLine("              ) ")

            stb.AppendLine(" VALUES ( ")

            stb.AppendLine("          " & Agro_SQL_SaveText_NULL(Cod_Contatto_ALTRO) & "  ")
            stb.AppendLine("         , " & Agro_SQL_SaveText_NULL(Cod_Indirizzo_ALTRO) & "  ")
            stb.AppendLine("         , " & Agro_SQL_SaveText_NULL(Cod_Contatto_GIAS) & "  ")
            stb.AppendLine("         , " & Agro_SQL_SaveNum_NULL(Cod_Indirizzo_GIAS) & "  ")

            stb.AppendLine("         , '" & Agro_SQL_SaveText(Descr_Anagrafica) & "'  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(Piva) & "'  ")

            stb.AppendLine("		 , " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            stb.AppendLine("		 , " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            stb.AppendLine("		 ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            stb.AppendLine("		 ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            stb.AppendLine("		 , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            stb.AppendLine("		 , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            stb.AppendLine(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return xRisp

    End Function

    '##############################################################################################
    Public Function Modifica_CodiciAnagraficheConIndirizzi(ByVal Id_Anag As Integer,
                                                           ByRef objParametri As AgronicaCoreParametri,
                                                           Optional ByVal suffissoColonnaAltro As String = "ALTRO",
                                                           Optional ByVal Cod_Contatto_ALTRO As String = Nothing,
                                                           Optional ByVal Cod_Indirizzo_ALTRO As String = Nothing,
                                                           Optional ByVal Cod_Contatto_GIAS As String = Nothing,
                                                           Optional ByVal Cod_Indirizzo_GIAS As Integer? = Nothing,
                                                           Optional ByVal Descr_Anagrafica As String = Nothing,
                                                           Optional ByVal Piva As String = Nothing,
                                                           Optional ByVal Validita_Inizio As Date? = Nothing,
                                                           Optional ByVal Validita_Fine As Date? = Nothing,
                                                           Optional ByVal Data_Modifica As DateTime = #2/1/1900#,
                                                           Optional ByVal Username_Modifica As String = ""
                                                           ) As Boolean

        Const nomeRoutine = "Gias_Interscambio_W.Modifica_CodiciAnagraficheConIndirizzi()"

        '====================================================================================
        'Parametri opzionali :
        '   Tutti i valori non chiave (se impostati a nothing o non passati 
        '   non ne verrà fatto l'aggiornamento e rimarranno i valori precedenti)
        '====================================================================================

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

            If Id_Anag = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Anag obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE Codici_AnagraficheConIndirizzi ")
            strSql.AppendLine(" SET Data_Modifica = " & Agro_SQL_SaveDateTime(Data_Modifica) & " ")
            strSql.AppendLine("   , Username_Modifica = '" & Agro_SQL_SaveText(Username_Modifica) & "' ")

            If Not IsNothing(Cod_Contatto_ALTRO) Then
                strSql.AppendLine("   , [Cod_Contatto_" & suffissoColonnaAltro & "] = '" & Agro_SQL_SaveText(Cod_Contatto_ALTRO) & "' ")
            End If

            If Not IsNothing(Cod_Indirizzo_ALTRO) Then
                strSql.AppendLine("   , [Cod_Indirizzo_" & suffissoColonnaAltro & "] = '" & Agro_SQL_SaveText(Cod_Indirizzo_ALTRO) & "' ")
            End If

            If Not IsNothing(Cod_Contatto_GIAS) Then
                strSql.AppendLine("   , Cod_Contatto_GIAS = '" & Agro_SQL_SaveText(Cod_Contatto_GIAS) & "' ")
            End If

            If Not IsNothing(Cod_Indirizzo_GIAS) Then
                strSql.AppendLine("   , Cod_Indirizzo_GIAS = " & Agro_SQL_SaveNum(Cod_Indirizzo_GIAS) & " ")
            End If

            If Not IsNothing(Descr_Anagrafica) Then
                strSql.AppendLine("   , Descr_Anagrafica = '" & Agro_SQL_SaveText(Descr_Anagrafica) & "' ")
            End If

            If Not IsNothing(Piva) Then
                strSql.AppendLine("   , Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Not IsNothing(Validita_Inizio) Then
                strSql.AppendLine("   , Validita_Inizio = " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            End If

            If Not IsNothing(Validita_Fine) Then
                strSql.AppendLine("   , Validita_Fine = " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            End If



            strSql.AppendLine(" WHERE Id_AnagInd = " & Agro_SQL_SaveNum(Id_Anag) & " ")

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




    '##############################################################################################
    ''' <summary>
    ''' Scrive un record nella tabella Parametri di GIAS INTERSCAMBIO
    ''' </summary>
    ''' <param name="Tipo">Tipo parametro: utilizzare le costanti INTERSCAMBIO_ATTIVITA_* (obbligatorio)</param>
    ''' <param name="Cod_Attivita_GIAS">Codice dell'Attività in GIAS</param>
    ''' <param name="Cod_Attivita_ALTRO">Codice dell'Attività in un altro programma (opzionale = "NULL")</param>
    ''' <param name="Cod_CentroLavoro_ALTRO">Codice del Centro di Lavoro in un altro programma (opzionale = "NULL")</param>
    ''' <param name="Descr_Attivita">Descrizione dell'Attività</param>
    ''' <param name="objParametri">AgronicaCoreParametri per la connessione</param>
    ''' <param name="suffissoColonnaAltro">suffisso sul nome della colonna per l'altro gestionale (ad es: "SAP" che verrà aggiunto a "Cod_Attivita_" (se non specificato = "ALTRO")</param>
    ''' <param name="Validita_Inizio"></param>
    ''' <param name="Validita_Fine"></param>
    ''' <param name="Data_creazione"></param>
    ''' <param name="Data_modifica"></param>
    ''' <param name="username_creazione"></param>
    ''' <param name="username_modifica"></param>
    ''' <returns>True se la scrittura è andata a buon fine, altrimenti False</returns>
    ''' <remarks>Per il tipo è possibile utilizzare le costanti personalizzate che cominciano per INTERSCAMBIO_ATTIVITA_*</remarks>
    Public Function Scrivi_CodiciAttivita(ByVal Tipo As String,
                                          ByVal Cod_Attivita_GIAS As Integer,
                                          ByVal Cod_Attivita_ALTRO As String,
                                          ByVal Cod_CentroLavoro_ALTRO As String,
                                          ByVal Descr_Attivita As String,
                                          ByVal Piva As String,
                                          ByRef objParametri As AgronicaCoreParametri,
                                          Optional ByVal suffissoColonnaAltro As String = "ALTRO",
                                          Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                                          Optional ByVal Validita_Fine As Date = AGRODATAFINE,
                                          Optional ByVal Data_creazione As Date = #2/1/1900#,
                                          Optional ByVal Data_modifica As Date = #2/1/1900#,
                                          Optional ByVal username_creazione As String = "",
                                          Optional ByVal username_modifica As String = ""
                                         ) As Boolean

        Const nomeRoutine = "Gias_Interscambio_W.Scrivi_CodiciAttivita()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            If Tipo Is Nothing OrElse IsDBNull(Tipo) OrElse Tipo.ToUpper = "NULL" OrElse String.IsNullOrWhiteSpace(Tipo) Then
                Throw New Exception("Tipo è obbligatorio")
            End If

            If Data_creazione = #2/1/1900# Then
                Data_creazione = Date.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If


            '---------------------------------------------
            stb.Length = 0
            stb.AppendLine(" INSERT INTO Codici_Parametri ")

            stb.AppendLine("              (")
            stb.AppendLine("              Tipo,               Cod_Attivita_GIAS, ")
            stb.AppendLine("              [Cod_Attivita_" & suffissoColonnaAltro & "], ")
            stb.AppendLine("              [Cod_CentroLavoro_" & suffissoColonnaAltro & "], ")

            stb.AppendLine("              Descr_Attivita,     Piva, ")

            stb.AppendLine("              Data_Creazione,     Data_Modifica, ")
            stb.AppendLine("              Username_Creazione, Username_Modifica, ")
            stb.AppendLine("              Validita_Inizio,    Validita_Fine ")
            stb.AppendLine("              ) ")

            stb.AppendLine(" VALUES ( ")

            stb.AppendLine("          '" & Agro_SQL_SaveText(Tipo) & "'  ")
            stb.AppendLine("         , " & Agro_SQL_SaveNum(Cod_Attivita_GIAS) & "  ")
            stb.AppendLine("         , " & Agro_SQL_SaveText_NULL(Cod_Attivita_ALTRO) & "  ")
            stb.AppendLine("         , " & Agro_SQL_SaveText_NULL(Cod_CentroLavoro_ALTRO) & "  ")

            stb.AppendLine("         , '" & Agro_SQL_SaveText(Descr_Attivita) & "'  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(Piva) & "'  ")

            stb.AppendLine("		 , " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            stb.AppendLine("		 , " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            stb.AppendLine("		 ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            stb.AppendLine("		 ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            stb.AppendLine("		 , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            stb.AppendLine("		 , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            stb.AppendLine(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return xRisp

    End Function

    '##############################################################################################
    Public Function Modifica_CodiciAttivita(ByVal Id_Attivita As Integer,
                                            ByRef objParametri As AgronicaCoreParametri,
                                            Optional ByVal suffissoColonnaAltro As String = "ALTRO",
                                            Optional ByVal Tipo As String = Nothing,
                                            Optional ByVal Cod_Attivita_GIAS As Integer? = Nothing,
                                            Optional ByVal Cod_Attivita_ALTRO As String = Nothing,
                                            Optional ByVal Cod_CentroLavoro_ALTRO As String = Nothing,
                                            Optional ByVal Descr_Attivita As String = Nothing,
                                            Optional ByVal Piva As String = Nothing,
                                            Optional ByVal Validita_Inizio As Date? = Nothing,
                                            Optional ByVal Validita_Fine As Date? = Nothing,
                                            Optional ByVal Data_Modifica As DateTime = #2/1/1900#,
                                            Optional ByVal Username_Modifica As String = ""
                                            ) As Boolean

        Const nomeRoutine = "Gias_Interscambio_W.Modifica_CodiciAttivita()"

        '====================================================================================
        'Parametri opzionali :
        '   Tutti i valori non chiave (se impostati a nothing o non passati 
        '   non ne verrà fatto l'aggiornamento e rimarranno i valori precedenti)
        '====================================================================================

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

            If Id_Attivita = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Attivita obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE Codici_Attivita ")
            strSql.AppendLine(" SET Data_Modifica = " & Agro_SQL_SaveDateTime(Data_Modifica) & " ")
            strSql.AppendLine("   , Username_Modifica = '" & Agro_SQL_SaveText(Username_Modifica) & "' ")

            If Not IsNothing(Tipo) Then
                strSql.AppendLine("   , Tipo = '" & Agro_SQL_SaveText(Tipo) & "' ")
            End If

            If Not IsNothing(Cod_Attivita_GIAS) Then
                strSql.AppendLine("   , Cod_Attivita_GIAS = " & Agro_SQL_SaveNum(Cod_Attivita_GIAS) & " ")
            End If

            If Not IsNothing(Cod_Attivita_ALTRO) Then
                strSql.AppendLine("   , [Cod_Attivita_" & suffissoColonnaAltro & "] = '" & Agro_SQL_SaveText(Cod_Attivita_ALTRO) & "' ")
            End If

            If Not IsNothing(Cod_CentroLavoro_ALTRO) Then
                strSql.AppendLine("   , [Cod_CentroLavoro_" & suffissoColonnaAltro & "] = '" & Agro_SQL_SaveText(Cod_CentroLavoro_ALTRO) & "' ")
            End If

            If Not IsNothing(Descr_Attivita) Then
                strSql.AppendLine("   , Descr_Attivita = '" & Agro_SQL_SaveText(Descr_Attivita) & "' ")
            End If

            If Not IsNothing(Piva) Then
                strSql.AppendLine("   , Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Not IsNothing(Validita_Inizio) Then
                strSql.AppendLine("   , Validita_Inizio = " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            End If

            If Not IsNothing(Validita_Fine) Then
                strSql.AppendLine("   , Validita_Fine = " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            End If



            strSql.AppendLine(" WHERE Id_Attivita = " & Agro_SQL_SaveNum(Id_Attivita) & " ")

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

    '##############################################################################################
    ''' <summary>
    ''' Scrive un record nella tabella Parametri di GIAS INTERSCAMBIO
    ''' </summary>
    ''' <param name="Tipo">Tipo parametro: utilizzare le costanti INTERSCAMBIO_PARAMETRI_* (obbligatorio)</param>
    ''' <param name="Codice_GIAS">Codice del parametro in GIAS</param>
    ''' <param name="Codice_ALTRO">Codice del parametro in un altro programma (opzionale = "NULL")</param>
    ''' <param name="Descr_Param">Descrizione del parametro</param>
    ''' <param name="objParametri">AgronicaCoreParametri per la connessione</param>
    ''' <param name="suffissoColonnaAltro">suffisso sul nome della colonna per l'altro gestionale (ad es: "SAP" che verrà aggiunto a "Codice_" (se non specificato = "ALTRO")</param>
    ''' <param name="Validita_Inizio"></param>
    ''' <param name="Validita_Fine"></param>
    ''' <param name="Data_creazione"></param>
    ''' <param name="Data_modifica"></param>
    ''' <param name="username_creazione"></param>
    ''' <param name="username_modifica"></param>
    ''' <returns>True se la scrittura è andata a buon fine, altrimenti False</returns>
    ''' <remarks>Per il tipo è possibile utilizzare le costanti personalizzate che cominciano per INTERSCAMBIO_PARAMETRI_*</remarks>
    Public Function Scrivi_CodiciParametri(ByVal Tipo As String,
                                           ByVal Codice_GIAS As String,
                                           ByVal Codice_ALTRO As String,
                                           ByVal Descr_Param As String,
                                           ByVal Piva As String,
                                           ByRef objParametri As AgronicaCoreParametri,
                                           Optional ByVal suffissoColonnaAltro As String = "ALTRO",
                                           Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                                           Optional ByVal Validita_Fine As Date = AGRODATAFINE,
                                           Optional ByVal Data_creazione As Date = #2/1/1900#,
                                           Optional ByVal Data_modifica As Date = #2/1/1900#,
                                           Optional ByVal username_creazione As String = "",
                                           Optional ByVal username_modifica As String = ""
                                           ) As Boolean

        Const nomeRoutine = "Gias_Interscambio_W.Scrivi_CodiciParametri()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            If Tipo Is Nothing OrElse IsDBNull(Tipo) OrElse Tipo.ToUpper = "NULL" OrElse String.IsNullOrWhiteSpace(Tipo) Then
                Throw New Exception("Tipo è obbligatorio")
            End If

            If Data_creazione = #2/1/1900# Then
                Data_creazione = Date.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If


            '---------------------------------------------
            stb.Length = 0
            stb.AppendLine(" INSERT INTO Codici_Parametri ")

            stb.AppendLine("              (")
            stb.AppendLine("              Tipo,               Codice_GIAS, ")
            stb.AppendLine("              [Codice_" & suffissoColonnaAltro & "], ")

            stb.AppendLine("              Descr_Param,        Piva, ")

            stb.AppendLine("              Data_Creazione,     Data_Modifica, ")
            stb.AppendLine("              Username_Creazione, Username_Modifica, ")
            stb.AppendLine("              Validita_Inizio,    Validita_Fine ")
            stb.AppendLine("              ) ")

            stb.AppendLine(" VALUES ( ")

            stb.AppendLine("          '" & Agro_SQL_SaveText(Tipo) & "'  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(Codice_GIAS) & "'  ")
            stb.AppendLine("         , " & Agro_SQL_SaveText_NULL(Codice_ALTRO) & "  ")

            stb.AppendLine("         , '" & Agro_SQL_SaveText(Descr_Param) & "'  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(Piva) & "'  ")

            stb.AppendLine("		 , " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            stb.AppendLine("		 , " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            stb.AppendLine("		 ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            stb.AppendLine("		 ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            stb.AppendLine("		 , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            stb.AppendLine("		 , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            stb.AppendLine(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return xRisp

    End Function

    '##############################################################################################
    Public Function Modifica_CodiciParametri(ByVal idParam As Integer,
                                             ByRef objParametri As AgronicaCoreParametri,
                                             Optional ByVal suffissoColonnaAltro As String = "ALTRO",
                                             Optional ByVal tipo As String = Nothing,
                                             Optional ByVal Codice_GIAS As String = Nothing,
                                             Optional ByVal Codice_ALTRO As String = Nothing,
                                             Optional ByVal descrParam As String = Nothing,
                                             Optional ByVal piva As String = Nothing,
                                             Optional ByVal validitaInizio As Date? = Nothing,
                                             Optional ByVal validitaFine As Date? = Nothing,
                                             Optional ByVal dataModifica As DateTime = #2/1/1900#,
                                             Optional ByVal usernameModifica As String = ""
                                             ) As Boolean

        Const nomeRoutine = "Gias_Interscambio_W.Modifica_CodiciParametri()"

        '====================================================================================
        'Parametri opzionali :
        '   Tutti i valori non chiave (se impostati a nothing o non passati 
        '   non ne verrà fatto l'aggiornamento e rimarranno i valori precedenti)
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            If dataModifica = #2/1/1900# Then
                dataModifica = Date.Now
            End If

            If usernameModifica = "" Then
                usernameModifica = objParametri.UsernameOperazione
            End If

            If idParam = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Param obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE Codici_Parametri ")
            strSql.AppendLine(" SET Data_Modifica = " & Agro_SQL_SaveDateTime(dataModifica) & " ")
            strSql.AppendLine("   , Username_Modifica = '" & Agro_SQL_SaveText(usernameModifica) & "' ")

            If Not IsNothing(tipo) Then
                strSql.AppendLine("   , Tipo = '" & Agro_SQL_SaveText(tipo) & "' ")
            End If

            If Not IsNothing(Codice_GIAS) Then
                strSql.AppendLine("   , Codice_GIAS = '" & Agro_SQL_SaveText(Codice_GIAS) & "' ")
            End If

            If Not IsNothing(Codice_ALTRO) Then
                strSql.AppendLine("   , [Codice_" & suffissoColonnaAltro & "] = '" & Agro_SQL_SaveText(Codice_ALTRO) & "' ")
            End If

            If Not IsNothing(descrParam) Then
                strSql.AppendLine("   , Descr_Param = '" & Agro_SQL_SaveText(descrParam) & "' ")
            End If

            If Not IsNothing(piva) Then
                strSql.AppendLine("   , Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            If Not IsNothing(validitaInizio) Then
                strSql.AppendLine("   , Validita_Inizio = " & Agro_SQL_SaveDate(validitaInizio) & " ")
            End If

            If Not IsNothing(validitaFine) Then
                strSql.AppendLine("   , Validita_Fine = " & Agro_SQL_SaveDate(validitaFine) & " ")
            End If



            strSql.AppendLine(" WHERE Id_Param = " & Agro_SQL_SaveNum(idParam) & " ")

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

    '##############################################################################################
    ''' <summary>
    ''' Scrive un record nella tabella Codici_Prodotti di GIAS INTERSCAMBIO
    ''' </summary>
    ''' <param name="Cod_Categoria_ALTRO">Codice categoria prodotto altro programma (opzionale = "")</param>
    ''' <param name="Cod_Prodotto_ALTRO">Codice prodotto altro programma (opzionale = "")</param>
    ''' <param name="Cod_Categoria_GIAS">Codice categoria GIAS (obbligatorio)</param>
    ''' <param name="Cod_Prodotto_GIAS">Codice prodotto GIAS (obbligatorio)</param>
    ''' <param name="Descr_Prodotto">Descrizione prodotto (opzionale = "")</param>
    ''' <param name="Piva"></param>
    ''' <param name="objParametri">AgronicaCoreParametri per la connessione</param>
    ''' <param name="suffissoColonnaAltro">suffisso sul nome della colonna per l'altro gestionale (ad es: "SAP" che verrà aggiunto a "Cod_Contatto_" (se non specificato = "ALTRO")</param>
    ''' <param name="Validita_Inizio"></param>
    ''' <param name="Validita_Fine"></param>
    ''' <param name="Data_creazione"></param>
    ''' <param name="Data_modifica"></param>
    ''' <param name="username_creazione"></param>
    ''' <param name="username_modifica"></param>
    ''' <returns>True se la scrittura è andata a buon fine, altrimenti False</returns>
    ''' <remarks>La PK è una colonna Identity: Id_Prod che viene scritta automaticamente</remarks>
    Public Function Scrivi_CodiciProdotti(ByVal Cod_Categoria_ALTRO As String,
                                          ByVal Cod_Prodotto_ALTRO As String,
                                          ByVal Cod_Categoria_GIAS As Integer,
                                          ByVal Cod_Prodotto_GIAS As Integer,
                                          ByVal Descr_Prodotto As String,
                                          ByVal Piva As String,
                                          ByRef objParametri As AgronicaCoreParametri,
                                          Optional ByVal suffissoColonnaAltro As String = "ALTRO",
                                          Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                                          Optional ByVal Validita_Fine As Date = AGRODATAFINE,
                                          Optional ByVal Data_creazione As Date = #2/1/1900#,
                                          Optional ByVal Data_modifica As Date = #2/1/1900#,
                                          Optional ByVal username_creazione As String = "",
                                          Optional ByVal username_modifica As String = ""
                                          ) As Boolean

        Const nomeRoutine = "Gias_Interscambio_W.Scrivi_CodiciProdotti()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_creazione = #2/1/1900# Then
                Data_creazione = Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If

            '---------------------------------------------
            stb.Length = 0
            stb.AppendLine(" INSERT INTO Codici_Prodotti ")

            stb.AppendLine("              (")
            stb.AppendLine("              [Cod_Categoria_" & suffissoColonnaAltro & "], ")
            stb.AppendLine("              [Cod_Prodotto_" & suffissoColonnaAltro & "], ")
            stb.AppendLine("              Cod_Categoria_GIAS,    Cod_Prodotto_GIAS, ")

            stb.AppendLine("              Descr_prodotto,     Piva, ")

            stb.AppendLine("              Data_Creazione,     Data_Modifica, ")
            stb.AppendLine("              Username_Creazione, Username_Modifica, ")
            stb.AppendLine("              Validita_Inizio,    Validita_Fine ")
            stb.AppendLine("              ) ")

            stb.AppendLine(" VALUES ( ")

            stb.AppendLine("          " & Agro_SQL_SaveText_NULL(Cod_Categoria_ALTRO) & "  ")
            stb.AppendLine("         , " & Agro_SQL_SaveText_NULL(Cod_Prodotto_ALTRO) & "  ")

            stb.AppendLine("         , " & Agro_SQL_SaveNum(Cod_Categoria_GIAS) & "  ")
            stb.AppendLine("         , " & Agro_SQL_SaveNum(Cod_Prodotto_GIAS) & "  ")

            stb.AppendLine("         , '" & Agro_SQL_SaveText(Descr_Prodotto) & "'  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(Piva) & "'  ")

            stb.AppendLine("		 , " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            stb.AppendLine("		 , " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            stb.AppendLine("		 ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            stb.AppendLine("		 ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            stb.AppendLine("		 , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            stb.AppendLine("		 , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            stb.AppendLine(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return xRisp

    End Function

    '##############################################################################################
    Public Function Modifica_CodiciProdotti(ByVal idProd As Integer,
                                            ByRef objParametri As AgronicaCoreParametri,
                                            Optional ByVal suffissoColonnaAltro As String = "ALTRO",
                                            Optional ByVal codCategoria_ALTRO As String = Nothing,
                                            Optional ByVal codProdotto_ALTRO As String = Nothing,
                                            Optional ByVal codCategoria_GIAS As Integer? = Nothing,
                                            Optional ByVal codProdotto_GIAS As Integer? = Nothing,
                                            Optional ByVal descrProdotto As String = Nothing,
                                            Optional ByVal piva As String = Nothing,
                                            Optional ByVal validitaInizio As Date? = Nothing,
                                            Optional ByVal validitaFine As Date? = Nothing,
                                            Optional ByVal dataModifica As DateTime = #2/1/1900#,
                                            Optional ByVal usernameModifica As String = ""
                                            ) As Boolean

        Const nomeRoutine = "Gias_Interscambio_W.Modifica_CodiciProdotti()"

        '====================================================================================
        'Parametri opzionali :
        '   Tutti i valori non chiave (se impostati a nothing o non passati 
        '   non ne verrà fatto l'aggiornamento e rimarranno i valori precedenti)
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            If dataModifica = #2/1/1900# Then
                dataModifica = Date.Now
            End If

            If usernameModifica = "" Then
                usernameModifica = objParametri.UsernameOperazione
            End If

            If idProd = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Prod obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE Codici_Prodotti ")
            strSql.AppendLine(" SET Data_Modifica = " & Agro_SQL_SaveDateTime(dataModifica) & " ")
            strSql.AppendLine("   , Username_Modifica = '" & Agro_SQL_SaveText(usernameModifica) & "' ")

            If Not IsNothing(codCategoria_ALTRO) Then
                strSql.AppendLine("   , [Cod_Categoria_" & suffissoColonnaAltro & "] = '" & Agro_SQL_SaveText(codCategoria_ALTRO) & "' ")
            End If

            If Not IsNothing(codProdotto_ALTRO) Then
                strSql.AppendLine("   , [Cod_Prodotto_" & suffissoColonnaAltro & "] = '" & Agro_SQL_SaveText(codProdotto_ALTRO) & "' ")
            End If

            If Not IsNothing(codCategoria_GIAS) Then
                strSql.AppendLine("   , Cod_Categoria_GIAS = " & Agro_SQL_SaveNum(codCategoria_GIAS) & " ")
            End If

            If Not IsNothing(codProdotto_GIAS) Then
                strSql.AppendLine("   , Cod_Prodotto_GIAS = " & Agro_SQL_SaveNum(codProdotto_GIAS) & " ")
            End If

            If Not IsNothing(descrProdotto) Then
                strSql.AppendLine("   , Descr_Prodotto = '" & Agro_SQL_SaveText(descrProdotto) & "' ")
            End If

            If Not IsNothing(piva) Then
                strSql.AppendLine("   , Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            If Not IsNothing(validitaInizio) Then
                strSql.AppendLine("   , Validita_Inizio = " & Agro_SQL_SaveDate(validitaInizio) & " ")
            End If

            If Not IsNothing(validitaFine) Then
                strSql.AppendLine("   , Validita_Fine = " & Agro_SQL_SaveDate(validitaFine) & " ")
            End If



            strSql.AppendLine(" WHERE Id_Prod = " & Agro_SQL_SaveNum(idProd) & " ")

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

    '##############################################################################################
    ''' <summary>
    ''' Scrive un record nella tabella Log_GIAS_*(ALTRO/SAP) di GIAS INTERSCAMBIO
    ''' </summary>
    ''' <param name="tipo">Tipologia del file</param>
    ''' <param name="nomeFile">Nome del file</param>
    ''' <param name="stato">Stato del file (A = Aperto, C = Completato, E = Errore): utilizzare le costanti INTERSCAMBIO_STATO_*</param>
    ''' <param name="sistemaOrigine">Sistema che ha esportato il file</param>
    ''' <param name="sistemaDestinazione">Sistema che ha importato il file</param>
    ''' <param name="objParametri">AgronicaCoreParametri per la connessione</param>
    ''' <param name="suffissoTabella">suffisso sul nome della tabella per l'altro gestionale (ad es: "SAP" che verrà aggiunto a "Log_GIAS_" (se non specificato = "ALTRO")</param>
    ''' <param name="dataCreazione"></param>
    ''' <param name="dataModifica"></param>
    ''' <param name="usernameCreazione"></param>
    ''' <param name="usernameModifica"></param>
    ''' <returns>True se la scrittura è andata a buon fine, altrimenti False</returns>
    ''' <remarks>La PK è una colonna Identity: Id_Log che viene scritta automaticamente</remarks>
    Public Function Scrivi_LogGiasAltro(ByVal tipo As String,
                                        ByVal nomeFile As String,
                                        ByVal stato As String,
                                        ByVal sistemaOrigine As String,
                                        ByVal sistemaDestinazione As String,
                                        ByRef objParametri As AgronicaCoreParametri,
                                        Optional ByVal suffissoTabella As String = "ALTRO",
                                        Optional ByVal dataCreazione As Date = #2/1/1900#,
                                        Optional ByVal dataModifica As Date = #2/1/1900#,
                                        Optional ByVal usernameCreazione As String = "",
                                        Optional ByVal usernameModifica As String = ""
                                        ) As Boolean

        Const nomeRoutine = "Gias_Interscambio_W.Scrivi_LogGiasAltro()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If dataCreazione = #2/1/1900# Then
                dataCreazione = Date.Now
            End If

            If dataModifica = #2/1/1900# Then
                dataModifica = Date.Now
            End If

            If usernameCreazione = "" Then
                usernameCreazione = objParametri.UsernameOperazione
            End If

            If usernameModifica = "" Then
                usernameModifica = objParametri.UsernameOperazione
            End If

            '---------------------------------------------
            stb.Length = 0
            stb.AppendLine(" INSERT INTO [Log_GIAS_" & suffissoTabella & "] ")

            stb.AppendLine("              (")
            stb.AppendLine("              Tipo,              Nome_File,             Stato, ")
            stb.AppendLine("              Sistema_Origine,   Sistema_Destinazione, ")

            stb.AppendLine("              Data_Creazione,     Data_Modifica, ")
            stb.AppendLine("              Username_Creazione, Username_Modifica ")
            stb.AppendLine("              ) ")

            stb.AppendLine(" VALUES ( ")

            stb.AppendLine("          '" & Agro_SQL_SaveText(tipo) & "'  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(nomeFile) & "'  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(stato) & "'  ")

            stb.AppendLine("         , '" & Agro_SQL_SaveText(sistemaOrigine) & "'  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(sistemaDestinazione) & "'  ")

            stb.AppendLine("		 , " & Agro_SQL_SaveDateTime(dataCreazione) & "  ")
            stb.AppendLine("		 , " & Agro_SQL_SaveDateTime(dataModifica) & "  ")
            stb.AppendLine("		 ,'" & Agro_SQL_SaveText(usernameCreazione) & "' ")
            stb.AppendLine("		 ,'" & Agro_SQL_SaveText(usernameModifica) & "' ")

            stb.AppendLine(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return xRisp

    End Function

    '##############################################################################################
    Public Function Modifica_LogGiasAltro(ByVal idLog As Integer,
                                          ByRef objParametri As AgronicaCoreParametri,
                                          Optional ByVal suffissoTabella As String = "ALTRO",
                                          Optional ByVal tipo As String = Nothing,
                                          Optional ByVal nomeFile As String = Nothing,
                                          Optional ByVal stato As String = Nothing,
                                          Optional ByVal sistemaOrigine As String = Nothing,
                                          Optional ByVal sistemaDestinazione As String = Nothing,
                                          Optional ByVal dataModifica As DateTime = #2/1/1900#,
                                          Optional ByVal usernameModifica As String = ""
                                          ) As Boolean

        Const nomeRoutine = "Gias_Interscambio_W.Modifica_LogGiasAltro()"

        '====================================================================================
        'Parametri opzionali :
        '   Tutti i valori non chiave (se impostati a nothing o non passati 
        '   non ne verrà fatto l'aggiornamento e rimarranno i valori precedenti)
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            If dataModifica = #2/1/1900# Then
                dataModifica = Date.Now
            End If

            If usernameModifica = "" Then
                usernameModifica = objParametri.UsernameOperazione
            End If

            If idLog = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Log obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE [Log_GIAS_" & suffissoTabella & "] ")
            strSql.AppendLine(" SET Data_Modifica = " & Agro_SQL_SaveDateTime(dataModifica) & " ")
            strSql.AppendLine("   , Username_Modifica = '" & Agro_SQL_SaveText(usernameModifica) & "' ")

            If Not IsNothing(tipo) Then
                strSql.AppendLine("   , Tipo = '" & Agro_SQL_SaveText(tipo) & "' ")
            End If

            If Not IsNothing(nomeFile) Then
                strSql.AppendLine("   , Nome_File = '" & Agro_SQL_SaveText(nomeFile) & "' ")
            End If

            If Not IsNothing(stato) Then
                strSql.AppendLine("   , Stato = '" & Agro_SQL_SaveText(stato) & "' ")
            End If

            If Not IsNothing(sistemaOrigine) Then
                strSql.AppendLine("   , Sistema_Origine = '" & Agro_SQL_SaveText(sistemaOrigine) & "' ")
            End If

            If Not IsNothing(sistemaDestinazione) Then
                strSql.AppendLine("   , Sistema_Destinazione = '" & Agro_SQL_SaveText(sistemaDestinazione) & "' ")
            End If

            strSql.AppendLine(" WHERE Id_Log = " & Agro_SQL_SaveNum(idLog) & " ")

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

    '##############################################################################################
    Public Function Scrivi_LogGiasSapItf(ByVal codInterfaccia As String,
                                         ByVal type As String,
                                         ByVal nomeFile As String,
                                         ByVal tipoLog As String,
                                         ByVal messaggio As String,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As Boolean

        Const nomeRoutine = "Gias_Interscambio_W.Scrivi_LogGiasSapItf()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            stb.Length = 0
            stb.AppendLine(" INSERT INTO [SAP_ITF_LOG] ")

            stb.AppendLine("              (")
            stb.AppendLine("              ITF_ID,       ITF_DESC,     PKG_NAME, ")
            stb.AppendLine("              SOURCE_NAME,  SUBPRG_TYPE,  SUBPRG_NAME, ")

            stb.AppendLine("              LOG_CRDAT,    LOG_CRTIM, ")
            stb.AppendLine("              MSG_TYPE,     MSG_NOTES, ")
            stb.AppendLine("              ITF_PAR01,    ITF_PAR02,    ITF_PAR03, ITF_PAR04, ITF_PAR05 ")
            stb.AppendLine("              ) ")

            stb.AppendLine(" VALUES ( ")

            stb.AppendLine("          '" & Agro_SQL_SaveText(codInterfaccia) & "'  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(type) & "'  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(nomeFile) & "'  ")

            stb.AppendLine("         , 'GIAS', 'M', NULL ")

            stb.AppendLine("         , CAST(CONVERT(varchar(10), GETDATE(), 112) AS int) ")
            stb.AppendLine("         , CAST(REPLACE(CONVERT(varchar(8), GETDATE(), 108), ':', '') AS int) ")

            stb.AppendLine("         , '" & Agro_SQL_SaveText(tipoLog) & "'  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(messaggio) & "'  ")

            stb.AppendLine("		 , '" & Agro_SQL_SaveText(nomeFile) & "' ")
            stb.AppendLine("		 , NULL, NULL, NULL, NULL ")

            stb.AppendLine(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return xRisp

    End Function



    '##############################################################################################
    Public Function Scrivi_LogDettaglioImport(ByVal NomeFile As String,
                                              ByVal TipoOperazione As String,
                                              ByVal KeyAgenda As String,
                                              ByVal KeyDettaglio As String,
                                              ByVal KeyExport As String,
                                              ByVal TipoRisorsa As String,
                                              ByVal Descrizione As String,
                                              ByVal Cod_ContattoGias As String,
                                              ByVal Cod_ContattoImport As String,
                                              ByVal Cod_Risum As Integer,
                                              ByVal Rag_Soc As String,
                                              ByVal Elem_Cod As Integer,
                                              ByVal Pro_Cod As Integer,
                                              ByVal Mat_Cod As Integer,
                                              ByVal Lotto As String,
                                              ByVal UdmGias As Integer,
                                              ByVal UdmImport As String,
                                              ByVal Qta As Decimal,
                                              ByVal Note As String,
                                              ByVal Riferimento_Mov As String,
                                              ByVal Tipo_Xml As String,
                                              ByRef objParametri As AgronicaCoreParametri
                                              ) As Boolean

        Const nomeRoutine = "Gias_Interscambio_W.Scrivi_LogDettaglioImport()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            stb.Length = 0
            stb.AppendLine(" INSERT INTO [GIAS_Dettaglio_Import_Log] ")

            stb.AppendLine("              (")
            stb.AppendLine("              NomeFile, TipoOperazione, KeyAgenda, KeyDettaglio, KeyExport, TipoRisorsa,  Cod_ContattoGias, ")
            stb.AppendLine("              Cod_ContattoImport, Cod_Risum,  Rag_Soc, ")
            stb.AppendLine("              Elem_Cod,  Pro_Cod,  Mat_Cod,  Lotto, ")
            stb.AppendLine("              Descrizione,     UdmGias,   UdmImport,  ")
            stb.AppendLine("              Qta,  Data_Log,  Note,  Riferimento_Mov, Tipo_Xml ")
            stb.AppendLine("              ) ")

            stb.AppendLine(" VALUES ( ")

            stb.AppendLine("           '" & Agro_SQL_SaveText(NomeFile) & "'  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(TipoOperazione) & "'  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(KeyAgenda) & "'  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(KeyDettaglio) & "'  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(KeyExport) & "'  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(TipoRisorsa) & "'  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(Cod_ContattoGias) & "'  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(Cod_ContattoImport) & "'  ")
            stb.AppendLine("         , " & Agro_SQL_SaveNum(Cod_Risum) & "  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(Rag_Soc) & "'  ")
            stb.AppendLine("         , " & Agro_SQL_SaveNum(Elem_Cod) & "  ")
            stb.AppendLine("         , " & Agro_SQL_SaveNum(Pro_Cod) & "  ")
            stb.AppendLine("         , " & Agro_SQL_SaveNum(Mat_Cod) & "  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(Lotto) & "'  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(Descrizione) & "'  ")
            stb.AppendLine("         , " & Agro_SQL_SaveNum(UdmGias) & "  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(UdmImport) & "'  ")
            stb.AppendLine("         , " & Agro_SQL_SaveNum(Qta) & "  ")
            stb.AppendLine("		 , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(Note) & "'  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(Riferimento_Mov) & "'  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(Tipo_Xml) & "'  ")


            stb.AppendLine(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return xRisp

    End Function


    '##############################################################################################
    Public Function Scrivi_LogDettaglioExport(ByVal NomeFile As String,
                                              ByVal TipoOperazione As String,
                                              ByVal KeyAgenda As String,
                                              ByVal KeyDettaglio As String,
                                              ByVal KeyExport As String,
                                              ByVal TipoRisorsa As String,
                                              ByVal Descrizione As String,
                                              ByVal Cod_Contatto As String,
                                              ByVal Cod_Risum As Integer,
                                              ByVal Mac_Cod As Integer,
                                              ByVal Elem_Cod As Integer,
                                              ByVal Pro_Cod As Integer,
                                              ByVal Mat_Cod As Integer,
                                              ByVal Lotto As String,
                                              ByVal UdmGias As Integer,
                                              ByVal UdmExport As String,
                                              ByVal Qta As Decimal,
                                              ByVal Note As String,
                                              ByRef objParametri As AgronicaCoreParametri
                                              ) As Boolean

        Const nomeRoutine = "Gias_Interscambio_W.Scrivi_LogDettaglioExport()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            stb.Length = 0
            stb.AppendLine(" INSERT INTO [GIAS_Dettaglio_Export_Log] ")

            stb.AppendLine("              (")
            stb.AppendLine("              NomeFile, TipoOperazione, KeyAgenda, KeyDettaglio, KeyExport, TipoRisorsa,  Cod_Contatto,  Cod_Risum,  Mac_Cod, ")
            stb.AppendLine("              Elem_Cod,  Pro_Cod,  Mat_Cod,  Lotto, ")
            stb.AppendLine("              Descrizione,     UdmGias,  UdmExport, ")
            stb.AppendLine("              Qta,  Data_Log,  Note ")
            stb.AppendLine("              ) ")

            stb.AppendLine(" VALUES ( ")

            stb.AppendLine("           '" & Agro_SQL_SaveText(NomeFile) & "'  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(TipoOperazione) & "'  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(KeyAgenda) & "'  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(KeyDettaglio) & "'  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(KeyExport) & "'  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(TipoRisorsa) & "'  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(Cod_Contatto) & "'  ")
            stb.AppendLine("         , " & Agro_SQL_SaveNum(Cod_Risum) & "  ")
            stb.AppendLine("         , " & Agro_SQL_SaveNum(Mac_Cod) & "  ")
            stb.AppendLine("         , " & Agro_SQL_SaveNum(Elem_Cod) & "  ")
            stb.AppendLine("         , " & Agro_SQL_SaveNum(Pro_Cod) & "  ")
            stb.AppendLine("         , " & Agro_SQL_SaveNum(Mat_Cod) & "  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(Lotto) & "'  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(Descrizione) & "'  ")
            stb.AppendLine("         , " & Agro_SQL_SaveNum(UdmGias) & "  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(UdmExport) & "'  ")
            stb.AppendLine("         , " & Agro_SQL_SaveNum(Qta) & "  ")
            stb.AppendLine("		 , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(Note) & "'  ")


            stb.AppendLine(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return xRisp

    End Function



#End Region

#Region "Codici Parametri Qualitativi"

    '##############################################################################################
    ''' <summary>
    ''' Scrive un record nella tabella Codici_Parametri_Qualitativi di GIAS INTERSCAMBIO
    ''' </summary>
    ''' <param name="Tipo_ALTRO">Tipo del parametro in un altro programma (opzionale = ""))</param>
    ''' <param name="Codice_ALTRO">Codice del parametro in un altro programma (opzionale = "NULL")</param>
    ''' <param name="Tipo_GIAS">Tipo del parametro in GIAS</param>
    ''' <param name="Codice_GIAS">Codice del parametro in GIAS</param>
    ''' <param name="Descr_Param">Descrizione del parametro</param>
    ''' <param name="objParametri">AgronicaCoreParametri per la connessione</param>
    ''' <param name="suffissoColonnaAltro">suffisso sul nome della colonna per l'altro gestionale (ad es: "SAP" che verrà aggiunto a "Codice_" (se non specificato = "ALTRO")</param>
    ''' <param name="Validita_Inizio"></param>
    ''' <param name="Validita_Fine"></param>
    ''' <param name="Data_creazione"></param>
    ''' <param name="Data_modifica"></param>
    ''' <param name="username_creazione"></param>
    ''' <param name="username_modifica"></param>
    ''' <returns>True se la scrittura è andata a buon fine, altrimenti False</returns>
    ''' <remarks>La PK è una colonna Identity: Id_Param che viene scritta automaticamente</remarks>
    Public Function Scrivi_CodiciParametriQualitativi(ByVal Tipo_ALTRO As String,
                                                      ByVal Codice_ALTRO As String,
                                                      ByVal Tipo_GIAS As String,
                                                      ByVal Codice_GIAS As Integer,
                                                      ByVal Descr_Param As String,
                                                      ByVal Piva As String,
                                                      ByRef objParametri As AgronicaCoreParametri,
                                                      Optional ByVal suffissoColonnaAltro As String = "ALTRO",
                                                      Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                                                      Optional ByVal Validita_Fine As Date = AGRODATAFINE,
                                                      Optional ByVal Data_creazione As Date = #2/1/1900#,
                                                      Optional ByVal Data_modifica As Date = #2/1/1900#,
                                                      Optional ByVal username_creazione As String = "",
                                                      Optional ByVal username_modifica As String = ""
                                                      ) As Boolean

        Const nomeRoutine = "Gias_Interscambio_W.Scrivi_CodiciParametriQualitativi()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_creazione = #2/1/1900# Then
                Data_creazione = Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If


            '---------------------------------------------
            stb.Length = 0
            stb.AppendLine(" INSERT INTO Codici_Parametri_Qualitativi ")

            stb.AppendLine("              (")
            stb.AppendLine("              [Tipo_" & suffissoColonnaAltro & "], ")
            stb.AppendLine("              [Codice_" & suffissoColonnaAltro & "], ")
            stb.AppendLine("              Tipo_GIAS,          Codice_GIAS, ")

            stb.AppendLine("              Descr_Param,        Piva, ")

            stb.AppendLine("              Data_Creazione,     Data_Modifica, ")
            stb.AppendLine("              Username_Creazione, Username_Modifica, ")
            stb.AppendLine("              Validita_Inizio,    Validita_Fine ")
            stb.AppendLine("              ) ")

            stb.AppendLine(" VALUES ( ")

            stb.AppendLine("           " & Agro_SQL_SaveText_NULL(Tipo_ALTRO) & "  ")
            stb.AppendLine("         , " & Agro_SQL_SaveText_NULL(Codice_ALTRO) & "  ")
            stb.AppendLine("         , " & Agro_SQL_SaveText_NULL(Tipo_GIAS) & "  ")
            stb.AppendLine("         , " & Agro_SQL_SaveNum(Codice_GIAS) & "  ")

            stb.AppendLine("         , '" & Agro_SQL_SaveText(Descr_Param) & "'  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(Piva) & "'  ")

            stb.AppendLine("		 , " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            stb.AppendLine("		 , " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            stb.AppendLine("		 ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            stb.AppendLine("		 ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            stb.AppendLine("		 , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            stb.AppendLine("		 , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            stb.AppendLine(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return xRisp

    End Function

    '##############################################################################################
    Public Function Modifica_CodiciParametriQualitativi(ByVal idParam As Integer,
                                                        ByRef objParametri As AgronicaCoreParametri,
                                                        Optional ByVal suffissoColonnaAltro As String = "ALTRO",
                                                        Optional ByVal Tipo_ALTRO As String = Nothing,
                                                        Optional ByVal Codice_ALTRO As String = Nothing,
                                                        Optional ByVal Tipo_GIAS As String = Nothing,
                                                        Optional ByVal Codice_GIAS As Integer? = Nothing,
                                                        Optional ByVal descrParam As String = Nothing,
                                                        Optional ByVal piva As String = Nothing,
                                                        Optional ByVal validitaInizio As Date? = Nothing,
                                                        Optional ByVal validitaFine As Date? = Nothing,
                                                        Optional ByVal dataModifica As DateTime = #2/1/1900#,
                                                        Optional ByVal usernameModifica As String = ""
                                                        ) As Boolean

        Const nomeRoutine = "Gias_Interscambio_W.Modifica_CodiciParametriQualitativi()"

        '====================================================================================
        'Parametri opzionali :
        '   Tutti i valori non chiave (se impostati a nothing o non passati 
        '   non ne verrà fatto l'aggiornamento e rimarranno i valori precedenti)
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            If dataModifica = #2/1/1900# Then
                dataModifica = Now
            End If

            If usernameModifica = "" Then
                usernameModifica = objParametri.UsernameOperazione
            End If

            If idParam = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Param obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE Codici_Parametri_Qualitativi ")
            strSql.AppendLine(" SET Data_Modifica = " & Agro_SQL_SaveDateTime(dataModifica) & " ")
            strSql.AppendLine("   , Username_Modifica = '" & Agro_SQL_SaveText(usernameModifica) & "' ")

            If Not IsNothing(Tipo_ALTRO) Then
                strSql.AppendLine("   , [Tipo_" & suffissoColonnaAltro & "] = '" & Agro_SQL_SaveText(Tipo_ALTRO) & "' ")
            End If

            If Not IsNothing(Codice_ALTRO) Then
                strSql.AppendLine("   , [Codice_" & suffissoColonnaAltro & "] = '" & Agro_SQL_SaveText(Codice_ALTRO) & "' ")
            End If

            If Not IsNothing(Tipo_GIAS) Then
                strSql.AppendLine("   , Tipo_GIAS = '" & Agro_SQL_SaveText(Tipo_GIAS) & "' ")
            End If

            If Not IsNothing(Codice_GIAS) Then
                strSql.AppendLine("   , Codice_GIAS = " & Agro_SQL_SaveNum(Codice_GIAS) & " ")
            End If

            If Not IsNothing(descrParam) Then
                strSql.AppendLine("   , Descr_Param = '" & Agro_SQL_SaveText(descrParam) & "' ")
            End If

            If Not IsNothing(piva) Then
                strSql.AppendLine("   , Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            If Not IsNothing(validitaInizio) Then
                strSql.AppendLine("   , Validita_Inizio = " & Agro_SQL_SaveDate(validitaInizio) & " ")
            End If

            If Not IsNothing(validitaFine) Then
                strSql.AppendLine("   , Validita_Fine = " & Agro_SQL_SaveDate(validitaFine) & " ")
            End If



            strSql.AppendLine(" WHERE Id_Param = " & Agro_SQL_SaveNum(idParam) & " ")

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

#End Region

#Region "Imprese Gestite"

    '##############################################################################################
    ''' <summary>
    ''' Scrive un record nella tabella Imprese_Gestite di GIAS INTERSCAMBIO
    ''' </summary>
    ''' <param name="Piva">Piva azienda gestita</param>
    ''' <param name="objParametri">AgronicaCoreParametri per la connessione</param>
    ''' <param name="Validita_Inizio"></param>
    ''' <param name="Validita_Fine"></param>
    ''' <param name="Data_creazione"></param>
    ''' <param name="Data_modifica"></param>
    ''' <param name="username_creazione"></param>
    ''' <param name="username_modifica"></param>
    ''' <returns>True se la scrittura è andata a buon fine, altrimenti False</returns>
    Public Function Scrivi_ImpreseGestite(ByVal Piva As String,
                                           ByRef objParametri As AgronicaCoreParametri,
                                           Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                                           Optional ByVal Validita_Fine As Date = AGRODATAFINE,
                                           Optional ByVal Data_creazione As Date = #2/1/1900#,
                                           Optional ByVal Data_modifica As Date = #2/1/1900#,
                                           Optional ByVal username_creazione As String = "",
                                           Optional ByVal username_modifica As String = ""
                                           ) As Boolean

        Const nomeRoutine = "Gias_Interscambio_W.Scrivi_ImpreseGestite()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_creazione = #2/1/1900# Then
                Data_creazione = Date.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If


            '---------------------------------------------
            stb.Length = 0
            stb.AppendLine(" INSERT INTO Imprese_Gestite ")

            stb.AppendLine("              (")
            stb.AppendLine("              Piva, ")

            stb.AppendLine("              Data_Creazione,     Data_Modifica, ")
            stb.AppendLine("              Username_Creazione, Username_Modifica, ")
            stb.AppendLine("              Validita_Inizio,    Validita_Fine ")
            stb.AppendLine("              ) ")

            stb.AppendLine(" VALUES ( ")

            stb.AppendLine("         '" & Agro_SQL_SaveText(Piva) & "'  ")

            stb.AppendLine("		 , " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            stb.AppendLine("		 , " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            stb.AppendLine("		 ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            stb.AppendLine("		 ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            stb.AppendLine("		 , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            stb.AppendLine("		 , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            stb.AppendLine(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return xRisp

    End Function

#End Region

#Region "Doc Processati"

    '##############################################################################################
    ''' <summary>
    ''' Scrive un record nella tabella DocProcessati di GIAS INTERSCAMBIO
    ''' </summary>
    ''' <param name="piva">Piva azienda gestita</param>
    ''' <param name="objParametri">AgronicaCoreParametri per la connessione</param>
    ''' <param name="Validita_Inizio"></param>
    ''' <param name="Validita_Fine"></param>
    ''' <param name="Data_creazione"></param>
    ''' <param name="Data_modifica"></param>
    ''' <param name="username_creazione"></param>
    ''' <param name="username_modifica"></param>
    ''' <returns>True se la scrittura è andata a buon fine, altrimenti False</returns>
    Public Function Scrivi_DocProcessati(ByVal piva As String,
                                         ByVal idAgenda As Integer,
                                         ByVal idDocAltro As String,
                                         ByVal intestatarioAltro As String,
                                         ByVal numDocAltro As String,
                                         ByVal dataDocAltro As Date,
                                         ByRef objParametri As AgronicaCoreParametri,
                                         Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                                         Optional ByVal Validita_Fine As Date = AGRODATAFINE,
                                         Optional ByVal Data_creazione As Date = #2/1/1900#,
                                         Optional ByVal Data_modifica As Date = #2/1/1900#,
                                         Optional ByVal username_creazione As String = "",
                                         Optional ByVal username_modifica As String = ""
                                         ) As Boolean

        Const nomeRoutine = "Gias_Interscambio_W.Scrivi_DocProcessati()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_creazione = #2/1/1900# Then
                Data_creazione = Date.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If


            '---------------------------------------------
            stb.Length = 0
            stb.AppendLine(" INSERT INTO Doc_Processati ")

            stb.AppendLine("              (")
            stb.AppendLine("              Piva, Id_Agenda, ")
            stb.AppendLine("              Id_Doc_Altro, Intestatario_Altro, NumDoc_Altro, DataDoc_Altro, ")

            stb.AppendLine("              Data_Creazione,     Data_Modifica, ")
            stb.AppendLine("              Username_Creazione, Username_Modifica, ")
            stb.AppendLine("              Validita_Inizio,    Validita_Fine ")
            stb.AppendLine("              ) ")

            stb.AppendLine(" VALUES ( ")

            stb.AppendLine("          '" & Agro_SQL_SaveText(piva) & "'  ")
            stb.AppendLine("         ," & Agro_SQL_SaveNum(idAgenda) & "  ")

            stb.AppendLine("         ,'" & Agro_SQL_SaveText(idDocAltro) & "'  ")
            stb.AppendLine("         ,'" & Agro_SQL_SaveText(intestatarioAltro) & "'  ")
            stb.AppendLine("         ,'" & Agro_SQL_SaveText(numDocAltro) & "'  ")
            stb.AppendLine("         ," & Agro_SQL_SaveDate(dataDocAltro) & "  ")

            stb.AppendLine("		 , " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            stb.AppendLine("		 , " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            stb.AppendLine("		 ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            stb.AppendLine("		 ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            stb.AppendLine("		 , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            stb.AppendLine("		 , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            stb.AppendLine(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    '##############################################################################################
    ''' <summary>
    ''' Modifica un record nella tabella DocProcessati di GIAS INTERSCAMBIO
    ''' </summary>
    ''' <param name="id"></param>
    ''' <param name="piva"></param>
    ''' <param name="idAgenda"></param>
    ''' <param name="idDocAltro"></param>
    ''' <param name="intestatarioAltro"></param>
    ''' <param name="numDocAltro"></param>
    ''' <param name="dataDocAltro"></param>
    ''' <param name="objParametri"></param>
    ''' <param name="Validita_Inizio"></param>
    ''' <param name="Validita_Fine"></param>
    ''' <param name="Data_modifica"></param>
    ''' <param name="username_modifica"></param>
    ''' <returns></returns>
    Public Function Modifica_DocProcessati(ByVal id As Integer,
                                           ByVal piva As String,
                                           ByVal idAgenda As Integer,
                                           ByVal idDocAltro As String,
                                           ByVal intestatarioAltro As String,
                                           ByVal numDocAltro As String,
                                           ByVal dataDocAltro As Date?,
                                           ByRef objParametri As AgronicaCoreParametri,
                                           Optional ByVal Validita_Inizio As Date? = Nothing,
                                           Optional ByVal Validita_Fine As Date? = Nothing,
                                           Optional ByVal Data_modifica As Date? = Nothing,
                                           Optional ByVal username_modifica As String = ""
                                           ) As Boolean

        Const nomeRoutine = "Gias_Interscambio_W.Modifica_DocProcessati()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_modifica Is Nothing Then
                Data_modifica = Date.Now
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If

            '---------------------------------------------
            stb.Length = 0
            stb.AppendLine(" UPDATE Doc_Processati SET ")
            stb.AppendLine(" Data_Modifica = " & Agro_SQL_SaveDateTime(Data_modifica) & " ")
            stb.AppendLine(" , Username_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "' ")

            If idDocAltro IsNot Nothing Then
                stb.AppendLine(" , Id_Doc_Altro = '" & Agro_SQL_SaveText(idDocAltro) & "' ")
            End If

            If intestatarioAltro IsNot Nothing Then
                stb.AppendLine(" , Intestatario_Altro = '" & Agro_SQL_SaveText(intestatarioAltro) & "' ")
            End If

            If numDocAltro IsNot Nothing Then
                stb.AppendLine(" , NumDoc_Altro = '" & Agro_SQL_SaveText(numDocAltro) & "' ")
            End If

            If dataDocAltro IsNot Nothing Then
                stb.AppendLine(" , DataDoc_Altro = " & Agro_SQL_SaveDateTime(dataDocAltro) & " ")
            End If

            If Validita_Inizio IsNot Nothing Then
                stb.AppendLine(" , Validita_Inizio = " & Agro_SQL_SaveDateTime(Validita_Inizio) & " ")
            End If

            If Validita_Fine IsNot Nothing Then
                stb.AppendLine(" , Validita_Fine = " & Agro_SQL_SaveDateTime(Validita_Fine) & " ")
            End If

            stb.AppendLine(" WHERE 1 = 1 ")

            If id <> 0 Then
                stb.AppendLine(" AND Id = " & Agro_SQL_SaveNum(id) & " ")
            End If

            If piva IsNot Nothing Then
                stb.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            If idAgenda <> 0 Then
                stb.AppendLine(" AND Id_Agenda = " & Agro_SQL_SaveNum(idAgenda) & " ")
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    '##############################################################################################
    ''' <summary>
    ''' Cancella un record nella tabella DocProcessati di GIAS INTERSCAMBIO
    ''' </summary>
    ''' <param name="piva">Piva azienda gestita</param>
    ''' <param name="objParametri">AgronicaCoreParametri per la connessione</param>
    ''' <param name="Validita_Inizio"></param>
    ''' <param name="Validita_Fine"></param>
    ''' <param name="Data_creazione"></param>
    ''' <param name="Data_modifica"></param>
    ''' <param name="username_creazione"></param>
    ''' <param name="username_modifica"></param>
    ''' <returns>True se la scrittura è andata a buon fine, altrimenti False</returns>
    Public Function Cancella_DocProcessati(ByVal id As Integer,
                                           ByVal piva As String,
                                           ByVal idAgenda As Integer,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByRef objParametri As AgronicaCoreParametri
                                           ) As Boolean

        Const nomeRoutine = "Gias_Interscambio_W.Cancella_DocProcessati()"

        '====================================================================================
        'Parametri opzionali :
        '   id = 0
        '   idAgenda = 0
        '====================================================================================


        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If id = 0 AndAlso idAgenda = 0 AndAlso xFiltroAggiuntivo = "" Then
                Throw New Exception("Parametro non corretto nella query (Indicare Id, Id_Agenda o Filtro Aggiuntivo)")
            End If


            stb.Length = 0
            stb.AppendLine(" DELETE ")
            stb.AppendLine(" FROM Doc_Processati ")
            stb.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(piva) & "' ")

            If id <> 0 Then
                stb.AppendLine(" AND Id = " & Agro_SQL_SaveNum(id) & "   ")
            End If

            If idAgenda <> 0 Then
                stb.AppendLine(" AND Id_Agenda = " & Agro_SQL_SaveNum(idAgenda) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function


#End Region

#Region "Agenzie importate Hash"
    Public Function Scrivi_AgenziaImportata(
                                           ByVal codiceMagazzinoAgenzia As String,
                                           ByVal hash As String,
                                           ByRef objParametri As AgronicaCoreParametri,
                                           Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                                           Optional ByVal Validita_Fine As Date = AGRODATAFINE
    ) As Boolean
        Const nomeRoutine = "Gias_Interscambio_W.Scrivi_AgenziaImportata()"
        Dim stb As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            stb.Length = 0
            stb.AppendLine(" INSERT INTO Agenzie_Importate_Hash ( ")
            stb.AppendLine("    CodiceMagazzinoAgenzia, ")
            stb.AppendLine("    Hash,")
            stb.AppendLine("    Validita_Inizio, Validita_Fine, ")
            stb.AppendLine("    Data_Creazione, Data_Modifica")
            stb.AppendLine(" ) ")

            stb.AppendLine(" VALUES ( ")

            stb.AppendLine($" '{Agro_SQL_SaveText(codiceMagazzinoAgenzia)}', ")
            stb.AppendLine($" '{Agro_SQL_SaveText(hash)}', ")
            stb.AppendLine($" {Agro_SQL_SaveDate(Validita_Inizio)}, ")
            stb.AppendLine($" {Agro_SQL_SaveDate(Validita_Fine)}, ")
            stb.AppendLine($" {Agro_SQL_SaveDate(DateTime.Now)}, ")
            stb.AppendLine($" {Agro_SQL_SaveDate(DateTime.Now)} ")

            stb.AppendLine(" ) ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            Dim messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception($"[{nomeRoutine}] : {messaggioErrore}")

        End Try

        Return xRisp

    End Function

    Public Function Aggiorna_AgenziaImportata(
                                           ByVal codiceMagazzinoAgenzia As String,
                                           ByVal hash As String,
                                           ByRef objParametri As AgronicaCoreParametri,
                                           Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                                           Optional ByVal Validita_Fine As Date = AGRODATAFINE
    ) As Boolean
        Const nomeRoutine = "Gias_Interscambio_W.Aggiorna_AgenziaImportata()"
        Dim stb As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            stb.Length = 0
            stb.AppendLine(" UPDATE Agenzie_Importate_Hash SET ")
            stb.AppendLine($" Hash = '{Agro_SQL_SaveText(hash)}', ")
            stb.AppendLine($" Validita_Inizio = {Agro_SQL_SaveDate(Validita_Inizio)}, ")
            stb.AppendLine($" Validita_Fine = {Agro_SQL_SaveDate(Validita_Fine)}, ")
            stb.AppendLine($" Data_Modifica = {Agro_SQL_SaveDateTime(DateTime.Now)} ")
            stb.AppendLine($" WHERE CodiceMagazzinoAgenzia = '{Agro_SQL_SaveText(codiceMagazzinoAgenzia)}'")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            Dim messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception($"[{nomeRoutine}] : {messaggioErrore}")

        End Try

        Return xRisp

    End Function
#End Region

#Region "Imprese importate Hash"
    Public Function Scrivi_AziendaImportata(
                                           ByVal piva As String,
                                           ByVal hash As String,
                                           ByRef objParametri As AgronicaCoreParametri,
                                           Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                                           Optional ByVal Validita_Fine As Date = AGRODATAFINE
    ) As Boolean
        Const nomeRoutine = "Gias_Interscambio_W.Scrivi_AziendaImportata()"
        Dim stb As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            stb.Length = 0
            stb.AppendLine(" INSERT INTO Aziende_Importate_Hash ( ")
            stb.AppendLine("    Piva, ")
            stb.AppendLine("    Hash,")
            stb.AppendLine("    Validita_Inizio, Validita_Fine, ")
            stb.AppendLine("    Data_Creazione, Data_Modifica ")
            stb.AppendLine(" ) ")

            stb.AppendLine(" VALUES ( ")

            stb.AppendLine($" '{Agro_SQL_SaveText(piva)}', ")
            stb.AppendLine($" '{Agro_SQL_SaveText(hash)}', ")
            stb.AppendLine($" {Agro_SQL_SaveDate(Validita_Inizio)}, ")
            stb.AppendLine($" {Agro_SQL_SaveDate(Validita_Fine)}, ")
            stb.AppendLine($" {Agro_SQL_SaveDateTime(DateTime.Now)}, ")
            stb.AppendLine($" {Agro_SQL_SaveDateTime(DateTime.Now)} ")

            stb.AppendLine(" ) ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            Dim messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception($"[{nomeRoutine}] : {messaggioErrore}")

        End Try

        Return xRisp

    End Function

    Public Function Aggiorna_AziendaImportata(
                                       ByVal piva As String,
                                       ByVal hash As String,
                                       ByRef objParametri As AgronicaCoreParametri,
                                       Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                                       Optional ByVal Validita_Fine As Date = AGRODATAFINE
) As Boolean
        Const nomeRoutine = "Gias_Interscambio_W.Aggiorna_AziendaImportata()"
        Dim stb As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            stb.Length = 0
            stb.AppendLine(" UPDATE Aziende_Importate_Hash SET ")
            stb.AppendLine($" Hash = '{Agro_SQL_SaveText(hash)}', ")
            stb.AppendLine($" Validita_Inizio = {Agro_SQL_SaveDate(Validita_Inizio)}, ")
            stb.AppendLine($" Validita_Fine = {Agro_SQL_SaveDate(Validita_Fine)}, ")
            stb.AppendLine($" Data_Modifica = {Agro_SQL_SaveDateTime(DateTime.Now)} ")
            stb.AppendLine($" WHERE Piva = '{Agro_SQL_SaveText(piva)}'")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            Dim messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception($"[{nomeRoutine}] : {messaggioErrore}")

        End Try

        Return xRisp

    End Function
#End Region
End Class
