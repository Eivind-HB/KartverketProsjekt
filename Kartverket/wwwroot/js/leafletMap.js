﻿    //Lager en kartvariabel med koodrinater for å vise hele Norge, tar i bruk kode fra https://leafletjs.com/
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

    //Sets topographyLayer as the basis map
    topographyLayer.addTo(map);
    //Makes a controller for the different map types
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
                .bindPopup('Du er her!').openPopup();
            //Error if geolocation fails
        }, function (error) {
            console.error('Geolokasjon feilet: ', error);
        });
    }

    document.getElementById("curLocationButton").addEventListener("click", findCurrentLocation);