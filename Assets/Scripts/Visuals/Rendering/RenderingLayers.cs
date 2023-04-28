/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer.Visuals {

    ///<summary>
    /// Stores a set of rendering layers for easy reference.
    ///<summary>
    public class RenderingLayers {

        #region Rendering Layers

        public string BACKGROUND = "Background";

        public string MIDGROUND = "Midground";

        public string FOREGROUND = "Foreground";

        public string UI = "UI";

        #endregion

        // FOREGROUND

        // Particles.
        // Projectiles.
        // Spikes.

        // Projectiles over spikes.
        public string PARTICLE_RENDERING_LAYER => FOREGROUND;
        public int PARTICLE_RENDERING_ORDER => PROJECTILE_RENDERING_ORDER + 1;

        // Projectiles over spikes.
        public string PROJECTILE_RENDERING_LAYER => FOREGROUND;
        public int PROJECTILE_RENDERING_ORDER => SPIKE_RENDERING_ORDER + 1;

        // Spikes on the 0 of foreground.
        public string SPIKE_RENDERING_LAYER => FOREGROUND;
        public int SPIKE_RENDERING_ORDER = 0;

        // MIDGROUND

        // Orbs.
        // Platforms.
        // Blocks.
        // Character.

        // Orbs over platforms.
        public string ORB_RENDERING_LAYER => MIDGROUND;
        public int ORB_RENDERING_ORDER => PLATFORM_RENDERING_ORDER + 1;

        // Platform over characters.
        public string PLATFORM_RENDERING_LAYER => MIDGROUND;
        public int PLATFORM_RENDERING_ORDER => BLOCK_RENDERING_ORDER + 1;

        // Blocks over orbs.
        public string BLOCK_RENDERING_LAYER => MIDGROUND;
        public int BLOCK_RENDERING_ORDER => CHARACTER_RENDERING_ORDER + 1;

        // Character.
        public string CHARACTER_RENDERING_LAYER => MIDGROUND;
        public int CHARACTER_RENDERING_ORDER = 0;

        // BACKGROUND
        
        // Tileset.
        // Background.

        // Character.
        public string TILE_RENDERING_LAYER => BACKGROUND;
        public int TILE_RENDERING_ORDER => SKY_RENDERING_ORDER + 1;

        // Character.
        public string SKY_RENDERING_LAYER => BACKGROUND;
        public int SKY_RENDERING_ORDER = 0;


    }

}
    