# Prompt NotebookLM — Vidéo pédagogique: Module Panier (Cart) avec pont vers Order

Objectif
- Produire une vidéo claire (8–12 min) en français expliquant le module Panier (Cart) et son intégration avec la création de commande (Order).
- Montrer le passage des données à chaque couche (Controller → Service → Mapping/DTO → Repository/DbContext → Views/ViewModels), puis le pont Cart → Order au checkout.
- Expliquer précisément les DTOs et ViewModels utilisés, ainsi que la logique de la fonction d’Upsert du panier.

Contexte à analyser automatiquement (fichiers du repo)
- Controllers/CartController.cs
- Application/Cart/Services/CartService.cs
- Application/Cart/Dtos/CartDetailsDto.cs
- Models/Cart/CheckoutCartViewModel.cs
- Mapping/Cart/CartMappingProfile.cs
- Infrastructure/Persistence/ApplicationDbContext.cs
- Application/Orders/Dtos/OrderHeaderDto.cs
- Application/Orders/Dtos/OrderDetailsDto.cs
- Application/Orders/Mapping/OrderMappingProfile.cs
- Application/Orders/Services/OrderServices.cs
- Infrastructure/Persistence/Repositories/OrderRepository.cs
- Views/Cart/Index.cshtml
- Views/Cart/Checkout.cshtml
- Views/Order/OrderDetail.cshtml
- Core.Entities: Cart (CartHeader, CartDetails), Orders (OrderHeader, OrderDetails)

Audience et style
- Audience: développeurs .NET débutants/intermédiaires.
- Style: pédagogique, concret, avec schémas et extraits de code. Voix off en français, rythme dynamique, annotations à l’écran.

Structure attendue de la vidéo (scénario + visuels)
1) Introduction (30–45s)
   - Présenter l’architecture en couches du projet.
   - Objectif du module Cart et sa relation avec Order (au moment du checkout).
2) Architecture et entités (1–2 min)
   - Expliquer les entités CartHeader/CartDetails et OrderHeader/OrderDetails.
   - Montrer comment elles sont exposées dans ApplicationDbContext (DbSet<>).
   - Schéma entité-relation simplifié (User → CartHeader → CartDetails → Product).
3) DTOs et Mapping (1–2 min)
   - Expliquer le rôle des DTOs côté application: CartDetailsDto, OrderDetailsDto, OrderHeaderDto.
   - Montrer CartMappingProfile et OrderMappingProfile (ex.: mapping des prix, ProductName/ImageUrl pour l’affichage, ignore des navigations pour éviter EF updates involontaires).
   - Diagramme “Source → Mapping → Destination” pour 2–3 cas concrets.
   - 3.b) Pourquoi un CartDto (≈1 min)
     - Agrégat du panier (CartHeader + CartDetails) exposé comme contrat inter-couche.
     - Séparation nette entre entités EF et ViewModels UI: réduire le couplage et stabiliser la surface d’API du service.
     - Centraliser les transformations via AutoMapper; testabilité et maintenabilité accrues.
     - Prépare l’exposition API et le versionnage: `CartDto` transporte précisément les données nécessaires.
     - Exemples concrets dans le repo: mapping `CartDto -> CartIndexViewModel` et `CartDto -> CheckoutCartViewModel`; `CartService` produit/consomme `CartDto`.
     - Consigne NotebookLM: détecter la définition exacte de `CartDto` (si le nom de fichier diffère) et citer son code et ses propriétés.
4) ViewModels et Views (1–2 min)
   - Expliquer CheckoutCartViewModel et son utilisation dans Views/Cart/Checkout.cshtml.
   - Montrer l’affichage des items, prix unitaires/total, images de produit.
5) Contrôleur Cart et flux de données (2–3 min)
   - Parcourir CartController.cs: de l’action vers le service.
   - Décrire le passage des données: Model (ViewModel) → DTO via AutoMapper → Service → Repository/EF → retour View.
   - Séquence animée (Sequence diagram) de l’ajout au panier et du checkout.
6) Logique d’Upsert du panier (2–3 min, coeur de la vidéo)
   - Dans CartService.cs, identifier la méthode d’upsert (chercher “Upsert”/“UpsertCart”).
   - Expliquer la décision: créer CartHeader s’il n’existe pas, puis créer/mettre à jour CartDetails par (UserId, ProductId).
   - Incrémenter Count si déjà présent, sinon insérer une nouvelle ligne.
   - Recalculer ou préparer les totaux côté header si applicable; valider les prix unitaires (depuis Product) vs prix calculés.
   - Pseudocode + surlignage des lignes clés depuis le code réel.
7) Pont Cart → Order au Checkout (1–2 min)
   - Montrer le mapping Cart → Order dans le checkout: CartHeaderDto → OrderHeaderDto, CartDetailsDto → OrderDetailsDto.
   - Expliquer la normalisation des prix unitaires, l’affectation de l’OrderHeaderId, et l’ignore des navigations EF.
   - Affichage OrderDetail: nom et image du produit via OrderMappingProfile.
8) Erreurs, sécurité, bonnes pratiques (45–60s)
   - Expliquer l’amélioration des messages d’erreur (inner exception), et vérif d’auth (ClaimTypes.NameIdentifier).
   - Conseils de test: smoke-test complet Cart → Checkout → Order → OrderDetail.
9) Conclusion (20–30s)
   - Récap du flux et des points d’attention (mapping, upsert, DTO/ViewModel).
   - Ouverture: prochaine vidéo sur le module Order en profondeur.

Exigences pour les visuels et la narration
- Générer un script voix off en français, segmenté par scènes, avec timing estimatif.
- Fournir un storyboard: pour chaque scène, liste des visuels (code extrait, schéma, animation de flèches) et textes à l’écran.
- Inclure 2 diagrammes clés:
  1) Diagramme de séquence “Add to Cart / Upsert”.
  2) Dataflow Cart → Order au checkout.
- Ajouter extraits de code commentés provenant directement des fichiers listés (rechercher les méthodes et mappings dans le repo; éviter toute invention).
- Mettre en évidence:
  - CartMappingProfile: mapping des prix unitaires depuis Product.
  - OrderMappingProfile: mapping ProductName/ImageUrl, ignore des collections/navigations au bon endroit.
  - CartController.Checkout: passage ViewModel → DTO → Service et normalisation des prix.
  - CartService: la méthode Upsert (décision créer/mettre à jour, incrément de Count).
  - CartDto: agrégat du panier (header + details), contrat inter-couche; mapping vers CartIndexViewModel/CheckoutCartViewModel; rôle dans CartService.
- Insérer des encadrés “Bonnes pratiques” (AutoMapper, EF Core, DTO vs ViewModel, validations).

Contraintes et consignes de production
- Respecter les noms de classes/propriétés/fichiers tels qu’ils apparaissent dans le code; ne rien inventer.
- Si un nom exact diffère, détecter automatiquement le bon nom dans le repo et l’utiliser.
- Citer les extraits avec contexte minimal (quelques lignes avant/après) et expliquer en français.
- Prévoir sous-titres FR + version du script prête pour TTS.
- Inclure une liste d’assets simples (icônes panier, produit, utilisateur) et styles d’annotation (couleurs, surbrillance code).

Livrables attendus
- Script vidéo complet (FR) avec timestamps.
- Storyboard scène par scène + liste d’assets.
- 2 diagrammes (sequence + dataflow) en mermaid ou description prête à dessiner.
- Pack d’extraits de code annotés (copiés du repo, non modifiés).
- Résumé “cheat sheet” du flux Cart et de l’Upsert.

Note
- Si une méthode “Upsert” porte un autre nom, analyser CartService.cs pour trouver la logique équivalente (ex.: création du CartHeader si absent, ajout/mise à jour du CartDetails par ProductId pour l’utilisateur) et l’expliquer.
