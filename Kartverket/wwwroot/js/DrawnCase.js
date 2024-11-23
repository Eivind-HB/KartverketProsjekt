/* 
  This function displays a single DrawnChange on a Leaflet map.
  It can use data from the DOM if geoJsonData and caseNo are not explicitly provided.
*/
function singleDrawnChange(geoJsonData = null, caseNo = null) {
    // If caseNo is null, log an error.
    if (!caseNo) {
        console.error("caseNo mangler. Kan ikke vise endringer uten et gyldig saksnummer.");
        return;
    }

    // If no geoJsonData attempt to fetch from DOM
    if (!geoJsonData) {
        const individualCase = document.querySelector(`.case-data[data-case-no="${caseNo}"]`);
        if (individualCase) {
            try {
                geoJsonData = JSON.parse(individualCase.getAttribute('data-geojson'));
            } catch (error) {
                console.error(`Ugyldig GeoJSON for sak ${caseNo}:`, error);
                return;
            }
        } else {
            console.error(`Ingen case-data funnet for caseNo: ${caseNo}`);
            return;
        }
    }

    // If  GeoJSON-data is available, add it to the map
    if (geoJsonData) {
        L.geoJSON(geoJsonData, {
            style: function () {
                return { className: 'geojson-feature' };
            },
            onEachFeature: function (feature, layer) {
                const popupContent = `Saksnummer: ${caseNo}`;
                layer.bindPopup(popupContent);
            }
        }).addTo(map);
        console.log(`Endring for sak ${caseNo} lagt til kartet.`);
    } else {
        console.error("GeoJSON-data er tom eller ugyldig.");
    }
}


/* 
    This function goes through all the cases currently available in the view and uses singleDrawnChange to draw them on a leaflet map.
    It only fetches data from the DOM:
    It requires a Leaflet map
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
