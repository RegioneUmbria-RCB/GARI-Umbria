Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreModelsSTD
Imports AgronicaCoreMetaSchemaDAL
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports System.Linq
Imports AgronicaCoreDTOStd.InData.AgronicaCoreUtentiBIZ
Imports AgronicaCoreDTOStd.InData

Public Class Imprese_Impostazioni_R

    Private Class Impostazione_DB
        Public Piva_SuperUser As String = ""
        Public Piva As String = ""
        Public Sa_Cod As Integer
        Public Impostazione_Cod As Integer
        Public Impostazione_Valore As String = ""
    End Class

    Private Class ImpostazioneBase
        Public descrizione As String = ""
        Public codice As String = ""
        Public value As String = ""
        Public Tipo_campo As String = ""
        Public Note As String = ""
    End Class

    ''' <summary>
    ''' Legge il valore di un'impostazione di impresa o centro aziendale
    ''' </summary>
    ''' <returns></returns>
    Public Function LeggiImpostazioniImpresa(
        impresa As ImpresaDto, impostazioni As IEnumerable(Of Integer),
        objPUtenti As AgronicaCoreParametri, objPServer As AgronicaCoreParametri
    ) As List(Of baseClass.BaseCodeDescr)
        Dim objImpostazioni As New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_R
        Dim valueList As New List(Of baseClass.BaseCodeDescr)
        Dim e = impostazioni.GetEnumerator()
        While e.MoveNext
            Dim value = objImpostazioni.LeggiScalareMulticentroAziendaSuperUser(
                impresa.piva, {impresa.Sa_Cod}.ToList,
                e.Current, "",
                objPUtenti, objPServer
            )
            valueList.Add(New baseClass.BaseCodeDescr(e.Current, value))
        End While
        Return valueList
    End Function

    ''' <summary>
    ''' Legge unicamente i valori personalizzati per le impostazioni imprese/centri
    ''' </summary>
    ''' <returns></returns>
    Public Function LeggiSoloImpostate(
        imprese As IEnumerable(Of ImpresaDto), impostazioni As IEnumerable(Of Integer),
        objPServer As AgronicaCoreParametri
    ) As DataTable
        Dim objImpostazioni As New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_R
        Dim dt = objImpostazioni.Leggi(
                piva:=String.Empty, SACOD_NOFILTRO, 0,
                selezioneVariabile:=enumSelezioneVariabile.Selezione_JoinDescrizioni,
                xFiltroAggiuntivo:=String.Empty, xOrderBy:=String.Empty, objPServer
            )
        If dt.Rows.Count > 0 Then
            Dim dr = dt.Select
            If impostazioni.Any Then
                dr = dr.Where(Function(row) impostazioni.Contains(row("Impostazione_Cod")))
            End If
            If imprese.Any Then
                Dim keys = imprese.Select(Function(i) i.piva & "_" & i.Sa_Cod)
                dr = dr.Where(Function(row) keys.Contains(row("Piva") & "_" & row("Sa_Cod")))
            End If
            Return If(dr.Any, dr.CopyToDataTable, dt)
        End If
        Return dt
    End Function

    ''' <summary>
    ''' Funzione utilizzata per leggere le guide delle impostazioni per imprese
    ''' e centri in modo da poterle collocare nelle corrette sezioni.
    ''' </summary>
    ''' <returns><tt>IEnumerable</tt> di guide impostazioni per aziende e centri </returns>
    Public Function CaricaSezioni_Impostazioni_Aziende_Centri(obj_Utenti As AgronicaCoreParametri) As IEnumerable(Of Object)
        Dim objImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

        Dim xFiltroAggiuntivo As String = " Impostazione_Azienda = 1 OR Impostazione_Azienda_Centro = 1 "
        Dim xOrderBy As String = " Ordine, Impostazione_Cod "

        Dim DT_Impostazioni As DataTable = objImpostazioni.LeggiSezioniImpostazioni(xFiltroAggiuntivo, xOrderBy, obj_Utenti)
        Dim settingsList = (From row In DT_Impostazioni.Rows
                            Select (New With {
                                .Sezione_Des = row.item("SezioneDes"),
                                .Sezione_Cod = row.item("SezioneCod"),
                                .SottoSezione_Des = row.item("SottoSezioneDes"),
                                .SottoSezione_Cod = row.item("SottoSezioneCod"),
                                .SottoSezione_Espandibile = row.item("SottoSezioneEspandibile"),
                                .Livello_Des = row.item("LivelloDes"),
                                .Livello_Cod = row.item("LivelloCod"),
                                .Impostazione_Cod = row.item("Impostazione_Cod"),
                                .Impostazione_Des = row.item("Label"),
                                .Tipo_Campo = row.item("Tipo_Campo"),
                                .Impostazione_Utente = row.item("Impostazione_Utente"),
                                .Impostazione_SuperUser = row.item("Impostazione_SuperUser"),
                                .Impostazione_AziendaCentro = row.item("Impostazione_Azienda_Centro"),
                                .Impostazione_Azienda = row.item("Impostazione_Azienda"),
                                .Flag_InApp = row.item("Flag_InApp"),
                                .Ordine = row.item("Ordine"),
                                .Note = row.item("Note"),
                                .Valore_Default = row.item("Valore_Default")
                            })).ToList()

        Return settingsList
    End Function

    Public Function CaricaImpostazioni_Aziende_Centri(impCod As Integer, obj_Utenti As AgronicaCoreParametri)

        Dim objImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        'Dim xFiltroAggiuntivo As String = " Impostazione_Azienda_Centro = 1 "
        Dim getStringOrEmpty = Function(row As DataRow, field As String) If(IsDBNull(row(field)), String.Empty, row(field))
        Dim DT_Impostazioni As DataTable
        DT_Impostazioni = objImpostazioni.LeggiDatiImpostazione(impCod, xFiltroAggiuntivo:=String.Empty,
                                                                enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                obj_Utenti)
        Dim data = DT_Impostazioni.Select.
                   Select(Function(row) New profilazione.ImpostazioneBase With {
                       .codice = getStringOrEmpty(row, "Codice"),
                       .descrizione = getStringOrEmpty(row, "Descrizione"),
                       .Valore = getStringOrEmpty(row, "Valore_Default"),
                       .TipoCampo = getStringOrEmpty(row, "Tipo_Campo"),
                       .Note = getStringOrEmpty(row, "Note")
                   }).ToList()

        Return New With {
                        .Impostazione_Cod = impCod,
                        .data = data
                    }
    End Function

    Public Function CaricaImpostazioni_Aziende_Centri(impostazioni As IEnumerable(Of Integer), obj_Server As AgronicaCoreParametri) As IEnumerable(Of Object)
        Dim objImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim getStringOrEmpty = Function(row As DataRow, field As String) If(IsDBNull(row(field)), String.Empty, row(field))
        Dim DT_Impostazioni = objImpostazioni.LeggiDatiImpostazione(impostazioni, xFiltroAggiuntivo:=String.Empty,
                                                                enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                obj_Server)
        Return DT_Impostazioni.Select.
            GroupBy(Function(r) r("Impostazione_Cod")).
            Select(Function(impostazione) New With {
                .Impostazione_Cod = impostazione.Key,
                .data = impostazione.Select(Function(row As DataRow) New profilazione.ImpostazioneBase With {
                    .codice = getStringOrEmpty(row, "Codice"),
                    .descrizione = getStringOrEmpty(row, "Descrizione"),
                    .Valore = getStringOrEmpty(row, "Valore_Default"),
                    .TipoCampo = getStringOrEmpty(row, "Tipo_Campo"),
                    .Note = getStringOrEmpty(row, "Note")
                }).ToList
            })
    End Function

    Public Function CaricaDatiImpostazioni_Aziende_Centri_NG(azienda As ImpresaDto,
                                                    ByRef ListaImpostazioni As IEnumerable(Of Object),
                                                    obj_Utenti As AgronicaCoreParametri,
                                                    obj_Server As AgronicaCoreParametri)
        Dim objSettings As New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_R
        Dim saCodList = New List(Of Integer) From {azienda.Sa_Cod}

        Dim valoriImpostati As New List(Of Impostazione_DB)
        Dim getDeafult = Function(cod As Integer, ll As IEnumerable(Of Object)) ll.Where(Function(i) i.Impostazione_Cod = cod).
            Select(Of IEnumerable(Of profilazione.ImpostazioneBase))(Function(i) i.data).
            Select(Of String)(Function(i) i.Select(Of String)(Function(d) d.Valore).FirstOrDefault).
            DefaultIfEmpty(String.Empty).First
        Dim impostazioni = ListaImpostazioni.Select(Function(i) i.Impostazione_Cod).GetEnumerator()
        While impostazioni.MoveNext
            Dim valoreStr = objSettings.LeggiScalareMulticentroAziendaSuperUser(
                azienda.piva, saCodList,
                impostazioni.Current, getDeafult(impostazioni.Current, ListaImpostazioni),
                obj_Utenti, obj_Server
            )
            valoriImpostati.Add(New Impostazione_DB With {
                .Piva = azienda.piva,
                .Sa_Cod = azienda.Sa_Cod,
                .Impostazione_Cod = impostazioni.Current,
                .Impostazione_Valore = valoreStr
            })
        End While

        Return ListaImpostazioni.Select(Function(item)
                                            Dim valoreDB As Impostazione_DB = valoriImpostati.Find(Function(i) i.Impostazione_Cod = item.Impostazione_Cod)
                                            item.data = patchValue(item.data, valoreDB.Impostazione_Valore)
                                            Return item
                                        End Function)
    End Function

    Private Function patchValue(ByRef valori As List(Of profilazione.ImpostazioneBase), valueStr As String) As List(Of profilazione.ImpostazioneBase)
        Select Case valori.First.TipoCampo
            Case enum_TipoControllo.CASELLA_SPUNTA
                If valori.Count = 1 Then
                    valori.First.Valore = valueStr
                Else
                    Dim selezionati As New List(Of String)
                    If valueStr.Contains(",") Then
                        selezionati.AddRange(valueStr.Split(","))
                    ElseIf valueStr.Contains("|") Then
                        selezionati.AddRange(valueStr.Split("|"))
                    End If
                    valori.ForEach(Sub(x) x.Valore = If(selezionati.Contains(x.codice), "1", "0"))
                End If

            Case enum_TipoControllo.NUMERO_DECIMALE,
                 enum_TipoControllo.NUMERO_INTERO,
                 enum_TipoControllo.CASELLA_TESTO,
                 enum_TipoControllo.AREA_TESTO

                If valori.Count = 1 Then
                    valori.First.Valore = valueStr
                Else
                    Throw New InvalidOperationException("More than one control seems to be associated to this setting")
                End If

            Case enum_TipoControllo.CALENDARIO
                'Dim yyyymmdd2date = Function(str)
                '                        Dim yyyy = Integer.Parse(valueStr.Substring(0, 4))
                '                        Dim mm = Integer.Parse(valueStr.Substring(4, 2))
                '                        Dim dd = Integer.Parse(valueStr.Substring(6, 2))
                '                        Return New Date(yyyy, mm, dd)
                '                    End Function
                'valori.ForEach(Sub(ddlItem) ddlItem.Valore = yyyymmdd2date(valueStr))
                valori.ForEach(Sub(ddlItem) ddlItem.Valore = valueStr)

            Case enum_TipoControllo.MENU_DISCESA,
                 enum_TipoControllo.PULSANTE_SCELTA
                valori.ForEach(Sub(ddlItem) ddlItem.Valore = valueStr)

            Case Else
                valori.ForEach(Sub(ddlItem) ddlItem.Valore = valueStr)

        End Select
        Return valori
    End Function
End Class

Public Class Imprese_Impostazioni_W

    <Obsolete("This method is deprecated, use ScriviModifica_Impostazione instead.")>
    Public Sub SalvaImpostazioni_Aziende_Centri(ByVal azienda As ImpresaDto, Impostazioni As List(Of profilazione.Impostazione), obj_Server As AgronicaCoreParametri)

        Dim objSettings_W As New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_W
        Dim objSettings_R As New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_R

        Dim PIVA = azienda.piva
        Dim Sa_Cod = azienda.Sa_Cod

        Dim list = Impostazioni.GroupBy(Of Integer)(Function(i)
                                                        Return i.Impostazione_Cod
                                                    End Function).ToList()
        Dim settingsCode
        Dim valueToSave As String = ""

        For Each imp In list
            settingsCode = imp.First.Impostazione_Cod
            Select Case imp.First.Tipo_Campo
                Case "checkbox"
                    valueToSave = "DEFAULT"
                        'valueToSave = handleCheckbox(imp.ToArray())

                Case "radiobutton", "ddl", "multi", "filtroGiacenze", "filtroLotti"
                    valueToSave = imp.First.value

                Case "text", "number", "stampaDocContab"
                    For Each it2 In imp.Take(imp.Count - 1)
                        ScriviModifica_Impostazione(PIVA, Sa_Cod, settingsCode, valueToSave, obj_Server)

                    Next
                    settingsCode = imp.Last.codice
                    valueToSave = imp.Last.value

                Case "date"
                    Dim data
                    For Each it2 In imp.Take(imp.Count - 1)
                        If it2.value.Equals("") Then
                            Continue For
                        End If
                        data = CDate(it2.value)

                        valueToSave += If(data.Day < 10, "0" & data.Day.ToString(), data.Day.ToString())
                        valueToSave += If(data.Month < 10, "0" & data.Month.ToString(), data.Month.ToString())
                        valueToSave += data.Year.ToString()

                        ScriviModifica_Impostazione(PIVA, Sa_Cod, settingsCode, valueToSave, obj_Server)

                    Next
                    If imp.First.value.Equals("") Then
                        Exit Select
                    End If
                    data = CDate(imp.First.value)
                    valueToSave += If(data.Day < 10, "0" & data.Day.ToString(), data.Day.ToString())
                    valueToSave += If(data.Month < 10, "0" & data.Month.ToString(), data.Month.ToString())
                    valueToSave += data.Year.ToString()

            End Select

            ScriviModifica_Impostazione(PIVA, Sa_Cod, settingsCode, valueToSave, obj_Server)


        Next


    End Sub

    Public Sub ScriviModifica_Impostazione(PIVA As String, Sa_Cod As Integer, Impostazione_Cod As Integer, Impostazione_Valore As String, obj_Server As AgronicaCoreParametri)

        Dim objImpostazioni_W As New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_W
        Dim objImpostazioni_R As New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_R


        Dim dt = objImpostazioni_R.Leggi(PIVA, Sa_Cod, Impostazione_Cod, "", "", obj_Server)
        If dt.Rows.Count > 0 Then
            objImpostazioni_W.Modifica(PIVA, Sa_Cod, Impostazione_Cod, Impostazione_Valore, AGRODATAINIZIO, AGRODATAFINE, obj_Server)
        Else
            If Impostazione_Valore IsNot Nothing AndAlso Impostazione_Valore IsNot "" Then
                objImpostazioni_W.Scrivi(PIVA, Sa_Cod, Impostazione_Cod, Impostazione_Valore, AGRODATAINIZIO, AGRODATAFINE, obj_Server)
            End If
        End If

    End Sub

    ''' <summary>
    ''' Sovrascrive le impostazione dell'azienda <c>base</c> impostando i valori delle impostazioni dell'azienda <c>template</c>.
    ''' </summary>
    ''' <remarks>
    ''' Tutte le impostazione dell'azienda <c>base</c> vengono prima eliminate,
    ''' poi vengono riscritte identiche a quelle dell'azienda <c>template</c>.
    ''' </remarks>
    ''' <param name="base">Chiave dell'azienda a cui si vogliono cambiare le impostazioni</param>
    ''' <param name="template">Chiave dell'azienda dal quale si vogliono copiare le impostazioni</param>
    ''' <param name="objParametri_Server"></param>
    ''' <param name="usaSingolaTransizione">Se impostato a `True` esegue l'operazione in mod atomico (default)</param>
    Public Sub CopiaImpostazioni(base As ImpresaDto, template As ImpresaDto, objParametri_Utenti As AgronicaCoreParametri, objParametri_Server As AgronicaCoreParametri, Optional usaSingolaTransizione As Boolean = True)


        Dim objImpostazioni_R As New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_R
        Dim objImpostazioni_W As New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_W
        Dim noFiltro = ""
        Dim tutteImpostazioni = 0

        Try
            If usaSingolaTransizione Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)
            End If

            Dim enu = objImpostazioni_R.Leggi(
                base.piva, base.Sa_Cod, tutteImpostazioni,
                noFiltro, noFiltro, objParametri_Server
            ).Rows.GetEnumerator
            While enu.MoveNext
                objImpostazioni_W.Cancella(base.piva, base.Sa_Cod, enu.Current.item("Impostazione_Cod"), objParametri_Server)
            End While

            enu = objImpostazioni_R.Leggi(
                template.piva, template.Sa_Cod, tutteImpostazioni,
                noFiltro, noFiltro, objParametri_Server
            ).Rows.GetEnumerator
            While enu.MoveNext
                objImpostazioni_W.Scrivi(
                    base.piva, base.Sa_Cod, enu.Current.item("Impostazione_Cod"),
                    enu.Current.item("Impostazione_Valore"),
                    enu.Current.item("Validita_Inizio"), enu.Current.item("Validita_Fine"),
                    objParametri_Server
                )
            End While

            If usaSingolaTransizione Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server)
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)
            End If
        Catch ex As Exception
            If usaSingolaTransizione Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)
            End If
            Throw ex
        End Try

    End Sub

    ''' <summary>
    ''' Genera la stringa da salvare per capi di tipo checkbox. 
    ''' Se il campo contiente un solo elemento, verrà restituito il valore "1" se true o "0" se false.
    ''' Se il campo contiene più elementi verrà restituita una concatenazione dei codici dei valori selezionati. 
    ''' (i.e. "0,1,4,7,13")
    ''' </summary>
    ''' <param name="checkboxOptions"></param>
    ''' <returns>Una stringa rappresentante la selezione effettuata.</returns>
    Private Function handleCheckbox(checkboxOptions As profilazione.Impostazione()) As String
        Dim selected As String = ""

        If checkboxOptions.Length < 2 Then
            Return CBool(checkboxOptions.First.value)
        End If

        For Each chk In checkboxOptions
            If chk.value = "true" Then
                selected += chk.codice & ","
            End If
        Next

        If selected.Length > 1 Then
            selected = selected.Remove(selected.Length - 1, 1)
        End If

        Return selected
    End Function

    Public Sub CancellaImpostazioni_Azienda_Centri(imprese As Object(), objServer As AgronicaCoreParametri)

        Dim objImpostazioni As New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_W
        Dim PIVA As String = ""
        Dim Sa_Cod As Integer = 0

        For Each azienda In imprese
            PIVA = azienda.item("piva")
            Sa_Cod = If(azienda.item("Sa_Cod").Equals(0), SACOD_NOFILTRO, azienda.item("Sa_Cod"))

            objImpostazioni.Cancella(PIVA, Sa_Cod, 0, objServer)

        Next

    End Sub
    Public Sub CancellaImpostazioni_AziendeCentri(impostazioni As IEnumerable(Of Imprese_Impostazioni), objServer As AgronicaCoreParametri)
        Dim objImpostazioni As New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_W
        Try
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, objServer)

            impostazioni.ToList.ForEach(Sub(impostazioneImpresa) objImpostazioni.Cancella(
                impostazioneImpresa.Piva, impostazioneImpresa.Sa_Cod,
                impostazioneImpresa.Impostazione_Cod, objServer
            ))

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(1, objServer)
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objServer)
        Catch ex As Exception
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objServer)
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objServer)
            Throw
        End Try
    End Sub

    Public Sub CancellaImpostazione_Azienda_Centri(azienda As IDictionary(Of String, Object), Impostazione_Cod As Integer, objServer As AgronicaCoreParametri)
        Dim objImpostazioni As New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_W
        Dim PIVA As String = azienda.Item("piva")
        Dim Sa_Cod As Integer = If(azienda.Item("Sa_Cod").Equals(0), SACOD_NOFILTRO, azienda.Item("Sa_Cod"))

        objImpostazioni.Cancella(PIVA, Sa_Cod, Impostazione_Cod, objServer)

    End Sub
    Public Sub CancellaImpostazione_Azienda_Centri_NG(azienda As ImpresaDto, Impostazione_Cod As Integer, objServer As AgronicaCoreParametri)
        Dim objImpostazioni As New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_W
        Dim PIVA As String = azienda.piva
        Dim Sa_Cod As Integer = If(azienda.Sa_Cod.Equals(0), SACOD_NOFILTRO, azienda.Sa_Cod)

        objImpostazioni.Cancella(PIVA, Sa_Cod, Impostazione_Cod, objServer)

    End Sub

End Class
