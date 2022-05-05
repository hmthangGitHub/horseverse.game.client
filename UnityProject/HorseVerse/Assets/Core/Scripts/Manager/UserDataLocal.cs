using System;

namespace CoreData
{
    public class UserDataLocal : ModelData
    {
        protected UserDataLocal()
        {
            _hasCreateDatabase = true;
            _folder = "UserDataLocal/";
            InitDatabase();
        }

        private static UserDataLocal _instance;
        public static UserDataLocal Instance
        {
            get
            {
                return _instance ?? (_instance = new UserDataLocal());
            }
        }

        public override string GetDatabaseName()
        {
            return "UserDataLocal";
        }
    }
}