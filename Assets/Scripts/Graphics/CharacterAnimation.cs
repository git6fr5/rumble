// /* --- Libraries --- */
// // System.
// using System.Collections;
// using System.Collections.Generic;
// // Unity.
// using UnityEngine;
// using Gobblefish.Animation;

// namespace Platformer.Animation {

//     [System.Serializable]
//     public class CharacterAnimation {
        
//         public float fps;
//         public List<Sprite[]> legSprites;
//         public Sprite[] bodySprites;

//         [HideInInspector] 
//         public float ticks;
//         public float duration => fps == 0f ? 0f : (float)sprites.Length / fps; 

//         public void Tick(float dt) {
//             ticks += Time.fixedDeltaTime;
//         }

//         public float t_leg(int i) {
//             return (ticks * fps) % (float)legSprites[i].Length;
//         }

//         public Sprite GetLegFrame(int i) {
//             if (i < legSprites.Count) {
//                 return legSprites[i][(int)Mathf.Floor(t_leg(i))];
//             }
//         }

//         public Sprite GetBody() {
//             return bodySprites.Length
//         }

//         public int GetOffset() {
//             return baseOffset + orderOffset;
//         }

//     }

// }
