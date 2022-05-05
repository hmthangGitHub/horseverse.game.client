using System;
using System.Collections;
using UnityEngine;

namespace CoreData
{
    public class UserData : ModelData
    {
        protected UserData()
        {
            _hasCreateDatabase = true;
            _folder = "UserData/";
            InitDatabase();
        }

        private static UserData _instance;
        public static UserData Instance
        {
            get
            {
                return _instance ?? (_instance = new UserData());
            }
        }

        public override string GetDatabaseName()
        {
            return "UserData";
        }
	}
}