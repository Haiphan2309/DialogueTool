# 💬 Dialogue Tool – Bubble Chat Style Dialogue System

This **Dialogue Tool** helps you build a dialogue system with **bubble-style chat**, inspired by **manga/comic** or **Mario & Luigi**-style RPGs. It automatically adjusts the **position of dialogue text box and direction of the dialogue box arrows** to avoid overlapping with the speaking character and to stay within the camera view.

---

## ✨ Inspiration

- 🎮 Inspired by the **Mario & Luigi RPG series**.
- 🙏 Special thanks to [Indie Wafflus](https://github.com/Wafflus/unity-dialogue-system?tab=readme-ov-file) for his **Unity Graph View-based Dialogue System**, which heavily influenced the implementation of Editor Window part.

---

## 📸 Video demo
[Video demo](https://youtu.be/lzvlNniNu8w)

---

## 📸 Screenshot:
<img width="938" height="525" alt="4" src="https://github.com/user-attachments/assets/d11de7b5-53b4-4440-b1c5-f7f6d309909a" />
<img width="937" height="520" alt="3" src="https://github.com/user-attachments/assets/09577203-8338-47d8-afcd-06a78c040299" />
<img width="1899" height="931" alt="2" src="https://github.com/user-attachments/assets/1bfe53d9-f2fd-48cb-9c63-dd8dfd1bb324" />
<img width="935" height="524" alt="1" src="https://github.com/user-attachments/assets/cf7f3bf6-3888-4f16-b993-f7f19aa757f1" />

<img width="1835" height="893" alt="image" src="https://github.com/user-attachments/assets/57e1ab36-ea6d-41f9-88d8-df2b295c5709" />
<img width="1919" height="1007" alt="image" src="https://github.com/user-attachments/assets/b02e58da-aafc-4a9e-94a9-ba7f35739279" />

---

## Feature

- You can set mutil talking NPCs in a single dialogue.
- The text box automatically adjusts the **position of dialogue text box and direction of the dialogue box arrows** to avoid overlapping with the speaking character and to stay within the camera view.
- Press **TALK** key when talking to skip the dialogue text box.
- Hold **TALK** key to speed up talking speed.
- When talking, if dialogue text meet special letter like **. ? ! ...**, the talking speed will slower then usual.

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

### 1. Attach the `BaseNPC` element to NPC object

- Add the `SampleNPC.cs` script to each NPC GameObject, which inherit from BaseNPC.
- You can customize `SampleNPC.cs` as needed to define how dialogue is triggered (e.g., on key press or collider trigger).

### 2. Assign the Dialogue Asset

- Drag and drop your `SODialogue.asset` (created using the editor) into the `SODialogue` field of the `NPC` component.

### 3. Configure `Talking NPC Datas`

After assigning a `SODialogue`, a new section called `Talking NPC Datas` will appear. The number of elements matches the number of `NPC Group`s in the dialogue graph.

<img width="740" height="612" alt="image" src="https://github.com/user-attachments/assets/76b27dea-b99c-44d0-8c33-4226c3c82c42" />

Each element contains:

- `Size`: The size of the NPC, used to calculate the dialogue bubble’s position to avoid overlap.
- `BaseNPC`: Drag the corresponding NPC object into this field (it must inherit from `BaseNPC`).
- `CenterTransform`: A `Transform` indicating the center point of the NPC’s head or face. It helps position the dialogue box properly (e.g., above the character instead of near their feet, if pivot is at the bottom).

### 4. Dialogue Activation

- By default, you can test the dialogue system by pressing the **Z key**.
- You can customize this in `SampleNPC.cs` or `DialogueManager.cs`.

---


## 📄 License

[MIT](https://github.com/Haiphan2309/DialogueTool?tab=MIT-1-ov-file)

---

## 🤝 Credits

- Developed by Phan Thanh Hai.
- Dialogue System Graph View reference by [Indie Wafflus](https://github.com/Wafflus/unity-dialogue-system)

