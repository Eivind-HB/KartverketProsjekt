/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//Denne siden har inntil videre majoriteten av Leaflet funksjonene. Dette skal separeres i flere moduler senere//
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//Lager en kartvariabel med koodrinater for å vise hele Norge, tar i bruk kode fra https://leafletjs.com/
var map = L.map('map').setView([60.14, 10.25], 5);


//Lager kart variablene. KAN VÆRE LURT Å VISE TIL HVOR KARTA ER FRA 
var topographyLayer = L.tileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/topo/default/webmercator/{z}/{y}/{x}.png');
var grayToneLayer = L.tileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/topograatone/default/webmercator/{z}/{y}/{x}.png');
var hikingLayer = L.tileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/toporaster/default/webmercator/{z}/{y}/{x}.png');
var seaLayer = L.tileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/sjokartraster/default/webmercator/{z}/{y}/{x}.png');


//Navngir Karttypene
var mapLayers = {
    "Topografisk kart": topographyLayer,
    "Gråtone kart": grayToneLayer,
    "Tur kart": hikingLayer,
    "Sjø Kart": seaLayer
};

//Setter topographyLayer som basis kart
topographyLayer.addTo(map);
//Lager leaflet controller som tar i bruk mapLayers
L.control.layers(mapLayers).addTo(map);

//Geolokasjon, dette må gjøres om til en knapp senere
navigator.geolocation.getCurrentPosition(function (position) {
    var lat = position.coords.latitude;
    var lng = position.coords.longitude;

    //Setter kartets visning til brukerens nåværende posisjon
    map.setView([lat, lng], 17);

    L.marker([lat, lng]).addTo(map)
        .bindPopup('Du er her!').openPopup();
    //Error om geolokasjon feiler
}, function (error) {
    console.error('Geolokasjon feilet: ', error);
});


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