using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Platformer.Rendering;
using Screen = Platformer.Rendering.Screen;

[RequireComponent(typeof(GridRenderer))]
public class GridTester : MonoBehaviour {

    [SerializeField] private GridRenderer m_GridRenderer;

    private Vector2 forceOrigin;

    public float pressBuffer = 0.3f;
    public float pressTicks = 0f;


    // Start is called before the first frame update
    void Start() {
        m_GridRenderer = GetComponent<GridRenderer>();
    }

    // Update is called once per frame
    void Update() {
        ClickToInteract();
        MoveToInteract();
    }

    private Vector3 prevMousePos;
    public float factor = 5000f;

    private void MoveToInteract() {

        Vector3 mousePos = (Vector3)(Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (m_GridRenderer.MainGrid != null) {
            print("Explosive");
            m_GridRenderer.MainGrid.ApplyCounterClockwiseForce(factor * (mousePos - prevMousePos).sqrMagnitude / (Time.deltaTime * Time.deltaTime), mousePos, 0.5f);
        }

        prevMousePos = mousePos;
    }

    private void ClickToInteract() {
        Vector3 mousePos = (Vector3)(Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos -= Camera.main.transform.position;
        mousePos.z = 0f;

        if (Input.GetMouseButton(0) && pressTicks <= 0f) {
            if (m_GridRenderer.MainGrid != null) {
                m_GridRenderer.MainGrid.ApplyCounterClockwiseForce(5000f, mousePos, 10f);
            }
            pressTicks = pressBuffer;
        }

        if (Input.GetMouseButton(1) && pressTicks <= 0f) {
            if (m_GridRenderer.MainGrid != null) {
                m_GridRenderer.MainGrid.ApplyClockwiseForce(5000f, mousePos, 10f);
            }
            pressTicks = pressBuffer;
        }

        if (pressTicks > 0f) {
            pressTicks -= Time.deltaTime;
        }
        else {
            pressTicks = 0f;
        }
    }
}
