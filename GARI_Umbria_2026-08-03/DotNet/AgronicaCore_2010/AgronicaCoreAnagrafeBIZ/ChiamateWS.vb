Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Imports System.Web.UI.WebControls
Imports AgronicaCoreDataProvider

Public Class ChiamateWS
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Shared Function IAF_Elenco(
                                ByRef objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                ByVal Disciplinare_Cod As Integer,
                                ByVal VEG_COD As Integer,
                                ByVal Flag_DisciplinareAttivo As Boolean) As DataTable


        Dim ObjDownloadWs As WS_Disciplinari.AgroWS_Disciplinari
        Dim Dati As String

        Dim XmlDocumento As New System.Xml.XmlDocument
        Dim XmlNodo As System.Xml.XmlNodeList
        Dim XmlElemento As System.Xml.XmlElement

        Dim Dt As New DataTable
        Dim Dr As DataRow

        Dt.Columns.Add(New DataColumn("IAF_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("IAF_Numero", GetType(String)))
        Dt.Columns.Add(New DataColumn("IAF_Descrizione", GetType(String)))
        Dt.Columns.Add(New DataColumn("IAF_Descrizione_Metodo", GetType(String)))

        'Verifico la possibilit di gestione dei disciplinari
        'If permessoDPI And (VEG_COD <> 0 Or Consultazione) Then
        If Flag_DisciplinareAttivo = True Then

            Try

                Dim agroWs As String
                Dim objConfigurazione_Siti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                agroWs = objConfigurazione_Siti.Leggi_Valore(0, "GiasOnline_WS_Disciplinari_AgroWS_Disciplinari", "", "", objParametri_Server)
                If agroWs = "" Then
                    agroWs = objConfigurazione_Siti.Leggi_Valore(0, "GiasOnline_WS_Disciplinari_AgroWS_Disciplinari", "", "", objParametri_Super_Server)
                End If

                ObjDownloadWs = New WS_Disciplinari.AgroWS_Disciplinari
                Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS
                objWs.NewWS(ObjDownloadWs, agroWs, objParametri_Utenti)

                Dim ASG_Utente_Username_Crypt = Sicurezza.Stringa_Codifica_LANCompatibile(objParametri_Server.SuperUserUsername, CostantiPersonalizzate.AgroKey_EncoderDecoder)
                Dim xletturautente As New AgronicaCoreUtentiDAL.Utenti_Read
                Dim pass As String = xletturautente.Password_From_UserName(objParametri_Server.SuperUserUsername, objParametri_Utenti)
                Dim ASG_Utente_Password_Crypt = Sicurezza.Stringa_Codifica_LANCompatibile(pass, AgroKey_EncoderDecoder)

                Dati = ObjDownloadWs.Leggi_IAF(Disciplinare_Cod,
                                                VEG_COD,
                                                objParametri_Server.FinestraTemporaleInizio,
                                                objParametri_Server.FinestraTemporaleFine,
                                                ASG_Utente_Username_Crypt,
                                                ASG_Utente_Password_Crypt)


                If Not IsNothing(Dati) AndAlso Dati.ToLower.IndexOf("errore") < 0 Then
                    XmlDocumento.LoadXml(Dati)
                    XmlNodo = XmlDocumento.GetElementsByTagName("Record")
                    If Not XmlNodo Is Nothing Then
                        For Each XmlElemento In XmlNodo
                            Dr = Dt.NewRow
                            Dr.Item("IAF_Cod") = XmlElemento.GetAttribute("iaf_cod")
                            Dr.Item("IAF_Numero") = XmlElemento.GetAttribute("iaf_numero")
                            Dr.Item("IAF_Descrizione") = XmlElemento.GetAttribute("iaf_descrizione")
                            Dr.Item("IAF_Descrizione_Metodo") = XmlElemento.GetAttribute("iaf_descrizione_metodo")
                            Dt.Rows.Add(Dr)
                        Next
                    End If
                End If

                If Not IsNothing(Dt) AndAlso Dt.Rows.Count > 0 Then

                    Dim Dv As New DataView
                    Dt.TableName = "IAF"
                    Dv.Table = Dt
                    Dv.Sort = "IAF_Numero, IAF_Descrizione, IAF_Descrizione_Metodo"


                    Return Dv.ToTable

                End If


            Catch ex As Exception

                Dati = "Si sono verificati errori in fase di chiamata al WebService Discliplinari!" & Chr(13) & ex.Message

            End Try

        End If

    End Function

End Class
