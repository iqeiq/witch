using UnityEngine;
using System.Linq;
using UniRx;


public class Wave : MonoBehaviour {

    [SerializeField]
    private GameObject[] prefabs;

    public void Load(int wave)
    {
        var filename = string.Format("stage/stage{0:D2}", wave); 
        var textAsset = Resources.Load(filename) as TextAsset;

        Debug.AssertFormat(textAsset != null, "{0} is not found.", filename);

        var jsonText = textAsset.text;
        JsonNode json = JsonNode.Parse(jsonText);

        //Debug.LogFormat("MP: {0}", json["mp"].Get<long>());
        json["enemy"].Select(e => new Tuple<long, Vector2>(
            e["type"].Get<long>(),
            Camera.main.ViewportToWorldPoint(new Vector2(
                (float)e["x"].Get<double>(),
                (float)e["y"].Get<double>()
            ))
        )).Select(e => 
            Instantiate(prefabs[e.Item1], e.Item2, Quaternion.identity) as GameObject
        ).ToList().ForEach(g => {
            g.transform.parent = transform;
        });
    }
    
	public bool isFinish() {
        return !(transform.childCount > 0);
    }
}
