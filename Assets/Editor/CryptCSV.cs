using UnityEditor;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using KK.Frame.Util;
using System.Collections.Generic;

namespace KK.Frame.Util.Editor
{
    public class CryptCSV
    {

        [MenuItem("Generator/CSV createScript")]
        static void CreateScripte()
        {
            string[] ExceptFile = { "DropItem" };
            string[] ExceptType = { "Str" };
            foreach (Object o in Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets))
            {

                bool con = false;
                for (int jj = 0; jj < ExceptFile.Length; jj++)
                {
                    if (o.name == ExceptFile[jj])
                    {
                        con = true;
                        break;
                    }
                }
                if (con == true) continue;
                string filePath = AssetDatabase.GetAssetPath(o);
                string csPath = "Assets/Scripts/Config/" + Path.GetFileName(filePath).Replace(".csv", ".cs");
                StreamWriter sc = new StreamWriter(csPath, false);
                List<CsvRow> rows = CsvHelper.ParseCSV(Path.GetFileName(o.name));
                if (rows.Count < 3)
                {
                    // UTools.__LogError("ÄãÒªÉú³ÉµÄ±í¸ñµÄ¸ñÊ½²»¶Ô");
                    return;
                }
                sc.Write("using UnityEngine;\n");
                sc.Write("using System.Collections;\n");
                sc.Write("using System.Collections.Generic;\n");
                sc.Write("using CSVHelper;\n");
                sc.Write("[System.Serializable]\n");
                sc.Write("public class " + o.name + " {\n");
                CsvRow types = rows[0];
                CsvRow vars = rows[1];
                //            CsvRow des = rows[2];
                for (int i = 0; i < types.Count; i++)
                {
                    if (types[i] == ExceptType[0]) continue;
                    sc.Write("\tpublic \t" + types[i] + "\t" + vars[i] + ";\n");
                }
                sc.Write("\t public static Dictionary<" + types[0] + "," + o.name + ">  dic = new " + " Dictionary<" + types[0] + "," + o.name + ">();\n");
                sc.Write("\tpublic static void load(){\n");
                sc.Write("\t\tCsvHelper.ParseCSV(" + "\"" + o.name + "\"" + ",OnLoaded,true" + ");\n");
                sc.Write("\t}\n");
                sc.Write("\tpublic static void OnLoaded(List<CsvRow> rows){\n");
                sc.Write("\t\tdic.Clear();\n");
                sc.Write("\t\tfor(int i =3;i < rows.Count;i++){\n");
                sc.Write("\t\tstring[] values = rows[i].ToArray();\n");
                sc.Write("\t\t if(values.Length<=0) continue;\n");
                sc.Write("\t\t" + o.name + "\t elem = new " + o.name + "();\n");
                for (int i = 0; i < types.Count; i++)
                {
                    if (types[i] == ExceptType[0]) continue;
                    sc.Write("\t\tif(values.Length >" + i.ToString() + ")\n");
                    sc.Write("\t\t{\n");
                    //                UTools.__LogInfo(types[i]);
                    if (types[i].Contains("float[]"))
                    {
                        sc.Write("\t\t\telem." + vars[i] + "= CsvHelper.ToFloatArray(values[" + i + "]);\n");

                    }
                    else if (types[i].Contains("int[]"))
                    {
                        sc.Write("\t\t\telem." + vars[i] + "= CsvHelper.ToIntArray(values[" + i + "]);\n");

                    }
                    else if (types[i].Contains("byte[]"))
                    {
                        sc.Write("\t\t\telem." + vars[i] + "= CsvHelper.ToByteArray(values[" + i + "]);\n");

                    }
                    else if (types[i].Contains("double[]"))
                    {
                        sc.Write("\t\t\telem." + vars[i] + "= CsvHelper.ToDoubleArray(values[" + i + "]);\n");

                    }
                    else if (types[i].Contains("string[]"))
                    {
                        sc.Write("\t\t\telem." + vars[i] + "= CsvHelper.ToStringArray(values[" + i + "]);\n");

                    }
                    else if (!types[i].Contains("string"))
                    {
                        sc.Write("\t\t\tif(!string.IsNullOrEmpty(values[" + i + "])){\n");
                        sc.Write("\t\t\t\t" + types[i] + ".TryParse(values[" + i + "]" + ",out elem." + vars[i] + ");\n");
                        sc.Write("\t\t\t}\n");

                    }
                    else
                        sc.Write("\t\t\t\telem." + vars[i] + "= values[" + i + "];\n");
                    sc.Write("\t\t}\n");
                }
                sc.Write("\t\t" + o.name + ".dic" + "[elem." + vars[0] + "] = elem;");
                sc.Write("\n\t\t}\n");
                sc.Write("\t}\n");
                sc.Write("}\n");
                sc.Close();
            }
            AssetDatabase.Refresh();
        }
    }
}
