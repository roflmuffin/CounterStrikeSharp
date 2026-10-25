import { test } from 'node:test';
import assert from 'node:assert/strict';
import fs from 'node:fs';
import os from 'node:os';
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import { manifestFrom, emit, nativeHash, entityClasses } from '../generate.mjs';

const root = path.resolve(path.dirname(fileURLToPath(import.meta.url)), '../../..');
const reference = path.join(root, 'docs/references/cs_script/master');
const master = manifestFrom(path.join(reference, 'tsconfig.json'), path.join(reference, 'point_script.d.ts'));

test('master module constants and optional money argument match the handover', () => {
  const receiver = master.receivers.find(r => r.exportName === 'GameRules');
  const money = receiver.methods.find(m => m.member === 'AddTeamMoney');
  const expected = JSON.parse(fs.readFileSync(path.join(root, 'docs/examples/cs_script/AddTeamMoney.shape.json')));
  assert.equal(money.supported, true);
  assert.equal(receiver.receiverType, expected.receiverType);
  assert.equal(master.source.gitBlobSha, expected.source.gitBlobSha);
  assert.equal(money.minimumArgumentCount, 2);
  assert.equal(money.maximumArgumentCount, 3);
  assert.deepEqual(money.parameters.map(p => ({ name: p.name, kind: p.shape.kind, optional: p.optional,
    ...(p.shape.enumName ? {enumName:p.shape.enumName} : {}) })), expected.parameters);
  const generated = emit(master);
  assert.match(generated.csharp, /AddTeamMoney\(double @team, CSTeamMoneyReason @reason\)/);
  assert.match(generated.csharp, /AddTeamMoney\(double @team, CSTeamMoneyReason @reason, double @amount\)/);
  const overloads = [...generated.csharp.matchAll(/void @AddTeamMoney\([^)]*\)\s*\{([^}]+)\}/g)].map(m => m[1]);
  assert.equal(overloads.length, 2);
  assert.deepEqual(overloads.map(body => (body.match(/call.Push\(/g) ?? []).length), [2, 3]);
  assert.equal(overloads[0].match(/BeginInvoke\((0x\w+)/)[1], overloads[1].match(/BeginInvoke\((0x\w+)/)[1]);
  assert.doesNotMatch(generated.csharp, /new object\[\]|Json|GetDouble\(/);
  assert.doesNotMatch(generated.native, /R"CSS|Json|nlohmann/);
  assert.equal(generated.descriptors.some(d => d.receiver === 'Instance' && d.member === 'AddTeamMoney'), false);
});

test('hashes are stable across declaration reordering, documentation edits and unrelated additions', () => {
  const original = emit(master);
  const changed = structuredClone(master);
  changed.receivers.reverse();
  for (const receiver of changed.receivers) {
    receiver.methods.reverse();
    for (const method of receiver.methods) {
      method.documentation = 'Edited documentation';
      method.parameters.forEach((p, i) => p.name = `arg${i}`);
    }
  }
  const reordered = emit(changed);
  assert.deepEqual(new Map(original.descriptors.map(d => [d.identity, d.id])),
    new Map(reordered.descriptors.map(d => [d.identity, d.id])));
  const schema = text => text.match(/std::array<uint64_t, 4> SchemaHash = \{([^}]+)\}/)[1];
  assert.equal(schema(original.native), schema(reordered.native));
  const receiver = changed.receivers.find(r => r.exportName === 'GameRules');
  receiver.methods.unshift({ ...structuredClone(receiver.methods.find(m => m.supported)), member: 'UnrelatedMethod' });
  const added = emit(changed);
  for (const descriptor of original.descriptors)
    assert.equal(added.descriptors.find(d => d.identity === descriptor.identity).id, descriptor.id);
  assert.notEqual(schema(original.native), schema(added.native));
});

test('hash follows existing native DJB2-XOR and signature changes get different IDs', () => {
  assert.equal(nativeHash(''), 5381);
  assert.equal(nativeHash('A'), (5381 * 33) ^ 65);
  const generated = emit(master);
  const money = generated.descriptors.find(d => d.receiver === 'GameRules' && d.member === 'AddTeamMoney');
  assert.equal(money.identity, 'cs_script/point_script:export:GameRules.AddTeamMoney(number,enum:CSTeamMoneyReason,number?)->void');
  assert.equal(money.id, nativeHash(money.identity));
  assert.notEqual(money.id, nativeHash(money.identity.replace('number?', 'number')));
  assert.notEqual(money.id, nativeHash(money.identity.replace('GameRules', 'Instance')));
  const changed = structuredClone(master);
  const duplicate = changed.receivers.find(r => r.exportName === 'GameRules');
  duplicate.methods.push(structuredClone(duplicate.methods.find(m => m.supported)));
  assert.throws(() => emit(changed), /Method hash collision/);
});

test('distinct signatures with colliding hashes fail generation', () => {
  const method = member => ({member, supported:true, parameters:[], returns:{kind:'void'}, minimumArgumentCount:0, maximumArgumentCount:0});
  const manifest = {module:'cs_script/point_script', enums:[], receivers:[{
    exportName:'GameRules', kind:'export', methods:[method('abF'), method('aea')]
  }]};
  assert.equal(nativeHash('cs_script/point_script:export:GameRules.abF()->void'),
    nativeHash('cs_script/point_script:export:GameRules.aea()->void'));
  assert.throws(() => emit(manifest), /Method hash collision.*abF.*aea/);
});

test('generated calls cannot overflow the native argument slots', () => {
  const changed = structuredClone(master);
  const method = changed.receivers.flatMap(r => r.methods).find(m => m.supported);
  method.maximumArgumentCount = 25;
  assert.throws(() => emit(changed), /argument capacity exceeded/);
});

test('enum values use compiler evaluation and inheritance is included', () => {
  const money = master.enums.find(e => e.name === 'CSTeamMoneyReason');
  assert.deepEqual(money.members.slice(0, 2), [{ name:'NONE', value:0 }, { name:'TERRORIST_WIN_BOMB', value:1 }]);
  const player = master.receivers.find(r => r.exportName === 'CSPlayerController');
  assert.ok(player.methods.some(m => m.member === 'SetEntityName' && m.supported));
  assert.deepEqual(emit(master), emit(master));
});

test('overloads, intersections, nested arrays, optional fields and unsupported lifetimes', () => {
  const dir = fs.mkdtempSync(path.join(os.tmpdir(), 'css-generator-'));
  try {
    fs.writeFileSync(path.join(dir, 'tsconfig.json'), JSON.stringify({compilerOptions:{strict:true,target:'ES2022',noEmit:true}}));
    fs.writeFileSync(path.join(dir, 'point_script.d.ts'), `declare module "cs_script/point_script" {
      export enum Reason { First = 4, Next, Shifted = 1 << 5 }
      type Options = { optional?: number; nested: string[][] } & { enabled: boolean };
      interface API {
        Overload(x: string): void; Overload(x: number): boolean;
        Shape(options: Options): void;
        Callback(cb: () => void): void;
        Later(): Promise<number>;
        Generic<T>(x: T): void;
        Unknown(x: any): void;
      }
      export const GameRules: API;
      export class Entity { Rename(name: string): void; }
      export class Child extends Entity { Own(): number; }
    }`);
    const manifest = manifestFrom(path.join(dir, 'tsconfig.json'), path.join(dir, 'point_script.d.ts'));
    assert.deepEqual(manifest.enums[0].members.map(m=>m.value), [4,5,32]);
    const methods = manifest.receivers.find(r=>r.exportName==='GameRules').methods;
    const shape = methods.find(m=>m.member==='Shape').parameters[0].shape;
    assert.equal(shape.properties.find(p=>p.name==='optional').optional,true);
    assert.deepEqual(shape.properties.find(p=>p.name==='nested').shape, {kind:'array',element:{kind:'array',element:{kind:'string'}}});
    assert.ok(methods.filter(m=>['Callback','Later','Generic','Unknown','Shape'].includes(m.member)).every(m=>!m.supported));
    const cs = emit(manifest, { ...entityClasses, Child: 'CTestChild' }).csharp;
    assert.match(cs, /void @Overload\(string @x\)/);
    assert.match(cs, /bool @Overload\(double @x\)/);
    assert.doesNotMatch(cs, /@Later\(/);
    assert.ok(manifest.receivers.find(r=>r.exportName==='Child').methods.some(m=>m.member==='Rename'));
  } finally { fs.rmSync(dir, {recursive:true,force:true}); }
});


test('entity methods extend schema classes and acquire the shared runtime per call', () => {
  const generated = emit(master);
  for (const [receiver, schemaClass] of Object.entries(entityClasses)) {
    assert.ok(fs.existsSync(path.join(root, `managed/CounterStrikeSharp.API/Generated/Schema/Classes/${schemaClass}.g.cs`)));
    assert.match(generated.entities, new RegExp(`public partial class ${schemaClass}\\b`));
    assert.doesNotMatch(generated.csharp, new RegExp(`class CsScript${receiver}\\b`));
  }
  const planted = generated.descriptors.find(d => d.receiver === 'CSPlantedC4' && d.member === 'IsBombsiteA');
  assert.match(generated.entities, new RegExp(`public partial class CPlantedC4[^]*?public bool @IsBombsiteA\\(\\)[^]*?CsScriptRuntime.Shared.BeginInvoke\\(0x${planted.id.toString(16)}u, EntityHandle.Raw\\)`));
  assert.equal((generated.entities.match(/public string @GetEntityName\(/g) ?? []).length, 1);
  assert.equal((generated.entities.match(/public void @SetModelScript\(/g) ?? []).length, 1);
  assert.doesNotMatch(generated.entities, /private readonly|internal CPlantedC4|public .* @(IsValid|Remove|SetModel|IsRescued|IsBot|IsScoped)\(/);
  for (const name of ['IsValid', 'Remove', 'SetModel', 'IsRescued', 'IsBot', 'IsScoped'])
    assert.match(generated.entities, new RegExp(`@${name}Script\\(`));
  assert.doesNotMatch(generated.entities, /new object\[\]|Json/);
});

test('unmapped entity exports fail generation', () => {
  const changed = structuredClone(master);
  changed.receivers.find(r => r.kind === 'entity').exportName = 'UnmappedEntity';
  assert.throws(() => emit(changed), /Missing schema class mapping: UnmappedEntity/);
});
