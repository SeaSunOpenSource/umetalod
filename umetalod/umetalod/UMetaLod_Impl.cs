using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class UMetaLod 
{
    private void _initDefault()
    {
        _factorBounds = UMetaLodDefaults.Factor_Bounds;
        _factorGeomComplexity = UMetaLodDefaults.Factor_GeomComplexity;
        _factorPSysComplexity = UMetaLodDefaults.Factor_PSysComplexity;
        _factorVisualImpact = UMetaLodDefaults.Factor_VisualImpact; 
    }

    private float _calculateImpact(UImpactFactor factor, float value)
    {
        float normalized = factor.Normalizer != null ? factor.Normalizer(value, factor.UpperBound, factor.LowerBound) : 0.0f;
        return normalized * Mathf.Clamp01(factor.Weight);
    }

    private bool _updateTime()
    {
        float deltaTime = Time.realtimeSinceStartup - _lastUpdateTime;
        if (deltaTime < UMetaLodConfig.UpdateInterval)
            return false;

        _lastUpdateTime = Time.realtimeSinceStartup;

        // update fps as needed
        float deltaFPSTime = Time.realtimeSinceStartup - _lastUpdateFPSTime;
        if (deltaFPSTime >= UMetaLodConfig.FPSUpdateInterval)
        {
            int passedFrames = Time.frameCount - _lastUpdateFrameCount;
            _currentFPS = (float)passedFrames / deltaTime;
            _lastUpdateFrameCount = Time.frameCount;
        }

        return true;
    }

    private void _updateHeatAttenuation()
    {
        // the magnification calculation is fixed across all targets
        // so that we extract this part and do it once per frame
        UMetaLodConfig.HeatAttenuationFormula(_currentFPS, out _distInnerAttenuated, out _distOuterAttenuated);
    }

    private void _updateTargets()
    {
        foreach (var target in _targets)
        {
            _updateLiveness(target);
        }
    }

    private void _updateLiveness(IMetaLodTarget target)
    {
        // impact factors accumulation
        float factorRate = 1.0f;
        factorRate -= _calculateImpact(_factorBounds, target.GetFactorBounds());
        factorRate -= _calculateImpact(_factorGeomComplexity, target.GetFactorGeomComplexity());
        factorRate -= _calculateImpact(_factorPSysComplexity, target.GetFactorPSysComplexity());
        factorRate -= _calculateImpact(_factorVisualImpact, target.GetFactorVisualImpact());
        foreach (var factor in _userFactors)
            factorRate -= _calculateImpact(factor, target.GetUserFactor(factor.Name));

        // perform liveness calculation
        float distance = target.GetDistance() * factorRate;
        float liveness = 1.0f - UMetaLodUtil.Percent(_distInnerAttenuated, _distOuterAttenuated, distance);
        target.SetLiveness(liveness);

        _dbgPrintToTarget(target, "factorRate: {0}, distInner: {1}, distOuter: {2}, liveness: {3}, (fps: {4})",
            factorRate, _distInnerAttenuated, _distOuterAttenuated, liveness, _currentFPS);
    }

    private UImpactFactor _getSysFactor(string name)
    {
        switch (name)
        {
            case UMetaLodConst.Factor_Bounds: return _factorBounds;
            case UMetaLodConst.Factor_GeomComplexity: return _factorGeomComplexity;
            case UMetaLodConst.Factor_PSysComplexity: return _factorPSysComplexity;
            case UMetaLodConst.Factor_VisualImpact: return _factorVisualImpact;
            default: return new UImpactFactor();
        }
    }

    private void _dbgPrintToTarget(IMetaLodTarget target, string fmt, params object[] args)
    {
        if (UMetaLodConfig.EnableDebuggingOutput)
        {
            target.DebugOutput(fmt, args);
        }
    }

    // impact targets
    private HashSet<IMetaLodTarget> _targets = new HashSet<IMetaLodTarget>();

    // impact factors
    private UImpactFactor _factorBounds;
    private UImpactFactor _factorGeomComplexity;
    private UImpactFactor _factorPSysComplexity;
    private UImpactFactor _factorVisualImpact;
    private HashSet<UImpactFactor> _userFactors = new HashSet<UImpactFactor>();

    // heat attenuation 
    private float _distInnerAttenuated;
    private float _distOuterAttenuated;

    private float _currentFPS;

    // internal states
    private float _lastUpdateTime = 0.0f;
    private float _lastUpdateFPSTime = 0.0f;
    private int _lastUpdateFrameCount = 0;
}
