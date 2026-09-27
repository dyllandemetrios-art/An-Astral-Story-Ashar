# An Astral Story : Ashar

**Shoot'em up narratif 2D en pixel art — projet de portfolio en technical game design.**

Je développe ce projet en solo sous **Unity 6 (URP, C#)**, avec une assistance IA pour la production technique. J'en assure la conception, la direction créative, les arbitrages et les playtests.

## Le jeu

Issu de mon univers *Anandavira*, **Ashar** adapte le début de l'acte 3 du préquel *Les Exilés d'Ashar*.

Le joueur incarne **Kaal'varis Dorn**, ancien messager du Conseil pléiadien, au moment où les détenus de **Phobos IX** prennent leur envol à bord de vaisseaux volés. La fuite les mène au-dessus de Mars, face aux forces de l'Assemblée Terrienne puis du Culte de l'Ascension Cybernétique.

Avec le soutien radio de la capitaine **Amara Solvik**, les fugitifs doivent ouvrir un passage et libérer une escadrille captive. L'action raconte la naissance d'un collectif : les Exilés d'Ashar.

## Mon rôle de technical game designer

Mon travail consiste à transformer une intention de jeu en comportements observables, en paramètres réglables et en critères de validation.

- **Game design** : boucle de combat, commandes, capacités, progression et équilibre entre prise de risque et lisibilité.
- **Technical design** : spécifications des systèmes, états et interactions, organisation des données et critères d'acceptation.
- **Level design** : composition des vagues, rythme des rencontres, apprentissage progressif et mise en scène des boss.
- **Narrative design** : adaptation de mon univers, rôle des personnages et articulation entre dialogues courts et action.
- **Itération dans Unity** : réglage des paramètres, sélection et intégration des assets, playtests et analyse des retours.
- **Pilotage de production** : définition du périmètre, priorisation du backlog et arbitrages pour aboutir à une démo courte et complète.

Un exemple central de cette démarche : les vaisseaux captifs du boss final doivent devenir des alliés une fois leurs entraves détruites. L'objectif est de rendre la libération immédiatement visible et utile dans le combat.

## Développement avec l'IA

J'utilise **Codex** pour m'accompagner dans la gestion du projet, la documentation, le découpage des tâches et les revues. **Claude Code** prend en charge l'implémentation technique à partir des spécifications, ainsi que les vérifications automatisées et les comptes rendus.

Le code est donc développé avec une contribution importante de l'IA. Mon apport porte sur la conception du jeu, la définition des comportements attendus, la direction du travail et la validation du résultat.

Le travail avance par fonctionnalités : intention, spécification, implémentation, vérifications, puis test en jeu. Je garde la décision sur le gameplay, le récit et les compromis de production. Les tests automatisés vérifient une partie du fonctionnement ; les sensations de contrôle, le rythme et la lisibilité se jugent en jouant.

## Gameplay visé pour la démo

- Déplacement à huit directions, tir principal, dash, bouclier et champ magnétique pléiadien.
- Tutoriel en vol : apprentissage par l'action pendant le chargement du générateur.
- Évasion collective depuis Phobos IX, puis survol de la surface de Mars.
- Forces de l'Assemblée blanches et rouges, suivies de vaisseaux du Culte plus sombres.
- Deux boss, dont un croiseur auquel sont enchaînés les vaisseaux à libérer.
- Dialogues courts entre les combats, validés manuellement pendant que le vol continue, et épilogue jouable.
- Une difficulté unique, ajustée par les playtests.

Ces éléments décrivent la cible de la démo ; leur intégration est en cours.

## Approche technique

- **Unity 6.6**, Universal Render Pipeline avec **2D Renderer** et rendu pixel perfect.
- **C#**, composants aux responsabilités ciblées et organisation par fonctionnalité.
- **ScriptableObjects** pour exposer les données de gameplay et faciliter les réglages dans l'Inspector.
- **Input System** pour séparer les actions de leurs bindings clavier et manette.
- **Unity Test Framework** pour vérifier la logique concernée.
- **Dialogues linéaires en ScriptableObjects**, textes français/anglais et validation manuelle.

**Cible actuelle : PC Windows**, avec un objectif de 1080p à 60 images par seconde.

## Documentation du projet

Le dépôt conserve les décisions de conception et les traces du développement :

- [Vision actuelle de la démo](docs/Demo_Revolte_Ashar.md)
- [Cahier des charges](docs/CAHIER_DES_CHARGES.md)
- [Backlog de production](docs/backlog.csv)
- [Spécifications et comptes rendus](docs/specs/)
- [Playtests et mesures](docs/playtests/)

## Statut

🚧 **En développement.** Démo gratuite destinée à itch.io.

L'objectif initial de livraison est fixé à **fin novembre 2026**. Le calendrier est à recalibrer selon le périmètre actualisé et les résultats des playtests. La durée finale de la démo reste à valider.

## Remerciements et assets

Merci aux créateurs dont les packs contribuent à l'identité visuelle et sonore du projet :

| Auteur | Contribution |
| --- | --- |
| **DyLESTorm** | *Futuristic Spaceship SHMUP Bundle* : vaisseaux, ennemis, décors spatiaux, projectiles et effets |
| **Felmir** | *Sci-Fi Turret Pack v2* : tourelles |
| **Cute SCKR** | *Futuristic Military Base* et *Pixel Mars Base Tileset* : installations et décors martiens |
| **Aleksis Tristan Shaw** | *Cyberpunk Synthwave* : musique |

Les packs de Cute SCKR sont signalés *AI Assisted* dans la documentation du projet.

Les assets tiers ne sont pas redistribués dans ce dépôt. Ils sont conservés dans le dossier Assets/ThirdParty/, exclu du suivi Git : un clone peut donc présenter des références manquantes sans les packs correspondants.

## Auteur et diffusion

**Dyllan Démétrios — Technical Game Design · Game Design · Narrative Design**

Projet personnel de portfolio, à but non commercial. Distribution gratuite prévue sur itch.io ; lien à venir.
Les assets tiers restent soumis aux licences de leurs auteurs.