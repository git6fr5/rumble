/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Character;

namespace Platformer.Physics {

    public interface IPositionArrayReciever {

        void RecievePositions(Vector3[] positions);

    }

}