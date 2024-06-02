/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.Events;
// Gobblefish.
using Gobblefish.Audio;
// Platformer.
using Platformer.Physics;

namespace Platformer.Entities.Utility {

    public class StaticAlternator : MonoBehaviour {

        public enum AlternatingType {
            A, B, C, D, Count,
        }

        public enum AlternatingState {
            Stable, Changing
        }

        [System.Serializable]
        public class AlternatingEntity {
            public Entity entity;
            public int index;
            // public bool flipped;
            public int offset;
            public AlternatingEntity(Entity e, int x, int y) { 
                this.entity=e; 
                this.index=x; 
                this.offset=y;
                // this.flipped= y == 0 ? false : true;
            }
        }

        [System.Serializable]
        public class GeneratedAlternatingGroup {
            public Timer timer;
            public List<AlternatingEntity> ents;
            public bool currState;
        }

        [System.Serializable]
        public class AlternatingSettings {

            public int index;
            public float period;
            public int offsetRange;
            
            public AudioSnippet preChangeSound;
            public AudioSnippet changeSound;

            // public Timer changeTimer = new Timer(0f, 0f);
            // public Timer prechangeTimer = new Timer(0f, 0f);
            // public Timer halfChangeTimer = new Timer(0f, 0f);
            // public Timer prehalfChangeTimer = new Timer(0f, 0f);


            // Dictionary<Timer, List<AlternatingEntity>> entityDict = new Dictionary<Timer, List<AlternatingEntity>>();

            public List<GeneratedAlternatingGroup> generatedGroups = new List<GeneratedAlternatingGroup>();

            // bool on;

            public void Start(List<AlternatingEntity> entityList) {
                // entityDict = new Dictionary<Timer, List<AlternatingEntity>>();
                generatedGroups = new List<GeneratedAlternatingGroup>();

                offsetRange = 1;
                foreach (AlternatingEntity e in entityList) {
                    if (e.index == index && e.offset >= offsetRange) {
                        offsetRange = e.offset + 1;
                    }
                }

                print("Alternator " + offsetRange.ToString());

                for (int i = 0; i < offsetRange; i++) {

                    Timer timer = new Timer(0f, 0f);
                    float r = 2 * (float)i / (float)offsetRange;

                    bool startOff = r >= 1f;
                    if (startOff) {
                        r = r % 1;
                    }

                    timer.Start(period * r);

                    List<AlternatingEntity> forthistimer = entityList.FindAll(e => e.index == index && e.offset == i);
                    GeneratedAlternatingGroup g = new GeneratedAlternatingGroup();
                    g.timer=timer;
                    g.ents = forthistimer;
                    g.currState = !startOff;
                    generatedGroups.Add(g);
                    
                    // entityDict.Add(timer, forthistimer);

                    // foreach (AlternatingEntity e in forthistimer) {
                    //     e.entity.gameObject.SetActive(!startOff);
                    // }

                }

            }

            public void Update(float dt) {

                foreach (var group in generatedGroups) {

                    bool changeFin = group.timer.TickDown(dt) || !group.timer.Active;

                    if (changeFin) {
                        group.currState = !group.currState;
                        group.timer.Start(period);
                    }

                    foreach (AlternatingEntity e in group.ents) {
                        if (e.entity.gameObject.activeSelf != group.currState) {
                            e.entity.gameObject.SetActive(group.currState);
                        }
                    }
                    
                }
                
            }

            // public void X() {

            //     bool changeFin = changeTimer.TickDown(dt) || !changeTimer.Active;
            //     if (changeFin) {
            //         prechangeTimer.Start(PRE_CHANGE_DURATION);
            //         changeTimer.Start(period);

            //         // preChangeSound.Play();
            //     }

            //     bool prechangeFin = prechangeTimer.TickDown(dt);
                
            //     if (prechangeFin) {

            //         List<AlternatingEntity> curatedOn = entityList.FindAll(e => e.index == index && !e.flipped);
            //         List<AlternatingEntity> curatedOff = entityList.FindAll(e => e.index == index && e.flipped);

            //         foreach (AlternatingEntity e in curatedOn) {
            //             e.entity.gameObject.SetActive(on);
            //         }
            //         foreach (AlternatingEntity e in curatedOff) {
            //             e.entity.gameObject.SetActive(!on);
            //         }
            //         on = !on;

            //         // changeSound.Play();
            //     }
            // }
        }

        public List<AlternatingEntity> entityList = new List<AlternatingEntity>();

        public List<AlternatingSettings> settingsList = new List<AlternatingSettings>();

        // The duration before changing that we indicate a change is happening.
        private const float PRE_CHANGE_DURATION = 0.3f;

        public int index;
        public int offIndex => (index + (int)Mathf.Floor((float)(int)AlternatingType.Count / 2f)) % (int)AlternatingType.Count;

        void Start() {
            foreach (AlternatingSettings s in settingsList) {
                s.Start(entityList);
            }
        }
        
        public void Refresh() {
            entityList = new List<AlternatingEntity>();
        }


        public void Add(Entity entity, int x, int y) {
            if (entity == null) { return; }
            entityList.Add(new AlternatingEntity(entity, x, y));
        }


        void FixedUpdate() {

            float dt = Time.fixedDeltaTime;
            foreach (AlternatingSettings s in settingsList) {
                s.Update(dt);
            }
            
        }

    }

}
