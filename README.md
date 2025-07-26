# 💬 Dialogue Tool – Bubble Chat Style Dialogue System

This **Dialogue Tool** helps you build a dialogue system with **bubble-style chat**, inspired by **manga/comic** or **Mario & Luigi**-style RPGs. It automatically adjusts the **position and direction of the dialogue box arrows** to avoid overlapping with the speaking character and to stay within the camera view.

---

## ✨ Inspiration

- 🎮 Inspired by the **Mario & Luigi RPG series**.
- 🙏 Special thanks to [Indie Wafflus](https://github.com/Wafflus/unity-dialogue-system?tab=readme-ov-file) for his **Unity Graph View-based Dialogue System**, which heavily influenced the implementation of Editor Window part.

---

## 📸 Video demo
[Video demo](https://youtu.be/lzvlNniNu8w)

---

## 📸 Screenshot:
<img width="1029" height="579" alt="image" src="https://github.com/user-attachments/assets/572ac4d0-da61-48e4-a5f4-c87a2cfb252c" />

<img width="1035" height="577" alt="image" src="https://github.com/user-attachments/assets/6d16ac7c-e35a-4d68-a19f-fcffd5bc65ce" />

<img width="1033" height="574" alt="image" src="https://github.com/user-attachments/assets/3d9d9a06-a478-4f27-a772-43cb6514e896" />

<img width="1037" height="575" alt="image" src="https://github.com/user-attachments/assets/df7244ec-34aa-46f0-8680-bca825dd9d09" />

<img width="1835" height="893" alt="image" src="https://github.com/user-attachments/assets/57e1ab36-ea6d-41f9-88d8-df2b295c5709" />

<img width="1919" height="1007" alt="image" src="https://github.com/user-attachments/assets/b02e58da-aafc-4a9e-94a9-ba7f35739279" />

---

## 🛠️ How to Use

### 🔍 Editor Window Access

Open the dialogue editor from the Unity top menu:  
`Window > DialogueEditor`

---

## 🧩 Components Overview

### 🟢 Start Node

- Represents the **starting point** of the dialogue flow.
- Should connect to the **first dialogue node**.

### 💬 Dialogue Node

Represents a single **dialogue entry**, with the following properties:

- `Dialogue Text`: The actual text shown in the bubble.
- `Text Box Type`: Defines the type of the bubble:
  - `NORMAL`: Standard dialogue.
  - `LOUD`: For shouting or strong expressions.
  - `THINKING`: Thoughtful/inner monologue style.
- `Emotion`: Represents the NPC's emotion during this line.
- `Add Choice`: Allows branching dialogue options. If added, players will be able to **choose responses** leading to different dialogue paths (or to the end).

### 👥 NPC Group

- Groups nodes together to represent a **single NPC**.
- Dialogue nodes inside a group are considered spoken by that NPC.
- **Ungrouped nodes** are treated as **system or neutral dialogue**.

---

## 🧭 Top Bar Functions

- `Dialogue Name`: Input the name of the current dialogue.
- `Save`: Saves the current dialogue as a `ScriptableObject`. You can configure the save path in the code.
- `Load`: Loads an existing dialogue by name.
- `Rename All Elements`: Automatically renames all nodes and components in the Graph View with sequential numbering.

---

## 📂 Saving & Loading

- Saved dialogues are stored as `ScriptableObject` assets.
- You can configure the **save directory** in the relevant part of the source code in DSUtils.cs.

```C#
const string assetFolderName = "DialogueDatas";
const string assetPath = "Assets/DialogueSystem/Resources/" + assetFolderName;
```

---

## 🧪 Inspector Setup

To use the dialogue tool in your scene, follow these steps:

### 1. Attach the `NPC.cs` Script

- Add the `NPC.cs` script to each NPC GameObject.
- You can customize `NPC.cs` as needed to define how dialogue is triggered (e.g., on key press or collider trigger).

### 2. Assign the Dialogue Asset

- Drag and drop your `SODialogue.asset` (created using the editor) into the `SODialogue` field of the `NPC` component.

### 3. Configure `talkingNPCDatas`

After assigning a `SODialogue`, a new section called `Talking NPC Datas` will appear. The number of elements matches the number of `NPC Group`s in the dialogue graph.

<img width="740" height="612" alt="image" src="https://github.com/user-attachments/assets/76b27dea-b99c-44d0-8c33-4226c3c82c42" />

Each element contains:

- `Size`: The size of the NPC, used to calculate the dialogue bubble’s position to avoid overlap.
- `BaseNPC`: Drag the corresponding NPC object into this field (it must inherit from `BaseNPC`).
- `CenterTransform`: A `Transform` indicating the center point of the NPC’s head or face. It helps position the dialogue box properly (e.g., above the character instead of near their feet, if pivot is at the bottom).

### 4. Dialogue Activation

- By default, you can test the dialogue system by pressing the **Z key**.
- You can customize this in `NPC.cs` or `DialogueManager.cs`.

---


## 📄 License

[MIT](https://github.com/Haiphan2309/DialogueTool?tab=MIT-1-ov-file)

---

## 🤝 Credits

- Developed by Phan Thanh Hai.
- Dialogue System Graph View reference by [Indie Wafflus](https://github.com/Wafflus/unity-dialogue-system)

