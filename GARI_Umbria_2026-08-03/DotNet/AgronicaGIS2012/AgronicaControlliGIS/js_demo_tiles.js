var rainfallOverlayToggle = 0;
var rainMapOverlay = new google.maps.ImageMapType({
    getTileUrl: function (coord, zoom) {
        return 'https://www.agronica.it/deposito/Immagini_Mappe_Satellite/' + coord.x + '_' + coord.y + '_' + zoom + '.jpg';
    },
    tileSize: new google.maps.Size(256, 256),
    maxZoom: 16,
    minZoom: 10
});


function DemoGis() {
    mappa.elemenotMappa.overlayMapTypes.insertAt(
      0, new CoordMapType(new google.maps.Size(256, 256)));

    var homeControlDiv = document.createElement('div');
    var homeControl = new HomeControl(homeControlDiv, map);

    homeControlDiv.index = 1;
    mappa.elemenotMappa.controls[google.maps.ControlPosition.TOP_RIGHT].push(homeControlDiv);

}






function attivaSatelliteNew() {

    //If the rainfall map is NOT showing aleady then show it ...
    if (rainfallOverlayToggle === 0) {
        //Overlays the rainfall map on top of the Google map
        mappa.elemenotMappa.overlayMapTypes.insertAt(0, rainMapOverlay);
        //Show the weather key.
        mappa.elemenotMappa.controls[google.maps.ControlPosition.TOP_LEFT].push(weatherKeyDiv);
        rainfallOverlayToggle = 1;
    }
    //If the rainfall map is showing already then hide it ...
    else {
        //remove the overlay map.
        mappa.elemenotMappa.overlayMapTypes.removeAt(0, rainMapOverlay);
        //remove the weather key
        mappa.elemenotMappa.controls[google.maps.ControlPosition.TOP_LEFT].pop(weatherKeyDiv);
        rainfallOverlayToggle = 0;
    }

}




function CoordMapType(tileSize) {
    this.tileSize = tileSize;
}


function HomeControl(controlDiv, map) {

    // Set CSS styles for the DIV containing the control
    // Setting padding to 5 px will offset the control
    // from the edge of the map.
    controlDiv.style.padding = '5px';

    // Set CSS for the control border.
    var controlUI = document.createElement('div');
    controlUI.style.backgroundColor = 'white';
    controlUI.style.borderStyle = 'solid';
    controlUI.style.borderWidth = '2px';
    controlUI.style.cursor = 'pointer';
    controlUI.style.textAlign = 'center';
    controlUI.style.zIndex = 10000;
    controlUI.title = 'sat';
    controlDiv.appendChild(controlUI);

    // Set CSS for the control interior.
    var controlText = document.createElement('div');
    controlText.style.fontFamily = 'Arial,sans-serif';
    controlText.style.fontSize = '12px';
    controlText.style.paddingLeft = '4px';
    controlText.style.paddingRight = '4px';
    controlText.innerHTML = '<strong>sat</strong>';
    controlUI.appendChild(controlText);

    // Setup the click event listeners: simply set the map to Chicago.
    google.maps.event.addDomListener(controlUI, 'click', function () {
        attivaSatelliteNew();
    });
}