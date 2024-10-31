# Backend_Harkka

Warning: Read this txt as a file, not in narrow previw window. Line wraping should be OFF

-----------------------
Intro:

Why: This is practice project for cource "AL00CM24-3003 Back-end työkalut" from "LAB - Ammattikorkeakoulu"
When: 02.09.2024 - 3.11.2024
What: // LISÄÄ TEXTI

-----------------------
List of api calls:

Every call requires following ApiKey.
ApiKey: LAIHFOEDFAOFJALOFAJSFHWQ

--- Users Api: ---

GET: api/Users/p_{page}
- Requires Authorization
Gets the information of 20 users in database ordered by username, increasing pagenumber shifts startpoint to later point, does not include users marked as deleted
Parameters: page
Response code: 200 (OK)

GET: api/Users/{username}
- Requires Authorization
Gets user specified by username
Parameters: username
Response code: 200 (OK)

PUT: api/Users/{username}
- Requires Authorization
Update user information, updates last login time.
Parameters: username, user
Response code: 204 (No Content)

POST: api/Users
Create new user
Parameters: user
Response code: 201 (Created)

DELETE: api/Users/{username}/HardDelete
- Requires Authorization
Delete user specified by username
Parameters: username
Response code: 204 (No Content)
Warning: Can cause issues on messages sent by this user

DELETE: api/Users/{username}
- Requires Authorizatin
Wipe user specified by username (Changes username as "DeletedUserX" where X is replaced with User Id, changes following values as null: {FirstName, LastName, Email}, changes IsDeleted value as True), updates last login time.
Parameters: username
Response code: 204 (No Content)

--- Messages Api: ---

GET: api/Messages/p_{pageNumber}
Get 20 public public messages ordered by post time, increasing pagenumber shifts startpoint to later point, does not include messages marked as deleted
Parameters: page
Response code: 200 (OK)

GET: api/Messages/{username}/Sent/p_{pageNumber}
- Requires Authorization
Get 20 messages sent by specified user ordered by post time, increasing pagenumber shifts startpoint to later point, does not include messages marked as deleted
Parameters: username, page
Response code: 200 (OK)

GET: api/Messages/username/Received/p_{pageNumber}
- Requires Authorization
Get 20 messages received by specified user ordered by post time, increasing pagenumber shifts startpoint to later point, does not include public messages or messages marked as deleted
Parameters: username, page
Response code: 200 (OK)

GET: api/Messages/{messageId}
- Requires Authorization
Get Message specified by Id
Parameters: messageId
Response code: 200 (OK)

PUT: api/Messages/{messageId}
- Requires Authorization
Update Message specified by Id, updates EditTime
Parameters: messageId, message
Response code: 204 (No Content)

POST: api/Messages
- Requires Authorization
Create new message, increase senders sent messages and recipients(if included) received messages number by 1 
Parameters: message
Response code: 201 (Created)

DELETE: api/Messages/{messageId}/HardDelete
- Requires Authorization
Delete message specified by id
Parameters: messageId
Response code: 204 (No Content)
Warning: Can break message threads

DELETE: api/Messages/{messageId}
- Requires Authorization
Wipe message specified by id (Changes title and body as "Deleted Message", changes IsDeleted value as True), updates EditTime
Parameters: messageId
Response code: 204 (No Content)

-----------------------
Example User and Message:

Headers:
ApiKey: LAIHFOEDFAOFJALOFAJSFHWQ
Authorization: Basic YWFhOnh5emE=
    (authorization is in style "Basic " + Basic64 encoded "username:password")
    Basic64 encoding from site https://www.base64encode.org/

User:
- automatically filled: UserCreated (DateTime), LastLogin, MessagesSent, MessagesReceived
- automatically filled, not in DTO: Id, Salt, IsDeleted
- required fields: UserName (Lenght between 3 and 25, must be unique and cannot contain word "deleted"), Password (Lenght between 4 and 100)
- optional fields: FirstName, LastName, Email
{
    "username": "aaa",
    "password": "xyza",
    "email": "aaa@g.com",
    "firstname":"fname",
    "lastname":"lname"
}

Message:
- automatically filled: Id, SendTime, EditTime
- automatically filled, not in DTO: IsDeleted
- required fields: Title (Lenght between 1 and 100), Body (Lenght between 1 and 1000), sender(username)
- optional fields: Recipient (username, if null then message is public), prevMessageId
{
    "title": "title",
    "body": "body",
    "sender": "aaa",
    "recipient":"bbb",
    "prevMessageId": 1
}

-----------------------
How to create DataBase:

Go to appsettings.json and locate part:
    "MessageServiceDB": "Server=localhost;Database=MessageServiceDB;Trusted_Connection=True;Encrypt=false;"
You need to change it to to correspond your server if using non local database

If using local database, use powershell "cd .\" to navigate to right folder
Then just run "dotnet ef database update"

-----------------------
How to create User:

Add following header: "ApiKey" and as its value "LAIHFOEDFAOFJALOFAJSFHWQ"
Put body as json and include atleast username and password parameters.
    Username lenght must be between 3 and 25 characters
    Password lenght must be between 4 and 100 characters
Json body may also include following optional fields: email, firstname, lastname
Call "POST: api/Users"

To Use authorized calls
Turn user credentials into style username:password and then encode it using Base64
Now add another header "Authorization" and it's value will be following combination "Basic " followed by your encoded credentials
    

-----------------------
Rakenne:

Tällainen rakenne helpottaa ylläpidon helppoutta ja jatkokehitysmahdollisuuksia:

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
  
-----------------------
Jatkokehitys ideoita:

Admin oikeudet
    Voi lukea muiden käyttäjien viestejä
    Voi poistaa/editoida muiden käyttäjien julkisia viestejä
    Voi poistaa/editoida muita käyttäjiä
    Vaatimus HardDeleteen

UserDeletet poistavat myös kaikki käyttäjän viestit
    Jos hard delete niin silloin hard delete viesteihin, softissa soft delete

HardDelete
    Kuinka viestiketjut reagoi?

Pyydettäessä listoja
    kertoo kun on viimeinen sivu

