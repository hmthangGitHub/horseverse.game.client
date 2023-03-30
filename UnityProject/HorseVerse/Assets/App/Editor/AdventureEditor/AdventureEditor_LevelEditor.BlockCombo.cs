using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Globalization;
using System.Data;

#if UNITY_EDITOR || UNITY_STANDALONE
using SimpleFileBrowser;
#endif

public partial class AdventureEditor_LevelEditor
{
    private readonly List<GameObject> blockComboPinList = new List<GameObject>();
    private MasterHorseTrainingProperty masterHorseTrainingProperty;
    private GameObject currentEditingPlatformObject;

    private MasterHorseTrainingBlockCombo[] blockCombos;
    private MasterTrainingBlockComboType CurrentBlockComboType = MasterTrainingBlockComboType.Modular;
    private int CurrentSelectetBlockCombo = -1;

    private bool isShowBlockComboList = true;

    void InitLevelEditor_BlockCombo() {
        UnSelectOldBlockCombo();
        CurrentSelectetBlockCombo = -1;
        blockCombos = GetCurrentBlockCombo(CurrentBlockComboType);
    }

    void GUI_ListBlockCombo()
    {
        if(GUILayout.Button("Add New Block Combo"))
        {
            OnAddNewBlockCombo();
        }

        if (GUILayout.Button("Save Current Block Combo"))
        {
            OnSaveCurrentBlockCombo();
        }

        if (GUILayout.Button("Cancel"))
        {
            CancelEditBlock();
        }

        if (CurrentBlockComboType == MasterTrainingBlockComboType.Modular)
        {
            EditorGUILayout.LabelField("MasterTrainingBlockComboType: Modular");
        }
        else if (CurrentBlockComboType == MasterTrainingBlockComboType.Predefine)
        {
            EditorGUILayout.LabelField("MasterTrainingBlockComboType: Predifine");
        }
        else if (CurrentBlockComboType == MasterTrainingBlockComboType.Custom)
        {
            EditorGUILayout.LabelField("MasterTrainingBlockComboType: Custom");
        }

        GUILayout.BeginHorizontal(GUILayout.Width(300));

        if(GUILayout.Button("Modular"))
        {
            CurrentBlockComboType = MasterTrainingBlockComboType.Modular;
            InitLevelEditor_BlockCombo();
        }

        if (GUILayout.Button("Predefine"))
        {
            CurrentBlockComboType = MasterTrainingBlockComboType.Predefine;
            InitLevelEditor_BlockCombo();
        }

        if (GUILayout.Button("Custom"))
        {
            CurrentBlockComboType = MasterTrainingBlockComboType.Custom;
            InitLevelEditor_BlockCombo();
        }

        GUILayout.EndHorizontal();
        isShowBlockComboList = EditorGUILayout.BeginFoldoutHeaderGroup(isShowBlockComboList, "List Block Combo");
        if (isShowBlockComboList)
        {
            if (blockCombos != default)
            {

                for (int i = 0; i < blockCombos.Length; i++)
                {
                    var combo = blockCombos[i];
                    GUILayout.BeginHorizontal(GUILayout.Width(300));

                    EditorGUILayout.LabelField(combo.Name);
                    EditorGUILayout.Toggle(CurrentSelectetBlockCombo == i);
                    if (GUILayout.Button("Edit"))
                    {
                        OnEditBlockCombo(i);
                    }
                    GUILayout.EndHorizontal();
                }
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        if (GUILayout.Button("Tiling"))
        {
            if (currentEditingPlatformObject != default)
            {
                var tmp = currentEditingPlatformObject.GetComponent<AdventureEditor_BlockComboData>();
                Tiling(tmp);
            }
        }

    }

    MasterHorseTrainingBlockCombo[] GetCurrentBlockCombo(MasterTrainingBlockComboType currentBlockComboType)
    {
        return masterHorseTrainingBlockComboContainer.DataList.Where(x => x.MasterTrainingBlockComboType == currentBlockComboType).ToArray();
    }


    void OnEditBlockCombo(int i)
    {
        if (CurrentSelectetBlockCombo != -1)
        {
            if (CurrentSelectetBlockCombo != i)
            {
                UnSelectOldBlockCombo();
            }
            else
                return;
        }
        else
        {
            if (currentEditingPlatformObject != default)
                UnSelectOldBlockCombo();
        }

        
        CreatePlatform(blockCombos[i]);

        CurrentSelectetBlockCombo = i;
    }

    void OnAddNewBlockCombo()
    {
        if (CurrentSelectetBlockCombo != -1)
        {
            UnSelectOldBlockCombo();
        }
        CurrentSelectetBlockCombo = -1;
        CreateNewPlatform();
    }

    void OnSaveCurrentBlockCombo()
    {
        if (currentEditingPlatformObject != default)
        {
            if (CurrentSelectetBlockCombo != -1)
            {
                // Save Edit Block
                var editData = blockCombos[CurrentSelectetBlockCombo];
                var data = masterHorseTrainingBlockComboContainer.DataList.Where(o => o.MasterHorseTrainingBlockId == editData.MasterHorseTrainingBlockId).FirstOrDefault();
                var blockComboData = currentEditingPlatformObject.GetComponent<AdventureEditor_BlockComboData>();
                var bnames = blockComboData.paddings.Select(x => x.name).ToArray();
                if (data != default)
                {
                    data.MasterHorseTrainingBlockIdList = bnames;
                    if (blockComboData.startPadding != default)
                        data.SetMasterTrainingModularBlockIdStart(blockComboData.startPadding.name);
                    if (blockComboData.endPadding != default)
                        data.SetMasterTrainingModularBlockIdEnd(blockComboData.endPadding.name);
                }
            }
            else
            {
                // Save New Block
                var blockComboName = currentEditingPlatformObject.name;
                var blockComboData = currentEditingPlatformObject.GetComponent<AdventureEditor_BlockComboData>();

                if (string.IsNullOrEmpty(blockComboName)) return;
                MasterTrainingModularBlockType masterTrainingModularBlockType = FromBlockComboTypeToModularBlockType(CurrentBlockComboType);
                
                if (CurrentBlockComboType == MasterTrainingBlockComboType.Predefine ||
                    CurrentBlockComboType == MasterTrainingBlockComboType.Custom) return;

                var masterHorseTrainingBlockComboId = masterHorseTrainingBlockComboContainer
                                                 .MasterHorseTrainingBlockComboIndexer.Max(x => x.Key) + 1;
                var data = new MasterHorseTrainingBlockCombo(masterHorseTrainingBlockComboId,
                    blockComboName,
                    CurrentBlockComboType,
                    string.Empty);

                var bnames = blockComboData.paddings.Select(x => x.name).ToArray();
                data.MasterHorseTrainingBlockIdList = bnames;
                if (blockComboData.startPadding != default)
                    data.SetMasterTrainingModularBlockIdStart(blockComboData.startPadding.name);
                if (blockComboData.endPadding != default)
                    data.SetMasterTrainingModularBlockIdEnd(blockComboData.endPadding.name);

                masterHorseTrainingBlockComboContainer.Add(data);

            }

        }
        UnSelectOldBlockCombo();
        InitLevelEditor_BlockCombo();
    }

    void CancelEditBlock()
    {
        UnSelectOldBlockCombo();
        CurrentSelectetBlockCombo = -1;
    }


    private void UnSelectOldBlockCombo()
    {
        if (currentEditingPlatformObject != default)
        {
            DestroyImmediate(currentEditingPlatformObject);
            currentEditingPlatformObject = default;
            CurrentSelectetBlockCombo = -1;
        }
    }

    private MasterTrainingModularBlockType FromBlockComboTypeToModularBlockType(
        MasterTrainingBlockComboType masterTrainingBlockComboType)
    {
        return masterTrainingBlockComboType switch
        {
            MasterTrainingBlockComboType.Custom => MasterTrainingModularBlockType.Custom,
            MasterTrainingBlockComboType.Modular => MasterTrainingModularBlockType.Modular,
            MasterTrainingBlockComboType.Predefine => MasterTrainingModularBlockType.Predefine,
            _ => MasterTrainingModularBlockType.Modular,
        };
    }

    private void CreatePlatform(MasterHorseTrainingBlockCombo masterHorseTrainingBlockCombo)
    {
        currentEditingPlatformObject = new GameObject();
        currentEditingPlatformObject.name = masterHorseTrainingBlockCombo.Name;

        var blockObj = new GameObject();
        blockObj.name = "Block";
        blockObj.transform.parent = currentEditingPlatformObject.transform;

        var obstObj = new GameObject();
        obstObj.name = "Obstacle";
        obstObj.transform.parent = currentEditingPlatformObject.transform;
        obstObj.transform.SetAsLastSibling();

        var tmp = AddDataComponent(currentEditingPlatformObject);
        tmp.id = masterHorseTrainingBlockCombo.MasterHorseTrainingBlockId;
        tmp.block_name = masterHorseTrainingBlockCombo.Name;

        var paddingStartBlockId = masterTrainingModularBlockContainer.GetFirstPaddingIfEmpty(masterHorseTrainingBlockCombo.MasterTrainingModularBlockIdStart);
        var paddingEndBlockId = masterTrainingModularBlockContainer.GetFirstPaddingIfEmpty(masterHorseTrainingBlockCombo.MasterTrainingModularBlockIdEnd);
        var modularBlockIds = masterHorseTrainingBlockCombo.MasterHorseTrainingBlockIdList;

        GeneBlocks(collection.BlocksLookUpTable[paddingStartBlockId].gameObject,
            collection.BlocksLookUpTable[paddingEndBlockId].gameObject, 
            modularBlockIds.Select(x => collection.BlocksLookUpTable[x].gameObject).ToArray(), blockObj.transform, tmp).Forget();

    }

    private void CreateNewPlatform()
    {
        currentEditingPlatformObject = new GameObject();

        var blockObj = new GameObject();
        blockObj.name = "Block";
        blockObj.transform.parent = currentEditingPlatformObject.transform;

        var obstObj = new GameObject();
        obstObj.name = "Obstacle";
        obstObj.transform.parent = currentEditingPlatformObject.transform;
        obstObj.transform.SetAsLastSibling();

        AddDataComponent(currentEditingPlatformObject);
    }

    private async UniTask GeneBlocks(GameObject start, GameObject end, GameObject[] gameObjects, Transform parent, AdventureEditor_BlockComboData data)
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();

        BoxCollider[] boxColliders = default;
        await InstantiateBlocksAsync(start, end, gameObjects, parent, data, (s) => boxColliders = s).AttachExternalCancellation(cts.Token);
        Tiling(boxColliders);
    }

    private async UniTask InstantiateBlocksAsync(GameObject paddingStartPrefab, GameObject paddingEndPrefab, GameObject[] gameObjects, Transform parent, AdventureEditor_BlockComboData data, System.Action<BoxCollider[]> finish)
    {
        var len = gameObjects.Length;
        var BoxColliders = new List<BoxCollider>();

        var paddingHead = Instantiate_PaddingHeadCollider(paddingStartPrefab, parent);
        var paddingTail = Instantiate_PaddingTailCollider(paddingEndPrefab, parent);

        var headCol = paddingHead?.GetComponentInChildren<BoxCollider>();
        var tailCol = paddingTail?.GetComponentInChildren<BoxCollider>();
        
        if (paddingHead != default)
        {
            BoxColliders.Add(headCol);
            data.startPadding = paddingHead;
        }

        for (int i = 0; i < len; i++)
        {
            var x = gameObjects[i];
            var block = Instantiate(x, parent);
            block.name = x.name;
            var ss = block.GetComponentInChildren<BoxCollider>();
            BoxColliders.Add(ss);
            data.paddings.Add(block);
            if (i % 5 == 0) await UniTask.DelayFrame(1);
        }

        if (paddingTail != default)
        {
            BoxColliders.Add(tailCol);
            paddingTail.transform.SetAsLastSibling();
            data.endPadding = paddingTail;
        }

        finish?.Invoke(BoxColliders.ToArray());
    }

    private GameObject Instantiate_PaddingHeadCollider(GameObject paddingHead, Transform parent)
    {
        if (paddingHead != default)
        {
            var obj = Instantiate(paddingHead, parent);
            obj.name = paddingHead.name;
            return obj;
        }
        return null;
    }

    private GameObject Instantiate_PaddingTailCollider(GameObject paddingTail, Transform parent)
    {
        if (paddingTail != default)
        {
            var obj = Instantiate(paddingTail, parent);
            obj.name = paddingTail.name;
            return obj;
        }
        return null;
    }

    private void Tiling(BoxCollider[] _BoxColliders)
    {
        ChangePositionOfParentToMatchChildPosition(_BoxColliders[0].transform.parent,
            _BoxColliders[0].transform,
            new Vector3(0, 0, 0));

        var centers = _BoxColliders.Select(x => x.center)
                                  .ToArray();
        for (var i = 1; i < _BoxColliders.Length; i++)
        {
            var baseCollider = _BoxColliders[i - 1];
            var alignedCollider = _BoxColliders[i];
            AlignCollider(baseCollider, alignedCollider, 1);
        }
    }

    private void Tiling(AdventureEditor_BlockComboData data)
    {
        List<BoxCollider> boxColliders = new List<BoxCollider>();

        if(data.startPadding != default)
        {
            boxColliders.Add(data.startPadding.GetComponentInChildren<BoxCollider>());
        }

        for (int i = 0; i < data.paddings.Count; i++)
        {
            var x = data.paddings[i];
            var ss = x.transform.GetChild(0).GetComponent<BoxCollider>();
            boxColliders.Add(ss);
        }

        if (data.endPadding != default)
        {
            boxColliders.Add(data.endPadding.GetComponentInChildren<BoxCollider>());
        }

        Tiling(boxColliders.ToArray());
    }

    private static void AlignCollider(BoxCollider baseCollider,
                                      BoxCollider alignedCollider,
                                      int direction)
    {
        var worldPos = baseCollider.transform.position + baseCollider.center +
                       new Vector3(0, baseCollider.bounds.extents.y, direction * baseCollider.bounds.extents.z)
                       - (alignedCollider.center + new Vector3(0, alignedCollider.bounds.extents.y, -direction * alignedCollider.bounds.extents.z));
        ChangePositionOfParentToMatchChildPosition(alignedCollider.transform.parent,
            alignedCollider.transform,
            worldPos);
    }

    public static void ChangePositionOfParentToMatchChildPosition(Transform parent,
                                                            Transform child,
                                                            Vector3 childWorldDestination)
    {
        parent.position += childWorldDestination - child.position;
    }

    static AdventureEditor_BlockComboData AddDataComponent(GameObject target)
    {
        AdventureEditor_BlockComboData tmp = target.AddComponent<AdventureEditor_BlockComboData>();
        return tmp;
    }

    //Block Paddings 
    private void UpdateBlocksInCombo(MasterHorseTrainingBlockCombo masterHorseTrainingBlockCombo, List<string> masterHorseTrainingBlockIdList)
    {
        masterHorseTrainingBlockCombo.MasterHorseTrainingBlockIdList = masterHorseTrainingBlockIdList.ToArray();
    }

}
