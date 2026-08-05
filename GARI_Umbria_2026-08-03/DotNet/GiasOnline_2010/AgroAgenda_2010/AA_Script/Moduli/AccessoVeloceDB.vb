Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi


Module AccessoVeloceDB

    Public Function TipoSelect_FiltroneSuperNova_from_TipoFiltroImprese(ByVal TipofiltroImprese As enum_TipoFiltroImprese) As enum_TipoSelect_FiltroneSuperNova

        'ImpreseAlbero = 1
        'Imprese = 2
        'CentriAziendali = 3
        'Appezzamenti = 4
        'Impianti = 5
        'Movimenti = 6
        'Contatti = 7
        'Campi = 8

        Select Case TipofiltroImprese

            Case enum_TipoFiltroImprese.f_Nessuno

                Return enum_TipoSelect_FiltroneSuperNova.Base

            Case enum_TipoFiltroImprese.f_RagioneSociale, _
                    enum_TipoFiltroImprese.f_PartitaIVA, _
                    enum_TipoFiltroImprese.f_CUAA, _
                    enum_TipoFiltroImprese.f_Regione, _
                    enum_TipoFiltroImprese.f_Provincia, _
                    enum_TipoFiltroImprese.f_Comune

                Return enum_TipoSelect_FiltroneSuperNova.Imprese

            Case enum_TipoFiltroImprese.f_CodiceOperatore
                Return enum_TipoSelect_FiltroneSuperNova.Imprese

            Case enum_TipoFiltroImprese.f_RappresentanteLegale
                Return enum_TipoSelect_FiltroneSuperNova.Contatti

            Case enum_TipoFiltroImprese.f_Specie
                Return enum_TipoSelect_FiltroneSuperNova.Impianti

            Case enum_TipoFiltroImprese.f_Codici
                Return enum_TipoSelect_FiltroneSuperNova.Imprese_Codici

            Case Else
                Return enum_TipoSelect_FiltroneSuperNova.Base

                'Case enum_TipoFiltroImprese.f_NormaRiferimento
                '    Return enum_TipoSelect_FiltroneSuperNova.

                'Case enum_TipoFiltroImprese.f_ValidazioneImprese
                '    Return enum_TipoSelect_FiltroneSuperNova.

        End Select



    End Function



    '################################################################################
    Public Function DesLib_from_IdAgenda(ByRef objParametri_Server As AgronicaCoreParametri, _
                                            ByVal Piva As String, _
                                            ByVal Sa_Cod As Integer, _
                                            ByVal Id_Agenda As Integer) As String

        Dim DT As DataTable

        Dim leggiAgenda As New AgronicaCoreContabDAL.Agenda_R
        leggiAgenda.Leggi( _
            Piva, _
            Sa_Cod, _
            Id_Agenda,
            0, _
            AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
            "", _
            "", _
            objParametri_Server _
        )

        If Not IsNothing(DT) Then
            If DT.Rows.Count <> 0 Then
                Return DT.Rows(0).Item("Des_Lib")
            Else
                Return ""
            End If
        Else
            Return ""
        End If

    End Function


    '################################################################################
    Public Function Numero_Imprese_from_SuperUser( _
                                                ByRef objParametri_Server As AgronicaCoreParametri, _
                                                ByRef objSession As System.Web.SessionState.HttpSessionState, _
                                                ByRef objPage As System.Web.UI.Page, _
                                                ByRef Piva As String, _
                                                ByRef Rag_Soc As String) As Integer

        Dim Num_Imprese As Integer = 0


        Piva = ""
        Rag_Soc = ""
        Dim rval As Integer = 0


        Dim DT As DataTable
        Dim imprese_leggi As New AgronicaCoreAnagrafeDAL.Imprese_Read

        DT = imprese_leggi.Leggi( _
                Piva, _
                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                "", _
                "", _
                objParametri_Server _
            )


        If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

            Piva = DT.Rows(0).Item("Piva")
            Rag_Soc = DT.Rows(0).Item("Rag_Soc")
            Return DT.Rows.Count
        End If


        Return Num_Imprese




    End Function



    'duplicata agronoicacoreanagrafe imprese_r RagSoc_from_Piva
    '################################################################################
    Public Function RagSoc_from_Piva(ByRef objParametri_Server As AgronicaCoreParametri, _
                                    ByRef objSession As System.Web.SessionState.HttpSessionState, _
                                    ByRef objPage As System.Web.UI.Page, _
                                    ByVal Piva As String) As String


        '----- Tabella IMPRESE
        Dim DT_Impresa As DataTable
        Dim Imprese_Leggi As New AgronicaCoreAnagrafeDAL.Imprese_Read
        DT_Impresa = Imprese_Leggi.Leggi(Piva, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

        If Not IsNothing(DT_Impresa) Then
            If DT_Impresa.Rows.Count <> 0 Then
                Return DT_Impresa.Rows(0).Item("Rag_Soc")
            Else
                Return ""
            End If
        Else
            Return ""
        End If



    End Function


    Public Function StallaSpecieDes_from_StallaSpecieCod2(ByVal GenCod As Integer, _
                                                    ByVal SpeCod As Integer, _
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                    ) As String

        Dim objSpecie As New AgronicaCoreZooDAL.Lista_Specie_Animali_R
        Dim Dt As DataTable

        'Leggo le imprese associate al profilo selezionato			
        Dt = objSpecie.Leggi(CInt(GenCod), _
                          CInt(SpeCod), _
                          AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                          "", "", objParametri)

        objSpecie = Nothing

        'Se il recordset non è chiuso allora ...	
        If Not Dt Is Nothing AndAlso Dt.Rows.Count > 0 Then

            Return Dt.Rows(0).Item("Spe_Des")

        End If

        'Elimino il recordset
        Dt = Nothing

    End Function

    Public Function StallaIProDes_from_StallaIProCod2( _
                                  ByVal GenCod As Integer, _
                                  ByVal SpeCod As Integer, _
                                  ByVal IProCod As Integer, _
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim objLista As New AgronicaCoreZooDAL.Lista_Indprod_Animali_R
        Dim Dt As DataTable

        'Leggo le imprese associate al profilo selezionato			
        Dt = objLista.Leggi(GenCod, SpeCod, IProCod, _
                            AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                            "", "", objParametri)

        objLista = Nothing

        'Se il recordset non è chiuso allora ...	
        If Not Dt Is Nothing AndAlso Dt.Rows.Count > 0 Then

            Return Dt.Rows(0).Item("IPro_Des")

        End If

        'Elimino il recordset
        Dt = Nothing

    End Function


    Public Function StallaCategoriaDes_from_StallaCategoriaCod2( _
                                  ByVal GenCod As Integer, _
                                  ByVal SpeCod As Integer, _
                                  ByVal IProCod As Integer, _
                                  ByVal CatCod As Integer, _
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                  ) As String

        Dim Dt As DataTable

        Dim objAnimali As New AgronicaCoreZooDAL.Lista_Categorie_Animali_R

        'Recupero le informazioni		
        Dt = objAnimali.Leggi(CInt(GenCod), _
                            CInt(SpeCod), _
                            CInt(IProCod), _
                            CInt(CatCod), _
                            0.0, "", "", _
                            AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                            "", "", objParametri)

        objAnimali = Nothing

        'Se il recordset non è chiuso allora ...	
        If Not Dt Is Nothing AndAlso Dt.Rows.Count > 0 Then

            Return Dt.Rows(0).Item("Cat_Des")

        End If

        'Elimino il recordset
        Dt = Nothing

    End Function

    Public Function StallaRazzaDes_from_StallaRazzaCod2( _
                                  ByVal GenCod As Integer, _
                                  ByVal SpeCod As Integer, _
                                  ByVal RazCod As Integer, _
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                  ) As String

        Dim Dt As DataTable

        Dim objRazze As New AgronicaCoreZooDAL.Lista_Razze_Animali_R

        'Recupero le informazioni		
        Dt = objRazze.Leggi(CInt(GenCod), _
                            CInt(SpeCod), _
                            CInt(RazCod), _
                            AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                            "", "", objParametri)

        'Elimino gli oggetti COM
        objRazze = Nothing

        'Se il recordset non è chiuso allora ...	
        If Not Dt Is Nothing AndAlso Dt.Rows.Count > 0 Then

            Return Dt.Rows(0).Item("Raz_Des")

        End If

        'Elimino il recordset
        Dt = Nothing

    End Function

    '###############################################################################
    Public Function VerificaEsistenza_CodContatto_as_PivaGIAS(ByRef objParametri_Server As AgronicaCoreParametri, _
                                    ByRef objSession As System.Web.SessionState.HttpSessionState, _
                                    ByRef objPage As System.Web.UI.Page, _
                                    ByVal Cod_RisUm As Integer) As Boolean

        '----- Descrizione
        Dim DescrizioneFunzione As String = "VerificaEsistenza_CodContatto_AS_PivaGIAS"

        Dim objSQL As New AgronicaCoreDataProvider.DataProvider
        Dim StrSQL As String
        Dim MessaggioErrore As String
        Dim DT As DataTable

        'Genero la query SQL
        StrSQL = ""
        StrSQL += " SELECT Imprese.* "
        StrSQL += " FROM Imprese INNER JOIN Contatti ON Imprese.Piva = Contatti.Cod_Contatto   "
        StrSQL += " INNER JOIN Risorse_Umane ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto  "
        StrSQL += " AND Contatti.Piva = Risorse_Umane.Piva   "
        StrSQL += " INNER JOIN UtentiXImprese ON Imprese.Piva = UtentiXImprese.PIVA "
        StrSQL += " WHERE UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(CStr(objSession("ASG_SuperUser_CodFiscale"))) & "' "
        StrSQL += " AND Risorse_Umane.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & " "


        'Recupero il recordset
        DT = objSQL.EseguiQuery_Lettura( _
            objParametri_Server, _
            StrSQL, _
            DescrizioneFunzione _
        )

        'Verifico la presenza di errori
        If IsNothing(MessaggioErrore) Then

            'Verifico se il recordset e' aperto
            If DT.Rows.Count <> 0 Then

                Return True

            Else

                Return False

            End If

        Else

            Throw New Exception("Modulo AccessoVeloceDB : " & DescrizioneFunzione & " : " & MessaggioErrore)
            Return Nothing


        End If


    End Function


End Module
