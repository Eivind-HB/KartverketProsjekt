function initializeDrawnChanges(drawnChanges) {
    
    // Initialize a feature group to hold all layers
    var allLayers = new L.FeatureGroup();
    map.addLayer(allLayers);

    // Loop over each drawn change and parse the GeoJSON from the locationInfo
    

    // Fit the map bounds to all layers
    if (allLayers.getLayers().length > 0) {
        map.fitBounds(allLayers.getBounds());
        map.invalidateSize();
    }

    // Handles the draw events
    map.on(L.Draw.Event.CREATED, function (e) {
        var drawnLayer = e.layer;
        allLayers.addLayer(drawnLayer);
        var bounds = allLayers.getBounds();
        if (bounds.isValid()) {
            map.fitBounds(bounds);
            map.invalidateSize();
        }
    });
}