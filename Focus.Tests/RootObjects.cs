namespace Focus.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.IO;
    using System.Linq;
    using Focus;
    using System.Collections.Generic;

    [TestClass]
    public class RootObjects
    {
        [TestMethod]
        public void CanLoadRootObjects()
        {
            var json = File.ReadAllText(@"TestFiles\RootObject.json");
            var container = new Focus.Container(json);
            var jtoken = container.Focus();
            Assert.AreEqual("This is root object 4", jtoken["test"]["Sub"].Value<string>("RootKey"));


            container = new Focus.Container(json);
            jtoken = container.Focus(new Dictionary<string, string>() { { "key", "value" } });
            Assert.AreEqual(1, jtoken["test"]["Sub"]["RootKey"].Value<int>("intProp"));
            Assert.AreEqual(1.1f, jtoken["test"]["Sub"]["RootKey"].Value<float>("FloatProp"));
            Assert.AreEqual("This is root object 4", jtoken["test"]["Sub"]["RootKey"].Value<string>("StringProp"));
            Assert.AreEqual(true, new[] { 1 }.IsEqual(jtoken["test"]["Sub"]["RootKey"]["intArray"].ToObject<int[]>()));
            Assert.AreEqual(true, new[] { "s1" }.IsEqual(jtoken["test"]["Sub"]["RootKey"]["StringArray"].ToObject<string[]>()));
        }
    }
}
