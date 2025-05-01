using System;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem
{
    ///////////////////////////////////////////////////////////// Pivot config
    public enum TextBoxType
    {
        NORMAL,
        LOUD,
        THINKING,
    }

    public enum ArrowType
    {
        NONE,
        UP,
        DOWN,
        LEFT,
        RIGHT,
    }

    [Serializable] 
    public class TextBoxArrowPositionConfig
    {
        public TextBoxType TextBoxType;
        public ArrowType ArrowType;
        public float DegreeZ;
        public Vector2 AnchorPos;
        public Vector2 AnchorMax; //It's for anchor (neo) in UI canvas
        public Vector2 AnchorMin;
    }

    [Serializable]
    public class TextBoxArrowSpriteConfig
    {
        public TextBoxType TextBoxType;
        public Sprite NormalSprite;
        public Sprite LeanSprite;
    }

    [Serializable]
    public class TextBoxArrowConfig
    {
        [SerializeField] private List<TextBoxArrowPositionConfig> _textBoxArrowPositionConfigs;
        [SerializeField] private List<TextBoxArrowSpriteConfig> _textBoxArrowSprtieConfigs;
        public float ArrowSize;
        public float PaddingBorder;
        public float MutiplePaddingLeanObject;

        public TextBoxArrowPositionConfig GetArrowPositionConfig(TextBoxType textBoxType, ArrowType arrowType)
        {
            foreach (var config in _textBoxArrowPositionConfigs)
            {
                if (config.ArrowType == arrowType && config.TextBoxType == textBoxType)
                {
                    return config;
                }
            }
            return null;
        }

        public TextBoxArrowSpriteConfig GetArrowSpriteConfig(TextBoxType textBoxType)
        {
            foreach (var config in _textBoxArrowSprtieConfigs)
            {
                if (config.TextBoxType == textBoxType)
                {
                    return config;
                }
            }
            return null;
        }
    }

    [Serializable]
    public class TextBoxSpriteConfig
    {
        public TextBoxType TextBoxType;
        public Sprite TextBoxSprite;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////// Text box config

    //[Serializable]
    //public class PreferHorizontalConfig //Using for prefer custom size of textbox, not dynamic follow text length
    //{
    //    public int MinTextLength;
    //    public float HorizontalSize;
    //}

    [Serializable]
    public class ChoosingTextBoxConfig
    {
        public UITextBoxChoice UITextBoxChoicePrefab;
    }

    [Serializable]
    [CreateAssetMenu(menuName = "Config/DialogueSystem/TextBoxConfig")]
    public class TextBoxConfig : ScriptableObject
    {
        public Vector2 MinSize;
        public Vector2 MaxSize;

        [SerializeField] private TextBoxArrowConfig _textBoxArrowConfig;
        public TextBoxArrowConfig TextBoxArrowConfig
        {
            get => _textBoxArrowConfig;
        }

        [SerializeField] private ChoosingTextBoxConfig _choosingTextBoxConfig;
        public ChoosingTextBoxConfig ChoosingTextBoxConfig
        {
            get => _choosingTextBoxConfig;
        }

        [SerializeField] private List<TextBoxSpriteConfig> _textBoxSprtieConfigs;

        public TextBoxSpriteConfig GetTextBoxSpriteConfig(TextBoxType textBoxType)
        {
            foreach (var config in _textBoxSprtieConfigs)
            {
                if (config.TextBoxType == textBoxType)
                {
                    return config;
                }
            }
            return null;
        }

        //[Header("Sort ascending by minTextLength")]
        //[SerializeField] private List<PreferHorizontalConfig> m_preferHorizontalConfigs;
        //public List<PreferHorizontalConfig> PreferHorizontalConfigs
        //{
        //    get => m_preferHorizontalConfigs;
        //    private set => m_preferHorizontalConfigs = value;
        //}

        //public float GetPreferHorizontalSize(string text)
        //{
        //    float result = MinSize.x;
        //    foreach (var config in m_preferHorizontalConfigs)
        //    {
        //        if (text.Length > config.MinTextLength)
        //        {
        //            result = config.MinTextLength;
        //        }
        //        else
        //        {
        //            break;
        //        }
        //    }
        //    return result;
        //}
    }
}
