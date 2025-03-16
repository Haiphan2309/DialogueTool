using UnityEngine;
using System;
using GDC.Enums;
using GDC.Common;
using GDC.Home;

namespace GDC.Events
{
    public class GameEvents
    {
        public static Action<SceneType, System.Action> LOAD_SCENE;
        public static Action<SceneType, System.Action> LOAD_SCENE_ASYNC;
        public static Action<SceneType, System.Action> UNLOAD_SCENE;
        //public static Action<ProfileData> ON_PROFILE_UPDATED;
        public static Action<int, int> ON_HAPPINESS_POINT_UPDATED;
        public static Action<int, int> ON_DECORATION_POINT_UPDATED;
        public static Action<bool> ON_LOADING;
        //public static Action<DecorationItem> ON_CLICK_DECORATION;

        public static Action<bool> TOGGLE_LOW_HEALTH_VFX;
    }
    public class GameConstants
    {
        public static float TRANSITION_TIME = 1f;
        public static float LOADING_TIME = 1f;
    }
}
