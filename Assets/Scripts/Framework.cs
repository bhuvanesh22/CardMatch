using System;
using System.Collections.Generic;

namespace Framework 
{
    public  enum TileState
    {
        Hidden = 0,
        Revealed = 1,
        Processing = 2,
        Mathced  = 3
    }

    public enum TileType
    {
        Empty = 0,
        Tappable = 1,
        Blocker = 2
    }

    [Serializable]
    public class LevelRow
    {
        public List<TileType> tiles;
    }
}
