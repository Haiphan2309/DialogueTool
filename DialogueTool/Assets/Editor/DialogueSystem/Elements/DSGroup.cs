using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using DialogueSystem.Data;

namespace DialogueSystem.Windows
{
    public class DSGroup : Group
    {
        public DSGroupData GroupData { get; set; }

        protected DSGraphView _graphView;

        public DSGroup(Vector2 position, DSGraphView graphView)
        {
            Setup(position, graphView);
            Draw();
        }

        protected void Setup(Vector2 position, DSGraphView graphView)
        {
            _graphView = graphView;
            GroupData = new DSGroupData();
            SetPosition(new Rect(position, Vector2.zero));
        }

        protected void Draw()
        {
            title = "New NPC";

            Label titleLabel = this.Q<Label>();
            titleLabel.style.backgroundColor = new Color(0.2f, 0.6f, 1f, 0.3f);
        }

        public void SaveData()
        {
            GroupData.Name = title;
            GroupData.Position = GetPosition().position;
        }

        public void LoadData(DSGroupData groupData)
        {
            SetPosition(new Rect(groupData.Position, Vector2.zero));
            title = groupData.Name;
            GroupData = groupData;
        }

        public void ReupdateNameByIndex()
        {
            title = "NPC " + GroupData.Index;
        }
    }
}
