
console.log("GeoJSON Data:", geoJsonData);
console.log("CaseNo", caseNumber);
// Makes sure om GeoJSON-data is valid
if (geoJsonData) {
    // Adds the GeoJson data to the map
    L.geoJSON(geoJsonData, {
        style: function (feature) {
            return { className: 'geojson-feature' }; // Uses the leafletdraw.css drawn colors
        }, //Adds casenumber as popup on click
        onEachFeature: function (feature, layer) {

            var popupContent = `Saksnummer: ${caseNumber}`;

            layer.bindPopup(popupContent);
        }
    }).addTo(map);
} else {
    console.error("GeoJSON-data er tom eller ugyldig.");
}