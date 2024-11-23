console.log("leafletDraw.js script loaded");

// Ensure the map variable is defined and initialized
if (typeof map === 'undefined') {
    console.error("Map is not defined");
} else {
    console.log("Map is defined");

    // Create a Leaflet feature group to store drawn items
    var drawnItems = new L.FeatureGroup();
    map.addLayer(drawnItems);

    const strokeColor = '#FF5F15'; /* Safety orange outline */
    const fillColor =  '#FFA500'; /* orange fill */    

    var drawControl = new L.Control.Draw({
        draw: {
            polyline: {
                shapeOptions: {
                    color: strokeColor,
                    fillColor: fillColor,
                    fillOpacity: 0.5
                }
            },
            marker: true,
            rectangle: {
                shapeOptions: {
                    color: strokeColor,
                    fillColor: fillColor,
                    fillOpacity: 0.5
                }
            },
            circle: false,
            circlemarker: false,          
            polygon: false,
        },
        edit: {
            featureGroup: drawnItems,
            edit: false,
        }
    });
    map.addControl(drawControl);

    // Handle the draw event
    map.on(L.Draw.Event.CREATED, function (e) {
        var type = e.layerType;
        var layer = e.layer;

        drawnItems.addLayer(layer);

        var geoJsonData = layer.toGeoJSON();
        var geoJsonString = JSON.stringify(geoJsonData);

        document.getElementById('geoJsonInput').value = geoJsonString;

        // Recalculate the bounds of all drawn items
        var bounds = drawnItems.getBounds();
        console.log("Bounds:", bounds);

        if (bounds.isValid()) {
            console.log("Fitting bounds:", bounds);
            map.fitBounds(bounds);

            // Force map update
            map.invalidateSize();
        } else {
            console.error("Invalid bounds:", bounds);
        }
    });
}

