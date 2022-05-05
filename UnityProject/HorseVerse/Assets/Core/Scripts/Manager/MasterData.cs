using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreData{
    public class MasterData : ModelData
    {
        protected MasterData()
        {
            _hasCreateDatabase = true;
            //_inStreamingAssets = true;
            _folder = "MasterData/";
            InitDatabase();
        }

        private static MasterData _instance;
        public static MasterData Instance
        {
            get
            {
                return _instance ?? (_instance = new MasterData());
            }
        }

        public override string GetDatabaseName()
        {
            return "MasterData";
        }
    }
}
