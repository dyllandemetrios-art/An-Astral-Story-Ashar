# Affectation de la musique (décidée par Dyllan après écoute, 26/09/2026)

Cette liste **remplace la proposition du §7.14** du cahier des charges (tableau « à valider à l'écoute »). Les 10 pistes sont importées dans `Assets/ThirdParty/AleksisTristanShaw/CyberpunkSynthwave/` (streaming Vorbis).

| Piste | Usage |
| --- | --- |
| Alien Weapon | Première mission (M1) |
| Cybernetic Breach | Combat rapide, fuite |
| Cyborgs vs. Androids | Combat intense |
| Highrise Massacre | Chemin d'épaves (champ de débris de M2) |
| Maverick Synth | Moments de pause (calme) **et** combat de boss |
| Mecha Fight | Combat de boss |
| Metropolis at Night | **Non utilisée** |
| Off-Grid Outlaw | Combat de difficulté moyenne |
| Posthuman | Boss final **et** menu principal |
| Resistance is Futile | Crédits de fin |

**Menu pause : aucune musique, et tout est vraiment mis en pause** (musique et sons compris, `Time.timeScale = 0` réservé à ce menu, voir la règle 9 de `CLAUDE.md`). À prévoir dans la story de pause : mettre en pause l'audio du jeu (par exemple `AudioListener.pause`), sans lancer de piste pour le menu pause, et le reprendre à la fermeture.

## Points à trancher quand les stories audio arriveront (E7-01 à E7-03)

- Plusieurs usages pour une même piste (Maverick Synth : pause **et** boss ; Posthuman : boss final **et** menu principal) : confirmer si c'est la même boucle qui reprend ou un départ à zéro à chaque fois.
- Plusieurs moments de combat (rapide, intense, moyen) : le choix de la piste par vague ou par phase se décide au moment de l'écriture des `MissionData` (champ `music`), pas avant.
- Le §7.14 prévoyait Metropolis at Night pour le menu et le codex : ils utiliseraient désormais Posthuman (menu principal) ; la musique du codex reste à préciser.
