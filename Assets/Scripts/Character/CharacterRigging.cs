/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.U2D;
using UnityExtensions;

/* --- Definitions --- */
using SpriteSkin = UnityEngine.U2D.Animation.SpriteSkin;

namespace Platformer.Character {

    ///<summary>
    /// An abstract class defining the functionality of a
    /// character's action. 
    ///<summary>
    public class CharacterRigging : MonoBehaviour {

        public SpriteSkin m_SpriteSkin;

        public const float FREQUENCY = 5f;
        public const float DAMPING_RATIO = 0.3f;
        public const float COLLIDER_SIZE = 0.05f;
        public const float MASS = 1f;
        public const float GRAVITY_SCALE = 1f;

        void Start() {

            int count = m_SpriteSkin.boneTransforms.Length;

            for (int i = 0; i < count; i++) {
                
                Rigidbody2D body = m_SpriteSkin.boneTransforms[i].gameObject.AddComponent<Rigidbody2D>();
                body.gravityScale = GRAVITY_SCALE;
                body.mass = MASS;
                // body.constraints = RigidbodyConstraints2D.FreezeRotation;

                CircleCollider2D circle = m_SpriteSkin.boneTransforms[i].gameObject.AddComponent<CircleCollider2D>();
                circle.radius = COLLIDER_SIZE;

            }

            for (int i = 0; i < count; i++) {
                
                
                SpringJoint2D left = m_SpriteSkin.boneTransforms[i].gameObject.AddComponent<SpringJoint2D>();
                SpringJoint2D right = m_SpriteSkin.boneTransforms[i].gameObject.AddComponent<SpringJoint2D>();                
                SpringJoint2D across = m_SpriteSkin.boneTransforms[i].gameObject.AddComponent<SpringJoint2D>();                

                int k = (i+1) % count;
                int j = (i-1);
                if (j < 0) {
                    j += count; 
                }

                int n = (i + count / 2) % count;


                left.connectedBody = m_SpriteSkin.boneTransforms[k].GetComponent<Rigidbody2D>();
                right.connectedBody = m_SpriteSkin.boneTransforms[j].GetComponent<Rigidbody2D>();
                across.connectedBody = m_SpriteSkin.boneTransforms[n].GetComponent<Rigidbody2D>();

                left.dampingRatio = DAMPING_RATIO;
                right.dampingRatio = DAMPING_RATIO;
                across.dampingRatio = DAMPING_RATIO;

                left.frequency = FREQUENCY;
                right.frequency = FREQUENCY;
                across.frequency = FREQUENCY / 2f;


            }
        }

    }

}

