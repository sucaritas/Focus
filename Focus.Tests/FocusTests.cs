namespace Focus.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.IO;
    using Focus;

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void CanLoadScalars()
        {
            var json = File.ReadAllText(@"TestFiles\Scalar.json");
            var container = new Focus.Container(json);
            var jtoken = container.Focus();

            Assert.AreEqual(1, jtoken.Value<int>("intProp"));
            Assert.AreEqual(1.1f, jtoken.Value<float>("FloatProp"));
            Assert.AreEqual("String Value 1", jtoken.Value<string>("StringProp"));
            Assert.AreEqual(1, jtoken["intArray"].ToObject<int[]>()[0]);
            Assert.AreEqual("s1", jtoken["StringArray"].ToObject<string[]>()[0]); 
        }
    }
}
