
Imports System.Xml
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

Imports AgronicaCoreDataProvider

Imports System.Text
Imports AgronicaCoreContabDAL
Imports AgronicaCoreAnagrafeDAL


Partial Public Class Funzioni
    Public Sub Elabora_Vasche_Salva( _
            ByVal objOpzioni As clsOpzioni, _
            ByRef Log_Import As StringBuilder, _
            ByRef Log_Errori As StringBuilder, _
            ByRef Log_Riepilogo As StringBuilder, _
            ByVal Piva_Origine As String, _
            ByVal Piva_Destinazione As String, _
            ByVal sa_cod_Origine As Integer, _
            ByVal sa_cod_Destinazione As Integer _
        )

        Const nomeFunzione As String = "Elabora_Vasche_Salva"

        Dim dtCantina_Caratteristiche As DataTable
        Dim dtCantina_Masse_Volumiche As DataTable
        Dim dtCantina_Pareti As DataTable
        Dim dtCantina_Vasche As DataTable

        Dim leggiCantina_Caratteristiche As New Cantina_Caratter_R
        Dim leggiCantina_Masse_Volumiche As New Cantina_Masse_Volumiche_R
        Dim leggiCantina_Pareti As New Cantina_Pareti_R
        Dim leggiCantina_Vasche As New Cantina_Vasche_R

        Dim scriviCantina_Caratteristiche As New Cantina_Caratter_W
        Dim scriviCantina_Masse_Volumiche As New Cantina_Masse_Volumiche_W
        Dim scriviCantina_Pareti As New Cantina_Pareti_W
        Dim scriviCantina_Vasche As New Cantina_Vasche_W

        dtCantina_Caratteristiche = leggiCantina_Caratteristiche.Leggi(Piva_Origine, sa_cod_Origine, 0, AGRODATAINIZIO, AGRODATAFINE, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objOpzioni.objParametri_Server_GIAS_ORIGINE)
        dtCantina_Masse_Volumiche = leggiCantina_Masse_Volumiche.Leggi(Piva_Origine, "", "", objOpzioni.objParametri_Server_GIAS_ORIGINE)
        dtCantina_Pareti = leggiCantina_Pareti.Leggi(Piva_Origine, sa_cod_Origine, 0, 0, AGRODATAINIZIO, AGRODATAFINE, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objOpzioni.objParametri_Server_GIAS_ORIGINE)
        dtCantina_Vasche = leggiCantina_Vasche.Leggi(Piva_Origine, sa_cod_Origine, 0, "", "", objOpzioni.objParametri_Server_GIAS_ORIGINE)


        Try

            For Each iCurrdtCantina_Caratteristiche In dtCantina_Caratteristiche.Rows
                scriviCantina_Caratteristiche.Scrivi( _
                      Piva_Destinazione _
                    , sa_cod_Destinazione _
                    , iCurrdtCantina_Caratteristiche("Piano_Cod") _
                    , iCurrdtCantina_Caratteristiche("Piano_Des") _
                    , iCurrdtCantina_Caratteristiche("DimX") _
                    , iCurrdtCantina_Caratteristiche("DimY") _
                    , iCurrdtCantina_Caratteristiche("Colore_Interno") _
                    , iCurrdtCantina_Caratteristiche("Colore_Esterno") _
                    , iCurrdtCantina_Caratteristiche("Spessore") _
                    , iCurrdtCantina_Caratteristiche("Riempimento") _
                    , iCurrdtCantina_Caratteristiche("Zoom") _
                    , iCurrdtCantina_Caratteristiche("username_creazione") _
                    , iCurrdtCantina_Caratteristiche("validita_inizio") _
                    , iCurrdtCantina_Caratteristiche("validita_fine") _
                    , objOpzioni.objParametri_Server_GIAS_DESTINAZIONE _
                    , iCurrdtCantina_Caratteristiche("data_creazione") _
                    , iCurrdtCantina_Caratteristiche("data_modifica") _
                    , iCurrdtCantina_Caratteristiche("username_modifica") _
                )

            Next

            For Each iCurrdtCantina_Masse_Volumiche In dtCantina_Masse_Volumiche.Rows
                scriviCantina_Masse_Volumiche.Scrivi( _
                      iCurrdtCantina_Masse_Volumiche("id_massa") _
                    , Piva_Destinazione _
                    , iCurrdtCantina_Masse_Volumiche("anno") _
                    , iCurrdtCantina_Masse_Volumiche("titolo") _
                    , iCurrdtCantina_Masse_Volumiche("massa") _
                    , iCurrdtCantina_Masse_Volumiche("validita_inizio") _
                    , iCurrdtCantina_Masse_Volumiche("validita_fine") _
                    , objOpzioni.objParametri_Server_GIAS_DESTINAZIONE _
                    , iCurrdtCantina_Masse_Volumiche("data_creazione") _
                    , iCurrdtCantina_Masse_Volumiche("data_modifica") _
                    , iCurrdtCantina_Masse_Volumiche("username_creazione") _
                    , iCurrdtCantina_Masse_Volumiche("username_modifica") _
                )

            Next

            For Each iCurrdtCantina_Pareti In dtCantina_Pareti.Rows
                scriviCantina_Pareti.Scrivi( _
                       iCurrdtCantina_Pareti("PIVA") _
                    , iCurrdtCantina_Pareti("sa_cod") _
                    , iCurrdtCantina_Pareti("Piano_Cod") _
                    , iCurrdtCantina_Pareti("Parete_Cod") _
                    , iCurrdtCantina_Pareti("DimX") _
                    , iCurrdtCantina_Pareti("DimY") _
                    , iCurrdtCantina_Pareti("Colore_Interno") _
                    , iCurrdtCantina_Pareti("Colore_Esterno") _
                    , iCurrdtCantina_Pareti("PosX") _
                    , iCurrdtCantina_Pareti("PosY") _
                    , iCurrdtCantina_Pareti("Spessore") _
                    , iCurrdtCantina_Pareti("Riempimento") _
                    , iCurrdtCantina_Pareti("username_creazione") _
                    , iCurrdtCantina_Pareti("validita_inizio") _
                    , iCurrdtCantina_Pareti("validita_fine") _
                    , objOpzioni.objParametri_Server_GIAS_DESTINAZIONE _
                    , iCurrdtCantina_Pareti("Data_Creazione") _
                    , iCurrdtCantina_Pareti("Data_Modifica") _
                    , iCurrdtCantina_Pareti("Username_Modifica") _
                )


            Next

            For Each iCurrdtCantina_Vasche In dtCantina_Vasche.Rows
                scriviCantina_Vasche.Scrivi( _
                            Piva_Destinazione _
                        , sa_cod_Destinazione _
                        , iCurrdtCantina_Vasche("Vas_Cod") _
                        , iCurrdtCantina_Vasche("Piano_Cod") _
                        , iCurrdtCantina_Vasche("Identificativo") _
                        , iCurrdtCantina_Vasche("Numero_Serie") _
                        , iCurrdtCantina_Vasche("Materiale_Cod") _
                        , iCurrdtCantina_Vasche("Appoggio_Cod") _
                        , iCurrdtCantina_Vasche("Inclinato") _
                        , iCurrdtCantina_Vasche("Tipo_Tasca") _
                        , iCurrdtCantina_Vasche("Coibentata") _
                        , iCurrdtCantina_Vasche("Udm_Cod_Capacita") _
                        , iCurrdtCantina_Vasche("Capacita_Nominale") _
                        , iCurrdtCantina_Vasche("Capacita_Effettiva") _
                        , iCurrdtCantina_Vasche("Udm_Cod_Altezza") _
                        , iCurrdtCantina_Vasche("Altezza_Cilindro") _
                        , iCurrdtCantina_Vasche("Altezza_Totale") _
                        , iCurrdtCantina_Vasche("Udm_Cod_Peso") _
                        , iCurrdtCantina_Vasche("Peso") _
                        , iCurrdtCantina_Vasche("Note") _
                        , iCurrdtCantina_Vasche("DimX") _
                        , iCurrdtCantina_Vasche("DimY") _
                        , iCurrdtCantina_Vasche("Rotazione") _
                        , iCurrdtCantina_Vasche("Costo_Acquisto") _
                        , iCurrdtCantina_Vasche("Modello") _
                        , iCurrdtCantina_Vasche("Ammortamento") _
                        , iCurrdtCantina_Vasche("Ultima_Revisione") _
                        , iCurrdtCantina_Vasche("Tipo") _
                        , iCurrdtCantina_Vasche("PosX") _
                        , iCurrdtCantina_Vasche("PosY") _
                        , iCurrdtCantina_Vasche("Spessore") _
                        , iCurrdtCantina_Vasche("Refrigerata") _
                        , iCurrdtCantina_Vasche("Colore_Esterno") _
                        , iCurrdtCantina_Vasche("Tipo_Serbatoio") _
                        , iCurrdtCantina_Vasche("validita_inizio") _
                        , iCurrdtCantina_Vasche("validita_fine") _
                        , objOpzioni.objParametri_Server_GIAS_DESTINAZIONE _
                        , iCurrdtCantina_Vasche("data_creazione") _
                        , iCurrdtCantina_Vasche("data_modifica") _
                        , iCurrdtCantina_Vasche("username_creazione") _
                        , iCurrdtCantina_Vasche("username_modifica") _
                )

            Next



        Catch ex As Exception

            Dim msg As String
            msg = CStr(Date.Now) + " - " + nomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)

        End Try


    End Sub

End Class
