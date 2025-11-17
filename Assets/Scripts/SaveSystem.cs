using Framework;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveSystem 
{
    static readonly string SAVE_FILENAME = "gamedata.json";

    static string GetSavePath()
    {
        return Path.Combine ( Application.persistentDataPath, SAVE_FILENAME );
    }

    public static void SaveGame(GameData data)
    {
        string savePath = GetSavePath();
        try
        {
            string json = JsonUtility.ToJson ( data, true );
            File.WriteAllText ( savePath, json );
            Debug.Log ( $"Game Saved Successfully at {savePath}" );
        }
        catch (System.Exception ex)
        {
            Debug.LogError ( $"Failed to save game data: {ex.Message}" );
        }
    }

    public static GameData LoadGame()
    {
        string savePath = GetSavePath ();

        if(!File.Exists ( savePath ))
        {
            Debug.Log ( "No save file found." );
            return null;
        }

        try
        {
            string json = File.ReadAllText ( savePath );
            GameData data = JsonUtility.FromJson<GameData> ( json );
            Debug.Log ( $"Game loaded successfully from: {savePath}" );
            return data;
        }
        catch ( System.Exception e )
        {
            Debug.LogError ( $"Failed to load game data: {e.Message}" );
            return null;
        }
    }

    public static void DeleteSaveFile( )
    {
        string savePath = GetSavePath () ;

        if ( File.Exists ( savePath ) )
        {
            try
            {
                File.Delete ( savePath );
                Debug.Log ( "Save file deleted." );
            }
            catch ( System.Exception e )
            {
                Debug.LogError ( $"Failed to delete save file: {e.Message}" );
            }
        }
    }
}
