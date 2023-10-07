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

namespace CounterStrikeSharp.API
{
    public class StringTable
    {
        private int _index;

        public StringTable(int index)
        {
            _index = index;
        }

        public void Add(string value, string userdata = null)
        {
            NativeAPI.AddToStringTable(_index, value, userdata);
        }

        public int FindStringIndex(string search)
        {
            return NativeAPI.FindStringIndex(_index, search);
        }

        public string GetUserData(int stringIndex)
        {
            return NativeAPI.GetStringTableData(_index, stringIndex);
        }

        public int GetUserDataLength(int stringIndex)
        {
            return NativeAPI.GetStringTableDataLength(_index, stringIndex);
        }

        public int MaxStringNum => NativeAPI.GetStringTableMaxStrings(_index);
        public string Name => NativeAPI.GetStringTableName(_index);

        public int NumStrings => NativeAPI.GetStringTableNumStrings(_index);

        public string Read(int stringIndex) => NativeAPI.ReadStringTable(_index, stringIndex);

        public void SetUserData(int stringIndex, string value) =>
            NativeAPI.SetStringTableData(_index, stringIndex, value);

    }

    public class StringTables
    {
        private static StringTable _downloadsTable = null;
        public static StringTable Find(string search)
        {
            var index = NativeAPI.FindStringTable(search);
            if (index > -1) return new StringTable(index);

            return null;
        }

        public static int NumberOfTables => NativeAPI.GetNumStringTables();

        public static bool Lock(bool @lock) => NativeAPI.LockStringTables(@lock);

        public static void AddFileToDownloadsTable(string filename)
        {
            if (_downloadsTable == null)
            {
                _downloadsTable = Find("downloadables");
            }

            bool save = Lock(false);
            _downloadsTable.Add(filename);
            Lock(save);
        }

    }
}