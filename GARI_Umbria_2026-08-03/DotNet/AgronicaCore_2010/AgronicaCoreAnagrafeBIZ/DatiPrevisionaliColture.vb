Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports InData.DatiPrevisionaliColture
Imports AgronicaCoreModelsSTD.metaschema
Imports AgronicaCoreModelsSTD.baseClass
Imports AgronicaCoreEntityFramework
Imports Newtonsoft.Json.Linq
Imports System.Linq


Public Class DatiPrevisionaliNoDataFoundException : Inherits System.Data.DataException
    Public ReadOnly requestData As DatiPrevisionaliColtureRequest
    Sub New(ByVal _msg As String, ByVal _requestData As DatiPrevisionaliColtureRequest)
        MyBase.New(_msg)
        requestData = _requestData
    End Sub
End Class

Public Class DatiPrevisionaliNoDataFoundAggregateException : Inherits System.Data.DataException
    Public ReadOnly datiPrevExs As List(Of DatiPrevisionaliNoDataFoundException)
    Sub New(ByVal _datiPrevExs As List(Of DatiPrevisionaliNoDataFoundException))
        MyBase.New("")
        datiPrevExs = _datiPrevExs
    End Sub

    Public Function GetExceptionSpecies() As HashSet(Of BaseCodeDescr)
        Dim species As New HashSet(Of BaseCodeDescr)
        For Each ex In datiPrevExs
            species.Add(ex.requestData.vegCod)
        Next
        Return species
    End Function
End Class

Public Class DatiPrevisionaliColtureRead

    '''dts stores all the datatable already read to avoid wasting time reading again same data
    Private dts As New Hashtable

    Public Function readDatiPrevisionali(
                                        ByVal params As DatiPrevisionaliColtureRequest,
                                        ByVal objParametri As AgronicaCoreParametri,
                                        Optional readDescription As Boolean = True
                                        ) As DatiPrevisionaliColture

        Dim dati As New DatiPrevisionaliColture
        Dim objDatiPrevisionali As New DatiPrevisionaliColture_R

        Dim priority = getRequestDataPriority()
        Dim dt As New DataTable

        If dts.ContainsKey(params.vegCod.codice) Then
            dt = dts.Item(params.vegCod.codice)
        Else
            dt = objDatiPrevisionali.readDatiPrevisionali(params, objParametri)
            dts.Add(params.vegCod.codice, dt)
        End If

        If dt.Rows.Count > 0 Then
            Dim prioEnum = priority.priorityList.GetEnumerator()
            Dim dv = dt.AsDataView

            Dim rowFilter = "1 = 1"
            While prioEnum.MoveNext AndAlso dv.Count > 1    'se ho un solo record rimasto in tabella, non ha senso proseguire il ciclo
                Dim filter = params.getFieldValueString(prioEnum.Current.cod)

                If dv.Table.Columns.Contains(filter.cod) Then   'controllo che la colonna sia presente nel DT
                    If prioEnum.Current.cod = "validitaInizio" OrElse prioEnum.Current.cod = "validitaFine" Then
                        rowFilter = rowFilter & " AND " & filter.cod & " = #" & Date.Parse(filter.value).ToString("MM/dd/yyyy") & "#"
                    Else
                        rowFilter = rowFilter & " AND " & filter.cod & " = " & filter.value
                    End If
                End If

                Dim tempRowFilter = dv.RowFilter
                dv.RowFilter = rowFilter

                If dv.Count = 0 Then
                    dv.RowFilter = tempRowFilter
                    rowFilter = tempRowFilter
                    'Exit While
                End If
            End While

            dati.parametroCod = params.parametroCod.codice
            dati.udm = New UnitaDiMisura(dv.ToTable.Rows(0)("udm_cod"))
            dati.udm.descrizione = dv.ToTable.Rows(0)("UDM_DES")
            dati.udm.simbolo = dv.ToTable.Rows(0)("UDM_SIM")
            dati.valore = dv.ToTable.Rows(0)("Valore")
        Else
            Dim species As New HashSet(Of BaseCodeDescr)
            species.Add(params.vegCod)

            Throw New DatiPrevisionaliNoDataFoundException(
            String.Concat(
                "Nessuna resa trovata per la specie: ",
                If(readDescription, GetSpecieDescriptions(species, objParametri).First(), "")
            ), params)
        End If

        Return dati

    End Function


    Public Function readDatiPrevisionaliDT(
                                          ByVal params As DatiPrevisionaliColtureRequest,
                                          ByVal objParametri As AgronicaCoreParametri
                                          ) As DataTable

        Dim objDatiPrevisionali As New DatiPrevisionaliColture_R
        Return objDatiPrevisionali.readDatiPrevisionaliAll(params, objParametri)

    End Function

    Private Function getRequestDataPriority() As RequestDataPriority

        Dim prio As New RequestDataPriority(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14)
        Return prio

    End Function

    Public Function readDatiPrevisionaliForImpianto(
                                                   ByVal piva As String,
                                                   ByVal sa_cod As Integer,
                                                   ByVal appezza As Integer,
                                                   ByVal id_reg As Integer,
                                                   ByVal objParametriServer As AgronicaCoreParametri,
                                                   ByVal objParametriUtenti As AgronicaCoreParametri,
                                                   ByVal objParametriSuperServer As AgronicaCoreParametri,
                                                   Optional pro_cod As Integer = 0 'ProgettoCod, per scegliere subito l'esercizio
                                                   ) As DatiPrevisionaliColture

        Dim objregImpiantoRead As New Reg_Impianto_R
        Dim objDatiPrevisionali As New DatiPrevisionaliColture_R

        Dim impianto = objregImpiantoRead.Leggi_Impianto_Anagrafica(piva, sa_cod, appezza, id_reg, True, False, New Date(), False, False, objParametriSuperServer, objParametriServer, objParametriUtenti)
        impianto.esercizi.Sort(Function(e1, e2) e1.validita.inizio.CompareTo(e2.validita.inizio))
        If pro_cod <> 0 Then
            Dim es = impianto.esercizi.Find(Function(e) e.codice = pro_cod)
            impianto.esercizi = New List(Of AgronicaCoreModelsSTD.anagrafiche.Esercizio)
            impianto.esercizi.Add(es)
        End If

        If TypeOf impianto.utilizzoTerreno Is AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta Then
            Dim esercizio = impianto.esercizi(0)
            Dim indirizzi = objDatiPrevisionali.readAddressForImpianto(piva, sa_cod, appezza, objParametriServer)

            Dim prov = indirizzi.Rows(0)("pro_cod_istat")
            Dim reg = indirizzi.Rows(0)("REG")
            Dim codiceStato As String = indirizzi.Rows(0)("stato")

            Dim params As New DatiPrevisionaliColtureRequest
            params.culCod = New BaseCodeDescr(impianto.utilizzoTerreno.codice, "")
            params.grfiCod = New BaseCodeDescr(impianto.gruppoFinalita.codice, "")
            params.grvaCod = New BaseCodeDescr(impianto.gruppoVarietale.codice, "")
            params.parametroCod = New BaseCodeDescr(4, "")
            params.portCod = New BaseCodeDescr(impianto.portinnesto.codice, "")
            params.foralCod = New BaseCodeDescr(impianto.formaAllevamento.codice, "")
            params.dettSpeciePersonalizzatoCod = impianto.dettaglio_varieta_personalizzato
            params.prov = New BaseCodeDescrStr(prov, "")
            params.reg = New BaseCodeDescrStr(reg, "")
            params.codiceStato = New BaseCodeDescrStr(codiceStato, "")
            params.regCod = New BaseCodeDescr(esercizio.regolamento.codice, "")
            params.statoCod = New BaseCodeDescr(esercizio.apportiMassimiMacroelementi.fase.codice, "")
            params.vegCod = New BaseCodeDescr(CType(impianto.utilizzoTerreno, AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta).specie.codice, "")
            params.validitaInizio = esercizio.validita.inizio
            params.validitaFine = esercizio.validita.fine

            Return readDatiPrevisionali(params, objParametriServer)
        Else
            Return New DatiPrevisionaliColture()
        End If

    End Function

    ''' <summary>
    ''' Rispetto alla readDatiPrevisionaliForImpianto, in questa versione i dati dell'indirizzo vengo letti in precedenza e passati all'interno dell'objImpianto, evitando letture continue
    ''' Non c'è il controllo sull'utilizzo perchè dalla modifica multipla vengono passati solo impianti con specie
    ''' non c'è bisogno di firlare sul progetto_cod perchè ci viene passato l'impianto-esercizio corretto
    ''' </summary>
    ''' <param name="impianto"></param>
    ''' <param name="objParametriServer"></param>
    ''' <returns></returns>
    Public Function readDatiPrevisionaliForImpianto_ModificaMultipla(ByRef impianto As Object,
                                                                     ByVal objParametriServer As AgronicaCoreParametri
                                                                     ) As DatiPrevisionaliColture

        Dim objDatiPrevisionali As New DatiPrevisionaliColture_R

        Dim params As New DatiPrevisionaliColtureRequest
        params.culCod = New BaseCodeDescr(impianto("Cul_Cod"), "")
        params.grfiCod = New BaseCodeDescr(impianto("GRFI_Cod"), "")
        params.grvaCod = New BaseCodeDescr(impianto("GRVA_Cod_VEG"), "")
        params.parametroCod = New BaseCodeDescr(4, "")
        params.portCod = New BaseCodeDescr(impianto("PORT_COD"), "")
        params.foralCod = New BaseCodeDescr(impianto("FORAL_COD"), "")
        params.dettSpeciePersonalizzatoCod = New BaseCodeDescrStr(impianto("dettSpeciePersonalizzatoCod"), "")
        params.prov = New BaseCodeDescrStr(impianto("pro_cod_istat"), "")
        params.reg = New BaseCodeDescrStr(impianto("REG"), "")
        params.codiceStato = New BaseCodeDescrStr(impianto("stato"), "")
        params.regCod = New BaseCodeDescr(impianto("Regolamento"), "")
        params.statoCod = New BaseCodeDescr(impianto("StatoCod"), "")
        params.vegCod = New BaseCodeDescr(impianto("veg_cod"), "")
        params.validitaInizio = impianto("Validita_Inizio_Esercizio")
        params.validitaFine = impianto("Validita_Fine_Esercizio")

        Return readDatiPrevisionali(params, objParametriServer)

    End Function

    Public Shared Function GetSpecieDescriptions(
                                          ByVal species As HashSet(Of AgronicaCoreModelsSTD.baseClass.BaseCodeDescr),
                                          ByVal objParametriServer As AgronicaCoreParametri,
                                          Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing
                                          ) As List(Of String)
        If IsNothing(GiasContext) Then
            Dim gefutils As New Gias_EF_Utility
            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametriServer.StringaConnessione)
            GiasContext = New Gias_DeveloperServer_Entities(EFConnString)
        End If

        Dim descriptions As New List(Of String)
        For Each specie In species
            Dim specieVegetale = (
                From e In GiasContext.SpecieVegetali
                Where e.Veg_Cod = specie.codice
                    ).FirstOrDefault

            If specieVegetale IsNot Nothing
                descriptions.Add(specieVegetale.Veg_Des)
            End If
        Next
        Return descriptions
    End Function

End Class

Public Class DatiPrevisionaliColtureEdit

    Public Sub editDatiPrevisionaliColture(ByVal data As DatiPrevisionaliColtureComplete,
                                           ByRef objParametri_Server As AgronicaCoreParametri,
                                           ByRef objParametri_Utenti As AgronicaCoreParametri)

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.DatiPrevisionaliColtureEdit.editDatiPrevisionaliColture()"

        Dim dal As New DatiPrevisionaliColture_W
        Dim record = dal.editDatiPrevisionaliColtureEF(
            data,
            objParametri_Server,
            objParametri_Utenti
            )

    End Sub

    Public Sub createDatiPrevisionaliColture(ByVal data As DatiPrevisionaliColtureComplete,
                                             ByRef objParametri_Server As AgronicaCoreParametri,
                                             ByRef objParametri_Utenti As AgronicaCoreParametri)

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.DatiPrevisionaliColtureEdit.createDatiPrevisionaliColture()"

        Dim dal As New DatiPrevisionaliColture_W
        Dim record = dal.createDatiPrevisionaliColtureEF(
            data,
            objParametri_Server,
            objParametri_Utenti
            )

    End Sub

    Public Sub deleteDatiPrevisionaliColture(ByVal data As DatiPrevisionaliColtureComplete,
                                             ByRef objParametri_Server As AgronicaCoreParametri,
                                             ByRef objParametri_Utenti As AgronicaCoreParametri)

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.DatiPrevisionaliColtureEdit.deleteDatiPrevisionaliColture()"

        Dim dal As New DatiPrevisionaliColture_W
        Dim record = dal.deleteDatiPrevisionaliColtureEF(
            data,
            objParametri_Server,
            objParametri_Utenti
            )

    End Sub

End Class