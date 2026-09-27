> **Document historique amendé par la nouvelle note.** Les éléments remplacés ne font plus foi ; conserver les informations non concernées. Voir [note actuelle de Dyllan](Demo_Revolte_Ashar.md) et [relais](notes/relais-demo-revolte.md). Aucun Chacal dans cette démo ; dialogues en action privilégiés ; final = croiseur du Culte et libération des alliés.

> **Note (25/09/2026)** : document de contexte. Ses valeurs d'échelle, de vitesse et sa correspondance d'assets sont obsolètes ; seules font foi celles du cahier des charges v1.2. Alignement complet prévu en P0-16.

**Fiche vaisseau du joueur v2**

Vaisseau terrien volé de Kaal’varis — Missions 1 et 2

Livrable P0-06 (version 2) — 20/09/2026 — **en attente de validation**. Référence : Game Concept v2.0, Bestiaire démo v2.

# 1. Ancrage canon et statuts

| **Élément** | **Statut et justification** |
| --- | --- |
| Vaisseau terrien volé | **Canon (préquel).** Kaal’varis et ses compagnons s’emparent d’un vaisseau terrien pour fuir Mars. Le modèle exact est **Inventé**. |
| Technologie pléiadienne | **Canon (préquel).** Les exilés l’ont récupérée auprès d’une faction dissidente ; elle justifie les modifications du vaisseau. |
| Pilotage par Kaal’varis | **Canon (Bible).** Expert en combat spatial et tactique de guérilla. |
| Impulsion de piratage | **Adapté.** Kaal’varis peut pirater les systèmes terriens (Bible) : l’arme secondaire de la v1 (orbes IEM) devient une impulsion, avec les mêmes chiffres. |
| Tir principal en rafale | **Adapté.** |
| Dash par propulseurs d’appoint | **Inventé.** Mécanique de gameplay ; à valider. |
| Bouclier éphémère | **Inventé.** Bouclier d’énergie temporaire d’origine pléiadienne. |

# 2. Échelle de référence

- **Vue :** caméra fixe de 10 unités de haut (1 unité = 108 px à 1080p), soit 17,8 unités de large.

- **Zone jouable :** l’écran moins une marge de 0,6 unité.

- **Ennemis :** lente = 2 u/s, moyenne = 4 u/s, rapide = 6 u/s, piqué = 12 u/s ; projectile standard = 5 u/s, projectile visé rapide = 7 u/s.

# 3. Caractéristiques de départ (mode Normal)

| **Paramètre** | **Valeur** | **Note** |
| --- | --- | --- |
| Vitesse de déplacement | 8 u/s | Traverse la largeur en environ 2,2 s ; 8 directions, diagonales normalisées. |
| Hitbox | rayon 0,10 u (≈ 11 px) | Petit point argent visible (option désactivable). |
| Taille du sprite | 1,0 × 1,2 u | ≈ 108 × 130 px à l’écran. |
| Vies, continues, respawn | Voir Bestiaire §5 | 5 / 3 / 3 vies ; invulnérabilité au respawn 3 / 2 / 1,5 s. |
| Tir principal | 10 tirs/s, 10 dégâts | 1 flux frontal = **100 dégâts/s** (référence pour tous les PV). |
| Vitesse du projectile joueur | 20 u/s | Argent-blanc. |
| Dash | 2,5 u en 0,18 s ; invulnérable 0,28 s ; cooldown 1,5 s | Direction de l’entrée, vers l’avant sans entrée. Cooldown Story 1,0 s, Arcade 2,0 s. |
| Impulsion de piratage (arme secondaire) | Cooldown 12 s ; rayon 3,0 u | Efface les projectiles des drones et des chasseurs (pas ceux des boss). Détruit les drones, étourdit les chasseurs 2 s, désactive les satellites 3 s. 300 dégâts à une tourelle ou à la navette. Sans effet sur LEASH. |
| Bouclier éphémère (power-up) | Absorbe 1 impact, 10 s maximum | Halo argent. |

# 4. PV des ennemis dérivés du DPS de référence

Avec 100 dégâts/s, les durées de destruction du Bestiaire donnent les PV de départ à saisir dans les EnemyData.

| **Unité** | **TTK (s)** | **PV de départ** | **Remarque** |
| --- | --- | --- | --- |
| Drone de garde | 0,5 | 50 | 1 à 2 tirs |
| Intercepteur | 1 | 100 |  |
| Chasseur A.E.T. | 2 | 200 |  |
| Satellite sentinelle | 3 | 300 |  |
| Tourelle à plasma | 12 | 1 200 | 3 verrous en M1 |
| Navette de garde (mini-boss) | 45 | 4 500 | 2 phases, bascule à 50 % |
| LEASH (boss) | ≈ 60 | 6 000 | 3 phases de 2 000 ; phase 4 en plus (Coupe 1) |

**Risque d’équilibrage :** le multi-shot (jusqu’à 2,6 fois le DPS de base) raccourcirait les combats de boss d’autant. Au playtest, prévoir des PV de boss majorés (jusqu’à × 1,5) ou une transition invulnérable.

# 5. Power-ups de la démo

| **Power-up** | **Priorité** | **Effet de départ** |
| --- | --- | --- |
| Multi-shot | Must | Niveau 1 : 1 flux (100/s) ; niveau 2 : 2 flux parallèles (200/s) ; niveau 3 : 3 flux en éventail étroit (≈ 260/s). Perdu au premier impact. |
| Bouclier éphémère | Must | Voir §3. |
| Drones de garde piratés (alliés) | Coupe 3 | Non spécifié tant qu’il n’est pas confirmé. |
| Laser pléiadien | Coupe 4 | Non spécifié ; justifié par la technologie pléiadienne récupérée. |

# 6. Palette de projectiles

| **Source** | Couleur |
| --- | --- |
| **Joueur (tirs, halo du bouclier)** | Argent-blanc ; vaisseau en coloris bleu du pack |
| **Ennemis génériques, navette, tourelles** | Rouge orangé (un seul projectile standard) |
| **Boss LEASH** | Phase 1 rouge orangé, phase 2 magenta, phase 3 jaune ; 3 couleurs maximum par phase |
| **Impulsion de piratage (effet, pas projectile)** | Argent avec arcs bleutés, jamais utilisés pour un tir ennemi |

# 7. Spécification visuelle (entrée du plan d’assets P0-14)

- **Vaisseau :** l’un des 10 vaisseaux du pack, coloris bleu ; les sprites existent en 3 résolutions (petit, moyen, grand).

- **À produire :** flash de dégâts ; halo de bouclier ; onde de l’impulsion de piratage ; point de hitbox ; traînée de dash.

- **Explosions et impacts :** systèmes de particules du pack, à vérifier en URP.

# 8. Points à valider

- Les valeurs de la section 3, notamment le DPS de référence de 100/s et l’impulsion de piratage.

- Le dash et le bouclier restent des inventions assumées.

- La palette de projectiles du §6.

Fiche vaisseau du joueur v2 — An Astral Story : Ashar — page