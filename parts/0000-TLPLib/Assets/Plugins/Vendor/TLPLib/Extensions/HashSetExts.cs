﻿using System.Collections.Generic;
using com.tinylabproductions.TLPLib.Functional;
using JetBrains.Annotations;

namespace com.tinylabproductions.TLPLib.Extensions {
  public static class HashSetExts {
    [PublicAPI]
    public static Option<A> headOption<A>(this HashSet<A> enumerable) {
      foreach (var a in enumerable)
        return F.some(a);
      return F.none<A>();
    }
  }
}