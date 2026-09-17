# EquipmentManager

EquipmentManager is een webapplicatie voor het beheren van laboratoriumapparatuur. Je kunt er apparatuur, locaties, categorieën, techniekers en onderhoudsgegevens in opvolgen.

Ik heb dit project gemaakt als oefenproject binnen de graduaatsopleiding Programmeren aan Hogeschool PXL. Het ontwerp is gestart in Figma en daarna verder uitgewerkt als een Vue-applicatie met een ASP.NET Core Web API.

## Login

email:      admin@company.com
password:   password123

## Functionaliteiten

- Inloggen met een beveiligd account
- Apparatuur bekijken, toevoegen, aanpassen en verwijderen
- Apparatuur indelen per categorie en locatie
- Onderhoudsbeurten registreren en opvolgen
- Techniekers aan apparatuur en onderhoud koppelen
- Meldingen tonen en als gelezen markeren
- Overzichten en statistieken bekijken op het dashboard
- Persoonlijke instellingen beheren
- Rechten beperken op basis van de rol van de gebruiker

## Gebruikte technologieën

### Frontend

- Vue 3
- JavaScript
- Vite
- Vue Router
- Pinia
- HTML en CSS

### Backend

- .NET 8
- ASP.NET Core Web API
- Dapper
- SQL Server
- JWT-authenticatie
- BCrypt voor wachtwoorden
- Swagger / OpenAPI

## Opbouw van het project

De backend is verdeeld in verschillende lagen:

- **Domain** bevat de modellen van de toepassing.
- **Application** bevat de services en repository-interfaces.
- **Infrastructure** verzorgt de communicatie met SQL Server via Dapper.
- **WebAPI** bevat de controllers, DTO's en configuratie van de API.

In de frontend zijn de views, herbruikbare componenten, Pinia-stores en API-services van elkaar gescheiden.

## Authenticatie en autorisatie

Aanmelden gebeurt via het volgende endpoint:

```http
POST /api/Auth/login
```

Na een geslaagde login geeft de API een JWT-token terug. De frontend bewaart dit token voor de duur van de sessie en stuurt het mee bij beveiligde aanvragen.

```http
Authorization: Bearer <token>
```

De meeste controllers zijn alleen bereikbaar voor ingelogde gebruikers. Voor bepaalde bewerkingen, zoals het aanpassen of verwijderen van gegevens, is de rol `Admin` vereist.

## API

De meeste onderdelen volgen dezelfde CRUD-structuur:

```text
GET    /GetAll
GET    /GetById/{id}
POST   /Add
PUT    /Update
DELETE /Delete/{id}
```

Via Swagger kunnen de beschikbare endpoints en verwachte gegevens verder bekeken worden.

## Auteur

Lorenzo Trabucco  
Student Graduaat Programmeren aan Hogeschool PXL

GitHub: [LorenzoTrabuccoPXL](https://github.com/LorenzoTrabuccoPXL)
