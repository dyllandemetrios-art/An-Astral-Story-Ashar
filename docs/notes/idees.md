# Idées hors périmètre (notées par Claude Code, jamais codées sans validation)

- **[P0-12] Réappliquer les réglages pixel art à la demande.** `PixelArtImportPostprocessor` n'agit qu'au premier import d'une texture (sans `.meta`), pour ne jamais écraser un réglage manuel. Un pack importé avec ses `.meta` (ex. `.unitypackage`) garderait donc ses réglages d'origine. Un menu `Ashar/Setup/Appliquer les réglages pixel art à la sélection` couvrirait ce cas. Utile au plus tard en P0-15.
- **[P0-12] Identité du build.** `PlayerSettings` garde `Company Name = DefaultCompany` : les sauvegardes et le `Player.log` iront dans `AppData/LocalLow/DefaultCompany/…`. À fixer avant la première sauvegarde (E6) ou la publication (E8).

- **[Commandes v2.4] Bouclier éphémère : point ouvert, bloquant pour E5-05.** La touche K (X à la manette) en fait une action du joueur, alors que le §7.10 le décrit comme un effet automatique. À trancher : durée fixe de 10 s une fois activé ? cooldown ? que se passe-t-il si K est pressée sans charge disponible ? Rien n'est codé.
- **[Commandes v2.4] `docs/Game_Concept.md` dit encore « ZQSD + espace/tirs »** (ligne 117, v2.3). À mettre à jour par toi en v2.4 : je n'ai pas modifié ce fichier.
