# An Astral Story : Ashar

Shoot'em up narratif 2D à scroll vertical, en pixel art, développé en solo sous **Unity 6 (URP, C#)**.

![Statut](https://img.shields.io/badge/statut-en%20d%C3%A9veloppement-orange)
![Moteur](https://img.shields.io/badge/moteur-Unity%206-black?logo=unity)
![Plateforme](https://img.shields.io/badge/plateforme-Windows-blue)

🎮 **Jouer sur itch.io** : lien à venir, une fois la démo prête.

## À propos

Spin-off du roman *Anandavira*, situé dans son préquel *Les Exilés d'Ashar*. Le joueur incarne **Kaal'varis Dorn**, ancien messager du Conseil pléiadien condamné à l'exil et détenu dans le complexe pénitentiaire orbital **Phobos IX**. Avec les exilés qu'il a rassemblés, il s'empare d'un vaisseau terrien et s'évade à travers l'orbite martienne — chaque mission dévoilant, via des dialogues en bulle (Ink), une voix off et des entrées de codex, une part d'une intrigue plus vaste de manipulation de l'humanité.

### Gameplay

- Arcade vertical, déplacement 8 directions, tir principal + arme secondaire (piratage)
- Dash/esquive avec invulnérabilité courte, bouclier éphémère, power-ups
- Narration en toile de fond : dialogues en bulle entre les vagues, codex débloqué en fin de mission
- Un seul niveau de difficulté

### Lien avec l'univers Anandavira

Ce projet est un pont vers le roman *Anandavira*, pas un résumé : les dialogues et le codex teasent l'univers sans en dévoiler les grandes révélations.

## Contrôles

Le [mapping v3.1 définitif](docs/specs/E3-00.md) remplace tous les anciens schémas.
Clavier : ZQSD et flèches simultanément actifs ; tir Espace, bouclier Maj gauche/droite, impulsion Ctrl gauche/droite, dash F, menus Entrée, codex Tab, pause Échap.
Manette Xbox : déplacement stick/croix ; tir A, bouclier X, dash B, impulsion Y, menus A, codex View, pause Menu. Gâchettes et bumpers inutilisés pour le gameplay core.
Implémentation et validation prévues dans E3-00, préalable obligatoire à E3-01 ; mécanique du bouclier encore à spécifier.

## Ce que ce projet m'a appris

À compléter en fin de projet.

## Points techniques

À compléter au fil du développement.

## Tech stack

- Unity 6.6, Universal Render Pipeline (2D Renderer), Pixel Perfect Camera
- C#, architecture par ScriptableObjects (EnemyData, WaveData, MissionData, PowerUpData, CodexEntry)
- Input System, TextMeshPro, Ink (inkle) pour les dialogues
- Unity Test Framework (tests EditMode)
- Cible : PC Windows, 1080p/60 fps, export Itch.io

## Assets tiers

Les packs achetés ne sont **pas inclus dans ce dépôt** : leurs licences interdisent la redistribution, y compris sous forme retouchée. Ils sont rangés dans `Assets/ThirdParty/`, exclu par le `.gitignore`. Un clone du dépôt affiche donc des sprites manquants : c'est normal.

| Pack | Auteur |
| --- | --- |
| Futuristic Spaceship SHMUP Bundle (vaisseaux, ennemis, fonds, explosions) | DyLESTorm (itch.io) |
| Sci-fi Turret Sprite Pack | Felmir (itch.io) |
| Futuristic Military Base, Pixel Mars Base Tileset | Cute SCKR (itch.io), packs signalés *AI Assisted* |
| Cyberpunk Synthwave (musique) | Aleksis Tristan Shaw (itch.io) |

## Méthode

Développé avec **Claude Code**, story par story : chaque story du backlog fait l'objet d'une spec (`docs/specs/`), implémentée sur sa propre branche, puis testée et validée par moi avant fusion. La conception (game, level et narrative design), les arbitrages et les tests restent humains.

## Statut

🚧 En développement — démo jouable (2 missions de 5 à 8 minutes) visée pour fin novembre 2026.

Développé seul, en 8 semaines (pré-production fin septembre, développement du 1ᵉʳ octobre au 25 novembre 2026), dans une logique de **portfolio** : démontrer le game design, le level design et le narrative design du jeu.

## Auteur

**Dyllan Démétrios**

## Licence

Projet personnel à but non commercial, distribué gratuitement sur [itch.io](#) une fois la démo prête. Lien à venir.
