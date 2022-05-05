using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class CreateDBScript : MonoBehaviour {

	public Text DebugText;

	// Use this for initialization
	void Start () {
		StartSync();
	}

    private void StartSync()
    {
        var ds = new DataService("tempDatabase.db");
        ds.CreateDB();

        ToConsole("Show Cars ...");
        var car = ds.GetCars();
        ToConsole (car);

        ToConsole("Searching for Cruze in Table[Car]...");
        car = ds.SearchCarByNameInCarTable("Cruze");
        ToConsole(car);

        ToConsole("Show  Brand...");
        var brands = ds.GetBrands();
        ToConsole(brands);

        ToConsole("Searching for S600 in Table[BrandContainer]...");
        var cruze = ds.SearchCarInBrand("Benz","S600");
        ToConsole(cruze);
    }

    private void ToConsole(IEnumerable<Car> cars){
		foreach (var c in cars) {
			ToConsole(c.ToString());
		}
	}
    private void ToConsole(IEnumerable<BrandContainer<Car>> brands){
		foreach (var b in brands) {
			ToConsole(b.ToString());
		}
	}
	
	private void ToConsole(string msg){
		DebugText.text += System.Environment.NewLine + msg;
		Debug.Log (msg);
	}
}
