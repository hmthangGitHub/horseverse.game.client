using Newtonsoft.Json;
using RuntimeHandle;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrapEditorWoodSpike : TrapEditorBase
{
    private TrainingTrapWoodSpike.Entity entity;

    public GameObject Direction;
    public List<GameObject> Points;
    public GameObject Trigger;
    public int TriggerSize;
    public bool TriggerZoneFullBlock;
    public GameObject PointPrefab;


    private GameObject _currentPoint = default;
    public GameObject CurrentPoint => _currentPoint;

    void OnEnable()
    {
        RestoreToDefault();
    }

    public void RestoreToDefault()
    {
        OffAll();
    }

    private void OffAll()
    {
        Direction.SetActive(false);
        Trigger.SetActive(false);
    }

    public void ActiveDirection()
    {
        OffAll();
        Direction.SetActive(true);
    }

    public void ActiveTrigger()
    {
        OffAll();
        Trigger.SetActive(true);
        var comp = Trigger.AddComponent<RuntimeTransformHandle>();
        comp.axes = HandleAxes.XZ;
    }

    public void AddNewPoint()
    {
        var point = AddPoint("point", new Vector3(0, 0, -2));
        Points.Add(point);
    }

    public void DeleteSelectedPoint()
    {
        if (_currentPoint != default)
        {
            Points.Remove(_currentPoint);
            Destroy(_currentPoint);
            _currentPoint = default;
        }
    }

    public void SelectPoint(int index)
    {
        if (index < Points.Count)
        {
            if (_currentPoint == Points[index]) return;
            if (_currentPoint != default)
            {
                var x = _currentPoint.GetComponent<RuntimeTransformHandle>();
                Destroy(x);
            }
            _currentPoint = Points[index];
            var comp = _currentPoint.AddComponent<RuntimeTransformHandle>();
            comp.axes = HandleAxes.XZ;
        }
    }

    private TrainingTrapWoodSpike.Entity getEntity()
    {
        if (entity == default) entity = new TrainingTrapWoodSpike.Entity();
        entity.Direction = new List<Position>();
        entity.Direction.AddRange(Points.Select( x => Position.FromVector3(x.transform.localPosition)).ToArray());
        entity.Trigger = Position.FromVector3(Trigger.transform.localPosition);
        entity.TriggerSize = TriggerSize;
        entity.TriggerZoneFullBlock = TriggerZoneFullBlock; Debug.Log("Save Data " + entity.Trigger.ToVector3());
        return entity;
    }

    public override string GetExtraData()
    {
        return JsonConvert.SerializeObject(getEntity()).Replace(",", "..."); ;
    }

    public void SetExtraData(string data)
    {
        //ClearPoints();
        entity = TrainingTrapWoodSpike.Entity.Parse(data);
        Trigger.transform.localPosition = entity.Trigger.ToVector3();
        TriggerSize = entity.TriggerSize;
        TriggerZoneFullBlock = entity.TriggerZoneFullBlock;
        OnChangeNumberOfPoint(entity.Direction.Count, entity.Direction.Select(x=>x.ToVector3()).ToArray());
        Debug.Log("SET EXTRA " + entity.Trigger.ToVector3());
    }

    public void ClearPoints()
    {
        foreach (Transform s in Direction.transform) {
            GameObject.Destroy(s.gameObject);
        }
        Points.Clear();
    }

    public void OnChangeNumberOfPoint(int number, Vector3[] positions)
    {
        if (number < 0) return;
        if (Points.Count > number)
        {
            var destroyPoints = Points.Skip(number).Take(Points.Count - number).ToArray();
            destroyPoints.ForEach(x => Object.Destroy(x));
            Points = Points.Except(destroyPoints)
                               .ToList();
        }
        else if (Points.Count < number)
        {
            Points.AddRange(Enumerable.Range(0, number - Points.Count)
                                        .Select(x =>
                                        {
                                            var point = AddPoint("point");
                                            return point;
                                        }));
        }

        for ( int i =0; i < Points.Count; i++)
        {
            Points[i].transform.localPosition = positions[i];
        }
    }

    private GameObject AddPoint(string name)
    {
        var point = Instantiate(PointPrefab, Direction.transform);
        point.name = name;
        return point;
    }

    private GameObject AddPoint(string name, Vector3 pos)
    {
        var point = AddPoint(name);
        point.transform.localPosition = pos;
        return point;
    }
}
