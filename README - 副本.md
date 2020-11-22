## A small, high performance, powerful serializer using bssom binary protocol
[![Nuget](https://img.shields.io/nuget/v/BssomSerializer.svg)](https://www.nuget.org/packages/BssomSerializer/) 

**Bssom.Net** is a high-performance **structured binary serializer** implemented using the **BSSOM**  protocol, suitable for storage services and computing services, it has the following characteristics: **small, fast and functional**.

1. Compact, dll only more than **300** k  
2. Fast, it has **first-class** serialization and deserialization [Performance](#1performance)  
3. Strong functionality:
	* You can get the size of the serialized object **without completely** serializing the object
	* You can read an element of the object **without completely** deserializing the object
	* You can change an element in the object **without complete** serialization
	* The serialized format is **self-describing**

## Why do we need?  

At present, C# has many binary serializers, but these serializers only provide a single serialization and deserialization function. 
 
Bssom.Net uses the [Bssom](https://github.com/1996v/Bssom) protocol, makes serialized data have **structured **characteristics and it has the function of marshalling fields directly, this allows Bssom.Net to do things that other serializers cannot.

* When I want to know the size of the object after serialization when serializing the object, so as to choose the correct position of the object to be serialized in advance (such as the FSM algorithm of the database engine), then Bssom.Net can satisfy you
* When I have a large binary data, but I only want to read one of the fields without contract to avoid the complete deserialization overhead, then Bssom.Net can satisfy you
* When I have a binary data that has been serialized, I only want to modify one of the fields without contract to avoid the overhead of re-serialization, then Bssom.Net can satisfy you
* When I want the object to be serialized and still retain the type information without relying on the entity, then Bssom.Net can satisfy you

### What is the Bssom protocol?
> [Bssom](https://github.com/1996v/Bssom)(Binary search algorithm structure model object binary marshalling)is a protocol for structured marshalling of objects using a binary search algorithm model，the marshalled data has special metadata information. the marshalled data has special metadata information. according to these metadata information, an element in the object can be efficiently read and changed, so that when serializing and deserializing large objects, it will not cause complete serialization overhead due to reading or writing a field.

## Table of Contents ##
* [1.Performance](#1performance)
* [2.Reader-Writer](#2reader-writer)
	* [IBssomBuffer](#ibssombuffer)
	* [IBssomBufferWriter](#ibssombufferwriter)
* [3.Formatter](#3formatter)
* [4.Resolver](#4resolver)
	* [PrimitiveResolver](#primitiveresolver)
	* [AttributeFormatterResolver](#attributeformatterresolver)
	* [BuildInResolver](#buildinresolver)
	* [BssomValueResolver](#bssomvalueresolver)
	* [IDictionaryResolver](#idictionaryresolver)
	* [ICollectionResolver](#icollectionresolver)
	* [MapCodeGenResolver](#mapcodegenresolver)
	* [MapCodeGenResolverAllowPrivate](#mapcodegenresolverallowprivate)
	* [ObjectResolver](#objectresolver)
	* [CompositedResolver](#compositedresolver)
	* [CompositedResolverAllowPrivate](#compositedresolverallowprivate)
	* [Array3CodeGenResolver](#array3codegenresolver)
	* [Array3CodeGenResolverAllowPrivate](#array3codegenresolverallowprivate)
	* [IntKeyCompositedResolver](#intkeycompositedresolver)
	* [IntKeyCompositedResolverAllowPrivate](#intkeycompositedresolverallowprivate)
	* [Reslover Without Contract](#reslover-without-contract)
	* [Reslover With Contract](#reslover-with-contract)
	* [The type supported by default](#the-type-supported-by-default)
* [5.Extensions](#5extensions)
* [6.High-Level API](#6high-level-api)
	* [BssomSerializer](#bssomserializer)
	* [BssomSerializerOptions](#bssomserializeroptions)
	* [BssomSerializeContext](#bssomserializecontext)
* [7.Field marshal](#7field-marshal)
	* [BssomFieldMarshaller](#BssomFieldMarshaller)
	* [Simple Field access language](#simple-field-access-language)
	* [Custom Field access interface](#custom-field-access-interface)
* [8.Dynamic code generation](#8dynamic-code-generation)
* [9.Attributes](#9attributes)
* [10.More possibilities](#10more-possibilities)
* [11.How to use](#11how-to-use)
	* [Size](#size)
	* [Serialize](#serialize)
	* [Deserialize](#deserialize)
	* [ReadValue](#readvalue)
	* [ReadAllMapKeys](#readallmapkeys)
	* [TryWriteValue](#trywritevalue)
	* [Other](#other)
	* [How to use Attribute](#how-to-use-attribute)
	* [How to define an extension](#how-to-define-an-extension)
* [12.Limitations](#12limitations)
	* [BssomSerializationArgumentException.BssomMapKeySame](#bssomserializationargumentexception.bssommapkeysame)
* [13.How to contribute](#13how-to-contribute)
* [14.Who is using](#14who-is-using)

## 1.Performance
![](https://user-images.githubusercontent.com/30827194/97228887-808af380-1812-11eb-846d-821ed0a7d978.png)  

Here is a benchmark for performance comparison with two excellent serializers([MessagePack](https://github.com/neuecc/MessagePack-CSharp) 和 [Protobuf-net](https://github.com/protobuf-net/protobuf-net)) under the .NET  

The **columnar** data represents the time it takes to perform the same task,  **lower means faster performance**,  the **polyline** data represents the GC generated by performing the same task,  **the lower it means the less garbage generated during execution** , from the performance comparison results, it can be seen that the performance of Bssom.Net is very excellent.

Bssom.Net uses many techniques to improve performance:

* Using **memory pool** technology, the memory used for writing can be **reused**
* Using Expressions and Emit dynamic programming technology, special treatment of types, and avoiding value type **boxing and unboxing**
* Use **generic static cache** to avoid dictionary lookup overhead
* Wrapped exception throwing code to increase the possibility of **inlining**
* More calls to **strong typing**, rather than interface abstraction
* **Preprocess** Map2 type metadata, do not need to re-encode it during serialization
* When looking for the Map2 key, **Fix the local reference** in advance, instead of the standard function call
* When parsing the Map1 type, an 8-byte **automaton** is automatically constructed for jump search
* It is worth mentioning that for the purpose of reducing dependency and reducing volume, Bssom.Net does not rely on `System.Memory.dll`, so it cannot use `Span<T>`, `Memory<T>` and other types. This This means that the implementation of Bssom.Net will not be able to use the JIT internal feature of `ByReference<T>`, so the current reader-writer will not have the three performance optimization points of read and write localization, devirtualization, and inline calling (But even so, the current performance of Bssom.Net is still very good), if it is possible to support the `Span<T>` type in the future, then Bssom.Net will use some additional performance techniques to improve performance again.

## 2.Reader-Writer

Bssom.Net For the entry of reading and writing, we do not use native `byte[]` directly, but provide buffer interface `IBssomBuffer` and writer interface `IBssomBufferWriter`.  
Unlike the native `byte[]`, the interface will be more flexible. After implementing `IBssomBuffer`, data can be read from any source, and after implementing `IBssomBufferWriter`, data can be written anywhere (such as non-contiguous fragments).

### IBssomBuffer

`IBssomBuffer` is a buffer interface for serialization, providing read behavior.
Method         |     Description
------------|-----------
Position   | The current position within the buffer
ReadRef  | Read the reference of the specified size byte sequence from the position in the current buffer
 Seek    |  Sets the position within the current buffer
SeekWithOutVerify | Set the position of the current buffer, and do not verify the boundary of position
<a name="tryreadfixedref">TryReadFixedRef</a> |  Attempt to return a fixed reference to a byte sequence from the current position in the buffer,When the Seek operation is performed, the reference position of the fixed byteRef will not be affected
UnFixed |  Used to cancel the reference fixed by TryReadFixedRef, the call of this method is always symmetrical with TryReadFixedRef

### IBssomBufferWriter

`IBssomBufferWriter` is a buffer-based write interface that provides write behavior.
Method  | Description
-----|-----
Buffered|The number of bytes written in the buffer
Advance| Used to indicate that part of the buffer has been written to
Position| The current position within the writer
Seek | Sets the position within the current writer
SeekWithOutVerify | Set the position of the current writer, and do not verify the boundary of Buffered.
GetRef | Get a reference to the byte sequence for writing from the current buffer
CanGetSizeRefForProvidePerformanceInTryWrite | In field marshall, Whether the current position can provide a byte sequence reference of specified size to be used to provide performance for some types of internal writes
GetBssomBuffer | Gets the buffer used by the current writer

**Bssom.Net has already encapsulated the interfaces of `IBssomBuffer` and `IBssomBufferWriter` for `byte[]`, `Stream`, users do not need to manually encapsulate**

## 3.Formatter

**Formatting** is a process by which Bssom.Net converts .Net objects and Bssom formats to each other. Bssom.Net uses `IBssomFormatter<T>` to format objects.
API  |  Description
-----|----------
Size |  In the case of no serialization,Gets the size of the binary data after the object is serialized
Serialize | Serialize the object into Bssom binary format
Deserialize | Deserialize the Bssom binary format into an object

Bssom.Net has built-in many formatters, such as .NET primitive types, key-value pair types, iterable types... They are in the `Bssom.Serializer.Formatters` namespace, you can find it and directly Call it. 

If you don't need to deal with a certain type specially, then these formatters can basically cover most of your needs. And how to find the formatter, this is what the Resolver needs to do.

## 4.Resolver

**Resolving** is a process of obtaining the .Net type object to the corresponding formatter. Bssom.Net implements the resolving of the object through `IFormatterResolver`.

API  | Description
-----|------
GetFormatter | Find a Formatter for a particular type to serialize and deserialize

The resolver usually has two functions of resolving the type and saving the formatter. The resolver implemented in Bssom.Net will internally search for the formatter of the .Net type, and then cache the results through the static generic type. The formatter completes the process of **binding one or a group of .Net types** to the corresponding formatter.  

`IFormatterResolver` is the topmost entry point for Bssom.NET to start serializing objects,  They are in the `Bssom.Serializer.Resolvers` namespace.

Name         |  Description 
------------|-------------
<a name="primitiveresolver">PrimitiveResolver</a> | This resolver provides a formatter of type `sbyte``sbyte`,`Int16`,`Int32`,`Int64`,`byte`,`UInt16`,`UInt32`,`UInt64`,`Single`,`Double`,`bool`,`char`,`Guid`,`Decimal`,`string`,`DateTime`
<a name="attributeformatterresolver">AttributeFormatterResolver</a> | Gets and provides an instance of a user-defined formatter
<a name="buildinresolver">BuildInResolver</a> | Provides `StringBuilder`, `BitArray`, `DataTable` and other types of resolvers
<a name="bssomvalueresolver">BssomValueResolver</a> | Provides a formatter of type `BssomValue`
<a name="idictionaryresolver">IDictionaryResolver</a> | Get and generate a type of resolver with the behavior of `IDictionary`, which abstracts the behavior rules defined for key-value pairs in the BCL, and generates dynamic resolving codes for objects that meet the rules. Inside the resolver, two formats of `Map1` or `Map2` will be selected through runtime configuration options
<a name="icollectionresolver">ICollectionResolver</a> | Get and generate a type of resolver with `IColloction` behavior. The resolver abstracts the behavior rules defined for the collector in the BCL, and generates dynamic resolving codes for objects satisfying the rules. Inside the resolver, if the element type in the collection is a primitive type, it will be resolved into the format of `Array1`, otherwise it will be resolved into the format of `Array2`
<a name="mapcodegenresolver">MapCodeGenResolver</a> | Get and generate a resolver for BssomMap type of the public fields and properties of the object. If the object is an interface, it will automatically generate the implementation of the interface as the carrier of deserialization. Inside the resolver, the type is always parsed into the `Map2` format, and the deserialization codes of the two formats `Map1` and `Map2` are provided
<a name="mapcodegenresolverallowprivate">MapCodeGenResolverAllowPrivate</a> | Get and generate all (public and non-public) fields and properties of the object to perform BssomMap type resolver. If the object is an interface, it will automatically generate the implementation of the interface as the carrier of deserialization. Inside the resolver, the type is always resolved into the `Map2` format, and the deserialization codes of the two formats `Map1` and `Map2` are provided
<a name="objectresolver">ObjectResolver</a> | Provides a formatter of type `Object`
<a name="compositedresolver">CompositedResolver</a> | Composite resolver, combined with `Object`,`Primitive`,`Attribute`,`BssomValue`,`BuildIn`,`IDictionary`,`ICollection`,`MapCodeGen` resolver
<a name="compositedresolverallowprivate">CompositedResolverAllowPrivate</a> | Composite resolver, combined with `Object`,`Primitive`,`Attribute`,`BssomValue`,`BuildIn`,`IDictionary`,`ICollection`,`MapCodeGenAllowPrivate` resolver
<a name="array3codegenresolver">Array3CodeGenResolver</a> | Resolver with contract, the public fields and properties in the type will be marshalled into BssomArray([Array3](https://github.com/1996v/Bssom#array3)) types. The elements in the serialized type must be marked with [KeyAttribute](#keyattribute). The resolver uses the element offset index instead of StringKey to obtain faster Field marshalling performance
<a name="array3codegenresolverallowprivate">Array3CodeGenResolverAllowPrivate</a> | Resolver with contract, Type marshalling of all (public and non-public) fields and properties in the type, the elements in the serialized type must be marked with [KeyAttribute](#keyattribute). The resolver uses the element offset index instead of StringKey to obtain faster Field marshalling performance
<a name="intkeycompositedresolver">IntKeyCompositedResolver</a> | Composite resolver with contract, providing faster serialization performance, combined with `Object`,`Primitive`,`Attribute`,`BssomValue`,`BuildIn`,`IDictionary`,`ICollection`,`Array3CodeGenResolver`  resolver
<a name="intkeycompositedresolverallowprivate">IntKeyCompositedResolverAllowPrivate</a> | Composite resolver with contract,  providing faster serialization performance, combined with `Object`,`Primitive`,`Attribute`,`BssomValue`,`BuildIn`,`IDictionary`,`ICollection`,`Array3CodeGenResolverAllowPrivate` resolver

>  Because of the sufficiently abstract rules defined in `IDictionaryResolver` and `ICollectionResolver`, Bssom.Net does not need to write specific resolving code for new implementations of `IDictionary` or `IColloction` that may appear in .NET in the future.
  
In Bssom.Net, you can inject the resolver required for serialization through the `FormatterResolver` Property in `BssomSerializerOptions`. The default is `CompositedResolver`, `CompositedResolver` will search for the types in order from `Object`, `Primitive`, `Attribute`, `BssomValue`, `BuildIn`, `IDictionary`, `ICollection`, `MapCodeGen` resolver , until the corresponding Resolver.

>  If you want to serialize the private fields in the object, please use `CompositedResolverAllowPrivate`, in `BssomSerializerOptions` you can directly use the default configuration set of `DefaultAllowPrivate`

### Reslover Without Contract
Bssom.Net uses the contract-free composite resolver [CompositedResolver](#compositedresolver) by default when serializing. This resolver does not require you to make any code changes for the types that need to be serialized, and can be used directly.
```c#
var obj = new MyClass();
BssomSerializer.Serialize(obj);//Use the default resolver CompositeResolver to resolve obj
```
### Reslover With Contract
Bssom.Net provides a contracted composite resolver with faster serialization performance[IntKeyCompositedResolver](#intkeycompositedresolver), Thanks to Bssom's Array3 format, the parser does not have the overhead of looking up the string type Key when deserializing, and it has faster performance than `CompositedResolver`. To use `IntKeyCompositedResolver`, you need to explicitly mark the elements in the type with [KeyAttribute](#keyattribute).
```c#
public class MyClass
{
    [Key(0)]
    public int Name;
    [Key(1)]
    public string Address;
}
BssomSerializer.Serialize(obj, option = BssomSerializerOptions.IntKeyCompositedResolverOption);//Use the specified IntKeyCompositedResolver to resolve obj
```

### The type supported by default
These types can be serialized by default：
* [Primitive](https://docs.microsoft.com/en-us/dotnet/api/system.type.isprimitive?view=net-5.0#remarks)(`byte`,`int`...),`string`,`DateTime`,`Guid`,`Decimal`...
* `Nullable<>`,`Lazy<>`,`IGrouping<,>`,`ILookup<,>`,`AnonymousType`,`StringDictionary`,`StringBuilder`,`BitArray`,`NameValueCollection`,`Version`,`Uri`,`TimeSpan`,`DBNull`,`DataTable`...
* `Dictionary<,>`,`IDictionary<,>`,`Hashtable`,`SortedDictionary<,>`,`ReadOnlyDictionary<,>`,`ConcurrentDictionary<,>`,`IReadOnlyDictionary<,>`,`SortedList<,>`...
* `Array[]`,`Array[,]`,`Array[,,]`,`Array[,,,]`,`ArraySegment<>`,`IList`,`ArrayList`,`LinkedList<>`,`Queue<>`,`Stack<>`,`ISet<>`,`HashSet<>`,`ReadOnlyCollection<>`,`ICollection<>`,`IEnumerable<>`,`IReadOnlyCollection<>`，`IReadOnlyList<>`,`Collection<>`,`ConcurrentQueue<>`,`ConcurrentStack<>`,`ConcurrentBag<>`...
* Any IDictionary : Bssom.Net summarizes the characteristics of IDictionary types in FCL and abstracts the corresponding rules. Any type with equivalent (serialization and deserialization) IDictionary characteristics (behavior or constructor) can be key-valued(Map) Parse the format
* Any ICollection : Bssom.Net summarizes the characteristics of ICollection types in FCL and abstracts the corresponding rules. Any type with equivalent (serialization and deserialization) ICollection characteristics (behavior or constructor) can be parsed in a collection format

## 5.Extensions

Let's take a look at the process of Bssom.Net serialization ：

     input T -> Call serialize(T) -> Find BssomResolver -> Provide type formatter -> formatter.Serialize(T);
    
In the entire serialization process, each step is **transparent**, which means that if users are not satisfied with the parser or formatter defined in Bssom.Net, they can extend it themselves.

Users can **replace** the default resolver by implementing `IFormatterResolver` and `IBssomFormatter`,  In `Bssom.Serializer.Binary.BssomBinaryPrimitives`(This class will be refactored in the upcoming minor version) and Low-level write and read implementations of the Bssom format are provided in the exposed public apis of the reader-writer itself

Simple examples can refer to [More possibilities](#10more-possibilities)

## 6.High-Level API

### BssomSerializer

`BssomSerializer` is the top-level API of Bssom. Under the `Bssom.Serializer` namespace, it is the entry point for Bssom to start working. Its static methods constitute the main API of Bssom.Net.

API     |    Description   |  Overload
--------|----------|-------------
<a name="sizeapi">Size</a>    | Without serialization, get the binary data size of the object after serialization |(t, option),(ref context, t)
<a name="serializeapi">Serialize</a> | Serializes the given value to the Bssom binary  | (byte[], t, option), (stream, t, option), (IBssomBufWriter, t, option), (ref context, t)
<a name="deserializeapi">Deserialize</a> | Deserialize the Bssom binary data into a.NET object  | (byte[], option),(stream, option),(IBssomBuf, option),(ref context)
SerializeAsync | Asynchronous serialization of the given value to Bssom binary  | Same as above
DeserializeAsync | Asynchronous deserialize the Bssom binary data into a.NET object  | Same as above

### BssomSerializerOptions
`BssomSerializer` is the top-level API. When calling it, we need to pass a nullable Option parameter of `BssomSerializerOptions` type.  
`BssomSerializerOptions` is the configuration that Bssom needs to use during the entire serialization work. default is `BssomSerializerOptions.Default`.  
-  **FormatterResolver** : In Option, you can register a resolver for `FormatterResolver`. If it is not manually registered, the default `CompositedResolver` will be used. Bssom will always use `FormatterResolver` to resolve types.
-  **Security** : This is a **security-related** option during serialization. Currently, it only provides deep verification during deserialization. **The default is no limit**.
-  **IsPriorityToDeserializeObjectAsBssomValue** : This option determines whether to convert the Object type to the BssomValue type during deserialization. If it is `false`, the default deserialization is the original type.   The default is `false`.
-  **IsUseStandardDateTime** : Bssom.Net implements the standard Bssom protocol **Unix format** and the **native format** of the .NET platform for the `DateTime` type. The native format has fewer bytes but does not have interactivity with other platforms. The default is `false`.
-  **IDictionaryIsSerializeMap1Type** : This option determines which format is used by default for serialization of types with `IDictionary` behavior. If it is `true`, use the `Map1` format, otherwise it is the `Map2` format.  The default is `true`

### BssomSerializeContext
`BssomSerializeContext` provides context information used during serialization, which also includes `BssomSerializerOptions`
-  **BssomSerializerOptions** : **Configuration information** used during serialization
-  **ContextDataSlots**<a name="contextdataslots"></a> : Provides a data slot, a **storage medium** for users to store and read by themselves during serialization
-  **CancellationToken** : A serialization operation canceled flag, Users can **cancel in midway** the ongoing serialization operation

## 7.Field marshal
Bssom.Net has functions to read fields without completely deserializing and changing values ​​without completely serializing, This is because [Bssom protocol](https://github.com/1996v/Bssom) has good structural features, In the implementation of Bssom.Net, such functions are exposed in `BssomFieldMarshaller`.

### BssomFieldMarshaller
`BssomFieldMarshaller` provides a set of APIs for lower-granularity control of serialized data.

API  |  Description   
-----|--------
IndexOf | Get the position of the specified object in the Bssom binary through a special input format,  return the offset information
ReadValue | Read the entire element through the specified offset information
ReadValueType | Read only the element type through the specified offset information
ReadValueTypeCode | Read only the binary code of the element type through the specified offset information
ReadValueSize | Get the size of the element stored in the Bssom binary through the specified offset information
ReadArrayCountByArrayType | Read the count of elements of the BssomArray through the specified offset information
ReadAllKeysByMapType | Read the metadata in the BssomMap (including the offset of value and key) through the specified offset information
TryWrite | Rewrite the value in the Bssom binary with the specified offset information. If the width of the written value is greater than the width of the slot to be written, it will fail

Each method provides overloads of `byte[]` and `IBssomBuf`

### Simple Field access language
Bssom.Net defines a simple field access language for `IndexOf`. The language defines two access forms, one is to access the `Map` type (the key of the Map type must be the `String` type), one One is to access the `Array` type. The two access forms can be freely combined.
-  [Key] : Represents access to the value of `Map` type through `Key`, and the input `Key` only represents `String` type
-  $Index : Represents access to elements of type `Array` through subscripts, the input Index can only be an integer type

Assuming the following data:
```c#
{
   "Postcodes" : {   
		  "WuHan" : [430070,430071,430072,430073],
		  "XiangYang" : [441000,441001,441002]
		},
   "Province" : "HuBei"
}
```
You can access elements in the following ways, more details can be found in [Example](#readvalue)
```c#
[Postcodes][WuHan]$1  => 4330071
[Province]  => "HuBei"
```

### Custom Field access interface

Bssom.Net provides the `IIndexOfInputSource` interface for `IndexOf` to receive custom field access sources. After using this interface, Map type Key will no longer be restricted, and Key can be any input type.  

`IndexOfObjectsInputSource` is a general implementation of the `IIndexOfInputSource` interface provided by Bssom.Net for users. It receives a set of iterable objects, and when IndexOf is called, the objects will be iterated in turn.

Assuming the following data:
```c#
{
   2018-01-01 : {
         0 : ["Rain1","Rain2","Rain3"],
         4 : ["Rain4","Fair5","Fair6"]   
    }
}
```
You can access elements in the following ways, more details can be found in [Example](#readvalue)
```c#
new IndexOfObjectsInputSource(new Entry[]{ 
     new Entry(DateTime.Parse("2018-01-01"),ValueIsMapKey: true),
     new Entry(3,ValueIsMapKey: true),
     new Entry(1,ValueIsMapKey: false),
  })

output => "Fair5"
```

## 8.Dynamic code generation

Bssom.Net uses dynamic code generation technology for `IDictionaryResolver`, `ICollectionResolver`, `MapCodeGenResolver`, and `ObjectResolver` to generate runtime code through **expression tree and Emit**. If the application is a pure AOT environment, Will not support.

In the `MapCodeGenResolver`, the deserialization of the `Map1` type uses the automaton search mode of the class prefix tree in units of 8 bytes (64-bit word length). This is a very effective and fast way, which avoids Perform complete Hash operation and character comparison overhead on strings. You will see these automatically generated codes through the `MapCodeGenResolver.Save()` method. 

![](https://user-images.githubusercontent.com/30827194/97230916-b2518980-1815-11eb-891d-12fee0f2fe0a.png)

The deserialization of `Map2` type in `MapCodeGenResolver` uses the built-in [Bssom protocol](https://github.com/1996v/Bssom) Map format search code, which is written in state machine mode, divided into For the fast and low-speed version, it depends on whether [reader](#ibssombuffer) can provide [TryReadFixedRef](#tryreadfixedref).
![](https://user-images.githubusercontent.com/30827194/97229613-99e06f80-1813-11eb-98ca-db941ce3d6d3.png)

In addition, for the `Size` method, the processing of MapCodeGenResolver is also very fast, because it has calculated the size of the metadata in advance, and inlined the fixed size of the primitive field itself.
![](https://user-images.githubusercontent.com/30827194/97229619-9e0c8d00-1813-11eb-8954-df92e96c7d18.png)

## 9.Attributes

There are currently 6 Attributes in Bssom.Net. :
-  **AliasAttribute** : Alias ​​Attribute, used to modify the field name saved in the binary of the Map format object field
-  **BssomFormatterAttribute** : Custom formatting Attribute, when the field property or type is marked by this Attribute, this type of formatting will use the formatter specified by this Attribute
-  **IgnoreKeyAttribute** : Ignore a certain Key, the marked field will be ignored during serialization, applicable to Map format
-  **OnlyIncludeAttribute** : 仅包含某一个Key，序列化时仅包含该Key，适用于Map格式，与`IgnoreKeyAttribute`作用相反，优先级更高
-  **SerializationConstructorAttribute** : 为类型的反序列化指定一个构造函数
-  **KeyAttribute** ：<a name="keyattribute"></a>使用`IntKeyCompositedResolver`为对象进行解析时，需要为对象中的元素标记Key下标

## 10.更多的可能性

你可以自己编写[解析器](#4解析器)，编写[格式化器](#3格式化器)，也可以定义你自己的特性，也可以封装用于序列化的[Option](#bssomserializeroptions)，并且Bssom.Net还提供了上下文[数据槽](#contextdataslots)的支持， 这可以让序列化行为变得多样性.  

如果你能为Bssom.Net提供有用或者侧重于高性能的**扩展包**， 那么请您告诉我.  

下面示例编写了以String类型为原型的解析器， 该解析器通过与上下文交互的方式来带来字符串类型序列化性能的提升.
```c#
public sealed class MyStringFormatterResolver : IFormatterResolver
{
    public static MyStringFormatterResolver Instance = new MyStringFormatterResolver();

    public IBssomFormatter<T> GetFormatter<T>()
    {
        return FormatterCache<T>.Formatter;
    }

    private static class FormatterCache<T>
    {
        public static readonly IBssomFormatter<T> Formatter;

        static FormatterCache()
        {
            if (typeof(T) == typeof(string))
                Formatter = (IBssomFormatter<T>)(object)MyStringFormatter.Instance;
        }
    }
}

```
```c#
public sealed class MyStringFormatter : IBssomFormatter<string>
{
    public static MyStringFormatter Instance = new MyStringFormatter();

    public string Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
    {
        if (reader.TryReadNull())
        {
            return null;
        }

        reader.EnsureType(BssomType.StringCode);
        int dataLen = reader.ReadVariableNumber();
        ref byte refb = ref reader.BssomBuffer.ReadRef((int)dataLen);
        fixed (byte* pRefb = &refb)
        {
            return new string((sbyte*)pRefb, 0, (int)dataLen, UTF8Encoding.UTF8);
        }
    }

    public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, string value)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }

        int valueUtf8Size = context.ContextDataSlots.PopMyStringSize();

        writer.WriteBuildInType(BssomType.StringCode);
        writer.WriteVariableNumber(valueUtf8Size);

        ref byte refb = ref writer.BufferWriter.GetRef(valueUtf8Size);
        fixed (char* pValue = value)
        fixed (byte* pRefb = &refb)
        {
            UTF8Encoding.UTF8.GetBytes(pValue, value.Length, pRefb, valueUtf8Size);
        }
        writer.BufferWriter.Advance(valueUtf8Size);
    }

    public int Size(ref BssomSizeContext context, string value)
    {
        if (value == null)
            return BssomBinaryPrimitives.NullSize;

        int dataSize = UTF8Encoding.UTF8.GetByteCount(value);
        context.ContextDataSlots.PushMyStringSize(dataSize);
        return BssomBinaryPrimitives.BuildInTypeCodeSize + dataSize;
    }
}
```
```c#
public void MyTest()
{
     var option = BssomSerializerOptions.Default.WithFormatterResolver(MyStringFormatterResolver.Instance);
     string str = RandomHelper.RandomValue<string>();
     BssomSizeContext sizeContext = new BssomSizeContext(option);
     int len = BssomSerializer.Size(ref sizeContext, str);
     if (len > 1000)
         throw new Exception("Size of value storage binary exceeded");

     BssomSerializeContext serContext = new BssomSerializeContext(option);
     sizeContext.ContextDataSlots.SetMyStringStack(serContext.ContextDataSlots);
     var bytes = BssomSerializer.Serialize(ref serContext, str);
     var deStr = BssomSerializer.Deserialize<string>(bytes);

     Assert.Equal(str,deStr);
}
```
上面的代码是单独为String定义了一个新的解析器和新的格式化器，该格式化器可以将Size方法中对字符串计算的UTF8大小存储在上下文中，      这样在序列化时不用重复对String再做一次UTF8大小计算.


## 11.如何使用

Bssom.Net默认是无合约的，开箱即用，这里有些示例代码.

### Size
[BssomSerializer.Size](#sizeapi) 方法用于 获取对象被序列化后的二进制数据大小，高性能的内部实现，几乎无开销
```c#
//获取值被序列化后的大小
object value = RandomHelper.RandomValue<object>();
int size = BssomSerializer.Size(value, option: BssomSerializerOptions.Default);
```
```c#
//使用上下文获取值被序列化后的大小
BssomSizeContext context = new BssomSizeContext(BssomSerializerOptions.Default);
object value = RandomHelper.RandomValue<object>();
int size = BssomSerializer.Size(ref context, value);
```

### Serialize
[BssomSerializer.Serialize](#serializeapi) 方法用于 将给定的值序列化为Bssom二进制，高性能的内部实现，以下是部分常用方法，每个方法都拥有CancellationToken的重载(CancellationToken可以中断正在进行的序列化操作)。
```c#
//直接对对象进行序列化,将返回一个被序列化后的字节数组
object value = RandomHelper.RandomValue<object>();
byte[] binary = BssomSerializer.Serialize(value, option: BssomSerializerOptions.Default);
```
```c#
//将对象序列化到指定的字节数组中,若容量不够将自动扩容,最终返回序列化的字节数
object value = RandomHelper.RandomValue<object>();
byte[] buf = local();
int serializeSize = BssomSerializer.Serialize(ref buf, 0, value, option: BssomSerializerOptions.Default);
```
```c#
//将对象序列化到自定义的写入器中
object value = RandomHelper.RandomValue<object>();
IBssomBufferWriter writer = new Impl();
BssomSerializer.Serialize(value, writer, option: BssomSerializerOptions.Default);
```
```c#
//使用序列化上下文进行序列化
object value = RandomHelper.RandomValue<object>();
BssomSerializeContext context = new BssomSerializeContext(BssomSerializerOptions.Default);
byte[] binary = BssomSerializer.Serialize(ref context, value);
```
```c#
//将对象序列化到流中
object value = RandomHelper.RandomValue<object>();
Stream stream = new MemoryStream();
BssomSerializer.Serialize(stream, value, option: BssomSerializerOptions.Default);
```
```c#
//异步的将对象序列化到流中
object value = RandomHelper.RandomValue<object>();
Stream stream = new MemoryStream();
await BssomSerializer.SerializeAsync(stream, value, option: BssomSerializerOptions.Default);
```
### Deserialize
[BssomSerializer.Deserialize](#deserializeapi) 方法用于 将给定的Bssom缓冲区反序列化为对象，高性能的内部实现，以下是部分常用方法，每个方法都拥有CancellationToken的重载(CancellationToken可以中断正在进行的序列化操作)。
```c#
//从给定的字节数组中反序列化对象
byte[] buf = remote();
T value = BssomSerializer.Deserialize<T>(buf, 0, out int readSize, option: BssomSerializerOptions.Default);
```
```c#
//从给定的buffer中反序列化对象
IBssomBuffer buffer = remote();
object value = BssomSerializer.Deserialize<object>(buffer, option: BssomSerializerOptions.Default);
```
```c#
//使用上下文从给定的buffer中反序列化对象
BssomDeserializeContext context = new BssomDeserializeContext(BssomSerializerOptions.Default);
IBssomBuffer buffer = remote();
object value = BssomSerializer.Deserialize<object>(ref context, buffer);
```
```c#
//从流中反序列化对象
Stream stream = remote();
object value = BssomSerializer.Deserialize<object>(stream, option: BssomSerializerOptions.Default);
```
```c#
//异步的从流中反序列化对象
Stream stream = remote();
object value = await BssomSerializer.DeserializeAsync<object>(stream, option: BssomSerializerOptions.Default);
```
```c#
//传递一个Type, 从流中反序列化对象为指定的Type类型
Stream stream = remote();
Type type = typeof(class);
object value = BssomSerializer.Deserialize(stream, type, option: BssomSerializerOptions.Default);
```
```c#
//传递一个Type, 异步的从流中反序列化对象为指定的Type类型
Stream stream = remote();
Type type = typeof(class);
object value = await BssomSerializer.DeserializeAsync(stream, type, option: BssomSerializerOptions.Default);
```

### ReadValue
[BssomFieldMarshaller.ReadValue](#bssomfieldmarshaller) 方法用于 在二进制数据中仅读取某一个值，如果你只想读取对象中的某一个值，而不用完整的反序列化它，那么这个方法非常有用

```c#
//通过内嵌的简单字段访问语言,获取Dict中的一个Key对应的值
var val = new Dictionary<string, object>() {
            { "A",(int)3},
            { "B",(DateTime)DateTime.MaxValue},
        };
var buf = BssomSerializer.Serialize(val);
var bsfm = new BssomFieldMarshaller(buf);
BssomFieldOffsetInfo fieldOffInfo = bsfm.IndexOf("[A]")
bsfm.ReadValue<int>(fieldOffInfo).Is(3);
```
```c#
//通过内嵌的简单字段访问语言,获取class中的一个属性的值
var val = new MyClass() {
            Name = "bssom",
            Nature = "Binary"
        };
var buf = BssomSerializer.Serialize(val);
var bsfm = new BssomFieldMarshaller(buf);
BssomFieldOffsetInfo fieldOffInfo = bsfm.IndexOf("[Name]")
bsfm.ReadValue<string>(fieldOffInfo).Is("bssom");
```
```c#
//通过内嵌的简单字段访问语言,获取数组中的一个属性的值
var val = new object[] { (int)1,(double)2.2 }
var buf = BssomSerializer.Serialize(val);
var bsfm = new BssomFieldMarshaller(buf);
BssomFieldOffsetInfo fieldOffInfo = bsfm.IndexOf("$1")
bsfm.ReadValue<double>(fieldOffInfo).Is((double)2.2);
```
```c#
//通过内嵌的简单字段访问语言,组合获取一个对象
var val = new MyClass() {
            Name = "bssom",
            Nature = "Binary",
            Data = new int[] { 3, 2, 1} 
        };
var buf = BssomSerializer.Serialize(val);
var bsfm = new BssomFieldMarshaller(buf);
BssomFieldOffsetInfo fieldOffInfo = bsfm.IndexOf("[Data]$1")
bsfm.ReadValue<int>(fieldOffInfo).Is(2);
```
```c#
//通过内嵌的简单字段访问语言,以下标方式,获取一个对象
public class MarkKeyClass
{
    [Key(0)]
    public string Name;
    [Key(1)]
    public string Nature;
}

var val = new MarkKeyClass() {
            Name = "bssom",
            Nature = "Binary",
        };
var buf = BssomSerializer.Serialize(val, option = BssomSerializerOptions.IntKeyCompositedResolverOption);
var bsfm = new BssomFieldMarshaller(buf);//buf is array3 format
BssomFieldOffsetInfo fieldOffInfo = bsfm.IndexOf("$1")
bsfm.ReadValue<string>(fieldOffInfo).Is("Binary");
```
```c#
//通过自定义的字段访问形式,组合获取一个对象
var val = new Dictionary<object, object>() {
            { DateTime.Parse("2018-01-01"), new object[]{'A','B'} },
            { "Charec",(DateTime)DateTime.MaxValue},
        };
var buf = BssomSerializer.Serialize(val);
var bsfm = new BssomFieldMarshaller(buf);
IIndexOfInputSource input = new IndexOfObjectsInputSource(new Entry[]{ 
     new Entry(DateTime.Parse("2018-01-01"),ValueIsMapKey: true),
     new Entry(1,ValueIsMapKey: false),
  })
BssomFieldOffsetInfo fieldOffInfo = bsfm.IndexOf(input)
bsfm.ReadValue<int>(fieldOffInfo).Is('B');
```
### ReadAllMapKeys
[BssomFieldMarshaller.ReadAllMapKeys](#bssomfieldmarshaller) 方法用于 在二进制数据中读取Map格式的所有Key和值偏移量，如果你想了解该二进制数据中的键值情况，但又不想完全读取它，那么这个方法非常有用.
```c#
var val = new Dictionary<object, object>(){
           { "Id" , 1 },
           { "Path" , "../t.jpg" },
           { "Data" , new byte[3000] }
};
var buf = BssomSerializer.Serialize(val);
var bsfm = new BssomFieldMarshaller(buf);
bsfm.ReadAllMapKeys<object>(BssomFieldOffsetInfo.Zero).Print();
//output
//  line 1: BssomString::"Id", BssomFieldOffsetInfo
//  line 2: BssomString::"Path", BssomFieldOffsetInfo
//  line 3: BssomString::"Data", BssomFieldOffsetInfo
```

### TryWriteValue
[BssomFieldMarshaller.TryWriteValue](#bssomfieldmarshaller) 方法用于 对二进制数据的值进行修改，当你只想修改对象中的某个值，而不用重新序列化整个对象时，那么这个方法非常有用
```c#
//修改字符串对象
var val = "abcd";
var buf = BssomSerializer.Serialize(val);
var bsfm = new BssomFieldMarshaller(buf);
bsfm.TryWrite(BssomFieldOffsetInfo.Zero, "abc");
string upVal = BssomSerializer.Deserialize<string>(buf);
upVal.Is("abc");
```
```c#
//修改IDict对象中的某个键
var val = new Dictionary<string, object>(){
           { "Id" , 1 },
           { "Path" , "../t.jpg" },
           { "Data" , new byte[3000] }
};
var buf = BssomSerializer.Serialize(val);
var bsfm = new BssomFieldMarshaller(buf);

bsfm.TryWrite(bsfm.IndexOf("[Id]"), 3);
var upVal = BssomSerializer.Deserialize<Dictionary<string, object>>(buf);
upVal["Id"].Is(3);
```
```c#
//修改IDict对象中的某个键
var val = new MyClass() {
            Name = "bssom",
            Nature = "Binary",
            Data = new int[] { 3, 2, 1} 
        };
var buf = BssomSerializer.Serialize(val);
var bsfm = new BssomFieldMarshaller(buf);

bsfm.TryWrite(bsfm.IndexOf("[Name]"), "zz");
var upVal = BssomSerializer.Deserialize<MyClass>(buf);
upVal["Name"].Is("zz");
```
```c#
//修改Array对象中的某个元素
var val = new object[] { "abc" , 37 };
var buf = BssomSerializer.Serialize(val);
var bsfm = new BssomFieldMarshaller(buf);

bsfm.TryWrite(bsfm.IndexOf("$1"), 40);
var upVal = BssomSerializer.Deserialize<MyClass>(buf);
((int)upVal[1]).Is(40);
```
```c#
//组合修改对象中的某个元素
var val = new object[] { 
        22, 
        37, 
        new MyClass() {
            Name = "bssom",
            Nature = "Binary",
            Data = new int[] { 3, 2, 1} 
        }
 };
var buf = BssomSerializer.Serialize(val);
var bsfm = new BssomFieldMarshaller(buf);

bsfm.TryWrite(bsfm.IndexOf("$2[Name]"), "zz");
var upVal = BssomSerializer.Deserialize<MyClass>(buf);
((MyClass)upVal[1]).Name.Is("zz");
```
### Other
以上代码示例主要演示了 **序列化**(`BssomSerializer.Serialize`) 和 **元素编组**(`BssomFieldMarshaller`) 两个方面，Bssom.Net还有如下方法未在演示中出现：
- BssomFieldMarshaller.ReadValueSize : 从缓冲区中获取下一个对象的二进制大小
- BssomFieldMarshaller.ReadValueType : 从缓冲区中获取下一个对象的[Bssom类型](https://github.com/1996v/Bssom#%E7%B1%BB%E5%9E%8B%E7%B3%BB%E7%BB%9F)
- BssomFieldMarshaller.ReadValueTypeCode : 从缓冲区中获取下一个对象的[Bssom类型码](https://github.com/1996v/Bssom#%E6%A6%82%E8%BF%B0)
- BssomFieldMarshaller.ReadArrayCountByArrayType ：从缓冲区中获取下一个数组对象的元素数量
- BssomFieldMarshaller.IndexOfArray3Item : 获取Array3格式中指定下标元素的偏移量


### [如何使用特性](#9特性)
### [如何定义扩展](#5扩展)

## 12.局限性
Bssom协议要求给定一个Key，能够在Map对象的二进制数据中查找到指定Key对应的值。这意味着需要将给定的Key在Map对象的二进制数据中进行比较.

在Bssom.Net中，二进制层面实现这样的比较是通过将Key转化为一定大小的二进制数据，然后在Map对象的二进制数据中进行对比比较.  
将Key转换为二进制数据， 该数据的大小 在Bssom中称为宽度。 而考虑到Bssom协议对类型进行了严格设定，因此在Bssom.Net中， 对于Map格式的Key类型将只支持 `String`,`sbyte`,`Int16`,`Int32`,`Int64`,`byte`,`UInt16`,`UInt32`,`UInt64`,`Single`,`Double`,`Boolean`,`Char`,`Decimal`,`Guid`,`DateTime` 这16个类型.

### BssomSerializationArgumentException.BssomMapKeySame
在Map中不能存在相同的Key，Bssom.Net是通过宽度值(转换后的二进制数据)来进行相等(长度,每个byte值)判断的。如果存在相同宽度值的Key，则会出现此异常`BssomSerializationArgumentException.BssomMapKeySame`  

* 如果Key为String类型， 则其宽度值为 UTF8编码下的二进制数据表示。因为宽度值不能为空，因此String类型不允许String.Empty的值
* 如果Key为整数类型， 则其宽度值为 其整数的二进制的补码数据表示  
* 如果Key为浮点类型， 则其宽度值为 浮点格式的二进制表示  
* 如果Key为Char类型，则其宽度值为 两个字节的UTF16格式表示
* 如果Key为Decimal,Guid,DateTime，则其宽度值为 类型宽度的块格式表示   

在实现一个散列结构时，桶中装载的Entry结构除了拥有HashCode外还有额外的Key对象，这是用来防止HashCode在碰撞的情况下能够通过Key对象来进行最终的判断的比较.  
而在Bssom.Net实现的宽度值中， 也同样会出现碰撞的现象， 例如 Int类型和UInt类型的宽度都是4个字节， 若都表示同一个正整数的话，那么其宽度值也是相同的， 这种情况下就会产生碰撞现象， 那么就会出现`BssomMapKeySame`异常。 因此， 当出现`BssomMapKeySame`异常时， 若Key的类型为Object的情况下， 那么需要检查需要写入的Map中是否存在不同类型的相同宽度值情况存在.


## 13.如何参与项目贡献
### 如果你想参与本项目的发展，那么我将非常荣幸和高兴，欢迎Fork或Pull Request，也可以加入QQ群976304396来进行开源技术的探讨
#### 点击加入群聊[.NET开源技术交流群](https://jq.qq.com/?_wv=1027&k=R5cEtIdl) 禁水，只能聊技术

## 14.谁在使用
* **BssomDB(即将开源)**   一个使用Bssom协议的纯C#的嵌入式事务型文档数据库
