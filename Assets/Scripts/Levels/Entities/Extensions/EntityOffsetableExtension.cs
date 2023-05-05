/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// Platformer.
using Platformer.Levels.LDtk;
using Platformer.Levels.Entities;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;

namespace Platformer.Levels.Entities {

    public interface IOffsetable {
        void SetOffset(int offset);
    }

    public static class EntityOffsetableExtensions {

        public static void SetOffset(this Entity entity, int index, List<LDtkTileData> controlData) {
            IOffsetable offsetable = entity.GetComponent<IOffsetable>();
            if (offsetable == null) {
                return;
            }
            
            offsetable.SetOffset(controlData[index].vectorID.x);

        }

    }
}