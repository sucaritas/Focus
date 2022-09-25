namespace Focus.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.IO;
    using System.Linq;
    using Focus;
    using System.Collections.Generic;

    [TestClass]
    public class UnitTest1
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
            Assert.AreEqual(true, EnumerableIsEqual(new[] { 1 }, jtoken["intArray"].ToObject<int[]>()));
            Assert.AreEqual(true, EnumerableIsEqual(new[] { "s1" }, jtoken["StringArray"].ToObject<string[]>()));
        }

        [TestMethod]
        public void CanLoadScalarsConstant()
        {
            var json = File.ReadAllText(@"TestFiles\Scalar.json");
            var container = new Focus.Container(json);
            container.Constants.Add("Constant");

            var jtoken = container.Focus();
            Assert.AreEqual(2, jtoken.Value<int>("intProp"));
            Assert.AreEqual(2.2f, jtoken.Value<float>("FloatProp"));
            Assert.AreEqual("String Value 2", jtoken.Value<string>("StringProp"));
            Assert.AreEqual(true, EnumerableIsEqual(new int[] { 2, 2 }, jtoken["intArray"].ToObject<int[]>()));
            Assert.AreEqual(true, EnumerableIsEqual(new string[] { "s2", "s2" }, jtoken["StringArray"].ToObject<string[]>()));
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
            Assert.AreEqual(true, EnumerableIsEqual(new int[] { 4, 4, 4, 4 }, jtoken["intArray"].ToObject<int[]>()));
            Assert.AreEqual(true, EnumerableIsEqual(new string[] { "s4", "s4", "s4", "s4" }, jtoken["StringArray"].ToObject<string[]>()));
        }

        [TestMethod]
        public void CanLoadScalarsWithLensAndConstant()
        {
            var json = File.ReadAllText(@"TestFiles\Scalar.json");
            var container = new Focus.Container(json);
            container.Constants.Add("Constant");

            var jtoken = container.Focus(new Dictionary<string, string>() { { "key", "value" } });
            Assert.AreEqual(3, jtoken.Value<int>("intProp"));
            Assert.AreEqual(3.3f, jtoken.Value<float>("FloatProp"));
            Assert.AreEqual("String Value 3", jtoken.Value<string>("StringProp"));
            Assert.AreEqual(true, EnumerableIsEqual(new int[] { 3, 3, 3 }, jtoken["intArray"].ToObject<int[]>()));
            Assert.AreEqual(true, EnumerableIsEqual(new string[] { "s3", "s3", "s3" }, jtoken["StringArray"].ToObject<string[]>()));
        }

        [TestMethod]
        public void CanLoadScalarsCompetingLenses()
        {
            var json = File.ReadAllText(@"TestFiles\Scalar.json");
            var container = new Focus.Container(json);
            container.Constants.Add("Constant");
            container.Constants.Add("Constant2");

            var jtoken = container.Focus(new Dictionary<string, string>() { { "key", "value" } });
            Assert.AreEqual(3, jtoken.Value<int>("intProp"));
            Assert.AreEqual(3.3f, jtoken.Value<float>("FloatProp"));
            Assert.AreEqual("String Value 3", jtoken.Value<string>("StringProp"));
            Assert.AreEqual(true, EnumerableIsEqual(new int[] { 3, 3, 3 }, jtoken["intArray"].ToObject<int[]>()));
            Assert.AreEqual(true, EnumerableIsEqual(new string[] { "s3", "s3", "s3" }, jtoken["StringArray"].ToObject<string[]>()));
        }

        private bool EnumerableIsEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        {
            return !actual.Any(x => !expected.Any(y => x.Equals(y))) || !expected.Any(x => !actual.Any(y => x.Equals(y)));
        }
    }
}
