// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;

namespace Platformer {

    [System.Serializable]
    public class PlayerSettings : Gobblefish.Settings<PlayerSettings> {
        
        public int deaths = 0;
        public int points = 0;
        
    }

}