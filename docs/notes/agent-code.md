# Référence agent — Structure et C#

À lire avant de créer, modifier ou relire du C#. Ces conventions restent obligatoires.

## Structure du projet Unity

Organisation **par fonctionnalité**, sur le modèle de `Component/<Feature>/` du dépôt Stuffy (nommée ici `Features/` pour éviter la confusion avec les composants Unity). Arborescence complète : cahier des charges §5.1.

```
Assets/
  _Project/
    Core/         Scripts/ Data/ Prefabs/        (GameSession, GameEvents, Layers, SceneFlow)
    Features/
      <Feature>/  Scripts/ Prefabs/ Data/ Art/ Animations/ Audio/   (seulement les sous-dossiers utiles)
    Scenes/       Boot, MainMenu, Mission, TestBed (TestBed exclue du build)
    Settings/     URP/ (UniversalRP, Renderer2D), Input/ (AsharControls.inputactions), Mixer
    Editor/       PixelArtImportPostprocessor, scripts de mise en place (menu Ashar/Setup/<ID>)
    Tests/        EditMode/
  ThirdParty/     packs achetés + _Modified/  → IGNORÉ PAR GIT
```

Règles :
- Aucun fichier à la racine de `Assets/` ni de `_Project/`. Aucun dossier vide versionné. Aucun dossier « Misc », « Temp », « New Folder ».
- Un script vit dans la fonctionnalité qui le possède. S'il sert à plusieurs fonctionnalités, il va dans `Core/` ou dans `Features/Combat/` (dégâts, projectiles).
- Noms de dossiers et de fichiers en PascalCase anglais, sans espace.
- Assembly definitions : `Ashar.Runtime` (racine de `_Project/`), `Ashar.Editor` (`Editor/`), `Ashar.Tests.EditMode` (`Tests/EditMode/`). Le namespace `Ashar.Editor` masque la classe `UnityEditor.Editor` : dans un inspecteur personnalisé, écris `UnityEditor.Editor` en entier.

## Conventions C# (anglais, pédagogique)

**Nommage**
- Namespace racine `Ashar`, puis la fonctionnalité : `Ashar.Player`, `Ashar.Enemies`, `Ashar.Core`…
- MonoBehaviour de gameplay : suffixe `Controller` (`PlayerDashController`). UI : préfixe `UI` (`UIHudController`, `UIDialogueBubbleView` pour un affichage pur). ScriptableObject : suffixe `Data` (`EnemyData`). Classe statique ou C# pur : pas de suffixe (`GameEvents`, `Layers`).
- PascalCase pour les types, méthodes, propriétés et événements (`OnPlayerHit`) ; `_camelCase` pour les champs privés ; `camelCase` pour les paramètres et variables locales.
- Les noms de classes cités ailleurs dans le cahier des charges sont indicatifs : applique toujours ces suffixes.

**Commentaires : en anglais, informatifs et pédagogiques**
- Chaque classe a un en-tête `/// <summary>` qui explique **ce qu'elle fait, comment, et pourquoi ce choix**, avec si utile les blocs `RESPONSIBILITIES`, `HOW IT WORKS`, `PATTERN` (style du `NarrativeManager` de Snake_2D).
- Chaque méthode (y compris `Awake`, `OnEnable`, `Update`…) a un `/// <summary>` d'une ligne.
- Chaque champ privé a un commentaire en fin de ligne ; chaque champ sérialisé a un `[Tooltip]` en anglais, pour que l'Inspector se lise sans ouvrir le code.
- Les champs sont regroupés par `[Header("...")]` ; l'état runtime utile au débogage est exposé en lecture seule sous `[Header("Debug")]`.
- Un commentaire explique le **pourquoi**, jamais la paraphrase du code. Une formule ou une astuce Unity non évidente (ordre d'exécution, `Time.unscaledDeltaTime`, normalisation des diagonales) est expliquée pour un lecteur débutant.

**Architecture**
- Un MonoBehaviour = une responsabilité. Pas de `Find*`, de `GetComponent` ni d'allocation dans `Update`.
- Communication découplée par un **bus d'événements statique `GameEvents`** (le motif `EventSystem` de Stuffy, renommé pour ne pas entrer en conflit avec `UnityEngine.EventSystems.EventSystem` utilisé par l'UI). Chaque événement est commenté. On s'abonne dans `OnEnable`, on se désabonne dans `OnDisable`.
- Un seul point d'accès global à l'état de partie : `GameSession` (état de jeu, mission, score, vies).
- Pas de code mort, pas de `Debug.Log` laissé hors d'un bloc `[Conditional("UNITY_EDITOR")]` ou d'un bool de debug sérialisé.

**Exemple du style attendu**

```csharp
namespace Ashar.Player
{
    /// <summary>
    /// Short burst of movement that makes the ship briefly invulnerable.
    /// HOW IT WORKS: on input, the ship travels dashDistance over dashDuration,
    /// then the dash goes on cooldown. Values come from PlayerShipData so they
    /// can be tuned in the Inspector without touching code.
    /// </summary>
    public class PlayerDashController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField, Tooltip("Tuning values shared by all player components.")]
        private PlayerShipData _shipData;

        [Header("Debug")]
        [SerializeField, Tooltip("Read-only: true while the dash grants invulnerability.")]
        private bool _isInvulnerable;

        private float _cooldownTimer; // Seconds left before the next dash is allowed.

        /// <summary>True while the dash protects the ship from damage.</summary>
        public bool IsInvulnerable => _isInvulnerable;
    }
}
```

