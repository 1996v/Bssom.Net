using System;

namespace Bssom.Serializer
{
    /// <summary>
    /// The exception thrown when formatting the type.
    /// </summary>
    public class BssomSerializationTypeFormatterException : BssomSerializationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BssomSerializationTypeFormatterException"/> class with a specified error message.
        /// </summary>
        public BssomSerializationTypeFormatterException(string message) : base(message)
        { }

        internal static void ThrowFormatterNotRegistered(Type type, IFormatterResolver resolver)
        {
            throw new BssomSerializationTypeFormatterException($"{type.Name}没有在{resolver.GetType().Name}中进行注册. {type.Name} is not registered in {resolver.GetType().Name}");
        }

        internal static BssomSerializationTypeFormatterException BssomMapKeyUnsupportedType(Type type)
        {
            throw new BssomSerializationTypeFormatterException($"Map2的Key类型不支持{type.Name} . Map2 Key type does not support {type.Name}.");
        }

        internal static T BssomMapKeyUnsupportedType<T>(Type type)
        {
            throw BssomMapKeyUnsupportedType(type);
        }

        internal static BssomSerializationTypeFormatterException Array3MembersMustDefindKeyAttribute(Type type, string memName)
        {
            return new BssomSerializationTypeFormatterException($"使用Array3Resolver在对{type.Name}类型格式化时发生了错误,被序列化的元素{memName}必须标记KeyAttribute或IgnoreKeyAttribute. An error occurred while formatting {type.Name} with Array3Resolver, and the serialized element:{memName} must be marked KeyAttribute or IgnoreKeyAttribute");
        }

        internal static BssomSerializationTypeFormatterException Array3KeyAttributeValueRepeated(Type type)
        {
            return new BssomSerializationTypeFormatterException($"使用Array3Resolver在对{type.Name}类型格式化时发生了错误,被KeyAttribute标记的元素的值重复. An error occurred using Array3Resolver while formatting the type {type.Name}, which was repeated by the value of the element tagged by KeyAttribute");
        }

        internal static BssomSerializationTypeFormatterException TypeFormatterError(Type type, string message)
        {
            return new BssomSerializationTypeFormatterException($"在对{type.Name}类型格式化时发生了错误. An error occurred while formatting the {type.Name}, {message}");
        }

        internal static BssomSerializationTypeFormatterException BuildNoPublicDynamicType(Type type)
        {
            throw new BssomSerializationTypeFormatterException($"动态构建时不允许该类型为非公开的{type.Name} . Building dynamic formatter only allows public type. Type: {type.Name}.");
        }

        internal static BssomSerializationTypeFormatterException UnsupportedType(Type type)
        {
            throw new BssomSerializationTypeFormatterException($"Map2的Key类型不支持{type.Name} . Map2 Key type does not support {type.Name}.");
        }

        internal static BssomSerializationTypeFormatterException AttributeFormatterTypeMismatch(Type formatterType, Type targetType)
        {
            throw new BssomSerializationTypeFormatterException($"FormatterAttribute的实现类型与目标类型不同. The implementation type of FormatterAttribute is different from the target type. FormatterAttribute: {formatterType} , TargetType: {targetType}");
        }

        internal static BssomSerializationTypeFormatterException CtorParasMismatch(Type type)
        {
            throw new BssomSerializationTypeFormatterException($"类型构造函数的参数数量或类型与属性值不同. The type constructor has a different number or type of argument than the property value. TypeName: {type.Name} ");
        }

        internal static BssomSerializationTypeFormatterException NotFoundSuitableCtor(Type type)
        {
            throw new BssomSerializationTypeFormatterException($"未找到合适的构造参数,只有公开的构造参数才能被选择. The appropriate construct parameter was not found and only the exposed construct parameter can be selected. TypeName: {type.Name}");
        }
    }
}
