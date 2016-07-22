# Memphis

Memphis is a lightweight command-line operating system built using C# and COSMOS.

## Goals

Memphis may be command-line right now - with less than ten commands - but here are some of the goals.

 - *Full ShiftOS cross-compatibility* - I want to make it so running a .saa in ShiftOS and running a .saa in Memphis will do the exact same thing - and I want ShiftOS lua scripts to be compatible with Memphis. Another goal is to have ShiftOS skins compatible with Memphis, and have a desktop environment that will support ShiftOS applications.
 - *Appscape Integration* - Another goal is to have full integration with the Appscape Package Manager - create an app for ShiftOS, and it'll work on Memphis.
 - *And so much more*. With this much power, my brain is full of ideas.

## Compiling the code

First, you'll need the latest Cosmos Developer Kit. You can get it from cloning https://github.com/CosmosOS/Cosmos.git, and following the instructions [here](https://github.com/CosmosOS/Cosmos/wiki/How-to-install-Cosmos#devkit). Note that you'll need to make sure you have everything listed under their [Prerequisites](https://github.com/CosmosOS/Cosmos/wiki/How-to-install-Cosmos#pre-requisite-software).

After all of that is done, you should be able to compile and run inside VMWare, just by opening the Memphis solution and striking the F5 key.

***NOTE***: As found by @CaveSponge and @jp2masa on the [Cosmos Gitter](http://gitter.im/CosmosOS/Cosmos), there is a build bug where Memphis may throw you a "MemphisBoot.bin not found" error when compiling. To fix this, in Visual Studio, right click the solution inside the Solution Explorer, go to Configuration Manager, and under the Build section make sure all checkboxes are **enabled**. Then, simply rebuild the solution (right click solution in Solution Explorer and click Rebuild Solution) and you should be good to go.

## Help! It won't boot!

Sometimes, this weird thing happens where an incorrect project is set as the startup one, and when you compile, the OS won't boot, but instead Visual Studio will say "You can't run this project because it isn't executable!" or something like that.

To fix it, simply find the "MemphisBoot" project in the Solution Explorer, right-click, and click 'Set As StartUp Project', then hit F5 and see what happens.
