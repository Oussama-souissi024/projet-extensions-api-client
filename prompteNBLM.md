# Prompt pour NotebookLM : Transition Architecture MVC vers Architecture Client-Serveur (N-Tiers)

**Rôle :** Tu es un Architecte Logiciel Senior et un Expert Pédagogue en .NET.

**Tâche :** Génère un script pour une vidéo de formation explicative.
**IMPORTANT :** Le titre et le sujet doivent être **"Architecture Client-Serveur avec API REST"** ou **"Architecture Distribuée"**. 
⚠️ **NE PAS utiliser le terme "Microservices"**. Nous sommes dans une étape intermédiaire (Architecture N-Tiers) : une seule API centrale et un Client. Ce n'est pas encore des microservices multiples.

## 1. Le Contexte du Projet (La "Big Picture")
Nous transformons une application E-commerce monolithique (**ASP.NET Core MVC Full Stack**) en une **Architecture N-Tiers Moderne**.

**DISCOURS POSITIF IMPORTANT :**
⚠️ **NE JAMAIS CRITIQUER** le projet initial. Au contraire, explique que grâce à la **Clean Architecture** (Architecture en Oignon) du projet de départ, la transition a été facile.
*   Nous avons conservé intactes les couches `Core` (Métier) et `Infrastructure` (Données).
*   Nous avons simplement **remplacé** la couche "Présentation MVC" par une couche "Présentation API".
*   C'est la preuve d'une architecture robuste et modulaire : **"Plug & Play"**.

*   **Avant (Monolithe) :** Une seule application bien structurée.
*   **Après (Client-Serveur) :** Deux applications distinctes, toujours basées sur le même cœur métier éprouvé.
    1.  **Backend (Web API) :** L'ancienne couche présentation est remplacée par une API REST. Elle réutilise 100% de la logique existante.
    2.  **Frontend (Client MVC) :** Une nouvelle interface légère qui consomme l'API.

**Analogie clé :** Utilise l'image d'un restaurant.
*   *Monolithe :* Le chef (Backend) sort de la cuisine pour servir le client à table.
*   *Architecture Distribuée :* Le chef reste en cuisine. Il prépare les plats (Données/JSON) et les pose au passe-plat (API). C'est un serveur (Le Client MVC) qui prend le plat et l'amène au client. Si demain on veut faire de la livraison à domicile (App Mobile), le chef n'a rien à changer, c'est juste un nouveau livreur qui vient chercher les plats.

## 2. Analyse du Code Source (Source Material)
**INSTRUCTION CRITIQUE :** Dans le script, tu DOIS lire et expliquer ce code ligne par ligne. Ne l'ignore pas.
Fais comme si tu montrais ton écran aux étudiants : "Regardez ici, dans le `CategoriesController`, nous avons..."

Utilise ces extraits de code RÉELS de notre projet pour ancrer tes explications techniques.

### A. Le Cerveau : L'API (`CategoriesController.cs`)
Ce contrôleur ne retourne plus de HTML (Vues), mais des données brutes (JSON). Il protège aussi l'accès.

```csharp
// CHEMIN : Projet API/Controllers/CategoriesController.cs
[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    // 1. EXPOSITION DES DONNÉES
    // Répond à GET /api/categories
    // Retourne du JSON (ApiResponse<IEnumerable<CategoryDto>>)
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        // Appelle le service métier existant (accès BDD)
        var categories = await _categoryService.ReadAllAsync();
        return Ok(new ApiResponse<IEnumerable<CategoryDto>> 
        { 
            Success = true, 
            Data = categories 
        });
    }

    // 2. SÉCURITÉ (Le point critique)
    // L'attribut [Authorize] est le gardien.
    // Il vérifie la présence du Token JWT envoyé par le client.
    [HttpPost]
    [Authorize] 
    public async Task<IActionResult> Create([FromBody] CreateCategoryDto createCategoryDto)
    {
        var result = await _categoryService.AddAsync(createCategoryDto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ...);
    }
}
```

### B. La Vitrine : Le Client MVC (`CategoryController.cs`)
Ce contrôleur est "bête" techniquement : il ne sait pas où est la BDD. Il sait juste parler à l'API.

```csharp
// CHEMIN : Projet Client MVC/Controllers/CategoryController.cs
[AuthorizeApi] // Notre propre filtre pour gérer le token côté client
public class CategoryController : Controller
{
    // Interface qui masque les appels HTTP (HttpClient)
    private readonly ICategoryApiService _categoryService;

    public async Task<IActionResult> CategoryIndex()
    {
        // 1. Appel réseau vers l'API
        // Le client demande : "Envoie-moi la liste en JSON"
        var categories = await _categoryService.GetAllAsync();
        
        // 2. Transformation pour l'humain
        // Le client prend le JSON et l'injecte dans la Vue HTML
        return View(categories);
    }
}
```

## 3. Scénario Détaillé à Expliquer

### Le Voyage de la Donnée (Data Flow)
Retrace le chemin complet pour afficher la liste des catégories :
1.  **Base de Données (SQL Server) :** La donnée brute réside ici.
2.  **API :** `CategoriesController` (API) interroge la BDD via Entity Framework. Il transforme l'entité SQL en un objet de transport léger (`CategoryDto`).
3.  **Réseau :** La donnée voyage sous forme de **JSON**. C'est le langage universel. (Pas de HTML sur le réseau !).
4.  **Client MVC :** `CategoryController` (Client) reçoit ce JSON.
5.  **Navigateur :** Le Client génère finalement le HTML via `View()` pour que l'utilisateur voie un joli tableau.

### Le "Pass VIP" : Le Token JWT
C'est le concept le plus important pour la sécurité dans cette architecture.
*   **Le Problème :** L'API est "Stateless" (sans état). Elle ne se souvient pas que "Oussama" s'est connecté il y a 5 minutes sur le site Client.
*   **La Solution :** Le Token JWT.
    *   Au login, l'API génère un Token (chiffre, sécurisé) et le donne au Client.
    *   Pour créer une catégorie (Action `Create`), le Client **DOIT** attacher ce Token à sa requête (dans le Header `Authorization`).
    *   L'attribut `[Authorize]` côté API agit comme un videur de boîte de nuit : "Pas de Token ? Tu rentres pas (Erreur 401)".
    *   Cela garantit que même si le Backend et le Frontend sont sur des serveurs différents à l'autre bout du monde, la sécurité est totale.

## 4. Conclusion
Terminez en expliquant pourquoi on fait tout ça :
*   Pour séparer les responsabilités (Backend = Logique / Frontend = Design).
*   Pour pouvoir créer demain une Application Mobile qui branchera sur la MÊME API sans rien recoder côté serveur.
