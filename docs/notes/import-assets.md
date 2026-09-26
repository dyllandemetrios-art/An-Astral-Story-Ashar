# Import des assets achetés (P0-15) : tri et mesures

Réalisé le 26/09/2026 par Claude Code, à partir de `_AssetInbox/` (dossier local, ignoré par Git). Les fichiers importés sont dans `Assets/ThirdParty/`, **jamais versionnés** (licences). Cette note ne contient que des noms et des mesures.

## Ce qui a été retenu

| Dossier dans `Assets/ThirdParty/` | Source | Contenu importé | Usage prévu (§7.15) |
| --- | --- | --- | --- |
| `DyLESTorm/AnimatedPixelShips` | Animated Pixel Ships | **Plane 07** (versions A et B), bouclier, icônes et power-ups, projectiles, unités de soutien, propulseurs de grands vaisseaux, 4 astéroïdes | Vaisseau de Kaal'varis, bouclier, HUD, tirs, Chacal en ailier, champ de débris (M2) |
| `DyLESTorm/PixelEnemies` | Pixel Enemies for SHMUP | Tout le pack (Bug, Heavy, Danger, Cannon, Mini, Wings, Emperor, Armor, 2 boss) | Drones, satellites, brouilleurs, croiseur de LEASH (à recolorer) |
| `DyLESTorm/PixelSpaceshipsSHMUP` | Pixel Spaceships for SHMUP | Tout le pack (48 px, 128 px, propulseurs, explosion, projectiles) | Chasseurs de l'A.E.T., navette de garde |
| `DyLESTorm/PixelSpaceshipsSHMUP2` | Pixel Spaceships for SHMUP 2 | Tout le pack (64 px) | Intercepteurs (M2) |
| `DyLESTorm/PixelSpaceBackgroundSHMUP` | Pixel Art Space Background SHMUP | Tout sauf les images d'exemple | Fond spatial, arc de Mars, sol de Phobos |
| `DyLESTorm/PixelArtVFXExplosions` | Pixel Art VFX Explosions | Tout le pack | Destructions, impulsion de piratage (explosion électrique), impacts |
| `Felmir/SciFiTurretPack` | Sci-Fi Turret Pack **v2** | 6 tourelles (autocannon, dual laser, flak, heavy laser, laser, **plasma**) | Tourelles à plasma |
| `CuteSCKR/FuturisticMilitaryBase` | Futuristic Military Base | 7 feuilles de tuiles 768 px | Hangar de départ (M1), Pacificateurs |
| `CuteSCKR/PixelMarsBaseTileset` | Pixel Mars Base Tileset, format **RPG Maker MV/MZ** | 7 feuilles de tuiles 768 px | Sol et dômes de Phobos IX (M1) |
| `AleksisTristanShaw/CyberpunkSynthwave` | Cyberpunk Synthwave | **Les 10 pistes** (à écouter avant d'affecter chacune ; le tableau §7.14 n'est qu'une proposition), CLUF et lisez-moi | Toute la musique de la démo |

Total : environ 630 fichiers, environ 350 Mo (dont 340 Mo de WAV). Les fichiers `Readme` des packs sont conservés.

## Ce qui a été laissé dans `_AssetInbox/` (et pourquoi)

- **Planes 01 à 06, 08 et 09** de Animated Pixel Ships : rouges, verts ou violets, donc en conflit avec la lisibilité (joueur bleu, ennemis rouge orangé). Aucun besoin identifié.
- **Portraits, fond spatial non tuilé 480×270, explosions de ce pack** : le Concept n'utilise pas de portraits, et le pack VFX est plus complet.
- **Pixel Art City Background** : décor terrien, marqué « réserve » au §7.15.
- **Sci-Fi Turret Pack 2** : tourelles légères (gatling, roquettes, missiles) qui ne servent pas au besoin « canon à plasma ».
- **Mars, les deux autres formats** (`mars base-20260309-updated` et `mars-base`, mêmes fichiers en 2048 px) : le format MV/MZ est en 768 px, soit 16 × 16 tuiles de 48 px, ce qui correspond au PPU 48 du jeu et à la base militaire. Les versions 2048 px auraient exigé un redimensionnement.
- **Le morceau d'exemple** de la musique (« Sample Track ») : un extrait promotionnel, pas une piste du pack.
- **Fichiers sources `.aseprite`** (ennemis, tourelles) : non importés, car Unity les transformerait en doublons. Ils resteront dans `_AssetInbox/` pour les recolorations (LEASH, sol de Phobos).

## Réglages appliqués

- **Textures (604)** : Sprite, Point, sans compression, sans mip maps, PPU 48, appliqués automatiquement par `PixelArtImportPostprocessor`. Contrôle fait sur les 604 : 604 conformes.
- **Musique (10 clips)** : chargement Streaming, compression Vorbis, qualité 0,7, chargement en arrière-plan (§7.14). Réglé à la main dans l'éditeur, car le postprocessor ne gère que les textures.
- **Mode Single imposé.** Unity 6.6 découpe par défaut chaque nouveau sprite en plusieurs sprites rognés (mode Multiple, découpe automatique) : à l'import, les 608 textures avaient donné 1 778 sprites dont 1 695 rognés, ce qui aurait déplacé le pivot d'une image d'animation à l'autre. `PixelArtImportPostprocessor` impose maintenant `Single`, et les 604 textures déjà importées ont été corrigées (582 restent après le nettoyage ci-dessous, toutes en Single, aucune rognée).
- **Feuilles de sprites** (ex. `planes_07A.png`, 384 × 480) : importées en un seul sprite. Les images individuelles (`planes_07A_1.png`…) sont déjà découpées et directement utilisables ; les feuilles seront découpées (mode Multiple, manuellement) si une story d'animation en a besoin.

## Mesures du contenu réel (hors transparence) à PPU 48

| Élément | Case | Contenu | En unités | Part de la hauteur 540 |
| --- | --- | --- | --- | --- |
| **Joueur 07A** (avec flammes) | 96×96 | 60 × 77 px | 1,25 × 1,60 | 14,3 % |
| Joueur 07B (« Powered Up ») | 96×96 | 70 × 80 px | 1,46 × 1,67 | 14,8 % |
| Bouclier | 96×96 | 96 × 96 px | 2,00 × 2,00 | 17,8 % |
| Ennemi Bug | 64×64 | 40 × 41 px | 0,83 × 0,85 | 7,6 % |
| Ennemi Heavy | 64×64 | 42 × 50 px | 0,88 × 1,04 | 9,3 % |
| Ennemi Cannon | 64×64 | 48 × 53 px | 1,00 × 1,10 | 9,8 % |
| Ennemi Armor | 88×88 | 88 × 87 px | 1,83 × 1,81 | 16,1 % |
| Chasseur 48 px | 48×48 | 40 × 43 px | 0,83 × 0,90 | 8,0 % |
| Intercepteur 64 px | 64×64 | 36 × 46 px | 0,75 × 0,96 | 8,5 % |
| Tourelle plasma | 32×32 | 32 × 32 px | 0,67 × 0,67 | 5,9 % |
| **Boss 01** | 240×240 | 228 × 218 px | 4,75 × 4,54 | 40,4 % |

## Ce que ça change

1. **La résolution 960×540 est confirmée** : le joueur occupe environ 14 % de la hauteur (contre près de 20 % à 640×360) et le boss environ 40 %, dans l'intention du §4 (12,5 % et 44 %). Aucun sprite flou dans la Pixel Perfect Camera (contrôlé sur une capture en 1920×1080). **Les vitesses et distances du §4 n'ont donc pas à être recalibrées.**
2. **Vaisseau joueur retenu : Plane 07** (bleu clair et argent, peu d'orange), plutôt que Plane 04 (bleu, mais nez et propulseurs orange qui gêneraient la lecture des ennemis). Il mesure 60 px de large contre les 58 px annoncés.
3. **Les tailles du §4 sont celles des cases, pas du contenu** : un ennemi « standard ≈ 1,33 u » (case de 64 px) a un contenu visible de 0,83 à 1,10 u. **Les colliders et hitbox d'ennemis devront suivre le contenu visible**, pas la case, sinon les tirs semblent touchés trop tôt.
4. **Lisibilité** : le boss 01 est rouge et orange, il devra bien être recoloré en magenta et jaune pour LEASH (§7.15). Les ennemis du pack ne sont pas tous rouge orangé (certains sont verts, violets ou turquoise) : c'est à surveiller au playtest, avec une recoloration si besoin.
5. **Test du bloom (§8)** : non fait ici, à prévoir avant S7.

## Nettoyage fait après validation (26/09)

- **Placeholders supprimés** : `PlayerShip.png` (joueur) et `BackgroundFar|Mid|Near.png` (fond). Le prefab `PlayerShip` utilise maintenant `planes_07A_1.png`, et `TestBackground` utilise `purple_background`, `stars_2` et `stars_1` (mode Tiled, 22,5 × 20 unités, deux tuiles par couche).
- **Retiré des packs importés** (les originaux restent dans `_AssetInbox/`) : `enemy.png` et `nebula_collection.png` (aperçus), le dossier `Explosion` du pack Spaceships (doublon du pack VFX), 8 icônes de vaisseaux inutiles (`icon-plane-01` à `06`, `08`, `09` ; on garde `icon-plane-07` pour les vies du HUD et `icon-plane`).
- **Conservés volontairement** : tous les types d'ennemis (des variantes recolorées sont prévues pour les chasseurs, satellites et brouilleurs), toutes les couleurs d'unités de soutien, les tuiles de bases.
- **`BackgroundScrollController`** lit désormais la hauteur du renderer (et non celle du sprite), pour que les couches en mode Tiled se raccordent : la distance de réenroulement est de 20 unités, un multiple de la hauteur du motif (10 unités).
- **Limite visible** : les étoiles du motif `stars_1` se répètent sur une grille régulière (motif de 5,6 × 10 u). Ça se voit en regardant bien ; à remplacer par un fond plus large (nébuleuses) en S3-S4 si ça gêne.

## Suite proposée

1. ~~Remplacer les placeholders~~ : fait.
2. Reprendre E1-03 (tir en rafale) avec les vrais projectiles du joueur (argent-blanc).
3. Recolorations dans Aseprite (LEASH, sol de Phobos) plus tard, avec les `.aseprite` restés dans `_AssetInbox/`.
