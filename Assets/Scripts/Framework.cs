using System;
using System.Collections.Generic;

namespace Framework 
{
    public  enum TileState
    {
        Hidden = 0,
        Revealed = 1,
        Processing = 2,
        Matched  = 3
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

    [Serializable] 
    public class GameData
    {
        public int currentLevelIndex;
        public int currentRunScore;
        public int highScore;

        public InLevelState currentLevelState;
    }

    [Serializable]
    public class InLevelState
    {
        public int matchesFound;
        public int turnsTaken;
        public List<TileSaveData> tileData;
    }

    [Serializable]
    public class TileSaveData
    {
        public string spriteId;
        public TileState state;
    }
}
