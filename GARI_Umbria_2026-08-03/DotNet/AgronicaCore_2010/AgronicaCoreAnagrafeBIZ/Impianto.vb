Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModelsSTD.anagrafiche

Public Class Impianto_R

    Public Sub Descrizioni_From_Piva_Sa_Cod_Appezza_Id_Reg(Piva As String,
                                                           Sa_Cod As Integer,
                                                           Appezza As Integer,
                                                           Id_Reg As Integer,
                                                           ByRef CulDes As String,
                                                           ByRef VegDes As String,
                                                           objParametri_Server As AgronicaCoreParametri)

        AgronicaCoreAnagrafeDAL.Reg_Impianti_Read.Descrizioni_From_Piva_Sa_Cod_Appezza_Id_Reg(Piva, Sa_Cod, Appezza, Id_Reg, "", "", "", "", CulDes, "", VegDes, objParametri_Server)
    End Sub

    Public Shared Function VerificaEsistenzaImpiantoDaCodiceImpianto(ByVal piva As String,
                                                                     ByVal sa_cod As Integer,
                                                                     ByVal appezza As Integer,
                                                                     ByVal ImpiantoCodice As String,
                                                                     ByRef objParametri_Server As AgronicaCoreParametri
                                                                     ) As AgronicaCoreModelsSTD.anagrafiche.Impianto.PK

        Dim ret As AgronicaCoreModelsSTD.anagrafiche.Impianto.PK = Nothing
        Dim xImpCodR As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R

        Try

            Dim r = xImpCodR.Leggi(piva, sa_cod, appezza, 0, "", enum_CodiciAnagrafe.Codice_Impianto, ImpiantoCodice, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
            If r.Rows.Count > 0 Then
                ret = New AgronicaCoreModelsSTD.anagrafiche.Impianto.PK
                ret.codice = r.Rows(0)("id_reg")
                ret.appezzamentoPK = New Appezzamento.PK(If(appezza = 0, r.Rows(0)("appezza"), appezza), New CentroAziendale.PK(sa_cod, piva))
            End If

        Catch ex As Exception
            ret = Nothing
            Throw New Exception(ex.Message, ex)
        End Try
 
        Return ret
 
    End Function

    ''' <summary>
    ''' Legge i dati di un impianto date le sue chiavi, richiamando il DAL Leggi_SpecieVarieta
    ''' con la selezione TabellaDatiMinimi.
    ''' </summary>
    ''' <param name="Piva">Partita IVA dell'azienda.</param>
    ''' <param name="saCod">Codice centro aziendale.</param>
    ''' <param name="Appezza">Codice appezzamento.</param>
    ''' <param name="Id_Reg">ID registrazione impianto.</param>
    ''' <param name="objParametri_Server">Parametri di connessione al database.</param>
    ''' <returns>
    ''' <see cref="AgronicaCoreDTOStd.OutData.Gis.Impianti.DatiImpianto_Out"/> con i dati
    ''' dell'impianto, oppure Nothing se nessun record corrisponde alle chiavi fornite.
    ''' </returns>
    Public Shared Function Leggi_SpecieVarieta(ByVal piva As String,
                                               ByVal saCod As Integer,
                                               ByVal appezza As Integer,
                                               ByVal idImp As Integer,
                                               ByRef objParametriServer As AgronicaCoreParametri
                                               ) As AgronicaCoreDTOStd.OutData.Gis.Impianti.DatiImpianto_Out

        Dim dal As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dim dt = dal.Leggi_SpecieVarieta(piva,
                                         saCod,
                                         appezza,
                                         idImp,
                                         Veg_Cod:=0,
                                         Cul_Cod:=0,
                                         xSelezioneVariabile:=enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                         xFiltroAggiuntivo:="",
                                         xOrderBy:="",
                                         objParametri:=objParametriServer)

        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            Return Nothing
        End If

        Dim row = dt.Rows(0)
        Return New AgronicaCoreDTOStd.OutData.Gis.Impianti.DatiImpianto_Out With {
            .Piva = CStr(row("Piva")),
            .SaCod = CInt(row("sa_Cod")),
            .Appezza = CInt(row("Appezza")),
            .IdImp = CInt(row("id_reg")),
            .CulCod = CInt(row("Cul_Cod")),
            .VegCod = CInt(row("Veg_Cod")),
            .ValiditaInizio = CDate(row("Validita_Inizio")),
            .ValiditaFine = CDate(row("Validita_Fine"))
        }

    End Function

    Public Shared Function VerificaEsistenzaImpiantoDaCodiceAnagrafe(ByVal piva As String,
                                                                     ByVal sa_cod As Integer,
                                                                     ByVal appezza As Integer,
                                                                     ByVal CodiceAnagrafe As String,
                                                                     ByVal ImpiantoCodice As String,
                                                                     ByRef objParametri_Server As AgronicaCoreParametri
                                                                     ) As AgronicaCoreModelsSTD.anagrafiche.Impianto.PK

        Dim ret As AgronicaCoreModelsSTD.anagrafiche.Impianto.PK = Nothing
        Dim xImpCodR As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R

        Try

            Dim r = xImpCodR.Leggi(piva, sa_cod, appezza, 0, "", CodiceAnagrafe, ImpiantoCodice, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
            If r.Rows.Count > 0 Then
                ret = New AgronicaCoreModelsSTD.anagrafiche.Impianto.PK
                ret.codice = r.Rows(0)("id_reg")
                ret.appezzamentoPK = New Appezzamento.PK(If(appezza = 0, r.Rows(0)("appezza"), appezza), New CentroAziendale.PK(sa_cod, piva))
            End If

        Catch ex As Exception
            ret = Nothing
            Throw New Exception(ex.Message, ex)
        End Try

        Return ret

    End Function

End Class
