# transmitto
IMitto is a simple in-memory or distributed event bus for sending and receiving messages between
different parts of a system. It is designed to be lightweight and easy to use, making it suitable
for a variety of applications.

----
**message format**

| 0x0001 0x01 | 0x00000001 Headers (k:v\nk1:v1\n) |--0x00000001 Contents (Blob | Raw bytes)|

| Descriptor | Headers | Package |
|:-:|:-:|:-:|
| Action &#124; Modifier | HeadersLength &#124; Headers | ContentLength &#124; Content |
| 16 bits &#124; 4 bits &#124; 4 bits (reserved, unused) | 32 bit Length &#124; k:v\n | 32 bits &#124; Length |

---

**Action Descriptor**
- 2 bytes | 16 bits
  - Action
    - none=0
    - auth=1
    - produce=2
    - connect=4
    - disconnect=8
    - consume=16
    - stream=32
    - session=64


- 1 byte | 4 bits
  - Modifier
    - none=0
    - end=1
    - start=2
    - ack=4
    - nack=8
    - error=16
    - next=32

- 1 byte | 4 bits
  - Reserded

---

Quality of Service
- none for now

[comment]: # ( - 1 byte | 4 bits)
[comment]: # (   - Quality of Service)
[comment]: # (     - None=0)
[comment]: # (     - AtMostOnce=1)
[comment]: # (     - AtLeastOnce=2)
[comment]: # (     - ExactlyOnce=3)

----
**client | server workflow**

|  request | => |response| <= | sender|
|:-|:-:|:-|:-:|:-:|
|action|modifier|action|modifier||
|||||client|
|connect||connect||client|
|session|start &#124; end &#124; ack &#124; nack &#124; error|session|end &#124; ack &#124; nack &#124; error|client|
|auth||auth||client|
|consume|start &#124; end &#124; ack &#124; nack &#124; error|consume|end &#124; ack &#124; nack &#124; error|client &#124; server|
|produce||produce||client &#124; server|
|stream|start &#124; next &#124; end &#124; ack &#124; nack &#124; error|stream|end &#124; ack &#124; nack &#124; error|client &#124; server|
|disconnect||||client &#124; server|


---

**Headers**
- 4 bytes | 32 bits
  - Headers Length
    - Length of the headers in bytes


**Topics**
- topics fan out from `/abc/def/ghi` -> `/abc/def` -> `/abc` -> `/`
- topic consumer registration can include wildcards (*) `/abc/*/ghi*` -> `/abc/{any}/ghi`            
---