> **Note (25/09/2026)** : document de contexte. Ses valeurs d'échelle, de vitesse et sa correspondance d'assets sont obsolètes ; seules font foi celles du cahier des charges v1.2. Alignement complet prévu en P0-16.

**Bestiaire démo v2**

An Astral Story : Ashar — Missions 1 et 2

Livrable P0-05 (version 2, adaptée au préquel) — 20/09/2026 — **en attente de validation**. Référence : Game Concept v2.0, document préquel, Stylized 2D Space Shooter Pack.

# 1. Conventions

- **Mécaniques inchangées depuis la v1 :** TTK, patterns, scores et modificateurs par mode restent identiques ; seuls les noms, les rôles narratifs et les assets changent.

- **TTK cible :** durée de destruction en secondes de tir soutenu du tir principal de base (100 dégâts/s), en mode Normal.

- **Statuts :** Canon (Scénario, bibles ou document préquel), Adapté, Inventé.

- **Assets :** le choix précis des sprites se fait sur les aperçus du pack (je ne les ai pas vus). Le pack contient 10 vaisseaux en 5 coloris, 22 petits ennemis en 2 à 4 coloris, 11 mini-boss, 4 boss géants, 7 missiles ou bombes, des météores, des rochers et des systèmes de particules d’explosion.

- **Lisibilité :** 3 couleurs de projectiles maximum par phase de boss ; un seul projectile standard pour les ennemis génériques.

# 2. Vue d’ensemble

| **Unité** | **Missions** | **TTK (s)** | **Score** | **Statut** | **Asset visé** |
| --- | --- | --- | --- | --- | --- |
| Drone de garde de LEASH | M1, M2 | 0,5 | 100 | Inventé | 1 des 22 petits ennemis |
| Chasseur de patrouille A.E.T. | M1, M2 | 2 | 200 | Inventé | Un vaisseau du pack, coloris rouge |
| Intercepteur (variante du chasseur) | M2 | 1 | 250 | Adapté | Même sprite que le chasseur |
| Satellite sentinelle | M1, M2 | 3 | 300 | Canon (préquel), Adapté | 1 des petits ennemis ou une bombe |
| Tourelle à plasma de Phobos IX | M1 | 12 | 1 000 | Canon (préquel) | À choisir parmi les mini-boss ou assembler |
| Navette de garde (mini-boss) | M1 | 45 | 5 000 | Inventé | 1 des 11 mini-boss |
| LEASH, croiseur de poursuite (boss) | M2 | ≈ 60 | 10 000 | Canon (LEASH), Inventé (boss) | 1 des 4 boss géants + drones et chaînes |

# 3. Fiches

## 3.1 Drone de garde de LEASH

| **Ancrage canon** | Préquel : LEASH est un ancien assistant domotique agricole modifié. Ses drones sont donc des machines agricoles reconverties en gardes. **[Inventé]** |
| --- | --- |
| **Rôle et apparition** | Unité de base en nombre. M1 zone 1 (tunnels), puis en soutien en M2. |
| **Déplacement** | Formations en V ou en ligne entrant par le haut. Deux comportements : descente sinueuse (harceleur) et piqué vers le joueur. |
| **Tir** | Harceleur : 1 projectile visé toutes les 2 s. Piqué : aucun tir. |
| **Projectiles** | Un seul projectile standard, petit, rouge orangé. |
| **TTK · score** | 0,5 s · 100. Détruit par l’impulsion de piratage. |
| **Coût** | Faible. Mesure de perf avec 20 drones ou plus (E2-01). |

## 3.2 Chasseur de patrouille A.E.T.

| **Ancrage canon** | Le préquel place la prison sous le contrôle de l’A.E.T. et du Consortium ; aucune fiche de vaisseau n’existe pour 2176. **[Inventé]** |
| --- | --- |
| **Rôle et apparition** | Escadrons de 3 à 5. M1 zone 2, M2 phase de poursuite. |
| **Déplacement** | Entrée latérale en formation, balayage de l’écran, sortie par le bord opposé. |
| **Tir** | Éventail de 3 projectiles toutes les 2,5 s. |
| **Projectiles** | Standard rouge orangé, plus gros que ceux des drones. |
| **TTK · score** | 2 s · 200. Étourdi 2 s par l’impulsion de piratage. |
| **Coût** | Faible : coloris rouge d’un vaisseau du pack, différent du bleu du joueur. |

## 3.3 Intercepteur (variante du chasseur)

| **Statut** | **Adapté** : simple variante de comportement du chasseur, sans sprite supplémentaire. |
| --- | --- |
| **Rôle et apparition** | M2, en appui des chasseurs. |
| **Déplacement** | Piqué rapide vers la position du joueur, puis remontée. |
| **Tir** | Rafale de 2 projectiles visés au bas du piqué. |
| **TTK · score** | 1 s · 250. |
| **Coût** | Faible. |

## 3.4 Satellite sentinelle

| **Ancrage canon** | Préquel : Phobos IX est protégé par des satellites sentinelles. **[Canon]** ; comportement Adapté. |
| --- | --- |
| **Rôle et apparition** | M1 zone 2 (dômes) et M2 (champ de débris). |
| **Comportement** | Fixe ou en lente orbite ; tir visé lent toutes les 3 s. Désactivé 3 s par l’impulsion de piratage. |
| **Projectiles** | Standard rouge orangé. |
| **TTK · score** | 3 s · 300. |
| **Coût** | Faible. |

## 3.5 Tourelle à plasma de Phobos IX

| **Ancrage canon** | Préquel : 3 tourelles automatiques à plasma protègent la prison. **[Canon]** |
| --- | --- |
| **Rôle et apparition** | M1 zone 3 : trois verrous à détruire pour ouvrir la sortie, un par un (structure de niveau). |
| **Comportement** | Fixe au bord de l’écran ; spirale lente de plasma, télégraphiée par une ligne 1 s avant le tir. |
| **Projectiles** | Plasma rouge orangé, large et lent. |
| **TTK · score** | 12 s chacune · 1 000. 300 dégâts par l’impulsion de piratage. |
| **Coût** | Moyen : pattern en spirale et verrou de progression. |

## 3.6 Navette de garde pénitentiaire (mini-boss M1)

| **Statut** | **Inventé.** Rôle repris du Guardian Dropship de la v1 (canon d’un autre arc). |
| --- | --- |
| **Rôle et apparition** | Fin de M1. LEASH menace par haut-parleurs pendant le combat. |
| **Phase 1** | Balayage horizontal ; canons en proue, éventails larges et tirs visés lents. |
| **Phase 2 (à 50 % des PV)** | Largage de 2 vagues de drones de garde ; canons plus lents. |
| **Projectiles** | Canons rouge orangé ; drones : projectile standard. |
| **TTK · score** | 45 s (combat réel de 75 à 90 s) · 5 000. Une amélioration garantie à la destruction. |
| **Coût** | Moyen : deux phases scriptées en S4, sans machine à états générique. |

## 3.7 LEASH, croiseur de poursuite (boss M2)

| **Ancrage canon** | LEASH est l’IA carcérale de Phobos IX (préquel). Qu’elle prenne le contrôle d’un croiseur pour poursuivre les fugitifs est une **invention** ; le message final sur les chaînes est canon. |
| --- | --- |
| **Mécanique** | Des drones retenus au croiseur par des chaînes d’énergie : couper les chaînes fait passer les phases. |
| **Phase 1** | Drones enchaînés en rotation autour du croiseur (rouge orangé). |
| **Phase 2** | Chaînes coupées : drones libres à tête chercheuse (magenta). |
| **Phase 3** | Noyau exposé : salves denses mais lisibles (jaune). |
| **Phase 4 (optionnelle)** | LEASH tente de couper la transmission des exilés. Coupe 1. |
| **TTK · score** | ≈ 60 s (3 phases de 20 s) · 10 000. Détail dans la fiche P0-07. |
| **Coût** | Élevé (machine à états à phases, E5-01 à E5-04). |

## 3.8 LEASH, voix ennemie (non-combattant)

| **Rôle** | Voix synthétique diffusée par la radio : menaces, ordres, comptes à rebours. **[Inventé]** |
| --- | --- |
| **Effet HUD** | Glitch bref de 0,5 à 1 s sur la fenêtre radio et les bords du HUD, jamais sur les commandes ni sur les projectiles. Coupe 2. |
| **Coût** | Faible. |

# 4. Correspondance ennemis ↔ pack d’assets

| **Unité** | **Catégorie du pack** | **Plan B si rien ne convient** |
| --- | --- | --- |
| Drone de garde | 22 petits ennemis | Kenney Space Shooter Redux (CC0) |
| Chasseur A.E.T. | 10 vaisseaux, coloris rouge | Coloris d’un autre vaisseau |
| Satellite sentinelle | Petits ennemis ou bombes | Météore ou rocher avec canon assemblé |
| Tourelle à plasma | 11 mini-boss | Assemblage rocher + canon |
| Navette de garde | 11 mini-boss | Vaisseau agrandi |
| Boss LEASH | 4 boss géants | Mini-boss agrandi |
| Explosions et impacts | Systèmes de particules du pack | Cartoon FX Remaster FREE (gratuit, URP) |

# 5. Modificateurs par mode (valeurs de départ)

La structure de la boucle reste identique dans les trois modes ; seuls ces paramètres changent.

| **Paramètre** | **Story** | **Normal** | **Arcade** |
| --- | --- | --- | --- |
| PV des ennemis | × 0,7 | × 1 | × 1,2 |
| Densité de tirs | × 0,6 | × 1 | × 1,3 |
| Vitesse des projectiles | × 0,85 | × 1 | × 1,1 |
| Vies de départ | 5 | 3 | 3 |
| Continues | Illimités | 3 | Aucun |
| Invulnérabilité au respawn | 3 s | 2 s | 1,5 s |

# 6. Points à valider

- **Palette :** projectiles du joueur argent-blanc, ennemis rouge orangé, boss LEASH magenta et jaune.

- **Tourelles destructibles** (trois verrous en M1) plutôt qu’invulnérables.

- **LEASH boss** sous forme de croiseur à chaînes.

- **Test d’import du pack en Unity 6 URP** (P0-15), avant tout autre travail sur les visuels.

Bestiaire démo v2 — An Astral Story : Ashar — page