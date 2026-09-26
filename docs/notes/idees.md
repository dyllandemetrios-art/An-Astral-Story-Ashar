# Idées hors périmètre (notées par Claude Code, jamais codées sans validation)

- **[P0-12] Réappliquer les réglages pixel art à la demande.** `PixelArtImportPostprocessor` n'agit qu'au premier import d'une texture (sans `.meta`), pour ne jamais écraser un réglage manuel. Un pack importé avec ses `.meta` (ex. `.unitypackage`) garderait donc ses réglages d'origine. Un menu `Ashar/Setup/Appliquer les réglages pixel art à la sélection` couvrirait ce cas. Utile au plus tard en P0-15.
- **[P0-12] Identité du build.** `PlayerSettings` garde `Company Name = DefaultCompany` : les sauvegardes et le `Player.log` iront dans `AppData/LocalLow/DefaultCompany/…`. À fixer avant la première sauvegarde (E6) ou la publication (E8).
