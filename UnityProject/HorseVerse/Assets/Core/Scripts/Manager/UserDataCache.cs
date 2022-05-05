using System;

namespace CoreData
{
    public class UserDataCache : ModelData
    {
        protected UserDataCache()
        {
            _hasCreateDatabase = false;
            InitDatabase();
        }

        private static UserDataCache _instance;
        public static UserDataCache Instance
        {
            get
            {
                return _instance ?? (_instance = new UserDataCache());
            }
        }

        public override string GetDatabaseName()
        {
            return "UserDataCache";
        }
    }
}