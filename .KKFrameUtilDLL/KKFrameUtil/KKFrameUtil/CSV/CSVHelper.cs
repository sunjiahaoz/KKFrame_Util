using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
namespace KK.Frame.Util
{
    public class CsvHelper
    {        
        public delegate void OnResourceLoaded(List<CsvRow> rows);

        public static List<CsvRow> ParseCSV_Exp(string pathNameInResource)
        {
            StreamReader mysr = new StreamReader(pathNameInResource);
            string str = mysr.ReadToEnd();
            if (str == string.Empty) return null;

            string contents = str;
            byte[] datas = System.Text.Encoding.UTF8.GetBytes(contents);
            CsvFileReader reader = new CsvFileReader(new MemoryStream(datas));
            List<CsvRow> result = new List<CsvRow>();
            CsvRow row = new CsvRow();

            while (reader.ReadRow(row))
            {
                result.Add(row);
                row = new CsvRow();
            }
            return result;

        }

        public static List<CsvRow> ParseCSV(TextAsset csvFile)
        {
            if (csvFile)
            {
                string contents = CryptHelper.DecryptString(csvFile.text);
                byte[] datas = System.Text.Encoding.UTF8.GetBytes(contents);
                CsvFileReader reader = new CsvFileReader(new MemoryStream(datas));
                List<CsvRow> result = new List<CsvRow>();
                CsvRow row;
                do
                {
                    row = new CsvRow();
                    result.Add(row);
                }
                while (reader.ReadRow(row));
                return result;
            }
            return null;
        }
        public static int[] ToIntArray(string conteng)
        {
            int[] ret = null;
            string[] szA = conteng.Split(';');
            if (null != szA && szA.Length > 0)
            {
                ret = new int[szA.Length];
                for (int i = 0; i < szA.Length; i++)
                {
                    if (string.IsNullOrEmpty(szA[i].Trim()))
                        continue;
                    ret[i] = Convert.ToInt32(szA[i]);
                }
            }
            return ret;
        }
        public static string[] ToStringArray(string conteng)
        {
            string[] ret = conteng.Split(';');
            return ret;
        }
        public static byte[] ToByteArray(string content)
        {
            byte[] ret = null;
            string[] szA = content.Split(';');
            if (null != szA && szA.Length > 0)
            {
                ret = new byte[szA.Length];
                for (int i = 0; i < szA.Length; i++)
                    ret[i] = Convert.ToByte(szA[i]);
            }
            return ret;
        }
        public static double[] ToDoubleArray(string content)
        {
            double[] ret = null;
            string[] szA = content.Split(';');
            if (null != szA && szA.Length > 0)
            {
                ret = new double[szA.Length];
                for (int i = 0; i < szA.Length; i++)
                    ret[i] = Convert.ToDouble(szA[i]);
            }
            return ret;
        }
        public static float[] ToFloatArray(string conteng)
        {
            float[] ret = null;
            string[] szA = conteng.Split(';');
            if (null != szA && szA.Length > 0)
            {
                ret = new float[szA.Length];
                for (int i = 0; i < szA.Length; i++)
                    ret[i] = Convert.ToSingle(szA[i]);
            }
            return ret;
        }

        public static List<CsvRow> ParseText(string text)
        {
            //  string contents = CryptHelper.DecryptString(text);
            byte[] datas = System.Text.Encoding.UTF8.GetBytes(text);
            CsvFileReader reader = new CsvFileReader(new MemoryStream(datas));
            List<CsvRow> result = new List<CsvRow>();
            CsvRow row;
            do
            {
                row = new CsvRow();
                result.Add(row);
            }
            while (reader.ReadRow(row));
            return result;
        }
        public static List<CsvRow> ParseBytes(byte[] datas)
        {
            CsvFileReader reader = new CsvFileReader(new MemoryStream(datas));
            List<CsvRow> result = new List<CsvRow>();
            CsvRow row;
            do
            {
                row = new CsvRow();
                result.Add(row);
            }
            while (reader.ReadRow(row));
            return result;
        }
        static Dictionary<string, OnResourceLoaded> m_Buff = new Dictionary<string, OnResourceLoaded>();
        public static List<CsvRow> ParseCSV(string strContent, OnResourceLoaded onResourceLoaded = null, bool isRequest = false)
        {
            string text = strContent;
            List<CsvRow> rows = ParseText(text);
            if (null != onResourceLoaded)
                onResourceLoaded(rows);
            return rows;

        }
        public static void onResouceLoaded(string szName, string text)
        {
            List<CsvRow> rows = ParseText(text);
            if (m_Buff.ContainsKey(szName))
            {
                if (m_Buff[szName] != null)
                    m_Buff[szName](rows);
            }
        }

    }
    /// <summary>
    /// Class to store one CSV row
    /// </summary>
    public class CsvRow : List<string>
    {
        public string LineText { get; set; }
    }

    /// <summary>
    /// Class to write data to a CSV file
    /// </summary>
    public class CsvFileWriter : StreamWriter
    {
        public CsvFileWriter(Stream stream)
            : base(stream)
        {
        }

        public CsvFileWriter(string filename)
            : base(filename)
        {
        }

        public CsvFileWriter(Stream stream, Encoding encouding)
            : base(stream, encouding)
        { }

        /// <summary>
        /// Writes a single row to a CSV file.
        /// </summary>
        /// <param name="row">The row to be written</param>
        public void WriteRow(CsvRow row)
        {
            StringBuilder builder = new StringBuilder();
            bool firstColumn = true;
            foreach (string value in row)
            {
                // Add separator if this isn't the first value
                if (!firstColumn)
                    builder.Append(',');
                // Implement special handling for values that contain comma or quote
                // Enclose in quotes and double up any double quotes
                if (value.IndexOfAny(new char[] { '"', ',' }) != -1)
                    builder.AppendFormat("\"{0}\"", value.Replace("\"", "\"\""));
                else
                    builder.Append(value);
                firstColumn = false;
            }
            row.LineText = builder.ToString();
            WriteLine(row.LineText);
        }
    }

    /// <summary>
    /// Class to read data from a CSV file
    /// </summary>
    public class CsvFileReader : StreamReader
    {
        public CsvFileReader(Stream stream)
            : base(stream)
        {
        }

        public CsvFileReader(Stream stream, Encoding encouding)
            : base(stream, encouding)
        { }

        public CsvFileReader(string filename)
            : base(filename)
        {
        }

        /// <summary>
        /// Reads a row of data from a CSV file
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public bool ReadRow(CsvRow row)
        {
            row.LineText = ReadLine();
            if (String.IsNullOrEmpty(row.LineText))
                return false;

            int pos = 0;
            int rows = 0;

            while (pos < row.LineText.Length)
            {
                string value;

                // Special handling for quoted field
                if (row.LineText[pos] == '"')
                {
                    // Skip initial quote
                    pos++;

                    // Parse quoted value
                    int start = pos;
                    while (pos < row.LineText.Length)
                    {
                        // Test for quote character
                        if (row.LineText[pos] == '"')
                        {
                            // Found one
                            pos++;

                            // If two quotes together, keep one
                            // Otherwise, indicates end of value
                            if (pos >= row.LineText.Length || row.LineText[pos] != '"')
                            {
                                pos--;
                                break;
                            }
                        }
                        pos++;
                    }
                    value = row.LineText.Substring(start, pos - start);
                    value = value.Replace("\"\"", "\"");
                }
                else
                {
                    // Parse unquoted value
                    int start = pos;
                    while (pos < row.LineText.Length && row.LineText[pos] != ',')
                        pos++;
                    value = row.LineText.Substring(start, pos - start);
                }

                // Add field to list
                if (rows < row.Count)
                    row[rows] = value;
                else
                    row.Add(value);
                rows++;

                // Eat up to and including next comma
                while (pos < row.LineText.Length && row.LineText[pos] != ',')
                    pos++;
                if (pos < row.LineText.Length)
                    pos++;
            }
            // Delete any unused items
            while (row.Count > rows)
                row.RemoveAt(rows);

            // Return true if any columns read
            return (row.Count > 0);
        }
    }

}