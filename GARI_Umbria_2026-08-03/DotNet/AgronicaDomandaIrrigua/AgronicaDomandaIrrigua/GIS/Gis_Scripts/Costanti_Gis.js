//costanti
var inv16x16 = "inv16x16.png";
var trattore = "../AB_Immagini/Icone/trattore.png";
var flgSelezionato = "../AB_Immagini/Icone/FrecciaDN_32x16.png";
var CoordFromPoints = "";
var CoordFromPointsMVCArray = new google.maps.MVCArray();

var centoH = $(window).height() - 15;
var centoW = $(window).width() - 15;
var centoH_xs = 300;
var centoW_xs = 550;

var separatoreChiaveAlbero = '§';
var zoomRicercaIndirizzo = 16;
var zoomRicercaIndirizzoxRitaglio = 17; //Zoom + ===>> indicare un numero maggiore
var offSetScrollTop = -150; //Offset per scroll dell'albero quando si clicka un poligono
var chiaveAlberoPlanning = "51";

var xCreazioneCombo = 'Version§ 5.10.038|GPS_Status§ 2|Status_Txt§ DGPS|Swath§ 22|Height§ -5,476|DateClosed§ 19/03/2012 00:00:00|TimeClosed§ 09:43:49am|AppldRate§ 13298,249|Moisture§ |Material§ Default_AppMaterial|MaterialID§ -1|Speed§ 0,013';
var xComboCorrenti = 'GPS_Status§Swath§Height§AppldRate§Status_Txt§Speed§';

var ID_Overlay = 50;

var CanaleAlfaDidascalia = "0.8"
//fine costanti
