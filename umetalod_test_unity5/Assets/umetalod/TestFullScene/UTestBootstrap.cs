using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class UTestBootstrap : MonoBehaviour
{
    UTestMetaLod m_testbed = new UTestMetaLod();

    GameObject m_player = null;
    GameObject m_moveTarget = null;

    bool _alwaysMove = false;
    bool _drawDebugLines = false;

    void Start()
    {
        m_testbed.Init();
        m_player = GameObject.Find("Player");
        m_moveTarget = GameObject.Find("MoveTarget");
    }

    void Update()
    {
        if (_drawDebugLines)
            UCore.DrawRect(m_testbed.Bound, 0.1f, Color.white);

        Vector3 target = m_moveTarget.transform.position;
        Vector3 dist = target - m_player.transform.position;
        if (dist.magnitude > 1.0f)
        {
            Vector3 delta = dist.normalized * Time.deltaTime * 100.0f;
            if (delta.magnitude > dist.magnitude)
            {
                m_player.transform.position = target;
            }
            else
            {
                m_player.transform.position += delta;
            }
        }
        else
        {
            if (_alwaysMove)
            {
                SetNewTarget();
            }
        }

        m_testbed.Update(m_player.transform.position);
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(50, Screen.height * 0.1f - 40, 80, 40), "Move"))
        {
            SetNewTarget();
        }

        _alwaysMove = GUI.Toggle(new Rect(50, Screen.height * 0.2f, 100, 20), _alwaysMove, "Always Move");
    }

    void SetNewTarget()
    {
        m_moveTarget.transform.position = m_testbed.NewRandomPoint(0.8f);
    }
}
