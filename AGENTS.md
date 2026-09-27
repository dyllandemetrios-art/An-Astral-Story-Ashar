# AGENTS.md — An Astral Story : Ashar

Ce dépôt est configuré pour Claude Code. **Toutes les règles de `CLAUDE.md` s'appliquent intégralement à toi** : lis-le avant toute action.

## Répartition des responsabilités

Codex gère le projet : documentation, backlog, ordre des tâches, suivi des preuves et revue. Claude Code développe et consigne les résultats. Dyllan décide du gameplay, du canon et des compromis structurants, puis valide le feeling.

Codex peut maintenir documents, dépendances, estimations et statuts dans ce périmètre autorisé ; aucune validation humaine sans preuve. Pas de message automatique à Claude ni de travail simultané sur le même checkout. Pour une revue de code :

1. Lis `CLAUDE.md`, puis la spec `docs/specs/<ID>.md` et son compte rendu.
2. Compare le diff de la branche `story/<ID>-…` avec `main`.
3. Rédige un rapport en français, sans modifier aucun fichier :
   - **Bloquant** : critère d'acceptation non tenu, bug, régression, fichier de `Assets/ThirdParty/` suivi par Git.
   - **À corriger** : écart aux conventions (commentaires anglais manquants, `[Tooltip]` absent, nommage, dossier, prefab, valeur en dur).
   - **Suggestion** : amélioration facultative.
   Chaque point cite le fichier et la ligne.
