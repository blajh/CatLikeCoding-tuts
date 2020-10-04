using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{

    [SerializeField]
    Transform pointPrefab = default;

	[SerializeField, Range(10, 100)]
	int resolution = 10;

	[SerializeField]
	FunctionLibrary.FunctionName function = default;

	Transform[] points;

	private Transform point = default;

    void Awake()
    {
		points = new Transform[resolution * resolution];
		float step = 2f / resolution;
        var position = Vector3.zero;
		var scale = Vector3.one * step;
		for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++)
		{
            if (x == resolution) {
				x = 0;
				z += 1;
            }
			Transform point = Instantiate(pointPrefab);
			position.x = (x + 0.5f) * step - 1f;
			position.z = (z + 0.5f) * step - 1f;
			point.localPosition = position;
			point.localScale = scale;
			point.SetParent(transform, false);
			points[i] = point;
		}
	}

    void Update()
    {
		FunctionLibrary.Function f = FunctionLibrary.GetFunction(function);
		float time = Time.time;

		for (int i = 0; i < points.Length; i++)
        {
			Transform point = points[i];
			Vector3 position = point.localPosition;
			position.y = f(position.x, position.z, time);
			point.localPosition = position;
        }    
    }
}
