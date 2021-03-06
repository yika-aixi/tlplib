﻿using System;
using System.Reflection;
using com.tinylabproductions.TLPLib.Data;
using com.tinylabproductions.TLPLib.Extensions;
using com.tinylabproductions.TLPLib.Functional;
using JetBrains.Annotations;

namespace com.tinylabproductions.TLPLib.reflection {
  public static class PrivateField {
    [PublicAPI]
    public static Fn<object, FieldType> getter<FieldType>(Type type, string fieldName) =>
      a => accessor<FieldType>(type, fieldName)(a).value;
    
    [PublicAPI]
    public static Fn<ObjectType, FieldType> getter<ObjectType, FieldType>(string fieldName) =>
      a => accessor<ObjectType, FieldType>(fieldName)(a).value;

    [PublicAPI]
    public static Fn<object, Ref<FieldType>> accessor<FieldType>(Type type, string fieldName) {
      var fieldInfo = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
      if (fieldInfo == null) throw new ArgumentException(
        $"Type {type} does not have non public instance field '{fieldName}'!"
      );

      return a => new LambdaRef<FieldType>(
        () => (FieldType) fieldInfo.GetValue(a),
        valueToSet => fieldInfo.SetValue(a, valueToSet)
      );
    }
    
    [PublicAPI]
    public static Fn<ObjectType, Ref<FieldType>> accessor<ObjectType, FieldType>(string fieldName) {
      var accessor = accessor<FieldType>(typeof(ObjectType), fieldName);
      return a => accessor(a);
    }
  }

  public static class PrivateMethod {
    [PublicAPI]
    public static Fn<object, object[], object> obtain(
      Type type, string methodName, Type[] argtypes, BindingFlags flags = BindingFlags.Instance
    ) {
      var methodInfo = type.GetMethod(
        methodName, flags | BindingFlags.NonPublic | BindingFlags.Public, Type.DefaultBinder,
        argtypes, F.emptyArray<ParameterModifier>()
      );
      if (methodInfo == null) throw new ArgumentException(
        $"Type {type} does not have non public method '{methodName}' with types {argtypes.mkStringEnum()}"
      );
      return (obj, args) => methodInfo.Invoke(obj, args);
    }
    
    [PublicAPI]
    public static Fn<object[], object> obtainStatic(
      Type type, string methodName, Type[] argtypes
    ) {
      var method = obtain(type, methodName, argtypes, BindingFlags.Static);
      return args => method(null, args);
    }

    [PublicAPI]
    public static Fn<A1, R> obtainStaticFn<A1, R>(
      Type type, string methodName
    ) {
      var method = obtainStatic(type, methodName, new[] {typeof(A1)});
      return a1 => (R) method(new object[] {a1});
    }

    [PublicAPI]
    public static Act<object> obtain(
      Type type, string methodName, BindingFlags flags = BindingFlags.Instance
    ) {
      var method = obtain(type, methodName, F.emptyArray<Type>(), flags);
      return obj => method(obj, new object[] {});
    }

    [PublicAPI]
    public static Act<object, A1, A2> obtain<A1, A2>(
      Type type, string methodName, BindingFlags flags = BindingFlags.Instance
    ) {
      var method = obtain(type, methodName, new[] {typeof(A1), typeof(A2)}, flags);
      return (obj, a1, a2) => method(obj, new object[] {a1, a2});
    }
  }

  public static class PrivateConstructor {
    public static Fn<object[], A> creator<A>() {
      var type = typeof(A);
      return args => (A) type.Assembly.CreateInstance(
          type.FullName, false,
          BindingFlags.Instance | BindingFlags.NonPublic,
          null, args, null, null
      );
    }
  }
}