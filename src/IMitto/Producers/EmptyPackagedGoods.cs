namespace IMitto.Producers;

public record EmptyPackagedGoods(string Topic) : PackagedGoods(typeof(object), Topic);
