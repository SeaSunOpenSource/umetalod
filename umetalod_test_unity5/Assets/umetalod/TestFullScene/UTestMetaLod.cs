using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MetaLodTargetGameObject : IMetaLodTarget
{
    public static Vector3 PlayerPos;

    // getters
    public float GetDistance() { return Vector2.Distance(
        new Vector2(PlayerPos.x, PlayerPos.z),
        new Vector2(gameObject.transform.position.x, gameObject.transform.position.z)); 
    }
    public float GetFactorBounds() { return 10.0f; }
    public float GetFactorGeomComplexity() { return 0.8f; }
    public float GetFactorPSysComplexity() { return 0; }
    public float GetFactorVisualImpact() { return 0.5f; }
    public float GetUserFactor(string factorName) { return 0.0f; }

    public void SetLiveness(float liveness) { Liveness = liveness; }

    public void DebugOutput(string fmt, params object[] args)
    {
        string output = string.Format("{0}: {1}", getName(), string.Format(fmt, args));
        Debug.Log(output);
    }

    // dummy helpers
    public string getName() { return gameObject.name; }

    public float Liveness;
    public GameObject gameObject;
}


public class UTestPrototypes
{
    public static List<GameObject> Prototypes = new List<GameObject>();

    public static void Init()
    {
        GameObject go = GameObject.Find("Prototypes");
        if (go != null)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                GameObject prototype = go.transform.GetChild(i).gameObject;
                Prototypes.Add(prototype);
            }

            Debug.Log(go.transform.childCount.ToString() + " prototypes found.");
        }
    }

    public static MetaLodTargetGameObject NewRandom()
    {
        int i = Random.Range(0, Prototypes.Count - 1);
        if (Prototypes[i] == null)
            return null;

        MetaLodTargetGameObject target = new MetaLodTargetGameObject();
        target.gameObject = Object.Instantiate(Prototypes[i]) as GameObject;
        return target;
    }
}

public class UTestMetaLod
{
    public Vector2 Min = new Vector2(-300, -300);
    public Vector2 Max = new Vector2(300, 300);
    public Rect Bound { get { return new Rect(Min.x, Min.y, Max.x - Min.x, Max.y - Min.y); } }

    public bool Init()
    {
        UTestPrototypes.Init();

        m_instRoot = GameObject.Find("Instances");
        if (m_instRoot == null)
            return false;

        m_metalod = new UMetaLod();
        for (int i = 0; i < 3000; i++)
        {
            MetaLodTargetGameObject target = UTestPrototypes.NewRandom();
            if (target != null)
            {
                target.gameObject.transform.localPosition = NewRandomPoint();
                target.gameObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                target.gameObject.transform.localRotation = Quaternion.Euler(0.0f, Random.Range(0.0f, 360.0f), 0.0f);
                target.gameObject.transform.parent = m_instRoot.transform;
                m_metalod.AddTarget(target);
            }
        }

        return true;
    }

    public void Update(Vector3 position)
    {
        MetaLodTargetGameObject.PlayerPos = position;
        m_metalod.Update();

        foreach (var target in m_metalod.Targets)
        {
            MetaLodTargetGameObject tgo = target as MetaLodTargetGameObject;
            Vector3 pos = tgo.gameObject.transform.position;
            Rect rect = new Rect() { };
            rect.center = new Vector2(pos.x, pos.z);
            rect.size = new Vector2(5.0f, 5.0f);
            UCore.DrawRect(rect, 10.0f, new Color(tgo.Liveness, 0, 0, 1));
        }
    }

    public Vector3 NewRandomPoint(float magnitude = 1.0f)
    {
        return new Vector3(
            Random.Range(Min.x, Max.x) * magnitude,
            0,
            Random.Range(Min.y, Max.y) * magnitude);
    }

    public UMetaLod MetaLod { get { return m_metalod; } }

    GameObject m_instRoot = null;
    UMetaLod m_metalod;
}

