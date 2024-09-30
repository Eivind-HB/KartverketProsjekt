

//Lager en kartvariabel med koodrinater for å vise hele Norge, tar i bruk kode fra https://leafletjs.com/
var map = L.map('map').setView([60.14, 10.25], 5);


//Lager kart variablene. KAN VÆRE LURT Å VISE TIL HVOR KARTA ER FRA 
var topographyLayer = L.tileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/topo/default/webmercator/{z}/{y}/{x}.png')
    grayToneLayer = L.tileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/topograatone/default/webmercator/{z}/{y}/{x}.png'),
    hikingLayer = L.tileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/toporaster/default/webmercator/{z}/{y}/{x}.png'),
    seaLayer = L.tileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/sjokartraster/default/webmercator/{z}/{y}/{x}.png');


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

//En funksjon som viser koodrinatene av der man klikker
var popup = L.popup();


/* MÅ FIKSE PÅ DENNE SÅ DEN FUNGERER MED TEGNEFUNKSJONEN

function onMapClick(e) {
    popup
        .setLatLng(e.latlng)
        .setContent("Du trykket på: " + e.latlng.toString())
        .openOn(map);
}

map.on('click', onMapClick); */




//Geolokasjon
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


//Lagrer en featureGroup for å lagre tegnede former
var drawnItems = new L.FeatureGroup();
map.addLayer(drawnItems);

//Lager leaflet tegne kontroller
var drawControl = new L.Control.Draw({
    edit: {
        featureGroup: drawnItems
    }
});
map.addControl(drawControl);

   

    //Håndterer eventen når en ny form er skapt
    map.on(L.Draw.Event.CREATED, function (e) {
        var layer = e.layer;
        drawnItems.addLayer(layer);

        var geojsonData = layer.toGeoJSON();

     //GeoJSON Data
        fetch('@Url.Action("GetGeoJsonData", "MapController")')
            .then(response => response.json())
            .then(geojsonData => {
                L.geoJSON(geojsonData).addTo(map);
            })
            .catch(error => console.error('Error ved lasting av GeoJSON data:', error));

     //Sender ny GeoJSON data til serveren
        fetch('@Url.Action("SaveGeoJsonData", "MapController")', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(geojsonData)
        }).then(response => {
            if (response.ok) {
                alert('GeoJSON lagret.');
            } else {
                alert('Error ved lagring av GeoJSON');
            }
        }).catch(error => console.error('Error:', error));
    });