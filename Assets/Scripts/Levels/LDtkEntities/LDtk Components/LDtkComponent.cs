/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.U2D;
using UnityExtensions;

namespace Platformer.Levels {

    ///<summary>
    ///
    ///<summary>
    public abstract class LDtkComponent : MonoBehaviour {

        public abstract void Init(LDtkEntity entity, List<LDtkTileData> entityData, LDtkTileData controlTile, List<LDtkTileData> controlData);

    }

}
