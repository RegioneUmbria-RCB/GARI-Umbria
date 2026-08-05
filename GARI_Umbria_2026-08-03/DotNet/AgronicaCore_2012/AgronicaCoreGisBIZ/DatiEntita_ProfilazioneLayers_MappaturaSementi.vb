
Imports <xmlns="http://www.agronica.it/grafica/">

Imports AgronicaGIS2012.Commons
Imports AgronicaCoreDataProvider
Imports System.Text

Public Class DatiEntita_ProfilazioneLayers_MappaturaSementi
    Inherits DatiEntita_ProfilazioneLayers

    Public Function InserisciLayers(ByVal xmlDatiEntita As String, objParametri As AgronicaCoreParametri) As String

        Dim xmlFinaleElaborato As XDocument = XDocument.Parse(xmlDatiEntita)
        Dim sXmlRval As String = ""

        Dim LeggiImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dim LeggiImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
        Dim leggiGerarchia As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R

        Dim piva As String
        Dim sa_cod As Integer
        Dim appezza As Integer
        Dim reg_impianto As Integer

        Dim veg_cod As Integer

        Dim DTImpianti As DataTable
        Dim DTGerarchia As DataTable

        For Each nodoEntita In ( _
            From e In xmlFinaleElaborato.<DatiEntita>.<Entita> _
            Select e).ToList

            'Profila impresa di appartenenza

            'Legge il padre
            piva = nodoEntita.<EntitaGIAS>.<DatoGias>.<Piva>.Value

            Dim pivaPadre As String = ""
            DTGerarchia = leggiGerarchia.LeggixFiglio( _
                piva, _
                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
                "", _
                "", _
                objParametri _
            )


            If DTGerarchia.Rows.Count <> 0 Then
                pivaPadre = DTGerarchia.Rows(0)("Padre")
            End If

            Dim layerPivaPadre = <layer tipologia_layer="100"><%= pivaPadre %></layer>
            nodoEntita.<layers>.FirstOrDefault.Add(layerPivaPadre)




            'Dati impianti
            sa_cod = nodoEntita.<EntitaGIAS>.<DatoGias>.<Sa_cod>.Value
            appezza = nodoEntita.<EntitaGIAS>.<DatoGias>.<Appezza>.Value
            reg_impianto = nodoEntita.<EntitaGIAS>.<DatoGias>.<Id_Imp>.Value

            Dim stb As New StringBuilder

            stb.Append("select top 1 veg_cod " & vbCrLf)
            stb.Append(" from specievegetali  " & vbCrLf)
            stb.Append(" where veg_cod in ( " & vbCrLf)
            stb.Append("    select Veg_Cod  " & vbCrLf)
            stb.Append("    from Cultivar  " & vbCrLf)
            stb.Append("    where Cul_Cod in ( " & vbCrLf)
            stb.Append("        select cul_cod " & vbCrLf)
            stb.Append("        from reg_impianti " & vbCrLf)
            stb.Append("        where 1=1 " & vbCrLf)
            stb.Append("        AND     Piva = '" & piva & "'   " & vbCrLf)
            stb.Append("        AND Sa_Cod = " & sa_cod & "   " & vbCrLf)
            stb.Append("        AND Appezza = " & appezza & "   " & vbCrLf)
            stb.Append("        AND Id_Reg = " & reg_impianto & "    " & vbCrLf)
            stb.Append("    ) " & vbCrLf)
            stb.Append(" ) " & vbCrLf)

            Dim appRLeggi As New AgronicaCoreDataProvider.DataProvider
            Dim dt As New DataTable
            dt = appRLeggi.EseguiQuery_Lettura(objParametri, _
                    stb.ToString, _
                    "" _
                )

            If dt.Rows.Count > 0 Then
                veg_cod = dt(0)(0)
            Else
                veg_cod = "-1"
            End If

            Dim layerSpecie = <layer tipologia_layer="5"><%= veg_cod %></layer>
            Dim layerImpianto = <layer tipologia_layer="1">13</layer>

            nodoEntita.<layers>.FirstOrDefault.Add(layerSpecie)
            nodoEntita.<layers>.FirstOrDefault.Add(layerImpianto)

        Next


        Dim listaNS As New List(Of String)
        listaNS.Add("http://www.opengis.net/gml")


        sXmlRval = xmlHelper.RemoveNamespace(xmlFinaleElaborato, listaNS).ToString.Replace("xmlns=""""", "")

        Return sXmlRval



    End Function
End Class
