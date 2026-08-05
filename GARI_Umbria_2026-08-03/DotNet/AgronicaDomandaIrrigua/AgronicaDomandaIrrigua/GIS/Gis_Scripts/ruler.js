/*
	javascript ruler for google maps V3

	by Giulio Pons. http://www.barattalo.it
	this function uses the label class from Marc Ridley Blog

*/
var righelli_mark=  new Array();
var rulerlabel = new Array();
var rulerpoly ;

function addruler(obj) {
 
    if (obj.is(":checked")) {
        
        righelli_mark = new Array();
        rulerlabel = new Array();
        righelli_mark.push(new google.maps.Marker({
            position: mappa.elemenotMappa.getCenter(),
            map: mappa.elemenotMappa,
            draggable: true
        }));
      
        righelli_mark.push(new google.maps.Marker({
            position: mappa.elemenotMappa.getCenter(),
            map: mappa.elemenotMappa,
            draggable: true
        }));

        rulerlabel.push(new Label({ map: mappa.elemenotMappa }));
        rulerlabel[0].bindTo('position', righelli_mark[0], 'position');
        rulerpoly = new google.maps.Polyline({
            path: [righelli_mark[0].position, righelli_mark[1].position],
            strokeColor: "#FFFF00",
            strokeWeight: 3,
            zIndex:100000
        });
        rulerpoly.setMap(mappa.elemenotMappa);

        rulerlabel[0].set('text', distance(righelli_mark[0].getPosition().lat(), righelli_mark[0].getPosition().lng(), righelli_mark[1].getPosition().lat(), righelli_mark[1].getPosition().lng()));
        


        google.maps.event.addListener(righelli_mark[0], 'drag', function () {
            var p_medio = P_Medio(righelli_mark[0].getPosition().lat(), righelli_mark[0].getPosition().lng(), righelli_mark[1].getPosition().lat(), righelli_mark[1].getPosition().lng());

            rulerlabel[0].set('text', distance(righelli_mark[0].getPosition().lat(), righelli_mark[0].getPosition().lng(), righelli_mark[1].getPosition().lat(), righelli_mark[1].getPosition().lng()));
            
            rulerpoly.setPath([righelli_mark[0].getPosition(), righelli_mark[1].getPosition()]);

            //rulerlabel[1].set('text', distance(righelli_mark[0].getPosition().lat(), righelli_mark[0].getPosition().lng(), righelli_mark[1].getPosition().lat(), righelli_mark[1].getPosition().lng()));
        });

        google.maps.event.addListener(righelli_mark[1], 'drag', function () {
            var p_medio = P_Medio(righelli_mark[0].getPosition().lat(), righelli_mark[0].getPosition().lng(), righelli_mark[1].getPosition().lat(), righelli_mark[1].getPosition().lng());
            rulerlabel[0].set('text', distance(righelli_mark[0].getPosition().lat(), righelli_mark[0].getPosition().lng(), righelli_mark[1].getPosition().lat(), righelli_mark[1].getPosition().lng()));
            //rulerlabel[0].bindTo('position', p_medio, 'position');
            rulerpoly.setPath([righelli_mark[0].getPosition(), righelli_mark[1].getPosition()]);
            
            //rulerlabel[1].set('text', distance(righelli_mark[0].getPosition().lat(), righelli_mark[0].getPosition().lng(), righelli_mark[1].getPosition().lat(), righelli_mark[1].getPosition().lng()));
        });

    } else {
        for (i in righelli_mark) {
            righelli_mark[i].setMap(null);
        }
        for (i in rulerlabel) {
            rulerlabel[i].setMap(null);
        }
        rulerpoly.setMap(null);
    }
}


function distance(lat1,lon1,lat2,lon2) {
	var R = 6371; // km (change this constant to get miles)
	var dLat = (lat2-lat1) * Math.PI / 180;
	var dLon = (lon2-lon1) * Math.PI / 180; 
	var a = Math.sin(dLat/2) * Math.sin(dLat/2) +
		Math.cos(lat1 * Math.PI / 180 ) * Math.cos(lat2 * Math.PI / 180 ) * 
		Math.sin(dLon/2) * Math.sin(dLon/2); 
	var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1-a)); 
	var d = R * c;
 	return Math.round(d * 1000) + "m";
}
function P_Medio(lat1, lon1, lat2, lon2) {
    var latMedia = (lat1 + lat2) / 2;
    var longMedia = (lon1 + lon2) / 2;
    return new google.maps.LatLng(latMedia, longMedia);
}



var AB_mark = new Array();
var ABlabel = new Array();
var ABpoly;
function addAB(obj, hiddenA, hiddenB, A, B, L1, L2) {

    if (obj.is(":checked")) {
        AB_mark = new Array();
        ABlabel = new Array();

        if (A == null) {
            utility.log("crea nuovo a:");
            A = new google.maps.Marker({
                position: mappa.elemenotMappa.getCenter(),
                map: mappa.elemenotMappa,
                draggable: true
            });
        }
        else {
            A.draggable = true;
        }
        AB_mark.push(A);

        if (B == null) {
            utility.log("crea nuovo b:");
            B = new google.maps.Marker({
                position: mappa.elemenotMappa.getCenter(),
                map: mappa.elemenotMappa,
                draggable: true
            });
        }
        else {
            B.draggable = true;
        }
        AB_mark.push(B);

        if (L1 == null) {
            L1 = new Label({ map: mappa.elemenotMappa });
            L2 = new Label({ map: mappa.elemenotMappa });
        }

        ABlabel.push(L1);
        ABlabel.push(L2);
        ABlabel[0].bindTo('position', AB_mark[0], 'position');
        ABlabel[1].bindTo('position', AB_mark[1], 'position');
        ABpoly = new google.maps.Polyline({
            path: [AB_mark[0].position, AB_mark[1].position],
            strokeColor: "#FFFF00",
            strokeWeight: 3,
            zIndex: 100000
        });
        ABpoly.setMap(mappa.elemenotMappa);

        ABlabel[0].set('text', 'A');
        ABlabel[1].set('text', 'B');

        hiddenA.val(AB_mark[0].getPosition().lat() + ', ' + AB_mark[0].getPosition().lng());
        hiddenB.val(AB_mark[1].getPosition().lat() + ', ' + AB_mark[1].getPosition().lng());

        google.maps.event.addListener(AB_mark[0], 'drag', function () {
            ABpoly.setPath([AB_mark[0].getPosition(), AB_mark[1].getPosition()]);
            hiddenA.val(AB_mark[0].getPosition().lat() + ', ' + AB_mark[0].getPosition().lng());
        });

        google.maps.event.addListener(AB_mark[1], 'drag', function () {
            ABpoly.setPath([AB_mark[0].getPosition(), AB_mark[1].getPosition()]);
            hiddenB.val(AB_mark[1].getPosition().lat() + ', ' + AB_mark[1].getPosition().lng());
        });

    } else {
        
        dialogAB();
        for (i in AB_mark) {
            AB_mark[i].setMap(null);
        }
        for (i in ABlabel) {
            ABlabel[i].setMap(null);
        }
        ABpoly.setMap(null);
    }
}
