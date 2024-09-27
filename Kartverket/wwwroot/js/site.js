// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


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

function onMapClick(e) {
    popup
        .setLatLng(e.latlng)
        .setContent("Du trykket på: " + e.latlng.toString())
        .openOn(map);
}

map.on('click', onMapClick);
