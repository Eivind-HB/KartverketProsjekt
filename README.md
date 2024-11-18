# KartverketProsjekt
Dette er Gruppe 13s prosjekt for semester 3. av IT og Informasjons systemer. Prosjektet gjøres for Kartverket og skal gjøre det mulig for brukerne å oprette en konto og legge til innmeldinger om forandringer eller feil i stien. Brukerne gjør det ved å markere området på det interaktivt kart. De må  også skrive inn beskrivelsen og velge type problem frå dropdown meny. Brukerne har også mulighet til å laste opp bildet av problemet om de ønsker så. Rettelser blir da registrert og lastet opp i databasen, brukeren blir omdirigert til siden som viser registrerte rettelser. 

Hvis brukerne ønsker det, så kan de også trykke på «Om oss» knappen på navigerinsmeny som sender til Kartverkets sin side som inneholder informasjon om dem. I dropdown meny som innehoder knapper som tilbyr innlogging og registreringstjenester så finnes det også «Kontakt oss» knapp som omdirigerer bruker til siden med alt kontakinformasjon til Kartverket. 

Applikasjonen ble utviklet i VisualStudio 2022, med C# som valgt programmeringsspråk, og kjører i Docker via docker-compose. Prosjektet er også koblet til MariaDB database som lagrer GeoJson og innloggingsdata, ved bruk av Entity Framework. Det gjør at både prosjektet og databasen kjører i samme container i Docker, noe som er essensielt for at data blir lagret på riktig måte. En Migration tabell er også oprettet for å holde databasen oppdatert med ny data.
Prosjekter bruker MVC desing mønster, som deler den i tre forskjellige lag: Model, View og Controller. I gruppas prosjekt så er det definert fire kontrollere: 
1. CaseController: kontroller for saksbehandlere som henter nye innmeldinger ved hjelp av saksnummer, sletter saker som er løst og oppdaterer beskrivelsen for saken. Den er også ansvarlig for logikken som viser meldinger om det har oppståd feil ved sletting av saken, eller om den ble ikke funnet. 
2.	HomeController: Hoved kontroller som er ansvarlig for mesteparten av logikken i applikasjonen. Dbcontext for Entity Framework, overføring av forandringer på kartet slik at det vises på separat side omdirigering til eksterne sider er bestemt i den kontroller.
3.	MapController: kontroller ansvarlig for å hente data angående kommuneinfo API, samt data om lokasjoner markert av brukerne for saksbehandlere.
4.	UserController: Kontroller anvsarlig for registrering av ny brukere, innlogging og henting av UserData fra databasen

I applikasjonen så er det 2 model mapper: Models og API_Models. Den fyrste mappa inneholder modeller for data for innlogging og brukerne, samt relevant data for bruk av GeoJson, innmeldinger, kart og fylke- og kommuneinfo. API_Models mappen inneholder too modeller for henting av data som blir brukt til å vise riktig info om kommuner og fylkeskommuner. 

I Views mapper så er det definert utseende til sidene i prosjektet. Her er det også definert bruken av leaflet til den interaktive kartet brukt for innmeldinger. I Layout.cshtml filen så er det ogå opgitt stylesheets som ble brukt til å forandre på utseendet til prosjektet.

Installasjon:
1. Bruk gitbash eller git i VS for å "pull"-e applikasjonen
2. Pass på at du har docker, git, og Visual Studio nedlastet og at de funker. Docker må være oppe for at neste steg skal fungere.
3. Trykk på tannhjulet ved siden av den grønne start knappen (pilen), og bytt til "docker compose", kjør deretter applikasjonen på "docker compose". Dette skal starte en container i Docker med blant annet MariaDB (Databasen).
4. Deretter trykker du på tannhjulet igjen og bytter tilbake til "Kartverket". Kjør deretter applikasjonen på dockerfile, ved å trykke på drop down menu-en ved siden av start knappen (grønne trekanten) og bytte til dockerfile.
5.  Etter dette trykker du på Tools-> NuGet Package Manager-> Package Manager Console. Dette skal åpne et vindu nederst i VS hvor du kan skrive inn kommandoer. Her skal du skrive inn "Update-Database", deretter venter du til den er ferdig.
6.  Nå kan du bytte ifra "dockerfile" til "http" (på lik måte som i steg 4). Nå skal applikasjonen være ferdig innstallert og du kan starte applikasjonen ved å trykke på den grønne start-knappen. Hvis du skrur av pcen og skal kjøre applikasjonen på nytt så må du starte docker containeren på nytt, dette kan du gjøre gjennom å gå i docker appen og trykke "start container" på containeren som ble lagd i steg 3 (den heter "dockercompose" også har den en god del tall i navnet)
