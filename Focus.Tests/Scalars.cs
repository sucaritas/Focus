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

            // Object deserialization
            var obj = container.Focus<ObjectWithScalars>();
            Assert.AreEqual(1, obj.intProp);
            Assert.AreEqual(1.1f, obj.FloatProp);
            Assert.AreEqual("String Value 1", obj.StringProp);
            Assert.AreEqual(true, new[] { 1 }.IsEqual(obj.intArray));
            Assert.AreEqual(true, new[] { "s1" }.IsEqual(obj.StringArray));

            // NewtonSoft.json token deserialization
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
            container.Constants.Add("Constant");

            // Object deserialization
            var obj = container.Focus<ObjectWithScalars>();
            Assert.AreEqual(2, obj.intProp);
            Assert.AreEqual(2.2f, obj.FloatProp);
            Assert.AreEqual("String Value 2", obj.StringProp);
            Assert.AreEqual(true, new int[] { 2, 2 }.IsEqual(obj.intArray));
            Assert.AreEqual(true, new string[] { "s2", "s2" }.IsEqual(obj.StringArray));

            // NewtonSoft.json token deserialization
            var jtoken = container.Focus();
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
            var lenses = new Dictionary<string, string>() { { "key", "value" } };

            // Object deserialization
            var obj = container.Focus<ObjectWithScalars>(lenses);
            Assert.AreEqual(4, obj.intProp);
            Assert.AreEqual(4.4f, obj.FloatProp);
            Assert.AreEqual("String Value 4", obj.StringProp);
            Assert.AreEqual(true, new int[] { 4, 4, 4, 4 }.IsEqual(obj.intArray));
            Assert.AreEqual(true, new string[] { "s4", "s4", "s4", "s4" }.IsEqual(obj.StringArray));

            // NewtonSoft.json token deserialization
            var jtoken = container.Focus(lenses);
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
            container.Constants.Add("Constant");
            var lenses = new Dictionary<string, string>() { { "key", "value" } };

            // Object deserialization
            var obj = container.Focus<ObjectWithScalars>(lenses);
            Assert.AreEqual(3, obj.intProp);
            Assert.AreEqual(3.3f, obj.FloatProp);
            Assert.AreEqual("String Value 3", obj.StringProp);
            Assert.AreEqual(true, new int[] { 3, 3, 3 }.IsEqual(obj.intArray));
            Assert.AreEqual(true, new string[] { "s3", "s3", "s3" }.IsEqual(obj.StringArray));

            // NewtonSoft.json token deserialization
            var jtoken = container.Focus(lenses);
            Assert.AreEqual(3, jtoken.Value<int>("intProp"));
            Assert.AreEqual(3.3f, jtoken.Value<float>("FloatProp"));
            Assert.AreEqual("String Value 3", jtoken.Value<string>("StringProp"));
            Assert.AreEqual(true, new int[] { 3, 3, 3 }.IsEqual(jtoken["intArray"].ToObject<int[]>()));
            Assert.AreEqual(true, new string[] { "s3", "s3", "s3" }.IsEqual(jtoken["StringArray"].ToObject<string[]>()));
        }

        [TestMethod]
        public void CanLoadScalarsCompetingConstraints()
        {
            var json = File.ReadAllText(@"TestFiles\Scalar.json");
            var container = new Focus.Container(json);
            container.Constants.Add("Constant");
            container.Constants.Add("Constant2");
            var lenses = new Dictionary<string, string>() { { "key", "value" } };

            // Object deserialization
            var obj = container.Focus<ObjectWithScalars>(lenses);
            Assert.AreEqual(3, obj.intProp);
            Assert.AreEqual(3.3f, obj.FloatProp);
            Assert.AreEqual("String Value 3", obj.StringProp);
            Assert.AreEqual(true, new int[] { 3, 3, 3 }.IsEqual(obj.intArray));
            Assert.AreEqual(true, new string[] { "s3", "s3", "s3" }.IsEqual(obj.StringArray));

            // NewtonSoft.json token deserialization
            var jtoken = container.Focus(lenses);
            Assert.AreEqual(3, jtoken.Value<int>("intProp"));
            Assert.AreEqual(3.3f, jtoken.Value<float>("FloatProp"));
            Assert.AreEqual("String Value 3", jtoken.Value<string>("StringProp"));
            Assert.AreEqual(true, new int[] { 3, 3, 3 }.IsEqual(jtoken["intArray"].ToObject<int[]>()));
            Assert.AreEqual(true, new string[] { "s3", "s3", "s3" }.IsEqual(jtoken["StringArray"].ToObject<string[]>()));
        }
    }
}
