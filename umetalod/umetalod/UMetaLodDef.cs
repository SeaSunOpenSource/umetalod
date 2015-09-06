using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace umlod
{
    public delegate float fnFactorNormalize(float value, float upper, float lower);
    public delegate void fnHeatAttenuate(float fps, out float distInner, out float distOuter);

    public struct UImpactFactor
    {
        public string Name;
        public float Weight;
        public float UpperBound;
        public float LowerBound;
        public fnFactorNormalize Normalizer;
    }

    public enum UPerfLevel
    {
        Highend,
        Medium,
        Lowend,
    }

    public class UMetaLodConst
    {
        // the bounding volume of the target
        public const string Factor_Bounds = "Bounds";

        // currently corresponds to vertex count of the target mesh, would be 0 for particle system
        public const string Factor_GeomComplexity = "GeomComplexity";

        // currently correspends to particle count of the target particle system, would be 0 for ordinary mesh
        public const string Factor_PSysComplexity = "PSysComplexity";

        // a subjective factor which reveals the visual importance of the target in some degrees
        // for instance, skill effects casted by player would generally has a 
        // pretty much higher visual impact than a static stone on the ground
        public const string Factor_VisualImpact = "VisualImpact";
    }

    public class UMetaLodUtil
    {
        public static float Percent(float from, float to, float value)
        {
            if (float.Equals(from, to))
                return 0.0f;

            float clamped = Mathf.Clamp(value, from, to);
            return (clamped - from) / (to - from);
        }
    }

    public class UMetaLodDefaults
    {
        public static UImpactFactor Factor_Bounds = new UImpactFactor()
        {
            Name = UMetaLodConst.Factor_Bounds,
            Weight = 0.2f,
            UpperBound = 20.0f,
            LowerBound = 5.0f,
            Normalizer = (value, upper, lower) => { return UMetaLodUtil.Percent(lower, upper, value); },
        };

        public static UImpactFactor Factor_GeomComplexity = new UImpactFactor()
        {
            Name = UMetaLodConst.Factor_GeomComplexity,
            Weight = 0.05f,
            UpperBound = 1.0f,
            LowerBound = 0.0f,
            Normalizer = (value, upper, lower) => { return UMetaLodUtil.Percent(lower, upper, value); },
        };

        public static UImpactFactor Factor_PSysComplexity = new UImpactFactor()
        {
            Name = UMetaLodConst.Factor_PSysComplexity,
            Weight = 0.05f,
            UpperBound = 1.0f,
            LowerBound = 0.0f,
            Normalizer = (value, upper, lower) => { return UMetaLodUtil.Percent(lower, upper, value); },
        };

        public static UImpactFactor Factor_VisualImpact = new UImpactFactor()
        {
            Name = UMetaLodConst.Factor_VisualImpact,
            Weight = 0.1f,
            UpperBound = 1.0f,
            LowerBound = 0.0f,
            Normalizer = (value, upper, lower) => { return UMetaLodUtil.Percent(lower, upper, value); },
        };

        public static fnHeatAttenuate HeatAttenuation = (float fps, out float inner, out float outer) =>
        {
            float fpsClamped = Mathf.Clamp(fps, UMetaLodConfig.FpsLowerBound, UMetaLodConfig.FpsUpperBound);
            float fpsScale = 0.0f;
            if (fpsClamped < UMetaLodConfig.FpsStandard)
            {
                fpsScale = UMetaLodConfig.FpsMinifyFactor * (1.0f - UMetaLodUtil.Percent(UMetaLodConfig.FpsLowerBound, UMetaLodConfig.FpsStandard, fpsClamped));
            }
            else
            {
                fpsScale = UMetaLodConfig.FpsMagnifyFactor * (1.0f - UMetaLodUtil.Percent(UMetaLodConfig.FpsStandard, UMetaLodConfig.FpsUpperBound, fpsClamped));
            }

            float overallScale = 1.0f + UMetaLodConfig.PerfLevelScaleLut[UMetaLodConfig.PerformanceLevel] + fpsScale;
            inner = UMetaLodConfig.DistInnerBound * overallScale;
            outer = UMetaLodConfig.DistOuterBound * overallScale;
        };
    }
}

