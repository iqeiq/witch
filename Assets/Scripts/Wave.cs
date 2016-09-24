using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;


public class Wave : MonoBehaviour {

    public GameObject[] prefabs;

    public void Load(int wave)
    {
        var filename = String.Format("stage/stage{0:D2}", wave); 
        var textAsset = Resources.Load(filename) as TextAsset;

        Debug.AssertFormat(textAsset != null, "{0} is not found.", filename);

        var jsonText = textAsset.text;
        JsonNode json = JsonNode.Parse(jsonText);

        //Debug.LogFormat("MP: {0}", json["mp"].Get<long>());
        var enemy = json["enemy"].Select(e => 
            new KeyValuePair<long, Vector2>(
                e["type"].Get<long>(),
                Camera.main.ViewportToWorldPoint(new Vector2(
                    (float)e["x"].Get<double>(),
                    (float)e["y"].Get<double>()
                ))
            )
        );
        Spawn(enemy);
    }

    void Spawn(IEnumerable<KeyValuePair<long, Vector2>> enemy)
    {
        foreach(var e in enemy)
        {
            var g = Instantiate(prefabs[e.Key], e.Value, Quaternion.identity) as GameObject;
            g.transform.parent = transform;
        }
    }
	
	public bool isFinish() {
        return !(transform.childCount > 0);
    }
}
