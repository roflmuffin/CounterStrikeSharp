import ts from 'typescript';
import fs from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import { createHash } from 'node:crypto';

const root = path.resolve(path.dirname(fileURLToPath(import.meta.url)), '../..');
export function manifestFrom(configPath, declarationPath) {
  const config = ts.readConfigFile(configPath, ts.sys.readFile);
  if (config.error) throw new Error(ts.flattenDiagnosticMessageText(config.error.messageText, '\n'));
  const parsed = ts.parseJsonConfigFileContent(config.config, ts.sys, path.dirname(configPath));
  const program = ts.createProgram([...new Set([...parsed.fileNames, declarationPath])], parsed.options);
  const errors = [...parsed.errors, ...ts.getPreEmitDiagnostics(program)].filter(d => d.category === ts.DiagnosticCategory.Error);
  if (errors.length) throw new Error(ts.formatDiagnosticsWithColorAndContext(errors, {
    getCanonicalFileName: x => x, getCurrentDirectory: () => root, getNewLine: () => '\n'
  }));
  const checker = program.getTypeChecker();
  const module = checker.getAmbientModules().find(s => s.name === '"cs_script/point_script"');
  if (!module) throw new Error('Missing cs_script/point_script module');
  const exports = checker.getExportsOfModule(module).map(s => s.flags & ts.SymbolFlags.Alias ? checker.getAliasedSymbol(s) : s);
  const enums = [];
  // Ambient auto-numbered members have no constant value in TypeScript.
  // Ask the compiler to evaluate an implementation-form copy of the declarations.
  const enumFile = path.resolve(path.dirname(declarationPath), '__cs_script_enums.ts');
  const enumText = exports.flatMap(s => s.declarations?.filter(ts.isEnumDeclaration).map(d => d.getText().replace(/^export\s+/, '')) ?? []).join('\n');
  const enumHost = ts.createCompilerHost(parsed.options);
  const getSourceFile = enumHost.getSourceFile.bind(enumHost);
  enumHost.getSourceFile = (name, ...args) => path.resolve(name) === enumFile ? ts.createSourceFile(name, enumText, ts.ScriptTarget.ES2022, true) : getSourceFile(name, ...args);
  const enumProgram = ts.createProgram([enumFile], parsed.options, enumHost);
  const enumChecker = enumProgram.getTypeChecker();
  const evaluatedEnums = new Map(enumProgram.getSourceFile(enumFile).statements.filter(ts.isEnumDeclaration).map(d => [d.name.text, d]));
  for (const symbol of exports) {
    const decl = symbol.declarations?.find(ts.isEnumDeclaration);
    if (!decl) continue;
    enums.push({ name: symbol.name, ...documentation(symbol), members: evaluatedEnums.get(symbol.name).members.map(m => {
      const value = enumChecker.getConstantValue(m);
      if (typeof value !== 'number' || !Number.isSafeInteger(value)) throw new Error(`Unsupported enum value: ${symbol.name}.${m.name.getText()}`);
      return { name: m.name.getText(), value };
    }) });
  }
  const enumNames = new Set(enums.map(e => e.name));
  const entities = new Set();
  function isEntity(type) {
    if (type.symbol?.name === 'Entity') return true;
    return type.getBaseTypes?.()?.some(isEntity) ?? false;
  }
  for (const symbol of exports) if (symbol.flags & ts.SymbolFlags.Class && isEntity(checker.getDeclaredTypeOfSymbol(symbol))) entities.add(symbol.name);
  function documentation(symbol) {
    const tags = symbol.getJsDocTags(checker);
    return { documentation: ts.displayPartsToString(symbol.getDocumentationComment(checker)),
      deprecated: tags.some(t => t.name === 'deprecated') };
  }
  function shape(type, seen = new Set()) {
    const enumName = enumNames.has(type.symbol?.name) ? type.symbol.name : type.symbol?.parent?.name;
    if (enumNames.has(enumName) && (type.flags & (ts.TypeFlags.EnumLike | ts.TypeFlags.Union))) return { kind: 'enum', enumName };
    if (type.isUnion() && type.types.every(t => t.flags & ts.TypeFlags.BooleanLike)) return { kind: 'boolean' };
    if (type.flags & ts.TypeFlags.Number) return { kind: 'number' };
    if (type.flags & ts.TypeFlags.String) return { kind: 'string' };
    if (type.flags & ts.TypeFlags.Boolean) return { kind: 'boolean' };
    if (type.flags & ts.TypeFlags.Void) return { kind: 'void' };
    if (type.flags & ts.TypeFlags.Undefined) return { kind: 'undefined' };
    if (type.flags & ts.TypeFlags.Null) return { kind: 'null' };
    if (type.isLiteral()) return { kind: 'literal', value: type.value };
    if (type.flags & ts.TypeFlags.BooleanLiteral) return { kind: 'literal', value: type.intrinsicName === 'true' };
    if (type.flags & ts.TypeFlags.Any) return { kind: 'unsupported', reason: 'any' };
    if (type.flags & ts.TypeFlags.TypeParameter) return { kind: 'unsupported', reason: 'generic' };
    if (type.symbol?.name === 'Promise') return { kind: 'unsupported', reason: 'Promise lifetime' };
    if (type.getCallSignatures().length) return { kind: 'unsupported', reason: 'function ownership' };
    if (entities.has(type.symbol?.name)) return { kind: 'entity', entityType: type.symbol.name };
    if (seen.has(type)) return { kind: 'unsupported', reason: 'recursive shape' };
    const next = new Set([...seen, type]);
    if (checker.isArrayType(type)) return { kind: 'array', element: shape(checker.getTypeArguments(type)[0], next) };
    if (type.isUnion()) return { kind: 'union', alternatives: type.types.map(t => shape(t, next)) };
    if (type.flags & ts.TypeFlags.Object || type.isIntersection()) return {
      kind: 'object', name: type.aliasSymbol?.name ?? type.symbol?.name ?? null,
      properties: checker.getPropertiesOfType(type).map(p => ({ name: p.name, optional: !!(p.flags & ts.SymbolFlags.Optional),
        shape: shape(checker.getTypeOfSymbolAtLocation(p, p.valueDeclaration ?? p.declarations[0]), next) }))
    };
    return { kind: 'unsupported', reason: checker.typeToString(type) };
  }
  const receivers = [];
  const diagnostics = [];
  for (const symbol of exports) {
    if (!(symbol.flags & (ts.SymbolFlags.Class | ts.SymbolFlags.BlockScopedVariable))) continue;
    const type = symbol.flags & ts.SymbolFlags.Class ? checker.getDeclaredTypeOfSymbol(symbol) : checker.getTypeOfSymbolAtLocation(symbol, symbol.valueDeclaration);
    const receiver = { exportName: symbol.name, receiverType: checker.typeToString(type), kind: entities.has(symbol.name) ? 'entity' : 'export', methods: [] };
    if (symbol.flags & ts.SymbolFlags.Class && !entities.has(symbol.name)) {
      diagnostics.push({ receiver: symbol.name, reason: 'non-entity class ownership' }); continue;
    }
    for (const member of checker.getPropertiesOfType(type)) {
      const memberType = checker.getTypeOfSymbolAtLocation(member, member.valueDeclaration ?? member.declarations[0]);
      const signatures = memberType.getCallSignatures();
      if (!signatures.length) { diagnostics.push({ receiver: symbol.name, member: member.name, reason: 'property access' }); continue; }
      for (const [overload, sig] of signatures.entries()) {
        const parameters = sig.parameters.map(p => {
          const decl = p.valueDeclaration;
          return { name: p.name, optional: !!(p.flags & ts.SymbolFlags.Optional) || !!decl?.questionToken,
            rest: !!decl?.dotDotDotToken, shape: shape(checker.getTypeOfSymbolAtLocation(p, decl)) };
        });
        const method = { member: member.name, overload, ...documentation(member), parameters,
          ...(receiver.kind === 'entity' ? { declaringReceiver: member.declarations[0].parent.name.text } : {}),
          returns: shape(sig.getReturnType()), minimumArgumentCount: sig.minArgumentCount, maximumArgumentCount: parameters.length };
        const primitive = s => ['number', 'string', 'boolean', 'enum', 'void'].includes(s.kind);
        // The manifest preserves every shape. Milestone one facades only expose
        // scalar signatures; unsupported ownership contracts are never stubs.
        for (const p of parameters) if (p.optional && p.shape.kind === 'union') {
          const rest = p.shape.alternatives.filter(x => x.kind !== 'undefined');
          if (rest.length === 1) p.shape = rest[0];
        }
        method.supported = !sig.typeParameters?.length && !method.deprecated && parameters.every(p => !p.rest && primitive(p.shape)) && primitive(method.returns);
        if (!method.supported) diagnostics.push({ receiver: symbol.name, member: member.name, overload,
          reason: sig.typeParameters?.length ? 'generic' : method.deprecated ? 'deprecated alias excluded' : 'shape requires additional marshalling/ownership support' });
        receiver.methods.push(method);
      }
    }
    receivers.push(receiver);
  }
  const bytes = Buffer.from(fs.readFileSync(declarationPath, 'utf8').replaceAll('\r\n', '\n'));
  const gitBlobSha = createHash('sha1').update(`blob ${bytes.length}\0`).update(bytes).digest('hex');
  return { schemaVersion: 1, module: 'cs_script/point_script', source: { gitBlobSha }, enums, receivers, diagnostics };
}

const ident = x => '@' + x;
const literal = x => JSON.stringify(x);
// Same DJB2-XOR algorithm as scripting/script_engine.h. Identities are ASCII
// so signed-char behaviour cannot differ between platforms.
export function nativeHash(identity) {
  if (!/^[\x00-\x7f]*$/.test(identity)) throw new Error('Native hash identities must be ASCII');
  let hash = 5381;
  for (const c of identity) hash = (Math.imul(hash, 33) ^ c.charCodeAt(0)) >>> 0;
  return hash;
}
const hex = value => '0x' + value.toString(16).padStart(8, '0');
const operations = ['Current', 'Host', 'Ready', 'Invoke', 'Observe', 'Observed', 'Unobserve'];
const shapeIdentity = shape => shape.kind === 'enum' ? `enum:${shape.enumName}` : shape.kind;
const byName = (a, b) => a.name < b.name ? -1 : a.name > b.name ? 1 : 0;
export function methodIdentity(module, receiver, method) {
  return `${module}:${receiver.kind}:${receiver.exportName}.${method.member}(` +
    method.parameters.map(p => shapeIdentity(p.shape) + (p.optional ? '?' : '')).join(',') +
    `)->${shapeIdentity(method.returns)}`;
}
// point_script names do not always match the native schema names. Keep this
// mapping explicit so new entity exports cannot silently create unrelated types.
export const entityClasses = {
  Entity: 'CBaseEntity',
  BaseModelEntity: 'CBaseModelEntity',
  CSWeaponBase: 'CCSWeaponBase',
  C4: 'CC4',
  CSGrenadeProjectileBase: 'CBaseCSGrenadeProjectile',
  CSPlantedC4: 'CPlantedC4',
  CSHostage: 'CHostage',
  CSRadarPoint: 'CCSRadarElement',
  CSObservablePoint: 'CCSObservableElement',
  CSPlayerController: 'CCSPlayerController',
  CSObserverPawn: 'CCSObserverPawn',
  CSPlayerPawn: 'CCSPlayerPawn',
  CustomHudLayout: 'CCSCustomHudLayout',
  PointTemplate: 'CPointTemplate',
  CustomPlayerCamera: 'CCSCustomPlayerCamera'
};
// Preserve existing schema properties and native helpers. Only conflicting
// script names receive a suffix; descriptor names and hashes remain unchanged.
const entityMemberNames = {
  Entity: { IsValid: 'IsValidScript', Remove: 'RemoveScript' },
  BaseModelEntity: { SetModel: 'SetModelScript' },
  CSHostage: { IsRescued: 'IsRescuedScript' },
  CSPlayerController: { IsBot: 'IsBotScript' },
  CSPlayerPawn: { IsScoped: 'IsScopedScript' }
};
export function emit(manifest, schemaClasses = entityClasses) {
  const lines = ['// <auto-generated />', '#nullable enable', 'using System;', 'namespace CounterStrikeSharp.API.Modules.CsScript;', ''];
  const entityLines = ['// <auto-generated />', '#nullable enable', 'using CounterStrikeSharp.API.Modules.CsScript;', 'namespace CounterStrikeSharp.API.Core;', ''];
  const type = s => ({ number: 'double', string: 'string', boolean: 'bool', void: 'void', enum: s.enumName })[s.kind];
  for (const e of manifest.enums) {
    if (e.deprecated) lines.push('[Obsolete]');
    lines.push(`public enum ${e.name} : long`, '{', ...e.members.map(m => `    ${ident(m.name)} = ${m.value}L,`), '}', '');
  }
  const descriptors = [];
  const identities = new Map();
  for (const receiver of manifest.receivers) {
    const isEntity = receiver.kind === 'entity';
    const name = isEntity ? schemaClasses[receiver.exportName] : `CsScript${receiver.exportName}`;
    if (!name) throw new Error(`Missing schema class mapping: ${receiver.exportName}`);
    const target = isEntity ? entityLines : lines;
    if (isEntity) target.push(`public partial class ${name}`, '{');
    else target.push(`public sealed class ${name}`, '{', '    private readonly CsScriptRuntime runtime;',
      `    internal ${name}(CsScriptRuntime runtime) { this.runtime = runtime; }`);
    const emitted = new Set();
    for (const method of receiver.methods.filter(m => m.supported)) {
      const identity = methodIdentity(manifest.module, receiver, method);
      const id = nativeHash(identity);
      if (identities.has(id)) throw new Error(`Method hash collision ${hex(id)}: ${identities.get(id)} / ${identity}`);
      identities.set(id, identity);
      if (method.maximumArgumentCount > 24) throw new Error(`ScriptContext argument capacity exceeded: ${identity}`);
      const descriptor = { id, identity, receiver: receiver.exportName, receiverKind: receiver.kind, ...method };
      descriptors.push(descriptor);
      // Retain all native contracts and hashes, but let C# inherit methods from
      // the corresponding schema base class instead of hiding them on each child.
      if (isEntity && method.declaringReceiver !== receiver.exportName) continue;
      const member = isEntity ? entityMemberNames[receiver.exportName]?.[method.member] ?? method.member : method.member;
      for (let argc = method.minimumArgumentCount; argc <= method.maximumArgumentCount; argc++) {
        const params = method.parameters.slice(0, argc);
        const key = `${member}(${params.map(p => type(p.shape)).join(',')})`;
        if (emitted.has(key)) throw new Error(`C# overload collision: ${receiver.exportName}.${key}`);
        emitted.add(key);
        if (method.documentation) target.push('    /// <summary>', ...method.documentation.split('\n').map(l => '    /// ' + l.replaceAll('&', '&amp;').replaceAll('<', '&lt;').replaceAll('>', '&gt;')), '    /// </summary>');
        target.push(`    public ${type(method.returns)} ${ident(member)}(${params.map(p => `${type(p.shape)} ${ident(p.name)}`).join(', ')})`);
        target.push('    {', `        using var call = ${isEntity ? 'CsScriptRuntime.Shared' : 'runtime'}.BeginInvoke(${hex(id)}u, ${isEntity ? 'EntityHandle.Raw' : 'uint.MaxValue'});`);
        for (const p of params) target.push(`        call.Push(${p.shape.kind === 'enum' ? '(double)' : ''}${ident(p.name)});`);
        target.push('        call.Invoke();');
        if (method.returns.kind !== 'void') {
          const result = method.returns.kind === 'string' ? 'call.StringResult()' :
            `call.Result<${method.returns.kind === 'enum' ? 'double' : type(method.returns)}>()`;
          target.push(`        return ${method.returns.kind === 'enum' ? `(${type(method.returns)})` : ''}${result};`);
        }
        target.push('    }');
      }
    }
    target.push('}', '');
  }
  const schema = JSON.stringify({ protocol: 'ScriptContext-singleton-v2', operations, enums: manifest.enums.map(e => ({
    name: e.name, members: [...e.members].sort(byName)
  })).sort(byName), descriptors: descriptors.map(d => ({
    id: d.id, identity: d.identity, minimumArgumentCount: d.minimumArgumentCount, maximumArgumentCount: d.maximumArgumentCount
  })).sort((a, b) => a.id - b.id) });
  const hash = createHash('sha256').update(schema).digest('hex');
  const words = hash.match(/.{16}/g);
  lines.push('internal enum CsScriptOperation : uint', '{', ...operations.map(op => `    ${op} = ${hex(nativeHash('CS_SCRIPT_' + op.toUpperCase()))}u,`), '}',
    'internal static class CsScriptSchema', '{', `    internal const uint Native = ${hex(nativeHash('CS_SCRIPT'))}u;`,
    ...words.map((word, i) => `    internal const ulong Word${i} = 0x${word}UL;`), '}');
  const kind = shape => ({ number: 'Number', enum: 'Number', string: 'String', boolean: 'Boolean', void: 'Void' })[shape.kind];
  const native = ['// <auto-generated />', '#pragma once', '#include <array>', '#include <cstdint>',
    'namespace counterstrikesharp::cs_script {',
    `inline constexpr std::array<uint64_t, 4> SchemaHash = {${words.map(w => `0x${w}ULL`).join(', ')}};`,
    'enum class Operation : uint32_t {', ...operations.map(op => `    ${op} = ${hex(nativeHash('CS_SCRIPT_' + op.toUpperCase()))}u,`), '};',
    'enum class ValueKind { Void, Number, Boolean, String };',
    'struct MethodDescriptor { uint32_t id; const char* receiver; const char* member; bool entity; int minimumArguments; int maximumArguments; std::array<ValueKind, 24> parameters; ValueKind result; };',
    `inline constexpr std::array<MethodDescriptor, ${descriptors.length}> Descriptors = {{`,
    ...descriptors.map(d => `    {${hex(d.id)}u, ${literal(d.receiver)}, ${literal(d.member)}, ${d.receiverKind === 'entity'}, ${d.minimumArgumentCount}, ${d.maximumArgumentCount}, {${d.parameters.map(p => `ValueKind::${kind(p.shape)}`).join(', ')}}, ValueKind::${kind(d.returns)}},`),
    '}};', '}', ''].join('\n');
  return { csharp: lines.join('\n')+'\n', entities: entityLines.join('\n')+'\n', native, descriptors };
}

if (process.argv[1] && path.resolve(process.argv[1]) === fileURLToPath(import.meta.url)) {
  const reference = path.join(root, 'docs/references/cs_script/master');
  const manifest = manifestFrom(path.join(reference, 'tsconfig.json'), path.join(reference, 'point_script.d.ts'));
  const expected = JSON.parse(fs.readFileSync(path.join(reference, 'source.json'))).files.find(f => f.path === 'point_script.d.ts');
  // Git checkouts may normalize line endings; the metadata refers to upstream LF bytes.
  if (manifest.source.gitBlobSha !== expected.git_blob_sha) throw new Error('Declaration hash differs from recorded master source');
  const output = emit(manifest);
  const files = new Map([
    ['tools/cs-script-generator/generated/manifest.json', JSON.stringify(manifest, null, 2)+'\n'],
    ['managed/CounterStrikeSharp.API/Modules/CsScript/Generated/Api.g.cs', output.csharp],
    ['managed/CounterStrikeSharp.API/Modules/CsScript/Generated/Entities.g.cs', output.entities],
    ['src/core/cs_script_descriptors.h', output.native]
  ]);
  for (const [name, contents] of files) {
    const dest = path.join(root, name);
    if (process.argv.includes('--check')) {
      if (!fs.existsSync(dest) || fs.readFileSync(dest, 'utf8').replaceAll('\r\n','\n') !== contents) throw new Error(`Stale output: ${name}`);
    } else { fs.mkdirSync(path.dirname(dest), { recursive: true }); fs.writeFileSync(dest, contents); }
  }
  console.log(`${output.descriptors.length} scalar methods; ${manifest.diagnostics.length} explicit diagnostics`);
}
