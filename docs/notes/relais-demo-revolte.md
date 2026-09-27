> **État actuel :** E3-02 implémentée selon compte rendu ; essais physiques et pause intégrée à confirmer. E3-09 prête : survie sans score visible. Dialogues ENTRE les combats, sans Ink. Entrée (UI/Submit, A manette) avance une réplique ; dernière validation puis combat. Vol, décor, musique et animations continus, aucune attaque ni nouvelle vague. Aucun timer de lecture ni validation par le tir. Tutoriel distinct : action demandée ou validation. Bouclier : 5 s de protection, puis 20 s de cooldown à compter de la fin de la protection. Pendant ces 20 s, aucune protection du bouclier. Les appuis pendant protection ou cooldown sont ignorés, sans prolongation ni mise en attente. Après recharge, nouvelle pression nécessaire ; maintien sans réactivation automatique. E3-06 est spécifiée après E3-09 ; E5-05 est spécifiée après E3-06 ; E5-06 et E3-08 sont spécifiées pour prototype, dépendances à livrer.

# Relais documentaire — Révolte d'Ashar

## Source et état

Auteur de la note : Dyllan. Intégration documentaire : Codex.
Source intégrale : docs/Demo_Revolte_Ashar.md, prioritaire pour les points remplacés.
Base Git actuelle : main, 13c17fd. Auteur actuel : Codex, préparation documentaire E3-09 et mise à jour du suivi ; modifications non commitées. Claude Code reste l'auteur de l'implémentation Unity.
Le Word du préquel, les assets Unity et les comptes rendus existants restent inchangés.

## Décisions de la note

Début acte 3 déjà en vol ; Chacal hors démo ; survol de la surface martienne ; Amara tactique et Kaal’varis commandement ; dialogues manuels entre combats, vol continu ; tutoriel et chargement initial de 45 s ; deux boss avec vague d'élite ; libération de captifs devenant alliés ; épilogue jouable auprès d'Amara.
La déclaration finale intervient plus tôt que dans la chronologie longue du préquel : adaptation de Dyllan pour cette démo, pas modification du roman.
La transition Phobos / surface martienne reste à mettre en scène ; ne pas supposer une continuité géographique littérale.

## Décisions encore ouvertes

- Bouclier résolu : Bouclier : 5 s de protection, puis 20 s de cooldown à compter de la fin de la protection. Pendant ces 20 s, aucune protection du bouclier. Les appuis pendant protection ou cooldown sont ignorés, sans prolongation ni mise en attente. Après recharge, nouvelle pression nécessaire ; maintien sans réactivation automatique. Les 20 s sont demandées par Dyllan ; les 5 s sont conservées. Recharge après protection et appuis ignorés précisent sa demande de ne pas être protégé en permanence.
- Champ magnétique : chargement initial 45 s imposé ; correspondance avec Pulse / ancienne impulsion et effets hors tutoriel à confirmer avant code. Ne pas ajouter implicitement un second pouvoir.
- Tutoriel : comportement si le compteur termine avant les consignes, si les cibles sont déjà détruites, ou si la dernière consigne est passée sans activation. Des cibles inoffensives permettent de montrer le bouclier mais pas de valider son absorption.
- Dialogues résolus : E3-02, Entrée/A, aucun timer, vol continu et combat suspendu ; sans Ink.
- Nombre de vagues/captifs/attaches, seuil de libération, statistiques, patterns, durées et formulations ouvertes : validation de Dyllan requise.

## Répartition et planning autorisés

Dyllan a explicitement confié la gestion du projet à Codex et le développement à Claude Code. La refonte documentaire du backlog est autorisée ; le refus automatique antérieur est levé par cette autorisation. Aucun échange automatique avec Claude n'est supposé.

Les IDs existants et comptes rendus sont conservés. E3-08 est ajouté pour assembler le tutoriel après les capacités. Les estimations des tâches remaniées sont « À réestimer » : pas de promesse fondée sur les anciennes demi-journées. Les semaines historiques non remaniées ne sont pas des engagements mis à jour.

Ordre de réalisation (dépendances du CSV prioritaires sur les numéros de story) :
1. Confirmer les essais physiques E3-00 ; ne pas réimplémenter le mapping. E3-01 est livré sur main (732c34f) ; E3-02 est implémentée (9052951 et 13c17fd), sans nouvelle validation gameplay déduite. E3-09 est la prochaine spec prête.
2. Lot A : affichage radio E3-01, dialogues linéaires E3-02 selon sa spec, retrait du score visible E3-09, puis menus E3-06 (spec prête). Les dialogues fournis suffisent aux essais de présentation ; P0-09 reste requis pour leur intégration finale.
3. Cadrage : P0-16 inspecte les assets déjà présents sans refaire le bootstrap P0-12/P0-15 ; P0-07/P0-08 précisent boss et séquences ; P0-09/P0-10 préparent les contenus avec Dyllan.
4. Lot B : bouclier E5-05 (spec prête après E3-06) et champ E5-06 après arbitrages, cibles E4-02, puis tutoriel E3-08 ; environnement/vagues/boss Assemblée E4 ; intégration et playtests E3-07/E4-06.
5. Lot C : Culte, captifs alliés et assaut E5-01 à E5-04, puis intégration E6 et épilogue Amara ; playtest complet E6-07.
6. Lot D : codex/sauvegarde, localisation, polish, régressions et build. Publication uniquement après autorisation explicite.

Anciennes tourelles, phase transmission et glitch LEASH hors démo ; Chacal réservé aux actes 1 et 2. Drones piratés et laser optionnels différés, sans confondre les drones optionnels avec les captifs alliés obligatoires.
Les lignes P0-12, E1/E2 munies de comptes rendus et E3-00 portent « Implémenté selon compte rendu — validation Dyllan à confirmer ». Ce n'est ni une revalidation ni un PASS gameplay.
Un seul auteur actif sur le checkout ; aucune nouvelle scène ou mission imposée.

## Vérification et limites

Modifications documentaires uniquement. Aucun test Unity exécuté, aucun PASS gameplay.
Contrôles exécutés : CSV relu avec le module csv du Python fourni par Codex ; 85 lignes de tâches, 11 colonnes, IDs uniques, toutes les dépendances existantes, aucun cycle et aucune dépendance active vers une tâche hors périmètre. IDs historiques conservés ; seul ajout E3-08. Note identique à la pièce jointe et compte rendu E3-00 intégralement conservé. git diff --check réussi ; git diff --exit-code -- Assets Packages ProjectSettings confirme aucun changement Unity. Résultat documentaire uniquement.
Aucun commit ni push dans cette tâche.
## Réparation autorisée

Dyllan a autorisé la réparation des quatre documents depuis Git et les corrections actuelles ont été réappliquées. Copies des fichiers tronqués conservées hors dépôt. Aucun code ni asset Unity modifié.

## Reprise depuis idees.md — 27/09/2026

Prochaine action Claude : implémenter uniquement docs/specs/E3-09.md, puis remplir son compte rendu et arrêter pour le test de Dyllan.
Décisions reprises : objectif de survie sans score visible ; aucune fin intermédiaire dans le parcours final. Continuité distribuée entre P0-08, E4-05 et E6-06, vérifiée globalement en E6-07.
Choix réversible : conserver les données internes de points pour éviter une migration ; aucune modification des vies, continues ou graze.
Fichiers documentaires de cette reprise : CLAUDE.md, docs/backlog.csv, docs/CAHIER_DES_CHARGES.md, docs/Demo_Revolte_Ashar.md, docs/specs/E3-02.md, docs/specs/E3-09.md, docs/notes/idees.md et ce relais.
Le compte rendu E3-02 est conservé ; son ancien câblage TestBed est corrigé par une note de suivi (13c17fd).
Aucun test Unity exécuté par Codex ; aucun PASS gameplay. Les 155 tests rapportés sont ceux du compte rendu Claude, pas une nouvelle exécution.

Vérification de cette reprise : module Python csv, 86 stories / 11 colonnes, IDs uniques et dépendances sans cycle ; compte rendu E3-02 identique à HEAD. git diff --check réussi ; git diff --exit-code -- Assets Packages ProjectSettings sans différence. Modifications documentaires uniquement, non commitées.


## Préparation E3-06 — Codex

Spec docs/specs/E3-06.md rédigée : menu principal, pause, options persistantes, propriété des inputs, reprise exacte du dialogue et QA. Base main 13c17fd ; documentation non commitée.
Ordre Claude : E3-09 puis arrêt/validation ; E3-06 seulement à la demande suivante. E3-06 ne livre ni progression sauvegardée, ni codex, ni tutoriel final.
Hypothèses réversibles consignées dans la spec : structure des panneaux, confirmation d'abandon, musique conservée pendant pause, PlayerPrefs pour préférences uniquement si aucun stockage existant.
Prochaine préparation Codex : E5-05, avec vérification du multi-shot inclus dans la tâche ; E5-06/E3-08 demandent encore de clarifier le lien champ/Pulse et les cas limites du tutoriel.
Aucun code Unity modifié et aucun test gameplay exécuté.

Contrôle final : CSV 86 stories / 11 colonnes ; git diff --check réussi, aucun diff Assets/Packages/ProjectSettings. Le checkout a évolué pendant la préparation vers story/E3-09-hide-score, 242a627 (Claude). E3-09 implémentée selon son rapport, validation humaine restante. Aucun travail Claude écrasé ; modifications Codex limitées à E3-06.md, backlog.csv et ce relais, non commitées.


## Préparation E5-05 — Codex

Fiche bouclier prête : états Prêt/Actif/Recharge, 5 s puis 20 s, raccord dégâts, pause/dialogue, UI et critères QA. Hypothèses réversibles explicitement distinguées des durées validées : compteurs figés en dialogue, conservation de recharge au respawn, comportement des projectiles.
Multi-shot conservé, extrait en E5-10 : aucune suppression de contenu ; réglages/collecte à spécifier séparément. E5-08 et E5-09 historiques conservées. La séparation évite de bloquer bouclier et tutoriel avec une mécanique indépendante.
Ordre : E3-09, E3-06, E5-05 ; une story par demande, arrêt/validation entre stories. E5-06/E3-08 restent conditionnées à la clarification champ/Pulse et cas limites du tutoriel.
Base inspectée : story/E3-09-hide-score, 242a627 ; modifications documentaires non commitées. Fichiers de cette préparation : E5-05.md, backlog.csv, CAHIER_DES_CHARGES.md et ce relais ; préparation E3-06 précédente préservée.
Contrôle : CSV 87 stories / 11 colonnes, IDs uniques, dépendances existantes sans cycle. Aucun test Unity ni PASS gameplay.


## Décision champ/Pulse et préparation E5-06 / E3-08

Dyllan confirme : un seul pouvoir ; instant kill petites unités, stun gros vaisseaux et boss. Ancienne immunité boss remplacée. E5-06 et E3-08 prêtes pour prototype selon dépendances CSV.
Les mentions antérieures « lien Pulse à confirmer » de ce relais sont résolues par cette décision.
Hypothèses de prototype identifiées dans les specs : stun 3 s réglable ; rayon 3,4 u/recharge 45 s conservés ; tutoriel sans timeout, charge attendue sans sauter les consignes, skip final autorisé sans activation automatique, aucune victime requise.
Le jeu final et les boss réels restent à intégrer/tester ; aucun résultat de gameplay affirmé.
P0-16 continue de sélectionner/classer les ennemis réels ; n'empêche pas le prototype sur cibles explicitement configurées.
Codex : documentation uniquement, fichiers E5-06.md/E3-08.md, backlog, cahier, note démo, CLAUDE et relais. Aucun commit/push.


## Lot de préparation restant — Codex

Fiches P0-08/P0-09/P0-10, E3-03/E3-04/E3-05/E3-07 et E5-10 rédigées. Corpus docs/dialogues_revolte.csv : 14 répliques FR extraites sans réécriture de la note ; EN placeholder explicite, finalisation E7-04.
Séquencier : trame existante, groupes/espacements prototype identifiés ; codex : deux synthèses proposées exclusivement sur faits de la note. Sauvegarde : reprise simple aux débuts de segments, hypothèse de prototype détaillée dans E3-04.
Les mentions précédentes « specs E3-03 à E3-07 / E5-10 à rédiger » sont remplacées par ce lot. Dépendances d'implémentation toujours requises ; aucun statut Fait ni validation humaine inventé.
Claude peut prendre E3-06 puis E5-05/E5-06 ; E5-10 et intégration corpus E3-05 disposent aussi d'un contrat. E3-08 attend notamment les Pacificateurs E4-02. Les boss/environnements finaux restent des travaux distincts.
