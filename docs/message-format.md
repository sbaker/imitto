# IMITTO Message Format v0.2

Each MTTO message consists of the following components:

```
+-----------+---------+----------+----------------------------------------+
| Version   | Action  | Modifier | Header Count                           |
| (1 byte)  | (2 B)   | (1 byte) | (1 byte)                               |
+-----------+---------+----------+----------------------------------------+
| Headers: lengths for keys and values are bytes (256) max length         |
| Known Keys: (each: Key ID + Value, length-prefixed)                     |
| Unknown Keys: Key ID + Key (length-prefixed) + Value (length-prefixed)  |
+-------------------------------------------------------------------------+
| Content Length (4 bytes)                                                |
+-------------------------------------------------------------------------+
| Content (binary or encoded payload)                                     |
+-------------------------------------------------------------------------+
```

## Field Descriptions

- **Version (1 byte)**: Protocol version number (e.g., `0x01`).
- **Action (2 bytes)**: 16-bit code representing the action.
- **Modifier (1 byte)**: Bitwise flags for message modifiers.
- **Header Count (1 byte)**: Number of headers in the message.
- **Headers**: Sequence of length-prefixed key-value pairs.
- **Content Length (4 bytes)**: Payload size.
- **Content**: JSON, Protobuf, or binary payload.

## Modifier Flags
```
Bit Position | Modifier
-------------|----------
0            | None
1            | End
2            | Start
3            | Ack
4            | Nack
5-7          | Error
```
