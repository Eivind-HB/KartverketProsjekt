
//Lager leaflet tegne kontroller
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

//Håndterer tegne eventen
map.on(L.Draw.Event.CREATED, function (e) {
    var type = e.layerType,
        layer = e.layer;

    drawnItems.addLayer(layer);

    var geoJsonData = layer.toGeoJSON();
    var geoJsonString = JSON.stringify(geoJsonData);

    document.getElementById('geoJsonInput').value = geoJsonString;
});