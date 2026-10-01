# With Radar Elements

Read a radar marker's current color using the existing
`CCSRadarElement.ElementColor` schema property. `RadarElementColor` names the
CS2 point-script API's `CSRadarColor` values. The generated property remains a
`ref uint`, so existing plugins stay compatible and unknown values remain readable.

The example registers `css_radar_color [targetname|*suffix]` for players and the
server console, with no admin permission. It only reads entities and replies to
the caller. With no argument it lists all `radar_element` entities; a leading `*`
matches a targetname suffix, including prefab prefixes. All matches are printed
with their entity index and name instead of choosing an arbitrary entity.

```text
css_radar_color
css_radar_color ant.base.radar
css_radar_color *ant.base.radar
```

## Rush antenna example

On the inspected `rush_001` map, the script publishes the antenna's displayed
owner to `ant.base.radar` through
`UIUpdateRoomControl` → `UIUpdateAntennaOwningTeam` → `CSRadarPoint.SetColor`.
Read its `ElementColor` after the map has initialized:

| ElementColor | Radar color | Rush displayed owner |
| --- | --- | --- |
| 1 | Gray | Neutral |
| 3 | CounterTerrorist | CT (team 3) |
| 4 | Terrorist | T (team 2) |

Color IDs are not team numbers or packed RGBA values. In particular, color 2 is
white, not team T. This ownership interpretation belongs to this map's script;
other maps may use the same colors differently. Unknown colors must not be
treated as neutral.

The button's `OnPressed` output is not a complete ownership-change event. The
script also publishes state during initialization and round transitions, and
pressing the button may leave ownership unchanged. During warmup it updates the
display without updating the normal room-ownership array. This example reads
the published display state, not a predicted winner or the internal script array.
It requires no output hook, native signature or hardcoded field offset.

For change notifications in a plugin, read the marker periodically and compare
with the last known value, refreshing after map/round transitions. Treat a missing
marker as unavailable, not neutral; do not cache entity handles across maps.
