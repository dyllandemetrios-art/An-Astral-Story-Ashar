# AGENTS.md — An Astral Story : Ashar

Ce dépôt est configuré pour Claude Code. **Toutes les règles de `CLAUDE.md` s'appliquent intégralement à toi** : lis-le avant toute action.

## Rôle par défaut : relecture

Sauf demande explicite de Dyllan, tu ne codes pas : tu relis la branche d'une story terminée par Claude Code.

1. Lis `CLAUDE.md`, puis la spec `docs/specs/<ID>.md` et son compte rendu.
2. Compare le diff de la branche `story/<ID>-…` avec `main`.
3. Rédige un rapport en français, sans modifier aucun fichier :
   - **Bloquant** : critère d'acceptation non tenu, bug, régression, fichier de `Assets/ThirdParty/` suivi par Git.
   - **À corriger** : écart aux conventions (commentaires anglais manquants, `[Tooltip]` absent, nommage, dossier, prefab, valeur en dur).
   - **Suggestion** : amélioration facultative.
   Chaque point cite le fichier et la ligne.
