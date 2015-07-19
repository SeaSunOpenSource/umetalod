using UnityEngine;
using System.Collections;

public class DummyMetaLodTarget : IMetaLodTarget
{
    // getters
    public float GetDistance() { return 30.0f; }
    public float GetFactorBounds() { return 10.0f; }
    public float GetFactorGeomComplexity() { return 0.8f; }
    public float GetFactorPSysComplexity() { return 0; }
    public float GetFactorVisualImpact() { return 0.5f; }
    public float GetUserFactor(string factorName) { return 0.0f; }

    // setters
    public void SetLiveness(float liveness) { /* do nothing */ }
    public void DebugOutput(string fmt, params object[] args)
    {
        string output = string.Format("{0}: {1}", getName(), string.Format(fmt, args));
        Debug.Log(output);
    }

    // dummy helpers
    private string getName() { return "_dummy_"; }
}

