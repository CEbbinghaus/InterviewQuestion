# SortableSorting Programming question (In Progress)

Given a newline delimited file of packets (A, B, C, D), Sort them such all packets belonging to a single request get emitted in order A -> B -> C -> D.

Packet format:
```
A::[EPK]::[Value?]
B::[PPK]::[EPK]::[Value?]
C::[PPK]::[EPK]::[Value?]
D::[PPK]::[Value?]
```

Each package sequence consists of exactly 1 `A`, 1 `B`, 1 `C` and 1 or More `D` packets. The `A` packet contains purely an `EPK` Id and `D` packets contain only `PPK`. 

Your job is to output incoming messages such that each unique sequence (EPK + PPK) is sent out in the correct order

As long as the order is kept the packets should be emitted as soon as possible so later packets that follow the order should be output instantly.

e.g
```
C::5::1::
B::6::2::
A::1::
B::5::1::
A::2::
D::5::
```
should result in:
```
A::1::
B::5::1::
C::5::1::
A::2::
B::6::2::
D::5::
```
being outputted to the screen. 

```
A::1::
B::5::1::
C::5::1::
A::2::
D::5::
D::5::
B::6::2::
D::5::
```
should be output as is as all the packets arrived in the correct order
