using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace umlod
{
    public struct UMetaLodTargetFactorInfo
    {
        public float FactorRate_Bounds;
        public float FactorRate_GeomComplexity;
        public float FactorRate_PSysComplexity;
        public float FactorRate_VisualImpact;
        public float FactorRate_UserFactors;

        public float GetAccumulated()
        {
            return FactorRate_Bounds + FactorRate_GeomComplexity + FactorRate_PSysComplexity + FactorRate_VisualImpact + FactorRate_UserFactors;
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

        void OutputDebugInfo(ref UMetaLodTargetFactorInfo debugInfo);
    }

    public static class UMetaLodConfig
    {
        // the time interval of an update (could be done discretedly)
        public static float UpdateInterval = 0.5f;

        // the time interval of an FPS update (could be done discretedly)
        public static float FPSUpdateInterval = 5.0f;

        // debug option (would output debugging strings to lod target if enabled)
        public static bool EnableDebuggingOutput = false;

        // performance level (target platform horsepower indication)
        public static UPerfLevel PerformanceLevel = UPerfLevel.Medium;
        // performance level magnifier
        public static Dictionary<UPerfLevel, float> PerfLevelScaleLut = new Dictionary<UPerfLevel, float> 
    {
        { UPerfLevel.Highend, 0.2f },
        { UPerfLevel.Medium, 0.0f },
        { UPerfLevel.Lowend, -0.2f },
    };

        // heat attenuation parameters overriding (including the formula)
        public static float DistInnerBound = 80.0f;
        public static float DistOuterBound = 180.0f;
        public static float FpsLowerBound = 15.0f;
        public static float FpsStandard = 30.0f;
        public static float FpsUpperBound = 60.0f;
        public static float FpsMinifyFactor = -0.2f;
        public static float FpsMagnifyFactor = 0.2f;
        public static fnHeatAttenuate HeatAttenuationFormula = UMetaLodDefaults.HeatAttenuation;
    }

    public partial class UMetaLod
    {
        public UMetaLod()
        {
            _initDefault();
        }

        public void AddTarget(IMetaLodTarget target)
        {
            if (!_targets.Contains(target))
            {
                _targets.Add(target);
            }
        }

        public void RemoveTarget(IMetaLodTarget target)
        {
            if (_targets.Contains(target))
            {
                _targets.Remove(target);
            }
        }

        public void AddUserFactor(UImpactFactor userFactor)
        {
            if (!_userFactors.Contains(userFactor))
            {
                _userFactors.Add(userFactor);
            }
        }

        public void Update()
        {
            if (_updateTime())
            {
                _updateHeatAttenuation();
                _updateTargets();
            }
        }

        public float DistInnerAttenuated
        {
            get { return _distInnerAttenuated; }
        }
        public float DistOuterAttenuated
        {
            get { return _distOuterAttenuated; }
        }

        // accessor to targets
        public HashSet<IMetaLodTarget> Targets { get { return _targets; } }

        // accessor to built-in factors (use this to modify default parameters in these factors)
        public UImpactFactor GetSysFactor(string name) { return _getSysFactor(name); }
    }
}

