function initializeLeafletMap() {
    // Makes the map, showcasing Kristiansand. Uses code from https://leafletjs.com/
    var map = L.map('map').setView([58.1467, 7.996], 9.6);

    // Makes the map layer variables.
    var topographyLayer = L.tileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/topo/default/webmercator/{z}/{y}/{x}.png', {
        attribution: 'Topografisk kart'
    });
    var grayToneLayer = L.tileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/topograatone/default/webmercator/{z}/{y}/{x}.png', {
        attribution: 'Gråtone kart'
    });
    var hikingLayer = L.tileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/toporaster/default/webmercator/{z}/{y}/{x}.png', {
        attribution: 'Tur kart'
    });
    var seaLayer = L.tileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/sjokartraster/default/webmercator/{z}/{y}/{x}.png', {
        attribution: 'Sjø Kart'
    });

    // Names the map layers
    var mapLayers = {
        "Topografisk kart": topographyLayer,
        "Gråtone kart": grayToneLayer,
        "Tur kart": hikingLayer,
        "Sjø Kart": seaLayer
    };

    // Sets topographyLayer as the basis map
    topographyLayer.addTo(map);
    // Makes a controller for the different map types
    L.control.layers(mapLayers).addTo(map);

    // Return the map instance to be used by other functions
    return map;
}

function initializeLeafletDraw(map) {
    console.log("leafletDraw.js script loaded");

    // Ensure the map variable is defined and initialized
    if (!map) {
        console.error("Map is not defined");
        return;
    }

    console.log("Map is defined");

    // Create a Leaflet feature group to store drawn items
    var drawnItems = new L.FeatureGroup();
    map.addLayer(drawnItems);

    const strokeColor = '#FF5F15'; /* Safety orange outline */
    const fillColor = '#FFA500'; /* Orange fill */

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
function addLocateButton(map) {
    if (!map) {
        console.error("Map is not defined");
        return;
    }

    // Add locate button under the draw controls
    L.control.locate({
        position: 'topleft',
        strings: {
            title: "Finn min posisjon"
        },
        locateOptions: {
            enableHighAccuracy: true,
            maxZoom: 18
        },
        flyTo: true,
        onLocationFound: function (e) {
            this._map.setView(e.latlng, 18, {
                animate: true,
                duration: 1
            });
        }
    }).addTo(map);
}