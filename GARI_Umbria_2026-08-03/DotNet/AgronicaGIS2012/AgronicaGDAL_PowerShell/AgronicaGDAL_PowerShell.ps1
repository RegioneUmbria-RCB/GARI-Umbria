<#
.SYNOPSIS
    .
.DESCRIPTION
    Elaborazione delle immagini sentinel2
.PARAMETER CartellaDaProcessare
    Percorso Assoluto della file Zip contentente la Cartella Da Processare
.PARAMETER TilesDaGenerare
    Livelli di zoom per i tiles da generare
.PARAMETER LivelliZoom
    Livelli di zoom per i tiles da generare
.PARAMETER Algoritmi
    Algoritmi da calcolare sui raster
.PARAMETER CoordinateRitaglio
	Coordinate per costruire un ritaglio della mappa.
.PARAMETER LivelliZoomRitaglio
	Livello di zoom per ritaglio
.PARAMETER CodiceElaborazione
	Codice di elaborazione per riscontro su Database
.PARAMETER DebugVerbosityLevel
	0 - solo erorri bloccanti, 1 - warning, 2 - approfondito
.PARAMETER CartellaBackupZipFile
	Cartella dove verrà copiato il file zip
.EXAMPLE
    C:\PS>AgronicaGDAL_PowerShell.ps1 -CartellaDaProcessare C:\www_cartografia\dati\Sentinel2\S2A_MSIL1C_20180708T101031_N0206_R022_T32TQQ_20180708T122246 -TilesDaGenerare RGB,NDVI -LivelliZoom 3-14 -Algoritmi NDVI
    Elabora la cartella con i tiles indicati: RGB, NDVI, per i livelli di Zoom da 3 a 14, produce un file TIFF con il calcolo NDVI
.NOTES
    Author: Vanni Costa
    Date:   18 Ottobre 2018
#>


#param (
#    [string]$CartellaDaProcessare,
#    [Parameter(Mandatory=$true)][string]$Algoritmi,
#    [string]$password = $( Read-Host "Input password, please" )
# )

 param (	
	[switch]$h = $false,
	[string]$CartellaDaProcessare,    
	[string]$TilesDaGenerare="",
	[string]$LivelliZoom="3-14",
    [string]$Algoritmi="",	
    [string]$CoordinateRitaglio="",	
	[string]$LivelliZoomRitaglio="15-17",
    [string]$CodiceElaborazione="0",
	[string]$DebugVerbosityLevel="0",
	[string]$CartellaBackupZipFile=""		 
 )


Set-Item -Path Env:OSGEO4W_ROOT -value("C:\PROGRA~1\QGIS3~1.2")
Set-Item -Path Env:GDAL_DATA -value("$Env:OSGEO4W_ROOT\share\gdal")
Set-Item -Path Env:Path -Value ("$Env:OSGEO4W_ROOT\bin;$Env:WINDIR\system32;$Env:WINDIR;$Env:WINDIR\system32\WBem")



if ($h) {
	Write-Error "digitare Get-help AgronicaGDAL_PowerShell.ps1 per la guida.. esempio di utilizzo: C:\>AgronicaGDAL_PowerShell.ps1 -CartellaDaProcessare c:\dati\sentinel\S2A_MSIL1C_20180708T101031_N0206_R022_T32TQQ_20180708T122246 -TilesDaGenerare RGB,NDVI -LivelliZoom 3-14 -Algoritmi NDVI"
	exit
}

if ($CartellaDaProcessare -eq "") {
	Write-Error "ERRORE, Nessuna Cartella Specificata."
	exit
}

$ZipTest = $CartellaDaProcessare + ".zip"
echo $ZipTest

if ((!(Test-Path -Path $ZipTest -PathType Leaf ) -and ($CoordinateRitaglio) -eq "" )) {
	Write-Error "ERRORE, File Zip non trovato."
	exit
}

Add-Type -AssemblyName System.IO.Compression.FileSystem
function Unzip
{
    param ( 
		[string]$zipfile, 
		[string]$outpath
	)

    [System.IO.Compression.ZipFile]::ExtractToDirectory($zipfile, $outpath)
}


function GenerazioneTilesRitaglio {
	param  (
		[string]$BaseDirPath,		
		[string]$BaseParentDirPath,		
		[string]$PercorsoCartellaTempLavoro,		
		[string]$LivelliZoomRitaglio,
		[string]$Suffisso,
		[string]$CoordinateRitaglio
	)

	

	echo "Generazione File temporaneo di ritaglio"
	echo $CoordinateRitaglio

	$PercorsoCartellaTiles_Suffisso = $BaseDirPath + "\" + $Suffisso
	$Suffisso_outputFile = $BaseDirPath + "\" + $Suffisso + ".tif"
	$FileTemporaneoDiRitaglio = $PercorsoCartellaTempLavoro + "\FileTemporaneoDiRitaglio_" + $Suffisso + ".tif"
	$FileTemporaneoDiRitaglioRiproiettato = $PercorsoCartellaTempLavoro + "\FileTemporaneoDiRitaglioPrj_" + $Suffisso + ".vrt"
	$FileTemporaneoDiRitaglioColorato = $PercorsoCartellaTempLavoro + "\FileTemporaneoDiRitaglioColore_" + $Suffisso + ".tif"		
		
	$ArrDirCoordinateRitaglio = $CoordinateRitaglio.split(' ')
	$ulx =$ArrDirCoordinateRitaglio[0]
	$uly =$ArrDirCoordinateRitaglio[1]
	$lrx =$ArrDirCoordinateRitaglio[2]
	$lry =$ArrDirCoordinateRitaglio[3]

	#versione precedente con ritaglio su UTM
	#gdal_translate -projwin $ulx $uly $lrx $lry -of GTiff $Suffisso_outputFile $FileTemporaneoDiRitaglio

	#Proiezione su pseudo mercator dal sistema di riferimento letto in una variabile e generazione del file "vrt" EPSG:32632 "+proj=utm +zone=22 +south +datum=WGS84 +units=m +no_defs"
	$sistemaRiferimentoOrigine = gdalsrsinfo -o proj4 $Suffisso_outputFile
	$sistemaRiferimentoOrigine = $sistemaRiferimentoOrigine -replace "'", ""
	$echoSistemaRiferimentoOrigine = "Sistema di riferimento del file origine letto: " + $sistemaRiferimentoOrigine
	echo $echoSistemaRiferimentoOrigine
	gdalwarp -s_srs $sistemaRiferimentoOrigine -t_srs EPSG:3857 -of vrt $Suffisso_outputFile $FileTemporaneoDiRitaglioRiproiettato
	
	#ritaglio su pseudo mercator
	gdal_translate -projwin $ulx $uly $lrx $lry -of GTiff $FileTemporaneoDiRitaglioRiproiettato $FileTemporaneoDiRitaglio


	$grass_color_text_file = $BaseParentDirPath + "\Grass_" + $Suffisso + "_colors.txt"

	echo "Colorazione File temporaneo di ritaglio"
	gdaldem color-relief $FileTemporaneoDiRitaglio $grass_color_text_file $FileTemporaneoDiRitaglioColorato

	echo "Generazione dei tile dal ritaglio temporaneo"
	$t1 = "_TMP_" + $CodiceElaborazione
	$PercorsoCartellaTiles_Suffisso_TMP = $PercorsoCartellaTiles_Suffisso + $t1

	gdal2tiles --version
	gdal2tiles --profile=mercator -z $LivelliZoomRitaglio $FileTemporaneoDiRitaglioColorato $PercorsoCartellaTiles_Suffisso_TMP

	#scorro tutti i file e li confronto con la controparte nella cartella di destinazione		
	Get-ChildItem $PercorsoCartellaTiles_Suffisso_TMP "*.png" -Recurse | Foreach-Object { 		
			
		$FileInDestinazione = $_.FullName
		$FileInDestinazione = $FileInDestinazione.Replace($t1,"")
		$DimensioneOrigine = (Get-Item $_.FullName).length

		if (Test-Path $FileInDestinazione ) {
			#echo "Il file esiste già"				
			$DimensioneDestinazione = (Get-Item $FileInDestinazione).length
			#echo $DimensioneOrigine
			#echo $DimensioneDestinazione
			if (([int]$DimensioneOrigine) -gt ([int]$DimensioneDestinazione)) {
				#echo " con dimensione minore dell'origine"				
				Copy-Item -Path $_.FullName -Destination $FileInDestinazione -Force
			}

		} else {

			New-Item -ItemType File -Path $FileInDestinazione -Force				
			Copy-Item -Path $_.FullName -Destination $FileInDestinazione
		}
	}

	Remove-Item $PercorsoCartellaTiles_Suffisso_TMP -Recurse -Force
	Remove-Item $FileTemporaneoDiRitaglioRiproiettato
	Remove-Item $FileTemporaneoDiRitaglio -Force
	Remove-Item $FileTemporaneoDiRitaglioColorato -Force


}


function GenerazioneTiles {
	param  (
		[string]$BaseDirPath,
		[string]$BaseParentDirPath,
		[string]$LivelliZoomTiles,
		[string]$Suffisso
	)

	if (!($LivelliZoomTiles -eq "")) {

		$grass_color_text_file = $BaseParentDirPath + "\Grass_" + $Suffisso + "_colors.txt"
		$Suffisso_outputFile = $BaseDirPath + "\" + $Suffisso + ".tif"
		$Suffisso_outputFile_color = $PercorsoCartellaTempLavoro + "\" + $Suffisso + "_color.tif"
		$Suffisso_outputFile_Color_Riproiettato = $PercorsoCartellaTempLavoro + "\" + $Suffisso + "_color_riproiettato.tif"

		#genero la cartella per i tiles di google maps
		$PercorsoCartellaTiles = $BaseDirPath + "\" + $Suffisso
		New-Item -ItemType Directory -Force -Path $PercorsoCartellaTiles


		#Coloro il file
		
		gdaldem color-relief $Suffisso_outputFile $grass_color_text_file $Suffisso_outputFile_color

	
		echo "Genero i tiles per il file " + $Suffisso		
		gdal2tiles --profile=mercator -z $LivelliZoomTiles $Suffisso_outputFile_Color $PercorsoCartellaTiles
	}

}

function ProcessaCartella {

	 param (	
		[string]$CartellaDaProcessare,    
		[string]$TilesDaGenerare,
		[string]$LivelliZoom,
		[string]$Algoritmi	
	 )

	$ArrDirDaProcessare = $CartellaDaProcessare.split('\')

	$NomeFileSentinel = $ArrDirDaProcessare[$ArrDirDaProcessare.Length-1]

	$BaseDirPath = $ArrDirDaProcessare[0..($ArrDirDaProcessare.Length-1)] -join "\"
	$BaseParentDirPath = $ArrDirDaProcessare[0..($ArrDirDaProcessare.Length-2)] -join "\"

	if ($DebugVerbosityLevel -eq "2") {
		echo "Directory base:" $BaseDirPath
		echo "Directory padre:" $BaseParentDirPath
		echo "File di base:" $NomeFileSentinel
	}

	# convenzione del nome: https://sentinel.esa.int/web/sentinel/user-guides/sentinel-2-msi/naming-convention
	#formato del file: S2B_MSIL1C_20180911T101019_N0206_R022_T32TQQ_20180911T154541

	$Missione,$LivelloProdotto,$DataOraMinutoSecondo,$Baseline,$OrbitaRelativa,$Tile,$DiscriminatoreProdotto = $NomeFileSentinel.split('_')

	if ($DebugVerbosityLevel -eq "2") {
		echo $Missione
		echo $LivelloProdotto
		echo $DataOraMinutoSecondo
		echo $Baseline
		echo $OrbitaRelativa
		echo $Tile
		echo $DiscriminatoreProdotto
	}

	$PercorsoJpeg2000Base = $BaseDirPath + "\" + $NomeFileSentinel + ".SAFE\GRANULE\"

	$PercorsoJpeg2000 = Get-ChildItem $PercorsoJpeg2000Base | Foreach-Object { $_.Name}

	$PercorsoJpeg2000 = $PercorsoJpeg2000base + $PercorsoJpeg2000 + "\IMG_DATA"

	if ($DebugVerbosityLevel -eq "2") {
		echo "Percorso jpeg 2000:"	$PercorsoJpeg2000
	}

	$FileListaJpeg2000 = Get-ChildItem $PercorsoJpeg2000 | Foreach-Object { $_.Name}

	if ($DebugVerbosityLevel -eq "2") {
		echo "Lista dei file in formato JPEG2000" $FileListaJpeg2000
	}

	$FileListaJpeg2000Split = $FileListaJpeg2000[0].Split("_")

	#join array su stringa ..: 
	$FileJpeg2000Base = $FileListaJpeg2000Split[0..($FileListaJpeg2000Split.Length-2)] -join "_"

	$Verde_FileJpeg2000 = $PercorsoJpeg2000 + "\" + $FileJpeg2000Base + "_B03.jp2"
	$Rosso_FileJpeg2000 = $PercorsoJpeg2000 + "\" + $FileJpeg2000Base + "_B04.jp2"	
	$InfraRosso_FileJpeg2000 = $PercorsoJpeg2000 + "\" + $FileJpeg2000Base + "_B08.jp2"	
	$InfraRossoOndeCorte20M_FileJpeg2000 = $PercorsoJpeg2000 + "\" + $FileJpeg2000Base + "_B12.jp2"
	$RGB_FileJpeg2000 = $PercorsoJpeg2000 + "\" + $FileJpeg2000Base + "_TCI.jp2"

	if ($DebugVerbosityLevel -eq "2") {
		echo "File Rosso: " $Rosso_FileJpeg2000
		echo "File InfraRosso: " $InfraRosso_FileJpeg2000
		echo "Output " $NDVI_outputFile
	}

	#genero la cartella temporanea di lavoro
	$PercorsoCartellaTempLavoro = $BaseDirPath + "\TempWorkDir"
	echo "cartella temporanea ..: "$PercorsoCartellaTempLavoro
	New-Item -ItemType Directory -Force -Path $PercorsoCartellaTempLavoro

	



	#### NDVI

	$NDVI_outputFile = $BaseDirPath + "\NDVI.tif"
	
	if ($Algoritmi -Match "NDVI") {
		echo "Calcolo TIF con NDVI"
		#calcolo gli indici NDVI, le funzioni asarray leggono i dati di tipo float32, come output ottengo un float32
		gdal_calc -A $InfraRosso_FileJpeg2000 -B $Rosso_FileJpeg2000 --outfile=$NDVI_outputFile --calc="(asarray(A, dtype=float32)-asarray(B, dtype=float32))/(asarray(A, dtype=float32)+asarray(B, dtype=float32))"   --type="Float32" --debug
	
	}

	#Ri-proietto il file NDVI per GMAPS
	#gdalwarp -t_srs EPSG:3857 -r cubicspline -ot Float32 -of GTiff $NDVI_outputFile_color $NDVI_outputFile_Color_Riproiettato

	#Genero i tiles per il file proiettato..

	

	if ($TilesDaGenerare -Match "NDVI" -and ($CoordinateRitaglio) -eq "") {		
		GenerazioneTiles $BaseDirPath $BaseParentDirPath $LivelliZoom "NDVI"
	}

	#### Fine NDVI



	#### NDWI_GAO (Gao)

	$NDWI_GAO_outputFile = $BaseDirPath + "\NDWI_GAO.tif"
	
	if ($Algoritmi -Match "NDWI_GAO") {

		$InfraRosso20M_FileJpeg2000 = $PercorsoCartellaTempLavoro + "\B08_Resample20M.tif"

		echo "Ricampionamento TIF a 20 metri  NDWI_GAO"		
		echo $InfraRosso20M_FileJpeg2000

		gdalwarp -tr 20 20  -r average $InfraRosso_FileJpeg2000 $InfraRosso20M_FileJpeg2000

		echo "Calcolo TIF con NDWI_GAO"
		#calcolo gli indici NDWI_GAO, le funzioni asarray leggono i dati di tipo float32, come output ottengo un float32
		gdal_calc -A $InfraRosso20M_FileJpeg2000 -B $InfraRossoOndeCorte20M_FileJpeg2000 --outfile=$NDWI_GAO_outputFile --calc="(asarray(A, dtype=float32)-asarray(B, dtype=float32))/(asarray(A, dtype=float32)+asarray(B, dtype=float32))"   --type="Float32" --debug
	
	}
	
	#Genero i tiles per il file proiettato..	

	if ($TilesDaGenerare -Match "NDWI_GAO" -and ($CoordinateRitaglio) -eq "") {		
		GenerazioneTiles $BaseDirPath $BaseParentDirPath $LivelliZoom "NDWI_GAO"
	}

	#### Fine NDWI_GAO

	

	#### NDWI_McFeeters (McFeeters)

	$NDWI_McFeeters_outputFile = $BaseDirPath + "\NDWI_McFeeters.tif"
	
	if ($Algoritmi -Match "NDWI_McFeeters") {
		echo "Calcolo TIF con NDWI_McFeeters"
		#calcolo gli indici NDWI_McFeeters, le funzioni asarray leggono i dati di tipo float32, come output ottengo un float32
		gdal_calc -A $Verde_FileJpeg2000 -B $InfraRosso_FileJpeg2000 --outfile=$NDWI_McFeeters_outputFile --calc="(asarray(A, dtype=float32)-asarray(B, dtype=float32))/(asarray(A, dtype=float32)+asarray(B, dtype=float32))"   --type="Float32" --debug
	
	}

	
	#Genero i tiles per il file proiettato..

	$grass_color_text_file = $BaseParentDirPath + "\Grass_NDWI_McFeeters_colors.txt"

	if ($TilesDaGenerare -Match "NDWI_McFeeters" -and ($CoordinateRitaglio) -eq "") {		
		GenerazioneTiles $BaseDirPath $BaseParentDirPath $LivelliZoom "NDWI_McFeeters"
	}


	#### Fine NDWI_McFeeters



	#### RGB
	$RGB_outputFile_Color_Riproiettato = $PercorsoCartellaTempLavoro + "\RGB_color_riproiettato.tif"
	#Ri-proietto il file RGB per GMAPS
	#gdalwarp -t_srs EPSG:3857 -r cubicspline -ot Float32 -of GTiff $RGB_FileJpeg2000  $RGB_outputFile_Color_Riproiettato


	if ($TilesDaGenerare -Match "RGB" -and ($CoordinateRitaglio) -eq "") {		

		
		#genero la cartella per i tiles di google maps
		$PercorsoCartellaTiles_RGB = $BaseDirPath + "\RGB"
		New-Item -ItemType Directory -Force -Path $PercorsoCartellaTiles_RGB

		echo "genero il file RGB per GMAPS (alternativo alla riproiezione)"		
		gdal_translate $RGB_FileJpeg2000  $RGB_outputFile_Color_Riproiettato
		
		echo "Genero i tiles per il file proiettato.."
		gdal2tiles --profile=mercator -z $LivelliZoom $RGB_outputFile_Color_Riproiettato $PercorsoCartellaTiles_RGB
	}

	#Genero, se richiesto, un ritaglio ed eseguo l'estrazione dei tile sul ritaglio..
	if (!($CoordinateRitaglio) -eq "") {
	
		echo "Genero un ritaglio ed eseguo l'estrazione dei tile"		
		New-Item -ItemType Directory -Force -Path $PercorsoCartellaTempLavoro
		
		GenerazioneTilesRitaglio $BaseDirPath $BaseParentDirPath $PercorsoCartellaTempLavoro $LivelliZoomRitaglio $TilesDaGenerare $CoordinateRitaglio		

	}
	

	#Rimuovo la cartella con i file temporanei..
	#Remove-Item $PercorsoCartellaTempLavoro -Force -Recurse

}


#Solo se necessario estraggo i file
$ZipFile = $CartellaDaProcessare + ".zip"
if (!(Test-Path -Path $CartellaDaProcessare )) {
	echo "Estrazione della cartella compressa"
	Unzip $ZipFile $CartellaDaProcessare

	#copia di backup del file
	if (!($CartellaBackupZipFile -eq "")) {
		Copy-Item -Path $ZipFile -Destination $CartellaBackupZipFile
	}
}


ProcessaCartella $CartellaDaProcessare $TilesDaGenerare $LivelliZoom $Algoritmi