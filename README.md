# Kartverket Prosjektet - Gruppe 13
Dette er Gruppe 13s prosjekt for semester 3. av IT og Informasjons systemer. Prosjektet blir gjort i samarbeid med Kartverket. Der målet er å lage en applikasjon som gjør det mulig for brukere å sende til innmeldinger om feil i kart, og valgfritt lage bruker for å følge med på disse innmeldingene. 
Innmeldingene blir gjort via en Leaflet og LeafletDraw kart GUI. Som gir lokasjons data via GeoJSON og kommune informasjon med bruk av Kartverkets egen KommuneInfoAPI. 
Bruker gir informasjon om innsendte feil via en tekst beskrivelse, en dropdown meny for type feilmelding og valgfritt bilde av problemet.

Applikasjonen ble utviklet i VisualStudio 2022, med C# som hoved programmeringsspråk, men også med JavaScript ettersom Leaflet er basert på dette. Applikasjonen kjører i Docker via docker-compose. Prosjektet er også koblet til MariaDB database som lagrer data ved bruk av Entity Framework. Det gjør at både prosjektet og databasen kjører i samme container i Docker, noe som er essensielt for at data blir lagret på riktig måte. En Migration tabell er også oprettet for å holde databasen oppdatert med ny data.
Prosjektet bruker MVC design mønster, som deler den i tre forskjellige lag: Model, View og Controller.

## Forrutsetninger:
Git, Docker, MariaDB


## Installasjon:
1. Bruk gitbash eller git i VS for å "pull"-e applikasjonen
2. Pass på at du har docker, git, og Visual Studio nedlastet og at de funker. Docker må være oppe for at neste steg skal fungere.
3. Trykk på tannhjulet ved siden av den grønne start knappen (pilen), og bytt til "docker compose", kjør deretter applikasjonen på "docker compose". Dette skal starte en container i Docker med blant annet MariaDB (Databasen).
4. Deretter trykker du på tannhjulet igjen og bytter tilbake til "Kartverket". Kjør deretter applikasjonen på dockerfile, ved å trykke på drop down menu-en ved siden av start knappen (grønne trekanten) og bytte til dockerfile.
5.  Etter dette trykker du på Tools-> NuGet Package Manager-> Package Manager Console. Dette skal åpne et vindu nederst i VS hvor du kan skrive inn kommandoer. Her skal du skrive inn "Update-Database", deretter venter du til den er ferdig.
6.  Nå kan du bytte ifra "dockerfile" til "http" (på lik måte som i steg 4). Nå skal applikasjonen være ferdig innstallert og du kan starte applikasjonen ved å trykke på den grønne start-knappen. Hvis du skrur av pcen og skal kjøre applikasjonen på nytt så må du starte docker containeren på nytt, dette kan du gjøre gjennom å gå i docker appen og trykke "start container" på containeren som ble lagd i steg 3 (den heter "dockercompose" også har den en god del tall i navnet)


til info: 
Leaflet kartet er litt sta så noen ganger etter man har lagd et "point", firkant, polygon osv. så må man trykke på kartet ved siden av det du tegnet for at kommune- og fylkesinformasjonen skal hentes av API. Hvis du får en feil etter at du har opprettet en sak så kan det være en god ide å sjekke nederst på "rett i kartet"-siden at det er fyllt inn informasjon under "Kommune Informasjon". 
