using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDownloadAssetProgress : PopupEntity<UIDownloadAssetProgress.Entity>
{
	[System.Serializable]
    public class Entity
    {
        public UIComponentProgressBar.Entity progressBar;
        public int totalFiles;
        public int currentFiles;
    }

    public UIComponentProgressBar progressBar;
    public FormattedTextComponent detailProgress;

    protected override void OnSetEntity()
    {
        progressBar.SetEntity(this.entity.progressBar);
        detailProgress.SetEntity(this.entity.currentFiles, this.entity.totalFiles);
    }
}	