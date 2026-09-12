# PristupačneAktivnosti

A directory of venues and activities listed by how accessible they are — wheelchair
access, accessible toilets, entrance ramps, lifts — with search and filtering by venue
type. University project (TVZ, Web Technologies), 2025/26.

Two ASP.NET Core projects: a REST API and a separate MVC web client that talks to it over
HTTP. The split was the point of the assignment, so the API has no views and the client
has no database access.

## Stack

.NET 8 · ASP.NET Core Web API · ASP.NET Core MVC · Entity Framework Core · SQL Server ·
JWT bearer auth · AutoMapper · Swagger

## Structure

```
WebAPI/     REST API — controllers, DTOs, EF Core models, JWT issuing, logging service
WebApp/     MVC client — views and controllers that call the API
db.sql      Schema
```

**API endpoints** are grouped by controller: `Aktivnosti`, `Pristupacnosti`, `Vrste`,
`Recenzije`, `Korisnici`, `Auth`, `Logs`.

**Data model.** An activity has a type (`Vrsta`) and any number of accessibility features
(`Pristupacnost`), joined through `AktivnostPristupacnost` — a many-to-many, because a
venue is usually several kinds of accessible at once. Reviews and users are separate
tables. Every write goes through a logging service that records what changed.

**Auth.** Passwords are stored hashed. Login returns a JWT; the API validates issuer,
audience, lifetime and signing key, and roles separate ordinary users from admins.

DTOs sit between the EF models and the API surface so the database shape isn't the
contract, with AutoMapper doing the translation.

## Running it

1. Create the database and run `db.sql` against it.
2. In `WebAPI/appsettings.json`, set `ConnectionStrings:DefaultConnection` to your SQL
   Server instance and replace `Jwt:Key` with a long random string.
3. Set the CORS origin in `WebAPI/Program.cs` to whatever port the MVC client runs on,
   and `ApiBaseUrl` in `WebApp/appsettings.json` to the API's port.
4. Run both projects.

Swagger is at `/swagger` on the API.
