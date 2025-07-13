using UnityEngine;
using GDC.Configuration;
using DialogueSystem;

namespace GDC.Managers
{
    public class ConfigManager : MonoBehaviour
    {
        public static ConfigManager Instance {get; private set;}
        public SceneConfig SceneConfig;
        //public TextBoxConfig TextBoxConfig;
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
