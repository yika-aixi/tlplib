﻿using System;
using com.tinylabproductions.TLPLib.Functional;

namespace com.tinylabproductions.TLPLib.Extensions {
  public static class ArrayExts {
    public static A[] concat<A>(this A[] a, params A[][] others) {
      var total = others.sum(_ => _.Length).getOrElse(0);

      var self = new A[a.Length + total];
      a.CopyTo(self, 0);
      others.foldLeft(a.Length, (startIdx, arr) => {
        arr.CopyTo(self, startIdx);
        return startIdx + arr.Length;
      });
      return self;
    }

    /**
     * Basically LINQ #Select, but for arrays. Needed because of performance/iOS
     * limitations of AOT.
     **/
    public static To[] map<From, To>(
      this From[] source, Func<From, To> mapper
    ) {
      var target = new To[source.Length];
      for (var i = 0; i < source.Length; i++) target[i] = mapper(source[i]);
      return target;
    }
  }
}
