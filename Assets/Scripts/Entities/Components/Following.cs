// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;

namespace Platformer.Entities.Components {

    using Platformer.Physics;
    using IReset = Platformer.Entities.Utility.IReset;
    using CharacterController = Platformer.Character.CharacterController;
    // using TrailAnimator = Gobblefish.Animation.TrailAnimator;

    [RequireComponent(typeof(Entity))]
    public class Following : MonoBehaviour, IReset {

        public enum FollowState {
            Still, 
            Following,
            Returning,
        }

        // The weight with which gravity pulls this.
        // private static float WEIGHT = 1f;

        // The body attached to this gameObject 
        private Entity m_Entity;

        // The current fall state of this falling spike.
        [SerializeField]
        private FollowState m_FollowState = FollowState.Still;

        // Whether this transform is following something.
        [SerializeField, ReadOnly] 
        private Transform m_FollowTransform = null;
        public Transform FollowTransform => m_FollowTransform;

        private Transform m_Parent;
        GameObject originObject = null;

        void Awake() {
            m_Parent = transform.parent;
            m_Entity = GetComponent<Entity>();

            if (Application.isPlaying) {
                originObject = new GameObject("origin " + gameObject.name);
                originObject.transform.SetParent(transform.parent);
                originObject.transform.position = transform.position;
            }

        }

        // Initialize the spike.
        void Start() {
            m_FollowState = FollowState.Still;
        }

        // Runs once every fixed interval.
        void FixedUpdate() {

            switch (m_FollowState) {
                case FollowState.Following:
                    WhileFollowing();
                    break;
                case FollowState.Returning:
                    WhileReturning();
                    break;
                default:
                    break;
            }

        }

        public void StartFollowing(CharacterController character) {
            // if (m_Entity.CollisionEnabled && m_FollowState == FollowState.Still) {
            if (m_FollowState == FollowState.Still) {
                OnFollow(character.transform);
                m_FollowState = FollowState.Following;
            }
        }

        private void OnFollow(Transform toFollow) {
            // Index.
            Following[] followingArray = (Following[])GameObject.FindObjectsOfType(typeof(Following));
            Following following = FindFollowing(followingArray, toFollow);
            if (following == null) {
                m_FollowTransform = toFollow;
            }
            else {
                Following cachedFollowing = following;
                while (following != null) {
                    cachedFollowing = following;
                    following = FindFollowing(followingArray, following.transform);
                }
                m_FollowTransform = cachedFollowing.transform;
            }

            transform.SetParent(m_FollowTransform.parent);
            
        }

        // super inefficient.
        public static Following FindFollowing(Following[] following, Transform transform) {
            for (int i = 0; i < following.Length; i++) {
                if (following[i].FollowTransform == transform) { 
                    return following[i];
                }
            }
            return null;
        }

        private void WhileFollowing() { 
            if (m_FollowTransform == null) {
                m_FollowState = FollowState.Returning;
                return;
            }

            Vector3 direction = -(m_FollowTransform.position - transform.position).normalized;
            Vector3 followPosition = m_FollowTransform.position + direction; // (m_Follow.position - transform.position).normalized * m_Index;
            float mag = (followPosition - transform.position).magnitude;
            transform.Move(followPosition, mag * 5f, Time.fixedDeltaTime);
        }

        private void WhileReturning() {
            OnFinishResetting();
        }

        public void OnStartResetting() {
            print("is this being called");
        }

        public void OnFinishResetting() {
            transform.SetParent(m_Parent);
            // m_FollowTransform = originObject.transform;
            
            transform.position = originObject.transform.position;
            m_FollowTransform = null;
            m_FollowState = FollowState.Still;

            // m_FollowState = FollowState.Following;

        }

    }

}
