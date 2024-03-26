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
namespace CounterStrikeSharp.API.Modules.Utils;

/// <summary>
/// A generic filter for determining whom to send message/sounds etc.
/// to and providing a bit of additional state information
/// </summary>
public class CRecipientFilter
{
    private List<int> recipients = new();

    public int GetRecipientCount() => recipients.Count;

    public int GetRecipientByIndex(int index)
    {
        if (index < 0 || index > GetRecipientCount())
            return -1;

        return recipients[index];
    }

    public void AddRecipient(int playerSlot) => recipients.Add(playerSlot);

    internal object[] GetRecipientsArray() => recipients.Cast<object>().ToArray(); // For NativeAPI call
}