Imports AgronicaCoreDataProvider
Imports AgronicaCoreModelsSTD
Imports AgronicaCoreModelsSTD.metaschema

Public Class MisurexIndiciMaturita_Anagrafiche

    Public Function GetValuesFor(
       matIdxCod As Integer, umCod As Integer, vegCod As Integer, objParametri_Server As AgronicaCoreParametri
   ) As DataTable
        Dim reader As New AgronicaCoreMetaSchemaDAL.MisuraxIndiciMaturita_Anagrafiche_R
        Dim todayStr = Today.ToShortDateString.Replace("/", "-")
        Dim validityFilter = " MisuraXIndiciMaturita_Anagrafiche.Validita_Inizio <= '" & todayStr &
            "' AND MisuraXIndiciMaturita_Anagrafiche.Validita_Fine >= '" & todayStr & "'"
        Dim orderBy = "MisuraXIndiciMaturita_Anagrafiche.Anag_des ASC"
        Return reader.Leggi(
            matIdxCod, umCod, vegCod,
            validityFilter, orderBy, objParametri_Server
        )
    End Function

    Public Function GetValuesListFor(
        matIdxCod As Integer, umCod As Integer, vegCod As Integer, params As ObjParams
    ) As IEnumerable(Of baseClass.IBaseCodeDescr)

        Dim DT As DataTable = Nothing

        If vegCod = CostantiPersonalizzate.NessunaSpecieQdC Then
            Dim xRead As New AgronicaCoreMetaSchemaDAL.MisuraxIndiciMaturita_Anagrafiche_R
            DT = xRead.Leggi_SenzaSpecie(matIdxCod, umCod, params.ObjParametri_Server)
        Else
            DT = GetValuesFor(matIdxCod, umCod, vegCod, params.ObjParametri_Server)
        End If

        Return DT.Select.Select(Function(row) New baseClass.BaseCodeDescr(row("Anag_valore"), row("Anag_des")))
    End Function

    Public Function LeggiAnagrafiche(ByVal indMatCod As Integer, ByVal udmCod As Integer, ByVal anagCod As Integer, ByRef objParametriServer As AgronicaCoreParametri) As List(Of MisuraPerIndiciMaturitaAnagrafica)

        Dim result As New List(Of MisuraPerIndiciMaturitaAnagrafica)
        Dim xRead As New AgronicaCoreMetaSchemaDAL.MisuraxIndiciMaturita_Anagrafiche_R

        Dim dt = xRead.LeggiAnagrafiche(indMatCod, udmCod, anagCod, objParametriServer)

        For Each row In dt.Rows
            Dim anagrafica As New MisuraPerIndiciMaturitaAnagrafica
            anagrafica.Ind_Mat_Cod = CInt(row("IND_MAT_COD"))
            anagrafica.Ind_Mat_Des = row("IND_MAT_DES").ToString()
            anagrafica.Udm_Cod = CInt(row("Udm_Cod"))
            anagrafica.Udm_Des = row("UDM_DES").ToString()
            anagrafica.Anag_Cod = CInt(row("Anag_Cod"))
            anagrafica.Anag_Des = row("Anag_des").ToString()
            anagrafica.Anag_Valore = CInt(row("Anag_valore"))
            anagrafica.DescrizioneMisura = $"{anagrafica.Ind_Mat_Des} ({anagrafica.Udm_Des})"
            anagrafica.CodiceCompostoMisura = $"{anagrafica.Ind_Mat_Cod}_{anagrafica.Udm_Cod}"

            Dim utilizzo As Boolean = xRead.VerificaUtilizzoAnagrafica(anagrafica.Ind_Mat_Cod, anagrafica.Udm_Cod, objParametriServer)

            anagrafica.Modificabile = Not utilizzo
            anagrafica.Cancellabile = Not utilizzo

            result.Add(anagrafica)
        Next

        Return result

    End Function

    Public Function InserisciAnagrafiche(ByVal listaAnagraficheDaInserire As List(Of MisuraPerIndiciMaturitaAnagrafica),
                                         ByRef objParametriServer As AgronicaCoreParametri) As Boolean

        Dim response As Boolean

        Dim writeDal As New AgronicaCoreMetaSchemaDAL.MisuraxIndiciMaturita_Anagrafiche_W
        Dim objSequenze As New Agro_Sequenze

        For Each anagrafica In listaAnagraficheDaInserire

            If anagrafica.Ind_Mat_Cod = -1 Then
                Throw New Exception("Impossibile inserire una nuova anagrafica senza Ind_Mat_Cod")
            End If

            If anagrafica.Udm_Cod = -1 Then
                Throw New Exception("Impossibile inserire una nuova anagrafica senza Udm_Cod")
            End If

            If anagrafica.Anag_Valore <= 0 Then
                Throw New Exception("Il valore dell'anagrafica deve essere maggiore di 0")
            End If

            If anagrafica.Anag_Cod = 0 Then
                anagrafica.Anag_Cod = objSequenze.NuovoId_Tabella("misuraxindicimaturita_anagrafiche",
                                                                  0,
                                                                  Int32.MaxValue,
                                                                  objParametriServer,
                                                                  True)

            End If

            response = writeDal.ScriviAnagrafica(anagrafica.Ind_Mat_Cod,
                                                 anagrafica.Udm_Cod,
                                                 anagrafica.Anag_Cod,
                                                 anagrafica.Anag_Des,
                                                 anagrafica.Anag_Valore,
                                                 objParametriServer)

            If Not response Then
                Exit For
            End If

        Next

        Return response

    End Function

    Public Function AggiornaAnagrafiche(ByVal listaAnagraficheDaAggiornare As List(Of MisuraPerIndiciMaturitaAnagrafica),
                                        ByRef objParametriServer As AgronicaCoreParametri) As Boolean

        Dim response As Boolean

        Dim writeDal As New AgronicaCoreMetaSchemaDAL.MisuraxIndiciMaturita_Anagrafiche_W
        Dim readDal As New AgronicaCoreMetaSchemaDAL.MisuraxIndiciMaturita_Anagrafiche_R

        Dim dt As DataTable

        For Each anagrafica In listaAnagraficheDaAggiornare

            If anagrafica.Ind_Mat_Cod = -1 Then
                Throw New Exception("Impossibile aggiornare un'anagrafica senza Ind_Mat_Cod.")
            End If

            If anagrafica.Udm_Cod = -1 Then
                Throw New Exception("Impossibile aggiornare un'anagrafica senza Udm_Cod")
            End If

            If anagrafica.Anag_Cod = 0 Then
                Throw New Exception("Impossibile aggiornare un'anagrafica senza Anag_Cod.")
            End If

            If anagrafica.Anag_Valore <= 0 Then
                Throw New Exception("Il valore dell'anagrafica deve essere maggiore di 0")
            End If

            dt = readDal.LeggiAnagrafiche(anagrafica.Ind_Mat_Cod, anagrafica.Udm_Cod, anagrafica.Anag_Cod, objParametriServer)

            If dt Is Nothing OrElse dt.Rows.Count = 0 Then
                Throw New Exception("Anagrafica indice maturità non trovata")
            End If

            Dim utilizzo = readDal.VerificaUtilizzoAnagrafica(anagrafica.Ind_Mat_Cod, anagrafica.Udm_Cod, objParametriServer)

            If utilizzo Then
                Throw New Exception("Anagrafica indice maturità utilizzata, impossibile modificare")
            End If

            response = writeDal.AggiornaAnagrafica(anagrafica.Ind_Mat_Cod, anagrafica.Udm_Cod,
                        anagrafica.Anag_Cod, anagrafica.Anag_Des, anagrafica.Anag_Valore,
                        objParametriServer)

            If Not response Then
                Exit For
            End If

        Next

        Return response

    End Function

    Public Function EliminaAnagrafiche(ByVal listaAnagraficheDaEliminare As List(Of MisuraPerIndiciMaturitaAnagrafica),
                                        ByRef objParametriServer As AgronicaCoreParametri) As Boolean

        Dim response As Boolean

        Dim writeDal As New AgronicaCoreMetaSchemaDAL.MisuraxIndiciMaturita_Anagrafiche_W
        Dim readDal As New AgronicaCoreMetaSchemaDAL.MisuraxIndiciMaturita_Anagrafiche_R

        Dim dt As DataTable

        For Each anagrafica In listaAnagraficheDaEliminare
            If anagrafica.Ind_Mat_Cod = -1 Then
                Throw New Exception("Impossibile eliminare un'anagrafica senza Ind_Mat_Cod.")
            End If

            If anagrafica.Udm_Cod = -1 Then
                Throw New Exception("Impossibile eliminare un'anagrafica senza Udm_Cod")
            End If

            If anagrafica.Anag_Cod = 0 Then
                Throw New Exception("Impossibile eliminare un'anagrafica senza Anag_Cod.")
            End If

            dt = readDal.LeggiAnagrafiche(anagrafica.Ind_Mat_Cod, anagrafica.Udm_Cod, anagrafica.Anag_Cod, objParametriServer)

            If dt Is Nothing OrElse dt.Rows.Count = 0 Then
                Throw New Exception("Anagrafica indice maturità non trovata")
            End If

            Dim utilizzo = readDal.VerificaUtilizzoAnagrafica(anagrafica.Ind_Mat_Cod, anagrafica.Udm_Cod, objParametriServer)

            If utilizzo Then
                Throw New Exception("Anagrafica indice maturità utilizzata, impossibile eliminare")
            End If

            response = writeDal.EliminaAnagrafica(anagrafica.Ind_Mat_Cod, anagrafica.Udm_Cod,
                anagrafica.Anag_Cod, objParametriServer)

            If Not response Then
                Exit For
            End If

        Next

        Return response

    End Function

End Class
