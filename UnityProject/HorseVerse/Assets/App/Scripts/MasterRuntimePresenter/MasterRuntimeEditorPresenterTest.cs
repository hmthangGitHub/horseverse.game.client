#if ENABLE_DEBUG_MODULE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterRuntimeEditorPresenterTest : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        await new MasterRuntimeEditorPresenter().PerformMasterEditAsyncGeneric<MasterHorseTrainingPropertyContainer>();
    }
}
#endif