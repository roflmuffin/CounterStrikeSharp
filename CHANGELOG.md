# Changelog

All notable changes to this project will be documented in this file.

## What's Changed in v1.0.319
* Improve FunctionReference trace logging with real user stack origin by [@SlynxCZ](https://github.com/SlynxCZ) in [#895](https://github.com/roflmuffin/CounterStrikeSharp/pull/895) ([6f66316](https://github.com/roflmuffin/CounterStrikeSharp/commit/6f663164ee887bd1fd67c3bb3aef62314ada99e2))

## New Contributors
* [@SlynxCZ](https://github.com/SlynxCZ) made their first contribution in [#895](https://github.com/roflmuffin/CounterStrikeSharp/pull/895)

## What's Changed in v1.0.318
* fix(gameevents): merge and sort game event properties if duplicate events ([073728b](https://github.com/roflmuffin/CounterStrikeSharp/commit/073728b4ceb408e797ad0003f54ed6e232c41793))
* chore: cleanup cpp & add clang format linting in [#862](https://github.com/roflmuffin/CounterStrikeSharp/pull/862) ([6511a00](https://github.com/roflmuffin/CounterStrikeSharp/commit/6511a0098a5fa40f523411d149cca62bdfc4d132))
* chore(build): duplicate include path: ${SOURCESDK}/public/entity2 by [@jonathan-up](https://github.com/jonathan-up) in [#756](https://github.com/roflmuffin/CounterStrikeSharp/pull/756) ([f50fb78](https://github.com/roflmuffin/CounterStrikeSharp/commit/f50fb783bb76a4984e263736ec2cc5d59b5b1bae))

## New Contributors
* [@jonathan-up](https://github.com/jonathan-up) made their first contribution in [#756](https://github.com/roflmuffin/CounterStrikeSharp/pull/756)

## What's Changed in v1.0.317
* chore(gameevents): update game events (bullet_damage changes) ([4be0634](https://github.com/roflmuffin/CounterStrikeSharp/commit/4be063462547c9894169e6c816ff34f105513fbe))
* chore(schema): update (mainly removal of danger zone classes) ([7025b62](https://github.com/roflmuffin/CounterStrikeSharp/commit/7025b626156cf14a664c40ef9c6901c8db7217a6))
* chore(deps): upgrade metamod & hl2sdk in [#856](https://github.com/roflmuffin/CounterStrikeSharp/pull/856) ([462ca52](https://github.com/roflmuffin/CounterStrikeSharp/commit/462ca52229d859b593f02e3001d2c819877352af))
* chore(deps): bump libraries/Protobufs from `3d85413` to `53da9bc` by [@dependabot[bot]](https://github.com/dependabot[bot]) in [#857](https://github.com/roflmuffin/CounterStrikeSharp/pull/857) ([d7e23e8](https://github.com/roflmuffin/CounterStrikeSharp/commit/d7e23e8282e8247e99305db9bfcc45c4ff4e9012))
* feat: add Scoreboard and Inspect buttons to `PlayerButtons` ([3ddfa71](https://github.com/roflmuffin/CounterStrikeSharp/commit/3ddfa71e3f5d785510a7e3f85ac76155a29d3479))
* Fix potential event natives crashes by [@ipsvn](https://github.com/ipsvn) in [#852](https://github.com/roflmuffin/CounterStrikeSharp/pull/852) ([688b226](https://github.com/roflmuffin/CounterStrikeSharp/commit/688b226bcf41aca7b622b7db5f0168a70e3296ac))

## What's Changed in v1.0.316
* fix(gamedata): update `CCSPlayer_WeaponServices_CanUse` signature ([68e6ffa](https://github.com/roflmuffin/CounterStrikeSharp/commit/68e6ffaebe6497814d757b276a6d0371d731e61d))
* fix: commit links in changelog finally ([2f8f370](https://github.com/roflmuffin/CounterStrikeSharp/commit/2f8f370cd3dd162dff745671a159f361e1ce487b))

## What's Changed in v1.0.315
* Update CCSPlayer_ItemServices_CanAcquire signature by [@schwarper](https://github.com/schwarper) in [#832](https://github.com/roflmuffin/CounterStrikeSharp/pull/832) ([0ce4a29](https://github.com/roflmuffin/CounterStrikeSharp/commit/0ce4a2903cd661c0e5702bf6db38abfc6cb340e3))
* docs: add automatic build and deploy guide by [@uFloppyDisk](https://github.com/uFloppyDisk) in [#831](https://github.com/roflmuffin/CounterStrikeSharp/pull/831) ([33b46eb](https://github.com/roflmuffin/CounterStrikeSharp/commit/33b46eb2147224825ff2411b6973e41780f58e36))
* fix: move EventPlayerChat to dedicated file and exclude from generator ([a27ba3b](https://github.com/roflmuffin/CounterStrikeSharp/commit/a27ba3b0058b08b5aa9ddea12d36adc9d31374d3))
* chore(changelog): cleanup whitespace once and for all ([57286c9](https://github.com/roflmuffin/CounterStrikeSharp/commit/57286c9990db1033c3baa3c351d40d1f534b07e5))
* chore: fix commit links in release changelog ([ae808c0](https://github.com/roflmuffin/CounterStrikeSharp/commit/ae808c05c8b1ce0287c33b0daf754c54692b5ba8))

## New Contributors
* [@uFloppyDisk](https://github.com/uFloppyDisk) made their first contribution in [#831](https://github.com/roflmuffin/CounterStrikeSharp/pull/831)

## What's Changed in v1.0.314
* fix: manually revert EventPlayerChat to old value in [#827](https://github.com/roflmuffin/CounterStrikeSharp/pull/827) ([2398ba0](https://github.com/roflmuffin/CounterStrikeSharp/commit/2398ba0a5deb0780cc2fb9d94849c8e427034967))
* ci: hide release commits in changelog ([e45c204](https://github.com/roflmuffin/CounterStrikeSharp/commit/e45c20481d07a236dd8b6c79f22c42edfd1f7b5c))
* ci: include full changelog link in discord message ([fe321ee](https://github.com/roflmuffin/CounterStrikeSharp/commit/fe321ee93de7984d3808e9df33b045aefa43ad60))
* chore: remove footer from cliff changelog ([be19103](https://github.com/roflmuffin/CounterStrikeSharp/commit/be191035563df9786770f8c61c05527db1c9eda4))
* chore: fix newlines in changelog ([64cb26b](https://github.com/roflmuffin/CounterStrikeSharp/commit/64cb26b86d1818841af91164f44a2c62c5667969))

## What's Changed in v1.0.313
* ci: fix cliff generation ([637224d](https://github.com/roflmuffin/CounterStrikeSharp/commit/637224dc55a32e55163df64f809f9a64462e05e3))
* ci: add changelog to release & webhook ([3aca7c3](https://github.com/roflmuffin/CounterStrikeSharp/commit/3aca7c37f1f1a1f3bb7c630d373751ddab3cb5a2))
* chore(changelog): update cliff.toml ([5daf947](https://github.com/roflmuffin/CounterStrikeSharp/commit/5daf94791f2c468f1ff5b08c48d0f4cdd00003cc))
* feat(config): add toml loading support in [#804](https://github.com/roflmuffin/CounterStrikeSharp/pull/804) ([c50213c](https://github.com/roflmuffin/CounterStrikeSharp/commit/c50213c4425992ab13c3555b32428d0a58694516))
* chore: add links to contributors github page ([c02d31c](https://github.com/roflmuffin/CounterStrikeSharp/commit/c02d31cb2ee1dab4b662ba305ee0eab3c625561c))
* chore: update changelog to use semantic tags ([98cbca4](https://github.com/roflmuffin/CounterStrikeSharp/commit/98cbca44d4202d05bbf3fd1f8ec7f38255bacf4a))
* fix(gameevents): promote `core.gameevents` to have higher priority in [#819](https://github.com/roflmuffin/CounterStrikeSharp/pull/819) ([4cf88fc](https://github.com/roflmuffin/CounterStrikeSharp/commit/4cf88fc03e0dea699cef1a2f51ec9d7088769523))
* chrore: Implement SemVer instead of build numbers in [#816](https://github.com/roflmuffin/CounterStrikeSharp/pull/816) ([f1dff6d](https://github.com/roflmuffin/CounterStrikeSharp/commit/f1dff6d4d3f2b0dbf64615560e848b697d3a8904))

## What's Changed in v1.0.312
* fix(schema): inherited schema classes with clashing properties in [#818](https://github.com/roflmuffin/CounterStrikeSharp/pull/818) ([414a59a](https://github.com/roflmuffin/CounterStrikeSharp/commit/414a59a36de25735088a167158c3f0891baad575))

## What's Changed in v1.0.311
* fix(concommand): don't remove reference flags when running convar/concmd unlocker in [#817](https://github.com/roflmuffin/CounterStrikeSharp/pull/817) ([f1c1080](https://github.com/roflmuffin/CounterStrikeSharp/commit/f1c108087b6b176b0c08c50644575ba250b6455c))
* add CHANGELOG.md ([47ddf42](https://github.com/roflmuffin/CounterStrikeSharp/commit/47ddf42c115ca1ea8db165c0c4ab160e39abbc9f))

## What's Changed in v1.0.310
* Fix `EmitSoundFilter` signature for linux by [@samyycX](https://github.com/samyycX) in [#815](https://github.com/roflmuffin/CounterStrikeSharp/pull/815) ([ded133f](https://github.com/roflmuffin/CounterStrikeSharp/commit/ded133f1db95303c7f07118688fb5ee40e95206a))

## What's Changed in v1.0.309
* feat: Implement `EmitSoundFilter` by [@samyycX](https://github.com/samyycX) in [#808](https://github.com/roflmuffin/CounterStrikeSharp/pull/808) ([bbc621a](https://github.com/roflmuffin/CounterStrikeSharp/commit/bbc621acdc36cabf0f9b1d5ba1262129071ef536))

## What's Changed in v1.0.308
* fix: change terrorist chat colour to orange to be more in line with the game in [#813](https://github.com/roflmuffin/CounterStrikeSharp/pull/813) ([f7784c2](https://github.com/roflmuffin/CounterStrikeSharp/commit/f7784c26c67980830ed56d89a683661e9143affc))

## What's Changed in v1.0.307
* feat: add `ListenerHandlerAttribute<T>` by [@qstage](https://github.com/qstage) in [#757](https://github.com/roflmuffin/CounterStrikeSharp/pull/757) ([2da5448](https://github.com/roflmuffin/CounterStrikeSharp/commit/2da5448c8ea43eafc93be701c668b7ac332c477c))
* chore: update protobuf in [#803](https://github.com/roflmuffin/CounterStrikeSharp/pull/803) ([e406b78](https://github.com/roflmuffin/CounterStrikeSharp/commit/e406b78044e5ebd727f3277918ee84431b1c1406))
* Update publish-docs.yml in [#802](https://github.com/roflmuffin/CounterStrikeSharp/pull/802) ([3839831](https://github.com/roflmuffin/CounterStrikeSharp/commit/3839831ea9076f0c5624c161096a27ed3189f26a))

## What's Changed in v1.0.306
* feat: Expose Metamod `MetaFactory` to NativeAPI by [@samyycX](https://github.com/samyycX) in [#801](https://github.com/roflmuffin/CounterStrikeSharp/pull/801) ([54ad6c0](https://github.com/roflmuffin/CounterStrikeSharp/commit/54ad6c0b790e2725624bbf6705479fb3e22a9c31))
* chore: fix deprecated CI steps in [#759](https://github.com/roflmuffin/CounterStrikeSharp/pull/759) ([7c2cc8a](https://github.com/roflmuffin/CounterStrikeSharp/commit/7c2cc8a7f6785fd34e70e34f45ee9279e88385a7))

## What's Changed in v1.0.305
* fix: errors when reloading auto generated config by [@qstage](https://github.com/qstage) in [#754](https://github.com/roflmuffin/CounterStrikeSharp/pull/754) ([e99d27c](https://github.com/roflmuffin/CounterStrikeSharp/commit/e99d27ca30e80e8693d179570ccbee41b2af19f4))
* chore(deps): bump libraries/Protobufs from `b46090a` to `157162d` by [@dependabot[bot]](https://github.com/dependabot[bot]) in [#753](https://github.com/roflmuffin/CounterStrikeSharp/pull/753) ([44d3c51](https://github.com/roflmuffin/CounterStrikeSharp/commit/44d3c51bc70ac343d064add47e1eae0cb6b08c03))

## What's Changed in v1.0.304
* feat: add ReplicateConVar by [@21Joakim](https://github.com/21Joakim) in [#751](https://github.com/roflmuffin/CounterStrikeSharp/pull/751) ([f72e6d5](https://github.com/roflmuffin/CounterStrikeSharp/commit/f72e6d5daf7c9f9efb5ccda2cf0e842bcb5d35a9))

## New Contributors
* [@21Joakim](https://github.com/21Joakim) made their first contribution in [#751](https://github.com/roflmuffin/CounterStrikeSharp/pull/751)

## What's Changed in v1.0.303
* Remove the characters limit for CenterHtmlMenu by [@samyycX](https://github.com/samyycX) in [#747](https://github.com/roflmuffin/CounterStrikeSharp/pull/747) ([c8bccb0](https://github.com/roflmuffin/CounterStrikeSharp/commit/c8bccb07e0e4080131b03ce110941ac255b60284))

## New Contributors
* [@samyycX](https://github.com/samyycX) made their first contribution in [#747](https://github.com/roflmuffin/CounterStrikeSharp/pull/747)

## What's Changed in v1.0.302
* fix: `ArgumentOutOfRangeException` when calling `GetArgTargetResult` by [@qstage](https://github.com/qstage) in [#745](https://github.com/roflmuffin/CounterStrikeSharp/pull/745) ([2c06407](https://github.com/roflmuffin/CounterStrikeSharp/commit/2c0640700aa464283bcbd958203cc3045731a01d))

## New Contributors
* [@qstage](https://github.com/qstage) made their first contribution in [#745](https://github.com/roflmuffin/CounterStrikeSharp/pull/745)

## What's Changed in v1.0.301
* feat: handle quotes for `FakeConVar<string>` by [@ianlucas](https://github.com/ianlucas) in [#743](https://github.com/roflmuffin/CounterStrikeSharp/pull/743) ([0f71e1a](https://github.com/roflmuffin/CounterStrikeSharp/commit/0f71e1aebec79c877b8702309518638eb9085555))

## What's Changed in v1.0.300
* chore(deps): bump libraries/hl2sdk-cs2 from `a658a0f` to `a26ca82` by [@dependabot[bot]](https://github.com/dependabot[bot]) in [#739](https://github.com/roflmuffin/CounterStrikeSharp/pull/739) ([ac38ec2](https://github.com/roflmuffin/CounterStrikeSharp/commit/ac38ec249b17ac0c836db373436ba24ed9cf5dc0))

## What's Changed in v1.0.299
* Revert "Make NetworkedVector support Vector and QAngle " ([cff24f4](https://github.com/roflmuffin/CounterStrikeSharp/commit/cff24f4321c46aa2bf46a2cb7351d45c9a836407))

## What's Changed in v1.0.298
* fix: NetworkedVector throwing error ([f05cc5e](https://github.com/roflmuffin/CounterStrikeSharp/commit/f05cc5e043ce7c1b8e288150b6cfff9c8d74ddc3))

## What's Changed in v1.0.297
* Make NetworkedVector support Vector and QAngle by [@Yarukon](https://github.com/Yarukon) in [#728](https://github.com/roflmuffin/CounterStrikeSharp/pull/728) ([bd1105d](https://github.com/roflmuffin/CounterStrikeSharp/commit/bd1105d752cd9cdaed73e78ddc6681be67910a31))

## What's Changed in v1.0.296
* Fix RemoveMapChangeTimers in [#720](https://github.com/roflmuffin/CounterStrikeSharp/pull/720) ([9b4ee72](https://github.com/roflmuffin/CounterStrikeSharp/commit/9b4ee727c7cc3d3f29114296826971c55d98f7b1))
* chore: use ninja in linux build in [#722](https://github.com/roflmuffin/CounterStrikeSharp/pull/722) ([38d248d](https://github.com/roflmuffin/CounterStrikeSharp/commit/38d248defe4d2919014d70317b6627b5b74c8a43))

## What's Changed in v1.0.295
* Update Schema to Latest in [#721](https://github.com/roflmuffin/CounterStrikeSharp/pull/721) ([6b43069](https://github.com/roflmuffin/CounterStrikeSharp/commit/6b4306948ba3b378bc89546eb9441d95dea00fa9))
* Update getting-started.md ([3fee00e](https://github.com/roflmuffin/CounterStrikeSharp/commit/3fee00e8c483d995331e7be341d0b4d769ff151e))
* chore: upgrade to cxx20 in [#719](https://github.com/roflmuffin/CounterStrikeSharp/pull/719) ([d22af14](https://github.com/roflmuffin/CounterStrikeSharp/commit/d22af142cbde5e66dd2a586b2221eefe849e1025))
* chore(deps): bump libraries/hl2sdk-cs2 from `14e77af` to `a658a0f` by [@dependabot[bot]](https://github.com/dependabot[bot]) in [#718](https://github.com/roflmuffin/CounterStrikeSharp/pull/718) ([c1176a3](https://github.com/roflmuffin/CounterStrikeSharp/commit/c1176a3192a981e1ce71d8171792bf887c226528))
* chore(deps): bump libraries/Protobufs from `76e070d` to `b46090a` by [@dependabot[bot]](https://github.com/dependabot[bot]) in [#717](https://github.com/roflmuffin/CounterStrikeSharp/pull/717) ([3c321be](https://github.com/roflmuffin/CounterStrikeSharp/commit/3c321be5a09b3848fd026315abc4eb1b4e816ee4))

## What's Changed in v1.0.294
* chore: back to using hl2sdk ([466da1b](https://github.com/roflmuffin/CounterStrikeSharp/commit/466da1b193d45d2f01d6fc3a69230b529b6583eb))

## What's Changed in v1.0.293
* fix: temporary patch hl2sdk entity listeners ([ba860a9](https://github.com/roflmuffin/CounterStrikeSharp/commit/ba860a92d2cb36db7c4664ee63dacab7c4435ec7))

## What's Changed in v1.0.292
* Fix `GiveNamedItem` and `CanAcquire` signatures by [@ianlucas](https://github.com/ianlucas) in [#713](https://github.com/roflmuffin/CounterStrikeSharp/pull/713) ([461fc0e](https://github.com/roflmuffin/CounterStrikeSharp/commit/461fc0ea6797eb6f80eb90f6bc24f12265cc1eeb))

## What's Changed in v1.0.291
* fix: revert base entity teleport changes, add warning to player controller in [#688](https://github.com/roflmuffin/CounterStrikeSharp/pull/688) ([6349c11](https://github.com/roflmuffin/CounterStrikeSharp/commit/6349c11d07af8ba95d280e4a0f994ccb46053ef5))
* Shuffle player documentation, add to game event documentation by [@zonical](https://github.com/zonical) in [#685](https://github.com/roflmuffin/CounterStrikeSharp/pull/685) ([b2046b2](https://github.com/roflmuffin/CounterStrikeSharp/commit/b2046b21c4412c84c85ffd933119871888fe22cb))
* Update publish-docs.yml ([3c6be48](https://github.com/roflmuffin/CounterStrikeSharp/commit/3c6be481c57464b7a5adfa901065f1695c8c3c85))
* Allow manual publish-docs.yml ([0a6fe09](https://github.com/roflmuffin/CounterStrikeSharp/commit/0a6fe0946db2d62f3d01f2abd275300fe78f37d8))

## What's Changed in v1.0.290
* Added hitgroup to CTakeDamageInfo by [@schwarper](https://github.com/schwarper) in [#665](https://github.com/roflmuffin/CounterStrikeSharp/pull/665) ([7929751](https://github.com/roflmuffin/CounterStrikeSharp/commit/79297511e386e32042fe0bb2d6e95aad37ef1389))

## What's Changed in v1.0.289
* CBaseEntity player teleport adjustment update by [@schwarper](https://github.com/schwarper) in [#661](https://github.com/roflmuffin/CounterStrikeSharp/pull/661) ([c6d3988](https://github.com/roflmuffin/CounterStrikeSharp/commit/c6d39889026c995bfe131434b6a3891a4019388c))

## What's Changed in v1.0.288
* Added PluginConfigExtensions by [@schwarper](https://github.com/schwarper) in [#675](https://github.com/roflmuffin/CounterStrikeSharp/pull/675) ([8a063f4](https://github.com/roflmuffin/CounterStrikeSharp/commit/8a063f4fb63a839a6a515c92d22df59947dafee0))

## What's Changed in v1.0.287
* System.ArgumentException: Player with slot X not found by [@oqyh](https://github.com/oqyh) in [#667](https://github.com/roflmuffin/CounterStrikeSharp/pull/667) ([6cf124b](https://github.com/roflmuffin/CounterStrikeSharp/commit/6cf124bfb6708c9decf653eae356718c2954b91e))

## New Contributors
* [@oqyh](https://github.com/oqyh) made their first contribution in [#667](https://github.com/roflmuffin/CounterStrikeSharp/pull/667)

## What's Changed in v1.0.286
* Added TargetType.PlayerAim by [@schwarper](https://github.com/schwarper) in [#639](https://github.com/roflmuffin/CounterStrikeSharp/pull/639) ([1f904a5](https://github.com/roflmuffin/CounterStrikeSharp/commit/1f904a52a7a41558128cd360e82c89562848c066))

## What's Changed in v1.0.285
* Implemented entity transmit feature by [@KillStr3aK](https://github.com/KillStr3aK) in [#608](https://github.com/roflmuffin/CounterStrikeSharp/pull/608) ([32c99b2](https://github.com/roflmuffin/CounterStrikeSharp/commit/32c99b2e4983eb68aa0cb80384aa41f38c8009f3))

## What's Changed in v1.0.284
* fix: only close the menu if it has exit button by [@KillStr3aK](https://github.com/KillStr3aK) in [#622](https://github.com/roflmuffin/CounterStrikeSharp/pull/622) ([5c9d38b](https://github.com/roflmuffin/CounterStrikeSharp/commit/5c9d38b2b006e7edf544bb8f185acb4bd5fb6722))

## What's Changed in v1.0.283
* fix: gamedata update by [@KillStr3aK](https://github.com/KillStr3aK) in [#631](https://github.com/roflmuffin/CounterStrikeSharp/pull/631) ([b54f5c3](https://github.com/roflmuffin/CounterStrikeSharp/commit/b54f5c3dee56779e2b62410935b2b84f9c2e5173))

## What's Changed in v1.0.282
* CCSPlayer_ItemServices_CanAcquire and GetCSWeaponDataFromKey signatures update by [@schwarper](https://github.com/schwarper) in [#636](https://github.com/roflmuffin/CounterStrikeSharp/pull/636) ([761380d](https://github.com/roflmuffin/CounterStrikeSharp/commit/761380dff600860ca702d74a98be6edef1fa9e3a))

## What's Changed in v1.0.281
* Add GetGameframeTime to NativeAPI by [@Interesting-exe](https://github.com/Interesting-exe) in [#627](https://github.com/roflmuffin/CounterStrikeSharp/pull/627) ([71ae253](https://github.com/roflmuffin/CounterStrikeSharp/commit/71ae253e5ef3ff42a0b85b4eb7d7da9fa2278f72))

## New Contributors
* [@Interesting-exe](https://github.com/Interesting-exe) made their first contribution in [#627](https://github.com/roflmuffin/CounterStrikeSharp/pull/627)

## What's Changed in v1.0.280
* Add GetCSWeaponDataFromKey and CCSPlayer_ItemServices_CanAcquire by [@schwarper](https://github.com/schwarper) in [#628](https://github.com/roflmuffin/CounterStrikeSharp/pull/628) ([c2f212d](https://github.com/roflmuffin/CounterStrikeSharp/commit/c2f212df518c7d12c7d916be95d02e0c7af8c580))

## New Contributors
* [@schwarper](https://github.com/schwarper) made their first contribution in [#628](https://github.com/roflmuffin/CounterStrikeSharp/pull/628)

## What's Changed in v1.0.279
* fix: `InvalidOperationException` when removing command in its callback by [@ianlucas](https://github.com/ianlucas) in [#626](https://github.com/roflmuffin/CounterStrikeSharp/pull/626) ([ad7f7bd](https://github.com/roflmuffin/CounterStrikeSharp/commit/ad7f7bd365d7080060acc94539830a9066cc4633))

## What's Changed in v1.0.278
* fix: remove command to use command manager by [@ianlucas](https://github.com/ianlucas) in [#579](https://github.com/roflmuffin/CounterStrikeSharp/pull/579) ([a0fcb78](https://github.com/roflmuffin/CounterStrikeSharp/commit/a0fcb7817ee1fe00183d54cc0c6e01062d7c95fb))
* New `NetworkDisconnectionReason` values by [@KillStr3aK](https://github.com/KillStr3aK) in [#621](https://github.com/roflmuffin/CounterStrikeSharp/pull/621) ([cdb7a6e](https://github.com/roflmuffin/CounterStrikeSharp/commit/cdb7a6ed17654937b3976268ba1fcc374376f610))
* chore(deps): bump libraries/hl2sdk-cs2 from `9be8cba` to `fc4b98f` by [@dependabot[bot]](https://github.com/dependabot[bot]) in [#620](https://github.com/roflmuffin/CounterStrikeSharp/pull/620) ([38e2953](https://github.com/roflmuffin/CounterStrikeSharp/commit/38e29531c396631cd1d9ff97faed55673200c233))
* chore(deps): bump libraries/Protobufs from `3ea793c` to `76e070d` by [@dependabot[bot]](https://github.com/dependabot[bot]) in [#619](https://github.com/roflmuffin/CounterStrikeSharp/pull/619) ([5f95969](https://github.com/roflmuffin/CounterStrikeSharp/commit/5f9596998059fb9baec1d193be2af8f048a641e1))

## What's Changed in v1.0.276
* chore(deps): bump libraries/Protobufs from `686a062` to `3ea793c` by [@dependabot[bot]](https://github.com/dependabot[bot]) in [#607](https://github.com/roflmuffin/CounterStrikeSharp/pull/607) ([42dd270](https://github.com/roflmuffin/CounterStrikeSharp/commit/42dd270b7852c7d866a8bdf835524fc0bd27a787))

## What's Changed in v1.0.275
* chore(deps): bump libraries/hl2sdk-cs2 from `1f1d158` to `9be8cba` by [@dependabot[bot]](https://github.com/dependabot[bot]) in [#606](https://github.com/roflmuffin/CounterStrikeSharp/pull/606) ([b807c3e](https://github.com/roflmuffin/CounterStrikeSharp/commit/b807c3eda80330eadcbd2e018fa1a3714687f1c2))

## What's Changed in v1.0.274
* Update dependabot.yaml ([3ede4c3](https://github.com/roflmuffin/CounterStrikeSharp/commit/3ede4c366cde8d8f46f36cadf9a30113b763a0d1))
* fix CreateEvent leak by [@number201724](https://github.com/number201724) in [#604](https://github.com/roflmuffin/CounterStrikeSharp/pull/604) ([49cc91e](https://github.com/roflmuffin/CounterStrikeSharp/commit/49cc91e373b5b7cfc40d15ec34e1ed1a3a94e352))

## New Contributors
* [@number201724](https://github.com/number201724) made their first contribution in [#604](https://github.com/roflmuffin/CounterStrikeSharp/pull/604)

## What's Changed in v1.0.273
* fix: CCSPlayerPawnBase_PostThink signature by [@stefanx111](https://github.com/stefanx111) in [#601](https://github.com/roflmuffin/CounterStrikeSharp/pull/601) ([8a795de](https://github.com/roflmuffin/CounterStrikeSharp/commit/8a795de9fa7631e0f887378b0b75a88b3b6beadb))

## What's Changed in v1.0.272
* chore(deps): bump libraries/hl2sdk-cs2 from `3c7b355` to `1f1d158` by [@dependabot[bot]](https://github.com/dependabot[bot]) in [#600](https://github.com/roflmuffin/CounterStrikeSharp/pull/600) ([e36d2e0](https://github.com/roflmuffin/CounterStrikeSharp/commit/e36d2e07e43c581ce19edc0ff4604e51efd37030))

## What's Changed in v1.0.271
* Fix crash after map change by [@Pisex](https://github.com/Pisex) in [#593](https://github.com/roflmuffin/CounterStrikeSharp/pull/593) ([cdd2a82](https://github.com/roflmuffin/CounterStrikeSharp/commit/cdd2a8275e138787588157d867df77aa0588c8c7))

## New Contributors
* [@Pisex](https://github.com/Pisex) made their first contribution in [#593](https://github.com/roflmuffin/CounterStrikeSharp/pull/593)

## What's Changed in v1.0.270
* Update Schema for Armory Update in [#592](https://github.com/roflmuffin/CounterStrikeSharp/pull/592) ([2b31f51](https://github.com/roflmuffin/CounterStrikeSharp/commit/2b31f519ebcffc14384d915f2a70740fc73185bd))

## What's Changed in v1.0.269
* fix: armory update broken signatures and offsets by [@KillStr3aK](https://github.com/KillStr3aK) in [#584](https://github.com/roflmuffin/CounterStrikeSharp/pull/584) ([2c7f896](https://github.com/roflmuffin/CounterStrikeSharp/commit/2c7f896189fe04b4f25f6da2b4014a00a90923c3))

## What's Changed in v1.0.268
* chore(deps): bump libraries/hl2sdk-cs2 from `40a9bb9` to `3c7b355` by [@dependabot[bot]](https://github.com/dependabot[bot]) in [#594](https://github.com/roflmuffin/CounterStrikeSharp/pull/594) ([5a354a2](https://github.com/roflmuffin/CounterStrikeSharp/commit/5a354a25e3e3b05e67cd3eac6eb3ea5625add899))

## What's Changed in v1.0.267
* chore(deps): bump libraries/hl2sdk-cs2 from `f21e0c9` to `40a9bb9` by [@dependabot[bot]](https://github.com/dependabot[bot]) in [#586](https://github.com/roflmuffin/CounterStrikeSharp/pull/586) ([74ce0d2](https://github.com/roflmuffin/CounterStrikeSharp/commit/74ce0d295a9c0b9402817e9077f72459a6600418))

## What's Changed in v1.0.266
* Fix calling Send using UM instance from UM hook by [@Yarukon](https://github.com/Yarukon) in [#578](https://github.com/roflmuffin/CounterStrikeSharp/pull/578) ([9c8f25f](https://github.com/roflmuffin/CounterStrikeSharp/commit/9c8f25f721ed5a5e2043b348ebc92fe646935c0a))

## What's Changed in v1.0.265
* CoreConfig: Prevent "Error invoking callback" if core.json not found by [@markus-wa](https://github.com/markus-wa) in [#576](https://github.com/roflmuffin/CounterStrikeSharp/pull/576) ([4b1a2c4](https://github.com/roflmuffin/CounterStrikeSharp/commit/4b1a2c427ec5681b603da33a35c5cb15cea5c62a))

## New Contributors
* [@markus-wa](https://github.com/markus-wa) made their first contribution in [#576](https://github.com/roflmuffin/CounterStrikeSharp/pull/576)

## What's Changed in v1.0.264
* fix: prevent early global cleanup when inside invoke ([8f59fd5](https://github.com/roflmuffin/CounterStrikeSharp/commit/8f59fd5b97bd1f62cd9b3fb6c0a08f3d374766d8))
* Update README ([cbeac50](https://github.com/roflmuffin/CounterStrikeSharp/commit/cbeac50e4a07b243262d4f215fcb3124a7558f74))
* Update README.md ([eba7d9c](https://github.com/roflmuffin/CounterStrikeSharp/commit/eba7d9c313e7a05f0c296f7a18cd571eba9683b2))

## What's Changed in v1.0.263
* chore(deps): bump libraries/hl2sdk-cs2 from `0b862d7` to `f21e0c9` by [@dependabot[bot]](https://github.com/dependabot[bot]) in [#560](https://github.com/roflmuffin/CounterStrikeSharp/pull/560) ([2382815](https://github.com/roflmuffin/CounterStrikeSharp/commit/23828153eb1436794eb4f83608a76ec1db3ad6ce))

## What's Changed in v1.0.262
* feat: add `player.Disconnect(reason)` method ([0e3698b](https://github.com/roflmuffin/CounterStrikeSharp/commit/0e3698b3705c0a660bfcdf519a721dc67436e69f))

## What's Changed in v1.0.261
* Cleanup Protobuf Generation in [#562](https://github.com/roflmuffin/CounterStrikeSharp/pull/562) ([bb38b1c](https://github.com/roflmuffin/CounterStrikeSharp/commit/bb38b1cb1af3df079cdf9d3227e1025d41bfa0b3))

## What's Changed in v1.0.260
* Implement Usermessages in [#435](https://github.com/roflmuffin/CounterStrikeSharp/pull/435) ([10f472e](https://github.com/roflmuffin/CounterStrikeSharp/commit/10f472ec85cf7dd8f7d338c1fdf742e0d9c2355b))

## What's Changed in v1.0.259
* chore(deps): bump libraries/hl2sdk-cs2 from `c57d5ab` to `0b862d7` by [@dependabot[bot]](https://github.com/dependabot[bot]) in [#550](https://github.com/roflmuffin/CounterStrikeSharp/pull/550) ([28ce183](https://github.com/roflmuffin/CounterStrikeSharp/commit/28ce183cdcabdc9533aae20dc359f7e23c0938aa))

## What's Changed in v1.0.258
* feat: add localizer extension methods for player language ([dddf24d](https://github.com/roflmuffin/CounterStrikeSharp/commit/dddf24d0f3207af59b79823a4e92b1eccdbac637))

## What's Changed in v1.0.257
* Update SetStateChanged parameters by [@Yarukon](https://github.com/Yarukon) in [#541](https://github.com/roflmuffin/CounterStrikeSharp/pull/541) ([fce68cb](https://github.com/roflmuffin/CounterStrikeSharp/commit/fce68cb16067379685dbda241b29aee331d04f80))

## What's Changed in v1.0.256
* Fix CBasePlayerController_SetPawn signature by [@ianlucas](https://github.com/ianlucas) in [#547](https://github.com/roflmuffin/CounterStrikeSharp/pull/547) ([a82faeb](https://github.com/roflmuffin/CounterStrikeSharp/commit/a82faeb5c8b9600181f9fdcc981141dc499993d3))

## What's Changed in v1.0.255
* chore(deps): bump libraries/hl2sdk-cs2 from `a5d9f80` to `c57d5ab` by [@dependabot[bot]](https://github.com/dependabot[bot]) in [#542](https://github.com/roflmuffin/CounterStrikeSharp/pull/542) ([ce3ff44](https://github.com/roflmuffin/CounterStrikeSharp/commit/ce3ff449b95d34cfaa4e658e3239a21f4d1a3827))
* fix: update gamedata json for update ([aa40d81](https://github.com/roflmuffin/CounterStrikeSharp/commit/aa40d81a86d0196e11dec23d200ec2c3afea0cdf))

## What's Changed in v1.0.253
* Improve plugin unloading consistency in [#532](https://github.com/roflmuffin/CounterStrikeSharp/pull/532) ([5644921](https://github.com/roflmuffin/CounterStrikeSharp/commit/5644921873663fada02f63ba70bbef60653d4cc0))

## What's Changed in v1.0.252
* fix: update CNetworkQuantizedFloat to resolve to float ([3860ca1](https://github.com/roflmuffin/CounterStrikeSharp/commit/3860ca1662bd66593fe02bd3f366427024d640ab))

## What's Changed in v1.0.251
* chore: update schema in [#531](https://github.com/roflmuffin/CounterStrikeSharp/pull/531) ([b79fd19](https://github.com/roflmuffin/CounterStrikeSharp/commit/b79fd19c8594a014744d64cb6edfcac492d64355))
* chore(deps): bump libraries/hl2sdk-cs2 from `4b31db7` to `a5d9f80` by [@dependabot[bot]](https://github.com/dependabot[bot]) in [#522](https://github.com/roflmuffin/CounterStrikeSharp/pull/522) ([b7ea025](https://github.com/roflmuffin/CounterStrikeSharp/commit/b7ea025b03001a7caf06085b6d96e2fa3b6a1496))
* chore: add actionlint in [#530](https://github.com/roflmuffin/CounterStrikeSharp/pull/530) ([5ac173d](https://github.com/roflmuffin/CounterStrikeSharp/commit/5ac173df51e9d3c5f7b4d4a1938ecafe907147c8))

## What's Changed in v1.0.249
* feat: update game events (adds `bullet_damage` event) ([c82a58f](https://github.com/roflmuffin/CounterStrikeSharp/commit/c82a58f5ab4c532981b8fe64c31c4c98ece4a575))

## What's Changed in v1.0.248
* Fix function hooking on windows by [@Yarukon](https://github.com/Yarukon) in [#529](https://github.com/roflmuffin/CounterStrikeSharp/pull/529) ([1806919](https://github.com/roflmuffin/CounterStrikeSharp/commit/180691955935ed02b48efd93410c7742c5b194fa))

## What's Changed in v1.0.247
* Update gamedata.json by [@xLeviNx](https://github.com/xLeviNx) in [#521](https://github.com/roflmuffin/CounterStrikeSharp/pull/521) ([f8451c2](https://github.com/roflmuffin/CounterStrikeSharp/commit/f8451c2818a26380b49229655956e3058c3855ff))

## New Contributors
* [@xLeviNx](https://github.com/xLeviNx) made their first contribution in [#521](https://github.com/roflmuffin/CounterStrikeSharp/pull/521)

## What's Changed in v1.0.246
* Improve Teleport Function by [@Yarukon](https://github.com/Yarukon) in [#513](https://github.com/roflmuffin/CounterStrikeSharp/pull/513) ([a87bd25](https://github.com/roflmuffin/CounterStrikeSharp/commit/a87bd25b48ff1407a71cfdce3222f5f55c8a2e0b))

## What's Changed in v1.0.245
* fix: bad vector natives ([9c5468e](https://github.com/roflmuffin/CounterStrikeSharp/commit/9c5468e5baecd8b9dd6b4187fd1b50f1474215fb))
* Add Linux Dev Container in [#448](https://github.com/roflmuffin/CounterStrikeSharp/pull/448) ([f826be6](https://github.com/roflmuffin/CounterStrikeSharp/commit/f826be664fc7ab1d337ad540da8832b7fcdda4ae))

## What's Changed in v1.0.244
* Add Basic VPROF Budgets, Add Con Command Unlocker to Core in [#505](https://github.com/roflmuffin/CounterStrikeSharp/pull/505) ([7f5103d](https://github.com/roflmuffin/CounterStrikeSharp/commit/7f5103d9ee5fdef2138eb4ca65c6033df43468db))

## What's Changed in v1.0.243
* chore(deps): bump libraries/hl2sdk-cs2 from `4202f1c` to `4b31db7` by [@dependabot[bot]](https://github.com/dependabot[bot]) in [#496](https://github.com/roflmuffin/CounterStrikeSharp/pull/496) ([877b7c5](https://github.com/roflmuffin/CounterStrikeSharp/commit/877b7c5a4ae45aa5cbfd5397f793348113f4013d))
* Change example command prefix to follow "best practice" by [@WidovV](https://github.com/WidovV) in [#503](https://github.com/roflmuffin/CounterStrikeSharp/pull/503) ([24363d6](https://github.com/roflmuffin/CounterStrikeSharp/commit/24363d6352c93d523a67677296df5a4190eb6cb6))

## What's Changed in v1.0.242
* Fix AddResource offset for Linux by [@Yarukon](https://github.com/Yarukon) in [#479](https://github.com/roflmuffin/CounterStrikeSharp/pull/479) ([2eaf7c2](https://github.com/roflmuffin/CounterStrikeSharp/commit/2eaf7c2d8c7ab0810ed87756ac5f6c1cf6d756e4))
* chore(deps): bump libraries/hl2sdk-cs2 from `739c88f` to `4202f1c` by [@dependabot[bot]](https://github.com/dependabot[bot]) in [#494](https://github.com/roflmuffin/CounterStrikeSharp/pull/494) ([8b486ec](https://github.com/roflmuffin/CounterStrikeSharp/commit/8b486ecf7e77e913f1987d49e810de0451280002))

## What's Changed in v1.0.240
* fix: CBasePlayerController_SetPawn signature by [@stefanx111](https://github.com/stefanx111) in [#493](https://github.com/roflmuffin/CounterStrikeSharp/pull/493) ([54cc93e](https://github.com/roflmuffin/CounterStrikeSharp/commit/54cc93e0f8df345257ce236f23ee0f9d7aeaaccb))

## New Contributors
* [@stefanx111](https://github.com/stefanx111) made their first contribution in [#493](https://github.com/roflmuffin/CounterStrikeSharp/pull/493)

## What's Changed in v1.0.239
* Revert "fix: improve error handling if globals are accessed before ready" ([a695eec](https://github.com/roflmuffin/CounterStrikeSharp/commit/a695eec4fafd20ac3966f45ead12c695216a2f61))

## What's Changed in v1.0.238
* fix: improve error handling if globals are accessed before ready ([e207be2](https://github.com/roflmuffin/CounterStrikeSharp/commit/e207be2fbff11b575bc4a757634b95e394f331bf))

## What's Changed in v1.0.237
* Update ``GiveNamedItem`` signature for linux by [@Nukoooo](https://github.com/Nukoooo) in [#470](https://github.com/roflmuffin/CounterStrikeSharp/pull/470) ([eea6451](https://github.com/roflmuffin/CounterStrikeSharp/commit/eea64519a65333b27a714feb87d9e563077d9cac))

## What's Changed in v1.0.236
* Update `GiveNamedItem` signature on Linux by [@ianlucas](https://github.com/ianlucas) in [#467](https://github.com/roflmuffin/CounterStrikeSharp/pull/467) ([13ec19e](https://github.com/roflmuffin/CounterStrikeSharp/commit/13ec19e4121af82e3724a02d88c19917a9fe67e1))

## What's Changed in v1.0.235
* Update `GiveNamedItem` signature on Linux by [@ianlucas](https://github.com/ianlucas) in [#463](https://github.com/roflmuffin/CounterStrikeSharp/pull/463) ([3240a5e](https://github.com/roflmuffin/CounterStrikeSharp/commit/3240a5e582b30e500401e7b1083351568f229575))

## What's Changed in v1.0.234
* Initial update after 2024-05-23 CS2 update in [#461](https://github.com/roflmuffin/CounterStrikeSharp/pull/461) ([cafc4e2](https://github.com/roflmuffin/CounterStrikeSharp/commit/cafc4e237fb51541ad6c722f2b550d8c4e2932dd))

## What's Changed in v1.0.233
* chore(deps): bump libraries/hl2sdk-cs2 from `f7ed6a0` to `36dd2db` by [@dependabot[bot]](https://github.com/dependabot[bot]) in [#452](https://github.com/roflmuffin/CounterStrikeSharp/pull/452) ([02d5191](https://github.com/roflmuffin/CounterStrikeSharp/commit/02d5191e746aa08fb912746ceac61d4c891f9146))

## What's Changed in v1.0.232
* chore(deps): bump libraries/hl2sdk-cs2 from `9ddef9a` to `f7ed6a0` by [@dependabot[bot]](https://github.com/dependabot[bot]) in [#451](https://github.com/roflmuffin/CounterStrikeSharp/pull/451) ([aec696a](https://github.com/roflmuffin/CounterStrikeSharp/commit/aec696abc097010e1b23a4060ff9e4d3d75235a8))

## What's Changed in v1.0.231
* chore: fix dependabot prefix ([c01aeec](https://github.com/roflmuffin/CounterStrikeSharp/commit/c01aeec14bb136a1cf9e7803a01e3913592157a9))
* chore(deps): update hl2sdk: bump libraries/metamod-source from `e857fbe` to `607301a` by [@dependabot[bot]](https://github.com/dependabot[bot]) in [#450](https://github.com/roflmuffin/CounterStrikeSharp/pull/450) ([41e7bee](https://github.com/roflmuffin/CounterStrikeSharp/commit/41e7bee85adbf01ef5efee99e2e1dbe4bf0078d8))
* chore: add metamod source dependabot config ([9834271](https://github.com/roflmuffin/CounterStrikeSharp/commit/9834271956cd28307874a77281084ee09b22703e))

## What's Changed in v1.0.230
* Back to C++17 in [#447](https://github.com/roflmuffin/CounterStrikeSharp/pull/447) ([fb5967d](https://github.com/roflmuffin/CounterStrikeSharp/commit/fb5967d102bb5aac480a17372734411d0f78f4e1))

## What's Changed in v1.0.229
* chore(deps): update hl2sdk: bump libraries/hl2sdk-cs2 from `3fc8d0f` to `9ddef9a` by [@dependabot[bot]](https://github.com/dependabot[bot]) in [#446](https://github.com/roflmuffin/CounterStrikeSharp/pull/446) ([928bc3f](https://github.com/roflmuffin/CounterStrikeSharp/commit/928bc3f74d1b5da88c072411f6425057ee08ceae))
* feat: add hl2sdk-cs2 dependabot ([6a7d7db](https://github.com/roflmuffin/CounterStrikeSharp/commit/6a7d7dba4f8403c350728a3e6cbf03c62735dd5c))

## New Contributors
* [@dependabot[bot]](https://github.com/dependabot[bot]) made their first contribution in [#446](https://github.com/roflmuffin/CounterStrikeSharp/pull/446)

## What's Changed in v1.0.228
* fix: concommand crash when no description ([11e5e99](https://github.com/roflmuffin/CounterStrikeSharp/commit/11e5e9972d48d8280a2b35003b95f48a2c45aee7))

## What's Changed in v1.0.227
* chore: bump hl2sdk after update in [#434](https://github.com/roflmuffin/CounterStrikeSharp/pull/434) ([9a221b4](https://github.com/roflmuffin/CounterStrikeSharp/commit/9a221b4ebb1f6f68440659a4b432b09c5fb88699))

## What's Changed in v1.0.226
* AcceptInput changes by [@Yarukon](https://github.com/Yarukon) in [#433](https://github.com/roflmuffin/CounterStrikeSharp/pull/433) ([e270bdf](https://github.com/roflmuffin/CounterStrikeSharp/commit/e270bdfd4459c57ae77c96495d7a2e8ec66a2ef9))

## What's Changed in v1.0.225
* fix: add memoverride on windows, use correct MT flag in [#431](https://github.com/roflmuffin/CounterStrikeSharp/pull/431) ([f591fe5](https://github.com/roflmuffin/CounterStrikeSharp/commit/f591fe58d050dab2e30692ecf7add09cfce8b5d2))

## What's Changed in v1.0.224
* chore: improve ccsplayercontroller helpers validity checks ([052cb4e](https://github.com/roflmuffin/CounterStrikeSharp/commit/052cb4e14e7db947e8f1f7d40705fddf37de56f7))

## What's Changed in v1.0.222
* feat: improve docs and add more null checking for handle accesses in [#417](https://github.com/roflmuffin/CounterStrikeSharp/pull/417) ([bc3bec4](https://github.com/roflmuffin/CounterStrikeSharp/commit/bc3bec4aa888dd1cbb6f0e17a947bc2c3998381f))

## What's Changed in v1.0.221
* feat: add `PrintToCenterAlert` player helper ([20bab7f](https://github.com/roflmuffin/CounterStrikeSharp/commit/20bab7f4a8fc1136fb299d08d6edb02471bea35d))

## What's Changed in v1.0.220
* feat: add generic math operators to Vector ([c7eac71](https://github.com/roflmuffin/CounterStrikeSharp/commit/c7eac71109accde348806c4362043989813ec013))

## What's Changed in v1.0.219
* chore: use IDA style sigs for gamedata ([e3d2370](https://github.com/roflmuffin/CounterStrikeSharp/commit/e3d2370e2e8ab0725a8666389a7887504d3c2ac1))

## What's Changed in v1.0.218
* chore: update give named item sig for linux ([1422427](https://github.com/roflmuffin/CounterStrikeSharp/commit/142242744c122ca8aac0dfe285a08b0668653c5b))

## What's Changed in v1.0.217
* Copy bytes of a module to prevent signature scanning failures and some minor changes by [@Nukoooo](https://github.com/Nukoooo) in [#414](https://github.com/roflmuffin/CounterStrikeSharp/pull/414) ([0eebffd](https://github.com/roflmuffin/CounterStrikeSharp/commit/0eebffd860ab893822f015618576e6291262ea7b))

## What's Changed in v1.0.216
* chore: use offsets for give named item in [#427](https://github.com/roflmuffin/CounterStrikeSharp/pull/427) ([25ca5db](https://github.com/roflmuffin/CounterStrikeSharp/commit/25ca5dbe0c4ebca038e97750c781ca116eee874b))

## What's Changed in v1.0.215
* Fix: new game update broke signatures by [@KillStr3aK](https://github.com/KillStr3aK) in [#425](https://github.com/roflmuffin/CounterStrikeSharp/pull/425) ([7cae4be](https://github.com/roflmuffin/CounterStrikeSharp/commit/7cae4be96d72b2ab3b48ffac28e29956759a2583))

## What's Changed in v1.0.214
* Make shared type loader less strict by [@KillStr3aK](https://github.com/KillStr3aK) in [#424](https://github.com/roflmuffin/CounterStrikeSharp/pull/424) ([83bc1a9](https://github.com/roflmuffin/CounterStrikeSharp/commit/83bc1a95fb7c781c7fa3ca7e707980546df9e27a))
* add windows install script ([71c694b](https://github.com/roflmuffin/CounterStrikeSharp/commit/71c694b52ea32fe60a0972b9c775abb09fb52e96))
* feat: add install script ([a452d79](https://github.com/roflmuffin/CounterStrikeSharp/commit/a452d79ba31d70e56a57d3db545a9ce4fa8ba6d6))

## What's Changed in v1.0.213
* Make signature support IDA/x64Dbg style by [@Nukoooo](https://github.com/Nukoooo) in [#407](https://github.com/roflmuffin/CounterStrikeSharp/pull/407) ([dfc9859](https://github.com/roflmuffin/CounterStrikeSharp/commit/dfc98598065e36b0f700545ad693aab53ceed533))

## What's Changed in v1.0.212
* Add mising ``GameData.GetSignature`` to ``NetworkStateChangedFunc`` by [@Nukoooo](https://github.com/Nukoooo) in [#415](https://github.com/roflmuffin/CounterStrikeSharp/pull/415) ([f99f584](https://github.com/roflmuffin/CounterStrikeSharp/commit/f99f58402a438dd3d30b70ca26c215cf86373278))
* Update .clang-format & add formatting tool install scripts in [#410](https://github.com/roflmuffin/CounterStrikeSharp/pull/410) ([6317559](https://github.com/roflmuffin/CounterStrikeSharp/commit/6317559bd22d34af99b70f7ee67724b6d9f79454))

## New Contributors
* [@Nukoooo](https://github.com/Nukoooo) made their first contribution in [#415](https://github.com/roflmuffin/CounterStrikeSharp/pull/415)

## What's Changed in v1.0.211
* chore: bump hl2sdk in [#408](https://github.com/roflmuffin/CounterStrikeSharp/pull/408) ([ad6e1ca](https://github.com/roflmuffin/CounterStrikeSharp/commit/ad6e1ca2e2fe0ba65d231569187779436cdb233c))

## What's Changed in v1.0.210
* fix: fix startup server listener ([2564ef9](https://github.com/roflmuffin/CounterStrikeSharp/commit/2564ef9f39a30051a73188689f91255a5dd25a79))

## What's Changed in v1.0.209
* feat: expose TargetTypeMap ([83a341d](https://github.com/roflmuffin/CounterStrikeSharp/commit/83a341d3cfbb677e646e0675f3be88bda0221c41))

## What's Changed in v1.0.208
* fix: bad commit ([534fc42](https://github.com/roflmuffin/CounterStrikeSharp/commit/534fc42444aae0d2354d8e116d3cd1109ad456a4))
* docs: add more comments to base plugin ([41355d0](https://github.com/roflmuffin/CounterStrikeSharp/commit/41355d05fa747a5688a2b9a58de0733cd004c2c6))

## What's Changed in v1.0.206
* Add teleport overloads by [@partiusfabaa](https://github.com/partiusfabaa) in [#399](https://github.com/roflmuffin/CounterStrikeSharp/pull/399) ([d9da15b](https://github.com/roflmuffin/CounterStrikeSharp/commit/d9da15be83dc37b869db75082c6820766ccb6071))
* Rename ACKNOWLEDGEMENTS to ACKNOWLEDGEMENTS.md by [@B3none](https://github.com/B3none) in [#401](https://github.com/roflmuffin/CounterStrikeSharp/pull/401) ([75e2f6e](https://github.com/roflmuffin/CounterStrikeSharp/commit/75e2f6e8aaa2bbb81b2323a3f2c703ab7ce41e8e))
* add basic contributing guide ([37b34e1](https://github.com/roflmuffin/CounterStrikeSharp/commit/37b34e1d416b65f7b3163265da04e6bee4ff4939))

## What's Changed in v1.0.205
* Add CenterHtmlMenu button colors by [@WidovV](https://github.com/WidovV) in [#398](https://github.com/roflmuffin/CounterStrikeSharp/pull/398) ([5ce0464](https://github.com/roflmuffin/CounterStrikeSharp/commit/5ce04649fd973e761632d3748d60520da7e38b77))
* Update README.md ([7b7202f](https://github.com/roflmuffin/CounterStrikeSharp/commit/7b7202fe8a10ed878e1de7d78cfa80996a0869f8))

## New Contributors
* [@WidovV](https://github.com/WidovV) made their first contribution in [#398](https://github.com/roflmuffin/CounterStrikeSharp/pull/398)

## What's Changed in v1.0.204
* fix: mark center html menu constructor as obsolete, throw error in [#396](https://github.com/roflmuffin/CounterStrikeSharp/pull/396) ([cadb817](https://github.com/roflmuffin/CounterStrikeSharp/commit/cadb817ed2aee2cd6f1d7209d17c7e32987d0f76))

## What's Changed in v1.0.203
* added `Open` method to each menu type by [@partiusfabaa](https://github.com/partiusfabaa) in [#385](https://github.com/roflmuffin/CounterStrikeSharp/pull/385) ([211516c](https://github.com/roflmuffin/CounterStrikeSharp/commit/211516cce5b830aaba31ab361ad4d9798ac7b24a))
* [skip ci] chore: update ApiCompat to v202, disable suppression file ([ab211a4](https://github.com/roflmuffin/CounterStrikeSharp/commit/ab211a42e699eb3c01f5a23880161926c2663242))

## What's Changed in v1.0.202
* fix: bad vector math ([696ecad](https://github.com/roflmuffin/CounterStrikeSharp/commit/696ecadee4002e16c58d589c40ea39ef003c4d34))

## What's Changed in v1.0.201
* fix: main version number ([e4d598d](https://github.com/roflmuffin/CounterStrikeSharp/commit/e4d598dba8b1c93ec36bee517d1e329458776984))
* feat: improve version stamping in [#389](https://github.com/roflmuffin/CounterStrikeSharp/pull/389) ([5c67d88](https://github.com/roflmuffin/CounterStrikeSharp/commit/5c67d888445f3cd7215a87240bb611f237668e64))

## What's Changed in v1.0.199
* .NET8 Upgrade in [#261](https://github.com/roflmuffin/CounterStrikeSharp/pull/261) ([9d8b6be](https://github.com/roflmuffin/CounterStrikeSharp/commit/9d8b6beae6a89f3fc4addb73c0d0135d27cb4cee))

## What's Changed in v1.0.198
* Fix backwards compatibility in DynamicHook by [@ipsvn](https://github.com/ipsvn) in [#388](https://github.com/roflmuffin/CounterStrikeSharp/pull/388) ([39604b7](https://github.com/roflmuffin/CounterStrikeSharp/commit/39604b7ad7a8547dce8f79bff854e7460514614c))

## New Contributors
* [@ipsvn](https://github.com/ipsvn) made their first contribution in [#388](https://github.com/roflmuffin/CounterStrikeSharp/pull/388)

## What's Changed in v1.0.197
* minor menu changes by [@partiusfabaa](https://github.com/partiusfabaa) in [#373](https://github.com/roflmuffin/CounterStrikeSharp/pull/373) ([1b1f1d0](https://github.com/roflmuffin/CounterStrikeSharp/commit/1b1f1d04dd288e051c27b21c01a5d2312fe7388e))
* Add `RemoveAll` method to `NetworkedVector` class by [@ianlucas](https://github.com/ianlucas) in [#380](https://github.com/roflmuffin/CounterStrikeSharp/pull/380) ([dbc348c](https://github.com/roflmuffin/CounterStrikeSharp/commit/dbc348c1bf96d3069a110e188215857ca8603478))

## New Contributors
* [@ianlucas](https://github.com/ianlucas) made their first contribution in [#380](https://github.com/roflmuffin/CounterStrikeSharp/pull/380)

## What's Changed in v1.0.195
* fix: update collision groups in [#376](https://github.com/roflmuffin/CounterStrikeSharp/pull/376) ([d295589](https://github.com/roflmuffin/CounterStrikeSharp/commit/d295589c44d1fac10f34e7ef3b319c84c2683351))

## What's Changed in v1.0.194
* feat: add `Server.RunOnTick` method which allows tick scheduling in [#374](https://github.com/roflmuffin/CounterStrikeSharp/pull/374) ([16767fd](https://github.com/roflmuffin/CounterStrikeSharp/commit/16767fd49438cab8ee42df87c6370e5cc281e928))

## What's Changed in v1.0.193
* fix: disable autoload plugin but allow shared library loading ([36a97bf](https://github.com/roflmuffin/CounterStrikeSharp/commit/36a97bfffda9c0077f196e0ddcb5c5716281137c))

## What's Changed in v1.0.192
* feat: add `PluginAutoLoadEnabled` config option ([178f747](https://github.com/roflmuffin/CounterStrikeSharp/commit/178f7472c66bd0ee5ebae53dbd25eef15e9d13cf))

## What's Changed in v1.0.191
* remove unused arguments by [@partiusfabaa](https://github.com/partiusfabaa) in [#334](https://github.com/roflmuffin/CounterStrikeSharp/pull/334) ([cba5144](https://github.com/roflmuffin/CounterStrikeSharp/commit/cba5144bbf513ea04ac1eb8ea4582252fe9e6b2b))

## New Contributors
* [@partiusfabaa](https://github.com/partiusfabaa) made their first contribution in [#334](https://github.com/roflmuffin/CounterStrikeSharp/pull/334)

## What's Changed in v1.0.190
* fix: allows game events to be freed, frees event in print to center html ([0de951c](https://github.com/roflmuffin/CounterStrikeSharp/commit/0de951cb6f3150d78ede0003957096583fc4e869))

## What's Changed in v1.0.189
* feat: use concurrent queue for next frame & world update tasks in [#365](https://github.com/roflmuffin/CounterStrikeSharp/pull/365) ([c3d44a8](https://github.com/roflmuffin/CounterStrikeSharp/commit/c3d44a87bc06afa5d569cd61d25f06b46fedddb2))
* Update minimum api version for shared api docs by [@B3none](https://github.com/B3none) in [#364](https://github.com/roflmuffin/CounterStrikeSharp/pull/364) ([bd3c0c7](https://github.com/roflmuffin/CounterStrikeSharp/commit/bd3c0c76e3a797e4f7379c6abc075f25caa46e81))

## What's Changed in v1.0.188
* Fix TerminateRound params by [@Yarukon](https://github.com/Yarukon) in [#363](https://github.com/roflmuffin/CounterStrikeSharp/pull/363) ([656c0e3](https://github.com/roflmuffin/CounterStrikeSharp/commit/656c0e3a840e47de6aa3747ffb3b2bffb90d66d1))

## What's Changed in v1.0.187
* fix: add error handling to `OnAllPluginsLoaded` ([40c8421](https://github.com/roflmuffin/CounterStrikeSharp/commit/40c842149c3f3db473b18be2220ee209dc020683))

## What's Changed in v1.0.186
* chore: remove erroneous log ([64d1c0a](https://github.com/roflmuffin/CounterStrikeSharp/commit/64d1c0a9f4da7aff090a9a86bc5bead672535cc1))

## What's Changed in v1.0.185
* fix: use concurrent dictionary for function reference ([a6de51c](https://github.com/roflmuffin/CounterStrikeSharp/commit/a6de51c444e791020a1f088c49b4adff24d736b1))

## What's Changed in v1.0.184
* feat: add assembly name lazy loading of shared libraries ([2535ac0](https://github.com/roflmuffin/CounterStrikeSharp/commit/2535ac057518006a874da4105321e5c9e1938b37))

## What's Changed in v1.0.183
* chore: migrate to protobufs submodule in [#362](https://github.com/roflmuffin/CounterStrikeSharp/pull/362) ([bc61323](https://github.com/roflmuffin/CounterStrikeSharp/commit/bc61323315eb235976b77fbd848b0cc5371f484a))

## What's Changed in v1.0.182
* feat: update game events dump from Feb 14 update ([241817b](https://github.com/roflmuffin/CounterStrikeSharp/commit/241817b7f2b70fa3423d9e79d309fa9fb3c883bb))

## What's Changed in v1.0.181
* fix: allow empty overrides to skip checks by [@busheezy](https://github.com/busheezy) in [#357](https://github.com/roflmuffin/CounterStrikeSharp/pull/357) ([fbcdce3](https://github.com/roflmuffin/CounterStrikeSharp/commit/fbcdce34fc0efdebfb36f5af3eb25ac3bbe1ae3f))

## What's Changed in v1.0.180
* Shared Plugin APIs/Capabilities in [#253](https://github.com/roflmuffin/CounterStrikeSharp/pull/253) ([daf0d25](https://github.com/roflmuffin/CounterStrikeSharp/commit/daf0d25f36db2c2705edb5a6d9816f2c9786339c))

## What's Changed in v1.0.179
* Adding the proper way to do resource precache by [@Yarukon](https://github.com/Yarukon) in [#358](https://github.com/roflmuffin/CounterStrikeSharp/pull/358) ([12485be](https://github.com/roflmuffin/CounterStrikeSharp/commit/12485be29f8c779dbe9dbdcf74d968d8dea426bd))

## New Contributors
* [@Yarukon](https://github.com/Yarukon) made their first contribution in [#358](https://github.com/roflmuffin/CounterStrikeSharp/pull/358)

## What's Changed in v1.0.178
* fix: update gamedata ([983d914](https://github.com/roflmuffin/CounterStrikeSharp/commit/983d91491d1fbef120cb6be50d0b52e3dc472aa1))

## What's Changed in v1.0.177
* fix: allow using an empty flag array in overrides by [@busheezy](https://github.com/busheezy) in [#351](https://github.com/roflmuffin/CounterStrikeSharp/pull/351) ([71507b1](https://github.com/roflmuffin/CounterStrikeSharp/commit/71507b1e25eefd3fa738e847a2e9022a9c9ba772))
* Bump the min api version in the fake convars example plugin by [@B3none](https://github.com/B3none) in [#350](https://github.com/roflmuffin/CounterStrikeSharp/pull/350) ([cfe14b3](https://github.com/roflmuffin/CounterStrikeSharp/commit/cfe14b35fecaffcf335d7dc9c6660e184be088d4))

## What's Changed in v1.0.176
* feat: improve entity validation by [@Poggicek](https://github.com/Poggicek) in [#348](https://github.com/roflmuffin/CounterStrikeSharp/pull/348) ([5a6cdf0](https://github.com/roflmuffin/CounterStrikeSharp/commit/5a6cdf0da336a013c3558f8ed62baa637c59aa50))

## What's Changed in v1.0.175
* feat: add `FakeConVar` class in [#325](https://github.com/roflmuffin/CounterStrikeSharp/pull/325) ([a5399dd](https://github.com/roflmuffin/CounterStrikeSharp/commit/a5399dd5fe0c2354cb0b1c9f589d800ac65e4047))

## What's Changed in v1.0.174
* fix: use function reference for next frame tasks ([ab996c3](https://github.com/roflmuffin/CounterStrikeSharp/commit/ab996c34e9da7362fbe406b56abe0d52a1707915))

## What's Changed in v1.0.173
* Re-implemented css_lang command by [@B3none](https://github.com/B3none) in [#343](https://github.com/roflmuffin/CounterStrikeSharp/pull/343) ([6e2e25b](https://github.com/roflmuffin/CounterStrikeSharp/commit/6e2e25b96e23d0d84713eb9ea900c464cb65d2d9))
* remove `untriaged` label when milestoning issues ([1142c9f](https://github.com/roflmuffin/CounterStrikeSharp/commit/1142c9f063d9d5c0ddbe49ead78943f912244974))

## What's Changed in v1.0.172
* feat: add chat color method for player controller in [#339](https://github.com/roflmuffin/CounterStrikeSharp/pull/339) ([72178c9](https://github.com/roflmuffin/CounterStrikeSharp/commit/72178c95980aacfea8b2128930f5ae9321db3d58))

## What's Changed in v1.0.171
* fix: handle byte array schema properties correctly in [#338](https://github.com/roflmuffin/CounterStrikeSharp/pull/338) ([7617bd1](https://github.com/roflmuffin/CounterStrikeSharp/commit/7617bd15566a89b93529f0424125fe937cd6e048))

## What's Changed in v1.0.170
* feat: add CommandCallingContext to commands in [#337](https://github.com/roflmuffin/CounterStrikeSharp/pull/337) ([501d51a](https://github.com/roflmuffin/CounterStrikeSharp/commit/501d51a6686860b5554e3bd5e4521630e3a0f06a))

## What's Changed in v1.0.169
* chore: update hl2sdk in [#336](https://github.com/roflmuffin/CounterStrikeSharp/pull/336) ([87f48cb](https://github.com/roflmuffin/CounterStrikeSharp/commit/87f48cb35cc947796b06a47600f002123471206c))
* Create FUNDING.yml 🥰 ([39aa430](https://github.com/roflmuffin/CounterStrikeSharp/commit/39aa4305289d591aa21131e3fcba699269f989c4))
* remove untriaged when author action label applied ([a404a4d](https://github.com/roflmuffin/CounterStrikeSharp/commit/a404a4d9d536291297b83592331f354f2caf5048))
* add basic issue management workflows ([62ba298](https://github.com/roflmuffin/CounterStrikeSharp/commit/62ba29891d3060edec9b785e91209ae9f9bfd6d5))

## What's Changed in v1.0.168
* Command Overhaul in [#330](https://github.com/roflmuffin/CounterStrikeSharp/pull/330) ([48fa9ca](https://github.com/roflmuffin/CounterStrikeSharp/commit/48fa9ca0768f7b7300ce0d7a66227d1e9e5517c7))

## What's Changed in v1.0.167
* feat: add `ExecuteClientCommandFromServer` native ([607e6c6](https://github.com/roflmuffin/CounterStrikeSharp/commit/607e6c61f53aa40e8b97a50d3fdec58464d46a6a))
* Update LICENSE ([2675d28](https://github.com/roflmuffin/CounterStrikeSharp/commit/2675d280e37fab4e7ff55337531cd8f9dcd3de4d))

## What's Changed in v1.0.166
* Update .gitmodules to use https references rather than git reference by [@B3none](https://github.com/B3none) in [#297](https://github.com/roflmuffin/CounterStrikeSharp/pull/297) ([f336dec](https://github.com/roflmuffin/CounterStrikeSharp/commit/f336dec61611f2a76b5b48374529f445220415c5))

## What's Changed in v1.0.165
* Dev -> Main (Schema Updates) in [#320](https://github.com/roflmuffin/CounterStrikeSharp/pull/320) ([2d8f7be](https://github.com/roflmuffin/CounterStrikeSharp/commit/2d8f7be84f0c803e3860ea1e792963bb706e0bad))

## What's Changed in v1.0.164
* fix: update gamedata (thanks to xLeviNx) ([8967c40](https://github.com/roflmuffin/CounterStrikeSharp/commit/8967c40bedee786c46c88a78cb618884b166191e))

## What's Changed in v1.0.163
* fix linux::CCSPlayerPawnBase_PostThink and windows::CEntityIOOutput_FireOutputInternal by [@KillStr3aK](https://github.com/KillStr3aK) in [#314](https://github.com/roflmuffin/CounterStrikeSharp/pull/314) ([12c6f4d](https://github.com/roflmuffin/CounterStrikeSharp/commit/12c6f4d0a1e03ce798d16e5c404d71d25283e140))

## What's Changed in v1.0.162
* Update gamedata for Call To Arms Update by [@zonical](https://github.com/zonical) in [#311](https://github.com/roflmuffin/CounterStrikeSharp/pull/311) ([f2b8044](https://github.com/roflmuffin/CounterStrikeSharp/commit/f2b8044b8c7b6973c3facf05b21a42f261973cf0))

## What's Changed in v1.0.161
* [skip ci] Update README.md ([eb9f566](https://github.com/roflmuffin/CounterStrikeSharp/commit/eb9f5667d8d11f94ffd862d82615948c2354b7fe))
* fix: default base menu post select action to reset ([b9ca63a](https://github.com/roflmuffin/CounterStrikeSharp/commit/b9ca63a60375dc6c75298df4ae0b1d3aacd804bf))

## What's Changed in v1.0.160
* Improved menu closing by [@B3none](https://github.com/B3none) in [#294](https://github.com/roflmuffin/CounterStrikeSharp/pull/294) ([8fc926e](https://github.com/roflmuffin/CounterStrikeSharp/commit/8fc926eacf8238e063763b11fca3a2513d7b421b))

## What's Changed in v1.0.159
* Updated css_plugins sub command responses. by [@B3none](https://github.com/B3none) in [#289](https://github.com/roflmuffin/CounterStrikeSharp/pull/289) ([5695c3f](https://github.com/roflmuffin/CounterStrikeSharp/commit/5695c3f922c7e42fc8b9525c108ebf0c15219926))

## What's Changed in v1.0.158
* Tidy CCSPlayerController by [@B3none](https://github.com/B3none) in [#287](https://github.com/roflmuffin/CounterStrikeSharp/pull/287) ([9071d51](https://github.com/roflmuffin/CounterStrikeSharp/commit/9071d51ecdaed9e6c2987a90f12ae46af6e4953f))

## What's Changed in v1.0.157
* fix: move discord notify into release pipeline ([0a32962](https://github.com/roflmuffin/CounterStrikeSharp/commit/0a32962f4ab2532ec88bfba1a65e6514791e53b1))

## What's Changed in v1.0.156
* Update discord-notify.yml ([271705b](https://github.com/roflmuffin/CounterStrikeSharp/commit/271705b37792f3a75b0f6862ddce1c116e2a64e3))

## What's Changed in v1.0.155
* Update discord-notify.yml ([cdcddbb](https://github.com/roflmuffin/CounterStrikeSharp/commit/cdcddbb5f30365504608ca03d36e068c6d27bb98))
* Update discord-notify.yml ([91f51d0](https://github.com/roflmuffin/CounterStrikeSharp/commit/91f51d0c5ccc9e7907cf1d810995812bf811181c))

## What's Changed in v1.0.154
* HTML Menu improvements by [@ValMadBox](https://github.com/ValMadBox) in [#284](https://github.com/roflmuffin/CounterStrikeSharp/pull/284) ([e97f804](https://github.com/roflmuffin/CounterStrikeSharp/commit/e97f8042945fdb29e3f54295e365c60daa00e868))

## What's Changed in v1.0.153
* Added canUse virtual method by [@ValMadBox](https://github.com/ValMadBox) in [#282](https://github.com/roflmuffin/CounterStrikeSharp/pull/282) ([4f805b1](https://github.com/roflmuffin/CounterStrikeSharp/commit/4f805b18e2af2c04f1848d38d81a5d1834813d74))

## New Contributors
* [@ValMadBox](https://github.com/ValMadBox) made their first contribution in [#282](https://github.com/roflmuffin/CounterStrikeSharp/pull/282)

## What's Changed in v1.0.152
* chore: update API compatibility version to 151 ([e1f9b56](https://github.com/roflmuffin/CounterStrikeSharp/commit/e1f9b5635eb21a7e2e31b1783b1b676719f88593))

## What's Changed in v1.0.151
* feat: add discord notify through GH actions ([59bff4f](https://github.com/roflmuffin/CounterStrikeSharp/commit/59bff4f500dfaccacb0b53584bf678c005a87598))

## What's Changed in v1.0.150
* Log exception if plugin load fails using the `load` command by [@wiesendaniel](https://github.com/wiesendaniel) in [#279](https://github.com/roflmuffin/CounterStrikeSharp/pull/279) ([a2581d8](https://github.com/roflmuffin/CounterStrikeSharp/commit/a2581d8e9116e3ab505029720719a82e4dd2fac5))
* Change TerroristsPlanned to TerroristsPlanted in RoundEndReason by [@Ravid-A](https://github.com/Ravid-A) ([e7d190a](https://github.com/roflmuffin/CounterStrikeSharp/commit/e7d190a6f74f9ccbf30e16f8a6a92b36e037e9a4))
* Menu system updates by [@B3none](https://github.com/B3none) ([5513d57](https://github.com/roflmuffin/CounterStrikeSharp/commit/5513d5710a20195922e725c9401ae1cb1291b3e8))
* fix(Offsets/Win): CCSPlayer_ItemServices.RemoveWeapons() by [@M1kep](https://github.com/M1kep) ([e5c2236](https://github.com/roflmuffin/CounterStrikeSharp/commit/e5c223699ccb300558788f86850a8e478e893156))
* Admin manager improvements by [@zonical](https://github.com/zonical) ([fa37c22](https://github.com/roflmuffin/CounterStrikeSharp/commit/fa37c222d9d8598a67cb2433c36b46a941813b14))

## New Contributors
* [@wiesendaniel](https://github.com/wiesendaniel) made their first contribution in [#279](https://github.com/roflmuffin/CounterStrikeSharp/pull/279)
* [@Ravid-A](https://github.com/Ravid-A) made their first contribution
* [@M1kep](https://github.com/M1kep) made their first contribution

## What's Changed in v1.0.149
* Create CODEOWNERS ([3b633fa](https://github.com/roflmuffin/CounterStrikeSharp/commit/3b633fafc7a1373fab8cc6e89f83953a88b4d20c))

## What's Changed in v1.0.148
* Fix css_plugins commands number of args check by [@abnerfs](https://github.com/abnerfs) in [#269](https://github.com/roflmuffin/CounterStrikeSharp/pull/269) ([765c56a](https://github.com/roflmuffin/CounterStrikeSharp/commit/765c56a38af0f0e5a7a8f4fcb9786cb56461c8bc))

## What's Changed in v1.0.147
* Add casted property .Team to CCSPlayerController by [@B3none](https://github.com/B3none) in [#259](https://github.com/roflmuffin/CounterStrikeSharp/pull/259) ([204850f](https://github.com/roflmuffin/CounterStrikeSharp/commit/204850fb55ef39143a6a2802b1b57fa5c45372ec))

## What's Changed in v1.0.144
* Purge disabled folder by [@zonical](https://github.com/zonical) in [#256](https://github.com/roflmuffin/CounterStrikeSharp/pull/256) ([bac31b9](https://github.com/roflmuffin/CounterStrikeSharp/commit/bac31b9190edcb2ab0e9ffdde93ddcb69ee50a8a))

## What's Changed in v1.0.143
* Add DarkRed to ChatColors class to follow naming convention by [@B3none](https://github.com/B3none) in [#245](https://github.com/roflmuffin/CounterStrikeSharp/pull/245) ([289f95a](https://github.com/roflmuffin/CounterStrikeSharp/commit/289f95a6b7e8fee693c8ca4992dd8d6993dc5e00))

## What's Changed in v1.0.142
* chore: remove `schema::GetOffset` warning message ([7b45a88](https://github.com/roflmuffin/CounterStrikeSharp/commit/7b45a884d461c362f21f6aa101515a3757402795))

## What's Changed in v1.0.141
* feat: add state changed and network state changed handler in [#229](https://github.com/roflmuffin/CounterStrikeSharp/pull/229) ([6ea6d0a](https://github.com/roflmuffin/CounterStrikeSharp/commit/6ea6d0a22de2cdde496910d59dbc1f215e25ea0e))

## What's Changed in v1.0.140
* fix: bad max player count ([1252345](https://github.com/roflmuffin/CounterStrikeSharp/commit/12523455c02736b605e5055b78536acf6a345c6d))

## What's Changed in v1.0.139
* feat: add `AcceptInput` method to `CEntityInstance` in [#228](https://github.com/roflmuffin/CounterStrikeSharp/pull/228) ([db63fdc](https://github.com/roflmuffin/CounterStrikeSharp/commit/db63fdc00c0050b7f83bcd09d0a872442ce66f94))

## What's Changed in v1.0.138
* fix: update `GetPlayers` to use slot access ([57747f2](https://github.com/roflmuffin/CounterStrikeSharp/commit/57747f2e1c5cc9cffc58fe89d51ffe79875644c4))

## What's Changed in v1.0.137
* feat: add GetMaxClients native, fixes #184 ([66b5f77](https://github.com/roflmuffin/CounterStrikeSharp/commit/66b5f77a2d06a37c7b377b2380e1001226dbd892))

## What's Changed in v1.0.136
* chore: update hl2sdk in [#227](https://github.com/roflmuffin/CounterStrikeSharp/pull/227) ([8dbcb6d](https://github.com/roflmuffin/CounterStrikeSharp/commit/8dbcb6d53147fa8d5f33299dfb52138eeb8e6e18))

## What's Changed in v1.0.135
* chore: disable compat suppression file by default ([2f0d34b](https://github.com/roflmuffin/CounterStrikeSharp/commit/2f0d34b27195800a1340bd8da2a181f589e7dd52))

## What's Changed in v1.0.134
* feat: add ApiCompat checker to determine breaking API changes ([2a59544](https://github.com/roflmuffin/CounterStrikeSharp/commit/2a59544fbce11df792e362be473c8944c8beffb3))
* Fix getting started image by [@pedrotski](https://github.com/pedrotski) in [#220](https://github.com/roflmuffin/CounterStrikeSharp/pull/220) ([f80f2ae](https://github.com/roflmuffin/CounterStrikeSharp/commit/f80f2ae9495c037c3e4030b9b4b82638e02dd25e))

## What's Changed in v1.0.133
* fix: ignore null designer names in FindAllEntitiesByDesignerNameFixes #212 ([f68a0ab](https://github.com/roflmuffin/CounterStrikeSharp/commit/f68a0abc6115d748b5401ac08b67b1a2ed769ffd))

## What's Changed in v1.0.132
* Fix CanPlayerTarget Immu by [@srtnlgn](https://github.com/srtnlgn) in [#222](https://github.com/roflmuffin/CounterStrikeSharp/pull/222) ([a07dd9d](https://github.com/roflmuffin/CounterStrikeSharp/commit/a07dd9d7d4d51341f58441730fa4e147c2a7feda))

## New Contributors
* [@srtnlgn](https://github.com/srtnlgn) made their first contribution in [#222](https://github.com/roflmuffin/CounterStrikeSharp/pull/222)

## What's Changed in v1.0.131
* Added a parameter for people to optionally remove the entity when calling RemoveItemByDesignerName by [@B3none](https://github.com/B3none) in [#214](https://github.com/roflmuffin/CounterStrikeSharp/pull/214) ([d527038](https://github.com/roflmuffin/CounterStrikeSharp/commit/d527038fba6543b065f4d947fa7f21537ce8d592))
* Update Getting Started Guide by [@pedrotski](https://github.com/pedrotski) in [#217](https://github.com/roflmuffin/CounterStrikeSharp/pull/217) ([ca85922](https://github.com/roflmuffin/CounterStrikeSharp/commit/ca8592227075c8050b2b4181bd8bd340aa8e8e44))

## What's Changed in v1.0.130
* Added links to referenced projects in the credits for the README.md by [@B3none](https://github.com/B3none) in [#210](https://github.com/roflmuffin/CounterStrikeSharp/pull/210) ([b837479](https://github.com/roflmuffin/CounterStrikeSharp/commit/b837479f98bd62fae415327bf3c433b0166978ab))
* Docs: Using DOTNET_SYSTEM_GLOBALIZATION_INVARIANT is no longer valid. by [@Hackmastr](https://github.com/Hackmastr) in [#209](https://github.com/roflmuffin/CounterStrikeSharp/pull/209) ([1e42f72](https://github.com/roflmuffin/CounterStrikeSharp/commit/1e42f72655c821a51a408f9241246e030f31a577))

## New Contributors
* [@B3none](https://github.com/B3none) made their first contribution in [#210](https://github.com/roflmuffin/CounterStrikeSharp/pull/210)

## What's Changed in v1.0.129
* feat: Add `SchemaMember` attribute to schema objects` in [#208](https://github.com/roflmuffin/CounterStrikeSharp/pull/208) ([1ad1828](https://github.com/roflmuffin/CounterStrikeSharp/commit/1ad1828e30d28c82999ef709efb04e8305bf4e86))

## What's Changed in v1.0.128
* feat: Added RemovePlayerItem() to CBasePlayerPawn. by [@CharlesBarone](https://github.com/CharlesBarone) in [#200](https://github.com/roflmuffin/CounterStrikeSharp/pull/200) ([563a5d7](https://github.com/roflmuffin/CounterStrikeSharp/commit/563a5d7b3a1c41c8c24208e2df66f12e34e7048e))
* feat: Add OnClientVoice listener by [@charliethomson](https://github.com/charliethomson) in [#204](https://github.com/roflmuffin/CounterStrikeSharp/pull/204) ([983b673](https://github.com/roflmuffin/CounterStrikeSharp/commit/983b673b4c02b36fcd9cb7b527cb3e035d2624d4))

## New Contributors
* [@charliethomson](https://github.com/charliethomson) made their first contribution in [#204](https://github.com/roflmuffin/CounterStrikeSharp/pull/204)

## What's Changed in v1.0.126
* fix: offsets for `CBaseEntity` derived classes ([74fd0e0](https://github.com/roflmuffin/CounterStrikeSharp/commit/74fd0e0832dbd8a8a2385bc76f0c5f585196b452))

## What's Changed in v1.0.125
* chore: license updates in [#199](https://github.com/roflmuffin/CounterStrikeSharp/pull/199) ([44e3f22](https://github.com/roflmuffin/CounterStrikeSharp/commit/44e3f2240c4c00ca70706b5e3bb37478fb66f0e4))
* CHANGE: visual adjustments by [@busheezy](https://github.com/busheezy) in [#198](https://github.com/roflmuffin/CounterStrikeSharp/pull/198) ([8af219e](https://github.com/roflmuffin/CounterStrikeSharp/commit/8af219e7a8fffef1232bbc59160d20baedd39820))

## What's Changed in v1.0.124
* Add voice manager (ability to override voice chat / mute players) by [@Poggicek](https://github.com/Poggicek) in [#179](https://github.com/roflmuffin/CounterStrikeSharp/pull/179) ([bff04e7](https://github.com/roflmuffin/CounterStrikeSharp/commit/bff04e77959a51f01fe5cd5ba4772b450b2cdd8b))

## What's Changed in v1.0.123
* chore: bump version ([d495ac6](https://github.com/roflmuffin/CounterStrikeSharp/commit/d495ac62307eb21792f96d3475f0f74273726335))
* fix: fallback to `en` language files for invariant mode ([f78abf0](https://github.com/roflmuffin/CounterStrikeSharp/commit/f78abf0c81419c00e94bd372b04245121e8d662b))

## What's Changed in v1.0.122
* feat: Added support for custom gamedata files. by [@CharlesBarone](https://github.com/CharlesBarone) in [#194](https://github.com/roflmuffin/CounterStrikeSharp/pull/194) ([bcacc42](https://github.com/roflmuffin/CounterStrikeSharp/commit/bcacc42d0e4a12bb760560ad0ac106e241c1af96))

## What's Changed in v1.0.121
* Update docs link in README.md by [@HerrMagiic](https://github.com/HerrMagiic) in [#193](https://github.com/roflmuffin/CounterStrikeSharp/pull/193) ([8235d5e](https://github.com/roflmuffin/CounterStrikeSharp/commit/8235d5e72808e8edde44fa04a657a224acf21190))

## New Contributors
* [@HerrMagiic](https://github.com/HerrMagiic) made their first contribution in [#193](https://github.com/roflmuffin/CounterStrikeSharp/pull/193)

## What's Changed in v1.0.120
* Plugin Translations in [#146](https://github.com/roflmuffin/CounterStrikeSharp/pull/146) ([5673935](https://github.com/roflmuffin/CounterStrikeSharp/commit/56739356d5aea39e8ae264cc698525b83f73e204))

## What's Changed in v1.0.119
* Admin improvements round 2! (this time it's personal) by [@zonical](https://github.com/zonical) in [#143](https://github.com/roflmuffin/CounterStrikeSharp/pull/143) ([aaba875](https://github.com/roflmuffin/CounterStrikeSharp/commit/aaba87551d998a3f5ce6dba1deb245bd350721de))

## What's Changed in v1.0.118
* chore: cleanup null reference warnings in virtual funcs ([a3466dd](https://github.com/roflmuffin/CounterStrikeSharp/commit/a3466dd5d119b11fbdb3c47ef32c5361ba765a1c))
* Update docs by [@johnoclockdk](https://github.com/johnoclockdk) in [#176](https://github.com/roflmuffin/CounterStrikeSharp/pull/176) ([c860476](https://github.com/roflmuffin/CounterStrikeSharp/commit/c8604760f27075d7bdf69fc4b5c5dfd046efeeee))

## What's Changed in v1.0.117
* feat: defers calls to `HookEvent` until after game loop mode init in [#187](https://github.com/roflmuffin/CounterStrikeSharp/pull/187) ([d50a945](https://github.com/roflmuffin/CounterStrikeSharp/commit/d50a945317dcaf63add1b4f22d9a5d0e788a8354))

## What's Changed in v1.0.116
* fix: discord links in [#190](https://github.com/roflmuffin/CounterStrikeSharp/pull/190) ([55396e0](https://github.com/roflmuffin/CounterStrikeSharp/commit/55396e005c1be2b2421b655ca3ac9835f4c84846))

## What's Changed in v1.0.115
* Merge branch 'FixSteamIdOnWindowsServer' into main ([98b2b01](https://github.com/roflmuffin/CounterStrikeSharp/commit/98b2b01992ce99054a5aac6dc5f2bafe818f2792))
* tests: update tests, throw out of range exception <= 0 ([a537be8](https://github.com/roflmuffin/CounterStrikeSharp/commit/a537be89e41e05a6e76b0aefc31d5212b248eb0c))
* Merge remote-tracking branch 'origin/main' into FixSteamIdOnWindowsServer in [#185](https://github.com/roflmuffin/CounterStrikeSharp/pull/185) ([c07d5d2](https://github.com/roflmuffin/CounterStrikeSharp/commit/c07d5d2aa95ffddbb12e3f6869daab1a6bd66262))

## What's Changed in v1.0.114
* feat: add basic tests project with SteamID tests in [#186](https://github.com/roflmuffin/CounterStrikeSharp/pull/186) ([1cc9555](https://github.com/roflmuffin/CounterStrikeSharp/commit/1cc95555feda6c6e4a1e7285a7288a2ff775defb))
* chore: bump hl2sdk version ([378c28d](https://github.com/roflmuffin/CounterStrikeSharp/commit/378c28dfd0b235cdf14acecdd9ced0b8763a0500))
* Fix SteamId on Windows Server #182 by [@TheR00st3r](https://github.com/TheR00st3r) ([c7343c3](https://github.com/roflmuffin/CounterStrikeSharp/commit/c7343c3b7a511ecaefac181ab0c9d6f273ac630e))

## New Contributors
* [@TheR00st3r](https://github.com/TheR00st3r) made their first contribution

## What's Changed in v1.0.112
* Add VData Access in [#181](https://github.com/roflmuffin/CounterStrikeSharp/pull/181) ([62f6b09](https://github.com/roflmuffin/CounterStrikeSharp/commit/62f6b09f50aff80007f5496c0383aa59faf687c4))

## What's Changed in v1.0.111
* Add Entity Output Hooks in [#174](https://github.com/roflmuffin/CounterStrikeSharp/pull/174) ([2a15a8d](https://github.com/roflmuffin/CounterStrikeSharp/commit/2a15a8de71cec166ad4bbfeccc9518588e2bfc43))
* docs: win32 related by [@laper32](https://github.com/laper32) in [#177](https://github.com/roflmuffin/CounterStrikeSharp/pull/177) ([1d6bee0](https://github.com/roflmuffin/CounterStrikeSharp/commit/1d6bee02cdafbb5142ada7c2649336ed61de3335))
* Fix svg colors/optimization by [@switz](https://github.com/switz) in [#175](https://github.com/roflmuffin/CounterStrikeSharp/pull/175) ([72e1f22](https://github.com/roflmuffin/CounterStrikeSharp/commit/72e1f22e147f29f96b843fe9bcc3efd748e7977e))

## What's Changed in v1.0.110
* docx: update 404 page ([a8a238b](https://github.com/roflmuffin/CounterStrikeSharp/commit/a8a238bdee2f5ef3eddcbc0f77b4576aeff2f751))
* docs: exclude system object inheritance ([cec8ef3](https://github.com/roflmuffin/CounterStrikeSharp/commit/cec8ef3d3054829a76f647605604d1a14592cc14))

## What's Changed in v1.0.109
* docs: update docfx fonts to match old docs ([c7384df](https://github.com/roflmuffin/CounterStrikeSharp/commit/c7384df39678af37dae2120b6597d1e4d32b23df))

## What's Changed in v1.0.108
* Replace documenation with docfx by [@busheezy](https://github.com/busheezy) in [#165](https://github.com/roflmuffin/CounterStrikeSharp/pull/165) ([84d4998](https://github.com/roflmuffin/CounterStrikeSharp/commit/84d4998a72d43449673fb1b7ffbb8ce51e87bc8c))

## New Contributors
* [@busheezy](https://github.com/busheezy) made their first contribution in [#165](https://github.com/roflmuffin/CounterStrikeSharp/pull/165)

## What's Changed in v1.0.107
* chore: update hl2sdk, gitignore ([1b19431](https://github.com/roflmuffin/CounterStrikeSharp/commit/1b194318af4628b0c8a99ec9e9bb4d822ede0c0e))

## What's Changed in v1.0.102
* fix: always run preworld update if tasks is empty in [#172](https://github.com/roflmuffin/CounterStrikeSharp/pull/172) ([22d0dd8](https://github.com/roflmuffin/CounterStrikeSharp/commit/22d0dd8200587949fbefcb5156a0cd6a91f1d1b2))

## What's Changed in v1.0.101
* Prevent calling natives on non-main thread by [@Poggicek](https://github.com/Poggicek) in [#170](https://github.com/roflmuffin/CounterStrikeSharp/pull/170) ([7baf0a2](https://github.com/roflmuffin/CounterStrikeSharp/commit/7baf0a25e228721802c06e1cfde285467b04fb13))

## New Contributors
* [@Poggicek](https://github.com/Poggicek) made their first contribution in [#170](https://github.com/roflmuffin/CounterStrikeSharp/pull/170)

## What's Changed in v1.0.100
* fix: run deleted plugin handler in next world update ([9fdbb95](https://github.com/roflmuffin/CounterStrikeSharp/commit/9fdbb9500bb742c82a1a82853bf3fb222885b80e))

## What's Changed in v1.0.99
* feat: add `NextWorldUpdate` helper to run on next pre world update ([0ab3cf4](https://github.com/roflmuffin/CounterStrikeSharp/commit/0ab3cf429a3764fd58c924886a4e62422fd96f8f))

## What's Changed in v1.0.98
* Enriched SteamID by [@chte](https://github.com/chte) in [#163](https://github.com/roflmuffin/CounterStrikeSharp/pull/163) ([1f9630b](https://github.com/roflmuffin/CounterStrikeSharp/commit/1f9630b92d5ee68a8443f431afaf375a3322905c))

## New Contributors
* [@chte](https://github.com/chte) made their first contribution in [#163](https://github.com/roflmuffin/CounterStrikeSharp/pull/163)

## What's Changed in v1.0.97
* docs: add database (dapper) example plugin ([02bf248](https://github.com/roflmuffin/CounterStrikeSharp/commit/02bf2483d3fd8352c080f707868b1c2cc37f0a72))
* docs: add database (dapper) example plugin ([cb181b6](https://github.com/roflmuffin/CounterStrikeSharp/commit/cb181b6a4976ebb33e8978a3bcf72362a68bd3a1))

## What's Changed in v1.0.96
* Exposing from `ISource2Server` and `IVEngineServer2` by [@KillStr3aK](https://github.com/KillStr3aK) in [#159](https://github.com/roflmuffin/CounterStrikeSharp/pull/159) ([cc21dca](https://github.com/roflmuffin/CounterStrikeSharp/commit/cc21dca5a0e836549e37e64abc1f6b80075715ea))

## What's Changed in v1.0.95
* feat: add overload for `PrintToCenterHtml` that accepts duration ([5721d06](https://github.com/roflmuffin/CounterStrikeSharp/commit/5721d060ea9fbc120ecc6e7c2c62acfc58ec3128))

## What's Changed in v1.0.94
* fix: free callback property on game event unhook ([220521d](https://github.com/roflmuffin/CounterStrikeSharp/commit/220521d57127d57643556b4f0ee501a499075c69))

## What's Changed in v1.0.93
* fix: use authorized Steam ID for admin system ([5698b51](https://github.com/roflmuffin/CounterStrikeSharp/commit/5698b511e9a5655ee0c0ca849c85194b68fefbc5))

## What's Changed in v1.0.92
* feat: add `IpAddress` to `CCSPlayerController` ([48c9d19](https://github.com/roflmuffin/CounterStrikeSharp/commit/48c9d195ff9e43876687af989a9863353382f271))

## What's Changed in v1.0.91
* fix: remove reference equality for `CEntityInstance` ([603827d](https://github.com/roflmuffin/CounterStrikeSharp/commit/603827d3318c03505064e23bac304caf55421b72))

## What's Changed in v1.0.90
* fix: fires client authorize on map change ([e557d54](https://github.com/roflmuffin/CounterStrikeSharp/commit/e557d54c3285691e46056e3823c0d13f609cb08f))

## What's Changed in v1.0.89
* VirtualFunction & MemoryFunction rework to support arbitrary binary path by [@KillStr3aK](https://github.com/KillStr3aK) in [#158](https://github.com/roflmuffin/CounterStrikeSharp/pull/158) ([48d3ade](https://github.com/roflmuffin/CounterStrikeSharp/commit/48d3ade5cfa76320365c919249c2f6759df1019c))

## What's Changed in v1.0.88
* fix github actions xml warnings by [@dran1x](https://github.com/dran1x) in [#164](https://github.com/roflmuffin/CounterStrikeSharp/pull/164) ([77b05e9](https://github.com/roflmuffin/CounterStrikeSharp/commit/77b05e912e8b0f5f065fc3f0a2eb1dafec06a938))

## New Contributors
* [@dran1x](https://github.com/dran1x) made their first contribution in [#164](https://github.com/roflmuffin/CounterStrikeSharp/pull/164)

## What's Changed in v1.0.87
* docs: add some example plugins in [#154](https://github.com/roflmuffin/CounterStrikeSharp/pull/154) ([1354b49](https://github.com/roflmuffin/CounterStrikeSharp/commit/1354b4972d0d8cf8e8cd0f59dfde66232dd312ba))

## What's Changed in v1.0.86
* Update README.md by [@johnoclockdk](https://github.com/johnoclockdk) in [#153](https://github.com/roflmuffin/CounterStrikeSharp/pull/153) ([8b5eb7e](https://github.com/roflmuffin/CounterStrikeSharp/commit/8b5eb7e38d5404f235747d890ef601cfbbd0bf27))
* docs: add missing core config doc ([2dd62c4](https://github.com/roflmuffin/CounterStrikeSharp/commit/2dd62c44d3b3f0ba36e4d17d28b9eabcc05d0c57))

## New Contributors
* [@johnoclockdk](https://github.com/johnoclockdk) made their first contribution in [#153](https://github.com/roflmuffin/CounterStrikeSharp/pull/153)

## What's Changed in v1.0.85
* feat: add option to disable plugin hot reload, closes #151 ([f811338](https://github.com/roflmuffin/CounterStrikeSharp/commit/f811338ce4eea6f5cbf0f784e9e02e1dee7bad88))
* Improve plugin setup docs by [@switz](https://github.com/switz) in [#152](https://github.com/roflmuffin/CounterStrikeSharp/pull/152) ([194c340](https://github.com/roflmuffin/CounterStrikeSharp/commit/194c340ae7b2eaddcba530ce6a8fb0b3e553458c))

## What's Changed in v1.0.84
* hotfix: revert entity enumeration ([6b0912d](https://github.com/roflmuffin/CounterStrikeSharp/commit/6b0912d3cdd5f42235f1de1b4aaa04c880d029db))

## What's Changed in v1.0.83
* hotfix: allow handles to be written to again ([2d3aa09](https://github.com/roflmuffin/CounterStrikeSharp/commit/2d3aa09aa466bca4dcfa874ca753de238e8e99c4))

## What's Changed in v1.0.82
* feat: Add Schema Size Native ([911084e](https://github.com/roflmuffin/CounterStrikeSharp/commit/911084e71ecb51fcb51687ef4815827939f96d58))
* Merge remote-tracking branch 'origin/main' into feature/add-schema-class-size ([5b99206](https://github.com/roflmuffin/CounterStrikeSharp/commit/5b9920656827bd5b0e7d1c1ab5ecd42088beaf3f))

## What's Changed in v1.0.81
* Entity Handle Overhaul in [#142](https://github.com/roflmuffin/CounterStrikeSharp/pull/142) ([9bcd0f7](https://github.com/roflmuffin/CounterStrikeSharp/commit/9bcd0f7e9214dc71fe93d86fc847c559906875d3))
* Merge branch 'feature/entity-handle-overhaul' into feature/add-schema-class-size ([4bfdf28](https://github.com/roflmuffin/CounterStrikeSharp/commit/4bfdf28beb2f91f079469eb4f377846ba864cb2d))
* chore: update test plugin version ([11c6486](https://github.com/roflmuffin/CounterStrikeSharp/commit/11c6486ec59fdabc8e17e07e0341292bae51b2c9))
* fix: bad style ([ee69560](https://github.com/roflmuffin/CounterStrikeSharp/commit/ee69560a66a94deae63e3d40e9968a79eff388c0))
* fix: use IntPtr.Zero instead of 0 ([d37e5e1](https://github.com/roflmuffin/CounterStrikeSharp/commit/d37e5e194a3f0dbc59ea6027a23d65cf7b2bd28c))
* feat: add schema class size native, cast native objects to input argument ([c4740d1](https://github.com/roflmuffin/CounterStrikeSharp/commit/c4740d1cc9d64fe50932300cfbe66cff24406153))
* feat: add `Slot` to player controller ([7e92f17](https://github.com/roflmuffin/CounterStrikeSharp/commit/7e92f178fd4cc051f8037e1cf5afd17f11cbe4dd))
* Merge branch 'main' into feature/entity-handle-overhaul ([107ca08](https://github.com/roflmuffin/CounterStrikeSharp/commit/107ca081324d4d90752656ae809c7fac06820c9b))

## What's Changed in v1.0.80
* feat: wrap `ExecuteClientCommand` and add sound example ([8cda8d9](https://github.com/roflmuffin/CounterStrikeSharp/commit/8cda8d9a500692a9656846aef58449c3e0ceb1a5))

## What's Changed in v1.0.79
* fix: add brute force fallback for enum member attribute, fixes #150 ([575c859](https://github.com/roflmuffin/CounterStrikeSharp/commit/575c859ddb6d8a842f76891014c7ce762ff5349c))

## What's Changed in v1.0.78
* fix: wildcard bytes for signatures (resolves #123 and related issues) by [@KillStr3aK](https://github.com/KillStr3aK) in [#148](https://github.com/roflmuffin/CounterStrikeSharp/pull/148) ([e12a7cb](https://github.com/roflmuffin/CounterStrikeSharp/commit/e12a7cb17ad610e7611c3ab10f52c451eabbdbef))
* feat: remove native call from native entity instantiation ([3d59a05](https://github.com/roflmuffin/CounterStrikeSharp/commit/3d59a05de831b07c1ef753f1d3ffd7442371313a))
* feat: add `GetAllEntities` method, update implementation ([77b7040](https://github.com/roflmuffin/CounterStrikeSharp/commit/77b7040d6c35b97c5c9b678a88f366bf4259c140))
* feat: move entity system into managed code for perf ([75de973](https://github.com/roflmuffin/CounterStrikeSharp/commit/75de9732ef9b8852ef418fbe2a2597e448f0b8cb))
* feat: update test plugin ([7c7f52a](https://github.com/roflmuffin/CounterStrikeSharp/commit/7c7f52a2196f5856362602c345b58c711c679a1e))
* feat: add `EntityIndex` back to api compat, mark as obsolete ([cd593fb](https://github.com/roflmuffin/CounterStrikeSharp/commit/cd593fb238030178dad895e550a685052b290876))
* fix: remove expensive calls in bullet impact event ([c5cc65b](https://github.com/roflmuffin/CounterStrikeSharp/commit/c5cc65be481a69d22f953d5ee0d0f5262366a45e))
* feat: add `NativeEntity` class ([59928bb](https://github.com/roflmuffin/CounterStrikeSharp/commit/59928bbcc55f4a71ec080636613e14ab58539f45))

## What's Changed in v1.0.77
* fix: bugs in config manager & plugin load, fixes #138 ([319b116](https://github.com/roflmuffin/CounterStrikeSharp/commit/319b116c5fcc364305c1db60c9a3d73fcf02985d))

## What's Changed in v1.0.76
* Config Example Parsing in [#136](https://github.com/roflmuffin/CounterStrikeSharp/pull/136) ([e0dc053](https://github.com/roflmuffin/CounterStrikeSharp/commit/e0dc053d22d2df135ed735aee6f96055222ed0c5))

## What's Changed in v1.0.75
* Feature: ProcessTargetString() & GetPlayerFromSteamId() by [@CharlesBarone](https://github.com/CharlesBarone) in [#121](https://github.com/roflmuffin/CounterStrikeSharp/pull/121) ([f2e0dac](https://github.com/roflmuffin/CounterStrikeSharp/commit/f2e0dac32de47b7978c817ae5bdb59bce475fbcc))

## New Contributors
* [@CharlesBarone](https://github.com/CharlesBarone) made their first contribution in [#121](https://github.com/roflmuffin/CounterStrikeSharp/pull/121)

## What's Changed in v1.0.74
* Implement Core & Plugin Service Collection in [#129](https://github.com/roflmuffin/CounterStrikeSharp/pull/129) ([4e8c18a](https://github.com/roflmuffin/CounterStrikeSharp/commit/4e8c18abc7fd0ba55647977c2001bcbef1c32418))

## What's Changed in v1.0.73
* Merge branch 'main' of github.com:roflmuffin/CounterStrikeSharp ([8d1891a](https://github.com/roflmuffin/CounterStrikeSharp/commit/8d1891a3a8b6c896f32ad42f8fce98ab0cf9b746))
* Check if userid is valid before accessing its fields, and explain why by [@miguno](https://github.com/miguno) in [#133](https://github.com/roflmuffin/CounterStrikeSharp/pull/133) ([f0c7869](https://github.com/roflmuffin/CounterStrikeSharp/commit/f0c7869f4ac3c0fa0c1fd4d652392c0ed5d48af6))
* feat: add trigger touch start and end hooks ([6bc4344](https://github.com/roflmuffin/CounterStrikeSharp/commit/6bc43444f7c5359e61e1fd910272e11f150add86))

## New Contributors
* [@miguno](https://github.com/miguno) made their first contribution in [#133](https://github.com/roflmuffin/CounterStrikeSharp/pull/133)

## What's Changed in v1.0.72
* feat: Added ability to GiveNamedItem using the new CsItem Enum by [@LordFetznschaedl](https://github.com/LordFetznschaedl) in [#105](https://github.com/roflmuffin/CounterStrikeSharp/pull/105) ([3e38ed3](https://github.com/roflmuffin/CounterStrikeSharp/commit/3e38ed3c77c7b2094e87752757c1a9e6434b50b8))

## New Contributors
* [@LordFetznschaedl](https://github.com/LordFetznschaedl) made their first contribution in [#105](https://github.com/roflmuffin/CounterStrikeSharp/pull/105)

## What's Changed in v1.0.71
* fix: wrong chat colors ([7e9e7c6](https://github.com/roflmuffin/CounterStrikeSharp/commit/7e9e7c666566b77f1de62366c75ecda27b42f903))

## What's Changed in v1.0.70
* feat: add player pawn post think signature ([9a018f2](https://github.com/roflmuffin/CounterStrikeSharp/commit/9a018f295b18130d0d8532e2d390256612c53496))

## What's Changed in v1.0.69
* Dynamic Hooks in [#78](https://github.com/roflmuffin/CounterStrikeSharp/pull/78) ([8b725d4](https://github.com/roflmuffin/CounterStrikeSharp/commit/8b725d435f472b85ad3c08e0842494cd0804e921))

## What's Changed in v1.0.68
* Add methods to respawn players by [@KillStr3aK](https://github.com/KillStr3aK) in [#114](https://github.com/roflmuffin/CounterStrikeSharp/pull/114) ([ea35964](https://github.com/roflmuffin/CounterStrikeSharp/commit/ea3596417a14b61da075e2a98c6a334d20aae535))
* docs: Additional info admin module documentation by [@abnerfs](https://github.com/abnerfs) in [#116](https://github.com/roflmuffin/CounterStrikeSharp/pull/116) ([123f419](https://github.com/roflmuffin/CounterStrikeSharp/commit/123f41914ee729576ac4dbe56c699147edce7302))

## New Contributors
* [@abnerfs](https://github.com/abnerfs) made their first contribution in [#116](https://github.com/roflmuffin/CounterStrikeSharp/pull/116)

## What's Changed in v1.0.67
* Update TestPlugin.cs, by [@Hackmastr](https://github.com/Hackmastr) in [#115](https://github.com/roflmuffin/CounterStrikeSharp/pull/115) ([8f69076](https://github.com/roflmuffin/CounterStrikeSharp/commit/8f69076405a6d789c83efca2b967881e3064e446))

## New Contributors
* [@Hackmastr](https://github.com/Hackmastr) made their first contribution in [#115](https://github.com/roflmuffin/CounterStrikeSharp/pull/115)

## What's Changed in v1.0.66
* chore: upgrade hl2sdk, add protoc generation in [#112](https://github.com/roflmuffin/CounterStrikeSharp/pull/112) ([44a85d1](https://github.com/roflmuffin/CounterStrikeSharp/commit/44a85d1201a66a13a48b69cbd4422b24d4d3d020))

## What's Changed in v1.0.65
* docs: update docs to use `ILogger` ([20f5028](https://github.com/roflmuffin/CounterStrikeSharp/commit/20f50289ee94728cc410f3db30f61e3c7e669016))

## What's Changed in v1.0.64
* Managed Core Logging & Plugin Logging in [#102](https://github.com/roflmuffin/CounterStrikeSharp/pull/102) ([bb5fb5d](https://github.com/roflmuffin/CounterStrikeSharp/commit/bb5fb5de729d6050edb8b5388f4e1fbeefe1c2eb))

## What's Changed in v1.0.63
* hotfix: new signatures by [@KillStr3aK](https://github.com/KillStr3aK) in [#107](https://github.com/roflmuffin/CounterStrikeSharp/pull/107) ([6147739](https://github.com/roflmuffin/CounterStrikeSharp/commit/6147739cfaf00f187555e64702637c4ba70e29dc))

## What's Changed in v1.0.62
* Remove required from flags in AdminData by [@zonical](https://github.com/zonical) in [#106](https://github.com/roflmuffin/CounterStrikeSharp/pull/106) ([3ab5893](https://github.com/roflmuffin/CounterStrikeSharp/commit/3ab5893f22e287d4418f903d1cc180006fca4664))

## What's Changed in v1.0.61
* feat: new virtual functions with wrapper methods by [@KillStr3aK](https://github.com/KillStr3aK) in [#87](https://github.com/roflmuffin/CounterStrikeSharp/pull/87) ([50ce09a](https://github.com/roflmuffin/CounterStrikeSharp/commit/50ce09a7b3f5a5e85a3a8985425c14a7d145aa10))

## What's Changed in v1.0.60
* Merge branch 'main' of github.com:roflmuffin/CounterStrikeSharp ([9c7944e](https://github.com/roflmuffin/CounterStrikeSharp/commit/9c7944e6f18533ac6cedec56e03c81b2b5d8ef6d))
* docs: use nuget instead by [@snorux](https://github.com/snorux) in [#89](https://github.com/roflmuffin/CounterStrikeSharp/pull/89) ([16a1efc](https://github.com/roflmuffin/CounterStrikeSharp/commit/16a1efc0cbfd600b50ed5adef877eb9c8ff4295e))
* feat: add `FireEventToClient` native ([bc71aa7](https://github.com/roflmuffin/CounterStrikeSharp/commit/bc71aa7739bd383cce9f1d7f581c00361ab6c38d))
* docs: add Admin Framework docs category ([8ae85ce](https://github.com/roflmuffin/CounterStrikeSharp/commit/8ae85cedf4787b9b1359e3c104ff29ba5ca74943))

## New Contributors
* [@snorux](https://github.com/snorux) made their first contribution in [#89](https://github.com/roflmuffin/CounterStrikeSharp/pull/89)

## What's Changed in v1.0.59
* Admin Manager improvements by [@zonical](https://github.com/zonical) in [#74](https://github.com/roflmuffin/CounterStrikeSharp/pull/74) ([8e2234f](https://github.com/roflmuffin/CounterStrikeSharp/commit/8e2234ff2535abd9a68a443c6c4938cf91c9c3f0))

## What's Changed in v1.0.58
* fix: add command listener pre handlers ([04e7ed6](https://github.com/roflmuffin/CounterStrikeSharp/commit/04e7ed682acb8827382103a4c991ea6ce25faf3e))

## What's Changed in v1.0.57
* feat: update schema from 17.11.23 update ([15e1260](https://github.com/roflmuffin/CounterStrikeSharp/commit/15e12601462cc9a3685f1217e7deb518ac07324c))

## What's Changed in v1.0.56
* fix: chat command config prefixes ([517607d](https://github.com/roflmuffin/CounterStrikeSharp/commit/517607d96242a923a15bb2768191ef0844742ad9))

## What's Changed in v1.0.55
* Update CommitSuicide offset for 17-11-2023 update by [@b0ink](https://github.com/b0ink) in [#98](https://github.com/roflmuffin/CounterStrikeSharp/pull/98) ([0f72631](https://github.com/roflmuffin/CounterStrikeSharp/commit/0f72631eb0966c6402a0f8fd9b6a67376c962cc3))

## New Contributors
* [@b0ink](https://github.com/b0ink) made their first contribution in [#98](https://github.com/roflmuffin/CounterStrikeSharp/pull/98)

## What's Changed in v1.0.54
* feat: managed coreconfig implementation by [@KillStr3aK](https://github.com/KillStr3aK) in [#79](https://github.com/roflmuffin/CounterStrikeSharp/pull/79) ([75fcf21](https://github.com/roflmuffin/CounterStrikeSharp/commit/75fcf21fb742c68456053b26f668ad8ceeeea059))

## What's Changed in v1.0.53
* fix: new signature for `CBaseModelEntity_SetModel` by [@KillStr3aK](https://github.com/KillStr3aK) in [#84](https://github.com/roflmuffin/CounterStrikeSharp/pull/84) ([0ddf6bc](https://github.com/roflmuffin/CounterStrikeSharp/commit/0ddf6bcdfaef3b5b46ff05f869020f1a22e9b336))

## What's Changed in v1.0.52
* fix: public and silent triggers (finally) ([98661cd](https://github.com/roflmuffin/CounterStrikeSharp/commit/98661cd0693f70af3d8f2c81495af8f42a6d584a))

## What's Changed in v1.0.51
* feat: re-add global command listener ([86a5699](https://github.com/roflmuffin/CounterStrikeSharp/commit/86a5699b40f7740f5aa075296522fbec41d53286))

## What's Changed in v1.0.50
* hotfix: wrap vfunc creation in try catch to prevent all vfuncs from erroring ([414710d](https://github.com/roflmuffin/CounterStrikeSharp/commit/414710d05cedf7e7a9de44f36067bac71aca4b2d))

## What's Changed in v1.0.49
* Improved Command Handling in [#76](https://github.com/roflmuffin/CounterStrikeSharp/pull/76) ([b09c2b6](https://github.com/roflmuffin/CounterStrikeSharp/commit/b09c2b62c840cf763951682cf07f257423743461))

## What's Changed in v1.0.48
* ci: fighting with the machines ([3176051](https://github.com/roflmuffin/CounterStrikeSharp/commit/31760518edd462fb9e9b109924cd0cb833ea2f37))
* ci: remove build number from PR checks ([6a160bc](https://github.com/roflmuffin/CounterStrikeSharp/commit/6a160bcc3df6c455a47f6f73934592b7589783be))

## What's Changed in v1.0.45
* ci: set fallback build number for PRs ([9c8e9db](https://github.com/roflmuffin/CounterStrikeSharp/commit/9c8e9db56e1564a11b391f3c65b8953e99b24f99))
* ci: run main pipeline skipping publish on PR ([e2e0eab](https://github.com/roflmuffin/CounterStrikeSharp/commit/e2e0eab87d3a5aabeeab124e495511678c9fe58d))

## What's Changed in v1.0.44
* feat: `CBaseModelEntity_SetModel` by [@KillStr3aK](https://github.com/KillStr3aK) in [#72](https://github.com/roflmuffin/CounterStrikeSharp/pull/72) ([43292bb](https://github.com/roflmuffin/CounterStrikeSharp/commit/43292bb1d22fda20a71cceb0f92ac0a848d4f426))

## What's Changed in v1.0.43
* hotfix: deserializer couldn't call setter by [@KillStr3aK](https://github.com/KillStr3aK) in [#70](https://github.com/roflmuffin/CounterStrikeSharp/pull/70) ([12c54cd](https://github.com/roflmuffin/CounterStrikeSharp/commit/12c54cd4fc01dcaf9273568d54319ba89b7fab1f))

## What's Changed in v1.0.42
* Cross platform builds in [#69](https://github.com/roflmuffin/CounterStrikeSharp/pull/69) ([e155a70](https://github.com/roflmuffin/CounterStrikeSharp/commit/e155a70873f72521780b3ed8c4f8e5e2ba2d2d82))

## What's Changed in v1.0.41
* feat: Provide configuration standard for plugins by [@KillStr3aK](https://github.com/KillStr3aK) in [#67](https://github.com/roflmuffin/CounterStrikeSharp/pull/67) ([69d9b5d](https://github.com/roflmuffin/CounterStrikeSharp/commit/69d9b5d2c8297dca50a127dfecab40bc6ea56131))

## What's Changed in v1.0.32
* [Windows] feat: Windows support by [@laper32](https://github.com/laper32) in [#52](https://github.com/roflmuffin/CounterStrikeSharp/pull/52) ([933fdf9](https://github.com/roflmuffin/CounterStrikeSharp/commit/933fdf9d81e76199eabaf8419d7c4c5ab8f151cd))

## New Contributors
* [@laper32](https://github.com/laper32) made their first contribution in [#52](https://github.com/roflmuffin/CounterStrikeSharp/pull/52)

## What's Changed in v1.0.31
* `CoreConfig` implementation on the managed side by [@KillStr3aK](https://github.com/KillStr3aK) in [#62](https://github.com/roflmuffin/CounterStrikeSharp/pull/62) ([18e9e37](https://github.com/roflmuffin/CounterStrikeSharp/commit/18e9e37a98def699b97d6b5f1fca40b23cad0571))

## What's Changed in v1.0.30
* hotfix: con command hot reload ([fe23680](https://github.com/roflmuffin/CounterStrikeSharp/commit/fe236806e1ca35f8efcbd4697aa7fdea70a3c80c))

## What's Changed in v1.0.29
* Small adjustments by [@KillStr3aK](https://github.com/KillStr3aK) in [#56](https://github.com/roflmuffin/CounterStrikeSharp/pull/56) ([2c4e9bc](https://github.com/roflmuffin/CounterStrikeSharp/commit/2c4e9bca42094cd6f32a11356a7bc0996387f49b))

## What's Changed in v1.0.28
* docs: add information about flags and standard flags ([8f3e0c2](https://github.com/roflmuffin/CounterStrikeSharp/commit/8f3e0c226b469d57cabdb304beb57920cc64a8d8))

## What's Changed in v1.0.27
* feat: change color marshalling to ABGR (tested against render color) ([5f6ccf2](https://github.com/roflmuffin/CounterStrikeSharp/commit/5f6ccf28391b88a12f7756274223b62ff843e645))
* hotfix: con command hot reload failing ([6c2f567](https://github.com/roflmuffin/CounterStrikeSharp/commit/6c2f56793b9431ea556e46f7f3b60b27ad01e745))

## What's Changed in v1.0.26
* ci: I have the utmost confidence ([cc7dd5c](https://github.com/roflmuffin/CounterStrikeSharp/commit/cc7dd5ca966b7a7e01a7c1ed354a9ebacefeaba6))

## What's Changed in v1.0.25
* ci: publish to api.nuget.org ([ebc361b](https://github.com/roflmuffin/CounterStrikeSharp/commit/ebc361b2f84a55660afebe94ab2cfc77395aa898))

## What's Changed in v1.0.24
* ci: fix nuget source ([c72eff2](https://github.com/roflmuffin/CounterStrikeSharp/commit/c72eff2546ef6b68bccf0e5ea55ce46111b277b1))

## What's Changed in v1.0.23
* ci: add package write permission ([4b432e9](https://github.com/roflmuffin/CounterStrikeSharp/commit/4b432e9efc058c3f9ed8b38b3e0f7c8561190732))
* Merge remote-tracking branch 'origin/main' into main ([22bbf83](https://github.com/roflmuffin/CounterStrikeSharp/commit/22bbf835c7ae019cfa5a5d8fd66796dd08c2e42e))

## What's Changed in v1.0.22
* Update README.md by [@pedrotski](https://github.com/pedrotski) in [#37](https://github.com/roflmuffin/CounterStrikeSharp/pull/37) ([4430060](https://github.com/roflmuffin/CounterStrikeSharp/commit/4430060efdf4c59aee36e2ece0b4e0ce247a7c04))
* ci: try publishing nuget package ([092a607](https://github.com/roflmuffin/CounterStrikeSharp/commit/092a6077c3e6d8424cdefd0467a9d273e0df0a97))
* fix: prevent server crash on duplicate command registration, fixes #51 ([77ea6fd](https://github.com/roflmuffin/CounterStrikeSharp/commit/77ea6fd80d11033343d981cfad7f27dffd50e8ad))

## New Contributors
* [@pedrotski](https://github.com/pedrotski) made their first contribution in [#37](https://github.com/roflmuffin/CounterStrikeSharp/pull/37)

## What's Changed in v1.0.20
* docs: update console command expected usage docs ([f18df3d](https://github.com/roflmuffin/CounterStrikeSharp/commit/f18df3df2b1f9ec4f0555af9b435bc82316fa2aa))

## What's Changed in v1.0.19
* feat: add disabled plugins folder, and source folder for source code ([4ce1ec2](https://github.com/roflmuffin/CounterStrikeSharp/commit/4ce1ec2cf5a96a0a5e3a237390c1fa763b9f90f5))
* fix: my bad merging skills ([9005f3c](https://github.com/roflmuffin/CounterStrikeSharp/commit/9005f3c29cf031084eade36b61d1ba27a2797173))
* feat: change permission helper attribute to `RequiresPermissions` ([b7ace42](https://github.com/roflmuffin/CounterStrikeSharp/commit/b7ace4256abc39bc7771f87bd0ccb4573b4f195a))

## What's Changed in v1.0.18
* Basic admin system framework (plus some cleanup) ([b725f7f](https://github.com/roflmuffin/CounterStrikeSharp/commit/b725f7f79aa1a7d3b74429b83f13b724cd847c35))

## What's Changed in v1.0.17
* feat: implement `IEquatable<T>` for `SteamID` ([cb6d86a](https://github.com/roflmuffin/CounterStrikeSharp/commit/cb6d86a54dbf75b4b170830ff66d946a272437e4))

## What's Changed in v1.0.16
* Merge branch 'main' of github.com:roflmuffin/CounterStrikeSharp into main ([d4a2ae6](https://github.com/roflmuffin/CounterStrikeSharp/commit/d4a2ae68e10b27286d926a9a9edfa9f8780dcf1c))

## What's Changed in v1.0.15
* feat: Add Current API Version to css console command by [@switz](https://github.com/switz) in [#47](https://github.com/roflmuffin/CounterStrikeSharp/pull/47) ([19a0923](https://github.com/roflmuffin/CounterStrikeSharp/commit/19a0923559189ff5114f49c90e96355001b4e43c))
* chore: simplify auto-copy `configs` folder ([82c92f5](https://github.com/roflmuffin/CounterStrikeSharp/commit/82c92f555b0f5afaf65d05ea7bbd438d633c1e58))

## New Contributors
* [@switz](https://github.com/switz) made their first contribution in [#47](https://github.com/roflmuffin/CounterStrikeSharp/pull/47)

## What's Changed in v1.0.14
* fix: ignore `-1` in get players, fixes, #46 ([cef9758](https://github.com/roflmuffin/CounterStrikeSharp/commit/cef9758c12b995a1d06816faa0f86f1fbb178b60))

## What's Changed in v1.0.13
* hotfix: native string memory leak ([0dc3581](https://github.com/roflmuffin/CounterStrikeSharp/commit/0dc35818dd6a334b8fee85e8359722cde90aaf31))

## What's Changed in v1.0.12
* feat: add `PrintToCenterHtml` to player class ([a0f9d30](https://github.com/roflmuffin/CounterStrikeSharp/commit/a0f9d30753b41c0bf6e896964f299f86bc4abb74))

## What's Changed in v1.0.11
* Generate all missing schema properties in [#40](https://github.com/roflmuffin/CounterStrikeSharp/pull/40) ([d6fe9e1](https://github.com/roflmuffin/CounterStrikeSharp/commit/d6fe9e10e183814e7d042ee1da0c1313f32a2975))

## What's Changed in v1.0.10
* Added enum for ItemDefinitionIndex by [@KillStr3aK](https://github.com/KillStr3aK) in [#30](https://github.com/roflmuffin/CounterStrikeSharp/pull/30) ([e1246af](https://github.com/roflmuffin/CounterStrikeSharp/commit/e1246af66a02efb4420aeba46b0a3cae044c9a63))

## What's Changed in v1.0.9
* feat: add custom schema marshalers, provide `Color` schema get/set ([6b4205a](https://github.com/roflmuffin/CounterStrikeSharp/commit/6b4205a0d2af8ab8cdc293487c6ade15b7320af0))

## What's Changed in v1.0.8
* chore: retarget release zip from build/output folder ([f1efc61](https://github.com/roflmuffin/CounterStrikeSharp/commit/f1efc6103d1c4ed601d69402d5e26406ae4af469))

## What's Changed in v1.0.7
* fix: cast long event params properly, fixes #35 ([f6935cc](https://github.com/roflmuffin/CounterStrikeSharp/commit/f6935cc9d2b51a670b57024363d06895b29fde45))

## What's Changed in v1.0.6
* chore: zip and create releases from build artifacts (hopefully) ([c492e9a](https://github.com/roflmuffin/CounterStrikeSharp/commit/c492e9abe1f6ac7a4366a41da196b8bc0604f5a7))
* feat: add `MinimumApiVersion` attribute in [#33](https://github.com/roflmuffin/CounterStrikeSharp/pull/33) ([12a654f](https://github.com/roflmuffin/CounterStrikeSharp/commit/12a654f70a7f866137eeed5bee4c8c4270b6ef94))
* fix: bad commit ([36b1d13](https://github.com/roflmuffin/CounterStrikeSharp/commit/36b1d13ec56b6c7d0317249ff30615d6a5a8fe20))
* feat: add `Utilities.GetPlayers` method which returns valid players ([a029e9d](https://github.com/roflmuffin/CounterStrikeSharp/commit/a029e9df909183a8bf21bd3a6077f9b2784d7dc3))
* chore: improve build artifact naming convention ([107f44a](https://github.com/roflmuffin/CounterStrikeSharp/commit/107f44a3c638e645b181377e3ac313459cf91019))
* fix: workflow permissions ([503ff7e](https://github.com/roflmuffin/CounterStrikeSharp/commit/503ff7e8e68504482b5e91ba681f2ac6ec9cd418))
* Merge branch 'main' of github.com:roflmuffin/CounterStrikeSharp into main ([53dafcd](https://github.com/roflmuffin/CounterStrikeSharp/commit/53dafcdbcd7f210cfdbc7507e739bd97133242e2))
* update docs about libicu / Invariant globalization by [@Apfelwurm](https://github.com/Apfelwurm) in [#32](https://github.com/roflmuffin/CounterStrikeSharp/pull/32) ([0fcc5ed](https://github.com/roflmuffin/CounterStrikeSharp/commit/0fcc5ed88a8cc240078b13eba0d41e8788129c9b))
* Added `HudDestination.Alert` and `CGlowProperty::m_bGlowing` by [@KillStr3aK](https://github.com/KillStr3aK) in [#29](https://github.com/roflmuffin/CounterStrikeSharp/pull/29) ([189d962](https://github.com/roflmuffin/CounterStrikeSharp/commit/189d9626c222d7fb115f522f7537230ce1f12a9c))
* feat: tag assembly with build number (if github actions plays nice) ([9c05f68](https://github.com/roflmuffin/CounterStrikeSharp/commit/9c05f68c1d81cf3a60229fe88952ce3b128e4c5c))
* feat: add `AbsRotation` shorthand accessor property ([901c26f](https://github.com/roflmuffin/CounterStrikeSharp/commit/901c26f58d7d7ebf62e823ad80f4ff5f393d2c97))
* feat: add `AbsOrigin` shorthand accessor property ([ab9242c](https://github.com/roflmuffin/CounterStrikeSharp/commit/ab9242ca4c47b968d02b2dea444734fb8ac2bc72))
* Merge branch 'main' of github.com:roflmuffin/CounterStrikeSharp into main ([fd1040f](https://github.com/roflmuffin/CounterStrikeSharp/commit/fd1040f77cffd200974d91592ea8d49f16f557b1))
* Change access level of BasePlugin RegisterListener / RemoveListener. Add SteamId32 property in SteamId class. by [@IKiwky](https://github.com/IKiwky) in [#27](https://github.com/roflmuffin/CounterStrikeSharp/pull/27) ([04c36ab](https://github.com/roflmuffin/CounterStrikeSharp/commit/04c36ababa034dc52f12a8d7e26f50d2df92a37e))
* fix: prevent crash when accessing StringValue of a non string ConVar ([16c4ee2](https://github.com/roflmuffin/CounterStrikeSharp/commit/16c4ee2b3438af6cad97ae113cd4726c48628810))
* feat: handle string schema setting ([43f0613](https://github.com/roflmuffin/CounterStrikeSharp/commit/43f06138616f497dffd683c97949d9a345c285d8))
* chore: remove sample command ([0aaad35](https://github.com/roflmuffin/CounterStrikeSharp/commit/0aaad356e9e5659674187c72ac74ae608fa0edf5))
* fix: fixes game event creation, and setting of player indexes, fixes #19 and #20 ([acea60d](https://github.com/roflmuffin/CounterStrikeSharp/commit/acea60d87ca6b0e76140749986a382b987a83955))
* feat: add `PrintToServerConsole` native that uses `ConPrint` ([9ed89b4](https://github.com/roflmuffin/CounterStrikeSharp/commit/9ed89b4c0f4d8b8254fcb0bbe6a9ce61ccfca209))
* fix: add gamedata for changeteam ([7efbbc6](https://github.com/roflmuffin/CounterStrikeSharp/commit/7efbbc649d3a42015082ade2df6cfa5ef02e2425))
* feat: add `ChangeTeam` offset ([822115e](https://github.com/roflmuffin/CounterStrikeSharp/commit/822115eddb4dca6c482ff68726506b5e397c7902))
* feat: add CollisionGroup enum ([7b9e237](https://github.com/roflmuffin/CounterStrikeSharp/commit/7b9e237feaa6e71a2646a518902eca4fc970534c))
* feat: add reading & writing of ConVars ([c72fbdc](https://github.com/roflmuffin/CounterStrikeSharp/commit/c72fbdc73bebf6b31d7c5c01dfbf1538b93323c1))
* fix: update test plugin after GiveNamedItem change ([3e87c16](https://github.com/roflmuffin/CounterStrikeSharp/commit/3e87c16e33153eee937ad704f1b9bfbbb04deb90))
* Modified GiveNamedItem to return created entity pointer by [@Muinez](https://github.com/Muinez) in [#23](https://github.com/roflmuffin/CounterStrikeSharp/pull/23) ([ddaa0b3](https://github.com/roflmuffin/CounterStrikeSharp/commit/ddaa0b3905cb7910d5e7623098f235e4534ebd90))
* Add ReplyToCommand for AddCommand / AddCommandListener by [@IKiwky](https://github.com/IKiwky) in [#17](https://github.com/roflmuffin/CounterStrikeSharp/pull/17) ([6df3919](https://github.com/roflmuffin/CounterStrikeSharp/commit/6df39196cfb63275b8b0a546a8bc7972344a67fe))
* 2/11 Gamedata Update by [@zonical](https://github.com/zonical) in [#18](https://github.com/roflmuffin/CounterStrikeSharp/pull/18) ([c01357e](https://github.com/roflmuffin/CounterStrikeSharp/commit/c01357e8006851f1f43dbf656d59e40013b92d1d))
* Implementation of 'ISource2Server hooks & functions' feature request by [@KillStr3aK](https://github.com/KillStr3aK) in [#16](https://github.com/roflmuffin/CounterStrikeSharp/pull/16) ([72cf5ce](https://github.com/roflmuffin/CounterStrikeSharp/commit/72cf5ce7d9255581a7b4285d87df1b2c4d7797c9))
* feat: allow multiple console command attributes for one handler ([f87cfb5](https://github.com/roflmuffin/CounterStrikeSharp/commit/f87cfb5965d01089b0ee03ca2c38cf109dc52191))
* feat: generate additional unused enums ([214264e](https://github.com/roflmuffin/CounterStrikeSharp/commit/214264ea1b86501e8a5043441027cbdd52245d53))
* feat: adds new optional `ModuleAuthor` and `ModuleDescription` properties for plugin authors ([a223151](https://github.com/roflmuffin/CounterStrikeSharp/commit/a2231514db0299b9378d6cb0eec98db132fb4efe))
* feat: add `GetPlayerFrom` utility methods ([c54eda1](https://github.com/roflmuffin/CounterStrikeSharp/commit/c54eda19679aa2db23db62881af9c7be47e1cf6f))
* feat: add ref enum support to schema, update `GameTime_t` to use float ([7655c27](https://github.com/roflmuffin/CounterStrikeSharp/commit/7655c276e0abf3b3e0ecf7e57c27d26183fd12a6))
* docs: update README.md ([c17d40a](https://github.com/roflmuffin/CounterStrikeSharp/commit/c17d40a82c5784587c404cbecab33e2f3f7d4f6b))
* docs: add some basic xml docs ([0489bf7](https://github.com/roflmuffin/CounterStrikeSharp/commit/0489bf7baf8e1d93dc7b7b41efba4100ba546651))
* docs: fix light mode theming ([b35da90](https://github.com/roflmuffin/CounterStrikeSharp/commit/b35da90b7c99181696b4647b7d7d090cea41f0aa))
* docs: add new "Referencing Players" documentation ([421572b](https://github.com/roflmuffin/CounterStrikeSharp/commit/421572b8651c540f502e2e7d4008ec9d5edee371))
* fix: bad commit ([5eb1c22](https://github.com/roflmuffin/CounterStrikeSharp/commit/5eb1c229d0d172745476e99a1cc32bc643701895))
* feat: adds client command listeners ([c49ed8e](https://github.com/roflmuffin/CounterStrikeSharp/commit/c49ed8eafb1c1e7599da05779f515402ff1ef7e3))
* fix: only allow `css_plugins` from server console ([739dcf4](https://github.com/roflmuffin/CounterStrikeSharp/commit/739dcf4da98dbf16291ef466536d2b27cfd56cdb))
* feat: add QAngle constructor ([1f1ce1d](https://github.com/roflmuffin/CounterStrikeSharp/commit/1f1ce1dd3f04abc58806463f89b4d2eba973805b))
* refactor: allow reload/unload by number ([0350e9b](https://github.com/roflmuffin/CounterStrikeSharp/commit/0350e9b36cde005f861c4bc071ba3d1426b44624))
* Add css, css_plugins commands. by [@zonical](https://github.com/zonical) in [#14](https://github.com/roflmuffin/CounterStrikeSharp/pull/14) ([1e9e7e4](https://github.com/roflmuffin/CounterStrikeSharp/commit/1e9e7e4e4bbdb5556a3340ec155f8258d2d56121))
* chore: reduce console spam, default log level to `info` ([c696481](https://github.com/roflmuffin/CounterStrikeSharp/commit/c696481ea392069c276b069148990f14ed35bc51))
* feat; add teleport function ([6798244](https://github.com/roflmuffin/CounterStrikeSharp/commit/6798244684dc648b6c066bcfeebb1c2a09e83417))
* feat: add `CInButtonState` to allow fetching of button status (this should really be generated) ([9c9415a](https://github.com/roflmuffin/CounterStrikeSharp/commit/9c9415a6d64a90f7968edbfd8e7079716964778e))
* feat: add `AbsVelocity` to `CBaseEntity` ([1a1332e](https://github.com/roflmuffin/CounterStrikeSharp/commit/1a1332e84c653c59168522473404e7cfe2586bf7))
* feat: add warcraft plugin example ([78778c8](https://github.com/roflmuffin/CounterStrikeSharp/commit/78778c870e92093e75ae195dae42b8d19e8d9c05))
* chore: remove traces ([7e8b329](https://github.com/roflmuffin/CounterStrikeSharp/commit/7e8b329333b6073875d63013c7f7ebb90aaa7521))
* fix: store prefixed phrase ([b87abdc](https://github.com/roflmuffin/CounterStrikeSharp/commit/b87abdc50c797c8f48a07d6e66e4bd2f2c3289cb))
* chore: add chat manager tracing ([3253d61](https://github.com/roflmuffin/CounterStrikeSharp/commit/3253d6104a0149159696e7fc408ed84aacf559cb))
* fix: bad references, add gun menu example ([c703887](https://github.com/roflmuffin/CounterStrikeSharp/commit/c703887c3d5f5dbb0f113d53e76088118e9e183d))
* feat: add basic `ChatMenu` functionality ([7794fcf](https://github.com/roflmuffin/CounterStrikeSharp/commit/7794fcf5bf07f2e69a80b4afc76cc80ac3d82ef8))
* fix: force `player_chat` event to fire ([5985cc0](https://github.com/roflmuffin/CounterStrikeSharp/commit/5985cc0dcb974d9ec337707102b865a5cfa63915))
* feat: add `Post` event handler hook mode, default event handlers to post ([bff8a80](https://github.com/roflmuffin/CounterStrikeSharp/commit/bff8a80bfa4a1abcf2544a595ac45c8245684099))
* feat: load offsets from gamedata.json ([ac69cc5](https://github.com/roflmuffin/CounterStrikeSharp/commit/ac69cc573b21153b72f6020d82c402a1aebbf5d1))
* feat: add `UTIL_Remove` and `Remove()` on `CEntityInstance` ([69c7db8](https://github.com/roflmuffin/CounterStrikeSharp/commit/69c7db83b8c4a8d3f7a3e5920da823f508278950))
* feat: add `GiveNamedItem` helper to `CCSPlayerController` ([04d79c1](https://github.com/roflmuffin/CounterStrikeSharp/commit/04d79c1ae0048e763ea144ff1873172c42ae9f9c))
* feat: add SwitchTeam method to CCSPlayerController ([6ae0033](https://github.com/roflmuffin/CounterStrikeSharp/commit/6ae0033018d146d8fd4b51ef9b0a927c37db2af7))
* feat: add checks for `css_` prefixed chat commands ([2c50722](https://github.com/roflmuffin/CounterStrikeSharp/commit/2c50722c02cfb816297feffeadf9186191a5a4a5))
* fix: OnAuthorized hook not firing for first player ([c9e4948](https://github.com/roflmuffin/CounterStrikeSharp/commit/c9e4948aa30ce91fdbfb3681fba3739035722e26))
* Merge branch 'main' of github.com:roflmuffin/CounterStrikeSharp into main ([89dd15d](https://github.com/roflmuffin/CounterStrikeSharp/commit/89dd15dcceb0df0da5a395dedab75aff802e0630))
* feat: Adds Chat Triggers for Console Commands in [#13](https://github.com/roflmuffin/CounterStrikeSharp/pull/13) ([e8903e5](https://github.com/roflmuffin/CounterStrikeSharp/commit/e8903e5590370342d984636a7f3a457a93de32bc))
* add libicu readme hint by [@Apfelwurm](https://github.com/Apfelwurm) in [#11](https://github.com/roflmuffin/CounterStrikeSharp/pull/11) ([cb10d9b](https://github.com/roflmuffin/CounterStrikeSharp/commit/cb10d9b45795a0365e6b8db86f1251a968049a43))
* Add license preamble ([9b1a14b](https://github.com/roflmuffin/CounterStrikeSharp/commit/9b1a14bfbd893d691d44dd8e206abfa959cc3e85))
* Create ACKNOWLEDGEMENTS ([adde012](https://github.com/roflmuffin/CounterStrikeSharp/commit/adde0128c6838fda5841a5544dd470fa8ba81b7e))
* fix: remove trace logs ([0bee31f](https://github.com/roflmuffin/CounterStrikeSharp/commit/0bee31f7260bcf5aaf91ae020af18a5491906c49))
* Update README.md ([2d97845](https://github.com/roflmuffin/CounterStrikeSharp/commit/2d978453118c08729ff1b919183872233242a69f))
* fix: `OnMapEnd` listener now fires correctly on `changelevel` ([90ea024](https://github.com/roflmuffin/CounterStrikeSharp/commit/90ea02465e4506577458d454ba1600fb47471848))
* feat: add `CommitSuicide` vfunc method ([369e7c6](https://github.com/roflmuffin/CounterStrikeSharp/commit/369e7c6aa9905dce03e87719ba47754a75fc2d0a))
* feat: add `Server.MaxPlayers` utility ([813ed21](https://github.com/roflmuffin/CounterStrikeSharp/commit/813ed21fe141892b5bcb951f31c58edc5fbcf13c))
* Game Event Broadcast Manipulation & Cancellation in [#10](https://github.com/roflmuffin/CounterStrikeSharp/pull/10) ([65bdd0b](https://github.com/roflmuffin/CounterStrikeSharp/commit/65bdd0b5ffd65e44be107033834bc21316a49759))
* feat: add network vector support ([37b085a](https://github.com/roflmuffin/CounterStrikeSharp/commit/37b085a9f57d6a0c28966a876a979f12ce2ce0fd))
* feat: add FixedArray schema types ([1e66351](https://github.com/roflmuffin/CounterStrikeSharp/commit/1e663514f65ef4bea05140e1cdea8d98c0caa8d4))
* Merge remote-tracking branch 'origin/main' into main ([b22e90d](https://github.com/roflmuffin/CounterStrikeSharp/commit/b22e90d9d3393d6f654d94af49f98fb83749f221))
* Merge branch 'main' of github.com:roflmuffin/CounterStrikeSharp into main ([2c9da17](https://github.com/roflmuffin/CounterStrikeSharp/commit/2c9da1711ccb1b52a7746f3d4947e7527a6f5852))
* docs: add gamedata to example folder structure ([d661283](https://github.com/roflmuffin/CounterStrikeSharp/commit/d661283235db44add9d8c76868875dd95bfd354e))
* docs: fix wrong code ([1dabb1d](https://github.com/roflmuffin/CounterStrikeSharp/commit/1dabb1de2d19c75dba914f8bd6e9414b4b5ed499))
* feat: add GiveNamedItem function ([80ebbe7](https://github.com/roflmuffin/CounterStrikeSharp/commit/80ebbe747796f5d34b8b89686803758dca0a19cd))
* feat: add print methods ([4aaadfd](https://github.com/roflmuffin/CounterStrikeSharp/commit/4aaadfd9d7f4dfce4e2ca22aca1c9a6f4f3b3af5))
* feat: add sig and offset virtual funcs up to 10 args ([8b9e62a](https://github.com/roflmuffin/CounterStrikeSharp/commit/8b9e62aac127d553f62795422eba02461834afc3))
* docs: update home page ([dedd69d](https://github.com/roflmuffin/CounterStrikeSharp/commit/dedd69d9234938e2beb55b33ad034f6130529042))
* docs: add basic command, game event & listener docs ([fd001a8](https://github.com/roflmuffin/CounterStrikeSharp/commit/fd001a8a8b094b537d8b28aa0e0b12207f309d6d))
* chore: apply prettier to docs ([bd8faec](https://github.com/roflmuffin/CounterStrikeSharp/commit/bd8faec9b098352e5803f00841c21c3eb7457d21))
* feat: update console commands to pass a nullable player controller ([726c7c1](https://github.com/roflmuffin/CounterStrikeSharp/commit/726c7c15c938e34f6aa9c38104c4603081649961))
* SDK Generation Overhaul in [#9](https://github.com/roflmuffin/CounterStrikeSharp/pull/9) ([010b0b7](https://github.com/roflmuffin/CounterStrikeSharp/commit/010b0b76db999dbb01402ac63c8a70da0913d5bf))
* Add helper function FindAllEntitiesByDesignerName by [@zonical](https://github.com/zonical) in [#8](https://github.com/roflmuffin/CounterStrikeSharp/pull/8) ([e345602](https://github.com/roflmuffin/CounterStrikeSharp/commit/e345602e5f66bec6ec555ed9f0ec6a2d5420f91d))
* feat: add `DesignerName` directly to `CEntityInstance` ([56b120c](https://github.com/roflmuffin/CounterStrikeSharp/commit/56b120c98aee159c7c5b144f39c21f3f549a87c6))
* feat: add gamedata support, print to all example ([6548894](https://github.com/roflmuffin/CounterStrikeSharp/commit/65488944f7488b6158494ebdc5223fc1699cc59d))
* feat: add `PrintToConsole` native ([12aff1d](https://github.com/roflmuffin/CounterStrikeSharp/commit/12aff1d71f3eb71fc7a3ecba13f35754f6d60fd6))
* feat: improve `CHandle` type, return player from game events ([7446673](https://github.com/roflmuffin/CounterStrikeSharp/commit/7446673b0fdb1ff2f11ae846d9b32f417ffd1c27))
* fix: con command crash ([a2142fe](https://github.com/roflmuffin/CounterStrikeSharp/commit/a2142feedaec0f61c99d4a5ca495705284ff594c))
* fix: attempted concommand fix ([6f88970](https://github.com/roflmuffin/CounterStrikeSharp/commit/6f889707616cf23acb7005bb37004da9ca44d89a))
* Merge remote-tracking branch 'origin/main' into main ([67da276](https://github.com/roflmuffin/CounterStrikeSharp/commit/67da27613df86ca3d10e39d60dc978b59704d610))
* feat: add gc ([f7909e5](https://github.com/roflmuffin/CounterStrikeSharp/commit/f7909e517924b83dd9d8c6f07a475c48d34959f1))
* chore: add trace logging to concommand ([490f84e](https://github.com/roflmuffin/CounterStrikeSharp/commit/490f84e90bce260b7b063499e762f244fdeaf6e2))
* fix: add `FCVAR_LINKED_CONCOMMAND` flag to new commands ([35dc33b](https://github.com/roflmuffin/CounterStrikeSharp/commit/35dc33b6711d24b7a4516b12ed331433786664ed))
* fix: simplify cache logic ([989d768](https://github.com/roflmuffin/CounterStrikeSharp/commit/989d768504bef28d95fcfa0edfdad06c14dc0300))
* feat: add `GetSchemaOffset` method ([ed0fdbb](https://github.com/roflmuffin/CounterStrikeSharp/commit/ed0fdbb490f9ff3efab87e5963756763c5a238a1))
* chore: update dependencies ([76a78c6](https://github.com/roflmuffin/CounterStrikeSharp/commit/76a78c609508208147feefd066d7ab903db4be19))
* feat: add long & short support to events ([d80d212](https://github.com/roflmuffin/CounterStrikeSharp/commit/d80d2128853667b626d9df52b9ee496b723e153b))
* feat: add `PointerTo` class to handle pointers ([b52a58a](https://github.com/roflmuffin/CounterStrikeSharp/commit/b52a58a5454149ab8a579b84689e6507167dd905))
* chore: only build docs on doc change, only build release on code change ([227ae14](https://github.com/roflmuffin/CounterStrikeSharp/commit/227ae14cb4a4d2ccdb600bfdeb0c98ea77363a13))
* Update README.md ([f96edb0](https://github.com/roflmuffin/CounterStrikeSharp/commit/f96edb09efbae6cea8fb1eabd2d693c02067fe73))
* chore: update download link location ([5a0bd4c](https://github.com/roflmuffin/CounterStrikeSharp/commit/5a0bd4caae2a86284366bffaccf7dd9b1682499a))
* feat: add home page ([14bbc9d](https://github.com/roflmuffin/CounterStrikeSharp/commit/14bbc9d8693f18d4027002d1018bbc5961bfa820))
* fix: redirects ([7bd7baf](https://github.com/roflmuffin/CounterStrikeSharp/commit/7bd7baf392f60d84e73071b0f03db0a204fab6ea))
* chore: remove feature/docs ([f4bd58a](https://github.com/roflmuffin/CounterStrikeSharp/commit/f4bd58a26d84aadb4820afb546ff57f0677f31f6))
* Merge branch 'feature/docs' into main ([698fed7](https://github.com/roflmuffin/CounterStrikeSharp/commit/698fed7c82525e924c4b3f28baf2264c96444def))
* feat: add docs ([855ab39](https://github.com/roflmuffin/CounterStrikeSharp/commit/855ab39bff23f8b9af09d6b2288aa41b187d807b))
* feat: generate native objects as partials ([d30a78a](https://github.com/roflmuffin/CounterStrikeSharp/commit/d30a78ac5b45a2826fe509a91b19afb685c4d503))
* feat: add CHandle<T> support ([079c4f6](https://github.com/roflmuffin/CounterStrikeSharp/commit/079c4f68e8fbfd8c604539699a0fe94be8002a03))
* feat: add `GET_ENTITY_POINTER_FROM_HANDLE` native ([46bff5a](https://github.com/roflmuffin/CounterStrikeSharp/commit/46bff5ad804ba1f16414a0f0425cf639e6e69c82))
* test: include generated files in git ([6968b06](https://github.com/roflmuffin/CounterStrikeSharp/commit/6968b065b2ba625724d66f1c5812a457d2a47d52))
* feat: add uint datamapper ([41b667f](https://github.com/roflmuffin/CounterStrikeSharp/commit/41b667fc60cdd23015ed61958450c68f3bde2006))
* fix: native object mapper ([8b96bd1](https://github.com/roflmuffin/CounterStrikeSharp/commit/8b96bd1622ca3576f3609b9d9d0ccc2b8768e1c8))
* fix: generate native objects into core folder ([ffe0de4](https://github.com/roflmuffin/CounterStrikeSharp/commit/ffe0de428dcba871274523e3b345b92a5531d8ce))
* chore: rename index to playerslot ([db02241](https://github.com/roflmuffin/CounterStrikeSharp/commit/db022410be2f8798997bb962ca0dce640e0d016c))
* fix: use dotnet publish ([70795bd](https://github.com/roflmuffin/CounterStrikeSharp/commit/70795bd7f463769b243e0d82be82b7ef0fe79225))
* fix: add uchar/uint8 & enum data mapper ([9f554c4](https://github.com/roflmuffin/CounterStrikeSharp/commit/9f554c4584c46fd994eef5f9ac4dc566b68bff95))
* Merge remote-tracking branch 'origin/main' into main ([4968722](https://github.com/roflmuffin/CounterStrikeSharp/commit/4968722d21adb373400da142069cc3dbb301de46))
* Adds Code Generated Native Objects in [#7](https://github.com/roflmuffin/CounterStrikeSharp/pull/7) ([fbdcda1](https://github.com/roflmuffin/CounterStrikeSharp/commit/fbdcda171c26ac1fcc82500587b35d557bf86cdf))
* fix: add long support to schema retriever ([7e50d81](https://github.com/roflmuffin/CounterStrikeSharp/commit/7e50d81d547f06ad47bd93f44f0408f7a7a4f664))
* Merge branch 'feature/code-generated-native-objects' into main ([3876e13](https://github.com/roflmuffin/CounterStrikeSharp/commit/3876e13fb745a2f6da5feecf13bb1e0651bec9f1))
* feat: add vector support ([c68e234](https://github.com/roflmuffin/CounterStrikeSharp/commit/c68e234f80de5a60627ce8695f3e30cc7b7737fe))
* feat: generate native objects from schema json file ([073c269](https://github.com/roflmuffin/CounterStrikeSharp/commit/073c269271b285a7bd1448c5329fbc7eee295dab))
* feat: Add `OnEntityCreated`, `OnEntityDeleted` and `OnEntityParentChanged` listeners ([dad64bf](https://github.com/roflmuffin/CounterStrikeSharp/commit/dad64bf0e577445a47edc0ff1cd398104c9dcc01))
* feat: add userid from index native ([64283bf](https://github.com/roflmuffin/CounterStrikeSharp/commit/64283bf33ddccb1edc5dc47394218aea7bcccd77))
* feat: add get designer name native ([3b0e0a1](https://github.com/roflmuffin/CounterStrikeSharp/commit/3b0e0a194468eec79e9b7caab6f10f95bf145780))
* feat: add `OnEntitySpawned` listener ([a0a8bfe](https://github.com/roflmuffin/CounterStrikeSharp/commit/a0a8bfe5617c807ea79095e8d84a89d5925fad37))
* feat: add entity from index native ([96830e0](https://github.com/roflmuffin/CounterStrikeSharp/commit/96830e023917e322454ae6055cbda4a2b461ad7c))
* Entity Manipulation/Schema System in [#6](https://github.com/roflmuffin/CounterStrikeSharp/pull/6) ([7fffc96](https://github.com/roflmuffin/CounterStrikeSharp/commit/7fffc96412018a5740effcaa2c3a4ad54a163870))
* feat: add `OnClientAuthorized` listener & steamid class ([4ccb091](https://github.com/roflmuffin/CounterStrikeSharp/commit/4ccb09148b694d0c60d5f30528dc407a7bfd4a32))
* fix: handle plugin deletion hot reload ([54f3f9a](https://github.com/roflmuffin/CounterStrikeSharp/commit/54f3f9a834ae38a6071984585f2efcaee464ac50))
* feat: simplify listeners code ([82495fa](https://github.com/roflmuffin/CounterStrikeSharp/commit/82495fa216fdd53239987faa75117ba26a5fb2ba))
* feat: add support for player entities in game events ([c99845d](https://github.com/roflmuffin/CounterStrikeSharp/commit/c99845d5768c4340eb96ed1626aebc26206efec3))
* fix: console command player slot & case sensitivity ([8164c7f](https://github.com/roflmuffin/CounterStrikeSharp/commit/8164c7fef68b23d0d1ea88d2535732efe920cb4b))
* feat: add module path/directory, cleanup example ([30c6c0f](https://github.com/roflmuffin/CounterStrikeSharp/commit/30c6c0f1e1314fd042cb2f446dd07eea4c26f8e7))
* feat: add virtual function invocation via pointer or signature ([1b9053f](https://github.com/roflmuffin/CounterStrikeSharp/commit/1b9053f770045563f7dae05063c1162c5f8e4ec0))
* chore: update hl2sdk, add c++17 ([238408e](https://github.com/roflmuffin/CounterStrikeSharp/commit/238408e554676a0e263107766dc335fbd7e12786))
* cleanup: more .net warnings ([0fe5c5c](https://github.com/roflmuffin/CounterStrikeSharp/commit/0fe5c5cb023eb6fa5f0558d616f1cfef085d0c7c))
* fix: those pesky warnings once and for all ([55bc777](https://github.com/roflmuffin/CounterStrikeSharp/commit/55bc7773fcc1ad69ce3923ef7eb0b35d427110b6))
* feat: add console command attributes ([0f7d11f](https://github.com/roflmuffin/CounterStrikeSharp/commit/0f7d11f4d2c6edf17e1b397434c10a132f75fdb6))
* feat: add console commands ([c25c4f0](https://github.com/roflmuffin/CounterStrikeSharp/commit/c25c4f068b04efee71a9ad49bd8e76e6a85357b9))
* feat: add protobuf generation to build pipeline ([849e815](https://github.com/roflmuffin/CounterStrikeSharp/commit/849e815080a1e5940510c7231e2f6cda8d99a8d0))
* chore: add protobuf compilation to build ([ea2cfb9](https://github.com/roflmuffin/CounterStrikeSharp/commit/ea2cfb96a49b183c1f452884452f67fb9b17e94f))
* feat: add protobufs from GameTracking-CS2 ([305fe82](https://github.com/roflmuffin/CounterStrikeSharp/commit/305fe82cc3611f10d6e5af8ae6d356b8e7869eb6))
* feat: add event support for `uint64` and `player_controller` (kind of) ([1e3e72e](https://github.com/roflmuffin/CounterStrikeSharp/commit/1e3e72ed64292be28c9795345104d36cd2995c41))
* feat: fix `OnMapStart` hook ([5db9c39](https://github.com/roflmuffin/CounterStrikeSharp/commit/5db9c391c84f4dc98388f098341bcdbedf3f539e))
* core: run clang-format over all files ([2f4ab6e](https://github.com/roflmuffin/CounterStrikeSharp/commit/2f4ab6e0f36be01d827ded7c2e9d81415eae01f9))
* chore: update clang-tidy and clang-format ([71b0961](https://github.com/roflmuffin/CounterStrikeSharp/commit/71b0961fb2c69b74bd9b35b78a4b25ae85e341fa))
* feat: add basic sig scanning native ([82793c7](https://github.com/roflmuffin/CounterStrikeSharp/commit/82793c7804247b691fb34f9c587e0bae16dfe871))
* fix: improve valve interface toString ([ba740e1](https://github.com/roflmuffin/CounterStrikeSharp/commit/ba740e169a7dd3acb10c08f16368d73f8b31c615))
* feat: add basic valve interfaces ([ce9b07b](https://github.com/roflmuffin/CounterStrikeSharp/commit/ce9b07b185dc874b3cbcd906042f4f642ef4a023))
* fix: server command native ([aeae59e](https://github.com/roflmuffin/CounterStrikeSharp/commit/aeae59e77fb7b391b82c269b1012a4b62012576e))
* feat: add initial `GetValveInterface` native ([e253397](https://github.com/roflmuffin/CounterStrikeSharp/commit/e253397957772b5c8d954be1731b84832b052b8b))
* Automatic event registration refactoring by [@Muinez](https://github.com/Muinez) in [#5](https://github.com/roflmuffin/CounterStrikeSharp/pull/5) ([7480abc](https://github.com/roflmuffin/CounterStrikeSharp/commit/7480abc929bf6779863bc7ed93a313160420b5ab))
* feat: update game directory native, add constants/addresses namespace ([312ae55](https://github.com/roflmuffin/CounterStrikeSharp/commit/312ae550c8a265f1740c51852c2d0e733b18af78))
* fix: remove boxing for set methods (thanks Muinez) ([f7a1c55](https://github.com/roflmuffin/CounterStrikeSharp/commit/f7a1c5552dde15e1fc05b99e1ccb4fc4c4d9dddd))
* feat: more game event improvements, automatic registration ([c1c2ec6](https://github.com/roflmuffin/CounterStrikeSharp/commit/c1c2ec6994c514029acd4997719b03e4b7a8cf7e))
* feat: add `EventNameAttribute` for enriching GameEvent types ([e33330c](https://github.com/roflmuffin/CounterStrikeSharp/commit/e33330ccb58abb80c5e4b87c1ebc4056e07d2e8e))
* fix: pr-check workflow ([11b074d](https://github.com/roflmuffin/CounterStrikeSharp/commit/11b074dbab1b483d6b0738f3ae917dc66d87905c))
* Merge remote-tracking branch 'origin/main' into main ([a2049c0](https://github.com/roflmuffin/CounterStrikeSharp/commit/a2049c0717f93ee2abea46ed5ac5384b13e9b4e9))
* feat: improve generic typing of GameEvent in [#4](https://github.com/roflmuffin/CounterStrikeSharp/pull/4) ([ca349d4](https://github.com/roflmuffin/CounterStrikeSharp/commit/ca349d45e76ba6eef2ce42a60aa7e0937fe162e0))
* chore: add conditional builds based on changing managed/cpp code ([dac9f8d](https://github.com/roflmuffin/CounterStrikeSharp/commit/dac9f8d02206a2676957b345a3f9f7c47ba0d3ae))
* fix: order codegen items by filename ([b8b2b02](https://github.com/roflmuffin/CounterStrikeSharp/commit/b8b2b02ba7694cce7f98b5bb78f36ebcd286e05e))
* Improve generated event classes by [@Muinez](https://github.com/Muinez) in [#3](https://github.com/roflmuffin/CounterStrikeSharp/pull/3) ([5b2855e](https://github.com/roflmuffin/CounterStrikeSharp/commit/5b2855efd1cd5b0377714436bb5547530168fdbb))
* feat: update buildkite pipeline for pull request checks ([811df8e](https://github.com/roflmuffin/CounterStrikeSharp/commit/811df8ee8b22509f90374b4cde0a9385f569a67b))
* Merge remote-tracking branch 'origin/main' into main ([8e3f1d1](https://github.com/roflmuffin/CounterStrikeSharp/commit/8e3f1d12108e80c2ed53ee1d7c7a94daedf9ab81))
* chore: update README.md ([8ace472](https://github.com/roflmuffin/CounterStrikeSharp/commit/8ace4725feaaff7cde83085cff50a96902a264d3))
* chore: update README ([427b7cd](https://github.com/roflmuffin/CounterStrikeSharp/commit/427b7cd6c94b61d85d11f61dbf9895443cd9efc3))
* fix: prioritise csgo/game events over core ones ([b4cdc18](https://github.com/roflmuffin/CounterStrikeSharp/commit/b4cdc18d39ecb3bf5c161b73c758957815c12504))
* fix: name overlap of some properties ([12f7959](https://github.com/roflmuffin/CounterStrikeSharp/commit/12f795948fddbe6ad386e66bbbaadd6a8a5528c7))
* chore: cleanup ci config ([5be841e](https://github.com/roflmuffin/CounterStrikeSharp/commit/5be841ebfa26833f20d02e62e3580f6d25303461))
* chore: update build path for CI ([895baa8](https://github.com/roflmuffin/CounterStrikeSharp/commit/895baa85e5f3cbcd940b8e010b2b3c582ab33802))
* chore: more debugging ([943e47b](https://github.com/roflmuffin/CounterStrikeSharp/commit/943e47b9fd8ae39bdec22cc868490b15d7a03198))
* chore: debug gh actions ([8b13ed7](https://github.com/roflmuffin/CounterStrikeSharp/commit/8b13ed781b7f80233370fdb713f3c374b98fa043))
* fix: shell command ([8a5861a](https://github.com/roflmuffin/CounterStrikeSharp/commit/8a5861a25fc30635d986ad3531686d5e11976777))
* feat: try compile against steamrt sniper docker image ([8a93341](https://github.com/roflmuffin/CounterStrikeSharp/commit/8a93341cae8706b3e6ed832b1391cac9d104d625))
* fix: event deregister on hot reload, add more events to sample plugin ([998dbff](https://github.com/roflmuffin/CounterStrikeSharp/commit/998dbff2fb16fe01786cfff48e6ff3f331e992ef))
* feat: add initial c# game event ergonomics, jank code gen script ([1ae0d21](https://github.com/roflmuffin/CounterStrikeSharp/commit/1ae0d21a3b49e73022f17f9e8ace9ac03da2882e))
* feat: make code gen more generic for future scripts ([e1f4046](https://github.com/roflmuffin/CounterStrikeSharp/commit/e1f40467275c1dbbf1e57e1eb1dfcb7808b54471))
* fix: move runtime init back to plugin load ([731d78b](https://github.com/roflmuffin/CounterStrikeSharp/commit/731d78bba7b90ea5ab87531d49a5022a6ea068da))
* Merge remote-tracking branch 'origin/main' into main ([0988707](https://github.com/roflmuffin/CounterStrikeSharp/commit/0988707a89b01118a9a999408516cc4a48e7ba8d))
* chore: update README ([f44ed9f](https://github.com/roflmuffin/CounterStrikeSharp/commit/f44ed9f36dd6e2426ca3acc8d6a231d605dfc1a6))
* chore: update roadmap on README ([491d43e](https://github.com/roflmuffin/CounterStrikeSharp/commit/491d43e548b9a99a8a900237dc98522cee2982ad))
* feat: add event handler to test plugin ([1a20557](https://github.com/roflmuffin/CounterStrikeSharp/commit/1a205570541f9f4d6a6bf8eaeeeaa0805eeb2ae7))
* fix: event handler deregistering ([d150d68](https://github.com/roflmuffin/CounterStrikeSharp/commit/d150d6810c66d2145b3a4ad1dc620b43ae29f84f))
* feat: hook server startup, move .net initialisation to startup ([6fab9d3](https://github.com/roflmuffin/CounterStrikeSharp/commit/6fab9d3fc7822ed6c4c8d4e2b49351c77fbc5497))
* feat: re-enable c# event management ([3d2796d](https://github.com/roflmuffin/CounterStrikeSharp/commit/3d2796d2e0042950ddcd733db7f482dd5007cd23))
* feat: update event listener (thanks to CS2Fixes) ([b677142](https://github.com/roflmuffin/CounterStrikeSharp/commit/b6771425618587ca7575e8630f00ad1413ebd6e7))
* feat: add dyncall ([6a95633](https://github.com/roflmuffin/CounterStrikeSharp/commit/6a956331bf67cae0b91c07edc1f655b76fc039e7))
* feat: add funchook ([0a7a85a](https://github.com/roflmuffin/CounterStrikeSharp/commit/0a7a85a9baca2d3e1d04ab753b6dbf92a29e753e))
* chore: bump hl2sdk-cs ([9ce2be0](https://github.com/roflmuffin/CounterStrikeSharp/commit/9ce2be0ad460982642b8ba5b535d8c7a3c1a9ec4))
* Merge remote-tracking branch 'origin/main' into main ([38f9e0f](https://github.com/roflmuffin/CounterStrikeSharp/commit/38f9e0f36689765f0904ce936bd1d6d01c3c439b))
* chore: update readme with progress ([5c41923](https://github.com/roflmuffin/CounterStrikeSharp/commit/5c41923b5ea1e2709c98c1f396adc5d1ac39fd7c))
* chore: cleanup dotnet errors ([1b5572a](https://github.com/roflmuffin/CounterStrikeSharp/commit/1b5572aeb42ef532ec1704d215ef576b14123e6d))
* feat: add codegen prebuild step for API ([74a21ee](https://github.com/roflmuffin/CounterStrikeSharp/commit/74a21ee0a1fcdf07124ca3b2a9a5234c2cee85ad))
* cleanup: comment out non-working future code ([3ff56b4](https://github.com/roflmuffin/CounterStrikeSharp/commit/3ff56b494854b5e4c1c29fc540c59977f149877e))
* fix: update player_manager events to send int slot ([8291784](https://github.com/roflmuffin/CounterStrikeSharp/commit/829178471b55834c0040985a3069b8daeb496ecf))
* feat: add timer natives ([397af03](https://github.com/roflmuffin/CounterStrikeSharp/commit/397af036803f846e9903db76156425facc7c1051))
* feat: add vector natives ([f5db64a](https://github.com/roflmuffin/CounterStrikeSharp/commit/f5db64a433cf5b6b754665a2679032621b09d1ef))
* cleanup: disable non-working modules, delete menu code ([67138a8](https://github.com/roflmuffin/CounterStrikeSharp/commit/67138a88e0bfd34fcaf6cbcd33767061ec2e4200))
* feat: add next frame task queueing, cleanup mm plugin ([c3e7122](https://github.com/roflmuffin/CounterStrikeSharp/commit/c3e7122cbd7c8f1e3a1060b288ac6f0021168a8a))
* feat: add callback natives yaml ([9428326](https://github.com/roflmuffin/CounterStrikeSharp/commit/9428326da88d302695c0c41f1f2b6ebbabeceb4f))
* feat: add basic native code-gen script ([3455c69](https://github.com/roflmuffin/CounterStrikeSharp/commit/3455c699fa57d6ff7bd0ae9b2f63127b0b923976))
* feat: add GetArguments tuple helper ([b102cea](https://github.com/roflmuffin/CounterStrikeSharp/commit/b102cea0509753d83a51fec8831b8444374ed5d3))
* fix: re-add game frame hook ([f4ea4c7](https://github.com/roflmuffin/CounterStrikeSharp/commit/f4ea4c73f3ed26701fbab398fe3626e61a48c1d4))
* fix: remove event cancellation (for now) ([2039edb](https://github.com/roflmuffin/CounterStrikeSharp/commit/2039edb8616e0332c29c0da49c9139f63e2f18b3))
* cleanup: remove hooks from main plugin ([b4290a6](https://github.com/roflmuffin/CounterStrikeSharp/commit/b4290a631edeb810ba3e6c620336fd53d2c6a956))
* feat: add initial playermanager class ([af4a6c3](https://github.com/roflmuffin/CounterStrikeSharp/commit/af4a6c353a5a424a5257bee628b14a4b636a186b))
* feat: add listener natives ([05ea03b](https://github.com/roflmuffin/CounterStrikeSharp/commit/05ea03b2bf7e7f0ff09f371e71eedc09ece1ea94))
* chore: fix bad name in docs ([f6a0e5f](https://github.com/roflmuffin/CounterStrikeSharp/commit/f6a0e5ffd51eb5cca2f3c65140f45f53ec9299cc))
* chore: update metamod plugin name & description ([d3ac651](https://github.com/roflmuffin/CounterStrikeSharp/commit/d3ac651cdb3d01be43c467ea0737f6f8ce764720))
* chore: add example plugin README ([ca756cb](https://github.com/roflmuffin/CounterStrikeSharp/commit/ca756cb188d1a953fa23ff4efa8d905e3a9acb4f))
* fix: mapname native error ([5dcc841](https://github.com/roflmuffin/CounterStrikeSharp/commit/5dcc84153c62ece475d66622d56874ffa195b2d2))
* fix: update fvisiblity flag, convert to shared library ([7de95be](https://github.com/roflmuffin/CounterStrikeSharp/commit/7de95be9e38f4f649f90d40c1145fd0c6d207550))
* feat: add engine natives, add engine trace shim ([ca81b20](https://github.com/roflmuffin/CounterStrikeSharp/commit/ca81b20e8fa76775ac07a5dd74f1978cb2b17789))
* chore: reformat autonative.h ([317aa6c](https://github.com/roflmuffin/CounterStrikeSharp/commit/317aa6c4764f32d6ad057e067b649430437d04e3))
* feat: add autonative defines ([ce1e5d5](https://github.com/roflmuffin/CounterStrikeSharp/commit/ce1e5d5c90d7da9bac268092be6de5c908d7a494))
* chore: add basic install docs ([9fd565d](https://github.com/roflmuffin/CounterStrikeSharp/commit/9fd565d490bba852ec15b349179e9522b5871ee6))
* chore: scaffold plugins folder in CI artifacts ([5441153](https://github.com/roflmuffin/CounterStrikeSharp/commit/544115331238ffcfc9ccbc1fd964e8839abebc56))
* feat: add timer system, make trace logs visible ([69dab34](https://github.com/roflmuffin/CounterStrikeSharp/commit/69dab3488ce82b0632068537187dc92a564724f8))
* chore: update readme to make it more obvious that not much works yet ([dd5ec58](https://github.com/roflmuffin/CounterStrikeSharp/commit/dd5ec58d55aeda14c232475170a85b4992fefca4))
* feat: add stub event manager ([c9a633e](https://github.com/roflmuffin/CounterStrikeSharp/commit/c9a633e2622c425676ae6740c85b535fa1c25e22))
* chore: use spdlogger for sample logs ([caa3386](https://github.com/roflmuffin/CounterStrikeSharp/commit/caa338665e338a52da40c81b778a289face0f8b2))
* feat: add callback manager, global class ([e4d4549](https://github.com/roflmuffin/CounterStrikeSharp/commit/e4d454967641ffea5d4e44e7434a6a4915a01ddd))
* feat: update test plugin ([9b396ed](https://github.com/roflmuffin/CounterStrikeSharp/commit/9b396ed3c29f2db9b490615aeb657117b071b980))
* feat: add native invoke and script engine context ([156fa53](https://github.com/roflmuffin/CounterStrikeSharp/commit/156fa53fae13c15724b08d53f407395ab2af8dfc))
* feat: add sample plugin ([ce24058](https://github.com/roflmuffin/CounterStrikeSharp/commit/ce240586b20c166d990c098a149d58929ee16018))
* fix: re-order CI steps ([7d9aa32](https://github.com/roflmuffin/CounterStrikeSharp/commit/7d9aa329fe8d43747ead6897b9a735dcbbb879f0))
* feat: add dotnet compilation to CI ([25a5e43](https://github.com/roflmuffin/CounterStrikeSharp/commit/25a5e43985e99f8f13cc6b97249946f4498a1791))
* fix: url ([61fddb2](https://github.com/roflmuffin/CounterStrikeSharp/commit/61fddb2360b6778373541fbeaee5018a1c5dc1f3))
* feat: add initial .net7 api ([21da2b3](https://github.com/roflmuffin/CounterStrikeSharp/commit/21da2b3e26382d7dcdb4de230aabd160e00c4c36))
* feat: use .NET 7 ([2fc9fb6](https://github.com/roflmuffin/CounterStrikeSharp/commit/2fc9fb60a923dfd687e55e3e056b87288501db1b))
* chore: update readme ([54d1a05](https://github.com/roflmuffin/CounterStrikeSharp/commit/54d1a05108d0bb1d23e0fed35fc844fb87fc52c9))
* chore: update readme ([9ccfd97](https://github.com/roflmuffin/CounterStrikeSharp/commit/9ccfd97d134571145d2fb47c4332bb2bc67a116f))
* chore: update readme ([6104244](https://github.com/roflmuffin/CounterStrikeSharp/commit/6104244a4fc57b6778cad7c48060ade7534b17f7))
* feat: include additional artifact that does not include runtime ([74959b6](https://github.com/roflmuffin/CounterStrikeSharp/commit/74959b6c8deb3e21c79f6679a28fe6acceec1c35))
* fix: bad path in ci ([13ccd49](https://github.com/roflmuffin/CounterStrikeSharp/commit/13ccd49c00bce2a7fcbe5c5614cf21e8841ca38c))
* feat: include dotnet runtime in output artifacts ([98979a3](https://github.com/roflmuffin/CounterStrikeSharp/commit/98979a35fbb4af8b88f9882d7c3ade22e98d8a33))
* fix: artifacts hash ([62e959f](https://github.com/roflmuffin/CounterStrikeSharp/commit/62e959f43b67e271eaffc453b758fef581dd5739))
* fix: artifacts ([e08ec35](https://github.com/roflmuffin/CounterStrikeSharp/commit/e08ec35b5a2f73bca5f8f92a1365b9fe32fc2102))
* feat: add vdf and build output directory structure ([a9e79e5](https://github.com/roflmuffin/CounterStrikeSharp/commit/a9e79e57da54b035cd48bc2b37f5e476af177b1f))
* chore: remove `lib` prefix from built file ([ebb2e89](https://github.com/roflmuffin/CounterStrikeSharp/commit/ebb2e8930608b5d46b521364aebc61920629e6e5))
* feat: enable dotnet host, disable .net entrypoint ([0365393](https://github.com/roflmuffin/CounterStrikeSharp/commit/0365393065e4ff175b3205024095f2182ea968ba))
* feat: add spdlog ([3cb2d0e](https://github.com/roflmuffin/CounterStrikeSharp/commit/3cb2d0e11253dd047b5506816051870a8bee08ae))
* feat: add initial globals and dotnet host ([bdeff3f](https://github.com/roflmuffin/CounterStrikeSharp/commit/bdeff3f12cce70677b0ef4fe8b6eb9cb2a1ea5cb))
* chore: update artifact name ([872c59a](https://github.com/roflmuffin/CounterStrikeSharp/commit/872c59a02903c1bc8579b145c4233abafa735f1f))
* chore: update library name ([09f90dd](https://github.com/roflmuffin/CounterStrikeSharp/commit/09f90ddef47ef25ae1debaa5e62bbfa83a4e58af))
* chore: fix artifact name ([79624c6](https://github.com/roflmuffin/CounterStrikeSharp/commit/79624c6c6e0ff39513eb852cd43ff955fe76a023))
* chore: upload artifacts on build ([1821fe4](https://github.com/roflmuffin/CounterStrikeSharp/commit/1821fe4cf3ab1be41d6d4fac5b56c9d2f3943cb9))
* fix: add submodules to build action ([fba161b](https://github.com/roflmuffin/CounterStrikeSharp/commit/fba161b5aec82dbbf41a2cb894a85ea08979d2b0))
* chore: add cmake action ([82e23a0](https://github.com/roflmuffin/CounterStrikeSharp/commit/82e23a0a8293c36ba28aaf1153a3a59dd0c1164b))
* chore: add license ([61993d4](https://github.com/roflmuffin/CounterStrikeSharp/commit/61993d4372bd42413d4ba5a3cdf60e6cde48873a))
* feat: add initial clang format and tidy ([0a74133](https://github.com/roflmuffin/CounterStrikeSharp/commit/0a74133419bed4966a5cf8d62d38ad325c9204e4))
* feat: initial sample plugin commit ([fbff445](https://github.com/roflmuffin/CounterStrikeSharp/commit/fbff4455047d02fc51a5720f7a82a6b3552902b0))
* feat: initial commit ([527bcf6](https://github.com/roflmuffin/CounterStrikeSharp/commit/527bcf61386b3c45f1884b2842fb9b702393298b))

## New Contributors
* [@roflmuffin](https://github.com/roflmuffin) made their first contribution
* [@Apfelwurm](https://github.com/Apfelwurm) made their first contribution in [#32](https://github.com/roflmuffin/CounterStrikeSharp/pull/32)
* [@KillStr3aK](https://github.com/KillStr3aK) made their first contribution in [#29](https://github.com/roflmuffin/CounterStrikeSharp/pull/29)
* [@IKiwky](https://github.com/IKiwky) made their first contribution in [#27](https://github.com/roflmuffin/CounterStrikeSharp/pull/27)
* [@Muinez](https://github.com/Muinez) made their first contribution in [#23](https://github.com/roflmuffin/CounterStrikeSharp/pull/23)
* [@zonical](https://github.com/zonical) made their first contribution in [#18](https://github.com/roflmuffin/CounterStrikeSharp/pull/18)

<!-- generated by git-cliff -->
