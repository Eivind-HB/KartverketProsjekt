
/* 
  This function is to display a single DrawnChange on a leafletmap. 
  Needs to be given valid geoJsonData a caseNumber to display
*/

function singleDrawnChange(geoJsonData, CaseNo) { 

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
}