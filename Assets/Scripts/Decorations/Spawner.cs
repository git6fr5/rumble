/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityExtensions;

namespace Platformer.LevelEditing {

    ///<summary>
    ///
    ///<summary>
    public class Spawner : MonoBehaviour {

        [SerializeField]
        private GameObject m_SpawnObject;

        [SerializeField]
        private float m_SpawnRadius = 1f;

        [SerializeField]
        private float m_MinSpawnTime;

        [SerializeField]
        private float m_MaxSpawnTime;

        [SerializeField]
        private Timer m_SpawnTimer = new Timer(0f, 0f);

        void Start() {
            m_SpawnTimer.Start(Random.Range(m_MinSpawnTime, m_MaxSpawnTime));
        }

        void FixedUpdate() {
            m_SpawnTimer.TickDown(Time.fixedDeltaTime);

            if (!m_SpawnTimer.Active) {
                SpawnObject();
                m_SpawnTimer.Start(Random.Range(m_MinSpawnTime, m_MaxSpawnTime));
            }
        }

        private void SpawnObject() {
            GameObject newObject = Instantiate(m_SpawnObject);
            newObject.transform.position = transform.position + (Vector3)Random.insideUnitCircle * m_SpawnRadius;
            newObject.SetActive(true);
        }

        void OnDrawGizmos() {
            Gizmos.DrawWireSphere(transform.position, m_SpawnRadius);
        }

    }

}