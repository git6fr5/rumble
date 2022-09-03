/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Platformer.UI;

namespace Platformer.UI {

    ///<summary>
    ///
    ///<summary>
    public class WorldButton : Button {

        public int index;

        public override void Activate() {
            SceneManager.LoadScene("World " + index.ToString());
        }

    }
}