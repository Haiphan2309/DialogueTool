using System.Collections.Generic;
using UnityEngine;
using System;
using GDC.Configuration;
using GDC.Enums;
using GDC.Constants;
using Unity.VisualScripting;

namespace GDC.Managers
{
    [Serializable]
    public struct GameData
    {
        public bool IsSaveLoadProcessing;

        //todo: Them cac du lieu trong game o day, co the them ca du lieu phuc tap
        //public int coin;
        //public string playerName;

        public void SetupData() //load, chuyen doi du lieu GameDataOrigin sang GameData
        {
            IsSaveLoadProcessing = true;
            GameDataOrigin gameDataOrigin = SaveLoadManager.Instance.GameDataOrigin;

            //coin = gameDataOrigin.coin;
            //playerName = gameDataOrigin.playerName;

            SaveLoadManager.Instance.GameDataOrigin = gameDataOrigin;
            IsSaveLoadProcessing = false;
        }
        public GameDataOrigin ConvertToGameDataOrigin() //save, chuyen doi du lieu GameData sang GameDataOrigin
        {
            IsSaveLoadProcessing = true;
            GameDataOrigin gameDataOrigin = new GameDataOrigin();

            //gameDataOrigin.coin = coin;
            //gameDataOrigin.playerName = playerName;

            IsSaveLoadProcessing = false;
            return gameDataOrigin;
        }

        #region support function
        
        #endregion
    }

    [Serializable]
    public struct GameDataOrigin
    {
        public bool IsHaveSaveData;
        //Todo: Them cac du lieu tuong ung GameData o day, chi cac du lieu co ban
        //public int coin;
        //public string playerName;
    }

    [Serializable]
    public struct CacheData
    {
        // Them cac du lieu tam thoi o day
    }
}
