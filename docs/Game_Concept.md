An Astral Story : Ashar

**Game Concept**

*Version 2.4 — 26 septembre 2026*

Dylan Boiteux

Shoot'em up narratif — Unity 6 • PC • Solo • 2–3 heures

*« Évadé de la prison orbitale de Phobos IX, Kaal**'**varis Dorn traverse l**'**orbite de Mars pour briser les chaînes des opprimés de la Chienne Rouge. »*

## Changements de la v2.4 (par rapport à la v2.3, 26 sept. 2026)

- Commandes révisées. Clavier : déplacement ZQSD ou flèches ; tir J ; bouclier éphémère K ; impulsion de piratage L ; dash M ; pause Échap. Sur AZERTY, W, X, C et V (main gauche) font la même chose que J, K, L et M. Manette : stick gauche ou croix pour se déplacer ; tir A ; bouclier X ; impulsion LT ; dash RT ; pause Start.
- Espace n'est plus utilisé, y compris pour avancer un dialogue : la touche de tir (J ou W, A à la manette) fait avancer les répliques.
- Point ouvert : le bouclier éphémère devient une action du joueur (touche K). Sa durée, son éventuel cooldown et le comportement d'un appui sans charge disponible restent à définir.

## Changements de la v2.3 (par rapport à la v2.2, 26 sept. 2026)

- Un seul niveau de difficulté : les trois modes (Story, Normal, Arcade) sont supprimés. La difficulté se règle globalement, avec une densité de tirs réduite par rapport à l'ancien mode Normal.
- Les dialogues deviennent des pauses narratives : un dialogue Ink commence après une séquence de jeu ; les actions ennemies passent en attente, aucune vague n'apparaît, le décor continue de défiler, la musique continue en boucle et le vaisseau passe en pilote automatique. Le joueur lit à son rythme et avance avec le bouton de tir ; quand il ferme la fenêtre de dialogue, il reprend les commandes et l'action reprend. Aucun dialogue pendant l'action. Remplace la règle « le dialogue ne bloque jamais le gameplay ».
- La bulle de dialogue redevient pleine largeur en bas d'écran, façon Pokémon (jusqu'à 3 lignes), puisqu'elle ne recouvre plus l'action.
- Langue des dialogues au choix FR ou EN (options du jeu) : n'est plus une coupe possible.
- Voix off (coupe n° 0) : elle double des répliques clés des dialogues, et non plus des répliques de combat.

## Changements de la v2.2 (par rapport à la v2.1, 24 sept. 2026)

- Narration hybride : la bulle textuelle (Ink, bruitages 16-bit) reste le système de base. Les répliques de combat (15 à 20 sur la démo) sont doublées en voix off française, filtrée radio et enregistrée par un proche ; le texte de la bulle sert alors de sous-titre (FR/EN). Les temps calmes (briefing, respiration, fin de mission) restent en texte seul. La voix off devient la coupe n° 0, première sacrifiée.

- Bulle de dialogue réduite : 2 lignes au maximum, dans le tiers bas gauche de l'écran, semi-transparente, pour ne pas masquer la zone où évolue le vaisseau.

- Graze : récompense précisée (bonus de score et réduction du cooldown de l'impulsion de piratage) [Inventé, à valider au playtest S1].

- Échelle technique : résolution de référence 960×540 en Pixel Perfect Camera (PPU 48), soit un agrandissement ×2 en 1080p, choisie d'après la taille réelle des sprites (révision du 25/09). Détail dans le cahier des charges.

- Pacificateurs du Culte : confirmés, grâce aux robots du pack Futuristic Military Base (robots au sol, M1). [Inventé]

## Changements de la v2.1 (par rapport à la v2.0, sept. 2026)

- Direction artistique : pixel art (au lieu du dessiné-main / Stylized 2D Space Shooter Pack). Assets retenus : Futuristic Spaceship SHMUP Bundle (DyLESTorm, itch.io).

- Narration : bulle de dialogue textuelle en bas d'écran façon Pokémon, système Ink, bruitages 16-bit, sans voix. La mention « FR (VO) / EN (VOST) » est retirée : le jeu est FR/EN en texte uniquement.

- Mécanique ajoutée : graze (frôlement de projectile récompensé), au service du pilotage agressif de Kaal'varis.

- Musique retenue : Cyberpunk Synthwave (Aleksis Tristan Shaw, itch.io) — 10 pistes dark synthwave bouclables. Remplace la description « synthwave astrale + percussions martiales + guitare metalcore ».

- Le prologue d'évasion à pied dans Phobos IX (2D de profil) est retiré du périmètre de ce Concept : il devient un second projet distinct, lié au même préquel, traité séparément (voir Annexe B).

# 1. Le jeu en une phrase

Évadé de la prison orbitale de Phobos IX, Kaal'varis Dorn traverse l'orbite de Mars pour briser les chaînes des opprimés de la Chienne Rouge. Un shoot'em up explosif où chaque mission révèle une part de l'intrigue via dialogues radio et codex secrets.

## Le pitch en trente secondes

Issu du préquel d'Anandavira « Les Exilés d'Ashar », ce spin-off suit Kaal'varis Dorn, ancien messager du Conseil pléiadien, hybride terrien-taygetien, capturé par l'Assemblée Terrienne / le Culte et condamné à l'exil sur Mars. Détenu dans la prison orbitale de Phobos IX, il a rassemblé les exilés, augmentés de force et réduits à des outils d'extraction. Grâce à une technologie pléiadienne récupérée, il s'empare d'un vaisseau terrien et s'évade avec eux. Des tunnels de « La Chienne Rouge » à l'orbite martienne, il affronte les drones de garde de LEASH, l'IA carcérale, puis les chasseurs et croiseurs de l'Assemblée Terrienne. Derrière l'action arcade, la radio et le codex révèlent la manipulation de l'humanité par l'Assemblée et teasent le roman. La démo se clôt sur le message crypté des Exilés, envoyé aux séditieux.

# 2. Fiche technique

| **Champ** | **Détail** |
| --- | --- |
| Titre provisoire | An Astral Story : Ashar |
| Genre | Shoot'em up narratif (arcade 2D vue de dessus + dialogues radio textuels) |
| Plateformes | PC (Windows), export Itch.io |
| Durée estimée | 20–30 minutes pour la démo (2 niveaux, environ 5–8 minutes chacun), 2–3 h pour la version complète |
| Calendrier | Pré-production du 21 au 30 septembre 2026 ; développement du 1er octobre au 25 novembre 2026 (8 semaines) |
| Langues prévues | Dialogues et interface en français ou en anglais, au choix dans les options ; voix off française sur des répliques clés (coupe n° 0), sous-titrée ; bruitages de dialogue 16-bit |
| Public cible | 12+, fans de SF mystique, shoot'em up arcade, narratif indé |
| Moteur | Unity 6 (URP), C# |
| Assets de base | Futuristic Spaceship SHMUP Bundle (DyLESTorm, itch.io) : vaisseaux et vaisseaux animés, portraits (non utilisés, cf. narration), vaisseaux ennemis, fonds spatiaux et urbains futuristes, explosions/VFX, pixel art 48×48 px. Musique : Cyberpunk Synthwave (Aleksis Tristan Shaw, itch.io). HUD et bulle de dialogue développés en interne (Unity). |
| Sauvegarde | Auto-save par mission |
| Builds prévus | Windows (.exe) + Itch.io |
| Perf cible | 1080p / 60 fps, rendu Pixel Perfect Camera (PPU 48, référence 960×540) |

# 3. Ce que le jeu raconte vraiment

Créer un jeu nerveux, immédiat et fun à jouer, tout en intégrant des bribes de narration forte. Chaque mission est un chapitre, chaque boss un symbole du système qui enchaîne les exilés. La frustration de la fin de la démo pousse le joueur à aller chercher les réponses dans le roman.

Époque : 2176 à 2183 (acte 3 du préquel, « La Révolte d'Ashar »). La version complète peut remonter à l'acte 2 ou avancer vers l'arc Sublimation (Icebox).

Résumé du monde : l'Assemblée Terrienne et le Consortium Minier Martien exploitent la Lune et Mars. Les dissidents, criminels politiques et réfugiés sont envoyés à Phobos IX, où ils travaillent de force et servent de cobayes. La Fédération galactique impose la non-intervention aux Pléiadiens, qui observent la Terre depuis l'orbite et n'aident les résistants qu'à distance. Dans l'ombre, le Culte de l'Ascension Cybernétique règne déjà sur le système solaire, dont l'Assemblée n'est que la façade ; ses Pacificateurs patrouillent et arrêtent les hérétiques. À mesure que sa technologie progresse, le Culte capture les Pléiadiens d'Ashar, les déclare infidèles et les déporte vers les colonies pénitentiaires orbitales de Mars, de Vénus et de Saturne.

Personnages principaux : Kaal'varis Dorn, héros et pilote, hybride terrien-taygetien, ancien messager du Conseil pléiadien, expert en combat spatial et capable de pirater les systèmes terriens [Canon ; compétences de combat et de piratage Adaptées au gameplay] ; le Chacal, ancien militaire, initiateur d'un projet d'évasion collectif selon les rumeurs de Phobos IX, relais radio et ailier [Inventé à partir d'une rumeur du préquel] ; la faction dissidente pléiadienne, qui offre l'opportunité d'évasion et envoie le message « Nous vous surveillons. La porte s'ouvrira bientôt. » [Canon, préquel] ; LEASH, IA carcérale de Phobos IX, ancien assistant domotique agricole modifié, antagoniste et voix ennemie [Canon, préquel ; rôle de boss Inventé] ; l'Assemblée Terrienne et le Consortium Minier Martien, autorités de Phobos IX, qui envoient chasseurs et croiseurs de poursuite [Canon, préquel ; unités Inventées] ; les Pacificateurs du Culte, qui gardent Phobos IX, dans une version plus rudimentaire que celle d'Anandavira [Inventé, à finaliser selon les assets].

Connexion à Anandavira : préquel de l'arc Sublimation. Les Exilés d'Ashar réapparaissent dans le Scénario (6.5, 11.1, 14.1) et à bord du Celestial Aeon en 2213 : la démo donne au joueur l'origine d'un personnage majeur du roman, sans en dévoiler les révélations.

## Règle de canon

Les bibles, le Scénario et le document préquel « Les Exilés d'Ashar » (20/02/2025) décident de ce qui est vrai dans l'univers ; ce Concept décide de ce que le jeu fait. Chaque élément narratif de la structure (§6) porte un statut :

- Canon : présent tel quel dans le Scénario, les bibles ou le document préquel.

- Adapté : ajusté pour le gameplay (échelle, mise en scène) sans contredire le canon.

- Inventé : absent du canon, à valider avant publication.

Principe : canon adaptable, structure narrative stable. Les personnages, les enjeux et les événements-clés restent fidèles au canon ; les décors, vaisseaux et ennemis peuvent être modifiés pour suivre les assets disponibles, avec le statut Adapté ou Inventé, sans contredire ni dévoiler une grande révélation du roman.

Finalité du projet : démontrer le game design, le level design et le narrative design (portfolio). Chaque choix d'adaptation est documenté. Ce Concept reste un ensemble d'idées : rien n'y est figé, et le contenu final dépend des assets retenus.

## La question centrale (formulation à valider)

Que reste-t-il de libre arbitre face à un système qui contrôle les IA, les corps et les récits ? Les exilés augmentés de force et LEASH, une IA qui garde ses prisonniers, en sont les deux faces.

# 4. Les piliers

| **Pilier** | **Description** |
| --- | --- |
| Action arcade nerveuse | Déplacement 8 directions, tir principal, arme secondaire, dash/esquive, bouclier éphémère, graze (frôlement de projectile récompensé) et power-ups au service d'un rythme immédiat et d'un pilotage agressif. |
| Narration en respiration | Dialogues en bulle textuelle entre les vagues, voix off sur des répliques clés et codex débloqués à chaque mission, qui tissent un récit plus grand connecté au roman Anandavira. Un dialogue est une pause narrative : les actions ennemies sont en attente, le vaisseau vole en pilote automatique, la carte défile, la musique continue ; le joueur lit à son rythme et reprend la main en fermant la fenêtre. Jamais de dialogue pendant l'action ; le codex se consulte hors action. |
| Progression maîtrisée | Un seul niveau de difficulté, réglé pour rester accessible (densité de tirs modérée, continues illimités) sans diluer l'intensité arcade, et un HUD simple. |
| Vitrine de design | Le projet sert de démonstration de game design, de level design et de narrative design : les fiches, tables de vagues et choix d'adaptation sont documentés et présentables. |

# 5. À quoi ça ressemble, manette en main

Vue : 2D verticale vue de dessus, pixel art (sprites 48×48 px), décors en couches, rendu en Pixel Perfect Camera.

Mécaniques principales : déplacement 8 directions, tir principal (rafale), arme secondaire (cooldown), dash/esquive (invulnérabilité courte), graze (zone de frôlement distincte de la hitbox létale : chaque projectile frôlé rapporte des points et réduit le cooldown de l'impulsion de piratage, ce qui récompense le pilotage agressif), bouclier éphémère, power-ups (armes, drones alliés).

Interface : HUD simple (vies, score, jauges) développé en interne ; dialogues affichés en bulle textuelle pleine largeur en bas d'écran, façon Pokémon (système Ink, jusqu'à 3 lignes) ; défilement caractère par caractère avec bruitage 16-bit, un timbre distinct par interlocuteur ; le bouton de tir complète la ligne puis passe à la suivante. Pendant un dialogue, l'action est suspendue : actions ennemies en attente, décor qui défile, musique qui continue, vaisseau en pilote automatique jusqu'à la fermeture de la fenêtre. Des répliques clés sont doublées en voix off filtrée radio (coupe n° 0).

Commandes : clavier (ZQSD ou flèches pour se déplacer ; J, K, L, M pour tirer, protéger, pirater et esquiver ; W, X, C, V équivalents sur AZERTY) et manette.

Style graphique : pixel art (Futuristic Spaceship SHMUP Bundle, DyLESTorm), fonds sombres, Mars en arrière-plan atténué. Règle de lisibilité : 3 couleurs de projectiles maximum par phase de boss.

Vaisseau du joueur : un vaisseau terrien volé, modifié avec la technologie pléiadienne récupérée. Kaal'varis, expert en combat spatial, le pilote. [Adapté : le préquel dit seulement que les exilés s'emparent d'un vaisseau terrien]

Arme secondaire : l'impulsion de piratage (zone, cooldown) : Kaal'varis pirate les systèmes terriens. Elle détruit les drones de garde, étourdit les chasseurs et n'a aucun effet sur les boss. [Adapté] Un laser pléiadien, obtenu plus tard, complète l'arsenal (power-up).

## La boucle de jeu

Chaque niveau est une mission scénarisée d'environ 5 à 8 minutes (briefing court, vagues d'ennemis entrecoupées de dialogues en bulle textuelle, respiration narrative avant le climax, boss, codex) : le joueur débloque une entrée de codex à l'issue de la mission, puis enchaîne sur la suivante. Sur la démo, ce cycle se répète deux fois pour un total de 20 à 30 minutes. Les dialogues s'insèrent entre les vagues, jamais pendant.

# 6. La structure

## Démo — Mission 1 : Phobos IX, « La Chienne Rouge » (2176–2183)

| **Champ** | **Détail** |
| --- | --- |
| Lieu | Orbite basse de Mars, complexe pénitentiaire Phobos IX : dômes géodésiques reliés par des tunnels creusés dans la roche. Le scroll monte du hangar vers la surface, puis vers l'espace. |
| Ancrage canon | Préquel (acte 3 et fiche Phobos IX) : évasion avec un vaisseau terrien, 3 tourelles automatiques à plasma, satellites sentinelles, IA carcérale LEASH. [Canon] |
| Combat | 1) Tunnels : drones de garde de LEASH, des machines agricoles reconverties. [Inventé] 2) Dômes : Pacificateurs en garde [Inventé, selon les assets], chasseurs de patrouille et satellites sentinelles. 3) Sortie : les 3 tourelles à plasma à détruire, comme trois verrous. 4) Mini-boss. Tutoriel diégétique : le Chacal explique les commandes par un échange textuel bref. |
| Mini-boss | Navette de garde pénitentiaire : canons en proue, puis largage de drones. [Inventé] |
| Dialogue | Kaal'varis, le Chacal, LEASH — bulle textuelle entre les vagues, bruitage synthétique distinct pour LEASH (menaçant, mécanique) ; voix off sur des répliques clés, LEASH passée au vocodeur. |
| Fin et codex | Le vaisseau franchit l'orbite. Codex (piste) : Phobos IX et LEASH. Teaser : que devient une IA de ferme quand on l'arme ? |
| Coût | Moyen. |

## Démo — Mission 2 : La course hors de Mars (2176–2183)

| **Champ** | **Détail** |
| --- | --- |
| Lieu | Orbite martienne puis champ de débris : poursuite vers les confins du système solaire. |
| Ancrage canon | Préquel (acte 3) : les exilés fuient Mars et trouvent refuge aux confins. [Canon] |
| Combat | 1) Chasseurs et intercepteurs de l'Assemblée Terrienne et du Culte, le Chacal en ailier. [Adapté] 2) Satellites sentinelles à désactiver. 3) Boss. LEASH pirate le HUD par un glitch bref (effet cosmétique, jamais d'inversion des commandes). |
| Boss | LEASH, croiseur de poursuite : LEASH prend le contrôle du croiseur le plus proche, dont les drones sont retenus par des chaînes d'énergie. Phase 1 : drones enchaînés en rotation. Phase 2 : les chaînes coupées libèrent des drones à tête chercheuse. Phase 3 : le noyau exposé. Phase 4 optionnelle : LEASH tente de couper la transmission des exilés (Coupe 1). [Inventé] |
| Dialogue | Kaal'varis, le Chacal, la faction dissidente (« Nous vous surveillons. La porte s'ouvrira bientôt. ») entre les vagues ; LEASH aux transitions de phase du boss ; voix off sur des répliques clés, texte seul pour le message de la faction et la fin. |
| Fin et codex | Le message crypté des exilés est envoyé sur le réseau CSM : « Nous sommes les Exilés d'Ashar. Et nous reviendrons briser les chaînes des opprimés de la Terre. » Codex (piste) : l'Assemblée Terrienne et les Exilés. [Canon] |
| Coût | Moyen à élevé (boss à phases avec chaînes). |

## Version complète (pistes, Icebox, non estimées)

| **Piste** | **Ancrage** | **Statut** |
| --- | --- | --- |
| Convoi Phobos → Lune | Acte 2 : l'opération de transport et le contact avec la faction dissidente | Canon + Inventé |
| Le Croc | Station orbitale fantôme reliée à Phobos IX par un tunnel (rumeur du préquel) | Inventé |
| L'artefact de Phobos | Artefact non humain réveillé par les forages (rumeur du préquel) | Inventé |
| Épisode suivant : Sublimation | Arc d'Andrew Jackson (2213) : Bibles, Scénario, Annexe A du Concept v1.2 | Canon |

## Ennemis, objets, calendrier

PNJ/ennemis : drones de garde de LEASH, chasseurs de l'Assemblée Terrienne et du Culte (et intercepteurs, variante de comportement), navette de garde (mini-boss), tourelles à plasma, satellites sentinelles, LEASH croiseur de poursuite (boss), Pacificateurs du Culte (selon les assets). Détail et statistiques : Bestiaire démo v2 (document séparé).

Objets/équipements : power-ups offensifs (multi-shot, laser pléiadien), défensifs (bouclier), drones de garde piratés en alliés, codex narratif.

Pré-production (21–30 septembre) : tous les livrables narratifs et techniques sont terminés avant le développement, sans travail en parallèle.

Étapes de développement (8 semaines, du 1er octobre au 25 novembre 2026) :

- S1 (1–7 oct) proto déplacement + tir + graze ; premier build Windows.

- S2 (8–14 oct) ennemis + boucle arcade ; point de contrôle 1 (performance 1080p/60).

- S3 (15–21 oct) dialogues en bulle (Ink) + codex ; sauvegarde par mission ; menus.

- S4 (22–28 oct) niveau 1 complet ; point de contrôle 2 (coupes).

- S5 (29 oct–4 nov) boss + power-ups.

- S6 (5–11 nov) niveau 2 complet ; point de contrôle 3 (coupes).

- S7 (12–18 nov) DA + audio (dont l'enregistrement de la voix off, si elle n'est pas coupée) + polish.

- S8 (19–25 nov) export build + teaser + publication + dossier de design.

## Liste de coupes (ordre de sacrifice)

Toute nouvelle idée est comparée à cette liste : ajouter quelque chose, c'est en retirer autre chose. Si la performance n'est pas tenue au point de contrôle 1, on plafonne d'abord le nombre de projectiles avant toute coupe de contenu.

| **Rang** | **Élément** | **Gain (½ j)** | **Point de contrôle** | **Déclencheur** |
| --- | --- | --- | --- | --- |
| 0 | Voix off des répliques clés | 2,5 | Fin S6 (11 nov) | Retard cumulé, M2 non jouable de bout en bout, ou séance d'enregistrement non calée |
| 1 | Phase 4 optionnelle du boss LEASH | 1 | Fin S4 (28 oct) | Retard cumulé de plus de 2 ½ j ou M1 non jouable |
| 2 | Glitch HUD de LEASH | 0,5 | Fin S6 (11 nov) | M2 non jouable de bout en bout |
| 3 | Drones de garde piratés (alliés) | 1,5 | Fin S4 (28 oct) | Retard cumulé de plus de 2 ½ j ou M1 non jouable |
| 4 | Laser pléiadien (power-up) | 1 | Fin S4 (28 oct) | Retard cumulé de plus de 2 ½ j |
| 5 | Le Chacal ailier scripté (dialogue seul) | 1 | Fin S6 (11 nov) | M2 non jouable de bout en bout |
| 6 | Mécanique de graze | 1 | Fin S2 (14 oct) | Retard cumulé de plus de 2 j dès le proto S1 |

Livrable démo : 2 niveaux complets + codex + dialogues.

# 7. Pourquoi ce jeu et pas un autre

Références : Ikaruga, Nier Automata (phases shmup), Gradius, Hellsing (direction artistique sombre) : un shoot'em up qui assume une identité narrative forte plutôt qu'un simple défouloir arcade.

Nom de studio : Anandavira Studio (indé)

Canaux : TikTok, Itch.io, YouTube, Discord

Contenus : devlogs courts, extraits gameplay, OST teasers

## Le détail qui n'existe nulle part ailleurs

Le codex narratif débloqué à l'issue de chaque mission est le pont ludique vers le roman Anandavira. « Ashar » raconte l'origine des Exilés, ceux qui reviendront dans l'arc Sublimation : le joueur apprend qui est Kaal'varis avant de le croiser dans le roman. Le projet est aussi une vitrine : un dossier de design (Concept, bestiaire, tables de vagues, journal des adaptations) accompagne la démo.

# 8. Le ton

Références visuelles : Nier Automata, Hellsing Ultimate, Evangelion (vaisseaux/colosses) — réinterprétées en pixel art.

Ambiances sonores : espace, bruitages de dialogue 16-bit, voix radio filtrées, alarmes, explosions, IA carcérale.

Musique retenue : Cyberpunk Synthwave (Aleksis Tristan Shaw, itch.io) — dark synthwave, 10 pistes bouclables (~33 min).

Effets sonores : tirs secs, lasers, impacts métalliques.

Voix/narration : bulle façon Pokémon (système Ink) avec bruitages 16-bit, entre les vagues ; voix off française filtrée radio sur des répliques clés, sous-titrée par la bulle (coupe n° 0).

Palette : fonds sombres, vaisseau du joueur bleu, projectiles du joueur argent-blanc, ennemis rouge orangé, boss LEASH magenta et jaune. Mars en arrière-plan atténué, jamais en aplat rouge sous les projectiles.

# Annexe A — Chronologie de Kaal'varis Dorn (source de vérité du jeu)

Sources : Bible des Personnages, document préquel (20/02/2025), Scénario. La colonne « Jeu » indique où l'événement apparaît.

| **Date · lieu** | **Événement** | **Source** | **Jeu** |
| --- | --- | --- | --- |
| 2070 · colonie proche de l'espace pléiadien | Naissance ; hybride terrien-taygetien issu d'une alliance secrète. | Bible | Codex |
| Avant 2155 · Conseil pléiadien | Il plaide pour une intervention sur Terre ; la Fédération impose la non-intervention ; il quitte les Pléiades et s'engage auprès des résistants humains. | Préquel (prologue) | Codex |
| 2155–2161 · Terre, Lune | Contact avec des scientifiques et militaires progressistes ; il découvre que les colons de l'extraction sont des prisonniers ; capturé, jugé, exilé sur Phobos. | Préquel (acte 1) | Codex ; Icebox |
| 2162–2176 · Phobos IX | Détenus augmentés de force ; résistance souterraine ; opération de transport vers la Lune et contact avec la faction dissidente : « Nous vous surveillons. La porte s'ouvrira bientôt. » | Préquel (acte 2) | Dialogue M2 ; Icebox |
| 2176–2183 · Phobos IX puis orbite de Mars | Grâce à la technologie pléiadienne récupérée, les exilés prennent un vaisseau terrien et fuient ; refuge aux confins ; message crypté sur le réseau CSM. | Préquel (acte 3) | M1, M2, fin de démo |
| Vers 2188–2198 · confins, Mars | Dix années d'exil ; il sauve Jake de son EMI et le rallie aux Exilés. | Scénario 6.5, Bible | Hors jeu (roman) |
| 2213 · Celestial Aeon | Les séditieux rescapés vivent avec les Exilés d'Ashar. | Scénario 11.1 | Hors jeu (Sublimation) |
| 2222 | Selon la Bible, il mène l'assaut décisif et se sacrifie. | Bible | Jamais évoqué (pont, pas spoiler) |

# Annexe B — Points ouverts et incohérences du canon

Durée de l'exil : le Scénario 6.5 parle de « dix dernières années d'exil » en 2198, alors que le préquel place la prison de 2162 à 2176 et l'évasion et le refuge aux confins de 2176 à 2183 (acte 3). L'exil du Scénario désigne l'errance des Exilés après l'évasion, que le préquel ne raconte pas : les deux versions sont compatibles (confirmé par l'auteur, 23/09/2026). Cette période d'épreuves reste à écrire.

Origine de Kaal'varis : Pléiadien siégeant au Conseil pléiadien (préquel, Prologue ; jamais qualifié de militaire, ce terme y désigne le Chacal) et « hybride terrien-taygetien né dans une colonie proche de l'espace pléiadien » (Bible) : versions compatibles, Taygeta appartenant aux Pléiades. La mention « ancien militaire » est retirée du Concept.

Orthographe : « Kaal'vaaris » (Bible des Vaisseaux) contre « Kaal'varis » (Scénario, Bible des Personnages). Choix retenu : Kaal'varis.

Prothèse au bras gauche : la Bible la mentionne, sans date ni origine ; le préquel n'en parle pas.

Éléments du préquel absents des bibles : Phobos IX, LEASH, le Chacal, Le Croc, l'artefact non humain, l'Assemblée Terrienne et le Consortium Minier Martien. Ils sont traités comme Canon (document préquel) pour le jeu ; à intégrer aux bibles.

Vaisseaux de 2176 : la Bible des Vaisseaux ne couvre pas cette époque ; tous les vaisseaux et ennemis du jeu sont Inventés ou Adaptés.

Test d'import du pack d'assets : le Futuristic Spaceship SHMUP Bundle (DyLESTorm) est nativement pixel art ; test d'import et de configuration (Pixel Perfect Camera, PPU 48) en Unity 6 URP à réaliser (remplace le test du Stylized 2D Space Shooter Pack, abandonné).

Voix off : enregistrée par un proche, en français seulement ; la date de la séance doit être réservée avant la fin de S6, faute de quoi la coupe n° 0 s'applique. Si la coupe s'applique, les dialogues restent en texte seul (fonctionnement de la v2.1).

Arc d'Andrew (Sublimation) : en Icebox ; la chronologie d'Andrew de la v1.2 reste valable comme référence.

Prologue d'évasion à pied (Phobos IX, 2D de profil) : envisagé un temps comme séquence intégrée à la Mission 1, il est retiré de ce Concept (24/09/2026) pour préserver le périmètre et le calendrier de la démo. Il devient un second projet distinct, lié au même préquel « Les Exilés d'Ashar », à l'état de pitch, sans backlog ni développement avant la livraison de cette démo.

Pistes selon les assets (idées non figées) : vaisseaux de l'Assemblée Terrienne et du Culte sur Mars et en orbite ; une station orbitale (Le Croc ?) ; un lieu d'exil encore à situer, pour les épreuves postérieures à l'évasion (inspiration Riddick) ; des créatures d'une densité supérieure, présentes dans le lore mais absentes du monde physique.

Fiches à créer : boss LEASH, tables de vagues M1 et M2, textes de dialogue et codex, Culte de l'Ascension Cybernétique et Pacificateurs (précisés par l'auteur, absents du préquel et des bibles consultées ; ceux de 2176 doivent rester technologiquement en retrait par rapport à ceux d'Anandavira).