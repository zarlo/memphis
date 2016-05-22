# SharpOS

[![Join the chat at https://gitter.im/MichaelTheShifter/sharpos](https://badges.gitter.im/MichaelTheShifter/sharpos.svg)](https://gitter.im/MichaelTheShifter/sharpos?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

SharpOS is a lightweight DOS-like operating system built using C# and COSMOS.

## Goals

SharpOS may be DOS-like right now - with less than ten commands - but here are some of the goals.

 - *Full ShiftOS cross-compatibility* - I want to make it so running a .saa in ShiftOS and running a .saa in SharpOS will do the exact same thing - and I want ShiftOS lua scripts to be compatible with SharpOS. Another goal is to have ShiftOS skins compatible with SharpOS, and have a desktop environment that will support ShiftOS applications.
 - *Appscape Integration* - Another goal is to have full integration with the Appscape Package Manager - create an app for ShiftOS, and it'll work on SharpOS.
 - *And so much more*. With this much power, my brain is full of ideas.

## Compiling the code

First, you'll need the latest Cosmos Developer Kit. You can get it from cloning https://github.com/CosmosOS/Cosmos.git, and following the instructions [here](https://github.com/CosmosOS/Cosmos/wiki/How-to-install-Cosmos#devkit). Note that you'll need to make sure you have everything listed under their [Prerequisites](https://github.com/CosmosOS/Cosmos/wiki/How-to-install-Cosmos#pre-requisite-software).

After all of that is done, you should be able to compile and run inside VMWare, just by opening the SharpOS solution and striking the F5 key.

## Help! It won't boot!

Sometimes, this weird thing happens where an incorrect project is set as the startup one, and when you compile, the OS won't boot, but instead Visual Studio will say "You can't run this project because it isn't executable!" or something like that.

To fix it, simply find the "SharpOSBoot" project in the Solution Explorer, right-click, and click 'Set As StartUp Project', then hit F5 and see what happens.
