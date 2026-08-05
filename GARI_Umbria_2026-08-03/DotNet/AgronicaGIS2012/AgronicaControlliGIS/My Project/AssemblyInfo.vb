Imports System
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Web.UI
Imports System.Runtime.CompilerServices

' Le informazioni generali relative a un assembly sono controllate dal seguente 
' insieme di attributi. Per modificare le informazioni associate a un assembly
' è necessario modificare i valori di questi attributi.

' Controllare i valori degli attributi dell'assembly

<Assembly: AssemblyTitle("AgronicaControlliGIS")> 
<Assembly: AssemblyDescription("")> 
<Assembly: AssemblyConfiguration("")> 
<Assembly: AssemblyCompany("")> 
<Assembly: AssemblyProduct("AgronicaControlliGIS")> 
<Assembly: AssemblyCopyright("Copyright © 2012")> 
<Assembly: AssemblyTrademark("")> 
<Assembly: AssemblyCulture("")> 

<Assembly: ComVisible(False)> 

'Se il progetto viene esposto a COM, il GUID seguente verrà utilizzato come ID della libreria dei tipi
<Assembly: Guid("63c1aa76-a201-494d-9eaa-be5415be3cb0")> 

' Le informazioni sulla versione di un assembly sono costituite dai seguenti quattro valori:
'
'      Numero di versione principale
'      Numero di versione secondario 
'      Numero build
'      Revisione
'
' È possibile specificare tutti i valori oppure impostare valori predefiniti per i numeri relativi alla revisione e alla build 
' utilizzando l'asterisco (*) come descritto di seguito:
' <Assembly: AssemblyVersion("1.0.*")> 

<Assembly: AssemblyVersion("1.0.0.0")> 
<Assembly: AssemblyFileVersion("1.0.0.0")> 


'CLIENTJS
<Assembly: WebResource("AgronicaControlliGIS.Colori.js", "text/javascript")> 
<Assembly: WebResource("AgronicaControlliGIS.Costanti_Gis.js", "text/javascript")> 
<Assembly: WebResource("AgronicaControlliGIS.Enumerativi_Gis.js", "text/javascript")> 
<Assembly: WebResource("AgronicaControlliGIS.Numeri.js", "text/javascript")> 
<Assembly: WebResource("AgronicaControlliGIS.Stringhe.js", "text/javascript")> 
<Assembly: WebResource("AgronicaControlliGIS.Utility.js", "text/javascript")> 
<Assembly: WebResource("AgronicaControlliGIS.Vettori.js", "text/javascript")> 
<Assembly: WebResource("AgronicaControlliGIS.Mappa.js", "text/javascript")> 
<Assembly: WebResource("AgronicaControlliGIS.gMapsUtility.js", "text/javascript")> 
<Assembly: WebResource("AgronicaControlliGIS.precision.js", "text/javascript")> 
<Assembly: WebResource("AgronicaControlliGIS.Shape.js", "text/javascript")> 
<Assembly: WebResource("AgronicaControlliGIS.Interfaccia.js", "text/javascript")> 

<Assembly: WebResource("AgronicaControlliGIS.jquery.linq.js", "text/javascript")> 


<Assembly: WebResource("AgronicaControlliGIS.scriptMaps_DI_PARTENZA.js", "text/javascript")> 






<Assembly: WebResource("AgronicaControlliGIS.labelsruler.js", "text/javascript")> 
<Assembly: WebResource("AgronicaControlliGIS.Overlay.js", "text/javascript")> 
<Assembly: WebResource("AgronicaControlliGIS.ruler.js", "text/javascript")> 
<Assembly: WebResource("AgronicaControlliGIS.js_demo_tiles.js", "text/javascript")> 





<Assembly: WebResource("AgronicaControlliGIS.labels.js", "text/javascript")>




<Assembly: WebResource("AgronicaControlliGIS.mappa.htm", "text/html", PerformSubstitution:=True)>
<Assembly: WebResource("AgronicaControlliGIS.mappaBS.htm", "text/html", PerformSubstitution:=True)>
<Assembly: WebResource("AgronicaControlliGIS.GisSmartBS.htm", "text/html", PerformSubstitution:=True)>

<Assembly: WebResource("AgronicaControlliGIS.css_mappa.css", "text/css")>
<Assembly: WebResource("AgronicaControlliGIS.css_mappaBS.css", "text/css")>
<Assembly: WebResource("AgronicaControlliGIS.css_GisSmartBS.css", "text/css")>


<Assembly: WebResource("AgronicaControlliGIS.jquery.ajax_upload.0.6.js", "text/javascript")> 

 


'mmagini
<Assembly: WebResource("AgronicaControlliGIS.meno.png", "image/png")> 
<Assembly: WebResource("AgronicaControlliGIS.piu.png", "image/png")> 
<Assembly: WebResource("AgronicaControlliGIS.refresh.png", "image/png")> 
<Assembly: WebResource("AgronicaControlliGIS.ruler.png", "image/png")> 
<Assembly: WebResource("AgronicaControlliGIS.save.png", "image/png")> 
<Assembly: WebResource("AgronicaControlliGIS.trash.png", "image/png")> 

<Assembly: WebResource("AgronicaControlliGIS.import.png", "image/png")> 
<Assembly: WebResource("AgronicaControlliGIS.import - Info.png", "image/png")> 
<Assembly: WebResource("AgronicaControlliGIS.export.png", "image/png")> 

<Assembly: WebResource("AgronicaControlliGIS.Stampa32.png", "image/png")> 
<Assembly: WebResource("AgronicaControlliGIS.select.png", "image/png")> 
<Assembly: WebResource("AgronicaControlliGIS.draw.png", "image/png")> 
<Assembly: WebResource("AgronicaControlliGIS.info.png", "image/png")> 
<Assembly: WebResource("AgronicaControlliGIS.ruler-info.png", "image/png")> 
<Assembly: WebResource("AgronicaControlliGIS.Grafica - Layer32.png", "image/ico")> 
<Assembly: WebResource("AgronicaControlliGIS.Agenda.png", "image/ico")> 
<Assembly: WebResource("AgronicaControlliGIS.Grafica - Misura.png", "image/ico")> 
<Assembly: WebResource("AgronicaControlliGIS.Doc4.png", "image/ico")> 
<Assembly: WebResource("AgronicaControlliGIS.Doc5.png", "image/ico")> 
<Assembly: WebResource("AgronicaControlliGIS.trattore.png", "image/png")> 

<Assembly: WebResource("AgronicaControlliGIS.gestisci_layer.png", "image/png")> 
<Assembly: WebResource("AgronicaControlliGIS.edit.png", "image/png")> 

<Assembly: WebResource("AgronicaControlliGIS.x04_GenericoLayer_16.png", "image/png")> 
<Assembly: WebResource("AgronicaControlliGIS.x04_GenericoLayer_32.png", "image/png")> 
<Assembly: WebResource("AgronicaControlliGIS.x05_Appezzamento_32.png", "image/png")> 
<Assembly: WebResource("AgronicaControlliGIS.x05_Appezzamento_16.png", "image/png")> 

<Assembly: WebResource("AgronicaControlliGIS.Ritaglio32.png", "image/png")> 

<Assembly: WebResource("AgronicaControlliGIS.Grafica - EditPuntiGPS.png", "image/png")> 

<Assembly: WebResource("AgronicaControlliGIS.Terreni_Variazione.png", "image/png")> 

<Assembly: WebResource("AgronicaControlliGIS.planning.png", "image/png")> 
<Assembly: WebResource("AgronicaControlliGIS.SelectPerSpecie.png", "image/png")> 


<Assembly: WebResource("AgronicaControlliGIS.gps.png", "image/png")> 

<Assembly: WebResource("AgronicaControlliGIS.marker.png", "image/png")> 

<Assembly: WebResource("AgronicaControlliGIS.ValidazioneNO_24.png", "image/png")> 
<Assembly: WebResource("AgronicaControlliGIS.ValidazioneSI_24.png", "image/png")> 

<Assembly: WebResource("AgronicaControlliGIS.BufferZone32.png", "image/png")> 
<Assembly: WebResource("AgronicaControlliGIS.BufferZone24.png", "image/png")> 
<Assembly: WebResource("AgronicaControlliGIS.BufferZone16.png", "image/png")>
<Assembly: WebResource("AgronicaControlliGIS.meteo.png", "image/png")>
<Assembly: WebResource("AgronicaControlliGIS.BloccoNote32.ico", "image/png")>
<Assembly: WebResource("AgronicaControlliGIS.Ingredienti.ico", "image/ico")>
<Assembly: WebResource("AgronicaControlliGIS.Ingredienti16.png", "image/png")>
<Assembly: WebResource("AgronicaControlliGIS.Insetto32.png", "image/png")>
<Assembly: WebResource("AgronicaControlliGIS.papiro.png", "image/png")>