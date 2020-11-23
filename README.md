## A small, high performance, powerful serializer using bssom binary protocol [中文Readme](https://github.com/1996v/Bssom.Net/blob/master/README.CN.md)
[![Nuget](https://img.shields.io/nuget/v/BssomSerializer.svg)](https://www.nuget.org/packages/BssomSerializer/) 
### 
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
-  **OnlyIncludeAttribute** : Applicable to the Map format, only the elements marked with this Attribute will be serialized during serialization，as opposed to `IgnoreKeyAttribute`, but at a higher priority
-  **SerializationConstructorAttribute** : Specify a constructor for type deserialization
-  **KeyAttribute** ：<a name="keyattribute"></a>When using `IntKeyCompositedResolver` to resolve an object, you need to mark the Key Index for the element in the object

## 10.More possibilities

You can code your own [Resolver](#4resolver)，code [Formatter](#3formatter)，you can also define your own `BssomFormatterAttribute`，You can also encapsulate the [Option](#bssomserializeroptions)，and Bssom.Net also provides support for context [Data slot](#contextdataslots), which can make serialization behavior diversified.  

If you can provide Bssom.Net with useful or high-performance **extensions**, then please let me know.

The following example writes a parser based on the String type. The parser interacts with the context to improve the serialization performance of the string type.
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
The above code separately defines a new parser and a new formatter for String, which can store the UTF8 size calculated on the string in the Size method in the context，in this way, there is no need to repeat the UTF8 size calculation on the String during serialization.


## 11.How to use

Bssom.Net is contractless by default and can be used directly. Here are some sample codes.

### Size
[BssomSerializer.Size](#sizeapi) method is used to obtain the binary data size of the object after serialization, a high-performance internal implementation, with almost no overhead
```c#
//Gets the serialized size of the value
object value = RandomHelper.RandomValue<object>();
int size = BssomSerializer.Size(value, option: BssomSerializerOptions.Default);
```
```c#
//Gets the serialized size of the value using the context
BssomSizeContext context = new BssomSizeContext(BssomSerializerOptions.Default);
object value = RandomHelper.RandomValue<object>();
int size = BssomSerializer.Size(ref context, value);
```

### Serialize
[BssomSerializer.Serialize](#serializeapi) method is used to serialize the given value into Bssom binary, a high-performance internal implementation. The following are some of the commonly used methods. Each method has an overload of CancellationToken (CancellationToken can interrupt the ongoing serialization operation).
```c#
//Serialize the object directly and return a serialized byte array
object value = RandomHelper.RandomValue<object>();
byte[] binary = BssomSerializer.Serialize(value, option: BssomSerializerOptions.Default);
```
```c#
//Serialize the object to the specified byte array, if the capacity is not enough, it will automatically expand, and finally return the number of serialized bytes
object value = RandomHelper.RandomValue<object>();
byte[] buf = local();
int serializeSize = BssomSerializer.Serialize(ref buf, 0, value, option: BssomSerializerOptions.Default);
```
```c#
//Serialize the object into a custom writer
object value = RandomHelper.RandomValue<object>();
IBssomBufferWriter writer = new Impl();
BssomSerializer.Serialize(value, writer, option: BssomSerializerOptions.Default);
```
```c#
//Serialization using serialization context
object value = RandomHelper.RandomValue<object>();
BssomSerializeContext context = new BssomSerializeContext(BssomSerializerOptions.Default);
byte[] binary = BssomSerializer.Serialize(ref context, value);
```
```c#
//Serialize the object into the stream
object value = RandomHelper.RandomValue<object>();
Stream stream = new MemoryStream();
BssomSerializer.Serialize(stream, value, option: BssomSerializerOptions.Default);
```
```c#
//Serialize the object to the stream asynchronously
object value = RandomHelper.RandomValue<object>();
Stream stream = new MemoryStream();
await BssomSerializer.SerializeAsync(stream, value, option: BssomSerializerOptions.Default);
```
### Deserialize
[BssomSerializer.Deserialize](#deserializeapi) method is used to deserialize a given Bssom buffer into an object, high-performance internal implementation, the following are some common methods, each method has an overload of CancellationToken (CancellationToken can interrupt the ongoing serialization operation).
```c#
//Deserialize the object from the given byte array
byte[] buf = remote();
T value = BssomSerializer.Deserialize<T>(buf, 0, out int readSize, option: BssomSerializerOptions.Default);
```
```c#
//Deserialize the object from the given buffer
IBssomBuffer buffer = remote();
object value = BssomSerializer.Deserialize<object>(buffer, option: BssomSerializerOptions.Default);
```
```c#
//Use the context to deserialize the object from the given buffer
BssomDeserializeContext context = new BssomDeserializeContext(BssomSerializerOptions.Default);
IBssomBuffer buffer = remote();
object value = BssomSerializer.Deserialize<object>(ref context, buffer);
```
```c#
//Deserialize objects from the stream
Stream stream = remote();
object value = BssomSerializer.Deserialize<object>(stream, option: BssomSerializerOptions.Default);
```
```c#
//Deserialize objects from the stream asynchronously
Stream stream = remote();
object value = await BssomSerializer.DeserializeAsync<object>(stream, option: BssomSerializerOptions.Default);
```
```c#
//Pass a Type, deserialize the object from the stream to the specified Type type
Stream stream = remote();
Type type = typeof(class);
object value = BssomSerializer.Deserialize(stream, type, option: BssomSerializerOptions.Default);
```
```c#
//Pass a Type, asynchronously deserialize the object from the stream to the specified Type type
Stream stream = remote();
Type type = typeof(class);
object value = await BssomSerializer.DeserializeAsync(stream, type, option: BssomSerializerOptions.Default);
```

### ReadValue
[BssomFieldMarshaller.ReadValue](#bssomfieldmarshaller) method is used to read only a certain value in the binary data. If you only want to read a certain value in the object without completely deserializing it, then this method is very useful

```c#
//Access the language through the simple field to get the value corresponding to a Key in Dict
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
//Access the language through the simple field to get the value corresponding to a Property in Class
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
//Access the language through the simple field to get the value corresponding to a Element in Array
var val = new object[] { (int)1,(double)2.2 }
var buf = BssomSerializer.Serialize(val);
var bsfm = new BssomFieldMarshaller(buf);
BssomFieldOffsetInfo fieldOffInfo = bsfm.IndexOf("$1")
bsfm.ReadValue<double>(fieldOffInfo).Is((double)2.2);
```
```c#
//Access language through simple fields, combine to get an object
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
//Access the language through a simple field, index to get an object
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
//Gets an object through a custom form of field access
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
[BssomFieldMarshaller.ReadAllMapKeys](#bssomfieldmarshaller) method is used to read all the keys and value offsets of the Map format in the binary data. This method is very useful if you want to know the key value in the binary data, but do not want to read it completely.
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
[BssomFieldMarshaller.TryWriteValue](#bssomfieldmarshaller) method is used to modify the value of binary data. This method is very useful when you only want to modify a value in the object without re-serializing the entire object.
```c#
//Modify string objects
var val = "abcd";
var buf = BssomSerializer.Serialize(val);
var bsfm = new BssomFieldMarshaller(buf);
bsfm.TryWrite(BssomFieldOffsetInfo.Zero, "abc");
string upVal = BssomSerializer.Deserialize<string>(buf);
upVal.Is("abc");
```
```c#
//Modify a key in the Dictionary object
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
//Modify a key in the Dictionary object
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
//Modify an element in the Array object
var val = new object[] { "abc" , 37 };
var buf = BssomSerializer.Serialize(val);
var bsfm = new BssomFieldMarshaller(buf);

bsfm.TryWrite(bsfm.IndexOf("$1"), 40);
var upVal = BssomSerializer.Deserialize<MyClass>(buf);
((int)upVal[1]).Is(40);
```
```c#
//A composition modifies an element in an object
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
The above code sample mainly demonstrates the two aspects of **Serialization**(`BssomSerializer.Serialize`) and element **Marshall**(`BssomFieldMarshaller`)，Bssom.Net also has the following methods that did not appear in the demo：
- BssomFieldMarshaller.ReadValueSize : Get the binary size of the next object from the buffer
- BssomFieldMarshaller.ReadValueType : Get the [Bssom type](https://github.com/1996v/Bssom#%E7%B1%BB%E5%9E%8B%E7%B3%BB%E7%BB%9F) of the next object from the buffer
- BssomFieldMarshaller.ReadValueTypeCode : Get the [Bssom type code](https://github.com/1996v/Bssom#%E6%A6%82%E8%BF%B0) of the next object from the buffer
- BssomFieldMarshaller.ReadArrayCountByArrayType ：Get the number of elements of the next array object from the buffer
- BssomFieldMarshaller.IndexOfArray3Item : Get the offset of the specified Index element in Array3 format


### [How to contribute](#9attributes)
### [How to define an extension](#5extensions)

## 12.Limitations
The Bssom protocol requires that given a Key, the value corresponding to the specified Key can be found in the binary data of the Map object. This means that the given Key needs to be compared in the binary data of the Map object.

In Bssom.Net, the binary level realizes such a comparison by converting the Key into binary data of a certain size, and then comparing and comparing in the binary data of the Map object.  
Convert Key to binary data, the size of this data is called width in Bssom. Given the strict type setting of the Bssom protocol, Therefore in Bssom.Net, the Key type of Map format will only support these 16 types : `String`,`sbyte`,`Int16`,`Int32`,`Int64`,`byte`,`UInt16`,`UInt32`,`UInt64`,`Single`,`Double`,`Boolean`,`Char`,`Decimal`,`Guid`,`DateTime` .

### BssomSerializationArgumentException.BssomMapKeySame
The same Key cannot exist in the Map. Bssom.Net judges the equality (length, each byte value) through the width value (converted binary data). This `BssomSerializationArgumentException.BssomMapKeySame` exception occurs if there are keys with the same width value.

* If Key is `string`, its width is the binary data representation encoded in UTF8.Because the width value cannot be Empty, the String type does not allow string.empty values
* If Key is an integer type, its width value is its little-endian data representation
* If Key is `float\double`, its width value is the binary representation of floating-point format  
* If Key is `char`, its width value is a UTF16 representation of two bytes
* If Key is `Decimal`, Guid, DateTime, its width value is the block format representation of type width

When implementing a hash structure, the entry structure loaded in the bucket has additional key objects besides hashcode, which is used to prevent hashcode from making final judgment through key in case of collision.  
And in the Bssom In the width value of the implementation, the phenomenon of collision will also appear，For example, the width of Int type and UInt type are both 4 bytes. If they both represent the same positive integer, then the width value is also the same. In this case there will be a collision phenomenon, then there will be a `BssomMapKeySame` exception. Therefore, when a `BssomMapKeySame` exception occurs, if the Key type is Object, it is necessary to check whether there are different types of the same width value in the Map to be written.


## 13.How to contribute
### If you want to participate in the development of this project, then I will be very honored and happy, welcome Fork or Pull Request, there is a [project](https://github.com/1996v/Bssom.Net/projects) plan that I temporarily drafted in the projects

## 14.Who is using
* **BssomDB(Open source soon)**  : A pure C# embedded transactional document database using Bssom protocol
