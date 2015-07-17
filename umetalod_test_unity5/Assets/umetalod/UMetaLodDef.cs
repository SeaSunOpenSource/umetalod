using UnityEngine;
using System.Collections;

public class UMetaLodDef 
{
    // the bounding volume of the target
    public static string Factor_Bounds = "Bounds";

    // currently corresponds to vertex count of the target mesh, would be 0 for particle system
    public static string Factor_GeomComplexity = "GeomComplexity";

    // currently correspends to particle count of the target particle system, would be 0 for ordinary mesh
    public static string Factor_PSysComplexity = "PSysComplexity";  
    
    // a subjective factor which reveals the visual importance of the target in some degrees
    // for instance, skill effects casted by player would generally has a 
    // pretty much higher visual impact than a static stone on the ground
    public static string Factor_VisualImpact = "VisualImpact";

}
