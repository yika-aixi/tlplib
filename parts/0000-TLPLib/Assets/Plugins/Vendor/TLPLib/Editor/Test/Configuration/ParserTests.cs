﻿ using com.tinylabproductions.TLPLib.Extensions;
using com.tinylabproductions.TLPLib.Functional;
using com.tinylabproductions.TLPLib.Test;
using NUnit.Framework;

namespace com.tinylabproductions.TLPLib.Configuration {
  class ConfigOptParserTest {
    readonly Config.Parser<Option<string>> parser = Config.opt(Config.stringParser);

    [Test]
    public void WhenNull() =>
      Config.parseJson(@"{""foo"": null}")
      .mapRight(_ => _.get("foo", parser))
      .shouldBeRight(Option<string>.None);

    [Test]
    public void WhenSome() =>
      Config.parseJson(@"{""foo"": ""bar""}")
      .mapRight(_ => _.get("foo", parser))
      .shouldBeRight("bar".some());

    [Test]
    public void WhenSomeBadType() =>
      Config.parseJson(@"{""foo"": 1}")
      .rightOrThrow.eitherGet("foo", parser)
      .leftOrThrow.kind.shouldEqual(ConfigLookupError.Kind.WRONG_TYPE);

    [Test]
    public void WhenNoKey() =>
      Config.parseJson(@"{""foo"": ""bar""}")
      .rightOrThrow.eitherGet("foo1", parser)
      .leftOrThrow.kind.shouldEqual(ConfigLookupError.Kind.KEY_NOT_FOUND);

  }
}
