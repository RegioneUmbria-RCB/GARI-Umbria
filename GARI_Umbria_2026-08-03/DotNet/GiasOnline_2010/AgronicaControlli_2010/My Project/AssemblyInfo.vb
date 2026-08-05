Imports System.Reflection
Imports System.Runtime.InteropServices

' Le informazioni generali relative a un assembly sono controllate dal seguente 
' insieme di attributi. Per modificare le informazioni associate a un assembly
' modificare i valori di tali attributi.

' Controllare i valori degli attributi degli assembly

<Assembly: AssemblyTitle("AgronicaControlli_2010")> 
<Assembly: AssemblyDescription("")> 
<Assembly: AssemblyCompany(".")> 
<Assembly: AssemblyProduct("AgronicaControlli_2010")> 
<Assembly: AssemblyCopyright("Copyright © . 2011")> 
<Assembly: AssemblyTrademark("")> 

<Assembly: ComVisible(False)>

'Se il progetto viene esposto a COM, il GUID seguente verrà utilizzato come ID della libreria dei tipi
<Assembly: Guid("c6bd3baf-c365-4f5f-ae60-3dfadde29f78")> 

' Le informazioni sulla versione di un assembly sono costituite dai seguenti quattro valori:
'
'      Numero di versione principale
'      Numero di versione secondario 
'      Numero build
'      Revisione
'
' È possibile specificare tutti i valori oppure impostare valori predefiniti per i numeri relativi alla revisione e alla build 
' utilizzando l'asterisco (*) come illustrato di seguito:

<Assembly: AssemblyVersion("1.0.0.0")> 
<Assembly: AssemblyFileVersion("1.0.0.0")> 
<assembly: WebResource("AgronicaControlli_2010.ClientControl1.js", "text/javascript")>
<assembly: ScriptResource("AgronicaControlli_2010.ClientControl1.js", _
   "AgronicaControlli_2010.ClientControl1", "AgronicaControlli_2010.Resource")>


'Agronica
<Assembly: WebResource("AgronicaControlli_2010.AgronicaBase.js", "text/javascript")>
<Assembly: WebResource("AgronicaControlli_2010.jAgroHelper.js", "text/javascript")>
<Assembly: WebResource("AgronicaControlli_2010.htmlComboMisuraXAvversitaInputData.htm", "text/html", PerformSubstitution:=True)>

'Watable
<Assembly: WebResource("AgronicaControlli_2010.waTableHelper.js", "text/javascript")>
<Assembly: WebResource("AgronicaControlli_2010.jquery.watable.js", "text/javascript")>

'jQuery
<Assembly: WebResource("AgronicaControlli_2010.jquery-1.11.1.min.js", "text/javascript")>
<Assembly: WebResource("AgronicaControlli_2010.jquery.min.1.11.2.js", "text/javascript")>
<Assembly: WebResource("AgronicaControlli_2010.jquery.min.1.12.3.js", "text/javascript")>
<Assembly: WebResource("AgronicaControlli_2010.jquery.min.3.5.1.js", "text/javascript")>

'Bootstrap
<Assembly: WebResource("AgronicaControlli_2010.bootstrap.min.js", "text/javascript")>
<Assembly: WebResource("AgronicaControlli_2010.bootstrap.5.3.3.min.js", "text/javascript")>
<Assembly: WebResource("AgronicaControlli_2010.bootstrap.4.5.2.min.js", "text/javascript")>
<Assembly: WebResource("AgronicaControlli_2010.bootstrap.3.3.2.min.js", "text/javascript")> 
<Assembly: WebResource("AgronicaControlli_2010.bootstrap.3.3.4.min.js", "text/javascript")>
<Assembly: WebResource("AgronicaControlli_2010.bootstrap.3.4.1.min.js", "text/javascript")>

<Assembly: WebResource("AgronicaControlli_2010.bootstrap.min.css", "text/css")>
<Assembly: WebResource("AgronicaControlli_2010.bootstrap.5.3.3.min.css", "text/css")>
<Assembly: WebResource("AgronicaControlli_2010.bootstrap.4.5.2.min.css", "text/css")>
<Assembly: WebResource("AgronicaControlli_2010.bootstrap.3.3.2.min.css", "text/css")>
<Assembly: WebResource("AgronicaControlli_2010.bootstrap.3.3.4.min.css", "text/css")>
<Assembly: WebResource("AgronicaControlli_2010.bootstrap.3.4.1.min.css", "text/css")>

'Bootstap addon
<Assembly: WebResource("AgronicaControlli_2010.bootstrap-datepicker.js", "text/javascript")> 
<Assembly: WebResource("AgronicaControlli_2010.bootstrap-datepicker._it.js", "text/javascript")> 
<Assembly: WebResource("AgronicaControlli_2010.bootstrap-select.js.map", "text/javascript")>
<Assembly: WebResource("AgronicaControlli_2010.bootstrap-select.js", "text/javascript")>
<Assembly: WebResource("AgronicaControlli_2010.bootstrap-select_1.12.4.js", "text/javascript")>
<Assembly: WebResource("AgronicaControlli_2010.bootstrap-WaitFrame.js", "text/javascript")>

'bootbox
<Assembly: WebResource("AgronicaControlli_2010.bootbox.min.js", "text/javascript")>

'cookie jquery
<Assembly: WebResource("AgronicaControlli_2010.jquery.cookie.js", "text/javascript")>

'jSonStat
<Assembly: WebResource("AgronicaControlli_2010.json-stat.js", "application/x-javascript")>

'visual
<Assembly: WebResource("AgronicaControlli_2010.visual.js", "application/x-javascript")> 
<Assembly: WebResource("AgronicaControlli_2010.visual.maps.js", "application/x-javascript")>
<Assembly: WebResource("AgronicaControlli_2010.visual_agronica.setup.js", "application/x-javascript")>

'lazyLoad
<Assembly: WebResource("AgronicaControlli_2010.lazyload.js", "application/x-javascript")>
<Assembly: WebResource("AgronicaControlli_2010.excanvas.js", "application/x-javascript")> 
<Assembly: WebResource("AgronicaControlli_2010.d3.v3.js", "application/x-javascript")> 

'Flot
<Assembly: WebResource("AgronicaControlli_2010.jquery.flot.js", "application/x-javascript")> 
<Assembly: WebResource("AgronicaControlli_2010.jquery.flot.categories.js", "application/x-javascript")> 
<Assembly: WebResource("AgronicaControlli_2010.jquery.flot.orderbars.js", "application/x-javascript")> 
<Assembly: WebResource("AgronicaControlli_2010.jquery.flot.pyramid.js", "application/x-javascript")>
<Assembly: WebResource("AgronicaControlli_2010.jquery.flot.stack.js", "application/x-javascript")>

<Assembly: WebResource("AgronicaControlli_2010.rotella.gif", "image/gif")>

'Pako (libreria per unzip di contenuti compressi lato js)
<Assembly: WebResource("AgronicaControlli_2010.pako.min.js", "text/javascript")>

'AgronicaMessaggi (centralizzazione Messaggi boostrap per masterpages)
<Assembly: WebResource("AgronicaControlli_2010.AgronicaMessaggi.js", "text/javascript")>

'Moment / Moment-Timezone (librerie per gestione fusi con Timezone)
<Assembly: WebResource("AgronicaControlli_2010.moment-timezone-utils.js", "text/javascript")>
<Assembly: WebResource("AgronicaControlli_2010.moment-timezone-with-data.js", "text/javascript")>
<Assembly: WebResource("AgronicaControlli_2010.moment-with-locales.js", "text/javascript")>
'<Assembly: WebResource("AgronicaControlli_2010.moment.js", "text/javascript")>
<Assembly: WebResource("AgronicaControlli_2010.AgroTimezone.js", "text/javascript")>
