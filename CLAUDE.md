# CLAUDE.md — An Astral Story : Ashar

Shoot'em up narratif 2D à scroll vertical, pixel art, Unity 6 (URP, C#), PC Windows, export Itch.io.
Projet solo : Dyllan conçoit, teste et valide ; Claude Code implémente. Démo de 2 missions (M1 Phobos IX, M2 La course hors de Mars),
développée du 1er octobre au 25 novembre 2026. Finalité : **portfolio**. Le code et le projet Unity seront lus par des recruteurs :
ils doivent être impeccables, au niveau des dépôts de référence de Dyllan (`Stuffy_Infinite_Runner`, `Snake_2D`).

## Documents de référence (dossier `docs/`)

| Fichier | Rôle pour toi |
| --- | --- |
| `docs/CAHIER_DES_CHARGES.md` | **Ta source principale** : architecture, données, comportements, valeurs. |
| `docs/specs/<ID>.md` | La story en cours. Tu ne travailles que sur elle. |
| `docs/Game_Concept.md` | Fait foi sur le design. Si le cahier des charges le contredit, arrête-toi et signale-le. |
| `docs/Fiche_boss_LEASH.md` | Spécification du boss (stories E5). |
| `docs/Bestiaire_v2.md`, `docs/Fiche_vaisseau_v2.md` | Contexte de design uniquement. **Leurs valeurs d'échelle et de vitesse sont obsolètes** : utilise exclusivement celles du cahier des charges. |
| `docs/backlog.csv` | IDs des stories (P0-xx, E1-xx…). |

## Règles non négociables

1. **Une story à la fois, puis stop.** Tu implémentes la spec donnée, rien de plus. Quand elle est terminée, tu remplis le compte rendu, tu commites sur la branche de la story et **tu t'arrêtes** : Dyllan teste et valide. Tu ne fusionnes jamais dans `main` et tu n'enchaînes jamais sur la story suivante sans qu'il le demande. Une idée ou un besoin hors périmètre va dans `docs/notes/idees.md`, sans être codé.
2. **Si tu ne peux pas répondre au besoin, tu t'arrêtes et tu le dis** (voir « Blocage » plus bas). Tu ne simules jamais un succès, tu n'affaiblis jamais un critère d'acceptation, tu ne désactives jamais un test pour le faire passer, et tu ne contournes jamais un obstacle par une bidouille (édition YAML d'une scène, valeur en dur, suppression d'une fonctionnalité).
3. **Les réglages manuels de Dyllan sont sacrés.** Toute valeur ajustable est exposée dans un ScriptableObject ou dans l'Inspector, avec un `[Tooltip]`. Un script de mise en place crée un asset **uniquement s'il n'existe pas** : il ne réécrit jamais un `.asset`, un prefab ou une scène existants. Si une story doit changer une valeur déjà réglée, propose-le dans le compte rendu au lieu de le faire.
4. **Aucun service en ligne** : pas de serveur, d'analytics, de multijoueur, de leaderboard en ligne, de SDK réseau.
5. **Données pilotées** : toute valeur d'équilibrage vit dans un ScriptableObject ; tout texte visible passe par la table de chaînes ou par Ink. Rien en dur dans le code.
6. **Prototype d'abord, mais propre** : `Instantiate`/`Destroy`, MonoBehaviour simples, `enum` + paramètres. Pas de pooling, de système de patterns générique, de machine à états générique, d'injection de dépendances ni d'ECS tant qu'une mesure (story E2-01) ou la spec ne le demande pas. Simple ne veut pas dire négligé : les conventions ci-dessous s'appliquent dès la première ligne.
7. **Tu n'écris jamais de contenu narratif** (dialogues, codex, noms de lieux) : utilise des placeholders `[TODO-TEXTE: description]`.
8. **Lisibilité** : projectiles joueur argent-blanc, ennemis rouge orangé, boss LEASH magenta et jaune ; jamais plus de 3 couleurs de projectiles par phase de boss.
9. **Un dialogue est une pause narrative, jamais un gel du jeu.** Pendant un dialogue : actions ennemies en attente, vagues suspendues, décor qui défile, musique qui continue, vaisseau en **pilote automatique** (le joueur ne le contrôle plus) ; le bouton de tir fait avancer, et fermer la fenêtre de dialogue rend la main. **N'utilise jamais `Time.timeScale = 0` pour un dialogue** (réservé au menu pause). Aucun dialogue pendant l'action. Une voix off absente ne doit jamais provoquer d'erreur. Détails : cahier des charges §7.7.
10. **Ne modifie pas `docs/`**, sauf `docs/notes/` et le fichier de spec en cours (section « Compte rendu »).
11. **N'édite jamais à la main** les fichiers `.unity`, `.prefab` ou `.asset`. Passe par l'éditeur (Unity CLI ou serveur MCP Unity s'ils sont configurés) ou par un script d'éditeur de mise en place idempotent (menu `Ashar/Setup/<ID>`), que Dyllan lance en un clic.
12. **Assets achetés : jamais sur GitHub.** Tout `Assets/ThirdParty/` est ignoré par Git (packs d'origine **et** leurs retouches : les licences interdisent de redistribuer même une version modifiée). Tu ne modifies jamais un fichier d'origine ; une version retouchée va dans `Assets/ThirdParty/_Modified/<Pack>/`. Seuls les sprites, sons et polices **entièrement faits maison** vont dans `Assets/_Project/`. N'utilise jamais `git add -f` ni `git add .` sans vérifier `git status`.

## Stack

Unity 6 (6000.x), URP avec 2D Renderer, Pixel Perfect Camera URP (`UnityEngine.Rendering.Universal`, **ne pas installer** `com.unity.2d.pixel-perfect`), Input System, TextMeshPro, Ink (ink-unity-integration, inkle), Unity Test Framework. Aucun autre package sans accord.

## Structure du projet Unity

Organisation **par fonctionnalité**, sur le modèle de `Component/<Feature>/` du dépôt Stuffy (nommée ici `Features/` pour éviter la confusion avec les composants Unity). Arborescence complète : cahier des charges §5.1.

```
Assets/
  _Project/
    Core/         Scripts/ Data/ Prefabs/        (GameSession, GameEvents, Layers, SceneFlow)
    Features/
      <Feature>/  Scripts/ Prefabs/ Data/ Art/ Animations/ Audio/   (seulement les sous-dossiers utiles)
    Scenes/       Boot, MainMenu, Mission, TestBed
    Settings/     URP, Input Actions, Quality, Mixer
    Editor/       scripts de mise en place (menu Ashar/Setup/<ID>)
    Tests/        EditMode/
  ThirdParty/     packs achetés + _Modified/  → IGNORÉ PAR GIT
```

Règles :
- Aucun fichier à la racine de `Assets/` ni de `_Project/`. Aucun dossier vide versionné. Aucun dossier « Misc », « Temp », « New Folder ».
- Un script vit dans la fonctionnalité qui le possède. S'il sert à plusieurs fonctionnalités, il va dans `Core/` ou dans `Features/Combat/` (dégâts, projectiles).
- Noms de dossiers et de fichiers en PascalCase anglais, sans espace.

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

## Conventions Unity (projet propre dans l'éditeur)

- **Tout ce qui apparaît en jeu est un prefab.** Aucun sprite glissé directement dans une scène, aucun GameObject de gameplay qui n'est pas une instance de prefab.
- **Structure d'un prefab** : la racine porte la logique et le collider ; un enfant `Visual` porte le `SpriteRenderer` (et l'`Animator`). Changer de sprite ne casse jamais la logique.
- **Variantes** : une unité qui dérive d'une autre est une *Prefab Variant* (ex. `Interceptor` variante de `PatrolFighter`). Les parties d'un boss sont des prefabs imbriqués.
- **Hiérarchie des scènes** : des GameObjects vides servent de sections, dans cet ordre : `--- SYSTEMS ---`, `--- CAMERA ---`, `--- ENVIRONMENT ---`, `--- GAMEPLAY ---`, `--- UI ---`. Les objets créés en jeu sont rangés sous `Runtime/Enemies`, `Runtime/Projectiles`, `Runtime/FX`.
- **Nommage dans l'éditeur** : GameObjects, prefabs et assets en PascalCase anglais (`PlayerShip`, `EnemyData_GuardDrone`, `WaveData_M1_Tunnels`).
- **Import des sprites** : réglages pixel art du cahier des charges §3 (Point, sans compression, PPU 48, pivot en pixels), appliqués par un `AssetPostprocessor` pour ne jamais dépendre d'un réglage manuel oublié.
- Aucune référence manquante (`Missing`) dans une scène ou un prefab livré.
- **Tant que les packs d'assets ne sont pas importés**, l'absence d'asset n'est jamais un blocage : utilise des placeholders simples (formes de base Unity, carrés et cercles colorés selon la palette, sons de test générés) rangés dans `Features/<Feature>/Art/Placeholder/`. Grâce à l'enfant `Visual`, remplacer un placeholder par le vrai sprite ne demande aucun changement de code. Signale chaque placeholder dans le compte rendu.

## Blocage : quand tu ne peux pas faire ce qui est demandé

Cas typiques : action possible seulement dans l'éditeur Unity, asset manquant, package non autorisé, spec ambiguë ou contradictoire, test qui échoue encore après deux tentatives de correction, critère impossible à vérifier depuis le terminal.

1. Tu t'arrêtes. Tu ne livres pas une solution dégradée sans le dire.
2. Tu termines ton message de session par un bloc **« ⚠️ Action requise de Dyllan »**, et tu le recopies dans le compte rendu de la spec :

```
## ⚠️ Action requise de Dyllan
- Bloqué sur : <critère ou étape>
- Pourquoi : <cause précise>
- À faire à ma place, pas à pas :
  1. Dans Unity : <menu exact> → <champ> = <valeur>
  2. ...
- Comment vérifier : <ce que tu dois voir>
- Ensuite, relance-moi avec : « <phrase exacte à me renvoyer> »
```

3. Le compte rendu et tous les messages destinés à Dyllan sont **en français** ; le code, les commentaires et les messages de commit sont **en anglais**.

## Définition de terminé (chaque story)

- [ ] Critères d'acceptation de la spec cochés un par un.
- [ ] Compile sans erreur ni nouvel avertissement ; console propre en Play Mode.
- [ ] Tests EditMode ajoutés pour la logique pure touchée ; tous les tests passent.
- [ ] Aucune valeur d'équilibrage ni aucun texte visible en dur.
- [ ] Conventions respectées : commentaires anglais sur chaque classe, méthode et champ, `[Tooltip]` sur chaque champ sérialisé, dossiers et nommage conformes.
- [ ] Entités de jeu en prefabs, scène rangée par sections, aucune référence manquante.
- [ ] Aucun fichier de `Assets/ThirdParty/` dans `git status`.
- [ ] Section « Compte rendu » du fichier de spec remplie (en français) : fichiers modifiés, comment tester en 2 minutes, limites connues, et le bloc « ⚠️ Action requise » si besoin.
- [ ] Un commit par story, message en anglais : `E1-02: Add 8-direction player movement`, sur la branche `story/E1-02-player-movement`.

## En cas de doute

Si la spec est ambiguë ou contredit le cahier des charges ou le Game Concept : pose la question avant de coder. N'invente aucune règle de design.
