/*
 *  This file is part of CounterStrikeSharp.
 *  CounterStrikeSharp is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  CounterStrikeSharp is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with CounterStrikeSharp.  If not, see <https://www.gnu.org/licenses/>. *
 */

using CounterStrikeSharp.API.Core;

namespace CounterStrikeSharp.API.Modules.Memory;

public partial class MemoryPatch
{
    private readonly string _patchName;

    private readonly string _patchSignature;

    public readonly string Name => _patchName;

    public readonly string Signature => _patchSignature;

    public MemoryPatch(string patchName, string patchSignature)
    {
        _patchName = patchName;
        _patchSignature = patchSignature;
    }

    public bool PerformPatch()
    {
        return NativeAPI.CreateMemoryPatch(_patchName, _patchSignature);
    }

    public void UndoPatch()
    {
        return NativeAPI.UndoMemoryPatch(_patchName);
    }
}