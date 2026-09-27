# Référence agent — Scènes et assets

À lire avant de créer, modifier ou relire une scène, un prefab, un asset ou ses imports. Ces conventions restent obligatoires.

## Conventions Unity (projet propre dans l'éditeur)

- **Tout ce qui apparaît en jeu est un prefab.** Aucun sprite glissé directement dans une scène, aucun GameObject de gameplay qui n'est pas une instance de prefab.
- **Structure d'un prefab** : la racine porte la logique et le collider ; un enfant `Visual` porte le `SpriteRenderer` (et l'`Animator`). Changer de sprite ne casse jamais la logique.
- **Variantes** : une unité qui dérive d'une autre est une *Prefab Variant* (ex. `Interceptor` variante de `PatrolFighter`). Les parties d'un boss sont des prefabs imbriqués.
- **Hiérarchie des scènes** : des GameObjects vides servent de sections, dans cet ordre : `--- SYSTEMS ---`, `--- CAMERA ---`, `--- ENVIRONMENT ---`, `--- GAMEPLAY ---`, `--- UI ---`. Les objets créés en jeu sont rangés sous `Runtime/Enemies`, `Runtime/Projectiles`, `Runtime/FX`.
- **Nommage dans l'éditeur** : GameObjects, prefabs et assets en PascalCase anglais (`PlayerShip`, `EnemyData_GuardDrone`, `WaveData_M1_Tunnels`).
- **Import des sprites** : réglages pixel art du cahier des charges §3 (Sprite, Point, sans compression, sans mip maps, PPU 48), appliqués par `Editor/PixelArtImportPostprocessor` à toute texture de `_Project/` et `ThirdParty/`, **au premier import seulement** (pour ne jamais écraser un réglage manuel de Dyllan). Le pivot se règle par sprite.
- **Caméra de chaque scène** (sous `--- CAMERA ---`) : orthographique, taille 5,625 (11,25 u de haut, §4), fond noir, HDR et MSAA désactivés.
- Aucune référence manquante (`Missing`) dans une scène ou un prefab livré.
- **Assets** : les packs achetés sont importés dans `Assets/ThirdParty/` (inventaire, mesures et choix : `docs/notes/import-assets.md`). Utilise-les à la place de placeholders. Le vaisseau du joueur est `Plane 07` de `DyLESTorm/AnimatedPixelShips`. Attention : les tailles du cahier des charges sont celles des cases ; les colliders suivent le contenu visible du sprite. Un placeholder reste permis quand aucun asset ne convient (formes de base, palette du jeu), rangé dans `Features/<Feature>/Art/Placeholder/` sous l'enfant `Visual` du prefab, et listé dans le compte rendu. Ne référence jamais un fichier de `_AssetInbox/` (dossier local hors projet).

