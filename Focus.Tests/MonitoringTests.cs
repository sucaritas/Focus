namespace Focus.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.IO;
    using System.Linq;
    using Focus;
    using System.Collections.Generic;

    [TestClass]
    public class MonitoringTests
    {
        [TestMethod]
        public void CanLoadMonitor()
        {
            File.Copy(@"TestFiles\Scalar.json", @"TestFiles\monitored.json", true);
            using (var monitor = new Focus.Monitored(@"TestFiles\monitored.json"))
            {
                var jtoken = monitor.Focus();
                Assert.AreEqual(1, jtoken.Value<int>("intProp"));
                Assert.AreEqual(1.1f, jtoken.Value<float>("FloatProp"));
                Assert.AreEqual("String Value 1", jtoken.Value<string>("StringProp"));
                Assert.AreEqual(true, new[] { 1 }.IsEqual(jtoken["intArray"].ToObject<int[]>()));
                Assert.AreEqual(true, new[] { "s1" }.IsEqual(jtoken["StringArray"].ToObject<string[]>()));

                File.Copy(@"TestFiles\RootObject.json", @"TestFiles\monitored.json", true);
                System.Threading.Thread.Sleep(200);
                jtoken = monitor.Focus();
                Assert.AreNotEqual(1, jtoken.Value<int>("intProp"));
                Assert.AreNotEqual(1.1f, jtoken.Value<float>("FloatProp"));
                Assert.AreNotEqual("String Value 1", jtoken.Value<string>("StringProp"));

                Assert.AreEqual("This is root object 4", jtoken["test"]["Sub"].Value<string>("RootKey"));
            }
        }
    }
}
