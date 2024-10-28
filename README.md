# Backend_Harkka

-----------------------
Rajapinnat:

API Key: LAIHFOEDFAOFJALOFAJSFHWQ

GET: api/Users
- Requires Authorization
Gets the information of all users in database

GET: api/Users/{username}
- Requires Authorization
Gets user specified by username

PUT: api/Users/{username}
- Requires Authorization
Update user information

POST: api/Users
Create new user

DELETE: api/Users/{username}/HardDelete
- Requires Authorization
Delete user specified by username
Warning: Can cause issues on messages sent by this user

DELETE: api/Users/{username}
- Requires Authorizatin
Wipe user specified by username

GET: api/Messages/p_{pageNumber}
Get 20 public public messages orderded by post time

GET: api/Messages/{username}/Sent/p_{pageNumber}
- Requires Authorization
Get 20 messages sent by specified user ordered by post time

GET: api/Messages/username/Received/p_{pageNumber}
- Requires Authorization
Get 20 messages received by specified user ordered by post time

GET: api/Messages/{MessageId}
- Requires Authorization
Get Message specified by Id

PUT: api/Messages/{MessageId}
- Requires Authorization
Update Message specified by Id

POST: api/Messages
- Requires Authorization
Create new message

DELETE: api/Messages/{MessageId}/HardDelete
- Requires Authorization
Delete message specified by id
Warning: Can break message threads

DELETE: api/Messages/{MessageId}
- Requires Authorization
Wipe message specified by id

-----------------------
Esimerkki viesti ja käyttäjä:

Headeriin:
apikey: LAIHFOEDFAOFJALOFAJSFHWQ
Authorization: Basic YWFhOnh5emE=
(authorization on muotoa "Basic " + Basic64 koodattu"username:password")
Basic64 koodaus esim sivulta https://www.base64encode.org/

Käyttäjä:
- required fields: UserName (Lenght between 3 and 25), Password (Lenght between 4 and 100)
- optional fields: FirstName, LastName, Email
{
    "username": "aaa",
    "password": "xyza",
    "email": "aaa@g.com",
    "firstname":"fname",
    "lastname":"lname"
}

Viesti:
- required fields: Title (Lenght between 1 and 100), Body (Lenght between 1 and 1000), sender(username)
- optional fields: Recipient (username, if null then message is public)
{
    "title": "title",
    "body": "body",
    "sender": "aaa",
    "recipient":"bbb"
}

-----------------------
Rakenne:

Controllers - Viestin käsittelijät, kutsuu myös authorisaatio ja claimi tarkistuksia. Kutsuvat omasta servicelle viestin mukaista async taskia ja Palauttaa statuksesta kertovan koodin sekä onnistuessa muut kutsutut tiedot
> MessagesController.cs
> UsersController.cs

Middleware
> ApiKeyMiddleware.cs
  - Tarkistaa apiavaimen
> BasicAuthenticationHandler.cs
  - Luo claimin ja käsittelee authorizaation
> UserAuthenticationService.cs
  - Muuntaa salasanan salattuun muotoon, tarkistaa että salasana on oikein taikka tarkistaa oikeudet viestiin

Models
> Message.cs
  - Viesti ja sen Dto luokat
> MessageServiceContext.cs
  - Contexti luokka
> User.cs
  - User ja sen Dto luokat

Repositories - Tekee tietokantapyynnöt, eli hoitaa kyseiseen tietokantaan tarvittavat lisäykset/päivitykset
> IMessageRepository.cs
> IUserRepository.cs
> MessageRepository.cs
> UserRepository.cs

Services - Välittää kutsun eteenpäin omalle Ropositorylle, tekee tarvittavat muutokset viestin sisältöön, Myöskin suurinosa automatisoiduista tietueista lisätään näissä (esim. timestampit)
> IMessageService.cs
> IUserService.cs
> MessageService.cs
> UserService.cs

Program.cs
  - luo builderin, käynnistää builder servicet, middlewaret ja lopuksi käynnistää appin
appsettings.json
  - Sisältää apikey:n, database connection ja allowedhostit

-----------------------
Rakenteen selostus esimerkillä (oletetaan että esimerkissä on annetut tiedot oikein eli: oikea username, passu, apikey sekä sivunumero):

GET: api/Messages/aaa/received/p_2

> MessagesController.cs
Ensimmäisenä kutsun jälkeen on [Authorization] eli tarkistetaan oikeudet.
  > BasicAuthenticationHandler.cs
  Ensin tarkistetaan puuttuuko apiavain ja sen jälkeen onko apiavain oikein.
  Seuraavana otetaan basic64:nä annetusta autentikaatiosta erileen username ja password ja lähetetään ne userautentikaatioserviseen
    > UserAuthenticationService.cs
    Tarkistetaan salasana: Ensin haetaan käyttäjän tiedot käyttäen usernamea
      > UserRepository.cs
      Palautetaan useri (tai jos ei löytynyt niin null)
    Tarkistetaan löytyikö käyttäjä ja ettei sitä ole merkattu poistetuksi
    Tämän jälkeen muutetaan kirjautumiseessa annettu salasana salattuun muotoon käyttäen käyttäjän suolaa (luotu randomilla kun käyttäjä on luotu) ja sha256 salausta
    Tarkistetaan onko salattu salasana sama kuin käyttäjän tietokantaan tallennettu salattu salasana
    Jos onnistunut tunnistautuminen niin palautetaan Useri
  Tarkistetaan ettei palautettu useri ole nulli
  Tämän jälkeen luodaan käyttäjästä claimi
  Palautetaan onnistunut autentikaatio tiketti
Tarkistetaan että viestin osoitteen username täsmää claimin usernameen
Kutsutaan messageServicen async taskia GetMyReceivedMessagesAsync
  > MessageService.cs
  Välitetään viestipyyntö repositorylle
    > MessageRepository.cs
    Haetaan repositorystä niiden messageiden määrä joissa annettu käyttäjä on vastaanottajana
    ConfirmPagella tarkistetaan että annettu sivunumero on validi (jos on annettu liian iso tai alle 1 sivunumerona niin se pakotetetaan sopivaksi)
    Tässä tapauksessa palautetaan kaikki viestit, joissa vastaanottajana on annettu käyttäjä. Viestit on aikajärjestyksessä (tuorein viesti ensimmäisenä) ja palautetut viestit ovat numerot 21 to 40. Viesteihin sisällytetään lähettäjän tiedot.
  Luodaan lista johon lisätään loopilla kaikki palautuneet viestit sen jälkeen kun ne on convertoitu dto:ksi (conversiossa senderi ja recipient muutetaan User olioista pelkiksi usernameiksi)
  Palautetaan kyseinen lista
Palautetaan lista ja annetaan Koodi "Ok" joka muuttuu numeroksi 200
  





