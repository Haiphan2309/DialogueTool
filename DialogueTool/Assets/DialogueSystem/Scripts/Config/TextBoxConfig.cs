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
    public enum PivotType
    {
        NONE,
        UP,
        DOWN,
        LEFT,
        RIGHT,
    }
    [Serializable]
    public class TextBoxPivotPositionConfig
    {
        public TextBoxType TextBoxType;
        public PivotType PivotType;
        public float DegreeZ;
        public Vector2 AnchorPos;
        public Vector2 AnchorMax; //It's for anchor (neo) in UI canvas
        public Vector2 AnchorMin;
    }

    [Serializable]
    public class TextBoxPivotSpriteConfig
    {
        public TextBoxType TextBoxType;
        public Sprite NormalSprite;
        public Sprite LeanSprite;
    }

    [Serializable]
    public class TextBoxPivotConfig
    {
        [SerializeField] private List<TextBoxPivotPositionConfig> m_textBoxPivotPositionConfigs;
        [SerializeField] private List<TextBoxPivotSpriteConfig> m_textBoxPivotSprtieConfigs;
        public float PivotSize;

        public TextBoxPivotPositionConfig GetTextBoxPivotPositionConfig(TextBoxType textBoxType, PivotType pivotType)
        {
            foreach (var config in m_textBoxPivotPositionConfigs)
            {
                if (config.PivotType == pivotType && config.TextBoxType == textBoxType)
                {
                    return config;
                }
            }
            return null;
        }

        public TextBoxPivotSpriteConfig GetTextBoxPivotSpriteConfig(TextBoxType textBoxType)
        {
            foreach (var config in m_textBoxPivotSprtieConfigs)
            {
                if (config.TextBoxType == textBoxType)
                {
                    return config;
                }
            }
            return null;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////// Text box config

    [Serializable]
    public class PreferHorizontalConfig //Using for prefer custom size of textbox, not dynamic follow text length
    {
        public int MinTextLength;
        public float HorizontalSize;
    }

    [Serializable]
    [CreateAssetMenu(menuName = "Config/DialogueSystem/TextBoxConfig")]
    public class TextBoxConfig : ScriptableObject
    {
        public Vector2 MinSize;
        public Vector2 MaxSize;
        [SerializeField] private TextBoxPivotConfig m_textBoxPivotConfig;
        public TextBoxPivotConfig TextBoxPivotConfig 
        {
            get => m_textBoxPivotConfig;
            private set => m_textBoxPivotConfig = value;
        }

        [Header("Sort ascending by minTextLength")]
        [SerializeField] private List<PreferHorizontalConfig> m_preferHorizontalConfigs;
        public List<PreferHorizontalConfig> PreferHorizontalConfigs
        {
            get => m_preferHorizontalConfigs;
            private set => m_preferHorizontalConfigs = value;
        }

        public float GetPreferHorizontalSize(string text)
        {
            float result = MinSize.x;
            foreach (var config in m_preferHorizontalConfigs)
            {
                if (text.Length > config.MinTextLength)
                {
                    result = config.MinTextLength;
                }
                else
                {
                    break;
                }
            }
            return result;
        }
    }
}
