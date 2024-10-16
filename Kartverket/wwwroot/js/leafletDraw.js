console.log("leafletDraw.js script loaded");

// Ensure the map variable is defined and initialized
if (typeof map === 'undefined') {
    console.error("Map is not defined");
} else {
    console.log("Map is defined");

    // Create a Leaflet feature group to store drawn items
    var drawnItems = new L.FeatureGroup();
    map.addLayer(drawnItems);

    var drawControl = new L.Control.Draw({
        draw: {
            polygon: true,
            polyline: true,
            marker: true,
            circle: false,
            rectangle: true
        },
        edit: {
            featureGroup: drawnItems
        }
    });
    map.addControl(drawControl);

    // Handle the draw event
    map.on(L.Draw.Event.CREATED, function (e) {
        var type = e.layerType,
            layer = e.layer;

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