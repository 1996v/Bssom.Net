## A small, high performance, powerful serializer using bssom binary protocol
[![Nuget](https://img.shields.io/nuget/v/BssomSerializer.svg)](https://www.nuget.org/packages/BssomSerializer/) 

**Bssom.Net**是一个使用**BSSOM协议**实现的高性能结构化二进制**序列化器**，适合存储服务和计算服务等，它具有以下特点，**小巧,快速,功能性强**。 

1. 小巧,文件仅**300多k**  
2. 快速,它具有**一流**的序列化和反序列化[性能](#1性能)  
3. 功能性强：
	* 可以获取对象被序列化后的大小而**不用完整**序列化对象
	* 可以读取对象中的某个元素而**不用完整**的反序列化对象
	* 可以更改对象中的某个元素而**不用完整**的序列化
	* 序列化后的格式具有**自描述性**

## 为什么需要?  

目前c#已经有很多二进制序列化器， 但这些序列化器都只提供了单一的序列化和反序列化功能。 
 
Bssom.Net采取了[Bssom协议](https://github.com/1996v/Bssom)， 使序列化后的数据具有**结构化特性**， 且拥有直接对字段进行编组的功能， 这使得Bssom.Net能做到其它序列化器所达不到的事情。  

* 当我想在序列化对象时知道对象被序列化后的大小, 以提前来选择该对象应该被序列化的正确位置(如数据库引擎的FSM算法)， 那么Bssom.Net能够满足你
* 当我拥有一个大的二进制数据， 但是我只想无合约的读取其中一个字段， 以避免完整的反序列化开销， 那么Bssom.Net能够满足你
* 当我拥有一个已经被序列化后的数据包， 我只想无合约的修改其中一个字段， 以避免重新序列化的开销， 那么Bssom.Net能够满足你
* 当我想让对象被序列化后仍能保留类型信息， 而不用依赖实体， 那么Bssom.Net能够满足你

### 什么是Bssom协议?
> [Bssom](https://github.com/1996v/Bssom)(Binary search algorithm structure model object binary marshalling)是一个使用二分查找算法模型对对象进行结构化编组的协议，被编组后的数据具有特殊的元数据信息，根据这些元数据信息可以高效的仅读取和更改对象中的某个元素，这样可以在对大对象进行序列化和反序列化的过程中不必因为只读取或只写入一个字段而造成完整的序列化开销。

## 大纲 ##
* [1.性能](#1性能)
* [2.读写器](#2读写器)
	* [IBssomBuffer](#ibssombuffer)
	* [IBssomBufferWriter](#ibssombufferwriter)
* [3.格式化器](#3格式化器)
* [4.解析器](#4解析器)
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
* [5.扩展](#5扩展)
* [6.高级API](#6高级API)
	* [BssomSerializer](#bssomserializer)
	* [BssomSerializerOptions](#bssomserializeroptions)
	* [BssomSerializeContext](#bssomserializecontext)
* [7.字段编组](#7字段编组)
	* [BssomFieldMarshaller](#BssomFieldMarshaller)
	* [简单字段访问语言](#简单字段访问语言)
	* [自定义字段访问形式接口](#自定义字段访问形式接口)
* [8.动态代码生成](#8动态代码生成)
* [9.特性](#9特性)
* [10.更多的可能性](#10更多的可能性)
* [11.如何使用](#11如何使用)
	* [Size](#size)
	* [Serialize](#serialize)
	* [Deserialize](#deserialize)
	* [ReadValue](#readvalue)
	* [ReadAllMapKeys](#readallmapkeys)
	* [TryWriteValue](#trywritevalue)
	* [如何使用特性](#如何使用特性)
	* [如何定义扩展](#如何定义扩展)
* [12.局限性](#12局限性)
	* [BssomSerializationArgumentException.BssomMapKeySame](#bssomserializationargumentexception.bssommapkeysame)
* [13.如何参与项目贡献](#13如何参与项目贡献)
* [14.谁在使用](#14谁在使用)

## 1.性能
![](https://user-images.githubusercontent.com/30827194/97228887-808af380-1812-11eb-846d-821ed0a7d978.png)
这里是与.NET平台下非常优秀的两款序列化程序([MessagePack](https://github.com/neuecc/MessagePack-CSharp) 和 [Protobuf-net](https://github.com/protobuf-net/protobuf-net))进行性能比较的基准。  

柱状数据代表执行相同任务所花费的时间，  **越低代表性能越快**，  折线数据代表执行相同任务所产生的GC，  **越低代表在执行中所产生的垃圾越少** ， 从性能比较结果可以看出Bssom.Net的性能是非常优异的。

Bssom.Net使用很多技术来提高性能：

* 使用**内存池**技术， 用于写入的内存可以**复用**
* 使用表达式和Emit**动态编程**技术，对类型进行了特殊处理， 且避免值类型装箱拆箱
* 使用泛型**静态缓存**， 避免了字典查找开销
* 包装了异常抛出代码， 以增加**内联**的可能性
* 更多的对**强类型**进行调用， 而不是接口抽象
* **预处理**Map2类型的元数据， 在序列化时不需要对其进行再次编码
* 在查找Map2键时， 提前**固定局部引用**， 而不是标准函数调用
* 解析Map1类型时， 自动构建8字节的**自动机**跳跃查找
* 值得一提的是， 出于减少依赖， 减少体积的目的， Bssom.Net并没有依赖`System.Memory.dll`， 因此无法使用`Span<T>`,`Memory<T>`等类型， 这意味着Bssom.Net的实现将无法使用`ByReference<T>`这一JIT内部特性， 因此目前的读写器将不具备读写局部化和去虚拟化及内联调用的这三个性能优化点 ( 但即使这样, 目前的Bssom.Net性能依然非常优秀 ) ， 若将来有可能支持`Span<T>`类型的话， 那么Bssom.Net将会通过一些额外的性能技巧来再次提升性能。

## 2.读写器

Bssom.Net对于读取和写入的入口并不是直接使用原生的`Byte[]`， 而是提供了缓冲区接口`IBssomBuffer`和写入器接口`IBssomBufferWriter`。
与原生的`byte[]`不同， 接口将更加灵活， 实现`IBssomBuffer`后可以从任意来源来读取数据， 实现`IBssomBufferWriter`后可以将数据写在任意地方(比如非连续的片段)。

### IBssomBuffer

`IBssomBuffer`是一个用于序列化的缓冲区接口， 提供了读取的行为。
方法         |     描述
------------|-----------
Position   | 缓冲区中的当前位置
ReadRef  | 从当前缓冲区中的位置读取指定大小序列的引用
 Seek    |  设置当前缓冲区的位置
SeekWithOutVerify | 设置当前缓冲区的位置， 并且不对position的边界进行验证
<a name="tryreadfixedref">TryReadFixedRef</a> |  尝试从当前缓冲区中的位置读取一个可以固定的字节序列的引用， 当进行Seek操作的时候不会影响被固定字节的引用位置
UnFixed |  用于取消由TryReadFixedRef所固定的引用, 此方法的调用始终和TryReadFixedRef对称

### IBssomBufferWriter

`IBssomBufferWriter`是基于缓冲区的写入接口， 提供了写入行为。
方法  | 描述
-----|-----
Buffered|在缓冲区中已写入字节的数目
Advance| 用于指示已写入缓冲区的部分
Position| 写入器的当前位置
Seek | 设置当前写入器的位置
SeekWithOutVerify | 设置当前写入器的位置， 并且不对Buffered的边界进行验证
GetRef | 从当前位置获取用于写入的字节序列的引用
CanGetSizeRefForProvidePerformanceInTryWrite | 在字段编组中， 当前位置是否能提供指定大小的字节序列引用以用来提供内部某些类型写入的性能
GetBssomBuffer | 获取当前写入器所使用的缓冲区

**Bssom.Net内部已经对`byte[]`， `Stream`进行了`IBssomBuffer`和`IBssomBufferWriter`接口的封装, 用户无需手动封装**

## 3.格式化器

**格式化**是Bssom.Net将.Net对象和Bssom格式进行互相转换的一个过程。 Bssom.Net通过`IBssomFormatter<T>`来实现对对象的格式化。
API  |  描述
-----|----------
Size |  获取对象被序列化后的大小
Serialize | 将对象序列化成Bssom二进制格式
Deserialize | 将Bssom二进制格式反序列化成对象

Bssom.Net内部已经内置了许多格式化器， 如.NET的基元类型， 键值对类型， 可迭代类型... 他们在`Bssom.Serializer.Formatters`命名空间下， 你可以找到它并直接调用它。  

如果你不需要特殊的处理某个类型的话， 那么这些格式化器基本可以覆盖你的大部分需求， 而如何找到格式化器, 这则是解析器所需要做的。

## 4.解析器

**解析**是将.Net类型对象**获取**到对应的**格式化器**的一个过程。Bssom.Net通过`IFormatterResolver`来实现对对象的解析。

API  | 描述
-----|------
GetFormatter | 获取对象的格式化器实例

解析器通常具备解析类型和保存格式化器这两种功能，  Bssom.Net中已实现的解析器在内部会对.net类型进行格式化器的查找，  然后通过静态泛型的特性缓存被找到的格式化器，  完成了将**一个或一组**.net类型**绑定**到对应的格式化器的这样过程.  

`IFormatterResolver`是Bssom.NET开始对对象序列化的最上层的入口， 他们在`Bssom.Serializer.Resolvers`命名空间下。

名称         |  描述 
------------|-------------
<a name="primitiveresolver">PrimitiveResolver</a> | 该解析器提供了`sbyte`,`Int16`,`Int32`,`Int64`,`byte`,`UInt16`,`UInt32`,`UInt64`,`Single`,`Double`,`bool`,`char`,`Guid`,`Decimal`,`string`,`DateTime`的类型的解析器
<a name="attributeformatterresolver">AttributeFormatterResolver</a> | 获取并提供用户自定义格式化器的实例
<a name="buildinresolver">BuildInResolver</a> | 提供了`StringBuilder`,`BitArray`,`DataTable`等类型的解析器
<a name="bssomvalueresolver">BssomValueResolver</a> | 提供了`BssomValue`类型的解析器
<a name="idictionaryresolver">IDictionaryResolver</a> | 获取和生成具有`IDictionary`行为的类型的解析器，  该解析器抽象了BCL中对于键值对定义的行为规则，  为满足该规则的对象进行动态解析代码的生成。在解析器内部，  将通过运行时的配置选项来选择`Map1`或`Map2`的两种格式
<a name="icollectionresolver">ICollectionResolver</a> | 获取和生成具有`IColloction`行为的类型的解析器，  该解析器抽象了BCL中对于收集器定义的行为规则，  为满足该规则的对象进行动态解析代码的生成。  在解析器内部，  如果集合中的元素类型为基元类型，  则将其解析成`Array1`格式，  否则解析为`Array2`格式
<a name="mapcodegenresolver">MapCodeGenResolver</a> | 获取和生成对象的公开字段和属性进行BssomMap类型编码的解析器，  若对象为接口，  则会自动生成该接口的实现作为反序列化的载体。在解析器内部，  始终将类型解析为`Map2`格式，  且提供`Map1`和`Map2`两种格式的反序列化代码
<a name="mapcodegenresolverallowprivate">MapCodeGenResolverAllowPrivate</a> | 获取和生成对象的所有(公开的和非公开的)字段和属性进行BssomMap类型编码的解析器，  若对象为接口，  则会自动生成该接口的实现作为反序列化的载体。在解析器内部，  始终将类型解析为`Map2`格式，  且提供`Map1`和`Map2`两种格式的反序列化代码
<a name="objectresolver">ObjectResolver</a> | 提供了`Object`类型的解析器
<a name="compositedresolver">CompositedResolver</a> | 复合解析器，组合了`Object`,`Primitive`,`Attribute`,`BssomValue`,`BuildIn`,`IDictionary`,`ICollection`,`MapCodeGen`解析器
<a name="compositedresolverallowprivate">CompositedResolverAllowPrivate</a> | 复合解析器，组合了`Object`,`Primitive`,`Attribute`,`BssomValue`,`BuildIn`,`IDictionary`,`ICollection`,`MapCodeGenAllowPrivate`解析器

>  因为`IDictionaryResolver`和`ICollectionResolver`中定义的足够抽象的规则，Bssom.Net不需要为未来.NET可能出现的新的`IDictionary`或`IColloction`实现而编写特定的解析代码
  

在Bssom.Net中可以通过`BssomSerializerOptions`中的`FormatterResolver`属性来注入序列化所需要的解析器，  默认为`CompositedResolver`,  `CompositedResolver`将会对类型依次从 `Object`,`Primitive`,`Attribute`,`BssomValue`,`BuildIn`,`IDictionary`,`ICollection`,`MapCodeGen`解析器中进行查找，  直到找到对应的解析器。

>  如果你想序列化对象中的私有字段，请使用`CompositedResolverAllowPrivate`，   在`BssomSerializerOptions`中可以直接使用`DefaultAllowPrivate`默认配置集

## 5.扩展

让我们看一下Bssom.Net序列化的过程：

     input T -> Call serialize(T) -> Find BssomResolver -> Provide type formatter -> formatter.Serialize(T);
    
在整个序列化的过程中， 每个步骤都是**透明**的，  这意味着若用户对Bssom.Net内部定义的解析器或格式化器不满意的话，  则可以自己扩展它。

用户可以自己通过实现`IFormatterResolver`和`IBssomFormatter`来**替代默认的解析器**， 在`Bssom.Serializer.Binary.BssomBinaryPrimitives`(在即将到来的小版本中将重构该类)和读写器本身所暴露的公开API中提供对Bssom格式的低级写入和读取实现。  

简单示例可以参考[更多可能介绍](#10更多的可能性)

## 6.高级API

### BssomSerializer

`BssomSerializer`是Bssom最上层的API， 在`Bssom.Serializer`命名空间下， 是Bssom开始工作的入口。 它的**静态方法**构成了Bssom.Net的**主要API**。

API     |    描述   |  重载
--------|----------|-------------
<a name="sizeapi">Size</a>    | 在不进行序列化的情况下， 获取对象被序列化后的二进制数据大小 |(t, option),(ref context, t)
<a name="serializeapi">Serialize</a> | 将给定的值序列化为Bssom二进制  | (byte[], t, option), (stream, t, option), (IBssomBufWriter, t, option), (ref context, t)
<a name="deserializeapi">Deserialize</a> | 将Bssom二进制数据反序列化成.net对象  | (byte[], option),(stream, option),(IBssomBuf, option),(ref context)
SerializeAsync | 异步的序列化给定的值为Bssom二进制  | 同上
DeserializeAsync | 异步的将Bssom二进制数据反序列化成.net对象  | 同上

### BssomSerializerOptions
`BssomSerializer`作为最上层的API，我们在调用它时，需要传递一个可空的`BssomSerializerOptions`类型的Option参数。  
`BssomSerializerOptions`是Bssom在整个序列化工作期间所需要使用的配置。  默认为`BssomSerializerOptions.Default`.  
-  **FormatterResolver** : 在Option中，你可以为`FormatterResolver`**注册解析器**， 如果没有手动注册， 则使用默认的`CompositedResolver`， Bssom将总是通过`FormatterResolver`来对类型进行解析.
-  **Security** : 这是用于序列化期间的**安全**相关选项，  目前仅提供了在反序列化期间对深度的验证，默认为 **不限制**
-  **IsPriorityToDeserializeObjectAsBssomValue** : 该选项决定了反序列化时是否将Object类型转换为BssomValue类型，  如果为`false`， 则默认反序列化为原生类型， 默认为`false`.
-  **IsUseStandardDateTime** : Bssom.Net对`DateTime`类型实现了标准的**Bssom协议Unix格式** 和 **.NET平台的本地格式**,  本地格式具有更少的字节，  但不具备和其它平台的交互性，  默认为`false`.
-  **IDictionaryIsSerializeMap1Type** : 此选项决定了对具有`IDictionary`行为的类型默认使用哪种格式进行序列化，  如果为`true`则使用`Map1`格式，  否则为`Map2`格式，  默认为`true`

### BssomSerializeContext
`BssomSerializeContext`提供了序列化期间所使用的上下文信息，  这其中也包括了`BssomSerializerOptions`
-  **BssomSerializerOptions** : 序列化期间所使用的**配置信息**
-  **ContextDataSlots**<a name="contextdataslots"></a> : 提供了一个数据槽，  供用户在序列化期间自己存储和读取的一个**存储介质**
-  **CancellationToken** : 一个序列化操作取消的标记，  用户可以**中途取消**正在进行的序列化操作

## 7.字段编组
Bssom.Net拥有读取字段而**不用完全反序列化**和更改值而不用完全序列化功能，  这是因为[Bssom协议](https://github.com/1996v/Bssom)有着良好的结构化特征，  在Bssom.Net的实现里，  这样的功能则暴露在`BssomFieldMarshaller`中.

### BssomFieldMarshaller
`BssomFieldMarshaller`提供一套API用于对被序列化后的数据进行**更低粒度**的控制.
API  |  描述   
-----|--------
IndexOf | 通过特殊的输入格式来获取被指定的对象在Bssom二进制中的位置,返回偏移量信息
ReadValue | 通过指定的偏移量信息来读取整个元素
ReadValueType | 通过指定的偏移量信息仅读取元素类型
ReadValueTypeCode | 通过指定的偏移量信息仅读取元素类型的二进制码
ReadValueSize | 通过指定的偏移量信息来获取元素在Bssom二进制中所存储的大小
ReadArrayCountByMapType | 通过指定的偏移量信息来读取BssomArray的元素数量
ReadAllKeysByMapType | 通过指定的偏移量信息来读取BssomMap中的元数据(包含Key和值的偏移量)
TryWrite | 通过指定的偏移量信息在Bssom二进制中重新对值进行写入，  若写入值的宽度大于被写入槽的宽度，则失败

每种方法都提供了 `byte[]` 和 `IBssomBuf` 的重载

### 简单字段访问语言
Bssom.Net为`IndexOf`定义了一种简单的字段访问语言，  该语言共定义了两种访问形式，  一种是访问`Map`类型(该Map类型的键必须为`String`类型)，  一种是访问`Array`类型.  两种访问形式可以自由组合.
-  [Key] : 代表通过`Key`来访问`Map`类型的值， 输入的`Key`只表示`String`类型
-  $Index : 代表通过下标来访问`Array`类型的元素，  输入的Index只能是整数类型  

假设有如下数据
```c#
{
   "Postcodes" : {   
		  "WuHan" : [430070,430071,430072,430073],
		  "XiangYang" : [441000,441001,441002]
		},
   "Province" : "HuBei"
}
```
可以通过如下方式进行元素访问， 在[示例](#readvalue)中可以了解更多细节
```c#
[Postcodes][WuHan]$1  => 4330071
[Province]  => "HuBei"
```

### 自定义字段访问形式接口

Bssom.Net为`IndexOf`提供了`IIndexOfInputSource`接口用来接收自定义的字段访问源，使用该接口后Map类型的Key将不再受限制，   Key可以为任意输入类型.  

`IndexOfObjectsInputSource` 是 Bssom.Net为用户提供的`IIndexOfInputSource`接口的**通用实现**。  它接收一组可迭代的对象，当调用IndexOf的时候，将依次对对象进行迭代.

假设有如下数据
```c#
{
   2018-01-01 : {
         0 : ["Rain1","Rain2","Rain3"],
         4 : ["Rain4","Fair5","Fair6"]   
    }
}
```
可以通过如下方式进行元素访问， 在[示例](#readvalue)中可以了解更多细节
```c#
new IndexOfObjectsInputSource(new Entry[]{ 
     new Entry(DateTime.Parse("2018-01-01"),ValueIsMapKey: true),
     new Entry(3,ValueIsMapKey: true),
     new Entry(1,ValueIsMapKey: false),
  })

output => "Fair5"
```

## 8.动态代码生成

Bssom.Net对`IDictionaryResolver`, `ICollectionResolver`, `MapCodeGenResolver`, `ObjectResolver` 使用了动态代码生成技术，通过**表达式树和Emit**共同生成运行时代码，如果应用程序是纯AOT环境，则将不支持.

在`MapCodeGenResolver`中对`Map1`类型的反序列化使用了以8字节(64位字长)为单位的类前缀树的自动机查找模式，这是非常有效且快速的方式，它避免了对字符串进行完全Hash运算以及字符比较开销，通过对`MapCodeGenResolver.Save()`方法你将看到这些自动生成的代码.  

![](https://user-images.githubusercontent.com/30827194/97230916-b2518980-1815-11eb-891d-12fee0f2fe0a.png)

`MapCodeGenResolver`中对`Map2`类型的反序列化则使用了内置的[Bssom协议](https://github.com/1996v/Bssom)的Map格式查找代码,该代码是状态机模式编写，分为快速和低速版，这取决于[读取器](#ibssombuffer)是否能够提供 [TryReadFixedRef](#tryreadfixedref).
![](https://user-images.githubusercontent.com/30827194/97229613-99e06f80-1813-11eb-98ca-db941ce3d6d3.png)

另外，对于`Size`方法，MapCodeGenResolver的处理也是非常快速的，因为它已经提前计算好了元数据的大小，并且内联了基元字段本身的固定大小.
![](https://user-images.githubusercontent.com/30827194/97229619-9e0c8d00-1813-11eb-8954-df92e96c7d18.png)

## 9.特性

Bssom.Net中目前拥有5个特性.
-  **AliasAttribute** : 别名特性，用于修改Map格式对象字段在二进制中所保存的字段名称
-  **BssomFormatterAttribute** : 自定义格式化特性，当字段属性或类型被该特性标记后，此类型的格式化将采用该特性所指定的格式化器
-  **IgnoreKeyAttribute** : 忽略某一个Key，序列化时将忽略被标记的字段，适用于Map格式
-  **OnlyIncludeAttribute** : 仅包含某一个Key，序列化时仅包含该Key，适用于Map格式，与`IgnoreKeyAttribute`作用相反，优先级更高
-  **SerializationConstructorAttribute** : 为类型的反序列化指定一个构造函数

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

Bssom.Net是无合约的，开箱即用，这里有些示例代码.

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
[BssomSerializer.Serialize](#serializeapi) 方法用于 将给定的值序列化为Bssom二进制，高性能的内部实现，以下是部分常用方法，每个方法都拥有CancellationToken的重载
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
[BssomSerializer.Deserialize](#deserializeapi) 方法用于 将给定的Bssom缓冲区反序列化为对象，高性能的内部实现，以下是部分常用方法，每个方法都拥有CancellationToken的重载
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
