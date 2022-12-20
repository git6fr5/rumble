// TODO: Clean

/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

using Platformer.Utilites;
using Platformer.Obstacles;
using Platformer.Character;
using Platformer.Rendering;
using Screen = Platformer.Rendering.Screen;

namespace Platformer.Obstacles {

    ///<summary>
    ///
    ///<summary>
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class ResetOrb : Orb {

        [SerializeField] private Orb m_DashOrb;
        [SerializeField] private Orb m_HopOrb;
        [SerializeField] private Orb m_GhostOrb;
        [SerializeField] private Orb m_ShadowOrb;
        [SerializeField] private Orb m_StickyOrb;

        void Awake() {
            m_Palette = Screen.DefaultPalette;
        }

        void Update() {

            CharacterState state = Game.MainPlayer;
            if (state.Dash.Enabled) {
                m_Palette = m_DashOrb.Palette;
                m_Type = Type.DashOrb;
            }
            else if (state.Hop.Enabled) {
                m_Palette = m_HopOrb.Palette;
                m_Type = Type.HopOrb;
            }
            else if (state.Ghost.Enabled) {
                m_Palette = m_GhostOrb.Palette;
                m_Type = Type.GhostOrb;
            }
            else if (state.Shadow.Enabled) {
                m_Palette = m_ShadowOrb.Palette;
                m_Type = Type.ShadowOrb;
            }
            else if (state.Sticky.Enabled) {
                m_Palette = m_StickyOrb.Palette;
                m_Type = Type.StickyOrb;
            }
            else {
                m_Palette = Screen.DefaultPalette;
                m_Type = Type.None;
            }

        }
        
    }
}