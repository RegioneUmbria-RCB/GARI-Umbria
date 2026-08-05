var gMapsUtility = {
    getArea: function (CoordFromPoints, CoordFromPointsMVCArray, selectedShape) {
        var area = 0;

        if (CoordFromPoints == '') {
            var MVCArray = selectedShape.getPath();
            area = google.maps.geometry.spherical.computeArea(MVCArray);

        }
        else {
            area = google.maps.geometry.spherical.computeArea(CoordFromPointsMVCArray);
        }

        area = numeri.arrotonda(area / 10000, 4);
        utility.log('mvcarray:' + CoordFromPointsMVCArray);
        utility.log('Area calcolat:' + area);
        return area;
    },
    fromLatLngToPoint: function (latLng, map) {
        var topRight = map.getProjection().fromLatLngToPoint(map.getBounds().getNorthEast());
        var bottomLeft = map.getProjection().fromLatLngToPoint(map.getBounds().getSouthWest());
        var scale = Math.pow(2, map.getZoom());
        var worldPoint = map.getProjection().fromLatLngToPoint(latLng);
        return new google.maps.Point((worldPoint.x - bottomLeft.x) * scale, (worldPoint.y - topRight.y) * scale);
    },
    getRadius: function(zoom) {
        var radius_px = 0.40 / 2
        var constMetersDegress = 1000; // TODO Verifiy

        //zoom <-> m/px = http://timwhitlock.info/blog/2010/04/google-maps-zoom-scales/
        var scaled_zooms = {
            22: 0.02,
            21: 0.04,
            20: 0.09,
            19: 0.19,
            18: 0.37,
            17: 0.74,
            16: 1.48,
            15: 3,
            14: 6,
            13: 12,
            12: 24,
            11: 48,
            10: 95,
            9: 190,
            8: 378,
            7: 752,
            6: 1485,
            5: 2909,
            4: 5540,
            3: 10064,
            2: 16355,
            1: 21282,
            0: 30000
        }

        var radius_meters = radius_px * scaled_zooms[zoom];
        var radius_degrees = radius_meters / constMetersDegress;
        return radius_degrees;
    },
    WMS_GetFeatureInfoGeoData: function (map, latLng) {
        var lat = parseFloat(latLng.lat());
        var lng = parseFloat(latLng.lng());

        //console.info('------------------------------')
        var radius_degrees = this.getRadius(map.getZoom());
        var buffer_sw_y_dd = lat - radius_degrees
        var buffer_sw_x_dd = lng - radius_degrees
        var buffer_ne_y_dd = lat + radius_degrees
        var buffer_ne_x_dd = lng + radius_degrees
        //console.info('bbox dd',buffer_sw_x_dd+','+buffer_sw_y_dd+','+buffer_ne_x_dd+','+buffer_ne_y_dd)

        var buffer_sw_dd = new google.maps.LatLng(buffer_sw_y_dd, buffer_sw_x_dd)//decimal degrees
        var buffer_ne_dd = new google.maps.LatLng(buffer_ne_y_dd, buffer_ne_x_dd)//decimal degrees

        var buffer_sw_px = this.fromLatLngToPoint(buffer_sw_dd, map);//pixels
        var buffer_ne_px = this.fromLatLngToPoint(buffer_ne_dd, map);//pixels
        //console.info('buffer_sw_px',buffer_sw_px,'buffer_ne_px',buffer_ne_px)

        var buffer_width_px = (Math.abs(buffer_ne_px.x - buffer_sw_px.x)).toFixed(0);
        var buffer_height_px = (Math.abs(buffer_ne_px.y - buffer_sw_px.y)).toFixed(0);
        //console.info('buffer_width_px',buffer_width_px, 'buffer_height_px',buffer_height_px)

        var center_x_px = (buffer_width_px / 2).toFixed(0);
        var center_y_px = (buffer_height_px / 2).toFixed(0);
        //console.info('center_x_px',center_x_px,'center_y_px',center_y_px)
        //console.info('------------------------------')

        var o = {
            WIDTH: buffer_width_px,
            HEIGHT: buffer_height_px,
            Y: center_y_px,
            X: center_x_px,
            BBOX: buffer_sw_x_dd + ',' + buffer_sw_y_dd + ',' + buffer_ne_x_dd + ',' + buffer_ne_y_dd
        }

        return o;
    }
}

export {gMapsUtility}
