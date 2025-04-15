using IMitto.Protocols;

namespace IMitto.Producers;

public record EmptyPackagedGoods(string Topic) : PackagedGoods(typeof(object), Topic);
