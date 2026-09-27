# Référence agent — Outillage Unity

À lire avant une commande Unity. Instructions déplacées de CLAUDE.md sans changement ; vérifier les commandes disponibles avant usage.

## Outillage : Unity CLI (voie active depuis P0-12)

L'éditeur de Dyllan reste ouvert sur le projet ; tu le pilotes depuis le terminal.
- `unity status` : vérifie qu'un éditeur est connecté (`state: ready`) avant toute action sur une scène ou un asset.
- `unity command <nom>` : `move_asset`, `create_scene`, `open_scene`, `save_scene`, `create_prefab`, `recompile` / `recompile_status`, `console` / `console_status`, `run_tests --mode EditMode`, `build` / `build_status`, `package_add` / `package_remove`… (`unity command` sans argument liste tout).
- `unity command eval_file --file <script.cs>` exécute du C# dans l'éditeur. Pas de directive `using` : écris les noms complets (`UnityEditor.AssetDatabase…`). Les scripts temporaires vont dans le scratchpad, jamais dans `Assets/`.
- Les commandes qui modifient le projet (paquets, réglages) exigent `--confirm true`. Après un changement de paquet ou une recompilation, l'éditeur recharge son domaine : une erreur « Network error » est normale, attends que `unity status` revienne à `ready`.
- Build de test : `Build/Windows/AsharDemo.exe` (dossier ignoré par Git).

