# Binding resolution behind a swappable seam

Spec B builds its own Vtable Check, Heal and Gamedata generator inside CounterStrikeSharp now, rather than waiting for the anchor-based resolver library being prototyped in `~/dev/cs2-0downtime-mod-stack-poc` (unbuilt; a feasibility test by its own ADR). To keep that resolver a drop-in later, resolution sits behind one seam at two levels:

- **Runtime:** CounterStrikeSharp resolves Bindings through one resolver interface whose only output is the Resolution Report (per Binding: status resolved/healed/failed, location, reason, evidence). The built-in signature scanner and fingerprint/alignment code is one adapter; the PoC library's C ABI would be another. Nothing outside the resolver touches binaries to locate Bindings.
- **Offline:** the generator's only output is Gamedata in the agreed additive shape. Any tool that emits that shape (the PoC's replay/regeneration tooling included) can replace it.

The fingerprint and alignment logic is one C++ implementation shared by the boot-time Vtable Check and the generator, so the two can never disagree; the Python research scripts serve as a test oracle.

## Considered Options

- **Depend on the PoC resolver for spec B**: ties T1 to an unproven feasibility test.
- **Python generator plus C++ boot check**: faster to write, but two fingerprint implementations drift, and each drift wrongly disables features.
