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
public class RecipientFilter : IList<CCSPlayerController>, IMarshalToNative
{
    private readonly List<CCSPlayerController> _recipients = new();

    internal Action? CollectionChanged;

    public RecipientFilter()
    {
    }

    public RecipientFilter(params CCSPlayerController[] slots)
    {
        foreach (var slot in slots)
        {
            Add(slot);
        }
    }

    public RecipientFilter(params int[] slots)
    {
        foreach (var slot in slots)
        {
            Add(slot);
        }
    }

    public RecipientFilter(ulong mask)
    {
        for (var i = 0; i < 64; i++)
        {
            if ((mask & ((ulong)1 << i)) != 0)
            {
                Add(i);
            }
        }
    }

    public IEnumerable<object> GetNativeObject()
    {
        yield return GetRecipientMask();
    }

    public ulong GetRecipientMask()
    {
        ulong recipientMask = 0;
        foreach (var recipient in _recipients)
        {
            recipientMask |= (ulong)1 << recipient.Slot;
        }

        return recipientMask;
    }

    public int IndexOf(CCSPlayerController item)
    {
        return _recipients.IndexOf(item);
    }

    public void Insert(int index, CCSPlayerController item)
    {
        _recipients.Insert(index, item);
        CollectionChanged?.Invoke();
    }

    public void RemoveAt(int index)
    {
        _recipients.RemoveAt(index);
        CollectionChanged?.Invoke();
    }

    public CCSPlayerController this[int index]
    {
        get => _recipients[index];
        set => throw new NotImplementedException();
    }

    public void Add(int slot)
    {
        var player = Utilities.GetPlayerFromSlot(slot);
        if (player == null)
        {
            throw new ArgumentException($"Player with slot {slot} not found");
        }

        _recipients.Add(player);
        CollectionChanged?.Invoke();
    }

    public void AddAllPlayers()
    {
        _recipients.AddRange(Utilities.GetPlayers());
        CollectionChanged?.Invoke();
    }

    public IEnumerator<CCSPlayerController> GetEnumerator()
    {
        return _recipients.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(CCSPlayerController item)
    {
        _recipients.Add(item);
        CollectionChanged?.Invoke();
    }

    public void Clear()
    {
        _recipients.Clear();
        CollectionChanged?.Invoke();
    }

    public bool Contains(CCSPlayerController item)
    {
        return _recipients.Contains(item);
    }

    public void CopyTo(CCSPlayerController[] array, int arrayIndex)
    {
        _recipients.CopyTo(array, arrayIndex);
    }

    public bool Remove(CCSPlayerController item)
    {
        var success = _recipients.Remove(item);
        CollectionChanged?.Invoke();
        return success;
    }

    public int Count => _recipients.Count;
    public bool IsReadOnly => false;

    public static implicit operator RecipientFilter(CCSPlayerController player) => new(player);

}

