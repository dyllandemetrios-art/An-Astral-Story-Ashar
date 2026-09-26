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
| `AleksisTristanShaw/CyberpunkSynthwave` | Cyberpunk Synthwave | **5 pistes** du tableau §7.14 (Metropolis at Night, Cybernetic Breach, Mecha Fight, Maverick Synth, Posthuman), CLUF et lisez-moi | Toute la musique de la démo |

Total : 620 fichiers, environ 190 Mo (dont 177 Mo de WAV). Les fichiers `Readme` des packs sont conservés.

## Ce qui a été laissé dans `_AssetInbox/` (et pourquoi)

- **Planes 01 à 06, 08 et 09** de Animated Pixel Ships : rouges, verts ou violets, donc en conflit avec la lisibilité (joueur bleu, ennemis rouge orangé). Aucun besoin identifié.
- **Portraits, fond spatial non tuilé 480×270, explosions de ce pack** : le Concept n'utilise pas de portraits, et le pack VFX est plus complet.
- **Pixel Art City Background** : décor terrien, marqué « réserve » au §7.15.
- **Sci-Fi Turret Pack 2** : tourelles légères (gatling, roquettes, missiles) qui ne servent pas au besoin « canon à plasma ».
- **Mars, les deux autres formats** (`mars base-20260309-updated` et `mars-base`, mêmes fichiers en 2048 px) : le format MV/MZ est en 768 px, soit 16 × 16 tuiles de 48 px, ce qui correspond au PPU 48 du jeu et à la base militaire. Les versions 2048 px auraient exigé un redimensionnement.
- **6 pistes de musique** (Alien Weapon, Cyborgs vs. Androids, Highrise Massacre, Off-Grid Outlaw, Resistance is Futile, morceau d'exemple) : le tableau §7.14 n'en utilise que 5. Elles restent disponibles si tu veux changer une affectation à l'écoute.
- **Fichiers sources `.aseprite`** (ennemis, tourelles) : non importés, car Unity les transformerait en doublons. Ils resteront dans `_AssetInbox/` pour les recolorations (LEASH, sol de Phobos).

## Réglages appliqués

- **Textures (604)** : Sprite, Point, sans compression, sans mip maps, PPU 48, appliqués automatiquement par `PixelArtImportPostprocessor`. Contrôle fait sur les 604 : 604 conformes.
- **Musique (5 clips)** : chargement Streaming, compression Vorbis, qualité 0,7, chargement en arrière-plan (§7.14). Réglé à la main dans l'éditeur, car le postprocessor ne gère que les textures.
- **Limite** : les feuilles de sprites (ex. `planes_07A.png`, 384 × 480) sont importées en un seul sprite. Les images individuelles (`planes_07A_1.png`…) sont déjà découpées et directement utilisables ; les feuilles seront découpées (mode Multiple) si une story d'animation en a besoin.

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

## Suite proposée

1. Remplacer les placeholders : vaisseau joueur (`Visual` du prefab `PlayerShip`, sprite 07A) et fond d'étoiles (couches de `TestBackground`). Aucun code à changer.
2. Reprendre E1-03 (tir en rafale) avec les vrais projectiles du joueur (argent-blanc).
3. Recolorations dans Aseprite (LEASH, sol de Phobos) plus tard, avec les `.aseprite` restés dans `_AssetInbox/`.
