# Extension Formation - Refactoring Full Stack MVC vers API + Client MVC (Clean Architecture)

## Objectif
Ce dépôt correspond à une **extension de formation** qui montre comment refactoriser un projet e-commerce initialement livré en **Full Stack ASP.NET Core MVC** (monolithique) vers une architecture **2 applications** :

- **Backend** : une application **ASP.NET Core Web API**
- **Frontend** : une application **ASP.NET Core MVC (Client)** qui consomme l’API

L’objectif pédagogique est de **valoriser la Clean Architecture** en montrant que l’on peut **remplacer uniquement la couche Présentation** sans réécrire les couches métier et infrastructure.

---

## Contexte (avant → après)

### Avant : Full Stack MVC (monolithique)
 Le projet initial est un **monolithe ASP.NET Core MVC** (UI + backend dans la même application).
 
 Il sert de point de départ à la formation : l’objectif est de le **découpler** en une API + un client MVC, tout en conservant les couches Clean Architecture.

Architecture (simplifiée) :

- Présentation : **ASP.NET Core MVC** (Controllers + Views)
- Couches Clean Architecture :
  - `Core` (Domain)
  - `Application` (Use cases / Services)
  - `Infrastructure` (Persistence / External services)

### Après : API + Client MVC
Le refactoring aboutit à 2 applications séparées :

- **API** :
  - `./Projet API`
- **Client MVC (UI)** :
  - `./Projet CLient MVC`

Idée clé :

- **On conserve** `Core`, `Application`, `Infrastructure` côté serveur
- **On remplace** la Présentation MVC par une Présentation API
- Le **Client MVC** garde la même UI (Views) et consomme l’API via des services HTTP

---

## Ce qui ne change pas (Clean Architecture)
Les couches suivantes restent le cœur du système et sont réutilisées :

- **Core**
  - Entités, interfaces, règles métier
- **Application**
  - DTOs, services (use cases), mapping AutoMapper côté application
- **Infrastructure**
  - EF Core, repositories, email sender, gestion fichiers, etc.

En pratique, les registrations DI `AddApplicationRegistration()` et `AddInfrastructureRegistration()` sont partagées et identiques.

---

## Ce qui change : la couche Présentation

### Présentation MVC (avant)
- Controllers MVC qui retournent des `View(...)` / `RedirectToAction(...)`
- Auth typiquement basée sur cookies côté MVC
- Gestion des erreurs orientée pages (ex: page erreur)

### Présentation API (après)
- Controllers API exposant des endpoints `api/...`
- Réponses JSON standardisées (ex: `ApiResponse<T>`) + codes HTTP (`200/201/400/401/403/404`)
- Auth via **JWT Bearer**
- CORS + Swagger
- Middleware global d’exception pour uniformiser les erreurs JSON

---

## Architecture cible (schéma)

```text
            (Browser)
               |
               v
      +-------------------+
      | Client MVC (UI)    |
      | Views + Controllers|
      | Services HTTP      |
      +---------+---------+
                |
                | HTTP (JSON / multipart)
                v
      +-------------------+
      | Web API (Backend) |
      | Controllers API    |
      +---------+---------+
                |
                v
  +------------------------------+
  | Application / Core / Infra   |
  | Services / Domain / EF Core  |
  +------------------------------+
```

---

## Stratégie d’authentification (simple pour débutants)

Dans cette extension, on utilise une stratégie volontairement simple côté client :

- L’API authentifie via **JWT**
- Le **Client MVC stocke le JWT dans un cookie “non-auth”**
- Les services HTTP du client ajoutent l’en-tête :
  - `Authorization: Bearer <token>`

Recommandations (même pour un projet débutant) :

- Cookie : `HttpOnly = true`
- Cookie : `Secure = true` en HTTPS
- Cookie : `SameSite = Lax`

L’API reste la **source de vérité** : même si l’UI masque des boutons, l’API doit garder `[Authorize]` / `[Authorize(Roles = "Admin")]`.

---

## Étapes de refactoring (guide)

### 1) Extraire / créer le projet API
- [ ] Créer un projet `Web API` (nouvelle Présentation)
- [ ] Référencer `Application` + `Infrastructure` (et indirectement `Core`)
- [ ] Reprendre les registrations DI :
  - [ ] `AddApplicationRegistration()`
  - [ ] `AddInfrastructureRegistration()`
- [ ] Configurer :
  - [ ] EF Core + connection string
  - [ ] Identity (si nécessaire)
  - [ ] JWT Bearer
  - [ ] Swagger
  - [ ] CORS
- [ ] Réécrire les controllers MVC en controllers API :
  - [ ] `View(...)` → `Ok(...) / BadRequest(...) / NotFound(...)`
  - [ ] `TempData` → `ApiResponse<T>` + message

### 2) Créer le projet Client MVC (UI)
- [ ] Créer un projet MVC dédié au front
- [ ] Copier les **Views** et **ViewModels** de l’ancien MVC
- [ ] Remplacer la logique des controllers :
  - [ ] plus d’appel direct aux services `Application`
  - [ ] appel aux services HTTP du dossier `Services`

### 3) Implémenter les Services HTTP (Client MVC)
- [ ] 1 service par ressource : `Auth`, `Products`, `Cart`, `Orders`, ...
- [ ] Gérer le token :
  - [ ] écrire cookie au login
  - [ ] lire cookie sur chaque appel
  - [ ] injecter `Authorization: Bearer`
- [ ] Gérer les erreurs :
  - [ ] 401/403 → redirection Login + suppression cookie
  - [ ] 400/404/500 → message utilisateur (TempData)

### 4) Checkout : garder la même UX mais déporter la logique via API
- [ ] `GET Checkout` : charger le panier via API
- [ ] `POST Checkout` : créer la commande via `POST /api/orders`
- [ ] Après succès : vider le panier via API (si non géré côté API)

---

## Structure des dossiers (référence)

### API
- `./Projet API/Formation-Ecommerce-API.sln`
- Projet host : `Formation-Ecommerce-11-2025.API`

### Client MVC
- `./Projet CLient MVC/Formation-Ecommerce-Client/Formation-Ecommerce-Client.sln`
- Dossier clé : `./Projet CLient MVC/Formation-Ecommerce-Client/Services`
- Checklist front : `./Projet CLient MVC/Formation-Ecommerce-Client/front.md`

---

## Lancer les projets (dev)

Pré-requis :
- .NET 8 SDK
- SQL Server (ou configuration équivalente)

Ordre recommandé :

1. Démarrer l’API
2. Démarrer le Client MVC

Notes :
- Vérifier les `appsettings.json` (URLs, connection strings, etc.)
- Vérifier la cohérence `EmailSettings:BaseUrl` selon l’application qui doit afficher les pages de confirmation/reset

---

## Résultat attendu
À la fin, tu obtiens :

- Une API réutilisant tes couches `Core/Application/Infrastructure`
- Un Client MVC **visuellement identique** à l’ancien site
- Une séparation claire :
  - UI = MVC Client
  - Backend = Web API

---

## Licence / Usage formation
Projet destiné à un usage pédagogique (formation / démonstration Clean Architecture).
