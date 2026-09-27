# Relais documentaire — Révolte d'Ashar

## Source et état

Auteur de la note : Dyllan. Intégration documentaire : Codex.
Source intégrale : docs/Demo_Revolte_Ashar.md, prioritaire pour les points remplacés.
Base Git : fix/ship-bank-frames, 335d3a2. Modifications documentaires non commitées.
Le Word du préquel, les assets Unity et les comptes rendus existants restent inchangés.

## Décisions de la note

Début acte 3 déjà en vol ; Chacal hors démo ; survol de la surface martienne ; Amara tactique et Kaal’varis commandement ; dialogues courts pendant gameplay ; tutoriel et chargement initial de 45 s ; deux boss avec vague d'élite ; libération de captifs devenant alliés ; épilogue jouable auprès d'Amara.
La déclaration finale intervient plus tôt que dans la chronologie longue du préquel : adaptation de Dyllan pour cette démo, pas modification du roman.
La transition Phobos / surface martienne reste à mettre en scène ; ne pas supposer une continuité géographique littérale.

## Décisions encore ouvertes

- Bouclier : cooldown rouvert à 20 ou 30 s ; remplace la décision précédente de 25 s. Durée 5 s et protection totale précédemment documentées non annulées explicitement ; les préserver sauf décision contraire. Appui pendant cooldown encore à confirmer.
- Champ magnétique : chargement initial 45 s imposé ; correspondance avec Pulse / ancienne impulsion et effets hors tutoriel à confirmer avant code. Ne pas ajouter implicitement un second pouvoir.
- Tutoriel : comportement si le compteur termine avant les consignes, si les cibles sont déjà détruites, ou si la dernière consigne est passée sans activation. Des cibles inoffensives permettent de montrer le bouclier mais pas de valider son absorption.
- Radio : avancement et durée hors tutoriel, moments réellement interrompus, prévention du double traitement de A.
- Nombre de vagues/captifs/attaches, seuil de libération, statistiques, patterns, durées et formulations ouvertes : validation de Dyllan requise.

## Répartition et planning autorisés

Dyllan a explicitement confié la gestion du projet à Codex et le développement à Claude Code. La refonte documentaire du backlog est autorisée ; le refus automatique antérieur est levé par cette autorisation. Aucun échange automatique avec Claude n'est supposé.

Les IDs existants et comptes rendus sont conservés. E3-08 est ajouté pour assembler le tutoriel après les capacités. Les estimations des tâches remaniées sont « À réestimer » : pas de promesse fondée sur les anciennes demi-journées. Les semaines historiques non remaniées ne sont pas des engagements mis à jour.

Ordre de réalisation (dépendances du CSV prioritaires sur les numéros de story) :
1. Confirmer les essais physiques E3-00 ; ne pas réimplémenter le mapping. E3-01 est la prochaine story de développement, spec préparée.
2. Lot A : affichage radio E3-01, lecture Ink E3-02 une fois son rythme défini, menus E3-06. Les dialogues fournis suffisent aux essais de présentation ; P0-09 reste requis pour leur intégration finale.
3. Cadrage : P0-16 inspecte les assets déjà présents sans refaire le bootstrap P0-12/P0-15 ; P0-07/P0-08 précisent boss et séquences ; P0-09/P0-10 préparent les contenus avec Dyllan.
4. Lot B : bouclier E5-05 et champ E5-06 après arbitrages, cibles E4-02, puis tutoriel E3-08 ; environnement/vagues/boss Assemblée E4 ; intégration et playtests E3-07/E4-06.
5. Lot C : Culte, captifs alliés et assaut E5-01 à E5-04, puis intégration E6 et épilogue Amara ; playtest complet E6-07.
6. Lot D : codex/sauvegarde, localisation, polish, régressions et build. Publication uniquement après autorisation explicite.

Anciennes tourelles, phase transmission et glitch LEASH hors démo ; Chacal réservé aux actes 1 et 2. Drones piratés et laser optionnels différés, sans confondre les drones optionnels avec les captifs alliés obligatoires.
Les lignes P0-12, E1/E2 munies de comptes rendus et E3-00 portent « Implémenté selon compte rendu — validation Dyllan à confirmer ». Ce n'est ni une revalidation ni un PASS gameplay.
Un seul auteur actif sur le checkout ; aucune nouvelle scène ou mission imposée.

## Vérification et limites

Modifications documentaires uniquement. Aucun test Unity exécuté, aucun PASS gameplay.
Contrôles exécutés : CSV relu avec le module csv du Python fourni par Codex ; 85 lignes de tâches, 11 colonnes, IDs uniques, toutes les dépendances existantes, aucun cycle et aucune dépendance active vers une tâche hors périmètre. IDs historiques conservés ; seul ajout E3-08. Note identique à la pièce jointe et compte rendu E3-00 intégralement conservé. git diff --check réussi ; git diff --exit-code -- Assets Packages ProjectSettings confirme aucun changement Unity. Résultat documentaire uniquement.
Aucun commit ni push dans cette tâche.