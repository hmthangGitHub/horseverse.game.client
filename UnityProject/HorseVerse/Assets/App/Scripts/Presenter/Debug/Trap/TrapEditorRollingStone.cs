using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using RuntimeHandle;
using UnityEngine;

public class TrapEditorRollingStone : TrapEditorBase
{
    private TrainingTrapBall.Entity entity;

    public GameObject Target;
    public GameObject Direction;
    public GameObject Trigger;
    public int TriggerSize;
    public bool TriggerZoneFullBlock;


    void OnEnable()
    {
        RestoreToDefault();
    }

    public void RestoreToDefault()
    {
        OffAll();
        Target.SetActive(true);
    }

    private void OffAll()
    {
        Direction.SetActive(false);
        Trigger.SetActive(false);
        var kd = Direction.GetComponent<RuntimeTransformHandle>();
        if (kd) Destroy(kd);
        var kt = Trigger.GetComponent<RuntimeTransformHandle>();
        if (kd) Destroy(kt);
    }

    public void ActiveDirection()
    {
        OffAll();
        Direction.SetActive(true);
        var comp = Direction.AddComponent<RuntimeTransformHandle>();
        comp.axes = HandleAxes.XZ;
    }

    public void ActiveTrigger()
    {
        OffAll();
        Trigger.SetActive(true);
        var comp = Trigger.AddComponent<RuntimeTransformHandle>();
        comp.axes = HandleAxes.XZ;
    }

    private TrainingTrapBall.Entity getEntity()
    {
        if (entity == default) entity = new TrainingTrapBall.Entity();
        entity.Target = Position.FromVector3(Target.transform.localPosition);
        entity.Direction = Position.FromVector3(Direction.transform.localPosition);
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
        entity = TrainingTrapBall.Entity.Parse(data);
        Target.transform.localPosition = entity.Target.ToVector3();
        Direction.transform.localPosition = entity.Direction.ToVector3();
        Trigger.transform.localPosition = entity.Trigger.ToVector3();
        TriggerSize = entity.TriggerSize;
        TriggerZoneFullBlock = entity.TriggerZoneFullBlock;
        Debug.Log("SET EXTRA " + entity.Trigger.ToVector3());
    }
}
