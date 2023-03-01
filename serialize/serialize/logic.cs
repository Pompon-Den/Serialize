using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace serialize
{
    public class Logic
    {
        public string SER(object type)
        {
            string s = "";
            if (type is byte || type is sbyte || type is short || type is ushort || type is int || type is uint || type is long || type is ulong || type is float || type is double)
            {
                s = Convert.ToString(type);
                if (s.Contains(','))
                {
                    s = s.Replace(",", ".");
                }
                return s;
            }
            if (type is string || type is char)
            {
                s = "\"" + Convert.ToString(type) + "\"";
                return s;
            }
            if (type is bool)
            {
                s = Convert.ToString(type).ToLower();
                return s;
            }

            if (type is Dictionary<string, object> dict)
            {
                s = "{";
                foreach (var elum in dict)
                {
                    s += SER(elum.Key) + ":" + SER(elum.Value);
                }
                s += "}";
                return s;
            }

            if (type.GetType().GetInterface("IList") != null)
            {
                foreach (var elum in (IList)type)
                {
                    s += SER(elum) + ",";
                }

                s = "[" + s.Remove(s.Length - 1) + "]";
                return s;
            }

            if (type.GetType().IsClass || type.GetType().IsValueType)
            {
                s = "{";
                s += "\"class\":" + type.GetType().Name;
                foreach (var elum in type.GetType().GetFields())
                {
                    object a = 0;
                    if (elum.IsPublic)
                    {
                        s += "," + "\"" + elum.Name + "\":" + SER(elum.GetValue(type));
                    }
                }
                s += "}";
                return s;
            }
            return " ";
        }


        public string  NameTypeClass(string s)
        {
            string answer = "";
            for(int i = 0; i < s.Length; i++ )
            {
                if (s[i] != ',')
                {
                    answer += s[i];
                }
            }
            return answer;
        }
        public object Deserialize(string str)
        {
            str = str.Replace("\n", "");
            str =  str.Replace(" ", "");
            if ( str[0] == '{')
            {
                if (str.Contains( "\"class\""))
                {
                    str = str.Trim();                   
                    string[] subs = str.Split(',');
                    object result = Activator.CreateInstance(Type.GetType(Assembly.GetEntryAssembly()!.GetName().Name + $".{subs[0].Split(':')[1]}")!);
                    FieldInfo[] a = result!.GetType().GetFields(BindingFlags.Public);
                    for (int i = 1; i <= a.Length; i++)
                    {
                        a[i - 1].SetValue(result, Deserialize(subs[i].Split(':')[1])); 
                    }

                    return result;
                 }
                else // dictionary
                {
                    str = str.Trim();
                    Dictionary<string, object> dict = new();
                    string[] subs = str.Split(',');
                    foreach (var elum in subs)
                    {
                        string[] ddd = elum.Split(':');
                        dict.Add(ddd[0], Deserialize(ddd[1]));
                    }
                    return dict;
                }
            }
            if (str[0]  == '[')
            {
                str = str.Trim();
                List<object> list = new();
                string[] subs = str.Split(',');
                foreach(var elum in subs)
                {
                    list.Add(Deserialize(elum));
                }
                return list;
            }
            if (str[0] == '\"')
            {
                return str.Trim();
            }
            if (str == "true" )
            {
                return true;
            }
            if (str == "false")
            {
                return false;
            }
            if (str[0] != '\"')
            {
                bool success = int.TryParse(str, out var number);
                if (!success || str.Contains('.'))
                {
                    return Convert.ToDouble(str);
                }
                return number;
            }
            


            return " ";
        }

        public void WriteDown()
        {
            var n = new Normal();
            Dictionary<string, object> dict = new Dictionary<string, object>() { { "name", 5 } };
            string s = "danil";
            int i = 6;
            double d = 13.4;
            bool b = true;
            char c = 'a';
            List<string> l = new List<string>();
            l.Add(s); l.Add(s);

            string objectSerialized = JsonSerializer.Serialize(n);
            Console.WriteLine(objectSerialized);
            Console.WriteLine(SER(n));
            File.WriteAllText("NewFileSerialized.json", objectSerialized);
        }
    }
}
