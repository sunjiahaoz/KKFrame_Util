using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KK.Frame.Util;

[System.Serializable]
public class LocalizationConfig {
	public 	string	Id;
	public 	string	CN;
	public 	string	EN;
	 public static Dictionary<string,LocalizationConfig>  dic = new  Dictionary<string,LocalizationConfig>();
	
	public static void OnLoaded(List<CsvRow> rows){
		dic.Clear();
		for(int i =3;i < rows.Count;i++){
		string[] values = rows[i].ToArray();
		 if(values.Length<=0) continue;
		LocalizationConfig	 elem = new LocalizationConfig();
		if(values.Length >0)
		{
				elem.Id= values[0];
		}
		if(values.Length >1)
		{
				elem.CN= values[1];
		}
		if(values.Length >2)
		{
				elem.EN= values[2];
		}
		LocalizationConfig.dic[elem.Id] = elem;
		}
	}
}
