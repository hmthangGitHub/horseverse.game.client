using System;
using SQLite4Unity3d;
using UnityEngine;
#if !UNITY_EDITOR
using System.Collections;
using System.IO;
#endif
using System.Collections.Generic;
using System.Linq;
using Assets.SQLite4Unity3d.Scripts;

public class DataService
{
    public BrandContainer<Car> Chevrolet = new BrandContainer<Car>()
    {
        Name = "Chevrolet"
    };

    public BrandContainer<Car> Benz = new BrandContainer<Car>()
    {
        Name = "Benz"
    };

    public Car Cruze = new Car()
    {
        Name = "Cruze",
        Quality = new Quality()
        {
            Hardness = 80,
            Speed = 210
        },
        Tags = new List<string>()
        {
            "family car",
            "economy car"
        }
    };

    public Car S600 = new Car()
    {
        Name = "S600",
        Quality = new Quality()
        {
            Hardness = 90,
            Speed = 320
        },
        Tags = new List<string>()
        {
            "business car",
            "comfortable car"
        }
    };

    public Car C600 = new Car()
    {
        Name = "C600",
        Quality = new Quality()
        {
            Hardness = 85,
            Speed = 280
        },
        Tags = new List<string>()
        {
            "Millions of sales",
            "comfortable car"
        }
    };
    private SQLiteConnection _connection;

    public DataService(string DatabaseName,bool existDB = false)
    {
        if (existDB)
        {
#if UNITY_EDITOR
            var dbPath = string.Format(@"Assets/StreamingAssets/{0}", DatabaseName);
#else // check if file exists in Application.persistentDataPath
        var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);

        if (!File.Exists(filepath))
        {
            Debug.Log("Database not in Persistent path");
            // if it doesn't ->
            // open StreamingAssets directory and load the db ->

#if UNITY_ANDROID
            var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + DatabaseName);  // this is the path to your StreamingAssets in android
            while (!loadDb.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
            // then save to Application.persistentDataPath
            File.WriteAllBytes(filepath, loadDb.bytes);
#elif UNITY_IOS
                 var loadDb = Application.dataPath + "/Raw/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);
#elif UNITY_WP8
                var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);

#elif UNITY_WINRT
		var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
		
#elif UNITY_STANDALONE_OSX
		var loadDb = Application.dataPath + "/Resources/Data/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
#else
	var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
	// then save to Application.persistentDataPath
	File.Copy(loadDb, filepath);

#endif

            Debug.Log("Database written");
        }

        var dbPath = filepath;
#endif
            Debug.Log("Final PATH: " + dbPath);
            _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        }
        else
        {
            var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);
            Debug.Log("Final PATH: " + filepath);
            try
            {
                _connection = new SQLiteConnection(filepath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
            }
            catch (Exception e)
            {
                Debug.Log("error: " + e);
            }
        }

    }
    
    public void CreateDB()
    {
        var orlando = new Car
        {
            Name = "Orlando",
        };
        var malibu = new Car
        {
            Name = "Malibu",
        };
        var c300L = new Car
        {
            Name = "c300L",
        };

        _connection.DropTable<Car>();
        _connection.DropTable<BrandContainer<Car>>();
        _connection.CreateTable<Car>();
        _connection.CreateTable<BrandContainer<Car>>();
        _connection.InsertAll(new Car[]
        {
            C600,
            S600,
            Cruze,
            orlando,
            malibu,
            c300L,
        });
        Chevrolet.Types = new List<Car>() { Cruze, orlando, malibu };
        Benz.Types = new List<Car>() { S600, C600, c300L };
        _connection.Insert(Chevrolet);
        _connection.Insert(Benz);
    }

    public IEnumerable<Car> GetCars()
    {
        return _connection.Table<Car>();
    }

    public IEnumerable<Car> SearchCarByNameInCarTable(string name)
    {
        return _connection.Table<Car>().Where(x => x.Name.Equals(name));
    }

    public IEnumerable<BrandContainer<Car>> GetBrands()
    {
        return _connection.Table<BrandContainer<Car>>();
    }

    public IEnumerable<Car> SearchCarInBrand(string brand, string car)
    {
        var Chevrolet = _connection.Table<BrandContainer<Car>>().Where(x => x.Name.Equals(brand)).FirstOrDefault();
        var cruze = Chevrolet.Types.Where(t => t.Name.Equals(car));
        return cruze;
    }
}