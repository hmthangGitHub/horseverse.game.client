#if ENABLE_MASTER_RUN_TIME_EDIT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterRuntimeEditorPresenterTest : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        await new MasterRuntimeEditorPresenter().PerformMasterEditAsync<MasterHorseTrainingPropertyContainer, MasterHorseTrainingProperty>();
    }
}
#endif