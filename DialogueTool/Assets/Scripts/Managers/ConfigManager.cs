using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDC.Configuration;

namespace GDC.Managers
{
    public class ConfigManager : MonoBehaviour
    {
        public static ConfigManager Instance {get; private set;}
        public SceneConfig SceneConfig;
        //public ItemsConfig ItemsConfig;
        //public MapConfig MapConfig;
        //public StreetPatrolConfig PatrolConfig;
        //public PlayerStatConfig PlayerStatConfig;
        //public CitizenConfig CitizenConfig;
        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
