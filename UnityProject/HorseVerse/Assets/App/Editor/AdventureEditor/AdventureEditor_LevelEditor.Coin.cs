using Cysharp.Threading.Tasks;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class AdventureEditor_LevelEditor
{
    private async UniTask GenerateCoin(MasterHorseTrainingBlockCombo masterHorseTrainingBlockCombo, Transform parent, AdventureEditor_BlockComboData data)
    {
        await UniTask.Yield();
        masterHorseTrainingBlockCombo.CoinList
                                  .ForEach(x =>
                                  {
                                      var c = CreateCoinEditorFromMasterCoin(x, parent);
                                      data.coins.Add(c);
                                  });
    }

    private AdventureEditor_CoinEditor CreateCoinEditorFromMasterCoin(Coin coin, Transform parent)
    {
        var coinEditor = CreateCoinEditor();
        coinEditor.gameObject.name = "Coin";
        var coinEditorTransform = coinEditor.transform;
        coinEditorTransform.parent = parent;
        coinEditorTransform.localPosition = coin.localPosition.ToVector3();
        coinEditor.Init(coin.numberOfCoin, coin.benzierPointPositions.Select(x => x.ToVector3()).ToArray());
        return coinEditor;
    }

    private AdventureEditor_CoinEditor CreateCoinEditor()
    {
        var coinEditor = new GameObject();
        var comp = coinEditor.AddComponent<AdventureEditor_CoinEditor>();
        return comp;
    }

    private AdventureEditor_CoinEditor CreateNewCoinEditor(Transform parent)
    {
        var coinEditor = CreateCoinEditor();
        coinEditor.gameObject.name = "Coin";
        var coinEditorTransform = coinEditor.transform;
        coinEditorTransform.parent = parent;
        coinEditorTransform.localPosition = Vector3.zero;
        Vector3[] points = new Vector3[] { Vector3.zero, Vector3.forward };
        coinEditor.Init(0, points);
        return coinEditor;
    }

    private void SaveCoinToBlock(MasterHorseTrainingBlockCombo masterHorseTrainingBlockCombo, List<AdventureEditor_CoinEditor> coins)
    {
#if ENABLE_DEBUG_MODULE
        masterHorseTrainingBlockCombo.CoinList = coins.Select(FromCoinEditorToMasterCoin)
            .ToArray();
#endif
    }

    private static Coin FromCoinEditorToMasterCoin(AdventureEditor_CoinEditor x)
    {
        return new Coin()
        {
            localPosition = Position.FromVector3(x.transform.localPosition),
            numberOfCoin = x.CoinNumber,
            benzierPointPositions = x.BenzierPointPositions.Select(Position.FromVector3)
                                     .ToArray()
        };
    }
}
