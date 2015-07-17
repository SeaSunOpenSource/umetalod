using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct UImpactFactor
{
    public string Name;
    public float Weight;
    public float UpperBound;
    public float LowerBound;

    public float Calculate(float value)
    {
        return 0.0f;
    }
}

public class UHeatAttenuation
{
    public float DistInnerBound;
    public float DistOuterBound;

    public int PerfLevel;
    public int FrameRate;

    public float CalculateLiveness(float distance)
    {
        return 0.0f;
    }
}

public interface IMetaLodTarget
{
    float GetDistance();

    float GetFactorBounds();
    float GetFactorGeomComplexity();
    float GetFactorPSysComplexity();
    float GetFactorVisualImpact();
    float GetUserFactor(string factorName);

    void SetLiveness(float liveness);
}

public class UMetaLod 
{
    public void AddTarget(IMetaLodTarget target)
    {
        if (!_targets.Contains(target))
        {
            _targets.Add(target);
        }
    }

    public void AddUserFactor(UImpactFactor userFactor)
    {
        if (!_userFactors.Contains(userFactor))
        {
            _userFactors.Add(userFactor);
        }
    }

    public void UpdateTargets()
    {
        foreach (var target in _targets)
        {
            UpdateLiveness(target);
        }
    }

    private void UpdateLiveness(IMetaLodTarget target)
    {
        float distance = target.GetDistance();

        float factorRate = 0.0f;
        factorRate += _factorBounds.Calculate(target.GetFactorBounds());
        factorRate += _factorBounds.Calculate(target.GetFactorBounds());
        factorRate += _factorBounds.Calculate(target.GetFactorBounds());
        factorRate += _factorBounds.Calculate(target.GetFactorBounds());

        foreach (var factor in _userFactors)
        {
            factorRate += factor.Calculate(target.GetUserFactor(factor.Name));
        }

        distance *= 1.0f + factorRate;

        float liveness = _heatAtte.CalculateLiveness(distance);
        target.SetLiveness(liveness);
    }

    private HashSet<IMetaLodTarget> _targets = new HashSet<IMetaLodTarget>();
    private UHeatAttenuation _heatAtte = new UHeatAttenuation();
    
    private HashSet<UImpactFactor> _userFactors;

    private UImpactFactor _factorBounds = new UImpactFactor();
    private UImpactFactor _factorGeomComplexity = new UImpactFactor();
    private UImpactFactor _factorPSysComplexity = new UImpactFactor();
    private UImpactFactor _factorVisualImpact = new UImpactFactor();
}
