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
using System.Collections.Generic;
using System.Text;

namespace CounterStrikeSharp.API
{
    [Flags]
    public enum ConVarFlags
    {
        None = 0,
        Unregistered = (1 << 0),
        DevelopmentOnly = (1 << 1),
        Notify = (1 << 8),
        Replicated = (1 << 13)
    }

    public enum ConCommandFlags
    {
        FCVAR_LINKED_CONCOMMAND = (1 << 0),

        FCVAR_DEVELOPMENTONLY =
            (1 << 1), // Hidden in released products. Flag is removed automatically if ALLOW_DEVELOPMENT_CVARS is defined.
        FCVAR_GAMEDLL = (1 << 2), // defined by the game DLL
        FCVAR_CLIENTDLL = (1 << 3), // defined by the client DLL
    }
}