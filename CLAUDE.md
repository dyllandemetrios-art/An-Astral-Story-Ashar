# CLAUDE.md — An Astral Story : Ashar

Shoot'em up narratif 2D à scroll vertical, pixel art, Unity 6 (URP, C#), PC Windows, export Itch.io.
Projet solo : Dyllan conçoit, teste et valide ; Claude Code implémente. Démo de 2 missions (M1 Phobos IX, M2 La course hors de Mars),
développée du 1er octobre au 25 novembre 2026. Finalité : **portfolio**. Le code et le projet Unity seront lus par des recruteurs :
ils doivent être impeccables, au niveau des dépôts de référence de Dyllan (`Stuffy_Infinite_Runner`, `Snake_2D`).

## Lecture ciblée et contexte

- Commencer par la spec demandée et son compte rendu, puis vérifier la branche, le statut Git et le diff pertinent. Pour une tâche sans story, partir du périmètre demandé.
- Lire les sections documentaires référencées et les fichiers concernés ; élargir aux dépendances nécessaires. Ne pas charger tout docs/, le backlog entier ou les autres specs par défaut.
- Ne pas relire un contenu encore présent dans le contexte, sauf modification, doute ou information manquante. Après une nouvelle session ou une compaction, reprendre les règles applicables et le relais utile.
- Cibler les recherches dans Assets/_Project/, Packages/ et ProjectSettings/ selon le besoin. Caches, builds, assets tiers et logs : seulement pour un diagnostic ciblé ; conserver les rapports complets sur disque et lire les passages utiles.
- Avant un changement de session, conserver dans le compte rendu de la spec : branche/commit, changements non commités, décisions, preuves de tests et prochaine action.
- Cette lecture ciblée ne dispense d'aucune règle, vérification de dépendance ou validation requise.

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
11. **N'édite jamais à la main** les fichiers `.unity`, `.prefab` ou `.asset`. Passe par l'éditeur ouvert via le **Unity CLI** (voie active, voir la référence Outillage) ou, à défaut, par un script d'éditeur de mise en place idempotent (menu `Ashar/Setup/<ID>`), que Dyllan lance en un clic.
12. **Assets achetés : jamais sur GitHub.** Tout `Assets/ThirdParty/` est ignoré par Git (packs d'origine **et** leurs retouches : les licences interdisent de redistribuer même une version modifiée). Tu ne modifies jamais un fichier d'origine ; une version retouchée va dans `Assets/ThirdParty/_Modified/<Pack>/`. Seuls les sprites, sons et polices **entièrement faits maison** vont dans `Assets/_Project/`. N'utilise jamais `git add -f` ni `git add .` sans vérifier `git status`.
13. **Commandes** : le mapping v3.1 de docs/specs/E3-00.md (référencé au cahier des charges §7.2) fait foi et remplace les anciens contrôles ; le lire avant toute tâche liée aux entrées, menus ou dialogues. Garder UI/Submit distinct du tir. E3-00 est un préalable obligatoire à E3-01 (voir backlog). **Bouclier bloqué (E5-05)** : durée, cooldown et appui sans charge restent à préciser ; aucun code de bouclier avant décision de Dyllan.

## Stack

Unity 6.6 (6000.6.2f1), URP avec 2D Renderer, Pixel Perfect Camera URP (`UnityEngine.Rendering.Universal`, **ne pas installer** `com.unity.2d.pixel-perfect`), Input System (seul actif), TextMeshPro, Ink (ink-unity-integration, inkle, à installer en S3), Unity Test Framework. Build Windows x86_64, Mono.

Outillage d'éditeur installé : `com.unity.pipeline` (pont du Unity CLI, désactivé dans les builds) et `com.unity.ai.assistant`. Retirés en P0-12 : Visual Scripting, Collab Proxy, IET Framework, AI Inference (Sentis). Aucun autre package sans accord.

## Références à lire selon la tâche

Ces liens sont des lectures ciblées, pas des imports automatiques. Les conventions restent obligatoires pour les fichiers concernés.
- C# (création, modification ou revue) : [structure et conventions](docs/notes/agent-code.md).
- Scènes, prefabs, assets et imports (création, modification ou revue) : [conventions Unity](docs/notes/agent-assets.md).
- Avant une commande Unity : [outillage](docs/notes/agent-outillage.md), puis vérifier la connexion et les commandes disponibles.
- Contrôles : cahier des charges §7.2, source unique du mapping souhaité. Ne pas déduire les touches d'une ancienne spec.

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
