# IMITTO Protocol

IMITTO is a lightweight, binary message protocol designed for efficient in-memory and distributed messaging.
It is particularly well-suited for high-performance applications, such as game engines, real-time data processing systems,
and other scenarios where low latency and high throughput are critical.


- [Message Format](./docs/message-format.md)
- [Topics & Filters](./docs/topics-and-filters.md)
- [Workflow](./docs/workflow.md)
- [Future Enhancements](./docs/future.md)

---

# Technical Summary of the IMITTO Solution

The IMITTO solution implements a lightweight, binary message protocol designed for high-performance in-memory and distributed messaging. It is optimized for low latency and high throughput, making it suitable for real-time applications like game engines and data processing systems. Below is a breakdown of the key components and their roles:

---

## Core Protocol Components

### 1. Message Format
- **Defined in:** `docs/message-format.md`
- **Structure:**
  - **Version (1 byte):** Protocol version (e.g., `0x01`).
  - **Action (2 bytes):** Represents the operation (e.g., `Connect`, `Produce`).
  - **Modifier (1 byte):** Bitwise flags for message modifiers (e.g., `Start`, `End`).
  - **Header Count (1 byte):** Number of headers in the message.
  - **Headers:** Length-prefixed key-value pairs.
  - **Content Length (4 bytes):** Size of the payload.
  - **Content:** Binary or encoded payload (e.g., JSON, Protobuf).

---

## Protocol Implementation

### 2. Protocol Transport
- **File:** `src/IMitto/Protocols/V1/MittoProtocolTransport.cs`
- Implements the transport layer for the protocol.
- **Key Methods:**
  - `ReadPackageAsync`: Reads a message package from a `MittoPipeReader`.
  - `WritePackageAsync`: Writes a message package to a `MittoPipeWriter`.
  - `ReadCommandAsync`: Parses the command (version, action, modifier).
  - `ReadHeadersAsync`: Reads and deserializes headers.
  - `ReadContentAsync`: Reads the message content.

### 3. Protocol Base
- **File:** `src/IMitto/Protocols/MittoProtcolTransportBase.cs`
- Abstract base class for protocol transport implementations.
- Defines the contract for reading, writing, sending, and receiving protocol packages.

---

## Headers and Content

### 4. Header Serialization
- **File:** `src/IMitto/Protocols/MittoHeaderKeys.cs`
- Provides utilities for serializing and deserializing headers.
- Supports both known and custom headers.

### 5. Content Handling
- **File:** `src/IMitto/Protocols/V1/MittoContent.cs`
- Represents the content of a message.
- Includes methods for reading and writing binary content.

---

## Testing and Benchmarks

### 6. Protocol Tests
- **File:** `test/IMitto.Tests/Protocols/ProtocolTransportTests.cs`
- Validates the protocol's ability to serialize and deserialize messages.
- Tests various combinations of headers and body sizes (small, medium, large).

### 7. Benchmarks
- **File:** `benchmarks/IMitto.Benchmarks/BenchmarkDotNet.Artifacts/results/IMitto.Benchmarks.MiddlewareExecutorBenchmarks-report.html`
- Benchmarks the performance of middleware execution.
- **Results:**
  - `ExecuteAsync_NonGeneric`: ~30.89 µs for 1,000 iterations.
  - `ExecuteAsync_Generic`: ~34.57 µs for 1,000 iterations.

---

## Error and Status Handling

### 8. Status Management
- **File:** `src/IMitto/Protocols/Models/MittoStatusBody.cs`
- Represents the status of a message or operation.
- Includes predefined statuses like `Completed` and `Error`.
- Provides methods to create custom statuses with details.

### 9. Status Codes
- **File:** `src/IMitto/Protocols/Models/MittoStatus.cs`
- Defines success and error codes.
- Includes logic to determine if a code represents a successful operation.

---

## Channel Management

### 10. Channel Providers
- **File:** `src/IMitto/Channels/IMittoChannelProvider.cs`
- Provides interfaces for managing `ChannelReader` and `ChannelWriter`.
- Supports asynchronous communication between producers and consumers.

---

## Key Features
- **Binary Protocol:** Efficient for high-performance scenarios.
- **Extensibility:** Supports custom headers and flexible content formats.
- **Asynchronous Design:** Leverages `async/await` for non-blocking operations.
- **Error Handling:** Built-in support for error codes and detailed statuses.
- **Performance:** Benchmarked for low-latency operations.

---

This solution is well-structured for real-time messaging systems, with a focus on performance, extensibility, and reliability.
