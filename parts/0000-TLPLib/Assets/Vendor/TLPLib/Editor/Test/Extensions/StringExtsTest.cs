﻿using System;
using com.tinylabproductions.TLPLib.Test;
using NUnit.Framework;

namespace com.tinylabproductions.TLPLib.Extensions {
  public class StringTestBase64Conversions {
    const string raw = "Aladdin:OpenSesame", encoded = "QWxhZGRpbjpPcGVuU2VzYW1l";

    [Test]
    public void toBase64() { raw.toBase64().shouldEqual(encoded); }

    [Test]
    public void fromBase64() { encoded.fromBase64().shouldEqual(raw); }
  }

  public class StringTestRepeat {
    [Test]
    public void whenTimesNegative() {
      Assert.Throws<ArgumentException>(() => "foo".repeat(-1));
    }

    [Test]
    public void whenTimesZero() {
      "foo".repeat(0).shouldEqual("");
    }

    [Test]
    public void whenTimesPositive() {
      var s = "foo";
      s.repeat(1).shouldEqual(s);
      s.repeat(3).shouldEqual("foofoofoo");
    }
  }

  public class StringTestEmptyness {
    [Test]
    public void isEmpty() {
      "".isEmpty().shouldBeTrue();
      "f".isEmpty().shouldBeFalse();
    }

    [Test]
    public void nonEmpty() {
      "".nonEmpty().shouldBeFalse();
      "f".nonEmpty().shouldBeTrue();
    }
  }

  public class StringTestEnsureStartsWith {
    [Test]
    public void WhenStarts() {
      "foobar".ensureStartsWith("foo").shouldEqual("foobar");
    }
    [Test]
    public void WhenDoesNotStart() {
      "bar".ensureStartsWith("foo").shouldEqual("foobar");
    }
  }
}