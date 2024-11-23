/* 
  This function is to display a single DrawnChange on a leafletmap. 
  Needs to be given valid geoJsonData a caseNumber to display
*/
function singleDrawnChange(geoJsonData, caseNo) { 

    console.log("GeoJSON Data:", geoJsonData);
    console.log("CaseNo", caseNo);
    // Makes sure om GeoJSON-data is valid
    if (geoJsonData) {
        // Adds the GeoJson data to the map
        L.geoJSON(geoJsonData, {
            style: function (feature) {
                return { className: 'geojson-feature' }; // Uses the leafletdraw.css drawn colors
            }, //Adds casenumber as popup on click
            onEachFeature: function (feature, layer) {

                var popupContent = `Saksnummer: ${caseNo}`;

                layer.bindPopup(popupContent);
            }
        }).addTo(map);
    } else {
        console.error("GeoJSON-data er tom eller ugyldig.");
    }
}
/* 
    This function goes through all the cases currently available in the view and uses singleDrawnChange to draw them on a leaflet map.
    It requires a leaflet map
    and a div like this: 
    
            <div class="case-data"
                 data-case-no="@individualCase.CaseNo"
                 data-geojson="@individualCase.LocationInfo">
            </div>

    This is safer than having to fetch the infomration with Html.Raw
*/
function multipleDrawnChanges() {
    document.querySelectorAll('.case-data').forEach(individualCase => {
        const caseNo = individualCase.getAttribute('data-case-no');
        const geoJson = JSON.parse(individualCase.getAttribute('data-geojson')); // Direkte parsing ved antatt gyldighet
        singleDrawnChange(geoJson, caseNo);
    });
}
