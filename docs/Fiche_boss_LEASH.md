> **Ancienne proposition remplacée pour la démo.** Ses phases et valeurs ne sont plus le contrat du boss actuel. Voir [note actuelle de Dyllan](Demo_Revolte_Ashar.md) et [relais](notes/relais-demo-revolte.md). Aucun Chacal dans cette démo ; dialogues en action privilégiés ; final = croiseur du Culte et libération des alliés.

# Fiche boss — LEASH, croiseur de poursuite (Mission 2)

Livrable **P0-07** — 24/09/2026 — **en attente de validation**.
Références : Game Concept v2.2 (§6, M2), Bestiaire démo v2 (§3.7), Fiche vaisseau v2, Cahier des charges v1.1 (§4, §7.6).
Valeurs de départ (difficulté unique, avant `GameBalanceData`), à l'échelle du cahier des charges v1.4 (1 u = 48 px de référence ; écran de 20 × 11,25 u en 960×540).

---

## 1. Intention

| | |
| --- | --- |
| **Rôle** | Climax de la démo : la dernière chose qui se dresse entre les Exilés et les confins. |
| **Symbole** | LEASH signifie « laisse ». Le boss est littéralement un système de contrôle : il tient des drones en laisse par des chaînes d'énergie. Le joueur brise ces chaînes, puis le noyau, et la démo se clôt sur le message canon « nous reviendrons briser les chaînes des opprimés ». Le combat met en jeu la question centrale (contrôle des IA, des corps, des récits). |
| **Sensation visée** | Phase 1 : comprendre et viser juste. Phase 2 : chaos sous contrôle. Phase 3 : tenir sous la pression. Phase 4 (optionnelle) : protéger la transmission. |
| **Durée cible** | 2 à 2 min 30 de combat réel pour les phases 1 à 3 (TTK pur ≈ 60 s) ; + 20 à 25 s pour la phase 4. |

## 2. Ancrage canon et statuts

| Élément | Statut |
| --- | --- |
| LEASH, IA carcérale de Phobos IX, ancien assistant domotique agricole modifié | **Canon** (préquel, fiche Phobos IX) |
| LEASH prend le contrôle à distance du croiseur de poursuite le plus proche | **Inventé** (ne contredit pas le canon : LEASH surveille déjà l'orbite) |
| Drones retenus par des chaînes d'énergie | **Inventé** (métaphore du nom et du message final) |
| Message crypté des Exilés sur le réseau CSM | **Canon** (préquel, cliffhanger final), affiché après le combat |
| Aucune révélation du roman | Respecté : LEASH ne dit rien du Culte au-delà de ce que dit le Concept |

## 3. Anatomie du boss

```
             ┌──────── croiseur (haut de l'écran, sprite ≈ 240 px, soit ≈ 5 u) ────────┐
   drone ◄─ chaîne ─ [ancrage A]   [ancrage B]  (noyau fermé)  [ancrage C]   [ancrage D] ─ chaîne ─► drone
             └─────────────────────────────────────────────────────────────────────┘
                       les 4 drones enchaînés tournent autour du croiseur
```

| Partie | PV | Vulnérable en | Note |
| --- | --- | --- | --- |
| Ancrages (×4) | 500 chacun (2 000 au total) | Phase 1 | Points lumineux sur la coque. Leur destruction rompt la chaîne correspondante. |
| Coque | 2 000 | Phase 2 | Toute la surface du croiseur, sauf le noyau. |
| Noyau | 2 000 | Phase 3 | Hitbox réduite (rayon ≈ 0,75 u) au centre du croiseur. |
| Brouilleurs (×2) | 500 chacun | Phase 4 | Phase optionnelle (coupe n° 1). |

Total des phases 1 à 3 : **6 000 PV** (conforme au Bestiaire). Une partie qui n'est pas vulnérable renvoie un petit éclat gris à l'impact : le joueur voit tout de suite que son tir est inutile.

## 4. Déroulé

### Entrée (≈ 4 s, invulnérable)
- Le croiseur descend du haut de l'écran pendant que le scroll ralentit. La barre de vie du boss se remplit.
- Une fois le croiseur en place, dialogue `m2_leash_entree` (pause narrative : le combat commence à la fin du dialogue).
- Musique de boss.

### Phase 1 — La laisse (ancrages, 2 000 PV) · couleur : **rouge orangé**
- 4 drones enchaînés tournent autour du croiseur (rayon 3,3 u, 40°/s). **Tant qu'ils sont enchaînés, ils sont invulnérables et bloquent les tirs du joueur** : ils forment un bouclier mobile. Le joueur doit tirer dans les trouées pour toucher les ancrages.
- Chaque drone tire 1 projectile visé toutes les 2,5 s (standard, 5,6 u/s).
- Le croiseur tire un éventail de 5 projectiles vers le bas toutes les 3 s (± 40°).
- Chaque ancrage détruit : la chaîne se rompt, son drone explose, la rotation des drones restants accélère de 10°/s.
- **Aide visuelle** : si aucun ancrage n'est détruit après 12 s, les ancrages clignotent plus fort. Pas de dialogue : aucun dialogue pendant l'action.
- Impulsion de piratage : sans effet (les drones enchaînés restent sous le contrôle de LEASH).

### Transition 1 → 2 (1,5 s, puis dialogue)
- Boss invulnérable et figé ; tous les projectiles ennemis présents sont effacés (lisibilité).
- Glitch du HUD (coupe n° 2), puis dialogue `m2_leash_phase2` (pause narrative) ; la phase 2 démarre à la fin du dialogue.
- La coque s'ouvre sur ses baies de lancement.

### Phase 2 — Meute lâchée (coque, 2 000 PV) · couleur : **magenta**
- Le croiseur largue 2 drones chercheurs toutes les 5 s (4 à l'écran au maximum). Un drone chercheur : 50 PV, 4,5 u/s, rotation 90°/s ; dégâts par contact ; il se désagrège au bout de 8 s. Contour magenta.
- La coque tire une rafale de 3 projectiles visés magenta toutes les 2,5 s.
- **Impulsion de piratage : détruit les drones chercheurs** (libérés de leur chaîne, ils ne sont plus protégés par LEASH), sans effet sur la coque ni sur ses projectiles. C'est le moment où l'arme de Kaal'varis prend tout son sens.
- Le croiseur se déplace lentement de gauche à droite (1,5 u/s), pour ouvrir des angles de tir.

### Transition 2 → 3 (1,5 s)
- Même règle : invulnérabilité et effacement des projectiles ; les drones chercheurs restants explosent.
- Le noyau s'expose : flash de 1 s, puis dialogue `m2_leash_phase3` (pause narrative) ; la phase 3 démarre à la fin du dialogue.

### Phase 3 — Le noyau (2 000 PV) · couleur : **jaune**
Cycle de 10,5 s, répété :
1. **Double spirale** (6 s) : 2 bras qui tournent en sens inverse, 1 projectile par bras toutes les 0,15 s, 4,5 u/s.
2. **Salves visées** (3 s) : 3 salves de 5 projectiles en éventail serré (± 15°), 6,75 u/s.
3. **Respiration** (1,5 s) : aucun tir, pour laisser le joueur se replacer.

Le croiseur reste immobile : c'est le joueur qui se déplace. Sous 25 % des PV du noyau, le croiseur se couvre d'étincelles (signal visuel, pas de dialogue).

### Phase 4 optionnelle — La transmission (coupe n° 1) · couleur : **magenta**
- Le noyau s'éteint, dialogue `m2_leash_transmission` (pause narrative). LEASH se réfugie dans 2 brouilleurs qui se détachent de l'épave et visent la transmission des Exilés.
- Une jauge « Transmission » apparaît en haut de l'écran et se remplit en 25 s.
- Chaque brouilleur tire une onde de 8 projectiles magenta en cercle toutes les 3 s.
- Détruire un brouilleur retire 8 s au remplissage de la jauge et rapporte 1 000 points.
- **Aucune condition d'échec supplémentaire** : la transmission aboutit toujours. Le joueur choisit seulement d'accélérer la fin et de gagner des points. On ne peut perdre que par la mort, comme partout ailleurs.

### Défaite de LEASH
- Explosion en chaîne du croiseur (3 s), 10 000 points ; dernier dialogue `m2_leash_defaite`.
- Fin de gameplay, puis dialogue de fin (texte seul), puis message CSM en plein écran (canon), puis codex M2.

## 5. Assets (pack DyLESTorm, contenu vérifié le 24/09 sur les pages itch.io)

Le pack ne contient **ni ancrages, ni chaînes, ni noyau** : ce sont des éléments à fabriquer. Les fichiers Aseprite (.ase) fournis avec le pack d'ennemis permettent de retoucher les sprites (recoloration, noyau ouvert).

| Élément | Source proposée | Travail |
| --- | --- | --- |
| Croiseur | Catégorie Boss de « Pixel Enemies for SHMUP » (≈ 240 × 240 px) | Recolorer en magenta et jaune ; variante « noyau ouvert » dans Aseprite |
| Ancrages (×4) | À dessiner (≈ 12 × 12 px, lumineux) | Faible |
| Chaînes | `LineRenderer` + texture de 8 px en tuile | Faible, pas de sprite de pack nécessaire |
| Drones enchaînés, drones chercheurs | Catégorie Bug (animée) ou Mini | Recoloration magenta pour les chercheurs |
| Noyau | Surimpression d'un sprite dessiné (≈ 24 × 24 px) sur le croiseur | Faible |
| Brouilleurs (phase 4) | Catégorie Mini ou Cannon | Aucun, ou recoloration |
| Projectiles rouge orangé, magenta, jaune | Projectiles du pack de vaisseaux joueur (peu variés, surtout des recolorations) | Recolorer ou dessiner 3 formes de 6 à 10 px |

**Échelle** : en 960×540 (résolution recommandée, cahier des charges §2 point 8), le croiseur d'environ 240 px occupe 44 % de la hauteur de l'écran, ce qui laisse la place d'esquiver. Si 640×360 était retenu, il faudrait le placer en partie hors écran.

## 6. Lisibilité

- Règle : projectiles du joueur argent-blanc, plus **2 couleurs ennemies au maximum par phase**, soit 3 couleurs à l'écran.
  - Phase 1 : rouge orangé.
  - Phase 2 : magenta.
  - Phase 3 : jaune.
  - Phase 4 : magenta.
- Les chaînes (effet et non projectile) sont blanc bleuté pâle et translucides, jamais d'une couleur de projectile.
- Toutes les parties vulnérables clignotent en blanc à l'impact ; les parties invulnérables renvoient un éclat gris.
- Les projectiles sont effacés à chaque transition.

## 7. Difficulté

Difficulté unique : les multiplicateurs globaux de `GameBalanceData` (cahier des charges §7.9) s'appliquent à tous les PV, aux intervalles de tir et aux vitesses de projectile. **Réglage au playtest** : si le multi-shot de niveau 2 ou 3 raccourcit trop le combat (Fiche vaisseau §4), majorer les PV des parties jusqu'à × 1,5 avant de toucher aux patterns.

## 8. Dialogues déclenchés

Textes à écrire en P0-09 ; knots Ink ci-dessous.

| Knot | Déclencheur | Interlocuteur | Voix off (coupe n° 0) |
| --- | --- | --- | --- |
| `m2_leash_entree` | Croiseur en place, avant la phase 1 | LEASH | Oui |
| `m2_leash_phase2` | Transition 1 → 2 | LEASH | Oui |
| `m2_leash_phase3` | Transition 2 → 3 | LEASH | Oui |
| `m2_leash_transmission` | Début de la phase 4 | LEASH | Oui |
| `m2_leash_defaite` | Défaite | LEASH | Oui |

Chaque dialogue est une pause narrative (cahier des charges §7.7) placée à un moment où le boss est invulnérable et les projectiles effacés. **Aucun dialogue pendant une phase de combat.**

## 9. Implémentation (stories E5-01 à E5-04, E5-07, E6-04)

Prototyper d'abord : **un script `LeashBoss` avec une énumération de phases et un `switch`**, et des coroutines pour les patterns. Il n'y a pas de machine à états générique tant que la navette n'en a pas besoin.

| Élément | Implémentation proposée |
| --- | --- |
| Données | `LeashBossData` (ScriptableObject) : PV par partie, vitesses, intervalles, durées de transition, rayon et vitesse de rotation. |
| Parties | Composant `Damageable` sur chaque ancrage, la coque, le noyau et les brouilleurs ; activé ou désactivé selon la phase ; chaque partie remonte ses dégâts à `LeashBoss`. |
| Chaînes | `LineRenderer` entre l'ancrage et le drone ; désactivé à la rupture. |
| Drones enchaînés | Enfants d'un pivot en rotation ; collider sur la couche Enemy, mais sans `Damageable` (ils absorbent les tirs). |
| Drones chercheurs | Prefab dédié, piloté par `EnemyData` (pulseResponse = Destroy). |
| Patterns | Réutiliser `EnemyShooter` et `FirePatternData` (Aimed, Fan, Spiral) ; la double spirale utilise deux émetteurs. |
| Transitions | Méthode unique `ClearEnemyBullets()` ; invulnérabilité par booléen. |
| Barre de vie | Une seule barre pour les phases 1 à 3 (6 000 PV), avec des crans à 2/3 et 1/3. |

**Coût : élevé** (≈ 5 ½ j pour E5-01 à E5-04, + 1 ½ j pour la phase 4). **Risque de performance** : la phase 3 est la plus dense. À mesurer après E2-01, et plafonner le nombre de projectiles de la spirale en priorité si les 60 fps ne tiennent pas.

## 10. Points à valider

1. Les drones enchaînés servent de bouclier (phase 1) : c'est ce qui rend le combat lisible et différent d'un simple sac à PV.
2. L'impulsion de piratage détruit les drones chercheurs libérés (phase 2). C'est une exception assumée à la règle « sans effet sur LEASH », justifiée par la fiction : un drone hors de la laisse redevient piratable.
3. La phase 4 n'a pas de condition d'échec propre.
4. La barre de vie unique avec des crans, plutôt qu'une barre par phase.
5. Le sprite du croiseur (catégorie Boss du pack d'ennemis) et sa taille à l'écran, à confirmer au test d'import (P0-15).
