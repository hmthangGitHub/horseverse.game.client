namespace Crosstales.BWF.Util
{
    /// <summary>Configuration for the asset.</summary>
    public static class Config
    {

        #region Variables

        /// <summary>Enable or disable debug logging for the asset.</summary>
        public static bool DEBUG = Constants.DEFAULT_DEBUG;

        /// <summary>Enable or disable debug logging for BadWords (Attention: slow!).</summary>
        public static bool DEBUG_BADWORDS = Constants.DEFAULT_DEBUG_BADWORDS;

        /// <summary>Enable or disable debug logging for Domains (Attention: VERY SLOOOOOOOOWWWW!).</summary>
        public static bool DEBUG_DOMAINS = Constants.DEFAULT_DEBUG_DOMAINS;

        /// <summary>Enable or disable the ensuring the name of the BWF gameobject.</summary>
        public static bool ENSURE_NAME = Constants.DEFAULT_ENSURE_NAME;

        /// <summary>Is the configuration loaded?</summary>
        public static bool isLoaded = false;

        #endregion

        /*
        #region Constructor

        static Config()
        {
            if (!isLoaded)
            {
                Load();

                if (DEBUG)
                    UnityEngine.Debug.Log("Config data loaded");
            }
        }

        #endregion
        */

        #region Public static methods

        /// <summary>Resets all changable variables to their default value.</summary>
        public static void Reset()
        {
            if (!Constants.DEV_DEBUG)
                DEBUG = Constants.DEFAULT_DEBUG;

            DEBUG_BADWORDS = Constants.DEFAULT_DEBUG_BADWORDS;
            DEBUG_DOMAINS = Constants.DEFAULT_DEBUG_DOMAINS;
            ENSURE_NAME = Constants.DEFAULT_ENSURE_NAME;
        }

        /// <summary>Loads all changable variables.</summary>
        public static void Load()
        {
            if (!Constants.DEV_DEBUG)
            {
                if (Common.Util.CTPlayerPrefs.HasKey(Constants.KEY_DEBUG))
                {
                    DEBUG = Common.Util.CTPlayerPrefs.GetBool(Constants.KEY_DEBUG);
                }
            }
            else
            {
                DEBUG = Constants.DEV_DEBUG;
            }

            if (Common.Util.CTPlayerPrefs.HasKey(Constants.KEY_DEBUG_BADWORDS))
            {
                DEBUG_BADWORDS = Common.Util.CTPlayerPrefs.GetBool(Constants.KEY_DEBUG_BADWORDS);
            }

            if (Common.Util.CTPlayerPrefs.HasKey(Constants.KEY_DEBUG_DOMAINS))
            {
                DEBUG_DOMAINS = Common.Util.CTPlayerPrefs.GetBool(Constants.KEY_DEBUG_DOMAINS);
            }

            if (Common.Util.CTPlayerPrefs.HasKey(Constants.KEY_ENSURE_NAME))
            {
                ENSURE_NAME = Common.Util.CTPlayerPrefs.GetBool(Constants.KEY_ENSURE_NAME);
            }

            isLoaded = true;
        }

        /// <summary>Saves all changable variables.</summary>
        public static void Save()
        {
            if (!Constants.DEV_DEBUG)
                Common.Util.CTPlayerPrefs.SetBool(Constants.KEY_DEBUG, DEBUG);

            Common.Util.CTPlayerPrefs.SetBool(Constants.KEY_DEBUG_BADWORDS, DEBUG_BADWORDS);
            Common.Util.CTPlayerPrefs.SetBool(Constants.KEY_DEBUG_DOMAINS, DEBUG_DOMAINS);
            Common.Util.CTPlayerPrefs.SetBool(Constants.KEY_ENSURE_NAME, ENSURE_NAME);

            Common.Util.CTPlayerPrefs.Save();
        }

        #endregion

    }
}
// © 2015-2019 crosstales LLC (https://www.crosstales.com)