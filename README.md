# Focus
A flexible configuration library for any application which needs a varying configuration, this could be to vary from request to request or from process to process, or simply from time to time.
It is simple to use and it is light weight. Focus reads a marked up json and creates a store of configs which then can be queried. 


Focus loads A json file and creates a container that holds the variations. When you tell a container to 'Focus' it will resolve the correct configs based on the dictionary you pass in.
The syntax is simple, every config in the json has a property name and an optional set of variations separated by the delimiter '|' for example:

```json
{
	"Config_1|key:value|Constant": "MyValue",
	"Config_1": "DefaultValue"
}
```

**Config_1** is the name of the name of the configuration.
**key:value** is a pair of key value variation **key** is the key and **value** is the value.
**Constant** is a constant reoresented in the dictionary as a key without value.

matching rules are simple as well, each property is evaluated based on the declared order. The first property that matched is able to satisfy all of its own constraints gets selected.   
Constraints are groupd all together into a dictionary. Constants are added as Keys with a empty string value.

```C#
var json = File.ReadAllText(@"file.json");
var container = new Focus.Container(json);

// Writes:  DefaultValue
var obj = container.Focus<MyConfig>();
Console.WriteLine(obj.Config_1);

// Writes:  DefaultValue, because only the key:value matched but not the constant.
var obj = container.Focus<MyConfig>(new Dictionary<string, string>(){ { "key", "value"} });
Console.WriteLine(obj.Config_1);

// Writes:  MyValue
container.Constants.Add("Constant");
obj = container.Focus<MyConfig>(new Dictionary<string, string>(){ { "key", "value"} });
Console.WriteLine(obj.Config_1);
```

You can switch values even across types. For example:   

```json
{
	"Config_1|Int": 1,
	"Config_1|Float": 1.1,
	"Config_1|String": "Hey",
	"Config_1|Object": { "SubItem1": 1.0, "SubItem2": "Hello" },
	"Config_1": "Default",
}
```

```C#
var json = File.ReadAllText(@"file.json");
var container = new Focus.Container(json);
var obj = container.Focus<MyConfig>();

// Writes:  DefaultValue
var obj = container.Focus<MyConfig>();
Console.WriteLine(obj.Config_1);

// Writes:  1
container.Constants.Add("Int");
var obj = container.Focus<MyConfig>();
Console.WriteLine(obj.Config_1);

// Writes:  1, because the constant Int matches first.
container.Constants.Add("Int");
container.Constants.Add("Float");
var obj = container.Focus<MyConfig>();
Console.WriteLine(obj.Config_1);

// Writes:  1.1
container.ClearConstants();
container.Constants.Add("Float");
var obj = container.Focus<MyConfig>();
Console.WriteLine(obj.Config_1);

// Writes:  "Hey"
container.ClearConstants();
container.Constants.Add("String");
var obj = container.Focus<MyConfig>();
Console.WriteLine(obj.Config_1);

// Writes:  "Hey"
container.ClearConstants();
container.Constants.Add("Object");
var obj = container.Focus<MyConfig>();
Console.WriteLine(obj.Config_1.SubItem2);
```

Here is a full example on how to use it.

_Scalar.json_
```json
{
  "intProp|key:value|Constant": 3,
  "intProp|Constant": 2,
  "intProp|key:value": 4,
  "intProp|key:value|Constant2": 5,
  "intProp": 1,

  "FloatProp|key:value|Constant": 3.3,
  "FloatProp|Constant": 2.2,
  "FloatProp|key:value": 4.4,
  "FloatProp|key:value|Constant2": 5.5,
  "FloatProp": 1.1,

  "StringProp|key:value|Constant": "String Value 3",
  "StringProp|Constant": "String Value 2",
  "StringProp|key:value": "String Value 4",
  "StringProp|key:value|Constant2": "String Value 5",
  "StringProp": "String Value 1",

  "intArray|key:value|Constant": [ 3, 3, 3 ],
  "intArray|Constant": [ 2, 2 ],
  "intArray|key:value": [ 4, 4, 4, 4 ],
  "intArray|key:value|Constant2": [ 5, 5, 5, 5, 5 ],
  "intArray": [ 1 ],

  "StringArray|key:value|Constant": [ "s3", "s3", "s3" ],
  "StringArray|Constant": [ "s2", "s2" ],
  "StringArray|key:value": [ "s4", "s4", "s4", "s4" ],
  "StringArray|key:value|Constant2": [ "s5", "s5", "s5", "s5", "s5" ],
  "StringArray": [ "s1" ]
}
```
_ObjectWithScalars.cs_
```C#
public class ObjectWithScalars
{
    public int intProp { get; set; }
    public float FloatProp { get; set; }
    public string StringProp { get; set; }
    public int[] intArray { get; set; }
    public string[] StringArray { get; set; }
}
```

```C#
var json = File.ReadAllText(@"TestFiles\Scalar.json");
var container = new Focus.Container(json);

// LOAD DEFAULT VALUES WITHOUT LENSES
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

// LOAD CONFIGS THAT MATCH A CONSTANT 
container.Constants.Add("Constant");
obj = container.Focus<ObjectWithScalars>();
Assert.AreEqual(2, obj.intProp);
Assert.AreEqual(2.2f, obj.FloatProp);
Assert.AreEqual("String Value 2", obj.StringProp);
Assert.AreEqual(true, new int[] { 2, 2 }.IsEqual(obj.intArray));
Assert.AreEqual(true, new string[] { "s2", "s2" }.IsEqual(obj.StringArray));

// LOAD CONFIGS THAT MATCH A CONSTANT 
container.ClearConstants();
container.Constants.Add("Constant");
obj = container.Focus<ObjectWithScalars>(new Dictionary<string, string>() { { "key", "value" } });
Assert.AreEqual(4, obj.intProp);
Assert.AreEqual(4.4f, obj.FloatProp);
Assert.AreEqual("String Value 4", obj.StringProp);
Assert.AreEqual(true, new int[] { 4, 4, 4, 4 }.IsEqual(obj.intArray));
Assert.AreEqual(true, new string[] { "s4", "s4", "s4", "s4" }.IsEqual(obj.StringArray));

// LOAD CONFIGS WITH BOTH A CONSTANT AND OTHER LENSES
container.ClearConstants();
container.Constants.Add("Constant");
obj = container.Focus<ObjectWithScalars>(new Dictionary<string, string>() { { "key", "value" } });
Assert.AreEqual(3, obj.intProp);
Assert.AreEqual(3.3f, obj.FloatProp);
Assert.AreEqual("String Value 3", obj.StringProp);
Assert.AreEqual(true, new int[] { 3, 3, 3 }.IsEqual(obj.intArray));
Assert.AreEqual(true, new string[] { "s3", "s3", "s3" }.IsEqual(obj.StringArray));

// LOAD CONFIGS WITH COMPETING LENSES
container.ClearConstants();
container.Constants.Add("Constant");
container.Constants.Add("Constant2");
jtoken = container.Focus(new Dictionary<string, string>() { { "key", "value" } });
Assert.AreEqual(3, obj.intProp);
Assert.AreEqual(3.3f, obj.FloatProp);
Assert.AreEqual("String Value 3", obj.StringProp);
Assert.AreEqual(true, new int[] { 3, 3, 3 }.IsEqual(obj.intArray));
Assert.AreEqual(true, new string[] { "s3", "s3", "s3" }.IsEqual(obj.StringArray));
```
