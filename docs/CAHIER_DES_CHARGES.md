# Cahier des charges — An Astral Story : Ashar (démo)

Version 1.1 — 24 septembre 2026 — livrable **P0-11** (spécification technique + CLAUDE.md du dépôt)
v1.5 : Game Concept v2.4 — commandes révisées (§7.2) : tir J, bouclier K, impulsion L, dash M, Espace retiré ; bouclier à trancher avant E5-05.
v1.1 : décisions de la section 2 validées ; voix off hybride ajoutée (Concept v2.2).
v1.4 : Game Concept v2.3 — difficulté unique (§7.9), dialogues en pause narrative avec pilote automatique (§7.7), FR/EN obligatoire (§7.13).
v1.3 : conventions de code et de projet alignées sur les dépôts de Dyllan (Stuffy_Infinite_Runner, Snake_2D) : organisation par fonctionnalité (§5.1), commentaires anglais, prefabs, procédure de blocage (§9).
v1.2 : inventaire des assets (§7.15), Pacificateurs, résolution révisée à 960×540 (§2, point 8, validée), règle de non-diffusion des assets achetés (§3).
Destinataires : Dyllan (conception, tests, arbitrages) et Claude Code (implémentation).
Sources : Game Concept v2.3 (fait foi), Bestiaire démo v2 et Fiche vaisseau joueur v2 (en attente de validation), backlog.

**Légende**
- **[GC]** : repris du Game Concept v2.3.
- **[Bestiaire]** / **[Fiche]** : valeur reprise de ces documents (convertie à l'échelle du §4 quand c'est nécessaire).
- **[Proposition]** : absent de la documentation. Proposition à valider, avec son coût (faible / moyen / élevé).
- **[À venir P0-xx]** : dépend d'un livrable de pré-production pas encore produit. Claude Code utilise des placeholders.

---

## 1. Objet et périmètre

### 1.1 Livrable

Une démo jouable Windows (.exe) publiée sur Itch.io : 2 missions de 5 à 8 minutes chacune, dialogues en bulle textuelle (FR ou EN), 2 entrées de codex, un seul niveau de difficulté, sauvegarde automatique par mission, 1080p à 60 fps. **[GC]**

### 1.2 Hors périmètre de la démo

- Tout service en ligne, multijoueur, leaderboard en ligne, contenu live.
- Voix off au-delà de 15 à 20 répliques clés, et voix en anglais. **[GC v2.3]**
- Plusieurs modes de difficulté. **[GC v2.3]**
- Prologue d'évasion à pied (projet distinct). **[GC v2.1]**
- Missions de la version complète (Icebox V-01 à V-06).
- Remappage des touches, succès, statistiques, New Game+.

### 1.3 Boucle de mission **[GC v2.3]**

```
Menu → Mission N
  dialogue de briefing → vague → dialogue → vague → … → (mini-)boss
  → écran de fin de mission (score) → entrée de codex débloquée → sauvegarde
  → Mission N+1 ou menu
```

---

## 2. Décisions de pré-production (validées le 24/09/2026)

Ces écarts venaient des changements de la v2.1. Les propositions ci-dessous sont **validées** et font désormais référence.

| # | Écart | Décision | Coût |
| --- | --- | --- | --- |
| 1 | L'échelle de la Fiche v2 (1 unité = 108 px, caméra de 10 unités de haut) n'est pas compatible avec le pixel art 48 px en rendu Pixel Perfect. | Résolution de référence 640×360 (×3 en 1080p, ×4 en 1440p, ×6 en 4K), PPU 48, 1 unité = 48 px de référence = 144 px à l'écran. Toutes les distances et vitesses de la Fiche sont multipliées par 0,75 pour garder les mêmes sensations à l'écran (§4). À confirmer au test d'import. | Faible |
| 2 | Le backlog prévoit une « fenêtre radio holographique avec portrait » et des répliques en ScriptableObject `RadioLine`. La v2.1 prévoit une bulle façon Pokémon, avec Ink, sans portrait. | Les dialogues sont écrits en Ink. Le ScriptableObject `RadioLine` disparaît et devient un événement `Dialogue` dans `WaveData`, qui pointe vers un knot Ink (§7.7). Reformuler E3-01 et E3-02. | Moyen (intégration d'Ink) |
| 3 | La bulle de dialogue « en bas d'écran » recouvre la zone où évolue le vaisseau du joueur dans un shmup vertical. | **Remplacé par le point 9** : les dialogues suspendant l'action, la bulle redevient pleine largeur en bas d'écran. | — |
| 4 | Le graze (ajouté en v2.1) n'apparaît ni dans le backlog ni dans la Fiche. | Nouvelle story **E1-09 Graze** en S1, 1 ½ j, coupe n° 6. La charge de S1 passe de 6,5 à 7,5 ½ j. | Faible |
| 5 | P0-15 prévoit de tester le Stylized 2D Space Shooter Pack, abandonné. | P0-15 devient « Test d'import du Futuristic Spaceship SHMUP Bundle (DyLESTorm) en URP, Pixel Perfect, PPU 48 ». La correspondance ennemis ↔ assets du Bestiaire (§4) est à refaire sur ce pack. | Faible |
| 6 | Durée de mission : 5 à 8 min dans le GC, environ 10 min dans P0-08. | Retenir 5 à 8 min (le GC fait foi). Corriger P0-08. | Aucun |
| 7 | Localisation « FR (VO) / EN (VOST) » dans E7-04. | Textes FR/EN uniquement (§7.12). Renommer E7-04. | Aucun |
| 9 | **Game Concept v2.3 (26/09/2026)** : un seul niveau de difficulté ; dialogues en pause narrative ; FR/EN obligatoire. | Modes supprimés (§7.9) ; nouvel état de jeu `Dialogue` (§7.7) : actions ennemies en attente, vaisseau et carte en pilote automatique, musique qui continue, avancement au bouton de tir, reprise à la fermeture de la fenêtre, **jamais `Time.timeScale = 0`** ; localisation EN retirée des coupes (§7.13). | Neutre : − 1,5 ½ j (modes, polish Arcade), + 1 ½ j (état Dialogue), EN désormais incompressible |
| 8 | **Révision du point 1 après inventaire des assets (validée le 25/09/2026)** : le vaisseau joueur retenu mesure environ 58 × 68 px, les ennemis 64 px, le boss environ 240 px. En 640×360, le joueur occuperait 19 % de la hauteur de l'écran et le boss les deux tiers. | Résolution de référence **960×540** (×2 en 1080p), PPU 48 conservé. Le joueur occupe environ 12,5 % de la hauteur, ce qui correspond à l'intention de la Fiche v2. Valeurs du §4 recalculées. Le test d'import P0-15 sert à le vérifier à l'écran. | Faible |

Reste à aligner sur la v2.3 : le prompt du projet, le Bestiaire v2 et la Fiche vaisseau v2 (story P0-16).

---

## 3. Stack et configuration du projet

| Élément | Choix |
| --- | --- |
| Moteur | Unity 6 (6000.x), template 2D URP |
| Rendu | URP avec 2D Renderer ; `UnityEngine.Rendering.Universal.PixelPerfectCamera` |
| Entrées | Input System (package officiel) ; ancien Input Manager désactivé |
| Texte | TextMeshPro avec une police pixel (à choisir dans le plan d'assets P0-14) |
| UI | uGUI (Canvas) : le plus simple pour un HUD et des menus en pixel art ; Canvas Scaler en résolution de référence 1920×1080 |
| Dialogues | Ink, via le package ink-unity-integration d'inkle (installation par URL Git ou Asset Store, gratuit) |
| Tests | Unity Test Framework (EditMode en priorité) |
| Versionnement | Git + GitHub, `.gitignore` Unity. Git LFS pour `*.wav`, `*.ogg`, `*.psd`, `*.aseprite` **[Proposition]**. **Les assets achetés ne doivent jamais être poussés sur un dépôt public** : leurs licences interdisent la redistribution. Tout `Assets/ThirdParty/` (packs d'origine **et** retouches, car les licences interdisent aussi la redistribution d'une version modifiée) est exclu par le `.gitignore` et sauvegardé à part. Le dépôt peut ainsi rester public pour le portfolio. |
| Build | Windows x86_64, Mono (IL2CPP inutile pour la démo) |

### Réglages obligatoires pour le pixel art

- Import des sprites : Filter Mode = Point, sans mip maps, Compression = None, PPU = 48, pivot en pixels.
- Pixel Perfect Camera : Assets PPU 48, Reference Resolution 960×540 (repli : 640×360, voir §2 point 8), Grid Snapping = Pixel Snapping (compatible avec le post-process de S7 ; `UpscaleRenderTexture` est exclu), Crop Frame = Windowbox.
- Quality Settings : anti-aliasing désactivé ; HDR et MSAA désactivés sur la caméra.
- `Application.targetFrameRate = 60` et VSync activé.

---

## 4. Échelle et repères spatiaux

**[Validé le 25/09/2026 : 960×540 ; valeurs dérivées de la Fiche v2 × 1,125, à régler au playtest E1-08]**

- Caméra fixe, orthographique : 11,25 unités de haut × 20 unités de large (960×540 px de référence).
- 1 unité = 48 px de référence = 96 px à l'écran en 1080p.
- Zone jouable : l'écran moins une marge de 0,7 unité sur chaque bord.
- Tailles à l'écran : joueur ≈ 1,2 × 1,4 u ; ennemi standard ≈ 1,33 u ; grand vaisseau ≈ 2,7 u ; boss ≈ 5 u (44 % de la hauteur).
- Le décor défile, pas la caméra. Les ennemis apparaissent hors écran, par le haut ou par les côtés.

| Repère | Fiche v2 | Valeur retenue |
| --- | --- | --- |
| Vitesse du joueur | 8 u/s | **9 u/s** (traverse l'écran en 2,2 s) |
| Rayon de la hitbox du joueur | 0,10 u | **0,11 u** (≈ 5 px de référence) |
| Projectile du joueur | 20 u/s | **22,5 u/s** |
| Dash | 2,5 u | **2,8 u** |
| Rayon de l'impulsion de piratage | 3,0 u | **3,4 u** |
| Rayon du graze | — | **0,6 u** |
| Ennemi lent / moyen / rapide / en piqué | 2 / 4 / 6 / 12 u/s | **2,25 / 4,5 / 6,75 / 13,5 u/s** |
| Projectile ennemi standard / visé rapide | 5 / 7 u/s | **5,6 / 7,9 u/s** |

Si 640×360 est finalement retenu : multiplier toutes les valeurs de ce tableau par 2/3.

Toutes ces valeurs vivent dans des ScriptableObjects (§6) et seront réglées au playtest E1-08.

---

## 5. Architecture

### 5.1 Arborescence

Organisation **par fonctionnalité**, reprise du `Component/<Feature>/{Scripts,Prefabs,...}` du dépôt Stuffy_Infinite_Runner. Chaque fonctionnalité ne contient que les sous-dossiers dont elle a besoin.

```
/README.md                  Présentation du projet (format des dépôts de Dyllan, voir §9.4)
/CLAUDE.md
/docs/                      Concept, cahier des charges, fiches, specs/, notes/, playtests/
/Assets/
  _Project/
    Core/
      Scripts/              GameSession, GameEvents (bus d'événements statique), SceneFlowController, Layers
      Data/                 GameBalanceData_Default (réglages globaux d'équilibrage)
      Prefabs/              Systems (GameSession, AudioManager… réunis dans un prefab placé en tête de chaque scène)
    Features/
      Player/               Scripts/ Prefabs/ Data/ Animations/   (vaisseau, tir, dash, graze, impulsion, bouclier)
      Combat/               Scripts/ Prefabs/ Data/               (ProjectileController, DamageableController, FirePatternData, projectiles)
      Enemies/              Scripts/ Prefabs/ Data/               (un prefab par unité, variantes en Prefab Variants, EnemyData)
      Bosses/
        GuardShuttle/       Scripts/ Prefabs/ Data/
        Leash/              Scripts/ Prefabs/ Data/ Art/          (ancrages et noyau dessinés maison)
      Waves/                Scripts/ Data/                        (MissionRunnerController, WaveData, MissionData)
      Environment/          Scripts/ Prefabs/                     (PlayAreaController, BackgroundScrollController, couches de décor M1 et M2)
      PowerUps/             Scripts/ Prefabs/ Data/
      Dialogue/             Scripts/ Prefabs/ Data/ Ink/          (bulle, SpeakerData, VoiceLibrary, m1_fr.ink…)
      Codex/                Scripts/ Prefabs/ Data/
      UI/
        HUD/  Menus/  Pause/  EndOfMission/   chacun : Scripts/ Prefabs/
      Save/                 Scripts/
      Localization/         Scripts/ strings.csv
      Audio/                Scripts/ Own/                         (bruitages de dialogue et sons faits maison)
    Scenes/                 Boot, MainMenu, Mission, TestBed
    Settings/               URP (Renderer 2D), Input (AsharControls.inputactions), AudioMixer, Quality
    Editor/                 AssetPostprocessor pixel art, scripts de mise en place (menu Ashar/Setup/<ID>)
    Tests/
      EditMode/
  ThirdParty/               IGNORÉ PAR GIT — packs achetés, rangés par éditeur :
    DyLESTorm/  Felmir/  CuteSCKR/  AleksisTristanShaw/
    _Modified/<Pack>/       retouches (recolorations Aseprite) : également jamais versionnées
```

Règles :
- Seuls les éléments **entièrement faits maison** sont versionnés dans `_Project/` (sprites Leash, bulle de dialogue, sons de dialogue…). Les prefabs versionnés peuvent référencer des sprites de `ThirdParty/` : sans les packs, un clone du dépôt affiche des sprites manquants, ce qui est normal et documenté dans le README.
- Aucun fichier à la racine de `Assets/` ou de `_Project/`, aucun dossier fourre-tout.
- Trois assembly definitions : `Ashar.Runtime` (racine de `_Project/`), `Ashar.Editor` (`Editor/`), `Ashar.Tests.EditMode` (`Tests/EditMode/`).
- **Nommage des classes** : `…Controller` pour un MonoBehaviour de gameplay, préfixe `UI…` pour l'interface, `…Data` pour un ScriptableObject, aucun suffixe pour une classe statique ou C# pur. Les noms de classes cités dans la suite de ce document (ex. `EnemyShooter`, `LeashBoss`, `DialogueDirector`) sont indicatifs : le nom final suit cette règle (`EnemyShootController`, `LeashBossController`, `DialogueDirectorController`).
- **Prefabs** : tout ce qui apparaît en jeu est un prefab ; racine = logique + collider, enfant `Visual` = rendu ; unités dérivées en Prefab Variants ; parties de boss en prefabs imbriqués.
- **Scènes** : hiérarchie rangée par sections vides `--- SYSTEMS ---`, `--- CAMERA ---`, `--- ENVIRONMENT ---`, `--- GAMEPLAY ---`, `--- UI ---` ; les objets créés en jeu sont rangés sous `Runtime/Enemies`, `Runtime/Projectiles`, `Runtime/FX`.

### 5.2 Scènes

| Scène | Contenu |
| --- | --- |
| `Boot` | Charge la sauvegarde et les réglages, puis bascule sur `MainMenu`. |
| `MainMenu` | Jouer (choix de la mission débloquée), Codex, Options (dont la langue FR/EN), Quitter. |
| `Mission` | Une seule scène de mission, paramétrée par un `MissionData` (M1 ou M2) : moins de scènes à maintenir. |
| `TestBed` | Bac à sable de développement : joueur, ennemis déposés à la main, compteur de FPS et de projectiles. Exclue du build. |

### 5.3 Flux de données

```
GameSession (état de jeu, mission en cours, vies, score)
   │   états : Gameplay · Dialogue · Paused · MissionEnd · GameOver (GameEvents.OnGameStateChanged)
   │
MissionRunner ── lit ── MissionData ── WaveData (timeline d'événements)
   │                                     ├─ Spawn    → EnemyController (EnemyData)
   │                                     ├─ Dialogue → état Dialogue (ennemis en attente, pilote automatique), DialogueDirector (knot Ink), reprise à la fermeture
   │                                     ├─ Zone     → change la zone de décor (M1 : hangar → dômes → espace)
   │                                     ├─ Wait     → attend la destruction d'un groupe
   │                                     ├─ Scroll   → vitesse du décor
   │                                     └─ Boss / End
   ├── GameBalanceData (multiplicateurs globaux appliqués au spawn)
   └── en fin de mission → CodexEntry débloquée → SaveSystem
```

### 5.4 Couches physiques et collisions

Physics2D avec des triggers et des Rigidbody2D Kinematic (prototype). Le passage à des tests de distance manuels n'aura lieu que si la mesure E2-01 l'exige.

| Couche | Entre en collision avec |
| --- | --- |
| PlayerHitbox | EnemyBullet, Enemy |
| PlayerGraze | EnemyBullet |
| PlayerBullet | Enemy |
| Enemy | PlayerBullet, PlayerHitbox |
| EnemyBullet | PlayerHitbox, PlayerGraze |
| PowerUp | PlayerHitbox |

---

## 6. Données (ScriptableObjects)

Tous les champs sont éditables dans l'Inspector. Les valeurs de départ figurent au §7.

### PlayerShipData
`moveSpeed`, `hitboxRadius`, `showHitbox` (bool), `fireRate` (tirs/s), `bulletDamage`, `bulletSpeed`, `dashDistance`, `dashDuration`, `dashInvulnTime`, `dashCooldown`, `grazeRadius`, `grazeScore`, `grazePulseCooldownReduction`, `pulseRadius`, `pulseCooldown`, `pulseDamageToHeavy`.

### EnemyData
`id`, `displayNameKey`, `prefab`, `maxHP`, `score`, `movement` (enum `Straight`, `Sine`, `Dive`, `LateralSweep`, `Orbit`, `Static`) et ses paramètres (`speed`, `amplitude`, `frequency`), `firePattern` (FirePatternData, optionnel), `pulseResponse` (enum `Destroy`, `Stun`, `Disable`, `Damage`, `Immune`), `pulseValue` (durée en secondes ou dégâts), `dropsPowerUp` (PowerUpData, optionnel).

### FirePatternData
`type` (enum `Aimed`, `Fan`, `Spiral`, `Burst`), `count`, `spreadAngle`, `interval`, `bulletSpeed`, `bulletPrefab`, `telegraphTime` (0 si aucun), `pulseErasable` (bool : l'impulsion efface-t-elle ces projectiles ?).

### WaveData
Liste ordonnée d'événements `{ time (s), type, paramètres }` :

| Type | Paramètres |
| --- | --- |
| `Spawn` | EnemyData, nombre, formation (enum `Line`, `V`, `Column`, `Single`), point d'entrée (haut, gauche, droite + décalage), délai entre unités, `groupTag` |
| `Dialogue` | nom du knot Ink. **Bloquant** : la timeline attend la fermeture de la fenêtre de dialogue (§7.7) |
| `Zone` | identifiant de zone de décor ; transition de 2 s entre les couches |
| `WaitForClear` | `groupTag` : la timeline se met en pause jusqu'à la destruction ou la sortie de tout le groupe |
| `ScrollSpeed` | vitesse cible, durée de transition |
| `Boss` | prefab du boss ; la timeline attend sa mort |
| `End` | fin de mission |

Le temps s'écoule depuis le dernier `WaitForClear` terminé. On peut ainsi réordonner les blocs sans recalculer toute la timeline.

### MissionData
`id` (M1, M2), `titleKey`, `waves` (liste de WaveData jouées à la suite), `backgroundLayers`, `music`, `bossMusic`, `inkStory` (par langue), `voiceLibrary` (VoiceLibrary, optionnel), `codexUnlocked` (CodexEntry), `nextMission`.

### GameBalanceData
Un seul asset (`GameBalanceData_Default`), réglages globaux : `enemyHPMultiplier`, `fireDensityMultiplier` (divise l'intervalle de tir), `bulletSpeedMultiplier`, `startLives`, `continues` (-1 = illimités), `respawnInvulnTime`. Valeurs de départ au §7.9.

### PowerUpData
`type` (enum `MultiShot`, `Shield`, puis `HackedDrones` et `PleiadianLaser` si ces coupes ne sont pas appliquées), `sprite`, `duration`, `value`.

### CodexEntry
`id`, `titleKey`, `bodyKey`, `mission`, `canonStatus` (enum `Canon`, `Adapte`, `Invente`), usage interne pour le dossier de design, jamais affiché.

### SpeakerData
`id` (KAAL, CHACAL, LEASH, DISSIDENTS), `nameKey`, `nameColor`, `blipClip`, `blipPitch`, `charsPerSecond`.

### DialogueSettingsData
`autopilotPosition` (position de croisière, en unités), `autopilotBlendTime` (s), `autopilotBobAmplitude` (u), `leftoverFadeTime` (s), `resumeGraceTime` (s), `typewriterSkipOnFirstPress` (bool).

### VoiceLibrary (coupe n° 0)
Un asset par mission : liste `{ voId, AudioClip }`. Le `voId` correspond au tag Ink `#vo:` d'une réplique. Voix en français uniquement, quelle que soit la langue des sous-titres.

---

## 7. Spécifications fonctionnelles

### 7.1 Joueur

| Fonction | Comportement | Valeurs de départ |
| --- | --- | --- |
| Déplacement | 8 directions, diagonales normalisées, bloqué dans la zone jouable. **[GC]** | 9 u/s |
| Tir principal | Tir continu tant que la touche est maintenue, en ligne droite vers le haut. **[GC] [Fiche]** | 10 tirs/s × 10 dégâts = **100 dégâts/s** (référence de tous les PV) |
| Hitbox | Cercle au centre du vaisseau ; point argent visible (désactivable dans les options). **[Fiche]** | rayon 0,11 u |
| Dash | Déplacement rapide dans la direction de l'entrée, vers le haut sans entrée, avec une invulnérabilité courte. Traînée visuelle. **[GC] [Fiche]** | 2,8 u en 0,18 s ; invulnérable 0,28 s ; cooldown 1,5 s |
| Graze | Zone circulaire distincte de la hitbox. Un projectile ennemi qui la traverse sans toucher la hitbox rapporte un bonus, **une seule fois par projectile**. Pas de graze pendant le dash. **[GC] + [Proposition]** | rayon 0,6 u ; +20 points ; −0,25 s sur le cooldown de l'impulsion. Coût faible. Le bonus d'impulsion relie le pilotage agressif au piratage : à retirer s'il déséquilibre. |
| Impulsion de piratage | Onde circulaire autour du vaisseau, avec cooldown. Elle efface les projectiles `pulseErasable`, puis applique `pulseResponse` à chaque ennemi touché. **[GC] [Fiche]** | cooldown 12 s ; rayon 3,4 u ; 300 dégâts aux tourelles et à la navette ; sans effet sur LEASH ni sur ses projectiles |
| Vies et respawn | Un impact = une vie perdue, perte du multi-shot, réapparition en bas au centre, invulnérabilité avec clignotement. **[Bestiaire §5]** | voir §7.9 |
| Continue | Reprise sur place, vies remises à leur valeur de départ, score remis à 0 (convention arcade). **[Proposition]**, coût faible | — |
| Game over | Sans continue restant : écran de game over, puis « Recommencer la mission » ou « Menu ». | — |

### 7.2 Commandes **[Proposition, à valider au playtest E1-08]**

**[Mise à jour v1.5, Game Concept v2.4]** : le mapping ci-dessous remplace entièrement l'ancien. **Espace est retiré de tous les usages**, y compris pour avancer un dialogue.

| Action | Clavier | Manette |
| --- | --- | --- |
| Déplacement | ZQSD, ou flèches | Stick gauche, croix |
| Tir (maintenu) | J, ou W | A |
| Bouclier éphémère | K, ou X (non lié : point ouvert ci-dessous) | X |
| Impulsion de piratage | L, ou C | LT (gâchette gauche) |
| Dash | M, ou V | RT (gâchette droite) |
| Pause | Échap | Start |
| Avancer un dialogue (hors action) | J ou W (touches de tir) | A |

Sur un clavier AZERTY, W, X, C et V (rangée du bas, main gauche) font la même chose que J, K, L et M. Elles sont liées par **position physique** (`<Keyboard>/z`, `x`, `c`, `v`) : elles ne se gênent donc pas avec le déplacement, ni sur AZERTY ni sur QWERTY (où ce sont Z, X, C et V).

**Point ouvert, bloquant pour E5-05** : la touche K fait du bouclier éphémère une action déclenchée par le joueur, alors que le §7.10 le décrit comme un effet automatique de power-up (« absorbe 1 impact, 10 s au maximum »). À trancher avant E5-05 : la durée est-elle toujours de 10 s une fois activé ? y a-t-il un cooldown ? que se passe-t-il si le joueur appuie sur K sans charge de bouclier disponible ? **Aucun code sur le bouclier tant que ce point n'est pas tranché** ; la touche K et le bouton X ne sont pas liés à une action pour l'instant.

Note technique : l'Input System sait lier une touche de deux façons. Le déplacement utilise la **position physique** : `<Keyboard>/w`, `a`, `s`, `d` donnent ZQSD sur un clavier AZERTY et WASD sur un QWERTY, sans code supplémentaire. Les actions J, K, L et M utilisent au contraire la **lettre imprimée sur la touche** (`<Keyboard>/#(m)`), car le M d'un clavier AZERTY n'est pas à la même place physique que celui d'un QWERTY.

### 7.3 Ennemis **[Bestiaire v2, vitesses converties au §4]**

PV = TTK × 100 dégâts/s (avant le multiplicateur global de `GameBalanceData`).

| Unité | Missions | PV | Score | Déplacement | Tir | Impulsion |
| --- | --- | --- | --- | --- | --- | --- |
| Drone de garde de LEASH (harceleur) | M1, M2 | 50 | 100 | Descente sinueuse, 4,5 u/s | 1 tir visé toutes les 2 s | Détruit |
| Drone de garde (piqué) | M1, M2 | 50 | 100 | Piqué vers le joueur, 13,5 u/s | Aucun | Détruit |
| Chasseur de patrouille | M1, M2 | 200 | 200 | Entrée latérale en formation, balayage, sortie par le bord opposé | Éventail de 3 toutes les 2,5 s | Étourdi 2 s |
| Intercepteur | M2 | 100 | 250 | Piqué vers la position du joueur, puis remontée | Rafale de 2 tirs visés au bas du piqué | Étourdi 2 s |
| Satellite sentinelle | M1, M2 | 300 | 300 | Fixe ou en orbite lente, 2,25 u/s | 1 tir visé lent toutes les 3 s | Désactivé 3 s |
| Pacificateur du Culte (robot au sol) **[Inventé, à valider]** | M1 | 300 | 300 | Collé au sol (défile avec le décor) + marche latérale lente | Rafale de 2 tirs visés toutes les 3 s | Étourdi 3 s |
| Tourelle à plasma (×3, verrous de M1) | M1 | 1 200 | 1 000 | Fixe au bord de l'écran | Spirale lente de plasma large, télégraphiée par une ligne 1 s avant le tir | 300 dégâts |

Projectile ennemi : un seul modèle standard, rouge orangé. Les chasseurs utilisent la même couleur en plus gros, les tourelles un plasma large et lent. Toutes les unités sont Inventées ou Adaptées, sauf les satellites et les tourelles (Canon, préquel). Aucun impact sur le code.

### 7.4 Patterns de tir

Prototype (E2-03) : un seul composant `EnemyShooter` qui lit `FirePatternData` et gère les 4 types par un `switch`. Il n'y aura pas de système de patterns composables tant que le boss LEASH ne le demande pas (E5).
- `Aimed` : `count` projectiles vers la position du joueur au moment du tir.
- `Fan` : `count` projectiles répartis sur `spreadAngle`, centrés sur le joueur ou vers le bas (option).
- `Spiral` : un projectile par intervalle, angle incrémenté.
- `Burst` : `count` tirs visés espacés de 0,1 s.
- `telegraphTime > 0` : ligne ou flash d'avertissement avant le tir.

### 7.5 Missions et vagues

Structure narrative et de combat **[GC §6]**. Chronologie détaillée : **[À venir P0-08]**.

**M1 Phobos IX, « La Chienne Rouge »** (5-8 min), avec un scroll qui monte du hangar vers la surface puis vers l'espace :
1. Hangar et tunnels : drones de garde. Tutoriel en dialogue avec le Chacal.
2. Dômes : Pacificateurs au sol, chasseurs de patrouille, satellites sentinelles.
3. Sortie : 3 tourelles à plasma. Le scroll ralentit, puis s'arrête (`ScrollSpeed` à 0) ; `WaitForClear` sur le groupe « tourelles » ouvre la sortie.
4. Mini-boss : navette de garde (§7.6).

**M2 La course hors de Mars** (5-8 min), orbite puis champ de débris :
1. Chasseurs et intercepteurs, avec le Chacal en ailier (coupe n° 5 : sinon, présence en dialogue seulement).
2. Satellites sentinelles à désactiver.
3. Boss LEASH (§7.6), puis fin de démo.

**Rythme et décor** :
- Les vagues suivent une **timeline en secondes**, indépendante de la musique : aucune synchronisation au tempo. La musique tourne en boucle en continu, y compris pendant les dialogues, donc rien ne se désynchronise quand un dialogue dure.
- Le décor **n'est pas procédural** : chaque mission a des zones dessinées à la main (level design), faites de couches qui bouclent. Le passage d'une zone à la suivante est déclenché par l'événement `Zone` de la timeline, jamais par la distance parcourue. Pendant un dialogue, la zone courante continue de défiler en boucle aussi longtemps que nécessaire.

En attendant P0-08, Claude Code crée des `WaveData` de test (`Test_M1.asset`) qui couvrent chaque type d'événement.

### 7.6 Boss

**Navette de garde pénitentiaire (mini-boss M1)** **[Bestiaire 3.6]** : 4 500 PV, 5 000 points, script dédié `GuardShuttle` à 2 phases, **sans machine à états générique**.
- Phase 1 : balayage horizontal ; canons en proue, avec éventails larges et tirs visés lents.
- Phase 2 (à 50 % des PV) : 2 vagues de drones de garde larguées ; canons plus lents.
- Barre de vie ; une amélioration (multi-shot) garantie à la destruction.

**LEASH, croiseur de poursuite (boss M2)** **[Bestiaire 3.7]** + **[À venir P0-07]** : 6 000 PV (3 phases de 2 000), 10 000 points.
- Phase 1 : des drones enchaînés tournent autour du croiseur (projectiles rouge orangé).
- Phase 2 : les chaînes coupées libèrent des drones à tête chercheuse (magenta).
- Phase 3 : noyau exposé, salves denses et lisibles (jaune).
- Phase 4 optionnelle : LEASH tente de couper la transmission (coupe n° 1).
- Transitions : 1,5 s d'invulnérabilité entre les phases **[Proposition]**, pour compenser le multi-shot (jusqu'à 2,6 fois le DPS de base, risque signalé par la Fiche §4). Les dialogues de LEASH se placent **uniquement à ces transitions** (projectiles effacés, boss figé) : le combat reprend à la fin du dialogue.
- Implémentation : E5-01 crée une machine à états **simple et propre à LEASH** (liste de phases avec seuil de PV, pattern, couleur) ; on ne la généralise que si la navette y gagne nettement.

### 7.7 Dialogues (Ink) et voix off **[GC v2.3] + [Proposition d'implémentation]**

Rendu : bulle pleine largeur en bas d'écran, façon Pokémon : nom de l'interlocuteur, jusqu'à 3 lignes, texte qui défile caractère par caractère avec un bruitage 16-bit par caractère (ou par groupe de 2), timbre distinct par interlocuteur (`SpeakerData`), indicateur ▼ quand la ligne est complète. LEASH a un son synthétique, menaçant et mécanique.

**Un dialogue est une pause narrative** (état de jeu `Dialogue`) : le jeu n'est pas mis en pause, **les actions ennemies sont mises en attente** et **le vaisseau et la carte passent en pilote automatique**.
1. **Début** : l'événement `Dialogue` de la timeline arrive après une séquence de jeu (normalement placé juste après un `WaitForClear`, donc sur un écran vide). La timeline des vagues est suspendue : aucun nouvel ennemi. Filet de sécurité si l'écran n'est pas vide : les ennemis encore présents cessent de tirer et quittent l'écran par le haut, les projectiles ennemis s'estompent en 0,3 s, sans points.
2. **Pilote automatique** : le joueur perd le contrôle du vaisseau (déplacement, tir, dash, impulsion ignorés). Le vaisseau rejoint en douceur une position de croisière (`autopilotPosition`, par défaut au centre, au-dessus de la bulle) en `autopilotBlendTime` (0,6 s), puis oscille légèrement pour rester vivant. **Le décor continue de défiler et la musique continue en boucle.** Le vaisseau ne peut pas être touché.
3. **Lecture** : le bouton de tir (J / A) fait avancer : première pression, la ligne s'affiche en entier ; deuxième pression, ligne suivante. Pause (Échap / Start) reste disponible.
4. **Fin** : la pression sur la dernière ligne ferme la fenêtre de dialogue. Le joueur reprend immédiatement les commandes, depuis la position de croisière ; le tir est ignoré pendant `resumeGraceTime` (0,3 s) pour qu'un appui prolongé ne tire pas aussitôt ; la timeline des vagues reprend.

**Règle technique non négociable : ne jamais utiliser `Time.timeScale = 0` pour un dialogue.** C'est ce qui figeait tout dans Snake_2D. Le dialogue suspend uniquement la timeline des vagues, l'IA et les tirs ennemis, et bascule le vaisseau en pilote automatique (via `GameEvents.OnGameStateChanged`). `Time.timeScale = 0` est réservé au menu pause.

Placement : entre les vagues et aux transitions de phase des boss, **jamais pendant l'action**. 3 lignes maximum par bulle.

Voix off (coupe n° 0 ; stories E7-07 à E7-09) :
- 15 à 20 répliques clés portent un tag `#vo:<id>`.
- Si le clip existe dans la `VoiceLibrary` : il joue à la place des bruitages et le texte sert de sous-titre ; si le joueur avance avant la fin, le clip s'arrête.
- Tag présent mais clip absent : comportement texte normal, sans erreur. Le jeu fonctionne donc tel quel si la coupe n° 0 s'applique.
- Coût : faible côté code (0,5 ½ j) ; l'essentiel est la production (séance, montage, filtre radio).

Format Ink attendu (l'écriture relève de P0-09 ; Claude Code n'écrit que des placeholders) :

```ink
=== m1_intro ===
#speaker:CHACAL #vo:m1_001
[TODO-TEXTE: le Chacal explique le déplacement]
#speaker:KAAL
[TODO-TEXTE: réponse de Kaal'varis]
-> DONE
```

- Un fichier `.ink` par mission et par langue (`m1_fr.ink`, `m1_en.ink`), avec des noms de knots identiques dans les deux langues.
- La langue choisie dans les options (FR ou EN) détermine le fichier Ink chargé ; un changement de langue s'applique au dialogue suivant.
- Le tag `#speaker:ID` détermine le nom affiché, la couleur et le son ; le tag optionnel `#vo:id` associe une voix off.
- `WaveData` déclenche un knot par son nom ; un nom de knot introuvable produit une erreur explicite dans la console.

### 7.8 Codex **[GC]**

- Une entrée par mission (M1 : Phobos IX et LEASH ; M2 : l'Assemblée Terrienne et les Exilés). Textes : **[À venir P0-10]**.
- Débloquée à la fin de la mission : notification sur l'écran de fin, puis sauvegarde.
- Consultable depuis le menu principal et depuis la pause, jamais pendant l'action.
- Écran : liste des entrées (les entrées verrouillées s'affichent « ??? »), puis le texte de l'entrée sélectionnée.
- Fin de la démo : le message crypté des Exilés sur le réseau CSM s'affiche en plein écran (texte canon, fourni par P0-09), suivi d'un écran « À suivre dans le roman Anandavira » **[Proposition]**, puis retour au menu.

### 7.9 Difficulté **[GC v2.3]**

Un seul niveau de difficulté. Les valeurs vivent dans `GameBalanceData_Default` et se règlent au playtest. **[Proposition de départ]** : densité réduite par rapport à l'ancien mode Normal.

| Paramètre | Valeur de départ |
| --- | --- |
| PV des ennemis | × 1 |
| Densité de tirs | × 0,8 |
| Vitesse des projectiles | × 1 |
| Vies de départ | 3 |
| Continues | Illimités (le score repart à 0) |
| Invulnérabilité au respawn | 2 s |

### 7.10 Power-ups **[Fiche §5]**

| Power-up | Priorité | Effet |
| --- | --- | --- |
| Multi-shot | Must | Niveau 1 : 1 flux (100 dégâts/s) ; niveau 2 : 2 flux parallèles (200/s) ; niveau 3 : 3 flux en éventail étroit (≈ 260/s). Retour au niveau 1 au premier impact. |
| Bouclier éphémère | Must | Absorbe 1 impact, 10 s au maximum, halo argent. |
| Drones de garde piratés | Could (coupe n° 3) | Non spécifié. Ne pas implémenter avant le point de contrôle de fin S4. |
| Laser pléiadien | Could (coupe n° 4) | Non spécifié. Même règle. |

Apparition : uniquement sur les ennemis marqués `dropsPowerUp` dans `EnemyData` ou dans la `WaveData`, pour que le level design reste maîtrisé ; plus un drop garanti sur la navette. **[Proposition]**, coût faible.

### 7.11 HUD et menus

- HUD **[GC]** : vies, score, jauge de l'impulsion (cooldown), niveau de multi-shot, barre de vie du boss (en haut, visible seulement pendant un combat de boss), bulle de dialogue (§7.7).
- Le glitch de LEASH (coupe n° 2) : perturbation visuelle de 0,5 à 1 s sur les bords du HUD et sur la bulle, jamais sur les commandes ni sur les projectiles.
- Menus : principal ; choix de la mission (seulement les missions débloquées) ; codex ; options (langue FR/EN, volumes musique, effets et dialogues, hitbox visible, plein écran) ; pause (reprendre, codex, options, abandonner la mission) ; fin de mission ; game over.
- Tous les menus sont navigables au clavier et à la manette.

### 7.12 Sauvegarde **[GC] + [Proposition de format]**

Fichier JSON dans `Application.persistentDataPath/save.json`, écrit à la fin de chaque mission et quand les options changent.

```json
{
  "version": 1,
  "unlockedMissions": ["M1", "M2"],
  "unlockedCodex": ["codex_m1"],
  "bestScores": { "M1": 0, "M2": 0 },
  "settings": { "language": "fr", "music": 0.8, "sfx": 0.8, "blips": 0.8, "showHitbox": true, "fullscreen": true }
}
```

Un fichier corrompu ou absent donne une sauvegarde neuve, avec un avertissement dans la console, jamais un crash.

### 7.13 Localisation **[GC v2.3]**

- Interface : `Localization/strings.csv` (`key;fr;en`) chargé dans un dictionnaire par une classe `Loc`. Clé absente : affichage `#key#` et avertissement dans la console. **[Proposition]** : plus léger que le package Unity Localization pour une démo de cette taille (coût faible).
- Dialogues : un fichier Ink par langue (§7.7). Codex : clés de la table.
- **FR et EN obligatoires** (Game Concept v2.3) : la localisation n'est plus une coupe. Langue choisie dans les options, FR par défaut, mémorisée dans la sauvegarde.

### 7.14 Audio **[GC v2.3]**

- AudioMixer avec 4 groupes : Music, SFX, Blips (dialogues), Voice (voix off). La musique continue en boucle pendant les dialogues et s'atténue légèrement pendant une voix off. Les alarmes et les avertissements de tir ne doivent jamais être masqués par la musique.
- Musique : pack Cyberpunk Synthwave (Aleksis Tristan Shaw), pistes bouclables. Vérifier la licence (E7-02). Répartition proposée **[Proposition, à valider à l'écoute]** :

| Moment | Piste | Raison |
| --- | --- | --- |
| Menu, codex | Metropolis at Night (2:39) | La plus posée du pack, d'après son titre |
| M1 Phobos IX | Cybernetic Breach (3:53) | L'effraction, l'évasion |
| Mini-boss (navette) | Mecha Fight (3:00) | Combat court |
| M2 La course hors de Mars | Maverick Synth (4:47) | La plus longue : moins de répétition sur 5 à 8 min |
| Boss LEASH | Posthuman (3:1x) | Écho au Culte de l'Ascension Cybernétique |

- Import : les WAV (344 Mo au total) sont convertis dans Unity en Vorbis, avec le type de chargement Streaming pour la musique. Les fichiers sources restent dans `Assets/ThirdParty/AleksisTristanShaw/`, hors dépôt.
- Effets : tirs secs, lasers, impacts métalliques, explosions, alarmes.
- Voix off : enregistrée par un proche en S7, filtre radio (bande étroite, grésillements), LEASH au vocodeur ; export OGG ; fichiers nommés par `voId`.

### 7.15 Assets **[Inventaire du 24/09/2026, d'après les pages itch.io ; aperçus visuels à valider au test d'import P0-15]**

Tailles en pixels de référence (PPU 48). Tous les packs autorisent l'usage commercial et interdisent la redistribution (§3).

| Pack | Contenu utile | Usage dans la démo |
| --- | --- | --- |
| **Animated Pixel Ships** (DyLESTorm) | 9 vaisseaux en 2 versions (normale, « Powered Up »), inclinaison (3 images) et propulseurs (4 images), environ 58 × 68 px ; sprite de bouclier animé (6 images) ; 15 icônes (santé, spécial, power-up, bombe) ; 7 projectiles et 5 missiles animés ; 2 explosions ; vaisseau de soutien en 5 couleurs ; astéroïdes et un fond spatial non tuilé | **Vaisseau de Kaal'varis** (bleu et argent ; version « Powered Up » au multi-shot niveau 3) ; **bouclier éphémère** ; **icônes du HUD et des power-ups** ; tirs du joueur ; astéroïdes du champ de débris (M2) ; vaisseau de soutien = Chacal en ailier (coupe n° 5) ou drones piratés (coupe n° 3). Les 4 portraits de pilotes ne sont pas utilisés (pas de portraits dans le Concept). |
| **Pixel Enemies for SHMUP** (DyLESTorm) | 50 ennemis, surtout 64 × 64 px (Bug animé, Heavy, Danger, Cannon, Mini, Wings, Emperor, Armor), boss d'environ 240 px, fichiers Aseprite | Drones de garde (Bug), satellites sentinelles (Mini ou Armor), croiseur de LEASH (Boss, recoloré), drones enchaînés et chercheurs, brouilleurs |
| **Pixel Art Spaceships for SHMUP** (DyLESTorm) | 48 vaisseaux de 48 px, 5 vaisseaux de type char, 12 grands vaisseaux de 128 px, explosion, propulseurs, projectiles | Chasseurs de patrouille de l'A.E.T. ; **navette de garde** (grand vaisseau de 128 px) |
| **Pixel Art Spaceships for SHMUP 2** (DyLESTorm) | 45 vaisseaux de 64 px | Intercepteurs (M2), variantes de chasseurs du Culte |
| **Pixel Art Space Background SHMUP** (DyLESTorm) | Blocs d'angle, 2 sols (uni, petites villes), tuyaux, 6 nébuleuses, 3 planètes, 2 couches d'étoiles | Fond spatial (M1 fin, M2) ; **arc de Mars** (une planète recolorée, en grand et en partie hors écran) ; compléments de la surface de Phobos |
| **Pixel Art City Background** (DyLESTorm) | Bâtiments et forêts d'environ 144 px (variantes normale et sombre), routes, nuages | Réserve : non utilisé dans la démo (décor terrien) |
| **Pixel Art VFX Explosions** (DyLESTorm) | 2 grandes et 5 moyennes explosions, 1 explosion électrique en 3 couleurs, 2 impacts | Destructions ; **explosion électrique = effet de l'impulsion de piratage** (étourdissement, désactivation) ; impacts de tir |
| **Sci-fi Turret Sprite Pack** (Felmir) | Tourelles vues de dessus, base et canon séparés (16, 32, 48 px), projectiles animés | **3 tourelles à plasma** (canon à plasma de 32 px, le canon pivote vers le joueur) ; projectiles ennemis |
| **Futuristic Military Base** (Cute SCKR, *AI Assisted*) | Bâtiments de base, hangar à mechs, robots de combat, véhicules, tourelles, équipements intérieurs, format RPG Maker | **Hangar de départ (M1)** ; **Pacificateurs** (robots) |
| **Pixel Mars Base Tileset** (Cute SCKR, *AI Assisted*) | Sol martien, cratères, dômes, labos, modules, tuyaux, cuves, panneaux solaires | **Sol et dômes de la surface de Phobos IX (M1)**, recolorés en gris-brun et assombris ; éléments en vue de trois-quarts (fusées, antennes) écartés |
| **Cyberpunk Synthwave** (Aleksis Tristan Shaw) | 10 pistes de synthwave sombre bouclables (WAV 16 bits, 44,1 kHz), 32 min 49 au total | Toute la musique de la démo (répartition au §7.14) |

**Règles d'import**
- Les éléments des deux packs Cute SCKR sont en vue RPG Maker (légère perspective) : ne garder que ceux qui restent lisibles sous des vaisseaux vus de dessus. Les packs sont signalés « AI Assisted » : le mentionner dans les crédits de la page itch.io.
- Recoloration dans Aseprite, jamais par teinte dans le moteur pour les couleurs de palette : sol de Phobos en gris-brun sombre, projectiles ennemis rouge orangé, LEASH magenta et jaune. La teinte `SpriteRenderer.color` reste réservée aux flashs d'impact.
- Aucun rouge saturé sous la zone de jeu : les sols et Mars sont assombris et désaturés pour laisser ressortir les projectiles.

**Encore à produire (maison, versionné dans la fonctionnalité concernée, ex. `Features/Bosses/Leash/Art/`)**
| Élément | Coût |
| --- | --- |
| Ancrages et noyau de LEASH (2 sprites) | Faible |
| Projectiles aux couleurs de la palette, si ceux des packs ne suffisent pas après recoloration | Faible |
| Bulle de dialogue (9-slice), cadre du codex, barre de boss | Faible |
| Police pixel FR/EN (accents) : à trouver, gratuite | Faible |
| Pack d'effets sonores, alarmes et bruitages de dialogue : à trouver | Moyen |

---

## 8. Exigences non fonctionnelles

| Exigence | Critère |
| --- | --- |
| Performance | 60 fps stables en 1080p dans un build Windows, sur la machine de développement puis sur une seconde machine (E8-01). |
| Projectiles | Plafond déterminé par la mesure E2-01 (500, 1 000 et 2 000 projectiles). Si les 60 fps ne tiennent pas, on plafonne d'abord le nombre de projectiles, avant toute coupe de contenu. **[GC]** |
| Pooling | Décidé après E2-01, pas avant. **Risque signalé** : `Instantiate`/`Destroy` en masse provoque des pics de garbage collector ; c'est le premier suspect si le framerate chute. |
| Lisibilité | Palette : joueur bleu, tirs du joueur argent-blanc, ennemis rouge orangé, LEASH magenta et jaune ; 3 couleurs de projectiles au maximum par phase de boss ; Mars atténué, jamais en aplat rouge sous les projectiles. **[GC]** |
| Pixel art | Aucun sprite flou ni scintillant ; aucune échelle non entière (Windowbox). |
| Post-process (S7) | Bloom léger possible avec Pixel Snapping. **Risque** : le bloom bave sur le pixel art ; à tester dès l'import (P0-15), pas en S7. |
| Robustesse | Aucune exception non gérée en build ; une sauvegarde corrompue n'empêche pas de lancer le jeu. |
| Accessibilité minimale | Hitbox visible activable ; dialogues lus au rythme du joueur ; continues illimités. |

---

## 9. Méthode de travail avec Claude Code

### 9.1 Cycle d'une story

1. Dyllan choisit **une** story dans le backlog, dans l'ordre des dépendances.
2. Il crée `docs/specs/<ID>.md` à partir du modèle ci-dessous (Claude en chat peut le rédiger).
3. Claude Code implémente sur la branche `story/<ID>-<slug>`, remplit le compte rendu, puis commite.
4. Dyllan teste avec la procédure « Test de validation », puis fusionne dans `main`.
5. Le statut est mis à jour dans le backlog.

### 9.2 Modèle de spec

```markdown
# <ID> — <Titre>
**Semaine** : Sx · **Estimation** : n ½ j · **Dépend de** : <IDs>

## Contexte
Pourquoi, et la section concernée du cahier des charges.

## Fichiers concernés
Création ou modification, avec les chemins.

## À faire
Liste courte et ordonnée.

## Critères d'acceptation
- [ ] …

## Test de validation (2 minutes maximum)
Étapes que Dyllan exécute dans l'éditeur ou le build.

## Hors périmètre
Ce qu'il ne faut PAS faire dans cette story.

## Compte rendu (rempli par Claude Code, en français)
Fichiers modifiés · comment tester · limites connues · idées notées.
+ bloc « ⚠️ Action requise de Dyllan » si Claude Code est bloqué (§9.4).
```

### 9.3 README du dépôt

Le `README.md` suit le format des dépôts de Dyllan (Stuffy_Infinite_Runner, Snake_2D), en français : titre et accroche, badges (statut, moteur, plateforme), lien itch.io, À propos, Contrôles, Ce que ce projet m'a appris, Points techniques, Tech stack, Statut, Auteur. S'y ajoutent une section « Assets tiers » (packs utilisés avec crédits, mention « AI Assisted » des packs Cute SCKR, et le fait qu'ils ne sont pas inclus dans le dépôt) et une section « Méthode » (développement assisté par Claude Code, story par story, avec validation humaine). Squelette créé en P0-12, complété en E8-05.

### 9.4 Procédure de blocage

Quand Claude Code ne peut pas répondre à une spec (action possible seulement dans l'éditeur, asset manquant, package non autorisé, spec ambiguë, test qui échoue encore après deux corrections), il s'arrête sans livrer de solution dégradée et termine par un bloc « ⚠️ Action requise de Dyllan » : ce qui bloque, pourquoi, les étapes à faire à sa place dans Unity (menus exacts), comment vérifier, et la phrase à lui renvoyer pour reprendre. Format exact : `CLAUDE.md`, section « Blocage ».

### 9.5 Tests automatisés (EditMode)

Seulement pour la logique pure, qui coûte peu à tester : multiplicateurs de `GameBalanceData`, suspension et reprise de la timeline pendant un dialogue, ordre et reprise de la timeline de vagues, graze compté une seule fois par projectile, cooldowns (dash, impulsion), aller-retour de sauvegarde JSON, chargement de la table de chaînes. Pas de tests PlayMode dans la démo, sauf besoin précis.

---

## 10. Plan par semaine

Le backlog reste la référence ; stories ajoutées ou modifiées par ce document en **gras**.

| Semaine | Stories | Jalon |
| --- | --- | --- |
| S0 (jusqu'au 30 sept) | P0-07, P0-08, **P0-15 (pack DyLESTorm)**, **P0-16 (alignement v2.3)**, P0-14, P0-09 (avec répliques `#vo`), P0-10, P0-12 | Projet vide + build + test d'import validé |
| S1 (1-7 oct) | E1-01 à E1-06, **E1-09 Graze**, E1-07, E1-08 | Premier build Windows jouable |
| S2 (8-14 oct) | E2-01 à E2-07 | Point de contrôle 1 : performance |
| S3 (15-21 oct) | **E3-01 Bulle de dialogue**, **E3-02 Ink et état Dialogue**, E3-03 à E3-07 | — |
| S4 (22-28 oct) | E4-01 à E4-07 | Point de contrôle 2 : coupes 1, 3, 4 |
| S5 (29 oct-4 nov) | E5-01 à E5-06 (+ E5-07 à E5-09 si non coupées) | — |
| S6 (5-11 nov) | E6-01 à E6-07 (+ E6-05 et E6-08 si non coupées) | Point de contrôle 3 : coupes 0, 2, 5 |
| S7 (12-18 nov) | E7-01 à E7-04, E7-06 (**E7-04 : FR/EN obligatoire**), **E7-07 à E7-09 voix off** si non coupée | — |
| S8 (19-25 nov) | E8-01 à E8-06 | Publication |

Liste de coupes, dans l'ordre de sacrifice **[GC v2.3]** : 0 voix off · 1 phase 4 de LEASH · 2 glitch du HUD · 3 drones piratés · 4 laser pléiadien · 5 Chacal ailier scripté · 6 graze.

---

## Annexe A — Specs S1 prêtes à déléguer

Chaque bloc se copie dans `docs/specs/<ID>.md`.

### P0-12 — Mise en place du dépôt et du projet Unity
- **Contexte** : §3, §5.1, §9.3. Le projet Unity **existe déjà** à la racine du dépôt (Unity 6000.6.2f1, template 2D URP). Ne pas le recréer : le nettoyer et le mettre aux normes.
- **État constaté le 26/09** : dossier `Assets/Welcome/` du template ; `Assets/Scenes/SampleScene.unity` ; `Assets/Settings/` (URP, Renderer2D, InputSystem_Actions) ; fichiers `.meta` orphelins sans dossier (`Art`, `Audio`, `Prefabs`, `Scripts`, `ScriptableOkjects`) ; packages non prévus au §3 : `com.unity.visualscripting`, `com.unity.collab-proxy`, `com.unity.learn.iet-framework` ; aucune règle `ThirdParty` dans le `.gitignore`.
- **À faire** :
  1. Supprimer `Assets/Welcome/`, `SampleScene` et les `.meta` orphelins ; retirer les trois packages ci-dessus. Ne toucher à aucun autre package : lister dans le compte rendu ceux qui semblent inutiles, Dyllan décide.
  2. Compléter le `.gitignore` par `/Assets/ThirdParty/` et `/Assets/ThirdParty.meta`. Vérifier que Git LFS est installé (`git lfs install`) : le `.gitattributes` envoie déjà l'audio dans LFS.
  3. Créer l'arborescence du §5.1 (seulement les dossiers utilisés en S1, aucun dossier vide) ; déplacer le contenu de `Assets/Settings/` vers `Assets/_Project/Settings/` **depuis l'éditeur** (pour conserver les GUID) ; renommer `InputSystem_Actions` en `AsharControls`. Asmdefs `Ashar.Runtime`, `Ashar.Editor`, `Ashar.Tests.EditMode`.
  4. Input System seul actif (ancien Input Manager désactivé) ; réglages pixel art du §3, dont `Editor/PixelArtImportPostprocessor.cs` qui applique Point, sans compression, sans mip maps, PPU 48 à toute texture importée dans `_Project/` et `ThirdParty/`.
  5. Scènes `Boot`, `MainMenu`, `Mission` et `TestBed` dans `_Project/Scenes/`, chacune avec les sections de hiérarchie du §5.1, enregistrées dans les Build Settings (`Boot` en premier, `TestBed` exclue).
  6. README : restructurer le `README.md` existant au format du §9.3 **en gardant le texte de Dyllan**, avec ces corrections : Kaal'varis n'est pas « ancien militaire » (retiré en Concept v2.2, Annexe B : ancien messager du Conseil pléiadien) ; Phobos IX est un « complexe pénitentiaire orbital » ; « dialogues radio » devient « dialogues en bulle (Ink) et voix off » ; retirer `RadioLine` et WebGL (cible : Windows) ; ajouter les sections « Assets tiers » et « Méthode ».
  7. Outillage : vérifier si le Unity CLI ou un serveur MCP Unity est utilisable (le package `com.unity.ai.assistant` est installé) pour créer prefabs et scènes dans l'éditeur ouvert ; sinon, poser la base des scripts de mise en place (menu `Ashar/Setup/<ID>`). Indiquer dans le compte rendu la voie active.
- **Critères** : le projet s'ouvre sans erreur ni avertissement ; un build Windows se lance et affiche un fond noir ; un test EditMode factice passe ; un fichier déposé dans `Assets/ThirdParty/` n'apparaît pas dans `git status` ; une texture importée reçoit automatiquement les réglages pixel art ; plus aucun fichier du template dans `Assets/`.
- **Test** : ouvrir le projet, lancer le Test Runner, construire et lancer l'exécutable, déposer une image dans `Assets/ThirdParty/` puis lancer `git status`.
- **Hors périmètre** : gameplay, import des packs d'assets (P0-15).

### E1-01 — Scène de test, caméra et scroll de fond
- **Contexte** : §4.
- **Fichiers** : `Features/Environment/Scripts/PlayAreaController.cs`, `Features/Environment/Scripts/BackgroundScrollController.cs`, prefabs `Features/Environment/Prefabs/PlayArea` et `TestBackground`, scène `Scenes/TestBed`.
- **À faire** : caméra orthographique avec Pixel Perfect Camera (960×540, PPU 48, Pixel Snapping, Windowbox) ; `PlayAreaController` expose les limites jouables (marge de 0,7 u) et les dessine en Gizmo ; fond en 2 ou 3 couches qui défilent vers le bas à des vitesses différentes (parallaxe), avec une vitesse réglable.
- **Critères** : le défilement est continu et sans couture ; le cadre jouable est visible dans la vue Scène ; aucun sprite flou.
- **Test** : Play, observer le défilement 30 s, changer la vitesse dans l'Inspector.

### E1-02 — Déplacement 8 directions
- **Fichiers** : `Features/Player/Scripts/PlayerShipData.cs` (classe) et `Features/Player/Data/PlayerShipData_Default.asset` (instance), `Features/Player/Scripts/PlayerMovementController.cs`, `Settings/Input/AsharControls.inputactions`, prefab `Features/Player/Prefabs/PlayerShip` (racine + enfant `Visual`).
- **À faire** : actions Move, Fire, Dash, Pulse et Pause (liaisons du §7.2) ; mouvement normalisé à `moveSpeed`, bloqué par `PlayAreaController`.
- **Critères** : ZQSD sur AZERTY, les flèches et le stick fonctionnent ; les diagonales ne sont pas plus rapides ; le vaisseau ne sort jamais de la zone ; la vitesse se modifie dans `PlayerShipData`.
- **Test** : traverser l'écran en diagonale au clavier puis à la manette.

### E1-03 — Tir principal en rafale
- **Fichiers** : `Features/Player/Scripts/PlayerShootController.cs`, `Features/Combat/Scripts/ProjectileController.cs`, prefab `Features/Combat/Prefabs/PlayerBullet` ; projectiles créés sous `Runtime/Projectiles`.
- **À faire** : tir tant que Fire est maintenu, à `fireRate` ; projectile droit à `bulletSpeed`, détruit hors écran ; `Instantiate`/`Destroy` (pas de pooling).
- **Critères** : cadence et dégâts réglables dans `PlayerShipData` ; aucun projectile ne survit hors écran (vérifié dans la hiérarchie).
- **Test** : maintenir le tir 10 s et vérifier que le nombre de projectiles reste stable.

### E1-04 — Dash
- **Fichiers** : `Features/Player/Scripts/PlayerDashController.cs`.
- **À faire** : dash de `dashDistance` en `dashDuration`, dans la direction de l'entrée (vers le haut sans entrée), invulnérable pendant `dashInvulnTime`, cooldown ; traînée visuelle provisoire.
- **Critères** : le dash ne traverse pas les limites ; il est impossible de dasher pendant le cooldown ; un booléen `IsInvulnerable` est lisible par `PlayerHealthController`.
- **Test** : enchaîner les dashes, puis vérifier le cooldown avec un compteur de débogage.

### E1-05 — Limites, collisions et hitbox lisible
- **Fichiers** : `Features/Player/Scripts/PlayerHealthController.cs`, `Core/Scripts/Layers.cs`, `Core/Scripts/GameEvents.cs` (premier événement : `OnPlayerHit`), prefabs `Features/Combat/Prefabs/EnemyBulletDummy` et `DebugBulletEmitter` (placés uniquement dans `TestBed`).
- **À faire** : couches et matrice de collisions du §5.4 ; hitbox circulaire de `hitboxRadius` avec un point argent (option `showHitbox`) ; un projectile ennemi factice qui touche la hitbox déclenche `GameEvents.OnPlayerHit` (flash), sauf si `IsInvulnerable`.
- **Critères** : un projectile qui frôle le sprite sans toucher la hitbox ne déclenche rien ; le point de hitbox se masque via le ScriptableObject.
- **Test** : dans `TestBed`, un émetteur factice tire sur le joueur ; vérifier les frôlements et les impacts.

### E1-09 — Graze (nouvelle story, coupe n° 6)
- **Contexte** : §7.1, **[Proposition]** à valider.
- **Fichiers** : `Features/Player/Scripts/PlayerGrazeController.cs`, extension de `ProjectileController` (champ `_hasBeenGrazed`), événement `GameEvents.OnPlayerGrazed`.
- **À faire** : trigger circulaire de `grazeRadius` sur la couche PlayerGraze ; quand un projectile ennemi entre, qu'il n'est pas marqué `grazed` et que le joueur n'est pas en dash : le marquer, émettre `GameEvents.OnPlayerGrazed` (score, et réduction du cooldown de l'impulsion quand elle existera) ; petit flash et son provisoire.
- **Critères** : un même projectile n'est compté qu'une fois ; aucun graze pendant le dash ; un projectile qui touche la hitbox compte comme un impact et non comme un graze (si les deux arrivent dans la même frame, l'impact l'emporte).
- **Test** : test EditMode sur le comptage unique ; en jeu, frôler l'émetteur factice.

### E1-06 — Feedback minimal
- **Fichiers** : `Features/Player/Scripts/PlayerFeedbackController.cs` (abonné à `GameEvents`), sons provisoires dans `Features/Audio/Own/Placeholder/`.
- **À faire** : effets sonores provisoires (tir, impact, dash, graze) ; flash blanc de 0,1 s sur l'enfant `Visual` du vaisseau touché.
- **Critères** : chaque action se distingue à l'oreille et à l'œil.

Puis **E1-07** (build Windows, Dyllan) et **E1-08** (playtest des sensations, Dyllan : consigner dans `docs/playtests/S1.md` les valeurs finales de vitesse, de cadence, de dash et de graze).
