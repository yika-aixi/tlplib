﻿using System;
using com.tinylabproductions.TLPLib.Concurrent;
using com.tinylabproductions.TLPLib.dispose;
using com.tinylabproductions.TLPLib.Extensions;
using com.tinylabproductions.TLPLib.Functional;
using com.tinylabproductions.TLPLib.Reactive;
using GenerationAttributes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace com.tinylabproductions.TLPLib.ResourceReference {
  public enum AssetLoadPriority : byte { Low, High }

  public partial class SingleAssetLoader<A> : IDisposable where A : Object {

    readonly DisposableTracker tracker = new DisposableTracker();
    readonly IRxRef<Option<ResourceRequest>> request = RxRef.a(F.none<ResourceRequest>());

    public readonly IRxRef<Option<AssetLoader<A>>> currentLoader = RxRef.a(Option<AssetLoader<A>>.None);
    public readonly IRxRef<AssetLoadPriority> priority = RxRef.a(AssetLoadPriority.High);
    public readonly IRxVal<Either<IsLoading, A>> assetState;

    const int
      PRIORITY_HIGH = 2,
      PRIORITY_LOW = 1,
      // We can't cancel loading, so we just set to lowest possible priority instead
      // Priority can't be set to negative value - got this info from error message
      PRIORITY_OFF = 0;

    [Record]
    public partial struct IsLoading {
      public readonly bool value;
    }

    public SingleAssetLoader() {
      assetState =
        currentLoader
          .flatMap(opt => {
            discardPreviousRequest();
            foreach (var binding in opt) {
              var (_request, assetFtr) = binding.loadAssetAsync();
              request.value = _request.some();
              return assetFtr.toRxVal().map(csOpt => csOpt.toRight(new IsLoading(true)));
            }

            return RxVal.cached(F.left<IsLoading, A>(new IsLoading(false)));
          });

      assetState.subscribe(tracker, e => {
        if (e.isRight) discardPreviousRequest();
      });

      currentLoader.zip(priority, request, (show, _priority, req) =>
        F.t(show.isSome ? (_priority == AssetLoadPriority.High ? PRIORITY_HIGH : PRIORITY_LOW) : PRIORITY_OFF, req)
      ).subscribe(tracker, tpl => {
        var (_priority, req) = tpl;
        foreach (var r in req) {
          r.priority = _priority;
        }
      });
    }

    void discardPreviousRequest() {
      foreach (var r in request.value) r.priority = PRIORITY_OFF;
      request.value = F.none<ResourceRequest>();
    }

    public void Dispose() {
      tracker.Dispose();
    }
  }
}