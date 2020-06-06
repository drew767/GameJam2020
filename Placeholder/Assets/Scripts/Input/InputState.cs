using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputState : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
    }

    public struct CharacterInputState
    {
        public Vector2 movementDirection;
        public bool jump;
        public bool attack;

        public Vector2 summaryMouseDelta;
    }


    static Vector2 GetInputTranslationDirection()
    {
        Vector2 direction = new Vector2();
        if (Input.GetKey(KeyCode.W))
        {
            direction += Vector2.up;
        }
        if (Input.GetKey(KeyCode.S))
        {
            direction += Vector2.down;
        }
        if (Input.GetKey(KeyCode.A))
        {
            direction += Vector2.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            direction += Vector2.right;
        }
        return direction;
    }

    CharacterInputState m_inputState = new CharacterInputState();

    public CharacterInputState GetInputState()
    {
        return m_inputState;
    }

    public void DropInputState()
    {
        m_inputState = new CharacterInputState();
    }

    Vector3 m_prevMousePosition;

    // Update is called once per frame
    void Update()
    {

        m_inputState.movementDirection = (GetInputTranslationDirection() + m_inputState.movementDirection).normalized;
        m_inputState.attack = m_inputState.attack || Input.GetMouseButtonDown(0);
        m_inputState.jump = m_inputState.jump || Input.GetKeyDown(KeyCode.Space);
        var mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        m_inputState.summaryMouseDelta += mouseMovement * Time.deltaTime;
        m_prevMousePosition = Input.mousePosition;
    }
}
