> **Précision issue des tests E3-02 (Dyllan, consignée dans idees.md) :** objectif de survie, sans score présenté au joueur. Partie continue du tutoriel à l'épilogue, sans écran de fin intermédiaire. Dialogues entre combats, pause volontaire et Game Over conservés. Application : E3-09 et intégration P0-08/E4-05/E6-06.

> **Clarification actuelle de Dyllan :** Dialogues ENTRE les combats, sans Ink. Entrée (UI/Submit, A manette) avance une réplique ; dernière validation puis combat. Vol, décor, musique et animations continus, aucune attaque ni nouvelle vague. Aucun timer de lecture ni validation par le tir. Tutoriel distinct : action demandée ou validation. Contrat : [E3-02](specs/E3-02.md).

> **Bouclier actualisé :** Bouclier : 5 s de protection, puis 20 s de cooldown à compter de la fin de la protection. Pendant ces 20 s, aucune protection du bouclier. Les appuis pendant protection ou cooldown sont ignorés, sans prolongation ni mise en attente. Après recharge, nouvelle pression nécessaire ; maintien sans réactivation automatique. Cooldown de 20 s demandé par Dyllan ; durée de 5 s conservée et réactivation précisée pour éviter la protection permanente.

# Mise à jour — Démo « Les Exilés d’Ashar »

## Objectif de la démo

La démo commence au début de **l'Acte 3 : La Révolte d'Ashar**.

Kaal’varis vient de s'échapper de **Phobos IX** avec les autres détenus. L'évasion de la prison elle-même n'est pas jouée : lorsque le joueur prend le contrôle, Kaal’varis est déjà aux commandes d'un vaisseau volé à l'Assemblée Terrienne.

La démo doit raconter un arc complet :

**Évasion de Phobos IX → fuite au-dessus de Mars → affrontement avec l'Assemblée → intervention du Culte → libération de l'escadrille d'Amara → rencontre avec Amara → naissance publique des Exilés d'Ashar.**

Le **Chacal n'intervient pas dans cette démo**. Il reste pertinent pour un futur jeu ou contenu consacré aux Actes 1 et 2.

---

# 1. INTRODUCTION — TUTORIEL

## Situation

Kaal’varis est déjà en vol à proximité des installations de Phobos IX.

Son vaisseau possède son armement standard ainsi qu'un **dispositif/générateur pléiadien** qu'il vient de connecter au vaisseau.

Ce dispositif permet d'utiliser la capacité de **champ magnétique**.

## Début

Au lancement :

**KAAL’VARIS**

« Générateur pléiadien connecté. Encore 45 secondes… et on va voir ce que ce tas de boue a dans le ventre. »

Une UI apparaît immédiatement pour montrer le chargement du dispositif :

**45 secondes → 0**

Ce compteur sert également à enseigner au joueur que la capacité magnétique possède un cooldown important.

## Tutoriel pendant le chargement

Pendant ces 45 secondes, introduire progressivement :

1. Déplacement
2. Tir
3. Dash
4. Bouclier

Chaque instruction disparaît lorsque le joueur effectue l'action demandée.

Le joueur doit également pouvoir passer une instruction avec la touche de validation prévue à cet effet.

Le gameplay ne doit pas être interrompu par les dialogues du tutoriel.

### Bouclier

Le bouclier fait partie du vaisseau indépendamment du générateur pléiadien.

Son cooldown doit être visible dans l'UI.

**Valeurs retenues : 5 s de protection, puis 20 s de cooldown sans protection.**

## Cibles du tutoriel

Quelques **Pacificateurs** sont présents au sol.

Pendant cette phase uniquement, ils ne tirent pas sur le joueur.

Ils servent à tester :

- déplacement ;
- tir ;
- dash ;
- éventuellement bouclier ;
- capacité magnétique finale.

## Fin du chargement

Lorsque les 45 secondes sont écoulées :

**KAAL’VARIS**

« Générateur chargé. Voyons ce que ça donne. »

Afficher la consigne permettant d'activer le champ magnétique.

Le joueur déclenche la capacité.

Les Pacificateurs présents dans la zone sont neutralisés/détruits.

**Fin du tutoriel.**

À partir de cet instant, les ennemis utilisent leur comportement de combat normal.

---

# 2. PHOBOS IX — DÉBUT DE LA RÉVOLTE

Le combat commence réellement.

Kaal’varis contacte les autres évadés.

**KAAL’VARIS**

« À tous les Exilés : prenez un vaisseau et dégagez de cette station. Les Pacificateurs qui vous barrent la route, abattez-les. »

Le joueur combat les Pacificateurs tandis que les autres prisonniers s'emparent progressivement de vaisseaux.

Il n'est pas nécessaire de simuler une flotte entière.

Quelques vaisseaux visibles en arrière-plan ou quittant la station suffisent à donner l'impression d'une évasion collective.

---

# 3. SORTIE DE PHOBOS IX — SURVOL DE MARS

La fuite se poursuit sans rupture narrative importante.

Le joueur quitte les installations carcérales et arrive au-dessus de **la surface de Mars**.

Le décor utilise les assets martiens disponibles :

- sol rouge ;
- installations ;
- structures/dômes ;
- environnement martien.

Les ennemis terrestres laissent progressivement place aux forces aériennes/spatiales de l'Assemblée Terrienne.

Leur identité visuelle principale est **blanche et rouge**.

## Intervention d'Amara

Amara informe les fugitifs de l'interception.

**AMARA**

« Kaal’varis, l'Assemblée vous a repérés. Des Horizon Scouts et des Guardian Dropships convergent vers vous. »

Puis :

**AMARA**

« Ils vont tenter de couper votre trajectoire. Ne les laissez pas refermer le passage. »

Les dialogues se déroulent entre les combats ; le vol continue, les attaques attendent la dernière validation.

Amara apporte principalement les **informations tactiques**, tandis que Kaal’varis commande les Exilés.

## Combat

Enchaîner plusieurs vagues de :

- Horizon Scouts ;
- Guardian Dropships ;
- autres unités de l'Assemblée déjà disponibles si nécessaire.

La séquence se termine avec le :

**BOSS 1 — Vaisseau de l'Assemblée Terrienne**

Après sa destruction, courte respiration avant l'arrivée de la menace suivante.

---

# 4. INTERVENTION DU CULTE

L'identité visuelle des ennemis change.

Les vaisseaux du **Culte de l'Ascension Cybernétique** utilisent une esthétique plus sombre, avec une dominante rouge.

**KAAL’VARIS**

« Le Culte… Ils viennent finir le travail. Restez groupés. On s'est évadés ensemble, on sortira d'ici ensemble. »

Les forces du Culte sont plus dangereuses que celles rencontrées précédemment.

Enchaîner plusieurs vagues.

Il n'est pas nécessaire de développer un deuxième petit boss spécifique.

Une **vague d'élite** ou une variante plus dangereuse des ennemis existants suffit avant le boss final.

---

# 5. BOSS FINAL — CROISEUR DU CULTE

Le croiseur du Culte apparaît.

Plusieurs vaisseaux pléiadiens sont physiquement **enchaînés au croiseur**.

Le boss ne doit pas immédiatement déclencher son affrontement principal afin que le joueur ait le temps de comprendre visuellement la situation.

## Identification des captifs

**AMARA**

« Attendez ! Ces vaisseaux… c'est mon escadrille. Ils les ont capturés. »

Puis :

**AMARA**

« Kaal’varis, visez leurs chaînes. Libérez-les ! »

Nouvel objectif :

**DÉTRUIRE LES ENTRAVES**

Le joueur doit viser les chaînes/dispositifs maintenant les vaisseaux captifs.

Les vaisseaux eux-mêmes ne doivent pas être les cibles.

## Libération

Chaque vaisseau libéré :

- se détache visuellement du croiseur ;
- devient allié ;
- rejoint le combat ;
- attaque les forces du Culte.

La libération doit produire une récompense gameplay immédiatement visible.

Une fois l'escadrille suffisamment libérée :

**KAAL’VARIS**

« Escadrille d'Amara, si vous me recevez : attaquons ce monstre de front ! »

Les alliés participent alors à l'assaut contre le croiseur.

Cette séquence constitue le **climax gameplay de la démo**.

Le joueur et les vaisseaux libérés détruisent ensemble le boss final.

---

# 6. ÉPILOGUE JOUABLE

Après la destruction du croiseur, le rythme retombe.

Plusieurs vaisseaux suivent Kaal’varis.

Le joueur traverse une courte zone d'astéroïdes avant de rejoindre le vaisseau d'Amara.

Le croiseur d'Amara est un grand vaisseau **argent et bleu**, visuellement comparable en taille à un boss mais non hostile.

## Rencontre avec Amara

**AMARA**

« Kaal’varis. Je suis heureuse de vous voir. Nous vous sommes reconnaissants pour votre aide. »

Puis :

**AMARA**

« Rejoignez-nous sur Taygeta Prime. Vous y serez en sécurité. »

Kaal’varis refuse de quitter le système solaire.

**KAAL’VARIS**

« Pas encore. Nous trouverons un refuge plus près de la Terre. Il reste des gens à libérer. »

Puis déclaration finale.

Le texte doit explicitement identifier le responsable des augmentations forcées : **les scientifiques du Culte**, et non un vague « ils ».

Proposition actuelle à retravailler si nécessaire :

**KAAL’VARIS**

« Terriens, Pléiadiens… Les scientifiques du Culte nous ont transformés de force, puis réduits en esclavage. »

La conclusion doit mener à l'identité collective des Exilés et rester cohérente avec le texte du préquel :

**« Nous sommes les Exilés d'Ashar. Et nous reviendrons briser les chaînes des opprimés de la Terre. »**

Cette déclaration clôt la démo.

---

# PRINCIPES DE DIALOGUE

Le jeu reste un **shoot'em up 2D**, pas un jeu narratif à dialogues longs.

Respecter les règles suivantes :

- interventions très courtes ;
- environ deux lignes maximum par prise de parole ;
- éviter les successions de nombreuses validations avec Entrée ;
- placer les dialogues dans les moments calmes entre les combats ;
- suspendre le combat pendant la lecture, sans figer le vol ni les animations ;
- Amara apporte principalement renseignements et informations tactiques ;
- Kaal’varis commande les Exilés et porte leur discours politique/narratif ;
- éviter de présenter Kaal’varis comme le « sauveur » d'Amara ou de son escadrille : ils sont des alliés engagés dans le même conflit.

---

# STRUCTURE GLOBALE

**Tutoriel**
→ Générateur pléiadien / 45 s  
→ Déplacement / Tir / Dash / Bouclier  
→ Test du champ magnétique

**Phase 1 — Phobos IX**
→ Pacificateurs  
→ Évasion collective

**Phase 2 — Mars / Assemblée Terrienne**
→ Horizon Scouts / Guardian Dropships  
→ Boss Assemblée

**Phase 3 — Culte**
→ Vagues plus dangereuses  
→ Vague d'élite

**Phase 4 — Boss final**
→ Croiseur du Culte  
→ Vaisseaux d'Amara enchaînés  
→ Destruction des chaînes  
→ Alliés libérés  
→ Assaut collectif

**Épilogue**
→ Astéroïdes  
→ Rencontre avec le croiseur d'Amara  
→ Proposition de Taygeta Prime  
→ Kaal’varis décide de rester près de la Terre  
→ « Nous sommes les Exilés d'Ashar. »

---

# POINTS NON ENCORE FIGÉS

Ne pas prendre de décision définitive sans validation concernant :

- cooldown du bouclier : **résolu — 20 s après les 5 s de protection** ;
- nombre exact de Pacificateurs du tutoriel ;
- nombre exact de vagues ;
- composition exacte des vagues ;
- statistiques et patterns des boss ;
- nombre de vaisseaux captifs ;
- nombre de chaînes/points d'attache par vaisseau ;
- formulation définitive de certains dialogues ;
- durée exacte des différentes séquences.

L'objectif actuel est d'abord d'obtenir une **démo jouable, courte et complète**, présentant les mécaniques principales et l'arc narratif de la Révolte d'Ashar sans augmenter inutilement le scope.