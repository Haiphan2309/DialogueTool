using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using GDC.Managers;

namespace GDC.Managers
{
    static public class SaveLoadSystem
    {
        static BinaryFormatter formatter = new BinaryFormatter();

        //public string folderName;
        //public string fileName;
        // Start is called before the first frame update


        // Update is called once per frame
        static public void SaveData(GameDataOrigin gameDataOrigin)
        {
            string folderName = "SaveData";
            string fileName = "Data";
            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }

            FileStream dataFile = File.Create(folderName + "/" + fileName + ".bin"); //"forderName/fileName.bin"

            formatter.Serialize(dataFile, gameDataOrigin);
            dataFile.Close();
            //print("Save data to: " + Directory.GetCurrentDirectory().ToString() + "/" + folderName + "/" + fileName + ".bin");
            //print("Value1 and Value2: " + gameData.sliderValue1 + " " + gameData.sliderValue2);
        }

        static public void LoadData(GameDataOrigin gameDataOrigin)
        {
            string folderName = "SaveData";
            string fileName = "Data";
            if (!Directory.Exists(folderName))
            {
                Logger.Log("Don't have data to load");
                return;
            }

            FileStream dataFile = File.Open(folderName + "/" + fileName + ".bin", FileMode.Open);
            gameDataOrigin = (GameDataOrigin)formatter.Deserialize(dataFile);
       
            SaveLoadManager.Instance.GameDataOrigin = gameDataOrigin;
            dataFile.Close();

        }
    }
}
