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

using System;

namespace CounterStrikeSharp.API.Modules.Listeners
{
    public partial class Listeners
    {
        public delegate void SourceEventHandler(object e);

        public delegate void SourceEventHandler<T>(T e);

        public class MapStartArgs : EventArgs
        {
            public string MapName { get; set; }
        }

        public class PlayerArgs : EventArgs
        {
            public int PlayerSlot { get; internal set; }
            public bool Cancel { get; set; }
            public string CancelReason { get; set; }
        }

        public class EntityArgs : EventArgs
        {
            public int EntityIndex { get; internal set; }
            public string Classname { get; set; }
        }

        public class PlayerConnectArgs : EventArgs
        {
            public int PlayerIndex { get; internal set; }
            public string Name { get; internal set; }
            public string Address { get; internal set; }
            public bool Cancel { get; set; }
            public string CancelReason { get; set; }
        }
    }
}