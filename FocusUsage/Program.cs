namespace FocusUsage
{
    using System;
    using System.IO;
    using System.Collections.Generic;

    class Program
    {
        static void Main(string[] args)
        {
            var json = File.ReadAllText("Test1.json");
            var pr = new Focus.Container(json);
            pr.CacheResults = true;

            var lenses = new Dictionary<string, string>();
            lenses.Add("kk", string.Empty);
            var result = pr.Focus(lenses).ToString();
            var x = pr.Focus<Sample1>(lenses);

            json = File.ReadAllText("Test2.json");
            pr = new Focus.Container(json);
        }
    }
}
