using UnityEngine;
using System.Collections;

public class DummyMetaLodTest : MonoBehaviour
{
    UMetaLod _metalod = new UMetaLod();
    DummyMetaLodTarget _dummy = new DummyMetaLodTarget();

	void Start () {
        _metalod.AddTarget(_dummy);	
	}
	
	void Update () {
        _metalod.Update();
	}
}
