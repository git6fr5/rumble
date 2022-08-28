/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Platformer.
using Platformer.Utilites;
using Platformer.Decor;
using Platformer.Rendering;
using Screen = Platformer.Rendering.Screen;

namespace Platformer.Decor {

    ///<summary>
    ///
    ///<summary>
    public class StepDust : Dust {

        // [SerializeField] protected float m_Speed = 0.75f;   
        
        // protected override void AdjustPosition(float deltaTime) {
        //     m_Sparkles.RemoveAll(thing => thing == null);

        //     for (int i = 0; i < m_Sparkles.Count; i++) {
        //         Vector3 direction = (m_Sparkles[i].transform.position - transform.position).normalized;
        //         Vector3 deltaPosition = direction * m_Speed * deltaTime;
        //         m_Sparkles[i].transform.position += deltaPosition;
        //     }
        // }

    }

}