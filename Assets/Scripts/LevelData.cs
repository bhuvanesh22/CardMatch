using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Data", menuName = " Card Match/Level Data  ")]
public class LevelData : ScriptableObject
{
    public List<LevelRow> gridLayout;

    public Sprite backSprite;

    public List<Sprite> cardSprites;
}
