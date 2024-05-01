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

using System.Collections;

namespace CounterStrikeSharp.API.Modules.Utils;

/// <summary>
/// A generic filter for determining whom to send message/sounds etc.
/// to and providing a bit of additional state information
/// </summary>
public class RecipientFilter : IEnumerable<CCSPlayerController>, IMarshalToNative
{
    public RecipientFilter()
    {
    }

    public RecipientFilter(params CCSPlayerController[] slots)
    {
        foreach (var slot in slots)
        {
            AddRecipient(slot);
        }
    }

    public RecipientFilter(params int[] slots)
    {
        foreach (var slot in slots)
        {
            AddRecipient(slot);
        }
    }

    private readonly List<CCSPlayerController> _recipients = new();

    public CCSPlayerController this[int index] => _recipients[index];

    public void AddRecipient(CCSPlayerController player) => _recipients.Add(player);

    public void AddRecipient(int slot)
    {
        var player = Utilities.GetPlayerFromSlot(slot);
        if (player == null)
        {
            throw new ArgumentException($"Player with slot {slot} not found");
        }

        _recipients.Add(player);
    }

    public void AddAllPlayers()
    {
        _recipients.AddRange(Utilities.GetPlayers());
    }

    public IEnumerator<CCSPlayerController> GetEnumerator()
    {
        return _recipients.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IEnumerable<object> GetNativeObject()
    {
        ulong recipientMask = 0;
        foreach (var recipient in _recipients)
        {
            recipientMask |= (ulong)1 << recipient.Slot;
        }

        yield return recipientMask;
    }
}

