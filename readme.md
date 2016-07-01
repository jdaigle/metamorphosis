# Metamorphosis

[***Die Verwandlung***](https://en.wikipedia.org/wiki/The_Metamorphosis) is an
experimental .NET Client for [Apache Kafka](http://kafka.apache.org/).

The project aims to build a high performance client with a feature set similar
to [StackExchange.Redis](https://github.com/StackExchange/StackExchange.Redis).
Specifically: a
"High performance multiplexed design, allowing for efficient use of shared connections from multiple calling threads"
and
"Full dual programming model both synchronous and asynchronous usage, without requiring "sync over async" usage of the TPL".
In addition, Metamorphosis should fully abstract the management of consumers and consumer groups.

# Requirements

Metamorphosis currently is currently build with .NET 4.6.1

# Roadmap

* TODO

# Acknowledgements

This project utilizes the following open source projects:
* TODO

Some core code is derived from [AMQP 1.0 .NET Client Library](https://github.com/Azure/amqpnetlite/tree/45c1b6f289621)
which is licensed under [Apache Licence, Version 2.0](http://www.apache.org/licenses/LICENSE-2.0). The relevant code files have headers to indicate the attribution.