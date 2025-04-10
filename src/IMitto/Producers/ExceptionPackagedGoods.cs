using IMitto.Net;

namespace IMitto.Producers;

public record ExceptionPackagedGoods(string Topic, Exception Exception) : PackagedGoods(Exception.GetType(), default!, Topic);
