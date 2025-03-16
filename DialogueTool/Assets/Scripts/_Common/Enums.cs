
namespace GDC.Enums
{
    public enum TransitionType
    {
        NONE,
        LEFT,
        RIGHT,
        UP,
        DOWN,
        IN,
        FADE,
    }
    public enum TransitionLoadSceneType
    {
        NEW_SCENE, //Load sang scene moi
        RELOAD_WITH_TRANSITION, //Load lai scene cu nhung van co transition
        RELOAD_WITHOUT_TRANSITION //Load lai scene cu va khong co transition
    }
    
    public enum SceneType
    {
        UNKNOWN = -999,
        MAIN = 0,
        HOME = 1,
        TUTORIAL = 2,
        INTRO = 3,
        
    }
}
