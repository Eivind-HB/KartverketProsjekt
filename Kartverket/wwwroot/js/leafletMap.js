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

/*Geolocation, function with eventlistener. If button is clicked user is prompted to give their location.
  If location is given, takes map to that location and sets a marker there.
*/
function findCurrentLocation() {
    navigator.geolocation.getCurrentPosition(function (position) {
        var lat = position.coords.latitude;
        var lng = position.coords.longitude;

        //Sets the map view to the users current coodinates.
        map.setView([lat, lng], 17);

        L.marker([lat, lng]).addTo(map)
            .bindPopup(`Du er her! Latidue: ${lat} Longitude: ${lng}`).openPopup();
        //Error if geolocation fails
    }, function (error) {
        console.error('Geolokasjon feilet: ', error);
    });
}

map.on('click', function (e) {

    const { lat, lng } = e.latlng;
    console.log(`Clicked on: Latitude: ${lat}, Longitude: ${lng}`);

    //Antageligvis feil
    fetch(`/api/kommuneinfo?latitude=${lat}&longitude=${lng}`) 
        .then(response => response.json())
        .then(data => {
            console.log('Kommuneinfo:', data);
            
        })
        .catch(error => console.error('Feil ved henting av kommuneinfo:', error));
});

document.getElementById("curLocationButton").addEventListener("click", findCurrentLocation);