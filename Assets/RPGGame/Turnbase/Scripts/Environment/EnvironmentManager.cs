using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    public Bounds recycleBounds;
    public EnvironmentObject[] spawningObjects;
    public float moveSpeed = 5;
    public bool isPause;
    public bool spawnOnAwake;
    private bool isSpawned;
    private readonly List<EnvironmentObject> objects = new List<EnvironmentObject>();

    private void Awake()
    {
        if (spawnOnAwake)
            SpawnObjects();
    }

    private void Update()
    {
        if (isPause || !isSpawned)
            return;

        foreach (var obj in objects)
        {
            obj.TempTransform.position += Vector3.left * moveSpeed * Time.deltaTime;
        }

        var firstObj = objects[0];
        var lastObj = objects[objects.Count - 1];
        if (!firstObj.GetBounds().Intersects(recycleBounds))
        {
            objects.RemoveAt(0);
            objects.Add(firstObj);
            var oldPosition = firstObj.TempTransform.position;
            var lastPosition = lastObj.TempTransform.position;
            firstObj.TempTransform.position = new Vector3(lastPosition.x + lastObj.GetBounds().size.x, oldPosition.y, oldPosition.z);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(recycleBounds.center, recycleBounds.size);
    }

    public void SpawnObjects()
    {
        for (var i = objects.Count - 1; i >= 0; --i)
        {
            var obj = objects[i];
            if (obj == null)
                continue;
            Destroy(obj.gameObject);
        }
        objects.Clear();
        isSpawned = true;
        var spawnOffsetX = recycleBounds.min.x;
        float spawnY = recycleBounds.max.y;
        float spawnZ = recycleBounds.center.z;
        Bounds tempBounds;
        while (true)
        {
            var randomSpawn = spawningObjects[Random.Range(0, spawningObjects.Length)];
            var spawnObject = Instantiate(randomSpawn);
            spawnObject.TempTransform.position = new Vector3(spawnOffsetX, spawnY, spawnZ);
            spawnObject.gameObject.SetActive(true);
            objects.Add(spawnObject);
            tempBounds = spawnObject.GetBounds();
            spawnOffsetX += tempBounds.size.x;
            // If current environment is outside of recycle bounds
            if (!tempBounds.Intersects(recycleBounds))
                break;
            // If current environment bounds is bigger than recycle bounds
            if (tempBounds.Contains(new Vector3(recycleBounds.min.x, tempBounds.center.y, tempBounds.center.z)) &&
                tempBounds.Contains(new Vector3(recycleBounds.max.x, tempBounds.center.y, tempBounds.center.z)))
                break;
        }
    }
}
