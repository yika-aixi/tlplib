﻿using com.tinylabproductions.TLPLib.Concurrent;
using com.tinylabproductions.TLPLib.Concurrent.unity_web_request;
using com.tinylabproductions.TLPLib.Data;
using com.tinylabproductions.TLPLib.Functional;
using JetBrains.Annotations;
using UnityEngine.Networking;

namespace com.tinylabproductions.TLPLib.Extensions {
  public static class UnityWebRequestExts {
    [PublicAPI]
    public static Future<Either<WebRequestError, byte[]>> downloadToRam(
      this UnityWebRequest req, AcceptedResponseCodes acceptedResponseCodes
    ) {
      var handler = 
        req.downloadHandler is DownloadHandlerBuffer h 
          ? h 
          : new DownloadHandlerBuffer();
      req.downloadHandler = handler;
      return req.toFuture(acceptedResponseCodes, _ => handler.data);
    }

    [PublicAPI]
    public static Future<Either<ErrorMsg, byte[]>> downloadToRamSimpleError(
      this UnityWebRequest req, AcceptedResponseCodes acceptedResponseCodes
    ) => req.downloadToRam(acceptedResponseCodes).map(_ => _.mapLeft(err => err.simplify));
  }
}