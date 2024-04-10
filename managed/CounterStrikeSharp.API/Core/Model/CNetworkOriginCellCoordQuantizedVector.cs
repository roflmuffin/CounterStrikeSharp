using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Core;

public partial class CNetworkOriginCellCoordQuantizedVector
{
    private const int CELL_WIDTH = 1 << 9;

    public Vector Vector => new(
        (CellX - 32) * CELL_WIDTH + X,
        (CellY - 32) * CELL_WIDTH + Y,
        (CellZ - 32) * CELL_WIDTH + Z);
}
