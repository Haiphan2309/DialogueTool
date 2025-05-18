using UnityEngine;

namespace DialogueSystem.Windows
{
    public class DSStartNode : DSNode
    {
        public DSStartNode(Vector2 position, DSGraphView graphView)
        {
            Setup(position, _graphView);
            Draw();
        }

        protected override void Draw()
        {
            base.Draw();

            mainContainer.style.backgroundColor = new Color(0.1f, 0.5f, 1f, 0.4f);

            title = "Start";
            inputContainer.Clear();
            extensionContainer.Clear();
        }
    }
}
