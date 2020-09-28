# Controlling an LED Bar Graph with .NET

[LED Bar Graphs](https://www.adafruit.com/product/1814) are a great device to use if you want a block of LEDs to control. A bar graph is a single package that includes a set of uniform LEDs, typically with an anode and cathode leg for each LED. [Single LEDs](https://www.adafruit.com/product/4204) can become a challenge to use since they all have to be separately mounted onto a breadboard or other device and can be difficult to align for an aesthetic presentation.

There are two options for controlling multiple LEDs:

- Assign a GPIO pin per LED and control the LEDs with those pins. This model is an extension of the approach used in the [Blink and LED](../led-blink/README.md) sample.
- Use a multiplexer to control multiple LEDs with a few pins (which control the multiplexer).

This sample uses a shift register (a kind of multiplexer) to control a 10 LED bar graph with just three pins. It also uses the [IOutputSegment](../../src/devices/Common/Iot/Device/Multiplexing/IOutputSegment.cs) interface to abstract the sample from the specific multiplexer used.
