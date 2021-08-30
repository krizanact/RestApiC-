# RestApiC#

RestApi projekt koji omogućuje postavljanje i ažuiranje artikala novosti i vraća json podatke za novosti za običnog registriranog korisnika 


### Kako pokrenuti
- Unutar git bash potrebno pokrenuti naredbu: git clone https://github.com/krizanact/RestApiC-/
- Kada se projekt klonira pokrenuti file ProjectApi.sln unutar Project.API foldera(npr. s Visual Studio ili VsCode)
- Pokrentu Project.API projekt gdje ćemo dobiti index stranicu sa svim metodama i opisima metoda


### Ostale napomene
- Potrebno pokrenut api/Login metodu da bi dobili token koji ćemo koristiti za pristup svim ostalim metodama, koristiti podatke za kreiranog admina username: krizanactoni691@gmail.com i password: Toni123!
- Moguće dodati i još korisnika nakon prijave s adminom s metodom api/user/createUser, moguće role za novog korisnika su Admin i Obični korisnik, Admin ima ovlasti da pokrene bilo koju metodu s liste, obični korisnik ima ovlast da pokrene samo metode koje vraćaju pojedinačnu novost ili sve novosti
