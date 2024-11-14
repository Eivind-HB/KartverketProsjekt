function initializeDrawnChanges(drawnChanges) {
    
    // Initialize a feature group to hold all layers
    var allLayers = new L.FeatureGroup();
    map.addLayer(allLayers);

    // Loop over each drawn change and parse the GeoJSON from the locationInfo
    drawnChanges.forEach(function (change) {
        var geoJsonData;

        // Try to parse the GeoJSON data and returns error if fails
        try {
            geoJsonData = JSON.parse(change.locationInfo);
        } catch (e) {
            console.error(`Invalid JSON in change.locationinfo for changeNo ${change.caseNo}: ${e.message}`);
            return; // Hopp over denne iterasjonen hvis JSON-parsingen feiler
        }


        // Create a layer and add a popup with the AreaChange description
        var drawnLayer = L.geoJSON(geoJsonData).bindPopup(change.description);
        allLayers.addLayer(drawnLayer);

        // Extract coordinates from GeoJSON and reverse geocode
        var geocoordinates = geoJsonData.geometry.coordinates;
        if (geocoordinates && geocoordinates.length >= 2) {
            var latitude = geocoordinates[1];
            var longitude = geocoordinates[0];
            console.log(`Processing change ID ${change.caseNo} with coordinates: ${latitude}, ${longitude}`);

            var url = `https://nominatim.openstreetmap.org/reverse?format=json&lat=${latitude}&lon=${longitude}`;
            fetch(url)
                .then(response => response.ok ? response.json() : Promise.reject(response))
                .then(data => {
                    var address = data.display_name || "Address not found";
                    var popupContent = `${change.description}<br>Address: ${address}`;
                    drawnLayer.setPopupContent(popupContent).openPopup();
                })
                .catch(error => {
                    var popupContent = `${change.description}<br>Address not available`;
                    drawnLayer.setPopupContent(popupContent).openPopup();
                    console.error(`Error updating popup for change ID ${change.caseNo}: ${error}`);
                });
        } else {
            console.error(`Invalid coordinates for issueID ${change.caseNo}`);
        }
    });

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
