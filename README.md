# RestApiC#

RestApi projekt koji omogućuje postavljanje i ažuiranje artikala novosti i vraća json podatke za novosti za običnog registriranog korisnika 


### Kako pokrenuti
- Unutar git bash potrebno pokrenuti naredbu: "git clone https://github.com/krizanact/RestApiC-.git"
- Kada se projekt klonira pokrenuti file ProjectApi.sln unutar Project.API foldera(npr. s Visual Studio ili VsCode)
- Pokreniti Project.API projekt gdje ćemo dobiti index stranicu sa svim metodama i opisima metoda koje se mogu pokrenuti direktno tu preko Swaggera


### Ostale napomene
- Potrebno pokrenut "api/Login" metodu da bi dobili token koji ćemo koristiti za pristup svim ostalim metodama, koristiti podatke za kreiranog admina(podaci su poslani u mailu)
- Moguće dodati i još korisnika nakon prijave s adminom s metodom "api/user/createUser", moguće role za novog korisnika su Admin i Obični korisnik, Admin ima ovlasti da pokrene bilo koju metodu s liste, obični korisnik ima ovlast da pokrene samo metode koje vraćaju pojedinačnu novost ili sve novosti
