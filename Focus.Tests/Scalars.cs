namespace Focus.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.IO;
    using System.Linq;
    using Focus;
    using System.Collections.Generic;

    [TestClass]
    public class Scalars
    {
        [TestMethod]
        public void CanLoadScalarsNoLenses()
        {
            var json = File.ReadAllText(@"TestFiles\Scalar.json");
            var container = new Focus.Container(json);

            var jtoken = container.Focus();
            Assert.AreEqual(1, jtoken.Value<int>("intProp"));
            Assert.AreEqual(1.1f, jtoken.Value<float>("FloatProp"));
            Assert.AreEqual("String Value 1", jtoken.Value<string>("StringProp"));
            Assert.AreEqual(true, new[] { 1 }.IsEqual(jtoken["intArray"].ToObject<int[]>()));
            Assert.AreEqual(true, new[] { "s1" }.IsEqual(jtoken["StringArray"].ToObject<string[]>()));
        }

        [TestMethod]
        public void CanLoadScalarsConstant()
        {
            var json = File.ReadAllText(@"TestFiles\Scalar.json");
            var container = new Focus.Container(json);

            var jtoken = container.Focus(new System.Collections.Generic.Dictionary<string, string>() { { "Constant", "" } });
            Assert.AreEqual(2, jtoken.Value<int>("intProp"));
            Assert.AreEqual(2.2f, jtoken.Value<float>("FloatProp"));
            Assert.AreEqual("String Value 2", jtoken.Value<string>("StringProp"));
            Assert.AreEqual(true, new int[] { 2, 2 }.IsEqual(jtoken["intArray"].ToObject<int[]>()));
            Assert.AreEqual(true, new string[] { "s2", "s2" }.IsEqual(jtoken["StringArray"].ToObject<string[]>()));
        }

        [TestMethod]
        public void CanLoadScalarsWithLens()
        {
            var json = File.ReadAllText(@"TestFiles\Scalar.json");
            var container = new Focus.Container(json);

            var jtoken = container.Focus(new System.Collections.Generic.Dictionary<string, string>() { { "key", "value" } });
            Assert.AreEqual(4, jtoken.Value<int>("intProp"));
            Assert.AreEqual(4.4f, jtoken.Value<float>("FloatProp"));
            Assert.AreEqual("String Value 4", jtoken.Value<string>("StringProp"));
            Assert.AreEqual(true, new int[] { 4, 4, 4, 4 }.IsEqual(jtoken["intArray"].ToObject<int[]>()));
            Assert.AreEqual(true, new string[] { "s4", "s4", "s4", "s4" }.IsEqual(jtoken["StringArray"].ToObject<string[]>()));
        }

        [TestMethod]
        public void CanLoadScalarsWithLensAndConstant()
        {
            var json = File.ReadAllText(@"TestFiles\Scalar.json");
            var container = new Focus.Container(json);

            var jtoken = container.Focus(new System.Collections.Generic.Dictionary<string, string>() { { "Constant", "" }, { "key", "value" } });
            Assert.AreEqual(3, jtoken.Value<int>("intProp"));
            Assert.AreEqual(3.3f, jtoken.Value<float>("FloatProp"));
            Assert.AreEqual("String Value 3", jtoken.Value<string>("StringProp"));
            Assert.AreEqual(true, new int[] { 3, 3, 3 }.IsEqual(jtoken["intArray"].ToObject<int[]>()));
            Assert.AreEqual(true, new string[] { "s3", "s3", "s3" }.IsEqual(jtoken["StringArray"].ToObject<string[]>()));
        }

        [TestMethod]
        public void CanLoadScalarsCompetingLenses()
        {
            var json = File.ReadAllText(@"TestFiles\Scalar.json");
            var container = new Focus.Container(json);

            var jtoken = container.Focus(new System.Collections.Generic.Dictionary<string, string>() { { "Constant", "" }, { "Constant2", "" }, { "key", "value" } });
            Assert.AreEqual(3, jtoken.Value<int>("intProp"));
            Assert.AreEqual(3.3f, jtoken.Value<float>("FloatProp"));
            Assert.AreEqual("String Value 3", jtoken.Value<string>("StringProp"));
            Assert.AreEqual(true, new int[] { 3, 3, 3 }.IsEqual(jtoken["intArray"].ToObject<int[]>()));
            Assert.AreEqual(true, new string[] { "s3", "s3", "s3" }.IsEqual(jtoken["StringArray"].ToObject<string[]>()));
        }
    }
}
